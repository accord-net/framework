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

namespace Accord.Statistics.Distributions.Fitting
{
    using System;
    using Accord.Compat;

    /// <summary>
    ///   Estimation methods for <see cref="Accord.Statistics.Distributions.Univariate.BetaDistribution">
    ///   Beta distributions</see>.
    /// </summary>
    /// 
    public enum BetaEstimationMethod
    {
        /// <summary>
        ///   Method-of-moments estimation.
        /// </summary>
        /// 
        Moments,

        /// <summary>
        ///   Maximum Likelihood estimation.
        /// </summary>
        /// 
        MaximumLikelihood
    };

    /// <summary>
    ///   Estimation options for <see cref="Accord.Statistics.Distributions.Univariate.BetaDistribution">
    ///   Beta distributions</see>.
    /// </summary>
    /// 
    [Serializable]
    public class BetaOptions : IFittingOptions
    {
     
        /// <summary>
        ///   Gets or sets which estimation method should be used by the fitting 
        ///   algorithm. Default is <see cref="BetaEstimationMethod.Moments"/>.
        /// </summary>
        /// 
        public BetaEstimationMethod Method { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="BetaOptions"/> class.
        /// </summary>
        /// 
        public BetaOptions()
        {
            Method = BetaEstimationMethod.Moments;
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
