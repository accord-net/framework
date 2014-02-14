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
    ///   Multiquadric Kernel.
    /// </summary>
    /// 
    /// <remarks>
    ///   The multiquadric kernel is only conditionally positive-definite.
    /// </remarks>
    /// 
    [Serializable]
    public sealed class Multiquadric : IKernel, ICloneable
    {

        private double constant;

        /// <summary>
        ///   Gets or sets the kernel's constant value.
        /// </summary>
        /// 
        public double Constant
        {
            get { return constant; }
            set { constant = value; }
        }

        /// <summary>
        ///   Constructs a new Multiquadric Kernel.
        /// </summary>
        /// 
        /// <param name="constant">The constant term theta.</param>
        /// 
        public Multiquadric(double constant)
        {
            this.constant = constant;
        }

        /// <summary>
        ///   Constructs a new Multiquadric Kernel.
        /// </summary>
        /// 
        public Multiquadric() : this(1) { }

        /// <summary>
        ///   Multiquadric Kernel function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double[] x, double[] y)
        {
            double norm = 0.0;
            for (int i = 0; i < x.Length; i++)
            {
                double d = x[i] - y[i];
                norm += d * d;
            }

            return -(norm + constant * constant);
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
