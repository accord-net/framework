// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
#pragma warning disable 612, 618

    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Accord.Math;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Models.Markov.Topology;

    /// <summary>
    ///   Learning algorithm for <see cref="HiddenMarkovClassifier{TDistribution}">
    ///   arbitrary-density generative hidden Markov sequence classifiers</see>.
    /// </summary>
    /// 
    public class HiddenMarkovClassifierLearning<TDistribution, TObservation> :
        BaseHiddenMarkovClassifierLearning<HiddenMarkovClassifier<TDistribution, TObservation>,
        HiddenMarkovModel<TDistribution, TObservation>, TDistribution, TObservation>
        where TDistribution : IDistribution<TObservation>
    {


        /// <summary>
        ///   Creates a new instance of the learning algorithm for a given 
        ///   Markov sequence classifier using the specified configuration
        ///   function.
        /// </summary>
        /// 
        [Obsolete("Please set the learning algorithm using the Learner property.")]
        public HiddenMarkovClassifierLearning(HiddenMarkovClassifier<TDistribution, TObservation> classifier,
            ClassifierLearningAlgorithmConfiguration algorithm)
            : base(classifier, algorithm)
        {
        }

        public HiddenMarkovClassifierLearning(HiddenMarkovClassifier<TDistribution, TObservation> classifier)
            : base(classifier)
        {
        }

        /// <summary>
        ///   Creates a new <see cref="Threshold">threshold model</see>
        ///   for the current set of Markov models in this sequence classifier.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="Threshold">threshold Markov model</see>.
        /// </returns>
        /// 
        public override HiddenMarkovModel<TDistribution, TObservation> Threshold()
        {
            var models = Classifier.Models;
            var topology = CreateThresholdTopology();

            int states = topology.States;

            // Create the threshold emission matrix
            var emissions = new TDistribution[states];


            // Then, for each hidden Markov model in the classifier
            for (int i = 0, modelStartIndex = 0; i < models.Length; i++)
            {
                // Retrieve the model definition matrices
                TDistribution[] B = models[i].Emissions;

                // Now, for each state in the model
                for (int j = 0; j < models[i].States; j++)
                {
                    // Copy emissions from the model
                    emissions[j + modelStartIndex] = B[j];
                }

                // Next model starts where this ends
                modelStartIndex += models[i].States;
            }


            // Create and return the threshold hidden Markov model
            return new HiddenMarkovModel<TDistribution, TObservation>(topology, emissions)
            {
                Tag = "Non-gesture"
            };
        }

    }
}
