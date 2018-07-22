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
    using System.Drawing;
    using Accord.Imaging.Converters;
    using AForge;
    using Accord.Imaging;
    using Accord.Imaging.Filters;
    using NUnit.Framework;
    using Accord.Tests.Imaging.Properties;
    using System.Windows.Media.Imaging;
    using System.IO;
    using System;
    using System.Windows.Media;
#if NO_BITMAP
    using Resources = Accord.Tests.Imaging.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class MatrixToBitmapSourceTest
    {

        [Test]
        public void ConvertTest()
        {
            var target = new MatrixToBitmapSource();

            byte[,] input = 
            {
                {   0,   0,   0 },
                {   0, 128,   0 },
                {   0,   0, 128 },
            };

            UnmanagedImage bitmap = input.ToBitmapSource().ToUnmanagedImage();

            var pixels = bitmap.CollectActivePixels();

            Assert.AreEqual(2, pixels.Count);
            Assert.IsTrue(pixels.Contains(new IntPoint(1, 1)));
            Assert.IsTrue(pixels.Contains(new IntPoint(2, 2)));
        }



        [Test]
        public void ConvertTest1()
        {
            var target = new MatrixToBitmapSource();

            double[,] pixels =
            {
                 { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 0
                 { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 1
                 { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 }, // 2 
                 { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 3
                 { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 4
                 { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 5
                 { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 6
                 { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 7
                 { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 8
                 { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 9
                 { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 10
                 { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 11new Bitmap(Properties
                 { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 12
                 { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 }, // 13
                 { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 14
                 { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 15
            };

            BitmapSource imageActual = pixels.ToBitmapSource();

            double[,] actual = imageActual.ToMatrix(0);

            BitmapSource imageSourceExpected = TestTools.GetImage("image1.bmp");
            Bitmap imageExpected = imageSourceExpected.ToBitmap();
            new Threshold().ApplyInPlace(imageExpected);
            new Invert().ApplyInPlace(imageExpected);
            double[,] expected = imageExpected.ToMatrix(0);

            for (int i = 0; i < pixels.GetLength(0); i++)
                for (int j = 0; j < pixels.GetLength(1); j++)
                    Assert.AreEqual(actual[i, j], expected[i, j]);
        }


        [Test]
        public void ConvertTest2()
        {
            // Create a matrix representation 
            // of a 4x4 image with a inner 2x2
            // square drawn in the middle

            double[,] pixels = 
            {
                { 0, 0, 0, 0 },
                { 0, 1, 1, 0 },
                { 0, 1, 1, 0 },
                { 0, 0, 0, 0 },
            };

            // Create the converter to convert the matrix to a image
            var conv = new MatrixToBitmapSource();

            // Declare a bitmap source and store the pixels on it
            BitmapSource image; conv.Convert(pixels, out image);

            var conv2 = new MatrixToImage();
            Bitmap image2; conv2.Convert(pixels, out image2);

            Assert.AreEqual(pixels, image.ToMatrix(0));
            Assert.AreEqual(pixels, image2.ToMatrix(0));

            Assert.AreEqual(PixelFormats.Gray32Float, image.Format);
            Assert.AreEqual(System.Drawing.Imaging.PixelFormat.Format8bppIndexed, image2.PixelFormat);
        }

    }
}
