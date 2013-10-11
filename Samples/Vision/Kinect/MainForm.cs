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
// Note: this sample application links against the libfreenect library, distributed
// under the Apache 2 License. See libfreenect.txt in this folder for more details.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Accord.Controls.Vision;
using Accord.Imaging.Filters;
using Accord.Math;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Video.Kinect;

namespace KinectController
{
    public partial class MainForm : Form
    {

        private Kinect kinectDevice = null;
        private KinectVideoCamera videoCamera = null;
        private KinectDepthCamera depthCamera = null;

        public VisionForm visionForm;
        public FaceForm faceForm;

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            toolStripStatusLabel1.Text = "Please chose a camera to begin";
        }

        private void headController1_HeadMove(object sender, HeadEventArgs e)
        {
            int x = (int)(e.X * 20f);
            int y = (int)(e.Y * 20f);
            int s = (int)(e.Scale * 20f);

            tbHorizontal.Value = Math.Min(Math.Max(x, tbHorizontal.Minimum), tbHorizontal.Maximum);
            tbVertical.Value = Math.Min(Math.Max(y, tbVertical.Minimum), tbVertical.Maximum);
            tbScale.Value = Math.Min(Math.Max(s, tbScale.Minimum), tbScale.Maximum);
            angleBox1.Angle = e.Angle;
        }

        private void controller_HeadEnter(object sender, HeadEventArgs e)
        {
            toolStripStatusLabel1.Text = "Tracking started";
        }

        private void controller_HeadLeave(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Seaching for faces";
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            // stop current video source
            controller.SignalToStop();

            Properties.Settings.Default.Save();
        }

        private void btnSelectCamera_Click(object sender, EventArgs e)
        {
            VideoCaptureDeviceForm form = new VideoCaptureDeviceForm();

            if (form.ShowDialog() == DialogResult.OK)
            {
                int deviceId = form.DeviceId;

                kinectDevice = Kinect.GetDevice(deviceId);

                if (videoCamera == null)
                {
                    videoCamera = kinectDevice.GetVideoCamera();
                    videoCamera.CameraMode = VideoCameraMode.Color;

                    controller.Device = videoCamera;
                    controller.Start();
                    controller.NewFrame += new AForge.Video.NewFrameEventHandler(controller_NewFrame);
                }

                if (depthCamera == null)
                {
                    //depthCamera = kinectDevice.GetDepthCamera();
                    depthCamera = new KinectDepthCamera(deviceId, CameraResolution.Medium, true);

                    videoSourcePlayer1.VideoSource = depthCamera;
                    videoSourcePlayer1.Start();
                }


                toolStripStatusLabel1.Text = "Initializing...";
            }
        }

