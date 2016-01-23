// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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

    /// <summary>
    ///   Binary split clustering algorithm.
    /// </summary>
    /// 
    /// <example>
    ///   How to perform clustering with Binary Split.
    ///   
    ///   <code>
    ///   // Declare some observations
    ///   double[][] observations = 
    ///   {
    ///       new double[] { -5, -2, -1 },
    ///       new double[] { -5, -5, -6 },
    ///       new double[] {  2,  1,  1 },
    ///       new double[] {  1,  1,  2 },
    ///       new double[] {  1,  2,  2 },
    ///       new double[] {  3,  1,  2 },
    ///       new double[] { 11,  5,  4 },
    ///       new double[] { 15,  5,  6 },
    ///       new double[] { 10,  5,  6 },
    ///   };
    ///  
    ///   // Create a new binary split with 3 clusters 
    ///   BinarySplit binarySplit = new BinarySplit(3);
    ///  
    ///   // Compute the algorithm, retrieving an integer array
    ///   //  containing the labels for each of the observations
    ///   int[] labels = binarySplit.Compute(observations);
    ///   
    ///   // In order to classify new, unobserved instances, you can
    ///   // use the binarySplit.Clusters.Nearest method, as shown below:
    ///   int c = binarySplit.Clusters.Nearest(new double[] { 4, 1, 9) });
    ///   </code>
    /// </example>
    /// 
    /// <seealso cref="KMeans"/>
    /// 
    [Serializable]
    public class BinarySplit : KMeans
    {

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
        ///   Divides the input data into K clusters.
        /// </summary>
        /// 
        /// <param name="data">The data where to compute the algorithm.</param>
        /// <param name="weights">The weight associated with each data point.</param>
        /// 
        public override int[] Compute(double[][] data, double[] weights)
        {
            // Initial argument checking
            if (data == null)
                throw new ArgumentNullException("data");

            if (data.Length < K)
                throw new ArgumentException("Not enough points. There should be more points than the number K of clusters.");

            if (weights == null)
                throw new ArgumentNullException("weights");

            if (data.Length != weights.Length)
                throw new ArgumentException("Data weights vector must be the same length as data samples.");

            double weightSum = weights.Sum();
            if (weightSum <= 0)
                throw new ArgumentException("Not enough points. There should be more points than the number K of clusters.");

            int cols = data[0].Length;
            for (int i = 0; i < data.Length; i++)
                if (data[0].Length != cols)
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

            double[][] centroids = Clusters.Centroids;
            double[][][] clusters = new double[k][][];
            double[] distortions = new double[k];

            // 1. Start with all data points in one cluster
            clusters[0] = data;

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


            return Clusters.Nearest(data);
        }

        private static Tuple<double[][], double[][]> split(double[][] cluster, KMeans kmeans)
        {
            kmeans.Randomize(cluster);

            int[] idx = kmeans.Compute(cluster);

            List<double[]> a = new List<double[]>();
            List<double[]> b = new List<double[]>();

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
