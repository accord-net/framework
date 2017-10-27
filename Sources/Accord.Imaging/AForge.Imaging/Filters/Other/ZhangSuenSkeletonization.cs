// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Hashem Zawary, 2017
// hashemzawary@gmail.com
// https://www.linkedin.com/in/hashem-zavvari-53b01457
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
    using Accord.Imaging;
    using Accord.Imaging.Filters;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Zhang-Suen skeletonization filter.
    /// </summary>
    /// 
    /// <remarks><para>Zhang-Suen Thinning Algorithm. The filter uses
    /// <see cref="Background"/> and <see cref="Foreground"/> colors to distinguish
    /// between object and background.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// ZhangSuenSkeletonization filter = new ZhangSuenSkeletonization( );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="..\images\imaging\sample14.png" width="150" height="150" />
    /// <para><b>Result image:</b></para>
    /// <img src="..\images\imaging\ZhangSuen_skeletonization.png" width="150" height="150" />
    /// </remarks>
    /// 
    public class ZhangSuenSkeletonization : BaseUsingCopyPartialFilter
    {
        private byte bg = byte.MinValue;
        private byte fg = byte.MaxValue;

        // private format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        /// 
        /// <remarks><para>See <see cref="IFilterInformation.FormatTranslations"/>
        /// documentation for additional information.</para></remarks>
        /// 
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        /// Background pixel color.
        /// </summary>
        /// 
        /// <remarks><para>The property sets background (none object) color to look for.</para>
        /// 
        /// <para>Default value is set to <b>0</b> - black.</para></remarks>
        /// 
        public byte Background
        {
            get { return bg; }
            set { bg = value; }
        }

        /// <summary>
        /// Foreground pixel color.
        /// </summary>
        /// 
        /// <remarks><para>The property sets objects' (none background) color to look for.</para>
        /// 
        /// <para>Default value is set to <b>255</b> - white.</para></remarks>
        /// 
        public byte Foreground
        {
            get { return fg; }
            set { fg = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleSkeletonization"/> class.
        /// </summary>
        public ZhangSuenSkeletonization()
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleSkeletonization"/> class.
        /// </summary>
        /// 
        /// <param name="bg">Background pixel color.</param>
        /// <param name="fg">Foreground pixel color.</param>
        /// 
        public ZhangSuenSkeletonization(byte bg, byte fg) : this()
        {
            this.bg = bg;
            this.fg = fg;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="source">Source image data.</param>
        /// <param name="destination">Destination image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        protected override unsafe void ProcessFilter(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
        {
            source.Copy(destination);

            // processing start and stop X,Y positions
            var startX = 1;
            var startY = 1;
            var stopX = rect.Width - 1;
            var stopY = rect.Height - 1;

            var dstStride = destination.Stride;
            var dstOffset = dstStride - rect.Width + 2;

            var delSize = (rect.Width - 2) * (rect.Height - 2);

            // do the job
            var dst0 = (byte*)destination.ImageData.ToPointer();
            var del0 = stackalloc byte[delSize]; for (var i = 0; i < delSize; i++) del0[i] = 0xFF;

            bool endOfAlgorithm;

            do
            {
                endOfAlgorithm = true;

                // Setp 1
                Process(startX, startY, stopX, stopY,
                    dst0 + (rect.Top + 1) * dstStride + rect.Left + 1, dstStride, dstOffset,
                    del0,
                    new[] { 1, 3, 5, 3, 5, 7 },
                    ref endOfAlgorithm);

                // Deletion
                delete(startX, startY, stopX, stopY,
                    dst0 + (rect.Top + 1) * dstStride + rect.Left + 1, dstOffset,
                    del0);

                // Setp 2
                Process(startX, startY, stopX, stopY,
                    dst0 + (rect.Top + 1) * dstStride + rect.Left + 1, dstStride, dstOffset,
                    del0,
                    new[] { 1, 3, 7, 1, 5, 7 },
                    ref endOfAlgorithm);

                // Deletion
                delete(startX, startY, stopX, stopY,
                    dst0 + (rect.Top + 1) * dstStride + rect.Left + 1, dstOffset,
                    del0);

            } while (!endOfAlgorithm);

            #region Set colors

            // allign pointers to the first pixel to process
            dst0 = dst0 + (rect.Top + 1) * dstStride + rect.Left + 1;

            // for each line
            for (var y = startY; y < stopY; y++)
            {
                // for each pixel
                for (var x = startX; x < stopX; x++, dst0++)
                    *dst0 = (*dst0) == byte.MinValue ? bg : fg;

                dst0 += dstOffset;
            }

            #endregion

        }

        unsafe void Process(
            int startX, int startY, int stopX, int stopY,
            byte* thinnedImageAddress, int thinnedImageStride, int thinnedImageOffset,
            byte* delVectorAddress,
            int[] sixNeighbors,
            ref bool endOfAlgorithm)
        {
            // for each line
            for (var y = startY; y < stopY; y++)
            {
                // for each pixel
                for (var x = startX; x < stopX; x++, thinnedImageAddress++, delVectorAddress++)
                {
                    var neighbors = new byte[]
                    {
                            *(thinnedImageAddress),
                            *(thinnedImageAddress - thinnedImageStride),
                            *(thinnedImageAddress - thinnedImageStride + 1),
                            *(thinnedImageAddress + 1),
                            *(thinnedImageAddress + thinnedImageStride + 1),
                            *(thinnedImageAddress + thinnedImageStride),
                            *(thinnedImageAddress + thinnedImageStride - 1),
                            *(thinnedImageAddress - 1),
                            *(thinnedImageAddress - thinnedImageStride - 1),
                            *(thinnedImageAddress - thinnedImageStride)
                    };

                    var sumNeighbors = 0;
                    for (var i = 1; i < neighbors.Length - 1; i++)
                        sumNeighbors += neighbors[i];

                    // Conditions
                    if (neighbors[0] == byte.MaxValue &&
                        // sumNeighbors <= 6 * 255 && sumNeighbors >= 2 * 255
                        sumNeighbors < 1785 && sumNeighbors > 255 &&
                        neighbors[sixNeighbors[0]] * neighbors[sixNeighbors[1]] * neighbors[sixNeighbors[2]] == byte.MinValue &&
                        neighbors[sixNeighbors[3]] * neighbors[sixNeighbors[4]] * neighbors[sixNeighbors[5]] == byte.MinValue)
                    {
                        // No. of 0, 255 patterns (transitions from 0 to 255) in the ordered sequence
                        var numberOfTransitions = 0;
                        for (var i = 1; i < neighbors.Length - 1; i++)
                            if (neighbors[i] == byte.MinValue && neighbors[i + 1] == byte.MaxValue)
                                numberOfTransitions++;

                        if (numberOfTransitions == 1)
                        {
                            *delVectorAddress = 0x00;
                            endOfAlgorithm = false;
                        }
                    }
                }
                thinnedImageAddress += thinnedImageOffset;
            }
        }

        unsafe void delete(
            int startX, int startY, int stopX, int stopY,
            byte* thinnedImageAddress, int thinnedImageOffset,
            byte* delVectorAddress)
        {
            // for each line
            for (var y = startY; y < stopY; y++)
            {
                // for each pixel
                for (var x = startX; x < stopX; x++, thinnedImageAddress++, delVectorAddress++)
                    *thinnedImageAddress &= *delVectorAddress;

                thinnedImageAddress += thinnedImageOffset;
            }
        }
    }
}
