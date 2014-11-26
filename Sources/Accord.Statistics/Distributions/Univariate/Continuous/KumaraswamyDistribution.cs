// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Ashley Messer, 2014
// glyphard at gmail.com
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


namespace Accord.Statistics.Distributions.Univariate.Continuous
{

    using System;
    using Accord.Math;

    /// <summary>
    ///   Kumaraswamy distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability and statistics, the Kumaraswamy's double bounded distribution is a 
    ///   family of continuous probability distributions defined on the interval [0,1] differing 
    ///   in the values of their two non-negative shape parameters, a and b.
    ///   It is similar to the Beta distribution, but much simpler to use especially in simulation 
    ///   studies due to the simple closed form of both its probability density function and 
    ///   cumulative distribution function. This distribution was originally proposed by Poondi 
    ///   Kumaraswamy for variables that are lower and upper bounded.</para>
    ///
    /// <para>
    ///   A good example of the use of the Kumaraswamy distribution is the storage volume of a reservoir of capacity zmax whose upper bound is zmax and lower bound is 0 (Fletcher & Ponnambalam, 1996).</para>
    ///
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Kumaraswamy_distribution">
    ///       Wikipedia, The Free Encyclopedia. Kumaraswamy distribution. Available on: 
    ///       http://en.wikipedia.org/wiki/Kumaraswamy_distribution </a></description></item>
    ///   </list></para>     
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example shows how to create and test the main characteristics
    ///   of an Kumaraswamy distribution given its two non-negative shape parameters: </para>
    ///   
    /// <code>
    ///     var kumaraswamyDistribution = new KumaraswamyDistribution(0.2d, 1.2d);
    ///     double mean = kumaraswamyDistribution.Mean; //0.1258821823337952
    ///     double variance = kumaraswamyDistribution.Variance; //0.045500725605275683
    ///     double median = kumaraswamyDistribution.Median; //0.016262209853672775
    ///     double mode = kumaraswamyDistribution.Mode; //NaN  
    ///     
    ///     double pdf = kumaraswamyDistribution.ProbabilityDensityFunction(0.3); //0.46195081771596241
    ///     double cdf = kumaraswamyDistribution.CumulativeHazardFunction(0.3); //1.8501524192880519
    ///     string tostr = kumaraswamyDistribution.ToString(CultureInfo.InvariantCulture); // Kumaraswamy(x; a = 0.2, b = 1.2)
    ///     
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class KumaraswamyDistribution : UnivariateContinuousDistribution
    {
        double a;
        double b;

        public KumaraswamyDistribution([Positive] double a, [Positive] double b)
        {
            if (a <= 0.0d || b < 0.0d) {
                throw new ArgumentOutOfRangeException(string.Format("a and b must be positive: a={0}, b={1}", a, b));
            }
            this.a = a;
            this.b = b;
        }

        public override double Mean
        {
            get 
            {  
                return   ( b * Gamma.Function( 1.0d + ( 1.0d / a ) ) * Gamma.Function( b ) ) /
                            ( Gamma.Function( 1.0d + ( 1.0d / a ) + b ) );                        
            }
        }

        public override double Variance
        {
            get 
            {
                return (MomentGeneratingFunction(2, a, b) - Math.Pow(MomentGeneratingFunction(1, a, b), 2.0d));
            }
        }

        private double MomentGeneratingFunction(int n, double a, double b) 
        {
            return ( b * Beta.Function( 1.0d + ((double) n) / a, b ));
        }



        public override double Median
        {
            get
            {
                return Math.Pow( 1.0d - Math.Pow( 2.0d, -1.0d / b), 1.0d / a );
            }
        }

        public override double Mode
        {
            get
            {

                if ((a >= 1.0d) && (b >= 1.0d) && (a != 1.0d && b != 1.0d))
                {
                    return Math.Pow(
                    (a - 1.0d) /
                    (a * b - 1.0d)
                    , 1.0d / a);
                }
                return double.NaN;
            }
        }


        public override double Entropy
        {
            get { return double.NaN; }
        }

        public override AForge.DoubleRange Support
        {
            get 
            {
                return new AForge.DoubleRange( 0.0d, 1.0d );
            }
        }

        public override double DistributionFunction( double x )
        {
            return (1.0d - Math.Pow( 1.0d - Math.Pow( x, a ), b ) );
        }

        public override double ProbabilityDensityFunction(double x)
        {
            return a * b * Math.Pow(x, a - 1.0d) * Math.Pow(1.0d - Math.Pow(x, a), b - 1.0d);
        }

        public override object Clone()
        {
            return new KumaraswamyDistribution(this.a, this.b);
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
            return String.Format("Kumaraswamy(x; a = {0}, b = {1})", a, b);
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
            return String.Format(formatProvider, "Kumaraswamy(x; a = {0}, b = {1})",
                a, b);
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
            return String.Format("Kumaraswamy(x; a = {0}, b = {1})",
                a.ToString(format, formatProvider),
                b.ToString(format, formatProvider));
        }

    }
}
