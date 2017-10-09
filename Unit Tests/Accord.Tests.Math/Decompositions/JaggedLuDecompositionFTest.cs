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
    using Accord.Math.Decompositions;
    using NUnit.Framework;
    using Accord.Math;
    using System;

    [TestFixture]
    public class JaggedLuDecompositionTest
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

                    var target = new JaggedLuDecompositionF(value);

                    var solution = target.Solve(I);
                    var inverse = target.Inverse();

                    Assert.IsTrue(Matrix.IsEqual(solution, inverse));
                }
            }
        }


        [Test]
        public void SolveTest1()
        {
            float[][] value =
            {
               new float[] {  2,  3,  0 },
               new float[] { -1,  2,  1 },
               new float[] {  0, -1,  3 }
            };

            float[] rhs = { 5, 0, 1 };

            float[] expected =
            {
                1.6522f,
                0.5652f,
                0.5217f,
            };

            JaggedLuDecompositionF target = new JaggedLuDecompositionF(value);

            float[] actual = target.Solve(rhs);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.001f));
        }

        [Test]
        public void InverseTest()
        {
            float[][] value =
            {
               new float[] {  2,  3,  0 },
               new float[] { -1,  2,  1 },
               new float[] {  0, -1,  3 }
            };

            float[][] expectedInverse =
            {
                new float[] { 0.3043f,   -0.3913f,    0.1304f },
                new float[] { 0.1304f,    0.2609f,   -0.0870f },
                new float[] { 0.0435f,    0.0870f,    0.3043f },
            };

            JaggedLuDecompositionF target = new JaggedLuDecompositionF(value);

            float[][] actualInverse = target.Inverse();

            Assert.IsTrue(Matrix.IsEqual(expectedInverse, actualInverse, 0.001f));
        }

        [Test]
        public void SolveTest()
        {
            float[][] value =
            {
               new float[] {  2,  3,  0 },
               new float[] { -1,  2,  1 },
               new float[] {  0, -1,  3 }
            };

            float[][] rhs =
            {
                new float[] { 1, 2, 3 },
                new float[] { 3, 2, 1 },
                new float[] { 5, 0, 1 },
            };

            float[][] expected =
            {
                new float[] { -0.2174f,   -0.1739f,    0.6522f },
                new float[] {  0.4783f,    0.7826f,    0.5652f },
                new float[] {  1.8261f,    0.2609f,    0.5217f },
            };

            JaggedLuDecompositionF target = new JaggedLuDecompositionF(value);

            float[][] actual = target.Solve(rhs);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.001f));
        }

        [Test]
        public void SolveTest3()
        {
            float[][] value =
            {
               new float[] {  2,  3,  0 },
               new float[] { -1,  2,  1 },
            };

            JaggedLuDecompositionF target = new JaggedLuDecompositionF(value);

            float[][] L = target.LowerTriangularFactor;
            float[][] U = target.UpperTriangularFactor;

            float[][] expectedL =
            {
               new float[] {  1.000f, 0.000f },
               new float[] { -0.500f, 1.000f },
            };

            float[][] expectedU =
            {
                new float[] { 2.000f, 3.000f, 0.000f },
                new float[] { 0.000f, 3.500f, 1.000f  },
            };


            Assert.IsTrue(Matrix.IsEqual(expectedL, L, 0.001f));
            Assert.IsTrue(Matrix.IsEqual(expectedU, U, 0.001f));
        }

        [Test]
        public void SolveTest4()
        {
            float[][] value =
            {
                new float[] { 2.1f, 3.1f },
                new float[] { 1.6f, 4.2f },
            };

            float[] rhs = { 6.1f, 4.3f };

            float[] expected = { 3.1839f, -0.1891f };

            JaggedLuDecompositionF target = new JaggedLuDecompositionF(value);

            float[] actual = target.Solve(rhs);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.001f));
        }

        [Test]
        public void SolveTest5()
        {
            float[][] value =
            {
                new float[] { 2.1f, 3.1f },
                new float[] { 1.6f, 4.2f },
                new float[] { 2.1f, 5.1f },
            };

            float[] rhs = { 6.1f, 4.3f, 2.1f };

            float[] expected = { 3.1839f, -0.1891f };

            JaggedLuDecompositionF target = new JaggedLuDecompositionF(value);

            bool thrown = false;
            try
            {
                float[] actual = target.Solve(rhs);
            }
            catch (InvalidOperationException) { thrown = true; }

            Assert.IsTrue(thrown);
        }

        [Test]
        public void SolveTransposeTest()
        {
            float[][] a =
            {
                new float[] { 2, 1, 4 },
                new float[] { 6, 2, 2 },
                new float[] { 0, 1, 6 },
            };

            float[][] b =
            {
                new float[] { 1, 0, 7 },
                new float[] { 5, 2, 1 },
                new float[] { 1, 5, 2 },
            };

            float[][] expected =
            {
                 new float[] { 0.5062f,    0.2813f,    0.0875f },
                 new float[] { 0.1375f,    1.1875f,   -0.0750f },
                 new float[] { 0.8063f,   -0.2188f,    0.2875f },
            };

            float[][] actual = new JaggedLuDecompositionF(b, transpose: true).SolveTranspose(a);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.001f));
        }

        [Test]
        public void LuDecompositionConstructorTest()
        {
            #region doc_ctor
            // Let's say we would like to compute the
            // LU decomposition of the following matrix:
            double[][] matrix =
            {
               new double[] {  2, -1,  0 },
               new double[] { -1,  2, -1 },
               new double[] {  0, -1,  2 }
            };

            // Compute the LU decomposition with:
            var lu = new JaggedLuDecomposition(matrix);


            // Retrieve the lower triangular factor L:
            double[][] L = lu.LowerTriangularFactor;

            // Should be equal to
            double[][] expectedL =
            {
                new double[] {  1.0000,         0,         0 },
                new double[] { -0.5000,    1.0000,         0 },
                new double[] {       0,   -0.6667,    1.0000 },
            };


            // Retrieve the upper triangular factor U:
            double[][] U = lu.UpperTriangularFactor;

            // Should be equal to
            double[][] expectedU =
            {
                new double[] { 2.0000,   -1.0000,         0 },
                new double[] {      0,    1.5000,   -1.0000 },
                new double[] {      0,         0,    1.3333 },
             };


            // Certify that the decomposition has worked as expected by
            // trying to reconstruct the original matrix with R = L * U:
            double[][] reconstruction = L.Dot(U);

            // reconstruction should be equal to
            // {
            //     {  2, -1,  0 },
            //     { -1,  2, -1 },
            //     {  0, -1,  2 }
            // };
            #endregion


            Assert.IsTrue(Matrix.IsEqual(matrix, reconstruction, 1e-4));
            Assert.IsTrue(Matrix.IsEqual(expectedL, L, 1e-4));
            Assert.IsTrue(Matrix.IsEqual(expectedU, U, 1e-4));


            lu = new JaggedLuDecomposition(matrix.Transpose(), true);

            L = lu.LowerTriangularFactor;
            U = lu.UpperTriangularFactor;

            Assert.IsTrue(Matrix.IsEqual(expectedL, L, 0.001));
            Assert.IsTrue(Matrix.IsEqual(expectedU, U, 0.001));
        }

        [Test]
        public void LogDeterminantTest()
        {
            JaggedLuDecompositionF lu = new JaggedLuDecompositionF(
                CholeskyDecompositionTest.bigmatrix.ToSingle().ToJagged());
            Assert.AreEqual(0, lu.Determinant);
            Assert.AreEqual(-2224.8931093738875, lu.LogDeterminant, 1e-3);
            Assert.IsTrue(lu.Nonsingular);
        }

        [Test]
        public void DeterminantTest()
        {
            float[][] value =
            {
               new float[] {  2,  3,  0 },
               new float[] { -1,  2,  1 },
               new float[] {  0, -1,  3 }
            };

            JaggedLuDecompositionF lu = new JaggedLuDecompositionF(value);
            Assert.AreEqual(23, lu.Determinant);
            Assert.IsTrue(lu.Nonsingular);
        }

        [Test]
        public void LogDeterminantTest2()
        {
            float[][] value =
            {
               new float[] {  2,  3,  0 },
               new float[] { -1,  2,  1 },
               new float[] {  0, -1,  3 }
            };

            JaggedLuDecompositionF lu = new JaggedLuDecompositionF(value);
            Assert.AreEqual(23, lu.Determinant);

            double expected = System.Math.Log(23);
            double actual = lu.LogDeterminant;

            Assert.AreEqual(expected, actual, 1e-5);
        }

        [Test]
        public void solve_for_diagonal()
        {
            float[][] value =
            {
                new float[] { 2.1f, 3.1f },
                new float[] { 1.6f, 4.2f },
            };

            float[] rhs = { 6.1f, 4.3f };

            float[][] expected = 
            {
                new float[] {  6.63730669f, -3.45336843f  },
                new float[] { -2.528498f,    2.3393786f   }
            };

            var target = new JaggedLuDecompositionF(value);

            float[][] actual = target.SolveForDiagonal(rhs);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-6));
            Assert.IsTrue(Matrix.IsEqual(value, target.Reverse(), 1e-6));
        }
    }
}
