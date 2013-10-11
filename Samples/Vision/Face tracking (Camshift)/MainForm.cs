// Accord.NET Sample Applications
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2013
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
// Based on code from the:
//   Motion Detection sample application
//   AForge.NET Framework
//   http://www.aforgenet.com/framework/
//

using Accord.Imaging.Filters;
using Accord.Vision.Detection;
using Accord.Vision.Detection.Cascades;
using Accord.Vision.Tracking;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Video.VFW;
using MotionDetectorSample;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace FaceTracking
{
    public partial class MainForm : Form
    {
        // opened video source
        private IVideoSource videoSource = null;

        // object detector
        HaarObjectDetector detector;

        // object tracker
        Camshift tracker = null;

        // window marker
        RectanglesMarker marker;


        private bool detecting = false;
        private bool tracking = false;

        // statistics length
        private const int statLength = 15;
        // current statistics index
        private int statIndex = 0;
        // ready statistics values
        private int statReady = 0;
        // statistics array
        private int[] statCount = new int[statLength];


        // Constructor
        public MainForm()
        {
            InitializeComponent();

            HaarCascade cascade = new FaceHaarCascade();
            detector = new HaarObjectDetector(cascade,
                25, ObjectDetectorSearchMode.Single, 1.2f,
                ObjectDetectorScalingMode.GreaterToSmaller);
        }

        // Application's main form is closing
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseVideoSource();
        }

        // "Exit" menu item clicked
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        // "Open" menu item click - open AVI file
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // create video source
                AVIFileVideoSource fileSource = new AVIFileVideoSource(openFileDialog.FileName);

                OpenVideoSource(fileSource);
            }
        }

        // Open JPEG URL
        private void openJPEGURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            URLForm form = new URLForm();

            form.Description = "Enter URL of an updating JPEG from a web camera:";
            form.URLs = new string[]
                {
                    "http://195.243.185.195/axis-cgi/jpg/image.cgi?camera=1"
                };

            if (form.ShowDialog(this) == DialogResult.OK)
            {
                // create video source
                JPEGStream jpegSource = new JPEGStream(form.URL);

                // open it
                OpenVideoSource(jpegSource);
            }
        }

        // Open MJPEG URL
        private void openMJPEGURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            URLForm form = new URLForm();

            form.Description = "Enter URL of an MJPEG video stream:";
            form.URLs = new string[]
                {
                    "http://195.243.185.195/axis-cgi/mjpg/video.cgi?camera=3",
                    "http://195.243.185.195/axis-cgi/mjpg/video.cgi?camera=4",
                };

            if (form.ShowDialog(this) == DialogResult.OK)
            {
                // create video source
                MJPEGStream mjpegSource = new MJPEGStream(form.URL);

                // open it
                OpenVideoSource(mjpegSource);
            }
        }

        // Open local video capture device
        private void localVideoCaptureDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VideoCaptureDeviceForm form = new VideoCaptureDeviceForm();

            if (form.ShowDialog(this) == DialogResult.OK)
            {
                // create video source
                VideoCaptureDevice videoSource = new VideoCaptureDevice(form.VideoDevice);

                // set frame size
                videoSource.VideoResolution = selectResolution(videoSource);

                // open it
                OpenVideoSource(videoSource);
            }
        }

        private static VideoCapabilities selectResolution(VideoCaptureDevice device)
        {
            foreach (var cap in device.VideoCapabilities)
            {
                if (cap.FrameSize.Height == 240)
                    return cap;
                if (cap.FrameSize.Width == 320)
                    return cap;
            }

            return device.VideoCapabilities.Last();
        }

        // Open video file using DirectShow
        private void openVideoFileusingDirectShowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // create video source
                FileVideoSource fileSource = new FileVideoSource(openFileDialog.FileName);

                // open it
                OpenVideoSource(fileSource);
            }

        }

        // Open video source
        private void OpenVideoSource(IVideoSource source)
        {
            // set busy cursor
            this.Cursor = Cursors.WaitCursor;

            // close previous video source
            CloseVideoSource();

            // start new video source
            videoSourcePlayer.VideoSource = source;
            videoSourcePlayer.Start();

            // reset statistics
            statIndex = statReady = 0;

            // start timers
            timer.Start();

            videoSource = source;

            this.Cursor = Cursors.Default;

            objectsCountLabel.Text = "Double-click the screen to find faces!";
        }

        // Close current video source
        private void CloseVideoSource()
        {
            // set busy cursor
            this.Cursor = Cursors.WaitCursor;

            // stop current video source
            videoSourcePlayer.SignalToStop();

            // wait 2 seconds until camera stops
            for (int i = 0; (i < 50) && (videoSourcePlayer.IsRunning); i++)
            {
                Thread.Sleep(100);
            }
            if (videoSourcePlayer.IsRunning)
                videoSourcePlayer.Stop();

            // stop timers
            timer.Stop();

            // reset motion detector
            tracker = new Camshift();
            tracker.Mode = hSLToolStripMenuItem.Checked ? CamshiftMode.HSL :
                           mixedToolStripMenuItem.Checked ? CamshiftMode.Mixed :
                           CamshiftMode.RGB;
            tracker.Conservative = true;
            tracker.AspectRatio = 1.5f;

            videoSourcePlayer.BorderColor = Color.Black;
            this.Cursor = Cursors.Default;
        }


        // New frame received by the player
        private void videoSourcePlayer_NewFrame(object sender, ref Bitmap image)
        {
            if (!detecting && !tracking)
                return;

            lock (this)
            {
                if (detecting)
                {
                    detecting = false;
                    tracking = false;

                    UnmanagedImage im = UnmanagedImage.FromManagedImage(image);

                    float xscale = image.Width / 160f;
                    float yscale = image.Height / 120f;

                    ResizeNearestNeighbor resize = new ResizeNearestNeighbor(160, 120);
                    UnmanagedImage downsample = resize.Apply(im);

                    Rectangle[] regions = detector.ProcessFrame(downsample);

                    if (regions.Length > 0)
                    {
                        tracker.Reset();

                        // Will track the first face found
                        Rectangle face = regions[0];

                        // Reduce the face size to avoid tracking background
                        Rectangle window = new Rectangle(
                            (int)((regions[0].X + regions[0].Width / 2f) * xscale),
                            (int)((regions[0].Y + regions[0].Height / 2f) * yscale),
                            1, 1);

                        window.Inflate(
                            (int)(0.2f * regions[0].Width * xscale),
                            (int)(0.4f * regions[0].Height * yscale));

                        // Initialize tracker
                        tracker.SearchWindow = window;
                        tracker.ProcessFrame(im);

                        marker = new RectanglesMarker(window);
                        marker.ApplyInPlace(im);

                        image = im.ToManagedImage();

                        tracking = true;
                        //detecting = true;
                    }
                    else
                    {
                        detecting = true;
                    }
                }
                else if (tracking)
                {
                    UnmanagedImage im = UnmanagedImage.FromManagedImage(image);

                    // Track the object
                    tracker.ProcessFrame(im);

                    // Get the object position
                    var obj = tracker.TrackingObject;
                    var wnd = tracker.SearchWindow;

                    if (displayBackprojectionToolStripMenuItem.Checked)
                    {
                        var backprojection = tracker.GetBackprojection(PixelFormat.Format24bppRgb);
                        im = UnmanagedImage.FromManagedImage(backprojection);
                    }

                    if (drawObjectAxisToolStripMenuItem.Checked)
                    {
                        LineSegment axis = obj.GetAxis();

                        // Draw X axis
                        Drawing.Line(im, axis.Start.Round(), axis.End.Round(), Color.Red);
                    }


                    if (drawObjectBoxToolStripMenuItem.Checked && drawTrackingWindowToolStripMenuItem.Checked)
                    {
                        marker = new RectanglesMarker(new Rectangle[] { wnd, obj.Rectangle });
                    }
                    else if (drawObjectBoxToolStripMenuItem.Checked)
                    {
                        //InteractionPoints p = new InteractionPoints();
                        //p.setHead(obj.Rectangle);

                        marker = new RectanglesMarker(obj.Rectangle);
                    }
                    else if (drawTrackingWindowToolStripMenuItem.Checked)
                    {
                        marker = new RectanglesMarker(wnd);
                    }
                    else
                    {
                        marker = null;
                    }


                    if (marker != null)
                        marker.ApplyInPlace(im);
                    image = im.ToManagedImage();
                }
                else
                {
                    if (marker != null)
                        image = marker.Apply(image);
                }

            }
        }



        // On timer event - gather statistics
        private void timer_Tick(object sender, EventArgs e)
        {
            IVideoSource videoSource = videoSourcePlayer.VideoSource;

            if (videoSource != null)
            {
                // get number of frames for the last second
                statCount[statIndex] = videoSource.FramesReceived;

                // increment indexes
                if (++statIndex >= statLength)
                    statIndex = 0;
                if (statReady < statLength)
                    statReady++;

                float fps = 0;

                // calculate average value
                for (int i = 0; i < statReady; i++)
                {
                    fps += statCount[i];
                }
                fps /= statReady;

                statCount[statIndex] = 0;

                fpsLabel.Text = fps.ToString("F2") + " fps";
            }
        }





        // On opening of Tools menu
        private void toolsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            localVideoCaptureSettingsToolStripMenuItem.Enabled =
                ((videoSource != null) && (videoSource is VideoCaptureDevice));
        }

        // Display properties of local capture device
        private void localVideoCaptureSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((videoSource != null) && (videoSource is VideoCaptureDevice))
            {
                try
                {
                    ((VideoCaptureDevice)videoSource).DisplayPropertyPage(this.Handle);
                }
                catch (NotSupportedException)
                {
                    MessageBox.Show("The video source does not support configuration property page.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        private void videoSourcePlayer_Click(object sender, EventArgs e)
        {
            detecting = true;
        }

        private void drawTrackingWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //showTrackingWindow = !showTrackingWindow;
            drawTrackingWindowToolStripMenuItem.Checked = !drawTrackingWindowToolStripMenuItem.Checked;
        }

        private void drawObjectAxisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //showTrackingAxis = !showTrackingAxis;
            drawObjectAxisToolStripMenuItem.Checked = !drawObjectAxisToolStripMenuItem.Checked;
        }

        private void drawObjectBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //showTrackingBox = !showTrackingBox;
            drawObjectBoxToolStripMenuItem.Checked = !drawObjectBoxToolStripMenuItem.Checked;
        }

        private void displayBackprojectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //showBackprojecton = !showBackprojecton;
            displayBackprojectionToolStripMenuItem.Checked = !displayBackprojectionToolStripMenuItem.Checked;
        }

        private void defineObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (videoSourcePlayer.VideoSource != null)
            {
                Bitmap currentVideoFrame = videoSourcePlayer.GetCurrentVideoFrame();

                if (currentVideoFrame != null)
                {
                    MotionRegionsForm form = new MotionRegionsForm();
                    form.VideoFrame = currentVideoFrame;

                    // show the dialog
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        Rectangle[] rects = form.ObjectRectangles;

                        if (rects.Length == 0)
                            rects = null;

                        tracker.Reset();
                        tracker.SearchWindow = rects[0];
                        detecting = false;
                        tracking = true;
                    }

                    return;
                }
            }
        }

        private void autodetectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            detecting = true;
            objectsCountLabel.Text = String.Empty;
        }

        private void rGBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tracker != null)
            {
                tracking = false;
                tracker.Mode = CamshiftMode.RGB;
                tracker.Reset();
                marker = null;
            }
            rGBToolStripMenuItem.Checked = true;
            hSLToolStripMenuItem.Checked = false;
            mixedToolStripMenuItem.Checked = false;
        }

        private void hSLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tracker != null)
            {
                tracking = false;
                tracker.Mode = CamshiftMode.HSL;
                tracker.Reset();
                marker = null;
            }
            rGBToolStripMenuItem.Checked = false;
            hSLToolStripMenuItem.Checked = true;
            mixedToolStripMenuItem.Checked = false;
        }

        private void mixedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tracker != null)
            {
                tracking = false;
                tracker.Mode = CamshiftMode.Mixed;
                tracker.Reset();
                marker = null;
            }
            rGBToolStripMenuItem.Checked = false;
            hSLToolStripMenuItem.Checked = false;
            mixedToolStripMenuItem.Checked = true;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }
    }
}