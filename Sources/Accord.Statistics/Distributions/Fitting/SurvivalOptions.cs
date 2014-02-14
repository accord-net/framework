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
    using Accord.Statistics.Distributions.Univariate;

    /// <summary>
    ///   Options for Survival distributions.
    /// </summary>
    /// 
    public class SurvivalOptions : IFittingOptions
    {

        /// <summary>
        ///   Gets or sets the values for
        ///   the right-censoring variable.
        /// </summary>
        /// 
        public int[] Censor { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SurvivalOptions"/> class.
        /// </summary>
        /// 
        public SurvivalOptions()
        {
        }
    }

    /// <summary>
    ///   Options for Empirical Hazard distributions.
    /// </summary>
    /// 
    public class EmpiricalHazardOptions : SurvivalOptions
    {
        /// <summary>
        ///   Gets or sets the outputs of the hazards model.
        /// </summary>
        /// 
        public double[] Output { get; set; }

        /// <summary>
        ///   Gets or sets the estimator to be used.
        /// </summary>
        /// 
        public HazardEstimator Estimator { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="EmpiricalHazardOptions"/> class.
        /// </summary>
        /// 
        public EmpiricalHazardOptions()
        {
            Estimator = HazardEstimator.BreslowNelsonAalen;
        }
    }
}
