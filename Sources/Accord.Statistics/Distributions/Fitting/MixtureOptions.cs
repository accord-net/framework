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
    ///   Estimation options for <see cref="Accord.Statistics.Distributions.Univariate.Mixture{T}">univariate</see>
    ///   and <see cref="Accord.Statistics.Distributions.Multivariate.MultivariateMixture{T}">multivariate</see> 
    ///   <see cref="Accord.Statistics.Distributions.IMixture{T}">mixture distributions</see>.
    /// </summary>
    /// 
    [Serializable]
    public class MixtureOptions : IFittingOptions
    {

        /// <summary>
        ///   Gets or sets the convergence criterion for the
        ///   Expectation-Maximization algorithm. Default is 1e-3.
        /// </summary>
        /// 
        /// <value>The convergence threshold.</value>
        /// 
        public double Threshold { get; set; }

        /// <summary>
        ///   Gets or sets the maximum number of iterations
        ///   to be performed by the Expectation-Maximization
        ///   algorithm. Default is zero (iterate until convergence).
        /// </summary>
        /// 
        public int Iterations { get; set; }

        /// <summary>
        ///   Gets or sets the fitting options for the inner
        ///   component distributions of the mixture density.
        /// </summary>
        /// 
        /// <value>The fitting options for inner distributions.</value>
        /// 
        public IFittingOptions InnerOptions { get; set; }

        /// <summary>
        ///   Gets or sets whether to make computations using the log
        ///   -domain. This might improve accuracy on large datasets.
        /// </summary>
        /// 
        public bool Logarithm { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MixtureOptions"/> class.
        /// </summary>
        /// 
        public MixtureOptions()
        {
            Threshold = 1e-3;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MixtureOptions"/> class.
        /// </summary>
        /// 
        /// <param name="threshold">The convergence criterion for the
        ///   Expectation-Maximization algorithm. Default is 1e-3.</param>
        ///   
        public MixtureOptions(double threshold)
        {
            Threshold = threshold;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MixtureOptions"/> class.
        /// </summary>
        /// 
        /// <param name="threshold">The convergence criterion for the
        ///   Expectation-Maximization algorithm. Default is 1e-3.</param>
        /// <param name="innerOptions">The fitting options for the inner
        ///   component distributions of the mixture density.</param>
        ///   
        public MixtureOptions(double threshold, IFittingOptions innerOptions)
        {
            Threshold = threshold;
            InnerOptions = innerOptions;
        }
    }
}
