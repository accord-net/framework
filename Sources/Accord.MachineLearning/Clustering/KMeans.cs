// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
// cesarsouza at gmail.com
//
// Copyright © Antonino Porcino, 2010
// iz8bly at yahoo.it
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
    using Accord.Math;
    using Accord.Statistics.Distributions.Univariate;
    using System.Threading.Tasks;
    using System.Threading;
    using Accord.Math.Comparers;
    using System.Runtime.Serialization;

    /// <summary>
    ///   k-Means clustering algorithm.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In statistics and machine learning, k-means clustering is a method
    ///   of cluster analysis which aims to partition n observations into k 
    ///   clusters in which each observation belongs to the cluster with the
    ///   nearest mean.</para>
    /// <para>
    ///   It is similar to the expectation-maximization algorithm for mixtures
    ///   of Gaussians in that they both attempt to find the centers of natural
    ///   clusters in the data as well as in the iterative refinement approach
    ///   employed by both algorithms.</para> 
    /// 
    /// <para>
    ///   The algorithm is composed of the following steps:
    ///   <list type="number">
    ///     <item><description>
    ///         Place K points into the space represented by the objects that are
    ///         being clustered. These points represent initial group centroids.
    ///     </description></item>
    ///     <item><description>
    ///         Assign each object to the group that has the closest centroid.
    ///     </description></item>
    ///     <item><description>
    ///         When all objects have been assigned, recalculate the positions
    ///         of the K centroids.
    ///     </description></item>
    ///     <item><description>
    ///         Repeat Steps 2 and 3 until the centroids no longer move. This
    ///         produces a separation of the objects into groups from which the
    ///         metric to be minimized can be calculated.
    ///     </description></item>
    ///   </list></para>
    /// 
    /// <para>
    ///   This particular implementation uses the squared Euclidean distance
    ///   as a similarity measure in order to form clusters. </para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia, The Free Encyclopedia. K-means clustering. Available on:
    ///       http://en.wikipedia.org/wiki/K-means_clustering </description></item>
    ///     <item><description>
    ///       Matteo Matteucci. A Tutorial on Clustering Algorithms. Available on:
    ///       http://home.dei.polimi.it/matteucc/Clustering/tutorial_html/kmeans.html </description></item>
    ///   </list></para>
    /// </remarks>
    /// <example>
    ///   How to perform clustering with K-Means.
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
    ///   // Create a new K-Means algorithm with 3 clusters 
    ///   KMeans kmeans = new KMeans(3);
    ///  
    ///   // Compute the algorithm, retrieving an integer array
    ///   //  containing the labels for each of the observations
    ///   int[] labels = kmeans.Compute(observations);
    ///  
    ///   // As result, the first two observations should belong to the
    ///   // same cluster (thus having the same label). The same should
    ///   // happen to the next four observations and to the last three.
    ///   
    ///   // In order to classify new, unobserved instances, you can
    ///   // use the kmeans.Clusters.Nearest method, as shown below:
    ///   int c = kmeans.Clusters.Nearest(new double[] { 4, 1, 9) });
    ///   </code>
    ///   
    /// <para>
    ///   The following example demonstrates how to use the Mean Shift algorithm
    ///   for color clustering. It is the same code which can be found in the
    ///   <a href="">color clustering sample application</a>.</para>
    ///   
    /// <code>
    ///
    ///  int k = 5; 
    ///  
    ///  // Load a test image (shown below)
    ///  Bitmap image = ...
    ///  
    ///  // Create converters
    ///  ImageToArray imageToArray = new ImageToArray(min: -1, max: +1);
    ///  ArrayToImage arrayToImage = new ArrayToImage(image.Width, image.Height, min: -1, max: +1);
    ///  
    ///  // Transform the image into an array of pixel values
    ///  double[][] pixels; imageToArray.Convert(image, out pixels);
    ///  
    ///  
    ///  // Create a K-Means algorithm using given k and a
    ///  //  square Euclidean distance as distance metric.
    ///  KMeans kmeans = new KMeans(k, Distance.SquareEuclidean);
    ///  
    ///  // Compute the K-Means algorithm until the difference in
    ///  //  cluster centroids between two iterations is below 0.05
    ///  int[] idx = kmeans.Compute(pixels, 0.05);
    ///  
    ///  
    ///  // Replace every pixel with its corresponding centroid
    ///  pixels.ApplyInPlace((x, i) => kmeans.Clusters.Centroids[idx[i]]);
    ///  
    ///  // Retrieve the resulting image in a picture box
    ///  Bitmap result; arrayToImage.Convert(pixels, out result);
    /// </code>
    /// 
    /// <para>
    ///   The original image is shown below:</para>
    /// 
    ///   <img src="..\images\kmeans-start.png" />
    ///   
    /// <para>
    ///   The resulting image will be:</para>
    /// 
    ///   <img src="..\images\kmeans-end.png" />
    /// 
    /// </example>
    /// 
    /// <seealso cref="KModes{T}"/>
    /// <seealso cref="MeanShift"/>
    /// <seealso cref="GaussianMixtureModel"/>
    ///
    [Serializable]
    public class KMeans : IClusteringAlgorithm<double[]>
    {

        private KMeansClusterCollection clusters;


        /// <summary>
        ///   Gets the clusters found by K-means.
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
        public Func<double[], double[], double> Distance
        {
            get { return clusters.Distance; }
            set { clusters.Distance = value; }
        }

        /// <summary>
        ///   Gets or sets whether covariance matrices for
        ///   the clusters should be computed at the end of
        ///   an iteration. Default is true.
        /// </summary>
        /// 
        public bool ComputeInformation { get; set; }

        /// <summary>
        ///   Gets or sets whether to use the k-means++ seeding
        ///   algorithm to improve the initial solution of the
        ///   clustering. Default is true.
        /// </summary>
        /// 
        public bool UseCentroidSeeding { get; set; }

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
        ///   Initializes a new instance of the K-Means algorithm
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide the input data into.</param>    
        /// 
        public KMeans(int k)
            : this(k, Accord.Math.Distance.SquareEuclidean) { }

        /// <summary>
        ///   Initializes a new instance of the KMeans algorithm
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide the input data into.</param>    
        /// <param name="distance">The distance function to use. Default is to
        /// use the <see cref="Accord.Math.Distance.SquareEuclidean(double[], double[])"/> distance.</param>
        /// 
        public KMeans(int k, Func<double[], double[], double> distance)
        {
            if (k <= 0)
                throw new ArgumentOutOfRangeException("k");

            if (distance == null)
                throw new ArgumentNullException("distance");

            this.Tolerance = 1e-5;
            this.ComputeInformation = true;
            this.UseCentroidSeeding = true;

            // Create the object-oriented structure to hold
            //  information about the k-means' clusters.
            this.clusters = new KMeansClusterCollection(k, distance);
        }

        /// <summary>
        ///   Randomizes the clusters inside a dataset.
        /// </summary>
        /// 
        /// <param name="points">The data to randomize the algorithm.</param>
        /// 
        public void Randomize(double[][] points)
        {
            Randomize(points, UseCentroidSeeding);
        }

        /// <summary>
        ///   Randomizes the clusters inside a dataset.
        /// </summary>
        /// 
        /// <param name="points">The data to randomize the algorithm.</param>
        /// <param name="useSeeding">True to use the k-means++ seeding algorithm. False otherwise.</param>
        /// 
        public void Randomize(double[][] points, bool useSeeding)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            this.UseCentroidSeeding = useSeeding;

            double[][] centroids = clusters.Centroids;

            if (UseCentroidSeeding)
            {
                // Initialize using K-Means++
                // http://en.wikipedia.org/wiki/K-means%2B%2B

                // 1. Choose one center uniformly at random from among the data points.
                centroids[0] = (double[])points[Accord.Math.Tools.Random.Next(0, points.Length)].Clone();

                for (int c = 1; c < centroids.Length; c++)
                {
                    // 2. For each data point x, compute D(x), the distance between
                    //    x and the nearest center that has already been chosen.

                    double sum = 0;
                    double[] D = new double[points.Length];
                    for (int i = 0; i < D.Length; i++)
                    {
                        double[] x = points[i];

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
                    centroids[c] = (double[])points[GeneralDiscreteDistribution.Random(D)].Clone();
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
        /// 
        public int[] Compute(double[][] points)
        {
#pragma warning disable 0618
            return Compute(points, Tolerance, ComputeInformation);
#pragma warning restore 0618
        }

        /// <summary>
        ///   Divides the input data into K clusters. 
        /// </summary>     
        /// 
        /// <param name="points">The data where to compute the algorithm.</param>
        /// <param name="threshold">The relative convergence threshold
        ///   for the algorithm. Default is 1e-5.</param>
        /// 
        [Obsolete("Please set the 'Tolerance' property instead of passing the 'threshold' parameter.")]
        public int[] Compute(double[][] points, double threshold)
        {
            return Compute(points, threshold, ComputeInformation);
        }

        /// <summary>
        ///   Divides the input data into K clusters. 
        /// </summary>
        /// 
        /// <param name="data">The data where to compute the algorithm.</param>
        /// <param name="threshold">The relative convergence threshold
        ///   for the algorithm. Default is 1e-5.</param>
        /// <param name="computeInformation">Pass <c>true</c> to compute additional information
        ///   when the algorithm finishes, such as cluster variances and proportions; false
        ///   otherwise. Default is true.</param>
        ///   
        [Obsolete("Please set the 'Tolerance' and 'ComputeInformation' properties instead of using this overload.")]
        public int[] Compute(double[][] data, double threshold, bool computeInformation)
        {
            // Initial argument checking
            if (data == null)
                throw new ArgumentNullException("data");

            if (data.Length < K)
                throw new ArgumentException("Not enough points. There should be more points than the number K of clusters.");

            if (threshold < 0)
                throw new ArgumentException("Threshold should be a positive number.", "threshold");

            this.ComputeInformation = computeInformation;
            this.Tolerance = threshold;
            this.Iterations = 0;

            // TODO: Implement a faster version using the triangle
            // inequality to reduce the number of distance calculations
            //
            //  - http://www-cse.ucsd.edu/~elkan/kmeansicml03.pdf
            //  - http://mloss.org/software/view/48/
            //

            int k = this.K;
            int rows = data.Length;
            int cols = data[0].Length;


            // Perform a random initialization of the clusters
            // if the algorithm has not been initialized before.
            //
            if (this.Clusters.Centroids[0] == null)
            {
                Randomize(data);
            }


            // Initial variables
            int[] count = new int[k];
            int[] labels = new int[rows];
            double[][] centroids = clusters.Centroids;
            double[][] newCentroids = new double[k][];
            for (int i = 0; i < newCentroids.Length; i++)
                newCentroids[i] = new double[cols];

            PerformClustering(data, threshold, newCentroids, count, labels, centroids);


            for (int i = 0; i < centroids.Length; i++)
            {
                // Compute the proportion of samples in the cluster
                clusters.Proportions[i] = count[i] / (double)data.Length;
            }


            if (ComputeInformation)
            {
                // Compute cluster information (optional)
                for (int i = 0; i < centroids.Length; i++)
                {
                    // Extract the data for the current cluster
                    double[][] sub = data.Submatrix(labels.Find(x => x == i));

                    if (sub.Length > 0)
                    {
                        // Compute the current cluster variance
                        clusters.Covariances[i] = Statistics.Tools.Covariance(sub, centroids[i]);
                    }
                    else
                    {
                        // The cluster doesn't have any samples
                        clusters.Covariances[i] = new double[cols, cols];
                    }
                }
            }


            // Return the classification result
            return labels;
        }

        /// <summary>
        ///   Performs the actual clustering, given a set of data points and
        ///   a convergence threshold. The remaining parameters must be set
        ///   before returning the method.
        /// </summary>
        /// 
        protected virtual void PerformClustering(double[][] data, double threshold,
            double[][] newCentroids, int[] count,
            int[] labels, double[][] centroids)
        {
            Object[] syncObjects = new Object[K];
            for (int i = 0; i < syncObjects.Length; i++)
                syncObjects[i] = new Object();


            bool shouldStop = false;

            while (!shouldStop) // Main loop
            {
                // Reset the centroids and the member counters
                for (int i = 0; i < newCentroids.Length; i++)
                    Array.Clear(newCentroids[i], 0, newCentroids[i].Length);
                Array.Clear(count, 0, count.Length);

                // First we will accumulate the data points
                // into their nearest clusters, storing this
                // information into the newClusters variable.

                // For each point in the data set,
                Parallel.For(0, data.Length, i =>
                {
                    // Get the point
                    double[] point = data[i];

                    // Get the nearest cluster centroid
                    int c = labels[i] = Clusters.Nearest(point);

                    // Increase the cluster's sample counter
                    Interlocked.Increment(ref count[c]);

                    // Get the closest cluster centroid
                    double[] centroid = newCentroids[c];

                    lock (syncObjects[c])
                    {
                        // Accumulate in the cluster centroid
                        for (int j = 0; j < point.Length; j++)
                            centroid[j] += point[j];
                    }
                });

                // Next we will compute each cluster's new centroid
                //  by dividing the accumulated sums by the number of
                //  samples in each cluster, thus averaging its members.
                for (int i = 0; i < newCentroids.Length; i++)
                {
                    double clusterCount = count[i];

                    if (clusterCount != 0)
                    {
                        for (int j = 0; j < newCentroids[i].Length; j++)
                            newCentroids[i][j] /= clusterCount;
                    }
                }


                // The algorithm stops when there is no further change in the
                //  centroids (relative difference is less than the threshold).
                shouldStop = converged(centroids, newCentroids, threshold);

                // go to next generation
                for (int i = 0; i < centroids.Length; i++)
                    for (int j = 0; j < centroids[i].Length; j++)
                        centroids[i][j] = newCentroids[i][j];
            }
        }

        /// <summary>
        ///   Divides the input data into K clusters. 
        /// </summary>  
        /// 
        /// <param name="data">The data where to compute the algorithm.</param>
        /// <param name="error">
        ///   The average square distance from the
        ///   data points to the clusters' centroids.
        /// </param>
        /// 
        public int[] Compute(double[][] data, out double error)
        {
#pragma warning disable 0618
            return Compute(data, Tolerance, out error, ComputeInformation);
#pragma warning restore 0618
        }

        /// <summary>
        ///   Divides the input data into K clusters. 
        /// </summary>  
        /// 
        /// <param name="data">The data where to compute the algorithm.</param>
        /// <param name="computeInformation">Pass <c>true</c> to compute additional information
        ///   when the algorithm finishes, such as cluster variances and proportions; false
        ///   otherwise. Default is true.</param>
        /// <param name="error">
        ///   The average square distance from the
        ///   data points to the clusters' centroids.
        /// </param>
        /// 
        [Obsolete("Please set the 'ComputeInformation' property instead of passing the 'computeInformation' parameter.")]
        public int[] Compute(double[][] data, out double error, bool computeInformation)
        {
            return Compute(data, Tolerance, out error, computeInformation);
        }

        /// <summary>
        ///   Divides the input data into K clusters. 
        /// </summary>  
        /// 
        /// <param name="data">The data where to compute the algorithm.</param>
        /// <param name="threshold">The relative convergence threshold
        /// for the algorithm. Default is 1e-5.</param>
        /// <param name="error">
        ///   The average square distance from the
        ///   data points to the clusters' centroids.
        /// </param>
        /// 
        [Obsolete("Please set the 'Tolerance' property instead of passing the 'threshold' parameter.")]
        public int[] Compute(double[][] data, double threshold, out double error)
        {
            return Compute(data, threshold, out error, ComputeInformation);
        }

        /// <summary>
        ///   Divides the input data into K clusters. 
        /// </summary>  
        /// 
        /// <param name="data">The data where to compute the algorithm.</param>
        /// <param name="threshold">The relative convergence threshold
        /// for the algorithm. Default is 1e-5.</param>
        /// <param name="computeInformation">Pass <c>true</c> to compute additional information
        ///   when the algorithm finishes, such as cluster variances and proportions; false
        ///   otherwise. Default is true.</param>
        /// <param name="error">
        ///   The average square distance from the
        ///   data points to the clusters' centroids.
        /// </param>
        /// 
        [Obsolete("Please set the 'Tolerance' property instead of passing the 'threshold' parameter.")]
        public int[] Compute(double[][] data, double threshold, out double error, bool computeInformation)
        {
            // Initial argument checking
            if (data == null)
                throw new ArgumentNullException("data");

            // Classify the input data
            int[] labels = Compute(data, threshold, computeInformation);

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
        private bool converged(double[][] centroids, double[][] newCentroids, double threshold)
        {
            Iterations++;

            if (MaxIterations > 0 && Iterations > MaxIterations)
                return true;

            for (int i = 0; i < centroids.Length; i++)
            {
                double[] centroid = centroids[i];
                double[] newCentroid = newCentroids[i];

                for (int j = 0; j < centroid.Length; j++)
                {
                    if ((System.Math.Abs((centroid[j] - newCentroid[j]) / centroid[j])) >= threshold)
                        return false;
                }
            }

            return true;
        }

        IClusterCollection<double[]> IClusteringAlgorithm<double[]>.Clusters
        {
            get { return clusters; }
        }


        [OnDeserialized]
        private void OnDeserializedMethod(StreamingContext context)
        {
            if (this.Iterations == 0 && MaxIterations == 0 && Tolerance == 0)
            {
                this.Tolerance = 1e-5;
                this.ComputeInformation = true;
                this.UseCentroidSeeding = true;
            }
        }


        #region Deprecated
        /// <summary>
        ///   Returns the closest cluster to an input point.
        /// </summary>
        /// 
        /// <param name="point">The input vector.</param>
        /// <returns>
        ///   The index of the nearest cluster
        ///   to the given data point. </returns>
        ///   
        [Obsolete("Please use Clusters.Nearest() instead.")]
        public int Nearest(double[] point)
        {
            return Clusters.Nearest(point);
        }

        /// <summary>
        ///   Returns the closest cluster to an input point.
        /// </summary>
        /// 
        /// <param name="points">The input vector.</param>
        /// <returns>
        ///   The index of the nearest cluster
        ///   to the given data point. </returns>
        ///   
        [Obsolete("Please use Clusters.Nearest() instead.")]
        public int[] Nearest(double[][] points)
        {
            return Clusters.Nearest(points);
        }
        #endregion

    }
}
