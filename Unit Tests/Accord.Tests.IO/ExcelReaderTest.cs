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
    using NUnit.Framework;
    using System.Data;
    using System.IO;

    [TestFixture]
    public class ExcelReaderTest
    {

        [Test]
        [Category("Office")]
        public void TestExcel8_Data1()
        {
            string path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "excel", "data1.xls");

            ExcelReader withHeader = new ExcelReader(path, true);
            ExcelReader sansHeader = new ExcelReader(path, false);

            testData1(withHeader, sansHeader);
        }


        [Test]
        [Category("Office")]
        public void TestExcel12_Data1()
        {
            string path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "excel", "data1.xlsx");

            ExcelReader withHeader = new ExcelReader(path, true);
            ExcelReader sansHeader = new ExcelReader(path, false);

            testData1(withHeader, sansHeader);
        }

        private static void testData1(ExcelReader withHeader, ExcelReader sansHeader)
        {
            var sheets1 = withHeader.GetWorksheetList();
            var sheets2 = sansHeader.GetWorksheetList();

            Assert.AreEqual(1, sheets1.Length);
            Assert.AreEqual(1, sheets2.Length);

            Assert.AreEqual("testnormal", sheets1[0]);
            Assert.AreEqual("testnormal", sheets2[0]);

            var dataWithHeader = withHeader.GetWorksheet("testnormal");
            var dataSansHeader = sansHeader.GetWorksheet("testnormal");

            Assert.AreEqual(119, dataWithHeader.Rows.Count);
            Assert.AreEqual(17, dataWithHeader.Columns.Count);

            Assert.AreEqual(120, dataSansHeader.Rows.Count);
            Assert.AreEqual(17, dataSansHeader.Columns.Count);

            var firstRowWithHeader = dataWithHeader.Rows[0].ItemArray;
            Assert.AreEqual("90", firstRowWithHeader[0]);
            Assert.AreEqual("0", firstRowWithHeader[1]);
            Assert.AreEqual("0", firstRowWithHeader[2]);
            Assert.AreEqual("0", firstRowWithHeader[3]);
            Assert.AreEqual("0", firstRowWithHeader[4]);

            var firstRowSansHeader = dataSansHeader.Rows[0].ItemArray;
            Assert.AreEqual("1", firstRowSansHeader[0]);
            Assert.AreEqual("2", firstRowSansHeader[1]);
            Assert.AreEqual("3", firstRowSansHeader[2]);
            Assert.AreEqual("4", firstRowSansHeader[3]);
            Assert.AreEqual("5", firstRowSansHeader[4]);
        }




        [Test]
        [Category("Office")]
        public void TestExcel8_Data2()
        {
            string path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "excel", "data2.xls");

            ExcelReader withHeader = new ExcelReader(path, true);
            ExcelReader sansHeader = new ExcelReader(path, false);

            testData2(withHeader, sansHeader);
        }


        [Test]
        [Category("Office")]
        public void TestExcel12_Data2()
        {
            string path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "excel", "data2.xlsx");

            ExcelReader withHeader = new ExcelReader(path, true);
            ExcelReader sansHeader = new ExcelReader(path, false);

            testData2(withHeader, sansHeader);
        }

        private static void testData2(ExcelReader withHeader, ExcelReader sansHeader)
        {
            var sheets1 = withHeader.GetWorksheetList();
            var sheets2 = sansHeader.GetWorksheetList();

            Assert.AreEqual(1, sheets1.Length);
            Assert.AreEqual(1, sheets2.Length);

            Assert.AreEqual("training", sheets1[0]);
            Assert.AreEqual("training", sheets2[0]);

            var dataWithHeader = withHeader.GetWorksheet("training");
            var dataSansHeader = sansHeader.GetWorksheet("training");

            Assert.AreEqual(19496, dataWithHeader.Rows.Count);
            Assert.AreEqual(17, dataWithHeader.Columns.Count);

            Assert.AreEqual(19497, dataSansHeader.Rows.Count);
            Assert.AreEqual(17, dataSansHeader.Columns.Count);

            var firstRowWithHeader = dataWithHeader.Rows[0].ItemArray;
            Assert.AreEqual("80", firstRowWithHeader[0]);
            Assert.AreEqual("0", firstRowWithHeader[1]);
            Assert.AreEqual("0", firstRowWithHeader[2]);
            Assert.AreEqual("0", firstRowWithHeader[3]);
            Assert.AreEqual("0", firstRowWithHeader[4]);

            var firstRowSansHeader = dataSansHeader.Rows[0].ItemArray;
            Assert.AreEqual("1", firstRowSansHeader[0]);
            Assert.AreEqual("2", firstRowSansHeader[1]);
            Assert.AreEqual("3", firstRowSansHeader[2]);
            Assert.AreEqual("4", firstRowSansHeader[3]);
            Assert.AreEqual("5", firstRowSansHeader[4]);
        }


        [Test]
        [Category("Office")]
        public void TestExcel8_Data3()
        {
            string path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "excel", "data3.xls");

            ExcelReader withHeader = new ExcelReader(path, true);
            ExcelReader sansHeader = new ExcelReader(path, false);

            testData3(withHeader, sansHeader);
        }


        [Test]
        [Category("Office")]
        public void TestExcel12_Data3()
        {
            string path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "excel", "data3.xlsx");

            ExcelReader withHeader = new ExcelReader(path, true);
            ExcelReader sansHeader = new ExcelReader(path, false);

            testData3(withHeader, sansHeader);
        }

        private static void testData3(ExcelReader withHeader, ExcelReader sansHeader)
        {
            var sheets1 = withHeader.GetWorksheetList();
            var sheets2 = sansHeader.GetWorksheetList();

            Assert.AreEqual(1, sheets1.Length);
            Assert.AreEqual(1, sheets2.Length);

            Assert.AreEqual("training", sheets1[0]);
            Assert.AreEqual("training", sheets2[0]);

            var dataWithHeader = withHeader.GetWorksheet("training");
            var dataSansHeader = sansHeader.GetWorksheet("training");

            Assert.AreEqual(302, dataWithHeader.Rows.Count);
            Assert.AreEqual(17, dataWithHeader.Columns.Count);

            Assert.AreEqual(303, dataSansHeader.Rows.Count);
            Assert.AreEqual(17, dataSansHeader.Columns.Count);

            var firstRowWithHeader = dataWithHeader.Rows[0].ItemArray;
            Assert.AreEqual("50", firstRowWithHeader[0]);
            Assert.AreEqual("0", firstRowWithHeader[1]);
            Assert.AreEqual("0", firstRowWithHeader[2]);
            Assert.AreEqual("0", firstRowWithHeader[3]);
            Assert.AreEqual("0", firstRowWithHeader[4]);

            var firstRowSansHeader = dataSansHeader.Rows[0].ItemArray;
            Assert.AreEqual("1", firstRowSansHeader[0]);
            Assert.AreEqual("2", firstRowSansHeader[1]);
            Assert.AreEqual("3", firstRowSansHeader[2]);
            Assert.AreEqual("4", firstRowSansHeader[3]);
            Assert.AreEqual("5", firstRowSansHeader[4]);
        }


        [Test]
        [Category("Office")]
        public void ExcelReaderConstructorTest()
        {
            string path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "sample.xls");

            // Create a new reader, opening a given path
            ExcelReader reader = new ExcelReader(path);

            // Afterwards, we can query the file for all
            // worksheets within the specified workbook:
            string[] sheets = reader.GetWorksheetList();

            // Finally, we can request an specific sheet:
            DataTable table = reader.GetWorksheet(sheets[1]);

            // Now, we have loaded the Excel file into a DataTable. We
            // can go further and transform it into a matrix to start
            // running other algorithms on it: 

            Assert.AreEqual(4, sheets.Length);
            Assert.AreEqual("Plan1", sheets[0]);
            Assert.AreEqual("Plan2", sheets[1]);
            Assert.AreEqual("Plan3", sheets[2]);
            Assert.AreEqual("Sheet1", sheets[3]);
            Assert.AreEqual(3, table.Columns.Count);
            Assert.AreEqual(2, table.Rows.Count);
        }

        [Test]
        [Category("Office")]
        public void ConstructorExcel8Test()
        {
            string path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "sample.xls");
            ExcelReader target = new ExcelReader(path);

            testWorksheets(target);

            testColumns(target, false);

            testTables(target);
        }

        [Test]
        [Category("Office")]
        public void ConstructorExcel10Test()
        {
            // If a 64-bit ACE is installed, this test requires a 64-bit process to run correctly.
            // You also need to configure the test runner to use x64 instead of x86:
            // https://msdn.microsoft.com/library/ee782531(VS.110).aspx
            string path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "sample.xlsx");
            ExcelReader target = new ExcelReader(path);

            testWorksheets(target);

            testColumns(target, true);

            testTables(target);
        }

        private static void testTables(ExcelReader target)
        {
            DataSet set = target.GetWorksheet();
            Assert.AreEqual(4, set.Tables.Count);
            Assert.AreEqual("Plan1", set.Tables[0].TableName);
            Assert.AreEqual("Plan2", set.Tables[1].TableName);
            Assert.AreEqual("Plan3", set.Tables[2].TableName);
            Assert.AreEqual("Sheet1", set.Tables[3].TableName);
        }

        private static void testWorksheets(ExcelReader target)
        {
            string[] list = target.GetWorksheetList();
            Assert.AreEqual(4, list.Length);
            Assert.AreEqual("Plan1", list[0]);
            Assert.AreEqual("Plan2", list[1]);
            Assert.AreEqual("Plan3", list[2]);
            Assert.AreEqual("Sheet1", list[3]);
        }

        private static void testColumns(ExcelReader target, bool xlsx)
        {
            string[] cols = target.GetColumnsList("Plan1");
            Assert.AreEqual(3, cols.Length);
            Assert.AreEqual("Header1", cols[0]);
            Assert.AreEqual("Header2", cols[1]);
            Assert.AreEqual("Header3", cols[2]);

            cols = target.GetColumnsList("Plan2");
            Assert.AreEqual(3, cols.Length);
            if (target.Provider == "Microsoft.Jet.OLEDB.4.0")
            {
                Assert.AreEqual("F1", cols[0]);
                Assert.AreEqual("F2", cols[1]);
                Assert.AreEqual("F3", cols[2]);
            }
            else
            {
                Assert.AreEqual("1", cols[0]);
                Assert.AreEqual("2", cols[1]);
                Assert.AreEqual("3", cols[2]);
            }

            cols = target.GetColumnsList("Plan3");
            Assert.AreEqual(2, cols.Length);
            Assert.AreEqual("A", cols[0]);
            Assert.AreEqual("B", cols[1]);

            cols = target.GetColumnsList("Sheet1");
            Assert.AreEqual(1, cols.Length);
            Assert.AreEqual("F1", cols[0]);
        }


        [Test]
        [Category("Office")]
        public void SpreadsheetNames_Success()
        {
            string path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "excel", "spreadsheet_names.xls");

            // Create a new reader, opening a given path
            ExcelReader reader = new ExcelReader(path);

            string[] sheets = reader.GetWorksheetList();

            Assert.AreEqual(4, sheets.Length);
            Assert.AreEqual("Example 1", sheets[0]);
            Assert.AreEqual("Example 2", sheets[1]);
            Assert.AreEqual("Example 3", sheets[2]);
            Assert.AreEqual("References", sheets[3]);

            // Finally, we can request an specific sheet:
            DataTable table = reader.GetWorksheet(sheets[1]);

            Assert.AreEqual(2, table.Columns.Count);
            Assert.AreEqual(42, table.Rows.Count);
        }
    }
}
