// AForge TeRK Robotics Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

using TeRKIceLib = TeRK;

namespace AForge.Robotics.TeRK
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Threading;
    using AForge;
    using AForge.Video;

    public partial class Qwerk
    {
        /// <summary>
        /// Provides access to web camera connected to Qwerk.
        /// </summary>
        /// 
        /// <remarks><para>The class allows to start Qwerk's camera and continuously receive
        /// frames from it. The class creates background thread to poll Qwerk's camera and provides
        /// them through <see cref="NewFrame"/> event. The video frame rate can be configured
        /// using <see cref="FrameInterval"/> property, which sets time interval between frames.</para>
        /// 
        /// <para><note>The class is deprecated.</note></para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // get Qwerk's video service
        /// Qwerk.Video video = qwerk.GetVideoService( );
        /// // set NewFrame event handler
        /// video.NewFrame += new NewFrameEventHandler( video_NewFrame );
        /// // start the video source
        /// video.Start( );
        /// // ...
        /// // signal to stop
        /// video.SignalToStop( );
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
        [Obsolete( "The class is deprecated." )]
        public class Video : IVideoSource
        {
            // Qwerk's video streamer
            private TeRKIceLib.VideoStreamerServerPrx videoStreamer = null;
            // video source string (informational only to implment IVideoSource interface only)
            private string source;
            // received frames count
            private int framesReceived;
            // recieved byte count
            private long bytesReceived;
            // frame interval in milliseconds
            private int frameInterval = 0;

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
            /// <remarks><para>The property sets the interval in milliseconds betwen frames. If the property is
            /// set to 100, then the desired frame rate will be 10 frames per second.</para>
            /// 
            /// <para>Default value is set to <b>0</b> - get new frames as fast as possible.</para>
            /// </remarks>
            /// 
            public int FrameInterval
            {
                get { return frameInterval; }
                set { frameInterval = value; }
            }

            /// <summary>
            /// Video source string.
            /// </summary>
            /// 
            /// <remarks>
            /// <para>The property keeps connection string, which is used to connect to TeRK's video
            /// streaming service.</para>
            /// </remarks>
            /// 
            public string Source
            {
                get { return source; }
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

                        // the thread is not running, free resources
                        Free( );
                    }
                    return false;
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Qwerk.Video"/> class.
            /// </summary>
            /// 
            /// <param name="qwerk">Reference to <see cref="Qwerk"/> object, which is connected to Qwerk board.</param>
            /// 
            /// <exception cref="NotConnectedException">The passed reference to <see cref="Qwerk"/> object is not connected to
            /// Qwerk board.</exception>
            /// <exception cref="ConnectionFailedException">Failed connecting to the requested service.</exception>
            /// <exception cref="ServiceAccessFailedException">Failed accessing to the requested service.</exception>
            /// 
            public Video( Qwerk qwerk )
            {
                string hostAddress = qwerk.HostAddress;

                // check if connection was established
                if ( hostAddress != null )
                {
                    try
                    {
                        // prepare video source string
                        source = "'::TeRK::VideoStreamerServer':tcp -h " + hostAddress + " -p 10101";

                        Ice.ObjectPrx obj = qwerk.iceCommunicator.stringToProxy( source );
                        obj = obj.ice_timeout( Qwerk.TimeOut );
                        videoStreamer = TeRKIceLib.VideoStreamerServerPrxHelper.checkedCast( obj );
                    }
                    catch ( Ice.ObjectNotExistException )
                    {
                        // the object does not exist on the host
                        throw new ServiceAccessFailedException( "Failed accessing to the requested service." );
                    }
                    catch
                    {
                        throw new ConnectionFailedException( "Failed connecting to the requested service." );
                    }

                    if ( videoStreamer == null )
                    {
                        throw new ServiceAccessFailedException( "Failed accessing to the requested cervice." );
                    }
                }
                else
                {
                    throw new NotConnectedException( "Qwerk object is not connected to a board." );
                }
            }

            /// <summary>
            /// Start video source.
            /// </summary>
            /// 
            /// <remarks>Starts video source and return execution to caller. Video source
            /// object creates background thread and notifies about new frames with the
            /// help of <see cref="NewFrame"/> event.</remarks>
            /// 
            public void Start( )
            {
                if ( thread == null )
                {
                    framesReceived = 0;
                    bytesReceived  = 0;

                    // create events
                    stopEvent = new ManualResetEvent( false );

                    // create and start new thread
                    thread = new Thread( new ThreadStart( WorkerThread ) );
                    thread.Name = source; // mainly for debugging
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
            /// <remarks>Waits for video source stopping after it was signalled to stop using
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
                // download start time and duration
                DateTime start;
                TimeSpan span;

                while ( !stopEvent.WaitOne( 0, false ) )
                {
                    try
                    {
                        // start Qwerk's camera
                        videoStreamer.startCamera( );

                        while ( !stopEvent.WaitOne( 0, false ) )
                        {
                            // get download start time
                            start = DateTime.Now;

                            TeRKIceLib.Image qwerkImage = videoStreamer.getFrame( 0 );

                            // increase frames' and bytes' counters
                            framesReceived++;
                            bytesReceived += qwerkImage.data.Length;

                            if ( qwerkImage.format == TeRKIceLib.ImageFormat.ImageJPEG )
                            {
                                Bitmap image = (Bitmap) Image.FromStream( new MemoryStream( qwerkImage.data ) );

                                // notify clients
                                if ( NewFrame != null )
                                {
                                    NewFrame( this, new NewFrameEventArgs( image ) );
                                }

                                image.Dispose( );
                            }
                            else
                            {
                                if ( VideoSourceError != null )
                                {
                                    VideoSourceError( this, new VideoSourceErrorEventArgs( "Video frame has unsupported format: " + qwerkImage.format ) );
                                }
                            }

                            // wait for a while ?
                            if ( frameInterval > 0 )
                            {
                                // get download duration
                                span = DateTime.Now.Subtract( start );
                                // miliseconds to sleep
                                int msec = frameInterval - (int) span.TotalMilliseconds;

                                while ( ( msec > 0 ) && ( stopEvent.WaitOne( 0, false ) == false ) )
                                {
                                    // sleeping ...
                                    Thread.Sleep( ( msec < 100 ) ? msec : 100 );
                                    msec -= 100;
                                }
                            }
                        }

                        // stop Qwerk's camera
                        videoStreamer.stopCamera( );
                    }
                    catch ( Ice.SocketException )
                    {
                        if ( VideoSourceError != null )
                        {
                            VideoSourceError( this, new VideoSourceErrorEventArgs( "Connection lost to Qwerk's video service." ) );
                        }
                    }
                    catch
                    {
                        if ( VideoSourceError != null )
                        {
                            VideoSourceError( this, new VideoSourceErrorEventArgs( "Failed getting video frame from Qwerk." ) );
                        }
                    }
                }

                if ( PlayingFinished != null )
                {
                    PlayingFinished( this, ReasonToFinishPlaying.StoppedByUser );
                } 
            }
        }
    }
}
