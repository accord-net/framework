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
    using Accord.Statistics.Distributions.Fitting;
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
    [Serializable]
    public class NaiveBayesLearning<TDistribution> : NaiveBayesLearning<TDistribution, IFittingOptions>
        where TDistribution : IFittableDistribution<double>
    {
        /// <summary>
        ///   Constructs a new Naïve Bayes learning algorithm.
        /// </summary>
        /// 
        public NaiveBayesLearning()
            : base()
        {
        }

        /// <summary>
        ///   Constructs a new Naïve Bayes learning algorithm.
        /// </summary>
        /// 
        /// <param name="classifier">An existing classifier that will be tuned.</param>
        /// 
        public NaiveBayesLearning(NaiveBayes<TDistribution> classifier)
            : base(classifier)
        {
        }
    }

    /// <summary>
    ///   Naïve Bayes learning algorithm.
    /// </summary>
    /// 
    [Serializable]
    public class NaiveBayesLearning<TDistribution, TOptions> :
        ISupervisedLearning<NaiveBayes<TDistribution>, double[], double[]>,
        ISupervisedLearning<NaiveBayes<TDistribution>, double[], int>
        where TDistribution : IFittableDistribution<double>
        where TOptions : class, IFittingOptions
    {

        NaiveBayes<TDistribution> classifier;


        /// <summary>
        ///   The corresponding output labels for the input data.
        /// </summary>
        /// 
        public bool Empirical { get; set; }

        /// <summary>
        ///   Gets or sets the fitting options to use when
        ///   estimating the class-specific distributions.
        /// </summary>
        /// 
        public TOptions Options { get; set; }


        /// <summary>
        ///   Constructs a new Naïve Bayes learning algorithm.
        /// </summary>
        /// 
        public NaiveBayesLearning()
        {
            Empirical = true;
        }

        /// <summary>
        ///   Constructs a new Naïve Bayes learning algorithm.
        /// </summary>
        /// 
        /// <param name="classifier">An existing classifier that will be tuned.</param>
        /// 
        public NaiveBayesLearning(NaiveBayes<TDistribution> classifier)
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
        public NaiveBayes<TDistribution> Learn(double[][] x, int[] y)
        {
            if (x == null)
                throw new ArgumentNullException("inputs");

            if (y == null)
                throw new ArgumentNullException("outputs");

            if (x.Length == 0)
                throw new ArgumentException("The array has zero length.", "inputs");

            if (y.Length != x.Length)
                throw new DimensionMismatchException("outputs");

            // For each class
            Parallel.For(0, classifier.NumberOfOutputs, i =>
            {
                // Estimate conditional distributions
                // Get variables values in class i
                var idx = y.Find(y_i => y_i == i);
                var values = x.Submatrix(idx, transpose: true);

                if (Empirical)
                    classifier.Priors[i] = (double)idx.Length / x.Length;

                // For each variable (col)
                Parallel.For(0, classifier.NumberOfInputs, j =>
                {
                    classifier.Distributions[i, j].Fit(values[j], Options);
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
        public NaiveBayes<TDistribution> Learn(double[][] x, double[][] y)
        {
            if (x == null)
                throw new ArgumentNullException("inputs");

            if (y == null)
                throw new ArgumentNullException("outputs");

            if (x.Length == 0)
                throw new ArgumentException("The array has zero length.", "inputs");

            if (y.Length != x.Length)
                throw new DimensionMismatchException("outputs");

            // For efficiency
            x = x.Transpose();

            // For each class
            Parallel.For(0, classifier.NumberOfOutputs, i =>
            {
                // Estimate conditional distributions
                // Get variables values in class i
                double[] weights = y.GetColumn(i);

                if (Empirical)
                    classifier.Priors[i] = weights.Sum() / x.Length;

                // For each variable (col)
                Parallel.For(0, classifier.NumberOfInputs, j =>
                {
                    classifier.Distributions[i, j].Fit(x[j], weights, Options);
                });
            });

            return classifier;
        }
    }
}
