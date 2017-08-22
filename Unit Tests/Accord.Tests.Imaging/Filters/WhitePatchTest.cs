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
    using Accord.Imaging.Filters;
    using Accord.Math;
    using NUnit.Framework;
    using Accord.Tests.Imaging.Properties;
#if NO_BITMAP
    using Resources = Accord.Tests.Imaging.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class WhitePatchTest
    {

        [Test]
        public void ApplyTest1()
        {
            Bitmap image = Accord.Imaging.Image.Clone(Resources.lena_color);

            // Create the White Patch filter
            var whitePatch = new WhitePatch();

            // Apply the filter
            Bitmap result = whitePatch.Apply(image);

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

            var whitePatch = new WhitePatch();

            // Apply the filter
            Bitmap output = whitePatch.Apply(input);

            double[,] actual; 
            
            new ImageToMatrix().Convert(output, out actual);

            string str = actual.ToString(CSharpMatrixFormatProvider.InvariantCulture);

            double[,] expected = 
            {
                { 1, 0.968627450980392, 1, 1, 1 },
                { 0.972549019607843, 1, 1, 1, 1 },
                { 1, 1, 1, 0.984313725490196, 0.976470588235294 },
                { 1, 1, 0.988235294117647, 0.980392156862745, 1 },
                { 1, 0.992156862745098, 0.964705882352941, 1, 1 } 
            };

            Assert.IsTrue(expected.IsEqual(actual, 1e-6));
        }

    }
}
