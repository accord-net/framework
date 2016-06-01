// Accord Machine Learning Library
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
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Accord.Math;
    using Accord.Statistics;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Math.Distances;

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
        public int K { get { return clusters.Count; } }

        /// <summary>
        ///   Gets the dimensionality of the data space.
        /// </summary>
        /// 
        public int Dimension
        {
            get { return clusters.Dimension; }
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
        ///   Initializes a new instance of KModes algorithm
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide input data.</param>       
        /// <param name="distance">The distance function to use. Default is to
        /// use the <see cref="Accord.Math.Distance.SquareEuclidean(double[], double[])"/> distance.</param>
        /// 
        [Obsolete("Please specify the distance function using classes instead of lambda functions.")]
        public KModes(int k, Func<T[], T[], double> distance)
            : this(k, Accord.Math.Distance.GetDistance(distance))
        {
        }

        /// <summary>
        ///   Initializes a new instance of KModes algorithm
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide input data.</param>       
        /// <param name="distance">The distance function to use. Default is to
        /// use the <see cref="Accord.Math.Distance.SquareEuclidean(double[], double[])"/> distance.</param>
        /// 
        public KModes(int k, IDistance<T[]> distance)
        {
            if (k <= 0)
                throw new ArgumentOutOfRangeException("k");

            if (distance == null)
                throw new ArgumentNullException("distance");

            // Create the object-oriented structure to hold
            //  information about the k-modes' clusters.
            this.clusters = new KModesClusterCollection<T>(k, distance);

            this.Initialization = Seeding.KMeansPlusPlus;
            this.Tolerance = 1e-5;
            this.MaxIterations = 100;
        }

        /// <summary>
        ///   Divides the input data into K clusters. 
        /// </summary>     
        /// 
        /// <param name="points">The data where to compute the algorithm.</param>
        /// 
        public int[] Compute(T[][] points)
        {
            // Initial argument checking
            if (points == null)
                throw new ArgumentNullException("points");

            if (points.Length < K)
                throw new ArgumentException("Not enough points. There should be more points than the number K of clusters.");

            int k = this.K;
            int rows = points.Length;
            int cols = points[0].Length;

            // Perform a random initialization of the clusters
            // if the algorithm has not been initialized before.
            //
            if (this.Clusters.Centroids[0] == null)
            {
                Clusters.Randomize(points);
            }

            // Initial variables
            int[] labels = new int[rows];
            double[] proportions = Clusters.Proportions;
            T[][] centroids = Clusters.Centroids;
            T[][] newCentroids = new T[k][];
            for (int i = 0; i < newCentroids.Length; i++)
                newCentroids[i] = new T[cols];

            var clusters = new List<T[]>[k];
            for (int i = 0; i < k; i++)
                clusters[i] = new List<T[]>();

            Iterations = 0;

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

                        T mode = values.Mode(alreadySorted: false, inPlace: true);

                        newCentroids[i][d] = mode;
                    }
                }


                // The algorithm stops when there is no further change in the
                //  centroids (relative difference is less than the threshold).
                if (converged(centroids, newCentroids)) break;


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

            if (ComputeError)
                // Compute the average error
                Error = Clusters.Distortion(points, labels);

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
        /// 
        /// <returns>Returns <see langword="true"/> if all centroids had a percentage change
        ///    less than <see param="threshold"/>. Returns <see langword="false"/> otherwise.</returns>
        ///    
        private bool converged(T[][] centroids, T[][] newCentroids)
        {
            Iterations++;

            if (MaxIterations > 0 && Iterations > MaxIterations)
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
            : base(k, new Accord.Math.Distances.Manhattan()) { }
    }

}
