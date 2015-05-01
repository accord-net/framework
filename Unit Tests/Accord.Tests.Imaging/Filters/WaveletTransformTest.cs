// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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

using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Math;
using Accord.Math.Wavelets;
using AForge.Imaging.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace Accord.Tests.Imaging
{
    [TestClass()]
    public class WaveletTransformTest
    {

        [TestMethod()]
        public void Example1()
        {
            Bitmap image = Properties.Resources.lena512;

            // Create a new Haar Wavelet transform filter
            var wavelet = new WaveletTransform(new Haar(1));

            // Apply the Wavelet transformation
            Bitmap result = wavelet.Apply(image);

            // Show on the screen
            //ImageBox.Show(result);
            Assert.IsNotNull(result);
            
            // Extract only one of the resulting images
            var crop = new Crop(new Rectangle(0, 0, 
                image.Width / 2, image.Height / 2));

            Bitmap quarter = crop.Apply(result);

            // Show on the screen
            //ImageBox.Show(quarter);
            Assert.IsNotNull(quarter);
        }

        [TestMethod()]
        public void WaveletTransformConstructorTest()
        {
            // Start with a grayscale image
            Bitmap src = Properties.Resources.lena512;

            // Create a wavelet filter            
            IWavelet wavelet = new Accord.Math.Wavelets.Haar(2);
            WaveletTransform target = new WaveletTransform(wavelet);

            // Apply the transformation
            Bitmap dst = target.Apply(src);

            // Revert the transformation
            target.Backward = true;
            Bitmap org = target.Apply(dst);

#pragma warning disable 0618
            double[,] actual = org.ToDoubleMatrix(0);
            double[,] expected = src.ToDoubleMatrix(0);
#pragma warning restore 0618

            Assert.IsTrue(actual.IsEqual(expected, 0.102));
        }
    }
}
