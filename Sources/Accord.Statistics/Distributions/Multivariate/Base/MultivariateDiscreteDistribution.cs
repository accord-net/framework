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
    using Accord.Math;
    using Accord.Statistics.Distributions.Fitting;

    /// <summary>
    ///   Abstract class for multivariate discrete probability distributions.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   A probability distribution identifies either the probability of each value of an
    ///   unidentified random variable (when the variable is discrete), or the probability
    ///   of the value falling within a particular interval (when the variable is continuous).</para>
    /// <para>
    ///   The probability distribution describes the range of possible values that a random
    ///   variable can attain and the probability that the value of the random variable is
    ///   within any (measurable) subset of that range.</para>  
    /// <para>
    ///   The function describing the probability that a given discrete value will
    ///   occur is called the probability function (or probability mass function,
    ///   abbreviated PMF), and the function describing the cumulative probability
    ///   that a given value or any value smaller than it will occur is called the
    ///   distribution function (or cumulative distribution function, abbreviated CDF).</para>  
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Probability_distribution">
    ///       Wikipedia, The Free Encyclopedia. Probability distribution. Available on:
    ///       http://en.wikipedia.org/wiki/Probability_distribution </a></description></item>
    ///     <item><description><a href="http://mathworld.wolfram.com/StatisticalDistribution.html">
    ///       Weisstein, Eric W. "Statistical Distribution." From MathWorld--A Wolfram Web Resource.
    ///       http://mathworld.wolfram.com/StatisticalDistribution.html </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    [Serializable]
    public abstract class MultivariateDiscreteDistribution : DistributionBase,
        IMultivariateDistribution, IMultivariateDistribution<int[]>,
        IDistribution<double[]>
    {

        private int dimension;

        /// <summary>
        ///   Constructs a new MultivariateDiscreteDistribution class.
        /// </summary>
        /// 
        protected MultivariateDiscreteDistribution(int dimension)
        {
            this.dimension = dimension;
        }

        /// <summary>
        ///   Gets the number of variables for this distribution.
        /// </summary>
        /// 
        public int Dimension { get { return dimension; } }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   An array of double-precision values containing
        ///   the mean values for this distribution.
        /// </value>
        /// 
        public abstract double[] Mean { get; }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   An array of double-precision values containing
        ///   the variance values for this distribution.
        /// </value>
        /// 
        public abstract double[] Variance { get; }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   An multidimensional array of double-precision values
        ///   containing the covariance values for this distribution.
        /// </value>
        /// 
        public abstract double[,] Covariance { get; }

        /// <summary>
        ///   Gets the mode for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   An array of double-precision values containing
        ///   the mode values for this distribution.
        /// </value>
        /// 
        public virtual double[] Mode
        {
            get { return Mean; }
        }

        /// <summary>
        ///   Gets the median for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   An array of double-precision values containing
        ///   the median values for this distribution.
        /// </value>
        /// 
        public virtual double[] Median
        {
            get { return Mean; }
        }

        #region IDistribution explicit members

        /// <summary>
        /// Gets the cumulative distribution function (cdf) for
        /// this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <remarks>
        /// The Cumulative Distribution Function (CDF) describes the cumulative
        /// probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// 
        double IDistribution.DistributionFunction(double[] x)
        {
            return DistributionFunction(Array.ConvertAll<double, int>(x, Convert.ToInt32));
        }

        /// <summary>
        ///   Gets the probability mass function (pmf) for
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
        ///   The Probability Mass Function (PMF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.</returns>
        ///   
        double IDistribution.ProbabilityFunction(double[] x)
        {
            return ProbabilityMassFunction(Array.ConvertAll<double, int>(x, Convert.ToInt32));
        }

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
        ///   
        /// <remarks>
        ///   The Probability Mass Function (PMF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// <returns>
        ///   The logarithm of the probability of <c>x</c>
        ///   occurring in the current distribution.
        /// </returns>
        ///   
        double IDistribution.LogProbabilityFunction(double[] x)
        {
            return LogProbabilityMassFunction(Array.ConvertAll<double, int>(x, Convert.ToInt32));
        }

        /// <summary>
        ///   Gets the complementary cumulative distribution function
        ///   (ccdf) for this distribution evaluated at point <c>x</c>.
        ///   This function is also known as the Survival function.
        /// </summary>
        /// 
        /// <remarks>
        ///   The Complementary Cumulative Distribution Function (CCDF) is
        ///   the complement of the Cumulative Distribution Function, or 1
        ///   minus the CDF.
        /// </remarks>
        /// 
        double IDistribution.ComplementaryDistributionFunction(double[] x)
        {
            return ComplementaryDistributionFunction(Array.ConvertAll<double, int>(x, Convert.ToInt32));
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">
        ///   The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <remarks>
        /// 
        ///   Although both double[] and double[][] arrays are supported,
        ///   providing a double[] for a multivariate distribution or a
        ///   double[][] for a univariate distribution may have a negative
        ///   impact in performance.
        /// </remarks>
        ///   
        void IDistribution.Fit(Array observations)
        {
            (this as IDistribution).Fit(observations, (IFittingOptions)null);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// <param name="observations">
        ///   The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">
        ///   The weight vector containing the weight for each of the samples.</param>
        /// <remarks>
        ///   Although both double[] and double[][] arrays are supported,
        ///   providing a double[] for a multivariate distribution or a
        ///   double[][] for a univariate distribution may have a negative
        ///   impact in performance.
        /// </remarks>
        /// 
        void IDistribution.Fit(Array observations, double[] weights)
        {
            (this as IDistribution).Fit(observations, weights, (IFittingOptions)null);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// <param name="observations">
        ///   The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="options">
        ///   Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        /// <remarks>
        ///   Although both double[] and double[][] arrays are supported,
        ///   providing a double[] for a multivariate distribution or a
        ///   double[][] for a univariate distribution may have a negative
        ///   impact in performance.
        /// </remarks>
        /// 
        void IDistribution.Fit(Array observations, IFittingOptions options)
        {
            double[] weights = new double[observations.Length];

            // Create equal weights for the observations
            double w = 1.0 / observations.Length;
            for (int i = 0; i < weights.Length; i++)
                weights[i] = w;

            (this as IDistribution).Fit(observations, weights, options);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// <param name="observations">
        ///   The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).
        /// </param>
        /// <param name="weights">
        ///   The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">
        ///   Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        /// <remarks>
        ///   Although both double[] and double[][] arrays are supported,
        ///   providing a double[] for a multivariate distribution or a
        ///   double[][] for a univariate distribution may have a negative
        ///   impact in performance.
        /// </remarks>
        /// 
        void IDistribution.Fit(Array observations, double[] weights, IFittingOptions options)
        {
            double[][] multivariate = observations as double[][];
            if (multivariate != null)
            {
                Fit(multivariate, weights, options);
                return;
            }

            double[] univariate = observations as double[];
            if (univariate != null)
            {
                Fit(univariate.Split(dimension), weights, options);
                return;
            }

            throw new ArgumentException("Unsupported parameter type.", "observations");
        }
        #endregion


        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range.</param>
        ///   
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// 
        public abstract double DistributionFunction(int[] x);

        /// <summary>
        ///   Gets the probability mass function (pmf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range.</param>
        ///   
        /// <remarks>
        ///   The Probability Mass Function (PMF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.</returns>
        ///   
        public abstract double ProbabilityMassFunction(int[] x);

        /// <summary>
        ///   Gets the log-probability mass function (pmf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range.</param>
        ///   
        /// <remarks>
        ///   The Probability Mass Function (PMF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        /// <returns>
        ///   The logarithm of the probability of <c>x</c> 
        ///   occurring in the current distribution.</returns>
        ///   
        public abstract double LogProbabilityMassFunction(int[] x);

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public virtual int[] InverseDistributionFunction(double p)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///   Gets the complementary cumulative distribution function
        ///   (ccdf) for this distribution evaluated at point <c>x</c>.
        ///   This function is also known as the Survival function.
        /// </summary>
        /// 
        /// <remarks>
        ///   The Complementary Cumulative Distribution Function (CCDF) is
        ///   the complement of the Cumulative Distribution Function, or 1
        ///   minus the CDF.
        /// </remarks>
        /// 
        public virtual double ComplementaryDistributionFunction(int[] x)
        {
            return 1.0 - DistributionFunction(x);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">
        ///   The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        ///   
        /// <remarks>
        ///   Although both double[] and double[][] arrays are supported,
        ///   providing a double[] for a multivariate distribution or a
        ///   double[][] for a univariate distribution may have a negative
        ///   impact in performance.
        /// </remarks>
        ///   
        public virtual void Fit(double[][] observations)
        {
            Fit(observations, (IFittingOptions)null);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">
        ///   The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">
        ///   The weight vector containing the weight for each of the samples.</param>
        ///   
        /// <remarks>
        ///   Although both double[] and double[][] arrays are supported,
        ///   providing a double[] for a multivariate distribution or a
        ///   double[][] for a univariate distribution may have a negative
        ///   impact in performance.
        /// </remarks>
        /// 
        public virtual void Fit(double[][] observations, double[] weights)
        {
            Fit(observations, weights, (IFittingOptions)null);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">
        ///   The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="options">
        ///   Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        ///   
        /// <remarks>
        ///   Although both double[] and double[][] arrays are supported,
        ///   providing a double[] for a multivariate distribution or a
        ///   double[][] for a univariate distribution may have a negative
        ///   impact in performance.
        /// </remarks>
        /// 
        public virtual void Fit(double[][] observations, IFittingOptions options)
        {
            double[] weights = new double[observations.Length];

            // Create equal weights for the observations
            double w = 1.0 / observations.Length;
            for (int i = 0; i < weights.Length; i++)
                weights[i] = w;

            Fit(observations, weights, options);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">
        ///   The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).
        /// </param>
        /// <param name="weights">
        ///   The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">
        ///   Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        ///   
        /// <remarks>
        ///   Although both double[] and double[][] arrays are supported,
        ///   providing a double[] for a multivariate distribution or a
        ///   double[][] for a univariate distribution may have a negative
        ///   impact in performance.
        /// </remarks>
        /// 
        public abstract void Fit(double[][] observations, double[] weights, IFittingOptions options);

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public abstract object Clone();


      
        double IDistribution<int[]>.ProbabilityFunction(int[] x)
        {
            return ProbabilityMassFunction(x);
        }

        double IDistribution<int[]>.LogProbabilityFunction(int[] x)
        {
            return LogProbabilityMassFunction(x);
        }



        double IDistribution<double[]>.DistributionFunction(double[] x)
        {
            return (this as IDistribution).DistributionFunction(x);
        }

        double IDistribution<double[]>.ProbabilityFunction(double[] x)
        {
            return (this as IDistribution).ProbabilityFunction(x);
        }

        double IDistribution<double[]>.LogProbabilityFunction(double[] x)
        {
            return (this as IDistribution).LogProbabilityFunction(x);
        }

        double IDistribution<double[]>.ComplementaryDistributionFunction(double[] x)
        {
            return (this as IDistribution).ComplementaryDistributionFunction(x);
        }

    }
}