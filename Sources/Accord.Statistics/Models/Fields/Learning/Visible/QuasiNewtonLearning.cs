// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//
#pragma warning disable 612, 618

namespace Accord.Statistics.Models.Fields.Learning
{
    using Accord.MachineLearning;
    using Accord.Math;
    using Accord.Math.Optimization;
    using Accord.Statistics.Models.Fields.Features;
    using Accord.Statistics.Models.Fields.Functions;
    using System;
    using System.Linq;
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Quasi-Newton (L-BFGS) learning algorithm for <see cref="ConditionalRandomField{T}">
    ///   Conditional Hidden Fields</see>.
    /// </summary>
    /// 
    public class QuasiNewtonLearning<T> : ParallelLearningBase,
        ISupervisedLearning<ConditionalRandomField<T>, T[], int[]>,
        IConditionalRandomFieldLearning<T>, IConvergenceLearning
    {

        private BoundedBroydenFletcherGoldfarbShanno lbfgs;

        /// <summary>
        ///   Gets or sets the model being trained.
        /// </summary>
        /// 
        public ConditionalRandomField<T> Model { get; set; }

        /// <summary>
        ///   Gets or sets the potential function to use if this learning
        ///   algorithm needs to create a new <see cref="ConditionalRandomField{T}"/>.
        /// </summary>
        /// 
        public IPotentialFunction<T> Function { get; set; }

        /// <summary>
        /// Gets or sets the tolerance value used to determine
        /// whether the algorithm has converged.
        /// </summary>
        /// <value>The tolerance.</value>
        public double Tolerance { get; set; }

        int IConvergenceLearning.Iterations
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the maximum number of iterations
        /// performed by the learning algorithm.
        /// </summary>
        /// <value>The maximum iterations.</value>
        public int MaxIterations { get; set; }

        /// <summary>
        /// Gets the current iteration number.
        /// </summary>
        /// <value>The current iteration.</value>
        public int CurrentIteration
        {
            get
            {
                if (lbfgs == null)
                    return lbfgs.Iterations;
                return 0;
            }
        }

        /// <summary>
        /// Gets or sets whether the algorithm has converged.
        /// </summary>
        /// <value><c>true</c> if this instance has converged; otherwise, <c>false</c>.</value>
        public bool HasConverged
        {
            get
            {
                return lbfgs.Status == BoundedBroydenFletcherGoldfarbShannoStatus.FunctionConvergence
                    || lbfgs.Status == BoundedBroydenFletcherGoldfarbShannoStatus.GradientConvergence;
            }
        }


        /// <summary>
        ///   Constructs a new L-BFGS learning algorithm.
        /// </summary>
        /// 
        public QuasiNewtonLearning(ConditionalRandomField<T> model)
            : this()
        {
            this.Model = model;
            init();
        }

        /// <summary>
        ///   Constructs a new L-BFGS learning algorithm.
        /// </summary>
        /// 
        public QuasiNewtonLearning()
        {
            this.Tolerance = 1e-3;
        }

        private void init()
        {
            this.lbfgs = new BoundedBroydenFletcherGoldfarbShanno(Model.Function.Weights.Length);
            this.lbfgs.FunctionTolerance = Tolerance;
            this.lbfgs.MaxIterations = MaxIterations;

            for (int i = 0; i < lbfgs.UpperBounds.Length; i++)
            {
                lbfgs.UpperBounds[i] = 1e10;
                lbfgs.LowerBounds[i] = -1e100;
            }
        }

        /// <summary>
        ///   Runs the learning algorithm with the specified input
        ///   training observations and corresponding output labels.
        /// </summary>
        /// 
        /// <param name="observations">The training observations.</param>
        /// <param name="labels">The observation's labels.</param>
        /// 
        [Obsolete("Please use Learn(x, y) instead.")]
        public double Run(T[][] observations, int[][] labels)
        {
            return run(observations, labels);
        }

        private double run(T[][] observations, int[][] labels)
        {
            int n = observations.Length;
            int numberOfParameters = Model.Function.Weights.Length;
            int states = Model.States;

            double[] g = new double[numberOfParameters];

            var model = Model;

            lbfgs.Function = parameters =>
            {
                model.Function.Weights = parameters;
                return -model.LogLikelihood(observations, labels);
            };

            lbfgs.Gradient = parameters =>
            {
                model.Function.Weights = parameters;
                gradient(observations, labels, g);
                return g;
            };

            lbfgs.Token = Token;

            if (lbfgs.Minimize(model.Function.Weights))
                model.Function.Weights = lbfgs.Solution;

            return model.LogLikelihood(observations, labels);
        }



        private double[] gradient(T[][] observations, int[][] labels, double[] g)
        {
            var model = Model;
            var function = model.Function;
            int states = model.States;
            int n = observations.Length;
            int d = Model.Function.Weights.Length;
            int Tmax = observations.Max(x => x.Length);
            int progress = 0;

            g.Clear();


            // Compute sequence probabilities
            Parallel.For(0, observations.Length, ParallelOptions,

                () =>
                {
                    // Create thread-local storage
                    var work = new double[states + 1, states][];
                    for (int j = 0; j < states + 1; j++)
                        for (int k = 0; k < states; k++)
                            work[j, k] = new double[Tmax];

                    return new
                    {
                        bwd = new double[Tmax, states],
                        fwd = new double[Tmax, states],
                        sum1 = new double[d],
                        sum2 = new double[d],
                        work = work,
                        count = new int[] { 0 }
                    };
                },

                (i, state, local) =>
                {
                    T[] x = observations[i];
                    var fwd = local.fwd;
                    var bwd = local.bwd;
                    var sum1 = local.sum1;
                    var sum2 = local.sum2;
                    var work = local.work;
                    ForwardBackwardAlgorithm.Forward(function.Factors[0], x, fwd);
                    ForwardBackwardAlgorithm.Backward(function.Factors[0], x, bwd);
                    double z = partition(fwd, x);

                    for (int prev = -1; prev < states; prev++)
                    {
                        for (int next = 0; next < states; next++)
                        {
                            double[] Pis = work[prev + 1, next];
                            for (int t = 0; t < x.Length; t++)
                                Pis[t] = p(prev, next, x, t, fwd, bwd, function) / z;
                        }
                    }

                    // Compute the gradient w.r.t. each feature
                    //  function in the model's potential function.

                    int[] y = labels[i];

                    Parallel.For(0, g.Length, ParallelOptions, k =>
                    {
                        IFeature<T> feature = function.Features[k];

                        // Compute first term of the partial derivative
                        sum1[k] += feature.Compute(-1, y[0], x, 0);
                        for (int t = 1; t < x.Length; t++)
                            sum1[k] += feature.Compute(y[t - 1], y[t], x, t);

                        // Compute second term of the partial derivative
                        for (int prev = -1; prev < states; prev++)
                        {
                            for (int next = 0; next < states; next++)
                            {
                                double[] Pis = work[prev + 1, next];
                                for (int t = 0; t < Pis.Length; t++)
                                    sum2[k] += feature.Compute(prev, next, x, t) * Pis[t];
                            }
                        }
                    });

                    local.count[0]++;
                    return local;
                },

                (local) =>
                {
                    lock (g)
                    {
                        for (int k = 0; k < g.Length; k++)
                            g[k] -= (local.sum1[k] - local.sum2[k]);
                        progress += local.count[0];
                    }
                }
            );

            return g;
        }

        private static double p(int previous, int next, T[] x, int t,
            double[,] fwd, double[,] bwd, IPotentialFunction<T> function)
        {
            // This is the probability of beginning at the start of the sequence, 
            // evaluating the sequence until the instant at which the label "previous"
            // happened, computing a transition from "previous" to "next" and then 
            // continuing until the end of the sequence.

            double p = 1.0;

            // Evaluate until the instant t - 1
            if (t > 0 && previous >= 0)
                p *= fwd[t - 1, previous];
            else if (t == 0 && previous == -1)
                p = 1.0;
            else return 0;

            // Compute a transition from "previous" to "next"
            p *= Math.Exp(function.Factors[0].Compute(previous, next, x, t));

            // Continue until the end of the sequence
            p *= bwd[t, next];

            return p;
        }

        private double partition(double[,] fwd, T[] x)
        {
            int T = x.Length - 1;
            int states = Model.States;

            double z = 0.0;
            for (int i = 0; i < states; i++)
                z += fwd[T, i];

            return z;
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        public ConditionalRandomField<T> Learn(T[][] x, int[][] y, double[] weights = null)
        {
            if (weights != null)
                throw new ArgumentException(Accord.Properties.Resources.NotSupportedWeights, "weights");

            if (Model == null)
            {
                Model = new ConditionalRandomField<T>(states: y.Max() + 1, function: Function);
                init();
            }

            run(x, y);

            return Model;
        }
    }
}
