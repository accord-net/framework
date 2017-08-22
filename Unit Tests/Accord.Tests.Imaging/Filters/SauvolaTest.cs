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
    using Accord.Imaging.Converters;
    using Accord.Imaging.Filters;
    using Accord.Math;
    using Accord.Tests.Imaging.Properties;
    using NUnit.Framework;
    using System.Drawing;
    using System.Drawing.Imaging;
#if NO_BITMAP
    using Resources = Accord.Tests.Imaging.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class SauvolaTest
    {

        [Test]
        public void SauvolaTest1()
        {
            Bitmap image = Accord.Imaging.Image.Clone(Resources.lena512);

            SauvolaThreshold sauvola = new SauvolaThreshold();

            Bitmap result = sauvola.Apply(image);

            // ImageBox.Show(result);

            Assert.IsNotNull(result);
        }

        [Test]
        public void SauvolaTest2()
        {
            double[,] diag = Matrix.Magic(5);

            Bitmap input;
            new MatrixToImage().Convert(diag, out input);

            // Create a new Variance filter
            SauvolaThreshold filter = new SauvolaThreshold();

            Assert.AreEqual(PixelFormat.Format8bppIndexed, input.PixelFormat);

            // Apply the filter
            Bitmap output = filter.Apply(input);

            double[,] actual;

            new ImageToMatrix().Convert(output, out actual);

            Assert.AreEqual(PixelFormat.Format8bppIndexed, output.PixelFormat);

            string str = actual.ToString(CSharpMatrixFormatProvider.InvariantCulture);

            double[,] expected = new double[,] 
            {
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 } 
            };

            Assert.IsTrue(expected.IsEqual(actual, 1e-6));
        }

        [Test]
        public void SauvolaTest3()
        {
            double[,] diag = Matrix.Magic(5);

            Bitmap input;
            new MatrixToImage()
            {
                Format = PixelFormat.Format32bppRgb,
            }.Convert(diag, out input);

            Assert.AreEqual(PixelFormat.Format32bppRgb, input.PixelFormat);

            SauvolaThreshold filter = new SauvolaThreshold();

            // Apply the filter
            Bitmap output = filter.Apply(input);

            Assert.AreEqual(PixelFormat.Format32bppRgb, output.PixelFormat);

            double[,] actual;

            for (int i = 0; i < 3; i++)
            {
                new ImageToMatrix()
                {
                    Channel = i
                }.Convert(output, out actual);

                string str = actual.ToString(CSharpMatrixFormatProvider.InvariantCulture);

                double[,] expected = new double[,] 
                {
                    { 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1 } 
                };

                Assert.IsTrue(expected.IsEqual(actual, 1e-6));
            }
        }

    }
}
