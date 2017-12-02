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

namespace Accord.MachineLearning
{
    using Accord.MachineLearning.Performance;
    using Accord.Statistics;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Accord.Compat;

    /// <summary>
    ///   k-Fold cross-validation. Please only use the static methods contained in this class,
    ///   the rest are marked as obsolete.
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
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\CrossValidationTest.cs" region="doc_learn" />
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\CrossValidationTest.cs" region="doc_learn_hmm" />
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\DecisionTrees\DecisionTreeTest.cs" region="doc_cross_validation" />
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\Bayes\NaiveBayesTest.cs" region="doc_cross_validation" />
    /// </example>
    /// 
    [Serializable]
#pragma warning disable 0618
    public class CrossValidation : CrossValidation<object>
#pragma warning restore 0618
    {
        /// <summary>
        ///   Obsolete. Please use <see cref="CrossValidation{TModel, TInput}"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use CrossValidation<TModel, TInput> instead.")]
        public CrossValidation(int size)
            : base(size) { }

        /// <summary>
        ///   Obsolete. Please use <see cref="CrossValidation{TModel, TInput}"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use CrossValidation<TModel, TInput> instead.")]
        public CrossValidation(int size, int folds)
            : base(size, folds) { }

        /// <summary>
        ///   Obsolete. Please use <see cref="CrossValidation{TModel, TInput}"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use CrossValidation<TModel, TInput> instead.")]
        public CrossValidation(int[] labels, int classes, int folds)
            : base(labels, classes, folds) { }

        /// <summary>
        ///   Obsolete. Please use <see cref="CrossValidation{TModel, TInput}"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use CrossValidation<TModel, TInput> instead.")]
        public CrossValidation(int[] indices, int folds)
            : base(indices, folds) { }

        /// <summary>
        ///   Obsolete. Please use <see cref="Classes.Random(int, int)"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use Classes.Random(size, folds) instead.")]
        public static int[] Splittings(int size, int folds)
        {
            return Classes.Random(size, folds);
        }

        /// <summary>
        ///   Obsolete. Please use Classes.Random(labels, classes, folds) instead.
        /// </summary>
        /// 
        [Obsolete("Please use Classes.Random(labels, classes, folds) instead.")]
        public static int[] Splittings(int[] labels, int classes, int folds)
        {
            return Classes.Random(labels, classes, folds);
        }

        /// <summary>
        ///   Creates a new <see cref="CrossValidation{TModel, TLearner, TInput, TOutput}"/> algorithm.
        /// </summary>
        /// 
        /// <typeparam name="TModel">The type of the machine learning model whose parameters should be searched.</typeparam>
        /// <typeparam name="TLearner">The type of the learning algorithm used to learn <typeparamref name="TModel"/>.</typeparam>
        /// <typeparam name="TInput">The type of the input data. Default is double[].</typeparam>
        /// <typeparam name="TOutput">The type of the output data. Default is int.</typeparam>
        /// 
        /// <param name="k">The number of folds in the k-fold cross-validation. Default is 10.</param>
        /// <param name="learner">A function that can create a <typeparamref name="TModel"/> given training parameters.</param>
        /// <param name="loss">A function that can measure how far model predictions are from the expected ground-truth.</param>
        /// <param name="fit">A function that specifies how to create a new model using the teacher learning algorirhm.</param>
        /// <param name="x">The input data to be used during training.</param>
        /// <param name="y">The output data to be used during training.</param>
        /// 
        /// <example>
        ///   <code source="Unit Tests\Accord.Tests.MachineLearning\CrossValidationTest.cs" region="doc_learn" />
        ///   <code source="Unit Tests\Accord.Tests.MachineLearning\CrossValidationTest.cs" region="doc_learn_hmm" />
        ///   <code source="Unit Tests\Accord.Tests.MachineLearning\DecisionTrees\DecisionTreeTest.cs" region="doc_cross_validation" />
        /// </example>
        /// 
        /// <returns>A grid-search algorithm that has been configured with the given parameters.</returns>
        /// 
        public static CrossValidation<TModel, TLearner, TInput, TOutput> Create<TModel, TLearner, TInput, TOutput>(int k,
            CreateLearnerFromSubset<TLearner, TInput, TOutput> learner,
            LearnNewModel<TLearner, TInput, TOutput, TModel> fit,
            ComputeLoss<TOutput, SetResult<TModel>> loss,
            TInput[] x, TOutput[] y)
            where TModel : class, ITransform<TInput, TOutput>
            where TLearner : class, ISupervisedLearning<TModel, TInput, TOutput>
        {
            return new CrossValidation<TModel, TLearner, TInput, TOutput>()
            {
                K = k,
                Learner = learner,
                Fit = fit,
                Loss = loss,
            };
        }

        /*
        public class Lambda<TModel, TInput, TOutput> : TransformBase<TInput, TOutput>,
           ISupervisedLearning<Lambda<TModel, TInput, TOutput>, TInput, TOutput>
           where TModel : class
        {

            public TModel Model { get; set; }

            public CrossValidationValues<TModel> Values { get; set; }

            public Func<TInput[], TOutput[], double[], TModel> LearnFunction { get; set; }

            public Func<TModel, TInput, TOutput> TransformFunction { get; set; }

            public CancellationToken Token { get; set; }

            public Lambda<TModel, TInput, TOutput> Learn(TInput[] x, TOutput[] y, double[] weights = null)
            {
                return new Lambda<TModel, TInput, TOutput>()
                {
                    Model = LearnFunction(x, y, weights),
                    TransformFunction = TransformFunction
                };
            }

            public override TOutput Transform(TInput input)
            {
                return TransformFunction(Model, input);
            }
        }

        public static CrossValidation<Lambda<TModel, TInput, TOutput>, TInput, TOutput> Create<TModel, TInput, TOutput>(
            Func<TrainValSplit<DataSubset<TInput, TOutput>>, TModel> learn,
            Func<TModel, TInput, TOutput> transform)
            where TModel : class
        {
            return new CrossValidation<Lambda<TModel, TInput, TOutput>, TInput, TOutput>()
            {
                Learner = (p) =>
                {
                    return new Lambda<TModel, TInput, TOutput>()
                    {
                        Model = learn(p),
                        TransformFunction = transform
                    };
                }
            };
        }

        public static CrossValidation<Lambda<TModel, TInput, TOutput>, TInput, TOutput> Create<TModel, TInput, TOutput>(
            CrossValidationFittingFunction<TModel> fitting)
            where TModel : class
        {
            return new CrossValidation<Lambda<TModel, TInput, TOutput>, TInput, TOutput>()
            {
                Learner = (p) =>
                {
                    CrossValidationValues<TModel> values = fitting(p.Training.Index, p.Training.Indices, p.Validation.Indices);

                    return new Lambda<TModel, TInput, TOutput>()
                    {
                        Model = values.Model,
                        Values = values,
                        TransformFunction = (model, input) => default(TOutput)
                    };
                },

                Loss = (expected, actual, p) =>
                {
                    if (p.Name == "Training")
                    {
                        p.Variance = p.Model.Values.TrainingVariance;
                        return p.Value = p.Model.Values.TrainingValue;
                    }
                    else
                    {
                        p.Variance = p.Model.Values.ValidationVariance;
                        return p.Value = p.Model.Values.ValidationValue;
                    }
                }
            };
        }

        public static CrossValidation<TModel, TInput, TOutput> Create<TModel, TInput, TOutput>(SplitSetValidationLearn<TModel, TInput, TOutput> learn)
            where TModel : class, ITransform<TInput, TOutput>
        {
            return new CrossValidation<TModel, TInput, TOutput>()
            {
                Learner = learn
            };
        }

        public static CrossValidation<TModel, TInput> Create<TModel, TInput>(SplitSetValidationLearn<TModel, TInput, int> learn)
            where TModel : class, IClassifier<TInput, int>
        {
            return new CrossValidation<TModel, TInput>()
            {
                Learner = learn
            };
        }
        */
    }
}
