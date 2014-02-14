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
    using Accord.Statistics.Distributions.Multivariate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Statistics.Distributions;
    using Accord.Math;
    using System;
    using Accord.Statistics.Distributions.Fitting;


    [TestClass()]
    public class MultivariateContinuousDistributionTest
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



        internal virtual MultivariateContinuousDistribution CreateMultivariateContinuousDistribution()
        {
            double[] mean = { 0.2, 4.2 };
            double[,] cov = 
            {
                { 1.2, 0.1 },
                { 0.1, 2.3 },
            };

            return new MultivariateNormalDistribution(mean, cov);
        }

        [TestMethod()]
        public void VarianceTest()
        {
            MultivariateContinuousDistribution target = CreateMultivariateContinuousDistribution();
            double[] actual = target.Variance;
            double[] expected = { 1.2, 2.3 };
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void MeanTest()
        {
            MultivariateContinuousDistribution target = CreateMultivariateContinuousDistribution();
            double[] actual = target.Mean;
            double[] expected = { 0.2, 4.2 };
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void DimensionTest()
        {
            MultivariateContinuousDistribution target = CreateMultivariateContinuousDistribution();
            int actual = target.Dimension;
            int expected = 2;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CovarianceTest()
        {
            MultivariateContinuousDistribution target = CreateMultivariateContinuousDistribution();
            double[,] actual = target.Covariance;
            double[,] expected =
            {
                { 1.2, 0.1 },
                { 0.1, 2.3 },
            };
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void ProbabilityDensityFunctionTest()
        {
            MultivariateContinuousDistribution target = CreateMultivariateContinuousDistribution();
            double[] x = { 0.4, 2 };
            double expected = 0.032309150392899;
            double actual = target.ProbabilityDensityFunction(x);
            Assert.AreEqual(expected, actual, 1e-10);
        }

        [TestMethod()]
        public void LogProbabilityDensityFunctionTest()
        {
            MultivariateContinuousDistribution target = CreateMultivariateContinuousDistribution();
            double[] x = { 0.4, 2 };
            double expected = System.Math.Log(0.032309150392899);
            double actual = target.LogProbabilityDensityFunction(x);
            Assert.AreEqual(expected, actual, 1e-10);
        }

        [TestMethod()]
        public void FitTest7()
        {
            MultivariateContinuousDistribution target = CreateMultivariateContinuousDistribution();

            double[][] observations = 
            {
                new double[] { 1, 2 },
                new double[] { 3, 2 },
                new double[] { 2, 3 },
                new double[] { 1, 2 },
            };

            double[] weights = { 0.125, 0.125, 0.25, 0.5 };

            target.Fit(observations, weights);

            double[] expectedMean = { 1.5, 2.25 };
            double[,] expectedCov = 
            {
                { 0.76190476190476186, 0.19047619047619047 },
                { 0.19047619047619047, 0.2857142857142857 },
            };

            Assert.IsTrue(expectedMean.IsEqual(target.Mean));
            Assert.IsTrue(expectedCov.IsEqual(target.Covariance));


            Assert.IsTrue(Matrix.Diagonal(expectedCov).IsEqual(target.Variance));

            double[] x = { 0.4, 2 };
            double expected = 0.168681501947055;
            double actual = target.ProbabilityDensityFunction(x);
            Assert.AreEqual(expected, actual, 1e-10);
        }

        [TestMethod()]
        public void FitTest6()
        {
            MultivariateContinuousDistribution target = CreateMultivariateContinuousDistribution(); // TODO: Initialize to an appropriate value
            double[][] observations = 
            {
                new double[] { 1, 2 },
                new double[] { 3, 2 },
                new double[] { 2, 3 },
                new double[] { 1, 2 },
            };

            target.Fit(observations);

            double[] expectedMean = { 1.75, 2.25 };
            double[,] expectedCov = 
            {
                { 0.91666666666666663,  0.083333333333333329 },
                { 0.083333333333333329, 0.25                 },
            };

            Assert.IsTrue(expectedMean.IsEqual(target.Mean));
            Assert.IsTrue(expectedCov.IsEqual(target.Covariance));

            Assert.IsTrue(expectedCov.Diagonal().IsEqual(target.Variance));

            double[] x = { 0.4, 2 };
            double expected = 0.120833904312578;
            double actual = target.ProbabilityDensityFunction(x);
            Assert.AreEqual(expected, actual, 1e-10);
        }

        [TestMethod()]
        public void FitTest5()
        {
            MultivariateContinuousDistribution target = CreateMultivariateContinuousDistribution(); // TODO: Initialize to an appropriate value
            double[][] observations = 
            {
                new double[] { 1, 1 },
                new double[] { 1, 1 },
            };

            double[] weights = { 0.50, 0.50 };

            bool thrown;

            IFittingOptions options = new NormalOptions()
            {
                Regularization = 0.1
            };

            thrown = false;
            try
            {
                target.Fit(observations, weights);
            }
            catch
            {
                thrown = true;
            }

            Assert.AreEqual(true, thrown);


            thrown = false;
            try
            {
                target.Fit(observations, weights, options);
            }
            catch
            {
                thrown = true;
            }

            Assert.AreEqual(false, thrown);
        }

        [TestMethod()]
        public void FitTest4()
        {
            MultivariateContinuousDistribution target = CreateMultivariateContinuousDistribution(); // TODO: Initialize to an appropriate value
            double[][] observations = 
            {
                new double[] { 1, 1 },
                new double[] { 1, 1 },
            };

            bool thrown;

            IFittingOptions options = new NormalOptions()
            {
                Regularization = 0.1
            };

            thrown = false;
            try
            {
                target.Fit(observations);
            }
            catch
            {
                thrown = true;
            }

            Assert.AreEqual(true, thrown);


            thrown = false;
            try
            {
                target.Fit(observations, options);
            }
            catch
            {
                thrown = true;
            }

            Assert.AreEqual(false, thrown);
        }

        [TestMethod()]
        [DeploymentItem("Accord.Statistics.dll")]
        public void ProbabilityFunctionTest()
        {
            IDistribution target = CreateMultivariateContinuousDistribution();
            double[] x = { 0.4, 2 };
            double expected = 0.032309150392899;
            double actual = target.ProbabilityFunction(x);
            Assert.AreEqual(expected, actual, 1e-10);
        }


        [TestMethod()]
        [DeploymentItem("Accord.Statistics.dll")]
        public void LogProbabilityFunctionTest()
        {
            IDistribution target = CreateMultivariateContinuousDistribution();
            double[] x = { 0.4, 2 };
            double expected = System.Math.Log(0.032309150392899);
            double actual = target.LogProbabilityFunction(x);
            Assert.AreEqual(expected, actual, 1e-10);
        }

        [TestMethod()]
        [DeploymentItem("Accord.Statistics.dll")]
        public void FitTest3()
        {
            IDistribution target = CreateMultivariateContinuousDistribution();

            double[][] observations = 
            {
                new double[] { 1, 2 },
                new double[] { 3, 2 },
                new double[] { 2, 3 },
                new double[] { 1, 2 },
            };

            target.Fit(observations);

            double[] expectedMean = Accord.Statistics.Tools.Mean(observations);
            double[,] expectedCov = Accord.Statistics.Tools.Covariance(observations, expectedMean);

            MultivariateContinuousDistribution actual = target as MultivariateContinuousDistribution;

            Assert.IsTrue(expectedMean.IsEqual(actual.Mean));
            Assert.IsTrue(expectedCov.IsEqual(actual.Covariance));
        }

        /// <summary>
        ///A test for Accord.Statistics.Distributions.IDistribution.Fit
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Accord.Statistics.dll")]
        public void FitTest2()
        {
            IDistribution target = CreateMultivariateContinuousDistribution();

            double[][] observations = 
            {
                new double[] { 1, 2 },
                new double[] { 3, 2 },
                new double[] { 2, 3 },
                new double[] { 1, 2 },
            };

            target.Fit(observations);

            double[] expectedMean = Accord.Statistics.Tools.Mean(observations);
            double[,] expectedCov = Accord.Statistics.Tools.Covariance(observations, expectedMean);

            MultivariateContinuousDistribution actual = target as MultivariateContinuousDistribution;

            Assert.IsTrue(expectedMean.IsEqual(actual.Mean));
            Assert.IsTrue(expectedCov.IsEqual(actual.Covariance));
        }

        /// <summary>
        ///A test for Accord.Statistics.Distributions.IDistribution.Fit
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Accord.Statistics.dll")]
        public void FitTest1()
        {
            IDistribution target = CreateMultivariateContinuousDistribution(); // TODO: Initialize to an appropriate value
           
            Array observations = new double[][]
            {
                new double[] { 1, 1 },
                new double[] { 1, 1 },
            };

            bool thrown;

            IFittingOptions options = new NormalOptions()
            {
                Regularization = 0.1
            };

            thrown = false;
            try
            {
                target.Fit(observations);
            }
            catch
            {
                thrown = true;
            }

            Assert.AreEqual(true, thrown);


            thrown = false;
            try
            {
                target.Fit(observations, options);
            }
            catch
            {
                thrown = true;
            }

            Assert.AreEqual(false, thrown);
        }



        /// <summary>
        ///A test for Accord.Statistics.Distributions.IDistribution.Fit
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Accord.Statistics.dll")]
        public void FitTest()
        {
            IDistribution target = CreateMultivariateContinuousDistribution(); 
            
            double[][] observations = 
            {
                new double[] { 1, 1 },
                new double[] { 1, 1 },
            };

            double[] weights = { 0.50, 0.50 };

            bool thrown;

            IFittingOptions options = new NormalOptions()
            {
                Regularization = 0.1
            };

            thrown = false;
            try
            {
                target.Fit(observations, weights);
            }
            catch
            {
                thrown = true;
            }

            Assert.AreEqual(true, thrown);


            thrown = false;
            try
            {
                target.Fit(observations, weights, options);
            }
            catch
            {
                thrown = true;
            }

            Assert.AreEqual(false, thrown);
        }



    }
}
