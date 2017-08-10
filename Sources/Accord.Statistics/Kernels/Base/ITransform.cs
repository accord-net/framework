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
    using System;
    using Accord.Compat;

    /// <summary>
    ///   Common interface for kernel functions that can explicitly 
    ///   project input points into the kernel feature space.
    /// </summary>
    /// 
    /// <seealso cref="IKernel"/>
    ///
    public interface ITransform : IKernel, ICloneable, ITransform<double[]>
    {
    }
    
    /// <summary>
    ///   Common interface for kernel functions that can explicitly 
    ///   project input points into the kernel feature space.
    /// </summary>
    /// 
    /// <seealso cref="IKernel"/>
    ///
    public interface ITransform<TInput> : IKernel<TInput>, ICloneable
    {
        /// <summary>
        ///   Projects an input point into feature space.
        /// </summary>
        /// 
        /// <param name="input">The input point to be projected into feature space.</param>
        /// 
        /// <returns>
        ///   The feature space representation of the given <paramref name="input"/> point.
        /// </returns>
        /// 
        double[] Transform(TInput input);

        /// <summary>
        ///   Projects an input point into feature space.
        /// </summary>
        /// 
        /// <param name="input">The input point to be projected into feature space.</param>
        /// 
        /// <returns>
        ///   The feature space representation of the given <paramref name="input"/> point.
        /// </returns>
        /// 
        double[][] Transform(TInput[] input);
    }
}
