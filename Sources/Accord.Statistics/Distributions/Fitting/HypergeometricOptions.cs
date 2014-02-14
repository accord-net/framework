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

namespace Accord.Statistics.Distributions.Fitting
{
    using System;


    /// <summary>
    ///   Estimable parameters of <see cref="Accord.Statistics.Distributions.Univariate.
    ///   HypergeometricDistribution">Hypergeometric</see> distributions.
    /// </summary>
    /// 
    public enum HypergeometricParameter
    {
        /// <summary>
        ///   Population size parameter <c>N</c>.
        /// </summary>
        /// 
        PopulationSize,

        /// <summary>
        ///   Successes in population parameter <c>m</c>.
        /// </summary>
        /// 
        PopulationSuccesses
    };

    /// <summary>
    ///   Estimation options for <see cref="Accord.Statistics.Distributions.Univariate.
    ///   HypergeometricDistribution">Hypergeometric distributions</see>.
    /// </summary>
    /// 
    [Serializable]
    public class HypergeometricOptions : IFittingOptions
    {


        /// <summary>
        ///   Gets or sets which parameter of the <see cref="Accord.Statistics.Distributions.
        ///   Univariate.HypergeometricDistribution"> Hypergeometric distribution</see> should be estimated.
        /// </summary>
        /// 
        /// <value>The hypergeometric parameters to estimate.</value>
        /// 
        public HypergeometricParameter Parameter { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="HypergeometricOptions"/> class.
        /// </summary>
        /// 
        public HypergeometricOptions()
        {
        }

    }
}
