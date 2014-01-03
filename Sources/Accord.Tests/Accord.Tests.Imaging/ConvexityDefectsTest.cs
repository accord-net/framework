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

using Accord.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accord.Math;
using System.Drawing;
using Accord.Controls;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge;
using System.Collections.Generic;
using Accord.Controls.Imaging;
using System;
using Accord.Imaging.Filters;
using AForge.Imaging;
using AForge.Math.Geometry;
using Accord.Math.Geometry;
//using Accord.Math.Geometry;

namespace Accord.Tests.Imaging
{


    /// <summary>
    ///This is a test class for ToolsTest and is intended
    ///to contain all ToolsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConvexityDefectsTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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
        public void FindDefectsTest()
        {

            Bitmap bmp = Properties.Resources.hand;

            Bitmap gray = AForge.Imaging.Filters.Grayscale.CommonAlgorithms.BT709.Apply(bmp);

            BlobCounter bc = new BlobCounter(gray);
            bc.ObjectsOrder = ObjectsOrder.Size;
            Blob[] blobs = bc.GetObjectsInformation();
            bc.ExtractBlobsImage(bmp, blobs[0], true);

            Bitmap blob = blobs[0].Image.ToManagedImage();

            BorderFollowing bf = new BorderFollowing();
            List<IntPoint> contour = bf.FindContour(blob);

            GrahamConvexHull graham = new GrahamConvexHull();
            List<IntPoint> hull = graham.FindHull(contour);

            ConvexHullDefects hullDefects = new ConvexHullDefects(10);
            List<ConvexityDefect> defects = hullDefects.FindDefects(contour, hull);

          /*  PointsMarker marker = new PointsMarker(hull, Color.Green, 10);
            marker.ApplyInPlace(blob);
            ImageBox.Show(blob);
            */

            Assert.AreEqual(2, defects.Count);
            Assert.AreEqual(new IntPoint(130, 10), contour[defects[0].Start]);
            Assert.AreEqual(new IntPoint(93, 109), contour[defects[0].Point]);
            Assert.AreEqual(new IntPoint(64, 9), contour[defects[0].End]);
            Assert.AreEqual(99.549179077148438, defects[0].Depth, 1e-5);
            Assert.IsFalse(double.IsNaN(defects[0].Depth));
            //    Assert.AreEqual(9912.9531239366424, defects[0].Area);

            Assert.AreEqual(new IntPoint(49, 18), contour[defects[1].Start]);
            Assert.AreEqual(new IntPoint(61, 106), contour[defects[1].Point]);
            Assert.AreEqual(new IntPoint(18, 127), contour[defects[1].End]);
            Assert.AreEqual(35.615153852366504, defects[1].Depth, 1e-5);
            Assert.IsFalse(double.IsNaN(defects[1].Depth));
            //    Assert.AreEqual(2293.7535682510002, defects[1].Area);

        }


    }
}
