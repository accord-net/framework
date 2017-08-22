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
    using Accord.Math;
    using NUnit.Framework;
    using System;
    using System.Data;

    public partial class MatrixTest
    {
#if !NO_DATA_TABLE
        [Test]
        public void ToTableTest()
        {
            double[,] matrix =
            {
                { 1, 2 },
                { 3, 4 },
                { 5, 6 },
            };

            string[] columnNames = { "A", "B", };
            DataTable actual = Matrix.ToTable(matrix, columnNames);

            Assert.AreEqual("A", actual.Columns[0].ColumnName);
            Assert.AreEqual("B", actual.Columns[1].ColumnName);

            Assert.AreEqual(1, (double)actual.Rows[0][0]);
            Assert.AreEqual(2, (double)actual.Rows[0][1]);
            Assert.AreEqual(3, (double)actual.Rows[1][0]);
            Assert.AreEqual(4, (double)actual.Rows[1][1]);
            Assert.AreEqual(5, (double)actual.Rows[2][0]);
            Assert.AreEqual(6, (double)actual.Rows[2][1]);
        }

        [Test]
        public void ToTableTest2()
        {
            double[][] matrix =
            {
                new double[] { 1, 2 },
                new double[] { 3, 4 },
                new double[] { 5, 6 },
            };

            DataTable actual = Matrix.ToTable(matrix);

            Assert.AreEqual(1, (double)actual.Rows[0][0]);
            Assert.AreEqual(2, (double)actual.Rows[0][1]);
            Assert.AreEqual(3, (double)actual.Rows[1][0]);
            Assert.AreEqual(4, (double)actual.Rows[1][1]);
            Assert.AreEqual(5, (double)actual.Rows[2][0]);
            Assert.AreEqual(6, (double)actual.Rows[2][1]);
        }

        [Test]
        public void FromTableToArrayTest()
        {
            DataTable table = new DataTable();
            table.Columns.Add("A", typeof(bool));
            table.Columns.Add("B", typeof(string));
            table.Rows.Add(true, "1.0");
            table.Rows.Add(true, "0");
            table.Rows.Add(false, "1");
            table.Rows.Add(false, "0.0");

            double[][] actual = table.ToArray(System.Globalization.CultureInfo.InvariantCulture);
            double[][] expected =
            {
                new double[] { 1, 1 },
                new double[] { 1, 0 },
                new double[] { 0, 1 },
                new double[] { 0, 0 },
            };

            Assert.IsTrue(expected.IsEqual(actual));
        }
#endif

        [Test]
        public void FromJaggedToMultidimensional()
        {
            // Declare a jagged matrix that we would like to convert to multi-dimensional
            int[][][] jagged = new[]
            {
                new[]
                {
                    new[] { 1, 2, 3 },
                    new[] { 4, 5, 6 }
                },

                new[]
                {
                    new[] { 7, 8, 9 },
                    new[] { 10, 11, 12 }
                },

                new[]
                {
                    new[] { 13, 14, 15 },
                    new[] { 16, 17, 18 }
                },

                new[]
                {
                    new[] { 19, 20, 21 },
                    new[] { 22, 23, 24 }
                }
            };

            // Test #1: Transform jagged matrix to a unidimensional vector. The extension method called DeepFlatten is available at:
            // https://github.com/accord-net/framework/blob/a195ce7afbd2fd2ae143a82f5214a08e2a1a2a07/Sources/Accord.Math/Matrix/Matrix.Common.cs#L1636
            Array values = jagged.DeepFlatten();

            // As a result, the resulting array is a simple int[] vector:
            Assert.AreEqual(values.GetType(), typeof(int[]));

            // Some more checks
            int[] innerValues = values as int[];
            Assert.AreEqual(24, innerValues.Length);
            Assert.AreEqual(new[] { 24 }, innerValues.GetLength());


            // Test #2: Transform the jagged matrix to a multidimensional matrix. The extension method called DeepToMatrix is available at:
            // https://github.com/accord-net/framework/blob/a195ce7afbd2fd2ae143a82f5214a08e2a1a2a07/Sources/Accord.Math/Matrix/Matrix.Conversions.cs#L39
            Array matrix = jagged.DeepToMatrix();

            // As a result, the resulting array is a multidimensional int[,,] array:
            Assert.AreEqual(matrix.GetType(), typeof(int[,,]));

            // Some more checks
            int[,,] innerMatrix = matrix as int[,,];
            int[] shape = innerMatrix.GetLength();
            Assert.AreEqual(new[] { 4, 2, 3 }, shape);


            int[,,] expected =
           {
                {
                    { 1, 2, 3 },
                    { 4, 5, 6 }
                },
                {
                    { 7, 8, 9 },
                    { 10, 11, 12 }
                },
                {
                    { 13, 14, 15 },
                    { 16, 17, 18 }
                },
                {
                    { 19, 20, 21 },
                    { 22, 23, 24 }
                }
            };

            for (int i = 0; i < jagged.Length; i++)
                for (int j = 0; j < jagged[i].Length; j++)
                    for (int k = 0; k < jagged[i][j].Length; k++)
                        Assert.AreEqual(jagged[i][j][k], expected[i, j, k]);
        }

    }
}
