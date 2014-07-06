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
    public class GeneralizedNormalDistributionTest
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
        public void ConstructorTest1()
        {
            var normal = new GeneralizedNormalDistribution(location: 1, scale: 5, shape: 0.42);

            double mean = normal.Mean;     // 1
            double median = normal.Median; // 1
            double mode = normal.Mode;     // 1
            double var = normal.Variance;  // 19200.781700666659

            double cdf = normal.DistributionFunction(x: 1.4); // 0.50877105447218995
            double pdf = normal.ProbabilityDensityFunction(x: 1.4); // 0.024215092283124507
            double lpdf = normal.LogProbabilityDensityFunction(x: 1.4); // -3.7207791921441378

            double ccdf = normal.ComplementaryDistributionFunction(x: 1.4); // 0.49122894552781005
            double icdf = normal.InverseDistributionFunction(p: cdf); // 1.4000000149740104

            double hf = normal.HazardFunction(x: 1.4); // 0.049294921448706883
            double chf = normal.CumulativeHazardFunction(x: 1.4); // 0.71084497569360638

            string str = normal.ToString(CultureInfo.InvariantCulture); // GGD(x; μ = 1, α = 5, β = 0.42)

            Assert.AreEqual(1, mean);
            Assert.AreEqual(1, median);
            Assert.AreEqual(1, mode);
            Assert.AreEqual(19200.781700666659, var);
            Assert.AreEqual(0.71084497569360638, chf);
            Assert.AreEqual(0.50877105447218995, cdf);
            Assert.AreEqual(0.024215092283124507, pdf);
            Assert.AreEqual(-3.7207791921441378, lpdf);
            Assert.AreEqual(0.049294921448706883, hf);
            Assert.AreEqual(0.49122894552781005, ccdf);
            Assert.AreEqual(1.4000000149740104, icdf);
            Assert.AreEqual("GGD(x; μ = 1, α = 5, β = 0.42)", str);
        }

        [TestMethod()]
        public void NormalTest()
        {
            var target = GeneralizedNormalDistribution.Normal(mean: 0.42, stdDev: 4.2);
            var normal = new NormalDistribution(mean: 0.42, stdDev: 4.2);

            test(target, normal);
        }

        [TestMethod()]
        public void LaplaceTest()
        {
            var target = GeneralizedNormalDistribution.Laplace(location: 0.42, scale: 4.2);
            var normal = new LaplaceDistribution(location: 0.42, scale: 4.2);

            test(target, normal);
        }

        private static void test(GeneralizedNormalDistribution target, UnivariateContinuousDistribution normal)
        {

            Assert.AreEqual(normal.Mean, target.Mean);
            Assert.AreEqual(normal.Variance, target.Variance, 1e-10);
            Assert.AreEqual(normal.Entropy, target.Entropy,1e-10);
            Assert.AreEqual(normal.StandardDeviation, target.StandardDeviation);
            Assert.AreEqual(normal.Mode, target.Mode);
            Assert.AreEqual(normal.Median, target.Median);

            for (double x = -10; x < 10; x += 0.0001)
            {
                double actual = target.ProbabilityDensityFunction(x);
                double expected = normal.ProbabilityDensityFunction(x);
                Assert.AreEqual(expected, actual, 1e-15);
                Assert.IsFalse(Double.IsNaN(actual));
            }

            for (double x = -10; x < 10; x += 0.0001)
            {
                double actual = target.LogProbabilityDensityFunction(x);
                double expected = normal.LogProbabilityDensityFunction(x);
                Assert.AreEqual(expected, actual, 1e-10);
                Assert.IsFalse(Double.IsNaN(actual));
            }
        }

        [TestMethod()]
        [Ignore]
        public void DistributionFunctionTest1()
        {
            var target = GeneralizedNormalDistribution.Normal(mean: 0.42, stdDev: 4.2);
            var normal = new NormalDistribution(mean: 0.42, stdDev: 4.2);

            for (double x = -10; x < 10; x += 0.0001)
            {
                double actual = target.DistributionFunction(x);
                double expected = normal.DistributionFunction(x);
                Assert.AreEqual(expected, actual, 1e-15);
                Assert.IsFalse(Double.IsNaN(actual));
            }
        }

        [TestMethod()]
        public void DistributionFunctionTest2()
        {
            var target = GeneralizedNormalDistribution.Laplace(location: 0.42, scale: 4.2);
            var normal = new LaplaceDistribution(location: 0.42, scale: 4.2);

            for (double x = -10; x < 10; x += 0.0001)
            {
                double actual = target.DistributionFunction(x);
                double expected = normal.DistributionFunction(x);
                Assert.AreEqual(expected, actual, 1e-15);
                Assert.IsFalse(Double.IsNaN(actual));
            }
        }

    }
}
