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
#pragma warning disable 612, 618

    using System;
    using System.Threading;
    using Accord.Math;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Models.Markov.Topology;
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Obsolete. Please use <see cref="HiddenMarkovClassifierLearning{TDistribution, TObservation}"/> instead.
    /// </summary>
    /// 
    [Obsolete("Please use HiddenMarkovClassifierLearning<TDistribution, TObservation> instead.")]
    public class HiddenMarkovClassifierLearning<TDistribution> :
        BaseHiddenMarkovClassifierLearning<HiddenMarkovClassifier<TDistribution>,
        HiddenMarkovModel<TDistribution>>
        where TDistribution : IDistribution
    {


        /// <summary>
        ///   Creates a new instance of the learning algorithm for a given 
        ///   Markov sequence classifier using the specified configuration
        ///   function.
        /// </summary>
        /// 
        public HiddenMarkovClassifierLearning(HiddenMarkovClassifier<TDistribution> classifier,
            ClassifierLearningAlgorithmConfiguration algorithm)
            : base(classifier, algorithm) { }


        /// <summary>
        ///   Trains each model to recognize each of the output labels.
        /// </summary>
        /// 
        /// <returns>The sum log-likelihood for all models after training.</returns>
        /// 
        public double Run(Array[] inputs, int[] outputs)
        {
            return base.Run<Array>(inputs, outputs);
        }


        /// <summary>
        ///   Compute model error for a given data set.
        /// </summary>
        /// 
        /// <param name="inputs">The input points.</param>
        /// <param name="outputs">The output points.</param>
        /// 
        /// <returns>The percent of misclassification errors for the data.</returns>
        /// 
        public double ComputeError(Array[] inputs, int[] outputs)
        {
            int errors = 0;
            Parallel.For(0, inputs.Length, i =>
            {
                int expectedOutput = outputs[i];
                int actualOutput = Classifier.Compute(inputs[i]);

                if (expectedOutput != actualOutput)
                    Interlocked.Increment(ref errors);
            });

            return errors / (double)inputs.Length;
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
        public override HiddenMarkovModel<TDistribution> Threshold()
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
            return new HiddenMarkovModel<TDistribution>(topology, emissions)
            { 
                Tag = "Non-gesture"
            };
        }

    }
}
