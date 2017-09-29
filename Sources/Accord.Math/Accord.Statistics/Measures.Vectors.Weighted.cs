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

namespace Accord.Statistics
{
    using Accord.Math;
    using System;
    using System.Collections.Generic;

    static partial class Measures
    {

        /// <summary>
        ///   Calculates the exponentially weighted mean.
        /// </summary>
        /// 
        /// <param name="values">
        ///   A vector of observations whose EW mean will be calculated. It is assumed that
        ///   the vector is ordered with the most recent observations at the end of 
        ///   the vector (and the oldest observations at the start).
        /// </param>
        /// <param name="alpha">
        ///   The weighting to be applied to the calculation. A higher alpha discounts
        ///   older observations faster. Alpha must be between 0 and 1 (inclusive).
        /// </param>
        /// 
        /// <returns>
        ///   Returns a <see cref="double"/> giving the exponentially weighted average of the
        ///   vector.
        /// </returns>
        /// 
        /// <example>
        /// <para>
        ///   The following example shows how to compute the EW mean.</para>
        ///   
        /// <code source="Unit Tests\Accord.Tests.Math\Accord.Statistics\MeasuresTest.cs" region="doc_example3" />
        /// </example>
        ///
        public static double ExponentialWeightedMean(this double[] values, double alpha = 0)
        {
            if (values == null)
                throw new ArgumentNullException("values", "The vector cannot be null.");

            return values.ExponentialWeightedMean(values.Length, alpha);
        }

        /// <summary>
        ///   Calculates the exponentially weighted mean.
        /// </summary>
        /// 
        /// <param name="values">
        ///   A vector of observations whose EW mean will be calculated. It is assumed that
        ///   the vector is ordered with the most recent observations at the end of 
        ///   the vector (and the oldest observations at the start).
        /// </param>
        /// <param name="window">The number of samples to be used in the calculation.</param>
        /// <param name="alpha">
        ///   The weighting to be applied to the calculation. A higher alpha discounts
        ///   older observations faster. Alpha must be between 0 and 1 (inclusive).
        /// </param>
        /// 
        /// <returns>
        ///   Returns a <see cref="double"/> giving the exponentially weighted average of the
        ///   vector.
        /// </returns>
        /// 
        /// <example>
        /// <para>
        ///   The following example shows how to compute the EW mean.</para>
        ///   
        /// <code source="Unit Tests\Accord.Tests.Math\Accord.Statistics\MeasuresTest.cs" region="doc_example3" />
        /// </example>
        ///
        public static double ExponentialWeightedMean(this double[] values, int window, double alpha = 0)
        {
            // Perform some basic error validation
            Validate(values, window, alpha);

            // Handle the trivial case
            if (alpha == 1)
                return values.Get(-1);

            double[] truncatedSeries = window == values.Length
                ? truncatedSeries = values
                : truncatedSeries = values.Get(-window, 0);

            if (alpha == 0)
                return truncatedSeries.Mean();

            // Now we create the weights
            double[] decayWeights = GetDecayWeights(window, alpha);

            return truncatedSeries.WeightedMean(decayWeights);
        }


        /// <summary>
        ///   Calculates the exponentially weighted variance.
        /// </summary>
        /// 
        /// <param name="values">
        ///   A vector of observations whose EW variance will be calculated. It is assumed 
        ///   that the vector is ordered with the most recent observations at the end of 
        ///   the vector (and the oldest observations at the start).
        /// </param>
        /// <param name="alpha">
        ///   The weighting to be applied to the calculation. A higher alpha discounts
        ///   older observations faster. Alpha must be between 0 and 1 (inclusive).
        /// </param>
        /// <param name="unbiased">Use a standard estimation bias correction.</param>
        /// 
        /// <returns>
        ///   Returns a <see cref="double"/> giving the exponentially weighted variance.
        /// </returns>
        /// 
        /// <example>
        /// <para>
        ///   The following example shows how to compute the EW variance.</para>
        ///   
        /// <code source="Unit Tests\Accord.Tests.Math\Accord.Statistics\MeasuresTest.cs" region="doc_example4" />
        /// </example>
        ///
        public static double ExponentialWeightedVariance(
            this double[] values, double alpha = 0, bool unbiased = false)
        {
            if (values == null)
                throw new ArgumentNullException("values", "The vector cannot be null.");

            return values.ExponentialWeightedVariance(values.Length, alpha, unbiased);
        }

        /// <summary>
        ///   Calculates the exponentially weighted variance.
        /// </summary>
        /// 
        /// <param name="values">
        ///   A vector of observations whose EW variance will be calculated. It is assumed 
        ///   that the vector is ordered with the most recent observations at the end of 
        ///   the vector (and the oldest observations at the start).
        /// </param>
        /// <param name="window">The number of samples to be used in the calculation.</param>
        /// <param name="alpha">
        ///   The weighting to be applied to the calculation. A higher alpha discounts
        ///   older observations faster. Alpha must be between 0 and 1 (inclusive).
        /// </param>
        /// <param name="unbiased">Use a standard estimation bias correction.</param>
        /// 
        /// <returns>
        ///   Returns a <see cref="double"/> giving the exponentially weighted variance.
        /// </returns>
        /// 
        /// <example>
        /// <para>
        ///   The following example shows how to compute the EW variance.</para>
        ///   
        /// <code source="Unit Tests\Accord.Tests.Math\Accord.Statistics\MeasuresTest.cs" region="doc_example4" />
        /// </example>
        ///
        public static double ExponentialWeightedVariance(
            this double[] values, int window, double alpha = 0, bool unbiased = false)
        {
            // Perform some basic error validation
            Validate(values, window, alpha);

            int rows = values.Rows();

            // Handle the trivial case
            if (alpha == 1)
                return 0;

            double[] truncatedSeries = window == values.Length
                ? truncatedSeries = values
                : truncatedSeries = values.Get(-window, 0);

            // Now we create the weights
            double[] decayWeights = GetDecayWeights(window, alpha);

            return truncatedSeries.WeightedVariance(decayWeights, unbiased);
        }

