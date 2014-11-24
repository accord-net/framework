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
    /// Performs quadrilateral transformation using bilinear algorithm for interpolation.
    /// </summary>
    /// 
    /// <remarks><para>The class is deprecated and <see cref="SimpleQuadrilateralTransformation"/> should be used instead.</para>
    /// </remarks>
    /// 
    /// <seealso cref="SimpleQuadrilateralTransformation"/>
    /// 
    [Obsolete( "The class is deprecated and SimpleQuadrilateralTransformation should be used instead" )]
    public class QuadrilateralTransformationBilinear : BaseTransformationFilter
    {
        private SimpleQuadrilateralTransformation baseFilter = null;

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return baseFilter.FormatTranslations; }
        }

        /// <summary>
        /// Automatic calculation of destination image or not.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies how to calculate size of destination (transformed)
        /// image. If the property is set to <see langword="false"/>, then <see cref="NewWidth"/>
        /// and <see cref="NewHeight"/> properties have effect and destination image's size is
        /// specified by user. If the property is set to <see langword="true"/>, then setting the above
        /// mentioned properties does not have any effect, but destionation image's size is
        /// automatically calculated from <see cref="SourceCorners"/> property - width and height
        /// come from length of longest edges.
        /// </para></remarks>
        /// 
        public bool AutomaticSizeCalculaton
        {
            get { return baseFilter.AutomaticSizeCalculaton; }
            set { baseFilter.AutomaticSizeCalculaton = value; }
        }

        /// <summary>
        /// Quadrilateral's corners in source image.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies four corners of the quadrilateral area
        /// in the source image to be transformed.</para>
        /// </remarks>
        /// 
        public List<IntPoint> SourceCorners
        {
            get { return baseFilter.SourceQuadrilateral; }
            set { baseFilter.SourceQuadrilateral = value; }
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
        /// is calculated automatically based on <see cref="SourceCorners"/> property.</note></para>
        /// </remarks>
        /// 
        public int NewWidth
        {
            get { return baseFilter.NewWidth; }
            set { baseFilter.NewWidth = value; }
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
        /// is calculated automatically based on <see cref="SourceCorners"/> property.</note></para>
        /// </remarks>
        /// 
        public int NewHeight
        {
            get { return baseFilter.NewHeight; }
            set { baseFilter.NewHeight = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuadrilateralTransformationBilinear"/> class.
        /// </summary>
        /// 
        /// <param name="sourceCorners">Corners of the source quadrilateral area.</param>
        /// <param name="newWidth">Width of the new transformed image.</param>
        /// <param name="newHeight">Height of the new transformed image.</param>
        /// 
        /// <remarks><para>This constructor sets <see cref="AutomaticSizeCalculaton"/> to
        /// <see langword="false"/>, which means that destination image will have width and
        /// height as specified by user.</para></remarks>
        /// 
        public QuadrilateralTransformationBilinear( List<IntPoint> sourceCorners, int newWidth, int newHeight )
		{
            baseFilter = new SimpleQuadrilateralTransformation( sourceCorners, newWidth, newHeight );
            baseFilter.UseInterpolation = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuadrilateralTransformationBilinear"/> class.
        /// </summary>
        /// 
        /// <param name="sourceCorners">Corners of the source quadrilateral area.</param>
        /// 
        /// <remarks><para>This constructor sets <see cref="AutomaticSizeCalculaton"/> to
        /// <see langword="true"/>, which means that destination image will have width and
        /// height automatically calculated based on <see cref="SourceCorners"/> property.</para></remarks>
        ///
        public QuadrilateralTransformationBilinear( List<IntPoint> sourceCorners )
        {
            baseFilter = new SimpleQuadrilateralTransformation( sourceCorners );
            baseFilter.UseInterpolation = true;
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
            baseFilter.Apply( sourceData, destinationData );
        }

        /// <summary>
        /// Calculates new image size.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// 
        /// <returns>New image size - size of the destination image.</returns>
        /// 
        /// <exception cref="ArgumentException">The specified quadrilateral's corners are outside of the given image.</exception>
        /// 
        protected override System.Drawing.Size CalculateNewImageSize( UnmanagedImage sourceData )
        {
            // perform checking of source corners - they must feet into the image
            foreach ( IntPoint point in baseFilter.SourceQuadrilateral )
            {
                if ( ( point.X < 0 ) ||
                     ( point.Y < 0 ) ||
                     ( point.X >= sourceData.Width ) ||
                     ( point.Y >= sourceData.Height ) )
                {
                    throw new ArgumentException( "The specified quadrilateral's corners are outside of the given image." );
                }
            }

            return new Size( baseFilter.NewWidth, baseFilter.NewHeight );
        }
    }
}
