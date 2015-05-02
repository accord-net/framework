// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace AForge.Imaging
{
    using System;
    using System.IO;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge;

    /// <summary>
    /// Core image relatad methods.
    /// </summary>
    /// 
    /// <remarks>All methods of this class are static and represent general routines
    /// used by different image processing classes.</remarks>
    /// 
    public static class Image
    {
        /// <summary>
        /// Check if specified 8 bpp image is grayscale.
        /// </summary>
        /// 
        /// <param name="image">Image to check.</param>
        /// 
        /// <returns>Returns <b>true</b> if the image is grayscale or <b>false</b> otherwise.</returns>
        /// 
        /// <remarks>The methods checks if the image is a grayscale image of 256 gradients.
        /// The method first examines if the image's pixel format is
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format8bppIndexed</see>
        /// and then it examines its palette to check if the image is grayscale or not.</remarks>
        /// 
        public static bool IsGrayscale( Bitmap image )
        {
            bool ret = false;

            // check pixel format
            if ( image.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                ret = true;
                // check palette
                ColorPalette cp = image.Palette;
                Color c;
                // init palette
                for ( int i = 0; i < 256; i++ )
                {
                    c = cp.Entries[i];
                    if ( ( c.R != i ) || ( c.G != i ) || ( c.B != i ) )
                    {
                        ret = false;
                        break;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Create and initialize new 8 bpp grayscale image.
        /// </summary>
        /// 
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// 
        /// <returns>Returns the created grayscale image.</returns>
        /// 
        /// <remarks>The method creates new 8 bpp grayscale image and initializes its palette.
        /// Grayscale image is represented as
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format8bppIndexed</see>
        /// image with palette initialized to 256 gradients of gray color.</remarks>
        /// 
        public static Bitmap CreateGrayscaleImage( int width, int height )
        {
            // create new image
            Bitmap image = new Bitmap( width, height, PixelFormat.Format8bppIndexed );
            // set palette to grayscale
            SetGrayscalePalette( image );
            // return new image
            return image;
        }

        /// <summary>
        /// Set pallete of the 8 bpp indexed image to grayscale.
        /// </summary>
        /// 
        /// <param name="image">Image to initialize.</param>
        /// 
        /// <remarks>The method initializes palette of
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format8bppIndexed</see>
        /// image with 256 gradients of gray color.</remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Provided image is not 8 bpp indexed image.</exception>
        /// 
        public static void SetGrayscalePalette( Bitmap image )
        {
            // check pixel format
            if ( image.PixelFormat != PixelFormat.Format8bppIndexed )
                throw new UnsupportedImageFormatException( "Source image is not 8 bpp image." );

            // get palette
            ColorPalette cp = image.Palette;
            // init palette
            for ( int i = 0; i < 256; i++ )
            {
                cp.Entries[i] = Color.FromArgb( i, i, i );
            }
            // set palette back
            image.Palette = cp;
        }

        /// <summary>
        /// Clone image.
        /// </summary>
        /// 
        /// <param name="source">Source image.</param>
        /// <param name="format">Pixel format of result image.</param>
        /// 
        /// <returns>Returns clone of the source image with specified pixel format.</returns>
        ///
        /// <remarks>The original <see cref="System.Drawing.Bitmap.Clone(System.Drawing.Rectangle, System.Drawing.Imaging.PixelFormat)">Bitmap.Clone()</see>
        /// does not produce the desired result - it does not create a clone with specified pixel format.
        /// More of it, the original method does not create an actual clone - it does not create a copy
        /// of the image. That is why this method was implemented to provide the functionality.</remarks> 
        ///
        public static Bitmap Clone( Bitmap source, PixelFormat format )
        {
            // copy image if pixel format is the same
            if ( source.PixelFormat == format )
                return Clone( source );

            int width = source.Width;
            int height = source.Height;

            // create new image with desired pixel format
            Bitmap bitmap = new Bitmap( width, height, format );

            // draw source image on the new one using Graphics
            Graphics g = Graphics.FromImage( bitmap );
            g.DrawImage( source, 0, 0, width, height );
            g.Dispose( );

            return bitmap;
        }

        /// <summary>
        /// Clone image.
        /// </summary>
        /// 
        /// <param name="source">Source image.</param>
        /// 
        /// <returns>Return clone of the source image.</returns>
        /// 
        /// <remarks>The original <see cref="System.Drawing.Bitmap.Clone(System.Drawing.Rectangle, System.Drawing.Imaging.PixelFormat)">Bitmap.Clone()</see>
        /// does not produce the desired result - it does not create an actual clone (it does not create a copy
        /// of the image). That is why this method was implemented to provide the functionality.</remarks> 
        /// 
        public static Bitmap Clone( Bitmap source )
        {
            // lock source bitmap data
            BitmapData sourceData = source.LockBits(
                new Rectangle( 0, 0, source.Width, source.Height ),
                ImageLockMode.ReadOnly, source.PixelFormat );

            // create new image
            Bitmap destination = Clone( sourceData );

            // unlock source image
            source.UnlockBits( sourceData );

            //
            if (
                ( source.PixelFormat == PixelFormat.Format1bppIndexed ) ||
                ( source.PixelFormat == PixelFormat.Format4bppIndexed ) ||
                ( source.PixelFormat == PixelFormat.Format8bppIndexed ) ||
                ( source.PixelFormat == PixelFormat.Indexed ) )
            {
                ColorPalette srcPalette = source.Palette;
                ColorPalette dstPalette = destination.Palette;

                int n = srcPalette.Entries.Length;

                // copy pallete
                for ( int i = 0; i < n; i++ )
                {
                    dstPalette.Entries[i] = srcPalette.Entries[i];
                }

                destination.Palette = dstPalette;
            }

            return destination;
        }

        /// <summary>
        /// Clone image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        ///
        /// <returns>Clones image from source image data. The message does not clone pallete in the
        /// case if the source image has indexed pixel format.</returns>
        /// 
        public static Bitmap Clone( BitmapData sourceData )
        {
            // get source image size
            int width = sourceData.Width;
            int height = sourceData.Height;

            // create new image
            Bitmap destination = new Bitmap( width, height, sourceData.PixelFormat );

            // lock destination bitmap data
            BitmapData destinationData = destination.LockBits(
                new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadWrite, destination.PixelFormat );

            AForge.SystemTools.CopyUnmanagedMemory( destinationData.Scan0, sourceData.Scan0, height * sourceData.Stride );

            // unlock destination image
            destination.UnlockBits( destinationData );

            return destination;
        }

        /// <summary>
        /// Format an image.
        /// </summary>
        /// 
        /// <param name="image">Source image to format.</param>
        /// 
        /// <remarks><para>Formats the image to one of the formats, which are supported
        /// by the <b>AForge.Imaging</b> library. The image is left untouched in the
        /// case if it is already of
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format24bppRgb</see> or
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format32bppRgb</see> or
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format32bppArgb</see> or
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format48bppRgb</see> or
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format64bppArgb</see>
        /// format or it is <see cref="IsGrayscale">grayscale</see>, otherwise the image
        /// is converted to <see cref="System.Drawing.Imaging.PixelFormat">Format24bppRgb</see>
        /// format.</para>
        /// 
        /// <para><note>The method is deprecated and <see cref="Clone(Bitmap, PixelFormat)"/> method should
        /// be used instead with specifying desired pixel format.</note></para>
        /// </remarks>
        ///
        [Obsolete( "Use Clone(Bitmap, PixelFormat) method instead and specify desired pixel format" )]
        public static void FormatImage( ref Bitmap image )
        {
            if (
                ( image.PixelFormat != PixelFormat.Format24bppRgb ) &&
                ( image.PixelFormat != PixelFormat.Format32bppRgb ) &&
                ( image.PixelFormat != PixelFormat.Format32bppArgb ) &&
                ( image.PixelFormat != PixelFormat.Format48bppRgb ) &&
                ( image.PixelFormat != PixelFormat.Format64bppArgb ) &&
                ( image.PixelFormat != PixelFormat.Format16bppGrayScale ) &&
                ( IsGrayscale( image ) == false )
                )
            {
                Bitmap tmp = image;
                // convert to 24 bits per pixel
                image = Clone( tmp, PixelFormat.Format24bppRgb );
                // delete old image
                tmp.Dispose( );
            }
        }

        /// <summary>
        /// Load bitmap from file.
        /// </summary>
        /// 
        /// <param name="fileName">File name to load bitmap from.</param>
        /// 
        /// <returns>Returns loaded bitmap.</returns>
        /// 
        /// <remarks><para>The method is provided as an alternative of <see cref="System.Drawing.Image.FromFile(string)"/>
        /// method to solve the issues of locked file. The standard .NET's method locks the source file until
        /// image's object is disposed, so the file can not be deleted or overwritten. This method workarounds the issue and
        /// does not lock the source file.</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// Bitmap image = AForge.Imaging.Image.FromFile( "test.jpg" );
        /// </code>
        /// </remarks>
        /// 
        public static System.Drawing.Bitmap FromFile( string fileName )
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

        /// <summary>
        /// Convert bitmap with 16 bits per plane to a bitmap with 8 bits per plane.
        /// </summary>
        /// 
        /// <param name="bimap">Source image to convert.</param>
        /// 
        /// <returns>Returns new image which is a copy of the source image but with 8 bits per plane.</returns>
        /// 
        /// <remarks><para>The routine does the next pixel format conversions:
        /// <list type="bullet">
        /// <item><see cref="PixelFormat.Format16bppGrayScale">Format16bppGrayScale</see> to
        /// <see cref="PixelFormat.Format8bppIndexed">Format8bppIndexed</see> with grayscale palette;</item>
        /// <item><see cref="PixelFormat.Format48bppRgb">Format48bppRgb</see> to
        /// <see cref="PixelFormat.Format24bppRgb">Format24bppRgb</see>;</item>
        /// <item><see cref="PixelFormat.Format64bppArgb">Format64bppArgb</see> to
        /// <see cref="PixelFormat.Format32bppArgb">Format32bppArgb</see>;</item>
        /// <item><see cref="PixelFormat.Format64bppPArgb">Format64bppPArgb</see> to
        /// <see cref="PixelFormat.Format32bppPArgb">Format32bppPArgb</see>.</item>
        /// </list>
        /// </para></remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Invalid pixel format of the source image.</exception>
        /// 
        public static Bitmap Convert16bppTo8bpp( Bitmap bimap )
        {
            Bitmap newImage = null;
            int layers = 0;

            // get image size
            int width  = bimap.Width;
            int height = bimap.Height;

            // create new image depending on source image format
            switch ( bimap.PixelFormat )
            {
                case PixelFormat.Format16bppGrayScale:
                    // create new grayscale image
                    newImage = CreateGrayscaleImage( width, height );
                    layers = 1;
                    break;

                case PixelFormat.Format48bppRgb:
                    // create new color 24 bpp image
                    newImage = new Bitmap( width, height, PixelFormat.Format24bppRgb );
                    layers = 3;
                    break;

                case PixelFormat.Format64bppArgb:
                    // create new color 32 bpp image
                    newImage = new Bitmap( width, height, PixelFormat.Format32bppArgb );
                    layers = 4;
                    break;

                case PixelFormat.Format64bppPArgb:
                    // create new color 32 bpp image
                    newImage = new Bitmap( width, height, PixelFormat.Format32bppPArgb );
                    layers = 4;
                    break;

                default:
                    throw new UnsupportedImageFormatException( "Invalid pixel format of the source image." );
            }

            // lock both images
            BitmapData sourceData = bimap.LockBits( new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadOnly, bimap.PixelFormat );
            BitmapData newData = newImage.LockBits( new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadWrite, newImage.PixelFormat );

            unsafe
            {
                // base pointers
                byte* sourceBasePtr = (byte*) sourceData.Scan0.ToPointer( );
                byte* newBasePtr    = (byte*) newData.Scan0.ToPointer( );
                // image strides
                int sourceStride = sourceData.Stride;
                int newStride = newData.Stride;

                for ( int y = 0; y < height; y++ )
                {
                    ushort* sourcePtr = (ushort*) ( sourceBasePtr + y * sourceStride );
                    byte* newPtr = (byte*) ( newBasePtr + y * newStride );

                    for ( int x = 0, lineSize = width * layers; x < lineSize; x++, sourcePtr++, newPtr++ )
                    {
                        *newPtr = (byte) ( *sourcePtr >> 8 );
                    }
                }
            }

            // unlock both image
            bimap.UnlockBits( sourceData );
            newImage.UnlockBits( newData );

            return newImage;
        }

        /// <summary>
        /// Convert bitmap with 8 bits per plane to a bitmap with 16 bits per plane.
        /// </summary>
        /// 
        /// <param name="bimap">Source image to convert.</param>
        /// 
        /// <returns>Returns new image which is a copy of the source image but with 16 bits per plane.</returns>
        /// 
        /// <remarks><para>The routine does the next pixel format conversions:
        /// <list type="bullet">
        /// <item><see cref="PixelFormat.Format8bppIndexed">Format8bppIndexed</see> (grayscale palette assumed) to
        /// <see cref="PixelFormat.Format16bppGrayScale">Format16bppGrayScale</see>;</item>
        /// <item><see cref="PixelFormat.Format24bppRgb">Format24bppRgb</see> to
        /// <see cref="PixelFormat.Format48bppRgb">Format48bppRgb</see>;</item>
        /// <item><see cref="PixelFormat.Format32bppArgb">Format32bppArgb</see> to
        /// <see cref="PixelFormat.Format64bppArgb">Format64bppArgb</see>;</item>
        /// <item><see cref="PixelFormat.Format32bppPArgb">Format32bppPArgb</see> to
        /// <see cref="PixelFormat.Format64bppPArgb">Format64bppPArgb</see>.</item>
        /// </list>
        /// </para></remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Invalid pixel format of the source image.</exception>
        /// 
        public static Bitmap Convert8bppTo16bpp( Bitmap bimap )
        {
            Bitmap newImage = null;
            int layers = 0;

            // get image size
            int width  = bimap.Width;
            int height = bimap.Height;

            // create new image depending on source image format
            switch ( bimap.PixelFormat )
            {
                case PixelFormat.Format8bppIndexed:
                    // create new grayscale image
                    newImage = new Bitmap( width, height, PixelFormat.Format16bppGrayScale );
                    layers = 1;
                    break;

                case PixelFormat.Format24bppRgb:
                    // create new color 48 bpp image
                    newImage = new Bitmap( width, height, PixelFormat.Format48bppRgb );
                    layers = 3;
                    break;

                case PixelFormat.Format32bppArgb:
                    // create new color 64 bpp image
                    newImage = new Bitmap( width, height, PixelFormat.Format64bppArgb );
                    layers = 4;
                    break;

                case PixelFormat.Format32bppPArgb:
                    // create new color 64 bpp image
                    newImage = new Bitmap( width, height, PixelFormat.Format64bppPArgb );
                    layers = 4;
                    break;

                default:
                    throw new UnsupportedImageFormatException( "Invalid pixel format of the source image." );
            }

            // lock both images
            BitmapData sourceData = bimap.LockBits( new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadOnly, bimap.PixelFormat );
            BitmapData newData = newImage.LockBits( new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadWrite, newImage.PixelFormat );

            unsafe
            {
                // base pointers
                byte* sourceBasePtr = (byte*) sourceData.Scan0.ToPointer( );
                byte* newBasePtr    = (byte*) newData.Scan0.ToPointer( );
                // image strides
                int sourceStride = sourceData.Stride;
                int newStride = newData.Stride;

                for ( int y = 0; y < height; y++ )
                {
                    byte* sourcePtr = (byte*) ( sourceBasePtr + y * sourceStride );
                    ushort* newPtr  = (ushort*) ( newBasePtr + y * newStride );

                    for ( int x = 0, lineSize = width * layers; x < lineSize; x++, sourcePtr++, newPtr++ )
                    {
                        *newPtr = (ushort) ( *sourcePtr << 8 );
                    }
                }
            }

            // unlock both image
            bimap.UnlockBits( sourceData );
            newImage.UnlockBits( newData );

            return newImage;
        }
    }
}
