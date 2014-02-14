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
    using Accord.Math;

    [TestClass()]
    public class MannWhitneyDistributionTest
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
            double[] ranks = { 1, 2, 3, 4, 5 };

            var mannWhitney = new MannWhitneyDistribution(ranks, n1: 2, n2: 3);

            double mean = mannWhitney.Mean;     // 2.7870954605658511
            double median = mannWhitney.Median; // 1.5219615583481305
            double var = mannWhitney.Variance;  // 18.28163603621158

            double cdf = mannWhitney.DistributionFunction(x: 4); // 0.6
            double pdf = mannWhitney.ProbabilityDensityFunction(x: 4); // 0.2
            double lpdf = mannWhitney.LogProbabilityDensityFunction(x: 4); // -1.6094379124341005

            double ccdf = mannWhitney.ComplementaryDistributionFunction(x: 4); // 0.4
            double icdf = mannWhitney.InverseDistributionFunction(p: cdf); // 3.6666666666666661

            double hf = mannWhitney.HazardFunction(x: 4); // 0.5
            double chf = mannWhitney.CumulativeHazardFunction(x: 4); // 0.916290731874155

            string str = mannWhitney.ToString(); // MannWhitney(u; n1 = 2, n2 = 3)

            Assert.AreEqual(3.0, mean);
            Assert.AreEqual(3.0000006357828775, median);
            Assert.AreEqual(3.0, var);
            Assert.AreEqual(0.916290731874155, chf);
            Assert.AreEqual(0.6, cdf);
            Assert.AreEqual(0.2, pdf);
            Assert.AreEqual(-1.6094379124341005, lpdf);
            Assert.AreEqual(0.5, hf);
            Assert.AreEqual(0.4, ccdf);
            Assert.AreEqual(3.6666666666666661, icdf);
            Assert.AreEqual("MannWhitney(u; n1 = 2, n2 = 3)", str);
        }

        [TestMethod()]
        public void ProbabilityDensityFunctionTest()
        {
            double[] ranks = { 1, 2, 3, 4, 5 };

            int n1 = 2;
            int n2 = 3;
            int N = n1 + n2;

            Assert.AreEqual(N, ranks.Length);


            var target = new MannWhitneyDistribution(ranks, n1, n2);

            // Number of possible combinations is 5!/(3!2!) = 10.
            int nc = (int)Special.Binomial(5, 3);

            Assert.AreEqual(10, nc);

            double[] expected = { 0.1, 0.1, 0.2, 0.2, 0.2, 0.1, 0.1, 0.0, 0.0 };
            double sum = 0;

            for (int i = 0; i < expected.Length; i++)
            {
                // P(U=i)
                double actual = target.ProbabilityDensityFunction(i);
                Assert.AreEqual(expected[i], actual);
                sum += actual;
            }

            Assert.AreEqual(1, sum);
        }

        [TestMethod()]
        public void DistributionFunctionTest()
        {
            double[] ranks = { 1, 2, 3, 4, 5 };

            int n1 = 2;
            int n2 = 3;
            int N = n1 + n2;

            Assert.AreEqual(N, ranks.Length);


            var target = new MannWhitneyDistribution(ranks, n1, n2);

            // Number of possible combinations is 5!/(3!2!) = 10.
            int nc = (int)Special.Binomial(5, 3);

            Assert.AreEqual(10, nc);

            double[] expected = { 0.0, 0.1, 0.1, 0.2, 0.2, 0.2, 0.1, 0.1, 0.0, 0.0 };
            expected = expected.CumulativeSum();

            for (int i = 0; i < expected.Length; i++)
            {
                // P(U<=i)
                double actual = target.DistributionFunction(i);
                Assert.AreEqual(expected[i], actual,1e-10);
            }

        }

        [TestMethod()]
        public void MedianTest()
        {
            double[] ranks = { 1, 1, 2, 3, 4, 7, 5 };

            int n1 = 4;
            int n2 = 3;

            Assert.AreEqual(n1 + n2, ranks.Length);


            var target = new MannWhitneyDistribution(ranks, n1, n2);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }
    }
}
