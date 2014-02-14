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
    using Accord.Imaging.Filters;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Drawing;
    using Accord.Controls.Imaging;
    using Accord.Imaging.Converters;
    using Accord.Math;

    [TestClass()]
    public class KirschEdgeDetectorTest
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
            Bitmap image = Properties.Resources.lena512;

            KirschEdgeDetector kirsch = new KirschEdgeDetector();

            Bitmap edges = kirsch.Apply(image);

            // ImageBox.Show(edges);

            Assert.IsNotNull(edges);
        }

        [TestMethod]
        public void ProcessImageTest()
        {
            double[,] diag = Matrix.Magic(5);

            Bitmap input;
            new MatrixToImage().Convert(diag, out input);

            // Create a new Gabor filter
            KirschEdgeDetector filter = new KirschEdgeDetector();

            // Apply the filter
            Bitmap output = filter.Apply(input);

            double[,] actual;

            new ImageToMatrix().Convert(output, out actual);

            // string str = actual.ToString(Accord.Math.Formats.CSharpMatrixFormatProvider.InvariantCulture);

            double[,] expected = 
            {
                { 1, 1, 1, 1, 1 },
                { 1, 0.458823529411765, 0.349019607843137, 0.698039215686274, 1 },
                { 1, 0.309803921568627, 0.658823529411765, 0.286274509803922, 1 },
                { 1, 0.619607843137255, 0.403921568627451, 0.890196078431373, 1 },
                { 1, 1, 1, 1, 0.968627450980392 }  
            };

            Assert.IsTrue(expected.IsEqual(actual, 1e-6));
        }
    }
}
