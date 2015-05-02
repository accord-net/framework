// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2010
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Euclidean color filtering.
    /// </summary>
    /// 
    /// <remarks><para>The filter filters pixels, which color is inside/outside
    /// of RGB sphere with specified center and radius - it keeps pixels with
    /// colors inside/outside of the specified sphere and fills the rest with
    /// <see cref="FillColor">specified color</see>.</para>
    /// 
    /// <para>The filter accepts 24 and 32 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// EuclideanColorFiltering filter = new EuclideanColorFiltering( );
    /// // set center colol and radius
    /// filter.CenterColor = new RGB( 215, 30, 30 );
    /// filter.Radius = 100;
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    ///
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/euclidean_filtering.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    /// <seealso cref="ColorFiltering"/>
    /// 
    public class EuclideanColorFiltering : BaseInPlacePartialFilter
    {
        private short radius = 100;
        private RGB center = new RGB( 255, 255, 255 );
        private RGB fill = new RGB( 0, 0, 0 );
        private bool fillOutside = true;

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
        /// RGB sphere's radius, [0, 450].
        /// </summary>
        /// 
        /// <remarks>Default value is 100.</remarks>
        /// 
        public short Radius
        {
            get { return radius; }
            set
            {
                radius = System.Math.Max( (short) 0, System.Math.Min( (short) 450, value ) );
            }
        }

        /// <summary>
        /// RGB sphere's center.
        /// </summary>
        /// 
        /// <remarks>Default value is (255, 255, 255) - white color.</remarks>
        /// 
        public RGB CenterColor
        {
            get { return center; }
            set { center = value; }
        }

        /// <summary>
        /// Fill color used to fill filtered pixels.
        /// </summary>
        public RGB FillColor
        {
            get { return fill; }
            set { fill = value; }
        }

        /// <summary>
        /// Determines, if pixels should be filled inside or outside specified
        /// RGB sphere.
        /// </summary>
        /// 
        /// <remarks><para>Default value is set to <see langword="true"/>, which means
        /// the filter removes colors outside of the specified range.</para></remarks>
        /// 
        public bool FillOutside
        {
            get { return fillOutside; }
            set { fillOutside = value; }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="EuclideanColorFiltering"/> class.
        /// </summary>
        /// 
        public EuclideanColorFiltering()
        {
            formatTranslations[PixelFormat.Format24bppRgb]  = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]  = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EuclideanColorFiltering"/> class.
        /// </summary>
        /// 
        /// <param name="center">RGB sphere's center.</param>
        /// <param name="radius">RGB sphere's radius.</param>
        /// 
        public EuclideanColorFiltering( RGB center, short radius ) :
            this( )
        {
            this.center = center;
            this.radius = radius;
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
            // get pixel size
            int pixelSize = ( image.PixelFormat == PixelFormat.Format24bppRgb ) ? 3 : 4;

            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;
            int offset  = image.Stride - rect.Width * pixelSize;
            int radius2 = radius * radius;

            int dr, dg, db;
            // sphere's center
            int cR = center.Red;
            int cG = center.Green;
            int cB = center.Blue;
            // fill color
            byte fR = fill.Red;
            byte fG = fill.Green;
            byte fB = fill.Blue;

            // do the job
            byte* ptr = (byte*) image.ImageData.ToPointer( );

            // allign pointer to the first pixel to process
            ptr += ( startY * image.Stride + startX * pixelSize );

            // for each row
            for ( int y = startY; y < stopY; y++ )
            {
                // for each pixel
                for ( int x = startX; x < stopX; x++, ptr += pixelSize )
                {
                    dr = cR - ptr[RGB.R];
                    dg = cG - ptr[RGB.G];
                    db = cB - ptr[RGB.B];

                    // calculate the distance
                    if ( dr * dr + dg * dg + db * db <= radius2 )
                    {
                        // inside sphere
                        if ( !fillOutside )
                        {
                            ptr[RGB.R] = fR;
                            ptr[RGB.G] = fG;
                            ptr[RGB.B] = fB;
                        }
                    }
                    else
                    {
                        // outside sphere
                        if ( fillOutside )
                        {
                            ptr[RGB.R] = fR;
                            ptr[RGB.G] = fG;
                            ptr[RGB.B] = fB;
                        }
                    }
                }
                ptr += offset;
            }
        }
    }
}
