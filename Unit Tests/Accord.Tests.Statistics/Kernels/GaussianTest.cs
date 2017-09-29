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
    using Accord.Statistics;
    using System;
    using Accord.Math.Distances;

    [TestFixture]
    public class GaussianTest
    {


        [Test]
        public void GaussianFunctionTest()
        {
            IKernel gaussian = new Gaussian(1);

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
            var gaussian = new Gaussian(1);

            double[] x = { 1, 1 };
            double[] y = { 1, 1 };

            double actual = gaussian.Distance(x, y);
            double expected = 0;

            Assert.AreEqual(expected, actual);


            gaussian = new Gaussian(11.5);

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
            Gaussian target = new Gaussian(sigma);
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
            Gaussian gaussian = new Gaussian(1);
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
            Gaussian gaussian = new Gaussian(3.6);
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
        public void estimate_gaussian_distances()
        {
            Accord.Math.Random.Generator.Seed = 0;

            double[][] data =
            {
                new double[] { 5.1, 3.5, 1.4, 0.2 },
                new double[] { 5.0, 3.6, 1.4, 0.2 },
                new double[] { 4.9, 3.0, 1.4, 0.2 },
                new double[] { 5.8, 4.0, 1.2, 0.2 },
                new double[] { 4.7, 3.2, 1.3, 0.2 },
            };

            var actual = Gaussian.Distances(data, 2);
            Assert.IsTrue(actual.IsSorted());
            Assert.AreEqual(0.95, actual.Mean(), 1e-5);

            actual = Gaussian.Distances(data, data.Length);
            Assert.IsTrue(actual.IsSorted());
            Assert.AreEqual(0.5296, actual.Mean(), 1e-5);

            Assert.Throws<ArgumentOutOfRangeException>(() => Gaussian.Distances(data, 10));
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
            Gaussian kernel = new Gaussian() { Gamma = 1 };

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
        public void GaussianReverseDistanceTest()
        {
            var gaussian = new Gaussian(4.2);

            var x = new double[] { 0.2, 0.5 };
            var y = new double[] { 0.3, -0.7 };

            double expected = Distance.SquareEuclidean(x, y);

            double df = gaussian.Distance(x, y);
            double actual = gaussian.ReverseDistance(df);

            Assert.AreEqual(expected, actual, 1e-10);
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
            Gaussian kernel = Gaussian.Estimate(data);
            double sigma = kernel.Sigma; // 0.36055512 
            double sigma2 = kernel.SigmaSquared;
            Assert.AreEqual(0.36055512754639879, sigma);
            Assert.AreEqual(sigma * sigma, sigma2);
        }

        [Test]
        public void ExpandDistanceTest()
        {
            for (int i = 1; i <= 3; i++)
            {
                TaylorGaussian kernel = new TaylorGaussian(i);

                kernel.Degree = 64000;

                var x = new double[] { 0.5, 2.0 };
                var y = new double[] { 1.3, -0.2 };

                var phi_x = kernel.Transform(x);
                var phi_y = kernel.Transform(y);

                double d1 = Distance.SquareEuclidean(phi_x, phi_y);
                double d2 = kernel.Distance(x, y);
                double d3 = Accord.Statistics.Tools.Distance(kernel.Gaussian, x, y);

                Assert.AreEqual(d1, d2, 1e-4);
                Assert.AreEqual(d1, d3, 1e-4);
                Assert.IsFalse(double.IsNaN(d1));
                Assert.IsFalse(double.IsNaN(d2));
                Assert.IsFalse(double.IsNaN(d3));
            }
        }

        [Test]
        public void ExpandReverseDistanceTest()
        {
            for (int i = 1; i <= 3; i++)
            {
                TaylorGaussian kernel = new TaylorGaussian(i);

                kernel.Degree = 64000;

                var x = new double[] { 0.5, 2.0 };
                var y = new double[] { 1.3, -0.2 };

                var phi_x = kernel.Transform(x);
                var phi_y = kernel.Transform(y);

                double d = Distance.SquareEuclidean(x, y);
                double phi_d = kernel.ReverseDistance(phi_x, phi_y);

                Assert.AreEqual(phi_d, d, 1e-3);
                Assert.IsFalse(double.IsNaN(phi_d));
                Assert.IsFalse(double.IsNaN(d));
            }
        }

        [Test]
        public void rbf_kernel()
        {
            #region doc_rbf
            // Let's say we created a Gaussian RBF kernel:
            IRadialBasisKernel rbf = new Gaussian(sigma: 3.6);

            // Normally, kernel functions are always defined
            // between two points. For example, if we were to
            // compute the Gaussian kernel between points
            double[] x = { 1, 3 };
            double[] y = { 4, 2 };

            // The most straightforward way would be to use
            double k = rbf.Function(x, y); // should be 0.6799048146023815

            // However, since this kernel is a RBF, it means we 
            // can also compute its kernel function through the
            // Euclidean distance between the two points:
            double z = Distance.Euclidean(x, y);
            double a = rbf.Function(z); // should also be 0.6799048146023815
            #endregion

            Assert.AreEqual(k, a);
        }

        static object[] rbf_source =
        {
            new Gaussian(sigma: 3.6),
            new Accord.Statistics.Kernels.Circular(sigma: 4.2),
            new Gaussian(sigma: 0.7),
            new Log(degree: 2),
            new Multiquadric(),
            new Power(degree: 2),
            new RationalQuadratic(constant: 4.2),
            new Spherical(sigma: 4.2),
            new SquaredSinc(gamma: 4.7),
            new SymmetricTriangle(gamma: 2.1),
            new TStudent(degree: 2),
            new Wave(sigma: 0.36),
        };

        [Test]
        [TestCaseSource("rbf_source")]
        public void rbf_kernel_mass_test(IRadialBasisKernel rbf)
        {
            for (int i = 2; i < 5; i++)
            {
                double[] x = Vector.Random(i);
                double[] y = Vector.Random(i);
                double expected = rbf.Function(x, y);
                double z = Distance.Euclidean(x, y);
                double actual = rbf.Function(z);
                Assert.AreEqual(expected, actual, 1e-8);
            }
        }
    }
}
