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

namespace Accord.Tests.Math
{
    using Accord.Math;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System;
    using System.Data;
    using AForge;

    partial class MatrixTest
    {
        [Test]
        public void TransposeAndMultiplyByDiagonalTest()
        {
            double[,] a =
            {
                { 3, 1, 0 },
                { 5, 2, 1 }
            };

            double[] b = { 5, 8 };

            double[,] expected = Matrix.Multiply(a.Transpose(), Matrix.Diagonal(b));
            double[,] actual = a.TransposeAndMultiplyByDiagonal(b);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-10));
        }

        [Test]
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

        [Test]
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

        [Test]
        public void MultiplyTwoMatrices3()
        {
            double[][] a = new double[,]
            {
              { 3.000, 1.000, 0.000 },
              { 5.000, 2.000, 1.000 }
            }.ToJagged();

            double[][] b = new double[,]
            {
              { 2.000, 4.000 },
              { 4.000, 6.000 },
              { 1.000, 9.000 }
            }.ToJagged();

            double[][] expected = new double[,]
            {
              { 10.000, 18.000 },
              { 19.000, 41.000 }
            }.ToJagged();

            double[][] actual = Matrix.Multiply(a, b);
            double[][] actual2 = a.ToMatrix().Dot(b);

            Assert.IsTrue(expected.IsEqual(actual, 0.0001));
            Assert.IsTrue(expected.IsEqual(actual2, 0.0001));
        }

        [Test]
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

        [Test]
        public void MultiplyVectorMatrixBatchTest()
        {
            const double Tolerance = 1e-10;

            for (int n = 1; n < 10; n++)
            {
                for (int m = 1; m < 10; m++)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        double[] a = Vector.Random(n);
                        double[,] B = Matrix.Random(n, m);
                        double[] c = Vector.Random(m);

                        double[] result1 = a.Dot(B, c);
                        double[] result2 = new double[B.Columns()];

                        for (int k = 0; k < B.Columns(); k++)
                        {
                            result2[k] = a.Dot(B.GetColumn(k));
                        }

                        Assert.IsTrue(result1.IsEqual(result2, Tolerance));
                    }
                }
            }
        }

        [Test]
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

        [Test]
        public void MultiplyMatrixVectorBatchTest()
        {
            const double Tolerance = 1e-10;

            for (int n = 1; n < 10; n++)
            {
                for (int m = 1; m < 10; m++)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        double[,] A = Matrix.Random(n, m);
                        double[] b = Vector.Random(m);
                        double[] c = Vector.Random(n);

                        double[] result1 = A.Dot(b, c);
                        double[] result2 = new double[A.Rows()];

                        for (int k = 0; k < A.Rows(); k++)
                        {
                            result2[k] = A.GetRow(k).Dot(b);
                        }

                        Assert.IsTrue(result1.IsEqual(result2, Tolerance));
                    }
                }
            }
        }

        [Test]
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


            double[,] expected = Matrix.Multiply(A, B);
            double[,] actual = A.MultiplyByDiagonal(b);


            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void MultiplyByTransposeTest()
        {
            double[,] a = Matrix.Magic(5);

            double[,] expected = Matrix.Multiply(a, a.Transpose());
            double[,] actual = Matrix.MultiplyByTranspose(a, a);

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void MultiplyByTransposeTest2()
        {
            double[,] a = Matrix.Random(2, 3);
            double[,] b = Matrix.Random(4, 3);


            double[,] expected = Matrix.Multiply(a, b.Transpose());
            double[,] actual = Matrix.MultiplyByTranspose(a, b);

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void TransposeAndMultiplyTest()
        {
            double Tolerance = 1e-12;

            double[,] a = Matrix.Random(2, 3);
            double[,] b = Matrix.Random(2, 4);
            double[,] actual = new double[3, 4];

            double[,] expected = Matrix.Multiply(a.Transpose(), b);
            a.TransposeAndMultiply(b, actual);

            Assert.IsTrue(expected.IsEqual(actual, Tolerance));
        }

        [Test]
        public void TransposeAndDotBatchTest()
        {
            double Tolerance = 1e-12;

            for (int i = 1; i < 10; i++)
                for (int j = 1; j < 10; j++)
                    for (int k = 1; k < 10; k++)
                    {
                        double[,] a = Matrix.Random(j, i);
                        double[,] b = Matrix.Random(j, k);
                        double[,] actual = Matrix.Random(i, k);

                        double[,] expected = a.Transpose().Dot(b);
                        a.TransposeAndDot(b, actual);

                        Assert.IsTrue(expected.IsEqual(actual, Tolerance));
                    }
        }

        [Test]
        public void DotWithTransposeTest()
        {
            double[,] a = Matrix.Random(5, 3);
            double[,] b = Matrix.Random(2, 3);
            double[,] actual = new double[5, 2];

            double[,] expected = Matrix.Dot(a, b.Transpose());
            Matrix.DotWithTransposed(a, b, actual);

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void DotWithTransposeBatchTest()
        {
            double Tolerance = 1e-12;

            for (int i = 1; i < 10; i++)
                for (int j = 1; j < 10; j++)
                    for (int k = 1; k < 10; k++)
                    {
                        double[,] a = Matrix.Random(i, k);
                        double[,] b = Matrix.Random(j, k);
                        double[,] actual = Matrix.Random(i, j);

                        double[,] expected = a.Dot(b.Transpose());
                        a.DotWithTransposed(b, actual);

                        Assert.IsTrue(expected.IsEqual(actual, Tolerance));
                    }
        }

        [Test]
        public void DotWithTransposeTest_Jagged()
        {
            double[][] a = Jagged.Random(5, 3);
            double[][] b = Jagged.Random(2, 3);

            double[][] expected = a.Dot(b.Transpose());
            double[][] actual = a.DotWithTransposed(b);

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void DotWithTransposeBatchTest_Jagged()
        {
            double Tolerance = 1e-12;

            for (int i = 1; i < 10; i++)
                for (int j = 1; j < 10; j++)
                    for (int k = 1; k < 10; k++)
                    {
                        double[][] a = Jagged.Random(i, k);
                        double[][] b = Jagged.Random(j, k);
                        double[][] actual = Jagged.Random(i, j);

                        double[][] expected = a.Dot(b.Transpose());
                        a.DotWithTransposed(b, actual);

                        Assert.IsTrue(expected.IsEqual(actual, Tolerance));
                    }
        }

        [Test]
        public void DotWithTransposeTest_Jagged1()
        {
            double[][] a = Jagged.Random(5, 3);
            double[,] b = Matrix.Random(2, 3);

            double[][] expected = a.Dot(b.Transpose());
            double[][] actual = a.DotWithTransposed(b);

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void DotWithTransposeBatchTest_Jagged1()
        {
            double Tolerance = 1e-12;

            for (int i = 1; i < 10; i++)
                for (int j = 1; j < 10; j++)
                    for (int k = 1; k < 10; k++)
                    {
                        double[][] a = Jagged.Random(i, k);
                        double[,] b = Matrix.Random(j, k);
                        double[][] actual = Jagged.Random(i, j);

                        double[][] expected = a.Dot(b.Transpose());
                        a.DotWithTransposed(b, actual);

                        Assert.IsTrue(expected.IsEqual(actual, Tolerance));
                    }
        }

        [Test]
        public void DotWithTransposeTest_Jagged2()
        {
            double[,] a = Matrix.Random(5, 3);
            double[][] b = Jagged.Random(2, 3);

            double[][] expected = a.Dot(b.Transpose());
            double[][] actual = a.DotWithTransposed(b);

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void DotWithTransposeBatchTest_Jagged2()
        {
            double Tolerance = 1e-12;

            for (int i = 1; i < 10; i++)
                for (int j = 1; j < 10; j++)
                    for (int k = 1; k < 10; k++)
                    {
                        double[,] a = Matrix.Random(i, k);
                        double[][] b = Jagged.Random(j, k);
                        double[][] actual = Jagged.Random(i, j);

                        double[][] expected = a.Dot(b.Transpose());
                        a.DotWithTransposed(b, actual);

                        Assert.IsTrue(expected.IsEqual(actual, Tolerance));
                    }
        }

        [Test]
        public void DotAndDotTest()
        {
            for (int a = 1; a < 10; a++)
            {
                for (int b = 1; b < 10; b++)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        double[] A = Vector.Random(a);
                        double[,] C = Matrix.Random(a, b);
                        double[] B = Vector.Random(b);

                        double result1 = A.Dot(C).Dot(B);
                        double result2 = A.DotAndDot(C, B);
                        Assert.AreEqual(result1, result2, 1e-10);

                        double result3 = A.Dot(C.ToJagged()).Dot(B);
                        double result4 = A.DotAndDot(C.ToJagged(), B);
                        Assert.AreEqual(result1, result3, 1e-10);
                        Assert.AreEqual(result3, result4, 1e-10);
                    }
                }
            }
        }

        [Test]
        public void KroneckerTest()
        {
            double[,] a = new double[,]
            {
              { 3, 1 },
              { 5, 2 }
            };

            double[,] b = new double[,]
            {
              { 2, 4 },
              { 7, 6 },
            };

            double[,] expected = new double[,]
            {
              { 6,  12, 2, 4 },
              { 21, 18, 7, 6 },
              { 10, 20, 4, 8 },
              { 35, 30, 14, 12 },
            };

            double[,] actual1 = a.Kronecker(b);
            double[,] actual2 = a.Kronecker(b.ToJagged()).ToMatrix();
            double[,] actual3 = a.ToJagged().Kronecker(b).ToMatrix();
            double[,] actual4 = a.ToJagged().Kronecker(b.ToJagged()).ToMatrix();

            Assert.IsTrue(expected.IsEqual(actual1));
            Assert.IsTrue(expected.IsEqual(actual2));
            Assert.IsTrue(expected.IsEqual(actual3));
            Assert.IsTrue(expected.IsEqual(actual4));
        }

        [Test]
        public void KroneckerVectorTest()
        {
            double[] a = { 3, 1 };
            double[] b = { 4, 7, 2 };

            double[] expected = { 12, 21, 6, 4, 7, 2 };
            double[] result = Vector.Random(a.Length * b.Length);

            double[] actual1 = a.Kronecker(b);

            Assert.IsTrue(expected.IsEqual(actual1));
        }

        [Test]
        public void KroneckerVectorBatchTest()
        {
            const double Tolerance = 1e-12;

            for (int i = 1; i < 5; i++)
                for (int j = 1; j < 5; j++)
                {
                    double[] a = Vector.Random(i);
                    double[] b = Vector.Random(j);
                    double[] result = Vector.Random(a.Length * b.Length);
                    double[] expected = a.Outer(b).Flatten();

                    double[] actual = a.Kronecker(b, result);

                    Assert.IsTrue(expected.IsEqual(actual, Tolerance));
                    Assert.AreSame(actual, result);
                }
        }

        [Test]
        public void KroneckerBatchTest()
        {
            const double Tolerance = 1e-12;

            for (int i = 1; i < 5; i++)
            {
                for (int j = 1; j < 5; j++)
                {
                    for (int k = 1; k < 5; k++)
                    {
                        for (int l = 1; l < 5; l++)
                        {
                            double[,] a = Matrix.Random(i, j);
                            double[,] b = Matrix.Random(k, l);

                            double[,] result1 = a.Kronecker(b);
                            double[,] result2 = a.Kronecker(b.ToJagged()).ToMatrix();
                            double[,] result3 = a.ToJagged().Kronecker(b).ToMatrix();
                            double[,] result4 = a.ToJagged().Kronecker(b.ToJagged()).ToMatrix();

                            Assert.IsTrue(result1.IsEqual(result1, Tolerance));
                            Assert.IsTrue(result1.IsEqual(result2, Tolerance));
                            Assert.IsTrue(result1.IsEqual(result3, Tolerance));
                            Assert.IsTrue(result1.IsEqual(result4, Tolerance));
                        }
                    }
                }
            }
        }

        [Test]
        public void CrossProductTest()
        {
            double[] u = { 3, -3, 1 };
            double[] v = { 4, +9, 2 };
            double[] r = Vector.Zeros(3);

            double[] expected = { -15, -2, 39 };
            double[] actual = Matrix.Cross(u, v, r);

            Assert.IsTrue(expected.IsEqual(actual));
            Assert.AreEqual(actual, r);
        }
    }
}
