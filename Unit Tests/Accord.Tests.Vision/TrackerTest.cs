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

namespace Accord.Tests.Vision
{
    using Accord.Vision.Tracking;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Drawing;
    using AForge.Imaging;

    [TestClass()]
    public class TrackerTest
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
        public void ProcessFrame()
        {
            Bitmap frame = Properties.Resources.lena_color_small;
            Rectangle trackedArea = new Rectangle(0, 0, 50, 50);

            UnmanagedImage uframe = UnmanagedImage.FromManagedImage(frame);

            // initialization
            Camshift target = new Camshift(trackedArea);
            target.Conservative = false;
            target.Mode = CamshiftMode.RGB;
            target.Smooth = false;
            target.AspectRatio = 0;
            target.Extract = true;

            target.ProcessFrame(uframe);
            var to = target.TrackingObject;
            var window = target.SearchWindow;
            Rectangle expected = new Rectangle(0, 0, 50, 50);
            Assert.AreEqual(expected, window);

            // Frame 1 - entire image
            target.ProcessFrame(uframe);

            to = target.TrackingObject;
            window = target.SearchWindow;

            Assert.AreEqual(0.9188701900796201, to.Angle, 0.001);
            Assert.AreEqual((int)47.57993716803177, to.Rectangle.Width, 0.01);
            Assert.AreEqual((int)57.3831709184114, to.Rectangle.Height, 0.05);

            Assert.AreEqual(new Rectangle(0, 0, 52, 62), window);

            Assert.IsNotNull(to.Image);
            Assert.AreEqual(47, to.Image.Width);
            Assert.AreEqual(54, to.Image.Height);


            // Frame 1 - conservative
            target = new Camshift(uframe, trackedArea);
            target.Conservative = true;
            target.ProcessFrame(uframe);
            target.Extract = false;

            to = target.TrackingObject;
            window = target.SearchWindow;

            Assert.AreEqual(0.0, to.Angle);
            Assert.AreEqual((int)57.7234764, to.Rectangle.Width, 0.000001);
            Assert.AreEqual((int)57.7234764, to.Rectangle.Height, 0.000005);
            Assert.AreEqual(25, to.Center.X);
            Assert.AreEqual(25, to.Center.Y);

            Assert.IsNull(to.Image);

            Assert.AreEqual(new Rectangle(0, 0, 63, 63), window);
        }


    }
}
