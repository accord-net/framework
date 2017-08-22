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
    using NUnit.Framework;
    using Accord.Statistics.Distributions;
    using System.Globalization;
    using Accord.Math;

    [TestFixture]
    public class PoissonDistributionTest
    {

        [Test]
        public void ConstructorTest()
        {
            // Create a new Poisson distribution with 
            var dist = new PoissonDistribution(lambda: 4.2);

            // Common measures
            double mean = dist.Mean;     // 4.2
            double median = dist.Median; // 4.0
            double var = dist.Variance;  // 4.2

            // Cumulative distribution functions
            double cdf1 = dist.DistributionFunction(k: 2); // 0.21023798702309743
            double cdf2 = dist.DistributionFunction(k: 4); // 0.58982702131057763
            double cdf3 = dist.DistributionFunction(k: 7); // 0.93605666027257894
            double ccdf = dist.ComplementaryDistributionFunction(k: 2); // 0.78976201297690252

            // Probability mass functions
            double pmf1 = dist.ProbabilityMassFunction(k: 4); // 0.19442365170822165
            double pmf2 = dist.ProbabilityMassFunction(k: 5); // 0.1633158674349062
            double pmf3 = dist.ProbabilityMassFunction(k: 6); // 0.11432110720443435
            double lpmf = dist.LogProbabilityMassFunction(k: 2); // -2.0229781299813

            // Quantile function
            int icdf1 = dist.InverseDistributionFunction(p: cdf1); // 2
            int icdf2 = dist.InverseDistributionFunction(p: cdf2); // 4
            int icdf3 = dist.InverseDistributionFunction(p: cdf3); // 7

            // Hazard (failure rate) functions
            double hf = dist.HazardFunction(x: 4); // 0.47400404660843515
            double chf = dist.CumulativeHazardFunction(x: 4); // 0.89117630901575073

            // String representation
            string str = dist.ToString(CultureInfo.InvariantCulture); // "Poisson(x; λ = 4.2)"


            // Median bounds
            // (http://en.wikipedia.org/wiki/Poisson_distribution#Median)

            double max = 4.2 + 1 / 3.0;
            double min = 4.2 - System.Math.Log(2);
            Assert.IsTrue(median < max);
            Assert.IsTrue(min <= median);

            Assert.AreEqual(4.2, mean);
            Assert.AreEqual(4.0, median);
            Assert.AreEqual(4.2, var);
            Assert.AreEqual(0.89117630901575073, chf, 1e-10);
            Assert.AreEqual(0.21023798702309743, cdf1);
            Assert.AreEqual(0.58982702131057763, cdf2);
            Assert.AreEqual(0.93605666027257894, cdf3);
            Assert.AreEqual(0.19442365170822165, pmf1);
            Assert.AreEqual(0.1633158674349062, pmf2);
            Assert.AreEqual(0.11432110720443435, pmf3);
            Assert.AreEqual(-2.0229781299813, lpmf);
            Assert.AreEqual(0.47400404660843515, hf);
            Assert.AreEqual(0.89117630901575073, chf);
            Assert.AreEqual(0.78976201297690252, ccdf);
            Assert.AreEqual(2, icdf1);
            Assert.AreEqual(4, icdf2);
            Assert.AreEqual(7, icdf3);
            Assert.AreEqual("Poisson(x; λ = 4.2)", str);

            var range1 = dist.GetRange(0.95);
            var range2 = dist.GetRange(0.99);
            var range3 = dist.GetRange(0.01);

            Assert.AreEqual(1, range1.Min);
            Assert.AreEqual(8, range1.Max);
            Assert.AreEqual(0, range2.Min);
            Assert.AreEqual(10, range2.Max);
            Assert.AreEqual(0, range3.Min);
            Assert.AreEqual(10, range3.Max);
        }

        [Test]
        public void DistributionFunctionTest()
        {
            // Create a new Poisson distribution
            var dist = new PoissonDistribution(lambda: 4.2);

            // P(X = 1) = 0.0629814226460064
            double equal = dist.ProbabilityMassFunction(k: 1);

            // P(X < 1) = 0.0149955768204777
            double less = dist.DistributionFunction(k: 1, inclusive: false);

            // P(X ≤ 1) = 0.0779769994664841
            double lessThanOrEqual = dist.DistributionFunction(k: 1, inclusive: true);

            // P(X > 1) = 0.922023000533516
            double greater = dist.ComplementaryDistributionFunction(k: 1);

            // P(X ≥ 1) = 0.985004423179522
            double greaterThanOrEqual = dist.ComplementaryDistributionFunction(k: 1, inclusive: true);

            double inv1 = dist.InverseDistributionFunction(less);
            double inv2 = dist.InverseDistributionFunction(lessThanOrEqual);

            Assert.AreEqual(equal, 0.0629814226460064, 1e-10);
            Assert.AreEqual(less, 0.0149955768204777, 1e-10);
            Assert.AreEqual(lessThanOrEqual, 0.0779769994664841, 1e-10);
            Assert.AreEqual(greater, 0.922023000533516, 1e-10);
            Assert.AreEqual(greaterThanOrEqual, 0.985004423179522, 1e-10);

            Assert.AreEqual(inv1, 1, 1e-10);
            Assert.AreEqual(inv2, 1, 1e-10);
        }

        [Test]
        public void InverseDistributionFunctionTest()
        {
            // Create a new Poisson distribution
            var dist = new PoissonDistribution(lambda: 2);

            double equal = dist.ProbabilityMassFunction(k: 7);

            double lte1 = dist.DistributionFunction(k: 7);
            double lte2 = dist.DistributionFunction(k: 7, inclusive: true);
            double less = dist.DistributionFunction(k: 7, inclusive: false);

            double inv0 = dist.InverseDistributionFunction(0.99890328103214132);
            double inv1 = dist.InverseDistributionFunction(lte1);
            double inv2 = dist.InverseDistributionFunction(lte2);
            double inv3 = dist.InverseDistributionFunction(less);

            Assert.AreEqual(equal, 0.0034370865583901638, 1e-10);
            Assert.AreEqual(less, 0.99546619447375118, 1e-10);
            Assert.AreEqual(lte1, 0.99890328103214132, 1e-10);
            Assert.AreEqual(lte2, 0.99890328103214132, 1e-10);

            Assert.AreEqual(inv0, 7, 1e-10);
            Assert.AreEqual(inv1, 7, 1e-10);
            Assert.AreEqual(inv2, 7, 1e-10);
            Assert.AreEqual(inv3, 6, 1e-10);
        }

        [Test]
        public void ConstructorTest2()
        {
            // Create a new Poisson distribution with lambda = 0.7
            PoissonDistribution poisson = new PoissonDistribution(0.7);

            double mean = poisson.Mean;                // 0.7    (lambda) 
            double median = poisson.Median;            // 1.0
            double mode = poisson.Mode;                // 0.7    (lambda)  
            double stdDev = poisson.StandardDeviation; // 0.836  [sqrt((lambda))]
            double var = poisson.Variance;             // 0.7    (lambda) 

            // The cumulative distribution function, or the probability that a real-valued 
            // random variable will be found to have a value less than or equal to some x:
            double cdf = poisson.DistributionFunction(k: 1);        // 0.84419501644539618

            // The probability density function, or the relative likelihood for a real-valued 
            // random variable will be found to take on a given specific value of x:
            double pdf = poisson.ProbabilityMassFunction(k: 1);  // 0.34760971265398666

            // The log of the probability density function, useful for applications where
            // precision is critical
            double lpdf = poisson.LogProbabilityMassFunction(k: 1); // -1.0566749439387324

            // The complementary distribution function, or the tail function, that gives the
            // probability that a real-valued random variable will be found to have a value 
            // greater than some x. This function is also known as the Survival function.
            double ccdf = poisson.ComplementaryDistributionFunction(k: 1); // 0.15580498355460382

            // The inverse distribution function, or the Quantile function, that is able to
            // revert probability values back to the real value that produces that probability
            int icdf = poisson.InverseDistributionFunction(p: cdf); // 1

            // The Hazard function, or the failure rate, the event rate at time t conditional 
            // on survival until time t or later. Note that this function may only make sense
            // when using time-defined distributions, such as the Poisson.
            double hf = poisson.HazardFunction(x: 1);            // 2.2310564445595058

            // The cumulative hazard function, that gives how much the hazard 
            // function accumulated over time until a given time instant x.
            double chf = poisson.CumulativeHazardFunction(x: 1); // 1.8591501591854034

            // Every distribution has a friendly string representation
            string str = poisson.ToString(System.Globalization.CultureInfo.InvariantCulture); // Poisson(x; λ = 0.7)

            Assert.AreEqual(0.84419501644539618, cdf);
            Assert.AreEqual(0.34760971265398666, pdf);
            Assert.AreEqual(-1.0566749439387324, lpdf);
            Assert.AreEqual(0.15580498355460382, ccdf);
            Assert.AreEqual(1, icdf);
            Assert.AreEqual(2.2310564445595058, hf);
            Assert.AreEqual(1.8591501591854034, chf);
            Assert.AreEqual("Poisson(x; λ = 0.7)", str);

            Assert.AreEqual(0.7, mean);
            Assert.AreEqual(0.7, mode);
            Assert.AreEqual(1, median);
            Assert.AreEqual(0.7, var);
            Assert.AreEqual(0.83666002653407556, stdDev);
        }

        [Test]
        public void FitTest()
        {
            PoissonDistribution target = new PoissonDistribution(1);
            double[] observations = { 0.2, 0.7, 1.0, 0.33 };

            target.Fit(observations);

            double expected = 0.5575;
            Assert.AreEqual(expected, target.Mean);
        }

        [Test]
        public void ProbabilityDensityFunctionTest()
        {
            PoissonDistribution target = new PoissonDistribution(25);

            double actual = target.ProbabilityMassFunction(20);
            double expected = 0.051917468608491321;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void LogProbabilityDensityFunctionTest()
        {
            PoissonDistribution target = new PoissonDistribution(25);

            double actual = target.LogProbabilityMassFunction(20);
            double expected = System.Math.Log(0.051917468608491321);

            Assert.AreEqual(expected, actual, 1e-10);
        }


        [Test]
        public void MedianTest()
        {
            for (int i = 0; i < 25; i++)
            {
                var target = new PoissonDistribution(i + 1);
                Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
            }
        }

        [Test]
        public void GenerateTest()
        {
            double lambda = 1.11022302462516E-16;
            var target = new PoissonDistribution(lambda) as ISampleableDistribution<double>;

            double[] values = target.Generate(samples: 10000);
            for (int i = 0; i < values.Length; i++)
                Assert.AreEqual(0, values[i]);
        }

        [Test]
        public void GenerateTest2()
        {
            double lambda = 1.11022302462516E-16;
            var target = new PoissonDistribution(lambda) as ISampleableDistribution<double>;

            double[] values = new double[10000];
            for (int i = 0; i < values.Length; i++)
                values[i] = target.Generate();

            for (int i = 0; i < values.Length; i++)
                Assert.AreEqual(0, values[i]);
        }


        [Test]
        public void GenerateTest3()
        {
            double lambda = 0.75;
            var a = new PoissonDistribution(lambda) as ISampleableDistribution<double>;


            Accord.Math.Random.Generator.Seed = 0;
            double[] expected = a.Generate(samples: 10000);

            Accord.Math.Random.Generator.Seed = 0;
            var b = new PoissonDistribution(lambda);
            Accord.Math.Random.Generator.Seed = 0;
            int[] actual = b.Generate(10000);

            Assert.IsTrue(expected.IsEqual(actual));
        }
    }
}
