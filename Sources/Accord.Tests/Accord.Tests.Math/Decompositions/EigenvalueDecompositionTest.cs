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

using Accord.Math.Decompositions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accord.Math;
namespace Accord.Tests.Math
{


    /// <summary>
    ///This is a test class for EigenvalueDecompositionTest and is intended
    ///to contain all EigenvalueDecompositionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EigenvalueDecompositionTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }




        [TestMethod()]
        public void EigenvalueDecompositionConstructorTest()
        {
            // Symmetric test
            double[,] A =
            {
                { 4, 2 },
                { 2, 4 }
            };

            EigenvalueDecomposition target = new EigenvalueDecomposition(A);

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
            var actualA = Q.Multiply(D).Multiply(Q.Inverse());

            Assert.IsTrue(Matrix.IsEqual(expectedD, D, 0.00001));
            Assert.IsTrue(Matrix.IsEqual(A, actualA, 0.0001));

        }



        [TestMethod()]
        public void EigenvalueDecompositionConstructorTest2()
        {
            // Asymmetric test
            double[,] A =
            {
                {  5, 2, 1 },
                {  1, 4, 1 },
                { -1, 2, 3 }
            };

            EigenvalueDecomposition target = new EigenvalueDecomposition(A);
            var D = target.DiagonalMatrix;
            var Q = target.Eigenvectors;

            double[,] expectedD =
            { 
                { 6, 0, 0 },
                { 0, 4, 0 },
                { 0, 0, 2 }
            };

            // Decomposition identity
            var actualA = Q.Multiply(D).Multiply(Q.Inverse());

            Assert.IsTrue(Matrix.IsEqual(expectedD, D, 0.00001));
            Assert.IsTrue(Matrix.IsEqual(A, actualA, 0.0001));

        }
    }
}
