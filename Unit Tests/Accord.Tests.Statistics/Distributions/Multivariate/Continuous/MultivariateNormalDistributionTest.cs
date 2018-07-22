﻿// Accord Unit Tests
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
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;

    [TestFixture]
    public class MultivariateNormalDistributionTest
    {

        [Test]
        public void ConstructorTest1()
        {
            var normal = new NormalDistribution(4.2, 1.2);
            var target = new MultivariateNormalDistribution(new[] { 4.2 }, new[,] { { 1.2 * 1.2 } });

            double[] mean = target.Mean;
            double[] median = target.Median;
            double[] var = target.Variance;
            double[,] cov = target.Covariance;

            double apdf1 = target.ProbabilityDensityFunction(new double[] { 2 });
            double apdf2 = target.ProbabilityDensityFunction(new double[] { 4 });
            double apdf3 = target.ProbabilityDensityFunction(new double[] { 3 });
            double alpdf = target.LogProbabilityDensityFunction(new double[] { 3 });
            double acdf = target.DistributionFunction(new double[] { 3 });
            double accdf = target.ComplementaryDistributionFunction(new double[] { 3 });

            double epdf1 = normal.ProbabilityDensityFunction(2);
            double epdf2 = normal.ProbabilityDensityFunction(4);
            double epdf3 = normal.ProbabilityDensityFunction(3);
            double elpdf = normal.LogProbabilityDensityFunction(3);
            double ecdf = normal.DistributionFunction(3);
            double eccdf = normal.ComplementaryDistributionFunction(3);


            Assert.AreEqual(normal.Mean, target.Mean[0]);
            Assert.AreEqual(normal.Median, target.Median[0]);
            Assert.AreEqual(normal.Variance, target.Variance[0]);
            Assert.AreEqual(normal.Variance, target.Covariance[0, 0]);

            Assert.AreEqual(epdf1, apdf1);
            Assert.AreEqual(epdf2, apdf2);
            Assert.AreEqual(epdf3, apdf3);
            Assert.AreEqual(elpdf, alpdf);
            Assert.AreEqual(ecdf, acdf);
            Assert.AreEqual(eccdf, accdf);
            Assert.AreEqual(1.0 - ecdf, eccdf);
        }

        [Test]
        public void ConstructorTest4()
        {
            // Create a multivariate Gaussian distribution 
            var dist = new MultivariateNormalDistribution
            (
                // mean vector mu
                mean: new double[] { 4, 2 },

                // covariance matrix sigma
                covariance: new double[,]
                {
                    { 0.3, 0.1 },
                    { 0.1, 0.7 }
                }
            );

            // Common measures
            double[] mean = dist.Mean;       // { 4, 2 }
            double[] median = dist.Median;   // { 4, 2 }
            double[] mode = dist.Mode;       // { 4, 2 }
            double[,] cov = dist.Covariance; // { { 0.3, 0.1 }, { 0.1, 0.7 } }
            double[] var = dist.Variance;    // { 0.3, 0.7 } (diagonal from cov)
            int dimensions = dist.Dimension; // 2

            // Probability density functions
            double pdf1 = dist.ProbabilityDensityFunction(2, 5);    // 0.000000018917884164743237
            double pdf2 = dist.ProbabilityDensityFunction(4, 2);    // 0.35588127170858852
            double pdf3 = dist.ProbabilityDensityFunction(3, 7);    // 0.000000000036520107734505265
            double lpdf = dist.LogProbabilityDensityFunction(3, 7); // -24.033158110192296

            // Cumulative distribution function (for up to two dimensions)
            double cdf = dist.DistributionFunction(3, 5); // 0.033944035782101534
            double ccdf = dist.ComplementaryDistributionFunction(3, 5); // 0.00016755510356109232


            // compared against R package mnormt: install.packages("mnormt")
            // pmnorm(c(3,5), mean=c(4,2), varcov=matrix(c(0.3,0.1,0.1,0.7), 2,2))


            Assert.AreEqual(4, mean[0]);
            Assert.AreEqual(2, mean[1]);
            Assert.AreEqual(4, mode[0]);
            Assert.AreEqual(2, mode[1]);
            Assert.AreEqual(4, median[0]);
            Assert.AreEqual(2, median[1]);
            Assert.AreEqual(0.3, var[0]);
            Assert.AreEqual(0.7, var[1]);
            Assert.AreEqual(0.3, cov[0, 0]);
            Assert.AreEqual(0.1, cov[0, 1]);
            Assert.AreEqual(0.1, cov[1, 0]);
            Assert.AreEqual(0.7, cov[1, 1]);
            Assert.AreEqual(0.000000018917884164743237, pdf1, 1e-10);
            Assert.AreEqual(0.35588127170858852, pdf2, 1e-10);
            Assert.AreEqual(0.000000000036520107734505265, pdf3, 1e-10);
            Assert.AreEqual(-24.033158110192296, lpdf, 1e-10);
            Assert.AreEqual(0.033944035782101534, cdf, 1e-10);
        }

        [Test]
        public void ProbabilityDensityFunctionTest()
        {
            double[] mean = { 1, -1 };
            double[,] covariance =
            {
                { 0.9, 0.4 },
                { 0.4, 0.3 },
            };

            var target = new MultivariateNormalDistribution(mean, covariance);

            double[] x = { 1.2, -0.8 };
            double expected = 0.446209421363460;
            double actual = target.ProbabilityDensityFunction(x);

            Assert.AreEqual(expected, actual, 0.00000001);
        }

        [Test]
        public void LogProbabilityDensityFunctionTest()
        {
            double[] mean = { 1, -1 };
            double[,] covariance =
            {
                { 0.9, 0.4 },
                { 0.4, 0.3 },
            };

            var target = new MultivariateNormalDistribution(mean, covariance);

            double[] x = { 1.2, -0.8 };
            double expected = System.Math.Log(0.446209421363460);
            double actual = target.LogProbabilityDensityFunction(x);

            Assert.AreEqual(expected, actual, 0.00000001);
        }

        [Test]
        public void ProbabilityDensityFunctionTest2()
        {
            double[] mean = new double[64];
            double[,] covariance = bigmatrix;

            var target = new MultivariateNormalDistribution(mean, covariance);

            double expected = double.PositiveInfinity;
            double actual = target.ProbabilityDensityFunction(mean);
            Assert.AreEqual(expected, actual, 0.00000001);

            expected = 1053.6344885618446;
            actual = target.LogProbabilityDensityFunction(mean);
            Assert.AreEqual(expected, actual, 0.00000001);

            double[] x = Matrix.Diagonal(covariance).Multiply(1.5945e7);

            expected = 4.781042576287362e-12;
            actual = target.ProbabilityDensityFunction(x);
            Assert.AreEqual(expected, actual, 1e-21);

            expected = System.Math.Log(4.781042576287362e-12);
            actual = target.LogProbabilityDensityFunction(x);
            Assert.AreEqual(expected, actual, 1e-10);
        }

        [Test]
        public void ProbabilityDensityFunctionTest3()
        {
            double[] mean = new double[3];
            double[,] covariance = Matrix.Identity(3);

            var target = new MultivariateNormalDistribution(mean, covariance);

            double[] x = { 1.2, -0.8 };

            bool thrown = false;
            try
            {
                target.ProbabilityDensityFunction(x);
            }
            catch (DimensionMismatchException)
            {
                thrown = true;
            }

            Assert.IsTrue(thrown);
        }

        [Test]
        public void ProbabilityFunctionTest4()
        {
            // https://code.google.com/p/accord/issues/detail?id=98

            /*
                mean = c(0.25, 0.082)
                sigma = matrix(c(0.0117, 0.0032}, 0.0032, 0.001062), 2, 2)

                d = seq(0.03, 0.13, 0.0001)
                n <- length(d)
                r <- rep(0, n)

                for (i in 1:n) {
                  r[i] = dmnorm(c(0.25, d[i]), mean, sigma) 
                }
             */

            var target = new MultivariateNormalDistribution(
                new[] { 0.25, 0.082 },
                new[,] { { 0.0117, 0.0032 }, { 0.0032, 0.001062 } });

            double[] vec = { 0.25, -1d };

            double[] d = Matrix.Vector(0.03, 0.13, 0.01);
            double[] actual = new double[d.Length];

            double[] expected =
            {
                  0.07736363146686682512598, 0.95791683037271524447931, 6.94400533773376249513376,
                  29.47023331179536498325433, 73.22314665629953367442795, 106.51345886810220520146686,
                  90.70931216253406148553040, 45.22624649290145271152142, 13.20141558295499173425469,
                  2.25601377127287250345944, 0.22571180597171525139544, 0.2257118059717152513954
            };

            for (int i = 0; i < d.Length; i++)
            {
                vec[1] = d[i];
                actual[i] = target.ProbabilityDensityFunction(vec);
            }

            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i], 1e-12);

            for (int i = 0; i < d.Length; i++)
            {
                vec[1] = d[i];
                actual[i] = System.Math.Exp(target.LogProbabilityDensityFunction(vec));
            }

            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i], 1e-12);
        }

        [Test]
        public void CumulativeFunctionTest1()
        {
            // Comparison against dmvnorm from the mvtnorm R package

            double[] mean = { 1, -1 };

            double[,] covariance =
            {
                { 0.9, 0.4 },
                { 0.4, 0.3 },
            };

            var target = new MultivariateNormalDistribution(mean, covariance);

            double[] x = { 1.2, -0.8 };

            // dmvnorm(x=c(1.2, -0.8), mean=c(1, -1), sigma=matrix(c(0.9, 0.4, 0.4, 0.3), 2, 2))
            double pdf = target.ProbabilityDensityFunction(x);

            // pmvnorm(upper=c(1.2, -0.8), mean=c(1, -1), sigma=matrix(c(0.9, 0.4, 0.4, 0.3), 2, 2))
            double cdf = target.DistributionFunction(x);

            // pmvnorm(lower=c(1.2, -0.8), mean=c(1, -1), sigma=matrix(c(0.9, 0.4, 0.4, 0.3), 2, 2))
            double ccdf = target.ComplementaryDistributionFunction(x);

            Assert.AreEqual(0.44620942136345987, pdf);
            Assert.AreEqual(0.5049523013014460826, cdf, 1e-10);
            Assert.AreEqual(0.27896707550525140507, ccdf, 1e-10);
        }

        [Test]
        public void CumulativeFunctionTest2()
        {
            double[] mean = { 4.2 };

            double[,] covariance = { { 1.4 } };

            var baseline = new NormalDistribution(4.2, System.Math.Sqrt(covariance[0, 0]));
            var target = new MultivariateNormalDistribution(mean, covariance);

            for (int i = 0; i < 10; i++)
            {
                double x = (i - 2) / 10.0;

                {
                    double actual = target.ProbabilityDensityFunction(x);
                    double expected = baseline.ProbabilityDensityFunction(x);
                    Assert.AreEqual(expected, actual, 1e-10);
                }

                {
                    double actual = target.DistributionFunction(x);
                    double expected = baseline.DistributionFunction(x);
                    Assert.AreEqual(expected, actual);
                }

                {
                    double actual = target.ComplementaryDistributionFunction(x);
                    double expected = baseline.ComplementaryDistributionFunction(x);
                    Assert.AreEqual(expected, actual);
                }
            }
        }

        [Test]
        public void ConstructorTest()
        {
            double[] mean = { 1, -1 };
            double[,] covariance =
            {
                { 2, 1 },
                { 1, 3 }
            };

            MultivariateNormalDistribution target = new MultivariateNormalDistribution(mean, covariance);

            Assert.AreEqual(covariance, target.Covariance);
            Assert.AreEqual(mean, target.Mean);
            Assert.AreEqual(2, target.Variance.Length);
            Assert.AreEqual(2, target.Variance[0]);
            Assert.AreEqual(3, target.Variance[1]);
            Assert.AreEqual(2, target.Dimension);
        }

        [Test]
        public void ConstructorTest2()
        {
            double[] mean = { 1, -1 };
            double[,] covariance = Matrix.Identity(4);

            bool thrown = false;

            try { new MultivariateNormalDistribution(mean, covariance); }
            catch (DimensionMismatchException) { thrown = true; }

            Assert.IsTrue(thrown);
        }

        [Test]
        public void ConstructorTest3()
        {
            double[] mean = { 0, 0 };
            double[,] covariance =
            {
                { 0, 1 },
                { 1, 0 }
            };

            bool thrown = false;

            try { new MultivariateNormalDistribution(mean, covariance); }
            catch (NonPositiveDefiniteMatrixException) { thrown = true; }

            Assert.IsTrue(thrown);
        }


        [Test]
        public void FitTest()
        {
            double[][] observations =
            {
                new double[] { 0.1000, -0.2000 },
                new double[] { 0.4000,  0.6000 },
                new double[] { 2.0000,  0.2000 },
                new double[] { 2.0000,  0.3000 }
            };

            double[] mean = Measures.Mean(observations, dimension: 0);
            double[][] cov = Measures.Covariance(observations);

            {
                var target = new MultivariateNormalDistribution(2);

                double[] weigths = { 0.25, 0.25, 0.25, 0.25 };

                target.Fit(observations, weigths);

                Assert.IsTrue(Matrix.IsEqual(mean, target.Mean));
                Assert.IsTrue(Matrix.IsEqual(cov, target.Covariance, 1e-10));
            }

            {
                var target = new MultivariateNormalDistribution(2);

                double[] weigths = { 1, 1, 1, 1 };

                target.Fit(observations, weigths);

                Assert.IsTrue(Matrix.IsEqual(mean, target.Mean));
                Assert.IsTrue(Matrix.IsEqual(cov, target.Covariance, 1e-10));
            }
        }

        [Test]
        public void FitTest2()
        {
            double[][] observations =
            {
                new double[] { 1, 2 },
                new double[] { 1, 2 },
                new double[] { 1, 2 },
                new double[] { 1, 2 }
            };


            var target = new MultivariateNormalDistribution(2);

            bool thrown = false;
            try { target.Fit(observations); }
            catch (NonPositiveDefiniteMatrixException) { thrown = true; }

            Assert.IsTrue(thrown);

            NormalOptions options = new NormalOptions() { Regularization = double.Epsilon };

            // No exception thrown
            target.Fit(observations, options);
        }

        [Test]
        public void FitTest4()
        {
            double[][] observations =
            {
                new double[] { 1, 2 },
                new double[] { 1, 2 },
                new double[] { 1, 2 },
                new double[] { 1, 2 }
            };


            var target = new MultivariateNormalDistribution(2);

            bool thrown = false;
            try { target.Fit(observations); }
            catch (NonPositiveDefiniteMatrixException) { thrown = true; }

            Assert.IsTrue(thrown);

            NormalOptions options = new NormalOptions() { Robust = true };

            // No exception thrown
            target.Fit(observations, options);

            checkDegenerate(target);
        }

        private static void checkDegenerate(MultivariateNormalDistribution target)
        {
            Assert.AreEqual(1, target.Mean[0]);
            Assert.AreEqual(2, target.Mean[1]);
            Assert.AreEqual(0, target.Covariance[0, 0]);
            Assert.AreEqual(0, target.Covariance[0, 1]);
            Assert.AreEqual(0, target.Covariance[1, 0]);
            Assert.AreEqual(0, target.Covariance[1, 1]);


            // Common measures
            double[] mean = target.Mean; // { 1, 2 }
            double[] median = target.Median; // { 4, 2 }
            double[] var = target.Variance; // { 0.0, 0.0 } (diagonal from cov)
            double[,] cov = target.Covariance; // { { 0.0, 0.0 }, { 0.0, 0.0 } }

            // Probability mass functions
            double pdf1 = target.ProbabilityDensityFunction(new double[] { 1, 2 });
            double pdf2 = target.ProbabilityDensityFunction(new double[] { 4, 2 });
            double pdf3 = target.ProbabilityDensityFunction(new double[] { 3, 7 });
            double lpdf = target.LogProbabilityDensityFunction(new double[] { 3, 7 });

            // Cumulative distribution function (for up to two dimensions)
            double cdf1 = target.DistributionFunction(new double[] { 1, 2 });
            double cdf2 = target.DistributionFunction(new double[] { 3, 5 });

            double ccdf1 = target.ComplementaryDistributionFunction(new double[] { 1, 2 });
            double ccdf2 = target.ComplementaryDistributionFunction(new double[] { 3, 5 });


            Assert.AreEqual(1, mean[0]);
            Assert.AreEqual(2, mean[1]);
            Assert.AreEqual(1, median[0]);
            Assert.AreEqual(2, median[1]);
            Assert.AreEqual(0.0, var[0]);
            Assert.AreEqual(0.0, var[1]);
            Assert.AreEqual(0.0, cov[0, 0]);
            Assert.AreEqual(0.0, cov[0, 1]);
            Assert.AreEqual(0.0, cov[1, 0]);
            Assert.AreEqual(0.0, cov[1, 1]);
            Assert.AreEqual(0.15915494309189532, pdf1);
            Assert.AreEqual(0.15915494309189532, pdf2);
            Assert.AreEqual(0.15915494309189532, pdf3);
            Assert.AreEqual(-1.8378770664093456, lpdf);
            Assert.AreEqual(1.0, cdf1);
            Assert.AreEqual(0.0, cdf2);
            Assert.AreEqual(0.0, ccdf1);
            Assert.AreEqual(1.0, ccdf2);
        }

        [Test]
        public void FitTest5()
        {
            double[][] observations =
            {
                new double[] { 1, 2 },
                new double[] { 1, 2 },
                new double[] { 0, 1 },
                new double[] { 5, 7 }
            };

            double[] weights = { 1, 1, 0, 0 };

            var target = new MultivariateNormalDistribution(2);

            bool thrown = false;
            try { target.Fit(observations, weights); }
            catch (NonPositiveDefiniteMatrixException) { thrown = true; }

            Assert.IsTrue(thrown);

            NormalOptions options = new NormalOptions() { Robust = true };

            // No exception thrown
            target.Fit(observations, weights, options);

            checkDegenerate(target);
        }

        [Test]
        public void FitTest3()
        {
            double[][] observations =
            {
                new double[] { 1, 2 },
                new double[] { 2, 4 },
                new double[] { 3, 6 },
                new double[] { 4, 8 }
            };


            var target = new MultivariateNormalDistribution(2);

            NormalOptions options = new NormalOptions()
            {
                Robust = true
            };

            target.Fit(observations, options);

            double pdf = target.ProbabilityDensityFunction(4, 2);
            double cdf = target.DistributionFunction(4, 2);
            bool psd = target.Covariance.IsPositiveDefinite();

            Assert.AreEqual(0.043239154739844896, pdf);
            Assert.AreEqual(0.12263905840338646, cdf);
            Assert.IsFalse(psd);
        }

        [Test]
        public void GenerateTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            var normal = new MultivariateNormalDistribution(
                new double[] { 2, 6 },
                new double[,] { { 2, 1 }, { 1, 5 } });

            double[][] sample = normal.Generate(1000000);

            double[] mean = sample.Mean(dimension: 0);
            double[][] cov = sample.Covariance(dimension: 0);

            Assert.AreEqual(2, mean[0], 1e-2);
            Assert.AreEqual(6, mean[1], 1e-2);

            Assert.AreEqual(2, cov[0][0], 1e-2);
            Assert.AreEqual(1, cov[0][1], 1e-2);
            Assert.AreEqual(1, cov[1][0], 1e-2);
            Assert.AreEqual(5, cov[1][1], 2e-2);
        }

        [Test]
        public void RobustGenerateTest()
        {
            Accord.Math.Random.Generator.Seed = 0;

            Assert.Throws<NonPositiveDefiniteMatrixException>(() =>
            {
                new MultivariateNormalDistribution(new double[] { 2, 6, 5 },
                    new double[,] { { 2, 1, 0 }, { 1, 5, 0 }, { 0, 0, 0 } });
            });

            var normal = new MultivariateNormalDistribution(new double[] { 2, 6, 5 },
                    new double[,] { { 2, 1, 0 }, { 1, 5, 0 }, { 0, 0, 0 } }, robust: true);

            double[][] sample = normal.Generate(1000000);

            double[] mean = sample.Mean(dimension: 0);
            double[][] cov = sample.Covariance(dimension: 0);

            Assert.AreEqual(2, mean[0], 1e-2);
            Assert.AreEqual(6, mean[1], 1e-2);
            Assert.AreEqual(5, mean[2], 1e-2);

            Assert.AreEqual(2, cov[0][0], 1e-2);
            Assert.AreEqual(1, cov[0][1], 1e-2);
            Assert.AreEqual(0, cov[0][2], 1e-2);
            Assert.AreEqual(1, cov[1][0], 1e-2);
            Assert.AreEqual(5, cov[1][1], 2e-2);
            Assert.AreEqual(0, cov[1][2], 1e-2);
            Assert.AreEqual(0, cov[2][2], 1e-2);
        }

        [Test]
        public void GenerateTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            var normal = new MultivariateNormalDistribution(
                new double[] { 2, 6 },
                new double[,] { { 2, 1 }, { 1, 5 } });

            double[][] sample = new double[1000000][];
            for (int i = 0; i < sample.Length; i++)
                sample[i] = normal.Generate();

            double[] mean = sample.Mean(dimension: 0);
            double[][] cov = sample.Covariance(dimension: 0);

            Assert.AreEqual(2, mean[0], 1e-2);
            Assert.AreEqual(6, mean[1], 1e-2);

            Assert.AreEqual(2, cov[0][0], 1e-2);
            Assert.AreEqual(1, cov[0][1], 1e-2);
            Assert.AreEqual(1, cov[1][0], 1e-2);
            Assert.AreEqual(5, cov[1][1], 2e-2);
        }

        #region public static resources
        public static double[,] bigmatrix =
            {
                { 1.048096e-015, -5.053802e-016, 2.036394e-016, 6.820896e-016, 1.467011e-016, -2.737318e-016, -4.369050e-016, 3.036825e-016, -4.184581e-016, -1.091808e-016, -3.365029e-016, 3.353775e-016, -2.523041e-016, -1.826842e-016, 2.794808e-016, 4.541482e-016, 3.609930e-016, 2.895183e-016, 1.167334e-016, 1.157434e-015, 5.121779e-016, -1.160963e-015, -2.593759e-016, 1.622026e-015, -2.532413e-016, -8.570181e-016, -2.137010e-016, 1.166040e-015, -1.639603e-017, 6.790217e-018, 3.364347e-016, 5.557278e-016, 7.480495e-017, 3.262848e-016, -8.507642e-017, 3.147782e-016, 4.577319e-017, 6.569669e-016, -1.805624e-016, 6.732729e-016, 6.345911e-017, 7.119585e-016, -1.170737e-016, 2.797540e-016, -3.052987e-016, 1.639708e-016, 3.125247e-016, -8.126759e-017, 2.085539e-017, 1.623780e-016, 5.622347e-018, -2.352627e-016, 2.894019e-016, 4.566915e-016, -2.057291e-016, -5.030543e-016, -8.290240e-017, 5.142563e-016, -1.426200e-016, -5.490774e-016, -4.398897e-016, 1.128013e-016, 2.832718e-016, -1.920737e-016 },
                { -5.053802e-016, 1.551576e-015, -1.508033e-016, -6.509226e-016, -2.923112e-016, 1.439295e-015, -1.180639e-016, -9.101488e-016, 3.538945e-016, 1.456736e-015, -1.727797e-017, -1.319145e-015, 4.362115e-016, 8.764482e-016, -6.679945e-016, -1.017345e-015, -1.013080e-016, -4.735602e-016, -3.460510e-016, -1.884595e-015, -6.395983e-016, 4.327286e-016, -1.475483e-016, -2.891799e-015, 2.583690e-016, 1.528561e-017, -4.585058e-016, -2.879429e-015, 5.019580e-017, -3.457728e-016, -8.784726e-016, -1.343446e-015, 2.479692e-016, -4.651328e-016, -1.616162e-016, -8.228558e-016, -1.400325e-016, -7.551743e-016, -4.531785e-016, -2.191172e-015, -3.824775e-017, -7.597672e-016, -7.353290e-016, -1.638812e-015, 2.538120e-016, -3.728058e-016, -7.119985e-016, -2.621404e-016, 1.917454e-016, -1.374888e-016, -6.755592e-017, 4.852442e-016, -2.692793e-016, -1.864878e-016, -2.194431e-016, 2.581185e-016, 4.618606e-017, -2.655708e-016, -5.090547e-016, 1.417274e-016, 1.373150e-016, -1.583129e-016, -3.924757e-016, 2.026914e-016 },
                { 2.036394e-016, -1.508033e-016, 8.267360e-015, 8.122208e-015, -4.094694e-016, -2.948420e-015, 8.762755e-015, 1.151289e-014, -5.875355e-016, -2.591336e-015, 8.582383e-015, 1.163177e-014, -1.416100e-015, -1.039885e-015, 7.298520e-015, 7.479507e-015, -2.720932e-015, 3.515014e-015, 9.589627e-015, 1.268116e-014, -2.027682e-015, 5.071492e-015, 1.126486e-014, 2.636586e-014, 3.162623e-016, 4.288996e-015, 1.108373e-014, 2.342482e-014, -1.103754e-015, 2.559267e-015, 8.588898e-015, 1.106246e-014, -1.622702e-016, 8.325815e-016, 5.557424e-015, 8.720747e-015, -1.077560e-015, 3.409525e-015, 6.327059e-015, 1.566448e-014, -2.490264e-015, 1.957638e-015, 7.991127e-015, 1.641106e-014, -4.744235e-015, -2.035682e-015, 7.917270e-015, 1.047941e-014, -6.298442e-017, -3.184643e-015, 3.499188e-015, 5.594643e-015, -1.440227e-015, -5.922086e-015, 4.904788e-015, 9.384437e-015, -3.001985e-015, -6.741519e-015, 6.082218e-015, 9.811901e-015, -2.743231e-015, -3.540824e-015, 4.641007e-015, 5.795578e-015 },
                { 6.820896e-016, -6.509226e-016, 8.122208e-015, 9.994339e-015, -5.231195e-017, -3.966454e-015, 9.034505e-015, 1.370862e-014, -1.106331e-015, -3.647445e-015, 9.418037e-015, 1.373885e-014, -1.325602e-015, -1.545551e-015, 7.831564e-015, 8.773315e-015, -2.672484e-015, 4.144628e-015, 9.195137e-015, 1.469580e-014, -1.780017e-015, 6.025143e-015, 1.164448e-014, 3.003945e-014, 9.632697e-018, 5.436266e-015, 1.214544e-014, 2.671913e-014, -1.230634e-015, 2.841019e-015, 9.059805e-015, 1.234172e-014, -4.204511e-017, 1.054484e-015, 5.552071e-015, 9.820758e-015, -7.846479e-016, 3.481214e-015, 6.564146e-015, 1.693550e-014, -2.934109e-015, 2.178698e-015, 8.787160e-015, 1.725443e-014, -5.305945e-015, -1.848102e-015, 8.544086e-015, 1.066245e-014, -9.123086e-017, -3.230437e-015, 3.442401e-015, 5.819395e-015, -1.032417e-015, -5.845677e-015, 4.859362e-015, 9.450302e-015, -3.100671e-015, -6.349470e-015, 6.425675e-015, 9.895084e-015, -3.099841e-015, -3.418837e-015, 5.213390e-015, 6.137238e-015 },
                { 1.467011e-016, -2.923112e-016, -4.094694e-016, -5.231195e-017, 1.643797e-015, -1.293267e-016, -1.695766e-015, -8.886324e-016, -1.353601e-016, -6.275483e-016, -2.056065e-015, -1.456956e-015, 2.157140e-016, -1.517900e-016, -7.472016e-016, -7.180905e-016, 1.633047e-016, 4.886522e-016, -1.091734e-015, -9.555686e-016, 1.304185e-015, -4.466408e-017, -2.348031e-015, -2.953506e-015, -1.200130e-016, -5.679764e-016, -2.239696e-015, -2.845552e-015, 5.347068e-016, -4.366203e-016, -1.171824e-015, -1.652939e-015, -4.609141e-017, 1.791980e-016, -9.753728e-016, -9.878520e-016, 2.345139e-016, -5.503452e-016, -1.131829e-015, -1.841463e-015, 3.386465e-016, -6.450367e-016, -1.109605e-015, -1.996067e-015, 3.356757e-016, -1.710639e-016, -7.814211e-016, -1.359269e-015, -3.557514e-016, 6.160673e-016, -8.074366e-016, -1.017815e-015, 9.508320e-017, 8.753154e-016, -1.030091e-015, -1.435232e-015, 1.551666e-016, 6.027797e-016, -6.278798e-016, -1.043930e-015, 6.217766e-017, -1.237308e-017, -2.977411e-016, -4.693200e-016 },
                { -2.737318e-016, 1.439295e-015, -2.948420e-015, -3.966454e-015, -1.293267e-016, 4.694228e-015, -3.473038e-015, -6.531299e-015, 7.694099e-016, 4.348169e-015, -3.798929e-015, -7.314924e-015, 4.670913e-016, 1.210396e-015, -3.773074e-015, -4.560153e-015, 1.324743e-015, -2.496255e-015, -3.201727e-015, -5.803833e-015, 7.767131e-016, -4.697680e-015, -4.390485e-015, -1.279311e-014, -6.621526e-016, -5.136394e-015, -4.837781e-015, -1.238047e-014, -7.163193e-018, -2.202541e-015, -3.525092e-015, -5.300892e-015, 2.225164e-016, -2.823130e-016, -2.173702e-015, -3.460062e-015, 1.124164e-016, -1.092804e-015, -2.160704e-015, -5.785736e-015, 8.847407e-016, -6.044992e-016, -3.146804e-015, -5.395085e-015, 1.817129e-015, 7.340195e-016, -3.035762e-015, -3.204682e-015, -1.414425e-016, 1.225106e-015, -1.693214e-015, -1.370870e-015, -3.443036e-017, 1.695801e-015, -1.739197e-015, -2.324399e-015, 1.130960e-015, 1.892237e-015, -2.615187e-015, -2.759935e-015, 1.124806e-015, 1.202098e-015, -2.014102e-015, -1.739059e-015 },
                { -4.369050e-016, -1.180639e-016, 8.762755e-015, 9.034505e-015, -1.695766e-015, -3.473038e-015, 1.378440e-014, 1.500373e-014, 1.444695e-016, -2.871687e-015, 1.301182e-014, 1.551240e-014, -1.487719e-015, -1.278550e-015, 8.438552e-015, 9.119973e-015, -3.000566e-015, 3.560169e-015, 1.017476e-014, 1.437455e-014, -3.364791e-015, 7.162820e-015, 1.592239e-014, 3.256088e-014, 6.453790e-016, 6.311009e-015, 1.558637e-014, 2.947729e-014, -1.826712e-015, 2.853400e-015, 1.070534e-014, 1.408856e-014, 1.309865e-016, 8.290539e-017, 6.589070e-015, 1.094583e-014, -8.119950e-016, 3.340536e-015, 8.328807e-015, 1.940461e-014, -3.738840e-015, 1.787870e-015, 1.107585e-014, 2.071037e-014, -5.840063e-015, -2.610585e-015, 9.881382e-015, 1.291767e-014, 3.758058e-016, -4.459188e-015, 4.310610e-015, 7.090931e-015, -1.172588e-015, -8.117300e-015, 6.524629e-015, 1.195381e-014, -3.772436e-015, -8.462924e-015, 8.212623e-015, 1.244884e-014, -2.936802e-015, -3.899713e-015, 5.796734e-015, 6.854453e-015 },
                { 3.036825e-016, -9.101488e-016, 1.151289e-014, 1.370862e-014, -8.886324e-016, -6.531299e-015, 1.500373e-014, 2.212771e-014, -1.455836e-015, -5.401407e-015, 1.587183e-014, 2.252437e-014, -1.797339e-015, -1.937925e-015, 1.226028e-014, 1.364037e-014, -4.183822e-015, 5.602316e-015, 1.297783e-014, 2.111567e-014, -3.388757e-015, 9.840432e-015, 1.834359e-014, 4.599585e-014, 5.338065e-016, 9.383631e-015, 1.949368e-014, 4.160911e-014, -1.814851e-015, 4.480426e-015, 1.426800e-014, 1.942377e-014, 1.727654e-016, 1.150465e-015, 7.987140e-015, 1.424904e-014, -8.234320e-016, 5.480319e-015, 9.518282e-015, 2.490622e-014, -4.761227e-015, 3.224767e-015, 1.358029e-014, 2.610336e-014, -7.783835e-015, -2.786904e-015, 1.260078e-014, 1.640830e-014, 3.921548e-016, -5.259668e-015, 5.168696e-015, 8.698436e-015, -1.129702e-015, -9.226524e-015, 7.343120e-015, 1.429777e-014, -4.992313e-015, -9.837584e-015, 9.941604e-015, 1.511715e-014, -4.185564e-015, -5.019759e-015, 7.523783e-015, 9.172326e-015 },
                { -4.184581e-016, 3.538945e-016, -5.875355e-016, -1.106331e-015, -1.353601e-016, 7.694099e-016, 1.444695e-016, -1.455836e-015, 2.725304e-015, 6.036791e-016, -1.193070e-015, -1.683002e-015, 1.210370e-017, 2.555556e-016, -1.543094e-015, -1.545754e-015, 2.376756e-016, 1.393141e-016, -6.585961e-016, -2.023479e-015, 2.167622e-016, 1.280524e-015, -2.490605e-016, -3.845130e-015, 1.519856e-015, -3.947496e-016, -1.270556e-015, -3.994659e-015, 2.479658e-016, -1.156886e-015, -1.507562e-015, -1.978646e-015, 1.162201e-016, -7.132420e-016, -5.605055e-016, -1.843959e-015, -2.613590e-016, -1.414035e-015, -1.287277e-015, -3.788408e-015, 4.313093e-016, -1.975010e-015, -1.282544e-015, -3.331571e-015, 1.155219e-015, -7.307182e-016, -1.425942e-015, -1.689568e-015, 2.254442e-016, 7.329589e-017, -3.975259e-016, -5.639408e-016, -4.681945e-016, 3.059403e-017, -4.586737e-016, -8.849318e-016, 3.319171e-016, 2.309829e-016, -7.819261e-016, -1.262509e-015, 6.038323e-016, -3.693910e-017, -8.831052e-016, -6.698485e-016 },
                { -1.091808e-016, 1.456736e-015, -2.591336e-015, -3.647445e-015, -6.275483e-016, 4.348169e-015, -2.871687e-015, -5.401407e-015, 6.036791e-016, 5.452826e-015, -2.936219e-015, -6.336619e-015, 5.821079e-016, 1.849702e-015, -3.412604e-015, -4.203399e-015, 1.024622e-015, -1.960982e-015, -2.867489e-015, -5.188135e-015, 7.993608e-016, -3.744933e-015, -3.535317e-015, -1.033820e-014, -2.787736e-016, -4.374856e-015, -4.093562e-015, -1.071110e-014, -3.220385e-016, -1.909115e-015, -3.106123e-015, -4.635043e-015, 2.034297e-016, -7.735621e-016, -2.241054e-015, -3.468452e-015, 2.170940e-016, -1.182212e-015, -1.963696e-015, -5.330873e-015, 7.091352e-016, -7.977098e-016, -3.036998e-015, -5.314905e-015, 1.414421e-015, 5.106886e-016, -2.776884e-015, -3.027627e-015, 9.822743e-017, 7.747388e-016, -1.626628e-015, -9.508070e-016, 2.195044e-016, 1.202030e-015, -1.576738e-015, -1.703428e-015, 1.139538e-015, 1.639088e-015, -2.682543e-015, -2.366028e-015, 7.459614e-016, 9.466835e-016, -1.699010e-015, -1.400862e-015 },
                { -3.365029e-016, -1.727797e-017, 8.582383e-015, 9.418037e-015, -2.056065e-015, -3.798929e-015, 1.301182e-014, 1.587183e-014, -1.193070e-015, -2.936219e-015, 1.584357e-014, 1.667734e-014, -1.571700e-015, -1.318492e-015, 1.015294e-014, 1.004307e-014, -3.233982e-015, 3.448704e-015, 1.055781e-014, 1.642983e-014, -4.062914e-015, 6.725764e-015, 1.635512e-014, 3.624266e-014, -4.968273e-016, 6.363757e-015, 1.866079e-014, 3.317400e-014, -1.912355e-015, 3.022839e-015, 1.232152e-014, 1.536286e-014, 4.795851e-016, 6.673239e-016, 6.937242e-015, 1.266983e-014, -1.043866e-015, 4.899545e-015, 9.056522e-015, 2.229884e-014, -4.983253e-015, 3.415042e-015, 1.293893e-014, 2.363962e-014, -6.802401e-015, -2.111009e-015, 1.085278e-014, 1.394105e-014, 7.027911e-016, -4.798515e-015, 4.616124e-015, 7.778961e-015, -9.538378e-016, -8.451137e-015, 7.108199e-015, 1.278091e-014, -4.624710e-015, -8.737082e-015, 8.967558e-015, 1.318305e-014, -3.235571e-015, -3.540275e-015, 5.915582e-015, 7.185095e-015 },
                { 3.353775e-016, -1.319145e-015, 1.163177e-014, 1.373885e-014, -1.456956e-015, -7.314924e-015, 1.551240e-014, 2.252437e-014, -1.683002e-015, -6.336619e-015, 1.667734e-014, 2.510963e-014, -1.909599e-015, -2.080679e-015, 1.311002e-014, 1.537870e-014, -4.279660e-015, 5.343667e-015, 1.329072e-014, 2.199082e-014, -4.285509e-015, 9.553159e-015, 1.913075e-014, 4.852988e-014, 6.881812e-016, 9.969758e-015, 2.047348e-014, 4.475449e-014, -1.598754e-015, 5.040763e-015, 1.497972e-014, 2.085685e-014, 3.448242e-016, 1.503374e-015, 8.518227e-015, 1.496812e-014, -1.188982e-015, 6.385343e-015, 9.821431e-015, 2.618565e-014, -4.436112e-015, 4.460381e-015, 1.389180e-014, 2.740826e-014, -7.850133e-015, -2.492630e-015, 1.320356e-014, 1.684478e-014, 5.466665e-016, -5.534194e-015, 5.707979e-015, 8.966900e-015, -1.018944e-015, -9.424052e-015, 7.649458e-015, 1.464488e-014, -4.867972e-015, -9.911077e-015, 1.031633e-014, 1.538045e-014, -4.362481e-015, -5.112001e-015, 7.946966e-015, 9.411838e-015 },
                { -2.523041e-016, 4.362115e-016, -1.416100e-015, -1.325602e-015, 2.157140e-016, 4.670913e-016, -1.487719e-015, -1.797339e-015, 1.210370e-017, 5.821079e-016, -1.571700e-015, -1.909599e-015, 2.013223e-015, 6.318751e-016, -2.056715e-015, -1.734843e-015, 2.721035e-016, -1.955414e-016, -2.602970e-015, -3.889605e-015, 3.717720e-016, 6.491575e-016, -3.068883e-015, -6.721157e-015, 3.347703e-016, 7.216961e-016, -2.934763e-015, -6.440512e-015, 1.498886e-015, -6.037200e-016, -2.782547e-015, -3.644998e-015, -1.584168e-016, -5.254235e-016, -1.807938e-015, -2.827154e-015, 3.625135e-016, -1.731638e-015, -2.250862e-015, -5.314900e-015, 6.635708e-016, -1.419813e-015, -2.831597e-015, -5.767468e-015, 1.057360e-015, -5.522236e-016, -2.066438e-015, -2.499200e-015, -1.338563e-017, 5.480694e-016, -1.113921e-015, -1.350543e-015, 2.965751e-016, 1.333942e-015, -1.649404e-015, -2.409481e-015, 6.238566e-016, 1.270692e-015, -1.479745e-015, -2.253154e-015, 2.138560e-016, 2.139728e-016, -8.517958e-016, -9.117066e-016 },
                { -1.826842e-016, 8.764482e-016, -1.039885e-015, -1.545551e-015, -1.517900e-016, 1.210396e-015, -1.278550e-015, -1.937925e-015, 2.555556e-016, 1.849702e-015, -1.318492e-015, -2.080679e-015, 6.318751e-016, 1.951647e-015, -1.210583e-015, -1.689149e-015, 4.595743e-016, -2.972446e-016, -1.859089e-015, -2.831872e-015, 9.572787e-017, 2.775378e-016, -1.571733e-015, -4.314127e-015, 9.464948e-016, 6.899673e-017, -1.864711e-015, -4.650730e-015, 2.922610e-016, -5.044782e-016, -1.685565e-015, -2.926883e-015, 5.510304e-016, -7.480046e-016, -1.458405e-015, -2.312100e-015, 3.126794e-016, -1.102268e-015, -1.587019e-015, -4.088093e-015, 5.282090e-016, -1.201103e-015, -2.165203e-015, -4.376738e-015, 4.815085e-016, -6.478230e-016, -1.444282e-015, -2.011743e-015, 3.591551e-016, 3.302071e-016, -8.295170e-016, -5.714610e-016, 2.821740e-016, 6.438415e-016, -1.024047e-015, -1.138182e-015, 3.868608e-016, 7.990021e-016, -1.430887e-015, -1.341726e-015, 1.645824e-017, 1.162304e-016, -7.027954e-016, -6.169391e-016 },
                { 2.794808e-016, -6.679945e-016, 7.298520e-015, 7.831564e-015, -7.472016e-016, -3.773074e-015, 8.438552e-015, 1.226028e-014, -1.543094e-015, -3.412604e-015, 1.015294e-014, 1.311002e-014, -2.056715e-015, -1.210583e-015, 1.034879e-014, 9.274784e-015, -2.580729e-015, 3.122567e-015, 9.122293e-015, 1.412966e-014, -2.514451e-015, 4.313109e-015, 1.186928e-014, 2.989879e-014, -3.116688e-016, 4.620587e-015, 1.327059e-014, 2.803731e-014, -1.959221e-015, 3.006115e-015, 1.174141e-014, 1.383253e-014, 4.628616e-016, 1.407114e-015, 5.856996e-015, 1.040709e-014, -1.003683e-015, 5.185674e-015, 7.173265e-015, 1.842048e-014, -3.154923e-015, 3.659759e-015, 9.782677e-015, 1.948125e-014, -5.750058e-015, -1.695590e-015, 9.480677e-015, 1.216706e-014, 3.765099e-016, -3.245852e-015, 3.664248e-015, 5.767774e-015, -9.124762e-016, -5.974122e-015, 5.304185e-015, 9.490865e-015, -4.184168e-015, -7.036117e-015, 7.422734e-015, 1.041049e-014, -2.666599e-015, -3.048426e-015, 5.270601e-015, 6.043312e-015 },
                { 4.541482e-016, -1.017345e-015, 7.479507e-015, 8.773315e-015, -7.180905e-016, -4.560153e-015, 9.119973e-015, 1.364037e-014, -1.545754e-015, -4.203399e-015, 1.004307e-014, 1.537870e-014, -1.734843e-015, -1.689149e-015, 9.274784e-015, 1.061492e-014, -2.779758e-015, 3.360785e-015, 9.065042e-015, 1.466745e-014, -2.679293e-015, 4.897458e-015, 1.193616e-014, 3.126412e-014, -2.697622e-016, 5.472207e-015, 1.276929e-014, 2.962936e-014, -1.361121e-015, 3.408366e-015, 1.047776e-014, 1.467478e-014, 8.018591e-017, 1.190087e-015, 5.939745e-015, 1.047712e-014, -1.032892e-015, 4.496522e-015, 6.827502e-015, 1.837497e-014, -2.619974e-015, 3.522163e-015, 9.087533e-015, 1.919905e-014, -5.091757e-015, -1.239939e-015, 8.771987e-015, 1.182484e-014, 2.250491e-016, -3.347019e-015, 3.867975e-015, 5.907793e-015, -8.273823e-016, -5.725462e-015, 4.956404e-015, 9.448197e-015, -3.278078e-015, -6.446398e-015, 6.783740e-015, 1.006794e-014, -2.661641e-015, -3.214094e-015, 5.174717e-015, 6.152235e-015 },
                { 3.609930e-016, -1.013080e-016, -2.720932e-015, -2.672484e-015, 1.633047e-016, 1.324743e-015, -3.000566e-015, -4.183822e-015, 2.376756e-016, 1.024622e-015, -3.233982e-015, -4.279660e-015, 2.721035e-016, 4.595743e-016, -2.580729e-015, -2.779758e-015, 2.296953e-015, -1.475726e-015, -3.746114e-015, -4.354258e-015, 1.056552e-015, -2.116496e-015, -4.382785e-015, -1.023047e-014, -2.053736e-017, -1.716515e-015, -4.411145e-015, -9.763425e-015, -3.286387e-016, -9.876530e-016, -2.898902e-015, -4.513414e-015, 7.605677e-016, 8.641983e-017, -2.355700e-015, -3.223079e-015, 4.892843e-016, -1.406771e-015, -2.970239e-015, -6.354595e-015, 1.081001e-015, -1.100961e-015, -3.400951e-015, -6.781461e-015, 1.138953e-015, 7.843396e-016, -2.656082e-015, -4.348254e-015, 8.277760e-018, 1.130824e-015, -1.432374e-015, -2.160447e-015, 6.772822e-016, 2.161953e-015, -1.948516e-015, -3.528496e-015, 1.248705e-015, 2.701486e-015, -2.627791e-015, -3.995654e-015, 7.477919e-016, 1.327797e-015, -1.610431e-015, -2.360289e-015 },
                { 2.895183e-016, -4.735602e-016, 3.515014e-015, 4.144628e-015, 4.886522e-016, -2.496255e-015, 3.560169e-015, 5.602316e-015, 1.393141e-016, -1.960982e-015, 3.448704e-015, 5.343667e-015, -1.955414e-016, -2.972446e-016, 3.122567e-015, 3.360785e-015, -1.475726e-015, 5.700140e-015, 3.195177e-015, 4.862674e-015, -6.505254e-017, 8.506216e-015, 3.828110e-015, 1.032836e-014, 5.961718e-016, 6.827389e-015, 4.015702e-015, 9.240960e-015, -1.751176e-016, 2.691200e-015, 2.905491e-015, 3.976242e-015, -5.566695e-016, -1.454940e-015, 8.058062e-016, 1.544727e-015, -9.719250e-016, -1.838851e-015, 1.356835e-015, 3.171339e-015, -1.199158e-015, -2.563932e-015, 2.036758e-015, 3.245409e-015, -2.075866e-015, -2.673738e-015, 2.159504e-015, 2.354633e-015, -1.861790e-016, -1.727986e-015, 5.845241e-016, 1.689241e-015, -6.152364e-016, -2.584689e-015, 7.631333e-016, 2.444445e-015, -1.609677e-015, -3.225398e-015, 1.747401e-015, 2.972356e-015, -1.218803e-015, -2.076810e-015, 1.464546e-015, 1.790899e-015 },
                { 1.167334e-016, -3.460510e-016, 9.589627e-015, 9.195137e-015, -1.091734e-015, -3.201727e-015, 1.017476e-014, 1.297783e-014, -6.585961e-016, -2.867489e-015, 1.055781e-014, 1.329072e-014, -2.602970e-015, -1.859089e-015, 9.122293e-015, 9.065042e-015, -3.746114e-015, 3.195177e-015, 1.408720e-014, 1.805942e-014, -2.812801e-015, 3.309953e-015, 1.521860e-014, 3.457221e-014, -2.049153e-016, 2.874426e-015, 1.502886e-014, 3.101998e-014, -2.111357e-015, 2.411739e-015, 1.182969e-014, 1.517630e-014, -5.583246e-016, 1.287025e-015, 9.213091e-015, 1.338821e-014, -1.664495e-015, 5.239069e-015, 1.010697e-014, 2.366506e-014, -3.813867e-015, 3.264083e-015, 1.212660e-014, 2.462851e-014, -5.526637e-015, -1.777181e-015, 1.042083e-014, 1.476378e-014, -1.865644e-016, -3.530971e-015, 5.400430e-015, 7.354908e-015, -2.315977e-015, -7.349458e-015, 7.580283e-015, 1.279406e-014, -3.630658e-015, -7.988312e-015, 8.263134e-015, 1.264779e-014, -2.620275e-015, -3.285103e-015, 5.404040e-015, 6.968812e-015 },
                { 1.157434e-015, -1.884595e-015, 1.268116e-014, 1.469580e-014, -9.555686e-016, -5.803833e-015, 1.437455e-014, 2.111567e-014, -2.023479e-015, -5.188135e-015, 1.642983e-014, 2.199082e-014, -3.889605e-015, -2.831872e-015, 1.412966e-014, 1.466745e-014, -4.354258e-015, 4.862674e-015, 1.805942e-014, 3.100622e-014, -2.957195e-015, 4.551165e-015, 2.179527e-014, 5.856956e-014, -7.373405e-016, 4.763454e-015, 2.348581e-014, 5.177426e-014, -3.210996e-015, 3.902076e-015, 1.829840e-014, 2.419220e-014, 1.833512e-017, 3.355606e-015, 1.119818e-014, 2.107232e-014, -1.593041e-015, 9.697292e-015, 1.350334e-014, 3.602781e-014, -5.516231e-015, 7.000662e-015, 1.776649e-014, 3.644931e-014, -9.252488e-015, -1.762500e-015, 1.593524e-014, 2.096113e-014, 1.050491e-017, -4.996767e-015, 6.103428e-015, 1.007302e-014, -2.131535e-015, -1.033102e-014, 9.525736e-015, 1.718625e-014, -5.520750e-015, -1.086132e-014, 1.152556e-014, 1.745243e-014, -4.579548e-015, -4.469756e-015, 8.448931e-015, 1.019888e-014 },
                { 5.121779e-016, -6.395983e-016, -2.027682e-015, -1.780017e-015, 1.304185e-015, 7.767131e-016, -3.364791e-015, -3.388757e-015, 2.167622e-016, 7.993608e-016, -4.062914e-015, -4.285509e-015, 3.717720e-016, 9.572787e-017, -2.514451e-015, -2.679293e-015, 1.056552e-015, -6.505254e-017, -2.812801e-015, -2.957195e-015, 2.775569e-015, -1.263901e-015, -4.851350e-015, -7.872517e-015, -3.173682e-016, -1.601480e-015, -4.782555e-015, -8.071287e-015, 4.393959e-016, -8.966974e-016, -2.795912e-015, -3.880067e-015, 1.049891e-016, -1.413173e-017, -2.464324e-015, -3.072064e-015, 6.668616e-016, -1.412356e-015, -2.928605e-015, -5.577314e-015, 8.826338e-016, -1.613866e-015, -3.261308e-015, -6.419347e-015, 1.172326e-015, 1.204474e-016, -2.651556e-015, -4.144417e-015, -1.477529e-016, 9.598220e-016, -1.832362e-015, -2.099103e-015, 5.203181e-016, 1.754124e-015, -2.214831e-015, -3.295873e-015, 9.715852e-016, 1.943875e-015, -2.344040e-015, -3.405742e-015, 5.715404e-016, 7.521520e-016, -1.540936e-015, -2.041665e-015 },
                { -1.160963e-015, 4.327286e-016, 5.071492e-015, 6.025143e-015, -4.466408e-017, -4.697680e-015, 7.162820e-015, 9.840432e-015, 1.280524e-015, -3.744933e-015, 6.725764e-015, 9.553159e-015, 6.491575e-016, 2.775378e-016, 4.313109e-015, 4.897458e-015, -2.116496e-015, 8.506216e-015, 3.309953e-015, 4.551165e-015, -1.263901e-015, 1.943975e-014, 6.125966e-015, 1.337398e-014, 2.259315e-015, 1.634148e-014, 6.271515e-015, 1.197538e-014, -3.387333e-016, 5.221486e-015, 3.011958e-015, 4.598034e-015, 2.297645e-016, -3.255956e-015, 6.107939e-016, 9.502410e-016, -6.706147e-016, -5.615130e-015, 2.311070e-016, -3.460004e-016, -1.792144e-015, -6.999392e-015, 1.323763e-015, -1.952106e-016, -3.029561e-015, -4.854799e-015, 2.127188e-015, 1.820352e-015, 3.968966e-016, -3.177408e-015, 7.214026e-016, 3.071905e-015, -8.794632e-016, -4.583487e-015, 7.280363e-016, 4.064776e-015, -1.907406e-015, -5.361735e-015, 2.060978e-015, 4.410368e-015, -1.570005e-015, -3.681980e-015, 1.724388e-015, 2.867989e-015 },
                { -2.593759e-016, -1.475483e-016, 1.126486e-014, 1.164448e-014, -2.348031e-015, -4.390485e-015, 1.592239e-014, 1.834359e-014, -2.490605e-016, -3.535317e-015, 1.635512e-014, 1.913075e-014, -3.068883e-015, -1.571733e-015, 1.186928e-014, 1.193616e-014, -4.382785e-015, 3.828110e-015, 1.521860e-014, 2.179527e-014, -4.851350e-015, 6.125966e-015, 2.271112e-014, 4.656853e-014, 7.239173e-016, 5.640071e-015, 2.196653e-014, 4.207291e-014, -2.845821e-015, 3.408561e-015, 1.548282e-014, 1.993050e-014, -1.415485e-016, 9.217873e-016, 1.006050e-014, 1.653368e-014, -1.520827e-015, 6.554053e-015, 1.323954e-014, 2.945111e-014, -4.937737e-015, 4.434643e-015, 1.639402e-014, 3.108902e-014, -7.711131e-015, -2.771313e-015, 1.406893e-014, 1.879819e-014, 3.208089e-016, -5.241248e-015, 6.104665e-015, 9.603167e-015, -1.923670e-015, -1.034374e-014, 9.362646e-015, 1.619008e-014, -5.160910e-015, -1.077167e-014, 1.088007e-014, 1.646902e-014, -3.804329e-015, -4.431427e-015, 7.750148e-015, 9.105728e-015 },
                { 1.622026e-015, -2.891799e-015, 2.636586e-014, 3.003945e-014, -2.953506e-015, -1.279311e-014, 3.256088e-014, 4.599585e-014, -3.845130e-015, -1.033820e-014, 3.624266e-014, 4.852988e-014, -6.721157e-015, -4.314127e-015, 2.989879e-014, 3.126412e-014, -1.023047e-014, 1.032836e-014, 3.457221e-014, 5.856956e-014, -7.872517e-015, 1.337398e-014, 4.656853e-014, 1.223920e-013, 4.126691e-016, 1.377366e-014, 4.913664e-014, 1.096645e-013, -4.779719e-015, 9.873265e-015, 3.665644e-014, 5.037350e-014, 1.381956e-016, 5.559466e-015, 2.097102e-014, 3.913015e-014, -2.585133e-015, 1.960354e-014, 2.619206e-014, 6.918233e-014, -9.923741e-015, 1.474792e-014, 3.406367e-014, 7.084443e-014, -1.840004e-014, -4.532894e-015, 3.160487e-014, 4.247748e-014, 1.025012e-015, -1.109122e-014, 1.203930e-014, 2.035511e-014, -3.432711e-015, -2.147045e-014, 1.847983e-014, 3.425657e-014, -1.167086e-014, -2.346643e-014, 2.344367e-014, 3.615289e-014, -1.019776e-014, -1.092889e-014, 1.794292e-014, 2.168970e-014 },
                { -2.532413e-016, 2.583690e-016, 3.162623e-016, 9.632697e-018, -1.200130e-016, -6.621526e-016, 6.453790e-016, 5.338065e-016, 1.519856e-015, -2.787736e-016, -4.968273e-016, 6.881812e-016, 3.347703e-016, 9.464948e-016, -3.116688e-016, -2.697622e-016, -2.053736e-017, 5.961718e-016, -2.049153e-016, -7.373405e-016, -3.173682e-016, 2.259315e-015, 7.239173e-016, 4.126691e-016, 3.090914e-015, 1.867129e-015, -5.527126e-016, -1.262462e-016, 2.245585e-016, 2.366758e-016, -6.211341e-016, -9.783373e-016, 3.038817e-016, -3.365358e-016, -3.858399e-016, -1.878439e-015, -7.799649e-017, -5.251117e-016, -1.170405e-015, -3.475610e-015, 1.335788e-015, -1.513940e-015, -1.797173e-015, -3.292045e-015, 4.100093e-016, -1.220933e-015, -6.149063e-016, -1.008161e-015, 3.083875e-016, -1.514190e-016, -4.459048e-017, -4.694759e-016, -2.328841e-016, -3.410883e-016, -4.262807e-016, -6.719495e-016, 3.625608e-016, -2.551205e-016, -7.106469e-016, -6.779124e-016, -1.926219e-016, -6.970103e-016, -1.645955e-016, 1.051744e-016 },
                { -8.570181e-016, 1.528561e-017, 4.288996e-015, 5.436266e-015, -5.679764e-016, -5.136394e-015, 6.311009e-015, 9.383631e-015, -3.947496e-016, -4.374856e-015, 6.363757e-015, 9.969758e-015, 7.216961e-016, 6.899673e-017, 4.620587e-015, 5.472207e-015, -1.716515e-015, 6.827389e-015, 2.874426e-015, 4.763454e-015, -1.601480e-015, 1.634148e-014, 5.640071e-015, 1.377366e-014, 1.867129e-015, 1.667189e-014, 5.709447e-015, 1.303783e-014, -4.998203e-016, 6.222634e-015, 3.261780e-015, 5.259952e-015, 1.377464e-016, -2.136527e-015, 8.023102e-016, 1.317191e-015, -3.134248e-017, -3.937308e-015, 4.282304e-016, 5.443410e-016, -7.830649e-016, -5.175351e-015, 7.333851e-016, 2.609884e-016, -3.150011e-015, -3.745026e-015, 2.054857e-015, 1.848862e-015, 3.939330e-016, -2.922935e-015, 9.603960e-016, 2.734235e-015, -3.651354e-016, -3.822234e-015, 5.327429e-016, 3.336137e-015, -1.433981e-015, -4.577949e-015, 1.756654e-015, 3.837701e-015, -1.567646e-015, -3.177154e-015, 1.608873e-015, 2.513914e-015 },
                { -2.137010e-016, -4.585058e-016, 1.108373e-014, 1.214544e-014, -2.239696e-015, -4.837781e-015, 1.558637e-014, 1.949368e-014, -1.270556e-015, -4.093562e-015, 1.866079e-014, 2.047348e-014, -2.934763e-015, -1.864711e-015, 1.327059e-014, 1.276929e-014, -4.411145e-015, 4.015702e-015, 1.502886e-014, 2.348581e-014, -4.782555e-015, 6.271515e-015, 2.196653e-014, 4.913664e-014, -5.527126e-016, 5.709447e-015, 2.477163e-014, 4.475036e-014, -2.724284e-015, 3.150780e-015, 1.696364e-014, 2.074471e-014, 3.594055e-016, 1.484383e-015, 9.986729e-015, 1.791618e-014, -1.524261e-015, 7.603355e-015, 1.329471e-014, 3.169490e-014, -6.495721e-015, 5.394453e-015, 1.835044e-014, 3.338375e-014, -8.579407e-015, -2.550421e-015, 1.497435e-014, 1.950591e-014, 4.697692e-016, -5.453610e-015, 6.100114e-015, 9.915064e-015, -1.665947e-015, -1.088443e-014, 9.923411e-015, 1.710305e-014, -6.055035e-015, -1.123958e-014, 1.206602e-014, 1.749810e-014, -3.959089e-015, -4.223915e-015, 8.048559e-015, 9.545379e-015 },
                { 1.166040e-015, -2.879429e-015, 2.342482e-014, 2.671913e-014, -2.845552e-015, -1.238047e-014, 2.947729e-014, 4.160911e-014, -3.994659e-015, -1.071110e-014, 3.317400e-014, 4.475449e-014, -6.440512e-015, -4.650730e-015, 2.803731e-014, 2.962936e-014, -9.763425e-015, 9.240960e-015, 3.101998e-014, 5.177426e-014, -8.071287e-015, 1.197538e-014, 4.207291e-014, 1.096645e-013, -1.262462e-016, 1.303783e-014, 4.475036e-014, 1.031350e-013, -3.919931e-015, 9.799453e-015, 3.433568e-014, 4.897916e-014, 1.546440e-016, 4.676970e-015, 1.928048e-014, 3.564554e-014, -2.344673e-015, 1.826102e-014, 2.439049e-014, 6.428013e-014, -9.106157e-015, 1.439483e-014, 3.181945e-014, 6.750844e-014, -1.599807e-014, -3.917839e-015, 2.871639e-014, 4.001444e-014, 8.578778e-016, -1.029736e-014, 1.115840e-014, 1.870246e-014, -3.163871e-015, -2.058810e-014, 1.717962e-014, 3.252816e-014, -1.103776e-014, -2.293811e-014, 2.235547e-014, 3.495480e-014, -8.478020e-015, -1.034268e-014, 1.614264e-014, 2.039045e-014 },
                { -1.639603e-017, 5.019580e-017, -1.103754e-015, -1.230634e-015, 5.347068e-016, -7.163193e-018, -1.826712e-015, -1.814851e-015, 2.479658e-016, -3.220385e-016, -1.912355e-015, -1.598754e-015, 1.498886e-015, 2.922610e-016, -1.959221e-015, -1.361121e-015, -3.286387e-016, -1.751176e-016, -2.111357e-015, -3.210996e-015, 4.393959e-016, -3.387333e-016, -2.845821e-015, -4.779719e-015, 2.245585e-016, -4.998203e-016, -2.724284e-015, -3.919931e-015, 3.299953e-015, -1.119112e-016, -3.002271e-015, -2.522407e-015, -3.217077e-016, -3.077778e-017, -1.772276e-015, -2.850799e-015, 2.409056e-016, -2.275508e-016, -2.447141e-015, -4.660041e-015, 9.188391e-016, 1.085826e-016, -3.083998e-015, -4.815425e-015, 2.325467e-015, -5.370087e-016, -2.864144e-015, -2.245580e-015, 1.572459e-016, 4.350302e-016, -9.344564e-016, -1.359872e-015, 1.163092e-016, 9.816399e-016, -1.689618e-015, -2.162564e-015, 5.469043e-016, 7.260658e-016, -1.635858e-015, -1.896980e-015, 3.654730e-016, -2.533739e-016, -1.299298e-015, -8.175961e-016 },
                { 6.790217e-018, -3.457728e-016, 2.559267e-015, 2.841019e-015, -4.366203e-016, -2.202541e-015, 2.853400e-015, 4.480426e-015, -1.156886e-015, -1.909115e-015, 3.022839e-015, 5.040763e-015, -6.037200e-016, -5.044782e-016, 3.006115e-015, 3.408366e-015, -9.876530e-016, 2.691200e-015, 2.411739e-015, 3.902076e-015, -8.966974e-016, 5.221486e-015, 3.408561e-015, 9.873265e-015, 2.366758e-016, 6.222634e-015, 3.150780e-015, 9.799453e-015, -1.119112e-016, 4.654432e-015, 2.769611e-015, 5.110816e-015, -2.856052e-016, 1.060975e-017, 6.953998e-016, 1.347781e-015, -2.800432e-016, 3.332615e-016, 9.317416e-016, 3.021403e-015, 6.134835e-016, 7.662096e-017, 3.596282e-016, 3.015941e-015, -1.039238e-015, -9.443091e-016, 1.075008e-015, 2.190588e-015, -4.112685e-017, -1.631570e-015, 8.075534e-016, 1.625463e-015, -1.617381e-016, -2.284659e-015, 4.071668e-016, 2.254364e-015, -5.207112e-016, -2.869232e-015, 5.339528e-016, 2.676958e-015, -6.431077e-016, -1.833488e-015, 6.082709e-016, 1.574258e-015 },
                { 3.364347e-016, -8.784726e-016, 8.588898e-015, 9.059805e-015, -1.171824e-015, -3.525092e-015, 1.070534e-014, 1.426800e-014, -1.507562e-015, -3.106123e-015, 1.232152e-014, 1.497972e-014, -2.782547e-015, -1.685565e-015, 1.174141e-014, 1.047776e-014, -2.898902e-015, 2.905491e-015, 1.182969e-014, 1.829840e-014, -2.795912e-015, 3.011958e-015, 1.548282e-014, 3.665644e-014, -6.211341e-016, 3.261780e-015, 1.696364e-014, 3.433568e-014, -3.002271e-015, 2.769611e-015, 1.618364e-014, 1.798101e-014, 4.053666e-016, 1.898121e-015, 7.726329e-015, 1.395972e-014, -1.185205e-015, 6.833963e-015, 9.998665e-015, 2.523531e-014, -4.332444e-015, 4.715295e-015, 1.395286e-014, 2.684530e-014, -7.357252e-015, -2.299225e-015, 1.315247e-014, 1.640273e-014, 1.879908e-016, -4.090338e-015, 4.507755e-015, 7.446866e-015, -1.454407e-015, -8.421186e-015, 7.518237e-015, 1.322442e-014, -5.244888e-015, -9.346030e-015, 9.977997e-015, 1.423217e-014, -2.883029e-015, -3.287211e-015, 6.673327e-015, 7.880065e-015 },
                { 5.557278e-016, -1.343446e-015, 1.106246e-014, 1.234172e-014, -1.652939e-015, -5.300892e-015, 1.408856e-014, 1.942377e-014, -1.978646e-015, -4.635043e-015, 1.536286e-014, 2.085685e-014, -3.644998e-015, -2.926883e-015, 1.383253e-014, 1.467478e-014, -4.513414e-015, 3.976242e-015, 1.517630e-014, 2.419220e-014, -3.880067e-015, 4.598034e-015, 1.993050e-014, 5.037350e-014, -9.783373e-016, 5.259952e-015, 2.074471e-014, 4.897916e-014, -2.522407e-015, 5.110816e-015, 1.798101e-014, 2.629524e-014, -8.593510e-018, 2.364113e-015, 9.390935e-015, 1.717813e-014, -1.452888e-015, 9.061463e-015, 1.170364e-014, 3.139263e-014, -4.611558e-015, 7.111125e-015, 1.569663e-014, 3.380669e-014, -7.194665e-015, -1.561850e-015, 1.388645e-014, 2.054165e-014, 2.869281e-016, -5.289423e-015, 5.457762e-015, 9.442709e-015, -1.928577e-015, -1.064826e-014, 8.374252e-015, 1.664568e-014, -5.589350e-015, -1.197939e-014, 1.106807e-014, 1.791183e-014, -3.257522e-015, -4.986235e-015, 7.243515e-015, 9.973792e-015 },
                { 7.480495e-017, 2.479692e-016, -1.622702e-016, -4.204511e-017, -4.609141e-017, 2.225164e-016, 1.309865e-016, 1.727654e-016, 1.162201e-016, 2.034297e-016, 4.795851e-016, 3.448242e-016, -1.584168e-016, 5.510304e-016, 4.628616e-016, 8.018591e-017, 7.605677e-016, -5.566695e-016, -5.583246e-016, 1.833512e-017, 1.049891e-016, 2.297645e-016, -1.415485e-016, 1.381956e-016, 3.038817e-016, 1.377464e-016, 3.594055e-016, 1.546440e-016, -3.217077e-016, -2.856052e-016, 4.053666e-016, -8.593510e-018, 1.900678e-015, -1.812106e-016, 3.971074e-017, 5.110461e-016, 5.129176e-016, 2.511125e-016, -8.295624e-016, -7.648265e-016, -5.188755e-016, -4.409233e-016, -4.846946e-017, -3.742512e-016, -5.337200e-016, -2.628826e-016, 4.287540e-016, -2.435939e-016, 8.303123e-016, -2.964358e-016, -1.072517e-016, 5.058952e-016, 3.897183e-016, -9.148736e-016, 1.384150e-016, 1.032753e-015, -1.478113e-016, -4.844483e-016, 6.539833e-017, 4.701921e-016, -1.864517e-016, -1.128009e-016, 1.664811e-016, 2.681738e-016 },
                { 3.262848e-016, -4.651328e-016, 8.325815e-016, 1.054484e-015, 1.791980e-016, -2.823130e-016, 8.290539e-017, 1.150465e-015, -7.132420e-016, -7.735621e-016, 6.673239e-016, 1.503374e-015, -5.254235e-016, -7.480046e-016, 1.407114e-015, 1.190087e-015, 8.641983e-017, -1.454940e-015, 1.287025e-015, 3.355606e-015, -1.413173e-017, -3.255956e-015, 9.217873e-016, 5.559466e-015, -3.365358e-016, -2.136527e-015, 1.484383e-015, 4.676970e-015, -3.077778e-017, 1.060975e-017, 1.898121e-015, 2.364113e-015, -1.812106e-016, 3.942636e-015, 5.270991e-016, 2.036214e-015, -2.477887e-016, 5.258135e-015, 2.914974e-016, 3.961921e-015, 1.909202e-016, 4.409775e-015, 1.098910e-015, 3.986692e-015, -9.125643e-016, 1.466725e-015, 1.595487e-015, 1.747355e-015, -3.851845e-016, -3.378991e-016, 4.723628e-016, 3.605134e-016, -5.449082e-016, -6.871113e-016, 5.227483e-016, 9.250482e-016, -5.642184e-016, -6.553888e-016, 9.125658e-016, 1.152561e-015, -7.027925e-016, -1.378086e-016, 6.626008e-016, 6.376868e-016 },
                { -8.507642e-017, -1.616162e-016, 5.557424e-015, 5.552071e-015, -9.753728e-016, -2.173702e-015, 6.589070e-015, 7.987140e-015, -5.605055e-016, -2.241054e-015, 6.937242e-015, 8.518227e-015, -1.807938e-015, -1.458405e-015, 5.856996e-015, 5.939745e-015, -2.355700e-015, 8.058062e-016, 9.213091e-015, 1.119818e-014, -2.464324e-015, 6.107939e-016, 1.006050e-014, 2.097102e-014, -3.858399e-016, 8.023102e-016, 9.986729e-015, 1.928048e-014, -1.772276e-015, 6.953998e-016, 7.726329e-015, 9.390935e-015, 3.971074e-017, 5.270991e-016, 8.151779e-015, 1.100630e-014, -6.089630e-016, 3.347891e-015, 8.194756e-015, 1.800064e-014, -3.161027e-015, 2.364584e-015, 9.405397e-015, 1.855568e-014, -3.561016e-015, -7.340066e-016, 7.534749e-015, 1.058402e-014, -2.232473e-017, -2.236529e-015, 5.028603e-015, 5.777760e-015, -1.147903e-015, -4.977184e-015, 6.402202e-015, 9.901771e-015, -2.122813e-015, -4.767464e-015, 6.407958e-015, 9.222305e-015, -1.402705e-015, -1.496717e-015, 4.059993e-015, 4.928873e-015 },
                { 3.147782e-016, -8.228558e-016, 8.720747e-015, 9.820758e-015, -9.878520e-016, -3.460062e-015, 1.094583e-014, 1.424904e-014, -1.843959e-015, -3.468452e-015, 1.266983e-014, 1.496812e-014, -2.827154e-015, -2.312100e-015, 1.040709e-014, 1.047712e-014, -3.223079e-015, 1.544727e-015, 1.338821e-014, 2.107232e-014, -3.072064e-015, 9.502410e-016, 1.653368e-014, 3.913015e-014, -1.878439e-015, 1.317191e-015, 1.791618e-014, 3.564554e-014, -2.850799e-015, 1.347781e-015, 1.395972e-014, 1.717813e-014, 5.110461e-016, 2.036214e-015, 1.100630e-014, 2.098679e-014, -6.462884e-016, 6.830977e-015, 1.296926e-014, 3.349173e-014, -5.391957e-015, 5.631031e-015, 1.612005e-014, 3.272870e-014, -6.874992e-015, -4.704925e-016, 1.333150e-014, 1.817633e-014, -1.701046e-016, -4.223108e-015, 6.377463e-015, 1.021258e-014, -9.718132e-016, -9.092126e-015, 9.211978e-015, 1.685242e-014, -3.859925e-015, -8.717815e-015, 1.049554e-014, 1.583032e-014, -2.989583e-015, -3.011094e-015, 7.322330e-015, 8.813787e-015 },
                { 4.577319e-017, -1.400325e-016, -1.077560e-015, -7.846479e-016, 2.345139e-016, 1.124164e-016, -8.119950e-016, -8.234320e-016, -2.613590e-016, 2.170940e-016, -1.043866e-015, -1.188982e-015, 3.625135e-016, 3.126794e-016, -1.003683e-015, -1.032892e-015, 4.892843e-016, -9.719250e-016, -1.664495e-015, -1.593041e-015, 6.668616e-016, -6.706147e-016, -1.520827e-015, -2.585133e-015, -7.799649e-017, -3.134248e-017, -1.524261e-015, -2.344673e-015, 2.409056e-016, -2.800432e-016, -1.185205e-015, -1.452888e-015, 5.129176e-016, -2.477887e-016, -6.089630e-016, -6.462884e-016, 1.779378e-015, -1.056388e-016, -4.432733e-016, -1.318143e-015, -1.066181e-016, 2.467929e-016, -7.985041e-016, -1.354499e-015, 2.744068e-016, 3.335488e-016, -6.121566e-016, -1.042796e-015, 6.769750e-017, 6.001223e-016, -4.391094e-016, -4.788912e-016, 9.970733e-016, 3.303140e-016, -1.200130e-016, -2.330089e-016, 2.070802e-016, 4.065217e-016, -2.398621e-016, 2.005546e-017, 2.201590e-016, 1.189837e-016, -1.631074e-016, -2.376304e-016 },
                { 6.569669e-016, -7.551743e-016, 3.409525e-015, 3.481214e-015, -5.503452e-016, -1.092804e-015, 3.340536e-015, 5.480319e-015, -1.414035e-015, -1.182212e-015, 4.899545e-015, 6.385343e-015, -1.731638e-015, -1.102268e-015, 5.185674e-015, 4.496522e-015, -1.406771e-015, -1.838851e-015, 5.239069e-015, 9.697292e-015, -1.412356e-015, -5.615130e-015, 6.554053e-015, 1.960354e-014, -5.251117e-016, -3.937308e-015, 7.603355e-015, 1.826102e-014, -2.275508e-016, 3.332615e-016, 6.833963e-015, 9.061463e-015, 2.511125e-016, 5.258135e-015, 3.347891e-015, 6.830977e-015, -1.056388e-016, 1.115908e-014, 4.069921e-015, 1.375635e-014, -1.489486e-015, 9.751659e-015, 6.272767e-015, 1.488278e-014, -2.669935e-015, 2.069992e-015, 5.757369e-015, 7.470431e-015, 1.593415e-016, -1.654809e-015, 2.055256e-015, 2.743429e-015, -3.665028e-016, -4.014721e-015, 3.584734e-015, 5.941800e-015, -2.637819e-015, -4.086172e-015, 4.615446e-015, 6.678920e-015, -1.670093e-015, -1.295210e-015, 2.900756e-015, 3.509588e-015 },
                { -1.805624e-016, -4.531785e-016, 6.327059e-015, 6.564146e-015, -1.131829e-015, -2.160704e-015, 8.328807e-015, 9.518282e-015, -1.287277e-015, -1.963696e-015, 9.056522e-015, 9.821431e-015, -2.250862e-015, -1.587019e-015, 7.173265e-015, 6.827502e-015, -2.970239e-015, 1.356835e-015, 1.010697e-014, 1.350334e-014, -2.928605e-015, 2.311070e-016, 1.323954e-014, 2.619206e-014, -1.170405e-015, 4.282304e-016, 1.329471e-014, 2.439049e-014, -2.447141e-015, 9.317416e-016, 9.998665e-015, 1.170364e-014, -8.295624e-016, 2.914974e-016, 8.194756e-015, 1.296926e-014, -4.432733e-016, 4.069921e-015, 1.219185e-014, 2.456434e-014, -3.984502e-015, 4.104221e-015, 1.328573e-014, 2.488660e-014, -4.786425e-015, -6.010253e-016, 1.012070e-014, 1.328195e-014, -7.260987e-016, -2.757732e-015, 4.909535e-015, 7.038558e-015, -5.887136e-016, -6.278312e-015, 7.857467e-015, 1.232515e-014, -3.085252e-015, -6.463785e-015, 8.634167e-015, 1.257122e-014, -1.843954e-015, -2.064644e-015, 5.859634e-015, 6.527695e-015 },
                { 6.732729e-016, -2.191172e-015, 1.566448e-014, 1.693550e-014, -1.841463e-015, -5.785736e-015, 1.940461e-014, 2.490622e-014, -3.788408e-015, -5.330873e-015, 2.229884e-014, 2.618565e-014, -5.314900e-015, -4.088093e-015, 1.842048e-014, 1.837497e-014, -6.354595e-015, 3.171339e-015, 2.366506e-014, 3.602781e-014, -5.577314e-015, -3.460004e-016, 2.945111e-014, 6.918233e-014, -3.475610e-015, 5.443410e-016, 3.169490e-014, 6.428013e-014, -4.660041e-015, 3.021403e-015, 2.523531e-014, 3.139263e-014, -7.648265e-016, 3.961921e-015, 1.800064e-014, 3.349173e-014, -1.318143e-015, 1.375635e-014, 2.456434e-014, 6.237071e-014, -8.980810e-015, 1.224311e-014, 2.959231e-014, 6.172553e-014, -1.179563e-014, -5.978438e-016, 2.400544e-014, 3.288888e-014, -1.078723e-015, -7.483365e-015, 1.107023e-014, 1.723920e-014, -1.643429e-015, -1.577205e-014, 1.703612e-014, 3.022222e-014, -7.493662e-015, -1.601340e-014, 1.959486e-014, 2.974631e-014, -5.216599e-015, -5.615859e-015, 1.315959e-014, 1.583434e-014 },
                { 6.345911e-017, -3.824775e-017, -2.490264e-015, -2.934109e-015, 3.386465e-016, 8.847407e-016, -3.738840e-015, -4.761227e-015, 4.313093e-016, 7.091352e-016, -4.983253e-015, -4.436112e-015, 6.635708e-016, 5.282090e-016, -3.154923e-015, -2.619974e-015, 1.081001e-015, -1.199158e-015, -3.813867e-015, -5.516231e-015, 8.826338e-016, -1.792144e-015, -4.937737e-015, -9.923741e-015, 1.335788e-015, -7.830649e-016, -6.495721e-015, -9.106157e-015, 9.188391e-016, 6.134835e-016, -4.332444e-015, -4.611558e-015, -5.188755e-016, 1.909202e-016, -3.161027e-015, -5.391957e-015, -1.066181e-016, -1.489486e-015, -3.984502e-015, -8.980810e-015, 4.612456e-015, -4.658749e-016, -6.988816e-015, -9.899587e-015, 2.527029e-015, 1.292971e-015, -4.390840e-015, -5.129482e-015, -1.443935e-016, 1.225836e-015, -1.811945e-015, -2.895870e-015, 3.735816e-016, 3.135960e-015, -3.575972e-015, -5.507640e-015, 2.642056e-015, 2.791119e-015, -4.618840e-015, -5.447381e-015, 6.682084e-016, 3.683172e-016, -2.265577e-015, -2.209749e-015 },
                { 7.119585e-016, -7.597672e-016, 1.957638e-015, 2.178698e-015, -6.450367e-016, -6.044992e-016, 1.787870e-015, 3.224767e-015, -1.975010e-015, -7.977098e-016, 3.415042e-015, 4.460381e-015, -1.419813e-015, -1.201103e-015, 3.659759e-015, 3.522163e-015, -1.100961e-015, -2.563932e-015, 3.264083e-015, 7.000662e-015, -1.613866e-015, -6.999392e-015, 4.434643e-015, 1.474792e-014, -1.513940e-015, -5.175351e-015, 5.394453e-015, 1.439483e-014, 1.085826e-016, 7.662096e-017, 4.715295e-015, 7.111125e-015, -4.409233e-016, 4.409775e-015, 2.364584e-015, 5.631031e-015, 2.467929e-016, 9.751659e-015, 4.104221e-015, 1.224311e-014, -4.658749e-016, 1.164915e-014, 4.726620e-015, 1.216521e-014, -1.553789e-015, 3.951771e-015, 4.223681e-015, 5.377164e-015, -1.116415e-016, -7.472440e-016, 1.602889e-015, 1.692255e-015, 8.027187e-016, -1.940909e-015, 2.713812e-015, 3.883185e-015, -1.620017e-015, -2.646547e-015, 3.659669e-015, 5.548144e-015, -1.438906e-015, -9.082226e-016, 2.803792e-015, 3.148993e-015 },
                { -1.170737e-016, -7.353290e-016, 7.991127e-015, 8.787160e-015, -1.109605e-015, -3.146804e-015, 1.107585e-014, 1.358029e-014, -1.282544e-015, -3.036998e-015, 1.293893e-014, 1.389180e-014, -2.831597e-015, -2.165203e-015, 9.782677e-015, 9.087533e-015, -3.400951e-015, 2.036758e-015, 1.212660e-014, 1.776649e-014, -3.261308e-015, 1.323763e-015, 1.639402e-014, 3.406367e-014, -1.797173e-015, 7.333851e-016, 1.835044e-014, 3.181945e-014, -3.083998e-015, 3.596282e-016, 1.395286e-014, 1.569663e-014, -4.846946e-017, 1.098910e-015, 9.405397e-015, 1.612005e-014, -7.985041e-016, 6.272767e-015, 1.328573e-014, 2.959231e-014, -6.988816e-015, 4.726620e-015, 1.863198e-014, 3.167745e-014, -7.164967e-015, -1.880953e-015, 1.393140e-014, 1.698677e-014, -4.229488e-016, -3.836205e-015, 5.584280e-015, 8.475889e-015, -1.208382e-015, -8.778266e-015, 9.937803e-015, 1.563433e-014, -5.457827e-015, -8.687476e-015, 1.226874e-014, 1.598824e-014, -2.512959e-015, -2.323574e-015, 7.491836e-015, 7.974665e-015 },
                { 2.797540e-016, -1.638812e-015, 1.641106e-014, 1.725443e-014, -1.996067e-015, -5.395085e-015, 2.071037e-014, 2.610336e-014, -3.331571e-015, -5.314905e-015, 2.363962e-014, 2.740826e-014, -5.767468e-015, -4.376738e-015, 1.948125e-014, 1.919905e-014, -6.781461e-015, 3.245409e-015, 2.462851e-014, 3.644931e-014, -6.419347e-015, -1.952106e-016, 3.108902e-014, 7.084443e-014, -3.292045e-015, 2.609884e-016, 3.338375e-014, 6.750844e-014, -4.815425e-015, 3.015941e-015, 2.684530e-014, 3.380669e-014, -3.742512e-016, 3.986692e-015, 1.855568e-014, 3.272870e-014, -1.354499e-015, 1.488278e-014, 2.488660e-014, 6.172553e-014, -9.899587e-015, 1.216521e-014, 3.167745e-014, 6.701699e-014, -1.289247e-014, -1.993293e-015, 2.625553e-014, 3.706635e-014, -1.148929e-015, -7.631725e-015, 1.142171e-014, 1.771328e-014, -2.805019e-015, -1.764972e-014, 1.836614e-014, 3.189656e-014, -8.861209e-015, -1.814395e-014, 2.089282e-014, 3.203171e-014, -5.051126e-015, -6.020865e-015, 1.358143e-014, 1.673780e-014 },
                { -3.052987e-016, 2.538120e-016, -4.744235e-015, -5.305945e-015, 3.356757e-016, 1.817129e-015, -5.840063e-015, -7.783835e-015, 1.155219e-015, 1.414421e-015, -6.802401e-015, -7.850133e-015, 1.057360e-015, 4.815085e-016, -5.750058e-015, -5.091757e-015, 1.138953e-015, -2.075866e-015, -5.526637e-015, -9.252488e-015, 1.172326e-015, -3.029561e-015, -7.711131e-015, -1.840004e-014, 4.100093e-016, -3.150011e-015, -8.579407e-015, -1.599807e-014, 2.325467e-015, -1.039238e-015, -7.357252e-015, -7.194665e-015, -5.337200e-016, -9.125643e-016, -3.561016e-015, -6.874992e-015, 2.744068e-016, -2.669935e-015, -4.786425e-015, -1.179563e-014, 2.527029e-015, -1.553789e-015, -7.164967e-015, -1.289247e-014, 6.491647e-015, 2.217105e-015, -8.405870e-015, -8.080586e-015, -2.809588e-017, 1.999919e-015, -2.148823e-015, -3.617781e-015, 6.423883e-016, 3.871413e-015, -3.860237e-015, -6.337443e-015, 3.190825e-015, 4.579024e-015, -5.626126e-015, -7.156349e-015, 2.802621e-015, 1.481243e-015, -4.642849e-015, -3.951543e-015 },
                { 1.639708e-016, -3.728058e-016, -2.035682e-015, -1.848102e-015, -1.710639e-016, 7.340195e-016, -2.610585e-015, -2.786904e-015, -7.307182e-016, 5.106886e-016, -2.111009e-015, -2.492630e-015, -5.522236e-016, -6.478230e-016, -1.695590e-015, -1.239939e-015, 7.843396e-016, -2.673738e-015, -1.777181e-015, -1.762500e-015, 1.204474e-016, -4.854799e-015, -2.771313e-015, -4.532894e-015, -1.220933e-015, -3.745026e-015, -2.550421e-015, -3.917839e-015, -5.370087e-016, -9.443091e-016, -2.299225e-015, -1.561850e-015, -2.628826e-016, 1.466725e-015, -7.340066e-016, -4.704925e-016, 3.335488e-016, 2.069992e-015, -6.010253e-016, -5.978438e-016, 1.292971e-015, 3.951771e-015, -1.880953e-015, -1.993293e-015, 2.217105e-015, 5.213927e-015, -2.704860e-015, -2.991792e-015, -1.510273e-016, 1.002899e-015, -5.242143e-016, -1.214925e-015, 1.097493e-015, 2.144097e-015, -1.119614e-015, -2.419548e-015, 2.149185e-015, 2.769221e-015, -2.343552e-015, -2.870417e-015, 8.540352e-016, 1.087815e-015, -1.314588e-015, -1.321628e-015 },
                { 3.125247e-016, -7.119985e-016, 7.917270e-015, 8.544086e-015, -7.814211e-016, -3.035762e-015, 9.881382e-015, 1.260078e-014, -1.425942e-015, -2.776884e-015, 1.085278e-014, 1.320356e-014, -2.066438e-015, -1.444282e-015, 9.480677e-015, 8.771987e-015, -2.656082e-015, 2.159504e-015, 1.042083e-014, 1.593524e-014, -2.651556e-015, 2.127188e-015, 1.406893e-014, 3.160487e-014, -6.149063e-016, 2.054857e-015, 1.497435e-014, 2.871639e-014, -2.864144e-015, 1.075008e-015, 1.315247e-014, 1.388645e-014, 4.287540e-016, 1.595487e-015, 7.534749e-015, 1.333150e-014, -6.121566e-016, 5.757369e-015, 1.012070e-014, 2.400544e-014, -4.390840e-015, 4.223681e-015, 1.393140e-014, 2.625553e-014, -8.405870e-015, -2.704860e-015, 1.449581e-014, 1.619115e-014, -8.234520e-017, -3.188663e-015, 4.620126e-015, 7.137322e-015, -1.315713e-015, -7.118085e-015, 7.758513e-015, 1.263882e-014, -5.022019e-015, -8.078870e-015, 1.030326e-014, 1.370895e-014, -3.674816e-015, -2.689605e-015, 7.887283e-015, 7.775594e-015 },
                { -8.126759e-017, -2.621404e-016, 1.047941e-014, 1.066245e-014, -1.359269e-015, -3.204682e-015, 1.291767e-014, 1.640830e-014, -1.689568e-015, -3.027627e-015, 1.394105e-014, 1.684478e-014, -2.499200e-015, -2.011743e-015, 1.216706e-014, 1.182484e-014, -4.348254e-015, 2.354633e-015, 1.476378e-014, 2.096113e-014, -4.144417e-015, 1.820352e-015, 1.879819e-014, 4.247748e-014, -1.008161e-015, 1.848862e-015, 1.950591e-014, 4.001444e-014, -2.245580e-015, 2.190588e-015, 1.640273e-014, 2.054165e-014, -2.435939e-016, 1.747355e-015, 1.058402e-014, 1.817633e-014, -1.042796e-015, 7.470431e-015, 1.328195e-014, 3.288888e-014, -5.129482e-015, 5.377164e-015, 1.698677e-014, 3.706635e-014, -8.080586e-015, -2.991792e-015, 1.619115e-014, 2.450441e-014, -4.013679e-016, -4.023247e-015, 6.426179e-015, 1.020141e-014, -2.548485e-015, -9.783417e-015, 9.804463e-015, 1.761000e-014, -5.844004e-015, -1.118593e-014, 1.181967e-014, 1.822261e-014, -3.395413e-015, -4.200093e-015, 8.210165e-015, 1.024099e-014 },
                { 2.085539e-017, 1.917454e-016, -6.298442e-017, -9.123086e-017, -3.557514e-016, -1.414425e-016, 3.758058e-016, 3.921548e-016, 2.254442e-016, 9.822743e-017, 7.027911e-016, 5.466665e-016, -1.338563e-017, 3.591551e-016, 3.765099e-016, 2.250491e-016, 8.277760e-018, -1.861790e-016, -1.865644e-016, 1.050491e-017, -1.477529e-016, 3.968966e-016, 3.208089e-016, 1.025012e-015, 3.083875e-016, 3.939330e-016, 4.697692e-016, 8.578778e-016, 1.572459e-016, -4.112685e-017, 1.879908e-016, 2.869281e-016, 8.303123e-016, -3.851845e-016, -2.232473e-017, -1.701046e-016, 6.769750e-017, 1.593415e-016, -7.260987e-016, -1.078723e-015, -1.443935e-016, -1.116415e-016, -4.229488e-016, -1.148929e-015, -2.809588e-017, -1.510273e-016, -8.234520e-017, -4.013679e-016, 1.158432e-015, -4.787695e-016, 1.593016e-016, 2.671184e-016, 2.161769e-016, -1.330404e-017, -2.349639e-016, -2.050915e-016, -1.426313e-016, 3.465621e-017, -1.224358e-016, -3.529629e-016, -1.027968e-016, 4.864721e-017, -4.368876e-017, -2.930554e-017 },
                { 1.623780e-016, -1.374888e-016, -3.184643e-015, -3.230437e-015, 6.160673e-016, 1.225106e-015, -4.459188e-015, -5.259668e-015, 7.329589e-017, 7.747388e-016, -4.798515e-015, -5.534194e-015, 5.480694e-016, 3.302071e-016, -3.245852e-015, -3.347019e-015, 1.130824e-015, -1.727986e-015, -3.530971e-015, -4.996767e-015, 9.598220e-016, -3.177408e-015, -5.241248e-015, -1.109122e-014, -1.514190e-016, -2.922935e-015, -5.453610e-015, -1.029736e-014, 4.350302e-016, -1.631570e-015, -4.090338e-015, -5.289423e-015, -2.964358e-016, -3.378991e-016, -2.236529e-015, -4.223108e-015, 6.001223e-016, -1.654809e-015, -2.757732e-015, -7.483365e-015, 1.225836e-015, -7.472440e-016, -3.836205e-015, -7.631725e-015, 1.999919e-015, 1.002899e-015, -3.188663e-015, -4.023247e-015, -4.787695e-016, 3.432614e-015, -1.929769e-015, -4.033363e-015, 1.338009e-016, 4.932931e-015, -2.524852e-015, -6.081882e-015, 1.031136e-015, 4.265542e-015, -2.762801e-015, -5.482534e-015, 9.204521e-016, 1.892846e-015, -1.786305e-015, -2.841512e-015 },
                { 5.622347e-018, -6.755592e-017, 3.499188e-015, 3.442401e-015, -8.074366e-016, -1.693214e-015, 4.310610e-015, 5.168696e-015, -3.975259e-016, -1.626628e-015, 4.616124e-015, 5.707979e-015, -1.113921e-015, -8.295170e-016, 3.664248e-015, 3.867975e-015, -1.432374e-015, 5.845241e-016, 5.400430e-015, 6.103428e-015, -1.832362e-015, 7.214026e-016, 6.104665e-015, 1.203930e-014, -4.459048e-017, 9.603960e-016, 6.100114e-015, 1.115840e-014, -9.344564e-016, 8.075534e-016, 4.507755e-015, 5.457762e-015, -1.072517e-016, 4.723628e-016, 5.028603e-015, 6.377463e-015, -4.391094e-016, 2.055256e-015, 4.909535e-015, 1.107023e-014, -1.811945e-015, 1.602889e-015, 5.584280e-015, 1.142171e-014, -2.148823e-015, -5.242143e-016, 4.620126e-015, 6.426179e-015, 1.593016e-016, -1.929769e-015, 4.075526e-015, 4.125736e-015, -6.368041e-016, -3.197175e-015, 4.349762e-015, 6.491289e-015, -1.189360e-015, -2.910793e-015, 4.048185e-015, 5.912947e-015, -9.255182e-016, -9.672758e-016, 2.590743e-015, 3.073602e-015 },
                { -2.352627e-016, 4.852442e-016, 5.594643e-015, 5.819395e-015, -1.017815e-015, -1.370870e-015, 7.090931e-015, 8.698436e-015, -5.639408e-016, -9.508070e-016, 7.778961e-015, 8.966900e-015, -1.350543e-015, -5.714610e-016, 5.767774e-015, 5.907793e-015, -2.160447e-015, 1.689241e-015, 7.354908e-015, 1.007302e-014, -2.099103e-015, 3.071905e-015, 9.603167e-015, 2.035511e-014, -4.694759e-016, 2.734235e-015, 9.915064e-015, 1.870246e-014, -1.359872e-015, 1.625463e-015, 7.446866e-015, 9.442709e-015, 5.058952e-016, 3.605134e-016, 5.777760e-015, 1.021258e-014, -4.788912e-016, 2.743429e-015, 7.038558e-015, 1.723920e-014, -2.895870e-015, 1.692255e-015, 8.475889e-015, 1.771328e-014, -3.617781e-015, -1.214925e-015, 7.137322e-015, 1.020141e-014, 2.671184e-016, -4.033363e-015, 4.125736e-015, 7.991742e-015, -6.669677e-016, -7.187974e-015, 5.666350e-015, 1.222005e-014, -2.085376e-015, -6.625620e-015, 5.896949e-015, 1.103246e-014, -1.600048e-015, -2.810887e-015, 4.003920e-015, 6.060716e-015 },
                { 2.894019e-016, -2.692793e-016, -1.440227e-015, -1.032417e-015, 9.508320e-017, -3.443036e-017, -1.172588e-015, -1.129702e-015, -4.681945e-016, 2.195044e-016, -9.538378e-016, -1.018944e-015, 2.965751e-016, 2.821740e-016, -9.124762e-016, -8.273823e-016, 6.772822e-016, -6.152364e-016, -2.315977e-015, -2.131535e-015, 5.203181e-016, -8.794632e-016, -1.923670e-015, -3.432711e-015, -2.328841e-016, -3.651354e-016, -1.665947e-015, -3.163871e-015, 1.163092e-016, -1.617381e-016, -1.454407e-015, -1.928577e-015, 3.897183e-016, -5.449082e-016, -1.147903e-015, -9.718132e-016, 9.970733e-016, -3.665028e-016, -5.887136e-016, -1.643429e-015, 3.735816e-016, 8.027187e-016, -1.208382e-015, -2.805019e-015, 6.423883e-016, 1.097493e-015, -1.315713e-015, -2.548485e-015, 2.161769e-016, 1.338009e-016, -6.368041e-016, -6.669677e-016, 1.779364e-015, 7.630931e-016, -7.092504e-016, -1.329854e-015, 6.342130e-016, 1.222436e-015, -8.337972e-016, -1.240879e-015, 2.200533e-016, 3.949533e-016, -2.706603e-016, -6.893790e-016 },
                { 4.566915e-016, -1.864878e-016, -5.922086e-015, -5.845677e-015, 8.753154e-016, 1.695801e-015, -8.117300e-015, -9.226524e-015, 3.059403e-017, 1.202030e-015, -8.451137e-015, -9.424052e-015, 1.333942e-015, 6.438415e-016, -5.974122e-015, -5.725462e-015, 2.161953e-015, -2.584689e-015, -7.349458e-015, -1.033102e-014, 1.754124e-015, -4.583487e-015, -1.034374e-014, -2.147045e-014, -3.410883e-016, -3.822234e-015, -1.088443e-014, -2.058810e-014, 9.816399e-016, -2.284659e-015, -8.421186e-015, -1.064826e-014, -9.148736e-016, -6.871113e-016, -4.977184e-015, -9.092126e-015, 3.303140e-016, -4.014721e-015, -6.278312e-015, -1.577205e-014, 3.135960e-015, -1.940909e-015, -8.778266e-015, -1.764972e-014, 3.871413e-015, 2.144097e-015, -7.118085e-015, -9.783417e-015, -1.330404e-017, 4.932931e-015, -3.197175e-015, -7.187974e-015, 7.630931e-016, 1.040041e-014, -6.033105e-015, -1.329527e-014, 2.560095e-015, 9.408951e-015, -6.346573e-015, -1.254156e-014, 1.547075e-015, 3.884562e-015, -3.734241e-015, -6.220624e-015 },
                { -2.057291e-016, -2.194431e-016, 4.904788e-015, 4.859362e-015, -1.030091e-015, -1.739197e-015, 6.524629e-015, 7.343120e-015, -4.586737e-016, -1.576738e-015, 7.108199e-015, 7.649458e-015, -1.649404e-015, -1.024047e-015, 5.304185e-015, 4.956404e-015, -1.948516e-015, 7.631333e-016, 7.580283e-015, 9.525736e-015, -2.214831e-015, 7.280363e-016, 9.362646e-015, 1.847983e-014, -4.262807e-016, 5.327429e-016, 9.923411e-015, 1.717962e-014, -1.689618e-015, 4.071668e-016, 7.518237e-015, 8.374252e-015, 1.384150e-016, 5.227483e-016, 6.402202e-015, 9.211978e-015, -1.200130e-016, 3.584734e-015, 7.857467e-015, 1.703612e-014, -3.575972e-015, 2.713812e-015, 9.937803e-015, 1.836614e-014, -3.860237e-015, -1.119614e-015, 7.758513e-015, 9.804463e-015, -2.349639e-016, -2.524852e-015, 4.349762e-015, 5.666350e-015, -7.092504e-016, -6.033105e-015, 7.275535e-015, 1.084271e-014, -2.557145e-015, -5.497614e-015, 7.359591e-015, 1.043185e-014, -1.322240e-015, -1.612921e-015, 4.270774e-015, 5.061638e-015 },
                { -5.030543e-016, 2.581185e-016, 9.384437e-015, 9.450302e-015, -1.435232e-015, -2.324399e-015, 1.195381e-014, 1.429777e-014, -8.849318e-016, -1.703428e-015, 1.278091e-014, 1.464488e-014, -2.409481e-015, -1.138182e-015, 9.490865e-015, 9.448197e-015, -3.528496e-015, 2.444445e-015, 1.279406e-014, 1.718625e-014, -3.295873e-015, 4.064776e-015, 1.619008e-014, 3.425657e-014, -6.719495e-016, 3.336137e-015, 1.710305e-014, 3.252816e-014, -2.162564e-015, 2.254364e-015, 1.322442e-014, 1.664568e-014, 1.032753e-015, 9.250482e-016, 9.901771e-015, 1.685242e-014, -2.330089e-016, 5.941800e-015, 1.232515e-014, 3.022222e-014, -5.507640e-015, 3.883185e-015, 1.563433e-014, 3.189656e-014, -6.337443e-015, -2.419548e-015, 1.263882e-014, 1.761000e-014, -2.050915e-016, -6.081882e-015, 6.491289e-015, 1.222005e-014, -1.329854e-015, -1.329527e-014, 1.084271e-014, 2.226226e-014, -4.166686e-015, -1.255896e-014, 1.149719e-014, 2.082277e-014, -2.566254e-015, -4.956021e-015, 7.028750e-015, 1.069791e-014 },
                { -8.290240e-017, 4.618606e-017, -3.001985e-015, -3.100671e-015, 1.551666e-016, 1.130960e-015, -3.772436e-015, -4.992313e-015, 3.319171e-016, 1.139538e-015, -4.624710e-015, -4.867972e-015, 6.238566e-016, 3.868608e-016, -4.184168e-015, -3.278078e-015, 1.248705e-015, -1.609677e-015, -3.630658e-015, -5.520750e-015, 9.715852e-016, -1.907406e-015, -5.160910e-015, -1.167086e-014, 3.625608e-016, -1.433981e-015, -6.055035e-015, -1.103776e-014, 5.469043e-016, -5.207112e-016, -5.244888e-015, -5.589350e-015, -1.478113e-016, -5.642184e-016, -2.122813e-015, -3.859925e-015, 2.070802e-016, -2.637819e-015, -3.085252e-015, -7.493662e-015, 2.642056e-015, -1.620017e-015, -5.457827e-015, -8.861209e-015, 3.190825e-015, 2.149185e-015, -5.022019e-015, -5.844004e-015, -1.426313e-016, 1.031136e-015, -1.189360e-015, -2.085376e-015, 6.342130e-016, 2.560095e-015, -2.557145e-015, -4.166686e-015, 3.721929e-015, 3.839287e-015, -4.795407e-015, -5.658954e-015, 1.275505e-015, 1.321950e-015, -2.611419e-015, -2.744857e-015 },
                { 5.142563e-016, -2.655708e-016, -6.741519e-015, -6.349470e-015, 6.027797e-016, 1.892237e-015, -8.462924e-015, -9.837584e-015, 2.309829e-016, 1.639088e-015, -8.737082e-015, -9.911077e-015, 1.270692e-015, 7.990021e-016, -7.036117e-015, -6.446398e-015, 2.701486e-015, -3.225398e-015, -7.988312e-015, -1.086132e-014, 1.943875e-015, -5.361735e-015, -1.077167e-014, -2.346643e-014, -2.551205e-016, -4.577949e-015, -1.123958e-014, -2.293811e-014, 7.260658e-016, -2.869232e-015, -9.346030e-015, -1.197939e-014, -4.844483e-016, -6.553888e-016, -4.767464e-015, -8.717815e-015, 4.065217e-016, -4.086172e-015, -6.463785e-015, -1.601340e-014, 2.791119e-015, -2.646547e-015, -8.687476e-015, -1.814395e-014, 4.579024e-015, 2.769221e-015, -8.078870e-015, -1.118593e-014, 3.465621e-017, 4.265542e-015, -2.910793e-015, -6.625620e-015, 1.222436e-015, 9.408951e-015, -5.497614e-015, -1.255896e-014, 3.839287e-015, 1.071660e-014, -7.263433e-015, -1.371615e-014, 2.111743e-015, 4.791419e-015, -4.528973e-015, -7.259861e-015 },
                { -1.426200e-016, -5.090547e-016, 6.082218e-015, 6.425675e-015, -6.278798e-016, -2.615187e-015, 8.212623e-015, 9.941604e-015, -7.819261e-016, -2.682543e-015, 8.967558e-015, 1.031633e-014, -1.479745e-015, -1.430887e-015, 7.422734e-015, 6.783740e-015, -2.627791e-015, 1.747401e-015, 8.263134e-015, 1.152556e-014, -2.344040e-015, 2.060978e-015, 1.088007e-014, 2.344367e-014, -7.106469e-016, 1.756654e-015, 1.206602e-014, 2.235547e-014, -1.635858e-015, 5.339528e-016, 9.977997e-015, 1.106807e-014, 6.539833e-017, 9.125658e-016, 6.407958e-015, 1.049554e-014, -2.398621e-016, 4.615446e-015, 8.634167e-015, 1.959486e-014, -4.618840e-015, 3.659669e-015, 1.226874e-014, 2.089282e-014, -5.626126e-015, -2.343552e-015, 1.030326e-014, 1.181967e-014, -1.224358e-016, -2.762801e-015, 4.048185e-015, 5.896949e-015, -8.337972e-016, -6.346573e-015, 7.359591e-015, 1.149719e-014, -4.795407e-015, -7.263433e-015, 1.037323e-014, 1.289256e-014, -2.224034e-015, -2.449138e-015, 6.157743e-015, 6.600829e-015 },
                { -5.490774e-016, 1.417274e-016, 9.811901e-015, 9.895084e-015, -1.043930e-015, -2.759935e-015, 1.244884e-014, 1.511715e-014, -1.262509e-015, -2.366028e-015, 1.318305e-014, 1.538045e-014, -2.253154e-015, -1.341726e-015, 1.041049e-014, 1.006794e-014, -3.995654e-015, 2.972356e-015, 1.264779e-014, 1.745243e-014, -3.405742e-015, 4.410368e-015, 1.646902e-014, 3.615289e-014, -6.779124e-016, 3.837701e-015, 1.749810e-014, 3.495480e-014, -1.896980e-015, 2.676958e-015, 1.423217e-014, 1.791183e-014, 4.701921e-016, 1.152561e-015, 9.222305e-015, 1.583032e-014, 2.005546e-017, 6.678920e-015, 1.257122e-014, 2.974631e-014, -5.447381e-015, 5.548144e-015, 1.598824e-014, 3.203171e-014, -7.156349e-015, -2.870417e-015, 1.370895e-014, 1.822261e-014, -3.529629e-016, -5.482534e-015, 5.912947e-015, 1.103246e-014, -1.240879e-015, -1.254156e-014, 1.043185e-014, 2.082277e-014, -5.658954e-015, -1.371615e-014, 1.289256e-014, 2.235207e-014, -3.185524e-015, -5.744974e-015, 8.143891e-015, 1.182600e-014 },
                { -4.398897e-016, 1.373150e-016, -2.743231e-015, -3.099841e-015, 6.217766e-017, 1.124806e-015, -2.936802e-015, -4.185564e-015, 6.038323e-016, 7.459614e-016, -3.235571e-015, -4.362481e-015, 2.138560e-016, 1.645824e-017, -2.666599e-015, -2.661641e-015, 7.477919e-016, -1.218803e-015, -2.620275e-015, -4.579548e-015, 5.715404e-016, -1.570005e-015, -3.804329e-015, -1.019776e-014, -1.926219e-016, -1.567646e-015, -3.959089e-015, -8.478020e-015, 3.654730e-016, -6.431077e-016, -2.883029e-015, -3.257522e-015, -1.864517e-016, -7.027925e-016, -1.402705e-015, -2.989583e-015, 2.201590e-016, -1.670093e-015, -1.843954e-015, -5.216599e-015, 6.682084e-016, -1.438906e-015, -2.512959e-015, -5.051126e-015, 2.802621e-015, 8.540352e-016, -3.674816e-015, -3.395413e-015, -1.027968e-016, 9.204521e-016, -9.255182e-016, -1.600048e-015, 2.200533e-016, 1.547075e-015, -1.322240e-015, -2.566254e-015, 1.275505e-015, 2.111743e-015, -2.224034e-015, -3.185524e-015, 2.140167e-015, 1.364200e-015, -2.655937e-015, -2.351550e-015 },
                { 1.128013e-016, -1.583129e-016, -3.540824e-015, -3.418837e-015, -1.237308e-017, 1.202098e-015, -3.899713e-015, -5.019759e-015, -3.693910e-017, 9.466835e-016, -3.540275e-015, -5.112001e-015, 2.139728e-016, 1.162304e-016, -3.048426e-015, -3.214094e-015, 1.327797e-015, -2.076810e-015, -3.285103e-015, -4.469756e-015, 7.521520e-016, -3.681980e-015, -4.431427e-015, -1.092889e-014, -6.970103e-016, -3.177154e-015, -4.223915e-015, -1.034268e-014, -2.533739e-016, -1.833488e-015, -3.287211e-015, -4.986235e-015, -1.128009e-016, -1.378086e-016, -1.496717e-015, -3.011094e-015, 1.189837e-016, -1.295210e-015, -2.064644e-015, -5.615859e-015, 3.683172e-016, -9.082226e-016, -2.323574e-015, -6.020865e-015, 1.481243e-015, 1.087815e-015, -2.689605e-015, -4.200093e-015, 4.864721e-017, 1.892846e-015, -9.672758e-016, -2.810887e-015, 3.949533e-016, 3.884562e-015, -1.612921e-015, -4.956021e-015, 1.321950e-015, 4.791419e-015, -2.449138e-015, -5.744974e-015, 1.364200e-015, 3.143230e-015, -2.104233e-015, -3.666932e-015 },
                { 2.832718e-016, -3.924757e-016, 4.641007e-015, 5.213390e-015, -2.977411e-016, -2.014102e-015, 5.796734e-015, 7.523783e-015, -8.831052e-016, -1.699010e-015, 5.915582e-015, 7.946966e-015, -8.517958e-016, -7.027954e-016, 5.270601e-015, 5.174717e-015, -1.610431e-015, 1.464546e-015, 5.404040e-015, 8.448931e-015, -1.540936e-015, 1.724388e-015, 7.750148e-015, 1.794292e-014, -1.645955e-016, 1.608873e-015, 8.048559e-015, 1.614264e-014, -1.299298e-015, 6.082709e-016, 6.673327e-015, 7.243515e-015, 1.664811e-016, 6.626008e-016, 4.059993e-015, 7.322330e-015, -1.631074e-016, 2.900756e-015, 5.859634e-015, 1.315959e-014, -2.265577e-015, 2.803792e-015, 7.491836e-015, 1.358143e-014, -4.642849e-015, -1.314588e-015, 7.887283e-015, 8.210165e-015, -4.368876e-017, -1.786305e-015, 2.590743e-015, 4.003920e-015, -2.706603e-016, -3.734241e-015, 4.270774e-015, 7.028750e-015, -2.611419e-015, -4.528973e-015, 6.157743e-015, 8.143891e-015, -2.655937e-015, -2.104233e-015, 5.582889e-015, 5.270603e-015 },
                { -1.920737e-016, 2.026914e-016, 5.795578e-015, 6.137238e-015, -4.693200e-016, -1.739059e-015, 6.854453e-015, 9.172326e-015, -6.698485e-016, -1.400862e-015, 7.185095e-015, 9.411838e-015, -9.117066e-016, -6.169391e-016, 6.043312e-015, 6.152235e-015, -2.360289e-015, 1.790899e-015, 6.968812e-015, 1.019888e-014, -2.041665e-015, 2.867989e-015, 9.105728e-015, 2.168970e-014, 1.051744e-016, 2.513914e-015, 9.545379e-015, 2.039045e-014, -8.175961e-016, 1.574258e-015, 7.880065e-015, 9.973792e-015, 2.681738e-016, 6.376868e-016, 4.928873e-015, 8.813787e-015, -2.376304e-016, 3.509588e-015, 6.527695e-015, 1.583434e-014, -2.209749e-015, 3.148993e-015, 7.974665e-015, 1.673780e-014, -3.951543e-015, -1.321628e-015, 7.775594e-015, 1.024099e-014, -2.930554e-017, -2.841512e-015, 3.073602e-015, 6.060716e-015, -6.893790e-016, -6.220624e-015, 5.061638e-015, 1.069791e-014, -2.744857e-015, -7.259861e-015, 6.600829e-015, 1.182600e-014, -2.351550e-015, -3.666932e-015, 5.270603e-015, 7.611055e-015 }
            };
        #endregion

    }
}
