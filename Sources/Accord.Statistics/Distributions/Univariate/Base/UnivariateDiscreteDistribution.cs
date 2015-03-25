﻿// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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

namespace Accord.Statistics.Distributions.Univariate
{
    using System;
    using Accord.Math;
    using Accord.Statistics.Distributions.Fitting;
    using AForge;
    using Accord.Math.Optimization;

    /// <summary>
    ///   Abstract class for univariate discrete probability distributions.
    /// </summary>
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
    /// <seealso cref="BernoulliDistribution"/>
    /// <seealso cref="GeometricDistribution"/>
    /// <seealso cref="PoissonDistribution"/>
    /// 
    [Serializable]
    public abstract class UnivariateDiscreteDistribution : DistributionBase,
        IUnivariateDistribution<int>, IUnivariateDistribution,
        IUnivariateDistribution<double>, IDistribution<double[]>,
        IDistribution<double>, ISampleableDistribution<double>, ISampleableDistribution<int>,
        IFormattable
    {

        double? median;
        double? stdDev;
        DoubleRange? quartiles;


        /// <summary>
        ///   Constructs a new UnivariateDistribution class.
        /// </summary>
        /// 
        protected UnivariateDiscreteDistribution()
        {
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean value.</value>
        /// 
        public abstract double Mean { get; }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        public abstract double Variance { get; }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's entropy.</value>
        /// 
        public abstract double Entropy { get; }

        /// <summary>
        ///   Gets the support interval for this distribution.
        /// </summary>
        /// 
        /// <value>A <see cref="AForge.IntRange"/> containing
        ///  the support interval for this distribution.</value>
        ///  
        public abstract IntRange Support { get; }

        /// <summary>
        ///   Gets the mode for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mode value.</value>
        /// 
        public virtual double Mode
        {
            get { return Mean; }
        }

        /// <summary>
        ///   Gets the median for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's median value.</value>
        /// 
        public virtual double Median
        {
            get
            {
                if (median == null)
                    median = InverseDistributionFunction(0.5);

                return median.Value;
            }
        }

        /// <summary>
        ///   Gets the Standard Deviation (the square root of
        ///   the variance) for the current distribution.
        /// </summary>
        /// 
        /// <value>The distribution's standard deviation.</value>
        /// 
        public virtual double StandardDeviation
        {
            get
            {
                if (!stdDev.HasValue)
                    stdDev = Math.Sqrt(this.Variance);
                return stdDev.Value;
            }
        }


        /// <summary>
        ///   Gets the Quartiles for this distribution.
        /// </summary>
        /// 
        /// <value>A <see cref="DoubleRange"/> object containing the first quartile
        /// (Q1) as its minimum value, and the third quartile (Q2) as the maximum.</value>
        /// 
        public virtual DoubleRange Quartiles
        {
            get
            {
                if (quartiles == null)
                {
                    double min = InverseDistributionFunction(0.25);
                    double max = InverseDistributionFunction(0.75);
                    quartiles = new DoubleRange(min, max);
                }

                return quartiles.Value;
            }
        }

        /// <summary>
        ///   Gets the distribution range within a given percentile.
        /// </summary>
        /// 
        /// <remarks>
        ///   If <c>0.25</c> is passed as the <paramref name="percentile"/> argument, 
        ///   this function returns the same as the <see cref="Quartiles"/> function.
        /// </remarks>
        /// 
        /// <param name="percentile">
        ///   The percentile at which the distribution ranges will be returned.</param>
        /// 
        /// <value>A <see cref="DoubleRange"/> object containing the minimum value
        /// for the distribution value, and the third quartile (Q2) as the maximum.</value>
        /// 
        public virtual IntRange GetRange(double percentile)
        {
            if (percentile <= 0 || percentile >= 1)
                throw new ArgumentOutOfRangeException("percentile", "The percentile must be between 0 and 1.");

            int a = InverseDistributionFunction(1.0 - percentile);
            int b = InverseDistributionFunction(percentile);

            if (b > a)
                return new IntRange(a, b);
            return new IntRange(b, a);
        }

        #region IDistribution explicit members

        /// <summary>
        ///   Gets the support interval for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   A <see cref="AForge.DoubleRange" /> containing 
        ///   the support interval for this distribution.
        /// </value>
        /// 
        DoubleRange IUnivariateDistribution.Support
        {
            get { return new DoubleRange(Support.Min, Support.Max); }
        }

        /// <summary>
        ///   Gets the support interval for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   A <see cref="AForge.DoubleRange" /> containing 
        ///   the support interval for this distribution.
        /// </value>
        /// 
        DoubleRange IUnivariateDistribution<int>.Support
        {
            get { return new DoubleRange(Support.Min, Support.Max); }
        }

        /// <summary>
        ///   Gets the support interval for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   A <see cref="AForge.DoubleRange" /> containing 
        ///   the support interval for this distribution.
        /// </value>
        /// 
        DoubleRange IUnivariateDistribution<double>.Support
        {
            get { return new DoubleRange(Support.Min, Support.Max); }
        }

        /// <summary>
        ///   Gets the distribution range within a given percentile.
        /// </summary>
        /// 
        /// <remarks>
        ///   If <c>0.25</c> is passed as the <paramref name="percentile"/> argument, 
        ///   this function returns the same as the <see cref="Quartiles"/> function.
        /// </remarks>
        /// 
        /// <param name="percentile">
        ///   The percentile at which the distribution ranges will be returned.</param>
        /// 
        /// <value>A <see cref="DoubleRange"/> object containing the minimum value
        /// for the distribution value, and the third quartile (Q2) as the maximum.</value>
        /// 
        DoubleRange IUnivariateDistribution.GetRange(double percentile)
        {
            IntRange range = GetRange(percentile);
            return new DoubleRange(range.Min, range.Max);
        }

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
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.</returns>
        ///   
        double IDistribution.DistributionFunction(double[] x)
        {
            if (double.IsNegativeInfinity(x[0]))
                return DistributionFunction(int.MinValue);
            else if (double.IsPositiveInfinity(x[0]))
                return DistributionFunction(int.MaxValue);

            return DistributionFunction((int)x[0]);
        }

        double IUnivariateDistribution.DistributionFunction(double x)
        {
            if (double.IsNegativeInfinity(x))
                return DistributionFunction(int.MinValue);
            else if (double.IsPositiveInfinity(x))
                return DistributionFunction(int.MaxValue);

            return DistributionFunction((int)x);
        }

        double IUnivariateDistribution.DistributionFunction(double a, double b)
        {
            int ia;
            if (double.IsNegativeInfinity(a))
                ia = int.MinValue;
            else if (double.IsPositiveInfinity(a))
                ia = int.MaxValue;
            else ia = (int)a;

            int ib;
            if (double.IsNegativeInfinity(b))
                ib = int.MinValue;
            else if (double.IsPositiveInfinity(b))
                ib = int.MaxValue;
            else ib = (int)b;

            return DistributionFunction(ia, ib);
        }

        double IUnivariateDistribution.InverseDistributionFunction(double p)
        {
            return InverseDistributionFunction(p);
        }

        double IDistribution.ComplementaryDistributionFunction(double[] x)
        {
            if (double.IsNegativeInfinity(x[0]))
                return ComplementaryDistributionFunction(int.MinValue);
            else if (double.IsPositiveInfinity(x[0]))
                return ComplementaryDistributionFunction(int.MaxValue);

            return ComplementaryDistributionFunction((int)x[0]);
        }

        double IDistribution.ProbabilityFunction(double[] x)
        {
            if (double.IsNegativeInfinity(x[0]))
                return ProbabilityMassFunction(int.MinValue);
            else if (double.IsPositiveInfinity(x[0]))
                return ProbabilityMassFunction(int.MaxValue);

            return ProbabilityMassFunction((int)x[0]);
        }

        double IDistribution.LogProbabilityFunction(double[] x)
        {
            return LogProbabilityMassFunction((int)x[0]);
        }

        double IUnivariateDistribution.ProbabilityFunction(double x)
        {
            return ProbabilityMassFunction((int)x);
        }

        double IUnivariateDistribution.LogProbabilityFunction(double x)
        {
            return LogProbabilityMassFunction((int)x);
        }

        double IUnivariateDistribution.ComplementaryDistributionFunction(double x)
        {
            return ComplementaryDistributionFunction((int)x);
        }

        double IUnivariateDistribution.HazardFunction(double x)
        {
            return HazardFunction((int)x);
        }

        double IUnivariateDistribution.CumulativeHazardFunction(double x)
        {
            return CumulativeHazardFunction((int)x);
        }

        void IDistribution.Fit(Array observations)
        {
            (this as IDistribution).Fit(observations, (IFittingOptions)null);
        }

        void IDistribution.Fit(Array observations, double[] weights)
        {
            (this as IDistribution).Fit(observations, weights, (IFittingOptions)null);
        }

        void IDistribution.Fit(Array observations, int[] weights)
        {
            (this as IDistribution).Fit(observations, weights, (IFittingOptions)null);
        }

        void IDistribution.Fit(Array observations, IFittingOptions options)
        {
            (this as IDistribution).Fit(observations, (double[])null, options);
        }

        void IDistribution.Fit(Array observations, double[] weights, IFittingOptions options)
        {
            double[] univariate = observations as double[];
            if (univariate != null)
            {
                Fit(univariate, weights, options);
                return;
            }

            double[][] multivariate = observations as double[][];
            if (multivariate != null)
            {
                Fit(Matrix.Concatenate(multivariate), weights, options);
                return;
            }

            throw new ArgumentException("Invalid input type.", "observations");
        }

        void IDistribution.Fit(Array observations, int[] weights, IFittingOptions options)
        {
            double[] univariate = observations as double[];
            if (univariate != null)
            {
                Fit(univariate, weights, options);
                return;
            }

            double[][] multivariate = observations as double[][];
            if (multivariate != null)
            {
                Fit(Matrix.Concatenate(multivariate), weights, options);
                return;
            }

            throw new ArgumentException("Invalid input type.", "observations");
        }
        #endregion


        /// <summary>
        ///   Gets P(X ≤ k), the cumulative distribution function
        ///   (cdf) for this distribution evaluated at point <c>k</c>.
        /// </summary>
        /// 
        /// <param name="k">
        ///   A single point in the distribution range.</param>
        ///   
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// 
        public abstract double DistributionFunction(int k);

        /// <summary>
        ///   Gets P(X ≤ k) or P(X &lt; k), the cumulative distribution function
        ///   (cdf) for this distribution evaluated at point <c>k</c>, depending
        ///   on the value of the <paramref name="inclusive"/> parameter.
        /// </summary>
        /// 
        /// <param name="k">
        ///   A single point in the distribution range.</param>
        /// <param name="inclusive">
        ///   True to return P(X ≤ x), false to return P(X &lt; x). Default is true.</param>
        ///   
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// 
        public virtual double DistributionFunction(int k, bool inclusive)
        {
            if (inclusive)
                return DistributionFunction(k);
            else
                return DistributionFunction(k) - ProbabilityMassFunction(k);
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for this
        ///   distribution in the semi-closed interval (a; b] given as
        ///   <c>P(a &lt; X ≤ b)</c>.
        /// </summary>
        /// 
        /// <param name="a">The start of the semi-closed interval (a; b].</param>
        /// <param name="b">The end of the semi-closed interval (a; b].</param>
        /// 
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// 
        public virtual double DistributionFunction(int a, int b)
        {
            if (a >= b)
            {
                throw new ArgumentOutOfRangeException("b",
                    "The start of the interval a must be smaller than b.");
            }

            return DistributionFunction(b, inclusive: true) - DistributionFunction(a);
        }

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
        ///   value when applied in the <see cref="DistributionFunction(int)"/>.</returns>
        /// 
        public virtual int InverseDistributionFunction(double p)
        {
            bool lowerBounded = !Double.IsInfinity(Support.Min);
            bool upperBounded = !Double.IsInfinity(Support.Max);

            if (lowerBounded && upperBounded)
            {
                return new BinarySearch(DistributionFunction, (int)Support.Min, (int)Support.Max).Find(p);
            }

            try
            {
                checked
                {
                    if (lowerBounded && !upperBounded)
                    {
                        int lower = (int)Support.Min;
                        int upper = lower + 1;

                        double f = DistributionFunction(lower);

                        if (f > p)
                        {
                            while (f > p)
                            {
                                upper = 2 * upper;
                                f = DistributionFunction(upper);
                            }
                        }
                        else
                        {
                            while (f < p)
                            {
                                upper = 2 * upper;
                                f = DistributionFunction(upper);
                            }
                        }

                        return new BinarySearch(DistributionFunction, lower, upper).Find(p);
                    }

                    if (!lowerBounded && upperBounded)
                    {
                        int upper = (int)Support.Max;
                        int lower = upper - 1;

                        double f = DistributionFunction(upper);

                        if (f > p)
                        {
                            while (f > p)
                            {
                                lower = lower - 2 * lower;
                                f = DistributionFunction(lower);
                            }
                        }
                        else
                        {
                            while (f < p)
                            {
                                lower = lower - 2 * lower;
                                f = DistributionFunction(lower);
                            }
                        }

                        return new BinarySearch(DistributionFunction, lower, upper).Find(p);
                    }

                    // completely unbounded
                    {
                        int lower = -1;
                        int upper = +1;

                        double f = DistributionFunction(0);

                        if (f > p)
                        {
                            while (f > p)
                            {
                                upper = lower;
                                lower = 2 * lower - 1;
                                f = DistributionFunction(lower);
                            }
                        }
                        else
                        {
                            while (f < p)
                            {
                                lower = upper;
                                upper = 2 * upper + 1;
                                f = DistributionFunction(upper);
                            }
                        }

                        return new BinarySearch(DistributionFunction, lower, upper).Find(p);
                    }
                }
            }
            catch (OverflowException)
            {
                return 0;
            }
        }

        /// <summary>
        ///   Gets the first derivative of the <see cref="InverseDistributionFunction">
        ///   inverse distribution function</see> (icdf) for this distribution evaluated
        ///   at probability <c>p</c>. 
        /// </summary>
        /// 
        /// <param name="p">A probability value between 0 and 1.</param>
        /// 
        public virtual double QuantileDensityFunction(double p)
        {
            return 1.0 / ProbabilityMassFunction(InverseDistributionFunction(p));
        }

        /// <summary>
        ///   Gets the complementary cumulative distribution function
        ///   (ccdf) for this distribution evaluated at point <c>k</c>.
        ///   This function is also known as the Survival function.
        /// </summary>
        /// 
        /// <param name="k">
        ///   A single point in the distribution range.</param>
        /// <param name="inclusive">
        ///   True to return P(X &gt;= x), false to return P(X &gt; x). Default is false.</param>
        ///
        /// <remarks>
        ///   The Complementary Cumulative Distribution Function (CCDF) is
        ///   the complement of the Cumulative Distribution Function, or 1
        ///   minus the CDF.
        /// </remarks>
        /// 
        public virtual double ComplementaryDistributionFunction(int k, bool inclusive)
        {
            if (inclusive)
                return ComplementaryDistributionFunction(k) + ProbabilityMassFunction(k);
            else
                return ComplementaryDistributionFunction(k);
        }

        /// <summary>
        ///   Gets P(X &gt; k) the complementary cumulative distribution function
        ///   (ccdf) for this distribution evaluated at point <c>k</c>.
        ///   This function is also known as the Survival function.
        /// </summary>
        /// 
        /// <param name="k">
        ///   A single point in the distribution range.</param>
        ///   
        /// <remarks>
        ///   The Complementary Cumulative Distribution Function (CCDF) is
        ///   the complement of the Cumulative Distribution Function, or 1
        ///   minus the CDF.
        /// </remarks>
        /// 
        public virtual double ComplementaryDistributionFunction(int k)
        {
            return 1.0 - DistributionFunction(k);
        }

        /// <summary>
        ///   Gets the probability mass function (pmf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="k">
        ///   A single point in the distribution range.</param>
        ///   
        /// <remarks>
        ///   The Probability Mass Function (PMF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        /// <returns>
        ///   The probability of <c>k</c> occurring
        ///   in the current distribution.</returns>
        ///   
        public abstract double ProbabilityMassFunction(int k);

        /// <summary>
        ///   Gets the log-probability mass function (pmf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="k">
        ///   A single point in the distribution range.</param>
        ///   
        /// <remarks>
        ///   The Probability Mass Function (PMF) describes the
        ///   probability that a given value <c>k</c> will occur.
        /// </remarks>
        /// 
        /// <returns>
        ///   The logarithm of the probability of <c>x</c>
        ///   occurring in the current distribution.</returns>
        ///   
        public virtual double LogProbabilityMassFunction(int k)
        {
            return Math.Log(ProbabilityMassFunction(k));
        }

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
        public virtual double HazardFunction(int x)
        {
            return ProbabilityMassFunction(x) / ComplementaryDistributionFunction(x);
        }

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
        public virtual double CumulativeHazardFunction(int x)
        {
            return -Math.Log(ComplementaryDistributionFunction(x));
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
        public virtual void Fit(double[] observations)
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
        ///   
        /// <param name="weights">
        ///   The weight vector containing the weight for each of the samples.</param>
        ///   
        public virtual void Fit(double[] observations, double[] weights)
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
        ///   
        /// <param name="weights">
        ///   The weight vector containing the weight for each of the samples.</param>
        ///   
        public virtual void Fit(double[] observations, int[] weights)
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
        ///   
        /// <param name="options">
        ///   Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        ///   
        public virtual void Fit(double[] observations, IFittingOptions options)
        {
            Fit(observations, (double[])null, options);
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
        /// 
        /// <param name="weights">
        ///   The weight vector containing the weight for each of the samples.</param>
        ///   
        /// <param name="options">
        ///   Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        ///   
        public virtual void Fit(double[] observations, double[] weights, IFittingOptions options)
        {
            throw new NotSupportedException();
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
        /// 
        /// <param name="weights">
        ///   The weight vector containing the weight for each of the samples.</param>
        ///   
        /// <param name="options">
        ///   Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        ///   
        public virtual void Fit(double[] observations, int[] weights, IFittingOptions options)
        {
            if (weights == null)
            {
                Fit(observations, (double[])null, options);
            }
            else
            {
                throw new NotSupportedException();
            }
        }


        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public virtual int[] Generate(int samples)
        {
            var random = Accord.Math.Tools.Random;

            int[] s = new int[samples];

            for (int i = 0; i < s.Length; i++)
            {
                double u = random.NextDouble();
                s[i] = InverseDistributionFunction(u);
            }

            return s;
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public virtual int Generate()
        {
            return InverseDistributionFunction(Accord.Math.Tools.Random.NextDouble());
        }

        double[] ISampleableDistribution<double>.Generate(int samples)
        {
            return Generate(samples).ToDouble();
        }

        double ISampleableDistribution<double>.Generate()
        {
            return (double)Generate();
        }


        double IDistribution<int>.ProbabilityFunction(int x)
        {
            return ProbabilityMassFunction(x);
        }

        double IDistribution<int>.LogProbabilityFunction(int x)
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

        double IDistribution<double>.DistributionFunction(double x)
        {
            return DistributionFunction((int)x);
        }

        double IDistribution<double>.ProbabilityFunction(double x)
        {
            return ProbabilityMassFunction((int)x);
        }

        double IDistribution<double>.LogProbabilityFunction(double x)
        {
            return LogProbabilityMassFunction((int)x);
        }

        double IDistribution<double>.ComplementaryDistributionFunction(double x)
        {
            return ComplementaryDistributionFunction((int)x);
        }

        double IUnivariateDistribution<double>.InverseDistributionFunction(double p)
        {
            return InverseDistributionFunction(p);
        }

        double IUnivariateDistribution<double>.HazardFunction(double x)
        {
            return HazardFunction((int)x);
        }

        double IUnivariateDistribution<double>.CumulativeHazardFunction(double x)
        {
            return CumulativeHazardFunction((int)x);
        }

    }
}