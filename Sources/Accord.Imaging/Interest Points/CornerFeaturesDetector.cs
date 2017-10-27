// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
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
    using Accord.Imaging;
    using System;
    using System.Linq;
    using Accord.Compat;

    /// <summary>
    ///   Feature detector based on corners.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class can be used as an adapter for classes implementing
    ///   AForge.NET's ICornersDetector interface, so they can be used
    ///   where an <see cref="IImageFeatureExtractor{T}"/> is needed.
    /// </remarks>
    /// 
    /// <example>
    ///   For an example on how to use this class, please take a look
    ///   on the example section for <c>BagOfVisualWords{T}</c>.
    /// </example>
    /// 
    public class CornerFeaturesDetector : BaseSparseFeatureExtractor<CornerFeaturePoint>
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
            this.SupportedFormats.UnionWith(detector.SupportedFormats);
        }

        /// <summary>
        /// This method should be implemented by inheriting classes to implement the
        /// actual corners detection, transforming the input image into a list of points.
        /// </summary>
        /// 
        protected override IEnumerable<CornerFeaturePoint> InnerTransform(UnmanagedImage input)
        {
            return Detector.ProcessImage(input).Select(x => new CornerFeaturePoint(x)).ToList();
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        protected override object Clone(ISet<PixelFormat> supportedFormats)
        {
            return new CornerFeaturesDetector((ICornersDetector)this.Detector.Clone())
            {
                SupportedFormats = supportedFormats
            };
        }

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// 
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged
        ///   resources; <c>false</c> to release only unmanaged resources.</param>
        /// 
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var d = Detector as IDisposable;
                if (d != null)
                    d.Dispose();
            }

            // free native resources if there are any.
            Detector = null;
        }

    }
}
