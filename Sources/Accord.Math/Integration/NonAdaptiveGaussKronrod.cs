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

namespace Accord.Math.Integration
{
    using System;
    using System.Diagnostics;
    using Accord.Compat;

    /// <summary>
    ///   Status codes for the <see cref="NonAdaptiveGaussKronrod"/>
    ///   integration method.
    /// </summary>
    /// 
    public enum NonAdaptiveGaussKronrodStatus
    {
        /// <summary>
        ///   The integration calculation has been completed with success.
        ///   The obtained result is under the selected convergence criteria.
        /// </summary>
        /// 
        Success,

        /// <summary>
        ///   Maximum number of steps has been reached.
        /// </summary>
        /// 
        /// <remarks>
        ///   The maximum number of steps has been executed. The integral
        ///   is probably too difficult to be calculated by dqng.
        /// </remarks>
        /// 
        MaximumSteps = 1,

    }

    /// <summary>
    ///   Non-Adaptive Gauss-Kronrod integration method. 
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The algorithm implemented by this class has been based on the original FORTRAN 
    ///   implementation from QUADPACK. The function implemented the Non-adaptive Gauss-
    ///   Kronrod integration is <c>qng(f,a,b,epsabs,epsrel,result,abserr,neval,ier)</c>.
    ///   The original source code is in the public domain, but this version is under the
    ///   LGPL. The original authors, as long as the original routine description, are 
    ///   listed below:</para>
    /// <para>
    ///   Robert Piessens, Elise de Doncker; Applied Mathematics and Programming Division,
    ///   K.U.Leuven, Leuvenappl. This routine calculates an approximation result to a given
    ///   definite integral i = integral of f over (a,b), hopefully satisfying following claim
    ///   for accuracy abs(i-result).le.max(epsabs,epsrel*abs(i)).</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/QUADPACK">
    ///       Wikipedia, The Free Encyclopedia. QUADPACK. Available on: 
    ///       http://en.wikipedia.org/wiki/QUADPACK </a></description></item>
    ///     <item><description><a href="http://www.netlib.no/netlib/quadpack/qng.f">
    ///       Robert Piessens, Elise de Doncker; Non-adaptive integration standard fortran 
    ///       subroutine (qng.f). Applied Mathematics and Programming Division, K.U.Leuven,
    ///       Leuvenappl. Available at: http://www.netlib.no/netlib/quadpack/qng.f </a>
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
    public class NonAdaptiveGaussKronrod : IUnivariateIntegration,
        INumericalIntegration<NonAdaptiveGaussKronrodStatus>
    {
        private double result;
        private double error;
        private int evaluations;

        private DoubleRange range;

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
        public DoubleRange Range
        {
            get { return range; }
            set
            {
                if (Double.IsInfinity(range.Min) || Double.IsNaN(range.Min))
                    throw new ArgumentOutOfRangeException("value", "Minimum is out of range.");

                if (Double.IsInfinity(range.Max) || Double.IsNaN(range.Max))
                    throw new ArgumentOutOfRangeException("value", "Maximum is out of range.");

                range = value;
            }
        }

        /// <summary>
        ///   Desired absolute accuracy. If set to zero, this parameter
        ///   will be ignored and only other requisites will be taken
        ///   into account. Default is zero.
        /// </summary>
        /// 
        public double ToleranceAbsolute { get; set; }

        /// <summary>
        ///   Desired relative accuracy. If set to zero, this parameter
        ///   will be ignored and only other requisites will be taken
        ///   into account. Default is 1e-3.
        /// </summary>
        /// 
        public double ToleranceRelative { get; set; }

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
        ///   Get the exit code returned in the last call to the
        ///   <see cref="INumericalIntegration.Compute()"/> method.
        /// </summary>
        /// 
        public NonAdaptiveGaussKronrodStatus Status { get; private set; }

        /// <summary>
        ///   Gets the number of function evaluations performed in 
        ///   the last call to the <see cref="Compute"/> method.
        /// </summary>
        /// 
        public int FunctionEvaluations { get { return evaluations; } }


        /// <summary>
        ///   Creates a new <see cref="NonAdaptiveGaussKronrod"/> integration algorithm.
        /// </summary>
        /// 
        public NonAdaptiveGaussKronrod()
        {
            ToleranceAbsolute = 0;
            ToleranceRelative = 1e-3;
            range = new DoubleRange(0, 1);
        }

        /// <summary>
        ///   Creates a new <see cref="NonAdaptiveGaussKronrod"/> integration algorithm.
        /// </summary>
        /// 
        /// <param name="function">The function to be integrated.</param>
        /// 
        public NonAdaptiveGaussKronrod(Func<double, double> function)
            : this()
        {
            if (function == null)
                throw new ArgumentNullException("function");

            ToleranceAbsolute = 0;
            ToleranceRelative = 1e-3;
        }

        /// <summary>
        ///   Creates a new <see cref="NonAdaptiveGaussKronrod"/> integration algorithm.
        /// </summary>
        /// 
        /// <param name="function">The function to be integrated.</param>
        /// <param name="a">The lower limit of integration.</param>
        /// <param name="b">The upper limit of integration.</param>
        /// 
        public NonAdaptiveGaussKronrod(Func<double, double> function, double a, double b)
            : this(function)
        {
            if (Double.IsInfinity(a) || Double.IsNaN(a))
                throw new ArgumentOutOfRangeException("a");

            if (Double.IsInfinity(b) || Double.IsNaN(b))
                throw new ArgumentOutOfRangeException("b");

            range = new DoubleRange(a, b);
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
            int errorCode;

            qng_(Function, Range.Min, Range.Max, ToleranceAbsolute, ToleranceRelative,
                out result, out error, out evaluations, out errorCode);

            Status = (NonAdaptiveGaussKronrodStatus)errorCode;

            return Status == NonAdaptiveGaussKronrodStatus.Success;
        }


        /// <summary>
        ///   Computes the area under the integral for the given function, 
        ///   in the given integration interval, using Gauss-Kronrod method.
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
            return Integrate(f, a, b, 1e-3);
        }

