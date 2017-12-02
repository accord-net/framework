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
    public class JaggedEigenvalueDecompositionTest
    {

        [Test]
        public void InverseTestNaN()
        {
            int n = 5;

            var I = Matrix.Identity(n);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    double[][] value = Matrix.JaggedMagic(n);

                    value[i][j] = double.NaN;

                    var target = new JaggedEigenvalueDecomposition(value);
                }
            }
        }

        [Test]
        public void EigenvalueDecompositionConstructorTest()
        {
            // Symmetric test
            double[][] A =
            {
                new double[] { 4, 2 },
                new double[] { 2, 4 }
            };

            var target = new JaggedEigenvalueDecomposition(A);

            var D = target.DiagonalMatrix;
            var Q = target.Eigenvectors;

            double[][] expectedD =
            { 
                new double[] { 2, 0 },
                new double[] { 0, 6 }
            };

            double[][] expectedQ = 
            {
               new double[] {  0.7071, 0.7071 },
               new double[] { -0.7071, 0.7071 }
            };


            Assert.IsTrue(Matrix.IsEqual(expectedD, D, 0.00001));
            Assert.IsTrue(Matrix.IsEqual(expectedQ, Q, 0.0001));


            // Decomposition identity
            var actualA = Matrix.Dot(Matrix.Dot(Q, D), Q.Inverse());

            Assert.IsTrue(Matrix.IsEqual(expectedD, D, 0.00001));
            Assert.IsTrue(Matrix.IsEqual(A, actualA, 0.0001));

            Assert.AreSame(target.DiagonalMatrix, target.DiagonalMatrix);
        }



        [Test]
        public void EigenvalueDecompositionConstructorTest2()
        {
            // Asymmetric test
            double[][] A =
            {
                new double[] {  5, 2, 1 },
                new double[] {  1, 4, 1 },
                new double[] { -1, 2, 3 }
            };

            var target = new JaggedEigenvalueDecomposition(A);
            var D = target.DiagonalMatrix;
            var Q = target.Eigenvectors;

            double[][] expectedD =
            { 
                new double[] { 6, 0, 0 },
                new double[] { 0, 4, 0 },
                new double[] { 0, 0, 2 }
            };

            // Decomposition identity
            var actualA = Matrix.Dot(Matrix.Dot(Q, D), Q.Inverse());

            Assert.IsTrue(Matrix.IsEqual(expectedD, D, 1e-5));
            Assert.IsTrue(Matrix.IsEqual(A, actualA, 1e-5));
        }
    }
}
