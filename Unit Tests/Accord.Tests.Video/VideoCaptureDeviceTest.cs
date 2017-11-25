// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmC:\Projects\Accord.NET\framework\Unit Tests\Accord.Tests.Video\BoundaryTest.csail.com
//
// Copyright © Stig Voss, 2017
// http://stigvoss.dk
// stig.voss at gmail.com
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

namespace Accord.Tests.Video
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Accord.Video;
    using System.IO;
    using Accord.Video.DirectShow;
    using System.Drawing;
    using Accord.Imaging.Filters;

    [TestFixture]
    public class VideoCaptureDeviceTest
    {
        [Test, Ignore("This test needs a webcam to run.")]
        public void doc_test()
        {
            #region doc_part_1
            // To use the VideoCaptureClass, the first step is to enumerate the different
            // video capture devices in your system. The output of the method below should
            // include any webcam currently attached to or integrated into your computer:
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            // A possible output will be: 
            // "{ Integrated Webcam(@device:pnp:\\?\usb#vid_0c45&pid_6713&mi_00#6&4851259&0&0000#{65e8773d-8f56-11d0-a3b9-00a0c9223196}\global)}"

            // Now, we can create a video source selecting one of the devices. For this,
            // pass the DirectShow device moniker (a kind of device identifier) of the 
            // device you would like to capture from to VideoCaptureDevice:
            var videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);

            // Then, we just have to define what we would like to do once the device send 
            // us a new frame. This can be done using standard .NET events (the actual 
            // contents of the video_NewFrame method is shown at the bottom of this page)
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);

            // Start the video source. This is not a blocking operation, meaning that 
            // the frame capturing routine will actually be running in a different thread 
            // and execution will be returned to the caller while the capture happens in 
            // the background:
            videoSource.Start();

            // ...

            // Let's say our application continues running, and after a while we decide
            // to stop capturing frames from the device. To stop the device, we can use

            videoSource.SignalToStop();

            // Note that this is again a non-blocking operation. The device will continue
            // capturing for a few milliseconds until it gets notified about our request.
            #endregion
        }

        #region doc_part_2
        // The video_NewFrame used in the example above could be defined as:
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // get new frame
            Bitmap bitmap = eventArgs.Frame;

            // process the frame (you can do anything with it, such as running an
            // image processing filter, saving it to disk, showing on screen, etc)
        }
        #endregion


        #region doc_part_3
        // This is anowher example on how the video_NewFrame could have been 
        // defined. In this example, we will apply a grayscale filter to the
        // images as they arrive from the capture device.

        // When processing multiple frames, it is better to try to cache
        // all operations and memory allocations outside of the main loop:
        Grayscale grayscale = Grayscale.CommonAlgorithms.BT709;

        // The video_NewFrame used in the example above could be defined as:
        private void video_NewFrame2(object sender, NewFrameEventArgs eventArgs)
        {
            // get new frame
            Bitmap bitmap = eventArgs.Frame;

            // Apply the filter we have cached outside:
            Bitmap grayBitmap = grayscale.Apply(bitmap);

            // process the frame (you can do anything with it, such as running an
            // image processing filter, saving it to disk, showing on screen, etc)
        }
        #endregion

        [Test, Ignore("This test needs a webcam to run.")]
        public void dispose_test()
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            var videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);

            videoSource.Start();

            Assert.IsFalse(videoSource.IsDisposed);
            Assert.IsTrue(videoSource.IsRunning);

            videoSource.Dispose();

            Assert.IsTrue(videoSource.IsDisposed);
            Assert.IsFalse(videoSource.IsRunning);

            Assert.Throws<ObjectDisposedException>(() => videoSource.Start());
        }
    }
}
