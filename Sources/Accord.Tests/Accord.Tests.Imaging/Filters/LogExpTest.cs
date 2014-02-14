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
    public class LogExpTest
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



        [TestMethod]
        public void ApplyTest()
        {
            Bitmap input = Properties.Resources.lena_color;

            Logarithm log = new Logarithm();
            Bitmap output = log.Apply(input);

            Exponential exp = new Exponential();
            Bitmap reconstruction = exp.Apply(output);

            //ImageBox.Show("input", input);
            //ImageBox.Show("output", output);
            //ImageBox.Show("reconstruction", reconstruction);
        }

        [TestMethod]
        public void ProcessImageTest()
        {
            double[,] diag = Matrix.Magic(5);

            Bitmap input;
            new MatrixToImage(0, 100).Convert(diag, out input);

            Logarithm log = new Logarithm();
            Bitmap output = log.Apply(input);

            Exponential exp = new Exponential();
            Bitmap reconstruction = exp.Apply(output);


            double[,] actual;
            new ImageToMatrix(0, 100).Convert(reconstruction, out actual);
            
            double[,] expected = diag;

            Assert.IsTrue(expected.IsEqual(actual, 1.5));
        }

    }
}
