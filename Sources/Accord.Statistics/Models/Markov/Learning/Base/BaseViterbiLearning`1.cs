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

namespace Accord.Statistics.Models.Markov.Learning
{
    using Accord.Math;
    using System;
    using Accord.Compat;
    using System.Threading;

    /// <summary>
    ///   Base class for implementations of the Viterbi learning algorithm.
    ///   This class cannot be instantiated.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class uses a template method pattern so specialized classes
    ///   can be written for each kind of hidden Markov model emission density
    ///   (either discrete or continuous).</para>
    ///   
    /// <para>
    ///   For the actual Viterbi classes, please refer to <see cref="ViterbiLearning"/>
    ///   or <see cref="ViterbiLearning{T}"/>. For other kinds of algorithms, please
    ///   see <see cref="BaumWelchLearning"/> and <see cref="MaximumLikelihoodLearning"/>
    ///   and their generic counter-parts.</para>
    /// </remarks>
    /// 
    /// <seealso cref="ViterbiLearning"/>
    /// <seealso cref="ViterbiLearning{T}"/>
    /// 
    public abstract class BaseViterbiLearning<T>
    {

        private RelativeConvergence convergence;
        private int batches = 1;

        /// <summary>
        ///   Gets or sets a cancellation token that can be used to
        ///   stop the learning algorithm while it is running.
        /// </summary>
        /// 
        public virtual CancellationToken Token { get; set; }

        /// <summary>
        ///   Gets or sets the maximum change in the average log-likelihood
        ///   after an iteration of the algorithm used to detect convergence.
        /// </summary>
        /// 
        /// <remarks>
        ///   This is the likelihood convergence limit L between two iterations of the algorithm. The
        ///   algorithm will stop when the change in the likelihood for two consecutive iterations
        ///   has not changed by more than L percent of the likelihood. If left as zero, the
        ///   algorithm will ignore this parameter and iterate over a number of fixed iterations
        ///   specified by the previous parameter.
        /// </remarks>
        /// 
        public double Tolerance
        {
            get { return convergence.Tolerance; }
            set { convergence.Tolerance = value; }
        }

        /// <summary>
        ///   Please use MaxIterations instead.
        /// </summary>
        /// 
        [Obsolete("Please use MaxIterations instead.")]
        public int Iterations
        {
            get { return convergence.MaxIterations; }
            set { convergence.MaxIterations = value; }
        }

        /// <summary>
        ///   Gets or sets the maximum number of iterations
        ///   performed by the learning algorithm.
        /// </summary>
        /// 
        /// <remarks>
        ///   This is the maximum number of iterations to be performed by the learning algorithm. If
        ///   specified as zero, the algorithm will learn until convergence of the model average
        ///   likelihood respecting the desired limit.
        /// </remarks>
        /// 
        public int MaxIterations
        {
            get { return convergence.MaxIterations; }
            set { convergence.MaxIterations = value; }
        }

        /// <summary>
        /// Gets the current iteration.
        /// </summary>
        /// <value>The current iteration.</value>
        public int CurrentIteration
        {
            get { return convergence.CurrentIteration; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has converged.
        /// </summary>
        /// <value><c>true</c> if this instance has converged; otherwise, <c>false</c>.</value>
        public bool HasConverged
        {
            get { return convergence.HasConverged; }
        }


        /// <summary>
        ///   Gets or sets on how many batches the learning data should be divided during learning.
        ///   Batches are used to estimate adequately the first models so they can better compute
        ///   the Viterbi paths for subsequent passes of the algorithm. Default is 1.
        /// </summary>
        /// 
        public int Batches
        {
            get { return batches; }
            set { batches = value; }
        }

        /// <summary>
        ///   Creates a new instance of the Viterbi learning algorithm.
        /// </summary>
        /// 
        protected BaseViterbiLearning()
        {
            this.convergence = new RelativeConvergence();
        }

        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        /// <remarks>
        ///   Learning problem. Given some training observation sequences O = {o1, o2, ..., oK}
        ///   and general structure of HMM (numbers of hidden and visible states), determine
        ///   HMM parameters M = (A, B, pi) that best fit training data. 
        /// </remarks>
        /// 
        public double Run(params T[] observations)
        {
            convergence.Clear();

            double newLogLikelihood = Double.NegativeInfinity;
            int[][] paths = new int[observations.Length][];


            do // Until convergence or max iterations is reached
            {
                if (Token.IsCancellationRequested)
                    return newLogLikelihood;

                if (batches == 1)
                {
                    RunEpoch(observations, paths);
                }
                else
                {
                    // Divide in batches
                    int[] groups = Classes.Random(observations.Length, batches);

                    // For each batch
                    for (int j = 0; j < batches; j++)
                    {
                        var idx = groups.Find(x => x == j);
                        var inputs = observations.Get(idx);
                        var outputs = paths.Get(idx);
                        RunEpoch(inputs, outputs);
                    }
                }

                // Compute log-likelihood
                newLogLikelihood = ComputeLogLikelihood(observations);

                // Check convergence
                convergence.NewValue = newLogLikelihood;

            } while (!convergence.HasConverged && !Token.IsCancellationRequested);

            return newLogLikelihood;
        }

        /// <summary>
        ///   Computes the log-likelihood for the current model for the given observations.
        /// </summary>
        /// 
        /// <param name="observations">The observation vectors.</param>
        /// 
        /// <returns>The log-likelihood of the observations belonging to the model.</returns>
        /// 
        protected abstract double ComputeLogLikelihood(T[] observations);

        /// <summary>
        ///   Runs one single epoch (iteration) of the learning algorithm.
        /// </summary>
        /// 
        /// <param name="inputs">The observation sequences.</param>
        /// <param name="outputs">A vector to be populated with the decoded Viterbi sequences.</param>
        /// 
        protected abstract void RunEpoch(T[] inputs, int[][] outputs);


    }
}
