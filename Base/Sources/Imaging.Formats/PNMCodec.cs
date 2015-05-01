// AForge Image Formats Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Formats
{
    using System;
    using System.IO;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Text;
    using System.ComponentModel;

    /// <summary>
    /// Information about PNM image's frame.
    /// </summary>
    public sealed class PNMImageInfo : ImageInfo
    {
        // PNM file version (1-6)
        private int version;
        // maximum data value
        private int maxDataValue;

        /// <summary>
        /// PNM file version (format), [1, 6].
        /// </summary>
        [Category( "PNM Info" )]
        public int Version
        {
            get { return version; }
            set { version = value; }
        }

        /// <summary>
        /// Maximum pixel's value in source PNM image.
        /// </summary>
        /// 
        /// <remarks><para>The value is used to scale image's data converting them
        /// from original data range to the range of
        /// <see cref="ImageInfo.BitsPerPixel">supported bits per pixel</see> format.</para></remarks>
        /// 
        [Category( "PNM Info" )]
        public int MaxDataValue
        {
            get { return maxDataValue; }
            set { maxDataValue = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PNMImageInfo"/> class.
        /// </summary>
        /// 
        public PNMImageInfo( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PNMImageInfo"/> class.
        /// </summary>
        /// 
        /// <param name="width">Image's width.</param>
        /// <param name="height">Image's height.</param>
        /// <param name="bitsPerPixel">Number of bits per image's pixel.</param>
        /// <param name="frameIndex">Frame's index.</param>
        /// <param name="totalFrames">Total frames in the image.</param>
        /// 
        public PNMImageInfo( int width, int height, int bitsPerPixel, int frameIndex, int totalFrames ) :
            base( width, height, bitsPerPixel, frameIndex, totalFrames ) { }

        /// <summary>
        /// Creates a new object that is a copy of the current instance. 
        /// </summary>
        /// 
        /// <returns>A new object that is a copy of this instance.</returns>
        /// 
        public override object Clone( )
        {
            PNMImageInfo clone = new PNMImageInfo( width, height, bitsPerPixel, frameIndex, totalFrames );

            clone.version = version;
            clone.maxDataValue = maxDataValue;

            return clone;
        }
    }

    /// <summary>
    /// PNM image format decoder.
    /// </summary>
    /// 
    /// <remarks><para>The PNM (an acronym derived from "Portable Any Map") format is an
    /// abstraction of the PBM, PGM and PPM formats. I.e. the name "PNM" refers collectively
    /// to PBM (binary images), PGM (grayscale images) and PPM (color image) image formats.</para>
    /// 
    /// <para>Image in PNM format can be found in different scientific databases and laboratories,
    /// for example <i>Yale Face Database</i> and <i>AT&amp;T Face Database</i>.</para>
    /// 
    /// <para><note>Only PNM images of P5 (binary encoded PGM) and P6 (binary encoded PPM) formats
    /// are supported at this point.</note></para>
    /// 
    /// <para><note>The maximum supported pixel value is 255 at this point.</note></para>
    /// 
    /// <para><note>The class supports only one-frame PNM images. As it is specified in format
    /// specification, the multi-frame PNM images has appeared starting from 2000.</note></para>
    /// 
    /// </remarks>
    /// 
    public class PNMCodec : IImageDecoder
    {
        // stream with PNM encoded data
        private Stream stream = null;
        // information about images retrieved from header
        private PNMImageInfo imageInfo = null;
        // stream position pointing to beginning of data - right after header
        private long dataPosition = 0;

        /// <summary>
        /// Decode first frame of PNM image.
        /// </summary>
        /// 
        /// <param name="stream">Source stream, which contains encoded image.</param>
        /// 
        /// <returns>Returns decoded image frame.</returns>
        /// 
        /// <exception cref="FormatException">Not a PNM image format.</exception>
        /// <exception cref="NotSupportedException">Format of the PNM image is not supported.</exception>
        /// <exception cref="ArgumentException">The stream contains invalid (broken) PNM image.</exception>
        /// 
        public Bitmap DecodeSingleFrame( Stream stream )
        {
            PNMImageInfo imageInfo = ReadHeader( stream );

            return ReadImageFrame( stream, imageInfo );
        }

        /// <summary>
        /// Open specified stream.
        /// </summary>
        /// 
        /// <param name="stream">Stream to open.</param>
        /// 
        /// <returns>Returns number of images found in the specified stream.</returns>
        /// 
        /// <exception cref="FormatException">Not a PNM image format.</exception>
        /// <exception cref="NotSupportedException">Format of the PNM image is not supported.</exception>
        /// <exception cref="ArgumentException">The stream contains invalid (broken) PNM image.</exception>
        ///
        public int Open( Stream stream )
        {
            // close previous decoding
            Close( );

            this.imageInfo    = ReadHeader( stream );
            this.stream       = stream;
            this.dataPosition = stream.Seek( 0, SeekOrigin.Current );

            return imageInfo.TotalFrames;
        }

        /// <summary>
        /// Decode specified frame.
        /// </summary>
        /// 
        /// <param name="frameIndex">Image frame to decode.</param>
        /// <param name="imageInfo">Receives information about decoded frame.</param>
        /// 
        /// <returns>Returns decoded frame.</returns>
        /// 
        /// <exception cref="NullReferenceException">No image stream was opened previously.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Stream does not contain frame with specified index.</exception>
        /// <exception cref="ArgumentException">The stream contains invalid (broken) PNM image.</exception>
        /// 
        public Bitmap DecodeFrame( int frameIndex, out ImageInfo imageInfo )
        {
            // check requested frame index
            if ( frameIndex != 0 )
            {
                throw new ArgumentOutOfRangeException( "Currently opened stream does not contain frame with specified index." );
            }

            // seek to the required frame
            stream.Seek( dataPosition, SeekOrigin.Begin );

            // read required frame
            Bitmap image = ReadImageFrame( stream, this.imageInfo );

            // provide also frame information
            imageInfo = (PNMImageInfo) this.imageInfo.Clone( );

            return image;
        }

        /// <summary>
        /// Close decoding of previously opened stream.
        /// </summary>
        /// 
        /// <remarks><para>The method does not close stream itself, but just closes
        /// decoding cleaning all associated data with it.</para></remarks>
        /// 
        public void Close( )
        {
            stream    = null;
            imageInfo = null;
        }

        // Read and process PNM header. After the header is read stream pointer will
        // point to data.
        private PNMImageInfo ReadHeader( Stream stream )
        {
            // read magic word
            byte magic1 = (byte) stream.ReadByte( );
            byte magic2 = (byte) stream.ReadByte( );

            // check if it is valid PNM image
            if ( ( magic1 != 'P' ) || ( magic2 < '1' ) || ( magic2 > '6' ) )
            {
                throw new FormatException( "The stream does not contain PNM image." );
            }

            // check if it is P5 or P6 format
            if ( ( magic2 != '5' ) && ( magic2 != '6' ) )
            {
                throw new NotSupportedException( "Format is not supported yet. Only P5 and P6 are supported for now." );
            }

            int width = 0, height = 0, maxValue = 0;

            try
            {
                // read image's width and height
                width = ReadIntegerValue( stream );
                height = ReadIntegerValue( stream );
                // read pixel's highiest value
                maxValue = ReadIntegerValue( stream );
            }
            catch
            {
                throw new ArgumentException( "The stream does not contain valid PNM image." );
            }

            // check if all attributes are valid
            if ( ( width <= 0 ) || ( height <= 0 ) || ( maxValue <= 0 ) )
            {
                throw new ArgumentException( "The stream does not contain valid PNM image." );
            }

            // check maximum pixel's value
            if ( maxValue > 255 )
            {
                throw new NotSupportedException( "255 is the maximum pixel's value, which is supported for now." );
            }

            // prepare image information
            PNMImageInfo imageInfo = new PNMImageInfo( width, height, ( magic2 == '5' ) ? 8 : 24, 0, 1 );
            imageInfo.Version = (int) ( magic2 - '0' );
            imageInfo.MaxDataValue = maxValue;

            return imageInfo;
        }

        // Read image frame from the specified stream (current stream's position is used)
        private Bitmap ReadImageFrame( Stream stream, PNMImageInfo imageInfo )
        {
            try
            {
                // decode PNM image depending on its format
                switch ( imageInfo.Version )
                {
                    case 5:
                        return ReadP5Image( stream, imageInfo.Width, imageInfo.Height, imageInfo.MaxDataValue );
                    case 6:
                        return ReadP6Image( stream, imageInfo.Width, imageInfo.Height, imageInfo.MaxDataValue );
                }
            }
            catch
            {
                throw new ArgumentException( "The stream does not contain valid PNM image." );
            }

            return null;
        }

        // Load P5 PGM image (grayscale PNM image with binary encoding)
        private unsafe Bitmap ReadP5Image( Stream stream, int width, int height, int maxValue )
        {
            double scalingFactor = (double) 255 / maxValue;

            // create new bitmap and lock it
            Bitmap image = Tools.CreateGrayscaleImage( width, height );
            BitmapData imageData = image.LockBits( new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed );

            int stride = imageData.Stride;
            byte* ptr = (byte*) imageData.Scan0.ToPointer( );

            // prepare a buffer for one line
            byte[] line = new byte[width];

            int totalBytesRead = 0, bytesRead = 0;

            // load all rows
            for ( int y = 0; y < height; y++ )
            {
                totalBytesRead = 0;
                bytesRead = 0;

                // load next line
                while ( totalBytesRead != width )
                {
                    bytesRead = stream.Read( line, totalBytesRead, width - totalBytesRead );

                    if ( bytesRead == 0 )
                    {
                        // if we've reached the end before complete image is loaded, then there should
                        // be something wrong
                        throw new Exception( );
                    }

                    totalBytesRead += bytesRead;
                }

                // fill next image row
                byte* row = ptr + stride * y;

                for ( int x = 0; x < width; x++, row++ )
                {
                    *row = (byte) ( scalingFactor * line[x] );
                }
            }

            // unlock image and return it
            image.UnlockBits( imageData );
            return image;
        }

        // Load P6 PPM image (color PNM image with binary encoding)
        private unsafe Bitmap ReadP6Image( Stream stream, int width, int height, int maxValue )
        {
            double scalingFactor = (double) 255 / maxValue;

            // create new bitmap and lock it
            Bitmap image = new Bitmap( width, height, PixelFormat.Format24bppRgb );
            BitmapData imageData = image.LockBits( new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb );

            int stride = imageData.Stride;
            byte* ptr = (byte*) imageData.Scan0.ToPointer( );

            // prepare a buffer for one line
            int lineSize = width * 3;
            byte[] line = new byte[lineSize];

            int totalBytesRead = 0, bytesRead = 0;

            // load all rows
            for ( int y = 0; y < height; y++ )
            {
                totalBytesRead = 0;
                bytesRead = 0;

                // load next line
                while ( totalBytesRead != lineSize )
                {
                    bytesRead = stream.Read( line, totalBytesRead, lineSize - totalBytesRead );

                    if ( bytesRead == 0 )
                    {
                        // if we've reached the end before complete image is loaded, then there should
                        // be something wrong
                        throw new Exception( );
                    }

                    totalBytesRead += bytesRead;
                }

                // fill next image row
                byte* row = ptr + stride * y;

                for ( int x = 0, i = 0; x < width; x++, i += 3, row += 3 )
                {
                    row[2] = (byte) ( scalingFactor * line[i] );       // red
                    row[1] = (byte) ( scalingFactor * line[i + 1] );   // green
                    row[0] = (byte) ( scalingFactor * line[i + 2] );   // blue
                }
            }

            // unlock image and return it
            image.UnlockBits( imageData );
            return image;
        }

        // Read integer ASCII value from the source stream.
        private int ReadIntegerValue( Stream stream )
        {
            byte[] buffer = new byte[256];
            int bytesRead = 1;

            // locate something, what is not spacing
            buffer[0] = SkipSpaces( stream );
            // complete reading useful value
            bytesRead += ReadUntilSpace( stream, buffer, 1 );

            return int.Parse( Encoding.ASCII.GetString( buffer, 0, bytesRead ) );
        }

        // Skip spaces (spaces, new lines, tabs and comment lines) in the specified stream
        // and return the first non-space byte. Stream's position will point to the next
        // byte coming after the first found non-space byte.
        private byte SkipSpaces( Stream stream )
        {
            byte nextByte = (byte) stream.ReadByte( );

            while ( ( nextByte == ' ' ) || ( nextByte == '\n' ) || ( nextByte == '\r' ) || ( nextByte == '\t' ) )
            {
                nextByte = (byte) stream.ReadByte( );
            }

            if ( nextByte == '#' )
            {
                // read until new line
                while ( nextByte != '\n' )
                {
                    nextByte = (byte) stream.ReadByte( );
                }
                // skip pending spaces or another comment
                return SkipSpaces( stream );
            }

            return nextByte;
        }

        // Read stream until space is found (space, new line, tab or comment). Returns
        // number of bytes read. Stream's position will point to the next
        // byte coming after the first found space byte.
        private int ReadUntilSpace( Stream stream, byte[] buffer, int start )
        {
            byte nextByte = (byte) stream.ReadByte( );
            int bytesRead = 0;

            while ( ( nextByte != ' ' ) && ( nextByte != '\n' ) && ( nextByte != '\r' ) && ( nextByte != '\t' ) && ( nextByte != '#' ) )
            {
                buffer[start + bytesRead] = nextByte;
                bytesRead++;
                nextByte = (byte) stream.ReadByte( );
            }

            return bytesRead;
        }
    }
}
