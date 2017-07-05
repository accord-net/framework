﻿// Accord Statistics Library
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
    using Accord.Math.Optimization;
    using Accord.Statistics.Models.Fields.Features;
    using Accord.Statistics.Models.Fields.Functions;
    using System;
using System.Threading;

    /// <summary>
    ///   Quasi-Newton (L-BFGS) learning algorithm for <see cref="ConditionalRandomField{T}">
    ///   Conditional Hidden Fields</see>.
    /// </summary>
    /// 
    public class QuasiNewtonLearning<T> : 
        ISupervisedLearning<ConditionalRandomField<T>, T[], int[]>,
        IConditionalRandomFieldLearning<T>
    {
        [NonSerialized]
        CancellationToken token = new CancellationToken();

        private BoundedBroydenFletcherGoldfarbShanno lbfgs;
        private ConditionalRandomField<T> model;

        /// <summary>
        /// Gets or sets a cancellation token that can be used to
        /// stop the learning algorithm while it is running.
        /// </summary>
        /// 
        public CancellationToken Token
        {
            get { return token; }
            set { token = value; }
        }

        /// <summary>
        ///   Constructs a new L-BFGS learning algorithm.
        /// </summary>
        /// 
        public QuasiNewtonLearning(ConditionalRandomField<T> model)
        {
            this.model = model;
            this.lbfgs = new BoundedBroydenFletcherGoldfarbShanno(model.Function.Weights.Length);
            this.lbfgs.FunctionTolerance = 1e-3;

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
            double f;
            double[] g;

            lbfgs.Function = parameters =>
            {
                model.Function.Weights = parameters;
                f = -model.LogLikelihood(observations, labels);
                return f;
            };

            lbfgs.Gradient = parameters =>
            {
                model.Function.Weights = parameters;
                g = gradient(observations, labels);
                return g;
            };

            lbfgs.Token = Token;

            if (lbfgs.Minimize(model.Function.Weights))
                model.Function.Weights = lbfgs.Solution;

            return model.LogLikelihood(observations, labels);
        }



        private double[] gradient(T[][] observations, int[][] labels)
        {
            int N = observations.Length;

            var function = model.Function;
            var states = model.States;

            double[] g = new double[function.Weights.Length];


            // Compute sequence probabilities
            var P = new double[N][,][];
            for (int i = 0; i < N; i++)
            {
                var Pi = P[i] = new double[states + 1, states][];

                T[] x = observations[i];
                var fwd = ForwardBackwardAlgorithm.Forward(function.Factors[0], x, 0);
                var bwd = ForwardBackwardAlgorithm.Backward(function.Factors[0], x, 0);
                double z = partition(fwd, x);

                for (int prev = -1; prev < states; prev++)
                {
                    for (int next = 0; next < states; next++)
                    {
                        double[] Pis = new double[x.Length];
                        for (int t = 0; t < x.Length; t++)
                            Pis[t] = p(prev, next, x, t, fwd, bwd, function) / z;

                        Pi[prev + 1, next] = Pis;
                    }
                }
            }

            // Compute the gradient w.r.t. each feature
            //  function in the model's potential function.
            for (int k = 0; k < g.Length; k++)
            {
                var feature = function.Features[k];

                double sum1 = 0.0, sum2 = 0.0;
                for (int i = 0; i < N; i++)
                {
                    T[] x = observations[i];
                    int[] y = labels[i];
                    var Pi = P[i];

                    // Compute first term of the partial derivative
                    sum1 += feature.Compute(-1, y[0], x, 0);
                    for (int t = 1; t < x.Length; t++)
                        sum1 += feature.Compute(y[t - 1], y[t], x, t);

                    // Compute second term of the partial derivative
                    for (int prev = -1; prev < states; prev++)
                    {
                        for (int next = 0; next < states; next++)
                        {
                            double[] Pis = Pi[prev + 1, next];

                            for (int t = 0; t < Pis.Length; t++)
                                sum2 += feature.Compute(prev, next, x, t) * Pis[t];
                        }
                    }
                }

                g[k] = -(sum1 - sum2);
            }

            return g;
        }

        private static double p(int previous, int next, T[] x, int t,
            double[,] fwd, double[,] bwd, IPotentialFunction<T> function)
        {
            // This is the probability of beginning at the start of the
            // sequence, evaluating the sequence until the instant at
            // which the label "previous" happened, computing a transition
            // from "previous" to "next" and then continuing until the end
            // of the sequence.

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

        private static double partition(double[,] fwd, T[] x)
        {
            int T = x.Length - 1;
            int states = fwd.GetLength(1);

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

            run(x, y);
            return model;
        }
    }
}
