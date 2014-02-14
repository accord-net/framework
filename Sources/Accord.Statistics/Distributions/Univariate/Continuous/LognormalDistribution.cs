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
    ///   Log-Normal (Galton) distribution.
    /// </summary>
    /// 
    /// <remarks>
    ///  <para>
    ///   The log-normal distribution is a probability distribution of a random
    ///   variable whose logarithm is normally distributed.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia, The Free Encyclopedia. Log-normal distribution.
    ///       Available on: http://en.wikipedia.org/wiki/Log-normal_distribution </description></item>
    ///     <item><description>
    ///       NIST/SEMATECH e-Handbook of Statistical Methods. Lognormal Distribution.
    ///       Available on: http://www.itl.nist.gov/div898/handbook/eda/section3/eda3669.htm </description></item>
    ///     <item><description>
    ///       Weisstein, Eric W. "Normal Distribution Function." From MathWorld--A Wolfram Web
    ///       Resource. http://mathworld.wolfram.com/NormalDistributionFunction.html </description></item>
    ///  </list></para>  
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///   // Create a new Log-normal distribution with μ = 2.79 and σ = 1.10
    ///   var log = new LognormalDistribution(location: 0.42, shape: 1.1);
    /// 
    ///   // Common measures
    ///   double mean = log.Mean;     // 2.7870954605658511
    ///   double median = log.Median; // 1.5219615583481305
    ///   double var = log.Variance;  // 18.28163603621158
    /// 
    ///   // Cumulative distribution functions
    ///   double cdf = log.DistributionFunction(x: 0.27);           // 0.057961222885664958
    ///   double ccdf = log.ComplementaryDistributionFunction(x: 0.27); // 0.942038777114335
    ///   double icdf = log.InverseDistributionFunction(p: cdf);        // 0.26999997937815973
    ///   
    ///   // Probability density functions
    ///   double pdf = log.ProbabilityDensityFunction(x: 0.27);     // 0.39035530085982068
    ///   double lpdf = log.LogProbabilityDensityFunction(x: 0.27); // -0.94069792674674835
    /// 
    ///   // Hazard (failure rate) functions
    ///   double hf = log.HazardFunction(x: 0.27);            // 0.41437285846720867
    ///   double chf = log.CumulativeHazardFunction(x: 0.27); // 0.059708840588116374
    /// 
    ///   // String representation
    ///   string str = log.ToString("N2", CultureInfo.InvariantCulture); // Lognormal(x; μ = 2.79, σ = 1.10)
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="NormalDistribution"/>
    /// 
    [Serializable]
    public class LognormalDistribution : UnivariateContinuousDistribution,
        IFittableDistribution<double, NormalOptions>,
        ISampleableDistribution<double>, IFormattable
    {

        // Distribution parameters
        private double location = 0; // mean of the variable's natural logarithm
        private double shape = 1;    // std. dev. of the variable's natural logarithm

        // Distribution measures
        private double? mean;
        private double? variance;
        private double? entropy;

        // Derived measures
        private double shape2; // variance of the variable's natural logarithm
        private double constant; // 1/sqrt(2*pi*shape²)

        private bool immutable;


        /// <summary>
        ///   Constructs a Log-Normal (Galton) distribution
        ///   with zero location and unit shape.
        /// </summary>
        /// 
        public LognormalDistribution()
        {
            initialize(location, shape, shape * shape);
        }

        /// <summary>
        ///   Constructs a Log-Normal (Galton) distribution
        ///   with given location and unit shape.
        /// </summary>
        /// 
        /// <param name="location">The distribution's location value μ (mu).</param>
        /// 
        public LognormalDistribution(double location)
        {
            initialize(location, shape, shape * shape);
        }

        /// <summary>
        ///   Constructs a Log-Normal (Galton) distribution
        ///   with given mean and standard deviation.
        /// </summary>
        /// 
        /// <param name="location">The distribution's location value μ (mu).</param>
        /// <param name="shape">The distribution's shape deviation σ (sigma).</param>
        /// 
        public LognormalDistribution(double location, double shape)
        {
            initialize(location, shape, shape * shape);
        }

        /// <summary>
        ///   Shape parameter σ (sigma) of 
        ///   the log-normal distribution. 
        /// </summary>
        /// 
        public double Shape
        {
            get { return shape; }
        }

        /// <summary>
        ///   Squared shape parameter σ² (sigma-squared)
        ///   of the log-normal distribution. 
        /// </summary>
        /// 
        public double Shape2
        {
            get { return shape2; }
        }

        /// <summary>
        ///   Location parameter μ (mu) of the log-normal distribution.
        /// </summary>
        /// 
        public double Location
        {
            get { return location; }
        }

        /// <summary>
        ///   Gets the Mean for this Log-Normal distribution.
        /// </summary>
        /// 
        /// <remarks>
        ///   The Lognormal distribution's mean is 
        ///   defined as <c>exp(μ + σ²/2).</c>
        /// </remarks>
        /// 
        public override double Mean
        {
            get
            {
                if (mean == null)
                    mean = Math.Exp(location + shape2 / 2.0);
                return mean.Value;
            }
        }

        /// <summary>
        ///   Gets the Variance (the square of the standard
        ///   deviation) for this Log-Normal distribution.
        /// </summary>
        /// 
        /// <remarks>
        ///   The Lognormal distribution's variance is
        ///   defined as <c>(exp(σ²) - 1) * exp(2*μ + σ²)</c>.
        /// </remarks>
        /// 
        public override double Variance
        {
            get
            {
                if (variance == null)
                    variance = (Math.Exp(shape2) - 1.0) * Math.Exp(2 * location + shape2);
                return variance.Value;
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
            get { return new DoubleRange(0, Double.PositiveInfinity); }
        }

        /// <summary>
        ///   Gets the Entropy for this Log-Normal distribution.
        /// </summary>
        /// 
        public override double Entropy
        {
            get
            {
                if (entropy == null)
                    entropy = 0.5 + 0.5 * Math.Log(2.0 * Math.PI * shape2) + location;
                return entropy.Value;
            }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   the this Log-Normal distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range.</param>
        /// <remarks>
        /// 
        /// <para>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.</para>
        ///   
        /// <para>
        ///  The calculation is computed through the relationship to the error function
        ///  as <see cref="Accord.Math.Special.Erfc">erfc</see>(-z/sqrt(2)) / 2. See 
        ///  [Weisstein] for more details.</para>  
        ///  
        /// <para>    
        ///   References:
        ///   <list type="bullet">
        ///     <item><description>
        ///       Weisstein, Eric W. "Normal Distribution Function." From MathWorld--A Wolfram Web
        ///       Resource. http://mathworld.wolfram.com/NormalDistributionFunction.html </description></item>
        ///   </list></para>
        /// </remarks>
        /// 
        /// <example>
        ///   See <see cref="LognormalDistribution"/>.
        /// </example>
        /// 
        public override double DistributionFunction(double x)
        {
            double z = (Math.Log(x) - location) / shape;
            return 0.5 * Special.Erfc(-z / Constants.Sqrt2);
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   the Normal distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range. For a
        ///   univariate distribution, this should be a single
        ///   double value. For a multivariate distribution,
        ///   this should be a double array.</param>
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
        ///   See <see cref="LognormalDistribution"/>.
        /// </example>
        /// 
        public override double ProbabilityDensityFunction(double x)
        {
            double z = (Math.Log(x) - location) / shape;
            return constant * Math.Exp((-z * z) * 0.5) / x;
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
        ///   See <see cref="LognormalDistribution"/>.
        /// </example>
        /// 
        public override double LogProbabilityDensityFunction(double x)
        {
            double z = (Math.Log(x) - location) / shape;
            return Math.Log(constant) + (-z * z) * 0.5 - Math.Log(x);
        }



        /// <summary>
        ///   Gets the Standard Log-Normal Distribution,
        ///   with location set to zero and unit shape.
        /// </summary>
        /// 
        public static LognormalDistribution Standard { get { return standard; } }

        private static readonly LognormalDistribution standard = new LognormalDistribution() { immutable = true };


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
        public override void Fit(double[] observations, double[] weights, IFittingOptions options)
        {
            NormalOptions normalOptions = options as NormalOptions;
            if (options != null && normalOptions == null)
                throw new ArgumentException("The specified options' type is invalid.", "options");

            Fit(observations, weights, normalOptions);
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
        public void Fit(double[] observations, double[] weights, NormalOptions options)
        {
            if (immutable) throw new InvalidOperationException();

            double mu, var;

            observations = Matrix.Log(observations);

            if (weights != null)
            {
#if DEBUG
                for (int i = 0; i < weights.Length; i++)
                    if (Double.IsNaN(weights[i]) || Double.IsInfinity(weights[i]))
                        throw new ArgumentException("Invalid numbers in the weight vector.", "weights");
#endif

                // Compute weighted mean
                mu = Statistics.Tools.WeightedMean(observations, weights);

                // Compute weighted variance
                var = Statistics.Tools.WeightedVariance(observations, weights, mu);
            }
            else
            {
                // Compute weighted mean
                mu = Statistics.Tools.Mean(observations);

                // Compute weighted variance
                var = Statistics.Tools.Variance(observations, mu);
            }

            if (options != null)
            {
                // Parse optional estimation options
                double regularization = options.Regularization;

                if (var == 0 || Double.IsNaN(var) || Double.IsInfinity(var))
                    var = regularization;
            }

            initialize(mu, Math.Sqrt(var), var);
        }


        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public override object Clone()
        {
            return new LognormalDistribution(location, shape);
        }


        private void initialize(double mu, double dev, double var)
        {
            this.location = mu;
            this.shape = dev;
            this.shape2 = var;

            // Compute derived values
            this.constant = 1.0 / (Constants.Sqrt2PI * dev);
        }


        /// <summary>
        ///   Estimates a new Log-Normal distribution from a given set of observations.
        /// </summary>
        /// 
        public static LognormalDistribution Estimate(double[] observations)
        {
            return Estimate(observations, null, null);
        }

        /// <summary>
        ///   Estimates a new Log-Normal distribution from a given set of observations.
        /// </summary>
        /// 
        public static LognormalDistribution Estimate(double[] observations, NormalOptions options)
        {
            return Estimate(observations, null, options);
        }

        /// <summary>
        ///   Estimates a new Log-Normal distribution from a given set of observations.
        /// </summary>
        /// 
        public static LognormalDistribution Estimate(double[] observations, double[] weights, NormalOptions options = null)
        {
            LognormalDistribution n = new LognormalDistribution();
            n.Fit(observations, weights, options);
            return n;
        }

        #region ISampleableDistribution<double> Members

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// 
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public double[] Generate(int samples)
        {
            return Random(location, shape, samples);
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public double Generate()
        {
            return Random(location, shape);
        }

        /// <summary>
        ///   Generates a random observation from the 
        ///   Lognormal distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="location">The distribution's location value.</param>
        /// <param name="shape">The distribution's shape deviation.</param>
        /// 
        /// <returns>A random double value sampled from the specified Lognormal distribution.</returns>
        /// 
        public static double Random(double location, double shape)
        {
            double x = NormalDistribution.Standard.Generate();
            return Math.Exp(location + shape * x);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Lognormal distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="location">The distribution's location value.</param>
        /// <param name="shape">The distribution's shape deviation.</param>
        /// <param name="samples">The number of samples to generate.</param>
        ///
        /// <returns>An array of double values sampled from the specified Lognormal distribution.</returns>
        /// 
        public static double[] Random(double location, double shape, int samples)
        {
            double[] x = NormalDistribution.Standard.Generate(samples);
            for (int i = 0; i < x.Length; i++)
                x[i] = Math.Exp(location + shape * x[i]);
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
            return String.Format("Lognormal(x; μ = {0}, σ = {1})", Mean, Shape);
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
            return String.Format(formatProvider, "Lognormal(x; μ = {0}, σ = {1})", Mean, Shape);
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
            return String.Format(formatProvider, "Lognormal(x; μ = {0}, σ = {1})",
                Mean.ToString(format, formatProvider),
                Shape.ToString(format, formatProvider));
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
            return String.Format("Lognormal(x; μ = {0}, σ = {1})",
                Mean.ToString(format), Shape.ToString(format));
        }
    }
}
