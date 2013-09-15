// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2013
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

            double[] x = new double[] { 2, 4, 1 };
            double[] y = new double[] { 0, 0, 0 };

            double expected = 2.07735368677415;
            double actual = Distance.Mahalanobis(x, y, cov.Inverse());

            Assert.AreEqual(expected, actual, 0.0000000000001);
        }

        /// <summary>
        ///  Manhattan Distance test
        /// </summary>
        [TestMethod()]
        public void ManhattanTest()
        {
            double[] x = { 3, 6 };
            double[] y = { 0, 0 };
            double expected = 9;
            double actual = Distance.Manhattan(x, y);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///  Euclidean distance test
        ///</summary>
        [TestMethod()]
        public void EuclideanTest()
        {
            double[] x = new double[] { 2, 4, 1 };
            double[] y = new double[] { 0, 0, 0 };
            double expected = 4.58257569495584;
            double actual = Distance.Euclidean(x, y);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///   Modular distance test
        /// </summary>
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

        /// <summary>
        ///A test for Bhattacharyya
        ///</summary>
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


            double expected = 0.450958210666019;
            double actual = Distance.Bhattacharyya(X, Y);
            Assert.AreEqual(expected, actual, 1e-10);
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
