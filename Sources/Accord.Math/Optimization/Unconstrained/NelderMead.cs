// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Steven G. Johnson, 2008
// stevenj@alum.mit.edu
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
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//
//
// The source code presented in this file has been adapted from the
// Nelder Mead Simplex method implementation presented in the NLopt
// Numerical Optimization Library. Original license is given below.
//
//    Copyright (c) 2007-2011 Massachusetts Institute of Technology
//
//    Permission is hereby granted, free of charge, to any person obtaining
//    a copy of this software and associated documentation files (the
//    "Software"), to deal in the Software without restriction, including
//    without limitation the rights to use, copy, modify, merge, publish,
//    distribute, sublicense, and/or sell copies of the Software, and to
//    permit persons to whom the Software is furnished to do so, subject to
//    the following conditions:
// 
//    The above copyright notice and this permission notice shall be
//    included in all copies or substantial portions of the Software.
// 
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//    LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//    OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//

namespace Accord.Math.Optimization
{
    using Accord.Collections;
    using Accord.Math.Convergence;
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///   <see cref="NelderMead"/> exit codes.
    /// </summary>
    /// 
    public enum NelderMeadStatus
    {
        /// <summary>
        ///   Optimization was canceled by the user.
        /// </summary>
        /// 
        ForcedStop,

        /// <summary>
        ///   Optimization ended successfully.
        /// </summary>
        /// 
        Success,

        /// <summary>
        ///   The execution time exceeded the established limit.
        /// </summary>
        /// 
        MaximumTimeReached,

        /// <summary>
        ///   The minimum desired value has been reached.
        /// </summary>
        /// 
        MinimumAllowedValueReached,

        /// <summary>
        ///   The algorithm had stopped prematurely because 
        ///   the maximum number of evaluations was reached.
        /// </summary>
        /// 
        MaximumEvaluationsReached,

        /// <summary>
        ///   The algorithm failed internally.
        /// </summary>
        /// 
        Failure,

        /// <summary>
        ///   The desired output tolerance (minimum change in the function
        ///   output between two consecutive iterations) has been reached.
        /// </summary>
        /// 
        FunctionToleranceReached,

        /// <summary>
        ///   The desired parameter tolerance (minimum change in the 
        ///   solution vector between two iterations) has been reached.
        /// </summary>
        /// 
        SolutionToleranceReached,
    }

    /// <summary>
    ///   Nelder-Mead simplex algorithm with support for bound 
    ///   constraints for non-linear, gradient-free optimization.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Nelder–Mead method or downhill simplex method or amoeba method is a 
    ///   commonly used nonlinear optimization technique, which is a well-defined 
    ///   numerical method for problems for which derivatives may not be known. 
    ///   However, the Nelder–Mead technique is a heuristic search method that can
    ///   converge to non-stationary points on problems that can be solved by 
    ///   alternative methods.</para>
    ///   
    /// <para>
    ///   The Nelder–Mead technique was proposed by John Nelder and Roger Mead (1965)
    ///   and is a technique for minimizing an objective function in a many-dimensional
    ///   space.</para>
    ///   
    /// <para>
    ///   The source code presented in this file has been adapted from the
    ///   Sbplx method (based on Nelder-Mead's Simplex) given in the NLopt
    ///   Numerical Optimization Library, created by Steven G. Johnson.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://ab-initio.mit.edu/nlopt">
    ///       Steven G. Johnson, The NLopt nonlinear-optimization package, 
    ///       http://ab-initio.mit.edu/nlopt </a></description></item>
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Nelder%E2%80%93Mead_method">
    ///       Wikipedia, The Free Encyclopedia. Nelder Mead method. Available on:
    ///       http://en.wikipedia.org/wiki/Nelder%E2%80%93Mead_method </a></description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code source="Unit Tests\Accord.Tests.Math\Optimization\NelderMeadTest.cs" region="doc_min" />
    /// </example>
    /// 
    public class NelderMead : BaseOptimizationMethod, IOptimizationMethod<NelderMeadStatus>
    {
        private int nmax;

        GeneralConvergence stop;

        // heuristic "strategy" constants:
        const double alpha = 1;
        const double beta = 0.5;
        const double gamm = 2;
        const double delta = 0.5;

        // bounds
        private double[] lb;
        private double[] ub;

        private double[] xstep; // initial step sizes


        private double[][] pts; // simplex points
        private double[] val;   // vertex values

        private double[] c; // centroid * n
        private double[] xcur;  // current point 

        NelderMeadStatus status;

        private double minf_max = Double.NegativeInfinity;

        private double psi;
        private double fdiff;


