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
    using Accord.Statistics.Distributions.Fitting;
    using AForge;

    /// <summary>
    ///   von-Mises (Circular Normal) distribution.
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>The von Mises distribution (also known as the circular normal distribution
    ///   or Tikhonov distribution) is a continuous probability distribution on the circle.
    ///   It may be thought of as a close approximation to the wrapped normal distribution,
    ///   which is the circular analogue of the normal distribution.</para>
    ///   
    ///   <para>The wrapped normal distribution describes the distribution of an angle that
    ///   is the result of the addition of many small independent angular deviations, such as
    ///   target sensing, or grain orientation in a granular material. The von Mises distribution
    ///   is more mathematically tractable than the wrapped normal distribution and is the
    ///   preferred distribution for many applications.</para>
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Von_Mises_distribution">
    ///       Wikipedia, The Free Encyclopedia. Von-Mises distribution. Available on:
    ///       http://en.wikipedia.org/wiki/Von_Mises_distribution </a></description></item>
    ///     <item><description><a href="http://www.kyb.mpg.de/publications/attachments/vmfnote_7045%5B0%5D.pdf">
    ///       Suvrit Sra, "A short note on parameter approximation for von Mises-Fisher distributions:
    ///       and a fast implementation of $I_s(x)$". (revision of Apr. 2009). Computational Statistics (2011).
    ///       Available on: http://www.kyb.mpg.de/publications/attachments/vmfnote_7045%5B0%5D.pdf </a></description></item>
    ///     <item><description>
    ///       Zheng Sun. M.Sc. Comparing measures of fit for circular distributions. Master thesis, 2006.
    ///       Available on: https://dspace.library.uvic.ca:8443/bitstream/handle/1828/2698/zhengsun_master_thesis.pdf </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///    // Create a new von-Mises distribution with μ = 0.42 and κ = 1.2
    ///    var vonMises = new VonMisesDistribution(mean: 0.42, concentration: 1.2);
    ///    
    ///    // Common measures
    ///    double mean = vonMises.Mean;     // 0.42
    ///    double median = vonMises.Median; // 0.42
    ///    double var = vonMises.Variance;  // 0.48721760532782921
    ///    
    ///    // Cumulative distribution functions
    ///    double cdf = vonMises.DistributionFunction(x: 1.4); // 0.81326928491589345
    ///    double ccdf = vonMises.ComplementaryDistributionFunction(x: 1.4); // 0.18673071508410655
    ///    double icdf = vonMises.InverseDistributionFunction(p: cdf); // 1.3999999637927665
    ///    
    ///    // Probability density functions
    ///    double pdf = vonMises.ProbabilityDensityFunction(x: 1.4); // 0.2228112141141676
    ///    double lpdf = vonMises.LogProbabilityDensityFunction(x: 1.4); // -1.5014304395467863
    ///    
    ///    // Hazard (failure rate) functions
    ///    double hf = vonMises.HazardFunction(x: 1.4); // 1.1932220899695576
    ///    double chf = vonMises.CumulativeHazardFunction(x: 1.4); // 1.6780877262500649
    ///    
    ///    // String representation
    ///    string str = vonMises.ToString(CultureInfo.InvariantCulture); // VonMises(x; μ = 0.42, κ = 1.2)
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="Circular"/>
    /// 
    [Serializable]
    public class VonMisesDistribution : UnivariateContinuousDistribution,
        IFittableDistribution<double, VonMisesOptions>
    {

        // Distribution parameters
        private double mean;  // μ (mu)
        private double kappa; // κ (kappa)

        // Distribution measures
        private double? variance;
        private double? entropy;

        // Derived measures
        private double constant;


        /// <summary>
        ///   Constructs a von-Mises distribution.
        /// </summary>
        /// 
        /// <param name="mean">The mean value μ (mu).</param>
        /// <param name="concentration">The concentration value κ (kappa).</param>
        /// 
        public VonMisesDistribution(double mean, double concentration)
        {
            initialize(mean, concentration);
        }

        private VonMisesDistribution()
        {
        }

        private void initialize(double m, double k)
        {
            this.mean = m;
            this.kappa = k;

            this.variance = null;
            this.constant = 1.0 / (2.0 * Math.PI * Bessel.I0(kappa));
        }

        /// <summary>
        ///   Gets the mean value μ (mu) for this distribution.
        /// </summary>
        /// 
        public override double Mean
        {
            get { return mean; }
        }

        /// <summary>
        ///   Gets the median value μ (mu) for this distribution.
        /// </summary>
        /// 
        public override double Median
        {
            get { return mean; }
        }

        /// <summary>
        ///   Gets the concentration κ (kappa) for this distribution.
        /// </summary>
        /// 
        public double Concentration
        {
            get { return kappa; }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <remarks>
        ///   The von-Mises Variance is defined in terms of the
        ///   <see cref="Bessel.I(double)">Bessel function of the first 
        ///   kind In(x)</see> as <c>var = 1 - I(1, κ) / I(0, κ)</c>
        /// </remarks>
        /// 
        public override double Variance
        {
            get
            {
                if (!variance.HasValue)
                {
                    double i1 = Bessel.I(1, kappa);
                    double i0 = Bessel.I0(kappa);
                    double a = i1 / i0;
                    variance = 1.0 - a;
                }

                return variance.Value;
            }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        public override double Entropy
        {
            get
            {
                if (!entropy.HasValue)
                {
                    double i1 = Bessel.I(1, kappa);
                    double i0 = Bessel.I0(kappa);
                    double a = i1 / i0;
                    entropy = -kappa * a + Math.Log(2 * Math.PI * i0);
                }

                return entropy.Value;
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
            get { return new DoubleRange(-Math.PI, Math.PI); }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        public override double DistributionFunction(double x)
        {
            return DistributionFunction(x, mean, kappa);
        }


        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        public override double ProbabilityDensityFunction(double x)
        {
            return constant * Math.Exp(kappa * Math.Cos(x - mean));
        }

        /// <summary>
        ///   Gets the log-probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The logarithm of the probability of <c>x</c>
        ///   occurring in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        public override double LogProbabilityDensityFunction(double x)
        {
            return Math.Log(constant) + kappa * Math.Cos(x - mean);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// 
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, such
        /// as regularization constants and additional parameters.</param>
        /// 
        /// <remarks>
        ///   Although both double[] and double[][] arrays are supported,
        ///   providing a double[] for a multivariate distribution or a
        ///   double[][] for a univariate distribution may have a negative
        ///   impact in performance.
        /// </remarks>
        /// 
        public override void Fit(double[] observations, double[] weights, IFittingOptions options)
        {
            VonMisesOptions misesOptions = options as VonMisesOptions;
            if (options != null && misesOptions == null)
                throw new ArgumentException("The specified options' type is invalid.", "options");

            Fit(observations, weights, misesOptions);
        }
        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// 
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, such
        /// as regularization constants and additional parameters.</param>
        /// 
        /// <remarks>
        ///   Although both double[] and double[][] arrays are supported,
        ///   providing a double[] for a multivariate distribution or a
        ///   double[][] for a univariate distribution may have a negative
        ///   impact in performance.
        /// </remarks>
        /// 
        public void Fit(double[] observations, double[] weights, VonMisesOptions options)
        {
            double m, k;

            if (weights != null)
            {
                m = Circular.WeightedMean(observations, weights);
                k = Circular.WeightedConcentration(observations, weights, m);
            }
            else
            {
                m = Circular.Mean(observations);
                k = Circular.Concentration(observations, m);
            }

            if (options != null)
            {
                // Parse optional estimation options
                if (options.UseBiasCorrection)
                {
                    double N = observations.Length;
                    if (k < 2)
                    {
                        k = System.Math.Max(k - 1.0 / (2.0 * (N * k)), 0);
                    }
                    else
                    {
                        double Nm1 = N - 1;
                        k = (Nm1 * Nm1 * Nm1 * k) / (N * N * N + N);
                    }
                }
            }

            initialize(m, k);
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
            return new VonMisesDistribution(mean, kappa);
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
            return String.Format("VonMises(x; μ = {0}, κ = {1})", mean, kappa);
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
            return String.Format(formatProvider, "VonMises(x; μ = {0}, κ = {1})", mean, kappa);
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
            return String.Format(formatProvider, "VonMises(x; μ = {0}, κ = {1})",
                mean.ToString(format, formatProvider),
                kappa.ToString(format, formatProvider));
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
            return String.Format("VonMises(x; μ = {0}, κ = {1})",
                mean.ToString(format), kappa.ToString(format));
        }


        /// <summary>
        ///   Estimates a new von-Mises distribution from a given set of angles.
        /// </summary>
        /// 
        public static VonMisesDistribution Estimate(double[] angles)
        {
            return Estimate(angles, null, null);
        }

        /// <summary>
        ///   Estimates a new von-Mises distribution from a given set of angles.
        /// </summary>
        /// 
        public static VonMisesDistribution Estimate(double[] angles, VonMisesOptions options)
        {
            return Estimate(angles, null, options);
        }

        /// <summary>
        ///   Estimates a new von-Mises distribution from a given set of angles.
        /// </summary>
        /// 
        public static VonMisesDistribution Estimate(double[] angles, double[] weights, VonMisesOptions options)
        {
            VonMisesDistribution vonMises = new VonMisesDistribution();
            vonMises.Fit(angles, weights, options);
            return vonMises;
        }


        /// <summary>
        ///   von-Mises cumulative distribution function.
        /// </summary>
        /// 
        /// <remarks>
        ///   This method implements the Von-Mises CDF calculation code
        ///   as given by Geoffrey Hill on his original FORTRAN code and
        ///   shared under the GNU LGPL license.
        /// 
        /// <para>    
        ///   References:
        ///   <list type="bullet">
        ///     <item><description>Geoffrey Hill, ACM TOMS Algorithm 518,
        ///     Incomplete Bessel Function I0: The von Mises Distribution,
        ///     ACM Transactions on Mathematical Software, Volume 3, Number 3,
        ///     September 1977, pages 279-284.</description></item>
        ///   </list></para>
        /// </remarks>
        /// 
        /// <param name="x">The point where to calculate the CDF.</param>
        /// <param name="mu">The location parameter μ (mu).</param>
        /// <param name="kappa">The concentration parameter κ (kappa).</param>
        /// 
        /// <returns>The value of the von-Mises CDF at point <paramref name="x"/>.</returns>
        /// 
        public static double DistributionFunction(double x, double mu, double kappa)
        {
            double a1 = 12.0;
            double a2 = 0.8;
            double a3 = 8.0;
            double a4 = 1.0;
            double c1 = 56.0;
            double ck = 10.5;
            double cdf = 0;

            if (x - mu <= -Math.PI)
                return 0;

            if (Math.PI <= x - mu)
                return 1.0;

            double z = kappa;

            double u = (x - mu + Math.PI) % (2.0 * Math.PI);

            if (u < 0.0)
                u = u + 2.0 * Math.PI;

            double y = u - Math.PI;

            if (z <= ck)
            {
                double v = 0.0;

                if (0.0 < z)
                {
                    double ip = Math.Floor(z * a2 - a3 / (z + a4) + a1);
                    double p = ip;
                    double s = Math.Sin(y);
                    double c = Math.Cos(y);
                    y = p * y;
                    double sn = Math.Sin(y);
                    double cn = Math.Cos(y);
                    double r = 0.0;
                    z = 2.0 / z;

                    for (int n = 2; n <= ip; n++)
                    {
                        p = p - 1.0;
                        y = sn;
                        sn = sn * c - cn * s;
                        cn = cn * c + y * s;
                        r = 1.0 / (p * z + r);
                        v = (sn / p + v) * r;
                    }

                }

                cdf = (u * 0.5 + v) / Math.PI;
            }
            else
            {
                double c = 24.0 * z;
                double v = c - c1;
                double r = Math.Sqrt((54.0 / (347.0 / v + 26.0 - c) - 6.0 + c) / 12.0);
                z = Math.Sin(0.5 * y) * r;
                double s = 2.0 * z * z;
                v = v - s + 3.0;
                y = (c - s - s - 16.0) / 3.0;
                y = ((s + 1.75) * s + 83.5) / v - y;
                double arg = z * (1.0 - s / (y * y));
                double erfx = Special.Erf(arg);
                cdf = 0.5 * erfx + 0.5;
            }

            cdf = Math.Max(cdf, 0.0);
            cdf = Math.Min(cdf, 1.0);

            return cdf;
        }

    }
}
