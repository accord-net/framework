// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
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
    using NUnit.Framework;
    using System;
    using Accord.Statistics.Distributions.Multivariate;
    using System.Globalization;

    [TestFixture]
    public class TukeyLambdaTest
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



        [Test]
        public void ConstructorTest()
        {
            var tukey = new TukeyLambdaDistribution(lambda: 0.14);

            double mean = tukey.Mean;     // 0.0
            double median = tukey.Median; // 0.0
            double mode = tukey.Mode;     // 0.0
            double var = tukey.Variance;  // 2.1102970222144855
            double stdDev = tukey.StandardDeviation;  // 1.4526861402982014

            double cdf = tukey.DistributionFunction(x: 1.4); // 0.83252947230217966
            double pdf = tukey.ProbabilityDensityFunction(x: 1.4); // 0.17181242109370659
            double lpdf = tukey.LogProbabilityDensityFunction(x: 1.4); // -1.7613519723149427

            double ccdf = tukey.ComplementaryDistributionFunction(x: 1.4); // 0.16747052769782034
            double icdf = tukey.InverseDistributionFunction(p: cdf); // 1.4000000000000004

            double hf = tukey.HazardFunction(x: 1.4); // 1.0219566231014163
            double chf = tukey.CumulativeHazardFunction(x: 1.4); // 1.7842102556452939

            string str = tukey.ToString(CultureInfo.InvariantCulture); // Tukey(x; λ = 0.14)

            Assert.AreEqual(0.0, mean);
            Assert.AreEqual(0.0, median);
            Assert.AreEqual(0.0, mode);
            Assert.AreEqual(2.1102970222144855, var);
            Assert.AreEqual(1.4526861402982014, stdDev);
            Assert.AreEqual(1.7869478972416082, chf);
            Assert.AreEqual(0.83252947230217966, cdf);
            Assert.AreEqual(0.17181242109370659, pdf);
            Assert.AreEqual(-1.7613519723149427, lpdf);
            Assert.AreEqual(1.0259263134569006, hf);
            Assert.AreEqual(0.16747052769782034, ccdf);
            Assert.AreEqual(1.4000000000000004, icdf);
            Assert.AreEqual("Tukey(x; λ = 0.14)", str);

            var range1 = tukey.GetRange(0.95);
            var range2 = tukey.GetRange(0.99);
            var range3 = tukey.GetRange(0.01);

            Assert.AreEqual(-2.395751074495986, range1.Min);
            Assert.AreEqual(2.395751074495986, range1.Max);
            Assert.AreEqual(-3.3841891582663117, range2.Min);
            Assert.AreEqual(3.3841891582663117, range2.Max);
            Assert.AreEqual(-3.3841891582663126, range3.Min);
            Assert.AreEqual(3.3841891582663117, range3.Max);
        }

        [Test]
        public void LogisticTest()
        {
            var target = new TukeyLambdaDistribution(lambda: 0);
            var logistic = new LogisticDistribution();

            compare(target, logistic, 1e-5);
        }

        [Test]
        public void UniformTest()
        {
            var target = new TukeyLambdaDistribution(lambda: 1);
            var uniform = new UniformContinuousDistribution(-1, +1);

            compare(target, uniform, 1e-10);
        }

        [Test]
        public void UniformTest2()
        {
            var target = new TukeyLambdaDistribution(lambda: 2);
            var uniform = new UniformContinuousDistribution(-0.5, +0.5);

            compare(target, uniform, 1e-10);
        }

        private static void compare(TukeyLambdaDistribution target, 
            UnivariateContinuousDistribution comparison, double tol)
        {
            Assert.AreEqual(comparison.Mean, target.Mean);
            Assert.AreEqual(comparison.Variance, target.Variance, tol);
            Assert.AreEqual(comparison.Entropy, target.Entropy, 1e-4);
            Assert.AreEqual(comparison.StandardDeviation, target.StandardDeviation, tol);
            Assert.AreEqual(comparison.Mode, target.Mode);
            Assert.AreEqual(comparison.Median, target.Median);

            for (double x = -10; x < 10; x += 0.0001)
            {
                double actual = target.ProbabilityDensityFunction(x);
                double expected = comparison.ProbabilityDensityFunction(x);
                Assert.AreEqual(expected, actual, tol);
                Assert.IsFalse(Double.IsNaN(actual));
            }

            for (double x = -10; x < 10; x += 0.0001)
            {
                double actual = target.DistributionFunction(x);
                double expected = comparison.DistributionFunction(x);
                Assert.AreEqual(expected, actual, tol);
                Assert.IsFalse(Double.IsNaN(actual));
            }

            for (double x = -10; x < 10; x += 0.0001)
            {
                double actual = target.LogProbabilityDensityFunction(x);
                double expected = comparison.LogProbabilityDensityFunction(x);
                Assert.AreEqual(expected, actual, tol);
                Assert.IsFalse(Double.IsNaN(actual));
            }
        }

    }
}
