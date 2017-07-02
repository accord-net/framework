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
    using Accord.Imaging;

#if NETSTANDARD2_0
    using Resources = Accord.Tests.Imaging.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class ResizeBilinearTest
    {

        [Test]
        public void resize_bilinear()
        {
            double[,] diag = Matrix.Magic(5);
            diag = diag.Divide(diag.Max());

            Bitmap input = diag.ToBitmap();

            // Create a new resize bilinear filter
            var filter = new ResizeBilinear(7, 8);

            // Apply the filter
            Bitmap output = filter.Apply(input);

            Assert.AreEqual(7, output.Width);
            Assert.AreEqual(8, output.Height);

            double[,] actual; 
            new ImageToMatrix().Convert(output, out actual);

            string str = actual.ToString(CSharpMatrixFormatProvider.InvariantCulture);

            double[,] expected = 
            {
                { 0.67843137254902, 0.874509803921569, 0.56078431372549, 0.0784313725490196, 0.274509803921569, 0.47843137254902, 0.6 },
                { 0.827450980392157, 0.580392156862745, 0.356862745098039, 0.227450980392157, 0.423529411764706, 0.556862745098039, 0.623529411764706 },
                { 0.725490196078431, 0.356862745098039, 0.262745098039216, 0.376470588235294, 0.576470588235294, 0.662745098039216, 0.698039215686274 },
                { 0.250980392156863, 0.23921568627451, 0.341176470588235, 0.525490196078431, 0.725490196078431, 0.811764705882353, 0.847058823529412 },
                { 0.27843137254902, 0.333333333333333, 0.474509803921569, 0.662745098039216, 0.792156862745098, 0.635294117647059, 0.498039215686275 },
                { 0.403921568627451, 0.47843137254902, 0.627450980392157, 0.780392156862745, 0.749019607843137, 0.4, 0.145098039215686 },
                { 0.427450980392157, 0.592156862745098, 0.776470588235294, 0.843137254901961, 0.36078431372549, 0.282352941176471, 0.294117647058824 },
                { 0.43921568627451, 0.635294117647059, 0.835294117647059, 0.866666666666667, 0.207843137254902, 0.235294117647059, 0.356862745098039 }
            };

            Assert.IsTrue(expected.IsEqual(actual, 1e-6));
        }

        [Test]
        public void resize_bicubic()
        {
            double[,] diag = Matrix.Magic(5);
            diag = diag.Divide(diag.Max());

            Bitmap input = diag.ToBitmap();

            // Create a new resize bilinear filter
            var filter = new ResizeBicubic(7, 8);

            // Apply the filter
            Bitmap output = filter.Apply(input);

            Assert.AreEqual(7, output.Width);
            Assert.AreEqual(8, output.Height);

            double[,] actual;
            new ImageToMatrix().Convert(output, out actual);

            string str = actual.ToString(CSharpMatrixFormatProvider.InvariantCulture);

            double[,] expected = 
                {
                { 0.725490196078431, 0.780392156862745, 1, 0.352941176470588, 0.0313725490196078, 0.341176470588235, 0.588235294117647 },
                { 0.733333333333333, 0.749019607843137, 0.909803921568627, 0.313725490196078, 0.0745098039215686, 0.352941176470588, 0.56078431372549 },
                { 1, 0.827450980392157, 0.376470588235294, 0.211764705882353, 0.286274509803922, 0.501960784313725, 0.596078431372549 },
                { 0.733333333333333, 0.568627450980392, 0.16078431372549, 0.250980392156863, 0.470588235294118, 0.666666666666667, 0.745098039215686 },
                { 0.16078431372549, 0.164705882352941, 0.223529411764706, 0.407843137254902, 0.623529411764706, 0.811764705882353, 0.866666666666667 },
                { 0.290196078431373, 0.290196078431373, 0.364705882352941, 0.56078431372549, 0.780392156862745, 0.874509803921569, 0.462745098039216 },
                { 0.443137254901961, 0.43921568627451, 0.529411764705882, 0.745098039215686, 0.819607843137255, 0.654901960784314, 0.203921568627451 },
                { 0.447058823529412, 0.474509803921569, 0.67843137254902, 0.96078431372549, 0.72156862745098, 0.12156862745098, 0.282352941176471 }
            };

            Assert.IsTrue(expected.IsEqual(actual, 1e-6));
        }

        [Test]
        public void resize_nearest()
        {
            double[,] diag = Matrix.Magic(5);
            diag = diag.Divide(diag.Max());

            Bitmap input = diag.ToBitmap();

            // Create a new resize bilinear filter
            var filter = new ResizeNearestNeighbor(7, 8);

            // Apply the filter
            Bitmap output = filter.Apply(input);

            Assert.AreEqual(7, output.Width);
            Assert.AreEqual(8, output.Height);

            double[,] actual;
            new ImageToMatrix().Convert(output, out actual);

            string str = actual.ToString(CSharpMatrixFormatProvider.InvariantCulture);

            double[,] expected =  
            {
                { 0.67843137254902, 0.67843137254902, 0.956862745098039, 0.0392156862745098, 0.0392156862745098, 0.317647058823529, 0.6 },
                { 0.67843137254902, 0.67843137254902, 0.956862745098039, 0.0392156862745098, 0.0392156862745098, 0.317647058823529, 0.6 },
                { 0.917647058823529, 0.917647058823529, 0.2, 0.27843137254902, 0.27843137254902, 0.556862745098039, 0.63921568627451 },
                { 0.917647058823529, 0.917647058823529, 0.2, 0.27843137254902, 0.27843137254902, 0.556862745098039, 0.63921568627451 },
                { 0.156862745098039, 0.156862745098039, 0.23921568627451, 0.517647058823529, 0.517647058823529, 0.8, 0.87843137254902 },
                { 0.4, 0.4, 0.47843137254902, 0.756862745098039, 0.756862745098039, 0.83921568627451, 0.117647058823529 },
                { 0.4, 0.4, 0.47843137254902, 0.756862745098039, 0.756862745098039, 0.83921568627451, 0.117647058823529 },
                { 0.43921568627451, 0.43921568627451, 0.717647058823529, 1, 1, 0.0784313725490196, 0.356862745098039 }
            };

            Assert.IsTrue(expected.IsEqual(actual, 1e-6));
        }

    }
}
