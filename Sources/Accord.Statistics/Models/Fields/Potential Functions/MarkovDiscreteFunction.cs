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
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Models.Fields.Features;
    using Accord.Statistics.Models.Fields.Functions.Specialized;

    /// <summary>
    ///   Potential function modeling Hidden Markov Models.
    /// </summary>
    /// 
    [Serializable]
    public sealed class MarkovDiscreteFunction : PotentialFunctionBase<int>, IPotentialFunction<int>, ICloneable
    {

        /// <summary>
        ///   Gets the number of symbols assumed by this function.
        /// </summary>
        /// 
        public int Symbols { get; private set; }

        /// <summary>
        ///   Constructs a new potential function modeling Hidden Markov Models.
        /// </summary>
        /// 
        /// <param name="states">The number of states.</param>
        /// <param name="symbols">The number of symbols.</param>
        /// <param name="outputClasses">The number of output classes.</param>
        /// 
        public MarkovDiscreteFunction(int states, int symbols, int outputClasses)
        {
            this.Outputs = outputClasses;
            this.Symbols = symbols;

            int factorIndex = 0;
            var factorParams = new List<double>();
            var factorFeatures = new List<IFeature<int>>();

            this.Factors = new FactorPotential<int>[Outputs];

            int[] classOffset = new int[outputClasses];
            int[] edgeOffset = new int[outputClasses];
            int[] stateOffset = new int[outputClasses];
            int[] classCount = new int[outputClasses];
            int[] edgeCount = new int[outputClasses];
            int[] stateCount = new int[outputClasses];


            // Create features for initial class probabilities
            for (int c = 0; c < outputClasses; c++)
            {
                var stateParams = new List<double>();
                var stateFeatures = new List<IFeature<int>>();

                var edgeParams = new List<double>();
                var edgeFeatures = new List<IFeature<int>>();

                var classParams = new List<double>();
                var classFeatures = new List<IFeature<int>>();


                // Create features for class labels
                classParams.Add(Math.Log(1.0 / outputClasses));
                classFeatures.Add(new OutputFeature<int>(this, c, c));

                // Create features for initial state probabilities
                for (int i = 0; i < states; i++)
                {
                    edgeParams.Add((i == 0) ? Math.Log(1.0) : Math.Log(0.0));
                    edgeFeatures.Add(new InitialFeature<int>(this, c, i));
                }

                // Create features for state transition probabilities
                for (int i = 0; i < states; i++)
                {
                    for (int j = 0; j < states; j++)
                    {
                        edgeParams.Add(Math.Log(1.0 / states));
                        edgeFeatures.Add(new TransitionFeature<int>(this, c, i, j));
                    }
                }

                // Create features for symbol emission probabilities
                for (int i = 0; i < states; i++)
                {
                    for (int k = 0; k < symbols; k++)
                    {
                        stateParams.Add(Math.Log(1.0 / symbols));
                        stateFeatures.Add(new EmissionFeature(this, c, i, k));
                    }
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

            for (int c = 0; c < outputClasses; c++)
            {
                Factors[c] = new MarkovDiscreteFactor(this, states, c, symbols,
                    classIndex: classOffset[c], classCount: classCount[c],  // 1. classes
                    edgeIndex: edgeOffset[c], edgeCount: edgeCount[c],      // 2. edges
                    stateIndex: stateOffset[c], stateCount: stateCount[c]); // 3. states
            }
        }

        /// <summary>
        ///   Constructs a new potential function modeling Hidden Markov Models.
        /// </summary>
        /// 
        /// <param name="classifier">The classifier model.</param>
        /// <param name="includePriors">True to include class features (priors), false otherwise.</param>
        /// 
        public MarkovDiscreteFunction(HiddenMarkovClassifier classifier, bool includePriors = true)
        {
            this.Symbols = classifier.Symbols;
            this.Outputs = classifier.Classes;

            int factorIndex = 0;
            var factorParams = new List<double>();
            var factorFeatures = new List<IFeature<int>>();

            this.Factors = new FactorPotential<int>[Outputs];

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
                var stateFeatures = new List<IFeature<int>>();

                var edgeParams = new List<double>();
                var edgeFeatures = new List<IFeature<int>>();

                var classParams = new List<double>();
                var classFeatures = new List<IFeature<int>>();

                var model = classifier[c];

                if (includePriors)
                {
                    // Create features for class labels
                    classParams.Add(Math.Log(classifier.Priors[c]));
                    classFeatures.Add(new OutputFeature<int>(this, c, c));
                }

                // Create features for initial state probabilities
                for (int i = 0; i < model.States; i++)
                {
                    edgeParams.Add(model.Probabilities[i]);
                    edgeFeatures.Add(new InitialFeature<int>(this, c, i));
                }

                // Create features for state transition probabilities
                for (int i = 0; i < model.States; i++)
                {
                    for (int j = 0; j < model.States; j++)
                    {
                        edgeParams.Add(model.Transitions[i, j]);
                        edgeFeatures.Add(new TransitionFeature<int>(this, c, i, j));
                    }
                }

                // Create features for symbol emission probabilities
                for (int i = 0; i < model.States; i++)
                {
                    for (int k = 0; k < model.Symbols; k++)
                    {
                        stateParams.Add(model.Emissions[i, k]);
                        stateFeatures.Add(new EmissionFeature(this, c, i, k));
                    }
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


            for (int c = 0; c < classifier.Models.Length; c++)
            {
                Factors[c] = new MarkovDiscreteFactor(this, classifier.Models[c].States, c, classifier.Symbols,
                    classIndex: classOffset[c], classCount: classCount[c],  // 1. classes
                    edgeIndex: edgeOffset[c], edgeCount: edgeCount[c],      // 2. edges
                    stateIndex: stateOffset[c], stateCount: stateCount[c]); // 3. states
            }
        }

        /// <summary>
        ///   Constructs a new potential function modeling Hidden Markov Models.
        /// </summary>
        /// 
        /// <param name="states">The number of states.</param>
        /// <param name="symbols">The number of symbols.</param>
        /// 
        public MarkovDiscreteFunction(int states, int symbols)
        {
            this.Symbols = symbols;

            var factorParams = new List<double>();
            var factorFeatures = new List<IFeature<int>>();

            var stateParams = new List<double>();
            var stateFeatures = new List<IFeature<int>>();

            var edgeParams = new List<double>();
            var edgeFeatures = new List<IFeature<int>>();


            // Create features for initial state probabilities
            for (int i = 0; i < states; i++)
            {
                edgeParams.Add(0);
                edgeFeatures.Add(new InitialFeature<int>(this, 0, i));
            }

            // Create features for state transition probabilities
            for (int i = 0; i < states; i++)
            {
                for (int j = 0; j < states; j++)
                {
                    edgeParams.Add(0);
                    edgeFeatures.Add(new TransitionFeature<int>(this, 0, i, j));
                }
            }

            // Create features for symbol emission probabilities
            for (int i = 0; i < states; i++)
            {
                for (int k = 0; k < symbols; k++)
                {
                    stateParams.Add(0);
                    stateFeatures.Add(new EmissionFeature(this, 0, i, k));
                }
            }


            // 1. edges
            factorFeatures.AddRange(edgeFeatures);
            factorParams.AddRange(edgeParams);

            // 2. states
            factorFeatures.AddRange(stateFeatures);
            factorParams.AddRange(stateParams);

            this.Features = factorFeatures.ToArray();
            this.Weights = factorParams.ToArray();



            this.Factors = new[] // First features and parameters always belong to edges
            { 
                new MarkovDiscreteFactor(this, states, 0, symbols,
                  edgeIndex: 0, edgeCount: edgeParams.Count,                    // 1. edges
                  stateIndex: edgeParams.Count, stateCount: stateParams.Count)  // 2. states
            };  
        }

        /// <summary>
        ///   Constructs a new potential function modeling Hidden Markov Models.
        /// </summary>
        /// 
        /// <param name="model">The hidden Markov model.</param>
        /// 
        public MarkovDiscreteFunction(HiddenMarkovModel model)
        {
            int states = model.States;
            this.Symbols = model.Symbols;

            var factorParams = new List<double>();
            var factorFeatures = new List<IFeature<int>>();

            var stateParams = new List<double>();
            var stateFeatures = new List<IFeature<int>>();

            var edgeParams = new List<double>();
            var edgeFeatures = new List<IFeature<int>>();


            // Create features for initial state probabilities
            for (int i = 0; i < states; i++)
            {
                edgeParams.Add(model.Probabilities[i]);
                edgeFeatures.Add(new InitialFeature<int>(this, 0, i));
            }

            // Create features for state transition probabilities
            for (int i = 0; i < states; i++)
            {
                for (int j = 0; j < states; j++)
                {
                    edgeParams.Add(model.Transitions[i, j]);
                    edgeFeatures.Add(new TransitionFeature<int>(this, 0, i, j));
                }
            }

            // Create features for symbol emission probabilities
            for (int i = 0; i < states; i++)
            {
                for (int k = 0; k < Symbols; k++)
                {
                    stateParams.Add(model.Emissions[i, k]);
                    stateFeatures.Add(new EmissionFeature(this, 0, i, k));
                }
            }


            // 1. edges
            factorFeatures.AddRange(edgeFeatures);
            factorParams.AddRange(edgeParams);

            // 2. states
            factorFeatures.AddRange(stateFeatures);
            factorParams.AddRange(stateParams);

            this.Features = factorFeatures.ToArray();
            this.Weights = factorParams.ToArray();


            this.Factors = new[] 
            {
                new MarkovDiscreteFactor(this, states, 0, Symbols,
                  edgeIndex: 0, edgeCount: edgeParams.Count,                   // 1. edges
                  stateIndex: edgeParams.Count, stateCount: stateParams.Count) // 2. states
            };  
        }



        #region ICloneable Members

        private MarkovDiscreteFunction() { }

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
            var clone = new MarkovDiscreteFunction();

            clone.Factors = new FactorPotential<int>[Factors.Length];
            for (int i = 0; i < Factors.Length; i++)
                clone.Factors[i] = Factors[i].Clone(newOwner: clone);

            clone.Features = new IFeature<int>[Features.Length];
            for (int i = 0; i < Features.Length; i++)
                clone.Features[i] = Features[i].Clone(newOwner: clone);

            clone.Outputs = Outputs;
            clone.Symbols = Symbols;

            clone.Weights = (double[])Weights.Clone();

            return clone;
        }

        #endregion
    }
}
