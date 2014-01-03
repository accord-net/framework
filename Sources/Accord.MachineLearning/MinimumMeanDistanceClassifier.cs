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
    using Accord.Math;

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
    public class MinimumMeanDistanceClassifier
    {
        private double[][] means;
        private Func<double[], double[], double> distance;

        /// <summary>
        ///   Initializes a new instance of the <see cref="MinimumMeanDistanceClassifier"/> class.
        /// </summary>
        /// 
        /// <param name="inputs">The input points.</param>
        /// <param name="outputs">The output labels associated with each
        ///   <paramref name="inputs">input points</paramref>.</param>
        /// 
        public MinimumMeanDistanceClassifier(double[][] inputs, int[] outputs)
        {
            this.init(inputs, outputs, Distance.SquareEuclidean);
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
        public MinimumMeanDistanceClassifier(Func<double[], double[], double> distance,
            double[][] inputs, int[] outputs)
        {
            if (distance == null)
                throw new ArgumentNullException("distance");

            this.init(inputs, outputs, distance);
        }

        private void init(double[][] inputs, int[] outputs, Func<double[], double[], double> distance)
        {
            this.distance = distance;

            int symbols = outputs.Max() + 1;
            int dimension = inputs[0].Length;

            int[] counts = new int[symbols];

            means = new double[symbols][];
            for (int i = 0; i < means.Length; i++)
                means[i] = new double[dimension];


            for (int i = 0; i < inputs.Length; i++)
            {
                int k = outputs[i];
                double[] mean = means[k];
                for (int j = 0; j < mean.Length; j++)
                    mean[j] += inputs[i][j];
                counts[k]++;
            }

            for (int i = 0; i < means.Length; i++)
                for (int j = 0; j < means[i].Length; j++)
                    means[i][j] /= counts[i];
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
        public int Compute(double[] input, out double[] distances)
        {
            distances = new double[means.Length];

            for (int i = 0; i < distances.Length; i++)
                distances[i] = distance(input, means[i]);

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
        public int Compute(double[] input)
        {
            double[] distances;
            return Compute(input, out distances);
        }
    }
}
