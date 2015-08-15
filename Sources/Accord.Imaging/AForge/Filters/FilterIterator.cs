// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2010
// contacts@aforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Filter iterator.
    /// </summary>
    /// 
    /// <remarks><para>Filter iterator performs specified amount of filter's iterations.
    /// The filter take the specified <see cref="BaseFilter">base filter</see> and applies it
    /// to source image <see cref="Iterations">specified amount of times</see>.</para>
    /// 
    /// <para><note>The filter itself does not have any restrictions to pixel format of source
    /// image. This is set by <see cref="BaseFilter">base filter</see>.</note></para>
    /// 
    /// <para><note>The filter does image processing using only <see cref="IFilter"/>
    /// interface of the specified <see cref="BaseFilter">base filter</see>. This means
    /// that this filter may not utilize all potential features of the base filter, like
    /// in-place processing (see <see cref="IInPlaceFilter"/>) and region based processing
    /// (see <see cref="IInPlacePartialFilter"/>). To utilize those features, it is required to
    /// do filter's iteration manually.</note></para>
    /// 
    /// <para>Sample usage (morphological thinning):</para>
    /// <code>
    /// // create filter sequence
    /// FiltersSequence filterSequence = new FiltersSequence( );
    /// // add 8 thinning filters with different structuring elements
    /// filterSequence.Add( new HitAndMiss(
    ///     new short [,] { { 0, 0, 0 }, { -1, 1, -1 }, { 1, 1, 1 } },
    ///     HitAndMiss.Modes.Thinning ) );
    /// filterSequence.Add( new HitAndMiss(
    ///     new short [,] { { -1, 0, 0 }, { 1, 1, 0 }, { -1, 1, -1 } },
    ///     HitAndMiss.Modes.Thinning ) );
    /// filterSequence.Add( new HitAndMiss(
    ///     new short [,] { { 1, -1, 0 }, { 1, 1, 0 }, { 1, -1, 0 } },
    ///     HitAndMiss.Modes.Thinning ) );
    /// filterSequence.Add( new HitAndMiss(
    ///     new short [,] { { -1, 1, -1 }, { 1, 1, 0 }, { -1, 0, 0 } },
    ///     HitAndMiss.Modes.Thinning ) );
    /// filterSequence.Add( new HitAndMiss(
    ///     new short [,] { { 1, 1, 1 }, { -1, 1, -1 }, { 0, 0, 0 } },
    ///     HitAndMiss.Modes.Thinning ) );
    /// filterSequence.Add( new HitAndMiss(
    ///     new short [,] { { -1, 1, -1 }, { 0, 1, 1 }, { 0, 0, -1 } },
    ///     HitAndMiss.Modes.Thinning ) );
    /// filterSequence.Add( new HitAndMiss(
    ///     new short [,] { { 0, -1, 1 }, { 0, 1, 1 }, { 0, -1, 1 } },
    ///     HitAndMiss.Modes.Thinning ) );
    /// filterSequence.Add( new HitAndMiss(
    ///     new short [,] { { 0, 0, -1 }, { 0, 1, 1 }, { -1, 1, -1 } },
    ///     HitAndMiss.Modes.Thinning ) );
    /// // create filter iterator for 10 iterations
    /// FilterIterator filter = new FilterIterator( filterSequence, 10 );
    /// // apply the filter
    /// Bitmap newImage = filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample14.png" width="150" height="150" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/thinning.png" width="150" height="150" />
    /// </remarks>
    /// 
    public class FilterIterator : IFilter, IFilterInformation
    {
        private IFilter	baseFilter;
        private int		iterations = 1;

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        /// 
        /// <remarks><para>See <see cref="IFilterInformation.FormatTranslations"/>
        /// documentation for additional information.</para>
        /// 
        /// <para><note>The filter provides format translation dictionary taken from
        /// <see cref="BaseFilter"/> filter.</note></para>
        /// </remarks>
        /// 
        public Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return ( (IFilterInformation) baseFilter).FormatTranslations; }
        }

        /// <summary>
        /// Base filter.
        /// </summary>
        /// 
        /// <remarks><para>The base filter is the filter to be applied specified amount of iterations to
        /// a specified image.</para></remarks>
        /// 
        public IFilter BaseFilter
        {
            get { return baseFilter; }
            set { baseFilter = value; }
        }

        /// <summary>
        /// Iterations amount, [1, 255].
        /// </summary>
        /// 
        /// <remarks><para>The amount of times to apply specified filter to a specified image.</para>
        /// 
        /// <para>Default value is set to <b>1</b>.</para>
        /// </remarks>
        /// 
        public int Iterations
        {
            get { return iterations; }
            set { iterations = Math.Max( 1, Math.Min( 255, value ) ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterIterator"/> class.
        /// </summary>
        /// 
        /// <param name="baseFilter">Filter to iterate.</param>
        /// 
        public FilterIterator( IFilter baseFilter )
        {
            this.baseFilter = baseFilter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterIterator"/> class.
        /// </summary>
        /// 
        /// <param name="baseFilter">Filter to iterate.</param>
        /// <param name="iterations">Iterations amount.</param>
        /// 
        public FilterIterator( IFilter baseFilter, int iterations )
        {
            this.baseFilter = baseFilter;
            this.iterations = iterations;
        }

        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="image">Source image to apply filter to.</param>
        /// 
        /// <returns>Returns filter's result obtained by applying the filter to
        /// the source image.</returns>
        /// 
        /// <remarks>The method keeps the source image unchanged and returns
        /// the result of image processing filter as new image.</remarks> 
        ///
        public Bitmap Apply( Bitmap image )
        {
            // lock source bitmap data
            BitmapData imageData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, image.PixelFormat );

            // apply the filter
            Bitmap dstImage = Apply( imageData );

            // unlock source image
            image.UnlockBits( imageData );

            return dstImage;
        }

        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="imageData">Source image to apply filter to.</param>
        /// 
        /// <returns>Returns filter's result obtained by applying the filter to
        /// the source image.</returns>
        /// 
        /// <remarks>The filter accepts bitmap data as input and returns the result
        /// of image processing filter as new image. The source image data are kept
        /// unchanged.</remarks>
        /// 
        public Bitmap Apply( BitmapData imageData )
        {
            // initial iteration
            Bitmap dstImg = baseFilter.Apply( imageData );
            Bitmap tmpImg;

            // continue to iterate
            for ( int i = 1; i < iterations; i++ )
            {
                tmpImg = dstImg;
                dstImg = baseFilter.Apply( tmpImg );
                tmpImg.Dispose( );
            }

            return dstImg;
        }

        /// <summary>
        /// Apply filter to an image in unmanaged memory.
        /// </summary>
        /// 
        /// <param name="image">Source image in unmanaged memory to apply filter to.</param>
        /// 
        /// <returns>Returns filter's result obtained by applying the filter to
        /// the source image.</returns>
        /// 
        /// <remarks>The method keeps the source image unchanged and returns
        /// the result of image processing filter as new image.</remarks>
        /// 
        public UnmanagedImage Apply( UnmanagedImage image )
        {
            // initial iteration
            UnmanagedImage dstImg = baseFilter.Apply( image );
            UnmanagedImage tmpImg;

            // continue to iterate
            for ( int i = 1; i < iterations; i++ )
            {
                tmpImg = dstImg;
                dstImg = baseFilter.Apply( tmpImg );
                tmpImg.Dispose( );
            }

            return dstImg;
        }

        /// <summary>
        /// Apply filter to an image in unmanaged memory.
        /// </summary>
        /// 
        /// <param name="sourceImage">Source image in unmanaged memory to apply filter to.</param>
        /// <param name="destinationImage">Destination image in unmanaged memory to put result into.</param>
        /// 
        /// <remarks><para>The method keeps the source image unchanged and puts result of image processing
        /// into destination image.</para>
        /// 
        /// <para><note>The destination image must have the same width and height as source image. Also
        /// destination image must have pixel format, which is expected by particular filter (see
        /// <see cref="FormatTranslations"/> property for information about pixel format conversions).</note></para>
        /// </remarks>
        /// 
        public void Apply( UnmanagedImage sourceImage, UnmanagedImage destinationImage )
        {
            if ( iterations == 1 )
            {
                baseFilter.Apply( sourceImage, destinationImage );
            }
            else
            {
                // initial iteration
                UnmanagedImage dstImg = baseFilter.Apply( sourceImage );
                UnmanagedImage tmpImg;

                iterations--;
                // continue to iterate
                for ( int i = 1; i < iterations; i++ )
                {
                    tmpImg = dstImg;
                    dstImg = baseFilter.Apply( tmpImg );
                    tmpImg.Dispose( );
                }

                // put result of last iteration into the specified destination
                baseFilter.Apply( dstImg, destinationImage );
            }
        }
    }
}
