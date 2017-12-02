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
#if NO_DEFAULT_ENCODING
    using Encoding = Accord.Compat.Encoding;
#endif

    [TestFixture]
    public class SparseWriterTest
    {

        string test_txt = Path.Combine(TestContext.CurrentContext.TestDirectory, "test.txt");
        string test2_txt = Path.Combine(TestContext.CurrentContext.TestDirectory, "test2.txt");
        string test_txt_gz = Path.Combine(TestContext.CurrentContext.TestDirectory, "test.txt.gz");


        [Test]
        public void WriteSamplesTest()
        {
            // http://www.csie.ntu.edu.tw/~cjlin/libsvmtools/datasets/multiclass.html#iris

            MemoryStream file = new MemoryStream(Encoding.Default.GetBytes(Accord.Tests.IO.Properties.Resources.iris_scale));

            int sampleSize = 4;

            SparseReader src = new SparseReader(stream: file, encoding: Encoding.Default, sampleSize: sampleSize);
            SparseWriter dst = new SparseWriter(test_txt, Encoding.Default);

            // Read a sample from the file
            var r = src.ReadDense();
            dst.Write(r.Item1, r.Item2);

            var s = src.ReadSparse();
            dst.Write(s.Item1, r.Item2);

            int count = 2;

            // Read all samples from the file
            while (!src.EndOfStream)
            {
                r = src.ReadDense();
                dst.Write(r.Item1, r.Item2);
                count++;
            }

            Assert.AreEqual(150, count);

            dst.Dispose();
            src.Dispose();


            file = new MemoryStream(Encoding.Default.GetBytes(Accord.Tests.IO.Properties.Resources.iris_scale));
            using (SparseReader orig = new SparseReader(stream: file, encoding: Encoding.Default, sampleSize: sampleSize))
            using (SparseReader copy = new SparseReader(test_txt, Encoding.Default, sampleSize))
            {
                while (!orig.EndOfStream)
                {
                    var expected = orig.ReadDense();
                    var actual = copy.ReadDense();

                    Assert.AreEqual(actual.Item1, expected.Item1);
                    Assert.AreEqual(actual.Item2, expected.Item2);
                }

                Assert.AreEqual(orig.EndOfStream, copy.EndOfStream);
            }

            file = new MemoryStream(Encoding.Default.GetBytes(Accord.Tests.IO.Properties.Resources.iris_scale));
            using (SparseReader orig = new SparseReader(stream: file, encoding: Encoding.Default, sampleSize: sampleSize))
            using (SparseReader copy = new SparseReader(test_txt, Encoding.Default))
            {
                while (!orig.EndOfStream)
                {
                    var expected = orig.ReadSparse();
                    var actual = copy.ReadSparse();

                    Assert.AreEqual(actual.Item1.Values, expected.Item1.Values);
                    Assert.AreEqual(actual.Item1.Indices, expected.Item1.Indices);
                    Assert.AreEqual(actual.Item2, expected.Item2);
                }

                Assert.AreEqual(orig.EndOfStream, copy.EndOfStream);
            }
        }

        [Test]
        public void WriteBoolTest()
        {
            double[][] samples = new[]
            {
                new double[] { 1, 2, 0, 3, 0 },
                new double[] { 6, 0, 4, 2, 0 },
                new double[] { 0, 0, 0, 0, 0 },
            };

            bool[] outputs = { false, true, false };

            SparseFormat.Save(samples, outputs, test_txt);

            string actual = File.ReadAllText(test_txt);
            string expected = @"-1 1:1 2:2 4:3
1 1:6 3:4 4:2
-1 
";
            expected = expected.Replace("\r\n", Environment.NewLine);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WriteBoolTest_compressed()
        {
            double[][] samples = new[]
            {
                new double[] { 1, 2, 0, 3, 0 },
                new double[] { 6, 0, 4, 2, 0 },
                new double[] { 0, 0, 0, 0, 0 },
            };

            bool[] outputs = { false, true, false };

            SparseFormat.Save(samples, outputs, test_txt_gz, compression: SerializerCompression.GZip);

            Sparse<double>[] newSamples;
            bool[] newOutput;
            SparseFormat.Load(test_txt_gz, out newSamples, out newOutput, compression: SerializerCompression.GZip);


            SparseFormat.Save(newSamples, newOutput, Path.Combine(TestContext.CurrentContext.TestDirectory, "test2.txt"));

            string actual = File.ReadAllText(test2_txt);
            string expected = @"-1 1:1 2:2 4:3
1 1:6 3:4 4:2
-1 
";
            expected = expected.Replace("\r\n", Environment.NewLine);

            Assert.AreEqual(expected, actual);
        }
    }
}
