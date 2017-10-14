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
    using Accord.MachineLearning;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    ///   Common interface for feature detectors (e.g. <see cref="SpeededUpRobustFeaturesDetector"/>,
    ///   <see cref="Haralick"/>, <see cref="HistogramsOfOrientedGradients"/>).
    /// </summary>
    /// 
    /// <typeparam name="TFeature">The type of the extracted features (e.g. <see cref="SpeededUpRobustFeaturePoint"/>, <see cref="FastRetinaKeypoint"/>]).</typeparam>
    /// 
    /// <seealso cref="SpeededUpRobustFeaturesDetector"/>
    /// <seealso cref="HistogramsOfOrientedGradients"/>
    /// <seealso cref="Haralick"/>
    /// <seealso cref="LocalBinaryPattern"/>
    /// 
    public interface IImageFeatureExtractor<
#if !NET35
        out 
#endif
        TFeature> :
        IFeatureExtractor<TFeature, Bitmap>,
        IFeatureExtractor<TFeature, UnmanagedImage>, ICloneable, IDisposable
    {
    }

    /// <summary>
    ///   Obsolete. See <see cref="IImageFeatureExtractor{TFeature}"/> instead.
    /// </summary>
    /// 
    [Obsolete("This class will be removed.")]
    public interface IFeatureDetector<
#if !NET35
        out 
#endif
        TPoint>
    {
        /// <summary>
        ///   Obsolete. Please use the <see cref="ICovariantTransform{TInput, TOutput}.Transform(TInput)"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Transform(image) method instead.")]
        IEnumerable<TPoint> ProcessImage(Bitmap image);

        /// <summary>
        ///   Obsolete. Please use the <see cref="ICovariantTransform{TInput, TOutput}.Transform(TInput)"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Transform(image) method instead.")]
        IEnumerable<TPoint> ProcessImage(BitmapData imageData);

        /// <summary>
        ///   Obsolete. Please use the <see cref="ICovariantTransform{TInput, TOutput}.Transform(TInput)"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Transform(image) method instead.")]
        IEnumerable<TPoint> ProcessImage(UnmanagedImage image);

    }

}
