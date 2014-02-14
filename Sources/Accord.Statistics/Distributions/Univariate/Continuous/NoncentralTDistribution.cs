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
    using AForge;

    /// <summary>
    ///   Noncentral t-distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   As with other noncentrality parameters, the noncentral t-distribution generalizes 
    ///   a probability distribution – <see cref="TDistribution">Student's t-distribution</see>
    ///   – using a noncentrality parameter. Whereas the central distribution describes how a
    ///   test statistic is distributed when the difference tested is null, the noncentral 
    ///   distribution also describes how <c>t</c> is distributed when the null is false. 
    ///   This leads to its use in statistics, especially calculating statistical power. The
    ///   noncentral t-distribution is also known as the singly noncentral t-distribution, and
    ///   in addition to its primary use in statistical inference, is also used in robust 
    ///   modeling for data.</para>
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Noncentral_t-distribution">
    ///       Wikipedia, The Free Encyclopedia. Noncentral t-distribution. Available on:
    ///       http://en.wikipedia.org/wiki/Noncentral_t-distribution </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///   var distribution = new NoncentralTDistribution(
    ///         degreesOfFreedom: 4, noncentrality: 2.42);
    ///
    ///   double mean = distribution.Mean;     // 3.0330202123035104
    ///   double median = distribution.Median; // 2.6034842414893795
    ///   double var = distribution.Variance;  // 4.5135883917583683
    ///   
    ///   double cdf = distribution.DistributionFunction(x: 1.4); // 0.15955740661144721
    ///   double pdf = distribution.ProbabilityDensityFunction(x: 1.4); // 0.23552141805184526
    ///   double lpdf = distribution.LogProbabilityDensityFunction(x: 1.4); // -1.4459534225195116
    ///   
    ///   double ccdf = distribution.ComplementaryDistributionFunction(x: 1.4); // 0.84044259338855276
    ///   double icdf = distribution.InverseDistributionFunction(p: cdf); // 1.4000000000123853
    ///   
    ///   double hf = distribution.HazardFunction(x: 1.4); // 0.28023498559521387
    ///   double chf = distribution.CumulativeHazardFunction(x: 1.4); // 0.17382662901507062
    ///   
    ///   string str = distribution.ToString(CultureInfo.InvariantCulture); // T(x; df = 4, μ = 2.42)
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="TDistribution"/>
    /// <seealso cref="Accord.Statistics.Testing.TTest"/>
    /// 
    [Serializable]
    public class NoncentralTDistribution : UnivariateContinuousDistribution, IFormattable
    {

        /// <summary>
        ///   Gets the degrees of freedom (v) for the distribution.
        /// </summary>
        /// 
        public double DegreesOfFreedom { get; private set; }

        /// <summary>
        ///   Gets the noncentrality parameter μ (mu) for the distribution.
        /// </summary>
        /// 
        public double Noncentrality { get; private set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="TDistribution"/> class.
        /// </summary>
        /// 
        /// <param name="degreesOfFreedom">The degrees of freedom v.</param>
        /// <param name="noncentrality">The noncentrality parameter μ (mu).</param>
        /// 
        public NoncentralTDistribution(double degreesOfFreedom, double noncentrality)
        {
            if (degreesOfFreedom <= 0)
                throw new ArgumentOutOfRangeException("degreesOfFreedom");

            this.DegreesOfFreedom = degreesOfFreedom;
            this.Noncentrality = noncentrality;
        }


        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <remarks>
        ///   The noncentral t-distribution's mean is defined in terms of
        ///   the <see cref="Gamma.Function(double)">Gamma function Γ(x)</see> as 
        ///   <c>μ * sqrt(v/2) * Γ((v - 1) / 2) / Γ(v / 2) for v > 1</c>.
        /// </remarks>
        /// 
        public override double Mean
        {
            get
            {
                double v = DegreesOfFreedom;
                double u = Noncentrality; // μ
                if (v > 1)
                    return u * Math.Sqrt(v / 2.0) * (Gamma.Function((v - 1) / 2)) / Gamma.Function(v / 2);
                return Double.NaN;
            }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <remarks>
        ///   The noncentral t-distribution's variance is defined in terms of
        ///   the <see cref="Gamma.Function(double)">Gamma function Γ(x)</see> as 
        ///   <c>a - b * c²</c> in which
        ///   <c>a = v*(1+μ²) / (v-2)</c>,
        ///   <c>b = (u² * v) / 2</c> and
        ///   <c>c = Γ((v - 1) / 2) / Γ(v / 2)</c> for <c>v > 2</c>.
        /// </remarks>
        /// 
        public override double Variance
        {
            get
            {
                double v = DegreesOfFreedom;
                double u = Noncentrality; // μ

                if (v > 2)
                {
                    double a = (v * (1 + u * u)) / (v - 2);
                    double b = (u * u * v) / 2;
                    double c = Gamma.Function((v - 1) / 2) / Gamma.Function(v / 2);
                    return a - b * c * c;
                }

                return Double.NaN;
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
        ///   A <see cref="AForge.DoubleRange" /> containing
        ///   the support interval for this distribution.
        /// </value>
        /// 
        public override DoubleRange Support
        {
            get { return new DoubleRange(Double.NegativeInfinity, Double.PositiveInfinity); }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// 
        /// <example>
        ///   See <see cref="NoncentralTDistribution"/>.
        /// </example>
        /// 
        public override double DistributionFunction(double x)
        {
            return distributionFunctionLowerTail(x, DegreesOfFreedom, Noncentrality);
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
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        /// <example>
        ///   See <see cref="NoncentralTDistribution"/>.
        /// </example>
        /// 
        public override double ProbabilityDensityFunction(double x)
        {
            double u = Noncentrality; // μ
            double v = DegreesOfFreedom;
            Func<double, double, double, double> F = distributionFunctionLowerTail;

            if (x != 0)
            {
                double A = F(x * Math.Sqrt(1 + 2 / v), v + 2, u);
                double B = F(x, v, u);
                double C = v / x;
                return C * (A - B);
            }
            else
            {
                double A = Gamma.Function((v + 1) / 2);
                double B = Math.Sqrt(Math.PI * v) * Gamma.Function(v / 2);
                double C = Math.Exp(-(u * u) / 2);
                return (A / B) * C;
            }
        }


        /// <summary>
        ///  Not supported.
        /// </summary>
        /// 
        public override void Fit(double[] observations, double[] weights, Fitting.IFittingOptions options)
        {
            throw new NotSupportedException();
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
            return new NoncentralTDistribution(DegreesOfFreedom, Noncentrality);
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
            return String.Format("T(x; df = {0}, μ = {1})", DegreesOfFreedom, Noncentrality);
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
            return String.Format(formatProvider, "T(x; df = {0}, μ = {1})", DegreesOfFreedom, Noncentrality);
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
            return String.Format(formatProvider, "T(x; df = {0}, μ = {1})",
                DegreesOfFreedom.ToString(format, formatProvider),
                Noncentrality.ToString(format, formatProvider));
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
            return String.Format("T(x; df = {0}, μ = {1})",
                DegreesOfFreedom.ToString(format), Noncentrality.ToString(format));
        }

        /// <summary>
        ///   Computes the cumulative probability at <c>t</c> of the
        ///   non-central T-distribution with DF degrees of freedom 
        ///   and non-centrality parameter.
        /// </summary>
        /// 
        /// <remarks>
        ///   This function is based on the original work done by
        ///   Russell Lent hand John Burkardt, shared under the
        ///   LGPL license. Original FORTRAN code can be found at:
        ///   http://people.sc.fsu.edu/~jburkardt/f77_src/asa243/asa243.html
        /// </remarks>
        /// 
        private static double distributionFunctionLowerTail(double t, double df, double delta)
        {
            double alnrpi = 0.57236494292470008707;
            double errmax = 1.0E-10;
            int itrmax = 100;
            double r2pi = 0.79788456080286535588;

            if (df <= 0.0)
            {
                throw new ArgumentOutOfRangeException("df",
                    "Degrees of freedom must be positive.");
            }

            double tt;
            double del;
            bool negdel;

            if (t < 0.0)
            {
                tt = -t;
                del = -delta;
                negdel = true;
            }
            else
            {
                tt = t;
                del = delta;
                negdel = false;
            }

            // Initialize twin series.
            double en = 1.0;
            double x = t * t / (t * t + df);
            double value = 0;

            if (x <= 0.0)
            {
                // upper tail of normal cumulative function
                value += Normal.Complemented(del);

                if (negdel)
                    value = 1.0 - value;
                return value;
            }

            double lambda = del * del;
            double p = 0.5 * Math.Exp(-0.5 * lambda);
            double q = r2pi * p * del;
            double s = 0.5 - p;
            double a = 0.5;
            double b = 0.5 * df;
            double rxb = Math.Pow(1.0 - x, b);
            double albeta = alnrpi + Gamma.Log(b) - Gamma.Log(a + b);
            double xodd = Beta.Incomplete(a, b, x);
            double godd = 2.0 * rxb * Math.Exp(a * Math.Log(x) - albeta);
            double xeven = 1.0 - rxb;
            double geven = b * x * rxb;
            value = p * xodd + q * xeven;

            // Repeat until convergence.
            while (true)
            {
                a = a + 1.0;
                xodd = xodd - godd;
                xeven = xeven - geven;
                godd = godd * x * (a + b - 1.0) / a;
                geven = geven * x * (a + b - 0.5) / (a + 0.5);
                p = p * lambda / (2.0 * en);
                q = q * lambda / (2.0 * en + 1.0);
                s = s - p;
                en = en + 1.0;
                value = value + p * xodd + q * xeven;
                double errbd = 2.0 * s * (xodd - godd);

                if (errbd <= errmax)
                    break;

                if (itrmax < en)
                    throw new ConvergenceException("Maximum number of iterations reached.");
            }

            // upper tail of normal cumulative function
            value = value + Normal.Complemented(del);

            if (negdel)
                value = 1.0 - value;

            return value;
        }

    }
}
