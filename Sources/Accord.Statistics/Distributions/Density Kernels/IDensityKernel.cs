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

    /// <summary>
    ///   Common interface for density estimation kernels.
    /// </summary>
    /// 
    /// <remarks>
    ///   Those kernels are different from <see cref="Accord.Statistics.Kernels.IKernel"> kernel
    ///   functions</see>. Density estimation kernels are required to obey normalization rules in
    ///   order to fulfill integrability and behavioral properties. Moreover, they are defined
    ///   over a single input vector, the point estimate of a random variable.
    /// </remarks>
    /// 
    /// <seealso cref="GaussianKernel"/>
    /// <seealso cref="EpanechnikovKernel"/>
    /// <seealso cref="UniformKernel"/>
    /// 
    public interface IDensityKernel
    {
        /// <summary>
        ///   Computes the kernel density function.
        /// </summary>
        /// 
        /// <param name="x">The input point.</param>
        /// 
        /// <returns>A density estimate around <paramref name="x"/>.</returns>
        /// 
        double Function(params double[] x);
    }
}
