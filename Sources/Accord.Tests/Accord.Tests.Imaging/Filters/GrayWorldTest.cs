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
    using Accord.Controls;


    [TestClass]
    public class GrayWorldTest
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
        public void ApplyTest1()
        {
            Bitmap image = Properties.Resources.lena_color;

            // Create the Gray World filter
            var grayWorld = new GrayWorld();

            // Apply the filter
            Bitmap result = grayWorld.Apply(image);

            // ImageBox.Show(result);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ProcessImageTest()
        {
            double[,] diag = Matrix.Magic(5);

            Bitmap input;
            new MatrixToImage()
            {
                Format = PixelFormat.Format24bppRgb
            }.Convert(diag, out input);

            GrayWorld gabor = new GrayWorld();

            // Apply the filter
            Bitmap output = gabor.Apply(input);

            double[,] actual; 
            
            new ImageToMatrix().Convert(output, out actual);

            string str = actual.ToString(CSharpMatrixFormatProvider.InvariantCulture);

            double[,] expected = 
            {
                { 0.309803921568627, 0.301960784313725, 0.333333333333333, 0.32156862745098, 0.313725490196078 },
                { 0.301960784313725, 0.325490196078431, 0.325490196078431, 0.313725490196078, 0.313725490196078 },
                { 0.329411764705882, 0.325490196078431, 0.317647058823529, 0.305882352941176, 0.305882352941176 },
                { 0.32156862745098, 0.317647058823529, 0.309803921568627, 0.305882352941176, 0.329411764705882 },
                { 0.317647058823529, 0.309803921568627, 0.301960784313725, 0.329411764705882, 0.32156862745098 } 
            };

            Assert.IsTrue(expected.IsEqual(actual, 1e-6));
        }

    }
}
