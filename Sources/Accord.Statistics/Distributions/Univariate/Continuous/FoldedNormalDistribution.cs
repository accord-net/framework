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
    using System.ComponentModel;
    using Accord.Math;
    using Accord.Compat;

    /// <summary>
    ///   Folded Normal (Gaussian) distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The folded normal distribution is a probability distribution related to the normal
    ///   distribution. Given a normally distributed random variable X with mean μ and variance
    ///   σ², the random variable Y = |X| has a folded normal distribution. Such a case may be
    ///   encountered if only the magnitude of some variable is recorded, but not its sign. The
    ///   distribution is called Folded because probability mass to the left of the x = 0 is 
    ///  "folded" over by taking the absolute value.</para>
    ///  
    /// <para>
    ///   The Half-Normal (Gaussian) distribution <see cref="HalfNormal">is a special 
    ///   case of this distribution and can be created using a named constructor</see>.
    /// </para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://en.wikipedia.org/wiki/Folded_normal_distribution">
    ///       Wikipedia, The Free Encyclopedia. Folded Normal distribution. Available on: 
    ///       https://en.wikipedia.org/wiki/Folded_normal_distribution </a></description></item>
    ///   </list></para> 
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   This examples shows how to create a Folded Normal distribution
    ///   and how to compute some of its properties and measures.</para>
    ///   
    /// <code>
    /// // Creates a new Folded Normal distribution based on a Normal
    /// // distribution with mean value 4 and standard deviation 4.2:
    /// //
    /// var fn = new FoldedNormalDistribution(mean: 4, stdDev: 4.2);
    /// 
    /// double mean = fn.Mean;     // 4.765653108337438
    /// double median = fn.Median; // 4.2593565881862734
    /// double mode = fn.Mode;     // 2.0806531871308014
    /// double var = fn.Variance;  // 10.928550450993715
    /// 
    /// double cdf = fn.DistributionFunction(x: 1.4);           // 0.16867109769018807
    /// double pdf = fn.ProbabilityDensityFunction(x: 1.4);     // 0.11998602818182187
    /// double lpdf = fn.LogProbabilityDensityFunction(x: 1.4); // -2.1203799747969523
    /// 
    /// double ccdf = fn.ComplementaryDistributionFunction(x: 1.4); // 0.83132890230981193
    /// double icdf = fn.InverseDistributionFunction(p: cdf);       // 1.4
    /// 
    /// double hf = fn.HazardFunction(x: 1.4);            // 0.14433039420191671
    /// double chf = fn.CumulativeHazardFunction(x: 1.4); // 0.18472977144474392
    /// 
    /// string str = fn.ToString(CultureInfo.InvariantCulture); // FN(x; μ = 4, σ² = 17.64)
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="Accord.Statistics.Distributions.Univariate.NormalDistribution"/>
    /// 
    [Serializable]
    public class FoldedNormalDistribution : UnivariateContinuousDistribution
    {

        // Distribution parameters
        private double mu = 0;     // location μ
        private double sigma2 = 1; // scale σ²

        // Derived parameters
        private double sigma = 1;


        /// <summary>
        ///   Creates a new <see cref="FoldedNormalDistribution"/> 
        ///   with zero mean and unit standard deviation.
        /// </summary>
        /// 
        public FoldedNormalDistribution()
        {
            initialize(0, 1, 1);
        }

        /// <summary>
        ///   Creates a new <see cref="FoldedNormalDistribution"/> with
        ///   the given <paramref name="mean"/> and unit standard deviation.
        /// </summary>
        /// 
        /// <param name="mean">
        ///   The mean of the original normal distribution that should be folded.</param>
        /// 
        public FoldedNormalDistribution([Real] double mean)
        {
            initialize(mean, 1, 1);
        }

        /// <summary>
        ///   Creates a new <see cref="FoldedNormalDistribution"/> with
        ///   the given <paramref name="mean"/> and <paramref name="stdDev">
        ///   standard deviation</paramref>
        /// </summary>
        /// 
        /// <param name="mean">
        ///   The mean of the original normal distribution that should be folded.</param>
        /// <param name="stdDev">
        ///   The standard deviation of the original normal distribution that should be folded.</param>
        /// 
        public FoldedNormalDistribution([Real] double mean, [Positive] double stdDev)
        {
            if (stdDev <= 0)
                throw new ArgumentOutOfRangeException("stdDev", "Standard deviation must be positive.");

            initialize(mean, stdDev, stdDev * stdDev);
        }

        /// <summary>
        ///   Creates a new Half-normal distribution with the given<paramref name="stdDev">
        ///   standard deviation</paramref>. The half-normal distribution is a special case 
        ///   of the <see cref="FoldedNormalDistribution"/> when location is zero.
        /// </summary>
        /// 
        /// <param name="stdDev">
        ///   The standard deviation of the original normal distribution that should be folded.</param>
        /// 
        public static FoldedNormalDistribution HalfNormal(double stdDev)
        {
            return new FoldedNormalDistribution(0, stdDev);
        }


        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's mean value.
        /// </value>
        /// 
        public override double Mean
        {
            get
            {
                double a = sigma * Math.Sqrt(2 / Math.PI) * Math.Exp(-(mu * mu) / (2 * sigma2));
                double b = mu * (1 - 2 * Normal.Function(-mu / sigma));
                return a + b;
            }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's variance.
        /// </value>
        /// 
        public override double Variance
        {
            get
            {
                double mean = Mean;
                return mu * mu + sigma2 - mean * mean;
            }
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
        ///   This method is not supported.
        /// </summary>
        /// 
        /// <exception cref="System.NotSupportedException"/>
        /// 
        public override double Entropy
        {
            get { throw new NotSupportedException(); }
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
            double y = Math.Abs(x);

            double den = Math.Sqrt(2) * sigma;
            double a = (y + mu) / den;
            double b = (y - mu) / den;

            double cdf = 0.5 * (Special.Erf(a) + Special.Erf(b));

            return cdf;
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The probability of <c>x</c> occurring in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        protected internal override double InnerProbabilityDensityFunction(double x)
        {
            double a = (+x - mu) / sigma;
            double b = (-x - mu) / sigma;

            return (Normal.Derivative(a) + Normal.Derivative(b)) / sigma;
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
            return new FoldedNormalDistribution(mu, sigma2);
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
            return String.Format(formatProvider, "FN(x; μ = {0}, σ² = {1})",
                mu.ToString(format, formatProvider),
                sigma2.ToString(format, formatProvider));
        }


        private void initialize(double mu, double dev, double var)
        {
            this.mu = mu;
            this.sigma = dev;
            this.sigma2 = var;
        }
    }
}
