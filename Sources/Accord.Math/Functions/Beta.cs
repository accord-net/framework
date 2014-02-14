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
    ///   Beta functions.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class offers implementations for the many Beta functions,
    ///   such as the <see cref="Function">Beta function itself</see>, 
    ///   <see cref="Log">its logarithm</see>, the <see cref="Incomplete"/>
    ///   incomplete regularized functions and others</para>
    ///   
    /// <para>
    ///   The beta function was studied by Euler and Legendre and was given
    ///   its name by Jacques Binet; its symbol Β is a Greek capital β rather
    ///   than the similar Latin capital B.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Cephes Math Library, http://www.netlib.org/cephes/ </description></item>
    ///     <item><description>
    ///       Wikipedia contributors, "Beta function,". Wikipedia, The Free 
    ///       Encyclopedia. Available at: http://en.wikipedia.org/wiki/Beta_function 
    ///       </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///   Beta.Function(4, 0.42);       // 1.2155480852832423
    ///   Beta.Log(4, 15.2);            // -9.46087817876467
    ///   Beta.Incbcf(4, 2, 4.2);       // -0.23046874999999992
    ///   Beta.Incbd(4, 2, 4.2);        // 0.7375
    ///   Beta.PowerSeries(4, 2, 4.2);  // -3671.801280000001
    ///   
    ///   Beta.Incomplete(a: 5, b: 4, x: 0.5);   // 0.36328125
    ///   Beta.IncompleteInverse(0.5, 0.6, 0.1); // 0.019145979066925722
    ///   Beta.Multinomial(0.42, 0.5, 5.2 );     // 0.82641912952987062
    /// </code>
    /// </example>
    /// 
    public static class Beta
    {

        /// <summary>
        ///   Beta function as gamma(a) * gamma(b) / gamma(a+b).
        /// </summary>
        /// 
        /// <example>
        ///   Please see <see cref="Beta"/>
        /// </example>
        /// 
        public static double Function(double a, double b)
        {
            return Math.Exp(Log(a, b));
        }

        /// <summary>
        ///   Natural logarithm of the Beta function.
        /// </summary>
        /// 
        /// <example>
        ///   Please see <see cref="Beta"/>
        /// </example>
        /// 
        public static double Log(double a, double b)
        {
            return Gamma.Log(a) + Gamma.Log(b) - Gamma.Log(a + b);
        }

        /// <summary>
        ///   Incomplete (regularized) Beta function Ix(a, b).
        /// </summary>
        /// 
        /// <example>
        ///   Please see <see cref="Beta"/>
        /// </example>
        /// 
        public static double Incomplete(double a, double b, double x)
        {
            double aa, bb, t, xx, xc, w, y;
            bool flag;

            if (a <= 0.0)
                throw new ArgumentOutOfRangeException("a", "Lower limit must be greater than zero.");
            if (b <= 0.0)
                throw new ArgumentOutOfRangeException("b", "Upper limit must be greater than zero.");

            if ((x <= 0.0) || (x >= 1.0))
            {
                if (x == 0.0) return 0.0;
                if (x == 1.0) return 1.0;
                throw new ArgumentOutOfRangeException("x", "Value must be between 0 and 1.");
            }

            flag = false;
            if ((b * x) <= 1.0 && x <= 0.95)
            {
                t = PowerSeries(a, b, x);
                return t;
            }

            w = 1.0 - x;

            if (x > (a / (a + b)))
            {
                flag = true;
                aa = b;
                bb = a;
                xc = x;
                xx = w;
            }
            else
            {
                aa = a;
                bb = b;
                xc = w;
                xx = x;
            }

            if (flag && (bb * xx) <= 1.0 && xx <= 0.95)
            {
                t = PowerSeries(aa, bb, xx);
                if (t <= Constants.DoubleEpsilon) t = 1.0 - Constants.DoubleEpsilon;
                else t = 1.0 - t;
                return t;
            }

            y = xx * (aa + bb - 2.0) - (aa - 1.0);
            if (y < 0.0)
                w = Incbcf(aa, bb, xx);
            else
                w = Incbd(aa, bb, xx) / xc;


            y = aa * System.Math.Log(xx);
            t = bb * System.Math.Log(xc);
            if ((aa + bb) < Gamma.GammaMax && System.Math.Abs(y) < Constants.LogMax && System.Math.Abs(t) < Constants.LogMax)
            {
                t = System.Math.Pow(xc, bb);
                t *= System.Math.Pow(xx, aa);
                t /= aa;
                t *= w;
                t *= Gamma.Function(aa + bb) / (Gamma.Function(aa) * Gamma.Function(bb));
                if (flag)
                {
                    if (t <= Constants.DoubleEpsilon) t = 1.0 - Constants.DoubleEpsilon;
                    else t = 1.0 - t;
                }
                return t;
            }

            y += t + Gamma.Log(aa + bb) - Gamma.Log(aa) - Gamma.Log(bb);
            y += System.Math.Log(w / aa);
            if (y < Constants.LogMin)
                t = 0.0;
            else
                t = System.Math.Exp(y);

            if (flag)
            {
                if (t <= Constants.DoubleEpsilon) t = 1.0 - Constants.DoubleEpsilon;
                else t = 1.0 - t;
            }
            return t;
        }

        /// <summary>
        ///   Continued fraction expansion #1 for incomplete beta integral.
        /// </summary>
        /// 
        /// <example>
        ///   Please see <see cref="Beta"/>
        /// </example>
        /// 
        public static double Incbcf(double a, double b, double x)
        {
            double xk, pk, pkm1, pkm2, qk, qkm1, qkm2;
            double k1, k2, k3, k4, k5, k6, k7, k8;
            double r, t, ans, thresh;
            int n;
            double big = 4.503599627370496e15;
            double biginv = 2.22044604925031308085e-16;

            k1 = a;
            k2 = a + b;
            k3 = a;
            k4 = a + 1.0;
            k5 = 1.0;
            k6 = b - 1.0;
            k7 = k4;
            k8 = a + 2.0;

            pkm2 = 0.0;
            qkm2 = 1.0;
            pkm1 = 1.0;
            qkm1 = 1.0;
            ans = 1.0;
            r = 1.0;
            n = 0;
            thresh = 3.0 * Constants.DoubleEpsilon;

            do
            {
                xk = -(x * k1 * k2) / (k3 * k4);
                pk = pkm1 + pkm2 * xk;
                qk = qkm1 + qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;

                xk = (x * k5 * k6) / (k7 * k8);
                pk = pkm1 + pkm2 * xk;
                qk = qkm1 + qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;

                if (qk != 0) r = pk / qk;
                if (r != 0)
                {
                    t = System.Math.Abs((ans - r) / r);
                    ans = r;
                }
                else
                    t = 1.0;

                if (t < thresh) return ans;

                k1 += 1.0;
                k2 += 1.0;
                k3 += 2.0;
                k4 += 2.0;
                k5 += 1.0;
                k6 -= 1.0;
                k7 += 2.0;
                k8 += 2.0;

                if ((System.Math.Abs(qk) + System.Math.Abs(pk)) > big)
                {
                    pkm2 *= biginv;
                    pkm1 *= biginv;
                    qkm2 *= biginv;
                    qkm1 *= biginv;
                }
                if ((System.Math.Abs(qk) < biginv) || (System.Math.Abs(pk) < biginv))
                {
                    pkm2 *= big;
                    pkm1 *= big;
                    qkm2 *= big;
                    qkm1 *= big;
                }
            } while (++n < 300);

            return ans;
        }

        /// <summary>
        ///   Continued fraction expansion #2 for incomplete beta integral.
        /// </summary>
        /// 
        /// <example>
        ///   Please see <see cref="Beta"/>
        /// </example>
        /// 
        public static double Incbd(double a, double b, double x)
        {
            double xk, pk, pkm1, pkm2, qk, qkm1, qkm2;
            double k1, k2, k3, k4, k5, k6, k7, k8;
            double r, t, ans, z, thresh;
            int n;
            double big = 4.503599627370496e15;
            double biginv = 2.22044604925031308085e-16;

            k1 = a;
            k2 = b - 1.0;
            k3 = a;
            k4 = a + 1.0;
            k5 = 1.0;
            k6 = a + b;
            k7 = a + 1.0;
            ;
            k8 = a + 2.0;

            pkm2 = 0.0;
            qkm2 = 1.0;
            pkm1 = 1.0;
            qkm1 = 1.0;
            z = x / (1.0 - x);
            ans = 1.0;
            r = 1.0;
            n = 0;
            thresh = 3.0 * Constants.DoubleEpsilon;
            do
            {
                xk = -(z * k1 * k2) / (k3 * k4);
                pk = pkm1 + pkm2 * xk;
                qk = qkm1 + qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;

                xk = (z * k5 * k6) / (k7 * k8);
                pk = pkm1 + pkm2 * xk;
                qk = qkm1 + qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;

                if (qk != 0) r = pk / qk;
                if (r != 0)
                {
                    t = System.Math.Abs((ans - r) / r);
                    ans = r;
                }
                else
                    t = 1.0;

                if (t < thresh) return ans;

                k1 += 1.0;
                k2 -= 1.0;
                k3 += 2.0;
                k4 += 2.0;
                k5 += 1.0;
                k6 += 1.0;
                k7 += 2.0;
                k8 += 2.0;

                if ((System.Math.Abs(qk) + System.Math.Abs(pk)) > big)
                {
                    pkm2 *= biginv;
                    pkm1 *= biginv;
                    qkm2 *= biginv;
                    qkm1 *= biginv;
                }
                if ((System.Math.Abs(qk) < biginv) || (System.Math.Abs(pk) < biginv))
                {
                    pkm2 *= big;
                    pkm1 *= big;
                    qkm2 *= big;
                    qkm1 *= big;
                }
            } while (++n < 300);

            return ans;
        }

        /// <summary>
        ///   Inverse of incomplete beta integral.
        /// </summary>
        /// 
        /// <example>
        ///   Please see <see cref="Beta"/>
        /// </example>
        /// 
        public static double IncompleteInverse(double aa, double bb, double yy0)
        {
            double a, b, y0, d, y, x, x0, x1, lgm, yp, di, dithresh, yl, yh;
            int i, dir;

            bool nflg;
            bool rflg;


            if (yy0 <= 0)
                return (0.0);
            if (yy0 >= 1.0)
                return (1.0);

            if (aa <= 1.0 || bb <= 1.0)
            {
                nflg = true;
                dithresh = 4.0 * Constants.DoubleEpsilon;
                rflg = false;
                a = aa;
                b = bb;
                y0 = yy0;
                x = a / (a + b);
                y = Incomplete(a, b, x);
                goto ihalve;
            }
            else
            {
                nflg = false;
                dithresh = 1.0e-4;
            }

            /* approximation to inverse function */

            yp = -Normal.Inverse(yy0);

            if (yy0 > 0.5)
            {
                rflg = true;
                a = bb;
                b = aa;
                y0 = 1.0 - yy0;
                yp = -yp;
            }
            else
            {
                rflg = false;
                a = aa;
                b = bb;
                y0 = yy0;
            }

            lgm = (yp * yp - 3.0) / 6.0;
            x0 = 2.0 / (1.0 / (2.0 * a - 1.0) + 1.0 / (2.0 * b - 1.0));
            y = yp * Math.Sqrt(x0 + lgm) / x0
                - (1.0 / (2.0 * b - 1.0) - 1.0 / (2.0 * a - 1.0))
                * (lgm + 5.0 / 6.0 - 2.0 / (3.0 * x0));
            y = 2.0 * y;

            if (y < Constants.LogMin)
            {
                x0 = 1.0;
                throw new ArithmeticException("underflow");
            }

            x = a / (a + b * Math.Exp(y));
            y = Incomplete(a, b, x);
            yp = (y - y0) / y0;

            if (Math.Abs(yp) < 1.0e-2)
                goto newt;

        ihalve:

            /* Resort to interval halving if not close enough */
            x0 = 0.0;
            yl = 0.0;
            x1 = 1.0;
            yh = 1.0;
            di = 0.5;
            dir = 0;

            for (i = 0; i < 400; i++)
            {
                if (i != 0)
                {
                    x = x0 + di * (x1 - x0);
                    if (x == 1.0)
                        x = 1.0 - Constants.DoubleEpsilon;
                    y = Incomplete(a, b, x);
                    yp = (x1 - x0) / (x1 + x0);
                    if (Math.Abs(yp) < dithresh)
                    {
                        x0 = x;
                        goto newt;
                    }
                }

                if (y < y0)
                {
                    x0 = x;
                    yl = y;
                    if (dir < 0)
                    {
                        dir = 0;
                        di = 0.5;
                    }
                    else if (dir > 1)
                        di = 0.5 * di + 0.5;
                    else
                        di = (y0 - y) / (yh - yl);
                    dir += 1;
                    if (x0 > 0.75)
                    {
                        if (rflg)
                        {
                            rflg = false;
                            a = aa;
                            b = bb;
                            y0 = yy0;
                        }
                        else
                        {
                            rflg = true;
                            a = bb;
                            b = aa;
                            y0 = 1.0 - yy0;
                        }
                        x = 1.0 - x;
                        y = Incomplete(a, b, x);
                        goto ihalve;
                    }
                }
                else
                {
                    x1 = x;
                    if (rflg && x1 < Constants.DoubleEpsilon)
                    {
                        x0 = 0.0;
                        goto done;
                    }
                    yh = y;
                    if (dir > 0)
                    {
                        dir = 0;
                        di = 0.5;
                    }
                    else if (dir < -1)
                        di = 0.5 * di;
                    else
                        di = (y - y0) / (yh - yl);
                    dir -= 1;
                }
            }

            if (x0 >= 1.0)
            {
                x0 = 1.0 - Constants.DoubleEpsilon;
                goto done;
            }

            if (x == 0.0)
                throw new ArithmeticException("underflow");

        newt:

            if (nflg)
                goto done;

            x0 = x;
            lgm = Gamma.Log(a + b) - Gamma.Log(a) - Gamma.Log(b);

            for (i = 0; i < 10; i++)
            {
                /* Compute the function at this point. */
                if (i != 0)
                    y = Incomplete(a, b, x0);

                /* Compute the derivative of the function at this point. */
                d = (a - 1.0) * Math.Log(x0) + (b - 1.0) * Math.Log(1.0 - x0) + lgm;

                if (d < Constants.LogMin)
                    throw new ArithmeticException("underflow");

                d = Math.Exp(d);

                /* compute the step to the next approximation of x */
                d = (y - y0) / d;
                x = x0;
                x0 = x0 - d;

                if (x0 <= 0.0)
                    throw new ArithmeticException("underflow");

                if (x0 >= 1.0)
                {
                    x0 = 1.0 - Constants.DoubleEpsilon;
                    goto done;
                }

                if (Math.Abs(d / x0) < 64.0 * Constants.DoubleEpsilon)
                    goto done;
            }

        done:
            if (rflg)
            {
                if (x0 <= Double.Epsilon)
                    x0 = 1.0 - Double.Epsilon;
                else
                    x0 = 1.0 - x0;
            }
            return (x0);
        }

        /// <summary>
        ///   Power series for incomplete beta integral. Use when b*x
        ///   is small and x not too close to 1.
        /// </summary>
        /// 
        /// <example>
        ///   Please see <see cref="Beta"/>
        /// </example>
        /// 
        public static double PowerSeries(double a, double b, double x)
        {
            double s, t, u, v, n, t1, z, ai;

            ai = 1.0 / a;
            u = (1.0 - b) * x;
            v = u / (a + 1.0);
            t1 = v;
            t = u;
            n = 2.0;
            s = 0.0;
            z = Constants.DoubleEpsilon * ai;
            while (System.Math.Abs(v) > z)
            {
                u = (n - b) * x / n;
                t *= u;
                v = t / (a + n);
                s += v;
                n += 1.0;
            }
            s += t1;
            s += ai;

            u = a * System.Math.Log(x);
            if ((a + b) < Gamma.GammaMax && System.Math.Abs(u) < Constants.LogMax)
            {
                t = Gamma.Function(a + b) / (Gamma.Function(a) * Gamma.Function(b));
                s = s * t * System.Math.Pow(x, a);
            }
            else
            {
                t = Gamma.Log(a + b) - Gamma.Log(a) - Gamma.Log(b) + u + System.Math.Log(s);
                if (t < Constants.LogMin) s = 0.0;
                else s = System.Math.Exp(t);
            }
            return s;
        }

        /// <summary>
        ///   Multinomial Beta function.
        /// </summary>
        /// 
        /// <example>
        ///   Please see <see cref="Beta"/>
        /// </example>
        /// 
        public static double Multinomial(params double[] x)
        {
            double sum = 0;
            double prd = 1;

            for (int i = 0; i < x.Length; i++)
            {
                sum += x[i];
                prd *= Gamma.Function(x[i]);
            }

            return prd / Gamma.Function(sum);
        }
    }
}
