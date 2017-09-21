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

    [TestFixture]
    public class JaggedQrDecompositionTest
    {

        [Test]
        public void InverseTestNaN()
        {
            int n = 5;

            var I = Matrix.JaggedIdentity(n);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    double[][] value = Matrix.JaggedMagic(n);

                    value[i][j] = double.NaN;

                    var target = new JaggedQrDecomposition(value);

                    var solution = target.Solve(I);
                    var inverse = target.Inverse();

                    Assert.IsTrue(Matrix.IsEqual(solution, inverse));
                }
            }
        }

        [Test]
        public void QrDecompositionConstructorTest()
        {
            double[][] value =
            {
               new double[] {  2, -1,  0 },
               new double[] { -1,  2, -1 },
               new double[] {  0, -1,  2 }
            };


            var target = new JaggedQrDecomposition(value);

            // Decomposition Identity
            var Q = target.OrthogonalFactor;
            var QQt = Matrix.Multiply(Q, Q.Transpose());
            Assert.IsTrue(Matrix.IsEqual(QQt, Matrix.JaggedIdentity(3), 1e-6));
            Assert.IsTrue(Matrix.IsEqual(value, target.Reverse(), 1e-6));

            // Linear system solving
            double[][] B = Matrix.ColumnVector(new double[] { 1, 2, 3 }).ToJagged();
            double[][] expected = Matrix.ColumnVector(new double[] { 2.5, 4.0, 3.5 }).ToJagged();
            double[][] actual = target.Solve(B);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0000000000001));
        }

        [Test]
        public void QrDecompositionConstructorTest2()
        {
            double[][] value =
            {
               new double[] {  1 },
               new double[] {  2 },
               new double[] {  3 }
            };


            var target = new JaggedQrDecomposition(value);

            // Decomposition Identity
            var Q = target.OrthogonalFactor;
            var QQt = Matrix.Multiply(Q.Transpose(), Q);
            Assert.IsTrue(Matrix.IsEqual(QQt, Matrix.JaggedIdentity(1), 1e-6));
            double[][] reverse = target.Reverse();
            Assert.IsTrue(Matrix.IsEqual(value, reverse, 1e-6));

            // Linear system solving
            double[][] B = Matrix.ColumnVector(new double[] { 4, 5, 6 }).ToJagged();
            double[][] expected = Jagged.ColumnVector(new double[] { 2.285714285714286 });
            double[][] actual = target.Solve(B);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0000000000001));

            double[][] R = target.UpperTriangularFactor;
            actual = value.Dot(R.Inverse());
            Assert.IsTrue(Matrix.IsEqual(Q, actual, 0.0000000000001));
        }

        [Test]
        public void JaggedQrDecompositionCachingTest()
        {
            double[,] value =
            {
               {  2, -1,  0 },
               { -1,  2, -1 },
               {  0, -1,  2 }
            };

            var target = new JaggedQrDecomposition(value.ToJagged());

            // Decomposition Identity
            double[][] Q1 = target.OrthogonalFactor;
            double[][] Q2 = target.OrthogonalFactor;

            double[][] utf1 = target.UpperTriangularFactor;
            double[][] utf2 = target.UpperTriangularFactor;

            Assert.AreSame(Q1, Q2);
            Assert.AreSame(utf1, utf2);
        }

        [Test]
        public void full_decomposition()
        {
            double[][] value =
            {
               new double[] { 1 },
               new double[] { 2 },
               new double[] { 3 }
            };


            var target = new JaggedQrDecomposition(value, economy: false);

            // Decomposition Identity
            var Q = target.OrthogonalFactor;
            var QtQ = Matrix.Multiply(Q.Transpose(), Q);
            Assert.IsTrue(Matrix.IsEqual(QtQ, Matrix.JaggedIdentity(3), 1e-6));
            double[][] reverse = target.Reverse();
            Assert.IsTrue(Matrix.IsEqual(value, reverse, 1e-6));

            // Linear system solving
            double[][] B = Matrix.ColumnVector(new double[] { 4, 5, 6 }).ToJagged();
            double[][] expected = Jagged.ColumnVector(new double[] { 2.285714285714286 });
            double[][] actual = target.Solve(B);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0000000000001));

            var QQt = Matrix.Multiply(Q, Q.Transpose());
            Assert.IsTrue(Matrix.IsEqual(QQt, Matrix.JaggedIdentity(3), 1e-6));
        }

        [Test]
        public void InverseTest()
        {
            double[][] value =
            {
               new double[] {  2, -1,  0 },
               new double[] { -1,  2, -1 },
               new double[] {  0, -1,  2 }
            };

            double[][] expected =
            {
                new double[] { 0.7500,    0.5000,    0.2500},
                new double[] { 0.5000,    1.0000,    0.5000},
                new double[] { 0.2500,    0.5000,    0.7500},
            };


            var target = new JaggedQrDecomposition(value);

            double[][] actual = target.Inverse();
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-10));
            Assert.IsTrue(Matrix.IsEqual(value, target.Reverse(), 1e-4));

            target = new JaggedQrDecomposition(value.Transpose(), true);
            actual = target.Inverse();
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-10));
            Assert.IsTrue(Matrix.IsEqual(value.Transpose(), target.Reverse(), 1e-4));
        }

        [Test]
        public void SolveTest()
        {
            double[][] value =
            {
               new double[] {  2, -1,  0 },
               new double[] { -1,  2, -1 },
               new double[] {  0, -1,  2 }
            };

            double[] b = { 1, 2, 3 };

            double[] expected = { 2.5000, 4.0000, 3.5000 };

            var target = new JaggedQrDecomposition(value);
            double[] actual = target.Solve(b);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-10));
            Assert.IsTrue(Matrix.IsEqual(value.Transpose(), target.Reverse(), 1e-6));
        }

        [Test]
        public void SolveTest2()
        {
            // Example from Lecture notes for MATHS 370: Advanced Numerical Methods
            // http://www.math.auckland.ac.nz/~sharp/370/qr-solving.pdf

            double[][] value =
            {
                new double[] { 1,           0,           0 },
                new double[] { 1,           7,          49 },
                new double[] { 1,          14,         196 },
                new double[] { 1,          21,         441 },
                new double[] { 1,          28,         784 },
                new double[] { 1,          35,        1225 },
            };

            // Matrices
            {
                double[][] b =
                {
                    new double[] { 4 },
                    new double[] { 1 },
                    new double[] { 0 },
                    new double[] { 0 },
                    new double[] { 2 },
                    new double[] { 5 },
                };

                double[][] expected =
                {
                    new double[] { 3.9286  },
                    new double[] { -0.5031 },
                    new double[] { 0.0153  },
                };

                var target = new JaggedQrDecomposition(value);
                double[][] actual = target.Solve(b);

                Assert.IsTrue(Matrix.IsEqual(expected, actual, atol: 1e-4));
                Assert.IsTrue(Matrix.IsEqual(value, target.Reverse(), 1e-6));
            }

            // Vectors
            {
                double[] b = { 4, 1, 0, 0, 2, 5 };
                double[] expected = { 3.9286, -0.5031, 0.0153 };

                var target = new JaggedQrDecomposition(value);
                double[] actual = target.Solve(b);

                Assert.IsTrue(Matrix.IsEqual(expected, actual, atol: 1e-4));
            }
        }

        [Test]
        public void Solve_full()
        {
            // Example from Lecture notes for MATHS 370: Advanced Numerical Methods
            // http://www.math.auckland.ac.nz/~sharp/370/qr-solving.pdf

            double[][] value =
            {
                new double[] { 1,           0,           0 },
                new double[] { 1,           7,          49 },
                new double[] { 1,          14,         196 },
                new double[] { 1,          21,         441 },
                new double[] { 1,          28,         784 },
                new double[] { 1,          35,        1225 },
            };

            // Matrices
            {
                double[][] b =
                {
                    new double[] { 4 },
                    new double[] { 1 },
                    new double[] { 0 },
                    new double[] { 0 },
                    new double[] { 2 },
                    new double[] { 5 },
                };

                double[][] expected =
                {
                    new double[] { 3.9286  },
                    new double[] { -0.5031 },
                    new double[] { 0.0153  },
                };

                var target = new JaggedQrDecomposition(value, economy: false);
                double[][] actual = target.Solve(b);

                Assert.IsTrue(Matrix.IsEqual(expected, actual, atol: 1e-4));
                Assert.IsTrue(Matrix.IsEqual(value, target.Reverse(), 1e-6));
            }

            // Vectors
            {
                double[] b = { 4, 1, 0, 0, 2, 5 };
                double[] expected = { 3.9286, -0.5031, 0.0153 };

                var target = new JaggedQrDecomposition(value, economy: false);
                double[] actual = target.Solve(b);

                Assert.IsTrue(Matrix.IsEqual(expected, actual, atol: 1e-4));
            }
        }


        [Test]
        public void SolveTest3()
        {
            double[][] value =
            {
                new double[] { 41.9, 29.1, 1 },
                new double[] { 43.4, 29.3, 1 },
                new double[] { 43.9, 29.5, 0 },
                new double[] { 44.5, 29.7, 0 },
                new double[] { 47.3, 29.9, 0 },
                new double[] { 47.5, 30.3, 0 },
                new double[] { 47.9, 30.5, 0 },
                new double[] { 50.2, 30.7, 0 },
                new double[] { 52.8, 30.8, 0 },
                new double[] { 53.2, 30.9, 0 },
                new double[] { 56.7, 31.5, 0 },
                new double[] { 57.0, 31.7, 0 },
                new double[] { 63.5, 31.9, 0 },
                new double[] { 65.3, 32.0, 0 },
                new double[] { 71.1, 32.1, 0 },
                new double[] { 77.0, 32.5, 0 },
                new double[] { 77.8, 32.9, 0 }
            };

            double[][] b =
            {
                new double[] { 251.3 },
                new double[] { 251.3 },
                new double[] { 248.3 },
                new double[] { 267.5 },
                new double[] { 273.0 },
                new double[] { 276.5 },
                new double[] { 270.3 },
                new double[] { 274.9 },
                new double[] { 285.0 },
                new double[] { 290.0 },
                new double[] { 297.0 },
                new double[] { 302.5 },
                new double[] { 304.5 },
                new double[] { 309.3 },
                new double[] { 321.7 },
                new double[] { 330.7 },
                new double[] { 349.0 }
            };

            double[][] expected =
            {
                new double[] { 1.7315235669547167  },
                new double[] { 6.25142110500275 },
                new double[] { -5.0909763966987178  },
            };

            var target = new JaggedQrDecomposition(value);
            double[][] actual = target.Solve(b);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, atol: 1e-4));
            var reconstruct = value.Dot(expected);
            Assert.IsTrue(Matrix.IsEqual(reconstruct, b, rtol: 1e-1));
            double[] b2 = b.GetColumn(0);
            double[] expected2 = expected.GetColumn(0);

            var target2 = new JaggedQrDecomposition(value);
            double[] actual2 = target2.Solve(b2);
            Assert.IsTrue(Matrix.IsEqual(expected2, actual2, atol: 1e-4));

            var targetMat = new QrDecomposition(value.ToMatrix());
            double[,] actualMat = targetMat.Solve(b.ToMatrix());

            Assert.IsTrue(Matrix.IsEqual(expected, actualMat, atol: 1e-4));
            var reconstructMat = value.ToMatrix().Dot(expected);
            Assert.IsTrue(Matrix.IsEqual(reconstruct, b, rtol: 1e-1));

            var targetMat2 = new QrDecomposition(value.ToMatrix());
            Assert.IsTrue(Matrix.IsEqual(target2.Diagonal, targetMat2.Diagonal, atol: 1e-4));
            Assert.IsTrue(Matrix.IsEqual(target2.OrthogonalFactor, targetMat2.OrthogonalFactor, atol: 1e-4));
            Assert.IsTrue(Matrix.IsEqual(target2.UpperTriangularFactor, targetMat2.UpperTriangularFactor, atol: 1e-4));

            double[] actualMat2 = targetMat2.Solve(b2);
            Assert.IsTrue(Matrix.IsEqual(expected2, actualMat2, atol: 1e-4));
        }

        [Test]
        public void InverseTest1()
        {
            double[][] value =
            {
                new double[] { 41.9, 29.1, 1 },
                new double[] { 43.4, 29.3, 1 },
                new double[] { 43.9, 29.5, 0 },
                new double[] { 44.5, 29.7, 0 },
                new double[] { 47.3, 29.9, 0 },
                new double[] { 47.5, 30.3, 0 },
                new double[] { 47.9, 30.5, 0 },
                new double[] { 50.2, 30.7, 0 },
                new double[] { 52.8, 30.8, 0 },
                new double[] { 53.2, 30.9, 0 },
                new double[] { 56.7, 31.5, 0 },
                new double[] { 57.0, 31.7, 0 },
                new double[] { 63.5, 31.9, 0 },
                new double[] { 65.3, 32.0, 0 },
                new double[] { 71.1, 32.1, 0 },
                new double[] { 77.0, 32.5, 0 },
                new double[] { 77.8, 32.9, 0 }
            };

            double[][] expected = new JaggedSingularValueDecomposition(value,
                computeLeftSingularVectors: true,
                computeRightSingularVectors: true,
                autoTranspose: true).Inverse();

            var target = new JaggedQrDecomposition(value);
            double[][] actual = target.Inverse();
            Assert.IsTrue(Matrix.IsEqual(expected, actual, atol: 1e-4));

            var targetMat = new QrDecomposition(value.ToMatrix());
            double[][] actualMat = target.Inverse();
            Assert.IsTrue(Matrix.IsEqual(expected, actualMat, atol: 1e-4));
        }


        [Test]
        public void SolveTransposeTest()
        {
            double[][] a =
            {
                new double[] { 2, 1, 4 },
                new double[] { 6, 2, 2 },
                new double[] { 0, 1, 6 },
            };

            double[][] b =
            {
                new double[] { 1, 0, 7 },
                new double[] { 5, 2, 1 },
                new double[] { 1, 5, 2 },
            };

            double[][] expected =
            {
                 new double[] { 0.5062,    0.2813,    0.0875 },
                 new double[] { 0.1375,    1.1875,   -0.0750 },
                 new double[] { 0.8063,   -0.2188,    0.2875 },
            };

            double[][] actual = new JaggedQrDecomposition(b, true).SolveTranspose(a);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.001));

            var bX = b.Dot(expected);
            var Xb = expected.Dot(b);
            Assert.IsTrue(Matrix.IsEqual(Xb, a, atol: 1e-3));
            Assert.IsFalse(Matrix.IsEqual(bX, a, atol: 1e-3));

            double[,] actualMatrix = new QrDecomposition(b.ToMatrix(), true).SolveTranspose(a.ToMatrix());
            Assert.IsTrue(Matrix.IsEqual(expected, actualMatrix, 0.001));
        }


        private void decompose(ref double[][] a, out double[][] q)
        {
            int m = a.Rows();
            int n = a.Columns();
            q = Jagged.Identity(m);
            for (int i = 0; i < n; i++)
            {
                var h = Jagged.Identity(m);
                h.Set(i, 0, i, 0, householder(a.GetColumn(i).Get(i, 0)));
                q = q.Dot(h);
                a = h.Dot(a);
            }
        }

        private double[][] householder(double[] a)
        {
            double[] v = a.Divide((a[0] + Special.Sign(Norm.Euclidean(a), a[0])));
            v[0] = 1;
            var H = Jagged.Identity(a.Length);
            var vr = Jagged.RowVector(v);
            var vc = Jagged.ColumnVector(v);
            var t = vc.Dot(vr);
            H.Subtract((2 / v.Dot(v)).Multiply(t), result: H);
            return H;
        }

        [Test]
        public void householderTest()
        {
            double[][] m = Jagged.ColumnVector(1.0, 2.0, 3.0);
            double[][] q;
            decompose(ref m, out q);

            double[][] expectedM =
            {
                new double[] { -3.7416573867739404 },
                new double[] { 0 },
                new double[] { 0 },
            };

            double[][] expectedQ =
            {
                new double[] { -0.26726124191242406, -0.53452248382484868, -0.80178372573727308 },
                new double[] { -0.53452248382484868, 0.77454192058843829, -0.33818711911734262 },
                new double[] { -0.80178372573727308, -0.33818711911734262, 0.4927193213239861 },
            };

            Assert.IsTrue(expectedM.IsEqual(m, 1e-6));
            Assert.IsTrue(expectedQ.IsEqual(q, 1e-6)); 
        }

        [Test]
        public void solve_for_diagonal()
        {
            double[][] value =
            {
               new double[] {  2, -1,  0 },
               new double[] { -1,  2, -1 },
               new double[] {  0, -1,  2 }
            };

            double[] b = { 1, 2, 3 };

            double[][] expected = 
            {
                new double[] { 0.75, 1, 0.75 },
                new double[] { 0.50, 2, 1.50 },
                new double[] { 0.25, 1, 2.25 }
            };

            var target = new JaggedQrDecomposition(value);
            double[][] actual = target.SolveForDiagonal(b);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-10));
            Assert.IsTrue(Matrix.IsEqual(value.Transpose(), target.Reverse(), 1e-6));
        }
    }
}
