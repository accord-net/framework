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
    using System.Collections.Generic;
    using System;
    using System.Data;
    using AForge;

    partial class MatrixTest
    {
        [TestMethod()]
        public void TransposeAndMultiplyByDiagonalTest()
        {
            double[,] a =
            { 
                { 3, 1, 0 },
                { 5, 2, 1 }
            };

            double[] b = { 5, 8 };

            double[,] expected = a.Transpose().Multiply(Matrix.Diagonal(b));
            double[,] actual = a.TransposeAndMultiplyByDiagonal(b);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-10));
        }

        [TestMethod()]
        public void MultiplyTwoMatrices()
        {
            double[,] a = new double[,]
            { 
              { 3.000, 1.000, 0.000 },
              { 5.000, 2.000, 1.000}
            };

            double[,] b = new double[,]
            { 
              { 2.000, 4.000 },
              { 4.000, 6.000 },
              { 1.000, 9.000 }
            };

            double[,] expected = new double[,]
            { 
              { 10.000, 18.000 },
              { 19.000, 41.000 }
            };


            double[,] actual = Matrix.Multiply(a, b);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0001));
        }

        [TestMethod()]
        public void MultiplyTwoMatrices2()
        {
            float[,] a = 
            { 
              { 3.000f, 1.000f, 0.000f },
              { 5.000f, 2.000f, 1.000f}
            };

            float[,] b = 
            { 
              { 2.000f, 4.000f },
              { 4.000f, 6.000f },
              { 1.000f, 9.000f }
            };

            float[,] expected =
            { 
              { 10.000f, 18.000f },
              { 19.000f, 41.000f }
            };

            float[,] actual = Matrix.Multiply(a, b);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0001f));
        }

        [TestMethod()]
        public void MultiplyTwoMatrices3()
        {
            double[][] a = new double[,]
            { 
              { 3.000, 1.000, 0.000 },
              { 5.000, 2.000, 1.000 }
            }.ToArray();

            double[][] b = new double[,]
            { 
              { 2.000, 4.000 },
              { 4.000, 6.000 },
              { 1.000, 9.000 }
            }.ToArray();

            double[][] expected = new double[,]
            { 
              { 10.000, 18.000 },
              { 19.000, 41.000 }
            }.ToArray();

            double[][] actual = Matrix.Multiply(a, b);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0001));
        }

        [TestMethod()]
        public void MultiplyVectorMatrixTest()
        {
            double[] a = { 1.000, 2.000, 3.000 };
            double[,] b = 
            { 
                { 2.000, 1.000, 5.000, 2.000 },
                { 2.000, 1.000, 2.000, 2.000 },
                { 1.000, 1.000, 1.000, 1.000 },
             };
            double[] expected = { 9.000, 6.000, 12.000, 9.000 };

            double[] actual = Matrix.Multiply(a, b);
            Assert.IsTrue(actual.IsEqual(expected));

        }

        [TestMethod()]
        public void MultiplyMatrixVectorTest()
        {
            double[,] a = 
            {
                { 4, 5, 1 },
                { 5, 5, 5 },
            };

            double[] b = new double[] { 2, 3, 1 };
            double[] expected = new double[] { 24, 30 };
            double[] actual;
            actual = Matrix.Multiply(a, b);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0000001));

        }

        [TestMethod()]
        public void MultiplyByDiagonalTest()
        {
            double[,] A = 
            {
                { 4, 1, 2 },
                { 5, 6, 5 },
            };

            double[,] B = 
            {
                { 1, 0, 0 },
                { 0, 2, 0 },
                { 0, 0, 3 },
            };

            double[] b = B.Diagonal();


            double[,] expected = A.Multiply(B);
            double[,] actual = A.MultiplyByDiagonal(b);


            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod]
        public void MultiplyByTransposeTest()
        {
            double[,] a = Matrix.Magic(5);

            double[,] expected = a.Multiply(a.Transpose());
            double[,] actual = Matrix.MultiplyByTranspose(a, a);

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod]
        public void MultiplyByTransposeTest2()
        {
            double[,] a = Matrix.Random(2, 3);
            double[,] b = Matrix.Random(4, 3);


            double[,] expected = a.Multiply(b.Transpose());
            double[,] actual = Matrix.MultiplyByTranspose(a, b);

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void TransposeAndMultiplyTest()
        {
            double[,] a = Matrix.Random(2, 3);
            double[,] b = Matrix.Random(2, 4);
            double[,] actual = new double[3, 4];

            double[,] expected = a.Transpose().Multiply(b);
            Matrix.TransposeAndMultiply(a, b, actual);

            Assert.IsTrue(expected.IsEqual(actual));
        }

    }
}
