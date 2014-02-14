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
    ///   Estimation options for <see cref="Accord.Statistics.Distributions.Univariate.VonMisesDistribution">
    ///   Von-Mises distributions</see>.
    /// </summary>
    /// 
    [Serializable]
    public class VonMisesOptions : IFittingOptions
    {

        /// <summary>
        ///   Gets or sets a value indicating whether to use bias correction
        ///   when estimating the concentration parameter of the von-Mises
        ///   distribution.
        /// </summary>
        /// 
        /// <value><c>true</c> to use bias correction; otherwise, <c>false</c>.</value>
        /// <remarks>
        ///   For more information, see: Best, D. and Fisher N. (1981). The bias
        ///   of the maximum likelihood estimators of the von Mises-Fisher concentration
        ///   parameters. Communications in Statistics - Simulation and Computation, B10(5),
        ///   493-502.
        /// </remarks>
        /// 
        public bool UseBiasCorrection { get; set;}

        /// <summary>
        ///   Initializes a new instance of the <see cref="VonMisesOptions"/> class.
        /// </summary>
        /// 
        public VonMisesOptions()
        {
            UseBiasCorrection = false;
        }
    }
}
