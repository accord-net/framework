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

namespace Accord.Math.Optimization
{
    using System;

    /// <summary>
    ///   Status codes for the <see cref="BrentSearch"/>.
    /// </summary>
    /// 
    public enum BrentSearchStatus : byte
    {
        /// <summary>
        ///   The status is unset.
        /// </summary>
        /// 
        None = 0,

        /// <summary>
        ///   Convergence was attained.
        /// </summary>
        /// 
        Success,

        /// <summary>
        ///   The root is not bracketed correctly. The root must be bracketed
        ///   once and only once.
        /// </summary>
        /// 
        RootNotBracketed,

        /// <summary>
        ///   The function was not finite or returned NaN.
        /// </summary>
        /// 
        FunctionNotFinite,

        /// <summary>
        ///   Maximum number of iterations reached.
        /// </summary>
        /// 
        MaxIterationsReached,
    }

    /// <summary>
    ///   Brent's root finding and minimization algorithms.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In numerical analysis, Brent's method is a complicated but popular root-finding 
    ///   algorithm combining the bisection method, the secant method and inverse quadratic
    ///   interpolation. It has the reliability of bisection but it can be as quick as some
    ///   of the less reliable methods. The idea is to use the secant method or inverse quadratic 
    ///   interpolation if possible, because they converge faster, but to fall back to the more
    ///   robust bisection method if necessary. Brent's method is due to Richard Brent (1973)
    ///   and builds on an earlier algorithm of Theodorus Dekker (1969).</para>
    ///   
    /// <para>
    ///   The algorithms implemented in this class are based on the original C source code
    ///   available in Netlib (http://www.netlib.org/c/brent.shar) by Oleg Keselyov, 1991.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       R.P. Brent (1973). Algorithms for Minimization without Derivatives, Chapter 4. 
    ///       Prentice-Hall, Englewood Cliffs, NJ. ISBN 0-13-022335-2. </description></item>
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Brent's_method">
    ///       Wikipedia contributors. "Brent's method." Wikipedia, The Free Encyclopedia.
    ///       Wikipedia, The Free Encyclopedia, 11 May. 2012. Web. 22 Jun. 2012. </a></description></item>
    ///   </list>
    /// </para>   
    /// 
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example shows how to compute the maximum,
    ///   minimum and a single root of a univariate function.</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.Math\Optimization\BrentSearchTest.cs" region="doc_example" />
    /// </example>
    /// 
    /// 
    public sealed class BrentSearch : IOptimizationMethod<double, double>
    {
        private const int  DefaultMaxIterations = 500;
        private const double DefaultTolerance = 1e-6;

        /// <summary>
        ///   Gets the number of variables (free parameters)
        ///   in the optimization problem.
        /// </summary>
        /// 
        /// <value>
        ///   The number of parameters.
        /// </value>
        /// 
        public int NumberOfVariables
        {
            get { return 1; }
            set
            {
                if (value != 1)
                    throw new InvalidOperationException("Brent Search supports only one variable.");
            }
        }

        /// <summary>
        ///   Gets or sets the tolerance margin when
        ///   looking for an answer. Default is 1e-6.
        /// </summary>
        /// 
        public double Tolerance { get; set; }

        /// <summary>
        ///   Gets or sets the lower bound for the search interval <c>a</c>.
        /// </summary>
        /// 
        public double LowerBound { get; set; }

        /// <summary>
        ///   Gets or sets the lower bound for the search interval <c>a</c>.
        /// </summary>
        /// 
        public double UpperBound { get; set; }

        /// <summary>
        ///   Gets the solution found in the last call
        ///   to <see cref="Minimize()"/>, <see cref="Maximize()"/>
        ///   or <see cref="FindRoot()"/>.
        /// </summary>
        /// 
        public double Solution { get; set; }

        /// <summary>
        ///   Gets the value at the solution found in the last call
        ///   to <see cref="Minimize()"/>, <see cref="Maximize()"/>,
        ///   <see cref="Find(double)"/> or <see cref="FindRoot()"/>.
        /// </summary>
        /// 
        public double Value { get; private set; }

        /// <summary>
        ///   Gets or sets the maximum number of iterations that should be 
        ///   performed before the method terminates. Default is 500.
        /// </summary>
        /// 
        public int MaxIterations { get; set; }

