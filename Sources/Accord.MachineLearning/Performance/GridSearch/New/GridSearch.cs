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

#if NET35
    #define DISABLE_DYNAMIC
#endif

namespace Accord.MachineLearning.Performance
{
    using Accord.Math;
    using System;

    /// <summary>
    ///   Grid search procedure for automatic parameter tuning.
    /// </summary>
    /// 
    /// <remarks>
    ///   Grid Search tries to find the best combination of parameters across a range of possible values that produces the best fit model. If there
    ///   are two parameters, each with 10 possible values, Grid Search will try an exhaustive evaluation of the model using every combination of points,
    ///   resulting in 100 model fits.
    /// </remarks>
    /// 
    /// <example>
    ///   <para>
    ///     The framework offers different ways to use grid search: one version is strongly-typed using generics
    ///     and the other might need some manual casting. The exapmle below shows how to perform grid-search in
    ///     a non-stringly typed way:</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\GridSearchTest.cs" region="doc_learn" />
    ///   
    ///   <para>
    ///     The main disadvantages of the method above is the need to keep string identifiers for each of the parameters
    ///     being searched. Furthermore, it is also necessary to keep track of their types in order to cast them accordingly
    ///     when using them in the specification of the <see cref="BaseGridSearch{TResult, TModel, TRange, TParam, TLearner, TInput, TOutput}.Learner"/>
    ///     property. </para>
    ///     
    ///   <para>    
    ///     The next example shows how to perform grid-search in a strongly typed way:</para>
    ///     <code source="Unit Tests\Accord.Tests.MachineLearning\GridSearchTest.cs" region="doc_learn_strongly_typed" />
    ///   
    ///   <para>
    ///     The code above uses anonymous types and generics to create a specialized <see cref="GridSearch{TModel, TInput, TOutput}"/>
    ///     class that keeps the anonymous type given as <see cref="BaseGridSearch{TResult, TModel, TRange, TParam, TLearner, TInput, TOutput}.ParameterRanges"/>.
    ///     Its main disadvantage is the (high) increase in type complexity, making the use of the <c>var</c> keyword almost
    ///     mandatory.</para>
    ///     
    ///   <para>
    ///     It is also possible to create grid-search objects using convenience methods from the static <see cref="GridSearch"/> class:</para>
    ///     <code source="Unit Tests\Accord.Tests.MachineLearning\GridSearchTest.cs" region="doc_create" />
    ///   
    ///   <para>
    ///     Finally, it is also possible to combine grid-search with <see cref="CrossValidation{TModel, TInput, TOutput}"/>, 
    ///     as shown in the examples below: </para>
    ///     <code source="Unit Tests\Accord.Tests.MachineLearning\GridSearchTest.cs" region="doc_learn_cv" />
    ///     <code source="Unit Tests\Accord.Tests.MachineLearning\GridSearchTest.cs" region="doc_learn_tree_cv" />
    /// </example>
    /// 
    public static class GridSearch
    {
        /// <summary>
        ///   Creates a range of parameter values that should be searched during <see cref="GridSearch"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the parameter values.</typeparam>
        /// 
        /// <param name="values">The values to be included in <see cref="GridSearch"/>.</param>
        /// 
        public static GridSearchRange<T> Values<T>(params T[] values)
        {
            return new GridSearchRange<T>() { Values = values };
        }

#if !DISABLE_DYNAMIC
        /// <summary>
        ///   Creates a range of parameter values that should be searched during <see cref="GridSearch"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the parameter values.</typeparam>
        /// 
        public static GridSearchRange<T> Range<T>(T toExclusive)
        {
            return new GridSearchRange<T>() { Values = Vector.Range(n: (dynamic)toExclusive) };
        }

        /// <summary>
        ///   Creates a range of parameter values that should be searched during <see cref="GridSearch"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the parameter values.</typeparam>
        /// 
        public static GridSearchRange<T> Range<T>(T fromInclusive, T toExclusive)
        {
            return new GridSearchRange<T>() { Values = Vector.Range(a: (dynamic)fromInclusive, b: (dynamic)toExclusive) };
        }

        /// <summary>
        ///   Creates a range of parameter values that should be searched during <see cref="GridSearch"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the parameter values.</typeparam>
        /// 
        public static GridSearchRange<T> Range<T>(T fromInclusive, T toExclusive, T stepSize)
        {
            return new GridSearchRange<T>() { Values = Vector.Range(a: (dynamic)fromInclusive, b: (dynamic)toExclusive, stepSize: (dynamic)stepSize) };
        }
#endif


