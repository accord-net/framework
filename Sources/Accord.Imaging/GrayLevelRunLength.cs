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

    /// <summary>
    ///   Gray-Level Run-Length Matrix.
    /// </summary>
    /// 
    public class GrayLevelRunLengthMatrix
    {

        private CooccurrenceDegree degree;

        private int numPrimitives;
        private bool autoGray = true;

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
        ///   Gets or sets the direction at which the co-occurrence should be found.
        /// </summary>
        /// 
        public CooccurrenceDegree Degree
        {
            get { return degree; }
            set { degree = value; }
        }

        /// <summary>
        ///   Gets the number of primitives found in the last 
        ///   call to <see cref="Compute(UnmanagedImage)"/>.
        /// </summary>
        /// 
        public int Primitives
        {
            get { return numPrimitives; }
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="GrayLevelDifferenceMethod"/> class.
        /// </summary>
        /// 
        /// <param name="degree">The direction at which the co-occurrence should be found.</param>
        /// 
        public GrayLevelRunLengthMatrix(CooccurrenceDegree degree)
        {
            this.degree = degree;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GrayLevelDifferenceMethod"/> class.
        /// </summary>
        /// 
        /// <param name="degree">The direction at which the co-occurrence should be found.</param>
        /// <param name="autoGray">Whether the maximum value of gray should be
        ///   automatically computed from the image. Default is true.</param>
        /// 
        public GrayLevelRunLengthMatrix(CooccurrenceDegree degree, bool autoGray)
        {
            this.degree = degree;
            this.autoGray = autoGray;
        }

        /// <summary>
        ///   Computes the Gray-level Run-length for the given image source.
        /// </summary>
        /// 
        /// <param name="source">The source image.</param>
        /// 
        /// <returns>An array of run-length vectors containing level counts
        ///   for every width pixel in <paramref name="source"/>.</returns>
        /// 
        public unsafe double[][] Compute(UnmanagedImage source)
        {
            int width = source.Width;
            int height = source.Height;
            int stride = source.Stride;
            int offset = stride - width;
            int maxGray = 255;

            byte* src = (byte*)source.ImageData.ToPointer();

            if (autoGray)
                maxGray = max(width, height, offset, src);

            numPrimitives = 0;
            double[][] runMatrix = new double[maxGray + 1][];
            for (int i = 0; i < runMatrix.Length; i++)
                runMatrix[i] = new double[width + 1];


            switch (degree)
            {
                case CooccurrenceDegree.Degree0:
                    for (int y = 0; y < height; y++)
                    {
                        int runs = 1;
                        for (int x = 1; x < width; x++)
                        {
                            byte a = src[stride * y + (x - 1)];
                            byte b = src[stride * y + x];

                            if (a == b)
                                runs++;

                            else
                            {
                                runMatrix[a][runs]++;
                                numPrimitives++;
                                runs = 1;
                            }

                            if ((a == b) && (x == width - 1)) runMatrix[a][runs]++;
                            if ((a != b) && (x == width - 1)) runMatrix[b][1]++;
                        }
                    }
                    break;

                case CooccurrenceDegree.Degree45:

                    // Compute I(0,0) and I(height,width)
                    runMatrix[0][1]++;
                    runMatrix[height - 1][width - 1]++;

                    // Compute height
                    for (int i = 1; i < height; i++)
                    {
                        int runs = 1;
                        int steps = i;
                        for (int j = 0; j < steps; j++)
                        {
                            byte a = src[stride * (i - j) + j];
                            byte b = src[stride * (i - j - 1) + j + 1];

                            if (a == b)
                                runs++;

                            else
                            {
                                runMatrix[a][runs]++;
                                numPrimitives++;
                                runs = 1;
                            }

                            if ((a == b) && (j == steps - 1)) runMatrix[a][runs]++;
                            if ((a != b) && (j == steps - 1)) runMatrix[b][1]++;
                        }
                    }

                    // Compute width
                    for (int j = 1; j < width - 1; j++)
                    {
                        int runs = 1;
                        int steps = height - j;
                        for (int i = 1; i < steps; i++)
                        {
                            byte a = src[stride * (height - i) + j + i - 1];
                            byte b = src[stride * (height - i - 1) + j + i];

                            if (a == b)
                                runs++;

                            else
                            {
                                runMatrix[a][runs]++;
                                numPrimitives++;
                                runs = 1;
                            }

                            if ((a == b) && (i == steps - 1)) runMatrix[a][runs]++;
                            if ((a != b) && (i == steps - 1)) runMatrix[b][1]++;

                        }
                    }
                    break;

                case CooccurrenceDegree.Degree90:
                    for (int j = 0; j < width; j++)
                    {
                        int runs = 1;
                        for (int i = 0; i < height - 1; i++)
                        {
                            byte a = src[stride * (height - i - 1) + j];
                            byte b = src[stride * (height - i - 2) + j];

                            if (a == b)
                                runs++;

                            else
                            {
                                runMatrix[a][runs]++;
                                numPrimitives++;
                                runs = 1;
                            }

                            if ((a == b) && (i == height - 2)) runMatrix[a][runs]++;
                            if ((a != b) && (i == height - 2)) runMatrix[b][1]++;
                        }
                    }
                    break;

                case CooccurrenceDegree.Degree135:

                    // Compute I(0,width) and I(height,0)
                    runMatrix[0][width - 1]++;
                    runMatrix[height - 1][0]++;

                    // Compute height
                    for (int i = 1; i < width; i++)
                    {
                        int runs = 1;
                        int steps = i;
                        int w = width - 1;
                        for (int j = 0; j < steps; j++)
                        {
                            byte a = src[stride * (i - j) + (w)];
                            byte b = src[stride * (i - j - 1) + (--w)];

                            if (a == b)
                                runs++;

                            else
                            {
                                runMatrix[a][runs]++;
                                numPrimitives++;
                                runs = 1;
                            }

                            if ((a == b) && (j == steps - 1)) runMatrix[a][runs]++;
                            if ((a != b) && (j == steps - 1)) runMatrix[b][1]++;
                        }
                    }

                    // Compute width
                    for (int j = 1; j < width - 1; j++)
                    {
                        int runs = 1;
                        int steps = height - j;
                        int w = width - 1 - j;
                        for (int i = 1; i < steps; i++)
                        {
                            byte a = src[stride * (height - i) + (w)];
                            byte b = src[stride * (height - i - 1) + (--w)];

                            if (a == b)
                                runs++;

                            else
                            {
                                runMatrix[a][runs]++;
                                numPrimitives++;
                                runs = 1;
                            }

                            if ((a == b) && (i == steps - 1)) runMatrix[a][runs]++;
                            if ((a != b) && (i == steps - 1)) runMatrix[b][1]++;
                        }
                    }
                    break;
            }

            return runMatrix;
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