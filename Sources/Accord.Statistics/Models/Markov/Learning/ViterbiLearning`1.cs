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
    using System;
    using System.Collections.Generic;
    using Accord.Math;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
#pragma warning disable 612, 618

    /// <summary>
    ///   Obsolete. Please use ViterbiLearning&lt;TDistribution, TObservation> instead.
    /// </summary>
    /// 
    [Obsolete("Please use ViterbiLearning<TDistribution, TObservation> instead.")]
    public class ViterbiLearning<TDistribution> : BaseViterbiLearning<double[][]>,
        IUnsupervisedLearning, IConvergenceLearning
        where TDistribution : IDistribution
    {

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
        ///   Creates a new instance of the Viterbi learning algorithm.
        /// </summary>
        /// 
        public ViterbiLearning(HiddenMarkovModel<TDistribution> model)
        {
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

            // Convert the generic representation to a vector of multivariate sequences
            double[][][] vectorObservations = new double[observations.Length][][];
            for (int i = 0; i < observations.Length; i++)
                vectorObservations[i] = convert(observations[i], model.Dimension);

            return base.Run(vectorObservations);
        }

        /// <summary>
        ///   Runs one single epoch (iteration) of the learning algorithm.
        /// </summary>
        /// 
        /// <param name="inputs">The observation sequences.</param>
        /// <param name="outputs">A vector to be populated with the decoded Viterbi sequences.</param>
        /// 
        protected override void RunEpoch(double[][][] inputs, int[][] outputs)
        {
            var model = mle.Model;

            // Compute the Viterbi path for all sequences
            for (int i = 0; i < inputs.Length; i++)
                outputs[i] = model.Decode(inputs[i]);

            // Compute Maximum Likelihood Estimation 
            mle.Run(inputs, outputs);
        }

        /// <summary>
        ///   Computes the log-likelihood for the current model for the given observations.
        /// </summary>
        /// 
        /// <param name="observations">The observation vectors.</param>
        /// 
        /// <returns>The log-likelihood of the observations belonging to the model.</returns>
        /// 
        protected override double ComputeLogLikelihood(double[][][] observations)
        {
            var model = mle.Model;

            double logLikelihood = Double.NegativeInfinity;
            for (int i = 0; i < observations.Length; i++)
                logLikelihood = Special.LogSum(logLikelihood, model.Evaluate(observations[i]));

            return logLikelihood;
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
