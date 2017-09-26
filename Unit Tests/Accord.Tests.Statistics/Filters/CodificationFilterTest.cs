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
    using Accord.IO;
    using Accord.Math;
    using Accord.Statistics.Filters;
    using NUnit.Framework;
    using System;
    using System.Data;
    using System.IO;
    using System.Linq.Expressions;

    [TestFixture]
    public class CodificationFilterTest
    {

#if !NO_DATA_TABLE
        // An extra example for the Codification filter is available
        // at the Accord.Tests.MachineLearning assembly in the file
        // CodificationFilterSvmTest.cs

        [Test]
        public void ApplyTest1()
        {
            DataTable table = ProjectionFilterTest.CreateTable();

            // Show the start data
            //Accord.Controls.DataGridBox.Show(table);

            // Create a new data projection (column) filter
            var filter = new Codification(table, "Category");

            // Apply the filter and get the result
            DataTable result = filter.Apply(table);

            // Show it
            //Accord.Controls.DataGridBox.Show(result);

            Assert.AreEqual(5, result.Columns.Count);
            Assert.AreEqual(5, result.Rows.Count);

            Assert.AreEqual(0, result.Rows[0]["Category"]);
            Assert.AreEqual(1, result.Rows[1]["Category"]);
            Assert.AreEqual(1, result.Rows[2]["Category"]);
            Assert.AreEqual(0, result.Rows[3]["Category"]);
            Assert.AreEqual(2, result.Rows[4]["Category"]);
        }

        [Test]
        public void ApplyTest()
        {
            Codification target = new Codification();

            DataTable input = new DataTable("Sample data");

            input.Columns.Add("Age", typeof(int));
            input.Columns.Add("Classification", typeof(string));

            input.Rows.Add(10, "child");
            input.Rows.Add(7, "child");
            input.Rows.Add(4, "child");
            input.Rows.Add(21, "adult");
            input.Rows.Add(27, "adult");
            input.Rows.Add(12, "child");
            input.Rows.Add(79, "elder");
            input.Rows.Add(40, "adult");
            input.Rows.Add(30, "adult");



            DataTable expected = new DataTable("Sample data");

            expected.Columns.Add("Age", typeof(int));
            expected.Columns.Add("Classification", typeof(int));

            expected.Rows.Add(10, 0);
            expected.Rows.Add(7, 0);
            expected.Rows.Add(4, 0);
            expected.Rows.Add(21, 1);
            expected.Rows.Add(27, 1);
            expected.Rows.Add(12, 0);
            expected.Rows.Add(79, 2);
            expected.Rows.Add(40, 1);
            expected.Rows.Add(30, 1);



            // Detect the mappings
            target.Detect(input);

            // Apply the categorization
            DataTable actual = target.Apply(input);


            for (int i = 0; i < actual.Rows.Count; i++)
                for (int j = 0; j < actual.Columns.Count; j++)
                    Assert.AreEqual(expected.Rows[i][j], actual.Rows[i][j]);
        }

        [Test]
        public void remapping_test()
        {
            // https://web.archive.org/web/20170210050820/http://www.ats.ucla.edu/stat/stata/dae/mlogit.htm
            CsvReader reader = CsvReader.FromText(Properties.Resources.hsbdemo, hasHeaders: true);

            var table = reader.ToTable();

            var codification = new Codification(table);
            codification["ses"].VariableType = CodificationVariable.CategoricalWithBaseline;
            codification["prog"].VariableType = CodificationVariable.Categorical;
            codification["write"].VariableType = CodificationVariable.Discrete;
            codification["ses"].Remap("low", 0);
            codification["ses"].Remap("middle", 1);
            codification["prog"].Remap("academic", 0);
            codification["prog"].Remap("general", 1);

            Assert.AreEqual(CodificationVariable.Discrete, codification["write"].VariableType);

            var inputs = codification.Apply(table, "write", "ses");
            var output = codification.Apply(table, "prog");

            // Get inputs
            string[] inputNames;
            var inputsData = inputs.ToArray(out inputNames);

            // Get outputs
            string[] outputNames;
            var outputData = output.ToArray(out outputNames);

            Assert.AreEqual(new[] { "write", "ses: middle", "ses: high" }, inputNames);
            Assert.AreEqual(new[] { "prog: academic", "prog: general", "prog: vocation" }, outputNames);

            Assert.AreEqual(new double[] { 35, 0, 0 }, inputsData[0]);
            Assert.AreEqual(new double[] { 33, 1, 0 }, inputsData[1]);
            Assert.AreEqual(new double[] { 39, 0, 1 }, inputsData[2]);

            Assert.AreEqual(new double[] { 0, 0, 1 }, outputData[0]);
            Assert.AreEqual(new double[] { 0, 1, 0 }, outputData[1]);
            Assert.AreEqual(new double[] { 0, 0, 1 }, outputData[2]);
            Assert.AreEqual(new double[] { 1, 0, 0 }, outputData[11]);
        }

        [Test]
        public void remapping_test_new_method()
        {
            // https://web.archive.org/web/20170210050820/http://www.ats.ucla.edu/stat/stata/dae/mlogit.htm

            // Let's download an example dataset from the web to learn a multinomial logistic regression:
            CsvReader reader = CsvReader.FromUrl("https://raw.githubusercontent.com/rlowrance/re/master/hsbdemo.csv", hasHeaders: true);

            // Let's read the CSV into a DataTable. As mentioned above, this step
            // can help, but is not necessarily required for learning a the model:
            DataTable table = reader.ToTable();

            // We will learn a MLR regression between the following input and output fields of this table:
            string[] inputNames = new[] { "write", "ses" };
            string[] outputNames = new[] { "prog" };

            // Now let's create a codification codebook to convert the string fields in the data 
            // into integer symbols. This is required because the MLR model can only learn from 
            // numeric data, so strings have to be transformed first. We can force a particular
            // interpretation for those columns if needed, as shown in the initializer below:
            var codification = new Codification()
            {
                new Codification.Options("write", CodificationVariable.Continuous),
                new Codification.Options("ses", CodificationVariable.CategoricalWithBaseline, order: new[] { "low", "middle", "high" }),
                new Codification.Options("prog", CodificationVariable.Categorical, order: new[] { "academic", "general" })
            };

            // Learn the codification
            codification.Learn(table);

            // Now, transform symbols into a vector representation, growing the number of inputs:
            double[][] inputsData = codification.Transform(table, inputNames, out inputNames).ToDouble();
            double[][] outputData = codification.Transform(table, outputNames, out outputNames).ToDouble();

            Assert.AreEqual(new[] { "write", "ses: middle", "ses: high" }, inputNames);
            Assert.AreEqual(new[] { "prog: academic", "prog: general", "prog: vocation" }, outputNames);

            Assert.AreEqual(new double[] { 35, 0, 0 }, inputsData[0]);
            Assert.AreEqual(new double[] { 33, 1, 0 }, inputsData[1]);
            Assert.AreEqual(new double[] { 39, 0, 1 }, inputsData[2]);

            Assert.AreEqual(new double[] { 0, 0, 1 }, outputData[0]);
            Assert.AreEqual(new double[] { 0, 1, 0 }, outputData[1]);
            Assert.AreEqual(new double[] { 0, 0, 1 }, outputData[2]);
            Assert.AreEqual(new double[] { 1, 0, 0 }, outputData[11]);
        }
#endif

        [Test]
        public void ApplyTest3()
        {
            string[] names = { "child", "adult", "elder" };

            Codification codebook = new Codification("Label", names);


            // After that, we can use the codebook to "translate"
            // the text labels into discrete symbols, such as:

            int a = codebook.Translate("Label", "child"); // returns 0
            int b = codebook.Translate("Label", "adult"); // returns 1
            int c = codebook.Translate("Label", "elder"); // returns 2

            // We can also do the reverse:
            string labela = codebook.Translate("Label", 0); // returns "child"
            string labelb = codebook.Translate("Label", 1); // returns "adult"
            string labelc = codebook.Translate("Label", 2); // returns "elder"

            Assert.AreEqual(0, a);
            Assert.AreEqual(1, b);
            Assert.AreEqual(2, c);
            Assert.AreEqual("child", labela);
            Assert.AreEqual("adult", labelb);
            Assert.AreEqual("elder", labelc);
        }

#if !NO_EXCEL
        [Test]
        [Category("Office")]
        public void ApplyTest4()
        {
            string path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "intrusion.xls");

            ExcelReader db = new ExcelReader(path, false, true);

            DataTable table = db.GetWorksheet("test");

            Codification codebook = new Codification(table);

            DataTable result = codebook.Apply(table);

            Assert.IsNotNull(result);

            foreach (DataColumn col in result.Columns)
                Assert.AreNotEqual(col.DataType, typeof(string));

            Assert.IsTrue(result.Rows.Count > 0);
        }
