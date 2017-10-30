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
    ///   Normalized Kernel.
    /// </summary>
    /// 
    /// <remarks>
    ///   This kernel definition can be used to provide normalized versions
    ///   of other kernel classes, such as the <see cref="Polynomial"/>. A
    ///   normalized kernel will always produce distances between -1 and 1.
    /// </remarks>
    /// 
    [Serializable]
    public sealed class Normalized<TKernel, TInput> : KernelBase<TInput>, IKernel<TInput>, ICloneable 
        where TKernel : IKernel<TInput>
    {
        private TKernel kernel;
        private double power;

        /// <summary>
        ///   Gets or sets the inner kernel function 
        ///   whose results should be normalized.
        /// </summary>
        /// 
        public TKernel Kernel
        {
            get { return kernel; }
            set { kernel = value; }
        }

        /// <summary>
        ///   Gets or sets the normalization power to be 
        ///   used. Default is 0.5 (take the square root).
        /// </summary>
        /// 
        public double Power
        {
            get { return power; }
            set { power = value; }
        }

        /// <summary>
        ///   Constructs a new Cauchy Kernel.
        /// </summary>
        /// 
        /// <param name="kernel">The kernel function to be normalized.</param>
        /// <param name="power">The normalization power to be used. Default is 0.5 (take the square root).</param>
        /// 
        public Normalized(TKernel kernel, double power = 0.5)
        {
            this.kernel = kernel;
            this.power = power;
        }

        /// <summary>
        ///   Normalized Kernel Function
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public override double Function(TInput x, TInput y)
        {
            return kernel.Function(x, y) / Math.Pow(kernel.Function(x, x) * kernel.Function(y, y), power);
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
