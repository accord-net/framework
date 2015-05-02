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
    using AForge.Math.Geometry;

    /// <summary>
    /// Performs backward quadrilateral transformation into an area in destination image.
    /// </summary>
    /// 
    /// <remarks><para>The class implements backward quadrilateral transformation algorithm,
    /// which allows to transform any rectangular image into any quadrilateral area
    /// in a given destination image. The idea of the algorithm is based on homogeneous
    /// transformation and its math is described by Paul Heckbert in his
    /// "<a href="http://graphics.cs.cmu.edu/courses/15-463/2008_fall/Papers/proj.pdf">Projective Mappings for Image Warping</a>" paper.
    /// </para>
    /// 
    /// <para>The image processing routines implements similar math to <see cref="QuadrilateralTransformation"/>,
    /// but performs it in backward direction.</para>
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
    /// BackwardQuadrilateralTransformation filter =
    ///     new BackwardQuadrilateralTransformation( sourceImage, corners );
    /// // apply the filter
    /// Bitmap newImage = filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Source image:</b></para>
    /// <img src="img/imaging/icon.png" width="128" height="128" />
    /// <para><b>Destination image:</b></para>
    /// <img src="img/imaging/sample18.jpg" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/backward_quadrilateral.jpg" width="320" height="240" />
    /// </remarks>
    /// 
    /// <seealso cref="QuadrilateralTransformation"/>
    /// 
    public class BackwardQuadrilateralTransformation : BaseInPlaceFilter
    {
        private Bitmap sourceImage = null;
        private UnmanagedImage sourceUnmanagedImage = null;
        private List<IntPoint> destinationQuadrilateral = null;

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
        /// Source image to be transformed into specified quadrilateral.
        /// </summary>
        /// 
        /// <remarks><para>The property sets the source image, which will be transformed
        /// to the specified quadrilateral and put into destination image the filter is applied to.</para>
        /// 
        /// <para><note>The source image must have the same pixel format as a destination image the filter
        /// is applied to. Otherwise exception will be generated when filter is applied.</note></para>
        /// 
        /// <para><note>Setting this property will clear the <see cref="SourceUnmanagedImage"/> property -
        /// only one source image is allowed: managed or unmanaged.</note></para>
        /// </remarks>
        ///
        public Bitmap SourceImage
        {
            get { return sourceImage; }
            set
            {
                sourceImage = value;

                if ( value != null )
                    sourceUnmanagedImage = null;
            }
        }

        /// <summary>
        /// Source unmanaged image to be transformed into specified quadrilateral.
        /// </summary>
        /// 
        /// <remarks><para>The property sets the source image, which will be transformed
        /// to the specified quadrilateral and put into destination image the filter is applied to.</para>
        /// 
        /// <para><note>The source image must have the same pixel format as a destination image the filter
        /// is applied to. Otherwise exception will be generated when filter is applied.</note></para>
        /// 
        /// <para><note>Setting this property will clear the <see cref="SourceImage"/> property -
        /// only one source image is allowed: managed or unmanaged.</note></para>
        /// </remarks>
        ///
        public UnmanagedImage SourceUnmanagedImage
        {
            get { return sourceUnmanagedImage; }
            set
            {
                sourceUnmanagedImage = value;

                if ( value != null )
                    sourceImage = null;
            }
        }

        /// <summary>
        /// Quadrilateral in destination image to transform into.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies 4 corners of a quadrilateral area
        /// in destination image where the source image will be transformed into.
        /// </para></remarks>
        ///
        public List<IntPoint> DestinationQuadrilateral
        {
            get { return destinationQuadrilateral; }
            set { destinationQuadrilateral = value; }
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
        /// Initializes a new instance of the <see cref="BackwardQuadrilateralTransformation"/> class.
        /// </summary>
        /// 
        public BackwardQuadrilateralTransformation( )
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]    = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb]   = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format32bppPArgb]  = PixelFormat.Format32bppPArgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackwardQuadrilateralTransformation"/> class.
        /// </summary>
        /// 
        /// <param name="sourceImage">Source image to be transformed into specified quadrilateral
        /// (see <see cref="SourceImage"/>).</param>
        /// 
        public BackwardQuadrilateralTransformation( Bitmap sourceImage )
            : this( )
        {
            this.sourceImage = sourceImage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackwardQuadrilateralTransformation"/> class.
        /// </summary>
        /// 
        /// <param name="sourceUnmanagedImage">Source unmanaged image to be transformed into specified quadrilateral
        /// (see <see cref="SourceUnmanagedImage"/>).</param>
        /// 
        public BackwardQuadrilateralTransformation( UnmanagedImage sourceUnmanagedImage )
            : this( )
        {
            this.sourceUnmanagedImage = sourceUnmanagedImage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackwardQuadrilateralTransformation"/> class.
        /// </summary>
        /// 
        /// <param name="sourceImage">Source image to be transformed into specified quadrilateral
        /// (see <see cref="SourceImage"/>).</param>
        /// <param name="destinationQuadrilateral">Quadrilateral in destination image to transform into.</param>
        /// 
        public BackwardQuadrilateralTransformation( Bitmap sourceImage, List<IntPoint> destinationQuadrilateral )
            : this( )
        {
            this.sourceImage = sourceImage;
            this.destinationQuadrilateral = destinationQuadrilateral;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackwardQuadrilateralTransformation"/> class.
        /// </summary>
        /// 
        /// <param name="sourceUnmanagedImage">Source unmanaged image to be transformed into specified quadrilateral
        /// (see <see cref="SourceUnmanagedImage"/>).</param>
        /// <param name="destinationQuadrilateral">Quadrilateral in destination image to transform into.</param>
        /// 
        public BackwardQuadrilateralTransformation( UnmanagedImage sourceUnmanagedImage, List<IntPoint> destinationQuadrilateral )
            : this( )
        {
            this.sourceUnmanagedImage = sourceUnmanagedImage;
            this.destinationQuadrilateral = destinationQuadrilateral;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Image data to process by the filter.</param>
        ///
        /// <exception cref="NullReferenceException">Destination quadrilateral was not set.</exception>
        /// 
        protected override unsafe void ProcessFilter( UnmanagedImage image )
        {
            if ( destinationQuadrilateral == null )
                throw new NullReferenceException( "Destination quadrilateral was not set." );

            // check overlay type
            if ( sourceImage != null )
            {
                // source and destination images must have same pixel format
                if ( image.PixelFormat != sourceImage.PixelFormat )
                    throw new InvalidImagePropertiesException( "Source and destination images must have same pixel format." );

                // lock source image
                BitmapData srcData = sourceImage.LockBits(
                    new Rectangle( 0, 0, sourceImage.Width, sourceImage.Height ),
                    ImageLockMode.ReadOnly, sourceImage.PixelFormat );

                try
                {
                    ProcessFilter( image, new UnmanagedImage( srcData ) );
                }
                finally
                {
                    // unlock source image
                    sourceImage.UnlockBits( srcData );
                }
            }
            else if ( sourceUnmanagedImage != null )
            {
                // source and destination images must have same pixel format
                if ( image.PixelFormat != sourceUnmanagedImage.PixelFormat )
                    throw new InvalidImagePropertiesException( "Source and destination images must have same pixel format." );

                ProcessFilter( image, sourceUnmanagedImage );
            }
            else
            {
                throw new NullReferenceException( "Source image is not set." );
            }
        }

        // Process both images transforming source image into quadrilateral in destination image
        private unsafe void ProcessFilter( UnmanagedImage dstImage, UnmanagedImage srcImage )
        {
            // get source and destination images size
            int srcWidth  = srcImage.Width;
            int srcHeight = srcImage.Height;
            int dstWidth  = dstImage.Width;
            int dstHeight = dstImage.Height;

            int pixelSize = Image.GetPixelFormatSize( srcImage.PixelFormat ) / 8;
            int srcStride = srcImage.Stride;
            int dstStride = dstImage.Stride;

            // get bounding rectangle of the quadrilateral
            IntPoint minXY, maxXY;
            PointsCloud.GetBoundingRectangle( destinationQuadrilateral, out minXY, out maxXY );

            // make sure the rectangle is inside of destination image
            if ( ( maxXY.X < 0 ) || ( maxXY.Y < 0 ) || ( minXY.X >= dstWidth ) || ( minXY.Y >= dstHeight ) )
                return; // nothing to do, since quadrilateral is completely outside

            // correct rectangle if required
            if ( minXY.X < 0 )
                minXY.X = 0;
            if ( minXY.Y < 0 )
                minXY.Y = 0;
            if ( maxXY.X >= dstWidth )
                maxXY.X = dstWidth - 1;
            if ( maxXY.Y >= dstHeight )
                maxXY.Y = dstHeight - 1;

            int startX = minXY.X;
            int startY = minXY.Y;
            int stopX  = maxXY.X + 1;
            int stopY  = maxXY.Y + 1;
            int offset = dstStride - ( stopX - startX ) * pixelSize;

            // calculate tranformation matrix
            List<IntPoint> srcRect = new List<IntPoint>( );
            srcRect.Add( new IntPoint( 0, 0 ) );
            srcRect.Add( new IntPoint( srcWidth - 1, 0 ) );
            srcRect.Add( new IntPoint( srcWidth - 1, srcHeight - 1 ) );
            srcRect.Add( new IntPoint( 0, srcHeight - 1 ) );

            double[,] matrix = QuadTransformationCalcs.MapQuadToQuad( destinationQuadrilateral, srcRect );

            // do the job
            byte* ptr = (byte*) dstImage.ImageData.ToPointer( );
            byte* baseSrc = (byte*) srcImage.ImageData.ToPointer( );

            // allign pointer to the first pixel to process
            ptr += ( startY * dstStride + startX * pixelSize );

            if ( !useInterpolation )
            {
                byte* p;

                // for each row
                for ( int y = startY; y < stopY; y++ )
                {
                    // for each pixel
                    for ( int x = startX; x < stopX; x++ )
                    {
                        double factor = matrix[2, 0] * x + matrix[2, 1] * y + matrix[2, 2];
                        double srcX = ( matrix[0, 0] * x + matrix[0, 1] * y + matrix[0, 2] ) / factor;
                        double srcY = ( matrix[1, 0] * x + matrix[1, 1] * y + matrix[1, 2] ) / factor;

                        if ( ( srcX >= 0 ) && ( srcY >= 0 ) && ( srcX < srcWidth ) && ( srcY < srcHeight ) )
                        {
                            // get pointer to the pixel in the source image
                            p = baseSrc + (int) srcY * srcStride + (int) srcX * pixelSize;
                            // copy pixel's values
                            for ( int i = 0; i < pixelSize; i++, ptr++, p++ )
                            {
                                *ptr = *p;
                            }
                        }
                        else
                        {
                            // skip the pixel
                            ptr += pixelSize;
                        }
                    }
                    ptr += offset;
                }
            }
            else
            {
                int srcWidthM1  = srcWidth - 1;
                int srcHeightM1 = srcHeight - 1;

                // coordinates of source points
                double dx1, dy1, dx2, dy2;
                int sx1, sy1, sx2, sy2;

                // temporary pointers
                byte* p1, p2, p3, p4;

                // for each row
                for ( int y = startY; y < stopY; y++ )
                {
                    // for each pixel
                    for ( int x = startX; x < stopX; x++ )
                    {
                        double factor = matrix[2, 0] * x + matrix[2, 1] * y + matrix[2, 2];
                        double srcX = ( matrix[0, 0] * x + matrix[0, 1] * y + matrix[0, 2] ) / factor;
                        double srcY = ( matrix[1, 0] * x + matrix[1, 1] * y + matrix[1, 2] ) / factor;

                        if ( ( srcX >= 0 ) && ( srcY >= 0 ) && ( srcX < srcWidth ) && ( srcY < srcHeight ) )
                        {
                            sx1 = (int) srcX;
                            sx2 = ( sx1 == srcWidthM1 ) ? sx1 : sx1 + 1;
                            dx1 = srcX - sx1;
                            dx2 = 1.0 - dx1;

                            sy1 = (int) srcY;
                            sy2 = ( sy1 == srcHeightM1 ) ? sy1 : sy1 + 1;
                            dy1 = srcY - sy1;
                            dy2 = 1.0 - dy1;

                            // get four points
                            p1 = p2 = baseSrc + sy1 * srcStride;
                            p1 += sx1 * pixelSize;
                            p2 += sx2 * pixelSize;

                            p3 = p4 = baseSrc + sy2 * srcStride;
                            p3 += sx1 * pixelSize;
                            p4 += sx2 * pixelSize;

                            // interpolate using 4 points
                            for ( int i = 0; i < pixelSize; i++, ptr++, p1++, p2++, p3++, p4++ )
                            {
                                *ptr = (byte) (
                                    dy2 * ( dx2 * ( *p1 ) + dx1 * ( *p2 ) ) +
                                    dy1 * ( dx2 * ( *p3 ) + dx1 * ( *p4 ) ) );
                            }
                        }
                        else
                        {
                            // skip the pixel
                            ptr += pixelSize;
                        }
                    }
                    ptr += offset;
                }
            }
        }
    }
}