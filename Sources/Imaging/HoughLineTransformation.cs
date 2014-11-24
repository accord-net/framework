// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging
{
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Hough line.
    /// </summary>
    /// 
    /// <remarks><para>Represents line of Hough Line transformation using
    /// <a href="http://en.wikipedia.org/wiki/Polar_coordinate_system">polar coordinates</a>.
    /// See <a href="http://en.wikipedia.org/wiki/Polar_coordinate_system#Converting_between_polar_and_Cartesian_coordinates">Wikipedia</a>
    /// for information on how to convert polar coordinates to Cartesian coordinates.
    /// </para>
    /// 
    /// <para><note><see cref="HoughLineTransformation">Hough Line transformation</see> does not provide
    /// information about lines start and end points, only slope and distance from image's center. Using
    /// only provided information it is not possible to draw the detected line as it exactly appears on
    /// the source image. But it is possible to draw a line through the entire image, which contains the
    /// source line (see sample code below).
    /// </note></para>
    /// 
    /// <para>Sample code to draw detected Hough lines:</para>
    /// <code>
    /// HoughLineTransformation lineTransform = new HoughLineTransformation( );
    /// // apply Hough line transofrm
    /// lineTransform.ProcessImage( sourceImage );
    /// Bitmap houghLineImage = lineTransform.ToBitmap( );
    /// // get lines using relative intensity
    /// HoughLine[] lines = lineTransform.GetLinesByRelativeIntensity( 0.5 );
    /// 
    /// foreach ( HoughLine line in lines )
    /// {
    ///     // get line's radius and theta values
    ///     int    r = line.Radius;
    ///     double t = line.Theta;
    ///     
    ///     // check if line is in lower part of the image
    ///     if ( r &lt; 0 )
    ///     {
    ///         t += 180;
    ///         r = -r;
    ///     }
    ///     
    ///     // convert degrees to radians
    ///     t = ( t / 180 ) * Math.PI;
    ///     
    ///     // get image centers (all coordinate are measured relative
    ///     // to center)
    ///     int w2 = image.Width /2;
    ///     int h2 = image.Height / 2;
    ///     
    ///     double x0 = 0, x1 = 0, y0 = 0, y1 = 0;
    ///     
    ///     if ( line.Theta != 0 )
    ///     {
    ///         // none-vertical line
    ///         x0 = -w2; // most left point
    ///         x1 = w2;  // most right point
    ///     
    ///         // calculate corresponding y values
    ///         y0 = ( -Math.Cos( t ) * x0 + r ) / Math.Sin( t );
    ///         y1 = ( -Math.Cos( t ) * x1 + r ) / Math.Sin( t );
    ///     }
    ///     else
    ///     {
    ///         // vertical line
    ///         x0 = line.Radius;
    ///         x1 = line.Radius;
    ///     
    ///         y0 = h2;
    ///         y1 = -h2;
    ///     }
    ///     
    ///     // draw line on the image
    ///     Drawing.Line( sourceData,
    ///         new IntPoint( (int) x0 + w2, h2 - (int) y0 ),
    ///         new IntPoint( (int) x1 + w2, h2 - (int) y1 ),
    ///         Color.Red );
    /// }
    /// </code>
    /// 
    /// <para>To clarify meaning of <see cref="Radius"/> and <see cref="Theta"/> values
    /// of detected Hough lines, let's take a look at the below sample image and
    /// corresponding values of radius and theta for the lines on the image:
    /// </para>
    /// 
    /// <img src="img/imaging/sample15.png" width="400" height="300" />
    /// 
    /// <para>Detected radius and theta values (color in corresponding colors):
    /// <list type="bullet">
    /// <item><font color="#FF0000">Theta = 90, R = 125, I = 249</font>;</item>
    /// <item><font color="#00FF00">Theta = 0, R = -170, I = 187</font> (converts to Theta = 180, R = 170);</item>
    /// <item><font color="#0000FF">Theta = 90, R = -58, I = 163</font> (converts to Theta = 270, R = 58);</item>
    /// <item><font color="#FFFF00">Theta = 101, R = -101, I = 130</font> (converts to Theta = 281, R = 101);</item>
    /// <item><font color="#FF8000">Theta = 0, R = 43, I = 112</font>;</item>
    /// <item><font color="#FF80FF">Theta = 45, R = 127, I = 82</font>.</item>
    /// </list>
    /// </para>
    /// 
    /// </remarks>
    /// 
    /// <seealso cref="HoughLineTransformation"/>
    /// 
    public class HoughLine : IComparable
    {
        /// <summary>
        /// Line's slope - angle between polar axis and line's radius (normal going
        /// from pole to the line). Measured in degrees, [0, 180).
        /// </summary>
        public readonly double  Theta;

        /// <summary>
        /// Line's distance from image center, (−∞, +∞).
        /// </summary>
        /// 
        /// <remarks><note>Negative line's radius means, that the line resides in lower
        /// part of the polar coordinates system. This means that <see cref="Theta"/> value
        /// should be increased by 180 degrees and radius should be made positive.
        /// </note></remarks>
        /// 
        public readonly short	Radius;

        /// <summary>
        /// Line's absolute intensity, (0, +∞).
        /// </summary>
        /// 
        /// <remarks><para>Line's absolute intensity is a measure, which equals
        /// to number of pixels detected on the line. This value is bigger for longer
        /// lines.</para>
        /// 
        /// <para><note>The value may not be 100% reliable to measure exact number of pixels
        /// on the line. Although these value correlate a lot (which means they are very close
        /// in most cases), the intensity value may slightly vary.</note></para>
        /// </remarks>
        /// 
        public readonly short	Intensity;

        /// <summary>
        /// Line's relative intensity, (0, 1].
        /// </summary>
        /// 
        /// <remarks><para>Line's relative intensity is relation of line's <see cref="Intensity"/>
        /// value to maximum found intensity. For the longest line (line with highest intesity) the
        /// relative intensity is set to 1. If line's relative is set 0.5, for example, this means
        /// its intensity is half of maximum found intensity.</para>
        /// </remarks>
        /// 
        public readonly double  RelativeIntensity;

        /// <summary>
        /// Initializes a new instance of the <see cref="HoughLine"/> class.
        /// </summary>
        /// 
        /// <param name="theta">Line's slope.</param>
        /// <param name="radius">Line's distance from image center.</param>
        /// <param name="intensity">Line's absolute intensity.</param>
        /// <param name="relativeIntensity">Line's relative intensity.</param>
        /// 
        public HoughLine( double theta, short radius, short intensity, double relativeIntensity )
        {
            Theta = theta;
            Radius = radius;
            Intensity = intensity;
            RelativeIntensity = relativeIntensity;
        }

        /// <summary>
        /// Compare the object with another instance of this class.
        /// </summary>
        /// 
        /// <param name="value">Object to compare with.</param>
        /// 
        /// <returns><para>A signed number indicating the relative values of this instance and <b>value</b>: 1) greater than zero - 
        /// this instance is greater than <b>value</b>; 2) zero - this instance is equal to <b>value</b>;
        /// 3) greater than zero - this instance is less than <b>value</b>.</para>
        /// 
        /// <para><note>The sort order is descending.</note></para></returns>
        /// 
        /// <remarks>
        /// <para><note>Object are compared using their <see cref="Intensity">intensity</see> value.</note></para>
        /// </remarks>
        /// 
        public int CompareTo( object value )
        {
            return ( -Intensity.CompareTo( ( (HoughLine) value ).Intensity ) );
        }
    }

    /// <summary>
    /// Hough line transformation.
    /// </summary>
    ///
    /// <remarks><para>The class implements Hough line transformation, which allows to detect
    /// straight lines in an image. Lines, which are found by the class, are provided in
    /// <a href="http://en.wikipedia.org/wiki/Polar_coordinate_system">polar coordinates system</a> -
    /// lines' distances from image's center and lines' slopes are provided.
    /// The pole of polar coordinates system is put into processing image's center and the polar
    /// axis is directed to the right from the pole. Lines' slope is measured in degrees and
    /// is actually represented by angle between polar axis and line's radius (normal going
    /// from pole to the line), which is measured in counter-clockwise direction.
    /// </para>
    /// 
    /// <para><note>Found lines may have negative <see cref="HoughLine.Radius">radius</see>.
    /// This means, that the line resides in lower part of the polar coordinates system
    /// and its <see cref="HoughLine.Theta"/> value should be increased by 180 degrees and
    /// radius should be made positive.
    /// </note></para>
    /// 
    /// <para>The class accepts binary images for processing, which are represented by 8 bpp grayscale images.
    /// All black pixels (0 pixel's value) are treated as background, but pixels with different value are
    /// treated as lines' pixels.</para>
    /// 
    /// <para>See also documentation to <see cref="HoughLine"/> class for additional information
    /// about Hough Lines.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// HoughLineTransformation lineTransform = new HoughLineTransformation( );
    /// // apply Hough line transofrm
    /// lineTransform.ProcessImage( sourceImage );
    /// Bitmap houghLineImage = lineTransform.ToBitmap( );
    /// // get lines using relative intensity
    /// HoughLine[] lines = lineTransform.GetLinesByRelativeIntensity( 0.5 );
    /// 
    /// foreach ( HoughLine line in lines )
    /// {
    ///     // ...
    /// }
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample8.jpg" width="400" height="300" />
    /// <para><b>Hough line transformation image:</b></para>
    /// <img src="img/imaging/hough_lines.jpg" width="500" height="180" />
    /// </remarks>
    /// 
    /// <seealso cref="HoughLine"/>
    /// 
    public class HoughLineTransformation
    {
        // Hough transformation quality settings
        private int     stepsPerDegree;
        private int     houghHeight;
        private double  thetaStep;

        // precalculated Sine and Cosine values
        private double[]	sinMap;
        private double[]	cosMap;
        // Hough map
        private short[,]	houghMap;
        private short		maxMapIntensity = 0;

        private int 		localPeakRadius = 4;
        private short       minLineIntensity = 10;
        private ArrayList   lines = new ArrayList( );

        /// <summary>
        /// Steps per degree.
        /// </summary>
        /// 
        /// <remarks><para>The value defines quality of Hough line transformation and its ability to detect
        /// lines' slope precisely.</para>
        /// 
        /// <para>Default value is set to <b>1</b>. Minimum value is <b>1</b>. Maximum value is <b>10</b>.</para></remarks>
        /// 
        public int StepsPerDegree
        {
            get { return stepsPerDegree; }
            set
            {
                stepsPerDegree = Math.Max( 1, Math.Min( 10, value ) );
                houghHeight = 180 * stepsPerDegree;
                thetaStep = Math.PI / houghHeight;

                // precalculate Sine and Cosine values
                sinMap = new double[houghHeight];
                cosMap = new double[houghHeight];

                for ( int i = 0; i < houghHeight; i++ )
                {
                    sinMap[i] = Math.Sin( i * thetaStep );
                    cosMap[i] = Math.Cos( i * thetaStep );
                }
            }
        }

        /// <summary>
        /// Minimum <see cref="HoughLine.Intensity">line's intensity</see> in Hough map to recognize a line.
        /// </summary>
        ///
        /// <remarks><para>The value sets minimum intensity level for a line. If a value in Hough
        /// map has lower intensity, then it is not treated as a line.</para>
        /// 
        /// <para>Default value is set to <b>10</b>.</para></remarks>
        ///
        public short MinLineIntensity
        {
            get { return minLineIntensity; }
            set { minLineIntensity = value; }
        }

        /// <summary>
        /// Radius for searching local peak value.
        /// </summary>
        /// 
        /// <remarks><para>The value determines radius around a map's value, which is analyzed to determine
        /// if the map's value is a local maximum in specified area.</para>
        /// 
        /// <para>Default value is set to <b>4</b>. Minimum value is <b>1</b>. Maximum value is <b>10</b>.</para></remarks>
        /// 
        public int LocalPeakRadius
        {
            get { return localPeakRadius; }
            set { localPeakRadius = Math.Max( 1, Math.Min( 10, value ) ); }
        }

        /// <summary>
        /// Maximum found <see cref="HoughLine.Intensity">intensity</see> in Hough map.
        /// </summary>
        /// 
        /// <remarks><para>The property provides maximum found line's intensity.</para></remarks>
        /// 
        public short MaxIntensity
        {
            get { return maxMapIntensity; }
        }

        /// <summary>
        /// Found lines count.
        /// </summary>
        /// 
        /// <remarks><para>The property provides total number of found lines, which intensity is higher (or equal to),
        /// than the requested <see cref="MinLineIntensity">minimum intensity</see>.</para></remarks>
        /// 
        public int LinesCount
        {
            get { return lines.Count; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HoughLineTransformation"/> class.
        /// </summary>
        /// 
        public HoughLineTransformation( )
        {
            StepsPerDegree = 1;
        }

        /// <summary>
        /// Process an image building Hough map.
        /// </summary>
        /// 
        /// <param name="image">Source image to process.</param>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// 
        public void ProcessImage( Bitmap image )
        {
            ProcessImage( image, new Rectangle( 0, 0, image.Width, image.Height ) );
        }

        /// <summary>
        /// Process an image building Hough map.
        /// </summary>
        /// 
        /// <param name="image">Source image to process.</param>
        /// <param name="rect">Image's rectangle to process.</param>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// 
        public void ProcessImage( Bitmap image, Rectangle rect )
        {
            // check image format
            if ( image.PixelFormat != PixelFormat.Format8bppIndexed )
            {
                throw new UnsupportedImageFormatException( "Unsupported pixel format of the source image." );
            }

            // lock source image
            BitmapData imageData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );

            try
            {
                // process the image
                ProcessImage( new UnmanagedImage( imageData ), rect );
            }
            finally
            {
                // unlock image
                image.UnlockBits( imageData );
            }
        }

        /// <summary>
        /// Process an image building Hough map.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to process.</param>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// 
        public void ProcessImage( BitmapData imageData )
        {
            ProcessImage( new UnmanagedImage( imageData ),
                new Rectangle( 0, 0, imageData.Width, imageData.Height ) );
        }

        /// <summary>
        /// Process an image building Hough map.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to process.</param>
        /// <param name="rect">Image's rectangle to process.</param>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// 
        public void ProcessImage( BitmapData imageData, Rectangle rect )
        {
            ProcessImage( new UnmanagedImage( imageData ), rect );
        }

        /// <summary>
        /// Process an image building Hough map.
        /// </summary>
        /// 
        /// <param name="image">Source unmanaged image to process.</param>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// 
        public void ProcessImage( UnmanagedImage image )
        {
            ProcessImage( image, new Rectangle( 0, 0, image.Width, image.Height ) );
        }

        /// <summary>
        /// Process an image building Hough map.
        /// </summary>
        /// 
        /// <param name="image">Source unmanaged image to process.</param>
        /// <param name="rect">Image's rectangle to process.</param>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// 
        public void ProcessImage( UnmanagedImage image, Rectangle rect )
        {
            if ( image.PixelFormat != PixelFormat.Format8bppIndexed )
            {
                throw new UnsupportedImageFormatException( "Unsupported pixel format of the source image." );
            }

            // get source image size
            int width       = image.Width;
            int height      = image.Height;
            int halfWidth   = width / 2;
            int halfHeight  = height / 2;

            // make sure the specified rectangle recides with the source image
            rect.Intersect( new Rectangle( 0, 0, width, height ) );

            int startX = -halfWidth  + rect.Left;
            int startY = -halfHeight + rect.Top;
            int stopX  = width  - halfWidth  - ( width  - rect.Right );
            int stopY  = height - halfHeight - ( height - rect.Bottom );

            int offset = image.Stride - rect.Width;

            // calculate Hough map's width
            int halfHoughWidth = (int) Math.Sqrt( halfWidth * halfWidth + halfHeight * halfHeight );
            int houghWidth = halfHoughWidth * 2;

            houghMap = new short[houghHeight, houghWidth];

            // do the job
            unsafe
            {
                byte* src = (byte*) image.ImageData.ToPointer( ) +
                    rect.Top * image.Stride + rect.Left;

                // for each row
                for ( int y = startY; y < stopY; y++ )
                {
                    // for each pixel
                    for ( int x = startX; x < stopX; x++, src++ )
                    {
                        if ( *src != 0 )
                        {
                            // for each Theta value
                            for ( int theta = 0; theta < houghHeight; theta++ )
                            {
                                int radius = (int) Math.Round( cosMap[theta] * x - sinMap[theta] * y ) + halfHoughWidth;

                                if ( ( radius < 0 ) || ( radius >= houghWidth ) )
                                    continue;

                                houghMap[theta, radius]++;
                            }
                        }
                    }
                    src += offset;
                }
            }

            // find max value in Hough map
            maxMapIntensity = 0;
            for ( int i = 0; i < houghHeight; i++ )
            {
                for ( int j = 0; j < houghWidth; j++ )
                {
                    if ( houghMap[i, j] > maxMapIntensity )
                    {
                        maxMapIntensity = houghMap[i, j];
                    }
                }
            }

            CollectLines( );
        }

        /// <summary>
        /// Convert Hough map to bitmap. 
        /// </summary>
        /// 
        /// <returns>Returns 8 bppp grayscale bitmap, which shows Hough map.</returns>
        /// 
        /// <exception cref="ApplicationException">Hough transformation was not yet done by calling
        /// ProcessImage() method.</exception>
        /// 
        public Bitmap ToBitmap( )
        {
            // check if Hough transformation was made already
            if ( houghMap == null )
            {
                throw new ApplicationException( "Hough transformation was not done yet." );
            }

            int width = houghMap.GetLength( 1 );
            int height = houghMap.GetLength( 0 );

            // create new image
            Bitmap image = AForge.Imaging.Image.CreateGrayscaleImage( width, height );

            // lock destination bitmap data
            BitmapData imageData = image.LockBits(
                new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed );

            int offset = imageData.Stride - width;
            float scale = 255.0f / maxMapIntensity;

            // do the job
            unsafe
            {
                byte * dst = (byte*) imageData.Scan0.ToPointer( );

                for ( int y = 0; y < height; y++ )
                {
                    for ( int x = 0; x < width; x++, dst++ )
                    {
                        *dst = (byte) System.Math.Min( 255, (int) ( scale * houghMap[y, x] ) );
                    }
                    dst += offset;
                }
            }

            // unlock destination images
            image.UnlockBits( imageData );

            return image;
        }

        /// <summary>
        /// Get specified amount of lines with highest <see cref="HoughLine.Intensity">intensity</see>.
        /// </summary>
        /// 
        /// <param name="count">Amount of lines to get.</param>
        /// 
        /// <returns>Returns array of most intesive lines. If there are no lines detected,
        /// the returned array has zero length.</returns>
        /// 
        public HoughLine[] GetMostIntensiveLines( int count )
        {
            // lines count
            int n = Math.Min( count, lines.Count );

            // result array
            HoughLine[] dst = new HoughLine[n];
            lines.CopyTo( 0, dst, 0, n );

            return dst;
        }

        /// <summary>
        /// Get lines with <see cref="HoughLine.RelativeIntensity">relative intensity</see> higher then specified value.
        /// </summary>
        /// 
        /// <param name="minRelativeIntensity">Minimum relative intesity of lines.</param>
        /// 
        /// <returns>Returns array of lines. If there are no lines detected,
        /// the returned array has zero length.</returns>
        /// 
        public HoughLine[] GetLinesByRelativeIntensity( double minRelativeIntensity )
        {
            int count = 0, n = lines.Count;

            while ( ( count < n ) && ( ( (HoughLine) lines[count] ).RelativeIntensity >= minRelativeIntensity ) )
                count++;

            return GetMostIntensiveLines( count );
        }


        // Collect lines with intesities greater or equal then specified
        private void CollectLines( )
        {
            int		maxTheta = houghMap.GetLength( 0 );
            int		maxRadius = houghMap.GetLength( 1 );

            short	intensity;
            bool	foundGreater;

            int     halfHoughWidth = maxRadius >> 1;

            // clean lines collection
            lines.Clear( );

            // for each Theta value
            for ( int theta = 0; theta < maxTheta; theta++ )
            {
                // for each Radius value
                for ( int radius = 0; radius < maxRadius; radius++ )
                {
                    // get current value
                    intensity = houghMap[theta, radius];

                    if ( intensity < minLineIntensity )
                        continue;

                    foundGreater = false;

                    // check neighboors
                    for ( int tt = theta - localPeakRadius, ttMax = theta + localPeakRadius; tt < ttMax; tt++ )
                    {
                        // break if it is not local maximum
                        if ( foundGreater == true )
                            break;

                        int cycledTheta = tt;
                        int cycledRadius = radius;

                        // check limits
                        if ( cycledTheta < 0 )
                        {
                            cycledTheta = maxTheta + cycledTheta;
                            cycledRadius = maxRadius - cycledRadius;
                        }
                        if ( cycledTheta >= maxTheta )
                        {
                            cycledTheta -= maxTheta;
                            cycledRadius = maxRadius - cycledRadius;
                        }

                        for ( int tr = cycledRadius - localPeakRadius, trMax = cycledRadius + localPeakRadius; tr < trMax; tr++ )
                        {
                            // skip out of map values
                            if ( tr < 0 )
                                continue;
                            if ( tr >= maxRadius )
                                break;

                            // compare the neighboor with current value
                            if ( houghMap[cycledTheta, tr] > intensity )
                            {
                                foundGreater = true;
                                break;
                            }
                        }
                    }

                    // was it local maximum ?
                    if ( !foundGreater )
                    {
                        // we have local maximum
                        lines.Add( new HoughLine( (double) theta / stepsPerDegree, (short) ( radius - halfHoughWidth ), intensity, (double) intensity / maxMapIntensity ) );
                    }
                }
            }

            lines.Sort( );
        }
    }
}
