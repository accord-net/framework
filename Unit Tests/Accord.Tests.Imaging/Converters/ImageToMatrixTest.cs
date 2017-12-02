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
    using System.Drawing;
    using System.Drawing.Imaging;
    using Accord.Imaging.Converters;
    using Accord.Math;
    using AForge;
    using Accord.Imaging;
    using Accord.Imaging.Filters;
    using NUnit.Framework;
    using Accord.Tests.Imaging.Properties;
#if NO_BITMAP
    using Resources = Accord.Tests.Imaging.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class ImageToMatrixTest
    {

        [Test]
        public void ImageToMatrixConstructorTest()
        {
            double min = -10;
            double max = +10;
            ImageToMatrix target = new ImageToMatrix(min, max);

            Assert.AreEqual(min, target.Min);
            Assert.AreEqual(max, target.Max);
            Assert.AreEqual(0, target.Channel);
        }

        [Test]
        public void ImageToMatrixConstructorTest1()
        {
            ImageToMatrix target = new ImageToMatrix();

            Assert.AreEqual(0, target.Min);
            Assert.AreEqual(1, target.Max);
            Assert.AreEqual(0, target.Channel);
        }

        [Test]
        public void ImageToMatrixConstructorTest2()
        {
            double min = -10;
            double max = +10;
            int channel = 2;
            ImageToMatrix target = new ImageToMatrix(min, max, channel);

            Assert.AreEqual(min, target.Min);
            Assert.AreEqual(max, target.Max);
            Assert.AreEqual(channel, target.Channel);
        }

        [Test]
        public void ConvertTest()
        {
            ImageToMatrix target = new ImageToMatrix(min: 0, max: 255);
            Bitmap image = Accord.Imaging.Image.Clone(Resources.image1);

            new Invert().ApplyInPlace(image);
            new Threshold().ApplyInPlace(image);

            double[,] output;
            double[,] outputExpected =
            {
                 { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 0
                 { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 1
                 { 0, 0, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 0, 0 }, // 2 
                 { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 3
                 { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 4
                 { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 5
                 { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 6
                 { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 7
                 { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 8
                 { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 9
                 { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 10
                 { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 11
                 { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 12
                 { 0, 0, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 0, 0 }, // 13
                 { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 14
                 { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 15
            };


            target.Convert(image, out output);

            for (int i = 0; i < outputExpected.GetLength(0); i++)
                for (int j = 0; j < outputExpected.GetLength(1); j++)
                    Assert.AreEqual(outputExpected[i, j], output[i, j]);
        }

        [Test]
        public void ConvertTest2()
        {
            // Load a test image
            Bitmap sourceImage = Accord.Imaging.Image.Clone(Resources.image1);

            // Make sure values are binary
            new Threshold().ApplyInPlace(sourceImage);

            // Create the converters
            ImageToMatrix imageToMatrix = new ImageToMatrix() { Min = 0, Max = 255 };
            MatrixToImage matrixToImage = new MatrixToImage() { Min = 0, Max = 255 };

            // Convert to matrix
            double[,] matrix; // initialization is not needed
            imageToMatrix.Convert(sourceImage, out matrix);

            // Revert to image
            Bitmap resultImage; // initialization is not needed
            matrixToImage.Convert(matrix, out resultImage);

            // Show both images, which should be equal
            // ImageBox.Show(sourceImage, PictureBoxSizeMode.Zoom);
            // ImageBox.Show(resultImage, PictureBoxSizeMode.Zoom);

            UnmanagedImage img1 = UnmanagedImage.FromManagedImage(sourceImage);
            UnmanagedImage img2 = UnmanagedImage.FromManagedImage(resultImage);

            List<IntPoint> p1 = img1.CollectActivePixels();
            List<IntPoint> p2 = img2.CollectActivePixels();

            bool equals = new HashSet<IntPoint>(p1).SetEquals(p2);

            Assert.IsTrue(equals);
        }

        [Test]
        public void ConvertTest3()
        {
            double[] pixels = 
            {
                 0, 0, 0, 0,
                 0, 1, 1, 0,
                 0, 1, 1, 0,
                 0, 0, 0, 0,
            };


            ArrayToImage conv1 = new ArrayToImage(width: 4, height: 4);
            Bitmap image;
            conv1.Convert(pixels, out image);
            image = new ResizeNearestNeighbor(16, 16).Apply(image);


            // Obtain an image
            // Bitmap image = ...

            // Show on screen
            //ImageBox.Show(image, PictureBoxSizeMode.Zoom);

            // Create the converter to convert the image to a
            //  matrix containing only values between 0 and 1 
            ImageToMatrix conv = new ImageToMatrix(min: 0, max: 1);

            // Convert the image and store it in the matrix
            double[,] matrix; conv.Convert(image, out matrix);

            /*
                        // Show the matrix on screen as an image
                        ImageBox.Show(matrix, PictureBoxSizeMode.Zoom);


                        // Show the matrix on screen as a .NET multidimensional array
                        MessageBox.Show(matrix.ToString(CSharpMatrixFormatProvider.InvariantCulture));

                        // Show the matrix on screen as a table
                        DataGridBox.Show(matrix, nonBlocking: true)
                            .SetAutoSizeColumns(DataGridViewAutoSizeColumnsMode.Fill)
                            .SetAutoSizeRows(DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders)
                            .SetDefaultFontSize(5)
                            .WaitForClose();
            */

            Assert.AreEqual(0, matrix.Min());
            Assert.AreEqual(1, matrix.Max());
            Assert.AreEqual(16 * 16, matrix.Length);
        }

        [Test]
        public void ConvertTest4()
        {
            ImageToMatrix target = new ImageToMatrix(min: 0, max: 255);
            Bitmap image = Accord.Imaging.Image.Clone(Resources.image3);
            Assert.AreEqual(PixelFormat.Format32bppArgb, image.PixelFormat);

            {
                double[,] output;
                double[,] outputExpected =
                {
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 0
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 1
                     { 0, 0, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 2 
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 3
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 4
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 5
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 6
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 7
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 8
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 9
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 10
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 11
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 12
                     { 0, 0, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 13
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 14
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 15
                };

                target.Channel = RGB.R;
                target.Convert(image, out output);

                for (int i = 0; i < outputExpected.GetLength(0); i++)
                    for (int j = 0; j < outputExpected.GetLength(1); j++)
                        Assert.AreEqual(outputExpected[i, j], output[i, j]);
            }

            {
                double[,] output;
                double[,] outputExpected =
                {
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 0
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 1
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 0, 0 }, // 2 
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 3
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 4
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 5
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 6
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 7
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 8
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 9
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 10
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 11
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 12
                     { 0, 0, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 13
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 14
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 15
                };

                target.Channel = RGB.G;
                target.Convert(image, out output);

                for (int i = 0; i < outputExpected.GetLength(0); i++)
                    for (int j = 0; j < outputExpected.GetLength(1); j++)
                        Assert.AreEqual(outputExpected[i, j], output[i, j]);
            }

            {
                double[,] output;
                double[,] outputExpected =
                {
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 0
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 1
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 2 
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 3
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 4
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 5
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 6
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 7
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 8
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 9
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 10
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 11
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 12
                     { 0, 0, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 0, 0 }, // 13
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 14
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 15
                };

                target.Channel = RGB.B;
                target.Convert(image, out output);

                for (int i = 0; i < outputExpected.GetLength(0); i++)
                    for (int j = 0; j < outputExpected.GetLength(1); j++)
                        Assert.AreEqual(outputExpected[i, j], output[i, j]);
            }
        }

    }
}
