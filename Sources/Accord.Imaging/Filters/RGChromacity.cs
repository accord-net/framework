// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Diego Catalano, 2013
// diego.catalano at live.com
//
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

using System.Collections.Generic;
using System.Drawing.Imaging;

namespace Accord.Imaging.Filters
{
    /// <summary>
    ///   RG Chromaticity.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia contributors. "rg chromaticity." Wikipedia, The Free Encyclopedia. Wikipedia,
    ///       The Free Encyclopedia. Available at http://en.wikipedia.org/wiki/Rg_chromaticity </description></item>
    ///   </list>
    /// </para>
    /// </remarks>
    /// 
    public class RGChromacity : BaseInPlaceFilter
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
        ///   Initializes a new instance of the <see cref="RGChromacity"/> class.
        /// </summary>
        /// 
        public RGChromacity()
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

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, src += pixelSize)
                {
                    double sum = src[RGB.R] + src[RGB.G] + src[RGB.B];
                    sum = sum == 0 ? 1 : sum;

                    double red = src[RGB.R] / sum;
                    double green = src[RGB.G] / sum;
                    double blue = 1 - red - green;

                    src[RGB.R] = (byte)(red * 255);
                    src[RGB.G] = (byte)(green * 255);
                    src[RGB.B] = (byte)(blue * 255);
                }
                src += offset;
            }
        }
    }
}