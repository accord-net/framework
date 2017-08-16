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
    using System.Reflection;
    using Accord.Compat;

    /// <summary>
    ///   Grid search procedure for automatic parameter tuning.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type of the machine learning model whose parameters should be searched.</typeparam>
    /// <typeparam name="TRange">The type that specifies how ranges of the parameter values are represented.</typeparam>
    /// <typeparam name="TLearner">The type of the learning algorithm used to learn <typeparamref name="TModel"/>.</typeparam>
    /// <typeparam name="TInput">The type of the input data. Default is double[].</typeparam>
    /// <typeparam name="TOutput">The type of the output data. Default is int.</typeparam>
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
    public class GridSearch<TModel, TRange, TLearner, TInput, TOutput>
        : BaseGridSearch<GridSearchResult<TModel, TRange, TInput, TOutput>, TModel, TRange, TRange, TLearner, TInput, TOutput>
        where TLearner : ISupervisedLearning<TModel, TInput, TOutput>
        where TModel : class, ITransform<TInput, TOutput>
    {


        /// <summary>
        /// Initializes a new instance of the <see cref="GridSearch{TModel, TRange, TLearner, TInput, TOutput}"/> class.
        /// </summary>
        public GridSearch()
        {
            this.Fit = (teacher, x, y, w) => teacher.Learn(x, y, w);
        }

        /// <summary>
        ///   Inheritors of this class should return the number of possible parameter values for
        ///   each parameter in the grid-search range. For example, if a problem should search
        ///   parameters in the range {0, 1, ... 9} (10 values) and {-1, -2, -3 } (3 values), this 
        ///   method should return { 10, 3 }.
        /// </summary>
        /// 
        /// <returns>The number of possibilities for each parameter.</returns>
        /// 
        protected override int[] GetLengths()
        {
            List<int> lengths = new List<int>();
#if NETSTANDARD1_4
            var properties = ParameterRanges.GetType().GetTypeInfo().DeclaredProperties;
#else
            var properties = ParameterRanges.GetType().GetProperties();
#endif
            foreach (PropertyInfo property in properties)
            {
                Type expected = typeof(GridSearchRange<object>).GetGenericTypeDefinition();
                Type actual = property.PropertyType.GetGenericTypeDefinition();
                if (actual != expected)
                    throw new Exception();

                var p = (IGridSearchRange)property.GetValue(ParameterRanges);
                lengths.Add(p.Length);
            }

            return lengths.ToArray();
        }

        /// <summary>
        ///   Inheritors of this class should specify how to get actual values for the parameters
        ///   given a index vector in the grid-search space. Those indices indicate which values
        ///   should be given, e.g. if there are two parameters in the problem, the ranges of the
        ///   first parameter are {10, 20, 30}, and the ranges of the second parameter are {0.1, 0.01, 0.001 },
        ///   if the index vector is { 1, 2 } this method should return { 20, 0.001 }.
        /// </summary>
        /// 
        /// <param name="indices">The indices in grid-search space.</param>
        /// 
        /// <returns>The parameters at the location indicated by <paramref name="indices"/>.</returns>
        /// 
        protected override TRange GetParameters(int[] indices)
        {
            var ranges = new List<IGridSearchRange>();
#if NETSTANDARD1_4
            var properties = ParameterRanges.GetType().GetTypeInfo().DeclaredProperties;
#else
            var properties = ParameterRanges.GetType().GetProperties();
#endif
            foreach (PropertyInfo property in properties)
            {
                var p = (IGridSearchRange)property.GetValue(ParameterRanges);
                ranges.Add((IGridSearchRange)p.Clone());
            }

            for (int j = 0; j < indices.Length; j++)
                ranges[j].Index = indices[j];

            return (TRange)Activator.CreateInstance(ParameterRanges.GetType(), ranges.ToArray());
        }


    }
}
