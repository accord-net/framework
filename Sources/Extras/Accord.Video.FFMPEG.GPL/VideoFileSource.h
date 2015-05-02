// AForge FFMPEG Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2011
// contacts@aforgenet.com
//

#pragma once

using namespace System;
using namespace System::Drawing;
using namespace System::Drawing::Imaging;
using namespace System::Threading;
using namespace AForge::Video;

namespace AForge { namespace Video { namespace FFMPEG
{
    /// <summary>
    /// Video source for video files.
    /// </summary>
    /// 
	/// <remarks><para>The video source provides access to video files using <a href="http://www.ffmpeg.org/">FFmpeg</a> library.</para>
	///
	/// <para><note>The class provides video only. Sound is not supported.</note></para>
    /// 
	/// <para><note>The class ignores presentation time of video frames while retrieving them from
	/// video file. Instead it provides video frames according to the FPS rate of the video file
	/// or the configured <see cref="FrameInterval"/>.</note></para>
    /// 
	/// <para><note>Make sure you have <b>FFmpeg</b> binaries (DLLs) in the output folder of your application in order
	/// to use this class successfully. <b>FFmpeg</b> binaries can be found in Externals folder provided with AForge.NET
	/// framework's distribution.</note></para>
	///
    /// <para>Sample usage:</para>
    /// <code>
    /// // create video source
    /// VideoFileSource videoSource = new VideoFileSource( fileName );
    /// // set NewFrame event handler
    /// videoSource.NewFrame += new NewFrameEventHandler( video_NewFrame );
    /// // start the video source
    /// videoSource.Start( );
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
	public ref class VideoFileSource : IVideoSource
	{
	public:

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
		virtual event NewFrameEventHandler^ NewFrame;

        /// <summary>
        /// Video source error event.
        /// </summary>
        /// 
        /// <remarks>This event is used to notify clients about any type of errors occurred in
        /// video source object, for example internal exceptions.</remarks>
        ///
		virtual event VideoSourceErrorEventHandler^ VideoSourceError;

        /// <summary>
        /// Video playing finished event.
        /// </summary>
        /// 
        /// <remarks><para>This event is used to notify clients that the video playing has finished.</para>
        /// </remarks>
        /// 
		virtual event PlayingFinishedEventHandler^ PlayingFinished;

		/// <summary>
        /// Video source.
        /// </summary>
        /// 
        /// <remarks><para>Video file name to play.</para></remarks>
        /// 
		property String^ Source
		{
			virtual String^ get( )
			{
				return m_fileName;
			}
			void set( String^ fileName )
			{
				m_fileName = fileName;
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
		property int FramesReceived
		{
			virtual int get( )
			{
				int frames = m_framesReceived;
				m_framesReceived = 0;
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
		property long long BytesReceived
		{
			virtual long long get( )
			{
				return 0;
			}
		}

        /// <summary>
        /// State of the video source.
        /// </summary>
        /// 
        /// <remarks>Current state of video source object - running or not.</remarks>
        /// 
		property bool IsRunning
		{
			virtual bool get( )
			{
				if ( m_workerThread != nullptr )
				{
					// check if the thread is still running
					if ( m_workerThread->Join( 0 ) == false )
						return true;

					Free( );
				}
				return false;
			}
		}

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
		/// <para><note>Setting this property has effect only when <see cref="FrameIntervalFromSource"/>
		/// is set to <see langword="false"/>.</note></para>
        /// 
        /// <para>Default value is set to <b>0</b>.</para>
        /// </remarks>
        /// 
        property int FrameInterval
        {
            int get( )
			{
				return m_frameInterval;
			}
            void set( int frameInterval )
			{
				m_frameInterval = frameInterval;
			}
        }

        /// <summary>
        /// Get frame interval from source or use manually specified.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies which frame rate to use for video playing.
        /// If the property is set to <see langword="true"/>, then video is played
        /// with original frame rate, which is set in source video file. If the property is
        /// set to <see langword="false"/>, then custom frame rate is used, which is
        /// calculated based on the manually specified <see cref="FrameInterval">frame interval</see>.</para>
        /// 
        /// <para>Default value is set to <see langword="true"/>.</para>
        /// </remarks>
        /// 
        property bool FrameIntervalFromSource
        {
            bool get( )
			{
				return m_frameIntervalFromSource;
			}
            void set( bool fpsFromSource )
			{
				m_frameIntervalFromSource = fpsFromSource;
			}
        }

	public:

		/// <summary>
        /// Initializes a new instance of the <see cref="VideoFileSource"/> class.
        /// </summary>
        /// 
		VideoFileSource( String^ fileName );

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
		virtual void Start( );

        /// <summary>
        /// Signal video source to stop its work.
        /// </summary>
        /// 
        /// <remarks>Signals video source to stop its background thread, stop to
        /// provide new frames and free resources.</remarks>
        /// 
		virtual void SignalToStop( );

        /// <summary>
        /// Wait for video source has stopped.
        /// </summary>
        /// 
        /// <remarks>Waits for source stopping after it was signalled to stop using
        /// <see cref="SignalToStop"/> method.</remarks>
        /// 
		virtual void WaitForStop( );

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
		virtual void Stop( );

	private:
		String^	m_fileName;
		Thread^	m_workerThread;
		ManualResetEvent^ m_needToStop;

		int  m_framesReceived;
        int  m_bytesReceived;
		bool m_frameIntervalFromSource;
		int  m_frameInterval;


	private:
		void Free( );
		void WorkerThreadHandler( );
	};

} } }
