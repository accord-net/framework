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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Filters;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Data;
    using Accord;
    using Accord.Math;
    using Accord.Controls;

    [TestClass()]
    public class ProjectionFilterTest
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

        public static DataTable CreateTable()
        {
            DataTable table = new DataTable("Buildings");
            table.Columns.Add("Id", "Floors", "Finished", "Category", "Cost (M)");

            table.Rows.Add(0, 19, false, "A", 212.522);
            table.Rows.Add(1, 5, false, "B", 4.124);
            table.Rows.Add(2, 7, true, "B", 2.683);
            table.Rows.Add(3, 5, true, "A", 3.021);
            table.Rows.Add(4, 2, false, "C", 2.151);

            return table;
        }


        [TestMethod()]
        public void ApplyTest()
        {
            DataTable table = CreateTable();

            // Show the start data
            // DataGridBox.Show(table);

            // Create a new data projection (column) filter
            var filter = new Projection("Floors", "Finished");

            // Apply the filter and get the result
            DataTable result = filter.Apply(table);

            // Show it
            // DataGridBox.Show(result);

            Assert.AreEqual(2, result.Columns.Count);
            Assert.AreEqual(5, result.Rows.Count);

            Assert.AreEqual("Floors", result.Columns[0].ColumnName);
            Assert.AreEqual("Finished", result.Columns[1].ColumnName);
        }
    }
}
