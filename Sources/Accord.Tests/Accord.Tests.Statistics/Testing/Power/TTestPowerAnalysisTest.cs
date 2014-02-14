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

    using Accord.Statistics.Testing.Power;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Statistics.Testing;
    using Accord.Statistics;
    using Accord.Math;

    [TestClass()]
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



        [TestMethod()]
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

        [TestMethod()]
        public void TTestPowerAnalysisConstructorTest()
        {
            // Declare two samples
            double[] A = { 5.0, 6.0, 7.9, 6.95, 5.3, 10.0, 7.48, 9.4, 7.6, 8.0, 6.22 };
            double[] B = { 5.0, 1.6, 5.75, 5.80, 2.9, 8.88, 4.56, 2.4, 5.0, 10.0 };

            double meanA = A.Mean();
            double meanB = B.Mean();

            double varA = A.Variance();
            double varB = B.Variance();

            double sdA = A.StandardDeviation();
            double sdB = B.StandardDeviation();

            double sigma = Math.Sqrt((varA + varB) / 2.0);

            Assert.AreEqual(7.259, meanA, 1e-3);
            Assert.AreEqual(5.189, meanB, 1e-3);

            Assert.AreEqual(2.492289, varA, 1e-6);
            Assert.AreEqual(7.091476, varB, 1e-6);

            Assert.AreEqual(1.5786985, sdA, 1e-6);
            Assert.AreEqual(2.6629826, sdB, 1e-6);

            // Perform a hypothesis test
            TwoSampleTTest test = new TwoSampleTTest(A, B, assumeEqualVariances: false);

            Assert.AreEqual(14.351, test.DegreesOfFreedom, 1e-3);
            Assert.AreEqual(2.14, test.Statistic, 1e-3);
            Assert.AreEqual(0.04999, test.PValue, 1e-5);
            Assert.AreEqual(0.00013662, test.Confidence.Min, 1e-6);
            Assert.AreEqual(4.14004519, test.Confidence.Max, 1e-6);
            Assert.IsTrue(test.Significant);

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
            var analysis = new TwoSampleTTestPowerAnalysis(test);

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

        [TestMethod()]
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

        [TestMethod()]
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

        [TestMethod()]
        public void TTestPowerAnalysisConstructorTest5()
        {
            var analysis = new TwoSampleTTestPowerAnalysis(TwoSampleHypothesis.ValuesAreDifferent);

            analysis.Effect = 5;
            analysis.Power = 0.95;
            analysis.ComputeSize();

            Assert.AreEqual(0.12207248549844732, analysis.Size);
            Assert.AreEqual(2, analysis.Samples1);
            Assert.AreEqual(2, analysis.Samples2);
        }

    }
}
