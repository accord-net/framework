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
    using Accord.Statistics.Distributions.Fitting;
    using AForge;

    /// <summary>
    ///   Weibull distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability theory and statistics, the Weibull distribution is a
    ///   continuous probability distribution. It is named after Waloddi Weibull,
    ///   who described it in detail in 1951, although it was first identified by
    ///   Fréchet (1927) and first applied by Rosin and Rammler (1933) to describe a
    ///   particle size distribution.</para>
    ///   
    /// <para>
    /// The Weibull distribution is related to a number of other probability distributions;
    /// in particular, it interpolates between the <see cref="ExponentialDistribution">
    /// exponential distribution</see> (for k = 1) and the <see cref="RayleighDistribution">
    /// Rayleigh distribution</see> (when k = 2). </para>
    /// 
    /// <para>
    /// If the quantity x is a "time-to-failure", the Weibull distribution gives a 
    /// distribution for which the failure rate is proportional to a power of time.
    /// The shape parameter, k, is that power plus one, and so this parameter can be
    /// interpreted directly as follows:</para>
    /// 
    /// <list type="bullet">
    ///   <item><description>
    ///     A value of k &lt; 1 indicates that the failure rate decreases over time. This
    ///     happens if there is significant "infant mortality", or defective items failing
    ///     early and the failure rate decreasing over time as the defective items are 
    ///     weeded out of the population.</description></item>
    ///   <item><description>
    ///     A value of k = 1 indicates that the failure rate is constant over time. This 
    ///     might suggest random external events are causing mortality, or failure.</description></item>
    ///   <item><description>
    ///     A value of k > 1 indicates that the failure rate increases with time. This 
    ///     happens if there is an "aging" process, or parts that are more likely to fail 
    ///     as time goes on.</description></item>
    /// </list>
    /// <para>
    /// In the field of materials science, the shape parameter <c>k</c> of a distribution 
    /// of strengths is known as the Weibull modulus.</para>
    /// 
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Weibull_distribution">
    ///       Wikipedia, The Free Encyclopedia. Weibull distribution. Available on:
    ///       http://en.wikipedia.org/wiki/Weibull_distribution </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///    // Create a new Weibull distribution with λ = 0.42 and k = 1.2
    ///    var weilbull = new WeibullDistribution(scale: 0.42, shape: 1.2);
    ///    
    ///    // Common measures
    ///    double mean = weilbull.Mean;     // 0.39507546046784414
    ///    double median = weilbull.Median; // 0.30945951550913292
    ///    double var = weilbull.Variance;  // 0.10932249666369542
    ///    double mode = weilbull.Mode;     // 0.094360430821809421
    ///    
    ///    // Cumulative distribution functions
    ///    double cdf = weilbull.DistributionFunction(x: 1.4);           //  0.98560487188700052
    ///    double pdf = weilbull.ProbabilityDensityFunction(x: 1.4);     //  0.052326687031379278
    ///    double lpdf = weilbull.LogProbabilityDensityFunction(x: 1.4); // -2.9502487697674415
    ///    
    ///    // Probability density functions
    ///    double ccdf = weilbull.ComplementaryDistributionFunction(x: 1.4); // 0.22369885565908001
    ///    double icdf = weilbull.InverseDistributionFunction(p: cdf);       // 1.400000001051205
    ///    
    ///    // Hazard (failure rate) functions
    ///    double hf = weilbull.HazardFunction(x: 1.4);            // 1.1093328057258516
    ///    double chf = weilbull.CumulativeHazardFunction(x: 1.4); // 1.4974545260150962
    ///    
    ///    // String representation
    ///    string str = weilbull.ToString(CultureInfo.InvariantCulture); // Weibull(x; λ = 0.42, k = 1.2)
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class WeibullDistribution : UnivariateContinuousDistribution,
        ISampleableDistribution<double>, IFormattable
    {

        // Distribution parameters
        private double k;      //  k > 0
        private double lambda; //  λ > 0 (lambda)


        /// <summary>
        ///   Initializes a new instance of the <see cref="WeibullDistribution"/> class.
        /// </summary>
        /// 
        /// <param name="scale">The scale parameter λ (lambda).</param>
        /// <param name="shape">The shape parameter k.</param>
        /// 
        public WeibullDistribution(double shape, double scale)
        {
            if (shape <= 0) // k
                throw new ArgumentOutOfRangeException("shape", "Shape (k) must be greater than zero.");

            if (scale <= 0) // lambda
                throw new ArgumentOutOfRangeException("shape", "Scale (lambda) must be greater than zero.");

            this.k = shape;
            this.lambda = scale;
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean value.</value>
        /// 
        public override double Mean
        {
            get { return lambda * Gamma.Function(1 + 1 / k); }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        public override double Variance
        {
            get { return lambda * lambda * Gamma.Function(1 + 2 / k) - Mean * Mean; }
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
            get { return lambda * Math.Pow(Math.Log(2.0), 1.0 / k); }
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
            get { return k > 1 ? lambda * Math.Pow((k - 1) / k, 1 / k) : 0; }
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
            get { return new DoubleRange(0, Double.PositiveInfinity); }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's entropy.</value>
        /// 
        public override double Entropy
        {
            get { return Constants.EulerGamma * (1 - 1 / k) + Math.Log(lambda / k) + 1; }
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
        public override double DistributionFunction(double x)
        {
            if (x < 0) return 0;

            return 1.0 - Math.Exp(-Math.Pow(x / lambda, k));
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
        public override double ProbabilityDensityFunction(double x)
        {
            if (x <= 0) return 0;

            double a = Math.Pow(x / lambda, k - 1);
            double b = Math.Exp(-Math.Pow(x / lambda, k));
            return (k / lambda) * a * b;
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
        public override double LogProbabilityDensityFunction(double x)
        {
            if (x <= 0) return Double.NegativeInfinity;

            return Math.Log(k / lambda) + (k - 1) * Math.Log(x / lambda) - Math.Pow(x / lambda, k);
        }

        /// <summary>
        ///   Gets the hazard function, also known as the failure rate or
        ///   the conditional failure density function for this distribution
        ///   evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The conditional failure density function <c>h(x)</c>
        ///   evaluated at <c>x</c> in the current distribution.
        /// </returns>
        /// 
        public override double HazardFunction(double x)
        {
            return Math.Pow(k * x, k - 1);
        }

        /// <summary>
        ///   Gets the cumulative hazard function for this
        ///   distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The cumulative hazard function <c>H(x)</c>
        ///   evaluated at <c>x</c> in the current distribution.
        /// </returns>
        /// 
        public override double CumulativeHazardFunction(double x)
        {
            return Math.Pow(x, k);
        }

        /// <summary>
        ///   Gets the complementary cumulative distribution function
        ///   (ccdf) for this distribution evaluated at point <c>x</c>.
        ///   This function is also known as the Survival function.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        public override double ComplementaryDistributionFunction(double x)
        {
            return Math.Exp(-Math.Pow(x, k));
        }

        /// <summary>
        ///   Gets the inverse of the <see cref="ComplementaryDistributionFunction"/>. 
        ///   The inverse complementary distribution function is also known as the 
        ///   inverse survival Function.
        /// </summary>
        /// 
        public double InverseComplementaryDistributionFunction(double p)
        {
            return Math.Pow(-Math.Log(p), 1 / k);
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
            throw new NotImplementedException();
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
            return new WeibullDistribution(lambda, k);
        }


        #region ISampleableDistribution<double> Members

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public double[] Generate(int samples)
        {
            return Random(k, lambda, samples);
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public double Generate()
        {
            return Random(k, lambda);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Weibull distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="scale">The scale parameter lambda.</param>
        /// <param name="shape">The shape parameter k.</param>
        /// <param name="samples">The number of samples to generate.</param>
        ///
        /// <returns>An array of double values sampled from the specified Weibull distribution.</returns>
        /// 
        public static double[] Random(double shape, double scale, int samples)
        {
            double[] r = new double[samples];
            for (int i = 0; i < r.Length; i++)
            {
                double u = Accord.Math.Tools.Random.NextDouble();
                r[i] = scale * Math.Pow(-Math.Log(u), 1 / shape);
            }

            return r;
        }

        /// <summary>
        ///   Generates a random observation from the 
        ///   Weibull distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="scale">The scale parameter lambda.</param>
        /// <param name="shape">The shape parameter k.</param>
        /// 
        /// <returns>A random double value sampled from the specified Weibull distribution.</returns>
        /// 
        public static double Random(double shape, double scale)
        {
            double u = Accord.Math.Tools.Random.NextDouble();
            return scale * Math.Pow(-Math.Log(u), 1 / shape);
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
        public override string ToString()
        {
            return String.Format("Weibull(x; λ = {0}, k = {1})", lambda, k);
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
            return String.Format(formatProvider, "Weibull(x; λ = {0}, k = {1})", lambda, k);
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
            return String.Format(formatProvider, "Weibull(x; λ = {0}, k = {1})",
                lambda.ToString(format, formatProvider), 
                k.ToString(format, formatProvider));
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
            return String.Format("Weibull(x; λ = {0}, k = {1})",
                lambda.ToString(format), k.ToString(format));
        }


    }
}
