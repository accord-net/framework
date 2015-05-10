// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Volodymyr Goncharov, 2007
// volodymyr.goncharov@gmail.com
//
// Copyright © Andrew Kirillov, 2007-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System.Drawing;
    using System.Collections.Generic;
    using System.Drawing.Imaging;

    /// <summary>
    /// Fill areas outiside of specified region.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>The filter fills areas outside of specified region using the specified color.</para>
    /// 
    /// <para>The filter accepts 8bpp grayscale and 24/32 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// CanvasCrop filter = new CanvasCrop( new Rectangle(
    ///                         5, 5, image.Width - 10, image.Height - 10 ), Color.Red );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/canvas_crop.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    /// <seealso cref="CanvasFill"/>
    /// 
    public class CanvasCrop : BaseInPlaceFilter
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
                fillRed   = value.R;
                fillGreen = value.G;
                fillBlue  = value.B;
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
        /// Region to keep.
        /// </summary>
        /// 
        /// <remarks>Pixels inside of the specified region will keep their values, but
        /// pixels outside of the region will be filled with specified color.</remarks>
        /// 
        public Rectangle Region
        {
            get { return region; }
            set { region = value; }
        }

        // Private constructor to do common initialization
        private CanvasCrop( )
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb]   = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format32bppRgb]    = PixelFormat.Format32bppRgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasCrop"/> class.
        /// </summary>
        /// 
        /// <param name="region">Region to keep.</param>
        /// 
        public CanvasCrop( Rectangle region ) : this( )
        {
            this.region = region;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasCrop"/> class.
        /// </summary>
        /// 
        /// <param name="region">Region to keep.</param>
        /// <param name="fillColorRGB">RGB color to use for filling areas outside of specified region in color images.</param>
        /// 
        public CanvasCrop( Rectangle region, Color fillColorRGB )
            : this( )
        {
            this.region    = region;
            this.fillRed   = fillColorRGB.R;
            this.fillGreen = fillColorRGB.G;
            this.fillBlue  = fillColorRGB.B;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasCrop"/> class.
        /// </summary>
        /// 
        /// <param name="region">Region to keep.</param>
        /// <param name="fillColorGray">Gray color to use for filling areas outside of specified region in grayscale images.</param>
        /// 
        public CanvasCrop( Rectangle region, byte fillColorGray )
            : this( )
        {
            this.region   = region;
            this.fillGray = fillColorGray;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasCrop"/> class.
        /// </summary>
        /// 
        /// <param name="region">Region to keep.</param>
        /// <param name="fillColorRGB">RGB color to use for filling areas outside of specified region in color images.</param>
        /// <param name="fillColorGray">Gray color to use for filling areas outside of specified region in grayscale images.</param>
        /// 
        public CanvasCrop( Rectangle region, Color fillColorRGB, byte fillColorGray )
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
            int offset = image.Stride - width * pixelSize;

            // do the job
            byte * ptr = (byte*) image.ImageData.ToPointer( );

            if ( image.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // grayscale image
                for ( int y = 0; y < height; y++ )
                {
                    for ( int x = 0; x < width; x++, ptr++ )
                    {
                        if ( !region.Contains( x, y ) )
                        {
                            *ptr = fillGray;
                        }
                    }
                    ptr += offset;
                }
            }
            else
            {
                // color image
                for ( int y = 0; y < height; y++ )
                {
                    for ( int x = 0; x < width; x++, ptr += pixelSize )
                    {
                        if ( !region.Contains( x, y ) )
                        {
                            // red
                            ptr[RGB.R] = fillRed;
                            // green
                            ptr[RGB.G] = fillGreen;
                            // blue
                            ptr[RGB.B] = fillBlue;
                        }
                    }
                    ptr += offset;
                }
            }
        }
    }
}