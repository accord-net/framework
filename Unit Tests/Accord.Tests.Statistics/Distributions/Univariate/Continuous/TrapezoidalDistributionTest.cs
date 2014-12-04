// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Ashley Messer, 2014
// glyphard at gmail.com
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

namespace Accord.Tests.Statistics.Distributions.Univariate.Continuous
{
    using System;
    using Accord.Statistics.Distributions.Univariate.Continuous;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Globalization;

    [TestClass]
    public class TrapezoidalDistributionTest
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

        [TestMethod]
        public void TrapezoidalDistributionConstructorTest()
        {
            double x = 0.75d;

            double a = 0;
            double b = (1.0d/3.0d);
            double c = (2.0d/3.0d);
            double d = 1.0d;
            double n1 = 2.0d;
            double n3 = 2.0d;

            var trapDist = new TrapezoidalDistribution(a, b, c, d, n1, n3);
            double mean = trapDist.Mean; //0.62499999999999989
            double variance = trapDist.Variance; //0.37103174603174593
            double pdf = trapDist.ProbabilityDensityFunction(x); //1.1249999999999998
            double cdf = trapDist.DistributionFunction(x); //1.28125
            string tostr = trapDist.ToString("N2", CultureInfo.InvariantCulture);

            Assert.AreEqual(mean, 0.625d, 0.00000001);
            Assert.AreEqual(variance, 0.37103175d, 0.00000001);
            Assert.AreEqual(pdf, 1.125, 0.000000001, "should match output from dtrapezoid in R");
            Assert.AreEqual(cdf, 1.28125,0.000000001);
            Assert.AreEqual(tostr, "Trapezoidal(x; a = 0.00, b = 0.33, c = 0.67, d = 1.00, n1 = 2.00, n3 = 2.00, α = 1.00)");
            //Verified using R package 'trapezoid'
        }

        [TestMethod()]
        public void DocumentationTest1()
        {
            // Create a new trapezoidal distribution with linear growth between
            // 0 and 2, stability between 2 and 8, and decrease between 8 and 10.
            //
            //
            //            +-----------+
            //           /|           |\
            //          / |           | \
            //         /  |           |  \
            //  -------+---+-----------+---+-------
            //   ...   0   2   4   6   8   10  ...
            //
            var trapz = new TrapezoidalDistribution(a: 0, b: 2, c: 8, d: 10, n1: 1, n3: 1);

            double mean = trapz.Mean;     // 2.25
            double median = trapz.Median; // 3.0
            double mode = trapz.Mode;     // 3.1353457616424696
            double var = trapz.Variance;  // 17.986666666666665

            double cdf = trapz.DistributionFunction(x: 1.4);       // 0.13999999999999999
            double pdf = trapz.ProbabilityDensityFunction(x: 1.4); // 0.10000000000000001
            double lpdf = trapz.LogProbabilityDensityFunction(x: 1.4); // -2.3025850929940455

            double ccdf = trapz.ComplementaryDistributionFunction(x: 1.4); // 0.85999999999999999
            double icdf = trapz.InverseDistributionFunction(p: cdf); // 1.3999999999999997

            double hf = trapz.HazardFunction(x: 1.4); // 0.11627906976744187
            double chf = trapz.CumulativeHazardFunction(x: 1.4); // 0.15082288973458366

            string str = trapz.ToString(CultureInfo.InvariantCulture); // Trapezoidal(x; a=0, b=2, c=8, d=10, n1=1, n3=1, α = 1)

            Assert.AreEqual(2.25, mean);
            Assert.AreEqual(3.0, median);
            Assert.AreEqual(3.1353457616424696, mode);
            Assert.AreEqual(17.986666666666665, var);
            Assert.AreEqual(0.15082288973458366, chf);
            Assert.AreEqual(0.13999999999999999, cdf);
            Assert.AreEqual(0.10000000000000001, pdf);
            Assert.AreEqual(-2.3025850929940455, lpdf);
            Assert.AreEqual(0.11627906976744187, hf);
            Assert.AreEqual(0.85999999999999999, ccdf);
            Assert.AreEqual(1.3999999999999997, icdf);
            Assert.AreEqual("Trapezoidal(x; a = 0, b = 2, c = 8, d = 10, n1 = 1, n3 = 1, α = 1)", str);

            var range1 = trapz.GetRange(0.95);
            var range2 = trapz.GetRange(0.99);
            var range3 = trapz.GetRange(0.01);

            Assert.AreEqual(0.50000000000000044, range1.Min);
            Assert.AreEqual(5.7000000000000002, range1.Max);
            Assert.AreEqual(0.10000000000000009, range2.Min);
            Assert.AreEqual(5.9400000000000004, range2.Max);
            Assert.AreEqual(0.10000000000000001, range3.Min);
            Assert.AreEqual(5.9400000000000004, range3.Max);
        }
    }
}