        /// <summary>
        ///   Gets the status of the search.
        /// </summary>
        /// 
        public BrentSearchStatus Status { get; private set; }

        /// <summary>
        ///   Gets the function to be searched.
        /// </summary>
        /// 
        public Func<double, double> Function { get; private set; }


        /// <summary>
        ///   Constructs a new Brent search algorithm.
        /// </summary>
        /// 
        /// <param name="function">The function to be searched.</param>
        /// <param name="lowerBound">Start of search region.</param>
        /// <param name="upperBound">End of search region.</param>
        /// <param name="tol">The tolerance for determining the solution.</param>
        /// <param name="maxIterations">The maximum number of iterations before terminating.</param>
        /// 
        public BrentSearch(
            Func<double, double> function, 
            double lowerBound, 
            double upperBound, 
            double tol = DefaultTolerance,
            int maxIterations = DefaultMaxIterations)
        {
            this.Function = function;
            this.LowerBound = lowerBound;
            this.UpperBound = upperBound;
            this.Tolerance = tol;
            this.MaxIterations = maxIterations;
        }


        /// <summary>
        ///   Attempts to find a root in the interval [a;b] 
        /// </summary>
        /// 
        /// <returns>Returns <c>true</c> if the method converged to a <see cref="IOptimizationMethod{TInput, TOutput}.Solution"/>.
        ///   In this case, the found value will also be available at the <see cref="IOptimizationMethod{TInput, TOutput}.Value"/>
        ///   property.</returns>
        ///
        public bool FindRoot()
        {
            BrentSearchResult result = FindRootInternal(
                Function, LowerBound, UpperBound, Tolerance, MaxIterations);

            Solution = result.Solution;
            Value = result.Value;
            Status = result.Status;

            return Status == BrentSearchStatus.Success;
        }

        /// <summary>
        ///   Attempts to find a value in the interval [a;b] 
        /// </summary>
        /// 
        /// <returns>Returns <c>true</c> if the method converged to a <see cref="IOptimizationMethod{TInput, TOutput}.Solution"/>.
        ///   In this case, the found value will also be available at the <see cref="IOptimizationMethod{TInput, TOutput}.Value"/>
        ///   property.</returns>
        ///
        public bool Find(double value)
        {
            BrentSearchResult result = FindRootInternal(
                x => Function(x) - value, LowerBound, UpperBound, Tolerance, MaxIterations);

            Solution = result.Solution;
            Value = result.Value + value;
            Status = result.Status;

            return Status == BrentSearchStatus.Success;
        }

        /// <summary>
        ///   Finds the minimum of the function in the interval [a;b]
        /// </summary>
        /// 
        /// <returns>Returns <c>true</c> if the method converged to a <see cref="IOptimizationMethod{TInput, TOutput}.Solution"/>.
        ///   In this case, the found value will also be available at the <see cref="IOptimizationMethod{TInput, TOutput}.Value"/>
        ///   property.</returns>
        ///
        public bool Minimize()
        {
            BrentSearchResult result = MinimizeInternal(
                Function, LowerBound, UpperBound, Tolerance, MaxIterations);

            Solution = result.Solution;
            Value = result.Value;
            Status = result.Status;

            return Status == BrentSearchStatus.Success;
        }

        /// <summary>
        ///   Finds the maximum of the function in the interval [a;b]
        /// </summary>
        /// 
        /// <returns>Returns <c>true</c> if the method converged to a <see cref="IOptimizationMethod{TInput, TOutput}.Solution"/>.
        ///   In this case, the found value will also be available at the <see cref="IOptimizationMethod{TInput, TOutput}.Value"/>
        ///   property.</returns>
        ///
        public bool Maximize()
        {
            BrentSearchResult result = MinimizeInternal(
                x => -Function(x), LowerBound, UpperBound, Tolerance, MaxIterations);

            Solution = result.Solution;
            Value = -result.Value;
            Status = result.Status;

            return Status == BrentSearchStatus.Success;
        }

