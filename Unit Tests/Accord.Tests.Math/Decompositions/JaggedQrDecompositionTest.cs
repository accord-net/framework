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
            Assert.IsTrue(Matrix.IsEqual(QQt, Matrix.JaggedIdentity(3), 0.0000001));


            // Linear system solving
            double[][] B = Matrix.ColumnVector(new double[] { 1, 2, 3 }).ToJagged();
            double[][] expected = Matrix.ColumnVector(new double[] { 2.5, 4.0, 3.5 }).ToJagged();
            double[][] actual = target.Solve(B);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0000000000001));
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
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0000000000001));

            target = new JaggedQrDecomposition(value.Transpose(), true);
            actual = target.Inverse();
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0000000000001));
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

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0000000000001));
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
        }
    }
}
