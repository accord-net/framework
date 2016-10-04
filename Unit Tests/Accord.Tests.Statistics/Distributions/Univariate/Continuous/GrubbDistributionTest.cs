// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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

            string str = grubb.ToString(System.Globalization.CultureInfo.InvariantCulture); // "B(x; α = 0.42, β = 1.57)

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
        }

        [Test]
#if MONO
        [Ignore("Mono output differs for the same formulae.")]
#endif
        public void cdf()
        {
            double[] x = Vector.Range(0.0, 1.0, stepSize: 1e-3);

            var target = new GrubbDistribution(42);
            double[] cdf = x.Apply(xi => target.InverseDistributionFunction(xi));

            double min = cdf.Min();
            double max = cdf.Max();

            Assert.AreEqual(6.3264373484457685, max, 1e-10);
            Assert.AreEqual(max, target.Support.Max, 1e-10);

            Assert.AreEqual(1.9451708565372674, min, 1e-10);

            Assert.AreEqual(0, target.DistributionFunction(-1));
            Assert.AreEqual(0, target.DistributionFunction(0));
            Assert.AreEqual(0, target.DistributionFunction(min));
            Assert.AreEqual(0, target.DistributionFunction(min - 1e-10));
            double actual = target.DistributionFunction(min + 1e-10);
            Assert.AreEqual(2.5226376543230344E-10, actual, 1e-10);
        }

    }
}
