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
    ///   Common interface for function optimization methods.
    /// </summary>
    /// 
    /// <seealso cref="BoundedBroydenFletcherGoldfarbShanno"/>
    /// <seealso cref="BroydenFletcherGoldfarbShanno"/>
    /// <seealso cref="ConjugateGradient"/>
    /// <seealso cref="ResilientBackpropagation"/>
    /// <seealso cref="GoldfarbIdnani"/>
    /// 
    public interface IOptimizationMethod : IOptimizationMethod<double[], double>
    {
        // For backward compatibility
    }

    /// <summary>
    ///   Common interface for function optimization methods.
    /// </summary>
    /// 
    /// <seealso cref="BoundedBroydenFletcherGoldfarbShanno"/>
    /// <seealso cref="BroydenFletcherGoldfarbShanno"/>
    /// <seealso cref="ConjugateGradient"/>
    /// <seealso cref="ResilientBackpropagation"/>
    /// <seealso cref="GoldfarbIdnani"/>
    /// 
    public interface IOptimizationMethod<TCode> : IOptimizationMethod, IOptimizationMethod<double[], double, TCode>
        where TCode : struct
    {
        // For backward compatibility
    }

    /// <summary>
    ///   Common interface for function optimization methods.
    /// </summary>
    /// 
    /// <seealso cref="BoundedBroydenFletcherGoldfarbShanno"/>
    /// <seealso cref="BroydenFletcherGoldfarbShanno"/>
    /// <seealso cref="ConjugateGradient"/>
    /// <seealso cref="ResilientBackpropagation"/>
    /// <seealso cref="GoldfarbIdnani"/>
    /// 
    public interface IOptimizationMethod<TInput, TOutput>
    {

        /// <summary>
        ///   Gets the number of variables (free parameters)
        ///   in the optimization problem.
        /// </summary>
        /// 
        /// <value>The number of parameters.</value>
        /// 
        int NumberOfVariables { get; set; }

        /// <summary>
        ///   Gets the current solution found, the values of 
        ///   the parameters which optimizes the function.
        /// </summary>
        /// 
        TInput Solution { get; set; }

        /// <summary>
        ///   Gets the output of the function at the current <see cref="Solution"/>.
        /// </summary>
        /// 
        TOutput Value { get; }

        /// <summary>
        ///   Finds the minimum value of a function. The solution vector
        ///   will be made available at the <see cref="Solution"/> property.
        /// </summary>
        /// 
        /// <returns>Returns <c>true</c> if the method converged to a <see cref="Solution"/>.
        ///   In this case, the found value will also be available at the <see cref="Value"/>
        ///   property.</returns>
        ///  
        bool Minimize();

        /// <summary>
        ///   Finds the maximum value of a function. The solution vector
        ///   will be made available at the <see cref="Solution"/> property.
        /// </summary>
        /// 
        /// <returns>Returns <c>true</c> if the method converged to a <see cref="Solution"/>.
        ///   In this case, the found value will also be available at the <see cref="Value"/>
        ///   property.</returns>
        ///  
        bool Maximize();

    }

    

    /// <summary>
    ///   Common interface for function optimization methods.
    /// </summary>
    /// 
    /// <seealso cref="BoundedBroydenFletcherGoldfarbShanno"/>
    /// <seealso cref="BroydenFletcherGoldfarbShanno"/>
    /// <seealso cref="ConjugateGradient"/>
    /// <seealso cref="ResilientBackpropagation"/>
    /// <seealso cref="GoldfarbIdnani"/>
    /// 
    public interface IOptimizationMethod<TInput, TOutput, TCode> : IOptimizationMethod<TInput, TOutput>
        where TCode : struct
    {
        /// <summary>
        ///   Get the exit code returned in the last call to the
        ///   <see cref="IOptimizationMethod{TInput, TOutput}.Maximize()"/> or 
        ///   <see cref="IOptimizationMethod{TInput, TOutput}.Minimize()"/> methods.
        /// </summary>
        /// 
        TCode Status { get; }
    }
}
