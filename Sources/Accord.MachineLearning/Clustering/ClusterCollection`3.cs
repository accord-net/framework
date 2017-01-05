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
    using System.Threading.Tasks;

    /// <summary>
    ///   Cluster collection.
    /// </summary>
    /// 
    /// <seealso cref="KMeans"/>
    /// <seealso cref="KModes"/>
    /// <seealso cref="MeanShift"/>
    /// <seealso cref="GaussianMixtureModel"/>
    /// 
    [Serializable]
    public class ClusterCollection<TData, TCentroids, TCluster> 
        : MulticlassScoreClassifierBase<TData>, IClusterCollection<TData, TCluster>
    {
        /// <summary>
        ///   Data cluster.
        /// </summary>
        /// 
        [Serializable]
        public class Cluster<TCollection>
            where TCollection : ClusterCollection<TData, TCentroids, TCluster>
        {
            private TCollection owner;
            private int index;

            /// <summary>
            ///   Gets the collection to which this cluster belongs to.
            /// </summary>
            /// 
            public TCollection Owner { get { return owner; } }

            /// <summary>
            ///   Gets the label for this cluster.
            /// </summary>
            /// 
            public int Index
            {
                get { return this.index; }
            }

            /// <summary>
            ///   Gets the cluster's centroid.
            /// </summary>
            /// 
            public TCentroids Centroid
            {
                get { return owner.Centroids[index]; }
                set { owner.Centroids[index] = value; }
            }

            /// <summary>
            ///   Gets the proportion of samples in the cluster.
            /// </summary>
            /// 
            public double Proportion
            {
                get { return owner.Proportions[index]; }
                set { owner.Proportions[index] = value; }
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
                return owner.Distortion(data, Vector.Create(data.Length, index));
            }

            /// <summary>
            ///   Initializes a new instance of the <see cref="Cluster{TCollection}"/> class.
            /// </summary>
            /// 
            /// <param name="owner">The owner collection.</param>
            /// <param name="index">The cluster index.</param>
            /// 
            public Cluster(TCollection owner, int index)
            {
                this.owner = owner;
                this.index = index;
            }
        }



        private IDistance<TData, TCentroids> distance;

        private double[] proportions;
        private TCentroids[] centroids;
        private TCluster[] clusters;


        /// <summary>
        ///   Initializes a new instance of the <see cref="KMeansClusterCollection"/> class.
        /// </summary>
        /// 
        /// <param name="k">The number of clusters K.</param>
        /// <param name="distance">The distance metric to consider.</param>
        /// 
        public ClusterCollection(int k, IDistance<TData, TCentroids> distance)
        {
            // To store centroids of the clusters
            this.proportions = new double[k];
            this.centroids = new TCentroids[k];
            this.clusters = new TCluster[k];
            this.Distance = distance;
            this.NumberOfOutputs = k;
        }

        /// <summary>
        ///   Gets or sets the distance function used to measure the distance
        ///   between a point and the cluster centroid in this clustering definition.
        /// </summary>
        /// 
        public IDistance<TData, TCentroids> Distance
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
        public TCentroids[] Centroids
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
            }
        }

        /// <summary>
        ///   Gets the proportion of samples in each cluster.
        /// </summary>
        /// 
        public virtual double[] Proportions
        {
            get { return proportions; }
        }


        /// <summary>
        ///   Gets the cluster definitions.
        /// </summary>
        /// 
        public TCluster[] Clusters { get { return clusters; } }


        /// <summary>
        ///   Returns the closest cluster to an input vector.
        /// </summary>
        /// 
        /// <param name="point">The input vector.</param>
        /// 
        /// <returns>
        ///   The index of the nearest cluster to the given data point.
        /// </returns>
        ///   
        [Obsolete("Please use Decide() instead.")]
        public virtual int Nearest(TData point)
        {
            return Decide(point);
        }

        /// <summary>
        ///   Returns the closest cluster to an input vector.
        /// </summary>
        /// 
        /// <param name="point">The input vector.</param>
        /// <param name="responses">The responses probabilities for each label.</param>
        /// 
        /// <returns>
        ///   The index of the nearest cluster to the given data point.
        /// </returns>
        ///   
        [Obsolete("Please use Scores() instead.")]
        public virtual int Nearest(TData point, out double[] responses)
        {
            int decision;
            responses = Scores(point, out decision);
            return decision;
        }

        /// <summary>
        ///   Returns the closest cluster to an input point.
        /// </summary>
        /// 
        /// <param name="point">The input vector.</param>
        /// <param name="response">A value between 0 and 1 representing
        ///   the confidence in the generated classification.</param>
        /// 
        /// <returns>
        ///   The index of the nearest cluster
        ///   to the given data point. </returns>
        ///   
        [Obsolete("Please use Score() instead.")]
        public virtual int Nearest(TData point, out double response)
        {
            int decision;
            response = Score(point, out decision);
            return decision;
        }

        /// <summary>
        ///   Returns the closest clusters to an input vector array.
        /// </summary>
        /// 
        /// <param name="points">The input vector array.</param>
        /// 
        /// <returns>
        ///   An array containing the index of the nearest cluster
        ///   to the corresponding point in the input array.</returns>
        ///   
        [Obsolete("Please use Decide() instead.")]
        public virtual int[] Nearest(TData[] points)
        {
            return Decide(points);
        }

        /// <summary>
        ///   Returns the closest clusters to an input vector array.
        /// </summary>
        /// 
        /// <param name="points">The input vector array.</param>
        /// <param name="responses">The responses probabilities for each label.</param>
        /// 
        /// <returns>
        ///   An array containing the index of the nearest cluster
        ///   to the corresponding point in the input array.</returns>
        ///   
        [Obsolete("Please use Scores() instead.")]
        public virtual int[] Nearest(TData[] points, out double[][] responses)
        {
            int[] decisions = new int[points.Length];
            responses = Scores(points, ref decisions);
            return decisions;
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
        public virtual double Distortion(TData[] data, int[] labels = null, double[] weights = null)
        {
            if (labels == null)
                labels = Decide(data);

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
        public virtual double[] Transform(TData[] points, int[] labels, double[] weights = null, double[] result = null)
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
        public virtual double[][] Transform(TData[] points, double[] weights = null, double[][] result = null)
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

        /// <summary>
        ///   Gets the number of clusters in the collection.
        /// </summary>
        /// 
        public int Count
        {
            get { return Clusters.Length; }
        }

        /// <summary>
        ///   Gets the cluster at the given index.
        /// </summary>
        /// 
        /// <param name="index">The index of the cluster. This should also be the class label of the cluster.</param>
        /// 
        /// <returns>An object holding information about the selected cluster.</returns>
        /// 
        public TCluster this[int index]
        {
            get { return clusters[index]; }
        }


        /// <summary>
        ///   Returns an enumerator that iterates through a collection.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// 
        public IEnumerator<TCluster> GetEnumerator()
        {
            foreach (var cluster in clusters)
                yield return cluster;
        }

        /// <summary>
        ///   Returns an enumerator that iterates through a collection.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// 
        IEnumerator IEnumerable.GetEnumerator()
        {
            return clusters.GetEnumerator();
        }


        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and a given
        /// <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// <returns>System.Double.</returns>
        public override double Score(TData input, int classIndex)
        {
            return -Distance.Distance(input, centroids[classIndex]);
        }
    }
}
