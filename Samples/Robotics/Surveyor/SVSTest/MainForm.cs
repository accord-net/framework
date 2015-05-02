// Surveyor SVS test application
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

using AForge.Video;
using AForge.Controls;
using AForge.Robotics.Surveyor;

namespace SVSTest
{
    public partial class MainForm : Form
    {
        private SVS svs = new SVS( );
        private StereoViewForm stereoViewForm;

        // statistics length
        private const int statLength = 15;
        // current statistics index
        private int statIndex = 0;
        // ready statistics values
        private int statReady = 0;
        // statistics array
        private int[] statCount1 = new int[statLength];
        private int[] statCount2 = new int[statLength];

        private bool receivedFirstDrivingCommand = false;
        private SRV1.MotorCommand lastMotorCommand;

        // maximum motors' power for 15 sectors 
        private float[] maxPowers = new float[] { 0.1f, 0.2f, 0.35f, 0.5f, 0.65f, 0.8f, 0.9f, 1.0f,
            1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f };

        private int maxMotorPower = 90;
        private int minMotorPower = 50;
     
        // Class constructor
        public MainForm( )
        {
            InitializeComponent( );
            EnableContols( false );

            minPowerUpDown.Value = minMotorPower;
            maxPowerUpDown.Value = maxMotorPower;
        }

        // On form closing
        private void MainForm_FormClosing( object sender, FormClosingEventArgs e )
        {
            Disconnect( );
        }

        // Enable/disable connection controls
        private void EnableContols( bool enable )
        {
            ipBox.Enabled              = !enable;
            connectButton.Enabled      = !enable;
            disconnectButton.Enabled   = enable;
            qualityCombo.Enabled       = enable;
            resolutionCombo.Enabled    = enable;
            srvDriverControl.Enabled   = enable;
            manipulatorControl.Enabled = enable;
            turnControl.Enabled        = enable;
        }

