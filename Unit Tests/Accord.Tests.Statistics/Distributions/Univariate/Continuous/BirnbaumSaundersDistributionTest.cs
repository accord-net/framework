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
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics;
    using NUnit.Framework;
    using System;
    using Accord.Statistics.Distributions.Multivariate;
    using System.Globalization;

    [TestFixture]
    public class BirnbaumSaundersDistributionTest
    {


        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }



        [Test]
        public void ConstructorTest1()
        {
            var bs = new BirnbaumSaundersDistribution(shape: 0.42);

            double mean = bs.Mean;     // 1.0882000000000001
            double median = bs.Median; // 1.0
            double var = bs.Variance;  // 0.21529619999999997

            try { double mode = bs.Mode; Assert.Fail(); }
            catch { }

            double cdf = bs.DistributionFunction(x: 1.4); // 0.78956384911580346
            double pdf = bs.ProbabilityDensityFunction(x: 1.4); // 1.3618433601225426
            double lpdf = bs.LogProbabilityDensityFunction(x: 1.4); // 0.30883919386130815

            double ccdf = bs.ComplementaryDistributionFunction(x: 1.4); // 0.21043615088419654
            double icdf = bs.InverseDistributionFunction(p: cdf); // 2.0618330099769064

            double hf = bs.HazardFunction(x: 1.4); // 6.4715276077824093
            double chf = bs.CumulativeHazardFunction(x: 1.4); // 1.5585729930861034

            string str = bs.ToString(CultureInfo.InvariantCulture); // BirnbaumSaunders(x; μ = 0, β = 1, γ = 0.42)

            Assert.AreEqual(1.0882000000000001, mean);
            Assert.AreEqual(1.0, median);
            Assert.AreEqual(0.21529619999999997, var);
            Assert.AreEqual(1.5585729930861034, chf);
            Assert.AreEqual(0.78956384911580346, cdf);
            Assert.AreEqual(1.3618433601225426, pdf);
            Assert.AreEqual(0.30883919386130815, lpdf);
            Assert.AreEqual(6.4715276077824093, hf);
            Assert.AreEqual(0.21043615088419654, ccdf);
            Assert.AreEqual(2.0618330099769064, icdf);
            Assert.AreEqual("BirnbaumSaunders(x; μ = 0, β = 1, γ = 0.42)", str);

            var range1 = bs.GetRange(0.95);
            var range2 = bs.GetRange(0.99);
            var range3 = bs.GetRange(0.01);

            Assert.AreEqual(0.096563470711246477, range1.Min);
            Assert.AreEqual(3.8243723816964965, range1.Max);
            Assert.AreEqual(0.0079430014187131717, range2.Min);
            Assert.AreEqual(5.8345020446298692, range2.Max);
            Assert.AreEqual(0.0079430014187131717, range3.Min);
            Assert.AreEqual(5.8345020446298692, range3.Max);
        }

    }
}
