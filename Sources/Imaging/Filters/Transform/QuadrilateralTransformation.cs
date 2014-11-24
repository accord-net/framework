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
    /// Performs quadrilateral transformation of an area in a given source image.
    /// </summary>
    /// 
    /// <remarks><para>The class implements quadrilateral transformation algorithm,
    /// which allows to transform any quadrilateral from a given source image
    /// to a rectangular image. The idea of the algorithm is based on homogeneous
    /// transformation and its math is described by Paul Heckbert in his
    /// "<a href="http://graphics.cs.cmu.edu/courses/15-463/2008_fall/Papers/proj.pdf">Projective Mappings for Image Warping</a>" paper.
    /// </para>
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
    /// QuadrilateralTransformation filter =
    ///     new QuadrilateralTransformation( corners, 200, 200 );
    /// // apply the filter
    /// Bitmap newImage = filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample18.jpg" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/quadrilateral_ex_bilinear.png" width="200" height="200" />
    /// </remarks>
    /// 
    /// <seealso cref="BackwardQuadrilateralTransformation"/>
    /// <seealso cref="SimpleQuadrilateralTransformation"/>
    /// 
    public class QuadrilateralTransformation : BaseTransformationFilter
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
        /// Initializes a new instance of the <see cref="QuadrilateralTransformation"/> class.
        /// </summary>
        /// 
        public QuadrilateralTransformation( )
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]    = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb]   = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format32bppPArgb]  = PixelFormat.Format32bppPArgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuadrilateralTransformation"/> class.
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
        public QuadrilateralTransformation( List<IntPoint> sourceQuadrilateral, int newWidth, int newHeight )
            : this( )
        {
            this.automaticSizeCalculaton = false;
            this.sourceQuadrilateral = sourceQuadrilateral;
            this.newWidth  = newWidth;
            this.newHeight = newHeight;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuadrilateralTransformation"/> class.
        /// </summary>
        /// 
        /// <param name="sourceQuadrilateral">Corners of the source quadrilateral area.</param>
        /// 
        /// <remarks><para>This constructor sets <see cref="AutomaticSizeCalculaton"/> to
        /// <see langword="true"/>, which means that destination image will have width and
        /// height automatically calculated based on <see cref="SourceQuadrilateral"/> property.</para></remarks>
        ///
        public QuadrilateralTransformation( List<IntPoint> sourceQuadrilateral )
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
            int offset = dstStride - dstWidth * pixelSize;

            // calculate tranformation matrix
            List<IntPoint> dstRect = new List<IntPoint>( );
            dstRect.Add( new IntPoint( 0, 0 ) );
            dstRect.Add( new IntPoint( dstWidth - 1, 0 ) );
            dstRect.Add( new IntPoint( dstWidth - 1, dstHeight - 1 ) );
            dstRect.Add( new IntPoint( 0, dstHeight - 1 ) );

            // calculate tranformation matrix
            double[,] matrix = QuadTransformationCalcs.MapQuadToQuad( dstRect, sourceQuadrilateral );

            // do the job
            byte* ptr = (byte*) destinationData.ImageData.ToPointer( );
            byte* baseSrc = (byte*) sourceData.ImageData.ToPointer( );

            if ( !useInterpolation )
            {
                byte* p;

                // for each row
                for ( int y = 0; y < dstHeight; y++ )
                {
                    // for each pixel
                    for ( int x = 0; x < dstWidth; x++ )
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
                for ( int y = 0; y < dstHeight; y++ )
                {
                    // for each pixel
                    for ( int x = 0; x < dstWidth; x++ )
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
                            ptr += pixelSize;
                        }
                    }
                    ptr += offset;
                }
            }
        }
    }
}