        // On "Connect" button click
        private void connectButton_Click( object sender, EventArgs e )
        {
            if ( Connect( ipBox.Text ) )
            {
                EnableContols( true );
                statusLabel.Text = "Connected";

                qualityCombo.SelectedIndex = 6;
                resolutionCombo.SelectedIndex = 1;
            }
            else
            {
                MessageBox.Show( "Failed connecting to SVS.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        // On "Disconnect" buttong click
        private void disconnectButton_Click( object sender, EventArgs e )
        {
            Disconnect( );
            statusLabel.Text  = "Disconnected";
            fpsLabel.Text     = string.Empty;
            versionLabel.Text = string.Empty;
        }

        // Connect to SVS
        private bool Connect( string host )
        {
            bool result = true;

            try
            {
                svs.Connect( host );
                svs.EnableFailsafeMode( 0, 0 );

                svs.FlipVideo( false );
                svs.SetQuality( 7 );
                svs.SetResolution( SRV1.VideoResolution.Small );

                // start left camera
                SRV1Camera leftCamera = svs.GetCamera( SVS.Camera.Left );
                leftCameraPlayer.VideoSource = leftCamera;
                leftCameraPlayer.Start( );

                // start right camera
                SRV1Camera rightCamera = svs.GetCamera( SVS.Camera.Right );
                rightCameraPlayer.VideoSource = rightCamera;
                rightCameraPlayer.Start( );

                versionLabel.Text = svs.GetVersion( );
                receivedFirstDrivingCommand = false;

                // reset statistics
                statIndex = statReady = 0;

                // start timer
                timer.Start( );
            }
            catch
            {
                result = false;
                Disconnect( );
            }

            return result;
        }

        // Disconnect from SVS
        private void Disconnect( )
        {
            if ( svs.IsConnected )
            {
                timer.Stop( );

                if ( leftCameraPlayer.VideoSource != null )
                {
                    leftCameraPlayer.VideoSource.SignalToStop( );
                    leftCameraPlayer.VideoSource.WaitForStop( );
                    leftCameraPlayer.VideoSource = null;
                }

                if ( rightCameraPlayer.VideoSource != null )
                {
                    rightCameraPlayer.VideoSource.SignalToStop( );
                    rightCameraPlayer.VideoSource.WaitForStop( );
                    rightCameraPlayer.VideoSource = null;
                }

                svs.StopMotors( );
                svs.Disconnect( );

                EnableContols( false );
            }
        }

        // On timer's tick
        private void timer_Tick( object sender, EventArgs e )
        {
            // update camaeras' FPS
            if ( ( leftCameraPlayer.VideoSource != null ) || ( rightCameraPlayer.VideoSource != null ) )
            {
                // get number of frames for the last second
                if ( leftCameraPlayer.VideoSource != null )
                {
                    statCount1[statIndex] = leftCameraPlayer.VideoSource.FramesReceived;
                }
                if ( rightCameraPlayer.VideoSource != null )
                {
                    statCount2[statIndex] = rightCameraPlayer.VideoSource.FramesReceived;
                }

                // increment indexes
                if ( ++statIndex >= statLength )
                    statIndex = 0;
                if ( statReady < statLength )
                    statReady++;

                float fps1 = 0;
                float fps2 = 0;

                // calculate average value
                for ( int i = 0; i < statReady; i++ )
                {
                    fps1 += statCount1[i];
                    fps2 += statCount2[i];
                }
                fps1 /= statReady;
                fps2 /= statReady;

                fpsLabel.Text = string.Format( "Left: {0:F2} fps, Right: {1:F2} fps",
                    fps1, fps2 );
            }
        }

        // Set video quality
        private void qualityCombo_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( svs.IsConnected )
            {
                try
                {
                    svs.SetQuality( qualityCombo.SelectedIndex + 1 );

                    // reset FPS statistics
                    statIndex = statReady = 0;
                }
                catch ( Exception ex )
                {
                    System.Diagnostics.Debug.WriteLine( "## " + ex.Message );
                }
            }
        }

        // Set video resolution
        private void resolutionCombo_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( svs.IsConnected )
            {
                try
                {
                    SRV1.VideoResolution resolution = SRV1.VideoResolution.Small;

                    switch ( resolutionCombo.SelectedIndex )
                    {
                        case 0:
                            resolution = SRV1.VideoResolution.Tiny;
                            break;
                        case 2:
                            resolution = SRV1.VideoResolution.Medium;
                            break;
                    }

                    svs.SetResolution( resolution );

                    // reset FPS statistics
                    statIndex = statReady = 0;
                }
                catch ( Exception ex )
                {
                    System.Diagnostics.Debug.WriteLine( "## " + ex.Message );
                }
            }
        }

        // Show window with stereo anaglyph
        private void showStereoButton_Click( object sender, EventArgs e )
        {
            if ( stereoViewForm == null )
            {
                stereoViewForm = new StereoViewForm( );
                stereoViewForm.TopMost = true;

                stereoViewForm.FormClosing += new FormClosingEventHandler( stereoViewForm_OnFormClosing );

                leftCameraPlayer.NewFrame += new VideoSourcePlayer.NewFrameHandler( stereoViewForm.OnNewLeftFrame );
                rightCameraPlayer.NewFrame += new VideoSourcePlayer.NewFrameHandler( stereoViewForm.OnNewRightFrame );
            }

            stereoViewForm.Show( );

            stereoViewForm.BringToFront( );
        }

        private void stereoViewForm_OnFormClosing( object sender, FormClosingEventArgs eventArgs )
        {
            leftCameraPlayer.NewFrame -= new VideoSourcePlayer.NewFrameHandler( stereoViewForm.OnNewLeftFrame );
            rightCameraPlayer.NewFrame -= new VideoSourcePlayer.NewFrameHandler( stereoViewForm.OnNewRightFrame );

            stereoViewForm.FormClosing -= new FormClosingEventHandler( stereoViewForm_OnFormClosing );
            stereoViewForm = null;
        }

        private void srvDriverControl_SrvDrivingCommand( object sender, SRV1.MotorCommand command )
        {
            if ( svs.IsConnected )
            {
                try
                {
                    if ( !receivedFirstDrivingCommand )
                    {
                        // use one direct control command first
                        svs.StopMotors( );
                    }

                    // send new command
                    svs.ControlMotors( command );

                    if ( ( ( command == SRV1.MotorCommand.DecreaseSpeed ) ||
                           ( command == SRV1.MotorCommand.IncreaseSpeed ) ) &&
                           ( receivedFirstDrivingCommand ) &&
                           ( lastMotorCommand != SRV1.MotorCommand.Stop ) )
                    {
                        // resend last command to get effect of speed increase/decrease
                        svs.ControlMotors( lastMotorCommand  );
                    }
                    else
                    {
                        receivedFirstDrivingCommand = true;
                        lastMotorCommand = command;
                    }
                }
                catch ( Exception ex )
                {
                    System.Diagnostics.Debug.WriteLine( "## " + ex.Message );
                }
            }
        }

        // Switch control type
        private void directControlRadio_CheckedChanged( object sender, EventArgs e )
        {
            bool directControlEnabled = directControlRadio.Checked;

            manipulatorControl.Visible = directControlEnabled;
            turnControl.Visible = directControlEnabled;
            minPowerLabel.Visible = directControlEnabled;
            maxPowerLabel.Visible = directControlEnabled;
            minPowerUpDown.Visible = directControlEnabled;
            maxPowerUpDown.Visible = directControlEnabled;

            srvDriverControl.Visible = !directControlEnabled;
        }

        // Driving with "software joystick"
        private void manipulatorControl_PositionChanged( object sender, ManipulatorControl.PositionEventArgs eventArgs )
        {
            float leftMotorPower = 0f, rightMotorPower = 0f;

            // calculate robot's direction and speed
            if ( ( eventArgs.X != 0 ) || ( eventArgs.Y != 0 ) )
            {
                // radius (distance from center)
                double r = eventArgs.R;
                // theta
                double t = eventArgs.Theta;

                if ( t > 180 )
                    t -= 180;

                // index of maximum power
                int maxPowerIndex = (int) ( t / 180 * maxPowers.Length );

                // check direction to move
                if ( eventArgs.Y > 0 )
                {
                    // forward direction
                    leftMotorPower  = (float) ( r * maxPowers[maxPowers.Length - maxPowerIndex - 1] );
                    rightMotorPower = (float) ( r * maxPowers[maxPowerIndex] );
                }
                else
                {
                    // backward direction
                    leftMotorPower  = (float) ( -r * maxPowers[maxPowerIndex] );
                    rightMotorPower = (float) ( -r * maxPowers[maxPowers.Length - maxPowerIndex - 1] );
                }
            }

            DriveMotors( leftMotorPower, rightMotorPower );
        }

        // Robot turning on place - opposite directions for motors
        private void turnControl_PositionChanged( object sender, float x )
        {
            DriveMotors( x, -x );
        }

        private void DriveMotors( float leftMotor, float rightMotor )
        {
            int leftMotorVelocity  = 0;
            int rightMotorVelocity = 0;

            // make sure velocities are in [min, max] range
            int delta = maxMotorPower - minMotorPower;

            if ( leftMotor != 0 )
            {
                if ( leftMotor > 0 )
                    leftMotorVelocity =  minMotorPower + (int) ( delta * leftMotor );
                else
                    leftMotorVelocity = -minMotorPower + (int) ( delta * leftMotor );
            }
            if ( rightMotor != 0 )
            {
                if ( rightMotor > 0 )
                    rightMotorVelocity =  minMotorPower + (int) ( delta * rightMotor );
                else
                    rightMotorVelocity = -minMotorPower + (int) ( delta * rightMotor );
            }

            System.Diagnostics.Debug.WriteLine( string.Format( "l: {0}, r: {1}", leftMotorVelocity, rightMotorVelocity ) );
            if ( svs.IsConnected )
            {
                try
                {
                    svs.RunMotors( leftMotorVelocity, rightMotorVelocity, 0 );
                }
                catch ( Exception ex )
                {
                    System.Diagnostics.Debug.WriteLine( "## " + ex.Message );
                }
            }
        }

        // Changing motors' power limits
        private void minPowerUpDown_ValueChanged( object sender, EventArgs e )
        {
            minMotorPower = (int) minPowerUpDown.Value;
        }

        private void maxPowerUpDown_ValueChanged( object sender, EventArgs e )
        {
            maxMotorPower = (int) maxPowerUpDown.Value;
        }

        private void aboutButton_Click( object sender, EventArgs e )
        {
            AboutForm form = new AboutForm( );

            form.ShowDialog( );
        }
    }
}
