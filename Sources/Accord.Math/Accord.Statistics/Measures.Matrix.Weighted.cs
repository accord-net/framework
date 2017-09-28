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

    /// <summary>
    ///   Sample weight types.
    /// </summary>
    /// 
    public enum WeightType
    {
        /// <summary>
        ///   Weights should be ignored.
        /// </summary>
        /// 
        None,

        /// <summary>
        ///   Weights are integers representing how many times a sample should repeat itself.
        /// </summary>
        /// 
        Repetition,

        /// <summary>
        ///   Weights are fractional numbers that sum up to one.
        /// </summary>
        /// 
        Fraction,

        /// <summary>
        ///   If weights sum up to one, they are handled as <see cref="Fraction">fractional
        ///   weights</see>. If they sum to a whole number, they are handled as <see cref="Repetition">
        ///   integer repetition counts</see>.
        /// </summary>
        /// 
        Automatic,
    }

    public static partial class Measures
    {

        /// <summary>
        ///   Calculates the weighted matrix Mean vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose means will be calculated.</param>
        /// <param name="weights">A vector containing the importance of each sample in the matrix.</param>
        /// 
        /// <returns>Returns a vector containing the means of the given matrix.</returns>
        /// 
        public static double[] WeightedMean(this double[][] matrix, double[] weights)
        {
            return WeightedMean(matrix, weights, 0);
        }

        /// <summary>
        ///   Calculates the weighted matrix Mean vector.
        /// </summary>
        /// <param name="matrix">A matrix whose means will be calculated.</param>
        /// <param name="weights">A vector containing the importance of each sample in the matrix.</param>
        /// <param name="dimension">
        ///   The dimension along which the means will be calculated. Pass
        ///   0 to compute a row vector containing the mean of each column,
        ///   or 1 to compute a column vector containing the mean of each row.
        ///   Default value is 0.
        /// </param>
        /// <returns>Returns a vector containing the means of the given matrix.</returns>
        /// 
        public static double[] WeightedMean(this double[][] matrix, double[] weights, int dimension = 0)
        {
            int rows = matrix.Length;

            if (rows == 0)
                return new double[0];

            int cols = matrix[0].Length;

            double[] mean;

            if (dimension == 0)
            {
                mean = new double[cols];

                if (rows != weights.Length)
                {
                    throw new DimensionMismatchException("weights",
                        "The number of rows and weights must match.");
                }

                // for each row
                for (int i = 0; i < rows; i++)
                {
                    double[] row = matrix[i];
                    double w = weights[i];

                    // for each column
                    for (int j = 0; j < cols; j++)
                        mean[j] += row[j] * w;
                }
            }
            else if (dimension == 1)
            {
                mean = new double[rows];

                if (cols != weights.Length)
                {
                    throw new DimensionMismatchException("weights",
                        "The number of columns and weights must match.");
                }

                // for each row
                for (int j = 0; j < rows; j++)
                {
                    double[] row = matrix[j];
                    double w = weights[j];

                    // for each column
                    for (int i = 0; i < cols; i++)
                        mean[j] += row[i] * w;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension.", "dimension");
            }

            double weightSum = weights.Sum();

            if (weightSum != 0)
                for (int i = 0; i < mean.Length; i++)
                    mean[i] /= weightSum;

            return mean;
        }

        /// <summary>
        ///   Calculates the weighted matrix Mean vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose means will be calculated.</param>
        /// <param name="weights">A vector containing the importance of each sample in the matrix.</param>
        /// 
        /// <returns>Returns a vector containing the means of the given matrix.</returns>
        /// 
        public static double[] WeightedMean(this double[,] matrix, double[] weights)
        {
            return WeightedMean(matrix, weights, 0);
        }

        /// <summary>
        ///   Calculates the weighted matrix Mean vector.
        /// </summary>
        /// <param name="matrix">A matrix whose means will be calculated.</param>
        /// <param name="weights">A vector containing the importance of each sample in the matrix.</param>
        /// <param name="dimension">
        ///   The dimension along which the means will be calculated. Pass
        ///   0 to compute a row vector containing the mean of each column,
        ///   or 1 to compute a column vector containing the mean of each row.
        ///   Default value is 0.
        /// </param>
        /// <returns>Returns a vector containing the means of the given matrix.</returns>
        /// 
        public static double[] WeightedMean(this double[,] matrix, double[] weights, int dimension = 0)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[] mean;

            if (dimension == 0)
            {
                mean = new double[cols];

                if (rows != weights.Length)
                {
                    throw new DimensionMismatchException("weights",
                        "The number of rows and weights must match.");
                }

                // for each row
                for (int i = 0; i < rows; i++)
                {
                    double w = weights[i];

                    // for each column
                    for (int j = 0; j < cols; j++)
                        mean[j] += matrix[i, j] * w;
                }
            }
            else if (dimension == 1)
            {
                mean = new double[rows];

                if (cols != weights.Length)
                {
                    throw new DimensionMismatchException("weights",
                        "The number of columns and weights must match.");
                }

                // for each row
                for (int j = 0; j < rows; j++)
                {
                    double w = weights[j];

                    // for each column
                    for (int i = 0; i < cols; i++)
                        mean[j] += matrix[j, i] * w;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension.", "dimension");
            }


            double weightSum = weights.Sum();

            if (weightSum != 0)
                for (int i = 0; i < mean.Length; i++)
                    mean[i] /= weightSum;

            return mean;
        }

        /// <summary>
        ///   Calculates the weighted matrix Mean vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose means will be calculated.</param>
        /// <param name="weights">A vector containing the importance of each sample in the matrix.</param>
        /// 
        /// <returns>Returns a vector containing the means of the given matrix.</returns>
        /// 
        public static double[] WeightedMean(this double[][] matrix, int[] weights)
        {
            return WeightedMean(matrix, weights, 0);
        }

        /// <summary>
        ///   Calculates the weighted matrix Mean vector.
        /// </summary>
        /// <param name="matrix">A matrix whose means will be calculated.</param>
        /// <param name="weights">A vector containing the importance of each sample in the matrix.</param>
        /// <param name="dimension">
        ///   The dimension along which the means will be calculated. Pass
        ///   0 to compute a row vector containing the mean of each column,
        ///   or 1 to compute a column vector containing the mean of each row.
        ///   Default value is 0.
        /// </param>
        /// <returns>Returns a vector containing the means of the given matrix.</returns>
        /// 
        public static double[] WeightedMean(this double[][] matrix, int[] weights, int dimension = 0)
        {
            int rows = matrix.Length;

            if (rows == 0)
                return new double[0];

            int cols = matrix[0].Length;

            double[] mean;

            if (dimension == 0)
            {
                mean = new double[cols];

                if (rows != weights.Length)
                {
                    throw new DimensionMismatchException("weights",
                        "The number of rows and weights must match.");
                }

                // for each row
                for (int i = 0; i < rows; i++)
                {
                    double[] row = matrix[i];
                    double w = weights[i];

                    // for each column
                    for (int j = 0; j < cols; j++)
                        mean[j] += row[j] * w;
                }
            }
            else if (dimension == 1)
            {
                mean = new double[rows];

                if (cols != weights.Length)
                {
                    throw new DimensionMismatchException("weights",
                        "The number of columns and weights must match.");
                }

                // for each row
                for (int j = 0; j < rows; j++)
                {
                    double[] row = matrix[j];
                    double w = weights[j];

                    // for each column
                    for (int i = 0; i < cols; i++)
                        mean[j] += row[i] * w;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension.", "dimension");
            }

            double weightSum = weights.Sum();

            if (weightSum != 0)
                for (int i = 0; i < mean.Length; i++)
                    mean[i] /= weightSum;

            return mean;
        }

        /// <summary>
        ///   Calculates the weighted matrix Mean vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose means will be calculated.</param>
        /// <param name="weights">A vector containing the importance of each sample in the matrix.</param>
        /// 
        /// <returns>Returns a vector containing the means of the given matrix.</returns>
        /// 
        public static double[] WeightedMean(this double[,] matrix, int[] weights)
        {
            return WeightedMean(matrix, weights, 0);
        }

        /// <summary>
        ///   Calculates the weighted matrix Mean vector.
        /// </summary>
        /// <param name="matrix">A matrix whose means will be calculated.</param>
        /// <param name="weights">A vector containing the importance of each sample in the matrix.</param>
        /// <param name="dimension">
        ///   The dimension along which the means will be calculated. Pass
        ///   0 to compute a row vector containing the mean of each column,
        ///   or 1 to compute a column vector containing the mean of each row.
        ///   Default value is 0.
        /// </param>
        /// <returns>Returns a vector containing the means of the given matrix.</returns>
        /// 
        public static double[] WeightedMean(this double[,] matrix, int[] weights, int dimension = 0)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[] mean;

            if (dimension == 0)
            {
                mean = new double[cols];

                if (rows != weights.Length)
                {
                    throw new DimensionMismatchException("weights",
                        "The number of rows and weights must match.");
                }

                // for each row
                for (int i = 0; i < rows; i++)
                {
                    double w = weights[i];

                    // for each column
                    for (int j = 0; j < cols; j++)
                        mean[j] += matrix[i, j] * w;
                }
            }
            else if (dimension == 1)
            {
                mean = new double[rows];

                if (cols != weights.Length)
                {
                    throw new DimensionMismatchException("weights",
                        "The number of columns and weights must match.");
                }

                // for each row
                for (int j = 0; j < rows; j++)
                {
                    double w = weights[j];

                    // for each column
                    for (int i = 0; i < cols; i++)
                        mean[j] += matrix[j, i] * w;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension.", "dimension");
            }


            double weightSum = weights.Sum();

            if (weightSum != 0)
                for (int i = 0; i < mean.Length; i++)
                    mean[i] /= weightSum;

            return mean;
        }

        /// <summary>
        ///   Calculates the exponentially weighted mean vector.
        /// </summary>
        /// 
        /// <param name="matrix">
        ///   A matrix of observations whose EW mean vector will be calculated. It is assumed 
        ///   that the matrix is ordered with the most recent observations at the bottom of 
        ///   the matrix.
        /// </param>
        /// <param name="alpha">
        ///   The weighting to be applied to the calculation. A higher alpha discounts
        ///   older observations faster. Alpha must be between 0 and 1 (inclusive).
        /// </param>
        /// 
        /// <returns>
        ///   Returns a vector containing the exponentially weighted average of the columns of 
        ///   the given matrix.
        /// </returns>
        /// 
        /// <example>
        /// <para>
        ///   The following example shows how to compute the EW mean.</para>
        ///   
        /// <code source="Unit Tests\Accord.Tests.Math\Accord.Statistics\MeasuresTest.cs" region="doc_example1" />
        /// </example>
        public static double[] ExponentialWeightedMean(this double[][] matrix, double alpha = 0)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix", "The matrix cannot be null.");

            return matrix.ExponentialWeightedMean(matrix.Rows(), alpha);
        }

        /// <summary>
        ///   Calculates the exponentially weighted mean vector.
        /// </summary>
        /// 
        /// <param name="matrix">
        ///   A matrix of observations whose EW mean vector will be calculated. It is assumed 
        ///   that the matrix is ordered with the most recent observations at the bottom of 
        ///   the matrix.
        /// </param>
        /// <param name="window">The number of rows to be used in the calculation.</param>
        /// <param name="alpha">
        ///   The weighting to be applied to the calculation. A higher alpha discounts
        ///   older observations faster. Alpha must be between 0 and 1 (inclusive).
        /// </param>
        /// 
        /// <returns>
        ///   Returns a vector containing the exponentially weighted average of the columns of 
        ///   the given matrix.
        /// </returns>
        /// 
        /// <example>
        /// <para>
        ///   The following example shows how to compute the EW mean.</para>
        ///   
        /// <code source="Unit Tests\Accord.Tests.Math\Accord.Statistics\MeasuresTest.cs" region="doc_example1" />
        /// </example>
        public static double[] ExponentialWeightedMean(this double[][] matrix, int window, double alpha = 0)
        {
            // Perform some basic error validation
            Validate(matrix, window, alpha);

            // Handle the trivial case
            if (alpha == 1)
                return matrix.GetRow(-1);

            double[][] truncatedSeries = window == matrix.Rows()
                ? truncatedSeries = matrix
                : truncatedSeries = matrix.Get(-window, 0);

            if (alpha == 0)
                return truncatedSeries.Mean(0);

            // Now we create the weights
            double[] decayWeights = GetDecayWeights(window, alpha);

            return truncatedSeries.WeightedMean(decayWeights);
        }

        /// <summary>
        ///   Calculates the exponentially weighted covariance matrix.
        /// </summary>
        /// 
        /// <param name="matrix">
        ///   A matrix of observations whose EW covariance matrix will be calculated. It 
        ///   is assumed that the matrix is ordered with the most recent observations at 
        ///   the bottom of the matrix.
        /// </param>
        /// <param name="alpha">
        ///   The weighting to be applied to the calculation. A higher alpha discounts
        ///   older observations faster. Alpha must be between 0 and 1 (inclusive).
        /// </param>
        /// <param name="unbiased">Use a standard estimation bias correction.</param>
        /// 
        /// <returns>
        ///   Returns a vector containing the exponentially weighted average of the columns of 
        ///   the given matrix.
        /// </returns>
        /// 
        /// <example>
        /// <para>
        ///   The following example shows how to compute the EW covariance matrix.</para>
        ///   
        /// <code source="Unit Tests\Accord.Tests.Math\Accord.Statistics\MeasuresTest.cs" region="doc_example2" />
        /// </example>
        public static double[,] ExponentialWeightedCovariance(
            this double[][] matrix, double alpha = 0, bool unbiased = false)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix", "The matrix cannot be null.");

            return matrix.ExponentialWeightedCovariance(matrix.Rows(), alpha, unbiased);
        }

        /// <summary>
        ///   Calculates the exponentially weighted covariance matrix.
        /// </summary>
        /// 
        /// <param name="matrix">
        ///   A matrix of observations whose EW covariance matrix will be calculated. It 
        ///   is assumed that the matrix is ordered with the most recent observations at 
        ///   the bottom of the matrix.
        /// </param>
        /// <param name="window">The number of rows to be used in the calculation.</param>
        /// <param name="alpha">
        ///   The weighting to be applied to the calculation. A higher alpha discounts
        ///   older observations faster. Alpha must be between 0 and 1 (inclusive).
        /// </param>
        /// <param name="unbiased">Use a standard estimation bias correction.</param>
        /// 
        /// <returns>
        ///   Returns a vector containing the exponentially weighted average of the columns of 
        ///   the given matrix.
        /// </returns>
        /// 
        /// <example>
        /// <para>
        ///   The following example shows how to compute the EW covariance matrix.</para>
        ///   
        /// <code source="Unit Tests\Accord.Tests.Math\Accord.Statistics\MeasuresTest.cs" region="doc_example2" />
        /// </example>
        public static double[,] ExponentialWeightedCovariance(
            this double[][] matrix, int window, double alpha = 0, bool unbiased = false)
        {
            // Perform some basic error validation
            Validate(matrix, window, alpha);

            int rows = matrix.Rows();
            int cols = matrix.Columns();

            // Handle the trivial case
            if (alpha == 1)
                return Matrix.Zeros(cols, cols);

            // Now we create the weights
            double[] decayWeights = GetDecayWeights(window, alpha);

            double[][] truncatedSeries = window == rows
                ? truncatedSeries = matrix
                : truncatedSeries = matrix.Get(-window, 0);

            if (unbiased)
                return truncatedSeries.WeightedCovariance(decayWeights);

            double[] weightedMeans = truncatedSeries.WeightedMean(decayWeights);

            double effectiveNumObs = alpha == 0 ? window : ((1 - Math.Pow(1 - alpha, window)) / alpha);

            return truncatedSeries.WeightedScatter(decayWeights, weightedMeans, 1 / effectiveNumObs, 0);
        }

        /// <summary>
        ///   Calculates the matrix Standard Deviations vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose deviations will be calculated.</param>
        /// <param name="weights">The number of times each sample should be repeated.</param>
        /// 
        /// <returns>Returns a vector containing the standard deviations of the given matrix.</returns>
        /// 
        public static double[] WeightedStandardDeviation(this double[,] matrix, int[] weights)
        {
            return WeightedStandardDeviation(matrix, weights, WeightedMean(matrix, weights));
        }

        /// <summary>
        ///   Calculates the matrix Standard Deviations vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose deviations will be calculated.</param>
        /// <param name="means">The mean vector containing already calculated means for each column of the matrix.</param>
        /// <param name="weights">The number of times each sample should be repeated.</param>
        /// 
        /// <returns>Returns a vector containing the standard deviations of the given matrix.</returns>
        /// 
        public static double[] WeightedStandardDeviation(this double[,] matrix, int[] weights, double[] means)
        {
            return Elementwise.Sqrt(WeightedVariance(matrix, weights, means));
        }

        /// <summary>
        ///   Calculates the matrix Standard Deviations vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose deviations will be calculated.</param>
        /// <param name="means">The mean vector containing already calculated means for each column of the matrix.</param>
        /// <param name="unbiased">
        ///   Pass true to compute the standard deviation using the sample variance.
        ///   Pass false to compute it using the population variance. See remarks
        ///   for more details.</param>
        /// <param name="weights">The number of times each sample should be repeated.</param>
        /// 
        /// <remarks>
        ///   <para>
        ///     Setting <paramref name="unbiased"/> to <c>true</c> will make this method 
        ///     compute the standard deviation σ using the sample variance, which is an unbiased 
        ///     estimator of the true population variance. Setting this parameter to true will
        ///     thus compute σ using the following formula:</para>
        ///     <code>
        ///                           N
        ///        σ² = 1 / (N - 1)  ∑   (x_i − μ)²
        ///                           i=1
        ///     </code>
        ///   <para>
        ///     Setting <paramref name="unbiased"/> to <c>false</c> will assume the given values
        ///     already represent the whole population, and will compute the population variance
        ///     using the formula: </para>
        ///     <code>
        ///                           N
        ///        σ² =   (1 / N)    ∑   (x_i − μ)²
        ///                           i=1
        ///     </code>
        /// </remarks>
        ///   
        /// <returns>Returns a vector containing the standard deviations of the given matrix.</returns>
        /// 
        public static double[] WeightedStandardDeviation(this double[][] matrix, int[] weights, double[] means, bool unbiased = true)
        {
            return Elementwise.Sqrt(WeightedVariance(matrix, weights, means, unbiased));
        }

        /// <summary>
        ///   Calculates the matrix Standard Deviations vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose deviations will be calculated.</param>
        /// <param name="unbiased">
        ///   Pass true to compute the standard deviation using the sample variance.
        ///   Pass false to compute it using the population variance. See remarks
        ///   for more details.</param>
        /// <param name="weights">The number of times each sample should be repeated.</param>
        /// 
        /// <remarks>
        ///   <para>
        ///     Setting <paramref name="unbiased"/> to <c>true</c> will make this method 
        ///     compute the standard deviation σ using the sample variance, which is an unbiased 
        ///     estimator of the true population variance. Setting this parameter to true will
        ///     thus compute σ using the following formula:</para>
        ///     <code>
        ///                           N
        ///        σ² = 1 / (N - 1)  ∑   (x_i − μ)²
        ///                           i=1
        ///     </code>
        ///   <para>
        ///     Setting <paramref name="unbiased"/> to <c>false</c> will assume the given values
        ///     already represent the whole population, and will compute the population variance
        ///     using the formula: </para>
        ///     <code>
        ///                           N
        ///        σ² =   (1 / N)    ∑   (x_i − μ)²
        ///                           i=1
        ///     </code>
        /// </remarks>
        /// 
        /// <returns>Returns a vector containing the standard deviations of the given matrix.</returns>
        /// 
        public static double[] WeightedStandardDeviation(this double[][] matrix, int[] weights, bool unbiased = true)
        {
            return WeightedStandardDeviation(matrix, weights, WeightedMean(matrix, weights), unbiased);
        }

        /// <summary>
        ///   Calculates the matrix Standard Deviations vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose deviations will be calculated.</param>
        /// <param name="weights">The number of times each sample should be repeated.</param>
        /// 
        /// <returns>Returns a vector containing the standard deviations of the given matrix.</returns>
        /// 
        public static double[] WeightedStandardDeviation(this double[,] matrix, double[] weights)
        {
            return WeightedStandardDeviation(matrix, weights, WeightedMean(matrix, weights));
        }

        /// <summary>
        ///   Calculates the matrix Standard Deviations vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose deviations will be calculated.</param>
        /// <param name="means">The mean vector containing already calculated means for each column of the matrix.</param>
        /// <param name="weights">The number of times each sample should be repeated.</param>
        /// 
        /// <returns>Returns a vector containing the standard deviations of the given matrix.</returns>
        /// 
        public static double[] WeightedStandardDeviation(this double[,] matrix, double[] weights, double[] means)
        {
            return Elementwise.Sqrt(WeightedVariance(matrix, weights, means));
        }

        /// <summary>
        ///   Calculates the matrix Standard Deviations vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose deviations will be calculated.</param>
        /// <param name="means">The mean vector containing already calculated means for each column of the matrix.</param>
        /// <param name="unbiased">
        ///   Pass true to compute the standard deviation using the sample variance.
        ///   Pass false to compute it using the population variance. See remarks
        ///   for more details.</param>
        /// <param name="weights">The number of times each sample should be repeated.</param>
        ///   
        /// <remarks>
        ///   <para>
        ///     Setting <paramref name="unbiased"/> to <c>true</c> will make this method 
        ///     compute the standard deviation σ using the sample variance, which is an unbiased 
        ///     estimator of the true population variance. Setting this parameter to true will
        ///     thus compute σ using the following formula:</para>
        ///     <code>
        ///                           N
        ///        σ² = 1 / (N - 1)  ∑   (x_i − μ)²
        ///                           i=1
        ///     </code>
        ///   <para>
        ///     Setting <paramref name="unbiased"/> to <c>false</c> will assume the given values
        ///     already represent the whole population, and will compute the population variance
        ///     using the formula: </para>
        ///     <code>
        ///                           N
        ///        σ² =   (1 / N)    ∑   (x_i − μ)²
        ///                           i=1
        ///     </code>
        /// </remarks>
        ///   
        /// <returns>Returns a vector containing the standard deviations of the given matrix.</returns>
        /// 
        public static double[] WeightedStandardDeviation(this double[][] matrix, double[] weights, double[] means, bool unbiased = true)
        {
            return Elementwise.Sqrt(WeightedVariance(matrix, weights, means, unbiased));
        }

        /// <summary>
        ///   Calculates the matrix Standard Deviations vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix whose deviations will be calculated.</param>
        /// <param name="unbiased">
        ///   Pass true to compute the standard deviation using the sample variance.
        ///   Pass false to compute it using the population variance. See remarks
        ///   for more details.</param>
        /// <param name="weights">The number of times each sample should be repeated.</param>
        ///   
        /// <remarks>
        ///   <para>
        ///     Setting <paramref name="unbiased"/> to <c>true</c> will make this method 
        ///     compute the standard deviation σ using the sample variance, which is an unbiased 
        ///     estimator of the true population variance. Setting this parameter to true will
        ///     thus compute σ using the following formula:</para>
        ///     <code>
        ///                           N
        ///        σ² = 1 / (N - 1)  ∑   (x_i − μ)²
        ///                           i=1
        ///     </code>
        ///   <para>
        ///     Setting <paramref name="unbiased"/> to <c>false</c> will assume the given values
        ///     already represent the whole population, and will compute the population variance
        ///     using the formula: </para>
        ///     <code>
        ///                           N
        ///        σ² =   (1 / N)    ∑   (x_i − μ)²
        ///                           i=1
        ///     </code>
        /// </remarks>
        /// 
        /// <returns>Returns a vector containing the standard deviations of the given matrix.</returns>
        /// 
        public static double[] WeightedStandardDeviation(this double[][] matrix, double[] weights, bool unbiased = true)
        {
            return WeightedStandardDeviation(matrix, weights, WeightedMean(matrix, weights), unbiased);
        }

        /// <summary>
        ///   Calculates the covariance matrix of a sample matrix.
        /// </summary>
        /// 
        /// <param name="matrix">A number multi-dimensional array containing the matrix values.</param>
        /// <param name="weights">An unit vector containing the importance of each sample
        /// in <see param="values"/>. The sum of this array elements should add up to 1.</param>
        /// <param name="means">The mean value of the given values, if already known.</param>
        /// 
        /// <returns>The covariance matrix.</returns>
        /// 
        public static double[,] WeightedCovariance(this double[][] matrix, double[] weights, double[] means)
        {
            return WeightedCovariance(matrix, weights, means, dimension: 0);
        }

        /// <summary>
        ///   Calculates the covariance matrix of a sample matrix.
        /// </summary>
        /// 
        /// <param name="matrix">A number multi-dimensional array containing the matrix values.</param>
        /// <param name="dimension">
        ///   Pass 0 to if mean vector is a row vector, 1 otherwise. Default value is 0.
        /// </param>
        /// <param name="weights">An unit vector containing the importance of each sample
        /// in <see param="values"/>. The sum of this array elements should add up to 1.</param>
        /// 
        /// <returns>The covariance matrix.</returns>
        /// 
        public static double[,] WeightedCovariance(this double[][] matrix, double[] weights, int dimension = 0)
        {
            double[] mean = WeightedMean(matrix, weights, dimension);
            return WeightedCovariance(matrix, weights, mean, dimension);
        }

        /// <summary>
        ///   Calculates the covariance matrix of a sample matrix.
        /// </summary>
        /// 
        /// <param name="matrix">A number multi-dimensional array containing the matrix values.</param>
        /// <param name="weights">The number of times each sample should be repeated.</param>
        /// <param name="dimension">
        ///   Pass 0 to if mean vector is a row vector, 1 otherwise. Default value is 0.
        /// </param>
        /// 
        /// <returns>The covariance matrix.</returns>
        /// 
        public static double[,] WeightedCovariance(this double[][] matrix, int[] weights, int dimension = 0)
        {
            double[] mean = WeightedMean(matrix, weights, dimension);
            return WeightedCovariance(matrix, weights, mean, dimension);
        }

        /// <summary>
        ///   Calculates the covariance matrix of a sample matrix.
        /// </summary>
        /// 
        /// <param name="matrix">A number multi-dimensional array containing the matrix values.</param>
        /// <param name="weights">An unit vector containing the importance of each sample
        /// in <see param="values"/>. The sum of this array elements should add up to 1.</param>
        /// <param name="means">The mean value of the given values, if already known.</param>
        /// <param name="dimension">
        ///   Pass 0 to if mean vector is a row vector, 1 otherwise. Default value is 0.
        /// </param>
        /// 
        /// <returns>The covariance matrix.</returns>
        /// 
        public static double[,] WeightedCovariance(this double[][] matrix, double[] weights, double[] means, int dimension)
        {
            double s1 = 0, s2 = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                s1 += weights[i];
                s2 += weights[i] * weights[i];
            }

            double factor = s1 / (s1 * s1 - s2);
            return WeightedScatter(matrix, weights, means, factor, dimension);
        }

        /// <summary>
        ///   Calculates the covariance matrix of a sample matrix.
        /// </summary>
        /// 
        /// <param name="matrix">A number multi-dimensional array containing the matrix values.</param>
        /// <param name="weights">The number of times each sample should be repeated.</param>
        /// <param name="means">The mean value of the given values, if already known.</param>
        /// <param name="dimension">
        ///   Pass 0 to if mean vector is a row vector, 1 otherwise. Default value is 0.
        /// </param>
        /// 
        /// <returns>The covariance matrix.</returns>
        /// 
        public static double[,] WeightedCovariance(this double[][] matrix, int[] weights, double[] means, int dimension)
        {
            double s1 = 0, s2 = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                s1 += weights[i];
                s2 += weights[i] * weights[i];
            }

            double factor = s1 / (s1 * s1 - s2);
            return WeightedScatter(matrix, weights, means, factor, dimension);
        }

        /// <summary>
        ///   Calculates the scatter matrix of a sample matrix.
        /// </summary>
        /// 
        /// <remarks>
        ///   By dividing the Scatter matrix by the sample size, we get the population
        ///   Covariance matrix. By dividing by the sample size minus one, we get the
        ///   sample Covariance matrix.
        /// </remarks>
        /// 
        /// <param name="matrix">A number multi-dimensional array containing the matrix values.</param>
        /// <param name="weights">An unit vector containing the importance of each sample
        /// in <see param="values"/>. The sum of this array elements should add up to 1.</param>
        /// <param name="means">The mean value of the given values, if already known.</param>
        /// <param name="factor">A real number to multiply each member of the matrix.</param>
        /// <param name="dimension">
        ///   Pass 0 to if mean vector is a row vector, 1 otherwise. Default value is 0.
        /// </param>
        /// 
        /// <returns>The covariance matrix.</returns>
        /// 
        public static double[,] WeightedScatter(this double[][] matrix, double[] weights,
            double[] means, double factor, int dimension)
        {
            int rows = matrix.Length;
            if (rows == 0)
                return new double[0, 0];
            int cols = matrix[0].Length;

            double[,] cov;

            if (dimension == 0)
            {
                if (means.Length != cols)
                {
                    throw new DimensionMismatchException("means",
                        "Length of the mean vector should equal the number of columns.");
                }

                if (rows != weights.Length)
                {
                    throw new DimensionMismatchException("weights",
                        "The number of rows and weights must match.");
                }

                cov = new double[cols, cols];
                for (int i = 0; i < cols; i++)
                {
                    for (int j = i; j < cols; j++)
                    {
                        double s = 0.0;
                        for (int k = 0; k < rows; k++)
                            s += weights[k] * (matrix[k][j] - means[j]) * (matrix[k][i] - means[i]);
                        cov[i, j] = s * factor;
                        cov[j, i] = s * factor;
                    }
                }
            }
            else if (dimension == 1)
            {
                if (means.Length != rows)
                {
                    throw new DimensionMismatchException("means",
                        "Length of the mean vector should equal the number of rows.");
                }

                if (cols != weights.Length)
                {
                    throw new DimensionMismatchException("weights",
                        "The number of columns and weights must match.");
                }

                cov = new double[rows, rows];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = i; j < rows; j++)
                    {
                        double s = 0.0;
                        for (int k = 0; k < cols; k++)
                            s += weights[k] * (matrix[j][k] - means[j]) * (matrix[i][k] - means[i]);
                        cov[i, j] = s * factor;
                        cov[j, i] = s * factor;
                    }
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension.", "dimension");
            }

            return cov;
        }

        /// <summary>
        ///   Calculates the scatter matrix of a sample matrix.
        /// </summary>
        /// 
        /// <remarks>
        ///   By dividing the Scatter matrix by the sample size, we get the population
        ///   Covariance matrix. By dividing by the sample size minus one, we get the
        ///   sample Covariance matrix.
        /// </remarks>
        /// 
        /// <param name="weights">The number of times each sample should be repeated.</param>
        /// <param name="matrix">A number multi-dimensional array containing the matrix values.</param>
        /// <param name="means">The mean value of the given values, if already known.</param>
        /// <param name="factor">A real number to multiply each member of the matrix.</param>
        /// <param name="dimension">
        ///   Pass 0 to if mean vector is a row vector, 1 otherwise. Default value is 0.
        /// </param>
        /// 
        /// <returns>The covariance matrix.</returns>
        /// 
        public static double[,] WeightedScatter(this double[][] matrix, int[] weights,
            double[] means, double factor, int dimension)
        {
            int rows = matrix.Length;
            if (rows == 0)
                return new double[0, 0];
            int cols = matrix[0].Length;

            double[,] cov;

            if (dimension == 0)
            {
                if (means.Length != cols)
                {
                    throw new DimensionMismatchException("means",
                        "Length of the mean vector should equal the number of columns.");
                }

                if (rows != weights.Length)
                {
                    throw new DimensionMismatchException("weights",
                        "The number of rows and weights must match.");
                }

                cov = new double[cols, cols];
                for (int i = 0; i < cols; i++)
                {
                    for (int j = i; j < cols; j++)
                    {
                        double s = 0.0;
                        for (int k = 0; k < rows; k++)
                            s += weights[k] * (matrix[k][j] - means[j]) * (matrix[k][i] - means[i]);
                        cov[i, j] = s * factor;
                        cov[j, i] = s * factor;
                    }
                }
            }
            else if (dimension == 1)
            {
                if (means.Length != rows)
                {
                    throw new DimensionMismatchException("means",
                        "Length of the mean vector should equal the number of rows.");
                }

                if (cols != weights.Length)
                {
                    throw new DimensionMismatchException("weights",
                        "The number of columns and weights must match.");
                }

                cov = new double[rows, rows];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = i; j < rows; j++)
                    {
                        double s = 0.0;
                        for (int k = 0; k < cols; k++)
                            s += weights[k] * (matrix[j][k] - means[j]) * (matrix[i][k] - means[i]);
                        cov[i, j] = s * factor;
                        cov[j, i] = s * factor;
                    }
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension.", "dimension");
            }

            return cov;
        }

        private static double[] GetDecayWeights(int window, double alpha)
        {
            if (alpha == 0)
                return Vector.Ones(window);

            double decay = 1 - alpha;
            double[] decayWeights = new double[window];

            double decayRow = 1;
            for (int i = window - 1; i >= 0; i--)
            {
                decayWeights[i] = decayRow;
                decayRow *= decay;
            }

            return decayWeights;
        }

        private static void Validate(double[][] matrix, int window, double alpha)
        {
            // Perform some basic error validation
            if (matrix == null)
            {
                throw new ArgumentNullException("matrix", "The matrix cannot be null.");
            }

            if (alpha < 0 || alpha > 1)
            {
                string message = string.Format(
                    "Alpha must lie in the interval [0, 1] but was {0}", alpha);

                throw new ArgumentOutOfRangeException("alpha", message);
            }

            int rows = matrix.Rows();

            if (window <= 0 || window > rows)
            {
                string message = string.Format(
                    "Window size ({0}) must be less than or equal to the total number of samples ({1})",
                    window,
                    rows);

                throw new ArgumentOutOfRangeException("window", message);
            }
        }

        private static double correct(bool unbiased, WeightType weightType, double sum, double weightSum, double squareSum)
        {
            if (unbiased)
            {
                if (weightType == WeightType.Automatic)
                {
                    if (weightSum > 1 && weightSum.IsInteger(1e-8))
                        return sum / (weightSum - 1);

                    return sum / (weightSum - (squareSum / weightSum));
                }
                else if (weightType == WeightType.Fraction)
                {
                    /*
                    if (Math.Abs(weightSum - 1.0) >= 1e-8)
                    {
                        throw new ArgumentException("An unbiased variance estimate"
                          + " cannot be computed if weights do not sum to one. The"
                          + " given weights sum up to " + squareSum, "weights");
                    }*/

                    return sum / (weightSum - (squareSum / weightSum));
                }
                else if (weightType == WeightType.Repetition)
                {
                    return sum / (weightSum - (squareSum / weightSum));
                }
            }

            return sum / weightSum;
        }
    }
}

