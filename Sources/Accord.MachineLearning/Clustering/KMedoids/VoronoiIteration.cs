// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza <cesarsouza at gmail.com>
// and other contributors, 2009-2017.
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
    using System.Threading;
    using System.Linq;
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   k-Medoids clustering using Voronoi iteration algorithm.
    /// </summary>
    /// 
    /// <remarks>
    /// <para> From Wikipedia:</para>
    /// <para>
    /// The k-medoids algorithm is a clustering algorithm related to the k-means algorithm and the medoidshift
    /// algorithm. Both the k-means and k-medoids algorithms are partitional (breaking the dataset up into groups)
    /// and both attempt to minimize the distance between points labeled to be in a cluster and a point designated
    /// as the center of that cluster. In contrast to the k-means algorithm, k-medoids chooses datapoints as centers
    /// (medoids or exemplars) and works with a generalization of the Manhattan Norm to define distance between
    /// datapoints instead of L2. This method was proposed in 1987[1] for the work with L1 norm and other distances.
    /// </para>
    /// <para>
    /// Voronoi iteration algorithm (or Lloyd algorithm) is one of possible implementations of the k-medoids
    /// clustering. It was suggested in the [2] and [3].
    /// </para>
    /// <para>
    /// [1] Kaufman, L. and Rousseeuw, P.J. (1987), Clustering by means of Medoids, in Statistical Data Analysis 
    /// Based on the L1–Norm and Related Methods, edited by Y. Dodge, North-Holland, 405–416.
    /// [2] T. Hastie, R. Tibshirani, and J.Friedman.The Elements of Statistical Learning, Springer (2001), 468–469.
    /// [3] H.S.Park , C.H.Jun, A simple and fast algorithm for K-medoids clustering, Expert Systems with Applications,
    /// 36, (2) (2009), 3336–3341.
    /// </para>
    /// </remarks>
    /// 
    /// <seealso cref="VoronoiIteration"/>
    /// <seealso cref="KMeans"/>
    /// <seealso cref="MeanShift"/>
    /// 
    /// <example>
    ///   How to perform K-Medoids clustering with Voronoi iteration algorithm.
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\Clustering\KMedoidsVITest.cs" region="doc_learn" />
    /// </example>
    /// 
    [Serializable]
    public class VoronoiIteration<T> : KMedoids<T>
    {

        /// <summary>
        ///   Initializes a new instance of VoronoiIteration algorithm
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide input data.</param>
        /// <param name="distance">The distance function to use. Default is to
        /// use the <see cref="Accord.Math.Distance.Manhattan(double[], double[])"/> distance.</param>
        /// 
        public VoronoiIteration(int k, IDistance<T[]> distance)
            : base(k, distance)
        {
            Initialization = Seeding.Uniform;
            Tolerance = 1e-5;
            MaxIterations = 100;
        }



        /// <summary>
        /// Helper class - cluster infromation.
        /// </summary>
        private class ClusterInfo
        {
            /// <summary>
            /// Index of the medoid point for this cluster.
            /// </summary>
            public int MedoidIndex { get; set; }

            /// <summary>
            /// Cost of this cluster, i.e. sum of distances of all
            /// cluster member points to the medoid point.
            /// </summary>
            public double Cost { get; set; }

            /// <summary>
            /// Set of member point indices.
            /// </summary>
            public HashSet<int> PointIndices { get; private set; }

            /// <summary>
            /// Initializes new ClusterInfo object.
            /// </summary>
            /// <param name="medoidIndex"></param>
            public ClusterInfo(int medoidIndex)
            {
                MedoidIndex = medoidIndex;
                PointIndices = new HashSet<int>();
                Reset();
            }

            /// <summary>
            /// Reset object to the initial state.
            /// </summary>
            public void Reset()
            {
                Cost = 0.0;
                PointIndices.Clear();
                PointIndices.Add(MedoidIndex);
            }
        }

        /// <summary>
        ///   Implementation of the Voronoi Iteration algorithm.
        /// </summary>
        /// 
        protected override int[] Compute(T[][] x, int[] labels, int[] currentMedoidIndicesArray)
        {
            var currentMedoidIndices = new HashSet<int>(currentMedoidIndicesArray);
            if (currentMedoidIndices.Count < currentMedoidIndicesArray.Length)
                throw new Exception("Some medoids are not unique");

            var clusters = new ClusterInfo[K];

            // Create clusters
            int clusterIndex = 0;
            foreach (int medoidIndex in currentMedoidIndices)
            {
                clusters[clusterIndex] = new ClusterInfo(medoidIndex);
                clusterIndex++;
            }

            for (; Iterations < MaxIterations; Iterations++)
            {
                // Prepare for the new iteration
                currentMedoidIndices.Clear();
                clusterIndex = 0;
                foreach (ClusterInfo cluster in clusters)
                {
                    cluster.Reset();
                    currentMedoidIndices.Add(cluster.MedoidIndex);
                    labels[cluster.MedoidIndex] = clusterIndex;
                    clusterIndex++;
                }

                // Assign points to clusters
                Parallel.For(0, x.Length, ParallelOptions, pointIndex =>
                {
                    // Skip current medoids
                    if (currentMedoidIndices.Contains(pointIndex))
                        return;

                    // Find first cluster with minimum cost
                    T[] point = x[pointIndex];
                    int minCostClusterIndex = 0;
                    double minCost = Distance.Distance(point, x[clusters[0].MedoidIndex]);
                    for (int j = 1; j < clusters.Length; j++)
                    {
                        double cost = Distance.Distance(point, x[clusters[j].MedoidIndex]);
                        if (cost < minCost)
                        {
                            minCost = cost;
                            minCostClusterIndex = j;
                        }
                    }

                    labels[pointIndex] = minCostClusterIndex;

                    // Update cluster info
                    ClusterInfo cluster = clusters[minCostClusterIndex];
                    lock (cluster)
                    {
                        cluster.PointIndices.Add(pointIndex);
                        cluster.Cost += minCost;
                    }
                });

                double initialTotalCost = clusters.Sum(cluster => cluster.Cost);

                // Find best medoid in the each cluster
                int improvementCount = 0;
                Parallel.For(0, clusters.Length, ParallelOptions, i =>
                {
                    ClusterInfo cluster = clusters[i];
                    int currentMedoidIndex = cluster.MedoidIndex;
                    T[] currentMedoid = x[currentMedoidIndex];
                    int minCostMedoidIndex = currentMedoidIndex;
                    double minCost = cluster.Cost;

                    foreach (int newMedoidIndex in cluster.PointIndices)
                    {
                        if (newMedoidIndex != currentMedoidIndex)
                        {
                            T[] newMedoid = x[newMedoidIndex];
                            double newCost = cluster.PointIndices.Sum(pointIndex => Distance.Distance(x[pointIndex], newMedoid));
                            if (newCost < minCost)
                            {
                                minCostMedoidIndex = newMedoidIndex;
                                minCost = newCost;
                            }
                        }
                    }

                    if (minCostMedoidIndex != currentMedoidIndex)
                    {
                        cluster.PointIndices.Remove(minCostMedoidIndex);
                        cluster.MedoidIndex = minCostMedoidIndex;
                        cluster.PointIndices.Add(currentMedoidIndex);
                        cluster.Cost = minCost;
                        Interlocked.Increment(ref improvementCount);
                    }
                });

                // Evaluate improvement
                if (improvementCount == 0)
                    break;

                double finalTotalCost = clusters.Sum(cluster => cluster.Cost);
                if (finalTotalCost >= initialTotalCost)
                    break;
            }

            // Assign medoids
            for (int i = 0; i < Clusters.Centroids.Length; i++)
                Clusters.Centroids[i] = x[clusters[i].MedoidIndex];

            return labels;
        }
    }

    /// <summary>
    ///   k-Medoids clustering using Voronoi iteration algorithm.
    /// </summary>
    /// 
    /// <remarks>
    /// <para> From Wikipedia:</para>
    /// <para>
    /// The k-medoids algorithm is a clustering algorithm related to the k-means algorithm and the medoidshift
    /// algorithm. Both the k-means and k-medoids algorithms are partitional (breaking the dataset up into groups)
    /// and both attempt to minimize the distance between points labeled to be in a cluster and a point designated
    /// as the center of that cluster. In contrast to the k-means algorithm, k-medoids chooses datapoints as centers
    /// (medoids or exemplars) and works with a generalization of the Manhattan Norm to define distance between
    /// datapoints instead of L2. This method was proposed in 1987[1] for the work with L1 norm and other distances.
    /// </para>
    /// <para>
    /// Voronoi iteration algorithm (or Lloyd algorithm) is one of possible implementations of the k-medoids
    /// clustering. It was suggested in the [2] and [3].
    /// </para>
    /// <para>
    /// [1] Kaufman, L. and Rousseeuw, P.J. (1987), Clustering by means of Medoids, in Statistical Data Analysis 
    /// Based on the L1–Norm and Related Methods, edited by Y. Dodge, North-Holland, 405–416.
    /// [2] T. Hastie, R. Tibshirani, and J.Friedman.The Elements of Statistical Learning, Springer (2001), 468–469.
    /// [3] H.S.Park , C.H.Jun, A simple and fast algorithm for K-medoids clustering, Expert Systems with Applications,
    /// 36, (2) (2009), 3336–3341.
    /// </para>
    /// </remarks>
    /// 
    /// <seealso cref="VoronoiIteration{T}"/>
    /// 
    [Serializable]
    public class VoronoiIteration : VoronoiIteration<double>
    {
        /// <summary>
        ///   Initializes a new instance of k-Medoids algorithm
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide input data.</param>    
        /// 
        public VoronoiIteration(int k)
            : base(k, new Accord.Math.Distances.Euclidean())
        {
        }
    }

}
