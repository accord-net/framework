// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// Copyright © John Burkardt, 2012
// http://people.sc.fsu.edu/~jburkardt/
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

namespace Accord.Math
{
    using System;

    /// <summary>
    ///   Owen's T function and related functions.
    /// </summary>
    /// 
    /// <remarks>
    /// 
    /// <para>
    ///   In mathematics, Owen's T function T(h, a), named after statistician Donald Bruce Owen,
    ///   is defined by</para>
    /// <code>
    ///                  1   a   exp{-0.5 h²(1+x²)
    ///     T(h, a) =  ----  ∫  ------------------- dx
    ///                 2π   0        1 + x²
    /// </code>
    /// 
    /// <para>
    ///   The function <c>T(h, a)</c> gives the probability of the event <c>(X > h and 0 &lt; Y &lt; aX)</c>
    ///   where <c>X</c> and <c>Y</c> are independent standard normal random variables. This function can 
    ///   be used to calculate <see cref="Normal.Bivariate">bivariate normal distribution probabilities</see>
    ///   and, from there, in the calculation of multivariate normal distribution probabilities. It also
    ///   frequently appears in various integrals involving <see cref="Normal">Gaussian</see> functions.
    /// </para>
    /// 
    /// <para>
    ///   The code is based on the original FORTRAN77 version by Mike Patefield, David Tandy;
    ///   and the C version created by John Burkardt. The original code for the C version can
    ///   be found at http://people.sc.fsu.edu/~jburkardt/c_src/owens/owens.html  and is valid
    ///   under the LGPL.</para>
    /// 
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       http://people.sc.fsu.edu/~jburkardt/c_src/owens/owens.html </description></item>
    ///     <item><description>
    ///       Mike Patefield, David Tandy, Fast and Accurate Calculation of Owen's T Function,
    ///       Journal of Statistical Software, Volume 5, Number 5, 2000, pages 1-25.
    ///       </description></item>
    ///   </list></para>  
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// // Computes Owens' T function
    /// double t = OwensT.Function(h: 2, a: 42); // 0.011375065974089608
    /// </code>
    /// </example>
    /// 
    public static class OwensT
    {
        /// <summary>
        ///   Computes Owen's T function for arbitrary H and A.
        /// </summary>
        /// 
        /// <param name="h">Owen's T function argument H.</param>
        /// <param name="a">Owen's T function argument A.</param>
        /// 
        /// <returns>The value of Owen's T function.</returns>
        /// 
        public static double Function(double h, double a)
        {
            double absa;
            double absh;
            double ah;
            const double cut = 0.67;
            double normah;
            double normh;
            double value;

            absh = Math.Abs(h);
            absa = Math.Abs(a);
            ah = absa * absh;

            if (absa <= 1.0)
            {
                value = Function(absh, absa, ah);
            }
            else if (absh <= cut)
            {
                value = 0.25 - (-0.5 + Normal.Function(absh)) * (-0.5 + Normal.Function(ah))
                  - Function(ah, 1.0 / absa, absh);
            }
            else
            {
                normh = Normal.Complemented(absh);
                normah = Normal.Complemented(ah);
                value = 0.5 * (normh + normah) - normh * normah
                - Function(ah, 1.0 / absa, absh);
            }

            if (a < 0.0)
                value = -value;

            return value;
        }

        private static readonly double[] arange = 
        {
            0.025, 0.09, 0.15, 0.36, 0.5,
            0.9, 0.99999 
        };

        private static readonly double[] c2 = 
        {
                                        0.99999999999999987510,
            -0.99999999999988796462,      0.99999999998290743652,
            -0.99999999896282500134,      0.99999996660459362918,
            -0.99999933986272476760,      0.99999125611136965852,
            -0.99991777624463387686,      0.99942835555870132569,
            -0.99697311720723000295,      0.98751448037275303682,
            -0.95915857980572882813,      0.89246305511006708555,
            -0.76893425990463999675,      0.58893528468484693250,
            -0.38380345160440256652,      0.20317601701045299653,
            -0.82813631607004984866E-01,  0.24167984735759576523E-01,
            -0.44676566663971825242E-02,  0.39141169402373836468E-03 
        };