        void controller_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            pictureBox2.Image = eventArgs.Frame;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            controller.Reset();
        }



        private void btnScaleMax_Click(object sender, EventArgs e)
        {
            controller.Calibrate(HeadMovement.Forward);
        }

        private void btnScaleMin_Click(object sender, EventArgs e)
        {
            controller.Calibrate(HeadMovement.Backward);
        }

        private void btnHorizontalMin_Click(object sender, EventArgs e)
        {
            controller.Calibrate(HeadMovement.Left);
        }

        private void btnHorizontalMax_Click(object sender, EventArgs e)
        {
            controller.Calibrate(HeadMovement.Right);
        }

        private void btnVerticalMin_Click(object sender, EventArgs e)
        {
            controller.Calibrate(HeadMovement.Down);
        }

        private void btnVerticalMax_Click(object sender, EventArgs e)
        {
            controller.Calibrate(HeadMovement.Up);
        }

        private void btnAngleMin_Click(object sender, EventArgs e)
        {
            controller.Calibrate(HeadMovement.TiltLeft);
        }

        private void btnAngleMax_Click(object sender, EventArgs e)
        {
            controller.Calibrate(HeadMovement.TiltRight);
        }

        private void btnTrackerVision_Click(object sender, EventArgs e)
        {
            if (visionForm == null || visionForm.IsDisposed)
                visionForm = new VisionForm(this);

            visionForm.Show();
        }

        private void btnFaceControls_Click(object sender, EventArgs e)
        {
            if (faceForm == null || faceForm.IsDisposed)
                faceForm = new FaceForm(this);

            faceForm.Show();
        }

        private void videoSourcePlayer1_NewFrame(object sender, ref Bitmap image)
        {
            Invert inv = new Invert();
            inv.ApplyInPlace(image);

            UnmanagedImage ui = UnmanagedImage.FromManagedImage(image);

            pictureBox1.Image = image;


            if (controller.Tracker.TrackingObject == null)
                return;

            if (controller.Tracker.TrackingObject.IsEmpty)
                return;

            var rect = controller.Tracker.TrackingObject.Rectangle;
            Crop crop = new Crop(rect);

            UnmanagedImage head = crop.Apply(ui);

            var points = new List<IntPoint>() { new IntPoint(head.Width / 2, head.Height / 2) };
            var pps = head.Collect16bppPixelValues(points);

            double mean = Accord.Statistics.Tools.Mean(pps);

            double cutoff = mean + 15;
            Threshold t = new Threshold((int)cutoff);
            var mask = t.Apply(ui);



            LevelsLinear16bpp levels = new LevelsLinear16bpp();
            levels.InGray = new IntRange((int)cutoff, 65535);
            levels.OutGray = new IntRange(0, 65535);
            levels.ApplyInPlace(ui);


            var mask8bit = AForge.Imaging.Image.Convert16bppTo8bpp(mask.ToManagedImage());



            BlobCounter bc = new BlobCounter();
            bc.ObjectsOrder = ObjectsOrder.Area;
            bc.ProcessImage(mask8bit);
            var blobs = bc.GetObjectsInformation();

            inv.ApplyInPlace(image);
            Intersect intersect = new Intersect();
            intersect.UnmanagedOverlayImage = mask;
            mask = intersect.Apply(ui);

            List<Rectangle> rects = new List<Rectangle>();

            // Extract the uppermost largest blobs.
            for (int i = 0; i < blobs.Length; i++)
            {
                double dx = (blobs[i].Rectangle.Top - controller.Tracker.TrackingObject.Center.Y);
                double d = (dx * dx) / controller.Tracker.TrackingObject.Area;
                if (d < 2 && blobs[i].Area > 1000)
                    rects.Add(blobs[i].Rectangle);
            }

            rects.Sort(compare);

            if (rects.Count > 0)
            {
                captureHand(mask, rects[0], pbLeftArm, pbLeftHand);
            }
            if (rects.Count > 1)
            {
                captureHand(mask, rects[1], pbRightArm, pbRightHand);

            }

            RectanglesMarker marker = new RectanglesMarker(rects);
            marker.MarkerColor = Color.White;
            marker.ApplyInPlace(mask8bit);

            image = mask.ToManagedImage();
        }

        private void captureHand(UnmanagedImage mask, Rectangle rect, PictureBox pbArm, PictureBox pbHand)
        {
            Crop c = new Crop(rect);
            var handImage = c.Apply(mask);

            var ps = handImage.Collect16bppPixelValues(handImage.CollectActivePixels());

            if (ps.Length > 0)
            {
                ushort max = Matrix.Max(ps);

                LevelsLinear16bpp levels = new LevelsLinear16bpp();
                levels.InGray = new IntRange(0, max);
                levels.OutGray = new IntRange(0, 65535);
                levels.ApplyInPlace(handImage);


               // pbArm.Image = handImage.ToManagedImage();


                double cutoff = 30000;
                Threshold th = new Threshold((int)cutoff);
                var handMask = th.Apply(handImage);

                var handMask8bit = AForge.Imaging.Image.Convert16bppTo8bpp(handMask.ToManagedImage());

                BlobCounter bch = new BlobCounter();
                bch.ObjectsOrder = ObjectsOrder.Area;
                bch.ProcessImage(handMask8bit);
                var blob = bch.GetObjectsInformation();

                if (blob.Length > 0)
                {
                    Intersect inters = new Intersect();
                    inters.UnmanagedOverlayImage = handMask;
                    inters.ApplyInPlace(handImage);

                    Crop ch = new Crop(blob[0].Rectangle);
                    handImage = ch.Apply(handImage);

                    ResizeNearestNeighbor res = new ResizeNearestNeighbor(25, 25);
                    handImage = res.Apply(handImage);

                    var leftHand = AForge.Imaging.Image.Convert16bppTo8bpp(handImage.ToManagedImage());

                    pbHand.Image = leftHand;
                }
            }
        }

        private static int compare(Rectangle a, Rectangle b)
        {
            return b.X.CompareTo(a.X);
        }



        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }

    }
}
