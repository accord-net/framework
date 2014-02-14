// Accord Math Library
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
    ///   Gamma Γ(x) functions.
    /// </summary>
    ///  
    /// <remarks>
    /// <para>
    ///   In mathematics, the gamma function (represented by the capital Greek 
    ///   letter Γ) is an extension of the factorial function, with its argument
    ///   shifted down by 1, to real and complex numbers. That is, if <c>n</c> is
    ///   a positive integer:</para>
    /// <code>
    ///   Γ(n) = (n-1)!</code>
    /// <para>
    ///   The gamma function is defined for all complex numbers except the negative
    ///   integers and zero. For complex numbers with a positive real part, it is 
    ///   defined via an improper integral that converges:</para>
    /// <code>
    ///          ∞
    ///   Γ(z) = ∫  t^(z-1)e^(-t) dt
    ///          0
    /// </code>     
    /// <para>
    ///   This integral function is extended by analytic continuation to all 
    ///   complex numbers except the non-positive integers (where the function 
    ///   has simple poles), yielding the meromorphic function we call the gamma
    ///   function.</para>
    /// <para>
    ///   The gamma function is a component in various probability-distribution 
    ///   functions, and as such it is applicable in the fields of probability 
    ///   and statistics, as well as combinatorics.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia contributors, "Gamma function,". Wikipedia, The Free 
    ///       Encyclopedia. Available at: http://en.wikipedia.org/wiki/Gamma_function 
    ///       </description></item>
    ///     <item><description>
    ///       Cephes Math Library, http://www.netlib.org/cephes/ </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///   double x = 0.17;
    ///   
    ///   // Compute main Gamma function and variants
    ///   double gamma = Gamma.Function(x); // 5.4511741801042106
    ///   double gammap = Gamma.Function(x, p: 2); // -39.473585841300675
    ///   double log = Gamma.Log(x);        // 1.6958310313607003
    ///   double logp = Gamma.Log(x, p: 2); // 3.6756317353404273
    ///   double stir = Gamma.Stirling(x);  // 24.040352622960743
    ///   double psi = Gamma.Digamma(x);    // -6.2100942259248626
    ///   double tri = Gamma.Trigamma(x);   // 35.915302055854525
    ///
    ///   double a = 4.2;
    ///   
    ///   // Compute the incomplete regularized Gamma functions P and Q:
    ///   double lower = Gamma.LowerIncomplete(a, x); // 0.000015685073063633753
    ///   double upper = Gamma.UpperIncomplete(a, x); // 0.9999843149269364
    /// </code>
    /// </example>
    /// 
    public static class Gamma
    {

        /// <summary>Maximum gamma on the machine.</summary>
        public const double GammaMax = 171.624376956302725;

        /// <summary>
        ///   Gamma function of the specified value.
        /// </summary>
        /// 
        public static double Function(double x)
        {
            double[] P =
            {
                1.60119522476751861407E-4,
                1.19135147006586384913E-3,
                1.04213797561761569935E-2,
                4.76367800457137231464E-2,
                2.07448227648435975150E-1,
                4.94214826801497100753E-1,
                9.99999999999999996796E-1
            };
            double[] Q =
            {
               -2.31581873324120129819E-5,
                5.39605580493303397842E-4,
               -4.45641913851797240494E-3,
                1.18139785222060435552E-2,
                3.58236398605498653373E-2,
               -2.34591795718243348568E-1,
                7.14304917030273074085E-2,
                1.00000000000000000320E0
            };

            double p, z;

            double q = System.Math.Abs(x);

            if (q > 33.0)
            {
                if (x < 0.0)
                {
                    p = System.Math.Floor(q);

                    if (p == q)
                        throw new OverflowException();

                    z = q - p;
                    if (z > 0.5)
                    {
                        p += 1.0;
                        z = q - p;
                    }
                    z = q * System.Math.Sin(System.Math.PI * z);

                    if (z == 0.0)
                        throw new OverflowException();

                    z = System.Math.Abs(z);
                    z = System.Math.PI / (z * Stirling(q));

                    return -z;
                }
                else
                {
                    return Stirling(x);
                }
            }

            z = 1.0;
            while (x >= 3.0)
            {
                x -= 1.0;
                z *= x;
            }

            while (x < 0.0)
            {
                if (x == 0.0)
                {
                    throw new ArithmeticException();
                }
                else if (x > -1.0E-9)
                {
                    return (z / ((1.0 + 0.5772156649015329 * x) * x));
                }
                z /= x;
                x += 1.0;
            }

            while (x < 2.0)
            {
                if (x == 0.0)
                {
                    throw new ArithmeticException();
                }
                else if (x < 1.0E-9)
                {
                    return (z / ((1.0 + 0.5772156649015329 * x) * x));
                }

                z /= x;
                x += 1.0;
            }

            if ((x == 2.0) || (x == 3.0)) return z;

            x -= 2.0;
            p = Special.Polevl(x, P, 6);
            q = Special.Polevl(x, Q, 7);
            return z * p / q;

        }

        /// <summary>
        ///   Multivariate Gamma function
        /// </summary>
        /// 
        public static double Multivariate(double x, int p)
        {
            if (p < 1) 
                throw new ArgumentOutOfRangeException("p",
                    "Parameter p must be higher than 1.");

            if (p == 1) 
                return Function(x);


            double prod = Math.Pow(Math.PI, (1 / 4.0) * p * (p - 1));

            for (int i = 0; i < p; i++)
                prod *= Function(x - 0.5 * i);

            return prod;
        }

        /// <summary>
        ///   Digamma function.
        /// </summary>
        /// 
        public static double Digamma(double x)
        {
            double s = 0;
            double w = 0;
            double y = 0;
            double z = 0;
            double nz = 0;

            bool negative = false;

            if (x <= 0.0)
            {
                negative = true;
                double q = x;
                double p = (int)System.Math.Floor(q);

                if (p == q)
                    throw new OverflowException("Function computation resulted in arithmetic overflow.");

                nz = q - p;

                if (nz != 0.5)
                {
                    if (nz > 0.5)
                    {
                        p = p + 1.0;
                        nz = q - p;
                    }
                    nz = Math.PI / Math.Tan(System.Math.PI * nz);
                }
                else
                {
                    nz = 0.0;
                }

                x = 1.0 - x;
            }

            if (x <= 10.0 & x == Math.Floor(x))
            {
                y = 0.0;
                int n = (int)Math.Floor(x);
                for (int i = 1; i <= n - 1; i++)
                {
                    w = i;
                    y = y + 1.0 / w;
                }
                y = y - 0.57721566490153286061;
            }
            else
            {
                s = x;
                w = 0.0;

                while (s < 10.0)
                {
                    w = w + 1.0 / s;
                    s = s + 1.0;
                }

                if (s < 1.0E17)
                {
                    z = 1.0 / (s * s);

                    double polv = 8.33333333333333333333E-2;
                    polv = polv * z - 2.10927960927960927961E-2;
                    polv = polv * z + 7.57575757575757575758E-3;
                    polv = polv * z - 4.16666666666666666667E-3;
                    polv = polv * z + 3.96825396825396825397E-3;
                    polv = polv * z - 8.33333333333333333333E-3;
                    polv = polv * z + 8.33333333333333333333E-2;
                    y = z * polv;
                }
                else
                {
                    y = 0.0;
                }
                y = Math.Log(s) - 0.5 / s - y - w;
            }

            if (negative == true)
            {
                y = y - nz;
            }

            return y;
        }

        /// <summary>
        ///   Trigamma function.
        /// </summary>
        /// 
        /// <remarks>
        ///   This code has been adapted from the FORTRAN77 and subsequent
        ///   C code by B. E. Schneider and John Burkardt. The code had been
        ///   made public under the GNU LGPL license.
        /// </remarks>
        /// 
        public static double Trigamma(double x)
        {
            double a = 0.0001;
            double b = 5.0;
            double b2 = 0.1666666667;
            double b4 = -0.03333333333;
            double b6 = 0.02380952381;
            double b8 = -0.03333333333;
            double value;
            double y;
            double z;

            // Check the input.
            if (x <= 0.0)
            {
                throw new ArgumentException("The input parameter x must be positive.", "x");
            }

            z = x;

            // Use small value approximation if X <= A.
            if (x <= a)
            {
                value = 1.0 / x / x;
                return value;
            }

            // Increase argument to ( X + I ) >= B.
            value = 0.0;

            while (z < b)
            {
                value = value + 1.0 / z / z;
                z = z + 1.0;
            }

            // Apply asymptotic formula if argument is B or greater.
            y = 1.0 / z / z;

            value = value + 0.5 *
                y + (1.0
              + y * (b2
              + y * (b4
              + y * (b6
              + y * b8)))) / z;

            return value;
        }

        /// <summary>
        ///   Gamma function as computed by Stirling's formula.
        /// </summary>
        /// 
        public static double Stirling(double x)
        {
            double[] STIR =
            {
                 7.87311395793093628397E-4,
                -2.29549961613378126380E-4,
                -2.68132617805781232825E-3,
                 3.47222221605458667310E-3,
                 8.33333333333482257126E-2,
            };

            double MAXSTIR = 143.01608;

            double w = 1.0 / x;
            double y = Math.Exp(x);

            w = 1.0 + w * Special.Polevl(w, STIR, 4);

            if (x > MAXSTIR)
            {
                double v = Math.Pow(x, 0.5 * x - 0.25);
                y = v * (v / y);
            }
            else
            {
                y = System.Math.Pow(x, x - 0.5) / y;
            }

            y = Constants.Sqrt2PI * y * w;
            return y;
        }

        /// <summary>
        ///   Upper incomplete regularized Gamma function Q
        ///   (a.k.a the incomplete complemented Gamma function)
        /// </summary>
        /// 
        public static double UpperIncomplete(double a, double x)
        {
            const double big = 4.503599627370496e15;
            const double biginv = 2.22044604925031308085e-16;
            double ans, ax, c, yc, r, t, y, z;
            double pk, pkm1, pkm2, qk, qkm1, qkm2;

            if (x <= 0 || a <= 0) return 1.0;

            if (x < 1.0 || x < a) return 1.0 - LowerIncomplete(a, x);

            ax = a * Math.Log(x) - x - Log(a);
            if (ax < -Constants.LogMax) return 0.0;

            ax = Math.Exp(ax);

            // continued fraction
            y = 1.0 - a;
            z = x + y + 1.0;
            c = 0.0;
            pkm2 = 1.0;
            qkm2 = x;
            pkm1 = x + 1.0;
            qkm1 = z * x;
            ans = pkm1 / qkm1;

            do
            {
                c += 1.0;
                y += 1.0;
                z += 2.0;
                yc = y * c;
                pk = pkm1 * z - pkm2 * yc;
                qk = qkm1 * z - qkm2 * yc;
                if (qk != 0)
                {
                    r = pk / qk;
                    t = Math.Abs((ans - r) / r);
                    ans = r;
                }
                else
                    t = 1.0;

                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                if (Math.Abs(pk) > big)
                {
                    pkm2 *= biginv;
                    pkm1 *= biginv;
                    qkm2 *= biginv;
                    qkm1 *= biginv;
                }
            } while (t > Constants.DoubleEpsilon);

            return ans * ax;
        }

        /// <summary>
        ///   Lower incomplete regularized gamma function P
        ///   (a.k.a. the incomplete Gamma function).
        /// </summary>
        /// 
        public static double LowerIncomplete(double a, double x)
        {
            double ans, ax, c, r;

            if (x <= 0 || a <= 0) return 0.0;

            if (x > 1.0 && x > a) return 1.0 - UpperIncomplete(a, x);

            ax = a * Math.Log(x) - x - Log(a);
            if (ax < -Constants.LogMax) return (0.0);

            ax = Math.Exp(ax);

            r = a;
            c = 1.0;
            ans = 1.0;

            do
            {
                r += 1.0;
                c *= x / r;
                ans += c;
            } while (c / ans > Constants.DoubleEpsilon);

            return (ans * ax / a);

        }

        /// <summary>
        ///   Natural logarithm of the gamma function.
        /// </summary>
        /// 
        public static double Log(double x)
        {
            double p, q, w, z;

            double[] A =
            {
                 8.11614167470508450300E-4,
                -5.95061904284301438324E-4,
                 7.93650340457716943945E-4,
                -2.77777777730099687205E-3,
                 8.33333333333331927722E-2
            };

            double[] B =
            {
                -1.37825152569120859100E3,
                -3.88016315134637840924E4,
                -3.31612992738871184744E5,
                -1.16237097492762307383E6,
                -1.72173700820839662146E6,
                -8.53555664245765465627E5
            };

            double[] C =
            {
                -3.51815701436523470549E2,
                -1.70642106651881159223E4,
                -2.20528590553854454839E5,
                -1.13933444367982507207E6,
                -2.53252307177582951285E6,
                -2.01889141433532773231E6
            };

            if (x < -34.0)
            {
                q = -x;
                w = Log(q);
                p = Math.Floor(q);

                if (p == q)
                    throw new OverflowException();

                z = q - p;
                if (z > 0.5)
                {
                    p += 1.0;
                    z = p - q;
                }
                z = q * Math.Sin(System.Math.PI * z);

                if (z == 0.0)
                    throw new OverflowException();

                z = Constants.LogPI - Math.Log(z) - w;
                return z;
            }

            if (x < 13.0)
            {
                z = 1.0;
                while (x >= 3.0)
                {
                    x -= 1.0;
                    z *= x;
                }
                while (x < 2.0)
                {
                    if (x == 0.0)
                        throw new OverflowException();

                    z /= x;
                    x += 1.0;
                }
                if (z < 0.0) z = -z;
                if (x == 2.0) return System.Math.Log(z);
                x -= 2.0;
                p = x * Special.Polevl(x, B, 5) / Special.P1evl(x, C, 6);
                return (Math.Log(z) + p);
            }

            if (x > 2.556348e305)
                throw new OverflowException();

            q = (x - 0.5) * Math.Log(x) - x + 0.91893853320467274178;
            if (x > 1.0e8) return (q);

            p = 1.0 / (x * x);
            if (x >= 1000.0)
            {
                q += ((7.9365079365079365079365e-4 * p
                    - 2.7777777777777777777778e-3) * p
                    + 0.0833333333333333333333) / x;
            }
            else
            {
                q += Special.Polevl(p, A, 4) / x;
            }

            return q;
        }

        /// <summary>
        ///   Natural logarithm of the multivariate Gamma function.
        /// </summary>
        /// 
        public static double Log(double x, int p)
        {
            if (p < 1) throw new ArgumentOutOfRangeException("p", "Parameter p must be higher than 1.");
            if (p == 1) return Log(x);

            double sum = Constants.LogPI / p;
            for (int i = 0; i < p; i++)
                sum += Log(x - 0.5 * i);

            return sum;
        }

        /// <summary>
        ///   Inverse of the <see cref="UpperIncomplete">complemented 
        ///   incomplete Gamma integral (UpperIncomplete)</see>.
        /// </summary>
        /// 
        public static double Inverse(double a, double y)
        {
            double x0, x1, x, yl, yh, yy, d, lgm, dithresh;
            int i, dir;

            // bound the solution
            x0 = Double.MaxValue;
            yl = 0;
            x1 = 0;
            yh = 1.0;
            dithresh = 5.0 * Constants.DoubleEpsilon;

            // approximation to inverse function
            d = 1.0 / (9.0 * a);
            yy = (1.0 - d - Normal.Inverse(y) * Math.Sqrt(d));
            x = a * yy * yy * yy;

            lgm = Gamma.Log(a);

            for (i = 0; i < 10; i++)
            {
                if (x > x0 || x < x1)
                    goto ihalve;
                yy = Gamma.UpperIncomplete(a, x);
                if (yy < yl || yy > yh)
                    goto ihalve;
                if (yy < y)
                {
                    x0 = x;
                    yl = yy;
                }
                else
                {
                    x1 = x;
                    yh = yy;
                }

                // compute the derivative of the function at this point
                d = (a - 1.0) * Math.Log(x) - x - lgm;
                if (d < -Constants.LogMax)
                    goto ihalve;
                d = -Math.Exp(d);

                // compute the step to the next approximation of x
                d = (yy - y) / d;
                if (Math.Abs(d / x) < Constants.DoubleEpsilon)
                    return x;
                x = x - d;
            }

        // Resort to interval halving if Newton iteration did not converge. 
        ihalve:

            d = 0.0625;
            if (x0 == Double.MaxValue)
            {
                if (x <= 0.0)
                    x = 1.0;
                while (x0 == Double.MaxValue)
                {
                    x = (1.0 + d) * x;
                    yy = Gamma.UpperIncomplete(a, x);
                    if (yy < y)
                    {
                        x0 = x;
                        yl = yy;
                        break;
                    }
                    d = d + d;
                }
            }
            d = 0.5;
            dir = 0;

            for (i = 0; i < 400; i++)
            {
                x = x1 + d * (x0 - x1);
                yy = Gamma.UpperIncomplete(a, x);
                lgm = (x0 - x1) / (x1 + x0);
                if (Math.Abs(lgm) < dithresh)
                    break;
                lgm = (yy - y) / y;
                if (Math.Abs(lgm) < dithresh)
                    break;
                if (x <= 0.0)
                    break;
                if (yy >= y)
                {
                    x1 = x;
                    yh = yy;
                    if (dir < 0)
                    {
                        dir = 0;
                        d = 0.5;
                    }
                    else if (dir > 1)
                        d = 0.5 * d + 0.5;
                    else
                        d = (y - yl) / (yh - yl);
                    dir += 1;
                }
                else
                {
                    x0 = x;
                    yl = yy;
                    if (dir > 0)
                    {
                        dir = 0;
                        d = 0.5;
                    }
                    else if (dir < -1)
                        d = 0.5 * d;
                    else
                        d = (y - yl) / (yh - yl);
                    dir -= 1;
                }
            }
            if (x == 0.0)
                throw new ArithmeticException();

            return x;
        }
    }
}
