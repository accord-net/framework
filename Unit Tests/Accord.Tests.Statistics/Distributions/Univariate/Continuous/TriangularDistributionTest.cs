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
    using Accord.Statistics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Statistics.Distributions.Multivariate;
    using System.Globalization;

    [TestClass()]
    public class TriangularDistributionTest
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
            var trig = new TriangularDistribution(a: 1, b: 6, c: 3);

            double mean = trig.Mean;     // 3.3333333333333335
            double median = trig.Median; // 3.2613872124741694
            double mode = trig.Mode;     // 3.0
            double var = trig.Variance;  // 1.0555555555555556

            double cdf = trig.DistributionFunction(x: 2); // 0.10000000000000001
            double pdf = trig.ProbabilityDensityFunction(x: 2); // 0.20000000000000001
            double lpdf = trig.LogProbabilityDensityFunction(x: 2); // -1.6094379124341003

            double ccdf = trig.ComplementaryDistributionFunction(x: 2); // 0.90000000000000002
            double icdf = trig.InverseDistributionFunction(p: cdf); // 2.0000000655718773

            double hf = trig.HazardFunction(x: 2); // 0.22222222222222224
            double chf = trig.CumulativeHazardFunction(x: 2); // 0.10536051565782628

            string str = trig.ToString(CultureInfo.InvariantCulture); // Triangular(x; a = 1, b = 6, c = 3)

            Assert.AreEqual(3.3333333333333335, mean);
            Assert.AreEqual(3.2613872124741694, median);
            Assert.AreEqual(1.0555555555555556, var);
            Assert.AreEqual(0.10536051565782628, chf);
            Assert.AreEqual(0.10000000000000001, cdf);
            Assert.AreEqual(0.20000000000000001, pdf);
            Assert.AreEqual(-1.6094379124341003, lpdf);
            Assert.AreEqual(0.22222222222222224, hf);
            Assert.AreEqual(0.90000000000000002, ccdf);
            Assert.AreEqual(2.0000000655718773, icdf);
            Assert.AreEqual("Triangular(x; a = 1, b = 6, c = 3)", str);
        }

        [TestMethod()]
        public void GenerateTest2()
        {

        }
    }
}
