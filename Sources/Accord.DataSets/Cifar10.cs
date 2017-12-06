// Accord Datasets Library
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

namespace Accord.DataSets
{
    using Accord.DataSets.Base;
    using Accord.Math;
    using System;
    using Accord.Compat;
    using System.Drawing;
    using System.IO;
    using ICSharpCode.SharpZipLib.Tar;
    using System.Collections.Generic;

    /// <summary>
    ///   Cifar-10 dataset of 32x32 images.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://www.cs.toronto.edu/~kriz/cifar.html">
    ///       Alex Krizhevsky. "The CIFAR-10 dataset.", 2009.</a>
    ///       </description></item>
    ///     <item><description><a href="https://www.cs.toronto.edu/~kriz/learning-features-2009-TR.pdf">
    ///       Alex Krizhevsky. "Learning Multiple Layers of Features from Tiny Images.", 2009.</a>
    ///       </description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    [Serializable]
    public class Cifar10 : WebDataSet
    {
        /// <summary>
        /// Gets the training set of the Cifar-10 dataset.
        /// </summary>
        /// 
        public Tuple<byte[][], int[]> Training { get; private set; }

        /// <summary>
        /// Gets the testing set of the Cifar-10 dataset.
        /// </summary>
        /// 
        public Tuple<byte[][], int[]> Testing { get; private set; }

        /// <summary>
        /// Gets or sets the class names in the Cifar-10 dataset.
        /// </summary>
        /// 
        public string[] ClassNames
        {
            get { return new[] { "airplane", "automobile", "bird", "cat", "deer", "dog", "frog", "horse" }; }
        }

        /// <summary>
        ///   Downloads and prepares the MNIST dataset.
        /// </summary>
        /// 
        /// <param name="path">The path where datasets will be stored. If null or empty, the dataset
        /// will be saved on a subfolder called "data" in the current working directory.</param>
        /// 
        public Cifar10(string path = null)
            : base(path)
        {
            // Download and uncompress the Cifar10 dataset
            string uncompressedFileName = Download("https://www.cs.toronto.edu/~kriz/cifar-10-binary.tar.gz");

            // Unpack the .tar
            string destinationFolder = uncompressedFileName.Remove(uncompressedFileName.Length - 4, 4);
            using (var inputStream = new FileStream(uncompressedFileName, FileMode.Open, FileAccess.Read))
            using (TarArchive tarArchive = TarArchive.CreateInputTarArchive(inputStream))
            {
                tarArchive.ExtractContents(destinationFolder);
            }

            // Load training set
            var trainingImages = new List<byte[]>();
            var trainingLabels = new List<int>();
            parse(destinationFolder, "cifar-10-batches-bin/data_batch_1.bin", trainingImages, trainingLabels);
            parse(destinationFolder, "cifar-10-batches-bin/data_batch_2.bin", trainingImages, trainingLabels);
            parse(destinationFolder, "cifar-10-batches-bin/data_batch_3.bin", trainingImages, trainingLabels);
            parse(destinationFolder, "cifar-10-batches-bin/data_batch_4.bin", trainingImages, trainingLabels);
            parse(destinationFolder, "cifar-10-batches-bin/data_batch_5.bin", trainingImages, trainingLabels);
            this.Training = Tuple.Create(trainingImages.ToArray(), trainingLabels.ToArray());

            // Load testing set
            var testingImages = new List<byte[]>();
            var testingLabels = new List<int>();
            parse(destinationFolder, "cifar-10-batches-bin/test_batch.bin", testingImages, testingLabels);
            this.Testing = Tuple.Create(testingImages.ToArray(), testingLabels.ToArray());
        }

        private void parse(string rootDir, string filePath, List<byte[]> images, List<int> labels)
        {
            string path = System.IO.Path.Combine(rootDir, filePath);
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(stream))
            {
                // Each file contain 10,000 images:
                for (int i = 0; i < 10000; i++)
                {
                    int label = reader.ReadByte(); // class label is represented as a single byte
                    byte[] image = reader.ReadBytes(32 * 32 * 3); // all images are 32x32 and RGB

                    labels.Add(label);
                    images.Add(image);
                }
            }
        }
    }
}
