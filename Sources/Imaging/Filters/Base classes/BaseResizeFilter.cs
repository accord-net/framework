// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Base class for image resizing filters.
    /// </summary>
    /// 
    /// <remarks><para>The abstract class is the base class for all filters,
    /// which implement image rotation algorithms.</para>
    /// </remarks>
    /// 
    public abstract class BaseResizeFilter : BaseTransformationFilter
    {
        /// <summary>
        /// New image width.
        /// </summary>
        protected int newWidth;

        /// <summary>
        /// New image height.
        /// </summary>
        protected int newHeight;

        /// <summary>
        /// Width of the new resized image.
        /// </summary>
        /// 
        public int NewWidth
        {
            get { return newWidth; }
            set { newWidth = Math.Max( 1, value ); }
        }

        /// <summary>
        /// Height of the new resized image.
        /// </summary>
        /// 
        public int NewHeight
        {
            get { return newHeight; }
            set { newHeight = Math.Max( 1, value ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseResizeFilter"/> class.
        /// </summary>
        /// 
        /// <param name="newWidth">Width of the new resized image.</param>
        /// <param name="newHeight">Height of the new resize image.</param>
        /// 
        protected BaseResizeFilter( int newWidth, int newHeight )
        {
            this.newWidth  = newWidth;
            this.newHeight = newHeight;
        }

        /// <summary>
        /// Calculates new image size.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// 
        /// <returns>New image size - size of the destination image.</returns>
        /// 
        protected override System.Drawing.Size CalculateNewImageSize( UnmanagedImage sourceData )
        {
            return new Size( newWidth, newHeight );
        }
    }
}
