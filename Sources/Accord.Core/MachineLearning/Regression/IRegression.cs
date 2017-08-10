// Accord Statistics Library
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
    using Accord.MachineLearning;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    ///   Common interface for regression models. Regression models
    ///   learn how to produce a real value (or a set of real values) <c>y</c>
    ///   from an input vector <c>x</c>.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// <typeparam name="TOutput">The data type for the predicted variables. Default is double.</typeparam>
    /// 
    public interface IRegression<TInput, TOutput> :
        ITransform<TInput, TOutput>
    {
        /// <summary>
        ///   Computes an output value for a given <paramref name="input"/>.
        /// </summary>
        /// 
        /// <param name="input">The input vector whose associated output
        ///   value should be computed.</param>
        /// 
        TOutput Regress(TInput input);

        /// <summary>
        ///   Computes output values for each vector in the given set of
        ///   <paramref name="input"/> vectors.
        /// </summary>
        /// 
        /// <param name="input">The input vectors whose output values 
        ///   should be computed.</param>
        /// 
        TOutput[] Regress(TInput[] input);

        /// <summary>
        ///   Computes output values for each vector in the given set of
        ///   <paramref name="input"/> vectors.
        /// </summary>
        /// 
        /// <param name="input">The input vectors whose output values 
        ///   should be computed.</param>
        /// <param name="result">The location where to store the output values.</param>
        /// 
        TOutput[] Regress(TInput[] input, TOutput[] result);

    }

    /// <summary>
    ///   Common interface for regression models. Regression models
    ///   learn how to produce a real value (or a set of real values) <c>y</c>
    ///   from an input vector <c>x</c>.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// 
    public interface IRegression<TInput> :
        IRegression<TInput, double>,
        IRegression<TInput, float>
    {
    }
}
