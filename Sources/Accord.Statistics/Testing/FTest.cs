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
    using Accord.Statistics.Distributions.Univariate;
    using System.Diagnostics;
    using Accord.Compat;

    /// <summary>
    ///   Snedecor's F-Test.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   A F-test is any statistical test in which the test statistic has an
    ///   <see cref="FDistribution">F-distribution</see> under the null hypothesis.
    ///   It is most often used when comparing statistical models that have been fit
    ///   to a data set, in order to identify the model that best fits the population
    ///   from which the data were sampled.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/F-test">
    ///        Wikipedia, The Free Encyclopedia. F-Test. Available on:
    ///        http://en.wikipedia.org/wiki/F-test </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <seealso cref="OneWayAnova"/>
    /// <seealso cref="TwoWayAnova"/>
    /// 
    /// <example>
    /// <code>
    /// // The following example has been based on the page "F-Test for Equality 
    /// // of Two Variances", from NIST/SEMATECH e-Handbook of Statistical Methods:
    /// //
    /// //  http://www.itl.nist.gov/div898/handbook/eda/section3/eda359.htm
    /// //
    /// 
    /// // Consider a data set containing 480 ceramic strength 
    /// // measurements for two batches of material. The summary
    /// // statistics for each batch are shown below:
    /// 
    /// // Batch 1:
    /// int numberOfObservations1 = 240;
    /// // double mean1 = 688.9987;
    /// double stdDev1 = 65.54909;
    /// double var1 = stdDev1 * stdDev1;
    /// 
    /// // Batch 2:
    /// int numberOfObservations2 = 240;
    /// // double mean2 = 611.1559;
    /// double stdDev2 = 61.85425;
    /// double var2 = stdDev2 * stdDev2;
    /// 
    /// // Here, we will be testing the null hypothesis that
    /// // the variances for the two batches are equal.
    /// 
    /// int degreesOfFreedom1 = numberOfObservations1 - 1;
    /// int degreesOfFreedom2 = numberOfObservations2 - 1;
    /// 
    /// // Now we can create a F-Test to test the difference between variances
    /// var ftest = new FTest(var1, var2, degreesOfFreedom1, degreesOfFreedom2);
    /// 
    /// double statistic = ftest.Statistic; // 1.123037
    /// double pvalue = ftest.PValue;       // 0.185191
    /// bool significant = ftest.Significant; // false
    /// 
    /// // The F test indicates that there is not enough evidence 
    /// // to reject the null hypothesis that the two batch variances
    /// // are equal at the 0.05 significance level.
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="Accord.Statistics.Distributions.Univariate.FDistribution"/>
    /// 
    [Serializable]
    public class FTest : HypothesisTest<FDistribution>
    {

        /// <summary>
        ///   Gets the alternative hypothesis under test. If the test is
        ///   <see cref="IHypothesisTest.Significant"/>, the null hypothesis can be rejected
        ///   in favor of this alternative hypothesis.
        /// </summary>
        /// 
        public TwoSampleHypothesis Hypothesis { get; private set; }

        /// <summary>
        ///   Gets the degrees of freedom for the
        ///   numerator in the test distribution.
        /// </summary>
        /// 
        public int DegreesOfFreedom1 { get { return StatisticDistribution.DegreesOfFreedom1; } }

        /// <summary>
        ///   Gets the degrees of freedom for the
        ///   denominator in the test distribution.
        /// </summary>
        /// 
        public int DegreesOfFreedom2 { get { return StatisticDistribution.DegreesOfFreedom2; } }

        /// <summary>
        ///   Creates a new F-Test for a given statistic with given degrees of freedom.
        /// </summary>
        /// 
        /// <param name="var1">The variance of the first sample.</param>
        /// <param name="var2">The variance of the second sample.</param>
        /// <param name="d1">The degrees of freedom for the first sample.</param>
        /// <param name="d2">The degrees of freedom for the second sample.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public FTest(double var1, double var2, int d1, int d2,
            TwoSampleHypothesis alternate = TwoSampleHypothesis.FirstValueIsGreaterThanSecond)
        {
            Compute(var1 / var2, d1, d2, alternate);
        }

        /// <summary>
        ///   Creates a new F-Test for a given statistic with given degrees of freedom.
        /// </summary>
        /// 
        /// <param name="statistic">The test statistic.</param>
        /// <param name="d1">The degrees of freedom for the numerator.</param>
        /// <param name="d2">The degrees of freedom for the denominator.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public FTest(double statistic, int d1, int d2,
            TwoSampleHypothesis alternate = TwoSampleHypothesis.FirstValueIsGreaterThanSecond)
        {
            Compute(statistic, d1, d2, alternate);
        }

        /// <summary>
        ///   Computes the F-test.
        /// </summary>
        /// 
        protected void Compute(double statistic, int d1, int d2, TwoSampleHypothesis alternate)
        {
            this.Statistic = statistic;
            this.StatisticDistribution = new FDistribution(d1, d2);

            this.Hypothesis = alternate;
            this.Tail = (DistributionTail)alternate;
            this.PValue = StatisticToPValue(Statistic);
            this.OnSizeChanged();
        }

        /// <summary>
        ///   Creates a new F-Test.
        /// </summary>
        /// 
        protected FTest()
        {
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
            if (Double.IsNaN(this.Statistic))
            {
                Trace.TraceWarning("The test statistic is NaN, probably because its standard error is zero. This test is not applicable in this case as the samples do " +
                    "not come from a normal distribution. One way to overcome this problem may be to increase the number of samples in your experiment.");
                return Double.NaN;
            }

            double p;
            switch (Tail)
            {
                case DistributionTail.TwoTail:
                    p = 2.0 * StatisticDistribution.DistributionFunction(Math.Abs(x));
                    break;

                case DistributionTail.OneUpper:
                    p = StatisticDistribution.ComplementaryDistributionFunction(x);
                    break;

                case DistributionTail.OneLower:
                    p = StatisticDistribution.DistributionFunction(x);
                    break;

                default:
                    throw new InvalidOperationException();
            }
            return p;
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
            double f;
            switch (Tail)
            {
                case DistributionTail.OneLower:
                    f = StatisticDistribution.InverseDistributionFunction(p);
                    break;
                case DistributionTail.OneUpper:
                    f = StatisticDistribution.InverseDistributionFunction(1.0 - p);
                    break;
                case DistributionTail.TwoTail:
                    f = StatisticDistribution.InverseDistributionFunction(1.0 - p / 2.0);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return f;
        }
    }
}
