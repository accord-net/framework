// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2007-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Fill areas iniside of the specified region.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>The filter fills areas inside of specified region using the specified color.</para>
    /// 
    /// <para>The filter accepts 8bpp grayscale and 24/32 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// CanvasFill filter = new CanvasFill( new Rectangle(
    ///                         5, 5, image.Width - 10, image.Height - 10 ), Color.Red );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="CanvasCrop"/>
    /// 
    public class CanvasFill : BaseInPlaceFilter
    {
        // RGB fill color
        private byte fillRed   = 255;
        private byte fillGreen = 255;
        private byte fillBlue  = 255;
        // gray fill color
        private byte fillGray = 255;
        // region to keep
        private Rectangle region;

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
        /// RGB fill color.
        /// </summary>
        /// 
        /// <remarks><para>The color is used to fill areas out of specified region in color images.</para>
        /// 
        /// <para>Default value is set to white - RGB(255, 255, 255).</para></remarks>
        /// 
        public Color FillColorRGB
        {
            get { return Color.FromArgb( fillRed, fillGreen, fillBlue ); }
            set
            {
                fillRed = value.R;
                fillGreen = value.G;
                fillBlue = value.B;
            }
        }

        /// <summary>
        /// Gray fill color.
        /// </summary>
        /// 
        /// <remarks><para>The color is used to fill areas out of specified region in grayscale images.</para>
        /// 
        /// <para>Default value is set to white - 255.</para></remarks>
        /// 
        public byte FillColorGray
        {
            get { return fillGray; }
            set { fillGray = value; }
        }

        /// <summary>
        /// Region to fill.
        /// </summary>
        /// 
        /// <remarks>Pixels inside of the specified region will be filled with specified color.</remarks>
        /// 
        public Rectangle Region
        {
            get { return region; }
            set { region = value; }
        }

        // Private constructor to do common initialization
        private CanvasFill( )
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb]   = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format32bppRgb]    = PixelFormat.Format32bppRgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasFill"/> class.
        /// </summary>
        /// 
        /// <param name="region">Region to fill.</param>
        /// 
        public CanvasFill( Rectangle region )
            : this( )
        {
            this.region = region;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasFill"/> class.
        /// </summary>
        /// 
        /// <param name="region">Region to fill.</param>
        /// <param name="fillColorRGB">RGB color to use for filling areas inside of specified region in color images.</param>
        /// 
        public CanvasFill( Rectangle region, Color fillColorRGB )
            : this( )
        {
            this.region    = region;
            this.fillRed   = fillColorRGB.R;
            this.fillGreen = fillColorRGB.G;
            this.fillBlue  = fillColorRGB.B;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasFill"/> class.
        /// </summary>
        /// 
        /// <param name="region">Region to fill.</param>
        /// <param name="fillColorGray">Gray color to use for filling areas inside of specified region in grayscale images.</param>
        /// 
        public CanvasFill( Rectangle region, byte fillColorGray )
            : this( )
        {
            this.region   = region;
            this.fillGray = fillColorGray;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasFill"/> class.
        /// </summary>
        /// 
        /// <param name="region">Region to fill.</param>
        /// <param name="fillColorRGB">RGB color to use for filling areas inside of specified region in color images.</param>
        /// <param name="fillColorGray">Gray color to use for filling areas inside of specified region in grayscale images.</param>
        /// 
        public CanvasFill( Rectangle region, Color fillColorRGB, byte fillColorGray )
            : this( )
        {
            this.region    = region;
            this.fillRed   = fillColorRGB.R;
            this.fillGreen = fillColorRGB.G;
            this.fillBlue  = fillColorRGB.B;
            this.fillGray  = fillColorGray;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        ///
        protected override unsafe void ProcessFilter( UnmanagedImage image )
        {
            int pixelSize = Image.GetPixelFormatSize( image.PixelFormat ) / 8;

            // get image width and height
            int width  = image.Width;
            int height = image.Height;

            // start (X, Y) point of filling region
            int startX = Math.Max( 0, region.X );
            int startY = Math.Max( 0, region.Y );

            // check if there is nothing to do
            if ( ( startX >= width ) || ( startY >= height ) )
                return;

            // stop (X, Y) point of filling region
            int stopX = Math.Min( width, region.Right );
            int stopY = Math.Min( height, region.Bottom );

            // check if there is nothing to do
            if ( ( stopX <= startX ) || ( stopY <= startY ) )
                return;

            int stride = image.Stride;

            // do the job
            byte * ptr = (byte*) image.ImageData.ToPointer( ) + startY * stride + startX * pixelSize;

            if ( image.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // grayscale image
                int fillWidth = stopX - startX;

                for ( int y = startY; y < stopY; y++ )
                {
                    AForge.SystemTools.SetUnmanagedMemory( ptr, fillGray, fillWidth );
                    ptr += stride;
                }
            }
            else
            {
                // color image
                int offset = stride - ( stopX - startX ) * pixelSize;

                for ( int y = startY; y < stopY; y++ )
                {
                    for ( int x = startX; x < stopX; x++, ptr += pixelSize )
                    {
                        ptr[RGB.R] = fillRed;
                        ptr[RGB.G] = fillGreen;
                        ptr[RGB.B] = fillBlue;
                    }
                    ptr += offset;
                }
            }
        }
    }
}
