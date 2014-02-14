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
    ///   Generalized Histogram Intersection Kernel.
    /// </summary>
    /// 
    /// <remarks>
    ///   The Generalized Histogram Intersection kernel is built based on the
    ///   Histogram Intersection Kernel for image classification but applies
    ///   in a much larger variety of contexts (Boughorbel, 2005).
    /// </remarks>
    /// 
    [Serializable]
    public sealed class HistogramIntersection : IKernel, ICloneable
    {
        private double alpha;
        private double beta;

        /// <summary>
        ///   Constructs a new Generalized Histogram Intersection Kernel.
        /// </summary>
        /// 
        public HistogramIntersection(double alpha = 1, double beta = 1)
        {
            this.alpha = alpha;
            this.beta = beta;
        }

        /// <summary>
        ///   Generalized Histogram Intersection Kernel Function
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double[] x, double[] y)
        {
            double sum = 0.0;

            for (int i = 0; i < x.Length; i++)
            {
                sum += System.Math.Min(
                    System.Math.Pow(System.Math.Abs(x[i]), alpha),
                    System.Math.Pow(System.Math.Abs(y[i]), beta));
            }

            return sum;
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
