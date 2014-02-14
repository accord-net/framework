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
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Accord.MachineLearning;

    /// <summary>
    ///   Bag of Visual Words
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The bag-of-words (BoW) model can be used to extract finite
    ///   length features from otherwise varying length representations.
    ///   This class uses the <see cref="SpeededUpRobustFeaturesDetector">
    ///   SURF features detector</see> to determine a coded representation
    ///   for a given image.</para>
    ///   
    /// <para>
    ///   It is also possible to use other feature detectors with this
    ///   class. For this, please refer to <see cref="BagOfVisualWords{TPoint}"/>
    ///   for more details and examples.</para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>  
    ///   The following example shows how to create and use a BoW with
    ///   default parameters. </para>
    ///   
    /// <code>
    ///   int numberOfWords = 32;
    ///   
    ///   // Create bag-of-words (BoW) with the given number of words
    ///   BagOfVisualWords bow = new BagOfVisualWords(numberOfWords);
    ///   
    ///   // Create the BoW codebook using a set of training images
    ///   bow.Compute(imageArray);
    ///   
    ///   // Create a fixed-length feature vector for a new image
    ///   double[] featureVector = bow.GetFeatureVector(image);
    /// </code>
    /// 
    /// <para>  
    ///   By default, the BoW uses K-Means to cluster feature vectors. The next
    ///   example demonstrates how to use a different clustering algorithm when
    ///   computing the BoW. The example will be given using the <see cref="BinarySplit">
    ///   Binary Split</see> clustering algorithm.</para>
    ///   
    /// <code>
    ///   int numberOfWords = 32;
    ///   
    ///   // Create an alternative clustering algorithm
    ///   BinarySplit binarySplit = new BinarySplit(numberOfWords);
    ///   
    ///   // Create bag-of-words (BoW) with the clustering algorithm
    ///   BagOfVisualWords bow = new BagOfVisualWords(binarySplit);
    ///   
    ///   // Create the BoW codebook using a set of training images
    ///   bow.Compute(imageArray);
    ///   
    ///   // Create a fixed-length feature vector for a new image
    ///   double[] featureVector = bow.GetFeatureVector(image);
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="BagOfVisualWords{TPoint}"/>
    /// 
    [Serializable]
    public class BagOfVisualWords : BagOfVisualWords<SpeededUpRobustFeaturePoint>
    {
        /// <summary>
        ///   Gets the <see cref="SpeededUpRobustFeaturesDetector">SURF</see>
        ///   feature point detector used to identify visual features in images.
        /// </summary>
        /// 
        public new SpeededUpRobustFeaturesDetector Detector
        {
            get { return base.Detector as SpeededUpRobustFeaturesDetector; }
        }

        /// <summary>
        ///   Constructs a new <see cref="BagOfVisualWords"/> using a
        ///   <see cref="SpeededUpRobustFeaturesDetector">surf</see>
        ///   feature detector to identify features.
        /// </summary>
        /// 
        /// <param name="numberOfWords">The number of codewords.</param>
        /// 
        public BagOfVisualWords(int numberOfWords)
            : base(new SpeededUpRobustFeaturesDetector(), numberOfWords)
        {
        }

        /// <summary>
        ///   Constructs a new <see cref="BagOfVisualWords"/> using a
        ///   <see cref="SpeededUpRobustFeaturesDetector">surf</see>
        ///   feature detector to identify features.
        /// </summary>
        /// 
        /// <param name="algorithm">The clustering algorithm to use.</param>
        /// 
        public BagOfVisualWords(IClusteringAlgorithm<double[]> algorithm)
            : base(new SpeededUpRobustFeaturesDetector(), algorithm) 
        {
        }


        /// <summary>
        ///   Loads a bag of words from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the bow is to be deserialized.</param>
        /// 
        /// <returns>The deserialized bag of words.</returns>
        /// 
        public static BagOfVisualWords Load(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            return (BagOfVisualWords)b.Deserialize(stream);
        }

        /// <summary>
        ///   Loads a bag of words from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the bow is to be deserialized.</param>
        /// 
        /// <returns>The deserialized bag of words.</returns>
        /// 
        public static BagOfVisualWords Load(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                return Load(fs);
            }
        }

        /// <summary>
        ///   Loads a bag of words from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the bow is to be deserialized.</param>
        /// 
        /// <returns>The deserialized bag of words.</returns>
        /// 
        public static BagOfVisualWords<TPoint> Load<TPoint>(Stream stream)
            where TPoint : IFeaturePoint
        {
            BinaryFormatter b = new BinaryFormatter();
            return (BagOfVisualWords<TPoint>)b.Deserialize(stream);
        }

        /// <summary>
        ///   Loads a bag of words from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the bow is to be deserialized.</param>
        /// 
        /// <returns>The deserialized bag of words.</returns>
        /// 
        public static BagOfVisualWords<TPoint> Load<TPoint>(string path)
            where TPoint : IFeaturePoint
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                return Load<TPoint>(fs);
            }
        }

        /// <summary>
        ///   Loads a bag of words from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the bow is to be deserialized.</param>
        /// 
        /// <returns>The deserialized bag of words.</returns>
        /// 
        public static BagOfVisualWords<TPoint, TFeature> Load<TPoint, TFeature>(Stream stream)
            where TPoint : IFeaturePoint<TFeature>
        {
            BinaryFormatter b = new BinaryFormatter();
            return (BagOfVisualWords<TPoint, TFeature>)b.Deserialize(stream);
        }

        /// <summary>
        ///   Loads a bag of words from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the bow is to be deserialized.</param>
        /// 
        /// <returns>The deserialized bag of words.</returns>
        /// 
        public static BagOfVisualWords<TPoint, TFeature> Load<TPoint, TFeature>(string path)
            where TPoint : IFeaturePoint<TFeature>
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                return Load<TPoint, TFeature>(fs);
            }
        }
    }

}
