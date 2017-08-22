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

    using System;
    using Accord.Statistics;
    using Accord.Statistics.Testing;
    using Accord.Statistics.Testing.Power;
    using NUnit.Framework;

    [TestFixture]
    public class TTestPowerAnalysisTest
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



        [Test]
        public void TTestPowerAnalysisConstructorTest4()
        {
            // Example from http://www.ats.ucla.edu/stat/stata/dae/t_test_power2.htm,
            // tested against G*Power results

            double meanA = 0;
            double meanB = 10;

            double sdA = 15;
            double sdB = 17;

            double varA = sdA * sdA;
            double varB = sdB * sdB;


            {
                var priori = TwoSampleTTestPowerAnalysis.GetSampleSize(10,
                    variance1: varA, variance2: varB, power: 0.8);

                Assert.AreEqual(41, Math.Truncate(priori.Samples1));
                Assert.AreEqual(41, Math.Truncate(priori.Samples2));
            }

            {
                TwoSampleTTest test = new TwoSampleTTest(
                    meanA, varA, 30,
                    meanB, varB, 30);

                Assert.AreEqual(0.661222, test.Analysis.Power, 1e-6);
            }

            {
                TwoSampleTTest test = new TwoSampleTTest(
                    meanA, varA, 20,
                    meanB, varB, 40);

                Assert.AreEqual(0.6102516, test.Analysis.Power, 1e-6);
            }


            {
                var priori = TwoSampleTTestPowerAnalysis.GetSampleSize(10,
                variance1: varA, variance2: varB, power: 0.8, alpha: 0.07);

                Assert.AreEqual(37, Math.Truncate(priori.Samples1));
                Assert.AreEqual(37, Math.Truncate(priori.Samples2));
            }
        }

        [Test]
        public void TTestPowerAnalysisConstructorTest()
        {
            // Let's say we have two samples, and we would like to know whether those
            // samples have the same mean. For this, we can perform a two sample T-Test:
            double[] A = { 5.0, 6.0, 7.9, 6.95, 5.3, 10.0, 7.48, 9.4, 7.6, 8.0, 6.22 };
            double[] B = { 5.0, 1.6, 5.75, 5.80, 2.9, 8.88, 4.56, 2.4, 5.0, 10.0 };

            // Perform the test, assuming the samples have unequal variances
            var test = new TwoSampleTTest(A, B, assumeEqualVariances: false);

            double df = test.DegreesOfFreedom;   // d.f. = 14.351
            double t = test.Statistic;           // t    = 2.14
            double p = test.PValue;              // p    = 0.04999
            bool significant = test.Significant; // true

            // The test gave us an indication that the samples may
            // indeed have come from different distributions (whose
            // mean value is actually distinct from each other).

            // Now, we would like to perform an _a posteriori_ analysis of the 
            // test. When doing an a posteriori analysis, we can not change some
            // characteristics of the test (because it has been already done), 
            // but we can measure some important features that may indicate 
            // whether the test is trustworthy or not.

            // One of the first things would be to check for the test's power.
            // A test's power is 1 minus the probability of rejecting the null
            // hypothesis when the null hypothesis is actually false. It is
            // the other side of the coin when we consider that the P-value
            // is the probability of rejecting the null hypothesis when the
            // null hypothesis is actually true.

            // Ideally, this should be a high value:
            double power = test.Analysis.Power; // 0.5376260

            // Check how much effect we are trying to detect
            double effect = test.Analysis.Effect; // 0.94566

            // With this power, that is the minimal difference we can spot?
            double sigma = Math.Sqrt(test.Variance);
            double thres = test.Analysis.Effect * sigma; // 2.0700909090909

            // This means that, using our test, the smallest difference that
            // we could detect with some confidence would be something around
            // 2 standard deviations. If we would like to say the samples are
            // different when they are less than 2 std. dev. apart, we would
            // need to do repeat our experiment differently.


            // Another way to create the power analysis is to pass 
            // the test to the t-test power analysis constructor:

            // Create an a posteriori analysis of the experiment
            var analysis = new TwoSampleTTestPowerAnalysis(test);

            // When creating a power analysis, we have three things we can
            // change. We can always freely configure two of those things
            // and then ask the analysis to give us the third.

            // Those are:
            double e = analysis.Effect;       // the test's minimum detectable effect size (0.94566)
            double n = analysis.TotalSamples; // the number of samples in the test (21 or (11 + 10))
            double b = analysis.Power;        // the probability of committing a type-2 error (0.53)

            // Let's say we would like to create a test with 80% power.
            analysis.Power = 0.8;
            analysis.ComputeEffect(); // what effect could we detect?

            double detectableEffect = analysis.Effect; // we would detect a difference of 1.290514


            // However, to achieve this 80%, we would need to redo our experiment
            // more carefully. Assuming we are going to redo our experiment, we will
            // have more freedom about what we can change and what we can not. For 
            // better addressing those points, we will create an a priori analysis 
            // of the experiment:

            // We would like to know how many samples we would need to gather in
            // order to achieve a 80% power test which can detect an effect size
            // of one standard deviation:
            //
            analysis = TwoSampleTTestPowerAnalysis.GetSampleSize
            (
                variance1: A.Variance(),
                variance2: B.Variance(),
                delta: 1.0, // the minimum detectable difference we want
                power: 0.8  // the test power that we want
            );

            // How many samples would we need in order to see the effect we need?
            int n1 = (int)Math.Ceiling(analysis.Samples1); // 77
            int n2 = (int)Math.Ceiling(analysis.Samples2); // 77

            // According to our power analysis, we would need at least 77 
            // observations in each sample in order to see the effect we
            // need with the required 80% power.

            Assert.AreEqual(1.2905141186795861, detectableEffect);
            Assert.AreEqual(0.45682188621283815, analysis.Effect, 1e-6);
            Assert.AreEqual(2.0700909090909088, thres);
            Assert.AreEqual(0.53762605885988846, power);

            Assert.AreEqual(77, n1);
            Assert.AreEqual(77, n1);

            double meanA = A.Mean();
            double meanB = B.Mean();

            double varA = A.Variance();
            double varB = B.Variance();

            double sdA = A.StandardDeviation();
            double sdB = B.StandardDeviation();

            double sigma2 = Math.Sqrt((varA + varB) / 2.0);
            Assert.AreEqual(sigma2, sigma);

            Assert.AreEqual(7.259, meanA, 1e-3);
            Assert.AreEqual(5.189, meanB, 1e-3);

            Assert.AreEqual(2.492289, varA, 1e-6);
            Assert.AreEqual(7.091476, varB, 1e-6);

            Assert.AreEqual(1.5786985, sdA, 1e-6);
            Assert.AreEqual(2.6629826, sdB, 1e-6);

            Assert.AreEqual(14.351, df, 1e-3);
            Assert.AreEqual(2.14, t, 1e-3);
            Assert.AreEqual(0.04999, p, 1e-5);
            Assert.AreEqual(0.00013662, test.Confidence.Min, 1e-6);
            Assert.AreEqual(4.14004519, test.Confidence.Max, 1e-6);
            Assert.AreEqual(4.7918828787878791, test.Variance);
            Assert.IsTrue(test.Significant);

            Assert.AreEqual(0.5376260, test.Analysis.Power, 1e-6);
            Assert.AreEqual(0.9456628, test.Analysis.Effect, 1e-6);
            Assert.AreEqual(2.070090, test.Analysis.Effect * sigma, 1e-6);

            test = new TwoSampleTTest(A, B, assumeEqualVariances: true);

            Assert.AreEqual(19, test.DegreesOfFreedom, 1e-3);
            Assert.AreEqual(2.1921894, test.Statistic, 1e-3);
            Assert.AreEqual(0.0410, test.PValue, 1e-4);
            Assert.AreEqual(0.09364214, test.Confidence.Min, 1e-6);
            Assert.AreEqual(4.04653967, test.Confidence.Max, 1e-6);
            Assert.IsTrue(test.Significant);

            // Check the actual power of the test...
            Assert.AreEqual(0.5376260, test.Analysis.Power, 1e-6);

            // Check how much effect we are trying to detect
            Assert.AreEqual(0.9456628, test.Analysis.Effect, 1e-6);

            // So, what is the minimal difference we can detect?
            Assert.AreEqual(2.070090, test.Analysis.Effect * sigma, 1e-6);

            // Create an a posteriori analysis of the experiment
            analysis = new TwoSampleTTestPowerAnalysis(test);

            analysis.Power = 0.8;     // With 80% power, how much
            analysis.ComputeEffect(); // effect could we really detect?

            Assert.AreEqual(1.29051411, analysis.Effect, 1e-6);

            // Create an a priori power analysis so we can determine the sample
            // size needed to detect at least a difference of 2 points in the
            // student mean grades with at least 80% power:

            analysis = TwoSampleTTestPowerAnalysis.GetSampleSize(1,
              variance1: varA, variance2: varB, power: 0.8);

            Assert.AreEqual(0.4568219, analysis.Effect, 1e-6);

            // Check how many samples we would need to detect this effect with 80% power

            Assert.AreEqual(77, Math.Ceiling(analysis.Samples1));
            Assert.AreEqual(77, Math.Ceiling(analysis.Samples2));
        }


        [Test]
        public void TTestPowerAnalysisConstructorTest2()
        {
            // Example from R's graphical manual
            // http://rgm2.lab.nig.ac.jp/RGM2/func.php?rd_id=pwr:pwr.t2n.test

            double effectSize = 0.6;
            int n1 = 90;
            int n2 = 60;

            TwoSampleTTestPowerAnalysis target;
            double actual, expected;

            target = new TwoSampleTTestPowerAnalysis(TwoSampleHypothesis.FirstValueIsGreaterThanSecond)
            {
                Effect = effectSize,
                Samples1 = n1,
                Samples2 = n2,
            };

            target.ComputePower();

            expected = 0.9737262;
            actual = target.Power;
            Assert.AreEqual(expected, actual, 1e-6);


            target = new TwoSampleTTestPowerAnalysis(TwoSampleHypothesis.FirstValueIsSmallerThanSecond)
            {
                Effect = effectSize,
                Samples1 = n1,
                Samples2 = n2,
            };

            target.ComputePower();

            expected = 0.0;
            actual = target.Power;
            Assert.AreEqual(expected, actual, 1e-6);



            target = new TwoSampleTTestPowerAnalysis(TwoSampleHypothesis.ValuesAreDifferent)
            {
                Effect = effectSize,
                Samples1 = n1,
                Samples2 = n2,
            };

            target.ComputePower();

            expected = 0.9470154;
            actual = target.Power;
            Assert.AreEqual(expected, actual, 1e-6);
        }

        [Test]
        public void TTestPowerAnalysisConstructorTest3()
        {
            // Examples from R's graphical manual
            // http://rgm2.lab.nig.ac.jp/RGM2/func.php?rd_id=pwr:pwr.t.test

            double actual, expected;

            {
                var target = new TTestPowerAnalysis(OneSampleHypothesis.ValueIsDifferentFromHypothesis)
                {
                    Effect = 0.2,
                    Samples = 60,
                    Size = 0.10,
                };

                target.ComputePower();

                expected = 0.4555818;
                actual = target.Power;
                Assert.AreEqual(expected, actual, 1e-5);
            }

            {
                var target = new TwoSampleTTestPowerAnalysis(TwoSampleHypothesis.ValuesAreDifferent)
                {
                    Effect = 2 / 2.8,
                    Samples1 = 30,
                    Samples2 = 30,
                };

                target.ComputePower();

                expected = 0.7764889;
                actual = target.Power;
                Assert.AreEqual(expected, actual, 1e-6);
            }

            {
                var target = new TwoSampleTTestPowerAnalysis(TwoSampleHypothesis.FirstValueIsGreaterThanSecond)
                {
                    Effect = 0.3,
                    Power = 0.75,
                };

                target.ComputeSamples();

                expected = 120.2232016;
                actual = target.Samples1;
                Assert.AreEqual(expected, actual, 1e-6);
                Assert.AreEqual(target.Samples1, target.Samples2);
            }
        }

        [Test]
        public void TTestPowerAnalysisConstructorTest7()
        {
            // When creating a power analysis, we have three things we can
            // change. We can always freely configure two of those things
            // and then ask the analysis to give us the third.

            var analysis = new TTestPowerAnalysis(OneSampleHypothesis.ValueIsDifferentFromHypothesis);

            // Those are:
            double e = analysis.Effect;   // the test's minimum detectable effect size
            double n = analysis.Samples;  // the number of samples in the test
            double p = analysis.Power;    // the probability of committing a type-2 error

            // Let's set the desired effect size and the 
            // number of samples so we can get the power

            analysis.Effect = 0.2; // we would like to detect at least 0.2 std. dev. apart
            analysis.Samples = 60; // we would like to use at most 60 samples
            analysis.ComputePower(); // what will be the power of this test?

            double power = analysis.Power; // The power is going to be 0.33 (or 33%)

            // Let's set the desired power and the number 
            // of samples so we can get the effect size

            analysis.Power = 0.8;  // we would like to create a test with 80% power
            analysis.Samples = 60; // we would like to use at most 60 samples
            analysis.ComputeEffect(); // what would be the minimum effect size we can detect?

            double effect = analysis.Effect; // The effect will be 0.36 standard deviations.

            // Let's set the desired power and the effect
            // size so we can get the number of samples

            analysis.Power = 0.8;  // we would like to create a test with 80% power
            analysis.Effect = 0.2; // we would like to detect at least 0.2 std. dev. apart
            analysis.ComputeSamples(); 

            double samples = analysis.Samples; // We would need around 199 samples.

            Assert.AreEqual(198.15082094251142, samples, 1e-10);
            Assert.AreEqual(0.36770431608203374, effect);
            Assert.AreEqual(0.33167864622935495, power);
        }

        [Test]
        public void TTestPowerAnalysisConstructorTest5()
        {
            var analysis = new TwoSampleTTestPowerAnalysis(TwoSampleHypothesis.ValuesAreDifferent);

            analysis.Effect = 5;
            analysis.Power = 0.95;
            analysis.ComputeSize();

            Assert.AreEqual(0.12207248549844732, analysis.Size, 1e-10);
            Assert.AreEqual(2, analysis.Samples1);
            Assert.AreEqual(2, analysis.Samples2);
        }

    }
}
