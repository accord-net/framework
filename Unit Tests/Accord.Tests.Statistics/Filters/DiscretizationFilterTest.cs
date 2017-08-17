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

namespace Accord.Tests.Statistics
{
    using System.Data;
    using Accord.Statistics.Filters;
    using NUnit.Framework;
    using System;
    using Accord.Math;

    [TestFixture]
    public class DiscretizationFilterTest
    {
#if !NO_DATA_TABLE
        [Test]
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

        [Test]
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



        [Test]
        public void rule_matching_test()
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
            var target = new Discretization<double, int>("x", "y");
            target.Columns["x"].Mapping[x => true] = x => (int)System.Math.Round(x, MidpointRounding.AwayFromZero);
            target.Columns["y"].Mapping[x => true] = x => ((x - (int)x) >= 0.7999999999999) ? ((int)x + 1) : (int)x;

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

                double ax = (int)actual.Rows[i][0];
                double ay = (int)actual.Rows[i][1];
                double az = (double)actual.Rows[i][2];

                Assert.AreEqual(ex, ax);
                Assert.AreEqual(ey, ay);
                Assert.AreEqual(ez, az);
            }
        }
#endif

        [Test]
        public void thresholds()
        {
            // Example for https://github.com/accord-net/framework/issues/737

            #region doc_percentage
            // Let's say we have some data representing the probability
            // of seeing some particular species of birds across the U.S.:
            string[] names = { "State", "Bird", "Percentage" };

            object[][] data =
            {
                new object[] { "Kansas", "Crow", 0.1 },
                new object[] { "Ohio", "Pardal", 0.5 },
                new object[] { "Hawaii", "Penguim", 0.7 }
            };

            // Create a new discretization filter for the given dataset:
            var discretization = new Discretization<double, string>(names)
            {
                { "Percentage", x => x >= 0.00 && x < 0.25, "lowest" },
                { "Percentage", x => x >= 0.25 && x < 0.50, "low" },
                { "Percentage", x => x >= 0.50 && x < 0.75, "medium" },
                { "Percentage", x => x >= 0.75 && x < 1.00, "likely" },
            };

            // Convert the data using the above conversion rules:
            object[][] output = discretization.Transform(data);

            // The output should be:
            object[][] expected =
            {
                new object[] { "Kansas", "Crow", "lowest" },
                new object[] { "Ohio", "Pardal", "medium" },
                new object[] { "Hawaii", "Penguim", "medium" }
            };
            #endregion

            Assert.IsTrue(output.IsEqual(expected));
        }

#if !NO_DATA_TABLE
        [Test]
        public void missing_values_thresholds_test()
        {
            DataTable input = new DataTable("Tennis Example with Missing Values");
            input.Columns.Add("Day", typeof(string));
            input.Columns.Add("Outlook", typeof(string));
            input.Columns.Add("Temperature", typeof(int));
            input.Columns.Add("Humidity", typeof(string));
            input.Columns.Add("Wind", typeof(string));
            input.Columns.Add("PlayTennis", typeof(string));
            input.Rows.Add("D1", "Sunny", 35, "High", "Weak", "No");
            input.Rows.Add("D2", null, 32, "High", "Strong", "No");
            input.Rows.Add("D3", null, null, "High", null, "Yes");
            input.Rows.Add("D4", "Rain", 25, "High", "Weak", "Yes");
            input.Rows.Add("D5", "Rain", 16, null, "Weak", "Yes");
            input.Rows.Add("D6", "Rain", 12, "Normal", "Strong", "No");
            input.Rows.Add("D7", "Overcast", "18", "Normal", "Strong", "Yes");
            input.Rows.Add("D8", null, 27, "High", null, "No");
            input.Rows.Add("D9", null, 17, "Normal", "Weak", "Yes");
            input.Rows.Add("D10", null, null, "Normal", null, "Yes");
            input.Rows.Add("D11", null, 23, "Normal", null, "Yes");
            input.Rows.Add("D12", "Overcast", 25, null, "Strong", "Yes");
            input.Rows.Add("D13", "Overcast", 33, null, "Weak", "Yes");
            input.Rows.Add("D14", "Rain", 24, "High", "Strong", "No");

            Assert.AreEqual(14, input.Rows.Count);
            Assert.AreEqual(6, input.Columns.Count);

            var discretization = new Discretization<double, string>()
            {
                { "Temperature", x => x >= 30 && x < 50, "Hot" },
                { "Temperature", x => x >= 20 && x < 30, "Mild" },
                { "Temperature", x => x >= 00 && x < 20, "Cool" },
            };

            DataTable actual = discretization.Apply(input);
            Assert.AreEqual(14, actual.Rows.Count);
            Assert.AreEqual(6, actual.Columns.Count);


            DataTable expected = new DataTable("Tennis Example with Missing Values");
            expected.Columns.Add("Day", typeof(string));
            expected.Columns.Add("Outlook", typeof(string));
            expected.Columns.Add("Temperature", typeof(string));
            expected.Columns.Add("Humidity", typeof(string));
            expected.Columns.Add("Wind", typeof(string));
            expected.Columns.Add("PlayTennis", typeof(string));
            expected.Rows.Add("D1", "Sunny", "Hot", "High", "Weak", "No");
            expected.Rows.Add("D2", null, "Hot", "High", "Strong", "No");
            expected.Rows.Add("D3", null, null, "High", null, "Yes");
            expected.Rows.Add("D4", "Rain", "Mild", "High", "Weak", "Yes");
            expected.Rows.Add("D5", "Rain", "Cool", null, "Weak", "Yes");
            expected.Rows.Add("D6", "Rain", "Cool", "Normal", "Strong", "No");
            expected.Rows.Add("D7", "Overcast", "Cool", "Normal", "Strong", "Yes");
            expected.Rows.Add("D8", null, "Mild", "High", null, "No");
            expected.Rows.Add("D9", null, "Cool", "Normal", "Weak", "Yes");
            expected.Rows.Add("D10", null, null, "Normal", null, "Yes");
            expected.Rows.Add("D11", null, "Mild", "Normal", null, "Yes");
            expected.Rows.Add("D12", "Overcast", "Mild", null, "Strong", "Yes");
            expected.Rows.Add("D13", "Overcast", "Hot", null, "Weak", "Yes");
            expected.Rows.Add("D14", "Rain", "Mild", "High", "Strong", "No");


            for (int j = 0; j < expected.Rows.Count; j++)
            {
                var erow = expected.Rows[j];
                var arow = actual.Rows[j];

                for (int i = 0; i < expected.Columns.Count; i++)
                {
                    object e = erow[i];
                    object a = arow[i];
                    Assert.AreEqual(e, a);
                }
            }
        }
#endif
    }
}
