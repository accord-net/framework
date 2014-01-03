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
    using AForge;
    using AForge.Imaging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using AForge.Imaging.Filters;
    using Accord.Controls.Imaging;
    using System.Windows.Forms;
    using System.Drawing.Drawing2D;

    [TestClass()]
    public class MatrixToImageTest
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
        public void MatrixToImageConstructorTest()
        {
            MatrixToImage target = new MatrixToImage();
            Assert.AreEqual(0, target.Min);
            Assert.AreEqual(1, target.Max);
        }

        [TestMethod()]
        public void MatrixToImageConstructorTest1()
        {
            double min = -100;
            double max = +100;
            MatrixToImage target = new MatrixToImage(min, max);
            Assert.AreEqual(min, target.Min);
        }

        [TestMethod()]
        public void ConvertTest()
        {
            MatrixToImage target = new MatrixToImage(min: 0, max: 128);

            byte[,] input = 
            {
                {   0,   0,   0 },
                {   0, 128,   0 },
                {   0,   0, 128 },
            };

            UnmanagedImage bitmap;

            target.Convert(input, out bitmap);

            var pixels = bitmap.CollectActivePixels();

            Assert.AreEqual(2, pixels.Count);
            Assert.IsTrue(pixels.Contains(new IntPoint(1, 1)));
            Assert.IsTrue(pixels.Contains(new IntPoint(2, 2)));
        }



        [TestMethod()]
        public void ConvertTest1()
        {
            MatrixToImage target = new MatrixToImage();

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
                 { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 11
                 { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 12
                 { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 }, // 13
                 { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 14
                 { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 15
            };

            Bitmap imageActual;
            target.Convert(pixels, out imageActual);


            double[,] actual;
            ImageToMatrix c = new ImageToMatrix();
            c.Convert(imageActual, out actual);

            double[,] expected;
            Bitmap imageExpected = Properties.Resources.image1;
            new Threshold().ApplyInPlace(imageExpected);
            new Invert().ApplyInPlace(imageExpected);
            c.Convert(imageExpected, out expected);


            for (int i = 0; i < pixels.GetLength(0); i++)
                for (int j = 0; j < pixels.GetLength(1); j++)
                    Assert.AreEqual(actual[i, j], expected[i, j]);
        }

        [TestMethod()]
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
            MatrixToImage conv = new MatrixToImage(min: 0, max: 1);

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

    }
}
