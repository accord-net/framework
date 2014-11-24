// AForge Image Processing Library
// AForge.NET framework
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
    /// Bottop-hat operator from Mathematical Morphology.
    /// </summary>
    /// 
    /// <remarks><para>Bottom-hat morphological operator <see cref="Subtract">subtracts</see>
    /// input image from the result of <see cref="Closing">morphological closing</see> on the
    /// the input image.</para>
    /// 
    /// <para>Applied to binary image, the filter allows to get all object parts, which were
    /// added by <see cref="Closing">closing</see> filter, but were not removed after that due
    /// to formed connections/fillings.</para>
    /// 
    /// <para>The filter accepts 8 and 16 bpp grayscale images and 24 and 48 bpp
    /// color images for processing.</para>
    ///
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// BottomHat filter = new BottomHat( );
    /// // apply the filter
    /// filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample12.png" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/bottomhat.png" width="320" height="240" />
    /// </remarks>
    /// 
    /// <seealso cref="TopHat"/>
    /// 
    public class BottomHat : BaseInPlaceFilter
    {
        private Closing closing = new Closing( );
        private Subtract subtract = new Subtract( );

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
        /// Initializes a new instance of the <see cref="BottomHat"/> class.
        /// </summary>
        /// 
        public BottomHat( )
        {
            // initialize format translation dictionary
            formatTranslations[PixelFormat.Format8bppIndexed]    = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]       = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            formatTranslations[PixelFormat.Format48bppRgb]       = PixelFormat.Format48bppRgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BottomHat"/> class.
        /// </summary>
        /// 
        /// <param name="se">Structuring element to pass to <see cref="Closing"/> operator.</param>
        /// 
        public BottomHat( short[,] se ) : this( )
        {
            closing = new Closing( se );
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        ///
        protected override unsafe void ProcessFilter( UnmanagedImage image )
        {
            // copy source image
            UnmanagedImage sourceImage = image.Clone( );
            // perform closing on the source image
            closing.ApplyInPlace( image );
            // subtract source image from the closed image
            subtract.UnmanagedOverlayImage = sourceImage;
            subtract.ApplyInPlace( image );

            sourceImage.Dispose( );
        }
    }
}