        /// <summary>
        ///   Creates a new <see cref="NelderMead"/> non-linear optimization algorithm.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of free parameters in the optimization problem.</param>
        /// 
        public NelderMead(int numberOfVariables)
            : base(numberOfVariables)
        {
            init(numberOfVariables);
        }


        /// <summary>
        ///   Creates a new <see cref="NelderMead"/> non-linear optimization algorithm.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of free parameters in the optimization problem.</param>
        /// <param name="function">The objective function whose optimum values should be found.</param>
        /// 
        public NelderMead(int numberOfVariables, Func<double[], double> function)
            : base(numberOfVariables, function)
        {
            init(numberOfVariables);
        }

        /// <summary>
        ///   Creates a new <see cref="NelderMead"/> non-linear optimization algorithm.
        /// </summary>
        /// 
        /// <param name="function">The objective function whose optimum values should be found.</param>
        /// 
        public NelderMead(NonlinearObjectiveFunction function)
            : base(function)
        {
            init(function.NumberOfVariables);
        }

        private void init(int n)
        {
            this.nmax = n;
            this.stop = new GeneralConvergence(nmax);

            pts = new double[n + 1][];
            for (int i = 0; i < pts.Length; i++)
                pts[i] = new double[n];

            val = new double[n + 1];

            c = new double[n];
            xcur = new double[n];

            xstep = new double[n];
            for (int i = 0; i < xstep.Length; i++)
                xstep[i] = 1e-5;

            lb = new double[n];
            for (int i = 0; i < lb.Length; i++)
                lb[i] = Double.NegativeInfinity;

            ub = new double[n];
            for (int i = 0; i < ub.Length; i++)
                ub[i] = Double.PositiveInfinity;
        }

        /// <summary>
        ///   Gets the maximum <see cref="NumberOfVariables">number of 
        ///   variables</see> that can be optimized by this instance.
        ///   This is the initial value that has been passed to this
        ///   class constructor at the time the algorithm was created.
        /// </summary>
        /// 
        public int Capacity
        {
            get { return nmax; }
        }

        /// <summary>
        ///   Gets or sets the maximum value that the objective 
        ///   function could produce before the algorithm could
        ///   be terminated as if the solution was good enough.
        /// </summary>
        /// 
        public double MaximumValue
        {
            get { return minf_max; }
            set { minf_max = value; }
        }

        /// <summary>
        ///   Gets the step sizes to be used by the optimization
        ///   algorithm. Default is to initialize each with 1e-5.
        /// </summary>
        /// 
        public double[] StepSize
        {
            get { return xstep; }
        }

        /// <summary>
        ///   Gets or sets the number of variables (free parameters) in the
        ///   optimization problem. This number can be decreased after the
        ///   algorithm has been created so it can operate on subspaces.
        /// </summary>
        /// 
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        /// 
        public override int NumberOfVariables
        {
            get { return base.NumberOfVariables; }
            set
            {
                if (nmax > 0 && (value <= 0 || value > nmax))
                {
                    throw new ArgumentOutOfRangeException("value",
                        "The number of variables must be higher than 0 and less than or"
                      + " equal to the maximum number of variables initially passed to the"
                      + " Nelder-Mead constructor (it was passed " + nmax + ").");
                }

                base.NumberOfVariables = value;
            }
        }

        /// <summary>
        /// Called when the <see cref="NumberOfVariables" /> property has changed.
        /// </summary>
        /// <param name="numberOfVariables">The number of variables.</param>
        protected override void OnNumberOfVariablesChanged(int numberOfVariables)
        {
            if (Solution == null || numberOfVariables > Solution.Length)
                base.OnNumberOfVariablesChanged(numberOfVariables);
        }

        /// <summary>
        ///   Gets or sets multiple convergence options to 
        ///   determine when the optimization can terminate.
        /// </summary>
        /// 
        public GeneralConvergence Convergence
        {
            get { return stop; }
            set { stop = value; }
        }

        /// <summary>
        ///   Get the exit code returned in the last call to the
        ///   <see cref="IOptimizationMethod{TInput, TOutput}.Maximize()"/> or 
        ///   <see cref="IOptimizationMethod{TInput, TOutput}.Minimize()"/> methods.
        /// </summary>
        /// 
        public NelderMeadStatus Status
        {
            get { return status; }
        }

        /// <summary>
        ///   Gets the lower bounds that should be respected in this 
        ///   optimization problem. Default is to initialize this vector
        ///   with <see cref="Double.NegativeInfinity"/>.
        /// </summary>
        /// 
        public double[] LowerBounds
        {
            get { return lb; }
        }

