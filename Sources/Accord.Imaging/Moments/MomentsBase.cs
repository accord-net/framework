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
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge.Imaging;

    /// <summary>
    ///   Base class for image moments.
    /// </summary>
    /// 
    public abstract class MomentsBase
    {
        /// <summary>
        ///   Gets or sets the maximum order of the moments.
        /// </summary>
        /// 
        public int Order { get; set; }



        /// <summary>
        ///   Initializes a new instance of the <see cref="MomentsBase"/> class.
        /// </summary>
        /// 
        /// <param name="order">The maximum order for the moments.</param>
        ///
        protected MomentsBase(int order)
        {
            Order = order;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MomentsBase"/> class.
        /// </summary>
        /// 
        /// <param name="order">The maximum order for the moments.</param>
        /// <param name="image">The image whose moments should be computed.</param>
        /// <param name="area">The region of interest in the image to compute moments for.</param>
        ///
        protected MomentsBase(Bitmap image, Rectangle area, int order)
            : this(order)
        {
            Compute(image, area);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MomentsBase"/> class.
        /// </summary>
        /// 
        /// <param name="order">The maximum order for the moments.</param>
        /// <param name="image">The image whose moments should be computed.</param>
        /// <param name="area">The region of interest in the image to compute moments for.</param>
        /// 
        protected MomentsBase(UnmanagedImage image, Rectangle area, int order)
            : this(order)
        {
            Compute(image, area);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MomentsBase"/> class.
        /// </summary>
        /// 
        /// <param name="order">The maximum order for the moments.</param>
        /// <param name="image">The image whose moments should be computed.</param>
        /// <param name="area">The region of interest in the image to compute moments for.</param>
        ///
        protected MomentsBase(float[,] image, Rectangle area, int order)
            : this(order)
        {
            Compute(image, area);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MomentsBase"/> class.
        /// </summary>
        /// 
        /// <param name="order">The maximum order for the moments.</param>
        /// <param name="image">The image whose moments should be computed.</param>
        /// 
        protected MomentsBase(Bitmap image, int order)
            : this(order)
        {
            Compute(image, Rectangle.Empty);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MomentsBase"/> class.
        /// </summary>
        /// 
        /// <param name="order">The maximum order for the moments.</param>
        /// <param name="image">The image whose moments should be computed.</param>
        ///
        protected MomentsBase(UnmanagedImage image, int order)
            : this(order)
        {
            Compute(image, Rectangle.Empty);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MomentsBase"/> class.
        /// </summary>
        /// 
        /// <param name="order">The maximum order for the moments.</param>
        /// <param name="image">The image whose moments should be computed.</param>
        ///
        protected MomentsBase(float[,] image, int order)
            : this(order)
        {
            Compute(image, Rectangle.Empty);
        }


        /// <summary>
        ///   Computes the moments for the specified image.
        /// </summary>
        /// 
        /// <param name="image">The image whose moments should be computed.</param>
        /// 
        public void Compute(Bitmap image)
        {
            Compute(image, new Rectangle(0, 0, image.Width, image.Height));
        }

        /// <summary>
        ///   Computes the moments for the specified image.
        /// </summary>
        /// 
        /// <param name="image">The image whose moments should be computed.</param>
        /// 
        public void Compute(UnmanagedImage image)
        {
            Compute(image, new Rectangle(0, 0, image.Width, image.Height));
        }

        /// <summary>
        ///   Computes the moments for the specified image.
        /// </summary>
        /// 
        /// <param name="image">The image whose moments should be computed.</param>
        /// 
        public void Compute(BitmapData image)
        {
            Compute(image, new Rectangle(0, 0, image.Width, image.Height));
        }

        /// <summary>
        ///   Computes the moments for the specified image.
        /// </summary>
        /// 
        /// <param name="image">The image whose moments should be computed.</param>
        /// <param name="area">The region of interest in the image to compute moments for.</param>
        /// 
        public void Compute(Bitmap image, Rectangle area)
        {
            // lock source bitmap data
            BitmapData imageData = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, image.PixelFormat);

            try
            {
                Compute(new UnmanagedImage(imageData), area);
            }
            finally
            {
                image.UnlockBits(imageData);
            }
        }

        /// <summary>
        ///   Computes the moments for the specified image.
        /// </summary>
        /// 
        /// <param name="image">The image whose moments should be computed.</param>
        /// <param name="area">The region of interest in the image to compute moments for.</param>
        /// 
        public void Compute(BitmapData image, Rectangle area)
        {
            Compute(new UnmanagedImage(image), area);
        }

        /// <summary>
        ///   Computes the moments for the specified image.
        /// </summary>
        /// 
        /// <param name="image">The image whose moments should be computed.</param>
        /// <param name="area">The region of interest in the image to compute moments for.</param>
        /// 
        public abstract void Compute(UnmanagedImage image, Rectangle area);

        /// <summary>
        ///   Computes the moments for the specified image.
        /// </summary>
        /// 
        /// <param name="image">The image whose moments should be computed.</param>
        /// <param name="area">The region of interest in the image to compute moments for.</param>
        /// 
        public abstract void Compute(float[,] image, Rectangle area);

    }
}
