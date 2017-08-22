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
    using Accord.Imaging.Converters;
    using Accord.Imaging.Filters;
    using Accord.Math;
    using Accord.Tests.Imaging.Properties;
    using NUnit.Framework;
    using System.Drawing;
    using System.Drawing.Imaging;
#if NO_BITMAP
    using Resources = Accord.Tests.Imaging.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class GrayWorldTest
    {

        [Test]
        public void ApplyTest1()
        {
            Bitmap image = Accord.Imaging.Image.Clone(Resources.lena_color);

            // Create the Gray World filter
            var grayWorld = new GrayWorld();

            // Apply the filter
            Bitmap result = grayWorld.Apply(image);

            // ImageBox.Show(result);

            Assert.IsNotNull(result);
        }

        [Test]
        public void ProcessImageTest()
        {
            double[,] diag = Matrix.Magic(5);

            Bitmap input;
            new MatrixToImage()
            {
                Format = PixelFormat.Format24bppRgb
            }.Convert(diag, out input);

            Assert.AreEqual(PixelFormat.Format24bppRgb, input.PixelFormat);

            GrayWorld gabor = new GrayWorld();

            // Apply the filter
            Bitmap output = gabor.Apply(input);

            Assert.AreEqual(PixelFormat.Format24bppRgb, output.PixelFormat);

            double[,] actual; 
            
            new ImageToMatrix().Convert(output, out actual);

            string str = actual.ToString(CSharpMatrixFormatProvider.InvariantCulture);

            double[,] expected =
            {
                { 0.937254901960784, 0.909803921568627, 1, 0.972549019607843, 0.945098039215686 },
                { 0.913725490196078, 0.984313725490196, 0.976470588235294, 0.949019607843137, 0.941176470588235 },
                { 0.988235294117647, 0.980392156862745, 0.952941176470588, 0.925490196078431, 0.917647058823529 },
                { 0.964705882352941, 0.956862745098039, 0.929411764705882, 0.92156862745098, 0.992156862745098 },
                { 0.96078431372549, 0.933333333333333, 0.905882352941176, 0.996078431372549, 0.968627450980392 } 
            };

            Assert.IsTrue(expected.IsEqual(actual, 1e-6));
        }

        [Test]
        public void ProcessImageTest2()
        {
            double[,] diag = Matrix.Magic(5);

            Bitmap input;
            new MatrixToImage()
            {
                Format = PixelFormat.Format32bppArgb
            }.Convert(diag, out input);

            Assert.AreEqual(PixelFormat.Format32bppArgb, input.PixelFormat);

            GrayWorld gabor = new GrayWorld();

            // Apply the filter
            Bitmap output = gabor.Apply(input);

            Assert.AreEqual(PixelFormat.Format32bppArgb, output.PixelFormat);

            double[,] actual;

            new ImageToMatrix().Convert(output, out actual);

            string str = actual.ToString(CSharpMatrixFormatProvider.InvariantCulture);

            double[,] expected =
            {
                { 0.937254901960784, 0.909803921568627, 1, 0.972549019607843, 0.945098039215686 },
                { 0.913725490196078, 0.984313725490196, 0.976470588235294, 0.949019607843137, 0.941176470588235 },
                { 0.988235294117647, 0.980392156862745, 0.952941176470588, 0.925490196078431, 0.917647058823529 },
                { 0.964705882352941, 0.956862745098039, 0.929411764705882, 0.92156862745098, 0.992156862745098 },
                { 0.96078431372549, 0.933333333333333, 0.905882352941176, 0.996078431372549, 0.968627450980392 } 
            };

            Assert.IsTrue(expected.IsEqual(actual, 1e-6));
        }

    }
}
