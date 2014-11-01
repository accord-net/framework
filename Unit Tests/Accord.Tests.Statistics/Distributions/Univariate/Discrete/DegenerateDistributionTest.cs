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
    using Accord.Statistics.Distributions.Univariate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Math;
    using System.Globalization;

    [TestClass()]
    public class DegenerateDistributionTest
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
            var dist = new DegenerateDistribution(value: 2);

            double mean = dist.Mean;     // 2
            double median = dist.Median; // 2
            double mode = dist.Mode;     // 2
            double var = dist.Variance;  // 1

            double cdf1 = dist.DistributionFunction(k: 1);    // 0
            double cdf2 = dist.DistributionFunction(k: 2);   // 1

            double pdf1 = dist.ProbabilityMassFunction(k: 1); // 0
            double pdf2 = dist.ProbabilityMassFunction(k: 2); // 1
            double pdf3 = dist.ProbabilityMassFunction(k: 3); // 0

            double lpdf = dist.LogProbabilityMassFunction(k: 2); // 0
            double ccdf = dist.ComplementaryDistributionFunction(k: 2); // 0.0

            int icdf1 = dist.InverseDistributionFunction(p: 0.0); // 3
            int icdf2 = dist.InverseDistributionFunction(p: 0.5); // 3
            int icdf3 = dist.InverseDistributionFunction(p: 1.0); // 2

            double hf = dist.HazardFunction(x: 0); // 0.0
            double chf = dist.CumulativeHazardFunction(x: 0); // 0.0

            string str = dist.ToString(CultureInfo.InvariantCulture); // Degenerate(x; k0 = 2)

            Assert.AreEqual(2, mean);
            Assert.AreEqual(2, median);
            Assert.AreEqual(2, mode);
            Assert.AreEqual(1, var);
            Assert.AreEqual(0, chf);
            Assert.AreEqual(0, cdf1);
            Assert.AreEqual(1, cdf2);
            Assert.AreEqual(0, pdf1);
            Assert.AreEqual(1, pdf2);
            Assert.AreEqual(0, pdf3);
            Assert.AreEqual(0, lpdf);
            Assert.AreEqual(0, hf);
            Assert.AreEqual(0, ccdf);
            Assert.AreEqual(3, icdf1);
            Assert.AreEqual(3, icdf2);
            Assert.AreEqual(2, icdf3);
            Assert.AreEqual("Degenerate(x; k0 = 2)", str);

            var range1 = dist.GetRange(0.95);
            var range2 = dist.GetRange(0.99);
            var range3 = dist.GetRange(0.01);

            Assert.AreEqual(3.0, range1.Min);
            Assert.AreEqual(3.0, range1.Max);
            Assert.AreEqual(3.0, range2.Min);
            Assert.AreEqual(3.0, range2.Max);
            Assert.AreEqual(3.0, range3.Min);
            Assert.AreEqual(3.0, range3.Max);
        }
    }
}
