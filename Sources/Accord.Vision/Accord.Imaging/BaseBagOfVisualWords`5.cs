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
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Threading;
    using System.Linq;
    using Accord.Compat;
    using System.Threading.Tasks;
    using System.Collections.Concurrent;
    using Accord.Statistics;
    using Accord.Statistics.Distributions.Univariate;
    using System.Diagnostics;
    using Accord.Statistics.Distributions.Fitting;
    using System.Drawing.Imaging;

    /// <summary>
    ///   Base class for <see cref="BagOfVisualWords">Bag of Visual Words</see> implementations.
    /// </summary>
    /// 
    /// <seealso cref="BagOfVisualWords"/>
    /// <seealso cref="BagOfVisualWords{TPoint, TFeature, TClustering, TExtractor}"/>
    /// <seealso cref="BagOfVisualWords{TPoint, TFeature}"/>
    /// <seealso cref="BagOfVisualWords{TPoint}"/>
    /// 
    [Serializable]
    public class BaseBagOfVisualWords<TModel, TFeature, TPoint, TClustering, TExtractor> :
        // TODO: Unify with Accord.MachineLearning.BaseBagOfWords
        BaseBagOfWords<TModel, TFeature, TPoint, TClustering, TExtractor, UnmanagedImage>,
        IBagOfWords<string>,
        IBagOfWords<Bitmap>,
        IBagOfWords<UnmanagedImage>,
        IUnsupervisedLearning<TModel, string, int[]>,
        IUnsupervisedLearning<TModel, string, double[]>,
        IUnsupervisedLearning<TModel, Bitmap, int[]>,
        IUnsupervisedLearning<TModel, Bitmap, double[]>,
        IUnsupervisedLearning<TModel, UnmanagedImage, int[]>,
        IUnsupervisedLearning<TModel, UnmanagedImage, double[]>
        where TFeature : IFeatureDescriptor<TPoint>
        where TModel : BaseBagOfVisualWords<TModel, TFeature, TPoint, TClustering, TExtractor>
        where TClustering : IUnsupervisedLearning<IClassifier<TPoint, int>, TPoint, int>
        where TExtractor : IImageFeatureExtractor<TFeature>
    {

        /// <summary>
        ///   Constructs a new <see cref="BagOfVisualWords"/>.
        /// </summary>
        /// 
        protected BaseBagOfVisualWords()
        {
        }

        #region Obsolete

        /// <summary>
        ///   Computes the Bag of Words model.
        /// </summary>
        /// 
        /// <param name="images">The set of images to initialize the model.</param>
        /// 
        /// <returns>The list of feature points detected in all images.</returns>
        /// 
        [Obsolete("Please use the Learn() method instead.")]
        public List<TFeature>[] Compute(Bitmap[] images)
        {
            var descriptors = new ConcurrentBag<TPoint>();
            var imagePoints = new List<TFeature>[images.Length];

            // For all images
            Parallel.For(0, images.Length, ParallelOptions,
                () => (TExtractor)Detector.Clone(),
                (i, state, detector) =>
                {
                    // Compute the feature points
                    IEnumerable<TFeature> points = detector.Transform(images[i]);
                    foreach (IFeatureDescriptor<TPoint> point in points)
                        descriptors.Add(point.Descriptor);

                    imagePoints[i] = (List<TFeature>)points;
                    return detector;
                },
                (detector) => detector.Dispose());

            Learn(descriptors.ToArray());

            return imagePoints;
        }

        /// <summary>
        ///   Computes the Bag of Words model.
        /// </summary>
        /// 
        /// <param name="features">The extracted image features to initialize the model.</param>
        /// 
        /// <returns>The list of feature points detected in all images.</returns>
        /// 
        [Obsolete("Please use the Learn() method instead.")]
        public void Compute(TPoint[] features)
        {
            Learn(features);
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
        [Obsolete("Please configure the tolerance of the clustering algorithm directly in the "
            + "algorithm itself by accessing it through the Clustering property of this class.")]
        public List<TFeature>[] Compute(Bitmap[] images, double threshold)
        {
            // Hack to maintain backwards compatibility
            var prop = Clustering.GetType().GetProperty("Tolerance");
            if (prop != null && prop.CanWrite)
                prop.SetValue(Clustering, threshold, null);

            return Compute(images);
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
        [Obsolete("Please use the Transform() method instead.")]
        public double[] GetFeatureVector(string value)
        {
            return Transform(value);
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
        [Obsolete("Please use the Transform() method instead.")]
        public double[] GetFeatureVector(Bitmap value)
        {
            return Transform(value);
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
        [Obsolete("Please use the Transform() method instead.")]
        public double[] GetFeatureVector(UnmanagedImage value)
        {
            return Transform(value);
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
        [Obsolete("Please use the Transform() method instead.")]
        public double[] GetFeatureVector(List<TFeature> points)
        {
            return Transform(points);
        }

        /// <summary>
        ///   Saves the bag of words to a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to which the bow is to be serialized.</param>
        /// 
        [Obsolete("Please use Accord.IO.Serializer.Save() instead (or use it as an extension method).")]
        public virtual void Save(Stream stream)
        {
            Accord.IO.Serializer.Save(this, stream);
        }

        /// <summary>
        ///   Saves the bag of words to a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file to which the bow is to be serialized.</param>
        /// 
        [Obsolete("Please use Accord.IO.Serializer.Save() instead (or use it as an extension method).")]

        public void Save(string path)
        {
            Accord.IO.Serializer.Save(this, path);
        }

        #endregion



        #region Transform

        /// <summary>
        /// Applies the transformation to a set of input vectors,
        /// producing an associated set of output vectors.
        /// </summary>
        /// <param name="input">The input data to which
        /// the transformation should be applied.</param>
        /// <param name="result">The location to where to store the
        /// result of this transformation.</param>
        /// <returns>The output generated by applying this
        /// transformation to the given input.</returns>
        public double[] Transform(string input, double[] result)
        {
            using (Bitmap bmp = Accord.Imaging.Image.FromFile(input))
                return Transform(bmp, result);
        }

        /// <summary>
        /// Applies the transformation to a set of input vectors,
        /// producing an associated set of output vectors.
        /// </summary>
        /// <param name="input">The input data to which
        /// the transformation should be applied.</param>
        /// <param name="result">The location to where to store the
        /// result of this transformation.</param>
        /// <returns>The output generated by applying this
        /// transformation to the given input.</returns>
        public int[] Transform(string input, int[] result)
        {
            using (Bitmap bmp = Accord.Imaging.Image.FromFile(input))
                return Transform(bmp, result);
        }

        /// <summary>
        /// Applies the transformation to a set of input vectors,
        /// producing an associated set of output vectors.
        /// </summary>
        /// <param name="input">The input data to which
        /// the transformation should be applied.</param>
        /// <param name="result">The location to where to store the
        /// result of this transformation.</param>
        /// <returns>The output generated by applying this
        /// transformation to the given input.</returns>
        public double[] Transform(Bitmap input, double[] result)
        {
            return Transform(Detector.Transform(input), result);
        }

        /// <summary>
        /// Applies the transformation to a set of input vectors,
        /// producing an associated set of output vectors.
        /// </summary>
        /// <param name="input">The input data to which
        /// the transformation should be applied.</param>
        /// <param name="result">The location to where to store the
        /// result of this transformation.</param>
        /// <returns>The output generated by applying this
        /// transformation to the given input.</returns>
        public int[] Transform(Bitmap input, int[] result)
        {
            return Transform(Detector.Transform(input), result);
        }

        /// <summary>
        /// Applies the transformation to an input, producing an associated output.
        /// </summary>
        /// <param name="input">The input data to which the transformation should be applied.</param>
        /// <returns>The output generated by applying this transformation to the given input.</returns>
        public double[] Transform(string input)
        {
            return Transform(input, new double[NumberOfWords]);
        }

        int[] ICovariantTransform<string, int[]>.Transform(string input)
        {
            return Transform(input, new int[NumberOfWords]);
        }

        /// <summary>
        /// Applies the transformation to an input, producing an associated output.
        /// </summary>
        /// <param name="input">The input data to which the transformation should be applied.</param>
        /// <returns>The output generated by applying this transformation to the given input.</returns>
        public double[] Transform(Bitmap input)
        {
            return Transform(input, new double[NumberOfWords]);
        }

        int[] ICovariantTransform<Bitmap, int[]>.Transform(Bitmap input)
        {
            return Transform(input, new int[NumberOfWords]);
        }



        /// <summary>
        /// Applies the transformation to a set of input vectors,
        /// producing an associated set of output vectors.
        /// </summary>
        /// <param name="input">The input data to which
        /// the transformation should be applied.</param>
        /// <returns>The output generated by applying this
        /// transformation to the given input.</returns>
        public double[][] Transform(string[] input)
        {
            return Transform(input, Jagged.Zeros(input.Length, NumberOfWords));
        }

        int[][] ICovariantTransform<string, int[]>.Transform(string[] input)
        {
            return Transform(input, Jagged.Zeros<int>(input.Length, NumberOfWords));
        }

        /// <summary>
        /// Applies the transformation to a set of input vectors,
        /// producing an associated set of output vectors.
        /// </summary>
        /// <param name="input">The input data to which
        /// the transformation should be applied.</param>
        /// <returns>The output generated by applying this
        /// transformation to the given input.</returns>
        public double[][] Transform(Bitmap[] input)
        {
            return Transform(input, Jagged.Zeros(input.Length, NumberOfWords));
        }

        int[][] ICovariantTransform<Bitmap, int[]>.Transform(Bitmap[] input)
        {
            return Transform(input, Jagged.Zeros<int>(input.Length, NumberOfWords));
        }


        /// <summary>
        /// Applies the transformation to a set of input vectors,
        /// producing an associated set of output vectors.
        /// </summary>
        /// <param name="input">The input data to which
        /// the transformation should be applied.</param>
        /// <param name="result">The location to where to store the
        /// result of this transformation.</param>
        /// <returns>The output generated by applying this
        /// transformation to the given input.</returns>
        public double[][] Transform(string[] input, double[][] result)
        {
            For(0, input.Length, (i, detector) =>
            {
                using (Bitmap bmp = Accord.Imaging.Image.FromFile(input[i]))
                    Transform(detector.Transform(bmp), result[i]);
            });

            return result;
        }

        /// <summary>
        /// Applies the transformation to a set of input vectors,
        /// producing an associated set of output vectors.
        /// </summary>
        /// <param name="input">The input data to which
        /// the transformation should be applied.</param>
        /// <param name="result">The location to where to store the
        /// result of this transformation.</param>
        /// <returns>The output generated by applying this
        /// transformation to the given input.</returns>
        public int[][] Transform(string[] input, int[][] result)
        {
            For(0, input.Length, (i, detector) =>
            {
                using (Bitmap bmp = Accord.Imaging.Image.FromFile(input[i]))
                    Transform(detector.Transform(bmp), result[i]);
            });

            return result;
        }

        /// <summary>
        /// Applies the transformation to a set of input vectors,
        /// producing an associated set of output vectors.
        /// </summary>
        /// <param name="input">The input data to which
        /// the transformation should be applied.</param>
        /// <param name="result">The location to where to store the
        /// result of this transformation.</param>
        /// <returns>The output generated by applying this
        /// transformation to the given input.</returns>
        public double[][] Transform(Bitmap[] input, double[][] result)
        {
            For(0, input.Length, (i, detector) =>
                Transform(detector.Transform(input[i]), result[i]));
            return result;
        }

        /// <summary>
        /// Applies the transformation to a set of input vectors,
        /// producing an associated set of output vectors.
        /// </summary>
        /// <param name="input">The input data to which
        /// the transformation should be applied.</param>
        /// <param name="result">The location to where to store the
        /// result of this transformation.</param>
        /// <returns>The output generated by applying this
        /// transformation to the given input.</returns>
        public int[][] Transform(Bitmap[] input, int[][] result)
        {
            For(0, input.Length, (i, detector) =>
                Transform(detector.Transform(input[i]), result[i]));
            return result;
        }

        #endregion



        #region Learn
        /// <summary>
        /// Learns a model that can map the given inputs to the desired outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="weights">The weight of importance for each input sample.</param>
        /// <returns>A model that has learned how to produce suitable outputs
        /// given the input data <paramref name="x" />.</returns>
        public TModel Learn(string[] x, double[] weights = null)
        {
            return InnerLearn(x, weights, (xi, detector) =>
            {
                using (Bitmap bmp = Accord.Imaging.Image.FromFile(xi))
                    return bmp.LockBits(ImageLockMode.ReadOnly, (ui) => detector.Transform(ui));
            });
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the desired outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="weights">The weight of importance for each input sample.</param>
        /// <returns>A model that has learned how to produce suitable outputs
        /// given the input data <paramref name="x" />.</returns>
        public TModel Learn(Bitmap[] x, double[] weights = null)
        {
            return InnerLearn(x, weights, (xi, detector) =>
            {
                return xi.LockBits(ImageLockMode.ReadOnly, (ui) => detector.Transform(ui));
            });
        }
        #endregion

        int ITransform.NumberOfInputs
        {
            get { return NumberOfInputs; }
            set { throw new InvalidOperationException("This property is read-only."); }
        }

        int ITransform.NumberOfOutputs
        {
            get { return NumberOfOutputs; }
            set { throw new InvalidOperationException("This property is read-only."); }
        }
    }
}
