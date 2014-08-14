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
    using Accord.Statistics.Moving;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Statistics;
    using Accord.Statistics.Distributions.Univariate;
    using System.Globalization;

    [TestClass()]
    public class RayleighDistributionTest
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
            RayleighDistribution n = new RayleighDistribution(0.807602);
            Assert.AreEqual(1.0121790039242726, n.Mean);
            Assert.AreEqual(0.27993564482286737, n.Variance);
        }

        [TestMethod()]
        public void ConstructorTest2()
        {
            var rayleigh = new RayleighDistribution(sigma: 0.42);

            double mean = rayleigh.Mean;     // 0.52639193767251
            double median = rayleigh.Median; // 0.49451220943852386
            double var = rayleigh.Variance;  // 0.075711527953380237

            double cdf = rayleigh.DistributionFunction(x: 1.4); // 0.99613407986052716
            double pdf = rayleigh.ProbabilityDensityFunction(x: 1.4); // 0.030681905868831811
            double lpdf = rayleigh.LogProbabilityDensityFunction(x: 1.4); // -3.4840821835248961

            double ccdf = rayleigh.ComplementaryDistributionFunction(x: 1.4); // 0.0038659201394728449
            double icdf = rayleigh.InverseDistributionFunction(p: cdf); // 1.4000000080222026

            double hf = rayleigh.HazardFunction(x: 1.4); // 7.9365079365078612
            double chf = rayleigh.CumulativeHazardFunction(x: 1.4); // 5.5555555555555456

            string str = rayleigh.ToString(CultureInfo.InvariantCulture); // Rayleigh(x; σ = 0.42)

            Assert.AreEqual(0.52639193767251, mean);
            Assert.AreEqual(0.49451220943852386, median, 1e-8);
            Assert.AreEqual(0.075711527953380237, var);
            Assert.AreEqual(5.5555555555555456, chf);
            Assert.AreEqual(0.99613407986052716, cdf);
            Assert.AreEqual(0.030681905868831811, pdf);
            Assert.AreEqual(-3.4840821835248961, lpdf);
            Assert.AreEqual(7.9365079365078612, hf);
            Assert.AreEqual(0.0038659201394728449, ccdf);
            Assert.AreEqual(1.40000000, icdf, 1e-8);
            Assert.AreEqual("Rayleigh(x; σ = 0.42)", str);
        }

        [TestMethod()]
        public void ProbabilityDistributionTest()
        {
            RayleighDistribution n = new RayleighDistribution(0.807602);

            double[] expected = { 0, 0.712311, 0.142855, 0.00463779, 0.0000288872 };
            double[] actual = new double[expected.Length];

            for (int i = 0; i < actual.Length; i++)
                actual[i] = n.ProbabilityDensityFunction(i);

            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 1e-5);
                Assert.IsFalse(double.IsNaN(actual[i]));
            }
        }

        [TestMethod()]
        public void CumulativeDistributionTest()
        {
            RayleighDistribution n = new RayleighDistribution(0.807602);

            double[] expected = { 0, 0.535415, 0.953414, 0.998992, 0.999995 };
            double[] actual = new double[expected.Length];

            for (int i = 0; i < actual.Length; i++)
                actual[i] = n.DistributionFunction(i);

            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 1e-6);
                Assert.IsFalse(double.IsNaN(actual[i]));
            }
        }


        [TestMethod()]
        public void GenerateTest()
        {
            RayleighDistribution target = new RayleighDistribution(2.5);

            double[] samples = target.Generate(1000000);

            var actual = RayleighDistribution.Estimate(samples);
            actual.Fit(samples);

            Assert.AreEqual(2, actual.Mean, 0.01);
            Assert.AreEqual(5, actual.Variance, 0.01);
        }

        [TestMethod()]
        public void GenerateTest2()
        {
            RayleighDistribution target = new RayleighDistribution(4.2);

            double[] samples = new double[1000000];
            for (int i = 0; i < samples.Length; i++)
                samples[i] = target.Generate();

            var actual = RayleighDistribution.Estimate(samples);
            actual.Fit(samples);

            Assert.AreEqual(4, actual.Mean, 0.01);
            Assert.AreEqual(2, actual.Variance, 0.01);
        }

        [TestMethod()]
        public void MedianTest()
        {
            RayleighDistribution target = new RayleighDistribution(0.52);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }
    }
}