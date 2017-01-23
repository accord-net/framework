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
    using System;
    using System.Collections.Generic;
    using Accord.Math;
    using Accord.Math.Distances;
    using Math.Optimization;
    using Statistics;
    using System.Threading.Tasks;

    /// <summary>
    ///   Balanced K-Means algorithm.
    /// </summary>
    /// 
    /// <remarks>
    ///   The Balanced k-Means algorithm attempts to find a clustering where each cluster
    ///   has approximately the same number of data points. The Balanced k-Means implementation
    ///   used in the framework uses the <see cref="Munkres"/> algorithm to solve the assignment
    ///   problem thus enforcing balance between the clusters.
    /// </remarks>
    /// 
    /// <example>
    ///   How to perform clustering with Balanced K-Means.
    ///   
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\Clustering\BalancedKMeansTest.cs" region="doc_learn" />
    /// </example>
    /// 
    /// <seealso cref="KMeans"/>
    /// <seealso cref="BinarySplit"/>
    /// <seealso cref="GaussianMixtureModel"/>
    /// 
    [Serializable]
    public class BalancedKMeans : KMeans
    {

        /// <summary>
        ///   Gets the labels assigned for each data point in the last 
        ///   call to <see cref="Learn(double[][], double[])"/>.
        /// </summary>
        /// 
        /// <value>The labels.</value>
        /// 
        public int[] Labels { get; private set; }

        /// <summary>
        ///   Initializes a new instance of the Balanced K-Means algorithm.
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide the input data into.</param>    
        /// <param name="distance">The distance function to use. Default is to
        /// use the <see cref="Accord.Math.Distance.SquareEuclidean(double[], double[])"/> distance.</param>
        /// 
        public BalancedKMeans(int k, IDistance<double[]> distance)
            : base(k, distance)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the Balanced K-Means algorithm.
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide the input data into.</param>    
        /// 
        public BalancedKMeans(int k)
            : base(k) { }


        /// <summary>
        /// Learns a model that can map the given inputs to the desired outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="weights">The weight of importance for each input sample.</param>
        /// <returns>A model that has learned how to produce suitable outputs
        /// given the input data <paramref name="x" />.</returns>
        public override KMeansClusterCollection Learn(double[][] x, double[] weights = null)
        {
            // Initial argument checking
            if (x == null)
                throw new ArgumentNullException("x");

            if (x.Length < K)
                throw new ArgumentException("Not enough points. There should be more points than the number K of clusters.");

            if (weights == null)
            {
                weights = Vector.Ones(x.Length);
            }
            else
            {
                if (x.Length != weights.Length)
                    throw new ArgumentException("Data weights vector must be the same length as data samples.");
            }

            double weightSum = weights.Sum();
            if (weightSum <= 0)
                throw new ArgumentException("Not enough points. There should be more points than the number K of clusters.");

            if (!x.IsRectangular())
                throw new DimensionMismatchException("data", "The points matrix should be rectangular. The vector at position {} has a different length than previous ones.");

            int k = this.K;
            int rows = x.Length;
            int cols = x[0].Length;

            // Perform a random initialization of the clusters
            // if the algorithm has not been initialized before.
            //
            if (this.Clusters.Centroids[0] == null)
            {
                Randomize(x);
            }

            // Initial variables
            int[] labels = new int[rows];
            double[] count = new double[k];
            double[][] centroids = Clusters.Centroids;
            double[][] newCentroids = new double[k][];
            for (int i = 0; i < newCentroids.Length; i++)
                newCentroids[i] = new double[cols];

            Object[] syncObjects = new Object[K];
            for (int i = 0; i < syncObjects.Length; i++)
                syncObjects[i] = new Object();

            Iterations = 0;

            bool shouldStop = false;

            var m = new Munkres(x.Length, x.Length);
            double[][] costMatrix = m.CostMatrix;

            while (!shouldStop) // Main loop
            {
                Array.Clear(count, 0, count.Length);
                for (int i = 0; i < newCentroids.Length; i++)
                    Array.Clear(newCentroids[i], 0, newCentroids[i].Length);
                for (int i = 0; i < labels.Length; i++)
                    labels[i] = -1;

                // Set the cost matrix for Munkres algorithm
                for (int i = 0; i < costMatrix.Length; i++)
                    for (int j = 0; j < costMatrix[i].Length; j++)
                        costMatrix[i][j] = Distance.Distance(x[j], centroids[i % k]);

                //string str = costMatrix.ToCSharp();

                m.Minimize(); // solve the assignment problem

                for (int i = 0; i < x.Length; i++)
                {
                    if (m.Solution[i] >= 0)
                        labels[(int)m.Solution[i]] = i % k;
                }

                // For each point in the data set,
                Parallel.For(0, x.Length, ParallelOptions, i =>
                {
                    // Get the point
                    double[] point = x[i];
                    double weight = weights[i];

                    // Get the nearest cluster centroid
                    int c = labels[i];

                    if (c >= 0)
                    {
                        // Get the closest cluster centroid
                        double[] centroid = newCentroids[c];

                        lock (syncObjects[c])
                        {
                            // Increase the cluster's sample counter
                            count[c] += weight;

                            // Accumulate in the cluster centroid
                            for (int j = 0; j < point.Length; j++)
                                centroid[j] += point[j] * weight;
                        }
                    }
                });

                // Next we will compute each cluster's new centroid
                //  by dividing the accumulated sums by the number of
                //  samples in each cluster, thus averaging its members.
                Parallel.For(0, newCentroids.Length, ParallelOptions, i =>
                {
                    double sum = count[i];

                    if (sum > 0)
                    {
                        for (int j = 0; j < newCentroids[i].Length; j++)
                            newCentroids[i][j] /= sum;
                    }
                });

                // The algorithm stops when there is no further change in the
                //  centroids (relative difference is less than the threshold).
                shouldStop = converged(centroids, newCentroids);

                // go to next generation
                Parallel.For(0, centroids.Length, ParallelOptions, i =>
                {
                    for (int j = 0; j < centroids[i].Length; j++)
                        centroids[i][j] = newCentroids[i][j];
                });
            }

            for (int i = 0; i < Clusters.Centroids.Length; i++)
            {
                // Compute the proportion of samples in the cluster
                Clusters.Proportions[i] = count[i] / weightSum;
            }

            this.Labels = labels;

            ComputeInformation(x, labels);

            return Clusters;
        }

    }
}
