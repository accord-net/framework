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
    public class RobinsonEdgeDetectorTest
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

            RobinsonEdgeDetector robinson = new RobinsonEdgeDetector();

            Bitmap edges = robinson.Apply(image);

            // ImageBox.Show(edges);
            Assert.IsNotNull(edges);
        }

        [TestMethod()]
        public void ColorApplyTest1()
        {
            Bitmap image = Properties.Resources.lena_color;

            RobinsonEdgeDetector robinson = new RobinsonEdgeDetector();

            Bitmap edges = robinson.Apply(image);

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
            RobinsonEdgeDetector filter = new RobinsonEdgeDetector();

            // Apply the filter
            Bitmap output = filter.Apply(input);

            double[,] actual;

            new ImageToMatrix().Convert(output, out actual);

            string str = actual.ToString(CSharpMatrixFormatProvider.InvariantCulture);

            double[,] expected = 
            {
                { 0.945098039215686, 1, 0.976470588235294, 0.984313725490196, 0.964705882352941 },
                { 0.996078431372549, 0.164705882352941, 0.0705882352941176, 0.247058823529412, 0.972549019607843 },
                { 0.980392156862745, 0.0509803921568627, 0.227450980392157, 0.0509803921568627, 0.937254901960784 },
                { 0.980392156862745, 0.247058823529412, 0.0705882352941176, 0.164705882352941, 0.988235294117647 },
                { 0.96078431372549, 0.980392156862745, 0.945098039215686, 0.988235294117647, 0.968627450980392 } 
            };

            Assert.IsTrue(expected.IsEqual(actual, 1e-6));
        }
    }
}
