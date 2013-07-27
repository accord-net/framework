// Accord.NET Sample Applications
// http://accord.googlecode.com
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

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Accord.Controls.Vision;
using AForge.Video.DirectShow;

namespace Controller
{
    public partial class MainForm : Form
    {

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
                VideoCaptureDevice device = new VideoCaptureDevice(form.VideoDevice);

                device.VideoResolution = selectResolution(device);

                controller.Device = device;
                controller.Start();

                toolStripStatusLabel1.Text = "Initializing...";
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

    }
}
