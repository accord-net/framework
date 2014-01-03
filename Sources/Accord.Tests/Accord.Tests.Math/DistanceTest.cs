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

using Accord.Math.Decompositions;

namespace Accord.Tests.Math
{
    using Accord.Math;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass()]
    public class DistanceTest
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
        public void MahalanobisTest()
        {
            double[] x = { 1, 0 };
            double[,] y = 
            {
                { 1, 0 },
                { 0, 8 },
                { 0, 5 }
            };

            // Computing the mean of y
            double[] meanY = Statistics.Tools.Mean(y);

            // Computing the covariance matrix of y
            double[,] covY = Statistics.Tools.Covariance(y, meanY);

            // Inverting the covariance matrix
            double[,] precision = covY.Inverse();

            // Run actual test
            double expected = 1.33333;
            double actual = Distance.SquareMahalanobis(x, meanY, precision);

            Assert.AreEqual(expected, actual, 0.0001);
        }


        [TestMethod()]
        public void MahalanobisTest2()
        {
            // Example from Statistical Distance Calculator
            // http://maplepark.com/~drf5n/cgi-bin/dist.cgi

            double[,] cov = 
            {
                { 1.030303, 2.132728, 0.576716 },
                { 2.132728, 4.510515, 1.185771 },
                { 0.576716, 1.185771, 0.398922 }
            };

            double[] x, y;
            double actual, expected;

            x = new double[] { 2, 4, 1 };
            y = new double[] { 0, 0, 0 };

            expected = 2.07735368677415;
            actual = Distance.Mahalanobis(x, y, cov.Inverse());
            Assert.AreEqual(expected, actual, 1e-10);


            x = new double[] { 7, 5, 1 };
            y = new double[] { 1, 0.52, -79 };

            expected = 277.8828871106366;
            actual = Distance.Mahalanobis(x, y, cov.Inverse());
            Assert.AreEqual(expected, actual, 0.0000000000001);
        }

        [TestMethod()]
        public void MahalanobisTest3()
        {
            // Example from Statistical Distance Calculator
            // http://maplepark.com/~drf5n/cgi-bin/dist.cgi

            double[,] cov = 
            {
                { 1.030303, 2.132728, 0.576716 },
                { 2.132728, 4.510515, 1.185771 },
                { 0.576716, 1.185771, 0.398922 }
            };

            

            double[] x, y;
            double actual, expected;

            var svd = new SingularValueDecomposition(cov, true, true, true);

            var inv = cov.Inverse();
            var pinv = svd.Inverse();
            Assert.IsTrue(inv.IsEqual(pinv, 1e-6));

            x = new double[] { 2, 4, 1 };
            y = new double[] { 0, 0, 0 };

            {
                var bla = cov.Solve(x);
                var blo = svd.Solve(x);
                var ble = inv.Multiply(x);
                var bli = pinv.Multiply(x);

                Assert.IsTrue(bla.IsEqual(blo, 1e-6));
                Assert.IsTrue(bla.IsEqual(ble, 1e-6));
                Assert.IsTrue(bla.IsEqual(bli, 1e-6));
            }

            expected = 2.0773536867741504;
            actual = Distance.Mahalanobis(x, y, inv);
            Assert.AreEqual(expected, actual, 1e-6);

            actual = Distance.Mahalanobis(x, y, svd);
            Assert.AreEqual(expected, actual, 1e-6);


            x = new double[] { 7, 5, 1 };
            y = new double[] { 1, 0.52, -79 };

            expected = 277.8828871106366;
            actual = Distance.Mahalanobis(x, y, inv);
            Assert.AreEqual(expected, actual, 1e-5);
            actual = Distance.Mahalanobis(x, y, svd);
            Assert.AreEqual(expected, actual, 1e-5);
        }

        [TestMethod()]
        public void MahalanobisTest4()
        {
            double[] x, y;
            double expected, actual;

            x = new double[] { 2, 4, 1 };
            y = new double[] { 0, 0, 0 };
            expected = Distance.Euclidean(x, y);
            actual = Distance.Mahalanobis(x, y, Matrix.Identity(3));
            Assert.AreEqual(expected, actual);

            x = new double[] { 0.1, 0.12, -1 };
            y = new double[] { 195, 0, 2912 };
            expected = Distance.Euclidean(x, y);
            actual = Distance.Mahalanobis(x, y, Matrix.Identity(3));
            Assert.AreEqual(expected, actual);

            x = new double[] { -2, -4, -1 };
            y = new double[] { -2, -4, -1 };
            expected = Distance.Euclidean(x, y);
            actual = Distance.Mahalanobis(x, y, Matrix.Identity(3));
            Assert.AreEqual(expected, actual);

            x = new double[] { 2, 4, 1 };
            y = new double[] { 0, -7.2, 4.6 };
            expected = Distance.Euclidean(x, y);
            actual = Distance.Mahalanobis(x, y, Matrix.Identity(3));
            Assert.AreEqual(expected, actual);

            x = new double[] { -2, 4, 1 };
            y = new double[] { 0, -0.1, 4.2 };
            expected = Distance.Euclidean(x, y);
            actual = Distance.Mahalanobis(x, y, Matrix.Identity(3));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void MahalanobisTest5()
        {
            double[] x, y;
            double expected, actual;

            x = new double[] { 2, 4, 1 };
            y = new double[] { 0, 0, 0 };
            expected = Distance.Euclidean(x, y);
            actual = Distance.Mahalanobis(x, y, new SingularValueDecomposition(Matrix.Identity(3)));
            Assert.AreEqual(expected, actual);

            x = new double[] { 0.1, 0.12, -1 };
            y = new double[] { 195, 0, 2912 };
            expected = Distance.Euclidean(x, y);
            actual = Distance.Mahalanobis(x, y, new SingularValueDecomposition(Matrix.Identity(3)));
            Assert.AreEqual(expected, actual);

            x = new double[] { -2, -4, -1 };
            y = new double[] { -2, -4, -1 };
            expected = Distance.Euclidean(x, y);
            actual = Distance.Mahalanobis(x, y, new SingularValueDecomposition(Matrix.Identity(3)));
            Assert.AreEqual(expected, actual);

            x = new double[] { 2, 4, 1 };
            y = new double[] { 0, -7.2, 4.6 };
            expected = Distance.Euclidean(x, y);
            actual = Distance.Mahalanobis(x, y, new SingularValueDecomposition(Matrix.Identity(3)));
            Assert.AreEqual(expected, actual);

            x = new double[] { -2, 4, 1 };
            y = new double[] { 0, -0.1, 4.2 };
            expected = Distance.Euclidean(x, y);
            actual = Distance.Mahalanobis(x, y, new SingularValueDecomposition(Matrix.Identity(3)));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void MahalanobisTest6()
        {
            double[] x = { -1, 0, 0 };
            double[] y = {  0, 0, 0 };

            double[,] covX = 
            {
                { 2, 3, 0 },
                { 3, 1, 0 },
                { 0, 0, 0 } 
            };

            var pinv = covX.PseudoInverse();

            // Run actual test
            double expected = 0.14285714285714282;
            double actual = Distance.SquareMahalanobis(x, y, pinv);

            Assert.AreEqual(expected, actual, 1e-6);
            Assert.IsFalse(Double.IsNaN(actual));
        }

        [TestMethod()]
        public void ManhattanTest()
        {
            double[] x = { 3, 6 };
            double[] y = { 0, 0 };
            double expected = 9;
            double actual = Distance.Manhattan(x, y);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod()]
        public void EuclideanTest()
        {
            double[] x = new double[] { 2, 4, 1 };
            double[] y = new double[] { 0, 0, 0 };
            double expected = 4.58257569495584;
            double actual = Distance.Euclidean(x, y);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod()]
        public void ModularTest()
        {
            int a = 1;
            int b = 359;
            int modulo = 360;
            int expected = 2;

            int actual = Distance.Modular(a, b, modulo);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod()]
        public void BhattacharyyaTest()
        {
            double[,] X = 
            {
                { 0.20, 0.52 },
                { 1.52, 2.53 },
                { 7.21, 0.92 },
            };

            double[,] Y = 
            {
                { 9.42, 5.21 },
                { 1.12, 3.14 },
                { 5.21, 2.12 },
            };


            double expected = 0.45095821066601938;
            double actual = Distance.Bhattacharyya(X, Y);
            Assert.AreEqual(expected, actual, 1e-10);
            Assert.IsFalse(Double.IsNaN(actual));
        }

        [TestMethod()]
        public void BhattacharyyaTest1()
        {
            double[] histogram1 = { 0.1, 0.5, 0.4 };
            double[] histogram2 = { 0.7, 0.2, 0.1 };

            double expected = 0.468184902444219;
            double actual = Distance.Bhattacharyya(histogram1, histogram2);

            Assert.AreEqual(expected, actual, 1e-10);
            Assert.IsFalse(Double.IsNaN(actual));
        }

        [TestMethod()]
        public void BhattacharyyaTest2()
        {
            double[] x = { 2, 0, 0 };
            double[] y = { 1, 0, 0 };

            double[,] covX = 
            {
                { 2, 3, 0 },
                { 3, 0, 0 },
                { 0, 0, 0 }
            };

            double[,] covY = 
            {
                { 2, 1, 0 },
                { 1, 0, 0 },
                { 0, 0, 0 }
            };

            // Run actual test
            double expected = 0.1438410362258904;
            double actual = Distance.Bhattacharyya(x, covX, y, covY);

            Assert.AreEqual(expected, actual, 1e-6);
            Assert.IsFalse(Double.IsNaN(actual));
        }

        [TestMethod()]
        public void BhattacharyyaTest3()
        {
            double[] x = { 2, 1, 0 };
            double[] y = { 1, 1, 0 };

            double[,] covX = 
            {
                { 2, 1, 0 },
                { 3, 1, 0 },
                { 0, 0, 0 } 
            };

            // Run actual test
            double expected = 0.125;
            double actual = Distance.Bhattacharyya(x, covX, y, covX);

            Assert.AreEqual(expected, actual, 1e-6);
            Assert.IsFalse(Double.IsNaN(actual));
        }

        [TestMethod()]
        public void BhattacharyyaTest4()
        {
            double[,] X = 
            {
                { 0.20, 0.52 },
                { 1.52, 2.53 },
                { 7.21, 0.92 },
            };


            double expected = 0.0;
            double actual = Distance.Bhattacharyya(X, X);
            Assert.AreEqual(expected, actual, 1e-10);
            Assert.IsFalse(Double.IsNaN(actual));
        }

        [TestMethod()]
        public void EuclideanTest1()
        {
            double x1 = 1.5;
            double y1 = -2.1;

            double x2 = 4;
            double y2 = 1;

            double expected = 4.6861498055439927;
            double actual = Distance.Euclidean(x1, y1, x2, y2);

            Assert.AreEqual(expected, actual, 1e-10);
            Assert.IsFalse(double.IsNaN(actual));
        }
    }
}
