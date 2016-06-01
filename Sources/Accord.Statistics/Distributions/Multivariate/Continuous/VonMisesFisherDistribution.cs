// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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

namespace Accord.Statistics.Distributions.Multivariate
{
    using System;
    using Accord.Math;
    using Accord.Math.Decompositions;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Univariate;

    /// <summary>
    ///   Von-Mises Fisher distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In directional statistics, the von Mises–Fisher distribution is a probability distribution 
    ///   on the (p-1)-dimensional sphere in R^p. If p = 2 the distribution reduces to the <see cref="VonMisesDistribution">
    ///   von Mises distribution on the circle</see>.</para>
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia, The Free Encyclopedia. Von Mises-Fisher Distribution. Available on:
    ///       <a href=" https://en.wikipedia.org/wiki/Von_Mises%E2%80%93Fisher_distribution">
    ///        https://en.wikipedia.org/wiki/Von_Mises%E2%80%93Fisher_distribution </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <seealso cref="VonMisesDistribution"/>
    /// 
    [Serializable]
    public class VonMisesFisherDistribution : MultivariateContinuousDistribution
    {

        // Distribution parameters
        private double[] mean; // μ (mu)
        private double kappa;  // κ (kappa)

        private double constant;


        /// <summary>
        ///   Constructs a Von-Mises Fisher distribution with unit mean.
        /// </summary>
        /// 
        /// <param name="dimension">The number of dimensions in the distribution.</param>
        /// <param name="concentration">The concentration value κ (kappa).</param>
        /// 
        public VonMisesFisherDistribution(int dimension, double concentration)
            : base(dimension)
        {
            if (concentration < 0)
                throw new ArgumentOutOfRangeException("concentration", "Concentration parameter kappa must be non-negative.");

            for (int i = 0; i < mean.Length; i++)
                mean[i] = 1 / Math.Sqrt(mean.Length);
        }

        /// <summary>
        ///   Constructs a Von-Mises Fisher distribution with unit mean.
        /// </summary>
        /// 
        /// <param name="mean">The mean direction vector (with unit length).</param>
        /// <param name="concentration">The concentration value κ (kappa).</param>
        /// 
        public VonMisesFisherDistribution(double[] mean, double concentration)
            : base(mean.Length)
        {
            if (concentration < 0)
                throw new ArgumentOutOfRangeException("concentration", "Concentration parameter kappa must be non-negative.");

            if (!Norm.Euclidean(mean).IsEqual(1, 1e-10))
                throw new ArgumentOutOfRangeException("mean", "The mean vector must have unit length.");

            this.mean = mean;
            this.kappa = concentration;

            int p = Dimension;
            double num = Math.Pow(concentration, p / 2 - 1);
            double den = Math.Pow(2 * Math.PI, p / 2) * Bessel.I(p / 2 - 1, concentration);
            this.constant = num / den;
        }


        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   A vector containing the mean values for the distribution.
        /// </value>
        /// 
        public override double[] Mean
        {
            get { return mean; }
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public override double[] Variance
        {
            get { return null; }
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public override double[,] Covariance
        {
            get { return null; }
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public override double DistributionFunction(params double[] x)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        ///   Gets the probability density function (pdf) for this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range. For a univariate distribution, this should be
        ///   a  single double value. For a multivariate distribution, this should be a double array.
        /// </param>
        ///   
        /// <returns>
        ///   The probability of <c>x</c> occurring in the current distribution.
        /// </returns>
        /// 
        /// <exception cref="DimensionMismatchException">x;The vector should have the same dimension as the distribution.</exception>
        /// 
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        public override double ProbabilityDensityFunction(params double[] x)
        {
            if (x.Length != Dimension)
                throw new DimensionMismatchException("x", "The vector should have the same dimension as the distribution.");

            double[] z = x.Normalize(Norm.Euclidean);
            double d = mean.Dot(z);
            return constant * Math.Exp(kappa * d);
        }


        private VonMisesFisherDistribution(int dimension)
            : base(dimension)
        {
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public override object Clone()
        {
            var clone = new VonMisesFisherDistribution(this.Dimension);
            clone.constant = constant;
            clone.mean = (double[])mean.Clone();
            clone.kappa = kappa;

            return clone;
        }


        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// 
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// 
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// 
        public override string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format(formatProvider, "VonMises-Fisher(X; μ, κ = {0})", kappa);
        }

    }
}
