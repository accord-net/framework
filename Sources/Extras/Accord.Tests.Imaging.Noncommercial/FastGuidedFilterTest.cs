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

namespace Accord.Tests.Extras.Imaging.Noncommercial
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using Accord.Imaging.Converters;
    using Accord.Imaging.Filters;
    using Accord.Math;
    using NUnit.Framework;

    [TestFixture]
    public class FastGuidedFilterTest
    {

        [Test]
        public void ApplyTest_color24()
        {
            Bitmap image = Accord.Imaging.Image.Clone(Properties.Resources.lena_color);
            Bitmap overlay = Accord.Imaging.Image.Clone(Properties.Resources.lena_color);

            Assert.AreEqual(image.PixelFormat, PixelFormat.Format24bppRgb);

            var fg = new FastGuidedFilter(overlay);

            // Apply the filter
            Bitmap result = fg.Apply(image);

            // ImageBox.Show(result);
            Assert.IsNotNull(result);
        }

        [Test]
        public void ApplyTest_color32()
        {
            Bitmap image = Accord.Imaging.Image.Clone(Properties.Resources.lena_color);
            Bitmap overlay = Accord.Imaging.Image.Clone(Properties.Resources.lena_color);

            image.MakeTransparent();
            overlay.MakeTransparent();

            Assert.AreEqual(image.PixelFormat, PixelFormat.Format32bppArgb);

            var fg = new FastGuidedFilter(overlay);

            // Apply the filter
            Bitmap result = fg.Apply(image);

            // ImageBox.Show(result);
            Assert.IsNotNull(result);
        }


        [Test]
        public void ApplyTest_bw()
        {
            Bitmap image = Accord.Imaging.Image.Clone(Properties.Resources.lena512);
            Bitmap overlay = Accord.Imaging.Image.Clone(Properties.Resources.lena512);

            Assert.AreEqual(image.PixelFormat, PixelFormat.Format8bppIndexed);

            var fg = new FastGuidedFilter(overlay);

            // Apply the filter
            Bitmap result = fg.Apply(image);

            // ImageBox.Show(result);
            Assert.IsNotNull(result);
        }

    }
}
