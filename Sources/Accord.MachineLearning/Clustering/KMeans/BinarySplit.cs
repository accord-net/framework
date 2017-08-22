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
    using Accord.Compat;

    /// <summary>
    ///   Binary split clustering algorithm.
    /// </summary>
    /// 
    /// <example>
    ///   How to perform clustering with Binary Split.
    ///   
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\Clustering\BinarySplitTest.cs" region="doc_sample1" />
    /// </example>
    /// 
    /// <seealso cref="KMeans"/>
    /// <seealso cref="GaussianMixtureModel"/>
    /// 
    [Serializable]
    public class BinarySplit : KMeans
    {
        private bool computeProportions;

        /// <summary>
        ///   Gets or sets whether <see cref="KMeansClusterCollection.Proportions">cluster proportions</see> 
        ///   should be calculated after the learning algorithm has finished computing the clusters. Default
        ///   is false.
        /// </summary>
        /// 
        /// <value><c>true</c> if  to compute proportions after learning; otherwise, <c>false</c>.</value>
        /// 
        public bool ComputeProportions
        {
            get { return computeProportions; }
            set { computeProportions = value; }
        }

        /// <summary>
        ///   Initializes a new instance of the Binary Split algorithm
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide the input data into.</param>    
        /// <param name="distance">The distance function to use. Default is to
        /// use the <see cref="Accord.Math.Distance.SquareEuclidean(double[], double[])"/> distance.</param>
        /// 
        public BinarySplit(int k, IDistance<double[]> distance)
            : base(k, distance)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the Binary Split algorithm
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide the input data into.</param>    
        /// 
        public BinarySplit(int k)
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
                weights = Vector.Ones(x.Length);

            if (x.Length != weights.Length)
                throw new ArgumentException("Data weights vector must be the same length as data samples.");

            double weightSum = weights.Sum();
            if (weightSum <= 0)
                throw new ArgumentException("Not enough points. There should be more points than the number K of clusters.");

            int cols = x.Columns();
            for (int i = 0; i < x.Length; i++)
                if (x[i].Length != cols)
                    throw new DimensionMismatchException("data", "The points matrix should be rectangular. The vector at position {} has a different length than previous ones.");

            int k = Clusters.Count;

            KMeans kmeans = new KMeans(2)
            {
                Distance = (IDistance<double[]>)Clusters.Distance,
                ComputeError = false,
                ComputeCovariances = false,
                UseSeeding = UseSeeding,
                Tolerance = Tolerance,
                MaxIterations = MaxIterations,
            };

            var centroids = Clusters.Centroids;
            var clusters = new double[k][][];
            var distortions = new double[k];

            // 1. Start with all data points in one cluster
            clusters[0] = x;

            // 2. Repeat steps 3 to 6 (k-1) times to obtain K centroids
            for (int current = 1; current < k; current++)
            {
                // 3. Choose cluster with largest distortion
                int choosen; distortions.Max(current, out choosen);

                // 4. Split cluster into two sub-clusters
                var splits = split(clusters[choosen], kmeans);

                clusters[choosen] = splits.Item1;
                clusters[current] = splits.Item2;

                // 5. Replace chosen centroid and add a new one
                centroids[choosen] = kmeans.Clusters.Centroids[0];
                centroids[current] = kmeans.Clusters.Centroids[1];

                // Recompute distortions for the updated clusters
                distortions[choosen] = kmeans.Clusters[0].Distortion(clusters[choosen]);
                distortions[current] = kmeans.Clusters[1].Distortion(clusters[current]);

                // 6. Increment cluster count (current = current + 1)
            }

            Clusters.NumberOfInputs = cols;

            Accord.Diagnostics.Debug.Assert(Clusters.NumberOfClasses == K);
            Accord.Diagnostics.Debug.Assert(Clusters.NumberOfOutputs == K);
            Accord.Diagnostics.Debug.Assert(Clusters.NumberOfInputs == x[0].Length);

            if (ComputeProportions)
            {
                int[] y = Clusters.Decide(x);
                int[] counts = y.Histogram();
                counts.Divide(y.Length, result: Clusters.Proportions);

                ComputeInformation(x, y);
            }
            else
            {
                ComputeInformation(x);
            }

            return Clusters;
        }

        private static Tuple<double[][], double[][]> split(double[][] cluster, KMeans kmeans)
        {
            kmeans.Randomize(cluster);

            int[] idx = kmeans.Learn(cluster).Decide(cluster);

            var a = new List<double[]>();
            var b = new List<double[]>();

            for (int i = 0; i < idx.Length; i++)
            {
                if (idx[i] == 0)
                    a.Add(cluster[i]);
                else
                    b.Add(cluster[i]);
            }

            return Tuple.Create(a.ToArray(), b.ToArray());
        }
    }
}
