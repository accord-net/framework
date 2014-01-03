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
    using System.Linq;
    using System.Threading.Tasks;
    using Accord.Math;

    /// <summary>
    ///   Fitting function delegate.
    /// </summary>
    /// 
    /// <param name="trainingSamples">
    ///   The sample indexes to be used as training samples in
    ///   the model fitting procedure. </param>
    /// <param name="validationSamples">
    ///   The sample indexes to be used as validation samples in
    ///   the model fitting procedure. </param>
    ///   
    /// <remarks>
    ///   The fitting function is called during the Bootstrap
    ///   procedure to fit a model with the given set of samples
    ///   for training and validation.
    /// </remarks>
    /// 
    public delegate BootstrapValues BootstrapFittingFunction(int[] trainingSamples, int[] validationSamples);

    /// <summary>
    ///   Bootstrap method for generalization
    ///   performance measurements.
    /// </summary>
    /// 
    /// <example>
    /// <code>
    /// // This is a sample code on how to use Bootstrap estimate
    /// // to assess the performance of Support Vector Machines.
    /// 
    /// // Consider the example binary data. We will be trying
    /// // to learn a XOR problem and see how well does SVMs
    /// // perform on this data.
    /// 
    /// double[][] data =
    /// {
    ///     new double[] { -1, -1 }, new double[] {  1, -1 },
    ///     new double[] { -1,  1 }, new double[] {  1,  1 },
    ///     new double[] { -1, -1 }, new double[] {  1, -1 },
    ///     new double[] { -1,  1 }, new double[] {  1,  1 },
    ///     new double[] { -1, -1 }, new double[] {  1, -1 },
    ///     new double[] { -1,  1 }, new double[] {  1,  1 },
    ///     new double[] { -1, -1 }, new double[] {  1, -1 },
    ///     new double[] { -1,  1 }, new double[] {  1,  1 },
    /// };
    /// 
    /// int[] xor = // result of xor for the sample input data
    /// {
    ///     -1,       1,
    ///      1,      -1,
    ///     -1,       1,
    ///      1,      -1,
    ///     -1,       1,
    ///      1,      -1,
    ///     -1,       1,
    ///      1,      -1,
    /// };
    /// 
    /// 
    /// // Create a new Bootstrap algorithm passing the set size and the number of resamplings
    /// var bootstrap = new Bootstrap(size: data.Length, subsamples: 50);
    /// 
    /// // Define a fitting function using Support Vector Machines. The objective of this
    /// // function is to learn a SVM in the subset of the data indicated by the bootstrap.
    /// 
    /// bootstrap.Fitting = delegate(int[] indicesTrain, int[] indicesValidation)
    /// {
    ///     // The fitting function is passing the indices of the original set which
    ///     // should be considered training data and the indices of the original set
    ///     // which should be considered validation data.
    /// 
    ///     // Lets now grab the training data:
    ///     var trainingInputs = data.Submatrix(indicesTrain);
    ///     var trainingOutputs = xor.Submatrix(indicesTrain);
    /// 
    ///     // And now the validation data:
    ///     var validationInputs = data.Submatrix(indicesValidation);
    ///     var validationOutputs = xor.Submatrix(indicesValidation);
    /// 
    /// 
    ///     // Create a Kernel Support Vector Machine to operate on the set
    ///     var svm = new KernelSupportVectorMachine(new Polynomial(2), 2);
    /// 
    ///     // Create a training algorithm and learn the training data
    ///     var smo = new SequentialMinimalOptimization(svm, trainingInputs, trainingOutputs);
    /// 
    ///     double trainingError = smo.Run();
    /// 
    ///     // Now we can compute the validation error on the validation data:
    ///     double validationError = smo.ComputeError(validationInputs, validationOutputs);
    /// 
    ///     // Return a new information structure containing the model and the errors achieved.
    ///     return new BootstrapValues(trainingError, validationError);
    /// };
    /// 
    /// 
    /// // Compute the bootstrap estimate
    /// var result = bootstrap.Compute();
    /// 
    /// // Finally, access the measured performance.
    /// double trainingErrors = result.Training.Mean;
    /// double validationErrors = result.Validation.Mean;
    /// 
    /// // And compute the 0.632 estimate
    /// double estimate = result.Estimate;
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="CrossValidation"/>
    /// <seealso cref="CrossValidation{T}"/>
    /// 
    /// <seealso cref="SplitSetValidation"/>
    /// <seealso cref="SplitSetValidation{T}"/>
    /// 
    [Serializable]
    public class Bootstrap
    {

        private int[][] subsampleIndices;

        private int samples;
        private bool parallel = true;

        [NonSerialized]
        BootstrapFittingFunction fitting;

        /// <summary>
        ///   Gets the number B of bootstrap samplings
        ///   to be drawn from the population dataset.
        /// </summary>
        /// 
        public int B { get { return subsampleIndices.Length; } }

        /// <summary>
        ///   Gets the total number of samples in the population dataset.
        /// </summary>
        /// 
        public int Samples { get { return samples; } }

        /// <summary>
        ///   Gets the bootstrap samples drawn from
        ///   the population dataset as indices.
        /// </summary>
        /// 
        public int[][] Subsamples { get { return subsampleIndices; } }


        /// <summary>
        ///   Gets or sets the model fitting function.
        /// </summary>
        /// <remarks>
        ///   The fitting function should accept an array of integers containing the
        ///   indexes for the training samples, an array of integers containing the
        ///   indexes for the validation samples and should return information about
        ///   the model fitted using those two subsets of the available data.
        /// </remarks>
        /// 
        public BootstrapFittingFunction Fitting
        {
            get { return fitting; }
            set { fitting = value; }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to use parallel
        ///   processing through the use of multiple threads or not.
        ///   Default is true.
        /// </summary>
        /// 
        /// <value><c>true</c> to use multiple threads; otherwise, <c>false</c>.</value>
        /// 
        public bool RunInParallel
        {
            get { return parallel; }
            set { parallel = value; }
        }

        /// <summary>
        ///   Creates a new Bootstrap estimation algorithm.
        /// </summary>
        /// 
        /// <param name="size">The size of the complete dataset.</param>
        /// <param name="subsamples">The number B of bootstrap resamplings to perform.</param>
        /// 
        public Bootstrap(int size, int subsamples)
            : this(size, subsamples, size) { }

        /// <summary>
        ///   Creates a new Bootstrap estimation algorithm.
        /// </summary>
        /// 
        /// <param name="size">The size of the complete dataset.</param>
        /// <param name="subsamples">The number B of bootstrap resamplings to perform.</param>
        /// <param name="subsampleSize">The number of samples in each subsample. Default
        ///   is to use the total number of samples in the population dataset.</param>.
        /// 
        public Bootstrap(int size, int subsamples, int subsampleSize)
        {
            this.samples = size;
            this.subsampleIndices = new int[subsamples][];

            this.subsampleIndices = Bootstrap.Samplings(size, subsamples, subsampleSize);
        }

        /// <summary>
        ///   Creates a new Bootstrap estimation algorithm.
        /// </summary>
        /// 
        /// <param name="size">The size of the complete dataset.</param>
        /// <param name="resamplings">The indices of the bootstrap samplings.</param>
        /// 
        public Bootstrap(int size, int[][] resamplings)
        {
            this.samples = size;
            this.subsampleIndices = resamplings;
        }

        /// <summary>
        ///   Gets the indices for the training and validation 
        ///   sets for the specified validation fold index.
        /// </summary>
        /// 
        /// <param name="index">The index of the validation fold.</param>
        /// <param name="trainingSet">The indices for the observations in the training set.</param>
        /// <param name="validationSet">The indices for the observations in the validation set.</param>
        /// 
        public void CreatePartitions(int index, out int[] trainingSet, out int[] validationSet)
        {
            if (index < 0 || index >= Subsamples.Length)
                throw new ArgumentOutOfRangeException("index");

            // Create indices for the foldings

            // The training set is already computed
            trainingSet = Subsamples[index];

            // The validation set is the complement of the training set
            validationSet = Matrix.Indices(0, Samples).Except(trainingSet).ToArray();
        }

        /// <summary>
        ///   Computes the cross validation algorithm.
        /// </summary>
        /// 
        public BootstrapResult Compute()
        {
            if (Fitting == null)
                throw new InvalidOperationException("Fitting function must have been previously defined.");

            var models = new BootstrapValues[Subsamples.Length];

            if (RunInParallel)
            {
                Parallel.For(0, Subsamples.Length, i =>
                {
                    int[] trainingSet, validationSet;

                    // Create training and validation sets
                    CreatePartitions(i, out trainingSet, out validationSet);

                    // Fit and evaluate the model
                    models[i] = fitting(trainingSet, validationSet);
                });
            }
            else
            {
                for (int i = 0; i < Subsamples.Length; i++)
                {
                    int[] trainingSet, validationSet;

                    // Create training and validation sets
                    CreatePartitions(i, out trainingSet, out validationSet);

                    // Fit and evaluate the model
                    models[i] = fitting(trainingSet, validationSet);
                }
            }

            // Return cross-validation statistics
            return new BootstrapResult(this, models);
        }


        /// <summary>
        ///   Gets the number of instances in training and validation
        ///   sets for the specified validation fold index.
        /// </summary>
        /// 
        /// <param name="index">The index of the bootstrap sample.</param>
        /// <param name="trainingCount">The number of instances in the training set.</param>
        /// <param name="validationCount">The number of instances in the validation set.</param>
        /// 
        public void GetPartitionSize(int index, out int trainingCount, out int validationCount)
        {
            if (index < 0 || index >= subsampleIndices.Length)
                throw new ArgumentOutOfRangeException("index");

            trainingCount = subsampleIndices[index].Length;

            validationCount = samples - trainingCount;
        }


        /// <summary>
        ///   Draws the bootstrap samples from the population.
        /// </summary>
        /// 
        /// <param name="size">The size of the samples to be drawn.</param>
        /// <param name="resamplings">The number of samples to drawn.</param>
        /// <param name="subsampleSize">The size of the samples to be drawn.</param>
        /// 
        /// <returns>The indices of the samples in the original set.</returns>
        /// 
        public static int[][] Samplings(int size, int resamplings, int subsampleSize)
        {
            // Create samples with replacement
            int[][] idx = new int[resamplings][];

            // For each fold to be created
            for (int i = 0; i < idx.Length; i++)
            {
                idx[i] = new int[subsampleSize];

                // Generate a random sample (with replacement)
                for (int j = 0; j < idx[i].Length; j++)
                    idx[i][j] = Accord.Math.Tools.Random.Next(0, size);
            }

            return idx;
        }
    }
}
