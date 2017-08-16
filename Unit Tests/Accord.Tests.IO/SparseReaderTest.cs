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
    public class SparseReaderTest
    {

        [Test]
        public void ReadSampleTest()
        {
            // http://www.csie.ntu.edu.tw/~cjlin/libsvmtools/datasets/multiclass.html#iris

            MemoryStream file = new MemoryStream(
                Encoding.Default.GetBytes(Accord.Tests.IO.Properties.Resources.iris_scale));

            // Suppose we are going to read a sparse sample file containing
            //  samples which have an actual dimension of 4. Since the samples
            //  are in a sparse format, each entry in the file will probably
            //  have a much lesser number of elements.
            int sampleSize = 4;

            // Create a new Sparse Sample Reader to read any given file,
            //  passing the correct dense sample size in the constructor
            SparseReader reader = new SparseReader(stream: file, encoding: Encoding.Default, sampleSize: sampleSize);

            // Declare some variables to receive each current sample
            int label = 0;
            string description;
            double[] sample;

            // Read a sample from the file
            var r = reader.ReadDense();
            sample = r.Item1;
            label = (int)r.Item2;
            description = reader.SampleDescriptions[0];

            Assert.AreEqual(1, label);
            Assert.AreEqual(String.Empty, description);

            Assert.AreEqual(4, sample.Length);
            Assert.AreEqual(-0.555556, sample[0], 0.0001);
            Assert.AreEqual(+0.250000, sample[1], 0.0001);
            Assert.AreEqual(-0.864407, sample[2], 0.0001);
            Assert.AreEqual(-0.916667, sample[3], 0.0001);

            var s = reader.ReadSparse();
            sample = s.Item1.ToSparse();
            label = (int)s.Item2;
            description = reader.SampleDescriptions[0];

            Assert.AreEqual(1, label);
            Assert.AreEqual(String.Empty, description);

            Assert.AreEqual(8, sample.Length);
            Assert.AreEqual(0, sample[0], 0.0001);
            Assert.AreEqual(-0.666667, sample[1], 0.0001);
            Assert.AreEqual(1, sample[2], 0.0001);
            Assert.AreEqual(-0.166667, sample[3], 0.0001);
            Assert.AreEqual(2, sample[4], 0.0001);
            Assert.AreEqual(-0.864407, sample[5], 0.0001);
            Assert.AreEqual(3, sample[6], 0.0001);
            Assert.AreEqual(-0.916667, sample[7], 0.0001);


            int count = 2;

            // Read all samples from the file
            while (!reader.EndOfStream)
            {
                reader.SampleDescriptions.Clear();
                r = reader.ReadDense();
                sample = r.Item1;
                label = (int)r.Item2;
                description = reader.SampleDescriptions[0];
                Assert.IsTrue(label >= 0 && label <= 3);
                Assert.IsTrue(description == String.Empty);
                Assert.AreEqual(4, sample.Length);
                count++;
            }

            Assert.AreEqual(150, count);
        }

        [Test]
        public void ReadAllTest()
        {
            MemoryStream file = new MemoryStream(
                Encoding.Default.GetBytes(Resources.iris_scale));

            // Suppose we are going to read a sparse sample file containing
            //  samples which have an actual dimension of 4. Since the samples
            //  are in a sparse format, each entry in the file will probably
            //  have a much lesser number of elements.
            int sampleSize = 4;

            // Create a new Sparse Sample Reader to read any given file,
            //  passing the correct dense sample size in the constructor
            SparseReader reader = new SparseReader(stream: file, encoding: Encoding.Default, sampleSize: sampleSize);

            // Declare a vector to obtain the label
            //  of each of the samples in the file

            // Declare a vector to obtain the description (or comments)
            //  about each of the samples in the file, if present.

            // Read the sparse samples and store them in a dense vector array
            var r = reader.ReadDenseToEnd();
            double[][] samples = r.Item1;
            int[] labels = r.Item2.ToInt32();
            string[] descriptions = reader.SampleDescriptions.ToArray();

            Assert.AreEqual(150, samples.Length);

            for (int i = 0; i < 150; i++)
            {
                Assert.IsTrue(labels[i] >= 0 && labels[i] <= 3);
                Assert.IsTrue(descriptions[i] == String.Empty);
                Assert.AreEqual(4, samples[i].Length);
            }
        }

        [Test]
        public void GuessNumberOfDimensionsTest()
        {
            MemoryStream file = new MemoryStream(
                Encoding.Default.GetBytes(Resources.iris_scale));

            SparseReader reader = new SparseReader(stream: file, encoding: Encoding.Default);

            Assert.AreEqual(4, reader.Dimensions);


            var r = reader.ReadDenseToEnd();
            double[][] samples = r.Item1;
            int[] labels = r.Item2.ToInt32();
            string[] descriptions = reader.SampleDescriptions.ToArray();


            Assert.AreEqual(150, samples.Length);

            for (int i = 0; i < 150; i++)
            {
                Assert.IsTrue(labels[i] >= 0 && labels[i] <= 3);
                Assert.IsTrue(descriptions[i] == String.Empty);
                Assert.AreEqual(4, samples[i].Length);
            }
        }

        [Test]
        public void DimensionsTest()
        {
            MemoryStream file = new MemoryStream(
                Encoding.Default.GetBytes(Resources.a9a_train));

            SparseReader reader = new SparseReader(stream: file, encoding: Encoding.Default);

            Assert.AreEqual(123, reader.Dimensions);

            var r = reader.ReadDenseToEnd();
            double[][] samples = r.Item1;
            int[] labels = r.Item2.ToInt32();
            string[] descriptions = reader.SampleDescriptions.ToArray();

            Assert.AreEqual(26049, samples.Length);
            for (int i = 0; i < labels.Length; i++)
            {
                Assert.IsTrue(labels[i] == -1 || labels[i] == 1);
                Assert.IsTrue(descriptions[i] == String.Empty);
                Assert.AreEqual(123, samples[i].Length);
            }
        }

        [Test]
        public void GuessDimensionsInMiddleRunTest()
        {
            MemoryStream file = new MemoryStream(
                Encoding.Default.GetBytes(Resources.a9a_train));

            SparseReader reader = new SparseReader(stream: file, encoding: Encoding.Default);

            var r = reader.ReadDenseToEnd();
            double[][] samples = r.Item1;
            int[] labels = r.Item2.ToInt32();
            string[] descriptions = reader.SampleDescriptions.ToArray();

            Assert.AreEqual(26049, samples.Length);
            for (int i = 0; i < labels.Length; i++)
            {
                Assert.IsTrue(labels[i] == -1 || labels[i] == 1);
                Assert.IsTrue(descriptions[i] == String.Empty);
                Assert.AreEqual(123, samples[i].Length);
            }
        }
    }
}
