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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Controls;
    using System.Globalization;

    [TestClass()]
    public class LogNormalDistributionTest
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
        public void ConstructorTest()
        {
            var log = new LognormalDistribution(location: 0.42, shape: 1.1);

            double mean = log.Mean;     // 2.7870954605658511
            double median = log.Median; // 1.5219615583481305
            double var = log.Variance;  // 18.28163603621158

            double cdf = log.DistributionFunction(x: 0.27); // 0.057961222885664958
            double pdf = log.ProbabilityDensityFunction(x: 0.27); // 0.39035530085982068
            double lpdf = log.LogProbabilityDensityFunction(x: 0.27); // -0.94069792674674835

            double ccdf = log.ComplementaryDistributionFunction(x: 0.27); // 0.942038777114335
            double icdf = log.InverseDistributionFunction(p: cdf); // 0.26999997937815973

            double hf = log.HazardFunction(x: 0.27); // 0.41437285846720867
            double chf = log.CumulativeHazardFunction(x: 0.27); // 0.059708840588116374

            string str = log.ToString("N2", CultureInfo.InvariantCulture); // Lognormal(x; μ = 2.79, σ = 1.10)

            Assert.AreEqual(2.7870954605658511, mean);
            Assert.AreEqual(1.5219615583481305, median, 1e-7);
            Assert.AreEqual(18.28163603621158, var);
            Assert.AreEqual(0.059708840588116374, chf);
            Assert.AreEqual(0.057961222885664958, cdf);
            Assert.AreEqual(0.39035530085982068, pdf);
            Assert.AreEqual(-0.94069792674674835, lpdf);
            Assert.AreEqual(0.41437285846720867, hf);
            Assert.AreEqual(0.942038777114335, ccdf);
            Assert.AreEqual(0.26999997937815973, icdf, 1e-7);
            Assert.AreEqual("Lognormal(x; μ = 2.79, σ = 1.10)", str);
        }

        [TestMethod()]
        public void LogNormalDistributionConstructorTest()
        {
            double location = 9.2;
            double shape = 4.4;
            LognormalDistribution target = new LognormalDistribution(location, shape);
            Assert.AreEqual(location, target.Location);
            Assert.AreEqual(shape, target.Shape);
        }

        [TestMethod()]
        public void LogNormalDistributionConstructorTest1()
        {
            double location = 4.1;
            LognormalDistribution target = new LognormalDistribution(location);
            Assert.AreEqual(location, target.Location);
        }

        [TestMethod()]
        public void LogNormalDistributionConstructorTest2()
        {
            LognormalDistribution target = new LognormalDistribution();
            Assert.AreEqual(0, target.Location);
            Assert.AreEqual(1, target.Shape);
        }

        [TestMethod()]
        public void CloneTest()
        {
            LognormalDistribution target = new LognormalDistribution(1.7, 4.2);

            LognormalDistribution clone = (LognormalDistribution)target.Clone();

            Assert.AreNotSame(target, clone);
            Assert.AreEqual(target.Entropy, clone.Entropy);
            Assert.AreEqual(target.Location, clone.Location);
            Assert.AreEqual(target.Mean, clone.Mean);
            Assert.AreEqual(target.Shape, clone.Shape);
            Assert.AreEqual(target.StandardDeviation, clone.StandardDeviation);
            Assert.AreEqual(target.Variance, clone.Variance);
        }

        [TestMethod()]
        public void DistributionFunctionTest()
        {
            LognormalDistribution target = new LognormalDistribution(1.7, 4.2);

            double x = 2.2;
            double expected = 0.414090938987083;
            double actual = target.DistributionFunction(x);

            Assert.AreEqual(expected, actual, 1e-15);
        }

        [TestMethod()]
        public void EstimateTest()
        {
            double[] observations = { 2, 2, 2, 2, 2 };

            NormalOptions options = new NormalOptions() { Regularization = 0.1 };

            LognormalDistribution actual = LognormalDistribution.Estimate(observations, options);
            Assert.AreEqual(System.Math.Log(2), actual.Location);
            Assert.AreEqual(System.Math.Sqrt(0.1), actual.Shape);
        }

        [TestMethod()]
        public void EstimateTest1()
        {
            double[] observations = 
            { 
                1.26, 0.34, 0.70, 1.75, 50.57, 1.55, 0.08, 0.42, 0.50, 3.20, 
                0.15, 0.49, 0.95, 0.24, 1.37, 0.17, 6.98, 0.10, 0.94, 0.38 
            };

            LognormalDistribution actual = LognormalDistribution.Estimate(observations);


            double expectedLocation = -0.307069523211925;
            double expectedShape = 1.51701553338489;

            Assert.AreEqual(expectedLocation, actual.Location, 1e-15);
            Assert.AreEqual(expectedShape, actual.Shape, 1e-14);
        }

        [TestMethod()]
        public void EstimateTest2()
        {
            double[] observations = { 0.04, 0.12, 1.52 };

            double[] weights = { 0.25, 0.50, 0.25 };
            LognormalDistribution actual = LognormalDistribution.Estimate(observations, weights);

            Assert.AreEqual(-1.76017314060255, actual.Location, 1e-15);
            Assert.AreEqual(1.6893403335885702, actual.Shape);
        }

        [TestMethod()]
        public void ProbabilityDensityFunctionTest()
        {
            LognormalDistribution target = new LognormalDistribution(1.7, 4.2);

            double x = 2.2;
            double expected = 0.0421705870979553;
            double actual = target.ProbabilityDensityFunction(x);

            Assert.AreEqual(expected, actual, 1e-15);
        }

        [TestMethod()]
        public void LogProbabilityDensityFunctionTest()
        {
            LognormalDistribution target = new LognormalDistribution(1.7, 4.2);

            double x = 2.2;
            double expected = System.Math.Log(0.0421705870979553);
            double actual = target.LogProbabilityDensityFunction(x);

            Assert.AreEqual(expected, actual, 1e-15);
        }

        [TestMethod()]
        public void MeanTest()
        {
            LognormalDistribution target = new LognormalDistribution(0.42, 0.56);
            double actual = target.Mean;
            Assert.AreEqual(1.7803322425420858, actual, 1e-10);
            Assert.IsFalse(Double.IsNaN(actual));
        }

        [TestMethod()]
        public void StandardTest()
        {
            LognormalDistribution actual = LognormalDistribution.Standard;
            Assert.AreEqual(0, actual.Location);
            Assert.AreEqual(1, actual.Shape);

            bool thrown = false;
            try { actual.Fit(new[] { 0.0 }); }
            catch (InvalidOperationException) { thrown = true; }
            Assert.IsTrue(thrown);
        }

        [TestMethod()]
        public void VarianceTest()
        {
            LognormalDistribution target = new LognormalDistribution(0.42, 0.56);
            double actual = target.Variance;
            Assert.AreEqual(1.1674914219333172, actual);
        }

        [TestMethod()]
        public void GenerateTest()
        {
            LognormalDistribution target = new LognormalDistribution(2, 5);

            double[] samples = target.Generate(1000000);

            var actual = LognormalDistribution.Estimate(samples);
            actual.Fit(samples);

            Assert.AreEqual(2, actual.Location, 0.01);
            Assert.AreEqual(5, actual.Shape, 0.01);
        }

        [TestMethod()]
        public void GenerateTest2()
        {
            LognormalDistribution target = new LognormalDistribution(4, 2);

            double[] samples = new double[1000000];
            for (int i = 0; i < samples.Length; i++)
                samples[i] = target.Generate();

            var actual = LognormalDistribution.Estimate(samples);
            actual.Fit(samples);

            Assert.AreEqual(4, actual.Location, 0.01);
            Assert.AreEqual(2, actual.Shape, 0.01);
        }

        [TestMethod()]
        public void MedianTest()
        {
            LognormalDistribution target = new LognormalDistribution(7, 0.6);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }
    }
}
