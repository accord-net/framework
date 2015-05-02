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
    using System.Runtime.InteropServices;
    using AForge;

    /// <summary>
    /// AVI files reading using Video for Windows.
    /// </summary>
    /// 
    /// <remarks><para>The class allows to read AVI files using Video for Windows API.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // instantiate AVI reader
    /// AVIReader reader = new AVIReader( );
    /// // open video file
    /// reader.Open( "test.avi" );
    /// // read the video file
    /// while ( reader.Position - reader.Start &lt; reader.Length )
    /// {
    ///     // get next frame
    ///     Bitmap image = reader.GetNextFrame( );
    ///     // .. process the frame somehow or display it
    /// }
    /// reader.Close( );
    /// </code>
    /// </remarks>
    /// 
    public class AVIReader : IDisposable
    {
        // AVI file
        private IntPtr file;
        // video stream
        private IntPtr stream;
        // get frame object
        private IntPtr getFrame;

        // width of video frames
        private int width;
        // height of vide frames
        private int height;
        // current position in video stream
        private int position;
        // starting position in video stream
        private int start;
        // length of video stream
        private int length;
        // frame rate
        private float rate;
        // codec used for video compression
        private string codec;

        // dummy object to lock for synchronization
        private object sync = new object( );

        /// <summary>
        /// Width of video frames.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies the width of video frames within the opened video
        /// file.</para></remarks>
        /// 
        public int Width
        {
            get { return width; }
        }

        /// <summary>
        /// Height of video frames.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies the height of video frames within the opened video
        /// file.</para></remarks>
        /// 
        public int Height
        {
            get { return height; }
        }

        /// <summary>
        /// Current position in video stream.
        /// </summary>
        /// 
        /// <remarks>Setting position outside of video range, will lead to reseting position to the start.</remarks>
        /// 
        public int Position
        {
            get { return position; }
            set { position = ( ( value < start ) || ( value >= start + length ) ) ? start : value; }
        }

        /// <summary>
        /// Starting position of video stream.
        /// </summary>
        /// 
        public int Start
        {
            get { return start; }
        }

        /// <summary>
        /// Video stream length.
        /// </summary>
        /// 
        public int Length
        {
            get { return length; }
        }

        /// <summary>
        /// Desired playing frame rate.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies the frame rate, which should be used to play the opened video
        /// file.</para></remarks>
        /// 
        public float FrameRate
        {
            get { return rate; }
        }

        /// <summary>
        /// Codec used for video compression.
        /// </summary>
        /// 
        /// <remarks><para>The property tells about which codec was used to encode the opened video file.</para></remarks>
        /// 
        public string Codec
        {
            get { return codec; }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="AVIReader"/> class.
        /// </summary>
        /// 
        /// <remarks>Initializes Video for Windows library.</remarks>
        /// 
        public AVIReader( )
        {
            Win32.AVIFileInit( );
        }

        /// <summary>
        /// Destroys the instance of the <see cref="AVIReader"/> class.
        /// </summary>
        /// 
        ~AVIReader( )
        {
            Dispose( false );
        }

        /// <summary>
        /// Dispose the object.
        /// </summary>
        /// 
        /// <remarks>Frees unmanaged resources used by the object. The object becomes unusable
        /// after that.</remarks>
        /// 
        public void Dispose( )
        {
            Dispose( true );
            // remove me from the Finalization queue 
            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// Dispose the object.
        /// </summary>
        /// 
        /// <param name="disposing">Indicates if disposing was initiated manually.</param>
        /// 
        protected virtual void Dispose( bool disposing )
        {
            if ( disposing )
            {
                // dispose managed resources
            }
            // close current AVI file if any opened and uninitialize AVI library
            Close( );
            Win32.AVIFileExit( );
        }

        /// <summary>
        /// Open AVI file.
        /// </summary>
        /// 
        /// <param name="fileName">AVI file name to open.</param>
        /// 
        /// <remarks><para>The method opens a video file and prepares the stream and decoder for
        /// reading video frames with the help of <see cref="GetNextFrame"/> method.</para>
        /// </remarks>
        /// 
        /// <exception cref="System.IO.IOException">Failed opening the specified file.</exception>
        /// <exception cref="VideoException">A error occurred while opening the video file. See exception message.</exception>
        ///
        /// 
        public void Open( string fileName )
        {
            // close previous file
            Close( );

            bool success = false;

            try
            {
                lock ( sync )
                {
                    // open AVI file
                    if ( Win32.AVIFileOpen( out file, fileName, Win32.OpenFileMode.ShareDenyWrite, IntPtr.Zero ) != 0 )
                        throw new System.IO.IOException( "Failed opening the specified AVI file." );

                    // get first video stream
                    if ( Win32.AVIFileGetStream( file, out stream, Win32.mmioFOURCC( "vids" ), 0 ) != 0 )
                        throw new VideoException( "Failed getting video stream." );

                    // get stream info
                    Win32.AVISTREAMINFO info = new Win32.AVISTREAMINFO( );
                    Win32.AVIStreamInfo( stream, ref info, Marshal.SizeOf( info ) );

                    width    = info.rectFrame.right;
                    height   = info.rectFrame.bottom;
                    position = info.start;
                    start    = info.start;
                    length   = info.length;
                    rate     = (float) info.rate / (float) info.scale;
                    codec    = Win32.decode_mmioFOURCC( info.handler );

                    // prepare decompressor
                    Win32.BITMAPINFOHEADER bitmapInfoHeader = new Win32.BITMAPINFOHEADER( );

                    bitmapInfoHeader.size        = Marshal.SizeOf( bitmapInfoHeader.GetType( ) );
                    bitmapInfoHeader.width       = width;
                    bitmapInfoHeader.height      = height;
                    bitmapInfoHeader.planes      = 1;
                    bitmapInfoHeader.bitCount    = 24;
                    bitmapInfoHeader.compression = 0; // BI_RGB

                    // get frame object
                    if ( ( getFrame = Win32.AVIStreamGetFrameOpen( stream, ref bitmapInfoHeader ) ) == IntPtr.Zero )
                    {
                        bitmapInfoHeader.height = -height;

                        if ( ( getFrame = Win32.AVIStreamGetFrameOpen( stream, ref bitmapInfoHeader ) ) == IntPtr.Zero )
                            throw new VideoException( "Failed initializing decompressor." );
                    }

                    success = true;
                }
            }
            finally
            {
                if ( !success )
                {
                    Close( );
                }
            }
        }

        /// <summary>
        /// Close video file.
        /// </summary>
        /// 
        public void Close( )
        {
            lock ( sync )
            {
                // release get frame object
                if ( getFrame != IntPtr.Zero )
                {
                    Win32.AVIStreamGetFrameClose( getFrame );
                    getFrame = IntPtr.Zero;
                }

                // release stream
                if ( stream != IntPtr.Zero )
                {
                    Win32.AVIStreamRelease( stream );
                    stream = IntPtr.Zero;
                }

                // release file
                if ( file != IntPtr.Zero )
                {
                    Win32.AVIFileRelease( file );
                    file = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        /// Get next frame of opened video stream.
        /// </summary>
        /// 
        /// <returns>Returns next frame as a bitmap.</returns>
        /// 
        /// <remarks><para>The method reads and returns the next video frame in the opened video stream
        /// at the position, which is set in <see cref="Position"/> property.</para></remarks>
        /// 
        /// <exception cref="System.IO.IOException">Thrown if no video file was open.</exception>
        /// <exception cref="VideoException">A error occurred while reading next video frame. See exception message.</exception>
        /// 
        public Bitmap GetNextFrame( )
        {
            lock ( sync )
            {
                if ( file == IntPtr.Zero )
                {
                    throw new System.IO.IOException( "Cannot read video frames since video file is not open." );
                }

                // get frame at specified position
                IntPtr DIB = Win32.AVIStreamGetFrame( getFrame, position );
                if ( DIB == IntPtr.Zero )
                    throw new VideoException( "Failed getting frame." );

                Win32.BITMAPINFOHEADER bitmapInfoHeader;

                // copy BITMAPINFOHEADER from unmanaged memory
                bitmapInfoHeader = (Win32.BITMAPINFOHEADER) Marshal.PtrToStructure( DIB, typeof( Win32.BITMAPINFOHEADER ) );

                // create new bitmap
                Bitmap image = new Bitmap( width, height, PixelFormat.Format24bppRgb );

                // lock bitmap data
                BitmapData imageData = image.LockBits(
                    new Rectangle( 0, 0, width, height ),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format24bppRgb );

                // copy image data
                int srcStride = imageData.Stride;
                int dstStride = imageData.Stride;

                // check image direction
                if ( bitmapInfoHeader.height > 0 )
                {
                    // it`s a bottom-top image
                    int dst = imageData.Scan0.ToInt32( ) + dstStride * ( height - 1 );
                    int src = DIB.ToInt32( ) + Marshal.SizeOf( typeof( Win32.BITMAPINFOHEADER ) );

                    for ( int y = 0; y < height; y++ )
                    {
                        Win32.memcpy( dst, src, srcStride );
                        dst -= dstStride;
                        src += srcStride;
                    }
                }
                else
                {
                    // it`s a top bootom image
                    int dst = imageData.Scan0.ToInt32( );
                    int src = DIB.ToInt32( ) + Marshal.SizeOf( typeof( Win32.BITMAPINFOHEADER ) );

                    // copy the whole image
                    Win32.memcpy( dst, src, srcStride * height );
                }

                // unlock bitmap data
                image.UnlockBits( imageData );

                // move position to the next frame
                position++;

                return image;
            }
        }
    }
}
