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

    /// <summary>
    /// Color dithering with a thresold matrix (ordered dithering).
    /// </summary>
    /// 
    /// <remarks><para>The class implements ordered color dithering as described on
    /// <a href="http://en.wikipedia.org/wiki/Ordered_dithering">Wikipedia</a>.
    /// The algorithm achieves dithering by applying a <see cref="ThresholdMatrix">threshold map</see> on
    /// the pixels displayed, causing some of the pixels to be rendered at a different color, depending on
    /// how far in between the color is of available <see cref="ColorTable">color entries</see>.</para>
    /// 
    /// <para>The image processing routine accepts 24/32 bpp color images for processing. As a result this routine
    /// produces 4 bpp or 8 bpp indexed image, which depends on size of the specified
    /// <see cref="ColorTable">color table</see> - 4 bpp result for
    /// color tables with 16 colors or less; 8 bpp result for larger color tables.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create color image quantization routine
    /// ColorImageQuantizer ciq = new ColorImageQuantizer( new MedianCutQuantizer( ) );
    /// // create 256 colors table
    /// Color[] colorTable = ciq.CalculatePalette( image, 256 );
    /// // create dithering routine
    /// OrderedColorDithering dithering = new OrderedColorDithering( );
    /// dithering.ColorTable = colorTable;
    /// // apply the dithering routine
    /// Bitmap newImage = dithering.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/ordered_color_dithering.png" width="480" height="361" />
    /// </remarks>
    /// 
    public class OrderedColorDithering
    {
        private bool useCaching = false;

        private Color[] colorTable = new Color[16]
        {
            Color.Black,   Color.DarkBlue,    Color.DarkGreen, Color.DarkCyan,
            Color.DarkRed, Color.DarkMagenta, Color.DarkKhaki, Color.LightGray,
            Color.Gray,    Color.Blue,        Color.Green,     Color.Cyan,
            Color.Red,     Color.Magenta,     Color.Yellow,    Color.White
        };

        private byte[,] matrix = new byte[4, 4]
		{
			{  2, 18,  6, 22 },
			{ 26, 10, 30, 14 },
			{  8, 24,  4, 20 },
			{ 32, 16, 28, 12 }
		};

        /// <summary>
        /// Threshold matrix - values to add source image's values.
        /// </summary>
        /// 
        /// <remarks><para>The property keeps a threshold matrix, which is applied to values of a source image
        /// to dither. By adding these values to the source image the algorithm produces the effect when pixels
        /// of the same color in source image may have different color in the result image (which depends on pixel's
        /// position). This threshold map is also known as an index matrix or Bayer matrix.</para>
        /// 
        /// <para>By default the property is inialized with the below matrix:
        /// <code lang="none">
        ///  2   18    6   22
        /// 26   10   30   14
        ///  8   24    4   20
        /// 32   16   28   12
        /// </code>
        /// </para>
        /// </remarks>
        /// 
        public byte[,] ThresholdMatrix
        {
            get { return (byte[,]) matrix.Clone( ); }
            set
            {
                if ( value == null )
                {
                    throw new NullReferenceException( "Threshold matrix cannot be set to null." );
                }
                matrix = value;
            }
        }

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
        /// Initializes a new instance of the <see cref="OrderedColorDithering"/> class.
        /// </summary>
        /// 
        public OrderedColorDithering( )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedColorDithering"/> class.
        /// </summary>
        /// 
        /// <param name="matrix">Threshold matrix (see <see cref="ThresholdMatrix"/> property).</param>
        /// 
        public OrderedColorDithering( byte[,] matrix )
        {
            ThresholdMatrix = matrix;
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

            // get image size
            int width  = sourceImage.Width;
            int height = sourceImage.Height;
            int stride = sourceImage.Stride;
            int pixelSize = Bitmap.GetPixelFormatSize( sourceImage.PixelFormat ) / 8;

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
            int r, g, b, toAdd;
            int rows = matrix.GetLength( 0 );
            int cols = matrix.GetLength( 1 );


            // do the job
            byte* ptr = (byte*) sourceImage.ImageData.ToPointer( );
            byte* dstBase = (byte*) destData.Scan0.ToPointer( );
            byte colorIndex;

            bool is8bpp = ( colorTable.Length > 16 );

            // for each line
            for ( int y = 0; y < height; y++ )
            {
                byte* dst = dstBase + y * destData.Stride;

                // for each pixels
                for ( int x = 0; x < width; x++, ptr += pixelSize )
                {
                    toAdd = matrix[( y % rows ), ( x % cols )];
                    r = ptr[RGB.R] + toAdd;
                    g = ptr[RGB.G] + toAdd;
                    b = ptr[RGB.B] + toAdd;

                    if ( r > 255 )
                        r = 255;
                    if ( g > 255 )
                        g = 255;
                    if ( b > 255 )
                        b = 255;

                    // get color from palette, which is the closest to current pixel's value
                    Color closestColor = GetClosestColor( r, g, b, out colorIndex );

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
            }

            destImage.UnlockBits( destData );

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
