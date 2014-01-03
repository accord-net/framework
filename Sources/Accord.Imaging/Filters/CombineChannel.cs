// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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
    using AForge.Imaging;
    using AForge.Imaging.Filters;
    using System;

    /// <summary>
    ///   Combine channel filter.
    /// </summary>
    /// 
    public class CombineChannel : BaseInPlaceFilter
    {

        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        private int baseWidth;
        private int baseHeight;
        private UnmanagedImage[] channels;

        /// <summary>
        ///   Format translations dictionary.
        /// </summary>
        /// 
        /// <remarks>
        ///   <para>The dictionary defines, which pixel formats are supported for
        ///   source images and which pixel format will be used for resulting image.</para>
        /// 
        ///   <para>See <see cref="P:AForge.Imaging.Filters.IFilterInformation.FormatTranslations"/>
        ///   for more information.</para>
        /// </remarks>
        /// 
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        ///   Constructs a new CombineChannel filter.
        /// </summary>
        /// 
        public CombineChannel(params UnmanagedImage[] channels)
        {
            if (channels == null)
                throw new ArgumentNullException("channels");
            if (channels.Length < 2)
                throw new ArgumentException("There must be at least two channels to be combined.", "channels");

            this.channels = channels;
            this.baseWidth = channels[0].Width;
            this.baseHeight = channels[0].Height;

            foreach (var c in channels)
                if (c.Width != baseWidth || c.Height != baseHeight)
                    throw new ArgumentException( "All images must have the same dimensions.", "channels");

            formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format64bppArgb;
            formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
        }


        /// <summary>
        ///   Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        /// 
        protected override void ProcessFilter(UnmanagedImage image)
        {
            int pixelSize = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;

            // get source image size
            int width = image.Width;
            int height = image.Height;
            int offset = image.Stride - width * pixelSize;

            // check is the same size
            if (image.Height != baseHeight || image.Width != baseWidth)
                throw new InvalidImagePropertiesException("Image does not have expected dimensions.", "image");

            unsafe
            {
                if (pixelSize == 8)
                {
                    // for each channel
                    for (int c = 0; c < channels.Length; c++)
                    {
                        byte* dst = (byte*)((int)image.ImageData + c);
                        byte* src = (byte*)channels[c].ImageData;

                        // copy channel to image
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                *(dst += pixelSize) = *(src++);
                            }
                        }
                    }
                }
                else if (pixelSize == 16)
                {
                    // for each channel
                    for (int c = 0; c < channels.Length; c++)
                    {
                        short* dst = (short*)((int)image.ImageData + c);
                        short* src = (short*)channels[c].ImageData;

                        // copy channel to image
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                *(dst += pixelSize) = *(src++);
                            }
                        }
                    }
                }
            }
        }
    }
}
