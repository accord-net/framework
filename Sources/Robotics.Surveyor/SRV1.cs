// AForge Surveyor Robotics Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace AForge.Robotics.Surveyor
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using AForge;

    /// <summary>
    /// Manipulation of Surveyor SRV-1 Blackfin robot/camera.
    /// </summary>
    ///
    /// <remarks>
    /// <para>The class allows to manipulate with <a href="http://www.surveyor.com/SRV_info.html">Surveyor SRV-1 Blackfin Robot</a>
    /// - getting video from its camera, manipulating motors and servos,
    /// reading ultrasonic modules' values, sending direct commands, etc.</para>
    /// 
    /// <para><img src="img/robotics/srv1-robot.jpg" width="240" height="216" /></para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// SRV1 srv = new SRV1( );
    /// // connect to SRV-1 robot
    /// srv.Connect( "169.254.0.10", 10001 );
    /// // stop motors
    /// srv.StopMotors( );
    /// // set video resolution and quality
    /// srv.SetQuality( 7 );
    /// srv.SetResolution( SRV1.VideoResolution.Small );
    /// // get version string
    /// string version = srv.GetVersion( );
    /// 
    /// // get robot's camera
    /// SRV1Camera camera = srv.GetCamera( );
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
    ///
    public class SRV1
    {
        // dummy object to lock for synchronization
        private object sync = new object( );

        /// <summary>
        /// Enumeration of predefined motors' commands.
        /// </summary>
        /// 
        /// <remarks><para>This enumeration defines set of motors' commands, which can
        /// be executed using <see cref="ControlMotors"/> method.</para>
        /// 
        /// <para><note>Controlling SRV-1 motors with these commands is only possible
        /// after at least one direct motor command is sent, which is done using <see cref="StopMotors"/> or
        /// <see cref="RunMotors"/> methods.</note></para>
        /// 
        /// <para><note>The <b>IncreaseSpeed</b> and <b>DecreaseSpeed</b> commands do not have any effect
        /// unless another driving command is sent. In other words, these do not increase/decrease speed of
        /// current operation, but affect speed of all following commands.</note></para>
        /// 
        /// <para><note>The <b>RotateLeft</b> and <b>RotateRight</b> commands may be useful only for the original
        /// <a href="http://www.surveyor.com/SRV_info.html">Surveyor SRV-1 Blackfin Robot</a>.
        /// For most of other robots, which may have different motors and moving base, these commands
        /// will not be accurate – will not rotate for 20 degrees.
        /// </note></para>
        /// </remarks>
        /// 
        public enum MotorCommand
        {
            /// <summary>
            /// Robot drive forward.
            /// </summary>
            DriveForward = '8',
            /// <summary>
            /// Robot drive back.
            /// </summary>
            DriveBack = '2',
            /// <summary>
            /// Robot drive left.
            /// </summary>
            DriveLeft = '4',
            /// <summary>
            /// Robot drive right.
            /// </summary>
            DriveRight = '6',
            /// <summary>
            /// Robot drift left.
            /// </summary>
            DriftLeft = '7',
            /// <summary>
            /// Robot drift right.
            /// </summary>
            DriftRight = '9',
            /// <summary>
            /// Robot stop.
            /// </summary>
            Stop = '5',
            /// <summary>
            /// Robot drive back and right.
            /// </summary>
            DriveBackRight = '3',
            /// <summary>
            /// Robot drive back and left.
            /// </summary>
            DriveBackLeft = '1',

            /// <summary>
            /// Robot rotate left 20 degrees.
            /// </summary>
            /// 
            RotateLeft = '0',

            /// <summary>
            /// Robot rotate right 20 degrees.
            /// </summary>
            /// 
            RotateRight = '.',
            
            /// <summary>
            /// Increase motors' speed.
            /// </summary>
            ///
            IncreaseSpeed = '+',

            /// <summary>
            /// Decrease motors' speed.
            /// </summary>
            /// 
            DecreaseSpeed = '-',
        }

        /// <summary>
        /// Enumeration of Surveyor SRV-1 Blackfin cameras resolutions.
        /// </summary>
        public enum VideoResolution
        {
            /// <summary>
            /// 160x120
            /// </summary>
            Tiny = 'a',
            /// <summary>
            /// 320x240
            /// </summary>
            Small = 'b',
            /// <summary>
            /// 640x480
            /// </summary>
            Medium = 'c',
            /// <summary>
            /// 1280x1024
            /// </summary>
            Large = 'd'
        }

        private IPEndPoint endPoint = null;

        // Connecton end-point
        internal IPEndPoint EndPoint
        {
            get { return endPoint; }
        }

        // socket used for communication with SVS
        Socket socket = null;
        // background communicaton thread
        private Thread thread = null;
        // event signaling thread to exit
        private ManualResetEvent stopEvent = null;
        // event signaling about available request in communication queue
        private AutoResetEvent requestIsAvailable;
        // event sugnaling about available response
        private AutoResetEvent replyIsAvailable;

        // last processed request which requires reply
        private CommunicationRequest lastRequestWithReply;

        // communication request
        private class CommunicationRequest
        {
            public byte[] Request;
            public byte[] ResponseBuffer;
            public int    BytesRead; // -1 on error

            public CommunicationRequest( byte[] request )
            {
                this.Request = request;
            }
            public CommunicationRequest( byte[] request, byte[] responseBuffer )
            {
                this.Request = request;
                this.ResponseBuffer = responseBuffer;
            }
        }

        // communication queue
        Queue<CommunicationRequest> communicationQueue = new Queue<CommunicationRequest>( );

        // SRV-1 camera
        private SRV1Camera camera;

        /// <summary>
        /// SRV-1 host address.
        /// </summary>
        /// 
        /// <remarks><para>The property keeps SRV-1 IP address if the class is connected
        /// to SRV-1 Blackfin robot/camera, otherwise it equals to <see langword="null."/>.</para></remarks>
        ///
        public string HostAddress
        {
            get { return ( endPoint == null ) ? null : endPoint.Address.ToString( ); }
        }

        /// <summary>
        /// SRV-1 port number.
        /// </summary>
        /// 
        /// <remarks><para>The property keeps SRV-1 port number if the class is connected
        /// to SRV-1 Blackfin robot/camera, otherwise it equals to 0.</para></remarks>
        ///
        public int Port
        {
            get { return ( endPoint == null ) ? 0 : endPoint.Port; }
        }

        /// <summary>
        /// Connection state.
        /// </summary>
        /// 
        /// <remarks><para>The property equals to <see langword="true"/> if the class is connected
        /// to SRV-1 Blackfin robot/camera, otherwise it equals to <see langword="false"/>.</para>
        /// 
        /// <para><note>The property is not updated by the class, when connection was lost or
        /// communication failure was detected (which results into <see cref="ConnectionLostException"/>
        /// exception). The property only shows status of <see cref="Connect"/> method.</note></para>
        /// </remarks>
        /// 
        public bool IsConnected
        {
            get { return ( endPoint != null ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SRV1"/> class.
        /// </summary>
        /// 
        public SRV1( )
        {
        }

        /// <summary>
        /// Connect to SRV-1 Blackfin robot/camera.
        /// </summary>
        /// 
        /// <param name="ip">IP address of SRV-1 robot.</param>
        /// <param name="port">Port number to connect to.</param>
        /// 
        /// <remarks><para>The method establishes connection to SRV-1 Blackfin robot/camera.
        /// If it succeeds then other methods can be used to manipulate the robot.</para>
        /// 
        /// <para><note>The method calls <see cref="Disconnect"/> before making any connection
        /// attempts to make sure previous connection is closed.</note></para>
        /// </remarks>
        /// 
        /// <exception cref="ConnectionFailedException">Failed connecting to SRV-1.</exception>
        /// 
        public void Connect( string ip, int port )
        {
            Disconnect( );

            lock ( sync )
            {
                try
                {
                    // make sure communication queue is empty
                    communicationQueue.Clear( );

                    endPoint = new IPEndPoint( IPAddress.Parse( ip ), Convert.ToInt16( port ) );
                    // create TCP/IP socket and set timeouts
                    socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
                    socket.ReceiveTimeout = 5000;
                    socket.SendTimeout    = 1000;

                    // connect to SVS
                    socket.Connect( endPoint );

                    // create events
                    stopEvent = new ManualResetEvent( false );

                    requestIsAvailable = new AutoResetEvent( false );
                    replyIsAvailable   = new AutoResetEvent( false );

                    // create and start new thread
                    thread = new Thread( new ThreadStart( CommunicationThread ) );
                    thread.Start( );
                }
                catch ( SocketException )
                {
                    socket.Close( );
                    socket   = null;
                    endPoint = null;

                    throw new ConnectionFailedException( "Failed connecting to SRV-1." );
                }
            }
        }

        /// <summary>
        /// Disconnect from SRV-1 Blackfin robot.
        /// </summary>
        /// 
        /// <remarks><para>The method disconnects from SRV-1 robot making all other methods
        /// unavailable (except <see cref="Connect"/> method). In the case if user
        /// obtained instance of camera using <see cref="GetCamera"/> method, the video will
        /// be stopped automatically (and those <see cref="SRV1Camera"/> instances should be discarded).
        /// </para></remarks>
        /// 
        public void Disconnect( )
        {
            lock ( sync )
            {
                if ( thread != null )
                {
                    // signal camera to stop
                    if ( camera != null )
                    {
                        camera.SignalToStop( );
                    }

                    // signal worker thread to stop
                    stopEvent.Set( );
                    requestIsAvailable.Set( );
                    replyIsAvailable.Set( );

                    // finilze the camera
                    if ( camera != null )
                    {
                        // wait for aroung 250 ms
                        for ( int i = 0; ( i < 5 ) && ( camera.IsRunning ); i++ )
                        {
                            System.Threading.Thread.Sleep( 50 );
                        }
                        // abort camera if it can not be stopped
                        if ( camera.IsRunning )
                        {
                            camera.Stop( );
                        }
                        camera = null;
                    }

                    // wait for aroung 1 s
                    for ( int i = 0; ( i < 20 ) && ( thread.Join( 0 ) == false ); i++ )
                    {
                        System.Threading.Thread.Sleep( 50 );
                    }
                    // abort thread if it can not be stopped
                    if ( thread.Join( 0 ) == false )
                    {
                        thread.Abort( );
                    }

                    thread = null;

                    // release events
                    stopEvent.Close( );
                    stopEvent = null;

                    requestIsAvailable.Close( );
                    requestIsAvailable = null;

                    replyIsAvailable.Close( );
                    replyIsAvailable = null;
                }

                if ( socket != null )
                {
                    if ( socket.Connected )
                    {
                        socket.Disconnect( false );
                    }
                    socket.Close( );
                    socket = null;
                    endPoint = null;
                }
            }
        }

        // Try to reconnect to SVS
        private void Reconnect( )
        {
            if ( socket != null )
            {
                if ( socket.Connected )
                {
                    socket.Disconnect( false );
                }
                socket.Close( );

                // create TCP/IP socket and set timeouts
                socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
                socket.ReceiveTimeout = 5000;
                socket.SendTimeout    = 1000;

                // connect to SVS
                socket.Connect( endPoint );
            }
        }

        /// <summary>
        /// Get camera object for the SRV-1 Blackfin robot/camera.
        /// </summary>
        /// 
        /// <returns>Returns <see cref="SRV1Camera"/> object, which is connected to SRV1 Blackfin camera.
        /// Use <see cref="SRV1Camera.Start"/> method to start the camera and start receiving video
        /// frames it.</returns>
        /// 
        /// <remarks><para>The method provides an instance of <see cref="SRV1Camera"/>, which can be used
        /// for receiving continuous video frames from the SRV-1 Blackfin camera.
        /// In the case if only one image is required, the <see cref="GetImage"/> method can be used.</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // get SRV-1 camera
        /// SRV1Camera camera = srv.GetCamera( );
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
        /// <exception cref="NotConnectedException">Not connected to SRV-1. Connect to SRV-1 robot/camera
        /// before using this method.</exception>
        /// 
        public SRV1Camera GetCamera( )
        {
            lock ( sync )
            {
                if ( socket == null )
                {
                    // handle error
                    throw new NotConnectedException( "Not connected to SRV-1." );
                }

                if ( camera == null )
                {
                    camera = new SRV1Camera( this );
                }
                return camera;
            }
        }

        /// <summary>
        /// Enqueue communication request.
        /// </summary>
        /// 
        /// <param name="request">Array of bytes (command) to send to SRV-1 Blackfin robot/camera.</param>
        /// 
        /// <remarks><para>The method puts specified command into communication queue and leaves
        /// immediately. Once internal communication thread becomes free from sending/receiving previous
        /// commands/replies, it will send the queued command.</para>
        /// 
        /// <para>The method is useful for those SRV-1 commands, which does not assume any response data
        /// in the command's reply.</para>
        /// 
        /// <para><note>Since the method only queues a communication request, it does not provide any status
        /// of request's delivery and it does not generate any exceptions on failure.</note></para>
        /// </remarks>
        /// 
        public void Send( byte[] request )
        {
            lock ( communicationQueue )
            {
                communicationQueue.Enqueue( new CommunicationRequest( request ) );
            }
            if ( requestIsAvailable != null )
            {
                requestIsAvailable.Set( );
            }
        }

        /// <summary>
        /// Enqueue communication request and wait for reply.
        /// </summary>
        /// 
        /// <param name="request">Array of bytes (command) to send to SRV-1 Blackfin robot/camera.</param>
        /// <param name="responseBuffer">Buffer to read response into.</param>
        /// 
        /// <returns>Returns total bytes read into the response buffer.</returns>
        /// 
        /// <remarks><para>The method puts specified command into communication queue and waits until
        /// the command is sent to SRV-1 Blackfin robot and reply is received.</para>
        /// 
        /// <para><note>If SRV-1 responds with more data than response buffer can fit, then
        /// the response buffer will take all the data it can store, but the rest of response
        /// will be discarded. The only exception is image request - if response buffer is too
        /// small to fit image response, then <see cref="IndexOutOfRangeException"/> exception
        /// is thrown. It is user's responsibility to provide response buffer of the correct
        /// size. Check definition of the <a href="http://www.surveyor.com/SRV_protocol.html">SRV-1
        /// Control Protocol</a> for information about supported commands and responses.</note></para>
        /// </remarks>
        /// 
        /// <exception cref="NotConnectedException">Not connected to SRV-1.</exception>
        /// <exception cref="ConnectionLostException">Connection lost or communicaton failure.</exception>
        /// <exception cref="IndexOutOfRangeException">Response buffer is too small.</exception>
        /// 
        public int SendAndReceive( byte[] request, byte[] responseBuffer )
        {
            lock ( sync )
            {
                if ( socket == null )
                {
                    // handle error
                    throw new NotConnectedException( "Not connected to SRV-1." );
                }

                lock ( communicationQueue )
                {
                    communicationQueue.Enqueue( new CommunicationRequest( request, responseBuffer ) );
                }
                requestIsAvailable.Set( );

                // waiting for reply
                replyIsAvailable.WaitOne( );

                // no reply since we got disconnect request from user - background thread is exiting
                if ( lastRequestWithReply == null )
                    return 0;

                // get number of bytes read
                int bytesRead = lastRequestWithReply.BytesRead;

                // clean the last reply
                lastRequestWithReply = null;

                if ( bytesRead == -1 )
                {
                    // handle error
                    throw new ConnectionLostException( "Connection lost or communicaton failure." );
                }
                if ( bytesRead == -2 )
                {
                    // handle error
                    throw new IndexOutOfRangeException( "Response buffer is too small." );
                }
                return bytesRead;
            }
        }

        /// <summary>
        /// Get single image from the SRV-1 Blackfin camera.
        /// </summary>
        /// 
        /// <returns>Returns image received from the SRV-1 Blackfin camera or <see langword="null"/>
        /// if failed decoding provided response.</returns>
        /// 
        /// <remarks><para>The method provides single video frame retrieved from the SRV-1 Blackfin
        /// camera. However in many cases it is required to receive video frames one after another, so
        /// the <see cref="GetCamera"/> method is more preferred for continuous video frames.</para></remarks>
        /// 
        /// <exception cref="NotConnectedException">Not connected to SRV-1. Connect to SRV-1 before using
        /// this method.</exception>
        /// <exception cref="ConnectionLostException">Connection lost or communicaton failure. Try to reconnect.</exception>
        /// 
        public Bitmap GetImage( )
        {
            Bitmap image = null;

            // buffer to read image into
            byte[] buffer = new byte[768 * 1024];
            // request image
            int bytesRead = SendAndReceive( new byte[] { (byte) 'I' }, buffer );

            if ( bytesRead > 10 )
            {
                // check for image reply signature
                if (
                    ( buffer[0] == (byte) '#' ) &&
                    ( buffer[1] == (byte) '#' ) &&
                    ( buffer[2] == (byte) 'I' ) &&
                    ( buffer[3] == (byte) 'M' ) &&
                    ( buffer[4] == (byte) 'J' ) )
                {
                    // extract image size
                    int imageSize = System.BitConverter.ToInt32( buffer, 6 );

                    try
                    {
                        // decode image from memory stream
                        image = (Bitmap) Bitmap.FromStream( new MemoryStream( buffer, 10, imageSize ) );
                    }
                    catch
                    {
                        image = null;
                    }
                }
            }
             
            return image;
        }

        /// <summary>
        /// Get SRV-1 firmware version string.
        /// </summary>
        /// 
        /// <returns>Returns SRV-1 version string.</returns>
        /// 
        /// <exception cref="NotConnectedException">Not connected to SRV-1. Connect to SRV-1 before using
        /// this method.</exception>
        /// <exception cref="ConnectionLostException">Connection lost or communicaton failure. Try to reconnect.</exception>
        /// 
        public string GetVersion( )
        {
            byte[] response = new byte[100];

            int read = SendAndReceive( new byte[] { (byte) 'V' }, response );

            string str = System.Text.ASCIIEncoding.ASCII.GetString( response, 0, read );

            str = str.Replace( "##Version -", "" );
            str = str.Trim( );

            return str;
        }

        /// <summary>
        /// Get SRV-1 running time.
        /// </summary>
        /// 
        /// <returns>Returns SRV-1 running time in milliseconds.</returns>
        /// 
        /// <exception cref="NotConnectedException">Not connected to SRV-1. Connect to SRV-1 before using
        /// this method.</exception>
        /// <exception cref="ConnectionLostException">Connection lost or communicaton failure. Try to reconnect.</exception>
        /// <exception cref="ApplicationException">Failed parsing response from SRV-1.</exception>
        /// 
        public long GetRunningTime( )
        {
            byte[] response = new byte[100];

            int read = SendAndReceive( new byte[] { (byte) 't' }, response );

            string str = System.Text.ASCIIEncoding.ASCII.GetString( response, 0, read );

            str = str.Replace( "##time - millisecs:", "" );
            str = str.Trim( );

            try
            {
                return long.Parse( str );
            }
            catch
            {
                throw new ApplicationException( "Failed parsing response from SRV-1." );
            }
        }

        /// <summary>
        /// Run motors connected to SRV-1 robot.
        /// </summary>
        /// 
        /// <param name="leftSpeed">Left motor's speed, [-127, 127].</param>
        /// <param name="rightSpeed">Right motor's speed, [-127, 127].</param>
        /// <param name="duration">Time duration to run motors measured in number
        /// of 10 milliseconds (0 for infinity), [0, 255].</param>
        /// 
        /// <remarks><para>The method provides direct access to motors setting specified,
        /// speed to both motors connected to the SRV-1 robot. The maximum absolute speed
        /// equals to 127, but the sign specifies direction of motor's rotation (forward or backward).
        /// </para>
        /// 
        /// <para><note>The method sends 'Mabc' SRV-1 command (see <a href="http://www.surveyor.com/SRV_protocol.html">SRV-1
        /// Control Protocol</a>), which uses 2<sup>nd</sup> and 3<sup>rd</sup> timers for
        /// controlling motors/servos.</note></para>
        /// </remarks>
        /// 
        public void RunMotors( int leftSpeed, int rightSpeed, int duration )
        {
            // check limits
            if ( leftSpeed == -128 )
                leftSpeed = -127;
            if ( rightSpeed == -128 )
                rightSpeed = -127;
            if ( duration > 255 )
                duration = 255;

            Send( new byte[] { (byte) 'M', (byte) leftSpeed, (byte) rightSpeed, (byte) duration } );
        }

        /// <summary>
        /// Stop both motors.
        /// </summary>
        /// 
        /// <remarks><para>The method stops both motors connected to the SRV-1 robot by calling
        /// <see cref="RunMotors"/> method specifying 0 for motors' speed.</para></remarks>
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
        /// by SRV-1 robot withing 2 seconds, motors' speed will be set to the specified values. The command
        /// is very useful to instruct robot to stop if no other commands were sent
        /// within 2 last seconds (probably lost connection).</para></remarks>
        ///
        public void EnableFailsafeMode( int leftSpeed, int rightSpeed )
        {
            // check limits
            if ( leftSpeed == -128 )
                leftSpeed = -127;
            if ( rightSpeed == -128 )
                rightSpeed = -127;

            Send( new byte[] { (byte) 'F', (byte) leftSpeed, (byte) rightSpeed } );
        }

        /// <summary>
        /// Disable fail safe mode.
        /// </summary>
        /// 
        /// <remarks><para>The method disable fail safe mode, which was set using
        /// <see cref="EnableFailsafeMode"/> method.</para></remarks>
        /// 
        public void DisableFailsafeMode( )
        {
            Send( new byte[] { (byte) 'f' } );
        }

        /// <summary>
        /// Direct servos control of SRV-1 robot.
        /// </summary>
        /// 
        /// <param name="leftServo">Left servo setting, [0, 100].</param>
        /// <param name="rightServo">Right servo setting, [0, 100].</param>
        /// 
        /// <remarks><para>Servo settings represent timing pulse widths ranging
        /// from 1ms to 2ms. 0 corresponds to a 1ms pulse, 100 corresponds to a 2ms pulse,
        /// and 50 is midrange with a 1.5ms pulse.</para>
        /// 
        /// <para><note>The method sends 'sab' SRV-1 command (see <a href="http://www.surveyor.com/SRV_protocol.html">SRV-1
        /// Control Protocol</a>), which controls 2<sup>nd</sup> bank of servos
        /// using 6<sup>th</sup> and 7<sup>th</sup> timers.</note></para>
        /// </remarks>
        /// 
        public void ControlServos( int leftServo, int rightServo )
        {
            // check limts
            if ( leftServo > 100 )
                leftServo = 100;
            if ( rightServo > 100 )
                rightServo = 100;

            Send( new byte[] { (byte) 's', (byte) leftServo, (byte) rightServo } );
        }

        /// <summary>
        /// Control SRV-1 robot's motors using predefined commands.
        /// </summary>
        /// 
        /// <param name="command">Motor command to send to the SRV-1 Blackfin robot.</param>
        /// 
        /// <remarks><para><note>Controlling SRV-1 motors with this method is only available
        /// after at least one direct motor command is sent, which is done using <see cref="StopMotors"/> or
        /// <see cref="RunMotors"/> methods.</note></para></remarks>
        /// 
        public void ControlMotors( MotorCommand command )
        {
            Send( new byte[] { (byte) command } );
        }

        /// <summary>
        /// Set video quality.
        /// </summary>
        /// 
        /// <param name="quality">Video quality to set, [1, 8].</param>
        ///
        /// <remarks><para>The method sets video quality, which is specified in [1, 8] range - 1 is
        /// the highest quality level, 8 is the lowest quality level.</para>
        /// 
        /// <para><note>Setting higher quality level and <see cref="SetResolution">resolution</see>
        /// may increase delays for other requests sent to SRV-1. So if
        /// robot is used not only for video, but also for controlling servos/motors, and higher
        /// response level is required, then do not set very high quality and resolution.
        /// </note></para>
        /// </remarks>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">Invalid quality level was specified.</exception>
        ///
        public void SetQuality( int quality )
        {
            if ( ( quality < 1 ) || ( quality > 8 ) )
                throw new ArgumentOutOfRangeException( "Invalid quality level was specified." );

            Send( new byte[] { (byte) 'q', (byte) ( quality + (byte) '0' ) } );
        }

        /// <summary>
        /// Set video resolution.
        /// </summary>
        /// 
        /// <param name="resolution">Video resolution to set.</param>
        /// 
        /// <remarks>
        /// <para><note>Setting higher <see cref="SetQuality">quality level</see> and resolution
        /// may increase delays for other requests sent to SRV-1. So if
        /// robot is used not only for video, but also for controlling servos/motors, and higher
        /// response level is required, then do not set very high quality and resolution.
        /// </note></para>
        /// </remarks>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">Invalid resolution was specified.</exception>
        ///
        public void SetResolution( VideoResolution resolution )
        {
            if ( !Enum.IsDefined( typeof( VideoResolution ), resolution ) )
            {
                throw new ArgumentException( "Invalid resolution was specified." );
            }

            Send( new byte[] { (byte) resolution } );
        }

        /// <summary>
        /// Flip video capture or not (for use with upside-down camera).
        /// </summary>
        /// 
        /// <param name="isFlipped">Specifies if video should be flipped (<see langword="true"/>),
        /// or not (<see langword="false"/>).</param>
        /// 
        public void FlipVideo( bool isFlipped )
        {
            Send( new byte[] { (byte) ( ( isFlipped ) ? 'y' : 'Y' ) } );
        }

        /// <summary>
        /// Ping ultrasonic ranging modules.
        /// </summary>
        /// 
        /// <returns>Returns array of ranges (distances) obtained from ultrasonic sensors. The ranges
        /// are measured in inches.</returns>
        /// 
        /// <remarks><para>The method sends 'p' SRV-1 command (see <a href="http://www.surveyor.com/SRV_protocol.html">SRV-1
        /// Control Protocol</a>), which gets values from ultrasonic ranging modules attached to
        /// pins 27, 28, 29, 30 with trigger on pin 18. Supports Maxbotics EZ0 and EZ1 ultrasonic modules.
        /// </para></remarks>
        /// 
        /// <exception cref="NotConnectedException">Not connected to SRV-1. Connect to SRV-1 before using
        /// this method.</exception>
        /// <exception cref="ConnectionLostException">Connection lost or communicaton failure. Try to reconnect.</exception>
        /// <exception cref="ApplicationException">Failed parsing response from SRV-1.</exception>
        /// 
        public float[] UltrasonicPing( )
        {
            byte[] response = new byte[100];

            int    read = SendAndReceive( new byte[] { (byte) 'p' }, response );
            string str = System.Text.ASCIIEncoding.ASCII.GetString( response, 0, read );

            str = str.Replace( "##ping ", "" );
            str = str.Trim( );

            // split string into separate values
            string[] strs = str.Split( ' ' );

            try
            {
                float[] distance = new float[4];

                for ( int i = 0; i < 4; i++ )
                {
                    distance[i] = (float) int.Parse( strs[i] ) / 100f;
                }

                return distance;
            }
            catch
            {
                throw new ApplicationException( "Failed parsing response from SRV-1." );
            }
        }

        /// <summary>
        /// Read byte from I2C device.
        /// </summary>
        /// 
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
        /// <exception cref="NotConnectedException">Not connected to SRV-1. Connect to SRV-1 before using
        /// this method.</exception>
        /// <exception cref="ConnectionLostException">Connection lost or communicaton failure. Try to reconnect.</exception>
        /// <exception cref="ApplicationException">Failed parsing response from SRV-1.</exception>
        /// 
        public byte I2CReadByte( byte deviceID, byte register )
        {
            byte[] response = new byte[100];

            int    read = SendAndReceive( new byte[] { (byte) 'i', (byte) 'r', deviceID, register }, response );
            string str  = System.Text.ASCIIEncoding.ASCII.GetString( response, 0, read );

            try
            {
                str = str.Trim( );
                // split string into separate values
                string[] strs = str.Split( ' ' );

                return byte.Parse( strs[1] );
            }
            catch
            {
                throw new ApplicationException( "Failed parsing response from SRV-1." );
            }
        }

        /// <summary>
        /// Read word from I2C device.
        /// </summary>
        /// 
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
        /// <exception cref="NotConnectedException">Not connected to SRV-1. Connect to SRV-1 before using
        /// this method.</exception>
        /// <exception cref="ConnectionLostException">Connection lost or communicaton failure. Try to reconnect.</exception>
        /// <exception cref="ApplicationException">Failed parsing response from SRV-1.</exception>
        /// 
        public ushort I2CReadWord( byte deviceID, byte register )
        {
            byte[] response = new byte[100];

            int    read = SendAndReceive( new byte[] { (byte) 'i', (byte) 'R', deviceID, register }, response );
            string str  = System.Text.ASCIIEncoding.ASCII.GetString( response, 0, read );

            try
            {
                str = str.Trim( );
                // split string into separate values
                string[] strs = str.Split( ' ' );

                return ushort.Parse( strs[1] );
            }
            catch
            {
                throw new ApplicationException( "Failed parsing response from SRV-1." );
            }
        }

        /// <summary>
        /// Write byte to I2C device.
        /// </summary>
        /// 
        /// <param name="deviceID">I2C device ID (7 bit notation).</param>
        /// <param name="register">I2C device register to write to.</param>
        /// <param name="byteToWrite">Byte to write to the specified register of the specified device.</param>
        /// 
        /// <para><note>The IC2 device ID should be specified in 7 bit notation. This means that low bit of the ID
        /// is not used for specifying read/write mode as in 8 bit notation. For example, if I2C device IDs are 0x44 for reading
        /// and 0x45 for writing in 8 bit notation, then it equals to 0x22 device ID in 7 bit notation.
        /// </note></para>
        /// 
        /// <exception cref="NotConnectedException">Not connected to SRV-1. Connect to SRV-1 before using
        /// this method.</exception>
        /// <exception cref="ConnectionLostException">Connection lost or communicaton failure. Try to reconnect.</exception>
        /// 
        public void I2CWriteByte( byte deviceID, byte register, byte byteToWrite )
        {
            byte[] response = new byte[100];

            // use SendAndReceive() to make sure the command was executed successfully
            int read = SendAndReceive( new byte[] { (byte) 'i', (byte) 'W', deviceID, register, byteToWrite }, response );
        }

        /// <summary>
        /// Write two bytes to I2C device.
        /// </summary>
        /// 
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
        /// <exception cref="NotConnectedException">Not connected to SRV-1. Connect to SRV-1 before using
        /// this method.</exception>
        /// <exception cref="ConnectionLostException">Connection lost or communicaton failure. Try to reconnect.</exception>
        /// 
        public void I2CWriteWord( byte deviceID, byte register, byte firstByteToWrite, byte secondByteToWrite )
        {
            byte[] response = new byte[100];

            // use SendAndReceive() to make sure the command was executed successfully
            int read = SendAndReceive( new byte[] { (byte) 'i', (byte) 'W', deviceID, register,
                firstByteToWrite, secondByteToWrite }, response );
        }

        // portion size to read at once
        private const int readSize = 1024;

        // Communication thread
        private void CommunicationThread( )
        {
            bool lastRequestFailed = false;

            while ( !stopEvent.WaitOne( 0, false ) )
            {
                // wait for any request
                requestIsAvailable.WaitOne( );

                while ( !stopEvent.WaitOne( 0, false ) )
                {
                    // get next communication request from queue
                    CommunicationRequest cr = null;

                    lock ( communicationQueue )
                    {
                        if ( communicationQueue.Count == 0 )
                            break;
                        cr = communicationQueue.Dequeue( );
                    }

                    try
                    {
                        // try to reconnect if we had communication issues on the last request
                        if ( lastRequestFailed )
                        {
                            Reconnect( );
                            lastRequestFailed = false;
                        }

                        if ( cr.Request[0] != (byte) 'I' )
                        {
                            // System.Diagnostics.Debug.WriteLine( ">> " +
                            //    System.Text.ASCIIEncoding.ASCII.GetString( cr.Request ) );
                        }

                        // send request
                        socket.Send( cr.Request );

                        // read response
                        if ( cr.ResponseBuffer != null )
                        {
                            int bytesToRead = Math.Min( readSize, cr.ResponseBuffer.Length );

                            // receive first portion
                            cr.BytesRead = socket.Receive( cr.ResponseBuffer, 0, bytesToRead, SocketFlags.None );

                            // check if response contains image
                            if ( ( cr.BytesRead > 10 ) &&
                                 ( cr.ResponseBuffer[0] == (byte) '#' ) &&
                                 ( cr.ResponseBuffer[1] == (byte) '#' ) &&
                                 ( cr.ResponseBuffer[2] == (byte) 'I' ) &&
                                 ( cr.ResponseBuffer[3] == (byte) 'M' ) &&
                                 ( cr.ResponseBuffer[4] == (byte) 'J' ) )
                            {
                                // extract image size
                                int imageSize = System.BitConverter.ToInt32( cr.ResponseBuffer, 6 );

                                bytesToRead = imageSize + 10 - cr.BytesRead;

                                if ( bytesToRead > cr.ResponseBuffer.Length - cr.BytesRead )
                                {
                                    // response buffer is too small
                                    throw new IndexOutOfRangeException( );
                                }

                                // read the rest
                                while ( !stopEvent.WaitOne( 0, false ) )
                                {
                                    int read = socket.Receive( cr.ResponseBuffer, cr.BytesRead,
                                        Math.Min( readSize, bytesToRead ), SocketFlags.None );

                                    cr.BytesRead += read;
                                    bytesToRead  -= read;

                                    if ( bytesToRead == 0 )
                                        break;
                                }
                            }
                            else
                            {
                                // commenting check for new line presence, because not all replies
                                // which start with '##' have new line in the end.
                                // this SRV-1 text based protocol drives me crazy.

                                /*
                                if ( ( cr.BytesRead >= 2 ) &&
                                     ( cr.ResponseBuffer[0] == (byte) '#' ) &&
                                     ( cr.ResponseBuffer[1] == (byte) '#' ) )
                                {
                                    int bytesChecked = 2;

                                    while ( cr.BytesRead != cr.ResponseBuffer.Length )
                                    {
                                        // ensure we got end of line for variable length replies
                                        bool endLineWasFound = false;

                                        for ( int n = cr.BytesRead - 1; bytesChecked < n; bytesChecked++ )
                                        {
                                            if ( ( ( cr.ResponseBuffer[bytesChecked]     == '\n' ) &&
                                                   ( cr.ResponseBuffer[bytesChecked + 1] == '\r' ) ) ||
                                                 ( ( cr.ResponseBuffer[bytesChecked]     == '\r' ) &&
                                                   ( cr.ResponseBuffer[bytesChecked + 1] == '\n' ) ) )
                                            {
                                                endLineWasFound = true;
                                                break;
                                            }
                                        }

                                        if ( ( endLineWasFound ) || stopEvent.WaitOne( 0, false ) )
                                            break;

                                        // read more
                                        bytesToRead = Math.Min( readSize, cr.ResponseBuffer.Length - cr.BytesRead );

                                        cr.BytesRead += socket.Receive( cr.ResponseBuffer, cr.BytesRead,
                                            bytesToRead, SocketFlags.None );
                                    }
                                }
                                */
                            }

                            // check if there is still something to read
                            // because of small buffer given by user
                            if ( socket.Available != 0 )
                            {
                                DiscardIncomingData( socket, stopEvent );
                            }


                            // System.Diagnostics.Debug.WriteLine( "<< (" + cr.BytesRead + ") " +
                            //     System.Text.ASCIIEncoding.ASCII.GetString( cr.ResponseBuffer, 0, Math.Min( 5, cr.BytesRead ) ) );
                        }
                        else
                        {
                            // read reply and throw it away, since nobody wants it
                            DiscardIncomingData( socket, stopEvent );
                        }
                    }

                    catch ( IndexOutOfRangeException )
                    {
                        cr.BytesRead = -2; // too small buffer
                    }
                    catch
                    {
                        if ( lastRequestFailed )
                        {
                            // wait a bit if we have 2 consequent failures
                            Thread.Sleep( 500 );
                        }

                        lastRequestFailed = true;
                        cr.BytesRead = -1; // communication failure
                    }
                    finally
                    {
                        // signal about available response to the waiting caller
                        if ( ( stopEvent != null ) && ( !stopEvent.WaitOne( 0, false ) ) && ( cr.ResponseBuffer != null ) )
                        {
                            lastRequestWithReply = cr;
                            replyIsAvailable.Set( );
                        }
                    }
                }
            }
        }

        private void DiscardIncomingData( Socket socket, ManualResetEvent stopEvent  )
        {
            byte[] buffer = new byte[100];

            while ( !stopEvent.WaitOne( 0, false ) )
            {
                int read = socket.Receive( buffer, 0, 100, SocketFlags.None );

                if ( socket.Available == 0 )
                {
                    // System.Diagnostics.Debug.WriteLine( "<< (" + read + ") " +
                    //     System.Text.ASCIIEncoding.ASCII.GetString( buffer, 0, Math.Min( 100, read ) ) );

                    break;
                }
            }
        }
    }
}
