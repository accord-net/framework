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

namespace Accord.Imaging.Moments
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge.Imaging;

    /// <summary>
    ///   Raw image moments.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In image processing, computer vision and related fields, an image moment is
    ///   a certain particular weighted average (moment) of the image pixels' intensities,
    ///   or a function of such moments, usually chosen to have some attractive property 
    ///   or interpretation.</para>
    ///
    /// <para>
    ///   Image moments are useful to describe objects after segmentation. Simple properties 
    ///   of the image which are found via image moments include area (or total intensity), 
    ///   its centroid, and information about its orientation.</para>
    ///   
    /// <para>
    ///   The raw moments are the most basic moments which can be computed from an image,
    ///   and can then be further processed to achieve <see cref="CentralMoments"/> or even
    ///   <see cref="HuMoments"/>.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia contributors. "Image moment." Wikipedia, The Free Encyclopedia. Wikipedia,
    ///       The Free Encyclopedia. Available at http://en.wikipedia.org/wiki/Image_moment </description></item>
    ///   </list>
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// Bitmap image = ...;
    ///
    /// // Compute the raw moments of up to third order
    /// RawMoments m = new RawMoments(image, order: 3);
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="HuMoments"/>
    /// <seealso cref="CentralMoments"/>
    /// 
    public class RawMoments : MomentsBase, IMoments
    {

        /// <summary>
        ///   Gets the default maximum moment order.
        /// </summary>
        /// 
        public const int DefaultOrder = 3;


        /// <summary>
        ///   Raw moment of order (0,0).
        /// </summary>
        /// 
        public float M00 { get; private set; }

        /// <summary>
        ///   Raw moment of order (1,0).
        /// </summary>
        /// 
        public float M10 { get; private set; }

        /// <summary>
        ///   Raw moment of order (0,1).
        /// </summary>
        /// 
        public float M01 { get; private set; }

        /// <summary>
        ///   Raw moment of order (1,1).
        /// </summary>
        /// 
        public float M11 { get; private set; }

        /// <summary>
        ///   Raw moment of order (2,0).
        /// </summary>
        /// 
        public float M20 { get; private set; }

        /// <summary>
        ///   Raw moment of order (0,2).
        /// </summary>
        /// 
        public float M02 { get; private set; }

        /// <summary>
        ///   Raw moment of order (2,1).
        /// </summary>
        /// 
        public float M21 { get; private set; }

        /// <summary>
        ///   Raw moment of order (1,2).
        /// </summary>
        /// 
        public float M12 { get; private set; }

        /// <summary>
        ///   Raw moment of order (3,0).
        /// </summary>
        /// 
        public float M30 { get; private set; }

        /// <summary>
        ///   Raw moment of order (0,3).
        /// </summary>
        /// 
        public float M03 { get; private set; }


        /// <summary>
        ///   Inverse raw moment of order (0,0).
        /// </summary>
        /// 
        public float InvM00 { get; private set; }



        /// <summary>
        ///   Gets the X centroid of the image.
        /// </summary>
        /// 
        public float CenterX { get; private set; }

        /// <summary>
        ///   Gets the Y centroid of the image.
        /// </summary>
        /// 
        public float CenterY { get; private set; }

        /// <summary>
        ///   Gets the area (for binary images) or sum of
        ///   gray level (for grayscale images).
        /// </summary>
        /// 
        public float Area { get { return M00; } }


        /// <summary>
        ///   Initializes a new instance of the <see cref="Moments"/> class.
        /// </summary>
        /// 
        public RawMoments(int order = DefaultOrder)
            : base(order) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Moments"/> class.
        /// </summary>
        /// 
        public RawMoments(Bitmap image, Rectangle area, int order = DefaultOrder)
            : base(order)
        {
            Compute(image, area);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Moments"/> class.
        /// </summary>
        /// 
        public RawMoments(UnmanagedImage image, Rectangle area, int order = DefaultOrder)
            : base(image, area, order) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Moments"/> class.
        /// </summary>
        /// 
        public RawMoments(float[,] image, Rectangle area, int order = DefaultOrder)
            : base(image, area, order) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Moments"/> class.
        /// </summary>
        /// 
        public RawMoments(UnmanagedImage image, int order = DefaultOrder)
            : base(image, order) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Moments"/> class.
        /// </summary>
        /// 
        public RawMoments(Bitmap image, int order = DefaultOrder)
            : base(image, order) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Moments"/> class.
        /// </summary>
        /// 
        public RawMoments(float[,] image, int order = DefaultOrder)
            : base(image, order) { }



        /// <summary>
        ///   Computes the raw moments for the specified image.
        /// </summary>
        /// 
        /// <param name="image">The image whose moments should be computed.</param>
        /// <param name="area">The region of interest in the image to compute moments for.</param>
        /// <param name="secondOrder"><c>True</c> to compute second order moments, <c>false</c> otherwise.</param>
        /// 
        [Obsolete("Use the Order property to determine the maximum order to be computed.")]
        public void Compute(float[,] image, Rectangle area, bool secondOrder)
        {
            Order = secondOrder ? 2 : 1;
            Compute(image, area);
        }

        /// <summary>
        ///   Computes the raw moments for the specified image.
        /// </summary>
        /// 
        /// <param name="image">The image.</param>
        /// <param name="area">The region of interest in the image to compute moments for.</param>
        /// 
        public unsafe override void Compute(float[,] image, Rectangle area)
        {
            int height = image.GetLength(0);
            int width = image.GetLength(1);

            if (area == Rectangle.Empty)
                area = new Rectangle(0, 0, width, height);

            Reset();

            // stay inside the image
            int windowX = Math.Max(area.X, 0);
            int windowY = Math.Max(area.Y, 0);
            int windowWidth = Math.Min(windowX + area.Width, width);
            int windowHeight = Math.Min(windowY + area.Height, height);

            int offset = width - (windowWidth - windowX);

            fixed (float* ptrImage = image)
            {
                float* src = ptrImage + windowY * width + windowX;

                // TODO: Walk using x and y directly instead of i and j.

                for (int j = windowY; j < windowHeight; j++)
                {
                    float y = j - windowY;

                    for (int i = windowX; i < windowWidth; i++, src++)
                    {
                        float x = i - windowX;

                        float v = *src;

                        M00 += v;
                        M01 += y * v;
                        M10 += x * v;

                        if (Order >= 2)
                        {
                            M11 += x * y * v;
                            M02 += y * y * v;
                            M20 += x * x * v;
                        }

                        if (Order >= 3)
                        {
                            M12 += x * y * y * v;
                            M21 += x * x * y * v;

                            M30 += x * x * x * v;
                            M03 += y * y * y * v;
                        }
                    }

                    src += offset;
                }
            }

            InvM00 = 1f / M00;
            CenterX = M10 * InvM00;
            CenterY = M01 * InvM00;
        }

        /// <summary>
        ///   Computes the raw moments for the specified image.
        /// </summary>
        /// 
        /// <param name="image">The image.</param>
        /// <param name="area">The region of interest in the image to compute moments for.</param>
        /// 
        public unsafe override void Compute(UnmanagedImage image, Rectangle area)
        {
            int height = image.Height;
            int width = image.Width;
            int stride = image.Stride;

            if (area == Rectangle.Empty)
                area = new Rectangle(0, 0, width, height);

            Reset();


            // stay inside the image
            int windowX = Math.Max(area.X, 0);
            int windowY = Math.Max(area.Y, 0);
            int windowWidth = Math.Min(windowX + area.Width, width);
            int windowHeight = Math.Min(windowY + area.Height, height);


            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                int offset = stride - (windowWidth - windowX);

                byte* src = (byte*)image.ImageData.ToPointer() + windowY * stride + windowX;

                for (int j = windowY; j < windowHeight; j++)
                {
                    float y = j - windowY;

                    for (int i = windowX; i < windowWidth; i++, src++)
                    {
                        float x = i - windowX;

                        float v = *src;

                        M00 += v;
                        M01 += y * v;
                        M10 += x * v;

                        if (Order >= 2)
                        {
                            M11 += x * y * v;
                            M02 += y * y * v;
                            M20 += x * x * v;
                        }

                        if (Order >= 3)
                        {
                            M12 += x * y * y * v;
                            M21 += x * x * y * v;

                            M30 += x * x * x * v;
                            M03 += y * y * y * v;
                        }
                    }

                    src += offset;
                }
            }
            else
            {
                // color images
                int pixelSize = Bitmap.GetPixelFormatSize(image.PixelFormat) / 8;
                int offset = stride - (windowWidth - windowX) * pixelSize;

                byte* src = (byte*)image.ImageData.ToPointer() + windowY * stride + windowX * pixelSize;

                for (int j = windowY; j < windowHeight; j++)
                {
                    float y = j - windowY;

                    for (int i = windowX; i < windowWidth; i++, src += pixelSize)
                    {
                        float x = i - windowX;

                        // BT709 - 0.2125, 0.7154, 0.0721 
                        float v = (float)(0.2125 * src[RGB.R] + 0.7154 * src[RGB.G] + 0.0721 * src[RGB.B]);

                        M00 += v;
                        M01 += y * v;
                        M10 += x * v;

                        if (Order >= 2)
                        {
                            M11 += x * y * v;
                            M02 += y * y * v;
                            M20 += x * x * v;
                        }

                        if (Order >= 3)
                        {
                            M12 += x * y * y * v;
                            M21 += x * x * y * v;

                            M30 += x * x * x * v;
                            M03 += y * y * y * v;
                        }
                    }

                    src += offset;
                }
            }


            InvM00 = 1f / M00;
            CenterX = M10 * InvM00;
            CenterY = M01 * InvM00;
        }


        /// <summary>
        ///   Resets all moments to zero.
        /// </summary>
        /// 
        protected void Reset()
        {
            M00 = M10 = M01 = 0;
            M11 = M20 = M02 = 0;

            M21 = M12 = 0;
            M30 = M03 = 0;

            InvM00 = 0;

            CenterX = CenterY = 0;
        }

    }
}
