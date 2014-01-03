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
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    ///   Common interface for clustering algorithms.
    /// </summary>
    /// 
    /// <typeparam name="TData">The type of the data being clustered, such as <see cref="T:double[]"/>.</typeparam>
    /// 
    /// <seealso cref="KMeans"/>
    /// <seealso cref="KModes{TData}"/>
    /// <seealso cref="BinarySplit"/>
    /// <seealso cref="GaussianMixtureModel"/>
    /// 
    public interface IClusteringAlgorithm<TData>
    {
        /// <summary>
        ///   Divides the input data into a number of clusters. 
        /// </summary>  
        /// 
        /// <param name="points">The data where to compute the algorithm.</param>
        /// <param name="threshold">The relative convergence threshold
        /// for the algorithm. Default is 1e-5.</param>
        /// 
        /// <returns>
        ///   The labelings for the input data.
        /// </returns>
        /// 
        int[] Compute(TData[] points, double threshold);

        /// <summary>
        ///   Gets the collection of clusters currently modeled by the clustering algorithm.
        /// </summary>
        /// 
        IClusterCollection<TData> Clusters { get; }
    }

    /// <summary>
    ///   Common interface for cluster collections.
    /// </summary>
    /// 
    /// <typeparam name="TData">The type of the data being clustered, such as <see cref="T:double[]"/>.</typeparam>
    /// 
    public interface IClusterCollection<in TData>
        : IEnumerable
    {
        /// <summary>
        ///   Gets the number of clusters in the collection.
        /// </summary>
        /// 
        int Count { get; }

        /// <summary>
        ///   Returns the closest cluster to an input point.
        /// </summary>
        /// 
        /// <param name="point">The input vector.</param>
        /// <returns>
        ///   The index of the nearest cluster
        ///   to the given data point. </returns>
        ///   
        int Nearest(TData point);
    }

    /// <summary>
    ///   Common interface for cluster collections.
    /// </summary>
    /// 
    /// <typeparam name="TData">The type of the data being clustered, such as <see cref="T:double[]"/>.</typeparam>
    /// <typeparam name="TCluster">The type of the clusters considered by a clustering algorithm.</typeparam>
    /// 
    public interface IClusterCollection<TData,
#if !NET35
        out
#endif
 TCluster>
        : IEnumerable<TCluster>, IClusterCollection<TData>
    {
        /// <summary>
        ///   Gets the cluster at the given index.
        /// </summary>
        /// 
        /// <param name="index">The index of the cluster. This should also be the class label of the cluster.</param>
        /// 
        /// <returns>An object holding information about the selected cluster.</returns>
        /// 
        TCluster this[int index] { get; }
    }

}
