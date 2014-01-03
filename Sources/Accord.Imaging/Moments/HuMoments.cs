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
    ///   Hu's set of invariant image moments.
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
    ///   Hu's set of invariant moments are invariant under translation, changes in scale, 
    ///   and also rotation. The first moment, <see cref="I1"/>, is analogous to the moment 
    ///   of inertia around the image's centroid, where the pixels' intensities are analogous
    ///   to physical density. The last one, I7, is skew invariant, which enables it to distinguish
    ///   mirror images of otherwise identical images.</para>
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
    /// // Compute the Hu moments of up to third order
    /// HuMoments hu = new HuMoments(image, order: 3);
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="RawMoments"/>
    /// <seealso cref="CentralMoments"/>
    /// 
    public class HuMoments : MomentsBase, IMoments
    {

        private const int defaultOrder = 3;

        /// <summary>
        ///   Hu moment of order 1.
        /// </summary>
        /// 
        public float I1 { get; private set; }

        /// <summary>
        ///   Hu moment of order 2.
        /// </summary>
        /// 
        public float I2 { get; private set; }

        /// <summary>
        ///   Hu moment of order 3.
        /// </summary>
        /// 
        public float I3 { get; private set; }

        /// <summary>
        ///   Hu moment of order 4.
        /// </summary>
        /// 
        public float I4 { get; private set; }

        /// <summary>
        ///   Hu moment of order 5.
        /// </summary>
        /// 
        public float I5 { get; private set; }

        /// <summary>
        ///   Hu moment of order 6.
        /// </summary>
        /// 
        public float I6 { get; private set; }

        /// <summary>
        ///   Hu moment of order 7.
        /// </summary>
        /// 
        public float I7 { get; private set; }



        /// <summary>
        ///   Initializes a new instance of the <see cref="HuMoments"/> class.
        /// </summary>
        /// 
        /// <param name="order">The maximum moment order to be computed.</param>
        /// 
        public HuMoments(int order = defaultOrder)
            : base(order) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="HuMoments"/> class.
        /// </summary>
        /// 
        /// <param name="order">The maximum moment order to be computed.</param>
        /// <param name="image">The image whose moments should be computed.</param>
        /// 
        public HuMoments(Bitmap image, int order = defaultOrder)
            : base(image, order) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="HuMoments"/> class.
        /// </summary>
        /// 
        /// <param name="order">The maximum order for the moments.</param>
        /// <param name="image">The image whose moments should be computed.</param>
        /// 
        public HuMoments(float[,] image, int order = defaultOrder)
            : base(image, order) { }


        /// <summary>
        ///   Computes the Hu moments from the specified central moments.
        /// </summary>
        /// 
        /// <param name="moments">The central moments to use as base of calculations.</param>
        /// 
        public void Compute(CentralMoments moments)
        {
            double inv = 1.0 / moments.Mu00;
            double inv2 = 1.0 / (moments.Mu00 * moments.Mu00);
            double inv5d2 = Math.Sqrt(inv2 * inv2 * inv);


            float n20 = (float)(moments.Mu20 * inv2);
            float n02 = (float)(moments.Mu02 * inv2);
            float n11 = (float)(moments.Mu11 * inv2);

            float n21 = (float)(moments.Mu21 * inv5d2);
            float n12 = (float)(moments.Mu12 * inv5d2);
            float n30 = (float)(moments.Mu30 * inv5d2);
            float n03 = (float)(moments.Mu03 * inv5d2);


            //   (η20 + η02)
            I1 = (n20 + n02);


            //   (η20 − η02)²              + 4    η11²
            I2 = (n20 - n02) * (n20 - n02) + 4 * (n11 * n11);


            //   (η30 − 3   η12)²
            I3 = (n30 - 3 * n12) * (n30 - 3 * n12)

            // + (3   η21 − η03)²
               + (3 * n21 - n03) * (3 * n21 - n03);


            //   (η30 + η12)²              + (η21 + η03)²
            I4 = (n30 + n12) * (n30 + n12) + (n21 + n03) * (n21 + n03);


            //   (η30 − 3   η12)   (η30 + η12)   [(η30 + η12)²               −3   (η21 + η03)²             ]
            I5 = (n30 - 3 * n12) * (n30 + n12) * ((n30 + n12) * (n30 + n12) - 3 * (n21 + n03) * (n21 + n03))

            //   (3   η21 − η03)   (η21 + η03)   [3   (η30 + η12)²              − (η21 + η03)²             ]
               + (3 * n21 - n03) * (n21 + n03) * (3 * (n30 + n12) * (n30 + n12) - (n21 + n03) * (n21 + n03));


            //   (η20 − η02)   [(η30 + η12)²              − (η21 + η03)²             ]
            I6 = (n20 - n02) * ((n30 + n12) * (n30 + n12) - (n21 + n03) * (n21 + n03))

            //  + 4   η11   (η30 + η12)   (η21 + η03)
                + 4 * n11 * (n30 + n12) * (n21 + n03);


            //   (3   η21 − η03)   (η30 + η12)                 [(η30 + η12)²              − 3   (η21 + η03)²             ]
            I7 = (3 * n21 - n03) * (n30 + n12) * (n30 + n12) * ((n30 + n12) * (n30 + n12) - 3 * (n21 + n03) * (n21 + n03))

            // - (η30 − 3   η12)   (η21 + η03)   [3   (η30 + η12)²              − (η21 + η03)²             ]
               - (n30 - 3 * n12) * (n21 + n03) * (3 * (n30 + n12) * (n30 + n12) - (n21 + n03) * (n21 + n03));
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
            RawMoments raw = new RawMoments(image, area, Order);
            CentralMoments center = new CentralMoments(raw);
            this.Compute(center);
        }

        /// <summary>
        ///   Computes the center moments for the specified image.
        /// </summary>
        /// 
        /// <param name="image">The image whose moments should be computed.</param>
        /// <param name="area">The region of interest in the image to compute moments for.</param>
        /// 
        public override void Compute(float[,] image, Rectangle area)
        {
            RawMoments raw = new RawMoments(image, area, Order);
            CentralMoments center = new CentralMoments(raw);
            this.Compute(center);
        }

    }
}
