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
    /// Merge two images using factors from texture.
    /// </summary>
    /// 
    /// <remarks><para>The filter is similar to <see cref="Morph"/> filter in its idea, but
    /// instead of using single value for balancing amount of source's and overlay's image
    /// values (see <see cref="Morph.SourcePercent"/>), the filter uses texture, which determines
    /// the amount to take from source image and overlay image.</para>
    /// 
    /// <para>The filter uses specified texture to adjust values using the next formula:<br/>
    /// <b>dst = src * textureValue + ovr * ( 1.0 - textureValue )</b>,<br/>
    /// where <b>src</b> is value of pixel in a source image, <b>ovr</b> is value of pixel in
    /// overlay image, <b>dst</b> is value of pixel in a destination image and
    /// <b>textureValue</b> is corresponding value from provided texture (see <see cref="TextureGenerator"/> or
    /// <see cref="Texture"/>).</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale and 24 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage #1:</para>
    /// <code>
    /// // create filter
    /// TexturedMerge filter = new TexturedMerge( new TextileTexture( ) );
    /// // create an overlay image to merge with
    /// filter.OverlayImage = new Bitmap( image.Width, image.Height,
    ///         PixelFormat.Format24bppRgb );
    /// // fill the overlay image with solid color
    /// PointedColorFloodFill fillFilter = new PointedColorFloodFill( Color.DarkKhaki );
    /// fillFilter.ApplyInPlace( filter.OverlayImage );
    /// // apply the merge filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para>Sample usage #2:</para>
    /// <code>
    /// // create filter
    /// TexturedMerge filter = new TexturedMerge( new CloudsTexture( ) );
    /// // create 2 images with modified Hue
    /// HueModifier hm1 = new HueModifier( 50 );
    /// HueModifier hm2 = new HueModifier( 200 );
    /// filter.OverlayImage = hm2.Apply( image );
    /// hm1.ApplyInPlace( image );
    /// // apply the merge filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image #1:</b></para>
    /// <img src="img/imaging/textured_merge1.jpg" width="480" height="361" />
    /// <para><b>Result image #2:</b></para>
    /// <img src="img/imaging/textured_merge2.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    public class TexturedMerge : BaseInPlaceFilter2
    {
        // texture generator
        private AForge.Imaging.Textures.ITextureGenerator textureGenerator;
        // generated texture
        private float[,] texture = null;

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
        private TexturedMerge( )
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TexturedMerge"/> class.
        /// </summary>
        /// 
        /// <param name="texture">Generated texture.</param>
        /// 
        public TexturedMerge( float[,] texture ) : this( )
        {
            this.texture = texture;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TexturedMerge"/> class.
        /// </summary>
        /// 
        /// <param name="generator">Texture generator.</param>
        /// 
        public TexturedMerge( AForge.Imaging.Textures.ITextureGenerator generator ) : this( )
        {
            this.textureGenerator = generator;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        /// <param name="overlay">Overlay image data.</param>
        ///
        protected override unsafe void ProcessFilter( UnmanagedImage image, UnmanagedImage overlay )
        {
            // get image dimension
            int width	= image.Width;
            int height	= image.Height;

            // width and height to process
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

            int pixelSize = Image.GetPixelFormatSize( image.PixelFormat ) / 8;
            int srcOffset = image.Stride - widthToProcess * pixelSize;
            int ovrOffset = overlay.Stride - widthToProcess * pixelSize;

            // do the job
            byte* ptr = (byte*) image.ImageData.ToPointer( );
            byte* ovr = (byte*) overlay.ImageData.ToPointer( );

            // for each line
            for ( int y = 0; y < heightToProcess; y++ )
            {
                // for each pixel
                for ( int x = 0; x < widthToProcess; x++ )
                {
                    double t1 = texture[y, x];
                    double t2 = 1 - t1;

                    for ( int i = 0; i < pixelSize; i++, ptr++, ovr++ )
                    {
                        *ptr = (byte) Math.Min( 255.0f, *ptr * t1 + *ovr * t2 );
                    }
                }
                ptr += srcOffset;
                ovr += ovrOffset;
            }
        }
    }
}
