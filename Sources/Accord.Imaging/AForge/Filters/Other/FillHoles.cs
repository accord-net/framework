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
    /// Fill holes in objects in binary image.
    /// </summary>
    /// 
    /// <remarks><para>The filter allows to fill black holes in white object in a binary image.
    /// It is possible to specify maximum holes' size to fill using <see cref="MaxHoleWidth"/>
    /// and <see cref="MaxHoleHeight"/> properties.</para>
    /// 
    /// <para>The filter accepts binary image only, which are represented  as 8 bpp images.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create and configure the filter
    /// FillHoles filter = new FillHoles( );
    /// filter.MaxHoleHeight = 20;
    /// filter.MaxHoleWidth  = 20;
    /// filter.CoupledSizeFiltering = false;
    /// // apply the filter
    /// Bitmap result = filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample19.png" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/filled_holes.png" width="320" height="240" />
    /// </remarks>
    /// 
    public class FillHoles : BaseInPlaceFilter
    {
        // private format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );
        // coupled size filtering or not
        private bool coupledSizeFiltering = true;
        // maximum hole size to fill
        private int maxHoleWidth  = int.MaxValue;
        private int maxHoleHeight = int.MaxValue;

        /// <summary>
        /// Specifies if size filetering should be coupled or not.
        /// </summary>
        /// 
        /// <remarks><para>In uncoupled filtering mode, holes are filled in the case if
        /// their width is smaller than or equal to <see cref="MaxHoleWidth"/> or height is smaller than 
        /// or equal to <see cref="MaxHoleHeight"/>. But in coupled filtering mode, holes are filled only in
        /// the case if both width and height are smaller or equal to the corresponding value.</para>
        /// 
        /// <para>Default value is set to <see langword="true"/>, what means coupled filtering by size.</para>
        /// </remarks>
        /// 
        public bool CoupledSizeFiltering
        {
            get { return coupledSizeFiltering; }
            set { coupledSizeFiltering = value; }
        }

        /// <summary>
        /// Maximum width of a hole to fill.
        /// </summary>
        ///
        /// <remarks><para>All holes, which have width greater than this value, are kept unfilled.
        /// See <see cref="CoupledSizeFiltering"/> for additional information.</para>
        /// 
        /// <para>Default value is set to <see cref="int.MaxValue"/>.</para></remarks>
        ///
        public int MaxHoleWidth
        {
            get { return maxHoleWidth; }
            set { maxHoleWidth = Math.Max( value, 0 ); }
        }

        /// <summary>
        /// Maximum height of a hole to fill.
        /// </summary>
        ///
        /// <remarks><para>All holes, which have height greater than this value, are kept unfilled.
        /// See <see cref="CoupledSizeFiltering"/> for additional information.</para>
        /// 
        /// <para>Default value is set to <see cref="int.MaxValue"/>.</para></remarks>
        ///
        public int MaxHoleHeight
        {
            get { return maxHoleHeight; }
            set { maxHoleHeight = Math.Max( value, 0 ); }
        }

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }
        
        /// <summary>   
        /// Initializes a new instance of the <see cref="FillHoles"/> class.
        /// </summary>
        public FillHoles( )
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        ///
        protected override unsafe void ProcessFilter( UnmanagedImage image )
        {
            int width  = image.Width;
            int height = image.Height;

            // 1 - invert the source image
            Invert invertFilter = new Invert( );
            UnmanagedImage invertedImage = invertFilter.Apply( image );

            // 2 - use blob counter to find holes (they are white objects now on the inverted image)
            BlobCounter blobCounter = new BlobCounter( );
            blobCounter.ProcessImage( invertedImage );
            Blob[] blobs = blobCounter.GetObjectsInformation( );

            // 3 - check all blobs and determine which should be filtered
            byte[] newObjectColors = new byte[blobs.Length + 1];
            newObjectColors[0] = 255; // don't touch the objects, which have 0 ID

            for ( int i = 0, n = blobs.Length; i < n; i++ )
            {
                Blob blob = blobs[i];

                if ( ( blob.Rectangle.Left == 0 ) || ( blob.Rectangle.Top == 0 ) ||
                     ( blob.Rectangle.Right == width ) || ( blob.Rectangle.Bottom == height ) )
                {
                    newObjectColors[blob.ID] = 0;
                }
                else
                {
                    if ( ( ( coupledSizeFiltering ) && ( blob.Rectangle.Width <= maxHoleWidth ) && ( blob.Rectangle.Height <= maxHoleHeight ) ) |
                         ( ( !coupledSizeFiltering ) && ( ( blob.Rectangle.Width <= maxHoleWidth ) || ( blob.Rectangle.Height <= maxHoleHeight ) ) ) )
                    {
                        newObjectColors[blob.ID] = 255;
                    }
                    else
                    {
                        newObjectColors[blob.ID] = 0;
                    }
                }
            }

            // 4 - process the source image image and fill holes
            byte* ptr = (byte*) image.ImageData.ToPointer( );
            int offset = image.Stride - width;

            int[] objectLabels = blobCounter.ObjectLabels;

            for ( int y = 0, i = 0; y < height; y++ )
            {
                for ( int x = 0; x < width; x++, i++, ptr++ )
                {
                    *ptr = newObjectColors[objectLabels[i]];
                }
                ptr += offset;
            }
        }
    }
}
