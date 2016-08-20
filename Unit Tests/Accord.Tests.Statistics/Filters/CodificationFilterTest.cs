// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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

    [TestFixture]
    public class CodificationFilterTest
    {

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

        [Test]
        public void ApplyTest4()
        {
            string path = @"Resources\intrusion.xls";

            ExcelReader db = new ExcelReader(path, false, true);

            DataTable table = db.GetWorksheet("test");

            Codification codebook = new Codification(table);

            DataTable result = codebook.Apply(table);

            Assert.IsNotNull(result);

            foreach (DataColumn col in result.Columns)
                Assert.AreNotEqual(col.DataType, typeof(string));

            Assert.IsTrue(result.Rows.Count > 0);
        }

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

            // ok, so values 1,2,3 are in column 1
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

            // ok, so values 1,2,3 are in column 1
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

    }
}
