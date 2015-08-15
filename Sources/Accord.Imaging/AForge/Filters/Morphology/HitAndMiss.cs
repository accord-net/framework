// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@aaforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Hit-And-Miss operator from Mathematical Morphology.
    /// </summary>
    /// 
    /// <remarks><para>The hit-and-miss filter represents generalization of <see cref="Erosion"/>
    /// and <see cref="Dilatation"/> filters by extending flexibility of structuring element and
    /// providing different modes of its work. Structuring element may contain:
    /// <list type="bullet">
    /// <item>1 - foreground;</item>
    /// <item>0 - background;</item>
    /// <item>-1 - don't care.</item>
    /// </list>
    /// </para>
    /// 
    /// <para>Filter's mode is set by <see cref="Mode"/> property. The list of modes and its
    /// documentation may be found in <see cref="Modes"/> enumeration.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images for processing. <b>Note</b>: grayscale images are treated
    /// as binary with 0 value equals to black and 255 value equals to white.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // define kernel to remove pixels on the right side of objects
    /// // (pixel is removed, if there is white pixel on the left and
    /// // black pixel on the right)
    /// short[,] se = new short[,] {
    ///     { -1, -1, -1 },
    ///     {  1,  1,  0 },
    ///     { -1, -1, -1 }
    /// };
    /// // create filter
    /// HitAndMiss filter = new HitAndMiss( se, HitAndMiss.Modes.Thinning );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample12.png" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/hit-and-miss.png" width="320" height="240" />
    /// </remarks>
    /// 
    public class HitAndMiss : BaseUsingCopyPartialFilter
    {
        /// <summary>
        /// Hit and Miss modes.
        /// </summary>
        /// 
        /// <remarks><para>Bellow is a list of modes meaning depending on pixel's correspondence
        /// to specified structuring element:
        /// <list type="bullet">
        /// <item><see cref="Modes.HitAndMiss"/> - on match pixel is set to white, otherwise to black;</item>
        /// <item><see cref="Modes.Thinning"/> - on match pixel is set to black, otherwise not changed.</item>
        /// <item><see cref="Modes.Thickening"/> - on match pixel is set to white, otherwise not changed.</item>
        /// </list>
        /// </para></remarks>
        /// 
        public enum Modes
        {
            /// <summary>
            /// Hit and miss mode.
            /// </summary>
            HitAndMiss = 0,

            /// <summary>
            /// Thinning mode.
            /// </summary>
            Thinning = 1,

            /// <summary>
            /// Thickening mode.
            /// </summary>
            Thickening = 2
        }

        // structuring element
        private short[,] se;
        private int size;

        // operation mode
        private Modes mode = Modes.HitAndMiss;

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
        /// Operation mode.
        /// </summary>
        /// 
        /// <remarks><para>Mode to use for the filter. See <see cref="Modes"/> enumeration
        /// for the list of available modes and their documentation.</para>
        /// 
        /// <para>Default mode is set to <see cref="Modes.HitAndMiss"/>.</para></remarks>
        /// 
        public Modes Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HitAndMiss"/> class.
        /// </summary>
        /// 
        /// <param name="se">Structuring element.</param>
        ///
        /// <remarks><para>Structuring elemement for the hit-and-miss morphological operator
        /// must be square matrix with odd size in the range of [3, 99].</para></remarks>
        /// 
        /// <exception cref="ArgumentException">Invalid size of structuring element.</exception>
        /// 
        public HitAndMiss( short[,] se )
        {
            int s = se.GetLength( 0 );

            // check structuring element size
            if ( ( s != se.GetLength( 1 ) ) || ( s < 3 ) || ( s > 99 ) || ( s % 2 == 0 ) )
                throw new ArgumentException( );

            this.se = se;
            this.size = s;

            // initialize format translation dictionary
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HitAndMiss"/> class.
        /// </summary>
        /// 
        /// <param name="se">Structuring element.</param>
        /// <param name="mode">Operation mode.</param>
        /// 
        public HitAndMiss( short[,] se, Modes mode )
            : this( se )
        {
            this.mode = mode;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        protected override unsafe void ProcessFilter( UnmanagedImage sourceData, UnmanagedImage destinationData, Rectangle rect )
        {
            // processing start and stop X,Y positions
            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;

            int srcStride = sourceData.Stride;
            int dstStride = destinationData.Stride;
            int srcOffset = srcStride - rect.Width;
            int dstOffset = dstStride - rect.Width;

            // loop and array indexes
            int ir, jr, i, j;
            // structuring element's radius
            int r = size >> 1;
            // pixel value
            byte dstValue, v;
            // structuring element's value
            short sv;

            // mode values
            byte[] hitValue  = new byte[3] { 255, 0, 255 };
            byte[] missValue = new byte[3] { 0, 0, 0 };
            int modeIndex = (int) mode;

            // do the job
            byte* src = (byte*) sourceData.ImageData.ToPointer( );
            byte* dst = (byte*) destinationData.ImageData.ToPointer( );

            // allign pointers to the first pixel to process
            src += ( startY * srcStride + startX );
            dst += ( startY * dstStride + startX );

            // for each line
            for ( int y = startY; y < stopY; y++ )
            {
                // for each pixel
                for ( int x = startX; x < stopX; x++, src++, dst++ )
                {
                    missValue[1] = missValue[2] = *src;
                    dstValue = 255;

                    // for each structuring element's row
                    for ( i = 0; i < size; i++ )
                    {
                        ir = i - r;

                        // for each structuring element's column
                        for ( j = 0; j < size; j++ )
                        {
                            jr = j - r;

                            // get structuring element's value
                            sv = se[i, j];

                            // skip "don't care" values
                            if ( sv == -1 )
                                continue;

                            // check, if we outside
                            if (
                                ( y + ir < startY ) || ( y + ir >= stopY ) ||
                                ( x + jr < startX ) || ( x + jr >= stopX )
                                )
                            {
                                // if it so, the result is zero,
                                // because it was required pixel
                                dstValue = 0;
                                break;
                            }

                            // get source image value
                            v = src[ir * srcStride + jr];

                            if (
                                ( ( sv != 0 ) || ( v != 0 ) ) &&
                                ( ( sv != 1 ) || ( v != 255 ) )
                                )
                            {
                                // failed structuring element mutch
                                dstValue = 0;
                                break;
                            }
                        }

                        if ( dstValue == 0 )
                            break;
                    }
                    // result pixel
                    *dst = ( dstValue == 255 ) ? hitValue[modeIndex] : missValue[modeIndex];
                }
                src += srcOffset;
                dst += dstOffset;
            }
        }
    }
}
