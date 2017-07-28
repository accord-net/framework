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
    using System.Collections.ObjectModel;
    using Accord.Math;
    using Accord.Statistics;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Math.Distances;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Diagnostics;

    /// <summary>
    ///   k-Medoids clustering using PAM (Partition Around Medoids) algorithm.
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
    /// The most common realisation of k-medoid clustering is the Partitioning Around Medoids (PAM) algorithm.
    /// PAM uses a greedy search which may not find the optimum solution, but it is faster than exhaustive search.
    /// </para>
    /// <para>
    /// [1] Kaufman, L. and Rousseeuw, P.J. (1987), Clustering by means of Medoids, in Statistical Data Analysis 
    /// Based on the L1–Norm and Related Methods, edited by Y. Dodge, North-Holland, 405–416.
    /// </para>
    /// </remarks>
    /// 
    /// <seealso cref="PartitioningAroundMedoids"/>
    /// <seealso cref="KMeans"/>
    /// <seealso cref="MeanShift"/>
    /// 
    /// <example>
    ///   How to perform clustering with K-Medoids.
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\Clustering\KMedoidsTest.cs" region="doc_learn" />
    /// </example>
    /// 
    [Serializable]
    public class PartitioningAroundMedoids<T> : ParallelLearningBase,
        IUnsupervisedLearning<KMedoidsClusterCollection<T>, T[], int>,
#pragma warning disable 0618
        IClusteringAlgorithm<T[]>
