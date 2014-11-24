// Qwerk robotics board start application
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

using AForge.Robotics.TeRK;
using AForge.Video;

namespace QwerkStart
{
    public partial class MainForm : Form
    {
        private Qwerk qwerk = new Qwerk( );

        // statistics length
        private const int statLength = 15;
        // current statistics index
        private int statIndex = 0;
        // ready statistics values
        private int statReady = 0;
        // statistics array
        private int[] statCount = new int[statLength];

        bool[] ledStates = new bool[10]
        {
            false, false, false, false, false, false, false, false, false, false
        };

        // Class constructor
        public MainForm( )
        {
            InitializeComponent( );

            EnableContols( false );

        }

        // On form closing
        private void MainForm_FormClosing( object sender, FormClosingEventArgs e )
        {
            Disconnect( );
        }

        // Enable/disable connection controls
        private void EnableContols( bool enable )
        {
            connectButton.Enabled = !enable;
            disconnectButton.Enabled = enable;

            led0Button.BackColor = Color.Black;
            led1Button.BackColor = Color.Black;
            led2Button.BackColor = Color.Black;
            led3Button.BackColor = Color.Black;
            led4Button.BackColor = Color.Black;
            led5Button.BackColor = Color.Black;
            led6Button.BackColor = Color.Black;
            led7Button.BackColor = Color.Black;
            led8Button.BackColor = Color.Black;
            led9Button.BackColor = Color.Black;

            led0Button.Enabled = enable;
            led1Button.Enabled = enable;
            led2Button.Enabled = enable;
            led3Button.Enabled = enable;
            led4Button.Enabled = enable;
            led5Button.Enabled = enable;
            led6Button.Enabled = enable;
            led7Button.Enabled = enable;
            led8Button.Enabled = enable;
            led9Button.Enabled = enable;
        }

        // On "Connect" button click
        private void connectButton_Click( object sender, EventArgs e )
        {
            if ( Connect( qwerkIPBox.Text ) )
            {
                EnableContols( true );
                statusLabel.Text = "Connected";
            }
            else
            {
                MessageBox.Show( "Failed connecting to Qwerk.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        // On "Disconnect" buttong click
        private void disconnectButton_Click( object sender, EventArgs e )
        {
            Disconnect( );
            statusLabel.Text = "Disconnected";
            fpsLabel.Text = string.Empty;
            voltageLabel.Text = string.Empty;
        }

        // Connect to Qwerk
        private bool Connect( string host )
        {
            bool result = true;

            try
            {
                qwerk.Connect( qwerkIPBox.Text );

                // turn off all LEDs
                qwerk.GetLedsService( ).SetLedsState( Qwerk.LedState.Off );
                // start video camera
                Qwerk.Video qwerkVideo = qwerk.GetVideoService( );
                qwerkVideo.FrameInterval = 1000 / 15;
                videoSourcePlayer.VideoSource = qwerkVideo;
                videoSourcePlayer.Start( );

                // reset statistics
                statIndex = statReady = 0;

                // start timer
                timer.Start( );
            }
            catch
            {
                result = false;
            }

            return result;
        }

        // Handle lost connection
        private void HandleLostConnection( )
        {
            Disconnect( );
            statusLabel.Text  = "Disconnected";
            fpsLabel.Text     = string.Empty;
            voltageLabel.Text = string.Empty;

            MessageBox.Show( "Connection to Qwerk was lost.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
        }

        // Disconnect from Qwerk
        private void Disconnect( )
        {
            if ( qwerk.IsConnected )
            {
                timer.Stop( );

                try
                {
                    // stop Qwerk's camera
                    Qwerk.Video qwerkVideo = qwerk.GetVideoService( );
                    qwerkVideo.SignalToStop( );
                    qwerkVideo.WaitForStop( );

                    // turn of all LEDs and disconnect
                    qwerk.GetLedsService( ).SetLedsState( Qwerk.LedState.Off );
                }
                catch
                {
                }

                qwerk.Disconnect( );

                EnableContols( false );
            }
        }

        // On timer's tick
        private void timer_Tick( object sender, EventArgs e )
        {
            // 1 - update voltage
            try
            {
                double power = (double) qwerk.GetPower( ) / 1000;
                voltageLabel.Text = "Voltage: " + power.ToString( "F3" );
            }
            catch
            {
                HandleLostConnection( );
            }

            // 2 - update FPS
            IVideoSource videoSource = videoSourcePlayer.VideoSource;

            if ( videoSource != null )
            {
                // get number of frames for the last second
                statCount[statIndex] = videoSource.FramesReceived;

                // increment indexes
                if ( ++statIndex >= statLength )
                    statIndex = 0;
                if ( statReady < statLength )
                    statReady++;

                float fps = 0;

                // calculate average value
                for ( int i = 0; i < statReady; i++ )
                {
                    fps += statCount[i];
                }
                fps /= statReady;

                statCount[statIndex] = 0;

                fpsLabel.Text = fps.ToString( "F2" ) + " fps";
            }
        }

        // On LED's butong click
        private void ledButton_Click( object sender, EventArgs e )
        {
            int ledIndex = int.Parse( ( (Button) sender ).Text );

            ledStates[ledIndex] = !ledStates[ledIndex];

            try
            {
                // turn LED on/off
                qwerk.GetLedsService( ).SetLedState( ledIndex, ( ledStates[ledIndex] ) ? Qwerk.LedState.On : Qwerk.LedState.Off );
                // update LED button's color
                ( (Button) sender ).BackColor = ( ledStates[ledIndex] ) ? Color.Green : Color.Black;
            }
            catch
            {
                HandleLostConnection( );
            }
        }
    }
}
