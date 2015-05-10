// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2010
// contacts@aforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Flood filling with specified color starting from specified point.
    /// </summary>
    /// 
    /// <remarks><para>The filter performs image's area filling (4 directional) starting
    /// from the <see cref="StartingPoint">specified point</see>. It fills
    /// the area of the pointed color, but also fills other colors, which
    /// are similar to the pointed within specified <see cref="Tolerance">tolerance</see>.
    /// The area is filled using <see cref="FillColor">specified fill color</see>.
    /// </para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images and 24 bpp
    /// color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// PointedColorFloodFill filter = new PointedColorFloodFill( );
    /// // configure the filter
    /// filter.Tolerance = Color.FromArgb( 150, 92, 92 );
    /// filter.FillColor = Color.FromArgb( 255, 255, 255 );
    /// filter.StartingPoint = new IntPoint( 150, 100 );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/pointed_color_fill.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    /// <seealso cref="PointedMeanFloodFill"/>
    /// 
    public unsafe class PointedColorFloodFill : BaseInPlacePartialFilter
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

        // fill color
        byte fillR, fillG, fillB;

        // starting point to fill from
        private IntPoint startingPoint = new IntPoint( 0, 0 );
        // filling tolerance
        private Color tolerance = Color.FromArgb( 0, 0, 0 );

        // format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        /// Flood fill tolerance.
        /// </summary>
        /// 
        /// <remarks><para>The tolerance value determines which colors to fill. If the
        /// value is set to 0, then only color of the <see cref="StartingPoint">pointed pixel</see>
        /// is filled. If the value is not 0, then other colors may be filled as well,
        /// which are similar to the color of the pointed pixel within the specified
        /// tolerance.</para>
        /// 
        /// <para>The tolerance value is specified as <see cref="System.Drawing.Color"/>,
        /// where each component (R, G and B) represents tolerance for the corresponding
        /// component of color. This allows to set different tolerances for red, green
        /// and blue components.</para>
        /// </remarks>
        /// 
        public Color Tolerance
        {
            get { return tolerance; }
            set { tolerance = value; }
        }

        /// <summary>
        /// Fill color.
        /// </summary>
        /// 
        /// <remarks><para>The fill color is used to fill image's area starting from the
        /// <see cref="StartingPoint">specified point</see>.</para>
        /// 
        /// <para>For grayscale images the color needs to be specified with all three
        /// RGB values set to the same value, (128, 128, 128) for example.</para>
        /// 
        /// <para>Default value is set to <b>black</b>.</para>
        /// </remarks>
        /// 
        public Color FillColor
        {
            get { return Color.FromArgb( fillR, fillG, fillB ); }
            set
            {
                fillR = value.R;
                fillG = value.G;
                fillB = value.B;
            }
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
        /// Initializes a new instance of the <see cref="PointedColorFloodFill"/> class.
        /// </summary>
        /// 
        public PointedColorFloodFill( )
        {
            // initialize format translation dictionary
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointedColorFloodFill"/> class.
        /// </summary>
        /// 
        /// <param name="fillColor">Fill color.</param>
        /// 
        public PointedColorFloodFill( Color fillColor )
            : this( )
        {
            FillColor = fillColor;
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
            if ( !rect.Contains( startingPoint.X, startingPoint.Y ) )
                return;

            // save bounding rectangle
            startX = rect.Left;
            startY = rect.Top;
            stopX  = rect.Right - 1;
            stopY  = rect.Bottom - 1;

            // save image properties
            scan0 = (byte*) image.ImageData.ToPointer( );
            stride = image.Stride;

            // create map visited pixels
            checkedPixels = new bool[image.Height, image.Width];

            if ( image.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                byte startColor = *( (byte*) CoordsToPointerGray( startingPoint.X, startingPoint.Y ) );
                minG = (byte) ( Math.Max(   0, startColor - tolerance.G ) );
                maxG = (byte) ( Math.Min( 255, startColor + tolerance.G ) );

                LinearFloodFill4Gray( startingPoint );
            }
            else
            {
                byte* startColor = (byte*) CoordsToPointerRGB( startingPoint.X, startingPoint.Y );

                minR = (byte) ( Math.Max(   0, startColor[RGB.R] - tolerance.R ) );
                maxR = (byte) ( Math.Min( 255, startColor[RGB.R] + tolerance.R ) );
                minG = (byte) ( Math.Max(   0, startColor[RGB.G] - tolerance.G ) );
                maxG = (byte) ( Math.Min( 255, startColor[RGB.G] + tolerance.G ) );
                minB = (byte) ( Math.Max(   0, startColor[RGB.B] - tolerance.B ) );
                maxB = (byte) ( Math.Min( 255, startColor[RGB.B] + tolerance.B ) );

                LinearFloodFill4RGB( startingPoint );
            }
        }

        // Liner flood fill in 4 directions for grayscale images
        private unsafe void LinearFloodFill4Gray( IntPoint startingPoint )
        {
            Queue<IntPoint> points = new Queue<IntPoint>( );
            points.Enqueue( startingPoint );

            while ( points.Count > 0 )
            {
                IntPoint point = points.Dequeue( );

                int x = point.X;
                int y = point.Y;

                // get image pointer for current (X, Y)
                byte* p = (byte*) CoordsToPointerGray( x, y );

                // find left end of line to fill
                int leftLineEdge = x;
                byte* ptr = p;

                while ( true )
                {
                    // fill current pixel
                    *ptr = fillG;
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
                int rightLineEdge = x;
                ptr = p;

                while ( true )
                {
                    // fill current pixel
                    *ptr = fillG;
                    // mark the pixel as checked
                    checkedPixels[y, rightLineEdge] = true;

                    rightLineEdge++;
                    ptr += 1;

                    // check if we need to stop on the edge of image or color area
                    if ( rightLineEdge > stopX || ( checkedPixels[y, rightLineEdge] ) || ( !CheckGrayPixel( *ptr ) ) )
                        break;
                }
                rightLineEdge--;


                // loop to go up and down
                ptr = (byte*) CoordsToPointerGray( leftLineEdge, y );

                bool upperPointIsQueued = false;
                bool lowerPointIsQueued = false;
                int upperY = y - 1;
                int lowerY = y + 1;

                for ( int i = leftLineEdge; i <= rightLineEdge; i++, ptr++ )
                {
                    // go up
                    if ( ( y > startY ) && ( !checkedPixels[y - 1, i] ) && ( CheckGrayPixel( *( ptr - stride ) ) ) )
                    {
                        if ( !upperPointIsQueued )
                        {
                            points.Enqueue( new IntPoint( i, upperY ) );
                            upperPointIsQueued = true;
                        }
                    }
                    else
                    {
                        upperPointIsQueued = false;
                    }

                    // go down
                    if ( ( y < stopY ) && ( !checkedPixels[y + 1, i] ) && ( CheckGrayPixel( *( ptr + stride ) ) ) )
                    {
                        if ( !lowerPointIsQueued )
                        {
                            points.Enqueue( new IntPoint( i, lowerY ) );
                            lowerPointIsQueued = true;
                        }
                    }
                    else
                    {
                        lowerPointIsQueued = false;
                    }
                }
            }
        }

        // Liner flood fill in 4 directions for RGB
        private unsafe void LinearFloodFill4RGB( IntPoint startPoint )
        {
            Queue<IntPoint> points = new Queue<IntPoint>( );
            points.Enqueue( startingPoint );

            while ( points.Count > 0 )
            {
                IntPoint point = points.Dequeue( );

                int x = point.X;
                int y = point.Y;

                // get image pointer for current (X, Y)
                byte* p = (byte*) CoordsToPointerRGB( x, y );

                // find left end of line to fill
                int leftLineEdge = x;
                byte* ptr = p;

                while ( true )
                {
                    // fill current pixel
                    ptr[RGB.R] = fillR;
                    ptr[RGB.G] = fillG;
                    ptr[RGB.B] = fillB;
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
                int rightLineEdge = x;
                ptr = p;

                while ( true )
                {
                    // fill current pixel
                    ptr[RGB.R] = fillR;
                    ptr[RGB.G] = fillG;
                    ptr[RGB.B] = fillB;
                    // mark the pixel as checked
                    checkedPixels[y, rightLineEdge] = true;

                    rightLineEdge++;
                    ptr += 3;

                    // check if we need to stop on the edge of image or color area
                    if ( rightLineEdge > stopX || ( checkedPixels[y, rightLineEdge] ) || ( !CheckRGBPixel( ptr ) ) )
                        break;
                }
                rightLineEdge--;

                // loop to go up and down
                ptr = (byte*) CoordsToPointerRGB( leftLineEdge, y );

                bool upperPointIsQueued = false;
                bool lowerPointIsQueued = false;
                int upperY = y - 1;
                int lowerY = y + 1;

                for ( int i = leftLineEdge; i <= rightLineEdge; i++, ptr += 3 )
                {
                    // go up
                    if ( ( y > startY ) && ( !checkedPixels[upperY, i] ) && ( CheckRGBPixel( ptr - stride ) ) )
                    {
                        if ( !upperPointIsQueued )
                        {
                            points.Enqueue( new IntPoint( i, upperY ) );
                            upperPointIsQueued = true;
                        }
                    }
                    else
                    {
                        upperPointIsQueued = false;
                    }

                    // go down
                    if ( ( y < stopY ) && ( !checkedPixels[lowerY, i] ) && ( CheckRGBPixel( ptr + stride ) ) )
                    {
                        if ( !lowerPointIsQueued )
                        {
                            points.Enqueue( new IntPoint( i, lowerY ) );
                            lowerPointIsQueued = true;
                        }
                    }
                    else
                    {
                        lowerPointIsQueued = false;
                    }
                }
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
