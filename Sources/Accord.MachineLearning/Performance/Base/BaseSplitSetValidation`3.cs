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

    /// <summary>
    ///   Base class for performance measurement methods based on splitting the data into multiple sets,
    ///   such as <see cref="SplitSetValidation{TModel, TInput, TOutput}"/>, <see cref="CrossValidation{TModel, TInput, TOutput}"/>
    ///   and <see cref="Bootstrap{TModel, TInput, TOutput}"/>.
    /// </summary>
    /// 
    /// <typeparam name="TResult">The type of the result learned by the validation method (e.g. <see cref="CrossValidationResult{TModel, TInput, TOutput}"/>).</typeparam>
    /// <typeparam name="TModel">The type of the machine learning model.</typeparam>
    /// <typeparam name="TInput">The type of the input data.</typeparam>
    /// <typeparam name="TOutput">The type of the output data or labels.</typeparam>
    /// 
    /// <seealso cref="GridSearch"/>
    /// <seealso cref="SplitSetValidation{TModel, TInput, TOutput}"/>
    /// <seealso cref="CrossValidation{TModel, TInput, TOutput}"/>
    /// <seealso cref="Bootstrap{TModel, TInput, TOutput}"/>
    /// 
    public abstract class BaseSplitSetValidation<TResult, TModel, TInput, TOutput> : BaseSplitSetValidation<
            TResult, 
            TModel, 
            ISupervisedLearning<TModel, TInput, TOutput>,
            TInput, TOutput>,
        ISupervisedLearning<TResult, TInput, TOutput>
        where TModel : class, ITransform<TInput, TOutput>
        where TResult : ITransform<TInput, TOutput>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSplitSetValidation{TResult, TModel, TInput, TOutput}"/> class.
        /// </summary>
        public BaseSplitSetValidation()
        {
            this.Fit = (teacher, inputs, outputs, weight) => teacher.Learn(inputs, outputs, weight);
        }
    }
}
