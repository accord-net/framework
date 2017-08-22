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
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;
    using System;
    using Accord.Math;
    using Accord.Statistics;

    [TestFixture]
    public class MannWhitneyDistributionTest
    {

        [Test]
        public void ConstructorTest()
        {
            #region doc_create
            double[] ranks = { 1, 2, 3, 4, 5 };

            var mannWhitney = new MannWhitneyDistribution(ranks, n1: 2, n2: 3);

            double mean = mannWhitney.Mean;     // 2.7870954605658511
            double median = mannWhitney.Median; // 1.5219615583481305
            double var = mannWhitney.Variance;  // 18.28163603621158
            double mode = mannWhitney.Mode;

            double cdf = mannWhitney.DistributionFunction(x: 4); // 0.6
            double pdf = mannWhitney.ProbabilityDensityFunction(x: 4); // 0.2
            double lpdf = mannWhitney.LogProbabilityDensityFunction(x: 4); // -1.6094379124341005

            double ccdf = mannWhitney.ComplementaryDistributionFunction(x: 4); // 0.4
            double icdf = mannWhitney.InverseDistributionFunction(p: cdf); // 3.6666666666666661

            double hf = mannWhitney.HazardFunction(x: 4); // 0.5
            double chf = mannWhitney.CumulativeHazardFunction(x: 4); // 0.916290731874155

            string str = mannWhitney.ToString(); // MannWhitney(u; n1 = 2, n2 = 3)
            #endregion


            Assert.AreEqual(3.0, mean);
            Assert.AreEqual(3.0, mode);
            Assert.AreEqual(3.0000006357828775, median, 1e-5);
            Assert.AreEqual(3.0, var, 1e-5);
            Assert.AreEqual(0.916290731874155, chf, 1e-8);
            Assert.AreEqual(0.8, cdf, 1e-8);
            Assert.AreEqual(0.2, pdf, 1e-8);
            Assert.AreEqual(-1.6094379124341005, lpdf, 1e-8);
            Assert.AreEqual(0.5, hf, 1e-8);
            Assert.AreEqual(0.4, ccdf, 1e-8);
            Assert.AreEqual(4, icdf, 1e-8);
            Assert.AreEqual("MannWhitney(u; n1 = 2, n2 = 3)", str);

            var range1 = mannWhitney.GetRange(0.95);
            var range2 = mannWhitney.GetRange(0.99);
            var range3 = mannWhitney.GetRange(0.01);

            Assert.AreEqual(0, range1.Min, 1e-5);
            Assert.AreEqual(6, range1.Max, 1e-5);
            Assert.AreEqual(-8.5830688476562492E-07, range2.Min, 1e-5);
            Assert.AreEqual(6.0000005561746477, range2.Max, 1e-4);
            Assert.AreEqual(-8.58306884765625E-07, range3.Min, 1e-5);
            Assert.AreEqual(6.0000005561746477, range3.Max, 1e-5);
        }

        [Test]
        public void ProbabilityDensityFunctionTest()
        {
            double[] ranks = { 1, 2, 3, 4, 5 };

            int n1 = 2;
            int n2 = 3;
            int N = n1 + n2;

            Assert.AreEqual(N, ranks.Length);


            var target = new MannWhitneyDistribution(ranks, n1, n2);

            // Number of possible combinations is 5!/(3!2!) = 10.
            int nc = (int)Special.Binomial(5, 3);

            Assert.AreEqual(10, nc);

            double[] expected = { 0.1, 0.1, 0.2, 0.2, 0.2, 0.1, 0.1, 0.0, 0.0 };
            double sum = 0;

            for (int i = 0; i < expected.Length; i++)
            {
                // P(U=i)
                double actual = target.ProbabilityDensityFunction(i);
                Assert.AreEqual(expected[i], actual);
                sum += actual;
            }

            Assert.AreEqual(1, sum);
        }

        [Test]
        public void DistributionFunctionTest()
        {
            double[] ranks = { 1, 2, 3, 4, 5 };

            int n1 = 2;
            int n2 = 3;
            int N = n1 + n2;

            Assert.AreEqual(N, ranks.Length);


            var target = new MannWhitneyDistribution(ranks, n1, n2);

            // Number of possible combinations is 5!/(3!2!) = 10.
            int nc = (int)Special.Binomial(5, 3);

            Assert.AreEqual(10, nc);

            double[] expected = { 0.1, 0.1, 0.2, 0.2, 0.2, 0.1, 0.1, 0.0, 0.0 };

            expected = expected.CumulativeSum();

            for (int i = 0; i < expected.Length; i++)
            {
                // P(U<=i)
                double actual = target.DistributionFunction(i);
                Assert.AreEqual(expected[i], actual, 1e-10);
            }


            expected = new double[] { 0.0, 0.0, 0.1, 0.1, 0.2, 0.2, 0.2, 0.1, 0.1 };

            expected = expected.CumulativeSum().Reversed();

            for (int i = 0; i < expected.Length; i++)
            {
                // P(U>i)
                double actual = target.ComplementaryDistributionFunction(i);
                Assert.AreEqual(expected[i], actual, 1e-10);
            }
        }

        [Test]
        public void MedianTest_approximation()
        {
            double[] ranks = { 1, 1, 2, 3, 4, 7, 5 };

            foreach (bool exact in new bool[] { false, true })
            {
                for (int i = 1; i < ranks.Length; i++)
                {
                    int n1 = i;
                    int n2 = ranks.Length - i;
                    Assert.AreEqual(n1 + n2, ranks.Length);
                    var target = new MannWhitneyDistribution(ranks, n1, n2, exact: exact);
                    Assert.AreEqual(exact, target.Exact);
                    Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
                }
            }
        }

        [Test]
        public void RCompatibilityTest1()
        {
            // Example from https://onlinecourses.science.psu.edu/stat464/node/36
            double[] a = { 37, 49, 55, 57 };
            double[] b = { 23, 31, 46 };
            double[] c = a.Concatenate(b);

            double[] rc = c.Rank();
            double[] ra = rc.Get(0, a.Length);
            double[] rb = rc.Get(a.Length, 0);

            Assert.IsTrue(rc.IsEqual(new[] { 3, 5, 6, 7, 1, 2, 4 }));
            Assert.IsTrue(ra.IsEqual(new[] { 3, 5, 6, 7 }));
            Assert.IsTrue(rb.IsEqual(new[] { 1, 2, 4 }));

            double w1 = MannWhitneyDistribution.MannWhitneyU(ra);
            double w2 = MannWhitneyDistribution.MannWhitneyU(rb);

            var target = new MannWhitneyDistribution(rc, a.Length, b.Length);
            Assert.IsTrue(target.Exact);
            double p1 = target.DistributionFunction(w1);
            double p2 = target.DistributionFunction(w2);
            Assert.AreEqual(0.057142857142857141, p1, 1e-5);
            Assert.AreEqual(0.97142857142857142, p2, 1e-5);

            target = new MannWhitneyDistribution(rc, b.Length, a.Length);
            Assert.IsTrue(target.Exact);
            p1 = target.DistributionFunction(w1);
            p2 = target.DistributionFunction(w2);
            Assert.AreEqual(0.97142857142857142, p1, 1e-5);
            Assert.AreEqual(0.057142857142857141, p2, 1e-5);



            target = new MannWhitneyDistribution(rc, a.Length, b.Length, exact: false);
            Assert.IsFalse(target.Exact);
            p1 = target.DistributionFunction(w1);
            p2 = target.DistributionFunction(w2);
            Assert.AreEqual(0.038549935871770913, p1, 1e-5);
            Assert.AreEqual(0.96145006412822909, p2, 1e-5);

            target = new MannWhitneyDistribution(rc, b.Length, a.Length, exact: false);
            Assert.IsFalse(target.Exact);
            p1 = target.DistributionFunction(w1);
            p2 = target.DistributionFunction(w2);
            Assert.AreEqual(0.96145006412822909, p1, 1e-5);
            Assert.AreEqual(0.038549935871770913, p2, 1e-5);
        }

        [Test, Ignore("This test can only be run in 64-bits")]
        public void ApproximationTest()
        {
            int t = 14;

            double[] m = Matrix.Magic(6).Reshape().Get(0, t * 2);

            double[] samples = m.Rank();

            double[] rank1 = samples.Get(0, t);
            double[] rank2 = samples.Get(t, 0);

            var exact = new MannWhitneyDistribution(rank1, rank2, exact: true);
            var approx = new MannWhitneyDistribution(rank1, rank2, exact: false);

            var nd = NormalDistribution.Estimate(exact.Table);
            Assert.AreEqual(nd.Mean, exact.Mean, 1e-10);
            Assert.AreEqual(nd.Variance, exact.Variance, 2e-5);

            foreach (double x in Vector.Range(0, 100))
            {
                double e = approx.DistributionFunction(x);
                double a = exact.DistributionFunction(x);

                if (e > 0.23)
                    Assert.AreEqual(e, a, 0.1);
                else Assert.AreEqual(e, a, 0.01);
            }

            foreach (double x in Vector.Range(0, 100))
            {
                double e = approx.ComplementaryDistributionFunction(x);
                double a = exact.ComplementaryDistributionFunction(x);

                if (e > 0.23)
                    Assert.AreEqual(e, a, 0.1);
                else Assert.AreEqual(e, a, 0.01);
            }
        }

        [Test]
        public void cdf()
        {
            var dist = new MannWhitneyDistribution(n1: 1, n2: 1);

            Assert.AreEqual(0, dist.DistributionFunction(Double.NegativeInfinity));
            Assert.AreEqual(0, dist.DistributionFunction(-1e100));
            Assert.AreEqual(0, dist.DistributionFunction(-10));
            Assert.AreEqual(0.022750131948179209d, dist.DistributionFunction(-0.5));
            Assert.AreEqual(0.15865525393145707d, dist.DistributionFunction(0.0));
            Assert.AreEqual(0.5, dist.DistributionFunction(+0.5));
            Assert.AreEqual(1, dist.DistributionFunction(+10));
            Assert.AreEqual(1, dist.DistributionFunction(+1e100));
            Assert.AreEqual(1, dist.DistributionFunction(Double.PositiveInfinity));
        }

        [Test]
        public void cdf_n2_greater_than_n1()
        {
            var dist = new MannWhitneyDistribution(n1: 1, n2: 10);

            Assert.AreEqual(0, dist.DistributionFunction(Double.NegativeInfinity));
            Assert.AreEqual(0, dist.DistributionFunction(-1e100));
            Assert.AreEqual(1.050717977957305E-06d, dist.DistributionFunction(-10));
            Assert.AreEqual(0.040995160500191474d, dist.DistributionFunction(-0.5));
            Assert.AreEqual(0.056923149003329065d, dist.DistributionFunction(0.0));
            Assert.AreEqual(0.077364461742689294d, dist.DistributionFunction(+0.5));
            Assert.AreEqual(0.94307685099667093d, dist.DistributionFunction(+10));
            Assert.AreEqual(1, dist.DistributionFunction(+1e100));
            Assert.AreEqual(1, dist.DistributionFunction(Double.PositiveInfinity));
        }

        [Test]
        public void cdf_n1_greater_than_n2()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(n1: 0, n2: 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(n1: 1, n2: 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(n1: 0, n2: 1));
        }

        [Test]
        public void icdf()
        {
            var dist = new MannWhitneyDistribution(n1: 1, n2: 1);

            Assert.AreEqual(Double.NegativeInfinity, dist.Support.Min);
            Assert.AreEqual(Double.PositiveInfinity, dist.Support.Max);

            {
                foreach (var x in new[] { 0.0, 1.0 })
                {
                    double a = dist.InverseDistributionFunction(x);
                    double ab = dist.DistributionFunction(a);
                    double abc = dist.InverseDistributionFunction(ab);
                    double abcd = dist.DistributionFunction(abc);

                    Assert.AreEqual(x, ab, 1e-5);
                    Assert.AreEqual(x, abcd, 1e-5);
                    Assert.AreEqual(a, abc, 1e-5);
                }
            }

            double[] percentiles = Vector.Range(0.0, 1.0, stepSize: 0.1);

            for (int i = 0; i < percentiles.Length; i++)
            {
                double x = percentiles[i];
                double icdf = dist.InverseDistributionFunction(x);
                double cdf = dist.DistributionFunction(icdf);
                Assert.AreEqual(x, cdf, 1e-5);
            }
        }

        [Test]
        public void RangeTest()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(new double[] { 1, 2 }, n1: 0, n2: 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(new double[] { 0, 2 }, n1: 0, n2: 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(new double[] { -1, 2 }, n1: 0, n2: 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(new double[] { -1, 2 }, n1: 1, n2: 1));

            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(new double[] { 1, 2 }, n1: 0, n2: 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(new double[] { 1, 2 }, n1: 0, n2: 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(new double[] { 1, 2 }, n1: 1, n2: 0));

            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(new double[] { 1, 2 }, n1: 0, n2: 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(new double[] { 1, 2 }, n1: 0, n2: 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(new double[] { 1, 2 }, n1: 2, n2: 0));

            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(new double[] { 1 }, n1: 1, n2: 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(new double[] { 1 }, n1: 2, n2: 2));

            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(new double[] { }, n1: 1, n2: 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(new double[] { }, n1: 2, n2: 2));

            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(n1: 0, n2: 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(n1: 0, n2: 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(n1: 0, n2: 1));

            Assert.AreEqual(0.5, new MannWhitneyDistribution(n1: 1, n2: 1).Mean);
            Assert.AreEqual(1, new MannWhitneyDistribution(n1: 1, n2: 2).Mean);
            Assert.AreEqual(1, new MannWhitneyDistribution(n1: 2, n2: 1).Mean);
            Assert.AreEqual(2, new MannWhitneyDistribution(n1: 2, n2: 2).Mean);



            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(n1: 0, n2: 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(n1: 0, n2: 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(n1: 1, n2: 0));

            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(n1: 0, n2: 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(n1: 0, n2: 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MannWhitneyDistribution(n1: 2, n2: 0));

        }
    }
}
