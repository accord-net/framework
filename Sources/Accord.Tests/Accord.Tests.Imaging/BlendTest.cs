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
    using System.Drawing.Imaging;
    using Accord.Imaging;
    using Accord.Imaging.Filters;
    using Microsoft.VisualStudio.TestTools.UnitTesting;


    [TestClass]
    public class BlendTest
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


        [TestMethod]
        public void ApplyTest()
        {
            var img1 = Properties.Resources.image2;
            var img2 = Properties.Resources.image2;

            MatrixH homography = new MatrixH(1, 0, 32,
                                             0, 1, 0,
                                             0, 0);

            Blend blend = new Blend(homography, img1);
            var actual = blend.Apply(img2);

            Assert.AreEqual(64, actual.Size.Width);
            Assert.AreEqual(32, actual.Size.Height);



            homography = new MatrixH(1, 0, 32,
                                     0, 1, 32,
                                     0, 0);

            blend = new Blend(homography, img1);
            actual = blend.Apply(img2);


            Assert.AreEqual(64, actual.Size.Width);
            Assert.AreEqual(64, actual.Size.Height);

            // ImageBox.Show(img3, PictureBoxSizeMode.Zoom);
        }

        [TestMethod]
        public void ApplyTest2()
        {
            var img1 = Properties.Resources.image2;
            var img2 = Properties.Resources.image2;

            var img3 = Properties.Resources.image2;
            var img4 = Properties.Resources.image2;


            MatrixH homography;
            Blend blend;

            homography = new MatrixH(1, 0, 32,
                                     0, 1, 0,
                                     0, 0);


            blend = new Blend(homography, img1);
            var img12 = blend.Apply(img2);

            //ImageBox.Show("Blend of 1 and 2", img12, PictureBoxSizeMode.Zoom);
            Assert.AreEqual(img12.PixelFormat, PixelFormat.Format32bppArgb);

            blend = new Blend(homography, img3);
            var img34 = blend.Apply(img4);

            //ImageBox.Show("Blend of 3 and 4", img34, PictureBoxSizeMode.Zoom);
            Assert.AreEqual(img34.PixelFormat, PixelFormat.Format32bppArgb);


            homography = new MatrixH(1, 0, 64,
                                     0, 1, 0,
                                     0, 0);


            blend = new Blend(homography, img12);
            var img1234 = blend.Apply(img34);

            //ImageBox.Show("Blend of 1, 2, 3, 4", img1234, PictureBoxSizeMode.Zoom);
            Assert.AreEqual(img1234.PixelFormat, PixelFormat.Format32bppArgb);



            // Blend of 1 and 5 (8bpp and 32bpp)
            homography = new MatrixH(1, 0, 0,
                                     0, 1, 32,
                                     0, 0);


            //ImageBox.Show("Image 1", img1, PictureBoxSizeMode.Zoom);
            blend = new Blend(homography, img1234);
            var img15 = blend.Apply(img1);
            //ImageBox.Show("Blend of 1 and 5", img15, PictureBoxSizeMode.Zoom);

            Assert.AreEqual(img1234.PixelFormat, PixelFormat.Format32bppArgb);
            Assert.AreEqual(img1.PixelFormat, PixelFormat.Format8bppIndexed);
            Assert.AreEqual(img15.PixelFormat, PixelFormat.Format32bppArgb);
            Assert.AreEqual(128, img15.Width);
            Assert.AreEqual(64, img15.Height);

        }

    }
}
