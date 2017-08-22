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

namespace Accord.Statistics.Distributions.Multivariate
{
    using System;
    using Accord.Statistics.Distributions.Univariate;

    /// <summary>
    ///   Common interface for components of <see cref="Mixture{T}"/> 
    ///   and <see cref="MultivariateMixture{T}"/> distributions.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the component distribution.</typeparam>
    /// 
    public interface IMixtureComponent<T> where T : IDistribution
    {

        /// <summary>
        ///   Gets the component distribution.
        /// </summary>
        /// 
        T Component { get; }

        /// <summary>
        ///   Gets the weight associated with this component.
        /// </summary>
        /// 
        double Coefficient { get; }
    }
}
