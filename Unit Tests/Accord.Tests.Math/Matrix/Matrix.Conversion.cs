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
    using Accord.Math;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System;
    using System.Data;
    using AForge;

    public partial class MatrixTest
    {

        [TestMethod()]
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

        [TestMethod()]
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

    }
}
