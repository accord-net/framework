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
    using System;
    using System.Globalization;

    [TestClass()]
    public class InverseGammaDistributionTest
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
        public void InverseGammaDistributionConstructorTest()
        {
            double actual, expected;

            {
                InverseGammaDistribution target = new InverseGammaDistribution(4, 2);
                actual = target.ProbabilityDensityFunction(-2);
                expected = 0;
                Assert.AreEqual(expected, actual);

                actual = target.ProbabilityDensityFunction(5);
                expected = 0.000572006;
                Assert.AreEqual(expected, actual, 1e-6);
                Assert.IsFalse(Double.IsNaN(actual));

                actual = target.ProbabilityDensityFunction(0.42);
                expected = 1.74443;
                Assert.AreEqual(expected, actual, 1e-6);
                Assert.IsFalse(Double.IsNaN(actual));
            }

            {
                InverseGammaDistribution target = new InverseGammaDistribution(2.4, 0.42);
                actual = target.ProbabilityDensityFunction(0);
                expected = 0;
                Assert.AreEqual(expected, actual);
                Assert.IsFalse(Double.IsNaN(actual));

                actual = target.ProbabilityDensityFunction(0.3);
                expected = 1.4838600;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));

                actual = target.ProbabilityDensityFunction(0.42);
                expected = 0.705140;
                Assert.AreEqual(expected, actual, 1e-6);
                Assert.IsFalse(Double.IsNaN(actual));
            }
        }

        [TestMethod()]
        public void InverseGammaDistributionConstructorTest2()
        {
            var invGamma = new InverseGammaDistribution(shape: 0.42, scale: 0.5);

            double mean = invGamma.Mean;     // -0.86206896551724133
            double median = invGamma.Median; // 3.1072323347401709
            double var = invGamma.Variance;  // -0.47035626665061164

            double cdf = invGamma.DistributionFunction(x: 0.27); // 0.042243552114989695
            double pdf = invGamma.ProbabilityDensityFunction(x: 0.27); // 0.35679850067181362
            double lpdf = invGamma.LogProbabilityDensityFunction(x: 0.27); // -1.0305840804381006
            double ccdf = invGamma.ComplementaryDistributionFunction(x: 0.27); // 0.95775644788501035
            double icdf = invGamma.InverseDistributionFunction(p: cdf); // 0.26999994629410995

            double hf = invGamma.HazardFunction(x: 0.27); // 0.3725357333377633
            double chf = invGamma.CumulativeHazardFunction(x: 0.27); // 0.043161763098266373

            string str = invGamma.ToString(CultureInfo.InvariantCulture); // Γ^(-1)(x; α = 0.42, β = 0.5)

            Assert.AreEqual(-0.86206896551724133, mean);
            Assert.AreEqual(3.1072323347401709, median, 1e-7);
            Assert.AreEqual(-0.47035626665061164, var);
            Assert.AreEqual(0.043161763098266373, chf);
            Assert.AreEqual(0.042243552114989695, cdf, 1e-10);
            Assert.AreEqual(0.35679850067181362, pdf);
            Assert.AreEqual(-1.0305840804381006, lpdf);
            Assert.AreEqual(0.3725357333377633, hf);
            Assert.AreEqual(0.95775644788501035, ccdf);
            Assert.AreEqual(0.27, icdf, 1e-8);
            Assert.AreEqual("Γ^(-1)(x; α = 0.42, β = 0.5)", str);

            double p05 = invGamma.DistributionFunction(median);
            Assert.AreEqual(0.5, p05, 1e-6);
        }

        [TestMethod()]
        public void MedianTest()
        {
            var target = new InverseGammaDistribution(shape: 4.2, scale: 7.8);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }
    }
}
