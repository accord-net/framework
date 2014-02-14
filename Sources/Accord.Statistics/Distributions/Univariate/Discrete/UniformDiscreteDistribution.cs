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

namespace Accord.Statistics.Distributions.Univariate
{
    using System;
    using Accord.Math;
    using AForge;

    /// <summary>
    ///   Discrete uniform distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability theory and statistics, the discrete uniform distribution is a 
    ///   symmetric probability distribution whereby a finite number of values are equally
    ///   likely to be observed; every one of n values has equal probability 1/n. Another 
    ///   way of saying "discrete uniform distribution" would be "a known, finite number of
    ///   outcomes equally likely to happen".</para>
    ///
    /// <para>
    ///   A simple example of the discrete uniform distribution is throwing a fair die. 
    ///   The possible values are 1, 2, 3, 4, 5, 6, and each time the die is thrown the 
    ///   probability of a given score is 1/6. If two dice are thrown and their values 
    ///   added, the resulting distribution is no longer uniform since not all sums have 
    ///   equal probability.</para>
    ///   
    /// <para>
    ///   The discrete uniform distribution itself is inherently non-parametric. It is 
    ///   convenient, however, to represent its values generally by an integer interval
    ///   [a,b], so that a,b become the main parameters of the distribution (often one
    ///   simply considers the interval [1,n] with the single parameter n). </para>
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Uniform_distribution_(discrete)">
    ///       Wikipedia, The Free Encyclopedia. Uniform distribution (discrete). Available on:
    ///       http://en.wikipedia.org/wiki/Uniform_distribution_(discrete) </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///   // Create an uniform (discrete) distribution in [2, 6] 
    ///   var dist = new UniformDiscreteDistribution(a: 2, b: 6);
    ///   
    ///   // Common measures
    ///   double mean = dist.Mean;     // 4.0
    ///   double median = dist.Median; // 4.0
    ///   double var = dist.Variance;  // 1.3333333333333333
    ///   
    ///   // Cumulative distribution functions
    ///   double cdf = dist.DistributionFunction(k: 2);               // 0.2
    ///   double ccdf = dist.ComplementaryDistributionFunction(k: 2); // 0.8
    ///   
    ///   // Probability mass functions
    ///   double pmf1 = dist.ProbabilityMassFunction(k: 4); // 0.2
    ///   double pmf2 = dist.ProbabilityMassFunction(k: 5); // 0.2
    ///   double pmf3 = dist.ProbabilityMassFunction(k: 6); // 0.2
    ///   double lpmf = dist.LogProbabilityMassFunction(k: 2); // -1.6094379124341003
    ///   
    ///   // Quantile function
    ///   int icdf1 = dist.InverseDistributionFunction(p: 0.17); // 2
    ///   int icdf2 = dist.InverseDistributionFunction(p: 0.46); // 4
    ///   int icdf3 = dist.InverseDistributionFunction(p: 0.87); // 6
    ///   
    ///   // Hazard (failure rate) functions
    ///   double hf = dist.HazardFunction(x: 4); // 0.5
    ///   double chf = dist.CumulativeHazardFunction(x: 4); // 0.916290731874155
    ///   
    ///   // String representation
    ///   string str = dist.ToString(CultureInfo.InvariantCulture); // "U(x; a = 2, b = 6)"
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class UniformDiscreteDistribution : UnivariateDiscreteDistribution
    {

        // Distribution parameters
        private int a;
        private int b;

        // Derived measures
        private int n;

        /// <summary>
        ///   Creates a discrete uniform distribution defined in the interval [a;b].
        /// </summary>
        /// 
        /// <param name="a">The starting (minimum) value a.</param>
        /// <param name="b">The ending (maximum) value b.</param>
        /// 
        public UniformDiscreteDistribution(int a, int b)
        {
            if (a > b)
                throw new ArgumentOutOfRangeException("b",
                    "The starting number a must be lower than b.");

            this.a = a;
            this.b = b;
            this.n = b - a + 1;
        }

        /// <summary>
        ///   Gets the minimum value of the distribution (a).
        /// </summary>
        /// 
        public double Minimum { get { return a; } }

        /// <summary>
        ///   Gets the maximum value of the distribution (b).
        /// </summary>
        /// 
        public double Maximum { get { return b; } }

        /// <summary>
        ///   Gets the length of the distribution (b - a + 1).
        /// </summary>
        /// 
        public double Length { get { return n; } }


        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        public override double Mean
        {
            get { return (a + b) / 2.0; }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        public override double Variance
        {
            get { return ((b - a) * (b - a)) / 12.0; }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        public override double Entropy
        {
            get { return Math.Log(b - a); }
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
        public override DoubleRange Support
        {
            get { return new DoubleRange(a, b); }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>k</c>.
        /// </summary>
        /// 
        /// <param name="k">A single point in the distribution range.</param>
        /// 
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// 
        public override double DistributionFunction(int k)
        {
            if (k < a)
                return 0;
            if (k >= b)
                return 1;
            return (k - a + 1.0) / n;
        }

        /// <summary>
        ///    Gets the probability mass function (pmf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="k">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The probability of <c>k</c> occurring
        ///   in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Mass Function (PMF) describes the
        ///   probability that a given value <c>k</c> will occur.
        /// </remarks>
        /// 
        public override double ProbabilityMassFunction(int k)
        {
            if (k >= a && k <= b)
                return 1.0 / n;
            else return 0;
        }

        /// <summary>
        /// Gets the log-probability mass function (pmf) for
        /// this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="k">A single point in the distribution range.</param>
        /// <returns>
        /// The logarithm of the probability of <c>k</c>
        /// occurring in the current distribution.
        /// </returns>
        /// <remarks>
        /// The Probability Mass Function (PMF) describes the
        /// probability that a given value <c>k</c> will occur.
        /// </remarks>
        public override double LogProbabilityMassFunction(int k)
        {
            if (k >= a && k <= b)
                return -Math.Log(n);
            else return double.NegativeInfinity;
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        /// elements can be either of type double (for univariate data) or
        /// type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, such
        /// as regularization constants and additional parameters.</param>
        /// 
        /// <remarks>
        ///   Although both double[] and double[][] arrays are supported,
        ///   providing a double[] for a multivariate distribution or a
        ///   double[][] for a univariate distribution may have a negative
        ///   impact in performance.
        /// </remarks>
        /// 
        public override void Fit(double[] observations, double[] weights, Fitting.IFittingOptions options)
        {
            if (options != null)
                throw new ArgumentException("No options may be specified.");

            if (weights != null)
                throw new ArgumentException("This distribution does not support weighted samples.");

            a = (int)observations.Min();
            b = (int)observations.Max();
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
            return new UniformDiscreteDistribution(a, b);
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public override string ToString()
        {
            return String.Format("U(x; a = {0}, b = {1})", a, b);
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public string ToString(IFormatProvider formatProvider)
        {
            return String.Format(formatProvider, "U(x; a = {0}, b = {1})", a, b);
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format(formatProvider, "U(x; a = {0}, b = {1})",
                a.ToString(format, formatProvider),
                b.ToString(format, formatProvider));
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public string ToString(string format)
        {
            return String.Format("U(x; a = {0}, b = {1})",
                a.ToString(format), b.ToString(format));
        }
    }
}
