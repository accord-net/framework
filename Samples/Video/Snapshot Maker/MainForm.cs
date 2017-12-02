// Snapshot Maker sample application
// AForge.NET Framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2017
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Accord.Video;
using Accord.Video.DirectShow;

namespace SampleApp
{
    public partial class MainForm : Form
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoDevice;
        private VideoCapabilities[] videoCapabilities;
        private VideoCapabilities[] snapshotCapabilities;

        private SnapshotForm snapshotForm = null;

        public MainForm()
        {
            InitializeComponent();
        }

        // Main form is loaded
        private void MainForm_Load(object sender, EventArgs e)
        {
            // enumerate video devices
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count != 0)
            {
                // add all devices to combo
                foreach (FilterInfo device in videoDevices)
                    devicesCombo.Items.Add(device.Name);
            }
            else
            {
                devicesCombo.Items.Add("No DirectShow devices found");
            }

            devicesCombo.SelectedIndex = 0;

            EnableConnectionControls(true);
        }

        // Closing the main form
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Disconnect();
        }

        // Enable/disable connection related controls
        private void EnableConnectionControls(bool enable)
        {
            devicesCombo.Enabled = enable;
            videoResolutionsCombo.Enabled = enable;
            snapshotResolutionsCombo.Enabled = enable;
            connectButton.Enabled = enable;
            disconnectButton.Enabled = !enable;
            triggerButton.Enabled = (!enable) && (snapshotCapabilities.Length != 0);
        }

        // New video device is selected
        private void devicesCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (videoDevices.Count != 0)
            {
                videoDevice = new VideoCaptureDevice(videoDevices[devicesCombo.SelectedIndex].MonikerString);
                videoDevice.VideoSourceError += VideoDevice_VideoSourceError;
                EnumeratedSupportedFrameSizes(videoDevice);
            }
        }

        private void VideoDevice_VideoSourceError(object sender, VideoSourceErrorEventArgs eventArgs)
        {
            throw eventArgs.Exception;
        }

        // Collect supported video and snapshot sizes
        private void EnumeratedSupportedFrameSizes(VideoCaptureDevice videoDevice)
        {
            this.Cursor = Cursors.WaitCursor;

            videoResolutionsCombo.Items.Clear();
            snapshotResolutionsCombo.Items.Clear();

            try
            {
                videoCapabilities = videoDevice.VideoCapabilities;
                snapshotCapabilities = videoDevice.SnapshotCapabilities;

                foreach (VideoCapabilities capabilty in videoCapabilities)
                {
                    videoResolutionsCombo.Items.Add(string.Format("{0} x {1}",
                        capabilty.FrameSize.Width, capabilty.FrameSize.Height));
                }

                foreach (VideoCapabilities capabilty in snapshotCapabilities)
                {
                    snapshotResolutionsCombo.Items.Add(string.Format("{0} x {1}",
                        capabilty.FrameSize.Width, capabilty.FrameSize.Height));
                }

                if (videoCapabilities.Length == 0)
                {
                    videoResolutionsCombo.Items.Add("Not supported");
                }
                if (snapshotCapabilities.Length == 0)
                {
                    snapshotResolutionsCombo.Items.Add("Not supported");
                }

                videoResolutionsCombo.SelectedIndex = 0;
                snapshotResolutionsCombo.SelectedIndex = 0;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        // On "Connect" button clicked
        private void connectButton_Click(object sender, EventArgs e)
        {
            if (videoDevice != null)
            {
                if ((videoCapabilities != null) && (videoCapabilities.Length != 0))
                {
                    // videoDevice.VideoResolution = videoCapabilities[videoResolutionsCombo.SelectedIndex];
                }

                if ((snapshotCapabilities != null) && (snapshotCapabilities.Length != 0))
                {
                    videoDevice.ProvideSnapshots = true;
                    videoDevice.SnapshotResolution = snapshotCapabilities[snapshotResolutionsCombo.SelectedIndex];
                    videoDevice.SnapshotFrame += new NewFrameEventHandler(videoDevice_SnapshotFrame);
                }

                EnableConnectionControls(false);

                videoSourcePlayer.VideoSource = videoDevice;
                videoSourcePlayer.Start();
            }
        }

        // On "Disconnect" button clicked
        private void disconnectButton_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        // Disconnect from video device
        private void Disconnect()
        {
            if (videoSourcePlayer.VideoSource != null)
            {
                // stop video device
                videoSourcePlayer.SignalToStop();
                videoSourcePlayer.WaitForStop();
                videoSourcePlayer.Stop();
                videoSourcePlayer.VideoSource = null;

                if (videoDevice.ProvideSnapshots)
                    videoDevice.SnapshotFrame -= new NewFrameEventHandler(videoDevice_SnapshotFrame);

                EnableConnectionControls(true);
            }
        }

        // Simulate snapshot trigger
        private void triggerButton_Click(object sender, EventArgs e)
        {
            if ((videoDevice != null) && (videoDevice.ProvideSnapshots))
            {
                videoDevice.SimulateTrigger();
            }
        }

        // New snapshot frame is available
        private void videoDevice_SnapshotFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Console.WriteLine(eventArgs.Frame.Size);

            ShowSnapshot((Bitmap)eventArgs.Frame.Clone());
        }

        private void ShowSnapshot(Bitmap snapshot)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Bitmap>(ShowSnapshot), snapshot);
            }
            else
            {
                if (snapshotForm == null)
                {
                    snapshotForm = new SnapshotForm();
                    snapshotForm.FormClosed += new FormClosedEventHandler(snapshotForm_FormClosed);
                    snapshotForm.Show();
                }

                snapshotForm.SetImage(snapshot);
            }
        }

        private void snapshotForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            snapshotForm = null;
        }

        private void videoSourcePlayer_NewFrameReceived(object sender, NewFrameEventArgs args)
        {
            DateTime now = DateTime.Now;
            Graphics g = Graphics.FromImage(args.Frame);

            // paint current time
            SolidBrush brush = new SolidBrush(Color.Red);
            g.DrawString(now.ToString(), this.Font, brush, new PointF(5, 5));
            brush.Dispose();

            g.Dispose();
        }
    }
}
