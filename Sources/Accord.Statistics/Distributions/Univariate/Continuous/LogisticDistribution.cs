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
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Multivariate;
    using AForge;

    /// <summary>
    ///   Logistic distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability theory and statistics, the logistic distribution is a continuous
    ///   probability distribution. Its cumulative distribution function is the logistic 
    ///   function, which appears in logistic regression and feedforward neural networks.
    ///   It resembles the normal distribution in shape but has heavier tails (higher 
    ///   kurtosis). The <see cref="TukeyLambdaDistribution">Tukey lambda distribution</see>
    ///   can be considered a generalization of the logistic distribution since it adds a
    ///   shape parameter, λ (the Tukey distribution becomes logistic when λ is zero).</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Logistic_distribution">
    ///       Wikipedia, The Free Encyclopedia. Logistic distribution. Available on: 
    ///       http://en.wikipedia.org/wiki/Logistic_distribution </a></description></item>
    ///   </list></para> 
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   This examples shows how to create a Logistic distribution,
    ///   compute some of its properties and generate a number of
    ///   random samples from it.</para>
    ///   
    /// <code>
    ///   // Create a normal distribution with mean 2 and sigma 3
    ///   var normal = new NormalDistribution(mean: 2, stdDev: 3);
    /// 

    /// </code>
    /// 
    /// <seealso cref="TukeyLambdaDistribution"/>
    /// 
    [Serializable]
    public class LogisticDistribution : UnivariateContinuousDistribution
    {

        // Distribution parameters
        private double mu;   // location μ
        private double s;        // scale s

        /// <summary>
        ///   Constructs a Logistic distribution
        ///   with given location and scale parameters.
        /// </summary>
        /// 
        public LogisticDistribution()
        {
            initialize(0, 1);
        }

        /// <summary>
        ///   Constructs a Logistic distribution
        ///   with given location and scale parameters.
        /// </summary>
        /// 
        /// <param name="mean">The distribution's location value μ (mu).</param>
        /// 
        public LogisticDistribution(double location)
        {
            initialize(location, 1);
        }

        /// <summary>
        ///   Constructs a Logistic distribution
        ///   with given location and scale parameters.
        /// </summary>
        /// 
        /// <param name="mean">The distribution's location value μ (mu).</param>
        /// <param name="stdDev">The distribution's scale value s.</param>
        /// 
        public LogisticDistribution(double location, double scale)
        {
            if (scale <= 0)
                throw new ArgumentOutOfRangeException("scale", "Scale must be positive.");

            initialize(location, scale);
        }



        /// <summary>
        ///   Gets the location value μ (mu).
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's mean value.
        /// </value>
        /// 
        public override double Mean
        {
            get { return mu; }
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
                System.Diagnostics.Debug.Assert(mu == base.Median);
                return mu;
            }
        }

        public override double Variance
        {
            get { return (s * s * Math.PI * Math.PI) / 3.0; }
        }

        public override double Mode
        {
            get { return mu; }
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

        public override double Entropy
        {
            get { return Math.Log(s) + 2; }
        }


        public override double DistributionFunction(double x)
        {
            double z = (x - mu) / s;

            return 1.0 / (1 + Math.Exp(-z));
        }


        public override double ProbabilityDensityFunction(double x)
        {
            double z = (x - mu) / s;

            double num = Math.Exp(-z);
            double a = (1 + num);
            double den = s * a * a;

            return num / den;
        }

        public override double LogProbabilityDensityFunction(double x)
        {
            double z = (x - mu) / s;

            double result = -z - (Math.Log(s) + 2 * Special.Log1p(Math.Exp(-z)));

            return result;
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
            return new LogisticDistribution(mu, s);
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
            return String.Format("Logistic(x; μ = {0}, s = {1})", mu, s);
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
            return String.Format(formatProvider, "Logistic(x; μ = {0}, scale = {1})", mu, s);
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
            return String.Format(formatProvider, "Logistic(x; μ = {0}, s = {1})",
                mu.ToString(format, formatProvider),
                s.ToString(format, formatProvider));
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
            return String.Format("Logistic(x; μ = {0}, s = {1})",
                mu.ToString(format), s.ToString(format));
        }


        private void initialize(double mean, double scale)
        {
            this.mu = mean;
            this.s = scale;
        }
    }
}
