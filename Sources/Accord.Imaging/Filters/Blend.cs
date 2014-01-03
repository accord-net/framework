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
    ///   Linear Gradient Blending filter.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   The blending filter is able to blend two images using a homography matrix.
    ///   A linear alpha gradient is used to smooth out differences between the two
    ///   images, effectively blending them in two images. The gradient is computed
    ///   considering the distance between the centers of the two images.</para>
    /// <para>
    ///   The first image should be passed at the moment of creation of the Blending
    ///   filter as the overlay image. A second image may be projected on top of the
    ///   overlay image by calling the Apply method and passing the second image as
    ///   argument.</para>  
    /// <para>
    ///   Currently the filter always produces 32bpp images, disregarding the format
    ///   of source images. The alpha layer is used as an intermediate mask in the
    ///   blending process.</para>  
    /// </remarks>
    /// 
    public class Blend : AForge.Imaging.Filters.BaseTransformationFilter
    {

        private MatrixH homography;
        private Bitmap overlayImage;
        private Point offset;
        private Point center;
        private Size imageSize;
        private Color fillColor = Color.FromArgb(0, Color.Black);
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        private bool gradient = true;
        private bool alphaOnly = false;

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
        ///   Gets or sets a value indicating whether to blend using a linear
        ///   gradient or just superimpose the two images with equal weights.
        /// </summary>
        /// 
        /// <value><c>true</c> to create a gradient; otherwise, <c>false</c>. Default is true.</value>
        /// 
        public bool Gradient
        {
            get { return gradient; }
            set { gradient = value; }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether only the alpha channel
        ///   should be blended. This can be used together with a transparency
        ///   mask to selectively blend only portions of the image.
        /// </summary>
        /// 
        /// <value><c>true</c> to blend only the alpha channel; otherwise, <c>false</c>. Default is false.</value>
        /// 
        public bool AlphaOnly
        {
            get { return alphaOnly; }
            set { alphaOnly = value; }
        }

        /// <summary>
        ///   Constructs a new Blend filter.
        /// </summary>
        /// 
        /// <param name="homography">The homography matrix mapping a second image to the overlay image.</param>
        /// <param name="overlayImage">The overlay image (also called the anchor).</param>
        /// 
        public Blend(double[,] homography, Bitmap overlayImage)
            : this(new MatrixH(homography), overlayImage) { }

        /// <summary>
        ///   Constructs a new Blend filter.
        /// </summary>
        /// 
        /// <param name="overlayImage">The overlay image (also called the anchor).</param>
        /// 
        public Blend(Bitmap overlayImage)
            : this(Matrix.Identity(3), overlayImage) { }

        /// <summary>
        ///   Constructs a new Blend filter.
        /// </summary>
        /// 
        /// <param name="homography">The homography matrix mapping a second image to the overlay image.</param>
        /// <param name="overlayImage">The overlay image (also called the anchor).</param>
        /// 
        public Blend(MatrixH homography, Bitmap overlayImage)
        {
            this.homography = homography;
            this.overlayImage = overlayImage;
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        /// <summary>
        ///   Computes the new image size.
        /// </summary>
        /// 
        protected override Size CalculateNewImageSize(UnmanagedImage sourceData)
        {
            // Calculate source size
            float w = sourceData.Width;
            float h = sourceData.Height;

            // Get the four corners and the center of the image
            PointF[] corners =
            {
                new PointF(0, 0),
                new PointF(w, 0),
                new PointF(0, h),
                new PointF(w, h),
                new PointF(w / 2f, h / 2f)
            };

            // Project those points
            corners = homography.Inverse().TransformPoints(corners);

            // Recalculate image size
            float[] px = { corners[0].X, corners[1].X, corners[2].X, corners[3].X };
            float[] py = { corners[0].Y, corners[1].Y, corners[2].Y, corners[3].Y };

            float maxX = Matrix.Max(px);
            float minX = Matrix.Min(px);
            float newWidth = Math.Max(maxX, overlayImage.Width) - Math.Min(0, minX);

            float maxY = Accord.Math.Matrix.Max(py);
            float minY = Accord.Math.Matrix.Min(py);
            float newHeight = Math.Max(maxY, overlayImage.Height) - Math.Min(0, minY);


            // Store overlay image size
            this.imageSize = new Size((int)Math.Round(maxX - minX), (int)Math.Round(maxY - minY));

            // Store image center
            this.center = Point.Round(corners[4]);

            // Calculate and store image offset
            int offsetX = 0, offsetY = 0;
            if (minX < 0) offsetX = (int)Math.Round(minX);
            if (minY < 0) offsetY = (int)Math.Round(minY);

            this.offset = new Point(offsetX, offsetY);

            if (Double.IsNaN(newWidth) || newWidth == 0)
                newWidth = 1;

            if (Double.IsNaN(newHeight) || newHeight == 0)
                newHeight = 1;

            // Return the final image size
            return new Size((int)Math.Ceiling(newWidth), (int)Math.Ceiling(newHeight));
        }


        /// <summary>
        ///   Process the image filter.
        /// </summary>
        /// 
        protected override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
        {

            // Locks the overlay image
            BitmapData overlayData = overlayImage.LockBits(
              new Rectangle(0, 0, overlayImage.Width, overlayImage.Height),
              ImageLockMode.ReadOnly, overlayImage.PixelFormat);


            // get source image size
            int width = sourceData.Width;
            int height = sourceData.Height;

            // get destination image size
            int newWidth = destinationData.Width;
            int newHeight = destinationData.Height;

            int srcPixelSize = System.Drawing.Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;
            int orgPixelSize = System.Drawing.Image.GetPixelFormatSize(overlayData.PixelFormat) / 8;

            int srcStride = sourceData.Stride;
            int dstOffset = destinationData.Stride - newWidth * 4; // destination always 32bpp argb


            // Get center of first image
            Point center1 = new Point((int)(overlayImage.Width / 2f),
                                      (int)(overlayImage.Height / 2f));

            // Get center of second image
            Point center2 = this.center;



            // Compute maximum center distances
            float dmax1 = Math.Min(
                distance(center1.X, center1.Y, center2.X - imageSize.Width / 2f, center1.Y),
                distance(center1.X, center1.Y, center1.X, center1.Y + overlayImage.Height / 2f));

            float dmax2 = Math.Min(
                distance(center2.X, center2.Y, center2.X + imageSize.Width / 2f, center2.Y),
                distance(center2.X, center2.Y, center2.X, center2.Y + imageSize.Height / 2f));

            float dmax = -System.Math.Abs(dmax2 - dmax1);


            // fill values
            byte fillR = fillColor.R;
            byte fillG = fillColor.G;
            byte fillB = fillColor.B;
            byte fillA = 0;//fillColor.A;

            // Retrieve homography matrix as float array
            float[,] H = (float[,])homography;


            // do the job
            unsafe
            {
                byte* org = (byte*)overlayData.Scan0.ToPointer();
                byte* src = (byte*)sourceData.ImageData.ToPointer();
                byte* dst = (byte*)destinationData.ImageData.ToPointer();

                // destination pixel's coordinate relative to image center
                double cx, cy;

                // destination pixel's homogenous coordinate
                double hx, hy, hw;

                // source pixel's coordinates
                int ox, oy;



                // Copy the overlay image
                for (int y = 0; y < newHeight; y++)
                {
                    for (int x = 0; x < newWidth; x++, dst += 4)
                    {
                        ox = (int)(x + offset.X);
                        oy = (int)(y + offset.Y);

                        // validate source pixel's coordinates
                        if ((ox < 0) || (oy < 0) || (ox >= overlayData.Width) || (oy >= overlayData.Height))
                        {
                            // fill destination image with filler
                            dst[0] = fillB;
                            dst[1] = fillG;
                            dst[2] = fillR;
                            dst[3] = fillA;
                        }
                        else
                        {
                            int c = oy * overlayData.Stride + ox * orgPixelSize;

                            // fill destination image with pixel from original image

                            if (orgPixelSize == 3)
                            {
                                // 24 bpp
                                dst[0] = org[c + 0];
                                dst[1] = org[c + 1];
                                dst[2] = org[c + 2];
                                dst[3] = (byte)255;
                            }
                            else if (orgPixelSize == 4)
                            {
                                // 32 bpp
                                dst[0] = org[c + 0];
                                dst[1] = org[c + 1];
                                dst[2] = org[c + 2];
                                dst[3] = org[c + 3];
                            }
                            else
                            {
                                // 8 bpp
                                dst[0] = org[c];
                                dst[1] = org[c];
                                dst[2] = org[c];
                                dst[3] = org[c];
                            }
                        }
                    }
                    dst += dstOffset;
                }

                org = (byte*)overlayData.Scan0.ToPointer();
                src = (byte*)sourceData.ImageData.ToPointer();
                dst = (byte*)destinationData.ImageData.ToPointer();

                // Project and blend the second image
                for (int y = 0; y < newHeight; y++)
                {
                    for (int x = 0; x < newWidth; x++, dst += 4)
                    {
                        cx = x + offset.X;
                        cy = y + offset.Y;

                        // projection using homogenous coordinates
                        hw = (H[2, 0] * cx + H[2, 1] * cy + H[2, 2]);
                        hx = (H[0, 0] * cx + H[0, 1] * cy + H[0, 2]) / hw;
                        hy = (H[1, 0] * cx + H[1, 1] * cy + H[1, 2]) / hw;

                        // coordinate of the nearest point
                        ox = (int)(hx);
                        oy = (int)(hy);

                        // validate source pixel's coordinates
                        if ((ox >= 0) && (oy >= 0) && (ox < width) && (oy < height))
                        {
                            int c = oy * srcStride + ox * srcPixelSize;

                            // fill destination image with pixel from source image
                            if (srcPixelSize == 4 && src[c + 3] == 0)
                            {
                                // source pixel is fully transparent, nothing to copy
                            }
                            else if (dst[3] > 0)
                            {
                                float f1 = 0.5f, f2 = 0.5f;

                                if (Gradient)
                                {
                                    // there is a pixel from the other image here, blend
                                    float d1 = distance(x, y, center1.X, center1.Y);
                                    float d2 = distance(x, y, center2.X, center2.Y);
                                    f1 = Accord.Math.Tools.Scale(0, dmax, 0, 1, d1 - d2);

                                    if (f1 < 0) f1 = 0f;
                                    if (f1 > 1) f1 = 1f;
                                    f2 = (1f - f1);
                                }

                                if (!AlphaOnly)
                                {
                                    if (srcPixelSize == 3)
                                    {
                                        // 24 bpp
                                        dst[0] = (byte)(src[c + 0] * f2 + dst[0] * f1);
                                        dst[1] = (byte)(src[c + 1] * f2 + dst[1] * f1);
                                        dst[2] = (byte)(src[c + 2] * f2 + dst[2] * f1);
                                        dst[3] = (byte)255;
                                    }
                                    else if (srcPixelSize == 4)
                                    {
                                        // 32 bpp
                                        dst[0] = (byte)(src[c + 0] * f2 + dst[0] * f1);
                                        dst[1] = (byte)(src[c + 1] * f2 + dst[1] * f1);
                                        dst[2] = (byte)(src[c + 2] * f2 + dst[2] * f1);
                                        dst[3] = (byte)(src[c + 3] * f2 + dst[3] * f1);
                                    }
                                    else
                                    {
                                        // 8 bpp
                                        dst[0] = (byte)(src[c] * f2 + dst[0] * f1);
                                        dst[1] = (byte)(src[c] * f2 + dst[1] * f1);
                                        dst[2] = (byte)(src[c] * f2 + dst[2] * f1);
                                        dst[3] = (byte)255;
                                    }
                                }
                                else
                                {
                                    if (srcPixelSize == 3)
                                    {
                                        // 24 bpp
                                        dst[0] = (byte)(src[c + 0]);
                                        dst[1] = (byte)(src[c + 1]);
                                        dst[2] = (byte)(src[c + 2]);
                                    }
                                    else if (srcPixelSize == 4)
                                    {
                                        // 32 bpp
                                        dst[0] = (byte)(src[c + 0]);
                                        dst[1] = (byte)(src[c + 1]);
                                        dst[2] = (byte)(src[c + 2]);
                                    }
                                    else
                                    {
                                        // 8 bpp
                                        dst[0] = (byte)(src[c]);
                                        dst[1] = (byte)(src[c]);
                                        dst[2] = (byte)(src[c]);
                                    }
                                }
                            }
                            else
                            {
                                // just copy the source into the destination image

                                if (srcPixelSize == 3)
                                {
                                    // 24bpp
                                    dst[0] = src[c + 0];
                                    dst[1] = src[c + 1];
                                    dst[2] = src[c + 2];
                                    dst[3] = (byte)255;
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
                                    dst[1] = src[c];
                                    dst[2] = src[c];
                                    dst[3] = 0;
                                }
                            }
                        }
                    }
                    dst += dstOffset;

                }
            }

            overlayImage.UnlockBits(overlayData);
        }

        /// <summary>
        ///   Computes a distance metric used to compute the blending mask
        /// </summary>
        private static float distance(float x1, float y1, float x2, float y2)
        {
            // Euclidean distance
            float u = (x1 - x2);
            float v = (y1 - y2);
            return (float)Math.Sqrt(u * u + v * v);
        }
    }
}
