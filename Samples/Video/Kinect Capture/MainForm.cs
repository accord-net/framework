// Kinect Capture sample application
// AForge.NET Framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2011
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using AForge.Video;
using AForge.Video.Kinect;

namespace KinectCapture
{
    public partial class MainForm : Form
    {
        private Kinect kinectDevice = null;
        private KinectVideoCamera videoCamera = null;
        private KinectDepthCamera depthCamera = null;

        private static readonly LedColorOption[] ledMode =
        {
           LedColorOption.Off, LedColorOption.Green, LedColorOption.Red,
           LedColorOption.Yellow, LedColorOption.BlinkGreen, LedColorOption.BlinkRedYellow
        };

        public MainForm( )
        {
            InitializeComponent( );
        }

        // On Main form loaded
        private void MainForm_Load( object sender, EventArgs e )
        {
            if ( Kinect.DeviceCount == 0 )
            {
                devicesCombo.Items.Add( "No Kinect devices" );
            }
            else
            {
                for ( int i = 0; i < Kinect.DeviceCount; i++ )
                {
                    devicesCombo.Items.Add( "Device " + i );
                }
            }
            devicesCombo.SelectedIndex = 0;
            videoModeCombo.SelectedIndex = 0;
            EnableConnectionControls( true );
        }

        // On closing the main form
        private void MainForm_FormClosing( object sender, FormClosingEventArgs e )
        {
            Disconnect( );
        }

        // Enable/disable controls related to connecton/disconnection
        private void EnableConnectionControls( bool enable )
        {
            devicesCombo.Enabled = enable;
            videoModeCombo.Enabled = enable;
            connectButton.Enabled = enable;
            disconnectButton.Enabled = !enable;
            ledColorCombo.Enabled = !enable;
            tiltUpDown.Enabled = !enable;
        }

        // On "Connect" butto clicked
        private void connectButton_Click( object sender, EventArgs e )
        {
            if ( Connect( ) )
            {
                EnableConnectionControls( false );
            }
        }

        // On "Disconnect" butto clicked
        private void disconnectButton_Click( object sender, EventArgs e )
        {
            Disconnect( );
            EnableConnectionControls( true );
        }

        // Connect to Kinect cameras
        private bool Connect( )
        {
            bool ret = false;

            Cursor = Cursors.WaitCursor;

            if ( Kinect.DeviceCount != 0 )
            {
                int deviceID = devicesCombo.SelectedIndex;

                try
                {
                    kinectDevice = Kinect.GetDevice( deviceID );
                    
                    if ( videoCamera == null )
                    {
                        videoCamera = kinectDevice.GetVideoCamera( );
                        videoCamera.CameraMode = ( videoModeCombo.SelectedIndex == 0 ) ? VideoCameraMode.Color :
                            ( ( videoModeCombo.SelectedIndex == 1 ) ? VideoCameraMode.Bayer : VideoCameraMode.InfraRed );
                        videoCameraPlayer.VideoSource = videoCamera;
                        videoCameraPlayer.Start( );
                    }
                    
                    if ( depthCamera == null )
                    {
                        depthCamera = kinectDevice.GetDepthCamera( );
                        depthCameraPlayer.VideoSource = depthCamera;
                        depthCameraPlayer.Start( );
                    }

                    ledColorCombo.SelectedIndex = 0;

                    if ( tiltUpDown.Value != 0 )
                    {
                        tiltUpDown.Value = 0;
                    }
                    else
                    {
                        kinectDevice.SetMotorTilt( (int) tiltUpDown.Value );
                    }

                    timer.Start( );

                    ret = true;
                }
                catch ( Exception ex )
                {
                    MessageBox.Show( "Failed connecting to Kinect device.\n" + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );

                    Disconnect( );
                }
            }

            Cursor = Cursors.Default;

            return ret;
        }

        // Disconnect from Kinect cameras
        private void Disconnect( )
        {
            timer.Stop( );

            if ( videoCamera != null )
            {
                videoCameraPlayer.VideoSource = null;
                videoCamera.Stop( );
                videoCamera = null;
            }

            if ( depthCamera != null )
            {
                depthCameraPlayer.VideoSource = null;
                depthCamera.Stop( );
                depthCamera = null;
            }

            if ( kinectDevice != null )
            {
                kinectDevice.Dispose( );
                kinectDevice = null;
            }
        }

        // Change color of the LED
        private void ledColorCombo_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( kinectDevice != null )
            {
                kinectDevice.SetLedColor( ledMode[ledColorCombo.SelectedIndex] );
            }
        }

        // Tilt the camera up/down
        private void tiltUpDown_ValueChanged( object sender, EventArgs e )
        {
            if ( kinectDevice != null )
            {
                kinectDevice.SetMotorTilt( (int) tiltUpDown.Value );
            }
        }

        // Read accelerometer values on timer tick
        private void timer_Tick( object sender, EventArgs e )
        {
            double x, y, z;

            kinectDevice.GetAccelerometerValues( out x, out y, out z );

            accelerometerLabel.Text = string.Format(
                "Accelerometer values (m/(s*s)): x = {0:F4}, y = {1:F4}, z = {2:F4}", x, y, z );
        }
    }
}
