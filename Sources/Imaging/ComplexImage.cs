// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge;
    using AForge.Math;

    /// <summary>
    /// Complex image.
    /// </summary>
    /// 
    /// <remarks><para>The class is used to keep image represented in complex numbers sutable for Fourier
    /// transformations.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create complex image
    /// ComplexImage complexImage = ComplexImage.FromBitmap( image );
    /// // do forward Fourier transformation
    /// complexImage.ForwardFourierTransform( );
    /// // get complex image as bitmat
    /// Bitmap fourierImage = complexImage.ToBitmap( );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample3.jpg" width="256" height="256" />
    /// <para><b>Fourier image:</b></para>
    /// <img src="img/imaging/fourier.jpg" width="256" height="256" />
    /// </remarks>
    /// 
    public class ComplexImage : ICloneable
    {
        // image complex data
        private Complex[,] data;
        // image dimension
        private int width;
        private int height;
        // current state of the image (transformed with Fourier ot not)
        private bool fourierTransformed = false;

        /// <summary>
        /// Image width.
        /// </summary>
        /// 
        public int Width
        {
            get { return width; }
        }

        /// <summary>
        /// Image height.
        /// </summary>
        /// 
        public int Height
        {
            get { return height; }
        }

        /// <summary>
        /// Status of the image - Fourier transformed or not.
        /// </summary>
        /// 
        public bool FourierTransformed
        {
            get { return fourierTransformed; }
        }

        /// <summary>
        /// Complex image's data.
        /// </summary>
        /// 
        /// <remarks>Return's 2D array of [<b>height</b>, <b>width</b>] size, which keeps image's
        /// complex data.</remarks>
        /// 
        public Complex[,] Data
        {
            get { return data; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexImage"/> class.
        /// </summary>
        /// 
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// 
        /// <remarks>The constractor is protected, what makes it imposible to instantiate this
        /// class directly. To create an instance of this class <see cref="FromBitmap(Bitmap)"/> or
        /// <see cref="FromBitmap(BitmapData)"/> method should be used.</remarks>
        ///
        protected ComplexImage( int width, int height )
        {
            this.width = width;
            this.height = height;
            this.data = new Complex[height, width];
            this.fourierTransformed = false;
        }

        /// <summary>
        /// Clone the complex image.
        /// </summary>
        /// 
        /// <returns>Returns copy of the complex image.</returns>
        /// 
        public object Clone( )
        {
            // create new complex image
            ComplexImage dstImage = new ComplexImage( width, height );
            Complex[,] data = dstImage.data;

            for ( int i = 0; i < height; i++ )
            {
                for ( int j = 0; j < width; j++ )
                {
                    data[i, j] = this.data[i, j];
                }
            }

            // clone mode as well
            dstImage.fourierTransformed = fourierTransformed;

            return dstImage;
        }

        /// <summary>
        /// Create complex image from grayscale bitmap.
        /// </summary>
        /// 
        /// <param name="image">Source grayscale bitmap (8 bpp indexed).</param>
        /// 
        /// <returns>Returns an instance of complex image.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
        /// <exception cref="InvalidImagePropertiesException">Image width and height should be power of 2.</exception>
        /// 
        public static ComplexImage FromBitmap( Bitmap image )
        {
            // check image format
            if ( image.PixelFormat != PixelFormat.Format8bppIndexed )
            {
                throw new UnsupportedImageFormatException( "Source image can be graysclae (8bpp indexed) image only." );
            }

            // lock source bitmap data
            BitmapData imageData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );

            ComplexImage complexImage;

            try
            {
                complexImage = FromBitmap( imageData );
            }
            finally
            {
                // unlock source images
                image.UnlockBits( imageData );
            }

            return complexImage;
        }

        /// <summary>
        /// Create complex image from grayscale bitmap.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data (8 bpp indexed).</param>
        /// 
        /// <returns>Returns an instance of complex image.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
        /// <exception cref="InvalidImagePropertiesException">Image width and height should be power of 2.</exception>
        /// 
        public static ComplexImage FromBitmap( BitmapData imageData )
        {
            // check image format
            if ( imageData.PixelFormat != PixelFormat.Format8bppIndexed )
            {
                throw new UnsupportedImageFormatException( "Source image can be graysclae (8bpp indexed) image only." );
            }

            // get source image size
            int width  = imageData.Width;
            int height = imageData.Height;
            int offset = imageData.Stride - width;

            // check image size
            if ( ( !Tools.IsPowerOf2( width ) ) || ( !Tools.IsPowerOf2( height ) ) )
            {
                throw new InvalidImagePropertiesException( "Image width and height should be power of 2." );
            }

            // create new complex image
            ComplexImage complexImage = new ComplexImage( width, height );
            Complex[,] data = complexImage.data;

            // do the job
            unsafe
            {
                byte* src = (byte*) imageData.Scan0.ToPointer( );

                // for each line
                for ( int y = 0; y < height; y++ )
                {
                    // for each pixel
                    for ( int x = 0; x < width; x++, src++ )
                    {
                        data[y, x].Re = (float) *src / 255;
                    }
                    src += offset;
                }
            }

            return complexImage;
        }

        /// <summary>
        /// Convert complex image to bitmap.
        /// </summary>
        /// 
        /// <returns>Returns grayscale bitmap.</returns>
        /// 
        public Bitmap ToBitmap( )
        {
            // create new image
            Bitmap dstImage = AForge.Imaging.Image.CreateGrayscaleImage( width, height );

            // lock destination bitmap data
            BitmapData dstData = dstImage.LockBits(
                new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed );

            int offset = dstData.Stride - width;
            double scale = ( fourierTransformed ) ? Math.Sqrt( width * height ) : 1;

            // do the job
            unsafe
            {
                byte* dst = (byte*) dstData.Scan0.ToPointer( );

                for ( int y = 0; y < height; y++ )
                {
                    for ( int x = 0; x < width; x++, dst++ )
                    {
                        *dst = (byte) System.Math.Max( 0, System.Math.Min( 255, data[y, x].Magnitude * scale * 255 ) );
                    }
                    dst += offset;
                }
            }
            // unlock destination images
            dstImage.UnlockBits( dstData );

            return dstImage;
        }

        /// <summary>
        /// Applies forward fast Fourier transformation to the complex image.
        /// </summary>
        /// 
        public void ForwardFourierTransform( )
        {
            if ( !fourierTransformed )
            {
                for ( int y = 0; y < height; y++ )
                {
                    for ( int x = 0; x < width; x++ )
                    {
                        if ( ( ( x + y ) & 0x1 ) != 0 )
                        {
                            data[y, x].Re *= -1;
                            data[y, x].Im *= -1;
                        }
                    }
                }

                FourierTransform.FFT2( data, FourierTransform.Direction.Forward );
                fourierTransformed = true;
            }
        }

        /// <summary>
        /// Applies backward fast Fourier transformation to the complex image.
        /// </summary>
        /// 
        public void BackwardFourierTransform( )
        {
            if ( fourierTransformed )
            {
                FourierTransform.FFT2( data, FourierTransform.Direction.Backward );
                fourierTransformed = false;

                for ( int y = 0; y < height; y++ )
                {
                    for ( int x = 0; x < width; x++ )
                    {
                        if ( ( ( x + y ) & 0x1 ) != 0 )
                        {
                            data[y, x].Re *= -1;
                            data[y, x].Im *= -1;
                        }
                    }
                }
            }
        }
    }
}
