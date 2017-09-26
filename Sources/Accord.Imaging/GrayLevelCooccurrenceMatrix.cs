// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Diego Catalano, 2013
// diego.catalano at live.com
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
    using Filters;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Collections.Generic;

    /// <summary>
    ///   Co-occurrence Degree.
    /// </summary>
    /// 
    public enum CooccurrenceDegree
    {
        /// <summary>
        ///   Find co-occurrences at 0° degrees.
        /// </summary>
        /// 
        Degree0 = 0,

        /// <summary>
        ///   Find co-occurrences at 45° degrees.
        /// </summary>
        /// 
        Degree45 = 45,

        /// <summary>
        ///   Find co-occurrences at 90° degrees.
        /// </summary>
        /// 
        Degree90 = 90,

        /// <summary>
        ///   Find co-occurrences at 135° degrees.
        /// </summary>
        /// 
        Degree135 = 135
    };


    /// <summary>
    ///   Gray-Level Co-occurrence Matrix (GLCM).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   A co-occurrence matrix or co-occurrence distribution is a matrix that is defined over an image to 
    ///   be the distribution of co-occurring pixel values (grayscale values, or colors) at a given offset.</para>
    ///   
    /// <para>
    ///   Any matrix or pair of matrices can be used to generate a co-occurrence matrix, though their most 
    ///   common application has been in measuring texture in images, so the typical definition, as above, 
    ///   assumes that the matrix is an image. It is also possible to define the matrix across two different
    ///   images.Such a matrix can then be used for color mapping.</para>
    /// </remarks>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Mryka Hall-Beyer, "The GLCM Tutorial Home Page", The GLCM Tutorial Home Page.
    ///       Available in: http://www.fp.ucalgary.ca/mhallbey/tutorial.htm </description></item>
    ///     <item><description><a href="https://en.wikipedia.org/wiki/Co-occurrence_matrix">
    ///       Wikipedia contributors. "Co-occurrence matrix." Wikipedia, The Free Encyclopedia. 
    ///       Wikipedia, The Free Encyclopedia, 7 Sep. 2016. Web. 27 Jan. 2017. Available in 
    ///       https://en.wikipedia.org/wiki/Co-occurrence_matrix </a></description></item>
    ///   </list>
    /// </para>   
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   Gray-level Cooccurrence matrices can be computed directly from images:</para>
    ///   <code source="Unit Tests\Accord.Tests.Imaging\GrayLevelCooccurrenceMatrixTest.cs" region="doc_learn" />
    ///   
    /// <para>
    ///   These matrices also play a major role in the computation of <see cref="Haralick"/> descriptors. For
    ///   more examples, including on how to use those matrices for image classification, please see <see cref="Haralick"/>
    ///   and <see cref="HaralickDescriptor"/> documentation pages.</para>
    /// </example>
    /// 
    /// <seealso cref="Haralick"/>
    /// <seealso cref="HaralickDescriptor"/>
    /// 
    public class GrayLevelCooccurrenceMatrix : ICloneable
    {
        private CooccurrenceDegree degree;
        private bool autoGray = true;
        private bool normalize = true;
        private int numPairs = 0;
        private int distance = 1;


        /// <summary>
        ///   Gets or sets whether the maximum value of gray should be
        ///   automatically computed from the image. If set to false,
        ///   the maximum gray value will be assumed 255.
        /// </summary>
        /// 
        public bool AutoGray
        {
            get { return autoGray; }
            set { autoGray = value; }
        }

        /// <summary>
        ///   Gets or sets whether the produced GLCM should be normalized,
        ///   dividing each element by the number of pairs. Default is true.
        /// </summary>
        /// 
        /// <value>
        ///   <c>true</c> if the GLCM should be normalized; otherwise, <c>false</c>.
        /// </value>
        /// 
        public bool Normalize
        {
            get { return normalize; }
            set { normalize = value; }
        }

        /// <summary>
        ///   Gets or sets the direction at which the co-occurrence should 
        ///   be found. Default is <see cref="CooccurrenceDegree.Degree0"/>.
        /// </summary>
        /// 
        public CooccurrenceDegree Degree
        {
            get { return degree; }
            set { degree = value; }
        }

        /// <summary>
        ///   Gets or sets the distance at which the 
        ///   texture should be analyzed. Default is 1.
        /// </summary>
        /// 
        public int Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        /// <summary>
        ///   Gets the number of pairs registered during the
        ///   last <see cref="Compute(UnmanagedImage)">computed GLCM</see>.
        /// </summary>
        /// 
        public int Pairs
        {
            get { return numPairs; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GrayLevelCooccurrenceMatrix"/> class.
        /// </summary>
        /// 
        public GrayLevelCooccurrenceMatrix()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GrayLevelCooccurrenceMatrix"/> class.
        /// </summary>
        /// 
        /// <param name="distance">The distance at which the texture should be analyzed. Default is 1.</param>
        /// <param name="degree">The direction to look for co-occurrences. Default is <see cref="CooccurrenceDegree.Degree0"/>.</param>
        /// 
        public GrayLevelCooccurrenceMatrix(int distance = 1, CooccurrenceDegree degree = CooccurrenceDegree.Degree0)
        {
            this.distance = distance;
            this.degree = degree;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GrayLevelCooccurrenceMatrix"/> class.
        /// </summary>
        /// 
        /// <param name="distance">The distance at which the texture should be analyzed. Default is 1.</param>
        /// <param name="degree">The direction to look for co-occurrences. Default is <see cref="CooccurrenceDegree.Degree0"/>.</param>
        /// <param name="autoGray">Whether the maximum value of gray should be
        ///   automatically computed from the image. Default is true.</param>
        /// <param name="normalize">Whether the produced GLCM should be normalized,
        ///   dividing each element by the number of pairs. Default is true.</param>
        /// 
        public GrayLevelCooccurrenceMatrix(int distance, CooccurrenceDegree degree,
            bool normalize = true, bool autoGray = true)
        {
            this.distance = distance;
            this.degree = degree;
            this.normalize = normalize;
            this.autoGray = autoGray;
        }

        /// <summary>
        ///   Computes the Gray-level Co-occurrence Matrix (GLCM) 
        ///   for the given source image.
        /// </summary>
        /// 
        /// <param name="source">The source image.</param>
        /// 
        /// <returns>A square matrix of double-precision values containing
        /// the GLCM for the given <paramref name="source"/>.</returns>
        /// 
        public double[,] Compute(Bitmap source)
        {
            var bitmapData = source.LockBits(ImageLockMode.ReadOnly);

            try
            {
                return Compute(bitmapData);
            }
            finally
            {
                source.UnlockBits(bitmapData);
            }
        }

        /// <summary>
        ///   Computes the Gray-level Co-occurrence Matrix (GLCM) 
        ///   for the given source image.
        /// </summary>
        /// 
        /// <param name="source">The source image.</param>
        /// 
        /// <returns>A square matrix of double-precision values containing
        /// the GLCM for the given <paramref name="source"/>.</returns>
        /// 
        public double[,] Compute(BitmapData source)
        {
            using (var unmanagedImage = new UnmanagedImage(source))
                return Compute(unmanagedImage);
        }

        /// <summary>
        ///   Computes the Gray-level Co-occurrence Matrix (GLCM) 
        ///   for the given source image.
        /// </summary>
        /// 
        /// <param name="source">The source image.</param>
        /// 
        /// <returns>A square matrix of double-precision values containing
        /// the GLCM for the given <paramref name="source"/>.</returns>
        /// 
        public double[,] Compute(UnmanagedImage source)
        {
            return Compute(source, new Rectangle(0, 0, source.Width, source.Height));
        }

        /// <summary>
        ///   Computes the Gray-level Co-occurrence Matrix for the given matrix.
        /// </summary>
        /// 
        /// <param name="source">The source image.</param>
        /// <param name="region">A region of the source image where
        ///  the GLCM should be computed for.</param>
        /// 
        /// <returns>A square matrix of double-precision values containing the GLCM for the
        ///   <paramref name="region"/> of the given <paramref name="source"/>.</returns>
        /// 
        public double[,] Compute(UnmanagedImage source, Rectangle region)
        {
            if (source.PixelFormat != PixelFormat.Format8bppIndexed)
                throw new UnsupportedImageFormatException("Only grayscale 8bpp images are supported.");

            int width = region.Width;
            int height = region.Height;
            int stride = source.Stride;
            int offset = stride - width;
            int maxGray = 255;

            int startX = region.X;
            int startY = region.Y;

            unsafe
            {
                byte* src = (byte*)source.ImageData.ToPointer() + startY * stride + startX;

                if (autoGray)
                    maxGray = max(width, height, offset, src);

                numPairs = 0;
                int size = maxGray + 1;
                double[,] cooccurrence = new double[size, size];


                switch (degree)
                {
                    case CooccurrenceDegree.Degree0:
                        for (int y = startY; y < height; y++)
                        {
                            for (int x = startX + distance; x < width; x++)
                            {
                                byte a = src[stride * y + (x - distance)];
                                byte b = src[stride * y + x];
                                cooccurrence[a, b]++;
                                numPairs++;
                            }
                        }
                        break;

                    case CooccurrenceDegree.Degree45:
                        for (int y = startY + distance; y < height; y++)
                        {
                            for (int x = startX; x < width - distance; x++)
                            {
                                byte a = src[stride * y + x];
                                byte b = src[stride * (y - distance) + (x + distance)];
                                cooccurrence[a, b]++;
                                numPairs++;
                            }
                        }
                        break;

                    case CooccurrenceDegree.Degree90:
                        for (int y = startY + distance; y < height; y++)
                        {
                            for (int x = startX; x < width; x++)
                            {
                                byte a = src[stride * (y - distance) + x];
                                byte b = src[stride * y + x];
                                cooccurrence[a, b]++;
                                numPairs++;
                            }
                        }
                        break;

                    case CooccurrenceDegree.Degree135:
                        for (int y = startY + distance; y < height; y++)
                        {
                            int steps = width - 1;
                            for (int x = startX; x < width - distance; x++)
                            {
                                byte a = src[stride * y + (steps - x)];
                                byte b = src[stride * (y - distance) + (steps - distance - x)];
                                cooccurrence[a, b]++;
                                numPairs++;
                            }
                        }
                        break;
                }

                if (normalize && numPairs > 0)
                {
                    fixed (double* ptrMatrix = cooccurrence)
                    {
                        double* c = ptrMatrix;
                        for (int i = 0; i < cooccurrence.Length; i++, c++)
                            *c /= numPairs;
                    }
                }

                return cooccurrence;
            }
        }

        unsafe private static int max(int width, int height, int offset, byte* src)
        {
            int maxGray = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, src++)
                    if (*src > maxGray) maxGray = *src;
                src += offset;
            }

            return maxGray;
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public object Clone()
        {
            var clone = new GrayLevelCooccurrenceMatrix();
            clone.autoGray = autoGray;
            clone.degree = degree;
            clone.distance = distance;
            clone.normalize = normalize;
            clone.numPairs = numPairs;
            return clone;
        }

    }
}