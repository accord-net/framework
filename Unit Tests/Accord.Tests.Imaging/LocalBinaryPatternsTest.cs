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

namespace Accord.Tests.Imaging
{
    using System.Collections.Generic;
    using Accord.Imaging;
    using Accord.Imaging.Converters;
    using NUnit.Framework;
    using Accord.DataSets;
    using System.Drawing;
    using System.Linq;

    [TestFixture]
    public class LocalBinaryPatternTest
    {

        [Test]
        public void ComputeTest()
        {
            UnmanagedImage output = createGradient();

            LocalBinaryPattern lbp = new LocalBinaryPattern();
            List<double[]> result = lbp.ProcessImage(output);

            int[,] actualPatterns = lbp.Patterns;
            Assert.AreEqual(255, actualPatterns.GetLength(0));
            Assert.AreEqual(255, actualPatterns.GetLength(1));

            for (int i = 0; i < 255; i++)
            {
                for (int j = 0; j < 255; j++)
                {
                    if (j == 0 || i == 0 || i == 254 || j == 254)
                    {
                        Assert.AreEqual(0, actualPatterns[i, j]);
                    }
                    else
                    {
                        Assert.AreEqual(7, actualPatterns[i, j]);
                    }
                }
            }

            Assert.AreEqual(196, result.Count);
        }

        private static UnmanagedImage createGradient()
        {
            byte[,] gradient = new byte[255, 255];
            for (int i = 0; i < 255; i++)
                for (int j = 0; j < 255; j++)
                    gradient[i, j] = (byte)j;

            UnmanagedImage output;
            new MatrixToImage().Convert(gradient, out output);
            return output;
        }

        [Test]
        public void CloneTest()
        {
            LocalBinaryPattern original = new LocalBinaryPattern();
            LocalBinaryPattern target = (LocalBinaryPattern)original.Clone();

            UnmanagedImage output = createGradient();

            List<double[]> result = target.ProcessImage(output);

            int[,] actualPatterns = target.Patterns;
            Assert.AreEqual(255, actualPatterns.GetLength(0));
            Assert.AreEqual(255, actualPatterns.GetLength(1));

            for (int i = 0; i < 255; i++)
                for (int j = 0; j < 255; j++)
                    if (j == 0 || i == 0 || i == 254 || j == 254)
                        Assert.AreEqual(0, actualPatterns[i, j]);
                    else
                        Assert.AreEqual(7, actualPatterns[i, j]);

            Assert.AreEqual(196, result.Count);
        }

        [Test]
        public void doc_test()
        {
            string localPath = TestContext.CurrentContext.TestDirectory;

            #region doc_apply
            // Let's load an example image, such as Lena,
            // from a standard dataset of example images:
            var images = new TestImages(path: localPath);
            Bitmap lena = images["lena.bmp"];

            // Create a new Local Binary Pattern with default values:
            var lbp = new LocalBinaryPattern(blockSize: 3, cellSize: 6);

            // Use it to extract descriptors from the Lena image:
            List<double[]> descriptors = lbp.ProcessImage(lena);

            // Now those descriptors can be used to represent the image itself, such
            // as for example, in the Bag-of-Visual-Words approach for classification.
            #endregion

            Assert.AreEqual(784, descriptors.Count);
            double sum = descriptors.Sum(x => x.Sum());
            Assert.AreEqual(6094.543992693033, sum, 1e-10);
        }


    }
}
