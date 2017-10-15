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
    using Accord.Audio;
    using Accord.Compat;

    /// <summary>
    ///   Bag of Audio Words
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The bag-of-words (BoW) model can be used to transform data with
    ///   multiple possible lengths (i.e. words in a text, pixels in an 
    ///   image) into finite-dimensional vectors of fixed length. Those 
    ///   vectors are usually referred as representations as they can be
    ///   used in place of the original data as if they were the data itself.
    ///   For example, using Bag-of-Words it becomes possible to transform
    ///   a set of <c>N</c> images with varying sizes and dimensions into a 
    ///   <c>N x C</c> matrix where <c>C</c> is the number of "visual words"
    ///   being used to represent each of the <c>N</c> images in the set.</para>
    ///   
    /// <para>
    ///   Those rows can then be used in classification, clustering, and any
    ///   other machine learning tasks where a finite vector representation
    ///   would be required.</para>
    ///   
    /// <para>
    ///   The framework can compute BoW representations for images using any
    ///   choice of feature extractor and clustering algorithm. By default,
    ///   the framework uses the <see cref="MelFrequencyCepstrumCoefficient">
    ///   MFCC features extractor</see> and the <see cref="KMeans"/> clustering
    ///   algorithm.</para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>  
    ///   The first example shows how to create and use a BoW with default parameters. </para>
    ///   
    ///   <code source="Unit Tests\Accord.Tests.Audio\BagOfAudioWordsTest.cs" region="doc_learn" />
    /// 
    /// <para>  
    ///   After the representations have been extracted, it is possible to use them
    ///   in arbitrary machine learning tasks, such as classification:</para>
    ///   
    ///   <code source="Unit Tests\Accord.Tests.Audio\BagOfAudioWordsTest.cs" region="doc_classification" />
    /// </example>
    /// 
    /// <seealso cref="BagOfAudioWords{TPoint, TFeature}"/>
    /// <seealso cref="BagOfAudioWords{TPoint, TFeature, TClustering, TExtractor}"/>
    /// 
    [Serializable]
    public class BagOfAudioWords :
            BaseBagOfAudioWords<BagOfAudioWords,
                MelFrequencyCepstrumCoefficientDescriptor, double[],
                IUnsupervisedLearning<IClassifier<double[], int>, double[], int>,
                MelFrequencyCepstrumCoefficient>
    {

        /// <summary>
        ///   Constructs a new <see cref="BagOfAudioWords"/> using a
        ///   <see cref="MelFrequencyCepstrumCoefficient">MFCC</see>
        ///   feature detector to identify features.
        /// </summary>
        /// 
        /// <param name="numberOfWords">The number of codewords.</param>
        /// 
        public BagOfAudioWords(int numberOfWords)
        {
            base.Init(new MelFrequencyCepstrumCoefficient(), BagOfWords.GetDefaultClusteringAlgorithm(numberOfWords));
        }

        /// <summary>
        ///   Constructs a new <see cref="BagOfAudioWords"/> using a
        ///   <see cref="MelFrequencyCepstrumCoefficient">MFCC</see>
        ///   feature detector to identify features.
        /// </summary>
        /// 
        /// <param name="algorithm">The clustering algorithm to use.</param>
        /// 
        public BagOfAudioWords(
            IUnsupervisedLearning<IClassifier<double[], int>, double[], int> algorithm)
        {
            base.Init(new MelFrequencyCepstrumCoefficient(), algorithm);
        }

        /// <summary>
        /// Creates a Bag-of-Words model using MFCC and K-Means.
        /// </summary>
        /// 
        public static BagOfAudioWords Create(int numberOfWords)
        {
            return new BagOfAudioWords(numberOfWords);
        }

        /// <summary>
        /// Creates a Bag-of-Words model using the given feature detector and clustering algorithm.
        /// </summary>
        /// 
        public static BagOfAudioWords<IFeatureDescriptor<double[]>, double[], TClustering, TExtractor>
            Create<TExtractor, TClustering>(TExtractor extractor, TClustering clustering)
            where TClustering : IUnsupervisedLearning<IClassifier<double[], int>, double[], int> 
            where TExtractor : IAudioFeatureExtractor<IFeatureDescriptor<double[]>>
        {
            return Create<TExtractor, TClustering, IFeatureDescriptor<double[]>, double[]>(extractor, clustering);
        }

        /// <summary>
        /// Creates a Bag-of-Words model using the given feature detector and K-Means.
        /// </summary>
        /// 
        public static BagOfAudioWords<IFeatureDescriptor<double[]>, double[], KMeans, TExtractor>
            Create<TExtractor>(TExtractor extractor, int numberOfWords)
            where TExtractor : IAudioFeatureExtractor<IFeatureDescriptor<double[]>>
        {
            return Create<TExtractor, KMeans, IFeatureDescriptor<double[]>, double[]>(extractor, BagOfWords.GetDefaultClusteringAlgorithm(numberOfWords));
        }

        /// <summary>
        /// Creates a Bag-of-Words model using the given feature detector and K-Means.
        /// </summary>
        /// 
        public static BagOfAudioWords<IFeatureDescriptor<double[]>, double[], KMeans, TExtractor>
            Create<TExtractor, TClustering>(TExtractor extractor, int numberOfWords)
            where TExtractor : IAudioFeatureExtractor<IFeatureDescriptor<double[]>>
        {
            return Create<TExtractor, KMeans, IFeatureDescriptor<double[]>, double[]>(extractor, BagOfWords.GetDefaultClusteringAlgorithm(numberOfWords));
        }

        /// <summary>
        /// Creates a Bag-of-Words model using the MFCC feature extractor and the given clustering algorithm.
        /// </summary>
        /// 
        public static BagOfAudioWords<MelFrequencyCepstrumCoefficientDescriptor, double[], TClustering, MelFrequencyCepstrumCoefficient>
            Create<TClustering>(TClustering clustering)
            where TClustering : IUnsupervisedLearning<IClassifier<double[], int>, double[], int>
        {
            return Create<MelFrequencyCepstrumCoefficient, TClustering, MelFrequencyCepstrumCoefficientDescriptor, double[]>(new MelFrequencyCepstrumCoefficient(), clustering);
        }


        /// <summary>
        /// Creates a Bag-of-Words model using the given feature detector and clustering algorithm.
        /// </summary>
        /// 
        public static BagOfAudioWords<IFeatureDescriptor<TFeature>, TFeature, TClustering, TExtractor>
            Create<TExtractor, TClustering, TFeature>(TExtractor extractor, TClustering clustering)
            where TClustering : IUnsupervisedLearning<IClassifier<TFeature, int>, TFeature, int> 
            where TExtractor : IAudioFeatureExtractor<IFeatureDescriptor<TFeature>>
        {
            return Create<TExtractor, TClustering, IFeatureDescriptor<TFeature>, TFeature>(extractor, clustering);
        }

        /// <summary>
        /// Creates a Bag-of-Words model using the given feature detector and clustering algorithm.
        /// </summary>
        /// 
        public static BagOfAudioWords<TPoint, TFeature, TClustering, TExtractor>
            Create<TExtractor, TClustering, TPoint, TFeature>(TExtractor extractor, TClustering clustering)
            where TPoint : IFeatureDescriptor<TFeature>
            where TClustering : IUnsupervisedLearning<IClassifier<TFeature, int>, TFeature, int> 
            where TExtractor : IAudioFeatureExtractor<TPoint>
        {
            return new BagOfAudioWords<TPoint, TFeature, TClustering, TExtractor>(extractor, clustering);
        }
    }

}
