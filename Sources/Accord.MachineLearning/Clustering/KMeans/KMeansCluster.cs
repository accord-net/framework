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
    using Accord.Math;
    using Accord.Math.Distances;
    using Accord.Statistics.Distributions.Univariate;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Accord.Compat;

    /// <summary>
    ///   k-Means cluster collection.
    /// </summary>
    /// 
    /// <seealso cref="KMeans"/>
    /// 
    [Serializable]
    public class KMeansClusterCollection : MulticlassScoreClassifierBase<double[]>,
        ICentroidClusterCollection<double[], KMeansClusterCollection.KMeansCluster>
    {
        KMeansCluster.ClusterCollection collection;

        double[][][] covariances;

        /// <summary>
        ///   k-Means' cluster.
        /// </summary>
        /// 
        [Serializable]
        public class KMeansCluster : CentroidCluster<KMeansClusterCollection, double[], KMeansCluster>
        {
            /// <summary>
            ///   Gets the covariance matrix for the samples in this cluster.
            /// </summary>
            /// 
            public double[][] Covariance
            {
                get { return Owner.covariances[Index]; }
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="KMeansClusterCollection"/> class.
        /// </summary>
        /// 
        /// <param name="k">The number of clusters K.</param>
        /// <param name="distance">The distance metric to consider.</param>
        /// 
        public KMeansClusterCollection(int k, IDistance<double[]> distance)
        {
            this.collection = new KMeansCluster.ClusterCollection(this, k, distance);
            this.covariances = new double[k][][];
        }

        /// <summary>
        ///   Gets the dimensionality of the data space.
        /// </summary>
        /// 
        [Obsolete("Please use NumberOfInputs instead.")]
        public int Dimension
        {
            get { return this.NumberOfInputs; }
        }

        /// <summary>
        ///   Gets the clusters' variance-covariance matrices.
        /// </summary>
        /// 
        /// <value>The clusters' variance-covariance matrices.</value>
        /// 
        public double[][][] Covariances
        {
            get { return covariances; }
        }






        // Using composition over inheritance to achieve the closest as possible effect to a Mixin
        // in C# - unfortunately needs a lot a boilerplate code to rewrire the interface methods to
        // where their actual implementation is


        /// <summary>
        ///   Gets or sets the clusters' centroids.
        /// </summary>
        /// 
        /// <value>The clusters' centroids.</value>
        /// 
        public double[][] Centroids
        {
            get { return collection.Centroids; }
            set { collection.Centroids = value; }
        }

        /// <summary>
        /// Gets the proportion of samples in each cluster.
        /// </summary>
        /// 
        public double[] Proportions
        {
            get { return collection.Proportions; }
        }

        /// <summary>
        /// Gets the collection of clusters currently modeled by the clustering algorithm.
        /// </summary>
        /// <value>The clusters.</value>
        public KMeansCluster[] Clusters
        {
            get { return collection.Clusters; }
        }

        /// <summary>
        /// Gets or sets the distance function used to measure the distance
        /// between a point and the cluster centroid in this clustering definition.
        /// </summary>
        /// <value>The distance.</value>
        public IDistance<double[], double[]> Distance
        {
            get { return collection.Distance; }
            set { collection.Distance = value; }
        }

        /// <summary>
        /// Gets the number of clusters in the collection.
        /// </summary>
        /// <value>The count.</value>
        public int Count { get { return collection.Count; } }

        /// <summary>
        /// Gets the <see cref="KMeansCluster"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>GaussianCluster.</returns>
        public KMeansCluster this[int index] { get { return collection[index]; } }

        /// <summary>
        /// Calculates the average square distance from the data points
        /// to the nearest clusters' centroids.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="labels">The labels.</param>
        /// <param name="weights">The weights.</param>
        /// <returns>The average square distance from the data points to the nearest
        /// clusters' centroids.</returns>
        /// <remarks>The average distance from centroids can be used as a measure
        /// of the "goodness" of the clustering. The more the data are
        /// aggregated around the centroids, the less the average distance.</remarks>
        public double Distortion(double[][] data, int[] labels = null, double[] weights = null)
        {
            return collection.Distortion(data, labels, weights);
        }

        /// <summary>
        /// Transform data points into feature vectors containing the
        /// distance between each point and each of the clusters.
        /// </summary>
        /// <param name="points">The input points.</param>
        /// <param name="weights">The weight associated with each point.</param>
        /// <param name="result">An optional matrix to store the computed transformation.</param>
        /// <returns>A vector containing the distance between the input points and the clusters.</returns>
        public double[][] Transform(double[][] points, double[] weights = null, double[][] result = null)
        {
            return collection.Transform(points, weights, result);
        }

        /// <summary>
        /// Transform data points into feature vectors containing the
        /// distance between each point and each of the clusters.
        /// </summary>
        /// <param name="points">The input points.</param>
        /// <param name="labels">The label of each input point.</param>
        /// <param name="weights">The weight associated with each point.</param>
        /// <param name="result">An optional matrix to store the computed transformation.</param>
        /// <returns>A vector containing the distance between the input points and the clusters.</returns>
        public double[] Transform(double[][] points, int[] labels, double[] weights = null, double[] result = null)
        {
            return collection.Transform(points, labels, weights, result);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<KMeansCluster> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return collection.GetEnumerator();
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
            return -Distance.Distance(input, Centroids[classIndex]);
        }

        /// <summary>
        ///   Randomizes the clusters inside a dataset.
        /// </summary>
        /// 
        /// <param name="points">The data to randomize the algorithm.</param>
        /// <param name="strategy">The seeding strategy to be used. Default is <see cref="Seeding.KMeansPlusPlus"/>.</param>
        /// 
        public void Randomize(double[][] points, Seeding strategy)
        {
            collection.Randomize(points, strategy);
        }
    }

}
