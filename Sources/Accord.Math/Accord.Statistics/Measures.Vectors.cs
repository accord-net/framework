// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © 2009-2017 César Souza <cesarsouza at gmail.com>
// and other contrinbutors.
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Accord.Math;
    using System.Runtime.CompilerServices;

    /// <summary>
    ///   Set of statistics measures, such as <see cref="Mean(double[])"/>,
    ///   <see cref="Variance(double[])"/> and <see cref="StandardDeviation(double[], bool)"/>.
    /// </summary>
    /// 
    public static partial class Measures
    {

        /// <summary>
        ///   Computes the mean of the given values.
        /// </summary>
        /// 
        /// <param name="values">A double array containing the vector members.</param>
        /// 
        /// <returns>The mean of the given data.</returns>
        /// 
        public static double Mean(this double[] values)
        {
            double sum = 0.0;

            for (int i = 0; i < values.Length; i++)
                sum += values[i];

            return sum / values.Length;
        }

        /// <summary>
        ///   Computes the mean of the given values.
        /// </summary>
        /// 
        /// <param name="values">An integer array containing the vector members.</param>
        /// 
        /// <returns>The mean of the given data.</returns>
        /// 
        public static double Mean(this int[] values)
        {
            double sum = 0.0;

            for (int i = 0; i < values.Length; i++)
                sum += values[i];

            return sum / values.Length;
        }


        /// <summary>
        ///   Computes the Geometric mean of the given values.
        /// </summary>
        /// 
        /// <param name="values">A double array containing the vector members.</param>
        /// 
        /// <returns>The geometric mean of the given data.</returns>
        /// 
        public static double GeometricMean(this double[] values)
        {
            double sum = 1.0;

            for (int i = 0; i < values.Length; i++)
                sum *= values[i];

            return Math.Pow(sum, 1.0 / values.Length);
        }

        /// <summary>
        ///   Computes the log geometric mean of the given values.
        /// </summary>
        /// 
        /// <param name="values">A double array containing the vector members.</param>
        /// 
        /// <returns>The log geometric mean of the given data.</returns>
        /// 
        public static double LogGeometricMean(this double[] values)
        {
            double lnsum = 0;

            for (int i = 0; i < values.Length; i++)
                lnsum += Math.Log(values[i]);

            return lnsum / values.Length;
        }

        /// <summary>
        ///   Computes the geometric mean of the given values.
        /// </summary>
        /// 
        /// <param name="values">A double array containing the vector members.</param>
        /// 
        /// <returns>The geometric mean of the given data.</returns>
        /// 
        public static double GeometricMean(this int[] values)
        {
            double sum = 1.0;

            for (int i = 0; i < values.Length; i++)
                sum *= values[i];

            return Math.Pow(sum, 1.0 / values.Length);
        }

        /// <summary>
        ///   Computes the log geometric mean of the given values.
        /// </summary>
        /// 
        /// <param name="values">A double array containing the vector members.</param>
        /// 
        /// <returns>The log geometric mean of the given data.</returns>
        /// 
        public static double LogGeometricMean(this int[] values)
        {
            double lnsum = 0;

            for (int i = 0; i < values.Length; i++)
                lnsum += Math.Log(values[i]);

            return lnsum / values.Length;
        }

        /// <summary>
        ///   Computes the (weighted) grand mean of a set of samples.
        /// </summary>
        /// 
        /// <param name="means">A double array containing the sample means.</param>
        /// <param name="samples">A integer array containing the sample's sizes.</param>
        /// 
        /// <returns>The grand mean of the samples.</returns>
        /// 
        public static double GrandMean(double[] means, int[] samples)
        {
            double sum = 0;
            int n = 0;

            for (int i = 0; i < means.Length; i++)
            {
                sum += samples[i] * means[i];
                n += samples[i];
            }

            return sum / n;
        }

        /// <summary>
        ///   Computes the mean of the given values.
        /// </summary>
        /// 
        /// <param name="values">A unsigned short array containing the vector members.</param>
        /// 
        /// <returns>The mean of the given data.</returns>
        /// 
        public static double Mean(this ushort[] values)
        {
            double sum = 0.0;

            for (int i = 0; i < values.Length; i++)
                sum += values[i];

            return sum / values.Length;
        }

        /// <summary>
        ///   Computes the mean of the given values.
        /// </summary>
        /// 
        /// <param name="values">A float array containing the vector members.</param>
        /// 
        /// <returns>The mean of the given data.</returns>
        /// 
        public static float Mean(this float[] values)
        {
            float sum = 0;

            for (int i = 0; i < values.Length; i++)
                sum += values[i];

            return sum / values.Length;
        }

        /// <summary>
        ///   Computes the truncated (trimmed) mean of the given values.
        /// </summary>
        /// 
        /// <param name="values">A double array containing the vector members.</param>
        /// <param name="inPlace">Whether to perform operations in place, overwriting the original vector.</param>
        /// <param name="alreadySorted">A boolean parameter informing if the given values have already been sorted.</param>
        /// <param name="percent">The percentage of observations to drop from the sample.</param>
        /// 
        /// <returns>The mean of the given data.</returns>
        /// 
        public static double TruncatedMean(this double[] values, double percent, bool inPlace = false, bool alreadySorted = false)
        {
            if (!alreadySorted)
            {
                values = (inPlace) ? values : (double[])values.Clone();
                Array.Sort(values);
            }

            int k = (int)Math.Floor(values.Length * percent);

            double sum = 0;
            for (int i = k; i < values.Length - k; i++)
                sum += values[i];

            return sum / (values.Length - 2 * k);
        }

        /// <summary>
        ///   Computes the contraharmonic mean of the given values.
        /// </summary>
        /// 
        /// <param name="values">A unsigned short array containing the vector members.</param>
        /// <param name="order">The order of the harmonic mean. Default is 1.</param>
        /// 
        /// <returns>The contraharmonic mean of the given data.</returns>
        /// 
        public static double ContraHarmonicMean(double[] values, int order)
        {
            double r1 = 0, r2 = 0;
            for (int i = 0; i < values.Length; i++)
            {
                r1 += Math.Pow(values[i], order + 1);
                r2 += Math.Pow(values[i], order);
            }

            return r1 / r2;
        }

        /// <summary>
        ///   Computes the contraharmonic mean of the given values.
        /// </summary>
        /// 
        /// <param name="values">A unsigned short array containing the vector members.</param>
        /// 
        /// <returns>The contraharmonic mean of the given data.</returns>
        /// 
        public static double ContraHarmonicMean(double[] values)
        {
            double r1 = 0, r2 = 0;
            for (int i = 0; i < values.Length; i++)
            {
                r1 += values[i] * values[i];
                r2 += values[i];
            }

            return r1 / r2;
        }

        /// <summary>
        ///   Computes the Standard Deviation of the given values.
        /// </summary>
        /// 
        /// <param name="values">A double array containing the vector members.</param>
        /// <param name="unbiased">
        ///   Pass true to compute the standard deviation using the sample variance.
        ///   Pass false to compute it using the population variance. See remarks
        ///   for more details.</param>
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
        /// <returns>The standard deviation of the given data.</returns>
        /// 
        public static double StandardDeviation(this double[] values, bool unbiased = true)
        {
            return StandardDeviation(values, Mean(values), unbiased);
        }

        /// <summary>
        ///   Computes the Standard Deviation of the given values.
        /// </summary>
        /// 
        /// <param name="values">A double array containing the vector members.</param>
        /// 
        /// <returns>The standard deviation of the given data.</returns>
        /// 
        public static double StandardDeviation(this float[] values)
        {
            return StandardDeviation(values, Mean(values));
        }

        /// <summary>
        ///   Computes the Standard Deviation of the given values.
        /// </summary>
        /// 
        /// <param name="values">A double array containing the vector members.</param>
        /// <param name="mean">The mean of the vector, if already known.</param>
        /// <param name="unbiased">
        ///   Pass true to compute the standard deviation using the sample variance.
        ///   Pass false to compute it using the population variance. See remarks
        ///   for more details.</param>
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
        /// <returns>The standard deviation of the given data.</returns>
        /// 
        public static double StandardDeviation(this double[] values, double mean, bool unbiased = true)
        {
            return System.Math.Sqrt(Variance(values, mean, unbiased));
        }

        /// <summary>
        ///   Computes the Standard Deviation of the given values.
        /// </summary>
        /// <param name="values">A float array containing the vector members.</param>
        /// <param name="mean">The mean of the vector, if already known.</param>
        /// <returns>The standard deviation of the given data.</returns>
        public static float StandardDeviation(this float[] values, float mean)
        {
            return (float)System.Math.Sqrt(Variance(values, mean));
        }

        /// <summary>
        ///   Computes the Standard Deviation of the given values.
        /// </summary>
        /// <param name="values">An integer array containing the vector members.</param>
        /// <param name="mean">The mean of the vector, if already known.</param>
        /// <returns>The standard deviation of the given data.</returns>
        public static double StandardDeviation(this int[] values, double mean)
        {
            return System.Math.Sqrt(Variance(values, mean));
        }

        /// <summary>
        ///   Computes the Standard Error for a sample size, which estimates the
        ///   standard deviation of the sample mean based on the population mean.
        /// </summary>
        /// <param name="samples">The sample size.</param>
        /// <param name="standardDeviation">The sample standard deviation.</param>
        /// <returns>The standard error for the sample.</returns>
        public static double StandardError(int samples, double standardDeviation)
        {
            return standardDeviation / System.Math.Sqrt(samples);
        }

        /// <summary>
        ///   Computes the Standard Error for a sample size, which estimates the
        ///   standard deviation of the sample mean based on the population mean.
        /// </summary>
        /// <param name="values">A double array containing the samples.</param>
        /// <returns>The standard error for the sample.</returns>
        public static double StandardError(double[] values)
        {
            return StandardError(values.Length, StandardDeviation(values));
        }


        /// <summary>
        ///   Computes the Variance of the given values.
        /// </summary>
        /// 
        /// <param name="values">A double precision number array containing the vector members.</param>
        /// <returns>The variance of the given data.</returns>
        /// 
        public static double Variance(this double[] values)
        {
            return Variance(values, Mean(values));
        }

        /// <summary>
        ///   Computes the Variance of the given values.
        /// </summary>
        /// 
        /// <param name="values">A double precision number array containing the vector members.</param>
        /// <param name="unbiased">
        ///   Pass true to compute the sample variance; or pass false to compute 
        ///   the population variance. See remarks for more details.</param>
        /// <remarks>
        ///   <para>
        ///     Setting <paramref name="unbiased"/> to <c>true</c> will make this method 
        ///     compute the variance σ² using the sample variance, which is an unbiased 
        ///     estimator of the true population variance. Setting this parameter to true 
        ///     will thus compute σ² using the following formula:</para>
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
        /// <returns>The variance of the given data.</returns>
        /// 
        public static double Variance(this double[] values, bool unbiased)
        {
            return Variance(values, Mean(values), unbiased);
        }

        /// <summary>
        ///   Computes the Variance of the given values.
        /// </summary>
        /// 
        /// <param name="values">An integer number array containing the vector members.</param>
        /// 
        /// <returns>The variance of the given data.</returns>
        /// 
        public static double Variance(this int[] values)
        {
            return Variance(values, Mean(values));
        }

        /// <summary>
        ///   Computes the Variance of the given values.
        /// </summary>
        /// 
        /// <param name="values">An integer number array containing the vector members.</param>
        /// 
        /// <param name="unbiased">
        ///   Pass true to compute the sample variance; or pass false to compute 
        ///   the population variance. See remarks for more details.</param>
        /// <remarks>
        ///   <para>
        ///     Setting <paramref name="unbiased"/> to <c>true</c> will make this method 
        ///     compute the variance σ² using the sample variance, which is an unbiased 
        ///     estimator of the true population variance. Setting this parameter to true 
        ///     will thus compute σ² using the following formula:</para>
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
        /// <returns>The variance of the given data.</returns>
        /// 
        public static double Variance(this int[] values, bool unbiased)
        {
            return Variance(values, Mean(values), unbiased);
        }

        /// <summary>
        ///   Computes the Variance of the given values.
        /// </summary>
        /// <param name="values">A single precision number array containing the vector members.</param>
        /// <returns>The variance of the given data.</returns>
        /// 
        public static double Variance(this float[] values)
        {
            return Variance(values, Mean(values));
        }

        /// <summary>
        ///   Computes the Variance of the given values.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector members.</param>
        /// <param name="mean">The mean of the array, if already known.</param>
        ///   
        /// <returns>The variance of the given data.</returns>
        /// 
        public static double Variance(this double[] values, double mean)
        {
            return Variance(values, mean, true);
        }

        /// <summary>
        ///   Computes the Variance of the given values.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector members.</param>
        /// <param name="mean">The mean of the array, if already known.</param>
        /// <param name="unbiased">
        ///   Pass true to compute the sample variance; or pass false to compute 
        ///   the population variance. See remarks for more details.</param>
        /// <remarks>
        ///   <para>
        ///     Setting <paramref name="unbiased"/> to <c>true</c> will make this method 
        ///     compute the variance σ² using the sample variance, which is an unbiased 
        ///     estimator of the true population variance. Setting this parameter to true 
        ///     will thus compute σ² using the following formula:</para>
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
        ///   
        /// <returns>The variance of the given data.</returns>
        /// 
        public static double Variance(this double[] values, double mean, bool unbiased = true)
        {
            double variance = 0.0;

            for (int i = 0; i < values.Length; i++)
            {
                double x = values[i] - mean;
                variance += x * x;
            }

            if (unbiased)
            {
                // Sample variance
                return variance / (values.Length - 1);
            }
            else
            {
                // Population variance
                return variance / values.Length;
            }
        }

        /// <summary>
        ///   Computes the Variance of the given values.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector members.</param>
        /// <param name="mean">The mean of the array, if already known.</param>
        /// <param name="unbiased">
        ///   Pass true to compute the sample variance; or pass false to compute 
        ///   the population variance. See remarks for more details.</param>
        /// <remarks>
        ///   <para>
        ///     Setting <paramref name="unbiased"/> to <c>true</c> will make this method 
        ///     compute the variance σ² using the sample variance, which is an unbiased 
        ///     estimator of the true population variance. Setting this parameter to true 
        ///     will thus compute σ² using the following formula:</para>
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
        ///   
        /// <returns>The variance of the given data.</returns>
        /// 
        public static double Variance(this int[] values, double mean, bool unbiased = true)
        {
            double variance = 0.0;

            for (int i = 0; i < values.Length; i++)
            {
                double x = values[i] - mean;
                variance += x * x;
            }

            if (unbiased)
            {
                // Sample variance
                return variance / (values.Length - 1);
            }
            else
            {
                // Population variance
                return variance / values.Length;
            }
        }

        /// <summary>
        ///   Computes the Variance of the given values.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector members.</param>
        /// <param name="mean">The mean of the array, if already known.</param>
        /// <returns>The variance of the given data.</returns>
        /// 
        public static float Variance(this float[] values, float mean)
        {
            float variance = 0;

            for (int i = 0; i < values.Length; i++)
            {
                float x = values[i] - mean;
                variance += x * x;
            }

            // Sample variance
            return variance / (values.Length - 1);
        }

        /// <summary>
        ///   Computes the pooled standard deviation of the given values.
        /// </summary>
        /// 
        /// <param name="samples">The grouped samples.</param>
        /// <param name="unbiased">
        ///   True to compute a pooled standard deviation using unbiased estimates
        ///   of the population variance; false otherwise. Default is true.</param>
        /// 
        public static double PooledStandardDeviation(bool unbiased, params double[][] samples)
        {
            return Math.Sqrt(PooledVariance(unbiased, samples));
        }

        /// <summary>
        ///   Computes the pooled standard deviation of the given values.
        /// </summary>
        /// 
        /// <param name="samples">The grouped samples.</param>
        /// 
        public static double PooledStandardDeviation(params double[][] samples)
        {
            return Math.Sqrt(PooledVariance(true, samples));
        }

        /// <summary>
        ///   Computes the pooled standard deviation of the given values.
        /// </summary>
        /// 
        /// <param name="sizes">The number of samples used to compute the <paramref name="variances"/>.</param>
        /// <param name="variances">The unbiased variances for the samples.</param>
        /// <param name="unbiased">
        ///   True to compute a pooled standard deviation using unbiased estimates
        ///   of the population variance; false otherwise. Default is true.</param>
        /// 
        public static double PooledStandardDeviation(int[] sizes, double[] variances, bool unbiased = true)
        {
            return Math.Sqrt(PooledVariance(sizes, variances, unbiased));
        }

        /// <summary>
        ///   Computes the pooled variance of the given values.
        /// </summary>
        /// 
        /// <param name="samples">The grouped samples.</param>
        /// 
        public static double PooledVariance(params double[][] samples)
        {
            return PooledVariance(true, samples);
        }

        /// <summary>
        ///   Computes the pooled variance of the given values.
        /// </summary>
        /// 
        /// <param name="unbiased">
        ///   True to obtain an unbiased estimate of the population
        ///   variance; false otherwise. Default is true.</param>
        /// 
        /// <param name="samples">The grouped samples.</param>
        /// 
        public static double PooledVariance(bool unbiased, params double[][] samples)
        {
            if (samples == null)
                throw new ArgumentNullException("samples");

            double sum = 0;
            int length = 0;

            for (int i = 0; i < samples.Length; i++)
            {
                double[] values = samples[i];
                double var = Variance(values);

                sum += (values.Length - 1) * var;

                if (unbiased)
                {
                    length += values.Length - 1;
                }
                else
                {
                    length += values.Length;
                }
            }

            return sum / length;
        }

        /// <summary>
        ///   Computes the pooled variance of the given values.
        /// </summary>
        /// 
        /// <param name="sizes">The number of samples used to compute the <paramref name="variances"/>.</param>
        /// <param name="variances">The unbiased variances for the samples.</param>
        /// <param name="unbiased">
        ///   True to obtain an unbiased estimate of the population
        ///   variance; false otherwise. Default is true.</param>
        /// 
        public static double PooledVariance(int[] sizes, double[] variances, bool unbiased = true)
        {
            if (sizes == null)
                throw new ArgumentNullException("sizes");

            if (variances == null)
                throw new ArgumentNullException("variances");

            if (sizes.Length != variances.Length)
                throw new DimensionMismatchException("variances");

            double sum = 0;
            int length = 0;

            for (int i = 0; i < variances.Length; i++)
            {
                double var = variances[i];

                sum += (sizes[i] - 1) * var;

                if (unbiased)
                {
                    length += sizes[i] - 1;
                }
                else
                {
                    length += sizes[i];
                }
            }

            return sum / (double)length;
        }

        /// <summary>
        ///   Computes the Mode of the given values.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector values.</param>
        /// 
        /// <returns>The most common value in the given data.</returns>
        /// 
        public static T Mode<T>(this T[] values)
        {
            int bestCount;
            return Mode<T>(values, out bestCount, inPlace: false, alreadySorted: false);
        }

        /// <summary>
        ///   Computes the Mode of the given values.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector values.</param>
        /// <param name="count">Returns how many times the detected mode happens in the values.</param>
        /// 
        /// <returns>The most common value in the given data.</returns>
        /// 
        public static T Mode<T>(this T[] values, out int count)
        {
            return Mode<T>(values, out count, inPlace: false, alreadySorted: false);
        }

        /// <summary>
        ///   Computes the Mode of the given values.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector values.</param>
        /// <param name="inPlace">True to perform the operation in place, altering the original input vector.</param>
        /// <param name="alreadySorted">Pass true if the list of values is already sorted.</param>
        /// 
        /// <returns>The most common value in the given data.</returns>
        /// 
        public static T Mode<T>(this T[] values,
            bool inPlace, bool alreadySorted = false)
        {
            int count;
            return Mode<T>(values, out count, inPlace: inPlace, alreadySorted: alreadySorted);
        }

        /// <summary>
        ///   Computes the Mode of the given values.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector values.</param>
        /// <param name="inPlace">True to perform the operation in place, altering the original input vector.</param>
        /// <param name="alreadySorted">Pass true if the list of values is already sorted.</param>
        /// <param name="count">Returns how many times the detected mode happens in the values.</param>
        /// 
        /// <returns>The most common value in the given data.</returns>
        /// 
        public static T Mode<T>(this T[] values, out int count,
            bool inPlace, bool alreadySorted = false)
        {
            if (values.Length == 0)
                throw new ArgumentException("The values vector cannot be empty.", "values");

            if (values[0] is IComparable)
                return mode_sort<T>(values, inPlace, alreadySorted, out count);

            return mode_bag<T>(values, out count);
        }

        private static T mode_bag<T>(T[] values, out int bestCount)
        {
            var bestValue = values[0];
            bestCount = 1;

            var set = new Dictionary<T, int>();

            foreach (var v in values)
            {
                int count;
                if (!set.TryGetValue(v, out count))
                    count = 1;
                else
                    count = count + 1;

                set[v] = count;

                if (count > bestCount)
                {
                    bestCount = count;
                    bestValue = v;
                }
            }

            return bestValue;
        }

        private static T mode_sort<T>(T[] values, bool inPlace, bool alreadySorted, out int bestCount)
        {
            if (!alreadySorted)
            {
                if (!inPlace)
                    values = (T[])values.Clone();
                Array.Sort(values);
            }

            var currentValue = values[0];
            var currentCount = 1;

            var bestValue = currentValue;
            bestCount = currentCount;


            for (int i = 1; i < values.Length; i++)
            {
                if (currentValue.Equals(values[i]))
                {
                    currentCount += 1;
                }
                else
                {
                    currentValue = values[i];
                    currentCount = 1;
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
        ///   Computes the Covariance between two arrays of values.
        /// </summary>
        /// 
        /// <param name="vector1">A number array containing the first vector elements.</param>
        /// <param name="vector2">A number array containing the second vector elements.</param>
        /// <param name="unbiased">
        ///   Pass true to compute the sample variance; or pass false to compute 
        ///   the population variance. See remarks for more details.</param>
        /// <remarks>
        ///   <para>
        ///     Setting <paramref name="unbiased"/> to <c>true</c> will make this method 
        ///     compute the variance σ² using the sample variance, which is an unbiased 
        ///     estimator of the true population variance. Setting this parameter to true 
        ///     will thus compute σ² using the following formula:</para>
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
        /// <returns>The variance of the given data.</returns>
        /// 
        public static double Covariance(this double[] vector1, double[] vector2, bool unbiased = true)
        {
            return Covariance(vector1, Mean(vector1), vector2, Mean(vector2), unbiased);
        }

        /// <summary>
        ///   Computes the Covariance between two arrays of values.
        /// </summary>
        /// 
        /// <param name="vector1">A number array containing the first vector elements.</param>
        /// <param name="vector2">A number array containing the second vector elements.</param>
        /// <param name="mean1">The mean value of <paramref name="vector1"/>, if known.</param>
        /// <param name="mean2">The mean value of <paramref name="vector2"/>, if known.</param>
        /// <param name="unbiased">
        ///   Pass true to compute the sample variance; or pass false to compute 
        ///   the population variance. See remarks for more details.</param>
        /// <remarks>
        ///   <para>
        ///     Setting <paramref name="unbiased"/> to <c>true</c> will make this method 
        ///     compute the variance σ² using the sample variance, which is an unbiased 
        ///     estimator of the true population variance. Setting this parameter to true 
        ///     will thus compute σ² using the following formula:</para>
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
        /// <returns>The variance of the given data.</returns>
        /// 
        public static double Covariance(this double[] vector1, double mean1, double[] vector2, double mean2, bool unbiased = true)
        {
            if (vector1 == null)
                throw new ArgumentNullException("vector1");

            if (vector2 == null)
                throw new ArgumentNullException("vector2");

            if (vector1.Length != vector2.Length)
                throw new DimensionMismatchException("vector2");

            double covariance = 0.0;

            for (int i = 0; i < vector1.Length; i++)
            {
                double x = vector1[i] - mean1;
                double y = vector2[i] - mean2;
                covariance += x * y;
            }

            if (unbiased)
            {
                // Sample variance
                return covariance / (vector1.Length - 1);
            }
            else
            {
                // Population variance
                return covariance / vector1.Length;
            }
        }

        /// <summary>
        ///   Computes the Skewness for the given values.
        /// </summary>
        /// 
        /// <remarks>
        ///   Skewness characterizes the degree of asymmetry of a distribution
        ///   around its mean. Positive skewness indicates a distribution with
        ///   an asymmetric tail extending towards more positive values. Negative
        ///   skewness indicates a distribution with an asymmetric tail extending
        ///   towards more negative values.
        /// </remarks>
        /// 
        /// <param name="values">A number array containing the vector values.</param>
        /// 
        /// <param name="unbiased">
        ///   True to compute the unbiased estimate of the population
        ///   skewness, false otherwise. Default is true (compute the 
        ///   unbiased estimator).</param>
        ///   
        /// <returns>The skewness of the given data.</returns>
        /// 
        public static double Skewness(this double[] values, bool unbiased = true)
        {
            double mean = Mean(values);
            return Skewness(values, mean, unbiased);
        }

        /// <summary>
        ///   Computes the Skewness for the given values.
        /// </summary>
        /// 
        /// <remarks>
        ///   Skewness characterizes the degree of asymmetry of a distribution
        ///   around its mean. Positive skewness indicates a distribution with
        ///   an asymmetric tail extending towards more positive values. Negative
        ///   skewness indicates a distribution with an asymmetric tail extending
        ///   towards more negative values.
        /// </remarks>
        /// 
        /// <param name="values">A number array containing the vector values.</param>
        /// <param name="mean">The values' mean, if already known.</param>
        /// <param name="unbiased">
        ///   True to compute the unbiased estimate of the population
        ///   skewness, false otherwise. Default is true (compute the 
        ///   unbiased estimator).</param>
        /// 
        /// <returns>The skewness of the given data.</returns>
        /// 
        public static double Skewness(this double[] values, double mean, bool unbiased = true)
        {
            double n = values.Length;

            double s2 = 0;
            double s3 = 0;

            for (int i = 0; i < values.Length; i++)
            {
                double dev = values[i] - mean;

                s2 += dev * dev;
                s3 += dev * dev * dev;
            }

            double m2 = s2 / n;
            double m3 = s3 / n;

            double g = m3 / (Math.Pow(m2, 3 / 2.0));

            if (unbiased)
            {
                double a = Math.Sqrt(n * (n - 1));
                double b = n - 2;
                return (a / b) * g;
            }

            return g;
        }

        /// <summary>
        ///   Computes the Kurtosis for the given values.
        /// </summary>
        /// 
        /// <remarks>
        ///   The framework uses the same definition used by default in SAS and SPSS.
        /// </remarks>
        ///
        /// <param name="values">A number array containing the vector values.</param>
        /// <param name="unbiased">
        ///   True to compute the unbiased estimate of the population
        ///   kurtosis, false otherwise. Default is true (compute the 
        ///   unbiased estimator).</param>
        /// 
        /// <returns>The kurtosis of the given data.</returns>
        /// 
        public static double Kurtosis(this double[] values, bool unbiased = true)
        {
            double mean = Mean(values);
            return Kurtosis(values, mean, unbiased);
        }

        /// <summary>
        ///   Computes the Kurtosis for the given values.
        /// </summary>
        /// 
        /// <remarks>
        ///   The framework uses the same definition used by default in SAS and SPSS.
        /// </remarks>
        ///
        /// <param name="values">A number array containing the vector values.</param>
        /// <param name="mean">The values' mean, if already known.</param>
        /// <param name="unbiased">
        ///   True to compute the unbiased estimate of the population
        ///   kurtosis, false otherwise. Default is true (compute the 
        ///   unbiased estimator).</param>
        /// 
        /// <returns>The kurtosis of the given data.</returns>
        /// 
        public static double Kurtosis(this double[] values, double mean, bool unbiased = true)
        {
            // http://www.ats.ucla.edu/stat/mult_pkg/faq/general/kurtosis.htm

            double n = values.Length;

            double s2 = 0;
            double s4 = 0;

            for (int i = 0; i < values.Length; i++)
            {
                double dev = values[i] - mean;

                s2 += dev * dev;
                s4 += dev * dev * dev * dev;
            }

            double m2 = s2 / n;
            double m4 = s4 / n;

            if (unbiased)
            {
                double v = s2 / (n - 1);

                double a = (n * (n + 1)) / ((n - 1) * (n - 2) * (n - 3));
                double b = s4 / (v * v);
                double c = ((n - 1) * (n - 1)) / ((n - 2) * (n - 3));

                return a * b - 3 * c;
            }
            else
            {
                return m4 / (m2 * m2) - 3;
            }
        }

        /// <summary>
        ///   Computes the entropy function for a set of numerical values in a 
        ///   given Probability Density Function (pdf).
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector values.</param>
        /// <param name="pdf">A probability distribution function.</param>
        /// <param name="weights">The importance for each sample.</param>
        /// 
        /// <returns>The distribution's entropy for the given values.</returns>
        /// 
        public static double Entropy(this double[] values, double[] weights, Func<double, double> pdf)
        {
            double sum = 0;
            for (int i = 0; i < values.Length; i++)
            {
                double p = pdf(values[i]) * weights[i];
                sum += p * Math.Log(p);
            }

            return sum;
        }

        /// <summary>
        ///   Computes the entropy function for a set of numerical values in a 
        ///   given Probability Density Function (pdf).
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector values.</param>
        /// <param name="pdf">A probability distribution function.</param>
        /// 
        /// <returns>The distribution's entropy for the given values.</returns>
        /// 
        public static double Entropy(this double[] values, Func<double, double> pdf)
        {
            double sum = 0;
            for (int i = 0; i < values.Length; i++)
            {
                double p = pdf(values[i]);
                sum += p * Math.Log(p);
            }

            return sum;
        }

        /// <summary>
        ///   Computes the entropy function between an expected value
        ///   and a predicted value between 0 and 1.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Entropy(bool expected, double predicted)
        {
            if (!expected)
                return 0;
            return Math.Log(predicted);
        }

        /// <summary>
        ///   Computes the entropy function between an expected value
        ///   and a predicted value.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Entropy(bool expected, bool predicted)
        {
            if (!expected)
                return 0;
            return predicted ? 0 : Double.NegativeInfinity;
        }

        /// <summary>
        ///   Computes the entropy function for a set of numerical values in a 
        ///   given Probability Density Function (pdf).
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector values.</param>
        /// <param name="pdf">A probability distribution function.</param>
        /// <param name="weights">The importance for each sample.</param>
        /// 
        /// <returns>The distribution's entropy for the given values.</returns>
        /// 
        public static double WeightedEntropy(this double[] values, double[] weights, Func<double, double> pdf)
        {
            double sum = 0;
            for (int i = 0; i < values.Length; i++)
            {
                double p = pdf(values[i]) * weights[i];
                sum += p * Math.Log(p);
            }

            return sum;
        }

        /// <summary>
        ///   Computes the entropy function for a set of numerical values in a 
        ///   given Probability Density Function (pdf).
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector values.</param>
        /// <param name="pdf">A probability distribution function.</param>
        /// <param name="weights">The repetition counts for each sample.</param>
        /// 
        /// <returns>The distribution's entropy for the given values.</returns>
        /// 
        public static double WeightedEntropy(this double[] values, int[] weights, Func<double, double> pdf)
        {
            double sum = 0;
            for (int i = 0; i < values.Length; i++)
            {
                double p = pdf(values[i]);
                sum += p * Math.Log(p) * weights[i];
            }

            return sum;
        }



        /// <summary>
        ///   Computes the entropy for the given values.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector values.</param>
        /// 
        /// <returns>The calculated entropy for the given values.</returns>
        /// 
        public static double Entropy(this double[] values)
        {
            double sum = 0;
            foreach (double v in values)
                sum += v * Math.Log(v);
            return -sum;
        }

        /// <summary>
        ///   Computes the entropy for the given values.
        /// </summary>
        /// 
        /// <param name="values">A number array containing the vector values.</param>
        /// <param name="eps">A small constant to avoid <see cref="Double.NaN"/>s in
        ///   case the there is a zero between the given <paramref name="values"/>.</param>
        /// 
        /// <returns>The calculated entropy for the given values.</returns>
        /// 
        public static double Entropy(this double[] values, double eps = 0)
        {
            double sum = 0;
            foreach (double v in values)
                sum += v * Math.Log(v + eps);
            return -sum;
        }

        /// <summary>
        ///   Computes the entropy for the given values.
        /// </summary>
        /// 
        /// <param name="values">A number matrix containing the matrix values.</param>
        /// <param name="eps">A small constant to avoid <see cref="Double.NaN"/>s in
        ///   case the there is a zero between the given <paramref name="values"/>.</param>
        /// 
        /// <returns>The calculated entropy for the given values.</returns>
        /// 
        public static double Entropy(this double[,] values, double eps = 0)
        {
            double sum = 0;
            foreach (double v in values)
                sum += v * Math.Log(v + eps);
            return -sum;
        }

        /// <summary>
        ///   Computes the entropy for the given values.
        /// </summary>
        /// 
        /// <param name="values">A number matrix containing the matrix values.</param>
        /// 
        /// <returns>The calculated entropy for the given values.</returns>
        /// 
        public static double Entropy(this double[,] values)
        {
            double sum = 0;
            foreach (double v in values)
                sum += v * Math.Log(v);
            return -sum;
        }

        /// <summary>
        ///   Computes the entropy for the given values.
        /// </summary>
        /// 
        /// <param name="values">An array of integer symbols.</param>
        /// <param name="startValue">The starting symbol.</param>
        /// <param name="endValue">The ending symbol.</param>
        /// <returns>The evaluated entropy.</returns>
        /// 
        public static double Entropy(int[] values, int startValue, int endValue)
        {
            double entropy = 0;

            // For each class
            for (int c = startValue; c <= endValue; c++)
            {
                int count = 0;

                // Count the number of instances inside
                for (int i = 0; i < values.Length; i++)
                    if (values[i] == c)
                        count++;

                if (count > 0)
                {
                    // Avoid situations limiting situations
                    //  by forcing 0 * Math.Log(0) to be 0.

                    double p = count / (double)values.Length;
                    entropy -= p * Math.Log(p, 2);
                }
            }

            return entropy;
        }

        /// <summary>
        ///   Computes the entropy for the given values.
        /// </summary>
        /// 
        /// <param name="values">An array of integer symbols.</param>
        /// <param name="startValue">The starting symbol.</param>
        /// <param name="endValue">The ending symbol.</param>
        /// <param name="weights">The importance for each sample.</param>
        /// 
        /// <returns>The evaluated entropy.</returns>
        /// 
        public static double WeightedEntropy(int[] values, double[] weights, int startValue, int endValue)
        {
            double entropy = 0;

            // For each class
            for (int c = startValue; c <= endValue; c++)
            {
                double count = 0;

                // Count the number of instances inside
                for (int i = 0; i < values.Length; i++)
                    if (values[i] == c)
                        count += weights[i];

                if (count > 0)
                {
                    // Avoid situations limiting situations
                    //  by forcing 0 * Math.Log(0) to be 0.

                    double p = count / (double)values.Length;
                    entropy -= p * Math.Log(p, 2);
                }
            }

            return entropy;
        }

        /// <summary>
        ///   Computes the entropy for the given values.
        /// </summary>
        /// 
        /// <param name="values">An array of integer symbols.</param>
        /// <param name="startValue">The starting symbol.</param>
        /// <param name="endValue">The ending symbol.</param>
        /// <param name="weights">The importance for each sample.</param>
        /// 
        /// <returns>The evaluated entropy.</returns>
        /// 
        public static double WeightedEntropy(double[] values, double[] weights, int startValue, int endValue)
        {
            double entropy = 0;
            double totalWeightSum = weights.Sum();

            // For each class
            for (int c = startValue; c <= endValue; c++)
            {
                double classWeightSum = 0;

                // Count the number of instances inside
                for (int i = 0; i < values.Length; i++)
                    if (values[i] == c)
                        classWeightSum += weights[i];

                if (classWeightSum > 0)
                {
                    // Avoid situations limiting situations
                    //  by forcing 0 * Math.Log(0) to be 0.

                    double p = classWeightSum / totalWeightSum;
                    entropy -= p * Math.Log(p, 2);
                }
            }

            return entropy;
        }

        /// <summary>
        ///   Computes the entropy for the given values.
        /// </summary>
        /// 
        /// <param name="values">An array of integer symbols.</param>
        /// <param name="startValue">The starting symbol.</param>
        /// <param name="endValue">The ending symbol.</param>
        /// <returns>The evaluated entropy.</returns>
        /// 
        public static double Entropy(IList<int> values, int startValue, int endValue)
        {
            double entropy = 0;
            double total = values.Count;

            // For each class
            for (int c = startValue; c <= endValue; c++)
            {
                int count = 0;

                // Count the number of instances inside
                foreach (int v in values)
                    if (v == c) count++;

                if (count > 0)
                {
                    // Avoid situations limiting situations
                    //  by forcing 0 * Math.Log(0) to be 0.

                    double p = count / total;
                    entropy -= p * Math.Log(p, 2);
                }
            }

            return entropy;
        }

        /// <summary>
        ///   Computes the entropy for the given values.
        /// </summary>
        /// 
        /// <param name="values">An array of integer symbols.</param>
        /// <param name="startValue">The starting symbol.</param>
        /// <param name="endValue">The ending symbol.</param>
        /// <param name="weights">The importance for each sample.</param>
        /// 
        /// <returns>The evaluated entropy.</returns>
        /// 
        public static double WeightedEntropy(IList<int> values, IList<double> weights, int startValue, int endValue)
        {
            double entropy = 0;
            double totalWeightSum = weights.Sum();

            // For each class
            for (int c = startValue; c <= endValue; c++)
            {
                double classWeightSum = 0;

                // Count the number of instances inside
                for (int i = 0; i < values.Count; i++)
                    if (values[i] == c)
                        classWeightSum += weights[i];

                if (classWeightSum > 0)
                {
                    // Avoid situations limiting situations
                    //  by forcing 0 * Math.Log(0) to be 0.

                    double p = classWeightSum / totalWeightSum;
                    entropy -= p * Math.Log(p, 2);
                }
            }

            return entropy;
        }

        /// <summary>
        ///   Computes the entropy for the given values.
        /// </summary>
        /// 
        /// <param name="values">An array of integer symbols.</param>
        /// <param name="valueRange">The range of symbols.</param>
        /// <returns>The evaluated entropy.</returns>
        /// 
        public static double Entropy(int[] values, IntRange valueRange)
        {
            return Entropy(values, valueRange.Min, valueRange.Max);
        }

        /// <summary>
        ///   Computes the entropy for the given values.
        /// </summary>
        /// 
        /// <param name="values">An array of integer symbols.</param>
        /// <param name="classes">The number of distinct classes.</param>
        /// <param name="weights">The importance for each sample.</param>
        /// 
        /// <returns>The evaluated entropy.</returns>
        /// 
        public static double WeightedEntropy(int[] values, double[] weights, int classes)
        {
            return WeightedEntropy(values, weights, 0, classes - 1);
        }

        /// <summary>
        ///   Computes the entropy for the given values.
        /// </summary>
        /// 
        /// <param name="values">An array of integer symbols.</param>
        /// <param name="classes">The number of distinct classes.</param>
        /// <param name="weights">The importance for each sample.</param>
        /// 
        /// <returns>The evaluated entropy.</returns>
        /// 
        public static double WeightedEntropy(double[] values, double[] weights, int classes)
        {
            return WeightedEntropy(values, weights, 0, classes - 1);
        }

        /// <summary>
        ///   Computes the entropy for the given values.
        /// </summary>
        /// 
        /// <param name="values">An array of integer symbols.</param>
        /// <param name="classes">The number of distinct classes.</param>
        /// <returns>The evaluated entropy.</returns>
        /// 
        public static double Entropy(int[] values, int classes)
        {
            return Entropy(values, 0, classes - 1);
        }

        /// <summary>
        ///   Computes the entropy for the given values.
        /// </summary>
        /// 
        /// <param name="values">An array of integer symbols.</param>
        /// <param name="classes">The number of distinct classes.</param>
        /// <returns>The evaluated entropy.</returns>
        /// 
        public static double Entropy(IList<int> values, int classes)
        {
            return Entropy(values, 0, classes - 1);
        }

        /// <summary>
        ///   Computes the entropy for the given values.
        /// </summary>
        /// 
        /// <param name="values">An array of integer symbols.</param>
        /// <param name="classes">The number of distinct classes.</param>
        /// <param name="weights">The importance for each sample.</param>
        /// 
        /// <returns>The evaluated entropy.</returns>
        /// 
        public static double WeightedEntropy(IList<int> values, IList<double> weights, int classes)
        {
            return WeightedEntropy(values, weights, 0, classes - 1);
        }

    }
}

