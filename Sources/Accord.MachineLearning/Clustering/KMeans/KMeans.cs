// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
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
    using System.Runtime.Serialization;
    using Accord.Math.Distances;
    using System.Collections.Generic;
    using System.Reflection;
    using Accord.Statistics;
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Initialization schemes for clustering algorithms.
    /// </summary>
    /// 
    public enum Seeding
    {
        /// <summary>
        ///   Do not perform initialization.
        /// </summary>
        /// 
        Fixed,

        /// <summary>
        ///   Randomly sample points to become centroids.
        /// </summary>
        /// 
        Uniform,

        /// <summary>
        ///   Use the kmeans++ seeding algorithm for generating initial centroids.
        /// </summary>
        /// 
        KMeansPlusPlus,

        /// <summary>
        ///   Use the PAM BUILD algorithm for generating initial centroids.
        /// </summary>
        /// 
        PamBuild
    };

    /// <summary>
    ///   Lloyd's k-Means clustering algorithm.
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
    /// 
    /// <example>
    /// <para>
    ///   How to perform clustering with K-Means.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\Clustering\KMeansTest.cs" region="doc_learn" />
    ///   
    /// <para>
    ///   How to perform clustering with K-Means applying different weights to different columns (dimensions) in the data.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\Clustering\KMeansTest.cs" region="doc_learn_weights" />
    ///   
    /// <para>
    ///   How to perform clustering with K-Means with mixed discrete, continuous and categorical data.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\Clustering\KMeansTest.cs" region="doc_learn_mixed" />
    ///   
    /// <para>
    ///   The following example demonstrates how to use the K-Means algorithm for color clustering. It is the same code which can be 
    ///   found in the <a href="https://github.com/accord-net/framework/wiki/Sample-applications#clustering-k-means-and-meanshift">
    ///   color clustering sample application</a>.</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.Vision\ColorClusteringTest.cs" region="doc_kmeans" />
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
#if !NETSTANDARD1_4
    [SerializationBinder(typeof(KMeans.KMeansBinder))]
#endif
    public class KMeans : ParallelLearningBase,
        IUnsupervisedLearning<KMeansClusterCollection, double[], int>,
#pragma warning disable 0618
        IClusteringAlgorithm<double[], double>
