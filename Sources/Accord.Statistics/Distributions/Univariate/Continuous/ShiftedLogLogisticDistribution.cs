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
    ///   Shift Log-Logistic distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The shifted log-logistic distribution is a probability distribution also known as
    ///   the generalized log-logistic or the three-parameter log-logistic distribution. It
    ///   has also been called the generalized logistic distribution, but this conflicts with
    ///   other uses of the term: see generalized logistic distribution.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Shifted_log-logistic_distribution">
    ///       Wikipedia, The Free Encyclopedia. Shifted log-logistic distribution. Available on: 
    ///       http://en.wikipedia.org/wiki/Shifted_log-logistic_distribution </a></description></item>
    ///   </list></para> 
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   This examples shows how to create a Shifted Log-Logistic distribution,
    ///   compute some of its properties and generate a number of random samples
    ///   from it.</para>
    ///   
    /// <code>
    /// // Create a LLD3 distribution with μ = 0.0, scale = 0.42, and shape = 0.1
    /// var log = new ShiftedLogLogisticDistribution(location: 0, scale: 0.42, shape: 0.1);
    /// 
    /// double mean = log.Mean;     // 0.069891101544818923
    /// double median = log.Median; // 0.0
    /// double mode = log.Mode;     // -0.083441677069328604
    /// double var = log.Variance;  // 0.62447259946747213
    /// 
    /// double cdf = log.DistributionFunction(x: 1.4); // 0.94668863559417671
    /// double pdf = log.ProbabilityDensityFunction(x: 1.4); // 0.090123683626808615
    /// double lpdf = log.LogProbabilityDensityFunction(x: 1.4); // -2.4065722895662613
    /// 
    /// double ccdf = log.ComplementaryDistributionFunction(x: 1.4); // 0.053311364405823292
    /// double icdf = log.InverseDistributionFunction(p: cdf); // 1.4000000037735139
    /// 
    /// double hf = log.HazardFunction(x: 1.4); // 1.6905154207038875
    /// double chf = log.CumulativeHazardFunction(x: 1.4); // 2.9316057546685061
    /// 
    /// string str = log.ToString(CultureInfo.InvariantCulture); // LLD3(x; μ = 0, σ = 0.42, ξ = 0.1)
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="LogisticDistribution"/>
    /// <seealso cref="ParetoDistribution"/>
    /// 
    [Serializable]
    public class ShiftedLogLogisticDistribution : UnivariateContinuousDistribution
    {

        // Distribution parameters
        private double mu;    // location μ (real)
        private double sigma; // scale σ    (positive)
        private double ksi;   // shape ξ    (real)

        /// <summary>
        ///   Constructs a Shifted Log-Logistic distribution
        ///   with zero location, unit scale, and zero shape.
        /// </summary>
        /// 
        public ShiftedLogLogisticDistribution()
        {
            initialize(0, 1, 0);
        }

        /// <summary>
        ///   Constructs a Shifted Log-Logistic distribution
        ///   with the given location, unit scale and zero shape.
        /// </summary>
        /// 
        /// <param name="location">The distribution's location value μ (mu).</param>
        /// 
        public ShiftedLogLogisticDistribution([Real] double location)
        {
            initialize(location, 1, 0);
        }

        /// <summary>
        ///   Constructs a Shifted Log-Logistic distribution
        ///   with the given location and scale and zero shape.
        /// </summary>
        /// 
        /// <param name="location">The distribution's location value μ (mu).</param>
        /// <param name="scale">The distribution's scale value σ (sigma).</param>
        /// 
        public ShiftedLogLogisticDistribution([Real] double location, [Positive] double scale)
        {
            if (scale <= 0)
                throw new ArgumentOutOfRangeException("scale", "Scale must be positive.");

            initialize(location, scale, 0);
        }

        /// <summary>
        ///   Constructs a Shifted Log-Logistic distribution
        ///   with the given location and scale and zero shape.
        /// </summary>
        /// 
        /// <param name="location">The distribution's location value μ (mu).</param>
        /// <param name="scale">The distribution's scale value s.</param>
        /// <param name="shape">The distribution's shape value ξ (ksi).</param>
        /// 
        public ShiftedLogLogisticDistribution([Real] double location, [Positive] double scale, [Real] double shape)
        {
            if (scale <= 0)
                throw new ArgumentOutOfRangeException("scale", "Scale must be positive.");

            initialize(location, scale, shape);
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
                if (ksi == 0)
                    return mu;

                double b = Math.PI * ksi;
                double a = sigma / ksi;
                return mu + a * b * Special.Cosec(b) - a * 1;
            }
        }

        /// <summary>
        ///   Gets the distribution's location value μ (mu).
        /// </summary>
        /// 
        public double Location
        {
            get { return mu; }
        }

        /// <summary>
        ///   Gets the distribution's scale value (σ).
        /// </summary>
        /// 
        public double Scale
        {
            get { return sigma; }
        }

        /// <summary>
        ///   Gets the distribution's shape value (ξ).
        /// </summary>
        /// 
        public double Shape
        {
            get { return ksi; }
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
                Accord.Diagnostics.Debug.Assert(mu.IsEqual(base.Median, 1e-5));
                return mu;
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
                if (ksi == 0)
                {
                    // lim x->0  ((b^2)/(x^2))*(2*pi*x csc(2*pi*x) - (pi*x*csc(pi*x))^2)
                    // http://www.wolframalpha.com/input/?i=lim+x-%3E0++%28a+%2B+%28%28b%5E2%29%2F%28x%5E2%29%29*%282*pi*x+csc%282*pi*x%29+-+%28pi*x*csc%28pi*x%29%29%5E2%29%29
                    return sigma * sigma * Math.PI * Math.PI / 3.0;
                }
                else
                {
                    double pb = Math.PI * ksi;
                    double a = (sigma * sigma) / (ksi * ksi);
                    double b = 2 * pb * Special.Cosec(2 * pb);
                    double c = pb * Special.Cosec(pb);

                    return a * (b - c * c);
                }
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
                if (ksi == 0)
                    return mu;

                return mu + (sigma / ksi) * (Math.Pow((1 - ksi) / (1 + ksi), ksi) - 1);
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
            get
            {
                if (ksi > 0)
                    return new DoubleRange(mu - sigma / ksi, Double.PositiveInfinity);

                if (ksi < 0)
                    return new DoubleRange(Double.NegativeInfinity, mu - sigma / ksi);

                // if (ksi == 0)
                return new DoubleRange(Double.NegativeInfinity, Double.PositiveInfinity);
            }
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
            if (ksi > 0 && x < mu - sigma / ksi)
                return 0;

            double z = (x - mu) / sigma;

            if (ksi == 0)
                return 1 / (1 + Math.Exp(-z));

            return 1 / (1 + Math.Pow(1 + ksi * z, -1 / ksi));
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
            double z = (x - mu) / sigma;

            double a, b;

            if (ksi == 0)
            {
                a = Math.Exp(-z);
                b = (1 + a);
            }
            else
            {
                a = Math.Pow(1 + ksi * z, -1 / ksi - 1);
                b = 1 + Math.Pow(1 + ksi * z, -1 / ksi);
            }

            return a / (sigma * b * b);
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
            return new ShiftedLogLogisticDistribution(mu, sigma, ksi);
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
            return String.Format(formatProvider, "LLD3(x; μ = {0}, σ = {1}, ξ = {2})",
                mu.ToString(format, formatProvider),
                sigma.ToString(format, formatProvider),
                ksi.ToString(format, formatProvider));
        }


        private void initialize(double mean, double scale, double shape)
        {
            this.mu = mean;
            this.sigma = scale;
            this.ksi = shape;
        }
    }
}
