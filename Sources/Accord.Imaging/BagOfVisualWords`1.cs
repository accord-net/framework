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
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Threading.Tasks;
    using Accord.MachineLearning;
    using Accord.Math;
    using AForge.Imaging;

    /// <summary>
    ///   Bag of Visual Words
    /// </summary>
    /// 
    /// <typeparam name="TPoint">
    ///   The <see cref="IFeaturePoint"/> type to be used with this class,
    ///   such as <see cref="SpeededUpRobustFeaturePoint"/>.</typeparam>
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
    ///   features detector</see>, please see <see cref="BagOfVisualWords"/>
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example shows how to use a BoW model with the
    ///   <see cref="SpeededUpRobustFeaturesDetector"/>.</para>
    ///   
    /// <code>
    ///   int numberOfWords = 32;
    ///   
    ///   // Create bag-of-words (BoW) with the given SURF detector
    ///   var bow = new BagOfVisualWords&lt;SpeededUpRobustFeaturePoint>(
    ///      new SpeededUpRobustFeaturesDetector(), numberOfWords);
    ///   
    ///   // Create the BoW codebook using a set of training images
    ///   bow.Compute(imageArray);
    ///   
    ///   // Create a fixed-length feature vector for a new image
    ///   double[] featureVector = bow.GetFeatureVector(image);
    /// </code>
    /// 
    /// <para>
    ///   The following example shows how to create a BoW which works with any
    ///   of corner detector, such as <see cref="FastCornersDetector"/>:</para>
    ///   
    /// <code>
    ///   int numberOfWords = 16;
    /// 
    ///   // Create a corners detector
    ///   MoravecCornersDetector moravec = new MoravecCornersDetector();
    ///   
    ///   // Create an adapter to convert corners to visual features
    ///   CornerFeaturesDetector detector = new CornerFeaturesDetector(moravec);
    ///   
    ///   // Create a bag-of-words (BoW) with the corners detector and number of words
    ///   var bow = new BagOfVisualWords&lt;CornerFeaturePoint>(detector, numberOfWords);
    ///   
    ///   // Create the BoW codebook using a set of training images
    ///   bow.Compute(imageArray);
    ///   
    ///   // Create a fixed-length feature vector for a new image
    ///   double[] featureVector = bow.GetFeatureVector(image);
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="BagOfVisualWords"/>
    /// <seealso cref="IFeatureDetector{TPoint}"/>
    /// 
    /// <seealso cref="SpeededUpRobustFeaturesDetector"/>
    /// <seealso cref="FastRetinaKeypointDetector"/>
    /// 
    [Serializable]
    public class BagOfVisualWords<TPoint> : BagOfVisualWords<TPoint, double[]>
        where TPoint : IFeatureDescriptor<double[]>
    {
        /// <summary>
        ///   Constructs a new <see cref="BagOfVisualWords"/>.
        /// </summary>
        /// 
        /// <param name="detector">The feature detector to use.</param>
        /// <param name="numberOfWords">The number of codewords.</param>
        /// 
        public BagOfVisualWords(IFeatureDetector<TPoint> detector, int numberOfWords)
            : base(detector, new KMeans(numberOfWords))
        {
        }

        /// <summary>
        ///   Constructs a new <see cref="BagOfVisualWords"/>.
        /// </summary>
        /// 
        /// <param name="detector">The feature detector to use.</param>
        /// <param name="algorithm">The clustering algorithm to use.</param>
        /// 
        public BagOfVisualWords(IFeatureDetector<TPoint> detector, IClusteringAlgorithm<double[]> algorithm)
            : base(detector, algorithm)
        {
        }
    }

}
