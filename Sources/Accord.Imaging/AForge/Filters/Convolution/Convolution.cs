// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace AForge.Imaging.Filters
{
	using System;
    using System.Collections.Generic;
    using System.Drawing;
	using System.Drawing.Imaging;

	/// <summary>
	/// Convolution filter.
	/// </summary>
	/// 
    /// <remarks><para>The filter implements convolution operator, which calculates each pixel
    /// of the result image as weighted sum of the correspond pixel and its neighbors in the source
    /// image. The weights are set by <see cref="Kernel">convolution kernel</see>. The weighted
    /// sum is divided by <see cref="Divisor"/> before putting it into result image and also
    /// may be thresholded using <see cref="Threshold"/> value.</para>
    /// 
    /// <para>Convolution is a simple mathematical operation which is fundamental to many common
    /// image processing filters. Depending on the type of provided kernel, the filter may produce
    /// different results, like blur image, sharpen it, find edges, etc.</para>
    /// 
    /// <para>The filter accepts 8 and 16 bpp grayscale images and 24, 32, 48 and 64 bpp
    /// color images for processing. Note: depending on the value of <see cref="ProcessAlpha"/>
    /// property, the alpha channel is either copied as is or processed with the kernel.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // define emboss kernel
    /// int[,] kernel = {
    ///             { -2, -1,  0 },
    ///             { -1,  1,  1 },
    ///             {  0,  1,  2 } };
    /// // create filter
    /// Convolution filter = new Convolution( kernel );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample5.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/emboss.jpg" width="480" height="361" />
    /// </remarks>
	/// 
    public class Convolution : BaseUsingCopyPartialFilter
	{
        // convolution kernel
        private int[,] kernel;
        // division factor
        private int divisor = 1;
        // threshold to add to weighted sum
        private int threshold = 0;
        // kernel size
        private int size;
        // use dynamic divisor for edges
        private bool dynamicDivisorForEdges = true;
        // specifies if alpha channel must be processed or just copied
        private bool processAlpha = false;

        // private format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        /// Convolution kernel.
        /// </summary>
        /// 
        /// <remarks>
        /// <para><note>Convolution kernel must be square and its width/height
        /// should be odd and should be in the [3, 99] range.</note></para>
        /// 
        /// <para><note>Setting convolution kernel through this property does not
        /// affect <see cref="Divisor"/> - it is not recalculated automatically.</note></para>
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">Invalid kernel size is specified.</exception>
        /// 
        public int[,] Kernel
        {
            get { return kernel; }
            set
            {
                int s = value.GetLength( 0 );

                // check kernel size
                if ( ( s != value.GetLength( 1 ) ) || ( s < 3 ) || ( s > 99 ) || ( s % 2 == 0 ) )
                    throw new ArgumentException( "Invalid kernel size." );

                this.kernel = value;
                this.size = s;
            }
        }

        /// <summary>
        /// Division factor.
        /// </summary>
        /// 
        /// <remarks><para>The value is used to divide convolution - weighted sum
        /// of pixels is divided by this value.</para>
        /// 
        /// <para><note>The value may be calculated automatically in the case if constructor
        /// with one parameter is used (<see cref="Convolution( int[,] )"/>).</note></para>
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">Divisor can not be equal to zero.</exception>
        /// 
        public int Divisor
        {
            get { return divisor; }
            set
            {
                if ( value == 0 )
                    throw new ArgumentException( "Divisor can not be equal to zero." );
                divisor = value;
            }
        }

        /// <summary>
        /// Threshold to add to weighted sum.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies threshold value, which is added to each weighted
        /// sum of pixels. The value is added right after division was done by <see cref="Divisor"/>
        /// value.</para>
        /// 
        /// <para>Default value is set to <b>0</b>.</para>
        /// </remarks>
        /// 
        public int Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }

        /// <summary>
        /// Use dynamic divisor for edges or not.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies how to handle edges. If it is set to
        /// <see langword="false"/>, then the same divisor (which is specified by <see cref="Divisor"/>
        /// property or calculated automatically) will be applied both for non-edge regions
        /// and for edge regions. If the value is set to <see langword="true"/>, then dynamically
        /// calculated divisor will be used for edge regions, which is sum of those kernel
        /// elements, which are taken into account for particular processed pixel
        /// (elements, which are not outside image).</para>
        /// 
        /// <para>Default value is set to <see langword="true"/>.</para>
        /// </remarks>
        /// 
        public bool DynamicDivisorForEdges
        {
            get { return dynamicDivisorForEdges; }
            set { dynamicDivisorForEdges = value; }
        }

        /// <summary>
        /// Specifies if alpha channel must be processed or just copied.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies the way how alpha channel is handled for 32 bpp
        /// and 64 bpp images. If the property is set to <see langword="false"/>, then alpha
        /// channel's values are just copied as is. If the property is set to <see langword="true"/>
        /// then alpha channel is convolved using the specified kernel same way as RGB channels.</para>
        /// 
        /// <para>Default value is set to <see langword="false"/>.</para>
        /// </remarks>
        /// 
        public bool ProcessAlpha
        {
            get { return processAlpha; }
            set { processAlpha = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Convolution"/> class.
        /// </summary>
        protected Convolution( )
        {
            formatTranslations[PixelFormat.Format8bppIndexed]    = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            formatTranslations[PixelFormat.Format24bppRgb]       = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]       = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb]      = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format48bppRgb]       = PixelFormat.Format48bppRgb;
            formatTranslations[PixelFormat.Format64bppArgb]      = PixelFormat.Format64bppArgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Convolution"/> class.
        /// </summary>
        /// 
        /// <param name="kernel">Convolution kernel.</param>
        /// 
        /// <remarks><para>Using this constructor (specifying only convolution kernel),
        /// <see cref="Divisor">division factor</see> will be calculated automatically
        /// summing all kernel values. In the case if kernel's sum equals to zero,
        /// division factor will be assigned to 1.</para></remarks>
        /// 
        /// <exception cref="ArgumentException">Invalid kernel size is specified. Kernel must be
        /// square, its width/height should be odd and should be in the [3, 25] range.</exception>
        /// 
        public Convolution( int[,] kernel ) : this( )
        {
            Kernel = kernel;

            divisor = 0;

            // calculate divisor
            for ( int i = 0, n = kernel.GetLength( 0 ); i < n; i++ )
            {
                for ( int j = 0, k = kernel.GetLength( 1 ); j < k; j++ )
                {
                    divisor += kernel[i, j];
                }
            }
            if ( divisor == 0 )
                divisor = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Convolution"/> class.
        /// </summary>
        /// 
        /// <param name="kernel">Convolution kernel.</param>
        /// <param name="divisor">Divisor, used used to divide weighted sum.</param>
        /// 
        /// <exception cref="ArgumentException">Invalid kernel size is specified. Kernel must be
        /// square, its width/height should be odd and should be in the [3, 25] range.</exception>
        /// <exception cref="ArgumentException">Divisor can not be equal to zero.</exception>
        /// 
        public Convolution( int[,] kernel, int divisor ) : this( )
        {
            Kernel = kernel;
            Divisor = divisor;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="source">Source image data.</param>
        /// <param name="destination">Destination image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        protected override unsafe void ProcessFilter( UnmanagedImage source, UnmanagedImage destination, Rectangle rect )
        {
            int pixelSize = Image.GetPixelFormatSize( source.PixelFormat ) / 8;

            // processing start and stop X,Y positions
            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;

            // check pixel size to find if we deal with 8 or 16 bpp channels
            if ( ( pixelSize <= 4 ) && ( pixelSize != 2 ) )
            {
                int srcStride = source.Stride;
                int dstStride = destination.Stride;

                int srcOffset = srcStride - rect.Width * pixelSize;
                int dstOffset = dstStride - rect.Width * pixelSize;

                byte* src = (byte*) source.ImageData.ToPointer( );
                byte* dst = (byte*) destination.ImageData.ToPointer( );

                // allign pointers to the first pixel to process
                src += ( startY * srcStride + startX * pixelSize );
                dst += ( startY * dstStride + startX * pixelSize );

                // do the processing job
                if ( destination.PixelFormat == PixelFormat.Format8bppIndexed )
                {
                    // grayscale image
                    Process8bppImage( src, dst, srcStride, dstStride, srcOffset, dstOffset, startX, startY, stopX, stopY );
                }
                else
                {
                    // RGB image
                    if ( ( pixelSize == 3 ) || ( !processAlpha ) )
                    {
                        Process24bppImage( src, dst, srcStride, dstStride, srcOffset, dstOffset, startX, startY, stopX, stopY, pixelSize );
                    }
                    else
                    {
                        Process32bppImage( src, dst, srcStride, dstStride, srcOffset, dstOffset, startX, startY, stopX, stopY );
                    }
                }
            }
            else
            {
                pixelSize /= 2;

                int dstStride = destination.Stride / 2;
                int srcStride = source.Stride / 2;

                // base pointers
                ushort* baseSrc = (ushort*) source.ImageData.ToPointer( );
                ushort* baseDst = (ushort*) destination.ImageData.ToPointer( );

                // allign pointers by X
                baseSrc += ( startX * pixelSize );
                baseDst += ( startX * pixelSize );

                if ( source.PixelFormat == PixelFormat.Format16bppGrayScale )
                {
                    // 16 bpp grayscale image
                    Process16bppImage( baseSrc, baseDst, srcStride, dstStride, startX, startY, stopX, stopY );
                }
                else
                {
                    // RGB image
                    if ( ( pixelSize == 3 ) || ( !processAlpha ) )
                    {
                        Process48bppImage( baseSrc, baseDst, srcStride, dstStride, startX, startY, stopX, stopY, pixelSize );
                    }
                    else
                    {
                        Process64bppImage( baseSrc, baseDst, srcStride, dstStride, startX, startY, stopX, stopY );
                    }
                }
            }
        }

        // Process 8 bpp grayscale images
        private unsafe void Process8bppImage( byte* src, byte* dst,
                                              int srcStride, int dstStride, int srcOffset, int dstOffset,
                                              int startX, int startY, int stopX, int stopY )
        {
            // loop and array indexes
            int i, j, t, k, ir, jr;
            // kernel's radius
            int radius = size >> 1;
            // color sums
            long g, div;

            // kernel size
            int kernelSize = size * size;
            // number of kernel elements taken into account
            int processedKernelSize;

            // for each line
            for ( int y = startY; y < stopY; y++ )
            {
                // for each pixel
                for ( int x = startX; x < stopX; x++, src++, dst++ )
                {
                    g = div = processedKernelSize = 0;

                    // for each kernel row
                    for ( i = 0; i < size; i++ )
                    {
                        ir = i - radius;
                        t = y + ir;

                        // skip row
                        if ( t < startY )
                            continue;
                        // break
                        if ( t >= stopY )
                            break;

                        // for each kernel column
                        for ( j = 0; j < size; j++ )
                        {
                            jr = j - radius;
                            t = x + jr;

                            // skip column
                            if ( t < startX )
                                continue;

                            if ( t < stopX )
                            {
                                k = kernel[i, j];

                                div += k;
                                g += k * src[ir * srcStride + jr];
                                processedKernelSize++;
                            }
                        }
                    }

                    // check if all kernel elements were processed
                    if ( processedKernelSize == kernelSize )
                    {
                        // all kernel elements are processed - we are not on the edge
                        div = divisor;
                    }
                    else
                    {
                        // we are on edge. do we need to use dynamic divisor or not?
                        if ( !dynamicDivisorForEdges )
                        {
                            // do
                            div = divisor;
                        }
                    }

                    // check divider
                    if ( div != 0 )
                    {
                        g /= div;
                    }
                    g += threshold;
                    *dst = (byte) ( ( g > 255 ) ? 255 : ( ( g < 0 ) ? 0 : g ) );
                }
                src += srcOffset;
                dst += dstOffset;
            }
        }

        // Process 24 bpp images or 32 bpp images with copying alpha channel
        private unsafe void Process24bppImage( byte* src, byte* dst,
                                               int srcStride, int dstStride, int srcOffset, int dstOffset,
                                               int startX, int startY, int stopX, int stopY, int pixelSize )
        {
            // loop and array indexes
            int i, j, t, k, ir, jr;
            // kernel's radius
            int radius = size >> 1;
            // color sums
            long r, g, b, div;

            // kernel size
            int kernelSize = size * size;
            // number of kernel elements taken into account
            int processedKernelSize;

            byte* p;

            // for each line
            for ( int y = startY; y < stopY; y++ )
            {
                // for each pixel
                for ( int x = startX; x < stopX; x++, src += pixelSize, dst += pixelSize )
                {
                    r = g = b = div = processedKernelSize = 0;

                    // for each kernel row
                    for ( i = 0; i < size; i++ )
                    {
                        ir = i - radius;
                        t = y + ir;

                        // skip row
                        if ( t < startY )
                            continue;
                        // break
                        if ( t >= stopY )
                            break;

                        // for each kernel column
                        for ( j = 0; j < size; j++ )
                        {
                            jr = j - radius;
                            t = x + jr;

                            // skip column
                            if ( t < startX )
                                continue;

                            if ( t < stopX )
                            {
                                k = kernel[i, j];
                                p = &src[ir * srcStride + jr * pixelSize];

                                div += k;

                                r += k * p[RGB.R];
                                g += k * p[RGB.G];
                                b += k * p[RGB.B];

                                processedKernelSize++;
                            }
                        }
                    }

                    // check if all kernel elements were processed
                    if ( processedKernelSize == kernelSize )
                    {
                        // all kernel elements are processed - we are not on the edge
                        div = divisor;
                    }
                    else
                    {
                        // we are on edge. do we need to use dynamic divisor or not?
                        if ( !dynamicDivisorForEdges )
                        {
                            // do
                            div = divisor;
                        }
                    }

                    // check divider
                    if ( div != 0 )
                    {
                        r /= div;
                        g /= div;
                        b /= div;
                    }
                    r += threshold;
                    g += threshold;
                    b += threshold;

                    dst[RGB.R] = (byte) ( ( r > 255 ) ? 255 : ( ( r < 0 ) ? 0 : r ) );
                    dst[RGB.G] = (byte) ( ( g > 255 ) ? 255 : ( ( g < 0 ) ? 0 : g ) );
                    dst[RGB.B] = (byte) ( ( b > 255 ) ? 255 : ( ( b < 0 ) ? 0 : b ) );

                    // take care of alpha channel
                    if ( pixelSize == 4 )
                        dst[RGB.A] = src[RGB.A];
                }
                src += srcOffset;
                dst += dstOffset;
            }
        }

        // Process 32 bpp images including alpha channel
        private unsafe void Process32bppImage( byte* src, byte* dst,
                                               int srcStride, int dstStride, int srcOffset, int dstOffset,
                                               int startX, int startY, int stopX, int stopY )
        {
            // loop and array indexes
            int i, j, t, k, ir, jr;
            // kernel's radius
            int radius = size >> 1;
            // color sums
            long r, g, b, a, div;

            // kernel size
            int kernelSize = size * size;
            // number of kernel elements taken into account
            int processedKernelSize;

            byte* p;

            // for each line
            for ( int y = startY; y < stopY; y++ )
            {
                // for each pixel
                for ( int x = startX; x < stopX; x++, src += 4, dst += 4 )
                {
                    r = g = b = a = div = processedKernelSize = 0;

                    // for each kernel row
                    for ( i = 0; i < size; i++ )
                    {
                        ir = i - radius;
                        t = y + ir;

                        // skip row
                        if ( t < startY )
                            continue;
                        // break
                        if ( t >= stopY )
                            break;

                        // for each kernel column
                        for ( j = 0; j < size; j++ )
                        {
                            jr = j - radius;
                            t = x + jr;

                            // skip column
                            if ( t < startX )
                                continue;

                            if ( t < stopX )
                            {
                                k = kernel[i, j];
                                p = &src[ir * srcStride + jr * 4];

                                div += k;

                                r += k * p[RGB.R];
                                g += k * p[RGB.G];
                                b += k * p[RGB.B];
                                a += k * p[RGB.A];

                                processedKernelSize++;
                            }
                        }
                    }

                    // check if all kernel elements were processed
                    if ( processedKernelSize == kernelSize )
                    {
                        // all kernel elements are processed - we are not on the edge
                        div = divisor;
                    }
                    else
                    {
                        // we are on edge. do we need to use dynamic divisor or not?
                        if ( !dynamicDivisorForEdges )
                        {
                            // do
                            div = divisor;
                        }
                    }

                    // check divider
                    if ( div != 0 )
                    {
                        r /= div;
                        g /= div;
                        b /= div;
                        a /= div;
                    }
                    r += threshold;
                    g += threshold;
                    b += threshold;
                    a += threshold;

                    dst[RGB.R] = (byte) ( ( r > 255 ) ? 255 : ( ( r < 0 ) ? 0 : r ) );
                    dst[RGB.G] = (byte) ( ( g > 255 ) ? 255 : ( ( g < 0 ) ? 0 : g ) );
                    dst[RGB.B] = (byte) ( ( b > 255 ) ? 255 : ( ( b < 0 ) ? 0 : b ) );
                    dst[RGB.A] = (byte) ( ( a > 255 ) ? 255 : ( ( a < 0 ) ? 0 : a ) );
                }
                src += srcOffset;
                dst += dstOffset;
            }
        }

        // Process 16 bpp grayscale images
        private unsafe void Process16bppImage( ushort* baseSrc, ushort* baseDst, int srcStride, int dstStride,
                                               int startX, int startY, int stopX, int stopY )
        {
            // loop and array indexes
            int i, j, t, k, ir, jr;
            // kernel's radius
            int radius = size >> 1;
            // color sums
            long g, div;

            // kernel size
            int kernelSize = size * size;
            // number of kernel elements taken into account
            int processedKernelSize;

            // for each line
            for ( int y = startY; y < stopY; y++ )
            {
                ushort* src = baseSrc + y * srcStride;
                ushort* dst = baseDst + y * dstStride;

                // for each pixel
                for ( int x = startX; x < stopX; x++, src++, dst++ )
                {
                    g = div = processedKernelSize = 0;

                    // for each kernel row
                    for ( i = 0; i < size; i++ )
                    {
                        ir = i - radius;
                        t = y + ir;

                        // skip row
                        if ( t < startY )
                            continue;
                        // break
                        if ( t >= stopY )
                            break;

                        // for each kernel column
                        for ( j = 0; j < size; j++ )
                        {
                            jr = j - radius;
                            t = x + jr;

                            // skip column
                            if ( t < startX )
                                continue;

                            if ( t < stopX )
                            {
                                k = kernel[i, j];

                                div += k;
                                g += k * src[ir * srcStride + jr];
                                processedKernelSize++;
                            }
                        }
                    }

                    // check if all kernel elements were processed
                    if ( processedKernelSize == kernelSize )
                    {
                        // all kernel elements are processed - we are not on the edge
                        div = divisor;
                    }
                    else
                    {
                        // we are on edge. do we need to use dynamic divisor or not?
                        if ( !dynamicDivisorForEdges )
                        {
                            // do
                            div = divisor;
                        }
                    }

                    // check divider
                    if ( div != 0 )
                    {
                        g /= div;
                    }
                    g += threshold;
                    *dst = (ushort) ( ( g > 65535 ) ? 65535 : ( ( g < 0 ) ? 0 : g ) );
                }
            }
        }

        // Process 48 bpp images or 64 bpp images with copying alpha channel
        private unsafe void Process48bppImage( ushort* baseSrc, ushort* baseDst, int srcStride, int dstStride,
                                               int startX, int startY, int stopX, int stopY, int pixelSize )
        {
            // loop and array indexes
            int i, j, t, k, ir, jr;
            // kernel's radius
            int radius = size >> 1;
            // color sums
            long r, g, b, div;

            // kernel size
            int kernelSize = size * size;
            // number of kernel elements taken into account
            int processedKernelSize;

            ushort* p;

            // for each line
            for ( int y = startY; y < stopY; y++ )
            {
                ushort* src = baseSrc + y * srcStride;
                ushort* dst = baseDst + y * dstStride;

                // for each pixel
                for ( int x = startX; x < stopX; x++, src += pixelSize, dst += pixelSize )
                {
                    r = g = b = div = processedKernelSize = 0;

                    // for each kernel row
                    for ( i = 0; i < size; i++ )
                    {
                        ir = i - radius;
                        t = y + ir;

                        // skip row
                        if ( t < startY )
                            continue;
                        // break
                        if ( t >= stopY )
                            break;

                        // for each kernel column
                        for ( j = 0; j < size; j++ )
                        {
                            jr = j - radius;
                            t = x + jr;

                            // skip column
                            if ( t < startX )
                                continue;

                            if ( t < stopX )
                            {
                                k = kernel[i, j];
                                p = &src[ir * srcStride + jr * pixelSize];

                                div += k;

                                r += k * p[RGB.R];
                                g += k * p[RGB.G];
                                b += k * p[RGB.B];

                                processedKernelSize++;
                            }
                        }
                    }

                    // check if all kernel elements were processed
                    if ( processedKernelSize == kernelSize )
                    {
                        // all kernel elements are processed - we are not on the edge
                        div = divisor;
                    }
                    else
                    {
                        // we are on edge. do we need to use dynamic divisor or not?
                        if ( !dynamicDivisorForEdges )
                        {
                            // do
                            div = divisor;
                        }
                    }

                    // check divider
                    if ( div != 0 )
                    {
                        r /= div;
                        g /= div;
                        b /= div;
                    }
                    r += threshold;
                    g += threshold;
                    b += threshold;

                    dst[RGB.R] = (ushort) ( ( r > 65535 ) ? 65535 : ( ( r < 0 ) ? 0 : r ) );
                    dst[RGB.G] = (ushort) ( ( g > 65535 ) ? 65535 : ( ( g < 0 ) ? 0 : g ) );
                    dst[RGB.B] = (ushort) ( ( b > 65535 ) ? 65535 : ( ( b < 0 ) ? 0 : b ) );

                    // take care of alpha channel
                    if ( pixelSize == 4 )
                        dst[RGB.A] = src[RGB.A];
                }
            }
        }

        // Process 64 bpp images including alpha channel
        private unsafe void Process64bppImage( ushort* baseSrc, ushort* baseDst, int srcStride, int dstStride,
                                               int startX, int startY, int stopX, int stopY )
        {
            // loop and array indexes
            int i, j, t, k, ir, jr;
            // kernel's radius
            int radius = size >> 1;
            // color sums
            long r, g, b, a, div;

            // kernel size
            int kernelSize = size * size;
            // number of kernel elements taken into account
            int processedKernelSize;

            ushort* p;

            // for each line
            for ( int y = startY; y < stopY; y++ )
            {
                ushort* src = baseSrc + y * srcStride;
                ushort* dst = baseDst + y * dstStride;

                // for each pixel
                for ( int x = startX; x < stopX; x++, src += 4, dst += 4 )
                {
                    r = g = b = a = div = processedKernelSize = 0;

                    // for each kernel row
                    for ( i = 0; i < size; i++ )
                    {
                        ir = i - radius;
                        t = y + ir;

                        // skip row
                        if ( t < startY )
                            continue;
                        // break
                        if ( t >= stopY )
                            break;

                        // for each kernel column
                        for ( j = 0; j < size; j++ )
                        {
                            jr = j - radius;
                            t = x + jr;

                            // skip column
                            if ( t < startX )
                                continue;

                            if ( t < stopX )
                            {
                                k = kernel[i, j];
                                p = &src[ir * srcStride + jr * 4];

                                div += k;

                                r += k * p[RGB.R];
                                g += k * p[RGB.G];
                                b += k * p[RGB.B];
                                a += k * p[RGB.A];

                                processedKernelSize++;
                            }
                        }
                    }

                    // check if all kernel elements were processed
                    if ( processedKernelSize == kernelSize )
                    {
                        // all kernel elements are processed - we are not on the edge
                        div = divisor;
                    }
                    else
                    {
                        // we are on edge. do we need to use dynamic divisor or not?
                        if ( !dynamicDivisorForEdges )
                        {
                            // do
                            div = divisor;
                        }
                    }

                    // check divider
                    if ( div != 0 )
                    {
                        r /= div;
                        g /= div;
                        b /= div;
                        a /= div;
                    }
                    r += threshold;
                    g += threshold;
                    b += threshold;
                    a += threshold;

                    dst[RGB.R] = (ushort) ( ( r > 65535 ) ? 65535 : ( ( r < 0 ) ? 0 : r ) );
                    dst[RGB.G] = (ushort) ( ( g > 65535 ) ? 65535 : ( ( g < 0 ) ? 0 : g ) );
                    dst[RGB.B] = (ushort) ( ( b > 65535 ) ? 65535 : ( ( b < 0 ) ? 0 : b ) );
                    dst[RGB.A] = (ushort) ( ( a > 65535 ) ? 65535 : ( ( a < 0 ) ? 0 : a ) );
                }
            }
        }
    }
}
