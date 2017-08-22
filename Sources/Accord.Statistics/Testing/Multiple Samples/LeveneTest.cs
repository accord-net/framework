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

namespace Accord.Statistics.Testing
{
    using System;
    using Accord.Compat;

    /// <summary>
    ///   Levene test computation methods.
    /// </summary>
    /// 
    public enum LeveneTestMethod
    {
        /// <summary>
        ///   The test has been computed using the Mean.
        /// </summary>
        /// 
        Mean,

        /// <summary>
        ///   The test has been computed using the Median
        ///   (which is known as the Brown-Forsythe test).
        /// </summary>
        /// 
        Median,

        /// <summary>
        ///   The test has been computed using the trimmed mean.
        /// </summary>
        /// 
        TruncatedMean
    }

    /// <summary>
    ///   Levene's test for equality of variances.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In statistics, Levene's test is an inferential statistic used to assess the 
    ///   equality of variances for a variable calculated for two or more groups. Some 
    ///   common statistical procedures assume that variances of the populations from 
    ///   which different samples are drawn are equal. Levene's test assesses this 
    ///   assumption. It tests the null hypothesis that the population variances are 
    ///   equal (called homogeneity of variance or homoscedasticity). If the resulting 
    ///   P-value of Levene's test is less than some significance level (typically 0.05), 
    ///   the obtained differences in sample variances are unlikely to have occurred based 
    ///   on random sampling from a population with equal variances. Thus, the null hypothesis
    ///   of equal variances is rejected and it is concluded that there is a difference 
    ///   between the variances in the population.</para>
    ///   
    /// <para>
    ///   Some of the procedures typically assuming homoscedasticity, for which one can use 
    ///   Levene's tests, include <see cref="IAnova">analysis of variance</see> and <see cref="TTest">
    ///   t-tests</see>. Levene's test is often used before a comparison of means. When Levene's
    ///   test shows significance, one should switch to generalized tests, free from homoscedasticity 
    ///   assumptions. Levene's test may also be used as a main test for answering a stand-alone 
    ///   question of whether two sub-samples in a given population have equal or different variances.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia, The Free Encyclopedia. Levene's test. Available on:
    ///       http://en.wikipedia.org/wiki/Levene's_test </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <seealso cref="BartlettTest"/>
    /// 
    /// <seealso cref="FTest"/>
    /// <seealso cref="Accord.Statistics.Distributions.Univariate.FDistribution"/>
    /// 
    [Serializable]
    public class LeveneTest : FTest
    {

        /// <summary>
        ///   Gets the method used to compute the Levene's test.
        /// </summary>
        /// 
        public LeveneTestMethod Method { get; private set; }

        /// <summary>
        ///   Tests the null hypothesis that all group variances are equal.
        /// </summary>
        /// 
        /// <param name="samples">The grouped samples.</param>
        /// <param name="median"><c>True</c> to use the median in the Levene calculation.
        /// <c>False</c> to use the mean. Default is false (use the mean). </param>
        /// 
        public LeveneTest(double[][] samples, bool median = false)
        {
            compute(samples, median ? LeveneTestMethod.Median : LeveneTestMethod.Mean, 0);
        }

        /// <summary>
        ///   Tests the null hypothesis that all group variances are equal.
        /// </summary>
        /// 
        /// <param name="samples">The grouped samples.</param>
        /// <param name="percent">The percentage of observations to discard
        /// from the sample when computing the test with the truncated mean.</param>
        /// 
        public LeveneTest(double[][] samples, double percent)
        {
            compute(samples, LeveneTestMethod.TruncatedMean, percent);
        }


        private void compute(double[][] samples, LeveneTestMethod method, double percent)
        {
            this.Method = method;
            int N = 0, k = samples.Length;

            // Compute group means
            var means = new double[samples.Length];

            switch (method)
            {
                case LeveneTestMethod.Mean:
                    for (int i = 0; i < means.Length; i++)
                        means[i] = samples[i].Mean();
                    break;

                case LeveneTestMethod.Median:
                    for (int i = 0; i < means.Length; i++)
                        means[i] = samples[i].Median();
                    break;

                case LeveneTestMethod.TruncatedMean:
                    for (int i = 0; i < means.Length; i++)
                        means[i] = samples[i].TruncatedMean(percent);
                    break;
            }

            // Compute absolute centered samples
            var z = new double[samples.Length][];
            for (int i = 0; i < z.Length; i++)
            {
                z[i] = new double[samples[i].Length];
                for (int j = 0; j < z[i].Length; j++)
                    z[i][j] = Math.Abs(samples[i][j] - means[i]);
            }

            // Compute means for the centered samples
            var newMeans = new double[samples.Length];
            for (int i = 0; i < newMeans.Length; i++)
                newMeans[i] = z[i].Mean();

            // Compute total mean
            double totalMean = 0;
            for (int i = 0; i < samples.Length; i++)
            {
                for (int j = 0; j < samples[i].Length; j++)
                    totalMean += z[i][j];
                N += samples[i].Length;
            }
            totalMean /= N;

            double sum1 = 0; // Numerator sum
            for (int i = 0; i < samples.Length; i++)
            {
                int n = samples[i].Length;
                double u = (newMeans[i] - totalMean);
                sum1 += n * u * u;
            }

            double sum2 = 0; // Denominator sum
            for (int i = 0; i < samples.Length; i++)
            {
                for (int j = 0; j < samples[i].Length; j++)
                {
                    double u = z[i][j] - newMeans[i];
                    sum2 += u * u;
                }
            }

            double num = (N - k) * sum1;
            double den = (k - 1) * sum2;

            double W = num / den;
            int degree1 = k - 1;
            int degree2 = N - k;

            Compute(W, degree1, degree2, TwoSampleHypothesis.FirstValueIsGreaterThanSecond);
        }
    }
}