#endif

#if !NO_DATA_TABLE
        /// <summary>
        ///   Testing Codification.Translate(string, string)
        ///   This method tests, that the correct DataColumn is used 
        /// </summary>
        /// 
        [Test]
        public void TranslateTest1()
        {
            string[] colNames = { "col1", "col2", "col3" };
            DataTable table = new DataTable("TranslateTest1 Table");
            table.Columns.Add(colNames);

            table.Rows.Add(1, 2, 3);
            table.Rows.Add(1, 3, 5);
            table.Rows.Add(1, 4, 7);
            table.Rows.Add(2, 4, 6);
            table.Rows.Add(2, 5, 8);
            table.Rows.Add(2, 6, 10);
            table.Rows.Add(3, 4, 5);
            table.Rows.Add(3, 5, 7);
            table.Rows.Add(3, 6, 9);

            // ok, so values 1,2,3 are in column 1
            // values 2,3,4,5,6 in column 2
            // values 3,5,6,7,8,9,10 in column 3
            var codeBook = new Codification(table);

            Assert.AreEqual(0, codeBook.Translate("col1", "1"));
            Assert.AreEqual(1, codeBook.Translate("col1", "2"));
            Assert.AreEqual(2, codeBook.Translate("col1", "3"));

            Assert.AreEqual(0, codeBook.Translate("col2", "2"));
            Assert.AreEqual(1, codeBook.Translate("col2", "3"));
            Assert.AreEqual(2, codeBook.Translate("col2", "4"));
            Assert.AreEqual(3, codeBook.Translate("col2", "5"));
            Assert.AreEqual(4, codeBook.Translate("col2", "6"));

            Assert.AreEqual(0, codeBook.Translate("col3", "3"));
            Assert.AreEqual(1, codeBook.Translate("col3", "5"));
            Assert.AreEqual(2, codeBook.Translate("col3", "7"));
            Assert.AreEqual(3, codeBook.Translate("col3", "6"));
            Assert.AreEqual(4, codeBook.Translate("col3", "8"));
            Assert.AreEqual(5, codeBook.Translate("col3", "10"));
            Assert.AreEqual(6, codeBook.Translate("col3", "9"));
        }

        /// <summary>
        ///   Testing Codification.Translate(string[], string[])
        /// </summary>
        /// 
        [Test]
        public void TranslateTest2()
        {
            string[] colNames = { "col1", "col2", "col3" };
            DataTable table = new DataTable("TranslateTest1 Table");
            table.Columns.Add(colNames);

            table.Rows.Add(1, 2, 3);
            table.Rows.Add(1, 3, 5);
            table.Rows.Add(1, 4, 7);
            table.Rows.Add(2, 4, 6);
            table.Rows.Add(2, 5, 8);
            table.Rows.Add(2, 6, 10);
            table.Rows.Add(3, 4, 5);
            table.Rows.Add(3, 5, 7);
            table.Rows.Add(3, 6, 9);

            // values 1,2,3 are in column 1
            // values 2,3,4,5,6 in column 2
            // values 3,5,6,7,8,9,10 in column 3
            var codeBook = new Codification(table);
            Matrix.IsEqual(new int[] { 0, 0, 0 }, codeBook.Translate(colNames, new[] { "1", "2", "3" }));
            Matrix.IsEqual(new int[] { 0, 1, 1 }, codeBook.Translate(colNames, new[] { "1", "3", "5" }));
            Matrix.IsEqual(new int[] { 0, 2, 2 }, codeBook.Translate(colNames, new[] { "1", "4", "7" }));
            Matrix.IsEqual(new int[] { 1, 2, 3 }, codeBook.Translate(colNames, new[] { "2", "4", "6" }));
            Matrix.IsEqual(new int[] { 1, 3, 4 }, codeBook.Translate(colNames, new[] { "2", "5", "8" }));
            Matrix.IsEqual(new int[] { 1, 4, 5 }, codeBook.Translate(colNames, new[] { "2", "6", "10" }));
            Matrix.IsEqual(new int[] { 2, 2, 1 }, codeBook.Translate(colNames, new[] { "3", "4", "5" }));
            Matrix.IsEqual(new int[] { 2, 3, 2 }, codeBook.Translate(colNames, new[] { "3", "5", "7" }));
            Matrix.IsEqual(new int[] { 2, 4, 6 }, codeBook.Translate(colNames, new[] { "3", "6", "9" }));

            Matrix.IsEqual(new int[] { 2 }, codeBook.Translate(colNames.First(1), new[] { "3" }));
            Matrix.IsEqual(new int[] { 2, 4 }, codeBook.Translate(colNames.First(2), new[] { "3", "6" }));
            Matrix.IsEqual(new int[] { 2, 4, 6 }, codeBook.Translate(colNames.First(3), new[] { "3", "6", "9" }));

            bool thrown = false;

            try
            {
                Matrix.IsEqual(new int[] { 2, 4, 6 },
                    codeBook.Translate(colNames.Concatenate("col4"), new[] { "3", "6", "9", "10" }));
            }
            catch (Exception) { thrown = true; }

            Assert.IsTrue(thrown);
        }

        /// <summary>
        ///   Testing Codification.Translate(params string[])
        ///   This test assumes string input is given in correct column order - is otherwise identical to TranslateTest2
        /// </summary>
        /// 
        [Test]
        public void TranslateTest3()
        {
            string[] colNames = { "col1", "col2", "col3" };
            DataTable table = new DataTable("TranslateTest1 Table");
            table.Columns.Add(colNames);

            table.Rows.Add(1, 2, 3);
            table.Rows.Add(1, 3, 5);
            table.Rows.Add(1, 4, 7);
            table.Rows.Add(2, 4, 6);
            table.Rows.Add(2, 5, 8);
            table.Rows.Add(2, 6, 10);
            table.Rows.Add(3, 4, 5);
            table.Rows.Add(3, 5, 7);
            table.Rows.Add(3, 6, 9);

            // values 1,2,3 are in column 1
            // values 2,3,4,5,6 in column 2
            // values 3,5,6,7,8,9,10 in column 3
            var codeBook = new Codification(table);
            Matrix.IsEqual(new int[] { 0, 0, 0 }, codeBook.Translate(new[] { "1", "2", "3" }));
            Matrix.IsEqual(new int[] { 0, 1, 1 }, codeBook.Translate(new[] { "1", "3", "5" }));
            Matrix.IsEqual(new int[] { 0, 2, 2 }, codeBook.Translate(new[] { "1", "4", "7" }));
            Matrix.IsEqual(new int[] { 1, 2, 3 }, codeBook.Translate(new[] { "2", "4", "6" }));
            Matrix.IsEqual(new int[] { 1, 3, 4 }, codeBook.Translate(new[] { "2", "5", "8" }));
            Matrix.IsEqual(new int[] { 1, 4, 5 }, codeBook.Translate(new[] { "2", "6", "10" }));
            Matrix.IsEqual(new int[] { 2, 2, 1 }, codeBook.Translate(new[] { "3", "4", "5" }));
            Matrix.IsEqual(new int[] { 2, 3, 2 }, codeBook.Translate(new[] { "3", "5", "7" }));
            Matrix.IsEqual(new int[] { 2, 4, 6 }, codeBook.Translate(new[] { "3", "6", "9" }));

            Matrix.IsEqual(new int[] { 2 }, codeBook.Translate(new[] { "3" }));
            Matrix.IsEqual(new int[] { 2, 4 }, codeBook.Translate(new[] { "3", "6" }));
            Matrix.IsEqual(new int[] { 2, 4, 6 }, codeBook.Translate(new[] { "3", "6", "9" }));

            bool thrown = false;

            try { codeBook.Translate(new[] { "3", "6", "9", "10" }); }
            catch (Exception) { thrown = true; }

            Assert.IsTrue(thrown);
        }