        /// <summary>
        ///   Computes the Weighted Mean of the given values.
        /// </summary>
        /// 
        /// <param name="values">A double array containing the vector members.</param>
        /// <param name="weights">An unit vector containing the importance of each sample
        ///   in <see param="values"/>.</param>
        /// 
        /// <returns>The mean of the given data.</returns>
        /// 
        public static double WeightedMean(this double[] values, double[] weights)
        {
            if (values.Length != weights.Length)
            {
                throw new DimensionMismatchException("weights",
                    "The values and weight vectors must have the same length");
            }

            double sum = 0.0;
            for (int i = 0; i < values.Length; i++)
                sum += weights[i] * values[i];

            double w = 0.0;
            for (int i = 0; i < weights.Length; i++)
                w += weights[i];

            return sum / w;
        }

        /// <summary>
        ///   Computes the Weighted Mean of the given values.
        /// </summary>
        /// 
        /// <param name="values">A double array containing the vector members.</param>
        /// <param name="weights">A vector containing how many times each element
        /// in <see param="values"/> repeats itself in the non-weighted data.</param>
        /// 
        /// <returns>The mean of the given data.</returns>
        /// 
        public static double WeightedMean(this double[] values, int[] weights)
        {
            if (values.Length != weights.Length)
            {
                throw new DimensionMismatchException("weights",
                    "The values and weight vectors must have the same length");
            }

            double sum = 0.0;
            for (int i = 0; i < values.Length; i++)
                sum += weights[i] * values[i];

            int w = 0;
            for (int i = 0; i < weights.Length; i++)
                w += weights[i];

            return sum / w;
        }

        /// <summary>
        ///   Computes the Standard Deviation of the given values.
        /// </summary>
        /// 
        /// <param name="values">A double array containing the vector members.</param>
        /// <param name="weights">An unit vector containing the importance of each sample
        /// in <see param="values"/>. The sum of this array elements should add up to 1.</param>
        /// 
        /// <returns>The standard deviation of the given data.</returns>
        /// 
        public static double WeightedStandardDeviation(this double[] values, double[] weights)
        {
            return Math.Sqrt(WeightedVariance(values, weights));
        }

        /// <summary>
        ///   Computes the Standard Deviation of the given values.
        /// </summary>
        /// 
        /// <param name="values">A double array containing the vector members.</param>
        /// <param name="weights">An unit vector containing the importance of each sample
        /// in <see param="values"/>.</param>
        /// <param name="weightType">How the weights should be interpreted for the bias correction.</param>
        /// 
        /// <returns>The standard deviation of the given data.</returns>
        /// 
        public static double WeightedStandardDeviation(this double[] values, double[] weights, WeightType weightType)
        {
            return Math.Sqrt(WeightedVariance(values, weights, weightType));
        }

        /// <summary>
        ///   Computes the Standard Deviation of the given values.
        /// </summary>
        /// 
        /// <param name="values">A double array containing the vector members.</param>
        /// <param name="weights">An unit vector containing the importance of each sample
        /// in <see param="values"/>.</param>
        /// <param name="unbiased">
        ///   Pass true to compute the sample variance; or pass false to compute the 
        ///   population variance. For <see cref="WeightType.Repetition">integers weights
        ///   </see>, the bias correction is equivalent to the non-weighted case. For 
        ///   <see cref="WeightType.Fraction">fractional weights</see>, the variance
        ///   bias cannot be completely eliminated.</param>
        /// <param name="weightType">How the weights should be interpreted for the bias correction.</param>
        /// 
        /// <returns>The standard deviation of the given data.</returns>
        /// 
        public static double WeightedStandardDeviation(this double[] values, double[] weights, bool unbiased, WeightType weightType)
        {
            return Math.Sqrt(WeightedVariance(values, weights, unbiased, weightType));
        }

        /// <summary>
        ///   Computes the Standard Deviation of the given values.
        /// </summary>
        /// 
        /// <param name="values">A double array containing the vector members.</param>
        /// <param name="weights">An unit vector containing the importance of each sample
        /// in <see param="values"/>. The sum of this array elements should add up to 1.</param>
        /// <param name="mean">The mean of the vector, if already known.</param>
        /// 
        /// <returns>The standard deviation of the given data.</returns>
        /// 
        public static double WeightedStandardDeviation(this double[] values, double[] weights, double mean)
        {
            return Math.Sqrt(WeightedVariance(values, weights, mean, true));
        }

