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
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;
    using System;
    using System.Globalization;
    using System.Linq;


    [TestFixture]
    public class BernoulliDistributionTest
    {

        [Test]
        public void ConstructorTest()
        {
            var bern = new BernoulliDistribution(mean: 0.42);

            // Common measures
            double mean = bern.Mean;     // 0.42
            double median = bern.Median; // 0.0
            double var = bern.Variance;  // 0.2436
            double mode = bern.Mode;     // 0.0

            // Probability mass functions
            double pdf = bern.ProbabilityMassFunction(k: 1); // 0.42
            double lpdf = bern.LogProbabilityMassFunction(k: 0); // -0.54472717544167193

            // Cumulative distribution function
            double cdf = bern.DistributionFunction(k: 0);    // 0.58
            double ccdf = bern.ComplementaryDistributionFunction(k: 0); // 0.42

            // Quantile function
            int icdf0 = bern.InverseDistributionFunction(p: 0.57); // 0
            int icdf1 = bern.InverseDistributionFunction(p: 0.59); // 1

            double hf = bern.HazardFunction(x: 0); // 1.3809523809523814
            double chf = bern.CumulativeHazardFunction(x: 0); // 0.86750056770472328

            string str = bern.ToString(CultureInfo.InvariantCulture); // "Bernoulli(x; p = 0.42, q = 0.58)"

            Assert.AreEqual(0.42, mean);
            Assert.AreEqual(0.0, median);
            Assert.AreEqual(0.2436, var);
            Assert.AreEqual(0.0, mode);
            Assert.AreEqual(0.86750056770472328, chf, 1e-10);
            Assert.AreEqual(0.58, cdf, 1e-10);
            Assert.AreEqual(0.42, pdf);
            Assert.AreEqual(-0.54472717544167193, lpdf);
            Assert.AreEqual(1.3809523809523814, hf, 1e-10);
            Assert.AreEqual(0.42, ccdf, 1e-10);
            Assert.AreEqual(0, icdf0);
            Assert.AreEqual(1, icdf1);
            Assert.AreEqual("Bernoulli(x; p = 0.42, q = 0.58)", str);

            var range1 = bern.GetRange(0.95);
            var range2 = bern.GetRange(0.99);
            var range3 = bern.GetRange(0.01);

            Assert.AreEqual(0, range1.Min);
            Assert.AreEqual(1.0, range1.Max);
            Assert.AreEqual(0, range2.Min);
            Assert.AreEqual(1.0, range2.Max);
            Assert.AreEqual(0, range3.Min);
            Assert.AreEqual(1.0, range3.Max);


            Assert.AreEqual(0, bern.Support.Min);
            Assert.AreEqual(1, bern.Support.Max);

            Assert.AreEqual(bern.InverseDistributionFunction(0), bern.Support.Min);
            Assert.AreEqual(bern.InverseDistributionFunction(1), bern.Support.Max);
        }

        [Test]
        public void ProbabilityMassFunctionTest()
        {
            BernoulliDistribution target = new BernoulliDistribution(0.6);

            double expected = 0.6;
            double actual = target.ProbabilityMassFunction(1);

            Assert.AreEqual(expected, actual, 1e-6);
        }

        [Test]
        public void LogProbabilityMassFunctionTest()
        {
            BernoulliDistribution target = new BernoulliDistribution(0.6);

            double expected = System.Math.Log(0.6);
            double actual = target.LogProbabilityMassFunction(1);

            Assert.AreEqual(expected, actual, 1e-6);
        }

        [Test]
        public void DistributionFunctionTest()
        {
            BernoulliDistribution target = new BernoulliDistribution(0.6);

            double expected = 0.4;
            double actual = target.DistributionFunction(0);

            Assert.AreEqual(expected, actual, 1e-6);
        }

        [Test]
        public void ComplementaryDistributionFunctionTest()
        {
            BernoulliDistribution target = new BernoulliDistribution(0.42);

            {
                double expected = 1.0 - target.DistributionFunction(0);
                double actual = target.ComplementaryDistributionFunction(0);
                Assert.AreEqual(expected, actual, 1e-6);
            }

            {
                double expected = 1.0 - target.DistributionFunction(1);
                double actual = target.ComplementaryDistributionFunction(1);
                Assert.AreEqual(expected, actual, 1e-6);
            }

            {
                double expected = 1.0 - target.DistributionFunction(-1);
                double actual = target.ComplementaryDistributionFunction(-1);
                Assert.AreEqual(expected, actual, 1e-6);
            }

            {
                double expected = 1.0 - target.DistributionFunction(2);
                double actual = target.ComplementaryDistributionFunction(2);
                Assert.AreEqual(expected, actual, 1e-6);
            }
        }

        [Test]
        public void MedianTest()
        {
            {
                // Special case for the Bernoulli distribution: while the median is 0.5, 
                // the inverse distribution function cannot return a double value and thus
                // returns 1.
                BernoulliDistribution target = new BernoulliDistribution(0.5);
                int icdf = target.InverseDistributionFunction(0.5);
                Assert.AreEqual(0, icdf);
                Assert.AreEqual(0.5, target.Median);

                // However, it should work if the Bernoulli is being used as a continuous distribution:
                Assert.AreEqual(0.5, (target as IUnivariateDistribution).InverseDistributionFunction(0.5));
                Assert.AreEqual(0.5, (target as IUnivariateDistribution<double>).InverseDistributionFunction(0.5));
            }

            {
                BernoulliDistribution target = new BernoulliDistribution(0.2);
                Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
            }

            {
                BernoulliDistribution target = new BernoulliDistribution(0.6);
                Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
            }

            {
                BernoulliDistribution target = new BernoulliDistribution(0.0);
                Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
            }

            {
                BernoulliDistribution target = new BernoulliDistribution(1.0);
                Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
            }
        }

        [Test]
        public void GenerationTest()
        {
            double prob = 0.5;
            int trials = 100000;

            BernoulliDistribution target = new BernoulliDistribution(prob);
            target.Fit(target.Generate(trials).Select(x => (double)x).ToArray());

            Assert.AreEqual(target.Mean, prob, 1e-2);
        }


        [Test]
        public void icdf()
        {
            // install.packages('LaplacesDemon')
            // library('LaplacesDemon')
            // 
            BernoulliDistribution dist = new BernoulliDistribution(0.5);

            double[] percentiles = Vector.Range(0.0, 1.0, stepSize: 0.1);
            for (int i = 0; i < percentiles.Length; i++)
            {
                double x = percentiles[i];
                int icdf = dist.InverseDistributionFunction(x);
                double cdf = dist.DistributionFunction(icdf);
                int iicdf = dist.InverseDistributionFunction(cdf);
                double iiicdf = dist.DistributionFunction(iicdf);

                if (i < 6) // qbern(p=((0:10)/10), prob=0.5)
                    Assert.AreEqual(0, icdf);
                else
                    Assert.AreEqual(1, icdf);

                double rx = System.Math.Round(x, MidpointRounding.ToEven);
                double rc = System.Math.Round(cdf, MidpointRounding.ToEven);

                Assert.AreEqual(rx, rc, 1e-5);
                Assert.AreEqual(iicdf, icdf, 1e-5);
                Assert.AreEqual(iiicdf, cdf, 1e-5);
            }


        }
    }
}
