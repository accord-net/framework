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
    using Accord.Tests.Imaging.Properties;
#if NO_BITMAP
    using Resources = Accord.Tests.Imaging.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class DifferenceOfGaussiansTest
    {


        [Test]
        public void ApplyTest1()
        {
            Bitmap image = Accord.Imaging.Image.Clone(Resources.lena512);

            var dog = new DifferenceOfGaussians();
            
            Bitmap result = dog.Apply(image);

            // ImageBox.Show(result);
            Assert.IsNotNull(result);
        }

        [Test]
        public void ProcessImageTest()
        {
            double[,] diag = Matrix.Magic(5);

            Bitmap input;
            new MatrixToImage().Convert(diag, out input);

            DifferenceOfGaussians gabor = new DifferenceOfGaussians();

            // Apply the filter
            Bitmap output = gabor.Apply(input);

            double[,] actual; 
            
            new ImageToMatrix().Convert(output, out actual);

            string str = actual.ToString(CSharpMatrixFormatProvider.InvariantCulture);

            double[,] expected = 
            {
                { 0.00784313725490196, 0.0274509803921569, 0, 0, 0.00392156862745098 },
                { 0.0196078431372549, 0, 0, 0.00392156862745098, 0.00392156862745098 },
                { 0, 0, 0, 0.0156862745098039, 0.0156862745098039 },
                { 0, 0, 0.0156862745098039, 0.0117647058823529, 0 },
                { 0, 0.00784313725490196, 0.0196078431372549, 0, 0 } 
            };

            Assert.IsTrue(expected.IsEqual(actual, 1e-6));
        }

    }
}
