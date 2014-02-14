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
    using Accord.Math;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Univariate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Globalization;

    [TestClass()]
    public class GeometricDistributionTest
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
            // Create a Geometric distribution with 42% success probability
            var dist = new GeometricDistribution(probabilityOfSuccess: 0.42);

            double mean = dist.Mean;     // 1.3809523809523812
            double median = dist.Median; // 1
            double var = dist.Variance;  // 3.2879818594104315
            double mode = dist.Mode;     // 0

            double cdf = dist.DistributionFunction(k: 2);               // 0.80488799999999994
            double ccdf = dist.ComplementaryDistributionFunction(k: 2); // 0.19511200000000006

            double pdf1 = dist.ProbabilityMassFunction(k: 0); // 0.42
            double pdf2 = dist.ProbabilityMassFunction(k: 1); // 0.2436
            double pdf3 = dist.ProbabilityMassFunction(k: 2); // 0.141288

            double lpdf = dist.LogProbabilityMassFunction(k: 2); // -1.956954918588067

            int icdf1 = dist.InverseDistributionFunction(p: 0.17); // 0
            int icdf2 = dist.InverseDistributionFunction(p: 0.46); // 1
            int icdf3 = dist.InverseDistributionFunction(p: 0.87); // 3

            double hf = dist.HazardFunction(x: 0); // 0.72413793103448265
            double chf = dist.CumulativeHazardFunction(x: 0); // 0.54472717544167193

            string str = dist.ToString(CultureInfo.InvariantCulture); // "Geometric(x; p = 0.42)"

            Assert.AreEqual(1.3809523809523812, mean);
            Assert.AreEqual(1, median);
            Assert.AreEqual(0, mode);
            Assert.AreEqual(3.2879818594104315, var);
            Assert.AreEqual(0.54472717544167193, chf, 1e-10);
            Assert.AreEqual(0.80488799999999994, cdf);
            Assert.AreEqual(0.42, pdf1);
            Assert.AreEqual(0.2436, pdf2);
            Assert.AreEqual(0.14128800000000002, pdf3);
            Assert.AreEqual(-1.956954918588067, lpdf);
            Assert.AreEqual(0.72413793103448265, hf);
            Assert.AreEqual(0.19511200000000006, ccdf);
            Assert.AreEqual(0, icdf1);
            Assert.AreEqual(1, icdf2);
            Assert.AreEqual(3, icdf3);
            Assert.AreEqual("Geometric(x; p = 0.42)", str);
        }

        [TestMethod()]
        public void GeometricDistributionConstructorTest()
        {
            double successProbability = 0.9;
            GeometricDistribution target = new GeometricDistribution(successProbability);
            Assert.AreEqual(0.9, target.ProbabilityOfSuccess);
            Assert.AreEqual((1 - 0.9) / 0.9, target.Mean);
            Assert.AreEqual((1 - 0.9) / (0.9 * 0.9), target.Variance);
        }

        [TestMethod()]
        public void CloneTest()
        {
            double successProbability = 1;
            GeometricDistribution target = new GeometricDistribution(successProbability);
            GeometricDistribution actual = (GeometricDistribution)target.Clone();

            Assert.AreNotEqual(target, actual);
            Assert.AreNotSame(target, actual);

            Assert.AreEqual(target.ProbabilityOfSuccess, actual.ProbabilityOfSuccess);
        }

        [TestMethod()]
        public void DistributionFunctionTest()
        {
            double successProbability = 0.42;
            GeometricDistribution target = new GeometricDistribution(successProbability);

            double[] values = { -1, 0, 1, 2, 3, 4, 5 };
            double[] expected = { 0, 0.42, 0.6636, 0.804888, 0.88683504, 0.9343643232, 0.961931307456 };


            for (int i = 0; i < values.Length; i++)
            {
                double actual = target.DistributionFunction(i - 1);
                Assert.AreEqual(expected[i], actual, 1e-10);
                Assert.IsFalse(Double.IsNaN(actual));
            }
        }

        [TestMethod()]
        public void FitTest()
        {
            double successProbability = 0;
            GeometricDistribution target = new GeometricDistribution(successProbability);

            double[] observations = { 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0 };
            double[] weights = null;
            IFittingOptions options = null;

            target.Fit(observations, weights, options);

            Assert.AreEqual(1 / (1 - 4 / 12.0), target.ProbabilityOfSuccess);
        }

        [TestMethod()]
        public void FitTest2()
        {
            double successProbability = 0;
            GeometricDistribution target = new GeometricDistribution(successProbability);

            double[] observations = { 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0 };
            double[] weights = { 0, 1, 1, 2, 0, 2, 1, 0, 0, 0, 1, 0 };
            weights = weights.Divide(weights.Sum());
            IFittingOptions options = null;

            target.Fit(observations, weights, options);

            Assert.AreEqual(1 / (1 - 4.0 / 8.0), target.ProbabilityOfSuccess);
        }


        [TestMethod()]
        public void ProbabilityMassFunctionTest()
        {
            double successProbability = 0.42;
            GeometricDistribution target = new GeometricDistribution(successProbability);

            double[] expected = { 0, 0.42, 0.2436, 0.141288, 0.08194704, 0.0475292832, 0.027566984256 };

            for (int i = 0; i < expected.Length; i++)
            {
                double actual = target.ProbabilityMassFunction(i - 1);
                Assert.AreEqual(expected[i], actual, 1e-10);
                Assert.IsFalse(Double.IsNaN(actual));
            }
        }

        [TestMethod()]
        public void MedianTest()
        {
            {
                var target = new GeometricDistribution(0.2);
                Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
            }

            {
                var target = new GeometricDistribution(0.6);
                Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
            }

            {
                var target = new GeometricDistribution(0.000001);
                Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
            }

            {
                var target = new GeometricDistribution(0.99999);
                Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
            }
        }
    }
}
