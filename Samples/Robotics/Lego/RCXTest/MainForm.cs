// Lego Mindstorm RCX test application
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

using AForge.Robotics.Lego;

namespace RCXTest
{
    public partial class MainForm : Form
    {
        // Lego RCX device
        private RCXBrick rcx = new RCXBrick( );

        // available motors
        private static RCXBrick.Motor[] motors = new RCXBrick.Motor[] {
            RCXBrick.Motor.A, RCXBrick.Motor.B, RCXBrick.Motor.C,
            RCXBrick.Motor.AC
        };

        // Constructor
        public MainForm( )
        {
            InitializeComponent( );

            soundTypeCombo.SelectedIndex = 0;
            sensorCombo.SelectedIndex = 0;
            sensorTypeCombo.SelectedIndex = 0;
            sensorModeCombo.SelectedIndex = 0;
            motorCombo.SelectedIndex = 0;
            directionCombo.SelectedIndex = 0;
            powerCombo.SelectedIndex = 0;
        }

        // On Connect button
        private void connectButton_Click( object sender, EventArgs e )
        {
            if ( rcx.Connect( RCXBrick.IRTowerType.USB ) )
            {
                EnableConnectionControls( true );

                rcx.SetTransmitterRange( true );

                // get version
                string romVersion, firmwareVersion;

                if ( rcx.GetVersion( out romVersion, out firmwareVersion ) )
                {
                    romVersionBox.Text = romVersion;
                    firmwareVersionBox.Text = firmwareVersion;
                }

                int power;

                if ( rcx.GetBatteryPower( out power ) )
                {
                    powerBox.Text = ( (double) power / 1000 ).ToString( "F3" );
                }
            }
            else
            {
                MessageBox.Show( "Failed connecting to Lego RCX", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        // On Disconnect button
        private void disconnectButton_Click( object sender, EventArgs e )
        {
            rcx.Disconnect( );
            EnableConnectionControls( false );
        }

        // Enable/disable controls available on connection
        private void EnableConnectionControls( bool enable )
        {
            connectButton.Enabled = !enable;
            disconnectButton.Enabled = enable;
            playButton.Enabled = enable;
            getValueButton.Enabled = enable;
            clearSensorButton.Enabled = enable;
            setSensorTypeButton.Enabled = enable;
            setSensorModeButton.Enabled = enable;
            motorOnButton.Enabled = enable;
            motorOffButton.Enabled = enable;
            turnDeviceOffButton.Enabled = enable;
        }

        // Play sound
        private void playButton_Click( object sender, EventArgs e )
        {
            rcx.PlaySound( (RCXBrick.SoundType) soundTypeCombo.SelectedIndex );
        }

        // Get value
        private void getValueButton_Click( object sender, EventArgs e )
        {
            short value;

            if ( rcx.GetSensorValue( (RCXBrick.Sensor) sensorCombo.SelectedIndex, out value ) )
            {
                valueBox.Text = value.ToString( );
            }
            else
            {
                valueBox.Text = string.Empty;
            }
        }

        // Clear sensor's value
        private void clearSensorButton_Click( object sender, EventArgs e )
        {
            rcx.ClearSensor( (RCXBrick.Sensor) sensorCombo.SelectedIndex );
        }

        // Set sensor's type
        private void setSensorTypeButton_Click( object sender, EventArgs e )
        {
            rcx.SetSensorType( (RCXBrick.Sensor) sensorCombo.SelectedIndex,
                (RCXBrick.SensorType) sensorTypeCombo.SelectedIndex );
        }

        // Set sensor's mode
        private void setSensorModeButton_Click( object sender, EventArgs e )
        {
            rcx.SetSensorMode( (RCXBrick.Sensor) sensorCombo.SelectedIndex,
                (RCXBrick.SensorMode) sensorModeCombo.SelectedIndex );
        }

        // Turm motor ON
        private void motorOnButton_Click( object sender, EventArgs e )
        {
            // set direction
            rcx.SetMotorDirection( motors[motorCombo.SelectedIndex],
                directionCombo.SelectedIndex == 0 );
            // set power
            rcx.SetMotorPower( motors[motorCombo.SelectedIndex],
                (byte) powerCombo.SelectedIndex );
            // turm motor on
            rcx.SetMotorOn( motors[motorCombo.SelectedIndex], true );
        }

        // Turn motor OFF
        private void motorOffButton_Click( object sender, EventArgs e )
        {
            rcx.SetMotorOn( motors[motorCombo.SelectedIndex], false );
        }

        // Turn Off Lego RCX brick
        private void turnDeviceOffButton_Click( object sender, EventArgs e )
        {
            rcx.PowerOff( );
            rcx.Disconnect( );
            EnableConnectionControls( false );
        }
    }
}