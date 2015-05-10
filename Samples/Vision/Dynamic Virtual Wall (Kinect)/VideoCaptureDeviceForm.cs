using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Video.Kinect;

namespace KinectController
{
    public partial class VideoCaptureDeviceForm : Form
    {
        private int device;

        // Video device
        public int DeviceId
        {
            get { return device; }
        }

        // Constructor
        public VideoCaptureDeviceForm( )
        {
            InitializeComponent( );

            if (Kinect.DeviceCount == 0)
            {
                devicesCombo.Items.Add("No Kinect devices");
            }
            else
            {
                for (int i = 0; i < Kinect.DeviceCount; i++)
                {
                    devicesCombo.Items.Add("Device " + i);
                }
            }
            devicesCombo.SelectedIndex = 0;
            //videoModeCombo.SelectedIndex = 0;

            devicesCombo.SelectedIndex = 0;
        }

        // Ok button clicked
        private void okButton_Click( object sender, EventArgs e )
        {
            device = devicesCombo.SelectedIndex;
        }
    }
}