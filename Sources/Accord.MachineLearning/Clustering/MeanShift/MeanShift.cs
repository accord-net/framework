﻿// Accord Statistics Library
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
    using Accord.MachineLearning.Structures;
    using Accord.Math;
    using Accord.Math.Comparers;
    using Accord.Math.Distances;
    using Accord.Statistics.Distributions.DensityKernels;
    using Accord.Statistics.Visualizations;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    ///   Mean shift clustering algorithm.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Mean shift is a non-parametric feature-space analysis technique originally 
    ///   presented in 1975 by Fukunaga and Hostetler. It is a procedure for locating
    ///   the maxima of a density function given discrete data sampled from that function.
    ///   The method iteratively seeks the location of the modes of the distribution using
    ///   local updates. </para>
    /// <para>
    ///   As it is, the method would be intractable; however, some clever optimizations such as
    ///   the use of appropriate data structures and seeding strategies as shown in Lee (2011)
    ///   and Carreira-Perpinan (2006) can improve its computational speed.</para> 
    /// 
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia, The Free Encyclopedia. Mean-shift. Available on:
    ///       http://en.wikipedia.org/wiki/Mean-shift </description></item>
    ///     <item><description>
    ///       Comaniciu, Dorin, and Peter Meer. "Mean shift: A robust approach toward 
    ///       feature space analysis." Pattern Analysis and Machine Intelligence, IEEE 
    ///       Transactions on 24.5 (2002): 603-619. Available at:
    ///       http://ieeexplore.ieee.org/xpls/abs_all.jsp?arnumber=1000236 </description></item>
    ///     <item><description>
    ///       Conrad Lee. Scalable mean-shift clustering in a few lines of python. The
    ///       Sociograph blog, 2011. Available at: 
    ///       http://sociograph.blogspot.com.br/2011/11/scalable-mean-shift-clustering-in-few.html </description></item>
    ///     <item><description>
    ///       Carreira-Perpinan, Miguel A. "Acceleration strategies for Gaussian mean-shift image
    ///       segmentation." Computer Vision and Pattern Recognition, 2006 IEEE Computer Society 
    ///       Conference on. Vol. 1. IEEE, 2006. Available at:
    ///       http://ieeexplore.ieee.org/xpl/articleDetails.jsp?arnumber=1640881
    ///     </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// // Declare some observations
    /// double[][] observations = 
    /// {
    ///     new double[] { -5, -2, -1 },
    ///     new double[] { -5, -5, -6 },
    ///     new double[] {  2,  1,  1 },
    ///     new double[] {  1,  1,  2 },
    ///     new double[] {  1,  2,  2 },
    ///     new double[] {  3,  1,  2 },
    ///     new double[] { 11,  5,  4 },
    ///     new double[] { 15,  5,  6 },
    ///     new double[] { 10,  5,  6 },
    /// };
    /// 
    /// // Create a uniform kernel density function
    /// UniformKernel kernel = new UniformKernel();
    /// 
    /// // Create a new Mean-Shift algorithm for 3 dimensional samples
    /// MeanShift meanShift = new MeanShift(dimension: 3, kernel: kernel, bandwidth: 1.5 );
    /// 
    /// // Compute the algorithm, retrieving an integer array
    /// //  containing the labels for each of the observations
    /// int[] labels = meanShift.Compute(observations);
    /// 
    /// // As a result, the first two observations should belong to the
    /// //  same cluster (thus having the same label). The same should
    /// //  happen to the next four observations and to the last three.
    /// </code>
    /// 
    /// <para>
    ///   The following example demonstrates how to use the Mean Shift algorithm
    ///   for color clustering. It is the same code which can be found in the
    ///   <a href="">color clustering sample application</a>.</para>
    ///   
    /// <code>
    /// 
    ///  int pixelSize = 3;   // RGB color pixel
    ///  double sigma = 0.06; // kernel bandwidth
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
    ///  // Create a MeanShift algorithm using given bandwidth
    ///  //   and a Gaussian density kernel as kernel function.
    ///  MeanShift meanShift = new MeanShift(pixelSize, new GaussianKernel(3), sigma);
    /// 
    /// 
    ///  // Compute the mean-shift algorithm until the difference in
    ///  //  shifting means between two iterations is below 0.05
    ///  int[] idx = meanShift.Compute(pixels, 0.05, maxIterations: 10);
    /// 
    /// 
    ///  // Replace every pixel with its corresponding centroid
    ///  pixels.ApplyInPlace((x, i) => meanShift.Clusters.Modes[idx[i]]);
    /// 
    ///  // Retrieve the resulting image in a picture box
    ///  Bitmap result; arrayToImage.Convert(pixels, out result);
    /// </code>
    /// 
    /// <para>
    ///   The original image is shown below:</para>
    /// 
    ///   <img src="..\images\mean-shift-start.png" />
    ///   
    /// <para>
    ///   The resulting image will be:</para>
    /// 
    ///   <img src="..\images\mean-shift-end.png" />
    /// 
    /// </example>
    ///     
    /// <see cref="KMeans"/>
    /// <see cref="KModes{T}"/>
    /// 
    [Serializable]
    public class MeanShift : IClusteringAlgorithm<double[]>
    {

        private int dimension;
        private double bandwidth;
        private int maximum;
        private bool cut = true;

        private IRadiallySymmetricKernel kernel;
        private MeanShiftClusterCollection clusters;

        /// <summary>
        ///   Gets the clusters found by Mean Shift.
        /// </summary>
        /// 
        public MeanShiftClusterCollection Clusters
        {
            get { return clusters; }
        }

        /// <summary>
        ///   Gets or sets the <see cref="IMetric{T}"/> used to 
        ///   compute distances between points in the clustering.
        /// </summary>
        /// 
        public IMetric<double[]> Distance { get; set; }

        /// <summary>
        ///   Gets or sets the bandwidth (radius, or smoothness)
        ///   parameter to be used in the mean-shift algorithm.
        /// </summary>
        /// 
        public double Bandwidth
        {
            get { return bandwidth; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value",
                        "Value must be positive and higher than zero.");
                bandwidth = value;
            }
        }

        /// <summary>
        ///   Gets or sets the maximum number of neighbors which should be
        ///   used to determine the direction of the mean-shift during the
        ///   computations. Default is zero (unlimited number of neighbors).
        /// </summary>
        /// 
        public int Maximum
        {
            get { return maximum; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value",
                        "Value most be non-negative.");
                maximum = value;
            }
        }

        /// <summary>
        ///   Gets or sets whether the algorithm can use parallel
        ///   processing to speedup computations. Enabling parallel
        ///   processing can, however, result in different results 
        ///   at each run.
        /// </summary>
        /// 
        public bool UseParallelProcessing { get; set; }

        /// <summary>
        ///   Gets or sets whether to use the agglomeration shortcut,
        ///   meaning the algorithm will stop early when it detects that
        ///   a sample is going to follow the same path as another sample
        ///   when running in parallel.
        /// </summary>
        /// 
        public bool UseAgglomeration { get; set; }
        /// <summary>
        ///   Gets or sets whether to use seeding to initialize the algorithm.
        ///   With seeding, new points will be sampled from an uniform grid in
        ///   the range of the input points to be used as seeds. Otherwise, the
        ///   input points themselves will be used as the initial centroids for 
        ///   the algorithm.
        /// </summary>
        /// 
        public bool UseSeeding { get; set; }

        /// <summary>
        ///   Gets or sets whether cluster labels should be computed
        ///   at the end of the learning iteration. Setting to <c>False</c>
        ///   might save a few computations in case they are not necessary.
        /// </summary>
        /// 
        public bool ComputeLabels { get; set; }

        /// <summary>
        ///   Gets or sets whether cluster proportions should be computed
        ///   at the end of the learning iteration. Setting to <c>False</c>
        ///   might save a few computations in case they are not necessary.
        /// </summary>
        /// 
        public bool ComputeProportions { get; set; }

        /// <summary>
        ///   Gets the dimension of the samples being 
        ///   modeled by this clustering algorithm.
        /// </summary>
        /// 
        public int Dimension
        {
            get { return dimension; }
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
        ///   Creates a new <see cref="MeanShift"/> algorithm.
        /// </summary>
        /// 
        /// <param name="dimension">The dimension of the samples to be clustered.</param>
        /// <param name="bandwidth">The bandwidth (also known as radius) to consider around samples.</param>
        /// <param name="kernel">The density kernel function to use.</param>
        /// 
        public MeanShift(int dimension, IRadiallySymmetricKernel kernel, double bandwidth)
        {
            this.dimension = dimension;
            this.kernel = kernel;
            this.Bandwidth = bandwidth;
            this.Distance = new Accord.Math.Distances.Euclidean();
            this.UseParallelProcessing = true;
            this.MaxIterations = 100;
            this.Tolerance = 1e-3;
            this.ComputeLabels = true;
            this.ComputeProportions = true;
        }

        /// <summary>
        ///   Divides the input data into clusters. 
        /// </summary>     
        /// 
        /// <param name="points">The data where to compute the algorithm.</param>
        /// 
        public int[] Compute(double[][] points)
        {
            return Compute(points, Vector.Ones<int>(points.Length));
        }

        /// <summary>
        ///   Divides the input data into clusters. 
        /// </summary>     
        /// 
        /// <param name="points">The data where to compute the algorithm.</param>
        /// <param name="weights">The weight associated with each data point.</param>
        /// 
        public int[] Compute(double[][] points, int[] weights)
        {
            if (points.Length != weights.Length)
            {
                throw new DimensionMismatchException("weights",
                    "The weights and points vector must have the same dimension.");
            }

            // First of all, construct map of the original points. We will
            // be saving the weight of every point in the node of the tree.
            KDTree<int> tree = KDTree.FromData(points, weights, Distance);

            // Let's sample some points in the problem surface
            double[][] seeds = createSeeds(points, 2 * Bandwidth);

            // Now, we will duplicate those points and make them "move" 
            // into this surface in the direction of the surface modes.
            double[][] current = seeds.MemberwiseClone();

            // We will store any modes that we find here
            var maxima = new ConcurrentStack<double[]>();

            // Optimization for uniform kernel
            Action<ICollection<KDTreeNodeDistance<int>>, double[]> func;
            if (kernel is UniformKernel)
                func = uniform;
            else func = general;

            // For each seed
            if (UseParallelProcessing)
            {
                Parallel.For(0, current.Length, i =>
                    move(tree, current, i, maxima, func));

                for (int i = 0; i < current.Length; i++)
                    supress(current, i, maxima);
            }
            else
            {
                for (int i = 0; i < current.Length; i++)
                    move(tree, current, i, maxima, func);
            }

            var modes = maxima.ToArray();

            // At this point, the current points have moved into
            // the location of the modes of the surface. Now we
            // have to backtrack and check, for each mode, from
            // where those points departed from.

            int[] labels = classify(modes: modes, points: current);

            // Now we create a decision map using the original seed positions
            tree = KDTree.FromData(seeds, labels, Distance, inPlace: true);


            clusters = new MeanShiftClusterCollection(this, modes.Length, tree, modes);

            if (ComputeLabels || ComputeProportions)
            {
                int sum = 0;
                int[] counts = new int[modes.Length];
                labels = new int[points.Length];
                for (int i = 0; i < labels.Length; i++)
                {
                    int j = tree.Nearest(points[i]).Value;
                    labels[i] = j;
                    counts[j] += weights[i];
                    sum += weights[i];
                }

                for (int i = 0; i < counts.Length; i++)
                    clusters.Proportions[i] = counts[i] / (double)sum;

                return labels;
            }

            return null;
        }

        private double[] move(KDTree<int> tree, double[][] points, int index,
            ConcurrentStack<double[]> modes,
            Action<ICollection<KDTreeNodeDistance<int>>, double[]> computeMean)
        {
            double[] current = points[index];
            double[] mean = new double[current.Length];
            double[] shift = new double[current.Length];

            // we will keep moving it in the
            // direction of the density modes

            int iterations = 0;

            // until convergence or max iterations reached
            while (iterations < MaxIterations)
            {
                iterations++;

                // Get points near the current point
                var neighbors = tree.Nearest(current, Bandwidth * 3, maximum);

                // compute the mean on the region 
                computeMean(neighbors, mean);

                // extract the mean shift vector
                for (int j = 0; j < mean.Length; j++)
                    shift[j] = current[j] - mean[j];

                // move the point towards a mode
                for (int j = 0; j < mean.Length; j++)
                    current[j] = mean[j];

                // Check if we are near to a maximum point
                if (cut)
                {
                    // Check if we are near a known mode
                    foreach (double[] mode in modes)
                    {
                        // compute the distance between points
                        // if they are near, they are duplicates
                        if (Distance.Distance(points[index], mode) < Bandwidth)
                        {
                            // Yes, we are close to a known mode. Let's substitute 
                            // this point with a reference to this nearest mode
                            return points[index] = mode; // and stop moving this point
                        }
                    }
                }

                // check for convergence: magnitude of the mean shift
                // vector converges to zero (Comaniciu 2002, page 606)
                if (Norm.Euclidean(shift) < Tolerance * Bandwidth)
                    return supress(points, index, modes);
            }

            throw new NotImplementedException();
        }

        private double[] supress(double[][] seeds, int index, ConcurrentStack<double[]> candidates)
        {
            // Check if we are near to one known mode
            foreach (double[] mode in candidates)
            {
                // compute the distance between points
                // if they are near, they are duplicates
                if (Distance.Distance(seeds[index], mode) < Bandwidth)
                {
                    // Yes, we are close to a known mode. Let's substitute 
                    // this point with a reference to this nearest mode
                    return seeds[index] = mode;
                }
            }

            // No, we are not close to any known mode. As
            // such, this point should probably be a mode
            candidates.Push(seeds[index]);
            return seeds[index];
        }

        private void general(ICollection<KDTreeNodeDistance<int>> neighbors, double[] result)
        {
            Array.Clear(result, 0, result.Length);

            double sum = 0;
            double h = Bandwidth * Bandwidth;

            // Compute weighted mean
            foreach (KDTreeNodeDistance<int> neighbor in neighbors)
            {
                double distance = neighbor.Distance; // ||(x-xi)||
                double[] point = neighbor.Node.Position;
                int weight = neighbor.Node.Value; // count

                double u = distance * distance;

                // Compute g = -k'(|| (x - xi) / h ||²)
                double g = -kernel.Derivative(u / h) * weight;
                for (int i = 0; i < result.Length; i++)
                    result[i] += g * point[i];

                sum += g;
            }

            // Normalize
            if (sum != 0)
            {
                for (int i = 0; i < result.Length; i++)
                    result[i] /= sum;
            }
        }

        private static void uniform(ICollection<KDTreeNodeDistance<int>> neighbors, double[] result)
        {
            Array.Clear(result, 0, result.Length);

            double sum = 0;

            // Optimization for the uniform case: In this case, we just 
            // have to compute the average mean of the neighbor points
            foreach (KDTreeNodeDistance<int> neighbor in neighbors)
            {
                double[] point = neighbor.Node.Position;
                int weight = neighbor.Node.Value; // count

                for (int i = 0; i < result.Length; i++)
                    result[i] += point[i] * weight;

                sum += weight; // total number of points
            }

            // Normalize
            if (sum != 0)
            {
                for (int i = 0; i < result.Length; i++)
                    result[i] /= sum;
            }
        }

        private double[][] createSeeds(double[][] points, double binSize)
        {
            if (binSize == 0)
            {
                double[][] seeds = new double[points.Length][];
                for (int i = 0; i < seeds.Length; i++)
                {
                    seeds[i] = new double[Dimension];
                    for (int j = 0; j < seeds[i].Length; j++)
                        seeds[i][j] = points[i][j];
                }

                return seeds;
            }
            else
            {
                int minBin = 1;

                // Create bins as suggested by (Conrad Lee, 2011):
                //
                // The dictionary holds the positions of the bins as keys and the
                // number of occurrences of a given point as the value associated 
                // with this key. The comparer tells the dictionary how to compare
                // integer vectors on an element-by-element basis.

                var bins = new Dictionary<int[], int>(new ArrayComparer<int>());

                // for each point
                foreach (var point in points)
                {
                    // create a indexing key
                    int[] key = new int[Dimension];
                    for (int j = 0; j < point.Length; j++)
                        key[j] = (int)(point[j] / binSize);

                    // increase the counter in the key
                    int previous;
                    if (bins.TryGetValue(key, out previous))
                        bins[key] = previous + 1;
                    else bins[key] = 1;
                }

                // now, read the dictionary and create seeds
                // for bins which contain more than one point

                var seeds = new List<double[]>();

                // for each bin-count pair
                foreach (var pair in bins)
                {
                    if (pair.Value >= minBin)
                    {
                        // recreate the point
                        int[] bin = pair.Key;

                        double[] point = new double[Dimension];
                        for (int i = 0; i < point.Length; i++)
                            point[i] = bin[i] * binSize;

                        seeds.Add(point);
                    }
                }

                return seeds.ToArray();
            }
        }

        private int[] classify(double[][] modes, double[][] points)
        {
            // classify seeds using a minimum distance classifier
            int[] labels = new int[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                int imin = 0;
                double dmin = Double.PositiveInfinity;
                for (int j = 0; j < modes.Length; j++)
                {
                    double d = Distance.Distance(modes[j], points[i]);

                    if (d < dmin)
                    {
                        imin = j;
                        dmin = d;
                    }
                }

                labels[i] = imin;
            }

            // Order the labels by their proportion
            int[] counts = Vector.Histogram(labels, modes.Length);
            int[] idx = Vector.Range(0, modes.Length);
            Array.Sort(counts, idx);

            for (int i = 0; i < labels.Length; i++)
                labels[i] = idx[labels[i]];

            return labels;
        }



        IClusterCollection<double[]> IClusteringAlgorithm<double[]>.Clusters
        {
            get { throw new NotImplementedException(); }
        }
    }
}
