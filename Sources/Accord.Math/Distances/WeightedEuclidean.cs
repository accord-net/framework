// Accord Math Library
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

namespace Accord.Math.Distances
{
    using System;
    using System.Runtime.CompilerServices;
    using Accord.Compat;

    /// <summary>
    ///   Weighted Euclidean distance metric.
    /// </summary>
    /// 
    /// <seealso cref="Euclidean"/>
    /// <seealso cref="SquareEuclidean"/>
    /// <seealso cref="WeightedSquareEuclidean"/>
    /// 
    [Serializable]
    public struct WeightedEuclidean :
        IMetric<double[]>, ISimilarity<double[]>
    {
        private double[] weights;


        /// <summary>
        /// Gets or sets the weights for each dimension. Default is a vector of ones.
        /// </summary>
        /// 
        /// <value>The weights.</value>
        /// 
        public double[] Weights
        {
            get { return weights; }
            set { weights = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightedEuclidean"/> struct.
        /// </summary>
        /// <param name="dimensions">The number of dimensions (columns) in the dataset.</param>
        public WeightedEuclidean(int dimensions)
        {
            this.weights = Vector.Ones(dimensions);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightedEuclidean"/> struct.
        /// </summary>
        /// <param name="weights">The weights.</param>
        public WeightedEuclidean(double[] weights)
        {
            this.weights = weights;
        }

        /// <summary>
        ///   Computes the distance <c>d(x,y)</c> between points
        ///   <paramref name="x"/> and <paramref name="y"/>.
        /// </summary>
        /// 
        /// <param name="x">The first point <c>x</c>.</param>
        /// <param name="y">The second point <c>y</c>.</param>
        /// 
        /// <returns>
        ///   A double-precision value representing the distance <c>d(x,y)</c>
        ///   between <paramref name="x"/> and <paramref name="y"/> according 
        ///   to the distance function implemented by this class.
        /// </returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public double Distance(double[] x, double[] y)
        {
            double sum = 0.0;
            for (int i = 0; i < x.Length; i++)
            {
                double u = x[i] - y[i];
                sum += u * u * weights[i];
            }
            return Math.Sqrt(sum);
        }

        /// <summary>
        ///   Gets a similarity measure between two points.
        /// </summary>
        /// 
        /// <param name="x">The first point to be compared.</param>
        /// <param name="y">The second point to be compared.</param>
        /// 
        /// <returns>A similarity measure between x and y.</returns>
        /// 
        public double Similarity(double[] x, double[] y)
        {
            return 1.0 / (1.0 + Distance(x, y));
        }

    }
}
