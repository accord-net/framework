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
    using System;
    using Accord.Math;
    using Accord.Math.Decompositions;
    using NUnit.Framework;

    [TestFixture]
    public class JaggedCholeskyDecompositionFTest
    {


        [Test]
        public void InverseTestNaN()
        {
            int n = 5;

            var I = Matrix.Identity(n).ToSingle().ToJagged();

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    var value = Matrix.Magic(n).ToJagged().ToSingle();

                    value[i][j] = Single.NaN;

                    bool thrown = false;

                    var target = new JaggedCholeskyDecompositionF(value);

                    try
                    {
                        target.Solve(I);
                    }
                    catch (NonPositiveDefiniteMatrixException)
                    {
                        thrown = true;
                    }

                    Assert.IsTrue(thrown);
                }
            }
        }


        [Test]
        public void JaggedCholeskyDecompositionFConstructorTest()
        {
            // Based on tests by Ken Johnson

            float[][] value = // positive-definite
            {
               new float[] {  2, -1,  0 },
               new float[] { -1,  2, -1 },
               new float[] {  0, -1,  2 }
            };

            float[][] expected =
            {
               new float[]  {  1.4142f,  0.0000f, 0.0000f },
               new float[]  { -0.7071f,  1.2247f, 0.0000f },
               new float[]  {  0.0000f, -0.8165f, 1.1547f }
            };


            var chol = new JaggedCholeskyDecompositionF(value);
            float[][] L = chol.LeftTriangularFactor;

            Assert.IsTrue(Matrix.IsEqual(L, expected, 1e-4f));

            // Decomposition Identity
            Assert.IsTrue(Matrix.IsEqual(Matrix.Multiply(L, L.Transpose()), value, 1e-3f));

            Assert.AreEqual(new JaggedLuDecompositionF(value).Determinant, chol.Determinant, 1e-5);
            Assert.IsTrue(chol.IsPositiveDefinite);
        }

        [Test]
        public void SolveTest()
        {
            float[][] value = // positive-definite
            {
               new float[] {  2, -1,  0 },
               new float[] { -1,  2, -1 },
               new float[] {  0, -1,  2 }
            };

            JaggedCholeskyDecompositionF chol = new JaggedCholeskyDecompositionF(value);
            float[][] L = chol.LeftTriangularFactor;

            float[][] B = Matrix.ColumnVector(new float[] { 1, 2, 3 }).ToJagged();

            float[][] expected = Matrix.ColumnVector(new float[] { 2.5f, 4.0f, 3.5f }).ToJagged();

            float[][] actual = chol.Solve(B);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-6f));
            Assert.AreNotEqual(actual, B);

            actual = chol.Solve(B, true);
            Assert.AreEqual(actual, B);
            Assert.IsTrue(Matrix.IsEqual(expected, B, 1e-6f));
        }

        [Test]
        public void SolveTest3()
        {
            float[][] value = // positive-definite
            {
               new float[] {  2, -1,  0 },
               new float[] { -1,  2, -1 },
               new float[] {  0, -1,  2 }
            };

            JaggedCholeskyDecompositionF chol = new JaggedCholeskyDecompositionF(value);
            float[][] L = chol.LeftTriangularFactor;

            float[] B = new float[] { 1, 2, 3 };

            float[] expected = new float[] { 2.5f, 4.0f, 3.5f };
            float[] actual = chol.Solve(B);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-5f));

            actual = chol.Solve(B, true);
            Assert.AreEqual(actual, B);
            Assert.IsTrue(Matrix.IsEqual(expected, B, 1e-5f));
        }

        [Test]
        public void SolveTest2()
        {
            float[][] value = // not positive-definite
            {
               new float[] {  6, -1,  2,  6 },
               new float[] { -1,  3, -3, -2 },
               new float[] {  2, -3,  2,  0 },
               new float[] {  6, -2,  0,  0 },
            };

            var chol = new JaggedCholeskyDecompositionF(value, robust: true);
            float[][] L = chol.LeftTriangularFactor;

            float[][] B = Matrix.Identity(4).ToSingle().ToJagged();

            float[][] expected =
            {
                new float[] { 0.4000f,    1.2000f,    1.4000f,   -0.5000f },
                new float[] { 1.2000f,    3.6000f,    4.2000f,   -2.0000f },
                new float[] { 1.4000f,    4.2000f,    5.4000f,   -2.5000f },
                new float[] { -0.5000f,  -2.0000f,   -2.5000f,    1.0000f },
            };

            float[][] actual = chol.Solve(B);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-4f));
        }

        [Test]
        public void SolveTest4()
        {
            float[][] value = // not positive-definite
            {
               new float[] {  6, -1,  2,  6 },
               new float[] { -1,  3, -3, -2 },
               new float[] {  2, -3,  2,  0 },
               new float[] {  6, -2,  0,  0 },
            };

            JaggedCholeskyDecompositionF chol = new JaggedCholeskyDecompositionF(value, robust: true);
            float[][] L = chol.LeftTriangularFactor;

            float[] B = new float[] { 1, 2, 3, 4 };

            float[] expected = { 5, 13, 16, -8 };
            float[] actual = chol.Solve(B);

            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i], 1e-3);
        }

        [Test]
        public void InverseTest()
        {
            float[][] value = // not positive-definite
            {
               new float[] {  6, -1,  2,  6 },
               new float[] { -1,  3, -3, -2 },
               new float[] {  2, -3,  2,  0 },
               new float[] {  6, -2,  0,  0 },
            };

            var chol = new JaggedCholeskyDecompositionF(value, robust: true);
            float[][] L = chol.LeftTriangularFactor;

            float[][] expected =
            {
                new float[] {  0.400f,  1.200f,  1.400f, -0.500f },
                new float[] {  1.200f,  3.600f,  4.200f, -2.000f },
                new float[] {  1.400f,  4.200f,  5.400f, -2.500f },
                new float[] { -0.500f, -2.000f, -2.500f,  1.000f },
            };

            float[][] actual = chol.Inverse();

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-4f));
        }

        [Test]
        public void InverseTest1()
        {
            float[][] value = // positive-definite
            {
               new float[] {  2, -1,  0 },
               new float[] { -1,  2, -1 },
               new float[] {  0, -1,  2 }
            };

            var chol = new JaggedCholeskyDecompositionF(value, robust: false);
            Assert.IsTrue(chol.IsPositiveDefinite);
            float[][] L = chol.LeftTriangularFactor;

            float[][] expected =
            {
                new float[] { 0.750f, 0.500f, 0.250f },
                new float[] { 0.500f, 1.000f, 0.500f },
                new float[] { 0.250f, 0.500f, 0.750f },
            };

            float[][] actual = chol.Inverse();
            Assert.IsTrue(actual.IsEqual(expected, 1e-5f));

            float[][] inv = chol.Solve(Jagged.Identity<float>(3));
            Assert.IsTrue(inv.IsEqual(expected, 1e-5f));
        }

        [Test]
        public void InverseDiagonalTest1()
        {
            float[][] value = // positive-definite
            {
               new float[] {  2, -1,  0 },
               new float[] { -1,  2, -1 },
               new float[] {  0, -1,  2 }
            };

            var chol = new JaggedCholeskyDecompositionF(value, robust: false);
            float[][] L = chol.LeftTriangularFactor;

            float[] expected = Matrix.Inverse(value.ToMatrix().ToDouble()).ToSingle().ToJagged().Diagonal();
            float[] actual = chol.InverseDiagonal();

            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i], 1e-6f);
        }

        [Test]
        public void InverseDiagonalTest2()
        {
            float[][] value = // positive-definite
            {
               new float[] {  2, -1,  0 },
               new float[] { -1,  2, -1 },
               new float[] {  0, -1,  2 }
            };

            float[] expected = Matrix.Inverse(value.ToMatrix().ToDouble()).ToSingle().ToJagged().Diagonal();

            var chol = new JaggedCholeskyDecompositionF(value, robust: false, inPlace: true);

            float[] actual = chol.InverseDiagonal();

            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i], 1e-6f);
        }

        [Test]
        public void InverseDiagonalTest3()
        {
            float[][] value = // not positive-definite
            {
               new float[] {  6, -1,  2,  6 },
               new float[] { -1,  3, -3, -2 },
               new float[] {  2, -3,  2,  0 },
               new float[] {  6, -2,  0,  0 },
            };

            float[] expected = Matrix.Inverse(value.ToMatrix().ToDouble()).ToSingle().ToJagged().Diagonal();

            var chol = new JaggedCholeskyDecompositionF(value, robust: true, inPlace: true);

            float[] actual = chol.InverseDiagonal();

            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i], 1e-4f);
        }

        [Test]
        public void InPlaceTest()
        {
            float[][] value = // positive-definite
            {
                new float[] {  9, -1,  0,  2, -3 },
                new float[] { -1, 11, -1,  3,  9 },
                new float[] {  0, -1, 15, -2, -2 },
                new float[] {  2,  3, -2, 16,  8 },
                new float[] { -3,  9, -2,  8, 11 },
            };

            float[][] original = value.MemberwiseClone();

            Assert.IsTrue(value.IsSymmetric());

            var chol = new JaggedCholeskyDecompositionF(value, robust: false, inPlace: true);
            float[] diagonal = value.Diagonal();

            // Upper triangular should contain the original matrix (except diagonal)
            for (int i = 1; i < value.Length; i++)
                for (int j = i + 1; j < value.Length; j++)
                    Assert.AreEqual(original[i][j], value[i][j]);

            // Lower triangular should contain the Cholesky factorization
            float[][] expected =
            {
                new float[] {  3.000f,       0,       0,       0,       0 },
                new float[] { -0.333f,  3.300f,       0,       0,       0 },
                new float[] {  0.000f, -0.303f,  3.861f,       0,       0 },
                new float[] {  0.667f,  0.976f, -0.441f,  3.796f,       0 },
                new float[] { -1.000f,  2.626f, -0.312f,  1.571f,  0.732f },
            };

            float[][] L = chol.LeftTriangularFactor;
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j <= i; j++)
                    Assert.AreEqual(expected[i][j], value[i][j], 1e-3);
        }

        [Test]
        public void CholeskyDecompositionFConstructorTest2()
        {
            float[][] value = // positive-definite
            {
               new float[] {  2, -1,  0 },
               new float[] { -1,  2, -1 },
               new float[] {  0, -1,  2 }
            };


            float[][] expected =
            {
                 new float[] {  1.0000f,  0.0000f,  0.0000f },
                 new float[] { -0.5000f,  1.0000f,  0.0000f },
                 new float[] {  0.0000f, -0.6667f,  1.0000f }
            };

            float[][] diagonal =
            {
                 new float[] {  2.0000f,  0.0000f,  0.0000f },
                 new float[] {  0.0000f,  1.5000f,  0.0000f },
                 new float[] {  0.0000f,  0.0000f,  1.3333f },
            };


            var chol = new JaggedCholeskyDecompositionF(value, true);
            float[][] L = chol.LeftTriangularFactor;
            float[][] D = chol.DiagonalMatrix;
            Assert.IsTrue(Matrix.IsEqual(L, expected, 0.001f));
            Assert.IsTrue(Matrix.IsEqual(D, diagonal, 0.001f));

            // Decomposition Identity
            Assert.IsTrue(Matrix.IsEqual(Matrix.Multiply(Matrix.Multiply(L, D), L.Transpose()), value, 1e-3f));
            Assert.IsTrue(Matrix.IsEqual(chol.Reverse(), value, 1e-3f));

            Assert.AreEqual(new JaggedLuDecompositionF(value).Determinant, chol.Determinant, 1e-10);
            Assert.IsTrue(chol.IsPositiveDefinite);
        }

        [Test]
        public void CholeskyDecompositionConstructorTest3()
        {
            float[][] value = // not positive-definite
            {
               new float[] {  6, -1,  2,  6 },
               new float[] { -1,  3, -3, -2 },
               new float[] {  2, -3,  2,  0 },
               new float[] {  6, -2,  0,  0 },
            };

            float[][] expected =
            {
                new float[] {  1.0000f,         0f,         0f,         0f },
                new float[] { -0.1667f,    1.0000f,         0f,         0f },
                new float[] {  0.3333f,   -0.9412f,    1.0000f,         0f },
                new float[] {  1.0000f,   -0.3529f,    2.5000f,    1.0000f },
            };

            float[][] diagonal =
            {
                new float[] { 6.0000f,         0f,         0f,         0f },
                new float[] {      0f,    2.8333f,         0f,         0f },
                new float[] {      0f,         0f,   -1.1765f,         0f },
                new float[] {      0f,         0f,         0f,    1.0000f },
            };

            var chol = new JaggedCholeskyDecompositionF(value, robust: true);
            float[][] L = chol.LeftTriangularFactor;
            float[][] D = chol.DiagonalMatrix;

            Assert.IsTrue(Matrix.IsEqual(L, expected, 1e-3f));
            Assert.IsTrue(Matrix.IsEqual(D, diagonal, 1e-3f));

            // Decomposition Identity
            Assert.IsTrue(Matrix.IsEqual(Matrix.Multiply(Matrix.Multiply(L, D), L.Transpose()), value, 1e-3f));
            Assert.IsTrue(Matrix.IsEqual(chol.Reverse(), value, 1e-3f));

            Assert.AreEqual(new JaggedLuDecompositionF(value).Determinant, chol.Determinant, 1e-4f);
            Assert.IsFalse(chol.IsPositiveDefinite);
        }

        [Test]
        public void CholeskyDecompositionConstructorTest4()
        {
            // Based on tests by Ken Johnson

            float[][] value = // positive-definite
            {
               new float[] {  2f,  0f,  0f },
               new float[] { -1f,  2f,  0f },
               new float[] {  0f, -1f,  2f }
            };

            float[][] expected =
            {
                new float[] {  1.4142f,  0.0000f, 0.0000f },
                new float[] { -0.7071f,  1.2247f, 0.0000f },
                new float[] {  0.0000f, -0.8165f, 1.1547f }
            };


            var chol = new JaggedCholeskyDecompositionF(value, robust: false, valueType: MatrixType.LowerTriangular);
            float[][] L = chol.LeftTriangularFactor;

            Assert.IsTrue(Matrix.IsEqual(L, expected, 0.0001f));
            Assert.AreEqual(4, chol.Determinant, 1e-6);
            Assert.IsTrue(chol.IsPositiveDefinite);
            var reverse = chol.Reverse();
            Assert.IsTrue(Matrix.IsEqual(reverse, value.GetSymmetric(MatrixType.LowerTriangular), 1e-6f));



            float[][] expected2 =
            {
                 new float[] {  1.0f,  0.0f,        0.0f },
                 new float[] { -0.5f,  1.0f,        0.0f },
                 new float[] {  0.0f, -0.6666667f,  1.0f }
            };

            chol = new JaggedCholeskyDecompositionF(value, robust: true, valueType: MatrixType.LowerTriangular);
            L = chol.LeftTriangularFactor;

            Assert.IsTrue(Matrix.IsEqual(L, expected2, 0.0001f));
            Assert.AreEqual(4, chol.Determinant, 1e-6);
            Assert.IsTrue(chol.IsPositiveDefinite);
            reverse = chol.Reverse();
            Assert.IsTrue(Matrix.IsEqual(reverse, value.GetSymmetric(MatrixType.LowerTriangular), 1e-6f));
        }

        [Test]
        public void solve_for_diagonal()
        {
            float[][] value = // not positive-definite
            {
               new float[] {  6, -1,  2,  6 },
               new float[] { -1,  3, -3, -2 },
               new float[] {  2, -3,  2,  0 },
               new float[] {  6, -2,  0,  0 },
            };

            var chol = new JaggedCholeskyDecompositionF(value, robust: true);
            float[][] L = chol.LeftTriangularFactor;

            float[] B = new float[] { 1, 2, 3, 4 };

            float[][] expected = 
            {
                new float[] { 0.4f,    2.4f,      4.2f,  -2 },
                new float[] { 1.2f,    7.2f,     12.6f,  -8 },
                new float[] { 1.4f,    8.4f,     16.2f, -10 },
                new float[] { -0.5f,  -4.0f,     -7.5f,   4 }
            };

            float[][] actual = chol.SolveForDiagonal(B);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-4f));
            float[][] reverse = chol.Reverse();
            Assert.IsTrue(Matrix.IsEqual(value, reverse, atol: 1e-3f));
        }
    }
}
