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
    using Accord.Imaging.Formats;
    using Accord.Math;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;

    [TestFixture]
    public class PNMCodecTest
    {

        [Test]
        public void doc_example()
        {
            string basePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "PNM");

            #region doc_load
            string filename = Path.Combine(basePath, "lena.pgm");

            // One way to decode the image explicitly is to use
            Bitmap image1 = ImageDecoder.DecodeFromFile(filename);

            // However, you could also obtain the same effect using
            Bitmap image2 = Accord.Imaging.Image.FromFile(filename);
            #endregion

            var i2m = new ImageToMatrix();

            byte[,] actualMatrix;
            i2m.Convert(image1, out actualMatrix);

            byte[,] expectedMatrix;
            i2m.Convert(image2, out expectedMatrix);

            Assert.IsTrue(expectedMatrix.IsEqual(actualMatrix, (byte)1));
        }

        [Test]
        public void decodeP2_1()
        {
            test("lena.ascii.pgm", "lena.png");
        }

        [Test]
        public void decodeP2_2()
        {
            test("baboon.ascii.pgm", "baboon.png");
        }

        [Test]
        public void decodeP5_2()
        {
            test("baboon.pgm", "baboon.png");
        }

        [Test]
        public void decodeP5_3()
        {
            test("pepper.pgm", "pepper.png");
        }

        [Test]
        public void decodeP6_1()
        {
            test("newton.ppm", "newton.png");
        }

        private static void test(string actual, string expected)
        {
            string basePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "PNM");

            string fileName = Path.Combine(basePath, actual);
            Bitmap actualImage = ImageDecoder.DecodeFromFile(fileName);

            string expectedFileName = Path.Combine(basePath, expected);
            Bitmap expectedImage = (Bitmap)Bitmap.FromFile(expectedFileName);

            var i2m = new ImageToMatrix();

            byte[,] actualMatrix;
            i2m.Convert(actualImage, out actualMatrix);

            byte[,] expectedMatrix;
            i2m.Convert(expectedImage, out expectedMatrix);

            Assert.IsTrue(expectedMatrix.IsEqual(actualMatrix, (byte)1));
        }
    }
}
