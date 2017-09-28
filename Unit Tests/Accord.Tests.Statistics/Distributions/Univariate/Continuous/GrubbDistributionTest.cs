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
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;
    using System;
    using Accord.Math;
    using Accord.Math.Differentiation;

    [TestFixture]
    public class GrubbDistributionTest
    {

        [Test]
        public void doc()
        {
            var grubb = new GrubbDistribution(samples: 8);

            double chf = grubb.CumulativeHazardFunction(x: 1.27);           // 0.25670891803568036
            double cdf = grubb.DistributionFunction(x: 1.27);               // 0.22640663992932097
            double ccdf = grubb.ComplementaryDistributionFunction(x: 1.27); // 0.773593360070679
            double icdf = grubb.InverseDistributionFunction(p: cdf);        // 1.27

            string str = grubb.ToString(System.Globalization.CultureInfo.InvariantCulture); // B(x; α = 0.42, β = 1.57)

            Assert.AreEqual(0.25670891803568036, chf);
            Assert.AreEqual(0.22640663992932097, cdf);
            Assert.AreEqual(0.773593360070679, ccdf);
            Assert.AreEqual(1.27, icdf, 1e-10);
            Assert.AreEqual("Grubb(x; n = 8)", str);

            var range1 = grubb.GetRange(0.95);
            var range2 = grubb.GetRange(0.99);
            var range3 = grubb.GetRange(0.01);

            Assert.AreEqual(1.1684847650106549, range1.Min);
            Assert.AreEqual(2.031652001549944, range1.Max);
            Assert.AreEqual(1.1468556105506391, range2.Min);
            Assert.AreEqual(2.2208334515104258, range2.Max);
            Assert.AreEqual(1.1468556105506391, range3.Min);
            Assert.AreEqual(2.2208334515104258, range3.Max);

            Assert.AreEqual(1.1415178307749025, grubb.Support.Min, 1e-10);
            Assert.AreEqual(2.4748737341529163, grubb.Support.Max, 1e-10);

            Assert.AreEqual(grubb.InverseDistributionFunction(0), grubb.Support.Min);
            Assert.AreEqual(grubb.InverseDistributionFunction(1), grubb.Support.Max);
        }

        [Test]
        public void cdf()
        {
            double[] x = Vector.Interval(0.0, 1.0, stepSize: 1e-3);

            var target = new GrubbDistribution(7);
            double[] cdf = x.Apply(xi => target.InverseDistributionFunction(xi));

            double min = cdf.Min();
            double max = cdf.Max();

            Assert.AreEqual(2.2677868380553634, max, 1e-10);
            Assert.AreEqual(max, target.Support.Max, 1e-10);

            Assert.AreEqual(1.0688047803397081, min, 1e-10);
            Assert.AreEqual(min, target.Support.Min, 1e-10);

            double actual;

            Assert.AreEqual(0, target.DistributionFunction(-1));
            Assert.AreEqual(0, target.DistributionFunction(0));
            Assert.AreEqual(0, target.DistributionFunction(1));
            Assert.AreEqual(0, target.DistributionFunction((max - min) / 2.0));
            Assert.AreEqual(0.33127709351171297d, target.DistributionFunction(1.27), 1e-8);
            Assert.AreEqual(0, target.DistributionFunction(min));
            Assert.AreEqual(0, target.DistributionFunction(min - 1e-10));
            actual = target.DistributionFunction(min + 1e-10);
            Assert.AreEqual(2.5226376543230344E-10, actual, 1e-10);

            Assert.AreEqual(1, target.DistributionFunction(max), 1e-10);
            Assert.AreEqual(1, target.DistributionFunction(max - 1e-10), 1e-10);
            actual = target.DistributionFunction(max + 1e-10);
            Assert.AreEqual(1, actual, 1e-10);
        }

        [Test]
        public void icdf()
        {
            var target = new GrubbDistribution(8);

            double icdf;
            icdf = target.InverseDistributionFunction(0);
            Assert.AreEqual(1.1415178307749025, icdf, 1e-8);

            icdf = target.InverseDistributionFunction(0.8);
            Assert.AreEqual(1.7490784053905972, icdf, 1e-8);

            icdf = target.InverseDistributionFunction(0.5);
            Assert.AreEqual(1.4560387907016017, icdf, 1e-8);

            icdf = target.InverseDistributionFunction(0.2);
            Assert.AreEqual(1.2540855738963992, icdf, 1e-8);

            icdf = target.InverseDistributionFunction(0.1);
            Assert.AreEqual(1.1961855587900832, icdf, 1e-8);

            icdf = target.InverseDistributionFunction(1);
            Assert.AreEqual(2.4748737341529163, icdf, 1e-8);

            Assert.Throws<ArgumentOutOfRangeException>(() => target.InverseDistributionFunction(1.5));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.InverseDistributionFunction(-0.5));
        }

        [Test]
        public void cdf2()
        {
            var target = new GrubbDistribution(3);
            double cdf;

            cdf = target.DistributionFunction(0);
            Assert.AreEqual(0, cdf, 1e-8);

            cdf = target.DistributionFunction(0.8);
            Assert.AreEqual(0.23089631020036727, cdf, 1e-8);

            cdf = target.DistributionFunction(0.1);
            Assert.AreEqual(0, cdf, 1e-8);

            cdf = target.DistributionFunction(1);
            Assert.AreEqual(0.49999999999999845, cdf, 1e-8);

            cdf = target.DistributionFunction(1.1);
            Assert.AreEqual(0.70489468240783792, cdf, 1e-8);

            cdf = target.DistributionFunction(1.1547005383792517); // precision error if not handled
            Assert.AreEqual(1, cdf, 1e-8);
        }

        [Test]
        public void range_test()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new GrubbDistribution(samples: 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new GrubbDistribution(samples: 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new GrubbDistribution(samples: 2));
            Assert.DoesNotThrow(() => new GrubbDistribution(samples: 3));
        }
    }
}
