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
    ///   This class can uses any <see cref="IFeatureDetector{TPoint}">feature
    ///   detector</see> to determine a coded representation for a given image.</para>
    ///   
    /// <para>
    ///   This is the most generic version for the BoW model, which can accept any
    ///   choice of <see cref="IFeatureDetector{TPoint}"/> for any kind of point,
    ///   even non-numeric ones. This class can also support any clustering algorithm
    ///   as well. </para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   In this example, we will create a Bag-of-Words to operate on <c>byte[]</c> vectors,
    ///   which otherwise wouldn't be supported by the simpler BoW version. Those byte vectors
    ///   are composed of binary features detected by a <see cref="FastRetinaKeypointDetector"/>.
    ///   In order to cluster those features, we will be using a <see cref="KModes{TData}"/>
    ///   algorithm with a matching template argument to make all constructors happy: </para>
    /// <code>
    ///   // Create a new FAST Corners Detector
    ///   FastCornersDetector fast = new FastCornersDetector();
    ///   
    ///   // Create a Fast Retina Keypoint (FREAK) detector using FAST
    ///   FastRetinaKeypointDetector freak = new FastRetinaKeypointDetector(fast);
    ///
    ///   // Create a K-Modes clustering algorithm which can operate on <c>byte[]</c>
    ///   var kmodes = new KModes&lt;byte[]>(numberOfWords, Distance.BitwiseHamming);
    ///
    ///   // Finally, create bag-of-words (BoW) with the given number of words
    ///   var bow = new BagOfVisualWords&lt;FastRetinaKeypoint, byte[]>(freak, kmodes);
    ///   
    ///   // Create the BoW codebook using a set of training images
    ///   bow.Compute(images);
    ///   
    ///   // Create a fixed-length feature vector for a new image
    ///   double[] featureVector = bow.GetFeatureVector(image);
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class BagOfVisualWords<TPoint, TFeature> : IBagOfWords<Bitmap>, IBagOfWords<UnmanagedImage>
        where TPoint : IFeatureDescriptor<TFeature>
    {

        /// <summary>
        ///   Gets the number of words in this codebook.
        /// </summary>
        /// 
        public int NumberOfWords { get; private set; }

        /// <summary>
        ///   Gets the clustering algorithm used to create this model.
        /// </summary>
        /// 
        public IClusteringAlgorithm<TFeature> Clustering { get; private set; }

        /// <summary>
        ///   Gets the <see cref="SpeededUpRobustFeaturesDetector">SURF</see>
        ///   feature point detector used to identify visual features in images.
        /// </summary>
        /// 
        public IFeatureDetector<TPoint, TFeature> Detector { get; private set; }


        /// <summary>
        ///   Constructs a new <see cref="BagOfVisualWords"/>.
        /// </summary>
        /// 
        /// <param name="detector">The feature detector to use.</param>
        /// <param name="algorithm">The clustering algorithm to use.</param>
        /// 
        public BagOfVisualWords(IFeatureDetector<TPoint, TFeature> detector, IClusteringAlgorithm<TFeature> algorithm)
        {
            this.NumberOfWords = algorithm.Clusters.Count;
            this.Clustering = algorithm;
            this.Detector = detector;
        }

        /// <summary>
        ///   Computes the Bag of Words model.
        /// </summary>
        /// 
        /// <param name="images">The set of images to initialize the model.</param>
        /// <param name="threshold">Convergence rate for the k-means algorithm. Default is 1e-5.</param>
        /// 
        /// <returns>The list of feature points detected in all images.</returns>
        /// 
        public List<TPoint>[] Compute(Bitmap[] images, double threshold = 1e-5)
        {

            List<TFeature> descriptors = new List<TFeature>();
            List<TPoint>[] imagePoints = new List<TPoint>[images.Length];

            // For all images
            for (int i = 0; i < images.Length; i++)
            {
                Bitmap image = images[i];

                // Compute the feature points
                var points = Detector.ProcessImage(image);

                foreach (IFeatureDescriptor<TFeature> point in points)
                    descriptors.Add(point.Descriptor);

                imagePoints[i] = points;
            }

            TFeature[] data = descriptors.ToArray();

            if (data.Length <= NumberOfWords)
            {
                throw new InvalidOperationException("Not enough data points to cluster. Please try "
                    + "to adjust the feature extraction algorithm to generate more points");
            }

            // Compute the descriptors clusters
            Clustering.Compute(data, threshold);

            return imagePoints;
        }

        /// <summary>
        ///   Gets the codeword representation of a given image.
        /// </summary>
        /// 
        /// <param name="value">The image to be processed.</param>
        /// 
        /// <returns>A double vector with the same length as words
        /// in the code book.</returns>
        /// 
        public double[] GetFeatureVector(Bitmap value)
        {
            // lock source image
            BitmapData imageData = value.LockBits(
                new Rectangle(0, 0, value.Width, value.Height),
                ImageLockMode.ReadOnly, value.PixelFormat);

            double[] features;

            try
            {
                // process the image
                features = GetFeatureVector(new UnmanagedImage(imageData));
            }
            finally
            {
                // unlock image
                value.UnlockBits(imageData);
            }

            return features;
        }

        /// <summary>
        ///   Gets the codeword representation of a given image.
        /// </summary>
        /// 
        /// <param name="value">The image to be processed.</param>
        /// 
        /// <returns>A double vector with the same length as words
        /// in the code book.</returns>
        /// 
        public double[] GetFeatureVector(UnmanagedImage value)
        {
            // Detect feature points in image
            List<TPoint> points = Detector.ProcessImage(value);

            return GetFeatureVector(points);
        }

        /// <summary>
        ///   Gets the codeword representation of a given image.
        /// </summary>
        /// 
        /// <param name="points">The interest points of the image.</param>
        /// 
        /// <returns>A double vector with the same length as words
        /// in the code book.</returns>
        /// 
        public double[] GetFeatureVector(List<TPoint> points)
        {
            int[] features = new int[NumberOfWords];

            // Detect all activation centroids
            Parallel.For(0, points.Count, i =>
            {
                int j = Clustering.Clusters.Nearest(points[i].Descriptor);

                // Form feature vector
                Interlocked.Increment(ref features[j]);
            });

            return features.ToDouble();
        }



        /// <summary>
        ///   Saves the bag of words to a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to which the bow is to be serialized.</param>
        /// 
        public virtual void Save(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(stream, this);
        }

        /// <summary>
        ///   Saves the bag of words to a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file to which the bow is to be serialized.</param>
        /// 
        public void Save(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                Save(fs);
            }
        }

    }
}
