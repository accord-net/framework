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
    using Accord.Math;
    using Accord.Imaging.Filters;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Drawing;
    using Accord.Controls.Imaging;
    using Accord.Imaging.Converters;


    [TestClass]
    public class GaborFilterTest
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



        [TestMethod()]
        public void GaborTest1()
        {
            Bitmap image = Properties.Resources.lena512;

            GaborFilter gabor = new GaborFilter();
            
            Bitmap result = gabor.Apply(image);

            // ImageBox.Show(result);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ProcessImageTest()
        {
            double[,] diag = Matrix.Magic(5);

            Bitmap input;
            new MatrixToImage().Convert(diag, out input);

            // Create a new Gabor filter
            GaborFilter gabor = new GaborFilter();

            // Apply the filter
            Bitmap output = gabor.Apply(input);

            double[,] actual; 
            
            new ImageToMatrix().Convert(output, out actual);

            double[,] expected = 
            {
                { 0.192156862745098, 0.176470588235294, 0.254901960784314, 0.396078431372549, 0.529411764705882 },
                { 0.16078431372549, 0.305882352941176, 0.494117647058824, 0.635294117647059, 0.654901960784314 },
                { 0.407843137254902, 0.623529411764706, 0.737254901960784, 0.701960784313725, 0.564705882352941 },
                { 0.752941176470588, 0.815686274509804, 0.713725490196078, 0.541176470588235, 0.403921568627451 },
                { 0.847058823529412, 0.694117647058824, 0.505882352941176, 0.380392156862745, 0.329411764705882 } 
            };

            Assert.IsTrue(expected.IsEqual(actual, 1e-6));
        }

    }
}
