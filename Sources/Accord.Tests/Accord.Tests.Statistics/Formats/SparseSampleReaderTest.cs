// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;
using System.Text;
using Accord.Statistics.Formats;

namespace Accord.Tests.Statistics
{


    /// <summary>
    ///This is a test class for SparseSampleReaderTest and is intended
    ///to contain all SparseSampleReaderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SparseSampleReaderTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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
        public void ReadSampleTest()
        {
            // http://www.csie.ntu.edu.tw/~cjlin/libsvmtools/datasets/multiclass.html#iris

            MemoryStream file = new MemoryStream(
                Encoding.Default.GetBytes(Properties.Resources.iris_scale));

            // Suppose we are going to read a sparse sample file containing
            //  samples which have an actual dimension of 4. Since the samples
            //  are in a sparse format, each entry in the file will probably
            //  have a much lesser number of elements.
            int sampleSize = 4;

            // Create a new Sparse Sample Reader to read any given file,
            //  passing the correct dense sample size in the constructor
            SparseSampleReader reader = new SparseSampleReader(file, Encoding.Default, sampleSize);

            // Declare some variables to receive each current sample
            int label = 0;
            string description;
            double[] sample;

            // Read a sample from the file
            sample = reader.ReadDense(out label, out description);

            Assert.AreEqual(1, label);
            Assert.AreEqual(String.Empty, description);

            Assert.AreEqual(4, sample.Length);
            Assert.AreEqual(-0.555556, sample[0], 0.0001);
            Assert.AreEqual(+0.250000, sample[1], 0.0001);
            Assert.AreEqual(-0.864407, sample[2], 0.0001);
            Assert.AreEqual(-0.916667, sample[3], 0.0001);

            sample = reader.ReadSparse(out label, out description);

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
                sample = reader.ReadDense(out label, out description);
                Assert.IsTrue(label >= 0 && label <= 3);
                Assert.IsTrue(description == String.Empty);
                Assert.AreEqual(4, sample.Length);
                count++;
            }

            Assert.AreEqual(150, count);
        }

        /// <summary>
        ///A test for ReadAll
        ///</summary>
        [TestMethod()]
        public void ReadAllTest()
        {
            MemoryStream file = new MemoryStream(
                Encoding.Default.GetBytes(Properties.Resources.iris_scale));

            // Suppose we are going to read a sparse sample file containing
            //  samples which have an actual dimension of 4. Since the samples
            //  are in a sparse format, each entry in the file will probably
            //  have a much lesser number of elements.
            int sampleSize = 4;

            // Create a new Sparse Sample Reader to read any given file,
            //  passing the correct dense sample size in the constructor
            SparseSampleReader reader = new SparseSampleReader(file, Encoding.Default, sampleSize);

            // Declare a vector to obtain the label
            //  of each of the samples in the file
            int[] labels = null;

            // Declare a vector to obtain the description (or comments)
            //  about each of the samples in the file, if present.
            string[] descriptions = null;

            // Read the sparse samples and store them in a dense vector array
            double[][] samples = reader.ReadToEnd(out labels, out descriptions);

            Assert.AreEqual(150, samples.Length);

            for (int i = 0; i < 150; i++)
            {
                Assert.IsTrue(labels[i] >= 0 && labels[i] <= 3);
                Assert.IsTrue(descriptions[i] == String.Empty);
                Assert.AreEqual(4, samples[i].Length);
            }
        }
    }
}
