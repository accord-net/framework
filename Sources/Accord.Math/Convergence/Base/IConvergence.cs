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

namespace Accord.Math
{
    using System;

    /// <summary>
    ///   Common interface for convergence detection algorithms.
    /// </summary>
    /// 
    public interface IConvergence<T> : IConvergence
    {
        /// <summary>
        ///   Please use MaxIterations instead.
        /// </summary>
        [Obsolete("Please use MaxIterations instead.")]
        int Iterations { get; set; }

        /// <summary>
        ///   Gets or sets the watched value after the iteration.
        /// </summary>
        /// 
        T NewValue { get; set; }
    }

    /// <summary>
    ///   Common interface for convergence detection algorithms.
    /// </summary>
    /// 
    public interface IConvergence
    {
        /// <summary>
        ///   Gets or sets the maximum relative change in the watched value
        ///   after an iteration of the algorithm used to detect convergence.
        /// </summary>
        /// 
        double Tolerance { get; set; }

        /// <summary>
        ///   Gets or sets the maximum number of iterations
        ///   performed by the iterative algorithm.
        /// </summary>
        /// 
        int MaxIterations { get; set; } 

        /// <summary>
        ///   Gets the current iteration number.
        /// </summary>
        /// 
        int CurrentIteration { get; }

        /// <summary>
        ///   Gets or sets whether the algorithm has converged.
        /// </summary>
        /// 
        bool HasConverged { get; }

        /// <summary>
        ///   Resets this instance, reverting all iteration statistics
        ///   statistics (number of iterations, last error) back to zero.
        /// </summary>
        /// 
        void Clear();
    }
}
