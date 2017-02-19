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
    public class NpyFormatTest
    {

        [Test]
        public void ReadByteMatrixTest()
        {
            MemoryStream ms = new MemoryStream(Resources.npy_byte);
            Array result = NpyFormat.LoadMatrix(ms);

            byte[,] expected =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            Assert.IsTrue(result.IsEqual(expected));
        }

        [Test]
        public void ReadByteJaggedTest()
        {
            MemoryStream ms = new MemoryStream(Resources.npy_byte);
            Array result = NpyFormat.LoadJagged(ms);

            byte[][] expected =
            {
                new byte[] { 1, 2, 3 },
                new byte[] { 4, 5, 6 }
            };

            Assert.IsTrue(result.IsEqual(expected));
        }

        [Test]
        public void ReadBoolMatrixTest()
        {
            MemoryStream ms = new MemoryStream(Resources.npy_bool);
            Array result = NpyFormat.LoadMatrix(ms);

            bool[,] expected =
            {
                { true, true, false },
                { false, false, false }
            };

            Assert.IsTrue(result.IsEqual(expected));
        }

        [Test]
        public void ReadBoolJaggedTest()
        {
            MemoryStream ms = new MemoryStream(Resources.npy_bool);
            Array result = NpyFormat.LoadJagged(ms);

            bool[][] expected =
            {
                new[] { true, true, false },
                new[] { false, false, false }
            };

            Assert.IsTrue(result.IsEqual(expected));
        }

        [Test]
        public void ReadInt64MatrixTest()
        {
            MemoryStream ms = new MemoryStream(Resources.npy_integer);
            Array result = NpyFormat.LoadMatrix(ms);

            long[,] expected =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            Assert.IsTrue(result.IsEqual(expected));
        }

        [Test]
        public void ReadStringMatrixTest()
        {
            MemoryStream ms = new MemoryStream(Resources.npy_strings);
            Array result = NpyFormat.LoadMatrix(ms);

            string[,] expected =
            {
                { "a", "b", "c" },
                { "d", "e", "f"}
            };

            Assert.IsTrue(result.IsEqual(expected));
        }

        [Test]
        public void ReadStringMatrixVariableLengthTest()
        {
            MemoryStream ms = new MemoryStream(Resources.npy_strings_var);
            Array result = NpyFormat.LoadMatrix(ms);

            string[,] expected =
            {
                { "a", "b", "abacaxi" },
                { "d", "cs", "f"}
            };

            Assert.IsTrue(result.IsEqual(expected));
        }

        [Test]
        public void ReadStringJaggedVariableLengthTest()
        {
            MemoryStream ms = new MemoryStream(Resources.npy_strings_var);
            Array result = NpyFormat.LoadJagged(ms);

            string[][] expected =
            {
                new[] { "a", "b", "abacaxi" },
                new[] { "d", "cs", "f"}
            };

            Assert.IsTrue(result.IsEqual(expected));
        }

    }
}
