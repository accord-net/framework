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
    using Accord.Math;
    using NUnit.Framework;
    using System.Drawing.Imaging;
    using Accord.Tests.Imaging.Properties;
#if NO_BITMAP
    using Resources = Accord.Tests.Imaging.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class VarianceFilterTest
    {


        [Test]
        public void VarianceTest1()
        {
            Bitmap image = Accord.Imaging.Image.Clone(Resources.lena512);
            Variance variance = new Variance();
            Bitmap result = variance.Apply(image);
            Assert.IsNotNull(result);
        }

        [Test]
        [Category("Slow")]
        public void VarianceColorRotate()
        {
            Bitmap image = Accord.Imaging.Image.Clone(Resources.wiki_flower);
            Bitmap expected = Accord.Imaging.Image.Clone(Resources.variance_color_expected);

            bool answer = ImageUtils.RotateTest32bpp(new Variance(), image, expected);
            Assert.IsTrue(answer);
        }

        [Test]
        public void ProcessImageTest()
        {
            double[,] diag = Matrix.Magic(5);

            Bitmap input;
            new MatrixToImage().Convert(diag, out input);

            // Create a new Variance filter
            Variance filter = new Variance();

            // Apply the filter
            Bitmap output = filter.Apply(input);

            double[,] actual; 
            
            new ImageToMatrix().Convert(output, out actual);

            string str = actual.ToString(CSharpMatrixFormatProvider.InvariantCulture);

            double[,] expected = 
            {
                { 0, 0, 0, 0, 0 },
                { 0.0941176470588235, 0.545098039215686, 0.396078431372549, 0.376470588235294, 0.192156862745098 },
                { 0.298039215686275, 0.376470588235294, 0.27843137254902, 0.211764705882353, 0.133333333333333 },
                { 0.317647058823529, 0.203921568627451, 0.2, 0.16078431372549, 0.109803921568627 },
                { 0.0509803921568627, 0.109803921568627, 0.16078431372549, 0.2, 0.203921568627451 } 
            };

            Assert.IsTrue(expected.IsEqual(actual, 1e-6));
        }

    }
}
