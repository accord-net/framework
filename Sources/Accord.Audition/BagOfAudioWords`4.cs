// Accord Audio Library
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

namespace Accord.Audition
{
    using Accord.Audio;
    using Accord.MachineLearning;
    using System;
    using Accord.Compat;

    /// <summary>
    ///   Bag of Audio Words
    /// </summary>
    /// 
    /// <typeparam name="TFeature">
    ///   The <see cref="IFeatureDescriptor{TPoint}"/> type to be used with this class,
    ///   such as <see cref="MelFrequencyCepstrumCoefficientDescriptor"/>.</typeparam>
    /// <typeparam name="TPoint">
    ///   The feature type of the <typeparamref name="TFeature"/>, such
    ///   as <see cref="T:double[]"/>.
    /// </typeparam>
    /// <typeparam name="TClustering">
    ///   The type of the clustering algorithm to be used to cluster the visual features
    ///   and form visual codewords.
    /// </typeparam>
    /// <typeparam name="TExtractor">
    ///   The type of the feature detector used to extract features from the images.
    /// </typeparam>
    /// 
    /// <remarks>
    /// <para>
    ///   The bag-of-words (BoW) model can be used to extract finite
    ///   length features from otherwise varying length representations.
    ///   This class can uses any <see cref="IAudioFeatureExtractor{TPoint}">feature
    ///   detector</see> to determine a coded representation for a given <see cref="Signal"/>.</para>
    ///   
    /// <para>
    ///   For a simpler, non-generic version of the Bag-of-Words model which 
    ///   defaults to the <see cref="MelFrequencyCepstrumCoefficientDescriptor">MFCC
    ///   feature extractor</see>, please see <see cref="BagOfAudioWords"/>.
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   Please see <see cref="BagOfAudioWords"/>.</para>
    /// </example>
    /// 
    /// <seealso cref="BagOfAudioWords"/>
    /// 
    [Serializable]
    public class BagOfAudioWords<TFeature, TPoint, TClustering, TExtractor> :
        BaseBagOfAudioWords<BagOfAudioWords<TFeature, TPoint, TClustering, TExtractor>,
            TFeature, TPoint, TClustering, TExtractor>
        where TFeature : IFeatureDescriptor<TPoint>
        where TClustering : IUnsupervisedLearning<IClassifier<TPoint, int>, TPoint, int>
        where TExtractor : IAudioFeatureExtractor<TFeature>
    {
        /// <summary>
        ///   Constructs a new <see cref="BagOfAudioWords"/>.
        /// </summary>
        /// 
        /// <param name="extractor">The feature extractor to use.</param>
        /// <param name="algorithm">The clustering algorithm to use.</param>
        /// 
        public BagOfAudioWords(TExtractor extractor, TClustering algorithm)
        {
            Init(extractor, algorithm);
        }

    }
}
