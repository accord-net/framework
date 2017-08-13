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

using System;
using Accord.Statistics;
using NUnit.Framework;

namespace Accord.Tests.Statistics
{
    [TestFixture]
    class QuantilesTest
    {
        private static readonly double[] EVEN_DATA = new double[] { 8.2, 6.2, 10.2, 20.2, 16.2, 15.2, 13.2, 3.2, 7.2, 8.2 };
        private static readonly double[] ODD_DATA = new double[] { 8.2, 6.2, 10.2, 9.2, 20.2, 16.2, 15.2, 13.2, 3.2, 7.2, 8.2 };
        private static readonly double[] TEST_PROBABILITIES = new double[] { 0.0, 0.25, 0.5, 0.75, 1.0 };

        [Test]
        public void Quantile1_Even()
        {
            var expected = new double[] { 3.2, 7.2, 8.2, 15.2, 20.2 };
            var actual = EVEN_DATA.Quantiles(1, TEST_PROBABILITIES);
            ArraysEqualWithThreshold(expected, actual);
        }

        [Test]
        public void Quantile1_Odd()
        {
            var expected = new double[] { 3.2, 7.2, 9.2, 15.2, 20.2 };
            var actual = ODD_DATA.Quantiles(1, TEST_PROBABILITIES);
            ArraysEqualWithThreshold(expected, actual);
        }

        [Test]
        public void Quantile2_Even()
        {
            var expected = new double[] { 3.2, 7.7, 9.2, 15.7, 20.2 };
            var actual = EVEN_DATA.Quantiles(2, TEST_PROBABILITIES);
            ArraysEqualWithThreshold(expected, actual);
        }

        [Test]
        public void Quantile2_Odd()
        {
            var expected = new double[] { 3.2, 7.7, 9.7, 15.7, 20.2 };
            var actual = ODD_DATA.Quantiles(2, TEST_PROBABILITIES);
            ArraysEqualWithThreshold(expected, actual);
        }

        [Test]
        public void Quantile3_Even()
        {
            var expected = new double[] { 3.2, 6.2, 8.2, 15.2, 20.2 };
            var actual = EVEN_DATA.Quantiles(3, TEST_PROBABILITIES);
            ArraysEqualWithThreshold(expected, actual);
        }

        [Test]
        public void Quantile3_Odd()
        {
            var expected = new double[] { 3.2, 7.2, 9.2, 13.2, 20.2 };
            var actual = ODD_DATA.Quantiles(3, TEST_PROBABILITIES);
            ArraysEqualWithThreshold(expected, actual);
        }

        [Test]
        public void Quantile4_Even()
        {
            var expected = new double[] { 3.2, 6.7, 8.2, 14.2, 20.2 };
            var actual = EVEN_DATA.Quantiles(4, TEST_PROBABILITIES);
            ArraysEqualWithThreshold(expected, actual);
        }

        [Test]
        public void Quantile4_Odd()
        {
            var expected = new double[] { 3.2, 6.95, 8.7, 13.7, 20.2 };
            var actual = ODD_DATA.Quantiles(4, TEST_PROBABILITIES);
            ArraysEqualWithThreshold(expected, actual);
        }

        [Test]
        public void Quantile5_Even()
        {
            var expected = new double[] { 3.2, 7.2, 9.2, 15.2, 20.2 };
            var actual = EVEN_DATA.Quantiles(5, TEST_PROBABILITIES);
            ArraysEqualWithThreshold(expected, actual);
        }

        [Test]
        public void Quantile5_Odd()
        {
            var expected = new double[] { 3.2, 7.45, 9.20, 14.7, 20.2 };
            var actual = ODD_DATA.Quantiles(5, TEST_PROBABILITIES);
            ArraysEqualWithThreshold(expected, actual);
        }

        [Test]
        public void Quantile6_Even()
        {
            var expected = new double[] { 3.2, 6.95, 9.2, 15.45, 20.2 };
            var actual = EVEN_DATA.Quantiles(6, TEST_PROBABILITIES);
            ArraysEqualWithThreshold(expected, actual);
        }

        [Test]
        public void Quantile6_Odd()
        {
            var expected = new double[] { 3.2, 7.2, 9.2, 15.2, 20.2 };
            var actual = ODD_DATA.Quantiles(6, TEST_PROBABILITIES);
            ArraysEqualWithThreshold(expected, actual);
        }

        [Test]
        public void Quantile7_Even()
        {
            var expected = new double[] { 3.2, 7.45, 9.2, 14.7, 20.2 };
            var actual = EVEN_DATA.Quantiles(7, TEST_PROBABILITIES);
            ArraysEqualWithThreshold(expected, actual);
        }

        [Test]
        public void Quantile7_Odd()
        {
            var expected = new double[] { 3.2, 7.7, 9.2, 14.2, 20.2 };
            var actual = ODD_DATA.Quantiles(7, TEST_PROBABILITIES);
            ArraysEqualWithThreshold(expected, actual);
        }

        [Test]
        public void Quantile8_Even()
        {
            var expected = new double[] { 3.2, 7.11666667, 9.2, 15.28333333, 20.2 };
            var actual = EVEN_DATA.Quantiles(8, TEST_PROBABILITIES);
            ArraysEqualWithThreshold(expected, actual);
        }

        [Test]
        public void Quantile8_Odd()
        {
            var expected = new double[] { 3.2, 7.36666667, 9.2, 14.86666667, 20.2 };
            var actual = ODD_DATA.Quantiles(8, TEST_PROBABILITIES);
            ArraysEqualWithThreshold(expected, actual);
        }

        [Test]
        public void Quantile9_Even()
        {
            var expected = new double[] { 3.2, 7.1375, 9.2, 15.2625, 20.2 };
            var actual = EVEN_DATA.Quantiles(9, TEST_PROBABILITIES);
            ArraysEqualWithThreshold(expected, actual);
        }

        [Test]
        public void Quantile9_Odd()
        {
            var expected = new double[] { 3.2, 7.3875, 9.2, 14.8250, 20.2 };
            var actual = ODD_DATA.Quantiles(9, TEST_PROBABILITIES);
            ArraysEqualWithThreshold(expected, actual);
        }

        private static void ArraysEqualWithThreshold(double[] expected, double[] actual,
            double differenceThreshold = 1e-8)
        {
            Assert.AreEqual(expected.Length, actual.Length, "Result length mismatch");
            for (int i = 0, n = expected.Length; i < n; i++)
            {
                Assert.AreEqual(expected[i], actual[i], differenceThreshold,
                    $"Value mismatch at index {i}");
            }
        }
    }
}
