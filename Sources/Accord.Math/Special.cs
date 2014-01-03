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

            double[] P =
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
            double[] Q =
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

            double[] R =
            {
                5.64189583547755073984E-1,
                1.27536670759978104416E0,
                5.01905042251180477414E0,
                6.16021097993053585195E0,
                7.40974269950448939160E0,
                2.97886665372100240670E0
			};
            double[] S =
            {
                2.26052863220117276590E0,
                9.39603524938001434673E0,
                1.20489539808096656605E1,
                1.70814450747565897222E1,
                9.60896809063285878198E0,
                3.36907645100081516050E0
			};

            if (value < 0.0) x = -value;
            else x = value;

            if (x < 1.0) return 1.0 - Erf(value);

            z = -value * value;

            if (z < -Constants.LogMax)
            {
                if (value < 0) return (2.0);
                else return (0.0);
            }

            z = System.Math.Exp(z);

            if (x < 8.0)
            {
                p = Polevl(x, P, 8);
                q = P1evl(x, Q, 8);
            }
            else
            {
                p = Polevl(x, R, 5);
                q = P1evl(x, S, 6);
            }

            y = (z * p) / q;

            if (value < 0) y = 2.0 - y;

            if (y == 0.0)
            {
                if (value < 0) return 2.0;
                else return (0.0);
            }


            return y;
        }

        /// <summary>
        ///   Error function of the specified value.
        /// </summary>
        /// 
        public static double Erf(double x)
        {
            double y, z;
            double[] T =
            {
                9.60497373987051638749E0,
                9.00260197203842689217E1,
                2.23200534594684319226E3,
                7.00332514112805075473E3,
                5.55923013010394962768E4
		    };
            double[] U =
            {
                3.35617141647503099647E1,
                5.21357949780152679795E2,
                4.59432382970980127987E3,
                2.26290000613890934246E4,
                4.92673942608635921086E4
		    };

            if (System.Math.Abs(x) > 1.0)
                return (1.0 - Erfc(x));

            z = x * x;
            y = x * Polevl(z, T, 4) / P1evl(z, U, 5);

            return y;
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
        public static double BSpline(int n, double x)
        {
            // ftp://ftp.esat.kuleuven.ac.be/pub/SISTA/hamers/PhD_bhamers.pdf
            // http://sepwww.stanford.edu/public/docs/sep105/sergey2/paper_html/node5.html

            if (n == Int32.MaxValue)
                throw new ArgumentOutOfRangeException("n");


            double a = 1.0 / Special.Factorial(n);
            double c;

            bool positive = true;
            for (int k = 0; k <= n + 1; k++)
            {
                c = Binomial(n + 1, k) * Tools.TruncatedPower(x + (n + 1.0) / 2.0 - k, n);
                a += positive ? c : -c;
                positive = !positive;
            }

            return a;
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
                fcache[0] = 1; fcache[1] = 1;
                fcache[2] = 2; fcache[3] = 6;
                fcache[4] = 24; ftop = 4;
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
        public static double Epslon(double x)
        {
            double a, b, c, eps;

            a = 1.3333333333333333;

        L10:
            b = a - 1.0;
            c = b + b + b;
            eps = System.Math.Abs(c - 1.0);

            if (eps == 0.0) goto L10;

            return eps * System.Math.Abs(x);
        }

        /// <summary>
        ///   Returns A with the sign of B.
        /// </summary>
        /// <remarks>
        ///   This is a port of the sign transfer function from EISPACK.
        /// </remarks>
        /// <returns>If B > 0 then the result is ABS(A), else it is -ABS(A).</returns>
        public static double Sign(double a, double b)
        {
            double x = (a >= 0 ? a : -a);
            return (b >= 0 ? x : -x);
        }

        /// <summary>
        ///   Computes x + y without losing precision using ln(x) and ln(y).
        /// </summary>
        /// 
        public static double LogDiff(double lna, double lnc)
        {
            if (lna > lnc)
                return lna + Math.Exp(1.0 - Math.Exp(lnc - lna));

            return Double.NegativeInfinity;
        }

        /// <summary>
        ///   Computes x + y without losing precision using ln(x) and ln(y).
        /// </summary>
        /// 
        public static double LogSum(double lna, double lnc)
        {
            if (lna == Double.NegativeInfinity)
                return lnc;
            if (lnc == Double.NegativeInfinity)
                return lna;

            if (lna > lnc)
                return lna + Special.Log1p(Math.Exp(lnc - lna));

            return lnc + Special.Log1p(Math.Exp(lna - lnc));
        }

        /// <summary>
        ///   Computes x + y without losing precision using ln(x) and ln(y).
        /// </summary>
        /// 
        public static double LogSum(float lna, float lnc)
        {
            if (lna == Single.NegativeInfinity)
                return lnc;
            if (lnc == Single.NegativeInfinity)
                return lna;

            if (lna > lnc)
                return lna + Special.Log1p(Math.Exp(lnc - lna));

            return lnc + Special.Log1p(Math.Exp(lna - lnc));
        }


        #endregion

    }
}
