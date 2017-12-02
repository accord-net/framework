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
    using System.Windows.Media.Imaging;
    using System.Windows.Media;
#if NO_BITMAP
    using Resources = Accord.Tests.Imaging.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class BitmapSourceToMatrixTest
    {
        [Test]
        public void ConvertTest()
        {
            var target = new BitmapSourceToMatrix();
            BitmapSource imageSource = TestTools.GetImage("image1.bmp");

            Bitmap bmp = imageSource.ToBitmap();
            new Invert().ApplyInPlace(bmp);
            new Threshold().ApplyInPlace(bmp);

            BitmapSource image = bmp.ToBitmapSource();

            double[,] output;
            double[,] outputExpected =
            {
                 { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 0
                 { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 1
                 { 0, 0,   1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   1, 0, 0 }, // 2 
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
                 { 0, 0,   1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   1, 0, 0 }, // 13
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
            BitmapSource sourceImage = TestTools.GetImage("image1.bmp");

            // Make sure values are binary
            sourceImage = new Threshold().Apply(sourceImage);

            // Create the converters
            var imageToMatrix = new BitmapSourceToMatrix();
            var matrixToImage = new MatrixToBitmapSource();

            // Convert to matrix
            double[,] matrix; // initialization is not needed
            imageToMatrix.Convert(sourceImage, out matrix);

            // Revert to image
            BitmapSource resultImage; // initialization is not needed
            matrixToImage.Convert(matrix, out resultImage);

            Assert.AreEqual(PixelFormats.Gray32Float, resultImage.Format);

            UnmanagedImage img1 = sourceImage.ToUnmanagedImage();
            UnmanagedImage img2 = resultImage.ToUnmanagedImage();

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


            var conv1 = new ArrayToBitmapSource(width: 4, height: 4);
            BitmapSource image;
            conv1.Convert(pixels, out image);
            image = new ResizeNearestNeighbor(16, 16).Apply(image);


            // Obtain an image
            // Bitmap image = ...

            // Show on screen
            //ImageBox.Show(image, PictureBoxSizeMode.Zoom);

            // Create the converter to convert the image to a
            //  matrix containing only values between 0 and 1 
            var conv = new BitmapSourceToMatrix();

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
            var target = new BitmapSourceToMatrix();
            BitmapSource image = Accord.Imaging.Image.Clone(Resources.image3).ToBitmapSource();
            Assert.AreEqual(System.Drawing.Imaging.PixelFormat.Format32bppArgb.ToWPF(), image.Format);

            {
                double[,] output;
                double[,] outputExpected =
                {
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 0
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 1
                     { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 2 
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
                     { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 13
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 14
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 15
                };

                target.Channel = 0;
                target.Convert(image, out output);

                double a = output[2, 2];
                double e = outputExpected[2, 2];
                Assert.AreEqual(e, a);

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
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 }, // 2 
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
                     { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 13
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 14
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 15
                };

                target.Channel = 1;
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
                     { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 }, // 13
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 14
                     { 0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 }, // 15
                };

                target.Channel = 2;
                target.Convert(image, out output);

                for (int i = 0; i < outputExpected.GetLength(0); i++)
                    for (int j = 0; j < outputExpected.GetLength(1); j++)
                        Assert.AreEqual(outputExpected[i, j], output[i, j]);
            }
        }

    }
}
