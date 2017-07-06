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
    using Accord.Statistics;
    using Accord.Statistics.Distributions.Univariate;
    using Math;
    using NUnit.Framework;
    using System;


    [TestFixture]
    public class WilcoxonDistributionTest
    {

        [Test]
        public void ConstructorTest()
        {
            double[] ranks = { 1, 2, 3, 4, 5.5, 5.5, 7, 8, 9, 10, 11, 12 };

            var W = new WilcoxonDistribution(ranks);

            double mean = W.Mean;     // 39.0
            double median = W.Median; // 38.5
            double var = W.Variance;  // 162.5
            double mode = W.Mode;     // 39.0

            double cdf = W.DistributionFunction(x: 42); // 0.60817384423279575
            double pdf = W.ProbabilityDensityFunction(x: 42); // 0.38418508862319295
            double lpdf = W.LogProbabilityDensityFunction(x: 42); // 0.38418508862319295

            double ccdf = W.ComplementaryDistributionFunction(x: 42); // 0.39182615576720425
            double icdf = W.InverseDistributionFunction(p: cdf); // 42
            double icdf2 = W.InverseDistributionFunction(p: 0.5); // 42

            double hf = W.HazardFunction(x: 42); // 0.98049883339449373
            double chf = W.CumulativeHazardFunction(x: 42); // 0.936937017743799

            string str = W.ToString(); // "W+(x; R)"

            Assert.AreEqual(39.0, mean);
            Assert.AreEqual(39, median, 1e-6);
            Assert.AreEqual(39.0, mode, 1e-8);
            Assert.AreEqual(162.5, var);
            Assert.AreEqual(0.87410248360375287, chf, 1e-8);
            Assert.AreEqual(0.59716796875, cdf, 1e-8);
            Assert.AreEqual(0.014404296875, pdf, 1e-8);
            Assert.AreEqual(-4.2402287228136233, lpdf, 1e-8);
            Assert.AreEqual(0.03452311293153891, hf, 1e-8);
            Assert.AreEqual(0.417236328125, ccdf, 1e-8);
            Assert.AreEqual(42.5, icdf, 0.05);
            Assert.AreEqual("W+(x; R)", str);

            Assert.AreEqual(0, W.Support.Min);
            Assert.AreEqual(double.PositiveInfinity, W.Support.Max);

            Assert.AreEqual(W.InverseDistributionFunction(0), W.Support.Min);
            Assert.AreEqual(W.InverseDistributionFunction(1), W.Support.Max);
        }

        [Test]
        public void ConstructorTest2()
        {
            double[] ranks = { 1, 2, 3, 4, 5.5, 5.5, 7, 8, 9, 10, 11, 12 };

            var W = new WilcoxonDistribution(ranks, exact: true);

            double mean = W.Mean;     // 39
            double median = W.Median; // 39
            double var = W.Variance;  // 162.5

            double cdf = W.DistributionFunction(x: 42); // 0.582763671875
            double pdf = W.ProbabilityDensityFunction(x: 42); // 0.014404296875
            double lpdf = W.LogProbabilityDensityFunction(x: 42); // -4.2402287228136233

            double ccdf = W.ComplementaryDistributionFunction(x: 42); // 0.417236328125
            double icdf = W.InverseDistributionFunction(p: cdf); // 41.965447500067114
            double icdf2 = W.InverseDistributionFunction(p: 0.5); // 39.000000487005138

            double hf = W.HazardFunction(x: 42); // 0.03452311293153891
            double chf = W.CumulativeHazardFunction(x: 42); // 0.87410248360375287

            string str = W.ToString(); // "W+(x; R)"

            Assert.AreEqual(39.0, mean);
            Assert.AreEqual(39.0, median, 1e-6);
            Assert.AreEqual(162.5, var);
            Assert.AreEqual(0.87410248360375287, chf);
            Assert.AreEqual(0.59716796875, cdf, 1e-5);
            Assert.AreEqual(0.014404296875, pdf, 1e-8);
            Assert.AreEqual(-4.2402287228136233, lpdf);
            Assert.AreEqual(0.03452311293153891, hf);
            Assert.AreEqual(0.417236328125, ccdf);
            Assert.AreEqual(42.5, icdf, 0.05);
            Assert.AreEqual("W+(x; R)", str);

            var range1 = W.GetRange(0.95);
            var range2 = W.GetRange(0.99);
            var range3 = W.GetRange(0.01);

            Assert.AreEqual(17.999999736111114, range1.Min, 1e-6);
            Assert.AreEqual(60.000000315408002, range1.Max, 1e-6);
            Assert.AreEqual(10.000000351098127, range2.Min, 1e-6);
            Assert.AreEqual(67.99999981945885, range2.Max, 1e-6);
            Assert.AreEqual(10.000000351098119, range3.Min, 1e-6);
            Assert.AreEqual(67.99999981945885, range3.Max, 1e-6);
        }

        [Test]
        public void WScore()
        {
            int[] signs = { -1, -1, +1, -1, +1, +1, +1, +1, +1, +1, +1, +1 };
            double[] ranks = { 1, 2, 3, 4, 5.5, 5.5, 7, 8, 9, 10, 11, 12 };
            double[] diffs = { 0.1, 0.2, 0.3, 0.6, 1.5, 1.5, 1.8, 2.0, 2.1, 2.3, 2.6, 12.4 };

            double wm, wp, u;
            {
                double expected = 7;
                wm = WilcoxonDistribution.WNegative(signs, ranks);
                Assert.AreEqual(expected, wm);
            }

            {
                double expected = 71;
                wp = WilcoxonDistribution.WPositive(signs, ranks);
                Assert.AreEqual(expected, wp);
            }

            {
                double expected = 7;
                u = WilcoxonDistribution.WMinimum(signs, ranks);
                Assert.AreEqual(expected, u);
            }

            {
                double n = signs.Length;
                double total = wm + wp;
                double expected = (n * (n + 1)) / 2;
                Assert.AreEqual(expected, total);
            }
        }

        [Test]
        public void ProbabilityTest()
        {
            // Example from https://onlinecourses.science.psu.edu/stat414/node/319

            double[] ranks = { 1, 2, 3 };

            WilcoxonDistribution target = new WilcoxonDistribution(ranks);

            double[] expected = { 1 / 8.0, 1 / 8.0, 1 / 8.0, 2 / 8.0, 1 / 8.0, 1 / 8.0, 1 / 8.0 };

            for (int i = 0; i < expected.Length; i++)
            {
                // P(W=i)
                double actual = target.ProbabilityDensityFunction(i);
                Assert.AreEqual(expected[i], actual);
            }
        }

        [Test]
        public void CumulativeTest()
        {
            // Example from https://onlinecourses.science.psu.edu/stat414/node/319

            double[] ranks = { 1, 2, 3 };

            WilcoxonDistribution target = new WilcoxonDistribution(ranks);

            double[] probabilities = { 1 / 8.0, 1 / 8.0, 1 / 8.0, 2 / 8.0, 1 / 8.0, 1 / 8.0, 1 / 8.0 };
            double[] expected = Accord.Math.Matrix.CumulativeSum(probabilities);

            for (int i = 0; i < expected.Length; i++)
            {
                // P(W<=i)
                double actual = target.DistributionFunction(i);
                Assert.AreEqual(expected[i], actual);
            }
        }

        [Test]
        public void CumulativeExactTest()
        {
            // example from https://onlinecourses.science.psu.edu/stat414/node/319

            double[] ranks =
            {
                22, 2, 13, 24, 16, 15, 25, 10, 9, 11, 5,
                17, 12, 20, 14, 30, 8, 6, 26, 19, 29, 27, 3, 28,
                7, 21, 23, 1, 18, 4
            };

            WilcoxonDistribution target = new WilcoxonDistribution(ranks);

            Assert.AreEqual(232.5, target.Mean);
            Assert.AreEqual(2363.75, target.Variance);
            Assert.AreEqual(Math.Sqrt(2363.75), target.StandardDeviation);

            double actual = target.DistributionFunction(200);
            double expected = 0.2546;
            Assert.AreEqual(expected, actual, 1e-2);

            double inv = target.InverseDistributionFunction(actual);

            Assert.AreEqual(200, inv);
        }

        [Test]
        public void MedianTest()
        {
            double[] ranks = { 1, 2, 3, 7 };

            WilcoxonDistribution target = new WilcoxonDistribution(ranks);
            Assert.IsTrue(target.Exact);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }

        [Test]
        public void MedianTest_approximation()
        {
            double[] ranks = { 1, 2, 3, 7 };

            WilcoxonDistribution target = new WilcoxonDistribution(ranks, exact: false);
            Assert.IsFalse(target.Exact);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }

        [Test]
        public void ApproximationTest()
        {
            double[] m = Matrix.Magic(5).Reshape().Get(0, 20);

            double[] samples = m.Rank();

            var exact = new WilcoxonDistribution(samples, exact: true);
            var approx = new WilcoxonDistribution(samples, exact: false);

            var nd = NormalDistribution.Estimate(exact.Table);
            Assert.AreEqual(nd.Mean, exact.Mean, 1e-10);
            Assert.AreEqual(nd.Variance, exact.Variance, 1e-3);

            foreach (double x in Vector.Range(0, 100))
            {
                double e = approx.DistributionFunction(x);
                double a = exact.DistributionFunction(x);

                if (e > 0.25)
                    Assert.AreEqual(e, a, 0.1);
                else Assert.AreEqual(e, a, 0.01);
            }

            foreach (double x in Vector.Range(0, 100))
            {
                double e = approx.ComplementaryDistributionFunction(x);
                double a = exact.ComplementaryDistributionFunction(x);

                if (e > 0.25)
                    Assert.AreEqual(e, a, 0.1);
                else Assert.AreEqual(e, a, 0.01);
            }
        }

        [Test]
        public void RangeTest()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new WilcoxonDistribution(0));
            Assert.AreEqual(0.5, new WilcoxonDistribution(1).Mean);
            Assert.AreEqual(1.5, new WilcoxonDistribution(2).Mean);
        }

        [Test]
        public void icdf()
        {
            var dist = new WilcoxonDistribution(1);

            double[] percentiles = Vector.Range(0.0, 1.0, stepSize: 0.1);
            for (int i = 0; i < percentiles.Length; i++)
            {
                double x = percentiles[i];
                double icdf = dist.InverseDistributionFunction(x);
                double cdf = dist.DistributionFunction(icdf);
                double iicdf = dist.InverseDistributionFunction(cdf);
                double iiicdf = dist.DistributionFunction(iicdf);

                Assert.AreEqual(iicdf, icdf, 1e-5);
                Assert.AreEqual(x, cdf, 1e-5);
                Assert.AreEqual(iiicdf, cdf, 1e-5);
            }
        }
    }
}
