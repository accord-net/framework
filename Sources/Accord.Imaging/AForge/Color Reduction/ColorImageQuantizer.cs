// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace AForge.Imaging.ColorReduction
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge.Imaging;

    /// <summary>
    /// Color quantization tools.
    /// </summary>
    ///
    /// <remarks><para>The class contains methods aimed to simplify work with color quantization
    /// algorithms implementing <see cref="IColorQuantizer"/> interface. Using its methods it is possible
    /// to calculate reduced color palette for the specified image or reduce colors to the specified number.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // instantiate the images' color quantization class
    /// ColorImageQuantizer ciq = new ColorImageQuantizer( new MedianCutQuantizer( ) );
    /// // get 16 color palette for a given image
    /// Color[] colorTable = ciq.CalculatePalette( image, 16 );
    /// 
    /// // ... or just reduce colors in the specified image
    /// Bitmap newImage = ciq.ReduceColors( image, 16 );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/reduced_colors.png" width="480" height="361" />
    /// </remarks>
    ///
    public class ColorImageQuantizer
    {
        private IColorQuantizer quantizer;
        private bool useCaching = false;

        /// <summary>
        /// Color quantization algorithm used by this class to build color palettes for the specified images.
        /// </summary>
        /// 
        public IColorQuantizer Quantizer
        {
            get { return quantizer; }
            set { quantizer = value; }
        }

        /// <summary>
        /// Use color caching during color reduction or not.
        /// </summary>
        /// 
        /// <remarks><para>The property has effect only for methods like <see cref="ReduceColors(Bitmap, int)"/> and
        /// specifies if internal cache of already processed colors should be used or not. For each pixel in the original
        /// image the color reduction routine does search in target color palette to find the best matching color.
        /// To avoid doing the search again and again for already processed colors, the class may use internal dictionary
        /// which maps colors of original image to indexes in target color palette.
        /// </para>
        /// 
        /// <para><note>The property provides a trade off. On one hand it may speedup color reduction routine, but on another
        /// hand it increases memory usage. Also cache usage may not be efficient for very small target color tables.</note></para>
        /// 
        /// <para>Default value is set to <see langword="false"/>.</para>
        /// </remarks>
        /// 
        public bool UseCaching
        {
            get { return useCaching; }
            set { useCaching = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorImageQuantizer"/> class.
        /// </summary>
        /// 
        /// <param name="quantizer">Color quantization algorithm to use for processing images.</param>
        /// 
        public ColorImageQuantizer( IColorQuantizer quantizer )
        {
            this.quantizer = quantizer;
        }

        /// <summary>
        /// Calculate reduced color palette for the specified image.
        /// </summary>
        /// 
        /// <param name="image">Image to calculate palette for.</param>
        /// <param name="paletteSize">Palette size to calculate.</param>
        /// 
        /// <returns>Return reduced color palette for the specified image.</returns>
        /// 
        /// <remarks><para>See <see cref="CalculatePalette(UnmanagedImage, int)"/> for details.</para></remarks>
        /// 
        public Color[] CalculatePalette( Bitmap image, int paletteSize )
        {
            BitmapData data = image.LockBits( new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, image.PixelFormat );

            try
            {
                return CalculatePalette( new UnmanagedImage( data ), paletteSize );
            }
            finally
            {
                image.UnlockBits( data );
            }
        }

        /// <summary>
        /// Calculate reduced color palette for the specified image.
        /// </summary>
        /// 
        /// <param name="image">Image to calculate palette for.</param>
        /// <param name="paletteSize">Palette size to calculate.</param>
        /// 
        /// <returns>Return reduced color palette for the specified image.</returns>
        /// 
        /// <remarks><para>The method processes the specified image and feeds color value of each pixel
        /// to the specified color quantization algorithm. Finally it returns color palette built by
        /// that algorithm.</para></remarks>
        ///
        /// <exception cref="UnsupportedImageFormatException">Unsupported format of the source image - it must 24 or 32 bpp color image.</exception>
        ///
        public Color[] CalculatePalette( UnmanagedImage image, int paletteSize )
        {
            if ( ( image.PixelFormat != PixelFormat.Format24bppRgb ) &&
                 ( image.PixelFormat != PixelFormat.Format32bppRgb ) &&
                 ( image.PixelFormat != PixelFormat.Format32bppArgb ) &&
                 ( image.PixelFormat != PixelFormat.Format32bppPArgb ) )
            {
                throw new UnsupportedImageFormatException( "Unsupported format of the source image." );
            }

            quantizer.Clear( );

            int width = image.Width;
            int height = image.Height;

            int pixelSize = Bitmap.GetPixelFormatSize( image.PixelFormat ) / 8;

            unsafe
            {
                byte* ptr = (byte*) image.ImageData.ToPointer( );
                int offset = image.Stride - width * pixelSize;

                for ( int y = 0; y < height; y++ )
                {
                    for ( int x = 0; x < width; x++, ptr += pixelSize )
                    {
                        quantizer.AddColor( Color.FromArgb( ptr[RGB.R], ptr[RGB.G], ptr[RGB.B] ) );
                    }

                    ptr += offset;
                }
            }

            return quantizer.GetPalette( paletteSize );
        }

        /// <summary>
        /// Create an image with reduced number of colors.
        /// </summary>
        /// 
        /// <param name="image">Source image to process.</param>
        /// <param name="paletteSize">Number of colors to get in the output image, [2, 256].</param>
        /// 
        /// <returns>Returns image with reduced number of colors.</returns>
        /// 
        /// <remarks><para>See <see cref="ReduceColors(UnmanagedImage, int)"/> for details.</para></remarks>
        /// 
        public Bitmap ReduceColors( Bitmap image, int paletteSize )
        {
            BitmapData data = image.LockBits( new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, image.PixelFormat );

            try
            {
                Bitmap result = ReduceColors( new UnmanagedImage( data ), paletteSize );
                if ( ( image.HorizontalResolution > 0 ) && ( image.VerticalResolution > 0 ) )
                {
                    result.SetResolution( image.HorizontalResolution, image.VerticalResolution );
                }
                return result;
            }
            finally
            {
                image.UnlockBits( data );
            }
        }

        /// <summary>
        /// Create an image with reduced number of colors.
        /// </summary>
        /// 
        /// <param name="image">Source image to process.</param>
        /// <param name="paletteSize">Number of colors to get in the output image, [2, 256].</param>
        /// 
        /// <returns>Returns image with reduced number of colors.</returns>
        /// 
        /// <remarks><para>The method creates an image, which looks similar to the specified image, but contains
        /// reduced number of colors. First, target color palette is calculated using <see cref="CalculatePalette(UnmanagedImage, int)"/>
        /// method and then a new image is created, where pixels from the given source image are substituted by
        /// best matching colors from calculated color table.</para>
        /// 
        /// <para><note>The output image has 4 bpp or 8 bpp indexed pixel format depending on the target palette size -
        /// 4 bpp for palette size 16 or less; 8 bpp otherwise.</note></para>
        /// </remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported format of the source image - it must 24 or 32 bpp color image.</exception>
        /// <exception cref="ArgumentException">Invalid size of the target color palette.</exception>
        /// 
        public Bitmap ReduceColors( UnmanagedImage image, int paletteSize )
        {
            if ( ( paletteSize < 2 ) || ( paletteSize > 256 ) )
            {
                throw new ArgumentException( "Invalid size of the target color palette." );
            }

            return ReduceColors( image, CalculatePalette( image, paletteSize ) );
        }

        /// <summary>
        /// Create an image with reduced number of colors using the specified palette.
        /// </summary>
        /// 
        /// <param name="image">Source image to process.</param>
        /// <param name="palette">Target color palette. Must contatin 2-256 colors.</param>
        /// 
        /// <returns>Returns image with reduced number of colors.</returns>
        /// 
        /// <remarks><para>See <see cref="ReduceColors(UnmanagedImage, Color[])"/> for details.</para></remarks>
        /// 
        public Bitmap ReduceColors( Bitmap image, Color[] palette )
        {
            BitmapData data = image.LockBits( new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, image.PixelFormat );

            try
            {
                Bitmap result = ReduceColors( new UnmanagedImage( data ), palette );
                if ( ( image.HorizontalResolution > 0 ) && ( image.VerticalResolution > 0 ) )
                {
                    result.SetResolution( image.HorizontalResolution, image.VerticalResolution );
                }
                return result;
            }
            finally
            {
                image.UnlockBits( data );
            }
        }

        /// <summary>
        /// Create an image with reduced number of colors using the specified palette.
        /// </summary>
        /// 
        /// <param name="image">Source image to process.</param>
        /// <param name="palette">Target color palette. Must contatin 2-256 colors.</param>
        /// 
        /// <returns>Returns image with reduced number of colors.</returns>
        /// 
        /// <remarks><para>The method creates an image, which looks similar to the specified image, but contains
        /// reduced number of colors. Is substitutes every pixel of the source image with the closest matching color
        /// in the specified paletter.</para>
        /// 
        /// <para><note>The output image has 4 bpp or 8 bpp indexed pixel format depending on the target palette size -
        /// 4 bpp for palette size 16 or less; 8 bpp otherwise.</note></para>
        /// </remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported format of the source image - it must 24 or 32 bpp color image.</exception>
        /// <exception cref="ArgumentException">Invalid size of the target color palette.</exception>
        /// 
        public Bitmap ReduceColors( UnmanagedImage image, Color[] palette )
        {
            if ( ( image.PixelFormat != PixelFormat.Format24bppRgb ) &&
                 ( image.PixelFormat != PixelFormat.Format32bppRgb ) &&
                 ( image.PixelFormat != PixelFormat.Format32bppArgb ) &&
                 ( image.PixelFormat != PixelFormat.Format32bppPArgb ) )
            {
                throw new UnsupportedImageFormatException( "Unsupported format of the source image." );
            }

            if ( ( palette.Length < 2 ) || ( palette.Length > 256 ) )
            {
                throw new ArgumentException( "Invalid size of the target color palette." );
            }

            paletteToUse = palette;
            cache.Clear( );

            // get image size
            int width  = image.Width;
            int height = image.Height;
            int stride = image.Stride;
            int pixelSize = Bitmap.GetPixelFormatSize( image.PixelFormat ) / 8;

            int offset = stride - width * pixelSize;

            // create destination image
            Bitmap destImage = new Bitmap( width, height, ( palette.Length > 16 ) ?
                PixelFormat.Format8bppIndexed : PixelFormat.Format4bppIndexed );
            // and init its palette
            ColorPalette cp = destImage.Palette;
            for ( int i = 0, n = palette.Length; i < n; i++ )
            {
                cp.Entries[i] = palette[i];
            }
            destImage.Palette = cp;

            // lock destination image
            BitmapData destData = destImage.LockBits( new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadWrite, destImage.PixelFormat );

            // do the job
            unsafe
            {
                byte* ptr = (byte*) image.ImageData.ToPointer( );
                byte* dstBase = (byte*) destData.Scan0.ToPointer( );

                bool is8bpp = ( palette.Length > 16 );

                // for each line
                for ( int y = 0; y < height; y++ )
                {
                    byte* dst = dstBase + y * destData.Stride;

                    // for each pixels
                    for ( int x = 0; x < width; x++, ptr += pixelSize )
                    {
                        // get color from palette, which is the closest to current pixel's value
                        byte colorIndex = (byte) GetClosestColor( ptr[RGB.R], ptr[RGB.G], ptr[RGB.B] );

                        // write color index as pixel's value to destination image
                        if ( is8bpp )
                        {
                            *dst = colorIndex;
                            dst++;
                        }
                        else
                        {
                            if ( x % 2 == 0 )
                            {
                                *dst |= (byte) ( colorIndex << 4 );
                            }
                            else
                            {
                                *dst |= ( colorIndex );
                                dst++;
                            }
                        }
                    }
                    ptr += offset;
                }
            }

            destImage.UnlockBits( destData );

            return destImage;
        }

        #region Helper methods
        [NonSerialized]
        private Color[] paletteToUse;
        [NonSerialized]
        private Dictionary<Color, int> cache = new Dictionary<Color, int>( );

        // Get closest color from palette to specified color
        private int GetClosestColor( int red, int green, int blue )
        {
            Color color = Color.FromArgb( red, green, blue );

            if ( ( useCaching ) && ( cache.ContainsKey( color ) ) )
            {
                return cache[color];
            }

            int colorIndex = 0;
            int minError = int.MaxValue;

            for ( int i = 0, n = paletteToUse.Length; i < n; i++ )
            {
                int dr = red - paletteToUse[i].R;
                int dg = green - paletteToUse[i].G;
                int db = blue - paletteToUse[i].B;

                int error = dr * dr + dg * dg + db * db;

                if ( error < minError )
                {
                    minError = error;
                    colorIndex = (byte) i;
                }
            }

            if ( useCaching )
            {
                cache.Add( color, colorIndex );
            }

            return colorIndex;
        }
        #endregion
    }
}