#endif

#if !NO_BINARY_SERIALIZATION
        [Test]
        [Category("Serialization")]
        public void SerializationTest()
        {
            string[] names = { "child", "adult", "elder" };

            Codification codebook = new Codification("Label", names);

            Assert.AreEqual(0, codebook.Translate("Label", "child"));
            Assert.AreEqual(1, codebook.Translate("Label", "adult"));
            Assert.AreEqual(2, codebook.Translate("Label", "elder"));
            Assert.AreEqual("child", codebook.Translate("Label", 0));
            Assert.AreEqual("adult", codebook.Translate("Label", 1));
            Assert.AreEqual("elder", codebook.Translate("Label", 2));


            byte[] bytes = codebook.Save();

            Codification reloaded = Serializer.Load<Codification>(bytes);

            Assert.AreEqual(codebook.Active, reloaded.Active);
            Assert.AreEqual(codebook.Columns.Count, reloaded.Columns.Count);
            Assert.AreEqual(codebook.Columns[0].ColumnName, reloaded.Columns[0].ColumnName);

            Assert.AreEqual(0, reloaded.Translate("Label", "child"));
            Assert.AreEqual(1, reloaded.Translate("Label", "adult"));
            Assert.AreEqual(2, reloaded.Translate("Label", "elder"));
            Assert.AreEqual("child", reloaded.Translate("Label", 0));
            Assert.AreEqual("adult", reloaded.Translate("Label", 1));
            Assert.AreEqual("elder", reloaded.Translate("Label", 2));
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
        public void StringApplyTest3()
        {
            // Example for https://github.com/accord-net/framework/issues/581

            // Let's say we have a dataset of US birds:
            string[] names = { "State", "Bird", "Color" };

            string[][] data =
            {
                new[] { "Kansas", "Crow", "Black" },
                new[] { "Ohio", "Pardal", "Yellow" },
                new[] { "Hawaii", "Penguim", "Black" }
            };

            // Create a codebook for the dataset
            var codebook = new Codification(names, data);

            // Transform the data into integer symbols
            int[][] values = codebook.Transform(data);

            // Transform the symbols into 1-of-K vectors
            double[][] states = Jagged.OneHot(values.GetColumn(0));
            double[][] birds = Jagged.OneHot(values.GetColumn(1));
            double[][] colors = Jagged.OneHot(values.GetColumn(2));

            // Normalize each variable separately if needed
            states = states.Divide(codebook["State"].NumberOfSymbols);
            birds = birds.Divide(codebook["Bird"].NumberOfSymbols);
            colors = colors.Divide(codebook["Color"].NumberOfSymbols);

            // Create final feature vectors
            double[][] features = Matrix.Concatenate(states, birds, colors);

            Assert.AreEqual(new[] { 3, 3 }, states.GetLength());
            Assert.AreEqual(new[] { 3, 3 }, birds.GetLength());
            Assert.AreEqual(new[] { 3, 2 }, colors.GetLength());
            Assert.AreEqual(new[] { 3, 8 }, features.GetLength());

            // string t = features.ToCSharp();
            var expected = new double[][]
            {
                new double[] { 0.333333333333333, 0, 0, 0.333333333333333, 0, 0, 0.5, 0 },
                new double[] { 0, 0.333333333333333, 0, 0, 0.333333333333333, 0, 0, 0.5 },
                new double[] { 0, 0, 0.333333333333333, 0, 0, 0.333333333333333, 0.5, 0 }
            };

            Assert.IsTrue(features.IsEqual(expected, rtol: 1e-10));
        }

        [Test]
        public void thresholds()
        {
            // Example for https://github.com/accord-net/framework/issues/737

            // Let's say we have a dataset of US birds:
            string[] names = { "State", "Bird", "Percentage" };

            object[][] inputData =
            {
                new object[] { "Kansas", "Crow", 0.1 },
                new object[] { "Ohio", "Pardal", 0.5 },
                new object[] { "Hawaii", "Penguim", 0.7 }
            };

            // Discretize the continous data from a doubles to a string representation
            var discretization = new Discretization<double, string>(names, inputData);
            discretization["Percentage"].Mapping[x => x >= 0.00 && x < 0.25] = x => "Q1";
            discretization["Percentage"].Mapping[x => x >= 0.25 && x < 0.50] = x => "Q2";
            discretization["Percentage"].Mapping[x => x >= 0.50 && x < 0.75] = x => "Q3";
            discretization["Percentage"].Mapping[x => x >= 0.75 && x < 1.09] = x => "Q4";

            // Transform the data into discrete categories
            string[][] discreteData = discretization.Transform(inputData);

            // Codify the discrete data from strings to integers
            var codebook = new Codification<string>(names, discreteData);

            // Transform the data into integer symbols
            int[][] values = codebook.Transform(discreteData);

            // Transform the symbols into 1-of-K vectors
            double[][] states = Jagged.OneHot(values.GetColumn(0));
            double[][] birds = Jagged.OneHot(values.GetColumn(1));
            double[][] colors = Jagged.OneHot(values.GetColumn(2));

            // Normalize each variable separately if needed
            states = states.Divide(codebook["State"].NumberOfSymbols);
            birds = birds.Divide(codebook["Bird"].NumberOfSymbols);
            colors = colors.Divide(codebook["Percentage"].NumberOfSymbols);

            // Create final feature vectors
            double[][] features = Matrix.Concatenate(states, birds, colors);

            Assert.AreEqual(new[] { 3, 3 }, states.GetLength());
            Assert.AreEqual(new[] { 3, 3 }, birds.GetLength());
            Assert.AreEqual(new[] { 3, 2 }, colors.GetLength());
            Assert.AreEqual(new[] { 3, 8 }, features.GetLength());

            // string t = features.ToCSharp();
            var expected = new double[][]
            {
                new double[] { 0.333333333333333, 0, 0, 0.333333333333333, 0, 0, 0.5, 0 },
                new double[] { 0, 0.333333333333333, 0, 0, 0.333333333333333, 0, 0, 0.5 },
                new double[] { 0, 0, 0.333333333333333, 0, 0, 0.333333333333333, 0, 0.5 }
            };

            Assert.IsTrue(features.IsEqual(expected, rtol: 1e-10));
        }
    }
}