        /// <summary>
        ///   Computes the Standard Deviation of the given values.
        /// </summary>
        /// 
        /// <param name="values">A double array containing the vector members.</param>
        /// <param name="weights">An unit vector containing the importance of each sample
        /// in <see param="values"/>. The sum of this array elements should add up to 1.</param>
        /// <param name="mean">The mean of the vector, if already known.</param>
        /// <param name="unbiased">
        ///   Pass true to compute the sample variance; or pass false to compute the 
        ///   population variance. For <see cref="WeightType.Repetition">integers weights
        ///   </see>, the bias correction is equivalent to the non-weighted case. For 
        ///   <see cref="WeightType.Fraction">fractional weights</see>, the variance
        ///   bias cannot be completely eliminated.</param>
        /// <param name="weightType">How the weights should be interpreted for the bias correction.</param>
        /// 
        /// <returns>The standard deviation of the given data.</returns>
        /// 
        public static double WeightedStandardDeviation(this double[] values, double[] weights, double mean,
            bool unbiased, WeightType weightType = WeightType.Fraction)
        {
            return Math.Sqrt(WeightedVariance(values, weights, mean, unbiased, weightType));
        }

        /// <summary>
        ///   Computes the weighted Variance of the given values.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector members.</param>
        /// <param name="weights">An unit vector containing the importance of each sample
        /// in <see param="values"/>. The sum of this array elements should add up to 1.</param>
        /// 
        /// <returns>The variance of the given data.</returns>
        /// 
        public static double WeightedVariance(this double[] values, double[] weights)
        {
            return WeightedVariance(values, weights, WeightedMean(values, weights), true);
        }

        /// <summary>
        ///   Computes the weighted Variance of the given values.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector members.</param>
        /// <param name="weights">An unit vector containing the importance of each sample
        /// in <see param="values"/>.</param>
        /// <param name="weightType">How the weights should be interpreted for the bias correction.</param>
        /// 
        /// <returns>The variance of the given data.</returns>
        /// 
        public static double WeightedVariance(this double[] values, double[] weights, WeightType weightType)
        {
            return WeightedVariance(values, weights, WeightedMean(values, weights), true, weightType);
        }

        /// <summary>
        ///   Computes the weighted Variance of the given values.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector members.</param>
        /// <param name="weights">An unit vector containing the importance of each sample
        /// in <see param="values"/>. The sum of this array elements should add up to 1.</param>
        /// <param name="unbiased">
        ///   Pass true to compute the sample variance; or pass false to compute the 
        ///   population variance. For <see cref="WeightType.Repetition">integers weights
        ///   </see>, the bias correction is equivalent to the non-weighted case. For 
        ///   <see cref="WeightType.Fraction">fractional weights</see>, the variance
        ///   bias cannot be completely eliminated.</param>
        /// 
        /// <returns>The variance of the given data.</returns>
        /// 
        public static double WeightedVariance(this double[] values, double[] weights, bool unbiased)
        {
            return WeightedVariance(values, weights, WeightedMean(values, weights), unbiased);
        }

        /// <summary>
        ///   Computes the weighted Variance of the given values.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector members.</param>
        /// <param name="weights">An unit vector containing the importance of each sample
        /// in <see param="values"/>.</param>
        /// <param name="unbiased">
        ///   Pass true to compute the sample variance; or pass false to compute the 
        ///   population variance. For <see cref="WeightType.Repetition">integers weights
        ///   </see>, the bias correction is equivalent to the non-weighted case. For 
        ///   <see cref="WeightType.Fraction">fractional weights</see>, the variance
        ///   bias cannot be completely eliminated.</param>
        /// <param name="weightType">How the weights should be interpreted for the bias correction.</param>
        /// 
        /// <returns>The variance of the given data.</returns>
        /// 
        public static double WeightedVariance(this double[] values, double[] weights, bool unbiased, WeightType weightType)
        {
            return WeightedVariance(values, weights, WeightedMean(values, weights), unbiased, weightType);
        }

        /// <summary>
        ///   Computes the weighted Variance of the given values.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector members.</param>
        /// <param name="weights">An unit vector containing the importance of each sample
        /// in <see param="values"/>. The sum of this array elements should add up to 1.</param>
        /// <param name="mean">The mean of the array, if already known.</param>
        /// 
        /// <returns>The variance of the given data.</returns>
        /// 
        public static double WeightedVariance(this double[] values, double[] weights, double mean)
        {
            return WeightedVariance(values, weights, mean, true);
        }



        /// <summary>
        ///   Computes the weighted Variance of the given values.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector members.</param>
        /// <param name="mean">The mean of the array, if already known.</param>
        /// <param name="weights">An unit vector containing the importance of each sample
        /// in <see param="values"/>.</param>
        /// <param name="unbiased">
        ///   Pass true to compute the sample variance; or pass false to compute the 
        ///   population variance. For <see cref="WeightType.Repetition">integers weights
        ///   </see>, the bias correction is equivalent to the non-weighted case. For 
        ///   <see cref="WeightType.Fraction">fractional weights</see>, the variance
        ///   bias cannot be completely eliminated.</param>
        /// <param name="weightType">How the weights should be interpreted for the bias correction.</param>
        /// 
        /// <returns>The variance of the given data.</returns>
        /// 
        public static double WeightedVariance(this double[] values, double[] weights, double mean,
            bool unbiased, WeightType weightType = WeightType.Fraction)
        {
            if (values.Length != weights.Length)
            {
                throw new DimensionMismatchException("weights",
                    "The values and weight vectors must have the same length");
            }

            // http://en.wikipedia.org/wiki/Weighted_variance#Weighted_sample_variance
            // http://www.gnu.org/software/gsl/manual/html_node/Weighted-Samples.html

            double sum = 0.0;
            double squareSum = 0.0;
            double weightSum = 0.0;

            for (int i = 0; i < values.Length; i++)
            {
                double z = values[i] - mean;
                double w = weights[i];

                sum += w * (z * z);

                weightSum += w;
                squareSum += w * w;
            }

            return correct(unbiased, weightType, sum, weightSum, squareSum);
        }

