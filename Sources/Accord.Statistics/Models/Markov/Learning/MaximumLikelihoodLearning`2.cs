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
    using System.Collections.Generic;
    using Accord.MachineLearning;
    using System.Threading;

    /// <summary>
    ///    Maximum Likelihood learning algorithm for discrete-density Hidden Markov Models.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The maximum likelihood estimate is a <see cref="ISupervisedLearning">
    ///   supervised learning algorithm</see>. It considers both the sequence
    ///   of observations as well as the sequence of states in the Markov model
    ///   are visible and thus during training. </para>
    ///   
    /// <para>
    ///   Often, the Maximum Likelihood Estimate can be used to give a starting
    ///   point to a unsupervised algorithm, making possible to use semi-supervised
    ///   techniques with HMMs. It is possible, for example, to use MLE to guess
    ///   initial values for an HMM given a small set of manually labeled labels,
    ///   and then further estimate this model using the <see cref="ViterbiLearning">
    ///   Viterbi learning algorithm</see>.</para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example comes from Prof. Yechiam Yemini slides on Hidden Markov
    ///   Models, available at http://www.cs.columbia.edu/4761/notes07/chapter4.3-HMM.pdf.
    ///   In this example, we will be specifying both the sequence of observations and
    ///   the sequence of states assigned to each observation in each sequence to learn
    ///   our Markov model.
    /// </para>
    /// 
    /// <code source="Unit Tests\Accord.Tests.Statistics\Models\Markov\MaximumLikelihoodLearning`1Test.cs" region="doc_learn"/>
    /// 
    /// <para>
    ///   The following example shows how hidden Markov models trained using Maximum Likelihood Learning
    ///   can be used in the context of fraud analysis.</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.Statistics\Models\Markov\MaximumLikelihoodLearning`1Test.cs" region="doc_learn_fraud_analysis"/>
    /// 
    /// <para>
    ///   Where the transform function is defined as:</para>
    /// <code source="Unit Tests\Accord.Tests.Statistics\Models\Markov\MaximumLikelihoodLearning`1Test.cs" region="doc_learn_fraud_transform"/>
    /// </example>
    /// 
    /// <seealso cref="MaximumLikelihoodLearning"/>
    /// <seealso cref="ViterbiLearning{TDistribution}"/>
    /// <seealso cref="HiddenMarkovModel{TDistribution}"/>
    /// 
    public class MaximumLikelihoodLearning<TDistribution, TObservation> :
        BaseMaximumLikelihoodLearning<HiddenMarkovModel<TDistribution, TObservation>, TDistribution, TObservation, IFittingOptions>,
        ISupervisedLearning<HiddenMarkovModel<TDistribution, TObservation>, TObservation[], int[]>
        where TDistribution : IFittableDistribution<TObservation>
    {


        /// <summary>
        ///   Creates a new instance of the Maximum Likelihood learning algorithm.
        /// </summary>
        /// 
        public MaximumLikelihoodLearning(HiddenMarkovModel<TDistribution, TObservation> model)
        {
            this.Model = model;
        }

        /// <summary>
        ///   Creates a new instance of the Maximum Likelihood learning algorithm.
        /// </summary>
        /// 
        public MaximumLikelihoodLearning()
        {
        }

        /// <summary>
        ///   Creates an instance of the model to be learned. Inheritors of this abstract 
        ///   class must define this method so new models can be created from the training data.
        /// </summary>
        /// 
        protected override HiddenMarkovModel<TDistribution, TObservation> Create(TObservation[][] x, int numberOfClasses)
        {
            var hmm = new HiddenMarkovModel<TDistribution, TObservation>(states: numberOfClasses, emissions: Emissions);

            MarkovHelperMethods.CheckObservationDimensions(x, hmm);

            return hmm;
        }

        
    }
}
