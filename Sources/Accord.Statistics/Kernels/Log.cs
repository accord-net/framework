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
    ///   Logarithm Kernel.
    /// </summary>
    /// 
    /// <remarks>
    ///   The Log kernel seems to be particularly interesting for
    ///   images, but is only conditionally positive definite.
    /// </remarks>
    /// 
    [Serializable]
    public sealed class Log : IKernel, ICloneable
    {
        private double degree;

        /// <summary>
        ///   Constructs a new Log Kernel
        /// </summary>
        /// 
        /// <param name="degree">The kernel's degree.</param>
        /// 
        public Log(int degree)
        {
            this.degree = degree;
        }

        /// <summary>
        ///   Constructs a new Log Kernel
        /// </summary>
        /// 
        /// <param name="degree">The kernel's degree.</param>
        /// 
        public Log(double degree)
        {
            this.degree = degree;
        }

        /// <summary>
        ///   Gets or sets the kernel's degree.
        /// </summary>
        /// 
        public double Degree
        {
            get { return degree; }
            set { degree = value; }
        }

        /// <summary>
        ///   Log Kernel function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double[] x, double[] y)
        {
            double norm = 0.0, d;

            for (int k = 0; k < x.Length; k++)
            {
                d = x[k] - y[k];
                norm += d * d;
            }

            return -System.Math.Log(System.Math.Pow(norm, degree / 2.0) + 1);
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
