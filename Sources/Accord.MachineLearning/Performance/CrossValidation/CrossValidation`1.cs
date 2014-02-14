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
    using System.Threading.Tasks;
    using Accord.Math;

    /// <summary>
    ///   Fitting function delegate.
    /// </summary>
    /// 
    /// <param name="k">
    ///   The fold index.</param>
    /// <param name="trainingSamples">
    ///   The sample indexes to be used as training samples in
    ///   the model fitting procedure. </param>
    /// <param name="validationSamples">
    ///   The sample indexes to be used as validation samples in
    ///   the model fitting procedure. </param>
    ///   
    /// <remarks>
    ///   The fitting function is called during the Cross-validation
    ///   procedure to fit a model with the given set of samples for
    ///   training and validation.
    /// </remarks>
    /// 
    public delegate CrossValidationValues<TModel>
        CrossValidationFittingFunction<TModel>(int k, int[] trainingSamples, int[] validationSamples)
        where TModel : class;

    /// <summary>
    ///   k-Fold cross-validation.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type of the model being analyzed.</typeparam>
    /// 
    /// <remarks>
    /// <para>
    ///   Cross-validation is a technique for estimating the performance of a predictive
    ///   model. It can be used to measure how the results of a statistical analysis will
    ///   generalize to an independent data set. It is mainly used in settings where the
    ///   goal is prediction, and one wants to estimate how accurately a predictive model
    ///   will perform in practice.</para>
    /// <para>
    ///   One round of cross-validation involves partitioning a sample of data into
    ///   complementary subsets, performing the analysis on one subset (called the
    ///   training set), and validating the analysis on the other subset (called the
    ///   validation set or testing set). To reduce variability, multiple rounds of 
    ///   cross-validation are performed using different partitions, and the validation 
    ///   results are averaged over the rounds.</para> 
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Cross-validation_(statistics)">
    ///       Wikipedia, The Free Encyclopedia. Cross-validation (statistics). Available on:
    ///       http://en.wikipedia.org/wiki/Cross-validation_(statistics) </a></description></item>
    ///   </list></para> 
    /// </remarks>
    /// 
    /// <example>
    ///   <code>
    ///   // This is a sample code on how to use Cross-Validation
    ///   // to access the performance of Support Vector Machines.
    ///
    ///   // Consider the example binary data. We will be trying
    ///   // to learn a XOR problem and see how well does SVMs
    ///   // perform on this data.
    ///
    ///   double[][] data =
    ///   {
    ///       new double[] { -1, -1 }, new double[] {  1, -1 },
    ///       new double[] { -1,  1 }, new double[] {  1,  1 },
    ///       new double[] { -1, -1 }, new double[] {  1, -1 },
    ///       new double[] { -1,  1 }, new double[] {  1,  1 },
    ///       new double[] { -1, -1 }, new double[] {  1, -1 },
    ///       new double[] { -1,  1 }, new double[] {  1,  1 },
    ///       new double[] { -1, -1 }, new double[] {  1, -1 },
    ///       new double[] { -1,  1 }, new double[] {  1,  1 },
    ///   };
    ///
    ///   int[] xor = // result of xor for the sample input data
    ///   {
    ///       -1,       1,
    ///        1,      -1,
    ///       -1,       1,
    ///        1,      -1,
    ///       -1,       1,
    ///        1,      -1,
    ///       -1,       1,
    ///        1,      -1,
    ///   };
    ///
    ///
    ///   // Create a new Cross-validation algorithm passing the data set size and the number of folds
    ///   var crossvalidation = new CrossValidation&lt;KernelSupportVectorMachine>(size: data.Length, folds: 3);
    ///
    ///   // Define a fitting function using Support Vector Machines. The objective of this
    ///   // function is to learn a SVM in the subset of the data indicated by cross-validation.
    ///
    ///   crossvalidation.Fitting = delegate(int k, int[] indicesTrain, int[] indicesValidation)
    ///   {
    ///       // The fitting function is passing the indices of the original set which
    ///       // should be considered training data and the indices of the original set
    ///       // which should be considered validation data.
    ///
    ///       // Lets now grab the training data:
    ///       var trainingInputs = data.Submatrix(indicesTrain);
    ///       var trainingOutputs = xor.Submatrix(indicesTrain);
    ///
    ///       // And now the validation data:
    ///       var validationInputs = data.Submatrix(indicesValidation);
    ///       var validationOutputs = xor.Submatrix(indicesValidation);
    ///
    ///
    ///       // Create a Kernel Support Vector Machine to operate on the set
    ///       var svm = new KernelSupportVectorMachine(new Polynomial(2), 2);
    ///
    ///       // Create a training algorithm and learn the training data
    ///       var smo = new SequentialMinimalOptimization(svm, trainingInputs, trainingOutputs);
    ///
    ///       double trainingError = smo.Run();
    ///
    ///       // Now we can compute the validation error on the validation data:
    ///       double validationError = smo.ComputeError(validationInputs, validationOutputs);
    ///
    ///       // Return a new information structure containing the model and the errors achieved.
    ///       return new CrossValidationValues&lt;KernelSupportVectorMachine>(svm, trainingError, validationError);
    ///   };
    ///
    ///
    ///   // Compute the cross-validation
    ///   var result = crossvalidation.Compute();
    ///
    ///   // Finally, access the measured performance.
    ///   double trainingErrors = result.Training.Mean;
    ///   double validationErrors = result.Validation.Mean;
    ///   </code>
    /// </example>
    /// 
    [Serializable]
    public class CrossValidation<TModel> where TModel : class
    {

        private int[] indices;
        private int[][] folds;

        private int samples;
        private bool parallel = true;

        [NonSerialized]
        CrossValidationFittingFunction<TModel> fitting;

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
        public CrossValidationFittingFunction<TModel> Fitting
        {
            get { return fitting; }
            set { fitting = value; }
        }

        /// <summary>
        ///   Gets the array of data set indexes contained in each fold.
        /// </summary>
        /// 
        public int[][] Folds { get { return folds; } }

        /// <summary>
        ///  Gets the array of fold indices for each point in the data set.
        /// </summary>
        public int[] Indices { get { return indices; } }

        /// <summary>
        ///   Gets the number of folds in the k-fold cross validation.
        /// </summary>
        /// 
        public int K { get { return folds.Length; } }

        /// <summary>
        ///   Gets the total number of data samples in the data set.
        /// </summary>
        /// 
        public int Samples { get { return samples; } }

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
        ///   Creates a new k-fold cross-validation algorithm.
        /// </summary>
        /// 
        /// <param name="size">The total number of available samples.</param>
        /// 
        public CrossValidation(int size)
            : this(size, 10) { }

        /// <summary>
        ///   Creates a new k-fold cross-validation algorithm.
        /// </summary>
        /// 
        /// <param name="size">The total number of available samples.</param>
        /// <param name="folds">The number of folds, usually denoted as <c>k</c> (default is 10).</param>
        /// 
        public CrossValidation(int size, int folds)
        {
            if (folds > size)
                throw new ArgumentException("The number of folds can not exceed the total number of samples in the data set", "folds");

            this.samples = size;
            this.folds = new int[folds][];

            this.indices = CrossValidation.Splittings(size, folds);

            // Create foldings
            for (int i = 0; i < folds; i++)
                this.folds[i] = indices.Find(x => x == i);
        }

        /// <summary>
        ///   Creates a new k-fold cross-validation algorithm.
        /// </summary>
        /// 
        /// <param name="indices">An already created set of fold indices for each sample in a dataset.</param>
        /// <param name="folds">The total number of folds referenced in the <paramref name="indices"/> parameter.</param>
        /// 
        public CrossValidation(int[] indices, int folds)
        {
            this.samples = indices.Length;
            this.folds = new int[folds][];
            this.indices = indices;

            // Create foldings
            for (int i = 0; i < folds; i++)
                this.folds[i] = indices.Find(x => x == i);
        }

        /// <summary>
        ///   Gets the indices for the training and validation 
        ///   sets for the specified validation fold index.
        /// </summary>
        /// 
        /// <param name="validationFoldIndex">The index of the validation fold.</param>
        /// <param name="trainingSet">The indices for the observations in the training set.</param>
        /// <param name="validationSet">The indices for the observations in the validation set.</param>
        /// 
        public void CreatePartitions(int validationFoldIndex, out int[] trainingSet, out int[] validationSet)
        {
            if (validationFoldIndex < 0 || validationFoldIndex >= folds.Length)
                throw new ArgumentOutOfRangeException("validationFoldIndex");

            // Create indices for the foldings

            // The training set is given by joining all sets
            // other than the current validation set.
            List<int[]> list = new List<int[]>();
            for (int j = 0; j < folds.Length; j++)
                if (validationFoldIndex != j) list.Add(folds[j]);

            // Select training set
            trainingSet = Matrix.Concatenate(list.ToArray());

            // Select validation set
            validationSet = folds[validationFoldIndex];
        }

        /// <summary>
        ///   Gets the number of instances in training and validation
        ///   sets for the specified validation fold index.
        /// </summary>
        /// 
        /// <param name="validationFoldIndex">The index of the validation fold.</param>
        /// <param name="trainingCount">The number of instances in the training set.</param>
        /// <param name="validationCount">The number of instances in the validation set.</param>
        /// 
        public void GetPartitionSize(int validationFoldIndex, out int trainingCount, out int validationCount)
        {
            if (validationFoldIndex < 0 || validationFoldIndex >= folds.Length)
                throw new ArgumentOutOfRangeException("validationFoldIndex");

            trainingCount = 0;
            validationCount = 0;

            for (int j = 0; j < folds.Length; j++)
                if (validationFoldIndex != j) trainingCount += folds[j].Length;

            validationCount = folds[validationFoldIndex].Length;
        }

        /// <summary>
        ///   Computes the cross validation algorithm.
        /// </summary>
        /// 
        public CrossValidationResult<TModel> Compute()
        {
            if (Fitting == null)
                throw new InvalidOperationException("Fitting function must have been previously defined.");

            var models = new CrossValidationValues<TModel>[folds.Length];

            if (RunInParallel)
            {
                Parallel.For(0, folds.Length, i =>
                {
                    int[] trainingSet, validationSet;

                    // Create training and validation sets
                    CreatePartitions(i, out trainingSet, out validationSet);

                    // Fit and evaluate the model
                    models[i] = fitting(i, trainingSet, validationSet);
                });
            }
            else
            {
                for (int i = 0; i < folds.Length; i++)
                {
                    int[] trainingSet, validationSet;

                    // Create training and validation sets
                    CreatePartitions(i, out trainingSet, out validationSet);

                    // Fit and evaluate the model
                    models[i] = fitting(i, trainingSet, validationSet);
                }
            }

            // Return cross-validation statistics
            return new CrossValidationResult<TModel>(this, models);
        }

    }
}
