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
    ///   Polynomial Kernel.
    /// </summary>
    /// 
    [Serializable]
    public sealed class Polynomial : IKernel, IDistance, ICloneable
    {
        private int degree;
        private double constant;

        /// <summary>
        ///   Constructs a new Polynomial kernel of a given degree.
        /// </summary>
        /// 
        /// <param name="degree">The polynomial degree for this kernel.</param>
        /// <param name="constant">The polynomial constant for this kernel. Default is 1.</param>
        /// 
        public Polynomial(int degree, double constant)
        {
            this.degree = degree;
            this.constant = constant;
        }

        /// <summary>
        ///   Constructs a new Polynomial kernel of a given degree.
        /// </summary>
        /// 
        /// <param name="degree">The polynomial degree for this kernel.</param>
        /// 
        public Polynomial(int degree) : this(degree, 1.0) { }

        /// <summary>
        ///   Gets or sets the kernel's polynomial degree.
        /// </summary>
        /// 
        public int Degree
        {
            get { return degree; }
            set { degree = value; }
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
        ///   Polynomial kernel function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double[] x, double[] y)
        {
            double sum = constant;
            for (int i = 0; i < x.Length; i++)
                sum += x[i] * y[i];

            return Math.Pow(sum, degree);
        }

        /// <summary>
        ///   Computes the distance in input space
        ///   between two points given in feature space.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in feature (kernel) space.</param>
        /// <param name="y">Vector <c>y</c> in feature (kernel) space.</param>
        /// <returns>Distance between <c>x</c> and <c>y</c> in input space.</returns>
        /// 
        public double Distance(double[] x, double[] y)
        {
            double q = 1.0 / degree;

            return Math.Pow(Function(x, x), q) + Math.Pow(Function(y, y), q)
                - 2.0 * Math.Pow(Function(x, y), q);
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
