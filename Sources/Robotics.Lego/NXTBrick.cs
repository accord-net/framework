// AForge Lego Robotics Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

namespace AForge.Robotics.Lego
{
    using System;
    using AForge;
    using AForge.Robotics.Lego.Internals;

    /// <summary>
    /// Manipulation of Lego Mindstorms NXT device.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>The class allows to manipulate with Lego Mindstorms NXT device,
    /// setting/getting its motors' state, getting information about sensors'
    /// values and retrieving generic information about the NXT brick.</para>
    /// <para><img src="img/robotics/nxt.jpg" width="250" height="201" /></para>
    /// 
    /// <para><note>Only communication through Bluetooth (virtual serial port) is supported at this point.</note></para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create an instance of NXT brick
    /// NXTBrick nxt = new NXTBrick( );
    /// // connect to the device
    /// if ( nxt.Connect( "COM8" ) )
    /// {
    ///     // run motor A
    ///     NXTBrick.MotorState motorState = new NXTBrick.MotorState( );
    /// 
    ///     motorState.Power      = 70;
    ///     motorState.TurnRatio  = 50;
    ///     motorState.Mode       = NXTBrick.MotorMode.On;
    ///     motorState.Regulation = NXTBrick.MotorRegulationMode.Idle;
    ///     motorState.RunState   = NXTBrick.MotorRunState.Running;
    ///     motorState.TachoLimit = 1000;
    /// 
    ///     nxt.SetMotorState( NXTBrick.Motor.A, motorState );
    /// 
    ///     // get input value from the first sensor
    ///     NXTBrick.SensorValues sensorValues;
    /// 
    ///     if ( nxt.GetSensorValue( NXTBrick.Sensor.First, out sensorValues ) )
    ///     {
    ///         // ...
    ///     }
    ///     // ...
    /// }
    /// </code>
    /// </remarks>
    /// 
    public partial class NXTBrick
    {
        #region Embedded types

        /// <summary>
        /// Enumeration of NXT brick sensor ports.
        /// </summary>
        public enum Sensor
        {
            /// <summary>
            /// First sensor.
            /// </summary>
            First,
            /// <summary>
            /// Second sensor.
            /// </summary>
            Second,
            /// <summary>
            /// Third sensor.
            /// </summary>
            Third,
            /// <summary>
            /// Fourth sensor.
            /// </summary>
            Fourth
        }

        /// <summary>
        /// Enumeration of NXT brick sensor types.
        /// </summary>
        /// 
        public enum SensorType
        {
            /// <summary>
            /// No sensor.
            /// </summary>
            NoSensor = 0x00,
            /// <summary>
            /// NXT or Legacy touch sensor.
            /// </summary>
            Switch = 0x01,
            /// <summary>
            /// Legacy temperature sensor.
            /// </summary>
            Temperature = 0x02,
            /// <summary>
            /// Legacy light sensor.
            /// </summary>
            Reflection = 0x03,
            /// <summary>
            /// Legacy rotation sensor.
            /// </summary>
            Angle = 0x04,
            /// <summary>
            /// NXT light sensor with floodlight enabled.
            /// </summary>
            LightActive = 0x05,
            /// <summary>
            /// NXT light sensor with floodlight disabled.
            /// </summary>
            LightInactive = 0x06,
            /// <summary>
            /// NXT sound sensor (dB scaling).
            /// </summary>
            SoundDB = 0x07,
            /// <summary>
            /// NXT sound sensor (dBA scaling).
            /// </summary>
            SoundDBA = 0x08,
            /// <summary>
            /// Unused
            /// </summary>
            Custom = 0x09,
            /// <summary>
            /// I2C digital sensor.
            /// </summary>
            Lowspeed = 0x0A,
            /// <summary>
            /// I2C digital sensor (9V power).
            /// </summary>
            Lowspeed9V = 0x0B,
            /// <summary>
            /// Unused.
            /// </summary>
            Highspeed = 0x0C,
            /// <summary>
            /// NXT 2.0 color sensor in color detector mode.
            /// </summary>
            ColorFull = 0x0D,
            /// <summary>
            /// NXT 2.0 color sensor in light sensor mode with red light on.
            /// </summary>
            ColorRed = 0x0E,
            /// <summary>
            /// NXT 2.0 color sensor in light sensor mode with green light on.
            /// </summary>
            ColorGreen = 0x0F,
            /// <summary>
            /// NXT 2.0 color sensor in light sensor mode with blue light on.
            /// </summary>
            ColorBlue = 0x10,
            /// <summary>
            /// NXT 2.0 color sensor in light sensor mode without light.
            /// </summary>
            ColorNone = 0x11,
            /// <summary>
            /// NXT 2.0 color sensor internal state (no functionality known yet).
            /// </summary>
            ColorExit = 0x12
        }

        /// <summary>
        /// Enumeration of NXT brick sensor modes.
        /// </summary>
        /// 
        public enum SensorMode
        {
            /// <summary>
            /// Raw mode.
            /// </summary>
            Raw = 0x00,
            /// <summary>
            /// Boolean mode. Report scaled value as 1 (TRUE) or 0 (FALSE). The firmware uses
            /// inverse Boolean logic to match the physical characteristics of NXT sensors. Readings
            /// are FALSE if raw value exceeds 55% of total range; reading are TRUE if raw value
            /// is less than 45% of total range.
            /// </summary>
            Boolean = 0x20,
            /// <summary>
            /// Report scaled value as number of transition between TRUE and FALSE.
            /// </summary>
            TransitionCounter = 0x40,
            /// <summary>
            /// Report scaled value as number of transitions from FALSE to TRUE, then back to FALSE.
            /// </summary>
            PeriodicCounter = 0x60,
            /// <summary>
            /// Report scaled value as percentage of full scale reading for configured sensor type.
            /// </summary>
            PCTFullScale = 0x80,
            /// <summary>
            /// Scale terperature reading to degrees Celsius.
            /// </summary>
            Celsius = 0xA0,
            /// <summary>
            /// Scale terperature reading to degrees Fahrenheit.
            /// </summary>
            Fahrenheit = 0xC0,
            /// <summary>
            /// Report scaled value as count of ticks on RCX-style rotation sensor.
            /// </summary>
            AngleSteps = 0xE0
        }