        private static readonly double[] hrange = 
        {
            0.02, 0.06, 0.09, 0.125, 0.26,
            0.4,  0.6,  1.6,  1.7,   2.33,
            2.4,  3.36, 3.4,  4.8 
        };

        private static readonly int[] meth = 
        {
            1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 3, 4, 4, 4, 4, 5, 6 
        };

        private static readonly int[] ord = 
        {
            2, 3, 4, 5, 7,10,12,18,10,20,30,20, 4, 7, 8,20,13, 0 
        };

        private static readonly double[] pts = 
        {
                                        0.35082039676451715489E-02,
            0.31279042338030753740E-01,  0.85266826283219451090E-01,
            0.16245071730812277011,      0.25851196049125434828,
            0.36807553840697533536,      0.48501092905604697475,
            0.60277514152618576821,      0.71477884217753226516,
            0.81475510988760098605,      0.89711029755948965867,
            0.95723808085944261843,      0.99178832974629703586 
        };

        private static readonly int[] select = 
        {
            1, 1, 2,13,13,13,13,13,13,13,13,16,16,16, 9,
            1, 2, 2, 3, 3, 5, 5,14,14,15,15,16,16,16, 9,
            2, 2, 3, 3, 3, 5, 5,15,15,15,15,16,16,16,10,
            2, 2, 3, 5, 5, 5, 5, 7, 7,16,16,16,16,16,10,
            2, 3, 3, 5, 5, 6, 6, 8, 8,17,17,17,12,12,11,
            2, 3, 5, 5, 5, 6, 6, 8, 8,17,17,17,12,12,12,
            2, 3, 4, 4, 6, 6, 8, 8,17,17,17,17,17,12,12,
            2, 3, 4, 4, 6, 6,18,18,18,18,17,17,17,12,12 
        };

        private static readonly double[] wts = 
        {
                                        0.18831438115323502887E-01,
            0.18567086243977649478E-01,  0.18042093461223385584E-01,
            0.17263829606398753364E-01,  0.16243219975989856730E-01,
            0.14994592034116704829E-01,  0.13535474469662088392E-01,
            0.11886351605820165233E-01,  0.10070377242777431897E-01,
            0.81130545742299586629E-02,  0.60419009528470238773E-02,
            0.38862217010742057883E-02,  0.16793031084546090448E-02 
        };

