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
    ///   Wilcoxon signed-rank test for the median.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Wilcoxon signed-rank test is a non-parametric statistical hypothesis test
    ///   used when comparing two related samples, matched samples, or repeated measurements 
    ///   on a single sample to assess whether their population mean ranks differ (i.e. it is
    ///   a paired difference test). It can be used as an alternative to the paired <see cref="TTest">
    ///   Student's t-test</see>, <see cref="PairedTTest">t-test for matched pairs</see>, or the t-test
    ///   for dependent samples when the population cannot be assumed to be normally distributed.</para>
    ///   
    /// <para>
    ///   The Wilcoxon signed-rank test is not the same as the <see cref="WilcoxonTest">Wilcoxon rank-sum
    ///   test</see>, although both are nonparametric and involve summation of ranks.</para>
    ///   
    /// <para>
    ///   This test uses the positive W statistic, as explained in 
    ///   https://onlinecourses.science.psu.edu/stat414/node/319 </para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia, The Free Encyclopedia. Wilcoxon signed-rank test. Available on:
    ///       http://en.wikipedia.org/wiki/Wilcoxon_signed-rank_test </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// // This example has been adapted from the Wikipedia's page about
    /// // the Z-Test, available from: http://en.wikipedia.org/wiki/Z-test
    /// 
    /// // We would like to check whether a sample of 20
    /// // students with a median score of 96 points ...
    /// 
    /// double[] sample = 
    /// { 
    ///     106, 115, 96, 88, 91, 88, 81, 104, 99, 68,
    ///     104, 100, 77, 98, 96, 104, 82, 94, 72, 96
    /// };
    /// 
    /// // ... could have happened just by chance inside a 
    /// // population with an hypothesized median of 100 points.
    /// 
    /// double hypothesizedMedian = 100;
    /// 
    /// // So we start by creating the test:
    /// WilcoxonSignedRankTest test = new WilcoxonSignedRankTest(sample,
    ///   hypothesizedMedian, OneSampleHypothesis.ValueIsSmallerThanHypothesis);
    /// 
    /// // Now, we can check whether this result would be
    /// // unlikely under a standard significance level:
    /// 
    /// bool significant = test.Significant; // false (so the event was likely)
    /// 
    /// // We can also check the test statistic and its P-Value
    /// double statistic = test.Statistic; // 40.0
    /// double pvalue = test.PValue; // 0.98585347446367344
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="TTest"/>
    /// 
    /// <seealso cref="SignTest"/>
    /// <seealso cref="WilcoxonTest"/>
    /// <seealso cref="TwoSampleWilcoxonSignedRankTest"/>
    /// 
    /// <seealso cref="Accord.Statistics.Distributions.Univariate.WilcoxonDistribution"/>
    /// 
    [Serializable]
    public class WilcoxonSignedRankTest : WilcoxonTest
    {

        /// <summary>
        ///   Gets the alternative hypothesis under test. If the test is
        ///   <see cref="IHypothesisTest.Significant"/>, the null hypothesis can be rejected
        ///   in favor of this alternative hypothesis.
        /// </summary>
        /// 
        public OneSampleHypothesis Hypothesis { get; protected set; }

        /// <summary>
        ///   Tests the null hypothesis that the sample median is equal to a hypothesized value.
        /// </summary>
        /// 
        /// <param name="sample">The data samples from which the test will be performed.</param>
        /// <param name="hypothesizedMedian">The constant to be compared with the samples.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// <param name="exact">True to compute the exact distribution. May require a significant 
        ///   amount of processing power for large samples (n > 12). If left at null, whether to
        ///   compute the exact or approximate distribution will depend on the number of samples.
        ///   Default is null.</param>
        /// <param name="adjustForTies">Whether to account for ties when computing the
        ///   rank statistics or not. Default is true.</param>
        ///   
        public WilcoxonSignedRankTest(double[] sample, double hypothesizedMedian = 0,
            OneSampleHypothesis alternate = OneSampleHypothesis.ValueIsDifferentFromHypothesis,
            bool? exact = null, bool adjustForTies = true)
        {
            int[] signs = new int[sample.Length];
            double[] diffs = new double[sample.Length];

            // 1. Compute absolute difference and sign function
            for (int i = 0; i < sample.Length; i++)
            {
                double d = sample[i] - hypothesizedMedian;
                signs[i] = Math.Sign(d);
                diffs[i] = Math.Abs(d);
            }

            this.Hypothesis = alternate;

            base.Compute(signs, diffs, (DistributionTail)alternate, exact, adjustForTies);
        }

    }
}