        /// <summary>
        ///   Finds the minimum of a function in the interval [a;b]
        /// </summary>
        /// 
        /// <param name="function">The function to be minimized.</param>
        /// <param name="lowerBound">Start of search region.</param>
        /// <param name="upperBound">End of search region.</param>
        /// <param name="tol">The tolerance for determining the solution.</param>
        /// <param name="maxIterations">The maximum number of iterations before terminating.</param>
        /// 
        /// <returns>The location of the minimum of the function in the given interval.</returns>
        /// 
        public static double Minimize(
            Func<double, double> function,
            double lowerBound,
            double upperBound,
            double tol = DefaultTolerance,
            int maxIterations = DefaultMaxIterations)
        {
            BrentSearchResult result = MinimizeInternal(function, lowerBound, upperBound, tol, maxIterations);
            return HandleResult(result);
        }

        /// <summary>
        ///   Finds the maximum of a function in the interval [a;b]
        /// </summary>
        /// 
        /// <param name="function">The function to be maximized.</param>
        /// <param name="lowerBound">Start of search region.</param>
        /// <param name="upperBound">End of search region.</param>
        /// <param name="tol">The tolerance for determining the solution.</param>
        /// <param name="maxIterations">The maximum number of iterations before terminating.</param>
        /// 
        /// <returns>The location of the maximum of the function in the given interval.</returns>
        /// 
        public static double Maximize(
            Func<double, double> function,
            double lowerBound,
            double upperBound, 
            double tol = DefaultTolerance,
            int maxIterations = DefaultMaxIterations)
        {
            return Minimize(x => -function(x), lowerBound, upperBound, tol, maxIterations);
        }

        /// <summary>
        ///   Finds the root of a function in the interval [a;b]
        /// </summary>
        /// 
        /// <param name="function">The function to have its root computed.</param>
        /// <param name="lowerBound">Start of search region.</param>
        /// <param name="upperBound">End of search region.</param>
        /// <param name="tol">The tolerance for determining the solution.</param>
        /// <param name="maxIterations">The maximum number of iterations before terminating.</param>
        /// 
        /// <returns>The location of the zero value in the given interval.</returns>
        /// 
        public static double FindRoot(
            Func<double, double> function,
            double lowerBound,
            double upperBound,
            double tol = DefaultTolerance,
            int maxIterations = DefaultMaxIterations)
        {
            BrentSearchResult result = FindRootInternal(function, lowerBound, upperBound, tol, maxIterations);
            return HandleResult(result);
        }

        /// <summary>
        ///   Finds a value of a function in the interval [a;b]
        /// </summary>
        /// 
        /// <param name="function">The function to have its root computed.</param>
        /// <param name="value">The value to be looked for in the function.</param>
        /// <param name="lowerBound">Start of search region.</param>
        /// <param name="upperBound">End of search region.</param>
        /// <param name="tol">The tolerance for determining the solution.</param>
        /// <param name="maxIterations">The maximum number of iterations before terminating.</param>
        /// 
        /// <returns>The location of the value in the given interval.</returns>
        /// 
        public static double Find(
            Func<double, double> function, 
            double value,
            double lowerBound,
            double upperBound,
            double tol = DefaultTolerance,
            int maxIterations = DefaultMaxIterations)
        {
            return FindRoot(x => function(x) - value, lowerBound, upperBound, tol, maxIterations);
        }

