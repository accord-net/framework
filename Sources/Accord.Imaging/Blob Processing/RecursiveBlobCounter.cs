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

namespace Accord.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Blob counter based on recursion.
    /// </summary>
    /// 
    /// <remarks><para>The class counts and extracts stand alone objects in
    /// images using recursive version of connected components labeling
    /// algorithm.</para>
    /// 
    /// <para><note>The algorithm treats all pixels with values less or equal to <see cref="BackgroundThreshold"/>
    /// as background, but pixels with higher values are treated as objects' pixels.</note></para>
    /// 
    /// <para><note>Since this algorithm is based on recursion, it is
    /// required to be careful with its application to big images with big blobs,
    /// because in this case recursion will require big stack size and may lead
    /// to stack overflow. The recursive version may be applied (and may be even
    /// faster than <see cref="BlobCounter"/>) to an image with small blobs -
    /// "star sky" image (or small cells, for example, etc).</note></para>
    /// 
    /// <para>For blobs' searching the class supports 8 bpp indexed grayscale images and
    /// 24/32 bpp color images. 
    /// See documentation about <see cref="BlobCounterBase"/> for information about which
    /// pixel formats are supported for extraction of blobs.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create an instance of blob counter algorithm
    /// var bc = new RecursiveBlobCounter();
    /// 
    /// // process binary image
    /// bc.ProcessImage(image);
    /// 
    /// // process blobs
    /// foreach (Rectangle rect in bc.GetObjectsRectangles())
    /// {
    ///     // ...
    /// }
    /// </code>
    /// </remarks>
    /// 
    public class RecursiveBlobCounter : BlobCounterBase
    {
        // temporary variable
        private int[] tempLabels;
        private int stride;
        private int pixelSize;

        private byte backgroundThresholdR = 0;
        private byte backgroundThresholdG = 0;
        private byte backgroundThresholdB = 0;

        /// <summary>
        /// Background threshold's value.
        /// </summary>
        /// 
        /// <remarks><para>The property sets threshold value for distinguishing between background
        /// pixel and objects' pixels. All pixel with values less or equal to this property are
        /// treated as background, but pixels with higher values are treated as objects' pixels.</para>
        /// 
        /// <para><note>In the case of colour images a pixel is treated as objects' pixel if <b>any</b> of its
        /// RGB values are higher than corresponding values of this threshold.</note></para>
        /// 
        /// <para><note>For processing grayscale image, set the property with all RGB components eqaul.</note></para>
        ///
        /// <para>Default value is set to <b>(0, 0, 0)</b> - black colour.</para></remarks>
        /// 
        public Color BackgroundThreshold
        {
            get { return Color.FromArgb(backgroundThresholdR, backgroundThresholdG, backgroundThresholdB); }
            set
            {
                backgroundThresholdR = value.R;
                backgroundThresholdG = value.G;
                backgroundThresholdB = value.B;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecursiveBlobCounter"/> class.
        /// </summary>
        /// 
        /// <remarks>Creates new instance of the <see cref="RecursiveBlobCounter"/> class with
        /// an empty objects map. Before using methods, which provide information about blobs
        /// or extract them, the <see cref="BlobCounterBase.ProcessImage(Bitmap)"/>,
        /// <see cref="BlobCounterBase.ProcessImage(BitmapData)"/> or <see cref="BlobCounterBase.ProcessImage(UnmanagedImage)"/>
        /// method should be called to collect objects map.</remarks>
        /// 
        public RecursiveBlobCounter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecursiveBlobCounter"/> class.
        /// </summary>
        /// 
        /// <param name="image">Image to look for objects in.</param>
        /// 
        public RecursiveBlobCounter(Bitmap image)
            : base(image)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecursiveBlobCounter"/> class.
        /// </summary>
        /// 
        /// <param name="imageData">Image data to look for objects in.</param>
        /// 
        public RecursiveBlobCounter(BitmapData imageData)
            : base(imageData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecursiveBlobCounter"/> class.
        /// </summary>
        /// 
        /// <param name="image">Unmanaged image to look for objects in.</param>
        /// 
        public RecursiveBlobCounter(UnmanagedImage image)
            : base(image)
        {
        }

        /// <summary>
        /// Actual objects map building.
        /// </summary>
        /// 
        /// <param name="image">Unmanaged image to process.</param>
        /// 
        /// <remarks>The method supports 8 bpp indexed grayscale images and 24/32 bpp color images.</remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// 
        protected override void BuildObjectsMap(UnmanagedImage image)
        {
            this.stride = image.Stride;

            // check pixel format
            if ((image.PixelFormat != PixelFormat.Format8bppIndexed) &&
                 (image.PixelFormat != PixelFormat.Format24bppRgb) &&
                 (image.PixelFormat != PixelFormat.Format32bppRgb) &&
                 (image.PixelFormat != PixelFormat.Format32bppArgb) &&
                 (image.PixelFormat != PixelFormat.Format32bppPArgb))
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }

            // allocate temporary labels array
            tempLabels = new int[(ImageWidth + 2) * (ImageHeight + 2)];
            // fill boundaries with reserved value
            for (int x = 0, mx = ImageWidth + 2; x < mx; x++)
            {
                tempLabels[x] = -1;
                tempLabels[x + (ImageHeight + 1) * (ImageWidth + 2)] = -1;
            }
            for (int y = 0, my = ImageHeight + 2; y < my; y++)
            {
                tempLabels[y * (ImageWidth + 2)] = -1;
                tempLabels[y * (ImageWidth + 2) + ImageWidth + 1] = -1;
            }

            // initial objects count
            ObjectsCount = 0;

            // do the job
            unsafe
            {
                byte* src = (byte*)image.ImageData.ToPointer();
                int p = ImageWidth + 2 + 1;

                if (image.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    int offset = stride - ImageWidth;

                    // for each line
                    for (int y = 0; y < ImageHeight; y++)
                    {
                        // for each pixel
                        for (int x = 0; x < ImageWidth; x++, src++, p++)
                        {
                            // check for non-labeled pixel
                            if ((*src > backgroundThresholdG) && (tempLabels[p] == 0))
                            {
                                ObjectsCount++;
                                LabelPixel(src, p);
                            }
                        }
                        src += offset;
                        p += 2;
                    }
                }
                else
                {
                    pixelSize = Bitmap.GetPixelFormatSize(image.PixelFormat) / 8;
                    int offset = stride - ImageWidth * pixelSize;

                    // for each line
                    for (int y = 0; y < ImageHeight; y++)
                    {
                        // for each pixel
                        for (int x = 0; x < ImageWidth; x++, src += pixelSize, p++)
                        {
                            // check for non-labeled pixel
                            if ((
                                    (src[RGB.R] > backgroundThresholdR) ||
                                    (src[RGB.G] > backgroundThresholdG) ||
                                    (src[RGB.B] > backgroundThresholdB)
                                  ) &&
                                (tempLabels[p] == 0))
                            {
                                ObjectsCount++;
                                LabelColorPixel(src, p);
                            }
                        }
                        src += offset;
                        p += 2;
                    }
                }
            }

            // allocate labels array
            ObjectLabels = new int[ImageWidth * ImageHeight];

            for (int y = 0; y < ImageHeight; y++)
                Array.Copy(tempLabels, (y + 1) * (ImageWidth + 2) + 1, ObjectLabels, y * ImageWidth, ImageWidth);
        }

        private unsafe void LabelPixel(byte* pixel, int labelPointer)
        {
            if ((tempLabels[labelPointer] == 0) && (*pixel > backgroundThresholdG))
            {
                tempLabels[labelPointer] = ObjectsCount;

                LabelPixel(pixel + 1, labelPointer + 1);                              // x + 1, y
                LabelPixel(pixel + 1 + stride, labelPointer + 1 + 2 + ImageWidth);    // x + 1, y + 1
                LabelPixel(pixel + stride, labelPointer + 2 + ImageWidth);            // x    , y + 1
                LabelPixel(pixel - 1 + stride, labelPointer - 1 + 2 + ImageWidth);    // x - 1, y + 1
                LabelPixel(pixel - 1, labelPointer - 1);                              // x - 1, y
                LabelPixel(pixel - 1 - stride, labelPointer - 1 - 2 - ImageWidth);    // x - 1, y - 1
                LabelPixel(pixel - stride, labelPointer - 2 - ImageWidth);            // x    , y - 1
                LabelPixel(pixel + 1 - stride, labelPointer + 1 - 2 - ImageWidth);    // x + 1, y - 1
            }
        }

        private unsafe void LabelColorPixel(byte* pixel, int labelPointer)
        {
            if ((tempLabels[labelPointer] == 0) && (
                (pixel[RGB.R] > backgroundThresholdR) ||
                (pixel[RGB.G] > backgroundThresholdG) ||
                (pixel[RGB.B] > backgroundThresholdB)))
            {
                tempLabels[labelPointer] = ObjectsCount;

                LabelColorPixel(pixel + pixelSize, labelPointer + 1);                              // x + 1, y
                LabelColorPixel(pixel + pixelSize + stride, labelPointer + 1 + 2 + ImageWidth);    // x + 1, y + 1
                LabelColorPixel(pixel + stride, labelPointer + 2 + ImageWidth);                    // x    , y + 1
                LabelColorPixel(pixel - pixelSize + stride, labelPointer - 1 + 2 + ImageWidth);    // x - 1, y + 1
                LabelColorPixel(pixel - pixelSize, labelPointer - 1);                              // x - 1, y
                LabelColorPixel(pixel - pixelSize - stride, labelPointer - 1 - 2 - ImageWidth);    // x - 1, y - 1
                LabelColorPixel(pixel - stride, labelPointer - 2 - ImageWidth);                    // x    , y - 1
                LabelColorPixel(pixel + pixelSize - stride, labelPointer + 1 - 2 - ImageWidth);    // x + 1, y - 1
            }
        }
    }
}
