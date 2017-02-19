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

using Accord.IO;
using Accord.Math;
using NUnit.Framework;
using System.Collections.Generic;

namespace Accord.Tests
{

    [TestFixture]
    public class SerializerTest
    {
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
        public void compression_test_auto_load()
        {
            var e = new Dictionary<int, string>();
            for (int i = 0; i < 100; i++)
                e[i] = e.ToString();

            Serializer.Save(e, "test.bin.gz", compression: SerializerCompression.GZip);

            Dictionary<int, string> a;
            Serializer.Load("test.bin.gz", out a);

            foreach (int k in e.Keys)
                Assert.AreEqual(e[k], a[k]);
        }

        [Test]
        public void compression_test_auto_save()
        {
            var e = new Dictionary<int, string>();
            for (int i = 0; i < 100; i++)
                e[i] = e.ToString();

            Serializer.Save(e, "test.bin.gz");

            Dictionary<int, string> a;
            Serializer.Load("test.bin.gz", out a, compression: SerializerCompression.GZip);

            foreach (int k in e.Keys)
                Assert.AreEqual(e[k], a[k]);
        }

        [Test]
        public void compression_test_override()
        {
            var e = new Dictionary<int, string>();
            for (int i = 0; i < 100; i++)
                e[i] = e.ToString();

            Serializer.Save(e, "test.bin.gz", compression: SerializerCompression.None);

            Dictionary<int, string> a;
            Serializer.Load("test.bin.gz", out a, compression: SerializerCompression.None);

            foreach (int k in e.Keys)
                Assert.AreEqual(e[k], a[k]);
        }
    }
}
