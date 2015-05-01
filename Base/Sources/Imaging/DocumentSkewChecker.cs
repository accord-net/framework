// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2010
// andrew.kirillov@aforgenet.com
//
// Alejandro Pirola, 2008
// alejamp@gmail.com
//

namespace AForge.Imaging
{
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Drawing.Imaging;
    
    /// <summary>
    /// Skew angle checker for scanned documents.
    /// </summary>
    ///
    /// <remarks><para>The class implements document's skew checking algorithm, which is based
    /// on <see cref="HoughLineTransformation">Hough line transformation</see>. The algorithm
    /// is based on searching for text base lines - black line of text bottoms' followed
    /// by white line below.</para>
    /// 
    /// <para><note>The routine supposes that a white-background document is provided
    /// with black letters. The algorithm is not supposed for any type of objects, but for
    /// document images with text.</note></para>
    /// 
    /// <para>The range of angles to detect is controlled by <see cref="MaxSkewToDetect"/> property.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create instance of skew checker
    /// DocumentSkewChecker skewChecker = new DocumentSkewChecker( );
    /// // get documents skew angle
    /// double angle = skewChecker.GetSkewAngle( documentImage );
    /// // create rotation filter
    /// RotateBilinear rotationFilter = new RotateBilinear( -angle );
    /// rotationFilter.FillColor = Color.White;
    /// // rotate image applying the filter
    /// Bitmap rotatedImage = rotationFilter.Apply( documentImage );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample10.png" width="300" height="184" />
    /// <para><b>Deskewed image:</b></para>
    /// <img src="img/imaging/deskew.png" width="335" height="250" /> 
    /// </remarks>
    /// 
    /// <seealso cref="HoughLineTransformation"/>
    ///
    public class DocumentSkewChecker
    {
        // Hough transformation: quality settings
        private int     stepsPerDegree;
        private int     houghHeight;
        private double  thetaStep;
        private double  maxSkewToDetect;

        // Hough transformation: precalculated Sine and Cosine values
        private double[]	sinMap;
        private double[]	cosMap;
        private bool        needToInitialize = true;

        // Hough transformation: Hough map
        private short[,]	houghMap;
        private short		maxMapIntensity = 0;

        private int 		localPeakRadius = 4;
        private ArrayList   lines = new ArrayList( );

        /// <summary>
        /// Steps per degree, [1, 10].
        /// </summary>
        /// 
        /// <remarks><para>The value defines quality of Hough transform and its ability to detect
        /// line slope precisely.</para>
        /// 
        /// <para>Default value is set to <b>1</b>.</para>
        /// </remarks>
        /// 
        public int StepsPerDegree
        {
            get { return stepsPerDegree; }
            set
            {
                stepsPerDegree = Math.Max( 1, Math.Min( 10, value ) );
                needToInitialize = true;
            }
        }

        /// <summary>
        /// Maximum skew angle to detect, [0, 45] degrees.
        /// </summary>
        /// 
        /// <remarks><para>The value sets maximum document's skew angle to detect.
        /// Document's skew angle can be as positive (rotated counter clockwise), as negative
        /// (rotated clockwise). So setting this value to 25, for example, will lead to
        /// [-25, 25] degrees detection range.</para>
        ///
        /// <para>Scanned documents usually have skew in the [-20, 20] degrees range.</para>
        /// 
        /// <para>Default value is set to <b>30</b>.</para>
        /// </remarks>
        /// 
        public double MaxSkewToDetect
        {
            get { return maxSkewToDetect; }
            set
            {
                maxSkewToDetect = Math.Max( 0, Math.Min( 45, value ) );
                needToInitialize = true;
            }
        }

        /// <summary>
        /// Minimum angle to detect skew in degrees.
        /// </summary>
        ///
        /// <remarks><para><note>The property is deprecated and setting it has not any effect.
        /// Use <see cref="MaxSkewToDetect"/> property instead.</note></para></remarks>
        ///
        [Obsolete( "The property is deprecated and setting it has not any effect. Use MaxSkewToDetect property instead." )]
        public double MinBeta
        {
            get { return ( -maxSkewToDetect ); }
            set { }
        }

