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
    using Accord.Collections;
    using Accord.Math;
    using System;
    using System.Collections.Generic;
    using Accord.Math.Distances;
    using System.Collections;
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Mean shift cluster collection.
    /// </summary>
    /// 
    /// <seealso cref="MeanShift"/>
    /// 
    [Serializable]
    public class MeanShiftClusterCollection : MulticlassClassifierBase<double[]>,
        IClusterCollectionEx<double[], MeanShiftClusterCollection.MeanShiftCluster>
    {
        MeanShiftCluster.ClusterCollection collection;

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
        public class MeanShiftCluster : Cluster<MeanShiftClusterCollection, double[], MeanShiftCluster>
        {
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
        {
            this.collection = new MeanShiftCluster.ClusterCollection(this, k);
            this.algorithm = algorithm;
            this.tree = tree;
            this.modes = modes;
        }

        /// <summary>
        /// Computes a class-label decision for a given <paramref name="input" />.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// one of the <see cref="ITransform.NumberOfOutputs" /> possible classes.</param>
        /// <returns>A class-label that best described <paramref name="input" /> according
        /// to this classifier.</returns>
        public override int Decide(double[] input)
        {
            // the tree contains the class label as the value for the seed point.
            return tree.Nearest(input).Value;
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
        public double Distortion(double[][] data, int[] labels = null, double[] weights = null)
        {
            if (labels != null)
                throw new NotSupportedException();

            if (weights == null)
                weights = Vector.Ones(data.Length);

            double error = 0.0;

            // TODO: Use ParallelOptions in the loop below
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
        public double[] Transform(double[][] points, int[] labels, double[] weights = null, double[] result = null)
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



        // Using composition over inheritance to achieve the closest as possible effect to a Mixin
        // in C# - unfortunately needs a lot a boilerplate code to rewrire the interface methods to
        // where their actual implementation is


        /// <summary>
        /// Gets the number of clusters in the collection.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return ((IClusterCollectionEx<double[], MeanShiftCluster>)collection).Count;
            }
        }

        /// <summary>
        /// Gets the collection of clusters currently modeled by the clustering algorithm.
        /// </summary>
        /// <value>The clusters.</value>
        public MeanShiftCluster[] Clusters
        {
            get
            {
                return ((IClusterCollectionEx<double[], MeanShiftCluster>)collection).Clusters;
            }
        }

        /// <summary>
        /// Gets the proportion of samples in each cluster.
        /// </summary>
        /// 
        public double[] Proportions
        {
            get
            {
                return ((IClusterCollectionEx<double[], MeanShiftCluster>)collection).Proportions;
            }
        }

        /// <summary>
        /// Gets the <see cref="MeanShiftCluster"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>GaussianCluster.</returns>
        public MeanShiftCluster this[int index]
        {
            get
            {
                return ((IClusterCollectionEx<double[], MeanShiftCluster>)collection)[index];
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<MeanShiftCluster> GetEnumerator()
        {
            return ((IClusterCollectionEx<double[], MeanShiftCluster>)collection).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IClusterCollectionEx<double[], MeanShiftCluster>)collection).GetEnumerator();
        }
    }

}
