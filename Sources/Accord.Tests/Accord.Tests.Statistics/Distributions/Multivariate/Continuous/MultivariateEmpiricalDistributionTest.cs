﻿// Accord Unit Tests
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
    using Accord.Math;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics;
    using System.Globalization;
    using Accord.Statistics.Distributions.DensityKernels;
    using System;

    [TestClass()]
    public class MultivariateEmpiricalDistributionTest
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
        public void ConstructorTest4()
        {
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

            var target = new MultivariateEmpiricalDistribution(observations);

            double[] weigths = { 0.25, 0.25, 0.25, 0.25 };

            bool thrown = false;

            try
            {
                target.Fit(observations, weigths);
            }
            catch (ArgumentException)
            {
                thrown = true;
            }

            Assert.IsTrue(thrown);
        }

        [TestMethod()]
        public void FitTest2()
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

            var target = new MultivariateEmpiricalDistribution(observations);

            target.Fit(observations);

            Assert.IsTrue(Matrix.IsEqual(mean, target.Mean));
            Assert.IsTrue(Matrix.IsEqual(cov, target.Covariance, 1e-10));
        }

        [TestMethod()]
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

            double[] sampleMean = samples.Mean();
            double[,] sampleCov = samples.Covariance();

            Assert.AreEqual(2, sampleMean[0], 1e-2);
            Assert.AreEqual(6, sampleMean[1], 1e-2);
            Assert.AreEqual(2, sampleCov[0, 0], 1e-2);
            Assert.AreEqual(1, sampleCov[0, 1], 1e-2);
            Assert.AreEqual(1, sampleCov[1, 0], 1e-2);
            Assert.AreEqual(5, sampleCov[1, 1], 2e-2);
        }


        [TestMethod()]
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
    }
}
