// XIMEA camera sample application
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2006-2011
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

using AForge.Video;
using AForge.Video.Ximea;

namespace XimeaSample
{
    public partial class MainForm : Form
    {
        private XimeaVideoSource videoSource = null;
        private Stopwatch stopWatch = null;

        public MainForm( )
        {
            InitializeComponent( );
        }

        // On application's form is loaded
        private void MainForm_Load( object sender, EventArgs e )
        {
            int deviceCount = 0;

            try
            {
                deviceCount = XimeaCamera.CamerasCount;
            }
            catch
            {
            }

            EnableConnectionControls( true );

            if ( deviceCount != 0 )
            {
                for ( int i = 0; i < deviceCount; i++ )
                {
                    deviceCombo.Items.Add( i.ToString( ) );
                }
            }
            else
            {
                deviceCombo.Items.Add( "No cameras" );
                deviceCombo.Enabled = false;
                connectButton.Enabled = false;
            }
            deviceCombo.SelectedIndex = 0;
        }

        // On closing the application's window
        private void MainForm_FormClosing( object sender, FormClosingEventArgs e )
        {
            CloseCamera( );
        }

        // Enable/disable controls related to camera connection/operation
        private void EnableConnectionControls( bool enable )
        {
            deviceCombo.Enabled = enable;
            connectButton.Enabled = enable;

            disconnectButton.Enabled = !enable;
            widthUpDown.Enabled = !enable;
            heightUpDown.Enabled = !enable;
            exposureUpDown.Enabled = !enable;
            gainUpDown.Enabled = !enable;
            offsetXUpDown.Enabled = !enable;
            offsetYUpDown.Enabled = !enable;
        }

        // On "Connect" button click
        private void connectButton_Click( object sender, EventArgs e )
        {
            // set busy cursor
            this.Cursor = Cursors.WaitCursor;

            // close whatever is open now
            CloseCamera( );

            if ( videoSource == null )
            {
                try
                {
                    videoSource = new XimeaVideoSource( deviceCombo.SelectedIndex );
                    
                    // start the camera
                    videoSource.Start( );

                    // get some parameters
                    nameBox.Text = videoSource.GetParamString( CameraParameter.DeviceName );
                    snBox.Text   = videoSource.GetParamString( CameraParameter.DeviceSerialNumber );
                    typeBox.Text = videoSource.GetParamString( CameraParameter.DeviceType);

                    // width
                    widthUpDown.Minimum = videoSource.GetParamInt( CameraParameter.WidthMin );
                    widthUpDown.Maximum = videoSource.GetParamInt( CameraParameter.WidthMax );
                    widthUpDown.Value = videoSource.GetParamInt( CameraParameter.WidthMax );

                    // height
                    heightUpDown.Minimum = videoSource.GetParamInt( CameraParameter.HeightMin );
                    heightUpDown.Maximum = videoSource.GetParamInt( CameraParameter.HeightMax );
                    heightUpDown.Value = videoSource.GetParamInt( CameraParameter.HeightMax );

                    // exposure
                    exposureUpDown.Minimum = videoSource.GetParamInt( CameraParameter.ExposureMin ) / 1000;
                    exposureUpDown.Maximum = videoSource.GetParamInt( CameraParameter.ExposureMax ) / 1000;
                    exposureUpDown.Value = 0;
                    exposureUpDown.Value = 10;

                    // gain
                    gainUpDown.Minimum = new Decimal( videoSource.GetParamFloat( CameraParameter.GainMin ) );
                    gainUpDown.Maximum = new Decimal( videoSource.GetParamFloat( CameraParameter.GainMax ) );
                    gainUpDown.Value = new Decimal( videoSource.GetParamFloat( CameraParameter.Gain ) );
                    
                    videoSourcePlayer.VideoSource = videoSource;

                    EnableConnectionControls( false );

                    // reset stop watch
                    stopWatch = null;

                    // start timer
                    timer.Start( );
                }
                catch ( Exception ex )
                {
                    MessageBox.Show( "Failed openning XIMEA camera:\n\n" + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error );
                    CloseCamera( );
                }
            }

            this.Cursor = Cursors.Default;
        }

