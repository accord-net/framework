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
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using IO;
    using Imaging;

    /// <summary>
    ///   Bag of Visual Words
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
    ///   the framework uses the <see cref="SpeededUpRobustFeaturesDetector">
    ///   SURF features detector</see> and the <see cref="KMeans"/> clustering
    ///   algorithm.</para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>  
    ///   The first example shows how to create and use a BoW with default parameters. </para>
    ///   
    ///   <code source="Unit Tests\Accord.Tests.Vision\Imaging\BagOfVisualWordsTest.cs" region="doc_learn" />
    /// 
    /// <para>  
    ///   After the representations have been extracted, it is possible to use them
    ///   in arbitrary machine learning tasks, such as classification:</para>
    ///   
    ///   <code source="Unit Tests\Accord.Tests.Vision\Imaging\BagOfVisualWordsTest.cs" region="doc_classification" />
    /// 
    /// 
    /// <para>  
    ///   By default, the BoW uses K-Means to cluster feature vectors. The next
    ///   example demonstrates how to use a different clustering algorithm when
    ///   computing the BoW, including the <see cref="BinarySplit">
    ///   Binary Split</see> algorithm.</para>
    ///   
    ///   <code source="Unit Tests\Accord.Tests.Vision\Imaging\BagOfVisualWordsTest.cs" region="doc_clustering" />
    ///   <code source="Unit Tests\Accord.Tests.Vision\Imaging\BagOfVisualWordsTest.cs" region="doc_classification_clustering" />
    ///   
    /// <para>
    ///   By default, the BoW uses the <see cref="SpeededUpRobustFeaturesDetector">
    ///   SURF feature detector</see> to extract sparse features from the images.
    ///   However, it is also possible to use other detectors, including dense
    ///   detectors such as <see cref="HistogramsOfOrientedGradients"/>.</para>
    ///   
    ///   <code source="Unit Tests\Accord.Tests.Vision\Imaging\BagOfVisualWordsTest.cs" region="doc_feature" />
    ///   <code source="Unit Tests\Accord.Tests.Vision\Imaging\BagOfVisualWordsTest.cs" region="doc_classification_feature" />
    ///   
    /// <para>
    ///   Or this also simple case using the <see cref="FastRetinaKeypointDetector">FREAK</see>
    ///   detector and the <see cref="BinarySplit"/> clustering algorithm:</para>
    ///   
    ///   <code source="Unit Tests\Accord.Tests.Vision\Imaging\BagOfVisualWordsTest.cs" region="doc_feature_freak" />
    ///   <code source="Unit Tests\Accord.Tests.Vision\Imaging\BagOfVisualWordsTest.cs" region="doc_classification_feature_freak" />
    ///   
    ///   
    /// <para>
    ///   More advanced use cases are also supported. For example, some image patches
    ///   can be represented using different data representations, such as byte vectors.
    ///   In this case, it is still possible to use the BoW using an appropriate clustering
    ///   algorithm that doesn't depend on Euclidean distances.</para>
    ///   
    ///   <code source="Unit Tests\Accord.Tests.Vision\Imaging\BagOfVisualWordsTest.cs" region="doc_datatype" />
    ///   <code source="Unit Tests\Accord.Tests.Vision\Imaging\BagOfVisualWordsTest.cs" region="doc_classification_datatype" />
    /// 
    /// <para>
    ///   Other more specialized feature extractors can also be used, such as <see cref="Haralick"/>
    ///   texture feature extractors for performing texture classification.</para>
    ///   
    ///   <code source="Unit Tests\Accord.Tests.Vision\Imaging\BagOfVisualWordsTest.cs" region="doc_feature_haralick" />
    ///   <code source="Unit Tests\Accord.Tests.Vision\Imaging\BagOfVisualWordsTest.cs" region="doc_classification_feature_haralick" />
    ///   
    /// <para>
    ///   In some applications, learning a BoW with the default settings might need a large amount of memory to be
    ///   available. In those cases, it is possible to reduce the memory and CPU requirements for the learning phase
    ///   using the <see cref="BaseBagOfWords{A,B,C,D,E,F}.NumberOfDescriptors"/> and 
    ///   <see cref="BaseBagOfWords{A,B,C,D,E,F}.MaxDescriptorsPerInstance"/> properties. It is also possible
    ///   to avoid loading all images at once by feeding the algorithm with the image filenames instead of their Bitmap
    ///   representations:</para>
    ///   
    ///   <code source="Unit Tests\Accord.Tests.Vision\Imaging\BagOfVisualWordsTest.cs" region="doc_learn_disk" />
    ///   <code source="Unit Tests\Accord.Tests.Vision\Imaging\BagOfVisualWordsTest.cs" region="doc_classification_disk" />
    /// </example>
    /// 
    /// <seealso cref="BagOfVisualWords{TPoint}"/>
    /// <seealso cref="BagOfVisualWords{TPoint, TFeature}"/>
    /// <seealso cref="BagOfVisualWords{TPoint, TFeature, TClustering, TExtractor}"/>
    /// 
    [Serializable]
    public class BagOfVisualWords :
            BaseBagOfVisualWords<BagOfVisualWords,
                SpeededUpRobustFeaturePoint, double[],
                IUnsupervisedLearning<IClassifier<double[], int>, double[], int>,
                SpeededUpRobustFeaturesDetector>
    {

        /// <summary>
        ///   Constructs a new <see cref="BagOfVisualWords"/> using a
        ///   <see cref="SpeededUpRobustFeaturesDetector">surf</see>
        ///   feature detector to identify features.
        /// </summary>
        /// 
        /// <param name="numberOfWords">The number of codewords.</param>
        /// 
        public BagOfVisualWords(int numberOfWords)
        {
            base.Init(new SpeededUpRobustFeaturesDetector(), BagOfWords.GetDefaultClusteringAlgorithm(numberOfWords));
        }

        /// <summary>
        ///   Constructs a new <see cref="BagOfVisualWords"/> using a
        ///   <see cref="SpeededUpRobustFeaturesDetector">surf</see>
        ///   feature detector to identify features.
        /// </summary>
        /// 
        /// <param name="algorithm">The clustering algorithm to use.</param>
        /// 
        public BagOfVisualWords(IUnsupervisedLearning<IClassifier<double[], int>, double[], int> algorithm)
        {
            base.Init(new SpeededUpRobustFeaturesDetector(), algorithm);
        }

        /// <summary>
        ///   Loads a bag of words from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the bow is to be deserialized.</param>
        /// 
        /// <returns>The deserialized bag of words.</returns>
        /// 
        [Obsolete("Please use the Accord.IO.Serializer<BagOfVisualWords>.Load(stream) method instead.")]
        public static BagOfVisualWords Load(Stream stream)
        {
            return Serializer.Load<BagOfVisualWords>(stream);
        }

        /// <summary>
        ///   Loads a bag of words from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the bow is to be deserialized.</param>
        /// 
        /// <returns>The deserialized bag of words.</returns>
        /// 
        [Obsolete("Please use the Accord.IO.Serializer<BagOfVisualWords>.Load(path) method instead.")]
        public static BagOfVisualWords Load(string path)
        {
            return Serializer.Load<BagOfVisualWords>(path);
        }

        /// <summary>
        ///   Loads a bag of words from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the bow is to be deserialized.</param>
        /// 
        /// <returns>The deserialized bag of words.</returns>
        /// 
        [Obsolete("Please use the Accord.IO.Serializer<BagOfVisualWords<TPoint>>.Load() method instead.")]
        public static BagOfVisualWords<TPoint> Load<TPoint>(Stream stream)
            where TPoint : IFeaturePoint<double[]>
        {
            return Serializer.Load<BagOfVisualWords<TPoint>>(stream);
        }

        /// <summary>
        ///   Loads a bag of words from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the bow is to be deserialized.</param>
        /// 
        /// <returns>The deserialized bag of words.</returns>
        /// 
        [Obsolete("Please use the Accord.IO.Serializer<BagOfVisualWords<TPoint>>.Load() method instead.")]
        public static BagOfVisualWords<TPoint> Load<TPoint>(string path)
            where TPoint : IFeaturePoint<double[]>
        {
            return Serializer.Load<BagOfVisualWords<TPoint>>(path);
        }

        /// <summary>
        ///   Loads a bag of words from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the bow is to be deserialized.</param>
        /// 
        /// <returns>The deserialized bag of words.</returns>
        /// 
        [Obsolete("Please use the Accord.IO.Serializer<BagOfVisualWords<TPoint, TFeature>>.Load() method instead.")]
        public static BagOfVisualWords<TPoint, TFeature> Load<TPoint, TFeature>(Stream stream)
            where TPoint : IFeaturePoint<TFeature>
        {
            return Serializer.Load<BagOfVisualWords<TPoint, TFeature>>(stream);
        }

        /// <summary>
        ///   Loads a bag of words from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the bow is to be deserialized.</param>
        /// 
        /// <returns>The deserialized bag of words.</returns>
        /// 
        [Obsolete("Please use the Accord.IO.Serializer<BagOfVisualWords<TPoint, TFeature>>.Load() method instead.")]
        public static BagOfVisualWords<TPoint, TFeature> Load<TPoint, TFeature>(string path)
            where TPoint : IFeaturePoint<TFeature>
        {
            return Serializer.Load<BagOfVisualWords<TPoint, TFeature>>(path);
        }

        /// <summary>
        /// Creates a Bag-of-Words model using <see cref="SpeededUpRobustFeaturesDetector"/> and <see cref="KMeans"/>
        /// </summary>
        /// 
        public static BagOfVisualWords Create(int numberOfWords)
        {
            return new BagOfVisualWords(numberOfWords);
        }

        /// <summary>
        /// Creates a Bag-of-Words model using the given feature detector and clustering algorithm.
        /// </summary>
        /// 
        public static BagOfVisualWords<FeatureDescriptor, double[], TClustering, IImageFeatureExtractor<FeatureDescriptor>>
            Create<TExtractor, TClustering>(IImageFeatureExtractor<FeatureDescriptor> detector, TClustering clustering)
            where TClustering: IUnsupervisedLearning<IClassifier<double[], int>, double[], int>
        {
            return new BagOfVisualWords<FeatureDescriptor, double[], TClustering, IImageFeatureExtractor<FeatureDescriptor>>(detector, clustering);
        }

       

        /// <summary>
        /// Creates a Bag-of-Words model using the <see cref="SpeededUpRobustFeaturesDetector">SURF feature detector</see> and the given clustering algorithm.
        /// </summary>
        /// 
        public static BagOfVisualWords<SpeededUpRobustFeaturePoint, double[], TClustering, SpeededUpRobustFeaturesDetector>
            Create<TClustering>(TClustering clustering)
            where TClustering : IUnsupervisedLearning<IClassifier<double[], int>, double[], int>
        {
            return Create<SpeededUpRobustFeaturesDetector, TClustering, SpeededUpRobustFeaturePoint, double[]>(new SpeededUpRobustFeaturesDetector(), clustering);
        }

#if NET35
        /// <summary>
        /// Creates a Bag-of-Words model using the given feature detector and clustering algorithm.
        /// </summary>
        /// 
        public static BagOfVisualWords<FastRetinaKeypoint, byte[], TClustering, FastRetinaKeypointDetector>
            Create<TExtractor, TClustering, TPoint>(FastRetinaKeypointDetector detector, TClustering clustering)
            where TClustering : IUnsupervisedLearning<IClassifier<byte[], int>, byte[], int>
            where TExtractor : FastRetinaKeypointDetector
        {
            return Create<FastRetinaKeypointDetector, TClustering, FastRetinaKeypoint, byte[]>(detector, clustering);
        }

        /// <summary>
        /// Creates a Bag-of-Words model using the given feature detector and clustering algorithm.
        /// </summary>
        /// 
        public static BagOfVisualWords<FastRetinaKeypoint, double[], TClustering, FastRetinaKeypointDetector>
            Create<TClustering>(FastRetinaKeypointDetector detector, TClustering clustering)
            where TClustering : IUnsupervisedLearning<IClassifier<double[], int>, double[], int>
        {
            return Create<FastRetinaKeypointDetector, TClustering, FastRetinaKeypoint, double[]>(detector, clustering);
        }

        /// <summary>
        /// Creates a Bag-of-Words model using the given feature detector and clustering algorithm.
        /// </summary>
        /// 
        public static BagOfVisualWords<FeatureDescriptor, double[], TClustering, TExtractor>
            Create<TExtractor, TClustering>(TExtractor detector, TClustering clustering)
            where TClustering : IUnsupervisedLearning<IClassifier<double[], int>, double[], int>
            where TExtractor : BaseFeatureExtractor<FeatureDescriptor>
        {
            return Create<TExtractor, TClustering, FeatureDescriptor, double[]>(detector, clustering);
        }
#else
        /// <summary>
        /// Creates a Bag-of-Words model using the given feature detector and clustering algorithm.
        /// </summary>
        /// 
        public static BagOfVisualWords<IFeatureDescriptor<double[]>, double[], TClustering, TExtractor>
            Create<TExtractor, TClustering>(TExtractor detector, TClustering clustering)
            where TClustering : IUnsupervisedLearning<IClassifier<double[], int>, double[], int>
            where TExtractor : IImageFeatureExtractor<IFeatureDescriptor<double[]>>
        {
            return Create<TExtractor, TClustering, IFeatureDescriptor<double[]>, double[]>(detector, clustering);
        }
#endif


        /// <summary>
        /// Creates a Bag-of-Words model using the given feature detector and <see cref="KMeans"/>.
        /// </summary>
        /// 
        public static BagOfVisualWords<IFeatureDescriptor<double[]>, double[], KMeans, TExtractor>
            Create<TExtractor>(TExtractor detector, int numberOfWords)
            where TExtractor : IImageFeatureExtractor<IFeatureDescriptor<double[]>>
        {
            return Create<TExtractor, KMeans, IFeatureDescriptor<double[]>, double[]>(detector, BagOfWords.GetDefaultClusteringAlgorithm(numberOfWords));
        }

        /// <summary>
        /// Creates a Bag-of-Words model using the given feature detector and clustering algorithm.
        /// </summary>
        /// 
        public static BagOfVisualWords<IFeatureDescriptor<TFeature>, TFeature, TClustering, TExtractor>
            Create<TExtractor, TClustering, TFeature>(TExtractor detector, TClustering clustering)
            where TClustering : IUnsupervisedLearning<IClassifier<TFeature, int>, TFeature, int> 
            where TExtractor : IImageFeatureExtractor<IFeatureDescriptor<TFeature>>
        {
            return Create<TExtractor, TClustering, IFeatureDescriptor<TFeature>, TFeature>(detector, clustering);
        }

        /// <summary>
        /// Creates a Bag-of-Words model using the given feature detector and clustering algorithm.
        /// </summary>
        /// 
        public static BagOfVisualWords<TPoint, TFeature, TClustering, TExtractor>
            Create<TExtractor, TClustering, TPoint, TFeature>(TExtractor detector, TClustering clustering)
            where TPoint : IFeatureDescriptor<TFeature>
            where TClustering : IUnsupervisedLearning<IClassifier<TFeature, int>, TFeature, int> 
            where TExtractor : IImageFeatureExtractor<TPoint>
        {
            return new BagOfVisualWords<TPoint, TFeature, TClustering, TExtractor>(detector, clustering);
        }
    }

}
