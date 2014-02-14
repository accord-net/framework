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
    ///   Power Kernel, also known as the (Unrectified) Triangular Kernel.
    /// </summary>
    /// 
    /// <remarks>
    ///   The Power kernel is also known as the (unrectified) triangular kernel.
    ///   It is an example of scale-invariant kernel (Sahbi and Fleuret, 2004) 
    ///   and is also only conditionally positive definite.
    /// </remarks>
    /// 
    [Serializable]
    public sealed class Power : IKernel, ICloneable
    {
        private double degree;

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
        ///   Constructs a new Power Kernel.
        /// </summary>
        /// 
        /// <param name="degree">The kernel's degree.</param>
        /// 
        public Power(int degree)
        {
            this.degree = degree;
        }

        /// <summary>
        ///   Constructs a new Power Kernel.
        /// </summary>
        /// 
        /// <param name="degree">The kernel's degree.</param>
        /// 
        public Power(double degree)
        {
            this.degree = degree;
        }

        /// <summary>
        ///   Power Kernel Function
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double[] x, double[] y)
        {
            // Optimization in case x and y are
            // exactly the same object reference.
            if (x == y) return 0.0;

            double norm = 0.0, d;
            for (int i = 0; i < x.Length; i++)
            {
                d = x[i] - y[i];
                norm += d * d;
            }

            return -System.Math.Pow(norm, degree);
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
