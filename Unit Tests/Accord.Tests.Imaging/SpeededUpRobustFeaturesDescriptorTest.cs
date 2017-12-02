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

    using System.Collections.Generic;
    using System.Drawing;
    using Accord.Imaging;
    using Accord.Math;
    using Accord.Tests.Imaging.Properties;
    using NUnit.Framework;

    [TestFixture]
    public class SpeededUpRobustFeaturesDescriptorTest
    {

        [Test]
        [Category("Slow")]
        public void ProcessImageTest()
        {
            var bitmaps = SpeededUpRobustFeaturesDetectorTest.GetImages();

            foreach (Bitmap img in bitmaps)
            {

                bool upright = true;
                bool extended = false;

                List<SpeededUpRobustFeaturePoint> expected;
                List<SpeededUpRobustFeaturePoint> actual;

                // Create OpenSURF detector by Chris Evans
                {
                    // Create Integral Image
                    var clone = Accord.Imaging.Image.Clone(img);
                    OpenSURFcs.IntegralImage iimg = OpenSURFcs.IntegralImage.FromImage(clone);

                    // Extract the interest points
                    var pts = OpenSURFcs.FastHessian.getIpoints(0.0002f, 5, 2, iimg);

                    // Describe the interest points
                    OpenSURFcs.SurfDescriptor.DecribeInterestPoints(pts, upright, extended, iimg);

                    expected = new List<SpeededUpRobustFeaturePoint>();
                    foreach (var p in pts)
                    {
                        expected.Add(new SpeededUpRobustFeaturePoint(
                            p.x, p.y, p.scale,
                            p.laplacian, p.orientation,
                            p.response, p.descriptor.ToDouble()));
                    }
                }

                {
                    // Create the detector
                    var surf = new SpeededUpRobustFeaturesDetector(0.0002f, 5, 2);

                    // Extract interest points
                    var clone = Accord.Imaging.Image.Clone(img);
                    actual = surf.ProcessImage(clone);

                    // Describe the interest points
                    var descriptor = surf.GetDescriptor();
                    descriptor.Invariant = !upright;
                    descriptor.Extended = extended;

                    foreach (var expectedPoint in expected)
                    {
                        var actualPoint = new SpeededUpRobustFeaturePoint(
                            expectedPoint.X,
                            expectedPoint.Y,
                            expectedPoint.Scale,
                            expectedPoint.Laplacian);

                        descriptor.Compute(actualPoint);

                        Assert.AreEqual(expectedPoint.X, actualPoint.X);
                        Assert.AreEqual(expectedPoint.Y, actualPoint.Y);
                        Assert.AreEqual(expectedPoint.Scale, actualPoint.Scale);
                        Assert.AreEqual(expectedPoint.Orientation, actualPoint.Orientation);
                        Assert.AreEqual(expectedPoint.Response, actualPoint.Response);
                        Assert.AreEqual(expectedPoint.Descriptor.Length, actualPoint.Descriptor.Length);

                        for (int i = 0; i < expectedPoint.Descriptor.Length; i++)
                        {
                            double e = expectedPoint.Descriptor[i];
                            double a = actualPoint.Descriptor[i];

                            double u = System.Math.Abs(e - a);
                            double v = System.Math.Abs(e);
                            Assert.AreEqual(e, a, 0.05);
                        }
                    }
                }
            }

        }
    }
}
