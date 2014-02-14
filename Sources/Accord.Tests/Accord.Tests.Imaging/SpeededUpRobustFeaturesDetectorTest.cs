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
    public class SpeededUpRobustFeaturesDetectorTest
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
            // Load an Image
            Bitmap img = Properties.Resources.sample_trans;

            // Extract the interest points
            var surf = new SpeededUpRobustFeaturesDetector(0.0002f, 5, 2);

            surf.ComputeDescriptors = SpeededUpRobustFeatureDescriptorType.None;

            List<SpeededUpRobustFeaturePoint> points = surf.ProcessImage(img);

            // Describe the interest points
            SpeededUpRobustFeaturesDescriptor descriptor = surf.GetDescriptor();
            descriptor.Compute(points);

            Assert.AreEqual(8, points.Count);

            SpeededUpRobustFeaturePoint p;

            p = points[0];
            Assert.AreEqual(0, p.Laplacian);
            Assert.AreEqual(25.3803387, p.X, 1e-2);
            Assert.AreEqual(14.7987738, p.Y, 1e-2);
            Assert.AreEqual(1.98713827, p.Scale, 1e-2);
            Assert.AreEqual(0.0, p.Response, 1e-2);
            Assert.AreEqual(4.78528404, p.Orientation, 1e-2);

            p = points[1];
            Assert.AreEqual(1, p.Laplacian, 1e-2);
            Assert.AreEqual(20.4856224, p.X, 1e-2);
            Assert.AreEqual(20.4817181, p.Y, 1e-2);
            Assert.AreEqual(1.90549147, p.Scale, 1e-2);
            Assert.AreEqual(0.0, p.Response, 1e-2);
            Assert.AreEqual(4.89748764, p.Orientation, 1e-2);

            p = points[2];
            Assert.AreEqual(0, p.Laplacian, 1e-2);
            Assert.AreEqual(14.7991896, p.X, 1e-2);
            Assert.AreEqual(25.3776169, p.Y, 1e-2);
            Assert.AreEqual(1.9869982, p.Scale, 1e-2);
            Assert.AreEqual(0.0, p.Response, 1e-2);
            Assert.AreEqual(3.07735944, p.Orientation, 1e-2);

            p = points[6];
            Assert.AreEqual(1, p.Laplacian, 1e-2);
            Assert.AreEqual(22.4346638, p.X, 1e-2);
            Assert.AreEqual(41.4026527, p.Y, 1e-2);
            Assert.AreEqual(2.83586049, p.Scale, 1e-2);
            Assert.AreEqual(0.0, p.Response, 1e-2);
            Assert.AreEqual(3.13142157, p.Orientation, 1e-2);


            descriptor.Extended = true;
            descriptor.Invariant = false;
            descriptor.Compute(points);

            p = points[5];
            Assert.AreEqual(1, p.Laplacian, 1e-3);
            Assert.AreEqual(41.4027748, p.X, 1e-3);
            Assert.AreEqual(22.4343891, p.Y, 1e-3);
            Assert.AreEqual(2.83486962, p.Scale, 1e-3);
            Assert.AreEqual(0.0, p.Response, 1e-3);
            Assert.AreEqual(4.72728586, p.Orientation, 1e-3);
        }

        [TestMethod()]
        public void ProcessImageTest2()
        {
            // Load an Image
            Bitmap img = Properties.Resources.sample_trans;

            // Extract the interest points
            var surf = new SpeededUpRobustFeaturesDetector(0.0002f, 5, 2);

            surf.ComputeDescriptors = SpeededUpRobustFeatureDescriptorType.None;
            surf.ComputeOrientation = true;

            List<SpeededUpRobustFeaturePoint> points = surf.ProcessImage(img);

            Assert.AreEqual(8, points.Count);

            SpeededUpRobustFeaturePoint p;

            p = points[0];
            Assert.AreEqual(0, p.Laplacian);
            Assert.AreEqual(25.3803387, p.X, 1e-2);
            Assert.AreEqual(14.7987738, p.Y, 1e-2);
            Assert.AreEqual(1.98713827, p.Scale, 1e-2);
            Assert.AreEqual(0.0, p.Response, 1e-2);
            Assert.AreEqual(4.78528404, p.Orientation, 1e-2);
            Assert.IsNull(p.Descriptor);

            p = points[1];
            Assert.AreEqual(1, p.Laplacian, 1e-2);
            Assert.AreEqual(20.4856224, p.X, 1e-2);
            Assert.AreEqual(20.4817181, p.Y, 1e-2);
            Assert.AreEqual(1.90549147, p.Scale, 1e-2);
            Assert.AreEqual(0.0, p.Response, 1e-2);
            Assert.AreEqual(4.89748764, p.Orientation, 1e-2);
            Assert.IsNull(p.Descriptor);

            p = points[2];
            Assert.AreEqual(0, p.Laplacian, 1e-2);
            Assert.AreEqual(14.7991896, p.X, 1e-2);
            Assert.AreEqual(25.3776169, p.Y, 1e-2);
            Assert.AreEqual(1.9869982, p.Scale, 1e-2);
            Assert.AreEqual(0.0, p.Response, 1e-2);
            Assert.AreEqual(3.07735944, p.Orientation, 1e-2);
            Assert.IsNull(p.Descriptor);

            p = points[6];
            Assert.AreEqual(1, p.Laplacian, 1e-2);
            Assert.AreEqual(22.4346638, p.X, 1e-2);
            Assert.AreEqual(41.4026527, p.Y, 1e-2);
            Assert.AreEqual(2.83586049, p.Scale, 1e-2);
            Assert.AreEqual(0.0, p.Response, 1e-2);
            Assert.AreEqual(3.13142157, p.Orientation, 1e-2);
            Assert.IsNull(p.Descriptor);
        }


        [TestMethod()]
        public void ProcessImageTest3()
        {
            // Load an Image
            Bitmap img = Properties.Resources.sample_trans;

            // Extract the interest points
            var surf = new SpeededUpRobustFeaturesDetector(0.0002f, 5, 2);

            surf.ComputeDescriptors = SpeededUpRobustFeatureDescriptorType.Extended;
            surf.ComputeOrientation = false;

            List<SpeededUpRobustFeaturePoint> points = surf.ProcessImage(img);

            Assert.AreEqual(8, points.Count);

            SpeededUpRobustFeaturePoint p;

            p = points[0];
            Assert.AreEqual(0, p.Laplacian);
            Assert.AreEqual(25.3803387, p.X, 1e-2);
            Assert.AreEqual(14.7987738, p.Y, 1e-2);
            Assert.AreEqual(1.98713827, p.Scale, 1e-2);
            Assert.AreEqual(0.0, p.Response, 1e-2);
            Assert.AreEqual(0.0, p.Orientation, 1e-2);
            Assert.AreEqual(128, p.Descriptor.Length);
            Assert.AreEqual(0.026527178478172982, p.Descriptor[23], 1e-10);
            Assert.AreEqual(0.28221266818142571, p.Descriptor[42], 1e-10);

            p = points[1];
            Assert.AreEqual(1, p.Laplacian, 1e-2);
            Assert.AreEqual(20.4856224, p.X, 1e-2);
            Assert.AreEqual(20.4817181, p.Y, 1e-2);
            Assert.AreEqual(1.90549147, p.Scale, 1e-2);
            Assert.AreEqual(0.0, p.Response, 1e-2);
            Assert.AreEqual(0, p.Orientation, 1e-2);
            Assert.AreEqual(128, p.Descriptor.Length);
            Assert.AreEqual(0.0017332996868934826, p.Descriptor[23], 1e-10);
            Assert.AreEqual(0.01141609085546454, p.Descriptor[54], 1e-10);

            p = points[2];
            Assert.AreEqual(0, p.Laplacian, 1e-2);
            Assert.AreEqual(14.7991896, p.X, 1e-2);
            Assert.AreEqual(25.3776169, p.Y, 1e-2);
            Assert.AreEqual(1.9869982, p.Scale, 1e-2);
            Assert.AreEqual(0.0, p.Response, 1e-2);
            Assert.AreEqual(0.0, p.Orientation, 1e-2);
            Assert.AreEqual(128, p.Descriptor.Length);
            Assert.AreEqual(0.013397918304161798, p.Descriptor[23], 1e-10);
            Assert.AreEqual(0.0000054080612707747483, p.Descriptor[12], 1e-10);

            p = points[6];
            Assert.AreEqual(1, p.Laplacian, 1e-2);
            Assert.AreEqual(22.4346638, p.X, 1e-2);
            Assert.AreEqual(41.4026527, p.Y, 1e-2);
            Assert.AreEqual(2.83586049, p.Scale, 1e-2);
            Assert.AreEqual(0.0, p.Response, 1e-2);
            Assert.AreEqual(0.0, p.Orientation, 1e-2);
            Assert.AreEqual(128, p.Descriptor.Length);
            Assert.AreEqual(0.059789486280406236, p.Descriptor[23], 1e-10);
            Assert.AreEqual(-0.0000056629312093282088, p.Descriptor[12], 1e-10);
        }


        [Ignore]
        [TestMethod]
        public void ProcessImageTest4()
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

            var surf = new SpeededUpRobustFeaturesDetector();

            int current = 0;
            foreach (Bitmap img in bitmaps)
            {
                List<SpeededUpRobustFeaturePoint> expected;
                List<SpeededUpRobustFeaturePoint> actual;

                // Create OpenSURF detector by Chris Evans
                {
                    // Create Integral Image
                    OpenSURFcs.IntegralImage iimg = OpenSURFcs.IntegralImage.FromImage(img);

                    // Extract the interest points
                    var pts = OpenSURFcs.FastHessian.getIpoints(0.0002f, 5, 2, iimg);

                    // Describe the interest points
                    OpenSURFcs.SurfDescriptor.DecribeInterestPoints(pts, false, false, iimg);

                    expected = new List<SpeededUpRobustFeaturePoint>();
                    foreach (var p in pts)
                        expected.Add(new SpeededUpRobustFeaturePoint(p.x, p.y, p.scale,
                            p.laplacian, p.orientation, p.response));
                }

                // Create Accord.NET SURF detector (based on OpenSURF by Chris Evans)
                {
                    actual = surf.ProcessImage(img);
                }

                var img1 = new FeaturesMarker(actual).Apply(img);
                var img2 = new FeaturesMarker(expected).Apply(img);

                ImageBox.Show(new Concatenate(img1).Apply(img2), PictureBoxSizeMode.Zoom);


                current++;
            }
        }

    }
}
