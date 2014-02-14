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
    using Accord.Math.Optimization;
    using Accord.Statistics.Distributions.Fitting;
    using AForge;

    /// <summary>
    ///   Cauchy-Lorentz distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Cauchy distribution, named after Augustin Cauchy, is a continuous probability
    ///   distribution. It is also known, especially among physicists, as the Lorentz
    ///   distribution (after Hendrik Lorentz), Cauchy–Lorentz distribution, Lorentz(ian)
    ///   function, or Breit–Wigner distribution. The simplest Cauchy distribution is called
    ///   the standard Cauchy distribution. It has the distribution of a random variable that
    ///   is the ratio of two independent standard normal random variables. </para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Cauchy_distribution">
    ///       Wikipedia, The Free Encyclopedia. Cauchy distribution. 
    ///       Available from: http://en.wikipedia.org/wiki/Cauchy_distribution </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example demonstrates how to instantiate a Cauchy distribution
    ///   with a given location parameter x0 and scale parameter γ (gamma), calculating
    ///   its main properties and characteristics: </para>
    ///   
    /// <code>
    ///   double location = 0.42;
    ///   double scale = 1.57;
    ///   
    ///   // Create a new Cauchy distribution with x0 = 0.42 and γ = 1.57 
    ///   CauchyDistribution cauchy = new CauchyDistribution(location, scale);
    ///   
    ///   // Common measures
    ///   double mean = cauchy.Mean;     // NaN - Cauchy's mean is undefined.
    ///   double var = cauchy.Variance;  // NaN - Cauchy's variance is undefined.
    ///   double median = cauchy.Median; // 0.42
    ///   
    ///   // Cumulative distribution functions
    ///   double cdf = cauchy.DistributionFunction(x: 0.27);           // 0.46968025841608563
    ///   double ccdf = cauchy.ComplementaryDistributionFunction(x: 0.27);          // 0.53031974158391437
    ///   double icdf = cauchy.InverseDistributionFunction(p: 0.69358638272337991); // 1.5130304686978195
    ///   
    ///   // Probability density functions
    ///   double pdf = cauchy.ProbabilityDensityFunction(x: 0.27);     // 0.2009112009763413
    ///   double lpdf = cauchy.LogProbabilityDensityFunction(x: 0.27); // -1.6048922547266871
    ///   
    ///   // Hazard (failure rate) functions
    ///   double hf = cauchy.HazardFunction(x: 0.27); // 0.3788491832800277
    ///   double chf = cauchy.CumulativeHazardFunction(x: 0.27); // 0.63427516833243092
    ///   
    ///   // String representation
    ///   string str = cauchy.ToString(CultureInfo.InvariantCulture); // "Cauchy(x; x0 = 0.42, γ = 1.57)
    /// </code>
    /// 
    /// <para>
    ///   The following example shows how to fit a Cauchy distribution (estimate its
    ///   location and shape parameters) given a set of observation values. </para>
    ///   
    /// <code>
    ///   // Create an initial distribution
    ///   CauchyDistribution cauchy = new CauchyDistribution();
    ///   
    ///   // Consider a vector of univariate observations
    ///   double[] observations = { 0.25, 0.12, 0.72, 0.21, 0.62, 0.12, 0.62, 0.12 };
    /// 
    ///   // Fit to the observations
    ///   cauchy.Fit(observations);
    ///     
    ///   // Check estimated values
    ///   double location = cauchy.Location; //  0.18383
    ///   double gamma    = cauchy.Scale;    // -0.10530
    /// </code>
    /// 
    /// <para>
    ///   It is also possible to estimate only some of the Cauchy parameters at
    ///   a time. For this, you can specify a <see cref="CauchyOptions"/> object
    ///   and pass it alongside the observations:</para>
    ///   
    /// <code>
    ///   // Create options to estimate location only
    ///   CauchyOptions options = new CauchyOptions()
    ///   {
    ///       EstimateLocation = true,
    ///       EstimateScale = false
    ///   };
    /// 
    ///   // Create an initial distribution with a pre-defined scale
    ///   CauchyDistribution cauchy = new CauchyDistribution(location: 0, scale: 4.2);
    ///
    ///   // Fit to the observations
    ///   cauchy.Fit(observations, options);
    ///
    ///   // Check estimated values
    ///   double location = cauchy.Location; //  0.3471218110202
    ///   double gamma    = cauchy.Scale;    //  4.2 (unchanged)
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="CauchyOptions"/>
    /// <seealso cref="WrappedCauchyDistribution"/>
    /// 
    [Serializable]
    public class CauchyDistribution : UnivariateContinuousDistribution,
        IFittableDistribution<double, CauchyOptions>,
        ISampleableDistribution<double>, IFormattable
    {

        // Distribution parameters
        private double location; // x0
        private double scale;    // γ (gamma)

        // Derived measures
        private double lnconstant;
        private double constant;

        private bool immutable;


        /// <summary>
        ///   Constructs a Cauchy-Lorentz distribution
        ///   with location parameter 0 and scale 1.
        /// </summary>
        /// 
        public CauchyDistribution() : this(0, 1) { }


        /// <summary>
        ///   Constructs a Cauchy-Lorentz distribution
        ///   with given location and scale parameters.
        /// </summary>
        /// 
        /// <param name="location">The location parameter x0.</param>
        /// <param name="scale">The scale parameter gamma (γ).</param>
        /// 
        public CauchyDistribution(double location, double scale)
        {
            if (scale <= 0)
                throw new ArgumentOutOfRangeException("scale", "Scale must be greater than zero.");

            init(location, scale);
        }

        private void init(double location, double scale)
        {
            this.location = location;
            this.scale = scale;

            this.constant = 1.0 / (Math.PI * scale);
            this.lnconstant = -Math.Log(Math.PI * scale);
        }

        /// <summary>
        ///   Gets the distribution's 
        ///   location parameter x0.
        /// </summary>
        /// 
        public double Location
        {
            get { return location; }
        }

        /// <summary>
        ///   Gets the distribution's
        ///   scale parameter gamma.
        /// </summary>
        /// 
        public double Scale
        {
            get { return scale; }
        }

        /// <summary>
        ///   Gets the median for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's median value.</value>
        /// 
        /// <remarks>
        ///   The Cauchy's median is the location parameter x0.
        /// </remarks>
        /// 
        public override double Median
        {
            get
            {
                System.Diagnostics.Debug.Assert(location.IsRelativelyEqual(base.Median, 1e-6));
                return location;
            }
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
            get { return new DoubleRange(Double.NegativeInfinity, Double.PositiveInfinity); }
        }

        /// <summary>
        ///   Gets the mode for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mode value.</value>
        /// 
        /// <remarks>
        ///   The Cauchy's median is the location parameter x0.
        /// </remarks>
        /// 
        public override double Mode
        {
            get { return location; }
        }


        /// <summary>
        ///   Cauchy's mean is undefined.
        /// </summary>
        /// 
        /// <value>Undefined.</value>
        /// 
        public override double Mean
        {
            get { return Double.NaN; }
        }

        /// <summary>
        ///   Cauchy's variance is undefined.
        /// </summary>
        /// 
        /// <value>Undefined.</value>
        /// 
        public override double Variance
        {
            get { return Double.NaN; }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's entropy.</value>
        /// 
        /// <remarks>
        ///   The Cauchy's entropy is defined as log(scale) + log(4*π).
        /// </remarks>
        /// 
        public override double Entropy
        {
            get { return Math.Log(scale) + Math.Log(4.0 * Math.PI); }
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
        ///   The Cauchy's CDF is defined as CDF(x) = 1/π * atan2(x-location, scale) + 0.5.
        /// </para>
        /// </remarks>
        /// 
        public override double DistributionFunction(double x)
        {
            return 1.0 / Math.PI * Math.Atan2(x - location, scale) + 0.5;
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
        ///   
        /// <para>
        ///   The Cauchy's PDF is defined as PDF(x) = c / (1.0 + ((x-location)/scale)²) 
        ///   where the constant c is given by c = 1.0 / (π * scale);
        /// </para>
        /// </remarks>
        /// 
        public override double ProbabilityDensityFunction(double x)
        {
            double z = (x - location) / scale;
            return constant / (1.0 + z * z);
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
        /// <seealso cref="ProbabilityDensityFunction"/>
        /// 
        public override double LogProbabilityDensityFunction(double x)
        {
            double z = (x - location) / scale;
            return lnconstant - Math.Log(1.0 + z * z);
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
        /// <example>
        ///   See <see cref="CauchyDistribution"/>.
        /// </example>
        /// 
        public override void Fit(double[] observations, double[] weights, IFittingOptions options)
        {
            CauchyOptions cauchyOptions = options as CauchyOptions;
            if (options != null && cauchyOptions == null)
                throw new ArgumentException("The specified options' type is invalid.", "options");

            Fit(observations, weights, cauchyOptions);
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
        /// <example>
        ///   See <see cref="CauchyDistribution"/>.
        /// </example>
        /// 
        public void Fit(double[] observations, double[] weights, CauchyOptions options)
        {
            if (immutable)
                throw new InvalidOperationException("This object can not be modified.");

            if (weights != null)
                throw new ArgumentException("This distribution does not support weighted samples.");

            bool useMLE = true;
            bool estimateT = true;
            bool estimateS = true;

            if (options != null)
            {
                useMLE = options.MaximumLikelihood;
                estimateT = options.EstimateLocation;
                estimateS = options.EstimateScale;
            }


            double t = location;
            double s = scale;

            int n = observations.Length;


            DoubleRange range;
            double median = Accord.Statistics.Tools.Quartiles(observations, out range, alreadySorted: false);

            if (estimateT)
                t = median;

            if (estimateS)
                s = range.Length;


            if (useMLE)
            {
                // Minimize the log-likelihood through numerical optimization
                BroydenFletcherGoldfarbShanno lbfgs = new BroydenFletcherGoldfarbShanno(2);


                // Define the negative log-likelihood function,
                // which is the objective we want to minimize:
                lbfgs.Function = (parameters) =>
                {
                    // Assume location is the first
                    // parameter, shape is the second
                    if (estimateT) t = parameters[0];
                    if (estimateS) s = parameters[1];

                    if (s < 0) s = -s;

                    double sum = 0;
                    for (int i = 0; i < observations.Length; i++)
                    {
                        double y = (observations[i] - t);
                        sum += Math.Log(s * s + y * y);
                    }

                    return -(n * Math.Log(s) - sum - n * Math.Log(Math.PI));
                };


                lbfgs.Gradient = (parameters) =>
                {
                    // Assume location is the first
                    // parameter, shape is the second
                    if (estimateT) t = parameters[0];
                    if (estimateS) s = parameters[1];

                    double sum1 = 0, sum2 = 0;
                    for (int i = 0; i < observations.Length; i++)
                    {
                        double y = (observations[i] - t);
                        sum1 += y / (s * s + y * y);
                        sum2 += s / (s * s + y * y);
                    }

                    double dt = -2.0 * sum1;
                    double ds = +2.0 * sum2 - n / s;

                    double[] g = new double[2];
                    g[0] = estimateT ? dt : 0;
                    g[1] = estimateS ? ds : 0;

                    return g;
                };


                // Initialize using the sample median as starting
                // value for location, and half interquartile range
                // for shape.

                double[] values = { t, s };

                // Minimize
                double error = lbfgs.Minimize(values);

                // Check solution
                t = lbfgs.Solution[0];
                s = lbfgs.Solution[1];
            }


            init(t, s); // Become the new distribution
        }

        /// <summary>
        ///   Gets the Standard Cauchy Distribution,
        ///   with zero location and unitary shape.
        /// </summary>
        /// 
        public static CauchyDistribution Standard { get { return standard; } }

        private static readonly CauchyDistribution standard = new CauchyDistribution() { immutable = true };


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
            return new CauchyDistribution(location, scale);
        }

        #region ISamplableDistribution<double> Members

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public double[] Generate(int samples)
        {
            return Random(location, scale, samples);
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public double Generate()
        {
            return Random(location, scale);
        }

        /// <summary>
        ///   Generates a random observation from the 
        ///   Cauchy distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="location">The location parameter x0.</param>
        /// <param name="scale">The scale parameter gamma.</param>
        /// 
        /// <returns>A random double value sampled from the specified Cauchy distribution.</returns>
        /// 
        public static double Random(double location, double scale)
        {
            // Generate uniform U on [-PI/2, +PI/2]
            double x = UniformContinuousDistribution.Random(-Math.PI / 2.0, +Math.PI / 2.0);
            return Math.Tan(x) * scale + location;
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Cauchy distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="location">The location parameter x0.</param>
        /// <param name="scale">The scale parameter gamma.</param>
        /// <param name="samples">The number of samples to generate.</param>
        /// 
        /// <returns>An array of double values sampled from the specified Cauchy distribution.</returns>
        /// 
        public static double[] Random(double location, double scale, int samples)
        {
            // Generate uniform U on [-PI/2, +PI/2]
            double[] x = UniformContinuousDistribution.Random(-Math.PI / 2.0, +Math.PI / 2.0, samples);
            for (int i = 0; i < x.Length; i++)
                x[i] = Math.Tan(x[i]) * scale + location;
            return x;
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
            return String.Format("Cauchy(x; x0 = {0}, γ = {1})", location, scale);
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
            return String.Format(formatProvider, "Cauchy(x; x0 = {0}, γ = {1})", location, scale);
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
            return String.Format("Cauchy(x; x0 = {0}, γ = {1})",
                location.ToString(format, formatProvider),
                scale.ToString(format, formatProvider));
        }
    }
}