#pragma warning restore 0618
    {

        private KMedoidsClusterCollection<T> clusters;

        /// <summary>
        ///   Gets the clusters found by k-Medoids.
        /// </summary>
        /// 
        public KMedoidsClusterCollection<T> Clusters
        {
            get { return clusters; }
        }

        /// <summary>
        ///   Gets the number of clusters.
        /// </summary>
        /// 
        public int K { get { return clusters.Count; } }

        /// <summary>
        ///   Gets the dimensionality of the data space.
        /// </summary>
        /// 
        public int Dimension
        {
            get { return clusters.NumberOfInputs; }
        }

        /// <summary>
        ///   Gets or sets whether the clustering distortion error (the
        ///   average distance between all data points and the cluster
        ///   centroids) should be computed at the end of the algorithm.
        ///   The result will be stored in <see cref="Error"/>. Default is true.
        /// </summary>
        /// 
        public bool ComputeError { get; set; }

        /// <summary>
        ///   Gets or sets the distance function used
        ///   as a distance metric between data points.
        /// </summary>
        /// 
        public IDistance<T[], T[]> Distance
        {
            get { return clusters.Distance; }
            set { clusters.Distance = value; }
        }

        /// <summary>
        ///   Gets or sets the maximum number of iterations to
        ///   be performed by the method. If set to zero, no
        ///   iteration limit will be imposed. Default is 0.
        /// </summary>
        /// 
        public int MaxIterations { get; set; }

        /// <summary>
        ///   Gets or sets the relative convergence threshold
        ///   for stopping the algorithm. Default is 1e-5.
        /// </summary>
        /// 
        public double Tolerance { get; set; }

        /// <summary>
        ///   Gets the number of iterations performed in the
        ///   last call to this class' Compute methods.
        /// </summary>
        /// 
        public int Iterations { get; private set; }

        /// <summary>
        ///   Gets the cluster distortion error (the average distance 
        ///   between data points and the cluster centroids) after the 
        ///   last call to this class' Compute methods.
        /// </summary>
        /// 
        public double Error { get; private set; }

        /// <summary>
        ///   Gets or sets the strategy used to initialize the
        ///   centroids of the clustering algorithm. Default is
        ///   <see cref="Seeding.KMeansPlusPlus"/>.
        /// </summary>
        /// 
        public Seeding Initialization { get; set; }

        /// <summary>
        ///   Initializes a new instance of PartitioningAroundMedoids algorithm
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide input data.</param>       
        /// <param name="distance">The distance function to use. Default is to
        /// use the <see cref="Accord.Math.Distance.SquareEuclidean(double[], double[])"/> distance.</param>
        /// 
        [Obsolete("Please specify the distance function using classes instead of lambda functions.")]
        public PartitioningAroundMedoids(int k, Func<T[], T[], double> distance)
            : this(k, Accord.Math.Distance.GetDistance(distance))
        {
        }

        /// <summary>
        ///   Initializes a new instance of PartitioningAroundMedoids algorithm
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide input data.</param>
        /// <param name="distance">The distance function to use. Default is to
        /// use the <see cref="Accord.Math.Distance.Manhattan(double[], double[])"/> distance.</param>
        /// 
        public PartitioningAroundMedoids(int k, IDistance<T[]> distance)
        {
            if (k <= 0)
                throw new ArgumentOutOfRangeException("k");

            if (distance == null)
                throw new ArgumentNullException("distance");

            // Create the object-oriented structure to hold
            //  information about the k-Medoids' clusters.
            clusters = new KMedoidsClusterCollection<T>(k, distance);

            Initialization = Seeding.PamBuild;
            Tolerance = 1e-5;
            MaxIterations = 100;
        }

        /// <summary>
        ///   Divides the input data into K clusters. 
        /// </summary>     
        /// 
        /// <param name="points">The data where to compute the algorithm.</param>
        /// 
        [Obsolete("Please use Learn(x) instead.")]
        public int[] Compute(T[][] points)
        {
            var clusters = Learn(points);
            return clusters.Decide(points);
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the desired outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="weights">The weight of importance for each input sample.</param>
        /// <returns>A model that has learned how to produce suitable outputs
        /// given the input data <paramref name="x" />.</returns>
        /// <exception cref="ArgumentNullException">points</exception>
        /// <exception cref="ArgumentException">Not enough points. There should be more points than the number K of clusters.</exception>
        public KMedoidsClusterCollection<T> Learn(T[][] x, double[] weights = null)
        {
            // Initial argument checking
            if (x == null)
                throw new ArgumentNullException("points");

            if (x.Length < K)
                throw new ArgumentException("Not enough points. There should be more points than the number K of clusters.");

            int rows = x.Length;
            int cols = x[0].Length;

            // Perform initialization of the clusters
            int[] currentMedoidIndicesArray = Clusters.Randomize(x, Initialization, ParallelOptions);

            // Detect initial medoid indices
            if (currentMedoidIndicesArray == null)
            {
                currentMedoidIndicesArray = Enumerable.Repeat(-1, K).ToArray();
                Parallel.For(0, x.Length, ParallelOptions, i =>
                {
                    T[] point = x[i];
                    for (int j = 0; j < K; ++j)
                    {
                        if (Clusters.Centroids[j].IsEqual(point))
                        {
                            int prev = Interlocked.CompareExchange(ref currentMedoidIndicesArray[j], i, -1);
                            if (prev != -1)
                                throw new Exception($"Duplicate medoid #{j} detected: {prev} and {i}");
                            break;
                        }
                    }
                });
            }

            for (int i = 0; i < K; ++i)
            {
                if (currentMedoidIndicesArray[i] == -1)
                {
                    throw new Exception($"Medoid #{i} not found.");
                }
            }

            var currentMedoidIndices = new HashSet<int>(currentMedoidIndicesArray);
            if (currentMedoidIndices.Count < currentMedoidIndicesArray.Length)
            {
                throw new Exception("Some medoids are not unique");
            }

            Iterations = 0;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////
            int[] pointClusterIndices = new int[x.Length];

            // Special case - one medoid
            if (K == 1)
            {
                double[] costs = new double[x.Length];
                for (int i = 0; i < x.Length; i++)
                {
                    double cost = 0.0;
                    for (int j = 0; j < x.Length; j++)
                        cost += Distance.Distance(x[i], x[j]);
                    costs[i] = cost;
                }
                int minCostPointIndex = 0;
                for (int i = 1; i < costs.Length; i++)
                {
                    if (costs[i] < costs[minCostPointIndex])
                        minCostPointIndex = i;
                }
                Clusters.Centroids[0] = x[minCostPointIndex];
                return Clusters;
            }

            var secondClusterDistance = new double[x.Length];

            for (; Iterations < MaxIterations; Iterations++)
            {
                // Assing points to clusters
                Parallel.For(0, x.Length, ParallelOptions, pointIndex =>
                {
                    int secondMinCostClusterIndex = -1;
                    double secondMinCost = double.PositiveInfinity;
                    int minCostClusterIndex = 0;
                    int medoidIndex = currentMedoidIndicesArray[minCostClusterIndex];
                    double minCost = Distance.Distance(x[pointIndex], x[medoidIndex]);
                    for (int i = 1; i < currentMedoidIndicesArray.Length; i++)
                    {
                        int medoidPointIndex = currentMedoidIndicesArray[i];
                        double cost = Distance.Distance(x[pointIndex], x[medoidPointIndex]);
                        if (cost < minCost)
                        {
                            secondMinCost = minCost;
                            minCost = cost;
                            secondMinCostClusterIndex = minCostClusterIndex;
                            minCostClusterIndex = i;
                        }
                        else if (cost < secondMinCost)
                        {
                            secondMinCost = cost;
                            secondMinCostClusterIndex = medoidPointIndex;
                        }
                    }
                    pointClusterIndices[pointIndex] = minCostClusterIndex;
                    secondClusterDistance[pointIndex] = secondMinCost;
                });

                // Compute total cost
                var totalCost = 0.0;
                Parallel.For(0, x.Length, i =>
                {
                    int clusterIndex = pointClusterIndices[i];
                    int medoidPointIndex = currentMedoidIndicesArray[clusterIndex];
                    double cost = Distance.Distance(x[i], x[medoidPointIndex]);
                    InterlockedEx.Add(ref totalCost, cost);
                });

                // Item = i, Item2 = h, Item3 = T[i,h]
                var minTih = Tuple.Create(-1, -1, double.PositiveInfinity);
                Parallel.For(0, x.Length, ParallelOptions, h =>
                {
                    // Skip current medoids
                    if (currentMedoidIndices.Contains(h)) return;

                    for (var i = 0; i < currentMedoidIndicesArray.Length; i++)
                    {
                        var pointIIndex = currentMedoidIndicesArray[i];

                        // Compute T[i, h]
                        var tih = 0.0;
                        Parallel.For(0, x.Length, ParallelOptions, j =>
                        {
                            // Skip current medoids and point #I
                            if (j == h || currentMedoidIndices.Contains(j)) return;

                            // Compute Kijh
                            var m = currentMedoidIndicesArray[pointClusterIndices[j]];
                            var dj = Distance.Distance(x[j], x[m]);
                            var djh = Distance.Distance(x[j], x[h]);
                            var dji = Distance.Distance(x[j], x[pointIIndex]);
                            var kjih = (dji == dj)
                                ? ((djh < secondClusterDistance[j]) ? djh - dj : secondClusterDistance[j] - dj)
                                : ((djh < dj ? djh - dj : 0.0));
                            InterlockedEx.Add(ref tih, kjih);
                        });

                        var currentMinTih = minTih;
                        if (tih < currentMinTih.Item3)
                        {
                            var newMinTih = Tuple.Create(i, h, tih);
                            do
                            {
                                var actualMinTih = Interlocked.CompareExchange(ref minTih, newMinTih, currentMinTih);
                                if (actualMinTih == currentMinTih) break;
                                currentMinTih = actualMinTih;
                            }
                            while (tih < currentMinTih.Item3);
                        }
                    }
                });

                // Check exit criteria
                if (minTih.Item3 >= 0.0)
                {
                    break;
                }

                // Swap points
                currentMedoidIndices.Remove(currentMedoidIndicesArray[minTih.Item1]);
                currentMedoidIndices.Add(minTih.Item2);
                currentMedoidIndicesArray[minTih.Item1] = minTih.Item2;
            }

            for (int i = 0; i < K; ++i)
                Clusters.Centroids[i] = x[currentMedoidIndicesArray[i]];

            if (ComputeError)
            {
                // Compute the average error
                int[] labels = new int[x.Length];
                Clusters.Decide(x, labels);
                Error = Clusters.Distortion(x, labels);
            }

            Accord.Diagnostics.Debug.Assert(Clusters.NumberOfClasses == K);
            Accord.Diagnostics.Debug.Assert(Clusters.NumberOfOutputs == K);
            Accord.Diagnostics.Debug.Assert(Clusters.NumberOfInputs == x[0].Length);

            // Return the classification result
            return Clusters;
        }

        /// <summary>
        ///   Determines if the algorithm has converged by comparing the
        ///   centroids between two consecutive iterations.
        /// </summary>
        /// 
        /// <param name="centroids">The previous centroids.</param>
        /// <param name="newCentroids">The new centroids.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if all centroids had a percentage change
        ///    less than <see param="threshold"/>. Returns <see langword="false"/> otherwise.</returns>
        ///    
        private bool converged(T[][] centroids, T[][] newCentroids)
        {
            Iterations++;

            if (MaxIterations > 0 && Iterations > MaxIterations)
                return true;

            if (Token.IsCancellationRequested)
                return true;

            for (int i = 0; i < centroids.Length; i++)
            {
                T[] centroid = centroids[i];
                T[] newCentroid = newCentroids[i];

                if (System.Math.Abs(Distance.Distance(centroid, newCentroid)) >= Tolerance)
                    return false;
            }

            return true;
        }

#pragma warning disable 0618
        IClusterCollection<T[]> IClusteringAlgorithm<T[]>.Clusters
        {
            get { return (IClusterCollection<T[]>)clusters; }
        }

        IClusterCollection<T[]> IUnsupervisedLearning<IClusterCollection<T[]>, T[], int>.Learn(T[][] x, double[] weights)
        {
            return (IClusterCollection<T[]>)Learn(x);
        }
#pragma warning restore 0618

    }

    /// <summary>
    ///   k-Medoids algorithm.
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
    /// The most common realisation of k-medoid clustering is the Partitioning Around Medoids (PAM) algorithm.
    /// PAM uses a greedy search which may not find the optimum solution, but it is faster than exhaustive search.
    /// </para>
    /// <para>
    /// [1] Kaufman, L. and Rousseeuw, P.J. (1987), Clustering by means of Medoids, in Statistical Data Analysis 
    /// Based on the L1–Norm and Related Methods, edited by Y. Dodge, North-Holland, 405–416.
    /// </para>
    /// <para>
    ///   This is the specialized, non-generic version of the k-Medoids algorithm
    ///   that is set to work on <see cref="T:System.Int32"/> arrays.
    /// </para>
    /// </remarks>
    /// 
    /// <seealso cref="PartitioningAroundMedoids{T}"/>
    /// 
    [Serializable]
    public class PartitioningAroundMedoids : PartitioningAroundMedoids<int>
    {
        /// <summary>
        ///   Initializes a new instance of k-Medoids algorithm
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide input data.</param>    
        /// 
        public PartitioningAroundMedoids(int k)
            : base(k, new Accord.Math.Distances.Manhattan())
        {
        }
    }

}
