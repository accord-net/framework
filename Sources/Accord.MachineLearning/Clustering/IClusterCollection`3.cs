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
    using System.Collections.Generic;
    using Accord.Math.Distances;

    /// <summary>
    ///   Common interface for collections of clusters (i.e. <see cref="KMeansClusterCollection"/>,
    ///   <see cref="GaussianClusterCollection"/>, <see cref="MeanShiftClusterCollection"/>).
    /// </summary>
    /// 
    public interface IClusterCollectionEx<TData, TCluster> : IEnumerable<TCluster>
    {

        /// <summary>
        ///   Gets the number of clusters in the collection.
        /// </summary>
        /// 
        int Count { get; }

        /// <summary>
        ///   Gets the collection of clusters currently modeled by the clustering algorithm.
        /// </summary>
        /// 
        TCluster[] Clusters { get; }

        /// <summary>
        /// Gets the proportion of samples in each cluster.
        /// </summary>
        /// 
        double[] Proportions { get; }

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

    /// <summary>
    ///   Common interface for clusters that contains centroids, where the centroid data type might be different 
    ///   from the data type of the data bring clustered (i.e. <see cref="GaussianClusterCollection.GaussianCluster"/>).
    /// </summary>
    /// 
    public interface ICentroidClusterCollection<TData, TCentroids, TCluster> : IClusterCollectionEx<TData, TCluster>
    {
        /// <summary>
        ///   Gets or sets the clusters' centroids.
        /// </summary>
        /// 
        /// <value>The clusters' centroids.</value>
        /// 
        TCentroids[] Centroids { get; set; }

        /// <summary>
        ///   Gets or sets the distance function used to measure the distance
        ///   between a point and the cluster centroid in this clustering definition.
        /// </summary>
        /// 
        IDistance<TData, TCentroids> Distance { get; set; }
        
        /// <summary>
        ///   Calculates the average square distance from the data points 
        ///   to the nearest clusters' centroids.
        /// </summary>
        /// 
        /// <remarks>
        ///   The average distance from centroids can be used as a measure
        ///   of the "goodness" of the clustering. The more the data are 
        ///   aggregated around the centroids, the less the average distance.
        /// </remarks>
        /// 
        /// <returns>
        ///   The average square distance from the data points to the nearest 
        ///   clusters' centroids.
        /// </returns>
        /// 
        double Distortion(TData[] data, int[] labels = null, double[] weights = null);

        /// <summary>
        ///   Transform data points into feature vectors containing the 
        ///   distance between each point and each of the clusters.
        /// </summary>
        /// 
        /// <param name="points">The input points.</param>
        /// <param name="weights">The weight associated with each point.</param>
        /// <param name="result">An optional matrix to store the computed transformation.</param>
        /// 
        /// <returns>A vector containing the distance between the input points and the clusters.</returns>
        /// 
        double[][] Transform(TData[] points, double[] weights = null, double[][] result = null);

        /// <summary>
        ///   Transform data points into feature vectors containing the 
        ///   distance between each point and each of the clusters.
        /// </summary>
        /// 
        /// <param name="points">The input points.</param>
        /// <param name="labels">The label of each input point.</param>
        /// <param name="weights">The weight associated with each point.</param>
        /// <param name="result">An optional matrix to store the computed transformation.</param>
        /// 
        /// <returns>A vector containing the distance between the input points and the clusters.</returns>
        /// 
        double[] Transform(TData[] points, int[] labels, double[] weights = null, double[] result = null);
    }

    /// <summary>
    ///   Common interface for clusters that contains centroids which are of the same data type 
    ///   as the clustered data types (i.e. <see cref="KMeansClusterCollection.KMeansCluster"/>).
    /// </summary>
    /// 
    public interface ICentroidClusterCollection<TData, TCluster> : ICentroidClusterCollection<TData, TData, TCluster>
    {
    }
}