        /// <summary>
        ///   Computes the Standard Deviation of the given values.
        /// </summary>
        /// 
        /// <param name="values">A double array containing the vector members.</param>
        /// <param name="weights">A vector containing how many times each element
        /// in <see param="values"/> repeats itself in the non-weighted data.</param>
        /// 
        /// <returns>The standard deviation of the given data.</returns>
        /// 
        public static double WeightedStandardDeviation(this double[] values, int[] weights)
        {
            return Math.Sqrt(WeightedVariance(values, weights));
        }

        /// <summary>
        ///   Computes the Standard Deviation of the given values.
        /// </summary>
        /// 
        /// <param name="values">A double array containing the vector members.</param>
        /// <param name="weights">A vector containing how many times each element
        /// in <see param="values"/> repeats itself in the non-weighted data.</param>
        /// <param name="mean">The mean of the vector, if already known.</param>
        /// 
        /// <returns>The standard deviation of the given data.</returns>
        /// 
        public static double WeightedStandardDeviation(this double[] values, int[] weights, double mean)
        {
            return Math.Sqrt(WeightedVariance(values, weights, mean, true));
        }

        /// <summary>
        ///   Computes the Standard Deviation of the given values.
        /// </summary>
        /// 
        /// <param name="values">A double array containing the vector members.</param>
        /// <param name="weights">A vector containing how many times each element
        /// in <see param="values"/> repeats itself in the non-weighted data.</param>
        /// <param name="mean">The mean of the vector, if already known.</param>
        /// <param name="unbiased">
        ///   Pass true to compute the sample variance; or pass false to compute the 
        ///   population variance. For <see cref="WeightType.Repetition">integers weights
        ///   </see>, the bias correction is equivalent to the non-weighted case. For 
        ///   <see cref="WeightType.Fraction">fractional weights</see>, the variance
        ///   bias cannot be completely eliminated.</param>
        /// 
        /// <returns>The standard deviation of the given data.</returns>
        /// 
        public static double WeightedStandardDeviation(this double[] values, int[] weights, double mean, bool unbiased)
        {
            return Math.Sqrt(WeightedVariance(values, weights, mean, unbiased));
        }

        /// <summary>
        ///   Computes the weighted Variance of the given values.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector members.</param>
        /// <param name="weights">A vector containing how many times each element
        /// in <see param="values"/> repeats itself in the non-weighted data.</param>
        /// 
        /// <returns>The variance of the given data.</returns>
        /// 
        public static double WeightedVariance(this double[] values, int[] weights)
        {
            return WeightedVariance(values, weights, WeightedMean(values, weights), true);
        }

        /// <summary>
        ///   Computes the weighted Variance of the given values.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector members.</param>
        /// <param name="weights">A vector containing how many times each element
        /// in <see param="values"/> repeats itself in the non-weighted data.</param>
        /// <param name="unbiased">
        ///   Pass true to compute the sample variance; or pass false to compute the 
        ///   population variance. For <see cref="WeightType.Repetition">integers weights
        ///   </see>, the bias correction is equivalent to the non-weighted case. For 
        ///   <see cref="WeightType.Fraction">fractional weights</see>, the variance
        ///   bias cannot be completely eliminated.</param>
        /// 
        /// <returns>The variance of the given data.</returns>
        /// 
        public static double WeightedVariance(this double[] values, int[] weights, bool unbiased)
        {
            return WeightedVariance(values, weights, WeightedMean(values, weights), unbiased);
        }

        /// <summary>
        ///   Computes the weighted Variance of the given values.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector members.</param>
        /// <param name="weights">A vector containing how many times each element
        /// in <see param="values"/> repeats itself in the non-weighted data.</param>
        /// <param name="mean">The mean of the array, if already known.</param>
        /// 
        /// <returns>The variance of the given data.</returns>
        /// 
        public static double WeightedVariance(this double[] values, int[] weights, double mean)
        {
            return WeightedVariance(values, weights, mean, true);
        }

        /// <summary>
        ///   Computes the weighted Variance of the given values.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector members.</param>
        /// <param name="weights">A vector containing how many times each element
        /// in <see param="values"/> repeats itself in the non-weighted data.</param>
        /// <param name="mean">The mean of the array, if already known.</param>
        /// <param name="unbiased">
        ///   Pass true to compute the sample variance; or pass false to compute the 
        ///   population variance. For <see cref="WeightType.Repetition">integers weights
        ///   </see>, the bias correction is equivalent to the non-weighted case. For 
        ///   <see cref="WeightType.Fraction">fractional weights</see>, the variance
        ///   bias cannot be completely eliminated.</param>
        /// 
        /// <returns>The variance of the given data.</returns>
        /// 
        public static double WeightedVariance(this double[] values, int[] weights, double mean,
            bool unbiased)
        {
            if (values.Length != weights.Length)
            {
                throw new DimensionMismatchException("weights",
                    "The values and weight vectors must have the same length");
            }

            // http://en.wikipedia.org/wiki/Weighted_variance#Weighted_sample_variance
            // http://www.gnu.org/software/gsl/manual/html_node/Weighted-Samples.html

            double variance = 0.0;
            int weightSum = 0;

            for (int i = 0; i < values.Length; i++)
            {
                double z = values[i] - mean;
                int w = weights[i];

                variance += w * (z * z);

                weightSum += w;
            }

            if (unbiased)
                return variance / (weightSum - 1.0);

            return variance / weightSum;
        }

