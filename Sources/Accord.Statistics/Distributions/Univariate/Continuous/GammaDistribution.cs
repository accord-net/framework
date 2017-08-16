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
    using Accord.Compat;

    /// <summary>
    ///   Gamma distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///    The gamma distribution is a two-parameter family of continuous probability
    ///    distributions. There are three different parameterizations in common use:</para>
    ///    <list type="bullet">
    ///    <item><description>
    ///       With a <see cref="Shape"/> parameter k and a
    ///       <see cref="Scale"/> parameter θ.</description></item>
    ///    <item><description>
    ///       With a shape parameter α = k and an inverse scale parameter
    ///       β = 1/θ, called a <see cref="Rate"/> parameter.</description></item>
    ///    <item><description>
    ///       With a shape parameter k and a <see cref="Mean"/>
    ///       parameter μ = k/β.</description></item>
    ///    </list>
    ///       
    /// <para>
    ///    In each of these three forms, both parameters are positive real numbers. The
    ///    parameterization with k and θ appears to be more common in econometrics and 
    ///    certain other applied fields, where e.g. the gamma distribution is frequently
    ///    used to model waiting times. For instance, in life testing, the waiting time 
    ///    until death is a random variable that is frequently modeled with a gamma 
    ///    distribution. This is the <see cref="GammaDistribution(double,double)">default 
    ///    construction method for this class</see>.</para>
    /// <para>
    ///    The parameterization with α and β is more common in Bayesian statistics, where 
    ///    the gamma distribution is used as a conjugate prior distribution for various 
    ///    types of inverse scale (aka rate) parameters, such as the λ of an exponential 
    ///    distribution or a Poisson distribution – or for that matter, the β of the gamma
    ///    distribution itself. (The closely related inverse gamma distribution is used as
    ///    a conjugate prior for scale parameters, such as the variance of a normal distribution.)
    ///    In order to create a Gamma distribution using the Bayesian parameterization, you
    ///    can use <see cref="GammaDistribution.FromBayesian"/>.</para>
    /// <para>
    ///    If k is an integer, then the distribution represents an Erlang distribution; i.e.,
    ///    the sum of k independent exponentially distributed random variables, each of which
    ///    has a mean of θ (which is equivalent to a rate parameter of 1/θ). </para>
    /// <para>
    ///    The gamma distribution is the maximum entropy probability distribution for a random 
    ///    variable X for which E[X] = kθ = α/β is fixed and greater than zero, and <c>E[ln(X)] = 
    ///    ψ(k) + ln(θ) = ψ(α) − ln(β)</c> is fixed (ψ is the digamma function).</para>
    ///    
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Gamma_distribution">
    ///       Wikipedia, The Free Encyclopedia. Gamma distribution. Available on: 
    ///       http://en.wikipedia.org/wiki/Gamma_distribution </a></description></item>
    ///   </list></para>     
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example shows how to create, test and compute the main 
    ///   functions of a Gamma distribution given parameters θ = 4 and k = 2: </para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.Statistics\Distributions\Univariate\Continuous\GammaDistributionTest.cs" region="doc_ctor"/>
    /// </example>
    /// 
    /// <seealso cref="Accord.Math.Gamma"/>
    /// <seealso cref="InverseGammaDistribution"/>
    /// 
    [Serializable]
    public class GammaDistribution : UnivariateContinuousDistribution,
        IFittableDistribution<double, GammaOptions>,
        ISampleableDistribution<double>
    {

        // Distribution parameters
        private double theta; // scale (θ)
        private double k;     // shape

        // Derived measures
        private double lnconstant;

        private bool immutable;


        /// <summary>
        ///   Constructs a Gamma distribution.
        /// </summary>
        /// 
        public GammaDistribution()
            : this(1, 1)
        {
            init(theta, k);
        }

        /// <summary>
        ///   Constructs a Gamma distribution.
        /// </summary>
        /// 
        /// <param name="theta">The scale parameter θ (theta). Default is 1.</param>
        /// <param name="k">The shape parameter k. Default is 1.</param>
        /// 
        public GammaDistribution([Positive] double theta, [Positive] double k)
        {
            if (k <= 0)
                throw new ArgumentOutOfRangeException("k", "Parameter k must be positive.");

            if (theta <= 0)
                throw new ArgumentOutOfRangeException("theta", "Theta must be positive.");

            init(theta, k);
        }

        /// <summary>
        ///   Constructs a Gamma distribution using α and β parameterization.
        /// </summary>
        /// 
        /// <param name="alpha">The shape parameter α = k.</param>
        /// <param name="beta">The inverse scale parameter β = 1/θ.</param>
        /// 
        /// <returns>A Gamma distribution constructed with the given parameterization.</returns>
        /// 
        public static GammaDistribution FromBayesian(double alpha, double beta)
        {
            return new GammaDistribution(1.0 / beta, alpha);
        }

        /// <summary>
        ///   Constructs a Gamma distribution using k and μ parameterization.
        /// </summary>
        /// 
        /// <param name="alpha">The shape parameter α = k.</param>
        /// <param name="mean">The mean parameter μ = k/β.</param>
        /// 
        /// <returns>A Gamma distribution constructed with the given parameterization.</returns>
        /// 
        public static GammaDistribution FromMean(double alpha, double mean)
        {
            return new GammaDistribution(mean / alpha, alpha);
        }

        private void init(double theta, double k)
        {
            if (Double.IsNaN(theta))
                throw new ArgumentOutOfRangeException("theta", "Theta contains a value that is Not-a-Number.");

            if (Double.IsNaN(k))
                throw new ArgumentOutOfRangeException("theta", "The shape parameter k contains a value that is Not-a-Number.");

            this.theta = theta;
            this.k = k;

            this.lnconstant = -(k * Math.Log(theta) + Gamma.Log(k));
        }

        /// <summary>
        ///   Gets the distribution's scale
        ///   parameter θ (theta).
        /// </summary>
        /// 
        public double Scale
        {
            get { return theta; }
        }

        /// <summary>
        ///   Gets the distribution's 
        ///   shape parameter k.
        /// </summary>
        /// 
        public double Shape
        {
            get { return k; }
        }

        /// <summary>
        ///   Gets the inverse scale parameter β = 1/θ.
        /// </summary>
        /// 
        public double Rate
        {
            get { return 1.0 / theta; }
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean value.</value>
        /// 
        /// <remarks>
        ///   In the Gamma distribution, the mean is computed as k*θ.
        /// </remarks>
        /// 
        public override double Mean
        {
            get { return k * theta; }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        /// <remarks>
        ///   In the Gamma distribution, the variance is computed as k*θ².
        /// </remarks>
        /// 
        public override double Variance
        {
            get { return k * theta * theta; }
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
                if (k > 1)
                    return (k - 1) * theta;

                return Double.NaN;
            }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's entropy.</value>
        /// 
        public override double Entropy
        {
            get { return k + Math.Log(theta) + Gamma.Log(k) + (1 - k) * Gamma.Digamma(k); }
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
            get { return new DoubleRange(0, Double.PositiveInfinity); }
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
        ///   
        /// <para>
        ///   The Gamma's CDF is computed in terms of the <see cref="Gamma.LowerIncomplete">
        ///   Lower Incomplete Regularized Gamma Function P</see> as <c>CDF(x) = P(shape, 
        ///   x / scale)</c>.</para>
        /// </remarks>
        /// 
        /// <example>
        ///   See <see cref="GammaDistribution"/>.
        /// </example>
        /// 
        protected internal override double InnerDistributionFunction(double x)
        {
            return Gamma.LowerIncomplete(k, x / theta);
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
        /// <example>
        ///   See <see cref="GammaDistribution"/>.
        /// </example>
        /// 
        protected internal override double InnerProbabilityDensityFunction(double x)
        {
            return Math.Exp(LogProbabilityDensityFunction(x));
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
        /// <example>
        ///   See <see cref="GammaDistribution"/>.
        /// </example>
        /// 
        protected internal override double InnerLogProbabilityDensityFunction(double x)
        {
            return lnconstant + (k - 1) * Math.Log(x) - x / theta;
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
        ///   value when applied in the <see cref="UnivariateContinuousDistribution.DistributionFunction(double)"/>.
        /// </returns>
        /// 
        protected internal override double InnerInverseDistributionFunction(double p)
        {
            return Gamma.InverseLowerIncomplete(k, p) * theta;
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
            Fit(observations, weights, (GammaOptions)options);
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
        public void Fit(double[] observations, double[] weights, GammaOptions options)
        {
            if (immutable)
                throw new InvalidOperationException("This object can not be modified.");

            if (weights != null)
                throw new ArgumentException("This distribution does not support weighted samples.");

            // Method from Choi, S.C.; Wette, R. (1969) "Maximum Likelihood Estimation 
            // of the Parameters of the Gamma Distribution and Their Bias", Technometrics,
            // 11(4) 683–690

            double lnsum = 0;
            for (int i = 0; i < observations.Length; i++)
                lnsum += Math.Log(observations[i]);

            double mean = observations.Mean();

            double s = Math.Log(mean) - lnsum / observations.Length;

            if (Double.IsNaN(s))
                throw new ArgumentException("Observation vector contains negative values.", "observations");

            // initial approximation
            double newK = (3 - s + Math.Sqrt((s - 3) * (s - 3) + 24 * s)) / (12 * s);

            double tol = 1e-8;
            int maxIter = 1000;

            if (options != null)
            {
                tol = options.Tolerance;
                maxIter = options.Iterations;
            }

            // Use Newton-Raphson update
            double oldK;

            for (int i = 0; i < maxIter; i++)
            {
                oldK = newK;
                double num = Math.Log(newK) - Gamma.Digamma(newK) - s;
                double den = (1 / newK) - Gamma.Trigamma(newK);
                newK = oldK - num / den;

                if (oldK.IsEqual(newK, rtol: tol))
                    break;
            }

            double theta = mean / newK;

            init(theta, newK);
        }

        /// <summary>
        ///   Estimates a new Gamma distribution from a given set of observations.
        /// </summary>
        /// 
        public static GammaDistribution Estimate(double[] observations,
            double[] weights = null, double tol = 1e-8, int iterations = 1000)
        {
            var n = new GammaDistribution();
            n.Fit(observations, weights, new GammaOptions()
            {
                Tolerance = tol,
                Iterations = iterations
            });
            return n;
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
            return new GammaDistribution(theta, k);
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
            return Random(k, theta, samples, source);
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public override double Generate(Random source)
        {
            return Random(k, theta, source);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Gamma distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="scale">The scale parameter theta (or inverse beta).</param>
        /// <param name="shape">The shape parameter k (or alpha).</param>
        /// <param name="samples">The number of samples to generate.</param>
        ///
        /// <returns>An array of double values sampled from the specified Gamma distribution.</returns>
        /// 
        public static double[] Random(double shape, double scale, int samples)
        {
            return Random(shape, scale, samples, new double[samples], Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Gamma distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="scale">The scale parameter theta (or inverse beta).</param>
        /// <param name="shape">The shape parameter k (or alpha).</param>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///
        /// <returns>An array of double values sampled from the specified Gamma distribution.</returns>
        /// 
        public static double[] Random(double shape, double scale, int samples, Random source)
        {
            return Random(shape, scale, samples, new double[samples], source);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Gamma distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="scale">The scale parameter theta (or inverse beta).</param>
        /// <param name="shape">The shape parameter k (or alpha).</param>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        ///
        /// <returns>An array of double values sampled from the specified Gamma distribution.</returns>
        /// 
        public static double[] Random(double shape, double scale, int samples, double[] result)
        {
            return Random(shape, scale, samples, result, Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Gamma distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="scale">The scale parameter theta (or inverse beta).</param>
        /// <param name="shape">The shape parameter k (or alpha).</param>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///
        /// <returns>An array of double values sampled from the specified Gamma distribution.</returns>
        /// 
        public static double[] Random(double shape, double scale, int samples, double[] result, Random source)
        {
            if (shape < 1)
            {
                double d = shape + 1.0 - 1.0 / 3.0;
                double c = (1.0 / 3.0) / Math.Sqrt(d);

                for (int i = 0; i < samples; i++)
                    result[i] = scale * Marsaglia(d, c) * Math.Pow(source.NextDouble(), 1.0 / shape);
            }
            else
            {
                double d = shape - 1.0 / 3.0;
                double c = (1.0 / 3.0) / Math.Sqrt(d);

                for (int i = 0; i < samples; i++)
                    result[i] = scale * Marsaglia(d, c);
            }

            return result;
        }

        /// <summary>
        ///   Generates a random observation from the 
        ///   Gamma distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="scale">The scale parameter theta (or inverse beta).</param>
        /// <param name="shape">The shape parameter k (or alpha).</param>
        /// 
        /// <returns>A random double value sampled from the specified Gamma distribution.</returns>
        /// 
        public static double Random(double shape, double scale)
        {
            return Random(shape, scale, Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        ///   Generates a random observation from the 
        ///   Gamma distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="scale">The scale parameter theta (or inverse beta).</param>
        /// <param name="shape">The shape parameter k (or alpha).</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        /// 
        /// <returns>A random double value sampled from the specified Gamma distribution.</returns>
        /// 
        public static double Random(double shape, double scale, Random source)
        {
            if (shape < 1)
            {
                double d = shape + 1.0 - 1.0 / 3.0;
                double c = (1.0 / 3.0) / Math.Sqrt(d);

                double u = source.NextDouble();
                return scale * Marsaglia(d, c) * Math.Pow(u, 1.0 / shape);
            }
            else
            {
                double d = shape - 1.0 / 3.0;
                double c = (1.0 / 3.0) / Math.Sqrt(d);

                return scale * Marsaglia(d, c);
            }
        }

        /// <summary>
        ///   Random Gamma-distribution number generation 
        ///   based on Marsaglia's Simple Method (2000).
        /// </summary>
        /// 
        public static double Marsaglia(double d, double c)
        {
            return Marsaglia(d, c, Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        ///   Random Gamma-distribution number generation 
        ///   based on Marsaglia's Simple Method (2000).
        /// </summary>
        /// 
        public static double Marsaglia(double d, double c, Random source)
        {
            // References:
            //
            // - Marsaglia, G. A Simple Method for Generating Gamma Variables, 2000
            //

            while (true)
            {
                // 2. Generate v = (1+cx)^3 with x normal
                double x, t, v;

                do
                {
                    x = NormalDistribution.Random();
                    t = (1.0 + c * x);
                    v = t * t * t;
                } while (v <= 0);


                // 3. Generate uniform U
                double U = source.NextDouble();

                // 4. If U < 1-0.0331*x^4 return d*v.
                double x2 = x * x;
                if (U < 1 - 0.0331 * x2 * x2)
                    return d * v;

                // 5. If log(U) < 0.5*x^2 + d*(1-v+log(v)) return d*v.
                if (Math.Log(U) < 0.5 * x2 + d * (1.0 - v + Math.Log(v)))
                    return d * v;

                // 6. Goto step 2
            }
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
            return String.Format(formatProvider, "Γ(x; k = {0}, θ = {1})",
                k.ToString(format, formatProvider),
                theta.ToString(format, formatProvider));
        }


        /// <summary>
        ///   Gets the standard Gamma distribution,
        ///   with scale θ = 1 and location k = 1.
        /// </summary>
        /// 
        public static GammaDistribution Standard { get { return standard; } }

        private static readonly GammaDistribution standard = new GammaDistribution() { immutable = true };

    }
}
