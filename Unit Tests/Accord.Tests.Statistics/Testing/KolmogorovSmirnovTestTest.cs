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
    using System;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Testing;
    using NUnit.Framework;
    using Accord.Statistics.Distributions;

    [TestFixture]
    public class KolmogorovSmirnovTestTest
    {

        [Test]
        public void KolmogorovSmirnovTestConstructorTest()
        {
            // Test against a standard Uniform distribution
            // References: http://www.math.nsysu.edu.tw/~lomn/homepage/class/92/kstest/kolmogorov.pdf


            // Suppose we got a new sample, and we would like to test whether this
            // sample seems to have originated from a uniform continuous distribution.
            //
            double[] sample = 
            { 
                0.621, 0.503, 0.203, 0.477, 0.710, 0.581, 0.329, 0.480, 0.554, 0.382
            };

            // First, we create the distribution we would like to test against:
            //
            var distribution = UniformContinuousDistribution.Standard;

            // Now we can define our hypothesis. The null hypothesis is that the sample
            // comes from a standard uniform distribution, while the alternate is that
            // the sample is not from a standard uniform distribution.
            //
            var kstest = new KolmogorovSmirnovTest(sample, distribution);

            double statistic = kstest.Statistic; // 0.29
            double pvalue = kstest.PValue; // 0.3067

            bool significant = kstest.Significant; // false

            // Since the null hypothesis could not be rejected, then the sample
            // can perhaps be from a uniform distribution. However, please note
            // that this doesn't means that the sample *is* from the uniform, it
            // only means that we could not rule out the possibility.

            Assert.AreEqual(distribution, kstest.TheoreticalDistribution);
            Assert.AreEqual(KolmogorovSmirnovTestHypothesis.SampleIsDifferent, kstest.Hypothesis);
            Assert.AreEqual(DistributionTail.TwoTail, kstest.Tail);

            Assert.AreEqual(0.29, statistic, 1e-16);
            Assert.AreEqual(0.3067, pvalue, 1e-4);
            Assert.IsFalse(Double.IsNaN(pvalue));

            
            Assert.IsFalse(kstest.Significant);
        }

        [Test]
        public void KolmogorovSmirnovTestConstructorTest2()
        {
            // Test against a Normal distribution

            // This time, let's see if the same sample from the previous example
            // could have originated from a standard Normal (Gaussian) distribution.
            //
            double[] sample =
            { 
                0.621, 0.503, 0.203, 0.477, 0.710, 0.581, 0.329, 0.480, 0.554, 0.382
            };

            // Before we could not rule out the possibility that the sample came from
            // a uniform distribution, which means the sample was not very far from
            // uniform. This would be an indicative that it would be far from what
            // would be expected from a Normal distribution:

            NormalDistribution distribution = NormalDistribution.Standard;

            var kstest = new KolmogorovSmirnovTest(sample, distribution);

            double statistic = kstest.Statistic; // 0.580432
            double pvalue = kstest.PValue; // 0.000999

            bool significant = kstest.Significant; // true

            // Since the test says that the null hypothesis should be rejected, then
            // this can be regarded as a strong indicative that the sample does not
            // comes from a Normal distribution, just as we expected.


            Assert.AreEqual(distribution, kstest.TheoreticalDistribution);
            Assert.AreEqual(KolmogorovSmirnovTestHypothesis.SampleIsDifferent, kstest.Hypothesis);
            Assert.AreEqual(DistributionTail.TwoTail, kstest.Tail);

            Assert.AreEqual(0.580432, kstest.Statistic, 1e-5);
            Assert.AreEqual(0.000999, kstest.PValue, 1e-5);
            Assert.IsFalse(Double.IsNaN(kstest.Statistic));

            // The null hypothesis can be rejected:
            // the sample is not from a standard Normal distribution
            Assert.IsTrue(kstest.Significant);
        }

        [Test]
        public void KolmogorovSmirnovTestConstructorTest3()
        {
            // Test if the sample's distribution is greater than a standard Normal distribution.

            double[] sample = { 0.621, 0.503, 0.203, 0.477, 0.710, 0.581, 0.329, 0.480, 0.554, 0.382 };

            NormalDistribution distribution = NormalDistribution.Standard;
            var target = new KolmogorovSmirnovTest(sample, distribution,
                KolmogorovSmirnovTestHypothesis.SampleIsGreater);

            Assert.AreEqual(distribution, target.TheoreticalDistribution);
            Assert.AreEqual(KolmogorovSmirnovTestHypothesis.SampleIsGreater, target.Hypothesis);
            Assert.AreEqual(DistributionTail.OneUpper, target.Tail);

            Assert.AreEqual(0.238852, target.Statistic, 1e-5);
            Assert.AreEqual(0.275544, target.PValue, 1e-5);
            Assert.IsFalse(Double.IsNaN(target.Statistic));
        }

        [Test]
        public void KolmogorovSmirnovTestConstructorTest4()
        {
            // Test if the sample's distribution is smaller than a standard Normal distribution

            double[] sample = { 0.621, 0.503, 0.203, 0.477, 0.710, 0.581, 0.329, 0.480, 0.554, 0.382 };

            NormalDistribution distribution = NormalDistribution.Standard;
            var target = new KolmogorovSmirnovTest(sample, distribution, 
                KolmogorovSmirnovTestHypothesis.SampleIsSmaller);

            Assert.AreEqual(distribution, target.TheoreticalDistribution);
            Assert.AreEqual(KolmogorovSmirnovTestHypothesis.SampleIsSmaller, target.Hypothesis);
            Assert.AreEqual(DistributionTail.OneLower, target.Tail);

            Assert.AreEqual(0.580432, target.Statistic, 1e-5);
            Assert.AreEqual(0.000499, target.PValue, 1e-5);
            Assert.IsFalse(Double.IsNaN(target.Statistic));
        }

        [Test]
        public void EmpiricalDistributionTest()
        {
            double[] sample = { 1, 5, 3, 1, 5, 2, 1 };
            UnivariateContinuousDistribution distribution = NormalDistribution.Standard;

            var target = new KolmogorovSmirnovTest(sample, distribution);

            EmpiricalDistribution actual = target.EmpiricalDistribution;

            Assert.AreNotSame(sample, actual.Samples);

            Array.Sort(sample);

            for (int i = 0; i < sample.Length; i++)
                Assert.AreEqual(sample[i], actual.Samples[i]);
        }

        [Test]
        public void TheoreticalDistributionTest()
        {
            double[] sample = { 1, 5, 3, 1, 5, 2, 1 };
            UnivariateContinuousDistribution distribution = NormalDistribution.Standard;

            var target = new KolmogorovSmirnovTest(sample, distribution);

            var actual = target.TheoreticalDistribution;
            Assert.AreEqual(distribution, actual);
        }
    }
}