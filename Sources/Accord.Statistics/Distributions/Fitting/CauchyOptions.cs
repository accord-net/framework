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

namespace Accord.Statistics.Distributions.Fitting
{
    using System;
    using Accord.Compat;

    /// <summary>
    ///   Estimation options for <see cref="Accord.Statistics.Distributions.Univariate.CauchyDistribution">
    ///   Cauchy distributions</see>.
    /// </summary>
    /// 
    [Serializable]
    public class CauchyOptions : IFittingOptions
    {

        /// <summary>
        ///   Gets or sets a value indicating whether the distribution parameters
        ///   should be estimated using maximum likelihood. Default is true.
        /// </summary>
        /// 
        /// <remarks>
        ///   The Cauchy distribution parameters can be estimated in many ways. One
        ///   approach is to use order statistics to derive approximations to the
        ///   location and scale parameters by analysis the interquartile range of
        ///   the data. The other approach is to use Maximum Likelihood to estimate
        ///   the parameters. The MLE does not exists in simple algebraic form, so
        ///   it has to be estimated using numeric optimization.
        /// </remarks>
        /// 
        /// <value><c>true</c> if the parameters should be estimated by ML; otherwise, <c>false</c>.</value>
        /// 
        public bool MaximumLikelihood { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether the scale
        ///   parameter should be estimated. Default is true.
        /// </summary>
        /// 
        /// <value><c>true</c> if the scale parameter should be estimated; otherwise, <c>false</c>.</value>
        /// 
        public bool EstimateScale { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether the location
        ///   parameter should be estimated. Default is true.
        /// </summary>
        /// 
        /// <value><c>true</c> if the location parameter should be estimated; otherwise, <c>false</c>.</value>
        /// 
        public bool EstimateLocation { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CauchyOptions"/> class.
        /// </summary>
        /// 
        public CauchyOptions()
        {
            MaximumLikelihood = true;
            EstimateLocation = true;
            EstimateScale = true;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }

}
