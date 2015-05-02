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
using namespace AForge::Video;

namespace AForge { namespace Video { namespace FFMPEG
{
	ref struct ReaderPrivateData;

	/// <summary>
	/// Class for reading video files utilizing FFmpeg library.
	/// </summary>
    /// 
    /// <remarks><para>The class allows to read video files using <a href="http://www.ffmpeg.org/">FFmpeg</a> library.</para>
    /// 
	/// <para><note>Make sure you have <b>FFmpeg</b> binaries (DLLs) in the output folder of your application in order
	/// to use this class successfully. <b>FFmpeg</b> binaries can be found in Externals folder provided with AForge.NET
	/// framework's distribution.</note></para>
	///
    /// <para>Sample usage:</para>
    /// <code>
    /// // create instance of video reader
    /// VideoFileReader reader = new VideoFileReader( );
    /// // open video file
    /// reader.Open( "test.avi" );
    /// // check some of its attributes
    /// Console.WriteLine( "width:  " + reader.Width );
    /// Console.WriteLine( "height: " + reader.Height );
    /// Console.WriteLine( "fps:    " + reader.FrameRate );
    /// Console.WriteLine( "codec:  " + reader.CodecName );
    /// // read 100 video frames out of it
    /// for ( int i = 0; i &lt; 100; i++ )
    /// {
    ///     Bitmap videoFrame = reader.ReadVideoFrame( );
    ///     // process the frame somehow
    ///     // ...
    /// 
    ///     // dispose the frame when it is no longer required
    ///     videoFrame.Dispose( );
    /// }
    /// reader.Close( );
	/// </code>
    /// </remarks>
	///
	public ref class VideoFileReader : IDisposable
	{
	public:

		/// <summary>
		/// Frame width of the opened video file.
		/// </summary>
		///
        /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
		///
		property int Width
		{
			int get( )
			{
				CheckIfVideoFileIsOpen( );
				return m_width;
			}
		}

		/// <summary>
		/// Frame height of the opened video file.
		/// </summary>
		///
        /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
		///
		property int Height
		{
			int get( )
			{
				CheckIfVideoFileIsOpen( );
				return m_height;
			}
		}

		/// <summary>
		/// Frame rate of the opened video file.
		/// </summary>
		///
        /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
		///
		property int FrameRate
		{
			int get( )
			{
				CheckIfVideoFileIsOpen( );
				return m_frameRate;
			}
		}

		/// <summary>
		/// Number of video frames in the opened video file.
		/// </summary>
		///
		/// <remarks><para><note><b>Warning</b>: some video file formats may report different value
		/// from the actual number of video frames in the file (subject to fix/investigate).</note></para>
		/// </remarks>
		///
        /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
		///
		property Int64 FrameCount
		{
			Int64 get( )
			{
				CheckIfVideoFileIsOpen( );
				return m_framesCount;
			}
		}

		/// <summary>
		/// Name of codec used for encoding the opened video file.
		/// </summary>
		///
        /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
		///
		property String^ CodecName
		{
			String^ get( )
			{
				CheckIfVideoFileIsOpen( );
				return m_codecName;
			}
		}

		/// <summary>
		/// The property specifies if a video file is opened or not by this instance of the class.
		/// </summary>
		property bool IsOpen
		{
			bool get ( )
			{
				return ( data != nullptr );
			}
		}

    protected:

        /// <summary>
        /// Object's finalizer.
        /// </summary>
        /// 
        !VideoFileReader( )
        {
            Close( );
        }

	public:

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoFileReader"/> class.
        /// </summary>
        /// 
		VideoFileReader( void );

        /// <summary>
        /// Disposes the object and frees its resources.
        /// </summary>
        /// 
        ~VideoFileReader( )
        {
            this->!VideoFileReader( );
            disposed = true;
        }

		/// <summary>
        /// Open video file with the specified name.
        /// </summary>
		///
		/// <param name="fileName">Video file name to open.</param>
		///
        /// <exception cref="System::IO::IOException">Cannot open video file with the specified name.</exception>
        /// <exception cref="VideoException">A error occurred while opening the video file. See exception message.</exception>
		///
		void Open( String^ fileName );

        /// <summary>
        /// Read next video frame of the currently opened video file.
        /// </summary>
		/// 
		/// <returns>Returns next video frame of the opened file or <see langword="null"/> if end of
		/// file was reached. The returned video frame has 24 bpp color format.</returns>
        /// 
        /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
        /// <exception cref="VideoException">A error occurred while reading next video frame. See exception message.</exception>
        /// 
		Bitmap^ ReadVideoFrame( );

        /// <summary>
        /// Close currently opened video file if any.
        /// </summary>
        /// 
		void Close( );

	private:

		int m_width;
		int m_height;
		int	m_frameRate;
		String^ m_codecName;
		Int64 m_framesCount;

	private:
		Bitmap^ DecodeVideoFrame( );

		// Checks if video file was opened
		void CheckIfVideoFileIsOpen( )
		{
			if ( data == nullptr )
			{
				throw gcnew System::IO::IOException( "Video file is not open, so can not access its properties." );
			}
		}

        // Check if the object was already disposed
        void CheckIfDisposed( )
        {
            if ( disposed )
            {
                throw gcnew System::ObjectDisposedException( "The object was already disposed." );
            }
        }

	private:
		// private data of the class
		ReaderPrivateData^ data;
        bool disposed;
	};

} } }
