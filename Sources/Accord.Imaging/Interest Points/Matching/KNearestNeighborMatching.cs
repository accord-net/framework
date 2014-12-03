// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
    using Accord.MachineLearning;
    using Accord.Math;

    /// <summary>
    ///   Nearest neighbor feature point matching algorithm.
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>
    ///     This class matches feature points using a <see cref="KNearestNeighbors">
    ///     k-Nearest Neighbors</see> algorithm.</para>
    /// </remarks>
    ///
    /// <seealso cref="CorrelationMatching"/>
    /// <seealso cref="RansacHomographyEstimator"/>
    /// 
    public class KNearestNeighborMatching : KNearestNeighborMatching<double[]>
    {

        /// <summary>
        ///   Constructs a new <see cref="KNearestNeighbors">
        ///   K-Nearest Neighbors matching</see> algorithm.
        /// </summary>
        /// 
        /// <param name="k">The number of neighbors to use when matching points.</param>
        /// 
        public KNearestNeighborMatching(int k)
            : base(k, Accord.Math.Distance.Euclidean)
        {
        }

        /// <summary>
        ///   Constructs a new <see cref="KNearestNeighbors">
        ///   K-Nearest Neighbors matching</see> algorithm.
        /// </summary>
        /// 
        /// <param name="k">The number of neighbors to use when matching points.</param>
        /// <param name="distance">The distance function to consider between points.</param>
        /// 
        public KNearestNeighborMatching(int k, Func<double[], double[], double> distance)
            : base(k, distance)
        {
        }


        /// <summary>
        ///   Creates a nearest neighbor algorithm with the feature points as
        ///   inputs and their respective indices a the corresponding output.
        /// </summary>
        /// 
        protected override KNearestNeighbors<double[]> CreateNeighbors(double[][] features)
        {
            int classes = features.Length;

            int[] outputs = new int[classes];
            for (int i = 0; i < outputs.Length; i++)
                outputs[i] = i;

            // Create a k-Nearest Neighbor classifier to classify points
            // in the second image to nearest points in the first image
            return new KNearestNeighbors(K, classes, features, outputs, Distance);
        }

    }
}
