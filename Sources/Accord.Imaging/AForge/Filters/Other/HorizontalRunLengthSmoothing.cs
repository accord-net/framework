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
    /// Horizontal run length smoothing algorithm.
    /// </summary>
    /// 
    /// <remarks><para>The class implements horizontal run length smoothing algorithm, which
    /// is described in: <b>K.Y. Wong, R.G. Casey and F.M. Wahl, "Document analysis system,"
    /// IBM J. Res. Devel., Vol. 26, NO. 6,111). 647-656, 1982.</b></para>
    /// 
    /// <para>Unlike the original description of this algorithm, this implementation must be applied
    /// to inverted binary images containing document, i.e. white text on black background. So this
    /// implementation fills horizontal black gaps between white pixels.</para>
    /// 
    /// <para><note>This algorithm is usually used together with <see cref="VerticalRunLengthSmoothing"/>,
    /// <see cref="Intersect"/> and then further analysis of white blobs.</note></para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images, which are supposed to be binary inverted documents.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// HorizontalRunLengthSmoothing hrls = new HorizontalRunLengthSmoothing( 32 );
    /// // apply the filter
    /// hrls.ApplyInPlace( image );
    /// </code>
    ///
    /// <para><b>Source image:</b></para>
    /// <img src="img/imaging/sample24.png" width="480" height="320" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/hrls.png" width="480" height="320" />
    /// </remarks>
    /// 
    /// <seealso cref="VerticalRunLengthSmoothing"/>
    /// 
    public class HorizontalRunLengthSmoothing : BaseInPlacePartialFilter
    {
        private int maxGapSize = 10;
        private bool processGapsWithImageBorders = false;

        /// <summary>
        /// Maximum gap size to fill (in pixels).
        /// </summary>
        /// 
        /// <remarks><para>The property specifies maximum horizontal gap between white pixels to fill.
        /// If number of black pixels between some white pixels is bigger than this value, then those
        /// black pixels are left as is; otherwise the gap is filled with white pixels.
        /// </para>
        /// 
        /// <para>Default value is set to <b>10</b>. Minimum value is 1. Maximum value is 1000.</para></remarks>
        ///
        public int MaxGapSize
        {
            get { return maxGapSize; }
            set { maxGapSize = Math.Max( 1, Math.Min( 1000, value ) ); }
        }

        /// <summary>
        /// Process gaps between objects and image borders or not.
        /// </summary>
        /// 
        /// <remarks><para>The property sets if gaps between image borders and objects must be treated as
        /// gaps between objects and also filled.</para>
        /// 
        /// <para>Default value is set to <see langword="false"/>.</para>
        /// </remarks>
        /// 
        public bool ProcessGapsWithImageBorders
        {
            get { return processGapsWithImageBorders; }
            set { processGapsWithImageBorders = value; }
        }

        // private format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        /// 
        /// <remarks><para>See <see cref="IFilterInformation.FormatTranslations"/>
        /// documentation for additional information.</para></remarks>
        /// 
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalRunLengthSmoothing"/> class.
        /// </summary>
        /// 
        public HorizontalRunLengthSmoothing( )
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalRunLengthSmoothing"/> class.
        /// </summary>
        /// 
        /// <param name="maxGapSize">Maximum gap size to fill (see <see cref="MaxGapSize"/>).</param>
        /// 
        public HorizontalRunLengthSmoothing( int maxGapSize ) : this( )
        {
            MaxGapSize = maxGapSize;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        ///
        protected override unsafe void ProcessFilter( UnmanagedImage image, Rectangle rect )
        {
            int startY = rect.Top;
            int stopY  = startY + rect.Height;
            int width  = rect.Width;
            int offset = image.Stride - rect.Width;

            byte* ptr = (byte*) image.ImageData.ToPointer( ) + startY * image.Stride + rect.Left;

            for ( int y = startY; y < stopY; y++ )
            {
                byte* lineStart = ptr;
                byte* lineEndPtr = ptr + width;
                
                // fill gaps between white pixels
                while ( ptr < lineEndPtr )
                {
                    byte* gapStart = ptr;

                    // look for non black pixel
                    while ( ( ptr < lineEndPtr ) && ( *ptr == 0 ) )
                    {
                        ptr++;
                    }

                    // fill the gap between white areas
                    if ( ptr - gapStart <= maxGapSize )
                    {
                        if ( ( processGapsWithImageBorders ) ||
                           ( ( gapStart != lineStart ) && ( ptr != lineEndPtr ) ) )
                        {
                            while ( gapStart < ptr )
                            {
                                *gapStart = 255;
                                gapStart++;
                            }
                        }
                    }

                    // skip all non black pixels
                    while ( ( ptr < lineEndPtr ) && ( *ptr != 0 ) )
                    {
                        ptr++;
                    }
                }

                ptr += offset;
            }
        }
    }
}
