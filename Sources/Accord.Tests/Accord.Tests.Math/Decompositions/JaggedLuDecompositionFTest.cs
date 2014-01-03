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
    public class JaggedLuDecompositionTest
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

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.001));
        }

        [TestMethod()]
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

            Assert.IsTrue(Matrix.IsEqual(expectedInverse, actualInverse, 0.001));
        }

        [TestMethod()]
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

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.001));
        }

        [TestMethod()]
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


            Assert.IsTrue(Matrix.IsEqual(expectedL, L, 0.001));
            Assert.IsTrue(Matrix.IsEqual(expectedU, U, 0.001));
        }

        [TestMethod()]
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

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.001));
        }

        [TestMethod()]
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

        [TestMethod()]
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
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.001));
        }

        [TestMethod()]
        public void LuDecompositionConstructorTest()
        {
            float[][] value =
            {
               new float[] {  2, -1,  0 },
               new float[] { -1,  2, -1 },
               new float[] {  0, -1,  2 }
            };


            float[][] expectedL =
            {
                new float[] {  1.0000f,         0f,         0f },
                new float[] { -0.5000f,    1.0000f,         0f },
                new float[] {       0f,   -0.6667f,    1.0000f },
            };


            float[][] expectedU =
            {
                new float[] { 2.0000f,   -1.0000f,         0f },
                new float[] {      0f,    1.5000f,   -1.0000f },
                new float[] {      0f,         0f,    1.3333f },
             };


            JaggedLuDecompositionF target = new JaggedLuDecompositionF(value);

            float[][] actualL = target.LowerTriangularFactor;
            float[][] actualU = target.UpperTriangularFactor;

            Assert.IsTrue(Matrix.IsEqual(expectedL, actualL, 0.001));
            Assert.IsTrue(Matrix.IsEqual(expectedU, actualU, 0.001));


            target = new JaggedLuDecompositionF(value.Transpose(), true);

            actualL = target.LowerTriangularFactor;
            actualU = target.UpperTriangularFactor;

            Assert.IsTrue(Matrix.IsEqual(expectedL, actualL, 0.001));
            Assert.IsTrue(Matrix.IsEqual(expectedU, actualU, 0.001));
        }

        [TestMethod()]
        public void LogDeterminantTest()
        {
            JaggedLuDecompositionF lu = new JaggedLuDecompositionF(
                CholeskyDecompositionTest.bigmatrix.ToSingle().ToArray());
            Assert.AreEqual(0, lu.Determinant);
            Assert.AreEqual(-2224.8931093738875, lu.LogDeterminant, 1e-3);
            Assert.IsTrue(lu.Nonsingular);
        }

        [TestMethod()]
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

        [TestMethod()]
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

    }
}
