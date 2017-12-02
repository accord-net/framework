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
    public class LillieforsTestTest
    {

        [Test]
        public void KolmogorovSmirnovTestConstructorTest()
        {
            #region doc_uniform_ks
            // Test against a standard Uniform distribution
            // References: http://www.math.nsysu.edu.tw/~lomn/homepage/class/92/kstest/kolmogorov.pdf

            // Make this example reproducible
            Accord.Math.Random.Generator.Seed = 1;
            
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
            var kstest = new LillieforsTest(sample, distribution, reestimate: false, iterations: 10 * 1000 * 1000);

            double statistic = kstest.Statistic; // 0.29
            double pvalue = kstest.PValue; // 0.3067

            bool significant = kstest.Significant; // false

            // Since the null hypothesis could not be rejected, then the sample
            // can perhaps be from a uniform distribution. However, please note
            // that this doesn't means that the sample *is* from the uniform, it
            // only means that we could not rule out the possibility.
            #endregion

            Assert.AreEqual(distribution, kstest.TheoreticalDistribution);
            Assert.AreEqual(KolmogorovSmirnovTestHypothesis.SampleIsDifferent, kstest.Hypothesis);
            Assert.AreEqual(DistributionTail.TwoTail, kstest.Tail);

            Assert.AreEqual(0.29, statistic, 1e-16);
            Assert.AreEqual(0.3067, pvalue, 2e-3);
            Assert.IsFalse(Double.IsNaN(pvalue));

            Assert.IsFalse(kstest.Significant);
        }

        [Test]
        public void KolmogorovSmirnovTestConstructorTest2()
        {
            #region doc_normal_ks
            // Test against a Normal distribution

            // Make this example reproducible
            Accord.Math.Random.Generator.Seed = 1;

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

            var kstest = new LillieforsTest(sample, distribution, reestimate: false);

            double statistic = kstest.Statistic; // 0.580432
            double pvalue = kstest.PValue; // 0.000999

            bool significant = kstest.Significant; // true

            // Since the test says that the null hypothesis should be rejected, then
            // this can be regarded as a strong indicative that the sample does not
            // comes from a Normal distribution, just as we expected.
            #endregion

            Assert.AreEqual(distribution, kstest.TheoreticalDistribution);
            Assert.AreEqual(KolmogorovSmirnovTestHypothesis.SampleIsDifferent, kstest.Hypothesis);
            Assert.AreEqual(DistributionTail.TwoTail, kstest.Tail);

            Assert.AreEqual(0.580432, kstest.Statistic, 1e-5);
            Assert.AreEqual(0.000999, kstest.PValue, 1e-3);
            Assert.IsFalse(Double.IsNaN(kstest.Statistic));

            // The null hypothesis can be rejected:
            // the sample is not from a standard Normal distribution
            Assert.IsTrue(kstest.Significant);
        }

        [Test]
        public void KolmogorovSmirnovTestConstructorTest3()
        {
            Accord.Math.Random.Generator.Seed = 1;

            // Test if the sample's distribution is greater than a standard Normal distribution.

            double[] sample = { 0.621, 0.503, 0.203, 0.477, 0.710, 0.581, 0.329, 0.480, 0.554, 0.382 };

            NormalDistribution distribution = NormalDistribution.Standard;
            var target = new LillieforsTest(sample, distribution, reestimate: false,
                alternate: KolmogorovSmirnovTestHypothesis.SampleIsGreater, iterations: 1000 * 1000);

            Assert.AreEqual(distribution, target.TheoreticalDistribution);
            Assert.AreEqual(KolmogorovSmirnovTestHypothesis.SampleIsGreater, target.Hypothesis);
            Assert.AreEqual(DistributionTail.OneUpper, target.Tail);

            Assert.AreEqual(0.238852, target.Statistic, 1e-5);
            Assert.AreEqual(0.275544, target.PValue, 5e-3);
            Assert.IsFalse(Double.IsNaN(target.Statistic));
        }

        [Test]
        public void KolmogorovSmirnovTestConstructorTest4()
        {
            Accord.Math.Random.Generator.Seed = 1;

            // Test if the sample's distribution is smaller than a standard Normal distribution

            double[] sample = { 0.621, 0.503, 0.203, 0.477, 0.710, 0.581, 0.329, 0.480, 0.554, 0.382 };

            NormalDistribution distribution = NormalDistribution.Standard;
            var target = new LillieforsTest(sample, distribution, reestimate: false,
                alternate: KolmogorovSmirnovTestHypothesis.SampleIsSmaller);

            Assert.AreEqual(distribution, target.TheoreticalDistribution);
            Assert.AreEqual(KolmogorovSmirnovTestHypothesis.SampleIsSmaller, target.Hypothesis);
            Assert.AreEqual(DistributionTail.OneLower, target.Tail);

            Assert.AreEqual(0.580432, target.Statistic, 1e-5);
            Assert.AreEqual(0.000499, target.PValue, 5e-4);
            Assert.IsFalse(Double.IsNaN(target.Statistic));
        }

        [Test]
        public void EmpiricalDistributionTest()
        {
            Accord.Math.Random.Generator.Seed = 1;

            double[] sample = { 1, 5, 3, 1, 5, 2, 1 };
            UnivariateContinuousDistribution distribution = NormalDistribution.Standard;

            var target = new LillieforsTest(sample, distribution, reestimate: false);

            EmpiricalDistribution actual = target.EmpiricalDistribution;

            Assert.AreNotSame(sample, actual.Samples);

            Array.Sort(sample);

            for (int i = 0; i < sample.Length; i++)
                Assert.AreEqual(sample[i], actual.Samples[i]);
        }

        [Test]
        public void TheoreticalDistributionTest()
        {
            Accord.Math.Random.Generator.Seed = 1;

            double[] sample = { 1, 5, 3, 1, 5, 2, 1 };
            UnivariateContinuousDistribution distribution = NormalDistribution.Standard;

            var target = new LillieforsTest(sample, distribution, reestimate: false);

            ISampleableDistribution<double> actual = target.TheoreticalDistribution;
            Assert.AreEqual(distribution, actual);
        }











        [Test]
        public void KolmogorovSmirnovTestConstructorTest_with_reestimation()
        {
            #region doc_uniform
            // Test against a Uniform distribution fitted from the data

            // Make this example reproducible
            Accord.Math.Random.Generator.Seed = 1;
             
            // Suppose we got a new sample, and we would like to test whether this
            // sample seems to have originated from a uniform continuous distribution.
            //
            double[] sample =
            {
                0.021, 0.003, 0.203, 0.177, 0.910, 0.881, 0.929, 0.180, 0.854, 0.982
            };

            // First, we create the distribution we would like to test against:
            //
            var distribution = UniformContinuousDistribution.Estimate(sample);

            // Now we can define our hypothesis. The null hypothesis is that the sample
            // comes from a standard uniform distribution, while the alternate is that
            // the sample is not from a standard uniform distribution.
            //
            var lillie = new LillieforsTest(sample, distribution, iterations: 10 * 1000 * 1000);

            double statistic = lillie.Statistic; // 0.36925
            double pvalue = lillie.PValue; // 0.09057

            bool significant = lillie.Significant; // false

            // Since the null hypothesis could not be rejected, then the sample
            // can perhaps be from a uniform distribution. However, please note
            // that this doesn't means that the sample *is* from the uniform, it
            // only means that we could not rule out the possibility.
            #endregion

            Assert.AreEqual(distribution, lillie.TheoreticalDistribution);
            Assert.AreEqual(KolmogorovSmirnovTestHypothesis.SampleIsDifferent, lillie.Hypothesis);
            Assert.AreEqual(DistributionTail.TwoTail, lillie.Tail);

            Assert.AreEqual(0.36925434116445355, statistic, 1e-16);
            Assert.AreEqual(0.090571500000000027, pvalue, 5e-3);

            Assert.IsFalse(lillie.Significant);
        }

        [Test]
        public void KolmogorovSmirnovTestConstructorTest2_with_reestimation()
        {
            #region doc_normal
            // Test against a Normal distribution

            // Make this example reproducible
            Accord.Math.Random.Generator.Seed = 1;

            // This time, let's see if the same sample from the previous example
            // could have originated from a fitted Normal (Gaussian) distribution.
            //
            double[] sample = 
            {
                0.021, 0.003, 0.203, 0.177, 0.910, 0.881, 0.929, 0.180, 0.854, 0.982
            };

            // Before we could not rule out the possibility that the sample came from
            // a uniform distribution, which means the sample was not very far from
            // uniform. This would be an indicative that it would be far from what
            // would be expected from a Normal distribution:

            NormalDistribution distribution = NormalDistribution.Estimate(sample);

            // Create the test
            var lillie = new LillieforsTest(sample, distribution);

            double statistic = lillie.Statistic; // 0.2882
            double pvalue = lillie.PValue; // 0.0207

            bool significant = lillie.Significant; // true

            // Since the test says that the null hypothesis should be rejected, then
            // this can be regarded as a strong indicative that the sample does not
            // comes from a Normal distribution, just as we expected.
            #endregion

            Assert.AreEqual(distribution, lillie.TheoreticalDistribution);
            Assert.AreEqual(KolmogorovSmirnovTestHypothesis.SampleIsDifferent, lillie.Hypothesis);
            Assert.AreEqual(DistributionTail.TwoTail, lillie.Tail);

            Assert.AreEqual(0.28823410018244089, lillie.Statistic, 1e-5);
            Assert.IsTrue(lillie.PValue < 0.03);

            // The null hypothesis can be rejected:
            // the sample is not from a standard Normal distribution
            Assert.IsTrue(lillie.Significant);
        }

        [Test]
        [Category("Slow")]
        public void KolmogorovSmirnovTestConstructorTest3_with_reestimation()
        {
            Accord.Math.Random.Generator.Seed = 1;

            // Test if the sample's distribution is greater than a standard Normal distribution.

            double[] sample = { 0.621, 0.503, 0.203, 0.477, 0.710, 0.581, 0.329, 0.480, 0.554, 0.382 };

            NormalDistribution distribution = NormalDistribution.Standard;
            var target = new LillieforsTest(sample, distribution,
                KolmogorovSmirnovTestHypothesis.SampleIsGreater, iterations: 1000 * 1000);

            Assert.AreEqual(distribution, target.TheoreticalDistribution);
            Assert.AreEqual(KolmogorovSmirnovTestHypothesis.SampleIsGreater, target.Hypothesis);
            Assert.AreEqual(DistributionTail.OneUpper, target.Tail);

            Assert.AreEqual(0.238852, target.Statistic, 1e-5);
            Assert.AreEqual(0.054032, target.PValue, 5e-3);
            Assert.IsFalse(Double.IsNaN(target.Statistic));
        }

        [Test]
        public void KolmogorovSmirnovTestConstructorTest4_with_reestimation()
        {
            Accord.Math.Random.Generator.Seed = 1;

            // Test if the sample's distribution is smaller than a standard Normal distribution

            double[] sample = { 0.621, 0.503, 0.203, 0.477, 0.710, 0.581, 0.329, 0.480, 0.554, 0.382 };

            NormalDistribution distribution = NormalDistribution.Standard;
            var target = new LillieforsTest(sample, distribution,
                KolmogorovSmirnovTestHypothesis.SampleIsSmaller);

            Assert.AreEqual(distribution, target.TheoreticalDistribution);
            Assert.AreEqual(KolmogorovSmirnovTestHypothesis.SampleIsSmaller, target.Hypothesis);
            Assert.AreEqual(DistributionTail.OneLower, target.Tail);

            Assert.AreEqual(0.580432, target.Statistic, 1e-5);
            Assert.AreEqual(0.000499, target.PValue, 5e-4);
            Assert.IsFalse(Double.IsNaN(target.Statistic));
        }

        [Test]
        public void EmpiricalDistributionTest_with_reestimation()
        {
            Accord.Math.Random.Generator.Seed = 1;

            double[] sample = { 1, 5, 3, 1, 5, 2, 1 };
            UnivariateContinuousDistribution distribution = NormalDistribution.Standard;

            var target = new LillieforsTest(sample, distribution);

            EmpiricalDistribution actual = target.EmpiricalDistribution;

            Assert.AreNotSame(sample, actual.Samples);

            Array.Sort(sample);

            for (int i = 0; i < sample.Length; i++)
                Assert.AreEqual(sample[i], actual.Samples[i]);
        }

        [Test]
        public void TheoreticalDistributionTest_with_reestimation()
        {
            Accord.Math.Random.Generator.Seed = 1;

            double[] sample = { 1, 5, 3, 1, 5, 2, 1 };
            UnivariateContinuousDistribution distribution = NormalDistribution.Standard;

            var target = new LillieforsTest(sample, distribution);

            ISampleableDistribution<double> actual = target.TheoreticalDistribution;
            Assert.AreEqual(distribution, actual);
        }
    }
}