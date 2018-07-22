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
    using Accord.Imaging.Filters;
    using NUnit.Framework;
    using Accord.Tests.Imaging.Properties;
    using System.Windows.Media.Imaging;
    using Accord.Imaging;
#if NO_BITMAP
    using Resources = Accord.Tests.Imaging.Properties.Resources_Standard;
#endif

    using Color = System.Drawing.Color;
    using System.Windows.Media;

    [TestFixture]
    public class ArrayToBitmapSourceTest
    {

        [Test]
        public void ConvertTest1()
        {

            var target = new ArrayToBitmapSource(16, 16);

            double[] pixels = 
            {
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 1
                 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, // 2 
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 3
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 4
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 5
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 6
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 7
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 8
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 9
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 10
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 11
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 12
                 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, // 13
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 14
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 15
            };

            BitmapSource imageActual;
            target.Convert(pixels, out imageActual);


            double[] actual;
            var c = new BitmapSourceToArray();
            c.Convert(imageActual, out actual);

            double[] expected;

            BitmapSource imageExpected = TestTools.GetImage("image1.bmp");
            Bitmap bitmapImageExpected = imageExpected.ToBitmap();
            new Invert().ApplyInPlace(bitmapImageExpected);
            new Threshold().ApplyInPlace(bitmapImageExpected);
            BitmapSource imageExpected2 = bitmapImageExpected.ToBitmapSource();

            c.Convert(imageExpected2, out expected);

            for (int i = 0; i < pixels.Length; i++)
                Assert.AreEqual(actual[i], expected[i]);
        }

        [Test]
        public void ConvertTest2()
        {
            // Create an array representation 
            // of a 4x4 image with a inner 2x2
            // square drawn in the middle

            double[] pixels = 
            {
                 0, 0, 0, 0, 
                 0, 1, 1, 0, 
                 0, 1, 1, 0, 
                 0, 0, 0, 0, 
            };

            // Create the converter to create a Bitmap from the array
            var conv = new ArrayToBitmapSource(width: 4, height: 4);

            // Declare a bitmap source and store the pixels on it
            BitmapSource image; conv.Convert(pixels, out image);

            double[,] expected =
            {
                 { 0, 0, 0, 0 },
                 { 0, 1, 1, 0 },
                 { 0, 1, 1, 0 },
                 { 0, 0, 0, 0 },
            };

            double[,] result = image.ToMatrix(0);

            Assert.AreEqual(expected, result);

            Assert.AreEqual(PixelFormats.Gray32Float, image.Format);
        }

        [Test]
        public void ConvertTest3()
        {
            // Create an array representation 
            // of a 4x4 image with a inner 2x2
            // square drawn in the middle

            byte[] pixels = 
            {
                 0,   0,   0,   0, 
                 0, 255, 255,   0, 
                 0, 255, 255,   0, 
                 0,   0,   0,   0, 
            };

            // Create the converter to create a Bitmap from the array
            var conv = new ArrayToBitmapSource(width: 4, height: 4);

            // Declare an image and store the pixels on it
            BitmapSource image; conv.Convert(pixels, out image);

            var conv2 = new ArrayToImage(width: 4, height: 4);
            Bitmap expected; conv2.Convert(pixels, out expected);

            Assert.AreEqual(expected.ToMatrix(0), image.ToMatrix(0));
        }

        [Test]
        public void ConvertTest4()
        {
            // Create an array representation 
            // of a 4x4 image with a inner 2x2
            // square drawn in the middle

            System.Windows.Media.Color[] pixels = 
            {
                 Colors.Red,   Colors.Lime,        Colors.Blue,   Colors.Black, 
                 Colors.Black, Colors.Transparent, Colors.Red,    Colors.Black, 
                 Colors.Black, Colors.Lime,        Colors.Blue,   Colors.Black, 
                 Colors.Black, Colors.Black,       Colors.Black,  Colors.Black, 
            };

            // Create the converter to create a Bitmap from the array
            var conv = new ArrayToBitmapSource(width: 4, height: 4);

            // Declare an image and store the pixels on it
            BitmapSource image; conv.Convert(pixels, out image);



            System.Drawing.Color[] pixels2 =
            {
                 Color.Red,   Color.Lime,        Color.Blue,  Color.Black,
                 Color.Black, Color.Transparent, Color.Red,    Color.Black,
                 Color.Black, Color.Lime,        Color.Blue,   Color.Black,
                 Color.Black, Color.Black,       Color.Black,  Color.Black,
            };

            ArrayToImage conv2 = new ArrayToImage(width: 4, height: 4);

            Bitmap image2; conv2.Convert(pixels2, out image2);

            var actual = image.ToMatrix();
            var expected = image2.ToMatrix();
            Assert.AreEqual(expected, actual);
        }

    }
}
