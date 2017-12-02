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
    using System.Windows.Media;
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

            var conv = new BitmapSourceToArray();

            double[] array; conv.Convert(image, out array);

            var conv2 = new ArrayToImage(width: 4, height: 4);
            Bitmap image2; conv2.Convert(pixels, out image2);

            Assert.AreEqual(0, array.Min());
            Assert.AreEqual(1, array.Max());
            Assert.AreEqual(16, array.Length);
            var expected = image2.ToVector(0);

            Assert.AreEqual(array, expected);
        }

        [Test]
        public void ConvertTest5()
        {
            double[] input =
            {
                0, 0.1,   0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0, 0.42, 0.42, 0.42, 0.42, 0.42, // 0
                0.11, 0.12,   0.13, 0.14, 0.15, 0.16, 0.17, 0.18, 0.19, 0.20, 0.21, 0.22, 0.23, 0.24, 0.25, 0.26, // 1
            };

            BitmapSource image = input.ToBitmapSource(2, 16);

            double[,] actual = image.ToMatrix(0);
            double[,] expected = input.Reshape(2, 16);

            Assert.IsTrue(expected.IsEqual(actual, 1e-6));
        }

        [Test]
        public void ConvertTest6()
        {
            var target = new BitmapSourceToArray();

            double[] input =
            {
                0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0, 0.42, 0.42, 0.42, 0.42, 0.42, // 0
                0.11, 0.12, 0.13, 0.14, 0.15, 0.16, 0.17, 0.18, 0.19, 0.20, 0.21, 0.22, 0.23, 0.24, 0.25, 0.26, // 1
            };

            BitmapSource image = input.ToBitmapSource(2, 16);
            Assert.AreEqual(PixelFormats.Gray32Float, image.Format);

            Assert.AreEqual(1, image.GetNumberOfChannels());

            double[] output;
            target.Channel = RGB.R;
            target.Convert(image, out output);

            for (int i = 0; i < input.Length; i++)
                Assert.AreEqual(input[i], output[i], 1e-5);
        }


        [Test]
        public void ConvertTest4()
        {
            var target = new BitmapSourceToArray();
            BitmapSource image = Accord.Imaging.Image.Clone(Resources.image3).ToBitmapSource();
            Assert.AreEqual(System.Drawing.Imaging.PixelFormat.Format32bppArgb.ToWPF(), image.Format);
            Assert.AreEqual(4, image.GetNumberOfChannels());

            {
                double[] output;
                double[] outputExpected =
                {
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 0
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 1
                      0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 2 
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
                      0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 13
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 14
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 15
                };

                target.Channel = 0;
                target.Convert(image, out output);

                double a = output[34];
                double e = outputExpected[34];
                Assert.AreEqual(e, a);

                for (int i = 0; i < outputExpected.Length; i++)
                    Assert.AreEqual(outputExpected[i], output[i]);
            }

            {
                double[] output;
                double[] outputExpected =
                {
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 0
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 1
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 , // 2 
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
                      0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 13
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 14
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 15
                };

                target.Channel = 1;
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
                      0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 , // 13
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 14
                      0, 0,   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   0, 0, 0 , // 15
                };

                target.Channel = 2;
                target.Convert(image, out output);

                for (int i = 0; i < outputExpected.Length; i++)
                    Assert.AreEqual(outputExpected[i], output[i]);
            }
        }

    }
}
