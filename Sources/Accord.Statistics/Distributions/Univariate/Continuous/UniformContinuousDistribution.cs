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

namespace Accord.Statistics.Distributions.Univariate
{
    using System;
    using Accord.Math;
    using Accord.Statistics.Distributions.Fitting;
    using System.ComponentModel;
    using Accord.Compat;

    /// <summary>
    ///   Continuous Uniform Distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The continuous uniform distribution or rectangular distribution is a family of 
    ///   symmetric probability distributions such that for each member of the family, all
    ///   intervals of the same length on the distribution's support are equally probable.
    ///   The support is defined by the two parameters, a and b, which are its minimum and
    ///   maximum values. The distribution is often abbreviated U(a,b). It is the maximum
    ///   entropy probability distribution for a random variate X under no constraint other
    ///   than that it is contained in the distribution's support.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Uniform_distribution_(continuous)">
    ///       Wikipedia, The Free Encyclopedia. Uniform Distribution (continuous). Available on: 
    ///       http://en.wikipedia.org/wiki/Uniform_distribution_(continuous) </a></description></item>
    ///   </list></para> 
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example demonstrates how to create an uniform 
    ///   distribution defined over the interval [0.42, 1.1]. </para>
    ///   
    /// <code>
    ///   // Create a new uniform continuous distribution from 0.42 to 1.1
    ///   var uniform = new UniformContinuousDistribution(a: 0.42, b: 1.1);
    ///   
    ///   // Common measures
    ///   double mean = uniform.Mean;     // 0.76
    ///   double median = uniform.Median; // 0.76
    ///   double var = uniform.Variance;  // 0.03853333333333335
    ///   
    ///   // Cumulative distribution functions
    ///   double cdf = uniform.DistributionFunction(x: 0.9);               // 0.70588235294117641
    ///   double ccdf = uniform.ComplementaryDistributionFunction(x: 0.9); // 0.29411764705882359
    ///   double icdf = uniform.InverseDistributionFunction(p: cdf);       // 0.9
    ///   
    ///   // Probability density functions
    ///   double pdf = uniform.ProbabilityDensityFunction(x: 0.9);     // 1.4705882352941173
    ///   double lpdf = uniform.LogProbabilityDensityFunction(x: 0.9); // 0.38566248081198445
    ///   
    ///   // Hazard (failure rate) functions
    ///   double hf = uniform.HazardFunction(x: 0.9); // 4.9999999999999973
    ///   double chf = uniform.CumulativeHazardFunction(x: 0.9); // 1.2237754316221154
    ///   
    ///   // String representation
    ///   string str = uniform.ToString(CultureInfo.InvariantCulture); // "U(x; a = 0.42, b = 1.1)"
    /// </code>
    /// </example>
    ///   
    [Serializable]
    public class UniformContinuousDistribution : UnivariateContinuousDistribution,
        IFittableDistribution<double, IFittingOptions>,
        ISampleableDistribution<double>
    {

        // Distribution parameters
        private double a;
        private double b;


        private bool immutable;


        /// <summary>
        ///   Creates a new uniform distribution defined in the interval [0;1].
        /// </summary>
        /// 
        public UniformContinuousDistribution()
            : this(0, 1) { }

        /// <summary>
        ///   Creates a new uniform distribution defined in the interval [min;max].
        /// </summary>
        /// 
        /// <param name="range">The output range for the uniform distribution.</param>
        /// 
        public UniformContinuousDistribution(DoubleRange range)
            : this(range.Min, range.Max)
        {
        }

        /// <summary>
        ///   Creates a new uniform distribution defined in the interval [a;b].
        /// </summary>
        /// 
        /// <param name="a">The starting number a.</param>
        /// <param name="b">The ending number b.</param>
        /// 
        public UniformContinuousDistribution(
            [Real, DefaultValue(0)] double a,
            [Real, DefaultValue(1)] double b)
        {
            if (a > b)
            {
                throw new ArgumentOutOfRangeException("b",
                    "The starting number a must be lower than b.");
            }

            this.a = a;
            this.b = b;
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
        ///   Gets the length of the distribution (b-a).
        /// </summary>
        /// 
        public double Length { get { return b - a; } }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        public override double Mean
        {
            get { return (a + b) / 2; }
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
        ///   Gets the mode for this distribution.
        /// </summary>
        /// 
        /// <remarks>
        ///   The mode of the uniform distribution is any value contained
        ///   in the interval of the distribution. The framework return
        ///   the same value as the <see cref="Mean"/>.
        /// </remarks>
        /// 
        /// <value>
        ///   The distribution's mode value.
        /// </value>
        /// 
        public override double Mode
        {
            get { return Mean; }
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
        ///   A <see cref="DoubleRange" /> containing
        ///   the support interval for this distribution.
        /// </value>
        /// 
        public override DoubleRange Support
        {
            get { return new DoubleRange(a, b); }
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
        protected internal override double InnerDistributionFunction(double x)
        {
            return (x - a) / (b - a);
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
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
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        protected internal override double InnerProbabilityDensityFunction(double x)
        {
            return 1.0 / (b - a);
        }

        /// <summary>
        ///   Gets the log-probability density function (pdf) for
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
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        protected internal override double InnerLogProbabilityDensityFunction(double x)
        {
            return -Math.Log(b - a);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        ///   
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
        public override void Fit(double[] observations, double[] weights, Fitting.IFittingOptions options)
        {
            if (immutable)
                throw new InvalidOperationException("This object can not be modified.");

            if (options != null)
                throw new ArgumentException("This method does not accept fitting options.");

            if (weights != null)
                throw new ArgumentException("This distribution does not support weighted samples.");

            a = observations.Min();
            b = observations.Max();
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
            return new UniformContinuousDistribution(a, b);
        }


        /// <summary>
        ///   Gets the Standard Uniform Distribution,
        ///   starting at zero and ending at one (a=0, b=1).
        /// </summary>
        /// 
        public static UniformContinuousDistribution Standard { get { return standard; } }

        private static readonly UniformContinuousDistribution standard = new UniformContinuousDistribution() { immutable = true };


        /// <summary>
        ///   Estimates a new uniform distribution from a given set of observations.
        /// </summary>
        /// 
        public static UniformContinuousDistribution Estimate(double[] observations)
        {
            var n = new UniformContinuousDistribution();
            n.Fit(observations);
            return n;
        }

        #region ISamplableDistribution<double> Members

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        /// 
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public override double[] Generate(int samples, double[] result, Random source)
        {
            return Random(a, b, samples, result, source);
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///   
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public override double Generate(Random source)
        {
            return Random(a, b, source);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Uniform distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="a">The starting number a.</param>
        /// <param name="b">The ending number b.</param>
        /// <param name="samples">The number of samples to generate.</param>
        ///
        /// <returns>An array of double values sampled from the specified Uniform distribution.</returns>
        /// 
        public static double[] Random(double a, double b, int samples)
        {
            return Random(a, b, samples, new double[samples], Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Uniform distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="a">The starting number a.</param>
        /// <param name="b">The ending number b.</param>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///
        /// <returns>An array of double values sampled from the specified Uniform distribution.</returns>
        /// 
        public static double[] Random(double a, double b, int samples, Random source)
        {
            return Random(a, b, samples, new double[samples], source);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Uniform distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="a">The starting number a.</param>
        /// <param name="b">The ending number b.</param>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        ///
        /// <returns>An array of double values sampled from the specified Uniform distribution.</returns>
        /// 
        public static double[] Random(double a, double b, int samples, double[] result)
        {
            return Random(a, b, samples, result, Accord.Math.Random.Generator.Random);
        }


        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Uniform distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="a">The starting number a.</param>
        /// <param name="b">The ending number b.</param>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///
        /// <returns>An array of double values sampled from the specified Uniform distribution.</returns>
        /// 
        public static double[] Random(double a, double b, int samples, double[] result, Random source)
        {
            for (int i = 0; i < samples; i++)
                result[i] = source.NextDouble() * (b - a) + a;
            return result;
        }

        /// <summary>
        ///   Generates a random observation from the Uniform 
        ///   distribution defined in the interval 0 and 1.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        ///
        /// <returns>An array of double values sampled from the specified Uniform distribution.</returns>
        /// 
        public static double[] Random(int samples)
        {
            return Random(samples, new double[samples]);
        }

        /// <summary>
        ///   Generates a random observation from the Uniform 
        ///   distribution defined in the interval 0 and 1.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        ///
        /// <returns>An array of double values sampled from the specified Uniform distribution.</returns>
        /// 
        public static double[] Random(int samples, double[] result)
        {
            return Random(samples, result, Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        ///   Generates a random observation from the Uniform 
        ///   distribution defined in the interval 0 and 1.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///
        /// <returns>An array of double values sampled from the specified Uniform distribution.</returns>
        /// 
        public static double[] Random(int samples, double[] result, Random source)
        {
            for (int i = 0; i < samples; i++)
                result[i] = source.NextDouble();
            return result;
        }

        /// <summary>
        ///   Generates a random observation from the Uniform 
        ///   distribution defined in the interval 0 and 1.
        /// </summary>
        /// 
        /// <returns>A random double value sampled from the specified Uniform distribution.</returns>
        /// 
        public static double Random()
        {
            return Random(Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        ///   Generates a random observation from the Uniform 
        ///   distribution defined in the interval 0 and 1.
        /// </summary>
        /// 
        /// <returns>A random double value sampled from the specified Uniform distribution.</returns>
        /// 
        public static double Random(Random source)
        {
            return source.NextDouble();
        }

        /// <summary>
        ///   Generates a random observation from the 
        ///   Uniform distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="a">The starting number a.</param>
        /// <param name="b">The ending number b.</param>
        /// 
        /// <returns>A random double value sampled from the specified Uniform distribution.</returns>
        /// 
        public static double Random(double a, double b)
        {
            return Random(a, b, Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        ///   Generates a random observation from the 
        ///   Uniform distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="a">The starting number a.</param>
        /// <param name="b">The ending number b.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        /// 
        /// <returns>A random double value sampled from the specified Uniform distribution.</returns>
        /// 
        public static double Random(double a, double b, Random source)
        {
            return source.NextDouble() * (b - a) + a;
        }
        #endregion


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
            return String.Format(formatProvider, "U(x; a = {0}, b = {1})",
                a.ToString(format, formatProvider),
                b.ToString(format, formatProvider));
        }

    }
}
