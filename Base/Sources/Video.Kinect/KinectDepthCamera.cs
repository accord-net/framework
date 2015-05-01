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
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;

    using AForge;
    using AForge.Video;

    /// <summary>
    /// Video source for Microsoft Kinect's depth sensor.
    /// </summary>
    /// 
    /// <remarks><para>The video source captures depth data from Microsoft <a href="http://en.wikipedia.org/wiki/Kinect">Kinect</a>
    /// depth sensor, which is aimed originally as a gaming device for XBox 360 platform.</para>
    /// 
    /// <para><note>Prior to using the class, make sure you've installed Kinect's drivers
    /// as described on <a href="http://openkinect.org/wiki/Getting_Started#Windows">Open Kinect</a>
    /// project's page.</note></para>
    ///
    /// <para><note>In order to run correctly the class requires <i>freenect.dll</i> library
    /// to be put into solution's output folder. This can be found within the AForge.NET framework's
    /// distribution in Externals folder.</note></para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create video source
    /// KinectDepthCamera videoSource = new KinectDepthCamera( 0 );
    /// // set NewFrame event handler
    /// videoSource.NewFrame += new NewFrameEventHandler( video_NewFrame );
    /// // start the video source
    /// videoSource.Start( );
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
    public class KinectDepthCamera : IVideoSource
    {
        // Kinect's device ID
        private int deviceID;
        // received frames count
        private int framesReceived;
        // recieved byte count
        private long bytesReceived;
        // specifies of original 11 bit depth images (as 16 bit) should be provided and 24 bit color image
        private bool provideOriginalDepthImage = false;
        private ushort[] gamma = new ushort[2048];
        // camera resolution to set
        private CameraResolution resolution = CameraResolution.Medium;
        // list of currently running cameras
        private static List<int> runningCameras = new List<int>( );

        // dummy object to lock for synchronization
        private object sync = new object( );

        /// <summary>
        /// New frame event.
        /// </summary>
        /// 
        /// <remarks><para>Notifies clients about new available frames from the video source.</para>
        /// 
        /// <para><note>Since video source may have multiple clients, each client is responsible for
        /// making a copy (cloning) of the passed video frame, because the video source disposes its
        /// own original copy after notifying of clients.</note></para>
        /// </remarks>
        /// 
        public event NewFrameEventHandler NewFrame;

        /// <summary>
        /// Video source error event.
        /// </summary>
        /// 
        /// <remarks>This event is used to notify clients about any type of errors occurred in
        /// video source object, for example internal exceptions.</remarks>
        ///
        public event VideoSourceErrorEventHandler VideoSourceError;

        /// <summary>
        /// Video playing finished event.
        /// </summary>
        /// 
        /// <remarks><para>This event is used to notify clients that the video playing has finished.</para>
        /// </remarks>
        /// 
        public event PlayingFinishedEventHandler PlayingFinished;

        /// <summary>
        /// Provide original depth image or colored depth map.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies if the video source should provide original data
        /// provided by Kinect's depth sensor or provide colored depth map. If the property is set to
        /// <see langword="true"/>, then the video source will provide 16 bpp grayscale images, where
        /// 11 least significant bits represent data provided by the sensor. If the property is
        /// set to <see langword="false"/>, then the video source will provide 24 bpp color images,
        /// which represents depth map. In this case depth is encoded by color gradient:
        /// white->red->yellow->green->cyan->blue->black. So colors which are closer to white represent
        /// objects which are closer to the Kinect sensor, but colors which are closer to black represent
        /// objects which are further away from Kinect.</para>
        /// 
        /// <para><note>The property must be set before running the video source to take effect.</note></para>
        /// 
        /// <para>Default value is set to <see langword="false"/>.</para>
        /// </remarks>
        /// 
        private bool ProvideOriginalDepthImage
        {
            get { return provideOriginalDepthImage; }
            set { provideOriginalDepthImage = value; }
        }

        /// <summary>
        /// Resolution of depth sensor to set.
        /// </summary>
        /// 
        /// <remarks><para><note>The property must be set before running the video source to take effect.</note></para>
        /// 
        /// <para>Default value of the property is set to <see cref="CameraResolution.Medium"/>.</para>
        /// </remarks>
        /// 
        public CameraResolution Resolution
        {
            get { return resolution; }
            set { resolution = value; }
        }

        /// <summary>
        /// A string identifying the video source.
        /// </summary>
        /// 
        public virtual string Source
        {
            get { return "Kinect:DepthCamera:" + deviceID; }
        }

        /// <summary>
        /// State of the video source.
        /// </summary>
        /// 
        /// <remarks>Current state of video source object - running or not.</remarks>
        /// 
        public bool IsRunning
        {
            get
            {
                lock ( sync )
                {
                    return ( device != null );
                }
            }
        }

        /// <summary>
        /// Received bytes count.
        /// </summary>
        /// 
        /// <remarks>Number of bytes the video source provided from the moment of the last
        /// access to the property.
        /// </remarks>
        /// 
        public long BytesReceived
        {
            get
            {
                long bytes = bytesReceived;
                bytesReceived = 0;
                return bytes;
            }
        }

        /// <summary>
        /// Received frames count.
        /// </summary>
        /// 
        /// <remarks>Number of frames the video source provided from the moment of the last
        /// access to the property.
        /// </remarks>
        /// 
        public int FramesReceived
        {
            get
            {
                int frames = framesReceived;
                framesReceived = 0;
                return frames;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectDepthCamera"/> class.
        /// </summary>
        /// 
        /// <param name="deviceID">Kinect's device ID (index) to connect to.</param>
        /// 
        public KinectDepthCamera( int deviceID ) : this( deviceID, CameraResolution.Medium, false ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectDepthCamera"/> class.
        /// </summary>
        /// 
        /// <param name="deviceID">Kinect's device ID (index) to connect to.</param>
        /// <param name="resolution">Resolution of depth sensor to set.</param>
        /// 
        public KinectDepthCamera( int deviceID, CameraResolution resolution ) : this( deviceID, resolution, false ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectDepthCamera"/> class.
        /// </summary>
        /// 
        /// <param name="deviceID">Kinect's device ID (index) to connect to.</param>
        /// <param name="resolution">Resolution of depth sensor to set.</param>
        /// <param name="provideOriginalDepthImage">Provide original depth image or colored depth map
        /// (see <see cref="ProvideOriginalDepthImage"/> property).</param>
        /// 
        public KinectDepthCamera( int deviceID, CameraResolution resolution, bool provideOriginalDepthImage )
        {
            this.deviceID = deviceID;
            this.resolution = resolution;
            this.provideOriginalDepthImage = provideOriginalDepthImage;

            // initialize gamma values (as shown in the original Kinect samples)
            for ( int i = 0; i < 2048; i++ )
            {
                double value = i / 2048.0;
                value = Math.Pow( value, 3.0 );
                gamma[i] = (ushort) ( value * 36.0 * 256.0 );
            }
        }

        private Kinect device = null;
        private IntPtr imageBuffer = IntPtr.Zero;
        private KinectNative.BitmapInfoHeader depthModeInfo;

        /// <summary>
        /// Start video source.
        /// </summary>
        /// 
        /// <remarks>Starts video source and returns execution to caller. Video camera will be started
        /// and will provide new video frames through the <see cref="NewFrame"/> event.</remarks>
        /// 
        /// <exception cref="ArgumentException">The specified resolution is not supported for the selected
        /// mode of the Kinect depth sensor.</exception>
        /// <exception cref="ConnectionFailedException">Could not connect to Kinect's depth sensor.</exception>
        /// <exception cref="DeviceBusyException">Another connection to the specified depth sensor is already running.</exception>
        /// 
        public void Start( )
        {
            lock ( sync )
            {
                lock ( runningCameras )
                {
                    if ( device == null )
                    {
                        bool success = false;

                        try
                        {
                            if ( runningCameras.Contains( deviceID ) )
                            {
                                throw new DeviceBusyException( "Another connection to the specified depth camera is already running." );
                            }

                            device = Kinect.GetDevice( deviceID );

                            // find depth format parameters
                            depthModeInfo = KinectNative.freenect_find_depth_mode( resolution, KinectNative.DepthCameraFormat.Format11Bit );

                            if ( depthModeInfo.IsValid == 0 )
                            {
                                throw new ArgumentException( "The specified resolution is not supported for the selected mode of the Kinect depth camera." );
                            }

                            // set depth format
                            if ( KinectNative.freenect_set_depth_mode( device.RawDevice, depthModeInfo ) != 0 )
                            {
                                throw new VideoException( "Could not switch to the specified depth format." );
                            }

                            // allocate video buffer and provide it freenect
                            imageBuffer = Marshal.AllocHGlobal( (int) depthModeInfo.Bytes );
                            KinectNative.freenect_set_depth_buffer( device.RawDevice, imageBuffer );

                            // set video callback
                            videoCallback = new KinectNative.FreenectDepthDataCallback( HandleDataReceived );
                            KinectNative.freenect_set_depth_callback( device.RawDevice, videoCallback );

                            // start the camera
                            if ( KinectNative.freenect_start_depth( device.RawDevice ) != 0 )
                            {
                                throw new ConnectionFailedException( "Could not start depth stream." );
                            }

                            success = true;
                            runningCameras.Add( deviceID );

                            device.AddFailureHandler( deviceID, Stop );
                        }
                        finally
                        {
                            if ( !success )
                            {
                                if ( device != null )
                                {
                                    device.Dispose( );
                                    device = null;
                                }

                                if ( imageBuffer != IntPtr.Zero )
                                {
                                    Marshal.FreeHGlobal( imageBuffer );
                                    imageBuffer = IntPtr.Zero;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Signal video source to stop its work.
        /// </summary>
        /// 
        /// <remarks><para><note>Calling this method is equivalent to calling <see cref="Stop"/>
        /// for Kinect video camera.</note></para></remarks>
        /// 
        public void SignalToStop( )
        {
            Stop( );
        }

        /// <summary>
        /// Wait for video source has stopped.
        /// </summary>
        /// 
        /// <remarks><para><note>Calling this method is equivalent to calling <see cref="Stop"/>
        /// for Kinect video camera.</note></para></remarks>
        /// 
        public void WaitForStop( )
        {
            Stop( );
        }

        /// <summary>
        /// Stop video source.
        /// </summary>
        /// 
        /// <remarks><para>The method stop the video source, so it no longer provides new video frames
        /// and does not consume any resources.</para>
        /// </remarks>
        ///
        public void Stop( )
        {
            lock ( sync )
            {
                lock ( runningCameras )
                {
                    if ( device != null )
                    {
                        bool deviceFailed = device.IsDeviceFailed( deviceID );

                        if ( !deviceFailed )
                        {
                            KinectNative.freenect_stop_depth( device.RawDevice );
                        }

                        device.Dispose( );
                        device = null;
                        runningCameras.Remove( deviceID );

                        if ( PlayingFinished != null )
                        {
                            PlayingFinished( this, ( !deviceFailed ) ?
                                ReasonToFinishPlaying.StoppedByUser : ReasonToFinishPlaying.DeviceLost );
                        }
                    }

                    if ( imageBuffer != IntPtr.Zero )
                    {
                        Marshal.FreeHGlobal( imageBuffer );
                        imageBuffer = IntPtr.Zero;
                    }

                    videoCallback = null;
                }
            }
        }

        // New video data event handler
        private KinectNative.FreenectDepthDataCallback videoCallback = null;

        private void HandleDataReceived( IntPtr device, IntPtr depthData, UInt32 timestamp )
        {
            int width  = depthModeInfo.Width;
            int height = depthModeInfo.Height;

            Bitmap image = null;
            BitmapData data = null;

            try
            {
                image = new Bitmap( width, height, ( !provideOriginalDepthImage ) ?
                    PixelFormat.Format24bppRgb : PixelFormat.Format16bppGrayScale );

                data = image.LockBits( new Rectangle( 0, 0, width, height ),
                    ImageLockMode.ReadWrite, image.PixelFormat );

                unsafe
                {
                    ushort* src = (ushort*) imageBuffer.ToPointer( );

                    if ( !provideOriginalDepthImage )
                    {
                        // color the depth image into white->red->yellow->green->cyan->blue->black gradient
                        byte* dst = (byte*) data.Scan0.ToPointer( );
                        int offset = data.Stride - width * 3;
                        byte red, green, blue;

                        for ( int y = 0; y < height; y++ )
                        {
                            for ( int x = 0; x < width; x++, src++, dst += 3 )
                            {
                                ushort pval = gamma[*src];
                                ushort lb 	= (ushort) ( pval & 0xff );

                                switch ( pval >> 8 )
                                {
                                    case 0: // white to red
                                        red = 255;
                                        green = (byte) ( 255 - lb );
                                        blue = (byte) ( 255 - lb );
                                        break;
                                    case 1: // red to yellow
                                        red = 255;
                                        green = (byte) lb;
                                        blue = 0;
                                        break;
                                    case 2: // yellow to green
                                        red = (byte) ( 255 - lb );
                                        green = 255;
                                        blue = 0;
                                        break;
                                    case 3: // green to cyan
                                        red = 0;
                                        green = 255;
                                        blue = (byte) lb;
                                        break;
                                    case 4: // cyan to blue
                                        red = 0;
                                        green = (byte) ( 255 - lb );
                                        blue = 255;
                                        break;
                                    case 5: // blue to black
                                        red = 0;
                                        green = 0;
                                        blue = (byte) ( 255 - lb );
                                        break;
                                    default:
                                        red = green = blue = 0;
                                        break;
                                }

                                dst[2] = red;
                                dst[1] = green;
                                dst[0] = blue;
                            }
                            dst += offset;
                        }
                    }
                    else
                    {
                        // copy original depth image
                        ushort* dst = (ushort*) data.Scan0.ToPointer( );
                        int offset = ( data.Stride >> 1 ) - width;

                        if ( offset == 0 )
                        {
                            SystemTools.CopyUnmanagedMemory( (byte*) dst, (byte*) src, height * width * 2 );
                        }
                        else
                        {
                            for ( int y = 0; y < height; y++ )
                            {
                                SystemTools.CopyUnmanagedMemory( (byte*) dst, (byte*) src, width * 2 );

                                dst += width;
                                src += width;
                                dst += offset;
                            }
                        }
                    }
                }

                image.UnlockBits( data );
                framesReceived++;
                bytesReceived += (int) depthModeInfo.Bytes;
            }
            catch ( Exception ex )
            {
                if ( VideoSourceError != null )
                {
                    VideoSourceError( this, new VideoSourceErrorEventArgs( ex.Message ) );
                }

                if ( image != null )
                {
                    if ( data != null )
                    {
                        image.UnlockBits( data );
                    }
                    image.Dispose( );
                    image = null;
                }
            }

            if ( image != null )
            {
                if ( NewFrame != null )
                {
                    NewFrame( this, new NewFrameEventArgs( image ) );
                }
                image.Dispose( );
            }
        }
    }
}
