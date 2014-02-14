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
    using Accord.Math;

    /// <summary>
    ///   B-Spline Kernel.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The B-Spline kernel is defined only in the interval [−1, 1]. It is 
    ///   also a member of the Radial Basis Functions family of kernels.</para>
    /// <para>  
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Bart Hamers, Kernel Models for Large Scale Applications. Doctoral thesis.
    ///       Available on: ftp://ftp.esat.kuleuven.ac.be/pub/SISTA/hamers/PhD_bhamers.pdf
    ///     </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    [Serializable]
    public sealed class BSpline : IKernel, ICloneable
    {
        private int order;

        /// <summary>
        ///   Gets or sets the B-Spline order.
        /// </summary>
        /// 
        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        /// <summary>
        ///   Constructs a new B-Spline Kernel.
        /// </summary>
        /// 
        public BSpline(int order)
        {
            this.order = order;
        }

        /// <summary>
        ///   B-Spline Kernel Function
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double[] x, double[] y)
        {
            double k = 1.0;
            int n = 2 * order + 1;

            for (int p = 0; p < x.Length; p++)
                k *= Special.BSpline(n, x[p] - y[p]);

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
