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
    using System.Collections.Generic;
    using System.Globalization;

    [TestFixture]
    public class ShapiroWilkDistributionTest
    {


        [Test]
        public void ConstructorTest()
        {
            // Example from http://www.nag.com/numeric/cl/nagdoc_cl23/pdf/G01/g01ddc.pdf

            double[] a =
            {
                0.11, 7.87, 4.61, 10.14, 7.95, 3.14, 0.46, 4.43,
                0.21, 4.75, 0.71, 1.52, 3.24, 0.93, 0.42, 4.97,
                9.53, 4.55, 0.47, 6.66
            };

            var sw = new ShapiroWilkDistribution(a.Length);

            double expected = 0.0421;
            double actual = sw.DistributionFunction(0.9005);
            Assert.AreEqual(expected, actual, 1e-4);
        }

        [Test]
        public void ConstructorTest2()
        {
            // Example from http://www.nag.com/numeric/cl/nagdoc_cl23/pdf/G01/g01ddc.pdf

            double[] b =
            {
                1.36, 1.14, 2.92, 2.55, 1.46, 1.06, 5.27, 1.11, 3.48,
                1.10, 0.88, 0.51, 1.46, 0.52, 6.20, 1.69, 0.08, 3.67,
                2.81, 3.49
            };

            var sw = new ShapiroWilkDistribution(b.Length);

            double expected = 0.5246;
            double actual = sw.DistributionFunction(0.9590);
            Assert.AreEqual(expected, actual, 1e-3);
        }

        [Test]
        public void ConstructorTest3()
        {
            var sw = new ShapiroWilkDistribution(samples: 12);

            double mean = sw.Mean;     // 0.940148636841248
            double median = sw.Median; // 0.940148636841248
            double mode = sw.Mode;     // 0.940148636841248

            double cdf = sw.DistributionFunction(x: 0.42); // 4.8168255270011394E-06
            double pdf = sw.ProbabilityDensityFunction(x: 0.42); // 0.000043477460596194137
            double lpdf = sw.LogProbabilityDensityFunction(x: 0.42); // -10.043267901368219

            double ccdf = sw.ComplementaryDistributionFunction(x: 0.42); // 0.999995183174473
            double icdf = sw.InverseDistributionFunction(p: cdf); // 0.42000002275671627

            double hf = sw.HazardFunction(x: 0.42); // 4.3477670020544943E-05
            double chf = sw.CumulativeHazardFunction(x: 0.42); // 4.8168371279400235E-06

            string str = sw.ToString(CultureInfo.InvariantCulture); // W(x; n = 12)

            Assert.AreEqual(0.940148636841248, mean);
            Assert.AreEqual(0.940148636841248, mode);
            Assert.AreEqual(0.940148636841248, median, 1e-8);
            Assert.AreEqual(4.8168371279400235E-06, chf);
            Assert.AreEqual(4.8168255270011394E-06, cdf);
            Assert.AreEqual(0.000043477460596194137, pdf);
            Assert.AreEqual(-10.043267901368219, lpdf);
            Assert.AreEqual(4.3477670020544943E-05, hf);
            Assert.AreEqual(0.999995183174473, ccdf);
            Assert.AreEqual(0.42000002275671627, icdf, 1e-8);
            Assert.AreEqual("W(x; n = 12)", str);

            var range1 = sw.GetRange(0.95);
            Assert.AreEqual(0.8607805197002204, range1.Min, 1e-6);
            Assert.AreEqual(0.97426955790462533, range1.Max, 1e-6);

            var range2 = sw.GetRange(0.99);
            Assert.AreEqual(0.80248479750351542, range2.Min, 1e-6);
            Assert.AreEqual(0.98186388183806661, range2.Max, 1e-6);

            var range3 = sw.GetRange(0.01);
            Assert.AreEqual(0.80248479750351542, range3.Min, 1e-6);
            Assert.AreEqual(0.98186388183806661, range3.Max, 1e-6);

            Assert.AreEqual(0, sw.Support.Min);
            Assert.AreEqual(1, sw.Support.Max);

            Assert.AreEqual(sw.InverseDistributionFunction(0), sw.Support.Min);
            Assert.AreEqual(sw.InverseDistributionFunction(1), sw.Support.Max);
        }

        [Test]
        public void ConstructorTest4()
        {
            // Create a new Shapiro-Wilk's W for 5 samples
            var sw = new ShapiroWilkDistribution(samples: 5);

            double mean = sw.Mean;     // 0.8490362043845332
            double median = sw.Median; // 0.8490362043845332
            double mode = sw.Mode;     // 0.8490362043845332

            double cdf = sw.DistributionFunction(x: 0.84); // 0.16492187919271617
            double pdf = sw.ProbabilityDensityFunction(x: 0.84); // 0.82021062372326459
            double lpdf = sw.LogProbabilityDensityFunction(x: 0.84); // -0.1981941135071546

            double ccdf = sw.ComplementaryDistributionFunction(x: 0.84); // 0.83507812080728383
            double icdf = sw.InverseDistributionFunction(p: cdf); // 0.84000000194587177

            double hf = sw.HazardFunction(x: 0.84); // 0.98219627994845971
            double chf = sw.CumulativeHazardFunction(x: 0.84); // 0.18023000065451003

            string str = sw.ToString(CultureInfo.InvariantCulture); // W(x; n = 12)

            Assert.AreEqual(0.8490362043845332, mean);
            Assert.AreEqual(0.8490362043845332, mode);
            Assert.AreEqual(0.8490362043845332, median, 1e-8);
            Assert.AreEqual(0.18023000065451003, chf);
            Assert.AreEqual(0.16492187919271617, cdf);
            Assert.AreEqual(0.82021062372326459, pdf);
            Assert.AreEqual(-0.1981941135071546, lpdf);
            Assert.AreEqual(0.98219627994845971, hf);
            Assert.AreEqual(0.83507812080728383, ccdf);
            Assert.AreEqual(0.84000000194587177, icdf, 1e-8);
            Assert.AreEqual("W(x; n = 5)", str);

            var range1 = sw.GetRange(0.95);
            Assert.AreEqual(0.77509977845943778, range1.Min, 1e-6);
            Assert.AreEqual(0.98299906816568339, range1.Max, 1e-6);

            var range2 = sw.GetRange(0.99);
            Assert.AreEqual(0.70180031139628618, range2.Min, 1e-6);
            Assert.AreEqual(0.99334588234528642, range2.Max, 1e-6);

            var range3 = sw.GetRange(0.01);
            Assert.AreEqual(0.70180031139628618, range3.Min, 1e-6);
            Assert.AreEqual(0.99334588234528642, range3.Max, 1e-6);

            Assert.AreEqual(0, sw.Support.Min);
            Assert.AreEqual(1, sw.Support.Max);

            Assert.AreEqual(sw.InverseDistributionFunction(0), sw.Support.Min);
            Assert.AreEqual(sw.InverseDistributionFunction(1), sw.Support.Max);
        }



        [Test]
        public void cdf()
        {
            var e1 = new Dictionary<int, double>
            {
                { 5,  0.081034388254077383 },
                { 10, 0.014498539552460298 },
                { 11, 0.0094071917739316415 },
                { 15, 0.0036762734018237525 }
            };

            var e2 = new Dictionary<int, double>
            {
                { 5,  0.99999999278182994 },
                { 10, 1 },
                { 11, 1 },
                { 15, 1 }
            };

            foreach (int samples in new[] { 5, 10, 11, 15 })
            {
                var dist = new ShapiroWilkDistribution(samples: samples);

                Assert.IsTrue(dist.Support.Min < dist.Support.Max);

                double actual, e;
                actual = dist.DistributionFunction(0);
                Assert.AreEqual(0, actual, 1e-8);

                actual = dist.DistributionFunction(1);
                Assert.AreEqual(1, actual);

                actual = dist.DistributionFunction(0.8);
                e = e1[samples];
                Assert.AreEqual(e, actual, 1e-10);

                actual = dist.DistributionFunction(0.999999);
                e = e2[samples];
                Assert.AreEqual(e, actual, 1e-10);

                actual = dist.DistributionFunction(-1);
                Assert.AreEqual(0, actual);

                actual = dist.DistributionFunction(2);
                Assert.AreEqual(1, actual);

                actual = dist.DistributionFunction(100);
                Assert.AreEqual(1, actual);

                actual = dist.DistributionFunction(double.PositiveInfinity);
                Assert.AreEqual(1, actual);

                actual = dist.DistributionFunction(double.NegativeInfinity);
                Assert.AreEqual(0, actual);
            }
        }

        [Test]
        public void inverse_cdf()
        {
            foreach (int samples in new[] { 5, 10, 11, 15 })
            {
                var dist = new ShapiroWilkDistribution(samples: samples);


                double[] percentiles = Vector.Range(0.49, 0.51, stepSize: 0.01);
                for (int i = 0; i < percentiles.Length; i++)
                {
                    double p = percentiles[i];
                    double icdf = dist.InverseDistributionFunction(p);
                    double cdf = dist.DistributionFunction(icdf);
                    Assert.AreEqual(p, cdf, 1e-5);
                }
            }
        }

        [Test]
        public void inverse_cdf_simple()
        {
            var sw = new ShapiroWilkDistribution(samples: 12);

            double cdf = sw.DistributionFunction(x: 0.42);
            Assert.AreEqual(4.8168255270011394E-06, cdf);

            double ccdf = sw.ComplementaryDistributionFunction(x: 0.42);
            Assert.AreEqual(0.999995183174473, ccdf);

            // Shapiro-Wilk support is inversed?
            double icdf = sw.InverseDistributionFunction(p: cdf);
            Assert.AreEqual(0.42000002275671627, icdf, 1e-8);
        }
    }
}