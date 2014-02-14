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

using Tools = Accord.Imaging.Tools;
using Accord.Controls;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge;
using System.Collections.Generic;
using Accord.Controls.Imaging;
using System;
using Accord.Imaging.Filters;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace Accord.Tests.Imaging
{

    [TestClass()]
    public class BorderFollowingTest
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
        public void FindContourTest()
        {
            Bitmap bmp = Properties.Resources.sample_black;

            Bitmap gray = AForge.Imaging.Filters.Grayscale.CommonAlgorithms.BT709.Apply(bmp);

            BlobCounter bc = new BlobCounter(gray);
            bc.ObjectsOrder = ObjectsOrder.Size;
            Blob[] blobs = bc.GetObjectsInformation();
            bc.ExtractBlobsImage(bmp, blobs[0], true);
            List<IntPoint> expected = bc.GetBlobsEdgePoints(blobs[0]);
            Bitmap blob = blobs[0].Image.ToManagedImage();

            BorderFollowing bf = new BorderFollowing();
            List<IntPoint> actual = bf.FindContour(blob);

            Assert.AreEqual(expected.Count, actual.Count);

            foreach (IntPoint point in expected)
                Assert.IsTrue(actual.Contains(point));

            foreach (IntPoint point in actual)
                Assert.IsTrue(expected.Contains(point));

            IntPoint prev = actual[0];
            for (int i = 1; i < actual.Count; i++)
            {
                IntPoint curr = actual[i];
                Assert.IsTrue(System.Math.Abs(prev.X - curr.X) <= 1 &&
                              System.Math.Abs(prev.Y - curr.Y) <= 1);
                prev = curr;
            }

            IntPoint first = actual[0];
            IntPoint last = actual[actual.Count - 1];
            Assert.IsTrue(System.Math.Abs(first.X - last.X) <= 1 &&
                          System.Math.Abs(first.Y - last.Y) <= 1);
        }

        [TestMethod()]
        public void FindContourTest2()
        {
            Bitmap bmp = Properties.Resources.hand2;

            BlobCounter bc = new BlobCounter(bmp);
            bc.ObjectsOrder = ObjectsOrder.Size;
            Blob[] blobs = bc.GetObjectsInformation();
            bc.ExtractBlobsImage(bmp, blobs[0], true);
            List<IntPoint> expected = bc.GetBlobsEdgePoints(blobs[0]);
            Bitmap blob = blobs[0].Image.ToManagedImage();

            BorderFollowing bf = new BorderFollowing();
            List<IntPoint> actual = bf.FindContour(blob);

            foreach (IntPoint point in expected)
                Assert.IsTrue(actual.Contains(point));

            IntPoint prev = actual[0];
            for (int i = 1; i < actual.Count; i++)
            {
                IntPoint curr = actual[i];
                Assert.IsTrue(System.Math.Abs(prev.X - curr.X) <= 1 &&
                              System.Math.Abs(prev.Y - curr.Y) <= 1);
                prev = curr;
            }

            IntPoint first = actual[0];
            IntPoint last = actual[actual.Count - 1];
            Assert.IsTrue(System.Math.Abs(first.X - last.X) <= 1 &&
                          System.Math.Abs(first.Y - last.Y) <= 1);
        }

        [TestMethod()]
        public void FindContourTest3()
        {
            Bitmap image = Properties.Resources.sample_black;
            Bitmap grayscaleImage = Grayscale.CommonAlgorithms.BT709.Apply(image);

            // Create a new border following algorithm
            BorderFollowing borderFollowing = new BorderFollowing();

            // Get all points in the contour of the image. 
            List<IntPoint> contour = borderFollowing.FindContour(grayscaleImage);

            // Mark all points in the contour point list in blue
            new PointsMarker(contour, Color.Blue).ApplyInPlace(image);

            // Show the result
            // ImageBox.Show(image);

            Assert.AreEqual(380, contour.Count);
        }

    }
}
