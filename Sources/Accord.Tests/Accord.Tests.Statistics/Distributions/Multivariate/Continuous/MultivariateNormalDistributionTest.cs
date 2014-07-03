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
    using Accord.Statistics;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Univariate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class MultivariateNormalDistributionTest
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
        public void ConstructorTest1()
        {

            NormalDistribution normal = new NormalDistribution(4.2, 1.2);
            MultivariateNormalDistribution target = new MultivariateNormalDistribution(new[] { 4.2 }, new[,] { { 1.2 * 1.2 } });

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

            double epdf1 = target.ProbabilityDensityFunction(new double[] { 2 });
            double epdf2 = target.ProbabilityDensityFunction(new double[] { 4 });
            double epdf3 = target.ProbabilityDensityFunction(new double[] { 3 });
            double elpdf = target.LogProbabilityDensityFunction(new double[] { 3 });
            double ecdf = target.DistributionFunction(new double[] { 3 });
            double eccdf = target.ComplementaryDistributionFunction(new double[] { 3 });


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

        [TestMethod()]
        public void ConstructorTest4()
        {
            // Create a multivariate Gaussian distribution 
            var dist = new MultivariateNormalDistribution(

                // mean vector mu
                mean: new double[] 
                {
                    4, 2 
                },

                // covariance matrix sigma
                covariance: new double[,] 
                {
                    { 0.3, 0.1 },
                    { 0.1, 0.7 }
                }
            );

            // Common measures
            double[] mean = dist.Mean;     // { 4, 2 }
            double[] median = dist.Median; // { 4, 2 }
            double[] var = dist.Variance;  // { 0.3, 0.7 } (diagonal from cov)
            double[,] cov = dist.Covariance; // { { 0.3, 0.1 }, { 0.1, 0.7 } }

            // Probability mass functions
            double pdf1 = dist.ProbabilityDensityFunction(new double[] { 2, 5 });
            double pdf2 = dist.ProbabilityDensityFunction(new double[] { 4, 2 });
            double pdf3 = dist.ProbabilityDensityFunction(new double[] { 3, 7 });
            double lpdf = dist.LogProbabilityDensityFunction(new double[] { 3, 7 });

            // Cumulative distribution function (for up to two dimensions)

            // compared against R package mnormt: install.packages("mnormt")
            // pmnorm(c(3,5), mean=c(4,2), varcov=matrix(c(0.3,0.1,0.1,0.7), 2,2))
            double cdf = dist.DistributionFunction(new double[] { 3, 5 });
            double ccdf = dist.ComplementaryDistributionFunction(new double[] { 3, 5 });


            Assert.AreEqual(4, mean[0]);
            Assert.AreEqual(2, mean[1]);
            Assert.AreEqual(4, median[0]);
            Assert.AreEqual(2, median[1]);
            Assert.AreEqual(0.3, var[0]);
            Assert.AreEqual(0.7, var[1]);
            Assert.AreEqual(0.3, cov[0, 0]);
            Assert.AreEqual(0.1, cov[0, 1]);
            Assert.AreEqual(0.1, cov[1, 0]);
            Assert.AreEqual(0.7, cov[1, 1]);
            Assert.AreEqual(0.000000018917884164743237, pdf1);
            Assert.AreEqual(0.35588127170858852, pdf2);
            Assert.AreEqual(0.000000000036520107734505265, pdf3);
            Assert.AreEqual(-24.033158110192296, lpdf);
            Assert.AreEqual(0.033944035782101548, cdf);
        }

        [TestMethod()]
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

        [TestMethod()]
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

        [TestMethod()]
        public void ProbabilityDensityFunctionTest2()
        {
            double[] mean = new double[64];
            double[,] covariance = Accord.Tests.Math.CholeskyDecompositionTest.bigmatrix;

            var target = new MultivariateNormalDistribution(mean, covariance);

            double expected = 1.0;
            double actual = target.ProbabilityDensityFunction(mean);

            Assert.AreEqual(expected, actual, 0.00000001);

            double[] x = Matrix.Diagonal(covariance).Multiply(1.5945e7);

            expected = 4.781042576287362e-12;
            actual = target.ProbabilityDensityFunction(x);

            Assert.AreEqual(expected, actual, 1e-21);
        }

        [TestMethod()]
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

        [TestMethod()]
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

        [TestMethod()]
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

        [TestMethod()]
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

        [TestMethod()]
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

        [TestMethod()]
        public void ConstructorTest2()
        {
            double[] mean = { 1, -1 };
            double[,] covariance = Matrix.Identity(4);

            bool thrown = false;

            try { new MultivariateNormalDistribution(mean, covariance); }
            catch (DimensionMismatchException) { thrown = true; }

            Assert.IsTrue(thrown);
        }

        [TestMethod()]
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


        [TestMethod()]
        public void FitTest()
        {
            double[][] observations = 
            {
                new double[] { 0.1000, -0.2000 },
                new double[] { 0.4000,  0.6000 },
                new double[] { 2.0000,  0.2000 },
                new double[] { 2.0000,  0.3000 }
            };

            double[] mean = Accord.Statistics.Tools.Mean(observations);
            double[,] cov = Accord.Statistics.Tools.Covariance(observations);

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

        [TestMethod()]
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

        [TestMethod()]
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

        [TestMethod()]
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

        [TestMethod()]
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

        [TestMethod()]
        public void GenerateTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            var normal = new MultivariateNormalDistribution(
                new double[] { 2, 6 },
                new double[,] { { 2, 1 }, { 1, 5 } });

            double[][] sample = normal.Generate(1000000);

            double[] mean = sample.Mean();
            double[,] cov = sample.Covariance();

            Assert.AreEqual(2, mean[0], 1e-2);
            Assert.AreEqual(6, mean[1], 1e-2);

            Assert.AreEqual(2, cov[0, 0], 1e-2);
            Assert.AreEqual(1, cov[0, 1], 1e-2);
            Assert.AreEqual(1, cov[1, 0], 1e-2);
            Assert.AreEqual(5, cov[1, 1], 2e-2);
        }

        [TestMethod()]
        public void GenerateTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            var normal = new MultivariateNormalDistribution(
                new double[] { 2, 6 },
                new double[,] { { 2, 1 }, { 1, 5 } });

            double[][] sample = new double[1000000][];
            for (int i = 0; i < sample.Length; i++)
                sample[i] = normal.Generate();

            double[] mean = sample.Mean();
            double[,] cov = sample.Covariance();

            Assert.AreEqual(2, mean[0], 1e-2);
            Assert.AreEqual(6, mean[1], 1e-2);

            Assert.AreEqual(2, cov[0, 0], 1e-2);
            Assert.AreEqual(1, cov[0, 1], 1e-2);
            Assert.AreEqual(1, cov[1, 0], 1e-2);
            Assert.AreEqual(5, cov[1, 1], 2e-2);
        }

    }
}