        /// <summary>
        /// Class describing sensor's values received from NXT brick's sensor port.
        /// </summary>
        /// 
        public class SensorValues
        {
            /// <summary>
            /// Specifies if data value should be treated as valid data.
            /// </summary>
            public bool IsValid
            {
                get { return isValid;  }
                internal set { isValid = value; }
            }
            private bool isValid;

            /// <summary>
            /// Specifies if calibration file was found and used for <see cref="Calibrated"/>
            /// field calculation.
            /// </summary>
            public bool IsCalibrated
            {
                get { return isCalibrated;  }
                internal set { isCalibrated = value; }
            }
            private bool isCalibrated;

            /// <summary>
            /// Sensor type.
            /// </summary>
            public SensorType SensorType
            {
                get { return sensorType; }
                internal set { sensorType = value; }
            }
            private SensorType sensorType;

            /// <summary>
            /// Sensor mode.
            /// </summary>
            public SensorMode SensorMode
            {
                get { return sensorMode; }
                internal set { sensorMode = value; }
            }
            private SensorMode sensorMode;

            /// <summary>
            /// Raw A/D value (device dependent).
            /// </summary>
            public ushort Raw
            {
                get { return raw; }
                internal set { raw = value; }
            }
            private ushort raw;

            /// <summary>
            /// Normalized A/D value (sensor type dependent), [0, 1023].
            /// </summary>
            public ushort Normalized
            {
                get { return normalized; }
                internal set { normalized = value; }
            }
            private ushort normalized;

            /// <summary>
            /// Scaled value (sensor mode dependent).
            /// </summary>
            public short Scaled
            {
                get { return scaled; }
                internal set { scaled = value; }
            }
            private short scaled;

            /// <summary>
            /// Value scaled according to calibration.
            /// </summary>
            /// 
            /// <remarks><note>According to Lego notes the value is currently unused.</note></remarks>
            /// 
            public short Calibrated
            {
                get { return calibrated; }
                internal set { calibrated = value; }
            }
            private short calibrated;
        }

        /// <summary>
        /// Enumeration of NXT brick motor ports.
        /// </summary>
        /// 
        public enum Motor
        {
            /// <summary>
            /// Motor A.
            /// </summary>
            A = 0x00,
            /// <summary>
            /// Motor B.
            /// </summary>
            B = 0x01,
            /// <summary>
            /// Motor C.
            /// </summary>
            C = 0x02,
            /// <summary>
            /// All motors (A, B and C).
            /// </summary>
            All = 0xFF
        }

        /// <summary>
        /// Enumeration of supported motor modes.
        /// </summary>
        /// 
        /// <remarks>Motor mode is a bit field, so several modes can be combined.</remarks>
        /// 
        [FlagsAttribute]
        public enum MotorMode
        {
            /// <summary>
            /// Mode is not set.
            /// </summary>
            None = 0x00,
            /// <summary>
            /// Turn on the motor.
            /// </summary>
            On = 0x01,
            /// <summary>
            /// Brake.
            /// </summary>
            Brake = 0x02,
            /// <summary>
            /// Turn on regulated mode.
            /// </summary>
            Regulated = 0x04
        }

        /// <summary>
        /// Enumeration of motor regulation modes.
        /// </summary>
        /// 
        public enum MotorRegulationMode
        {
            /// <summary>
            /// No regulation will be enabled.
            /// </summary>
            Idle = 0x00,
            /// <summary>
            /// Power control will be enabled on specified motor.
            /// </summary>
            Speed = 0x01,
            /// <summary>
            /// Synchronization will be enabled.
            /// </summary>
            /// 
            /// <remarks><note>Synchronization need to be enabled on two motors.</note></remarks>
            /// 
            Sync = 0x02
        }

        /// <summary>
        /// Enumeration of motor run states.
        /// </summary>
        /// 
        public enum MotorRunState
        {
            /// <summary>
            /// Motor will be idle.
            /// </summary>
            Idle = 0x00,
            /// <summary>
            /// Motor will ramp-up.
            /// </summary>
            RampUp = 0x10,
            /// <summary>
            /// Motor will be running.
            /// </summary>
            Running = 0x20,
            /// <summary>
            /// Motor will ramp-down.
            /// </summary>
            RampDown = 0x40
        }

        /// <summary>
        /// Class describing motor's state.
        /// </summary>
        /// 
        public class MotorState
        {
            /// <summary>
            /// Power, [-100, 100].
            /// </summary>
            public int Power
            {
                get { return power; }
                set { power = Math.Min( Math.Max( -100, value ), 100 ); }
            }
            private int power;

            /// <summary>
            /// Turn ratio, [-100, 100].
            /// </summary>
            public int TurnRatio
            {
                get { return turnRatio; }
                set { turnRatio = Math.Min( Math.Max( -100, value ), 100 ); }
            }
            private int turnRatio;

            /// <summary>
            /// Mode (bit field).
            /// </summary>
            public MotorMode Mode
            {
                get { return mode; }
                set { mode = value; }
            }
            private MotorMode mode = MotorMode.None;

            /// <summary>
            /// Regulation mode.
            /// </summary>
            public MotorRegulationMode Regulation
            {
                get { return regulation; }
                set { regulation = value; }
            }
            private MotorRegulationMode regulation = MotorRegulationMode.Idle;

            /// <summary>
            /// Run state.
            /// </summary>
            public MotorRunState RunState
            {
                get { return runState; }
                set { runState = value; }
            }
            private MotorRunState runState = MotorRunState.Idle;

            /// <summary>
            /// Tacho limit (0 - run forever).
            /// </summary>
            /// 
            /// <remarks>The value determines motor's run limit.</remarks>
            public int TachoLimit
            {
                get { return tachoLimit; }
                set { tachoLimit = Math.Max( 0, value ); }
            }
            private int tachoLimit;

            /// <summary>
            /// Number of counts since last reset of motor counter.
            /// </summary>
            /// 
            /// <remarks><note>The value is ignored when motor's state is set. The value is
            /// provided when motor's state is retrieved.</note></remarks>
            public int TachoCount
            {
                get { return tachoCount; }
                internal set { tachoCount = value; }
            }
            private int tachoCount;

