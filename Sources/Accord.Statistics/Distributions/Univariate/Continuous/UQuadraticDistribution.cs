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
    using AForge;
    /// <summary>
    ///   U-quadratic distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    /// 
    /// </para>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/U-quadratic_distribution">
    ///       Wikipedia, The Free Encyclopedia. U-quadratic distribution. Available on: 
    ///       http://en.wikipedia.org/wiki/U-quadratic_distribution </a></description></item>
    ///   </list></para>     
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example shows how to create and test the main characteristics
    ///   of an U-quadratic distribution given its two parameters: </para>
    ///   
    /// <code>
    ///     double a = -2.0d;
    ///     double b = 2.0d;
    ///     double x = 0.0d;
    ///
    ///     var uQuadDist = new UQuadraticDistribution(a, b);
    ///     double mean = uQuadDist.Mean; //0.0
    ///     double variance = uQuadDist.Variance; //2.4
    ///     double median = uQuadDist.Median; //0.0
    ///     double pdf = uQuadDist.ProbabilityDensityFunction(x); //0.0
    ///     double cdf = uQuadDist.DistributionFunction(x); //0.5
    ///     string tostr = uQuadDist.ToString(); //UQuadratic(x; a = -2, b = 2)
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class UQuadraticDistribution : UnivariateContinuousDistribution
    {
        //parameters
        double a;
        double b;
        double alpha;
        double beta;

        public UQuadraticDistribution([Real] double a, [Real] double b) {
            if (a >= b) { 
                throw new ArgumentOutOfRangeException(string.Format("a must be smaller than b (a={0}, b={1})", a, b));
            }
            this.a = a;
            this.b = b;
            this.alpha = 12.0d / Math.Pow( b - a, 3.0d );
            this.beta = ( b + a ) / 2.0d ;
        }

        public override double Mean
        {
            get { return (a + b) / 2.0d; }
        }

        public override double Median
        {
            get { return (a + b) / 2.0d; }
        }

        public override double Variance
        {
            get { return ( 3.0d / 20.0d ) * Math.Pow( b - a, 2.0d ); }
        }

        public override double Entropy
        {
            get { return double.NaN; }
        }

        public override DoubleRange Support
        {
            get { return new DoubleRange(a, b); }
        }

        public override double DistributionFunction(double x)
        {
            return ( alpha / 3.0d ) * ( Math.Pow( x - beta, 3.0d ) + Math.Pow( beta - a, 3.0d ) );
        }

        public override double ProbabilityDensityFunction(double x)
        {
            return alpha * Math.Pow( x - beta, 2.0d );
        }

        public override object Clone()
        {
            return new UQuadraticDistribution( this.a, this.b );
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
            return String.Format("UQuadratic(x; a = {0}, b = {1})", a, b);
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
            return String.Format(formatProvider, "UQuadratic(x; a = {0}, b = {1})", a, b);
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
            return String.Format("UQuadratic(x; a = {0}, b = {1})",
                a.ToString(format, formatProvider),
                b.ToString(format, formatProvider));
        }

    }
}
