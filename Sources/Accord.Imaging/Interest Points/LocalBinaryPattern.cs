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
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using Accord.Math;
    using AForge.Imaging;
    using AForge.Imaging.Filters;

    /// <summary>
    ///   Local Binary Patterns.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///    Local binary patterns (LBP) is a type of feature used for classification
    ///    in computer vision. LBP is the particular case of the Texture Spectrum 
    ///    model proposed in 1990. LBP was first described in 1994. It has since 
    ///    been found to be a powerful feature for texture classification; it has
    ///    further been determined that when LBP is combined with the Histogram of
    ///    oriented gradients (HOG) classifier, it improves the detection performance
    ///    considerably on some datasets. </para>
    /// 
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia Contributors, "Local Binary Patterns". Available at
    ///       http://en.wikipedia.org/wiki/Local_binary_patterns </description></item>
    ///   </list>
    /// </para>
    /// </remarks>
    /// 
    public class LocalBinaryPattern : IFeatureDetector<FeatureDescriptor>
    {
        const int numberOfBins = 256;

        int cellSize = 6;  // size of the cell, in number of pixels
        int blockSize = 3; // size of the block, in number of cells
        bool normalize = true;
        
        double epsilon = 1e-10;


        int[,] patterns;
        int[,][] histograms;


        /// <summary>
        ///   Gets the size of a cell, in pixels.
        /// </summary>
        /// 
        public int CellSize { get { return cellSize; } }

        /// <summary>
        ///   Gets the size of a block, in pixels.
        /// </summary>
        /// 
        public int BlockSize { get { return blockSize; } }

        /// <summary>
        ///   Gets the set of local binary patterns computed for each
        ///   pixel in the last call to to <see cref="ProcessImage(Bitmap)"/>.
        /// </summary>
        /// 
        public int[,] Patterns { get { return patterns; } }

        /// <summary>
        ///   Gets the histogram computed at each cell.
        /// </summary>
        /// 
        public int[,][] Histograms { get { return histograms; } }

        /// <summary>
        ///   Gets or sets whether to normalize final 
        ///   histogram feature vectors. Default is true.
        /// </summary>
        /// 
        public bool Normalize
        {
            get { return normalize; }
            set { normalize = value; }
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="LocalBinaryPattern"/> class.
        /// </summary>
        /// 
        /// <param name="blockSize">
        ///   The size of a block, measured in cells. Default is 3.</param>
        /// <param name="cellSize">
        ///   The size of a cell, measured in pixels. If set to zero, the entire
        ///   image will be used at once, forming a single block. Default is 6.</param>
        /// <param name="normalize">
        ///   Whether to normalize generated histograms. Default is true.</param>
        /// 
        public LocalBinaryPattern(int blockSize = 3, int cellSize = 6, bool normalize = true)
        {
            this.cellSize = cellSize;
            this.blockSize = blockSize;
            this.normalize = normalize;
        }


        /// <summary>
        ///   Process image looking for interest points.
        /// </summary>
        /// 
        /// <param name="image">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found features points.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">
        ///   The source image has incorrect pixel format.
        /// </exception>
        /// 
        public unsafe List<double[]> ProcessImage(UnmanagedImage image)
        {

            // check image format
            if (
                (image.PixelFormat != PixelFormat.Format8bppIndexed) &&
                (image.PixelFormat != PixelFormat.Format24bppRgb) &&
                (image.PixelFormat != PixelFormat.Format32bppRgb) &&
                (image.PixelFormat != PixelFormat.Format32bppArgb)
                )
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }

            // make sure we have grayscale image
            UnmanagedImage grayImage = null;

            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                grayImage = image;
            }
            else
            {
                // create temporary grayscale image
                grayImage = Grayscale.CommonAlgorithms.BT709.Apply(image);
            }


            // get source image size
            int width = grayImage.Width;
            int height = grayImage.Height;
            int stride = grayImage.Stride;
            int offset = stride - width;

            // 1. Calculate 8-pixel neighborhood binary patterns 
            patterns = new int[height, width];

            fixed (int* ptrPatterns = patterns)
            {
                // Begin skipping first line
                byte* src = (byte*)grayImage.ImageData.ToPointer() + stride;
                int* neighbors = ptrPatterns + width;

                // for each line
                for (int y = 1; y < height - 1; y++)
                {
                    // skip first column
                    neighbors++; src++;

                    // for each inner pixel in line (skipping first and last)
                    for (int x = 1; x < width - 1; x++, src++, neighbors++)
                    {
                        // Retrieve the pixel neighborhood
                        byte a11 = src[+stride + 1], a12 = src[+1], a13 = src[-stride + 1];
                        byte a21 = src[+stride + 0], a22 = src[0], a23 = src[-stride + 0];
                        byte a31 = src[+stride - 1], a32 = src[-1], a33 = src[-stride - 1];

                        int sum = 0;
                        if (a22 < a11) sum += 1 << 0;
                        if (a22 < a12) sum += 1 << 1;
                        if (a22 < a13) sum += 1 << 2;
                        if (a22 < a21) sum += 1 << 3;
                        if (a22 < a23) sum += 1 << 4;
                        if (a22 < a31) sum += 1 << 5;
                        if (a22 < a32) sum += 1 << 6;
                        if (a22 < a33) sum += 1 << 7;

                        *neighbors = sum;
                    }

                    // Skip last column
                    neighbors++; src += offset + 1;
                }
            }

            // Free some resources which wont be needed anymore
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
                grayImage.Dispose();


            // 2. Compute cell histograms
            int cellCountX;
            int cellCountY;

            if (cellSize > 0)
            {
                cellCountX = (int)Math.Floor(width / (double)cellSize);
                cellCountY = (int)Math.Floor(height / (double)cellSize);
                histograms = new int[cellCountX, cellCountY][];

                // For each cell
                for (int i = 0; i < cellCountX; i++)
                {
                    for (int j = 0; j < cellCountY; j++)
                    {
                        // Compute the histogram
                        int[] histogram = new int[numberOfBins];

                        int startCellX = i * cellSize;
                        int startCellY = j * cellSize;

                        // for each pixel in the cell
                        for (int x = 0; x < cellSize; x++)
                            for (int y = 0; y < cellSize; y++)
                                histogram[patterns[startCellY + y, startCellX + x]]++;

                        histograms[i, j] = histogram;
                    }
                }
            }
            else
            {
                cellCountX = cellCountY = 1;
                int[] histogram = new int[numberOfBins];

                for (int i = 0; i < height; i++)
                    for (int j = 0; j < width; j++)
                        histogram[patterns[i, j]]++;

                histograms = new int[,][] { { histogram } };
            }

            // 3. Group the cells into larger, normalized blocks
            int blocksCountX;
            int blocksCountY;

            if (blockSize > 0)
            {
                blocksCountX = (int)Math.Floor(cellCountX / (double)blockSize);
                blocksCountY = (int)Math.Floor(cellCountY / (double)blockSize);
            }
            else 
            {
                blockSize = blocksCountX = blocksCountY = 1;
            }

            List<double[]> blocks = new List<double[]>();

            for (int i = 0; i < blocksCountX; i++)
            {
                for (int j = 0; j < blocksCountY; j++)
                {
                    double[] v = new double[blockSize * blockSize * numberOfBins];

                    int startBlockX = i * blockSize;
                    int startBlockY = j * blockSize;
                    int c = 0;

                    // for each cell in the block
                    for (int x = 0; x < blockSize; x++)
                    {
                        for (int y = 0; y < blockSize; y++)
                        {
                            int[] histogram =
                                histograms[startBlockX + x, startBlockY + y];

                            // Copy all histograms to the block vector
                            for (int k = 0; k < histogram.Length; k++)
                                v[c++] = histogram[k];
                        }
                    }

                    double[] block = (normalize) ?
                        v.Divide(v.Euclidean() + epsilon) : v;

                    blocks.Add(block);
                }
            }

            return blocks;
        }



        /// <summary>
        ///   Process image looking for interest points.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found interest points.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">
        ///   The source image has incorrect pixel format.
        /// </exception>
        /// 
        public List<double[]> ProcessImage(BitmapData imageData)
        {
            return ProcessImage(new UnmanagedImage(imageData));
        }

        /// <summary>
        ///   Process image looking for interest points.
        /// </summary>
        /// 
        /// <param name="image">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found interest points.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">
        ///   The source image has incorrect pixel format.
        /// </exception>
        /// 
        public List<double[]> ProcessImage(Bitmap image)
        {
            // check image format
            if (
                (image.PixelFormat != PixelFormat.Format8bppIndexed) &&
                (image.PixelFormat != PixelFormat.Format24bppRgb) &&
                (image.PixelFormat != PixelFormat.Format32bppRgb) &&
                (image.PixelFormat != PixelFormat.Format32bppArgb)
                )
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source");
            }

            // lock source image
            BitmapData imageData = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, image.PixelFormat);

            List<double[]> blocks;

            try
            {
                // process the image
                blocks = ProcessImage(new UnmanagedImage(imageData));
            }
            finally
            {
                // unlock image
                image.UnlockBits(imageData);
            }

            return blocks;
        }


        List<FeatureDescriptor> IFeatureDetector<FeatureDescriptor, double[]>.ProcessImage(Bitmap image)
        {
            return ProcessImage(image).ConvertAll(p => new FeatureDescriptor(p));
        }

        List<FeatureDescriptor> IFeatureDetector<FeatureDescriptor, double[]>.ProcessImage(BitmapData imageData)
        {
            return ProcessImage(imageData).ConvertAll(p => new FeatureDescriptor(p));
        }

        List<FeatureDescriptor> IFeatureDetector<FeatureDescriptor, double[]>.ProcessImage(UnmanagedImage image)
        {
            return ProcessImage(image).ConvertAll(p => new FeatureDescriptor(p));
        }
    }
}
