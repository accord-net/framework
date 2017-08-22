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
    using System.Globalization;
    using Accord.Math;
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;
    using Accord.Statistics.Distributions.Reflection;

    [TestFixture]
    public class ParetoDistributionTest
    {

        [Test]
        public void ConstructorTest()
        {
            var pareto = new ParetoDistribution(scale: 0.42, shape: 3);

            double mean = pareto.Mean;     // 0.63
            double median = pareto.Median; // 0.52916684095584676
            double var = pareto.Variance;  // 0.13229999999999997
            double mode = pareto.Mode;     // 0.42

            double cdf = pareto.DistributionFunction(x: 1.4); // 0.973
            double pdf = pareto.ProbabilityDensityFunction(x: 1.4); // 0.057857142857142857
            double lpdf = pareto.LogProbabilityDensityFunction(x: 1.4); // -2.8497783609309111

            double ccdf = pareto.ComplementaryDistributionFunction(x: 1.4); // 0.027000000000000024
            double icdf = pareto.InverseDistributionFunction(p: cdf); // 1.4000000446580794

            double hf = pareto.HazardFunction(x: 1.4); // 2.142857142857141
            double chf = pareto.CumulativeHazardFunction(x: 1.4); // 3.6119184129778072

            string str = pareto.ToString(CultureInfo.InvariantCulture); // Pareto(x; xm = 0.42, α = 3)

            Assert.AreEqual(0.63, mean);
            Assert.AreEqual(0.52916684095584676, median);
            Assert.AreEqual(0.13229999999999997, var);
            Assert.AreEqual(0.42, mode, 1e-10);
            Assert.AreEqual(3.6119184129778072, chf);
            Assert.AreEqual(0.973, cdf);
            Assert.AreEqual(0.057857142857142857, pdf);
            Assert.AreEqual(-2.8497783609309111, lpdf);
            Assert.AreEqual(2.142857142857141, hf);
            Assert.AreEqual(0.027000000000000024, ccdf);
            Assert.AreEqual(1.40, icdf, 1e-7);
            Assert.AreEqual("Pareto(x; xm = 0.42, α = 3)", str);

            var range1 = pareto.GetRange(0.95);
            var range2 = pareto.GetRange(0.99);
            var range3 = pareto.GetRange(0.01);

            Assert.AreEqual(0.42724297039643383, range1.Min, 1e-8);
            Assert.AreEqual(1.1400554029735852, range1.Max, 1e-8);
            Assert.AreEqual(0.42140940651872005, range2.Min, 1e-8);
            Assert.AreEqual(1.9494675279346789, range2.Max, 1e-8);
            Assert.AreEqual(0.42140940651872005, range3.Min, 1e-8);
            Assert.AreEqual(1.9494675279346789, range3.Max, 1e-8);

            Assert.AreEqual(0.42, pareto.Support.Min);
            Assert.AreEqual(double.PositiveInfinity, pareto.Support.Max);

            Assert.AreEqual(pareto.InverseDistributionFunction(0), pareto.Support.Min);
            Assert.AreEqual(pareto.InverseDistributionFunction(1), pareto.Support.Max);
        }

        [Test]
        public void ParetoDistributionConstructorTest()
        {
            double expected, actual;

            {
                ParetoDistribution target = new ParetoDistribution(3.1, 4.42);
                actual = target.ProbabilityDensityFunction(-1);
                expected = 0.0;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));

                actual = target.ProbabilityDensityFunction(0);
                expected = 0.0;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));

                actual = target.ProbabilityDensityFunction(3.09);
                expected = 0.0;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));

                actual = target.ProbabilityDensityFunction(3.1);
                expected = 1.4258064;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));

                actual = target.ProbabilityDensityFunction(3.2);
                expected = 1.20040576;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));

                actual = target.ProbabilityDensityFunction(5.8);
                expected = 0.0478037;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));

                actual = target.ProbabilityDensityFunction(10);
                expected = 0.00249598;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));
            }
        }

        [Test]
        public void MedianTest()
        {
            var target = new ParetoDistribution(scale: 7.12, shape: 2);

            double median = target.Median;
            Assert.AreEqual(10.069200564096438, median, 1e-10);

            Assert.AreEqual(median, target.InverseDistributionFunction(0.5), 1e-6);
        }

        [Test]
        public void MedianTest2()
        {
            var target = UnivariateDistributionInfo.CreateInstance<ParetoDistribution>();

            Assert.AreEqual(1, target.Alpha);
            Assert.AreEqual(1, target.Scale);

            double median = target.Median;
            Assert.AreEqual(2, target.Median);

            Assert.AreEqual(median, target.InverseDistributionFunction(0.5), 1e-6);
        }

        [Test]
        public void FitTest()
        {
            var source = new ParetoDistribution(scale: 7.12, shape: 2);
            var sample = new double[10000];
            var step = 1.0 / sample.Length;

            for (var i = 0; i < sample.Length; i++)
                sample[i] = source.InverseDistributionFunction(i * step);

            var target = new ParetoDistribution();
            target.Fit(sample);

            Assert.AreEqual(7.12, target.Scale, 1e-6);
            Assert.AreEqual(2.0, target.Alpha, 1e-2);
        }

        [Test]
        public void FitWeights()
        {
            var source = new ParetoDistribution(scale: 7.12, shape: 2);
            var sample = new double[10000];
            var step = 1.0 / sample.Length;

            for (var i = 0; i < sample.Length; i++)
                sample[i] = source.InverseDistributionFunction(i * step);

            var target = new ParetoDistribution();

            double[] weights = Matrix.Vector(sample.Length, 1.0 / sample.Length);

            target.Fit(sample, weights);

            Assert.AreEqual(7.12, target.Scale, 1e-6);
            Assert.AreEqual(2.0, target.Alpha, 1e-2);
        }

    }
}
