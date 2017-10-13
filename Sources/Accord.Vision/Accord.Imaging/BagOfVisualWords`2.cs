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
    using Accord.Compat;
    using System.Threading.Tasks;

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
    /// 
    /// <remarks>
    /// <para>
    ///   The bag-of-words (BoW) model can be used to extract finite
    ///   length features from otherwise varying length representations.
    ///   This class can uses any <see cref="IImageFeatureExtractor{TPoint}">feature
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
    public class BagOfVisualWords<TPoint, TFeature> : 
        BaseBagOfVisualWords<BagOfVisualWords<TPoint, TFeature>,
            TPoint, TFeature,
            IUnsupervisedLearning<IClassifier<TFeature, int>, TFeature, int>,
            IImageFeatureExtractor<TPoint>>
        where TPoint : IFeatureDescriptor<TFeature>
    {

        /// <summary>
        ///   Constructs a new <see cref="BagOfVisualWords"/>.
        /// </summary>
        /// 
        /// <param name="extractor">The feature extractor to use.</param>
        /// <param name="algorithm">The clustering algorithm to use.</param>
        /// 
        public BagOfVisualWords(IImageFeatureExtractor<TPoint> extractor, 
            IUnsupervisedLearning<IClassifier<TFeature, int>, TFeature, int> algorithm)
        {
            Init(extractor, algorithm);
        }

        /// <summary>
        ///   Constructs a new <see cref="BagOfVisualWords"/>.
        /// </summary>
        /// 
        protected BagOfVisualWords()
        {

        }

    }
}
