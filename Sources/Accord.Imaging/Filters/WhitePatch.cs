// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
//
// This file was contributed to the project by Diego Catalano, based
// on the MATLAB implementation by Juan Manuel Perez Rua, distributed
// under the BSD license. The original license terms are given below:
//
//   Copyright © Juan Manuel Perez Rua, 2012
//
//   Redistribution and use in source and binary forms, with or without
//   modification, are permitted provided that the following conditions are
//   met:
//
//       * Redistributions of source code must retain the above copyright
//         notice, this list of conditions and the following disclaimer.
//       * Redistributions in binary form must reproduce the above copyright
//         notice, this list of conditions and the following disclaimer in
//         the documentation and/or other materials provided with the distribution
//  
//   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//   AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
//   ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
//   LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
//   CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
//   SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
//   INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
//   CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
//   ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
//   POSSIBILITY OF SUCH DAMAGE.
//

namespace Accord.Imaging.Filters
{
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using AForge.Imaging;
    using AForge.Imaging.Filters;

    /// <summary>
    ///   White Patch filter for color normalization.
    /// </summary>
    /// 
    public class WhitePatch : BaseInPlaceFilter
    {
        Dictionary<PixelFormat, PixelFormat> formatTranslations;

        /// <summary>
        ///   Format translations dictionary.
        /// </summary>
        /// 
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="WhitePatch"/> class.
        /// </summary>
        /// 
        public WhitePatch()
        {
            formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        /// <summary>
        ///   Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        /// 
        protected unsafe override void ProcessFilter(UnmanagedImage image)
        {
            int width = image.Width;
            int height = image.Height;

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
            int stride = image.Stride;
            int offset = stride - image.Width * pixelSize;

            byte* src = (byte*)image.ImageData.ToPointer();

            // Get maximum color image values
            int maxR = 0, maxG = 0, maxB = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (src[RGB.R] > maxR) maxR = src[RGB.R];
                    if (src[RGB.G] > maxG) maxG = src[RGB.G];
                    if (src[RGB.B] > maxB) maxB = src[RGB.B];
                }
            }

            double kr = maxR > 0 ? (255.0 / maxR) : 0;
            double kg = maxG > 0 ? (255.0 / maxG) : 0;
            double kb = maxB > 0 ? (255.0 / maxB) : 0;


            src = (byte*)image.ImageData.ToPointer();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, src += pixelSize)
                {
                    double r = kr * src[RGB.R];
                    double g = kg * src[RGB.G];
                    double b = kb * src[RGB.B];

                    src[RGB.R] = (byte)(r > 255 ? 255 : r);
                    src[RGB.G] = (byte)(g > 255 ? 255 : g);
                    src[RGB.B] = (byte)(b > 255 ? 255 : b);
                }

                src += offset;
            }
        }
    }
}
