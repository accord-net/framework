// Motion Detection sample application
// AForge.NET Framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using AForge.Video.DirectShow;

namespace MotionDetectorSample
{
    public partial class VideoCaptureDeviceForm : Form
    {
        FilterInfoCollection videoDevices;
        private string device;

        // Video device
        public string VideoDevice
        {
            get { return device; }
        }

        // Constructor
        public VideoCaptureDeviceForm( )
        {
            InitializeComponent( );

            // show device list
			try
			{
                // enumerate video devices
                videoDevices = new FilterInfoCollection( FilterCategory.VideoInputDevice );

                if ( videoDevices.Count == 0 )
                    throw new ApplicationException( );

                // add all devices to combo
                foreach ( FilterInfo device in videoDevices )
                {
                    devicesCombo.Items.Add( device.Name );
                }
            }
            catch ( ApplicationException )
            {
                devicesCombo.Items.Add( "No local capture devices" );
                devicesCombo.Enabled = false;
                okButton.Enabled = false;
            }

            devicesCombo.SelectedIndex = 0;
        }

        // Ok button clicked
        private void okButton_Click( object sender, EventArgs e )
        {
            device = videoDevices[devicesCombo.SelectedIndex].MonikerString;
        }
    }
}