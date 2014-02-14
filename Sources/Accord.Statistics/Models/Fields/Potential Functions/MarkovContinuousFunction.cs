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

namespace Accord.Statistics.Models.Fields.Functions
{
    using System;
    using System.Collections.Generic;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Models.Fields.Features;
    using Accord.Statistics.Models.Fields.Functions.Specialized;
    using Accord.Statistics.Models.Markov;

    /// <summary>
    ///   Potential function modeling <see cref="HiddenMarkovClassifier">Hidden Markov Classifiers</see>.
    /// </summary>
    /// 
    [Serializable]
    public sealed class MarkovContinuousFunction : PotentialFunctionBase<double>, IPotentialFunction<double>
    {

        /// <summary>
        ///   Constructs a new potential function modeling Hidden Markov Models.
        /// </summary>
        /// 
        /// <param name="classifier">A hidden Markov sequence classifier.</param>
        /// 
        public MarkovContinuousFunction(HiddenMarkovClassifier<NormalDistribution> classifier)
        {
            this.Outputs = classifier.Classes;

            int factorIndex = 0;
            var factorParams = new List<double>();
            var factorFeatures = new List<IFeature<double>>();

            this.Factors = new FactorPotential<double>[Outputs];

            int[] classOffset = new int[classifier.Classes];
            int[] edgeOffset = new int[classifier.Classes];
            int[] stateOffset = new int[classifier.Classes];
            int[] classCount = new int[classifier.Classes];
            int[] edgeCount = new int[classifier.Classes];
            int[] stateCount = new int[classifier.Classes];


            // Create features for initial class probabilities
            for (int c = 0; c < classifier.Classes; c++)
            {
                var stateParams = new List<double>();
                var stateFeatures = new List<IFeature<double>>();

                var edgeParams = new List<double>();
                var edgeFeatures = new List<IFeature<double>>();

                var classParams = new List<double>();
                var classFeatures = new List<IFeature<double>>();

                var model = classifier[c];

                // Create features for class labels
                classParams.Add(Math.Log(classifier.Priors[c]));
                classFeatures.Add(new OutputFeature<double>(this, c, c));

                // Create features for initial state probabilities
                for (int i = 0; i < model.States; i++)
                {
                    edgeParams.Add(model.Probabilities[i]);
                    edgeFeatures.Add(new InitialFeature<double>(this, c, i));
                }

                // Create features for state transition probabilities
                for (int i = 0; i < model.States; i++)
                {
                    for (int j = 0; j < model.States; j++)
                    {
                        edgeParams.Add(model.Transitions[i, j]);
                        edgeFeatures.Add(new TransitionFeature<double>(this, c, i, j));
                    }
                }

                // Create features emission probabilities
                for (int i = 0; i < model.States; i++)
                {
                    double mean = model.Emissions[i].Mean;
                    double var = model.Emissions[i].Variance;

                    // Occupancy
                    stateParams.Add(-0.5 * (Math.Log(2.0 * Math.PI * var) + (mean * mean) / var));
                    stateFeatures.Add(new OccupancyFeature<double>(this, c, i));

                    // 1st Moment (x)
                    stateParams.Add(mean / var);
                    stateFeatures.Add(new FirstMomentFeature(this, c, i));

                    // 2nd Moment (x²)
                    stateParams.Add(-1.0 / (2.0 * var));
                    stateFeatures.Add(new SecondMomentFeature(this, c, i));
                }


                classOffset[c] = factorIndex;
                edgeOffset[c] = factorIndex + classParams.Count;
                stateOffset[c] = factorIndex + classParams.Count + edgeParams.Count;

                classCount[c] = classParams.Count;
                edgeCount[c] = edgeParams.Count;
                stateCount[c] = stateParams.Count;


                // 1. classes
                factorFeatures.AddRange(classFeatures);
                factorParams.AddRange(classParams);

                // 2. edges
                factorFeatures.AddRange(edgeFeatures);
                factorParams.AddRange(edgeParams);

                // 3. states
                factorFeatures.AddRange(stateFeatures);
                factorParams.AddRange(stateParams);

                factorIndex += classParams.Count + stateParams.Count + edgeParams.Count;
            }

            System.Diagnostics.Debug.Assert(factorIndex == factorParams.Count);
            System.Diagnostics.Debug.Assert(factorIndex == factorFeatures.Count);

            this.Weights = factorParams.ToArray();
            this.Features = factorFeatures.ToArray();


            for (int c = 0; c < classifier.Classes; c++)
            {
                Factors[c] = new MarkovNormalFactor(this, classifier.Models[c].States, c,
                    classIndex: classOffset[c], classCount: classCount[c],  // 1. classes
                    edgeIndex: edgeOffset[c], edgeCount: edgeCount[c],      // 2. edges
                    stateIndex: stateOffset[c], stateCount: stateCount[c]); // 3. states
            }
        }

        #region ICloneable Members

        private MarkovContinuousFunction() { }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public object Clone()
        {
            var clone = new MarkovContinuousFunction();

            clone.Factors = new FactorPotential<double>[Factors.Length];
            for (int i = 0; i < Factors.Length; i++)
                clone.Factors[i] = Factors[i].Clone(newOwner: clone);

            clone.Features = new IFeature<double>[Features.Length];
            for (int i = 0; i < Features.Length; i++)
                clone.Features[i] = Features[i].Clone(newOwner: clone);

            clone.Outputs = Outputs;
            clone.Weights = (double[])Weights.Clone();

            return clone;
        }

        #endregion

    }
}
