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

namespace Accord.Imaging
{
    using Accord.Math;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;

    /// <summary>
    ///   Base class for corner detectors implementing the <see cref="ICornersDetector"/> interface.
    ///   Corner detectors can be seen as the simplest sparse feature extractors, where the extracted
    ///   features are the (x,y) positions themselves.
    /// </summary>
    /// 
    /// <seealso cref="Accord.Imaging.BaseSparseFeatureExtractor{T}" />
    /// <seealso cref="Accord.Imaging.ICornersDetector" />
    /// 
    [Serializable]
    public abstract class BaseCornersDetector : BaseSparseFeatureExtractor<CornerFeaturePoint>, ICornersDetector
    {
        /// <summary>
        /// Process image looking for corners.
        /// </summary>
        /// 
        /// <param name="image">Source image to process.</param>
        /// 
        /// <returns>Returns list of found corners (X-Y coordinates).</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
        /// 
        public new List<IntPoint> ProcessImage(Bitmap image)
        {
            // check image format
            if (!SupportedFormats.Contains(image.PixelFormat))
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");

            // lock source image
            BitmapData imageData = image.LockBits(ImageLockMode.ReadOnly);

            List<IntPoint> corners;

            try
            {
                // process the image
                corners = ProcessImage(new UnmanagedImage(imageData));
            }
            finally
            {
                // unlock image
                image.UnlockBits(imageData);
            }

            return corners;
        }

        /// <summary>
        ///   This method should be implemented by inheriting classes to implement the
        ///   actual corners detection, transforming the input image into a list of points.
        /// </summary>
        /// 
        protected abstract List<IntPoint> InnerProcess(UnmanagedImage image);

        /// <summary>
        ///   This method should be implemented by inheriting classes to implement the
        ///   actual feature extraction, transforming the input image into a list of features.
        /// </summary>
        /// 
        protected sealed override IEnumerable<CornerFeaturePoint> InnerTransform(UnmanagedImage input)
        {
            return InnerProcess(input).Select(x => new CornerFeaturePoint(x));
        }

        /// <summary>
        /// Process image looking for corners.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found corners (X-Y coordinates).</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
        /// 
        public new List<IntPoint> ProcessImage(BitmapData imageData)
        {
            return ProcessImage(new UnmanagedImage(imageData));
        }

        /// <summary>
        /// Process image looking for corners.
        /// </summary>
        /// 
        /// <param name="input">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found corners (X-Y coordinates).</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
        /// 
        public new List<IntPoint> ProcessImage(UnmanagedImage input)
        {
            // check image format
            if (!SupportedFormats.Contains(input.PixelFormat))
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");

            return InnerProcess(input);
        }
    }
}