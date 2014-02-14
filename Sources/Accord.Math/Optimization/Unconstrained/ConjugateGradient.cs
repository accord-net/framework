// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
//
// Copyright © Jorge Nocedal, 1990
// http://users.eecs.northwestern.edu/~nocedal/
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
    ///   Conjugate gradient direction update formula.
    /// </summary>
    /// 
    public enum ConjugateGradientMethod
    {
        /// <summary>
        ///   Fletcher-Reeves formula.
        /// </summary>
        /// 
        FletcherReeves = 1,

        /// <summary>
        ///   Polak-Ribière formula.
        /// </summary>
        /// 
        /// <remarks>
        ///   The Polak-Ribière is known to perform better for non-quadratic functions.
        /// </remarks>
        /// 
        PolakRibiere = 2,

        /// <summary>
        ///   Polak-Ribière formula.
        /// </summary>
        /// 
        /// <remarks>
        ///   The Polak-Ribière is known to perform better for non-quadratic functions.
        ///   The positive version B=max(0,Bpr) provides a direction reset automatically.
        /// </remarks>
        /// 
        PositivePolakRibiere = 3,
    }

    /// <summary>
    ///   Conjugate Gradient (CG) optimization method.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In mathematics, the conjugate gradient method is an algorithm for the numerical solution of
    ///   particular systems of linear equations, namely those whose matrix is symmetric and positive-
    ///   definite. The conjugate gradient method is an iterative method, so it can be applied to sparse
    ///   systems that are too large to be handled by direct methods. Such systems often arise when
    ///   numerically solving partial differential equations. The nonlinear conjugate gradient method 
    ///   generalizes the conjugate gradient method to nonlinear optimization (Wikipedia, 2011).</para>
    /// <para>
    ///   T</para>
    /// <para>
    ///   The framework implementation of this method is based on the original FORTRAN source code
    ///   by Jorge Nocedal (see references below). The original FORTRAN source code of CG+ (for large
    ///   scale unconstrained problems) is available at http://users.eecs.northwestern.edu/~nocedal/CG+.html
    ///   and had been made freely available for educational or commercial use. The original authors
    ///   expect that all publications describing work using this software quote the (Gilbert and Nocedal, 1992)
    ///   reference given below.</para>
    /// 
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://users.eecs.northwestern.edu/~nocedal/CG+.html">
    ///        J. C. Gilbert and J. Nocedal. Global Convergence Properties of Conjugate Gradient
    ///        Methods for Optimization, (1992) SIAM J. on Optimization, 2, 1.</a></description></item>
    ///     <item><description>
    ///        Wikipedia contributors, "Nonlinear conjugate gradient method," Wikipedia, The Free 
    ///        Encyclopedia, http://en.wikipedia.org/w/index.php?title=Nonlinear_conjugate_gradient_method
    ///        (accessed December 22, 2011).</description></item>
    ///     <item><description>
    ///        Wikipedia contributors, "Conjugate gradient method," Wikipedia, The Free Encyclopedia,
    ///        http://en.wikipedia.org/w/index.php?title=Conjugate_gradient_method 
    ///        (accessed December 22, 2011).</description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    public class ConjugateGradient : IGradientOptimizationMethod
    {

        private int numberOfVariables;

        private double[] x; // current solution x
        private double f;   // value at current solution f(x)
        private double[] g; // gradient at current solution

        private double[] d;
        private double[] gold;
        private double[] w;

        private double ftol = 1e-4;
        private double gtol = 0.1;
        private double xtol = 1e-17;
        private double stpmin = 1e-20;
        private double stpmax = 1e20;
        private double epsilon = 1e-5;
        private int maxfev = 40;
        private int iterations;
        private int evaluations;
        private int searches;
        private int maxIterations;
        private double tolerance = 0;

        /// <summary>
        ///   Gets or sets the relative difference threshold
        ///   to be used as stopping criteria between two
        ///   iterations. Default is 0 (iterate until convergence). 
        /// </summary>
        /// 
        public double Tolerance
        {
            get { return tolerance; }
            set { tolerance = value; }
        }

        /// <summary>
        ///   Gets or sets the maximum number of iterations
        ///   to be performed during optimization. Default
        ///   is 0 (iterate until convergence).
        /// </summary>
        /// 
        public int MaxIterations
        {
            get { return maxIterations; }
            set { maxIterations = value; }
        }

        /// <summary>
        ///   Gets or sets the conjugate gradient update 
        ///   method to be used during optimization.
        /// </summary>
        /// 
        public ConjugateGradientMethod Method { get; set; }

        /// <summary>
        ///   Gets the number of iterations performed 
        ///   in the last call to <see cref="Minimize()"/>.
        /// </summary>
        /// 
        /// <value>
        ///   The number of iterations performed
        ///   in the previous optimization.</value>
        ///   
        public int Iterations
        {
            get { return iterations; }
        }

        /// <summary>
        ///   Gets the number of function evaluations performed
        ///   in the last call to <see cref="Minimize()"/>.
        /// </summary>
        /// 
        /// <value>
        ///   The number of evaluations performed
        ///   in the previous optimization.</value>
        ///   
        public int Evaluations
        {
            get { return evaluations; }
        }

        /// <summary>
        ///   Gets the number of linear searches performed
        ///   in the last call to <see cref="Minimize()"/>.
        /// </summary>
        /// 
        public int Searches
        {
            get { return searches; }
        }

        /// <summary>
        ///   Occurs when progress is made during the optimization.
        /// </summary>
        /// 
        public event EventHandler<OptimizationProgressEventArgs> Progress;

        /// <summary>
        ///   Gets or sets the function to be optimized.
        /// </summary>
        /// 
        /// <value>The function to be optimized.</value>
        /// 
        public Func<double[], double> Function { get; set; }

        /// <summary>
        ///   Gets or sets a function returning the gradient
        ///   vector of the function to be optimized for a
        ///   given value of its free parameters.
        /// </summary>
        /// 
        /// <value>The gradient function.</value>
        /// 
        public Func<double[], double[]> Gradient { get; set; }

        /// <summary>
        ///   Gets the number of variables (free parameters)
        ///   in the optimization problem.
        /// </summary>
        /// 
        /// <value>The number of parameters.</value>
        /// 
        public int Parameters
        {
            get { return numberOfVariables; }
        }


        /// <summary>
        ///   Gets the solution found, the values of the 
        ///   parameters which optimizes the function.
        /// </summary>
        /// 
        public double[] Solution
        {
            get { return x; }
        }

        /// <summary>
        ///   Gets the output of the function at the current solution.
        /// </summary>
        /// 
        public double Value
        {
            get { return f; }
        }


        /// <summary>
        ///   Creates a new instance of the CG optimization algorithm.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of free parameters in the optimization problem.</param>
        /// 
        public ConjugateGradient(int numberOfVariables)
        {
            if (numberOfVariables <= 0)
                throw new ArgumentOutOfRangeException("numberOfVariables");

            this.numberOfVariables = numberOfVariables;

            d = new double[numberOfVariables];
            gold = new double[numberOfVariables];
            w = new double[numberOfVariables];

            x = new double[numberOfVariables];
            for (int i = 0; i < x.Length; i++)
                x[i] = Accord.Math.Tools.Random.NextDouble() * 2 - 1;
        }


        /// <summary>
        ///   Creates a new instance of the CG optimization algorithm.
        /// </summary>
        /// 
        /// <param name="parameters">The number of free parameters in the function to be optimized.</param>
        /// <param name="function">The function to be optimized.</param>
        /// <param name="gradient">The gradient of the function.</param>
        /// 
        public ConjugateGradient(int parameters, Func<double[], double> function, Func<double[], double[]> gradient)
            : this(parameters)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            if (gradient == null)
                throw new ArgumentNullException("gradient");

            this.Function = function;
            this.Gradient = gradient;
        }


        /// <summary>
        ///   Optimizes the defined function.
        /// </summary>
        /// 
        public double Minimize()
        {
            return minimize();
        }

        /// <summary>
        ///   Optimizes the defined function.
        /// </summary>
        /// 
        /// <param name="values">The initial guess values for the parameters.</param>
        /// 
        public double Minimize(double[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            if (values.Length != numberOfVariables)
                throw new DimensionMismatchException("values");

            // Copy initial guess for solution
            for (int i = 0; i < x.Length; i++)
                x[i] = values[i];

            return minimize();
        }

        private double minimize()
        {
            // This code has been adapted from the original
            // FORTRAN function CGFAM by Jorge Nocedal, 1992.

            if (Function == null) throw new InvalidOperationException(
                 "The function to be minimized has not been defined.");

            if (Gradient == null) throw new InvalidOperationException(
                "The gradient function has not been defined.");

            int irest = 1;
            int n = numberOfVariables;
            f = Function(x);
            g = Gradient(x);
            int method = (int)Method;

            iterations = 0;
            evaluations = 1;
            bool bnew = true;
            int nrst = 0;
            int im = 0;   // Number of times betapr was negative for method 2 or 3
            searches = 0; // Number of line search iterations after Wolfe conditions were satisfied.
            double dg0 = 0;

            for (int i = 0; i < g.Length; ++i)
                d[i] = -g[i];

            double gnorm = Norm.Euclidean(g);
            double xnorm = Math.Max(1.0, Norm.Euclidean(x));
            double stp1 = 1.0 / gnorm;
            double f_old = f;


            bool finish = false;

            // Make initial progress report with initialization parameters
            if (Progress != null) Progress(this, new OptimizationProgressEventArgs
                (iterations, evaluations, g, gnorm, x, xnorm, f, stp1, finish));



            // Main iteration
            while (!finish)
            {
                iterations++;

                nrst++;

                // Call the line search routine of Mor'e and Thuente
                // (modified for Nocedal's CG method) 
                // ------------------------------------------------- 
                //
                //  J.J. Mor'e and D. Thuente, "Linesearch Algorithms with Guaranteed 
                //  Sufficient Decrease". ACM Transactions on Mathematical 
                //  Software 20 (1994), pp 286-307. 
                //

                int nfev = 0;
                int info = 0;
                double dgout = 0;

                // Save original gradient
                for (int i = 0; i < g.Length; i++)
                    gold[i] = g[i];

                double dg = Matrix.InnerProduct(d, g);
                double dgold = dg;
                double stp = 1.0;

                // Shanno-Phua's formula for trial step
                if (!bnew) stp = dg0 / dg;

                if (iterations == 1)
                    stp = stp1;

                int ides = 0;
                bnew = false;

            L72:

                // Call to the line search subroutine
                cvsmod(d, ref stp, ref info, ref nfev, w, ref dg, ref dgout);

                // Test if descent direction is obtained for methods 2 and 3
                double gg = Matrix.InnerProduct(g, g);
                double gg0 = Matrix.InnerProduct(g, gold);
                double betapr = (gg - gg0) / (gnorm * gnorm);

                // When nrst > n and irest == 1 then restart.
                if (irest == 1 && nrst > n)
                {
                    nrst = 0;
                    bnew = true;
                }
                else
                {
                    if (method != 1)
                    {
                        double dg1 = -gg + betapr * dgout;

                        if (dg1 >= 0.0)
                        {
                            ides++;

                            if (ides > 5)
                                throw new LineSearchFailedException("Descent was not obtained (-2)");

                            goto L72; // retry
                        }
                    }
                }

                evaluations += nfev;
                searches += ides;

                // Determine correct beta value for method chosen
                double betafr = gg / (gnorm * gnorm);
                double beta = 0;

                if (nrst == 0)
                {
                    beta = 0.0;
                }
                else
                {
                    if (method == 1)
                    {
                        beta = betafr;
                    }
                    else if (method == 2)
                    {
                        beta = betapr;
                    }
                    else if (method == 3)
                    {
                        beta = Math.Max(0.0, betapr);
                    }

                    if ((method == 2 || method == 3) && betapr < 0.0)
                    {
                        im++;
                    }
                }

                // Compute the new direction
                for (int i = 0; i < g.Length; i++)
                    d[i] = -g[i] + beta * d[i];

                dg0 = dgold * stp;

                // Check for termination
                gnorm = Norm.Euclidean(g);
                xnorm = Math.Max(1.0, Norm.Euclidean(x));

                // Convergence test
                if (gnorm / xnorm <= epsilon)
                    finish = true;

                // Stopping criteria by function delta
                if (tolerance > 0 && iterations > 1)
                {
                    double delta = (f_old - f) / f;
                    f_old = f;

                    if (delta < tolerance)
                        finish = true;
                }

                // Stopping criteria by max iterations
                if (maxIterations > 0)
                {
                    if (iterations > maxIterations)
                        finish = true;
                }

                if (Progress != null) Progress(this, new OptimizationProgressEventArgs
                            (iterations, evaluations, g, gnorm, x, xnorm, f, stp, finish));
            }

            return f;
        }



        // TODO: Move to separate classes

        bool brackt;
        bool stage1;
        double finit;
        double dgtest;
        double width;
        double width1;

        double stmin;
        double stmax;
        double dg2;
        double dgx;
        double dgy;
        int infoc;
        double stx;
        double fx;
        double sty;
        double fy;
        double ftest1;

        private unsafe int cvsmod(double[] s, ref double stp, ref int info,
             ref int nfev, double[] wa, ref double dginit, ref double dgout)
        {
            int n = numberOfVariables;

            if (info == 1)
                goto L321;

            infoc = 1;


            if (stp <= 0) // Check the input parameters for errors
                throw new LineSearchFailedException(1, "Invalid step size.");

            // Compute the initial gradient in the search direction
            // and check that S is a descent direction.

            if (dginit >= 0)
                throw new LineSearchFailedException(0, "The search direction is not a descent direction.");

            // Initialize local variables
            brackt = false;
            stage1 = true;
            nfev = 0;
            finit = f;
            dgtest = ftol * dginit;
            width = stpmax - stpmin;
            width1 = width / 0.5;

            for (int j = 0; j < x.Length; ++j)
                wa[j] = x[j];

            // The variables STX, FX, DGX contain the values of the step,
            //   function, and directional derivative at the best step.
            // The variables STY, FY, DGY contain the value of the step,
            //   function, and derivative at the other endpoint of the interval
            //   of uncertainty.
            // The variables STP, F, DG contain the values of the step,
            //   function, and derivative at the current step.

            stx = 0;
            fx = finit;
            dgx = dginit;
            sty = 0;
            fy = finit;
            dgy = dginit;


        L30: // Start of iteration.

            // Set the minimum and maximum steps to correspond
            // to the present interval of uncertainty.

            if (brackt)
            {
                stmin = Math.Min(stx, sty);
                stmax = Math.Max(stx, sty);
            }
            else
            {
                stmin = stx;
                stmax = stp + 4 * (stp - stx);
            }

            // Force the step to be within
            // the bounds STPMAX and STPMIN.

            stp = Math.Max(stp, stpmin);
            stp = Math.Min(stp, stpmax);

            // If an unusual termination is to occur then 
            // let STP be the lowest point obtained so far.

            if (brackt && (stp <= stmin || stp >= stmax) || nfev >= maxfev - 1 ||
                infoc == 0 || brackt && stmax - stmin <= xtol * stmax)
            {
                stp = stx;
            }

            // Evaluate the function and gradient at STP
            // and compute the directional derivative.

            for (int j = 0; j < s.Length; ++j)
                x[j] = wa[j] + stp * s[j];

            // Fetch function and gradient
            f = Function(x);
            g = Gradient(x);


            info = 0;
            nfev++;
            dg2 = 0;

            for (int j = 0; j < g.Length; ++j)
                dg2 += g[j] * s[j];

            ftest1 = finit + stp * dgtest;

            if ((brackt && (stp <= stmin || stp >= stmax)) || infoc == 0)
                throw new LineSearchFailedException(6, "Rounding errors prevent further progress." +
                    "There may not be a step which satisfies the sufficient decrease and curvature conditions. Tolerances may be too small.");

            if (stp == stpmax && f <= ftest1 && dg2 <= dgtest)
                throw new LineSearchFailedException(5, "The step size has reached the upper bound.");

            if (stp == stpmin && (f > ftest1 || dg2 >= dgtest))
                throw new LineSearchFailedException(4, "The step size has reached the lower bound.");

            if (nfev >= maxfev)
                throw new LineSearchFailedException(3, "Maximum number of function evaluations has been reached.");

            if (brackt && stmax - stmin <= xtol * stmax)
                throw new LineSearchFailedException(2, "Relative width of the interval of uncertainty is at machine precision.");


            // More's code has been modified so that at least one new 
            //  function value is computed during the line search (enforcing 
            //  at least one interpolation is not easy, since the code may 
            //  override an interpolation) 

            if (f <= ftest1 && Math.Abs(dg2) <= gtol * (-dginit) && nfev > 1)
            {
                info = 1;
                dgout = dg2;
                return 0;
            }


        L321:

            // In the first stage we seek a step for which the modified
            // function has a nonpositive value and nonnegative derivative.
            if (stage1 && f <= ftest1 && dg2 >= Math.Min(ftol, gtol) * dginit)
            {
                stage1 = false;
            }

            // A modified function is used to predict the step only if
            // we have not obtained a step for which the modified function
            // has a nonpositive function value and nonnegative derivative,
            // and if a lower function value has been obtained but the 
            // decrease is not sufficient.

            if (stage1 && f <= fx && f > ftest1)
            {

                // Define the modified function and derivative values
                double fm = f - stp * dgtest;
                double fxm = fx - stx * dgtest;
                double fym = fy - sty * dgtest;
                double dgm = dg2 - dgtest;
                double dgxm = dgx - dgtest;
                double dgym = dgy - dgtest;

                // Call CSTEPM to update the interval of
                // uncertainty and to compute the new step.

                BroydenFletcherGoldfarbShanno.SearchStep(ref stx, ref fxm, ref dgxm,
                    ref sty, ref fym, ref dgym, ref stp, fm, dgm, ref brackt, out infoc);

                // Reset the function and gradient values for f.
                fx = fxm + stx * dgtest;
                fy = fym + sty * dgtest;
                dgx = dgxm + dgtest;
                dgy = dgym + dgtest;
            }
            else
            {
                // Call CSTEPM to update the interval of
                // uncertainty and to compute the new step.
                BroydenFletcherGoldfarbShanno.SearchStep(ref stx, ref fx, ref dgx,
                    ref sty, ref fy, ref dgy, ref stp, f, dg2, ref brackt, out infoc);
            }

            // Force a sufficient decrease in the 
            // size of the interval of uncertainty.

            if (brackt)
            {
                if ((Math.Abs(sty - stx)) >= 0.66 * width1)
                    stp = stx + 0.5 * (sty - stx);

                width1 = width;
                width = Math.Abs(sty - stx);
            }

            goto L30;
        }

    }
}
