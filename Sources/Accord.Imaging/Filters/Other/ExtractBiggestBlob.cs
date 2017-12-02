// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
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
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Extract the biggest blob from image.
    /// </summary>
    /// 
    /// <remarks><para>The filter locates the biggest blob in the source image and extracts it.
    /// The filter also can use the source image for the biggest blob's location only, but extract it from
    /// another image, which is set using <see cref="OriginalImage"/> property. The original image 
    /// usually is the source of the processed image.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images and 24/32 color images for processing as source image passed to
    /// <see cref="Apply( Bitmap )"/> method and also for the <see cref="OriginalImage"/>.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// var filter = new ExtractBiggestBlob();
    /// 
    /// // apply the filter
    /// Bitmap biggestBlobsImage = filter.Apply(image);
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="..\images\imaging\sample2.jpg" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="..\images\imaging\biggest_blob.jpg" width="141" height="226" />
    /// </remarks>
    /// 
    public class ExtractBiggestBlob : IFilter, IFilterInformation
    {
        private Bitmap originalImage = null;
        private IntPoint blobPosition;

        /// <summary>
        /// Position of the extracted blob.
        /// </summary>
        /// 
        /// <remarks><para>After applying the filter this property keeps position of the extracted
        /// blob in the source image.</para></remarks>
        /// 
        public IntPoint BlobPosition
        {
            get { return blobPosition; }
        }

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        /// 
        /// <remarks><para>The dictionary defines, which pixel formats are supported for
        /// source images and which pixel format will be used for resulting image.
        /// </para>
        /// 
        /// <para>See <see cref="IFilterInformation.FormatTranslations"/> for more information.</para>
        /// </remarks>
        ///
        public Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

                // initialize format translation dictionary
                if (originalImage == null)
                {
                    formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
                    formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
                    formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
                    formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
                    formatTranslations[PixelFormat.Format32bppPArgb] = PixelFormat.Format32bppPArgb;
                }
                else
                {
                    formatTranslations[PixelFormat.Format8bppIndexed] = originalImage.PixelFormat;
                    formatTranslations[PixelFormat.Format24bppRgb] = originalImage.PixelFormat;
                    formatTranslations[PixelFormat.Format32bppArgb] = originalImage.PixelFormat;
                    formatTranslations[PixelFormat.Format32bppRgb] = originalImage.PixelFormat;
                    formatTranslations[PixelFormat.Format32bppPArgb] = originalImage.PixelFormat;
                }

                return formatTranslations;
            }
        }

        /// <summary>
        /// Original image, which is the source of the processed image where the biggest blob is searched for.
        /// </summary>
        /// 
        /// <remarks><para>The property may be set to <see langword="null"/>. In this case the biggest blob
        /// is extracted from the image, which is passed to <see cref="Apply(Bitmap)"/> image.</para>
        /// </remarks>
        /// 
        public Bitmap OriginalImage
        {
            get { return originalImage; }
            set { originalImage = value; }
        }

        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="image">Source image to get biggest blob from.</param>
        /// 
        /// <returns>Returns image of the biggest blob.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the original image.</exception>
        /// <exception cref="InvalidImagePropertiesException">Source and original images must have the same size.</exception>
        /// <exception cref="ArgumentException">The source image does not contain any blobs.</exception>
        ///
        public Bitmap Apply(Bitmap image)
        {
            // lock source bitmap data
            BitmapData srcData = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, image.PixelFormat);

            Bitmap dstImage = null;

            try
            {
                // apply the filter
                dstImage = Apply(srcData);
            }
            finally
            {
                // unlock source image
                image.UnlockBits(srcData);
            }

            return dstImage;
        }

        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="imageData">Source image to get biggest blob from.</param>
        /// 
        /// <returns>Returns image of the biggest blob.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the original image.</exception>
        /// <exception cref="InvalidImagePropertiesException">Source and original images must have the same size.</exception>
        /// <exception cref="ArgumentException">The source image does not contain any blobs.</exception>
        ///
        public Bitmap Apply(BitmapData imageData)
        {
            using (UnmanagedImage biggestBlob = Apply(new UnmanagedImage(imageData)))
            {
                // dispose unmanaged image of the biggest blob
                return biggestBlob.ToManagedImage();
            }
        }

        /// <summary>
        /// Apply filter to an image (not implemented).
        /// </summary>
        /// 
        /// <param name="image">Image in unmanaged memory.</param>
        /// 
        /// <returns>Returns filter's result obtained by applying the filter to
        /// the source image.</returns>
        /// 
        public UnmanagedImage Apply(UnmanagedImage image)
        {
            // check pixel format of the source image
            if (!FormatTranslations.ContainsKey(image.PixelFormat))
                throw new UnsupportedImageFormatException("Source pixel format is not supported by the filter.");

            // locate blobs in the source image
            BlobCounter blobCounter = new BlobCounter(image);

            // get information about blobs
            Blob[] blobs = blobCounter.GetObjectsInformation();

            // find the biggest blob
            int maxSize = 0;
            Blob biggestBlob = null;

            for (int i = 0, n = blobs.Length; i < n; i++)
            {
                int size = blobs[i].Rectangle.Width * blobs[i].Rectangle.Height;

                if (size > maxSize)
                {
                    maxSize = size;
                    biggestBlob = blobs[i];
                }
            }

            // check if any blob was found
            if (biggestBlob == null)
                throw new ArgumentException("The source image does not contain any blobs.");

            blobPosition = new IntPoint(biggestBlob.Rectangle.Left, biggestBlob.Rectangle.Top);

            // extract biggest blob's image
            if (originalImage == null)
            {
                blobCounter.ExtractBlobsImage(image, biggestBlob, false);
            }
            else
            {
                // check original image's format
                if (
                    (originalImage.PixelFormat != PixelFormat.Format24bppRgb) &&
                    (originalImage.PixelFormat != PixelFormat.Format32bppArgb) &&
                    (originalImage.PixelFormat != PixelFormat.Format32bppRgb) &&
                    (originalImage.PixelFormat != PixelFormat.Format32bppPArgb) &&
                    (originalImage.PixelFormat != PixelFormat.Format8bppIndexed)
                    )
                {
                    throw new UnsupportedImageFormatException("Original image may be grayscale (8bpp indexed) or color (24/32bpp) image only.");
                }

                // check its size
                if ((originalImage.Width != image.Width) || (originalImage.Height != image.Height))
                    throw new InvalidImagePropertiesException("Original image must have the same size as passed source image.");

                blobCounter.ExtractBlobsImage(originalImage, biggestBlob, false);
            }

            return biggestBlob.Image;
        }

        /// <summary>
        /// Apply filter to an image (not implemented).
        /// </summary>
        /// 
        /// <param name="sourceImage">Source image to be processed.</param>
        /// <param name="destinationImage">Destination image to store filter's result.</param>
        /// 
        public void Apply(UnmanagedImage sourceImage, UnmanagedImage destinationImage)
        {
            Apply(sourceImage).Copy(destinationImage);
        }
    }
}
