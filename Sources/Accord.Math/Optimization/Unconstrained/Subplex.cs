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
// Sbplx method (based on Nelder-Mead's Simplex) given in the NLopt
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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Accord.Math.Convergence;
    using Accord.Compat;

    /// <summary>
    ///   Subplex
    /// </summary>
    /// 
    /// <remarks>
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
    public class Subplex : BaseOptimizationMethod, IOptimizationMethod<NelderMeadStatus>
    {

        private int n;

        private double minf_max;
        private GeneralConvergence stop;


        // subplex strategy constants:
        const double psi = 0.25;
        const double omega = 0.1;
        const int nsmin = 2;
        const int nsmax = 5;

        // bounds
        private double[] lb;
        private double[] ub;

        private double[] xprev;
        private double[] dx;
        private double[] absdx;

        private double[] xstep; // initial step sizes
        private double[] xstep0;


        private int[] p; // subspace index permutation
        private int sindex; // starting index for this subspace


        NelderMead nelderMead;
        NelderMeadStatus status;


        /// <summary>
        ///   Creates a new <see cref="Subplex"/> optimization algorithm.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of free parameters in the optimization problem.</param>
        /// 
        public Subplex(int numberOfVariables)
            : base(numberOfVariables)
        {
            init(numberOfVariables);
        }

        /// <summary>
        ///   Creates a new <see cref="Subplex"/> optimization algorithm.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of free parameters in the optimization problem.</param>
        /// <param name="function">The objective function whose optimum values should be found.</param>
        /// 
        public Subplex(int numberOfVariables, Func<double[], double> function)
            : base(numberOfVariables, function)
        {
            init(numberOfVariables);
        }

        /// <summary>
        ///   Creates a new <see cref="Subplex"/> optimization algorithm.
        /// </summary>
        /// 
        /// <param name="function">The objective function whose optimum values should be found.</param>
        /// 
        public Subplex(NonlinearObjectiveFunction function)
            : base(function)
        {
            init(function.NumberOfVariables);
        }

        private void init(int n)
        {
            this.n = n;
            this.stop = new GeneralConvergence(n);

            nelderMead = new NelderMead(nsmax, subspace_func);
            nelderMead.Convergence = this.stop;

            xstep = new double[n];
            xstep0 = new double[n];
            for (int i = 0; i < xstep0.Length; i++)
                xstep0[i] = 1e-5;

            p = new int[n];
            dx = new double[n];
            xprev = new double[n];
            absdx = new double[n];

            lb = new double[n];
            for (int i = 0; i < lb.Length; i++)
                lb[i] = Double.NegativeInfinity;

            ub = new double[n];
            for (int i = 0; i < ub.Length; i++)
                ub[i] = Double.PositiveInfinity;
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
            get { return xstep0; }
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
        ///   Implements the actual optimization algorithm. This
        ///   method should try to minimize the objective function.
        /// </summary>
        /// 
        protected override bool Optimize()
        {
            status = sbplx_minimize();

            return (status == NelderMeadStatus.Success ||
                status == NelderMeadStatus.FunctionToleranceReached ||
                status == NelderMeadStatus.SolutionToleranceReached);
        }

        NelderMeadStatus sbplx_minimize()
        {
            var ret = NelderMeadStatus.Success;

            double[] x = Solution;
            Value = Function(x);

            this.stop.Evaluations++;
            if (NelderMead.nlopt_stop_forced(stop))
                return NelderMeadStatus.ForcedStop;
            if (Value < this.minf_max)
                return NelderMeadStatus.MinimumAllowedValueReached;
            if (NelderMead.nlopt_stop_evals(stop))
                return NelderMeadStatus.MaximumEvaluationsReached;
            if (NelderMead.nlopt_stop_time(stop))
                return NelderMeadStatus.MaximumTimeReached;


            Array.Copy(xstep0, xstep, xstep.Length);


            while (true)
            {
                double normi = 0;
                double normdx = 0;
                int ns, nsubs = 0;
                int nevals = this.stop.Evaluations;
                double fdiff, fdiff_max = 0;

                Array.Copy(x, xprev, x.Length);

                double fprev = Value;

                // sort indices into the progress vector dx
                // by decreasing order of magnitude abs(dx)
                //
                for (int i = 0; i < p.Length; i++)
                    p[i] = i;

                for (int j = 0; j < absdx.Length; j++)
                    absdx[j] = Math.Abs(dx[j]);

                Array.Sort(p, absdx);


                // find the subspaces, and perform nelder-mead on each one
                for (int i = 0; i < absdx.Length; i++)
                    normdx += absdx[i]; // L1 norm

                int last = 0;
                for (int i = 0; i + nsmin < n; i += ns)
                {
                    last = i;

                    // find subspace starting at index i
                    double ns_goodness = -Double.MaxValue;
                    double norm = normi;
                    int nk = i + nsmax > n ? n : i + nsmax; // max k for this subspace

                    for (int k = i; k < i + nsmin - 1; k++)
                        norm += absdx[p[k]];

                    ns = nsmin;
                    for (int k = i + nsmin - 1; k < nk; k++)
                    {
                        double goodness;
                        norm += absdx[p[k]];

                        // remaining subspaces must be big enough to partition
                        if (n - (k + 1) < nsmin)
                            continue;

                        // maximize figure of merit defined by Rowan thesis:
                        // look for sudden drops in average |dx|

                        if (k + 1 < n)
                        {
                            goodness = norm / (k + 1) - (normdx - norm) / (n - (k + 1));
                        }
                        else
                        {
                            goodness = normdx / n;
                        }

                        if (goodness > ns_goodness)
                        {
                            ns_goodness = goodness;
                            ns = (k + 1) - i;
                        }
                    }

                    for (int k = i; k < i + ns; ++k)
                        normi += absdx[p[k]];

                    // do nelder-mead on subspace of dimension ns starting w/i 
                    sindex = i;
                    for (int k = i; k < i + ns; ++k)
                    {
                        nelderMead.Solution[k - i] = x[p[k]];
                        nelderMead.StepSize[k - i] = xstep[p[k]];
                        nelderMead.LowerBounds[k - i] = lb[p[k]];
                        nelderMead.UpperBounds[k - i] = ub[p[k]];
                    }

                    nsubs++;
                    nevals = this.stop.Evaluations;

                    nelderMead.NumberOfVariables = ns;
                    nelderMead.DiameterTolerance = psi;
                    ret = nelderMead.Minimize(Value);

                    fdiff = nelderMead.Difference;
                    Value = nelderMead.Value;

                    if (fdiff > fdiff_max)
                        fdiff_max = fdiff;

                    Trace.WriteLine(String.Format("{0} NM iterations for ({1},{2}) subspace",
                       this.stop.Evaluations - nevals, sindex, ns));

                    for (int k = i; k < i + ns; k++)
                        x[p[k]] = nelderMead.Solution[k - i];

                    if (ret == NelderMeadStatus.Failure)
                        return NelderMeadStatus.SolutionToleranceReached;

                    if (ret != NelderMeadStatus.SolutionToleranceReached)
                        return ret;
                }

                // nelder-mead on last subspace 
                ns = n - last;
                sindex = last;
                for (int i = last; i < n; i++)
                {
                    nelderMead.Solution[i - sindex] = x[p[i]];
                    nelderMead.StepSize[i - sindex] = xstep[p[i]];
                    nelderMead.LowerBounds[i - sindex] = lb[p[i]];
                    nelderMead.UpperBounds[i - sindex] = ub[p[i]];
                }

                nsubs++;
                nevals = this.stop.Evaluations;

                nelderMead.NumberOfVariables = ns;
                nelderMead.DiameterTolerance = psi;
                ret = nelderMead.Minimize(Value);

                fdiff = nelderMead.Difference;
                Value = nelderMead.Value;

                if (fdiff > fdiff_max)
                    fdiff_max = fdiff;

                Trace.WriteLine(String.Format("sbplx: {0} NM iterations for ({1},{2}) subspace",
                   this.stop.Evaluations - nevals, sindex, ns));


                for (int i = sindex; i < p.Length; i++)
                    x[p[i]] = nelderMead.Solution[i - sindex];

                if (ret == NelderMeadStatus.Failure)
                    return NelderMeadStatus.SolutionToleranceReached;

                if (ret != NelderMeadStatus.SolutionToleranceReached)
                    return ret;

                // termination tests:
                if (NelderMead.nlopt_stop_ftol(stop, Value, Value + fdiff_max))
                    return NelderMeadStatus.FunctionToleranceReached;

                if (NelderMead.nlopt_stop_xtol(stop, x, xprev, n))
                {
                    int j;

                    // as explained in Rowan's thesis, it is important
                    // to check |xstep| as well as |x-xprev|, since if
                    // the step size is too large (in early iterations),
                    // the inner Nelder-Mead may not make much progress 
                    //
                    for (j = 0; j < xstep.Length; j++)
                    {
                        if (Math.Abs(xstep[j]) * psi > stop.AbsoluteParameterTolerance[j]
                         && Math.Abs(xstep[j]) * psi > stop.RelativeParameterTolerance * Math.Abs(x[j]))
                            break;
                    }

                    if (j == n)
                    {
                        return NelderMeadStatus.SolutionToleranceReached;
                    }
                }

                // compute change in optimal point
                for (int i = 0; i < x.Length; i++)
                    dx[i] = x[i] - xprev[i];

                // setting step sizes
                {
                    double scale;
                    if (nsubs == 1)
                    {
                        scale = psi;
                    }
                    else
                    {
                        double stepnorm = 0, dxnorm = 0;
                        for (int i = 0; i < dx.Length; i++)
                        {
                            stepnorm += Math.Abs(xstep[i]);
                            dxnorm += Math.Abs(dx[i]);
                        }

                        scale = dxnorm / stepnorm;

                        if (scale < omega)
                            scale = omega;

                        if (scale > 1 / omega)
                            scale = 1 / omega;
                    }


                    Trace.WriteLine("sbplx: stepsize scale factor = " + scale);


                    for (int i = 0; i < xstep.Length; i++)
                    {
                        xstep[i] = (dx[i] == 0) ?
                            -(xstep[i] * scale) : Special.Sign(xstep[i] * scale, dx[i]);
                    }
                }
            }
        }


        /// <summary>
        ///   Wrapper around objective function for subspace optimization.
        /// </summary>
        /// 
        double subspace_func(double[] xs)
        {
            double[] x = Solution;

            for (int i = sindex; i < sindex + n; i++)
                x[p[i]] = xs[i - sindex];

            return Function(x);
        }

    }
}
