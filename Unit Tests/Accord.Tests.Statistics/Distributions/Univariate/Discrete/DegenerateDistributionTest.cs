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
    using Accord.Math;
    using System.Globalization;
    using Accord.Statistics.Distributions;

    [TestFixture]
    public class DegenerateDistributionTest
    {

        [Test]
        public void ConstructorTest()
        {
            var dist = new DegenerateDistribution(value: 2);

            double mean = dist.Mean;     // 2
            double median = dist.Median; // 2
            double mode = dist.Mode;     // 2
            double var = dist.Variance;  // 1

            double cdf1 = dist.DistributionFunction(k: 1);   // 0
            double cdf2 = dist.DistributionFunction(k: 2);   // 1

            double pdf1 = dist.ProbabilityMassFunction(k: 1); // 0
            double pdf2 = dist.ProbabilityMassFunction(k: 2); // 1
            double pdf3 = dist.ProbabilityMassFunction(k: 3); // 0

            double lpdf = dist.LogProbabilityMassFunction(k: 2); // 0
            double ccdf = dist.ComplementaryDistributionFunction(k: 2); // 0.0

            int icdf1 = dist.InverseDistributionFunction(p: 0.0); // 1
            int icdf2 = dist.InverseDistributionFunction(p: 0.7); // 3
            int icdf3 = dist.InverseDistributionFunction(p: 1.0); // 2

            double hf = dist.HazardFunction(x: 0); // 0.0
            double chf = dist.CumulativeHazardFunction(x: 0); // 0.0

            string str = dist.ToString(CultureInfo.InvariantCulture); // Degenerate(x; k0 = 2)

            Assert.AreEqual(2, mean);
            Assert.AreEqual(2, median);
            Assert.AreEqual(2, mode);
            Assert.AreEqual(0, var);
            Assert.AreEqual(0, chf);
            Assert.AreEqual(0, cdf1);
            Assert.AreEqual(1, cdf2);
            Assert.AreEqual(0, pdf1);
            Assert.AreEqual(1, pdf2);
            Assert.AreEqual(0, pdf3);
            Assert.AreEqual(0, lpdf);
            Assert.AreEqual(0, hf);
            Assert.AreEqual(0, ccdf);
            Assert.AreEqual(1, icdf1);
            Assert.AreEqual(3, icdf2);
            Assert.AreEqual(2, icdf3);
            Assert.AreEqual("Degenerate(x; k0 = 2)", str);

            var range1 = dist.GetRange(0.95);
            var range2 = dist.GetRange(0.99);
            var range3 = dist.GetRange(0.01);

            Assert.AreEqual(1.0, range1.Min);
            Assert.AreEqual(3.0, range1.Max);
            Assert.AreEqual(1.0, range2.Min);
            Assert.AreEqual(3.0, range2.Max);
            Assert.AreEqual(1.0, range3.Min);
            Assert.AreEqual(3.0, range3.Max);
        }

        [Test]
        public void ConstructorTest_DoubleIntConversionTest()
        {
            IUnivariateDistribution dist = new DegenerateDistribution(value: 2);

            double mean = dist.Mean;     // 2
            double median = dist.Median; // 2
            double mode = dist.Mode;     // 2
            double var = dist.Variance;  // 1

            double cdf1 = dist.DistributionFunction(x: 1.9);   // 0
            double cdf2 = dist.DistributionFunction(x: 2.0);   // 1
            double cdf3 = dist.DistributionFunction(x: 2.1);   // 1

            double pdf1 = dist.ProbabilityFunction(x: 1.9); // 0
            double pdf2 = dist.ProbabilityFunction(x: 2.0); // 1
            double pdf3 = dist.ProbabilityFunction(x: 2.1); // 0

            double lpdf = dist.LogProbabilityFunction(x: 2); // 0
            double ccdf = dist.ComplementaryDistributionFunction(x: 2); // 0.0

            double icdf1 = dist.InverseDistributionFunction(p: 0.0); // 1
            double icdf2 = dist.InverseDistributionFunction(p: 0.7); // 3
            double icdf3 = dist.InverseDistributionFunction(p: 1.0); // 2

            double hf1 = dist.HazardFunction(x: 1.9); // 0.0
            double hf2 = dist.HazardFunction(x: 2.0); // 0.0
            double hf3 = dist.HazardFunction(x: 2.1); // 0.0
            double chf1 = dist.CumulativeHazardFunction(x: 1.9); // 0.0
            double chf2 = dist.CumulativeHazardFunction(x: 2.0); // 0.0
            double chf3 = dist.CumulativeHazardFunction(x: 2.1); // 0.0

            Assert.AreEqual(2, mean);
            Assert.AreEqual(2, median);
            Assert.AreEqual(2, mode);
            Assert.AreEqual(0, var);
            Assert.AreEqual(0, chf1);
            Assert.AreEqual(double.PositiveInfinity, chf2);
            Assert.AreEqual(double.PositiveInfinity, chf3);
            Assert.AreEqual(0, cdf1);
            Assert.AreEqual(1, cdf2);
            Assert.AreEqual(1, cdf3);
            Assert.AreEqual(0, pdf1);
            Assert.AreEqual(1, pdf2);
            Assert.AreEqual(1, pdf3);
            Assert.AreEqual(0, lpdf);
            Assert.AreEqual(0, hf1);
            Assert.AreEqual(double.PositiveInfinity, hf2);
            Assert.AreEqual(double.PositiveInfinity, hf3);
            Assert.AreEqual(0, ccdf);
            Assert.AreEqual(1, icdf1);
            Assert.AreEqual(3, icdf2);
            Assert.AreEqual(2, icdf3);

            Assert.AreEqual(0, chf1);
            Assert.AreEqual(double.PositiveInfinity, chf2);
            Assert.AreEqual(double.PositiveInfinity, chf3);

            var range1 = dist.GetRange(0.95);
            var range2 = dist.GetRange(0.99);
            var range3 = dist.GetRange(0.01);

            Assert.AreEqual(1.0, range1.Min);
            Assert.AreEqual(3.0, range1.Max);
            Assert.AreEqual(1.0, range2.Min);
            Assert.AreEqual(3.0, range2.Max);
            Assert.AreEqual(1.0, range3.Min);
            Assert.AreEqual(3.0, range3.Max);
        }
    }
}
