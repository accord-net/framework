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

namespace Accord.Tests.Vision
{
    using Accord.Vision.Tracking;
    using NUnit.Framework;
    using System.Drawing;
    using Accord.Imaging;
    using Accord.Video;
    using Accord.DataSets;
    using Accord.Video.FFMPEG;
    using System.Drawing.Imaging;
    using Accord.Imaging.Filters;
    using System.IO;
#if NO_BITMAP
    using Resources = Accord.Tests.Vision.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class MatchingTrackerTest
    {

        [Test]
        public void ProcessFrame()
        {
            string basePath = Path.Combine(NUnit.Framework.TestContext.CurrentContext.TestDirectory, "matching-tracker");

            #region doc_track
            // Let's test the tracker using a sample video from 
            // the collection of test videos in the framework:
            TestVideos ds = new TestVideos(basePath);
            string fileName = ds["walking.mp4"];

            // Now, let's open the video using FFMPEG:
            var video = new VideoFileReader();
            video.Open(fileName);

            // And then check the contents of one of the frames:
            Bitmap frame = video.ReadVideoFrame(frameIndex: 150);
            frame.Save(Path.Combine(basePath, "walking_frame.png"));

            // Let's register a template for the bike rider in gray shirt
            Rectangle roi = new Rectangle(x: 70, y: 105, width: 28, height: 54);

            // initialization
            var tracker = new MatchingTracker()
            {
                SearchWindow = roi,
                Threshold = 0.0, // never reset the tracker in case it gets lost
                RegistrationThreshold = 0.95 // re-register the template if we are 95% 
                // confident that the tracked object is indeed the object we want to follow
            };

            // Creating bitmaps and locking them is an expensive 
            // operation. Instead, let's allocate once and reuse
            BitmapData bitmapData = frame.LockBits(ImageLockMode.ReadWrite);
            UnmanagedImage unmanagedImage = new UnmanagedImage(bitmapData);

            // We will create two color markers: one to show the location of the
            // tracked object (red) and another one to show the regions of the image
            // that the tracker is looking at (white).
            RectanglesMarker objectMarker = new RectanglesMarker(Color.Red);
            RectanglesMarker windowMarker = new RectanglesMarker(Color.White);

            // Now, for each frame of the video
            for (int frameIndex = 0; frameIndex < video.FrameCount; frameIndex++)
            {
                // Read the current frame into the bitmap data
                video.ReadVideoFrame(frameIndex, bitmapData);

                if (frameIndex > 150) // wait until the bike rider enters the scene
                {
                    // Feed the frame to the tracker
                    tracker.ProcessFrame(unmanagedImage);

                    // Mark the location of the tracker object in red color
                    objectMarker.SingleRectangle = tracker.TrackingObject.Rectangle;
                    objectMarker.ApplyInPlace(unmanagedImage); // overwrite the frame

                    windowMarker.SingleRectangle = tracker.SearchWindow;
                    windowMarker.ApplyInPlace(unmanagedImage); // overwrite the frame
                }

                // Save it to disk
                frame.Save(Path.Combine(basePath, "frame_{0}.png".Format(frameIndex)));
            }

            frame.UnlockBits(bitmapData);
            #endregion
        }
    }
}
