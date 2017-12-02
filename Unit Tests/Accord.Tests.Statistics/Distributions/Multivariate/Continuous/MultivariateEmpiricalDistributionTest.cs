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
    using Accord.Statistics;
    using Accord.Statistics.Distributions.DensityKernels;
    using Accord.Statistics.Distributions.Multivariate;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class MultivariateEmpiricalDistributionTest
    {

        [Test]
        public void ConstructorTest4()
        {
            #region doc_fit_epanechnikov
            // Suppose we have the following data, and we would
            // like to estimate a distribution from this data

            double[][] samples =
            {
                new double[] { 0, 1 },
                new double[] { 1, 2 },
                new double[] { 5, 1 },
                new double[] { 7, 1 },
                new double[] { 6, 1 },
                new double[] { 5, 7 },
                new double[] { 2, 1 },
            };

            // Start by specifying a density kernel
            IDensityKernel kernel = new EpanechnikovKernel(dimension: 2);

            // Create a multivariate Empirical distribution from the samples
            var dist = new MultivariateEmpiricalDistribution(kernel, samples);


            // Common measures
            double[] mean = dist.Mean;     // { 3.71, 2.00 }
            double[] median = dist.Median; // { 3.71, 2.00 }
            double[] var = dist.Variance;  // { 7.23, 5.00 } (diagonal from cov)
            double[,] cov = dist.Covariance; // { { 7.23, 0.83 }, { 0.83, 5.00 } }

            // Probability mass functions
            double pdf1 = dist.ProbabilityDensityFunction(new double[] { 2, 1 }); // 0.039131176997318849
            double pdf2 = dist.ProbabilityDensityFunction(new double[] { 4, 2 }); // 0.010212109770266639
            double pdf3 = dist.ProbabilityDensityFunction(new double[] { 5, 7 }); // 0.02891906722705221
            double lpdf = dist.LogProbabilityDensityFunction(new double[] { 5, 7 }); // -3.5432541357714742
            #endregion

            Assert.AreEqual(3.7142857142857144, mean[0]);
            Assert.AreEqual(2.0, mean[1]);
            Assert.AreEqual(3.7142857142857144, median[0]);
            Assert.AreEqual(2.0, median[1]);
            Assert.AreEqual(7.2380952380952381, var[0]);
            Assert.AreEqual(5.0, var[1]);
            Assert.AreEqual(7.2380952380952381, cov[0, 0]);
            Assert.AreEqual(0.83333333333333337, cov[0, 1]);
            Assert.AreEqual(0.83333333333333337, cov[1, 0]);
            Assert.AreEqual(5.0, cov[1, 1]);
            Assert.AreEqual(0.039131176997318849, pdf1);
            Assert.AreEqual(0.010212109770266639, pdf2);
            Assert.AreEqual(0.02891906722705221, pdf3);
            Assert.AreEqual(-3.5432541357714742, lpdf);
        }

        [Test]
        public void fit_gaussian_test()
        {
            #region doc_fit_gaussian
            // Suppose we have the following data, and we would
            // like to estimate a distribution from this data

            double[][] samples =
            {
                new double[] { 0, 1 },
                new double[] { 1, 2 },
                new double[] { 5, 1 },
                new double[] { 7, 1 },
                new double[] { 6, 1 },
                new double[] { 5, 7 },
                new double[] { 2, 1 },
            };

            // Start by specifying a density kernel
            IDensityKernel kernel = new GaussianKernel(dimension: 2);

            // The density kernel gives a window function centered in a particular sample. 
            // By creating one of those windows for each sample, we can achieve an empirical 
            // multivariate distribution function. An output example for a single Gaussian 
            // kernel would be:
            double z = kernel.Function(new double[] { 0, 1 }); // should be 0.096532352630053914


            // Create a multivariate Empirical distribution from the samples
            var dist = new MultivariateEmpiricalDistribution(kernel, samples);

            // Common measures
            double[] mean = dist.Mean;       // { 3.71, 2.00 }
            double[] median = dist.Median;   // { 3.71, 2.00 }
            double[] var = dist.Variance;    // { 7.23, 5.00 } (diagonal from cov)
            double[,] cov = dist.Covariance; // { { 7.23, 0.83 }, { 0.83, 5.00 } }

            // Probability mass functions
            double pdf1 = dist.ProbabilityDensityFunction(new double[] { 2, 1 });    // 0.017657515909330332
            double pdf2 = dist.ProbabilityDensityFunction(new double[] { 4, 2 });    // 0.011581172997320841
            double pdf3 = dist.ProbabilityDensityFunction(new double[] { 5, 7 });    // 0.0072297668067630525
            double lpdf = dist.LogProbabilityDensityFunction(new double[] { 5, 7 }); // -4.929548496891365
            #endregion

            Assert.AreEqual(0.096532352630053914, z);

            Assert.AreEqual(3.7142857142857144, mean[0]);
            Assert.AreEqual(2.0, mean[1]);
            Assert.AreEqual(3.7142857142857144, median[0]);
            Assert.AreEqual(2.0, median[1]);
            Assert.AreEqual(7.2380952380952381, var[0]);
            Assert.AreEqual(5.0, var[1]);
            Assert.AreEqual(7.2380952380952381, cov[0, 0]);
            Assert.AreEqual(0.83333333333333337, cov[0, 1]);
            Assert.AreEqual(0.83333333333333337, cov[1, 0]);
            Assert.AreEqual(5.0, cov[1, 1]);
            Assert.AreEqual(0.017657515909330332, pdf1);
            Assert.AreEqual(0.011581172997320841, pdf2);
            Assert.AreEqual(0.0072297668067630525, pdf3);
            Assert.AreEqual(-4.929548496891365, lpdf);
        }




        [Test]
        public void FitTest()
        {
            double[] original = { 5, 5, 1, 4, 1, 2, 2, 3, 3, 3, 4, 3, 3, 3, 4, 3, 2, 3 };
            var distribution = new MultivariateEmpiricalDistribution(original.ToJagged());

            int[] weights = { 2, 1, 1, 1, 2, 3, 1, 3, 1, 1, 1, 1 };
            double[] sources = { 5, 1, 4, 1, 2, 3, 4, 3, 4, 3, 2, 3 };
            double[][] samples = sources.ToJagged();
            var target = new MultivariateEmpiricalDistribution(Jagged.Zeros(1, 1));

            target.Fit(samples, weights);

            Assert.AreEqual(distribution.Mean[0], target.Mean[0]);
            Assert.AreEqual(distribution.Median[0], target.Median[0]);
            Assert.AreEqual(distribution.Mode[0], target.Mode[0]);
            Assert.AreEqual(distribution.Smoothing[0, 0], target.Smoothing[0, 0]);
            Assert.AreEqual(distribution.Variance[0], target.Variance[0]);
            Assert.IsTrue(target.Weights.IsEqual(weights.Divide(weights.Sum())));
            Assert.AreEqual(target.Samples, samples);

            for (double x = 0; x < 6; x += 0.1)
            {
                double actual, expected;
                expected = distribution.ComplementaryDistributionFunction(x);
                actual = target.ComplementaryDistributionFunction(x);
                Assert.AreEqual(expected, actual);

                expected = distribution.DistributionFunction(x);
                actual = target.DistributionFunction(x);
                Assert.AreEqual(expected, actual);

                expected = distribution.LogProbabilityDensityFunction(x);
                actual = target.LogProbabilityDensityFunction(x);
                Assert.AreEqual(expected, actual, 1e-15);

                expected = distribution.ProbabilityDensityFunction(x);
                actual = target.ProbabilityDensityFunction(x);
                Assert.AreEqual(expected, actual, 1e-15);
            }
        }

        [Test]
        public void FitTest2()
        {
            double[][] observations =
            {
                new double[] { 0.1000, -0.2000 },
                new double[] { 0.4000,  0.6000 },
                new double[] { 2.0000,  0.2000 },
                new double[] { 2.0000,  0.3000 }
            };

            double[] mean = Measures.Mean(observations, dimension: 0);
            double[][] cov = Measures.Covariance(observations, dimension: 0);

            var target = new MultivariateEmpiricalDistribution(observations);

            target.Fit(observations);

            Assert.IsTrue(Matrix.IsEqual(mean, target.Mean));
            Assert.IsTrue(Matrix.IsEqual(cov, target.Covariance, 1e-10));
        }

        [Test]
        [Category("Slow")]
        public void GenerateTest1()
        {
            Accord.Math.Tools.SetupGenerator(0);

            double[] mean = { 2, 6 };

            double[,] cov =
            {
                { 2, 1 },
                { 1, 5 }
            };

            var normal = new MultivariateNormalDistribution(mean, cov);
            double[][] source = normal.Generate(10000000);

            var target = new MultivariateEmpiricalDistribution(source);

            Assert.IsTrue(mean.IsEqual(target.Mean, 0.001));
            Assert.IsTrue(cov.IsEqual(target.Covariance, 0.003));

            double[][] samples = target.Generate(10000000);

            double[] sampleMean = samples.Mean(dimension: 0);
            double[][] sampleCov = samples.Covariance();

            Assert.AreEqual(2, sampleMean[0], 1e-2);
            Assert.AreEqual(6, sampleMean[1], 1e-2);
            Assert.AreEqual(2, sampleCov[0][0], 1e-2);
            Assert.AreEqual(1, sampleCov[0][1], 1e-2);
            Assert.AreEqual(1, sampleCov[1][0], 1e-2);
            Assert.AreEqual(5, sampleCov[1][1], 2e-2);
        }


        [Test]
        public void EpanechnikovKernelTest()
        {
            EpanechnikovKernel kernel = new EpanechnikovKernel(dimension: 1);

            Assert.AreEqual(3.0 / 4.0, kernel.Constant);

            double[] actual = new double[11];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = kernel.Function((i - 5) / 10.0);

            double[] expected =
            {
                0.5625, 0.63, 0.6825, 0.72, 0.74249999999999994,
                0.75,
                0.74249999999999994, 0.72, 0.6825, 0.63, 0.5625
            };

            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);
        }

        [Test]
        public void WeightedEmpiricalDistributionConstructorTest()
        {
            double[] original = { 5, 5, 1, 4, 1, 2, 2, 3, 3, 3, 4, 3, 3, 3, 4, 3, 2, 3 };
            var distribution = new MultivariateEmpiricalDistribution(original.ToJagged());

            int[] weights = { 2, 1, 1, 1, 2, 3, 1, 3, 1, 1, 1, 1 };
            double[] sources = { 5, 1, 4, 1, 2, 3, 4, 3, 4, 3, 2, 3 };
            double[][] samples = sources.ToJagged();
            var target = new MultivariateEmpiricalDistribution(samples, weights);

            Assert.AreEqual(distribution.Mean[0], target.Mean[0]);
            Assert.AreEqual(distribution.Median[0], target.Median[0]);
            Assert.AreEqual(distribution.Mode[0], target.Mode[0]);
            Assert.AreEqual(distribution.Smoothing[0, 0], target.Smoothing[0, 0]);
            Assert.AreEqual(distribution.Variance[0], target.Variance[0]);
            Assert.IsTrue(target.Weights.IsEqual(weights.Divide(weights.Sum())));
            Assert.AreEqual(target.Samples, samples);

            for (double x = 0; x < 6; x += 0.1)
            {
                double actual, expected;
                expected = distribution.ComplementaryDistributionFunction(x);
                actual = target.ComplementaryDistributionFunction(x);
                Assert.AreEqual(expected, actual);

                expected = distribution.DistributionFunction(x);
                actual = target.DistributionFunction(x);
                Assert.AreEqual(expected, actual);

                expected = distribution.LogProbabilityDensityFunction(x);
                actual = target.LogProbabilityDensityFunction(x);
                Assert.AreEqual(expected, actual, 1e-15);

                expected = distribution.ProbabilityDensityFunction(x);
                actual = target.ProbabilityDensityFunction(x);
                Assert.AreEqual(expected, actual, 1e-15);
            }
        }

        [Test]
        public void WeightedEmpiricalDistributionConstructorTest2()
        {
            double[] original = { 5, 5, 1, 4, 1, 2, 2, 3, 3, 3, 4, 3, 3, 3, 4, 3, 2, 3 };
            var distribution = new MultivariateEmpiricalDistribution(original.ToJagged());

            double[] weights = { 2, 1, 1, 1, 2, 3, 1, 3, 1, 1, 1, 1 };
            double[] source = { 5, 1, 4, 1, 2, 3, 4, 3, 4, 3, 2, 3 };
            double[][] samples = source.ToJagged();

            weights = weights.Divide(weights.Sum());

            var target = new MultivariateEmpiricalDistribution(samples,
                weights, distribution.Smoothing);

            Assert.AreEqual(distribution.Mean[0], target.Mean[0]);
            Assert.AreEqual(distribution.Median[0], target.Median[0]);
            Assert.AreEqual(distribution.Mode[0], target.Mode[0]);
            Assert.AreEqual(distribution.Smoothing[0, 0], target.Smoothing[0, 0]);
            Assert.AreEqual(1.3655172413793104, target.Variance[0]);
            Assert.AreEqual(target.Weights, weights);
            Assert.AreEqual(target.Samples, samples);

            for (double x = 0; x < 6; x += 0.1)
            {
                double actual, expected;
                expected = distribution.ComplementaryDistributionFunction(x);
                actual = target.ComplementaryDistributionFunction(x);
                Assert.AreEqual(expected, actual, 1e-15);

                expected = distribution.DistributionFunction(x);
                actual = target.DistributionFunction(x);
                Assert.AreEqual(expected, actual, 1e-15);

                expected = distribution.LogProbabilityDensityFunction(x);
                actual = target.LogProbabilityDensityFunction(x);
                Assert.AreEqual(expected, actual, 1e-15);

                expected = distribution.ProbabilityDensityFunction(x);
                actual = target.ProbabilityDensityFunction(x);
                Assert.AreEqual(expected, actual, 1e-15);
            }
        }

        [Test]
        public void WeightedEmpiricalDistributionConstructorTest3()
        {
            double[] weights = { 2, 1, 1, 1, 2, 3, 1, 3, 1, 1, 1, 1 };
            double[] samples = { 5, 1, 4, 1, 2, 3, 4, 3, 4, 3, 2, 3 };

            weights = weights.Divide(weights.Sum());

            var target = new MultivariateEmpiricalDistribution(samples.ToJagged(), weights);

            Assert.AreEqual(1.2377597081667415, target.Smoothing[0, 0]);
        }

        [Test]
        public void WeightedEmpiricalDistribution_DistributionFunction()
        {
            double[][] samples =
            {
                new double[] { 5, 2 },
                new double[] { 1, 5 },
                new double[] { 4, 7 },
                new double[] { 1, 6 },
                new double[] { 2, 2 },
                new double[] { 3, 4 },
                new double[] { 4, 8 },
                new double[] { 3, 2 },
                new double[] { 4, 4 },
                new double[] { 3, 7 },
                new double[] { 2, 4 },
                new double[] { 3, 1 },
            };


            var target = new MultivariateEmpiricalDistribution(samples);

            double[] expected =
            {
               0.33333333333333331, 0.083333333333333329, 0.83333333333333337,
               0.16666666666666666, 0.083333333333333329, 0.41666666666666669,
               0.91666666666666663, 0.25, 0.5,
               0.66666666666666663, 0.16666666666666666, 0.083333333333333329
            };

            for (int i = 0; i < samples.Length; i++)
            {
                double e = expected[i];
                double a = target.DistributionFunction(samples[i]);
                Assert.AreEqual(e, a);
            }

        }
    }
}