        /// <summary>
        ///   Calculates the matrix Variance vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose variances will be calculated.</param>
        /// <param name="weights">An unit vector containing the importance of each sample
        /// in <see param="values"/>. The sum of this array elements should add up to 1.</param>
        /// 
        /// <returns>Returns a vector containing the variances of the given matrix.</returns>
        /// 
        public static double[] WeightedVariance(this double[][] matrix, double[] weights)
        {
            return WeightedVariance(matrix, weights, WeightedMean(matrix, weights), true);
        }

        /// <summary>
        ///   Calculates the matrix Variance vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose variances will be calculated.</param>
        /// <param name="weights">An unit vector containing the importance of each sample
        /// in <see param="values"/>. The sum of this array elements should add up to 1.</param>
        /// <param name="means">The mean vector containing already calculated means for each column of the matrix.</param>
        /// 
        /// <returns>Returns a vector containing the variances of the given matrix.</returns>
        /// 
        public static double[] WeightedVariance(this double[][] matrix, double[] weights, double[] means)
        {
            return WeightedVariance(matrix, weights, means, true);
        }

        /// <summary>
        ///   Calculates the matrix Variance vector.
        /// </summary>
        /// <param name="matrix">A matrix whose variances will be calculated.</param>
        /// <param name="weights">An unit vector containing the importance of each sample
        /// in <see param="values"/>. The sum of this array elements should add up to 1.</param>
        /// <param name="means">The mean vector containing already calculated means for each column of the matrix.</param>
        /// <param name="weightType">How the weights should be interpreted for the bias correction.</param>
        /// <param name="unbiased">
        ///   Pass true to compute the sample variance; or pass false to compute the 
        ///   population variance. For <see cref="WeightType.Repetition">integers weights
        ///   </see>, the bias correction is equivalent to the non-weighted case. For 
        ///   <see cref="WeightType.Fraction">fractional weights</see>, the variance
        ///   bias cannot be completely eliminated.</param>
        /// 
        /// <returns>Returns a vector containing the variances of the given matrix.</returns>
        /// 
        public static double[] WeightedVariance(this double[][] matrix, double[] weights, double[] means,
            bool unbiased, WeightType weightType = WeightType.Fraction)
        {
            int rows = matrix.Length;

            if (rows != weights.Length)
            {
                throw new DimensionMismatchException("weights",
                    "The values and weight vectors must have the same length");
            }

            if (rows == 0)
                return new double[0];

            int cols = matrix[0].Length;

            double[] variance = new double[cols];

            // for each column (for each variable)
            for (int j = 0; j < variance.Length; j++)
            {
                double sum = 0.0;
                double weightSum = 0.0;
                double squareSum = 0.0;

                for (int i = 0; i < matrix.Length; i++)
                {
                    double z = matrix[i][j] - means[j];
                    double w = weights[i];

                    sum += w * (z * z);

                    weightSum += w;
                    squareSum += w * w;
                }

                variance[j] = correct(unbiased, weightType, sum, weightSum, squareSum);
            }

            return variance;
        }


        /// <summary>
        ///   Calculates the matrix Variance vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose variances will be calculated.</param>
        /// <param name="weights">An unit vector containing the importance of each sample
        /// in <see param="values"/>. The sum of this array elements should add up to 1.</param>
        /// 
        /// <returns>Returns a vector containing the variances of the given matrix.</returns>
        /// 
        public static double[] WeightedVariance(this double[,] matrix, double[] weights)
        {
            return WeightedVariance(matrix, weights, WeightedMean(matrix, weights), true);
        }

        /// <summary>
        ///   Calculates the matrix Variance vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose variances will be calculated.</param>
        /// <param name="weights">An unit vector containing the importance of each sample
        /// in <see param="values"/>.</param>
        /// <param name="weightType">How the weights should be interpreted for the bias correction.</param>
        /// 
        /// <returns>Returns a vector containing the variances of the given matrix.</returns>
        /// 
        public static double[] WeightedVariance(this double[,] matrix, double[] weights, WeightType weightType)
        {
            return WeightedVariance(matrix, weights, WeightedMean(matrix, weights), true, weightType);
        }

        /// <summary>
        ///   Calculates the matrix Variance vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose variances will be calculated.</param>
        /// <param name="weights">An unit vector containing the importance of each sample
        /// in <see param="values"/>. The sum of this array elements should add up to 1.</param>
        /// <param name="means">The mean vector containing already calculated means for each column of the matrix.</param>
        /// 
        /// <returns>Returns a vector containing the variances of the given matrix.</returns>
        /// 
        public static double[] WeightedVariance(this double[,] matrix, double[] weights, double[] means)
        {
            return WeightedVariance(matrix, weights, means, true);
        }

        /// <summary>
        ///   Calculates the matrix Variance vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose variances will be calculated.</param>
        /// <param name="weights">An unit vector containing the importance of each sample
        /// in <see param="values"/>. The sum of this array elements should add up to 1.</param>
        /// <param name="means">The mean vector containing already calculated means for each column of the matrix.</param>
        /// <param name="weightType">How the weights should be interpreted for the bias correction.</param>
        /// 
        /// <returns>Returns a vector containing the variances of the given matrix.</returns>
        /// 
        public static double[] WeightedVariance(this double[,] matrix, double[] weights, double[] means, WeightType weightType)
        {
            return WeightedVariance(matrix, weights, means, true, weightType);
        }

