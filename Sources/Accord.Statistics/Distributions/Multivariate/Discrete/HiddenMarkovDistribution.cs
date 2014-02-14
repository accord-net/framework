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

namespace Accord.Statistics.Distributions.Multivariate
{
    using System;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Models.Markov;

    /// <summary>
    ///   Hidden Markov Model probability distribution.
    /// </summary>
    /// 
    public class HiddenMarkovDistribution : MultivariateDiscreteDistribution
    {

        HiddenMarkovModel model;

        /// <summary>
        ///   Initializes a new instance of the <see cref="HiddenMarkovDistribution"/> class.
        /// </summary>
        /// 
        /// <param name="model">The model.</param>
        /// 
        public HiddenMarkovDistribution(HiddenMarkovModel model)
            : base(0)
        {
            this.model = model;
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   An array of double-precision values containing
        ///   the mean values for this distribution.
        /// </value>
        /// 
        public override double[] Mean
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   An array of double-precision values containing
        ///   the variance values for this distribution.
        /// </value>
        /// 
        public override double[] Variance
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   An multidimensional array of double-precision values
        ///   containing the covariance values for this distribution.
        /// </value>
        /// 
        public override double[,] Covariance
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// 
        public override double DistributionFunction(int[] x)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   Gets the probability mass function (pmf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Mass Function (PMF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        public override double ProbabilityMassFunction(int[] x)
        {
            return Math.Exp(model.Evaluate(x));
        }

        /// <summary>
        ///   Gets the log-probability mass function (pmf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The logarithm of the probability of <c>x</c>
        ///   occurring in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Mass Function (PMF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        public override double LogProbabilityMassFunction(int[] x)
        {
            return model.Evaluate(x);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        ///   
        /// <remarks>
        ///   Although both double[] and double[][] arrays are supported,
        ///   providing a double[] for a multivariate distribution or a
        ///   double[][] for a univariate distribution may have a negative
        ///   impact in performance.
        /// </remarks>
        /// 
        public override void Fit(double[][] observations, double[] weights, Fitting.IFittingOptions options)
        {
            HiddenMarkovOptions normalOptions = options as HiddenMarkovOptions;
            if (options != null && normalOptions == null)
                throw new ArgumentException("The specified options' type is invalid.", "options");

            Fit(observations, weights, normalOptions);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        ///   
        /// <remarks>
        ///   Although both double[] and double[][] arrays are supported,
        ///   providing a double[] for a multivariate distribution or a
        ///   double[][] for a univariate distribution may have a negative
        ///   impact in performance.
        /// </remarks>
        /// 
        public void Fit(double[][] observations, double[] weights, HiddenMarkovOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            options.Learning(model, observations, weights);
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public override object Clone()
        {
            return model.Clone();
        }
    }
}
