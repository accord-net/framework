// AForge Image Formats Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Formats
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    /// <summary>
    /// Image decoder to decode different custom image file formats.
    /// </summary>
    /// 
    /// <remarks><para>The class represent a help class, which simplifies decoding of image
    /// files finding appropriate image decoder automatically (using list of registered
    /// image decoders). Instead of using required image decoder directly, users may use this
    /// class, which will find required decoder by file's extension.</para>
    /// 
    /// <para>By default the class registers on its own all decoders, which are available in
    /// AForge.Imaging.Formats library. If user has implementation of his own image decoders, he
    /// needs to register them using <see cref="RegisterDecoder"/> method to be able to use them through
    /// the <see cref="ImageDecoder"/> class.</para>
    /// 
    /// <para><note>If the class can not find appropriate decode in the list of registered
    /// decoders, it passes file to .NET's image decoder for decoding.</note></para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // sample file name
    /// string fileName = "myFile.pnm";
    /// // decode image file
    /// Bitmap = ImageDecoder.DecodeFromFile( fileName );
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="PNMCodec"/>
    /// <seealso cref="FITSCodec"/>
    /// 
    public class ImageDecoder
    {
        private static Dictionary< string, IImageDecoder >  decoders = new Dictionary<string, IImageDecoder>( );

        static ImageDecoder( )
        {
            // register PNM file format
            IImageDecoder decoder = new PNMCodec( );

            RegisterDecoder( "pbm", decoder );
            RegisterDecoder( "pgm", decoder );
            RegisterDecoder( "pnm", decoder );
            RegisterDecoder( "ppm", decoder );

            // register FITS file format
            decoder = new FITSCodec( );

            RegisterDecoder( "fit",  decoder );
            RegisterDecoder( "fits", decoder );
        }

        /// <summary>
        /// Register image decoder for a specified file extension.
        /// </summary>
        /// 
        /// <param name="fileExtension">File extension to register decoder for ("bmp", for example).</param>
        /// <param name="decoder">Image decoder to use for the specified file extension.</param>
        /// 
        /// <remarks><para>The method allows to register image decoder object, which should be used
        /// to decode images from files with the specified extension.</para></remarks>
        /// 
        public static void RegisterDecoder( string fileExtension, IImageDecoder decoder )
        {
            System.Diagnostics.Debug.WriteLine( "Registering decoder: " + fileExtension );

            decoders.Add( fileExtension.ToLower( ), decoder );
        }

        /// <summary>
        /// Decode first frame for the specified file.
        /// </summary>
        /// 
        /// <param name="fileName">File name to read image from.</param>
        /// 
        /// <returns>Return decoded image. In the case if file format support multiple
        /// frames, the method return the first frame.</returns>
        /// 
        /// <remarks><para>The method uses table of registered image decoders to find the one,
        /// which should be used for the specified file. If there is not appropriate decoder
        /// found, the method uses default .NET's image decoding routine (see
        /// <see cref="System.Drawing.Image.FromFile( string )"/>).</para></remarks>
        /// 
        public static Bitmap DecodeFromFile( string fileName )
        {
            ImageInfo imageInfo = null;

            return DecodeFromFile( fileName, out imageInfo );
        }

        /// <summary>
        /// Decode first frame for the specified file.
        /// </summary>
        /// 
        /// <param name="fileName">File name to read image from.</param>
        /// <param name="imageInfo">Information about the decoded image.</param>
        /// 
        /// <returns>Return decoded image. In the case if file format support multiple
        /// frames, the method return the first frame.</returns>
        /// 
        /// <remarks><para>The method uses table of registered image decoders to find the one,
        /// which should be used for the specified file. If there is not appropriate decoder
        /// found, the method uses default .NET's image decoding routine (see
        /// <see cref="System.Drawing.Image.FromFile( string )"/>).</para></remarks>
        /// 
        public static Bitmap DecodeFromFile( string fileName, out ImageInfo imageInfo )
        {
            Bitmap bitmap = null;

            string fileExtension = Path.GetExtension( fileName ).ToLower( );

            if ( ( fileExtension != string.Empty ) && ( fileExtension.Length != 0 ) )
            {
                fileExtension = fileExtension.Substring( 1 );

                if ( decoders.ContainsKey( fileExtension ) )
                {
                    IImageDecoder decoder = decoders[fileExtension];

                    // open stream
                    FileStream stream = new FileStream( fileName, FileMode.Open );
                    // open decoder
                    decoder.Open( stream );
                    // read the first frame
                    bitmap = decoder.DecodeFrame( 0, out imageInfo );

                    // close decoder and stream
                    decoder.Close( );
                    stream.Close( );
                    stream.Dispose( );

                    return bitmap;
                }
            }

            // use default .NET's image decoding routine
            bitmap = FromFile( fileName );

            imageInfo = new ImageInfo( bitmap.Width, bitmap.Height, Image.GetPixelFormatSize( bitmap.PixelFormat ), 0, 1 );

            return bitmap;
        }

        private static System.Drawing.Bitmap FromFile( string fileName )
        {
            Bitmap loadedImage = null;
            FileStream stream = null;

            try
            {
                // read image to temporary memory stream
                stream = File.OpenRead( fileName );
                MemoryStream memoryStream = new MemoryStream( );

                byte[] buffer = new byte[10000];
                while ( true )
                {
                    int read = stream.Read( buffer, 0, 10000 );

                    if ( read == 0 )
                        break;

                    memoryStream.Write( buffer, 0, read );
                }

                loadedImage = (Bitmap) Bitmap.FromStream( memoryStream );
            }
            finally
            {
                if ( stream != null )
                {
                    stream.Close( );
                    stream.Dispose( );
                }
            }

            return loadedImage;
        }
    }
}