        /// <summary>
        ///   Owen's T function for a restricted range of parameters.
        /// </summary>
        /// 
        /// <param name="h">Owen's T function argument H (where 0 &lt;= H).</param>
        /// <param name="a">Owen's T function argument A (where 0 &lt;= A &lt;= 1).</param>
        /// <param name="ah">The value of A*H.</param>
        /// 
        /// <returns>The value of Owen's T function.</returns>
        /// 
        public static double Function(double h, double a, double ah)
        {
            double ai;
            double aj;
            double AS;
            double dhs;
            double dj;
            double gj;

            double hs;
            int i;
            int iaint;
            int icode;
            int ihint;
            int ii;
            int j;
            int jj;
            int m;
            int maxii;
            double normh;

            double r;
            const double rrtpi = 0.39894228040143267794;
            const double rtwopi = 0.15915494309189533577;

            double y;
            double yi;
            double z;
            double zi;

            double value = 0;
            double vi;


            /*
              Determine appropriate method from t1...t6
            */
            ihint = 15;

            for (i = 1; i <= 14; i++)
            {
                if (h <= hrange[i - 1])
                {
                    ihint = i;
                    break;
                }
            }

            iaint = 8;

            for (i = 1; i <= 7; i++)
            {
                if (a <= arange[i - 1])
                {
                    iaint = i;
                    break;
                }
            }

            icode = select[ihint - 1 + (iaint - 1) * 15];
            m = ord[icode - 1];

            /*
              t1(h, a, m) ; m = 2, 3, 4, 5, 7, 10, 12 or 18
              jj = 2j - 1 ; gj = exp(-h*h/2) * (-h*h/2)**j / j
              aj = a**(2j-1) / (2*pi)
            */

            if (meth[icode - 1] == 1)
            {
                hs = -0.5 * h * h;
                dhs = Math.Exp(hs);
                AS = a * a;
                j = 1;
                jj = 1;
                aj = rtwopi * a;
                value = rtwopi * Math.Atan(a);
                dj = dhs - 1.0;
                gj = hs * dhs;

                for (; ; )
                {
                    value = value + dj * aj / (double)(jj);

                    if (m <= j)
                    {
                        return value;
                    }
                    j = j + 1;
                    jj = jj + 2;
                    aj = aj * AS;
                    dj = gj - dj;
                    gj = gj * hs / (double)(j);
                }
            }

          /*
            t2(h, a, m) ; m = 10, 20 or 30
            z = (-1)^(i-1) * zi ; ii = 2i - 1
            vi = (-1)^(i-1) * a^(2i-1) * exp[-(a*h)^2/2] / sqrt(2*pi)
          */
            else if (meth[icode - 1] == 2)
            {
                maxii = m + m + 1;
                ii = 1;
                value = 0.0;
                hs = h * h;
                AS = -a * a;
                vi = rrtpi * a * Math.Exp(-0.5 * ah * ah);
                z = 0.5 * (-0.5 + Normal.Function(ah)) / h;
                y = 1.0 / hs;

                for (; ; )
                {
                    value = value + z;

                    if (maxii <= ii)
                    {
                        value = value * rrtpi * Math.Exp(-0.5 * hs);
                        return value;
                    }
                    z = y * (vi - (double)(ii) * z);
                    vi = AS * vi;
                    ii = ii + 2;
                }
            }
            /*
              t3(h, a, m) ; m = 20
              ii = 2i - 1
              vi = a**(2i-1) * exp[-(a*h)**2/2] / sqrt(2*pi)
            */
            else if (meth[icode - 1] == 3)
            {
                i = 1;
                ii = 1;
                value = 0.0;
                hs = h * h;
                AS = a * a;
                vi = rrtpi * a * Math.Exp(-0.5 * ah * ah);
                zi = 0.5 * (-0.5 + Normal.Function(ah)) / h;
                y = 1.0 / hs;

                for (; ; )
                {
                    value = value + zi * c2[i - 1];

                    if (m < i)
                    {
                        value = value * rrtpi * Math.Exp(-0.5 * hs);
                        return value;
                    }
                    zi = y * ((double)(ii) * zi - vi);
                    vi = AS * vi;
                    i = i + 1;
                    ii = ii + 2;
                }
            }
            /*
              t4(h, a, m) ; m = 4, 7, 8 or 20;  ii = 2i + 1
              ai = a * exp[-h*h*(1+a*a)/2] * (-a*a)**i / (2*pi)
            */
            else if (meth[icode - 1] == 4)
            {
                maxii = m + m + 1;
                ii = 1;
                hs = h * h;
                AS = -a * a;
                value = 0.0;
                ai = rtwopi * a * Math.Exp(-0.5 * hs * (1.0 - AS));
                yi = 1.0;

                for (; ; )
                {
                    value = value + ai * yi;

                    if (maxii <= ii)
                        return value;

                    ii = ii + 2;
                    yi = (1.0 - hs * yi) / (double)(ii);
                    ai = ai * AS;
                }
            }
            /*
              t5(h, a, m) ; m = 13
              2m - point gaussian quadrature
            */
            else if (meth[icode - 1] == 5)
            {
                value = 0.0;
                AS = a * a;
                hs = -0.5 * h * h;
                for (i = 1; i <= m; i++)
                {
                    r = 1.0 + AS * pts[i - 1];
                    value = value + wts[i - 1] * Math.Exp(hs * r) / r;
                }
                value = a * value;
            }
            /*
              t6(h, a);  approximation for a near 1, (a<=1)
            */
            else if (meth[icode - 1] == 6)
            {
                normh = Normal.Complemented(h);
                value = 0.5 * normh * (1.0 - normh);
                y = 1.0 - a;
                r = Math.Atan(y / (1.0 + a));

                if (r != 0.0)
                    value = value - rtwopi * r * Math.Exp(-0.5 * y * h * h / r);
            }

            return value;
        }
    }
}
