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
    using Accord.Statistics.Distributions;
    using System.Globalization;

    [TestClass()]
    public class PoissonDistributionTest
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
            // Create a new Poisson distribution with 
            var dist = new PoissonDistribution(lambda: 4.2);

            // Common measures
            double mean = dist.Mean;     // 4.2
            double median = dist.Median; // 4.0
            double var = dist.Variance;  // 4.2

            // Cumulative distribution functions
            double cdf = dist.DistributionFunction(k: 2);               // 0.39488100648845126
            double ccdf = dist.ComplementaryDistributionFunction(k: 2); // 0.60511899351154874

            // Probability mass functions
            double pmf1 = dist.ProbabilityMassFunction(k: 4); // 0.19442365170822165
            double pmf2 = dist.ProbabilityMassFunction(k: 5); // 0.1633158674349062
            double pmf3 = dist.ProbabilityMassFunction(k: 6); // 0.11432110720443435
            double lpmf = dist.LogProbabilityMassFunction(k: 2); // -2.0229781299813

            // Quantile function
            int icdf1 = dist.InverseDistributionFunction(p: 0.17); // 2
            int icdf2 = dist.InverseDistributionFunction(p: 0.46); // 4
            int icdf3 = dist.InverseDistributionFunction(p: 0.87); // 7

            // Hazard (failure rate) functions
            double hf = dist.HazardFunction(x: 4); // 0.19780423301883465
            double chf = dist.CumulativeHazardFunction(x: 4); // 0.017238269667812049

            // String representation
            string str = dist.ToString(CultureInfo.InvariantCulture); // "Poisson(x; λ = 4.2)"


            // Median bounds
            // (http://en.wikipedia.org/wiki/Poisson_distribution#Median)

            Assert.IsTrue(median < 4.2 + 1 / 3.0);
            Assert.IsTrue(4.2 - System.Math.Log(2) <= median);

            Assert.AreEqual(4.2, mean);
            Assert.AreEqual(4.0, median);
            Assert.AreEqual(4.2, var);
            Assert.AreEqual(0.017238269667812049, chf, 1e-10);
            Assert.AreEqual(0.39488100648845126, cdf);
            Assert.AreEqual(0.19442365170822165, pmf1);
            Assert.AreEqual(0.1633158674349062, pmf2);
            Assert.AreEqual(0.11432110720443435, pmf3);
            Assert.AreEqual(-2.0229781299813, lpmf);
            Assert.AreEqual(0.19780423301883465, hf);
            Assert.AreEqual(0.60511899351154874, ccdf);
            Assert.AreEqual(2, icdf1);
            Assert.AreEqual(4, icdf2);
            Assert.AreEqual(7, icdf3);
            Assert.AreEqual("Poisson(x; λ = 4.2)", str);
        }

        [TestMethod()]
        public void FitTest()
        {
            PoissonDistribution target = new PoissonDistribution(1);
            double[] observations = { 0.2, 0.7, 1.0, 0.33 };

            target.Fit(observations);

            double expected = 0.5575;
            Assert.AreEqual(expected, target.Mean);
        }

        [TestMethod()]
        public void ProbabilityDensityFunctionTest()
        {
            PoissonDistribution target = new PoissonDistribution(25);

            double actual = target.ProbabilityMassFunction(20);
            double expected = 0.051917468608491321;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void LogProbabilityDensityFunctionTest()
        {
            PoissonDistribution target = new PoissonDistribution(25);

            double actual = target.LogProbabilityMassFunction(20);
            double expected = System.Math.Log(0.051917468608491321);

            Assert.AreEqual(expected, actual, 1e-10);
        }


        [TestMethod()]
        public void MedianTest()
        {
            for (int i = 0; i < 25; i++)
            {
                var target = new PoissonDistribution(i + 1);
                Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
            }
        }
    }
}
