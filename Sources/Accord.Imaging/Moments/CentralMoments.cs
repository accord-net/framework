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
    using AForge.Imaging;

    /// <summary>
    ///   Central image moments.
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
    ///   The central moments can be used to find the location, center of mass and the 
    ///   dimensions of a given object within an image.</para>
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
    /// // Compute the center moments of up to third order
    /// CentralMoments cm = new CentralMoments(image, order: 3);
    /// 
    /// // Get size and orientation of the image
    /// SizeF size = target.GetSize();
    /// float angle = target.GetOrientation();
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="RawMoments"/>
    /// <seealso cref="HuMoments"/>
    /// 
    public class CentralMoments : MomentsBase, IMoments
    {
        /// <summary>
        ///   Gets the default maximum moment order.
        /// </summary>
        /// 
        public const int DefaultOrder = 2;

        /// <summary>
        ///   Central moment of order (0,0).
        /// </summary>
        /// 
        public float Mu00 { get; private set; }

        /// <summary>
        ///   Central moment of order (1,0).
        /// </summary>
        /// 
        public float Mu10 { get; private set; }

        /// <summary>
        ///   Central moment of order (0,1).
        /// </summary>
        /// 
        public float Mu01 { get; private set; }

        /// <summary>
        ///   Central moment of order (1,1).
        /// </summary>
        /// 
        public float Mu11 { get; private set; }

        /// <summary>
        ///   Central moment of order (2,0).
        /// </summary>
        /// 
        public float Mu20 { get; private set; }

        /// <summary>
        ///   Central moment of order (0,2).
        /// </summary>
        /// 
        public float Mu02 { get; private set; }

        /// <summary>
        ///   Central moment of order (2,1).
        /// </summary>
        /// 
        public float Mu21 { get; private set; }

        /// <summary>
        ///   Central moment of order (1,2).
        /// </summary>
        /// 
        public float Mu12 { get; private set; }

        /// <summary>
        ///   Central moment of order (3,0).
        /// </summary>
        /// 
        public float Mu30 { get; private set; }

        /// <summary>
        ///   Central moment of order (0,3).
        /// </summary>
        /// 
        public float Mu03 { get; private set; }


        private float invM00;


        /// <summary>
        ///   Initializes a new instance of the <see cref="CentralMoments"/> class.
        /// </summary>
        /// 
        public CentralMoments(int order = DefaultOrder) :
            base(order) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CentralMoments"/> class.
        /// </summary>
        /// 
        /// <param name="moments">The raw moments to construct central moments.</param>
        /// 
        public CentralMoments(RawMoments moments)
            : base(moments.Order)
        {
            Compute(moments);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CentralMoments"/> class.
        /// </summary>
        /// 
        /// <param name="order">The maximum order for the moments.</param>
        /// <param name="image">The image whose moments should be computed.</param>
        /// 
        public CentralMoments(Bitmap image, int order = DefaultOrder)
            : base(image, order) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CentralMoments"/> class.
        /// </summary>
        /// 
        /// <param name="order">The maximum order for the moments.</param>
        /// <param name="image">The image whose moments should be computed.</param>
        /// 
        public CentralMoments(float[,] image, int order = DefaultOrder)
            : base(image, order) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CentralMoments"/> class.
        /// </summary>
        /// 
        /// <param name="order">The maximum order for the moments.</param>
        /// <param name="image">The image whose moments should be computed.</param>
        /// <param name="area">The region of interest in the image to compute moments for.</param>
        /// 
        public CentralMoments(Bitmap image, Rectangle area, int order = DefaultOrder)
            : base(image, area, order) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CentralMoments"/> class.
        /// </summary>
        /// 
        /// <param name="order">The maximum order for the moments.</param>
        /// <param name="image">The image whose moments should be computed.</param>
        /// <param name="area">The region of interest in the image to compute moments for.</param>
        /// 
        public CentralMoments(UnmanagedImage image, Rectangle area, int order = DefaultOrder)
            : base(image, area, order) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CentralMoments"/> class.
        /// </summary>
        /// 
        /// <param name="order">The maximum order for the moments.</param>
        /// <param name="image">The image whose moments should be computed.</param>
        /// <param name="area">The region of interest in the image to compute moments for.</param>
        /// 
        public CentralMoments(float[,] image, Rectangle area, int order = DefaultOrder)
            : base(image, area, order) { }



        /// <summary>
        ///   Computes the center moments from the specified raw moments.
        /// </summary>
        /// 
        /// <param name="moments">The raw moments to use as base of calculations.</param>
        /// 
        public void Compute(RawMoments moments)
        {
            float x = moments.CenterX;
            float y = moments.CenterY;

            Mu00 = moments.M00;

            Mu01 = Mu10 = 0;
            Mu11 = moments.M11 - moments.M01 * x;

            Mu20 = moments.M20 - moments.M10 * x;
            Mu02 = moments.M02 - moments.M01 * y;

            Mu21 = moments.M21 - 2 * x * moments.M11 - y * moments.M20 + 2 * x * x * moments.M01;
            Mu12 = moments.M12 - 2 * y * moments.M11 - x * moments.M02 + 2 * y * y * moments.M10;

            Mu30 = moments.M30 - 3 * x * moments.M20 + 2 * x * x * moments.M10;
            Mu03 = moments.M03 - 3 * y * moments.M02 + 2 * y * y * moments.M01;

            invM00 = moments.InvM00;
        }

        /// <summary>
        ///   Computes the center moments for the specified image.
        /// </summary>
        /// 
        /// <param name="image">The image.</param>
        /// <param name="area">The region of interest in the image to compute moments for.</param>
        /// 
        public override void Compute(float[,] image, Rectangle area)
        {
            this.Compute(new RawMoments(image, area, Order));
        }

        /// <summary>
        ///   Computes the center moments for the specified image.
        /// </summary>
        /// 
        /// <param name="image">The image whose moments should be computed.</param>
        /// <param name="area">The region of interest in the image to compute moments for.</param>
        /// 
        public override void Compute(UnmanagedImage image, Rectangle area)
        {
            this.Compute(new RawMoments(image, area, Order));
        }

        /// <summary>
        ///   Gets the size of the ellipse containing the image.
        /// </summary>
        /// 
        /// <returns>The size of the ellipse containing the image.</returns>
        /// 
        public SizeF GetSize()
        {
            // Compute the covariance matrix
            //
            double a = Mu20 * invM00; //                | a    b |
            double b = Mu11 * invM00; //  Cov[I(x,y)] = |        |
            double c = Mu02 * invM00; //                | b    c |

            double d = a + c, e = a - c;
            double s = Math.Sqrt((4.0 * b * b) + (e * e));

            // Compute size
            return new SizeF((float)Math.Sqrt((d - s) * 0.5) * 4,
                             (float)Math.Sqrt((d + s) * 0.5) * 4);
        }

        /// <summary>
        ///   Gets the orientation of the ellipse containing the image.
        /// </summary>
        /// 
        /// <returns>The angle of orientation of the ellipse, in radians.</returns>
        /// 
        public float GetOrientation()
        {
            // Compute the covariance matrix
            //
            double a = Mu20 * invM00; //                | a    b |
            double b = Mu11 * invM00; //  Cov[I(x,y)] = |        |
            double c = Mu02 * invM00; //                | b    c |

            // Compute eigenvalues of the covariance matrix
            double d = a + c, e = a - c;
            double s = Math.Sqrt((4.0 * b * b) + (e * e));

            // Compute angle
            float angle = (float)Math.Atan2(2.0 * b, e + s);
            if (angle < 0) angle = (float)(angle + Math.PI);

            return angle;
        }

        /// <summary>
        ///   Gets both size and orientation of the ellipse containing the image.
        /// </summary>
        /// 
        /// <param name="angle">The angle of orientation of the ellipse, in radians.</param>
        /// <returns>The size of the ellipse containing the image.</returns>
        /// 
        public SizeF GetSizeAndOrientation(out float angle)
        {
            // Compute the covariance matrix
            //
            double a = Mu20 * invM00; //                | a    b |
            double b = Mu11 * invM00; //  Cov[I(x,y)] = |        |
            double c = Mu02 * invM00; //                | b    c |

            double d = a + c, e = a - c;
            double s = Math.Sqrt((4.0 * b * b) + (e * e));

            // Compute angle
            angle = (float)Math.Atan2(2.0 * b, e + s);
            if (angle < 0) angle = (float)(angle + Math.PI);

            // Compute size
            return new SizeF((float)Math.Sqrt((d - s) * 0.5) * 4,
                             (float)Math.Sqrt((d + s) * 0.5) * 4);
        }



    }
}
