// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class HyperbolicSecantTest
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



        [TestMethod()]
        public void ConstructorTest1()
        {
            var sech = new HyperbolicSecantDistribution();

            double mean = sech.Mean;     // 0.0
            double median = sech.Median; // 0.0
            double mode = sech.Mode;     // 0.0
            double var = sech.Variance;  // 1.0

            double cdf = sech.DistributionFunction(x: 1.4); // 0.92968538268895873
            double pdf = sech.ProbabilityDensityFunction(x: 1.4); // 0.10955386512899701
            double lpdf = sech.LogProbabilityDensityFunction(x: 1.4); // -2.2113389316917877

            double ccdf = sech.ComplementaryDistributionFunction(x: 1.4); // 0.070314617311041272
            double icdf = sech.InverseDistributionFunction(p: cdf); // 1.4000000017042411

            double hf = sech.HazardFunction(x: 1.4); // 1.5580524977385339

            string str = sech.ToString(); // Sech(x)

            Assert.AreEqual(0, mean);
            Assert.AreEqual(0, median);
            Assert.AreEqual(1, var);
            Assert.AreEqual(0, mode);
            Assert.AreEqual(0.92968538268895873, cdf);
            Assert.AreEqual(0.10955386512899701, pdf);
            Assert.AreEqual(-2.2113389316917877, lpdf);
            Assert.AreEqual(1.5580524977385339, hf);
            Assert.AreEqual(0.070314617311041272, ccdf);
            Assert.AreEqual(1.4000000017042411, icdf);
            Assert.AreEqual("Sech(x)", str);

            Assert.IsFalse(double.IsNaN(icdf));

            var range1 = sech.GetRange(0.95);
            var range2 = sech.GetRange(0.99);
            var range3 = sech.GetRange(0.01);

            Assert.AreEqual(-1.6183450347411152, range1.Min);
            Assert.AreEqual(1.6183450347411155, range1.Max);
            Assert.AreEqual(-2.6442035634463368, range2.Min);
            Assert.AreEqual(2.6442035634463381, range2.Max);
            Assert.AreEqual(-2.6442035634463372, range3.Min);
            Assert.AreEqual(2.6442035634463381, range3.Max);
        }

    }
}
