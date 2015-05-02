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

    using AForge.Imaging.Textures;

    /// <summary>
    /// Textured filter - filter an image using texture.
    /// </summary>
    /// 
    /// <remarks><para>The filter is similar to <see cref="TexturedMerge"/> filter in its
    /// nature, but instead of working with source image and overly, it uses provided
    /// filters to create images to merge (see <see cref="Filter1"/> and <see cref="Filter2"/>
    /// properties). In addition, it uses a bit more complex formula for calculation
    /// of destination pixel's value, which gives greater amount of flexibility:<br />
    /// <b>dst = <see cref="FilterLevel"/> * ( src1 * textureValue + src2 * ( 1.0 - textureValue ) ) + <see cref="PreserveLevel"/> * src2</b>,
    /// where <b>src1</b> is value of pixel from the image produced by <see cref="Filter1"/>,
    /// <b>src2</b> is value of pixel from the image produced by <see cref="Filter2"/>,
    /// <b>dst</b> is value of pixel in a destination image and <b>textureValue</b> is corresponding value
    /// from provided texture (see <see cref="TextureGenerator"/> or <see cref="Texture"/>).</para>
    /// 
    /// <para><note>It is possible to set <see cref="Filter2"/> to <see langword="null"/>. In this case
    /// original source image will be used instead of result produced by the second filter.</note></para>
    /// 
    /// <para>The filter 24 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage #1:</para>
    /// <code>
    /// // create filter
    /// TexturedFilter filter = new TexturedFilter( new CloudsTexture( ),
    ///     new HueModifier( 50 ) );
    /// // apply the filter
    /// Bitmap newImage = filter.Apply( image );
    /// </code>
    /// 
    /// <para>Sample usage #2:</para>
    /// <code>
    /// // create filter
    /// TexturedFilter filter = new TexturedFilter( new CloudsTexture( ),
    ///     new GrayscaleBT709( ), new Sepia( ) );
    /// // apply the filter
    /// Bitmap newImage = filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image #1:</b></para>
    /// <img src="img/imaging/textured_filter1.jpg" width="480" height="361" />
    /// <para><b>Result image #2:</b></para>
    /// <img src="img/imaging/textured_filter2.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    public class TexturedFilter : BaseFilter
    {
        // texture generator
        private ITextureGenerator textureGenerator;
        // generated texture
        private float[,] texture = null;
        // two filters
        private IFilter	filter1 = null;
        private IFilter	filter2 = null;

        // filtering factor
        private double filterLevel = 1.0;
        // preservation factor
        private double preserveLevel = 0.0;

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
        /// Filter level value, [0, 1].
        /// </summary>
        /// 
        /// <remarks><para>Filtering factor determines portion of the destionation image, which is formed
        /// as a result of merging source images using specified texture.</para>
        /// 
        /// <para>Default value is set to <b>1.0</b>.</para>
        /// 
        /// <para>See <see cref="TexturedFilter"/> class description for more details.</para>
        /// </remarks>
        /// 
        public double FilterLevel
        {
            get { return filterLevel; }
            set { filterLevel = Math.Max( 0.0, Math.Min( 1.0, value ) ); }
        }

        /// <summary>
        /// Preserve level value
        /// </summary>
        /// 
        /// <remarks><para>Preserving factor determines portion taken from the image produced
        /// by <see cref="Filter2"/> (or from original source) without applying textured
        /// merge to it.</para>
        /// 
        /// <para>Default value is set to <b>0.0</b>.</para>
        /// 
        /// <para>See <see cref="TexturedFilter"/> class description for more details.</para>
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
        /// <para><note>Size of the provided texture should be the same as size of images, which will
        /// be passed to the filter.</note></para>
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
        public ITextureGenerator TextureGenerator
        {
            get { return textureGenerator; }
            set { textureGenerator = value; }
        }

        /// <summary>
        /// First filter.
        /// </summary>
        /// 
        /// <remarks><para>Filter, which is used to produce first image for the merge. The filter
        /// needs to implement <see cref="IFilterInformation"/> interface, so it could be possible
        /// to get information about the filter. The filter must be able to process color 24 bpp
        /// images and produce color 24 bpp or grayscale 8 bppp images as result.</para>
        /// </remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">The specified filter does not support 24 bpp color images.</exception>
        /// <exception cref="UnsupportedImageFormatException">The specified filter does not produce image of supported format.</exception>
        /// <exception cref="ArgumentException">The specified filter does not implement IFilterInformation interface.</exception>
        /// 
        public IFilter Filter1
        {
            get { return filter1; }
            set
            {
                if ( value is IFilterInformation )
                {
                    IFilterInformation info = (IFilterInformation) value;
                    if ( !info.FormatTranslations.ContainsKey( PixelFormat.Format24bppRgb ) )
                        throw new UnsupportedImageFormatException( "The specified filter does not support 24 bpp color images." );
                    if (
                        ( info.FormatTranslations[PixelFormat.Format24bppRgb] != PixelFormat.Format24bppRgb ) &&
                        ( info.FormatTranslations[PixelFormat.Format24bppRgb] != PixelFormat.Format8bppIndexed ) )
                        throw new UnsupportedImageFormatException( "The specified filter does not produce image of supported format." );
                }
                else
                {
                    throw new ArgumentException( "The specified filter does not implement IFilterInformation interface." );
                }

                filter1 = value;
            }
        }

        /// <summary>
        /// Second filter
        /// </summary>
        /// 
        /// <remarks><para>Filter, which is used to produce second image for the merge. The filter
        /// needs to implement <see cref="IFilterInformation"/> interface, so it could be possible
        /// to get information about the filter. The filter must be able to process color 24 bpp
        /// images and produce color 24 bpp or grayscale 8 bppp images as result.</para>
        /// 
        /// <para><note>The filter may be set to <see langword="null"/>. In this case original source image
        /// is used as a second image for the merge.</note></para>
        /// </remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">The specified filter does not support 24 bpp color images.</exception>
        /// <exception cref="UnsupportedImageFormatException">The specified filter does not produce image of supported format.</exception>
        /// <exception cref="ArgumentException">The specified filter does not implement IFilterInformation interface.</exception>
        /// 
        public IFilter Filter2
        {
            get { return filter2; }
            set
            {
                if ( value is IFilterInformation )
                {
                    IFilterInformation info = (IFilterInformation) value;
                    if ( !info.FormatTranslations.ContainsKey( PixelFormat.Format24bppRgb ) )
                        throw new UnsupportedImageFormatException( "The specified filter does not support 24 bpp color images." );
                    if (
                        ( info.FormatTranslations[PixelFormat.Format24bppRgb] != PixelFormat.Format24bppRgb ) &&
                        ( info.FormatTranslations[PixelFormat.Format24bppRgb] != PixelFormat.Format8bppIndexed ) )
                        throw new UnsupportedImageFormatException( "The specified filter does not produce image of supported format." );
                }
                else
                {
                    throw new ArgumentException( "The specified filter does not implement IFilterInformation interface." );
                }

                filter2 = value;
            }
        }

        // Private constructor to 
        private TexturedFilter( )
        {
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TexturedFilter"/> class.
        /// </summary>
        /// 
        /// <param name="texture">Generated texture.</param>
        /// <param name="filter1">First filter.</param>
        /// 
        public TexturedFilter( float[,] texture, IFilter filter1 ) : this( )
        {
            this.texture = texture;
            this.filter1 = filter1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TexturedFilter"/> class.
        /// </summary>
        /// 
        /// <param name="texture">Generated texture.</param>
        /// <param name="filter1">First filter.</param>
        /// <param name="filter2">Second filter.</param>
        /// 
        public TexturedFilter( float[,] texture, IFilter filter1, IFilter filter2 ) : this( )
        {
            this.texture = texture;
            this.filter1 = filter1;
            this.filter2 = filter2;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TexturedFilter"/> class.
        /// </summary>
        /// 
        /// <param name="generator">Texture generator.</param>
        /// <param name="filter1">First filter.</param>
        /// 
        public TexturedFilter( ITextureGenerator generator, IFilter filter1 ) : this( )
        {
            this.textureGenerator = generator;
            this.filter1 = filter1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TexturedFilter"/> class.
        /// </summary>
        /// 
        /// <param name="generator">Texture generator.</param>
        /// <param name="filter1">First filter.</param>
        /// <param name="filter2">Second filter.</param>
        /// 
        public TexturedFilter( ITextureGenerator generator, IFilter filter1, IFilter filter2 ) : this( )
        {
            this.textureGenerator = generator;
            this.filter1 = filter1;
            this.filter2 = filter2;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// 
        /// <exception cref="InvalidImagePropertiesException">Texture size does not match image size.</exception>
        /// <exception cref="ApplicationException">Filters should not change image dimension.</exception>
        /// 
        protected override unsafe void ProcessFilter( UnmanagedImage sourceData, UnmanagedImage destinationData )
        {
            // get source image dimension
            int width  = sourceData.Width;
            int height = sourceData.Height;

            // if generator was specified, then generate a texture
            // otherwise use provided texture
            if ( textureGenerator != null )
            {
                texture = textureGenerator.Generate( width, height );
            }
            else
            {
                // check existing texture
                if ( ( texture.GetLength( 0 ) != height ) || ( texture.GetLength( 1 ) != width ) )
                {
                    // sorry, but source image must have the same dimension as texture
                    throw new InvalidImagePropertiesException( "Texture size does not match image size." );
                }
            }

            // apply first filter
            UnmanagedImage filteredImage1 = filter1.Apply( sourceData );

            // check size of the result image
            if ( ( width != filteredImage1.Width ) || ( height != filteredImage1.Height ) )
            {
                filteredImage1.Dispose( );
                throw new ApplicationException( "Filters should not change image dimension." );
            }

            // convert 1st image to RGB if required
            if ( filteredImage1.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                GrayscaleToRGB coloringFilter = new GrayscaleToRGB( );
                UnmanagedImage temp = coloringFilter.Apply( filteredImage1 );
                filteredImage1.Dispose( );
                filteredImage1 = temp;
            }

            UnmanagedImage filteredImage2 = null;
            // apply second filter, if it was specified
            if ( filter2 != null )
            {
                filteredImage2 = filter2.Apply( sourceData );
                // check size of the result image
                if ( ( width != filteredImage2.Width ) || ( height != filteredImage2.Height ) )
                {
                    filteredImage1.Dispose( );
                    filteredImage2.Dispose( );
                    // we are not handling such situations yet
                    throw new ApplicationException( "Filters should not change image dimension." );
                }

                // convert 2nd image to RGB if required
                if ( filteredImage2.PixelFormat == PixelFormat.Format8bppIndexed )
                {
                    GrayscaleToRGB coloringFilter = new GrayscaleToRGB( );
                    UnmanagedImage temp = coloringFilter.Apply( filteredImage2 );
                    filteredImage2.Dispose( );
                    filteredImage2 = temp;
                }
            }

            // use source image as a second image, if second filter is not set
            if ( filteredImage2 == null )
            {
                filteredImage2 = sourceData;
            }

            // do the job
            unsafe
            {
                byte* dst = (byte*) destinationData.ImageData.ToPointer( );
                byte* src1 = (byte*) filteredImage1.ImageData.ToPointer( );
                byte* src2 = (byte*) filteredImage2.ImageData.ToPointer( );

                int dstOffset  = destinationData.Stride - 3 * width;
                int src1Offset = filteredImage1.Stride  - 3 * width;
                int src2Offset = filteredImage2.Stride  - 3 * width;

                if ( preserveLevel != 0.0 )
                {
                    // for each line
                    for ( int y = 0; y < height; y++ )
                    {
                        // for each pixel
                        for ( int x = 0; x < width; x++ )
                        {
                            double t1 = texture[y, x];
                            double t2 = 1 - t1;

                            for ( int i = 0; i < 3; i++, src1++, src2++, dst++ )
                            {
                                *dst = (byte) Math.Min( 255.0f,
                                    filterLevel * ( t1 * ( *src1 ) + t2 * ( *src2 ) ) +
                                    preserveLevel * ( *src2 ) );
                            }
                        }
                        src1 += src1Offset;
                        src2 += src2Offset;
                        dst  += dstOffset;
                    }
                }
                else
                {
                    // for each line
                    for ( int y = 0; y < height; y++ )
                    {
                        // for each pixel
                        for ( int x = 0; x < width; x++ )
                        {
                            double t1 = texture[y, x];
                            double t2 = 1 - t1;

                            for ( int i = 0; i < 3; i++, src1++, src2++, dst++ )
                            {
                                *dst = (byte) Math.Min( 255.0f, t1 * *src1 + t2 * *src2 );
                            }
                        }
                        src1 += src1Offset;
                        src2 += src2Offset;
                        dst  += dstOffset;
                    }
                }
            }

            // dispose temp images
            filteredImage1.Dispose( );
            if ( filteredImage2 != sourceData )
            {
                filteredImage2.Dispose( );
            }
        }
    }
}
