// Lego Mindstorm NXT test application
// AForge.NET Framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using AForge;
using AForge.Robotics.Lego;

namespace NXTTest
{
    public partial class MainForm : Form
    {
        // NXT brick
        private NXTBrick nxt = new NXTBrick( );
        // rugulation modes
        private NXTBrick.MotorRegulationMode[] regulationModes = new NXTBrick.MotorRegulationMode[] {
            NXTBrick.MotorRegulationMode.Idle,
            NXTBrick.MotorRegulationMode.Speed,
            NXTBrick.MotorRegulationMode.Sync };
        // run states
        private NXTBrick.MotorRunState[] runStates = new NXTBrick.MotorRunState[] {
            NXTBrick.MotorRunState.Idle,
            NXTBrick.MotorRunState.RampUp,
            NXTBrick.MotorRunState.Running,
            NXTBrick.MotorRunState.RampDown };
        // sensor types
        private NXTBrick.SensorType[] sensorTypes = new NXTBrick.SensorType[] {
            NXTBrick.SensorType.NoSensor, NXTBrick.SensorType.Switch,
            NXTBrick.SensorType.Temperature, NXTBrick.SensorType.Reflection,
            NXTBrick.SensorType.Angle, NXTBrick.SensorType.LightActive,
            NXTBrick.SensorType.LightInactive, NXTBrick.SensorType.SoundDB,
            NXTBrick.SensorType.SoundDBA, NXTBrick.SensorType.Custom,
            NXTBrick.SensorType.Lowspeed, NXTBrick.SensorType.Lowspeed9V };
        // sensor modes
        private NXTBrick.SensorMode[] sensorModes = new NXTBrick.SensorMode[] {
            NXTBrick.SensorMode.Raw, NXTBrick.SensorMode.Boolean,
            NXTBrick.SensorMode.TransitionCounter, NXTBrick.SensorMode.PeriodicCounter,
            NXTBrick.SensorMode.PCTFullScale, NXTBrick.SensorMode.Celsius,
            NXTBrick.SensorMode.Fahrenheit, NXTBrick.SensorMode.AngleSteps };

        // Constructor
        public MainForm( )
        {
            InitializeComponent( );

            // setup defaults
            portBox.Text = "COM8";
            motorCombo.SelectedIndex = 0;
            regulationModeCombo.SelectedIndex = 0;
            runStateCombo.SelectedIndex = 2;
            inputPortCombo.SelectedIndex = 0;
            sensorTypeCombo.SelectedIndex = 0;
            sensorModeCombo.SelectedIndex = 0;

            nxt.MessageSent += new MessageTransferHandler( nxt_MessageSent );
            nxt.MessageRead += new MessageTransferHandler( nxt_MessageRead );
        }

        // On message sent by NXT brick
        private void nxt_MessageSent( object sender, CommunicationBufferEventArgs eventArgs )
        {
            System.Diagnostics.Debug.WriteLine( string.Format( ">> [ {0} ]", eventArgs.GetMessageString( ) ) );
        }

        // On message received by NXT brick
        private void nxt_MessageRead( object sender, CommunicationBufferEventArgs eventArgs )
        {
            System.Diagnostics.Debug.WriteLine( string.Format( "<< [ {0} ]", eventArgs.GetMessageString( ) ) );
        }

