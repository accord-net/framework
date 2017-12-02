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
    using System.Drawing.Imaging;
    using Accord.Imaging.Converters;
    using Accord.Math;
    using Accord.Imaging;
    using Accord.Imaging.Filters;
    using NUnit.Framework;
    using Accord.Tests.Imaging.Properties;
    using System.Windows.Media.Imaging;
#if NO_BITMAP
    using Resources = Accord.Tests.Imaging.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class BitmapSourceToArrayTest
    {

        [Test]
        public void ConvertTest3()
        {
            double[] pixels = 
            {
                 0, 0, 0, 0,
                 0, 1, 1, 0,
                 0, 1, 1, 0,
                 0, 0, 0, 0,
            };

            var conv1 = new ArrayToBitmapSource(width: 4, height: 4);
            BitmapSource image; conv1.Convert(pixels, out image);

            // Create the converter to convert the image to an
            //   array containing only values between 0 and 1 
            var conv = new BitmapSourceToArray();

            // Convert the image and store it in the array
            double[] array; conv.Convert(image, out array);

            Assert.AreEqual(0, array.Min());
            Assert.AreEqual(1, array.Max());
            Assert.AreEqual(16 * 16, array.Length);
        }

        [Test]
        public void ConvertTest4()
        {

            var target = new BitmapSourceToArray();
            BitmapSource image = TestTools.GetImage("image3.bmp");
            Assert.AreEqual(PixelFormat.Format32bppArgb.ToWPF(), image.Format);

            {
                double[] output;
                double[] outputExpected =
                {
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 0
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 1
                      0, 0, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 2 
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 3
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 4
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 5
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 6
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 7
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 8
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 9
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 10
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 11
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 12
                      0, 0, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 13
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 14
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 15
                };

                target.Channel = RGB.R;
                target.Convert(image, out output);

                for (int i = 0; i < outputExpected.Length; i++)
                        Assert.AreEqual(outputExpected[i], output[i]);
            }

            {
                double[] output;
                double[] outputExpected =
                {
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 0
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 1
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 0, 0 , // 2 
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 3
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 4
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 5
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 6
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 7
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 8
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 9
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 10
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 11
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 12
                      0, 0, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 13
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 14
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 15
                };

                target.Channel = RGB.G;
                target.Convert(image, out output);

                for (int i = 0; i < outputExpected.Length; i++)
                    for (int j = 0; j < outputExpected.Length; j++)
                        Assert.AreEqual(outputExpected[i], output[i]);
            }

            {
                double[] output;
                double[] outputExpected =
                {
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 0
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 1
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 2 
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 3
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 4
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 5
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 6
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 7
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 8
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 9
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 10
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 11
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 12
                      0, 0, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 0, 0 , // 13
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 14
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 15
                };

                target.Channel = RGB.B;
                target.Convert(image, out output);

                for (int i = 0; i < outputExpected.Length; i++)
                        Assert.AreEqual(outputExpected[i], output[i]);
            }
        }

    }
}
