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
    using System.Collections.Generic;
    using Accord.Math;

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
    public class BinarySplit : IClusteringAlgorithm<double[]>
    {

        private KMeansClusterCollection clusters;


        /// <summary>
        ///   Gets the clusters.
        /// </summary>
        /// 
        public KMeansClusterCollection Clusters
        {
            get { return clusters; }
        }

        /// <summary>
        ///   Gets the number of clusters.
        /// </summary>
        /// 
        public int K
        {
            get { return clusters.Count; }
        }

        /// <summary>
        ///   Gets or sets the distance function used
        ///   as a distance metric between data points.
        /// </summary>
        /// 
        public Func<double[], double[], double> Distance
        {
            get { return clusters.Distance; }
            set { clusters.Distance = value; }
        }

        /// <summary>
        ///   Gets the dimensionality of the data space.
        /// </summary>
        /// 
        public int Dimension
        {
            get { return clusters.Centroids[0].Length; }
        }

        /// <summary>
        ///   Initializes a new instance of the Binary Split algorithm
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide the input data into.</param>    
        /// <param name="distance">The distance function to use. Default is to
        /// use the <see cref="Accord.Math.Distance.SquareEuclidean(double[], double[])"/> distance.</param>
        /// 
        public BinarySplit(int k, Func<double[], double[], double> distance)
        {
            if (k <= 0)
                throw new ArgumentOutOfRangeException("k");
            if (distance == null) 
                throw new ArgumentNullException("distance");

            // Create the object-oriented structure to hold
            //  information about the k-means' clusters.
            this.clusters = new KMeansClusterCollection(k, distance);
        }

        /// <summary>
        ///   Initializes a new instance of the Binary Split algorithm
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide the input data into.</param>    
        /// 
        public BinarySplit(int k)
            : this(k, Accord.Math.Distance.SquareEuclidean) { }

        /// <summary>
        ///   Divides the input data into K clusters. 
        /// </summary>     
        /// 
        /// <param name="points">The data where to compute the algorithm.</param>
        /// 
        public int[] Compute(double[][] points)
        {
            return Compute(points, 1e-5);
        }

        /// <summary>
        ///   Divides the input data into K clusters. 
        /// </summary>     
        /// 
        /// <param name="points">The data where to compute the algorithm.</param>
        /// <param name="threshold">The relative convergence threshold
        ///   for the algorithm. Default is 1e-5.</param>
        /// 
        public int[] Compute(double[][] points, double threshold)
        {
            int k = Clusters.Count;

            KMeans kmeans = new KMeans(2, Clusters.Distance);

            double[][] centroids = Clusters.Centroids;
            double[][][] clusters = new double[k][][];
            double[] distortions = new double[k];

            // 1. Start with all data points in one cluster
            clusters[0] = points;

            // 2. Repeat steps 3 to 6 (k-1) times to obtain K centroids
            for (int current = 1; current < k; current++)
            {
                // 3. Choose cluster with largest distortion
                int choosen; distortions.Max(current, out choosen);

                // 4. Split cluster into two sub-clusters
                var splits = split(clusters[choosen], kmeans, threshold);

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

            return Clusters.Nearest(points);
        }


        /// <summary>
        ///   Gets the collection of clusters currently modeled by the clustering algorithm.
        /// </summary>
        /// 
        IClusterCollection<double[]> IClusteringAlgorithm<double[]>.Clusters
        {
            get { return clusters; }
        }



        private static Tuple<double[][], double[][]> split(double[][] cluster,
            KMeans kmeans, double threshold)
        {
            kmeans.Randomize(cluster, useSeeding: false);

            int[] idx = kmeans.Compute(cluster, threshold, false);

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