        /// <summary>
        ///   Computes the area under the integral for the given function, in the given 
        ///   integration interval, using the Non-Adaptive Gauss Kronrod algorithm.
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
            double result;
            double error;
            int evaluations;
            int errorCode;

            qng_(f, a, b, 0, tolerance, out result, out error, out evaluations, out errorCode);

            return result;
        }





        #region Quadpack

        internal static int qng_(Func<double, double> f, double a, double b,
            double epsabs, double epsrel, out double result, out double abserr,
            out int neval, out int ier)
        {

            /* Local variables */
            int k, l;
            double[] fv1 = new double[5];
            double[] fv2 = new double[5];
            double[] fv3 = new double[5];
            double[] fv4 = new double[5];

            int ipx = 0;
            double absc, fval, res10;
            double res21 = 0, res43 = 0, res87;
            double fval1, fval2, hlgth;
            double centr, reskh;

            double dhlgth, resabs = 0, resasc = 0, fcentr;
            double[] savfun = new double[21];

            /*           machine dependent constants               */
            /*           ---------------------------               */
            /*           epmach is the largest relative spacing.   */
            /*           uflow is the smallest positive magnitude. */

            double epmach = Constants.DoubleEpsilon;
            double uflow = Double.Epsilon;

            /*           test on validity of parameters */
            /*           ------------------------------ */

            result = 0.0f;
            abserr = 0.0f;
            neval = 0;
            ier = 6;

            if (epsabs <= 0.0f && epsrel < Math.Max(5e-15, epmach * 50f))
            {
                goto L80;
            }

            hlgth = (b - a) * .5f;
            dhlgth = Math.Abs(hlgth);

            centr = (b + a) * .5f;
            fcentr = f(centr);
            neval = 21;
            ier = 1;

            /*          compute the integral using the 10- and 21-point formula. */

            for (l = 1; l <= 3; ++l)
            {
                switch (l)
                {
                    case 1: goto L5;
                    case 2: goto L25;
                    case 3: goto L45;
                }

            L5:
                res10 = 0.0f;
                res21 = w21b[5] * fcentr;
                resabs = w21b[5] * Math.Abs(fcentr);
                for (k = 1; k <= 5; ++k)
                {
                    absc = hlgth * x1[k - 1];
                    fval1 = f(centr + absc);
                    fval2 = f(centr - absc);
                    fval = fval1 + fval2;
                    res10 += w10[k - 1] * fval;
                    res21 += w21a[k - 1] * fval;
                    resabs += w21a[k - 1] * (Math.Abs(fval1) + Math.Abs(fval2));
                    savfun[k - 1] = fval;
                    fv1[k - 1] = fval1;
                    fv2[k - 1] = fval2;
                }
                ipx = 5;
                for (k = 1; k <= 5; ++k)
                {
                    ++ipx;
                    absc = hlgth * x2[k - 1];
                    fval1 = f(centr + absc);
                    fval2 = f(centr - absc);
                    fval = fval1 + fval2;
                    res21 += w21b[k - 1] * fval;
                    resabs += w21b[k - 1] * (Math.Abs(fval1) + Math.Abs(fval2));
                    savfun[ipx - 1] = fval;
                    fv3[k - 1] = fval1;
                    fv4[k - 1] = fval2;
                }

                /*          test for convergence. */

                result = res21 * hlgth;
                resabs *= dhlgth;
                reskh = res21 * .5f;
                resasc = w21b[5] * Math.Abs(fcentr - reskh);

                for (k = 1; k <= 5; ++k)
                {
                    resasc = resasc
                        + w21a[k - 1] * ((Math.Abs(fv1[k - 1] - reskh)) + (Math.Abs(fv2[k - 1] - reskh)))
                        + w21b[k - 1] * ((Math.Abs(fv3[k - 1] - reskh)) + (Math.Abs(fv4[k - 1] - reskh)));
                }
                abserr = Math.Abs((res21 - res10) * hlgth);
                resasc *= dhlgth;
                goto L65;

                /*          compute the integral using the 43-point formula. */

                L25:
                res43 = w43b[11] * fcentr;
                neval = 43;

                for (k = 1; k <= 10; ++k)
                {
                    res43 += savfun[k - 1] * w43a[k - 1];
                }

                for (k = 1; k <= 11; ++k)
                {
                    ++ipx;
                    absc = hlgth * x3[k - 1];
                    fval = f(absc + centr) + f(centr - absc);
                    res43 += fval * w43b[k - 1];
                    savfun[ipx - 1] = fval;
                }

                /*          test for convergence. */

                result = res43 * hlgth;
                abserr = Math.Abs((res43 - res21) * hlgth);
                goto L65;

                /*          compute the integral using the 87-point formula. */

                L45:
                res87 = w87b[22] * fcentr;
                neval = 87;

                for (k = 1; k <= 21; ++k)
                {
                    res87 += savfun[k - 1] * w87a[k - 1];
                }
                for (k = 1; k <= 22; ++k)
                {
                    absc = hlgth * x4[k - 1];
                    res87 += w87b[k - 1] * (f(absc + centr) + f(centr - absc));
                }

                result = res87 * hlgth;
                abserr = Math.Abs(res87 - res43) * hlgth;

            L65:
                if (resasc != 0.0f && abserr != 0.0f)
                {
                    abserr = resasc * Math.Min(1.0f, Math.Pow(abserr * 200.0f / resasc, 1.5));
                }
                if (resabs > uflow / (epmach * 50.0f))
                {
                    abserr = Math.Max(epmach * 50.0f * resabs, abserr);
                }

                if (abserr <= Math.Max(epsabs, epsrel * Math.Abs(result)))
                {
                    ier = 0;
                }

                /* ***jump out of do-loop */
                if (ier == 0)
                {
                    goto L999;
                }
            }
        L80:
            //throw new Exception("abnormal return from  qng ");
            Trace.Write("abnormal return from  qng");

        L999:
            return 0;
        } /* qng_ */

        static readonly double[] x1 = 
        {
            0.9739065285171717f, 0.8650633666889845f,
	        .6794095682990244f,.4333953941292472f,.1488743389816312f 
        };

        static readonly double[] w87a =
        { 
            0.008148377384149173f, 0.01876143820156282f,  0.02734745105005229f, 
            0.03367770731163793f,  0.03693509982042791f,  0.002884872430211531f, 
            0.0136859460227127f,   0.02328041350288831f,  0.03087249761171336f,  
            0.03569363363941877f,  9.152833452022414e-4f, 0.005399280219300471f, 
            0.01094767960111893f,  0.01629873169678734f,  0.02108156888920384f,  
            0.02537096976925383f,  0.02918969775647575f,  0.03237320246720279f,  
            0.03478309895036514f,  0.03641222073135179f,  0.03725387550304771f 
        };

        static readonly double[] w87b = 
        { 
            2.741455637620724e-4f,.001807124155057943f, 0.004096869282759165f,
            0.006758290051847379f,.009549957672201647f, .01232944765224485f,
            0.01501044734638895f,.01754896798624319f, 0.01993803778644089f, 
            0.02219493596101229f,.02433914712600081f, 0.02637450541483921f,
            0.0282869107887712f,.0300525811280927f, 0.03164675137143993f,
            0.0330504134199785f,.03425509970422606f, 0.03526241266015668f,
            0.0360769896228887f,.03669860449845609f, 0.03712054926983258f,
            0.03733422875193504f,.03736107376267902f 
        };

        static readonly double[] x2 =
        {
            0.9956571630258081f, 0.9301574913557082f,
	        0.7808177265864169f, 0.5627571346686047f, 0.2943928627014602f
        };

        static readonly double[] x3 =
        {
            .9993333609019321f,.9874334029080889f,
	        .9548079348142663f,.9001486957483283f,.8251983149831142f,
	        .732148388989305f,.6228479705377252f,.4994795740710565f,
	        .3649016613465808f,.2222549197766013f,.07465061746138332f 
        };

        static readonly double[] x4 =
        {
            .9999029772627292f,.9979898959866787f,
	        .9921754978606872f,.9813581635727128f,.9650576238583846f,
	        .9431676131336706f,.9158064146855072f,.8832216577713165f,
	        .8457107484624157f,.803557658035231f,.7570057306854956f,
	        .7062732097873218f,.6515894665011779f,.5932233740579611f,
	        .5314936059708319f,.4667636230420228f,.3994248478592188f,
	        .3298748771061883f,.2585035592021616f,.1856953965683467f,
	        .1118422131799075f,.03735212339461987f 
        };

        static readonly double[] w10 = 
        {
            .06667134430868814f,.1494513491505806f,
	        .219086362515982f,.2692667193099964f,.2955242247147529f 
        };

        static readonly double[] w21a =
        {
            .03255816230796473f,.07503967481091995f,
	        .1093871588022976f,.1347092173114733f,.1477391049013385f 
        };

        static readonly double[] w21b =
        { 
            .01169463886737187f,.054755896574352f,
	        .09312545458369761f,.1234919762620659f,.1427759385770601f,
	        .1494455540029169f 
        };

        static readonly double[] w43a =
        {
            .01629673428966656f,.0375228761208695f,
	        .05469490205825544f,.06735541460947809f,.07387019963239395f,
	        .005768556059769796f,.02737189059324884f,.04656082691042883f,
	        .06174499520144256f,.0713872672686934f 
        };

        static readonly double[] w43b =
        {
            .001844477640212414f,.01079868958589165f,
	        .02189536386779543f,.03259746397534569f,.04216313793519181f,
	        .05074193960018458f,.05837939554261925f,.06474640495144589f,
	        .06956619791235648f,.07282444147183321f,.07450775101417512f,
	        .07472214751740301f 
        };
        #endregion


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
            NonAdaptiveGaussKronrod clone = new NonAdaptiveGaussKronrod(
                this.Function, this.Range.Min, this.Range.Max);

            clone.error = error;
            clone.result = result;
            clone.ToleranceAbsolute = ToleranceAbsolute;
            clone.ToleranceRelative = ToleranceRelative;

            return clone;
        }
    }
}
