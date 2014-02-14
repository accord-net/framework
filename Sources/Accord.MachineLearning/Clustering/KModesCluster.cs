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
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    ///   k-Modes cluster collection.
    /// </summary>
    /// 
    /// <seealso cref="KModes{T}"/>
    /// 
    [Serializable]
    public class KModesClusterCollection<TData> : IClusterCollection<TData, KModesCluster<TData>>
        where TData : ICloneable
    {

        private List<KModesCluster<TData>> clusters;

        private double[] proportions;
        private TData[] centroids;
        private Func<TData, TData, double> distance;

        /// <summary>
        ///   Gets the proportion of samples in each cluster.
        /// </summary>
        /// 
        public double[] Proportions
        {
            get { return proportions; }
        }

        /// <summary>
        ///   Gets or sets the distance function used
        ///   as a distance metric between data points.
        /// </summary>
        /// 
        public Func<TData, TData, double> Distance
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
        public TData[] Centroids
        {
            get { return centroids; }
            set
            {
                if (value == centroids)
                    return;

                if (value == null)
                    throw new ArgumentNullException("value");

                int k = clusters.Count;

                if (value.Length != k)
                    throw new ArgumentException("The number of centroids should be equal to K.", "value");

                // Make a deep copy of the
                // input centroids vector.
                for (int i = 0; i < k; i++)
                    centroids[i] = (TData)value[i].Clone();

                // Reset derived information
                proportions = new double[k];
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="KModesClusterCollection&lt;TData&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="k">The number of clusters K.</param>
        /// <param name="distance">The distance metric to use.</param>
        /// 
        public KModesClusterCollection(int k, Func<TData, TData, double> distance)
        {
            // To store centroids of the clusters
            this.proportions = new double[k];
            this.centroids = new TData[k];
            this.distance = distance;

            clusters = new List<KModesCluster<TData>>();
            for (int i = 0; i < k; i++) clusters.Add(new KModesCluster<TData>(this, i));
        }


        /// <summary>
        ///   Returns the closest cluster to an input vector.
        /// </summary>
        /// 
        /// <param name="point">The input vector.</param>
        /// <returns>
        ///   The index of the nearest cluster
        ///   to the given data point. </returns>
        ///   
        public int Nearest(TData point)
        {
            int min_cluster = 0;
            double min_distance = distance(point, centroids[0]);

            for (int i = 1; i < centroids.Length; i++)
            {
                double dist = distance(point, centroids[i]);
                if (dist < min_distance)
                {
                    min_distance = dist;
                    min_cluster = i;
                }
            }

            return min_cluster;
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
        public int[] Nearest(TData[] points)
        {
            int[] labels = new int[points.Length];
            for (int i = 0; i < points.Length; i++)
                labels[i] = Nearest(points[i]);

            return labels;
        }


        /// <summary>
        ///   Calculates the average square distance from the data points
        ///   to the clusters' centroids.
        /// </summary>
        /// 
        /// <remarks>
        ///   The average distance from centroids can be used as a measure
        ///   of the "goodness" of the clusterization. The more the data
        ///   are aggregated around the centroids, the less the average
        ///   distance.
        /// </remarks>
        /// 
        /// <returns>
        ///   The average square distance from the data points to the
        ///   clusters' centroids.
        /// </returns>
        /// 
        public double Distortion(TData[] data)
        {
            double error = 0.0;

            for (int i = 0; i < data.Length; i++)
                error += distance(data[i], centroids[Nearest(data[i])]);

            return error / (double)data.Length;
        }

        /// <summary>
        ///   Calculates the average square distance from the data points
        ///   to the clusters' centroids.
        /// </summary>
        /// 
        /// <remarks>
        ///   The average distance from centroids can be used as a measure
        ///   of the "goodness" of the clusterization. The more the data
        ///   are aggregated around the centroids, the less the average
        ///   distance.
        /// </remarks>
        /// 
        /// <returns>
        ///   The average square distance from the data points to the
        ///   clusters' centroids.
        /// </returns>
        /// 
        public double Distortion(TData[] data, int[] labels)
        {
            double error = 0.0;

            for (int i = 0; i < data.Length; i++)
                error += distance(data[i], centroids[labels[i]]);

            return error / (double)data.Length;
        }

        /// <summary>
        ///   Gets the number of clusters in the collection.
        /// </summary>
        /// 
        public int Count
        {
            get { return clusters.Count; }
        }

        /// <summary>
        ///   Gets the cluster at the given index.
        /// </summary>
        /// 
        /// <param name="index">The index of the cluster. This should also be the class label of the cluster.</param>
        /// 
        /// <returns>An object holding information about the selected cluster.</returns>
        /// 
        public KModesCluster<TData> this[int index]
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
        public IEnumerator<KModesCluster<TData>> GetEnumerator()
        {
            return clusters.GetEnumerator();
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

    }

    /// <summary>
    ///   k-Modes' cluster.
    /// </summary>
    /// 
    /// <seealso cref="KModes{T}"/>
    /// <seealso cref="KModesClusterCollection{T}"/>
    /// 
    [Serializable]
    public class KModesCluster<TData>
        where TData : ICloneable
    {
        private KModesClusterCollection<TData> owner;
        private int index;

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
        public TData Mode
        {
            get { return owner.Centroids[index]; }
        }

        /// <summary>
        ///   Gets the proportion of samples in the cluster.
        /// </summary>
        /// 
        public double Proportion
        {
            get { return owner.Proportions[index]; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="KModesCluster&lt;TData&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="owner">The owner.</param>
        /// <param name="index">The cluster index.</param>
        /// 
        public KModesCluster(KModesClusterCollection<TData> owner, int index)
        {
            this.owner = owner;
            this.index = index;
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
            TData centroid = Mode;

            double error = 0.0;
            for (int i = 0; i < data.Length; i++)
                error += owner.Distance(data[i], centroid);
            return error / (double)data.Length;
        }
    }

}
