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

namespace Accord.Statistics.Distributions
{

    /// <summary>
    ///   Common interface for mixture distributions.
    /// </summary>
    /// 
    /// <typeparam name="TDistribution">
    ///   The type of the mixture distribution, if either univariate or multivariate.
    /// </typeparam>
    /// 
    public interface IMixture<out TDistribution> : IDistribution
        where TDistribution : IDistribution
    {

        /// <summary>
        ///   Gets the mixture coefficients (component weights).
        /// </summary>
        /// 
        double[] Coefficients { get; }

        /// <summary>
        ///   Gets the mixture components.
        /// </summary>
        /// 
        TDistribution[] Components { get;  }

    }
}
