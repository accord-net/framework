// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Diego Catalano, 2013
// diego.catalano at live.com
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
    using AForge.Imaging;
    using System.Drawing;

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
        Degree0,

        /// <summary>
        ///   Find co-occurrences at 45° degrees.
        /// </summary>
        /// 
        Degree45,

        /// <summary>
        ///   Find co-occurrences at 90° degrees.
        /// </summary>
        /// 
        Degree90,

        /// <summary>
        ///   Find co-occurrences at 135° degrees.
        /// </summary>
        /// 
        Degree135
    };


    /// <summary>
    ///   Gray-Level Co-occurrence Matrix (GLCM).
    /// </summary>
    /// 
    public class GrayLevelCooccurrenceMatrix
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
        ///   Gets or sets the direction at which the co-occurrence should be found.
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
        public GrayLevelCooccurrenceMatrix() { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GrayLevelCooccurrenceMatrix"/> class.
        /// </summary>
        /// 
        /// <param name="distance">The distance at which the texture should be analyzed.</param>
        /// 
        public GrayLevelCooccurrenceMatrix(int distance)
        {
            this.distance = distance;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GrayLevelCooccurrenceMatrix"/> class.
        /// </summary>
        /// 
        /// <param name="distance">The distance at which the texture should be analyzed.</param>
        /// <param name="degree">The direction to look for co-occurrences.</param>
        /// 
        public GrayLevelCooccurrenceMatrix(int distance, CooccurrenceDegree degree)
        {
            this.distance = distance;
            this.degree = degree;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GrayLevelCooccurrenceMatrix"/> class.
        /// </summary>
        /// 
        /// <param name="distance">The distance at which the texture should be analyzed.</param>
        /// <param name="degree">The direction to look for co-occurrences.</param>
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
        public unsafe double[,] Compute(UnmanagedImage source, Rectangle region)
        {
            int width = region.Width;
            int height = region.Height;
            int stride = source.Stride;
            int offset = stride - width;
            int maxGray = 255;

            int startX = region.X;
            int startY = region.Y;

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
    }
}