        /// <summary>
        ///   Gets the upper bounds that should be respected in this 
        ///   optimization problem. Default is to initialize this vector
        ///   with <see cref="Double.PositiveInfinity"/>.
        /// </summary>
        /// 
        public double[] UpperBounds
        {
            get { return ub; }
        }

        /// <summary>
        ///   Gets or sets the by how much the simplex diameter |xl - xh| must be
        ///   reduced before the algorithm can be terminated. Setting this value
        ///   to a value higher than zero causes the algorithm to replace the
        ///   standard <see cref="Convergence"/> criteria with this condition.
        ///   Default is zero.
        /// </summary>
        /// 
        public double DiameterTolerance
        {
            get { return psi; }
            set { psi = value; }
        }

        /// <summary>
        ///   The difference between the high and low function 
        ///   values of the last simplex in the previous call
        ///   to the optimization function.
        /// </summary>
        /// 
        public double Difference
        {
            get { return fdiff; }
        }


        /// <summary>
        ///   Finds the minimum value of a function, using the function output at
        ///   the current value, if already known. This overload can be used when
        ///   embedding Nelder-Mead in other algorithms to avoid initial checks.
        /// </summary>
        /// 
        /// <param name="fmin">The function output at the current values, if already known.</param>
        ///  
        public NelderMeadStatus Minimize(double fmin)
        {
            Value = fmin;
            status = minimize();
            return status;
        }

        /// <summary>
        ///   Implements the actual optimization algorithm. This
        ///   method should try to minimize the objective function.
        /// </summary>
        /// 
        protected override bool Optimize()
        {
            status = NelderMeadStatus.Success;

            Value = Function(Solution);
            stop.Evaluations++;

            if (nlopt_stop_forced(stop))
                status = NelderMeadStatus.ForcedStop;
            else if (Value < minf_max)
                status = NelderMeadStatus.MinimumAllowedValueReached;
            else if (nlopt_stop_evals(stop))
                status = NelderMeadStatus.MaximumEvaluationsReached;
            else if (nlopt_stop_time(stop))
                status = NelderMeadStatus.MaximumTimeReached;

            if (status != NelderMeadStatus.Success)
                return false;

            status = minimize();

            return (status == NelderMeadStatus.Success ||
                status == NelderMeadStatus.FunctionToleranceReached ||
                status == NelderMeadStatus.SolutionToleranceReached);
        }



