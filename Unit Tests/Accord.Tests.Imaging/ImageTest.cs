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

namespace Accord.Imaging.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge;
    using Accord.Imaging;
    using NUnit.Framework;
    using Accord.DataSets;
    using Accord.Tests.Imaging.Properties;
#if NO_BITMAP
    using Resources = Accord.Tests.Imaging.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class ImageTest
    {
        [Test]
        public void load_true_grayscale_test()
        {
            string localPath = TestContext.CurrentContext.TestDirectory;
            var images = new TestImages(path: localPath);
            Bitmap lena1 = images["lena.bmp"];
            Bitmap lena2 = Accord.Imaging.Image.Clone(Resources.lena512);

            Assert.AreEqual(lena1.Width, lena2.Width);
            Assert.AreEqual(lena1.Height, lena2.Height);

            Assert.IsTrue(lena1.IsGrayscale());
            Assert.IsTrue(lena2.IsGrayscale());

            int max1 = lena1.Max();
            int max2 = lena2.Max();

            int min1 = lena1.Min();
            int min2 = lena2.Min();

            Assert.AreEqual(244, max1);
            Assert.AreEqual(245, max2);
            Assert.AreEqual(28, min1);
            Assert.AreEqual(25, min2);
        }

        [Test]
        public void test_images_test()
        {
            string localPath = TestContext.CurrentContext.TestDirectory;
            var images = new TestImages(path: localPath);
            Bitmap lena = images["lena.bmp"];

            Assert.IsTrue(lena.IsGrayscale());
        }
    }
}
