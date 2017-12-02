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

namespace Accord.Tests.Vision
{
    using Accord.Vision.Tracking;
    using NUnit.Framework;
    using System.Drawing;
    using Accord.Imaging;
    using Accord.Imaging.Converters;
    using Accord.MachineLearning;
    using Accord.Math.Distances;
    using Accord.Math;
    using Accord.Statistics.Distributions.DensityKernels;
    using System.IO;
    using Accord.DataSets;

    [TestFixture]
    public class ColorClusteringTest
    {

        [Test]
        public void kmeans()
        {
            string basePath = Path.Combine(NUnit.Framework.TestContext.CurrentContext.TestDirectory, "kmeans");
            Directory.CreateDirectory(basePath);

            #region doc_kmeans
            // Load a test image (shown in a picture box below)
            var sampleImages = new TestImages(path: basePath);
            Bitmap image = sampleImages.GetImage("airplane.png");

            // ImageBox.Show("Original", image).Hold();

            // Create converters to convert between Bitmap images and double[] arrays
            var imageToArray = new ImageToArray(min: -1, max: +1);
            var arrayToImage = new ArrayToImage(image.Width, image.Height, min: -1, max: +1);

            // Transform the image into an array of pixel values
            double[][] pixels; imageToArray.Convert(image, out pixels);


            // Create a K-Means algorithm using given k and a
            //  square Euclidean distance as distance metric.
            KMeans kmeans = new KMeans(k: 5)
            {
                Distance = new SquareEuclidean(),

                // We will compute the K-Means algorithm until cluster centroids
                // change less than 0.5 between two iterations of the algorithm
                Tolerance = 0.05
            };


            // Learn the clusters from the data
            var clusters = kmeans.Learn(pixels);

            // Use clusters to decide class labels
            int[] labels = clusters.Decide(pixels);

            // Replace every pixel with its corresponding centroid
            double[][] replaced = pixels.Apply((x, i) => clusters.Centroids[labels[i]]);

            // Retrieve the resulting image (shown in a picture box)
            Bitmap result; arrayToImage.Convert(replaced, out result);

            // ImageBox.Show("k-Means clustering", result).Hold();
            #endregion
        }


        [Test]
        public void meanShift()
        {
            string basePath = Path.Combine(NUnit.Framework.TestContext.CurrentContext.TestDirectory, "kmeans");
            Directory.CreateDirectory(basePath);

            #region doc_meanshift
            // Load a test image (shown in a picture box below)
            var sampleImages = new TestImages(path: basePath);
            Bitmap image = sampleImages.GetImage("airplane.png");

            // ImageBox.Show("Original", image).Hold();

            // Create converters to convert between Bitmap images and double[] arrays
            var imageToArray = new ImageToArray(min: -1, max: +1);
            var arrayToImage = new ArrayToImage(image.Width, image.Height, min: -1, max: +1);

            // Transform the image into an array of pixel values
            double[][] pixels; imageToArray.Convert(image, out pixels);

            // Create a MeanShift algorithm using given bandwidth
            //   and a Gaussian density kernel as kernel function.
            MeanShift meanShift = new MeanShift()
            {
                Kernel = new GaussianKernel(3),
                Bandwidth = 0.06,

                // We will compute the mean-shift algorithm until the means
                // change less than 0.05 between two iterations of the algorithm
                Tolerance = 0.05,
                MaxIterations = 10
            };

            // Learn the clusters from the data
            var clusters = meanShift.Learn(pixels);

            // Use clusters to decide class labels
            int[] labels = clusters.Decide(pixels);

            // Replace every pixel with its corresponding centroid
            double[][] replaced = pixels.Apply((x, i) => clusters.Modes[labels[i]]);

            // Retrieve the resulting image (shown in a picture box)
            Bitmap result; arrayToImage.Convert(replaced, out result);

            // ImageBox.Show("Mean-Shift clustering", result).Hold();
            #endregion
        }

    }
}
