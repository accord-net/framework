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
    ///   Nakagami distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Nakagami distribution has been used in the modeling of wireless
    ///   signal attenuation while traversing multiple paths. </para>
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Nakagami_distribution">
    ///       Wikipedia, The Free Encyclopedia. Nakagami distribution. Available on:
    ///       http://en.wikipedia.org/wiki/Nakagami_distribution </a></description></item>
    ///     <item><description>
    ///       Laurenson, Dave (1994). "Nakagami Distribution". Indoor Radio Channel Propagation
    ///       Modeling by Ray Tracing Techniques. </description></item>
    ///     <item><description>  
    ///       R. Kolar, R. Jirik, J. Jan (2004) "Estimator Comparison of the Nakagami-m Parameter
    ///       and Its Application in Echocardiography", Radioengineering, 13 (1), 8–12 </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///   var nakagami = new NakagamiDistribution(shape: 2.4, spread: 4.2);
    ///   
    ///   double mean = nakagami.Mean;     // 1.946082119049118
    ///   double median = nakagami.Median; // 1.9061151110206338
    ///   double var = nakagami.Variance;  // 0.41276438591729486
    ///   
    ///   double cdf = nakagami.DistributionFunction(x: 1.4); // 0.20603416752368109
    ///   double pdf = nakagami.ProbabilityDensityFunction(x: 1.4); // 0.49253215371343023
    ///   double lpdf = nakagami.LogProbabilityDensityFunction(x: 1.4); // -0.708195533773302
    ///   
    ///   double ccdf = nakagami.ComplementaryDistributionFunction(x: 1.4); // 0.79396583247631891
    ///   double icdf = nakagami.InverseDistributionFunction(p: cdf); // 1.400000000131993
    ///   
    ///   double hf = nakagami.HazardFunction(x: 1.4); // 0.62034426869133652
    ///   double chf = nakagami.CumulativeHazardFunction(x: 1.4); // 0.23071485080660473
    ///   
    ///   string str = nakagami.ToString(CultureInfo.InvariantCulture); // Nakagami(x; μ = 2,4, ω = 4,2)"
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class NakagamiDistribution : UnivariateContinuousDistribution,
        ISampleableDistribution<double>, IFormattable
    {
        // distribution parameters
        private double mu;
        private double omega;

        // derived values
        private double? mean;
        private double? variance;

        private double constant; // 2 * μ ^ μ / (Γ(μ) * ω ^ μ))
        private double nratio;   // -μ / ω
        private double twoMu1;   // 2 * μ - 1.0


        /// <summary>
        ///   Initializes a new instance of the <see cref="NakagamiDistribution"/> class.
        /// </summary>
        /// 
        /// <param name="shape">The shape parameter μ (mu).</param>
        /// <param name="spread">The spread parameter ω (omega).</param>
        /// 
        public NakagamiDistribution([Positive(minimum: 0.5), DefaultValue(0.5)] double shape,
            [Positive] double spread)
        {
            if (shape < 0.5)
            {
                throw new ArgumentOutOfRangeException("shape",
                "Shape parameter (mu) should be greater than or equal to 0.5.");
            }

            if (spread <= 0)
            {
                throw new ArgumentOutOfRangeException("spread",
                "Spread parameter (omega) should be greater than 0.");
            }

            this.mu = shape;
            this.omega = spread;

            init(shape, spread);
        }

        private void init(double shape, double spread)
        {
            this.mu = shape;
            this.omega = spread;

            double twoMuMu = 2.0 * Math.Pow(mu, mu);
            double gammaMu = Gamma.Function(mu);
            double spreadMu = Math.Pow(omega, mu);
            nratio = -mu / omega;
            twoMu1 = 2.0 * mu - 1.0;

            constant = twoMuMu / (gammaMu * spreadMu);

            mean = null;
            variance = null;
        }

        /// <summary>
        ///   Gets the distribution's shape parameter μ (mu).
        /// </summary>
        /// 
        /// <value>The shape parameter μ (mu).</value>
        /// 
        public double Shape
        {
            get { return mu; }
        }

        /// <summary>
        ///   Gets the distribution's spread parameter ω (omega).
        /// </summary>
        /// 
        /// <value>The spread parameter ω (omega).</value>
        /// 
        public double Spread
        {
            get { return omega; }
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean value.</value>
        /// 
        /// <remarks>
        ///   Nakagami's mean is defined in terms of the <see cref="Gamma.Function(double)">
        ///   Gamma function Γ(x)</see> as <c>(Γ(μ + 0.5) / Γ(μ)) * sqrt(ω / μ)</c>.
        /// </remarks>
        /// 
        public override double Mean
        {
            get
            {
                if (mean == null)
                    mean = (Gamma.Function(mu + 0.5) / Gamma.Function(mu)) * Math.Sqrt(omega / mu);
                return mean.Value;
            }
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
            get
            {
                double a = Math.Sqrt(2) / 2;
                double b = ((2 * mu - 1) * omega) / mu;
                return a * Math.Sqrt(b);
            }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        /// <remarks>
        ///   Nakagami's variance is defined in terms of the <see cref="Gamma.Function(double)">
        ///   Gamma function Γ(x)</see> as <c>ω * (1 - (1 / μ) * (Γ(μ + 0.5) / Γ(μ))²</c>.
        /// </remarks>
        /// 
        public override double Variance
        {
            get
            {
                if (variance == null)
                {
                    double a = Gamma.Function(mu + 0.5) / Gamma.Function(mu);
                    variance = omega * (1.0 - (1.0 / mu) * (a * a));
                }
                return variance.Value;
            }
        }

        /// <summary>
        ///   This method is not supported.
        /// </summary>
        /// 
        public override double Entropy
        {
            get { throw new NotSupportedException(); }
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
            get { return new DoubleRange(Double.Epsilon, Double.PositiveInfinity); }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <remarks>
        /// <para>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.</para>
        /// <para>
        ///   The Nakagami's distribution CDF is defined in terms of the 
        ///   <see cref="Gamma.LowerIncomplete">Lower incomplete regularized 
        ///   Gamma function P(a, x)</see> as <c>CDF(x) = P(μ, μ / ω) * x²</c>.
        /// </para>
        /// </remarks>
        /// 
        /// <example>
        ///   See <see cref="NakagamiDistribution"/>.
        /// </example>
        /// 
        protected internal override double InnerDistributionFunction(double x)
        {
            return Gamma.LowerIncomplete(mu, (mu / omega) * (x * x));
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
        /// <para>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.</para>
        /// <para>
        ///   Nakagami's PDF is defined as 
        ///   <c>PDF(x) = c * x^(2 * μ - 1) * exp(-(μ / ω) * x²)</c>
        ///   in which <c>c = 2 * μ ^ μ / (Γ(μ) * ω ^ μ))</c></para>
        /// </remarks>
        /// 
        /// <example>
        ///   See <see cref="NakagamiDistribution"/>.
        /// </example>
        /// 
        protected internal override double InnerProbabilityDensityFunction(double x)
        {
            return constant * Math.Pow(x, twoMu1) * Math.Exp(nratio * x * x);
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
        /// <para>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.</para>
        /// <para>
        ///   Nakagami's PDF is defined as 
        ///   <c>PDF(x) = c * x^(2 * μ - 1) * exp(-(μ / ω) * x²)</c>
        ///   in which <c>c = 2 * μ ^ μ / (Γ(μ) * ω ^ μ))</c></para>
        /// </remarks>
        /// 
        /// <example>
        ///   See <see cref="NakagamiDistribution"/>.
        /// </example>
        /// 
        protected internal override double InnerLogProbabilityDensityFunction(double x)
        {
            return Math.Log(constant) + twoMu1 * Math.Log(x) + nratio * x * x;
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
                throw new ArgumentException("This method does not accept fitting options.");

            // R. Kolar, R. Jirik, J. Jan (2004) "Estimator Comparison of the
            // Nakagami-m Parameter and Its Application in Echocardiography", 
            // Radioengineering, 13 (1), 8–12

            double[] x2 = Elementwise.Pow(observations, 2);

            double mean, var;
            if (weights == null)
            {
                mean = Measures.Mean(x2);
                var = Measures.Variance(x2, mean);
            }
            else
            {
                mean = Measures.WeightedMean(x2, weights);
                var = Measures.WeightedVariance(x2, weights, mean);
            }

            double shape = (mean * mean) / var;
            double spread = mean;

            init(shape, spread);
        }

        /// <summary>
        ///   Estimates a new Nakagami distribution from a given set of observations.
        /// </summary>
        /// 
        public static NakagamiDistribution Estimate(double[] observations)
        {
            return Estimate(observations, null);
        }

        /// <summary>
        ///   Estimates a new Nakagami distribution from a given set of observations.
        /// </summary>
        /// 
        public static NakagamiDistribution Estimate(double[] observations, double[] weights)
        {
            var n = new NakagamiDistribution();
            n.Fit(observations, weights, null);
            return n;
        }

        private NakagamiDistribution() { }

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
            return new NakagamiDistribution(mu, omega);
        }

        #region ISampleableDistribution<double> Members

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
            return Random(mu, omega, samples, result, source);
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
            return Random(mu, omega, source);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Nakagami distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="shape">The shape parameter μ.</param>
        /// <param name="spread">The spread parameter ω.</param>
        /// <param name="samples">The number of samples to generate.</param>
        ///
        /// <returns>An array of double values sampled from the specified Nakagami distribution.</returns>
        /// 
        public static double[] Random(double shape, double spread, int samples)
        {
            return Random(shape, spread, samples, Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Nakagami distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="shape">The shape parameter μ.</param>
        /// <param name="spread">The spread parameter ω.</param>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        ///
        /// <returns>An array of double values sampled from the specified Nakagami distribution.</returns>
        /// 
        public static double[] Random(double shape, double spread, int samples, double[] result)
        {
            return Random(shape, spread, samples, result, Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        ///   Generates a random observation from the 
        ///   Nakagami distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="shape">The shape parameter μ.</param>
        /// <param name="spread">The spread parameter ω.</param>
        /// 
        /// <returns>A random double value sampled from the specified Nakagami distribution.</returns>
        /// 
        public static double Random(double shape, double spread)
        {
            return Random(shape, spread, Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Nakagami distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="shape">The shape parameter μ.</param>
        /// <param name="spread">The spread parameter ω.</param>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///
        /// <returns>An array of double values sampled from the specified Nakagami distribution.</returns>
        /// 
        public static double[] Random(double shape, double spread, int samples, Random source)
        {
            return Random(shape, spread, samples, new double[samples], source);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Nakagami distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="shape">The shape parameter μ.</param>
        /// <param name="spread">The spread parameter ω.</param>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///
        /// <returns>An array of double values sampled from the specified Nakagami distribution.</returns>
        /// 
        public static double[] Random(double shape, double spread, int samples, double[] result, Random source)
        {
            GammaDistribution.Random(shape, spread / shape, samples, result, source: source);
            for (int i = 0; i < result.Length; i++)
                result[i] = Math.Sqrt(result[i]);
            return result;
        }

        /// <summary>
        ///   Generates a random observation from the 
        ///   Nakagami distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="shape">The shape parameter μ.</param>
        /// <param name="spread">The spread parameter ω.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        /// 
        /// <returns>A random double value sampled from the specified Nakagami distribution.</returns>
        /// 
        public static double Random(double shape, double spread, Random source)
        {
            return Math.Sqrt(GammaDistribution.Random(shape: shape, scale: spread / shape, source: source));
        }

        #endregion


        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public override string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format(formatProvider, "Nakagami(x; μ = {0}, ω = {1})",
                mu.ToString(format, formatProvider),
                omega.ToString(format, formatProvider));
        }

    }
}
