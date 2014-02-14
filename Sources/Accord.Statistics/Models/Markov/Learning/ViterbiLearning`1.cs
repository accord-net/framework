// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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
    using System;
    using System.Collections.Generic;
    using Accord.Math;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;

    /// <summary>
    ///   Viterbi learning algorithm.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Viterbi learning algorithm is an alternate learning algorithms
    ///   for hidden Markov models. It works by obtaining the Viterbi path
    ///   for the set of training observation sequences and then computing
    ///   the maximum likelihood estimates for the model parameters. Those
    ///   operations are repeated iteratively until model convergence.</para>
    ///   
    /// <para>
    ///   The Viterbi learning algorithm is also known as the Segmental K-Means
    ///   algorithm.</para>
    /// 
    /// <see cref="ViterbiLearning"/>
    /// <see cref="BaumWelchLearning{TDistribution}"/>
    /// 
    /// </remarks>
    /// 
    public class ViterbiLearning<TDistribution> : IUnsupervisedLearning, IConvergenceLearning
        where TDistribution : IDistribution
    {

        private AbsoluteConvergence convergence;
        private MaximumLikelihoodLearning<TDistribution> mle;

        /// <summary>
        ///   Gets the model being trained.
        /// </summary>
        /// 
        public HiddenMarkovModel<TDistribution> Model
        {
            get { return mle.Model; }
        }

        /// <summary>
        ///   Gets or sets the distribution fitting options
        ///   to use when estimating distribution densities
        ///   during learning.
        /// </summary>
        /// <value>The distribution fitting options.</value>
        /// 
        public IFittingOptions FittingOptions
        {
            get { return mle.FittingOptions; }
            set { mle.FittingOptions = value; }
        }

        /// <summary>
        ///   Gets or sets whether to use Laplace's rule
        ///   of succession to avoid zero probabilities.
        /// </summary>
        /// 
        /// <remarks>
        ///   When this property is set, it will only affect the estimation
        ///   of the transition and initial state probabilities. To control
        ///   the estimation of the emission probabilities, please use the
        ///   corresponding <see cref="FittingOptions"/> property.
        /// </remarks>
        /// 
        public bool UseLaplaceRule
        {
            get { return mle.UseLaplaceRule; }
            set { mle.UseLaplaceRule = value; }
        }

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
        public int Iterations
        {
            get { return convergence.Iterations; }
            set { convergence.Iterations = value; }
        }

        /// <summary>
        ///   Creates a new instance of the Viterbi learning algorithm.
        /// </summary>
        /// 
        public ViterbiLearning(HiddenMarkovModel<TDistribution> model)
        {
            this.convergence = new AbsoluteConvergence();
            this.mle = new MaximumLikelihoodLearning<TDistribution>(model);
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
        public double Run(params Array[] observations)
        {
            var model = mle.Model;
            convergence.Clear();

            // Convert the generic representation to a vector of multivariate sequences
            double[][][] vectorObservations = new double[observations.Length][][];
            for (int i = 0; i < observations.Length; i++)
                vectorObservations[i] = convert(observations[i], model.Dimension);


            double logLikelihood = Double.NegativeInfinity;
            for (int i = 0; i < observations.Length; i++)
                logLikelihood = Special.LogSum(logLikelihood, model.Evaluate(observations[i]));

            double newLogLikelihood = Double.NegativeInfinity;

            do // Until convergence or max iterations is reached
            {
                logLikelihood = newLogLikelihood;

                // Compute the Viterbi path for all sequences
                int[][] paths = new int[observations.Length][];
                for (int i = 0; i < observations.Length; i++)
                    paths[i] = model.Decode(vectorObservations[i]);

                // Compute Maximum Likelihood Estimation 
                mle.Run(vectorObservations, paths);

                // Compute log-likelihood
                newLogLikelihood = Double.NegativeInfinity;
                for (int i = 0; i < observations.Length; i++)
                    newLogLikelihood = Special.LogSum(newLogLikelihood, model.Evaluate(observations[i]));

                // Check convergence
                convergence.NewValue = newLogLikelihood;

            } while (!convergence.HasConverged);

            return newLogLikelihood;
        }


        /// <summary>
        ///   Converts a univariate or multivariate array
        ///   of observations into a two-dimensional jagged array.
        /// </summary>
        /// 
        private static double[][] convert(Array array, int dimension)
        {
            double[][] multivariate = array as double[][];
            if (multivariate != null) return multivariate;

            double[] univariate = array as double[];
            if (univariate != null) return Accord.Math.Matrix.Split(univariate, dimension);

            throw new ArgumentException("Invalid array argument type.", "array");
        }

    }
}
