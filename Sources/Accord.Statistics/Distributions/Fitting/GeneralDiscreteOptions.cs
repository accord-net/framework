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
        ///   distribution.
        /// </summary>
        /// 
        public double Minimum { get; set; }

        /// <summary>
        ///   Gets ors sets whether to use Laplace's rule
        ///   of succession to avoid zero probabilities.
        /// </summary>
        /// 
        public bool UseLaplaceRule { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GeneralDiscreteOptions"/> class.
        /// </summary>
        /// 
        public GeneralDiscreteOptions()
        {
            Minimum = 1e-10;
            UseLaplaceRule = false;
        }

    }
}
