// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
        ISupervisedLearning<NaiveBayes, int[], double[]>,
        ISupervisedLearning<NaiveBayes, int[], int>
    {

        NaiveBayes classifier;

        /// <summary>
        ///   The amount of regularization to be used in the m-estimator. Default is 1e-5.
        /// </summary>
        /// 
        public double Regularization { get; set; }

        /// <summary>
        ///   The corresponding output labels for the input data.
        /// </summary>
        /// 
        public bool Empirical { get; set; }

        

        /// <summary>
        ///   Constructs a new Naïve Bayes learning algorithm.
        /// </summary>
        /// 
        public NaiveBayesLearning()
        {
            Empirical = true;
            Regularization = 1e-5;
        }

        /// <summary>
        ///   Constructs a new Naïve Bayes learning algorithm.
        /// </summary>
        /// 
        /// <param name="classifier">An existing classifier that will be tuned.</param>
        /// 
        public NaiveBayesLearning(NaiveBayes classifier)
            : this()
        {
            this.classifier = classifier;
        }


        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// 
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// 
        /// <returns>
        ///   A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        /// 
        public NaiveBayes Learn(int[][] x, int[] y)
        {
            if (x == null)
                throw new ArgumentNullException("inputs");

            if (y == null)
                throw new ArgumentNullException("outputs");

            if (x.Length == 0)
                throw new ArgumentException("The array has zero length.", "inputs");

            if (x.Length != y.Length)
                throw new DimensionMismatchException("outputs");

            if (classifier == null)
            {
                int[] inputs = x.DistinctCount();
                int outputs = y.DistinctCount();
                classifier = new NaiveBayes(outputs, inputs);
            }

            // For each class
            Parallel.For(0, classifier.NumberOfOutputs, i =>
            {
                // Estimate conditional distributions
                // Get variables values in class i
                var idx = y.Find(y_i => y_i == i);
                var values = x.Submatrix(idx);

                if (Empirical)
                    classifier.Priors[i] = idx.Length / (double)x.Length;

                // For each variable (col)
                Parallel.For(0, classifier.NumberOfInputs, j =>
                {
                    // Count value occurrences and store
                    // frequencies to form probabilities
                    int numberOfSymbols = classifier.NumberOfSymbols[j];
                    double[] frequencies = new double[numberOfSymbols];
                    double[] probabilities = classifier.Distributions[i, j];

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
                        double num = frequencies[k] + Regularization * prior;
                        double den = values.Length + Regularization;

                        probabilities[k] = num / den;
                    }
                });
            });

            return classifier;
        }


        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// 
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// 
        /// <returns>
        ///   A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        /// 
        public NaiveBayes Learn(int[][] x, double[][] y)
        {
            if (x == null)
                throw new ArgumentNullException("inputs");

            if (y == null)
                throw new ArgumentNullException("outputs");

            if (x.Length == 0)
                throw new ArgumentException("The array has zero length.", "inputs");

            if (x.Length != y.Length)
                throw new DimensionMismatchException("outputs");

            if (classifier == null)
            {
                int[] inputs = x.DistinctCount();
                int outputs = y[0].Length;
                classifier = new NaiveBayes(outputs, inputs);
            }

            // For each class
            Parallel.For(0, classifier.NumberOfOutputs, i =>
            {
                // Estimate conditional distributions
                // Get variables values in class i
                double sum = 0;
                for (int j = 0; j < y.Length; j++)
                    sum += y[i][i];

                if (Empirical)
                    classifier.Priors[i] = sum / x.Length;

                // For each variable (col)
                Parallel.For(0, classifier.NumberOfInputs, j =>
                {
                    // Count value occurrences and store
                    // frequencies to form probabilities
                    int numberOfSymbols = classifier.NumberOfSymbols[j];
                    double[] frequencies = new double[numberOfSymbols];
                    double[] probabilities = classifier.Distributions[i, j];

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
                        double num = frequencies[k] + Regularization * prior;
                        double den = sum + Regularization;

                        probabilities[k] = num / den;
                    }
                });
            });

            return classifier;
        }
    }
}
