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

using Accord.Imaging.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accord.Math.Wavelets;
using System.Drawing;
using Accord.Math;
using Accord.Imaging;
using Accord.Controls;

namespace Accord.Tests.Imaging
{
    
    
    [TestClass()]
    public class WaveletTransformTest
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

            double[,] actual = org.ToDoubleMatrix(0);
            double[,] expected = src.ToDoubleMatrix(0);

            Assert.IsTrue(actual.IsEqual(expected, 0.102));
        }
    }
}
