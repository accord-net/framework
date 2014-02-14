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
    using Accord.Statistics.Moving;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Statistics;
    using Accord.Statistics.Distributions.Univariate;
    using System;
    using System.Globalization;

    [TestClass()]
    public class WeibullDistributionTest
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
            WeibullDistribution n = new WeibullDistribution(0.807602, 12.5);
            Assert.AreEqual(14.067993598321863, n.Mean);
            Assert.AreEqual(17.552908226174811, n.StandardDeviation);
            Assert.IsFalse(Double.IsNaN(n.Mean));
            Assert.IsFalse(Double.IsNaN(n.Variance));
        }

        [TestMethod()]
        public void ConstructorTest2()
        {
            var weilbull = new WeibullDistribution(scale: 0.42, shape: 1.2);

            double mean = weilbull.Mean;     // 0.39507546046784414
            double median = weilbull.Median; // 0.30945951550913292
            double var = weilbull.Variance;  // 0.10932249666369542
            double mode = weilbull.Mode;     // 0.094360430821809421

            double cdf = weilbull.DistributionFunction(x: 1.4); // 0.98560487188700052
            double pdf = weilbull.ProbabilityDensityFunction(x: 1.4); // 0.052326687031379278
            double lpdf = weilbull.LogProbabilityDensityFunction(x: 1.4); // -2.9502487697674415

            double ccdf = weilbull.ComplementaryDistributionFunction(x: 1.4); // 0.22369885565908001
            double icdf = weilbull.InverseDistributionFunction(p: cdf); // 1.400000001051205

            double hf = weilbull.HazardFunction(x: 1.4); // 1.1093328057258516
            double chf = weilbull.CumulativeHazardFunction(x: 1.4); // 1.4974545260150962

            string str = weilbull.ToString(CultureInfo.InvariantCulture); // Weibull(x; λ = 0.42, k = 1.2)

            double imedian = weilbull.InverseDistributionFunction(p: 0.5);

            Assert.AreEqual(0.39507546046784414, mean);
            Assert.AreEqual(0.30945951550913292, median);
            Assert.AreEqual(0.094360430821809421, mode);
            Assert.AreEqual(0.3094595, imedian, 1e-5);
            Assert.AreEqual(0.10932249666369542, var);
            Assert.AreEqual(1.4974545260150962, chf);
            Assert.AreEqual(0.98560487188700052, cdf);
            Assert.AreEqual(0.052326687031379278, pdf);
            Assert.AreEqual(-2.9502487697674415, lpdf);
            Assert.AreEqual(1.1093328057258516, hf);
            Assert.AreEqual(0.22369885565908001, ccdf);
            Assert.AreEqual(1.40, icdf, 1e-6);
            Assert.AreEqual("Weibull(x; λ = 0.42, k = 1.2)", str);
        }

        [TestMethod()]
        public void ProbabilityDistributionTest()
        {
            WeibullDistribution n = new WeibullDistribution(0.80, 12.5);

            double[] expected = 
            {
                0.0,
                0.09289322,
                0.0733005, 
                0.06186956, 
                0.0537782,
                0.0475457 
            };

            double[] actual = new double[expected.Length];

            for (int i = 0; i < actual.Length; i++)
                actual[i] = n.ProbabilityDensityFunction(i);

            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 1e-5);
                Assert.IsFalse(double.IsNaN(actual[i]));
            }
        }

        [TestMethod()]
        public void CumulativeDistributionTest()
        {
            WeibullDistribution n = new WeibullDistribution(0.80, 12.5);

            double[] expected = 
            {
                0.0,
                0.1241655,
                0.2061272,
                0.2733265,
                0.3309536,
                0.3814949,
            };

            double[] actual = new double[expected.Length];

            for (int i = 0; i < actual.Length; i++)
                actual[i] = n.DistributionFunction(i);

            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 1e-6);
                Assert.IsFalse(double.IsNaN(actual[i]));
            }
        }

        [TestMethod()]
        public void MedianTest()
        {
            WeibullDistribution target = new WeibullDistribution(1.52, 0.6);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5), 1e-8);
        }
    }
}