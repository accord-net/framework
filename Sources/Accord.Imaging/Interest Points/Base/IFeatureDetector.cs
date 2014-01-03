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
    using AForge.Imaging;

    /// <summary>
    ///   Common interface for feature detectors.
    /// </summary>
    /// 
    public interface IFeatureDetector<TPoint> : IFeatureDetector<TPoint, double[]>
        where TPoint : IFeatureDescriptor<double[]>
    {
        // This class exists to maintain backward compatibility with
        // the non-generic version of IFeaturePoint (and to provide
        // a more intuitive way of handling standard, double valued
        // feature description vectors.
    }

    /// <summary>
    ///   Common interface for feature detectors.
    /// </summary>
    /// 
    public interface IFeatureDetector<TPoint, TFeature> 
        where TPoint : IFeatureDescriptor<TFeature>
    {
        /// <summary>
        ///   Process image looking for interest points.
        /// </summary>
        /// 
        /// <param name="image">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found interest points.</returns>
        /// 
        List<TPoint> ProcessImage(Bitmap image);

        /// <summary>
        ///   Process image looking for interest points.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found interest points.</returns>
        /// 
        List<TPoint> ProcessImage(BitmapData imageData);

        /// <summary>
        ///   Process image looking for interest points.
        /// </summary>
        /// 
        /// <param name="image">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found interest points.</returns>
        /// 
        List<TPoint> ProcessImage(UnmanagedImage image);
    }

}
