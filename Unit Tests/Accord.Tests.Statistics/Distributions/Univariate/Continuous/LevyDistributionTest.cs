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
    using System;
    using System.Globalization;
    using Accord.Statistics;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;

    [TestFixture]
    public class LevyDistributionTest
    {

        [Test]
        public void ConstructorTest()
        {
            var levy = new LevyDistribution(location: 1, scale: 4.2);

            double mean = levy.Mean;     // +inf
            double median = levy.Median; // 10.232059220934481
            double mode = levy.Mode;     // NaN
            double var = levy.Variance;  // +inf

            double cdf = levy.DistributionFunction(x: 1.4); // 0.0011937454448720029
            double pdf = levy.ProbabilityDensityFunction(x: 1.4); // 0.016958939623898304
            double lpdf = levy.LogProbabilityDensityFunction(x: 1.4); // -4.0769601727487803

            double ccdf = levy.ComplementaryDistributionFunction(x: 1.4); // 0.99880625455512795
            double icdf = levy.InverseDistributionFunction(p: cdf); // 1.3999999

            double hf = levy.HazardFunction(x: 1.4); // 0.016979208476674869
            double chf = levy.CumulativeHazardFunction(x: 1.4); // 0.0011944585265140923

            string str = levy.ToString(CultureInfo.InvariantCulture); // Lévy(x; μ = 1, c = 4.2)

            // Tested against GNU R's rmutils package
            //
            // dlevy(1.4, m=1, s=4.2)
            // [1] 0.016958939623898303811
            //
            // plevy(1.4, m=1, s=4.2)
            // [1] 0.0011937454448720519196


            Assert.AreEqual(Double.PositiveInfinity, mean);
            Assert.AreEqual(10.232059220934481, median);
            Assert.IsTrue(Double.IsNaN(mode));
            Assert.AreEqual(Double.PositiveInfinity, var);
            Assert.AreEqual(0.0011944585265140923, chf);
            Assert.AreEqual(0.0011937454448720519196, cdf, 1e-10); // R
            Assert.AreEqual(0.016958939623898303811, pdf, 1e-10); // R
            Assert.AreEqual(-4.0769601727487803, lpdf);
            Assert.AreEqual(0.016979208476674869, hf);
            Assert.AreEqual(0.99880625455512795, ccdf);
            Assert.AreEqual(1.4, icdf, 1e-6);
            Assert.AreEqual("Lévy(x; μ = 1, c = 4.2)", str);

            double p = levy.DistributionFunction(levy.Median);
            Assert.AreEqual(0.5, p, 1e-10);
            Assert.IsFalse(Double.IsNaN(p));

            var range1 = levy.GetRange(0.95);
            var range2 = levy.GetRange(0.99);
            var range3 = levy.GetRange(0.01);

            Assert.AreEqual(2.0933346408334241, range1.Min);
            Assert.AreEqual(1069.1206671123464, range1.Max);
            Assert.AreEqual(1.6330166470647871, range2.Min);
            Assert.AreEqual(26737.630417446126, range2.Max);
            Assert.AreEqual(1.6330166470647871, range3.Min);
            Assert.AreEqual(26737.630417446126, range3.Max);
        }

        [Test]
        public void ConstructorTest2()
        {
            var levy = new LevyDistribution(location: 0, scale: 2.4);

            double mean = levy.Mean;     // +inf
            double median = levy.Median; // 5.2754624119625602
            double mode = levy.Mode;     // 0.79999999999999993
            double var = levy.Variance;  // +inf

            double cdf = levy.DistributionFunction(x: 1.4); // 0.19043026382552419
            double pdf = levy.ProbabilityDensityFunction(x: 1.4); // 0.15833291961096485
            double lpdf = levy.LogProbabilityDensityFunction(x: 1.4); // -1.8430553766023994

            double ccdf = levy.ComplementaryDistributionFunction(x: 1.4); // 0.80956973617447581
            double icdf = levy.InverseDistributionFunction(p: cdf); // 1.4000000000000001

            double hf = levy.HazardFunction(x: 1.4); // 0.19557662859180974
            double chf = levy.CumulativeHazardFunction(x: 1.4); // 0.21125236235504694

            string str = levy.ToString(CultureInfo.InvariantCulture); // Lévy(x; μ = 0, c = 2.4)

            // Tested against GNU R's rmutils package
            //
            // dlevy(1.4, m=0, s=2.4)
            // [1] 0.1583329196109648229207
            //
            // plevy(1.4, m=0, s=2.4)
            // [1] 0.1904302638255241930665


            Assert.AreEqual(Double.PositiveInfinity, mean);
            Assert.AreEqual(5.2754624119625602, median);
            Assert.AreEqual(0.79999999999999993, mode);
            Assert.AreEqual(Double.PositiveInfinity, var);
            Assert.AreEqual(0.21125236235504694, chf);
            Assert.AreEqual(0.1904302638255241930665, cdf, 1e-10); // R
            Assert.AreEqual(0.1583329196109648229207, pdf, 1e-10); // R
            Assert.AreEqual(-1.8430553766023994, lpdf);
            Assert.AreEqual(0.19557662859180974, hf);
            Assert.AreEqual(0.80956973617447581, ccdf);
            Assert.AreEqual(1.4, icdf, 1e-6);
            Assert.AreEqual("Lévy(x; μ = 0, c = 2.4)", str);

            double p = levy.DistributionFunction(levy.Median);
            Assert.AreEqual(0.5, p, 1e-10);
            Assert.IsFalse(Double.IsNaN(p));

            var range1 = levy.GetRange(0.95);
            var range2 = levy.GetRange(0.99);
            var range3 = levy.GetRange(0.01);

            Assert.AreEqual(0.62476265190481373, range1.Min);
            Assert.AreEqual(610.35466692134071, range1.Max);
            Assert.AreEqual(0.36172379832273538, range2.Min);
            Assert.AreEqual(15278.074524254929, range2.Max);
            Assert.AreEqual(0.36172379832273538, range3.Min);
            Assert.AreEqual(15278.074524254929, range3.Max);
        }


        [Test]
        public void MedianTest1()
        {
            // plevy(5.2754624119625602, m=0, s=2.4)
            // [1] 0.5

            var levy = new LevyDistribution(location: 0, scale: 2.4);

            double expected = 5.2754624119625602;
            double actual = levy.Median;

            Assert.AreEqual(expected, actual);

            double p = levy.DistributionFunction(levy.Median);
            Assert.AreEqual(0.5, p, 1e-10);
            Assert.IsFalse(Double.IsNaN(p));
        }

        [Test]
        public void MedianTest2()
        {
            // plevy(7.2754624119625602, m=2, s=2.4)
            // [1] 0.5

            var levy = new LevyDistribution(location: 2, scale: 2.4);

            double expected = 7.2754624119625602;
            double actual = levy.Median;

            Assert.AreEqual(expected, actual);

            double p = levy.DistributionFunction(levy.Median);
            Assert.AreEqual(0.5, p, 1e-10);
            Assert.IsFalse(Double.IsNaN(p));
        }
    }
}
