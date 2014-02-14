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

    using Accord.Imaging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using AForge.Imaging;
    using AForge;
    using System.Collections.Generic;
    using System.Drawing;
    using Accord.Controls;
    using System.Windows.Forms;
    using Accord.Imaging.Filters;
    using Accord.Tests.Imaging.Properties;
    using Accord.Math;
    using Accord.Controls.Imaging;

    [TestClass()]
    public class SpeededUpRobustFeaturesDescriptorTest
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
        public void ProcessImageTest()
        {
            Bitmap[] bitmaps = 
            {
                Resources.flower01,
                Resources.flower03,
                Resources.flower06,
                Resources.flower07,
                Resources.flower09,
                Resources.flower10,
            };

            foreach (Bitmap img in bitmaps)
            {

                bool upright = true;
                bool extended = false;

                List<SpeededUpRobustFeaturePoint> expected;
                List<SpeededUpRobustFeaturePoint> actual;

                // Create OpenSURF detector by Chris Evans
                {
                    // Create Integral Image
                    OpenSURFcs.IntegralImage iimg = OpenSURFcs.IntegralImage.FromImage(img);

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

                // Create the detector
                var surf = new SpeededUpRobustFeaturesDetector(0.0002f, 5, 2);

                // Extract interest points
                actual = surf.ProcessImage(img);

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