        private NelderMeadStatus minimize()
        {
            /*
               Internal version of nldrmd_minimize, intended to be used as
             a subroutine for the subplex method.  Three differences compared
             to nldrmd_minimize:

             *minf should contain the value of f(x)  (so that we don't have to
             re-evaluate f at the starting x).

             if psi > 0, then it *replaces* xtol and ftol in stop with the condition
             that the simplex diameter |xl - xh| must be reduced by a factor of psi 
             ... this is for when nldrmd is used within the subplex method; for
             ordinary termination tests, set psi = 0. 

             scratch should contain an array of length >= (n+1)*(n+1) + 2*n,
             used as scratch workspace. 

             On output, *fdiff will contain the difference between the high
             and low function values of the last simplex.                   */


            double[] x = Solution;
            int n = NumberOfVariables;

            double ninv = 1.0 / n;
            var ret = NelderMeadStatus.Success;
            double init_diam = 0;

            var t = new RedBlackTree<double, double[]>(allowDuplicates: true);

            fdiff = Double.MaxValue;

            // initialize the simplex based on the starting xstep
            Array.Copy(x, pts[0], n);

            val[0] = Value;

            if (Value < minf_max)
                return NelderMeadStatus.MinimumAllowedValueReached;

            for (int i = 0; i < n; i++)
            {
                double[] pt = pts[i + 1];

                Array.Copy(x, pt, x.Length);

                pt[i] += xstep[i];

                if (pt[i] > ub[i])
                {
                    if (ub[i] - x[i] > Math.Abs(xstep[i]) * 0.1)
                    {
                        pt[i] = ub[i];
                    }
                    else
                    {
                        // ub is too close to pt, go in other direction
                        pt[i] = x[i] - Math.Abs(xstep[i]);
                    }
                }

                if (pt[i] < lb[i])
                {
                    if (x[i] - lb[i] > Math.Abs(xstep[i]) * 0.1)
                    {
                        pt[i] = lb[i];
                    }
                    else
                    {
                        // lb is too close to pt, go in other direction
                        pt[i] = x[i] + Math.Abs(xstep[i]);

                        if (pt[i] > ub[i])
                        {
                            // go towards further of lb, ub
                            pt[i] = 0.5 * ((ub[i] - x[i] > x[i] - lb[i] ?
                                      ub[i] : lb[i]) + x[i]);
                        }
                    }
                }

                if (close(pt[i], x[i]))
                    return NelderMeadStatus.Failure;

                val[i + 1] = Function(pt);

                ret = checkeval(pt, val[i + 1]);
                if (ret != NelderMeadStatus.Success)
                    return ret;
            }

            restart:
            for (int i = 0; i < n + 1; i++)
            {
                t.Add(new KeyValuePair<double, double[]>(val[i], pts[i]));
            }

            while (true)
            {
                var low = t.Min();
                var high = t.Max();
                double fl = low.Value.Key;
                double[] xl = low.Value.Value;
                double fh = high.Value.Key;
                double[] xh = high.Value.Value;
                double fr;

                fdiff = fh - fl;

                if (init_diam == 0)
                {
                    // initialize diam for psi convergence test
                    for (int i = 0; i < n; i++)
                        init_diam += Math.Abs(xl[i] - xh[i]);
                }

                if (psi <= 0 && nlopt_stop_ftol(stop, fl, fh))
                    return NelderMeadStatus.FunctionToleranceReached;

                // compute centroid ... if we cared about the performance of this,
                //   we could do it iteratively by updating the centroid on
                //   each step, but then we would have to be more careful about
                //   accumulation of rounding errors... anyway n is unlikely to
                //   be very large for Nelder-Mead in practical cases

                Array.Clear(c, 0, n);
                for (int i = 0; i < n + 1; i++)
                {
                    double[] xi = pts[i];

                    if (xi != xh)
                    {
                        for (int j = 0; j < n; ++j)
                            c[j] += xi[j];
                    }
                }

                for (int i = 0; i < n; i++)
                    c[i] *= ninv;

                // x convergence check: find xcur = max radius from centroid
                Array.Clear(xcur, 0, n);

                for (int i = 0; i < n + 1; i++)
                {
                    double[] xi = pts[i];
                    for (int j = 0; j < n; j++)
                    {
                        double dx = Math.Abs(xi[j] - c[j]);

                        if (dx > xcur[j])
                            xcur[j] = dx;
                    }
                }

                for (int i = 0; i < n; i++)
                    xcur[i] += c[i];

                if (psi > 0)
                {
                    double diam = 0;
                    for (int i = 0; i < n; i++)
                        diam += Math.Abs(xl[i] - xh[i]);

                    if (diam < psi * init_diam)
                        return NelderMeadStatus.SolutionToleranceReached;
                }
                else if (nlopt_stop_xtol(stop, c, xcur, n))
                {
                    return NelderMeadStatus.SolutionToleranceReached;
                }

                // reflection
                if (!reflectpt(n, xcur, c, alpha, xh, lb, ub))
                {
                    return NelderMeadStatus.SolutionToleranceReached;
                }

                fr = Function(xcur);

                ret = checkeval(xcur, fr);
                if (ret != NelderMeadStatus.Success)
                    return ret;

                if (fr < fl)
                {
                    // new best point, expand simplex 
                    if (!reflectpt(n, xh, c, gamm, xh, lb, ub))
                        return NelderMeadStatus.SolutionToleranceReached;

                    fh = Function(xh);

                    ret = checkeval(xh, fh);
                    if (ret != NelderMeadStatus.Success)
                        return ret;

                    if (fh >= fr)
                    {
                        // expanding didn't improve
                        fh = fr;
                        Array.Copy(xcur, xh, n);
                    }
                }
                else if (fr < t.GetPreviousNode(high).Value.Key)
                {
                    // accept new point
                    Array.Copy(xcur, xh, n);
                    fh = fr;
                }
                else
                {
                    // new worst point, contract
                    double fc;
                    if (!reflectpt(n, xcur, c, fh <= fr ? -beta : beta, xh, lb, ub))
                    {
                        return NelderMeadStatus.SolutionToleranceReached;
                    }

                    fc = Function(xcur);

                    ret = checkeval(xcur, fc);
                    if (ret != NelderMeadStatus.Success)
                        return ret;

                    if (fc < fr && fc < fh)
                    {
                        // successful contraction
                        Array.Copy(xcur, xh, n);
                        fh = fc;
                    }
                    else
                    {
                        // failed contraction, shrink simplex
                        t.Clear();

                        for (int i = 0; i < n + 1; i++)
                        {
                            double[] pt = pts[i];

                            if (pt != xl)
                            {
                                if (!reflectpt(n, pt, xl, -delta, pt, lb, ub))
                                {
                                    return NelderMeadStatus.SolutionToleranceReached;
                                }

                                val[i] = Function(pt);
                                ret = checkeval(pt, val[i]);
                                if (ret != NelderMeadStatus.Success)
                                    return ret;
                            }
                        }

                        goto restart;
                    }
                }

                high.Value = new KeyValuePair<double, double[]>(fh, high.Value.Value);
                t.Resort(high);
            }
        }






