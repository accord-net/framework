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

namespace Accord.Tests.Math
{
    using Accord.Math;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public partial class MatrixTest
    {

        #region Reshape
        [TestMethod()]
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

        [TestMethod()]
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

        [TestMethod()]
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

        [TestMethod()]
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

    }
}
