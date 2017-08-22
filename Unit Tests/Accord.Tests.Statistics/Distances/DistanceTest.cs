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
    using NUnit.Framework;
    using System;
    using Accord.Math.Decompositions;
    using Accord.Math.Distances;


    [TestFixture]
    public class DistanceTest
    {

        [Test]
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
            double actual = new Bhattacharyya().Distance(X, Y);
            Assert.AreEqual(expected, actual, 1e-10);
            Assert.IsFalse(Double.IsNaN(actual));
        }

        [Test]
        public void BhattacharyyaTest1()
        {
            double[] histogram1 = { 0.1, 0.5, 0.4 };
            double[] histogram2 = { 0.7, 0.2, 0.1 };

            double expected = 0.468184902444219;
            double actual = new Bhattacharyya().Distance(histogram1, histogram2);

            Assert.AreEqual(expected, actual, 1e-10);
            Assert.IsFalse(Double.IsNaN(actual));
        }

        [Test]
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
            double actual = new Bhattacharyya().Distance(x, covX, y, covY);

            Assert.AreEqual(expected, actual, 1e-6);
            Assert.IsFalse(Double.IsNaN(actual));
        }

        [Test]
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
            double actual = new Bhattacharyya().Distance(x, covX, y, covX);

            Assert.AreEqual(expected, actual, 1e-6);
            Assert.IsFalse(Double.IsNaN(actual));
        }

        [Test]
        public void BhattacharyyaTest4()
        {
            double[,] X = 
            {
                { 0.20, 0.52 },
                { 1.52, 2.53 },
                { 7.21, 0.92 },
            };


            double expected = 0.0;
            double actual = new Bhattacharyya().Distance(X, X);
            Assert.AreEqual(expected, actual, 1e-10);
            Assert.IsFalse(Double.IsNaN(actual));
        }

        [Test]
        public void IsMetricTest()
        {
            Assert.IsFalse(Distance.IsMetric(new Bhattacharyya().Distance));

            Assert.IsFalse(Distance.IsMetric<double[]>(new Bhattacharyya()));
        }
    }
}
