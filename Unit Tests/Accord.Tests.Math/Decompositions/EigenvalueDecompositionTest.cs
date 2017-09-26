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
    public class EigenvalueDecompositionTest
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
                    double[,] value = Matrix.Magic(n);

                    value[i, j] = double.NaN;

                    var target = new EigenvalueDecomposition(value);
                }
            }
        }

        [Test]
        public void EigenvalueDecompositionConstructorTest()
        {
            // Symmetric test
            double[,] A =
            {
                { 4, 2 },
                { 2, 4 }
            };

            var target = new EigenvalueDecomposition(A);

            var D = target.DiagonalMatrix;
            var Q = target.Eigenvectors;

            double[,] expectedD =
            {
                { 2, 0 },
                { 0, 6 }
            };

            double[,] expectedQ =
            {
               {  0.7071, 0.7071 },
               { -0.7071, 0.7071 }
            };


            Assert.IsTrue(Matrix.IsEqual(expectedD, D, 0.00001));
            Assert.IsTrue(Matrix.IsEqual(expectedQ, Q, 0.0001));


            // Decomposition identity
            var inv = Q.Inverse();
            var actualA = Matrix.Dot(Matrix.Dot(Q, D), inv);


            Assert.IsTrue(Matrix.IsEqual(expectedD, D, 0.00001));
            Assert.IsTrue(Matrix.IsEqual(A, actualA, 0.0001));

            Assert.AreSame(target.DiagonalMatrix, target.DiagonalMatrix);
        }



        [Test]
        public void EigenvalueDecompositionConstructorTest2()
        {
            // Asymmetric test
            double[,] A =
            {
                {  5, 2, 1 },
                {  1, 4, 1 },
                { -1, 2, 3 }
            };

            var target = new EigenvalueDecomposition(A);
            var D = target.DiagonalMatrix;
            var Q = target.Eigenvectors;

            double[,] expectedD =
            {
                { 6, 0, 0 },
                { 0, 4, 0 },
                { 0, 0, 2 }
            };

            // Decomposition identity
            var actualA = Matrix.Dot(Matrix.Dot(Q, D), Q.Inverse());

            Assert.IsTrue(Matrix.IsEqual(expectedD, D, 1e-5));
            Assert.IsTrue(Matrix.IsEqual(A, actualA, 1e-5));
            //var actual = target.Reverse();
            //Assert.IsTrue(Matrix.IsEqual(A, actual, 1e-5));
        }
    }
}
