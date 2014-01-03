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
    using System;
    using AForge;
    using Accord.Statistics.Testing.Power;

    [TestClass()]
    public class TwoSampleZTestTest
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
        public void TwoSampleZTestConstructorTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            double[] samples1 = new Accord.Statistics.Distributions.Univariate
                .NormalDistribution(29.8, 4.0).Generate(200);

            double[] samples2 = new Accord.Statistics.Distributions.Univariate
                .NormalDistribution(34.7, 5.0).Generate(250);

            TwoSampleZTest actual = new TwoSampleZTest(samples1, samples2);

            double mean1 = Accord.Statistics.Tools.Mean(samples1);
            double mean2 = Accord.Statistics.Tools.Mean(samples2);

            double var1 = Accord.Statistics.Tools.Variance(samples1);
            double var2 = Accord.Statistics.Tools.Variance(samples2);

            int n1 = samples1.Length;
            int n2 = samples2.Length;

            TwoSampleZTest expected = new TwoSampleZTest(mean1, var1, n1, mean2, var2, n2);


            Assert.AreEqual(expected.EstimatedValue1, actual.EstimatedValue1);
            Assert.AreEqual(expected.EstimatedValue2, actual.EstimatedValue2);


            Assert.AreEqual(expected.StandardError, actual.StandardError);
            Assert.AreEqual(expected.Statistic, actual.Statistic);

            Assert.IsTrue(actual.Significant);
        }

        [TestMethod()]
        public void TwoSampleZTestConstructorTest1()
        {
            // Example from http://www.stat.ucla.edu/~cochran/stat10/winter/lectures/lect21.html

            double mean1 = 9.78;
            double var1 = System.Math.Pow(4.05, 2);
            int samples1 = 900;

            double mean2 = 15.10;
            double var2 = System.Math.Pow(4.28, 2);
            int samples2 = 1000;

            TwoSampleZTest target = new TwoSampleZTest(mean1, var1, samples1, mean2, var2, samples2);

            Assert.AreEqual(mean1, target.EstimatedValue1);
            Assert.AreEqual(mean2, target.EstimatedValue2);

            Assert.AreEqual(0.19, target.StandardError, 0.005);
            Assert.AreEqual(-28, target.Statistic, 0.5);

            Assert.IsTrue(target.Significant);
        }

        [TestMethod()]
        public void TwoSampleZTestConstructorTest2()
        {
            // Example from http://www.stat.purdue.edu/~tlzhang/stat511/chapter9_1.pdf

            double mean1 = 29.8;
            double var1 = System.Math.Pow(4.0, 2);
            int samples1 = 20;

            double mean2 = 34.7;
            double var2 = System.Math.Pow(5.0, 2);
            int samples2 = 25;

            TwoSampleZTest target = new TwoSampleZTest(
                mean1, var1, samples1,
                mean2, var2, samples2);

            Assert.AreEqual(mean1, target.EstimatedValue1);
            Assert.AreEqual(mean2, target.EstimatedValue2);

            Assert.AreEqual(-3.66, target.Statistic, 0.01);

            var range = target.GetConfidenceInterval(0.99);
            Assert.AreEqual(-8.36, range.Min, 0.01);
            Assert.AreEqual(-1.44, range.Max, 0.01);

            Assert.IsTrue(target.Significant);

            target.Size = 0.01;

            Assert.IsTrue(target.Significant);
        }

        [TestMethod()]
        public void TwoSampleZTestConstructorTest3()
        {
            // Example from Larser & Farber, Elementary Statistics: Picturing the world

            /*
             * A high school math teacher claims that students in her class
             * will score higher on the math portion of the ACT then students
             * in a colleague’s math class.  The mean ACT math score for 49
             * students in her class is 22.1 and the standard deviation is 4.8.
             * The mean ACT math score for 44 of the colleague’s students is
             * 19.8 and the standard deviation is 5.4.  At a = 0.10, can the 
             * teacher’s claim be supported?
             */

            double mean1 = 22.1;
            double var1 = System.Math.Pow(4.8, 2);
            int samples1 = 49;

            double mean2 = 19.8;
            double var2 = System.Math.Pow(5.4, 2);
            int samples2 = 44;

            {
                TwoSampleZTest target = new TwoSampleZTest(
                    mean1, var1, samples1,
                    mean2, var2, samples2,
                    alternate: TwoSampleHypothesis.FirstValueIsGreaterThanSecond);

                target.Size = 0.10;

                Assert.AreEqual(mean1, target.EstimatedValue1);
                Assert.AreEqual(mean2, target.EstimatedValue2);

                Assert.AreEqual(1.0644, target.StandardError, 0.0001);
                Assert.AreEqual(2.161, target.Statistic, 0.001);

                Assert.IsTrue(target.Significant);
            }
            {
                TwoSampleZTest target = new TwoSampleZTest(
                    mean2, var2, samples2,
                    mean1, var1, samples1,
                    alternate: TwoSampleHypothesis.FirstValueIsSmallerThanSecond);

                target.Size = 0.10;

                Assert.AreEqual(mean2, target.EstimatedValue1);
                Assert.AreEqual(mean1, target.EstimatedValue2);

                Assert.AreEqual(1.0644, target.StandardError, 0.0001);
                Assert.AreEqual(-2.161, target.Statistic, 0.001);

                Assert.IsTrue(target.Significant);
            }
        }

        [TestMethod()]
        public void PowerTest1()
        {
            // Example from http://www.statpower.net/Content/310/Print%20Version%20--%20Power%20for%20the%202-Sample%20Z-Statistic.pdf

            double mean1 = 0.5;
            double mean2 = 0.0;
            double var1 = 0.5;
            double var2 = 0.5;
            int samples = 25;

            TwoSampleZTest test = new TwoSampleZTest(
                mean1, var1, samples,
                mean2, var2, samples);

            TwoSampleZTestPowerAnalysis pa = (TwoSampleZTestPowerAnalysis)test.Analysis;

            Assert.AreEqual(0.43, pa.Power, 0.01);

            pa.Power = 0.8;
            pa.ComputeSamples();

            double expectedSamples = 63;
            double actualSamples = Math.Ceiling(2 * pa.Samples1);

            Assert.AreEqual(expectedSamples, actualSamples);
        }

        [TestMethod]
        public void SampleSizeTest1()
        {
            // Example from http://udel.edu/~mcdonald/statttest.html

            double mean1 = 3.2;
            double mean2 = 0;
            double var1 = System.Math.Pow(4.0, 2);
            double var2 = System.Math.Pow(4.3, 2);
            double alpha = 0.05;
            double power = 0.80;

            TwoSampleZTest test = new TwoSampleZTest(mean1, var1, 10, mean2, var2, 10,
                alternate: TwoSampleHypothesis.ValuesAreDifferent);

            var target = (TwoSampleZTestPowerAnalysis)test.Analysis.Clone();

            target.Power = power;
            target.Size = alpha;

            target.ComputeSamples(1);

            double actual = Math.Ceiling(target.Samples1);

            double expected = 27;

            Assert.AreEqual(expected, actual, 1e-3);
        }
    }
}
