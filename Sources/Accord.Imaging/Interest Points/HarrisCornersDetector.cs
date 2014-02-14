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
    using AForge;
    using AForge.Imaging;
    using AForge.Imaging.Filters;

    /// <summary>
    ///   Corners measures to be used in <see cref="HarrisCornersDetector"/>.
    /// </summary>
    /// 
    public enum HarrisCornerMeasure
    {
        /// <summary>
        ///   Original Harris' measure. Requires the setting of
        ///   a parameter k (default is 0.04), which may be a
        ///   bit arbitrary and introduce more parameters to tune.
        /// </summary>
        /// 
        Harris,

        /// <summary>
        ///   Noble's measure. Does not require a parameter
        ///   and may be more stable.
        /// </summary>
        /// 
        Noble,
    }

    /// <summary>
    ///   Harris Corners Detector.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>This class implements the Harris corners detector.</para>
    /// <para>Sample usage:</para>
    /// 
    /// <code>
    /// // create corners detector's instance
    /// HarrisCornersDetector hcd = new HarrisCornersDetector( );
    /// // process image searching for corners
    /// Point[] corners = hcd.ProcessImage( image );
    /// // process points
    /// foreach ( Point corner in corners )
    /// {
    ///     // ... 
    /// }
    /// </code>
    /// 
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       P. D. Kovesi. MATLAB and Octave Functions for Computer Vision and Image Processing.
    ///       School of Computer Science and Software Engineering, The University of Western Australia.
    ///       Available in: http://www.csse.uwa.edu.au/~pk/Research/MatlabFns/Spatial/harris.m </description></item>
    ///     <item><description>
    ///       C.G. Harris and M.J. Stephens. "A combined corner and edge detector", 
    ///       Proceedings Fourth Alvey Vision Conference, Manchester.
    ///       pp 147-151, 1988.</description></item>
    ///     <item><description>
    ///       Alison Noble, "Descriptions of Image Surfaces", PhD thesis, Department
    ///       of Engineering Science, Oxford University 1989, p45.</description></item>
    ///   </list>
    /// </para>
    /// </remarks>
    /// 
    /// <seealso cref="MoravecCornersDetector"/>
    /// <seealso cref="SusanCornersDetector"/>
    ///
    public class HarrisCornersDetector : ICornersDetector
    {
        // Harris parameters
        private HarrisCornerMeasure measure = HarrisCornerMeasure.Harris;
        private float k = 0.04f;
        private float threshold = 20000f;

        // Non-maximum suppression parameters
        private int r = 3;

        // Gaussian smoothing parameters
        private double sigma = 1.2;
        private float[] kernel;
        private int size = 7;


        /// <summary>
        ///   Gets or sets the measure to use when detecting corners.
        /// </summary>
        /// 
        public HarrisCornerMeasure Measure
        {
            get { return measure; }
            set { measure = value; }
        }

        /// <summary>
        ///   Harris parameter k. Default value is 0.04.
        /// </summary>
        /// 
        public float K
        {
            get { return k; }
            set { k = value; }
        }

        /// <summary>
        ///   Harris threshold. Default value is 20000.
        /// </summary>
        /// 
        public float Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }

        /// <summary>
        ///   Gaussian smoothing sigma. Default value is 1.2.
        /// </summary>
        /// 
        public double Sigma
        {
            get { return sigma; }
            set
            {
                if (sigma != value)
                {
                    sigma = value;
                    createGaussian();
                }
            }
        }

        /// <summary>
        ///   Non-maximum suppression window radius. Default value is 3.
        /// </summary>
        /// 
        public int Suppression
        {
            get { return r; }
            set { r = value; }
        }


        #region Constructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="HarrisCornersDetector"/> class.
        /// </summary>
        public HarrisCornersDetector()
        {
            initialize(HarrisCornerMeasure.Harris, k, threshold, sigma, r, size);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="HarrisCornersDetector"/> class.
        /// </summary>
        public HarrisCornersDetector(float k)
        {
            initialize(HarrisCornerMeasure.Harris, k, threshold, sigma, r, size);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="HarrisCornersDetector"/> class.
        /// </summary>
        public HarrisCornersDetector(float k, float threshold)
        {
            initialize(HarrisCornerMeasure.Harris, k, threshold, sigma, r, size);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="HarrisCornersDetector"/> class.
        /// </summary>
        public HarrisCornersDetector(float k, float threshold, double sigma)
        {
            initialize(HarrisCornerMeasure.Harris, k, threshold, sigma, r, size);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="HarrisCornersDetector"/> class.
        /// </summary>
        public HarrisCornersDetector(float k, float threshold, double sigma, int suppression)
        {
            initialize(HarrisCornerMeasure.Harris, k, threshold, sigma, suppression, size);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="HarrisCornersDetector"/> class.
        /// </summary>
        public HarrisCornersDetector(HarrisCornerMeasure measure, float threshold, double sigma, int suppression)
        {
            initialize(measure, k, threshold, sigma, suppression, size);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="HarrisCornersDetector"/> class.
        /// </summary>
        public HarrisCornersDetector(HarrisCornerMeasure measure, float threshold, double sigma)
        {
            initialize(measure, k, threshold, sigma, r, size);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="HarrisCornersDetector"/> class.
        /// </summary>
        public HarrisCornersDetector(HarrisCornerMeasure measure, float threshold)
        {
            initialize(measure, k, threshold, sigma, r, size);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="HarrisCornersDetector"/> class.
        /// </summary>
        public HarrisCornersDetector(HarrisCornerMeasure measure)
        {
            initialize(measure, k, threshold, sigma, r, size);
        }

        private void initialize(HarrisCornerMeasure measure, float k,
            float threshold, double sigma, int suppression, int size)
        {
            this.measure = measure;
            this.threshold = threshold;
            this.k = k;
            this.r = suppression;
            this.sigma = sigma;
            this.size = size;

            createGaussian();
        }

        private void createGaussian()
        {
            double[] aforgeKernel = new AForge.Math.Gaussian(sigma).Kernel(size);
            this.kernel = Array.ConvertAll<double, float>(aforgeKernel, Convert.ToSingle);
        }
        #endregion


        /// <summary>
        ///   Process image looking for corners.
        /// </summary>
        /// 
        /// <param name="image">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found corners (X-Y coordinates).</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">
        ///   The source image has incorrect pixel format.
        /// </exception>
        /// 
        public unsafe List<IntPoint> ProcessImage(UnmanagedImage image)
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


            // 1. Calculate partial differences
            float[,] diffx = new float[height, width];
            float[,] diffy = new float[height, width];
            float[,] diffxy = new float[height, width];


            fixed (float* pdx = diffx, pdy = diffy, pdxy = diffxy)
            {
                // Begin skipping first line
                byte* src = (byte*)grayImage.ImageData.ToPointer() + stride;
                float* dx = pdx + width;
                float* dy = pdy + width;
                float* dxy = pdxy + width;

                // for each line
                for (int y = 1; y < height - 1; y++)
                {
                    // skip first column
                    dx++; dy++; dxy++; src++;

                    // for each inner pixel in line (skipping first and last)
                    for (int x = 1; x < width - 1; x++, src++, dx++, dy++, dxy++)
                    {
                        // Retrieve the pixel neighborhood
                        byte a11 = src[+stride + 1], a12 = src[+1], a13 = src[-stride + 1];
                        byte a21 = src[+stride + 0], /*  a22    */  a23 = src[-stride + 0];
                        byte a31 = src[+stride - 1], a32 = src[-1], a33 = src[-stride - 1];

                        // Convolution with horizontal differentiation kernel mask
                        float h = ((a11 + a12 + a13) - (a31 + a32 + a33)) * 0.166666667f;

                        // Convolution with vertical differentiation kernel mask
                        float v = ((a11 + a21 + a31) - (a13 + a23 + a33)) * 0.166666667f;

                        // Store squared differences directly
                        *dx = h * h;
                        *dy = v * v;
                        *dxy = h * v;
                    }

                    // Skip last column
                    dx++; dy++; dxy++; 
                    src += offset + 1;
                }

                // Free some resources which wont be needed anymore
                if (image.PixelFormat != PixelFormat.Format8bppIndexed)
                    grayImage.Dispose();
            }


            // 2. Smooth the diff images
            if (sigma > 0.0)
            {
                float[,] temp = new float[height, width];

                // Convolve with Gaussian kernel
                convolve(diffx, temp, kernel);
                convolve(diffy, temp, kernel);
                convolve(diffxy, temp, kernel);
            }


            // 3. Compute Harris Corner Response Map
            float[,] map = new float[height, width];

            fixed (float* pdx = diffx, pdy = diffy, pdxy = diffxy, pmap = map)
            {
                float* dx = pdx;
                float* dy = pdy;
                float* dxy = pdxy;
                float* H = pmap;
                float M, A, B, C;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, dx++, dy++, dxy++, H++)
                    {
                        A = *dx;
                        B = *dy;
                        C = *dxy;

                        if (measure == HarrisCornerMeasure.Harris)
                        {
                            // Original Harris corner measure
                            M = (A * B - C * C) - (k * ((A + B) * (A + B)));
                        }
                        else
                        {
                            // Harris-Noble corner measure
                            M = (A * B - C * C) / (A + B + Constants.SingleEpsilon);
                        }

                        if (M > threshold)
                        {
                            *H = M; // insert value in the map
                        }
                    }
                }
            }


            // 4. Suppress non-maximum points
            List<IntPoint> cornersList = new List<IntPoint>();

            // for each row
            for (int y = r, maxY = height - r; y < maxY; y++)
            {
                // for each pixel
                for (int x = r, maxX = width - r; x < maxX; x++)
                {
                    float currentValue = map[y, x];

                    // for each windows' row
                    for (int i = -r; (currentValue != 0) && (i <= r); i++)
                    {
                        // for each windows' pixel
                        for (int j = -r; j <= r; j++)
                        {
                            if (map[y + i, x + j] > currentValue)
                            {
                                currentValue = 0;
                                break;
                            }
                        }
                    }

                    // check if this point is really interesting
                    if (currentValue != 0)
                    {
                        cornersList.Add(new IntPoint(x, y));
                    }
                }
            }


            return cornersList;
        }

        /// <summary>
        ///   Convolution with decomposed 1D kernel.
        /// </summary>
        /// 
        private static void convolve(float[,] image, float[,] temp, float[] kernel)
        {
            int width = image.GetLength(1);
            int height = image.GetLength(0);
            int radius = kernel.Length / 2;

            unsafe
            {
                fixed (float* ptrImage = image, ptrTemp = temp)
                {
                    float* src = ptrImage + radius;
                    float* tmp = ptrTemp + radius;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = radius; x < width - radius; x++, src++, tmp++)
                        {
                            float v = 0;
                            for (int k = 0; k < kernel.Length; k++)
                                v += src[k - radius] * kernel[k];
                            *tmp = v;
                        }
                        src += 2 * radius;
                        tmp += 2 * radius;
                    }


                    for (int x = 0; x < width; x++)
                    {
                        for (int y = radius; y < height - radius; y++)
                        {
                            src = ptrImage + y * width + x;
                            tmp = ptrTemp + y * width + x;

                            float v = 0;
                            for (int k = 0; k < kernel.Length; k++)
                                v += tmp[width * (k - radius)] * kernel[k];
                            *src = v;
                        }
                    }
                }
            }
        }


        /// <summary>
        ///   Process image looking for corners.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found corners (X-Y coordinates).</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">
        ///   The source image has incorrect pixel format.
        /// </exception>
        /// 
        public List<IntPoint> ProcessImage(BitmapData imageData)
        {
            return ProcessImage(new UnmanagedImage(imageData));
        }

        /// <summary>
        ///   Process image looking for corners.
        /// </summary>
        /// 
        /// <param name="image">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found corners (X-Y coordinates).</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">
        ///   The source image has incorrect pixel format.
        /// </exception>
        /// 
        public List<IntPoint> ProcessImage(Bitmap image)
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

            List<IntPoint> corners;

            try
            {
                // process the image
                corners = ProcessImage(new UnmanagedImage(imageData));
            }
            finally
            {
                // unlock image
                image.UnlockBits(imageData);
            }

            return corners;
        }

    }
}
