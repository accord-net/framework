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
    public class LogisticDistributionTest
    {

        [Test]
        public void ConstructorTest1()
        {
            var log = new LogisticDistribution(location: 0.42, scale: 1.2);

            double mean = log.Mean;     // 0.42
            double median = log.Median; // 0.42
            double mode = log.Mode;     // 0.42
            double var = log.Variance;  // 4.737410112522892

            double cdf = log.DistributionFunction(x: 1.4); // 0.693528308197921
            double pdf = log.ProbabilityDensityFunction(x: 1.4); // 0.17712232827170876
            double lpdf = log.LogProbabilityDensityFunction(x: 1.4); // -1.7309146649427332

            double ccdf = log.ComplementaryDistributionFunction(x: 1.4); // 0.306471691802079
            double icdf = log.InverseDistributionFunction(p: cdf); // 1.3999999999999997

            double hf = log.HazardFunction(x: 1.4); // 0.57794025683160088
            double chf = log.CumulativeHazardFunction(x: 1.4); // 1.1826298874077226

            string str = log.ToString(CultureInfo.InvariantCulture); // Logistic(x; μ = 0.42, s = 1.2)

            Assert.AreEqual(0.41999999999999998, mean);
            Assert.AreEqual(0.41999999999999998, median);
            Assert.AreEqual(0.41999999999999998, mode);
            Assert.AreEqual(4.737410112522892, var);
            Assert.AreEqual(1.1826298874077226, chf);
            Assert.AreEqual(0.693528308197921, cdf);
            Assert.AreEqual(0.17712232827170876, pdf);
            Assert.AreEqual(-1.7309146649427332, lpdf);
            Assert.AreEqual(0.57794025683160088, hf);
            Assert.AreEqual(0.306471691802079, ccdf);
            Assert.AreEqual(1.3999999999999997, icdf);
            Assert.AreEqual("Logistic(x; μ = 0.42, s = 1.2)", str);

            var range1 = log.GetRange(0.95);
            var range2 = log.GetRange(0.99);
            var range3 = log.GetRange(0.01);

            Assert.AreEqual(-3.1133267749997273, range1.Min);
            Assert.AreEqual(3.9533267749997272, range1.Max);
            Assert.AreEqual(-5.0941438201615066, range2.Min);
            Assert.AreEqual(5.9341438201615064, range2.Max);
            Assert.AreEqual(-5.0941438201615075, range3.Min);
            Assert.AreEqual(5.9341438201615064, range3.Max);

            Assert.AreEqual(double.NegativeInfinity, log.Support.Min);
            Assert.AreEqual(double.PositiveInfinity, log.Support.Max);

            Assert.AreEqual(log.InverseDistributionFunction(0), log.Support.Min);
            Assert.AreEqual(log.InverseDistributionFunction(1), log.Support.Max);
        }

    }
}
