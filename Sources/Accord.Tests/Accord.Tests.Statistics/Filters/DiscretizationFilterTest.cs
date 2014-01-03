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
    using System;
    using Accord.Controls;

    [TestClass()]
    public class DiscretizationFilterTest
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
        public void ApplyTest1()
        {
            DataTable table = ProjectionFilterTest.CreateTable();

            // Show the start data
            // DataGridBox.Show(table);

            // Create a new data projection (column) filter
            var filter = new Discretization("Cost (M)");

            // Apply the filter and get the result
            DataTable result = filter.Apply(table);

            // Show it
            // DataGridBox.Show(result);

            Assert.AreEqual(5, result.Columns.Count);
            Assert.AreEqual(5, result.Rows.Count);

            Assert.AreEqual("213", result.Rows[0]["Cost (M)"]);
            Assert.AreEqual("4", result.Rows[1]["Cost (M)"]);
            Assert.AreEqual("3", result.Rows[2]["Cost (M)"]);
            Assert.AreEqual("3", result.Rows[3]["Cost (M)"]);
            Assert.AreEqual("2", result.Rows[4]["Cost (M)"]);
        }

        [TestMethod()]
        public void ApplyTest()
        {
            DataTable input = new DataTable("Sample data");
            input.Columns.Add("x", typeof(double));
            input.Columns.Add("y", typeof(double));
            input.Columns.Add("z", typeof(double));

            input.Rows.Add(0.02, 60.6, 24.2);
            input.Rows.Add(0.92, 50.2, 21.1);
            input.Rows.Add(0.32, 60.9, 19.8);
            input.Rows.Add(2.02, 61.8, 92.4);


            // Create a discretization filter to operate on the first 2 columns
            Discretization target = new Discretization("x", "y");
            target.Columns["y"].Threshold = 0.8;

            DataTable expected = new DataTable("Sample data");
            expected.Columns.Add("x", typeof(double));
            expected.Columns.Add("y", typeof(double));
            expected.Columns.Add("z", typeof(double));

            expected.Rows.Add(0, 60, 24.2);
            expected.Rows.Add(1, 50, 21.1);
            expected.Rows.Add(0, 61, 19.8);
            expected.Rows.Add(2, 62, 92.4);


            DataTable actual = target.Apply(input);

            for (int i = 0; i < actual.Rows.Count; i++)
            {
                double ex = (double)expected.Rows[i][0];
                double ey = (double)expected.Rows[i][1];
                double ez = (double)expected.Rows[i][2];

                double ax = (double)actual.Rows[i][0];
                double ay = (double)actual.Rows[i][1];
                double az = (double)actual.Rows[i][2];

                Assert.AreEqual(ex, ax);
                Assert.AreEqual(ey, ay);
                Assert.AreEqual(ez, az);
            }

        }
    }
}
