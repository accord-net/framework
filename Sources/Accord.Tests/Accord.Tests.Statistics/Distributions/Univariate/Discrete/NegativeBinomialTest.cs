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
    public class NegativeBinomialTest
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
            // Create a Negative Binomial distribution with r = 7, p = 0.42
            var dist = new NegativeBinomialDistribution(failures: 7, probability: 0.42);

            // Common measures
            double mean = dist.Mean;     // 5.068965517241379
            double median = dist.Median; // 5.0
            double var = dist.Variance;  // 8.7395957193816862

            // Cumulative distribution functions
            double cdf = dist.DistributionFunction(k: 2);               // 0.19605133020527743
            double ccdf = dist.ComplementaryDistributionFunction(k: 2); // 0.80394866979472257

            // Probability mass functions
            double pmf1 = dist.ProbabilityMassFunction(k: 4); // 0.054786846293416853
            double pmf2 = dist.ProbabilityMassFunction(k: 5); // 0.069908015870399909
            double pmf3 = dist.ProbabilityMassFunction(k: 6); // 0.0810932984096639
            double lpmf = dist.LogProbabilityMassFunction(k: 2); // -2.3927801721315989

            // Quantile function
            int icdf1 = dist.InverseDistributionFunction(p: 0.17); // 2
            int icdf2 = dist.InverseDistributionFunction(p: 0.46); // 4
            int icdf3 = dist.InverseDistributionFunction(p: 0.87); // 8

            // Hazard (failure rate) functions
            double hf = dist.HazardFunction(x: 4); // 0.10490438293398294
            double chf = dist.CumulativeHazardFunction(x: 4); // 0.64959916255036043

            // String representation
            string str = dist.ToString(CultureInfo.InvariantCulture); // "NegativeBinomial(x; r = 7, p = 0.42)"

            Assert.AreEqual(5.068965517241379, mean);
            Assert.AreEqual(5.0, median);
            Assert.AreEqual(8.7395957193816862, var);
            Assert.AreEqual(0.64959916255036043, chf, 1e-10);
            Assert.AreEqual(0.19605133020527743, cdf);
            Assert.AreEqual(0.054786846293416853, pmf1);
            Assert.AreEqual(0.069908015870399909, pmf2);
            Assert.AreEqual(0.0810932984096639, pmf3);
            Assert.AreEqual(-3.8297538146412009, lpmf);
            Assert.AreEqual(0.10490438293398294, hf);
            Assert.AreEqual(0.80394866979472257, ccdf);
            Assert.AreEqual(2, icdf1);
            Assert.AreEqual(4, icdf2);
            Assert.AreEqual(8, icdf3);
            Assert.AreEqual("NegativeBinomial(x; r = 7, p = 0.42)", str);
        }

        [TestMethod()]
        public void NegativeBinomialConstructorTest()
        {
            double expected, actual;

            {
                NegativeBinomialDistribution target = new NegativeBinomialDistribution(6, 0.42);
                actual = target.ProbabilityMassFunction(-1);
                expected = 0.0;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));

                actual = target.ProbabilityMassFunction(0);
                expected = 0.00548903;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));

                actual = target.ProbabilityMassFunction(1);
                expected = 0.0191018;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));

                actual = target.ProbabilityMassFunction(2);
                expected = 0.0387767;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));

                actual = target.ProbabilityMassFunction(10);
                expected = 0.0710119;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));
            }
        }

        [TestMethod()]
        public void MedianTest()
        {
            for (int i = 0; i < 10; i++)
            {
                int failures = i + 1;
                {
                    var target = new NegativeBinomialDistribution(failures, 0.0);
                    Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
                }

                {
                    var target = new NegativeBinomialDistribution(failures, 0.7);
                    Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
                }

                {
                    var target = new NegativeBinomialDistribution(failures, 0.2);
                    Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
                }

                {
                    var target = new NegativeBinomialDistribution(failures, 1.0);
                    Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
                }
            }
        }
    }
}
