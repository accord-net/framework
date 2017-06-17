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
    using Accord.Math;
    using Accord.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.Concurrent;

    /// <summary>
    ///   Bag of Visual Words
    /// </summary>
    /// 
    /// <typeparam name="TPoint">
    ///   The <see cref="IFeaturePoint{TFeature}"/> type to be used with this class,
    ///   such as <see cref="SpeededUpRobustFeaturePoint"/>.</typeparam>
    /// <typeparam name="TFeature">
    ///   The feature type of the <typeparamref name="TPoint"/>, such
    ///   as <see cref="T:double[]"/>.
    /// </typeparam>
    /// <typeparam name="TClustering">
    ///   The type of the clustering algorithm to be used to cluster the visual features
    ///   and form visual codewords.
    /// </typeparam>
    /// <typeparam name="TDetector">
    ///   The type of the feature detector used to extract features from the images.
    /// </typeparam>
    /// 
    /// <remarks>
    /// <para>
    ///   The bag-of-words (BoW) model can be used to extract finite
    ///   length features from otherwise varying length representations.
    ///   This class can uses any <see cref="IFeatureDetector{TPoint}">feature
    ///   detector</see> to determine a coded representation for a given image.</para>
    ///   
    /// <para>
    ///   For a simpler, non-generic version of the Bag-of-Words model which 
    ///   defaults to the <see cref="SpeededUpRobustFeaturesDetector">SURF 
    ///   features detector</see>, please see <see cref="BagOfVisualWords"/>.
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   Please see <see cref="BagOfVisualWords"/>.</para>
    /// </example>
    /// 
    /// <seealso cref="BagOfVisualWords"/>
    /// 
    [Serializable]
    public class BagOfVisualWords<TPoint, TFeature, TClustering, TDetector> :
        BaseBagOfVisualWords<BagOfVisualWords<TPoint, TFeature, TClustering, TDetector>, 
            TPoint, TFeature, TClustering, TDetector>
        where TPoint : IFeatureDescriptor<TFeature>
        where TClustering : IUnsupervisedLearning<IClassifier<TFeature, int>, TFeature, int>
        where TDetector : IFeatureDetector<TPoint, TFeature>
    {
        /// <summary>
        ///   Constructs a new <see cref="BagOfVisualWords"/>.
        /// </summary>
        /// 
        /// <param name="detector">The feature detector to use.</param>
        /// <param name="algorithm">The clustering algorithm to use.</param>
        /// 
        public BagOfVisualWords(TDetector detector, TClustering algorithm)
        {
            Init(detector, algorithm);
        }

    }
}
