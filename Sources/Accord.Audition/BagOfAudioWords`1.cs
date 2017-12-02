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
    using Accord.MachineLearning;
    using System;
    using Accord.Compat;
    using Accord.Audio;
    using Accord.Audition;

    /// <summary>
    ///   Bag of Audio Words
    /// </summary>
    /// 
    /// <typeparam name="TFeature">
    ///   The <see cref="IFeatureDescriptor{T}"/> type to be used with this class,
    ///   such as <see cref="MelFrequencyCepstrumCoefficientDescriptor"/>.</typeparam>
    /// 
    /// <remarks>
    /// <para>
    ///   The bag-of-words (BoW) model can be used to extract finite
    ///   length features from otherwise varying length representations.
    ///   This class can uses any <see cref="IAudioFeatureExtractor{T}">feature
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
    public class BagOfAudioWords<TFeature> :
        BaseBagOfAudioWords<BagOfAudioWords<TFeature>,
            TFeature, double[],
            IUnsupervisedLearning<IClassifier<double[], int>, double[], int>,
            IAudioFeatureExtractor<TFeature>>
        where TFeature : IFeatureDescriptor<double[]>
    {
        /// <summary>
        ///   Constructs a new <see cref="BagOfAudioWords"/>.
        /// </summary>
        /// 
        /// <param name="extractor">The feature extractor to use.</param>
        /// <param name="numberOfWords">The number of codewords.</param>
        /// 
        public BagOfAudioWords(IAudioFeatureExtractor<TFeature> extractor, int numberOfWords)
        {
            base.Init(extractor, BagOfWords.GetDefaultClusteringAlgorithm(numberOfWords));
        }

        /// <summary>
        ///   Constructs a new <see cref="BagOfAudioWords"/>.
        /// </summary>
        /// 
        /// <param name="extractor">The feature extractor to use.</param>
        /// <param name="algorithm">The clustering algorithm to use.</param>
        /// 
        public BagOfAudioWords(IAudioFeatureExtractor<TFeature> extractor, 
            IUnsupervisedLearning<IClassifier<double[], int>, double[], int> algorithm)
        {
            base.Init(extractor, algorithm);
        }

    }

}
