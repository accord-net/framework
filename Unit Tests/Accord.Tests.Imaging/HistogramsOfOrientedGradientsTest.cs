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
    using Accord.Imaging;
    using Accord.Imaging.Converters;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Drawing;
    using Accord.Math;
    using Accord.Tests.Imaging.Properties;
    using Accord.DataSets;
    using System.Linq;
#if NO_BITMAP
    using Resources = Accord.Tests.Imaging.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class HistogramsOfOrientedGradientsTest
    {
        public static Bitmap[] GetImages()
        {
            Bitmap[] images =
            {
                Accord.Imaging.Image.Clone(Resources.flower01),
                Accord.Imaging.Image.Clone(Resources.flower02),
                Accord.Imaging.Image.Clone(Resources.flower03),
                Accord.Imaging.Image.Clone(Resources.flower04),
                Accord.Imaging.Image.Clone(Resources.flower05),
                Accord.Imaging.Image.Clone(Resources.flower06),
            };

            return images;
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

            // Create a new Histogram of Oriented Gradients with the default parameter values:
            var hog = new HistogramsOfOrientedGradients(numberOfBins: 9, blockSize: 3, cellSize: 6);

            // Use it to extract descriptors from the Lena image:
            List<double[]> descriptors = hog.ProcessImage(lena);

            // Now those descriptors can be used to represent the image itself, such
            // as for example, in the Bag-of-Visual-Words approach for classification.
            #endregion

            Assert.AreEqual(784, descriptors.Count);
            double sum = descriptors.Sum(x => x.Sum());
            Assert.AreEqual(3359.1014569812564, sum, 1e-3);
        }

        [Test]
        public void MagnitudeDirectionTest()
        {
            byte[,] gradient = new byte[255, 255];

            for (int i = 0; i < 255; i++)
                for (int j = 0; j < 255; j++)
                    gradient[i, j] = (byte)j;

            UnmanagedImage output;
            new MatrixToImage().Convert(gradient, out output);

            //ImageBox.Show(output);
            var hog = new HistogramsOfOrientedGradients();
            var result = hog.ProcessImage(output);

            float[,] actualDir = hog.Direction;
            Assert.AreEqual(255, actualDir.GetLength(0));
            Assert.AreEqual(255, actualDir.GetLength(1));

            for (int i = 0; i < 255; i++)
                for (int j = 0; j < 255; j++)
                    Assert.AreEqual(0, actualDir[i, j]);

            float[,] actualMag = hog.Magnitude;
            Assert.AreEqual(255, actualMag.GetLength(0));
            Assert.AreEqual(255, actualMag.GetLength(1));

            for (int i = 0; i < 255; i++)
            {
                for (int j = 0; j < 255; j++)
                {
                    if (i == 0 || j == 0 || i == 254 || j == 254)
                        Assert.AreEqual(0, actualMag[i, j]);
                    else
                        Assert.AreEqual(1, actualMag[i, j]);
                }
            }
        }

        [Test]
        public void CloneTest()
        {
            var images = GetImages();

            var hog = new HistogramsOfOrientedGradients();
            var clone1 = (HistogramsOfOrientedGradients)hog.Clone();

            List<double[]> features1 = hog.ProcessImage(images[0]);
            Assert.AreEqual(features1.Count, 2352);

            List<double[]> features2 = clone1.ProcessImage(images[0]);
            Assert.AreEqual(features2.Count, 2352);
            Assert.IsTrue(features1.ToArray().IsEqual(features2.ToArray()));

            var clone2 = (HistogramsOfOrientedGradients)hog.Clone();
            List<double[]> features3 = clone2.ProcessImage(images[0]);
            Assert.AreEqual(features3.Count, 2352);
            Assert.IsTrue(features1.ToArray().IsEqual(features3.ToArray()));
        }
    }
}
