// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Stereo anaglyph filter.
    /// </summary>
    /// 
    /// <remarks><para>The image processing filter produces stereo anaglyph images which are
    /// aimed to be viewed through anaglyph glasses with red filter over the left eye and
    /// cyan over the right.</para>
    /// 
    /// <img src="img/imaging/anaglyph_glasses.png" width="125" height="97" />
    /// 
    /// <para>The stereo image is produced by combining two images of the same scene taken
    /// from a bit different points. The right image must be provided to the filter using
    /// <see cref="BaseInPlaceFilter2.OverlayImage"/> property, but the left image must be provided to
    /// <see cref="IFilter.Apply(Bitmap)"/> method, which creates the anaglyph image.</para>
    /// 
    /// <para>The filter accepts 24 bpp color images for processing.</para>
    /// 
    /// <para>See <see cref="Algorithm"/> enumeration for the list of supported anaglyph algorithms.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// StereoAnaglyph filter = new StereoAnaglyph( );
    /// // set right image as overlay
    /// filter.Overlay = rightImage
    /// // apply the filter (providing left image)
    /// Bitmap resultImage = filter.Apply( leftImage );
    /// </code>
    /// 
    /// <para><b>Source image (left):</b></para>
    /// <img src="img/imaging/sample16_left.png" width="320" height="240" />
    /// <para><b>Overlay image (right):</b></para>
    /// <img src="img/imaging/sample16_right.png" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/anaglyph.png" width="320" height="240" />
    /// 
    /// </remarks>
    /// 
    public class StereoAnaglyph : BaseInPlaceFilter2
    {
        /// <summary>
        /// Enumeration of algorithms for creating anaglyph images.
        /// </summary>
        /// 
        /// <remarks><para>See <a href="http://www.3dtv.at/Knowhow/AnaglyphComparison_en.aspx">anaglyph methods comparison</a> for
        /// descipton of different algorithms.</para>
        /// </remarks>
        /// 
        public enum Algorithm
        {
            /// <summary>
            /// Creates anaglyph image using the below calculations:
            /// <list type="bullet">
            /// <item>R<sub>a</sub>=0.299*R<sub>l</sub>+0.587*G<sub>l</sub>+0.114*B<sub>l</sub>;</item>
            /// <item>G<sub>a</sub>=0;</item>
            /// <item>B<sub>a</sub>=0.299*R<sub>r</sub>+0.587*G<sub>r</sub>+0.114*B<sub>r</sub>.</item>
            /// </list>
            /// </summary>
            TrueAnaglyph,

            /// <summary>
            /// Creates anaglyph image using the below calculations:
            /// <list type="bullet">
            /// <item>R<sub>a</sub>=0.299*R<sub>l</sub>+0.587*G<sub>l</sub>+0.114*B<sub>l</sub>;</item>
            /// <item>G<sub>a</sub>=0.299*R<sub>r</sub>+0.587*G<sub>r</sub>+0.114*B<sub>r</sub>;</item>
            /// <item>B<sub>a</sub>=0.299*R<sub>r</sub>+0.587*G<sub>r</sub>+0.114*B<sub>r</sub>.</item>
            /// </list>
            /// </summary>
            GrayAnaglyph,

            /// <summary>
            /// Creates anaglyph image using the below calculations:
            /// <list type="bullet">
            /// <item>R<sub>a</sub>=R<sub>l</sub>;</item>
            /// <item>G<sub>a</sub>=G<sub>r</sub>;</item>
            /// <item>B<sub>a</sub>=B<sub>r</sub>.</item>
            /// </list>
            /// </summary>
            ColorAnaglyph,

            /// <summary>
            /// Creates anaglyph image using the below calculations:
            /// <list type="bullet">
            /// <item>R<sub>a</sub>=0.299*R<sub>l</sub>+0.587*G<sub>l</sub>+0.114*B<sub>l</sub>;</item>
            /// <item>G<sub>a</sub>=G<sub>r</sub>;</item>
            /// <item>B<sub>a</sub>=B<sub>r</sub>.</item>
            /// </list>
            /// </summary>
            HalfColorAnaglyph,

            /// <summary>
            /// Creates anaglyph image using the below calculations:
            /// <list type="bullet">
            /// <item>R<sub>a</sub>=0.7*G<sub>l</sub>+0.3*B<sub>l</sub>;</item>
            /// <item>G<sub>a</sub>=G<sub>r</sub>;</item>
            /// <item>B<sub>a</sub>=B<sub>r</sub>.</item>
            /// </list>
            /// </summary>
            OptimizedAnaglyph
        }

        private Algorithm anaglyphAlgorithm = Algorithm.GrayAnaglyph;

        /// <summary>
        /// Algorithm to use for creating anaglyph images.
        /// </summary>
        /// 
        /// <remarks><para>Default value is set to <see cref="Algorithm.GrayAnaglyph"/>.</para></remarks>
        /// 
        public Algorithm AnaglyphAlgorithm
        {
            get { return anaglyphAlgorithm; }
            set { anaglyphAlgorithm = value; }
        }


        // private format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StereoAnaglyph"/> class.
        /// </summary>
        public StereoAnaglyph( )
        {
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StereoAnaglyph"/> class.
        /// </summary>
        /// 
        /// <param name="anaglyphAlgorithm">Algorithm to use for creating anaglyph images.</param>
        /// 
        public StereoAnaglyph( Algorithm anaglyphAlgorithm )
            : this( )
        {
            this.anaglyphAlgorithm = anaglyphAlgorithm;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data (left image).</param>
        /// <param name="overlay">Overlay image data (right image).</param>
        ///
        protected override unsafe void ProcessFilter( UnmanagedImage image, UnmanagedImage overlay )
        {
            // get image dimension
            int width  = image.Width;
            int height = image.Height;

            // initialize other variables
            int offset    = image.Stride - width * 3;
            int ovrOffset = overlay.Stride - width * 3;

            // do the job
            byte * ptr = (byte*) image.ImageData.ToPointer( );
            byte * ovr = (byte*) overlay.ImageData.ToPointer( );

            switch ( anaglyphAlgorithm )
            {
                case Algorithm.TrueAnaglyph:
                    // for each line
                    for ( int y = 0; y < height; y++ )
                    {
                        // for each pixel
                        for ( int x = 0; x < width; x++, ptr += 3, ovr += 3 )
                        {
                            ptr[RGB.R] = (byte) ( ptr[RGB.R] * 0.299 + ptr[RGB.G] * 0.587 + ptr[RGB.B] * 0.114 );
                            ptr[RGB.G] = 0;
                            ptr[RGB.B] = (byte) ( ovr[RGB.R] * 0.299 + ovr[RGB.G] * 0.587 + ovr[RGB.B] * 0.114 );
                        }
                        ptr += offset;
                        ovr += ovrOffset;
                    }
                    break;

                case Algorithm.GrayAnaglyph:
                    // for each line
                    for ( int y = 0; y < height; y++ )
                    {
                        // for each pixel
                        for ( int x = 0; x < width; x++, ptr += 3, ovr += 3 )
                        {
                            ptr[RGB.R] = (byte) ( ptr[RGB.R] * 0.299 + ptr[RGB.G] * 0.587 + ptr[RGB.B] * 0.114 );
                            ptr[RGB.G] = (byte) ( ovr[RGB.R] * 0.299 + ovr[RGB.G] * 0.587 + ovr[RGB.B] * 0.114 );
                            ptr[RGB.B] = ptr[RGB.G];
                        }
                        ptr += offset;
                        ovr += ovrOffset;
                    }
                    break;
                
                case Algorithm.ColorAnaglyph:
                    // for each line
                    for ( int y = 0; y < height; y++ )
                    {
                        // for each pixel
                        for ( int x = 0; x < width; x++, ptr += 3, ovr += 3 )
                        {
                            // keep Red as it is and take only Green and Blue from the second image
                            ptr[RGB.G] = ovr[RGB.G];
                            ptr[RGB.B] = ovr[RGB.B];
                        }
                        ptr += offset;
                        ovr += ovrOffset;
                    }
                    break;

                case Algorithm.HalfColorAnaglyph:
                    // for each line
                    for ( int y = 0; y < height; y++ )
                    {
                        // for each pixel
                        for ( int x = 0; x < width; x++, ptr += 3, ovr += 3 )
                        {
                            ptr[RGB.R] = (byte) ( ptr[RGB.R] * 0.299 + ptr[RGB.G] * 0.587 + ptr[RGB.B] * 0.114 );
                            ptr[RGB.G] = ovr[RGB.G];
                            ptr[RGB.B] = ovr[RGB.B];
                        }
                        ptr += offset;
                        ovr += ovrOffset;
                    }
                    break;

                case Algorithm.OptimizedAnaglyph:
                    // for each line
                    for ( int y = 0; y < height; y++ )
                    {
                        // for each pixel
                        for ( int x = 0; x < width; x++, ptr += 3, ovr += 3 )
                        {
                            ptr[RGB.R] = (byte) ( ptr[RGB.G] * 0.7 + ptr[RGB.B] * 0.3 );
                            ptr[RGB.G] = ovr[RGB.G];
                            ptr[RGB.B] = ovr[RGB.B];
                        }
                        ptr += offset;
                        ovr += ovrOffset;
                    }
                    break;
            }
        }
    }
}
