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

namespace Accord.Tests.IO
{
    using Accord.IO;
    using Accord.Math;
    using NUnit.Framework;
    using System;
    using System.Data;
    using System.IO;

    [TestFixture]
    public class CsvWriterTest
    {

        [Test]
        public void write_matrix()
        {
            // GH-663: Please add an example CsvWriter Class
            // https://github.com/accord-net/framework/issues/663

            #region doc_matrix
            // Tell where to store the matrix (adjust to your own liking)
            string filename = Path.Combine(Path.GetTempPath(), "matrix.csv");

            // Let's say we want to store a multidimensional array
            double[,] values =
            {
                { 1,  2,  3,  4 },
                { 5,  6,  7,  8 },
                { 9, 10, 11, 12 },
            };

            // Create a new writer and write the values to disk
            using (CsvWriter writer = new CsvWriter(filename))
            {
                writer.WriteHeaders("a", "b", "c", "d"); // this is optional
                writer.Write(values);
            }

            // Afterwards, we could read them back using
            var reader = new CsvReader(filename, hasHeaders: true);
            double[,] sameMatrix = reader.ToMatrix();
            #endregion

            Assert.IsTrue(values.IsEqual(sameMatrix));
        }

        [Test]
        public void write_jagged()
        {
            // GH-663: Please add an example CsvWriter Class
            // https://github.com/accord-net/framework/issues/663

            #region doc_jagged
            // Tell where to store the matrix (adjust to your own liking)
            string filename = Path.Combine(Path.GetTempPath(), "jagged.csv");

            // Let's say we want to store a jagged array
            double[][] values =
            {
                new double[] { 1,  2,  3,  4 },
                new double[] { 5,  6,  7,  8 },
                new double[] { 9, 10, 11, 12 },
            };

            // Create a new writer and write the values to disk
            using (CsvWriter writer = new CsvWriter(filename))
            {
                writer.WriteHeaders("a", "b", "c", "d"); // this is optional
                writer.Write(values);
            }

            // Afterwards, we could read them back using
            var reader = new CsvReader(filename, hasHeaders: true); 
            double[][] sameMatrix = reader.ToJagged();
            #endregion

            Assert.IsTrue(values.IsEqual(sameMatrix));

            double[] lastColumn = sameMatrix.GetColumn(-1);
            double[][] firstColumns = sameMatrix.Get(null, 0, -1);
            Assert.AreEqual(lastColumn, new[] { 4.0, 8.0, 12 });
            Assert.IsTrue(firstColumns.IsEqual(new []
                {
                    new[] { 1, 2, 3.0   },
                    new[] { 5, 6, 7.0   },
                    new[] { 9, 10, 11.0 },
                }));
        }

#if !NO_DATA_TABLE
        [Test]
        public void write_table()
        {
            // GH-663: Please add an example CsvWriter Class
            // https://github.com/accord-net/framework/issues/663

            #region doc_table
            // Tell where to store the data table (adjust to your own liking)
            string filename = Path.Combine(Path.GetTempPath(), "table.csv");

            // Let's say we have the following data table
            DataTable table = new DataTable("My table");
            table.Columns.Add("Name");
            table.Columns.Add("Age");
            table.Columns.Add("City");

            // Let's add some rows for these columns
            table.Rows.Add("John", 42, "New York");
            table.Rows.Add("Josephine", 25, "Grenoble");
            table.Rows.Add("João", 22, "Valinhos");

            // Create a new writer and write the values to disk
            using (CsvWriter writer = new CsvWriter(filename))
            {
                writer.Write(table);
            }

            // Later, we could read it back from the disk using
            var reader = new CsvReader(filename, hasHeaders: true);
            DataTable sameTable = reader.ToTable();
            #endregion 

            string text = new StreamReader(filename).ReadToEnd();
            
            string expected = 
                "\"Name\",\"Age\",\"City\"\r\n" +
                "\"John\",\"42\",\"New York\"\r\n" +
                "\"Josephine\",\"25\",\"Grenoble\"\r\n" +
                "\"João\",\"22\",\"Valinhos\"\r\n";

            Assert.AreEqual(text, expected.Replace("\r\n", Environment.NewLine));

            Assert.IsTrue(table.ToMatrix<string>().IsEqual(sameTable.ToMatrix<string>()));
        }
#endif

        [Test]
        public void write_objects()
        {
            #region doc_objects
            // Tell where to store the matrix (adjust to your own liking)
            string filename = Path.Combine(Path.GetTempPath(), "objects.csv");

            // Let's say we want to store a jagged array of mixed types
            object[][] values =
            {
                new object[] { "a",    2,    3,   4 },
                new object[] {   5,  "b",    7,   8 },
                new object[] {   9,   10,  "c",  12 },
            };

            // Create a new writer and write the values to disk
            using (CsvWriter writer = new CsvWriter(filename))
            {
                writer.WriteHeaders("a", "b", "c", "d"); // this is optional
                writer.Write(values);
            }

            // Later, we could read it back from the disk using
            var reader = new CsvReader(filename, hasHeaders: true);
            object[][] sameMatrix = reader.ToJagged<object>();
            #endregion

            string[][] expected = values.Apply((x, i, j) => x.ToString());
            Assert.IsTrue(expected.IsEqual(sameMatrix));
        }
    }

}
