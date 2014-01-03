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

namespace Accord.Statistics.Distributions
{
    using System;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Univariate;
    using AForge;

    /// <summary>
    ///   Common interface for univariate probability distributions.
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>
    ///   This interface is implemented by both univariate <see cref="UnivariateDiscreteDistribution">
    ///   Discrete Distributions</see> and <see cref="UnivariateContinuousDistribution">Continuous 
    ///   Distributions</see>.</para>
    ///   
    ///   <para>
    ///   For Multivariate distributions, see <see cref="IMultivariateDistribution"/>.</para>
    /// </remarks>
    /// 
    /// <seealso cref="NormalDistribution"/>
    /// <seealso cref="GammaDistribution"/>
    /// <seealso cref="UniformContinuousDistribution"/>
    /// <seealso cref="UniformDiscreteDistribution"/>
    /// 
    public interface IUnivariateDistribution : IDistribution
    {

        /// <summary>
        ///   Gets the mean value for the distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean.</value>
        /// 
        double Mean { get; }

        /// <summary>
        ///   Gets the variance value for the distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        double Variance { get; }

        /// <summary>
        ///   Gets the median value for the distribution.
        /// </summary>
        /// 
        /// <value>The distribution's median.</value>
        /// 
        double Median { get; }

        /// <summary>
        ///   Gets the mode value for the distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mode.</value>
        /// 
        double Mode { get; }

        /// <summary>
        ///   Gets entropy of the distribution.
        /// </summary>
        /// 
        /// <value>The distribution's entropy.</value>
        /// 
        double Entropy { get; }

        /// <summary>
        ///   Gets the support interval for this distribution.
        /// </summary>
        /// 
        /// <value>A <see cref="AForge.DoubleRange"/> containing
        ///  the support interval for this distribution.</value>
        ///  
        DoubleRange Support { get; }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// 
        double DistributionFunction(double x);

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range. For a 
        ///   univariate distribution, this should be a single
        ///   double value. For a multivariate distribution,
        ///   this should be a double array.</param>
        ///   
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.</returns>
        ///   
        double ProbabilityFunction(double x);

        /// <summary>
        ///   Gets the log-probability density function (pdf)
        ///   for this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range. For a 
        ///   univariate distribution, this should be a single
        ///   double value. For a multivariate distribution,
        ///   this should be a double array.</param>
        /// <returns>
        ///   The logarithm of the probability of <c>x</c>
        ///   occurring in the current distribution.</returns>
        ///   
        double LogProbabilityFunction(double x);

        /// <summary>
        ///   Gets the inverse of the cumulative distribution function (icdf) for
        ///   this distribution evaluated at probability <c>p</c>. This function 
        ///   is also known as the Quantile function.
        /// </summary>
        /// 
        /// <remarks>
        ///   The Inverse Cumulative Distribution Function (ICDF) specifies, for
        ///   a given probability, the value which the random variable will be at,
        ///   or below, with that probability.
        /// </remarks>
        /// 
        /// <param name="p">A probability value between 0 and 1.</param>
        /// 
        /// <returns>A sample which could original the given probability 
        ///   value when applied in the <see cref="DistributionFunction"/>.</returns>
        /// 
        double InverseDistributionFunction(double p);

         /// <summary>
        ///   Gets the complementary cumulative distribution function
        ///   (ccdf) for this distribution evaluated at point <c>x</c>.
        ///   This function is also known as the Survival function.
        /// </summary>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range.</param>
        ///   
        /// <remarks>
        ///   The Complementary Cumulative Distribution Function (CCDF) is
        ///   the complement of the Cumulative Distribution Function, or 1
        ///   minus the CDF.
        /// </remarks>
        /// 
        double ComplementaryDistributionFunction(double x);

        /// <summary>
        ///   Gets the hazard function, also known as the failure rate or
        ///   the conditional failure density function for this distribution
        ///   evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <remarks>
        ///   The hazard function is the ratio of the probability
        ///   density function f(x) to the survival function, S(x).
        /// </remarks>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range.</param>
        ///   
        /// <returns>
        ///   The conditional failure density function <c>h(x)</c>
        ///   evaluated at <c>x</c> in the current distribution.</returns>
        /// 
        double HazardFunction(double x);

