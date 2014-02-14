// Accord Machine Learning Library
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

namespace Accord.MachineLearning
{
    using System;
    using System.Linq;
    using Accord.Math;

    /// <summary>
    ///   K-Nearest Neighbor (k-NN) algorithm.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the input data.</typeparam>
    /// 
    /// <remarks>
    /// <para> The k-nearest neighbor algorithm (k-NN) is a method for classifying objects
    ///   based on closest training examples in the feature space. It is amongst the simplest
    ///   of all machine learning algorithms: an object is classified by a majority vote of
    ///   its neighbors, with the object being assigned to the class most common amongst its 
    ///   k nearest neighbors (k is a positive integer, typically small).</para>
    ///   
    /// <para>If k = 1, then the object is simply assigned to the class of its nearest neighbor.</para>
    /// 
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia contributors. "K-nearest neighbor algorithm." Wikipedia, The
    ///       Free Encyclopedia. Wikipedia, The Free Encyclopedia, 10 Oct. 2012. Web.
    ///       9 Nov. 2012. http://en.wikipedia.org/wiki/K-nearest_neighbor_algorithm </description></item>
    ///   </list>
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>The following example shows how to create
    /// and use a k-Nearest Neighbor algorithm to classify
    /// a set of numeric vectors.</para>
    /// 
    /// <code>
    /// // Create some sample learning data. In this data,
    /// // the first two instances belong to a class, the
    /// // four next belong to another class and the last
    /// // three to yet another.
    /// 
    /// double[][] inputs = 
    /// {
    ///     // The first two are from class 0
    ///     new double[] { -5, -2, -1 },
    ///     new double[] { -5, -5, -6 },
    /// 
    ///     // The next four are from class 1
    ///     new double[] {  2,  1,  1 },
    ///     new double[] {  1,  1,  2 },
    ///     new double[] {  1,  2,  2 },
    ///     new double[] {  3,  1,  2 },
    /// 
    ///     // The last three are from class 2
    ///     new double[] { 11,  5,  4 },
    ///     new double[] { 15,  5,  6 },
    ///     new double[] { 10,  5,  6 },
    /// };
    /// 
    /// int[] outputs =
    /// {
    ///     0, 0,        // First two from class 0
    ///     1, 1, 1, 1,  // Next four from class 1
    ///     2, 2, 2      // Last three from class 2
    /// };
    /// 
    /// 
    /// // Now we will create the K-Nearest Neighbors algorithm. For this
    /// // example, we will be choosing k = 4. This means that, for a given
    /// // instance, its nearest 4 neighbors will be used to cast a decision.
    /// KNearestNeighbor knn = new KNearestNeighbor(k: 4, classes: 3,
    ///     inputs: inputs, outputs: outputs);
    /// 
    /// 
    /// // After the algorithm has been created, we can classify a new instance:
    /// int answer = knn.Compute(new double[] { 11, 5, 4 }); // answer will be 2.
    /// </code>
    /// 
    /// 
    /// <para>The k-Nearest neighbor algorithm implementation in the 
    /// framework can also be used with any instance data type. For
    /// such cases, the framework offers a generic version of the 
    /// classifier, as shown in the example below.</para>
    /// 
    /// <code>
    /// // The k-Nearest Neighbors algorithm can be used with
    /// // any kind of data. In this example, we will see how
    /// // it can be used to compare, for example, Strings.
    /// 
    /// string[] inputs = 
    /// {
    ///     "Car",    // class 0
    ///     "Bar",    // class 0
    ///     "Jar",    // class 0
    /// 
    ///     "Charm",  // class 1
    ///     "Chair"   // class 1
    /// };
    /// 
    /// int[] outputs =
    /// {
    ///     0, 0, 0,  // First three are from class 0
    ///     1, 1,     // And next two are from class 1
    /// };
    /// 
    /// 
    /// // Now we will create the K-Nearest Neighbors algorithm. For this
    /// // example, we will be choosing k = 1. This means that, for a given
    /// // instance, only its nearest neighbor will be used to cast a new
    /// // decision. 
    ///             
    /// // In order to compare strings, we will be using Levenshtein's string distance
    /// KNearestNeighbors&lt;string> knn = new KNearestNeighbors&lt;string>(k: 1, classes: 2,
    ///     inputs: inputs, outputs: outputs, distance: Distance.Levenshtein);
    /// 
    /// 
    /// // After the algorithm has been created, we can use it:
    /// int answer = knn.Compute("Chars"); // answer should be 1.
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="KNearestNeighbors"/>
    /// 
    [Serializable]
    public class KNearestNeighbors<T>
    {
        private int k;

        private T[] inputs;
        private int[] outputs;

        private int classCount;

        private Func<T, T, double> distance;

        private double[] distances;


        /// <summary>
        ///   Creates a new <see cref="KNearestNeighbors"/>.
        /// </summary>
        /// 
        /// <param name="k">The number of nearest neighbors to be used in the decision.</param>
        /// 
        /// <param name="inputs">The input data points.</param>
        /// <param name="outputs">The associated labels for the input points.</param>
        /// <param name="distance">The distance measure to use in the decision.</param>
        /// 
        public KNearestNeighbors(int k, T[] inputs, int[] outputs, Func<T, T, double> distance)
        {
            checkArgs(k, null, inputs, outputs, distance);

            int classCount = outputs.Distinct().Count();

            initialize(k, classCount, inputs, outputs, distance);
        }

