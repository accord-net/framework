// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
//
// Copyright © Diego Catalano, 2013
// diego.catalano at live.com
//
// Based on the implementation by
// Copyright © Max Bügler, 2010-2013
// http://www.maxbuegler.eu/
//
// Licensed under the LGPL with permission of the original author.
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
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using Accord.Math;
    using AForge.Imaging;
    using AForge.Imaging.Filters;

    /// <summary>
    ///   Gabor filter.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In image processing, a Gabor filter, named after Dennis Gabor, is a linear 
    ///   filter used for edge detection. Frequency and orientation representations 
    ///   of Gabor filters are similar to those of the human visual system, and they
    ///   have been found to be particularly appropriate for texture representation 
    ///   and discrimination. In the spatial domain, a 2D Gabor filter is a Gaussian
    ///   kernel function modulated by a sinusoidal plane wave. The Gabor filters are
    ///   self-similar: all filters can be generated from one mother wavelet by dilation
    ///   and rotation. </para>
    /// </remarks>
    /// 
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia Contributors, "Gabor filter". Available at
    ///       http://en.wikipedia.org/wiki/Gabor_filter </description></item>
    ///   </list>
    /// </para>
    /// 
    /// <example>
    /// <para>
    ///   The following example applies a Gabor filter to detect lines
    ///   at a 45 degrees from the following image: </para>
    ///   
    /// <img src="..\images\lines.png" /> 
    /// 
    /// <code>
    ///   Bitmap input = ...;
    ///   
    ///   // Create a new Gabor filter
    ///   GaborFilter filter = new GaborFilter();
    ///   
    ///   // Apply the filter
    ///   Bitmap output = filter.Apply(input);
    ///   
    ///   // Show the output
    ///   ImageBox.Show(output);
    /// </code>
    /// 
    /// <para>
    ///   The resulting image is shown below. </para>
    ///   
    /// <img src="..\images\lines-gabor.png" /> 
    /// 
    /// </example>
    public class GaborFilter : AForge.Imaging.Filters.BaseFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations
            = new Dictionary<PixelFormat, PixelFormat>();

        private double[,] kernel;

        private int size = 3;        // kernel size
        private double lambda = 4.0; // wavelength
        private double theta = 0.6;  // orientation
        private double psi = 1.0;    // phase offset
        private double sigma = 2.0;  // Gaussian variance
        private double gamma = 0.3;  // aspect ratio

        bool recompute = true;

        /// <summary>
        ///   Gets or sets the size of the filter. Default is 3.
        /// </summary>
        /// 
        public int Size
        {
            get { return size; }
            set { size = value; recompute = true; }
        }

        /// <summary>
        ///   Gets or sets the Gaussian variance for the filter. Default is 2.
        /// </summary>
        /// 
        public double Sigma
        {
            get { return sigma; }
            set { sigma = value; recompute = true; }
        }

        /// <summary>
        ///   Gets or sets the orientation for the filter, in radians. Default is 0.6.
        /// </summary>
        /// 
        public double Theta
        {
            get { return theta; }
            set { theta = value; recompute = true; }
        }

        /// <summary>
        ///   Gets or sets the wavelength for the filter. Default is 4.0.
        /// </summary>
        /// 
        public double Lambda
        {
            get { return lambda; }
            set { lambda = value; recompute = true; }
        }

        /// <summary>
        ///   Gets or sets the aspect ratio for the filter. Default is 0.3.
        /// </summary>
        /// 
        public double Gamma
        {
            get { return gamma; }
            set { gamma = value; recompute = true; }
        }

        /// <summary>
        ///   Gets or sets the phase offset for the filter. Default is 1.0.
        /// </summary>
        /// 
        public double Psi
        {
            get { return psi; }
            set { psi = value; recompute = true; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GaborFilter"/> class.
        /// </summary>
        /// 
        public GaborFilter()
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            recompute = true;
        }


        /// <summary>
        ///   Format translations dictionary.
        /// </summary>
        /// 
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        ///   Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// 
        protected unsafe override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
        {
            // check image format
            if ((sourceData.PixelFormat != PixelFormat.Format8bppIndexed) &&
                (sourceData.PixelFormat != PixelFormat.Format24bppRgb))
                throw new UnsupportedImageFormatException("Unsupported image format.");

            if (sourceData.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                sourceData = Grayscale.CommonAlgorithms.BT709.Apply(sourceData);
            }

            if (recompute)
            {
                recompute = false;
                kernel = Gabor.Kernel2D(size: size,
                                        lambda: lambda,
                                        theta: theta,
                                        psi: psi,
                                        sigma: sigma,
                                        gamma: gamma,
                                        normalized: true,
                                        function: GaborKernelKind.Imaginary);
            }

            int kernelHeight = kernel.GetLength(0);
            int kernelWidth = kernel.GetLength(1);

            int centerX = kernelHeight / 2;
            int centerY = kernelWidth / 2;

            int width = sourceData.Width;
            int height = sourceData.Height;

            int srcStride = sourceData.Stride;
            int srcOffset = srcStride - width;

            byte* src = (byte*)sourceData.ImageData.ToPointer();


            int[,] response = new int[height, width];

            int max = int.MinValue;
            int min = int.MaxValue;

            // for each image row
            for (int y = 0; y < height; y++)
            {
                // for each pixel in the row
                for (int x = 0; x < width; x++, src++)
                {
                    double sum = 0;

                    // for each kernel row
                    for (int i = 0; i < kernelHeight; i++)
                    {
                        int ir = i - centerY;
                        int t = y + ir;

                        // skip row
                        if (t < 0)
                            continue;

                        // break
                        if (t >= height)
                            break;

                        int col = ir * srcStride;

                        // for each kernel value in the row
                        for (int j = 0; j < kernelWidth; j++)
                        {
                            int jr = j - centerX;
                            t = x + jr;

                            // skip column
                            if (t < 0)
                                continue;

                            if (t < width)
                            {
                                double k = kernel[i, j];
                                sum += k * src[col + jr];
                            }
                        }

                        int v = response[y, x] = (int)sum;

                        if (v > max) max = v;
                        if (v < min) min = v;
                    }
                }

                src += srcOffset;
            }


            byte* dst = (byte*)destinationData.ImageData.ToPointer();
            int pixelSize = System.Drawing.Image.GetPixelFormatSize(destinationData.PixelFormat) / 8;
            int dstStride = destinationData.Stride;
            int dstOffset = dstStride - width * pixelSize;

            if (destinationData.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                // for each image row
                for (int y = 0; y < height; y++)
                {
                    // for each pixel in the row
                    for (int x = 0; x < width; x++, dst++)
                    {
                        *dst = (byte)((255 * (response[y, x] - min)) / (max - min));
                    }

                    dst += dstOffset;
                }
            }
            else
            {
                // for each image row
                for (int y = 0; y < height; y++)
                {
                    // for each pixel in the row
                    for (int x = 0; x < width; x++, dst += pixelSize)
                    {
                        int v = response[y, x];

                        if (v > 0)
                        {
                            dst[RGB.R] = (byte)((255 * v) / max);
                        }
                        else // (v <= 0)
                        {
                            dst[RGB.B] = (byte)((255 * v) / min);
                        }
                    }

                    dst += dstOffset;
                }
            }
        }

    }
}