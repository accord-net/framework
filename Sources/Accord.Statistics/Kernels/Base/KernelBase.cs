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

namespace Accord.Statistics.Kernels
{
    using Accord.Math;
    using Accord.Math.Distances;
    using System;
    using Accord.Compat;

    /// <summary>
    ///   Base class for kernel functions. This class provides automatic
    ///   distance calculations for classes that do not provide optimized
    ///   implementations.
    /// </summary>
    /// 
    [Serializable]
    public abstract class KernelBase : KernelBase<double[]>, IKernel, IDistance
    {
    }

    /// <summary>
    ///   Base class for kernel functions. This class provides automatic
    ///   distance calculations for classes that do not provide optimized
    ///   implementations.
    /// </summary>
    /// 
    [Serializable]
    public abstract class KernelBase<TInput> : IDistance<TInput>, IKernel<TInput>
    {

        /// <summary>
        ///   Computes the squared distance in feature space
        ///   between two points given in input space.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// 
        /// <returns>
        ///   Squared distance between <c>x</c> and <c>y</c> in feature (kernel) space.
        /// </returns>
        /// 
        public virtual double Distance(TInput x, TInput y)
        {
            return Function(x, x) + Function(y, y) - 2 * Function(x, y);
        }


        /// <summary>
        ///   The kernel function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// 
        /// <returns>
        ///   Dot product in feature (kernel) space.
        /// </returns>
        /// 
        public abstract double Function(TInput x, TInput y);
        
    }
}
