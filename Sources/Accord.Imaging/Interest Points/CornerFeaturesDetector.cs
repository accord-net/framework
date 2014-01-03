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
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge;
    using AForge.Imaging;

    /// <summary>
    ///   Feature detector based on corners.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class can be used as an adapter for classes implementing
    ///   AForge.NET's ICornersDetector interface, so they can be used
    ///   where an <see cref="IFeatureDetector{T}"/> is needed.
    /// </remarks>
    /// 
    /// <example>
    ///   For an example on how to use this class, please take a look
    ///   on the example section for <see cref="BagOfVisualWords{T}"/>.
    /// </example>
    /// 
    /// <seealso cref="BagOfVisualWords{T}"/>
    /// 
    public class CornerFeaturesDetector : IFeatureDetector<CornerFeaturePoint>
    {

        /// <summary>
        ///   Gets the corners detector used to generate features.
        /// </summary>
        /// 
        public ICornersDetector Detector { get; private set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="CornerFeaturesDetector"/> class.
        /// </summary>
        /// 
        /// <param name="detector">A corners detector.</param>
        /// 
        public CornerFeaturesDetector(ICornersDetector detector)
        {
            this.Detector = detector;
        }

        /// <summary>
        ///   Process image looking for interest points.
        /// </summary>
        /// 
        /// <param name="image">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found interest points.</returns>
        /// 
        public List<CornerFeaturePoint> ProcessImage(Bitmap image)
        {
            List<IntPoint> corners = Detector.ProcessImage(image);
            return corners.ConvertAll(convert);
        }

        /// <summary>
        ///   Process image looking for interest points.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found interest points.</returns>
        /// 
        public List<CornerFeaturePoint> ProcessImage(BitmapData imageData)
        {
            List<IntPoint> corners = Detector.ProcessImage(imageData);
            return corners.ConvertAll(convert);
        }

        /// <summary>
        ///   Process image looking for interest points.
        /// </summary>
        /// 
        /// <param name="image">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found interest points.</returns>
        /// 
        public List<CornerFeaturePoint> ProcessImage(UnmanagedImage image)
        {
            List<IntPoint> corners = Detector.ProcessImage(image);
            return corners.ConvertAll(convert);
        }


        private CornerFeaturePoint convert(IntPoint point)
        {
            return new CornerFeaturePoint(point);
        }
    }
}
