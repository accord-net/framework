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

namespace Accord.Statistics.Distributions.DensityKernels
{
    using System;
    using Accord.Math;

    /// <summary>
    ///   Epanechnikov density kernel.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Comaniciu, Dorin, and Peter Meer. "Mean shift: A robust approach toward 
    ///       feature space analysis." Pattern Analysis and Machine Intelligence, IEEE 
    ///       Transactions on 24.5 (2002): 603-619. Available at:
    ///       http://ieeexplore.ieee.org/xpls/abs_all.jsp?arnumber=1000236 </description></item>
    ///     <item><description>
    ///       Dan Styer, Oberlin College Department of Physics and Astronomy; Volume of a d-dimensional
    ///       sphere. Last updated 30 August 2007. Available at:
    ///       http://www.oberlin.edu/physics/dstyer/StatMech/VolumeDSphere.pdf </description></item>
    ///     <item><description>
    ///       David W. Scott, Multivariate Density Estimation: Theory, Practice, and 
    ///       Visualization, Wiley, Aug 31, 1992</description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <seealso cref="Accord.Statistics.Distributions.Univariate.EmpiricalDistribution"/>
    /// 
    [Serializable]
    public class EpanechnikovKernel : IRadiallySymmetricKernel
    {

        private double constant;

        /// <summary>
        ///   Gets or sets the kernel's normalization constant.
        /// </summary>
        /// 
        public double Constant
        {
            get { return constant; }
            set { constant = value; }
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="EpanechnikovKernel"/> class.
        /// </summary>
        /// 
        public EpanechnikovKernel()
            : this(1)
        { 
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="EpanechnikovKernel"/> class.
        /// </summary>
        /// 
        /// <param name="constant">The constant by which the kernel formula
        ///   is multiplied at the end. Default is to consider the area
        ///   of a unit-sphere of dimension 1.</param>
        /// 
        public EpanechnikovKernel(double constant)
        {
            this.constant = constant;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="EpanechnikovKernel"/> class.
        /// </summary>
        /// 
        /// <param name="dimension">The desired dimension <c>d</c>.</param>
        /// 
        public EpanechnikovKernel(int dimension)
        {
            // compute the area of the d-dimensional unit hypersphere
            // http://www.oberlin.edu/physics/dstyer/StatMech/VolumeDSphere.pdf

            double num = Math.Pow(Math.PI, dimension / 2.0);
            double den = Gamma.Function(dimension / 2.0 + 1);

            double area = num / den;

            this.constant = (1.0 / (2.0 * area)) * (dimension + 2);
        }

        /// <summary>
        ///   Computes the kernel density function.
        /// </summary>
        /// 
        /// <param name="x">The input point.</param>
        /// 
        /// <returns>
        ///   A density estimate around <paramref name="x"/>.
        /// </returns>
        /// 
        public double Function(params double[] x)
        {
            double sum = 0.0;
            for (int i = 0; i < x.Length; i++)
                sum += x[i] * x[i];

            if (sum < 1)
                return constant * (1.0 - sum);
            return 0;
        }

        /// <summary>
        ///   Computes the kernel profile function.
        /// </summary>
        /// 
        /// <param name="x">The point estimate <c>x</c>.</param>
        /// 
        /// <returns>
        ///   The value of the profile function at point <paramref name="x"/>.
        /// </returns>
        /// 
        public double Profile(double x)
        {
            if (x < 1)
                return constant * (1 - x);
            return 0;
        }

        /// <summary>
        ///   Computes the derivative of the kernel profile function.
        /// </summary>
        /// 
        /// <param name="x">The point estimate <c>x</c>.</param>
        /// 
        /// <returns>
        ///   The value of the derivative profile function at point <paramref name="x"/>.
        /// </returns>
        /// 
        public double Derivative(double x)
        {
            return -constant;
        }
    }
}
