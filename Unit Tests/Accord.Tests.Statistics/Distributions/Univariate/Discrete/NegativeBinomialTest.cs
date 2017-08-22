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
    using Accord.Math;
    using Accord.Statistics.Distributions.Reflection;
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;
    using System;
    using System.Globalization;

    [TestFixture]
    public class NegativeBinomialTest
    {

        [Test]
        public void ConstructorTest()
        {
            #region doc_example
            // Create a Negative Binomial distribution with r = 7, p = 0.42
            var dist = new NegativeBinomialDistribution(failures: 7, probability: 0.42);

            // Common measures
            double mean = dist.Mean;     // 5.068965517241379
            double median = dist.Median; // 9.0
            double var = dist.Variance;  // 8.7395957193816862

            // Cumulative distribution functions
            double cdf = dist.DistributionFunction(k: 2);               // 0.033380251139644379
            double ccdf = dist.ComplementaryDistributionFunction(k: 2); // 0.96661974886035562

            // Probability mass functions
            double pmf1 = dist.ProbabilityMassFunction(k: 4); // 0.054786846293416853
            double pmf2 = dist.ProbabilityMassFunction(k: 5); // 0.069908015870399909
            double pmf3 = dist.ProbabilityMassFunction(k: 6); // 0.0810932984096639
            double lpmf = dist.LogProbabilityMassFunction(k: 2); // -2.3927801721315989

            // Quantile function
            int icdf = dist.InverseDistributionFunction(p: cdf); // 2
            int icdf1 = dist.InverseDistributionFunction(p: 0.17); // 5
            int icdf2 = dist.InverseDistributionFunction(p: 0.46); // 9
            int icdf3 = dist.InverseDistributionFunction(p: 0.87); // 15

            // Hazard (failure rate) functions
            double hf = dist.HazardFunction(x: 4); // 0.062681673912893129
            double chf = dist.CumulativeHazardFunction(x: 4); // 0.13461898882526471

            // String representation
            string str = dist.ToString(CultureInfo.InvariantCulture); // "NegativeBinomial(x; r = 7, p = 0.42)"
            #endregion

            double[] probabilities = new double[10];
            for (int i = 0; i < probabilities.Length; i++)
                probabilities[i] = dist.DistributionFunction(i);

            Assert.AreEqual(5.068965517241379, mean);
            Assert.AreEqual(9.0, median);
            Assert.AreEqual(8.7395957193816862, var);
            Assert.AreEqual(0.13461898882526471, chf, 1e-10);
            Assert.AreEqual(0.033380251139644379, cdf);
            Assert.AreEqual(0.054786846293416853, pmf1);
            Assert.AreEqual(0.069908015870399909, pmf2);
            Assert.AreEqual(0.0810932984096639, pmf3);
            Assert.AreEqual(-3.8297538146412009, lpmf);
            Assert.AreEqual(0.062681673912893129, hf);
            Assert.AreEqual(0.96661974886035562, ccdf);
            Assert.AreEqual(2, icdf);
            Assert.AreEqual(5, icdf1);
            Assert.AreEqual(9, icdf2);
            Assert.AreEqual(15, icdf3);
            Assert.AreEqual("NegativeBinomial(x; r = 7, p = 0.42)", str);

            var range1 = dist.GetRange(0.95);
            var range2 = dist.GetRange(0.99);
            var range3 = dist.GetRange(0.01);

            Assert.AreEqual(3, range1.Min);
            Assert.AreEqual(18.0, range1.Max);
            Assert.AreEqual(1, range2.Min);
            Assert.AreEqual(23, range2.Max);
            Assert.AreEqual(1, range3.Min);
            Assert.AreEqual(23, range3.Max);
        }

        [Test]
        public void NegativeBinomialConstructorTest()
        {
            double expected, actual;

            var target = new NegativeBinomialDistribution(6, 0.42);
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

        [Test]
        public void simple_cdf_test()
        {
            var distribution = new NegativeBinomialDistribution(71, 0.9712);
            double actual = distribution.DistributionFunction(1);
            Assert.AreEqual(0.38236189547264443, actual, 1e-10);
        }

        [Test]
        public void cdf_test()
        {
            double expected, actual;
            var target = new NegativeBinomialDistribution(6, 0.42);

            actual = target.DistributionFunction(-1);
            expected = 0.0;
            Assert.AreEqual(expected, actual, 1e-7);

            actual = target.DistributionFunction(0);
            expected = 0.00548903;
            Assert.AreEqual(expected, actual, 1e-7);

            actual = target.DistributionFunction(1);
            expected = 0.024590862213120013;
            Assert.AreEqual(expected, actual, 1e-7);

            actual = target.DistributionFunction(2);
            expected = 0.063367578065433472;
            Assert.AreEqual(expected, actual, 1e-7);

            actual = target.DistributionFunction(10);
            expected = 0.72802982018820717;
            Assert.AreEqual(expected, actual, 1e-7);
        }

        [Test]
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

        [Test]
        public void icdf2()
        {
            int trials = 1;

            var dist = new NegativeBinomialDistribution(trials, 0.5);

            double[] percentiles = Vector.Range(0.0, 1.0, stepSize: 0.1);
            for (int i = 0; i < percentiles.Length; i++)
            {
                double x = percentiles[i];
                int icdf = dist.InverseDistributionFunction(x);
                double cdf = dist.DistributionFunction(icdf);
                int iicdf = dist.InverseDistributionFunction(cdf);
                double iiicdf = dist.DistributionFunction(iicdf);

                double rx = System.Math.Round(x, MidpointRounding.ToEven);
                double rc = System.Math.Round(cdf, MidpointRounding.ToEven);

                Assert.AreEqual(rx, rc, 1e-5);
                Assert.AreEqual(iicdf, icdf, 1e-5);
                Assert.AreEqual(iiicdf, cdf, 1e-5);
            }
        }

        [Test]
        public void pdf()
        {
            NegativeBinomialDistribution dist = UnivariateDistributionInfo.CreateInstance<NegativeBinomialDistribution>();

            Assert.AreEqual(0.5, dist.ProbabilityOfSuccess);
            Assert.AreEqual(1, dist.NumberOfFailures);

            double median = dist.Median;
            Assert.AreEqual(0, median);

            int middle = (int)median;

            double pdf = dist.ProbabilityMassFunction(middle);
            double lpdf = dist.LogProbabilityMassFunction(middle);

            Assert.AreEqual(Math.Log(pdf), lpdf, 1e-10);
            Assert.AreEqual(pdf, Math.Exp(lpdf), 1e-10);

        }
    }
}