        /// <summary>
        ///   Calculates the matrix Variance vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose variances will be calculated.</param>
        /// <param name="weights">An unit vector containing the importance of each sample
        ///   in <see param="values"/>. How those values are interpreted depend on the
        ///   value for <paramref name="weightType"/>.</param>
        /// <param name="means">The mean vector containing already calculated means for each column of the matrix.</param>
        /// <param name="unbiased">
        ///   Pass true to compute the sample variance; or pass false to compute the 
        ///   population variance. For <see cref="WeightType.Repetition">integers weights
        ///   </see>, the bias correction is equivalent to the non-weighted case. For 
        ///   <see cref="WeightType.Fraction">fractional weights</see>, the variance
        ///   bias cannot be completely eliminated.</param>
        /// <param name="weightType">How the weights should be interpreted for the bias correction.</param>
        /// 
        /// <returns>Returns a vector containing the variances of the given matrix.</returns>
        /// 
        public static double[] WeightedVariance(this double[,] matrix, double[] weights, double[] means,
            bool unbiased, WeightType weightType = WeightType.Fraction)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (rows != weights.Length)
            {
                throw new DimensionMismatchException("weights",
                    "The values and weight vectors must have the same length.");
            }

            double[] variance = new double[cols];

            // for each column (for each variable)
            for (int j = 0; j < variance.Length; j++)
            {
                double sum = 0.0;
                double weightSum = 0.0;
                double squareSum = 0.0;

                for (int i = 0; i < rows; i++)
                {
                    double z = matrix[i, j] - means[j];
                    double w = weights[i];

                    sum += w * (z * z);

                    weightSum += w;
                    squareSum += w * w;
                }

                variance[j] = correct(unbiased, weightType, sum, weightSum, squareSum);
            }

            return variance;
        }

        /// <summary>
        ///   Calculates the matrix Variance vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose variances will be calculated.</param>
        /// <param name="weights">The number of times each sample should be repeated.</param>
        /// 
        /// <returns>Returns a vector containing the variances of the given matrix.</returns>
        /// 
        public static double[] WeightedVariance(this double[][] matrix, int[] weights)
        {
            return WeightedVariance(matrix, weights, WeightedMean(matrix, weights), true);
        }

        /// <summary>
        ///   Calculates the matrix Variance vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose variances will be calculated.</param>
        /// <param name="means">The mean vector containing already calculated means for each column of the matrix.</param>
        /// <param name="weights">The number of times each sample should be repeated.</param>
        /// 
        /// <returns>Returns a vector containing the variances of the given matrix.</returns>
        /// 
        public static double[] WeightedVariance(this double[][] matrix, int[] weights, double[] means)
        {
            return WeightedVariance(matrix, weights, means, true);
        }

        /// <summary>
        ///   Calculates the matrix Variance vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose variances will be calculated.</param>
        /// <param name="means">The mean vector containing already calculated means for each column of the matrix.</param>
        /// <param name="weights">The number of times each sample should be repeated.</param>
        /// <param name="unbiased">
        ///   Pass true to compute the sample variance; or pass false to compute the 
        ///   population variance. For <see cref="WeightType.Repetition">integers weights
        ///   </see>, the bias correction is equivalent to the non-weighted case. For 
        ///   <see cref="WeightType.Fraction">fractional weights</see>, the variance
        ///   bias cannot be completely eliminated.</param>
        /// 
        /// <returns>Returns a vector containing the variances of the given matrix.</returns>
        /// 
        public static double[] WeightedVariance(this double[][] matrix, int[] weights, double[] means, bool unbiased)
        {
            int rows = matrix.Length;

            if (rows != weights.Length)
            {
                throw new DimensionMismatchException("weights",
                    "The values and weight vectors must have the same length");
            }

            if (rows == 0)
                return new double[0];

            int cols = matrix[0].Length;

            double[] variance = new double[cols];

            // for each column (for each variable)
            for (int j = 0; j < variance.Length; j++)
            {
                double sum = 0.0;
                double weightSum = 0.0;
                double squareSum = 0.0;

                for (int i = 0; i < matrix.Length; i++)
                {
                    double z = matrix[i][j] - means[j];
                    double w = weights[i];

                    sum += w * (z * z);

                    weightSum += w;
                    squareSum += w * w;
                }

                if (unbiased)
                    variance[j] = sum / (weightSum - 1);
                else
                    variance[j] = sum / weightSum;
            }

            return variance;
        }

        /// <summary>
        ///   Calculates the matrix Variance vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose variances will be calculated.</param>
        /// <param name="weights">The number of times each sample should be repeated.</param>
        /// 
        /// <returns>Returns a vector containing the variances of the given matrix.</returns>
        /// 
        public static double[] WeightedVariance(this double[,] matrix, int[] weights)
        {
            return WeightedVariance(matrix, weights, WeightedMean(matrix, weights), true);
        }

        /// <summary>
        ///   Calculates the matrix Variance vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose variances will be calculated.</param>
        /// <param name="means">The mean vector containing already calculated means for each column of the matrix.</param>
        /// <param name="weights">The number of times each sample should be repeated.</param>
        /// 
        /// <returns>Returns a vector containing the variances of the given matrix.</returns>
        /// 
        public static double[] WeightedVariance(this double[,] matrix, int[] weights, double[] means)
        {
            return WeightedVariance(matrix, weights, means, true);
        }

