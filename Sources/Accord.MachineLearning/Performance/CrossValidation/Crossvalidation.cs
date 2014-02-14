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

    /// <summary>
    ///   k-Fold cross-validation.
    /// </summary>
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
    ///   // to assess the performance of Support Vector Machines.
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
    ///   var crossvalidation = new CrossValidation(size: data.Length, folds: 3);
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
    ///       return new CrossValidationValues(svm, trainingError, validationError);
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
    /// <seealso cref="Bootstrap"/>
    /// <seealso cref="CrossValidation{T}"/>
    /// <seealso cref="SplitSetValidation{T}"/>
    /// 
    [Serializable]
    public class CrossValidation : CrossValidation<object>
    {
        /// <summary>
        ///   Creates a new k-fold cross-validation algorithm.
        /// </summary>
        /// 
        /// <param name="size">The total number of available samples.</param>
        /// 
        public CrossValidation(int size) : base(size) { }

        /// <summary>
        ///   Creates a new k-fold cross-validation algorithm.
        /// </summary>
        /// 
        /// <param name="size">The complete dataset for training and testing.</param>
        /// <param name="folds">The number of folds, usually denoted as <c>k</c> (default is 10).</param>
        /// 
        public CrossValidation(int size, int folds) : base(size, folds) { }

        /// <summary>
        ///   Creates a new k-fold cross-validation algorithm.
        /// </summary>
        /// 
        /// <param name="indices">An already created set of fold indices for each sample in a dataset.</param>
        /// <param name="folds">The total number of folds referenced in the <paramref name="indices"/> parameter.</param>
        /// 
        public CrossValidation(int[] indices, int folds) : base(indices, folds) { }

        /// <summary>
        ///   Create cross-validation folds by generating
        ///   a vector of random fold indices.
        /// </summary>
        /// 
        /// <param name="size">The number of points in the data set.</param>
        /// <param name="folds">The number of folds in the cross-validation.</param>
        /// 
        /// <returns>A vector of indices defining the a fold for each point in the data set.</returns>
        /// 
        public static int[] Splittings(int size, int folds)
        {
            return Accord.Statistics.Tools.RandomGroups(size, folds);
        }

    }
}
