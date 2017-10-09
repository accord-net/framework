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
    using Accord.Statistics;
    using NUnit.Framework;
    using System;
    using Accord.Statistics.Distributions.Multivariate;
    using System.Globalization;

    [TestFixture]
    public class PowerNormalDistributionTest
    {


        [Test]
        public void ConstructorTest1()
        {
            var pnormal = new PowerNormalDistribution(power: 4.2);

            try { double mean = pnormal.Mean; Assert.Fail(); }
            catch { }
            try { double variance = pnormal.Variance; Assert.Fail(); }
            catch { }
            try { double mode = pnormal.Mode; Assert.Fail(); }
            catch { }
            try { double median = pnormal.Median; Assert.Fail(); }
            catch { }

            double cdf = pnormal.DistributionFunction(x: 1.4); // 0.99997428721920678
            double pdf = pnormal.ProbabilityDensityFunction(x: 1.4); // 0.00020022645890003279
            double lpdf = pnormal.LogProbabilityDensityFunction(x: 1.4); // -0.20543269836728234

            double ccdf = pnormal.ComplementaryDistributionFunction(x: 1.4); // 0.000025712780793218926
            double icdf = pnormal.InverseDistributionFunction(p: cdf); // 1.3999999999998953

            double hf = pnormal.HazardFunction(x: 1.4); // 7.7870402470368854
            double chf = pnormal.CumulativeHazardFunction(x: 1.4); // 10.568522382550167

            string str = pnormal.ToString(CultureInfo.InvariantCulture); // PND(x; p = 4.2)

            Assert.AreEqual(10.568522382550167, chf, 1e-10);
            Assert.AreEqual(0.99997428721920678, cdf, 1e-10);
            Assert.AreEqual(0.00020022645890003279, pdf, 1e-10);
            Assert.AreEqual(-0.20543269836728234, lpdf, 1e-10);
            Assert.AreEqual(7.7870402470368854, hf, 1e-10);
            Assert.AreEqual(0.000025712780793218926, ccdf, 1e-10);
            Assert.AreEqual(1.3999999999998953, icdf, 1e-10);
            Assert.AreEqual("PND(x; p = 4.2)", str);

            var range1 = pnormal.GetRange(0.95);
            var range2 = pnormal.GetRange(0.99);
            var range3 = pnormal.GetRange(0.01);

            Assert.AreEqual(-2.2527196079209415, range1.Min, 1e-10);
            Assert.AreEqual(0.024970246427532511, range1.Max, 1e-10);
            Assert.AreEqual(-2.8214873257900464, range2.Min, 1e-10);
            Assert.AreEqual(0.42876122902825864, range2.Max, 1e-10);
            Assert.AreEqual(-2.8214873257900464, range3.Min, 1e-10);
            Assert.AreEqual(0.42876122902825864, range3.Max, 1e-10);

            Assert.AreEqual(double.NegativeInfinity, pnormal.Support.Min);
            Assert.AreEqual(double.PositiveInfinity, pnormal.Support.Max);

            double icdf0 = pnormal.InverseDistributionFunction(0);
            double icdf1 = pnormal.InverseDistributionFunction(1);
            Assert.AreEqual(icdf0, pnormal.Support.Min);
            Assert.AreEqual(icdf1, pnormal.Support.Max);
        }

    }
}
