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
    using Accord.Statistics.Kernels;
    using Accord.Math;
    using NUnit.Framework;
    using System.Diagnostics;

    [TestFixture]
    public class GaussianLinearTest
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


        [Test]
        public void GaussianFunctionTest()
        {
            IKernel gaussian = new Gaussian<Linear>(new Linear(0), 1);

            double[] x = { 1, 1 };
            double[] y = { 1, 1 };

            double actual = gaussian.Function(x, y);

            double expected = 1;

            Assert.AreEqual(expected, actual);


            gaussian = new Gaussian(11.5);

            x = new double[] { 0.2, 5 };
            y = new double[] { 3, 0.7 };

            actual = gaussian.Function(x, y);
            expected = 0.9052480234;

            Assert.AreEqual(expected, actual, 1e-10);
        }

        [Test]
        public void GaussianDistanceTest()
        {
            var gaussian = new Gaussian<Linear>(new Linear(0), 1);

            double[] x = { 1, 1 };
            double[] y = { 1, 1 };

            double actual = gaussian.Distance(x, y);
            double expected = 0;

            Assert.AreEqual(expected, actual);


            gaussian = new Gaussian<Linear>(new Linear(0), 11.5);

            x = new double[] { 0.2, 0.5 };
            y = new double[] { 0.3, -0.7 };

            actual = gaussian.Distance(x, y);
            expected = Accord.Statistics.Tools.Distance(gaussian, x, y);

            Assert.AreEqual(expected, actual, 1e-10);
        }

        [Test]
        public void FunctionTest()
        {
            double sigma = 0.1;
            var target = new Gaussian<Linear>(new Linear(0), sigma);
            double[] x = { 2.0, 3.1, 4.0 };
            double[] y = { 2.0, 3.1, 4.0 };
            double expected = 1;
            double actual;

            actual = target.Function(x, y);
            Assert.AreEqual(expected, actual);

            actual = target.Function(x, x);
            Assert.AreEqual(expected, actual);

            actual = target.Function(y, y);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GammaSigmaTest()
        {
            var gaussian = new Gaussian<Linear>(new Linear(0), 1);
            double expected, actual, gamma, sigma;

            expected = 0.01;
            gaussian.Sigma = expected;
            gamma = gaussian.Gamma;

            gaussian.Gamma = gamma;
            actual = gaussian.Sigma;

            Assert.AreEqual(expected, actual);


            expected = 0.01;
            gaussian.Gamma = expected;
            sigma = gaussian.Sigma;

            gaussian.Sigma = sigma;
            actual = gaussian.Gamma;

            Assert.AreEqual(expected, actual, 1e-12);
        }

        [Test]
        public void GammaSigmaSquaredTest()
        {
            var gaussian = new Gaussian<Linear>(new Linear(0), 3.6);
            Assert.AreEqual(3.6 * 3.6, gaussian.SigmaSquared);
            Assert.AreEqual(3.6, gaussian.Sigma);
            Assert.AreEqual(1.0 / (2 * 3.6 * 3.6), gaussian.Gamma);

            gaussian.SigmaSquared = 81;
            Assert.AreEqual(81, gaussian.SigmaSquared);
            Assert.AreEqual(9, gaussian.Sigma);
            Assert.AreEqual(1.0 / (2 * 81), gaussian.Gamma);

            gaussian.Sigma = 6;
            Assert.AreEqual(36, gaussian.SigmaSquared);
            Assert.AreEqual(6, gaussian.Sigma);
            Assert.AreEqual(1.0 / (2 * 36), gaussian.Gamma);

            gaussian.Gamma = 1.0 / (2 * 49);
            Assert.AreEqual(49, gaussian.SigmaSquared, 1e-10);
            Assert.AreEqual(7, gaussian.Sigma, 1e-10);
            Assert.AreEqual(1.0 / (2 * 49), gaussian.Gamma);
        }

        [Test]
        public void FunctionTest2()
        {
            // Tested against R's kernlab

            double[][] data = 
            {
                new double[] { 5.1, 3.5, 1.4, 0.2 },
                new double[] { 5.0, 3.6, 1.4, 0.2 },
                new double[] { 4.9, 3.0, 1.4, 0.2 },
                new double[] { 5.8, 4.0, 1.2, 0.2 },
                new double[] { 4.7, 3.2, 1.3, 0.2 },
            };

            // rbf <- rbfdot(sigma = 1)

            // R's sigma is framework's Gaussian's gamma:
            var kernel = new Gaussian<Linear>(new Linear(0)) { Gamma = 1 };

            // Compute the kernel matrix
            double[,] actual = new double[5, 5];
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    actual[i, j] = kernel.Function(data[i], data[j]);

            double[,] expected =
            {
                { 1.0000000, 0.9801987, 0.7482636, 0.4584060, 0.7710516 },
                { 0.9801987, 1.0000000, 0.6907343, 0.4317105, 0.7710516 },
                { 0.7482636, 0.6907343, 1.0000000, 0.1572372, 0.9139312 },
                { 0.4584060, 0.4317105, 0.1572372, 1.0000000, 0.1556726 },
                { 0.7710516, 0.7710516, 0.9139312, 0.1556726, 1.0000000 },
            };

            // Assert both are equal
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    Assert.AreEqual(expected[i, j], actual[i, j], 1e-6);
        }


        [Test]
        public void GaussianEstimateTest()
        {
            // Suppose we have the following data 
            // 
            double[][] data =  
            { 
                new double[] { 5.1, 3.5, 1.4, 0.2 }, 
                new double[] { 5.0, 3.6, 1.4, 0.2 }, 
                new double[] { 4.9, 3.0, 1.4, 0.2 }, 
                new double[] { 5.8, 4.0, 1.2, 0.2 }, 
                new double[] { 4.7, 3.2, 1.3, 0.2 }, 
            };

            // Estimate an appropriate sigma from data 
            var kernel = Gaussian.Estimate(new Linear(0), data);
            double sigma = kernel.Sigma; // 0.36055512 
            double sigma2 = kernel.SigmaSquared;
            Assert.AreEqual(0.36055512754639879, sigma);
            Assert.AreEqual(sigma * sigma, sigma2);
        }

        [Test]
        public void FunctionTest_EqualInputs()
        {
            var x = new double[] { 1, 2, 5, 1 };
            var y = new double[] { 1, 2, 5, 1 };

            var target = new Gaussian<Linear>(new Linear(1), 4.2);

            double expected = target.Function(x, y);
            double actual = target.Function(x, x);

            Assert.AreEqual(expected, actual);
        }
    }
}
