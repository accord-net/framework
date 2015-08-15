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
    /// Texturer filter.
    /// </summary>
    /// 
    /// <remarks><para>Adjust pixels’ color values using factors from the given texture. In conjunction with different type
    /// of texture generators, the filter may produce different type of interesting effects.</para>
    /// 
    /// <para>The filter uses specified texture to adjust values using the next formula:<br/>
    /// <b>dst = src * <see cref="PreserveLevel"/> + src * <see cref="FilterLevel"/> * textureValue</b>,<br/>
    /// where <b>src</b> is value of pixel in a source image, <b>dst</b> is value of pixel in a destination image and
    /// <b>textureValue</b> is corresponding value from provided texture (see <see cref="TextureGenerator"/> or
    /// <see cref="Texture"/>). Using <see cref="PreserveLevel"/> and <see cref="FilterLevel"/> values it is possible
    /// to control the portion of source data affected by texture.
    /// </para>
    /// 
    /// <para>In most cases the <see cref="PreserveLevel"/> and <see cref="FilterLevel"/> properties are set in such
    /// way, that <see cref="PreserveLevel"/> + <see cref="FilterLevel"/> = <b>1</b>. But there is no limitations actually
    /// for those values, so their sum may be as greater, as lower than 1 in order create different type of
    /// effects.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale and 24 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// Texturer filter = new Texturer( new TextileTexture( ), 0.3, 0.7 );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/texturer.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    public class Texturer : BaseInPlacePartialFilter
    {
        // texture generator
        private AForge.Imaging.Textures.ITextureGenerator textureGenerator;
        // generated texture
        private float[,] texture = null;

        // filtering factor
        private double filterLevel = 0.5;
        // preservation factor
        private double preserveLevel = 0.5;

        // private format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        ///
        /// <remarks><para>See <see cref="IFilterInformation.FormatTranslations"/> for more information.</para></remarks>
        ///
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        /// Filter level value.
        /// </summary>
        /// 
        /// <remarks><para>Filtering factor determines image fraction to filter - to multiply 
        /// by values from the provided texture.</para>
        /// 
        /// <para>Default value is set to <b>0.5</b>.</para>
        /// 
        /// <para>See <see cref="Texturer"/> class description for more details.</para>
        /// </remarks>
        /// 
        public double FilterLevel
        {
            get { return filterLevel; }
            set { filterLevel = Math.Max( 0.0, Math.Min( 1.0, value ) ); }
        }

        /// <summary>
        /// Preserve level value.
        /// </summary>
        /// 
        /// <remarks><para>Preserving factor determines image fraction to keep from filtering.</para>
        /// 
        /// <para>Default value is set to <b>0.5</b>.</para>
        /// 
        /// <para>See <see cref="Texturer"/> class description for more details.</para>
        /// </remarks>
        /// 
        public double PreserveLevel
        {
            get { return preserveLevel; }
            set { preserveLevel = Math.Max( 0.0, Math.Min( 1.0, value ) ); }
        }

        /// <summary>
        /// Generated texture.
        /// </summary>
        /// 
        /// <remarks><para>Two dimensional array of texture intensities.</para>
        /// 
        /// <para><note>In the case if image passed to the filter is smaller or
        /// larger than the specified texture, than image's region is processed, which equals to the
        /// minimum overlapping area.</note></para>
        /// 
        /// <para><note>The <see cref="TextureGenerator"/> property has priority over this property - if
        /// generator is specified than the static generated texture is not used.</note></para>
        /// </remarks>
        /// 
        public float[,] Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        /// <summary>
        /// Texture generator.
        /// </summary>
        /// 
        /// <remarks><para>Generator used to generate texture.</para>
        /// 
        /// <para><note>The property has priority over the <see cref="Texture"/> property.</note></para>
        /// </remarks>
        /// 
        public AForge.Imaging.Textures.ITextureGenerator TextureGenerator
        {
            get { return textureGenerator; }
            set { textureGenerator = value; }
        }

        // Private constructor to do common initialization
        private Texturer( )
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texturer"/> class.
        /// </summary>
        /// 
        /// <param name="texture">Generated texture.</param>
        /// 
        public Texturer( float[,] texture ) : this( )
        {
            this.texture = texture;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texturer"/> class.
        /// </summary>
        /// 
        /// <param name="texture">Generated texture.</param>
        /// <param name="filterLevel">Filter level value (see <see cref="FilterLevel"/> property).</param>
        /// <param name="preserveLevel">Preserve level value (see <see cref="PreserveLevel"/> property).</param>
        /// 
        public Texturer( float[,] texture, double filterLevel, double preserveLevel ) : this( )
        {
            this.texture = texture;
            this.filterLevel = filterLevel;
            this.preserveLevel = preserveLevel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texturer"/> class.
        /// </summary>
        /// 
        /// <param name="generator">Texture generator.</param>
        /// 
        public Texturer( AForge.Imaging.Textures.ITextureGenerator generator ) : this( )
        {
            this.textureGenerator = generator;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texturer"/> class.
        /// </summary>
        /// 
        /// <param name="generator">Texture generator.</param>
        /// <param name="filterLevel">Filter level value (see <see cref="FilterLevel"/> property).</param>
        /// <param name="preserveLevel">Preserve level value (see <see cref="PreserveLevel"/> property).</param>
        /// 
        public Texturer( AForge.Imaging.Textures.ITextureGenerator generator, double filterLevel, double preserveLevel )
            : this( )
        {
            this.textureGenerator = generator;
            this.filterLevel = filterLevel;
            this.preserveLevel = preserveLevel;
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
            int pixelSize = Image.GetPixelFormatSize( image.PixelFormat ) / 8;

            // processing width and height
            int width  = rect.Width;
            int height = rect.Height;

            // processing region's dimension
            int widthToProcess  = width;
            int heightToProcess = height;

            // if generator was specified, then generate a texture
            // otherwise use provided texture
            if ( textureGenerator != null )
            {
                texture = textureGenerator.Generate( width, height );
            }
            else
            {
                widthToProcess  = Math.Min( width, texture.GetLength( 1 ) );
                heightToProcess = Math.Min( height, texture.GetLength( 0 ) );
            }

            int offset = image.Stride - widthToProcess * pixelSize;

            // do the job
            byte* ptr = (byte*) image.ImageData.ToPointer( );

            // allign pointer to the first pixel to process
            ptr += ( rect.Top * image.Stride + rect.Left * pixelSize );

            // texture
            for ( int y = 0; y < heightToProcess; y++ )
            {
                for ( int x = 0; x < widthToProcess; x++ )
                {
                    double t = texture[y, x];
                    // process each pixel
                    for ( int i = 0; i < pixelSize; i++, ptr++ )
                    {
                        *ptr = (byte) Math.Min( 255.0f, ( preserveLevel * ( *ptr ) ) + ( filterLevel * ( *ptr ) ) * t );
                    }
                }
                ptr += offset;
            }
        }
    }
}
