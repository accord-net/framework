﻿// Copyright © César Souza, 2009-2017
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

namespace Accord.Imaging.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge;
    using Accord.Imaging;
    using NUnit.Framework;
    using Accord.Math;

    [TestFixture]
    public class UnmanagedImageTest
    {
        [Test]
        public void Collect8bppPixelValuesTest_Grayscale()
        {
            // create grayscale image
            UnmanagedImage image = UnmanagedImage.Create(320, 240, PixelFormat.Format8bppIndexed);

            // draw vertical and horizontal lines
            Drawing.Line(image, new IntPoint(10, 10), new IntPoint(20, 10), Color.FromArgb(128, 128, 128));
            Drawing.Line(image, new IntPoint(20, 20), new IntPoint(20, 30), Color.FromArgb(64, 64, 64));

            // prepare lists with coordinates
            List<IntPoint> horizontal = new List<IntPoint>();
            List<IntPoint> horizontalU = new List<IntPoint>();
            List<IntPoint> horizontalD = new List<IntPoint>();

            for (int x = 10; x <= 20; x++)
            {
                horizontal.Add(new IntPoint(x, 10));  // on the line
                horizontalU.Add(new IntPoint(x, 9));  // above
                horizontalD.Add(new IntPoint(x, 11)); // below
            }

            List<IntPoint> vertical = new List<IntPoint>();
            List<IntPoint> verticalL = new List<IntPoint>();
            List<IntPoint> verticalR = new List<IntPoint>();

            for (int y = 20; y <= 30; y++)
            {
                vertical.Add(new IntPoint(20, y));    // on the line
                verticalL.Add(new IntPoint(19, y));   // left
                verticalR.Add(new IntPoint(21, y));   // right
            }

            // collect all pixel's values
            byte[] horizontalValues = image.Collect8bppPixelValues(horizontal);
            byte[] horizontalUValues = image.Collect8bppPixelValues(horizontalU);
            byte[] horizontalDValues = image.Collect8bppPixelValues(horizontalD);
            byte[] verticalValues = image.Collect8bppPixelValues(vertical);
            byte[] verticalLValues = image.Collect8bppPixelValues(verticalL);
            byte[] verticalRValues = image.Collect8bppPixelValues(verticalR);

            Assert.AreEqual(horizontal.Count, horizontalValues.Length);
            Assert.AreEqual(vertical.Count, verticalValues.Length);

            // check all pixel values
            for (int i = 0, n = horizontalValues.Length; i < n; i++)
            {
                Assert.AreEqual(128, horizontalValues[i]);
                Assert.AreEqual(0, horizontalUValues[i]);
                Assert.AreEqual(0, horizontalDValues[i]);
            }

            for (int i = 0, n = verticalValues.Length; i < n; i++)
            {
                Assert.AreEqual(64, verticalValues[i]);
                Assert.AreEqual(0, verticalLValues[i]);
                Assert.AreEqual(0, verticalRValues[i]);
            }
        }

        [Test]
        public void Collect8bppPixelValuesTest_RGB()
        {
            // create grayscale image
            UnmanagedImage image = UnmanagedImage.Create(320, 240, PixelFormat.Format24bppRgb);

            // draw vertical and horizontal lines
            Drawing.Line(image, new IntPoint(10, 10), new IntPoint(20, 10), Color.FromArgb(128, 129, 130));
            Drawing.Line(image, new IntPoint(20, 20), new IntPoint(20, 30), Color.FromArgb(64, 65, 66));

            // prepare lists with coordinates
            List<IntPoint> horizontal = new List<IntPoint>();
            List<IntPoint> horizontalU = new List<IntPoint>();
            List<IntPoint> horizontalD = new List<IntPoint>();

            for (int x = 10; x <= 20; x++)
            {
                horizontal.Add(new IntPoint(x, 10));  // on the line
                horizontalU.Add(new IntPoint(x, 9));  // above
                horizontalD.Add(new IntPoint(x, 11)); // below
            }

            List<IntPoint> vertical = new List<IntPoint>();
            List<IntPoint> verticalL = new List<IntPoint>();
            List<IntPoint> verticalR = new List<IntPoint>();

            for (int y = 20; y <= 30; y++)
            {
                vertical.Add(new IntPoint(20, y));    // on the line
                verticalL.Add(new IntPoint(19, y));   // left
                verticalR.Add(new IntPoint(21, y));   // right
            }

            // collect all pixel's values
            byte[] horizontalValues = image.Collect8bppPixelValues(horizontal);
            byte[] horizontalUValues = image.Collect8bppPixelValues(horizontalU);
            byte[] horizontalDValues = image.Collect8bppPixelValues(horizontalD);
            byte[] verticalValues = image.Collect8bppPixelValues(vertical);
            byte[] verticalLValues = image.Collect8bppPixelValues(verticalL);
            byte[] verticalRValues = image.Collect8bppPixelValues(verticalR);

            Assert.AreEqual(horizontal.Count * 3, horizontalValues.Length);
            Assert.AreEqual(vertical.Count * 3, verticalValues.Length);

            // check all pixel values
            for (int i = 0, n = horizontalValues.Length; i < n; i += 3)
            {
                Assert.AreEqual(128, horizontalValues[i]);
                Assert.AreEqual(129, horizontalValues[i + 1]);
                Assert.AreEqual(130, horizontalValues[i + 2]);

                Assert.AreEqual(0, horizontalUValues[i]);
                Assert.AreEqual(0, horizontalUValues[i + 1]);
                Assert.AreEqual(0, horizontalUValues[i + 2]);

                Assert.AreEqual(0, horizontalDValues[i]);
                Assert.AreEqual(0, horizontalDValues[i + 1]);
                Assert.AreEqual(0, horizontalDValues[i + 2]);
            }

            for (int i = 0, n = verticalValues.Length; i < n; i += 3)
            {
                Assert.AreEqual(64, verticalValues[i]);
                Assert.AreEqual(65, verticalValues[i + 1]);
                Assert.AreEqual(66, verticalValues[i + 2]);

                Assert.AreEqual(0, verticalLValues[i]);
                Assert.AreEqual(0, verticalLValues[i + 1]);
                Assert.AreEqual(0, verticalLValues[i + 2]);

                Assert.AreEqual(0, verticalRValues[i]);
                Assert.AreEqual(0, verticalRValues[i + 1]);
                Assert.AreEqual(0, verticalRValues[i + 2]);
            }
        }

        [Test]
        public void CollectActivePixelsTest()
        {
            // create grayscale image
            UnmanagedImage image24 = UnmanagedImage.Create(7, 7, PixelFormat.Format24bppRgb);
            UnmanagedImage image8 = UnmanagedImage.Create(7, 7, PixelFormat.Format8bppIndexed);

            Drawing.FillRectangle(image24, new Rectangle(1, 1, 5, 5), Color.FromArgb(255, 255, 255));
            Drawing.FillRectangle(image24, new Rectangle(2, 2, 3, 3), Color.FromArgb(1, 1, 1));
            Drawing.FillRectangle(image24, new Rectangle(3, 3, 1, 1), Color.FromArgb(0, 0, 0));

            Drawing.FillRectangle(image8, new Rectangle(1, 1, 5, 5), Color.FromArgb(255, 255, 255));
            Drawing.FillRectangle(image8, new Rectangle(2, 2, 3, 3), Color.FromArgb(1, 1, 1));
            Drawing.FillRectangle(image8, new Rectangle(3, 3, 1, 1), Color.FromArgb(0, 0, 0));

            List<IntPoint> pixels24 = image24.CollectActivePixels();
            List<IntPoint> pixels8 = image8.CollectActivePixels();

            Assert.AreEqual(pixels24.Count, 24);
            Assert.AreEqual(pixels8.Count, 24);

            for (int i = 1; i < 6; i++)
            {
                for (int j = 1; j < 6; j++)
                {
                    if ((i == 3) && (j == 3))
                        continue;

                    Assert.IsTrue(pixels24.Contains(new IntPoint(j, i)));
                    Assert.IsTrue(pixels8.Contains(new IntPoint(j, i)));
                }
            }

            pixels24 = image24.CollectActivePixels(new Rectangle(1, 0, 5, 4));
            pixels8 = image8.CollectActivePixels(new Rectangle(1, 0, 5, 4));

            Assert.AreEqual(pixels24.Count, 14);
            Assert.AreEqual(pixels8.Count, 14);

            for (int i = 1; i < 4; i++)
            {
                for (int j = 1; j < 6; j++)
                {
                    if ((i == 3) && (j == 3))
                        continue;

                    Assert.IsTrue(pixels24.Contains(new IntPoint(j, i)));
                    Assert.IsTrue(pixels8.Contains(new IntPoint(j, i)));
                }
            }
        }

        [TestCase(PixelFormat.Format8bppIndexed)]
        [TestCase(PixelFormat.Format24bppRgb)]
        [TestCase(PixelFormat.Format32bppArgb)]
        [TestCase(PixelFormat.Format32bppRgb)]
        [TestCase(PixelFormat.Format16bppGrayScale)]
        [TestCase(PixelFormat.Format48bppRgb)]
        [TestCase(PixelFormat.Format64bppArgb)]
        public void SetPixelTest(PixelFormat pixelFormat)
        {
            UnmanagedImage image = UnmanagedImage.Create(320, 240, pixelFormat);
            Color color = Color.White;
            byte value = 255;

            image.SetPixel(0, 0, color);
            image.SetPixel(319, 0, color);
            image.SetPixel(0, 239, color);
            image.SetPixel(319, 239, value);
            image.SetPixel(160, 120, value);

            image.SetPixel(-1, -1, color);
            image.SetPixel(320, 0, color);
            image.SetPixel(0, 240, value);
            image.SetPixel(320, 240, value);

            List<IntPoint> pixels = image.CollectActivePixels();

            Assert.AreEqual(5, pixels.Count);

            Assert.IsTrue(pixels.Contains(new IntPoint(0, 0)));
            Assert.IsTrue(pixels.Contains(new IntPoint(319, 0)));
            Assert.IsTrue(pixels.Contains(new IntPoint(0, 239)));
            Assert.IsTrue(pixels.Contains(new IntPoint(319, 239)));
            Assert.IsTrue(pixels.Contains(new IntPoint(160, 120)));
        }

        public void SetPixelTestUnsupported(PixelFormat pixelFormat)
        {
            UnmanagedImage image = UnmanagedImage.Create(320, 240, pixelFormat);
            Color color = Color.White;
            byte value = 255;

            image.SetPixel(0, 0, color);
            image.SetPixel(319, 0, color);
            image.SetPixel(0, 239, color);
            image.SetPixel(319, 239, value);
            image.SetPixel(160, 120, value);

            image.SetPixel(-1, -1, color);
            image.SetPixel(320, 0, color);
            image.SetPixel(0, 240, value);
            image.SetPixel(320, 240, value);

            Assert.Throws<UnsupportedImageFormatException>(() => image.CollectActivePixels(), "");
        }

        [Test]
        public void SetGetPixelGrayscale()
        {
            UnmanagedImage image = UnmanagedImage.Create(320, 240, PixelFormat.Format8bppIndexed);

            image.SetPixel(0, 0, 255);
            image.SetPixel(319, 0, 127);
            image.SetPixel(0, 239, Color.FromArgb(64, 64, 64));

            Color color1 = image.GetPixel(0, 0);
            Color color2 = image.GetPixel(319, 0);
            Color color3 = image.GetPixel(0, 239);

            Assert.AreEqual(255, color1.R);
            Assert.AreEqual(255, color1.G);
            Assert.AreEqual(255, color1.B);

            Assert.AreEqual(127, color2.R);
            Assert.AreEqual(127, color2.G);
            Assert.AreEqual(127, color2.B);

            Assert.AreEqual(64, color3.R);
            Assert.AreEqual(64, color3.G);
            Assert.AreEqual(64, color3.B);
        }

        [TestCase(PixelFormat.Format24bppRgb)]
        [TestCase(PixelFormat.Format32bppArgb)]
        [TestCase(PixelFormat.Format32bppRgb)]
        public void SetGetPixelColor(PixelFormat pixelFormat)
        {
            UnmanagedImage image = UnmanagedImage.Create(320, 240, pixelFormat);

            image.SetPixel(0, 0, Color.FromArgb(255, 10, 20, 30));
            image.SetPixel(319, 0, Color.FromArgb(127, 110, 120, 130));
            image.SetPixel(0, 239, Color.FromArgb(64, 210, 220, 230));

            Color color1 = image.GetPixel(0, 0);
            Color color2 = image.GetPixel(319, 0);
            Color color3 = image.GetPixel(0, 239);

            Assert.AreEqual(10, color1.R);
            Assert.AreEqual(20, color1.G);
            Assert.AreEqual(30, color1.B);

            Assert.AreEqual(110, color2.R);
            Assert.AreEqual(120, color2.G);
            Assert.AreEqual(130, color2.B);

            Assert.AreEqual(210, color3.R);
            Assert.AreEqual(220, color3.G);
            Assert.AreEqual(230, color3.B);

            if (pixelFormat == PixelFormat.Format32bppArgb)
            {
                Assert.AreEqual(255, color1.A);
                Assert.AreEqual(127, color2.A);
                Assert.AreEqual(64, color3.A);
            }
        }

        [TestCase(PixelFormat.Format8bppIndexed)]
        [TestCase(PixelFormat.Format24bppRgb)]
        [TestCase(PixelFormat.Format32bppArgb)]
        [TestCase(PixelFormat.Format32bppRgb)]
        [TestCase(PixelFormat.Format16bppGrayScale)]
        [TestCase(PixelFormat.Format48bppRgb)]
        [TestCase(PixelFormat.Format64bppArgb)]
        public void SetPixelsTest(PixelFormat pixelFormat)
        {
            UnmanagedImage image = UnmanagedImage.Create(320, 240, pixelFormat);
            Color color = Color.White;
            List<IntPoint> points = new List<IntPoint>();

            points.Add(new IntPoint(0, 0));
            points.Add(new IntPoint(319, 0));
            points.Add(new IntPoint(0, 239));
            points.Add(new IntPoint(319, 239));
            points.Add(new IntPoint(160, 120));

            points.Add(new IntPoint(-1, -1));
            points.Add(new IntPoint(320, 0));
            points.Add(new IntPoint(0, 240));
            points.Add(new IntPoint(320, 240));

            image.SetPixels(points, color);

            List<IntPoint> pixels = image.CollectActivePixels();

            Assert.AreEqual(5, pixels.Count);

            Assert.IsTrue(pixels.Contains(new IntPoint(0, 0)));
            Assert.IsTrue(pixels.Contains(new IntPoint(319, 0)));
            Assert.IsTrue(pixels.Contains(new IntPoint(0, 239)));
            Assert.IsTrue(pixels.Contains(new IntPoint(319, 239)));
            Assert.IsTrue(pixels.Contains(new IntPoint(160, 120)));
        }

        [TestCase(PixelFormat.Format32bppPArgb)]
        public void SetPixelsTestUnsupported(PixelFormat pixelFormat)
        {
            UnmanagedImage image = UnmanagedImage.Create(320, 240, pixelFormat);
            Color color = Color.White;
            List<IntPoint> points = new List<IntPoint>();

            points.Add(new IntPoint(0, 0));
            points.Add(new IntPoint(319, 0));
            points.Add(new IntPoint(0, 239));
            points.Add(new IntPoint(319, 239));
            points.Add(new IntPoint(160, 120));

            points.Add(new IntPoint(-1, -1));
            points.Add(new IntPoint(320, 0));
            points.Add(new IntPoint(0, 240));
            points.Add(new IntPoint(320, 240));

            Assert.Throws<UnsupportedImageFormatException>(() => image.SetPixels(points, color),
                "The pixel format is not supported: Format32bppPArgb");
        }

        [TestCase(PixelFormat.Format24bppRgb, 1, 1, 240, 0, 0)]
        [TestCase(PixelFormat.Format24bppRgb, 318, 1, 0, 240, 0)]
        [TestCase(PixelFormat.Format24bppRgb, 318, 238, 240, 240, 0)]
        [TestCase(PixelFormat.Format24bppRgb, 1, 238, 0, 0, 240)]
        [TestCase(PixelFormat.Format24bppRgb, 160, 120, 240, 240, 240)]

        [TestCase(PixelFormat.Format32bppArgb, 1, 1, 240, 0, 0)]
        [TestCase(PixelFormat.Format32bppArgb, 318, 1, 0, 240, 0)]
        [TestCase(PixelFormat.Format32bppArgb, 318, 238, 240, 240, 0)]
        [TestCase(PixelFormat.Format32bppArgb, 1, 238, 0, 0, 240)]
        [TestCase(PixelFormat.Format32bppArgb, 160, 120, 240, 240, 240)]

        [TestCase(PixelFormat.Format32bppRgb, 1, 1, 240, 0, 0)]
        [TestCase(PixelFormat.Format32bppRgb, 318, 1, 0, 240, 0)]
        [TestCase(PixelFormat.Format32bppRgb, 318, 238, 240, 240, 0)]
        [TestCase(PixelFormat.Format32bppRgb, 1, 238, 0, 0, 240)]
        [TestCase(PixelFormat.Format32bppRgb, 160, 120, 240, 240, 240)]

        [TestCase(PixelFormat.Format8bppIndexed, 1, 1, 128, 128, 128)]
        [TestCase(PixelFormat.Format8bppIndexed, 318, 1, 96, 96, 96)]
        [TestCase(PixelFormat.Format8bppIndexed, 318, 238, 192, 192, 192)]
        [TestCase(PixelFormat.Format8bppIndexed, 1, 238, 32, 32, 32)]
        [TestCase(PixelFormat.Format8bppIndexed, 160, 120, 255, 255, 255)]

        public void ToManagedImageTest(PixelFormat pixelFormat, int x, int y, byte red, byte green, byte blue)
        {
            UnmanagedImage image = UnmanagedImage.Create(320, 240, pixelFormat);

            image.SetPixel(new IntPoint(x, y), Color.FromArgb(255, red, green, blue));

            Bitmap bitmap = image.ToManagedImage();

            // check colors of pixels
            Assert.AreEqual(Color.FromArgb(255, red, green, blue), bitmap.GetPixel(x, y));

            // make sure there are only 1 pixel
            UnmanagedImage temp = UnmanagedImage.FromManagedImage(bitmap);

            List<IntPoint> pixels = temp.CollectActivePixels();
            Assert.AreEqual(1, pixels.Count);

            image.Dispose();
            bitmap.Dispose();
            temp.Dispose();
        }

        [TestCase(PixelFormat.Format8bppIndexed, 3, 3, 9)]
        [TestCase(PixelFormat.Format8bppIndexed, 4, 5, 20)]
        [TestCase(PixelFormat.Format8bppIndexed, 5, 4, 20)]
        [TestCase(PixelFormat.Format8bppIndexed, 17, 13, 221)]
        public void ToByteArray_test8pp(PixelFormat pixelFormat, int w, int h, int expected)
        {
            int[,] values = Vector.Range(0, 255).Get(0, h * w).Reshape(h, w);
            UnmanagedImage image = values.ToBitmap().ToUnmanagedImage();

            int formatBytes = pixelFormat.GetPixelFormatSizeInBytes();
            byte[] b = image.ToByteArray();

            Assert.AreEqual(w * h * formatBytes, b.Length);
            Assert.AreEqual(expected, b.Length);

            // Reconstruct the original matrix
            UnmanagedImage r = UnmanagedImage.FromByteArray(b, w, h, pixelFormat);
            byte[,] actual = r.ToManagedImage().ToMatrix(0, 0, 255);

            Assert.AreEqual(values, actual);
        }

        [TestCase(PixelFormat.Format8bppIndexed, 3, 3, 9)]
        [TestCase(PixelFormat.Format8bppIndexed, 4, 5, 20)]
        [TestCase(PixelFormat.Format8bppIndexed, 5, 4, 20)]
        [TestCase(PixelFormat.Format8bppIndexed, 17, 13, 221)]
        [TestCase(PixelFormat.Format32bppArgb, 3, 3, 4 * 9)]
        [TestCase(PixelFormat.Format32bppArgb, 4, 5, 4 * 20)]
        [TestCase(PixelFormat.Format32bppArgb, 5, 4, 4 * 20)]
        [TestCase(PixelFormat.Format24bppRgb, 3, 3, 3 * 9)]
        [TestCase(PixelFormat.Format24bppRgb, 4, 5, 3 * 20)]
        [TestCase(PixelFormat.Format24bppRgb, 5, 4, 3 * 20)]
        public void ToByteArray_test_general(PixelFormat pixelFormat, int w, int h, int expected)
        {
            int c = pixelFormat.GetNumberOfChannels();
            byte[,,] values = (byte[,,])Vector.Range((byte)0, (byte)255).Get(0, h * w).Reshape(new[] { h, w, c });
            UnmanagedImage image = values.ToBitmap().ToUnmanagedImage();

            int formatBytes = pixelFormat.GetPixelFormatSizeInBytes();
            byte[] b = image.ToByteArray();

            Assert.AreEqual(w * h * formatBytes, b.Length);
            Assert.AreEqual(expected, b.Length);

            // Reconstruct the original matrix
            UnmanagedImage r = UnmanagedImage.FromByteArray(b, w, h, pixelFormat);
            byte[,,] actual = r.ToManagedImage().ToMatrix((byte)0, (byte)255);

            Assert.AreEqual(values, actual);
        }
    }
}
