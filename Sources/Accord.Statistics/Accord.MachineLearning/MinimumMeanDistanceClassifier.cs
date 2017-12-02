// Accord Machine Learning Library
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

namespace Accord.MachineLearning
{
    using System;
    using Accord.Math;
    using Accord.Math.Distances;
    using Accord.Compat;
    using System.Threading;

    /// <summary>
    ///   Minimum (Mean) Distance Classifier.
    /// </summary>
    /// 
    /// <remarks>
    ///   This is one of the simplest possible pattern recognition classifiers. 
    ///   This classifier works by comparing a new input vector against the mean
    ///   value of the other classes. The class which is closer to this new input 
    ///   vector is considered the winner, and the vector will be classified as 
    ///   having the same label as this class.
    /// </remarks>
    /// 
    /// <example>
    /// <code source="Unit Tests\Accord.Tests.Statistics\MinimumMeanDistanceClassifierTest.cs" region="doc_learn" />
    /// </example>
    /// 
    [Serializable]
    public class MinimumMeanDistanceClassifier : MulticlassScoreClassifierBase<double[]>,
        ISupervisedLearning<MinimumMeanDistanceClassifier, double[], int>
    {
        [NonSerialized]
        CancellationToken token = new CancellationToken();

        private double[][] means;
        private IDistance<double[]> distance = new SquareEuclidean();

        /// <summary>
        /// Gets or sets a cancellation token that can be used to
        /// stop the learning algorithm while it is running.
        /// </summary>
        public CancellationToken Token
        {
            get { return token; }
            set { token = value; }
        }

        /// <summary>
        ///   Gets or sets the class means to which samples will be compared against.
        /// </summary>
        /// 
        public double[][] Means
        {
            get { return means; }
            set { means = value; }
        }

        /// <summary>
        ///   Gets or sets the distance function to be used when comparing a sample to a class mean.
        /// </summary>
        /// 
        public IDistance<double[]> Function
        {
            get { return distance; }
            set { distance = value; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MinimumMeanDistanceClassifier"/> class.
        /// </summary>
        /// 
        public MinimumMeanDistanceClassifier()
        {
            this.distance = new SquareEuclidean();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MinimumMeanDistanceClassifier"/> class.
        /// </summary>
        /// 
        /// <param name="inputs">The input points.</param>
        /// <param name="outputs">The output labels associated with each
        ///   <paramref name="inputs">input points</paramref>.</param>
        /// 
        public MinimumMeanDistanceClassifier(double[][] inputs, int[] outputs)
            : this()
        {
            Learn(inputs, outputs);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MinimumMeanDistanceClassifier"/> class.
        /// </summary>
        /// 
        /// <param name="distance">A distance function. Default is to use 
        ///   the <see cref="Distance.Euclidean(double[],double[])"/> distance.</param>
        /// <param name="inputs">The input points.</param>
        /// <param name="outputs">The output labels associated with each
        ///   <paramref name="inputs">input points</paramref>.</param>
        ///   
        public MinimumMeanDistanceClassifier(IDistance<double[]> distance,
            double[][] inputs, int[] outputs)
        {
            if (distance == null)
                throw new ArgumentNullException("distance");

            this.distance = distance;

            Learn(inputs, outputs);
        }


        /// <summary>
        ///   Computes the label for the given input.
        /// </summary>
        /// 
        /// <param name="input">The input value.</param>
        /// <param name="distances">The distances from <paramref name="input"/> to the class means.</param>
        /// 
        /// <returns>The output label assigned to this point.</returns>
        /// 
        [Obsolete("Please use Decide instead.")]
        public int Compute(double[] input, out double[] distances)
        {
            distances = new double[means.Length];
            for (int i = 0; i < distances.Length; i++)
                distances[i] = distance.Distance(input, means[i]);

            int imin;
            distances.Min(out imin);

            return imin;
        }

        /// <summary>
        ///   Computes the label for the given input.
        /// </summary>
        /// 
        /// <param name="input">A input.</param>
        /// 
        /// <returns>The output label assigned to this point.</returns>
        /// 
        [Obsolete("Please use Decide instead.")]
        public int Compute(double[] input)
        {
            double[] distances;
            return Compute(input, out distances);
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and each class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the result will be stored,
        /// avoiding unnecessary memory allocations.</param>
        /// <returns></returns>
        public override double[] Scores(double[] input, double[] result)
        {
            for (int i = 0; i < means.Length; i++)
                result[i] = -distance.Distance(input, means[i]);
            return result;
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and a given
        /// <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// <returns>System.Double.</returns>
        public override double Score(double[] input, int classIndex)
        {
            return -distance.Distance(input, means[classIndex]);
        }



        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// <returns>A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.</returns>
        public MinimumMeanDistanceClassifier Learn(double[][] x, int[] y, double[] weights = null)
        {
            if (weights != null)
                throw new ArgumentException(Accord.Properties.Resources.NotSupportedWeights, "weights");

            NumberOfInputs = x.Columns();
            NumberOfOutputs = y.Max() + 1;
            NumberOfClasses = y.Max() + 1;
            means = Jagged.Zeros(NumberOfOutputs, NumberOfInputs);

            int[] counts = new int[NumberOfOutputs];

            // Compute the average of the input vectors for each of
            // the output classes. Afterwards, a decision can be cast
            // by checking to which average a new sample is closer.
            for (int i = 0; i < x.Length; i++)
            {
                int k = y[i];
                double[] mean = means[k];
                for (int j = 0; j < mean.Length; j++)
                    mean[j] += x[i][j];
                counts[k]++;

                if (Token.IsCancellationRequested)
                    break;
            }

            means.Divide(counts, dimension: (VectorType)1, result: means);

            return this;
        }

    }
}