        private static BrentSearchResult MinimizeInternal(
            Func<double, double> function,
            double lowerBound, 
            double upperBound, 
            double tol,
            int maxIterations)
        {
            // perform some basic validation checks and allow these to throw
            if (double.IsInfinity(lowerBound))
                throw new ArgumentOutOfRangeException("lowerBound", "Bounds must be finite");

            if (double.IsInfinity(upperBound))
                throw new ArgumentOutOfRangeException("upperBound", "Bounds must be finite");

            if (tol < 0)
                throw new ArgumentOutOfRangeException("tol", "Tolerance must be positive.");

            if (maxIterations == 0)
                maxIterations = Int32.MaxValue;

            double x, v, w; // Abscissas
            double fx;      // f(x)             
            double fv;      // f(v)
            double fw;      // f(w)

            // Gold section ratio: (3.9 - sqrt(5)) / 2; 
            const double r = 0.831966011250105;

            if (upperBound < lowerBound)
            {
                double tmp = upperBound;
                upperBound = lowerBound;
                lowerBound = tmp;
            }

            // First step - always gold section
            v = lowerBound + r * (upperBound - lowerBound);
            fv = function(v);
            x = v; fx = fv;
            w = v; fw = fv;


            // Main loop
            for (int i = 0; i < maxIterations; i++)
            {
                double range = upperBound - lowerBound; // Range over which the minimum

                double middle_range = lowerBound / 2.0 + upperBound / 2.0;
                double tol_act = Math.Sqrt(Constants.DoubleEpsilon) * Math.Abs(x) + tol / 3;
                double new_step; // Step at this iteration

                // Check if an acceptable solution has been found
                if (Math.Abs(x - middle_range) + range / 2 <= 2 * tol_act)
                    return new BrentSearchResult(x, fx, BrentSearchStatus.Success);

                // Obtain the gold section step
                new_step = r * (x < middle_range ? upperBound - x : lowerBound - x);


                // Decide if the interpolation can be tried:
                // Check if x and w are distinct.
                if (Math.Abs(x - w) >= tol_act)
                {
                    // Yes, they are. Interpolation may be tried. The
                    // interpolation step is calculated as p/q, but the
                    // division operation is delayed until last moment

                    double t = (x - w) * (fx - fv);
                    double q = (x - v) * (fx - fw);
                    double p = (x - v) * q - (x - w) * t;
                    q = 2 * (q - t);

                    // If q was calculated with the opposite sign,
                    // make q positive and assign possible minus to p
                    if (q > 0) { p = -p; } else { q = -q; }


                    if (Math.Abs(p) < Math.Abs(new_step * q) && // If x+p/q falls in [a,b]
                        p > q * (lowerBound - x + 2 * tol_act) &&        // not too close to a and 
                        p < q * (upperBound - x - 2 * tol_act))          // b, and isn't too large
                    {
                        // It is accepted. Otherwise if p/q is too large then the
                        // gold section procedure can reduce [a,b] range further.
                        new_step = p / q;
                    }
                }

                // Adjust the step to be not less than tolerance
                if (Math.Abs(new_step) < tol_act)
                {
                    new_step = (new_step > 0) ? tol_act : -tol_act;
                }

                // Now obtain the next approximation to 
                // min and reduce the enveloping range
                {

                    double t = x + new_step;     // Tentative point for the min
                    double ft = function(t);     // recompute f(tentative point)

                    if (double.IsNaN(ft) || double.IsInfinity(ft))
                        return new BrentSearchResult(x, fx, BrentSearchStatus.FunctionNotFinite);

                    if (ft <= fx)
                    {
                        // t is a better approximation, so reduce
                        // the range so that t would fall within it
                        if (t < x)
                            upperBound = x;
                        else lowerBound = x;

                        // Best approx.
                        v = w; fv = fw;
                        w = x; fw = fx;
                        x = t; fx = ft;
                    }
                    else
                    {
                        // x still remains the better approximation,
                        // so we can reduce the range enclosing x
                        if (t < x)
                            lowerBound = t;
                        else upperBound = t;

                        if (ft <= fw || w == x)
                        {
                            v = w; fv = fw;
                            w = t; fw = ft;
                        }
                        else if (ft <= fv || v == x || v == w)
                        {
                            v = t; fv = ft;
                        }
                    }
                }
            }

            return new BrentSearchResult(x, fx, BrentSearchStatus.MaxIterationsReached);
        }

