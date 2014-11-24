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
    /// Flood filling with mean color starting from specified point.
    /// </summary>
    /// 
    /// <remarks><para>The filter performs image's area filling (4 directional) starting
    /// from the <see cref="StartingPoint">specified point</see>. It fills
    /// the area of the pointed color, but also fills other colors, which
    /// are similar to the pointed within specified <see cref="Tolerance">tolerance</see>.
    /// The area is filled using its mean color.
    /// </para>
    /// 
    /// <para>The filter is similar to <see cref="PointedColorFloodFill"/> filter, but instead
    /// of filling the are with specified color, it fills the area with its mean color. This means
    /// that this is a two pass filter - first pass is to calculate the mean value and the second pass is to
    /// fill the area. Unlike to <see cref="PointedColorFloodFill"/> filter, this filter has nothing
    /// to do in the case if zero <see cref="Tolerance">tolerance</see> is specified.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images and 24 bpp
    /// color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// PointedMeanFloodFill filter = new PointedMeanFloodFill( );
    /// // configre the filter
    /// filter.Tolerance = Color.FromArgb( 150, 92, 92 );
    /// filter.StartingPoint = new IntPoint( 150, 100 );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/pointed_mean_fill.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    /// <seealso cref="PointedColorFloodFill"/>
    /// 
    public unsafe class PointedMeanFloodFill : BaseInPlacePartialFilter
    {
        // map of pixels, which are already checked by the flood fill algorithm
        private bool[,] checkedPixels;

        // set of variables (which describe image property and min/max color) to avoid passing them
        // recursively as parameters
        byte* scan0;      // pointer to first image line
        int stride;     // size of image's line
        int startX;     // X1 of bounding rectangle
        int stopX;      // Y1 of bounding rectangle
        int startY;     // X2 of bounding rectangle (including)
        int stopY;      // Y2 of bounding rectangle (including)

        // min/max colors
        byte minR, maxR;      // min/max Red
        byte minG, maxG;      // min/max Green (Gray) color
        byte minB, maxB;      // min/max Blue

        // mean color
        int meanR, meanG, meanB;
        int pixelsCount = 0;

        // starting point to fill from
        private IntPoint startingPoint = new IntPoint( 0, 0 );
        // filling tolerance
        private Color tolerance = Color.FromArgb( 16, 16, 16 );

        // format translation dictionary
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
        /// Flood fill tolerance.
        /// </summary>
        /// 
        /// <remarks><para>The tolerance value determines the level of similarity between
        /// colors to fill and the pointed color. If the value is set to zero, then the
        /// filter does nothing, since the filling area contains only one color and its
        /// filling with mean is meaningless.</para>
        /// 
        /// <para>The tolerance value is specified as <see cref="System.Drawing.Color"/>,
        /// where each component (R, G and B) represents tolerance for the corresponding
        /// component of color. This allows to set different tolerances for red, green
        /// and blue components.</para>
        /// 
        /// <para>Default value is set to <b>(16, 16, 16)</b>.</para>
        /// </remarks>
        /// 
        public Color Tolerance
        {
            get { return tolerance; }
            set { tolerance = value; }
        }

        /// <summary>
        /// Point to start filling from.
        /// </summary>
        /// 
        /// <remarks><para>The property allows to set the starting point, where filling is
        /// started from.</para>
        /// 
        /// <remarks>Default value is set to <b>(0, 0)</b>.</remarks>
        /// </remarks>
        /// 
        public IntPoint StartingPoint
        {
            get { return startingPoint; }
            set { startingPoint = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointedMeanFloodFill"/> class.
        /// </summary>
        /// 
        public PointedMeanFloodFill( )
        {
            // initialize format translation dictionary
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
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
            // skip, if there is nothing to fill
            if ( !rect.Contains( startingPoint.X, startingPoint.Y ) || ( tolerance == Color.Black ) )
                return;

            // save bounding rectangle
            startX = rect.Left;
            startY = rect.Top;
            stopX  = rect.Right - 1;
            stopY  = rect.Bottom - 1;

            // save image properties
            scan0 = (byte*) image.ImageData.ToPointer( );
            stride = image.Stride;

            // create map of visited pixels
            checkedPixels = new bool[image.Height, image.Width];

            pixelsCount = meanR = meanG = meanB = 0;

            if ( image.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                byte startColor= *( (byte*) CoordsToPointerGray( startingPoint.X, startingPoint.Y ) );
                minG = (byte) ( Math.Max(   0, startColor - tolerance.G ) );
                maxG = (byte) ( Math.Min( 255, startColor + tolerance.G ) );

                LinearFloodFill4Gray( startingPoint.X, startingPoint.Y );

                // calculate mean value
                meanG /= pixelsCount;
                byte fillG = (byte) meanG;

                // do fill with the mean
                byte* src = (byte*) image.ImageData.ToPointer( );
                // allign pointer to the first pixel to process
                src += ( startY * stride + startX );

                int offset = stride - rect.Width;

                // for each line	
                for ( int y = startY; y <= stopY; y++ )
                {
                    // for each pixel
                    for ( int x = startX; x <= stopX; x++, src++ )
                    {
                        if ( checkedPixels[y, x] )
                        {
                            *src = fillG; 
                        }
                    }
                    src += offset;
                }
            }
            else
            {
                byte* startColor= (byte*) CoordsToPointerRGB( startingPoint.X, startingPoint.Y );

                minR = (byte) ( Math.Max(   0, startColor[RGB.R] - tolerance.R ) );
                maxR = (byte) ( Math.Min( 255, startColor[RGB.R] + tolerance.R ) );
                minG = (byte) ( Math.Max(   0, startColor[RGB.G] - tolerance.G ) );
                maxG = (byte) ( Math.Min( 255, startColor[RGB.G] + tolerance.G ) );
                minB = (byte) ( Math.Max(   0, startColor[RGB.B] - tolerance.B ) );
                maxB = (byte) ( Math.Min( 255, startColor[RGB.B] + tolerance.B ) );

                LinearFloodFill4RGB( startingPoint.X, startingPoint.Y );

                // calculate mean value
                meanR /= pixelsCount;
                meanG /= pixelsCount;
                meanB /= pixelsCount;

                byte fillR = (byte) meanR;
                byte fillG = (byte) meanG;
                byte fillB = (byte) meanB;

                // do fill with the mean
                byte* src = (byte*) image.ImageData.ToPointer( );
                // allign pointer to the first pixel to process
                src += ( startY * stride + startX * 3);

                int offset = stride - rect.Width * 3;

                // for each line	
                for ( int y = startY; y <= stopY; y++ )
                {
                    // for each pixel
                    for ( int x = startX; x <= stopX; x++, src += 3 )
                    {
                        if ( checkedPixels[y, x] )
                        {
                            src[RGB.R] = fillR;
                            src[RGB.G] = fillG;
                            src[RGB.B] = fillB;
                        }
                    }
                    src += offset;
                }
            }
        }

        // Liner flood fill in 4 directions for grayscale images
        private unsafe void LinearFloodFill4Gray( int x, int y )
        {
            // get image pointer for current (X, Y)
            byte* p = (byte*) CoordsToPointerGray( x, y );

            // find left end of line to fill
            int leftLineEdge = x;
            byte* ptr = p;

            while ( true )
            {
                // sum value of the current pixel
                meanG += *ptr;
                pixelsCount++;
                // mark the pixel as checked
                checkedPixels[y, leftLineEdge] = true;

                leftLineEdge--;
                ptr -= 1;

                // check if we need to stop on the edge of image or color area
                if ( ( leftLineEdge < startX ) || ( checkedPixels[y, leftLineEdge] ) || ( !CheckGrayPixel( *ptr ) ) )
                    break;

            }
            leftLineEdge++;

            // find right end of line to fill
            int rightLineEdge = x + 1;
            ptr = p + 1;

            // while we don't need to stop on the edge of image or color area
            while ( !( rightLineEdge > stopX || ( checkedPixels[y, rightLineEdge] ) || ( !CheckGrayPixel( *ptr ) ) ) )
            {
                // sum value of the current pixel
                meanG += *ptr;
                pixelsCount++;
                // mark the pixel as checked
                checkedPixels[y, rightLineEdge] = true;

                rightLineEdge++;
                ptr += 1;

            }
            rightLineEdge--;


            // loop to go up and down
            ptr = (byte*) CoordsToPointerGray( leftLineEdge, y );
            for ( int i = leftLineEdge; i <= rightLineEdge; i++, ptr++ )
            {
                // go up
                if ( ( y > startY ) && ( !checkedPixels[y - 1, i] ) && ( CheckGrayPixel( *( ptr - stride ) ) ) )
                    LinearFloodFill4Gray( i, y - 1 );
                // go down
                if ( ( y < stopY ) && ( !checkedPixels[y + 1, i] ) && ( CheckGrayPixel( *( ptr + stride ) ) ) )
                    LinearFloodFill4Gray( i, y + 1 );
            }
        }

        // Liner flood fill in 4 directions for RGB
        private unsafe void LinearFloodFill4RGB( int x, int y )
        {
            // get image pointer for current (X, Y)
            byte* p = (byte*) CoordsToPointerRGB( x, y );

            // find left end of line to fill
            int leftLineEdge = x;
            byte* ptr = p;

            while ( true )
            {
                // sum value of the current pixel
                meanR += ptr[RGB.R];
                meanG += ptr[RGB.G];
                meanB += ptr[RGB.B];
                pixelsCount++;
                // mark the pixel as checked
                checkedPixels[y, leftLineEdge] = true;

                leftLineEdge--;
                ptr -= 3;

                // check if we need to stop on the edge of image or color area
                if ( ( leftLineEdge < startX ) || ( checkedPixels[y, leftLineEdge] ) || ( !CheckRGBPixel( ptr ) ) )
                    break;

            }
            leftLineEdge++;

            // find right end of line to fill
            int rightLineEdge = x + 1;
            ptr = p + 3;

            // while we don't need to stop on the edge of image or color area
            while ( !( rightLineEdge > stopX || ( checkedPixels[y, rightLineEdge] ) || ( !CheckRGBPixel( ptr ) ) ) )
            {
                // sum value of the current pixel
                meanR += ptr[RGB.R];
                meanG += ptr[RGB.G];
                meanB += ptr[RGB.B];
                pixelsCount++;
                // mark the pixel as checked
                checkedPixels[y, rightLineEdge] = true;

                rightLineEdge++;
                ptr += 3;
            }
            rightLineEdge--;


            // loop to go up and down
            ptr = (byte*) CoordsToPointerRGB( leftLineEdge, y );
            for ( int i = leftLineEdge; i <= rightLineEdge; i++, ptr += 3 )
            {
                // go up
                if ( ( y > startY ) && ( !checkedPixels[y - 1, i] ) && ( CheckRGBPixel( ptr - stride ) ) )
                    LinearFloodFill4RGB( i, y - 1 );
                // go down
                if ( ( y < stopY ) && ( !checkedPixels[y + 1, i] ) && ( CheckRGBPixel( ptr + stride ) ) )
                    LinearFloodFill4RGB( i, y + 1 );
            }
        }

        // Check if pixel equals to the starting color within required tolerance
        private unsafe bool CheckGrayPixel( byte pixel )
        {
            return ( pixel >= minG ) && ( pixel <= maxG );
        }

        // Check if pixel equals to the starting color within required tolerance
        private unsafe bool CheckRGBPixel( byte* pixel )
        {
            return  ( pixel[RGB.R] >= minR ) && ( pixel[RGB.R] <= maxR ) &&
                    ( pixel[RGB.G] >= minG ) && ( pixel[RGB.G] <= maxG ) &&
                    ( pixel[RGB.B] >= minB ) && ( pixel[RGB.B] <= maxB );
        }

        // Convert image coordinate to pointer for Grayscale images
        private byte* CoordsToPointerGray( int x, int y )
        {
            return scan0 + ( stride * y ) + x;
        }

        // Convert image coordinate to pointer for RGB images
        private byte* CoordsToPointerRGB( int x, int y )
        {
            return scan0 + ( stride * y ) + x * 3;
        }
    }
}
