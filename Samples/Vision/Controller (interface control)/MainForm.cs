// Accord.NET Sample Applications
// http://accord-framework.net
//
// Copyright © 2009-2014, César Souza
// All rights reserved. 3-BSD License:
//
//   Redistribution and use in source and binary forms, with or without
//   modification, are permitted provided that the following conditions are met:
//
//      * Redistributions of source code must retain the above copyright
//        notice, this list of conditions and the following disclaimer.
//
//      * Redistributions in binary form must reproduce the above copyright
//        notice, this list of conditions and the following disclaimer in the
//        documentation and/or other materials provided with the distribution.
//
//      * Neither the name of the Accord.NET Framework authors nor the
//        names of its contributors may be used to endorse or promote products
//        derived from this software without specific prior written permission.
// 
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
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

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }

    }
}
