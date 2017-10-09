// Accord Math Library
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
    using System.Runtime.CompilerServices;

    /// <summary>
    ///   Set of special mathematic functions.
    /// </summary>
    ///  
    /// <remarks>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Cephes Math Library, http://www.netlib.org/cephes/ </description></item>
    ///     <item><description>
    ///       John D. Cook, http://www.johndcook.com/ </description></item>
    ///   </list>
    /// </remarks>
    /// 
    public static class Special
    {

        private static readonly double[] erfc_P =
        {
            2.46196981473530512524E-10,
            5.64189564831068821977E-1,
            7.46321056442269912687E0,
            4.86371970985681366614E1,
            1.96520832956077098242E2,
            5.26445194995477358631E2,
            9.34528527171957607540E2,
            1.02755188689515710272E3,
            5.57535335369399327526E2
        };

        private static readonly double[] erfc_Q =
        {
            1.32281951154744992508E1,
            8.67072140885989742329E1,
            3.54937778887819891062E2,
            9.75708501743205489753E2,
            1.82390916687909736289E3,
            2.24633760818710981792E3,
            1.65666309194161350182E3,
            5.57535340817727675546E2
        };

        private static readonly double[] erfc_R =
        {
            5.64189583547755073984E-1,
            1.27536670759978104416E0,
            5.01905042251180477414E0,
            6.16021097993053585195E0,
            7.40974269950448939160E0,
            2.97886665372100240670E0
        };

        private static readonly double[] erfc_S =
        {
            2.26052863220117276590E0,
            9.39603524938001434673E0,
            1.20489539808096656605E1,
            1.70814450747565897222E1,
            9.60896809063285878198E0,
            3.36907645100081516050E0
        };

        /// <summary>
        ///   Complementary error function of the specified value.
        /// </summary>
        /// 
        /// <remarks>
        ///   http://mathworld.wolfram.com/Erfc.html
        /// </remarks>
        /// 
        public static double Erfc(double value)
        {
            double x, y, z, p, q;

            if (value < 0.0)
                x = -value;
            else
                x = value;

            if (x < 1.0)
                return 1.0 - Erf(value);

            z = -value * value;

            if (z < -Constants.LogMax)
            {
                if (value < 0)
                    return (2.0);
                else
                    return (0.0);
            }

            z = System.Math.Exp(z);

            if (x < 8.0)
            {
                p = Polevl(x, erfc_P, 8);
                q = P1evl(x, erfc_Q, 8);
            }
            else
            {
                p = Polevl(x, erfc_R, 5);
                q = P1evl(x, erfc_S, 6);
            }

            y = (z * p) / q;

            if (value < 0)
                y = 2.0 - y;

            if (y == 0.0)
            {
                if (value < 0)
                    return 2.0;
                else
                    return (0.0);
            }

            return y;
        }

        private static readonly double[] erfc_T =
        {
            9.60497373987051638749E0,
            9.00260197203842689217E1,
            2.23200534594684319226E3,
            7.00332514112805075473E3,
            5.55923013010394962768E4
        };

        private static readonly double[] erfc_U =
        {
            3.35617141647503099647E1,
            5.21357949780152679795E2,
            4.59432382970980127987E3,
            2.26290000613890934246E4,
            4.92673942608635921086E4
        };

        /// <summary>
        ///   Error function of the specified value.
        /// </summary>
        /// 
        public static double Erf(double x)
        {
            double y, z;

            if (System.Math.Abs(x) > 1.0)
                return (1.0 - Erfc(x));

            z = x * x;
            y = x * Polevl(z, erfc_T, 4) / P1evl(z, erfc_U, 5);

            return y;
        }

        /// <summary>
        ///   Inverse error function (<see cref="Erf(double)"/>.
        /// </summary>
        /// 
        public static double Ierf(double y)
        {
            double s = Normal.Inverse(0.5 * y + 0.5);
            double r = s * Math.Sqrt(2) / 2.0;
            return r;
        }


        /// <summary>
        ///   Inverse complemented error function (<see cref="Erfc(double)"/>.
        /// </summary>
        /// 
        public static double Ierfc(double y)
        {
            double s = Normal.Inverse(-0.5 * y + 1);
            double r = s * Math.Sqrt(2) / 2.0;
            return r;
        }


        #region Polynomial and spline functions
        /// <summary>
        ///   Evaluates polynomial of degree N
        /// </summary>
        /// 
        public static double Polevl(double x, double[] coef, int n)
        {
            double ans;

            ans = coef[0];

            for (int i = 1; i <= n; i++)
                ans = ans * x + coef[i];

            return ans;
        }

        /// <summary>
        ///   Evaluates polynomial of degree N with assumption that coef[N] = 1.0
        /// </summary>
        /// 
        public static double P1evl(double x, double[] coef, int n)
        {
            double ans;

            ans = x + coef[0];

            for (int i = 1; i < n; i++)
                ans = ans * x + coef[i];

            return ans;
        }

        /// <summary>
        ///   Computes the Basic Spline of order <c>n</c>
        /// </summary>
        /// 
        public static double BSpline(int n, double x)
        {
            // ftp://ftp.esat.kuleuven.ac.be/pub/SISTA/hamers/PhD_bhamers.pdf
            // http://sepwww.stanford.edu/public/docs/sep105/sergey2/paper_html/node5.html

            double sum = 0.0;
            bool positive = true;
            for (int k = 0; k <= n + 1; k++)
            {
                double c = Binomial(n + 1, k) * Tools.TruncatedPower(x + (n + 1.0) / 2.0 - k, n);
                sum += positive ? c : -c;  // Sum terms over k
                positive = !positive;
            }

            return (1.0 / Special.Factorial(n)) * sum;  // Finally apply the 1/n! factor
        }

        #endregion

        #region Factorial and related functions
        /// <summary>
        ///   Computes the binomial coefficients C(n,k).
        /// </summary>
        /// 
        public static double Binomial(int n, int k)
        {
            return Math.Round(Math.Exp(LogFactorial(n) - LogFactorial(k) - LogFactorial(n - k)));
        }

        /// <summary>
        ///   Computes the binomial coefficients C(n,k).
        /// </summary>
        /// 
        public static double Binomial(double n, double k)
        {
            return Math.Round(Math.Exp(LogFactorial(n) - LogFactorial(k) - LogFactorial(n - k)));
        }

        /// <summary>
        ///   Computes the log binomial Coefficients Log[C(n,k)].
        /// </summary>
        /// 
        public static double LogBinomial(int n, int k)
        {
            return LogFactorial(n) - LogFactorial(k) - LogFactorial(n - k);
        }

        /// <summary>
        ///   Computes the log binomial Coefficients Log[C(n,k)].
        /// </summary>
        /// 
        public static double LogBinomial(double n, double k)
        {
            return LogFactorial(n) - LogFactorial(k) - LogFactorial(n - k);
        }

        /// <summary>
        ///   Returns the extended factorial definition of a real number.
        /// </summary>
        /// 
        public static double Factorial(double n)
        {
            return Gamma.Function(n + 1.0);
        }

        /// <summary>
        ///   Returns the log factorial of a number (ln(n!))
        /// </summary>
        /// 
        public static double LogFactorial(double n)
        {
            return Gamma.Log(n + 1.0);
        }

        /// <summary>
        ///   Returns the log factorial of a number (ln(n!))
        /// </summary>
        /// 
        public static double LogFactorial(int n)
        {
            if (lnfcache == null)
                lnfcache = new double[101];

            if (n < 0)
            {
                // Factorial is not defined for negative numbers.
                throw new ArgumentException("Argument cannot be negative.", "n");
            }
            if (n <= 1)
            {
                // Factorial for n between 0 and 1 is 1, so log(factorial(n)) is 0.
                return 0.0;
            }
            if (n <= 100)
            {
                // Compute the factorial using ln(gamma(n)) approximation, using the cache
                // if the value has been previously computed.
                return (lnfcache[n] > 0) ? lnfcache[n] : (lnfcache[n] = Gamma.Log(n + 1.0));
            }
            else
            {
                // Just compute the factorial using ln(gamma(n)) approximation.
                return Gamma.Log(n + 1.0);
            }
        }

        /// <summary>
        ///   Computes the factorial of a number (n!)
        /// </summary>
        public static double Factorial(int n)
        {
            if (fcache == null)
            {
                // Initialize factorial cache
                fcache = new double[33];
                fcache[0] = 1;
                fcache[1] = 1;
                fcache[2] = 2;
                fcache[3] = 6;
                fcache[4] = 24;
                ftop = 4;
            }

            if (n < 0)
            {
                // Factorial is not defined for negative numbers
                throw new ArgumentException("Argument can not be negative", "n");
            }
            if (n > 32)
            {
                // Return Gamma approximation using exp(gammaln(n+1)),
                //  which for some reason is faster than gamma(n+1).
                return Math.Exp(Gamma.Log(n + 1.0));
            }
            else
            {
                // Compute in the standard way, but use the
                //  factorial cache to speed up computations.
                while (ftop < n)
                {
                    int j = ftop++;
                    fcache[ftop] = fcache[j] * ftop;
                }
                return fcache[n];
            }
        }

        // factorial function caches
        private static int ftop;
        private static double[] fcache;
        private static double[] lnfcache;
        #endregion


        #region Utility functions

        /// <summary>
        ///   Computes log(1-x) without losing precision for small values of x.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Log1m(double x)
        {
            if (x >= 1.0)
                return Double.NaN;

            if (Math.Abs(x) > 1e-4)
                return Math.Log(1.0 - x);

            // Use Taylor approx. log(1 + x) = x - x^2/2 with error roughly x^3/3
            // Since |x| < 10^-4, |x|^3 < 10^-12, relative error less than 10^-8
            return -(0.5 * x + 1.0) * x;
        }

        /// <summary>
        ///   Computes log(1+x) without losing precision for small values of x.
        /// </summary>
        /// 
        /// <remarks>
        ///   References:
        ///   - http://www.johndcook.com/csharp_log_one_plus_x.html
        /// </remarks>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Log1p(double x)
        {
            if (x <= -1.0)
                return Double.NaN;

            if (Math.Abs(x) > 1e-4)
                return Math.Log(1.0 + x);

            // Use Taylor approx. log(1 + x) = x - x^2/2 with error roughly x^3/3
            // Since |x| < 10^-4, |x|^3 < 10^-12, relative error less than 10^-8
            return (-0.5 * x + 1.0) * x;
        }

        /// <summary>
        ///   Compute exp(x) - 1 without loss of precision for small values of x.
        /// </summary>
        /// <remarks>
        ///   References:
        ///   - http://www.johndcook.com/cpp_expm1.html
        /// </remarks>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Expm1(double x)
        {
            if (Math.Abs(x) < 1e-5)
                return x + 0.5 * x * x;
            else
                return Math.Exp(x) - 1.0;
        }

        /// <summary>
        ///   Estimates unit round-off in quantities of size x.
        /// </summary>
        /// <remarks>
        ///   This is a port of the epslon function from EISPACK.
        /// </remarks>
        /// 
        public static double Epslon(double x)
        {
            double a, b, c, eps;

            a = 1.3333333333333333;

        L10:
            b = a - 1.0;
            c = b + b + b;
            eps = System.Math.Abs(c - 1.0);

            if (eps == 0.0)
                goto L10;

            return eps * System.Math.Abs(x);
        }

        /// <summary>
        ///   Returns <paramref name="a"/> with the sign of <paramref name="b"/>. 
        /// </summary>
        /// 
        /// <remarks>
        ///   This is a port of the sign transfer function from EISPACK,
        ///   and is is equivalent to C++'s std::copysign function.
        /// </remarks>
        /// 
        /// <returns>If B > 0 then the result is ABS(A), else it is -ABS(A).</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Sign(double a, double b)
        {
            double x = (a >= 0 ? a : -a);
            return (b >= 0 ? x : -x);
        }

        /// <summary>
        ///   Computes x + y without losing precision using ln(x) and ln(y).
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double LogDiff(double lnx, double lny)
        {
            if (lnx > lny)
                return lnx + Math.Exp(1.0 - Math.Exp(lny - lnx));

            return Double.NegativeInfinity;
        }

        /// <summary>
        ///   Computes x + y without losing precision using ln(x) and ln(y).
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double LogSum(double lnx, double lny)
        {
            if (lnx == Double.NegativeInfinity)
                return lny;
            if (lny == Double.NegativeInfinity)
                return lnx;

            if (lnx > lny)
                return lnx + Special.Log1p(Math.Exp(lny - lnx));

            return lny + Special.Log1p(Math.Exp(lnx - lny));
        }

        /// <summary>
        ///   Computes x + y without losing precision using ln(x) and ln(y).
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double LogSum(float lnx, float lny)
        {
            if (lnx == Single.NegativeInfinity)
                return lny;
            if (lny == Single.NegativeInfinity)
                return lnx;

            if (lnx > lny)
                return lnx + Special.Log1p(Math.Exp(lny - lnx));

            return lny + Special.Log1p(Math.Exp(lnx - lny));
        }

        /// <summary>
        ///   Computes x + y without losing precision using ln(x) and ln(y).
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double LogSum(double[] values)
        {
            double logsum = Double.NegativeInfinity;
            for (int i = 0; i < values.Length; i++)
                logsum = Special.LogSum(logsum, values[i]);
            return logsum;
        }

        /// <summary>
        ///   Computes sum(x) without losing precision using ln(x_0) ... ln(x_n).
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double LogSumExp(this double[] array)
        {
            double sum = Double.NegativeInfinity;
            for (int i = 0; i < array.Length; i++)
                sum = Special.LogSum(array[i], sum);
            return sum;
        }
        #endregion


        #region Derived trigonometric functions
        //
        // http://msdn.microsoft.com/en-us/library/w3t84e33.aspx

        /// <summary>
        ///   Secant.
        /// </summary>
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Sec(double x)
        {
            return 1 / Math.Cos(x);
        }

        /// <summary>
        ///   Cosecant.
        /// </summary>
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Cosec(double x)
        {
            return 1 / Math.Sin(x);
        }

        /// <summary>
        ///   Cotangent.
        /// </summary>
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Cotan(double x)
        {
            return 1 / Math.Tan(x);
        }

        /// <summary>
        ///   Inverse secant.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Asec(double x)
        {
            double u = x * x - 1;
            return 2 * Math.Atan(1) - Math.Atan2(Math.Sign(x), u * u);
        }

        /// <summary>
        ///   Inverse cosecant.
        /// </summary>
        /// 
        public static double Acosec(double x)
        {
            double u = x * x - 1;
            return Math.Atan2(Math.Sign(x), u * u);
        }

        /// <summary>
        ///   Inverse cotangent.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Acotan(double x)
        {
            return 2 * Math.Atan(1) - Math.Atan(x);
        }

        /// <summary>
        ///   Hyperbolic secant.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Sech(double x)
        {
            return 2 / (Math.Exp(x) + Math.Exp(-x));
        }

        /// <summary>
        ///   Hyperbolic secant.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Cosech(double x)
        {
            return 2 / (Math.Exp(x) - Math.Exp(-x));
        }

        /// <summary>
        ///   Hyperbolic cotangent.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Cotanh(double x)
        {
            return (Math.Exp(x) + Math.Exp(-x)) / (Math.Exp(x) - Math.Exp(-x));
        }

        /// <summary>
        ///   Inverse hyperbolic sin.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Asinh(double x)
        {
            double u = x * x + 1;
            return Math.Log(x + u * u);
        }

        /// <summary>
        ///   Inverse hyperbolic cos.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Acosh(double x)
        {
            double u = x * x - 1;
            return Math.Log(x + u * u);
        }

        /// <summary>
        ///   Inverse hyperbolic tangent.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Atanh(double x)
        {
            return Math.Log((1 + x) / (1 - x)) / 2;
        }

        /// <summary>
        ///   Inverse hyperbolic secant.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Asech(double x)
        {
            double u = -x * x + 1;
            return Math.Log((u * u + 1) / x);
        }

        /// <summary>
        ///   Inverse hyperbolic cosecant.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Acosech(double x)
        {
            double u = x * x + 1;
            return Math.Log((Math.Sign(x) * u * u + 1) / x);
        }

        /// <summary>
        ///   Inverse hyperbolic cotangent.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Acotanh(double x)
        {
            return Math.Log((x + 1) / (x - 1)) / 2;
        }

        #endregion

        /// <summary>
        ///   Computes the Softmax function (also known as normalized Exponencial
        ///   function) that "squashes"a vector or arbitrary real values into a 
        ///   vector of real values in the range (0, 1) that add up to 1.
        /// </summary>
        /// 
        /// <param name="input">The real values to be converted into the unit interval.</param>
        /// 
        /// <returns>A vector with the same number of dimensions as <paramref name="input"/>
        ///   but where values lie between 0 and 1.</returns>
        ///   
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] Softmax(double[] input)
        {
            return Softmax(input, new double[input.Length]);
        }

        /// <summary>
        ///   Computes the Softmax function (also known as normalized Exponencial
        ///   function) that "squashes"a vector or arbitrary real values into a 
        ///   vector of real values in the range (0, 1) that add up to 1.
        /// </summary>
        /// 
        /// <param name="input">The real values to be converted into the unit interval.</param>
        /// <param name="result">The location where to store the result of this operation.</param>
        /// 
        /// <returns>A vector with the same number of dimensions as <paramref name="input"/>
        ///   but where values lie between 0 and 1.</returns>
        ///   
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] Softmax(double[] input, double[] result)
        {
            double sum = Special.LogSumExp(input);

            for (int i = 0; i < input.Length; i++)
                result[i] = Math.Exp(input[i] - sum);

            return result;
        }

        /// <summary>
        ///   Computes log(1 + exp(x)) without losing precision.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Log1pexp(double x)
        {
            // Computes Math.Log(1.0 / (1.0 + Math.Exp(-sum)));
            // https://cran.r-project.org/web/packages/Rmpfr/vignettes/log1mexp-note.pdf

            if (x < -37)
                return Math.Exp(x);
            if (x <= 18)
                return Special.Log1p(Math.Exp(x));
            if (x <= 33)
                return x + Math.Exp(-x);
            return x;
        }
    }
}
