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
    using System.Linq;
    using System.Text;
    using Accord.Math;
    using Accord.Statistics.Distributions.Fitting;
    using AForge;

    /// <summary>
    ///   Mixture of univariate probability distributions.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   A mixture density is a probability density function which is expressed
    ///   as a convex combination (i.e. a weighted sum, with non-negative weights
    ///   that sum to 1) of other probability density functions. The individual
    ///   density functions that are combined to make the mixture density are
    ///   called the mixture components, and the weights associated with each
    ///   component are called the mixture weights.</para>
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Mixture_density">
    ///       Wikipedia, The Free Encyclopedia. Mixture density. Available on:
    ///       http://en.wikipedia.org/wiki/Mixture_density </a></description></item>
    ///   </list></para>
    /// </remarks>
    ///   
    /// <typeparam name="T">
    ///   The type of the univariate component distributions.</typeparam>
    ///   
    /// <example>
    /// <code>
    ///   // Create a new mixture containing two Normal distributions
    ///   Mixture&lt;NormalDistribution> mix = new Mixture&lt;NormalDistribution>(
    ///       new NormalDistribution(2, 1), new NormalDistribution(5, 1));
    ///   
    ///   // Common measures
    ///   double mean   = mix.Mean;     // 3.5
    ///   double median = mix.Median;   // 3.4999998506015895
    ///   double var    = mix.Variance; // 3.25
    ///   
    ///   // Cumulative distribution functions
    ///   double cdf = mix.DistributionFunction(x: 4.2);               // 0.59897597553494908
    ///   double ccdf = mix.ComplementaryDistributionFunction(x: 4.2); // 0.40102402446505092
    ///   
    ///   // Probability mass functions
    ///   double pmf1 = mix.ProbabilityDensityFunction(x: 1.2); // 0.14499174984363708
    ///   double pmf2 = mix.ProbabilityDensityFunction(x: 2.3); // 0.19590437513747333
    ///   double pmf3 = mix.ProbabilityDensityFunction(x: 3.7); // 0.13270883471234715
    ///   double lpmf = mix.LogProbabilityDensityFunction(x: 4.2); // -1.8165661905848629
    ///   
    ///   // Quantile function
    ///   double icdf1 = mix.InverseDistributionFunction(p: 0.17); // 1.5866611690305095
    ///   double icdf2 = mix.InverseDistributionFunction(p: 0.46); // 3.1968506765456883
    ///   double icdf3 = mix.InverseDistributionFunction(p: 0.87); // 5.6437596300843076
    ///   
    ///   // Hazard (failure rate) functions
    ///   double hf = mix.HazardFunction(x: 4.2);            // 0.40541978256972522
    ///   double chf = mix.CumulativeHazardFunction(x: 4.2); // 0.91373394208601633
    ///   
    ///   // String representation:
    ///   // Mixture(x; 0.5 * N(x; μ = 5, σ² = 1) + 0.5 * N(x; μ = 5, σ² = 1))
    ///   string str = mix.ToString(CultureInfo.InvariantCulture);
    /// </code>
    /// </example>
    ///   
    [Serializable]
    public class Mixture<T> : UnivariateContinuousDistribution, IMixture<T>,
        IFittableDistribution<double, MixtureOptions>,
        ISampleableDistribution<double>
        where T : IUnivariateDistribution<double>
    {

        // distribution parameters
        private double[] coefficients;
        private T[] components;

        // distributions measures
        private double? mean;
        private double? variance;

        // cache
        IDistribution<double>[] cache;


        /// <summary>
        ///   Initializes a new instance of the <see cref="Mixture&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="components">The mixture distribution components.</param>
        /// 
        public Mixture(params T[] components)
        {
            if (components == null)
                throw new ArgumentNullException("components");


            this.components = components;

            this.coefficients = new double[components.Length];
            for (int i = 0; i < coefficients.Length; i++)
                coefficients[i] = 1.0 / coefficients.Length;

            this.cache = new IDistribution<double>[coefficients.Length];
            for (int i = 0; i < cache.Length; i++)
                cache[i] = components[i];

            this.initialize();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Mixture&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="coefficients">The mixture weight coefficients.</param>
        /// <param name="components">The mixture distribution components.</param>
        /// 
        public Mixture(double[] coefficients, params T[] components)
        {
            if (components == null)
                throw new ArgumentNullException("components");

            if (coefficients == null)
                throw new ArgumentNullException("coefficients");

            if (coefficients.Length != components.Length)
                throw new ArgumentException(
                    "The coefficient and component arrays should have the same length.",
                    "components");

            this.components = components;
            this.coefficients = coefficients;

            this.cache = new IDistribution<double>[coefficients.Length];
            for (int i = 0; i < cache.Length; i++)
                cache[i] = components[i];

            this.initialize();
        }

        private void initialize()
        {
            this.mean = null;
            this.variance = null;
        }

        /// <summary>
        ///   Gets the mixture components.
        /// </summary>
        /// 
        public T[] Components
        {
            get { return components; }
        }

        /// <summary>
        ///   Gets the weight coefficients.
        /// </summary>
        /// 
        public double[] Coefficients
        {
            get { return coefficients; }
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
            double r = 0.0;
            for (int i = 0; i < components.Length; i++)
                r += coefficients[i] * components[i].ProbabilityFunction(x);
            return r;
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
            double r = 0.0;
            for (int i = 0; i < components.Length; i++)
                r += coefficients[i] * components[i].ProbabilityFunction(x);
            return Math.Log(r);
        }

        /// <summary>
        ///   This method is not supported.
        /// </summary>
        /// 
        public override double DistributionFunction(double x)
        {
            double r = 0.0;
            for (int i = 0; i < components.Length; i++)
                r += coefficients[i] * components[i].DistributionFunction(x);
            return r;
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
        public override void Fit(double[] observations, double[] weights, IFittingOptions options)
        {
            MixtureOptions mixOptions = options as MixtureOptions;
            if (options != null && mixOptions == null)
                throw new ArgumentException("The specified options' type is invalid.", "options");

            Fit(observations, weights, mixOptions);
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
        public void Fit(double[] observations, double[] weights, MixtureOptions options)
        {
            var pdf = new IFittableDistribution<double>[coefficients.Length];
            for (int i = 0; i < components.Length; i++)
                pdf[i] = (IFittableDistribution<double>)components[i];

            bool log = (options != null && options.Logarithm);

            if (log)
            {
                if (weights != null)
                {
                    throw new ArgumentException("The model fitting algorithm does not"
                    + " currently support different weights when the logarithm option"
                    + " is enabled. To avoid this exception, pass 'null' as the second"
                    + " parameter's value when calling this method.");
                }

                var em = new LogExpectationMaximization<double>(coefficients, pdf);

                if (options != null)
                {
                    em.InnerOptions = options.InnerOptions;
                    em.Convergence.Iterations = options.Iterations;
                    em.Convergence.Tolerance = options.Threshold;
                }

                em.Compute(observations);
            }
            else
            {
                var em = new ExpectationMaximization<double>(coefficients, pdf);

                if (options != null)
                {
                    em.InnerOptions = options.InnerOptions;
                    em.Convergence.Iterations = options.Iterations;
                    em.Convergence.Tolerance = options.Threshold;
                }

                em.Compute(observations, weights);
            }

            for (int i = 0; i < components.Length; i++)
                cache[i] = components[i] = (T)pdf[i];

            this.initialize();
        }

        /// <summary>
        ///   Computes the log-likelihood of the distribution
        ///   for a given set of observations.
        /// </summary>
        /// 
        public double LogLikelihood(double[] observations, double[] weights)
        {
            return ExpectationMaximization<double>.LogLikelihood(coefficients, cache, observations,
                weights, weights.Sum());
        }

        /// <summary>
        ///   Computes the log-likelihood of the distribution
        ///   for a given set of observations.
        /// </summary>
        /// 
        public double LogLikelihood(double[] observations)
        {
            return ExpectationMaximization<double>.LogLikelihood(coefficients, cache, observations);
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
            // Clone the mixture coefficients
            double[] pi = (double[])coefficients.Clone();

            // Clone the mixture components
            T[] pdf = new T[components.Length];
            for (int i = 0; i < components.Length; i++)
                pdf[i] = (T)components[i].Clone();

            return new Mixture<T>(pi, pdf);
        }


        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        public override double Mean
        {
            get
            {
                if (mean == null)
                {
                    double mu = 0.0;
                    for (int i = 0; i < coefficients.Length; i++)
                        mu += coefficients[i] * components[i].Mean;
                    this.mean = mu;
                }

                return mean.Value;
            }
        }


        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <remarks>
        ///   References: Lidija Trailovic and Lucy Y. Pao, Variance Estimation and
        ///   Ranking of Gaussian Mixture Distributions in Target Tracking
        ///   Applications, Department of Electrical and Computer Engineering
        /// </remarks>
        /// 
        public override double Variance
        {
            get
            {
                if (variance == null)
                {
                    double mu = Mean;
                    double var = 0.0;
                    for (int i = 0; i < coefficients.Length; i++)
                    {
                        double w = coefficients[i];
                        double m = components[i].Mean;
                        double v = components[i].Variance;
                        var += w * (v + m * m);
                    }

                    this.variance = var - mu * mu;
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
        ///   A <see cref="AForge.DoubleRange" /> containing
        ///   the support interval for this distribution.
        /// </value>
        /// 
        public override DoubleRange Support
        {
            get
            {
                double min = Components.Min(p => p.Support.Min);
                double max = Components.Max(p => p.Support.Max);
                return new DoubleRange(min, max);
            }
        }

        /// <summary>
        ///   Estimates a new mixture model from a given set of observations.
        /// </summary>
        /// 
        /// <param name="data">A set of observations.</param>
        /// <param name="components">The initial components of the mixture model.</param>
        /// <returns>Returns a new Mixture fitted to the given observations.</returns>
        /// 
        public static Mixture<T> Estimate(double[] data, params T[] components)
        {
            var mixture = new Mixture<T>(components);
            mixture.Fit(data);
            return mixture;
        }

        /// <summary>
        ///   Estimates a new mixture model from a given set of observations.
        /// </summary>
        /// 
        /// <param name="data">A set of observations.</param>
        /// <param name="coefficients">The initial mixture coefficients.</param>
        /// <param name="components">The initial components of the mixture model.</param>
        /// <returns>Returns a new Mixture fitted to the given observations.</returns>
        /// 
        public static Mixture<T> Estimate(double[] data, double[] coefficients, params T[] components)
        {
            var mixture = new Mixture<T>(coefficients, components);
            mixture.Fit(data);
            return mixture;
        }

        /// <summary>
        ///   Estimates a new mixture model from a given set of observations.
        /// </summary>
        /// 
        /// <param name="data">A set of observations.</param>
        /// <param name="coefficients">The initial mixture coefficients.</param>
        /// <param name="threshold">The convergence threshold for the Expectation-Maximization estimation.</param>
        /// <param name="components">The initial components of the mixture model.</param>
        /// <returns>Returns a new Mixture fitted to the given observations.</returns>
        /// 
        public static Mixture<T> Estimate(double[] data, double threshold, double[] coefficients, params T[] components)
        {
            var mixture = new Mixture<T>(coefficients, components);
            mixture.Fit(data, new MixtureOptions(threshold));
            return mixture;
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
            double[] r = new double[samples];
            r.ApplyInPlace(x => Generate());
            return r;
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public double Generate()
        {
            // Choose one coefficient at random
            int c = GeneralDiscreteDistribution.Random(coefficients);

            // Sample from the chosen coefficient
            var d = components[c] as ISampleableDistribution<double>;

            if (d == null) throw new InvalidOperationException();

            return d.Generate();
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
            StringBuilder sb = new StringBuilder();
            sb.Append("Mixture(x; ");

            for (int i = 0; i < coefficients.Length; i++)
            {
                sb.AppendFormat("{0}*{1}", coefficients[0], components[1].ToString());
                if (i < coefficients.Length - 1)
                    sb.Append(" + ");
            }
            sb.Append(")");

            return sb.ToString();
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
            StringBuilder sb = new StringBuilder();
            sb.Append("Mixture(x; ");

            for (int i = 0; i < coefficients.Length; i++)
            {
                sb.AppendFormat("{0}*{1}",
                    coefficients[0].ToString(formatProvider),
                    components[1].ToString());

                if (i < coefficients.Length - 1)
                    sb.Append(" + ");
            }
            sb.Append(")");

            return sb.ToString();
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
            StringBuilder sb = new StringBuilder();
            sb.Append("Mixture(x; ");

            for (int i = 0; i < coefficients.Length; i++)
            {
                sb.AppendFormat("{0}*{1}",
                    coefficients[0].ToString(format, formatProvider),
                    components[1].ToString());

                if (i < coefficients.Length - 1)
                    sb.Append(" + ");
            }
            sb.Append(")");

            return sb.ToString();
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
            StringBuilder sb = new StringBuilder();
            sb.Append("Mixture(x; ");

            for (int i = 0; i < coefficients.Length; i++)
            {
                sb.AppendFormat("{0}*{1}",
                    coefficients[0].ToString(format),
                    components[1].ToString());

                if (i < coefficients.Length - 1)
                    sb.Append(" + ");
            }
            sb.Append(")");

            return sb.ToString();
        }
    }
}
