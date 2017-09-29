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

    /// <summary>
    ///   Interface for Radial Basis Function kernels.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   A radial basis function (RBF) is a real-valued function whose value depends only 
    ///   on the distance from the origin, so that <c>ϕ(x) = ϕ(||x||)</c>; or alternatively 
    ///   on the distance from some other point <c>c</c>, called a center, so that 
    ///   <c>ϕ(x,c) = ϕ(||x−c||)</c>. Any function ϕ that satisfies the property 
    ///   <c>ϕ(x) = ϕ(||x||)</c> is a radial function. The norm is usually Euclidean distance,
    ///   although other distance functions are also possible. </para>
    ///   
    /// <para>
    ///   Examples of radial basis kernels include:</para>
    /// <list type="bullet">  
    ///   <description><see cref="Circular"/></description>  
    ///   <description><see cref="Gaussian"/></description>  
    ///   <description><see cref="Log"/></description>  
    ///   <description><see cref="Multiquadric"/></description>  
    ///   <description><see cref="Power "/></description>  
    ///   <description><see cref="RationalQuadratic"/></description>  
    ///   <description><see cref="Spherical"/></description>  
    ///   <description><see cref="SquaredSinc"/></description>  
    ///   <description><see cref="SymmetricTriangle"/></description>  
    ///   <description><see cref="TStudent"/></description>  
    ///   <description><see cref="Wave"/></description>  
    /// </list>  
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia, The Free Encyclopedia. Radial basis functions. Available on:
    ///       https://en.wikipedia.org/wiki/Radial_basis_function </description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    /// <code source="Unit Tests\Accord.Tests.Statistics\Kernels\GaussianTest.cs" region="doc_rbf" />
    /// 
    public interface IRadialBasisKernel : IKernel
    {

        /// <summary>
        ///   The kernel function.
        /// </summary>
        /// 
        /// <param name="z">Distance <c>z</c> between two vectors in input space.</param>
        /// 
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        double Function(double z);

    }
}
