// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//
// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Imaging.Filters
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
        ///   New image width.
        /// </summary>
        /// 
        protected int newWidth;

        /// <summary>
        ///   New image height.
        /// </summary>
        /// 
        protected int newHeight;

        /// <summary>
        ///   Width of the new resized image.
        /// </summary>
        /// 
        public int NewWidth
        {
            get { return newWidth; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Value must be higher than zero.");
                this.newWidth = value;
            }
        }

        /// <summary>
        ///   Height of the new resized image.
        /// </summary>
        /// 
        public int NewHeight
        {
            get { return newHeight; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Value must be higher than zero.");
                this.newHeight = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseResizeFilter"/> class.
        /// </summary>
        /// 
        /// <param name="newWidth">Width of the new resized image.</param>
        /// <param name="newHeight">Height of the new resize image.</param>
        /// 
        protected BaseResizeFilter(int newWidth, int newHeight)
        {
            this.NewWidth = newWidth;
            this.NewHeight = newHeight;
        }

        /// <summary>
        /// Calculates new image size.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// 
        /// <returns>New image size - size of the destination image.</returns>
        /// 
        protected override System.Drawing.Size CalculateNewImageSize(UnmanagedImage sourceData)
        {
            return new Size(newWidth, newHeight);
        }
    }
}
