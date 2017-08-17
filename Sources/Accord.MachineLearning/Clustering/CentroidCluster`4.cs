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
    using System;
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Data cluster.
    /// </summary>
    /// 
    [Serializable]
    public class CentroidCluster<TCollection, TData, TCentroid, TCluster> : Cluster<TCollection, TData, TCluster>
        where TCollection : ICentroidClusterCollection<TData, TCentroid, TCluster>, IMulticlassScoreClassifier<TData, int>
        where TCluster : CentroidCluster<TCollection, TData, TCentroid, TCluster>, new()
    {
        /// <summary>
        ///   Gets the cluster's centroid.
        /// </summary>
        /// 
        public TCentroid Centroid
        {
            get { return Owner.Centroids[Index]; }
            set { Owner.Centroids[Index] = value; }
        }

        /// <summary>
        ///   Computes the distortion of the cluster, measured
        ///   as the average distance between the cluster points
        ///   and its centroid.
        /// </summary>
        /// 
        /// <param name="data">The input points.</param>
        /// 
        /// <returns>The average distance between all points
        /// in the cluster and the cluster centroid.</returns>
        /// 
        public double Distortion(TData[] data)
        {
            return Owner.Distortion(data, Vector.Create(data.Length, Index));
        }

        [Serializable]
        new internal class ClusterCollection : Cluster<TCollection, TData, TCluster>.ClusterCollection,
            ICentroidClusterCollection<TData, TCentroid, TCluster>
        {
            private IDistance<TData, TCentroid> distance;
            private TCentroid[] centroids;


            /// <summary>
            ///   Initializes a new instance of the <see cref="KMeansClusterCollection"/> class.
            /// </summary>
            /// 
            /// <param name="collection">The collection that contains this instance as a field.</param>
            /// <param name="k">The number of clusters K.</param>
            /// <param name="distance">The distance metric to consider.</param>
            /// 
            public ClusterCollection(TCollection collection, int k, IDistance<TData, TCentroid> distance)
                : base(collection, k)
            {
                this.distance = distance;
                this.centroids = new TCentroid[k];

                collection.NumberOfOutputs = k;
                collection.NumberOfClasses = k;
            }

            /// <summary>
            ///   Gets or sets the distance function used to measure the distance
            ///   between a point and the cluster centroid in this clustering definition.
            /// </summary>
            /// 
            public IDistance<TData, TCentroid> Distance
            {
                get { return distance; }
                set { distance = value; }
            }

            /// <summary>
            ///   Gets or sets the clusters' centroids.
            /// </summary>
            /// 
            /// <value>The clusters' centroids.</value>
            /// 
            public TCentroid[] Centroids
            {
                get { return centroids; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("value");

                    if (value.Length != this.Count)
                        throw new ArgumentException("The number of centroids should be equal to K.", "value");

                    for (int i = 0; i < centroids.Length; i++)
                        centroids[i] = value[i];

                    Owner.NumberOfInputs = Tools.GetNumberOfInputs(value);
                }
            }

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
            public double Distortion(TData[] data, int[] labels = null, double[] weights = null)
            {
                if (labels == null)
                    labels = Owner.Decide(data);

                if (weights == null)
                {
                    double error = 0.0;
                    for (int i = 0; i < data.Length; i++)
                        error += Distance.Distance(data[i], centroids[labels[i]]);
                    return error / (double)data.Length;
                }
                else
                {
                    double error = 0.0;
                    for (int i = 0; i < data.Length; i++)
                        error += weights[i] * Distance.Distance(data[i], centroids[labels[i]]);
                    return error / weights.Sum();
                }
            }

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
            public double[] Transform(TData[] points, int[] labels, double[] weights = null, double[] result = null)
            {
                if (result == null)
                    result = new double[points.Length];

                if (weights == null)
                {
                    for (int i = 0; i < result.Length; i++)
                        result[i] = Distance.Distance(points[i], centroids[labels[i]]);
                }
                else
                {
                    for (int i = 0; i < result.Length; i++)
                        result[i] = weights[i] * Distance.Distance(points[i], centroids[labels[i]]);
                }

                return result;
            }

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
            public double[][] Transform(TData[] points, double[] weights = null, double[][] result = null)
            {
                if (result == null)
                {
                    result = new double[points.Length][];
                    for (int i = 0; i < result.Length; i++)
                        result[i] = new double[centroids.Length];
                }

                if (weights == null)
                {
                    for (int i = 0; i < result.Length; i++)
                        for (int j = 0; j < centroids.Length; j++)
                            result[i][j] = Distance.Distance(points[i], centroids[j]);
                }
                else
                {
                    for (int i = 0; i < result.Length; i++)
                        for (int j = 0; j < centroids.Length; j++)
                            result[i][j] = weights[i] * Distance.Distance(points[i], centroids[j]);
                }

                return result;
            }

            public double Score(TData input, int classIndex)
            {
                return -Distance.Distance(input, Centroids[classIndex]);
            }
        }
    }
}