        // On "Disconnect" button click
        private void disconnectButton_Click( object sender, EventArgs e )
        {
            CloseCamera( );
            EnableConnectionControls( true );
        }

        // Close currently open camera if any
        private void CloseCamera( )
        {
            timer.Stop( );

            if ( videoSource != null )
            {
                videoSourcePlayer.VideoSource = null;

                videoSource.SignalToStop( );
                videoSource.WaitForStop( );
                videoSource = null;
            }
        }

        // On timer tick - update FPS info
        private void timer_Tick( object sender, EventArgs e )
        {
            if ( videoSource != null )
            {
                // get number of frames since the last timer tick
                int framesReceived = videoSource.FramesReceived;

                if ( stopWatch == null )
                {
                    stopWatch = new Stopwatch( );
                    stopWatch.Start( );
                }
                else
                {
                    stopWatch.Stop( );

                    float fps = 1000.0f * framesReceived / stopWatch.ElapsedMilliseconds;
                    fpsLabel.Text = fps.ToString( "F2" ) + " fps";

                    stopWatch.Reset( );
                    stopWatch.Start( );
                }
            }
        }

        // Width need to be changed
        private void widthUpDown_ValueChanged( object sender, EventArgs e )
        {
            if ( videoSource != null )
            {
                try
                {
                    videoSource.SetParam( CameraParameter.Width, (int) widthUpDown.Value );

                    offsetXUpDown.Maximum = widthUpDown.Maximum - widthUpDown.Value;
                    offsetXUpDown.Value = 0;
                }
                catch
                {
                }
            }
        }

        // Height need to be changed
        private void heightUpDown_ValueChanged( object sender, EventArgs e )
        {
            if ( videoSource != null )
            {
                try
                {
                    videoSource.SetParam( CameraParameter.Height, (int) heightUpDown.Value );

                    offsetYUpDown.Maximum = heightUpDown.Maximum - heightUpDown.Value;
                    offsetYUpDown.Value = 0;
                }
                catch
                {
                }
            }
        }

        // Exposure need to be changed
        private void exposureUpDown_ValueChanged( object sender, EventArgs e )
        {
            if ( videoSource != null )
            {
                try
                {
                    videoSource.SetParam( CameraParameter.Exposure, (int) ( (float) exposureUpDown.Value * 1000 ) );

                    // set interval between capturing new frames from camera
                    videoSource.FrameInterval = (int) ( 1000.0f / videoSource.GetParamFloat( CameraParameter.FramerateMax ) );

                    // reset statistics
                    stopWatch = null;
                    int bin = videoSource.FramesReceived;

                    spareLabel.Text = string.Format( "frame interval = {0} ms, max fps = {1}",
                        videoSource.FrameInterval, videoSource.GetParamFloat( CameraParameter.FramerateMax ) );
                }
                catch
                {
                }
            }
        }

        // Gain need to be changed
        private void gainUpDown_ValueChanged( object sender, EventArgs e )
        {
            if ( videoSource != null )
            {
                try
                {
                    videoSource.SetParam( CameraParameter.Gain, (float) gainUpDown.Value );
                }
                catch
                {
                }
            }
        }

        // X offset need to be changed
        private void offsetXUpDown_ValueChanged( object sender, EventArgs e )
        {
            if ( videoSource != null )
            {
                try
                {
                    videoSource.SetParam( CameraParameter.OffsetX, (int) offsetXUpDown.Value );
                }
                catch
                {
                }
            }
        }

        // Y offset need to be changed
        private void offsetYUpDown_ValueChanged( object sender, EventArgs e )
        {
            if ( videoSource != null )
            {
                try
                {
                    videoSource.SetParam( CameraParameter.OffsetY, (int) offsetYUpDown.Value );
                }
                catch
                {
                }
            }
        }

    }
}
