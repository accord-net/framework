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
    /// var tukey = new TukeyLambdaDistribution(lambda: 0.14);
    /// 
    /// double mean = tukey.Mean;     // 0.0
    /// double median = tukey.Median; // 0.0
    /// double mode = tukey.Mode;     // 0.0
    /// double var = tukey.Variance;  // 2.1102970222144855
    /// double stdDev = tukey.StandardDeviation;  // 1.4526861402982014
    /// 
    /// double cdf = tukey.DistributionFunction(x: 1.4); // 0.83252947230217966
    /// double pdf = tukey.ProbabilityDensityFunction(x: 1.4); // 0.17181242109370659
    /// double lpdf = tukey.LogProbabilityDensityFunction(x: 1.4); // -1.7613519723149427
    /// 
    /// double ccdf = tukey.ComplementaryDistributionFunction(x: 1.4); // 0.16747052769782034
    /// double icdf = tukey.InverseDistributionFunction(p: cdf); // 1.4000000000000004
    /// 
    /// double hf = tukey.HazardFunction(x: 1.4); // 1.0219566231014163
    /// double chf = tukey.CumulativeHazardFunction(x: 1.4); // 1.7842102556452939
    /// 
    /// string str = tukey.ToString(CultureInfo.InvariantCulture); // Tukey(x; λ = 0.14)
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="LogisticDistribution"/>
    /// <seealso cref="UniformDistribution"/>
    /// <seealso cref="NormalDistribution"/>
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

        public override double Mode
        {
            get { return 0; }
        }

        public override double Entropy
        {
            get
            {
                double lower = 1e-15;
                double upper = 1.0 - 1e-15;

                return NonAdaptiveGaussKronrod
                    .Integrate(LogQuantileDensityFunction, lower, upper);
            }
        }

        public override double Variance
        {
            get
            {
                if (lambda == 0)
                    return (Math.PI * Math.PI) / 3.0;

                double a = 2.0 / (lambda * lambda);
                double b = 1 / (1 + 2 * lambda);
                double c = Gamma.Function(lambda + 1);
                double d = Gamma.Function(2 * lambda + 2);
                return a * (b - (c * c) / d);
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
            if (x < Support.Min) return 0;
            if (x > Support.Max) return 1;

            return BrentSearch.Find(InverseDistributionFunction, x, 0, 1, 1e-10);
        }

        public override double InverseDistributionFunction(double p)
        {
            if (lambda == 0)
                return Math.Log(p / (1 - p));

            double a = Math.Pow(p, lambda);
            double b = Math.Pow(1 - p, lambda);

            return (a - b) / lambda;
        }

        public override double QuantileDensityFunction(double p)
        {
            return Math.Pow(p, lambda - 1) + Math.Pow(1 - p, lambda - 1);
        }

        public double LogQuantileDensityFunction(double p)
        {
            double a = Math.Pow(p, lambda - 1);
            double b = Math.Pow(1 - p, lambda - 1);
            return Math.Log(a + b);
        }

        public override double ProbabilityDensityFunction(double x)
        {
            if (x < Support.Min) return 0;
            if (x > Support.Max) return 0;

            // http://www.ism.ac.jp/editsec/aism/pdf/044_4_0721.pdf
            return 1.0 / QuantileDensityFunction(DistributionFunction(x));
        }

        public override double LogProbabilityDensityFunction(double x)
        {
            if (x < Support.Min) return Double.NegativeInfinity;
            if (x > Support.Max) return Double.NegativeInfinity;

            return -LogQuantileDensityFunction(DistributionFunction(x));
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
