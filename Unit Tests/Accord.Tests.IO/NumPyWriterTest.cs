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
    using System;
    using System.IO;
    using System.Text;
    using Accord.IO;
    using Accord.Math;
    using Accord.Tests.IO.Properties;
    using NUnit.Framework;

    [TestFixture]
    public class NpyWriterTest
    {
        public static byte[] GetNpy(string resourceName)
        {
            string fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "numpy", resourceName);
            var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            var ms = new MemoryStream();
            fs.CopyTo(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms.ToArray();
        }

        [Test]
        public void WriteByteMatrixTest()
        {
            byte[,] input =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            byte[] actual = NpyFormat.Save(input);

            // header should be
            // string header = "{ 'descr': '|i1', 'fortran_order': False, 'shape': (2, 3), }          \n";
            byte[] expected = GetNpy("npy_byte.npy");

            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(actual[i], expected[i]);

            Assert.IsTrue(actual.IsEqual(expected));
        }

        [Test]
        public void WriteByteJaggedTest()
        {
            byte[][] input =
            {
                new byte[] { 1, 2, 3 },
                new byte[] { 4, 5, 6 }
            };

            byte[] actual = NpyFormat.Save(input);
            byte[] expected = GetNpy("npy_byte.npy");

            Assert.IsTrue(actual.IsEqual(expected));
        }

        [Test]
        public void WriteBoolMatrixTest()
        {
            bool[,] input =
            {
                { true, true, false },
                { false, false, false }
            };

            byte[] actual = NpyFormat.Save(input);
            byte[] expected = GetNpy("npy_bool.npy");

            // Expected header
            // "{'descr': '|b1', 'fortran_order': False, 'shape': (2, 3), }          \n"

            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(actual[i], expected[i]);

            Assert.IsTrue(actual.IsEqual(expected));
        }

        [Test]
        public void WriteBoolJaggedTest()
        {
            bool[][] input =
            {
                new[] { true, true, false },
                new[] { false, false, false }
            };

            byte[] actual = NpyFormat.Save(input);
            byte[] expected = GetNpy("npy_bool.npy");

            Assert.IsTrue(actual.IsEqual(expected));
        }

        [Test]
        public void WriteInt64MatrixTest()
        {
            long[,] input =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            byte[] actual = NpyFormat.Save(input);
            byte[] expected = GetNpy("npy_integer.npy");

            Assert.IsTrue(actual.IsEqual(expected));
        }

        [Test]
        public void WriteSingleMatrixTest()
        {
            float[,] input =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            byte[] actual = NpyFormat.Save(input);
            byte[] expected = GetNpy("npy_single.npy");

            Assert.IsTrue(actual.IsEqual(expected));
        }

        [Test]
        public void WriteDoubleMatrixTest()
        {
            double[,] input =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            byte[] actual = NpyFormat.Save(input);
            byte[] expected = GetNpy("npy_double.npy");

            Assert.IsTrue(actual.IsEqual(expected));
        }

        [Test]
        public void WriteStringMatrixTest()
        {
            string[,] value =
            {
                { "a", "b", "c" },
                { "d", "e", "f"}
            };

            byte[] actual = NpyFormat.Save(value);
            byte[] expected = GetNpy("npy_strings.npy");

            Assert.IsTrue(actual.IsEqual(expected));
        }

        [Test]
        public void WriteStringMatrixVariableLengthTest()
        {
            string[,] input =
            {
                { "a", "b", "abacaxi" },
                { "d", "cs", "f"}
            };

            byte[] actual = NpyFormat.Save(input);
            byte[] expected = GetNpy("npy_strings_var.npy");

            Assert.IsTrue(actual.IsEqual(expected));
        }

        [Test]
        public void WriteStringJaggedVariableLengthTest()
        {
            string[][] input =
            {
                new[] { "a", "b", "abacaxi" },
                new[] { "d", "cs", "f"}
            };

            byte[] actual = NpyFormat.Save(input);
            byte[] expected = GetNpy("npy_strings_var.npy");

            Assert.IsTrue(actual.IsEqual(expected));
        }

        [Test]
        public void WriteStringJaggedVariableColsTest()
        {
            string[][] value =
            {
                new[] { "a", "b" },
                new[] { "d", "cs", "f"},
                new[] { "z"},
            };

            byte[] bytes = NpyFormat.Save(value);

            string[,] expectedJagged =
            {
                { "a", "b", null },
                { "d", "cs", "f" },
                { "z", null, null,},
            };

            string[,] expectedMatrix = expectedJagged;
            string[,] actualMatrix = (string[,])NpyFormat.LoadMatrix(bytes);
            Assert.IsTrue(actualMatrix.IsEqual(expectedMatrix));
        }

        [Test]
        public void WriteStringJaggedVariableColsTrimTest()
        {
            string[][] value =
            {
                new[] { "a", "b" },
                new[] { "d", "cs", "f"},
                new[] { "z"},
            };

            byte[] bytes = NpyFormat.Save(value);

            string[][] expectedJagged =
            {
                new[] { "a", "b" },
                new[] { "d", "cs", "f" },
                new[] { "z" },
            };

            string[][] actualJagged = NpyFormat.Load<string[][]>(bytes);
            Assert.IsTrue(actualJagged.IsEqual(expectedJagged));
        }



#if !NET35 && !NET40
        [Test]
        public void WriteStringJaggedVariableColsTrimTest_Compressed()
        {
            string[][] value =
            {
                new[] { "a", "b" },
                new[] { "d", "cs", "f"},
                new[] { "z"},
            };

            byte[] bytes = NpzFormat.Save(value);

            string[][] expectedJagged =
            {
                new[] { "a", "b" },
                new[] { "d", "cs", "f" },
                new[] { "z" },
            };

            var arrJagged = NpzFormat.Load<string[][]>(bytes);
            string[][] actualJagged = arrJagged["arr_0"];
            Assert.IsTrue(actualJagged.IsEqual(expectedJagged));


            string[,] expectedMatrix =
            {
                { "a", "b", null},
                { "d", "cs", "f" },
                { "z", null, null },
            };

            var arrMatrix = NpzFormat.Load<string[,]>(bytes);
            string[,] actualMatrix = arrMatrix["arr_0"];
            Assert.IsTrue(actualJagged.IsEqual(expectedJagged));
        }
#endif
    }
}