        /// <summary>
        /// Maximum angle to detect skew in degrees.
        /// </summary>
        ///
        /// <remarks><para><note>The property is deprecated and setting it has not any effect.
        /// Use <see cref="MaxSkewToDetect"/> property instead.</note></para></remarks>
        ///
        [Obsolete( "The property is deprecated and setting it has not any effect. Use MaxSkewToDetect property instead." )]
        public double MaxBeta
        {
            get { return ( maxSkewToDetect ); }
            set { }
        }

        /// <summary>
        /// Radius for searching local peak value, [1, 10].
        /// </summary>
        /// 
        /// <remarks><para>The value determines radius around a map's value, which is analyzed to determine
        /// if the map's value is a local maximum in specified area.</para>
        /// 
        /// <para>Default value is set to <b>4</b>.</para></remarks>
        /// 
        public int LocalPeakRadius
        {
            get { return localPeakRadius; }
            set { localPeakRadius = Math.Max( 1, Math.Min( 10, value ) ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentSkewChecker"/> class.
        /// </summary>
        public DocumentSkewChecker( )
        {
            StepsPerDegree = 10;
            MaxSkewToDetect = 30;
        }

        /// <summary>
        /// Get skew angle of the provided document image.
        /// </summary>
        /// 
        /// <param name="image">Document's image to get skew angle of.</param>
        /// 
        /// <returns>Returns document's skew angle. If the returned angle equals to -90,
        /// then document skew detection has failed.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// 
        public double GetSkewAngle( Bitmap image )
        {
            return GetSkewAngle( image, new Rectangle( 0, 0, image.Width, image.Height ) );
        }

        /// <summary>
        /// Get skew angle of the provided document image.
        /// </summary>
        /// 
        /// <param name="image">Document's image to get skew angle of.</param>
        /// <param name="rect">Image's rectangle to process (used to exclude processing of
        /// regions, which are not relevant to skew detection).</param>
        /// 
        /// <returns>Returns document's skew angle. If the returned angle equals to -90,
        /// then document skew detection has failed.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// 
        public double GetSkewAngle( Bitmap image, Rectangle rect )
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

            double skewAngle;

            try
            {
                // process the image
                skewAngle = GetSkewAngle( new UnmanagedImage( imageData ), rect );
            }
            finally
            {
                // unlock image
                image.UnlockBits( imageData );
            }

            return skewAngle;
        }

        /// <summary>
        /// Get skew angle of the provided document image.
        /// </summary>
        /// 
        /// <param name="imageData">Document's image data to get skew angle of.</param>
        /// 
        /// <returns>Returns document's skew angle. If the returned angle equals to -90,
        /// then document skew detection has failed.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// 
        public double GetSkewAngle( BitmapData imageData )
        {
            return GetSkewAngle( new UnmanagedImage( imageData ),
                new Rectangle( 0, 0, imageData.Width, imageData.Height ) );
        }

        /// <summary>
        /// Get skew angle of the provided document image.
        /// </summary>
        /// 
        /// <param name="imageData">Document's image data to get skew angle of.</param>
        /// <param name="rect">Image's rectangle to process (used to exclude processing of
        /// regions, which are not relevant to skew detection).</param>
        /// 
        /// <returns>Returns document's skew angle. If the returned angle equals to -90,
        /// then document skew detection has failed.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// 
        public double GetSkewAngle( BitmapData imageData, Rectangle rect )
        {
            return GetSkewAngle( new UnmanagedImage( imageData ), rect );
        }

        /// <summary>
        /// Get skew angle of the provided document image.
        /// </summary>
        /// 
        /// <param name="image">Document's unmanaged image to get skew angle of.</param>
        /// 
        /// <returns>Returns document's skew angle. If the returned angle equals to -90,
        /// then document skew detection has failed.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// 
        public double GetSkewAngle( UnmanagedImage image )
        {
            return GetSkewAngle( image, new Rectangle( 0, 0, image.Width, image.Height ) );
        }

        /// <summary>
        /// Get skew angle of the provided document image.
        /// </summary>
        /// 
        /// <param name="image">Document's unmanaged image to get skew angle of.</param>
        /// <param name="rect">Image's rectangle to process (used to exclude processing of
        /// regions, which are not relevant to skew detection).</param>
        /// 
        /// <returns>Returns document's skew angle. If the returned angle equals to -90,
        /// then document skew detection has failed.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// 
        public double GetSkewAngle( UnmanagedImage image, Rectangle rect )
        {
            if ( image.PixelFormat != PixelFormat.Format8bppIndexed )
            {
                throw new UnsupportedImageFormatException( "Unsupported pixel format of the source image." );
            }

            // init hough transformation settings
            InitHoughMap( );

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
            int stopY  = height - halfHeight - ( height - rect.Bottom ) - 1;

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
                byte* srcBelow = src + image.Stride;

                // for each row
                for ( int y = startY; y < stopY; y++ )
                {
                    // for each pixel
                    for ( int x = startX; x < stopX; x++, src++, srcBelow++ )
                    {
                        // if current pixel is more black
                        // and pixel below is more white
                        if ( ( *src < 128 ) && ( *srcBelow >= 128 ) )
                        {
                            // for each Theta value
                            for ( int theta = 0; theta < houghHeight; theta++ )
                            {
                                int radius = (int) ( cosMap[theta] * x - sinMap[theta] * y ) + halfHoughWidth;

                                if ( ( radius < 0 ) || ( radius >= houghWidth ) )
                                    continue;

                                houghMap[theta, radius]++;
                            }
                        }
                    }
                    src += offset;
                    srcBelow += offset;
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

            CollectLines( (short) ( width / 10 ) );

            // get skew angle
            HoughLine[] hls = this.GetMostIntensiveLines( 5 );

            double skewAngle = 0;
            double sumIntensity = 0;

            foreach ( HoughLine hl in hls )
            {
                if ( hl.RelativeIntensity > 0.5 )
                {
                    skewAngle += ( hl.Theta * hl.RelativeIntensity );
                    sumIntensity += hl.RelativeIntensity;
                }
            }
            if ( hls.Length > 0 ) skewAngle = skewAngle / sumIntensity;

            return skewAngle - 90.0;
        }

        // Get specified amount of lines with highest intensity
        private HoughLine[] GetMostIntensiveLines( int count )
        {
            // lines count
            int n = Math.Min( count, lines.Count );

            // result array
            HoughLine[] dst = new HoughLine[n];
            lines.CopyTo( 0, dst, 0, n );

            return dst;
        }

        // Collect lines with intesities greater or equal then specified
        private void CollectLines( short minLineIntensity )
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
                        // skip out of map values
                        if ( tt < 0 )
                            continue;
                        if ( tt >= maxTheta )
                            break;

                        // break if it is not local maximum
                        if ( foundGreater == true )
                            break;

                        for ( int tr = radius - localPeakRadius, trMax = radius + localPeakRadius; tr < trMax; tr++ )
                        {
                            // skip out of map values
                            if ( tr < 0 )
                                continue;
                            if ( tr >= maxRadius )
                                break;

                            // compare the neighboor with current value
                            if ( houghMap[tt, tr] > intensity )
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
                        lines.Add( new HoughLine( 90.0 - maxSkewToDetect + (double) theta / stepsPerDegree, (short) ( radius - halfHoughWidth ), intensity, (double) intensity / maxMapIntensity ) );
                    }
                }
            }

            lines.Sort( );
        }

        // Init Hough settings and map
        private void InitHoughMap( )
        {
            if ( needToInitialize )
            {
                needToInitialize = false;

                houghHeight = (int) ( 2 * maxSkewToDetect * stepsPerDegree );
                thetaStep = ( 2 * maxSkewToDetect * Math.PI / 180 ) / houghHeight;

                // precalculate Sine and Cosine values
                sinMap = new double[houghHeight];
                cosMap = new double[houghHeight];

                double minTheta = 90.0 - maxSkewToDetect;

                for ( int i = 0; i < houghHeight; i++ )
                {
                    sinMap[i] = Math.Sin( ( minTheta * Math.PI / 180 ) + ( i * thetaStep ) );
                    cosMap[i] = Math.Cos( ( minTheta * Math.PI / 180 ) + ( i * thetaStep ) );
                }
            }
        }
    }
}
