// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@aforgenet.com
//
// Article by Bill Green was used as the reference
// http://www.pages.drexel.edu/~weg22/can_tut.html
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Canny edge detector.
    /// </summary>
    /// 
    /// <remarks><para>The filter searches for objects' edges by applying Canny edge detector.
    /// The implementation follows
    /// <a href="http://www.pages.drexel.edu/~weg22/can_tut.html">Bill Green's Canny edge detection tutorial</a>.</para>
    /// 
    /// <para><note>The implemented canny edge detector has one difference with the above linked algorithm.
    /// The difference is in hysteresis step, which is a bit simplified (getting faster as a result). On the
    /// hysteresis step each pixel is compared with two threshold values: <see cref="HighThreshold"/> and
    /// <see cref="LowThreshold"/>. If pixel's value is greater or equal to <see cref="HighThreshold"/>, then
    /// it is kept as edge pixel. If pixel's value is greater or equal to <see cref="LowThreshold"/>, then
    /// it is kept as edge pixel only if there is at least one neighbouring pixel (8 neighbours are checked) which
    /// has value greater or equal to <see cref="HighThreshold"/>; otherwise it is none edge pixel. In the case
    /// if pixel's value is less than <see cref="LowThreshold"/>, then it is marked as none edge immediately.
    /// </note></para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// CannyEdgeDetector filter = new CannyEdgeDetector( );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample2.jpg" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/canny_edges.png" width="320" height="240" />
    /// </remarks>
    /// 
    public class CannyEdgeDetector : BaseUsingCopyPartialFilter
    {
        private GaussianBlur gaussianFilter = new GaussianBlur( );
        private byte lowThreshold = 20;
        private byte highThreshold = 100;

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
        /// Low threshold.
        /// </summary>
        /// 
        /// <remarks><para>Low threshold value used for hysteresis
        /// (see  <a href="http://www.pages.drexel.edu/~weg22/can_tut.html">tutorial</a>
        /// for more information).</para>
        /// 
        /// <para>Default value is set to <b>20</b>.</para>
        /// </remarks>
        /// 
        public byte LowThreshold
        {
            get { return lowThreshold; }
            set { lowThreshold = value; }
        }

        /// <summary>
        /// High threshold.
        /// </summary>
        /// 
        /// <remarks><para>High threshold value used for hysteresis
        /// (see  <a href="http://www.pages.drexel.edu/~weg22/can_tut.html">tutorial</a>
        /// for more information).</para>
        /// 
        /// <para>Default value is set to <b>100</b>.</para>
        /// </remarks>
        /// 
        public byte HighThreshold
        {
            get { return highThreshold; }
            set { highThreshold = value; }
        }

        /// <summary>
        /// Gaussian sigma.
        /// </summary>
        /// 
        /// <remarks>Sigma value for <see cref="GaussianBlur.Sigma">Gaussian bluring</see>.</remarks>
        /// 
        public double GaussianSigma
        {
            get { return gaussianFilter.Sigma; }
            set { gaussianFilter.Sigma = value; }
        }

        /// <summary>
        /// Gaussian size.
        /// </summary>
        /// 
        /// <remarks>Size of <see cref="GaussianBlur.Size">Gaussian kernel</see>.</remarks>
        /// 
        public int GaussianSize
        {
            get { return gaussianFilter.Size; }
            set { gaussianFilter.Size = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannyEdgeDetector"/> class.
        /// </summary>
        /// 
        public CannyEdgeDetector( )
        {
            // initialize format translation dictionary
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannyEdgeDetector"/> class.
        /// </summary>
        /// 
        /// <param name="lowThreshold">Low threshold.</param>
        /// <param name="highThreshold">High threshold.</param>
        /// 
        public CannyEdgeDetector( byte lowThreshold, byte highThreshold ) : this( )
        {
            this.lowThreshold  = lowThreshold;
            this.highThreshold = highThreshold;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannyEdgeDetector"/> class.
        /// </summary>
        /// 
        /// <param name="lowThreshold">Low threshold.</param>
        /// <param name="highThreshold">High threshold.</param>
        /// <param name="sigma">Gaussian sigma.</param>
        /// 
        public CannyEdgeDetector( byte lowThreshold, byte highThreshold, double sigma )
            : this( )
        {
            this.lowThreshold    = lowThreshold;
            this.highThreshold   = highThreshold;
            gaussianFilter.Sigma = sigma;
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
            // processing start and stop X,Y positions
            int startX  = rect.Left + 1;
            int startY  = rect.Top + 1;
            int stopX   = startX + rect.Width - 2;
            int stopY   = startY + rect.Height - 2;

            int width  = rect.Width - 2;
            int height = rect.Height - 2;

            int dstStride = destination.Stride;
            int srcStride = source.Stride;

            int dstOffset = dstStride - rect.Width + 2;
            int srcOffset = srcStride - rect.Width + 2;

            // pixel's value and gradients
            int gx, gy;
            //
            double orientation, toAngle = 180.0 / System.Math.PI;
            float leftPixel = 0, rightPixel = 0;

            // STEP 1 - blur image
            UnmanagedImage blurredImage = gaussianFilter.Apply( source );

            // orientation array
            byte[] orients = new byte[width * height];
            // gradients array
            float[,] gradients = new float[source.Width, source.Height];
            float maxGradient = float.NegativeInfinity;

            // do the job
            byte* src = (byte*) blurredImage.ImageData.ToPointer( );
            // allign pointer
            src += srcStride * startY + startX;

            // STEP 2 - calculate magnitude and edge orientation
            int p = 0;

            // for each line
            for ( int y = startY; y < stopY; y++ )
            {
                // for each pixel
                for ( int x = startX; x < stopX; x++, src++, p++ )
                {
                    gx = src[-srcStride + 1] + src[srcStride + 1]
                       - src[-srcStride - 1] - src[srcStride - 1]
                       + 2 * ( src[1] - src[-1] );

                    gy = src[-srcStride - 1] + src[-srcStride + 1]
                       - src[srcStride - 1] - src[srcStride + 1]
                       + 2 * ( src[-srcStride] - src[srcStride] );

                    // get gradient value
                    gradients[x, y] = (float) Math.Sqrt( gx * gx + gy * gy );
                    if ( gradients[x, y] > maxGradient )
                        maxGradient = gradients[x, y];

                    // --- get orientation
                    if ( gx == 0 )
                    {
                        // can not divide by zero
                        orientation = ( gy == 0 ) ? 0 : 90;
                    }
                    else
                    {
                        double div = (double) gy / gx;

                        // handle angles of the 2nd and 4th quads
                        if ( div < 0 )
                        {
                            orientation = 180 - System.Math.Atan( -div ) * toAngle;
                        }
                        // handle angles of the 1st and 3rd quads
                        else
                        {
                            orientation = System.Math.Atan( div ) * toAngle;
                        }

                        // get closest angle from 0, 45, 90, 135 set
                        if ( orientation < 22.5 )
                            orientation = 0;
                        else if ( orientation < 67.5 )
                            orientation = 45;
                        else if ( orientation < 112.5 )
                            orientation = 90;
                        else if ( orientation < 157.5 )
                            orientation = 135;
                        else orientation = 0;
                    }

                    // save orientation
                    orients[p] = (byte) orientation;
                }
                src += srcOffset;
            }

            // STEP 3 - suppres non maximums
            byte* dst = (byte*) destination.ImageData.ToPointer( );
            // allign pointer
            dst += dstStride * startY + startX;

            p = 0;

            // for each line
            for ( int y = startY; y < stopY; y++ )
            {
                // for each pixel
                for ( int x = startX; x < stopX; x++, dst++, p++ )
                {
                    // get two adjacent pixels
                    switch ( orients[p] )
                    {
                        case 0:
                            leftPixel  = gradients[x - 1, y];
                            rightPixel = gradients[x + 1, y];
                            break;
                        case 45:
                            leftPixel  = gradients[x - 1, y + 1];
                            rightPixel = gradients[x + 1, y - 1];
                            break;
                        case 90:
                            leftPixel  = gradients[x, y + 1];
                            rightPixel = gradients[x, y - 1];
                            break;
                        case 135:
                            leftPixel  = gradients[x + 1, y + 1];
                            rightPixel = gradients[x - 1, y - 1];
                            break;
                    }
                    // compare current pixels value with adjacent pixels
                    if ( ( gradients[x, y] < leftPixel ) || ( gradients[x, y] < rightPixel ) )
                    {
                        *dst = 0;
                    }
                    else
                    {
                        *dst = (byte) ( gradients[x, y] / maxGradient * 255 );
                    }
                }
                dst += dstOffset;
            }

            // STEP 4 - hysteresis
            dst = (byte*) destination.ImageData.ToPointer( );
            // allign pointer
            dst += dstStride * startY + startX;

            // for each line
            for ( int y = startY; y < stopY; y++ )
            {
                // for each pixel
                for ( int x = startX; x < stopX; x++, dst++ )
                {
                    if ( *dst < highThreshold )
                    {
                        if ( *dst < lowThreshold )
                        {
                            // non edge
                            *dst = 0;
                        }
                        else
                        {
                            // check 8 neighboring pixels
                            if ( ( dst[-1] < highThreshold ) &&
                                ( dst[1] < highThreshold ) &&
                                ( dst[-dstStride - 1] < highThreshold ) &&
                                ( dst[-dstStride] < highThreshold ) &&
                                ( dst[-dstStride + 1] < highThreshold ) &&
                                ( dst[dstStride - 1] < highThreshold ) &&
                                ( dst[dstStride] < highThreshold ) &&
                                ( dst[dstStride + 1] < highThreshold ) )
                            {
                                *dst = 0;
                            }
                        }
                    }
                }
                dst += dstOffset;
            }

            // STEP 5 - draw black rectangle to remove those pixels, which were not processed
            // (this needs to be done for those cases, when filter is applied "in place" -
            //  source image is modified instead of creating new copy)
            Drawing.Rectangle( destination, rect, Color.Black );

            // release blurred image
            blurredImage.Dispose( );
        }
    }
}
