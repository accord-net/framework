// AForge XIMEA Video Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace AForge.Video.Ximea
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Threading;
    using AForge.Video;
    using AForge.Video.Ximea.Internal;

    /// <summary>
    /// The class provides continues access to XIMEA cameras.
    /// </summary>
    /// 
    /// <remarks><para>The video source class is aimed to provide continues access to XIMEA camera, when
    /// images are continuosly acquired from camera and provided throw the <see cref="NewFrame"/> event.
    /// It just creates a background thread and gets new images from <see cref="XimeaCamera">XIMEA camera</see>
    /// keeping the <see cref="FrameInterval">specified time interval</see> between image acquisition.
    /// Essentially it is a wrapper class around <see cref="XimeaCamera"/> providing <see cref="IVideoSource"/> interface.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create video source for the XIMEA camera with ID 0
    /// XimeaVideoSource videoSource = new XimeaVideoSource( 0 );
    /// // set event handlers
    /// videoSource.NewFrame += new NewFrameEventHandler( video_NewFrame );
    /// // start the video source
    /// videoSource.Start( );
    /// 
    /// // set exposure time to 10 milliseconds
    /// videoSource.SetParam( CameraParameter.Exposure, 10 * 1000 );
    /// 
    /// // ...
    /// 
    /// // New frame event handler, which is invoked on each new available video frame
    /// private void video_NewFrame( object sender, NewFrameEventArgs eventArgs )
    /// {
    ///     // get new frame
    ///     Bitmap bitmap = eventArgs.Frame;
    ///     // process the frame
    /// }
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="XimeaCamera"/>
    /// 
    public class XimeaVideoSource : IVideoSource
    {
        // XIMEA camera to capture images from
        private XimeaCamera camera = new XimeaCamera( );

        // camera ID
        private int deviceID;
        // received frames count
        private int framesReceived;
        // recieved byte count
        private long bytesReceived;
        // frame interval in milliseconds
        private int frameInterval = 200;

        private Thread thread = null;
        private ManualResetEvent stopEvent = null;

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
        /// A string identifying the video source.
        /// </summary>
        /// 
        public virtual string Source
        {
            get { return "Ximea:" + deviceID; }
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
                Thread tempThread = null;

                lock ( sync )
                {
                    tempThread = thread;
                }

                if ( tempThread != null )
                {
                    // check thread status
                    if ( tempThread.Join( 0 ) == false )
                        return true;

                    // the thread is not running, so free resources
                    Free( );
                }

                return false;
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
        /// Time interval between frames.
        /// </summary>
        /// 
        /// <remarks><para>The property sets the interval in milliseconds between getting new frames from the camera.
        /// If the property is set to 100, then the desired frame rate should be about 10 frames per second.</para>
        /// 
        /// <para><note>Setting this property to 0 leads to no delay between video frames - frames
        /// are read as fast as possible.</note></para>
        /// 
        /// <para>Default value is set to <b>200</b>.</para>
        /// </remarks>
        /// 
        public int FrameInterval
        {
            get { return frameInterval; }
            set { frameInterval = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XimeaVideoSource"/> class.
        /// </summary>
        /// 
        /// <param name="deviceID">XIMEA camera ID (index) to connect to.</param>
        /// 
        public XimeaVideoSource( int deviceID )
        {
            this.deviceID = deviceID;
        }

        /// <summary>
        /// Start video source.
        /// </summary>
        /// 
        /// <remarks>Starts video source and returns execution to caller. Video camera will be started
        /// and will provide new video frames through the <see cref="NewFrame"/> event.</remarks>
        /// 
        /// <exception cref="ArgumentException">There is no XIMEA camera with specified ID connected to the system.</exception>
        /// <exception cref="VideoException">An error occurred while communicating with a camera. See error
        /// message for additional information.</exception>
        /// 
        public void Start( )
        {
            if ( IsRunning )
                return;

            lock ( sync )
            {
                if ( thread == null )
                {
                    // check source
                    if ( deviceID >= XimeaCamera.CamerasCount )
                    {
                        throw new ArgumentException( "There is no XIMEA camera with specified ID connected to the system." );
                    }

                    // prepare the camera
                    camera.Open( deviceID );

                    framesReceived = 0;
                    bytesReceived = 0;

                    // create events
                    stopEvent = new ManualResetEvent( false );

                    // create and start new thread
                    thread = new Thread( new ThreadStart( WorkerThread ) );
                    thread.Name = Source;
                    thread.Start( );
                }
            }
        }

        /// <summary>
        /// Signal video source to stop its work.
        /// </summary>
        /// 
        /// <remarks><para></para></remarks>
        /// 
        public void SignalToStop( )
        {
            lock ( sync )
            {
                // stop thread
                if ( thread != null )
                {
                    // signal to stop
                    stopEvent.Set( );
                }
            }
        }

        /// <summary>
        /// Wait for video source has stopped.
        /// </summary>
        /// 
        /// <remarks><para></para></remarks>
        /// 
        public void WaitForStop( )
        {
            Thread tempThread = null;

            lock ( sync )
            {
                tempThread = thread;
            }

            if ( tempThread != null )
            {
                // wait for thread stop
                tempThread.Join( );

                Free( );
            }
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
            Thread tempThread = null;

            lock ( sync )
            {
                tempThread = thread;
            }

            if ( tempThread != null )
            {
                if ( tempThread.Join( 0 ) == false )
                {
                    tempThread.Abort( );
                    tempThread.Join( );
                }
                Free( );
            }
        }

        /// <summary>
        /// Set camera's parameter.
        /// </summary>
        /// 
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Integer parameter value.</param>
        /// 
        /// <remarks><para><note>The call is redirected to <see cref="XimeaCamera.SetParam(string, int)"/>.</note></para></remarks>
        ///
        public void SetParam( string parameterName, int value )
        {
            camera.SetParam( parameterName, value );
        }

        /// <summary>
        /// Set camera's parameter.
        /// </summary>
        /// 
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Float parameter value.</param>
        /// 
        /// <remarks><para><note>The call is redirected to <see cref="XimeaCamera.GetParamFloat"/>.</note></para></remarks>
        ///
        public void SetParam( string parameterName, float value )
        {
            camera.SetParam( parameterName, value );
        }

        /// <summary>
        /// Get camera's parameter as integer value.
        /// </summary>
        /// 
        /// <param name="parameterName">Parameter name to get from camera.</param>
        /// 
        /// <returns>Returns integer value of the requested parameter.</returns>
        /// 
        /// <remarks><para><note>The call is redirected to <see cref="XimeaCamera.GetParamFloat"/>.</note></para></remarks>
        ///
        public int GetParamInt( string parameterName )
        {
            return camera.GetParamInt( parameterName );
        }

        /// <summary>
        /// Get camera's parameter as float value.
        /// </summary>
        /// 
        /// <param name="parameterName">Parameter name to get from camera.</param>
        /// 
        /// <returns>Returns float value of the requested parameter.</returns>
        /// 
        /// <remarks><para><note>The call is redirected to <see cref="XimeaCamera.GetParamFloat"/>.</note></para></remarks>
        ///
        public float GetParamFloat( string parameterName )
        {
            return camera.GetParamFloat( parameterName );
        }

        /// <summary>
        /// Get camera's parameter as string value.
        /// </summary>
        /// 
        /// <param name="parameterName">Parameter name to get from camera.</param>
        /// 
        /// <returns>Returns string value of the requested parameter.</returns>
        /// 
        /// <remarks><para><note>The call is redirected to <see cref="XimeaCamera.GetParamString"/>.</note></para></remarks>
        ///
        public string GetParamString( string parameterName )
        {
            return camera.GetParamString( parameterName );
        }

        // Free resources
        private void Free( )
        {
            lock ( sync )
            {
                thread = null;

                // release events
                if ( stopEvent != null )
                {
                    stopEvent.Close( );
                    stopEvent = null;
                }

                camera.Close( );
            }
        }

        // Worker thread
        private void WorkerThread( )
        {
            ReasonToFinishPlaying reasonToStop = ReasonToFinishPlaying.StoppedByUser;

            try
            {
                camera.StartAcquisition( );

                // while there is no request for stop
                while ( !stopEvent.WaitOne( 0, false ) )
                {
                    // start time
                    DateTime start = DateTime.Now;

                    // get next frame
                    Bitmap bitmap = camera.GetImage( 15000, false );

                    framesReceived++;
                    bytesReceived += bitmap.Width * bitmap.Height * ( Bitmap.GetPixelFormatSize( bitmap.PixelFormat ) >> 3 );

                    if ( NewFrame != null )
                        NewFrame( this, new NewFrameEventArgs( bitmap ) );

                    // free image
                    bitmap.Dispose( );

                    // wait for a while ?
                    if ( frameInterval > 0 )
                    {
                        // get frame duration
                        TimeSpan span = DateTime.Now.Subtract( start );

                        // miliseconds to sleep
                        int msec = frameInterval - (int) span.TotalMilliseconds;

                        if ( ( msec > 0 ) && ( stopEvent.WaitOne( msec, false ) ) )
                            break;
                    }
                }
            }
            catch ( Exception exception )
            {
                reasonToStop = ReasonToFinishPlaying.VideoSourceError;
                // provide information to clients
                if ( VideoSourceError != null )
                {
                    VideoSourceError( this, new VideoSourceErrorEventArgs( exception.Message ) );
                }
            }
            finally
            {
                try
                {
                    camera.StopAcquisition( );
                }
                catch
                {
                }
            }

            if ( PlayingFinished != null )
            {
                PlayingFinished( this, reasonToStop );
            }
        }
    }
}
