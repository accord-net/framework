// AForge Kinect Video Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace AForge.Video.Kinect
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using AForge;

    /// <summary>
    /// The class provides access to Microsoft's Xbox <a href="http://en.wikipedia.org/wiki/Kinect">Kinect</a>
    /// controller.
    /// </summary>
    /// 
    /// <remarks><para>The class allows to manipulate Kinec device by changing its LED color, setting motor's
    /// tilt value and accessing its camera. See <see cref="KinectVideoCamera"/> and <see cref="KinectDepthCamera"/>
    /// classes, which provide access to actual video.</para>
    /// 
    /// <para><img src="img/video/kinect.jpg" width="320" height="140" /></para>
    /// 
    /// <para><note>In order to run correctly the class requires <i>freenect.dll</i> library
    /// to be put into solution's output folder. This can be found within the AForge.NET framework's
    /// distribution in Externals folder.</note></para>
    ///
    /// <para>Sample usage:</para>
    /// <code>
    /// // get Kinect device
    /// Kinect kinectDevice = Kinect.GetDevice( 0 );
    /// // change LED color
    /// kinectDevice.LedColor = LedColorOption.Yellow;
    /// // set motor tilt angle to -10 degrees
    /// kinectDevice.SetMotorTilt( -10 );
    /// // get video camera
    /// KinectVideoCamera videoCamera = kinectDevice.GetVideoCamera( );
    /// 
    /// // see example for video camera also
    /// </code>
    /// </remarks>
    ///
    public class Kinect : IDisposable
    {
        internal delegate void DeviceFailureHandler( );

        private class DeviceContext
        {
            public IntPtr Device;
            public int ReferenceCounter;
            public bool DeviceFailed;
            public KinectNative.TiltState TiltState;

            private object sync = new object( );
            private List<DeviceFailureHandler> failureHanlders = new List<DeviceFailureHandler>( );

            public DeviceContext( IntPtr device )
            {
                Device = device;
                ReferenceCounter = 0;
                DeviceFailed = false;
            }

            public void AddFailureHandler( DeviceFailureHandler handler )
            {
                lock ( sync )
                {
                    failureHanlders.Add( handler );
                }
            }

            public void FireFailureHandlers( )
            {
                lock ( sync )
                {
                    foreach ( DeviceFailureHandler handler in failureHanlders )
                    {
                        handler( );
                    }
                    failureHanlders.Clear( );
                }
            }
        }

        // dictionary of all opened devices
        private static Dictionary<int, DeviceContext> openDevices = new Dictionary<int, DeviceContext>( );

        // thread for Kinect status updates and an event for stopping it
        private static Thread updateStatusThread;
        //private static Thread watchDogThread;
        private static ManualResetEvent stopEvent;

        // pointer to the freekinect's opened device
        private IntPtr rawDevice;
        // ID of the opened device
        private readonly int deviceID;

        // dummy object to lock for synchronization
        private object sync = new object( );

        /// <summary>
        /// ID of the opened Kinect device.
        /// </summary>
        /// 
        public int DeviceID
        {
            get { return deviceID; }
        }

        internal IntPtr RawDevice
        {
            get { return rawDevice; }
        }

        /// <summary>
        /// Number of Kinect devices available in the system.
        /// </summary>
        public static int DeviceCount
        {
            get { return KinectNative.freenect_num_devices( KinectNative.Context ); }
        }

        /// <summary>
        /// Get initialized instance of the Kinect device.
        /// </summary>
        /// 
        /// <param name="deviceID">ID of the Kinect device to get instance of, [0, <see cref="DeviceCount"/>),</param>
        /// 
        /// <returns>Returns initialized Kinect device. Use <see cref="Dispose()"/> method
        /// when the device is no longer required.</returns>
        /// 
        /// <exception cref="ArgumentException">There is no Kinect device with specified ID connected to the system.</exception>
        /// <exception cref="ConnectionFailedException">Failed connecting to the Kinect device specified ID.</exception>
        /// 
        public static Kinect GetDevice( int deviceID )
        {
            if ( ( deviceID < 0 ) || ( deviceID >= DeviceCount ) )
            {
                throw new ArgumentException( "There is no Kinect device with specified ID connected to the system." );
            }

            bool needToStartStatusThread = false;
            Kinect kinect = null;

            lock ( openDevices )
            {
                needToStartStatusThread = ( openDevices.Count == 0 );

                // check if the device is open already
                if ( !openDevices.ContainsKey( deviceID ) )
                {
                    IntPtr devicePointer = IntPtr.Zero;

                    // connect to Kinect device witht the specified ID
                    if ( KinectNative.freenect_open_device( KinectNative.Context, ref devicePointer, deviceID ) != 0 )
                    {
                        throw new ConnectionFailedException( "Failed connecting to the Kinect device with ID: " + deviceID );
                    }

                    openDevices.Add( deviceID, new DeviceContext( devicePointer ) );
                }

                openDevices[deviceID].ReferenceCounter++;
                kinect = new Kinect( openDevices[deviceID].Device, deviceID );
            }

            if ( needToStartStatusThread )
            {
                StartStatusThread( );
            }

            return kinect;
        }

        // Private constructor to make sure class instance can be obtained through GetDevice() only
        private Kinect( IntPtr rawDevice, int deviceID )
        {
            this.rawDevice = rawDevice;
            this.deviceID  = deviceID;
            KinectNative.OnError += new KinectNative.ErrorHandler( Kinect_OnError );
        }

        /// <summary>
        /// Object finalizer/destructor makes sure unmanaged resource are freed if user did not call <see cref="Dispose()"/>.
        /// </summary>
        ~Kinect( )
        {
            Dispose( false );
        }

        /// <summary>
        /// Dispose device freeing all associated unmanaged resources.
        /// </summary>
        public void Dispose( )
        {
            lock ( sync )
            {
                if ( rawDevice == IntPtr.Zero )
                    return;

                Dispose( true );
            }
            KinectNative.OnError -= new KinectNative.ErrorHandler( Kinect_OnError );
            GC.SuppressFinalize( this );
        }

        private void Dispose( bool disposing )
        {
            bool needToStopStatusThread = false;

            lock ( openDevices )
            {
                // decrease reference counter and check if we need to close the device
                if ( --openDevices[deviceID].ReferenceCounter == 0 )
                {
                    if ( !openDevices[deviceID].DeviceFailed )
                    {
                        KinectNative.freenect_close_device( rawDevice );
                    }
                    openDevices.Remove( deviceID );
                }

                needToStopStatusThread = ( openDevices.Count == 0 );
            }

            rawDevice = IntPtr.Zero;

            if ( needToStopStatusThread )
            {
                StopStatusThread( );
            }
        }

        // Check if the device was disposed or not
        private void CheckDevice( )
        {
            if ( rawDevice == IntPtr.Zero )
            {
                throw new ObjectDisposedException( "Cannot access already disposed object." );
            }
        }

        /// <summary>
        /// Set color of Kinect's LED.
        /// </summary>
        /// 
        /// <param name="ledColor">LED color to set.</param>
        /// 
        /// <exception cref="DeviceErrorException">Some error occurred with the device. Check error message.</exception>
        /// 
        public void SetLedColor( LedColorOption ledColor )
        {
            lock ( sync )
            {
                CheckDevice( );

                int result = KinectNative.freenect_set_led( rawDevice, ledColor );

                if ( result != 0 )
                {
                    throw new DeviceErrorException( "Failed setting LED color to " + ledColor + ". Error code: " + result );
                }
            }
        }

        /// <summary>
        /// Set motor's tilt value.
        /// </summary>
        /// 
        /// <param name="angle">Tilt value to set, [-31, 30] degrees.</param>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">Motor tilt has to be in the [-31, 31] range.</exception>
        /// <exception cref="DeviceErrorException">Some error occurred with the device. Check error message.</exception>
        ///
        public void SetMotorTilt( int angle )
        {
            lock ( sync )
            {
                CheckDevice( );

                // check if value is in valid range
                if ( ( angle < -31 ) || ( angle > 31 ) )
                {
                    throw new ArgumentOutOfRangeException( "angle", "Motor tilt has to be in the [-31, 31] range." );
                }

                int result = KinectNative.freenect_set_tilt_degs( rawDevice, angle );
                if ( result != 0 )
                {
                    throw new DeviceErrorException( "Failed setting motor tilt. Error code: " + result );
                }
            }
        }

        private const double CountsPerGravity = 819.0;
        private const double Gravity = 9.80665;

        /// <summary>
        /// Get accelerometer values for 3 axes.
        /// </summary>
        /// 
        /// <param name="x">X axis value on the accelerometer.</param>
        /// <param name="y">Y axis value on the accelerometer.</param>
        /// <param name="z">Z axis value on the accelerometer.</param>
        /// 
        /// <remarks><para>Units of all 3 values are m/s<sup>2</sup>. The <b>g</b> value used
        /// for calculations is taken as 9.80665 m/s<sup>2</sup>.</para></remarks>
        /// 
        public void GetAccelerometerValues( out double x, out double y, out double z )
        {
            KinectNative.TiltState tiltState = new KinectNative.TiltState( );

            lock ( sync )
            {
                CheckDevice( );
                tiltState = openDevices[deviceID].TiltState;
            }

            x = (double) tiltState.AccelerometerX / CountsPerGravity * Gravity;
            y = (double) tiltState.AccelerometerY / CountsPerGravity * Gravity;
            z = (double) tiltState.AccelerometerZ / CountsPerGravity * Gravity;
        }

        /// <summary>
        /// Get Kinect's video camera.
        /// </summary>
        /// 
        /// <returns>Returns Kinect's video camera.</returns>
        /// 
        /// <remarks><para>The method simply creates instance of the <see cref="KinectVideoCamera"/> class
        /// by calling its appropriate constructor. Use <see cref="KinectVideoCamera.Start"/> method
        /// to start the video then.</para></remarks>
        /// 
        public KinectVideoCamera GetVideoCamera( )
        {
            return new KinectVideoCamera( deviceID );
        }

        /// <summary>
        /// Get Kinect's depth camera.
        /// </summary>
        /// 
        /// <returns>Returns Kinect's depth camera.</returns>
        /// 
        /// <remarks><para>The method simply creates instance of the <see cref="KinectDepthCamera"/> class
        /// by calling its appropriate constructor. Use <see cref="KinectDepthCamera.Start"/> method
        /// to start the video then.</para></remarks>
        /// 
        public KinectDepthCamera GetDepthCamera( )
        {
            return new KinectDepthCamera( deviceID );
        }

        #region Kinect Status Thread

        private static object statusThreadSync = new object( );

        // Start status thread to handle freenect's events
        private static void StartStatusThread( )
        {
            lock ( statusThreadSync )
            {
                stopEvent = new ManualResetEvent( false );
                updateStatusThread = new Thread( KinectStatusThread );
                updateStatusThread.IsBackground = true;
                updateStatusThread.Start( );
            }
        }

        // Stop the status thread
        private static void StopStatusThread( )
        {
            lock ( statusThreadSync )
            {
                stopEvent.Set( );
                if ( !updateStatusThread.Join( 2000 ) )
                {
                    updateStatusThread.Abort( );
                }

                stopEvent.Close( );
                updateStatusThread = null;
                stopEvent = null;
            }
        }

        // Kinect's status thread to process freenect's events
        private static void KinectStatusThread( )
        {
            while ( !stopEvent.WaitOne( 100, false ) )
            {
                lock ( openDevices )
                {
                    if ( openDevices.Count != 0 )
                    {
                        // update the status for each open device
                        foreach ( DeviceContext deviceContext in openDevices.Values )
                        {
                            if ( deviceContext.DeviceFailed )
                            {
                                continue;
                            }

                            if ( KinectNative.freenect_update_tilt_state( deviceContext.Device ) < 0 )
                            {
                                deviceContext.DeviceFailed = true;
                                deviceContext.FireFailureHandlers( );
                            }
                            else
                            {
                                // get updated device status
                                IntPtr ptr = KinectNative.freenect_get_tilt_state( deviceContext.Device );
                                deviceContext.TiltState = (KinectNative.TiltState)
                                    System.Runtime.InteropServices.Marshal.PtrToStructure( ptr, typeof( KinectNative.TiltState ) );
                            }
                        }
                    }
                }

                // let the kinect library handle any pending stuff on the usb stream
                KinectNative.freenect_process_events_timeout0( KinectNative.Context );
            }
        }

        private void Kinect_OnError( IntPtr device )
        {
            if ( device == rawDevice )
            {
                Console.WriteLine( "Error is detected in device : " + deviceID );
            }
        }

        internal bool IsDeviceFailed( int deviceID )
        {
            return ( ( openDevices.ContainsKey( deviceID ) ) && ( openDevices[deviceID].DeviceFailed ) );
        }

        internal void AddFailureHandler( int deviceID, DeviceFailureHandler handler )
        {
            if ( openDevices.ContainsKey( deviceID ) )
            {
                openDevices[deviceID].AddFailureHandler( handler );
            }
        }
        #endregion
    }
}
