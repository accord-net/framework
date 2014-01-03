// Accord Statistics Library
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
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Accord.MachineLearning.Structures;
    using Accord.Math;
    using Accord.Math.Comparers;
    using Accord.Statistics.Distributions.DensityKernels;

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

        private KDTree<int> tree;

        private IRadiallySymmetricKernel kernel;
        private MeanShiftClusterCollection clusters;
        private Func<double[], double[], double> distance;


        /// <summary>
        ///   Gets the clusters found by Mean Shift.
        /// </summary>
        /// 
        public MeanShiftClusterCollection Clusters
        {
            get { return clusters; }
        }

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
        ///   Gets or sets whether the mean-shift can be shortcut
        ///   as soon as a mean enters the neighborhood of a local
        ///   maxima candidate. Default is true.
        /// </summary>
        /// 
        public bool Agglomerate
        {
            get { return cut; }
            set { cut = value; }
        }

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
            this.distance = Accord.Math.Distance.SquareEuclidean;
        }

        /// <summary>
        ///   Divides the input data into clusters. 
        /// </summary>     
        /// 
        /// <param name="points">The data where to compute the algorithm.</param>
        /// <param name="threshold">The relative convergence threshold
        /// for the algorithm. Default is 1e-3.</param>
        /// 
        public int[] Compute(double[][] points, double threshold = 1e-3)
        {
            return Compute(points, threshold, 100);
        }

        /// <summary>
        ///   Divides the input data into clusters. 
        /// </summary>     
        /// 
        /// <param name="points">The data where to compute the algorithm.</param>
        /// <param name="threshold">The relative convergence threshold
        /// for the algorithm. Default is 1e-3.</param>
        /// <param name="maxIterations">The maximum number of iterations. Default is 100.</param>
        /// 
        public int[] Compute(double[][] points, double threshold, int maxIterations = 100)
        {
            // first, select initial points
            double[][] seeds = createSeeds(points, 2 * Bandwidth);
            var maxcandidates = new ConcurrentStack<double[]>();

            // construct map of the data
            tree = KDTree.FromData<int>(points, distance);

            // now, for each initial point 
            Parallel.For(0, seeds.Length,
#if DEBUG
 new ParallelOptions() { MaxDegreeOfParallelism = 1 },
#endif

            (index) =>
            {
                double[] point = seeds[index];
                double[] mean = new double[point.Length];
                double[] delta = new double[point.Length];

                // we will keep moving it in the
                // direction of the density modes

                int iterations = 0;

                // until convergence or max iterations reached
                while (iterations < maxIterations)
                {
                    iterations++;

                    // compute the shifted mean 
                    computeMeanShift(point, mean);

                    // extract the mean shift vector
                    for (int j = 0; j < mean.Length; j++)
                        delta[j] = point[j] - mean[j];

                    // update the point towards a mode
                    for (int j = 0; j < mean.Length; j++)
                        point[j] = mean[j];

                    // Check if we are already near any maximum point
                    if (cut && nearest(point, maxcandidates) != null)
                        break;

                    // check for convergence: magnitude of the mean shift
                    // vector converges to zero (Comaniciu 2002, page 606)
                    if (Norm.Euclidean(delta) < threshold * Bandwidth)
                        break;
                }

                if (cut)
                {
                    double[] match = nearest(point, maxcandidates);

                    if (match != null)
                        seeds[index] = match;

                    else maxcandidates.Push(point);
                }
            });


            // suppress non-maximum points
            double[][] maximum = cut ? maxcandidates.ToArray() : supress(seeds);

            // create a decision map using seeds
            int[] seedLabels = classifySeeds(seeds, maximum);
            tree = KDTree.FromData(seeds, seedLabels, distance);

            // create the cluster structure
            clusters = new MeanShiftClusterCollection(tree, maximum);

            // label each point
            return clusters.Nearest(points);
        }

        private double[] nearest(double[] point, ConcurrentStack<double[]> candidates)
        {
            foreach (double[] seed in candidates)
            {
                // compute the distance between points
                // if they are near, they are duplicates
                if (distance(point, seed) < Bandwidth)
                    return seed;
            }

            return null;
        }

        private void computeMeanShift(double[] x, double[] shift)
        {
            // Get points near the current point
            ICollection<KDTreeNodeDistance<int>> neighbors;

            if (maximum > 0)
                neighbors = tree.Nearest(x, Bandwidth * 3, maximum);
            else neighbors = tree.Nearest(x, Bandwidth * 3);

            double sum = 0;
            Array.Clear(shift, 0, shift.Length);

            // Compute weighted mean
            foreach (KDTreeNodeDistance<int> neighbor in neighbors)
            {
                double distance = neighbor.Distance;
                double[] p = neighbor.Node.Position;

                double u = distance / Bandwidth;

                // Compute g = -k'(||(x-xi)/h||²)
                double g = -kernel.Derivative(u * u);

                for (int i = 0; i < shift.Length; i++)
                    shift[i] += g * p[i];

                sum += g;
            }

            // Normalize
            if (sum != 0)
            {
                for (int i = 0; i < shift.Length; i++)
                    shift[i] /= sum;
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

        private int[] classifySeeds(double[][] seeds, double[][] modes)
        {
            // classify seeds using a minimum distance classifier

            int[] labels = new int[seeds.Length];
            for (int i = 0; i < seeds.Length; i++)
            {
                int imin = 0;
                double dmin = Double.PositiveInfinity;
                for (int j = 0; j < modes.Length; j++)
                {
                    double d = distance(modes[j], seeds[i]);

                    if (d < dmin)
                    {
                        imin = j;
                        dmin = d;
                    }
                }

                labels[i] = imin;
            }

            return labels;
        }

        private double[][] supress(double[][] seeds)
        {
            // According to Comaniciu et al (2002), local maxima points are 
            // defined according the Capture Theorem as unique stationary
            // points within some small open sphere.

            bool[] duplicate = new bool[seeds.Length];

            // for each unique point
            for (int i = 0; i < seeds.Length; i++)
            {
                if (duplicate[i]) continue;

                // against all other unique points
                for (int j = i + 1; j < seeds.Length; j++)
                {
                    if (duplicate[j]) continue;

                    // compute the distance between kernels
                    double d = distance(seeds[i], seeds[j]);

                    // if they are near, they are duplicates
                    if (d < Bandwidth) duplicate[j] = true;
                }
            }

            // Create a list containing only unique points
            List<double[]> maximum = new List<double[]>();
            for (int i = 0; i < duplicate.Length; i++)
                if (!duplicate[i])
                    maximum.Add(seeds[i]);

            return maximum.ToArray();
        }

        IClusterCollection<double[]> IClusteringAlgorithm<double[]>.Clusters
        {
            get { return clusters; }
        }
    }
}