        /// <summary>
        ///   Performs the reflection <c>xnew = c + scale * (c - xold)</c>,
        ///   returning 0 if <c>xnew == c</c> or <c>xnew == xold</c> (coincident
        ///   points), and 1 otherwise.
        /// </summary>
        /// 
        /// <remarks>
        ///   The reflected point xnew is "pinned" to the lower and upper bounds
        ///   (lb and ub), as suggested by J. A. Richardson and J. L. Kuester,
        ///   "The complex method for constrained optimization," Commun. ACM
        ///   16(8), 487-489 (1973).  This is probably a suboptimal way to handle
        ///   bound constraints, but I don't know a better way.  The main danger
        ///   with this is that the simplex might collapse into a
        ///   lower-dimensional hyperplane; this danger can be ameliorated by
        ///   restarting (as in subplex), however.
        /// </remarks>
        /// 
        static bool reflectpt(int n, double[] xnew, double[] c,
            double scale, double[] xold, double[] lb, double[] ub)
        {
            bool equalc = true, equalold = true;

            for (int i = 0; i < n; ++i)
            {
                double newx = c[i] + scale * (c[i] - xold[i]);

                if (newx < lb[i])
                    newx = lb[i];

                if (newx > ub[i])
                    newx = ub[i];

                equalc = equalc && close(newx, c[i]);
                equalold = equalold && close(newx, xold[i]);
                xnew[i] = newx;
            }

            return !(equalc || equalold);
        }


        internal NelderMeadStatus checkeval(double[] xc, double fc)
        {
            stop.Evaluations++;

            if (nlopt_stop_forced(stop))
                return NelderMeadStatus.ForcedStop;

            if (fc <= Value)
            {
                Value = fc;

                Array.Copy(xc, Solution, NumberOfVariables);

                if (Value < minf_max)
                    return NelderMeadStatus.MinimumAllowedValueReached;
            }

            if (nlopt_stop_evals(stop))
                return NelderMeadStatus.MaximumEvaluationsReached;
            if (nlopt_stop_time(stop))
                return NelderMeadStatus.MaximumTimeReached;

            return NelderMeadStatus.Success;
        }



        /// <summary>
        ///   Determines whether two numbers are numerically 
        ///   close (within current floating-point precision).
        /// </summary>
        /// 
        static bool close(double a, double b)
        {
            return Math.Abs(a - b) <= 1e-13 * (Math.Abs(a) + Math.Abs(b));
        }









        internal static bool nlopt_stop_ftol(GeneralConvergence stop, double f, double oldf)
        {
            var ftol_rel = stop.RelativeFunctionTolerance;
            var ftol_abs = stop.AbsoluteFunctionTolerance;

            return (relstop(oldf, f, ftol_rel, ftol_abs));
        }

        internal static bool nlopt_stop_xtol(GeneralConvergence stop, double[] x, double[] oldx, int n)
        {
            var xtol_rel = stop.RelativeParameterTolerance;
            var xtol_abs = stop.AbsoluteParameterTolerance;

            for (int i = 0; i < n; ++i)
            {
                if (!relstop(oldx[i], x[i], xtol_rel, xtol_abs[i]))
                    return false;
            }

            return true;
        }

        internal static bool nlopt_stop_forced(GeneralConvergence stop)
        {
            return stop.Cancel;
        }

        internal static bool nlopt_stop_evals(GeneralConvergence stop)
        {
            var maxeval = stop.MaximumEvaluations;
            var nevals = stop.Evaluations;
            return (maxeval > 0 && nevals >= maxeval);
        }

        internal static bool nlopt_stop_time(GeneralConvergence stop)
        {
            var maxtime = stop.MaximumTime;
            var start = stop.StartTime;
            return (maxtime > TimeSpan.Zero && DateTime.Now - start >= maxtime);
        }


        internal static bool relstop(double old, double n, double reltol, double abstol)
        {
            if (Double.IsInfinity(old))
                return false;

            return (Math.Abs(n - old) < abstol
               || Math.Abs(n - old) < reltol * (Math.Abs(n) + Math.Abs(old)) * 0.5
               || (reltol > 0 && n == old)); /* catch new == old == 0 case */
        }
    }
}
