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
    ///   Trapezoidal distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    /// Trapezoidal distributions have been advocated in risk analysis problems by Pouliquen
    /// (1970) and more recently by Powell and Wilson (1997). They have also found application
    /// as membership functions in fuzzy set theory (see, e.g. Chen and Hwang (1992)).</para>
    /// 
    /// <para>        
    ///      ...trapezoidal distributions
    /// have been used in the screening and detection of cancer (see, e.g. Flehinger and Kimmel,
    /// (1987) and Brown (1999)).
    /// Trapezoidal distributions seem to be appropriate for modeling the duration and the
    /// form of a phenomenon which may be represented by three stages. The first stage can beviewed as a growth-stage, the second corresponds to a relative stability and the third
    /// represents a decline (decay). These distributions however are restricted since the growth
    /// and decay (in the first and third stages) are limited in the trapezoidal case to linear forms
    /// and the second stage represents complete stability rather than a possible mild incline or
    /// decline. </para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://www.seas.gwu.edu/~dorpjr/Publications/JournalPapers/Metrika2003VanDorp.pdf">
    ///       J. René van Dorp, Samuel Kotz, Trapezoidal distribution. Available on: 
    ///       http://www.seas.gwu.edu/~dorpjr/Publications/JournalPapers/Metrika2003VanDorp.pdf </a></description></item>
    ///   </list></para>     
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example shows how to create and test the main characteristics
    ///   of a Trapezoidal distribution given its parameters: </para>
    ///   
    /// <code>
    /// // Create a new Trapezoidal distribution 
    ///     double x = 0.75d;
    /// 
    ///     double a = 0;
    ///     double b = (1.0d/3.0d);
    ///     double c = (2.0d/3.0d);
    ///     double d = 1.0d;
    ///     double n1 = 2.0d;
    ///     double n3 = 2.0d;
    ///     double alpha = 1.0d;
    /// 
    ///     var trapDist = new TrapezoidalDistribution(a, b, c, d, n1, n3, alpha);
    ///     double mean = trapDist.Mean; //0.62499999999999989
    ///     double variance = trapDist.Variance; //0.37103174603174593
    ///     double pdf = trapDist.ProbabilityDensityFunction(x); //1.1249999999999998
    ///     double cdf = trapDist.DistributionFunction(x); //1.28125
    ///     string tostr = trapDist.ToString();//"Trapezoidal(x; a=0, b=0.333333333333333, c=0.666666666666667, d=1, n1=2, n3=2, α = 1)"
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class TrapezoidalDistribution : UnivariateContinuousDistribution
    {
        //parameters
        double a; //left bottom boundary
        double b; //left top boundary
        double c; //right top boundary
        double d; //right bottom boundary
        double n1; //growth rate
        double n3; //decay rate
        double alpha; //boundary ratio

        public TrapezoidalDistribution(
                                       [Real] double a, //left bottom boundary
                                       [Real] double b, //left top boundary
                                       [Real] double c, //right top boundary
                                       [Real] double d, //right bottom boundary
                                       [Positive] double n1, //growth rate
                                       [Positive] double n3, //decay rate
                                       [Positive] double alpha //boundary ratio
                                       )
        {

            //boundary validation
            if (!(a < b && b < c && c < d)) {
                throw new ArgumentOutOfRangeException(string.Format("boundary parameter inequality must be valid, a < b < c < d: (a={0}, b={1}, c={2}, d={3}", a, b, c, d));
            }

            //mixing/ratio validation
            if( !(n1 > 0  && n3 > 0 && alpha > 0)){
                throw new ArgumentOutOfRangeException(string.Format("mixing and ratio parameters must be positive, n1 > 1, n3, > 0, alpha > 0: (n1={0}, n3={1}, alpha={2}, d={3}", n1, n3, alpha));            
            }


            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.n1 = n1;
            this.n3 = n3;
            this.alpha = alpha;
        }

        public override double Mean
        {
            get 
            { 
                double expectationX1;
                double expectationX2;
                double expectationX3;
 
                expectationX1 = ( a + n1 * b ) / ( n1 + 1.0d );
                expectationX2 = ( (-2.0d / 3.0d ) * ( alpha - 1.0d ) * ( Math.Pow( c, 3.0d )  - Math.Pow( b, 3.0d )) + ( alpha * c - b ) * ( Math.Pow( c, 2.0d ) - Math.Pow( b, 2.0d ) ) ) /
                                (  Math.Pow( c - b, 2.0d ) * ( alpha + 1.0d ) );
                expectationX3 = ( n3 * c + d ) / ( n3 + 1.0d ); 

                return  (
                            ( 2.0d * alpha * (b - a) * n3 * expectationX1 ) +
                            ( n1 * n3 * ( expectationX2 ) ) +
                            ( 2.0d * ( d - c ) * n1 * expectationX3 )
                        ) / 
                        (
                            ( 2.0d * alpha * ( b - a ) * n3 ) +
                            ( ( alpha + 1.0d ) * ( c - b ) * n1 * n3 ) +
                            ( 2.0d * ( d - c ) * n1 )
                        );

            }
        }

        public override double Variance
        {
            get
            {
                double expectationX1_2;
                double expectationX2_2;
                double expectationX3_2;

                expectationX1_2 =   ( 2.0d * a * a  + 2.0d * n1 * a * b + n1 * ( n1 + 1.0d ) * b * b ) /
                                    ( ( n1 + 2.0d ) + ( n1 + 1.0d ) );
                expectationX2_2 =   ( ( -1.0d  / 2.0d ) * ( alpha - 1.0d ) * ( Math.Pow( c, 4.0d) - Math.Pow( b, 4.0d ) ) + ( 2.0d / 3.0d ) * ( alpha * c - b ) * ( Math.Pow(c, 3.0d) - Math.Pow( b, 3.0d ) ) ) /
                                    ( Math.Pow( c - b, 2.0d ) * ( alpha + 1.0d ) );
                expectationX3_2 =   ( 2.0d * d * d + 2.0d * n3 * c * d + n3 * ( n3 + 1.0d ) * c * c ) / 
                                    ( ( n3 + 2.0d ) * ( n3 + 1.0d) );

                return  (
                          (
                            ( 2.0d * alpha * ( b - a ) * n3 ) /
                            ( 2.0d * alpha * ( b - a ) * n3 + ( alpha + 1.0d ) * ( c - b ) * n1 * n3 + 2.0d * ( d - c ) * n1)
                          ) * expectationX1_2 +
                          (
                            ( n1 * n3 ) /
                            ( 2.0d * alpha * ( b - a ) * n3 + ( alpha + 1.0d ) * ( c - b ) * n1 * n3 + 2.0d * ( d - c ) * n1)
                          ) * expectationX2_2 +
                          (
                            ( 2.0d * ( d - c ) * n1 ) /
                            ( 2.0d * alpha * ( b - a ) * n3 + ( alpha + 1.0d ) * ( c - b ) * n1 * n3 + 2.0d * ( d - c ) * n1)
                          ) * expectationX3_2 
                        );
            }
        }

        public override double Entropy
        {
            get { return double.NaN; }
        }

        public override DoubleRange Support
        {
            get { return new DoubleRange( double.MinValue, double.MaxValue ); }
        }

        public override double DistributionFunction(double x)
        {
            if (x < a)
            {
                return 0.0d;
            }
            else if (x < b)
            {
                return (
                            ( 2.0d * alpha * ( b - a ) * n3 ) / 
                            ( 
                                ( 2.0d * alpha * ( b - a ) * n3 ) +
                                ( ( alpha + 1.0d ) * ( c - b ) * n1 * n3 ) +
                                ( 2.0d * ( d - c ) * n1)
                            )
                        ) * Math.Pow( ( x - a ) / ( b - a ), n1 );
            }
            else if (x < c)
            {
                return  (
                            ( 2.0d * alpha * ( b - a ) * n3 ) +
                            ( 2.0d * ( x - b ) * n1 * n3 *
                                ( 1.0d + 
                                    ( (alpha - 1.0d) * ( 2.0d * c - b - x) ) /
                                    ( 2.0d * ( c - b ) ) 
                                )
                            ) 
                        ) /
                        (
                            ( 2.0d * alpha * ( b - a ) * n3 ) +
                            ( ( alpha + 1.0d ) * ( c - b ) * n1 * n3 ) +
                            ( 2.0d * ( c - d ) * n1 )
                        );
            }
            else if (x < d)
            {
                return  ( 1.0d -
                            (
                                ( 2.0d * ( c - d ) * n1 ) /
                                (
                                    ( 2.0d * alpha * ( b - a ) * n3 ) +
                                    ( ( alpha + 1.0d ) * ( c - b ) * n1 * n3 ) +
                                    ( 2.0d * ( c - d ) * n1 )
                                ) *
                                Math.Pow(  ( d - x ) / ( d - c ), n3 )                                    
                            )
                        );
            }

            return 1.0d;
        }

        public override double ProbabilityDensityFunction(double x)
        {
            if ( x < a ) {
                return 0.0d;
            }
            else if (x < b) {
                return DensityConstant() * alpha * Math.Pow((x - a) / (b - a), n1 - 1.0d);
            }
            else if (x < c) {
                return DensityConstant() * ( ( ( alpha - 1.0d ) * ( c - x ) / ( c - b ) ) + 1.0d );
            }
            else if (x < d) {
                return DensityConstant() * Math.Pow((d - x) / (d - c), n3 - 1.0d);            
            }

            return 0.0d;
        }

        private double DensityConstant() { 
            return  ( 2.0d * n1 * n3 ) /
                    (   2.0d * alpha * ( b - a ) * n3 +
                        ( alpha + 1 ) * ( c - b ) * n1 * n3 +
                        2.0d * ( d - c ) * n1
                    );
        }

        public override object Clone()
        {
            return new TrapezoidalDistribution(
                                            this.a,
                                            this.b,
                                            this.c,
                                            this.d,
                                            this.n1,
                                            this.n3,
                                            this.alpha);
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
            return String.Format("Trapezoidal(x; a={0}, b={1}, c={2}, d={3}, n1={4}, n3={5}, α = {6})",
                a, b, c, d, n1, n3, alpha);
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
            return String.Format(formatProvider, "Trapezoidal(x; a={0}, b={1}, c={2}, d={3}, n1={4}, n3={5}, α = {6})",
                a, b, c, d, n1, n3, alpha);
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
            return String.Format("Trapezoidal(x; a={0}, b={1}, c={2}, d={3}, n1={4}, n3={5}, α = {6})",
                a.ToString(format, formatProvider),
                b.ToString(format, formatProvider),
                c.ToString(format, formatProvider),
                d.ToString(format, formatProvider),
                n1.ToString(format, formatProvider),
                n3.ToString(format, formatProvider),
                alpha.ToString(format, formatProvider));
        }
    }
}
