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
            string filename = Path.Combine(Path.GetTempPath(), "matrix.csv");

            double[,] values =
            {
                { 1,  2,  3,  4 },
                { 5,  6,  7,  8 },
                { 9, 10, 11, 12 },
            };

            using (CsvWriter writer = new CsvWriter(filename))
            {

                writer.WriteHeaders("a", "b", "c", "d");
                writer.Write(values);
            }
            #endregion doc_matrix

            CsvReader reader = new CsvReader(filename, hasHeaders: true);
            double[,] actual = reader.ToMatrix();
            Assert.IsTrue(values.IsEqual(actual));
        }

        [Test]
        public void write_table()
        {
            // GH-663: Please add an example CsvWriter Class
            // https://github.com/accord-net/framework/issues/663

            #region doc_matrix
            string filename = Path.Combine(Path.GetTempPath(), "table.csv");

            DataTable table = new DataTable("My table");
            table.Columns.Add("Name");
            table.Columns.Add("Age");
            table.Columns.Add("City");

            table.Rows.Add("John", 42, "New York");
            table.Rows.Add("Josephine", 25, "Grenoble");
            table.Rows.Add("João", 22, "Valinhos");

            using (CsvWriter writer = new CsvWriter(filename))
            {
                writer.Write(table);
            }
            #endregion doc_table

            string text = new StreamReader(filename).ReadToEnd();
            Assert.AreEqual(text,
"\"Name\",\"Age\",\"City\"\r\n" +
"\"John\",\"42\",\"New York\"\r\n" +
"\"Josephine\",\"25\",\"Grenoble\"\r\n" +
"\"João\",\"22\",\"Valinhos\"\r\n");

            CsvReader reader = new CsvReader(filename, hasHeaders: true);
            DataTable actual = reader.ToTable();
            Assert.IsTrue(table.ToMatrix<string>().IsEqual(actual.ToMatrix<string>()));
        }
    }

}
