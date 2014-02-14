// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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
    using AForge.Imaging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using AForge.Imaging.Filters;
    using Accord.Controls.Imaging;
    using System.Windows.Forms;

    [TestClass()]
    public class ArrayToImageTest
    {

        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }



        [TestMethod()]
        public void ArrayToImageConstructorTest()
        {
            int height = 16;
            int width = 32;
            ArrayToImage target = new ArrayToImage(32, 16);
            Assert.AreEqual(0, target.Min);
            Assert.AreEqual(1, target.Max);
            Assert.AreEqual(height, target.Height);
            Assert.AreEqual(width, target.Width);
        }


        [TestMethod()]
        public void ConvertTest1()
        {

            ArrayToImage target = new ArrayToImage(16, 16);

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

            Bitmap imageActual;
            target.Convert(pixels, out imageActual);


            double[] actual;
            ImageToArray c = new ImageToArray();
            c.Convert(imageActual, out actual);

            double[] expected;

            Bitmap imageExpected = Properties.Resources.image1;
            new Invert().ApplyInPlace(imageExpected);
            new Threshold().ApplyInPlace(imageExpected);

            c.Convert(imageExpected, out expected);


            for (int i = 0; i < pixels.Length; i++)
                Assert.AreEqual(actual[i], expected[i]);
        }

        [TestMethod()]
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
            ArrayToImage conv = new ArrayToImage(width: 4, height: 4);

            // Declare an image and store the pixels on it
            Bitmap image; conv.Convert(pixels, out image);

            // Show the image on screen
            image = new ResizeNearestNeighbor(320, 320).Apply(image);
            // ImageBox.Show(image, PictureBoxSizeMode.Zoom);

            Assert.AreEqual(0, conv.Min);
            Assert.AreEqual(1, conv.Max);
            Assert.AreEqual(320, image.Height);
            Assert.AreEqual(320, image.Width);
        }

        [TestMethod()]
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
            ArrayToImage conv = new ArrayToImage(width: 4, height: 4);

            // Declare an image and store the pixels on it
            Bitmap image; conv.Convert(pixels, out image);

            // Show the image on screen
            image = new ResizeNearestNeighbor(320, 320).Apply(image);
            // Accord.Controls.ImageBox.Show(image, PictureBoxSizeMode.Zoom);

            Assert.AreEqual(0, conv.Min);
            Assert.AreEqual(1, conv.Max);
            Assert.AreEqual(320, image.Height);
            Assert.AreEqual(320, image.Width);
        }

        [TestMethod()]
        public void ConvertTest4()
        {
            // Create an array representation 
            // of a 4x4 image with a inner 2x2
            // square drawn in the middle

            Color[] pixels = 
            {
                 Color.Black, Color.Black,       Color.Black, Color.Black, 
                 Color.Black, Color.Transparent, Color.Red,   Color.Black, 
                 Color.Black, Color.Green,       Color.Blue,  Color.Black, 
                 Color.Black, Color.Black,       Color.Black,  Color.Black, 
            };

            // Create the converter to create a Bitmap from the array
            ArrayToImage conv = new ArrayToImage(width: 4, height: 4);

            // Declare an image and store the pixels on it
            Bitmap image; conv.Convert(pixels, out image);

            // Show the image on screen
            image = new ResizeNearestNeighbor(320, 320).Apply(image);
            // Accord.Controls.ImageBox.Show(image, PictureBoxSizeMode.Zoom);

            Assert.AreEqual(0, conv.Min);
            Assert.AreEqual(1, conv.Max);
            Assert.AreEqual(320, image.Height);
            Assert.AreEqual(320, image.Width);
        }

    }
}
