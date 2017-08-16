// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Diego Catalano, 2015
// diego.catalano at live.com
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
    ///   Hellinger Kernel.
    /// </summary>
    /// 
    /// <remarks>
    ///   Hellinger kernel is an euclidean norm of linear kernel.
    ///   Reference: http://www.di.ens.fr/willow/events/cvml2011/materials/practical-classification/
    /// </remarks>
    /// 
    [Serializable]
    public sealed class Hellinger : KernelBase, IKernel, ICloneable
    {

        /// <summary>
        /// Constructs a new Hellinger Kernel.
        /// </summary>
        public Hellinger() { }

        /// <summary>
        ///   Hellinger Function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public override double Function(double[] x, double[] y)
        {
            double r = 0;
            for (int i = 0; i < x.Length; i++)
            {
                r += Math.Sqrt(x[i] * y[i]);
            }

            return r;
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