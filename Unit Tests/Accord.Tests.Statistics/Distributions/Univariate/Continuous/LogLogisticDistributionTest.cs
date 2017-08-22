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
    using System.Globalization;
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;

    [TestFixture]
    public class LogLogisticDistributionTest
    {


        [Test]
        public void ConstructorTest1()
        {
            var log = new LogLogisticDistribution(alpha: 0.42, beta: 2.2);

            double mean = log.Mean;     // 0.60592605102976937
            double median = log.Median; // 0.42
            double mode = log.Mode;     // 0.26892249963239817
            double var = log.Variance;  // 1.4357858982592435

            double cdf = log.DistributionFunction(x: 1.4); // 0.93393329906725353
            double pdf = log.ProbabilityDensityFunction(x: 1.4); // 0.096960115938100763
            double lpdf = log.LogProbabilityDensityFunction(x: 1.4); // -2.3334555609306102

            double ccdf = log.ComplementaryDistributionFunction(x: 1.4); // 0.066066700932746525
            double icdf = log.InverseDistributionFunction(p: cdf); // 1.4000000000000006

            double hf = log.HazardFunction(x: 1.4); // 1.4676094699628273
            double chf = log.CumulativeHazardFunction(x: 1.4); // 2.7170904270953637

            string str = log.ToString(CultureInfo.InvariantCulture); // LogLogistic(x; α = 0.42, β = 2.2)

            // Tested against R's package VGAM:
            // http://www.inside-r.org/packages/cran/VGAM/docs/Fisk

            // dfisk(x=1.4, shape = 2.2, scale = 0.42)
            // [1] 0.096960115938100735478

            // pfisk(1.4, shape = 2.2, scale = 0.42)
            // [1] 0.93393329906725353062

            Assert.AreEqual(0.60592605102976937, mean);
            Assert.AreEqual(0.41999999999999998, median);
            Assert.AreEqual(0.26892249963239817, mode);
            Assert.AreEqual(1.4210644953907947, var, 1e-14);
            Assert.AreEqual(2.7170904270953637, chf);
            Assert.AreEqual(0.93393329906725353062, cdf);
            Assert.AreEqual(0.096960115938100735478, pdf, 1e-15);
            Assert.AreEqual(-2.3334555609306102, lpdf);
            Assert.AreEqual(1.4676094699628273, hf);
            Assert.AreEqual(0.066066700932746525, ccdf);
            Assert.AreEqual(1.4000000000000006, icdf);
            Assert.AreEqual("LogLogistic(x; α = 0.42, β = 2.2)", str);

            var range1 = log.GetRange(0.95);
            var range2 = log.GetRange(0.99);
            var range3 = log.GetRange(0.01);
            var range4 = log.GetRange(1.00);

            Assert.AreEqual(0.11015333581796467, range1.Min);
            Assert.AreEqual(1.6014040672496028, range1.Max);
            Assert.AreEqual(0.052016650554947114, range2.Min);
            Assert.AreEqual(3.3912218129781762, range2.Max);
            Assert.AreEqual(0.0520166505549471, range3.Min);
            Assert.AreEqual(3.3912218129781762, range3.Max);
            Assert.AreEqual(0, range4.Min);
            Assert.AreEqual(double.PositiveInfinity, range4.Max);

            Assert.AreEqual(0, log.Support.Min);
            Assert.AreEqual(double.PositiveInfinity, log.Support.Max);

            Assert.AreEqual(log.InverseDistributionFunction(0), log.Support.Min);
            Assert.AreEqual(log.InverseDistributionFunction(1), log.Support.Max);
        }

        [Test]
        public void default_ctor_test()
        {
            var log = new LogLogisticDistribution();

            Assert.AreEqual(1, log.Shape);
            Assert.AreEqual(1, log.Scale);
        }
    }
}
