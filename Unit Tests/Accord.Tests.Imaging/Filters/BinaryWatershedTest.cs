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
    using Accord.DataSets;
    using Accord.Imaging.Converters;
    using Accord.Imaging.Filters;
    using Accord.Math;
    using Accord.Tests.Imaging.Properties;
    using NUnit.Framework;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
#if NO_BITMAP
    using Resources = Accord.Tests.Imaging.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class BinaryWatershedTest
    {

        [Test]
        public void ApplyTest1()
        {
            string basePath = Path.Combine(NUnit.Framework.TestContext.CurrentContext.TestDirectory, "watershed");
            Directory.CreateDirectory(basePath);

            Bitmap shapes = Accord.Imaging.Image.Clone(Resources.water);
            shapes.Save(Path.Combine(basePath, "shapes.jpg"));

            var bw = new BinaryWatershed();
            Bitmap result = bw.Apply(shapes);

            Assert.AreEqual(746, result.Width);
            Assert.AreEqual(643, result.Height);
            Assert.AreEqual(PixelFormat.Format8bppIndexed, result.PixelFormat);

            Assert.AreEqual(9, bw.MaxPoints.Count);

            string strX = bw.MaxPoints.Select(i => i.X).ToArray().ToCSharp();
            string strY = bw.MaxPoints.Select(i => i.Y).ToArray().ToCSharp();

            double[] x = new double[] { 310, 546, 136, 254, 429, 612, 398, 345, 498 };
            double[] y = new double[] { 436, 153, 392, 201, 336, 339, 242, 183, 319 };

            Assert.AreEqual(x, bw.MaxPoints.Select(i => i.X).ToArray());
            Assert.AreEqual(y, bw.MaxPoints.Select(i => i.Y).ToArray());

            result.Save(Path.Combine(basePath, "watershed.jpg"));

            GrayscaleToRGB toRGB = new GrayscaleToRGB();
            result = toRGB.Apply(result);

            PointsMarker marker = new PointsMarker(Color.Red, 5);
            marker.Points = bw.MaxPoints;
            Bitmap marked = marker.Apply(result);

            marked.Save(Path.Combine(basePath, "watershed-marks.jpg"));

            Assert.IsNotNull(result);
            Assert.IsNotNull(marked);
        }

    }
}
