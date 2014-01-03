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

namespace Accord.Imaging
{

    /// <summary>
    ///   Static tool functions for imaging.
    /// </summary>
    /// 
    public static unsafe class UnsafeTools
    {

        /// <summary>
        ///   Computes the sum of all pixels 
        ///   within a given image region.
        /// </summary>
        /// 
        /// <param name="src">The image region.</param>
        /// <param name="width">The region width.</param>
        /// <param name="height">The region height.</param>
        /// <param name="stride">The image stride.</param>
        /// 
        /// <returns>The sum of all pixels within the region.</returns>
        /// 
        public static int Sum(byte* src, int width, int height, int stride)
        {
            int sum = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, src++)
                    sum += *src;

                src += stride - width;
            }

            return sum;
        }

        /// <summary>
        ///   Computes the mean pixel value 
        ///   within a given image region.
        /// </summary>
        /// 
        /// <param name="src">The image region.</param>
        /// <param name="width">The region width.</param>
        /// <param name="height">The region height.</param>
        /// <param name="stride">The image stride.</param>
        /// 
        /// <returns>The mean pixel value within the region.</returns>
        /// 
        public static double Mean(byte* src, int width, int height, int stride)
        {
            return Sum(src, width, height, stride) / (double)(width * height);
        }

        /// <summary>
        ///   Computes the pixel scatter 
        ///   within a given image region.
        /// </summary>
        /// 
        /// <param name="src">The image region.</param>
        /// <param name="width">The region width.</param>
        /// <param name="height">The region height.</param>
        /// <param name="stride">The image stride.</param>
        /// <param name="mean">The region pixel mean.</param>
        /// 
        /// <returns>The scatter value within the region.</returns>
        /// 
        public static double Scatter(byte* src, int width, int height, int stride, double mean)
        {
            double scatter = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, src++)
                    scatter += (*src - mean) * (*src - mean);

                src += stride - width;
            }

            return scatter;
        }

        /// <summary>
        ///   Computes the pixel variance 
        ///   within a given image region.
        /// </summary>
        /// 
        /// <param name="src">The image region.</param>
        /// <param name="width">The region width.</param>
        /// <param name="height">The region height.</param>
        /// <param name="stride">The image stride.</param>
        /// <param name="mean">The region pixel mean.</param>
        /// 
        /// <returns>The variance value within the region.</returns>
        /// 
        public static double Variance(byte* src, int width, int height, int stride, double mean)
        {
            return Scatter(src, width, height, stride, mean) / (double)(width * height - 1);
        }
    }
}
