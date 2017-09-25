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
        public void EwmaTest(double alpha, double e1, double e2)
        {
            // Arrange
            double[] expected = { e1, e2 };

            // Act
            double[] ewmas = this.series.ExponentialWeightedMean(alpha);

            // Assert
            Assert.True(ewmas.IsEqual(expected, Tolerance), "EWMA does not agree with expected");


            /*
            double[] x = X.Transpose()[0];

            decay = 0.2;

            new Ewma().Compute(X, x.Rows(), 1 - decay);

            double ema = x[0];
            double emv = 0;

            for (int i = 1; i < x.Length; i++)
            {
                double delta = x[i] - ema;
                ema = ema + decay * delta;
                emv = (1 - decay) * (emv + decay * delta * delta);
            }
            */
        }
    }
}