            /// <summary>
            /// Current position relative to last programmed movement.
            /// </summary>
            /// 
            /// <remarks><note>The value is ignored when motor's state is set. The value is
            /// provided when motor's state is retrieved.</note></remarks>
            public int BlockTachoCount
            {
                get { return blockTachoCount; }
                internal set { blockTachoCount = value; }
            }
            private int blockTachoCount;

            /// <summary>
            /// Current position relative to last reset of motor's rotation sensor.
            /// </summary>
            /// 
            /// <remarks><note>The value is ignored when motor's state is set. The value is
            /// provided when motor's state is retrieved.</note></remarks>
            public int RotationCount
            {
                get { return rotationCount; }
                internal set { rotationCount = value; }
            }
            private int rotationCount;

            /// <summary>
            /// Initializes a new instance of the <see cref="MotorState"/> class.
            /// </summary>
            public MotorState( ) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="MotorState"/> class.
            /// </summary>
            /// 
            /// <param name="power">Power, [-100, 100].</param>
            /// <param name="turnRatio">Turn ratio, [-100, 100].</param>
            /// <param name="mode">Mode (bit field).</param>
            /// <param name="regulation">Regulation mode.</param>
            /// <param name="runState">Run state.</param>
            /// <param name="tachoLimit">The value determines motor's run limit.</param>
            /// 
            public MotorState( int power, int turnRatio, MotorMode mode,
                MotorRegulationMode regulation, MotorRunState runState, int tachoLimit )
            {
                Power      = power;
                TurnRatio  = turnRatio;
                Mode       = mode;
                Regulation = regulation;
                RunState   = runState;
                TachoLimit = tachoLimit;

                TachoCount      = 0;
                BlockTachoCount = 0;
                RotationCount   = 0;
            }
        }

        #endregion

        // communication interfaced used for communication with NXT brick
        private INXTCommunicationInterface communicationInterface;

        // dummy object to lock for synchronization
        private object sync = new object( );

