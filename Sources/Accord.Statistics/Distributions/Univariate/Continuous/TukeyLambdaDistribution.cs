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
    using Accord.Math.Integration;
    using Accord.Math.Optimization;
    using AForge;

    /// <summary>
    ///   Tukey-Lambda distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Formalized by John Tukey, the Tukey lambda distribution is a continuous 
    ///   probability distribution defined in terms of its quantile function. It is
    ///   typically used to identify an appropriate distribution and not used in 
    ///   statistical models directly.</para>
    /// <para>
    ///   The Tukey lambda distribution has a single shape parameter λ. As with other
    ///   probability distributions, the Tukey lambda distribution can be transformed 
    ///   with a location parameter, μ, and a scale parameter, σ. Since the general form
    ///   of probability distribution can be expressed in terms of the standard distribution, 
    ///   the subsequent formulas are given for the standard form of the function.</para>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Tukey_lambda_distribution">
    ///       Wikipedia, The Free Encyclopedia. Tukey-Lambda distribution. Available on: 
    ///       http://en.wikipedia.org/wiki/Tukey_lambda_distribution </a></description></item>
    ///   </list></para> 
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   This examples shows how to create a Tukey distribution and
    ///   compute some of its properties .</para>
    ///   
    /// <code>

    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class TukeyLambdaDistribution : UnivariateContinuousDistribution
    {

        // Distribution parameters
        private double lambda = 0;   // shape λ


        /// <summary>
        ///   Constructs a Tukey-Lambda distribution
        ///   with the given lambda (shape) parameter.
        /// </summary>
        /// 
        public TukeyLambdaDistribution(double lambda)
        {
            initialize(lambda);
        }

        /// <summary>
        ///   Gets the distribution shape parameter lambda (λ).
        /// </summary>
        /// 
        public double Lambda
        {
            get { return lambda; }
        }

        public override double Mean
        {
            get { return 0; }
        }


        public override double Median
        {
            get { return 0; }
        }

        public override double Entropy
        {
            get { return Romberg.Integrate(LogQuantileDensityFunction, 0, 1); }
        }

        public override double Variance
        {
            get
            {
                if (lambda == 0)
                    return (Math.PI * Math.PI) / 3.0;

                double a = 2.0 / lambda;
                double b = 1 / (1 + 2 * lambda);
                double c = Gamma.Function(lambda + 1);
                double d = Gamma.Function(2 * lambda + 2);
                return a * (b - c / d);
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
            get
            {
                if (lambda > 0)
                    return new DoubleRange(-1 / lambda, 1 / lambda);
                return new DoubleRange(Double.NegativeInfinity, Double.PositiveInfinity);
            }
        }


        public override double DistributionFunction(double x)
        {
            return BrentSearch.Find(InverseDistributionFunction, x, 0, 1);
        }

        public override double InverseDistributionFunction(double p)
        {
            double num = Math.Pow(p, lambda) - Math.Pow(1 - p, lambda);

            return num / lambda;
        }

        public override double QuantileDensityFunction(double p)
        {
            return Math.Pow(p, lambda - 1) + Math.Pow(1 - p, lambda - 1);
        }

        public double LogQuantileDensityFunction(double p)
        {
            return (lambda - 1) * p + (lambda - 1) * (1 - p);
        }

        public override double ProbabilityDensityFunction(double x)
        {
            // http://www.ism.ac.jp/editsec/aism/pdf/044_4_0721.pdf
            return 1.0 / QuantileDensityFunction(DistributionFunction(x));
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
            return new TukeyLambdaDistribution(lambda);
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
            return String.Format("Tukey(x; λ = {0})", lambda);
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
            return String.Format(formatProvider, "Tukey(x; λ = {0})", lambda);
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
            return String.Format(formatProvider, "Tukey(x; λ = {0})",
                lambda.ToString(format, formatProvider));
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
            return String.Format("Tukey(x; λ = {0})", lambda.ToString(format));
        }


        private void initialize(double lambda)
        {
            this.lambda = lambda;
        }


    }
}
