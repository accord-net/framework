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
    using Accord.Collections;
    using Accord.IO;
    using Accord.Math;
    using Accord.Statistics;
    using Accord.Statistics.Filters;
    using NUnit.Framework;
    using System;
    using System.Collections.Specialized;
    using System.Data;
    using System.IO;
    using System.Linq.Expressions;

    [TestFixture]
    public class InputationFilterTest
    {

#if !NO_DATA_TABLE
        [Test]
        public void ApplyTest1()
        {
            DataTable table = new DataTable("Buildings");
            table.Columns.Add(new OrderedDictionary<string, Type>
            {
                { "Id",       typeof(int) },
                { "Floors",   typeof(int) },
                { "Finished", typeof(bool) },
                { "Category", typeof(string) },
                { "Cost (M)", typeof(double) },
                { "Extra",    typeof(decimal) }
            });

            table.Rows.Add(0, 19, false, "A", 212.522, 1);
            table.Rows.Add(1, 5, false, "B", 4.124, 2);
            table.Rows.Add(-1, -1, true, "B", 2.683, 3);
            table.Rows.Add(3, 5, true, null, Double.NaN, 4);
            table.Rows.Add(4, -1, false, "C", 2.151, 5);

            Assert.AreEqual(0, table.Rows[0][0]);
            Assert.AreEqual(-1, table.Rows[2][0]);

            // Create a new data projection (column) filter
            var filter = new Imputation("Id", "Floors", "Finished", "Category", "Cost (M)");

            Assert.AreEqual(ImputationStrategy.FixedValue, filter["Id"].Strategy);
            Assert.AreEqual(ImputationStrategy.FixedValue, filter["Floors"].Strategy);
            Assert.AreEqual(ImputationStrategy.FixedValue, filter["Finished"].Strategy);
            Assert.AreEqual(ImputationStrategy.FixedValue, filter["Category"].Strategy);
            Assert.AreEqual(ImputationStrategy.FixedValue, filter["Cost (M)"].Strategy);
            Assert.IsFalse(filter.Columns.Contains("Extra"));

            filter["Floors"].Strategy = ImputationStrategy.FixedValue;
            filter["Floors"].ReplaceWith = 42;

            filter["Category"].Strategy = ImputationStrategy.Mode;
            filter["Cost (M)"].Strategy = ImputationStrategy.Median;

            filter.Learn(table);

            Assert.AreEqual(ImputationStrategy.FixedValue, filter["Floors"].Strategy);
            Assert.AreEqual(-1, filter["Floors"].MissingValue);
            Assert.AreEqual(42, filter["Floors"].ReplaceWith);

            Assert.AreEqual(ImputationStrategy.Mode, filter["Category"].Strategy);
            Assert.AreEqual("B", filter["Category"].ReplaceWith);

            Assert.AreEqual(ImputationStrategy.Median, filter["Cost (M)"].Strategy);
            Assert.AreEqual(3.4034999999999997d, (double)filter["Cost (M)"].ReplaceWith, 1e-10);

            Assert.AreEqual(6, filter.Columns.Count);

            DataTable result = filter.Apply(table);

            Assert.AreEqual(6, result.Columns.Count);
            Assert.AreEqual(5, result.Rows.Count);

            Assert.AreEqual(0, result.Rows[0]["Id"]);
            Assert.AreEqual(1, result.Rows[1]["Id"]);
            Assert.AreEqual(-1, result.Rows[2]["Id"]);
            Assert.AreEqual(3, result.Rows[3]["Id"]);
            Assert.AreEqual(4, result.Rows[4]["Id"]);

            Assert.AreEqual(19, result.Rows[0]["Floors"]);
            Assert.AreEqual(5, result.Rows[1]["Floors"]);
            Assert.AreEqual(42, result.Rows[2]["Floors"]);
            Assert.AreEqual(5, result.Rows[3]["Floors"]);
            Assert.AreEqual(42, result.Rows[4]["Floors"]);

            Assert.AreEqual(false, result.Rows[0]["Finished"]);
            Assert.AreEqual(false, result.Rows[1]["Finished"]);
            Assert.AreEqual(true, result.Rows[2]["Finished"]);
            Assert.AreEqual(true, result.Rows[3]["Finished"]);
            Assert.AreEqual(false, result.Rows[4]["Finished"]);

            Assert.AreEqual("A", result.Rows[0]["Category"]);
            Assert.AreEqual("B", result.Rows[1]["Category"]);
            Assert.AreEqual("B", result.Rows[2]["Category"]);
            Assert.AreEqual("B", result.Rows[3]["Category"]);
            Assert.AreEqual("C", result.Rows[4]["Category"]);

            Assert.AreEqual(212.52199999999999d, (double)result.Rows[0]["Cost (M)"], 1e-10);
            Assert.AreEqual(4.1239999999999997d, (double)result.Rows[1]["Cost (M)"], 1e-10);
            Assert.AreEqual(2.6829999999999998d, (double)result.Rows[2]["Cost (M)"], 1e-10);
            Assert.AreEqual(3.4034999999999997d, (double)result.Rows[3]["Cost (M)"], 1e-10);
            Assert.AreEqual(2.1509999999999998d, (double)result.Rows[4]["Cost (M)"], 1e-10);
        }

        [Test]
        public void ApplyTest()
        {
            Imputation target = new Imputation();
            target["Classification"].Strategy = ImputationStrategy.Mode;
            target["Classification"].MissingValue = "N/A";

            DataTable input = new DataTable("Sample data");

            input.Columns.Add("Age", typeof(int));
            input.Columns.Add("Classification", typeof(string));

            input.Rows.Add(10, "child");
            input.Rows.Add(7, "child");
            input.Rows.Add(4, "N/A");
            input.Rows.Add(21, "adult");
            input.Rows.Add(27, "adult");
            input.Rows.Add(12, "child");
            input.Rows.Add(79, "N/A");
            input.Rows.Add(40, "adult");
            input.Rows.Add(30, "adult");


            DataTable expected = new DataTable("Sample data");

            expected.Columns.Add("Age", typeof(int));
            expected.Columns.Add("Classification", typeof(string));

            expected.Rows.Add(10, "child");
            expected.Rows.Add(7, "child");
            expected.Rows.Add(4, "adult");
            expected.Rows.Add(21, "adult");
            expected.Rows.Add(27, "adult");
            expected.Rows.Add(12, "child");
            expected.Rows.Add(79, "adult");
            expected.Rows.Add(40, "adult");
            expected.Rows.Add(30, "adult");

            target.Detect(input);

            DataTable actual = target.Apply(input);

            Assert.AreEqual(ImputationStrategy.FixedValue, target["Age"].Strategy);
            Assert.AreEqual(-1, target["Age"].MissingValue);
            Assert.AreEqual(null, target["Age"].ReplaceWith);

            Assert.AreEqual(ImputationStrategy.Mode, target["Classification"].Strategy);
            Assert.AreEqual("N/A", target["Classification"].MissingValue);
            Assert.AreEqual("adult", target["Classification"].ReplaceWith);

            for (int i = 0; i < actual.Rows.Count; i++)
                for (int j = 0; j < actual.Columns.Count; j++)
                    Assert.AreEqual(expected.Rows[i][j], actual.Rows[i][j]);
        }


        [Test]
        public void missing_values_gh809()
        {
            // https://github.com/accord-net/framework/issues/809

            DataTable data = new DataTable("Tennis Example with Missing Values");

            data.Columns.Add("Day", typeof(string));
            data.Columns.Add("Outlook", typeof(string));
            data.Columns.Add("Temperature", typeof(string));
            data.Columns.Add("Humidity", typeof(string));
            data.Columns.Add("Wind", typeof(string));
            data.Columns.Add("PlayTennis", typeof(string));

            data.Rows.Add("D2", null, "Hot", "High", "Strong", "No");
            data.Rows.Add("D3", null, null, "High", null, "Yes");
            data.Rows.Add("D4", "Rain", "Mild", "High", "Weak", "Yes");
            data.Rows.Add("D5", "Rain", "Cool", null, "Weak", "Yes");
            data.Rows.Add("D8", null, "Mild", "High", null, "No");

            var codebook = new Codification(data)
            {
                DefaultMissingValueReplacement = Double.NaN
            };

            codebook["Wind"].MissingValueReplacement = 42;

            DataTable symbols = codebook.Apply(data);
            double[][] inputs = symbols.ToJagged("Outlook", "Temperature", "Humidity", "Wind");

            //string str = inputs.ToCSharp();

            double[][] expected =
            {
                new double[] { Double.NaN, 0, 0, 0 },
                new double[] { Double.NaN, Double.NaN, 0, 42 },
                new double[] { 0, 1, 0, 1 },
                new double[] { 0, 2, Double.NaN, 1 },
                new double[] { Double.NaN, 1, 0, 42 }
            };

            Assert.AreEqual(expected, inputs);
        }
#endif

        [Test]
        public void object_test()
        {
            object[][] table =
            {
                new object[] { 0, 19, false, "A", 212.522, 1 },
                new object[] { 1, 5, false, "B", 4.124, 2 },
                new object[] { -1, -1, true, "B", 2.683, 3 },
                new object[] { 3, 5, true, null, Double.NaN, 4 },
                new object[] { 4, -1, false, "C", 2.151, 5 },
            };
            
            // Create a new data projection (column) filter
            var filter = new Imputation("Id", "Floors", "Finished", "Category", "Cost (M)");

            Assert.AreEqual(ImputationStrategy.FixedValue, filter["Id"].Strategy);
            Assert.AreEqual(ImputationStrategy.FixedValue, filter["Floors"].Strategy);
            Assert.AreEqual(ImputationStrategy.FixedValue, filter["Finished"].Strategy);
            Assert.AreEqual(ImputationStrategy.FixedValue, filter["Category"].Strategy);
            Assert.AreEqual(ImputationStrategy.FixedValue, filter["Cost (M)"].Strategy);
            Assert.IsFalse(filter.Columns.Contains("Extra"));

            filter["Floors"].Strategy = ImputationStrategy.FixedValue;
            filter["Floors"].ReplaceWith = 42;

            filter["Category"].Strategy = ImputationStrategy.Mode;
            filter["Cost (M)"].Strategy = ImputationStrategy.Median;

            filter.Learn(table);

            Assert.AreEqual(ImputationStrategy.FixedValue, filter["Floors"].Strategy);
            Assert.AreEqual(-1, filter["Floors"].MissingValue);
            Assert.AreEqual(42, filter["Floors"].ReplaceWith);

            Assert.AreEqual(ImputationStrategy.Mode, filter["Category"].Strategy);
            Assert.AreEqual("B", filter["Category"].ReplaceWith);

            Assert.AreEqual(ImputationStrategy.Median, filter["Cost (M)"].Strategy);
            Assert.AreEqual(3.4034999999999997d, (double)filter["Cost (M)"].ReplaceWith, 1e-10);

            Assert.AreEqual(6, filter.Columns.Count);

            object[][] result = filter.Transform(table);

            Assert.AreEqual(6, result.Columns());
            Assert.AreEqual(5, result.Rows());

            Assert.AreEqual(0, result[0][0]);
            Assert.AreEqual(1, result[1][0]);
            Assert.AreEqual(-1, result[2][0]);
            Assert.AreEqual(3, result[3][0]);
            Assert.AreEqual(4, result[4][0]);

            Assert.AreEqual(19, result[0][1]);
            Assert.AreEqual(5, result[1][1]);
            Assert.AreEqual(42, result[2][1]);
            Assert.AreEqual(5, result[3][1]);
            Assert.AreEqual(42, result[4][1]);

            Assert.AreEqual(false, result[0][2]);
            Assert.AreEqual(false, result[1][2]);
            Assert.AreEqual(true, result[2][2]);
            Assert.AreEqual(true, result[3][2]);
            Assert.AreEqual(false, result[4][2]);

            Assert.AreEqual("A", result[0][3]);
            Assert.AreEqual("B", result[1][3]);
            Assert.AreEqual("B", result[2][3]);
            Assert.AreEqual("B", result[3][3]);
            Assert.AreEqual("C", result[4][3]);

            Assert.AreEqual(212.52199999999999d, (double)result[0][4], 1e-10);
            Assert.AreEqual(4.1239999999999997d, (double)result[1][4], 1e-10);
            Assert.AreEqual(2.6829999999999998d, (double)result[2][4], 1e-10);
            Assert.AreEqual(3.4034999999999997d, (double)result[3][4], 1e-10);
            Assert.AreEqual(2.1509999999999998d, (double)result[4][4], 1e-10);
        }

        [Test]
        public void learn_test()
        {
            #region doc_learn
            // Suppose we have the following double data containing missing values 
            // (indicated by Double.NaN values). Let's say we would like to replace 
            // those NaN values by inputing likely values at their original locations:

            double[][] data =
            {
                new[] { Double.NaN,        0.5,  0.2,               0.7 },
                new[] {        1.2,        6.2,  1.2,               4.2 },
                new[] {         10,        2.2, -1.1,        Double.NaN },
                new[] {         10, Double.NaN, -1.1,               1.0 },
                new[] {         10,        2.2,  Double.NaN,        1.0 },
            };

            // Let's create a new data imputation filter:
            var imputation = new Imputation<double>();

            // Let's instruct it to replace NaN values in the first
            // column by their median, values in the second column
            // by their average, values in the third column by a fixed 
            // value and values in the last column by their mode:
            imputation[0].Strategy = ImputationStrategy.Median;
            imputation[1].Strategy = ImputationStrategy.Mean;
            imputation[2].Strategy = ImputationStrategy.FixedValue;
            imputation[2].ReplaceWith = 42;
            imputation[3].Strategy = ImputationStrategy.Mode;

            // Learn from the data:
            imputation.Learn(data);

            // Now, let's transform the input data using the 
            // data imputation rules we just defined above:
            double[][] result = imputation.Transform(data);

            // The result should be:
            //
            //   double[][] expected = new double[][]
            //   {
            //       new[] {   a, 0.5,  0.2, 0.7 },
            //       new[] { 1.2, 6.2,  1.2, 4.2 },
            //       new[] {  10, 2.2, -1.1,   d },
            //       new[] {  10,   b, -1.1, 1.0 },
            //       new[] {  10, 2.2,    c, 1.0 },
            //   };
            //
            #endregion

            double a = Measures.Median(new[] { 1.2, 10, 10, 10 });
            double b = Measures.Mean(new[] { 0.5, 6.2, 2.2, 2.2 });
            double c = 42;
            double d = Measures.Mode(new[] { 0.7, 4.2, 1.0, 1.0 });

            Assert.AreEqual(imputation[0].ReplaceWith, a);
            Assert.AreEqual(imputation[1].ReplaceWith, b);
            Assert.AreEqual(imputation[2].ReplaceWith, c);
            Assert.AreEqual(imputation[3].ReplaceWith, d);

            string str = result.ToCSharp();


            var expected = new double[][]
            {
                new[] {   a, 0.5,  0.2, 0.7 },
                new[] { 1.2, 6.2,  1.2, 4.2 },
                new[] {  10, 2.2, -1.1,   d },
                new[] {  10,   b, -1.1, 1.0 },
                new[] {  10, 2.2,    c, 1.0 },
            };

            Assert.IsTrue(result.IsEqual(expected, rtol: 1e-10));
        }

        [Test]
        public void StringApplyTest3()
        {
            // Let's say we have a dataset of US birds:
            string[] names = { "State", "Bird", "Color" };

            string[][] data =
            {
                new[] { "Kansas", "Crow", "Black" },
                new[] { "Ohio", "Pardal", "Yellow" },
                new[] { "Hawaii", "Penguim", "Black" }
            };

            var codebook = new Imputation<string>(names, data);

            string[][] values = codebook.Transform(data);

            string str = values.ToCSharp();

            // string t = features.ToCSharp();
            var expected = new string[][]
            {
                new string[] { "Kansas", "Crow", "Black" },
                new string[] { "Ohio", "Pardal", "Yellow" },
                new string[] { "Hawaii", "Penguim", "Black" }
            };

            Assert.IsTrue(values.IsEqual(expected, rtol: 1e-10));
        }

    }
}
