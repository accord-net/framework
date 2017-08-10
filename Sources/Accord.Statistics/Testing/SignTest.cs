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
    ///   Sign test for the median.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In statistics, the sign test can be used to test the hypothesis that the difference
    ///   median is zero between the continuous distributions of two random variables X and Y,
    ///   in the situation when we can draw paired samples from X and Y. It is a non-parametric
    ///   test which makes very few assumptions about the nature of the distributions under test
    ///   - this means that it has very general applicability but may lack the <see cref="Accord.Statistics.Testing.Power">
    ///   statistical power</see> of other tests such as the <see cref="PairedTTest">paired-samples
    ///   t-test</see> or the <see cref="WilcoxonSignedRankTest">Wilcoxon signed-rank test</see>.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia, The Free Encyclopedia. Sign test. Available on:
    ///       http://en.wikipedia.org/wiki/Sign_test </description></item>
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
    /// SignTest test = new SignTest(sample, hypothesizedMedian,
    ///                 OneSampleHypothesis.ValueIsSmallerThanHypothesis);
    /// 
    /// // Now, we can check whether this result would be
    /// // unlikely under a standard significance level:
    /// 
    /// bool significant = test.Significant; // false (so the event was likely)
    /// 
    /// // We can also check the test statistic and its P-Value
    /// double statistic = test.Statistic; // 5
    /// double pvalue = test.PValue; // 0.99039
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="WilcoxonSignedRankTest"/>
    /// 
    /// <seealso cref="BinomialTest"/>
    /// <seealso cref="Accord.Statistics.Distributions.Univariate.BinomialDistribution"/>
    /// 
    [Serializable]
    public class SignTest : BinomialTest
    {

        /// <summary>
        ///   Gets the alternative hypothesis under test. If the test is
        ///   <see cref="IHypothesisTest.Significant"/>, the null hypothesis
        ///   can be rejected in favor of this alternative hypothesis.
        /// </summary>
        /// 
        public new OneSampleHypothesis Hypothesis { get; protected set; }

        /// <summary>
        ///   Tests the null hypothesis that the sample median is equal to a hypothesized value.
        /// </summary>
        /// 
        /// <param name="positiveSamples">The number of positive samples.</param>
        /// <param name="totalSamples">The total number of samples.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public SignTest(int positiveSamples, int totalSamples, OneSampleHypothesis alternate)
        {
            Compute(positiveSamples, totalSamples, alternate);
        }

        /// <summary>
        ///   Tests the null hypothesis that the sample median is equal to a hypothesized value.
        /// </summary>
        /// 
        /// <param name="sample">The data samples from which the test will be performed.</param>
        /// <param name="hypothesizedMedian">The constant to be compared with the samples.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public SignTest(double[] sample, double hypothesizedMedian = 0,
            OneSampleHypothesis alternate = OneSampleHypothesis.ValueIsDifferentFromHypothesis)
        {
            int positive = 0;
            int negative = 0;

            for (int i = 0; i < sample.Length; i++)
            {
                double d = sample[i] - hypothesizedMedian;

                if (d > 0)
                    positive++;
                else if (d < 0)
                    negative++;
            }


            Compute(positive, positive + negative, alternate);
        }

        /// <summary>
        ///   Computes the one sample sign test.
        /// </summary>
        /// 
        protected void Compute(int positive, int total, OneSampleHypothesis alternate)
        {
            this.Hypothesis = alternate;

            // The underlying test is to check whether the probability
            // value from the samples are higher than or lesser than 0.5,
            // thus the actual Binomial test hypothesis is inverted:

            OneSampleHypothesis binomialHypothesis;

            switch (alternate)
            {
                case OneSampleHypothesis.ValueIsDifferentFromHypothesis:
                    binomialHypothesis = OneSampleHypothesis.ValueIsDifferentFromHypothesis; break;
                case OneSampleHypothesis.ValueIsGreaterThanHypothesis:
                    binomialHypothesis = OneSampleHypothesis.ValueIsSmallerThanHypothesis; break;
                case OneSampleHypothesis.ValueIsSmallerThanHypothesis:
                    binomialHypothesis = OneSampleHypothesis.ValueIsGreaterThanHypothesis; break;
                default: throw new InvalidOperationException();
            }

            base.Compute(positive, total, 0.5, binomialHypothesis);
        }


    }
}
