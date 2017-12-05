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
    using Accord.Imaging;
    using NUnit.Framework;
    using System.Drawing;
    using System.IO;
    using Accord.Math.Geometry;
    using System.Collections.Generic;
    using Accord.Imaging.Filters;
    using Accord.Controls;

    [TestFixture]
    public class BlobCounterTest
    {

        [Test]
        public void blobcounter_test()
        {
            string basePath = Path.Combine(NUnit.Framework.TestContext.CurrentContext.WorkDirectory, "Resources");

            #region doc_process
            // Load an example image containing blobs (such the sample from the Blob Detection sample applications)
            // https://github.com/accord-net/framework/raw/development/Samples/Imaging/Detection%20(Blobs)/demo.png
            Bitmap image = Accord.Imaging.Image.FromFile(Path.Combine(basePath, "blob-input.png"));

            // Creeate a new blob counter object
            var blobCounter = new BlobCounter();

            // Process the image looking for blobs
            blobCounter.ProcessImage(image);

            // Get information about all the image blobs found:
            Blob[] blobs = blobCounter.GetObjectsInformation();
            
            // Prepare to extract their Convex Hull
            var grahamScan = new GrahamConvexHull();
            var colors = new ColorSequenceCollection();

            // For each blob in the image
            for (int i = 0; i < blobs.Length; i++)
            {
                // Get the blob
                Blob blob = blobs[i];

                // Collect edge points
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blob);

                // Find convex hull
                List<IntPoint> hull = grahamScan.FindHull(edgePoints);

                // Prepare to mark the hull in the image
                var marker = new PointsMarker(colors[i])
                {
                    Points = hull,
                    Connect = true // connect the points with line segments
                };

                // Draw the hull lines
                marker.ApplyInPlace(image);
            }

            // Save the image to disk
            image.Save(Path.Combine(basePath, "test.png"));
            #endregion

            Assert.AreEqual(0, blobs.Length);
        }
    }
}
