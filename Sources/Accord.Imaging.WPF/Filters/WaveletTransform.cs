﻿// Accord Imaging Library
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
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using Accord.Math.Wavelets;
    using Accord.Math;

    /// <summary>
    ///   Wavelet transform filter.
    /// </summary>
    /// 
    /// <example>
    /// <code>
    /// Bitmap image = ... // Lena's famous picture
    /// 
    /// // Create a new Haar Wavelet transform filter
    /// var wavelet = new WaveletTransform(new Haar(1));
    /// 
    /// // Apply the Wavelet transformation
    /// Bitmap result = wavelet.Apply(image);
    /// 
    /// // Show on the screen
    /// ImageBox.Show(result);
    /// </code>
    /// 
    /// <para>
    ///   The resulting image is shown below. </para>
    ///   
    /// <img src="..\images\wavelet-1.png" /> 
    /// 
    /// <code>
    /// // Extract only one of the resulting images
    /// var crop = new Crop(new Rectangle(0, 0, 
    ///     image.Width / 2, image.Height / 2));
    /// 
    /// Bitmap quarter = crop.Apply(result);
    /// 
    /// // Show on the screen
    /// ImageBox.Show(quarter);
    /// </code>
    /// 
    /// <para>
    ///   The resulting image is shown below. </para>
    ///   
    /// <img src="..\images\wavelet-2.png" /> 
    /// </example>
    /// 
    public class WaveletTransform : BaseFilter
    {
        private IWavelet wavelet;
        private bool backward;

        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        /// <summary>
        ///   Constructs a new Wavelet Transform filter.
        /// </summary>
        /// 
        /// <param name="wavelet">A wavelet function.</param>
        /// 
        public WaveletTransform(IWavelet wavelet)
            : this(wavelet, false)
        {
        }

        /// <summary>
        ///   Constructs a new Wavelet Transform filter.
        /// </summary>
        /// 
        /// <param name="wavelet">A wavelet function.</param>
        /// <param name="backward">True to perform backward transform, false otherwise.</param>
        /// 
        public WaveletTransform(IWavelet wavelet, bool backward)
        {
            this.wavelet = wavelet;
            this.backward = backward;

            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
        }

        /// <summary>
        ///   Format translations dictionary.
        /// </summary>
        /// 
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        ///   Gets or sets the Wavelet function
        /// </summary>
        /// 
        public IWavelet Wavelet
        {
            get { return wavelet; }
            set { wavelet = value; }
        }

        /// <summary>
        ///   Gets or sets whether the filter should be applied forward or backwards.
        /// </summary>
        /// 
        public bool Backward
        {
            get { return backward; }
            set { backward = value; }
        }


        /// <summary>
        ///   Applies the filter to the image.
        /// </summary>
        /// 
        protected override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
        {

            // check image format
            if (
                (sourceData.PixelFormat != PixelFormat.Format8bppIndexed) &&
                (sourceData.PixelFormat != PixelFormat.Format16bppGrayScale)
                )
            {
                throw new UnsupportedImageFormatException("Only grayscale images are supported.");
            }

            // get source image size
            int width = sourceData.Width;
            int height = sourceData.Height;

            int srcPixelSize = System.Drawing.Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;
            int dstPixelSize = System.Drawing.Image.GetPixelFormatSize(destinationData.PixelFormat) / 8;

            int srcOffset = sourceData.Stride - width * srcPixelSize;
            int dstOffset = destinationData.Stride - width * dstPixelSize;

            // check image size
            if ((!Accord.Math.Tools.IsPowerOf2(width)) || (!Accord.Math.Tools.IsPowerOf2(height)))
            {
                throw new InvalidImagePropertiesException("Image width and height should be power of 2.");
            }

            // create new complex image
            double[,] data = new double[width, height];

            if (sourceData.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                // do the job
                unsafe
                {
                    byte* src = (byte*)sourceData.ImageData.ToPointer();

                    // for each line
                    for (int y = 0; y < height; y++)
                    {
                        // for each pixel
                        for (int x = 0; x < width; x++, src++)
                        {
                            data[y, x] = Vector.Scale(*src, (byte)0, (byte)255, -1.0, 1.0);
                        }
                        src += srcOffset;
                    }
                }
            }
            else
            {
                // do the job
                unsafe
                {
                    ushort* src = (ushort*)sourceData.ImageData.ToPointer();

                    // for each line
                    for (int y = 0; y < height; y++)
                    {
                        // for each pixel
                        for (int x = 0; x < width; x++, src++)
                        {
                            data[y, x] = Vector.Scale(*src, 0, 65535, -1.0, 1.0);
                        }
                        src += srcOffset;
                    }
                }
            }

            // Apply the transform
            if (backward)
            {
                wavelet.Backward(data);
            }
            else
            {
                wavelet.Forward(data);
            }


            if (sourceData.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                unsafe
                {
                    byte* dst = (byte*)destinationData.ImageData.ToPointer();


                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, dst++)
                        {
                            *dst = (byte)Vector.Scale(data[y, x], -1, 1, 0, 255);
                        }
                        dst += dstOffset;
                    }
                }
            }
            else
            {
                unsafe
                {
                    ushort* dst = (ushort*)destinationData.ImageData.ToPointer();

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, dst++)
                        {
                            *dst = (ushort)Vector.Scale(data[y, x], -1, 1, 0, 65535);
                        }
                        dst += dstOffset;
                    }
                }

            }
        }
    }
}



