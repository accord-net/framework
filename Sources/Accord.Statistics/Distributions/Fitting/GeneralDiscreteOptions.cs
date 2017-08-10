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
    ///   Estimation options for <see cref="Accord.Statistics.Distributions.Univariate.
    ///   GeneralDiscreteDistribution">general discrete (categorical) distributions</see>.
    /// </summary>
    /// 
    [Serializable]
    public class GeneralDiscreteOptions : IFittingOptions
    {
        /// <summary>
        ///   Gets or sets the minimum allowed probability
        ///   in the frequency tables specifying the discrete
        ///   distribution. Default is 1e-10.
        /// </summary>
        /// 
        public double Minimum { get; set; }

        /// <summary>
        ///   Gets ors sets whether to use Laplace's rule of succession 
        ///   to avoid zero probabilities. Default is false.
        /// </summary>
        /// 
        public bool UseLaplaceRule { get; set; }

        /// <summary>
        ///   Gets or sets how much percent of the previous value for
        ///   the distribution should be kept in its updated value. Default is 0.
        /// </summary>
        /// 
        public double Regularization { get; set; }

        /// <summary>
        ///   Gets or sets whether current frequency values in the distribution
        ///   should be considered as priors during the next time the distribution
        ///   is estimated. Default is false.
        /// </summary>
        /// 
        public bool UsePreviousValuesAsPriors { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GeneralDiscreteOptions"/> class.
        /// </summary>
        /// 
        public GeneralDiscreteOptions()
        {
            Minimum = 1e-10;
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
