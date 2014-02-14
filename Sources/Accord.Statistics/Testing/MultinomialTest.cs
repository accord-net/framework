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

namespace Accord.Statistics.Testing
{
    using System;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Math;

    /// <summary>
    ///   Multinomial test (approximated).
    /// </summary>
    /// 
    /// <remarks>
    ///   In statistics, the multinomial test is the test of the null hypothesis that the
    ///   parameters of a multinomial distribution equal specified values. The test can be
    ///   approximated using a <see cref="ChiSquareDistribution">chi-square distribution</see>.
    /// </remarks>
    ///
    [Serializable]
    public class MultinomialTest : ChiSquareTest
    {

        /// <summary>
        ///   Gets the observed sample proportions.
        /// </summary>
        /// 
        public double[] ObservedProportions { get; private set; }

        /// <summary>
        ///   Gets the hypothesized population proportions.
        /// </summary>
        /// 
        public double[] HypothesizedProportions { get; private set; }

        /// <summary>
        ///   Creates a new Multinomial test.
        /// </summary>
        /// 
        /// <param name="sampleProportions">The proportions for each category in the sample.</param>
        /// <param name="sampleSize">The number of observations in the sample.</param>
        /// 
        public MultinomialTest(double[] sampleProportions, int sampleSize)
        {
            double[] hypothesizedProportions = new double[sampleProportions.Length];
            for (int i = 0; i < hypothesizedProportions.Length; i++)
                hypothesizedProportions[i] = 1.0 / hypothesizedProportions.Length;

            Compute(sampleSize, sampleProportions, hypothesizedProportions);
        }

        /// <summary>
        ///   Creates a new Multinomial test.
        /// </summary>
        /// 
        /// <param name="sampleCounts">The number of occurrences for each category in the sample.</param>
        /// 
        public MultinomialTest(int[] sampleCounts)
        {
            double[] hypothesizedProportions = new double[sampleCounts.Length];
            for (int i = 0; i < hypothesizedProportions.Length; i++)
                hypothesizedProportions[i] = 1.0 / hypothesizedProportions.Length;

            int sampleSize = sampleCounts.Sum();
            double[] sampleProportions = sampleCounts.Divide(sampleSize);

            Compute(sampleSize, sampleProportions, hypothesizedProportions);
        }


        /// <summary>
        ///   Creates a new Multinomial test.
        /// </summary>
        /// 
        /// <param name="sampleCounts">The number of occurrences for each category in the sample.</param>
        /// <param name="hypothesizedProportions">The hypothesized category proportions. Default is
        ///   to assume uniformly equal proportions.</param>
        /// 
        public MultinomialTest(int[] sampleCounts, double[] hypothesizedProportions)
        {
            if (sampleCounts.Length != hypothesizedProportions.Length)
                throw new DimensionMismatchException("hypothesizedProportions");

            int sampleSize = sampleCounts.Sum();
            double[] sampleProportions = sampleCounts.Divide(sampleSize);

            Compute(sampleSize, sampleProportions, hypothesizedProportions);
        }

        /// <summary>
        ///   Creates a new Multinomial test.
        /// </summary>
        /// 
        /// <param name="sampleProportions">The proportions for each category in the sample.</param>
        /// <param name="sampleSize">The number of observations in the sample.</param>
        /// <param name="hypothesizedProportions">The hypothesized category proportions. Default is
        ///   to assume uniformly equal proportions.</param>
        /// 
        public MultinomialTest(double[] sampleProportions, int sampleSize, double[] hypothesizedProportions)
        {
            if (sampleProportions.Length != hypothesizedProportions.Length)
                throw new DimensionMismatchException("hypothesizedProportions");

            Compute(sampleSize, sampleProportions, hypothesizedProportions);
        }

        /// <summary>
        ///   Creates a new Multinomial test.
        /// </summary>
        /// 
        /// <param name="sample">The categories for each observation in the sample.</param>
        /// <param name="categories">The number of possible categories.</param>
        /// 
        public MultinomialTest(int[] sample, int categories)
        {
            double[] hypothesizedProportions = new double[categories];
            for (int i = 0; i < hypothesizedProportions.Length; i++)
                hypothesizedProportions[i] = 1.0 / hypothesizedProportions.Length;

            double[] observed = new double[categories];
            for (int i = 0; i < observed.Length; i++)
                observed[sample[i]]++;

            Compute(sample.Length, observed, hypothesizedProportions);
        }

        /// <summary>
        ///   Creates a new Multinomial test.
        /// </summary>
        /// 
        /// <param name="sample">The categories for each observation in the sample.</param>
        /// <param name="categories">The number of possible categories.</param>
        /// <param name="hypothesizedProportions">The hypothesized category proportions. Default is
        ///   to assume uniformly equal proportions.</param>
        /// 
        public MultinomialTest(int[] sample, int categories, double[] hypothesizedProportions)
        {
            double[] observed = new double[categories];
            for (int i = 0; i < observed.Length; i++)
                observed[sample[i]]++;

            Compute(sample.Length, observed, hypothesizedProportions);
        }

        /// <summary>
        ///   Computes the Multinomial test.
        /// </summary>
        /// 
        protected void Compute(int sampleSize, double[] observed, double[] expected)
        {
            // Approximate using a Chi-Square distribution

            this.ObservedProportions = observed;
            this.HypothesizedProportions = expected;

            double sum = 0;
            for (int i = 0; i < ObservedProportions.Length; i++)
            {
                double e = sampleSize * expected[i];
                double u = sampleSize * observed[i] - e;
                sum += (u * u) / e;
            }

            base.Compute(sum, observed.Length - 1);
        }


    }
}
