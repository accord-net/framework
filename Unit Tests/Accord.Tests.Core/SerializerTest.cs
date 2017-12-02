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

#if !NO_BINARY_SERIALIZATION

using System;
using Accord.IO;
using Accord.Math;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace Accord.Tests
{

    [TestFixture]
    public class SerializerTest
    {
        string path = TestContext.CurrentContext.TestDirectory;

        [Test, Ignore("this feature has been removed")]
        public void large_array_test()
        {
            string[][] expected = new string[300000][];
            for (int i = 0; i < expected.Length; i++)
            {
                expected[i] = new string[50];
                for (int j = 0; j < expected[i].Length; j++)
                    expected[i][j] = i.ToString() + j.ToString();
            }

            Serializer.Save(expected, "test.bin");

            string[][] actual;
            Serializer.Load("test.bin", out actual);

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void base_test()
        {
            #region doc_simple
            // The serializer class can be used to save and load Accord.NET 
            // models to and from the disk (and/or streams and byte arrays).

            // For example, we can create a new object:
            DoubleRange range = new DoubleRange(4, 2);

            // And we would like to save it as
            string filename = Path.Combine(path, "my_range.accord");

            // We can call Serializer.Save to save it:
            Serializer.Save(obj: range, path: filename);

            // Then, we can load it back using:
            Serializer.Load(filename, out range);

            // Or using the explicit generic method call:
            range = Serializer.Load<DoubleRange>(filename);
            #endregion

            Assert.AreEqual(range.Min, 4);
            Assert.AreEqual(range.Max, 2);
        }

        [Test]
        public void compression_test()
        {
            #region doc_compression
            // The serializer can also use compression

            // For example, we can create a new object:
            DoubleRange range = new DoubleRange(4, 2);

            // And we would like to save it as
            string filename = Path.Combine(path, "my_range.accord");

            // We can call Serializer.Save to save it:
            Serializer.Save(obj: range, path: filename,
                compression: SerializerCompression.GZip);

            // Please note that in this case, the serializer will 
            // append the ".gz" extension to the generated file, 
            // if it doesn't have it already.

            // Then, again we could load it back using:
            Serializer.Load(filename + ".gz", out range);
            #endregion

            Assert.AreEqual(range.Min, 4);
            Assert.AreEqual(range.Max, 2);
        }

        [Test]
        public void compression_test_auto_load()
        {
            var e = new Dictionary<int, string>();
            for (int i = 0; i < 100; i++)
                e[i] = e.ToString();

            string filename = Path.Combine(path, "test.bin.gz");
            Serializer.Save(e, filename, compression: SerializerCompression.GZip);

            Dictionary<int, string> a;
            Serializer.Load(filename, out a);

            foreach (int k in e.Keys)
                Assert.AreEqual(e[k], a[k]);
        }

        [Test]
        public void compression_test_auto_save()
        {
            var e = new Dictionary<int, string>();
            for (int i = 0; i < 100; i++)
                e[i] = e.ToString();

            string filename = Path.Combine(path, "test.bin.gz");

            Serializer.Save(e, filename);

            Dictionary<int, string> a;
            Serializer.Load(filename, out a, compression: SerializerCompression.GZip);

            foreach (int k in e.Keys)
                Assert.AreEqual(e[k], a[k]);
        }

        [Test]
        public void compression_test_override()
        {
            var e = new Dictionary<int, string>();
            for (int i = 0; i < 100; i++)
                e[i] = e.ToString();

            string filename = Path.Combine(path, "test.bin.gz");

            Serializer.Save(e, filename, compression: SerializerCompression.None);

            Dictionary<int, string> a;
            Serializer.Load(filename, out a, compression: SerializerCompression.None);

            foreach (int k in e.Keys)
                Assert.AreEqual(e[k], a[k]);
        }

        [Test]
        public void compression_test_stream()
        {
            using (var stream = new MemoryStream())
            {
                var e = new byte[] {1, 2, 3};
                Serializer.Save(e, stream, compression: SerializerCompression.GZip);

                stream.Position = 0;

                byte[] a;
                Serializer.Load(stream, out a, compression: SerializerCompression.GZip);

                CollectionAssert.AreEqual(e, a);
            }
        }
    }
}
#endif