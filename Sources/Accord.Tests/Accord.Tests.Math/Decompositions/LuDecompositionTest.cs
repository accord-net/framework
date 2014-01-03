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
    using Accord.Math.Decompositions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Math;
    using System;

    [TestClass()]
    public class LuDecompositionTest
    {

        private TestContext testContextInstance;

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
        public void SolveTest1()
        {
            double[,] value =
            {
               {  2,  3,  0 },
               { -1,  2,  1 },
               {  0, -1,  3 }
            };

            double[] rhs = { 5, 0, 1 };

            double[] expected =
            {
                1.6522,
                0.5652,
                0.5217,
            };

            LuDecomposition target = new LuDecomposition(value);

            double[] actual = target.Solve(rhs);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.001));
        }

        [TestMethod()]
        public void InverseTest()
        {
            double[,] value =
            {
               {  2,  3,  0 },
               { -1,  2,  1 },
               {  0, -1,  3 }
            };

            double[,] expectedInverse =
            {
                { 0.3043,   -0.3913,    0.1304 },
                { 0.1304,    0.2609,   -0.0870 },
                { 0.0435,    0.0870,    0.3043 },
            };

            LuDecomposition target = new LuDecomposition(value);

            double[,] actualInverse = target.Inverse();

            Assert.IsTrue(Matrix.IsEqual(expectedInverse, actualInverse, 0.001));

        }

        [TestMethod()]
        public void SolveTest()
        {
            double[,] value =
            {
               {  2,  3,  0 },
               { -1,  2,  1 },
               {  0, -1,  3 }
            };

            double[,] rhs =
            {
                { 1, 2, 3 },
                { 3, 2, 1 },
                { 5, 0, 1 },
            };

            double[,] expected =
            {
                { -0.2174,   -0.1739,    0.6522 },
                {  0.4783,    0.7826,    0.5652 },
                {  1.8261,    0.2609,    0.5217 },
            };

            LuDecomposition target = new LuDecomposition(value);

            double[,] actual = target.Solve(rhs);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.001));
        }

        [TestMethod()]
        public void SolveTest3()
        {
            double[,] value =
            {
               {  2.000,  3.000,  0.000 },
               { -1.000,  2.000,  1.000 },
            };

            LuDecomposition target = new LuDecomposition(value);

            double[,] L = target.LowerTriangularFactor;
            double[,] U = target.UpperTriangularFactor;

            double[,] expectedL = 
            {
               {  1.000, 0.000 },
               { -0.500, 1.000 },
            };

            double[,] expectedU = 
            {
                { 2.000, 3.000, 0.000 },
                { 0.000, 3.500, 1.000  },
            };


            Assert.IsTrue(Matrix.IsEqual(expectedL, L, 0.001));
            Assert.IsTrue(Matrix.IsEqual(expectedU, U, 0.001));
        }

        [TestMethod()]
        public void SolveTest4()
        {
            double[,] value =
            {
                { 2.1, 3.1 },
                { 1.6, 4.2 },
            };

            double[] rhs = {  6.1, 4.3 };

            double[] expected = {  3.1839, -0.1891 };

            LuDecomposition target = new LuDecomposition(value);

            double[] actual = target.Solve(rhs);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.001));
        }

        [TestMethod()]
        public void SolveTest5()
        {
            double[,] value =
            {
                { 2.1, 3.1 },
                { 1.6, 4.2 },
                { 2.1, 5.1 },
            };

            double[] rhs = { 6.1, 4.3, 2.1 };

            double[] expected = { 3.1839, -0.1891 };

            LuDecomposition target = new LuDecomposition(value);

            bool thrown = false;
            try
            {
                double[] actual = target.Solve(rhs);
            }
            catch (InvalidOperationException)
            {
                thrown = true;
            }

            Assert.IsTrue(thrown);
        }

        [TestMethod()]
        public void SolveTransposeTest()
        {
            double[,] a = 
            {
                { 2, 1, 4 },
                { 6, 2, 2 },
                { 0, 1, 6 },
            };

            double[,] b =
            {
                { 1, 0, 7 },
                { 5, 2, 1 },
                { 1, 5, 2 },
            };

            double[,] expected =
            {
                 { 0.5062,    0.2813,    0.0875 },
                 { 0.1375,    1.1875,   -0.0750 },
                 { 0.8063,   -0.2188,    0.2875 },
            };

            double[,] actual = new LuDecomposition(b, true).SolveTranspose(a);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.001));
        }

        [TestMethod()]
        public void LuDecompositionConstructorTest()
        {
            double[,] value =
            {
               {  2, -1,  0 },
               { -1,  2, -1 },
               {  0, -1,  2 }
            };


            double[,] expectedL =
            {
                {  1.0000,         0,         0 },
                { -0.5000,    1.0000,         0 },
                {       0,   -0.6667,    1.0000 },
            };


            double[,] expectedU =
            {
                { 2.0000,   -1.0000,         0 },
                {      0,    1.5000,   -1.0000 },
                {      0,         0,    1.3333 },
             };


            LuDecomposition target = new LuDecomposition(value);

            double[,] actualL = target.LowerTriangularFactor;
            double[,] actualU = target.UpperTriangularFactor;

            Assert.IsTrue(Matrix.IsEqual(expectedL, actualL, 0.001));
            Assert.IsTrue(Matrix.IsEqual(expectedU, actualU, 0.001));


            target = new LuDecomposition(value.Transpose(), true);

            actualL = target.LowerTriangularFactor;
            actualU = target.UpperTriangularFactor;

            Assert.IsTrue(Matrix.IsEqual(expectedL, actualL, 0.001));
            Assert.IsTrue(Matrix.IsEqual(expectedU, actualU, 0.001));
        }

        [TestMethod()]
        public void LogDeterminantTest()
        {
            LuDecomposition lu = new LuDecomposition(CholeskyDecompositionTest.bigmatrix);
            Assert.AreEqual(0, lu.Determinant);
            Assert.AreEqual(-2224.8931093738875, lu.LogDeterminant, 1e-12);
            Assert.IsTrue(lu.Nonsingular);
        }

        [TestMethod()]
        public void DeterminantTest()
        {
            double[,] value =
            {
               {  2,  3,  0 },
               { -1,  2,  1 },
               {  0, -1,  3 }
            };

            LuDecomposition lu = new LuDecomposition(value);
            Assert.AreEqual(23, lu.Determinant);
            Assert.IsTrue(lu.Nonsingular);
        }

        [TestMethod()]
        public void LogDeterminantTest2()
        {
            double[,] value =
            {
               {  2,  3,  0 },
               { -1,  2,  1 },
               {  0, -1,  3 }
            };

            LuDecomposition lu = new LuDecomposition(value);
            Assert.AreEqual(23, lu.Determinant);

            double expected = System.Math.Log(23);
            double actual = lu.LogDeterminant;

            Assert.AreEqual(expected, actual);
        }

    }
}
