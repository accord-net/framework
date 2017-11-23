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
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Accord.Math;
    using Accord.Math.Decompositions;
    using AForge;
    using NUnit.Framework;
    using Accord.IO;

    [TestFixture]
    public partial class MatrixTest
    {

        #region Comparison
        [Test]
        public void IsEqualTest1()
        {
            double[,] a =
            {
                { 0.2 },
            };

            double[,] b =
            {
                { Double.NaN },
            };

            double threshold = 0.2;
            bool expected = false;
            bool actual = Matrix.IsEqual(a, b, threshold);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsEqualTest2()
        {
            double[,] a =
            {
                { 0.2,  0.1, 0.0                     },
                { 0.2, -0.5, Double.NaN              },
                { 0.2, -0.1, Double.NegativeInfinity },
            };

            double[,] b =
            {
                { 0.23,  0.1,  0.0                     },
                { 0.21, -0.5,  Double.NaN              },
                { 0.19, -0.11, Double.NegativeInfinity },
            };

            double threshold = 0.03;
            bool expected = true;
            bool actual = Matrix.IsEqual(a, b, atol: threshold);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsEqualTest3()
        {
            double[] a = { 1, 1, 1 };
            double x = 1;
            bool expected = true;
            bool actual;

            actual = Matrix.IsEqual(a, x);
            Assert.AreEqual(expected, actual);

            actual = a.IsEqual(x);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsEqualTest()
        {
            double[,] matrix = Matrix.Create(2, 2, 1.0);

            bool actual;

            actual = Matrix.IsEqual(matrix, 1.0);
            Assert.AreEqual(true, actual);

            actual = Matrix.IsEqual(matrix, 0.0);
            Assert.AreEqual(false, actual);
        }

        [Test]
        public void isequal_objects_small()
        {
            object[,] expectedTable =
            {
                { "Label",                 "6" },
                { "Total",                2025 },
            };

            object[,] differentTable1 =
            {
                { "Label",                 "6"  },
                { "Total",          /*m*/    1  },
            };

            Assert.IsFalse(expectedTable.IsEqual(differentTable1, atol: 1e-10));
        }

        [Test]
        public void isequal_objects_null()
        {
            object[,] expectedTable =
            {
                { "Label",                 "6" },
                { "Total",                null },
            };

            object[,] differentTable1 =
            {
                { "Label",                 "6"  },
                { "Total",          /*m*/    0  },
            };

            Assert.IsFalse(expectedTable.IsEqual(differentTable1, atol: 1e-10));
        }

        #endregion

        #region Matrix and vector creation
        [Test]
        public void ColumnVectorTest()
        {
            double[] values = { 1, 2, 3 };
            double[,] expected = {
                                   { 1 },
                                   { 2 },
                                   { 3 }
                                 };
            double[,] actual;
            actual = Matrix.ColumnVector(values);
            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void RowVectorTest()
        {
            double[] values = { 1, 2, 3 };
            double[,] expected = {
                                    { 1, 2, 3 },
                                 };
            double[,] actual;
            actual = Matrix.RowVector(values);
            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void DiagonalTest()
        {
            int rows = 2;
            int cols = 3;

            double value = -4.2;
            double[,] expected =
            {
                { -4.2,  0.0, 0.0 },
                {  0.0, -4.2, 0.0 }
            };

            double[,] actual = Matrix.Diagonal(rows, cols, value);

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void DiagonalTest2()
        {
            int rows = 3;
            int cols = 2;

            double value = -4.2;
            double[,] expected =
            {
                { -4.2,  0.0 },
                {  0.0, -4.2 },
                {  0.0,  0.0 }
            };

            double[,] actual = Matrix.Diagonal(rows, cols, value);

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void IntervalTest2()
        {
            double from = 0;
            double to = 10;
            int steps = 11;
            double[] expected = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            double[] actual = Matrix.Interval(from, to, steps);

            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void IntervalTest3()
        {
            double[] actual;

            actual = Matrix.Interval(0.0, 0.0, 1);
            Assert.AreEqual(0, actual[0]);

            actual = Matrix.Interval(0.0, 0.0, 5);
            Assert.IsTrue(actual.All(x => x == 0));
            Assert.AreEqual(actual.Length, 5);
        }

        [Test]
        public void IntervalTest4()
        {
            double[] actual;

            actual = Matrix.Interval(0.0, 0.0, 1.0);
            Assert.AreEqual(0, actual[0]);

            actual = Matrix.Interval(0.0, 0.0, 5.0);
            Assert.AreEqual(0, actual[0]);
            Assert.AreEqual(actual.Length, 1);
        }

        [Test]
        public void IntervalTest2Inverse()
        {
            double from = 10;
            double to = 0;
            int steps = 11;
            double[] expected = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            double[] actual = Matrix.Interval(from, to, steps);

            Assert.IsTrue(Matrix.IsEqual(expected.Reverse().ToArray(), actual));
        }

        [Test]
        public void IntervalTest1()
        {
            int from = -2;
            int to = 4;
            int[] expected = { -2, -1, 0, 1, 2, 3, 4 };
            int[] actual = Matrix.Interval(from, to);

            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void IntervalTest()
        {
            double from = -1;
            double to = 1;
            double stepSize = 0.2;
            double[] expected = { -1.0, -0.8, -0.6, -0.4, -0.2, 0.0, 0.2, 0.4, 0.6, 0.8, 1.0 };
            double[] actual = Matrix.Interval(from, to, stepSize);

            Assert.IsTrue(Matrix.IsEqual(expected, Matrix.Round(actual, 15)));
        }

        [Test]
        public void IntervalTest_float()
        {
            float from = -1;
            float to = 1;
            float stepSize = 0.2f;
            float[] expected = { -1.0f, -0.8f, -0.6f, -0.4f, -0.2f, 0.0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f };
            float[] actual = Matrix.Interval(from, to, stepSize);
            float[] round = Matrix.Round(actual, 3);
            Assert.IsTrue(Matrix.IsEqual(expected, round));
        }

        [Test]
        public void IntervalTestInverse()
        {
            double from = 1;
            double to = -1;
            double stepSize = 0.2;
            double[] expected = { -1.0, -0.8, -0.6, -0.4, -0.2, 0.0, 0.2, 0.4, 0.6, 0.8, 1.0 };
            double[] actual = Matrix.Interval(from, to, stepSize);

            Assert.IsTrue(Matrix.IsEqual(expected.Reverse().ToArray(), Matrix.Round(actual, 15)));
        }

        [Test]
        public void IndexesTest()
        {
            int from = -1;
            int to = 6;
            int[] expected = { -1, 0, 1, 2, 3, 4, 5 };
            int[] actual = Matrix.Indices(from, to);
            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void IndexesTestInverse()
        {
            int from = 6;
            int to = -1;
            int[] expected = { 5, 4, 3, 2, 1, 0, -1 };
            int[] actual = Matrix.Indices(from, to);
            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }
        #endregion

        #region Elementwise Operations
        [Test]
        public void ElementwiseMultiplyTest()
        {
            double[] a = { 5, 2, 1 };
            double[] b = { 5, 1, 2 };
            double[] expected = { 25, 2, 2 };
            double[] actual = Matrix.ElementwiseMultiply(a, b);
            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void ElementwiseDivideTest2()
        {
            double[] a = { 5, 2, 1 };
            double[] b = { 5, 1, 2 };
            double[] expected = { 1, 2, 0.5 };
            double[] actual = Matrix.ElementwiseDivide(a, b);
            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void ElementwiseDivideTest1()
        {
            double[,] a =
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
            };

            double[,] b =
            {
                { 2, 1, 0.5 },
                { 2, 5, 3.0 },
            };

            double[,] expected =
            {
                { 0.5, 2, 6 },
                { 2, 1, 2 },
            };

            double[,] actual = Matrix.ElementwiseDivide(a, b);
            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void ElementwiseDivideTest()
        {
            double[,] a =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            double[] b = { 1, 2 };
            int dimension = 1;
            double[,] expected =
            {
                { 1, 2, 3 },
                { 2, 2.5, 3},
            };


            double[,] actual = Matrix.ElementwiseDivide(a, b, dimension);
            Assert.IsTrue(Matrix.IsEqual(expected, actual));

            b = new double[] { 1, 2, 3 };
            dimension = 0;
            expected = new double[,]
            {
                { 1, 1, 1 },
                { 4, 2.5, 2},
            };

            actual = Matrix.ElementwiseDivide(a, b, dimension);
            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void DivideTest1()
        {
            double scalar = -2;
            double[,] matrix =
            {
                { 4.2,  1.2, -0.6 },
                { 1.2, -0.6,  0.8 },
            };
            double[,] expected =
            {
                { -2/ 4.2, -2/ 1.2, -2/-0.6 },
                { -2/ 1.2, -2/-0.6, -2/ 0.8 },
            };

            double[,] actual = Elementwise.Divide(scalar, matrix);

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void DivideTest4()
        {
            float[] vector = { 4.2f, 1.2f };
            float x = 2;
            float[] expected = { 2.1f, 0.6f };
            float[] actual = Elementwise.Divide(vector, x);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void ElementwiseMultiplyTest1()
        {
            double[,] a =
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
            };

            double[,] b =
            {
                { 2, 1, 0.5 },
                { 2, 5, 3.0 },
            };

            double[,] expected =
            {
                { 2, 2, 1.5 },
                { 8, 25, 18 },
            };

            double[,] actual = Elementwise.Multiply(a, b);
            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void ElementwisePowerTest()
        {
            double[] x = { 1, 2, 3 };
            double y = 2;
            double[] expected = { 1, 4, 9 };
            double[] actual = Elementwise.Pow(x, y);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void ElementwiseMultiplyTest3()
        {
            double[] a = { 0.20, 1.65 };
            double[] b = { -0.72, 0.00 };
            double[] expected = { -0.1440, 0 };
            double[] actual = Elementwise.Multiply(a, b);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void ElementwiseMultiplyTest2()
        {
            int[,] a =
            {
                { 1, -5 },
                { 2,  0 },
            };

            int[,] b =
            {
                { -6, -5 },
                {  2,  9 },
            };

            int[,] expected =
            {
                { -6, 25 },
                {  4,  0 },
            };

            int[,] actual = Elementwise.Multiply(a, b);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void AddTest()
        {
            double[,] a =
            {
                { 2, 5, -1 },
                { 5, 0,  2 },
            };

            double[,] b =
            {
                {  1, 5, 1 },
                { -5, 2, 2 },
            };

            double[,] expected =
            {
                {  3, 10, 0 },
                {  0,  2, 4 },
            };

            double[,] actual = Elementwise.Add(a, b);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void AddTest1()
        {
            double[][] a =
            {
                new double[] { 2, 5, -1 },
                new double[] { 5, 0,  2 },
            };

            double[][] b =
            {
                new double[] {  1, 5, 1 },
                new double[] { -5, 2, 2 },
            };

            double[][] expected =
            {
                new double[] {  3, 10, 0 },
                new double[] {  0,  2, 4 },
            };

            double[][] actual = Elementwise.Add(a, b);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void AddToDiagTest1()
        {
            double[,] a =
            {
                { 2, 5, -1 },
                { 5, 0,  2 },
            };

            double[,] expected =
            {
                {  3, 5, -1 },
                {  5, 1,  2 },
            };

            double[,] actual = Matrix.AddToDiagonal(a, 1.0);
            Assert.IsTrue(expected.IsEqual(actual));

            actual = Elementwise.SubtractFromDiagonal(actual, 1.0);
            Assert.IsTrue(actual.IsEqual(a));
        }

        [Test]
        public void AddToDiagTest2()
        {
            double[][] a =
            {
                new double[] { 2, 5, -1 },
                new double[] { 5, 0,  2 },
            };

            double[][] expected =
            {
                new double[] {  3, 5, -1 },
                new double[] {  5, 1,  2 },
            };

            var actual = Matrix.AddToDiagonal(a, 1.0);
            Assert.IsTrue(expected.IsEqual(actual));

            actual = Matrix.SubtractFromDiagonal(actual, 1.0);
            Assert.IsTrue(actual.IsEqual(a));
        }

        [Test]
        public void ElementwiseMultiplyTest4()
        {
            double[,] a =
            {
                { 1,  5, 1 },
                { 0, -2, 1 },
            };
            double[] b = { 1, 2 };
            int dimension = 1;

            double[,] expected =
            {
                { 1,  5, 1 },
                { 0, -4, 2 },
            };

            double[,] actual = Matrix.ElementwiseMultiply(a, b, dimension);
            Assert.IsTrue(expected.IsEqual(actual));


            b = new double[] { 4, 1, 2 };
            dimension = 0;

            expected = new double[,]
            {
                { 4,  5, 2 },
                { 0, -2, 2 },
            };

            actual = Matrix.ElementwiseMultiply(a, b, dimension);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void SubtractTest()
        {
            double[,] a =
            {
                { 2, 5, -1 },
                { 5, 0,  2 },
            };

            double[,] b =
            {
                {  1, 5, 1 },
                { -5, 2, 2 },
            };

            double[,] expected =
            {
                {  1,  0, -2 },
                { 10, -2, 0 },
            };

            double[,] actual = Elementwise.Subtract(a, b);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void ElementwiseMultiplyTest5()
        {
            int[] a = { 2, 1, 6, 1 };
            int[] b = { 5, 1, 2, 0 };

            int[] expected = { 10, 1, 12, 0 };
            int[] actual = Matrix.ElementwiseMultiply(a, b);

            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void AddMatrixAndVectorTest()
        {
            double[,] a = Matrix.Create(3, 5, 0.0);
            double[] v = { 1, 2, 3, 4, 5 };
            double[,] actual;


            double[,] expected =
            {
                { 1, 2, 3, 4, 5 },
                { 1, 2, 3, 4, 5 },
                { 1, 2, 3, 4, 5 },
            };

            actual = Matrix.Add(a, v, 0); // Add to rows
            Assert.IsTrue(Matrix.IsEqual(expected, actual));


            double[,] b = Matrix.Create(5, 4, 0.0);
            double[] u = { 1, 2, 3, 4, 5 };

            double[,] expected2 =
            {
                { 1, 1, 1, 1, },
                { 2, 2, 2, 2, },
                { 3, 3, 3, 3, },
                { 4, 4, 4, 4, },
                { 5, 5, 5, 5, },
            };

            actual = Matrix.Add(b, u, 1); // Add to columns
            Assert.IsTrue(Matrix.IsEqual(expected2, actual));

        }

        [Test]
        public void SubtractTest1()
        {
            double a = 0.1;
            double[,] b =
            {
                { 0.2, 0.5, 0.2 },
                { 0.2, 0.7, 0.1 },
            };

            double[,] expected =
            {
                { -0.1, -0.4, -0.1 },
                { -0.1, -0.6, -0.0 },
            };

            double[,] actual = Elementwise.Subtract(a, b);

            Assert.IsTrue(expected.IsEqual(actual, 1e-6));
        }

        [Test]
        public void AbsTest()
        {
            double[,] value =
            {
                { -1.0,  2.1 },
                {  0.1, -0.7 }
            };

            double[,] expected =
            {
                {  1.0,  2.1 },
                {  0.1,  0.7 }
            };

            double[,] actual = Matrix.Abs(value);
            Assert.IsTrue(actual.IsEqual(expected));


            int[,] ivalue =
            {
                { -1,  2 },
                {  0, -6 }
            };

            int[,] iexpected =
            {
                {  1,  2 },
                {  0,  6 }
            };

            int[,] iactual = Matrix.Abs(ivalue);
            Assert.IsTrue(iactual.IsEqual(iexpected));

            int[] avalue = { -1, 2 };
            int[] aexpected = { 1, 2 };

            int[] aactual = Matrix.Abs(avalue);
            Assert.IsTrue(aactual.IsEqual(aexpected));
        }

        [Test]
        public void SqrtTest()
        {
            double[,] value =
            {
                { 3,  2 },
                { 1, -2 },
            };

            double[,] expected =
            {
                { 1.7321,  Accord.Math.Constants.Sqrt2 },
                { 1.0000, Double.NaN },
            };

            double[,] actual = Matrix.Sqrt(value);
            Assert.IsTrue(expected.IsEqual(actual, 0.0001));
        }


        [Test]
        public void gh_927()
        {
            // https://github.com/accord-net/framework/issues/927
            int rows = 100;
            double[,] matrix = Matrix.Zeros(rows: rows, columns: 3);
            double[] vec = { 1, 2, 3 }; // this is a row-vector with the same length as the number of columns

            double[,] broadcasted1 = matrix.Add(vec, dimension: 0);
            double[,] broadcasted2 = matrix.Add(vec, dimension: VectorType.RowVector);

#if DEBUG
            Assert.Throws<DimensionMismatchException>(() => matrix.Add(vec, dimension: 1));
            Assert.Throws<DimensionMismatchException>(() => matrix.Add(vec, dimension: VectorType.ColumnVector));
#else
            Assert.Throws<IndexOutOfRangeException>(() => matrix.Add(vec, dimension: 1));
            Assert.Throws<IndexOutOfRangeException>(() => matrix.Add(vec, dimension: VectorType.ColumnVector));
#endif

            for (int i = 0; i < rows; i++)
            {
                Assert.AreEqual(vec, broadcasted1.GetRow(i));
                Assert.AreEqual(vec, broadcasted2.GetRow(i));
            }
        }
        #endregion

        #region Conversions
        [Test]
        public void ToMatrixTest()
        {
            double[] array = { 1, 5, 2 };
            double[,] expected = { { 1, 5, 2 } };
            double[,] actual = Matrix.ToMatrix(array);
            Assert.IsTrue(expected.IsEqual(actual));
        }

#if !NO_DATA_TABLE
        [Test]
        public void ToArrayTest()
        {
            DataTable table = new DataTable("myData");
            table.Columns.Add("Double", typeof(double));
            table.Columns.Add("Integer", typeof(int));
            table.Columns.Add("Boolean", typeof(bool));

            table.Rows.Add(4.20, 42, true);
            table.Rows.Add(-3.14, -17, false);
            table.Rows.Add(21.00, 0, false);

            double[] expected;
            double[] actual;

            expected = new double[] { 4.20, -3.14, 21 };
            actual = table.Columns["Double"].ToArray();
            Assert.IsTrue(expected.IsEqual(actual));

            expected = new double[] { 42, -17, 0 };
            actual = table.Columns["Integer"].ToArray();
            Assert.IsTrue(expected.IsEqual(actual));

            expected = new double[] { 1, 0, 0 };
            actual = table.Columns["Boolean"].ToArray();
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void ToArrayTest1()
        {
            DataTable table = new DataTable("myData");
            table.Columns.Add("Double", typeof(double));
            table.Columns.Add("Integer", typeof(int));
            table.Columns.Add("Boolean", typeof(bool));

            table.Rows.Add(4.20, 42, true);
            table.Rows.Add(-3.14, -17, false);
            table.Rows.Add(21.00, 0, false);

            double[][] expected =
            {
                new double[] {  4.20,  42, 1 },
                new double[] { -3.14, -17, 0 },
                new double[] { 21.00,   0, 0 },
            };

            double[][] actual = table.ToArray();

            Assert.IsTrue(expected.IsEqual(actual));


            string[] expectedNames = { "Double", "Integer", "Boolean" };
            string[] actualNames;

            table.ToArray(out actualNames);

            Assert.IsTrue(expectedNames.IsEqual(actualNames));
        }

        [Test]
        public void ToMatrixTest2()
        {
            DataTable table = new DataTable("myData");
            table.Columns.Add("Double", typeof(double));
            table.Columns.Add("Integer", typeof(int));
            table.Columns.Add("Boolean", typeof(bool));

            table.Rows.Add(4.20, 42, true);
            table.Rows.Add(-3.14, -17, false);
            table.Rows.Add(21.00, 0, false);

            double[,] expected =
            {
                {  4.20,  42, 1 },
                { -3.14, -17, 0 },
                { 21.00,   0, 0 },
            };

            double[,] actual = table.ToMatrix();

            Assert.IsTrue(expected.IsEqual(actual));


            string[] expectedNames = { "Double", "Integer", "Boolean" };
            string[] actualNames;

            table.ToMatrix(out actualNames);

            Assert.IsTrue(expectedNames.IsEqual(actualNames));
        }
#endif

        [Test]
        public void ToDoubleTest()
        {
            float[,] matrix =
            {
                { 2.1f,  5.2f },
                { 0.1f, -0.2f }
            };

            double[,] expected =
            {
                { 2.1,  5.2 },
                { 0.1, -0.2 }
            };

            double[,] actual = Matrix.ToDouble(matrix);
            Assert.IsTrue(expected.IsEqual(actual, 1e-6));
        }
        #endregion

        #region Sum and Product
        [Test]
        public void ProductsTest1()
        {
            double[] u = { 1, 6, 3 };
            double[] v = { 9, 4, 2 };

            // products
            double inner = u.InnerProduct(v);    // 39.0
            double[,] outer = u.OuterProduct(v); // see below
            double[] kronecker = u.KroneckerProduct(v); // { 9, 4, 2, 54, 24, 12, 27, 12, 6 }
            double[][] cartesian = u.CartesianProduct(v); // all possible pair-wise combinations

            Assert.AreEqual(39, inner);
            Assert.IsTrue(new double[,]
            {
               {  9,  4,  2 },
               { 54, 24, 12 },
               { 27, 12,  6 },
            }.IsEqual(outer));

            Assert.IsTrue(new double[] { 9, 4, 2, 54, 24, 12, 27, 12, 6 }
                .IsEqual(kronecker));


            // addition
            double[] addv = u.Add(v); // { 10, 10, 5 }
            double[] add5 = u.Add(5); // {  6, 11, 8 }

            Assert.IsTrue(addv.IsEqual(10, 10, 5));
            Assert.IsTrue(add5.IsEqual(6, 11, 8));

            double[] abs = u.Abs();   // { 1, 6, 3 }
            double[] log = u.Log();   // { 0, 1.79, 1.09 }
            double[] cos = u.Apply(Math.Cos); // { 0.54, 0.96, -0.989 }

            Assert.IsTrue(abs.IsEqual(new double[] { 1, 6, 3 }));
            Assert.IsTrue(log.IsEqual(new double[] { 0, 1.79, 1.09 }, 1e-2));
            Assert.IsTrue(cos.IsEqual(new double[] { 0.54, 0.96, -0.989 }, 1e-2));

            double[,] m =
            {
                { 0, 5, 2 },
                { 2, 1, 5 }
            };

            double[] vcut = v.Submatrix(0, 1); // { 9, 4 }
            Assert.IsTrue(new double[] { 9, 4 }.IsEqual(vcut));

            double[] mv = m.Multiply(v); // { 24, 32 }
            double[] vm = vcut.Multiply(m); // { 8, 49, 38 }
            double[,] md = m.MultiplyByDiagonal(v); // { { 0, 20, 4 }, { 18, 4, 10 } }
            double[,] mmt = m.MultiplyByTranspose(m); // { { 29, 15 }, { 15, 30 } }

            Assert.IsTrue(new double[] { 24, 32 }.IsEqual(mv));
            Assert.IsTrue(new double[] { 8, 49, 38 }.IsEqual(vm));
            Assert.IsTrue(new double[,] { { 0, 20, 4 }, { 18, 4, 10 } }.IsEqual(md));
            Assert.IsTrue(new double[,] { { 29, 15 }, { 15, 30 } }.IsEqual(mmt));
        }

        [Test]
        public void SumTest()
        {
            double[,] value =
            {
                { 1, 2, 3, 4 },
                { 5, 6, 7, 8 }
            };

            double[] expected0 = { 6, 8, 10, 12 };

            double[] actual = Matrix.Sum(value, 0);
            Assert.IsTrue(expected0.IsEqual(actual));

            double[] actual0 = Matrix.Sum(value, 0);
            Assert.IsTrue(expected0.IsEqual(actual0));

            double[] expected1 = { 10, 26 };
            double[] actual1 = Matrix.Sum(value, 1);
            Assert.IsTrue(expected1.IsEqual(actual1));

        }

        [Test]
        public void SumTest1()
        {
            double[] value = { 1, 2, 3 };
            double expected = 6;
            double actual = Matrix.Sum(value);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SumTest2()
        {
            int[,] value =
            {
                { 2, 5, -1 },
                { 5, 0,  2 },
            };

            int[] expected;
            int[] actual;

            expected = new int[] { 7, 5, 1 };

            actual = Matrix.Sum(value, 0);
            Assert.IsTrue(expected.IsEqual(actual));

            actual = Matrix.Sum(value, 0);
            Assert.IsTrue(expected.IsEqual(actual));

            expected = new int[] { 6, 7 };
            actual = Matrix.Sum(value, 1);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void SumTest4()
        {
            double[,] value =
            {
                { 2.2, 5.1, -1.0 },
                { 5.8, 0.0,  2.7 },
            };

            double[] expected;
            double[] actual;

            expected = new double[] { 8, 5.1, 1.7 };

            actual = Matrix.Sum(value, 0);
            Assert.IsTrue(expected.IsEqual(actual, 0.000000001));

            actual = Matrix.Sum(value, 0);
            Assert.IsTrue(expected.IsEqual(actual, 0.000000001));

            expected = new double[] { 6.3, 8.5 };
            actual = Matrix.Sum(value, 1);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void SumTest5()
        {
            int[] value = { 9, -2, 1 };
            int expected = 8;
            int actual = Matrix.Sum(value);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SumTest6()
        {
            double[] value = { 9.2, -2, 1 };
            double expected = 8.2;
            double actual = Matrix.Sum(value);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CumulativeSumTest()
        {
            double[] expected1 = { 1, 3, 6, 10, 15 };
            double[] actual1 = Matrix.CumulativeSum(new double[] { 1, 2, 3, 4, 5 });

            Assert.IsTrue(actual1.IsEqual(expected1));

            double[,] A =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            double[][] actual2 = A.ToJagged().CumulativeSum(1);
            double[][] expected2 =
            {
                new double[] { 1, 2, 3 },
                new double[] { 5, 7, 9 }
            };

            Assert.IsTrue(actual2.IsEqual(expected2));

            double[][] actual3 = A.ToJagged().CumulativeSum(0);
            double[][] expected3 =
            {
                new double[] {1,  4 },
                new double[] {3,  9 },
                new double[] {6, 15 }
            };

            Assert.IsTrue(actual3.IsEqual(expected3));
        }

        [Test]
        public void ProductTest()
        {
            double[] value = { -1, 2.4, 7 };
            double expected = -16.8;
            double actual = Matrix.Product(value);
            Assert.AreEqual(expected, actual, 0.00001);
        }

        [Test]
        public void ProductTest1()
        {
            int[] value = { -1, 2, 7 };
            int expected = -14;
            int actual = Matrix.Product(value);
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region Combine
        [Test]
        public void CombineTest()
        {
            int[][,] matrices =
            {
                new int[,]
                {
                      {0, 1}
                },

                new int[,]
                {
                      {1, 0},
                      {1, 0}
                },

                new int[,]
                {
                      {0, 2}
                }
            };


            int[,] expected =
            {
                 {0, 1},
                 {1, 0},
                 {1, 0},
                 {0, 2}
            };

            int[,] actual;
            actual = Matrix.Stack(matrices);

            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void CombineTest1()
        {
            double[][] vectors = new double[][]
            {
                new double[] { 0, 1, 2 },
                new double[] { 3, 4, },
                new double[] { 5, 6, 7, 8, 9},
            };

            double[] expected = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            var actual = Matrix.Concatenate(vectors);

            Assert.IsTrue(Matrix.IsEqual(expected, actual));

        }

        [Test]
        public void CombineTest3()
        {
            double[] a1 = new double[] { 1, 2 };
            double[] a2 = new double[] { 3, 4, 5 };

            double[] expected = new double[] { 1, 2, 3, 4, 5 };
            double[] actual = Matrix.Concatenate(a1, a2);

            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void CombineTest4()
        {
            double[] a1 = new double[] { 1, 2 };
            double a2 = 3;

            double[] expected = new double[] { 1, 2, 3 };
            double[] actual = Matrix.Concatenate(a1, a2);

            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void ConcatenateTest2()
        {
            double[][] a =
            {
                new double[] { 1, 2 },
                new double[] { 6, 7 },
                new double[] { 11, 12 },
            };

            double[][] b =
            {
                new double[] {  3,  4,  5 },
                new double[] {  8,  9, 10 },
                new double[] { 13, 14, 15 },
            };

            double[][] expected =
            {
                new double[] {  1,  2,  3,  4,  5 },
                new double[] {  6,  7,  8,  9, 10 },
                new double[] { 11, 12, 13, 14, 15 },
            };

            {
                double[][] actual = Matrix.Concatenate(a, b);
                Assert.IsTrue(Matrix.IsEqual(expected, actual));
            }

            {
                double[][] actual = a.Concatenate(b);
                Assert.IsTrue(Matrix.IsEqual(expected, actual));
            }
        }

        [Test]
        public void CombineTest5()
        {
            double[,] A = Matrix.Square(2, 2.0);
            double[,] B = Matrix.Square(2, 1.0);

            double[,] expected =
            {
                { 2, 2 },
                { 2, 2 },
                { 1, 1 },
                { 1, 1 }
            };

            double[,] actual = Matrix.Stack<double>(new[] { A, B });

            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }
        #endregion

        #region Vectorial Products
        [Test]
        public void OuterProductTest()
        {
            double[] a = { 1, 2, 3, 4 };
            double[] b = { 1, 2, 3, 4 };
            double[,] expected =
            {
                { 1.000,  2.000,  3.000,  4.000 },
                { 2.000,  4.000,  6.000,  8.000 },
                { 3.000,  6.000,  9.000, 12.000 },
                { 4.000,  8.000, 12.000, 16.000 }
            };

            double[,] actual = Matrix.OuterProduct(a, b);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void OuterProductTestDifferentOverloads()
        {
            double[] a = { 1, 2, 3 };
            double[] b = { 4, 5 };

            double[,] expected = new double[,]
            {
              {  4,  5 },
              {  8, 10 },
              { 12, 15 },
            };

            double[,] actual2 = Matrix.Random(a.Length, b.Length);
            double[][] actual3 = actual2.ToJagged();

            double[,] actual1 = a.Outer(b);
            double[][] actual3a = a.Outer(b, actual3);

            Assert.AreSame(actual3, actual3a);

            Assert.IsTrue(expected.IsEqual(actual1));
            Assert.IsTrue(expected.IsEqual(actual3));
        }

        [Test]
        public void CartesianProductTest()
        {
            int[][] sequences =
            {
                new int[] { 1, 2, 3},
                new int[] { 4, 5, 6},
            };

            var actual = Matrix.CartesianProduct(sequences);

            var list = new List<int[]>();
            foreach (IEnumerable<int> point in actual)
                list.Add(new List<int>(point).ToArray());

            var points = list.ToArray();

            Assert.IsTrue(points[0].IsEqual(new int[] { 1, 4 }));
            Assert.IsTrue(points[1].IsEqual(new int[] { 1, 5 }));
            Assert.IsTrue(points[2].IsEqual(new int[] { 1, 6 }));
            Assert.IsTrue(points[3].IsEqual(new int[] { 2, 4 }));
            Assert.IsTrue(points[4].IsEqual(new int[] { 2, 5 }));
            Assert.IsTrue(points[5].IsEqual(new int[] { 2, 6 }));
            Assert.IsTrue(points[6].IsEqual(new int[] { 3, 4 }));
            Assert.IsTrue(points[7].IsEqual(new int[] { 3, 5 }));
            Assert.IsTrue(points[8].IsEqual(new int[] { 3, 6 }));

        }

        [Test]
        public void CartesianProductTest2()
        {
            int[][] sequences =
            {
                new int[] { 1, 2, 3},
                new int[] { 4, 5, 6},
            };

            int[][] points = Matrix.CartesianProduct(sequences);

            Assert.IsTrue(points[0].IsEqual(new int[] { 1, 4 }));
            Assert.IsTrue(points[1].IsEqual(new int[] { 1, 5 }));
            Assert.IsTrue(points[2].IsEqual(new int[] { 1, 6 }));
            Assert.IsTrue(points[3].IsEqual(new int[] { 2, 4 }));
            Assert.IsTrue(points[4].IsEqual(new int[] { 2, 5 }));
            Assert.IsTrue(points[5].IsEqual(new int[] { 2, 6 }));
            Assert.IsTrue(points[6].IsEqual(new int[] { 3, 4 }));
            Assert.IsTrue(points[7].IsEqual(new int[] { 3, 5 }));
            Assert.IsTrue(points[8].IsEqual(new int[] { 3, 6 }));

        }

        [Test]
        public void CartesianProductTest3()
        {
            int[][] sequences =
            {
                new int[] { 1, 2, 3},
                new int[] { 4, 5, 6},
            };

            int[][] points = sequences[0].CartesianProduct(sequences[1]);

            Assert.IsTrue(points[0].IsEqual(new int[] { 1, 4 }));
            Assert.IsTrue(points[1].IsEqual(new int[] { 1, 5 }));
            Assert.IsTrue(points[2].IsEqual(new int[] { 1, 6 }));
            Assert.IsTrue(points[3].IsEqual(new int[] { 2, 4 }));
            Assert.IsTrue(points[4].IsEqual(new int[] { 2, 5 }));
            Assert.IsTrue(points[5].IsEqual(new int[] { 2, 6 }));
            Assert.IsTrue(points[6].IsEqual(new int[] { 3, 4 }));
            Assert.IsTrue(points[7].IsEqual(new int[] { 3, 5 }));
            Assert.IsTrue(points[8].IsEqual(new int[] { 3, 6 }));

        }
        #endregion

        #region Inverse, division and solving
        [Test]
        public void InverseTest2x2()
        {
            double[,] value =
            {
                { 3.0, 1.0 },
                { 2.0, 2.0 }
            };

            double[,] expected = new SingularValueDecomposition(value).Inverse();

            double[,] actual = Matrix.Inverse(value);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-6));
        }

        [Test]
        public void InverseTest3x3_svd()
        {
            double[,] value =
            {
                { 6.0, 1.0, 2.0 },
                { 0.0, 8.0, 1.0 },
                { 2.0, 4.0, 5.0 }
            };

            Assert.IsFalse(value.IsSingular());

            double[,] expected = new SingularValueDecomposition(value).Inverse();

            double[,] actual = Matrix.Inverse(value);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-6));
        }

        [Test]
        public void doc_inverse()
        {
            #region doc_inverse
            // Declare a matrix as
            double[,] matrix =
            {
                { 6.0, 1.0, 2.0 },
                { 0.0, 8.0, 1.0 },
                { 2.0, 4.0, 5.0 }
            };

            // Compute the inverse using
            double[,] inv = matrix.Inverse();

            // We can write the result with
            string strInv = inv.ToCSharp();

            // The result should be:
            // new double[,] 
            // {
            //     { 0.193548387096774, 0.0161290322580645, -0.0806451612903226 },
            //     { 0.010752688172043, 0.139784946236559, -0.032258064516129 },
            //     { -0.0860215053763441, -0.118279569892473, 0.258064516129032 }
            // };

            // We can confirm this is indeed the inverse by
            // checking whether "inv(matrix) * matrix" and
            // "matrix * inv(matrix)" equals I (identity):
            double[,] a = inv.Dot(matrix);
            double[,] b = matrix.Dot(inv);

            // Again we write the result:
            string strA = a.ToCSharp();
            string strB = b.ToCSharp();

            // The result should be:
            // new double[,] 
            // {
            //     { 1, 0, 0 },
            //     { 0, 1, 0 },
            //     { 0, 0, 1 }
            // };
            #endregion

            Assert.IsTrue(Matrix.IsEqual(a, Matrix.Identity(3), 1e-6));
            Assert.IsTrue(Matrix.IsEqual(b, Matrix.Identity(3), 1e-6));
        }

        [Test]
        public void doc_pseudo_inverse()
        {
            #region doc_pseudoinverse
            // Declare a non-invertible matrix as
            double[,] matrix =
            {
                { 6.0, 1.0, 2.0 },
                { 0.0, 8.0, 1.0 },
            };

            // Let's check that this matrix really cannot be
            // inverted using standard matrix inversion:
            bool isSingular = matrix.IsSingular(); // should be true

            // Compute the pseudo-inverse using
            double[,] pinv = matrix.PseudoInverse();

            // We can write the result with
            string strInv = pinv.ToCSharp();

            // The result should be:
            // new double[,] 
            // {
            //     { 0.193548387096774, 0.0161290322580645, -0.0806451612903226 },
            //     { 0.010752688172043, 0.139784946236559, -0.032258064516129 },
            //     { -0.0860215053763441, -0.118279569892473, 0.258064516129032 }
            // };

            // We can confirm this is indeed the pseudo-inverse 
            // by checking whether "matrix * pinv(matrix) * matrix"
            // equals the original matrix:
            double[,] r = matrix.Dot(pinv.Dot(matrix));

            // Again we write the result:
            string strA = r.ToCSharp();

            // The result should be:
            // new double[,] 
            // {
            //  { 6.0, 1.0, 2.0 },
            //  { 0.0, 8.0, 1.0 },
            // };
            #endregion

            Assert.IsTrue(isSingular);
            Assert.IsTrue(Matrix.IsEqual(r, matrix, 1e-6));
        }

        [Test]
        public void PseudoInverse()
        {
            double[,] value = new double[,]
                { { 1.0, 1.0 },
                  { 2.0, 2.0 }  };


            double[,] expected = new double[,]
                { { 0.1, 0.2 },
                  { 0.1, 0.2 }  };

            double[,] actual = Matrix.PseudoInverse(value);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.001));
        }

        [Test]
        public void PseudoInverse1()
        {
            double[,] value =
            {
                 {  1,  1,  1 },
                 {  2,  2,  2 }
             };

            double[,] expected =
             {
                 { 0.0667,    0.1333 },
                 { 0.0667,    0.1333 },
                 { 0.0667,    0.1333 },
             };

            double[,] actual = Matrix.PseudoInverse(value);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.001));

            actual = Matrix.PseudoInverse(value.Transpose());
            Assert.IsTrue(Matrix.IsEqual(expected.Transpose(), actual, 0.001));

            actual = Matrix.PseudoInverse(value.ToJagged()).ToMatrix();
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.001));

            actual = Matrix.PseudoInverse(value.ToJagged().Transpose()).ToMatrix();
            Assert.IsTrue(Matrix.IsEqual(expected.Transpose(), actual, 0.001));
        }

        [Test]
        public void PseudoInverse2()
        {
            double[,] X =
            {
               { 2.5, 2.3 }
            };

            double[,] actual = X.PseudoInverse();

            double[,] expected =
            {
               { 0.2166 },
               { 0.1993 }
            };

            Assert.IsTrue(expected.IsEqual(actual, 0.001));
        }

        [Test]
        public void PseudoInverse3()
        {
            double[,] X =
            {
               { 2.5 },
               { 2.3 }
            };

            double[,] actual = X.PseudoInverse();

            double[,] expected =
            {
               { 0.2166, 0.1993 }
            };

            Assert.IsTrue(expected.IsEqual(actual, 0.001));
        }

        [Test]
        public void PseudoInverse4()
        {
            double[,] X = CsvReader.FromText(Properties.Resources.pseudoInverse1, false).ToMatrix();

            double[,] invX = X.PseudoInverse();
            double[,] actual = X.Dot(invX);
            double[,] expected = Matrix.Identity(9);
            Assert.IsTrue(expected.IsEqual(actual, 1e-6));
        }

        [Test]
        public void PseudoInverse5()
        {
            double[][] X = CsvReader.FromText(Properties.Resources.pseudoInverse1, false).ToJagged();

            double[][] invX = X.PseudoInverse();
            double[][] actual = X.Dot(invX);
            double[][] expected = Jagged.Identity(9);
            Assert.IsTrue(expected.IsEqual(actual, 1e-6));
        }

        [Test]
        public void SolveTest()
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

            double[] actual = Matrix.Solve(value, rhs);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-3));

            actual = Matrix.Solve(value.ToJagged(), rhs);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-3));
        }

        [Test]
        public void SolveTest2()
        {
            double[,] value =
            {
               {  2, -1,  0 },
               { -1,  2, -1 },
               {  0, -1,  2 }
            };

            double[] b = { 1, 2, 3 };

            double[] expected = { 2.5000, 4.0000, 3.5000 };

            double[] actual = Matrix.Solve(value, b);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-10));

            actual = Matrix.Solve(value.ToJagged(), b);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-10));
        }

        [Test]
        public void NullTest()
        {
            double[,] value =
            {
                { 3.0, 1.0 },
                { 2.0, 2.0 }
            };

            double[,] actual = Matrix.Null(value);

            Assert.AreEqual(2, actual.Rows());
            Assert.AreEqual(0, actual.Columns());
        }

        [Test]
        public void NullTest2()
        {
            double[] value = new double[] { 1, 2, 3 };

            double[,] expected =
            {
                { -0.53452248382484879, -0.80178372573727341 },
                { 0.77454192058843829,  -0.33818711911734273 },
                { -0.33818711911734267,  0.49271932132398588 },
            };

            double[][] actual = Matrix.Null(value);

            var a = Jagged.RowVector(value).Dot(expected.GetColumn(0));
            var b = Jagged.RowVector(value).Dot(expected.GetColumn(1));
            Assert.IsTrue(Matrix.IsEqual(a, new[] { 0 }, 1e-6));
            Assert.IsTrue(Matrix.IsEqual(b, new[] { 0 }, 1e-6));

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-6));
        }

        [Test]
        public void FindTest2()
        {
            double[] value = new double[] { 1, 2, 3 };

            int[] expected =
            {
            };

            int[] actual = Matrix.Find(value, x => x < 0);

            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void DivideTest()
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

            double[,] actual = Matrix.Divide(a, b);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-3));

            actual = Matrix.Divide(a.ToJagged(), b.ToJagged()).ToMatrix();
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-3));
        }

        [Test]
        public void DivideTest2()
        {
            double[,] a =
            {
                { 2, 1, 4, 1 },
                { 6, 2, 2, 2 },
                { 0, 1, 6, 1 },
            };

            var stra = a.ToString(OctaveMatrixFormatProvider.InvariantCulture);

            double[,] b =
            {
                { 1, 0, 7, 7 },
                { 5, 2, 1, 2 },
                { 1, 5, 2, 1 },
            };

            var strb = b.ToString(OctaveMatrixFormatProvider.InvariantCulture);

            double[,] expected =
            {
                { 0.2757,  0.2122,  0.1904 },
                { 0.0464,  1.1602, -0.0343 },
                { 0.4819, -0.3160,  0.4323 },
            };

            double[,] actual = Matrix.Divide(a, b);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-3));

            actual = Matrix.Divide(a.ToJagged(), b.ToJagged()).ToMatrix();
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-3));
        }

        [Test]
        public void DivideTest3()
        {
            double[,] a =
            {
                { 1,     0,     5 },
                { 1,     2,     1 },
                { 0,     6,     1 },
                { 2,     6,     5 },
                { 2,     1,     1 },
                { 5,     1,     1 }
            };

            var stra = a.ToString(OctaveMatrixFormatProvider.InvariantCulture);

            double[,] b =
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 },
            };

            var strb = b.ToString(OctaveMatrixFormatProvider.InvariantCulture);

            double[,] actual = Matrix.Divide(a, b);
            Assert.IsTrue(Matrix.IsEqual(a, actual, 1e-3));

            actual = Matrix.Divide(a.ToJagged(), b.ToJagged()).ToMatrix();
            Assert.IsTrue(Matrix.IsEqual(a, actual, 1e-3));
        }

        [Test]
        public void DivideByDiagonalTest()
        {
            double[,] a =
            {
                { 1,     0,     5 },
                { 1,     2,     1 },
                { 0,     6,     1 },
                { 2,     6,     5 },
                { 2,     1,     1 },
                { 5,     1,     1 }
            };

            double[] b = { 2, 6, 1 };
            double[,] result = new double[6, 3];
            Matrix.DivideByDiagonal(a, b, result);

            double[,] expected = Matrix.Divide(a, Matrix.Diagonal(b));
            Assert.IsTrue(expected.IsEqual(result, 1e-6));

            result = Matrix.DivideByDiagonal(a.ToJagged(), b).ToMatrix();
            expected = Matrix.Divide(a.ToJagged(), Jagged.Diagonal(b)).ToMatrix();
            Assert.IsTrue(expected.IsEqual(result, 1e-6));
        }
        #endregion

        #region Matrix characteristics
        [Test]
        public void DeterminantTest()
        {
            double[,] m =
            {
                { 3.000, 1.000, 0.000, 2.000 },
                { 4.000, 1.000, 2.000, 4.000 },
                { 1.000, 1.000, 1.000, 1.000 },
                { 0.000, 1.000, 2.000, 3.000 }
            };

            double expected = -11;
            double actual = Matrix.Determinant(m);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DeterminantTest2()
        {
            double[,] m =
            {
                { 3.000, 1.000, 0.000, 2.000 },
                { 4.000, 1.000, 2.000, 4.000 },
                { 1.000, 1.000, 0.000, 1.000 },
                { 0.000, 1.000, 2.000, 0.000 }
            };

            double expected = 8;

            double det;
            det = Matrix.Determinant(m);
            Assert.AreEqual(expected, det);

            det = Matrix.LogDeterminant(m);
            Assert.AreEqual(Math.Log(expected), det, 1e-10);
            Assert.IsFalse(Double.IsNaN(det));

            det = Matrix.PseudoDeterminant(m);
            Assert.AreEqual(expected, det, 1e-10);
            Assert.IsFalse(Double.IsNaN(det));
        }

        [Test]
        public void DeterminantTest3()
        {
            double[,] m =
            {
                { 0, 4, 0, 2 },
                { 4, 1, 2, 4 },
                { 0, 2, 1, 1 },
                { 2, 4, 1, 1 }
            };

            Assert.IsTrue(m.IsSymmetric());
            Assert.IsFalse(m.IsPositiveDefinite());

            double expected = 44;

            bool thrown = false;

            try
            {
                Matrix.Determinant(m, symmetric: true);
            }
            catch (Exception)
            {
                thrown = true;
            }

            Assert.IsTrue(thrown);

            thrown = false;

            try
            {
                Matrix.LogDeterminant(m, symmetric: true);
            }
            catch (Exception)
            {
                thrown = true;
            }

            Assert.IsTrue(thrown);

            double det = Matrix.PseudoDeterminant(m);
            Assert.AreEqual(expected, det, 1e-10);
            Assert.IsFalse(Double.IsNaN(det));
        }


        [Test]
        public void PositiveDefiniteTest()
        {
            double[,] m =
            {
                { 2, 2 },
                { 2, 2 },
            };

            bool expected = false;
            bool actual = Matrix.IsPositiveDefinite(m);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PositiveDefiniteTest2()
        {
            double[,] m =
            {
                {  2, -1,  0 },
                { -1,  2, -1 },
                {  0, -1,  2 },
            };

            bool expected = true;
            bool actual = Matrix.IsPositiveDefinite(m);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PositiveDefiniteJaggedTest()
        {
            double[][] m =
            {
                new double[] { 2, 2 },
                new double[] { 2, 2 },
            };

            bool expected = false;
            bool actual = Matrix.IsPositiveDefinite(m);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PositiveDefiniteJaggedTest2()
        {
            double[][] m =
            {
                new double[] {  2, -1,  0 },
                new double[] { -1,  2, -1 },
                new double[] {  0, -1,  2 },
            };

            bool expected = true;
            bool actual = Matrix.IsPositiveDefinite(m);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TraceTest()
        {
            double[,] m =
            {
                { 3.000, 1.000, 0.000, 2.000 },
                { 4.000, 1.000, 2.000, 4.000 },
                { 1.000, 1.000, 1.000, 1.000 },
                { 0.000, 1.000, 2.000, 3.000 }
            };

            double expected = 8;
            double actual = Matrix.Trace(m);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsSymmetricTest()
        {
            double[,] matrix =
            {
                { 1, 2 },
                { 3, 4 }
            };

            bool expected = false;
            bool actual = Matrix.IsSymmetric(matrix);
            Assert.AreEqual(expected, actual);

            double[,] matrix2 =
            {
                { 1, 2 },
                { 2, 1 }
            };

            expected = true;
            actual = Matrix.IsSymmetric(matrix2);
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void IsSymmetricTest_multidimensional()
        {
            double[,] matrix =
            {
                { 1, 2.0000004 },
                { 2.0000002, 4 }
            };

            Assert.IsFalse(Matrix.IsSymmetric(matrix, atol: 1e-10));
            Assert.IsTrue(Matrix.IsSymmetric(matrix, atol: 1e-3));
        }

        [Test]
        public void IsSymmetricTest_jagged()
        {
            double[][] matrix =
            {
                new[] { 1, 2.00000005 },
                new[] { 2.0000003, 4 }
            };

            Assert.IsFalse(Matrix.IsSymmetric(matrix, atol: 1e-10));
            Assert.IsTrue(Matrix.IsSymmetric(matrix, atol: 1e-3));
        }

        [Test]
        public void MaxMinTest1()
        {
            double[] a = { 5 };

            int imax;
            int imin;

            double max = Matrix.Max(a, out imax);
            double min = Matrix.Min(a, out imin);

            Assert.AreEqual(max, min);
            Assert.AreEqual(imax, imin);

            Assert.AreEqual(5, max);
            Assert.AreEqual(0, imax);
        }

        [Test]
        public void MaxMinTest()
        {
            double[,] matrix = new double[,]
            {
                { 0, 1, 3, 1},
                { 9, 1, 3, 1},
                { 2, 4, 4, 11},
            };

            // Max
            int dimension = 1;
            int[] imax = null;
            int[] imaxExpected = new int[] { 2, 0, 3 };
            double[] expected = new double[] { 3, 9, 11 };
            double[] actual;
            actual = Matrix.Max(matrix, dimension, out imax);

            Assert.IsTrue(Matrix.IsEqual(imaxExpected, imax));
            Assert.IsTrue(Matrix.IsEqual(expected, actual));


            dimension = 0;
            imaxExpected = new int[] { 1, 2, 2, 2 };
            expected = new double[] { 9, 4, 4, 11 };

            actual = Matrix.Max(matrix, dimension, out imax);

            Assert.IsTrue(Matrix.IsEqual(imaxExpected, imax));
            Assert.IsTrue(Matrix.IsEqual(expected, actual));


            // Min
            dimension = 1;
            int[] imin = null;
            int[] iminExpected = new int[] { 0, 1, 0 };
            expected = new double[] { 0, 1, 2 };
            actual = Matrix.Min(matrix, dimension, out imin);

            Assert.IsTrue(Matrix.IsEqual(iminExpected, imin));
            Assert.IsTrue(Matrix.IsEqual(expected, actual));


            dimension = 0;
            iminExpected = new int[] { 0, 0, 0, 0 };
            expected = new double[] { 0, 1, 3, 1 };
            actual = Matrix.Min(matrix, dimension, out imin);

            Assert.IsTrue(Matrix.IsEqual(iminExpected, imin));
            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void UpperTriangularTest()
        {
            double[,] U =
            {
                { 1, 2, 1, },
                { 0, 2, 1, },
                { 0, 0, 1, },
            };

            double[,] L =
            {
                { 1, 0, 0, },
                { 5, 2, 0, },
                { 2, 1, 1, },
            };

            double[,] D =
            {
                { 1, 0, 0, },
                { 0, 2, 0, },
                { 0, 0, 0, },
            };

            Assert.IsTrue(U.IsUpperTriangular());
            Assert.IsFalse(U.IsLowerTriangular());
            Assert.IsFalse(U.IsDiagonal());

            Assert.IsFalse(L.IsUpperTriangular());
            Assert.IsTrue(L.IsLowerTriangular());
            Assert.IsFalse(L.IsDiagonal());

            Assert.IsTrue(D.IsUpperTriangular());
            Assert.IsTrue(D.IsLowerTriangular());
            Assert.IsTrue(D.IsDiagonal());
        }

        [Test]
        public void UpperTriangularTest2()
        {
            double[][] U =
            {
                new double[] { 1, 2, 1, },
                new double[] { 0, 2, 1, },
                new double[] { 0, 0, 1, },
            };

            double[][] L =
            {
                new double[] { 1, 0, 0, },
                new double[] { 5, 2, 0, },
                new double[] { 2, 1, 1, },
            };

            double[][] D =
            {
                new double[] { 1, 0, 0, },
                new double[] { 0, 2, 0, },
                new double[] { 0, 0, 0, },
            };

            Assert.IsTrue(U.IsUpperTriangular());
            Assert.IsFalse(U.IsLowerTriangular());
            Assert.IsFalse(U.IsDiagonal());

            Assert.IsFalse(L.IsUpperTriangular());
            Assert.IsTrue(L.IsLowerTriangular());
            Assert.IsFalse(L.IsDiagonal());

            Assert.IsTrue(D.IsUpperTriangular());
            Assert.IsTrue(D.IsLowerTriangular());
            Assert.IsTrue(D.IsDiagonal());
        }

        [Test]
        public void ToUpperTriangularTest()
        {
            double[,] U =
            {
                { 1, 2, 1, },
                { 0, 2, 1, },
                { 0, 0, 1, },
            };

            double[,] L =
            {
                { 1, 0, 0, },
                { 5, 2, 0, },
                { 2, 1, 1, },
            };

            double[,] X =
            {
                { 1, 0, 5, },
                { 0, 2, 0, },
                { 6, 0, 3, },
            };

            Assert.IsTrue(U.ToUpperTriangular(from: MatrixType.UpperTriangular).GetUpperTriangle(true).IsEqual(U));
            Assert.IsTrue(U.ToLowerTriangular(from: MatrixType.UpperTriangular).GetLowerTriangle(true).IsEqual(U.Transpose()));

            Assert.IsTrue(L.ToUpperTriangular(from: MatrixType.LowerTriangular).GetUpperTriangle(true).IsEqual(L.Transpose()));
            Assert.IsTrue(L.ToLowerTriangular(from: MatrixType.LowerTriangular).GetLowerTriangle(true).IsEqual(L));

            var LowerToUpper = X.ToUpperTriangular(from: MatrixType.LowerTriangular);
            var UpperToUpper = X.ToUpperTriangular(from: MatrixType.UpperTriangular);
            var LowerToLower = X.ToLowerTriangular(from: MatrixType.LowerTriangular);
            var UpperToLower = X.ToLowerTriangular(from: MatrixType.UpperTriangular);

            var a = LowerToUpper.ToCSharp();
            var b = UpperToUpper.ToCSharp();
            var c = LowerToLower.ToCSharp();
            var d = UpperToLower.ToCSharp();

            Assert.IsTrue(LowerToUpper.IsEqual(new double[,] {
                { 1, 0, 6 },
                { 0, 2, 0 },
                { 5, 0, 3 }
            }));

            Assert.IsTrue(UpperToUpper.IsEqual(new double[,] {
                { 1, 0, 5 },
                { 0, 2, 0 },
                { 6, 0, 3 }
            }));

            Assert.IsTrue(LowerToLower.IsEqual(new double[,] {
                { 1, 0, 5 },
                { 0, 2, 0 },
                { 6, 0, 3 }
            }));

            Assert.IsTrue(UpperToLower.IsEqual(new double[,] {
                { 1, 0, 6 },
                { 0, 2, 0 },
                { 5, 0, 3 }
            }));
        }

        #endregion

        #region Transpose
        [Test]
        public void TransposeTest()
        {
            int[] value = { 1, 5, 2 };
            int[,] expected =
            {
                { 1 },
                { 5 },
                { 2 },
            };

            int[,] actual = Matrix.Transpose(value);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void TransposeTest2()
        {
            int[,] value =
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 }
            };

            int[,] expected =
            {
                { 1, 4, 7 },
                { 2, 5, 8 },
                { 3, 6, 9 }
            };

            int[,] actual;

            actual = Matrix.Transpose(value, false);
            Assert.IsTrue(expected.IsEqual(actual));
            Assert.AreNotEqual(value, actual);

            actual = Matrix.Transpose(value, true);
            Assert.IsTrue(expected.IsEqual(actual));
            Assert.AreEqual(value, actual);
        }

        [Test]
        public void TransposeTest3()
        {
            double[][] matrix = new double[,]
            {
                { 1, 2 },
                { 3, 4 },
            }.ToJagged();

            bool inPlace = true;
            double[][] expected = new double[,]
            {
                { 1, 3 },
                { 2, 4 },
            }.ToJagged();

            double[][] actual = Matrix.Transpose(matrix, inPlace);

            Assert.AreEqual(matrix, actual);
            Assert.IsTrue(actual.IsEqual(expected));
        }

        [Test]
        public void GeneralizedTransposeTest()
        {
            double[,] a =
            {
                { 5, 1, 6, 3, 1 },
                { 7, 5, 1, 2, 8 },
            };

            double[,] actual = a.Transpose(new int[] { 1, 0 });
            double[,] expected = a.Transpose();

            Assert.IsTrue(actual.IsEqual(expected));
        }
        #endregion

        #region Apply
        [Test]
        public void ApplyTest()
        {
            double[] data = { 42, 1, -5 };
            Func<double, double> func = x => x - x;

            double[] actual;
            double[] expected = { 0, 0, 0 };

            actual = Matrix.Apply(data, func);
            Assert.IsTrue(expected.IsEqual(actual));
            Assert.AreNotEqual(actual, data);

            Matrix.ApplyInPlace(data, func);
            Assert.IsTrue(expected.IsEqual(data));
        }

        [Test]
        public void ApplyTest2()
        {
            double[,] data =
            {
                { 42, 1, -5 },
                { 42, 1, -5 },
            };

            Func<double, double> func = x => x - x;

            double[,] actual;
            double[,] expected =
            {
                { 0, 0, 0 },
                { 0, 0, 0 },
            };

            actual = Matrix.Apply(data, func);
            Assert.IsTrue(expected.IsEqual(actual));
            Assert.AreNotEqual(actual, data);

            Matrix.ApplyInPlace(data, func);
            Assert.IsTrue(expected.IsEqual(data));
        }

        [Test]
        public void ApplyTest3()
        {
            double[] data = { 42, 1, -5 };
            Func<double, int> func = x => (int)(x - x);

            int[] actual;
            int[] expected = { 0, 0, 0 };

            actual = Matrix.Apply(data, func);
            Assert.IsTrue(expected.IsEqual(actual));
            Assert.AreNotEqual(actual, data);
        }

        [Test]
        public void ApplyTest4()
        {
            double[,] data =
            {
                { 42, 1, -5 },
                { 42, 1, -5 },
            };

            Func<double, int> func = x => (int)(x - x);

            int[,] actual;
            int[,] expected =
            {
                { 0, 0, 0 },
                { 0, 0, 0 },
            };

            actual = Matrix.Apply(data, func);
            Assert.IsTrue(expected.IsEqual(actual));
            Assert.AreNotEqual(actual, data);
        }

        [Test]
        public void ApplyTest1()
        {
            int[,] matrix =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            Func<int, int, int, string> func =
                (x, i, j) => "Element at (" + i + "," + j + ") is " + x;

            string[,] expected =
            {
                { "Element at (0,0) is 1", "Element at (0,1) is 2", "Element at (0,2) is 3" },
                { "Element at (1,0) is 4", "Element at (1,1) is 5", "Element at (1,2) is 6" },
            };

            string[,] actual = Matrix.ApplyWithIndex(matrix, func);

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void ApplyTest5()
        {
            int[] matrix = { 1, 2, 3 };

            Func<int, int, string> func =
                (x, i) => "Element at (" + i + ") is " + x;

            string[] expected =
            {
                "Element at (0) is 1",
                "Element at (1) is 2",
                "Element at (2) is 3",
            };

            string[] actual = Matrix.ApplyWithIndex(matrix, func);

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void ApplyInPlaceTest()
        {
            float[] vector = { 1, 2, 3 };
            Func<float, int, float> func = (x, i) => x + i;
            Matrix.Apply(vector, func, vector);
            float[] expected = { 1, 3, 5 };

            Assert.IsTrue(expected.IsEqual(vector));
        }

        #endregion

        #region Floor, Ceiling, Rouding
        [Test]
        public void CeilingTest1()
        {
            double[] vector = { 0.1, 0.5, 1.5 };
            double[] expected = { 1.0, 1.0, 2.0 };
            double[] actual = Matrix.Ceiling(vector);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void CeilingTest()
        {
            double[,] matrix =
            {
                {  0.1, 0.5, 1.5 },
                { -1.1, 2.5, 0.5 },
            };
            double[,] expected =
            {
                {  1.0, 1.0, 2.0 },
                { -1.0, 3.0, 1.0 },
            };

            double[,] actual = Matrix.Ceiling(matrix);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        /// <summary>
        ///A test for Floor
        ///</summary>
        [Test]
        public void FloorTest1()
        {
            double[] vector = { 0.1, 0.5, 1.5 };
            double[] expected = { 0.0, 0.0, 1.0 };
            double[] actual = Matrix.Floor(vector);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        /// <summary>
        ///A test for Floor
        ///</summary>
        [Test]
        public void FloorTest()
        {
            double[,] matrix =
            {
                {  0.1, 0.5, 1.5 },
                { -1.1, 2.5, 0.5 },
            };
            double[,] expected =
            {
                {  0.0, 0.0, 1.0 },
                { -2.0, 2.0, 0.0 },
            };

            double[,] actual = Matrix.Floor(matrix);
            Assert.IsTrue(expected.IsEqual(actual));
        }
        #endregion

        #region Power
        [Test]
        public void PowerTest()
        {
            double[,] a = Matrix.Magic(5);
            double[,] expected = Matrix.Identity(5);
            double[,] actual = Matrix.Power(a, 0);
            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void PowerTest1()
        {
            double[,] a = Matrix.Identity(5);
            double[,] expected = Matrix.Identity(5);
            double[,] actual = Matrix.Power(a, 10);
            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void PowerTest2()
        {
            double[,] a = Matrix.Magic(5);
            double[,] expected =
            {
                { 233699250, 233153250, 230181625, 230588750, 232667750 },
                { 233231250, 231525875, 230557375, 231902000, 233074125 },
                { 230869500, 229869750, 232058125, 234246500, 233246750 },
                { 231042125, 232214250, 233558875, 232590375, 230885000 },
                { 231448500, 233527500, 233934625, 230963000, 230417000 },
            };

            double[,] actual = Matrix.Power(a, 5);

            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }
        #endregion

        [Test]
        public void ExpandTest()
        {
            double[][] data =
            {
               new double[] { 0, 0 },
               new double[] { 0, 1 },
               new double[] { 1, 0 },
               new double[] { 1, 1 }
            };

            int[] count =
            {
                2,
                1,
                3,
                1
            };

            double[][] expected =
            {
                new double[] { 0, 0 },
                new double[] { 0, 0 }, // 2
                new double[] { 0, 1 }, // 1
                new double[] { 1, 0 },
                new double[] { 1, 0 },
                new double[] { 1, 0 }, // 3
                new double[] { 1, 1 }, // 1
            };

            double[][] actual = Matrix.Expand(data, count);

            Assert.IsTrue(Matrix.IsEqual(expected.ToMatrix(), actual.ToMatrix()));
        }



        [Test]
        public void MagicTest()
        {
            var actual = Matrix.Magic(3);

            double[,] expected =
            {
                { 8,   1,   6 },
                { 3,   5,   7 },
                { 4,   9,   2 },
            };

            Assert.IsTrue(Matrix.IsEqual(actual, expected));

            actual = Matrix.Magic(4);

            expected = new double[,]
            {
                { 16,     2,     3,    13 },
                {  5,    11,    10,     8 },
                {  9,     7,     6,    12 },
                {  4,    14,    15,     1 },
            };

            Assert.IsTrue(Matrix.IsEqual(actual, expected));

            actual = Matrix.Magic(6);

            expected = new double[,]
            {
                 { 35,     1,     6,    26,    19,    24 },
                 {  3,    32,     7,    21,    23,    25 },
                 { 31,     9,     2,    22,    27,    20 },
                 {  8,    28,    33,    17,    10,    15 },
                 { 30,     5,    34,    12,    14,    16 },
                 {  4,    36,    29,    13,    18,    11 },
            };

            Assert.IsTrue(Matrix.IsEqual(actual, expected));

        }

        [Test]
        public void FindTest()
        {
            double[,] data =
            {
                { 1, 2, 0, 3 },
                { 1, 0, 1, 3 },
            };

            Func<double, bool> func = x => x == 0;
            bool firstOnly = false;
            int[][] expected =
            {
                new int[] { 0, 2 },
                new int[] { 1, 1 },
            };

            int[][] actual = Matrix.Find(data, func, firstOnly);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void InsertColumnTest()
        {
            double[,] m =
            {
                {  2, 10, 0 },
                {  0,  2, 4 },
            };

            double[] column = { 1, 1 };

            double[,] expected =
            {
                {  2, 1, 10, 0 },
                {  0, 1,  2, 4 },
            };

            double[,] actual = Matrix.InsertColumn(m, column, 1);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void InsertRowTest()
        {
            double[,] I = Matrix.Identity(3);
            double[] row = Matrix.Vector(3, new[] { 1.0, 1.0, 1.0 });

            double[,] expected;
            double[,] actual;


            expected = new double[,]
            {
                { 1, 1, 1 },
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 },
            };

            actual = Matrix.InsertRow(I, row, 0);
            Assert.IsTrue(actual.IsEqual(expected));


            expected = new double[,]
            {
                { 1, 0, 0 },
                { 1, 1, 1 },
                { 0, 1, 0 },
                { 0, 0, 1 },
            };

            actual = Matrix.InsertRow(I, row, 1);
            Assert.IsTrue(actual.IsEqual(expected));


            expected = new double[,]
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 1, 1, 1 },
                { 0, 0, 1 },
            };

            actual = Matrix.InsertRow(I, row, 2);
            Assert.IsTrue(actual.IsEqual(expected));


            expected = new double[,]
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 },
                { 1, 1, 1 },
            };

            actual = Matrix.InsertRow(I, row, 3);
            Assert.IsTrue(actual.IsEqual(expected));
        }

        [Test]
        public void InsertRowTest2()
        {
            double[,] a =
            {
               { 100.00, 27.56, 33.89},
               { 27.56, 100.00, 24.76},
               { 33.89, 24.76, 100.00}
             };

            Assert.AreEqual(3, a.GetLength(0));
            Assert.AreEqual(3, a.GetLength(1));

            double[,] b = a.InsertColumn(new double[] { 1, 2, 3, 100 });

            Assert.AreEqual(3, a.GetLength(0));
            Assert.AreEqual(3, a.GetLength(1));
            Assert.AreEqual(4, b.GetLength(0));
            Assert.AreEqual(4, b.GetLength(1));
            Assert.IsTrue(b.GetRow(3).IsEqual(new[] { 0, 0, 0, 100 }));

            double[,] c = a.InsertRow(new double[] { 1, 2, 3, 100 });

            Assert.AreEqual(3, a.GetLength(0));
            Assert.AreEqual(3, a.GetLength(1));
            Assert.AreEqual(4, c.GetLength(0));
            Assert.AreEqual(4, c.GetLength(1));
            Assert.IsTrue(c.GetColumn(3).IsEqual(new[] { 0, 0, 0, 100 }));

            a = a.InsertColumn(new double[] { 1, 2, 3 })
                 .InsertRow(new double[] { 1, 2, 3, 100 });

            Assert.AreEqual(4, a.GetLength(0));
            Assert.AreEqual(4, a.GetLength(1));
            Assert.IsTrue(a.GetRow(3).IsEqual(new[] { 1, 2, 3, 100 }));
            Assert.IsTrue(a.GetColumn(3).IsEqual(new[] { 1, 2, 3, 100 }));
        }

        [Test]
        public void InsertRowTest5()
        {
            double[][] a =
            {
               new double[] { 100.00, 27.56, 33.89},
               new double[] { 27.56, 100.00, 24.76},
               new double[] { 33.89, 24.76, 100.00}
             };

            Assert.AreEqual(3, a.Length);
            Assert.AreEqual(3, a[0].Length);

            double[][] b = a.InsertColumn(new double[] { 1, 2, 3, 100 });

            Assert.AreEqual(3, a.Length);
            Assert.AreEqual(3, a[0].Length);
            Assert.AreEqual(4, b.Length);
            Assert.AreEqual(4, b[0].Length);

            double[][] c = a.InsertRow(new double[] { 1, 2, 3, 100 });

            Assert.AreEqual(3, a.Length);
            Assert.AreEqual(3, a[0].Length);
            Assert.AreEqual(4, b.Length);
            Assert.AreEqual(4, b[0].Length);
            Assert.IsTrue(c.GetColumn(3).IsEqual(new[] { 0, 0, 0, 100 }));

            a = a.InsertColumn(new double[] { 1, 2, 3 })
                 .InsertRow(new double[] { 1, 2, 3, 100 });

            Assert.AreEqual(4, a.Length);
            Assert.AreEqual(4, a[0].Length);
            Assert.IsTrue(a.GetRow(3).IsEqual(new[] { 1, 2, 3, 100 }));
            Assert.IsTrue(a.GetColumn(3).IsEqual(new[] { 1, 2, 3, 100 }));
        }


        [Test]
        public void InsertRowTest3()
        {
            double[,] a =
            {
               { 100.00, 27.56, 33.89},
               { 27.56, 100.00, 24.76},
               { 33.89, 24.76, 100.00}
             };

            Assert.AreEqual(3, a.GetLength(0));
            Assert.AreEqual(3, a.GetLength(1));

            double[,] b = a.InsertColumn(new double[] { 1, 2, 3 });

            Assert.AreEqual(3, a.GetLength(0));
            Assert.AreEqual(3, a.GetLength(1));
            Assert.AreEqual(3, b.GetLength(0));
            Assert.AreEqual(4, b.GetLength(1));

            double[,] c = a.InsertRow(new double[] { 1, 2, 3 });

            Assert.AreEqual(3, a.GetLength(0));
            Assert.AreEqual(3, a.GetLength(1));
            Assert.AreEqual(4, c.GetLength(0));
            Assert.AreEqual(3, c.GetLength(1));

            a = a.InsertColumn(new double[] { 1, 2, 3 })
                 .InsertRow(new double[] { 1, 2, 3 });

            Assert.AreEqual(4, a.GetLength(0));
            Assert.AreEqual(4, a.GetLength(1));
        }

        [Test]
        public void InsertRowTest4()
        {
            double[][] a =
            {
               new double[] { 100.00, 27.56, 33.89},
               new double[] { 27.56, 100.00, 24.76},
               new double[] { 33.89, 24.76, 100.00}
             };

            Assert.AreEqual(3, a.Length);
            Assert.AreEqual(3, a[0].Length);

            double[][] b = a.InsertColumn(new double[] { 1, 2, 3 });

            Assert.AreEqual(3, a.Length);
            Assert.AreEqual(3, a[0].Length);
            Assert.AreEqual(3, b.Length);
            Assert.AreEqual(4, b[0].Length);

            double[][] c = a.InsertRow(new double[] { 1, 2, 3 });

            Assert.AreEqual(3, a.Length);
            Assert.AreEqual(3, a[0].Length);
            Assert.AreEqual(4, c.Length);
            Assert.AreEqual(3, c[0].Length);

            a = a.InsertColumn(new double[] { 1, 2, 3 })
                 .InsertRow(new double[] { 1, 2, 3 });

            Assert.AreEqual(4, a.Length);
            Assert.AreEqual(4, a[0].Length);
        }

        [Test]
        public void ConvolveTest()
        {
            double[] a = { 3, 4, 5 };
            double[] kernel = { 2, 1 };
            double[] expected = { 6, 11, 14, 5 };
            double[] actual = Matrix.Convolve(a, kernel);
            Assert.IsTrue(expected.IsEqual(actual));

            a = new double[] { 1, 2, 3, 4 };
            kernel = new double[] { 1, 2, 1 };
            expected = new double[] { 1, 4, 8, 12, 11, 4 };
            actual = Matrix.Convolve(a, kernel);
            Assert.IsTrue(expected.IsEqual(actual));

            a = new double[] { 1, 2, 3, 4 };
            kernel = new double[] { 0, 1, 0 };
            expected = new double[] { 0, 1, 2, 3, 4, 0 };
            actual = Matrix.Convolve(a, kernel);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void ConvolveTest2()
        {
            double[] a = { 3, 4, 5 };
            double[] kernel = { 2, 1 };
            double[] expected = { 6, 11, 14 };
            double[] actual = Matrix.Convolve(a, kernel, true);
            Assert.IsTrue(expected.IsEqual(actual));

            a = new double[] { 1, 2, 3, 4 };
            kernel = new double[] { 1, 2, 1 };
            expected = new double[] { 4, 8, 12, 11 };
            actual = Matrix.Convolve(a, kernel, true);
            Assert.IsTrue(expected.IsEqual(actual));

            a = new double[] { 1, 2, 3, 4 };
            kernel = new double[] { 0, 1, 0 };
            expected = new double[] { 1, 2, 3, 4 };
            actual = Matrix.Convolve(a, kernel, true);
            Assert.IsTrue(expected.IsEqual(actual));

            a = new double[] { 1, 2, 3, 4, 5, 6, 7 };
            kernel = new double[] { 0, 1, 0 };
            expected = new double[] { 1, 2, 3, 4, 5, 6, 7 };
            actual = Matrix.Convolve(a, kernel, true);
            Assert.IsTrue(expected.IsEqual(actual));

            a = new double[] { 1, 2 };
            kernel = new double[] { 0, 1, 0 };
            expected = new double[] { 1, 2 };
            actual = Matrix.Convolve(a, kernel, true);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void TensorProductTest()
        {
            double[,] a =
            {
                { 1, 2 },
                { 3, 4 },
            };

            double[,] b =
            {
                { 0, 5 },
                { 6, 7 },
            };

            double[,] expected =
            {
                {  0,  5,  0, 10 },
                {  6,  7, 12, 14 },
                {  0, 15,  0, 20 },
                { 18, 21, 24, 28 },
            };

            double[,] actual = Matrix.KroneckerProduct(a, b);
            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void TensorProductTest2()
        {
            double[] a = { 1, 2 };

            double[] b = { 4, 5, 6 };

            double[] expected = { 4, 5, 6, 8, 10, 12 };

            double[] actual = Matrix.KroneckerProduct(a, b);
            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }




        [Test]
        public void ConcatenateTest()
        {
            double[,] matrix =
            {
                { 1, 2 },
                { 3, 4 },
            };

            double[] vector = { 5, 6 };

            double[,] expected =
            {
                { 1, 2, 5 },
                { 3, 4, 6 },
            };


            double[,] actual = Matrix.Concatenate(matrix, vector);

            Assert.IsTrue(expected.IsEqual(actual));
        }



        [Test]
        public void MeshTest()
        {
            DoubleRange rowRange = new DoubleRange(-1, 1);
            DoubleRange colRange = new DoubleRange(-1, 1);
            double rowSteps = 0.5f;
            double colSteps = 0.5f;
            double[][] expected =
            {
                new double[] { -1.0, -1.0 },
                new double[] { -1.0, -0.5 },
                new double[] { -1.0,  0.0 },
                new double[] { -1.0,  0.5 },
                new double[] { -1.0,  1.0 },

                new double[] { -0.5, -1.0 },
                new double[] { -0.5, -0.5 },
                new double[] { -0.5,  0.0 },
                new double[] { -0.5,  0.5 },
                new double[] { -0.5,  1.0 },

                new double[] {  0.0, -1.0 },
                new double[] {  0.0, -0.5 },
                new double[] {  0.0,  0.0 },
                new double[] {  0.0,  0.5 },
                new double[] {  0.0,  1.0 },

                new double[] {  0.5, -1.0 },
                new double[] {  0.5, -0.5 },
                new double[] {  0.5,  0.0 },
                new double[] {  0.5,  0.5 },
                new double[] {  0.5,  1.0 },

                new double[] {  1.0, -1.0 },
                new double[] {  1.0, -0.5 },
                new double[] {  1.0,  0.0 },
                new double[] {  1.0,  0.5 },
                new double[] {  1.0,  1.0 },
            };

            double[][] actual = Matrix.Mesh(rowRange, colRange, rowSteps, colSteps);

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void MeshTest2()
        {
            // The Mesh method generates all possible (x,y) pairs
            // between two vector of points. For example, let's
            // suppose we have the values:
            //
            double[] a = { 0, 1 };
            double[] b = { 0, 1 };

            // We can create a grid as
            double[][] grid = a.Mesh(b);

            // result will be:
            //
            double[][] expected =
            {
                new double[] { 0, 0 },
                new double[] { 0, 1 },
                new double[] { 1, 0 },
                new double[] { 1, 1 },
            };

            Assert.IsTrue(expected.IsEqual(grid));
        }

        [Test]
        public void MeshTest3()
        {
            // The Mesh method can be used to generate all
            // possible (x,y) pairs between two ranges. 

            // We can create a grid as
            double[][] grid = Matrix.Mesh
            (
                rowMin: 0, rowMax: 1, rowStepSize: 0.3,
                colMin: 0, colMax: 1, colStepSize: 0.1
            );

            // Now we can plot the points on-screen
            // Accord.Controls.ScatterplotBox.Show("Grid (step size)", grid).Hold();

            Assert.AreEqual(55, grid.Length);
        }

        [Test]
        public void MeshTest4()
        {
            // The Mesh method can be used to generate all
            // possible (x,y) pairs between two ranges. 

            // We can create a grid as
            double[][] grid = Matrix.Mesh
            (
                rowMin: 0, rowMax: 1, rowSteps: 11,
                colMin: 0, colMax: 1, colSteps: 6
            );

            // Now we can plot the points on-screen
            // Accord.Controls.ScatterplotBox.Show("Grid (fixed steps)", grid).Hold();

            Assert.AreEqual(66, grid.Length);
        }

        [Test]
        public void MeshGridTest1()
        {
            // The MeshGrid method generates two matrices that can be
            // used to generate all possible (x,y) pairs between two
            // vector of points. For example, let's suppose we have
            // the values:
            //
            double[] a = { 1, 2, 3 };
            double[] b = { 4, 5, 6 };

            // We can create a grid
            var grid = a.MeshGrid(b);

            // get the x-axis values
            double[,] x = grid.Item1;

            // get the y-axis values
            double[,] y = grid.Item2;


            // we can either use those matrices separately (such as for plotting 
            // purposes) or we can also generate a grid of all the (x,y) pairs as
            //
            double[,][] xy = x.ApplyWithIndex((v, i, j) => new[] { x[i, j], y[i, j] });

            double[,] ex =
            {
                { 1, 1, 1 },
                { 2, 2, 2 },
                { 3, 3, 3 },
            };

            double[,] ey =
            {
                { 4, 5, 6 },
                { 4, 5, 6 },
                { 4, 5, 6 },
            };

            double[,,] expected =
            {
                { { 1, 4 }, { 1, 5 }, { 1, 6 } },
                { { 2, 4 }, { 2, 5 }, { 2, 6 } },
                { { 3, 4 }, { 3, 5 }, { 3, 6 } },
            };

            Assert.IsTrue(ex.IsEqual(x));
            Assert.IsTrue(ey.IsEqual(y));

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Assert.AreEqual(expected[i, j, 0], xy[i, j][0]);
                    Assert.AreEqual(expected[i, j, 1], xy[i, j][1]);
                }
            }
        }


        [Test]
        public void MultiplyByTransposeTestFloat()
        {
            float[,] a =
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
            };

            float[,] b =
            {
                {  7,  8,  9 },
                { 10, 11, 12 },
            };


            float[,] expected =
            {
                {  50, 68  },
                { 122, 167 }
            };

            float[,] actual = Matrix.MultiplyByTranspose(a, b);

            Assert.IsTrue(actual.IsEqual(expected));
        }



        [Test]
        public void ConcatenateTest1()
        {
            double[][,] matrices =
            {
                new double[,]
                {
                    { 0, 1 },
                    { 2, 3 },
                },

                new double[,]
                {
                    { 4, 5 },
                    { 6, 7 },
                },
            };


            double[,] expected =
            {
                { 0, 1, 4, 5 },
                { 2, 3, 6, 7 },
            };

            double[,] actual = Matrix.Concatenate(matrices);

            Assert.IsTrue(expected.IsEqual(actual));
        }


        [Test]
        public void RemoveColumnTest()
        {
            double[,] matrix =
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
            };


            double[,] a = matrix.RemoveColumn(0);
            double[,] b = matrix.RemoveColumn(1);
            double[,] c = matrix.RemoveColumn(2);

            double[,] expectedA =
            {
                { 2, 3 },
                { 5, 6 },
            };

            double[,] expectedB =
            {
                { 1, 3 },
                { 4, 6 },
            };

            double[,] expectedC =
            {
                { 1, 2 },
                { 4, 5 },
            };

            Assert.IsTrue(expectedA.IsEqual(a));
            Assert.IsTrue(expectedB.IsEqual(b));
            Assert.IsTrue(expectedC.IsEqual(c));
        }


        [Test]
        public void RemoveRowTest()
        {
            double[,] matrix =
            {
                { 1, 2 },
                { 3, 4 },
                { 5, 6 },
                { 7, 8 },
            };


            double[,] a = matrix.RemoveRow(0);
            double[,] b = matrix.RemoveRow(1);
            double[,] c = matrix.RemoveRow(3);

            double[,] expectedA =
            {
                { 3, 4 },
                { 5, 6 },
                { 7, 8 },
            };

            double[,] expectedB =
            {
                { 1, 2 },
                { 5, 6 },
                { 7, 8 },
            };

            double[,] expectedC =
            {
                { 1, 2 },
                { 3, 4 },
                { 5, 6 },
            };

            Assert.IsTrue(expectedA.IsEqual(a));
            Assert.IsTrue(expectedB.IsEqual(b));
            Assert.IsTrue(expectedC.IsEqual(c));
        }


        [Test]
        public void SolveTest1()
        {
            double[,] a =
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
            };

            double[,] b =
            {
                {  7,  8,  9 },
                { 10, 11, 12 },
            };

            // Test with more rows than columns
            {
                double[,] matrix = a.Transpose();
                double[,] rightSide = b.Transpose();

                Assert.IsTrue(matrix.GetLength(0) > matrix.GetLength(1));

                double[,] expected =
                {
                    { -1, -2 },
                    {  2,  3 }
                };


                double[,] actual = Matrix.Solve(matrix, rightSide);

                Assert.IsTrue(expected.IsEqual(actual, 1e-10));
            }

            // test with more columns than rows
            {
                double[,] matrix = a;
                double[,] rightSide = b;

                Assert.IsTrue(matrix.GetLength(0) < matrix.GetLength(1));


                double[,] expected =
                {
                    { -13/6.0,  -8/3.0, -19/6.0 },
                    {   2/6.0,   2/6.0,   2/6.0 },
                    {  17/6.0,  20/6.0,  23/6.0 }
                };

                double[,] actual = Matrix.Solve(matrix, rightSide);

                Assert.IsTrue(expected.IsEqual(actual, 1e-10));
            }
        }

        [Test]
        public void SolveTest4()
        {
            // Test with more rows than columns
            {
                double[,] matrix =
                {
                    { 1, 2 },
                    { 3, 4 },
                    { 5, 6 },
                };

                double[,] rightSide =
                {
                    { 7 },
                    { 8 },
                    { 9 },
                };

                Assert.IsTrue(matrix.GetLength(0) > matrix.GetLength(1));

                double[,] expected =
                {
                    { -6   },
                    {  6.5 }
                };

                double[,] actual = Matrix.Solve(matrix, rightSide);

                Assert.IsTrue(expected.IsEqual(actual, 1e-10));
            }

            // test with more columns than rows
            {
                double[,] matrix =
                {
                    { 1, 2, 3 },
                    { 4, 5, 6 },
                };

                double[,] rightSide =
                {
                    { 7 },
                    { 8 }
                };

                Assert.IsTrue(matrix.GetLength(0) < matrix.GetLength(1));


                double[,] expected =
                {
                   { -55 / 18.0 },
                   {  1  /  9.0 },
                   {  59 / 18.0 },
                };

                double[,] actual = Matrix.Solve(matrix, rightSide);

                Assert.IsTrue(expected.IsEqual(actual, 1e-10));
            }
        }

        [Test]
        public void SolveTest3()
        {
            // Test with more rows than columns
            {
                double[,] matrix =
                {
                    { 1, 2 },
                    { 3, 4 },
                    { 5, 6 },
                };

                double[] rightSide = { 7, 8, 9 };

                Assert.IsTrue(matrix.GetLength(0) > matrix.GetLength(1));

                double[] expected = { -6, 6.5 };

                double[] actual = Matrix.Solve(matrix, rightSide);

                Assert.IsTrue(expected.IsEqual(actual, 1e-10));
            }

            // test with more columns than rows
            {
                double[,] matrix =
                {
                    { 1, 2, 3 },
                    { 4, 5, 6 },
                };

                double[] rightSide = { 7, 8 };

                Assert.IsTrue(matrix.GetLength(0) < matrix.GetLength(1));


                double[] expected = { -55 / 18.0, 1 / 9.0, 59 / 18.0 };

                double[] actual = Matrix.Solve(matrix, rightSide);

                Assert.IsTrue(expected.IsEqual(actual, 1e-10));
            }
        }

        [Test]
        public void SolveTest5()
        {
            // Test with singular matrix
            {
                // Create a matrix. Please note that this matrix
                // is singular (i.e. not invertible), so only a 
                // least squares solution would be feasible here.

                double[,] matrix =
                {
                    { 1, 2, 3 },
                    { 4, 5, 6 },
                    { 7, 8, 9 },
                };

                // Define a right side vector b:
                double[] rightSide = { 1, 2, 3 };

                // Solve the linear system Ax = b by finding x:
                double[] x = Matrix.Solve(matrix, rightSide, leastSquares: true);

                // The answer should be { -1/18, 2/18, 5/18 }.

                double[] expected = { -1 / 18.0, 2 / 18.0, 5 / 18.0 };
                Assert.IsTrue(matrix.IsSingular());
                Assert.IsTrue(expected.IsEqual(x, 1e-10));
            }
            {
                double[,] matrix =
                {
                    { 1, 2, 3 },
                    { 4, 5, 6 },
                    { 7, 8, 9 },
                };

                double[,] rightSide = { { 1 }, { 2 }, { 3 } };

                Assert.IsTrue(matrix.IsSingular());

                double[,] expected = { { -1 / 18.0 }, { 2 / 18.0 }, { 5 / 18.0 } };

                double[,] actual = Matrix.Solve(matrix, rightSide, leastSquares: true);

                Assert.IsTrue(expected.IsEqual(actual, 1e-10));
            }
        }

        [Test]
        public void TopBottomTest()
        {
            double[] values = { 9, 3, 6, 3, 1, 8, 4, 1, 8, 4, 4, 1, 0, -2, 4 };

            {
                int[] idx = values.Top(5);
                double[] selected = values.Submatrix(idx);
                Assert.AreEqual(5, idx.Length);
                Assert.AreEqual(9, selected[0]);
                Assert.AreEqual(8, selected[1]);
                Assert.AreEqual(8, selected[2]);
                Assert.AreEqual(6, selected[3]);
                Assert.AreEqual(4, selected[4]);
            }

            {
                int[] idx = values.Bottom(5);
                double[] selected = values.Submatrix(idx);
                Assert.AreEqual(5, idx.Length);
                Assert.AreEqual(-2, selected[0]);
                Assert.AreEqual(0, selected[1]);
                Assert.AreEqual(1, selected[2]);
                Assert.AreEqual(1, selected[3]);
                Assert.AreEqual(1, selected[4]);
            }

        }

        [Test]
        public void TopBottomTest2()
        {
            for (int i = 0; i < 10; i++)
            {
                double[] values = Matrix.Random(20, -1.0, 1.0);

                for (int k = 1; k < 11; k++)
                {
                    double[] actualTop = values.Submatrix(values.Top(k));
                    double[] actualBottom = values.Submatrix(values.Bottom(k));

                    Array.Sort(values);

                    double[] expectedTop = values.Get(values.Length - k, values.Length);
                    double[] expectedBottom = values.Get(0, k);

                    Assert.AreEqual(k, actualTop.Length);
                    Assert.AreEqual(k, actualBottom.Length);
                    Assert.AreEqual(expectedTop.Length, actualTop.Length);
                    Assert.AreEqual(expectedBottom.Length, actualBottom.Length);

                    foreach (var v in actualTop)
                        Assert.IsTrue(expectedTop.Contains(v));

                    foreach (var v in actualBottom)
                        Assert.IsTrue(expectedBottom.Contains(v));
                }
            }

        }


        [Test]
        public void GetIndicesTest()
        {
            double[,] v = Matrix.Ones(2, 3);
            int[][] idx = v.GetIndices().ToArray();
            Assert.IsTrue(idx.IsEqual(Jagged.Create(new[,]
                {
                    {0, 0},
                    {0, 1},
                    {0, 2},
                    {1, 0},
                    {1, 1},
                    {1, 2},
                })));
        }

        [Test]
        public void GetIndicesTest2()
        {
            double[,] v = Matrix.Ones(2, 0);
            int[][] idx = v.GetIndices().ToArray();
            Assert.AreEqual(idx.Length, 0);
        }

        [Test]
        public void GetIndicesTest3()
        {
            double[,] v = Matrix.Ones(0, 3);
            int[][] idx = v.GetIndices().ToArray();
            Assert.AreEqual(idx.Length, 0);
        }

        [Test]
        public void GetIndicesTest4()
        {
            double[][] v = Jagged.Ones(0, 3);
            int[][] idx = v.GetIndices().ToArray();
            Assert.AreEqual(idx.Length, 0);
        }

        [Test]
        public void find_test_1()
        {
            int[] a = { 5, 1, 10, 5 };
            Assert.AreEqual(new[] { 0, 3 }, a.Find(x => x == 5));
            Assert.AreEqual(1, a.Find(x => x == 1)[0]);
            Assert.AreEqual(2, a.Find(x => x == 10)[0]);
            Assert.AreEqual(0, a.Find(x => x == 0).Length);
            Assert.AreEqual(0, a.Find(x => x == 2).Length);
            Assert.AreEqual(0, a.Find(x => x == 11).Length);
        }

        [Test]
        public void first_test()
        {
            int[] a = { 5, 1, 10 };
            Assert.AreEqual(0, a.First(x => x == 5));
            Assert.AreEqual(1, a.First(x => x == 1));
            Assert.AreEqual(2, a.First(x => x == 10));
            Assert.Throws<IndexOutOfRangeException>(() => a.First(x => x == 0));
            Assert.Throws<IndexOutOfRangeException>(() => a.First(x => x == 2));
            Assert.Throws<IndexOutOfRangeException>(() => a.First(x => x == 11));
        }

        [Test]
        public void first_or_default_test()
        {
            int[] a = { 5, 1, 10 };
            Assert.AreEqual(0, a.FirstOrNull(x => x == 5));
            Assert.AreEqual(1, a.FirstOrNull(x => x == 1));
            Assert.AreEqual(2, a.FirstOrNull(x => x == 10));
            Assert.IsNull(a.FirstOrNull(x => x == 0));
            Assert.IsNull(a.FirstOrNull(x => x == 2));
            Assert.IsNull(a.FirstOrNull(x => x == 11));
        }
    }
}
