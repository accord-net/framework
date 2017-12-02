// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
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

namespace Accord.MachineLearning.Performance
{
    using System;
    using System.Collections.Generic;
    using Accord.Math;
    using Accord.Math.Optimization.Losses;
    using Accord.Statistics;
    using Accord.Compat;
    using System.Threading.Tasks;

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
    /// <typeparam name="TModel">The type of the machine learning model.</typeparam>
    /// <typeparam name="TInput">The type of the input data.</typeparam>
    /// <typeparam name="TOutput">The type of the output data or labels.</typeparam>
    /// <typeparam name="TLearner">The type of the learning algorithm used to learn <typeparamref name="TModel"/>.</typeparam>
    /// 
    /// <example>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\CrossValidationTest.cs" region="doc_learn" />
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\CrossValidationTest.cs" region="doc_learn_hmm" />
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\DecisionTrees\DecisionTreeTest.cs" region="doc_cross_validation" />
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\Bayes\NaiveBayesTest.cs" region="doc_cross_validation" />
    /// </example>
    /// 
    public class CrossValidation<TModel, TLearner, TInput, TOutput> :
        BaseSplitSetValidation<CrossValidationResult<TModel, TInput, TOutput>, TModel, TLearner, TInput, TOutput>
    where TModel : class, ITransform<TInput, TOutput>
    where TLearner : class, ISupervisedLearning<TModel, TInput, TOutput>
    {
        private int k = 10;
        private int[][] folds;
        private int[] indices;


        /// <summary>
        ///   Gets the array of data set indexes contained in each fold.
        /// </summary>
        /// 
        public int[][] Folds
        {
            get { return folds; }
        }

        /// <summary>
        ///  Gets the array of fold indices for each point in the data set.
        /// </summary>
        /// 
        public int[] Indices
        {
            get { return indices; }
        }

        /// <summary>
        ///   Gets the number of folds in the k-fold cross validation.
        /// </summary>
        /// 
        public int K
        {
            get { return k; }
            set
            {
                if (k == 0)
                    throw new ArgumentOutOfRangeException("The number of folds must be higher than zero.");
                k = value;
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CrossValidation" /> class.
        /// </summary>
        /// 
        public CrossValidation()
        {

        }

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// <returns>A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// Please set the Learner property before calling the Learn(x, y) method.
        /// or
        /// Please set the Learner property before calling the Learn(x, y) method.
        /// or
        /// The number of folds can not exceed the total number of samples in the data set.
        /// </exception>
        public override CrossValidationResult<TModel, TInput, TOutput> Learn(TInput[] x, TOutput[] y, double[] weights = null)
        {
            if (Learner == null)
                throw new InvalidOperationException("Please set the Learner property before calling the Learn(x, y) method.");

            if (Loss == null)
                throw new InvalidOperationException("Please set the Learner property before calling the Learn(x, y) method.");

            if (K > x.Length)
                throw new InvalidOperationException("The number of folds can not exceed the total number of samples in the data set.");

            this.indices = CreateValidationSplits(x, y, k);

            // Create indices for each fold
            this.folds = new int[k][];
            for (int i = 0; i < folds.Length; i++)
            {
                this.folds[i] = indices.Find(j => j == i);

                // Assert all folds have enough samples
                if (this.folds[i].Length == 0)
                    throw new InvalidOperationException("Some folds do not have any samples. Please decrease the number of data folds.");
            }




            var foldResults = new SplitResult<TModel, TInput, TOutput>[k];

            if (ParallelOptions.MaxDegreeOfParallelism == 1)
            {
                for (int i = 0; i < K; i++)
                    foldResults[i] = LearnSubset(GetFold(i, x, y, weights), i);
            }
            else
            {
                Parallel.For(0, K, ParallelOptions, i =>
                {
                    foldResults[i] = LearnSubset(GetFold(i, x, y, weights), i);
                });
            }

            return new CrossValidationResult<TModel, TInput, TOutput>(foldResults);
        }

        /// <summary>
        ///   Creates a list of the sample indices that should serve as the validation set.
        /// </summary>
        /// 
        /// <param name="x">The input data from where subsamples should be drawn.</param>
        /// <param name="y">The output data from where subsamples should be drawn.</param>
        /// <param name="numberOfFolds">The number of folds to be created.</param>
        /// 
        /// <returns>The indices of the samples in the original set that should compose the validation set.</returns>
        /// 
        protected virtual int[] CreateValidationSplits(TInput[] x, TOutput[] y, int numberOfFolds)
        {
            return Classes.Random(samples: x.Length, classes: numberOfFolds);
        }

        /// <summary>
        ///   Gets a subset of the training and testing sets.
        /// </summary>
        /// 
        /// <param name="validationFoldIndex">Index of the subsample.</param>
        /// <param name="x">The input data x.</param>
        /// <param name="y">The output data y.</param>
        /// <param name="weights">The weights of each sample.</param>
        /// 
        /// <returns>A <see cref="TrainValDataSplit{TInput, TOutput}"/> that defines a 
        ///   data split of a subsample of the dataset.</returns>
        /// 
        public TrainValDataSplit<TInput, TOutput> GetFold(int validationFoldIndex, TInput[] x, TOutput[] y, double[] weights)
        {
            if (validationFoldIndex < 0 || validationFoldIndex >= folds.Length)
                throw new ArgumentOutOfRangeException("validationFoldIndex");

            // The training set is given by joining all sets
            // other than the current validation set.
            List<int> list = new List<int>();
            for (int j = 0; j < folds.Length; j++)
                if (validationFoldIndex != j)
                    list.AddRange(folds[j]);

            // Select training set
            int[] trainingSet = list.ToArray();

            // Select validation set
            int[] validationSet = folds[validationFoldIndex];

            return new TrainValDataSplit<TInput, TOutput>(validationFoldIndex, x, y, weights, trainingSet, validationSet);
        }

    }
}
