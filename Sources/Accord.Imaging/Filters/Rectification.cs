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

namespace Accord.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge.Imaging;
    using Matrix = Accord.Math.Matrix;

    /// <summary>
    ///   Rectification filter for projective transformation.
    /// </summary>
    /// 
    public class Rectification : AForge.Imaging.Filters.BaseTransformationFilter
    {

        private MatrixH homography;
        private Color fillColor = Color.FromArgb(0, Color.Black);
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();



        /// <summary>
        ///   Format translations dictionary.
        /// </summary>
        /// 
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        ///   Gets or sets the Homography matrix used to map a image passed to
        ///   the filter to the overlay image specified at filter creation.
        /// </summary>
        /// 
        public MatrixH Homography
        {
            get { return homography; }
            set { homography = value; }
        }

        /// <summary>
        ///   Gets or sets the filling color used to fill blank spaces.
        /// </summary>
        /// 
        /// <remarks>
        ///   The filling color will only be visible after the image is converted
        ///   to 24bpp. The alpha channel will be used internally by the filter.
        /// </remarks>
        /// 
        public Color FillColor
        {
            get { return fillColor; }
            set { fillColor = value; }
        }

        /// <summary>
        ///   Constructs a new Blend filter.
        /// </summary>
        /// 
        /// <param name="homography">The homography matrix mapping a second image to the overlay image.</param>
        /// 
        public Rectification(double[,] homography)
            : this(new MatrixH(homography)) { }


        /// <summary>
        ///   Constructs a new Blend filter.
        /// </summary>
        /// 
        /// <param name="homography">The homography matrix mapping a second image to the overlay image.</param>
        /// 
        public Rectification(MatrixH homography)
        {
            this.homography = homography;
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        /// <summary>
        ///   Computes the new image size.
        /// </summary>
        /// 
        protected override Size CalculateNewImageSize(UnmanagedImage sourceData)
        {
            return new Size(sourceData.Width, sourceData.Height);
        }


        /// <summary>
        ///   Process the image filter.
        /// </summary>
        /// 
        protected override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
        {

            // get source image size
            int width = sourceData.Width;
            int height = sourceData.Height;

            int srcStride = sourceData.Stride;
            int srcPixelSize = System.Drawing.Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;
            int srcOffset = sourceData.Stride - width * srcPixelSize;

            // get destination image size
            int newWidth = destinationData.Width;
            int newHeight = destinationData.Height;

            int dstStride = destinationData.Stride;
            int dstOffset = destinationData.Stride - newWidth * srcPixelSize;


            // fill values
            byte fillR = fillColor.R;
            byte fillG = fillColor.G;
            byte fillB = fillColor.B;
            byte fillA = fillColor.A;

            // Retrieve homography matrix as float array
            float[,] H = (float[,])homography;


            // do the job
            unsafe
            {
                byte* src = (byte*)sourceData.ImageData.ToPointer();
                byte* dst = (byte*)destinationData.ImageData.ToPointer();

                // Project the second image
                for (int y = 0; y < newHeight; y++)
                {
                    for (int x = 0; x < newWidth; x++, dst += srcPixelSize)
                    {
                        double cx = x;
                        double cy = y;

                        // projection using homogenous coordinates
                        double hw = (H[2, 0] * cx + H[2, 1] * cy + H[2, 2]);
                        double hx = (H[0, 0] * cx + H[0, 1] * cy + H[0, 2]) / hw;
                        double hy = (H[1, 0] * cx + H[1, 1] * cy + H[1, 2]) / hw;

                        // coordinate of the nearest point
                        int ox = (int)(hx);
                        int oy = (int)(hy);

                        // validate source pixel's coordinates
                        if ((ox >= 0) && (oy >= 0) && (ox < width) && (oy < height))
                        {
                            int c = oy * srcStride + ox * srcPixelSize;

                            // just copy the source into the destination image
                            if (srcPixelSize == 3)
                            {
                                // 24bpp
                                dst[0] = src[c + 0];
                                dst[1] = src[c + 1];
                                dst[2] = src[c + 2];
                            }
                            else if (srcPixelSize == 4)
                            {
                                // 32bpp
                                dst[0] = src[c + 0];
                                dst[1] = src[c + 1];
                                dst[2] = src[c + 2];
                                dst[3] = src[c + 3];
                            }
                            else
                            {
                                // 8bpp
                                dst[0] = src[c];
                            }
                        }
                        else
                        {
                            // Fill.

                            // just copy the source into the destination image
                            if (srcPixelSize == 3)
                            {
                                // 24bpp
                                dst[0] = fillB;
                                dst[1] = fillG;
                                dst[2] = fillR;
                            }
                            else if (srcPixelSize == 4)
                            {
                                // 32bpp
                                dst[0] = fillA;
                                dst[1] = fillB;
                                dst[2] = fillG;
                                dst[3] = fillR;
                            }
                            else
                            {
                                // 8bpp
                                dst[0] = fillR;
                            }
                        }
                    }

                    dst += dstOffset;
                }
            }

        }


    }
}
