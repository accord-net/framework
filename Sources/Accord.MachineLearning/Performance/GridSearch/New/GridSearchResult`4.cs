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
    using Accord.Compat;

    /// <summary>
    ///   Contains results from the grid-search procedure.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type of the machine learning model whose parameters should be searched.</typeparam>
    /// <typeparam name="TInput">The type of the input data. Default is double[].</typeparam>
    /// <typeparam name="TOutput">The type of the output data. Default is int.</typeparam>
    /// 
    /// <seealso cref="GridSearch"/>
    /// 
    public class GridSearchResult<TModel, TInput, TOutput> : GridSearchResult<TModel, GridSearchParameterCollection, TInput, TOutput>
        where TModel : class, ITransform<TInput, TOutput>
    {
    }

    /// <summary>
    ///   Contains results from the grid-search procedure.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type of the machine learning model whose parameters should be searched.</typeparam>
    /// <typeparam name="TParam">The type that specifies how the value for a single parameter is represented.</typeparam>
    /// <typeparam name="TInput">The type of the input data. Default is double[].</typeparam>
    /// <typeparam name="TOutput">The type of the output data. Default is int.</typeparam>
    /// 
    /// <seealso cref="GridSearch"/>
    /// 
    [Serializable]
    public class GridSearchResult<TModel, TParam, TInput, TOutput> : ITransform<TInput, TOutput>
        where TModel : class, ITransform<TInput, TOutput>
    {

        private TParam[] parameters;
        private TModel[] models;
        private double[] errors;
        private int bestIndex;
        private Exception[] exceptions;

        /// <summary>
        ///   Gets all combination of parameters tried.
        /// </summary>
        /// 
        public TParam[] Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        /// <summary>
        ///   Gets all models created during the search.
        /// </summary>
        /// 
        public TModel[] Models
        {
            get { return models; }
            set { models = value; }
        }

        /// <summary>
        ///   Gets the error for each of the created models.
        /// </summary>
        /// 
        public double[] Errors
        {
            get { return errors; }
            set { errors = value; }
        }

        /// <summary>
        ///   Gets exceptions found during the learning of each of the created models, if any.
        /// </summary>
        /// 
        public Exception[] Exceptions
        {
            get { return exceptions; }
            set { exceptions = value; }
        }


        /// <summary>
        ///   Gets the index of the best found model
        ///   in the <see cref="Models"/> collection.
        /// </summary>
        /// 
        public int BestModelIndex
        {
            get { return bestIndex; }
            set { bestIndex = value; }
        }

        /// <summary>
        ///   Gets the best model found.
        /// </summary>
        /// 
        public TModel BestModel
        {
            get { return models[bestIndex]; }
        }

        /// <summary>
        ///   Gets the best parameter combination found.
        /// </summary>
        /// 
        public TParam BestParameters
        {
            get { return parameters[bestIndex]; }
        }

        /// <summary>
        ///   Gets the minimum validation error found. If this
        ///   result has been retrieved through Grid-Search Cross-Validation,
        ///   this will correspond to the minimum average validation error
        ///   for the different data splits (validation folds).
        /// </summary>
        /// 
        public double BestModelError
        {
            get { return errors[bestIndex]; }
        }


        /// <summary>
        ///   Gets the size of the grid used in the grid-search.
        /// </summary>
        /// 
        public int Count
        {
            get { return models.Length; }
        }

        /// <summary>
        /// Gets the number of inputs accepted by the model.
        /// </summary>
        /// 
        public int NumberOfInputs
        {
            get { return BestModel.NumberOfInputs; }
            set { throw new InvalidOperationException("This property is read-only."); }
        }

        /// <summary>
        /// Gets the number of outputs generated by the model.
        /// </summary>
        /// 
        public int NumberOfOutputs
        {
            get { return BestModel.NumberOfOutputs; }
            set { throw new InvalidOperationException("This property is read-only."); }
        }


        /// <summary>
        /// Applies the transformation to an input, producing an associated output.
        /// </summary>
        /// <param name="input">The input data to which the transformation should be applied.</param>
        /// <returns>The output generated by applying this transformation to the given input.</returns>
        public TOutput Transform(TInput input)
        {
            return BestModel.Transform(input);
        }

        /// <summary>
        /// Applies the transformation to a set of input vectors,
        /// producing an associated set of output vectors.
        /// </summary>
        /// <param name="input">The input data to which
        /// the transformation should be applied.</param>
        /// <returns>The output generated by applying this
        /// transformation to the given input.</returns>
        public TOutput[] Transform(TInput[] input)
        {
            return BestModel.Transform(input);
        }

        /// <summary>
        /// Applies the transformation to a set of input vectors,
        /// producing an associated set of output vectors.
        /// </summary>
        /// <param name="input">The input data to which
        /// the transformation should be applied.</param>
        /// <param name="result">The location to where to store the
        /// result of this transformation.</param>
        /// <returns>The output generated by applying this
        /// transformation to the given input.</returns>
        public TOutput[] Transform(TInput[] input, TOutput[] result)
        {
            return BestModel.Transform(input, result);
        }
    }
}