        /// <summary>
        ///   Creates a new <see cref="GridSearch{TModel, TRange, TLearner, TInput, TOutput}"/> algorithm.
        /// </summary>
        /// 
        /// <typeparam name="TInput">The type of the input data. Default is double[].</typeparam>
        /// <typeparam name="TOutput">The type of the output data. Default is int.</typeparam>
        /// <typeparam name="TModel">The type of the machine learning model whose parameters should be searched.</typeparam>
        /// <typeparam name="TLearner">The type of the learning algorithm used to learn <typeparamref name="TModel"/>.</typeparam>
        /// 
        /// <param name="ranges">The range of parameters to consider during search.</param>
        /// <param name="learner">A function that can create a <typeparamref name="TModel"/> given training parameters.</param>
        /// <param name="loss">A function that can measure how far model predictions are from the expected ground-truth.</param>
        /// <param name="fit">A function that specifies how to create a new model using the teacher learning algorirhm.</param>
        /// <param name="x">The input data to be used during training.</param>
        /// <param name="y">The output data to be used during training.</param>
        /// 
        /// <example>
        ///   <code source="Unit Tests\Accord.Tests.MachineLearning\GridSearchTest.cs" region="doc_create" />
        /// </example>
        /// 
        /// <returns>A grid-search algorithm that has been configured with the given parameters.</returns>
        /// 
        public static GridSearch<TModel, TLearner, TInput, TOutput> Create<TInput, TOutput, TModel, TLearner>(
            GridSearchRange[] ranges,
            CreateLearnerFromParameter<TLearner, GridSearchParameterCollection> learner,
            ComputeLoss<TOutput, TModel> loss,
            LearnNewModel<TLearner, TInput, TOutput, TModel> fit,
            TInput[] x, TOutput[] y)
        where TModel : class, ITransform<TInput, TOutput>
        where TLearner : ISupervisedLearning<TModel, TInput, TOutput>
        {
            return GridSearch<TInput, TOutput>.Create(ranges, learner, loss, fit);
        }

        /// <summary>
        ///   Creates a new <see cref="GridSearch{TModel, TRange, TLearner, TInput, TOutput}"/> algorithm.
        /// </summary>
        /// 
        /// <typeparam name="TModel">The type of the machine learning model whose parameters should be searched.</typeparam>
        /// <typeparam name="TRange">The type that specifies how ranges of the parameter values are represented.</typeparam>
        /// <typeparam name="TLearner">The type of the learning algorithm used to learn <typeparamref name="TModel"/>.</typeparam>
        /// <typeparam name="TInput">The type of the input data. Default is double[].</typeparam>
        /// <typeparam name="TOutput">The type of the output data. Default is int.</typeparam>
        /// 
        /// <param name="ranges">The range of parameters to consider during search.</param>
        /// <param name="learner">A function that can create a <typeparamref name="TModel"/> given training parameters.</param>
        /// <param name="loss">A function that can measure how far model predictions are from the expected ground-truth.</param>
        /// <param name="fit">A function that specifies how to create a new model using the teacher learning algorirhm.</param>
        /// <param name="x">The input data to be used during training.</param>
        /// <param name="y">The output data to be used during training.</param>
        /// 
        /// <example>
        ///   <code source="Unit Tests\Accord.Tests.MachineLearning\GridSearchTest.cs" region="doc_learn_strongly_typed" />
        /// </example>
        /// 
        /// <returns>A grid-search algorithm that has been configured with the given parameters.</returns>
        /// 
        public static GridSearch<TModel, TRange, TLearner, TInput, TOutput> Create<TInput, TOutput, TRange, TModel, TLearner>(
            TRange ranges,
            CreateLearnerFromParameter<TLearner, TRange> learner,
            ComputeLoss<TOutput, TModel> loss,
            LearnNewModel<TLearner, TInput, TOutput, TModel> fit,
            TInput[] x, TOutput[] y)
        where TModel : class, ITransform<TInput, TOutput>
        where TLearner : ISupervisedLearning<TModel, TInput, TOutput>
        {
            return GridSearch<TInput, TOutput>.Create(ranges, learner, loss, fit);
        }

