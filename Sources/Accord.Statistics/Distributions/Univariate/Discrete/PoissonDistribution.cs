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
    ///   Poisson probability distribution.
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>The Poisson distribution is a discrete probability distribution that
    ///   expresses the probability of a number of events occurring in a fixed
    ///   period of time if these events occur with a known average rate and
    ///   independently of the time since the last event.</para>
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Poisson_distribution">
    ///       Wikipedia, The Free Encyclopedia. Poisson distribution. Available on:
    ///       http://en.wikipedia.org/wiki/Poisson_distribution </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example shows how to instantiate a new Poisson distribution
    ///   with a given rate λ and how to compute its measures and associated functions.</para>
    ///   
    /// <code>
    /// // Create a new Poisson distribution with 
    /// var dist = new PoissonDistribution(lambda: 4.2);
    /// 
    /// // Common measures
    /// double mean = dist.Mean;     // 4.2
    /// double median = dist.Median; // 4.0
    /// double var = dist.Variance;  // 4.2
    /// 
    /// // Cumulative distribution functions
    /// double cdf1 = dist.DistributionFunction(k: 2); // 0.21023798702309743
    /// double cdf2 = dist.DistributionFunction(k: 4); // 0.58982702131057763
    /// double cdf3 = dist.DistributionFunction(k: 7); // 0.93605666027257894
    /// double ccdf = dist.ComplementaryDistributionFunction(k: 2); // 0.78976201297690252
    /// 
    /// // Probability mass functions
    /// double pmf1 = dist.ProbabilityMassFunction(k: 4); // 0.19442365170822165
    /// double pmf2 = dist.ProbabilityMassFunction(k: 5); // 0.1633158674349062
    /// double pmf3 = dist.ProbabilityMassFunction(k: 6); // 0.11432110720443435
    /// double lpmf = dist.LogProbabilityMassFunction(k: 2); // -2.0229781299813
    /// 
    /// // Quantile function
    /// int icdf1 = dist.InverseDistributionFunction(p: cdf1); // 2
    /// int icdf2 = dist.InverseDistributionFunction(p: cdf2); // 4
    /// int icdf3 = dist.InverseDistributionFunction(p: cdf3); // 7
    /// 
    /// // Hazard (failure rate) functions
    /// double hf = dist.HazardFunction(x: 4); // 0.47400404660843515
    /// double chf = dist.CumulativeHazardFunction(x: 4); // 0.89117630901575073
    /// 
    /// // String representation
    /// string str = dist.ToString(CultureInfo.InvariantCulture); // "Poisson(x; λ = 4.2)"
    /// </code>
    /// 
    /// <para>
    ///   This example shows hows to call the distribution function 
    ///   to compute different types of probabilities. </para>
    ///   
    /// <code>
    /// // Create a new Poisson distribution
    /// var dist = new PoissonDistribution(lambda: 4.2);
    /// 
    /// // P(X = 1) = 0.0629814226460064
    /// double equal = dist.ProbabilityMassFunction(k: 1);
    /// 
    /// // P(X &lt; 1) = 0.0149955768204777
    /// double less = dist.DistributionFunction(k: 1, inclusive: false);
    /// 
    /// // P(X ≤ 1) = 0.0779769994664841
    /// double lessThanOrEqual = dist.DistributionFunction(k: 1, inclusive: true);
    /// 
    /// // P(X > 1) = 0.922023000533516
    /// double greater = dist.ComplementaryDistributionFunction(k: 1);
    /// 
    /// // P(X ≥ 1) = 0.985004423179522
    /// double greaterThanOrEqual = dist.ComplementaryDistributionFunction(k: 1, inclusive: true);
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class PoissonDistribution : UnivariateDiscreteDistribution,
        IFittableDistribution<double, IFittingOptions>,
        ISampleableDistribution<int>
    {

        // Distribution parameters
        private double lambda; // λ

        // Derived values
        private double epml;

        // Distribution measures
        private double? entropy;

        bool immutable;


        /// <summary>
        ///   Creates a new Poisson distribution with λ = 1.
        /// </summary>
        /// 
        public PoissonDistribution()
            : this(1)
        {
        }

        /// <summary>
        ///   Creates a new Poisson distribution with the given λ (lambda).
        /// </summary>
        /// 
        /// <param name="lambda">The Poisson's λ (lambda) parameter. Default is 1.</param>
        /// 
        public PoissonDistribution([Positive] double lambda)
        {
            if (lambda <= 0)
            {
                throw new ArgumentOutOfRangeException("lambda",
                    "Poisson's λ must be greater than zero.");
            }

            initialize(lambda);
        }

        private void initialize(double lm)
        {
            if (lm <= 0)
                lm = Constants.DoubleEpsilon;

            this.lambda = lm;
            this.epml = Math.Exp(-lm);

            this.entropy = null;
        }

        /// <summary>
        ///   Gets the Poisson's parameter λ (lambda).
        /// </summary>
        /// 
        public double Lambda
        {
            get { return lambda; }
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        public override double Mean
        {
            get { return lambda; }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        public override double Variance
        {
            get { return lambda; }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <remarks>
        ///   A closed form expression for the entropy of a Poisson
        ///   distribution is unknown. This property returns an approximation
        ///   for large lambda.
        /// </remarks>
        /// 
        public override double Entropy
        {
            get
            {
                if (!entropy.HasValue)
                {
                    entropy = 0.5 * System.Math.Log(2.0 * System.Math.PI * lambda)
                        - 1 / (12 * lambda)
                        - 1 / (24 * lambda * lambda)
                        - 19 / (360 * lambda * lambda * lambda);
                }

                return entropy.Value;
            }
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
        /// <param name="k">
        ///   A single point in the distribution range.</param>
        ///   
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// 
        protected internal override double InnerDistributionFunction(int k)
        {
            return Gamma.UpperIncomplete(k + 1, lambda);
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
        ///   probability that a given value <c>k</c> will occur.
        /// </remarks>
        /// 
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.</returns>
        ///   
        protected internal override double InnerProbabilityMassFunction(int k)
        {
            return (Math.Pow(lambda, k) / Special.Factorial(k)) * epml;
        }

        /// <summary>
        ///   Gets the log-probability mass function (pmf) for
        ///   this distribution evaluated at point <c>k</c>.
        /// </summary>
        /// 
        /// <param name="k">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The logarithm of the probability of <c>k</c>
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
            return (k * Math.Log(lambda) - Special.LogFactorial(k)) - lambda;
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
            if (immutable)
                throw new InvalidOperationException("This object can not be modified.");

            if (options != null)
                throw new ArgumentException("No options may be specified.");

            double mean;

            if (weights == null)
                mean = observations.Mean();
            else
                mean = observations.WeightedMean(weights);

            initialize(mean);
        }


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
        public override int[] Generate(int samples, int[] result, Random source)
        {
            if (lambda > 30)
            {
                for (int i = 0; i < samples; i++)
                    result[i] = InverseDistributionFunction(source.NextDouble());
            }
            else
            {
                for (int i = 0; i < samples; i++)
                    result[i] = knuth(source, lambda);
            }

            return result;
        }

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
            if (lambda > 30)
            {
                for (int i = 0; i < samples; i++)
                    result[i] = InverseDistributionFunction(source.NextDouble());
            }
            else
            {
                for (int i = 0; i < samples; i++)
                    result[i] = knuth(source, lambda);
            }

            return result;
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public override int Generate(Random source)
        {
            if (lambda > 30)
                return InverseDistributionFunction(source.NextDouble());

            return knuth(source, lambda);
        }

        private static int knuth(Random random, double lambda)
        {
            // Knuth, 1969.
            double p = 1.0;
            double L = Math.Exp(-lambda);

            int k;

            for (k = 0; p > L; k++)
                p *= random.NextDouble();

            return k - 1;
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
            return new PoissonDistribution(lambda);
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
            return String.Format("Poisson(x; λ = {0})",
                lambda.ToString(format, formatProvider));
        }


        /// <summary>
        ///   Gets the standard Poisson distribution,
        ///   with lambda (rate) equal to 1.
        /// </summary>
        /// 
        public static PoissonDistribution Standard { get { return standard; } }

        private static readonly PoissonDistribution standard = new PoissonDistribution() { immutable = true };

    }
}
