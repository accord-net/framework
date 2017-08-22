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
    using Accord.Statistics.Testing;

    [TestFixture]
    public class GompertzDistributionTest
    {

        [Test]
        public void ConstructorTest()
        {
            var gompertz = new GompertzDistribution(eta: 4.2, b: 1.1);

            try { double mean = gompertz.Mean; Assert.Fail(); }
            catch { }
            double median = gompertz.Median; // 0.13886469671401389
            double mode = gompertz.Mode;     // 0.0

            double cdf = gompertz.DistributionFunction(x: 0.27); // 0.76599768199799145
            double pdf = gompertz.ProbabilityDensityFunction(x: 0.27); // 1.4549484164912097
            double lpdf = gompertz.LogProbabilityDensityFunction(x: 0.27); // 0.37497044741163688

            double ccdf = gompertz.ComplementaryDistributionFunction(x: 0.27); // 0.23400231800200855
            double icdf = gompertz.InverseDistributionFunction(p: cdf); // 0.26999999999766749

            double hf = gompertz.HazardFunction(x: 0.27); // 6.2176666834502088
            double chf = gompertz.CumulativeHazardFunction(x: 0.27); // 1.4524242576820101

            string str = gompertz.ToString(System.Globalization.CultureInfo.InvariantCulture);
            // "Gompertz(x; η = 4.2, b = 1.1)"

            Assert.AreEqual(0.13886469671401389, median);
            Assert.AreEqual(0.0, mode);
            Assert.AreEqual(1.4524242576820101, chf);
            Assert.AreEqual(0.76599768199799145, cdf);
            Assert.AreEqual(1.4549484164912097, pdf);
            Assert.AreEqual(0.37497044741163688, lpdf);
            Assert.AreEqual(6.2176666834502088, hf);
            Assert.AreEqual(0.23400231800200855, ccdf);
            Assert.AreEqual(0.26999999999766749, icdf);
            Assert.AreEqual("Gompertz(x; η = 4.2, b = 1.1)", str);

            var range1 = gompertz.GetRange(0.95);
            var range2 = gompertz.GetRange(0.99);
            var range3 = gompertz.GetRange(0.01);

            Assert.AreEqual(0.011035174219697141, range1.Min, 1e-6);
            Assert.AreEqual(0.48945776418276288, range1.Max, 1e-6);
            Assert.AreEqual(0.002172798720176344, range2.Min, 1e-6);
            Assert.AreEqual(0.67295877422837591, range2.Max, 1e-6);
            Assert.AreEqual(0.0021727987201762976, range3.Min, 1e-6);
            Assert.AreEqual(0.67295877422837591, range3.Max, 1e-6);

            Assert.AreEqual(0, gompertz.Support.Min);
            Assert.AreEqual(double.PositiveInfinity, gompertz.Support.Max);

            Assert.AreEqual(gompertz.InverseDistributionFunction(0), gompertz.Support.Min);
            Assert.AreEqual(gompertz.InverseDistributionFunction(1), gompertz.Support.Max);
        }

        [Test]
        public void ConstructorTest2()
        {
            var gompertz = new GompertzDistribution(eta: 0.2, b: 1.1);

            try { double mean = gompertz.Mean; Assert.Fail(); }
            catch { }
            double median = gompertz.Median; // 1.3603945605494754
            double mode = gompertz.Mode;     // 1.4631253749400912

            double cdf = gompertz.DistributionFunction(x: 0.27); // 0.066825495660013834
            double pdf = gompertz.ProbabilityDensityFunction(x: 0.27); // 0.27629371549904269
            double lpdf = gompertz.LogProbabilityDensityFunction(x: 0.27); // -1.2862907925193949

            double ccdf = gompertz.ComplementaryDistributionFunction(x: 0.27); // 0.93317450433998617
            double icdf = gompertz.InverseDistributionFunction(p: cdf); // 0.27000016175969527

            double hf = gompertz.HazardFunction(x: 0.27); // 0.29607936587858147
            double chf = gompertz.CumulativeHazardFunction(x: 0.27); // 0.069163059889619516

            string str = gompertz.ToString(System.Globalization.CultureInfo.InvariantCulture);
            // "Gompertz(x; η = 4.2, b = 1.1)"

            Assert.AreEqual(1.3603945605494754, median);
            Assert.AreEqual(1.4631253749400912, mode);
            Assert.AreEqual(0.069163059889619516, chf);
            Assert.AreEqual(0.066825495660013834, cdf);
            Assert.AreEqual(0.27629371549904269, pdf);
            Assert.AreEqual(-1.2862907925193949, lpdf);
            Assert.AreEqual(0.29607936587858147, hf);
            Assert.AreEqual(0.93317450433998617, ccdf);
            Assert.AreEqual(0.27000016175969527, icdf);
            Assert.AreEqual("Gompertz(x; η = 0.2, b = 1.1)", str);
        }

        [Test]
        public void ConstructorTest3()
        {
            try
            {
                new GompertzDistribution(eta: 0, b: 7);
                Assert.Fail();
            }
            catch { }

            try
            {
                new GompertzDistribution(eta: 1, b: 0);
                Assert.Fail();
            }
            catch { }
        }

        [Test]
        public void MedianTest()
        {
            var target = new GompertzDistribution(eta: 42, b: 4.2);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5), 1e-8);
        }


    }
}
