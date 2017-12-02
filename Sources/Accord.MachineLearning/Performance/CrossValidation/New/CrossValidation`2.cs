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
    using System.Collections;
    using System.Collections.Generic;
    using Accord.Math;
    using Accord.Math.Optimization.Losses;
    using Accord.Statistics.Analysis;
    using Accord.Statistics;

    /// <summary>
    ///   k-Fold cross-validation (with support for stratification and default loss function for classification).
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
    /// 
    /// <example>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\CrossValidationTest.cs" region="doc_learn" />
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\CrossValidationTest.cs" region="doc_learn_hmm" />
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\DecisionTrees\DecisionTreeTest.cs" region="doc_cross_validation" />
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\Bayes\NaiveBayesTest.cs" region="doc_cross_validation" />
    /// </example>
    /// 
    /// <seealso cref="Bootstrap{TModel, TInput, TOutput}"/>
    /// <seealso cref="SplitSetValidation{TModel, TInput, TOutput}"/>
    /// 
    public class CrossValidation<TModel, TInput> : CrossValidation<TModel, TInput, int>
        where TModel : class, IClassifier<TInput, int>
    {

        /// <summary>
        ///   Gets or sets a value indicating whether the prevalence of an output 
        ///   label should be balanced between training and testing sets. Default is false.
        /// </summary>
        /// 
        /// <value>
        /// 	<c>true</c> if this instance is stratified; otherwise, <c>false</c>.
        /// </value>
        /// 
        public bool Stratify { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrossValidation{TModel, TInput}"/> class.
        /// </summary>
        /// 
        public CrossValidation()
        {
            this.Loss = (expected, actual, r) =>
            {
                return new GeneralConfusionMatrix(r.Model.NumberOfClasses, expected, actual).Error;
            };
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
        protected override int[] CreateValidationSplits(TInput[] x, int[] y, int numberOfFolds)
        {
            if (Stratify)
                return Classes.Random(y, y.DistinctCount(), numberOfFolds);
            return base.CreateValidationSplits(x, y, numberOfFolds);
        }

    }
}
