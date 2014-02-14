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
    ///   Common interface for radially symmetric kernels.
    /// </summary>
    /// 
    /// <seealso cref="GaussianKernel"/>
    /// <seealso cref="EpanechnikovKernel"/>
    /// <seealso cref="UniformKernel"/>
    /// 
    public interface IRadiallySymmetricKernel : IDensityKernel
    {
        
        /// <summary>
        ///   Computes the kernel profile function.
        /// </summary>
        /// 
        /// <param name="x">The point estimate <c>x</c>.</param>
        /// 
        /// <returns>The value of the profile function at point <paramref name="x"/>.</returns>
        /// 
        double Profile(double x);

        /// <summary>
        ///   Computes the derivative of the kernel profile function.
        /// </summary>
        /// 
        /// <param name="x">The point estimate <c>x</c>.</param>
        /// 
        /// <returns>The value of the derivative profile function at point <paramref name="x"/>.</returns>
        /// 
        double Derivative(double x);
    }

}
