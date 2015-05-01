// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Globalization;

    [TestClass()]
    public class AndersonDarlingDistributionTest
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
        public void ConstructorTest()
        {
            // Create a new Anderson Darling distribution (A²) for comparing against a Gaussian
            var a2 = new AndersonDarlingDistribution(AndersonDarlingDistributionType.Normal, 30);

            double median = a2.Median; // 0.33089957635450062

            double chf = a2.CumulativeHazardFunction(x: 0.27); // 0.42618068373640966
            double cdf = a2.DistributionFunction(x: 0.27); // 0.34700165471995292
            double ccdf = a2.ComplementaryDistributionFunction(x: 0.27); // 0.65299834528004708
            double icdf = a2.InverseDistributionFunction(p: cdf); // 0.27000000012207787

            string str = a2.ToString(CultureInfo.InvariantCulture); // "A²(x; n = 30)"

            Assert.AreEqual(0.33089957635450062, median);
            Assert.AreEqual(0.42618068373640966, chf);
            Assert.AreEqual(0.34700165471995292, cdf);
            Assert.AreEqual(0.65299834528004708, ccdf);
            Assert.AreEqual(0.27, icdf, 1e-6);
            Assert.AreEqual("A²(x; n = 30)", str);

            Assert.IsFalse(Double.IsNaN(median));

            var range1 = a2.GetRange(0.95);
            var range2 = a2.GetRange(0.99);
            var range3 = a2.GetRange(0.01);

            Assert.AreEqual(0.1552371857564955, range1.Min);
            Assert.AreEqual(0.73303544269106991, range1.Max);
            Assert.AreEqual(0.11507415861447158, range2.Min);
            Assert.AreEqual(1.0, range2.Max);
            Assert.AreEqual(0.1150741586144715, range3.Min);
            Assert.AreEqual(1.0, range3.Max);
        }

    }
}
