﻿// Accord Unit Tests
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
    using Accord.Imaging;
    using Accord.Math;
    using NUnit.Framework;
    using Accord.Imaging.Converters;
    using System.Drawing.Imaging;
    using Accord.DataSets;

    [TestFixture]
    public class IntegralImage2Test
    {

#pragma warning disable 0618

        [Test]
        public void lena_test()
        {
            string localPath = NUnit.Framework.TestContext.CurrentContext.TestDirectory;

            #region doc_lena
            // In this example, we will compute an integral image
            // representation of Lena Söderberg's famous picture:
            TestImages testImages = new TestImages(path: localPath);
            Bitmap lena = testImages["lena.bmp"]; // get the image

            // Create a new Integral Image (squared and tilted) from Lena's picture:
            IntegralImage2 ii = IntegralImage2.FromBitmap(lena, computeTilted: true);

            // Let's say we would like to get the summed area in the rectangular region
            // delimited by pixel (34, 50) until pixels (60, 105). This is equivalent to
            // the region under the rectangle (34, 50, 34+60, 50+105) = (34, 50, 94, 155):
            long sum = ii.GetSum(34, 50, 94, 155); // this is the sum of values (1760032)

            // Now let's say we would like to get the squared sum and tilted sum as well:
            long ssum = ii.GetSum2(34, 50, 94, 155); // this is the sum of squared values (229508896)
            long tsum = ii.GetSumT(34, 50, 94, 155); // this is the sum of tilted values (-593600)
            #endregion

            Assert.AreEqual(1760032, sum);
            Assert.AreEqual(229508896, ssum);
            Assert.AreEqual(-593600, tsum);
        }

        [Test]
        public void GetSumTest()
        {
            byte[,] img =
            {
                { 01, 02, 03, 04, 05, 06, 07, 08, 09,  10 },
                { 11, 12, 13, 14, 15, 16, 17, 18, 19,  20 },
                { 21, 22, 23, 24, 25, 26, 27, 28, 29,  30 },
                { 31, 32, 33, 34, 35, 36, 37, 38, 39,  40 },
                { 41, 42, 43, 44, 45, 46, 47, 48, 49,  50 },
                { 51, 52, 53, 54, 55, 56, 57, 58, 59,  60 },
                { 61, 62, 63, 64, 65, 66, 67, 68, 69,  70 },
                { 71, 72, 73, 74, 75, 76, 77, 78, 79,  80 },
                { 81, 82, 83, 84, 85, 86, 87, 88, 89,  90 },
                { 91, 92, 93, 94, 95, 96, 97, 98, 99, 100 }
            };

            // Create integral image
            Bitmap bmp; new MatrixToImage().Convert(img, out bmp);
            IntegralImage2 ii = IntegralImage2.FromBitmap(bmp, 0, true);


            // Horizontal rectangular feature
            int x = 6, y = 0, w = 3, h = 2;

            int expected = 7 + 8 + 9 + 17 + 18 + 19;
            long actual = ii.GetSum(x, y, w, h);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetSumTest3()
        {
            // Example from Rainer Lienhart and Jochen Maydt:
            // "An Extended Set of Haar-like Features for Rapid Object Detection"

            int x = 6, y = 2, h = 4, w = 6;

            byte[,] img =
            { //  0 1 2 3 4 5 6 7 8 9 A B C D E 
          /*0*/ { 9,9,9,9,9,9,9,9,9,9,9,9,9,9,9 },
          /*1*/ { 9,9,9,9,9,9,9,9,9,9,9,9,9,9,9 },
          /*2*/ { 9,9,9,9,9,1,1,9,9,9,9,9,9,9,9 },
          /*3*/ { 9,9,9,9,1,1,1,1,9,9,9,9,9,9,9 },
          /*4*/ { 9,9,9,1,1,1,1,1,1,9,9,9,9,9,9 },
          /*5*/ { 9,9,1,1,1,1,1,1,1,1,9,9,9,9,9 },
          /*6*/ { 9,9,9,1,1,1,1,1,1,1,1,9,9,9,9 },
          /*7*/ { 9,9,9,9,1,1,1,1,1,1,1,1,9,9,9 },
          /*8*/ { 9,9,9,9,9,1,1,1,1,1,1,9,9,9,9 },
          /*9*/ { 9,9,9,9,9,9,1,1,1,1,9,9,9,9,9 },
          /*A*/ { 9,9,9,9,9,9,9,1,1,9,9,9,9,9,9 },
          /*B*/ { 9,9,9,9,9,9,9,9,9,9,9,9,9,9,9 },
          /*C*/ { 9,9,9,9,9,9,9,9,9,9,9,9,9,9,9 },
            };

            // -RSAT(x-1,y-1)         = [6-1,2-1]         = [ 5, 1]  => [ 1, 5]
            // +RSAT(x+w-1,y+w-1)     = [6+6-1,2+6-1]     = [11, 7]  => [ 7,11]
            // -RSAT(x+w-1-h,y+w-1+h) = [6+6-1-4,2+6-1+4] = [ 7,11]  => [11, 7]
            // +RSAT(x-h-1, y+h-1)    = [6-4-1,2+4-1]     = [ 1, 5]  => [ 5, 1]

            // int sum = -iit[5,1] + iit[11,7] - iit[7,11] + iit[1,5];

            // Create integral image
            Bitmap bmp = Accord.Imaging.Tools.ToBitmap(img);
            IntegralImage2 ii = IntegralImage2.FromBitmap(bmp, 0, true);


            // Tilted rectangular feature
            long[,] iit = tiltedIntegral3(img);

            long expected = 48;

            long sum = -(-iit[5 + (1), 1 + (2)] + iit[11 + (1), 7 + (2)]
                - iit[7 + (1), 11 + (2)] + iit[1 + (1), 5 + (2)]);

            long a = iit[y - 1 + (1), x - 1 + (2)];
            long b = iit[y + w - 1 + (1), x + w - 1 + (2)];
            long c = iit[y + w - 1 + h + (1), x + w - 1 - h + (2)];
            long d = iit[y + h - 1 + (1), x - h - 1 + (2)];

            long manual = -a + b - c + d;

            Assert.AreEqual(expected, sum);
            Assert.AreEqual(expected, manual);

            long actual = ii.GetSumT(x, y, w, h);
            Assert.AreEqual(expected, actual);

        }

        [Test]
        public void GetSumTest2()
        {
            #region doc_sum
            // Let's say we have the following image representation:
            byte[,] img =
            {
                { 5, 2, 3, 4, 1 },
                { 1, 5, 4, 2, 3 },
                { 2, 2, 1, 3, 4 },
                { 3, 5, 6, 4, 5 },
                { 4, 1, 3, 2, 6 },
            };

            // Let's convert it to bitmap and 
            // pretend it is our input image:
            Bitmap bmp = img.ToBitmap();

            // Now, create an integral image from this bitmap:
            IntegralImage2 ii = IntegralImage2.FromBitmap(bmp);

            // The sum-table would be:
            long[,] actual = ii.Image;

            // Which would be the same as:
            long[,] expected =
            {
                { 0,  0,  0,  0,  0,  0 },
                { 0,  5,  7, 10, 14, 15 },
                { 0,  6, 13, 20, 26, 30 },
                { 0,  8, 17, 25, 34, 42 },
                { 0, 11, 25, 39, 52, 65 },
                { 0, 15, 30, 47, 62, 81 }
            };
            #endregion


            Assert.IsTrue(Matrix.IsEqual(expected, actual));

        }

        [Test]
        public void GetSumTest4()
        {
            byte[,] img =
            {
                { 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1 },
            };

            Bitmap bmp = Accord.Imaging.Tools.ToBitmap(img);
            IntegralImage2 ii = IntegralImage2.FromBitmap(bmp, 0, true);

            // http://software.intel.com/sites/products/documentation/hpc/ipp/ippi/ippi_ch11/functn_TiltedIntegral.html
            long[,] expected = tiltedIntegral3(img);

            long[,] iit =
            {
                {  0,  0,  0,  0,  0,  0,  0 },
                {  0,  0,  0,  0,  0,  0,  0 },
                {  1,  3,  5,  6,  6,  5,  3 },
                {  3,  6,  9, 11, 11,  9,  6 },
                {  6, 10, 14, 16, 16, 14, 10 },
                { 10, 15, 19, 21, 21, 19, 15 },
                { 15, 19, 22, 24, 24, 22, 19 },
            };

            long[,] iit2 =
            {
                {  1,  3,  6, 10, 15,  19,  0 },
                {  1,  4,  8, 13, 18,  22,  0 },
                {  1,  4,  9, 14, 19,  23,  0 },
                {  1,  4,  8, 13, 18,  22,  0 },
                {  1,  3,  6, 10, 15,  19,  0 },
                {  0,  1,  3,  6, 10,  15,  0 },
            };

            long[,] iit3 =
            {
                {  0, 0,  0,  1,  3,  6,  10,  15  },
                {  0, 0,  1,  3,  6, 10,  15,  21  },
                {  0, 0,  1,  4,  8, 13,  19,  25  },
                {  0, 0,  1,  4,  9, 15,  21,  27  },
                {  0, 0,  1,  4,  9, 15,  21,  27  },
                {  0, 0,  1,  4,  8, 13,  19,  24  },
                {  0, 0,  1,  3,  6, 10,  14,  18  },
                {  0, 0,  0,  1,  3,  5,   7,   9  }
            };

            Assert.IsTrue(Matrix.IsEqual(iit3, expected));


            long[,] actual = ii.Rotated;

            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void ImageFormatsTest()
        {
            byte[,] img =
            {
                { 5, 2, 3, 4, 1 },
                { 1, 5, 4, 2, 3 },
                { 2, 2, 1, 3, 4 },
                { 3, 5, 6, 4, 5 },
                { 4, 1, 3, 2, 6 },
            };



            long[,] actual8bpp = create(img, PixelFormat.Format8bppIndexed);
            long[,] actual24rgb = create(img, PixelFormat.Format24bppRgb);
            long[,] actual32rgb = create(img, PixelFormat.Format32bppRgb);
            long[,] actual32argb = create(img, PixelFormat.Format32bppArgb);

            Assert.IsTrue(actual8bpp.IsEqual(actual24rgb));
            Assert.IsTrue(actual8bpp.IsEqual(actual32rgb));
            Assert.IsTrue(actual8bpp.IsEqual(actual32argb));
        }

        private static long[,] create(byte[,] img, PixelFormat format)
        {
            long[,] actual8bpp;
            Bitmap image;
            MatrixToImage converter = new MatrixToImage();
            converter.Format = format;
            converter.Convert(img, out image);

            Assert.AreEqual(format, image.PixelFormat);

            IntegralImage2 ii8bpp = IntegralImage2.FromBitmap(image, 0);
            actual8bpp = ii8bpp.Image;
            return actual8bpp;
        }

        private static int[,] tiltedIntegral(byte[,] img)
        {
            int val = 0;
            int[,] expected = new int[img.GetLength(0) + 2, img.GetLength(0) + 2];
            for (int i = 2; i < img.GetLength(0) + 2; i++)
            {
                for (int j = 0; j < img.GetLength(1) + 2; j++)
                {
                    int sum = val;

                    for (int k = 0; k < img.GetLength(0); k++)
                    {
                        for (int l = 0; l < img.GetLength(1); l++)
                        {
                            if (k - l <= i - j + 1 &&
                                k + l <= i + j - 2)
                                sum += img[k, l];
                        }
                    }

                    expected[i, j] = sum;
                }
            }
            return expected;
        }

        private static int[,] tiltedIntegral2(byte[,] img)
        {
            int val = 0;
            int[,] expected = new int[img.GetLength(0) + 1, img.GetLength(0) + 1];
            for (int x = 0; x < img.GetLength(0) + 1; x++)
            {
                for (int y = 0; y < img.GetLength(1) + 1; y++)
                {
                    int sum = val;

                    for (int xl = 0; xl < img.GetLength(0); xl++)
                    {
                        for (int yl = 0; yl < img.GetLength(1); yl++)
                        {
                            if (xl <= x &&
                                xl <= x - System.Math.Abs(y - yl))
                                sum += img[yl, xl];
                        }
                    }

                    expected[y, x] = sum;
                }
            }
            return expected;
        }

        private static long[,] tiltedIntegral3(byte[,] img)
        {
            int rows = img.GetLength(0);
            int cols = img.GetLength(1);

            int val = 0;
            var expected = new long[rows + 2, cols + 2];
            for (int y = 1; y < rows + 2; y++)
            {
                for (int x = 2; x < cols + 2; x++)
                {
                    long r = val;
                    // if (y - 1 >= 0 && x - 1 >= 0)
                    r += expected[y - 1, x - 1];
                    // if (y >= 0 && x - 1 >= 0)
                    r += expected[y, x - 1];
                    if (y - 1 >= 0 && x - 1 >= 0 && y - 1 < rows && x - 2 < cols)
                        r += img[y - 1, x - 2];
                    //if (y - 1 >= 0 && x - 2 >= 0)
                    r -= expected[y - 1, x - 2];
                    expected[y, x] = r;
                }
            }

            for (int y = rows + 1; y >= 0; y--)
            {
                for (int x = cols + 1; x >= 0; x--)
                {
                    long r = expected[y, x];
                    if (y + 1 < rows + 2 && x - 1 >= 0)
                        r += expected[y + 1, x - 1];
                    if (y >= 0 && x - 2 >= 0)
                        r -= expected[y, x - 2];
                    expected[y, x] = r;
                }
            }

            return expected;
        }

#pragma warning restore 0618

    }
}
