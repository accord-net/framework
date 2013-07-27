// Accord Math Library
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
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

// Contains functions from the Cephes Math Library Release 2.8:
// June, 2000 Copyright 1984, 1987, 1988, 2000 by Stephen L. Moshier
//
// Original license is listed below:
//
//   Some software in this archive may be from the book _Methods and
// Programs for Mathematical Functions_ (Prentice-Hall or Simon & Schuster
// International, 1989) or from the Cephes Mathematical Library, a
// commercial product. In either event, it is copyrighted by the author.
// What you see here may be used freely but it comes with no support or
// guarantee.
//
//   The two known misprints in the book are repaired here in the
// source listings for the gamma function and the incomplete beta
// integral.
//
//
//   Stephen L. Moshier
//   moshier@na-net.ornl.gov
//

namespace Accord.Math
{
    using System;

    /// <summary>
    ///   Normal distribution functions.
    /// </summary>
    ///  
    /// <remarks>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Cephes Math Library, http://www.netlib.org/cephes/ </description></item>
    ///     <item><description>
    ///       George Marsaglia, Evaluating the Normal Distribution, 2004.
    ///       Available in: http://www.jstatsoft.org/v11/a05/paper </description></item>
    ///   </list>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example shows the normal usages for the Normal functions:
    /// </para>
    /// 
    /// <code>
    ///   // Compute standard precision functions
    ///   double phi  = Normal.Function(0.42);     //  0.66275727315175048
    ///   double phic = Normal.Complemented(0.42); //  0.33724272684824952
    ///   double inv  = Normal.Inverse(0.42);      // -0.20189347914185085
    ///   
    ///   // Compute at the limits
    ///   double phi  = Normal.Function(16.6);     //  1.0
    ///   double phic = Normal.Complemented(16.6); //  3.4845465199504055E-62
    /// </code>
    /// 
    /// </example>
    /// 
    public static class Normal
    {

        /// <summary>
        ///   Normal cumulative distribution function.
        /// </summary>
        /// <returns>
        ///   The area under the Gaussian p.d.f. integrated
        ///   from minus infinity to the given value.
        /// </returns>
        /// 
        public static double Function(double value)
        {
            return 0.5 * Special.Erfc(-value / Constants.Sqrt2);
        }

        /// <summary>
        ///   Complemented cumulative distribution function.
        /// </summary>
        /// 
        public static double Complemented(double value)
        {
            return 0.5 * Special.Erfc(value / Constants.Sqrt2);
        }

        /// <summary>
        ///    Normal (Gaussian) inverse cumulative distribution function.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        ///    For small arguments <c>0 &lt; y &lt; exp(-2)</c>, the program computes <c>z =
        ///    sqrt( -2.0 * log(y) )</c>;  then the approximation is <c>x = z - log(z)/z  - 
        ///    (1/z) P(1/z) / Q(1/z)</c>.</para>
        /// <para>
        ///    There are two rational functions P/Q, one for <c>0 &lt; y &lt; exp(-32)</c> and
        ///    the other for <c>y</c> up to <c>exp(-2)</c>. For larger arguments, <c>w = y - 0.5</c>,
        ///    and  <c>x/sqrt(2pi) = w + w^3 * R(w^2)/S(w^2))</c>.</para>
        /// </remarks>
        /// 
        /// <returns>
        ///    Returns the value, <c>x</c>, for which the area under the Normal (Gaussian)
        ///    probability density function (integrated from minus infinity to <c>x</c>) is
        ///    equal to the argument <c>y</c> (assumes mean is zero, variance is one).
        /// </returns>
        /// 
        public static double Inverse(double y0)
        {
            if (y0 <= 0.0)
            {
                if (y0 == 0) return Double.NegativeInfinity;
                throw new ArgumentOutOfRangeException("y0");
            }
            if (y0 >= 1.0)
            {
                if (y0 == 1) return Double.PositiveInfinity;
                throw new ArgumentOutOfRangeException("y0");
            }


            double s2pi = Math.Sqrt(2.0 * Math.PI);
            int code = 1;
            double y = y0;
            double x;

            double[] P0 =
            {
                -59.963350101410789,
                98.001075418599967,
                -56.676285746907027,
                13.931260938727968,
                -1.2391658386738125
            };

            double[] Q0 = 
            {
                1.9544885833814176,
                4.6762791289888153, 
                86.360242139089053,
                -225.46268785411937,
                200.26021238006066,
                -82.037225616833339,
                15.90562251262117,
                -1.1833162112133
            };

            double[] P1 = 
            {
                4.0554489230596245,
                31.525109459989388,
                57.162819224642128,
                44.080507389320083,
                14.684956192885803,
                2.1866330685079025,
                -0.14025607917135449,
                -0.035042462682784818,
                -0.00085745678515468545
            };

            double[] Q1 = 
            {
                15.779988325646675,
                45.390763512887922, 
                41.317203825467203, 
                15.04253856929075, 
                2.5046494620830941, 
                -0.14218292285478779, 
                -0.038080640769157827,
                -0.00093325948089545744
            };

            double[] P2 = 
            {
                3.2377489177694603, 
                6.9152288906898418, 
                3.9388102529247444,
                1.3330346081580755, 
                0.20148538954917908, 
                0.012371663481782003,
                0.00030158155350823543, 
                2.6580697468673755E-06,
                6.2397453918498331E-09
            };

            double[] Q2 = 
            {
                6.02427039364742,
                3.6798356385616087,
                1.3770209948908132,
                0.21623699359449663, 
                0.013420400608854318, 
                0.00032801446468212774,
                2.8924786474538068E-06,
                6.7901940800998127E-09
            };

            if (y > 0.8646647167633873)
            {
                y = 1.0 - y;
                code = 0;
            }

            if (y > 0.1353352832366127)
            {
                y -= 0.5;
                double y2 = y * y;
                x = y + y * ((y2 * Special.Polevl(y2, P0, 4)) / Special.P1evl(y2, Q0, 8));
                x *= s2pi;
                return x;
            }

            x = Math.Sqrt(-2.0 * Math.Log(y));
            double x0 = x - Math.Log(x) / x;
            double z = 1.0 / x;
            double x1;

            if (x < 8.0)
            {
                x1 = (z * Special.Polevl(z, P1, 8)) / Special.P1evl(z, Q1, 8);
            }
            else
            {
                x1 = (z * Special.Polevl(z, P2, 8)) / Special.P1evl(z, Q2, 8);
            }

            x = x0 - x1;

            if (code != 0)
                x = -x;

            return x;
        }

