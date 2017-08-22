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
    using Accord.Math;
    using Accord.Math.Distances;
    using Accord.Compat;

    /// <summary>
    ///   Normalized Polynomial Kernel. This class is equivalent to the
    ///   Normalized&gt;Polynomial> kernel but has more efficient
    ///   implementation.
    /// </summary>
    /// 
    [Serializable]
    public sealed class NormalizedPolynomial : KernelBase, IKernel,
        IDistance, ICloneable
    {
        private int degree;
        private double constant;

        /// <summary>
        ///   Constructs a new Normalized Polynomial kernel of a given degree.
        /// </summary>
        /// 
        /// <param name="degree">The polynomial degree for this kernel.</param>
        /// <param name="constant">The polynomial constant for this kernel. Default is 1.</param>
        /// 
        public NormalizedPolynomial(int degree, double constant)
        {
            this.degree = degree;
            this.constant = constant;
        }

        /// <summary>
        ///   Constructs a new Normalized Polynomial kernel of a given degree.
        /// </summary>
        /// 
        /// <param name="degree">The polynomial degree for this kernel.</param>
        /// 
        public NormalizedPolynomial(int degree)
            : this(degree, 1.0) { }

        /// <summary>
        ///   Gets or sets the kernel's polynomial degree.
        /// </summary>
        /// 
        public int Degree
        {
            get { return degree; }
            set
            {
                if (degree <= 0)
                    throw new ArgumentOutOfRangeException("value", "Degree must be positive.");

                degree = value;
            }
        }

        /// <summary>
        ///   Gets or sets the kernel's polynomial constant term.
        /// </summary>
        /// 
        public double Constant
        {
            get { return constant; }
            set { constant = value; }
        }


        /// <summary>
        ///   Normalized polynomial kernel function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public override double Function(double[] x, double[] y)
        {
            double sum = constant;
            double sumX = constant;
            double sumY = constant;

            for (int i = 0; i < x.Length; i++)
            {
                sum += x[i] * y[i];
                sumX += x[i] * x[i];
                sumY += y[i] * y[i];
            }

            return Math.Pow(sum / (sumX + sumY), degree);
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
