// AForge Surveyor Robotics Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2007-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Robotics.Surveyor
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// Manipulation of Surveyor SVS (Stereo Vision System) board.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>The class allows to manipulate with <a href="http://www.surveyor.com/stereo/stereo_info.html">Surveyor SVS</a>
    /// board (stereo vision system) - getting video from both cameras, manipulating motors and servos,
    /// reading ultrasonic modules' values, sending direct commands, etc.</para>
    /// 
    /// <para><img src="img/robotics/svs.jpg" width="320" height="189" /></para>
    /// 
    /// <para>This class essentially creates to instances of <see cref="SRV1"/> class to communicate
    /// with both SVS's cameras (ports 10001 and 10002 are used) and directs all calls through them.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// SVS svs = new SVS( );
    /// // connect to SVS board
    /// svs.Connect( "169.254.0.10" );
    /// // stop motors
    /// svs.StopMotors( );
    /// // set video resolution and quality
    /// svs.SetQuality( 7 );
    /// svs.SetResolution( SRV1.VideoResolution.Small );
    /// // get version string
    /// string version = svs.GetVersion( );
    /// 
    /// // get left camera
    /// SRV1Camera camera = svs.GetCamera( SVS.Camera.Left );
    /// 
    /// // set NewFrame event handler
    /// camera.NewFrame += new NewFrameEventHandler( video_NewFrame );
    /// // start the video source
    /// camera.Start( );
    /// // ...
    /// 
    /// private void video_NewFrame( object sender, NewFrameEventArgs eventArgs )
    /// {
    ///     // get new frame
    ///     Bitmap bitmap = eventArgs.Frame;
    ///     // process the frame
    /// }
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="SRV1Camera"/>
    /// <seealso cref="SRV1"/>
    /// 
    public partial class SVS
    {
        /// <summary>
        /// Enumeration of SVS's Blackfin cameras.
        /// </summary>
        /// 
        /// <remarks><para>Since SVS board consists of two SRV-1 Blackfin cameras, the enumeration
        /// is used by different methods to specify which one to access.</para></remarks>
        /// 
        public enum Camera
        {
            /// <summary>
            /// Left camera (default port number is 10000).
            /// </summary>
            Left,
            /// <summary>
            /// Right camera (default port number is 10001).
            /// </summary>
            Right
        }

        /// <summary>
        /// Enumeration of SVS's servos' banks.
        /// </summary>
        public enum ServosBank
        {
            /// <summary>
            /// First bank of the first (<see cref="Camera.Left"/>) SRV-1 Blackfin camera,
            /// timers 2 and 3 (marked as TMR2-1 and TMR3-1 on the SVS board). Note: these
            /// timers on SVS board are supposed for controlling motors by default
            /// (see <see cref="RunMotors"/> and <see cref="ControlMotors"/>), so use 0th
            /// servos bank only when you've done proper configuration changes on SVS side.
            /// </summary>
            Bank0,

            /// <summary>
            /// Second bank of the first (<see cref="Camera.Left"/>) SRV-1 Blackfin camera,
            /// timers 6 and 7 (marked as TMR6-1 and TMR7-1 on the SVS board).
            /// </summary>
            Bank1,

            /// <summary>
            /// First bank of the second (<see cref="Camera.Right"/>) SRV-1 Blackfin camera,
            /// timers 2 and 3 (marked as TMR2-2 and TMR3-2 on the SVS board).
            /// </summary>
            Bank2,

            /// <summary>
            /// Second bank of the second (<see cref="Camera.Right"/>) SRV-1 Blackfin camera,
            /// timers 6 and 7 (marked as TMR6-2 and TMR7-2 on the SVS board).
            /// </summary>
            Bank3
        }

        // host address if connection was established
        private string hostAddress;
        // communicators used for communication with SVS
        private SRV1 communicator1 = null;
        private SRV1 communicator2 = null;
        // SVS cameras
        private SRV1Camera leftCamera;
        private SRV1Camera rightCamera;

        private string sync1 = "1";
        private string sync2 = "1";

        /// <summary>
        /// SVS's host address.
        /// </summary>
        /// 
        /// <remarks><para>The property keeps SVS's IP address if the class is connected
        /// to SVS board, otherwise it equals to <see langword="null."/>.</para></remarks>
        ///
        public string HostAddress
        {
            get { return hostAddress; }
        }

        /// <summary>
        /// Connection state.
        /// </summary>
        /// 
        /// <remarks><para>The property equals to <see langword="true"/> if the class is connected
        /// to SVS board, otherwise it equals to <see langword="false"/>.</para>
        /// 
        /// <para><note>The property is not updated by the class, when connection was lost or
        /// communication failure was detected (which results into <see cref="ConnectionLostException"/>
        /// exception). The property only shows status of <see cref="Connect"/> method.</note></para>
        /// </remarks>
        /// 
        public bool IsConnected
        {
            get { return ( hostAddress != null ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SVS"/> class.
        /// </summary>
        ///
        public SVS( )
        {
        }

        /// <summary>
        /// Connect to SVS board.
        /// </summary>
        /// 
        /// <param name="ipAddress">IP address of SVS board.</param>
        /// 
        /// <remarks><para>The method establishes connection to SVS board. If it succeeds then
        /// other methods can be used to manipulate the board.</para>
        /// 
        /// <para><note>The method calls <see cref="Disconnect"/> before making any connection
        /// attempts to make sure previous connection is closed.</note></para>
        /// </remarks>
        /// 
        /// <exception cref="ConnectionFailedException">Failed connecting to SVS.</exception>
        /// 
        public void Connect( string ipAddress )
        {
            // close previous connection
            Disconnect( );

            lock ( sync1 )
            {
                lock ( sync2 )
                {
                    try
                    {
                        communicator1 = new SRV1( );
                        communicator2 = new SRV1( );

                        communicator1.Connect( ipAddress, 10001 );
                        communicator2.Connect( ipAddress, 10002 );

                        hostAddress = ipAddress;
                    }
                    catch
                    {
                        Disconnect( );

                        throw new ConnectionFailedException( "Failed connecting to SVS." );
                    }
                }
            }
        }

        /// <summary>
        /// Disconnect from SVS device.
        /// </summary>
        /// 
        /// <remarks><para>The method disconnects from SVS board making all other methods
        /// unavailable (except <see cref="Connect"/> method). In the case if user
        /// obtained instance of left or right camera using <see cref="GetCamera"/>
        /// method, the video will be stopped automatically (and those <see cref="SRV1Camera"/>
        /// instances should be discarded).
        /// </para></remarks>
        /// 
        public void Disconnect( )
        {
            lock ( sync1 )
            {
                lock ( sync2 )
                {
                    hostAddress = null;

                    // signal cameras to stop
                    if ( leftCamera != null )
                    {
                        leftCamera.SignalToStop( );
                    }
                    if ( rightCamera != null )
                    {
                        rightCamera.SignalToStop( );
                    }

                    // wait until cameras stop or abort them
                    if ( leftCamera != null )
                    {
                        // wait for aroung 250 ms
                        for ( int i = 0; ( i < 5 ) && ( leftCamera.IsRunning ); i++ )
                        {
                            System.Threading.Thread.Sleep( 50 );
                        }
                        // abort camera if it can not be stopped
                        if ( leftCamera.IsRunning )
                        {
                            leftCamera.Stop( );
                        }
                        leftCamera = null;
                    }
                    if ( rightCamera != null )
                    {
                        // wait for aroung 250 ms
                        for ( int i = 0; ( i < 5 ) && ( rightCamera.IsRunning ); i++ )
                        {
                            System.Threading.Thread.Sleep( 50 );
                        }
                        // abort camera if it can not be stopped
                        if ( rightCamera.IsRunning )
                        {
                            rightCamera.Stop( );
                        }
                        rightCamera = null;
                    }

                    if ( communicator1 != null )
                    {
                        communicator1.Disconnect( );
                        communicator1 = null;
                    }
                    if ( communicator2 != null )
                    {
                        communicator2.Disconnect( );
                        communicator2 = null;
                    }
                }
            }
        }

        /// <summary>
        /// Get SVS's camera.
        /// </summary>
        /// 
        /// <param name="camera">SVS camera to get.</param>
        /// 
        /// <returns>Returns <see cref="SRV1Camera"/> object, which is connected to SVS's Blackfin camera.
        /// Use <see cref="SRV1Camera.Start"/> method to start the camera and start receiving video
        /// frames from it.</returns>
        /// 
        /// <remarks><para>The method provides an instance of <see cref="SRV1Camera"/>, which can be used
        /// for receiving continuous video frames from the SVS board.
        /// In the case if only one image is required, the <see cref="GetImage"/> method can be used.</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // get SRV-1 camera
        /// SRV1Camera camera = svs.GetCamera( SVS.Camera.Left );
        /// // set NewFrame event handler
        /// camera.NewFrame += new NewFrameEventHandler( video_NewFrame );
        /// // start the video source
        /// camera.Start( );
        /// // ...
        /// 
        /// private void video_NewFrame( object sender, NewFrameEventArgs eventArgs )
        /// {
        ///     // get new frame
        ///     Bitmap bitmap = eventArgs.Frame;
        ///     // process the frame
        /// }
        /// </code>
        /// </remarks>
        /// 
        /// <exception cref="NotConnectedException">Not connected to SVS. Connect to SVS board before using
        /// this method.</exception>
        /// 
        public SRV1Camera GetCamera( Camera camera )
        {
            if ( camera == Camera.Left )
            {
                if ( leftCamera == null )
                {
                    leftCamera = SafeGetCommunicator1( ).GetCamera( );
                }
                return leftCamera;
            }

            if ( rightCamera == null )
            {
                rightCamera = SafeGetCommunicator2( ).GetCamera( );
            }
            return rightCamera;
        }

        /// <summary>
        /// Get single image from the SVS board.
        /// </summary>
        /// 
        /// <param name="camera">Camera to get image from.</param>
        /// 
        /// <returns>Returns image received from the specified camera of the SVS board or
        /// <see langword="null"/> if failed decoding provided response.</returns>
        /// 
        /// <remarks><para>The method provides single video frame retrieved from the specified SVS's
        /// camera. However in many cases it is required to receive video frames one after another, so
        /// the <see cref="GetCamera"/> method is more preferred for continuous video frames.</para></remarks>
        ///
        /// <exception cref="NotConnectedException">Not connected to SRV-1. Connect to SRV-1 before using
        /// this method.</exception>
        /// <exception cref="ConnectionLostException">Connection lost or communicaton failure. Try to reconnect.</exception>
        /// 
        public Bitmap GetImage( Camera camera )
        {
            return ( camera == Camera.Left ) ?
                SafeGetCommunicator1( ).GetImage( ) : SafeGetCommunicator2( ).GetImage( );
        }

        /// <summary>
        /// Get direct access to one of the SVS's SRV-1 Blackfin cameras.
        /// </summary>
        /// 
        /// <param name="camera">SRV-1 Blackfin to get direct access to.</param>
        /// 
        /// <returns>Returns <see cref="SRV1"/> object connected to the requested
        /// SRV-1 Blackfin camera.</returns>
        /// 
        /// <remarks><para>The method provides direct access to one of the SVS's SRV-1
        /// Blackfin cameras, so it could be possible to send some direct commands to it
        /// using <see cref="SRV1.Send"/> and <see cref="SRV1.SendAndReceive"/> methods.</para></remarks>
        /// 
        /// <exception cref="NotConnectedException">Not connected to SVS. Connect to SVS board before using
        /// this method.</exception>
        ///
        public SRV1 GetDirectAccessToSRV1( Camera camera )
        {
            return ( camera == Camera.Left ) ? SafeGetCommunicator1( ) : SafeGetCommunicator2( );
        }

        /// <summary>
        /// Get SVS board's firmware version string.
        /// </summary>
        /// 
        /// <returns>Returns SVS's version string.</returns>
        /// 
        /// <exception cref="NotConnectedException">Not connected to SVS. Connect to SVS board before using
        /// this method.</exception>
        /// <exception cref="ConnectionLostException">Connection lost or communicaton failure. Try to reconnect.</exception>
        /// 
        public string GetVersion( )
        {
            string str = SafeGetCommunicator1( ).GetVersion( );

            str = str.Replace( "##Version -", "" );
            str = str.Trim( );

            // remove "(stereo master)" or (stereo slave)" string
            int specificInfoPos = str.IndexOf( " (stereo " );
            if ( specificInfoPos != -1 )
            {
                str = str.Substring( 0, specificInfoPos );
            }

            return str;
        }

        /// <summary>
        /// Get SVS's board's running time.
        /// </summary>
        /// 
        /// <returns>Returns SVS boards running time in milliseconds.</returns>
        /// 
        /// <exception cref="NotConnectedException">Not connected to SVS. Connect to SVS board before using
        /// this method.</exception>
        /// <exception cref="ConnectionLostException">Connection lost or communicaton failure. Try to reconnect.</exception>
        /// <exception cref="ApplicationException">Failed parsing response from SVS.</exception>
        /// 
        public long GetRunningTime( )
        {
            return SafeGetCommunicator1( ).GetRunningTime( );
        }

        /// <summary>
        /// Run motors connected to the SVS board.
        /// </summary>
        /// 
        /// <param name="leftSpeed">Left motor's speed, [-127, 127].</param>
        /// <param name="rightSpeed">Right motor's speed, [-127, 127].</param>
        /// <param name="duration">Time duration to run motors measured in number
        /// of 10 milliseconds (0 for infinity), [0, 255].</param>
        /// 
        /// <remarks><para>The method sets specified speed to both motors connected to
        /// the SVS board. The maximum absolute speed equals to 127, but the sign specifies
        /// direction of motor's rotation (forward or backward).
        /// </para></remarks>
        /// 
        /// <exception cref="NotConnectedException">Not connected to SVS. Connect to SVS board before using
        /// this method.</exception>
        /// 
        public void RunMotors( int leftSpeed, int rightSpeed, int duration )
        {
            SafeGetCommunicator1( ).RunMotors( leftSpeed, rightSpeed, duration );
        }

        /// <summary>
        /// Stop both motors.
        /// </summary>
        /// 
        /// <remarks><para>The method stops both motors connected to the SVS board by calling
        /// <see cref="RunMotors"/> method specifying 0 for motors' speed.</para></remarks>
        /// 
        /// <exception cref="NotConnectedException">Not connected to SVS. Connect to SVS board before using
        /// this method.</exception>
        /// 
        public void StopMotors( )
        {
            RunMotors( 0, 0, 0 );
        }

        /// <summary>
        /// Enables fail safe mode - setting motors' speed after timeout.
        /// </summary>
        /// 
        /// <param name="leftSpeed">Left motor's speed, [-127, 127].</param>
        /// <param name="rightSpeed">Right motor's speed, [-127, 127].</param>
        /// 
        /// <remarks><para>In the case if fail safe mode is enabled and no commands are received
        /// by SVS robot withing 2 seconds, motors' speed will be set to the specified values. The command
        /// is very useful to instruct robot to stop if no other commands were sent
        /// within 2 last seconds (probably lost connection).</para></remarks>
        /// 
        /// <exception cref="NotConnectedException">Not connected to SVS. Connect to SVS board before using
        /// this method.</exception>
        /// 
        public void EnableFailsafeMode( int leftSpeed, int rightSpeed )
        {
            SafeGetCommunicator1( ).EnableFailsafeMode( leftSpeed, rightSpeed );
        }

        /// <summary>
        /// Disable fail safe mode.
        /// </summary>
        /// 
        /// <remarks><para>The method disable fail safe mode, which was set using
        /// <see cref="EnableFailsafeMode"/> method.</para></remarks>
        /// 
        /// <exception cref="NotConnectedException">Not connected to SVS. Connect to SVS board before using
        /// this method.</exception>
        /// 
        public void DisableFailsafeMode( )
        {
            SafeGetCommunicator1( ).DisableFailsafeMode( );
        }

        /// <summary>
        /// Control motors connected to SVS board using predefined commands.
        /// </summary>
        /// 
        /// <param name="command">Motor command to send to the SVS board.</param>
        /// 
        /// <remarks><para><note>Controlling SVS motors with this method is only available
        /// after at least one direct motor command is sent, which is done using <see cref="StopMotors"/> or
        /// <see cref="RunMotors"/> methods.</note></para></remarks>
        /// 
        /// <exception cref="NotConnectedException">Not connected to SVS. Connect to SVS board before using
        /// this method.</exception>
        /// 
        public void ControlMotors( SRV1.MotorCommand command )
        {
            SafeGetCommunicator1( ).ControlMotors( command );
        }

        /// <summary>
        /// Direct servos control of the SVS board.
        /// </summary>
        /// 
        /// <param name="servosBank">SVS's servo bank to control.</param>
        /// <param name="leftServo">Left servo setting, [0, 100].</param>
        /// <param name="rightServo">Right servo setting, [0, 100].</param>
        /// 
        /// <remarks><para>The method performs servos control of the SVS board.
        /// For <see cref="ServosBank.Bank1"/> and <see cref="ServosBank.Bank3"/>
        /// banks it calls <see cref="SRV1.ControlServos"/> method for the corresponding
        /// SRV-1 Blackfin camera. In the case of <see cref="ServosBank.Bank0"/> or <see cref="ServosBank.Bank2"/>,
        /// the method sends 'Sab' SRV-1 command (see <a href="http://www.surveyor.com/SRV_protocol.html">SRV-1
        /// Control Protocol</a>) to the appropriate SRV-1 Blackfin camera.</para>
        /// </remarks>
        /// 
        /// <exception cref="NotConnectedException">Not connected to SVS. Connect to SVS board before using
        /// this method.</exception>
        /// 
        public void ControlServos( ServosBank servosBank, int leftServo, int rightServo )
        {
            switch ( servosBank )
            {
                case ServosBank.Bank0:
                    // check limts
                    if ( leftServo > 100 )
                        leftServo = 100;
                    if ( rightServo > 100 )
                        rightServo = 100;

                    SafeGetCommunicator1( ).Send( new byte[] { (byte) 'S', (byte) leftServo, (byte) rightServo } );
                    break;

                case ServosBank.Bank1:
                    SafeGetCommunicator1( ).ControlServos( leftServo, rightServo );
                    break;

                case ServosBank.Bank2:
                    // check limts
                    if ( leftServo > 100 )
                        leftServo = 100;
                    if ( rightServo > 100 )
                        rightServo = 100;

                    SafeGetCommunicator2( ).Send( new byte[] { (byte) 'S', (byte) leftServo, (byte) rightServo } );
                    break;

                case ServosBank.Bank3:
                    SafeGetCommunicator2( ).ControlServos( leftServo, rightServo );
                    break;
            }
        }

        /// <summary>
        /// Ping ultrasonic ranging modules.
        /// </summary>
        /// 
        /// <param name="camera">SRV-1 Blackfin camera to check ultrasonic modules values.</param>
        /// 
        /// <returns>Returns array of ranges (distances) obtained from ultrasonic sensors. The ranges
        /// are measured in inches.</returns>
        /// 
        /// <remarks><para>The method calls <see cref="SRV1.UltrasonicPing"/> for the specified
        /// SRV-1 Blackfin camera.</para></remarks>
        /// 
        /// <exception cref="NotConnectedException">Not connected to SVS. Connect to SVS board before using
        /// this method.</exception>
        /// <exception cref="ConnectionLostException">Connection lost or communicaton failure. Try to reconnect.</exception>
        /// <exception cref="ApplicationException">Failed parsing response from SVS.</exception>
        /// 
        public float[] UltrasonicPing( Camera camera )
        {
            return GetDirectAccessToSRV1( camera ).UltrasonicPing( );
        }

        /// <summary>
        /// Read byte from I2C device.
        /// </summary>
        /// 
        /// <param name="camera">SRV-1 Blackfin camera to access I2C device on.</param>
        /// <param name="deviceID">I2C device ID (7 bit notation).</param>
        /// <param name="register">I2C device register to read.</param>
        /// 
        /// <returns>Returns byte read from the specified register of the specified I2C device.</returns>
        /// 
        /// <para><note>The IC2 device ID should be specified in 7 bit notation. This means that low bit of the ID
        /// is not used for specifying read/write mode as in 8 bit notation. For example, if I2C device IDs are 0x44 for reading
        /// and 0x45 for writing in 8 bit notation, then it equals to 0x22 device ID in 7 bit notation.
        /// </note></para>
        /// 
        /// <exception cref="NotConnectedException">Not connected to SVS. Connect to SVS board before using
        /// this method.</exception>
        /// <exception cref="ConnectionLostException">Connection lost or communicaton failure. Try to reconnect.</exception>
        /// <exception cref="ApplicationException">Failed parsing response from SRV-1.</exception>
        /// 
        public byte I2CReadByte( Camera camera, byte deviceID, byte register )
        {
            return GetDirectAccessToSRV1( camera ).I2CReadByte( deviceID, register );
        }

        /// <summary>
        /// Read word from I2C device.
        /// </summary>
        /// 
        /// <param name="camera">SRV-1 Blackfin camera to access I2C device on.</param>
        /// <param name="deviceID">I2C device ID (7 bit notation).</param>
        /// <param name="register">I2C device register to read.</param>
        /// 
        /// <returns>Returns word read from the specified register of the specified I2C device.</returns>
        /// 
        /// <para><note>The IC2 device ID should be specified in 7 bit notation. This means that low bit of the ID
        /// is not used for specifying read/write mode as in 8 bit notation. For example, if I2C device IDs are 0x44 for reading
        /// and 0x45 for writing in 8 bit notation, then it equals to 0x22 device ID in 7 bit notation.
        /// </note></para>
        ///
        /// <exception cref="NotConnectedException">Not connected to SVS. Connect to SVS board before using
        /// this method.</exception>
        /// <exception cref="ConnectionLostException">Connection lost or communicaton failure. Try to reconnect.</exception>
        /// <exception cref="ApplicationException">Failed parsing response from SRV-1.</exception>
        /// 
        public ushort I2CReadWord( Camera camera, byte deviceID, byte register )
        {
            return GetDirectAccessToSRV1( camera ).I2CReadWord( deviceID, register );
        }

        /// <summary>
        /// Write byte to I2C device.
        /// </summary>
        /// 
        /// <param name="camera">SRV-1 Blackfin camera to access I2C device on.</param>
        /// <param name="deviceID">I2C device ID (7 bit notation).</param>
        /// <param name="register">I2C device register to write to.</param>
        /// <param name="byteToWrite">Byte to write to the specified register of the specified device.</param>
        /// 
        /// <para><note>The IC2 device ID should be specified in 7 bit notation. This means that low bit of the ID
        /// is not used for specifying read/write mode as in 8 bit notation. For example, if I2C device IDs are 0x44 for reading
        /// and 0x45 for writing in 8 bit notation, then it equals to 0x22 device ID in 7 bit notation.
        /// </note></para>
        /// 
        /// <exception cref="NotConnectedException">Not connected to SVS. Connect to SVS board before using
        /// this method.</exception>
        /// <exception cref="ConnectionLostException">Connection lost or communicaton failure. Try to reconnect.</exception>
        /// 
        public void I2CWriteByte( Camera camera, byte deviceID, byte register, byte byteToWrite )
        {
            GetDirectAccessToSRV1( camera ).I2CWriteByte( deviceID, register, byteToWrite );
        }

        /// <summary>
        /// Write two bytes to I2C device.
        /// </summary>
        /// 
        /// <param name="camera">SRV-1 Blackfin camera to access I2C device on.</param>
        /// <param name="deviceID">I2C device ID (7 bit notation).</param>
        /// <param name="register">I2C device register to write to.</param>
        /// <param name="firstByteToWrite">First byte to write to the specified register of the specified device.</param>
        /// <param name="secondByteToWrite">Second byte to write to the specified register of the specified device.</param>
        /// 
        /// <para><note>The IC2 device ID should be specified in 7 bit notation. This means that low bit of the ID
        /// is not used for specifying read/write mode as in 8 bit notation. For example, if I2C device IDs are 0x44 for reading
        /// and 0x45 for writing in 8 bit notation, then it equals to 0x22 device ID in 7 bit notation.
        /// </note></para>
        /// 
        /// <exception cref="NotConnectedException">Not connected to SVS. Connect to SVS board before using
        /// this method.</exception>
        /// <exception cref="ConnectionLostException">Connection lost or communicaton failure. Try to reconnect.</exception>
        /// 
        public void I2CWriteWord( Camera camera, byte deviceID, byte register, byte firstByteToWrite, byte secondByteToWrite )
        {
            GetDirectAccessToSRV1( camera ).I2CWriteWord( deviceID, register, firstByteToWrite, secondByteToWrite );
        }

        /// <summary>
        /// Set video quality for both cameras.
        /// </summary>
        /// 
        /// <param name="quality">Video quality to set, [1, 8].</param>
        ///
        /// <remarks><para>The method sets video quality for both SVS cameras, which is specified in [1, 8] range - 1 is
        /// the highest quality level, 8 is the lowest quality level.</para>
        /// 
        /// <para><note>Setting higher quality level and <see cref="SetResolution">resolution</see>
        /// may increase delays for other requests sent to SVS. So if
        /// robot is used not only for video, but also for controlling servos/motors, and higher
        /// response level is required, then do not set very high quality and resolution.
        /// </note></para>
        /// </remarks>
        /// 
        /// <exception cref="NotConnectedException">Not connected to SVS. Connect to SVS board before using
        /// this method.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid quality level was specified.</exception>
        ///
        public void SetQuality( int quality )
        {
            SafeGetCommunicator1( ).SetQuality( quality );
            SafeGetCommunicator2( ).SetQuality( quality );
        }

        /// <summary>
        /// Set video resolution for both cameras.
        /// </summary>
        /// 
        /// <param name="resolution">Video resolution to set.</param>
        /// 
        /// <remarks>
        /// <para><note>Setting higher <see cref="SetQuality">quality level</see> and resolution
        /// may increase delays for other requests sent to SVS. So if
        /// robot is used not only for video, but also for controlling servos/motors, and higher
        /// response level is required, then do not set very high quality and resolution.
        /// </note></para>
        /// </remarks>
        /// 
        /// <exception cref="NotConnectedException">Not connected to SVS. Connect to SVS board before using
        /// this method.</exception>
        /// 
        public void SetResolution( SRV1.VideoResolution resolution )
        {
            SafeGetCommunicator1( ).SetResolution( resolution );
            SafeGetCommunicator2( ).SetResolution( resolution );
        }

        /// <summary>
        /// Flip video capture for both cameras or not (for use with upside-down camera).
        /// </summary>
        /// 
        /// <param name="isFlipped">Specifies if video should be flipped (<see langword="true"/>),
        /// or not (<see langword="false"/>).</param>
        /// 
        /// <exception cref="NotConnectedException">Not connected to SVS. Connect to SVS board before using
        /// this method.</exception>
        /// 
        public void FlipVideo( bool isFlipped )
        {
            SafeGetCommunicator1( ).FlipVideo( isFlipped );
            SafeGetCommunicator2( ).FlipVideo( isFlipped );
        }

        // Get first communicator safely
        private SRV1 SafeGetCommunicator1( )
        {
            lock ( sync1 )
            {
                if ( communicator1 == null )
                {
                    throw new NotConnectedException( "Not connected to SVS." );
                }
                return communicator1;
            }
        }

        // Get second communicator safely
        private SRV1 SafeGetCommunicator2( )
        {
            lock ( sync2 )
            {
                if ( communicator2 == null )
                {
                    throw new NotConnectedException( "Not connected to SVS." );
                }
                return communicator2;
            }
        }
    }
}
