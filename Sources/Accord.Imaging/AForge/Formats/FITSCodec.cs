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
    /// Information about FITS image's frame.
    /// </summary>
    public sealed class FITSImageInfo : ImageInfo
    {
        private int originalBitsPerPixl;
        private double minDataValue;
        private double maxDataValue;
        private string telescope;
        private string acquiredObject;
        private string observer;
        private string instrument;

        /// <summary>
        /// Original bits per pixel.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies original number of bits per image's pixel. For
        /// FITS images the value may be equal to 8, 16, 32, -32 (32 bit image with float data
        /// type for pixel encoding), -64 (64 bit image with double data type for pixel encoding).
        /// </para></remarks>
        /// 
        [Category( "FITS Info" )]
        public int OriginalBitsPerPixl
        {
            get { return originalBitsPerPixl; }
            set { originalBitsPerPixl = value; }
        }

        /// <summary>
        /// Minimum data value found during parsing FITS image.
        /// </summary>
        /// 
        /// <remarks><para>Minimum and maximum data values are used to scale image's data converting
        /// them from <see cref="OriginalBitsPerPixl">original bits per pixel</see> format to
        /// <see cref="ImageInfo.BitsPerPixel">supported bits per pixel</see> format.</para></remarks>
        /// 
        [Category( "FITS Info" )]
        public double MinDataValue
        {
            get { return minDataValue; }
            set { minDataValue = value; }
        }

        /// <summary>
        /// Maximum data value found during parsing FITS image.
        /// </summary>
        /// 
        /// <remarks><para>Minimum and maximum data values are used to scale image's data converting
        /// them from <see cref="OriginalBitsPerPixl">original bits per pixel</see> format to
        /// <see cref="ImageInfo.BitsPerPixel">supported bits per pixel</see> format.</para></remarks>
        /// 
        [Category( "FITS Info" )]
        public double MaxDataValue
        {
            get { return maxDataValue; }
            set { maxDataValue = value; }
        }

        /// <summary>
        /// Telescope used for object's observation.
        /// </summary>
        [Category( "FITS Info" )]
        public string Telescope
        {
            get { return telescope; }
            set { telescope = value; }
        }

        /// <summary>
        /// Object acquired during observation.
        /// </summary>
        [Category( "FITS Info" )]
        public string Object
        {
            get { return acquiredObject; }
            set { acquiredObject = value; }
        }

        /// <summary>
        /// Observer doing object's acquiring.
        /// </summary>
        [Category( "FITS Info" )]
        public string Observer
        {
            get { return observer; }
            set { observer = value; }
        }

        /// <summary>
        /// Instrument used for observation.
        /// </summary>
        [Category( "FITS Info" )]
        public string Instrument
        {
            get { return instrument; }
            set { instrument = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FITSImageInfo"/> class.
        /// </summary>
        /// 
        public FITSImageInfo( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FITSImageInfo"/> class.
        /// </summary>
        /// 
        /// <param name="width">Image's width.</param>
        /// <param name="height">Image's height.</param>
        /// <param name="bitsPerPixel">Number of bits per image's pixel.</param>
        /// <param name="frameIndex">Frame's index.</param>
        /// <param name="totalFrames">Total frames in the image.</param>
        /// 
        public FITSImageInfo( int width, int height, int bitsPerPixel, int frameIndex, int totalFrames ) :
            base( width, height, bitsPerPixel, frameIndex, totalFrames ) { }

        /// <summary>
        /// Creates a new object that is a copy of the current instance. 
        /// </summary>
        /// 
        /// <returns>A new object that is a copy of this instance.</returns>
        /// 
        public override object Clone( )
        {
            FITSImageInfo clone = new FITSImageInfo( width, height, bitsPerPixel, frameIndex, totalFrames );

            clone.originalBitsPerPixl = originalBitsPerPixl;
            clone.minDataValue = minDataValue;
            clone.maxDataValue = maxDataValue;
            clone.telescope = telescope;
            clone.acquiredObject = acquiredObject;
            clone.observer = observer;
            clone.instrument = instrument;

            return clone;
        }
    }
    
    /// <summary>
    /// FITS image format decoder.
    /// </summary>
    ///
    /// <remarks><para>The FITS (an acronym derived from "Flexible Image Transport System") format
    /// is an astronomical image and table format created and supported by NASA. FITS is the most
    /// commonly used in astronomy and is designed specifically for scientific data. Different astronomical
    /// organizations keep their images acquired using telescopes and other equipment in FITS format.</para>
    /// 
    /// <para><note>The class extracts image frames only from the main data section of FITS file.
    /// 2D (single frame) and 3D (series of frames) data structures are supported.</note></para>
    /// 
    /// <para><note>During image reading/parsing, its data are scaled using minimum and maximum values of
    /// the source image data. FITS tags are not used for this purpose - data are scaled from the
    /// [min, max] range found to the range of supported image format ([0, 255] for 8 bpp grayscale
    /// or [0, 65535] for 16 bpp grayscale image).</note></para>
    /// </remarks>
    ///
    public class FITSCodec : IImageDecoder
    {
        // stream with FITS encoded data
        private Stream stream = null;
        // information about images retrieved from header
        private FITSImageInfo imageInfo = null;
        // stream position pointing to beginning of data (or extension) - right after header
        private long dataPosition = 0;
        
        /// <summary>
        /// Decode first frame of FITS image.
        /// </summary>
        /// 
        /// <param name="stream">Source stream, which contains encoded image.</param>
        /// 
        /// <returns>Returns decoded image frame.</returns>
        /// 
        /// <exception cref="FormatException">Not a FITS image format.</exception>
        /// <exception cref="NotSupportedException">Format of the FITS image is not supported.</exception>
        /// <exception cref="ArgumentException">The stream contains invalid (broken) FITS image.</exception>
        /// 
        public Bitmap DecodeSingleFrame( Stream stream )
        {
            FITSImageInfo imageInfo = ReadHeader( stream );

            // check if there any image frame
            if ( imageInfo.TotalFrames == 0 )
            {
                throw new ArgumentException( "The FITS stream does not contain any image in main section." );
            }
            // read and return first frame
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
        /// <exception cref="FormatException">Not a FITS image format.</exception>
        /// <exception cref="NotSupportedException">Format of the FITS image is not supported.</exception>
        /// <exception cref="ArgumentException">The stream contains invalid (broken) FITS image.</exception>
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
        /// <exception cref="ArgumentException">The stream contains invalid (broken) FITS image.</exception>
        /// 
        public Bitmap DecodeFrame( int frameIndex, out ImageInfo imageInfo )
        {
            // check requested frame index
            if ( frameIndex >= this.imageInfo.TotalFrames )
            {
                throw new ArgumentOutOfRangeException( "Currently opened stream does not contain frame with specified index." );
            }

            // seek to the required frame
            stream.Seek( dataPosition + frameIndex * this.imageInfo.Width * this.imageInfo.Height *
                Math.Abs( this.imageInfo.OriginalBitsPerPixl ) / 8, SeekOrigin.Begin );

            // read required frame
            Bitmap image = ReadImageFrame( stream, this.imageInfo );

            // provide also frame information
            imageInfo = (FITSImageInfo) this.imageInfo.Clone( );
            imageInfo.FrameIndex = frameIndex;

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

        // Read and process FITS header. After the header is read stream pointer will
        // point to data or extension.
        private FITSImageInfo ReadHeader( Stream stream )
        {
            byte[]  headerRecord = new byte[80];
            int     recordsRead = 1;
            bool    endKeyWasFound = false;

            FITSImageInfo imageInfo = new FITSImageInfo( );

            // read first record and check for correct image
            if (
                ( Tools.ReadStream( stream, headerRecord, 0, 80 ) < 80 ) ||
                ( Encoding.ASCII.GetString( headerRecord, 0, 8 ) != "SIMPLE  " ) )
            {
                throw new FormatException( "The stream does not contatin FITS image." );
            }
            else
            {
                // check if the image has standard FITS format
                if ( Encoding.ASCII.GetString( headerRecord, 10, 70 ).Split( '/' )[0].Trim( ) != "T" )
                {
                    throw new NotSupportedException( "The stream contains not standard FITS data file." );
                }
            }

            // read header and locate data block
            while ( true )
            {
                // read next record
                if ( Tools.ReadStream( stream, headerRecord, 0, 80 ) < 80 )
                {
                    throw new ArgumentException( "The stream does not contain valid FITS image." );
                }
                recordsRead++;

                // get keyword
                string keyword = Encoding.ASCII.GetString( headerRecord, 0, 8 );

                // skip commenct and history
                if ( ( keyword == "COMMENT " ) || ( keyword == "HISTORY " ) )
                    continue;

                // check if it is end of header keyword
                if ( keyword == "END     " )
                    endKeyWasFound = true;

                if ( endKeyWasFound )
                {
                    if ( recordsRead % 36 == 0 )
                    {
                        // found data or extension header

                        // make a small check of some header values
                        if ( ( imageInfo.BitsPerPixel == 0 ) || ( imageInfo.Width == 0 ) || ( imageInfo.Height == 0 ) )
                        {
                            imageInfo.TotalFrames = 0;
                        }

                        // let's return here and let other routines process data
                        break;
                    }
                }
                else
                {
                    // get string representation of value/comments
                    string strValue = Encoding.ASCII.GetString( headerRecord, 10, 70 );

                    // check important keywords
                    if ( keyword == "BITPIX  " )
                    {
                        int value = ExtractIntegerValue( strValue );

                        if ( ( value != 8 ) && ( value != 16 ) && ( value != 32 ) && ( value != -32 ) && ( value != -64 ) )
                        {
                            throw new NotSupportedException( "Data format (" + value + ") is not supported." );
                        }

                        // bits per pixel
                        imageInfo.BitsPerPixel = ( value == 8 ) ? 8 : 16;
                        imageInfo.OriginalBitsPerPixl = value;
                    }
                    else if ( Encoding.ASCII.GetString( headerRecord, 0, 5 ) == "NAXIS" )
                    {
                        // information about data axis
                        int value = ExtractIntegerValue( strValue );

                        // check axis
                        switch ( headerRecord[5] )
                        {
                            // number of axis
                            case (byte) ' ':
                                switch ( value )
                                {
                                    case 1:
                                    default:
                                        throw new NotSupportedException( "FITS files with data dimension of " + value + " are not supported." );
                                    case 0:
                                        // the stream does not have an image, do nothing
                                        break;
                                    case 2:
                                        // the stream has 1 2D image
                                        imageInfo.TotalFrames = 1;
                                        break;
                                    case 3:
                                        // the stream has 3D image - series of 2D images
                                        break;
                                }
                                break;
                            // length of 1st axis
                            case (byte) '1':
                                imageInfo.Width = value;
                                break;
                            // length of 2nd axis
                            case (byte) '2':
                                imageInfo.Height = value;
                                break;
                            // length of 3rd axis
                            case (byte) '3':
                                imageInfo.TotalFrames = value;
                                break;
                        }
                    }
                    else if ( keyword == "TELESCOP" )
                    {
                        imageInfo.Telescope = ExtractStringValue( strValue );
                    }
                    else if ( keyword == "OBJECT  " )
                    {
                        imageInfo.Object = ExtractStringValue( strValue );
                    }
                    else if ( keyword == "OBSERVER" )
                    {
                        imageInfo.Observer = ExtractStringValue( strValue );
                    }
                    else if ( keyword == "INSTRUME" )
                    {
                        imageInfo.Instrument = ExtractStringValue( strValue );
                    }

                    // --- for debugging ---
                    /* if ( keyword[0] != ' ' )
                    {
                        System.Diagnostics.Debug.Write( keyword );
                        if ( headerRecord[8] == '=' )
                        {
                            System.Diagnostics.Debug.WriteLine( " = " + strValue );
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine( "" );
                        }
                    } */
                    // --- ---
                }
            }

            // scan all available data to find minimum and maximum values,
            // which will be used for scaling. the scan is done here (not while
            // reading actual frame) because FITS file may have set of images
            // packed into data cube, so entire scan of all the data is required.

            // if is stream is seekable
            if ( !stream.CanSeek )
            {
                throw new ArgumentException( "The stream must be seekable." );
            }
            
            // remember current position
            long dataPos = stream.Seek( 0, SeekOrigin.Current );

            // data size
            int lineLength = imageInfo.Width * ( Math.Abs( imageInfo.OriginalBitsPerPixl ) / 8 );
            int totalLines = imageInfo.Height * imageInfo.TotalFrames;
            int originalBitsPerPixel = imageInfo.OriginalBitsPerPixl;

            byte[] buffer = new byte[lineLength];
            byte[] temp = new byte[8];

            // min and max values
            double min = double.MaxValue;
            double max = double.MinValue;

            for ( int i = 0; i < totalLines; i++ )
            {
                // read next line
                if ( Tools.ReadStream( stream, buffer, 0, lineLength ) < lineLength )
                    throw new ArgumentException( "The stream does not contain valid FITS image." );

                // scan the line
                for ( int j = 0; j < lineLength; )
                {
                    double value = 0;

                    // read values accordint to their format
                    switch ( originalBitsPerPixel )
                    {
                        case 8:    // 8 bit unsigned integer
                            value = buffer[j++];
                            break;
                        case 16:    // 16 bit signed integer
                        {
                            short tempValue = 0;
                            unchecked
                            {
                                tempValue = (short) ( ( buffer[j++] << 8 ) | buffer[j++] );
                            }
                            value = tempValue;
                            break;
                        }
                        case 32:    // 32 bit signed integer
                        {
                            temp[3] = buffer[j++];
                            temp[2] = buffer[j++];
                            temp[1] = buffer[j++];
                            temp[0] = buffer[j++];

                            value = BitConverter.ToInt32( temp, 0 );

                            break;
                        }
                        case -32:   // 32 bit float
                        {
                            temp[3] = buffer[j++];
                            temp[2] = buffer[j++];
                            temp[1] = buffer[j++];
                            temp[0] = buffer[j++];

                            value = BitConverter.ToSingle( temp, 0 );
                            break;
                        }
                        case -64:   // 64 bit double
                        {
                            temp[7] = buffer[j++];
                            temp[6] = buffer[j++];
                            temp[5] = buffer[j++];
                            temp[4] = buffer[j++];
                            temp[3] = buffer[j++];
                            temp[2] = buffer[j++];
                            temp[1] = buffer[j++];
                            temp[0] = buffer[j++];

                            value = BitConverter.ToDouble( temp, 0 );
                            break;
                        }
                    }

                    if ( value > max )
                        max = value;
                    if ( value < min )
                        min = value;
                }
            }

            imageInfo.MaxDataValue = max;
            imageInfo.MinDataValue = min;

            // restore stream position to the begining of data
            stream.Seek( dataPos, SeekOrigin.Begin );

            return imageInfo;
        }

        // Read image frame from the specified stream (current stream's position is used)
        private unsafe Bitmap ReadImageFrame( Stream stream, FITSImageInfo imageInfo )
        {
            int width  = imageInfo.Width;
            int height = imageInfo.Height;

            // create new bitmap
            Bitmap image = ( imageInfo.BitsPerPixel == 8 ) ?
                Tools.CreateGrayscaleImage( width, height ) :
                new Bitmap( width, height, PixelFormat.Format16bppGrayScale );

            // lock it
            BitmapData imageData = image.LockBits( new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadWrite, image.PixelFormat );

            int originalBitsPerPixel = imageInfo.OriginalBitsPerPixl;
            int stride = imageData.Stride;
            byte* ptr = (byte*) imageData.Scan0.ToPointer( );

            double min = imageInfo.MinDataValue;
            double max = imageInfo.MaxDataValue;

            // check number of bits per pixel and load image appropriately
            if ( imageInfo.BitsPerPixel == 16 )
            {
                // 16 bpp grayscale image
                double coef = 65535.0 / ( max - min );

                // prepare a buffer for one line
                int lineSize = width * Math.Abs( originalBitsPerPixel ) / 8;
                byte[] line = new byte[lineSize];
                byte[] temp = new byte[8];

                // load all rows
                for ( int y = height - 1; y >= 0; y-- )
                {
                    // load next line
                    if ( Tools.ReadStream( stream, line, 0, lineSize ) < lineSize )
                        throw new ArgumentException( "The stream does not contain valid FITS image." );

                    // fill next image row
                    ushort* row = (ushort*) ( ptr + stride * y );

                    for ( int x = 0, i = 0; x < width; x++, row++ )
                    {
                        double value = 0;

                        switch ( originalBitsPerPixel )
                        {
                            case 16:    // 16 bit signed integer
                            {
                                short tempValue = 0;
                                unchecked
                                {
                                    tempValue = (short) ( ( line[i++] << 8 ) + line[i++] );
                                }
                                value = tempValue;
                                break;
                            }
                            case 32:    // 32 bit signed integer
                            {
                                temp[3] = line[i++];
                                temp[2] = line[i++];
                                temp[1] = line[i++];
                                temp[0] = line[i++];

                                value = BitConverter.ToInt32( temp, 0 );

                                break;
                            }
                            case -32:    // 32 bit float
                            {
                                temp[3] = line[i++];
                                temp[2] = line[i++];
                                temp[1] = line[i++];
                                temp[0] = line[i++];

                                value = BitConverter.ToSingle( temp, 0 );
                                break;
                            }
                            case -64:    // 64 bit double
                            {
                                temp[7] = line[i++];
                                temp[6] = line[i++];
                                temp[5] = line[i++];
                                temp[4] = line[i++];
                                temp[3] = line[i++];
                                temp[2] = line[i++];
                                temp[1] = line[i++];
                                temp[0] = line[i++];

                                value = BitConverter.ToDouble( temp, 0 );
                                break;
                            }
                        }

                        *row = (ushort) ( ( value - min ) * coef );
                    }
                }
            }
            else
            {
                // 8 bpp grayscale image
                double coef = 255.0 / ( max - min );

                // prepare a buffer for one line
                byte[] line = new byte[width];

                // load all rows
                for ( int y = height - 1; y >= 0; y-- )
                {
                    // load next line
                    if ( Tools.ReadStream( stream, line, 0, width ) < width )
                        throw new ArgumentException( "The stream does not contain valid FITS image." );

                    // fill next image row
                    byte* row = ptr + stride * y;

                    for ( int x = 0; x < width; x++, row++ )
                    {
                        *row = (byte) ( ( (double) line[x] - min ) * coef );
                    }
                }
            }

            // unlock image and return it
            image.UnlockBits( imageData );
            return image;
        }

        // Extract integer value from string representation of value/comments
        private int ExtractIntegerValue( string strValue )
        {
            try
            {
                // split value from comment
                string[] strs = strValue.Split( '/' );
                // return value as integer
                return int.Parse( strs[0].Trim( ) );
            }
            catch
            {
                throw new ArgumentException( "The stream does not contain valid FITS image." );
            }
        }

        // Extract string value
        private string ExtractStringValue( string strValue )
        {
            // split value from comment
            string[] strs = strValue.Split( '/' );

            return strs[0].Replace( "''", "``" ).Replace( "'", "" ).Replace( "''", "``" ).Trim( );
        }
    }
}
