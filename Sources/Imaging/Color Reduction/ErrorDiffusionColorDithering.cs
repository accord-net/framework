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
    /// Base class for error diffusion color dithering.
    /// </summary>
    /// 
    /// <remarks><para>The class is the base class for color dithering algorithms based on
    /// <a href="http://en.wikipedia.org/wiki/Error_diffusion">error diffusion</a>.</para>
    /// 
    /// <para>Color dithering with error diffusion is based on the idea that each pixel from the specified source
    /// image is substituted with a best matching color (or better say with color's index) from the specified color
    /// table. However, the error (difference between color value in the source image and the best matching color)
    /// is diffused to neighbor pixels of the source image, which affects the way those pixels are substituted by colors
    /// from the specified table.</para>
    /// 
    /// <para>The image processing routine accepts 24/32 bpp color images for processing. As a result this routine
    /// produces 4 bpp or 8 bpp indexed image, which depends on size of the specified <see cref="ColorTable">color table</see> - 4 bpp result for
    /// color tables with 16 colors or less; 8 bpp result for larger color tables.</para>
    /// </remarks>
    /// 
    public abstract class ErrorDiffusionColorDithering
    {
        private bool useCaching = false;

        /// <summary>
        /// Current processing X coordinate.
        /// </summary>
        protected int x;

        /// <summary>
        /// Current processing Y coordinate.
        /// </summary>
        protected int y;

        /// <summary>
        /// Processing image's width.
        /// </summary>
        protected int width;

        /// <summary>
        /// Processing image's height.
        /// </summary>
        protected int height;

        /// <summary>
        /// Processing image's stride (line size).
        /// </summary>
        protected int stride;

        /// <summary>
        /// Processing image's pixel size in bytes.
        /// </summary>
        protected int pixelSize;

        private Color[] colorTable = new Color[16]
        {
            Color.Black,   Color.DarkBlue,    Color.DarkGreen, Color.DarkCyan,
            Color.DarkRed, Color.DarkMagenta, Color.DarkKhaki, Color.LightGray,
            Color.Gray,    Color.Blue,        Color.Green,     Color.Cyan,
            Color.Red,     Color.Magenta,     Color.Yellow,    Color.White
        };

        /// <summary>
        /// Color table to use for image dithering. Must contain 2-256 colors.
        /// </summary>
        /// 
        /// <remarks><para>Color table size determines format of the resulting image produced by this
        /// image processing routine. If color table contains 16 color or less, then result image will have
        /// 4 bpp indexed pixel format. If color table contains more than 16 colors, then result image will
        /// have 8 bpp indexed pixel format.</para>
        /// 
        /// <para>By default the property is initialized with default 16 colors, which are:
        /// Black, Dark Blue, Dark Green, Dark Cyan, Dark Red, Dark Magenta, Dark Khaki, Light Gray,
        /// Gray, Blue, Green, Cyan, Red, Magenta, Yellow and White.</para>
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">Color table length must be in the [2, 256] range.</exception>
        /// 
        public Color[] ColorTable
        {
            get { return colorTable; }
            set
            {
                if ( ( colorTable.Length < 2 ) || ( colorTable.Length > 256 ) )
                    throw new ArgumentException( "Color table length must be in the [2, 256] range." );

                colorTable = value;
            }
        }

        /// <summary>
        /// Use color caching during color dithering or not.
        /// </summary>
        /// 
        /// <remarks><para>The property  specifies if internal cache of already processed colors should be used or not.
        /// For each pixel in the original image the color dithering routine does search in target color palette to find
        /// the best matching color. To avoid doing the search again and again for already processed colors, the class may
        /// use internal dictionary which maps colors of original image to indexes in target color palette.
        /// </para>
        /// 
        /// <para><note>The property provides a trade off. On one hand it may speedup color dithering routine, but on another
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
        /// Initializes a new instance of the <see cref="ErrorDiffusionColorDithering"/> class.
        /// </summary>
        /// 
        protected ErrorDiffusionColorDithering( )
        {
        }

        /// <summary>
        /// Do error diffusion.
        /// </summary>
        /// 
        /// <param name="rError">Error value of red component.</param>
        /// <param name="gError">Error value of green component.</param>
        /// <param name="bError">Error value of blue component.</param>
        /// <param name="ptr">Pointer to current processing pixel.</param>
        /// 
        /// <remarks>All parameters of the image and current processing pixel's coordinates
        /// are initialized in protected members.</remarks>
        /// 
        protected abstract unsafe void Diffuse( int rError, int gError, int bError, byte* ptr );


        /// <summary>
        /// Perform color dithering for the specified image.
        /// </summary>
        /// 
        /// <param name="sourceImage">Source image to do color dithering for.</param>
        /// 
        /// <returns>Returns color dithered image. See <see cref="ColorTable"/> for information about format of
        /// the result image.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image. It must 24 or 32 bpp color image.</exception>
        /// 
        public Bitmap Apply( Bitmap sourceImage )
        {
            BitmapData data = sourceImage.LockBits( new Rectangle( 0, 0, sourceImage.Width, sourceImage.Height ),
                ImageLockMode.ReadOnly, sourceImage.PixelFormat );

            Bitmap result = null;

            try
            {
                result = Apply( new UnmanagedImage( data ) );
                if ( ( sourceImage.HorizontalResolution > 0 ) && ( sourceImage.VerticalResolution > 0 ) )
                {
                    result.SetResolution( sourceImage.HorizontalResolution, sourceImage.VerticalResolution );
                }
            }
            finally
            {
                sourceImage.UnlockBits( data );
            }

            return result;
        }

        /// <summary>
        /// Perform color dithering for the specified image.
        /// </summary>
        /// 
        /// <param name="sourceImage">Source image to do color dithering for.</param>
        /// 
        /// <returns>Returns color dithered image. See <see cref="ColorTable"/> for information about format of
        /// the result image.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image. It must 24 or 32 bpp color image.</exception>
        /// 
        public unsafe Bitmap Apply( UnmanagedImage sourceImage )
        {
            if ( ( sourceImage.PixelFormat != PixelFormat.Format24bppRgb ) &&
                 ( sourceImage.PixelFormat != PixelFormat.Format32bppRgb ) &&
                 ( sourceImage.PixelFormat != PixelFormat.Format32bppArgb ) &&
                 ( sourceImage.PixelFormat != PixelFormat.Format32bppPArgb ) )
            {
                throw new UnsupportedImageFormatException( "Unsupported pixel format of the source image." );
            }

            cache.Clear( );

            // make a copy of the original image
            UnmanagedImage source = sourceImage.Clone( );

            // get image size
            width  = sourceImage.Width;
            height = sourceImage.Height;
            stride = sourceImage.Stride;
            pixelSize = Bitmap.GetPixelFormatSize( sourceImage.PixelFormat ) / 8;

            int offset = stride - width * pixelSize;

            // create destination image
            Bitmap destImage = new Bitmap( width, height, ( colorTable.Length > 16 ) ?
                PixelFormat.Format8bppIndexed : PixelFormat.Format4bppIndexed );
            // and init its palette
            ColorPalette cp = destImage.Palette;
            for ( int i = 0, n = colorTable.Length; i < n; i++ )
            {
                cp.Entries[i] = colorTable[i];
            }
            destImage.Palette = cp;

            // lock destination image
            BitmapData destData = destImage.LockBits( new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadWrite, destImage.PixelFormat );

            // pixel values
            int r, g, b;

            // do the job
            byte* ptr = (byte*) source.ImageData.ToPointer( );
            byte* dstBase = (byte*) destData.Scan0.ToPointer( );
            byte colorIndex;

            bool is8bpp = ( colorTable.Length > 16 );

            // for each line
            for ( y = 0; y < height; y++ )
            {
                byte* dst = dstBase + y * destData.Stride;

                // for each pixels
                for ( x = 0; x < width; x++, ptr += pixelSize )
                {
                    r = ptr[RGB.R];
                    g = ptr[RGB.G];
                    b = ptr[RGB.B];

                    // get color from palette, which is the closest to current pixel's value
                    Color closestColor = GetClosestColor( r, g, b, out colorIndex );

                    // do error diffusion
                    Diffuse( r - closestColor.R, g - closestColor.G, b - closestColor.B, ptr );

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

            destImage.UnlockBits( destData );
            source.Dispose( );

            return destImage;
        }

        [NonSerialized]
        private Dictionary<Color, byte> cache = new Dictionary<Color, byte>( );

        // Get closest color from palette to the specified color
        private Color GetClosestColor( int red, int green, int blue, out byte colorIndex )
        {
            Color color = Color.FromArgb( red, green, blue );

            if ( ( useCaching ) && ( cache.ContainsKey( color ) ) )
            {
                colorIndex = cache[color];
            }
            else
            {
                colorIndex = 0;
                int minError = int.MaxValue;

                for ( int i = 0, n = colorTable.Length; i < n; i++ )
                {
                    int dr = red - colorTable[i].R;
                    int dg = green - colorTable[i].G;
                    int db = blue - colorTable[i].B;

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
            }

            return colorTable[colorIndex];
        }
    }
}
