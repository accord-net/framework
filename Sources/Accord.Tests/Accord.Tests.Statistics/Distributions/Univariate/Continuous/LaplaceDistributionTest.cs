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
    public class LaplaceDistributionTest
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
        public void LaplaceDistributionConstructorTest()
        {
            {
                LaplaceDistribution target = new LaplaceDistribution(0, 0.2);

                double[] expected = { 2.5, 1.5163266, 0.919699, 0.557825, 0.338338 };

                for (int i = 0; i < expected.Length; i++)
                {

                    double x = i / 10.0;
                    double actual = target.ProbabilityDensityFunction(x);
                    Assert.AreEqual(expected[i], actual, 1e-6);
                    Assert.IsFalse(double.IsNaN(actual));
                }
            }

            {
                LaplaceDistribution target = new LaplaceDistribution(-2, 5.79);

                double[] expected = { 0.0666469, 0.0655057, 0.06438406, 0.0632816234, 0.062198060, 0.061133051 };

                for (int i = 0; i < expected.Length; i++)
                {

                    double x = (i - 5) / 10.0;
                    double actual = target.ProbabilityDensityFunction(x);
                    Assert.AreEqual(expected[i], actual, 1e-8);
                    Assert.IsFalse(double.IsNaN(actual));
                }
            }
        }

        [TestMethod()]
        public void ConstructorTest2()
        {
            var laplace = new LaplaceDistribution(location: 4, scale: 2);

            double mean = laplace.Mean;     // 4.0
            double median = laplace.Median; // 4.0
            double var = laplace.Variance;  // 8.0

            double cdf = laplace.DistributionFunction(x: 0.27); // 0.077448104942453522
            double pdf = laplace.ProbabilityDensityFunction(x: 0.27); // 0.038724052471226761
            double lpdf = laplace.LogProbabilityDensityFunction(x: 0.27); // -3.2512943611198906

            double ccdf = laplace.ComplementaryDistributionFunction(x: 0.27); // 0.92255189505754642
            double icdf = laplace.InverseDistributionFunction(p: cdf); // 0.27

            double hf = laplace.HazardFunction(x: 0.27); // 0.041974931360160776
            double chf = laplace.CumulativeHazardFunction(x: 0.27); // 0.080611649844768624

            string str = laplace.ToString(CultureInfo.InvariantCulture); // Laplace(x; μ = 4, b = 2)

            Assert.AreEqual(4.0, mean);
            Assert.AreEqual(4.0, median);
            Assert.AreEqual(8.0, var);
            Assert.AreEqual(0.080611649844768624, chf);
            Assert.AreEqual(0.077448104942453522, cdf);
            Assert.AreEqual(0.038724052471226761, pdf);
            Assert.AreEqual(-3.2512943611198906, lpdf);
            Assert.AreEqual(0.041974931360160776, hf);
            Assert.AreEqual(0.92255189505754642, ccdf);
            Assert.AreEqual(0.26999999840794775, icdf);
            Assert.AreEqual("Laplace(x; μ = 4, b = 2)", str);
        }

        [TestMethod()]
        public void MedianTest()
        {
            var target = new LaplaceDistribution(location: 2, scale: 0.42);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }

    }
}
