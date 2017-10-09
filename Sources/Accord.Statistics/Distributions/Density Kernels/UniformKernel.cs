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

namespace Accord.Statistics.Distributions.DensityKernels
{
    using System;
    using Accord.Compat;

    /// <summary>
    ///   Uniform density kernel.
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
    /// <example>
    /// <para>
    ///   The following example demonstrates how to use the Mean Shift algorithm with 
    ///   a <see cref="UniformKernel">uniform kernel</see> to solve a clustering task:</para>
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\Clustering\MeanShiftTest.cs" region="doc_sample1" />
    /// </example>
    /// 
    /// <seealso cref="Accord.Statistics.Distributions.Univariate.EmpiricalDistribution"/>
    /// 
    /// <seealso cref="EpanechnikovKernel"/>
    /// <seealso cref="GaussianKernel"/>
    /// 
    [Serializable]
    public class UniformKernel : IRadiallySymmetricKernel
    {

        double constant = 0.5;


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
        ///   Initializes a new instance of the <see cref="UniformKernel"/> class.
        /// </summary>
        public UniformKernel() { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="UniformKernel"/> class.
        /// </summary>
        /// 
        /// <param name="constant">The normalization constant <c>c</c>.</param>
        /// 
        public UniformKernel(double constant)
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

            if (norm <= 1)
                return constant;
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
#if DEBUG
            if (x < 0)
                throw new Exception();
#endif

            if (x <= 1)
                return 1;
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
#if DEBUG
            if (x < 0)
                throw new Exception();
#endif

            return 0;
        }
    }
}
