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

namespace Accord.Tests.MachineLearning
{
    using System.Collections.Generic;
    using System.Drawing;
    using Accord.MachineLearning.Geometry;
    using AForge;
    using Accord.Tests.Imaging.Properties;
    using NUnit.Framework;
    using Point = Accord.Point;
    using Accord.Imaging;
    using Accord.Imaging.Filters;
    using Accord.Math.Geometry;
    using System.Diagnostics;
#if NO_BITMAP
    using Resources = Accord.Tests.Imaging.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class RansacLineImageTest
    {

        [Test]
#if MONO
        [Ignore("Disabled on Mono")]
#endif
        public void RansacLineConstructorTest2()
        {
            Accord.Math.Random.Generator.Seed = 0;

            Bitmap image = Accord.Imaging.Image.Clone(Resources.noise_line);

            //Accord.Controls.ImageBox.Show(image); 

            var detector = new SusanCornersDetector();

            List<IntPoint> cloud = detector.ProcessImage(image);
            Assert.AreEqual(211, cloud.Count);

            Bitmap marks = new PointsMarker(cloud, Color.Pink).Apply(image);
            //Accord.Controls.ImageBox.Show(marks);

            RansacLine ransac = new RansacLine(5, 1e-10);
            Line line = ransac.Estimate(cloud);

            Assert.AreEqual(0.501134932f, line.Intercept, 1e-5);
            Assert.AreEqual(-0.865369201f, line.Slope, 1e-5);

            //var result = new LineMarker(line).Apply(image);
            //Accord.Controls.ImageBox.Show(result);
        }

    }
}
