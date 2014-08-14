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
    public class ExponentialDistributionTest
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
            ExponentialDistribution n = new ExponentialDistribution(3.42521);
            Assert.AreEqual(3.42521, n.Rate);
            Assert.AreEqual(0.29195290215782393, n.Mean);
            Assert.AreEqual(0.085236497078375897, n.Variance);
        }

        [TestMethod()]
        public void ConstructorTest2()
        {
            var exp = new ExponentialDistribution(rate: 0.42);

            double mean = exp.Mean;     // 2.3809523809523809
            double median = exp.Median; // 1.6503504299046317
            double var = exp.Variance;  // 5.6689342403628125

            double cdf = exp.DistributionFunction(x: 0.27); // 0.10720652870550407
            double pdf = exp.ProbabilityDensityFunction(x: 0.27); // 0.3749732579436883
            double lpdf = exp.LogProbabilityDensityFunction(x: 0.27); // -0.98090056770472311

            double ccdf = exp.ComplementaryDistributionFunction(x: 0.27); // 0.89279347129449593
            double icdf = exp.InverseDistributionFunction(p: cdf); // 0.27

            double hf = exp.HazardFunction(x: 0.27); // 0.42
            double chf = exp.CumulativeHazardFunction(x: 0.27); // 0.1134

            string str = exp.ToString(CultureInfo.InvariantCulture); // Exp(x; λ = 0.42)

            Assert.AreEqual(2.3809523809523809, mean);
            Assert.AreEqual(1.6503504299046317, median);
            Assert.AreEqual(5.6689342403628125, var);
            Assert.AreEqual(0.1134, chf);
            Assert.AreEqual(0.10720652870550407, cdf);
            Assert.AreEqual(0.3749732579436883, pdf);
            Assert.AreEqual(-0.98090056770472311, lpdf);
            Assert.AreEqual(0.42, hf);
            Assert.AreEqual(0.89279347129449593, ccdf);
            Assert.AreEqual(0.27, icdf);
            Assert.AreEqual("Exp(x; λ = 0.42)", str);
        }

        [TestMethod()]
        public void ProbabilityDistributionTest()
        {
            ExponentialDistribution n = new ExponentialDistribution(3);

            double[] expected = { 3, 0.149361, 0.00743626, 0.000370229, 0.0000184326 };
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
        public void ProbabilityDistributionTest2()
        {
            ExponentialDistribution n = new ExponentialDistribution(0.42);

            double[] expected = { 0.42, 0.27596, 0.181318, 0.119135, 0.0782771, 0.0514317 };
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
            ExponentialDistribution n = new ExponentialDistribution(3);

            double[] expected = { 0, 0.950213, 0.997521, 0.999877, 0.999994, 1.0 };
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
        public void MedianTest()
        {
            ExponentialDistribution target = new ExponentialDistribution(2.5);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }

        [TestMethod()]
        public void GenerateTest()
        {
            ExponentialDistribution target = new ExponentialDistribution(2.5);

            double[] samples = target.Generate(1000000);

            var actual = ExponentialDistribution.Estimate(samples);
            actual.Fit(samples);

            Assert.AreEqual(2.5, actual.Rate, 0.01);
        }

        [TestMethod()]
        public void GenerateTest2()
        {
            ExponentialDistribution target = new ExponentialDistribution(2.5);

            double[] samples = new double[1000000];
            for (int i = 0; i < samples.Length; i++)
                samples[i] = target.Generate();

            var actual = ExponentialDistribution.Estimate(samples);
            actual.Fit(samples);

            Assert.AreEqual(2.5, actual.Rate, 0.01);
        }
    }
}