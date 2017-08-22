// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms o the GNU Lesser General Public
//    License as published by the Free Softwaref Foundation; either
//    version 2.1 o the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for moref details.
//
//    You should have received a copy o the GNU Lesser General Public
//    License along with this library; i not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Math.Integration
{
    using System;
    using AForge;

    /// <summary>
    ///   Status codes for the <see cref="InfiniteAdaptiveGaussKronrodStatus"/>
    ///   integration method.
    /// </summary>
    /// 
    public enum InfiniteAdaptiveGaussKronrodStatus
    {
        /// <summary>
        ///   The integration calculation has been completed with success.
        ///   The obtained result is under the selected convergence criteria.
        /// </summary>
        /// 
        Success,

        /// <summary>
        ///   Maximum number of allowed subdivisions has been reached.
        /// </summary>
        /// 
        /// <remarks>
        ///  The maximum number of subdivisions allowed has been achieved. One can allow 
        ///  more subdivisions by increasing the value of limit (and taking the according
        ///  dimension adjustments into account). However, if this yields no improvement 
        ///  it is advised to analyze the integrand in order to determine the integration 
        ///  difficulties. If the position of a local difficulty can be determined (e.g. 
        ///  singularity, discontinuity within the interval) one will probably gain from
        ///  splitting up the interval at this point and calling the integrator on the 
        ///  subranges. if possible, an appropriate special-purpose integrator should be
        ///  used, which is designed for handling the type of difficulty involved.
        /// </remarks>
        /// 
        MaximumSubdivisions = 1,

        /// <summary>
        ///   Roundoff errors prevent the tolerance from being reached.
        /// </summary>
        /// 
        /// <remarks>
        ///   The occurrence of roundoff error is detected, which prevents the requested 
        ///   tolerance from being achieved. The error may be under-estimated.
        /// </remarks>
        /// 
        RoundoffError = 2,

        /// <summary>
        ///   There are severe discontinuities in the integrand function.
        /// </summary>
        /// 
        /// <remarks>
        ///   Extremely bad integrand behaviour occurs at some points of the 
        ///   integration interval.
        /// </remarks>
        /// 
        BadBehavioredFunction = 3,

        /// <summary>
        ///   The algorithm cannot converge.
        /// </summary>
        /// 
        /// <remarks>
        ///   The algorithm does not converge. Roundoff error is detected in the
        ///   extrapolation table. It is assumed that the requested tolerance cannot
        ///   be achieved, and that the returned result is the best which can be obtained.
        /// </remarks>
        /// 
        AlgorithmDivergence = 4,

        /// <summary>
        ///   The integral is divergent or slowly convergent.
        /// </summary>
        /// 
        /// <remarks>
        ///   The integral is probably divergent, or slowly convergent. It must be
        ///   noted that divergence can occur with any other error code.
        /// </remarks>
        /// 
        IntegralDiverence = 5,

    }

    /// <summary>
    ///   Infinite Adaptive Gauss-Kronrod integration method. 
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In applied mathematics, adaptive quadrature is a process in which the
    ///   integral of a function f(x) is approximated using static quadrature rules
    ///   on adaptively refined subintervals of the integration domain. Generally, 
    ///   adaptive algorithms are just as efficient and effective as traditional
    ///   algorithms for "well behaved" integrands, but are also effective for 
    ///   "badly behaved" integrands for which traditional algorithms fail.</para>
    /// 
    /// <para>
    ///   The algorithm implemented by this class has been based on the original FORTRAN 
    ///   implementation from QUADPACK. The function implemented the Non-adaptive Gauss-
    ///   Kronrod integration is <c>qagi(f,bound,inf,epsabs,epsrel,result,abserr,neval,
    ///   ier,limit,lenw,last,iwork,work)</c>. The original source code is in the public 
    ///   domain, but this version is under the LGPL. The original authors, as long as the 
    ///   original routine description, are listed below:</para>
    ///   
    /// <para>
    ///   Robert Piessens, Elise de Doncker; Applied Mathematics and Programming Division,
    ///   K.U.Leuven, Leuvenappl. This routine calculates an approximation result to a given 
    ///   integral   i = integral of f over (bound,+infinity) or i = integral of f over 
    ///   (-infinity,bound) or i = integral of f over (-infinity,+infinity) hopefully satisfying
    ///   following claim for accuracy abs(i-result).le.max(epsabs,epsrel*abs(i)).</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Adaptive_quadrature">
    ///       Wikipedia, The Free Encyclopedia. Adaptive quadrature. Available on: 
    ///       http://en.wikipedia.org/wiki/Adaptive_quadrature </a></description></item>
    ///     <item><description><a href="http://en.wikipedia.org/wiki/QUADPACK">
    ///       Wikipedia, The Free Encyclopedia. QUADPACK. Available on: 
    ///       http://en.wikipedia.org/wiki/QUADPACK </a></description></item>
    ///     <item><description><a href="http://www.netlib.no/netlib/quadpack/qagi.f">
    ///       Robert Piessens, Elise de Doncker; Non-adaptive integration standard fortran 
    ///       subroutine (qng.f). Applied Mathematics and Programming Division, K.U.Leuven,
    ///       Leuvenappl. Available at: http://www.netlib.no/netlib/quadpack/qagi.f </a>
    ///     </description></item>
    ///   </list>
    ///  </para>
    ///  </remarks>
    /// 
    /// <example>
    /// <para>
    ///   Let's say we would like to compute the definite integral of the function 
    ///   <c>f(x) = cos(x)</c> in the interval -1 to +1 using a variety of integration 
    ///   methods, including the <see cref="TrapezoidalRule"/>, <see cref="RombergMethod"/>
    ///   and <see cref="NonAdaptiveGaussKronrod"/>. Those methods can compute definite
    ///   integrals where the integration interval is finite:
    /// </para>
    /// 
    /// <code>
    /// // Declare the function we want to integrate
    /// Func&lt;double, double> f = (x) => Math.Cos(x);
    ///
    /// // We would like to know its integral from -1 to +1
    /// double a = -1, b = +1;
    ///
    /// // Integrate!
    /// double trapez  = TrapezoidalRule.Integrate(f, a, b, steps: 1000); // 1.6829414
    /// double romberg = RombergMethod.Integrate(f, a, b);                // 1.6829419
    /// double nagk    = NonAdaptiveGaussKronrod.Integrate(f, a, b);      // 1.6829419
    /// </code>
    /// 
    /// <para>
    ///   Moreover, it is also possible to calculate the value of improper integrals
    ///   (it is, integrals with infinite bounds) using <see cref="InfiniteAdaptiveGaussKronrod"/>,
    ///   as shown below. Let's say we would like to compute the area under the Gaussian
    ///   curve from -infinite to +infinite. While this function has infinite bounds, this
    ///   function is known to integrate to 1.</para>
    ///   
    /// <code>
    /// // Declare the Normal distribution's density function (which is the Gaussian's bell curve)
    /// Func&lt;double, double> g = (x) => (1 / Math.Sqrt(2 * Math.PI)) * Math.Exp(-(x * x) / 2);
    ///
    /// // Integrate!
    /// double iagk = InfiniteAdaptiveGaussKronrod.Integrate(g,
    ///     Double.NegativeInfinity, Double.PositiveInfinity);   // Result should be 0.99999...
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="TrapezoidalRule"/>
    /// <seealso cref="RombergMethod"/>
    /// <seealso cref="InfiniteAdaptiveGaussKronrod"/>
    /// <seealso cref="MonteCarloIntegration"/>
    /// 
    public class InfiniteAdaptiveGaussKronrod : IUnivariateIntegration,
        INumericalIntegration<InfiniteAdaptiveGaussKronrodStatus>
    {
        private int[] iwork;
        private double[] work;
        private double result;
        private double error;
        private int evaluations;

        private double abstol;
        private double reltol;

        private int limit;
        private int lenw;

        /// <summary>
        ///   Get the maximum number of subintervals to be utilized in the
        ///   partition of the <see cref="Range">integration interval</see>.
        /// </summary>
        /// 
        public int Subintervals { get { return iwork.Length; } }

        /// <summary>
        ///   Gets or sets the function to be differentiated.
        /// </summary>
        /// 
        public Func<double, double> Function { get; set; }

        /// <summary>
        ///   Gets or sets the input range under
        ///   which the integral must be computed.
        /// </summary>
        /// 
        public DoubleRange Range { get; set; }

        /// <summary>
        ///   Desired absolute accuracy. If set to zero, this parameter
        ///   will be ignored and only other requisites will be taken
        ///   into account. Default is zero.
        /// </summary>
        /// 
        public double ToleranceAbsolute
        {
            get { return abstol; }
            set { abstol = value; }
        }

        /// <summary>
        ///   Desired relative accuracy. If set to zero, this parameter
        ///   will be ignored and only other requisites will be taken
        ///   into account. Default is 1e-3.
        /// </summary>
        /// 
        public double ToleranceRelative
        {
            get { return reltol; }
            set
            {
                double limit = Math.Max(Constants.SingleEpsilon * 50.0, 5e-15);

                if (value <= limit)
                {
                    throw new ArgumentOutOfRangeException("value",
                        "Tolerance must be higher than machine precision"
                        + "(must be greater than " + limit + ")");
                }

                reltol = value;
            }
        }

        /// <summary>
        ///   Get the exit code returned in the last call to the
        ///   <see cref="INumericalIntegration.Compute()"/> method.
        /// </summary>
        /// 
        public InfiniteAdaptiveGaussKronrodStatus Status { get; private set; }

        /// <summary>
        ///   Gets the numerically computed result of the
        ///   definite integral for the specified function.
        /// </summary>
        /// 
        public double Area { get { return result; } }

        /// <summary>
        ///   Gets the integration error for the
        ///   computed <see cref="Area"/> value.
        /// </summary>
        /// 
        public double Error { get { return error; } }

        /// <summary>
        ///   Gets the number of function evaluations performed in 
        ///   the last call to the <see cref="Compute"/> method.
        /// </summary>
        /// 
        public int FunctionEvaluations { get { return evaluations; } }


        /// <summary>
        ///   Creates a new <see cref="InfiniteAdaptiveGaussKronrod"/> integration algorithm.
        /// </summary>
        /// 
        /// <param name="subintervals">Maximum number of subintervals in the 
        ///   partition of the given integration interval. Default is 100.</param>
        /// 
        public InfiniteAdaptiveGaussKronrod(int subintervals)
        {
            init(subintervals, null, Double.MinValue, Double.MaxValue);
        }

        /// <summary>
        ///   Creates a new <see cref="InfiniteAdaptiveGaussKronrod"/> integration algorithm.
        /// </summary>
        /// 
        /// <param name="subintervals">Maximum number of subintervals in the 
        ///   partition of the given integration interval. Default is 100.</param>
        /// <param name="function">The function to be integrated.</param>
        /// 
        public InfiniteAdaptiveGaussKronrod(int subintervals, Func<double, double> function)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            init(subintervals, function, Double.MinValue, Double.MaxValue);
        }

        /// <summary>
        ///   Creates a new <see cref="InfiniteAdaptiveGaussKronrod"/> integration algorithm.
        /// </summary>
        /// 
        /// <param name="subintervals">Maximum number of subintervals in the 
        ///   partition of the given integration interval. Default is 100.</param>
        /// <param name="function">The function to be integrated.</param>
        /// <param name="a">The lower limit of integration.</param>
        /// <param name="b">The upper limit of integration.</param>
        /// 
        public InfiniteAdaptiveGaussKronrod(int subintervals,
            Func<double, double> function, double a, double b)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            init(subintervals, function, a, b);
        }

        private void init(int subintervals, Func<double, double> function, double a, double b)
        {
            if (subintervals <= 0)
            {
                throw new ArgumentOutOfRangeException("subintervals",
                    "Number of subintervals must be higher than zero.");
            }

            Function = function;
            Range = new DoubleRange(a, b);

            ToleranceAbsolute = 0;
            ToleranceRelative = 1e-3;

            work = new double[subintervals * 4 + 100];
            iwork = new int[subintervals + 100];

            limit = subintervals;
            lenw = subintervals * 4;
        }

        /// <summary>
        ///   Computes the area of the function under the selected <see cref="Range"/>.
        ///   The computed value will be available at this object's <see cref="Area"/>.
        /// </summary>
        /// 
        /// <remarks>
        ///   If the integration method fails, the reason will be available at <see cref="Status"/>.
        /// </remarks>
        /// 
        /// <returns>
        ///   True if the integration method succeeds, false otherwise.
        /// </returns>
        /// 
        public bool Compute()
        {
            Status = InfiniteAdaptiveGaussKronrodStatus.Success;

            int errorCode;

            double a = this.Range.Min;
            double b = this.Range.Max;

            if (!Double.IsInfinity(a) && !Double.IsInfinity(b))
            {
                NonAdaptiveGaussKronrod.qng_(Function, a, b, ToleranceAbsolute, ToleranceRelative,
                        out result, out error, out evaluations, out errorCode);
            }

            else
            {
                double bound = 0;
                int inf;
                int last;

                if (Double.IsInfinity(a) && Double.IsInfinity(b))
                {
                    inf = 2;
                }
                else if (Double.IsInfinity(a))
                {
                    bound = b;
                    inf = -1;
                }
                else // if (Double.IsInfinity(b))
                {
                    bound = a;
                    inf = 1;
                }

                qagi_(Function, bound, inf, ToleranceAbsolute, ToleranceRelative,
                    out result, out error, out evaluations, out errorCode,
                    limit, lenw, out last, iwork, work);
            }


            if (errorCode == 6)
            {
                throw new InvalidOperationException("Invalid inputs. If this error happens, the "
                    + "framework didn't check for inputs correctly. If you encounter this error, "
                    + "please feel in a bug report at the framework's issue tracking system.");
            }

            Status = (InfiniteAdaptiveGaussKronrodStatus)errorCode;

            return Status == InfiniteAdaptiveGaussKronrodStatus.Success;
        }

        /// <summary>
        ///   Computes the area under the integral for the given function, in the given 
        ///   integration interval, using the Infinite Adaptive Gauss Kronrod algorithm.
        /// </summary>
        /// 
        /// <param name="f">The unidimensional function whose integral should be computed.</param>
        /// 
        /// <returns>The integral's value in the current interval.</returns>
        /// 
        public static double Integrate(Func<double, double> f)
        {
            var iagk = new InfiniteAdaptiveGaussKronrod(100, f);

            iagk.Compute();

            return iagk.Area;
        }

        /// <summary>
        ///   Computes the area under the integral for the given function, in the given 
        ///   integration interval, using the Infinite Adaptive Gauss Kronrod algorithm.
        /// </summary>
        /// 
        /// <param name="f">The unidimensional function whose integral should be computed.</param>
        /// <param name="a">The beginning of the integration interval.</param>
        /// <param name="b">The ending of the integration interval.</param>
        /// 
        /// <returns>The integral's value in the current interval.</returns>
        /// 
        public static double Integrate(Func<double, double> f, double a, double b)
        {
            var iagk = new InfiniteAdaptiveGaussKronrod(1000, f, a, b);

            iagk.Compute();

            return iagk.Area;
        }

        /// <summary>
        ///   Computes the area under the integral for the given function, in the given 
        ///   integration interval, using the Infinite Adaptive Gauss Kronrod algorithm.
        /// </summary>
        /// 
        /// <param name="f">The unidimensional function whose integral should be computed.</param>
        /// <param name="a">The beginning of the integration interval.</param>
        /// <param name="b">The ending of the integration interval.</param>
        /// <param name="tolerance">
        ///   The relative tolerance under which the solution has to be found. Default is 1e-3.</param>
        /// 
        /// <returns>The integral's value in the current interval.</returns>
        /// 
        public static double Integrate(Func<double, double> f, double a, double b, double tolerance)
        {
            var iagk = new InfiniteAdaptiveGaussKronrod(100, f, a, b)
            {
                ToleranceRelative = tolerance
            };

            iagk.Compute();

            return iagk.Area;
        }




        #region Quadpack

        static int qagi_(Func<double, double> f, double bound, int inf, double epsabs,
            double epsrel, out double result, out double abserr, out int neval,
            out int ier, int limit, int lenw, out int last, int[] iwork,
            double[] dwork)
        {
            int l1, l2, l3;

            /*         check validity o limit and lenw. */

            /* ***first executable statement  qagi */
            /* Parameter adjustments */

            /* Function Body */
            ier = 6;
            neval = 0;
            last = 0;
            result = 0.0;
            abserr = 0.0;

            if (limit < 1 || lenw < (limit << 2))
            {
                goto L10;
            }

            unsafe
            {
                /*         preparef call for qagie. */
                fixed (double* work = &dwork[10])
                fixed (int* Iwork = &iwork[10])
                {
                    l1 = limit + 1;
                    l2 = limit + l1;
                    l3 = limit + l2;

                    qagie_(f, ref bound, ref inf, ref epsabs, ref epsrel, ref limit, ref result,
                        ref abserr, ref neval, ref ier, &work[1], &work[l1], &work[l2],
                        &work[l3], &Iwork[1], ref last);

                    /*         call error handler i necessary. */
                }
            }

            // lvl = 0;
        L10:
            if (ier == 6)
            {
                // lvl = 1;
            }

            return 0;
        }

        static unsafe int qagie_(Func<double, double> f, ref double bound, ref int inf, ref double epsabs,
            ref double epsrel, ref int limit, ref double result, ref double abserr,
            ref int neval, ref int ier, double* alist__, double* blist,
            double* rlist, double* elist, int* iord, ref int last)
        {
            /* System generated locals */
            int i__1, i__2;

            /* Local variables */
            int k;
            double a1, a2, b1, b2;
            int id;
            double area;

            double dres;
            int ksgn;
            double boun;
            int nres;
            double area1 = 0;
            double area2 = 0;
            double area12;
            double small = 0, erro12;
            int ierro;
            double defab1 = 0;
            double defab2 = 0;
            int ktmin, nrmax;
            double oflow, uflow;
            bool noext;

            int iroff1, iroff2, iroff3;

            double[] res3la_ = new double[3 + 1];
            double[] rlist2_ = new double[52 + 1];

            fixed (double* res3la = res3la_)
            fixed (double* rlist2 = rlist2_)
            {
                double error1 = 0;
                double error2 = 0;

                int numrl2;
                double defabs = 0, epmach;
                double erlarg = 1;
                double abseps = 0;
                double correc = 0;
                double errbnd;
                double resabs = 0;
                int jupbnd;
                double erlast, errmax;
                int maxerr;
                double reseps = 0;
                bool extrap;
                double ertest = 0;
                double errsum;

                /* Parameter adjustments */
                --iord;
                --elist;
                --rlist;
                --blist;
                --alist__;

                /* Function Body */
                epmach = Constants.SingleEpsilon;

                /*           test on validity o parameters */
                /*           ----------------------------- */

                /* ***first executable statement  qagie */
                ier = 0;
                neval = 0;
                last = 0;
                result = 0.0;
                abserr = 0.0;
                alist__[1] = 0.0;
                blist[1] = 1.0;
                rlist[1] = 0.0;
                elist[1] = 0.0;
                iord[1] = 0;

                if (epsabs <= 0.0 && epsrel < Math.Max(epmach * 50.0, 5e-15))
                {
                    ier = 6;
                }

                if (ier == 6)
                {
                    return 0;
                }


                /*           first approximation to the integral */
                /*           ----------------------------------- */

                /*           determine the interval to be mapped onto (0,1). */
                /*           i in = 2 the integral is computed as i = i1+i2, wheref */
                /*           i1 = integral o  over (-infinity,0), */
                /*           i2 = integral o  over (0,+infinity). */

                boun = bound;

                if (inf == 2)
                {
                    boun = 0.0;
                }

                qk15i_(f, ref boun, ref inf, 0.0, 1.0,
                    ref result, ref abserr, ref defabs, ref resabs);

                /*           test on accuracy */

                last = 1;
                rlist[1] = result;
                elist[1] = abserr;
                iord[1] = 1;
                dres = Math.Abs(result);

                errbnd = Math.Max(epsabs, epsrel * dres);
                if (abserr <= epmach * 100.0 * defabs && abserr > errbnd)
                {
                    ier = 2;
                }
                if (limit == 1)
                {
                    ier = 1;
                }
                if (ier != 0 || abserr <= errbnd && abserr != resabs || abserr == 0.0)
                {
                    goto L130;
                }

                /*           initialization */
                /*           -------------- */

                uflow = Single.MinValue;
                oflow = Single.MaxValue;
                rlist2[0] = result;
                errmax = abserr;
                maxerr = 1;
                area = result;
                errsum = abserr;
                abserr = oflow;
                nrmax = 1;
                nres = 0;
                ktmin = 0;
                numrl2 = 2;
                extrap = false;
                noext = false;
                ierro = 0;
                iroff1 = 0;
                iroff2 = 0;
                iroff3 = 0;
                ksgn = -1;
                if (dres >= (1.0 - epmach * 50.0) * defabs)
                {
                    ksgn = 1;
                }

                /*           main do-loop */
                /*           ------------ */

                i__1 = limit;
                for (last = 2; last <= i__1; ++last)
                {

                    /*           bisect the subinterval with nrmax-th largest */
                    /*           error estimate. */

                    a1 = alist__[maxerr];
                    b1 = (alist__[maxerr] + blist[maxerr]) * .5;
                    a2 = b1;
                    b2 = blist[maxerr];
                    erlast = errmax;

                    qk15i_(f, ref boun, ref inf, a1, b1, ref area1,
                        ref error1, ref resabs, ref defab1);

                    qk15i_(f, ref boun, ref inf, a2, b2, ref area2,
                        ref error2, ref resabs, ref defab2);

                    /*           improve previous approximations to integral */
                    /*           and error and test for accuracy. */

                    area12 = area1 + area2;
                    erro12 = error1 + error2;
                    errsum = errsum + erro12 - errmax;
                    area = area + area12 - rlist[maxerr];
                    if (defab1 == error1 || defab2 == error2)
                    {
                        goto L15;
                    }
                    if ((Math.Abs(rlist[maxerr] - area12)) > Math.Abs(area12) * 1e-5 || erro12 < errmax * 0.99)
                    {
                        goto L10;
                    }
                    if (extrap)
                    {
                        ++iroff2;
                    }
                    if (!extrap)
                    {
                        ++iroff1;
                    }
                L10:
                    if (last > 10 && erro12 > errmax)
                    {
                        ++iroff3;
                    }
                L15:
                    rlist[maxerr] = area1;
                    rlist[last] = area2;
                    errbnd = Math.Max(epsabs, epsrel * Math.Abs(area));

                    /*           test for roundof error and eventually */
                    /*           set error flag. */

                    if (iroff1 + iroff2 >= 10 || iroff3 >= 20)
                    {
                        ier = 2;
                    }
                    if (iroff2 >= 5)
                    {
                        ierro = 3;
                    }

                    /*           set error flag in the case that the number o */
                    /*           subintervals equals limit. */

                    if (last == limit)
                    {
                        ier = 1;
                    }

                    /*           set error flag in the case o bad integrand behaviour */
                    /*           at some points o the integration range. */

                    double alpha = Math.Max(Math.Abs(a1), Math.Abs(b2));
                    double beta = (epmach * 100.0 + 1.0) * (Math.Abs(a2) + uflow * 1e3);
                    if (alpha <= beta)
                    {
                        ier = 4;
                    }

                    /*           append the newly-created intervals to the list. */

                    if (error2 > error1)
                    {
                        goto L20;
                    }
                    alist__[last] = a2;
                    blist[maxerr] = b1;
                    blist[last] = b2;
                    elist[maxerr] = error1;
                    elist[last] = error2;
                    goto L30;
                L20:
                    alist__[maxerr] = a2;
                    alist__[last] = a1;
                    blist[last] = b1;
                    rlist[maxerr] = area2;
                    rlist[last] = area1;
                    elist[maxerr] = error2;
                    elist[last] = error1;

                /*           call subroutine qpsrt to maintain the descending ordering */
                /*           in the list o error estimates and select the */
                /*           subinterval with nrmax-th largest error estimate (to be */
                /*           bisected next). */

            L30:
                    qpsrt_(ref limit, ref last, ref maxerr, ref errmax,
                        &elist[1], &iord[1], ref nrmax);

                    if (errsum <= errbnd)
                    {
                        goto L115;
                    }
                    if (ier != 0)
                    {
                        goto L100;
                    }
                    if (last == 2)
                    {
                        goto L80;
                    }
                    if (noext)
                    {
                        goto L90;
                    }

                    erlarg -= erlast;

                    if (Math.Abs(b1 - a1) > small)
                    {
                        erlarg += erro12;
                    }
                    if (extrap)
                    {
                        goto L40;
                    }

                    /*           test whether the interval to be bisected next is the */
                    /*           smallest interval. */

                    if (Math.Abs(blist[maxerr] - alist__[maxerr]) > small)
                    {
                        goto L90;
                    }
                    extrap = true;
                    nrmax = 2;
                L40:
                    if (ierro == 3 || erlarg <= ertest)
                    {
                        goto L60;
                    }

                    /*           the smallest interval has the largest error. */
                    /*           beforef bisecting decrease the sum o the errors */
                    /*           over the larger intervals (erlarg) and perform */
                    /*           extrapolation. */

                    id = nrmax;
                    jupbnd = last;
                    if (last > limit / 2 + 2)
                    {
                        jupbnd = limit + 3 - last;
                    }
                    i__2 = jupbnd;
                    for (k = id; k <= i__2; ++k)
                    {
                        maxerr = iord[nrmax];
                        errmax = elist[maxerr];
                        if ((Math.Abs(blist[maxerr] - alist__[maxerr])) > small)
                        {
                            goto L90;
                        }
                        ++nrmax;
                        /* L50: */
                    }

            /*           perform extrapolation. */

            L60:
                    ++numrl2;

                    Accord.Diagnostics.Debug.Assert(numrl2 > 1);

                    rlist2[numrl2 - 1] = area;
                    qelg_(ref numrl2, rlist2, ref reseps, ref abseps, res3la, ref nres);

                    ++ktmin;

                    if (ktmin > 5 && abserr < errsum * .001)
                    {
                        ier = 5;
                    }
                    if (abseps >= abserr)
                    {
                        goto L70;
                    }

                    ktmin = 0;
                    abserr = abseps;
                    result = reseps;
                    correc = erlarg;

                    ertest = Math.Max(epsabs, epsrel * Math.Abs(reseps));

                    if (abserr <= ertest)
                    {
                        goto L100;
                    }

            /*            preparef bisection o the smallest interval. */

            L70:
                    if (numrl2 == 1)
                    {
                        noext = true;
                    }
                    if (ier == 5)
                    {
                        goto L100;
                    }
                    maxerr = iord[1];
                    errmax = elist[maxerr];
                    nrmax = 1;
                    extrap = false;
                    small *= .5;
                    erlarg = errsum;
                    goto L90;
                L80:
                    small = .375;
                    erlarg = errsum;
                    ertest = errbnd;
                    rlist2[1] = area;
                L90:
                    ;
                }

        /*           set final result and error estimate. */
            /*           ------------------------------------ */

        L100:
                if (abserr == oflow)
                {
                    goto L115;
                }
                if (ier + ierro == 0)
                {
                    goto L110;
                }
                if (ierro == 3)
                {
                    abserr += correc;
                }
                if (ier == 0)
                {
                    ier = 3;
                }
                if (result != 0.0 && area != 0.0)
                {
                    goto L105;
                }
                if (abserr > errsum)
                {
                    goto L115;
                }
                if (area == 0.0)
                {
                    goto L130;
                }
                goto L110;
            L105:
                if (abserr / Math.Abs(result) > errsum / Math.Abs(area))
                {
                    goto L115;
                }

        /*           test on divergence */

        L110:
                /* Computing MAX */
                if (ksgn == -1 && Math.Max(Math.Abs(result), Math.Abs(area)) <= defabs * 0.01)
                {
                    goto L130;
                }
                if (0.01 > result / area || result / area > 100.0 || errsum > Math.Abs(area)
                    )
                {
                    ier = 6;
                }
                goto L130;

        /*           compute global integral sum. */

        L115:
                result = 0.0;
                for (k = 1; k <= last; ++k)
                {
                    result += rlist[k];
                    /* L120: */
                }
                abserr = errsum;
            L130:
                neval = last * 30 - 15;
                if (inf == 2)
                {
                    neval <<= 1;
                }
                if (ier > 2)
                {
                    --(ier);
                }
            }

            return 0;
        }

        static unsafe int qelg_(ref int n, double* epstab, ref double result,
            ref double abserr, double* res3la, ref int nres)
        {
            /* System generated locals */
            int i__1;

            /* Local variables */
            int i__;
            double e0, e1, e2, e3;
            int k1, k2, k3, ib, ie;
            double ss;
            int ib2;
            double res;
            int num;
            double err1, err2, err3, tol1, tol2, tol3;
            int indx;
            double e1abs, oflow, error, delta1, delta2, delta3;
            double epmach, epsin;
            int newelm, limexp;

            /* ***first executable statement  qelg */
            /* Parameter adjustments */
            --res3la;
            --epstab;

            /* Function Body */
            epmach = Constants.SingleEpsilon;
            oflow = Single.MaxValue;
            ++(nres);
            abserr = oflow;

            Accord.Diagnostics.Debug.Assert(n > 0 && n < 52);

            result = epstab[n];
            if (n < 3)
            {
                goto L100;
            }
            limexp = 50;

            Accord.Diagnostics.Debug.Assert(n > 0 && n < 52);
            Accord.Diagnostics.Debug.Assert(n + 2 > 0 && n + 2 < 52);

            epstab[n + 2] = epstab[n];
            newelm = (n - 1) / 2;
            epstab[n] = oflow;
            num = n;
            k1 = n;
            for (i__ = 1; i__ <= newelm; ++i__)
            {
                k2 = k1 - 1;
                k3 = k1 - 2;
                res = epstab[k1 + 2];

                Accord.Diagnostics.Debug.Assert(k3 > 0 && k3 < 52);
                Accord.Diagnostics.Debug.Assert(k2 > 0 && k2 < 52);

                e0 = epstab[k3];
                e1 = epstab[k2];
                e2 = res;
                e1abs = Math.Abs(e1);
                delta2 = e2 - e1;
                err2 = Math.Abs(delta2);
                tol2 = Math.Max(Math.Abs(e2), e1abs) * epmach;
                delta3 = e1 - e0;
                err3 = Math.Abs(delta3);
                tol3 = Math.Max(e1abs, Math.Abs(e0)) * epmach;
                if (err2 > tol2 || err3 > tol3)
                {
                    goto L10;
                }

                /*           i e0, e1 and e2 aref equal to within machine */
                /*           accuracy, convergence is assumed. */
                /*           result = e2 */
                /*           abserr = abs(e1-e0)+abs(e2-e1) */

                result = res;
                abserr = err2 + err3;
                /* ***jump out o do-loop */
                goto L100;
            L10:

                Accord.Diagnostics.Debug.Assert(k1 > 0 && k1 < 52);

                e3 = epstab[k1];
                epstab[k1] = e1;
                delta1 = e1 - e3;
                err1 = Math.Abs(delta1);
                tol1 = Math.Max(e1abs, Math.Abs(e3)) * epmach;

                /*           i two elements aref very close to each other, omit */
                /*           a part o the table by adjusting the value o n */

                if (err1 <= tol1 || err2 <= tol2 || err3 <= tol3)
                {
                    goto L20;
                }
                ss = 1.0 / delta1 + 1.0 / delta2 - 1.0 / delta3;
                epsin = (Math.Abs(ss * e1));

                /*           test to detect irregular behaviour in the table, and */
                /*           eventually omit a part o the table adjusting the value */
                /*           o n. */

                if (epsin > 1e-4)
                {
                    goto L30;
                }
            L20:
                n = i__ + i__ - 1;
                /* ***jump out o do-loop */
                goto L50;

                /*           compute a new element and eventually adjust */
            /*           the value o result. */

                L30:
                res = e1 + 1.0 / ss;

                Accord.Diagnostics.Debug.Assert(k1 > 0 && k1 < 52);

                epstab[k1] = res;
                k1 += -2;
                error = err2 + Math.Abs(res - e2) + err3;
                if (error > abserr)
                {
                    goto L40;
                }
                abserr = error;
                result = res;
            L40:
                ;
            }

            /*           shift the table. */

            L50:
            if (n == limexp)
            {
                n = (limexp / 2 << 1) - 1;
            }
            ib = 1;
            if (num / 2 << 1 == num)
            {
                ib = 2;
            }
            ie = newelm + 1;
            i__1 = ie;
            for (i__ = 1; i__ <= i__1; ++i__)
            {
                ib2 = ib + 2;

                Accord.Diagnostics.Debug.Assert(ib > 0 && ib < 52);
                Accord.Diagnostics.Debug.Assert(ib2 > 0 && ib2 < 52);

                epstab[ib] = epstab[ib2];
                ib = ib2;
                /* L60: */
            }
            if (num == n)
            {
                goto L80;
            }
            indx = num - n + 1;
            for (i__ = 1; i__ <= n; ++i__)
            {
                Accord.Diagnostics.Debug.Assert(i__ > 0 && i__ < 52);
                Accord.Diagnostics.Debug.Assert(indx > 0 && indx < 52);

                epstab[i__] = epstab[indx];
                ++indx;
                /* L70: */
            }
        L80:
            if (nres >= 4)
            {
                goto L90;
            }

            Accord.Diagnostics.Debug.Assert(nres > 0 && nres < 4);

            res3la[nres] = result;
            abserr = oflow;
            goto L100;

            /*           compute error estimate */

            L90:
            abserr = Math.Abs(result - res3la[3]) + Math.Abs(result -
                res3la[2]) + Math.Abs(result - res3la[1]);
            res3la[1] = res3la[2];
            res3la[2] = res3la[3];
            res3la[3] = result;
        L100:
            abserr = Math.Max(abserr, epmach * 5.0 * Math.Abs(result));
            return 0;
        } /* qelg_ */

        static unsafe int qk15i_(Func<double, double> f, ref double boun, ref int inf, double a,
            double b, ref double result, ref double abserr, ref double resabs, ref double resasc)
        {
            /* Initialized data */

            double[] xgk = 
            { 
                0.9914553711208126, 0.9491079123427585,
	            0.8648644233597691, 0.7415311855993944, 0.5860872354676911,
	            0.4058451513773972, 0.2077849550078985, 0.0 
            };

            double[] wgk = 
            {
                0.02293532201052922, 0.06309209262997855,
	            0.1047900103222502,  0.1406532597155259,  0.1690047266392679,
	            0.1903505780647854,  0.2044329400752989,  0.2094821410847278 
            };

            double[] wg = 
            { 
                0.0000000000000000, 0.1294849661688697, 0.0000000000000000,
                0.2797053914892767, 0.0000000000000000, 0.3818300505051189, 
                0.0000000000000000, 0.4179591836734694 
            };

            /* Local variables */
            int j;
            double fc;
            double[] fv1_ = new double[7 + 1];
            double[] fv2_ = new double[7 + 1];

            fixed (double* fv1 = fv1_)
            fixed (double* fv2 = fv2_)
            {
                double absc, din, resg, resk, fsum, absc1;
                double absc2, fval1, fval2, hlgth, centr, reskh, uflow;
                double tabsc1, tabsc2, epmach;


                /* ***first executable statement  qk15i */
                epmach = Constants.SingleEpsilon;
                uflow = Single.MinValue;
                din = Math.Min(1, inf);

                centr = (a + b) * .5;
                hlgth = (b - a) * .5;
                tabsc1 = boun + din * (1.0 - centr) / centr;
                fval1 = f(tabsc1);

                if (inf == 2)
                    fval1 += f(-tabsc1);

                fc = fval1 / centr / centr;

                /*           compute the 15-point kronrod approximation to */
                /*           the integral, and estimate the error. */

                resg = wg[7] * fc;
                resk = wgk[7] * fc;
                resabs = Math.Abs(resk);
                for (j = 1; j <= 7; ++j)
                {
                    absc = hlgth * xgk[j - 1];
                    absc1 = centr - absc;
                    absc2 = centr + absc;
                    tabsc1 = boun + din * (1.0 - absc1) / absc1;
                    tabsc2 = boun + din * (1.0 - absc2) / absc2;
                    fval1 = f(tabsc1);
                    fval2 = f(tabsc2);
                    if (inf == 2)
                    {
                        fval1 += f(-tabsc1);
                    }
                    if (inf == 2)
                    {
                        fval2 += f(-tabsc2);
                    }
                    fval1 = fval1 / absc1 / absc1;
                    fval2 = fval2 / absc2 / absc2;

                    Accord.Diagnostics.Debug.Assert(j > 0);

                    fv1[j - 1] = fval1;
                    fv2[j - 1] = fval2;
                    fsum = fval1 + fval2;
                    resg += wg[j - 1] * fsum;
                    resk += wgk[j - 1] * fsum;
                    resabs += wgk[j - 1] * (Math.Abs(fval1) + Math.Abs(fval2));
                    /* L10: */
                }
                reskh = resk * .5;
                resasc = wgk[7] * Math.Abs(fc - reskh);
                for (j = 1; j <= 7; ++j)
                {
                    Accord.Diagnostics.Debug.Assert(j > 0);

                    resasc += wgk[j - 1] * (Math.Abs(fv1[j - 1] - reskh)
                        + Math.Abs(fv2[j - 1] - reskh));
                    /* L20: */
                }
                result = resk * hlgth;
                resasc *= hlgth;
                resabs *= hlgth;
                abserr = Math.Abs((resk - resg) * hlgth);

                if (resasc != 0.0 && abserr != 0.0)
                {
                    abserr = resasc * Math.Min(1.0, Math.Pow((abserr * 200.0 / resasc), 1.5));
                }

                if (resabs > uflow / (epmach * 50.0))
                {
                    abserr = Math.Max(epmach * 50.0 * resabs, abserr);
                }
            }

            return 0;
        }

        static unsafe int qpsrt_(ref int limit, ref int last, ref int maxerr,
            ref double ermax, double* elist, int* iord, ref int nrmax)
        {
            /* System generated locals */
            int i__1;

            /* Local variables */
            int i__, j, k, ido, ibeg, jbnd, isucc, jupbn;
            double errmin, errmax;


            /* Function Body */
            if (last > 2)
            {
                goto L10;
            }
            iord[1] = 1;
            iord[2] = 2;
            goto L90;

            /*           this part o the routine is only executed */
        /*           i, due to a difficult integrand, subdivision */
        /*           increased the error estimate. in the normal case */
        /*           the insert proceduref should start after the */
        /*           nrmax-th largest error estimate. */

            L10:
            errmax = elist[maxerr];
            if (nrmax == 1)
            {
                goto L30;
            }
            ido = nrmax - 1;
            i__1 = ido;
            for (i__ = 1; i__ <= i__1; ++i__)
            {
                isucc = iord[nrmax - 1];
                /* ***jump out o do-loop */
                if (errmax <= elist[isucc])
                {
                    goto L30;
                }
                iord[nrmax] = isucc;
                --(nrmax);
                /* L20: */
            }

            /*           compute the number o elements in the list to */
        /*           be maintained in descending order. this number */
        /*           depends on the number o subdivisions still */
        /*           allowed. */

            L30:
            jupbn = last;
            if (last > limit / 2 + 2)
            {
                jupbn = limit + 3 - last;
            }
            errmin = elist[last];

            /*           insert errmax by traversing the list top-down, */
            /*           starting comparison from the element elist(iord(nrmax+1)). */

            jbnd = jupbn - 1;
            ibeg = nrmax + 1;
            if (ibeg > jbnd)
            {
                goto L50;
            }

            for (i__ = ibeg; i__ <= jbnd; ++i__)
            {
                isucc = iord[i__];
                /* ***jump out o do-loop */
                if (errmax >= elist[isucc])
                {
                    goto L60;
                }
                iord[i__ - 1] = isucc;
                /* L40: */
            }
        L50:
            iord[jbnd] = maxerr;
            iord[jupbn] = last;
            goto L90;

            /*           insert errmin by traversing the list bottom-up. */

            L60:
            iord[i__ - 1] = maxerr;
            k = jbnd;
            i__1 = jbnd;
            for (j = i__; j <= i__1; ++j)
            {
                isucc = iord[k];
                /* ***jump out o do-loop */
                if (errmin < elist[isucc])
                {
                    goto L80;
                }
                iord[k + 1] = isucc;
                --k;
                /* L70: */
            }
            iord[i__] = last;
            goto L90;
        L80:
            iord[k + 1] = last;

            /*           set maxerr and ermax. */

            L90:
            maxerr = iord[nrmax];
            ermax = elist[maxerr];
            return 0;
        } /* qpsrt_ */


        #endregion

        private InfiniteAdaptiveGaussKronrod()
        {
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public object Clone()
        {
            var clone = new InfiniteAdaptiveGaussKronrod();

            clone.evaluations = evaluations;
            clone.iwork = (int[])iwork.Clone();
            clone.work = (double[])work.Clone();
            clone.error = error;
            clone.result = result;
            clone.ToleranceAbsolute = ToleranceAbsolute;
            clone.ToleranceRelative = ToleranceRelative;

            return clone;
        }
    }
}
