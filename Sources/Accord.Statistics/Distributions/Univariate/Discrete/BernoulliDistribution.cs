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
    using Accord.Statistics.Distributions.Fitting;
    using AForge;

    /// <summary>
    ///   Bernoulli probability distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Bernoulli distribution is a distribution for a single
    ///   binary variable x E {0,1}, representing, for example, the
    ///   flipping of a coin. It is governed by a single continuous
    ///   parameter representing the probability of an observation
    ///   to be equal to 1.</para>
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Bernoulli_distribution">
    ///       Wikipedia, The Free Encyclopedia. Bernoulli distribution. Available on:
    ///       http://en.wikipedia.org/wiki/Bernoulli_distribution </a></description></item>
    ///     <item><description>
    ///       C. Bishop. “Pattern Recognition and Machine Learning”. Springer. 2006.</description></item>
    ///   </list></para>
    /// </remarks>
    ///
    /// <example>
    /// <code>
    ///    // Create a distribution with probability 0.42
    ///    var bern = new BernoulliDistribution(mean: 0.42);
    ///    
    ///    // Common measures
    ///    double mean   = bern.Mean;     // 0.42
    ///    double median = bern.Median;   // 0.0
    ///    double var    = bern.Variance; // 0.2436
    ///    double mode   = bern.Mode;     // 0.0
    ///    
    ///    // Probability mass functions
    ///    double pdf = bern.ProbabilityMassFunction(k: 1); // 0.42
    ///    double lpdf = bern.LogProbabilityMassFunction(k: 0); // -0.54472717544167193
    ///    
    ///    // Cumulative distribution functions
    ///    double cdf = bern.DistributionFunction(k: 0);    // 0.58
    ///    double ccdf = bern.ComplementaryDistributionFunction(k: 0); // 0.42
    ///    
    ///    // Quantile functions
    ///    int icdf0 = bern.InverseDistributionFunction(p: 0.57); // 0
    ///    int icdf1 = bern.InverseDistributionFunction(p: 0.59); // 1
    ///    
    ///    // Hazard / failure rate functions
    ///    double hf = bern.HazardFunction(x: 0); // 1.3809523809523814
    ///    double chf = bern.CumulativeHazardFunction(x: 0); // 0.86750056770472328
    ///    
    ///    // String representation
    ///    string str = bern.ToString(CultureInfo.InvariantCulture); // "Bernoulli(x; p = 0.42, q = 0.58)"
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="BinomialDistribution"/>
    ///
    [Serializable]
    public class BernoulliDistribution : UnivariateDiscreteDistribution,
        IFittableDistribution<double, IFittingOptions>,
        ISampleableDistribution<int>
    {

        // Distribution parameters
        private double probability;

        // Derived parameter values
        private double complement;

        // Distribution measures
        private double? entropy;


        /// <summary>
        ///   Creates a new <see cref="BernoulliDistribution">Bernoulli</see> distribution.
        /// </summary>
        /// 
        /// <param name="mean">The probability of an observation being equal to 1.</param>
        /// 
        public BernoulliDistribution(double mean)
        {
            if (mean < 0 || mean > 1)
                throw new ArgumentOutOfRangeException("mean",
                    "Mean should be between zero and one.");

            this.initialize(mean);
        }

        private void initialize(double mean)
        {
            this.probability = mean;
            this.complement = 1.0 - mean;

            this.entropy = null;
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        public override double Mean
        {
            get { return probability; }
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
                double median;

                if (complement > probability)
                    median = 0;

                else if (complement < probability) 
                    median = 1;

                else median = 0.5;

                System.Diagnostics.Debug.Assert(median == base.Median);

                return median;
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
                if (complement > probability)
                    return 0;

                if (complement < probability)
                    return 1;

                return 0; // TODO: should return both 0 and 1
            }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        public override double Variance
        {
            get { return probability * complement; }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        public override double Entropy
        {
            get
            {
                if (!entropy.HasValue)
                {
                    entropy = -probability * System.Math.Log(probability) -
                                complement * System.Math.Log(complement);
                }

                return entropy.Value;
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
            get { return new DoubleRange(0, 1); }
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
            if (k < 0) return 0;
            if (k >= 1) return 1;
            return complement;
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
        ///   value when applied in the <see cref="DistributionFunction"/>.
        /// </returns>
        /// 
        public override int InverseDistributionFunction(double p)
        {
            return (p > this.complement) ? 1 : 0;
        }

        /// <summary>
        ///   Gets P(X &gt; k) the complementary cumulative distribution function
        ///   (ccdf) for this distribution evaluated at point <c>k</c>.
        ///   This function is also known as the Survival function.
        /// </summary>
        /// 
        /// <param name="k">A single point in the distribution range.</param>
        /// 
        /// <remarks>
        ///   The Complementary Cumulative Distribution Function (CCDF) is
        ///   the complement of the Cumulative Distribution Function, or 1
        ///   minus the CDF.
        /// </remarks>
        /// 
        public override double ComplementaryDistributionFunction(int k)
        {
            if (k < 0) return 1;
            if (k >= 1) return 0;
            return probability;
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
        /// <remarks>
        ///   The Probability Mass Function (PMF) describes the
        ///   probability that a given value <c>k</c> will occur.
        /// </remarks>
        /// 
        public override double ProbabilityMassFunction(int k)
        {
            if (k == 1) return probability;
            if (k == 0) return complement;
            return 0;
        }

        /// <summary>
        ///   Gets the log-probability mass function (pmf) for
        ///   this distribution evaluated at point <c>x</c>.
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
        public override double LogProbabilityMassFunction(int k)
        {
            if (k == 1) return Math.Log(probability);
            if (k == 0) return Math.Log(complement);
            return double.NegativeInfinity;
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
                mean = observations.Mean();
            else
                mean = observations.WeightedMean(weights);

            initialize(mean);
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
            return new BernoulliDistribution(probability);
        }


        #region ISampleableDistribution<int> Members

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public int[] Generate(int samples)
        {
            int[] r = new int[samples];
            for (int i = 0; i < r.Length; i++)
            {
                double u = Accord.Math.Tools.Random.Next();
                r[i] = u > this.probability ? 1 : 0;
            }

            return r;
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public int Generate()
        {
            double u = Accord.Math.Tools.Random.Next();
            return u > this.probability ? 1 : 0;
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
            return String.Format("Bernoulli(x; p = {0}, q = {1})", probability, complement);
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
            return String.Format(formatProvider, "Bernoulli(x; p = {0}, q = {1})", probability, complement);
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
            return String.Format("Bernoulli(x; p = {0}, q = {1})",
                probability.ToString(format, formatProvider),
                complement.ToString(format, formatProvider));
        }
    }
}
