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
    using System.ComponentModel.DataAnnotations;
    using Accord.Math;
    using AForge;
    using Accord.Statistics.Testing;
    using Accord;
    using Accord.Compat;

    /// <summary>
    ///   Grubb's statistic distribution.
    /// </summary>
    /// 
    /// <seealso cref="GrubbTest"/>
    /// 
    [Serializable]
    public class GrubbDistribution : UnivariateContinuousDistribution
    {
        private TDistribution tDistribution;

        /// <summary>
        /// Gets the number of samples for the distribution.
        /// </summary>
        /// 
        public int NumberOfSamples { get; private set; }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public override double Mean
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public override double Variance
        {
            get { throw new NotSupportedException(); }
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
        /// Gets the support interval for this distribution.
        /// </summary>
        /// <value>A <see cref="DoubleRange" /> containing
        /// the support interval for this distribution.</value>
        public override DoubleRange Support
        {
            get
            {
                return new DoubleRange(InnerInverseDistributionFunction(0), (NumberOfSamples - 1.0) / Math.Sqrt(NumberOfSamples));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GrubbDistribution"/> class.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples.</param>
        /// 
        public GrubbDistribution([PositiveInteger(minimum: 3)] int samples)
        {
            if (samples <= 2)
                throw new ArgumentOutOfRangeException("samples", "Number of samples must be greater than two.");

            this.NumberOfSamples = samples;
            this.tDistribution = new TDistribution(samples - 2);
        }



        /// <summary>
        /// Gets the cumulative distribution function (cdf) for
        /// this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        /// <returns>System.Double.</returns>
        /// <remarks>The Cumulative Distribution Function (CDF) describes the cumulative
        /// probability that a given value or any value smaller than it will occur.</remarks>
        protected internal override double InnerDistributionFunction(double x)
        {
            // http://graphpad.com/support/faqid/1598/
            double N = NumberOfSamples;
            double num = N * (N - 2) * x * x;
            double den = ((N - 1) * (N - 1)) - N * x * x;
            double t = Math.Sqrt(num / den);
            double p = tDistribution.ComplementaryDistributionFunction(t);
            double r = 1.0 - N * p;
            if (r > 1)
                return 1;
            if (r < 0)
                return 0;
            return r;
        }

        /// <summary>
        /// Gets the inverse of the cumulative distribution function (icdf) for
        /// this distribution evaluated at probability <c>p</c>. This function
        /// is also known as the Quantile function.
        /// </summary>
        /// <param name="p">A probability value between 0 and 1.</param>
        /// <returns>A sample which could original the given probability
        /// value when applied in the <see cref="UnivariateContinuousDistribution.DistributionFunction(double)" />.</returns>
        /// <remarks>The Inverse Cumulative Distribution Function (ICDF) specifies, for
        /// a given probability, the value which the random variable will be at,
        /// or below, with that probability.</remarks>
        protected internal override double InnerInverseDistributionFunction(double p)
        {
            // https://www.wolframalpha.com/input/?i=sqrt((N+*(+N-+2)*x%C2%B2)+%2F+((N-1)%C2%B2+-+N*x%C2%B2))+%3D+t+solve+for+x
            double N = NumberOfSamples;
            double t = tDistribution.InverseDistributionFunction((1 - p) / N);
            double a = (N - 1.0) / Math.Sqrt(N);
            double b = Math.Sqrt((t * t) / (N + t * t - 2));
            double r = a * b;
            return r;
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        protected internal override double InnerProbabilityDensityFunction(double x)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new GrubbDistribution(NumberOfSamples);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format("Grubb(x; n = {0})", NumberOfSamples);
        }
    }
}
