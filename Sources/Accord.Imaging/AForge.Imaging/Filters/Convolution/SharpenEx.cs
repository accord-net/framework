// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//
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
// Original idea of the SharpenEx found in Paint.NET project
// http://www.eecs.wsu.edu/paint.net/
//

namespace Accord.Imaging.Filters
{
    using Accord.Math;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Gaussian sharpen filter.
    /// </summary>
    /// 
    /// <remarks><para>The filter performs <see cref="Convolution">convolution filter</see> using
    /// the kernel, which is calculate with the help of <see cref="AForge.Math.Gaussian.Kernel2D"/>
    /// method and then converted to integer sharpening kernel. First of all the integer kernel
    /// is calculated from <see cref="AForge.Math.Gaussian.Kernel2D"/> by dividing all elements by
    /// the element with the smallest value. Then the integer kernel is converted to sharpen kernel by
    /// negating all kernel's elements (multiplying with <b>-1</b>), but the central kernel's element
    /// is calculated as <b>2 * sum - centralElement</b>, where <b>sum</b> is the sum off elements
    /// in the integer kernel before negating.</para>
    /// 
    /// <para>For the list of supported pixel formats, see the documentation to <see cref="Convolution"/>
    /// filter.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter with kernel size equal to 11
    /// // and Gaussia sigma value equal to 4.0
    /// GaussianSharpen filter = new GaussianSharpen( 4, 11 );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    ///
    /// <para><b>Initial image:</b></para>
    /// <img src="..\images\imaging\sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="..\images\imaging\gaussian_sharpen.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    /// <seealso cref="Convolution"/>
    ///
    public class GaussianSharpen : Convolution
    {
        private double sigma = 1.4;
        private int size = 5;

        /// <summary>
        /// Gaussian sigma value, [0.5, 5.0].
        /// </summary>
        /// 
        /// <remarks><para>Sigma value for Gaussian function used to calculate
        /// the kernel.</para>
        /// 
        /// <para>Default value is set to <b>1.4</b>.</para>
        /// </remarks>
        /// 
        public double Sigma
        {
            get { return sigma; }
            set
            {
                // get new sigma value
                sigma = Math.Max(0.5, Math.Min(5.0, value));
                // create filter
                CreateFilter();
            }
        }

        /// <summary>
        /// Kernel size, [3, 5].
        /// </summary>
        /// 
        /// <remarks><para>Size of Gaussian kernel.</para>
        /// 
        /// <para>Default value is set to <b>5</b>.</para>
        /// </remarks>
        ///
        public int Size
        {
            get { return size; }
            set
            {
                size = Math.Max(3, Math.Min(21, value | 1));
                CreateFilter();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GaussianSharpen"/> class.
        /// </summary>
        /// 
        public GaussianSharpen()
        {
            CreateFilter();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GaussianSharpen"/> class.
        /// </summary>
        /// 
        /// <param name="sigma">Gaussian sigma value.</param>
        /// 
        public GaussianSharpen(double sigma)
        {
            Sigma = sigma;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GaussianSharpen"/> class.
        /// </summary>
        /// 
        /// <param name="sigma">Gaussian sigma value.</param>
        /// <param name="size">Kernel size.</param>
        /// 
        public GaussianSharpen(double sigma, int size)
        {
            Sigma = sigma;
            Size = size;
        }



        // Create Gaussian filter
        private void CreateFilter()
        {
            // create Gaussian kernel
            double[,] kernel = Normal.Kernel2D(sigma * sigma, size);
            double min = kernel[0, 0];

            // integer kernel
            int[,] intKernel = new int[size, size];
            int sum = 0;
            int divisor = 0;

            // calculate integer kernel
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    double v = kernel[i, j] / min;

                    if (v > ushort.MaxValue)
                        v = ushort.MaxValue;

                    intKernel[i, j] = (int)v;

                    // collect sum
                    sum += intKernel[i, j];
                }
            }

            // recalculate kernel
            int c = size >> 1;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if ((i == c) && (j == c))
                    {
                        // calculate central value
                        intKernel[i, j] = 2 * sum - intKernel[i, j];
                    }
                    else
                    {
                        // invert value
                        intKernel[i, j] = -intKernel[i, j];
                    }

                    // collect divisor
                    divisor += intKernel[i, j];
                }
            }

            // update filter
            this.Kernel = intKernel;
            this.Divisor = divisor;
        }
    }
}