        /// <summary>
        ///   Creates a new <see cref="KNearestNeighbors"/>.
        /// </summary>
        /// 
        /// <param name="k">The number of nearest neighbors to be used in the decision.</param>
        /// <param name="classes">The number of classes in the classification problem.</param>
        /// 
        /// <param name="inputs">The input data points.</param>
        /// <param name="outputs">The associated labels for the input points.</param>
        /// <param name="distance">The distance measure to use in the decision.</param>
        /// 
        public KNearestNeighbors(int k, int classes, T[] inputs, int[] outputs, Func<T, T, double> distance)
        {
            checkArgs(k, classes, inputs, outputs, distance);

            initialize(k, classes, inputs, outputs, distance);
        }

        private void initialize(int k, int classes, T[] inputs, int[] outputs, Func<T, T, double> distance)
        {
            this.inputs = inputs;
            this.outputs = outputs;

            this.k = k;
            this.classCount = classes;

            this.distance = distance;
            this.distances = new double[inputs.Length];
        }


        /// <summary>
        ///   Gets the set of points given
        ///   as input of the algorithm.
        /// </summary>
        /// 
        /// <value>The input points.</value>
        /// 
        public T[] Inputs
        {
            get { return inputs; }
        }

        /// <summary>
        ///   Gets the set of labels associated
        ///   with each <see cref="Inputs"/> point.
        /// </summary>
        /// 
        public int[] Outputs
        {
            get { return outputs; }
        }

        /// <summary>
        ///   Gets the number of class labels
        ///   handled by this classifier.
        /// </summary>
        /// 
        public int ClassCount
        {
            get { return classCount; }
        }

        /// <summary>
        ///   Gets or sets the distance function used
        ///   as a distance metric between data points.
        /// </summary>
        /// 
        public Func<T, T, double> Distance
        {
            get { return distance; }
            set { distance = value; }
        }


        /// <summary>
        ///   Gets or sets the number of nearest 
        ///   neighbors to be used in the decision.
        /// </summary>
        /// 
        /// <value>The number of neighbors.</value>
        /// 
        public int K
        {
            get { return k; }
            set
            {
                if (value <= 0 || value > inputs.Length)
                    throw new ArgumentOutOfRangeException("value",
                        "The value for k should be greater than zero and less than total number of input points.");

                k = value;
            }
        }

        /// <summary>
        ///   Computes the most likely label of a new given point.
        /// </summary>
        /// 
        /// <param name="input">A point to be classified.</param>
        /// 
        /// <returns>The most likely label for the given point.</returns>
        /// 
        public int Compute(T input)
        {
            double[] scores;
            return Compute(input, out scores);
        }

        /// <summary>
        ///   Computes the most likely label of a new given point.
        /// </summary>
        /// 
        /// <param name="input">A point to be classified.</param>
        /// <param name="response">A value between 0 and 1 giving 
        /// the strength of the classification in relation to the
        /// other classes.</param>
        /// 
        /// <returns>The most likely label for the given point.</returns>
        /// 
        public int Compute(T input, out double response)
        {
            double[] scores;
            int result = Compute(input, out scores);
            response = scores[result] / scores.Sum();

            return result;
        }

        /// <summary>
        ///   Computes the most likely label of a new given point.
        /// </summary>
        /// 
        /// <param name="input">A point to be classified.</param>
        /// <param name="scores">The distance score for each possible class.</param>
        /// 
        /// <returns>The most likely label for the given point.</returns>
        /// 
        public virtual int Compute(T input, out double[] scores)
        {
            // Compute all distances
            for (int i = 0; i < inputs.Length; i++)
                distances[i] = distance(input, inputs[i]);

            int[] idx = distances.Bottom(k, inPlace: true);

            scores = new double[classCount];

            for (int i = 0; i < idx.Length; i++)
            {
                int j = idx[i];

                int label = outputs[j];
                double d = distances[i];

                // Convert to similarity measure
                scores[label] += 1.0 / (1.0 + d);
            }

            // Get the maximum weighted score
            int result; scores.Max(out result);

            return result;
        }






        private static void checkArgs(int k, int? classes, T[] inputs, int[] outputs, Func<T, T, double> distance)
        {
            if (k <= 0)
                throw new ArgumentOutOfRangeException("k", "Number of neighbors should be greater than zero.");

            if (classes != null && classes <= 0)
                throw new ArgumentOutOfRangeException("k", "Number of classes should be greater than zero.");

            if (inputs == null)
                throw new ArgumentNullException("inputs");

            if (outputs == null)
                throw new ArgumentNullException("inputs");

            if (inputs.Length != outputs.Length)
                throw new DimensionMismatchException("outputs",
                    "The number of input vectors should match the number of corresponding output labels");

            if (distance == null)
                throw new ArgumentNullException("distance");
        }
    }
}
