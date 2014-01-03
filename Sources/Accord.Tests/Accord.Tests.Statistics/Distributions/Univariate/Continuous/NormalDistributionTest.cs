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
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Statistics.Distributions.Multivariate;
    using System.Globalization;

    [TestClass()]
    public class NormalDistributionTest
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
        public void ConstructorTest5()
        {
            var normal = new NormalDistribution(mean: 4, stdDev: 4.2);

            double mean = normal.Mean;     // 4.0
            double median = normal.Median; // 4.0
            double var = normal.Variance;  // 17.64

            double cdf = normal.DistributionFunction(x: 1.4); // 0.26794249453351904
            double pdf = normal.ProbabilityDensityFunction(x: 1.4); // 0.078423391448155175
            double lpdf = normal.LogProbabilityDensityFunction(x: 1.4); // -2.5456330358182586

            double ccdf = normal.ComplementaryDistributionFunction(x: 1.4); // 0.732057505466481
            double icdf = normal.InverseDistributionFunction(p: cdf); // 1.4

            double hf = normal.HazardFunction(x: 1.4); // 0.10712736480747137
            double chf = normal.CumulativeHazardFunction(x: 1.4); // 0.31189620872601354

            string str = normal.ToString(CultureInfo.InvariantCulture); // N(x; μ = 4, σ² = 17.64)

            Assert.AreEqual(4.0, mean);
            Assert.AreEqual(4.0, median);
            Assert.AreEqual(17.64, var);
            Assert.AreEqual(0.31189620872601354, chf);
            Assert.AreEqual(0.26794249453351904, cdf);
            Assert.AreEqual(0.078423391448155175, pdf);
            Assert.AreEqual(-2.5456330358182586, lpdf);
            Assert.AreEqual(0.10712736480747137, hf);
            Assert.AreEqual(0.732057505466481, ccdf);
            Assert.AreEqual(1.4, icdf);
            Assert.AreEqual("N(x; μ = 4, σ² = 17.64)", str);
        }

        [TestMethod()]
        public void FitTest()
        {
            double expectedMean = 1.125;
            double expectedSigma = 1.01775897605147;

            NormalDistribution target;

            target = new NormalDistribution();
            double[] observations = { 0.10, 0.40, 2.00, 2.00 };
            double[] weights = { 0.25, 0.25, 0.25, 0.25 };
            target.Fit(observations, weights);

            Assert.AreEqual(expectedMean, target.Mean);
            Assert.AreEqual(expectedSigma, target.StandardDeviation, 1e-6);


            target = new NormalDistribution();
            double[] observations2 = { 0.10, 0.10, 0.40, 2.00 };
            double[] weights2 = { 0.125, 0.125, 0.25, 0.50 };
            target.Fit(observations2, weights2);

            Assert.AreEqual(expectedMean, target.Mean);
        }

        [TestMethod()]
        public void FitTest2()
        {
            NormalDistribution target;

            target = new NormalDistribution();
            double[] observations = { 1, 1, 1, 1 };

            bool thrown = false;
            try { target.Fit(observations); }
            catch (ArgumentException) { thrown = true; }

            Assert.IsTrue(thrown);
        }

        [TestMethod()]
        public void ConstructorTest()
        {
            double mean = 7;
            double dev = 5;
            double var = 25;

            NormalDistribution target = new NormalDistribution(mean, dev);
            Assert.AreEqual(mean, target.Mean);
            Assert.AreEqual(dev, target.StandardDeviation);
            Assert.AreEqual(var, target.Variance);

            target = new NormalDistribution();
            Assert.AreEqual(0, target.Mean);
            Assert.AreEqual(1, target.StandardDeviation);
            Assert.AreEqual(1, target.Variance);

            target = new NormalDistribution(3);
            Assert.AreEqual(3, target.Mean);
            Assert.AreEqual(1, target.StandardDeviation);
            Assert.AreEqual(1, target.Variance);
        }

        [TestMethod()]
        public void ConstructorTest2()
        {
            bool thrown = false;

            try { NormalDistribution target = new NormalDistribution(4.2, 0); }
            catch (ArgumentOutOfRangeException) { thrown = true; }

            Assert.IsTrue(thrown);
        }

        [TestMethod()]
        public void ConstructorTest3()
        {
            Accord.Math.Tools.SetupGenerator(0);

            // Create a normal distribution with mean 2 and sigma 3
            var normal = new NormalDistribution(mean: 2, stdDev: 3);

            // In a normal distribution, the median and
            // the mode coincide with the mean, so

            double mean = normal.Mean;     // 2
            double mode = normal.Mode;     // 2
            double median = normal.Median; // 2

            // The variance is the square of the standard deviation
            double variance = normal.Variance; // 3² = 9

            // Let's check what is the cumulative probability of
            // a value less than 3 occurring in this distribution:
            double cdf = normal.DistributionFunction(3); // 0.63055

            // Finally, let's generate 1000 samples from this distribution
            // and check if they have the specified mean and standard dev.

            double[] samples = normal.Generate(1000);

            double sampleMean = samples.Mean();             // 1.92
            double sampleDev = samples.StandardDeviation(); // 3.00

            Assert.AreEqual(2, mean);
            Assert.AreEqual(2, mode);
            Assert.AreEqual(2, median);

            Assert.AreEqual(9, variance);
            Assert.AreEqual(1000, samples.Length);
            Assert.AreEqual(1.9245, sampleMean, 1e-4);
            Assert.AreEqual(3.0008, sampleDev, 1e-4);
        }

        [TestMethod()]
        public void ProbabilityDensityFunctionTest()
        {
            double x = 3;
            double mean = 7;
            double dev = 5;

            NormalDistribution target = new NormalDistribution(mean, dev);

            double expected = 0.0579383105522966;
            double actual = target.ProbabilityDensityFunction(x);

            Assert.IsFalse(double.IsNaN(actual));
            Assert.AreEqual(expected, actual, 1e-15);
        }

        [TestMethod()]
        public void ToMultivariateTest()
        {
            double x = 3;
            double mean = 7;
            double dev = 5;

            NormalDistribution target = new NormalDistribution(mean, dev);

            MultivariateNormalDistribution multi = 
                target.ToMultivariateDistribution();

            Assert.AreEqual(target.Mean, multi.Mean[0]);
            Assert.AreEqual(target.Variance, multi.Covariance[0, 0]);
            Assert.AreEqual(target.Variance, multi.Variance[0]);

            double expected = 0.0579383105522966;
            double actual = multi.ProbabilityDensityFunction(x);

            Assert.IsFalse(double.IsNaN(actual));
            Assert.AreEqual(expected, actual, 1e-15);
        }

        [TestMethod()]
        public void ProbabilityDensityFunctionTest2()
        {
            double expected, actual;

            // Test for small variance
            NormalDistribution target = new NormalDistribution(4.2, double.Epsilon);

            expected = 0;
            actual = target.ProbabilityDensityFunction(0);
            Assert.AreEqual(expected, actual);

            expected = double.PositiveInfinity;
            actual = target.ProbabilityDensityFunction(4.2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void LogProbabilityDensityFunctionTest()
        {
            double x = 3;
            double mean = 7;
            double dev = 5;

            NormalDistribution target = new NormalDistribution(mean, dev);

            double expected = System.Math.Log(0.0579383105522966);
            double actual = target.LogProbabilityDensityFunction(x);

            Assert.IsFalse(double.IsNaN(actual));
            Assert.AreEqual(expected, actual, 1e-15);
        }


        [TestMethod()]
        public void DistributionFunctionTest()
        {
            double x = 3;
            double mean = 7;
            double dev = 5;

            NormalDistribution target = new NormalDistribution(mean, dev);

            double expected = 0.211855398583397;
            double actual = target.DistributionFunction(x);

            Assert.IsFalse(double.IsNaN(actual));
            Assert.AreEqual(expected, actual, 1e-15);
        }

        [TestMethod()]
        public void DistributionFunctionTest3()
        {
            double expected, actual;

            // Test small variance
            NormalDistribution target = new NormalDistribution(1.0, double.Epsilon);

            expected = 0;
            actual = target.DistributionFunction(0);
            Assert.AreEqual(expected, actual);

            expected = 0.5;
            actual = target.DistributionFunction(1.0);
            Assert.AreEqual(expected, actual);

            expected = 1.0;
            actual = target.DistributionFunction(1.0 + 1e-15);
            Assert.AreEqual(expected, actual);

            expected = 0.0;
            actual = target.DistributionFunction(1.0 - 1e-15);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void InverseDistributionFunctionTest()
        {
            double[] expected =
            {
                Double.NegativeInfinity, -4.38252, -2.53481, -1.20248,
                -0.0640578, 1.0, 2.06406, 3.20248, 4.53481, 6.38252,
                Double.PositiveInfinity
            };

            NormalDistribution target = new NormalDistribution(1.0, 4.2);

            for (int i = 0; i < expected.Length; i++)
            {
                double x = i / 10.0;
                double actual = target.InverseDistributionFunction(x);
                Assert.AreEqual(expected[i], actual, 1e-5);
                Assert.IsFalse(Double.IsNaN(actual));
            }
        }

        [TestMethod()]
        public void ZScoreTest()
        {
            double x = 5;
            double mean = 3;
            double dev = 6;

            NormalDistribution target = new NormalDistribution(mean, dev);

            double expected = (x - 3) / 6;
            double actual = target.ZScore(x);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CloneTest()
        {
            NormalDistribution target = new NormalDistribution(0.5, 4.2);

            NormalDistribution clone = (NormalDistribution)target.Clone();

            Assert.AreNotSame(target, clone);
            Assert.AreEqual(target.Entropy, clone.Entropy);
            Assert.AreEqual(target.Mean, clone.Mean);
            Assert.AreEqual(target.StandardDeviation, clone.StandardDeviation);
            Assert.AreEqual(target.Variance, clone.Variance);
        }


        [TestMethod()]
        public void GenerateTest()
        {
            NormalDistribution target = new NormalDistribution(2, 5);

            double[] samples = target.Generate(1000000);

            var actual = NormalDistribution.Estimate(samples);
            actual.Fit(samples);

            Assert.AreEqual(2, actual.Mean, 0.01);
            Assert.AreEqual(5, actual.StandardDeviation, 0.01);
        }

        [TestMethod()]
        public void GenerateTest2()
        {
            NormalDistribution target = new NormalDistribution(4, 2);

            double[] samples = new double[1000000];
            for (int i = 0; i < samples.Length; i++)
                samples[i] = target.Generate();

            var actual = NormalDistribution.Estimate(samples);
            actual.Fit(samples);

            Assert.AreEqual(4, actual.Mean, 0.01);
            Assert.AreEqual(2, actual.StandardDeviation, 0.01);
        }

        [TestMethod()]
        public void MedianTest()
        {
            NormalDistribution target = new NormalDistribution(0.4, 2.2);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }
    }
}
