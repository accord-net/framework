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
    using System.Globalization;
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;
    using System;
    using Accord.Math;

    [TestFixture]
    public class ShiftedLogLogisticDistributionTest
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
        public void ConstructorTest1()
        {
            var log = new ShiftedLogLogisticDistribution(location: 0, scale: 0.42, shape: 0.1);

            double mean = log.Mean;     // 0.069891101544818923
            double median = log.Median; // 0.0
            double mode = log.Mode;     // -0.083441677069328604
            double var = log.Variance;  // 0.62447259946747213

            double cdf = log.DistributionFunction(x: 1.4); // 0.94668863559417671
            double pdf = log.ProbabilityDensityFunction(x: 1.4); // 0.090123683626808615
            double lpdf = log.LogProbabilityDensityFunction(x: 1.4); // -2.4065722895662613

            double ccdf = log.ComplementaryDistributionFunction(x: 1.4); // 0.053311364405823292
            double icdf = log.InverseDistributionFunction(p: cdf); // 1.4000000037735139

            double hf = log.HazardFunction(x: 1.4); // 1.6905154207038875
            double chf = log.CumulativeHazardFunction(x: 1.4); // 2.9316057546685061

            string str = log.ToString(CultureInfo.InvariantCulture); // LLD3(x; μ = 0, σ = 0.42, ξ = 0.1)


            Assert.AreEqual(0.069891101544818923, mean, 1e-15);
            Assert.AreEqual(0.0, median);
            Assert.AreEqual(-0.083441677069328604, mode, 1e-15);
            Assert.AreEqual(0.62447259946747213, var, 1e-14);
            Assert.AreEqual(2.9316057546685061, chf, 1e-15);
            Assert.AreEqual(0.94668863559417671, cdf, 1e-15);
            Assert.AreEqual(0.090123683626808615, pdf, 1e-15);
            Assert.AreEqual(-2.4065722895662613, lpdf, 1e-15);
            Assert.AreEqual(1.6905154207038875, hf, 1e-15);
            Assert.AreEqual(0.053311364405823292, ccdf, 1e-15);
            Assert.AreEqual(1.4000000001930302, icdf, 1e-15);
            Assert.AreEqual("LLD3(x; μ = 0, σ = 0.42, ξ = 0.1)", str);


            var range1 = log.GetRange(0.95);
            var range2 = log.GetRange(0.99);
            var range3 = log.GetRange(0.01);

            Assert.AreEqual(-1.0712279538154128, range1.Min, 1e-15);
            Assert.AreEqual(1.43799453428117, range1.Max, 1e-15);
            Assert.AreEqual(-1.5473143609724813, range2.Min, 1e-15);
            Assert.AreEqual(2.4498647072756436, range2.Max, 1e-15);
            Assert.AreEqual(-1.5473143609724815, range3.Min, 1e-15);
            Assert.AreEqual(2.4498647072756436, range3.Max, 1e-15);
        }

        [Test]
        public void EquivalencyTest1()
        {
            // when mu = sigma / ksi, the shifted log-logistic reduces to the log-logistic distribution.

            double sigma = 4.2;     // scale
            double ksi = 0.42;        // shape
            double mu = sigma / ksi; // location

            var target = new ShiftedLogLogisticDistribution(location: mu, scale: sigma, shape: ksi);
            var log = LogLogisticDistribution.FromLocationShape(location: mu, shape: ksi);

            Assert.AreEqual(log.Mean, target.Mean, 1e-10);
            Assert.AreEqual(log.Median, target.Median);
            Assert.AreEqual(log.Mode, target.Mode, 1e-10);
            Assert.AreEqual(log.Variance, target.Variance, 1e-10);

            double actual, expected;

            for (double i = -10; i < 10; i += 0.1)
            {
                expected = log.DistributionFunction(i);
                actual = target.DistributionFunction(i);
                Assert.IsTrue(expected.IsRelativelyEqual(actual, 1e-5));

                expected = log.ProbabilityDensityFunction(i);
                actual = target.ProbabilityDensityFunction(i);
                Assert.IsTrue(expected.IsRelativelyEqual(actual, 1e-5));

                expected = log.LogProbabilityDensityFunction(i);
                actual = target.LogProbabilityDensityFunction(i);
                Assert.IsTrue(expected.IsRelativelyEqual(actual, 1e-5));

                expected = log.ComplementaryDistributionFunction(i);
                actual = target.ComplementaryDistributionFunction(i);
                Assert.IsTrue(expected.IsRelativelyEqual(actual, 1e-5));

                double p = log.DistributionFunction(i);
                expected = log.InverseDistributionFunction(p);
                actual = target.InverseDistributionFunction(p);
                Assert.AreEqual(expected, actual, 1e-5);

                expected = log.HazardFunction(i);
                actual = target.HazardFunction(i);
                Assert.IsTrue(expected.IsRelativelyEqual(actual, 1e-5));

                expected = log.CumulativeHazardFunction(i);
                actual = target.CumulativeHazardFunction(i);
                Assert.IsTrue(expected.IsRelativelyEqual(actual, 1e-5));
            }
        }

        [Test]
        public void EquivalencyTest2()
        {
            // when ksi -> 0, the shifted log-logistic reduces to the logistic distribution.

            double sigma = 0.42;  // scale
            double ksi = 1e-10;  // shape
            double mu = 2.4;      // location

            var target = new ShiftedLogLogisticDistribution(location: mu, scale: sigma, shape: ksi);
            var log = new LogisticDistribution(mu, sigma);

            Assert.AreEqual(log.Mean, target.Mean, 1e-6);
            Assert.AreEqual(log.Median, target.Median, 1e-15);
            Assert.AreEqual(log.Mode, target.Mode, 1e-15);
            //Assert.AreEqual(log.Variance, target.Variance);

            double actual, expected;

            for (double i = -10; i < 10; i += 0.1)
            {
                expected = log.DistributionFunction(i);
                actual = target.DistributionFunction(i);
                Assert.IsTrue(expected.IsRelativelyEqual(actual, 1e-5));

                expected = log.ProbabilityDensityFunction(i);
                actual = target.ProbabilityDensityFunction(i);
                Assert.IsTrue(expected.IsRelativelyEqual(actual, 1e-5));

                expected = log.LogProbabilityDensityFunction(i);
                actual = target.LogProbabilityDensityFunction(i);
                Assert.IsTrue(expected.IsRelativelyEqual(actual, 1e-5));

                expected = log.ComplementaryDistributionFunction(i);
                actual = target.ComplementaryDistributionFunction(i);
                Assert.IsTrue(expected.IsRelativelyEqual(actual, 1e-5));

                double p = log.DistributionFunction(i);
                expected = log.InverseDistributionFunction(p);
                actual = target.InverseDistributionFunction(p);
                Assert.AreEqual(expected, actual, 1e-5);

                expected = log.HazardFunction(i);
                actual = target.HazardFunction(i);
                Assert.IsTrue(expected.IsRelativelyEqual(actual, 1e-5));

                expected = log.CumulativeHazardFunction(i);
                actual = target.CumulativeHazardFunction(i);
                Assert.IsTrue(expected.IsRelativelyEqual(actual, 1e-5));
            }
        }

        [Test]
        public void EquivalencyTest3()
        {
            // when ksi -> 0, the shifted log-logistic reduces to the logistic distribution.

            double sigma = 0.42;  // scale
            double ksi = 0;  // shape
            double mu = 2.4;      // location

            var target = new ShiftedLogLogisticDistribution(location: mu, scale: sigma, shape: ksi);
            var log = new LogisticDistribution(mu, sigma);

            Assert.AreEqual(log.Mean, target.Mean);
            Assert.AreEqual(log.Median, target.Median);
            Assert.AreEqual(log.Mode, target.Mode);
            Assert.AreEqual(log.Variance, target.Variance);

            double actual, expected;

            for (double i = -10; i < 10; i += 0.1)
            {
                expected = log.DistributionFunction(i);
                actual = target.DistributionFunction(i);
                Assert.IsTrue(expected.IsRelativelyEqual(actual, 1e-15));

                expected = log.ProbabilityDensityFunction(i);
                actual = target.ProbabilityDensityFunction(i);
                Assert.IsTrue(expected.IsRelativelyEqual(actual, 1e-15));

                expected = log.LogProbabilityDensityFunction(i);
                actual = target.LogProbabilityDensityFunction(i);
                Assert.IsTrue(expected.IsRelativelyEqual(actual, 1e-10));

                expected = log.ComplementaryDistributionFunction(i);
                actual = target.ComplementaryDistributionFunction(i);
                Assert.IsTrue(expected.IsRelativelyEqual(actual, 1e-15));

                double p = log.DistributionFunction(i);
                expected = log.InverseDistributionFunction(p);
                actual = target.InverseDistributionFunction(p);
                Assert.AreEqual(expected, actual, 1e-5);

                expected = log.HazardFunction(i);
                actual = target.HazardFunction(i);
                Assert.IsTrue(expected.IsRelativelyEqual(actual, 1e-15));

                expected = log.CumulativeHazardFunction(i);
                actual = target.CumulativeHazardFunction(i);
                Assert.IsTrue(expected.IsRelativelyEqual(actual, 1e-15));
            }
        }

    }
}
