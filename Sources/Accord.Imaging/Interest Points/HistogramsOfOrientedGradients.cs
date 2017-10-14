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
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using Accord.Math;
    using Accord.Imaging;
    using Accord.Imaging.Filters;
    using Accord.Compat;

    /// <summary>
    ///   Histograms of Oriented Gradients (HOG) descriptor extractor.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Navneet Dalal and Bill Triggs, "Histograms of Oriented Gradients for Human Detection",
    ///       CVPR 2005. Available at: <a href="http://lear.inrialpes.fr/people/triggs/pubs/Dalal-cvpr05.pdf">
    ///       http://lear.inrialpes.fr/people/triggs/pubs/Dalal-cvpr05.pdf </a> </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The first example shows how to extract HOG descriptors from a standard test image:</para>
    ///   <code source="Unit Tests\Accord.Tests.Imaging\HistogramsOfOrientedGradientsTest.cs" region="doc_apply" />
    ///   
    /// <para>
    ///   The second example shows how to use HOG descriptors as part of a BagOfVisualWords (BoW) pipeline 
    ///   for image classification:</para>
    ///   <code source="Unit Tests\Accord.Tests.Vision\Imaging\BagOfVisualWordsTest.cs" region="doc_feature_lbp" />
    ///   <code source="Unit Tests\Accord.Tests.Vision\Imaging\BagOfVisualWordsTest.cs" region="doc_classification_feature_lbp" />
    /// </example>
    /// 
    public class HistogramsOfOrientedGradients : BaseFeatureExtractor<FeatureDescriptor>
    {

        int numberOfBins = 9;
        int cellSize = 6;  // size of the cell, in number of pixels
        int blockSize = 3; // size of the block, in number of cells
        bool normalize = true;

        double epsilon = 1e-10;
        double binWidth;

        float[,] direction;
        float[,] magnitude;
        double[,][] histograms;


        /// <summary>
        ///   Gets the size of a cell, in pixels. Default is 6.
        /// </summary>
        /// 
        public int CellSize
        {
            get { return cellSize; }
            set { cellSize = value; }
        }

        /// <summary>
        ///   Gets the size of a block, in pixels. Default is 3.
        /// </summary>
        /// 
        public int BlockSize
        {
            get { return blockSize; }
            set { blockSize = value; }
        }

        /// <summary>
        ///   Gets the number of histogram bins. Default is 9.
        /// </summary>
        /// 
        public int NumberOfBins
        {
            get { return numberOfBins; }
            set
            {
                this.numberOfBins = value;
                this.binWidth = (2.0 * System.Math.PI) / numberOfBins; // 0 to 360}
            }
        }

        /// <summary>
        ///   Gets the width of the histogram bin. This property is 
        ///   computed as <c>(2.0 * System.Math.PI) / numberOfBins</c>.
        /// </summary>
        /// 
        public double BinWidth
        {
            get { return this.binWidth; }
        }

        /// <summary>
        ///   Gets the matrix of orientations generated in 
        ///   the last call to <see cref="BaseFeatureExtractor{TFeature}.Transform(Bitmap)"/>.
        /// </summary>
        /// 
        public float[,] Direction { get { return direction; } }

        /// <summary>
        ///   Gets the matrix of magnitudes generated in 
        ///   the last call to <see cref="BaseFeatureExtractor{TFeature}.Transform(Bitmap)"/>.
        /// </summary>
        /// 
        public float[,] Magnitude { get { return magnitude; } }

        /// <summary>
        ///   Gets the histogram computed at each cell.
        /// </summary>
        /// 
        public double[,][] Histograms { get { return histograms; } }

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
        ///   Initializes a new instance of the <see cref="HistogramsOfOrientedGradients"/> class.
        /// </summary>
        /// 
        public HistogramsOfOrientedGradients()
        {
            init(numberOfBins, blockSize, cellSize);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="HistogramsOfOrientedGradients"/> class.
        /// </summary>
        /// 
        /// <param name="numberOfBins">The number of histogram bins.</param>
        /// <param name="blockSize">The size of a block, measured in cells.</param>
        /// <param name="cellSize">The size of a cell, measured in pixels.</param>
        /// 
        public HistogramsOfOrientedGradients(int numberOfBins = 9, int blockSize = 3, int cellSize = 6)
        {
            init(numberOfBins, blockSize, cellSize);
        }

        private void init(int numberOfBins, int blockSize, int cellSize)
        {
            this.NumberOfBins = numberOfBins;
            this.CellSize = cellSize;
            this.BlockSize = blockSize;

            base.SupportedFormats.UnionWith(new[] {
                PixelFormat.Format8bppIndexed,
                PixelFormat.Format24bppRgb,
                PixelFormat.Format32bppRgb,
                PixelFormat.Format32bppArgb });
        }

        /// <summary>
        ///   This method should be implemented by inheriting classes to implement the 
        ///   actual feature extraction, transforming the input image into a list of features.
        /// </summary>
        /// 
        protected override IEnumerable<FeatureDescriptor> InnerTransform(UnmanagedImage image)
        {
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


            // 1. Calculate partial differences
            if (direction == null || height > direction.GetLength(0) || width > direction.GetLength(1))
            {
                direction = new float[height, width];
                magnitude = new float[height, width];
            }
            else
            {
                System.Diagnostics.Debug.Write(String.Format("Reusing storage for direction and magnitude. " +
                    "Need ({0}, {1}), have ({1}, {2})", height, width, direction.Rows(), direction.Columns()));
            }


            unsafe
            {
                fixed (float* ptrDir = direction, ptrMag = magnitude)
                {
                    // Begin skipping first line
                    byte* src = (byte*)grayImage.ImageData.ToPointer() + stride;
                    float* dir = ptrDir + width;
                    float* mag = ptrMag + width;

                    // for each line
                    for (int y = 1; y < height - 1; y++)
                    {
                        // skip first column
                        dir++; mag++; src++;

                        // for each inner pixel in line (skipping first and last)
                        for (int x = 1; x < width - 1; x++, src++, dir++, mag++)
                        {
                            // Retrieve the pixel neighborhood
                            byte a11 = src[+stride + 1], a12 = src[+1], a13 = src[-stride + 1];
                            byte a21 = src[+stride + 0], /*  a22    */  a23 = src[-stride + 0];
                            byte a31 = src[+stride - 1], a32 = src[-1], a33 = src[-stride - 1];

                            // Convolution with horizontal differentiation kernel mask
                            float h = ((a11 + a12 + a13) - (a31 + a32 + a33)) * 0.166666667f;

                            // Convolution with vertical differentiation kernel mask
                            float v = ((a11 + a21 + a31) - (a13 + a23 + a33)) * 0.166666667f;

                            // Store angles and magnitudes directly
                            *dir = (float)Math.Atan2(v, h);
                            *mag = (float)Math.Sqrt(h * h + v * v);
                        }

                        // Skip last column
                        dir++; mag++; src += offset + 1;
                    }
                }
            }

            // Free some resources which wont be needed anymore
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
                grayImage.Dispose();



            // 2. Compute cell histograms
            int cellCountX = (int)Math.Floor(width / (double)cellSize);
            int cellCountY = (int)Math.Floor(height / (double)cellSize);

            if (histograms == null || cellCountX > histograms.GetLength(0) || cellCountY > histograms.GetLength(1))
            {
                this.histograms = new double[cellCountX, cellCountY][];
                for (int i = 0; i < cellCountX; i++)
                    for (int j = 0; j < cellCountY; j++)
                        this.histograms[i, j] = new double[NumberOfBins];
            }
            else
            {
                System.Diagnostics.Debug.Write(String.Format("Reusing storage for histograms. " +
                    "Need ({0}, {1}), have ({1}, {2})", cellCountX, cellCountY, histograms.Rows(), histograms.Columns()));
            }

            // For each cell
            for (int i = 0; i < cellCountX; i++)
            {
                for (int j = 0; j < cellCountY; j++)
                {
                    // Compute the histogram
                    double[] histogram = this.histograms[i, j];
                    Array.Clear(histogram, 0, histogram.Length);

                    int startCellX = i * cellSize;
                    int startCellY = j * cellSize;

                    // for each pixel in the cell
                    for (int x = 0; x < cellSize; x++)
                    {
                        for (int y = 0; y < cellSize; y++)
                        {
                            double ang = direction[startCellY + y, startCellX + x];
                            double mag = magnitude[startCellY + y, startCellX + x];

                            // Get its angular bin
                            int bin = (int)System.Math.Floor((ang + Math.PI) * binWidth);

                            histogram[bin] += mag;
                        }
                    }
                }
            }

            // 3. Group the cells into larger, normalized blocks
            int blocksCountX = (int)Math.Floor(cellCountX / (double)blockSize);
            int blocksCountY = (int)Math.Floor(cellCountY / (double)blockSize);

            var blocks = new List<FeatureDescriptor>();

            for (int i = 0; i < blocksCountX; i++)
            {
                for (int j = 0; j < blocksCountY; j++)
                {
                    double[] block = new double[blockSize * blockSize * numberOfBins];

                    int startBlockX = i * blockSize;
                    int startBlockY = j * blockSize;
                    int c = 0;

                    // for each cell in the block
                    for (int x = 0; x < blockSize; x++)
                    {
                        for (int y = 0; y < blockSize; y++)
                        {
                            double[] histogram = histograms[startBlockX + x, startBlockY + y];

                            // Copy all histograms to the block vector
                            for (int k = 0; k < histogram.Length; k++)
                                block[c++] = histogram[k];
                        }
                    }

                    // TODO: Remove this block and instead propose a general architecture 
                    //       for applying normalizations to descriptor blocks
                    if (normalize)
                        block.Divide(block.Euclidean() + epsilon, result: block);

                    blocks.Add(block);
                }
            }

            return blocks;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        protected override object Clone(ISet<PixelFormat> supportedFormats)
        {
            var clone = new HistogramsOfOrientedGradients(numberOfBins, blockSize, cellSize);
            clone.SupportedFormats = supportedFormats;
            clone.epsilon = epsilon;
            clone.normalize = normalize;
            return clone;
        }

    }
}
