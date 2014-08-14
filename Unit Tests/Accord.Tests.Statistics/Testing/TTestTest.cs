// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Statistics.Testing.Power;
    using System;

    [TestClass()]
    public class TTestTest
    {


        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }



        [TestMethod()]
        public void TTestConstructorTest()
        {

            // mean = 0.5, var = 1
            double[] sample = 
            { 
                -0.849886940156521, 3.53492346633185,  1.22540422494611, 0.436945126810344, 1.21474290382610,
                 0.295033941700225, 0.375855651783688, 1.98969760778547, 1.90903448980048,  1.91719241342961
            };


            // Null Hypothesis: Values are equal
            // Alternative    : Values are different
            double hypothesizedMean = 0;
            OneSampleHypothesis hypothesis = OneSampleHypothesis.ValueIsDifferentFromHypothesis;
            TTest target = new TTest(sample, hypothesizedMean, hypothesis);

            Assert.AreEqual(3.1254485381338246, target.Statistic);
            Assert.AreEqual(OneSampleHypothesis.ValueIsDifferentFromHypothesis, target.Hypothesis);
            Assert.AreEqual(DistributionTail.TwoTail, target.Tail);
            Assert.AreEqual(0.012210924322697769, target.PValue);
            Assert.IsTrue(target.Significant);


            // Null hypothesis: value is smaller than hypothesis
            // Alternative    : value is greater than hypothesis

            // If the null hypothesis states that the population parameter is less
            // than zero (or a constant), the z-score that rejects the null is always
            // positive and greater than the score set for the rejection condition.
            hypothesis = OneSampleHypothesis.ValueIsGreaterThanHypothesis;
            target = new TTest(sample, hypothesizedMean, hypothesis);

            // z-score is positive:
            Assert.AreEqual(3.1254485381338246, target.Statistic);
            Assert.AreEqual(OneSampleHypothesis.ValueIsGreaterThanHypothesis, target.Hypothesis); // right tail
            Assert.AreEqual(DistributionTail.OneUpper, target.Tail); // right tail
            Assert.AreEqual(0.0061054621613488846, target.PValue);
            Assert.IsTrue(target.Significant); // null should be rejected

            // Null hypothesis: value is greater than hypothesis
            // Alternative:     value is smaller than hypothesis

            // If the null hypothesis states that the population parameter is 
            // greater than zero (or a constant), the z-score that rejects the
            // null is always negative and less than the score set for the 
            // rejection condition.
            hypothesis = OneSampleHypothesis.ValueIsSmallerThanHypothesis;
            target = new TTest(sample, hypothesizedMean, hypothesis);

            // z-score is positive:
            Assert.AreEqual(3.1254485381338246, target.Statistic);
            Assert.AreEqual(OneSampleHypothesis.ValueIsSmallerThanHypothesis, target.Hypothesis); // left tail
            Assert.AreEqual(DistributionTail.OneLower, target.Tail); // left tail
            Assert.AreEqual(0.99389453783865112, target.PValue);
            Assert.IsFalse(target.Significant); // null cannot be rejected
        }

        [TestMethod()]
        public void TTestConstructorTest2()
        {

            // Consider a sample generated from a Gaussian
            // distribution with mean 0.5 and unit variance.

            double[] sample = 
            { 
                -0.849886940156521,	3.53492346633185,  1.22540422494611, 0.436945126810344, 1.21474290382610,
                 0.295033941700225, 0.375855651783688, 1.98969760778547, 1.90903448980048,	1.91719241342961
            };

            // One may rise the hypothesis that the mean of the sample is not
            // significantly different from zero. In other words, the fact that
            // this particular sample has mean 0.5 may be attributed to chance.

            double hypothesizedMean = 0;

            // Create a T-Test to check this hypothesis
            TTest test = new TTest(sample, hypothesizedMean,
                OneSampleHypothesis.ValueIsDifferentFromHypothesis);

            // Check if the mean is significantly different
            Assert.AreEqual(true, test.Significant);

            // Now, we would like to test if the sample mean is
            // significantly greater than the hypothesized zero.

            // Create a T-Test to check this hypothesis
            TTest greater = new TTest(sample, hypothesizedMean,
                OneSampleHypothesis.ValueIsGreaterThanHypothesis);

            // Check if the mean is significantly larger
            Assert.AreEqual(true, greater.Significant);

            // Now, we would like to test if the sample mean is
            // significantly smaller than the hypothesized zero.

            // Create a T-Test to check this hypothesis
            TTest smaller = new TTest(sample, hypothesizedMean,
                OneSampleHypothesis.ValueIsSmallerThanHypothesis);

            // Check if the mean is significantly smaller
            Assert.AreEqual(false, smaller.Significant);

        }

        [TestMethod()]
        public void StatisticToPValueTest()
        {
            double df = 2.2;

            double t = 1.96;
            {
                TTest target = new TTest(t, df, OneSampleHypothesis.ValueIsDifferentFromHypothesis);
                Assert.AreEqual(DistributionTail.TwoTail, target.Tail);
                double actual = target.StatisticToPValue(t);
                double expected = 0.1773;
                Assert.AreEqual(expected, actual, 1e-4);
            }

            {
                TTest target = new TTest(t, df, OneSampleHypothesis.ValueIsSmallerThanHypothesis);
                Assert.AreEqual(DistributionTail.OneLower, target.Tail);
                double actual = target.StatisticToPValue(t);
                double expected = 0.9113;
                Assert.AreEqual(expected, actual, 1e-4);
            }

            {
                TTest target = new TTest(t, df, OneSampleHypothesis.ValueIsGreaterThanHypothesis);
                Assert.AreEqual(DistributionTail.OneUpper, target.Tail);
                double actual = target.StatisticToPValue(t);
                double expected = 0.0887;
                Assert.AreEqual(expected, actual, 1e-4);
            }

            t = -1.96;
            {
                TTest target = new TTest(t, df, OneSampleHypothesis.ValueIsDifferentFromHypothesis);
                Assert.AreEqual(DistributionTail.TwoTail, target.Tail);
                double actual = target.StatisticToPValue(t);
                double expected = 0.1773;
                Assert.AreEqual(expected, actual, 1e-4);
            }

            {
                TTest target = new TTest(t, df, OneSampleHypothesis.ValueIsSmallerThanHypothesis);
                Assert.AreEqual(DistributionTail.OneLower, target.Tail);
                double actual = target.StatisticToPValue(t);
                double expected = 0.0887;
                Assert.AreEqual(expected, actual, 1e-4);
            }

            {
                TTest target = new TTest(t, df, OneSampleHypothesis.ValueIsGreaterThanHypothesis);
                Assert.AreEqual(DistributionTail.OneUpper, target.Tail);
                double actual = target.StatisticToPValue(t);
                double expected = 0.9113;
                Assert.AreEqual(expected, actual, 1e-4);
            }
        }

        [TestMethod()]
        public void PValueToStatisticTest()
        {
            double df = 2.6;
            double p = 0.05;
            double t = 0;
            {
                TTest target = new TTest(t, df, OneSampleHypothesis.ValueIsDifferentFromHypothesis);
                Assert.AreEqual(DistributionTail.TwoTail, target.Tail);
                double actual = target.PValueToStatistic(p);
                double expected = 3.4782;
                Assert.AreEqual(expected, actual, 1e-4);
            }

            {
                TTest target = new TTest(t, df, OneSampleHypothesis.ValueIsSmallerThanHypothesis);
                Assert.AreEqual(DistributionTail.OneLower, target.Tail);
                double actual = target.PValueToStatistic(p);
                double expected = -2.5086;
                Assert.AreEqual(expected, actual, 1e-4);
            }

            {
                TTest target = new TTest(t, df, OneSampleHypothesis.ValueIsGreaterThanHypothesis);
                Assert.AreEqual(DistributionTail.OneUpper, target.Tail);
                double actual = target.PValueToStatistic(p);
                double expected = 2.5086;
                Assert.AreEqual(expected, actual, 1e-4);
            }

            p = 0.95;
            {
                TTest target = new TTest(t, df, OneSampleHypothesis.ValueIsDifferentFromHypothesis);
                Assert.AreEqual(DistributionTail.TwoTail, target.Tail);
                double actual = target.PValueToStatistic(p);
                double expected = 0.0689;
                Assert.AreEqual(expected, actual, 1e-4);
            }

            {
                TTest target = new TTest(t, df, OneSampleHypothesis.ValueIsSmallerThanHypothesis);
                Assert.AreEqual(DistributionTail.OneLower, target.Tail);
                double actual = target.PValueToStatistic(p);
                double expected = 2.5086;
                Assert.AreEqual(expected, actual, 1e-4);
            }

            {
                TTest target = new TTest(t, df, OneSampleHypothesis.ValueIsGreaterThanHypothesis);
                Assert.AreEqual(DistributionTail.OneUpper, target.Tail);
                double actual = target.PValueToStatistic(p);
                double expected = -2.5086;
                Assert.AreEqual(expected, actual, 1e-4);
            }
        }

        [TestMethod()]
        public void PowerTest()
        {
            int samples = 5;
            double stdDev = 1;
            double mean = 0.2;

            {
                TTest test = new TTest(mean, stdDev: stdDev, samples: samples,
                    alternate: OneSampleHypothesis.ValueIsSmallerThanHypothesis);

                Assert.AreEqual(4, test.StatisticDistribution.DegreesOfFreedom);
                Assert.AreEqual(0.02138791, test.Analysis.Power, 1e-6);
                Assert.AreEqual(0.2, test.Analysis.Effect);
                Assert.AreEqual(5, test.Analysis.Samples);

                TTestPowerAnalysis target = (TTestPowerAnalysis)test.Analysis;
                target.Power = 0.6;
                target.ComputeSamples();

                Assert.IsTrue(Double.IsNaN(target.Samples));
                Assert.AreEqual(0.6, target.Power, 1e-6);
                Assert.AreEqual(0.2, target.Effect);
            }


            {
                TTest test = new TTest(mean, stdDev: stdDev, samples: samples,
                  alternate: OneSampleHypothesis.ValueIsGreaterThanHypothesis);

                Assert.AreEqual(4, test.StatisticDistribution.DegreesOfFreedom);

                Assert.AreEqual(0.2, test.Analysis.Effect);
                Assert.AreEqual(0.102444276600, test.Analysis.Power, 1e-6);
                Assert.AreEqual(5, test.Analysis.Samples, 1e-4);

                TTestPowerAnalysis target = (TTestPowerAnalysis)test.Analysis;
                target.Power = 0.6;
                target.ComputeSamples();

                Assert.AreEqual(91.444828012, target.Samples, 1e-6);
                Assert.AreEqual(0.6, target.Power, 1e-6);
                Assert.AreEqual(0.2, target.Effect);
            }


            {
                TTest test = new TTest(mean, stdDev: stdDev, samples: samples,
                   alternate: OneSampleHypothesis.ValueIsDifferentFromHypothesis);

                Assert.AreEqual(4, test.StatisticDistribution.DegreesOfFreedom);

                Assert.AreEqual(0.2, test.Analysis.Effect);
                Assert.AreEqual(0.06426957, test.Analysis.Power, 1e-6);
                Assert.AreEqual(5, test.Analysis.Samples, 1e-4);

                TTestPowerAnalysis target = (TTestPowerAnalysis)test.Analysis;
                target.Power = 0.6;
                target.ComputeSamples();

                Assert.AreEqual(124.3957558, target.Samples, 1e-6);
                Assert.AreEqual(0.6, target.Power, 1e-6);
                Assert.AreEqual(0.2, target.Effect);
            }
        }
    }
}
