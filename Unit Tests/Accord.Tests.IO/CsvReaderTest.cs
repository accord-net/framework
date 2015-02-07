// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Data;
    using Accord.Math;
    using System.IO;

    [TestClass()]
    public class CsvReaderTest
    {

        // Tests from https://github.com/maxogden/csv-spectrum

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
        public void CsvReader_Simple_NoHeaders()
        {
            var stream = new StringReader(Properties.Resources.csv_simple);

            CsvReader reader = new CsvReader(stream, false);

            var actual = reader.ReadToEnd().ToArray().ToMatrix();

            string[,] expected =
            {
                {"a", "b", "c" },
                {"1", "2", "3" },
            };

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void CsvReader_Simple_WithHeaders()
        {
            {
                var stream = new StringReader(Properties.Resources.csv_simple);

                CsvReader reader = new CsvReader(stream, true);

                var actual = reader.ReadToEnd().ToArray().ToMatrix();

                string[,] expected =
                {
                    {"1", "2", "3" },
                };

                Assert.IsTrue(expected.IsEqual(actual));
            }

            {
                var stream = new StringReader(Properties.Resources.csv_simple);

                CsvReader reader = new CsvReader(stream, true);

                var actual = reader.ToTable();

                Assert.AreEqual("a", actual.Columns[0].ColumnName);
                Assert.AreEqual("b", actual.Columns[1].ColumnName);
                Assert.AreEqual("c", actual.Columns[2].ColumnName);

                Assert.AreEqual(1, actual.Rows.Count);
                Assert.AreEqual("1", actual.Rows[0][0]);
                Assert.AreEqual("2", actual.Rows[0][1]);
                Assert.AreEqual("3", actual.Rows[0][2]);
            }
        }


        [TestMethod()]
        public void CsvReader_Simple_clrf()
        {
            var stream = new StringReader(Properties.Resources.csv_simple_crlf);

            CsvReader reader = new CsvReader(stream, false);

            var actual = reader.ReadToEnd().ToArray().ToMatrix();

            string[,] expected =
            {
                {"a", "b", "c" },
                {"1", "2", "3" },
            };

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void CsvReader_newlines()
        {
            var stream = new StringReader(Properties.Resources.csv_newlines);

            CsvReader reader = new CsvReader(stream, false);

            var actual = reader.ReadToEnd().ToArray().ToMatrix();

            string[,] expected =
            {
                {"a", "b", "c" },
                {"1", "2", "3" },
                {"Once upon \na time", "5", "6" },
                {"7", "8", "9" },
            };

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void CsvReader_newlines_clrf()
        {
            var stream = new StringReader(Properties.Resources.csv_newlines_crlf);

            CsvReader reader = new CsvReader(stream, false);

            var actual = reader.ReadToEnd().ToArray().ToMatrix();

            string[,] expected =
            {
                {"a", "b", "c" },
                {"1", "2", "3" },
                {"Once upon \r\na time", "5", "6" },
                {"7", "8", "9" },
            };

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void CsvReader_quotes_and_newlines()
        {
            var stream = new StringReader(Properties.Resources.csv_quotes_and_newlines);

            CsvReader reader = new CsvReader(stream, false);

            var actual = reader.ReadToEnd().ToArray().ToMatrix();

            string[,] expected =
            {
                {"a", "b" },
                {"1", "ha \n\"ha\" \nha" },
                {"3", "4", },
            };

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void CsvReader_escaped_quotes()
        {
            var stream = new StringReader(Properties.Resources.csv_escaped_quotes);

            CsvReader reader = new CsvReader(stream, false);

            var actual = reader.ReadToEnd().ToArray().ToMatrix();

            string[,] expected =
            {
                {"a", "b" },
                {"1", "ha \"ha\" ha" },
                {"3", "4", },
            };

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void CsvReader_commas_in_quotes()
        {
            var stream = new StringReader(Properties.Resources.csv_comma_in_quotes);

            CsvReader reader = new CsvReader(stream, false);

            var actual = reader.ReadToEnd().ToArray().ToMatrix();

            string[,] expected =
            {
                { "first", "last",   "address",       "city",     "zip" },
                {  "John", "Doe",  "120 any st.", "Anytown, WW", "08123"}
            };

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void CsvReader_empty()
        {
            var stream = new StringReader(Properties.Resources.csv_empty);

            CsvReader reader = new CsvReader(stream, false);

            var actual = reader.ReadToEnd().ToArray().ToMatrix();

            string[,] expected =
            {
                { "a", "b", "c" },
                { "1",  "",  "" },
                { "2", "3", "4" },
            };

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void CsvReader_empty_crlf()
        {
            var stream = new StringReader(Properties.Resources.csv_empty_crlf);

            CsvReader reader = new CsvReader(stream, false);

            var actual = reader.ReadToEnd().ToArray().ToMatrix();

            string[,] expected =
            {
                { "a", "b", "c" },
                { "1",  "",  "" },
                { "2", "3", "4" },
            };

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void CsvReader_utf8()
        {
            var stream = new StringReader(Properties.Resources.csv_utf8);

            CsvReader reader = new CsvReader(stream, false);

            var actual = reader.ReadToEnd().ToArray().ToMatrix();

            string[,] expected =
            {
                { "a", "b", "c" },
                { "1", "2", "3" },
                { "4", "5", "ʤ" },
            };

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void CsvReader_tabs()
        {
            var stream = new StringReader(Properties.Resources.csv_tabs);

            CsvReader reader = new CsvReader(stream, false);

            var actual = reader.ReadToEnd().ToArray().ToMatrix();

            string[,] expected =
            {
                { "a", "b", "c" },
                { "1", "2", "3" },
                { "7", "8", "9" },
            };

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void CsvReader_french()
        {
            var stream = new StringReader(Properties.Resources.csv_french);

            CsvReader reader = new CsvReader(stream, false);

            var actual = reader.ReadToEnd().ToArray().ToMatrix();

            string[,] expected =
            {
                { "a", "b", "c" },
                { "0,52", "2,52", "3,5" },
                { "4", "5", "6" },
            };

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void CsvReader_pipes()
        {
            var stream = new StringReader(Properties.Resources.csv_pipes);

            CsvReader reader = new CsvReader(stream, false);

            var actual = reader.ReadToEnd().ToArray().ToMatrix();

            string[,] expected =
            {
                { "a", "b", "c" },
                { "1", "2", "3" },
                { "7", "8", "9" },
            };

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void CsvReader_double_quotes()
        {
            var stream = new StringReader(Properties.Resources.csv_double_quotes);

            CsvReader reader = new CsvReader(stream, false);

            reader.DefaultParseErrorAction = ParseErrorAction.AdvanceToNextLine;

            var actual = reader.ReadToEnd().ToArray().ToMatrix();

            string[,] expected =
            {
                { "a", "b", "c" },
                { "1", "a", null },
                { "d", "e", "f" },
            };

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void CsvReader_error_recovery()
        {
            var stream = new StringReader(Properties.Resources.csv_error_recovery);

            CsvReader reader = new CsvReader(stream, false);

            reader.DefaultParseErrorAction = ParseErrorAction.AdvanceToNextLine;

            var actual = reader.ReadToEnd().ToArray().ToMatrix();

            string[,] expected =
            {
                { "a", "b", "c"  },
                { "1", "2", "3"  },
                { "4", "6", null },
                { "7", "8", "9"  },
            };

            Assert.IsTrue(expected.IsEqual(actual));
        }
    }
}
