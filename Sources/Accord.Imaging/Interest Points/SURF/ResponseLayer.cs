// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Christopher Evans, 2009-2011
// http://www.chrisevansdev.com/
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
    using System.Collections;
    using System.Collections.Generic;
    using AForge.Imaging;

    /// <summary>
    ///   Response filter.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In SURF, the scale-space is divided into a number of octaves,
    ///   where an octave refers to a series of <see cref="ResponseLayer"/>
    ///   response maps covering a doubling of scale.</para>
    /// <para>
    ///   In the traditional approach to constructing a scale-space,
    ///   the image size is varied and the Gaussian filter is repeatedly
    ///   applied to smooth subsequent layers. The SURF approach leaves
    ///   the original image unchanged and varies only the filter size.</para>
    /// </remarks>
    /// 
    internal class ResponseLayerCollection : IEnumerable<ResponseLayer[]>
    {
        private int width;
        private int height;
        private int step;
        private int octaves;

        private static readonly int[,] map = 
        {
            { 0,  1,  2,  3 },
            { 1,  3,  4,  5 },
            { 3,  5,  6,  7 },
            { 5,  7,  8,  9 },
            { 7,  9, 10, 11 }
        };

        private ResponseLayer[] responses;


        /// <summary>
        ///   Creates the initial map of responses according to
        ///   the specified number of octaves and initial step.
        /// </summary>
        /// 
        public ResponseLayerCollection(int width, int height, int octaves, int initial)
        {
            this.width = width;
            this.height = height;
            this.step = initial;
            this.octaves = octaves;

            this.initialize();
        }

        /// <summary>
        ///   Updates the response filter definitions
        ///   without recreating objects.
        /// </summary>
        /// 
        public void Update(int width, int height, int initial)
        {
            this.width = width;
            this.height = height;
            this.step = initial;

            this.update();
        }

        /// <summary>
        ///   Computes the filter using the specified <see cref="IntegralImage">
        ///   Integral Image.</see>
        /// </summary>
        /// 
        /// <param name="integral">The integral image.</param>
        /// 
        public void Compute(IntegralImage integral)
        {
            for (int i = 0; i < responses.Length; ++i)
                responses[i].Compute(integral);
        }

        private void initialize()
        {
            List<ResponseLayer> list = new List<ResponseLayer>();

            // Get image attributes
            int w = width / step;
            int h = height / step;
            int s = step;

            if (octaves >= 1)
            {
                list.Add(new ResponseLayer(w, h, s, 9));
                list.Add(new ResponseLayer(w, h, s, 15));
                list.Add(new ResponseLayer(w, h, s, 21));
                list.Add(new ResponseLayer(w, h, s, 27));
            }

            if (octaves >= 2)
            {
                list.Add(new ResponseLayer(w / 2, h / 2, s * 2, 39));
                list.Add(new ResponseLayer(w / 2, h / 2, s * 2, 51));
            }

            if (octaves >= 3)
            {
                list.Add(new ResponseLayer(w / 4, h / 4, s * 4, 75));
                list.Add(new ResponseLayer(w / 4, h / 4, s * 4, 99));
            }

            if (octaves >= 4)
            {
                list.Add(new ResponseLayer(w / 8, h / 8, s * 8, 147));
                list.Add(new ResponseLayer(w / 8, h / 8, s * 8, 195));
            }

            if (octaves >= 5)
            {
                list.Add(new ResponseLayer(w / 16, h / 16, s * 16, 291));
                list.Add(new ResponseLayer(w / 16, h / 16, s * 16, 387));
            }

            this.responses = list.ToArray();
        }

        private void update()
        {
            // Get image attributes
            int w = width / step;
            int h = height / step;
            int s = step;

            int i = 0;
            if (octaves >= 1)
            {
                responses[i++].Update(w, h, s, 9);
                responses[i++].Update(w, h, s, 15);
                responses[i++].Update(w, h, s, 21);
                responses[i++].Update(w, h, s, 27);
            }

            if (octaves >= 2)
            {
                responses[i++].Update(w / 2, h / 2, s * 2, 39);
                responses[i++].Update(w / 2, h / 2, s * 2, 51);
            }

            if (octaves >= 3)
            {
                responses[i++].Update(w / 4, h / 4, s * 4, 75);
                responses[i++].Update(w / 4, h / 4, s * 4, 99);
            }

            if (octaves >= 4)
            {
                responses[i++].Update(w / 8, h / 8, s * 8, 147);
                responses[i++].Update(w / 8, h / 8, s * 8, 195);
            }

            if (octaves >= 5)
            {
                responses[i++].Update(w / 16, h / 16, s * 16, 291);
                responses[i++].Update(w / 16, h / 16, s * 16, 387);
            }
        }


        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// 
        public IEnumerator<ResponseLayer[]> GetEnumerator()
        {
            ResponseLayer[] pyramid = new ResponseLayer[3];

            for (int i = 0; i < octaves; i++)
            {
                // for each set of response layers
                for (int j = 0; j <= 1; j++)
                {
                    // Grab the three layers forming the pyramid
                    pyramid[0] = responses[map[i, j + 0]];
                    pyramid[1] = responses[map[i, j + 1]];
                    pyramid[2] = responses[map[i, j + 2]];
                    yield return pyramid;
                }
            }
        }

        /// <summary>
        ///   Returns an enumerator that iterates through this collection.
        /// </summary>
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// 
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }

    }

    /// <summary>
    ///   Response Layer.
    /// </summary>
    /// 
    [Serializable]
    internal class ResponseLayer
    {
        /// <summary>
        ///   Gets the width of the filter.
        /// </summary>
        /// 
        public int Width { get; private set; }

        /// <summary>
        ///   Gets the height of the filter.
        /// </summary>
        /// 
        public int Height { get; private set; }

        /// <summary>
        ///   Gets the filter step.
        /// </summary>
        /// 
        public int Step { get; private set; }

        /// <summary>
        ///   Gets the filter size.
        /// </summary>
        /// 
        public int Size { get; private set; }

        /// <summary>
        ///   Gets the responses computed from the filter.
        /// </summary>
        /// 
        public float[,] Responses { get; private set; }

        /// <summary>
        ///   Gets the Laplacian computed from the filter.
        /// </summary>
        /// 
        public int[,] Laplacian { get; private set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="ResponseLayer"/> class.
        /// </summary>
        /// 
        public ResponseLayer(int width, int height, int step, int filter)
        {
            this.Width = width;
            this.Height = height;
            this.Step = step;
            this.Size = filter;

            this.Responses = new float[height, width];
            this.Laplacian = new int[height, width];
        }

        /// <summary>
        ///   Updates the response layer definitions
        ///   without recreating objects.
        /// </summary>
        /// 
        public void Update(int width, int height, int step, int filter)
        {
            if (height > Height || width > Width)
            {
                this.Responses = new float[height, width];
                this.Laplacian = new int[height, width];
            }

            this.Width = width;
            this.Height = height;
            this.Step = step;
            this.Size = filter;
        }

        /// <summary>
        ///   Computes the filter for the specified integral image.
        /// </summary>
        /// 
        /// <param name="image">The integral image.</param>
        /// 
        public void Compute(IntegralImage image)
        {
            int b = (Size - 1) / 2 + 1;
            int c = Size / 3;
            int w = Size;
            float inv = 1f / (w * w);
            float Dxx, Dyy, Dxy;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    // Get the image coordinates
                    int i = y * Step;
                    int j = x * Step;

                    // Compute response components
                    Dxx = ((int)image.GetRectangleSum(j - b, i - c + 1, j - b + w - 1, i - c + 2 * c - 1)
                         - (int)image.GetRectangleSum(j - c / 2, i - c + 1, j - c / 2 + c - 1, i - c + 2 * c - 1) * 3);

                    Dyy = ((int)image.GetRectangleSum(j - c + 1, i - b, j - c + 2 * c - 1, i - b + w - 1)
                         - (int)image.GetRectangleSum(j - c + 1, i - c / 2, j - c + 2 * c - 1, i - c / 2 + c - 1) * 3);

                    Dxy = ((int)image.GetRectangleSum(j + 1, i - c, j + c, i - 1)
                         + (int)image.GetRectangleSum(j - c, i + 1, j - 1, i + c)
                         - (int)image.GetRectangleSum(j - c, i - c, j - 1, i - 1)
                         - (int)image.GetRectangleSum(j + 1, i + 1, j + c, i + c));

                    // Normalize the filter responses with respect to their size
                    Dxx *= inv / 255f;
                    Dyy *= inv / 255f;
                    Dxy *= inv / 255f;

                    // Get the determinant of Hessian response & laplacian sign
                    Responses[y, x] = (Dxx * Dyy) - (0.9f * 0.9f * Dxy * Dxy);
                    Laplacian[y, x] = (Dxx + Dyy) >= 0 ? 1 : 0;
                }
            }
        }

    }
}
