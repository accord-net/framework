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
    using Accord.Math;
    using Accord.Statistics.Testing;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class MannWhitneyWilcoxonTestTest
    {

        [Test]
        public void MannWhitneyWilcoxonTestConstructorTest()
        {
            // The following example comes from Richard Lowry's page at http://vassarstats.net/textbook/ch11a.html. As 
            // stated by Richard, this example deals with persons seeking treatment by claustrophobia. Those persons are 
            // randomly divided into two groups, and each group receive a different treatment for the disorder.

            // The hypothesis would be that treatment A would more effective than B. To check this 
            // hypothesis, we can use Mann -Whitney's Test to compare the medians of both groups.

            // Claustrophobia test scores for people treated with treatment A
            double[] sample1 = { 4.6, 4.7, 4.9, 5.1, 5.2, 5.5, 5.8, 6.1, 6.5, 6.5, 7.2 };

            // Claustrophobia test scores for people treated with treatment B
            double[] sample2 = { 5.2, 5.3, 5.4, 5.6, 6.2, 6.3, 6.8, 7.7, 8.0, 8.1 };

            // Create a new Mann-Whitney-Wilcoxon's test to compare the two samples
            var test = new MannWhitneyWilcoxonTest(sample1, sample2,
                TwoSampleHypothesis.FirstValueIsGreaterThanSecond);
            Assert.IsTrue(test.StatisticDistribution.Exact);

            Assert.AreEqual(096.5, test.RankSum1);
            Assert.AreEqual(134.5, test.RankSum2);

            Assert.AreEqual(30.5, test.Statistic1);
            Assert.AreEqual(79.5, test.Statistic2);
            //Assert.AreEqual(30.5, test.Statistic);

            Assert.AreEqual(DistributionTail.OneUpper, test.Tail);
            Assert.AreEqual(TwoSampleHypothesis.FirstValueIsGreaterThanSecond, test.Hypothesis);
            Assert.IsTrue(test.StatisticDistribution.Exact);

            Assert.IsFalse(test.Significant);
            Assert.AreEqual(0.959420610349403, test.PValue, 1e-4);



            // Create a new Mann-Whitney-Wilcoxon's test to compare the two samples
            test = new MannWhitneyWilcoxonTest(sample1, sample2);

            Assert.AreEqual(TwoSampleHypothesis.ValuesAreDifferent, test.Hypothesis);

            double[] expectedRank1 = { 1, 2, 3, 4, 5.5, 9, 11, 12, 15.5, 15.5, 18 };
            double[] expectedRank2 = { 5.5, 7, 8, 10, 13, 14, 17, 19, 20, 21 };

            Assert.IsTrue(test.Rank1.IsEqual(expectedRank1));
            Assert.IsTrue(test.Rank2.IsEqual(expectedRank2));

            Assert.AreEqual(096.5, test.RankSum1);
            Assert.AreEqual(134.5, test.RankSum2);

            Assert.AreEqual(30.5, test.Statistic1);
            Assert.AreEqual(79.5, test.Statistic2);
            //Assert.AreEqual(30.5, test.Statistic);

            // Non-directional is non-significant
            Assert.IsFalse(test.Significant);
            Assert.IsTrue(test.StatisticDistribution.Exact);

            Assert.AreEqual(0.087668265686841537, test.PValue, 1e-4);



            // Create a new Mann-Whitney-Wilcoxon's test to compare the two samples
            test = new MannWhitneyWilcoxonTest(sample1, sample2,
                TwoSampleHypothesis.FirstValueIsSmallerThanSecond);

            Assert.AreEqual(096.5, test.RankSum1);
            Assert.AreEqual(134.5, test.RankSum2);

            Assert.AreEqual(30.5, test.Statistic1);
            Assert.AreEqual(79.5, test.Statistic2);
            //Assert.AreEqual(30.5, test.Statistic);

            Assert.IsTrue(test.StatisticDistribution.Exact);
            Assert.AreEqual(DistributionTail.OneLower, test.Tail);
            Assert.AreEqual(TwoSampleHypothesis.FirstValueIsSmallerThanSecond, test.Hypothesis);

            // Directional should be significant
            Assert.IsTrue(test.Significant);
            Assert.AreEqual(0.043834132843420769, test.PValue, 1e-4);
        }

        [Test]
        public void same_as_above_inverted_order()
        {
            double[] sample2 = { 4.6, 4.7, 4.9, 5.1, 5.2, 5.5, 5.8, 6.1, 6.5, 6.5, 7.2 };
            double[] sample1 = { 5.2, 5.3, 5.4, 5.6, 6.2, 6.3, 6.8, 7.7, 8.0, 8.1 };

            var test = new MannWhitneyWilcoxonTest(sample1, sample2,
                TwoSampleHypothesis.FirstValueIsGreaterThanSecond);

            Assert.AreEqual(096.5, test.RankSum2);
            Assert.AreEqual(134.5, test.RankSum1);

            Assert.AreEqual(79.5, test.Statistic1);
            Assert.AreEqual(30.5, test.Statistic2);
            //Assert.AreEqual(30.5, test.Statistic);

            Assert.AreEqual(DistributionTail.OneUpper, test.Tail);
            Assert.AreEqual(TwoSampleHypothesis.FirstValueIsGreaterThanSecond, test.Hypothesis);

            Assert.IsTrue(test.Significant);
            Assert.AreEqual(0.043834132843420769, test.PValue, 1e-10);


            test = new MannWhitneyWilcoxonTest(sample1, sample2);
            Assert.AreEqual(TwoSampleHypothesis.ValuesAreDifferent, test.Hypothesis);

            double[] expectedRank2 = { 1, 2, 3, 4, 5.5, 9, 11, 12, 15.5, 15.5, 18 };
            double[] expectedRank1 = { 5.5, 7, 8, 10, 13, 14, 17, 19, 20, 21 };

            Assert.IsTrue(test.Rank1.IsEqual(expectedRank1));
            Assert.IsTrue(test.Rank2.IsEqual(expectedRank2));

            Assert.AreEqual(096.5, test.RankSum2);
            Assert.AreEqual(134.5, test.RankSum1);

            Assert.AreEqual(79.5, test.Statistic1);
            Assert.AreEqual(30.5, test.Statistic2);

            Assert.IsFalse(test.Significant);
            Assert.AreEqual(0.087668265686841537, test.PValue, 1e-10);



            // Create a new Mann-Whitney-Wilcoxon's test to compare the two samples
            test = new MannWhitneyWilcoxonTest(sample1, sample2,
                TwoSampleHypothesis.FirstValueIsSmallerThanSecond);

            Assert.AreEqual(096.5, test.RankSum2);
            Assert.AreEqual(134.5, test.RankSum1);

            Assert.AreEqual(79.5, test.Statistic1);
            Assert.AreEqual(30.5, test.Statistic2);
            //Assert.AreEqual(30.5, test.Statistic);

            Assert.AreEqual(DistributionTail.OneLower, test.Tail);
            Assert.AreEqual(TwoSampleHypothesis.FirstValueIsSmallerThanSecond, test.Hypothesis);

            Assert.IsFalse(test.Significant);
            Assert.AreEqual(0.959420610349403, test.PValue, 1e-10);
        }

        [Test]
        public void same_as_above_non_exact()
        {
            double[] sample2 = { 4.6, 4.7, 4.9, 5.1, 5.2, 5.5, 5.8, 6.1, 6.5, 6.5, 7.2 };
            double[] sample1 = { 5.2, 5.3, 5.4, 5.6, 6.2, 6.3, 6.8, 7.7, 8.0, 8.1 };

            // Create a new Mann-Whitney-Wilcoxon's test to compare the two samples
            var test = new MannWhitneyWilcoxonTest(sample1, sample2,
                TwoSampleHypothesis.FirstValueIsGreaterThanSecond, exact: false);

            Assert.AreEqual(096.5, test.RankSum2);
            Assert.AreEqual(134.5, test.RankSum1);

            Assert.AreEqual(79.5, test.Statistic1);
            Assert.AreEqual(30.5, test.Statistic2);
            //Assert.AreEqual(30.5, test.Statistic);

            Assert.AreEqual(DistributionTail.OneUpper, test.Tail);
            Assert.AreEqual(TwoSampleHypothesis.FirstValueIsGreaterThanSecond, test.Hypothesis);

            Assert.IsTrue(test.Significant);
            Assert.AreEqual(0.0454062185864162, test.PValue, 1e-10);



            // Create a new Mann-Whitney-Wilcoxon's test to compare the two samples
            test = new MannWhitneyWilcoxonTest(sample1, sample2, exact: false);

            Assert.AreEqual(TwoSampleHypothesis.ValuesAreDifferent, test.Hypothesis);

            double[] expectedRank2 = { 1, 2, 3, 4, 5.5, 9, 11, 12, 15.5, 15.5, 18 };
            double[] expectedRank1 = { 5.5, 7, 8, 10, 13, 14, 17, 19, 20, 21 };

            Assert.IsTrue(test.Rank1.IsEqual(expectedRank1));
            Assert.IsTrue(test.Rank2.IsEqual(expectedRank2));

            Assert.AreEqual(096.5, test.RankSum2);
            Assert.AreEqual(134.5, test.RankSum1);

            Assert.AreEqual(79.5, test.Statistic1);
            Assert.AreEqual(30.5, test.Statistic2);

            // Non-directional is non-significant
            Assert.IsFalse(test.Significant);
            Assert.AreEqual(0.0908124371728325, test.PValue, 1e-10);



            // Create a new Mann-Whitney-Wilcoxon's test to compare the two samples
            test = new MannWhitneyWilcoxonTest(sample1, sample2,
                TwoSampleHypothesis.FirstValueIsSmallerThanSecond, exact: false);

            Assert.AreEqual(096.5, test.RankSum2);
            Assert.AreEqual(134.5, test.RankSum1);

            Assert.AreEqual(79.5, test.Statistic1);
            Assert.AreEqual(30.5, test.Statistic2);

            Assert.AreEqual(DistributionTail.OneLower, test.Tail);
            Assert.AreEqual(TwoSampleHypothesis.FirstValueIsSmallerThanSecond, test.Hypothesis);

            // Directional should be significant
            Assert.IsFalse(test.Significant);
            Assert.AreEqual(0.96093080726732927, test.PValue, 1e-10);
        }

        [Test]
        public void non_exact_symmetry()
        {
            /*
                 a = c(4.6, 4.7, 4.9, 5.1, 5.2, 5.5, 5.8, 6.1, 6.5, 6.5, 7.2)
                 b = c(5.2, 5.3, 5.4, 5.6, 6.2, 6.3, 6.8, 7.7, 8.0, 8.1)
                 wilcox.test(a,b,'greater') # W = 30.5, p-value = 0.9609
                 wilcox.test(a,b,'less')    # W = 30.5, p-value = 0.04541
            */
            double[] sample2 = { 4.6, 4.7, 4.9, 5.1, 5.2, 5.5, 5.8, 6.1, 6.5, 6.5, 7.2 };
            double[] sample1 = { 5.2, 5.3, 5.4, 5.6, 6.2, 6.3, 6.8, 7.7, 8.0, 8.1 };
            {
                // Those should be equal
                var e12 = new MannWhitneyWilcoxonTest(sample1, sample2,
                    TwoSampleHypothesis.FirstValueIsSmallerThanSecond, exact: true);
                Assert.AreEqual(e12.PValue, 0.96093080726732927, 1e-5); // ok
                Assert.IsFalse(e12.IsExact); // the data contains ties
                var a12 = new MannWhitneyWilcoxonTest(sample1, sample2,
                    TwoSampleHypothesis.FirstValueIsSmallerThanSecond, exact: false);
                Assert.IsFalse(a12.IsExact); // the data contains ties
                Assert.AreEqual(a12.PValue, 0.96093080726732927, 1e-5); // ok
                var e21 = new MannWhitneyWilcoxonTest(sample2, sample1,
                    TwoSampleHypothesis.FirstValueIsGreaterThanSecond, exact: true);
                Assert.IsFalse(e21.IsExact); // the data contains ties
                Assert.AreEqual(e21.PValue, 0.96093080726732927, 1e-5); // ok
                var a21 = new MannWhitneyWilcoxonTest(sample2, sample1,
                    TwoSampleHypothesis.FirstValueIsGreaterThanSecond, exact: false);
                Assert.AreEqual(a21.PValue, 0.96093080726732927, 1e-5); // ok
                Assert.IsFalse(a21.IsExact); // the data contains ties

                Assert.AreEqual(a12.PValue, e12.PValue, 1e-8);
                Assert.AreEqual(a21.PValue, e21.PValue, 1e-8);
                Assert.AreEqual(a12.PValue, e21.PValue, 1e-8);
            }

            {
                // Those should be equal
                var e12 = new MannWhitneyWilcoxonTest(sample1, sample2,
                    TwoSampleHypothesis.FirstValueIsGreaterThanSecond, exact: true);
                Assert.AreEqual(e12.PValue, 0.0454062185864162, 1e-5); // ok
                var a12 = new MannWhitneyWilcoxonTest(sample1, sample2,
                    TwoSampleHypothesis.FirstValueIsGreaterThanSecond, exact: false);
                Assert.AreEqual(a12.PValue, 0.0454062185864162, 1e-5); // ok
                var e21 = new MannWhitneyWilcoxonTest(sample2, sample1,
                    TwoSampleHypothesis.FirstValueIsSmallerThanSecond, exact: true);
                Assert.AreEqual(e21.PValue, 0.0454062185864162, 1e-5); // ok
                var a21 = new MannWhitneyWilcoxonTest(sample2, sample1,
                    TwoSampleHypothesis.FirstValueIsSmallerThanSecond, exact: false);
                Assert.AreEqual(a21.PValue, 0.0454062185864162, 1e-5); // ok

                Assert.AreEqual(a12.PValue, e12.PValue, 1e-8);
                Assert.AreEqual(a21.PValue, e21.PValue, 1e-8);
                Assert.AreEqual(a12.PValue, e21.PValue, 1e-8);
            }
        }

        [Test]
        public void non_exact_symmetry_same_size()
        {
            double[] sample2 = { 4.6, 4.7, 4.9, 5.1, 5.2, 5.5, 5.8, 6.1, 6.5, 6.5 };
            double[] sample1 = { 5.2, 5.3, 5.4, 5.6, 6.2, 6.3, 6.8, 7.7, 8.0, 8.1 };
            {
                // Those should be equal
                var e12 = new MannWhitneyWilcoxonTest(sample1, sample2,
                    TwoSampleHypothesis.FirstValueIsSmallerThanSecond, exact: true);
                Assert.AreEqual(e12.PValue, 0.97945122400837448, 1e-5); // ok
                var a12 = new MannWhitneyWilcoxonTest(sample1, sample2,
                    TwoSampleHypothesis.FirstValueIsSmallerThanSecond, exact: false);
                Assert.AreEqual(a12.PValue, 0.97945122400837448, 1e-5); // ok
                var e21 = new MannWhitneyWilcoxonTest(sample2, sample1,
                    TwoSampleHypothesis.FirstValueIsGreaterThanSecond, exact: true);
                Assert.AreEqual(e21.PValue, 0.97945122400837448, 1e-5); // ok
                var a21 = new MannWhitneyWilcoxonTest(sample2, sample1,
                    TwoSampleHypothesis.FirstValueIsGreaterThanSecond, exact: false);
                Assert.AreEqual(a21.PValue, 0.97945122400837448, 1e-5); // ok

                Assert.IsFalse(a12.IsExact);
                Assert.IsFalse(a21.IsExact);
                Assert.IsFalse(a12.IsExact);
                Assert.IsFalse(e12.IsExact);

                Assert.AreEqual(a12.PValue, e12.PValue, 1e-8);
                Assert.AreEqual(a21.PValue, e21.PValue, 1e-8);
                Assert.AreEqual(a12.PValue, e21.PValue, 1e-8);
                Assert.AreEqual(e12.PValue, e21.PValue, 1e-8);
            }

            {
                // Those should be equal
                var e12 = new MannWhitneyWilcoxonTest(sample1, sample2,
                    TwoSampleHypothesis.FirstValueIsGreaterThanSecond, exact: true);
                Assert.AreEqual(e12.PValue, 0.024597676755701903, 1e-5); // ok
                var a12 = new MannWhitneyWilcoxonTest(sample1, sample2,
                    TwoSampleHypothesis.FirstValueIsGreaterThanSecond, exact: false);
                Assert.AreEqual(a12.PValue, 0.024597676755701903, 1e-5); // ok
                var e21 = new MannWhitneyWilcoxonTest(sample2, sample1,
                    TwoSampleHypothesis.FirstValueIsSmallerThanSecond, exact: true);
                Assert.AreEqual(e21.PValue, 0.024597676755701903, 1e-5); // ok
                var a21 = new MannWhitneyWilcoxonTest(sample2, sample1,
                    TwoSampleHypothesis.FirstValueIsSmallerThanSecond, exact: false);
                Assert.AreEqual(a21.PValue, 0.024597676755701903, 1e-5); // ok

                Assert.IsFalse(a12.IsExact);
                Assert.IsFalse(a21.IsExact);
                Assert.IsFalse(a12.IsExact);
                Assert.IsFalse(e12.IsExact);

                Assert.AreEqual(a12.PValue, e12.PValue, 1e-8);
                Assert.AreEqual(a21.PValue, e21.PValue, 1e-8);
                Assert.AreEqual(a12.PValue, e21.PValue, 1e-8);
                Assert.AreEqual(e12.PValue, e21.PValue, 1e-8);
            }
        }


        [Test]
        public void MannWhitneyWilcoxonTestConstructorTest1()
        {
            // Example from Conover, "Practical nonparametric statistics", 1999  (pg. 218)

            double[] sample1 = { 14.8, 7.3, 5.6, 6.3, 9.0, 4.2, 10.6, 12.5, 12.9, 16.1, 11.4, 2.7 };

            double[] sample2 =
            {
                12.7, 14.2, 12.6, 2.1, 17.7, 11.8, 16.9, 7.9, 16.0, 10.6, 5.6,
                5.6, 7.6, 11.3, 8.3,6.7, 3.6, 1.0, 2.4, 6.4, 9.1, 6.7, 18.6, 3.2,
                6.2, 6.1, 15.3, 10.6, 1.8, 5.9, 9.9, 10.6, 14.8, 5.0, 2.6, 4.0
            };

            var target = new MannWhitneyWilcoxonTest(sample1, sample2);
            Assert.IsFalse(target.StatisticDistribution.Exact);
            Assert.AreEqual(216, target.StatisticDistribution.Mean); // mean of U (not W as in R)
            Assert.AreEqual(1762.4680851063831, target.StatisticDistribution.Variance, 1e-10);
            Assert.AreEqual(0.52789241803534459, target.PValue, 1e-10);

            target = new MannWhitneyWilcoxonTest(sample1, sample2, TwoSampleHypothesis.FirstValueIsGreaterThanSecond);
            Assert.IsFalse(target.StatisticDistribution.Exact);
            Assert.AreEqual(DistributionTail.OneUpper, target.Tail);
            Assert.AreEqual(target.Tail, DistributionTail.OneUpper);
            Assert.AreEqual(0.2639462090176723, target.PValue, 1e-10);

            target = new MannWhitneyWilcoxonTest(sample1, sample2, TwoSampleHypothesis.FirstValueIsSmallerThanSecond);
            Assert.IsFalse(target.StatisticDistribution.Exact);
            Assert.AreEqual(DistributionTail.OneLower, target.Tail);
            Assert.AreEqual(0.743781059309828, target.PValue, 1e-10);



            // Same but with inverted sample1 & sample2
            target = new MannWhitneyWilcoxonTest(sample2, sample1);
            Assert.IsFalse(target.StatisticDistribution.Exact);
            Assert.AreEqual(216, target.StatisticDistribution.Mean); // mean of U (not W as in R)
            Assert.AreEqual(1762.4680851063831, target.StatisticDistribution.Variance, 1e-10);
            Assert.AreEqual(0.52789241803534459, target.PValue, 1e-10);

            target = new MannWhitneyWilcoxonTest(sample2, sample1, TwoSampleHypothesis.FirstValueIsGreaterThanSecond);
            Assert.IsFalse(target.StatisticDistribution.Exact);
            Assert.AreEqual(DistributionTail.OneUpper, target.Tail);
            Assert.AreEqual(0.743781059309828, target.PValue, 1e-10);

            target = new MannWhitneyWilcoxonTest(sample2, sample1, TwoSampleHypothesis.FirstValueIsSmallerThanSecond);
            Assert.IsFalse(target.StatisticDistribution.Exact);
            Assert.AreEqual(DistributionTail.OneLower, target.Tail);
            Assert.AreEqual(0.2639462090176723, target.PValue, 1e-10);
        }

        [Test]
        public void RCompatibilityTest()
        {
            // Test case from https://github.com/accord-net/framework/issues/389

            /*
                 // Test against R:
                 a <- c(35, 15, 25, 10, 45, 20, 21, 22, 30, 17)
                 b <- c(20, 17, 23, 15, 49, 19, 24, 26, 33, 18)

                 wilcox.test(a,b, paired=FALSE, alternative="greater")     # W = 49.5, p-value = 0.5302
                 wilcox.test(a,b, paired=FALSE, alternative="two.sided")   # W = 49.5, p-value = 1     
                 wilcox.test(a,b, paired=FALSE, alternative="less")        # W = 49.5, p-value = 0.5   
            */

            var s1 = new double[] { 35, 15, 25, 10, 45, 20, 21, 22, 30, 17 };
            var s2 = new double[] { 20, 17, 23, 15, 49, 19, 24, 26, 33, 18 };

            var mannWhitneyWilcoxonTest = new MannWhitneyWilcoxonTest(s1, s2,
                TwoSampleHypothesis.FirstValueIsGreaterThanSecond, exact: false);
            Assert.AreEqual(0.5302, mannWhitneyWilcoxonTest.PValue, 1e-4);
            // Assert.AreEqual(49.5, mannWhitneyWilcoxonTest.Statistic);
            Assert.AreEqual(49.5, mannWhitneyWilcoxonTest.Statistic1);
            Assert.AreEqual(50.5, mannWhitneyWilcoxonTest.Statistic2);
            Assert.AreEqual(false, mannWhitneyWilcoxonTest.Significant);
            Assert.AreEqual(DistributionTail.OneUpper, mannWhitneyWilcoxonTest.Tail);
            Assert.AreEqual(TwoSampleHypothesis.FirstValueIsGreaterThanSecond, mannWhitneyWilcoxonTest.Hypothesis);

            mannWhitneyWilcoxonTest = new MannWhitneyWilcoxonTest(s1, s2,
                TwoSampleHypothesis.ValuesAreDifferent, exact: false);
            Assert.AreEqual(1, mannWhitneyWilcoxonTest.PValue);
            // Assert.AreEqual(49.5, mannWhitneyWilcoxonTest.Statistic);
            Assert.AreEqual(49.5, mannWhitneyWilcoxonTest.Statistic1);
            Assert.AreEqual(50.5, mannWhitneyWilcoxonTest.Statistic2);
            Assert.AreEqual(false, mannWhitneyWilcoxonTest.Significant);
            Assert.AreEqual(TwoSampleHypothesis.ValuesAreDifferent, mannWhitneyWilcoxonTest.Hypothesis);

            mannWhitneyWilcoxonTest = new MannWhitneyWilcoxonTest(s1, s2,
                TwoSampleHypothesis.FirstValueIsSmallerThanSecond, exact: false);
            Assert.AreEqual(0.5, mannWhitneyWilcoxonTest.PValue);
            //Assert.AreEqual(49.5, mannWhitneyWilcoxonTest.Statistic);
            Assert.AreEqual(49.5, mannWhitneyWilcoxonTest.Statistic1);
            Assert.AreEqual(50.5, mannWhitneyWilcoxonTest.Statistic2);
            Assert.AreEqual(false, mannWhitneyWilcoxonTest.Significant);
            Assert.AreEqual(TwoSampleHypothesis.FirstValueIsSmallerThanSecond, mannWhitneyWilcoxonTest.Hypothesis);
        }

        [Test]
        public void RCompatibilityTest2()
        {
            // Test case from https://github.com/accord-net/framework/issues/389

            /*
                 // Test against R:
                 c <- c(45,45,15,50,30,15,30,35,25)
                 d <- c(45,55,20,55,20,25,35,45,20)
                 wilcox.test(c,d, paired=FALSE, alternative="two.sided", conf.level=0.95, exact=TRUE) # W=49.5, p-value=0.6557
                 wilcox.test(c,d, paired=FALSE, alternative="less", conf.level=0.95, exact=TRUE)      # W=49.5, p-value=0.3278
                 wilcox.test(c,d, paired=FALSE, alternative="greater", conf.level=0.95, exact=TRUE)   # W=35, p-value=0.7037
            */

            var s1 = new double[] { 45, 45, 15, 50, 30, 15, 30, 35, 25 };
            var s2 = new double[] { 45, 55, 20, 55, 20, 25, 35, 45, 20 };

            var mannWhitneyWilcoxonTest = new MannWhitneyWilcoxonTest(s1, s2,
                TwoSampleHypothesis.FirstValueIsGreaterThanSecond, exact: false);
            Assert.AreEqual(0.7037, mannWhitneyWilcoxonTest.PValue, 1e-4);
            // Assert.AreEqual(49.5, mannWhitneyWilcoxonTest.Statistic);
            Assert.AreEqual(35, mannWhitneyWilcoxonTest.Statistic1);
            Assert.AreEqual(46, mannWhitneyWilcoxonTest.Statistic2);
            Assert.AreEqual(false, mannWhitneyWilcoxonTest.Significant);
            Assert.AreEqual(DistributionTail.OneUpper, mannWhitneyWilcoxonTest.Tail);
            Assert.AreEqual(TwoSampleHypothesis.FirstValueIsGreaterThanSecond, mannWhitneyWilcoxonTest.Hypothesis);

            mannWhitneyWilcoxonTest = new MannWhitneyWilcoxonTest(s1, s2,
                TwoSampleHypothesis.ValuesAreDifferent, exact: false);
            Assert.AreEqual(0.6557, mannWhitneyWilcoxonTest.PValue, 1e-4);
            // Assert.AreEqual(49.5, mannWhitneyWilcoxonTest.Statistic);
            Assert.AreEqual(35, mannWhitneyWilcoxonTest.Statistic1);
            Assert.AreEqual(46, mannWhitneyWilcoxonTest.Statistic2);
            Assert.AreEqual(false, mannWhitneyWilcoxonTest.Significant);
            Assert.AreEqual(TwoSampleHypothesis.ValuesAreDifferent, mannWhitneyWilcoxonTest.Hypothesis);

            mannWhitneyWilcoxonTest = new MannWhitneyWilcoxonTest(s1, s2,
                TwoSampleHypothesis.FirstValueIsSmallerThanSecond, exact: false);
            Assert.AreEqual(0.3278, mannWhitneyWilcoxonTest.PValue, 1e-4);
            //Assert.AreEqual(49.5, mannWhitneyWilcoxonTest.Statistic);
            Assert.AreEqual(35, mannWhitneyWilcoxonTest.Statistic1);
            Assert.AreEqual(46, mannWhitneyWilcoxonTest.Statistic2);
            Assert.AreEqual(false, mannWhitneyWilcoxonTest.Significant);
            Assert.AreEqual(TwoSampleHypothesis.FirstValueIsSmallerThanSecond, mannWhitneyWilcoxonTest.Hypothesis);
        }

        [Test]
        public void same_vectors()
        {
            // https://github.com/accord-net/framework/issues/857

            /*
                a <- c(250,200,450,400,250,250,350,0,200,400,300,600,200,200,
                       550,100,300,250,350,200,550,200,450,400,200,400,450,
                       200,400,400,500,450,300,250,200)
                b <- c(250,200,450,400,250,250,350,0,200,400,300,600,200,200,
                       550,100,300,250,350,200,550,200,450,400,200,400,450,
                       200,400,400,500,450,300,250,200)

                wilcox.test(a, b)
             */

            double[] a = new double[] { 250, 200, 450, 400, 250, 250, 350, 0, 200, 400, 300, 600, 200, 200,
                550, 100, 300, 250, 350, 200, 550, 200, 450, 400, 200, 400, 450,
                200, 400, 400, 500, 450, 300, 250, 200 };
            double[] b = new double[] { 250, 200, 450, 400, 250, 250, 350, 0, 200, 400, 300, 600, 200, 200,
               550, 100, 300, 250, 350, 200, 550, 200, 450, 400, 200, 400, 450,
               200, 400, 400, 500, 450, 300, 250, 200 };

            var target = new MannWhitneyWilcoxonTest(a, b, adjustForTies: true);
            Assert.AreEqual(1.0, target.PValue, 5e-3);
        }


        [Test]
        public void symmetry_test()
        {
            // Example from https://www.r-bloggers.com/wilcoxon-mann-whitney-rank-sum-test-or-test-u/

            double[] a = { 6, 8, 2, 4, 4, 5 };
            double[] b = { 7, 10, 4, 3, 5, 6 };

            var ab = new MannWhitneyWilcoxonTest(a, b);
            var ba = new MannWhitneyWilcoxonTest(b, a);

            Assert.AreEqual(0.5736, ab.PValue, 1e-4);
            Assert.AreEqual(ab.PValue, ba.PValue, 1e-10);
        }
    }
}
