// AForge Video for Windows Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace AForge.Video.VFW
{
    using System;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.IO;
	using System.Threading;

    using AForge.Video;

	/// <summary>
	/// AVI file video source.
	/// </summary>
    /// 
    /// <remarks><para>The video source reads AVI files using Video for Windows.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create AVI file video source
    /// AVIFileVideoSource source = new AVIFileVideoSource( "some file" );
    /// // set event handlers
    /// source.NewFrame += new NewFrameEventHandler( video_NewFrame );
    /// // start the video source
    /// source.Start( );
    /// // ...
    /// // signal to stop
    /// source.SignalToStop( );
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
	public class AVIFileVideoSource : IVideoSource
	{
        // video file name
		private string source;
        // received frames count
		private int framesReceived;
        // recieved byte count
        private long bytesReceived;
        // frame interval in milliseconds
        private int frameInterval = 0;
        // get frame interval from source or use manually specified
        private bool frameIntervalFromSource = true;

		private Thread thread = null;
		private ManualResetEvent stopEvent = null;

        /// <summary>
        /// New frame event.
        /// </summary>
        /// 
        /// <remarks><para>Notifies clients about new available frame from video source.</para>
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
        /// Frame interval.
        /// </summary>
        /// 
        /// <remarks><para>The property sets the interval in milliseconds between frames. If the property is
        /// set to 100, then the desired frame rate will be 10 frames per second.</para>
        /// 
        /// <para><note>Setting this property to 0 leads to no delay between video frames - frames
        /// are read as fast as possible.</note></para>
        /// 
        /// <para>Default value is set to <b>0</b>.</para>
        /// </remarks>
        /// 
        public int FrameInterval
        {
            get { return frameInterval; }
            set { frameInterval = value; }
        }

        /// <summary>
        /// Get frame interval from source or use manually specified.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies which frame rate to use for video playing.
        /// If the property is set to <see langword="true"/>, then video is played
        /// with original frame rate, which is set in source AVI file. If the property is
        /// set to <see langword="false"/>, then custom frame rate is used, which is
        /// calculated based on the manually specified <see cref="FrameInterval">frame interval</see>.</para>
        /// 
        /// <para>Default value is set to <see langword="true"/>.</para>
        /// </remarks>
        /// 
        public bool FrameIntervalFromSource
        {
            get { return frameIntervalFromSource; }
            set { frameIntervalFromSource = value; }
        }

        /// <summary>
        /// Video source.
        /// </summary>
        /// 
        /// <remarks><para>Video file name to play.</para></remarks>
        /// 
        public virtual string Source
		{
			get { return source; }
			set { source = value; }
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
        /// State of the video source.
        /// </summary>
        /// 
        /// <remarks>Current state of video source object - running or not.</remarks>
        /// 
        public bool IsRunning
		{
			get
			{
				if ( thread != null )
				{
                    // check thread status
                    if ( thread.Join( 0 ) == false )
						return true;

					// the thread is not running, so free resources
					Free( );
				}
				return false;
			}
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="AVIFileVideoSource"/> class.
        /// </summary>
        /// 
		public AVIFileVideoSource( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AVIFileVideoSource"/> class.
        /// </summary>
        /// 
        /// <param name="source">Video file name.</param>
        /// 
        public AVIFileVideoSource( string source )
        {
            this.source = source;
        }

        /// <summary>
        /// Start video source.
        /// </summary>
        /// 
        /// <remarks>Starts video source and return execution to caller. Video source
        /// object creates background thread and notifies about new frames with the
        /// help of <see cref="NewFrame"/> event.</remarks>
        /// 
        /// <exception cref="ArgumentException">Video source is not specified.</exception>
        /// 
        public void Start( )
		{
            if ( !IsRunning )
			{
                // check source
                if ( string.IsNullOrEmpty( source ) )
                    throw new ArgumentException( "Video source is not specified." );
                
                framesReceived = 0;
                bytesReceived = 0;

				// create events
				stopEvent = new ManualResetEvent( false );
				
				// create and start new thread
				thread = new Thread( new ThreadStart( WorkerThread ) );
				thread.Name = source;
				thread.Start( );
			}
		}

        /// <summary>
        /// Signal video source to stop its work.
        /// </summary>
        /// 
        /// <remarks>Signals video source to stop its background thread, stop to
        /// provide new frames and free resources.</remarks>
        /// 
        public void SignalToStop( )
		{
			// stop thread
			if ( thread != null )
			{
				// signal to stop
				stopEvent.Set( );
			}
		}

        /// <summary>
        /// Wait for video source has stopped.
        /// </summary>
        /// 
        /// <remarks>Waits for source stopping after it was signalled to stop using
        /// <see cref="SignalToStop"/> method.</remarks>
        /// 
        public void WaitForStop( )
		{
			if ( thread != null )
			{
				// wait for thread stop
				thread.Join( );

				Free( );
			}
		}

        /// <summary>
        /// Stop video source.
        /// </summary>
        /// 
        /// <remarks><para>Stops video source aborting its thread.</para>
        /// 
        /// <para><note>Since the method aborts background thread, its usage is highly not preferred
        /// and should be done only if there are no other options. The correct way of stopping camera
        /// is <see cref="SignalToStop">signaling it stop</see> and then
        /// <see cref="WaitForStop">waiting</see> for background thread's completion.</note></para>
        /// </remarks>
        /// 
        public void Stop( )
		{
			if ( this.IsRunning )
			{
				thread.Abort( );
				WaitForStop( );
			}
		}

        /// <summary>
        /// Free resource.
        /// </summary>
        /// 
        private void Free( )
		{
			thread = null;

			// release events
			stopEvent.Close( );
			stopEvent = null;
		}

        /// <summary>
        /// Worker thread.
        /// </summary>
        /// 
        private void WorkerThread( )
		{
            ReasonToFinishPlaying reasonToStop = ReasonToFinishPlaying.StoppedByUser;
            // AVI reader
			AVIReader aviReader = new AVIReader( );

			try
			{
				// open file
				aviReader.Open( source );

                // stop positions
                int stopPosition = aviReader.Start + aviReader.Length;

                // frame interval
                int interval = ( frameIntervalFromSource ) ? (int) ( 1000 / aviReader.FrameRate ) : frameInterval;

                while ( !stopEvent.WaitOne( 0, false ) )
				{
					// start time
					DateTime start = DateTime.Now;

					// get next frame
					Bitmap bitmap = aviReader.GetNextFrame( );

					framesReceived++;
                    bytesReceived += bitmap.Width * bitmap.Height *
                        ( Bitmap.GetPixelFormatSize( bitmap.PixelFormat ) >> 3 );

					if ( NewFrame != null )
						NewFrame( this, new NewFrameEventArgs( bitmap ) );

					// free image
					bitmap.Dispose( );

                    // check current position
                    if ( aviReader.Position >= stopPosition )
                    {
                        reasonToStop = ReasonToFinishPlaying.EndOfStreamReached;
                        break;
                    }

                    // wait for a while ?
                    if ( interval > 0 )
                    {
                        // get frame extract duration
                        TimeSpan span = DateTime.Now.Subtract( start );

                        // miliseconds to sleep
                        int msec = interval - (int) span.TotalMilliseconds;

                        if ( ( msec > 0 ) && ( stopEvent.WaitOne( msec, false ) ) )
                            break;
                    }
				}
			}
			catch ( Exception exception )
			{
                // provide information to clients
                if ( VideoSourceError != null )
                {
                    VideoSourceError( this, new VideoSourceErrorEventArgs( exception.Message ) );
                }
			}

			aviReader.Dispose( );
			aviReader = null;

            if ( PlayingFinished != null )
            {
                PlayingFinished( this, reasonToStop );
            } 
		}
	}
}
