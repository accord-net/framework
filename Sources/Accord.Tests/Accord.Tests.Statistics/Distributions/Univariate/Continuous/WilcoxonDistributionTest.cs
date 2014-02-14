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


    [TestClass()]
    public class WilcoxonDistributionTest
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
            double[] ranks = { 1, 2, 3, 4, 5.5, 5.5, 7, 8, 9, 10, 11, 12 };

            var W = new WilcoxonDistribution(ranks);

            double mean = W.Mean;     // 39.0
            double median = W.Median; // 38.5
            double var = W.Variance;  // 162.5

            double cdf = W.DistributionFunction(w: 42); // 0.60817384423279575
            double pdf = W.ProbabilityDensityFunction(w: 42); // 0.38418508862319295
            double lpdf = W.LogProbabilityDensityFunction(w: 42); // 0.38418508862319295

            double ccdf = W.ComplementaryDistributionFunction(x: 42); // 0.39182615576720425
            double icdf = W.InverseDistributionFunction(p: cdf); // 42
            double icdf2 = W.InverseDistributionFunction(p: 0.5); // 42

            double hf = W.HazardFunction(x: 42); // 0.98049883339449373
            double chf = W.CumulativeHazardFunction(x: 42); // 0.936937017743799

            string str = W.ToString(); // "W+(x; R)"

            Assert.AreEqual(39.0, mean);
            Assert.AreEqual(38.5, median, 1e-6);
            Assert.AreEqual(162.5, var);
            Assert.AreEqual(0.936937017743799, chf);
            Assert.AreEqual(0.60817384423279575, cdf);
            Assert.AreEqual(0.38418508862319295, pdf);
            Assert.AreEqual(-0.95663084089698047, lpdf);
            Assert.AreEqual(0.98049883339449373, hf);
            Assert.AreEqual(0.39182615576720425, ccdf);
            Assert.AreEqual(42, icdf, 1e-6);
            Assert.AreEqual("W+(x; R)", str);
        }

        [TestMethod()]
        public void ConstructorTest2()
        {
            double[] ranks = { 1, 2, 3, 4, 5.5, 5.5, 7, 8, 9, 10, 11, 12 };

            var W = new WilcoxonDistribution(ranks, forceExact: true);

            double mean = W.Mean;     // 39
            double median = W.Median; // 39
            double var = W.Variance;  // 162.5

            double cdf = W.DistributionFunction(w: 42); // 0.582763671875
            double pdf = W.ProbabilityDensityFunction(w: 42); // 0.014404296875
            double lpdf = W.LogProbabilityDensityFunction(w: 42); // -4.2402287228136233

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
            Assert.AreEqual(0.582763671875, cdf);
            Assert.AreEqual(0.014404296875, pdf);
            Assert.AreEqual(-4.2402287228136233, lpdf);
            Assert.AreEqual(0.03452311293153891, hf);
            Assert.AreEqual(0.417236328125, ccdf);
            Assert.AreEqual(42, icdf, 0.05);
            Assert.AreEqual("W+(x; R)", str);
        }

        [TestMethod()]
        public void WScore()
        {
            int[] signs = { -1, -1, +1, -1, +1, +1, +1, +1, +1, +1, +1, +1 };
            double[] ranks = { 1, 2, 3, 4, 5.5, 5.5, 7, 8, 9, 10, 11, 12 };
            double[] diffs = { 0.1, 0.2, 0.3, 0.6, 1.5, 1.5, 1.8, 2.0, 2.1, 2.3, 2.6, 12.4 };

            double wm, wp, w;
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
                w = WilcoxonDistribution.WMinimum(signs, ranks);
                Assert.AreEqual(expected, w);
            }

            {
                double n = signs.Length;
                double total = wm + wp;
                double expected = (n * (n + 1)) / 2;
                Assert.AreEqual(expected, total);
            }
        }

        [TestMethod()]
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

        [TestMethod()]
        public void CumulativeTest()
        {
            // Example from https://onlinecourses.science.psu.edu/stat414/node/319

            double[] ranks = { 1, 2, 3 };

            WilcoxonDistribution target = new WilcoxonDistribution(ranks);

            double[] probabilities = { 0.0, 1 / 8.0, 1 / 8.0, 1 / 8.0, 2 / 8.0, 1 / 8.0, 1 / 8.0, 1 / 8.0 };
            double[] expected = Accord.Math.Matrix.CumulativeSum(probabilities);

            for (int i = 0; i < expected.Length; i++)
            {
                // P(W<=i)
                double actual = target.DistributionFunction(i);
                Assert.AreEqual(expected[i], actual);
            }
        }

        [TestMethod()]
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

        [TestMethod()]
        public void MedianTest()
        {
            double[] ranks = { 1, 2, 3, 7 };

            WilcoxonDistribution target = new WilcoxonDistribution(ranks);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }
    }
}
