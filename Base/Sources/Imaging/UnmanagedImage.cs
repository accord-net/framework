// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2012
// contacts@aforgenet.com
//

namespace AForge.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Collections.Generic;

    /// <summary>
    /// Image in unmanaged memory.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>The class represents wrapper of an image in unmanaged memory. Using this class
    /// it is possible as to allocate new image in unmanaged memory, as to just wrap provided
    /// pointer to unmanaged memory, where an image is stored.</para>
    /// 
    /// <para>Usage of unmanaged images is mostly beneficial when it is required to apply <b>multiple</b>
    /// image processing routines to a single image. In such scenario usage of .NET managed images 
    /// usually leads to worse performance, because each routine needs to lock managed image
    /// before image processing is done and then unlock it after image processing is done. Without
    /// these lock/unlock there is no way to get direct access to managed image's data, which means
    /// there is no way to do fast image processing. So, usage of managed images lead to overhead, which
    /// is caused by locks/unlock. Unmanaged images are represented internally using unmanaged memory
    /// buffer. This means that it is not required to do any locks/unlocks in order to get access to image
    /// data (no overhead).</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // sample 1 - wrapping .NET image into unmanaged without
    /// // making extra copy of image in memory
    /// BitmapData imageData = image.LockBits(
    ///     new Rectangle( 0, 0, image.Width, image.Height ),
    ///     ImageLockMode.ReadWrite, image.PixelFormat );
    /// 
    /// try
    /// {
    ///     UnmanagedImage unmanagedImage = new UnmanagedImage( imageData ) );
    ///     // apply several routines to the unmanaged image
    /// }
    /// finally
    /// {
    ///     image.UnlockBits( imageData );
    /// }
    /// 
    /// 
    /// // sample 2 - converting .NET image into unmanaged
    /// UnmanagedImage unmanagedImage = UnmanagedImage.FromManagedImage( image );
    /// // apply several routines to the unmanaged image
    /// ...
    /// // conver to managed image if it is required to display it at some point of time
    /// Bitmap managedImage = unmanagedImage.ToManagedImage( );
    /// </code>
    /// </remarks>
    /// 
    public class UnmanagedImage : IDisposable
    {
        // pointer to image data in unmanaged memory
        private IntPtr imageData;
        // image size
        private int width, height;
        // image stride (line size)
        private int stride;
        // image pixel format
        private PixelFormat pixelFormat;
        // flag which indicates if the image should be disposed or not
        private bool mustBeDisposed = false;

        /// <summary>
        /// Pointer to image data in unmanaged memory.
        /// </summary>
        public IntPtr ImageData
        {
            get { return imageData; }
        }

        /// <summary>
        /// Image width in pixels.
        /// </summary>
        public int Width
        {
            get { return width; }
        }

        /// <summary>
        /// Image height in pixels.
        /// </summary>
        public int Height
        {
            get { return height; }
        }

        /// <summary>
        /// Image stride (line size in bytes).
        /// </summary>
        public int Stride
        {
            get { return stride; }
        }

        /// <summary>
        /// Image pixel format.
        /// </summary>
        public PixelFormat PixelFormat
        {
            get { return pixelFormat; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnmanagedImage"/> class.
        /// </summary>
        /// 
        /// <param name="imageData">Pointer to image data in unmanaged memory.</param>
        /// <param name="width">Image width in pixels.</param>
        /// <param name="height">Image height in pixels.</param>
        /// <param name="stride">Image stride (line size in bytes).</param>
        /// <param name="pixelFormat">Image pixel format.</param>
        /// 
        /// <remarks><para><note>Using this constructor, make sure all specified image attributes are correct
        /// and correspond to unmanaged memory buffer. If some attributes are specified incorrectly,
        /// this may lead to exceptions working with the unmanaged memory.</note></para></remarks>
        /// 
        public UnmanagedImage( IntPtr imageData, int width, int height, int stride, PixelFormat pixelFormat )
        {
            this.imageData   = imageData;
            this.width       = width;
            this.height      = height;
            this.stride      = stride;
            this.pixelFormat = pixelFormat;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnmanagedImage"/> class.
        /// </summary>
        /// 
        /// <param name="bitmapData">Locked bitmap data.</param>
        /// 
        /// <remarks><note>Unlike <see cref="FromManagedImage(BitmapData)"/> method, this constructor does not make
        /// copy of managed image. This means that managed image must stay locked for the time of using the instance
        /// of unamanged image.</note></remarks>
        /// 
        public UnmanagedImage( BitmapData bitmapData )
        {
            this.imageData   = bitmapData.Scan0;
            this.width       = bitmapData.Width;
            this.height      = bitmapData.Height;
            this.stride      = bitmapData.Stride;
            this.pixelFormat = bitmapData.PixelFormat;
        }

        /// <summary>
        /// Destroys the instance of the <see cref="UnmanagedImage"/> class.
        /// </summary>
        /// 
        ~UnmanagedImage( )
        {
            Dispose( false );
        }

        /// <summary>
        /// Dispose the object.
        /// </summary>
        /// 
        /// <remarks><para>Frees unmanaged resources used by the object. The object becomes unusable
        /// after that.</para>
        /// 
        /// <par><note>The method needs to be called only in the case if unmanaged image was allocated
        /// using <see cref="Create"/> method. In the case if the class instance was created using constructor,
        /// this method does not free unmanaged memory.</note></par>
        /// </remarks>
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
            // free image memory if the image was allocated using this class
            if ( ( mustBeDisposed ) && ( imageData != IntPtr.Zero ) )
            {
                System.Runtime.InteropServices.Marshal.FreeHGlobal( imageData );
                System.GC.RemoveMemoryPressure( stride * height );
                imageData = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Clone the unmanaged images.
        /// </summary>
        /// 
        /// <returns>Returns clone of the unmanaged image.</returns>
        /// 
        /// <remarks><para>The method does complete cloning of the object.</para></remarks>
        /// 
        public UnmanagedImage Clone( )
        {
            // allocate memory for the image
            IntPtr newImageData = System.Runtime.InteropServices.Marshal.AllocHGlobal( stride * height );
            System.GC.AddMemoryPressure( stride * height );

            UnmanagedImage newImage = new UnmanagedImage( newImageData, width, height, stride, pixelFormat );
            newImage.mustBeDisposed = true;

            AForge.SystemTools.CopyUnmanagedMemory( newImageData, imageData, stride * height );

            return newImage;
        }

        /// <summary>
        /// Copy unmanaged image.
        /// </summary>
        /// 
        /// <param name="destImage">Destination image to copy this image to.</param>
        /// 
        /// <remarks><para>The method copies current unmanaged image to the specified image.
        /// Size and pixel format of the destination image must be exactly the same.</para></remarks>
        /// 
        /// <exception cref="InvalidImagePropertiesException">Destination image has different size or pixel format.</exception>
        /// 
        public void Copy( UnmanagedImage destImage )
        {
            if (
                ( width != destImage.width ) || ( height != destImage.height ) ||
                ( pixelFormat != destImage.pixelFormat ) )
            {
                throw new InvalidImagePropertiesException( "Destination image has different size or pixel format." );
            }

            if ( stride == destImage.stride )
            {
                // copy entire image
                AForge.SystemTools.CopyUnmanagedMemory( destImage.imageData, imageData, stride * height );
            }
            else
            {
                unsafe
                {
                    int dstStride = destImage.stride;
                    int copyLength = ( stride < dstStride ) ? stride : dstStride;

                    byte* src = (byte*) imageData.ToPointer( );
                    byte* dst = (byte*) destImage.imageData.ToPointer( );

                    // copy line by line
                    for ( int i = 0; i < height; i++ )
                    {
                        AForge.SystemTools.CopyUnmanagedMemory( dst, src, copyLength );

                        dst += dstStride;
                        src += stride;
                    }
                }
            }
        }

        /// <summary>
        /// Allocate new image in unmanaged memory.
        /// </summary>
        /// 
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// <param name="pixelFormat">Image pixel format.</param>
        /// 
        /// <returns>Return image allocated in unmanaged memory.</returns>
        /// 
        /// <remarks><para>Allocate new image with specified attributes in unmanaged memory.</para>
        /// 
        /// <para><note>The method supports only
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format8bppIndexed</see>,
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format16bppGrayScale</see>,
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format24bppRgb</see>,
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format32bppRgb</see>,
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format32bppArgb</see>,
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format32bppPArgb</see>,
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format48bppRgb</see>,
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format64bppArgb</see> and
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format64bppPArgb</see> pixel formats.
        /// In the case if <see cref="System.Drawing.Imaging.PixelFormat">Format8bppIndexed</see>
        /// format is specified, pallete is not not created for the image (supposed that it is
        /// 8 bpp grayscale image).
        /// </note></para>
        /// </remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format was specified.</exception>
        /// <exception cref="InvalidImagePropertiesException">Invalid image size was specified.</exception>
        /// 
        public static UnmanagedImage Create( int width, int height, PixelFormat pixelFormat )
        {
            int bytesPerPixel = 0 ;

            // calculate bytes per pixel
            switch ( pixelFormat )
            {
                case PixelFormat.Format8bppIndexed:
                    bytesPerPixel = 1;
                    break;
                case PixelFormat.Format16bppGrayScale:
                    bytesPerPixel = 2;
                    break;
                case PixelFormat.Format24bppRgb:
                    bytesPerPixel = 3;
                    break;
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                    bytesPerPixel = 4;
                    break;
                case PixelFormat.Format48bppRgb:
                    bytesPerPixel = 6;
                    break;
                case PixelFormat.Format64bppArgb:
                case PixelFormat.Format64bppPArgb:
                    bytesPerPixel = 8;
                    break;
                default:
                    throw new UnsupportedImageFormatException( "Can not create image with specified pixel format." );
            }

            // check image size
            if ( ( width <= 0 ) || ( height <= 0 ) )
            {
                throw new InvalidImagePropertiesException( "Invalid image size specified." );
            }

            // calculate stride
            int stride = width * bytesPerPixel;

            if ( stride % 4 != 0 )
            {
                stride += ( 4 - ( stride % 4 ) );
            }

            // allocate memory for the image
            IntPtr imageData = System.Runtime.InteropServices.Marshal.AllocHGlobal( stride * height );
            AForge.SystemTools.SetUnmanagedMemory( imageData, 0, stride * height );
            System.GC.AddMemoryPressure( stride * height );

            UnmanagedImage image = new UnmanagedImage( imageData, width, height, stride, pixelFormat );
            image.mustBeDisposed = true;

            return image;
        }

        /// <summary>
        /// Create managed image from the unmanaged.
        /// </summary>
        /// 
        /// <returns>Returns managed copy of the unmanaged image.</returns>
        /// 
        /// <remarks><para>The method creates a managed copy of the unmanaged image with the
        /// same size and pixel format (it calls <see cref="ToManagedImage(bool)"/> specifying
        /// <see langword="true"/> for the <b>makeCopy</b> parameter).</para></remarks>
        /// 
        public Bitmap ToManagedImage( )
        {
            return ToManagedImage( true );
        }

        /// <summary>
        /// Create managed image from the unmanaged.
        /// </summary>
        /// 
        /// <param name="makeCopy">Make a copy of the unmanaged image or not.</param>
        /// 
        /// <returns>Returns managed copy of the unmanaged image.</returns>
        /// 
        /// <remarks><para>If the <paramref name="makeCopy"/> is set to <see langword="true"/>, then the method
        /// creates a managed copy of the unmanaged image, so the managed image stays valid even when the unmanaged
        /// image gets disposed. However, setting this parameter to <see langword="false"/> creates a managed image which is
        /// just a wrapper around the unmanaged image. So if unmanaged image is disposed, the
        /// managed image becomes no longer valid and accessing it will generate an exception.</para></remarks>
        /// 
        /// <exception cref="InvalidImagePropertiesException">The unmanaged image has some invalid properties, which results
        /// in failure of converting it to managed image. This may happen if user used the
        /// <see cref="UnmanagedImage(IntPtr, int, int, int, PixelFormat)"/> constructor specifying some
        /// invalid parameters.</exception>
        /// 
        public Bitmap ToManagedImage( bool makeCopy )
        {
            Bitmap dstImage = null;

            try
            {
                if ( !makeCopy )
                {
                    dstImage = new Bitmap( width, height, stride, pixelFormat, imageData );
                    if ( pixelFormat == PixelFormat.Format8bppIndexed )
                    {
                        Image.SetGrayscalePalette( dstImage );
                    }
                }
                else
                {
                    // create new image of required format
                    dstImage = ( pixelFormat == PixelFormat.Format8bppIndexed ) ?
                        AForge.Imaging.Image.CreateGrayscaleImage( width, height ) :
                        new Bitmap( width, height, pixelFormat );

                    // lock destination bitmap data
                    BitmapData dstData = dstImage.LockBits(
                        new Rectangle( 0, 0, width, height ),
                        ImageLockMode.ReadWrite, pixelFormat );

                    int dstStride = dstData.Stride;
                    int lineSize  = Math.Min( stride, dstStride );

                    unsafe
                    {
                        byte* dst = (byte*) dstData.Scan0.ToPointer( );
                        byte* src = (byte*) imageData.ToPointer( );

                        if ( stride != dstStride )
                        {
                            // copy image
                            for ( int y = 0; y < height; y++ )
                            {
                                AForge.SystemTools.CopyUnmanagedMemory( dst, src, lineSize );
                                dst += dstStride;
                                src += stride;
                            }
                        }
                        else
                        {
                            AForge.SystemTools.CopyUnmanagedMemory( dst, src, stride * height );
                        }
                    }

                    // unlock destination images
                    dstImage.UnlockBits( dstData );
                }

                return dstImage;
            }
            catch ( Exception )
            {
                if ( dstImage != null )
                {
                    dstImage.Dispose( );
                }

                throw new InvalidImagePropertiesException( "The unmanaged image has some invalid properties, which results in failure of converting it to managed image." );
            }
        }

        /// <summary>
        /// Create unmanaged image from the specified managed image.
        /// </summary>
        /// 
        /// <param name="image">Source managed image.</param>
        /// 
        /// <returns>Returns new unmanaged image, which is a copy of source managed image.</returns>
        /// 
        /// <remarks><para>The method creates an exact copy of specified managed image, but allocated
        /// in unmanaged memory.</para></remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of source image.</exception>
        /// 
        public static UnmanagedImage FromManagedImage( Bitmap image )
        {
            UnmanagedImage dstImage = null;

            BitmapData sourceData = image.LockBits( new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, image.PixelFormat );

            try
            {
                dstImage = FromManagedImage( sourceData );
            }
            finally
            {
                image.UnlockBits( sourceData );
            }

            return dstImage;
        }

        /// <summary>
        /// Create unmanaged image from the specified managed image.
        /// </summary>
        /// 
        /// <param name="imageData">Source locked image data.</param>
        /// 
        /// <returns>Returns new unmanaged image, which is a copy of source managed image.</returns>
        /// 
        /// <remarks><para>The method creates an exact copy of specified managed image, but allocated
        /// in unmanaged memory. This means that managed image may be unlocked right after call to this
        /// method.</para></remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of source image.</exception>
        /// 
        public static UnmanagedImage FromManagedImage( BitmapData imageData )
        {
            PixelFormat pixelFormat = imageData.PixelFormat;

            // check source pixel format
            if (
                ( pixelFormat != PixelFormat.Format8bppIndexed ) &&
                ( pixelFormat != PixelFormat.Format16bppGrayScale ) &&
                ( pixelFormat != PixelFormat.Format24bppRgb ) &&
                ( pixelFormat != PixelFormat.Format32bppRgb ) &&
                ( pixelFormat != PixelFormat.Format32bppArgb ) &&
                ( pixelFormat != PixelFormat.Format32bppPArgb ) &&
                ( pixelFormat != PixelFormat.Format48bppRgb ) &&
                ( pixelFormat != PixelFormat.Format64bppArgb ) &&
                ( pixelFormat != PixelFormat.Format64bppPArgb ) )
            {
                throw new UnsupportedImageFormatException( "Unsupported pixel format of the source image." );
            }

            // allocate memory for the image
            IntPtr dstImageData = System.Runtime.InteropServices.Marshal.AllocHGlobal( imageData.Stride * imageData.Height );
            System.GC.AddMemoryPressure( imageData.Stride * imageData.Height );

            UnmanagedImage image = new UnmanagedImage( dstImageData, imageData.Width, imageData.Height, imageData.Stride, pixelFormat );
            AForge.SystemTools.CopyUnmanagedMemory( dstImageData, imageData.Scan0, imageData.Stride * imageData.Height );
            image.mustBeDisposed = true;

            return image;
        }

        /// <summary>
        /// Collect pixel values from the specified list of coordinates.
        /// </summary>
        /// 
        /// <param name="points">List of coordinates to collect pixels' value from.</param>
        /// 
        /// <returns>Returns array of pixels' values from the specified coordinates.</returns>
        /// 
        /// <remarks><para>The method goes through the specified list of points and for each point retrievs
        /// corresponding pixel's value from the unmanaged image.</para>
        /// 
        /// <para><note>For grayscale image the output array has the same length as number of points in the
        /// specified list of points. For color image the output array has triple length, containing pixels'
        /// values in RGB order.</note></para>
        /// 
        /// <para><note>The method does not make any checks for valid coordinates and leaves this up to user.
        /// If specified coordinates are out of image's bounds, the result is not predictable (crash in most cases).
        /// </note></para>
        /// 
        /// <para><note>This method is supposed for images with 8 bpp channels only (8 bpp grayscale image and
        /// 24/32 bpp color images).</note></para>
        /// </remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image. Use Collect16bppPixelValues() method for
        /// images with 16 bpp channels.</exception>
        /// 
        public byte[] Collect8bppPixelValues( List<IntPoint> points )
        {
            int pixelSize = Bitmap.GetPixelFormatSize( pixelFormat ) / 8;

            if ( ( pixelFormat == PixelFormat.Format16bppGrayScale ) || ( pixelSize > 4 ) )
            {
                throw new UnsupportedImageFormatException( "Unsupported pixel format of the source image. Use Collect16bppPixelValues() method for it." );
            }

            byte[] pixelValues = new byte[points.Count * ( ( pixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3 )];

            unsafe
            {
                byte* basePtr = (byte*) imageData.ToPointer( );
                byte* ptr;

                if ( pixelFormat == PixelFormat.Format8bppIndexed )
                {
                    int i = 0;

                    foreach ( IntPoint point in points )
                    {
                        ptr = basePtr + stride * point.Y + point.X;
                        pixelValues[i++] = *ptr;
                    }
                }
                else
                {
                    int i = 0;

                    foreach ( IntPoint point in points )
                    {
                        ptr = basePtr + stride * point.Y + point.X * pixelSize;
                        pixelValues[i++] = ptr[RGB.R];
                        pixelValues[i++] = ptr[RGB.G];
                        pixelValues[i++] = ptr[RGB.B];
                    }
                }
            }

            return pixelValues;
        }

        /// <summary>
        /// Collect coordinates of none black pixels in the image.
        /// </summary>
        /// 
        /// <returns>Returns list of points, which have other than black color.</returns>
        /// 
        public List<IntPoint> CollectActivePixels( )
        {
            return CollectActivePixels( new Rectangle( 0, 0, width, height ) );
        }

        /// <summary>
        /// Collect coordinates of none black pixels within specified rectangle of the image.
        /// </summary>
        /// 
        /// <param name="rect">Image's rectangle to process.</param>
        /// 
        /// <returns>Returns list of points, which have other than black color.</returns>
        ///
        public List<IntPoint> CollectActivePixels( Rectangle rect )
        {
            List<IntPoint> pixels = new List<IntPoint>( );

            int pixelSize = Bitmap.GetPixelFormatSize( pixelFormat ) / 8;

            // correct rectangle
            rect.Intersect( new Rectangle( 0, 0, width, height ) );

            int startX = rect.X;
            int startY = rect.Y;
            int stopX  = rect.Right;
            int stopY  = rect.Bottom;

            unsafe
            {
                byte* basePtr = (byte*) imageData.ToPointer( );

                if ( ( pixelFormat == PixelFormat.Format16bppGrayScale ) || ( pixelSize > 4 ) )
                {
                    int pixelWords = pixelSize >> 1;

                    for ( int y = startY; y < stopY; y++ )
                    {
                        ushort* ptr = (ushort*) ( basePtr + y * stride + startX * pixelSize );

                        if ( pixelWords == 1 )
                        {
                            // grayscale images
                            for ( int x = startX; x < stopX; x++, ptr++ )
                            {
                                if ( *ptr != 0 )
                                {
                                    pixels.Add( new IntPoint( x, y ) );
                                }
                            }
                        }
                        else
                        {
                            // color images
                            for ( int x = startX; x < stopX; x++, ptr += pixelWords )
                            {
                                if ( ( ptr[RGB.R] != 0 ) || ( ptr[RGB.G] != 0 ) || ( ptr[RGB.B] != 0 ) )
                                {
                                    pixels.Add( new IntPoint( x, y ) );
                                }
                            }
                        }
                    }
                }
                else
                {
                    for ( int y = startY; y < stopY; y++ )
                    {
                        byte* ptr = basePtr + y * stride + startX * pixelSize;

                        if ( pixelSize == 1 )
                        {
                            // grayscale images
                            for ( int x = startX; x < stopX; x++, ptr++ )
                            {
                                if ( *ptr != 0 )
                                {
                                    pixels.Add( new IntPoint( x, y ) );
                                }
                            }
                        }
                        else
                        {
                            // color images
                            for ( int x = startX; x < stopX; x++, ptr += pixelSize )
                            {
                                if ( ( ptr[RGB.R] != 0 ) || ( ptr[RGB.G] != 0 ) || ( ptr[RGB.B] != 0 ) )
                                {
                                    pixels.Add( new IntPoint( x, y ) );
                                }
                            }
                        }
                    }
                }
            }

            return pixels;
        }

        /// <summary>
        /// Set pixels with the specified coordinates to the specified color.
        /// </summary>
        /// 
        /// <param name="coordinates">List of points to set color for.</param>
        /// <param name="color">Color to set for the specified points.</param>
        /// 
        /// <remarks><para><note>For images having 16 bpp per color plane, the method extends the specified color
        /// value to 16 bit by multiplying it by 256.</note></para></remarks>
        ///
        public void SetPixels( List<IntPoint> coordinates, Color color )
        {
            unsafe
            {
                int pixelSize = Bitmap.GetPixelFormatSize( pixelFormat ) / 8;
                byte* basePtr = (byte*) imageData.ToPointer( );

                byte red   = color.R;
                byte green = color.G;
                byte blue  = color.B;
                byte alpha = color.A;

                switch ( pixelFormat )
                {
                    case PixelFormat.Format8bppIndexed:
                        {
                            byte grayValue = (byte) ( 0.2125 * red + 0.7154 * green + 0.0721 * blue );

                            foreach ( IntPoint point in coordinates )
                            {
                                if ( ( point.X >= 0 ) && ( point.Y >= 0 ) && ( point.X < width ) && ( point.Y < height ) )
                                {
                                    byte* ptr = basePtr + point.Y * stride + point.X;
                                    *ptr = grayValue;
                                }
                            }
                        }
                        break;

                    case PixelFormat.Format24bppRgb:
                    case PixelFormat.Format32bppRgb:
                        {


                            foreach ( IntPoint point in coordinates )
                            {
                                if ( ( point.X >= 0 ) && ( point.Y >= 0 ) && ( point.X < width ) && ( point.Y < height ) )
                                {
                                    byte* ptr = basePtr + point.Y * stride + point.X * pixelSize;
                                    ptr[RGB.R] = red;
                                    ptr[RGB.G] = green;
                                    ptr[RGB.B] = blue;
                                }
                            }
                        }
                        break;

                    case PixelFormat.Format32bppArgb:
                        {
                            foreach ( IntPoint point in coordinates )
                            {
                                if ( ( point.X >= 0 ) && ( point.Y >= 0 ) && ( point.X < width ) && ( point.Y < height ) )
                                {
                                    byte* ptr = basePtr + point.Y * stride + point.X * pixelSize;
                                    ptr[RGB.R] = red;
                                    ptr[RGB.G] = green;
                                    ptr[RGB.B] = blue;
                                    ptr[RGB.A] = alpha;
                                }
                            }
                        }
                        break;

                    case PixelFormat.Format16bppGrayScale:
                        {
                            ushort grayValue = (ushort) ( (ushort) ( 0.2125 * red + 0.7154 * green + 0.0721 * blue ) << 8 );

                            foreach ( IntPoint point in coordinates )
                            {
                                if ( ( point.X >= 0 ) && ( point.Y >= 0 ) && ( point.X < width ) && ( point.Y < height ) )
                                {
                                    ushort* ptr = (ushort*) ( basePtr + point.Y * stride ) + point.X;
                                    *ptr = grayValue;
                                }
                            }
                        }
                        break;

                    case PixelFormat.Format48bppRgb:
                        {
                            ushort red16   = (ushort) ( red   << 8 );
                            ushort green16 = (ushort) ( green << 8 );
                            ushort blue16  = (ushort) ( blue  << 8 );

                            foreach ( IntPoint point in coordinates )
                            {
                                if ( ( point.X >= 0 ) && ( point.Y >= 0 ) && ( point.X < width ) && ( point.Y < height ) )
                                {
                                    ushort* ptr = (ushort*) ( basePtr + point.Y * stride + point.X * pixelSize );
                                    ptr[RGB.R] = red16;
                                    ptr[RGB.G] = green16;
                                    ptr[RGB.B] = blue16;
                                }
                            }
                        }
                        break;

                    case PixelFormat.Format64bppArgb:
                        {
                            ushort red16   = (ushort) ( red   << 8 );
                            ushort green16 = (ushort) ( green << 8 );
                            ushort blue16  = (ushort) ( blue  << 8 );
                            ushort alpha16 = (ushort) ( alpha << 8 );

                            foreach ( IntPoint point in coordinates )
                            {
                                if ( ( point.X >= 0 ) && ( point.Y >= 0 ) && ( point.X < width ) && ( point.Y < height ) )
                                {
                                    ushort* ptr = (ushort*) ( basePtr + point.Y * stride + point.X * pixelSize );
                                    ptr[RGB.R] = red16;
                                    ptr[RGB.G] = green16;
                                    ptr[RGB.B] = blue16;
                                    ptr[RGB.A] = alpha16;
                                }
                            }
                        }
                        break;

                    default:
                        throw new UnsupportedImageFormatException( "The pixel format is not supported: " + pixelFormat );
                }
            }
        }

        /// <summary>
        /// Set pixel with the specified coordinates to the specified color.
        /// </summary>
        /// 
        /// <param name="point">Point's coordiates to set color for.</param>
        /// <param name="color">Color to set for the pixel.</param>
        /// 
        /// <remarks><para>See <see cref="SetPixel(int, int, Color)"/> for more information.</para></remarks>
        ///
        public void SetPixel( IntPoint point, Color color )
        {
            SetPixel( point.X, point.Y, color );
        }

        /// <summary>
        /// Set pixel with the specified coordinates to the specified color.
        /// </summary>
        /// 
        /// <param name="x">X coordinate of the pixel to set.</param>
        /// <param name="y">Y coordinate of the pixel to set.</param>
        /// <param name="color">Color to set for the pixel.</param>
        /// 
        /// <remarks><para><note>For images having 16 bpp per color plane, the method extends the specified color
        /// value to 16 bit by multiplying it by 256.</note></para>
        /// 
        /// <para>For grayscale images this method will calculate intensity value based on the below formula:
        /// <code lang="none">
        /// 0.2125 * Red + 0.7154 * Green + 0.0721 * Blue
        /// </code>
        /// </para>
        /// </remarks>
        /// 
        public void SetPixel( int x, int y, Color color )
        {
            SetPixel( x, y, color.R, color.G, color.B, color.A );
        }

        /// <summary>
        /// Set pixel with the specified coordinates to the specified value.
        /// </summary>
        ///
        /// <param name="x">X coordinate of the pixel to set.</param>
        /// <param name="y">Y coordinate of the pixel to set.</param>
        /// <param name="value">Pixel value to set.</param>
        /// 
        /// <remarks><para>The method sets all color components of the pixel to the specified value.
        /// If it is a grayscale image, then pixel's intensity is set to the specified value.
        /// If it is a color image, then pixel's R/G/B components are set to the same specified value
        /// (if an image has alpha channel, then it is set to maximum value - 255 or 65535).</para>
        /// 
        /// <para><note>For images having 16 bpp per color plane, the method extends the specified color
        /// value to 16 bit by multiplying it by 256.</note></para>
        /// </remarks>
        /// 
        public void SetPixel( int x, int y, byte value )
        {
            SetPixel( x, y, value, value, value, 255 );
        }

        private void SetPixel( int x, int y, byte r, byte g, byte b, byte a )
        {
            if ( ( x >= 0 ) && ( y >= 0 ) && ( x < width ) && ( y < height ) )
            {
                unsafe
                {
                    int pixelSize = Bitmap.GetPixelFormatSize( pixelFormat ) / 8;
                    byte* ptr = (byte*) imageData.ToPointer( ) + y * stride + x * pixelSize;
                    ushort* ptr2 = (ushort*) ptr;

                    switch ( pixelFormat )
                    {
                        case PixelFormat.Format8bppIndexed:
                            *ptr = (byte) ( 0.2125 * r + 0.7154 * g + 0.0721 * b );
                            break;

                        case PixelFormat.Format24bppRgb:
                        case PixelFormat.Format32bppRgb:
                            ptr[RGB.R] = r;
                            ptr[RGB.G] = g;
                            ptr[RGB.B] = b;
                            break;

                        case PixelFormat.Format32bppArgb:
                            ptr[RGB.R] = r;
                            ptr[RGB.G] = g;
                            ptr[RGB.B] = b;
                            ptr[RGB.A] = a;
                            break;

                        case PixelFormat.Format16bppGrayScale:
                            *ptr2 = (ushort) ( (ushort) ( 0.2125 * r + 0.7154 * g + 0.0721 * b ) << 8 );
                            break;

                        case PixelFormat.Format48bppRgb:
                            ptr2[RGB.R] = (ushort) ( r << 8 );
                            ptr2[RGB.G] = (ushort) ( g << 8 );
                            ptr2[RGB.B] = (ushort) ( b << 8 );
                            break;

                        case PixelFormat.Format64bppArgb:
                            ptr2[RGB.R] = (ushort) ( r << 8 );
                            ptr2[RGB.G] = (ushort) ( g << 8 );
                            ptr2[RGB.B] = (ushort) ( b << 8 );
                            ptr2[RGB.A] = (ushort) ( a << 8 );
                            break;

                        default:
                            throw new UnsupportedImageFormatException( "The pixel format is not supported: " + pixelFormat );
                    }
                }
            }
        }

        /// <summary>
        /// Get color of the pixel with the specified coordinates.
        /// </summary>
        /// 
        /// <param name="point">Point's coordiates to get color of.</param>
        /// 
        /// <returns>Return pixel's color at the specified coordinates.</returns>
        /// 
        /// <remarks><para>See <see cref="GetPixel(int, int)"/> for more information.</para></remarks>
        ///
        public Color GetPixel( IntPoint point )
        {
            return GetPixel( point.X, point.Y );
        }

        /// <summary>
        /// Get color of the pixel with the specified coordinates.
        /// </summary>
        /// 
        /// <param name="x">X coordinate of the pixel to get.</param>
        /// <param name="y">Y coordinate of the pixel to get.</param>
        /// 
        /// <returns>Return pixel's color at the specified coordinates.</returns>
        /// 
        /// <remarks>
        /// <para><note>In the case if the image has 8 bpp grayscale format, the method will return a color with
        /// all R/G/B components set to same value, which is grayscale intensity.</note></para>
        /// 
        /// <para><note>The method supports only 8 bpp grayscale images and 24/32 bpp color images so far.</note></para>
        /// </remarks>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">The specified pixel coordinate is out of image's bounds.</exception>
        /// <exception cref="UnsupportedImageFormatException">Pixel format of this image is not supported by the method.</exception>
        /// 
        public Color GetPixel( int x, int y )
        {
            if ( ( x < 0 ) || ( y < 0 ) )
            {
                throw new ArgumentOutOfRangeException( "x", "The specified pixel coordinate is out of image's bounds." );
            }

            if ( ( x >= width ) || ( y >= height ) )
            {
                throw new ArgumentOutOfRangeException( "y", "The specified pixel coordinate is out of image's bounds." );
            }

            Color color = new Color( );

            unsafe
            {
                int pixelSize = Bitmap.GetPixelFormatSize( pixelFormat ) / 8;
                byte* ptr = (byte*) imageData.ToPointer( ) + y * stride + x * pixelSize;

                switch ( pixelFormat )
                {
                    case PixelFormat.Format8bppIndexed:
                        color = Color.FromArgb( *ptr, *ptr, *ptr );
                        break;

                    case PixelFormat.Format24bppRgb:
                    case PixelFormat.Format32bppRgb:
                        color = Color.FromArgb( ptr[RGB.R], ptr[RGB.G], ptr[RGB.B] );
                        break;

                    case PixelFormat.Format32bppArgb:
                        color = Color.FromArgb( ptr[RGB.A], ptr[RGB.R], ptr[RGB.G], ptr[RGB.B] );
                        break;

                    default:
                        throw new UnsupportedImageFormatException( "The pixel format is not supported: " + pixelFormat );
                }
            }

            return color;
        }

        /// <summary>
        /// Collect pixel values from the specified list of coordinates.
        /// </summary>
        /// 
        /// <param name="points">List of coordinates to collect pixels' value from.</param>
        /// 
        /// <returns>Returns array of pixels' values from the specified coordinates.</returns>
        /// 
        /// <remarks><para>The method goes through the specified list of points and for each point retrievs
        /// corresponding pixel's value from the unmanaged image.</para>
        /// 
        /// <para><note>For grayscale image the output array has the same length as number of points in the
        /// specified list of points. For color image the output array has triple length, containing pixels'
        /// values in RGB order.</note></para>
        /// 
        /// <para><note>The method does not make any checks for valid coordinates and leaves this up to user.
        /// If specified coordinates are out of image's bounds, the result is not predictable (crash in most cases).
        /// </note></para>
        /// 
        /// <para><note>This method is supposed for images with 16 bpp channels only (16 bpp grayscale image and
        /// 48/64 bpp color images).</note></para>
        /// </remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image. Use Collect8bppPixelValues() method for
        /// images with 8 bpp channels.</exception>
        ///
        public ushort[] Collect16bppPixelValues( List<IntPoint> points )
        {
            int pixelSize = Bitmap.GetPixelFormatSize( pixelFormat ) / 8;

            if ( ( pixelFormat == PixelFormat.Format8bppIndexed ) || ( pixelSize == 3 ) || ( pixelSize == 4 ) )
            {
                throw new UnsupportedImageFormatException( "Unsupported pixel format of the source image. Use Collect8bppPixelValues() method for it." );
            }

            ushort[] pixelValues = new ushort[points.Count * ( ( pixelFormat == PixelFormat.Format16bppGrayScale ) ? 1 : 3 )];

            unsafe
            {
                byte* basePtr = (byte*) imageData.ToPointer( );
                ushort* ptr;

                if ( pixelFormat == PixelFormat.Format16bppGrayScale )
                {
                    int i = 0;

                    foreach ( IntPoint point in points )
                    {
                        ptr = (ushort*) ( basePtr + stride * point.Y + point.X * pixelSize );
                        pixelValues[i++] = *ptr;
                    }
                }
                else
                {
                    int i = 0;

                    foreach ( IntPoint point in points )
                    {
                        ptr = (ushort*) ( basePtr + stride * point.Y + point.X * pixelSize );
                        pixelValues[i++] = ptr[RGB.R];
                        pixelValues[i++] = ptr[RGB.G];
                        pixelValues[i++] = ptr[RGB.B];
                    }
                }
            }

            return pixelValues;
        }
    }
}
