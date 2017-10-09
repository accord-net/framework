﻿// Accord Statistics Library
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

namespace Accord.Statistics.Distributions.DensityKernels
{
    using System;
    using Accord.Compat;
    using Accord.Statistics.Kernels;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Distributions.Multivariate;

    /// <summary>
    ///   Gaussian density kernel.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class provides a Gaussian density kernel (not to be confused
    ///   with a <see cref="Gaussian"/> kernel function) to be used in density
    ///   estimation models (i.e. <see cref="EmpiricalDistribution"/>) and 
    ///   clustering algorithms (i.e. MeanShift.</para>
    /// 
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
    /// <example>
    /// <para>
    ///   The following example shows how to fit a <see cref="MultivariateEmpiricalDistribution"/> 
    ///   using <see cref="GaussianKernel">Gaussian kernels</see>:</para>
    ///   <code source="Unit Tests\Accord.Tests.Statistics\Distributions\Multivariate\Continuous\MultivariateEmpiricalDistributionTest.cs" region="doc_fit_gaussian" />
    /// </example>
    /// 
    /// <seealso cref="Accord.Statistics.Distributions.Univariate.EmpiricalDistribution"/>
    /// <seealso cref="EpanechnikovKernel"/>
    /// <seealso cref="UniformKernel"/>
    ///
    [Serializable]
    public class GaussianKernel : IRadiallySymmetricKernel
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
        ///   Initializes a new instance of the <see cref="GaussianKernel"/> class.
        /// </summary>
        /// 
        /// <param name="dimension">The desired dimension <c>d</c>.</param>
        /// 
        public GaussianKernel(int dimension)
        {
            constant = 1.0 / Math.Pow(2 * Math.PI, dimension / 2.0);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GaussianKernel"/> class.
        /// </summary>
        /// 
        /// <param name="constant">The normalization constant to use.</param>
        /// 
        public GaussianKernel(double constant)
        {
            this.constant = constant;
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
        public double Function(double[] x)
        {
            double norm = 0.0;
            for (int i = 0; i < x.Length; i++)
                norm += x[i] * x[i];

            return constant * Math.Exp(-0.5 * norm * norm);
        }

        /// <summary>
        ///   Computes the kernel profile function.
        /// </summary>
        /// 
        /// <param name="x">The squared point estimate <c>x²</c>.</param>
        /// 
        /// <returns>
        ///   The value of the profile function at point <paramref name="x"/>².
        /// </returns>
        /// 
        public double Profile(double x)
        {
#if DEBUG
            if (x < 0)
                throw new Exception();
#endif

            // Equation 9 in (Comaniciu & Meer)
            // https://courses.csail.mit.edu/6.869/handouts/PAMIMeanshift.pdf
            // and http://www.serc.iisc.ernet.in/~venky/SE263/slides/Mean-Shift-Theory.pdf

            return Math.Exp(-0.5 * x);
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
#if DEBUG
            if (x < 0)
                throw new Exception();
#endif

            return -0.5 * Math.Exp(-0.5 * x);
        }
    }
}
