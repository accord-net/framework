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
    ///   Power Lognormal distribution.
    /// </summary>
    /// 
    [Serializable]
    public class PowerLognormalDistribution : UnivariateContinuousDistribution
    {

        // Distribution parameters
        private double power = 1; // power (p)
        private double sigma = 1; // sigma (σ)

        /// <summary>
        ///   Constructs a Power Lognormal distribution
        ///   with the given power and shape parameters.
        /// </summary>
        /// 
        /// <param name="mean">The distribution's power p.</param>
        /// <param name="mean">The distribution's shape σ.</param>
        /// 
        public PowerLognormalDistribution(double power, double shape)
        {
            if (power <= 0)
                throw new ArgumentOutOfRangeException("power", "Power must be positive.");

            if (shape <= 0)
                throw new ArgumentOutOfRangeException("shape", "Shape must be positive.");

            initialize(power, shape);
        }

        /// <summary>
        ///   Gets the distribution shape (power) parameter.
        /// </summary>
        /// 
        public double Power
        {
            get { return power; }
        }

        public double Shape
        {
            get { return sigma; }
        }

        public override double Mean
        {
            get { throw new NotSupportedException(); }
        }


        public override double Median
        {
            get { throw new NotSupportedException(); }
        }


        public override double Variance
        {
            get { throw new NotSupportedException(); }
        }

        public override double StandardDeviation
        {
            get { throw new NotSupportedException(); }
        }

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
            get { return new DoubleRange(0, Double.PositiveInfinity); }
        }

        public override double DistributionFunction(double x)
        {
            double z = Math.Log(x) / sigma;
            return 1.0 - Math.Pow(Normal.Function(-z), power);
        }

        public override double InverseDistributionFunction(double p)
        {
            return Math.Exp(Normal.Inverse(1.0 - Math.Pow(1.0 - p, 1.0 / power)) * sigma);
        }

        public override double ProbabilityDensityFunction(double x)
        {
            double a = power / (x * sigma);
            double z = Math.Log(x) / sigma;

            return a * Normal.Derivative(z) * Math.Pow(Normal.Function(-z), power - 1);
        }


        public override double HazardFunction(double x)
        {
            double a = power / (x * sigma);
            double z = Math.Log(x) / sigma;

            double num = power * a * Normal.Derivative(z);
            double den = Normal.Function(-z);
            return num / den;
        }

        public override double CumulativeHazardFunction(double x)
        {
            double z = Math.Log(x) / sigma;
            return -Math.Log(Math.Pow(Normal.Function(-z), power));
        }

        public override double ComplementaryDistributionFunction(double x)
        {
            double z = Math.Log(x) / sigma;
            return Math.Pow(Normal.Function(-z), power);
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
            return new PowerLognormalDistribution(power, sigma);
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
            return String.Format("PKD(x; p = {0}, σ = {1})", power);
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
            return String.Format(formatProvider, "PLD(x; p = {0}, σ = {1})", 
                power, sigma);
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
            return String.Format(formatProvider, "PLD(x; p = {0}, σ = {1})",
                power.ToString(format, formatProvider),
                sigma.ToString(format, formatProvider));
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
            return String.Format("PLD(x; p = {0}, σ = {1})",
                power.ToString(format), sigma.ToString(format));
        }


        private void initialize(double power, double sigma)
        {
            this.power = power;
            this.sigma = sigma;
        }

    }
}
