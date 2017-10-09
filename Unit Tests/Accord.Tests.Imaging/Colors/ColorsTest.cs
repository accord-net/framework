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
    using Accord.Imaging;

    [TestFixture]
    public class ColorsTest
    {

        [Test]
        public void fromRGB()
        {
            #region doc_rgb
            // This example shows how to convert to and from various
            // pixel representations. For example, let's say we start
            // with a RGB pixel with the value (24, 250, 153):
            RGB rgb = new RGB(red: 24, green: 250, blue: 153);

            // The value of this pixel in HSL format would be:
            HSL hsl = HSL.FromRGB(rgb);       // should be (154, 0.9576271, 0.5372549)

            // The value of this pixel in YCbCr format would be:
            YCbCr yCbCr = YCbCr.FromRGB(rgb); // should be (0.671929836, -0.0406815559, -0.412097245)

            // Note: we can also convert using casting:
            HSL a = (HSL)rgb;
            YCbCr b = (YCbCr)rgb;
            #endregion

            Assert.AreEqual(24, rgb.Red);
            Assert.AreEqual(250, rgb.Green);
            Assert.AreEqual(153, rgb.Blue);
            Assert.AreEqual(255, rgb.Alpha);
            Assert.AreEqual(154, hsl.Hue);
            Assert.AreEqual(0.9576271, hsl.Saturation, 1e-6);
            Assert.AreEqual(0.5372549, hsl.Luminance, 1e-6);
            Assert.AreEqual(0.671929836, yCbCr.Y, 1e-6);
            Assert.AreEqual(-0.0406815559, yCbCr.Cb, 1e-6);
            Assert.AreEqual(-0.412097245, yCbCr.Cr, 1e-6);

            Assert.AreEqual(hsl, a);
            Assert.AreEqual(yCbCr, b);
        }

        [Test]
        public void fromHSL()
        {
            #region doc_hsl
            // This example shows how to convert to and from various
            // pixel representations. For example, let's say we start
            // with a HSL pixel with the value (123, 0.4, 0.8):
            HSL hsl = new HSL(hue: 123, saturation: 0.4f, luminance: 0.8f);

            // The value of this pixel in RGB format would be:
            RGB rgb = hsl.ToRGB();       // should be (183, 224, 185)

            // The value of this pixel in YCbCr format would be:
            YCbCr yCbCr = YCbCr.FromRGB(rgb); // should be (0.8128612, -0.0493462719, -0.0679121539)

            // Note: we can also convert using casting:
            RGB a = (RGB)hsl;
            YCbCr b = (YCbCr)hsl;
            #endregion

            Assert.AreEqual(183, rgb.Red);
            Assert.AreEqual(224, rgb.Green);
            Assert.AreEqual(185, rgb.Blue);
            Assert.AreEqual(255, rgb.Alpha);
            Assert.AreEqual(123, hsl.Hue);
            Assert.AreEqual(0.4, hsl.Saturation, 1e-6);
            Assert.AreEqual(0.8, hsl.Luminance, 1e-6);
            Assert.AreEqual(0.8128612, yCbCr.Y, 1e-6);
            Assert.AreEqual(-0.0493462719, yCbCr.Cb, 1e-6);
            Assert.AreEqual(-0.0679121539, yCbCr.Cr, 1e-6);

            Assert.AreEqual(rgb, a);
            Assert.AreEqual(yCbCr, b);
        }

        [Test]
        public void fromYCbCr()
        {
            #region doc_ycbcr
            // This example shows how to convert to and from various
            // pixel representations. For example, let's say we start
            // with a YCbCr pixel with the value (0.8, 0.2, 0.5):
            YCbCr yCbCr = new YCbCr(y: 0.8f, cb: 0.2f, cr: 0.5f);

            // The value of this pixel in RGB format would be:
            RGB rgb = yCbCr.ToRGB();  // should be (255, 95, 255)

            // The value of this pixel in HSL format would be:
            HSL hsl = HSL.FromRGB(rgb); // should be (299, 0.99999994, 0.6862745)

            // Note: we can also convert using casting:
            RGB a = (RGB)yCbCr;
            HSL b = (HSL)yCbCr;
            #endregion

            Assert.AreEqual(255, rgb.Red);
            Assert.AreEqual(95, rgb.Green);
            Assert.AreEqual(255, rgb.Blue);
            Assert.AreEqual(255, rgb.Alpha);
            Assert.AreEqual(299, hsl.Hue, 1);
            Assert.AreEqual(0.99999994, hsl.Saturation, 1e-6);
            Assert.AreEqual(0.6862745, hsl.Luminance, 1e-6);
            Assert.AreEqual(0.8, yCbCr.Y, 1e-6);
            Assert.AreEqual(0.2, yCbCr.Cb, 1e-6);
            Assert.AreEqual(0.5, yCbCr.Cr, 1e-6);

            Assert.AreEqual(rgb, a);
            Assert.AreEqual(hsl, b);
        }

    }
}
