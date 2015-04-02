// Accord Statistics Library
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
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Collections.Concurrent;

    using Accord.Math;
    using Accord.Math.Comparers;
    using Accord.MachineLearning.Structures;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Multivariate;
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
        private int maximum;
        private int dimension;
        private bool cut = true;
        private double bandwidth;
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
        ///   Gets or sets whether the algorithm can use parallel
        ///   processing to speedup computations. Enabling parallel
        ///   processing can, however, result in different results 
        ///   at each run.
        /// </summary>
        /// 
        public bool UseParallelProcessing { get; set; }

        /// <summary>
        ///   Gets the dimension of the samples being 
        ///   modeled by this clustering algorithm.
        /// </summary>
        /// 
        public int Dimension
        {
            get { return dimension; }
        }

        IClusterCollection<double[]> IClusteringAlgorithm<double[]>.Clusters
        {
            get { return clusters; }
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
            this.kernel = kernel;
            this.Bandwidth = bandwidth;
            this.dimension = dimension;
            this.UseParallelProcessing = true;
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
            return Compute(points, null, threshold, 100);
        }

        /// <summary>
        ///   Divides the input data into clusters. 
        /// </summary>     
        /// 
        /// <param name="points">The data where to compute the algorithm.</param>
        /// <param name="weights">The weight of each data point, set to null for equal weight for each point</param>
        /// <param name="threshold">The relative convergence threshold
        /// for the algorithm. Default is 1e-3.</param>
        /// <param name="maxIterations">The maximum number of iterations. Default is 100.</param>
        /// 
        public int[] Compute(double[][] points, double[] weights, double threshold, int maxIterations = 100)
        {
            if (points == null)
                throw new ArgumentException("points", "Points must not be null");
            if (weights != null && weights.Length != points.Length)
                throw new ArgumentException("weights", "Weights must have the same length as points");

            if (weights == null)
                weights = Enumerable.Repeat(1.0, points.GetLength(0)).ToArray();

            // normalize weights
            weights = weights.Divide(weights.Sum() + Double.Epsilon, false);

            // first, select initial points
            double[][] pointsTmp = new double[points.Length][];

            for (int i = 0; i < pointsTmp.Length; i++)
            {
                pointsTmp[i] = new double[Dimension];
                Array.Copy(points[i], pointsTmp[i], Dimension);
            }

            var modeCandidates = new ConcurrentDictionary<double[], List<int>>();

            // construct map of the data
            tree = KDTree.FromData<int>(points, distance);

            // now, for each initial point 
            if (UseParallelProcessing)
            {
                Parallel.For(0, pointsTmp.Length, (index) =>
                    iterate(threshold, maxIterations, pointsTmp, weights, modeCandidates, index));
            }
            else
            {
                for (int index = 0; index < pointsTmp.Length; index++)
                    iterate(threshold, maxIterations, pointsTmp, weights, modeCandidates, index);
            }

            // suppress non-maximum points
            // pool points that are too close together to a single mode
            Dictionary<double[], List<int>> finalModes = merge(modeCandidates, bandwidth / 4);

            // convert modes to labels names and label each point
            int label = 0;

            List<double[]> modes = new List<double[]>();

            int[] pointsLabels = new int[points.GetLength(0)];

            foreach (var mode in finalModes.Keys)
            {
                foreach (var index in finalModes[mode])
                    pointsLabels[index] = label;

                modes.Add(mode);

                label++;
            }

            // create a decision map using seeds
            tree = KDTree.FromData(pointsTmp, pointsLabels, distance);

            // create the cluster structure
            clusters = new MeanShiftClusterCollection(tree, modes.ToArray());

            // label each point
            return pointsLabels;
        }

        private void iterate(
            double threshold,
            int maxIterations,
            double[][] points,
            double[] weights,
            ConcurrentDictionary<double[], List<int>> maxcandidates,
            int index)
        {
            bool finished = false;
            bool pointAdded = false;
            double[] point = points[index];
            double[] mean = new double[point.Length];
            double[] delta = new double[point.Length];

            // we will keep moving it in the
            // direction of the density modes
            int iterations = 0;

            // until convergence or max iterations reached
            while (!finished)
            {
                // compute the shifted mean 
                computeMeanShift(point, weights, mean);

                // extract the mean shift vector
                for (int j = 0; j < mean.Length; j++)
                    delta[j] = point[j] - mean[j];

                // update the point towards a mode
                Array.Copy(mean, point, mean.Length);

                // Check if we are already near any maximum point
                if (cut)
                {
                    lock (maxcandidates)
                    {
                        double[] tmpMode = nearest(point, maxcandidates, bandwidth / 4);

                        if (tmpMode != null)
                        {
                            maxcandidates[tmpMode].Add(index);
                            finished = true;
                            pointAdded = true;
                        }
                    }
                }

                // check for convergence: magnitude of the mean shift
                // vector converges to zero (Comaniciu 2002, page 606)
                if (Norm.SquareEuclidean(delta) < threshold * bandwidth)
                    finished = true;

                if (iterations >= maxIterations)
                    finished = true;

                iterations++;
            }

            // group together points that too close together
            if (pointAdded == false)
            {
                lock (maxcandidates)
                {
                    if (maxcandidates.ContainsKey(point))
                        maxcandidates[point].Add(index);
                    else
                        maxcandidates.TryAdd(point, new List<int>() { index });
                }
            }
        }

        private double[] nearest(
            double[] point,
            ConcurrentDictionary<double[], List<int>> candidates,
            Double minDistance)
        {
            // compute the distance between points
            // if they are near, they are duplicates
            foreach (double[] candidate in candidates.Keys)
                if (distance(point, candidate) < minDistance)
                    return candidate;

            return null;
        }

        private void computeMeanShift(double[] originalPosition, double[] pointWeights, double[] shiftedPosition)
        {
            // Get points near the current point
            ICollection<KDTreeNodeDistance<int>> neighbors;

            if (maximum == 0)
                neighbors = tree.Nearest(originalPosition, radius: bandwidth * 4);
            else
                neighbors = tree.Nearest(originalPosition, radius: bandwidth * 4, maximum: maximum);

            Array.Clear(shiftedPosition, 0, shiftedPosition.Length);

            // special case, no neighbors found
            // return same point
            if (neighbors.Count == 0)
            {
                Array.Copy(originalPosition, shiftedPosition, shiftedPosition.Length);
            }
            else
            {
                double sum = 0.0;

                // Compute weighted mean
                foreach (KDTreeNodeDistance<int> neighbor in neighbors)
                {
                    double distance = neighbor.Distance;
                    double weight = pointWeights[neighbor.Node.Value];
                    double[] neighborPosition = neighbor.Node.Position;

                    double u = distance / Bandwidth;

                    // Compute g = -k'(||(x-xi)/h||²) * weight
                    double g = -kernel.Derivative(u) * weight;

                    for (int i = 0; i < shiftedPosition.Length; i++)
                        shiftedPosition[i] += g * neighborPosition[i];

                    sum += Math.Abs(g);
                }

                // Normalize weighted average of shifted position
                if (sum > 0)
                    shiftedPosition.Divide(sum, true);
            }
        }

        /// <summary>
        /// Weighted average of modes
        /// </summary>
        /// <param name="modes"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        private double[] meanMode(double[][] modes, double[] weights)
        {
            double sumWeights = weights.Sum() + Double.Epsilon;
            double[] meanModeWeighted = new double[modes[0].Length];

            for (int i = 0; i < modes.Length; i++)
            {
                var tmpVector = modes[i].Multiply(weights[i] / sumWeights);
                meanModeWeighted = meanModeWeighted.Add(tmpVector);
            }

            return meanModeWeighted;
        }

        /// <summary>
        /// Merge candidates that are too close
        /// </summary>
        /// <param name="modeCandidates"></param>
        /// <param name="radius">Radius to merge in SquareEuclidean distance</param>
        /// <returns></returns>
        private Dictionary<double[], List<int>> merge(
            ConcurrentDictionary<double[], List<int>> modeCandidates,
            double radius)
        {
            return merge(
                modeCandidates.ToDictionary(kvp => kvp.Key,kvp => kvp.Value), 
                radius);
        }

        /// <summary>
        /// Merge candidates that are too close
        /// </summary>
        /// <param name="modeCandidates"></param>
        /// <param name="radius">Radius to merge in SquareEuclidean distance</param>
        /// <returns></returns>
        private Dictionary<double[], List<int>> merge(
            Dictionary<double[], List<int>> modeCandidates, 
            double radius)
        {
            List<double[]> tmpModes = new List<double[]>();

            // indicates a node that has been used in a merge
            Dictionary<double[], bool> usedFlags = new Dictionary<double[], bool>();
            Dictionary<double[], List<int>> finalModes = new Dictionary<double[], List<int>>();

            foreach (var modeCandidate in modeCandidates.Keys)
            {
                tmpModes.Add(modeCandidate);
                usedFlags.Add(modeCandidate, false);
            }

            KDTree<int> modeTree = KDTree.FromData<int>(tmpModes.ToArray(), distance);

            // create an ordered list of the mode candidates
            // based on their number of points
            List<Double[]> modeCandidatesOrdered = modeCandidates
                .OrderByDescending(x => x.Value.Count)
                .Select(x => x.Key)
                .ToList();

            // start merging mode candidates, 
            // start from the mode candidates
            // with the highest amount of points
            foreach (var modeCandidate in modeCandidatesOrdered)
            {
                // select all neighbors 
                // that were not used in a already
                var neighbors = modeTree
                    .Nearest(modeCandidate, radius: radius)
                    .FindAll(x => !usedFlags[x.Node.Position]);

                if (neighbors.Count == 0)
                    continue;

                double[][] neighborModeCandidates = neighbors
                    .Select(x => x.Node.Position)
                    .ToArray();

                // the weight of each neighbor is equal to the number of 
                // of points that are stationary in the mean shift sense 
                // to its position
                double[] neighborModeWeights = Array.ConvertAll(
                    neighbors
                    .Select(x => modeCandidates[x.Node.Position].Count)
                    .ToArray(), Convert.ToDouble);

                // weighted average of the neighboring modes 
                double[] tmpMode = meanMode(neighborModeCandidates, neighborModeWeights);

                finalModes.Add(tmpMode, new List<int>());

                // add all the points to the final mode 
                // raise the flag for each of the neighbors
                foreach (var neighbor in neighbors)
                {
                    // add all the neighbors points to the new mode
                    finalModes[tmpMode].AddRange(modeCandidates[neighbor.Node.Position]);

                    // raise flag indicating the candidate neighbor mode cannot be reused
                    usedFlags[neighbor.Node.Position] = true;
                }
            }

            return finalModes;
        }
    }
}
