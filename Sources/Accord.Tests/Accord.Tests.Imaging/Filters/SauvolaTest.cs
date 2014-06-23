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
    public class SauvolaTest
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
        public void SauvolaTest1()
        {
            Bitmap image = Properties.Resources.lena512;

            SauvolaThreshold sauvola = new SauvolaThreshold();

            Bitmap result = sauvola.Apply(image);

            // ImageBox.Show(result);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void NiblackTest2()
        {
            double[,] diag = Matrix.Magic(5);

            Bitmap input;
            new MatrixToImage().Convert(diag, out input);

            // Create a new Variance filter
            SauvolaThreshold filter = new SauvolaThreshold();

            // Apply the filter
            Bitmap output = filter.Apply(input);

            double[,] actual;

            new ImageToMatrix().Convert(output, out actual);

            string str = actual.ToString(CSharpMatrixFormatProvider.InvariantCulture);

            double[,] expected = 
            {
                { 0, 0, 0, 0, 0 },
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 } 
            };

            Assert.IsTrue(expected.IsEqual(actual, 1e-6));
        }

    }
}
