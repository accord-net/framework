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
    using Accord.Math;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.MachineLearning;
    using Accord.Compat;
    using System.Threading;

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
    /// </remarks>
    /// 
    /// <example>
    /// <code source="Unit Tests\Accord.Tests.Statistics\Models\Markov\ViterbiLearning`1Test.cs" region="doc_learn" />
    /// </example>
    /// 
    /// <seealso cref="ViterbiLearning"/>
    /// <seealso cref="BaumWelchLearning{TDistribution}"/>
    /// 
    /// 
    public class ViterbiLearning<TDistribution, TObservation> : BaseViterbiLearning<TObservation[]>,
        IUnsupervisedLearning<HiddenMarkovModel<TDistribution, TObservation>, TObservation[], int[]>
        where TDistribution : IFittableDistribution<TObservation>
    {

        private MaximumLikelihoodLearning<TDistribution, TObservation> mle;

        /// <summary>
        ///   Gets the model being trained.
        /// </summary>
        /// 
        public HiddenMarkovModel<TDistribution, TObservation> Model
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
        /// Gets or sets a cancellation token that can be used
        /// to cancel the algorithm while it is running.
        /// </summary>
        /// 
        public override CancellationToken Token
        {
            get { return mle.Token; }
            set { mle.Token = value; }
        }

        /// <summary>
        ///   Creates a new instance of the Viterbi learning algorithm.
        /// </summary>
        /// 
        public ViterbiLearning(HiddenMarkovModel<TDistribution, TObservation> model)
        {
            this.mle = new MaximumLikelihoodLearning<TDistribution, TObservation>(model);
        }


        /// <summary>
        /// Learns a model that can map the given inputs to the desired outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="weights">The weight of importance for each input sample.</param>
        /// <returns>A model that has learned how to produce suitable outputs
        /// given the input data <paramref name="x" />.</returns>
        public HiddenMarkovModel<TDistribution, TObservation> Learn(TObservation[][] x, double[] weights = null)
        {
            var model = mle.Model;
            base.Run(x);
            return model;
        }


        /// <summary>
        ///   Runs one single epoch (iteration) of the learning algorithm.
        /// </summary>
        /// 
        /// <param name="inputs">The observation sequences.</param>
        /// <param name="outputs">A vector to be populated with the decoded Viterbi sequences.</param>
        /// 
        protected override void RunEpoch(TObservation[][] inputs, int[][] outputs)
        {
            var model = mle.Model;

            // Compute the Viterbi path for all sequences
            model.Decide(inputs, result: outputs);

            // Compute Maximum Likelihood Estimation 
            mle.Learn(inputs, outputs);
        }

        /// <summary>
        ///   Computes the log-likelihood for the current model for the given observations.
        /// </summary>
        /// 
        /// <param name="observations">The observation vectors.</param>
        /// 
        /// <returns>The log-likelihood of the observations belonging to the model.</returns>
        /// 
        protected override double ComputeLogLikelihood(TObservation[][] observations)
        {
            var model = mle.Model;

            double logLikelihood = Double.NegativeInfinity;
            for (int i = 0; i < observations.Length; i++)
                logLikelihood = Special.LogSum(logLikelihood, model.LogLikelihood(observations[i]));

            return logLikelihood;
        }
    }
}