        /// <summary>
        ///   Calculates the matrix Variance vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose variances will be calculated.</param>
        /// <param name="means">The mean vector containing already calculated means for each column of the matrix.</param>
        /// <param name="weights">The number of times each sample should be repeated.</param>
        /// <param name="unbiased">
        ///   Pass true to compute the sample variance; or pass false to compute the 
        ///   population variance. For <see cref="WeightType.Repetition">integers weights
        ///   </see>, the bias correction is equivalent to the non-weighted case. For 
        ///   <see cref="WeightType.Fraction">fractional weights</see>, the variance
        ///   bias cannot be completely eliminated.</param>
        /// 
        /// <returns>Returns a vector containing the variances of the given matrix.</returns>
        /// 
        public static double[] WeightedVariance(this double[,] matrix, int[] weights, double[] means, bool unbiased)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (rows != weights.Length)
            {
                throw new DimensionMismatchException("weights",
                    "The values and weight vectors must have the same length.");
            }

            double[] variance = new double[cols];

            // for each column (for each variable)
            for (int j = 0; j < variance.Length; j++)
            {
                double sum = 0.0;
                double weightSum = 0.0;
                double squareSum = 0.0;

                for (int i = 0; i < matrix.Length; i++)
                {
                    double z = matrix[i, j] - means[j];
                    double w = weights[i];

                    sum += w * (z * z);

                    weightSum += w;
                    squareSum += w * w;
                }

                if (unbiased)
                    variance[j] = sum / (weightSum - 1);
                else
                    variance[j] = sum / weightSum;
            }