        /// <summary>
        ///   High-accuracy Normal cumulative distribution function.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        ///   The following formula provide probabilites with an absolute error
        ///   less than 8e-16.</para>
        /// <para>
        ///   References:
        ///    - George Marsaglia, Evaluating the Normal Distribution, 2004.
        ///      Available in: http://www.jstatsoft.org/v11/a05/paper </para>  
        /// </remarks>
        /// 
        public static double HighAccuracyFunction(double x)
        {
            double sum = x;
            double term = 0;

            double nextTerm = x;
            double pwr = x * x;
            double i = 1;

            // Iterate until adding next terms doesn't produce
            // any change within the current numerical accuracy.

            while (sum != term)
            {
                term = sum;

                // Next term
                nextTerm *= pwr / (i += 2);

                sum += nextTerm;
            }

            return 0.5 + sum * Math.Exp(-0.5 * pwr - 0.5 * Constants.Log2PI);
        }

        /// <summary>
        ///   High-accuracy Complementary normal distribution function.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        ///   This function uses 9 tabled values to provide tail values of the
        ///   normal distribution, also known as complementary Phi, with an
        ///   absolute error of 1e-14 ~ 1e-16.
        /// </para>
        ///   References:
        ///    - George Marsaglia, Evaluating the Normal Distribution, 2004.
        ///      Available in: http://www.jstatsoft.org/v11/a05/paper
        /// </remarks>
        /// 
        /// <returns>
        ///   The area under the Gaussian p.d.f. integrated
        ///   from the given value to positive infinity.
        /// </returns>
        /// 
        public static double HighAccuracyComplemented(double x)
        {
            double[] R = 
            {
                1.25331413731550025,   0.421369229288054473,  0.236652382913560671,
                0.162377660896867462,  0.123131963257932296,  0.0990285964717319214,
                0.0827662865013691773, 0.0710695805388521071, 0.0622586659950261958
            };

            int j = (int)(0.5 * (Math.Abs(x) + 1));

            if (j >= R.Length)
            {
                return x > 0 ? 0 : 1;
            }

            double a = R[j];
            double z = 2 * j;
            double b = a * z - 1.0;

            double h = Math.Abs(x) - z;
            double q = h * h;
            double pwr = 1;

            double sum = a + h * b;
            double term = a;


            for (int i = 2; sum != term; i += 2)
            {
                term = sum;

                a = (a + z * b) / (i);
                b = (b + z * a) / (i + 1);
                pwr *= q;

                sum = term + pwr * (a + h * b);
            }

            sum *= Math.Exp(-0.5 * x * x - 0.91893853320467274178);

            return (x >= 0) ? sum : (1.0 - sum);
        }

    }
}
