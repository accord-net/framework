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
    /// Filter to mark (highlight) corners of objects.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>The filter highlights corners of objects on the image using provided corners
    /// detection algorithm.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale and 24/32 color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create corner detector's instance
    /// SusanCornersDetector scd = new SusanCornersDetector( );
    /// // create corner maker filter
    /// CornersMarker filter = new CornersMarker( scd, Color.Red );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample2.jpg" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/susan_corners.png" width="320" height="240" />
    /// </remarks>
    /// 
    public class CornersMarker : BaseInPlaceFilter
    {
        // color used to mark corners
        private Color markerColor = Color.White;
        // algorithm used to detect corners
        private ICornersDetector detector = null;

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
        /// Color used to mark corners.
        /// </summary>
        public Color MarkerColor
        {
            get { return markerColor; }
            set { markerColor = value; }
        }

        /// <summary>
        /// Interface of corners' detection algorithm used to detect corners.
        /// </summary>
        public ICornersDetector Detector
        {
            get { return detector; }
            set { detector = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CornersMarker"/> class.
        /// </summary>
        /// 
        /// <param name="detector">Interface of corners' detection algorithm.</param>
        /// 
        public CornersMarker( ICornersDetector detector ) : this( detector, Color.White )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CornersMarker"/> class.
        /// </summary>
        /// 
        /// <param name="detector">Interface of corners' detection algorithm.</param>
        /// <param name="markerColor">Marker's color used to mark corner.</param>
        /// 
        public CornersMarker( ICornersDetector detector, Color markerColor )
        {
            this.detector    = detector;
            this.markerColor = markerColor;

            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]    = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb]   = PixelFormat.Format32bppArgb;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        ///
        protected override unsafe void ProcessFilter( UnmanagedImage image )
        {
            // get collection of corners
            List<IntPoint> corners = detector.ProcessImage( image );
            // mark all corners
            foreach ( IntPoint corner in corners )
            {
                Drawing.FillRectangle( image, new Rectangle( corner.X - 1, corner.Y - 1, 3, 3 ), markerColor );
            }
        }
    }
}
