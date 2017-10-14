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
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;
    using System;
    using System.Globalization;

    [TestFixture]
    public class GeneralizedNormalDistributionTest
    {


        [Test]
        public void ConstructorTest1()
        {
            var normal = new GeneralizedNormalDistribution(location: 1, scale: 5, shape: 0.42);

            double mean = normal.Mean;     // 1
            double median = normal.Median; // 1
            double mode = normal.Mode;     // 1
            double var = normal.Variance;  // 19200.781700666659

            double cdf = normal.DistributionFunction(x: 1.4); // 0.51076148867681703
            double pdf = normal.ProbabilityDensityFunction(x: 1.4); // 0.024215092283124507
            double lpdf = normal.LogProbabilityDensityFunction(x: 1.4); // -3.7207791921441378

            double ccdf = normal.ComplementaryDistributionFunction(x: 1.4); // 0.48923851132318297
            double icdf = normal.InverseDistributionFunction(p: cdf); // 1.4000000149740108

            double hf = normal.HazardFunction(x: 1.4); // 0.049495474543966168
            double chf = normal.CumulativeHazardFunction(x: 1.4); // 0.7149051552030572

            string str = normal.ToString(CultureInfo.InvariantCulture); // GGD(x; μ = 1, α = 5, β = 0.42)

            Assert.AreEqual(1, mean);
            Assert.AreEqual(1, median);
            Assert.AreEqual(1, mode);
            Assert.AreEqual(19200.781700666659, var);
            Assert.AreEqual(0.7149051552030572, chf);
            Assert.AreEqual(0.51076148867681703, cdf);
            Assert.AreEqual(0.024215092283124507, pdf);
            Assert.AreEqual(-3.7207791921441378, lpdf);
            Assert.AreEqual(0.049495474543966168, hf);
            Assert.AreEqual(0.48923851132318297, ccdf);
            Assert.AreEqual(1.4000000149740108, icdf);
            Assert.AreEqual("GGD(x; μ = 1, α = 5, β = 0.42)", str);

            var range1 = normal.GetRange(0.95);
            var range2 = normal.GetRange(0.99);
            var range3 = normal.GetRange(0.01);

            Assert.AreEqual(-173.60070095277663, range1.Min);
            Assert.AreEqual(175.60070093821949, range1.Max);
            Assert.AreEqual(-428.92857248354409, range2.Min);
            Assert.AreEqual(430.92857248354375, range2.Max);
            Assert.AreEqual(-428.92857248354403, range3.Min);
            Assert.AreEqual(430.92857248354375, range3.Max);

            Assert.AreEqual(double.NegativeInfinity, normal.Support.Min);
            Assert.AreEqual(double.PositiveInfinity, normal.Support.Max);

            Assert.AreEqual(normal.InverseDistributionFunction(0), normal.Support.Min);
            Assert.AreEqual(normal.InverseDistributionFunction(1), normal.Support.Max);
        }

        [Test]
        public void NormalTest()
        {
            var target = GeneralizedNormalDistribution.Normal(mean: 0.42, stdDev: 4.2);
            var normal = new NormalDistribution(mean: 0.42, stdDev: 4.2);

            test(target, normal);
        }

        [Test]
        public void NormalTest2()
        {
            var target = GeneralizedNormalDistribution.Normal(mean: 0.0, stdDev: 2 / Constants.Sqrt2);
            var normal = new NormalDistribution(mean: 0.0, stdDev: 2 / Constants.Sqrt2);

            test(target, normal);

            var support = target.Support;
            Assert.AreEqual(normal.Support.Min, support.Min);
            Assert.AreEqual(normal.Support.Max, support.Max);

            for (double i = 0.01; i <= 1; i += 0.01)
            {
                var actual = normal.GetRange(i);
                var expected = normal.GetRange(i);

                Assert.AreEqual(expected.Min, actual.Min);
                Assert.AreEqual(expected.Max, actual.Max);
            }
        }

        [Test]
        public void SupportTest1()
        {
            var target = new GeneralizedNormalDistribution(0.0, 10, 2);

            var range = target.Support;

            Assert.AreEqual(double.NegativeInfinity, range.Min);
            Assert.AreEqual(double.PositiveInfinity, range.Max);
        }

        [Test]
        public void LaplaceTest()
        {
            var target = GeneralizedNormalDistribution.Laplace(location: 0.42, scale: 4.2);
            var normal = new LaplaceDistribution(location: 0.42, scale: 4.2);

            test(target, normal);

            var support = target.Support;
            Assert.AreEqual(normal.Support.Min, support.Min);
            Assert.AreEqual(normal.Support.Max, support.Max);

            for (double i = 0.01; i <= 1.0; i += 0.01)
            {
                var actual = normal.GetRange(i);
                var expected = normal.GetRange(i);

                Assert.AreEqual(expected.Min, actual.Min);
                Assert.AreEqual(expected.Max, actual.Max);
            }
        }

        private static void test(GeneralizedNormalDistribution target, UnivariateContinuousDistribution normal)
        {

            Assert.AreEqual(normal.Mean, target.Mean);
            Assert.AreEqual(normal.Variance, target.Variance, 1e-10);
            Assert.AreEqual(normal.Entropy, target.Entropy, 1e-10);
            Assert.AreEqual(normal.StandardDeviation, target.StandardDeviation, 1e-10);
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

        [Test]
        public void DistributionFunctionTest1()
        {
            var target = GeneralizedNormalDistribution.Normal(mean: 0.42, stdDev: 4.2);
            var normal = new NormalDistribution(mean: 0.42, stdDev: 4.2);

            for (double x = -10; x < 10; x += 0.0001)
            {
                double actual = target.DistributionFunction(x);
                double expected = normal.DistributionFunction(x);
                Assert.AreEqual(expected, actual, 1e-10);
                Assert.IsFalse(Double.IsNaN(actual));
            }
        }

        [Test]
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
