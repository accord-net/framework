// Accord Math Library
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

namespace Accord.Math.Optimization
{
    using System;

    /// <summary>
    ///   Common interface for function optimization methods which depend on
    ///   having both an objective function and a gradient function definition
    ///   available.
    /// </summary>
    /// 
    /// <seealso cref="BroydenFletcherGoldfarbShanno"/>
    /// <seealso cref="ConjugateGradient"/>
    /// <seealso cref="ResilientBackpropagation"/>
    /// 
    public interface IGradientOptimizationMethod : IOptimizationMethod, IGradientOptimizationMethod<double[], double>
    {
        // For backward compatibility

    }

    /// <summary>
    ///   Common interface for function optimization methods which depend on
    ///   having both an objective function and a gradient function definition
    ///   available.
    /// </summary>
    /// 
    /// <seealso cref="BroydenFletcherGoldfarbShanno"/>
    /// <seealso cref="ConjugateGradient"/>
    /// <seealso cref="ResilientBackpropagation"/>
    /// 
    public interface IGradientOptimizationMethod<TInput, TOutput> : IFunctionOptimizationMethod<TInput, TOutput>
    {
        /// <summary>
        ///   Gets or sets a function returning the gradient
        ///   vector of the function to be optimized for a
        ///   given value of its free parameters.
        /// </summary>
        /// 
        /// <value>The gradient function.</value>
        /// 
        Func<TInput, TInput> Gradient { get; set; }

    }
}
