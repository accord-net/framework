﻿// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    using Accord.MachineLearning.Structures;
    using Accord.Math;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    ///   Mean shift cluster collection.
    /// </summary>
    /// 
    /// <seealso cref="MeanShift"/>
    /// 
    [Serializable]
    public class MeanShiftClusterCollection : ClusterCollection<double[], MeanShiftClusterCollection.MeanShiftCluster>
    {
        private MeanShift algorithm;
        private KDTree<int> tree;
        private double[][] modes;

        /// <summary>
        ///   Mean shift cluster.
        /// </summary>
        /// 
        /// <seealso cref="MeanShift"/>
        /// <seealso cref="MeanShiftClusterCollection"/>
        /// 
        [Serializable]
        public class MeanShiftCluster : Cluster<MeanShiftClusterCollection>
        {
            /// <summary>
            ///   Initializes a new instance of the <see cref="MeanShiftCluster"/> class.
            /// </summary>
            /// 
            /// <param name="owner">The owner collection.</param>
            /// <param name="index">The cluster index.</param>
            /// 
            public MeanShiftCluster(MeanShiftClusterCollection owner, int index)
                : base(owner, index)
            {

            }
        }

        /// <summary>
        ///   Gets the cluster modes.
        /// </summary>
        /// 
        public double[][] Modes { get { return modes; } }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MeanShiftClusterCollection"/> class.
        /// </summary>
        /// 
        public MeanShiftClusterCollection(MeanShift algorithm, int k, KDTree<int> tree, double[][] modes)
            : base(k, tree.Distance)
        {
            this.algorithm = algorithm;
            this.tree = tree;
            this.modes = modes;
        }

        /// <summary>
        ///   Returns the closest cluster to an input point.
        /// </summary>v
        /// 
        /// <param name="point">The input vector.</param>
        /// <returns>
        ///   The index of the nearest cluster
        ///   to the given data point. </returns>
        /// 
        public override int Nearest(double[] point)
        {
            // the tree contains the class label as the value for the seed point.
            return tree.Nearest(point).Value;
        }

        /// <summary>
        ///   Returns the closest cluster to an input point.
        /// </summary>
        /// 
        /// <param name="point">The input vector.</param>
        /// <param name="response">The responses for each of the cluster labels.</param>
        /// 
        /// <returns>
        ///   The index of the nearest cluster
        ///   to the given data point. </returns>
        /// 
        public override int Nearest(double[] point, out double[] response)
        {
            // the tree contains the class label as the value for the seed point.
            ICollection<KDTreeNodeDistance<int>> values =
                tree.Nearest(point, algorithm.Bandwidth, algorithm.Maximum);

            response = new double[algorithm.Clusters.Count];
            int[] counts = new int[algorithm.Clusters.Count];
            double sum = 0;
            foreach (var value in values)
            {
                int j = value.Node.Value;
                response[j] += 1 / (1 + value.Distance);
                counts[j] += 1;
                sum += response[j];
            }

            Elementwise.Divide(response, counts, result: response);

            return response.ArgMax();
        }

        /// <summary>
        ///   Returns the closest cluster to an input point.
        /// </summary>
        /// 
        /// <param name="point">The input vector.</param>
        /// <returns>
        ///   The index of the nearest cluster
        ///   to the given data point. </returns>
        /// 
        public override int[] Nearest(double[][] point)
        {
            int[] labels = new int[point.Length];
            Parallel.For(0, labels.Length, i =>
            {
                labels[i] = Nearest(point[i]);
            });
            return labels;
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
        public override double Distortion(double[][] data, int[] labels = null, double[] weights = null)
        {
            if (labels != null)
                throw new NotSupportedException();

            if (weights == null)
                weights = Vector.Ones(data.Length);

            double error = 0.0;

#if !NET35
            Parallel.For(0, data.Length,

                () => 0.0,

                (i, state, acc) =>
                {
                    double distance;
                    tree.Nearest(data[i], out distance);
                    return acc + weights[i] * distance;
                },

                acc =>
                {
                    lock (labels)
                        error += acc;
                });
#else
            for (int i = 0; i < data.Length; i++)
            {

                double distance;
                tree.Nearest(data[i], out distance);
                error += weights[i] * distance;
            }
#endif
            return error / weights.Sum();
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
        /// <returns>
        ///   A vector containing the distance between the input points and the clusters.
        /// </returns>
        /// 
        /// <exception cref="System.NotSupportedException"></exception>
        /// 
        public override double[] Transform(double[][] points, int[] labels, double[] weights = null, double[] result = null)
        {
            if (result == null)
                result = new double[points.Length];

            if (labels != null)
                throw new NotSupportedException();

            if (weights == null)
                weights = Vector.Ones(points.Length);

            Parallel.For(0, result.Length, i =>
            {
                double distance;
                tree.Nearest(points[i], out distance);
                result[i] = weights[i] * distance;
            });

            return result;
        }
    }

}
