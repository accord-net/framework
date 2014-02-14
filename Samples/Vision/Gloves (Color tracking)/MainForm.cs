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

using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Math.Geometry;
using Accord.Vision.Tracking;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Video.VFW;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace GloveTracking
{
    public partial class MainForm : Form
    {
        // opened video source
        private IVideoSource videoSource = null;

        private HslBlobTracker tracker;

        BorderFollowing bf = new BorderFollowing(10);

        KCurvature kcurv = new KCurvature(30, new DoubleRange(0, 45));

        GaussianBlur blur = new GaussianBlur(1.1);

        PointsMarker cmarker = new PointsMarker(Color.White, 1);

        PointsMarker pmarker = new PointsMarker(Color.Green, 8);

        bool showContour;
        bool showRectangle;
        bool showFingertips;
        bool showAngle;

        // Constructor
        public MainForm()
        {
            InitializeComponent();

            tracker = new HslBlobTracker();
            tracker.Filter.Hue = new IntRange(347, 8);
            tracker.Filter.Saturation = new Range(0.353f, 1.0f);
            tracker.Filter.Luminance = new Range(0.125f, 1.0f);
            tracker.MinHeight = 25;
            tracker.MinWidth = 25;
            tracker.MaxHeight = 200;
            tracker.MaxWidth = 200;
            tracker.Extract = true;
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



        // "Open" menu item clieck - open AVI file
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

            videoSource = source;

            this.Cursor = Cursors.Default;
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

            videoSourcePlayer.BorderColor = Color.Black;
            this.Cursor = Cursors.Default;
        }



        // New frame received by the player
        private void videoSourcePlayer_NewFrame(object sender, ref Bitmap image)
        {
            lock (this)
            {
                if (tracker == null)
                    return;

                if (form != null && !form.IsDisposed)
                    form.Image = image;

                BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.ReadWrite, image.PixelFormat);

                UnmanagedImage img = new UnmanagedImage(data);

                tracker.ComputeOrientation = showAngle;

                tracker.ProcessFrame(img);

                Rectangle rect = tracker.TrackingObject.Rectangle;

                UnmanagedImage hand = tracker.TrackingObject.Image;


                if (hand != null && (showContour || showFingertips))
                {
                    UnmanagedImage grayhand = Grayscale.CommonAlgorithms.BT709.Apply(hand);

                    blur.ApplyInPlace(grayhand);

                    List<IntPoint> contour = bf.FindContour(grayhand);

                    for (int i = 0; i < contour.Count; i++)
                        contour[i] += new IntPoint(rect.X, rect.Y);

                    List<IntPoint> peaks = kcurv.FindPeaks(contour);

                    if (showContour)
                    {
                        cmarker.Points = contour;
                        cmarker.ApplyInPlace(img);
                    }

                    if (showFingertips)
                    {
                        pmarker.Points = peaks;
                        pmarker.ApplyInPlace(img);
                    }
                }

                if (showRectangle)
                {
                    RectanglesMarker marker = new RectanglesMarker(rect);
                    marker.ApplyInPlace(img);
                }

                if (showAngle)
                {
                    LineSegment axis = tracker.TrackingObject.GetAxis(AxisOrientation.Vertical);

                    if (axis != null)
                    {
                        // Draw X axis
                        Drawing.Line(img, axis.Start.Round(), axis.End.Round(), Color.Red);
                    }
                }

                image.UnlockBits(data);
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

        private void cbContour_CheckedChanged(object sender, EventArgs e)
        {
            showContour = cbContour.Checked;
            showRectangle = cbContainer.Checked;
            showFingertips = cbFingertips.Checked;
            showAngle = cbAngle.Checked;
        }

        private void tbSensitivity_Scroll(object sender, EventArgs e)
        {
            bf.Threshold = (byte)tbSensitivity.Value;
        }

        HSLFilteringForm form;

        private void button1_Click(object sender, EventArgs e)
        {
            if (form == null || form.IsDisposed)
                form = new HSLFilteringForm();
            form.Filter = tracker.Filter;
            form.Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }

    }
}