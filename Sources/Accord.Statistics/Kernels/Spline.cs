// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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

    /// <summary>
    ///   Infinite Spline Kernel function.
    /// </summary>
    /// 
    /// <remarks>
    ///   The Spline kernel is given as a piece-wise cubic
    ///   polynomial, as derived in the works by Gunn (1998).
    /// </remarks>
    /// 
    [Serializable]
    public sealed class Spline : IKernel, ICloneable
    {

        /// <summary>
        ///   Constructs a new Spline Kernel.
        /// </summary>
        /// 
        public Spline()
        {
        }

        /// <summary>
        ///   Spline Kernel Function
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double[] x, double[] y)
        {
            double k = 1;
            for (int i = 0; i < x.Length; i++)
            {
                double min = System.Math.Min(x[i], y[i]);
                double xy = x[i] * y[i];

                // prod{1}^d 1 + xy + xy*min - (x+y)/2 min² + min³/3} 
                k *= 1.0 + xy + xy * min - ((x[i] + y[i]) / 2.0) * min * min + (min * min * min) / 3.0;
            }

            return k;
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
