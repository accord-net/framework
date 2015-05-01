// Two Cameras Test sample application
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
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using AForge.Video;
using AForge.Video.DirectShow;

namespace TwoCamerasTest
{
    public partial class MainForm : Form
    {
        // list of video devices
        FilterInfoCollection videoDevices;
        // stop watch for measuring fps
        private Stopwatch stopWatch = null;

        public MainForm( )
        {
            InitializeComponent( );

            camera1FpsLabel.Text = string.Empty;
            camera2FpsLabel.Text = string.Empty;

            // show device list
			try
			{
                // enumerate video devices
                videoDevices = new FilterInfoCollection( FilterCategory.VideoInputDevice );

                if ( videoDevices.Count == 0 )
                {
                    throw new Exception( );
                }

                for ( int i = 1, n = videoDevices.Count; i <= n; i++ )
                {
                    string cameraName = i + " : " + videoDevices[i - 1].Name;

                    camera1Combo.Items.Add( cameraName );
                    camera2Combo.Items.Add( cameraName );
                }

                // check cameras count
                if ( videoDevices.Count == 1 )
                {
                    camera2Combo.Items.Clear( );

                    camera2Combo.Items.Add( "Only one camera found" );
                    camera2Combo.SelectedIndex = 0;
                    camera2Combo.Enabled = false;
                }
                else
                {
                    camera2Combo.SelectedIndex = 1;
                }
                camera1Combo.SelectedIndex = 0;
            }
            catch
            {
                startButton.Enabled = false;

                camera1Combo.Items.Add( "No cameras found" );
                camera2Combo.Items.Add( "No cameras found" );

                camera1Combo.SelectedIndex = 0;
                camera2Combo.SelectedIndex = 0;

                camera1Combo.Enabled = false;
                camera2Combo.Enabled = false;
            }
        }

        // On form closing
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopCameras( );
        }

        // On "Start" button click
        private void startButton_Click( object sender, EventArgs e )
        {
            StartCameras( );

            startButton.Enabled = false;
            stopButton.Enabled = true;
        }

        // On "Stop" button click
        private void stopButton_Click( object sender, EventArgs e )
        {
            StopCameras( );

            startButton.Enabled = true;
            stopButton.Enabled = false;

            camera1FpsLabel.Text = string.Empty;
            camera2FpsLabel.Text = string.Empty;
        }

        // Start cameras
        private void StartCameras( )
        {
            // create first video source
            VideoCaptureDevice videoSource1 = new VideoCaptureDevice( videoDevices[camera1Combo.SelectedIndex].MonikerString );
            videoSource1.DesiredFrameRate = 10;

            videoSourcePlayer1.VideoSource = videoSource1;
            videoSourcePlayer1.Start( );

            // create second video source
            if ( camera2Combo.Enabled == true )
            {
                System.Threading.Thread.Sleep( 500 );

                VideoCaptureDevice videoSource2 = new VideoCaptureDevice( videoDevices[camera2Combo.SelectedIndex].MonikerString );
                videoSource2.DesiredFrameRate = 10;

                videoSourcePlayer2.VideoSource = videoSource2;
                videoSourcePlayer2.Start( );
            }

            // reset stop watch
            stopWatch = null;
            // start timer
            timer.Start( );
        }

        // Stop cameras
        private void StopCameras( )
        {
            timer.Stop( );

            videoSourcePlayer1.SignalToStop( );
            videoSourcePlayer2.SignalToStop( );

            videoSourcePlayer1.WaitForStop( );
            videoSourcePlayer2.WaitForStop( );
        }

        // On times tick - collect statistics
        private void timer_Tick( object sender, EventArgs e )
        {
            IVideoSource videoSource1 = videoSourcePlayer1.VideoSource;
            IVideoSource videoSource2 = videoSourcePlayer2.VideoSource;

            int framesReceived1 = 0;
            int framesReceived2 = 0;

            // get number of frames for the last second
            if ( videoSource1 != null )
            {
                framesReceived1 = videoSource1.FramesReceived;
            }

            if ( videoSource2 != null )
            {
                framesReceived2 = videoSource2.FramesReceived;
            }

            if ( stopWatch == null )
            {
                stopWatch = new Stopwatch( );
                stopWatch.Start( );
            }
            else
            {
                stopWatch.Stop( );

                float fps1 = 1000.0f * framesReceived1 / stopWatch.ElapsedMilliseconds;
                float fps2 = 1000.0f * framesReceived2 / stopWatch.ElapsedMilliseconds;

                camera1FpsLabel.Text = fps1.ToString( "F2" ) + " fps";
                camera2FpsLabel.Text = fps2.ToString( "F2" ) + " fps";

                stopWatch.Reset( );
                stopWatch.Start( );
            }
        }
    }
}
