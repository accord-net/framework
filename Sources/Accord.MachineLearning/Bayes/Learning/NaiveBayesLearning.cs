// Accord Machine Learning Library
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

namespace Accord.MachineLearning.Bayes
{
    using Accord.Math;
    using Accord.Math.Optimization.Losses;
    using Accord.Statistics;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Univariate;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;



    /// <summary>
    ///   Naïve Bayes learning algorithm.
    /// </summary>
    /// 
    /// <seealso cref="NaiveBayes"/>
    /// 
    [Serializable]
    public class NaiveBayesLearning :
        NaiveBayesLearningBase<NaiveBayes, GeneralDiscreteDistribution, int,
                           IndependentOptions<GeneralDiscreteOptions>,
                           GeneralDiscreteOptions>,
        ISupervisedLearning<NaiveBayes, int[], double[]>,
        ISupervisedLearning<NaiveBayes, int[], int>
    {

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// 
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weight">The weight of importance for each input-output pair.</param>
        /// 
        /// <returns>
        ///   A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        /// 
        public override NaiveBayes Learn(int[][] x, int[] y, double[] weight = null)
        {
            CheckArgs(x, y);

            if (Model == null)
            {
                int[] inputs = x.DistinctCount();
                int outputs = y.DistinctCount();
                Model = new NaiveBayes(outputs, inputs);
            }

            // For each class
            Parallel.For(0, Model.NumberOfOutputs, ParallelOptions, i =>
            {
                // Estimate conditional distributions
                // Get variables values in class i
                int[] idx = y.Find(y_i => y_i == i);
                int[][] values = x.Get(idx);

                int n = idx.Length;

                if (Empirical)
                    Model.Priors[i] = n / (double)x.Length;

                double regularization = Options.InnerOption.Regularization;
                if (Options.InnerOptions != null)
                    regularization = Options.InnerOptions[i].Regularization;

                // For each variable (col)
                Parallel.For(0, Model.NumberOfInputs, ParallelOptions, j =>
                {
                    // Count value occurrences and store
                    // frequencies to form probabilities
                    int numberOfSymbols = Model.NumberOfSymbols[j];
                    double[] frequencies = new double[numberOfSymbols];
                    double[] probabilities = Model.Distributions[i, j];

                    // For each input row (instance)
                    // belonging to the current class
                    for (int k = 0; k < values.Length; k++)
                        frequencies[values[k][j]]++;

                    // Transform into probabilities
                    for (int k = 0; k < frequencies.Length; k++)
                    {
                        // Use a M-estimator using the previously
                        // available probabilities as priors.
                        double prior = probabilities[k];
                        double num = frequencies[k] + regularization;
                        double den = values.Length + regularization;

                        probabilities[k] = (num / den) * prior;
                    }
                });
            });

            return Model;
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// 
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weight">The weight of importance for each input-output pair.</param>
        /// 
        /// <returns>
        ///   A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        /// 
        public override NaiveBayes Learn(int[][] x, double[][] y, double[] weight = null)
        {
            CheckArgs(x, y);

            if (Model == null)
            {
                int[] inputs = x.DistinctCount();
                int outputs = y[0].Length;
                Model = new NaiveBayes(outputs, inputs);
            }

            if (Options.InnerOptions != null)
                for (int i = 0; i < Options.InnerOptions.Length; i++)
                    Options.InnerOptions[i] = Options.InnerOption;

            // For each class
            Parallel.For(0, Model.NumberOfOutputs, ParallelOptions, i =>
            {
                // Estimate conditional distributions
                // Get variables values in class i
                double sumOfWeights = 0;
                for (int j = 0; j < y.Length; j++)
                    sumOfWeights += y[j][i];

                if (Empirical)
                    Model.Priors[i] = sumOfWeights / x.Length;

                double regularization = Options.InnerOptions[i].Regularization;

                // For each variable (col)
                Parallel.For(0, Model.NumberOfInputs, ParallelOptions, j =>
                {
                    // Count value occurrences and store
                    // frequencies to form probabilities
                    int numberOfSymbols = Model.NumberOfSymbols[j];
                    double[] frequencies = new double[numberOfSymbols];
                    double[] probabilities = Model.Distributions[i, j];

                    // For each input row (instance)
                    // belonging to the current class
                    for (int k = 0; k < x.Length; k++)
                        frequencies[x[k][j]] += y[i][j];

                    // Transform into probabilities
                    for (int k = 0; k < frequencies.Length; k++)
                    {
                        // Use a M-estimator using the previously
                        // available probabilities as priors.
                        double prior = probabilities[k];
                        double num = frequencies[k] + regularization;
                        double den = sumOfWeights + regularization;

                        probabilities[k] = (num / den) * prior;
                    }
                });
            });

            return Model;
        }
    }
}
