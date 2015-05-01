// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Binary dilatation operator from Mathematical Morphology with 3x3 structuring element.
    /// </summary>
    /// 
    /// <remarks><para>The filter represents an optimized version of <see cref="Dilatation"/>
    /// filter, which is aimed for binary images (containing black and white pixels) processed
    /// with 3x3 structuring element. This makes this filter ideal for growing objects in binary
    /// images – it puts white pixel to the destination image in the case if there is at least
    /// one white neighbouring pixel in the source image.</para>
    /// 
    /// <para>See <see cref="Dilatation"/> filter, which represents generic version of
    /// dilatation filter supporting custom structuring elements and wider range of image formats.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale (binary) images for processing.</para>
    /// </remarks>
    /// 
    /// <seealso cref="Dilatation"/>
    /// <seealso cref="Dilatation3x3"/>
    /// 
    public class BinaryDilatation3x3 : BaseUsingCopyPartialFilter
    {
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
        /// Initializes a new instance of the <see cref="BinaryDilatation3x3"/> class.
        /// </summary>
        /// 
        public BinaryDilatation3x3( )
        {
            // initialize format translation dictionary
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        /// <exception cref="InvalidImagePropertiesException">Processing rectangle mast be at least 3x3 in size.</exception>
        /// 
        protected override unsafe void ProcessFilter( UnmanagedImage sourceData, UnmanagedImage destinationData, Rectangle rect )
        {
            if ( ( rect.Width < 3 ) || ( rect.Height < 3 ) )
            {
                throw new InvalidImagePropertiesException( "Processing rectangle mast be at least 3x3 in size." );
            }

            // processing start and stop X,Y positions
            int startX  = rect.Left + 1;
            int startY  = rect.Top + 1;
            int stopX   = rect.Right - 1;
            int stopY   = rect.Bottom - 1;

            int dstStride = destinationData.Stride;
            int srcStride = sourceData.Stride;

            int dstOffset = dstStride - rect.Width + 1;
            int srcOffset = srcStride - rect.Width + 1;

            // image pointers
            byte* src = (byte*) sourceData.ImageData.ToPointer( );
            byte* dst = (byte*) destinationData.ImageData.ToPointer( );

            // allign pointers by X and Y
            src += ( startX - 1 ) + ( startY - 1 ) * srcStride;
            dst += ( startX - 1 ) + ( startY - 1 ) * dstStride;

            // --- process the first line
            *dst = (byte) ( *src | src[1] | src[srcStride] | src[srcStride + 1] );

            src++;
            dst++;

            // for each pixel
            for ( int x = startX; x < stopX; x++, src++, dst++ )
            {
                *dst = (byte) ( *src | src[-1] | src[1] |
                    src[srcStride] | src[srcStride - 1] | src[srcStride + 1] );
            }

            *dst = (byte) ( *src | src[-1] | src[srcStride] | src[srcStride - 1] );

            src += srcOffset;
            dst += dstOffset;

            // --- process all lines except the last one
            for ( int y = startY; y < stopY; y++ )
            {
                *dst = (byte) ( *src | src[1] |
                    src[-srcStride] | src[-srcStride + 1] |
                    src[srcStride] | src[srcStride + 1] );

                src++;
                dst++;

                // for each pixel
                for ( int x = startX; x < stopX; x++, src++, dst++ )
                {
                    *dst = (byte) ( *src | src[-1] | src[1] |
                        src[-srcStride] | src[-srcStride - 1] | src[-srcStride + 1] |
                        src[ srcStride] | src[ srcStride - 1] | src[ srcStride + 1] );
                }

                *dst = (byte) ( *src | src[-1] |
                    src[-srcStride] | src[-srcStride - 1] |
                    src[srcStride] | src[srcStride - 1] );

                src += srcOffset;
                dst += dstOffset;
            }

            // --- process the last line
            *dst = (byte) ( *src | src[1] | src[-srcStride] | src[-srcStride + 1] );

            src++;
            dst++;

            // for each pixel
            for ( int x = startX; x < stopX; x++, src++, dst++ )
            {
                *dst = (byte) ( *src | src[-1] | src[1] |
                    src[-srcStride] | src[-srcStride - 1] | src[-srcStride + 1] );
            }

            *dst = (byte) ( *src | src[-1] | src[-srcStride] | src[-srcStride - 1] );
        }
    }
}
