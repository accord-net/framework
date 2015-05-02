// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright ©
//   Mladen Prajdic  (spirit1_fe@yahoo.com),
//   Andrew Kirillov (andrew.kirillov@aforgenet.com)
// 2005-2009
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Flat field correction filter.
    /// </summary>
    /// 
    /// <remarks><para>The goal of flat-field correction is to remove artifacts from 2-D images that
    /// are caused by variations in the pixel-to-pixel sensitivity of the detector and/or by distortions
    /// in the optical path. The filter requires two images for the input - source image, which represents
    /// acquisition of some objects (using microscope, for example), and background image, which is taken
    /// without any objects presented. The source image is corrected using the formula: <b>src = bgMean * src / bg</b>,
    /// where <b>src</b> - source image's pixel value, <b>bg</b> - background image's pixel value, <b>bgMean</b> - mean
    /// value of background image.</para>
    /// 
    /// <para><note>If background image is not provided, then it will be automatically generated on each filter run
    /// from source image. The automatically generated background image is produced running Gaussian Blur on the
    /// original image with (sigma value is set to 5, kernel size is set to 21). Before blurring the original image
    /// is resized to 1/3 of its original size and then the result of blurring is resized back to the original size.
    /// </note></para>
    /// 
    /// <para><note>The class processes only grayscale (8 bpp indexed) and color (24 bpp) images.</note></para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// FlatFieldCorrection filter = new FlatFieldCorrection( bgImage );
    /// // process image
    /// filter.ApplyInPlace( sourceImage );
    /// </code>
    /// 
    /// <para><b>Source image:</b></para>
    /// <img src="img/imaging/sample4.jpg" width="480" height="387" />
    /// <para><b>Background image:</b></para>
    /// <img src="img/imaging/bg.jpg" width="480" height="387" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/flat_field.jpg" width="480" height="387" />
    /// </remarks>
    /// 
    public class FlatFieldCorrection : BaseInPlaceFilter
    {
        Bitmap backgroundImage = null;
        UnmanagedImage unmanagedBackgroundImage = null;

        /// <summary>
        /// Background image used for flat field correction.
        /// </summary>
        /// 
        /// <remarks><para>The property sets the background image (without any objects), which will be used
        /// for illumination correction of an image passed to the filter.</para>
        /// 
        /// <para><note>The background image must have the same size and pixel format as source image.
        /// Otherwise exception will be generated when filter is applied to source image.</note></para>
        /// 
        /// <para><note>Setting this property will clear the <see cref="UnmanagedBackgoundImage"/> property -
        /// only one background image is allowed: managed or unmanaged.</note></para>
        /// </remarks>
        /// 
        public Bitmap BackgoundImage
        {
            get { return backgroundImage; }
            set
            {
                backgroundImage = value;

                if ( value != null )
                    unmanagedBackgroundImage = null;
            }
        }

        /// <summary>
        /// Background image used for flat field correction.
        /// </summary>
        /// 
        /// <remarks><para>The property sets the background image (without any objects), which will be used
        /// for illumination correction of an image passed to the filter.</para>
        /// 
        /// <para><note>The background image must have the same size and pixel format as source image.
        /// Otherwise exception will be generated when filter is applied to source image.</note></para>
        /// 
        /// <para><note>Setting this property will clear the <see cref="BackgoundImage"/> property -
        /// only one background image is allowed: managed or unmanaged.</note></para>
        /// </remarks>
        /// 
        public UnmanagedImage UnmanagedBackgoundImage
        {
            get { return unmanagedBackgroundImage; }
            set
            {
                unmanagedBackgroundImage = value;

                if ( value != null )
                    backgroundImage = null;
            }
        }

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
        /// Initializes a new instance of the <see cref="FlatFieldCorrection"/> class.
        /// </summary>
        /// 
        /// <remarks><para>This constructor does not set background image, which means that background
        /// image will be generated on the fly on each filter run. The automatically generated background
        /// image is produced running Gaussian Blur on the original image with (sigma value is set to 5,
        /// kernel size is set to 21). Before blurring the original image is resized to 1/3 of its original size
        /// and then the result of blurring is resized back to the original size.</para></remarks>
        /// 
        public FlatFieldCorrection( )
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlatFieldCorrection"/> class.
        /// </summary>
        /// 
        /// <param name="backgroundImage">Background image used for flat field correction.</param>
        /// 
        public FlatFieldCorrection( Bitmap backgroundImage ) : this( )
        {
            this.backgroundImage = backgroundImage;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        ///
        protected override unsafe void ProcessFilter( UnmanagedImage image )
        {
            UnmanagedImage bgImage = null;
            BitmapData bgLockedData = null;

            // get image size
            int width  = image.Width;
            int height = image.Height;
            int offset = image.Stride - ( ( image.PixelFormat == PixelFormat.Format8bppIndexed ) ? width : width * 3 );
            
            // check if we have provided background
            if ( ( backgroundImage == null ) && ( unmanagedBackgroundImage == null ) )
            {
                // resize image to 1/3 of its original size to make bluring faster
                ResizeBicubic resizeFilter = new ResizeBicubic( (int) width / 3, (int) height / 3 );
                UnmanagedImage tempImage = resizeFilter.Apply( image );

                // create background image from the input image blurring it with Gaussian 5 times
                GaussianBlur blur = new GaussianBlur( 5, 21 );

                blur.ApplyInPlace( tempImage );
                blur.ApplyInPlace( tempImage );
                blur.ApplyInPlace( tempImage );
                blur.ApplyInPlace( tempImage );
                blur.ApplyInPlace( tempImage );

                // resize the blurred image back to original size
                resizeFilter.NewWidth  = width;
                resizeFilter.NewHeight = height;
                bgImage = resizeFilter.Apply( tempImage );

                tempImage.Dispose( );
            }
            else
            {
                if ( backgroundImage != null )
                {
                    // check background image
                    if ( ( width != backgroundImage.Width ) || ( height != backgroundImage.Height ) || ( image.PixelFormat != backgroundImage.PixelFormat ) )
                    {
                        throw new InvalidImagePropertiesException( "Source image and background images must have the same size and pixel format" );
                    }

                    // lock background image
                    bgLockedData = backgroundImage.LockBits(
                        new Rectangle( 0, 0, width, height ),
                        ImageLockMode.ReadOnly, backgroundImage.PixelFormat );

                    bgImage = new UnmanagedImage( bgLockedData );
                }
                else
                {
                    bgImage = unmanagedBackgroundImage;
                }
            }

            // get background image's statistics (mean value is used as correction factor)
            ImageStatistics bgStatistics = new ImageStatistics( bgImage );

            byte* src = (byte*) image.ImageData.ToPointer( );
            byte* bg  = (byte*) bgImage.ImageData.ToPointer( );

            // do the job
            if ( image.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // grayscale image
                double mean = bgStatistics.Gray.Mean;

                for ( int y = 0; y < height; y++ )
                {
                    for ( int x = 0; x < width; x++, src++, bg++ )
                    {
                        if ( *bg != 0 )
                        {
                            *src = (byte) Math.Min( mean * *src / *bg, 255 );
                        }
                    }
                    src += offset;
                    bg  += offset;
                }
            }
            else
            {
                // color image
                double meanR = bgStatistics.Red.Mean;
                double meanG = bgStatistics.Green.Mean;
                double meanB = bgStatistics.Blue.Mean;

                for ( int y = 0; y < height; y++ )
                {
                    for ( int x = 0; x < width; x++, src += 3, bg += 3 )
                    {
                        // red
                        if ( bg[RGB.R] != 0 )
                        {
                            src[RGB.R] = (byte) Math.Min( meanR * src[RGB.R] / bg[RGB.R], 255 );
                        }
                        // green
                        if ( bg[RGB.G] != 0 )
                        {
                            src[RGB.G] = (byte) Math.Min( meanG * src[RGB.G] / bg[RGB.G], 255 );
                        }
                        // blue
                        if ( bg[RGB.B] != 0 )
                        {
                            src[RGB.B] = (byte) Math.Min( meanB * src[RGB.B] / bg[RGB.B], 255 );
                        }
                    }
                    src += offset;
                    bg  += offset;
                }
            }

            if ( backgroundImage != null )
            {
                backgroundImage.UnlockBits( bgLockedData );
            }

            // dispose background image if it was not set manually
            if ( ( backgroundImage == null ) && ( unmanagedBackgroundImage == null ) )
            {
                bgImage.Dispose( );
            }
        }
    }
}