        /// <summary>
        /// Check if connection to NXT brick is established.
        /// </summary>
        /// 
        public bool IsConnected
        {
            get
            {
                lock ( sync )
                {
                    return ( communicationInterface != null );
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NXTBrick"/> class.
        /// </summary>
        /// 
        public NXTBrick( )
        {
        }

        /// <summary>
        /// Destroys the instance of the <see cref="NXTBrick"/> class.
        /// </summary>
        /// 
        ~NXTBrick( )
        {
            Disconnect( );
        }

        /// <summary>
        /// Connect to NXT brick.
        /// </summary>
        /// 
        /// <param name="portName">Serial port name to use for communication, for example COM1.</param>
        /// 
        /// <returns>Returns <b>true</b> on successful connection or <b>false</b>
        /// otherwise.</returns>
        /// 
        /// <remarks>If connection to NXT brick was established before the call, existing connection will be reused.
        /// If it is required to force reconnection, then <see cref="Disconnect"/> method should be called before.
        /// </remarks>
        /// 
        public bool Connect( string portName )
        {
            lock ( sync )
            {
                if ( communicationInterface != null )
                    return true;

                // create communication interface,
                communicationInterface = new SerialCommunication( portName );
                // connect and check if NXT is alive
                if ( ( communicationInterface.Connect( ) ) && ( IsAlive( ) ) )
                    return true;

                Disconnect( );
            }
            return false;
        }

        /// <summary>
        /// Disconnect from Lego NXT brick.
        /// </summary>
        /// 
        public void Disconnect( )
        {
            lock ( sync )
            {
                if ( communicationInterface != null )
                {
                    communicationInterface.Disconnect( );
                    communicationInterface = null;
                }
            }
        }

        /// <summary>
        /// Check if the NXT brick is alive and responds to messages.
        /// </summary>
        /// 
        /// <returns>Returns <b>true</b> if device is alive or <b>false</b> otherwise.</returns>
        /// 
        /// <remarks>The command also keeps NXT brick alive preventing it from sleep.</remarks>
        /// 
        public bool IsAlive( )
        {
            return SendCommand( new byte[] { (byte) NXTCommandType.DirectCommand,
                (byte) NXTDirectCommand.KeepAlive }, new byte[7] );
        }

        /// <summary>
        /// Play tone of specified frequency.
        /// </summary>
        /// 
        /// <param name="frequency">Tone frequency in Hz.</param>
        /// <param name="duration">Tone duration in milliseconds.</param>
        /// 
        /// <returns>Returns <b>true</b> if device is alive or <b>false</b> otherwise.</returns>
        /// 
        public bool PlayTone( short frequency, short duration )
        {
            return PlayTone( frequency, duration, true );
        }

        /// <summary>
        /// Play tone of specified frequency.
        /// </summary>
        /// 
        /// <param name="frequency">Tone frequency in Hz.</param>
        /// <param name="duration">Tone duration in milliseconds.</param>
        /// <param name="waitReply">Wait reply from NXT (safer option) or not (faster option).</param>
        /// 
        /// <returns>Returns <b>true</b> if device is alive or <b>false</b> otherwise.</returns>
        /// 
        public bool PlayTone( short frequency, short duration, bool waitReply )
        {
            byte[] command = new byte[6];

            // prepare command
            command[0] = (byte) ( ( waitReply ) ? NXTCommandType.DirectCommand : NXTCommandType.DirectCommandWithoutReply );
            command[1] = (byte) NXTDirectCommand.PlayTone;
            command[2] = (byte) ( frequency & 0xFF );
            command[3] = (byte) ( frequency >> 8 );
            command[4] = (byte) ( duration & 0xFF );
            command[5] = (byte) ( duration >> 8 );

            // execute command
            return SendCommand( command, new byte[3] );
        }

        /// <summary>
        /// Get firmware version of NXT brick.
        /// </summary>
        /// 
        /// <param name="protocolVersion">Protocol version number.</param>
        /// <param name="firmwareVersion">Firmware version number.</param>
        /// 
        /// <returns>Returns <b>true</b> if command was executed successfully or <b>false</b> otherwise.</returns>
        ///
        public bool GetVersion( out string protocolVersion, out string firmwareVersion )
        {
            byte[] reply = new byte[7];

            if ( SendCommand( new byte[] { (byte) NXTCommandType.SystemCommand,
                (byte) NXTSystemCommand.GetFirmwareVersion }, reply ) )
            {
                protocolVersion = string.Format( "{0}.{1}", reply[4], reply[3] );
                firmwareVersion = string.Format( "{0}.{1}", reply[6], reply[5] );
                return true;
            }

            protocolVersion = null;
            firmwareVersion = null;

            return false;
        }

        /// <summary>
        /// Get information about NXT device.
        /// </summary>
        /// 
        /// <param name="deviceName">Device name.</param>
        /// <param name="btAddress">Bluetooth address.</param>
        /// <param name="btSignalStrength">Bluetooth signal strength.</param>
        /// <param name="freeUserFlash">Free user Flash.</param>
        /// 
        /// <returns>Returns <b>true</b> if command was executed successfully or <b>false</b> otherwise.</returns>
        ///
        public bool GetDeviceInformation( out string deviceName, out byte[] btAddress, out int btSignalStrength, out int freeUserFlash )
        {
            byte[] reply = new byte[33];

            if ( SendCommand( new byte[] { (byte) NXTCommandType.SystemCommand,
                (byte) NXTSystemCommand.GetDeviceInfo }, reply ) )
            {
                // devince name
                deviceName = System.Text.ASCIIEncoding.ASCII.GetString( reply, 3, 15 );
                // Bluetooth address
                btAddress = new byte[7];
                Array.Copy( reply, 18, btAddress, 0, 7 );
                // Bluetooth signal strength
                btSignalStrength = reply[25] | ( reply[26] << 8 ) |
                    ( reply[27] << 16 ) | ( reply[28] << 24 );
                // free user Flash
                freeUserFlash = reply[29] | ( reply[30] << 8 ) |
                    ( reply[31] << 16 ) | ( reply[32] << 24 );

                return true;
            }

            deviceName = null;
            btAddress = null;
            btSignalStrength = 0;
            freeUserFlash = 0;

            return false;
        }

        /// <summary>
        /// Get battery power of NXT brick.
        /// </summary>
        /// 
        /// <param name="power">NXT brick's battery power in millivolts.</param>
        /// 
        /// <returns>Returns <b>true</b> if command was executed successfully or <b>false</b> otherwise.</returns>
        /// 
        public bool GetBatteryPower( out int power )
        {
            byte[] reply = new byte[5];

            if ( SendCommand( new byte[] { (byte) NXTCommandType.DirectCommand,
                (byte) NXTDirectCommand.GetBatteryLevel }, reply ) )
            {
                power = reply[3] | ( reply[4] << 8 );
                return true;
            }

            power = 0;

            return false;
        }

        /// <summary>
        /// Set name of NXT device.
        /// </summary>
        /// 
        /// <param name="deviceName">Device name to set for the brick.</param>
        /// 
        /// <returns>Returns <b>true</b> if command was executed successfully or <b>false</b> otherwise.</returns>
        /// 
        public bool SetBrickName( string deviceName )
        {
            byte[] command = new byte[18];

            // prepare message
            command[0] = (byte) NXTCommandType.SystemCommand;
            command[1] = (byte) NXTSystemCommand.SetBrickName;
            // convert string to bytes
            System.Text.ASCIIEncoding.ASCII.GetBytes( deviceName, 0, Math.Min( deviceName.Length, 14 ), command, 2 );

            return SendCommand( command, new byte[3] );
        }

        /// <summary>
        /// Reset motor's position.
        /// </summary>
        /// 
        /// <param name="motor">Motor to reset.</param>
        /// <param name="relative">Specifies if relative (to last movement) or absolute motor's
        /// position should reset.</param>
        ///
        /// <returns>Returns <b>true</b> if command was executed successfully or <b>false</b> otherwise.</returns>
        /// 
        public bool ResetMotorPosition( Motor motor, bool relative )
        {
            return ResetMotorPosition( motor, relative, true );
        }

        /// <summary>
        /// Reset motor's position.
        /// </summary>
        /// 
        /// <param name="motor">Motor to reset.</param>
        /// <param name="relative">Specifies if relative (to last movement) or absolute motor's
        /// position should reset.</param>
        /// <param name="waitReply">Wait reply from NXT (safer option) or not (faster option).</param>
        ///
        /// <returns>Returns <b>true</b> if command was executed successfully or <b>false</b> otherwise.</returns>
        /// 
        public bool ResetMotorPosition( Motor motor, bool relative, bool waitReply )
        {
            byte[] command = new byte[4];

            // prepare message
            command[0] = (byte) ( ( waitReply ) ? NXTCommandType.DirectCommand : NXTCommandType.DirectCommandWithoutReply );
            command[1] = (byte) NXTDirectCommand.ResetMotorPosition;
            command[2] = (byte) motor;
            command[3] = (byte) ( ( relative ) ? 0xFF : 0x00 ); // reset relative or absolute position

            return SendCommand( command, new byte[3] );
        }

        /// <summary>
        /// Set motor state.
        /// </summary>
        /// 
        /// <param name="motor">Motor to set state for.</param>
        /// <param name="state">Motor's state to set.</param>
        /// 
        /// <returns>Returns <b>true</b> if command was executed successfully or <b>false</b> otherwise.</returns>
        /// 
        public bool SetMotorState( Motor motor, MotorState state )
        {
            return SetMotorState( motor, state, true );
        }

        /// <summary>
        /// Set motor state.
        /// </summary>
        /// 
        /// <param name="motor">Motor to set state for.</param>
        /// <param name="state">Motor's state to set.</param>
        /// <param name="waitReply">Wait reply from NXT (safer option) or not (faster option).</param>
        /// 
        /// <returns>Returns <b>true</b> if command was executed successfully or <b>false</b> otherwise.</returns>
        /// 
        public bool SetMotorState( Motor motor, MotorState state, bool waitReply )
        {
            byte[] command = new byte[12];

            // prepare message
            command[0] = (byte) ( ( waitReply ) ? NXTCommandType.DirectCommand : NXTCommandType.DirectCommandWithoutReply );
            command[1] = (byte) NXTDirectCommand.SetOutputState;
            command[2] = (byte) motor;
            command[3] = (byte) state.Power;
            command[4] = (byte) state.Mode;
            command[5] = (byte) state.Regulation;
            command[6] = (byte) state.TurnRatio;
            command[7] = (byte) state.RunState;
            // tacho limit
            command[8]  = (byte) ( state.TachoLimit & 0xFF );
            command[9]  = (byte) ( ( state.TachoLimit >> 8 ) & 0xFF );
            command[10] = (byte) ( ( state.TachoLimit >> 16 ) & 0xFF );
            command[11] = (byte) ( ( state.TachoLimit >> 24 ) & 0xFF );

            return SendCommand( command, new byte[3] );
        }

        /// <summary>
        /// Get motor state.
        /// </summary>
        /// 
        /// <param name="motor">Motor to get state for.</param>
        /// <param name="state">Motor's state.</param>
        /// 
        /// <returns>Returns <b>true</b> if command was executed successfully or <b>false</b> otherwise.</returns>
        /// 
        public bool GetMotorState( Motor motor, out MotorState state )
        {
            state = new MotorState( );

            // check motor port
            if ( motor == Motor.All )
            {
                throw new ArgumentException( "Motor state can be retrieved for one motor only" );
            }

            byte[] command = new byte[3];
            byte[] reply = new byte[25];

            // prepare message
            command[0] = (byte) NXTCommandType.DirectCommand;
            command[1] = (byte) NXTDirectCommand.GetOutputState;
            command[2] = (byte) motor;

            if ( SendCommand( command, reply ) )
            {
                state.Power      = (sbyte) reply[4];
                state.Mode       = (MotorMode) reply[5];
                state.Regulation = (MotorRegulationMode) reply[6];
                state.TurnRatio  = (sbyte) reply[7];
                state.RunState   = (MotorRunState) reply[8];

                // tacho limit
                state.TachoLimit = reply[9] | ( reply[10] << 8 ) |
                        ( reply[11] << 16 ) | ( reply[12] << 24 );
                // tacho count
                state.TachoCount = reply[13] | ( reply[14] << 8 ) |
                        ( reply[15] << 16 ) | ( reply[16] << 24 );
                // block tacho count
                state.BlockTachoCount = reply[17] | ( reply[18] << 8 ) |
                        ( reply[19] << 16 ) | ( reply[20] << 24 );
                // rotation count
                state.RotationCount = reply[21] | ( reply[22] << 8 ) |
                        ( reply[23] << 16 ) | ( reply[24] << 24 );

                return true;
            }

            return false;
        }

        /// <summary>
        /// Set sensor's type and mode.
        /// </summary>
        /// 
        /// <param name="sensor">Sensor to set type of.</param>
        /// <param name="type">Sensor's type.</param>
        /// <param name="mode">Sensor's mode.</param>
        /// 
        /// <returns>Returns <b>true</b> if command was executed successfully or <b>false</b> otherwise.</returns>
        /// 
        public bool SetSensorMode( Sensor sensor, SensorType type, SensorMode mode )
        {
            return SetSensorMode( sensor, type, mode, true );
        }

        /// <summary>
        /// Set sensor's type and mode.
        /// </summary>
        /// 
        /// <param name="sensor">Sensor to set type of.</param>
        /// <param name="type">Sensor's type.</param>
        /// <param name="mode">Sensor's mode.</param>
        /// <param name="waitReply">Wait reply from NXT (safer option) or not (faster option).</param>
        /// 
        /// <returns>Returns <b>true</b> if command was executed successfully or <b>false</b> otherwise.</returns>
        /// 
        public bool SetSensorMode( Sensor sensor, SensorType type, SensorMode mode, bool waitReply )
        {
            byte[] command = new byte[5];

            // prepare message
            command[0] = (byte) ( ( waitReply ) ? NXTCommandType.DirectCommand : NXTCommandType.DirectCommandWithoutReply );
            command[1] = (byte) NXTDirectCommand.SetInputMode;
            command[2] = (byte) sensor;
            command[3] = (byte) type;
            command[4] = (byte) mode;

            return SendCommand( command, new byte[3] );
        }

        /// <summary>
        /// Get sensor's values.
        /// </summary>
        /// 
        /// <param name="sensor">Sensor to get values of.</param>
        /// <param name="sensorValues">etrieved sensor's values.</param>
        /// 
        /// <returns>Returns <b>true</b> if command was executed successfully or <b>false</b> otherwise.</returns>
        /// 
        public bool GetSensorValue( Sensor sensor, out SensorValues sensorValues )
        {
            byte[] command = new byte[3];
            byte[] reply = new byte[16];

            sensorValues = new SensorValues( );

            // prepare message
            command[0] = (byte) NXTCommandType.DirectCommand;
            command[1] = (byte) NXTDirectCommand.GetInputValues;
            command[2] = (byte) sensor;

            if ( SendCommand( command, reply ) )
            {
                sensorValues.IsValid      = ( reply[4] != 0 );
                sensorValues.IsCalibrated = ( reply[5] != 0 );
                sensorValues.SensorType   = (SensorType) reply[6];
                sensorValues.SensorMode   = (SensorMode) reply[7];
                sensorValues.Raw          = (ushort) ( reply[8] | ( reply[9] << 8 ) );
                sensorValues.Normalized   = (ushort) ( reply[10] | ( reply[11] << 8 ) );
                sensorValues.Scaled       = (short) ( reply[12] | ( reply[13] << 8 ) );
                sensorValues.Calibrated   = (short) ( reply[14] | ( reply[15] << 8 ) );

                return true;
            }

            return false;
        }

        /// <summary>
        /// Clear sensor's scaled value. 
        /// </summary>
        /// 
        /// <param name="sensor">Sensor to clear value of.</param>
        /// 
        /// <returns>Returns <b>true</b> if command was executed successfully or <b>false</b> otherwise.</returns>
        /// 
        public bool ClearSensor( Sensor sensor )
        {
            return ClearSensor( sensor, true );
        }

        /// <summary>
        /// Clear sensor's scaled value. 
        /// </summary>
        /// 
        /// <param name="sensor">Sensor to clear value of.</param>
        /// <param name="waitReply">Wait reply from NXT (safer option) or not (faster option).</param>
        /// 
        /// <returns>Returns <b>true</b> if command was executed successfully or <b>false</b> otherwise.</returns>
        /// 
        public bool ClearSensor( Sensor sensor, bool waitReply )
        {
            byte[] command = new byte[3];

            // prepare message
            command[0] = (byte) ( ( waitReply ) ? NXTCommandType.DirectCommand : NXTCommandType.DirectCommandWithoutReply );
            command[1] = (byte) NXTDirectCommand.ResetInputScaledValue;
            command[2] = (byte) sensor;

            return SendCommand( command, new byte[3] );
        }
        
        /// <summary>
        /// Get status of Low Speed bus.
        /// </summary>
        /// 
        /// <param name="sensor">Sensor to get the status from.</param>
        /// <param name="readyBytes">Number of bytes that are ready to be read from the bus.</param>
        /// 
        /// <returns>Returns <b>true</b> if command was executed successfully or <b>false</b> otherwise.</returns>
        /// 
        public bool LsGetStatus( Sensor sensor, out int readyBytes )
        {
            byte[] command = new byte[3];
            byte[] reply = new byte[4];

            // prepare message
            command[0] = (byte) NXTCommandType.DirectCommand;
            command[1] = (byte) NXTDirectCommand.LsGetStatus;
            command[2] = (byte) sensor;

            if ( SendCommand( command, reply ) )
            {
                readyBytes = reply[3];
                return true;
            }

            readyBytes = -1;
            return false;
        }

        /// <summary>
        /// Write to Low Speed bus.
        /// </summary>
        /// 
        /// <param name="sensor">Sensor to write to.</param>
        /// <param name="data">Data to send to the I2C device.</param>
        /// <param name="expectedBytes">Number of bytes expected from device on reply, [0..16].
        /// Can be set to zero if I2C command does not suppose any reply.</param>
        /// 
        /// <returns>Returns <b>true</b> if command was executed successfully or <b>false</b> otherwise.</returns>
        /// 
        /// <exception cref="ArgumentException">Data length must be in the [1..16] range.</exception>
        /// 
        public bool LsWrite( Sensor sensor, byte[] data, int expectedBytes )
        {
            return LsWrite( sensor, data, expectedBytes, true );
        }

        /// <summary>
        /// Write to Low Speed bus.
        /// </summary>
        /// 
        /// <param name="sensor">Sensor to write to.</param>
        /// <param name="data">Data to send to the I2C device.</param>
        /// <param name="expectedBytes">Number of bytes expected from device on reply, [0..16].
        /// Can be set to zero if I2C command does not suppose any reply.</param>
        /// <param name="waitReply">Wait reply from NXT (safer option) or not (faster option).</param>
        /// 
        /// <returns>Returns <b>true</b> if command was executed successfully or <b>false</b> otherwise.</returns>
        /// 
        /// <exception cref="ArgumentException">Data length must be in the [1..16] range.</exception>
        /// 
        public bool LsWrite( Sensor sensor, byte[] data, int expectedBytes, bool waitReply )
        {
            if ( ( data.Length == 0 ) || ( data.Length > 16 ) )
            {
                throw new ArgumentException( "Data length must be in the [1..16] range.", "data" );
            }

            byte[] command = new byte[5 + data.Length];
            byte[] reply = new byte[3];

            // prepare message
            command[0] = (byte) ( ( waitReply ) ? NXTCommandType.DirectCommand : NXTCommandType.DirectCommandWithoutReply );
            command[1] = (byte) NXTDirectCommand.LsWrite;
            command[2] = (byte) sensor;

            command[3] = (byte) data.Length;
            command[4] = (byte) expectedBytes;

            Array.Copy( data, 0, command, 5, data.Length );

            return ( SendCommand( command, reply ) );
        }

        /// <summary>
        /// Read data from Low Speed bus.
        /// </summary>
        /// 
        /// <param name="sensor">Sensor to read data from.</param>
        /// <param name="readValues">Array to read data to.</param>
        /// <param name="bytesRead">Bytes actually read from I2C device.</param>
        /// 
        /// <returns>Returns <b>true</b> if command was executed successfully or <b>false</b> otherwise.</returns>
        /// 
        public bool LsRead( Sensor sensor, byte[] readValues, out int bytesRead )
        {
            byte[] command = new byte[3];
            byte[] reply = new byte[20];

            // prepare message
            command[0] = (byte) NXTCommandType.DirectCommand;
            command[1] = (byte) NXTDirectCommand.LsRead;
            command[2] = (byte) sensor;

            if ( SendCommand( command, reply ) )
            {
                bytesRead = reply[3];
                Array.Copy( reply, 4, readValues, 0, Math.Min( readValues.Length, bytesRead ) );
                return true;
            }

            bytesRead = -1;
            return false;
        }

        /// <summary>
        /// Read value of ultrasonic distance sensor.
        /// </summary>
        /// 
        /// <param name="sensor">Sensor to read value from.</param>
        /// <param name="value">Distance value obtained from ultrasonic sensor, [0..255] cm.</param>
        /// 
        /// <returns>Returns <b>true</b> if command was executed successfully or <b>false</b> otherwise.</returns>
        /// 
        /// <remarks><para>The method retrieves value of ultrasonic distance sensor by
        /// communicating with I2C device (writing to and reading from low speed bus).
        /// The method first sends { 0x02, 0x42 } command to the specified device using
        /// <see cref="LsWrite( Sensor, byte[], int )"/> method. Then it waits until there is something available
        /// to read using <see cref="LsGetStatus"/> method. Finally it reads sensor's value
        /// using <see cref="LsRead"/> device. See
        /// <a href="http://hsrc.static.net/Research/NXT%20I2C%20Communication/">this page</a>
        /// for details.</para>
        /// 
        /// <para><note>Before using this method it is required to use
        /// <see cref="SetSensorMode( Sensor, SensorType, SensorMode, bool )"/> method to set sensor's type to
        /// <see cref="SensorType.Lowspeed9V"/> mode. It should be done
        /// once after NXT brick is powered on. If sensor's type is not set properly,
        /// the method will generate an exception. Also after setting sensor's
        /// type application may need to wait a bit to give device some time
        /// to initialize.</note></para>
        /// </remarks>
        /// 
        public bool GetUltrasonicSensorsValue( Sensor sensor, out int value )
        {
            value = -1;

            // request distance value
            if ( !LsWrite( sensor, new byte[] { 0x02, 0x42 }, 1 ) )
                return false;

            int readyBytes = -1;

            for ( int i = 0; i < 10; i++ )
            {
                if ( !LsGetStatus( sensor, out readyBytes ) )
                    return false;

                if ( readyBytes >= 1 )
                {
                    // read from I2C device
                    byte[] readValues = new byte[1];
                    int bytesRead;

                    if ( !LsRead( sensor, readValues, out bytesRead ) )
                        return false;

                    value = readValues[0];

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// The event is raised every time a command is sent successfully.
        /// </summary>
        /// 
        public event MessageTransferHandler MessageSent;

        /// <summary>
        /// The event is raised every time a command is read successfully.
        /// </summary>
        /// 
        public event MessageTransferHandler MessageRead;

        /// <summary>
        /// Send command to Lego NXT brick and read reply.
        /// </summary>
        /// 
        /// <param name="command">Command to send.</param>
        /// <param name="reply">Buffer to receive reply into.</param>
        /// 
        /// <returns>Returns <b>true</b> if the command was sent successfully and reply was
        /// received, otherwise <b>false</b>.</returns>
        /// 
        /// <exception cref="NullReferenceException">Communication can not be performed, because connection with
        /// NXT brick was not established yet.</exception>
        /// <exception cref="ArgumentException">Reply buffer size is smaller than the reply data size.</exception>
        /// <exception cref="ApplicationException">Reply does not correspond to command (second byte of reply should
        /// be equal to second byte of command).</exception>
        /// <exception cref="ApplicationException">Error occurred on NXT brick side.</exception>
        /// 
        public bool SendCommand( byte[] command, byte[] reply )
        {
            bool result = false;

            lock ( sync )
            {
                // check connection
                if ( communicationInterface == null )
                {
                    throw new NullReferenceException( "Not connected to NXT brick" );
                }

                // send message to NXT brick
                if ( communicationInterface.SendMessage( command, command.Length ) )
                {
                    // notifies clients if any
                    if ( MessageSent != null )
                    {
                        MessageSent( this, new CommunicationBufferEventArgs( command ) );
                    }

                    if ( ( command[0] == (byte) NXTCommandType.DirectCommandWithoutReply ) ||
                         ( command[1] == (byte) NXTCommandType.SystemCommandWithoutReply ) )
                    {
                        result = true;
                    }
                    else
                    {
                        int bytesRead;

                        // read message
                        if ( communicationInterface.ReadMessage( reply, out bytesRead ) )
                        {
                            // notifies clients if any
                            if ( MessageRead != null )
                            {
                                MessageRead( this, new CommunicationBufferEventArgs( reply, 0, bytesRead ) );
                            }

                            // check that reply corresponds to command
                            if ( reply[1] != command[1] )
                                throw new ApplicationException( "Reply does not correspond to command" );

                            // check for errors
                            if ( reply[2] != 0 )
                            {
                                if ( reply[2] == 221 )
                                {
                                    throw new ApplicationException( "It seems that a wrong sensor type is connected to the corresponding port" );
                                }
                                else
                                {
                                    throw new ApplicationException( "Error occurred in NXT brick. Error code: " + reply[2].ToString( ) );
                                }
                            }

                            result = true;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Read data from HiTechnic color sensor (also color sensor v2).
        /// </summary>
        /// 
        /// <param name="sensor">Sensor to read from.</param>
        /// <param name="colorNumber"><a href="http://www.hitechnic.com/contents/media/Color%20Number.jpg">Found color number.</a></param>
        /// <param name="redValue">Found red value.</param>
        /// <param name="greenValue">Found green value.</param>
        /// <param name="blueValue">Found blue value.</param>
        /// 
        /// <returns>Returns <b>true</b> if the command was sent successfully and reply was
        /// received, otherwise <b>false</b>.</returns>
        /// 
        /// <remarks><para>The method retrieves the color valuse of a <a href="http://www.hitechnic.com/products/">HiTechnic color sensor</a>
        /// by communicating with I2C device (writing to and reading from low speed bus).
        /// The method first sends { 0x02, 0x42 } command to the specified device using
        /// <see cref="LsWrite( Sensor, byte[], int )"/> method. Then it waits until there is something available
        /// to read using <see cref="LsGetStatus"/> method. Finally it reads sensor's value
        /// using <see cref="LsRead"/> device. See
        /// <a href="http://hsrc.static.net/Research/NXT%20I2C%20Communication/">this page</a>
        /// for details.</para>
        /// 
        /// <para><note>Before using this method it is required to use
        /// <see cref="SetSensorMode( Sensor, SensorType, SensorMode, bool )"/> method to set sensor's type to
        /// <see cref="SensorType.Lowspeed"/> mode. It should be done
        /// once after NXT brick is powered on. If sensor's type is not set properly,
        /// the method will generate an exception. Also after setting sensor's
        /// type application may need to wait a bit to give device some time
        /// to initialize.</note></para>
        /// 
        /// <para><note>NXT Firmware version 1.24 must be loaded in the NXT for the HiTechnic color sensor to operate correctly.
        /// You can check the firmware version using the <see cref="GetVersion"/> method.</note></para>
        /// 
        /// <para><note>The color sensor V2 must be configured to match the mains electricity frequency for your
        /// country. Details on how to configure the Color Sensor V2 can be found at
        /// <a href="http://www.hitechnic.com/colorsensor"></a></note></para>
        /// </remarks>
        /// 
        public bool ReadHiTechnicColorSensor( NXTBrick.Sensor sensor, ref int colorNumber, ref int redValue, ref int greenValue, ref int blueValue )
        {
            byte[] command = { 0x02, 0x42 };
            byte[] readBuffer = new byte[4];

            int bytesReady;
            int bytesRead;

            LsWrite( sensor, command, readBuffer.Length );
            LsGetStatus( sensor, out bytesReady );
            LsRead( sensor, readBuffer, out bytesRead );

            if ( bytesRead == readBuffer.Length )
            {
                colorNumber = readBuffer[0];
                redValue    = readBuffer[1];
                greenValue  = readBuffer[2];
                blueValue   = readBuffer[3];
                return true;
            }

            return false;
        }

        /// <summary>
        /// Read data from HiTechnic compass sensor.
        /// </summary>
        /// 
        /// <param name="sensor">Sensor to read from.</param>
        /// <param name="angle">The magnetic heading, [0, 359] degrees.</param>
        /// 
        /// <returns>Returns <b>true</b> if the command was sent successfully and reply was
        /// received, otherwise <b>false</b>.</returns>
        /// 
        /// <remarks><para>The method retrieves the angle of a <a href="http://www.hitechnic.com/products/">
        /// HiTechnic compass sensor</a> by
        /// communicating with I2C device (writing to and reading from low speed bus).
        /// The method first sends { 0x02, 0x42 } command to the specified device using
        /// <see cref="LsWrite( Sensor, byte[], int )"/> method. Then it waits until there is something available
        /// to read using <see cref="LsGetStatus"/> method. Finally it reads sensor's value
        /// using <see cref="LsRead"/> device. See
        /// <a href="http://hsrc.static.net/Research/NXT%20I2C%20Communication/">this page</a>
        /// for details.</para>
        /// 
        /// <para><note>Before using this method it is required to use
        /// <see cref="SetSensorMode( Sensor, SensorType, SensorMode, bool )"/> method to set sensor's type to
        /// <see cref="SensorType.Lowspeed"/> mode. It should be done
        /// once after NXT brick is powered on. If sensor's type is not set properly,
        /// the method will generate an exception. Also after setting sensor's
        /// type application may need to wait a bit to give device some time
        /// to initialize.</note></para>
        /// 
        /// <para><note>The HiTechnic compass sensor will only operate correctly in a horizontal plane so you must keep the compass
        /// level for it to read correctly. This is very important so remember this when you build it into your robot.
        /// It is highly desirable to mount the compass at least 6 inches (15cm) away from the motors and 4 inches (10cm) away from the NXT brick
        /// itself. Try to make sure it is firmly mounted, if it bounces around, the readings may bounce around too.
        /// </note></para>
        /// 
        /// <para><note>NXT Firmware version 1.03 must be loaded in the NXT for the compass to operate correctly. You can check the firmware version
        /// using the <see cref="GetVersion"/> method.</note></para>
        /// </remarks>
        /// 
        public bool ReadHiTechnicCompassSensor( NXTBrick.Sensor sensor, ref int angle )
        {
            byte[] command = { 0x02, 0x42 };
            byte[] readBuffer = new byte[2];

            int bytesReady;
            int bytesRead;

            LsWrite( sensor, command, readBuffer.Length );
            LsGetStatus( sensor, out bytesReady );
            LsRead( sensor, readBuffer, out bytesRead );

            if ( bytesRead == readBuffer.Length )
            {
                angle = ( readBuffer[0] * 2 ) + readBuffer[1];
                return true;
            }

            return false;
        }

        /// <summary>
        /// Read data from HiTechnic acceleration/tilt sensor. The HiTechnic accelerometer/tilt sensor measures acceleration in 
        /// three axes. It measures also tilt along each axis. Using the sensor, you can measure the acceleration of your robot in the range
        /// of -2g to 2g.
        /// </summary>
        /// 
        /// <param name="sensor">Sensor to read from.</param>
        /// <param name="xAceeleration">Acceleration in X direction, with a scaling of approximately 200 counts per g.</param>
        /// <param name="yAceeleration">Acceleration in Y direction, with a scaling of approximately 200 counts per g.</param>
        /// <param name="zAceeleration">Acceleration in Z direction, with a scaling of approximately 200 counts per g.</param>
        /// 
        /// <returns>Returns <b>true</b> if the command was sent successfully and reply was
        /// received, otherwise <b>false</b>.</returns>
        /// 
        /// <remarks><para>The method retrieves the acceleration in three directions of a
        /// <a href="http://www.hitechnic.com/products/"> HiTechnic acceleration/tilt sensor</a> by
        /// communicating with I2C device (writing to and reading from low speed bus).
        /// The method first sends { 0x02, 0x42 } command to the specified device using
        /// <see cref="LsWrite( Sensor, byte[], int )"/> method. Then it waits until there is something available
        /// to read using <see cref="LsGetStatus"/> method. Finally it reads sensor's value
        /// using <see cref="LsRead"/> device. See
        /// <a href="http://hsrc.static.net/Research/NXT%20I2C%20Communication/">this page</a>
        /// for details.</para>
        /// 
        /// <para><note>Before using this method it is required to use
        /// <see cref="SetSensorMode( Sensor, SensorType, SensorMode, bool )"/> method to set sensor's type to
        /// <see cref="SensorType.Lowspeed"/> mode. It should be done
        /// once after NXT brick is powered onq If sensor's type is not set properly,
        /// the method will generate an exception. Also after setting sensor's
        /// type application may need to wait a bit to give device some time
        /// to initialize.</note></para>
        /// 
        /// <para>The acceleration sensor can also be used to measure tilt in three axes This is possible because gravity is perceived
        /// as acceleration. When the sensor is stationary and in the normal horizontal position, the x and y axis will be near 
        /// zero, because they are horizontal, while the z axis will be near 200, which represents g. If you tilt the sensor then 
        /// gravity will also be detected on the other axis and the value for the z axis will go down. Since gravity is distributed
        /// among the three component vectors, the tilt of the sensor can be determined.</para>
        ///
        /// <para><note>NXT Firmware version 1.05 or later must be loaded in the NXT for the acceleration/tilt sensor and other digital I2C
        /// sensors to operate correctly. You can check the firmware version using the <see cref="GetVersion"/> method.</note></para>
        /// </remarks>
        /// 
        public bool ReadHiTechnicAccelerationTiltSensor( NXTBrick.Sensor sensor, ref int xAceeleration, ref int yAceeleration, ref int zAceeleration )
        {
            byte[] command = { 0x02, 0x42 };
            byte[] readBuffer = new byte[6];

            int intReady;
            int bytesRead;

            LsWrite( sensor, command, readBuffer.Length );
            LsGetStatus( sensor, out intReady );
            LsRead( sensor, readBuffer, out bytesRead );

            if ( bytesRead == readBuffer.Length )
            {
                xAceeleration = readBuffer[0] > 127 ? ( readBuffer[0] - 256 ) * 4 + readBuffer[3] : readBuffer[0] * 4 + readBuffer[3];
                yAceeleration = readBuffer[1] > 127 ? ( readBuffer[1] - 256 ) * 4 + readBuffer[4] : readBuffer[1] * 4 + readBuffer[4];
                zAceeleration = readBuffer[2] > 127 ? ( readBuffer[2] - 256 ) * 4 + readBuffer[5] : readBuffer[2] * 4 + readBuffer[5];

                return true;
            }

            return false;
        }
    }
}
