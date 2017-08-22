// Accord Unit Tests
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

namespace Accord.Tests.MachineLearning.Structures
{
    using Accord.Math;
    using Accord.Math.Distances;
    using System;
    using System.Linq;

    public class NaiveKNearestNeighbors : NaiveKNearestNeighbors<double>
    {

        /// <summary>
        ///   Creates a new <see cref="KNearestNeighbor"/>.
        /// </summary>
        /// 
        /// <param name="k">The number of nearest neighbors to be used in the decision.</param>
        /// 
        /// <param name="inputs">The input data points.</param>
        /// <param name="outputs">The associated labels for the input points.</param>
        /// 
        public NaiveKNearestNeighbors(int k, double[][] inputs, int[] outputs)
            : base(k, inputs, outputs, new Euclidean()) { }

    }

    public class NaiveKNearestNeighbors<T>
    {
        private int k;

        private T[][] inputs;
        private int[] outputs;

        private int classCount;

        private IDistance<T[]> distance;

        private double[] distances;

        /// <summary>
        ///   Creates a new <see cref="KNearestNeighbor"/>.
        /// </summary>
        /// 
        /// <param name="k">The number of nearest neighbors to be used in the decision.</param>
        /// 
        /// <param name="inputs">The input data points.</param>
        /// <param name="outputs">The associated labels for the input points.</param>
        /// <param name="distance">The distance measure to use in the decision.</param>
        /// 
        public NaiveKNearestNeighbors(int k, T[][] inputs, int[] outputs, IDistance<T[]> distance)
        {
            this.inputs = inputs;
            this.outputs = outputs;

            this.k = k;
            this.classCount = outputs.Distinct().Count();

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
        public T[][] Inputs
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
        ///   Gets or sets the distance function used
        ///   as a distance metric between data points.
        /// </summary>
        /// 
        public IDistance<T[]> Distance
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
        /// <param name="input">A point to be classificated.</param>
        /// 
        /// <returns>The most likely label for the given point.</returns>
        /// 
        public int Compute(T[] input)
        {
            for (int i = 0; i < inputs.Length; i++)
                distances[i] = distance.Distance(input, inputs[i]);

            int[] nearestIndices = Matrix.Indices(0, inputs.Length);

            Array.Sort(distances, nearestIndices);

            double[] scores = new double[classCount];

            for (int i = 0; i < k; i++)
            {
                int j = nearestIndices[i];

                int label = outputs[j];
                double d = distances[i];

                scores[label] += 1.0 / (1 + d);
            }

            // Get the maximum weighted score
            int result; scores.Max(out result);

            return result;
        }

        public virtual T[][] GetNearestNeighbors(T[] input, out int[] labels)
        {
            // Compute all distances
            for (int i = 0; i < inputs.Length; i++)
                distances[i] = distance.Distance(input, inputs[i]);

            int[] nearestIndices = Matrix.Indices(0, inputs.Length);

            Array.Sort(distances, nearestIndices);

            int[] idx = nearestIndices.First(k);

            labels = outputs.Submatrix(idx);
            return inputs.Submatrix(idx);
        }

    }
}
