// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2007-2010
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    using AForge;

    /// <summary>
    /// Performs quadrilateral transformation of an area in the source image.
    /// </summary>
    /// 
    /// <remarks><para>The class implements simple algorithm described by
    /// <a href="http://www.codeguru.com/forum/showpost.php?p=1186454&amp;postcount=2">Olivier Thill</a>
    /// for transforming quadrilateral area from a source image into rectangular image.
    /// The idea of the algorithm is based on finding for each line of destination
    /// rectangular image a corresponding line connecting "left" and "right" sides of
    /// quadrilateral in a source image. Then the line is linearly transformed into the
    /// line in destination image.</para>
    /// 
    /// <para><note>Due to simplicity of the algorithm it does not do any correction for perspective.
    /// </note></para>
    /// 
    /// <para><note>To make sure the algorithm works correctly, it is preferred if the
    /// "left-top" corner of the quadrilateral (screen coordinates system) is
    /// specified first in the list of quadrilateral's corners. At least
    /// user need to make sure that the "left" side (side connecting first and the last
    /// corner) and the "right" side (side connecting second and third corners) are
    /// not horizontal.</note></para>
    /// 
    /// <para>Use <see cref="QuadrilateralTransformation"/> to avoid the above mentioned limitations,
    /// which is a more advanced quadrilateral transformation algorithms (although a bit more
    /// computationally expensive).</para>
    /// 
    /// <para>The image processing filter accepts 8 grayscale images and 24/32 bpp
    /// color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // define quadrilateral's corners
    /// List&lt;IntPoint&gt; corners = new List&lt;IntPoint&gt;( );
    /// corners.Add( new IntPoint(  99,  99 ) );
    /// corners.Add( new IntPoint( 156,  79 ) );
    /// corners.Add( new IntPoint( 184, 126 ) );
    /// corners.Add( new IntPoint( 122, 150 ) );
    /// // create filter
    /// SimpleQuadrilateralTransformation filter =
    ///     new SimpleQuadrilateralTransformation( corners, 200, 200 );
    /// // apply the filter
    /// Bitmap newImage = filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample18.jpg" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/quadrilateral_bilinear.png" width="200" height="200" />
    /// </remarks>
    /// 
    /// <seealso cref="QuadrilateralTransformation"/>
    ///
    public class SimpleQuadrilateralTransformation : BaseTransformationFilter
    {
        private bool automaticSizeCalculaton = true;
        private bool useInterpolation = true;

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
        /// New image width.
        /// </summary>
        protected int newWidth;

        /// <summary>
        /// New image height.
        /// </summary>
        protected int newHeight;

        /// <summary>
        /// Automatic calculation of destination image or not.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies how to calculate size of destination (transformed)
        /// image. If the property is set to <see langword="false"/>, then <see cref="NewWidth"/>
        /// and <see cref="NewHeight"/> properties have effect and destination image's size is
        /// specified by user. If the property is set to <see langword="true"/>, then setting the above
        /// mentioned properties does not have any effect, but destionation image's size is
        /// automatically calculated from <see cref="SourceQuadrilateral"/> property - width and height
        /// come from length of longest edges.
        /// </para>
        /// 
        /// <para>Default value is set to <see langword="true"/>.</para>
        /// </remarks>
        /// 
        public bool AutomaticSizeCalculaton
        {
            get { return automaticSizeCalculaton; }
            set
            {
                automaticSizeCalculaton = value;
                if ( value )
                {
                    CalculateDestinationSize( );
                }
            }
        }

        // Quadrilateral's corners in source image.
        private List<IntPoint> sourceQuadrilateral;

        /// <summary>
        /// Quadrilateral's corners in source image.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies four corners of the quadrilateral area
        /// in the source image to be transformed.</para>
        /// 
        /// <para>See documentation to the <see cref="SimpleQuadrilateralTransformation"/>
        /// class itself for additional information.</para>
        /// </remarks>
        /// 
        public List<IntPoint> SourceQuadrilateral
        {
            get { return sourceQuadrilateral; }
            set
            {
                sourceQuadrilateral = value;
                if ( automaticSizeCalculaton )
                {
                    CalculateDestinationSize( );
                }
            }
        }

        /// <summary>
        /// Width of the new transformed image.
        /// </summary>
        /// 
        /// <remarks><para>The property defines width of the destination image, which gets
        /// transformed quadrilateral image.</para>
        /// 
        /// <para><note>Setting the property does not have any effect, if <see cref="AutomaticSizeCalculaton"/>
        /// property is set to <see langword="true"/>. In this case destination image's width
        /// is calculated automatically based on <see cref="SourceQuadrilateral"/> property.</note></para>
        /// </remarks>
        /// 
        public int NewWidth
        {
            get { return newWidth; }
            set
            {
                if ( !automaticSizeCalculaton )
                {
                    newWidth = Math.Max( 1, value );
                }
            }
        }

        /// <summary>
        /// Height of the new transformed image.
        /// </summary>
        /// 
        /// <remarks><para>The property defines height of the destination image, which gets
        /// transformed quadrilateral image.</para>
        /// 
        /// <para><note>Setting the property does not have any effect, if <see cref="AutomaticSizeCalculaton"/>
        /// property is set to <see langword="true"/>. In this case destination image's height
        /// is calculated automatically based on <see cref="SourceQuadrilateral"/> property.</note></para>
        /// </remarks>
        /// 
        public int NewHeight
        {
            get { return newHeight; }
            set
            {
                if ( !automaticSizeCalculaton )
                {
                    newHeight = Math.Max( 1, value );
                }
            }
        }

        /// <summary>
        /// Specifies if bilinear interpolation should be used or not.
        /// </summary>
        /// 
        /// <remarks><para>Default value is set to <see langword="true"/> - interpolation
        /// is used.</para>
        /// </remarks>
        /// 
        public bool UseInterpolation
        {
            get { return useInterpolation; }
            set { useInterpolation = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleQuadrilateralTransformation"/> class.
        /// </summary>
        /// 
        public SimpleQuadrilateralTransformation( )
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]    = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb]   = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format32bppPArgb]  = PixelFormat.Format32bppPArgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleQuadrilateralTransformation"/> class.
        /// </summary>
        /// 
        /// <param name="sourceQuadrilateral">Corners of the source quadrilateral area.</param>
        /// <param name="newWidth">Width of the new transformed image.</param>
        /// <param name="newHeight">Height of the new transformed image.</param>
        /// 
        /// <remarks><para>This constructor sets <see cref="AutomaticSizeCalculaton"/> to
        /// <see langword="false"/>, which means that destination image will have width and
        /// height as specified by user.</para></remarks>
        /// 
        public SimpleQuadrilateralTransformation( List<IntPoint> sourceQuadrilateral, int newWidth, int newHeight )
            : this( )
        {
            this.automaticSizeCalculaton = false;
            this.sourceQuadrilateral = sourceQuadrilateral;
            this.newWidth  = newWidth;
            this.newHeight = newHeight;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleQuadrilateralTransformation"/> class.
        /// </summary>
        /// 
        /// <param name="sourceQuadrilateral">Corners of the source quadrilateral area.</param>
        /// 
        /// <remarks><para>This constructor sets <see cref="AutomaticSizeCalculaton"/> to
        /// <see langword="true"/>, which means that destination image will have width and
        /// height automatically calculated based on <see cref="SourceQuadrilateral"/> property.</para></remarks>
        ///
        public SimpleQuadrilateralTransformation( List<IntPoint> sourceQuadrilateral )
            : this( )
        {
            this.automaticSizeCalculaton = true;
            this.sourceQuadrilateral = sourceQuadrilateral;
            CalculateDestinationSize( );
        }

        /// <summary>
        /// Calculates new image size.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// 
        /// <returns>New image size - size of the destination image.</returns>
        /// 
        /// <exception cref="NullReferenceException">Source quadrilateral was not set.</exception>
        /// 
        protected override System.Drawing.Size CalculateNewImageSize( UnmanagedImage sourceData )
        {
            if ( sourceQuadrilateral == null )
                throw new NullReferenceException( "Source quadrilateral was not set." );

            return new Size( newWidth, newHeight );
        }

        // Calculates size of destination image
        private void CalculateDestinationSize( )
        {
            if ( sourceQuadrilateral == null )
                throw new NullReferenceException( "Source quadrilateral was not set." );

            newWidth  = (int) Math.Max( sourceQuadrilateral[0].DistanceTo( sourceQuadrilateral[1] ),
                                        sourceQuadrilateral[2].DistanceTo( sourceQuadrilateral[3] ) );
            newHeight = (int) Math.Max( sourceQuadrilateral[1].DistanceTo( sourceQuadrilateral[2] ),
                                        sourceQuadrilateral[3].DistanceTo( sourceQuadrilateral[0] ) );
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// 
        protected override unsafe void ProcessFilter( UnmanagedImage sourceData, UnmanagedImage destinationData )
        {
            // get source and destination images size
            int srcWidth  = sourceData.Width;
            int srcHeight = sourceData.Height;
            int dstWidth  = destinationData.Width;
            int dstHeight = destinationData.Height;

            int pixelSize = Image.GetPixelFormatSize( sourceData.PixelFormat ) / 8;
            int srcStride = sourceData.Stride;
            int dstStride = destinationData.Stride;

            // find equations of four quadrilateral's edges ( f(x) = k*x + b )
            double kTop,    bTop;
            double kBottom, bBottom;
            double kLeft,   bLeft;
            double kRight,  bRight;

            // top edge
            if ( sourceQuadrilateral[1].X == sourceQuadrilateral[0].X )
            {
                kTop = 0;
                bTop = sourceQuadrilateral[1].X;
            }
            else
            {
                kTop = (double) ( sourceQuadrilateral[1].Y - sourceQuadrilateral[0].Y ) /
                                ( sourceQuadrilateral[1].X - sourceQuadrilateral[0].X );
                bTop = (double) sourceQuadrilateral[0].Y - kTop * sourceQuadrilateral[0].X;
            }

            // bottom edge
            if ( sourceQuadrilateral[2].X == sourceQuadrilateral[3].X )
            {
                kBottom = 0;
                bBottom = sourceQuadrilateral[2].X;
            }
            else
            {
                kBottom = (double) ( sourceQuadrilateral[2].Y - sourceQuadrilateral[3].Y ) /
                                   ( sourceQuadrilateral[2].X - sourceQuadrilateral[3].X );
                bBottom = (double) sourceQuadrilateral[3].Y - kBottom * sourceQuadrilateral[3].X;
            }

            // left edge
            if ( sourceQuadrilateral[3].X == sourceQuadrilateral[0].X )
            {
                kLeft = 0;
                bLeft = sourceQuadrilateral[3].X;
            }
            else
            {
                kLeft = (double) ( sourceQuadrilateral[3].Y - sourceQuadrilateral[0].Y ) /
                                 ( sourceQuadrilateral[3].X - sourceQuadrilateral[0].X );
                bLeft = (double) sourceQuadrilateral[0].Y - kLeft * sourceQuadrilateral[0].X;
            }

            // right edge
            if ( sourceQuadrilateral[2].X == sourceQuadrilateral[1].X )
            {
                kRight = 0;
                bRight = sourceQuadrilateral[2].X;
            }
            else
            {
                kRight = (double) ( sourceQuadrilateral[2].Y - sourceQuadrilateral[1].Y ) /
                                  ( sourceQuadrilateral[2].X - sourceQuadrilateral[1].X );
                bRight = (double) sourceQuadrilateral[1].Y - kRight * sourceQuadrilateral[1].X;
            }

            // some precalculated values
            double leftFactor  = (double) ( sourceQuadrilateral[3].Y - sourceQuadrilateral[0].Y ) / dstHeight;
            double rightFactor = (double) ( sourceQuadrilateral[2].Y - sourceQuadrilateral[1].Y ) / dstHeight;

            int srcY0 = sourceQuadrilateral[0].Y;
            int srcY1 = sourceQuadrilateral[1].Y;

            // do the job
            byte* baseSrc = (byte*) sourceData.ImageData.ToPointer( );
            byte* baseDst = (byte*) destinationData.ImageData.ToPointer( );

            // source width and height decreased by 1
            int ymax = srcHeight - 1;
            int xmax = srcWidth - 1;

            // coordinates of source points
            double  dx1, dy1, dx2, dy2;
            int     sx1, sy1, sx2, sy2;

            // temporary pointers
            byte* p1, p2, p3, p4, p;

            // for each line
            for ( int y = 0; y < dstHeight; y++ )
            {
                byte* dst = baseDst + dstStride * y;

                // find corresponding Y on the left edge of the quadrilateral
                double yHorizLeft = leftFactor * y + srcY0;
                // find corresponding X on the left edge of the quadrilateral
                double xHorizLeft = ( kLeft == 0 ) ? bLeft : ( yHorizLeft - bLeft ) / kLeft;

                // find corresponding Y on the right edge of the quadrilateral
                double yHorizRight = rightFactor * y + srcY1;
                // find corresponding X on the left edge of the quadrilateral
                double xHorizRight = ( kRight == 0 ) ? bRight : ( yHorizRight - bRight ) / kRight;

                // find equation of the line joining points on the left and right edges
                double kHoriz, bHoriz;

                if ( xHorizLeft == xHorizRight )
                {
                    kHoriz = 0;
                    bHoriz = xHorizRight;
                }
                else
                {
                    kHoriz = ( yHorizRight - yHorizLeft ) / ( xHorizRight - xHorizLeft );
                    bHoriz = yHorizLeft - kHoriz * xHorizLeft;
                }

                double horizFactor = ( xHorizRight - xHorizLeft ) / dstWidth;

                if ( !useInterpolation )
                {
                    for ( int x = 0; x < dstWidth; x++ )
                    {
                        double xs = horizFactor * x + xHorizLeft;
                        double ys = kHoriz * xs + bHoriz;

                        if ( ( xs >= 0 ) && ( ys >= 0 ) && ( xs < srcWidth ) && ( ys < srcHeight ) )
                        {
                            // get pointer to the pixel in the source image
                            p = baseSrc + ( (int) ys * srcStride + (int) xs * pixelSize );
                            // copy pixel's values
                            for ( int i = 0; i < pixelSize; i++, dst++, p++ )
                            {
                                *dst = *p;
                            }
                        }
                        else
                        {
                            dst += pixelSize;
                        }
                    }
                }
                else
                {
                    for ( int x = 0; x < dstWidth; x++ )
                    {
                        double xs = horizFactor * x + xHorizLeft;
                        double ys = kHoriz * xs + bHoriz;

                        if ( ( xs >= 0 ) && ( ys >= 0 ) && ( xs < srcWidth ) && ( ys < srcHeight ) )
                        {
                            sx1 = (int) xs;
                            sx2 = ( sx1 == xmax ) ? sx1 : sx1 + 1;
                            dx1 = xs - sx1;
                            dx2 = 1.0 - dx1;

                            sy1 = (int) ys;
                            sy2 = ( sy1 == ymax ) ? sy1 : sy1 + 1;
                            dy1 = ys - sy1;
                            dy2 = 1.0 - dy1;

                            // get four points
                            p1 = p2 = baseSrc + sy1 * srcStride;
                            p1 += sx1 * pixelSize;
                            p2 += sx2 * pixelSize;

                            p3 = p4 = baseSrc + sy2 * srcStride;
                            p3 += sx1 * pixelSize;
                            p4 += sx2 * pixelSize;

                            // interpolate using 4 points
                            for ( int i = 0; i < pixelSize; i++, dst++, p1++, p2++, p3++, p4++ )
                            {
                                *dst = (byte) (
                                    dy2 * ( dx2 * ( *p1 ) + dx1 * ( *p2 ) ) +
                                    dy1 * ( dx2 * ( *p3 ) + dx1 * ( *p4 ) ) );
                            }
                        }
                        else
                        {
                            dst += pixelSize;
                        }
                    }
                }
            }
        }
    }
}
