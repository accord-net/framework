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
    using Accord.Compat;

    /// <summary>
    ///   Log-Logistic distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability and statistics, the log-logistic distribution (known as the Fisk 
    ///   distribution in economics) is a continuous probability distribution for a non-negative
    ///   random variable. It is used in survival analysis as a parametric model for events 
    ///   whose rate increases initially and decreases later, for example mortality rate from
    ///   cancer following diagnosis or treatment. It has also been used in hydrology to model 
    ///   stream flow and precipitation, and in economics as a simple model of the distribution
    ///   of wealth or income.</para>
    ///   
    /// <para>
    ///   The log-logistic distribution is the probability distribution of a random variable
    ///   whose logarithm has a logistic distribution. It is similar in shape to the log-normal
    ///   distribution but has heavier tails. Its cumulative distribution function can be written
    ///   in closed form, unlike that of the log-normal.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Log-logistic_distribution">
    ///       Wikipedia, The Free Encyclopedia. Log-logistic distribution. Available on: 
    ///       http://en.wikipedia.org/wiki/Log-logistic_distribution </a></description></item>
    ///   </list></para> 
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   This examples shows how to create a Log-Logistic distribution
    ///   and compute some of its properties and characteristics.</para>
    ///   
    /// <code>
    /// // Create a LLD2 distribution with scale = 0.42, shape = 2.2
    /// var log = new LogLogisticDistribution(scale: 0.42, shape: 2.2);
    /// 
    /// double mean = log.Mean;     // 0.60592605102976937
    /// double median = log.Median; // 0.42
    /// double mode = log.Mode;     // 0.26892249963239817
    /// double var = log.Variance;  // 1.4357858982592435
    /// 
    /// double cdf = log.DistributionFunction(x: 1.4);           // 0.93393329906725353
    /// double pdf = log.ProbabilityDensityFunction(x: 1.4);     // 0.096960115938100763
    /// double lpdf = log.LogProbabilityDensityFunction(x: 1.4); // -2.3334555609306102
    /// 
    /// double ccdf = log.ComplementaryDistributionFunction(x: 1.4); // 0.066066700932746525
    /// double icdf = log.InverseDistributionFunction(p: cdf);       // 1.4000000000000006
    /// 
    /// double hf = log.HazardFunction(x: 1.4);            // 1.4676094699628273
    /// double chf = log.CumulativeHazardFunction(x: 1.4); // 2.7170904270953637
    /// 
    /// string str = log.ToString(CultureInfo.InvariantCulture); // LogLogistic(x; α = 0.42, β = 2.2)
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="LogisticDistribution"/>
    /// <seealso cref="ShiftedLogLogisticDistribution"/>
    /// 
    [Serializable]
    public class LogLogisticDistribution : UnivariateContinuousDistribution
    {

        // Distribution parameters
        private double alpha;   // scale α
        private double beta;    // shape β

        /// <summary>
        ///   Constructs a Log-Logistic distribution with unit scale and unit shape.
        /// </summary>
        /// 
        public LogLogisticDistribution()
        {
            initialize(1, 1);
        }

        /// <summary>
        ///   Constructs a Log-Logistic distribution
        ///   with the given scale and unit shape.
        /// </summary>
        /// 
        /// <param name="alpha">The distribution's scale value α (alpha). Default is 1.</param>
        /// 
        public LogLogisticDistribution([Positive] double alpha)
        {
            if (alpha <= 0)
                throw new ArgumentOutOfRangeException("alpha", "Alpha must be positive.");

            initialize(alpha, 1);
        }

        /// <summary>
        ///   Constructs a Log-Logistic distribution
        ///   with the given scale and shape parameters.
        /// </summary>
        /// 
        /// <param name="alpha">The distribution's scale value α (alpha). Default is 1.</param>
        /// <param name="beta">The distribution's shape value β (beta). Default is 1.</param>
        /// 
        public LogLogisticDistribution([Positive] double alpha, [Positive] double beta)
        {
            if (alpha <= 0)
                throw new ArgumentOutOfRangeException("alpha", "Alpha must be positive.");

            if (beta <= 0)
                throw new ArgumentOutOfRangeException("beta", "Beta must be positive.");

            initialize(alpha, beta);
        }


        /// <summary>
        ///   Gets the distribution's scale value (α).
        /// </summary>
        /// 
        public double Scale
        {
            get { return alpha; }
        }

        /// <summary>
        ///   Gets the distribution's shape value (β).
        /// </summary>
        /// 
        public double Shape
        {
            get { return beta; }
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
                if (beta <= 1)
                    return Double.NaN;

                double b = Math.PI / beta;
                return (alpha * b) / Math.Sin(b);
            }
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
                Accord.Diagnostics.Debug.Assert(alpha == base.Median);
                return alpha;
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
                if (beta <= 2)
                    return Double.NaN;

                double pb = Math.PI / beta;
                double a = alpha * alpha;
                double b = 2 * pb / Math.Sin(2 * pb);
                double c = pb / Math.Sin(pb);

                return a * (b - c * c);
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
                if (beta <= 1)
                    return 0;
                return alpha * Math.Pow((beta - 1) / (beta + 1), 1 / beta);
            }
        }

        /// <summary>
        ///   Not supported.
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
            get { return new DoubleRange(0, Double.PositiveInfinity); }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        protected internal override double InnerDistributionFunction(double x)
        {
            double xb = Math.Pow(x, beta);
            double ab = Math.Pow(alpha, beta);

            return xb / (ab + xb);
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
        protected internal override double InnerProbabilityDensityFunction(double x)
        {
            double ba = beta / alpha;
            double xa = x / alpha;

            double num = ba * Math.Pow(xa, beta - 1);
            double den = 1 + Math.Pow(xa, beta);

            return num / (den * den);
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
            return alpha * Math.Pow(p / (1 - p), 1 / beta);
        }

        /// <summary>
        ///   Gets the first derivative of the <see cref="UnivariateContinuousDistribution.InverseDistributionFunction(double)">
        ///   inverse distribution function</see> (icdf) for this distribution evaluated
        ///   at probability <c>p</c>. 
        /// </summary>
        /// 
        /// <param name="p">A probability value between 0 and 1.</param>
        /// 
        public override double QuantileDensityFunction(double p)
        {
            double qdf = (alpha / beta) * Math.Pow(p / (1 - p), 1 / beta - 1);

            Accord.Diagnostics.Debug.Assert(qdf == base.QuantileDensityFunction(p));

            return qdf;
        }

        /// <summary>
        ///   Gets the complementary cumulative distribution function
        ///   (ccdf) for this distribution evaluated at point <c>x</c>.
        ///   This function is also known as the Survival function.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <remarks>
        ///   The Complementary Cumulative Distribution Function (CCDF) is
        ///   the complement of the Cumulative Distribution Function, or 1
        ///   minus the CDF.
        /// </remarks>
        /// 
        protected internal override double InnerComplementaryDistributionFunction(double x)
        {
            double xa = x / alpha;
            double den = 1 + Math.Pow(xa, beta);

            double icdf = 1 / den;

            Accord.Diagnostics.Debug.Assert(icdf.IsEqual(base.InnerComplementaryDistributionFunction(x), 1e-10));

            return icdf;
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
        /// <remarks>
        ///   The hazard function is the ratio of the probability
        ///   density function f(x) to the survival function, S(x).
        /// </remarks>
        /// 
        public override double HazardFunction(double x)
        {
            if (x < 0)
                return 0;

            double ba = beta / alpha;
            double xa = x / alpha;

            double num = ba * Math.Pow(xa, beta - 1);
            double den = 1 + Math.Pow(xa, beta);

            double h = num / den;

            Accord.Diagnostics.Debug.Assert(h.IsEqual(base.HazardFunction(x), 1e-10));

            return h;
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
            return new LogLogisticDistribution(alpha, beta);
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
            return String.Format(formatProvider, "LogLogistic(x; α = {0}, β = {1})",
                alpha.ToString(format, formatProvider),
                beta.ToString(format, formatProvider));
        }



        private void initialize(double scale, double shape)
        {
            this.alpha = scale;
            this.beta = shape;
        }

        /// <summary>
        ///   Creates a new <see cref="LogLogisticDistribution"/> using 
        ///   the location-shape parametrization. In this parametrization,
        ///   <see cref="Beta"/> is taken as 1 / <paramref name="shape"/>.
        /// </summary>
        /// 
        /// <param name="location">The location parameter μ (mu) [taken as μ = α].</param>
        /// <param name="shape">The distribution's shape value σ (sigma) [taken as σ = β].</param>
        /// 
        /// <returns>
        ///   A <see cref="LogLogisticDistribution"/> with α = μ  and β = 1/σ.
        /// </returns>
        /// 
        public static LogLogisticDistribution FromLocationShape(double location, double shape)
        {
            return new LogLogisticDistribution(alpha: location, beta: 1 / shape);
        }
    }
}
