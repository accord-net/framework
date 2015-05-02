// AForge Video for Windows Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com

//
namespace AForge.Video.VFW
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Windows API functions and structures.
    /// </summary>
    /// 
    /// <remarks>The class provides Video for Windows and some other Win32 functions and structurs.</remarks>
    /// 
    internal static class Win32
    {
        /// <summary>
        /// Copy a block of memory.
        /// </summary>
        /// 
        /// <param name="dst">Destination pointer.</param>
        /// <param name="src">Source pointer.</param>
        /// <param name="count">Memory block's length to copy.</param>
        /// 
        /// <returns>Return's the value of <b>dst</b> - pointer to destination.</returns>
        /// 
        [DllImport( "ntdll.dll", CallingConvention = CallingConvention.Cdecl )]
        public static extern int memcpy(
            int dst,
            int src,
            int count );


        // --- Video for Windows Functions

        /// <summary>
        /// Initialize the AVIFile library.
        /// </summary>
        /// 
        [DllImport( "avifil32.dll" )]
        public static extern void AVIFileInit( );

        /// <summary>
        /// Exit the AVIFile library.
        /// </summary>
        [DllImport( "avifil32.dll" )]
        public static extern void AVIFileExit( );

        /// <summary>
        /// Open an AVI file.
        /// </summary>
        /// 
        /// <param name="aviHandler">Opened AVI file interface.</param>
        /// <param name="fileName">AVI file name.</param>
        /// <param name="mode">Opening mode (see <see cref="OpenFileMode"/>).</param>
        /// <param name="handler">Handler to use (<b>null</b> to use default).</param>
        /// 
        /// <returns>Returns zero on success or error code otherwise.</returns>
        /// 
        [DllImport( "avifil32.dll", CharSet = CharSet.Unicode )]
        public static extern int AVIFileOpen(
            out IntPtr aviHandler,
            String fileName,
            OpenFileMode mode,
            IntPtr handler );

        /// <summary>
        /// Release an open AVI stream.
        /// </summary>
        /// 
        /// <param name="aviHandler">Open AVI file interface.</param>
        /// 
        /// <returns>Returns the reference count of the file.</returns>
        /// 
        [DllImport( "avifil32.dll" )]
        public static extern int AVIFileRelease(
            IntPtr aviHandler );

        /// <summary>
        /// Get stream interface that is associated with a specified AVI file
        /// </summary>
        /// 
        /// <param name="aviHandler">Handler to an open AVI file.</param>
        /// <param name="streamHandler">Stream interface.</param>
        /// <param name="streamType">Stream type to open.</param>
        /// <param name="streamNumner">Count of the stream type. Identifies which occurrence of the specified stream type to access. </param>
        /// 
        /// <returns></returns>
        /// 
        [DllImport( "avifil32.dll" )]
        public static extern int AVIFileGetStream(
            IntPtr aviHandler,
            out IntPtr streamHandler,
            int streamType,
            int streamNumner );

        /// <summary>
        /// Create a new stream in an existing file and creates an interface to the new stream. 
        /// </summary>
        /// 
        /// <param name="aviHandler">Handler to an open AVI file.</param>
        /// <param name="streamHandler">Stream interface.</param>
        /// <param name="streamInfo">Pointer to a structure containing information about the new stream.</param>
        /// 
        /// <returns>Returns zero if successful or an error otherwise.</returns>
        /// 
        [DllImport( "avifil32.dll" )]
        public static extern int AVIFileCreateStream(
            IntPtr aviHandler,
            out IntPtr streamHandler,
            ref AVISTREAMINFO streamInfo );

        /// <summary>
        /// Release an open AVI stream.
        /// </summary>
        /// 
        /// <param name="streamHandler">Handle to an open stream.</param>
        /// 
        /// <returns>Returns the current reference count of the stream.</returns>
        /// 
        [DllImport( "avifil32.dll" )]
        public static extern int AVIStreamRelease(
            IntPtr streamHandler );

        /// <summary>
        /// Set the format of a stream at the specified position.
        /// </summary>
        /// 
        /// <param name="streamHandler">Handle to an open stream.</param>
        /// <param name="position">Position in the stream to receive the format.</param>
        /// <param name="format">Pointer to a structure containing the new format.</param>
        /// <param name="formatSize">Size, in bytes, of the block of memory referenced by <b>format</b>.</param>
        /// 
        /// <returns>Returns zero if successful or an error otherwise.</returns>
        /// 
        [DllImport( "avifil32.dll" )]
        public static extern int AVIStreamSetFormat(
            IntPtr streamHandler,
            int position,
            ref BITMAPINFOHEADER format,
            int formatSize );

        /// <summary>
        /// Get the starting sample number for the stream.
        /// </summary>
        /// 
        /// <param name="streamHandler">Handle to an open stream.</param>
        /// 
        /// <returns>Returns the number if successful or – 1 otherwise.</returns>
        /// 
        [DllImport( "avifil32.dll" )]
        public static extern int AVIStreamStart(
            IntPtr streamHandler );

        /// <summary>
        /// Get the length of the stream.
        /// </summary>
        /// 
        /// <param name="streamHandler">Handle to an open stream.</param>
        /// 
        /// <returns>Returns the stream's length, in samples, if successful or -1 otherwise. </returns>
        /// 
        [DllImport( "avifil32.dll" )]
        public static extern int AVIStreamLength(
            IntPtr streamHandler );

        /// <summary>
        /// Obtain stream header information.
        /// </summary>
        /// 
        /// <param name="streamHandler">Handle to an open stream.</param>
        /// <param name="streamInfo">Pointer to a structure to contain the stream information.</param>
        /// <param name="infoSize">Size, in bytes, of the structure used for <b>streamInfo</b>.</param>
        /// 
        /// <returns>Returns zero if successful or an error otherwise.</returns>
        /// 
        [DllImport( "avifil32.dll", CharSet = CharSet.Unicode )]
        public static extern int AVIStreamInfo(
            IntPtr streamHandler,
            ref AVISTREAMINFO streamInfo,
            int infoSize );

        /// <summary>
        /// Prepare to decompress video frames from the specified video stream
        /// </summary>
        /// 
        /// <param name="streamHandler">Pointer to the video stream used as the video source.</param>
        /// <param name="wantedFormat">Pointer to a structure that defines the desired video format. Specify NULL to use a default format.</param>
        /// 
        /// <returns>Returns an object that can be used with the <see cref="AVIStreamGetFrame"/> function.</returns>
        /// 
        [DllImport( "avifil32.dll" )]
        public static extern IntPtr AVIStreamGetFrameOpen(
            IntPtr streamHandler,
            ref BITMAPINFOHEADER wantedFormat );

        /// <summary>
        /// Prepare to decompress video frames from the specified video stream.
        /// </summary>
        /// 
        /// <param name="streamHandler">Pointer to the video stream used as the video source.</param>
        /// <param name="wantedFormat">Pointer to a structure that defines the desired video format. Specify NULL to use a default format.</param>
        /// 
        /// <returns>Returns a <b>GetFrame</b> object that can be used with the <see cref="AVIStreamGetFrame"/> function.</returns>
        /// 
        [DllImport( "avifil32.dll" )]
        public static extern IntPtr AVIStreamGetFrameOpen(
            IntPtr streamHandler,
            int wantedFormat );

        /// <summary>
        /// Releases resources used to decompress video frames.
        /// </summary>
        /// 
        /// <param name="getFrameObject">Handle returned from the <see cref="AVIStreamGetFrameOpen(IntPtr,int)"/> function.</param>
        /// 
        /// <returns>Returns zero if successful or an error otherwise.</returns>
        /// 
        [DllImport( "avifil32.dll" )]
        public static extern int AVIStreamGetFrameClose(
            IntPtr getFrameObject );

        /// <summary>
        /// Return the address of a decompressed video frame. 
        /// </summary>
        /// 
        /// <param name="getFrameObject">Pointer to a GetFrame object.</param>
        /// <param name="position">Position, in samples, within the stream of the desired frame.</param>
        /// 
        /// <returns>Returns a pointer to the frame data if successful or NULL otherwise.</returns>
        /// 
        [DllImport( "avifil32.dll" )]
        public static extern IntPtr AVIStreamGetFrame(
            IntPtr getFrameObject,
            int position );

        /// <summary>
        /// Write data to a stream.
        /// </summary>
        /// 
        /// <param name="streamHandler">Handle to an open stream.</param>
        /// <param name="start">First sample to write.</param>
        /// <param name="samples">Number of samples to write.</param>
        /// <param name="buffer">Pointer to a buffer containing the data to write. </param>
        /// <param name="bufferSize">Size of the buffer referenced by <b>buffer</b>.</param>
        /// <param name="flags">Flag associated with this data.</param>
        /// <param name="samplesWritten">Pointer to a buffer that receives the number of samples written. This can be set to NULL.</param>
        /// <param name="bytesWritten">Pointer to a buffer that receives the number of bytes written. This can be set to NULL.</param>
        /// 
        /// <returns>Returns zero if successful or an error otherwise.</returns>
        /// 
        [DllImport( "avifil32.dll" )]
        public static extern int AVIStreamWrite(
            IntPtr streamHandler,
            int start,
            int samples,
            IntPtr buffer,
            int bufferSize,
            int flags,
            IntPtr samplesWritten,
            IntPtr bytesWritten );

        /// <summary>
        /// Retrieve the save options for a file and returns them in a buffer.
        /// </summary>
        /// 
        /// <param name="window">Handle to the parent window for the Compression Options dialog box.</param>
        /// <param name="flags">Flags for displaying the Compression Options dialog box.</param>
        /// <param name="streams">Number of streams that have their options set by the dialog box.</param>
        /// <param name="streamInterfaces">Pointer to an array of stream interface pointers.</param>
        /// <param name="options">Pointer to an array of pointers to AVICOMPRESSOPTIONS structures.</param>
        /// 
        /// <returns>Returns TRUE if the user pressed OK, FALSE for CANCEL, or an error otherwise.</returns>
        /// 
        [DllImport( "avifil32.dll" )]
        public static extern int AVISaveOptions(
            IntPtr window,
            int flags,
            int streams,
            [In, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] IntPtr[] streamInterfaces,
            [In, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] IntPtr[] options );

        /// <summary>
        /// Free the resources allocated by the AVISaveOptions function. 
        /// </summary>
        /// 
        /// <param name="streams">Count of the AVICOMPRESSOPTIONS structures referenced in <b>options</b>.</param>
        /// <param name="options">Pointer to an array of pointers to AVICOMPRESSOPTIONS structures.</param>
        /// 
        /// <returns>Returns 0.</returns>
        /// 
        [DllImport( "avifil32.dll" )]
        public static extern int AVISaveOptionsFree(
            int streams,
            [In, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] IntPtr[] options );

        /// <summary>
        /// Create a compressed stream from an uncompressed stream and a
        /// compression filter, and returns the address of a pointer to
        /// the compressed stream.
        /// </summary>
        /// 
        /// <param name="compressedStream">Pointer to a buffer that receives the compressed stream pointer.</param>
        /// <param name="sourceStream">Pointer to the stream to be compressed.</param>
        /// <param name="options">Pointer to a structure that identifies the type of compression to use and the options to apply.</param>
        /// <param name="clsidHandler">Pointer to a class identifier used to create the stream.</param>
        /// 
        /// <returns>Returns 0 if successful or an error otherwise.</returns>
        /// 
        [DllImport( "avifil32.dll" )]
        public static extern int AVIMakeCompressedStream(
            out IntPtr compressedStream,
            IntPtr sourceStream,
            ref AVICOMPRESSOPTIONS options,
            IntPtr clsidHandler );

        // --- structures

        /// <summary>
        /// Structure to define the coordinates of the upper-left and
        /// lower-right corners of a rectangle. 
        /// </summary>
        /// 
        [StructLayout( LayoutKind.Sequential, Pack = 1 )]
        public struct RECT
        {
            /// <summary>
            /// x-coordinate of the upper-left corner of the rectangle.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int left;

            /// <summary>
            /// y-coordinate of the upper-left corner of the rectangle.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int top;

            /// <summary>
            /// x-coordinate of the bottom-right corner of the rectangle.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int right;

            /// <summary>
            /// y-coordinate of the bottom-right corner of the rectangle.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int bottom;
        }

        /// <summary>
        /// Structure, which contains information for a single stream .
        /// </summary>
        /// 
        [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1 )]
        public struct AVISTREAMINFO
        {
            /// <summary>
            /// Four-character code indicating the stream type.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int type;

            /// <summary>
            /// Four-character code of the compressor handler that will compress this video stream when it is saved.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int handler;

            /// <summary>
            /// Applicable flags for the stream.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int flags;

            /// <summary>
            /// Capability flags; currently unused.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int Capabilities;

            /// <summary>
            /// Priority of the stream.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I2 )]
            public short priority;

            /// <summary>
            /// Language of the stream.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I2 )]
            public short language;

            /// <summary>
            /// Time scale applicable for the stream.
            /// </summary>
            /// 
            /// <remarks>Dividing <b>rate</b> by <b>scale</b> gives the playback rate in number of samples per second.</remarks>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int scale;

            /// <summary>
            /// Rate in an integer format.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int rate;

            /// <summary>
            /// Sample number of the first frame of the AVI file.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int start;

            /// <summary>
            /// Length of this stream.
            /// </summary>
            /// 
            /// <remarks>The units are defined by <b>rate</b> and <b>scale</b>.</remarks>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int length;

            /// <summary>
            /// Audio skew. This member specifies how much to skew the audio data ahead of the video frames in interleaved files.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int initialFrames;

            /// <summary>
            /// Recommended buffer size, in bytes, for the stream.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int suggestedBufferSize;

            /// <summary>
            /// Quality indicator of the video data in the stream.
            /// </summary>
            /// 
            /// <remarks>Quality is represented as a number between 0 and 10,000.</remarks>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int quality;

            /// <summary>
            /// Size, in bytes, of a single data sample.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int sampleSize;

            /// <summary>
            /// Dimensions of the video destination rectangle.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.Struct, SizeConst = 16 )]
            public RECT rectFrame;

            /// <summary>
            /// Number of times the stream has been edited.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int editCount;

            /// <summary>
            /// Number of times the stream format has changed.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int formatChangeCount;

            /// <summary>
            /// Description of the stream.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 64 )]
            public string name;
        }

        /// <summary>
        /// Structure, which contains information about the dimensions and color format of a DIB.
        /// </summary>
        /// 
        [StructLayout( LayoutKind.Sequential, Pack = 1 )]
        public struct BITMAPINFOHEADER
        {
            /// <summary>
            /// Specifies the number of bytes required by the structure.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int size;

            /// <summary>
            /// Specifies the width of the bitmap, in pixels.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int width;

            /// <summary>
            /// Specifies the height of the bitmap, in pixels.
            /// </summary>
            /// 
            /// <remarks>If <b>heigh</b>t is positive, the bitmap is a bottom-up DIB and its origin is
            /// the lower-left corner. If <b>height</b> is negative, the bitmap is a top-down DIB and its
            /// origin is the upper-left corner.</remarks>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int height;

            /// <summary>
            /// Specifies the number of planes for the target device. This value must be set to 1.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I2 )]
            public short planes;

            /// <summary>
            /// Specifies the number of bits-per-pixel.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I2 )]
            public short bitCount;

            /// <summary>
            /// Specifies the type of compression for a compressed bottom-up bitmap (top-down DIBs cannot be compressed).
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int compression;

            /// <summary>
            /// Specifies the size, in bytes, of the image.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int sizeImage;

            /// <summary>
            /// Specifies the horizontal resolution, in pixels-per-meter, of the target device for the bitmap.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int xPelsPerMeter;

            /// <summary>
            /// Specifies the vertical resolution, in pixels-per-meter, of the target device for the bitmap.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int yPelsPerMeter;

            /// <summary>
            /// Specifies the number of color indexes in the color table that are actually used by the bitmap.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int colorsUsed;

            /// <summary>
            /// Specifies the number of color indexes that are required for displaying the bitmap.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int colorsImportant;
        }

        /// <summary>
        /// Structure, which contains information about a stream and how it is compressed and saved. 
        /// </summary>
        /// 
        [StructLayout( LayoutKind.Sequential, Pack = 1 )]
        public struct AVICOMPRESSOPTIONS
        {
            /// <summary>
            /// Four-character code indicating the stream type.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int type;

            /// <summary>
            /// Four-character code for the compressor handler that will compress this video stream when it is saved.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int handler;

            /// <summary>
            /// Maximum period between video key frames.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int keyFrameEvery;

            /// <summary>
            /// Quality value passed to a video compressor.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int quality;

            /// <summary>
            /// Video compressor data rate.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int bytesPerSecond;

            /// <summary>
            /// Flags used for compression.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int flags;

            /// <summary>
            /// Pointer to a structure defining the data format.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int format;

            /// <summary>
            /// Size, in bytes, of the data referenced by <b>format</b>.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int formatSize;

            /// <summary>
            /// Video-compressor-specific data; used internally.
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int parameters;

            /// <summary>
            /// Size, in bytes, of the data referenced by <b>parameters</b>.
            /// </summary>
            [MarshalAs( UnmanagedType.I4 )]
            public int parametersSize;

            /// <summary>
            /// Interleave factor for interspersing stream data with data from the first stream. 
            /// </summary>
            /// 
            [MarshalAs( UnmanagedType.I4 )]
            public int interleaveEvery;
        }

        // --- enumerations

        /// <summary>
        /// File access modes. 
        /// </summary>
        /// 
        [Flags]
        public enum OpenFileMode
        {
            Read = 0x00000000,
            Write = 0x00000001,
            ReadWrite = 0x00000002,
            ShareCompat = 0x00000000,
            ShareExclusive = 0x00000010,
            ShareDenyWrite = 0x00000020,
            ShareDenyRead = 0x00000030,
            ShareDenyNone = 0x00000040,
            Parse = 0x00000100,
            Delete = 0x00000200,
            Verify = 0x00000400,
            Cancel = 0x00000800,
            Create = 0x00001000,
            Prompt = 0x00002000,
            Exist = 0x00004000,
            Reopen = 0x00008000
        }

        /// <summary>
        /// .NET replacement of mmioFOURCC macros. Converts four characters to code.
        /// </summary>
        /// 
        /// <param name="str">Four characters string.</param>
        /// 
        /// <returns>Returns the code created from provided characters.</returns>
        /// 
        public static int mmioFOURCC( string str )
        {
            return (
                ( (int) (byte) ( str[0] ) ) |
                ( (int) (byte) ( str[1] ) << 8 ) |
                ( (int) (byte) ( str[2] ) << 16 ) |
                ( (int) (byte) ( str[3] ) << 24 ) );
        }

        /// <summary>
        /// Inverse to <see cref="mmioFOURCC"/>. Converts code to fout characters string.
        /// </summary>
        /// 
        /// <param name="code">Code to convert.</param>
        /// 
        /// <returns>Returns four characters string.</returns>
        /// 
        public static string decode_mmioFOURCC( int code )
        {
            char[] chs = new char[4];

            for ( int i = 0; i < 4; i++ )
            {
                chs[i] = (char) (byte) ( ( code >> ( i << 3 ) ) & 0xFF );
                if ( !char.IsLetterOrDigit( chs[i] ) )
                    chs[i] = ' ';
            }
            return new string( chs );
        }


        /// <summary>
        /// Version of <see cref="AVISaveOptions(IntPtr, int, int, IntPtr[], IntPtr[])"/> for one stream only.
        /// </summary>
        /// 
        /// <param name="stream">Stream to configure.</param>
        /// <param name="options">Stream options.</param>
        /// 
        /// <returns>Returns TRUE if the user pressed OK, FALSE for CANCEL, or an error otherwise.</returns>
        /// 
        public static int AVISaveOptions( IntPtr stream, ref AVICOMPRESSOPTIONS options )
        {
            IntPtr[] streams = new IntPtr[1];
            IntPtr[] infPtrs = new IntPtr[1];
            IntPtr mem;
            int ret;

            // alloc unmanaged memory
            mem = Marshal.AllocHGlobal( Marshal.SizeOf( typeof( AVICOMPRESSOPTIONS ) ) );

            // copy from managed structure to unmanaged memory
            Marshal.StructureToPtr( options, mem, false );

            streams[0] = stream;
            infPtrs[0] = mem;

            // show dialog with a list of available compresors and configuration
            ret = AVISaveOptions( IntPtr.Zero, 0, 1, streams, infPtrs );

            // copy from unmanaged memory to managed structure
            options = (AVICOMPRESSOPTIONS) Marshal.PtrToStructure( mem, typeof( AVICOMPRESSOPTIONS ) );

            // free AVI compression options
            AVISaveOptionsFree( 1, infPtrs );

            // clear it, because the information already freed by AVISaveOptionsFree
            options.format = 0;
            options.parameters = 0;

            // free unmanaged memory
            Marshal.FreeHGlobal( mem );

            return ret;
        }
    }
}
