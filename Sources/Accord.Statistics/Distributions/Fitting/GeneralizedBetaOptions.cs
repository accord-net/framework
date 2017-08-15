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
    ///   BetaPERT's distribution estimation method.
    /// </summary>
    /// 
    public enum BetaPertMethod
    {
        /// <summary>
        ///   Estimates the mode using the classic method.
        /// </summary>
        /// 
        Classic,

        /// <summary>
        ///   Estimates the mode using the Vose method.
        /// </summary>
        /// 
        Vose
    }

    /// <summary>
    ///   Estimation options for <see cref="Accord.Statistics.Distributions.Univariate.GeneralizedBetaDistribution">
    ///   Beta PERT distributions</see>. 
    /// </summary>
    /// 
    [Serializable]
    public class GeneralizedBetaOptions : IFittingOptions
    {
        /// <summary>
        ///   Gets or sets the index of the minimum observed 
        ///   value, if already known. Default is -1.
        /// </summary>
        /// 
        public int MinIndex { get; set; }

        /// <summary>
        ///   Gets or sets the index of the maximum observed 
        ///   value, if already known. Default is -1.
        /// </summary>
        /// 
        public int MaxIndex { get; set; }

        /// <summary>
        ///   Gets or sets which estimation method should be used by the fitting 
        ///   algorithm. Default is <see cref="BetaEstimationMethod.Moments"/>.
        /// </summary>
        /// 
        public BetaEstimationMethod Method { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether the observations are already sorted.
        /// </summary>
        /// 
        /// <value>
        ///   Set to <c>true</c> if the observations are sorted; otherwise, <c>false</c>.
        /// </value>
        /// 
        public bool IsSorted { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether the maximum
        ///   value should be treated as fixed and not be estimated.
        ///   Default is true.
        /// </summary>
        /// 
        public bool FixMax { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether the minimum
        ///   value should be treated as fixed and not be estimated.
        ///   Default is true.
        /// </summary>
        /// 
        public bool FixMin { get; set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="GeneralizedBetaOptions"/> class.
        /// </summary>
        /// 
        public GeneralizedBetaOptions()
        {
            MinIndex = -1;
            MaxIndex = -1;
            IsSorted = false;
            FixMin = true;
            FixMax = true;
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
