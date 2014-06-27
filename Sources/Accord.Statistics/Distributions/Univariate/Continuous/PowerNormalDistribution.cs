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
    ///   Power Normal distribution.
    /// </summary>
    /// 
    [Serializable]
    public class PowerNormalDistribution : UnivariateContinuousDistribution
    {

        // Distribution parameters
        private double power = 1; // power (p)

        /// <summary>
        ///   Constructs a Power Normal distribution
        ///   with given power (shape) parameter.
        /// </summary>
        /// 
        /// <param name="mean">The distribution's power p.</param>
        /// 
        public PowerNormalDistribution(double power)
        {
            if (power <= 0)
                throw new ArgumentOutOfRangeException("power", "Power must be positive.");

            initialize(power);
        }

        /// <summary>
        ///   Gets the distribution shape (power) parameter.
        /// </summary>
        /// 
        public double Power
        {
            get { return power; }
        }


        public override double Mean
        {
            get { throw new NotSupportedException(); }
        }


        public override double Median
        {
            get { throw new NotSupportedException(); }
        }

        public override double Mode
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
            return 1.0 - Math.Pow(Normal.Function(-x), power);
        }

        public override double InverseDistributionFunction(double p)
        {
            return Normal.Inverse(1.0 - Math.Pow(1.0 - p, 1.0 / power));
        }

        public override double ProbabilityDensityFunction(double x)
        {
            return power * Normal.Derivative(x) * Math.Pow(Normal.Function(-x), power - 1);
        }

        public override double LogProbabilityDensityFunction(double x)
        {
            return Math.Log(power) + Normal.LogDerivative(x) + (power - 1) * Normal.Function(-x);
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
            return new PowerNormalDistribution(power);
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
            return String.Format("PND(x; p = {0})", power);
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
            return String.Format(formatProvider, "PND(x; p = {0})", power);
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
            return String.Format(formatProvider, "PND(x; p = {0})",
                power.ToString(format, formatProvider));
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
            return String.Format("PND(x; p = {0})", power.ToString(format));
        }


        private void initialize(double power)
        {
            this.power = power;
        }

    }
}
