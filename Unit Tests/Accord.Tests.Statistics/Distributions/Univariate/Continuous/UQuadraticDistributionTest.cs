// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Ashley Messer, 2014
// glyphard at gmail.com
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

namespace Accord.Tests.Statistics.Distributions.Univariate.Continuous
{
    using System;
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;
    using System.Globalization;

    [TestFixture]
    public class UQuadraticDistributionTest
    {

        [Test]
        public void Constructor_UQuadratic()
        {
            double a = -2.0d;
            double b = 2.0d;
            double x = 0.0d;

            var uQuadDist = new UQuadraticDistribution(a, b);
            double mean = uQuadDist.Mean; //0.0
            double variance = uQuadDist.Variance; //2.4
            double median = uQuadDist.Median; //0.0
            double pdf = uQuadDist.ProbabilityDensityFunction(x); //0.0
            double cdf = uQuadDist.DistributionFunction(x); //0.5
            string tostr = uQuadDist.ToString(); //UQuadratic(x; a = -2, b = 2)

            Assert.AreEqual(mean, 0);
            Assert.AreEqual(variance, 2.4d);
            Assert.AreEqual(median, 0);
            Assert.AreEqual(pdf, 0);
            Assert.AreEqual(cdf, 0.5);
            Assert.AreEqual(tostr, "U-Quadratic(x; a = -2, b = 2)");
        }

        [Test]
        public void DocumentationTest1()
        {
            var u2 = new UQuadraticDistribution(a: 0.42, b: 4.2);

            double mean = u2.Mean;     // 2.3100000000000001
            double median = u2.Median; // 2.3100000000000001
            double mode = u2.Mode;     // 0.8099060089153145
            double var = u2.Variance;  // 2.1432600000000002

            double cdf = u2.DistributionFunction(x: 1.4); // 0.44419041812731797
            double pdf = u2.ProbabilityDensityFunction(x: 1.4); // 0.18398763254730335
            double lpdf = u2.LogProbabilityDensityFunction(x: 1.4); // -1.6928867380489712

            double ccdf = u2.ComplementaryDistributionFunction(x: 1.4); // 0.55580958187268203
            double icdf = u2.InverseDistributionFunction(p: cdf); // 1.3999998213768274

            double hf = u2.HazardFunction(x: 1.4); // 0.3310263776442936
            double chf = u2.CumulativeHazardFunction(x: 1.4); // 0.58732952203701494

            string str = u2.ToString(CultureInfo.InvariantCulture); // "UQuadratic(x; a = 0.42, b = 4.2)"

            Assert.AreEqual(2.3100000000000001, mean);
            Assert.AreEqual(2.3100000000000001, median);
            Assert.AreEqual(0.8099060089153145, mode);
            Assert.AreEqual(2.1432600000000002, var);
            Assert.AreEqual(0.58732952203701494, chf);
            Assert.AreEqual(0.44419041812731797, cdf);
            Assert.AreEqual(0.18398763254730335, pdf);
            Assert.AreEqual(-1.6928867380489712, lpdf);
            Assert.AreEqual(0.3310263776442936, hf);
            Assert.AreEqual(0.55580958187268203, ccdf);
            Assert.AreEqual(1.3999998213768274, icdf);
            Assert.AreEqual("U-Quadratic(x; a = 0.42, b = 4.2)", str);

            var range1 = u2.GetRange(0.95);
            var range2 = u2.GetRange(0.99);
            var range3 = u2.GetRange(0.01);

            Assert.AreEqual(0.48522504056375509, range1.Min);
            Assert.AreEqual(4.1347749594362453, range1.Max);
            Assert.AreEqual(0.43268494596201756, range2.Min);
            Assert.AreEqual(4.1873150540379829, range2.Max);
            Assert.AreEqual(0.43268494596201751, range3.Min);
            Assert.AreEqual(4.1873150540379829, range3.Max);
        }
    }
}
