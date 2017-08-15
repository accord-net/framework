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
    using Accord.Imaging.Filters;
    using AForge;
    using NUnit.Framework;
    using Accord.Tests.Imaging.Properties;
#if NO_BITMAP
    using Resources = Accord.Tests.Imaging.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class BorderFollowingTest
    {

        [Test]
        [Category("MonoNotSupported")]
        public void FindContourTest()
        {
            Bitmap bmp = Accord.Imaging.Image.Clone(Resources.sample_black);

            Bitmap gray = Grayscale.CommonAlgorithms.BT709.Apply(bmp);

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

        [Test]
        public void FindContourTest2()
        {
            Bitmap bmp = Accord.Imaging.Image.Clone(Resources.hand2);

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

        [Test]
        public void FindContourTest3()
        {
            Bitmap image = Accord.Imaging.Image.Clone(Resources.sample_black);
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