#pragma warning restore 0618
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
        ///   Gets or sets the cluster centroids. 
        /// </summary>
        /// 
        public double[][] Centroids
        {
            get { return clusters.Centroids; }
            set { clusters.Centroids = value; }
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
            get { return clusters.NumberOfInputs; }
        }

        /// <summary>
        ///   Gets or sets the distance function used
        ///   as a distance metric between data points.
        /// </summary>
        /// 
        public IDistance<double[], double[]> Distance
        {
            get { return clusters.Distance; }
            set { clusters.Distance = value; }
        }

        /// <summary>
        ///   Gets or sets whether covariance matrices for the clusters should 
        ///   be computed at the end of an iteration. Default is true.
        /// </summary>
        /// 
        public bool ComputeCovariances { get; set; }

        /// <summary>
        ///   Gets or sets whether the clustering distortion error (the
        ///   average distance between all data points and the cluster
        ///   centroids) should be computed at the end of the algorithm.
        ///   The result will be stored in <see cref="Error"/>. Default is true.
        /// </summary>
        /// 
        public bool ComputeError { get; set; }

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
        public int Iterations { get; protected set; }

        /// <summary>
        ///   Gets the cluster distortion error after the 
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
        public Seeding UseSeeding { get; set; }

        /// <summary>
        ///   Initializes a new instance of KMeans algorithm
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide input data.</param>       
        /// <param name="distance">The distance function to use. Default is to
        /// use the <see cref="Accord.Math.Distance.SquareEuclidean(double[], double[])"/> distance.</param>
        /// 
        [Obsolete("Please specify the distance function using classes instead of lambda functions.")]
        public KMeans(int k, Func<double[], double[], double> distance)
            : this(k, Accord.Math.Distance.GetDistance(distance))
        {
        }

        /// <summary>
        ///   Initializes a new instance of the K-Means algorithm
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide the input data into.</param>    
        /// 
        public KMeans(int k)
            : this(k, new Accord.Math.Distances.SquareEuclidean()) { }

        /// <summary>
        ///   Initializes a new instance of the KMeans algorithm
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide the input data into.</param>    
        /// <param name="distance">The distance function to use. Default is to use the 
        /// <see cref="Accord.Math.Distance.SquareEuclidean(double[], double[])"/> distance.</param>
        /// 
        public KMeans(int k, IDistance<double[]> distance)
        {
            if (k <= 0)
                throw new ArgumentOutOfRangeException("k");

            if (distance == null)
                throw new ArgumentNullException("distance");

            this.Tolerance = 1e-5;
            this.ComputeCovariances = true;
            this.ComputeError = true;
            this.UseSeeding = Seeding.KMeansPlusPlus;
            this.MaxIterations = 0;

            // Create the object-oriented structure to hold
            //  information about the k-means' clusters.
            this.clusters = new KMeansClusterCollection(k, distance);

            this.ParallelOptions = new ParallelOptions();
        }

        /// <summary>
        ///   Randomizes the clusters inside a dataset.
        /// </summary>
        /// 
        /// <param name="points">The data to randomize the algorithm.</param>
        /// 
        public void Randomize(double[][] points)
        {
            clusters.Randomize(points, UseSeeding);

            Accord.Diagnostics.Debug.Assert(clusters.NumberOfInputs == points[0].Length);
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the desired outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="weights">The weight of importance for each input sample.</param>
        /// <returns>A model that has learned how to produce suitable outputs
        /// given the input data <paramref name="x" />.</returns>
        public virtual KMeansClusterCollection Learn(double[][] x, double[] weights = null)
        {
            // Initial argument checking
            if (x == null)
                throw new ArgumentNullException("x");

            if (x.Length < K)
                throw new ArgumentException("Not enough points. There should be more points than the number K of clusters.", "x");

            if (weights == null)
                weights = Vector.Ones(x.Length);

            if (x.Length != weights.Length)
                throw new DimensionMismatchException("weights", "Data weights vector must be the same length as data samples.");

            double weightSum = weights.Sum();
            if (weightSum <= 0)
                throw new ArgumentException("Not enough points. There should be more points than the number K of clusters.", "x");

            int cols = x[0].Length;
            for (int i = 0; i < x.Length; i++)
                if (x[i].Length != cols)
                    throw new DimensionMismatchException("x", "The points matrix should be rectangular. The vector at position {} has a different length than previous ones.");

            compute(x, weights, weightSum);

            Accord.Diagnostics.Debug.Assert(clusters.NumberOfClasses == K);
            Accord.Diagnostics.Debug.Assert(clusters.NumberOfOutputs == K);
            Accord.Diagnostics.Debug.Assert(clusters.NumberOfInputs == x[0].Length);

            return clusters;
        }

        /// <summary>
        ///   Divides the input data into K clusters. 
        /// </summary>
        /// 
        /// <param name="data">The data where to compute the algorithm.</param>
        ///   
        [Obsolete("Please use Learn(x) instead.")]
        public int[] Compute(double[][] data)
        {
            return Compute(data, Vector.Ones(data.Length));
        }

        /// <summary>
        ///   Divides the input data into K clusters. 
        /// </summary>
        /// 
        /// <param name="data">The data where to compute the algorithm.</param>
        /// <param name="weights">The weight associated with each data point.</param>
        ///   
        [Obsolete("Please use Learn(x) instead.")]
        public virtual int[] Compute(double[][] data, double[] weights)
        {
            return Learn(data, weights).Decide(data);
        }

        /// <summary>
        /// Computes the information about each cluster (covariance, proportions and error).
        /// </summary>
        /// 
        /// <param name="data">The data points.</param>
        /// 
        protected void ComputeInformation(double[][] data)
        {
            if (ComputeCovariances || ComputeError)
                ComputeInformation(data, clusters.Decide(data));
        }

        /// <summary>
        /// Computes the information about each cluster (covariance, proportions and error).
        /// </summary>
        /// 
        /// <param name="data">The data points.</param>
        /// <param name="labels">The assigned labels.</param>
        /// 
        protected void ComputeInformation(double[][] data, int[] labels)
        {
            // Compute distortion and other metrics regarding the clustering
            if (ComputeCovariances)
            {
                // Compute cluster information (optional)
                // Note: If you get OutOfMemoryExceptions here, just disable the 
                // computation of variances by setting ComputeCovariances = false
                if (ParallelOptions.MaxDegreeOfParallelism == 1)
                {
                    for (int i = 0; i < clusters.Count; i++)
                        innerComputeCovariance(data, labels, i);
                }
                else
                {
                    Parallel.For(0, clusters.Count, ParallelOptions, i =>
                    {
                        innerComputeCovariance(data, labels, i);
                    });
                }
            }

            if (ComputeError)
            {
                Error = clusters.Distortion(data);
            }
        }

        private void innerComputeCovariance(double[][] data, int[] labels, int i)
        {
            double[][] centroids = clusters.Centroids;

            // Extract the data for the current cluster
            double[][] sub = data.Get(labels.Find(x => x == i));

            if (sub.Length > 0)
            {
                // Compute the current cluster variance
                clusters.Covariances[i] = sub.Covariance(centroids[i]);
            }
            else
            {
                // The cluster doesn't have any samples
                clusters.Covariances[i] = Jagged.Zeros(Dimension, Dimension);
            }
        }

        /// <summary>
        ///   Divides the input data into K clusters. 
        /// </summary>
        /// 
        /// <param name="data">The data where to compute the algorithm.</param>
        /// <param name="weights">The weight to consider for each data sample. This is used in weighted K-Means</param>
        /// <param name="weightSum">The total sum of the weights in <paramref name="weights"/>.</param>
        ///   
        private int[] compute(double[][] data, double[] weights, double weightSum)
        {
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
            var labels = new int[rows];
            var count = new double[k];
            double[][] centroids = clusters.Centroids;
            double[][] newCentroids = Jagged.Zeros(k, cols);

            Object[] syncObjects = new Object[K];
            for (int i = 0; i < syncObjects.Length; i++)
                syncObjects[i] = new Object();

            Iterations = 0;

            bool shouldStop = false;

            while (!shouldStop) // Main loop
            {
                Array.Clear(count, 0, count.Length);
                for (int i = 0; i < newCentroids.Length; i++)
                    Array.Clear(newCentroids[i], 0, newCentroids[i].Length);

                // First we will accumulate the data points
                // into their nearest clusters, storing this
                // information into the newClusters variable.

                // For each point in the data set,
                Parallel.For(0, data.Length, ParallelOptions, i =>
                {
                    // Get the point
                    double[] point = data[i];
                    double weight = weights[i];

                    // Get the nearest cluster centroid
                    int c = labels[i] = Clusters.Decide(point);

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

            for (int i = 0; i < clusters.Centroids.Length; i++)
            {
                // Compute the proportion of samples in the cluster
                clusters.Proportions[i] = count[i] / weightSum;
            }

            ComputeInformation(data, labels);

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
        protected bool converged(double[][] centroids, double[][] newCentroids)
        {
            Iterations++;

            if (MaxIterations > 0 && Iterations >= MaxIterations)
                return true;

            if (Token.IsCancellationRequested)
                return true;

            for (int i = 0; i < centroids.Length; i++)
            {
                double[] centroid = centroids[i];
                double[] newCentroid = newCentroids[i];

                for (int j = 0; j < centroid.Length; j++)
                {
                    if ((System.Math.Abs((centroid[j] - newCentroid[j]) / centroid[j])) >= Tolerance)
                        return false;
                }
            }

            return true;
        }

#pragma warning disable 0618
        IClusterCollection<double[]> IClusteringAlgorithm<double[]>.Clusters
        {
            get { return (IClusterCollection<double[]>)clusters; }
        }

        IClusterCollection<double[]> IUnsupervisedLearning<IClusterCollection<double[]>, double[], int>.Learn(double[][] x, double[] weights)
        {
            return (IClusterCollection<double[]>)Learn(x);
        }
#pragma warning restore 0618

        [OnDeserialized]
        private void OnDeserializedMethod(StreamingContext context)
        {
            if (this.Iterations == 0 && MaxIterations == 0 && Tolerance == 0)
            {
                this.Tolerance = 1e-5;
                this.ComputeCovariances = true;
                this.UseSeeding = Seeding.KMeansPlusPlus;
                this.ComputeError = true;
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
        [Obsolete("Please get the error value through this class' Error property.")]
        public int[] Compute(double[][] data, out double error)
        {
            int[] labels = Learn(data).Decide(data);
            error = Error;
            return labels;
        }


#region Serialization backwards compatibility
#if !NETSTANDARD1_4
        internal class KMeansBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                AssemblyName name = new AssemblyName(assemblyName);
                if (typeName.StartsWith("System.Collections.Generic.List`1[[Accord.MachineLearning.KMeansCluster, Accord.MachineLearning, Version=2."))
                    return typeof(List<KMeansCluster_2_13>);

                if (name.Version < new Version(3, 1, 0))
                {
                    if (typeName == "Accord.MachineLearning.KMeans")
                        return typeof(KMeans_2_13);
                    else if (typeName == "Accord.MachineLearning.KMeansClusterCollection")
                        return typeof(KMeansClusterCollection_2_13);
                    else if (typeName == "Accord.MachineLearning.KMeansCluster")
                        return typeof(KMeansCluster_2_13);
                }

                return null;
            }
        }

#pragma warning disable 0169
#pragma warning disable 0649

        [Serializable]
        class KMeans_2_13
        {
            public KMeansClusterCollection_2_13 clusters;
            public bool ComputeInformation { get; set; }
            public bool UseCentroidSeeding { get; set; }
            public int MaxIterations { get; set; }
            public double Tolerance { get; set; }
            public int Iterations { get; private set; }

            public static implicit operator KMeans(KMeans_2_13 obj)
            {
                var func = obj.clusters.distance;
                var dist = Accord.Math.Distance.GetDistance(func);

                KMeans kmeans = new KMeans(obj.clusters.clusters.Count)
                {
                    ComputeCovariances = obj.ComputeInformation,
                    ComputeError = true,
                    MaxIterations = obj.MaxIterations,
                    Tolerance = obj.Tolerance,
                    Iterations = obj.Iterations,
                    UseSeeding = obj.UseCentroidSeeding ? Seeding.KMeansPlusPlus : Seeding.Fixed,
                    Distance = dist
                };

                if (obj.Iterations == 0 && obj.MaxIterations == 0 && obj.Tolerance == 0)
                {
                    kmeans.Tolerance = 1e-5;
                    kmeans.ComputeCovariances = true;
                    kmeans.UseSeeding = Seeding.KMeansPlusPlus;
                }

                for (int i = 0; i < kmeans.Clusters.Count; i++)
                {
                    kmeans.Clusters.Proportions[i] = obj.clusters.proportions[i];
                    kmeans.Clusters.Centroids[i] = obj.clusters.centroids[i];
                    kmeans.Clusters.Covariances[i] = obj.clusters.covariances[i].ToJagged();
                }

                return kmeans;
            }
        }

        [Serializable]
        class KMeansClusterCollection_2_13
        {
            public List<KMeansCluster_2_13> clusters;
            public Func<double[], double[], double> distance;
            public double[] proportions;
            public double[][] centroids;
            public double[][,] covariances;
        }

        [Serializable]
        class KMeansCluster_2_13
        {
            public KMeansClusterCollection_2_13 owner;
            public int index;
        }

#pragma warning restore 0169
#pragma warning restore 0649
#endif
#endregion



    }
}
