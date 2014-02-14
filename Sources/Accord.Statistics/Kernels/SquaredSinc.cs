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
    ///   Squared Sinc Kernel.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///      Chaudhuri et al, A Comparative Study of Kernels for the Multi-class Support Vector
    ///      Machine, 2008. Available on: http://www.computer.org/portal/web/csdl/doi/10.1109/ICNC.2008.803 </description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    [Serializable]
    public sealed class SquaredSinc : IKernel, ICloneable
    {
        private double gamma;

        /// <summary>
        ///   Constructs a new Squared Sinc Kernel
        /// </summary>
        /// 
        public SquaredSinc(double gamma)
        {
            this.Gamma = gamma;
        }

        /// <summary>
        ///   Gets or sets the gamma value for the kernel.
        /// </summary>
        /// 
        public double Gamma
        {
            get { return gamma; }
            set { gamma = value; }
        }

        /// <summary>
        ///   Squared Sine Kernel function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double[] x, double[] y)
        {
            double norm = 0.0, d;
            for (int i = 0; i < x.Length; i++)
            {
                d = x[i] - y[i];
                norm += d * d;
            }

            double num = gamma * Math.Sqrt(norm);
            double den = gamma * gamma * norm;

            return Math.Sin(num) / den;
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
