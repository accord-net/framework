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
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Compat;

    /// <summary>
    ///    Symmetric Geometric Distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Symmetric Geometric Distribution can be seen as a discrete case
    ///   of the <see cref="LaplaceDistribution"/>.</para>
    /// </remarks>
    /// 
    /// <seealso cref="GeometricDistribution"/>
    /// <seealso cref="HypergeometricDistribution"/>
    /// 
    [Serializable]
    public class SymmetricGeometricDistribution : UnivariateDiscreteDistribution
    {

        // Distribution parameters
        private double p;

        // Derived measures
        private double lnconstant;


        /// <summary>
        ///   Gets the success probability for the distribution.
        /// </summary>
        /// 
        public double ProbabilityOfSuccess
        {
            get { return p; }
        }


        /// <summary>
        ///   Creates a new symmetric geometric distribution.
        /// </summary>
        /// 
        /// <param name="probabilityOfSuccess">The success probability.</param>
        /// 
        public SymmetricGeometricDistribution([Unit] double probabilityOfSuccess)
        {
            if (probabilityOfSuccess < 0 || probabilityOfSuccess > 1)
                throw new ArgumentOutOfRangeException("probabilityOfSuccess",
                    "A probability must be between 0 and 1.");

            this.p = probabilityOfSuccess;
            this.lnconstant = Math.Log(p) - Math.Log(2.0 * (1.0 - p));
        }


        /// <summary>
        ///   Gets the support interval for this distribution, which
        ///   in the case of the Symmetric Geometric is [-inf, +inf].
        /// </summary>
        /// 
        /// <value>
        ///   A <see cref="IntRange" /> containing
        ///   the support interval for this distribution.
        /// </value>
        /// 
        public override IntRange Support
        {
            get { return new IntRange(Int32.MinValue, Int32.MaxValue); }
        }

        /// <summary>
        ///   Gets the mean for this distribution, which in 
        ///   the case of the Symmetric Geometric is zero.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's mean value.
        /// </value>
        /// 
        public override double Mean
        {
            get { return 0; } // TODO: Test
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        public override double Variance
        {
            get { return ((2 - p) * (1 - p)) / (p * p); }
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
        ///   Not supported.
        /// </summary>
        /// 
        protected internal override double InnerDistributionFunction(int k)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   Gets the probability mass function (pmf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="k">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The probability of <c>k</c> occurring
        ///   in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Mass Function (PMF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        protected internal override double InnerProbabilityMassFunction(int k)
        {
            if (k == 0)
                return p; // need special case otherwise will be double at center
            return Math.Exp(lnconstant) * Math.Pow(1 - p, Math.Abs(k) - 1);
        }

        /// <summary>
        ///   Gets the log-probability mass function (pmf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="k">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The logarithm of the probability of <c>x</c>
        ///   occurring in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Mass Function (PMF) describes the
        ///   probability that a given value <c>k</c> will occur.
        /// </remarks>
        /// 
        protected internal override double InnerLogProbabilityMassFunction(int k)
        {
            if (k == 0)
                return Math.Log(p); // need special case otherwise will be double at center
            return lnconstant + (Math.Abs(k) - 1) * Math.Log(1 - p);
        }

        /// <summary>
        /// Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///   
        /// <returns>
        /// A random observations drawn from this distribution.
        /// </returns>
        /// 
        public override int Generate(Random source)
        {
            double u = source.NextDouble();
            return Math.Sign(u - 0.5) * (int)GeometricDistribution.Random(p, source);
        }

        /// <summary>
        /// Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// 
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///   
        /// <returns>
        /// A random vector of observations drawn from this distribution.
        /// </returns>
        /// 
        public override double[] Generate(int samples, double[] result, Random source)
        {
            GeometricDistribution.Random(p, samples, result, source);
            for (int i = 0; i < samples; i++)
                result[i] *= Math.Sign(source.NextDouble() - 0.5);
            return result;
        }

        /// <summary>
        /// Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///   
        /// <returns>
        /// A random vector of observations drawn from this distribution.
        /// </returns>
        /// 
        public override int[] Generate(int samples, int[] result, Random source)
        {
            GeometricDistribution.Random(p, samples, result, source);
            for (int i = 0; i < samples; i++)
                result[i] *= Math.Sign(source.NextDouble() - 0.5);
            return result;
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
            return new SymmetricGeometricDistribution(p);
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
            return String.Format(formatProvider, "SymmetricGeometric(x; p = {0})",
                p.ToString(format, formatProvider));
        }

    }
}
