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
    using System.Collections.ObjectModel;
    using Accord.Math;
    using Accord.Statistics.Distributions.Univariate;

    /// <summary>
    ///   k-Modes algorithm.
    /// </summary>
    /// 
    /// <remarks>
    ///   The k-Modes algorithm is a variant of the k-Means which instead of 
    ///   locating means attempts to locate the modes of a set of points. As
    ///   the algorithm does not require explicit numeric manipulation of the
    ///   input points (such as addition and division to compute the means),
    ///   the algorithm can be used with arbitrary (generic) data structures.
    /// </remarks>
    /// 
    /// <seealso cref="KModes"/>
    /// <seealso cref="KMeans"/>
    /// <seealso cref="MeanShift"/>
    /// 
    [Serializable]
    public class KModes<T> : IClusteringAlgorithm<T[]>
    {

        private KModesClusterCollection<T> clusters;


        /// <summary>
        ///   Gets the clusters found by K-modes.
        /// </summary>
        /// 
        public KModesClusterCollection<T> Clusters
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
        ///   Gets the dimensionality of the data space.
        /// </summary>
        /// 
        public int Dimension
        {
            get { return clusters.Dimension; }
        }

        /// <summary>
        ///   Gets or sets the distance function used
        ///   as a distance metric between data points.
        /// </summary>
        /// 
        public Func<T[], T[], double> Distance
        {
            get { return clusters.Distance; }
            set { clusters.Distance = value; }
        }

        /// <summary>
        ///   Initializes a new instance of KMeans algorithm
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide input data.</param>       
        /// <param name="distance">The distance function to use. Default is to
        /// use the <see cref="Accord.Math.Distance.SquareEuclidean(double[], double[])"/> distance.</param>
        /// 
        public KModes(int k, Func<T[], T[], double> distance)
        {
            if (k <= 0)
                throw new ArgumentOutOfRangeException("k");

            if (distance == null)
                throw new ArgumentNullException("distance");

            // Create the object-oriented structure to hold
            //  information about the k-means' clusters.
            this.clusters = new KModesClusterCollection<T>(k, distance);
        }


        /// <summary>
        ///   Randomizes the clusters inside a dataset.
        /// </summary>
        /// 
        /// <param name="points">The data to randomize the algorithm.</param>
        /// <param name="useSeeding">True to use the k-means++ seeding algorithm. False otherwise.</param>
        /// 
        public void Randomize(T[][] points, bool useSeeding = true)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            T[][] centroids = clusters.Centroids;

            if (useSeeding)
            {
                // Initialize using K-Means++
                // http://en.wikipedia.org/wiki/K-means%2B%2B

                // 1. Choose one center uniformly at random from among the data points.
                centroids[0] = (T[])points[Accord.Math.Tools.Random.Next(0, points.Length)].Clone();

                for (int c = 1; c < centroids.Length; c++)
                {
                    // 2. For each data point x, compute D(x), the distance between
                    //    x and the nearest center that has already been chosen.

                    double sum = 0;
                    double[] D = new double[points.Length];
                    for (int i = 0; i < D.Length; i++)
                    {
                        T[] x = points[i];

                        double min = Distance(x, centroids[0]);
                        for (int j = 1; j < c; j++)
                        {
                            double d = Distance(x, centroids[j]);
                            if (d < min) min = d;
                        }

                        D[i] = min;
                        sum += min;
                    }

                    for (int i = 0; i < D.Length; i++)
                        D[i] /= sum;

                    // 3. Choose one new data point at random as a new center, using a weighted
                    //    probability distribution where a point x is chosen with probability 
                    //    proportional to D(x)^2.
                    centroids[c] = (T[])points[GeneralDiscreteDistribution.Random(D)].Clone();
                }
            }
            else
            {
                // pick K unique random indexes in the range 0..n-1
                int[] idx = Accord.Statistics.Tools.RandomSample(points.Length, K);

                // assign centroids from data set
                centroids = points.Submatrix(idx).MemberwiseClone();
            }

            this.clusters.Centroids = centroids;
        }

        /// <summary>
        ///   Divides the input data into K clusters. 
        /// </summary>     
        /// 
        /// <param name="points">The data where to compute the algorithm.</param>
        /// <param name="threshold">The relative convergence threshold
        /// for the algorithm. Default is 1e-5.</param>
        /// 
        public int[] Compute(T[][] points, double threshold = 1e-5)
        {
            // Initial argument checking
            if (points == null)
                throw new ArgumentNullException("points");

            if (threshold < 0)
                throw new ArgumentException("Threshold should be a positive number.", "threshold");


            int k = this.K;
            int rows = points.Length;


            // Perform a random initialization of the clusters
            // if the algorithm has not been initialized before.

            if (Clusters.Centroids[0] == null)
            {
                Randomize(points, useSeeding: true);
            }


            // Initial variables
            int[] labels = new int[rows];
            double[] proportions = Clusters.Proportions;
            T[][] centroids = Clusters.Centroids;
            T[][] newCentroids = new T[k][];
            for (int i = 0; i < newCentroids.Length; i++)
                newCentroids[i] = new T[Dimension];

            var clusters = new List<T[]>[k];
            for (int i = 0; i < k; i++)
                clusters[i] = new List<T[]>();


            do // Main loop
            {
                // Reset the centroids and the
                //  cluster member counters'
                for (int i = 0; i < k; i++)
                    clusters[i].Clear();


                // First we will accumulate the data points
                // into their nearest clusters, storing this
                // information into the newClusters variable.

                // For each point in the data set,
                for (int i = 0; i < points.Length; i++)
                {
                    // Get the point
                    T[] point = points[i];

                    // Compute the nearest cluster centroid
                    int c = labels[i] = Clusters.Nearest(points[i]);

                    // Accumulate in the corresponding centroid
                    clusters[c].Add(point);
                }

                // Next we will compute each cluster's new centroid
                //  value by computing the mode in each cluster.

                for (int i = 0; i < k; i++)
                {
                    if (clusters[i].Count == 0)
                    {
                        newCentroids[i] = centroids[i];
                        continue;
                    }

                    T[][] p = clusters[i].ToArray();

                    // For each dimension
                    for (int d = 0; d < Dimension; d++)
                    {
                        T[] values = p.GetColumn(d);

                        T mode = Accord.Statistics.Tools
                            .Mode<T>(values, alreadySorted: false, inPlace: true);

                        newCentroids[i][d] = mode;
                    }
                }


                // The algorithm stops when there is no further change in the
                //  centroids (relative difference is less than the threshold).
                if (converged(centroids, newCentroids, threshold)) break;


                // go to next generation
                for (int i = 0; i < centroids.Length; i++)
                    centroids[i] = newCentroids[i];
            }
            while (true);


            // Compute cluster information (optional)
            for (int i = 0; i < k; i++)
            {
                // Compute the proportion of samples in the cluster
                proportions[i] = clusters[i].Count / (double)points.Length;
            }


            // Return the classification result
            return labels;
        }

        /// <summary>
        ///   Divides the input data into K clusters. 
        /// </summary>  
        /// 
        /// <param name="points">The data where to compute the algorithm.</param>
        /// <param name="threshold">The relative convergence threshold
        /// for the algorithm. Default is 1e-5.</param>
        /// 
        /// <param name="error">
        ///   The average distance metric from the
        ///   data points to the clusters' centroids.
        /// </param>
        /// 
        public int[] Compute(T[][] points, out double error, double threshold = 1e-5)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            // Classify the input data
            int[] labels = Compute(points, threshold);

            // Compute the average error
            error = Clusters.Distortion(points, labels);

            // Return the classification result
            return labels;
        }



        /// <summary>
        ///   Determines if the algorithm has converged by comparing the
        ///   centroids between two consecutive iterations.
        /// </summary>
        /// 
        /// <param name="centroids">The previous centroids.</param>
        /// <param name="newCentroids">The new centroids.</param>
        /// <param name="threshold">A convergence threshold.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if all centroids had a percentage change
        ///    less than <see param="threshold"/>. Returns <see langword="false"/> otherwise.</returns>
        ///    
        private bool converged(T[][] centroids, T[][] newCentroids, double threshold)
        {
            for (int i = 0; i < centroids.Length; i++)
            {
                T[] centroid = centroids[i];
                T[] newCentroid = newCentroids[i];

                if (System.Math.Abs(Distance(centroid, newCentroid)) >= threshold)
                    return false;
            }

            return true;
        }


        IClusterCollection<T[]> IClusteringAlgorithm<T[]>.Clusters
        {
            get { return clusters; }
        }

    }

    /// <summary>
    ///   k-Modes algorithm.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The k-Modes algorithm is a variant of the k-Means which instead of 
    ///   locating means attempts to locate the modes of a set of points. As
    ///   the algorithm does not require explicit numeric manipulation of the
    ///   input points (such as addition and division to compute the means),
    ///   the algorithm can be used with arbitrary (generic) data structures.</para>
    /// <para>
    ///   This is the specialized, non-generic version of the K-Modes algorithm
    ///   that is set to work on <see cref="T:System.Int32"/> arrays.</para>
    /// </remarks>
    /// 
    /// <seealso cref="KModes{T}"/>
    /// 
    [Serializable]
    public class KModes : KModes<int>
    {

        /// <summary>
        ///   Initializes a new instance of K-Modes algorithm
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide input data.</param>    
        /// 
        public KModes(int k) 
            : base(k, Accord.Math.Distance.Manhattan) { }
    }

}
