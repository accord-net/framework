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
    public class KModes<TData> : IClusteringAlgorithm<TData>
        where TData : ICloneable
    {

        private KModesClusterCollection<TData> clusters;


        /// <summary>
        ///   Gets the clusters found by K-modes.
        /// </summary>
        /// 
        public KModesClusterCollection<TData> Clusters
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
        public Func<TData, TData, double> Distance
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
        public KModes(int k, Func<TData, TData, double> distance)
        {
            if (k <= 0) 
                throw new ArgumentOutOfRangeException("k");
            if (distance == null) 
                throw new ArgumentNullException("distance");

            // Create the object-oriented structure to hold
            //  information about the k-means' clusters.
            this.clusters = new KModesClusterCollection<TData>(k, distance);
        }


        /// <summary>
        ///   Randomizes the clusters inside a dataset.
        /// </summary>
        /// 
        /// <param name="data">The data to randomize the algorithm.</param>
        /// 
        public void Randomize(TData[] data)
        {
            if (data == null) 
                throw new ArgumentNullException("data");

            // pick K unique random indexes in the range 0..n-1
            int[] idx = Accord.Statistics.Tools.RandomSample(data.Length, K);

            // assign centroids from data set
            Clusters.Centroids = (TData[])data.Submatrix(idx).Clone();
        }

        /// <summary>
        ///   Divides the input data into K clusters. 
        /// </summary>     
        /// 
        /// <param name="points">The data where to compute the algorithm.</param>
        /// <param name="threshold">The relative convergence threshold
        /// for the algorithm. Default is 1e-5.</param>
        /// 
        public int[] Compute(TData[] points, double threshold = 1e-5)
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
                Randomize(points);
            }


            // Initial variables
            int[] labels = new int[rows];
            double[] proportions = Clusters.Proportions;
            TData[] centroids = Clusters.Centroids;
            TData[] newCentroids = new TData[k];

            List<TData>[] clusters = new List<TData>[k];
            for (int i = 0; i < k; i++)
                clusters[i] = new List<TData>();


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
                    TData point = points[i];

                    // Compute the nearest cluster centroid
                    int c = labels[i] = Clusters.Nearest(points[i]);

                    // Accumulate in the corresponding centroid
                    clusters[c].Add(point);
                }

                // Next we will compute each cluster's new centroid
                //  value by computing the mode in each cluster.

                for (int i = 0; i < k; i++)
                    newCentroids[i] = Accord.Statistics.Tools.Mode<TData>(clusters[i].ToArray());


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
        /// <param name="data">The data where to compute the algorithm.</param>
        /// <param name="threshold">The relative convergence threshold
        /// for the algorithm. Default is 1e-5.</param>
        /// 
        /// <param name="error">
        ///   The average distance metric from the
        ///   data points to the clusters' centroids.
        /// </param>
        /// 
        public int[] Compute(TData[] data, out double error, double threshold = 1e-5)
        {
            // Initial argument checking
            if (data == null) throw new ArgumentNullException("data");

            // Classify the input data
            int[] labels = Compute(data, threshold);

            // Compute the average error
            error = Clusters.Distortion(data, labels);

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
        private bool converged(TData[] centroids, TData[] newCentroids, double threshold)
        {
            for (int i = 0; i < centroids.Length; i++)
            {
                TData centroid = centroids[i];
                TData newCentroid = newCentroids[i];

                if (System.Math.Abs(Distance(centroid, newCentroid)) >= threshold)
                    return false;
            }
            return true;
        }


        IClusterCollection<TData> IClusteringAlgorithm<TData>.Clusters
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
    ///   This is the specialized, non-generic version of the K-Models algorithm
    ///   that is set to work on <see cref="T:System.Int32"/> arrays.</para>
    /// </remarks>
    /// 
    /// <seealso cref="KModes{T}"/>
    /// 
    [Serializable]
    public class KModes : KModes<int[]>
    {

        /// <summary>
        ///   Initializes a new instance of K-Modes algorithm
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide input data.</param>    
        /// 
        public KModes(int k) : base(k, Accord.Math.Distance.Manhattan) { }
    }

}
