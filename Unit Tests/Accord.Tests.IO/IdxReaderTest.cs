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
    using System.IO;
    using Accord.IO;
    using Accord.Tests.IO.Properties;
    using NUnit.Framework;

    [TestFixture]
    public class IdxReaderTest
    {


        [Test]
        public void ReadSampleTest()
        {
            string fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "t10k-images-idx3-ubyte.gz");

            IdxReader reader = new IdxReader(fileName, compressed: true);

            Assert.AreEqual(IdxDataType.UnsignedByte, reader.DataType);
            Assert.AreEqual(10000, reader.Samples);

            var samples = new byte[reader.Samples][,];

            for (int i = 0; i < samples.Length; i++)
                samples[i] = reader.ReadMatrix<byte>();

            for (int i = 0; i < samples.Length; i++)
            {
                Assert.AreEqual(28, samples[i].GetLength(0));
                Assert.AreEqual(28, samples[i].GetLength(1));
            }
        }

        [Test]
        public void ReadSampleTest2()
        {
            string fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "t10k-images-idx3-ubyte.gz");

            IdxReader reader = new IdxReader(fileName, compressed: true);

            var samples = reader.ReadToEndAsMatrices<byte>();

            for (int i = 0; i < samples.Length; i++)
            {
                Assert.AreEqual(28, samples[i].GetLength(0));
                Assert.AreEqual(28, samples[i].GetLength(1));
            }
        }

        [Test]
        public void ReadSampleTest3()
        {
            string fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "t10k-images-idx3-ubyte.gz");
            var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            IdxReader reader = new IdxReader(stream, compressed: true);

            var samples = reader.ReadToEndAsVectors<byte>();

            for (int i = 0; i < samples.Length; i++)
                Assert.AreEqual(28 * 28, samples[i].Length);
        }

    }
}