        /// <summary>
        ///   Gets the cumulative hazard function for this
        ///   distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The cumulative hazard function <c>H(x)</c>  
        ///   evaluated at <c>x</c> in the current distribution.</returns>
        /// 
        double CumulativeHazardFunction(double x);
        
    }

    /// <summary>
    ///   Common interface for univariate probability distributions.
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>
    ///   This interface is implemented by both univariate <see cref="UnivariateDiscreteDistribution">
    ///   Discrete Distributions</see> and <see cref="UnivariateContinuousDistribution">Continuous 
    ///   Distributions</see>. However, unlike <see cref="IUnivariateDistribution"/>, this interface
    ///   has a generic parameter that allows to define the type of the distribution values (i.e.
    ///   <see cref="T:double"/>).</para>
    ///   
    ///   <para>
    ///   For Multivariate distributions, see <see cref="IMultivariateDistribution{T}"/>.</para>
    /// </remarks>
    /// 
    /// <seealso cref="NormalDistribution"/>
    /// <seealso cref="GammaDistribution"/>
    /// <seealso cref="UniformContinuousDistribution"/>
    /// <seealso cref="UniformDiscreteDistribution"/>
    /// 
    public interface IUnivariateDistribution<TObservation> : IDistribution<TObservation>
    {

        /// <summary>
        ///   Gets the mean value for the distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean.</value>
        /// 
        double Mean { get; }

        /// <summary>
        ///   Gets the variance value for the distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        double Variance { get; }

        /// <summary>
        ///   Gets the median value for the distribution.
        /// </summary>
        /// 
        /// <value>The distribution's median.</value>
        /// 
        double Median { get; }

        /// <summary>
        ///   Gets the mode value for the distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mode.</value>
        /// 
        double Mode { get; }

        /// <summary>
        ///   Gets entropy of the distribution.
        /// </summary>
        /// 
        /// <value>The distribution's entropy.</value>
        /// 
        double Entropy { get; }

        /// <summary>
        ///   Gets the support interval for this distribution.
        /// </summary>
        /// 
        /// <value>A <see cref="AForge.DoubleRange"/> containing
        ///  the support interval for this distribution.</value>
        ///  
        DoubleRange Support { get; }

        /// <summary>
        ///   Gets the inverse of the cumulative distribution function (icdf) for
        ///   this distribution evaluated at probability <c>p</c>. This function 
        ///   is also known as the Quantile function.
        /// </summary>
        /// 
        /// <remarks>
        ///   The Inverse Cumulative Distribution Function (ICDF) specifies, for
        ///   a given probability, the value which the random variable will be at,
        ///   or below, with that probability.
        /// </remarks>
        /// 
        /// <param name="p">A probability value between 0 and 1.</param>
        /// 
        /// <returns>A sample which could original the given probability 
        ///   value when applied in the <see cref="IDistribution{T}.DistributionFunction"/>.</returns>
        /// 
        TObservation InverseDistributionFunction(double p);

        /// <summary>
        ///   Gets the hazard function, also known as the failure rate or
        ///   the conditional failure density function for this distribution
        ///   evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <remarks>
        ///   The hazard function is the ratio of the probability
        ///   density function f(x) to the survival function, S(x).
        /// </remarks>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range.</param>
        ///   
        /// <returns>
        ///   The conditional failure density function <c>h(x)</c>
        ///   evaluated at <c>x</c> in the current distribution.</returns>
        /// 
        double HazardFunction(TObservation x);

        /// <summary>
        ///   Gets the cumulative hazard function for this
        ///   distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The cumulative hazard function <c>H(x)</c>  
        ///   evaluated at <c>x</c> in the current distribution.</returns>
        /// 
        double CumulativeHazardFunction(TObservation x);

    }
}
