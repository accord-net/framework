// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Diego Catalano, 2013
// diego.catalano at live.com
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
    using System.Drawing.Imaging;

    /// <summary>
    ///   Difference of Gaussians filter.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In imaging science, the difference of Gaussians is a feature 
    ///   enhancement algorithm that involves the subtraction of one blurred 
    ///   version of an original image from another, less blurred version of 
    ///   the original. </para>
    ///   
    /// <para>
    ///   In the simple case of grayscale images, the blurred images are 
    ///   obtained by convolving the original grayscale images with Gaussian
    ///   kernels having differing standard deviations. Blurring an image using
    ///   a Gaussian kernel suppresses only high-frequency spatial information.
    ///   Subtracting one image from the other preserves spatial information that
    ///   lies between the range of frequencies that are preserved in the two blurred
    ///   images. Thus, the difference of Gaussians is a band-pass filter that 
    ///   discards all but a handful of spatial frequencies that are present in the
    ///   original grayscale image.</para>
    ///   
    /// <para>
    ///  This filter implementation has been contributed by Diego Catalano.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///        Wikipedia contributors. "Difference of Gaussians." Wikipedia, The Free 
    ///        Encyclopedia. Wikipedia, The Free Encyclopedia, 1 Jun. 2013. Web. 10 Feb.
    ///        2014.</description></item>
    ///   </list></para>   
    /// </remarks>
    ///
    /// <example>
    /// <code>
    ///   Bitmap image = ... // Lena's famous picture
    /// 
    ///   // Create a new Difference of Gaussians
    ///   var DoG = new DifferenceOfGaussians();
    /// 
    ///   // Apply the filter
    ///   Bitmap result = DoG.Apply(image);
    ///   
    ///   // Show on the screen
    ///   ImageBox.Show(result);
    /// </code>
    /// 
    /// <para>
    ///   The resulting image is shown below. </para>
    ///   
    /// <img src="..\images\differenceOfGaussians.png" /> 
    ///
    /// </example>
    /// 
    public class DifferenceOfGaussians : BaseInPlaceFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;

        GaussianBlur first;
        GaussianBlur second;
        Subtract subtract;

        /// <summary>
        ///   Gets or sets the first Gaussian filter.
        /// </summary>
        /// 
        public GaussianBlur First
        {
            get { return first; }
            set { first = value; }
        }

        /// <summary>
        ///   Gets or sets the second Gaussian filter.
        /// </summary>
        /// 
        public GaussianBlur Second
        {
            get { return first; }
            set { first = value; }
        }

        /// <summary>
        ///   Gets or sets the subtract filter used to compute
        ///   the difference of the two Gaussian blurs.
        /// </summary>
        /// 
        public Subtract Subtract
        {
            get { return subtract; }
            set { subtract = value; }
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
        ///   Initializes a new instance of the <see cref="DifferenceOfGaussians"/> class.
        /// </summary>
        /// 
        public DifferenceOfGaussians()
        {
            formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
            formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format64bppArgb;

            this.first = new GaussianBlur()
            {
                Size = 3,
                Sigma = 0.4
            };

            this.second = new GaussianBlur()
            {
                Size = 5,
                Sigma = 0.4
            };

            this.subtract = new Subtract();
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="DifferenceOfGaussians"/> class.
        /// </summary>
        /// 
        /// <param name="windowSize1">The first window size. Default is 3</param>
        /// <param name="windowSize2">The second window size. Default is 4.</param>
        /// 
        public DifferenceOfGaussians(int windowSize1, int windowSize2)
            : this()
        {
            this.first.Size = windowSize1;
            this.second.Size = windowSize2;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DifferenceOfGaussians"/> class.
        /// </summary>
        /// 
        /// <param name="windowSize1">The window size for the first Gaussian. Default is 3</param>
        /// <param name="windowSize2">The window size for the second Gaussian. Default is 4.</param>
        /// 
        /// <param name="sigma1">The sigma for the first Gaussian. Default is 0.4.</param>
        /// <param name="sigma2">The sigma for the second Gaussian. Default is 0.4</param>
        /// 
        public DifferenceOfGaussians(int windowSize1, int windowSize2, double sigma1, double sigma2)
            : this(windowSize1, windowSize2)
        {
            this.first.Sigma = sigma1;
            this.second.Sigma = sigma2;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DifferenceOfGaussians"/> class.
        /// </summary>
        /// 
        /// <param name="windowSize1">The window size for the first Gaussian. Default is 3</param>
        /// <param name="windowSize2">The window size for the second Gaussian. Default is 4.</param>
        /// 
        /// <param name="sigma">The sigma for both Gaussian filters. Default is 0.4.</param>
        /// 
        public DifferenceOfGaussians(int windowSize1, int windowSize2, double sigma)
            : this(windowSize1, windowSize2, sigma, sigma)
        {
        }


        /// <summary>
        ///   Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        /// 
        protected override void ProcessFilter(UnmanagedImage image)
        {
            // Apply first Gaussian blur
            var image1 = first.Apply(image);

            // Apply second Gaussian blur
            second.ApplyInPlace(image);

            // Subtract the two images
            subtract.UnmanagedOverlayImage = image1;
            subtract.ApplyInPlace(image);
        }

    }
}
