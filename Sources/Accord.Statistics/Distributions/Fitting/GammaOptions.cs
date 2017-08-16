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
    ///   Estimation options for <see cref="Accord.Statistics.Distributions.Univariate.GammaDistribution">
    ///   Gamma distributions</see>.
    /// </summary>
    /// 
    [Serializable]
    public class GammaOptions : IFittingOptions
    {
        /// <summary>
        ///   Gets or sets the relative tolerance when
        ///   iteratively estimating the distribution. 
        ///   Default is 1e-8.
        /// </summary>
        /// 
        /// <value>The relative tolerance value.</value>
        /// 
        public double Tolerance { get; set; }

        /// <summary>
        ///   Gets or sets the maximum number of iterations to
        ///   attempt when estimating the Gamma distribution.
        ///   Default is 1000.
        /// </summary>
        /// 
        public int Iterations { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GammaOptions"/> class.
        /// </summary>
        /// 
        public GammaOptions()
        {
            Tolerance = 1e-8;
            Iterations = 1000;
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
