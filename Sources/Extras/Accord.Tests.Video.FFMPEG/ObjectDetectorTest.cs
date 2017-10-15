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
    using Accord.Vision;
    using NUnit.Framework;
    using System.Drawing;
    using Accord.Vision.Detection;
    using Accord.Vision.Detection.Cascades;
    using Accord.DataSets;
    using Accord.Imaging.Filters;
    using Accord.Video.FFMPEG;
    using System.Drawing.Imaging;
    using Accord.Imaging;
    using System.IO;
#if NO_BITMAP
    using Resources = Accord.Tests.Vision.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class ObjectDetectorTest
    {

        [Test]
        public void ProcessVideo()
        {
            string basePath = Path.Combine(NUnit.Framework.TestContext.CurrentContext.TestDirectory, "detector");

            #region doc_video
            // Let's test the detector using a sample video from 
            // the collection of test videos in the framework:
            TestVideos ds = new TestVideos(basePath);
            string fileName = ds["crowd.mp4"];

            // In this example, we will be creating a cascade for a Face detector:
            var cascade = new Accord.Vision.Detection.Cascades.FaceHaarCascade();

            // Now, create a new Haar object detector with the cascade:
            var detector = new HaarObjectDetector(cascade, minSize: 25,
                searchMode: ObjectDetectorSearchMode.Average,
                scalingMode: ObjectDetectorScalingMode.SmallerToGreater,
                scaleFactor: 1.1f)
            {
                Suppression = 5 // This should make sure we only report regions as faces if 
                // they have been detected at least 5 times within different cascade scales.
            };

            // Now, let's open the video using FFMPEG:
            var video = new VideoFileReader();
            video.Open(fileName);

            // And then check the contents of one of the frames:
            Bitmap frame = video.ReadVideoFrame(frameIndex: 0);

            // Creating bitmaps and locking them is an expensive 
            // operation. Instead, let's allocate once and reuse
            BitmapData bitmapData = frame.LockBits(ImageLockMode.ReadWrite);
            UnmanagedImage unmanagedImage = new UnmanagedImage(bitmapData);

            // We will create a color marker to show the faces 
            var objectMarker = new RectanglesMarker(Color.Red);

            // This example is going to show two different ways to save results to disk. The 
            // first is to save the results frame-by-frame, saving each individual frame as 
            // a separate .png file. The second is to save them as a video in .mp4 format.

            // To save results as a movie clip in mp4 format, you can use:
            VideoFileWriter writer = new VideoFileWriter();
            writer.Open(Path.Combine(basePath, "detected_faces.mp4"), frame.Width, frame.Height);

            // Now, for each frame of the video
            for (int frameIndex = 0; frameIndex < video.FrameCount; frameIndex++)
            {
                // Read the current frame into the bitmap data
                video.ReadVideoFrame(frameIndex, bitmapData);

                // Feed the frame to the tracker
                Rectangle[] faces = detector.ProcessFrame(unmanagedImage);

                // Mark the location of the tracker object in red color
                objectMarker.Rectangles = faces;
                objectMarker.ApplyInPlace(unmanagedImage); // overwrite the frame

                // Save it to disk: first saving each frame separately:
                frame.Save(Path.Combine(basePath, "frame_{0}.png".Format(frameIndex)));

                // And then, saving as a .mp4 file:
                writer.WriteVideoFrame(bitmapData);
            }

            // The generated video can be seen at https://1drv.ms/v/s!AoiTwBxoR4OAoLJhPozzixD25XcbiQ
            video.Close();
            writer.Close();
            #endregion
        }
    }
}
