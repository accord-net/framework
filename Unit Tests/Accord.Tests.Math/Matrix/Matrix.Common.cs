// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
    using NUnit.Framework;

    public partial class MatrixTest
    {

        #region Reshape
        [Test]
        public void ReshapeTest()
        {
            int[] array = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int rows = 3;
            int cols = 3;

            int[,] expected = 
            {
                { 1, 4, 7 },
                { 2, 5, 8 },
                { 3, 6, 9 },
            };

            int[,] actual = Matrix.Reshape(array, rows, cols);

            Assert.IsTrue(Matrix.IsEqual(expected, actual));

        }

        [Test]
        public void ReshapeTest1()
        {
            double[,] array = 
            {
                { 1, 2, 3},
                { 4, 5, 6},
            };

            int dimension = 1;
            double[] expected = { 1, 2, 3, 4, 5, 6 };
            double[] actual = Matrix.Reshape(array, dimension);
            Assert.IsTrue(Matrix.IsEqual(expected, actual));

            dimension = 0;
            expected = new double[] { 1, 4, 2, 5, 3, 6 };
            actual = Matrix.Reshape(array, dimension);
            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void ReshapeTest2()
        {
            double[][] array = 
            {
                new double[] { 1, 2, 3 },
                new double[] { 4, 5, 6 }
            };

            {
                double[] expected = { 1, 2, 3, 4, 5, 6 };
                double[] actual = Matrix.Reshape(array, 1);
                Assert.IsTrue(expected.IsEqual(actual));
            }

            {
                double[] expected = { 1, 4, 2, 5, 3, 6 };
                double[] actual = Matrix.Reshape(array, 0);
                Assert.IsTrue(expected.IsEqual(actual));
            }
        }

        [Test]
        public void ReshapeTest3()
        {
            double[][] array = 
            {
                new double[] { 1, 2 },
                new double[] { 3, 4, 5, 6 }
            };

            {
                double[] expected = { 1, 2, 3, 4, 5, 6 };
                double[] actual = Matrix.Reshape(array, 1);
                Assert.IsTrue(expected.IsEqual(actual));
            }

            {
                double[] expected = { 1, 3, 2, 4, 5, 6 };
                double[] actual = Matrix.Reshape(array, 0);
                Assert.IsTrue(expected.IsEqual(actual));
            }
        }
        #endregion


        [Test]
        public void RelativelyEqualsTest()
        {
            Assert.IsFalse(double.PositiveInfinity.IsRelativelyEqual(1, 1e-10));
            Assert.IsFalse(1.0.IsRelativelyEqual(double.PositiveInfinity, 1e-10));

            Assert.IsFalse(double.NegativeInfinity.IsRelativelyEqual(1, 1e-10));
            Assert.IsFalse(1.0.IsRelativelyEqual(double.NegativeInfinity, 1e-10));

            Assert.IsFalse(double.PositiveInfinity.IsRelativelyEqual(double.NegativeInfinity, 1e-10));
            Assert.IsFalse(double.NegativeInfinity.IsRelativelyEqual(double.PositiveInfinity, 1e-10));

            Assert.IsTrue(double.PositiveInfinity.IsRelativelyEqual(double.PositiveInfinity, 1e-10));
            Assert.IsTrue(double.NegativeInfinity.IsRelativelyEqual(double.NegativeInfinity, 1e-10));

            Assert.IsTrue(1.0.IsRelativelyEqual(1.1, 0.11));
            Assert.IsTrue(1.1.IsRelativelyEqual(1.0, 0.11));

            Assert.IsFalse(0.0.IsRelativelyEqual(1.1, 0.11));
            Assert.IsFalse(1.1.IsRelativelyEqual(0.0, 0.11));

            Assert.IsFalse(1.0.IsRelativelyEqual(1.2, 0.11));
            Assert.IsFalse(1.2.IsRelativelyEqual(1.0, 0.11));

        }

        [Test]
        public void AddAxisTest()
        {
            int[,] m = Vector.Range(0, 15).Reshape(3, 5);

            var actual = m.Add(5);
            Assert.IsTrue(actual.IsEqual(new int [,]
            {
                { 5,    6,   7,  8,  9 }, 
                { 10,  11,  12, 13, 14 }, 
                { 15,  16,  17, 18, 19 }, 
            }));

            actual = m.Add(new[] { 10, 20, 30 }, dimension: 0);
            Assert.IsTrue(actual.IsEqual(new int[,]
            {
                { 10,   11,  12,  13,  14 }, 
                { 25,   26,  27,  28,  29 }, 
                { 40,   41,  42,  43,  44 }, 
            }));

            actual = m.Add(new[] { 10, 20, 30, 40, 50 }, dimension: 1);
            Assert.IsTrue(actual.IsEqual(new int[,]
            {
                { 10,   21,  32,  43,  54 }, 
                { 15,   26,  37,  48,  59 }, 
                { 20,   31,  42,  53,  64 }, 
            }));
        }
    }
}