            return variance;
        }

        /// <summary>
        ///   Computes the Mode of the given values.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector values.</param>
        /// <param name="weights">The number of times each sample should be repeated.</param>
        /// <param name="inPlace">True to perform the operation in place, altering the original input vector.</param>
        /// <param name="alreadySorted">Pass true if the list of values is already sorted.</param>
        /// 
        /// <returns>The most common value in the given data.</returns>
        /// 
        public static T WeightedMode<T>(this T[] values, double[] weights, bool inPlace = false, bool alreadySorted = false)
        {
            if (values.Length == 0)
                throw new ArgumentException("The values vector cannot be empty.", "values");

            if (values[0] is IComparable)
                return weighted_mode_sort<T>(values, weights, inPlace, alreadySorted);

            return weighted_mode_bag<T>(values, weights);
        }

        /// <summary>
        ///   Computes the Mode of the given values.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector values.</param>
        /// <param name="weights">The number of times each sample should be repeated.</param>
        /// <param name="inPlace">True to perform the operation in place, altering the original input vector.</param>
        /// <param name="alreadySorted">Pass true if the list of values is already sorted.</param>
        /// 
        /// <returns>The most common value in the given data.</returns>
        /// 
        public static T WeightedMode<T>(this T[] values, int[] weights, bool inPlace = false, bool alreadySorted = false)
        {
            if (values.Length == 0)
                throw new ArgumentException("The values vector cannot be empty.", "values");

            if (values[0] is IComparable)
                return weighted_mode_sort<T>(values, weights, inPlace, alreadySorted);

            return weighted_mode_bag<T>(values, weights);
        }

        private static T weighted_mode_bag<T>(T[] values, double[] weights)
        {
            var bestValue = values[0];
            double bestCount = 1;

            var set = new Dictionary<T, double>();

            for (int i = 0; i < values.Length; i++)
            {
                T v = values[i];

                double count;
                if (!set.TryGetValue(v, out count))
                    count = weights[i];
                else
                    count = count + weights[i];

                set[v] = count;

                if (count > bestCount)
                {
                    bestCount = count;
                    bestValue = v;
                }
            }

            return bestValue;
        }

        private static T weighted_mode_sort<T>(T[] values, double[] weights, bool inPlace, bool alreadySorted)
        {
            if (!alreadySorted)
            {
                if (!inPlace)
                    values = (T[])values.Clone();
                Array.Sort(values);
            }

            var currentValue = values[0];
            double currentCount = weights[0];

            var bestValue = currentValue;
            double bestCount = currentCount;


            for (int i = 1; i < values.Length; i++)
            {
                if (currentValue.Equals(values[i]))
                {
                    currentCount += weights[i];
                }
                else
                {
                    currentValue = values[i];
                    currentCount = weights[i];
                }

                if (currentCount > bestCount)
                {
                    bestCount = currentCount;
                    bestValue = currentValue;
                }
            }

            return bestValue;
        }

        private static T weighted_mode_bag<T>(T[] values, int[] weights)
        {
            var bestValue = values[0];
            int bestCount = 1;

            var set = new Dictionary<T, int>();

            for (int i = 0; i < values.Length; i++)
            {
                T v = values[i];

                int count;
                if (!set.TryGetValue(v, out count))
                    count = weights[i];
                else
                    count = count + weights[i];

                set[v] = count;

                if (count > bestCount)
                {
                    bestCount = count;
                    bestValue = v;
                }
            }

            return bestValue;
        }

        private static T weighted_mode_sort<T>(T[] values, int[] weights, bool inPlace, bool alreadySorted)
        {
            if (!alreadySorted)
            {
                if (!inPlace)
                    values = (T[])values.Clone();
                Array.Sort(values);
            }

            var currentValue = values[0];
            var currentCount = weights[0];

            var bestValue = currentValue;
            int bestCount = currentCount;


            for (int i = 1; i < values.Length; i++)
            {
                if (currentValue.Equals(values[i]))
                {
                    currentCount += weights[i];
                }
                else
                {
                    currentValue = values[i];
                    currentCount = weights[i];
                }

                if (currentCount > bestCount)
                {
                    bestCount = currentCount;
                    bestValue = currentValue;
                }
            }

            return bestValue;
        }

        /// <summary>
        ///   Gets the maximum value in a vector of observations that has a weight higher than zero.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector values.</param>
        /// <param name="weights">A vector containing the importance of each sample in <see param="values"/>.</param>
        /// <param name="imax">The index of the maximum element in the vector, or -1 if it could not be found.</param>
        /// <param name="alreadySorted">Pass true if the list of values is already sorted.</param>
        /// 
        /// <returns>The maximum value in the given data.</returns>
        /// 
        public static double WeightedMax(this double[] values, double[] weights, out int imax, bool alreadySorted = false)
        {
            imax = -1;

            if (alreadySorted)
            {
                // Look for the last value whose weight is different from zero
                for (int i = weights.Length - 1; i >= 0; i--)
                {
                    if (weights[i] > 0)
                    {
                        imax = i;
                        break;
                    }
                }

                return values[imax];
            }

            // Base case for non sorted arrays
            double max = Double.NegativeInfinity;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > max && weights[i] > 0)
                {
                    max = values[i];
                    imax = i;
                }
            }

            return max;
        }

        /// <summary>
        ///   Gets the minimum value in a vector of observations that has a weight higher than zero.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector values.</param>
        /// <param name="weights">A vector containing the importance of each sample in <see param="values"/>.</param>
        /// <param name="imin">The index of the minimum element in the vector, or -1 if it could not be found.</param>
        /// <param name="alreadySorted">Pass true if the list of values is already sorted.</param>
        /// 
        /// <returns>The minimum value in the given data.</returns>
        /// 
        public static double WeightedMin(this double[] values, double[] weights, out int imin, bool alreadySorted = false)
        {
            imin = -1;

            if (alreadySorted)
            {
                // Look for the first value whose weight is different from zero
                for (int i = 0; i < weights.Length; i++)
                {
                    if (weights[i] > 0)
                    {
                        imin = i;
                        return values[imin];
                    }
                }

                return Double.PositiveInfinity;
            }


            // Base case for non sorted arrays
            double min = Double.PositiveInfinity;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] < min && weights[i] > 0)
                {
                    min = values[i];
                    imin = i;
                }
            }

            return min;
        }

        /// <summary>
        ///   Gets the maximum value in a vector of observations that has a weight higher than zero.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector values.</param>
        /// <param name="weights">The number of times each sample should be repeated.</param>
        /// <param name="imax">The index of the maximum element in the vector, or -1 if it could not be found.</param>
        /// <param name="alreadySorted">Pass true if the list of values is already sorted.</param>
        /// 
        /// <returns>The maximum value in the given data.</returns>
        /// 
        public static double WeightedMax(this double[] values, int[] weights, out int imax, bool alreadySorted = false)
        {
            imax = -1;

            if (alreadySorted)
            {
                // Look for the last value whose weight is different from zero
                for (int i = weights.Length - 1; i >= 0; i--)
                {
                    if (weights[i] > 0)
                    {
                        imax = i;
                        break;
                    }
                }

                return values[imax];
            }

            // Base case for non sorted arrays
            double max = Double.NegativeInfinity;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > max && weights[i] > 0)
                {
                    max = values[i];
                    imax = i;
                }
            }

            return max;
        }

        /// <summary>
        ///   Gets the minimum value in a vector of observations that has a weight higher than zero.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector values.</param>
        /// <param name="weights">The number of times each sample should be repeated.</param>
        /// <param name="imin">The index of the minimum element in the vector, or -1 if it could not be found.</param>
        /// <param name="alreadySorted">Pass true if the list of values is already sorted.</param>
        /// 
        /// <returns>The minimum value in the given data.</returns>
        /// 
        public static double WeightedMin(this double[] values, int[] weights, out int imin, bool alreadySorted = false)
        {
            imin = -1;

            if (alreadySorted)
            {
                // Look for the first value whose weight is different from zero
                for (int i = 0; i < weights.Length; i++)
                {
                    if (weights[i] > 0)
                    {
                        imin = i;
                        return values[imin];
                    }
                }

                return Double.PositiveInfinity;
            }


            // Base case for non sorted arrays
            double min = Double.PositiveInfinity;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] < min && weights[i] > 0)
                {
                    min = values[i];
                    imin = i;
                }
            }

            return min;
        }

        private static void Validate(double[] vector, int window, double alpha)
        {
            // Perform some basic error validation
            if (vector == null)
                throw new ArgumentNullException("vector", "The vector cannot be null.");

            if (alpha < 0 || alpha > 1)
            {
                string message = string.Format(
                    "Alpha must lie in the interval [0, 1] but was {0}", alpha);

                throw new ArgumentOutOfRangeException("alpha", message);
            }

            int numSamples = vector.Length;

            if (window <= 0 || window > numSamples)
            {
                string message = string.Format(
                    "Window size ({0}) must be less than or equal to the total number of samples ({1})",
                    window,
                    numSamples);

                throw new ArgumentOutOfRangeException("window", message);
            }
        }
    }
}

