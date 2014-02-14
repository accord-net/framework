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
    ///   Dirichlet Kernel.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///      A Tutorial on Support Vector Machines (1998). Available on: http://www.umiacs.umd.edu/~joseph/support-vector-machines4.pdf </description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    [Serializable]
    public sealed class Dirichlet : IKernel, ICloneable
    {
        private int N;

        /// <summary>
        ///   Constructs a new Dirichlet Kernel
        /// </summary>
        /// 
        public Dirichlet(int dimension)
        {
            this.N = dimension;
        }


        /// <summary>
        ///   Gets or sets the dimension for the kernel. 
        /// </summary>
        /// 
        public int Dimension
        {
            get { return N; }
            set { N = value; }
        }

        /// <summary>
        ///   Dirichlet Kernel function.
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

            double prod = 1;
            for (int i = 0; i < x.Length; i++)
            {
                double delta = x[i] - y[i];
                double num = Math.Sin((N + 0.5) * (delta));
                double den = 2.0 * Math.Sin(delta / 2.0);
                prod *= num / den;
            }

            return prod;
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
