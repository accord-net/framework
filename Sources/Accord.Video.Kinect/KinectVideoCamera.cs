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
    using AForge.Imaging;
    using AForge.Video;

    /// <summary>
    /// Enumeration of video camera modes for the <see cref="KinectVideoCamera"/>.
    /// </summary>
    public enum VideoCameraMode
    {
        /// <summary>
        /// 24 bit per pixel RGB mode.
        /// </summary>
        Color,

        /// <summary>
        /// 8 bit per pixel Bayer mode.
        /// </summary>
        Bayer,

        /// <summary>
        /// 8 bit per pixel Infra Red mode.
        /// </summary>
        InfraRed
    }
    
    /// <summary>
    /// Video source for Microsoft Kinect's video camera.
    /// </summary>
    /// 
    /// <remarks><para>The video source captures video data from Microsoft <a href="http://en.wikipedia.org/wiki/Kinect">Kinect</a>
    /// video camera, which is aimed originally as a gaming device for XBox 360 platform.</para>
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
    /// KinectVideoCamera videoSource = new KinectVideoCamera( 0 );
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
    public class KinectVideoCamera : IVideoSource
    {
        // Kinect's device ID
        private int deviceID;
        // received frames count
        private int framesReceived;
        // recieved byte count
        private long bytesReceived;
        // camera mode
        private VideoCameraMode cameraMode = VideoCameraMode.Color;
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
        /// Specifies video mode for the camera.
        /// </summary>
        /// 
        /// <remarks>
        /// <para><note>The property must be set before running the video source to take effect.</note></para>
        /// 
        /// <para>Default value of the property is set to <see cref="VideoCameraMode.Color"/>.</para>
        /// </remarks>
        /// 
        public VideoCameraMode CameraMode
        {
            get { return cameraMode; }
            set { cameraMode = value; }
        }

        /// <summary>
        /// Resolution of video camera to set.
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
            get { return "Kinect:VideoCamera:" + deviceID; }
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
        /// Initializes a new instance of the <see cref="KinectVideoCamera"/> class.
        /// </summary>
        /// 
        /// <param name="deviceID">Kinect's device ID (index) to connect to.</param>
        /// 
        public KinectVideoCamera( int deviceID )
        {
            this.deviceID = deviceID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectVideoCamera"/> class.
        /// </summary>
        /// 
        /// <param name="deviceID">Kinect's device ID (index) to connect to.</param>
        /// <param name="resolution">Resolution of video camera to set.</param>
        /// 
        public KinectVideoCamera( int deviceID, CameraResolution resolution )
        {
            this.deviceID   = deviceID;
            this.resolution = resolution;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectVideoCamera"/> class.
        /// </summary>
        /// 
        /// <param name="deviceID">Kinect's device ID (index) to connect to.</param>
        /// <param name="resolution">Resolution of video camera to set.</param>
        /// <param name="cameraMode">Sets video camera mode.</param>
        /// 
        public KinectVideoCamera( int deviceID, CameraResolution resolution, VideoCameraMode cameraMode )
        {
            this.deviceID   = deviceID;
            this.resolution = resolution;
            this.cameraMode = cameraMode;
        }

        private Kinect device = null;
        private IntPtr imageBuffer = IntPtr.Zero;
        private KinectNative.BitmapInfoHeader videoModeInfo;

        /// <summary>
        /// Start video source.
        /// </summary>
        /// 
        /// <remarks>Starts video source and returns execution to caller. Video camera will be started
        /// and will provide new video frames through the <see cref="NewFrame"/> event.</remarks>
        /// 
        /// <exception cref="ArgumentException">The specified resolution is not supported for the selected
        /// mode of the Kinect video camera.</exception>
        /// <exception cref="ConnectionFailedException">Could not connect to Kinect's video camera.</exception>
        /// <exception cref="DeviceBusyException">Another connection to the specified video camera is already running.</exception>
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
                                throw new DeviceBusyException( "Another connection to the specified video camera is already running." );
                            }

                            // get Kinect device
                            device = Kinect.GetDevice( deviceID );

                            KinectNative.VideoCameraFormat dataFormat = KinectNative.VideoCameraFormat.RGB;
 
                            if ( cameraMode == VideoCameraMode.Bayer )
                            {
                                dataFormat = KinectNative.VideoCameraFormat.Bayer;
                            }
                            else if ( cameraMode == VideoCameraMode.InfraRed )
                            {
                                dataFormat = KinectNative.VideoCameraFormat.IR8Bit;
                            }

                            // find video format parameters
                            videoModeInfo = KinectNative.freenect_find_video_mode( resolution, dataFormat );

                            if ( videoModeInfo.IsValid == 0 )
                            {
                                throw new ArgumentException( "The specified resolution is not supported for the selected mode of the Kinect video camera." );
                            }

                            // set video format
                            if ( KinectNative.freenect_set_video_mode( device.RawDevice, videoModeInfo ) != 0 )
                            {
                                throw new VideoException( "Could not switch to the specified video format." );
                            }

                            // allocate video buffer and provide it freenect
                            imageBuffer = Marshal.AllocHGlobal( (int) videoModeInfo.Bytes );
                            KinectNative.freenect_set_video_buffer( device.RawDevice, imageBuffer );

                            // set video callback
                            videoCallback = new KinectNative.FreenectVideoDataCallback( HandleDataReceived );
                            KinectNative.freenect_set_video_callback( device.RawDevice, videoCallback );

                            // start the camera
                            if ( KinectNative.freenect_start_video( device.RawDevice ) != 0 )
                            {
                                throw new ConnectionFailedException( "Could not start video stream." );
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
        /// <remarks><para>The method stops the video source, so it no longer provides new video frames
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
                            KinectNative.freenect_stop_video( device.RawDevice );
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
        private KinectNative.FreenectVideoDataCallback videoCallback = null;

        private void HandleDataReceived( IntPtr device, IntPtr imageData, UInt32 timeStamp )
        {
            int width  = videoModeInfo.Width;
            int height = videoModeInfo.Height;

            Bitmap image = null;
            BitmapData data = null;

            try
            {
                image = ( cameraMode == VideoCameraMode.Color ) ?
                    new Bitmap( width, height, PixelFormat.Format24bppRgb ) :
                    AForge.Imaging.Image.CreateGrayscaleImage( width, height );

                data = image.LockBits( new Rectangle( 0, 0, width, height ),
                    ImageLockMode.ReadWrite, image.PixelFormat );

                unsafe
                {
                    byte* dst = (byte*) data.Scan0.ToPointer( );
                    byte* src = (byte*) imageBuffer.ToPointer( );

                    if ( cameraMode == VideoCameraMode.Color )
                    {
                        // color RGB 24 mode
                        int offset = data.Stride - width * 3;

                        for ( int y = 0; y < height; y++ )
                        {
                            for ( int x = 0; x < width; x++, src += 3, dst += 3 )
                            {
                                dst[0] = src[2];
                                dst[1] = src[1];
                                dst[2] = src[0];
                            }
                            dst += offset;
                        }
                    }
                    else
                    {
                        // infra red mode - grayscale output
                        int stride = data.Stride;

                        if ( stride != width )
                        {
                            for ( int y = 0; y < height; y++ )
                            {
                                SystemTools.CopyUnmanagedMemory( dst, src, width );
                                dst += stride;
                                src += width;
                            }
                        }
                        else
                        {
                            SystemTools.CopyUnmanagedMemory( dst, src, width * height );
                        }
                    }
                }
                image.UnlockBits( data );

                framesReceived++;
                bytesReceived += width * height;
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
