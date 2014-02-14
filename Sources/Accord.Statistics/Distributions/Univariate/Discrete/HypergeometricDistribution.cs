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
    ///   Hypergeometric probability distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The hypergeometric distribution is a discrete probability distribution that 
    ///   describes the probability of <c>k</c> successes in <c>n</c> draws from a finite population
    ///   without replacement. This is in contrast to the <see cref="BinomialDistribution">
    ///   binomial distribution</see>, which describes the probability of <c>k</c> successes 
    ///   in <c>n</c> draws with replacement.</para>
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Hypergeometric_distribution">
    ///       Wikipedia, The Free Encyclopedia. Hypergeometric distribution. Available on:
    ///       http://en.wikipedia.org/wiki/Hypergeometric_distribution </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///    // Distribution parameters
    ///    int populationSize = 15; // population size N
    ///    int success = 7;         // number of successes in the sample  
    ///    int samples = 8;         // number of samples drawn from N
    ///    
    ///    // Create a new Hypergeometric distribution with N = 15, n = 8, and s = 7
    ///    var dist = new HypergeometricDistribution(populationSize, success, samples);
    ///    
    ///    // Common measures
    ///    double mean   = dist.Mean;     // 1.3809523809523812
    ///    double median = dist.Median;   // 4.0
    ///    double var    = dist.Variance; // 3.2879818594104315
    ///    double mode   = dist.Mode;     // 4.0
    ///    
    ///    // Cumulative distribution functions
    ///    double cdf = dist.DistributionFunction(k: 2);               // 0.80488799999999994
    ///    double ccdf = dist.ComplementaryDistributionFunction(k: 2); // 0.19511200000000006
    ///    
    ///    // Probability mass functions
    ///    double pdf1 = dist.ProbabilityMassFunction(k: 4); // 0.38073038073038074
    ///    double pdf2 = dist.ProbabilityMassFunction(k: 5); // 0.18275058275058276
    ///    double pdf3 = dist.ProbabilityMassFunction(k: 6); // 0.030458430458430458
    ///    double lpdf = dist.LogProbabilityMassFunction(k: 2); // -2.3927801721315989
    ///    
    ///    // Quantile function
    ///    int icdf1 = dist.InverseDistributionFunction(p: 0.17); // 3
    ///    int icdf2 = dist.InverseDistributionFunction(p: 0.46); // 4
    ///    int icdf3 = dist.InverseDistributionFunction(p: 0.87); // 5
    ///    
    ///    // Hazard (failure rate) functions
    ///    double hf = dist.HazardFunction(x: 4); // 1.7753623188405792
    ///    double chf = dist.CumulativeHazardFunction(x: 4); // 1.5396683418789763
    ///    
    ///    // String representation
    ///    string str = dist.ToString(CultureInfo.InvariantCulture); // "HyperGeometric(x; N = 15, m = 7, n = 8)"
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="BinomialDistribution"/>
    /// <seealso cref="GeometricDistribution"/>
    /// 
    [Serializable]
    public class HypergeometricDistribution : UnivariateDiscreteDistribution,
        IFittableDistribution<double, HypergeometricOptions>
    {

        // Distribution parameters
        private int N;
        private int n;
        private int m;


        /// <summary>
        ///   Gets the size <c>N</c> of the 
        ///   population for this distribution.
        /// </summary>
        /// 
        public int PopulationSize
        {
            get { return N; }
        }

        /// <summary>
        ///   Gets the size <c>n</c> of the sample drawn
        ///   from <see cref="PopulationSize">N</see>.
        /// </summary>
        /// 
        public int SampleSize
        {
            get { return n; }
        }

        /// <summary>
        ///   Gets the count of success trials in the
        ///   population for this distribution. This
        ///   is often referred as <c>m</c>.
        /// </summary>
        /// 
        public int PopulationSuccess
        {
            get { return m; }
        }

        /// <summary>
        ///   Constructs a new <see cref="HypergeometricDistribution">Hypergeometric distribution</see>.
        /// </summary>
        /// 
        /// <param name="populationSize">Size <c>N</c> of the population.</param>
        /// <param name="successes">The number <c>m</c> of successes in the population.</param>
        /// <param name="samples">The number <c>n</c> of samples drawn from the population.</param>
        /// 
        public HypergeometricDistribution(int populationSize, int successes, int samples)
        {
            if (populationSize <= 0)
                throw new ArgumentOutOfRangeException("populationSize",
                    "Size of the population should be greater than zero.");

            if (samples <= 0)
                throw new ArgumentOutOfRangeException("samples",
                    "The number of samples drawn from the population should be greater than zero.");

            if (samples > populationSize)
                throw new ArgumentOutOfRangeException("samples",
                    "The number of samples can not exceed the total size of the population.");

            if (successes < 0 || successes > populationSize)
                throw new ArgumentOutOfRangeException("successes",
                    "The number of successes in the population should range from zero to the size of the population.");

            this.N = populationSize;
            this.n = samples;
            this.m = successes;
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean value.</value>
        /// 
        public override double Mean
        {
            get { return n * (m / (double)N); }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        public override double Variance
        {
            get { return (double)n * (m / (double)N) * ((N - m) / (double)N) * ((N - n) / (N - 1.0)); }
        }

        /// <summary>
        ///  Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's entropy.</value>
        /// 
        public override double Entropy
        {
            get { throw new NotSupportedException(); }
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
                double num = (n + 1) * (m + 1);
                double den = N + 2;
                return Math.Floor(num / den);
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
            get { return new DoubleRange(Math.Max(0, n + m - N), Math.Min(m, n)); }
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
            // This is a really naive implementation. A better approach
            // is described in (Trong Wu; An accurate computation of the
            // hypergeometric distribution function, 1993)

            double sum = 0;
            for (int i = 0; i <= k; i++)
                sum += ProbabilityMassFunction(i);

            return sum;
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
        public override double ProbabilityMassFunction(int k)
        {
            if (k < Math.Max(0, n + m - N) || k > Math.Min(m, n))
                return 0;

            double a = Special.Binomial(m, k);
            double b = Special.Binomial(N - m, n - k);
            double c = Special.Binomial(N, n);

            return (a * b) / c;
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
        public override double LogProbabilityMassFunction(int k)
        {
            return Special.LogBinomial(m, k) + Special.LogBinomial(N - m, n - k)
                - Special.LogBinomial(N, n);
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
        /// <remarks>
        ///   Although both double[] and double[][] arrays are supported,
        ///   providing a double[] for a multivariate distribution or a
        ///   double[][] for a univariate distribution may have a negative
        ///   impact in performance.
        /// </remarks>
        /// 
        public override void Fit(double[] observations, double[] weights, IFittingOptions options)
        {
            HypergeometricOptions geometricOptions = options as HypergeometricOptions;
            if (options != null && geometricOptions == null)
                throw new ArgumentException("The specified options' type is invalid.", "options");

            Fit(observations, weights, geometricOptions);
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
        public void Fit(double[] observations, double[] weights, HypergeometricOptions options)
        {
            if (weights != null)
                throw new NotSupportedException("Weighted estimation is not supported.");

            if (options == null)
                throw new ArgumentNullException("options",
                    "Please specify which parameter to estimate.");

            int successes = 0;
            for (int i = 0; i < observations.Length; i++)
                if (observations[i] == 1) successes++;

            if (options.Parameter == HypergeometricParameter.PopulationSize)
            {
                // Estimate N (population size)
                int newN = (int)Math.Truncate(n * m / (double)successes);
                this.N = newN;
            }
            else if (options.Parameter == HypergeometricParameter.PopulationSuccesses)
            {
                // Estimate m (number of successes in population)
                int newM = (int)Math.Truncate(successes * (N + 1.0) / (double)n);
                this.m = newM;
            }
            else
            {
                throw new InvalidOperationException("Unexpected parameter.");
            }
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
            return new HypergeometricDistribution(N, m, n);
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
            return String.Format("HyperGeometric(x; N = {0}, m = {1}, n = {2})", N, m, n);
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
            return String.Format(formatProvider, "HyperGeometric(x; N = {0}, m = {1}, n = {2})", N, m, n);
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
            return String.Format("HyperGeometric(x; N = {0}, m = {1}, n = {2})",
                N.ToString(format, formatProvider),
                m.ToString(format, formatProvider),
                n.ToString(format, formatProvider));
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
            return String.Format("HyperGeometric(x; N = {0}, m = {1}, n = {2})",
                N.ToString(format), m.ToString(format), n.ToString(format));
        }
    }
}
