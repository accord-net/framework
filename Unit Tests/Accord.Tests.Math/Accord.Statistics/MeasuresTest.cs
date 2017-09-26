// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © 2009-2017 César Souza <cesarsouza at gmail.com>
// and other contributors.
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
    using System;
    using Accord.Math;
    using Accord.Statistics;
    using NUnit.Framework;

    [TestFixture]
    class MeasuresTest
    {
        private const double Tolerance = 1e-10;
        private double[][] series;

        [SetUp]
        public void Setup()
        {
            double[,] data =
            {
                { 1, 3, 5, 6, 4, 2, 7, 8, 9, 2, 3, 4, 5, 6, 7 },
                { 5, 3, 8, 6, 4, 4, 3, 8, 9, 0, 9, 9, 1, 9, 2 },
            };

            this.series = data.Transpose().ToJagged();
        }

        [Test]
        [TestCase(0, 4.8, 16d / 3)]
        [TestCase(0.2, 5.3708532126850983, 5.2012181301523794)]
        [TestCase(0.5, 6.1216162602618489, 4.3349406414990694)]
        [TestCase(1, 7, 2)]
        public void EwmaTest(double alpha, double e1, double e2)
        {
            // Arrange
            double[] expected = { e1, e2 };

            // Act
            double[] ewmas = this.series.ExponentialWeightedMean(alpha);

            // Assert
            Assert.True(ewmas.IsEqual(expected, Tolerance), "EWMA does not agree with expected");
        }

        [Test]
        [TestCase(0, 5.6, 2.28571428571429, 10.0952380952381)]
        [TestCase(0.2, 4.31014623570092, 0.625302929087394, 14.7567276814633)]
        [TestCase(0.5, 2.2476817067779, -2.04530939646465, 18.1801422111128)]
        [TestCase(1, 0, 0, 0)]
        public void UnbiasedEwmCovTest(double alpha, double cov11, double cov12, double cov22)
        {
            // Arrange
            double[,] expected =
            {
                { cov11, cov12 },
                { cov12, cov22 },
            };

            // Act
            double[,] ewmCov = this.series.ExponentialWeightedCovariance(alpha, bias: false);

            // Assert
            Assert.True(ewmCov.IsEqual(expected, Tolerance), "EWM Covariance does not agree with expected");
        }

        [Test]
        [TestCase(0.0, 5.22666666666667, 2.13333333333333, 9.422222222222222)]
        [TestCase(0.2, 3.79631219373019, 0.550757446419597, 12.9975045330648)]
        [TestCase(0.5, 1.49840874058829, -1.36349798444697, 12.1197249201803)]
        [TestCase(1.0, 0, 0, 0)]
        public void BiasedEwmCovTest(double alpha, double cov11, double cov12, double cov22)
        {
            // Arrange
            double[,] expected =
            {
                { cov11, cov12 },
                { cov12, cov22 },
            };

            // Act
            double[,] ewmCov = this.series.ExponentialWeightedCovariance(alpha, bias: true);

            // Assert
            Assert.True(ewmCov.IsEqual(expected, Tolerance), "EWM Covariance does not agree with expected");
        }
    }
}
