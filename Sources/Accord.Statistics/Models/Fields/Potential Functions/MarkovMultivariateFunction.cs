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
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Models.Fields.Features;
    using Accord.Statistics.Models.Fields.Functions.Specialized;
    using Accord.Statistics.Models.Markov;

    /// <summary>
    ///   Potential function modeling Hidden Markov Models.
    /// </summary>
    /// 
    [Serializable]
    public sealed class MarkovMultivariateFunction
        : PotentialFunctionBase<double[]>, IPotentialFunction<double[]>
    {

        /// <summary>
        ///   Gets the total number of dimensions for 
        ///   this multivariate potential function.
        /// </summary>
        /// 
        public int Dimensions { get; private set; }

        /// <summary>
        ///   Constructs a new potential function modeling Hidden Markov Models.
        /// </summary>
        /// 
        /// <param name="classifier">A hidden Markov sequence classifier.</param>
        /// <param name="includePriors">True to include class features (priors), false otherwise.</param>
        /// 
        public MarkovMultivariateFunction(
            HiddenMarkovClassifier<MultivariateNormalDistribution> classifier, bool includePriors = true)
        {
            this.Outputs = classifier.Classes;
            this.Dimensions = classifier.Models[0].Dimension;

            int factorIndex = 0;
            var factorParams = new List<double>();
            var factorFeatures = new List<IFeature<double[]>>();

            this.Factors = new FactorPotential<double[]>[Outputs];

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
                var stateFeatures = new List<IFeature<double[]>>();

                var edgeParams = new List<double>();
                var edgeFeatures = new List<IFeature<double[]>>();

                var classParams = new List<double>();
                var classFeatures = new List<IFeature<double[]>>();

                var model = classifier[c];

                if (includePriors)
                {
                    // Create features for class labels
                    classParams.Add(Math.Log(classifier.Priors[c]));
                    classFeatures.Add(new OutputFeature<double[]>(this, c, c));
                }

                // Create features for initial state probabilities
                for (int i = 0; i < model.States; i++)
                {
                    edgeParams.Add(model.Probabilities[i]);
                    edgeFeatures.Add(new InitialFeature<double[]>(this, c, i));
                }

                // Create features for state transition probabilities
                for (int i = 0; i < model.States; i++)
                {
                    for (int j = 0; j < model.States; j++)
                    {
                        edgeParams.Add(model.Transitions[i, j]);
                        edgeFeatures.Add(new TransitionFeature<double[]>(this, c, i, j));
                    }
                }

                // Create features emission probabilities
                for (int i = 0; i < model.States; i++)
                {
                    for (int d = 0; d < model.Dimension; d++)
                    {
                        double mean = model.Emissions[i].Mean[d];
                        double var = model.Emissions[i].Variance[d];

                        // Occupancy
                        stateParams.Add(-0.5 * (Math.Log(2.0 * Math.PI * var) + (mean * mean) / var));
                        stateFeatures.Add(new OccupancyFeature<double[]>(this, c, i));

                        // 1st Moment (x)
                        stateParams.Add(mean / var);
                        stateFeatures.Add(new MultivariateFirstMomentFeature(this, c, i, d));

                        // 2nd Moment (x²)
                        stateParams.Add(-1.0 / (2.0 * var));
                        stateFeatures.Add(new MultivariateSecondMomentFeature(this, c, i, d));
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
                Factors[c] = new MarkovMultivariateNormalFactor(this, classifier.Models[c].States, c, Dimensions,
                        classIndex: classOffset[c], classCount: classCount[c],  // 1. classes
                        edgeIndex: edgeOffset[c], edgeCount: edgeCount[c],      // 2. edges
                        stateIndex: stateOffset[c], stateCount: stateCount[c]); // 3. states
            }
        }

        /// <summary>
        ///   Constructs a new potential function modeling Hidden Markov Models.
        /// </summary>
        /// 
        /// <param name="classifier">A hidden Markov sequence classifier.</param>
        /// <param name="includePriors">True to include class features (priors), false otherwise.</param>
        /// 
        public MarkovMultivariateFunction(
            HiddenMarkovClassifier<Independent<NormalDistribution>> classifier, bool includePriors = true)
        {
            this.Outputs = classifier.Classes;

            int factorIndex = 0;
            var factorParams = new List<double>();
            var factorFeatures = new List<IFeature<double[]>>();

            this.Factors = new FactorPotential<double[]>[Outputs];

            int[] startClassIndex = new int[classifier.Classes];
            int[] startEdgeIndex = new int[classifier.Classes];
            int[] startStateIndex = new int[classifier.Classes];
            int[] endClassIndex = new int[classifier.Classes];
            int[] endEdgeIndex = new int[classifier.Classes];
            int[] endStateIndex = new int[classifier.Classes];


            // Create features for initial class probabilities
            for (int c = 0; c < classifier.Classes; c++)
            {
                var stateParams = new List<double>();
                var stateFeatures = new List<IFeature<double[]>>();

                var edgeParams = new List<double>();
                var edgeFeatures = new List<IFeature<double[]>>();

                var classParams = new List<double>();
                var classFeatures = new List<IFeature<double[]>>();

                var model = classifier[c];

                if (includePriors)
                {
                    // Create features for class labels
                    classParams.Add(Math.Log(classifier.Priors[c]));
                    classFeatures.Add(new OutputFeature<double[]>(this, c, c));
                }

                // Create features for initial state probabilities
                for (int i = 0; i < model.States; i++)
                {
                    edgeParams.Add(model.Probabilities[i]);
                    edgeFeatures.Add(new InitialFeature<double[]>(this, c, i));
                }

                // Create features for initial state probabilities
                for (int i = 0; i < model.States; i++)
                {
                    stateParams.Add(model.Probabilities[i]);
                    stateFeatures.Add(new InitialFeature<double[]>(this, c, i));
                }

                // Create features for state transition probabilities
                for (int i = 0; i < model.States; i++)
                {
                    for (int j = 0; j < model.States; j++)
                    {
                        edgeParams.Add(model.Transitions[i, j]);
                        edgeFeatures.Add(new TransitionFeature<double[]>(this, c, i, j));
                    }
                }

                // Create features emission probabilities
                for (int i = 0; i < model.States; i++)
                {
                    for (int d = 0; d < model.Emissions[i].Mean.Length; d++)
                    {
                        double mean = model.Emissions[i].Mean[d];
                        double var = model.Emissions[i].Variance[d];

                        // Occupancy
                        stateParams.Add(-0.5 * (Math.Log(2.0 * Math.PI * var) + (mean * mean) / var));
                        stateFeatures.Add(new OccupancyFeature<double[]>(this, c, i));

                        // 1st Moment (x)
                        stateParams.Add(mean / var);
                        stateFeatures.Add(new MultivariateFirstMomentFeature(this, c, i, d));

                        // 2nd Moment (x²)
                        stateParams.Add(-1.0 / (2.0 * var));
                        stateFeatures.Add(new MultivariateSecondMomentFeature(this, c, i, d));
                    }
                }

                startClassIndex[c] = factorIndex;
                startEdgeIndex[c] = factorIndex + classParams.Count;
                startStateIndex[c] = factorIndex + classParams.Count + edgeParams.Count;

                endClassIndex[c] = classParams.Count;
                endEdgeIndex[c] = edgeParams.Count;
                endStateIndex[c] = stateParams.Count;
              

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
                Factors[c] = new MarkovMultivariateNormalFactor(this, classifier.Models[c].States, c, Dimensions,
                    classIndex: startClassIndex[c], classCount: endClassIndex[c],  // 1. classes
                    edgeIndex: startEdgeIndex[c], edgeCount: endEdgeIndex[c],      // 2. edges
                    stateIndex: startStateIndex[c], stateCount: endStateIndex[c]); // 3. states
            }
        }

        /// <summary>
        ///   Constructs a new potential function modeling Hidden Markov Models.
        /// </summary>
        /// 
        /// <param name="model">A normal density hidden Markov.</param>
        /// 
        public MarkovMultivariateFunction(
            HiddenMarkovModel<MultivariateNormalDistribution> model)
        {
            this.Dimensions = model.Dimension;

            var factorParams = new List<double>();
            var factorFeatures = new List<IFeature<double[]>>();


            var stateParams = new List<double>();
            var stateFeatures = new List<IFeature<double[]>>();

            var edgeParams = new List<double>();
            var edgeFeatures = new List<IFeature<double[]>>();



            // Create features for initial state probabilities
            for (int i = 0; i < model.States; i++)
            {
                edgeParams.Add(model.Probabilities[i]);
                edgeFeatures.Add(new InitialFeature<double[]>(this, 0, i));
            }

            // Create features for state transition probabilities
            for (int i = 0; i < model.States; i++)
            {
                for (int j = 0; j < model.States; j++)
                {
                    edgeParams.Add(model.Transitions[i, j]);
                    edgeFeatures.Add(new TransitionFeature<double[]>(this, 0, i, j));
                }
            }

            // Create features emission probabilities
            for (int i = 0; i < model.States; i++)
            {
                for (int d = 0; d < model.Dimension; d++)
                {
                    double mean = model.Emissions[i].Mean[d];
                    double var = model.Emissions[i].Variance[d];

                    // Occupancy
                    stateParams.Add(-0.5 * (Math.Log(2.0 * Math.PI * var) + (mean * mean) / var));
                    stateFeatures.Add(new OccupancyFeature<double[]>(this, 0, i));

                    // 1st Moment (x)
                    stateParams.Add(mean / var);
                    stateFeatures.Add(new MultivariateFirstMomentFeature(this, 0, i, d));

                    // 2nd Moment (x²)
                    stateParams.Add(-1.0 / (2.0 * var));
                    stateFeatures.Add(new MultivariateSecondMomentFeature(this, 0, i, d));
                }
            }


            // 1. edges
            factorFeatures.AddRange(edgeFeatures);
            factorParams.AddRange(edgeParams);

            // 2. states
            factorFeatures.AddRange(stateFeatures);
            factorParams.AddRange(stateParams);

            this.Weights = factorParams.ToArray();
            this.Features = factorFeatures.ToArray();



            Factors = new[] // First features and parameters are always belonging to edges
            {
                new MarkovMultivariateNormalFactor(this, model.States, 0, Dimensions,
                  edgeIndex: 0, edgeCount: edgeParams.Count,      // 1. edges
                  stateIndex: edgeParams.Count, stateCount: stateParams.Count)  // 2. states
            };
        }

        /// <summary>
        ///   Constructs a new potential function modeling Hidden Markov Models.
        /// </summary>
        /// 
        /// <param name="classifier">A hidden Markov sequence classifier.</param>
        /// <param name="includePriors">True to include class features (priors), false otherwise.</param>
        /// 
        public MarkovMultivariateFunction(HiddenMarkovClassifier<Independent> classifier, bool includePriors = true)
        {
            this.Outputs = classifier.Classes;

            int factorIndex = 0;
            var factorParams = new List<double>();
            var factorFeatures = new List<IFeature<double[]>>();

            this.Factors = new FactorPotential<double[]>[Outputs];

            int[] classOffset = new int[classifier.Classes];
            int[] edgeOffset = new int[classifier.Classes];
            int[] stateOffset = new int[classifier.Classes];
            int[] classCount = new int[classifier.Classes];
            int[] edgeCount = new int[classifier.Classes];
            int[] stateCount = new int[classifier.Classes];

            int[][][] lookupTables = new int[classifier.Classes][][];


            // Create features for initial class probabilities
            for (int c = 0; c < classifier.Classes; c++)
            {
                var stateParams = new List<double>();
                var stateFeatures = new List<IFeature<double[]>>();

                var edgeParams = new List<double>();
                var edgeFeatures = new List<IFeature<double[]>>();

                var classParams = new List<double>();
                var classFeatures = new List<IFeature<double[]>>();

                var model = classifier[c];

                int[][] lookupTable = new int[model.States][];
                for (int i = 0; i < lookupTable.Length; i++)
                    lookupTable[i] = new int[model.Dimension];

                if (includePriors)
                {
                    // Create features for class labels
                    classParams.Add(Math.Log(classifier.Priors[c]));
                    classFeatures.Add(new OutputFeature<double[]>(this, c, c));
                }

                // Create features for initial state probabilities
                for (int i = 0; i < model.States; i++)
                {
                    edgeParams.Add(model.Probabilities[i]);
                    edgeFeatures.Add(new InitialFeature<double[]>(this, c, i));
                }

                // Create features for state transition probabilities
                for (int i = 0; i < model.States; i++)
                {
                    for (int j = 0; j < model.States; j++)
                    {
                        edgeParams.Add(model.Transitions[i, j]);
                        edgeFeatures.Add(new TransitionFeature<double[]>(this, c, i, j));
                    }
                }

                int position = 0;

                // Create features emission probabilities
                for (int i = 0; i < model.States; i++)
                {
                    for (int d = 0; d < model.Emissions[i].Components.Length; d++)
                    {
                        IUnivariateDistribution distribution = model.Emissions[i].Components[d];

                        NormalDistribution normal = distribution as NormalDistribution;
                        if (normal != null)
                        {
                            double var = normal.Variance;
                            double mean = normal.Mean;

                            // Occupancy
                            stateParams.Add(-0.5 * (Math.Log(2.0 * Math.PI * var) + (mean * mean) / var));
                            stateFeatures.Add(new OccupancyFeature<double[]>(this, c, i));
                            lookupTable[i][d] = position;
                            position++;

                            // 1st Moment (x)
                            stateParams.Add(mean / var);
                            stateFeatures.Add(new MultivariateFirstMomentFeature(this, c, i, d));
                            position++;

                            // 2nd Moment (x²)
                            stateParams.Add(-1.0 / (2.0 * var));
                            stateFeatures.Add(new MultivariateSecondMomentFeature(this, c, i, d));
                            position++;

                            continue;
                        }

                        GeneralDiscreteDistribution discrete = distribution as GeneralDiscreteDistribution;
                        if (discrete != null)
                        {
                            lookupTable[i][d] = position;

                            for (int k = 0; k < discrete.Frequencies.Length; k++)
                            {
                                stateParams.Add(Math.Log(discrete.Frequencies[k]));
                                stateFeatures.Add(new MultivariateEmissionFeature(this, c, i, k, d));
                                position++;
                            }

                            continue;
                        }
                    }
                }

                classOffset[c] = factorIndex;
                edgeOffset[c] = factorIndex + classParams.Count;
                stateOffset[c] = factorIndex + classParams.Count + edgeParams.Count;

                classCount[c] = classParams.Count;
                edgeCount[c] = edgeParams.Count;
                stateCount[c] = stateParams.Count;

                lookupTables[c] = lookupTable;

                
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
                Factors[c] = new MarkovIndependentFactor(this, classifier.Models[c].States, c, lookupTables[c],
                    classIndex: classOffset[c], classCount: classCount[c],  // 1. classes
                    edgeIndex: edgeOffset[c], edgeCount: edgeCount[c],      // 2. edges
                    stateIndex: stateOffset[c], stateCount: stateCount[c]); // 3. states
            }
        }


        #region ICloneable Members

        private MarkovMultivariateFunction() { }

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
            var clone = new MarkovMultivariateFunction();

            clone.Factors = new FactorPotential<double[]>[Factors.Length];
            for (int i = 0; i < Factors.Length; i++)
                clone.Factors[i] = Factors[i].Clone(newOwner: clone);

            clone.Features = new IFeature<double[]>[Features.Length];
            for (int i = 0; i < Features.Length; i++)
                clone.Features[i] = Features[i].Clone(newOwner: clone);

            clone.Outputs = Outputs;
            clone.Weights = (double[])Weights.Clone();

            clone.Dimensions = Dimensions;

            return clone;
        }

        #endregion

    }
}
