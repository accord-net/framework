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
    using Accord.Math;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Compat;

    /// <summary>
    ///   Shapiro-Wilk test for normality.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Shapiro–Wilk test is a test of normality in frequentist statistics. It was 
    ///   published in 1965 by Samuel Sanford Shapiro and Martin Wilk. The The Shapiro–Wilk 
    ///   test tests the null hypothesis that a sample came from a normally distributed 
    ///   population.</para>
    ///   
    /// <para>
    ///   The null-hypothesis of this test is that the population is normally distributed. Thus,
    ///   if the <see cref="HypothesisTest{T}.PValue">p-value</see> is less than the chosen 
    ///   <see cref="HypothesisTest{T}.Size">alpha level</see>, then the null hypothesis is rejected
    ///   and there is evidence that the data tested are not from a normally distributed population; 
    ///   in other words, the data are not normal. On the contrary, if the p-value is greater than
    ///   the chosen alpha level, then the null hypothesis that the data came from a normally 
    ///   distributed population cannot be rejected (e.g., for an alpha level of 0.05, a data 
    ///   set with a p-value of 0.02 rejects the null hypothesis that the data are from a normally
    ///   distributed population). However, since the test is biased by sample size, the 
    ///   test may be statistically significant from a normal distribution in any large samples. 
    ///   Thus a Q–Q plot is required for verification in addition to the test.</para>
    /// 
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Shapiro%E2%80%93Wilk_test">
    ///       Wikipedia, The Free Encyclopedia. Shapiro-Wilk test. Available on:
    ///       http://en.wikipedia.org/wiki/Shapiro%E2%80%93Wilk_test </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    ///   <code source="Unit Tests\Accord.Tests.Statistics\Testing\ShapiroWilkTestTest.cs" region="doc_test"/>
    /// </example>
    /// 
    [Serializable]
    public class ShapiroWilkTest : HypothesisTest<ShapiroWilkDistribution>,
        IHypothesisTest<ShapiroWilkDistribution>
    {

        /// <summary>
        ///   Creates a new Shapiro-Wilk test.
        /// </summary>
        /// 
        /// <param name="sample">The sample we would like to test.</param>
        /// 
        /// <exception cref="System.ArgumentException">
        ///   The sample must contain at least 4 observations.</exception>
        /// 
        public ShapiroWilkTest(double[] sample)
        {
            double N = sample.Length;
            int n = sample.Length;

            if (n < 4)
            {
                throw new ArgumentException("The sample must contain at least 4 observations.", "sample");
            }

            double mean = sample.Mean();

            StatisticDistribution = new ShapiroWilkDistribution(n);

            // Create a copy of the samples to prevent altering the
            // constructor's original arguments in the sorting step 
            double[] Y = (double[])sample.Clone();

            // Sort sample
            Array.Sort(Y);

            double[] m = new double[sample.Length];

            // Compute Bloom scores
            for (int i = 0; i < Y.Length; i++)
            {
                double num = ((i + 1) - 3.0 / 8.0);
                double den = (N + 1.0 / 4.0);
                m[i] = NormalDistribution.Standard.InverseDistributionFunction(num / den);
            }

            double mm = m.Dot(m);

            double[] c = m.Divide(Math.Sqrt(mm));

            double u = 1.0 / Math.Sqrt(N);

            double u2 = u * u;
            double u3 = u2 * u;
            double u4 = u2 * u2;
            double u5 = u4 * u;

            double[] a = new double[n];

            double an = a[n - 1] = -2.706056 * u5 + 4.434685 * u4 - 2.071190 * u3 - 0.147981 * u2 + 0.221157 * u + c[n - 1];
            double mn = m[n - 1];
            a[0] = -an;

            if (N <= 5)
            {
                double phi = (mm - 2 * mn * mn) / (1.0 - 2 * an * an);
                double sqrt = Math.Sqrt(phi);

                for (int i = 1; i < n - 1; i++)
                    a[i] = m[i] / sqrt;
            }
            else
            {
                double anm1 = a[n - 2] = -3.582633 * u5 + 5.682633 * u4 - 1.752461 * u3 - 0.293762 * u2 + 0.042981 * u + c[n - 2];
                double mnm1 = m[n - 2];
                a[1] = -anm1;

                double phi = (mm - 2 * mn * mn - 2 * mnm1 * mnm1) / (1.0 - 2 * an * an - 2 * anm1 * anm1);
                double sqrt = Math.Sqrt(phi);

                for (int i = 2; i < n - 2; i++)
                    a[i] = m[i] / sqrt;
            }

            double Wnum = 0;
            double Wden = 0;

            for (int i = 0; i < sample.Length; i++)
            {
                double w = (Y[i] - mean);
                Wnum += a[i] * Y[i];
                Wden += w * w;
            }

            double W = (Wnum * Wnum) / Wden;

            this.Statistic = W;
            this.PValue = this.StatisticToPValue(W);
        }

        /// <summary>
        ///   Converts a given p-value to a test statistic.
        /// </summary>
        /// 
        /// <param name="p">The p-value.</param>
        /// 
        /// <returns>The test statistic which would generate the given p-value.</returns>
        /// 
        public override double PValueToStatistic(double p)
        {
            return StatisticDistribution.InverseDistributionFunction(p);
        }

        /// <summary>
        ///   Converts a given test statistic to a p-value.
        /// </summary>
        /// 
        /// <param name="x">The value of the test statistic.</param>
        /// 
        /// <returns>The p-value for the given statistic.</returns>
        /// 
        public override double StatisticToPValue(double x)
        {
            return StatisticDistribution.DistributionFunction(x);
        }
    }
}