// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// Copyright © Milos Simic, 2017
// milos.simic.ms at gmail.it
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
    using Accord.Math.Random;
    using Accord.Math.Distances;
    using Math.Optimization;
    using Statistics;
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Fast k-means clustering algorithm.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Mini-Batch K-Means clustering algorithm is a modification of the K-Means
    ///   algorithm. </para>
    /// <para>
    ///   In each iteration, it uses only a portion of data to update the cluster centroids with the gradient step.
    ///   The subsets of data are called mini-batches and are randomly sampled from the whole dataset in each iteration.
    /// </para> 
    /// <para>
    ///   Mini-Batch K-Means is faster than k-means for large datasets since batching reduces computational time of the algorithm. 
    /// </para>
    /// <para>
    ///   The algorithm is composed of the following steps:
    ///   <list type="number">
    ///     <item><description>
    ///         Place K points into the space represented by the objects that are
    ///         being clustered. These points represent initial group centroids.
    ///     </description></item>
    ///     <item><description>
    ///         Form a batch by choosing B objects from the whole input dataset.
    ///         For each object in the batch, determine the group that has the closest centroid.
    ///         Then, update the centroid with a gradient step.
    ///     </description></item>
    ///     <item><description>
    ///         Repeat step 2 until the centroids converge or the maximal number of iterations has been performed.
    ///     </description></item>
    ///   </list></para>
    /// 
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       D. Sculley. Web-Scale K-Means Clustering. Available on:
    ///       https://www.eecs.tufts.edu/~dsculley/papers/fastkmeans.pdf </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    ///     <code source="Unit Tests\Accord.Tests.MachineLearning\Clustering\MiniBatchKMeansTest.cs" region="doc_learn" />
    /// </example>
    /// 
    /// <seealso cref="KMeans"/>
    ///
    public class MiniBatchKMeans: KMeans 
    {
        private int batchSize;
        private int? initializationBatchSize;
        private int numberOfInitializations = 1;

        /// <summary>
        ///   Gets the labels assigned for each data point in the last 
        ///   call to <see cref="Learn(double[][], double[])"/>.
        /// </summary>
        /// 
        /// <value>The labels.</value>
        /// 
        public int[] Labels { get; private set; }

        /// <summary>
        ///   Initializes a new instance of Mini-Batch K-Means algorithm
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide input data.</param>
        /// <param name="batchSize">The size of batches.</param>       
        /// <param name="distance">The distance function to use. Default is to
        /// use the <see cref="Accord.Math.Distance.SquareEuclidean(double[], double[])"/> Euclidean distance.</param>
        /// 
        public MiniBatchKMeans(int k, int batchSize, IDistance<double[]> distance) : base(k, distance)
        {
            this.BatchSize = batchSize;
        }

        /// <summary>
        ///   Initializes a new instance of KMeans algorithm
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide input data.</param>       
        /// <param name="batchSize">The size of batches.</param> 
        /// 
        public MiniBatchKMeans(int k, int batchSize) : base(k)
        {
            this.BatchSize = batchSize;
        }

        /// <summary>
        ///   Gets or sets the size of the batch used during initialization.
        /// </summary>
        ///
        public int? InitializationBatchSize
        {
            get
            {
                 return this.initializationBatchSize;
            }
            set 
            {
                if (value.HasValue)
                {
                    if (value < this.K)
                    {
                        throw new ArgumentException("Batch size for initialization must be greater than the number of clusters.");
                    }
                    if (value <= 0)
                    {
                        throw new ArgumentException("Batch size for initialization must be a positive integer.");
                    }
                    
                }
                this.initializationBatchSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of different initializations of the centroids.
        /// </summary>
        ///
        public int NumberOfInitializations
        {
            get { return this.numberOfInitializations; }
            set {
                if (value <= 0)
                {
                    throw new ArgumentException("Number of random initializations must be a positive integer.");
                }
                this.numberOfInitializations = value;
            }
        }

        /// <summary>
        ///   Gets or sets the size of batches.
        /// </summary>
        ///
        public int BatchSize
        {
            get { return this.batchSize; }
            set { 
                if (value <= 0)
                {
                    throw new ArgumentException("Batch size should be a positive integer.");
                }
                this.batchSize = value; 
            }
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the desired outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="weights">The weight of importance for each input sample.</param>
        /// <returns>A model that has learned how to produce suitable outputs
        /// given the input data <paramref name="x" />.</returns>
        public override KMeansClusterCollection Learn(double[][] x, double[] weights = null)
        {
            if (x == null)
                throw new ArgumentNullException("x", "No data supplied to Mini-Batch K-Means. The parameter x cannot be null.");

            if (x.Length < K)
                throw new ArgumentException("x", "Not enough points. There should be more points than the number K of clusters.");

            if (weights == null)
            {
                weights = Vector.Ones(x.Length);
            }
            else
            {
                if (x.Length != weights.Length)
                    throw new DimensionMismatchException("weights", "Data weights vector must be the same length as data samples.");
            }
            if (this.InitializationBatchSize > x.Length)
            {
                this.InitializationBatchSize = x.Length;
            }
    
            double weightSum = weights.Sum();

            if (weightSum <= 0)
                throw new ArgumentException("weights", "Not enough points. There should be more points than the number K of clusters.");

            if (!x.IsRectangular())
                throw new DimensionMismatchException("x", "The points matrix should be rectangular. The vector at position {} has a different length than previous ones.");
                
            if (this.batchSize > x.Length)
            {
                throw new ArgumentException("Not enough points. There should be more points in the dataset than in a batch. ");
            }
            if (this.initializationBatchSize.HasValue == false)
            {
                this.InitializationBatchSize = 3 * this.K;
            }

            int k = this.K;
            int rows = x.Length;
            int cols = x[0].Length;

            // Initial variables
            int[] labels = new int[rows];
            double[] count = new double[k];
            double[][] centroids = Clusters.Centroids;
            double[][] newCentroids = Jagged.Zeros(k, cols);
            int[] batchIndices = new int[this.batchSize];

            Iterations = 0;

            bool shouldStop = false;

            if(Clusters.Centroids != null)
            {
                InitializeCentroids(x, weights);
            }
            
            for (int i = 0; i < k; i++){
                count[i] = 0;
            }

            while (!shouldStop) // Main loop
            {
                Iterations = Iterations + 1;
                // Getting indices of the points in this iteration's batch
                batchIndices = MakeBatch(rows, this.batchSize); 
                 
                foreach (int index in batchIndices)
                {
                    // Caching the center nearest to the point x[index]
                    // and storing it in labels[index].
                    double[] point = x[index];
                    int clusterIndex = Clusters.Decide(point);
                    labels[index] = clusterIndex;
                }
            
                // The centroids from the previous iteration will remain in the variable centroids.
                // The refined centroids will be stored in the variable newCentroids on which we are going to operate
                // in this iteration.
                for (int i = 0; i < centroids.Length; i++)
                    for (int j = 0; j < centroids[i].Length; j++)
                        newCentroids[i][j] = centroids[i][j];

                // Updating the centroids.
                foreach(int pointIndex in batchIndices)
                {
                    double[] point = x[pointIndex];
                    int clusterIndex = labels[pointIndex];
                    count[clusterIndex]++;
                    double eta = 1.0 / count[clusterIndex];
                    // Gradient step.
                    for (int i = 0; i < newCentroids[clusterIndex].Length; i++)
                    {
                        newCentroids[clusterIndex][i] = (1.0 - eta) * newCentroids[clusterIndex][i] + eta * point[i] * weights[pointIndex];
                    }
                };

                // The algorithm stops when there is no further change in the
                // centroids (relative difference is lower than the threshold).
                shouldStop = converged(centroids, newCentroids);

                // Copying the refined centroids
                // from the variable newCentroids to the variable centroids.
                for (int i = 0; i < centroids.Length; i++)
                    for (int j = 0; j < centroids[i].Length; j++)
                        centroids[i][j] = newCentroids[i][j];
            }
            // ... decide for every point in x?

            for (int i = 0; i < Clusters.Centroids.Length; i++)
            {
                // Computing the proportion of samples in the cluster.
                Clusters.Proportions[i] = count[i] / weightSum;
            }

            this.Labels = labels;

            ComputeInformation(x, labels);

            Accord.Diagnostics.Debug.Assert(Clusters.NumberOfClasses == K);
            Accord.Diagnostics.Debug.Assert(Clusters.NumberOfOutputs == K);
            Accord.Diagnostics.Debug.Assert(Clusters.NumberOfInputs == x[0].Length);

            return Clusters;
        }

        /// <summary>
        /// Creates a random batch.
        /// </summary>
        /// <param name="totalNumberOfPoints">The size of the model input dataset.</param>
        /// <param name="batchSize">The size of the batch.</param>
        /// <returns>An array of indices of the input objects which the created batch contains.</returns>
        private int[] MakeBatch(int totalNumberOfPoints, int batchSize)
        {
            // The variable chosenIndices is a hash set that contains
            // the indices of the points that we have selected 
            // to put in this batch.
            HashSet<int> chosenIndices = new HashSet<int>();
            // Randomly select an index
            // and if it has not already been selected
            // store it in chosenIndices.
            // Repeat until batchSize indices have been selected.S
            while (chosenIndices.Count < batchSize)
            {
                int index = Generator.Random.Next(totalNumberOfPoints);
                if (chosenIndices.Contains(index) == false)
                {
                    chosenIndices.Add(index);
                }
            }
            // Return the indices as an array of ints.
            int[] arrayOfIndices = new int[batchSize];
            chosenIndices.CopyTo(arrayOfIndices);
            return arrayOfIndices;
        }
        /// <summary>
        /// Initializes the centroids.
        /// </summary>
        /// <param name="data">The model inputs.</param>
        /// <param name="weights">The weight of importance for each input sample.</param>
        private void InitializeCentroids(double[][] data, double[] weights)
        {   
            int rows = data.Length;
            int cols = data[0].Length;
            
            // indices of centroids are placed in centroidIndices
            int[] centroidIndices = MakeBatch(rows, this.InitializationBatchSize.Value);
            
            // indices of the points in the validation batch are placed in validationIndices
            int[] validationIndices = MakeBatch(rows, this.InitializationBatchSize.Value);
            
            // the actual centroids
            double[][] centroidBatch = new double[this.InitializationBatchSize.Value][];
            
            // the actual points in the validation batch
            double[][] validationBatch = new double[this.InitializationBatchSize.Value][];
            
            // the weights of the validation points
            double[] validationWeights = new double[this.InitializationBatchSize.Value];
            
            // temporary variables
            double minDistortion = -1;
            double[][] bestCentroids = new double[this.numberOfInitializations][];
            
            for (int i = 0; i < this.numberOfInitializations; i++)
            {
                for (int j = 0; j < this.InitializationBatchSize; j++)
                {
                    // Copying points to batches
                    int centroidIndex = centroidIndices[j];
                    int validationIndex = validationIndices[j];
                    centroidBatch[j] = new double[cols];
                    validationBatch[j] = new double[cols];
                    data[centroidIndex].CopyTo(centroidBatch[j]);
                    data[validationIndex].CopyTo(validationBatch[j]);
                    validationWeights[j] = weights[validationIndex];
                }
                // Computing distortion of the current centroid set.
                Clusters.Randomize(centroidBatch, UseSeeding);
                double distortion = Clusters.Distortion(validationBatch, weights : validationWeights);
                // If this is the very first centroid set
                // or is better than the best so far
                // we remember it.
                if (minDistortion == -1 || distortion < minDistortion)
                {
                    minDistortion = distortion;
                    bestCentroids = Clusters.Centroids;
                }
            }
            // Setting the initial centroids 
            // to the best found set.
            Clusters.Centroids = bestCentroids;
        }
    }
}