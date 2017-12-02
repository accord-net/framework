﻿// Accord Statistics Library
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

namespace Accord.Statistics.Distributions.Univariate
{
    using System;
    using Accord.Math;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Compat;

    /// <summary>
    ///    (Shifted) Geometric Distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class represents the shifted version of the Geometric distribution
    ///   with support on { 0, 1, 2, 3, ... }. This is the probability distribution
    ///   of the number Y = X − 1 of failures before the first success, supported
    ///   on the set { 0, 1, 2, 3, ... }.</para>
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Geometric_distribution">
    ///       Wikipedia, The Free Encyclopedia. Geometric distribution. Available on:
    ///       http://en.wikipedia.org/wiki/Geometric_distribution </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///   // Create a Geometric distribution with 42% success probability
    ///   var dist = new GeometricDistribution(probabilityOfSuccess: 0.42);
    ///   
    ///   // Common measures
    ///   double mean   = dist.Mean;     // 1.3809523809523812
    ///   double median = dist.Median;   // 1.0
    ///   double var    = dist.Variance; // 3.2879818594104315
    ///   double mode   = dist.Mode;     // 0.0
    ///   
    ///   // Cumulative distribution functions
    ///   double cdf = dist.DistributionFunction(k: 2);               // 0.80488799999999994
    ///   double ccdf = dist.ComplementaryDistributionFunction(k: 2); // 0.19511200000000006
    ///   
    ///   // Probability mass functions
    ///   double pdf1 = dist.ProbabilityMassFunction(k: 0); // 0.42
    ///   double pdf2 = dist.ProbabilityMassFunction(k: 1); // 0.2436
    ///   double pdf3 = dist.ProbabilityMassFunction(k: 2); // 0.141288
    ///   double lpdf = dist.LogProbabilityMassFunction(k: 2); // -1.956954918588067
    ///   
    ///   // Quantile functions
    ///   int icdf1 = dist.InverseDistributionFunction(p: 0.17); // 0
    ///   int icdf2 = dist.InverseDistributionFunction(p: 0.46); // 1
    ///   int icdf3 = dist.InverseDistributionFunction(p: 0.87); // 3
    ///   
    ///   // Hazard (failure rate) functions
    ///   double hf = dist.HazardFunction(x: 0); // 0.72413793103448265
    ///   double chf = dist.CumulativeHazardFunction(x: 0); // 0.54472717544167193
    ///   
    ///   // String representation
    ///   string str = dist.ToString(CultureInfo.InvariantCulture); // "Geometric(x; p = 0.42)"
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="HypergeometricDistribution"/>
    /// 
    [Serializable]
    public class GeometricDistribution : UnivariateDiscreteDistribution,
        IFittableDistribution<double, IFittingOptions>,
        ISampleableDistribution<int>
    {

        // Distribution parameters
        private double p;


        /// <summary>
        ///   Gets the success probability for the distribution.
        /// </summary>
        /// 
        public double ProbabilityOfSuccess
        {
            get { return p; }
        }

        /// <summary>
        ///   Creates a new (shifted) geometric distribution.
        /// </summary>
        /// 
        /// <param name="probabilityOfSuccess">The success probability.</param>
        /// 
        public GeometricDistribution([Unit] double probabilityOfSuccess)
        {
            if (probabilityOfSuccess < 0 || probabilityOfSuccess > 1)
                throw new ArgumentOutOfRangeException("probabilityOfSuccess",
                    "A probability must be between 0 and 1.");

            this.p = probabilityOfSuccess;
        }



        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean value.</value>
        /// 
        public override double Mean
        {
            get { return (1 - p) / p; }
        }

        /// <summary>
        ///   Gets the mode for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's mode value.
        /// </value>
        /// 
        public override double Mode
        {
            get { return 0; }
        }

        /// <summary>
        ///   Gets the median for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's median value.
        /// </value>
        /// 
        public override double Median
        {
            get
            {
                double median = Math.Ceiling(-1.0 / Math.Log(1 - p, 2)) - 1;
                Accord.Diagnostics.Debug.Assert(median == base.Median);
                return median;
            }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        public override double Variance
        {
            get { return (1 - p) / (p * p); }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's entropy.</value>
        /// 
        public override double Entropy
        {
            get { return (-(1 - p) * Math.Log(1 - p, 2) - p * Math.Log(p, 2)) / p; }
        }

        /// <summary>
        ///   Gets the support interval for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   A <see cref="IntRange" /> containing
        ///   the support interval for this distribution.
        /// </value>
        /// 
        public override IntRange Support
        {
            get { return new IntRange(0, Int32.MaxValue); }
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
        protected internal override double InnerDistributionFunction(int k)
        {
            return 1 - Math.Pow(1 - p, k + 1);
        }

        /// <summary>
        ///   Gets the probability mass function (pmf) for
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
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        protected internal override double InnerProbabilityMassFunction(int k)
        {
            return Math.Pow(1 - p, k) * p;
        }

        /// <summary>
        ///   Gets the log-probability mass function (pmf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="k">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The logarithm of the probability of <c>x</c>
        ///   occurring in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Mass Function (PMF) describes the
        ///   probability that a given value <c>k</c> will occur.
        /// </remarks>
        /// 
        protected internal override double InnerLogProbabilityMassFunction(int k)
        {
            return k * Math.Log(1.0 - p) + Math.Log(p);
        }

        /// <summary>
        ///   Gets the inverse of the cumulative distribution function (icdf) for
        ///   this distribution evaluated at probability <c>p</c>. This function
        ///   is also known as the Quantile function.
        /// </summary>
        /// 
        /// <param name="p">A probability value between 0 and 1.</param>
        /// 
        /// <returns>
        ///   A sample which could original the given probability
        ///   value when applied in the <see cref="UnivariateDiscreteDistribution.DistributionFunction(int)"/>.
        /// </returns>
        /// 
        protected override int InnerInverseDistributionFunction(double p)
        {
            double num = Special.Log1m(p);
            double den = Special.Log1m(this.p);
            double ratio = num / den;
            return (int)Math.Ceiling(ratio) - 1;
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
        public override void Fit(double[] observations, double[] weights, IFittingOptions options)
        {
            if (options != null)
                throw new ArgumentException("No options may be specified.");

            double mean;

            if (weights == null)
                mean = Measures.Mean(observations);
            else
                mean = Measures.WeightedMean(observations, weights);

            p = 1.0 / (1.0 - mean);
        }

        /// <summary>
        /// Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        /// 
        /// <returns>
        /// A random observations drawn from this distribution.
        /// </returns>
        /// 
        public override int Generate(Random source)
        {
             return (int)Random(p, source);
        }

        /// <summary>
        /// Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        /// 
        /// <returns>
        /// A random vector of observations drawn from this distribution.
        /// </returns>
        /// 
        public override double[] Generate(int samples, double[] result, Random source)
        {
            return Random(p, samples, result, source);
        }

        /// <summary>
        /// Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///   
        /// <returns>
        /// A random vector of observations drawn from this distribution.
        /// </returns>
        public override int[] Generate(int samples, int[] result, Random source)
        {
            return Random(p, samples, result, source);
        }

        /// <summary>
        /// Generates a random observation from the current distribution.
        /// </summary>
        /// <param name="p">The probability of success.</param>
        /// <returns>
        /// A random observations drawn from this distribution.
        /// </returns>
        public static double Random(double p)
        {
            return Random(p, Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        /// Generates a random vector of observations from the current distribution.
        /// </summary>
        /// <param name="p">The probability of success.</param>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <returns>
        /// A random vector of observations drawn from this distribution.
        /// </returns>
        public static double[] Random(double p, int samples, double[] result)
        {
            return Random(p, samples, result, Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        /// Generates a random vector of observations from the current distribution.
        /// </summary>
        /// <param name="p">The probability of success.</param>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <returns>
        /// A random vector of observations drawn from this distribution.
        /// </returns>

        public static int[] Random(double p, int samples, int[] result)
        {
            return Random(p, samples, result, Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        /// Generates a random observation from the current distribution.
        /// </summary>
        /// <param name="p">The probability of success.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        /// <returns>
        /// A random observations drawn from this distribution.
        /// </returns>
        public static double Random(double p, Random source)
        {
            return Math.Floor(Special.Log1m(source.NextDouble()) / Special.Log1m(p));
        }

        /// <summary>
        /// Generates a random vector of observations from the current distribution.
        /// </summary>
        /// <param name="p">The probability of success.</param>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///   
        /// <returns>
        /// A random vector of observations drawn from this distribution.
        /// </returns>
        /// 
        public static double[] Random(double p, int samples, double[] result, Random source)
        {
            for (int i = 0; i < samples; i++)
                result[i] = Math.Floor(Special.Log1m(source.NextDouble()) / Special.Log1m(p));
            return result;
        }

        /// <summary>
        /// Generates a random vector of observations from the current distribution.
        /// </summary>
        /// <param name="p">The probability of success.</param>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///   
        /// <returns>
        /// A random vector of observations drawn from this distribution.
        /// </returns>
        /// 
        public static int[] Random(double p, int samples, int[] result, Random source)
        {
            for (int i = 0; i < samples; i++)
                result[i] = (int)Math.Floor(Special.Log1m(source.NextDouble()) / Special.Log1m(p));
            return result;
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
            return new GeometricDistribution(p);
        }


        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public override string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format(formatProvider, "Geometric(x; p = {0})",
                p.ToString(format, formatProvider));
        }

    }
}
