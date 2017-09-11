// Accord Unit Tests
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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Testing;
    using NUnit.Framework;
    using System;
    using System.Data;
    using Accord.Math;
    using Accord.Statistics;

    [TestFixture]
    public class WilcoxonSignedRankTestTest
    {

        [Test]
        public void WilcoxonSignedRankTestConstructorTest()
        {
            double[] sample = { 17, 50, 45, 59.8, 21.74, 16, 9, 15.43, 5.12, 40, 35, 13.35, 13.4 };

            double hypothesizedMedian = 7.38;

            var target = new WilcoxonSignedRankTest(sample, hypothesizedMedian);

            Assert.IsFalse(target.StatisticDistribution.Exact);
            Assert.AreEqual(OneSampleHypothesis.ValueIsDifferentFromHypothesis, target.Hypothesis);

            double[] delta = { 9.62, 42.62, 37.62, 52.42, 14.36, 8.62, 1.62, 8.05, 2.26, 32.62, 27.62, 5.97, 6.02 };
            double[] ranks = { 7, 12, 11, 13, 8, 6, 1, 5, 2, 10, 9, 3, 4 };

            Assert.IsTrue(delta.IsEqual(target.Delta, 1e-6));
            Assert.IsTrue(ranks.IsEqual(target.Ranks, 1e-6));

            Assert.AreEqual(89, target.Statistic);
            Assert.AreEqual(0.003, target.PValue, 1e-3);
            Assert.IsTrue(target.Significant);
        }

        [Test]
        public void WilcoxonSignedRankTestConstructorTest2()
        {
            // Example from https://onlinecourses.science.psu.edu/stat414/node/319
            double[] sample = { 5.0, 3.9, 5.2, 5.5, 2.8, 6.1, 6.4, 2.6, 1.7, 4.3 };

            double hypothesizedMedian = 3.7;

            var target = new WilcoxonSignedRankTest(sample, hypothesizedMedian);

            Assert.AreEqual(OneSampleHypothesis.ValueIsDifferentFromHypothesis, target.Hypothesis);

            // Absolute differences and original signs
            double[] delta = { +1.3, +0.2, +1.5, +1.8, +0.9, +2.4, +2.7, +1.1, +2.0, +0.6 };
            double[] signs = { +1.0, +1.0, +1.0, +1.0, -1.0, -1.0, +1.0, -1.0, -1.0, +1.0 };
            double[] ranks = { +5.0, +1.0, +6.0, +7.0, +3.0, +9.0, +10, +4.0, +8.0, +2.0 };

            Assert.AreEqual(40, target.Statistic);
            Assert.AreEqual(0.232, target.PValue, 1e-3);
            Assert.IsFalse(target.Significant);
        }

        [Test]
        public void WilcoxonSignedRankTestConstructorTest3()
        {
            // Example from https://onlinecourses.science.psu.edu/stat414/node/319

            double[] sample =
            {
                35.5,   44.5,  39.8,  33.3,  51.4,  51.3,  30.5,  48.9,   42.1,   40.3,
                46.8,   38.0,  40.1,  36.8,  39.3,  65.4,  42.6,  42.8,   59.8,   52.4,
                26.2,   60.9,  45.6,  27.1,  47.3,  36.6,  55.6,  45.1,   52.2,   43.5,
            };

            double hypothesizedMedian = 45;

            var target = new WilcoxonSignedRankTest(sample, hypothesizedMedian);
            Assert.AreEqual(232.5, target.StatisticDistribution.Mean);
            Assert.AreEqual(2363.75, target.StatisticDistribution.Variance);
            Assert.AreEqual(OneSampleHypothesis.ValueIsDifferentFromHypothesis, target.Hypothesis);
            Assert.AreEqual(200, target.Statistic);
            Assert.AreEqual(0.510, target.PValue, 1e-3); // 510 in linked page
            Assert.IsFalse(target.Significant);

            target = new WilcoxonSignedRankTest(sample, hypothesizedMedian, OneSampleHypothesis.ValueIsDifferentFromHypothesis);
            Assert.AreEqual(232.5, target.StatisticDistribution.Mean);
            Assert.AreEqual(2363.75, target.StatisticDistribution.Variance);
            Assert.AreEqual(OneSampleHypothesis.ValueIsDifferentFromHypothesis, target.Hypothesis);
            Assert.AreEqual(200, target.Statistic);
            Assert.AreEqual(0.510, target.PValue, 1e-3); 
            Assert.IsFalse(target.Significant);

            target = new WilcoxonSignedRankTest(sample, hypothesizedMedian, OneSampleHypothesis.ValueIsGreaterThanHypothesis);
            Assert.AreEqual(232.5, target.StatisticDistribution.Mean);
            Assert.AreEqual(2363.75, target.StatisticDistribution.Variance);
            Assert.AreEqual(OneSampleHypothesis.ValueIsGreaterThanHypothesis, target.Hypothesis);
            Assert.AreEqual(200, target.Statistic);
            Assert.AreEqual(0.75135350754785524, target.PValue, 1e-10); 
            Assert.IsFalse(target.Significant);

            target = new WilcoxonSignedRankTest(sample, hypothesizedMedian, OneSampleHypothesis.ValueIsSmallerThanHypothesis);
            Assert.AreEqual(232.5, target.StatisticDistribution.Mean);
            Assert.AreEqual(2363.75, target.StatisticDistribution.Variance);
            Assert.AreEqual(OneSampleHypothesis.ValueIsSmallerThanHypothesis, target.Hypothesis);
            Assert.AreEqual(200, target.Statistic);
            Assert.AreEqual(0.25520903378968796, target.PValue, 1e-10); 
            Assert.IsFalse(target.Significant);
        }

        [Test]
        public void ConstructorTest2_not_exact()
        {
            // This example has been adapted from the Wikipedia's page about
            // the Z-Test, available from: http://en.wikipedia.org/wiki/Z-test

            // We would like to check whether a sample of 20
            // students with a median score of 96 points ...

            double[] sample =
            {
                106, 115, 96, 88, 91, 88, 81, 104, 99, 68,
                104, 100, 77, 98, 96, 104, 82, 94, 72, 96
            };

            // ... could have happened just by chance inside a 
            // population with an hypothesized median of 100 points.

            double hypothesizedMedian = 100;

            // So we start by creating the test:
            var test = new WilcoxonSignedRankTest(sample,
                hypothesizedMedian, OneSampleHypothesis.ValueIsSmallerThanHypothesis);

            // Now, we can check whether this result would be
            // unlikely under a standard significance level:

            bool significant = test.Significant; // true (so the median of the students is
            // significantly lower than 100, more than what could have been due to chance)

            // We can also check the test statistic and its P-Value
            double statistic = test.Statistic; // 40.0
            double pvalue = test.PValue; // 0.0080210309581972838

            /*
              Test against R:
              a <- c(106, 115, 96, 88, 91, 88, 81, 104, 99, 68, 104, 100, 77, 98, 96, 104, 82, 94, 72, 96)
              wilcox.test(a, mu=100, alternative='less') # V = 40, p-value = 0.01385

                Wilcoxon signed rank test with continuity correction

                data:  a
                V = 40, p-value = 0.01385
                alternative hypothesis: true location is less than 100

                Warning messages:
                1: In wilcox.test.default(a, mu = 100, alternative = "less") :
                  cannot compute exact p-value with ties
                2: In wilcox.test.default(a, mu = 100, alternative = "less") :
                  cannot compute exact p-value with zeroes
             */

            Assert.IsFalse(test.StatisticDistribution.Exact);
            Assert.AreEqual(96, sample.Median());
            Assert.AreEqual(statistic, 40.0); 
            Assert.AreEqual(pvalue, 0.01414652553632656, 1e-5);
            Assert.AreEqual(DistributionTail.OneLower, test.Tail);
        }

        [Test]
        public void ConstructorTest2_exact()
        {
            // This example has been adapted from the Wikipedia's page about
            // the Z-Test, available from: http://en.wikipedia.org/wiki/Z-test

            // We would like to check whether a sample of 20
            // students with a median score of 96 points ...

            double[] sample =
            {
                106, 115, 96, 88, 91, 88, 81, 104, 99, 68,
                104, 100.0001, 77, 98, 96, 104, 82, 94, 72, 96
            };

            // ... could have happened just by chance inside a 
            // population with an hypothesized median of 100 points.

            double hypothesizedMedian = 100;

            // So we start by creating the test:
            var test = new WilcoxonSignedRankTest(sample,
                hypothesizedMedian, OneSampleHypothesis.ValueIsSmallerThanHypothesis,
                exact: true);

            // Now, we can check whether this result would be
            // unlikely under a standard significance level:

            bool significant = test.Significant; // true (so the median of the students is
            // significantly lower than 100, more than what could have been due to chance)

            // We can also check the test statistic and its P-Value
            double statistic = test.Statistic; // 40.0
            double pvalue = test.PValue; // 0.012226104736328125

            /*
              Test against R:
              a <- c(106, 115, 96, 88, 91, 88, 81, 104, 99, 68, 104, 100.0001, 77, 98, 96, 104, 82, 94, 72, 96)
              wilcox.test(a, mu=100, alternative='less') # V = 40, p-value = 0.01385
              
                Wilcoxon signed rank test with continuity correction

                data:  a
                V = 46, p-value = 0.01422
                alternative hypothesis: true location is less than 100

                Warning message:
                In wilcox.test.default(a, mu = 100, alternative = "less") :
                  cannot compute exact p-value with ties
             */

            Assert.IsTrue(test.StatisticDistribution.Exact);
            Assert.AreEqual(96, sample.Median());
            Assert.AreEqual(statistic, 46.0);
            Assert.AreEqual(pvalue, 0.012667655944824219, 1e-10);
            Assert.AreEqual(DistributionTail.OneLower, test.Tail);
        }


        [Test]
        public void RCompatibilityTest()
        {
            // Test case from https://github.com/accord-net/framework/issues/389

            /*
                 // Test against R:
                 a <- c(35, 15, 25, 10, 45, 20, 21, 22, 30, 17)
                 b <- c(20, 17, 23, 15, 49, 19, 24, 26, 33, 18)

                 wilcox.test(a, mu=13, alternative="greater")    # V = 53, p-value = 0.00293
                 wilcox.test(a, mu=13, alternative="two.sided")  # V = 53, p-value = 0.005859
                 wilcox.test(a, mu=13, alternative="less")       # V = 53, p-value = 0.998
            */

            var s1 = new double[] { 35, 15, 25, 10, 45, 20, 21, 22, 30, 17 };

            var wilcoxonSignedRankTest = new WilcoxonSignedRankTest(s1, 13, OneSampleHypothesis.ValueIsGreaterThanHypothesis);
            Assert.IsTrue(wilcoxonSignedRankTest.StatisticDistribution.Exact);
            Assert.AreEqual(0.00293, wilcoxonSignedRankTest.PValue, 1e-5);
            Assert.AreEqual(53, wilcoxonSignedRankTest.Statistic);
            Assert.AreEqual(true, wilcoxonSignedRankTest.Significant);
            Assert.AreEqual(OneSampleHypothesis.ValueIsGreaterThanHypothesis, wilcoxonSignedRankTest.Hypothesis);

            wilcoxonSignedRankTest = new WilcoxonSignedRankTest(s1, 13, OneSampleHypothesis.ValueIsDifferentFromHypothesis);
            Assert.IsTrue(wilcoxonSignedRankTest.StatisticDistribution.Exact);
            Assert.AreEqual(0.005859, wilcoxonSignedRankTest.PValue, 1e-5);
            Assert.AreEqual(53, wilcoxonSignedRankTest.Statistic);
            Assert.AreEqual(true, wilcoxonSignedRankTest.Significant);
            Assert.AreEqual(OneSampleHypothesis.ValueIsDifferentFromHypothesis, wilcoxonSignedRankTest.Hypothesis);

            wilcoxonSignedRankTest = new WilcoxonSignedRankTest(s1, 13, OneSampleHypothesis.ValueIsSmallerThanHypothesis);
            Assert.IsTrue(wilcoxonSignedRankTest.StatisticDistribution.Exact);
            Assert.AreEqual(0.998, wilcoxonSignedRankTest.PValue, 1e-2);
            Assert.AreEqual(53, wilcoxonSignedRankTest.Statistic);
            Assert.AreEqual(false, wilcoxonSignedRankTest.Significant);
            Assert.AreEqual(OneSampleHypothesis.ValueIsSmallerThanHypothesis, wilcoxonSignedRankTest.Hypothesis);




            wilcoxonSignedRankTest = new WilcoxonSignedRankTest(s1, 13, OneSampleHypothesis.ValueIsGreaterThanHypothesis, exact: false);
            Assert.AreEqual(0.0054134606385656562, wilcoxonSignedRankTest.PValue, 1e-5);
            Assert.AreEqual(53, wilcoxonSignedRankTest.Statistic);
            Assert.AreEqual(true, wilcoxonSignedRankTest.Significant);
            Assert.IsFalse(wilcoxonSignedRankTest.StatisticDistribution.Exact);
            Assert.AreEqual(OneSampleHypothesis.ValueIsGreaterThanHypothesis, wilcoxonSignedRankTest.Hypothesis);

            wilcoxonSignedRankTest = new WilcoxonSignedRankTest(s1, 13, OneSampleHypothesis.ValueIsDifferentFromHypothesis, exact: false);
            Assert.AreEqual(0.0093441130022048919, wilcoxonSignedRankTest.PValue, 2e-3);
            Assert.AreEqual(53, wilcoxonSignedRankTest.Statistic);
            Assert.AreEqual(true, wilcoxonSignedRankTest.Significant);
            Assert.IsFalse(wilcoxonSignedRankTest.StatisticDistribution.Exact);
            Assert.AreEqual(OneSampleHypothesis.ValueIsDifferentFromHypothesis, wilcoxonSignedRankTest.Hypothesis);

            wilcoxonSignedRankTest = new WilcoxonSignedRankTest(s1, 13, OneSampleHypothesis.ValueIsSmallerThanHypothesis, exact: false);
            Assert.AreEqual(0.9959773812567192, wilcoxonSignedRankTest.PValue, 1e-5);
            Assert.AreEqual(53, wilcoxonSignedRankTest.Statistic);
            Assert.AreEqual(false, wilcoxonSignedRankTest.Significant);
            Assert.IsFalse(wilcoxonSignedRankTest.StatisticDistribution.Exact);
            Assert.AreEqual(OneSampleHypothesis.ValueIsSmallerThanHypothesis, wilcoxonSignedRankTest.Hypothesis);
        }
    }
}