        private static BrentSearchResult FindRootInternal(
            Func<double, double> function,
            double lowerBound,
            double upperBound,
            double tol,
            int maxIterations)
        {
            // perform some basic validation checks and allow these to throw
            if (double.IsInfinity(lowerBound))
                throw new ArgumentOutOfRangeException("lowerBound", "Bounds must be finite");

            if (double.IsInfinity(upperBound))
                throw new ArgumentOutOfRangeException("upperBound", "Bounds must be finite");

            if (tol < 0)
                throw new ArgumentOutOfRangeException("tol", "Tolerance must be positive.");

            double fa = function(lowerBound); // f(a)
            double fb = function(upperBound); // f(b) 
            double c = lowerBound;            // Abscissas
            double fc = fa;                   // f(c)

            if (Math.Sign(fa) == Math.Sign(fb))
                return new BrentSearchResult(upperBound, fb, BrentSearchStatus.RootNotBracketed);


            // Main loop
            for (int i = 0; i < maxIterations; i++)
            {
                double prev_step = upperBound - lowerBound;

                double new_step; // current step
                double tol_act;  // actual tolerance

                // Interpolation step is calculated in the form p/q, but
                // division operations are delayed until the last moment.


                if (Math.Abs(fc) < Math.Abs(fb))
                {
                    // Swap data for b to be the best approximation
                    lowerBound = upperBound; fa = fb;
                    upperBound = c; fb = fc;
                    c = lowerBound; fc = fa;
                }

                tol_act = 2 * Constants.DoubleEpsilon * Math.Abs(upperBound) + tol / 2;
                new_step = (c - upperBound) / 2;

                // Check if an acceptable solution has been found
                if (Math.Abs(new_step) <= tol_act || fb == 0)
                    return new BrentSearchResult(upperBound, fb, BrentSearchStatus.Success);

                // Decide if the interpolation can be tried
                if (Math.Abs(prev_step) >= tol_act  // If prev_step was large enough
                    && Math.Abs(fa) > Math.Abs(fb)) // and was in the true direction,
                {
                    // Then interpolation may be tried   
                    double t1, cb, t2;
                    double p, q;
                    cb = c - upperBound;
                    if (lowerBound == c)
                    {
                        // If we have only two distinct points, then
                        // only linear interpolation can be applied
                        t1 = fb / fa;
                        p = cb * t1;
                        q = 1.0 - t1;
                    }
                    else
                    {
                        // We can apply quadric inverse interpolation
                        q = fa / fc;
                        t1 = fb / fc;
                        t2 = fb / fa;
                        p = t2 * (cb * q * (q - t1) - (upperBound - lowerBound) * (t1 - 1.0));
                        q = (q - 1.0) * (t1 - 1.0) * (t2 - 1.0);
                    }

                    // If q was calculated with the opposite sign,
                    // make q positive and assign possible minus to p
                    if (p > 0) q = -q; else p = -p;

                    // If b+p/q falls in [b,c] and isn't too large, then
                    if (p < (0.75 * cb * q - Math.Abs(tol_act * q) / 2)
                        && p < Math.Abs(prev_step * q / 2))
                    {
                        // It is accepted.
                        new_step = p / q;
                    }

                    // Otherwise if p/q is too large then the bisection
                    // procedure can reduce [b,c] to a further extent
                    // 
                }

                if (Math.Abs(new_step) < tol_act)
                {
                    // Adjust the step to be not less than tolerance
                    new_step = (new_step > 0) ? tol_act : -tol_act;
                }

                // Save the previous approximation,
                lowerBound = upperBound; fa = fb;

                // and do a step to a new approximation
                upperBound += new_step;
                fb = function(upperBound);

                if (double.IsNaN(fb) || double.IsInfinity(fb))
                    return new BrentSearchResult(upperBound, fb, BrentSearchStatus.FunctionNotFinite);

                // Adjust c to have a sign opposite to that of b
                if ((fb > 0 && fc > 0) || (fb < 0 && fc < 0))
                {
                    c = lowerBound; fc = fa;
                }
            }

            return new BrentSearchResult(upperBound, fb, BrentSearchStatus.MaxIterationsReached);
        }

        private static double HandleResult(BrentSearchResult result)
        {
            switch (result.Status)
            {
                case BrentSearchStatus.Success:
                    return result.Solution;
                case BrentSearchStatus.RootNotBracketed:
                    throw new ConvergenceException("Root must be enclosed between bounds once and only once.");
                case BrentSearchStatus.FunctionNotFinite:
                    throw new ConvergenceException("Function evaluation didn't return a finite number.");
                case BrentSearchStatus.MaxIterationsReached:
                    throw new ConvergenceException("The maximum number of iterations was reached.");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private struct BrentSearchResult
        {
            public BrentSearchResult(double solution, double value, BrentSearchStatus status)
                : this()
            {
                this.Solution = solution;
                this.Value = value;
                this.Status = status;
            }

            public double Solution { get; private set; }
            public double Value { get; private set; }
            public BrentSearchStatus Status { get; private set; }
        }
    }
}
