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

    [TestClass()]
    public class SingularValueDecompositionTest
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
        public void InverseTest()
        {
            double[,] value = new double[,]
            { 
                  { 1.0, 1.0 },
                  { 2.0, 2.0 }
            };

            SingularValueDecomposition target = new SingularValueDecomposition(value);

            double[,] expected = new double[,]
            {
               { 0.1, 0.2 },
               { 0.1, 0.2 }
            };

            double[,] actual = target.Solve(Matrix.Identity(2));
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.001));

            actual = target.Inverse();
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.001));
        }

        [TestMethod()]
        public void SingularValueDecompositionConstructorTest1()
        {
            // This test catches the bug in SingularValueDecomposition in the line
            //   for (int j = k + 1; j < nu; j++)
            // where it should be
            //   for (int j = k + 1; j < n; j++)


            // Test for m-x-n matrices where m < n. The available SVD
            // routine was not meant to be used in this case.

            double[,] value = new double[,]
             { 
                 { 1, 2 },
                 { 3, 4 },
                 { 5, 6 },
                 { 7, 8 }
             }.Transpose(); // value is 2x4, having less rows than columns.

            SingularValueDecomposition target = new SingularValueDecomposition(value, true, true, false);

            double[,] actual = target.LeftSingularVectors.Multiply(
                Matrix.Diagonal(target.Diagonal)).Multiply(target.RightSingularVectors.Transpose());

            // Checking the decomposition
            Assert.IsTrue(Matrix.IsEqual(actual, value, 0.01));

            // Checking values
            double[,] U =
            {
                { -0.641423027995072, -0.767187395072177 },
                { -0.767187395072177,  0.641423027995072 },
            };

            // U should be equal
            Assert.IsTrue(Matrix.IsEqual(target.LeftSingularVectors, U, 0.001));


            double[,] V = // economy svd
            {
                { -0.152483233310201,  0.822647472225661,  },
                { -0.349918371807964,  0.421375287684580,  },
                { -0.547353510305727,  0.0201031031435023, },
                { -0.744788648803490, -0.381169081397574,  },
            };

            // V can be different, but for the economy SVD it is often equal
            Assert.IsTrue(Matrix.IsEqual(target.RightSingularVectors.Submatrix(0, 3, 0, 1), V, 0.0001));


            double[,] S = 
            {
                { 14.2690954992615, 0.000000000000000 },
                {  0.0000000000000,	0.626828232417543 },
            };

            // The diagonal values should be equal
            Assert.IsTrue(Matrix.IsEqual(target.Diagonal.Submatrix(2), Matrix.Diagonal(S), 0.001));
        }


        [TestMethod()]
        public void SingularValueDecompositionConstructorTest3()
        {
            // Test using SVD assumption auto-correction feature.

            // Test for m-x-n matrices where m < n. The available SVD
            // routine was not meant to be used in this case.

            double[,] value = new double[,]
             { 
                 { 1, 2 },
                 { 3, 4 },
                 { 5, 6 },
                 { 7, 8 }
             }.Transpose(); // value is 2x4, having less rows than columns.

            SingularValueDecomposition target = new SingularValueDecomposition(value, true, true, true);

            double[,] actual = target.LeftSingularVectors.Multiply(
                Matrix.Diagonal(target.Diagonal)).Multiply(target.RightSingularVectors.Transpose());

            // Checking the decomposition
            Assert.IsTrue(Matrix.IsEqual(actual, value, 0.01));

            // Checking values
            double[,] U =
            {
                { 0.641423027995072, -0.767187395072177 },
                { 0.767187395072177,  0.641423027995072 },
            };

            // U should be equal despite some sign changes
            Assert.IsTrue(Matrix.IsEqual(target.LeftSingularVectors, U, 0.001));


            double[,] V = // economy svd
            {
                {  0.152483233310201,  0.822647472225661,  },
                {  0.349918371807964,  0.421375287684580,  },
                {  0.547353510305727,  0.0201031031435023, },
                {  0.744788648803490, -0.381169081397574,  },
            };

            // V can be different, but for the economy SVD it is often equal
            Assert.IsTrue(Matrix.IsEqual(target.RightSingularVectors, V, 0.0001));


            double[,] S = 
            {
                { 14.2690954992615, 0.000000000000000 },
                {  0.0000000000000,	0.626828232417543 },
            };

            // The diagonal values should be equal
            Assert.IsTrue(Matrix.IsEqual(target.Diagonal, Matrix.Diagonal(S), 0.001));
        }


        [TestMethod()]
        public void SingularValueDecompositionConstructorTest2()
        {
            // test for m-x-n matrices where m > n (4 > 2)

            double[,] value = new double[,]
             { 
                 { 1, 2 },
                 { 3, 4 },
                 { 5, 6 },
                 { 7, 8 }
             }; // value is 4x2, thus having more rows than columns


            SingularValueDecomposition target = new SingularValueDecomposition(value, true, true, false);

            double[,] actual = target.LeftSingularVectors.Multiply(
                Matrix.Diagonal(target.Diagonal)).Multiply(target.RightSingularVectors.Transpose());

            // Checking the decomposition
            Assert.IsTrue(Matrix.IsEqual(actual, value, 0.01));

            double[,] U = // economy svd
            {
                {  0.152483233310201,  0.822647472225661,  },
                {  0.349918371807964,  0.421375287684580,  },
                {  0.547353510305727,  0.0201031031435023, },
                {  0.744788648803490, -0.381169081397574,  },
            };

            // U should be equal except for some sign changes
            Assert.IsTrue(Matrix.IsEqual(target.LeftSingularVectors, U, 0.001));



            // Checking values
            double[,] V =
            {
                {  0.641423027995072, -0.767187395072177 },
                {  0.767187395072177,  0.641423027995072 },
            };

            // V should be equal except for some sign changes
            Assert.IsTrue(Matrix.IsEqual(target.RightSingularVectors, V, 0.0001));


            double[,] S = 
            {
                { 14.2690954992615, 0.000000000000000 },
                {  0.0000000000000,	0.626828232417543 },
            };

            // The diagonal values should be equal
            Assert.IsTrue(Matrix.IsEqual(target.Diagonal, Matrix.Diagonal(S), 0.001));
        }


        [TestMethod()]
        public void SingularValueDecompositionConstructorTest4()
        {
            // Test using SVD assumption auto-correction feature
            // without computing the right singular vectors.

            double[,] value = new double[,]
             { 
                 { 1, 2 },
                 { 3, 4 },
                 { 5, 6 },
                 { 7, 8 }
             }.Transpose(); // value is 2x4, having less rows than columns.

            SingularValueDecomposition target = new SingularValueDecomposition(value, true, false, true);


            // Checking values
            double[,] U =
            {
                { 0.641423027995072, -0.767187395072177 },
                { 0.767187395072177,  0.641423027995072 },
            };

            // U should be equal despite some sign changes
            Assert.IsTrue(Matrix.IsEqual(target.LeftSingularVectors, U, 0.001));


            // Checking values
            double[,] V =
            {
                {  0.0, 0.0 },
                {  0.0, 0.0 },
                {  0.0, 0.0 },
                {  0.0, 0.0 },
            };

            // V should not have been computed.
            Assert.IsTrue(Matrix.IsEqual(target.RightSingularVectors, V));


            double[,] S = 
            {
                { 14.2690954992615, 0.000000000000000 },
                {  0.0000000000000,	0.626828232417543 },
            };

            // The diagonal values should be equal
            Assert.IsTrue(Matrix.IsEqual(target.Diagonal, Matrix.Diagonal(S), 0.001));
        }

        [TestMethod()]
        public void SingularValueDecompositionConstructorTest5()
        {
            // Test using SVD assumption auto-correction feature
            // without computing the left singular vectors.

            double[,] value = new double[,]
             { 
                 { 1, 2 },
                 { 3, 4 },
                 { 5, 6 },
                 { 7, 8 }
             }.Transpose(); // value is 2x4, having less rows than columns.

            SingularValueDecomposition target = new SingularValueDecomposition(value, false, true, true);


            // Checking values
            double[,] U =
            {
                { 0.0, 0.0 },
                { 0.0, 0.0 },
            };

            // U should not have been computed
            Assert.IsTrue(Matrix.IsEqual(target.LeftSingularVectors, U));


            double[,] V = // economy svd
            {
                {  0.152483233310201,  0.822647472225661,  },
                {  0.349918371807964,  0.421375287684580,  },
                {  0.547353510305727,  0.0201031031435023, },
                {  0.744788648803490, -0.381169081397574,  },
            };

            // V can be different, but for the economy SVD it is often equal
            Assert.IsTrue(Matrix.IsEqual(target.RightSingularVectors, V, 0.0001));



            double[,] S = 
            {
                { 14.2690954992615, 0.000000000000000 },
                {  0.0000000000000,	0.626828232417543 },
            };

            // The diagonal values should be equal
            Assert.IsTrue(Matrix.IsEqual(target.Diagonal, Matrix.Diagonal(S), 0.001));
        }


        [TestMethod()]
        public void SingularValueDecompositionConstructorTest6()
        {
            // Test using SVD assumption auto-correction feature in place

            double[,] value1 =
            {
                { 2.5,  2.4 },
                { 0.5,  0.7 },
                { 2.2,  2.9 },
                { 1.9,  2.2 },
                { 3.1,  3.0 },
                { 2.3,  2.7 },
                { 2.0,  1.6 },
                { 1.0,  1.1 },
                { 1.5,  1.6 },
                { 1.1,  0.9 }
            };

            double[,] value2 = value1.Transpose();

            var target1 = new SingularValueDecomposition(value1, true, true, true, true);
            var target2 = new SingularValueDecomposition(value2, true, true, true, true);

            Assert.IsTrue(target1.LeftSingularVectors.IsEqual(target2.RightSingularVectors));
            Assert.IsTrue(target1.RightSingularVectors.IsEqual(target2.LeftSingularVectors));
            Assert.IsTrue(target1.DiagonalMatrix.IsEqual(target2.DiagonalMatrix));

        }

        [TestMethod()]
        public void SingularValueDecompositionConstructorTest7()
        {
            int count = 100;
            double[,] value = new double[count, 3];
            double[] output = new double[count];

            for (int i = 0; i < count; i++)
            {
                double x = i + 1;
                double y = 2 * (i + 1) - 1;
                value[i, 0] = x;
                value[i, 1] = y;
                value[i, 2] = 1;
                output[i] = 4 * x - y + 3;
            }



            SingularValueDecomposition target = new SingularValueDecomposition(value,
                computeLeftSingularVectors: true,
                computeRightSingularVectors: true);

            {
                double[,] expected = value;
                double[,] actual = target.LeftSingularVectors.Multiply(
                    Matrix.Diagonal(target.Diagonal)).Multiply(target.RightSingularVectors.Transpose());

                // Checking the decomposition
                Assert.IsTrue(Matrix.IsEqual(actual, expected, 1e-8));
            }

            {
                double[] solution = target.Solve(output);

                double[] expected= output;
                double[] actual = value.Multiply(solution);

                Assert.IsTrue(Matrix.IsEqual(actual, expected, 1e-8));
            }
        }
    }
}