        // On "Connect" button click
        private void connectButton_Click( object sender, EventArgs e )
        {
            if ( nxt.Connect( portBox.Text ) )
            {
                System.Diagnostics.Debug.WriteLine( "Connected successfully" );

                CollectInformation( );

                // enable controls
                resetMotorButton.Enabled    = true;
                setMotorStateButton.Enabled = true;
                getMotorStateButton.Enabled = true;
                getInputButton.Enabled      = true;
                setInputModeButton.Enabled  = true;

                connectButton.Enabled    = false;
                disconnectButton.Enabled = true;

                nxt.PlayTone( 100, 200, false );
            }
            else
            {
                MessageBox.Show( "Failed connecting to NXT device", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        // On "Disconnect" button click
        private void disconnectButton_Click( object sender, EventArgs e )
        {
            nxt.Disconnect( );

            // clear information
            firmwareBox.Text = string.Empty;
            protocolBox.Text = string.Empty;
            deviceNameBox.Text = string.Empty;
            btAddressBox.Text = string.Empty;
            btSignalStrengthBox.Text = string.Empty;
            freeUserFlashBox.Text = string.Empty;
            batteryLevelBox.Text = string.Empty;

            tachoCountBox.Text = string.Empty;
            blockTachoCountBox.Text = string.Empty;
            rotationCountBox.Text = string.Empty;

            validCheck.Checked = false;
            calibratedCheck.Checked = false;
            sensorTypeBox.Text = string.Empty;
            sensorModeBox.Text = string.Empty;
            rawInputBox.Text = string.Empty;
            normalizedInputBox.Text = string.Empty;
            scaledInputBox.Text = string.Empty;
            calibratedInputBox.Text = string.Empty;

            // disable controls
            resetMotorButton.Enabled    = false;
            setMotorStateButton.Enabled = false;
            getMotorStateButton.Enabled = false;
            getInputButton.Enabled      = false;
            setInputModeButton.Enabled  = false;

            connectButton.Enabled    = true;
            disconnectButton.Enabled = false;
        }

        // Collect information about Lego NXT brick
        private void CollectInformation( )
        {
            // ------------------------------------------------
            // get NXT version
            string firmwareVersion;
            string protocolVersion;

            if ( nxt.GetVersion( out protocolVersion, out firmwareVersion ) )
            {
                firmwareBox.Text = firmwareVersion;
                protocolBox.Text = protocolVersion;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine( "Failed getting verion" );
            }

            // ------------------------------------------------
            // get device information
            string deviceName;
            byte[] btAddress;
            int btSignalStrength;
            int freeUserFlesh;

            if ( nxt.GetDeviceInformation( out deviceName, out btAddress, out btSignalStrength, out freeUserFlesh ) )
            {
                deviceNameBox.Text = deviceName;

                btAddressBox.Text = string.Format( "{0} {1} {2} {3} {4} {5} {6}",
                    btAddress[0].ToString( "X2" ),
                    btAddress[1].ToString( "X2" ),
                    btAddress[2].ToString( "X2" ),
                    btAddress[3].ToString( "X2" ),
                    btAddress[4].ToString( "X2" ),
                    btAddress[5].ToString( "X2" ),
                    btAddress[6].ToString( "X2" )
                );

                btSignalStrengthBox.Text = btSignalStrength.ToString( );
                freeUserFlashBox.Text = freeUserFlesh.ToString( );
            }
            else
            {
                System.Diagnostics.Debug.WriteLine( "Failed getting device information" );
            }


            // ------------------------------------------------
            // get battery level
            int batteryLevel;

            if ( nxt.GetBatteryPower( out batteryLevel ) )
            {
                batteryLevelBox.Text = batteryLevel.ToString( );
            }
            else
            {
                System.Diagnostics.Debug.WriteLine( "Failed getting battery level" );
            }
        }

        // Returns selected motor
        private NXTBrick.Motor GetSelectedMotor( )
        {
            return (NXTBrick.Motor) motorCombo.SelectedIndex;
        }

        // Returns selected input port
        private NXTBrick.Sensor GetSelectedSensor( )
        {
            return (NXTBrick.Sensor) inputPortCombo.SelectedIndex;
        }

        // On motor "Reset" button click
        private void resetMotorButton_Click( object sender, EventArgs e )
        {
            if ( nxt.ResetMotorPosition( GetSelectedMotor( ), false, false ) != true )
            {
                System.Diagnostics.Debug.WriteLine( "Failed reseting motor" );
            }
        }

        // On motor "Set state" button click
        private void setMotorStateButton_Click( object sender, EventArgs e )
        {
            NXTBrick.MotorState motorState = new NXTBrick.MotorState( );

            // prepare motor's state to set
            motorState.Power = (sbyte) powerUpDown.Value;
            motorState.TurnRatio = (sbyte) turnRatioUpDown.Value;
            motorState.Mode = ( ( modeOnCheck.Checked ) ? NXTBrick.MotorMode.On : NXTBrick.MotorMode.None ) |
                ( ( modeBrakeCheck.Checked ) ? NXTBrick.MotorMode.Brake : NXTBrick.MotorMode.None ) |
                ( ( modeRegulatedBox.Checked ) ? NXTBrick.MotorMode.Regulated : NXTBrick.MotorMode.None );
            motorState.Regulation = regulationModes[regulationModeCombo.SelectedIndex];
            motorState.RunState = runStates[runStateCombo.SelectedIndex];
            // tacho limit
            try
            {
                motorState.TachoLimit = Math.Max( 0, Math.Min( 100000, int.Parse( tachoLimitBox.Text ) ) );
            }
            catch
            {
                motorState.TachoLimit = 1000;
                tachoLimitBox.Text = motorState.TachoLimit.ToString( );
            }

            // set motor's state
            if ( nxt.SetMotorState( GetSelectedMotor( ), motorState, false ) != true )
            {
                System.Diagnostics.Debug.WriteLine( "Failed setting motor state" );
            }
        }

        // On motor "Get state" button click
        private void getMotorStateButton_Click( object sender, EventArgs e )
        {
            NXTBrick.MotorState motorState;

            // get motor's state
            if ( nxt.GetMotorState( GetSelectedMotor( ), out motorState ) )
            {
                tachoCountBox.Text = motorState.TachoCount.ToString( );
                blockTachoCountBox.Text = motorState.BlockTachoCount.ToString( );
                rotationCountBox.Text = motorState.RotationCount.ToString( );
            }
            else
            {
                System.Diagnostics.Debug.WriteLine( "Failed getting motor state" );
            }
        }

        // On "Get input" button click
        private void getInputButton_Click( object sender, EventArgs e )
        {
            NXTBrick.SensorValues sensorValues;

            // get input values
            if ( nxt.GetSensorValue( GetSelectedSensor( ), out sensorValues ) )
            {
                validCheck.Checked      = sensorValues.IsValid;
                calibratedCheck.Checked = sensorValues.IsCalibrated;
                sensorTypeBox.Text      = sensorValues.SensorType.ToString( );
                sensorModeBox.Text      = sensorValues.SensorMode.ToString( );
                rawInputBox.Text        = sensorValues.Raw.ToString( );
                normalizedInputBox.Text = sensorValues.Normalized.ToString( );
                scaledInputBox.Text     = sensorValues.Scaled.ToString( );
                calibratedInputBox.Text = sensorValues.Calibrated.ToString( );
            }
            else
            {
                System.Diagnostics.Debug.WriteLine( "Failed getting input values" );
            }
        }

        // On "Set mode" button click
        private void setInputModeButton_Click( object sender, EventArgs e )
        {
            if ( nxt.SetSensorMode( GetSelectedSensor( ),
                sensorTypes[sensorTypeCombo.SelectedIndex],
                sensorModes[sensorModeCombo.SelectedIndex], false ) != true )
            {
                System.Diagnostics.Debug.WriteLine( "Failed setting input mode" );
            }
        }
    }
}