        /// <summary>
        ///   Creates a new <see cref="GridSearch{TModel, TRange, TLearner, TInput, TOutput}"/> combined with 
        ///   <see cref="CrossValidation{TModel, TInput, TOutput}"/> algorithms.
        /// </summary>
        /// 
        /// <typeparam name="TModel">The type of the machine learning model whose parameters should be searched.</typeparam>
        /// <typeparam name="TRange">The type that specifies how ranges of the parameter values are represented.</typeparam>
        /// <typeparam name="TLearner">The type of the learning algorithm used to learn <typeparamref name="TModel"/>.</typeparam>
        /// <typeparam name="TInput">The type of the input data. Default is double[].</typeparam>
        /// <typeparam name="TOutput">The type of the output data. Default is int.</typeparam>
        /// 
        /// <param name="ranges">The range of parameters to consider during search.</param>
        /// <param name="learner">A function that can create a <typeparamref name="TModel"/> given training parameters.</param>
        /// <param name="loss">A function that can measure how far model predictions are from the expected ground-truth.</param>
        /// <param name="fit">A function that specifies how to create a new model using the teacher learning algorirhm.</param>
        /// <param name="folds">The number of folds in the k-fold cross-validation. Default is 10.</param>
        /// <param name="x">The input data to be used during training.</param>
        /// <param name="y">The output data to be used during training.</param>
        /// 
        /// <example>
        ///   <code source="Unit Tests\Accord.Tests.MachineLearning\GridSearchTest.cs" region="doc_learn_strongly_typed" />
        /// </example>
        /// 
        /// <returns>A grid-search algorithm that has been configured with the given parameters.</returns>
        /// 
        public static GridSearch<CrossValidationResult<TModel, TInput, TOutput>, TRange, CrossValidation<TModel, TInput, TOutput>, TInput, TOutput> 
            CrossValidate<TInput, TOutput, TRange, TModel, TLearner>(
            TRange ranges,
            Func<TRange, DataSubset<TInput, TOutput>, TLearner> learner,
            ComputeLoss<TOutput, TModel> loss,
            LearnNewModel<TLearner, TInput, TOutput, TModel> fit, // necessary to auto-detect TModel,
            TInput[] x, TOutput[] y, int folds = 10)
        where TModel : class, ITransform<TInput, TOutput>
        where TLearner : ISupervisedLearning<TModel, TInput, TOutput>
        {
            return GridSearch<TInput, TOutput>.CrossValidate(ranges, learner, loss, fit, folds);
        }

        /// <summary>
        ///   Creates a new <see cref="GridSearch{TModel, TRange, TLearner, TInput, TOutput}"/> combined with 
        ///   <see cref="CrossValidation{TModel, TInput, TOutput}"/> algorithms.
        /// </summary>
        /// 
        /// <typeparam name="TModel">The type of the machine learning model whose parameters should be searched.</typeparam>
        /// <typeparam name="TLearner">The type of the learning algorithm used to learn <typeparamref name="TModel"/>.</typeparam>
        /// <typeparam name="TInput">The type of the input data. Default is double[].</typeparam>
        /// <typeparam name="TOutput">The type of the output data. Default is int.</typeparam>
        /// 
        /// <param name="ranges">The range of parameters to consider during search.</param>
        /// <param name="learner">A function that can create a <typeparamref name="TModel"/> given training parameters.</param>
        /// <param name="loss">A function that can measure how far model predictions are from the expected ground-truth.</param>
        /// <param name="fit">A function that specifies how to create a new model using the teacher learning algorirhm.</param>
        /// <param name="folds">The number of folds in the k-fold cross-validation. Default is 10.</param>
        /// <param name="x">The input data to be used during training.</param>
        /// <param name="y">The output data to be used during training.</param>
        /// 
        /// <example>
        ///   <code source="Unit Tests\Accord.Tests.MachineLearning\GridSearchTest.cs" region="doc_learn_strongly_typed" />
        /// </example>
        /// 
        /// <returns>A grid-search algorithm that has been configured with the given parameters.</returns>
        /// 
        public static GridSearch<CrossValidationResult<TModel, TInput, TOutput>, CrossValidation<TModel, TInput, TOutput>, TInput, TOutput> CrossValidate<TInput, TOutput, TModel, TLearner>(
            GridSearchRange[] ranges,
            Func<GridSearchParameterCollection, DataSubset<TInput, TOutput>, TLearner> learner,
             ComputeLoss<TOutput, TModel> loss,
            LearnNewModel<TLearner, TInput, TOutput, TModel> fit, // necessary to auto-detect TModel,
            TInput[] x, TOutput[] y, int folds = 10)
        where TModel : class, ITransform<TInput, TOutput>
        where TLearner : ISupervisedLearning<TModel, TInput, TOutput>
        {
            return GridSearch<TInput, TOutput>.CrossValidate(ranges, learner, loss, fit, folds);
        }
    }
}
