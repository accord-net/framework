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
    ///   Limited-memory Broyden–Fletcher–Goldfarb–Shanno (L-BFGS) optimization method.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The L-BFGS algorithm is a member of the broad family of quasi-Newton optimization
    ///   methods. L-BFGS stands for 'Limited memory BFGS'. Indeed, L-BFGS uses a limited
    ///   memory variation of the Broyden–Fletcher–Goldfarb–Shanno (BFGS) update to approximate
    ///   the inverse Hessian matrix (denoted by Hk). Unlike the original BFGS method which
    ///   stores a dense  approximation, L-BFGS stores only a few vectors that represent the
    ///   approximation implicitly. Due to its moderate memory requirement, L-BFGS method is
    ///   particularly well suited for optimization problems with a large number of variables.</para>
    /// <para>
    ///   L-BFGS never explicitly forms or stores Hk. Instead, it maintains a history of the past
    ///   <c>m</c> updates of the position <c>x</c> and gradient <c>g</c>, where generally the history
    ///   <c>m</c>can be short, often less than 10. These updates are used to implicitly do operations
    ///   requiring the Hk-vector product.</para>
    ///   
    /// <para>
    ///   The framework implementation of this method is based on the original FORTRAN source code
    ///   by Jorge Nocedal (see references below). The original FORTRAN source code of L-BFGS (for
    ///   unconstrained problems) is available at http://www.netlib.org/opt/lbfgs_um.shar and had
    ///   been made available under the public domain. </para>
    /// 
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://www.netlib.org/opt/lbfgs_um.shar">
    ///        Jorge Nocedal. Limited memory BFGS method for large scale optimization (Fortran source code). 1990.
    ///        Available in http://www.netlib.org/opt/lbfgs_um.shar </a></description></item>
    ///     <item><description>
    ///        Jorge Nocedal. Updating Quasi-Newton Matrices with Limited Storage. <i>Mathematics of Computation</i>,
    ///        Vol. 35, No. 151, pp. 773--782, 1980.</description></item>
    ///     <item><description>
    ///        Dong C. Liu, Jorge Nocedal. On the limited memory BFGS method for large scale optimization.</description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example shows the basic usage of the L-BFGS solver
    ///   to find the minimum of a function specifying its function and
    ///   gradient. </para>
    ///   
    /// <code>
    /// // Suppose we would like to find the minimum of the function
    /// // 
    /// //   f(x,y)  =  -exp{-(x-1)²} - exp{-(y-2)²/2}
    /// //
    /// 
    /// // First we need write down the function either as a named
    /// // method, an anonymous method or as a lambda function:
    /// 
    /// Func&lt;double[], double> f = (x) =>
    ///     -Math.Exp(-Math.Pow(x[0] - 1, 2)) - Math.Exp(-0.5 * Math.Pow(x[1] - 2, 2));
    /// 
    /// // Now, we need to write its gradient, which is just the
    /// // vector of first partial derivatives del_f / del_x, as:
    /// //
    /// //   g(x,y)  =  { del f / del x, del f / del y }
    /// // 
    /// 
    /// Func&lt;double[], double[]> g = (x) => new double[] 
    /// {
    ///     // df/dx = {-2 e^(-    (x-1)^2) (x-1)}
    ///     2 * Math.Exp(-Math.Pow(x[0] - 1, 2)) * (x[0] - 1),
    /// 
    ///     // df/dy = {-  e^(-1/2 (y-2)^2) (y-2)}
    ///     Math.Exp(-0.5 * Math.Pow(x[1] - 2, 2)) * (x[1] - 2)
    /// };
    /// 
    /// // Finally, we can create the L-BFGS solver, passing the functions as arguments
    /// var lbfgs = new BroydenFletcherGoldfarbShanno(numberOfVariables: 2, function: f, gradient: g);
    /// 
    /// // And then minimize the function:
    /// double minValue = lbfgs.Minimize();
    /// double[] solution = lbfgs.Solution;
    /// 
    /// // The resultant minimum value should be -2, and the solution
    /// // vector should be { 1.0, 2.0 }. The answer can be checked on
    /// // Wolfram Alpha by clicking the following the link:
    /// 
    /// // http://www.wolframalpha.com/input/?i=maximize+%28exp%28-%28x-1%29%C2%B2%29+%2B+exp%28-%28y-2%29%C2%B2%2F2%29%29
    /// 
    /// </code>
    /// </example>
    /// 
    public class BroydenFletcherGoldfarbShanno : IGradientOptimizationMethod
    {
        // those values need not be modified
        private const double ftol = 0.0001;
        private const double xtol = 1e-16; // machine precision
        private const double stpmin = 1e-20;
        private const double stpmax = 1e20;

        // Line search parameters
        private double gtol = 0.9;
        private int maxfev = 20;

        private double tolerance = 1e-10;
        private int iterations;
        private int evaluations;

        private int numberOfVariables;
        private int corrections = 5;

        private double[] x; // current solution x
        private double f;   // value at current solution f(x)
        double[] g;         // gradient at current solution

        private double[] lowerBound;
        private double[] upperBound;

        private double[] work;


        #region Properties

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
        ///   Gets or sets a function returning the Hessian
        ///   diagonals to be used during optimization.
        /// </summary>
        /// 
        /// <value>A function for the Hessian diagonal.</value>
        /// 
        public Func<double[]> Diagonal { get; set; }

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
        ///   Gets the number of iterations performed in the last
        ///   call to <see cref="Minimize()"/>.
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
        ///   Gets or sets the maximum number of iterations
        ///   to be performed during optimization. Default
        ///   is 0 (iterate until convergence).
        /// </summary>
        /// 
        public int MaxIterations
        {
            get;
            set;
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
        ///   Gets or sets the number of corrections used in the L-BFGS
        ///   update. Recommended values are between 3 and 7. Default is 5.
        /// </summary>
        /// 
        public int Corrections
        {
            get { return corrections; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");

                if (corrections != value)
                {
                    corrections = value;
                    createWorkVector();
                }
            }
        }

        /// <summary>
        ///   Gets or sets the upper bounds of the interval
        ///   in which the solution must be found.
        /// </summary>
        /// 
        public double[] UpperBounds
        {
            get { return upperBound; }
        }

        /// <summary>
        ///   Gets or sets the lower bounds of the interval
        ///   in which the solution must be found.
        /// </summary>
        /// 
        public double[] LowerBounds
        {
            get { return lowerBound; }
        }

        /// <summary>
        ///   Gets or sets the accuracy with which the solution
        ///   is to be found. Default value is 1e-10.
        /// </summary>
        /// 
        /// <remarks>
        ///   The optimization routine terminates when ||G|| &lt; EPS max(1,||X||),
        ///   where ||.|| denotes the Euclidean norm and EPS is the value for this
        ///   property.
        /// </remarks>
        /// 
        public double Tolerance
        {
            get { return tolerance; }
            set { tolerance = value; }
        }

        /// <summary>
        ///   Gets or sets a tolerance value controlling the accuracy of the
        ///   line search routine. If the function and gradient evaluations are
        ///   inexpensive with respect to the cost of the iteration (which is
        ///   sometimes the case when solving very large problems) it may be
        ///   advantageous to set this to a small value. A typical small value
        ///   is 0.1. This value should be greater than 1e-4. Default is 0.9.
        /// </summary>
        /// 
        public double Precision
        {
            get { return gtol; }
            set
            {
                if (value <= 1e-4)
                    throw new ArgumentOutOfRangeException("value");

                gtol = value;
            }
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

        #endregion

        #region Constructors

        /// <summary>
        ///   Creates a new instance of the L-BFGS optimization algorithm.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of free parameters in the optimization problem.</param>
        /// 
        public BroydenFletcherGoldfarbShanno(int numberOfVariables)
        {
            if (numberOfVariables <= 0)
                throw new ArgumentOutOfRangeException("numberOfVariables");

            this.numberOfVariables = numberOfVariables;

            this.createWorkVector();

            this.upperBound = new double[numberOfVariables];
            this.lowerBound = new double[numberOfVariables];

            for (int i = 0; i < upperBound.Length; i++)
                lowerBound[i] = Double.NegativeInfinity;

            for (int i = 0; i < upperBound.Length; i++)
                upperBound[i] = Double.PositiveInfinity;

            x = new double[numberOfVariables];
            for (int i = 0; i < x.Length; i++)
                x[i] = Accord.Math.Tools.Random.NextDouble() * 2.0 - 1.0;
        }

        /// <summary>
        ///   Creates a new instance of the L-BFGS optimization algorithm.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of free parameters in the function to be optimized.</param>
        /// <param name="function">The function to be optimized.</param>
        /// <param name="gradient">The gradient of the function.</param>
        /// 
        public BroydenFletcherGoldfarbShanno(int numberOfVariables, Func<double[], double> function, Func<double[], double[]> gradient)
            : this(numberOfVariables)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            if (gradient == null)
                throw new ArgumentNullException("gradient");

            this.Function = function;
            this.Gradient = gradient;

        }

        /// <summary>
        ///   Creates a new instance of the L-BFGS optimization algorithm.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of free parameters in the function to be optimized.</param>
        /// <param name="function">The function to be optimized.</param>
        /// <param name="gradient">The gradient of the function.</param>
        /// <param name="diagonal">The diagonal of the Hessian.</param>
        /// 
        public BroydenFletcherGoldfarbShanno(int numberOfVariables, Func<double[], double> function, Func<double[], double[]> gradient, Func<double[]> diagonal)
            : this(numberOfVariables, function, gradient)
        {
            this.Diagonal = diagonal;
        }
        #endregion


        /// <summary>
        ///   Minimizes the defined function. 
        /// </summary>
        /// 
        /// <returns>The minimum value found at the <see cref="Solution"/>.</returns>
        /// 
        public double Minimize()
        {
            return minimize();
        }

        /// <summary>
        ///   Minimizes the defined function. 
        /// </summary>
        /// 
        /// <param name="values">The initial guess values for the parameters. Default is the zero vector.</param>
        /// 
        /// <returns>The minimum value found at the <see cref="Solution"/>.</returns>
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

        private unsafe double minimize()
        {
            if (Function == null) throw new InvalidOperationException(
                "The function to be minimized has not been defined.");

            if (Gradient == null) throw new InvalidOperationException(
                "The gradient function has not been defined.");


            // Initialization
            int n = numberOfVariables, m = corrections;

            // Make initial evaluation
            f = getFunction(x);
            g = getGradient(x);

            this.iterations = 0;
            this.evaluations = 1;


            // Obtain initial Hessian
            double[] diagonal = null;

            if (Diagonal != null)
            {
                diagonal = getDiagonal();
            }
            else
            {
                diagonal = new double[n];
                for (int i = 0; i < diagonal.Length; i++)
                    diagonal[i] = 1.0;
            }


            fixed (double* w = work)
            {
                // The first N locations of the work vector are used to
                //  store the gradient and other temporary information.

                double* rho = &w[n];                   // Stores the scalars rho.
                double* alpha = &w[n + m];             // Stores the alphas in computation of H*g.
                double* steps = &w[n + 2 * m];         // Stores the last M search steps.
                double* delta = &w[n + 2 * m + n * m]; // Stores the last M gradient differences.


                // Initialize work vector
                for (int i = 0; i < g.Length; i++)
                    steps[i] = -g[i] * diagonal[i];

                // Initialize statistics
                double gnorm = Norm.Euclidean(g);
                double xnorm = Norm.Euclidean(x);
                double stp = 1.0 / gnorm;
                double stp1 = stp;

                // Initialize loop
                int nfev, point = 0;
                int npt = 0, cp = 0;
                bool finish = false;

                // Make initial progress report with initialization parameters
                if (Progress != null) Progress(this, new OptimizationProgressEventArgs
                    (iterations, evaluations, g, gnorm, x, xnorm, f, stp, finish));


                // Start main
                while (!finish)
                {
                    iterations++;
                    double bound = iterations - 1;

                    if (iterations != 1)
                    {
                        if (iterations > m)
                            bound = m;

                        double ys = 0;
                        for (int i = 0; i < n; i++)
                            ys += delta[npt + i] * steps[npt + i];

                        // Compute the diagonal of the Hessian
                        // or use an approximation by the user.

                        if (Diagonal != null)
                        {
                            diagonal = getDiagonal();
                        }
                        else
                        {
                            double yy = 0;
                            for (int i = 0; i < n; i++)
                                yy += delta[npt + i] * delta[npt + i];
                            double d = ys / yy;

                            for (int i = 0; i < n; i++)
                                diagonal[i] = d;
                        }


                        // Compute -H*g using the formula given in:
                        //   Nocedal, J. 1980, "Updating quasi-Newton matrices with limited storage",
                        //   Mathematics of Computation, Vol.24, No.151, pp. 773-782.

                        cp = (point == 0) ? m : point;
                        rho[cp - 1] = 1.0 / ys;
                        for (int i = 0; i < n; i++)
                            w[i] = -g[i];

                        cp = point;
                        for (int i = 1; i <= bound; i += 1)
                        {
                            if (--cp == -1) cp = m - 1;

                            double sq = 0;
                            for (int j = 0; j < n; j++)
                                sq += steps[cp * n + j] * w[j];

                            double beta = alpha[cp] = rho[cp] * sq;
                            for (int j = 0; j < n; j++)
                                w[j] -= beta * delta[cp * n + j];
                        }

                        for (int i = 0; i < diagonal.Length; i++)
                            w[i] *= diagonal[i];

                        for (int i = 1; i <= bound; i += 1)
                        {
                            double yr = 0;
                            for (int j = 0; j < n; j++)
                                yr += delta[cp * n + j] * w[j];

                            double beta = alpha[cp] - rho[cp] * yr;
                            for (int j = 0; j < n; j++)
                                w[j] += beta * steps[cp * n + j];

                            if (++cp == m) cp = 0;
                        }

                        npt = point * n;

                        // Store the search direction
                        for (int i = 0; i < n; i++)
                            steps[npt + i] = w[i];

                        stp = 1;
                    }

                    // Save original gradient
                    for (int i = 0; i < g.Length; i++)
                        w[i] = g[i];


                    // Obtain the one-dimensional minimizer of f by computing a line search
                    mcsrch(x, ref f, ref g, &steps[point * n], ref stp, out nfev, diagonal);

                    // Register evaluations
                    evaluations += nfev;

                    // Compute the new step and
                    // new gradient differences
                    for (int i = 0; i < g.Length; i++)
                    {
                        steps[npt + i] *= stp;
                        delta[npt + i] = g[i] - w[i];
                    }

                    if (++point == m) point = 0;


                    // Check for termination
                    gnorm = Norm.Euclidean(g);
                    xnorm = Norm.Euclidean(x);
                    xnorm = Math.Max(1.0, xnorm);

                    if (gnorm / xnorm <= tolerance)
                        finish = true;

                    if (Progress != null) Progress(this, new OptimizationProgressEventArgs
                        (iterations, evaluations, g, gnorm, x, xnorm, f, stp, finish));
                }
            }

            return f; // return the minimum value found (at solution x)
        }


        #region Line Search (mcsrch)

        /// <summary>
        ///   Finds a step which satisfies a sufficient decrease and curvature condition.
        /// </summary>
        /// 
        private unsafe void mcsrch(double[] x, ref double f, ref double[] g, double* s,
            ref double stp, out int nfev, double[] wa)
        {
            int n = numberOfVariables;
            double ftest1 = 0;
            int infoc = 1;

            nfev = 0;

            if (stp <= 0)
                throw new LineSearchFailedException(1, "Invalid step size.");

            // Compute the initial gradient in the search direction
            // and check that s is a descent direction.

            double dginit = 0;

            for (int j = 0; j < g.Length; j++)
                dginit = dginit + g[j] * s[j];

            if (dginit >= 0)
                throw new LineSearchFailedException(0, "The search direction is not a descent direction.");

            bool brackt = false;
            bool stage1 = true;

            double finit = f;
            double dgtest = ftol * dginit;
            double width = stpmax - stpmin;
            double width1 = width / 0.5;

            for (int j = 0; j < x.Length; j++)
                wa[j] = x[j];

            // The variables stx, fx, dgx contain the values of the
            // step, function, and directional derivative at the best
            // step.

            double stx = 0;
            double fx = finit;
            double dgx = dginit;

            // The variables sty, fy, dgy contain the value of the
            // step, function, and derivative at the other endpoint
            // of the interval of uncertainty.

            double sty = 0;
            double fy = finit;
            double dgy = dginit;

            // The variables stp, f, dg contain the values of the step,
            // function, and derivative at the current step.

            double dg = 0;


            while (true)
            {
                // Set the minimum and maximum steps to correspond
                // to the present interval of uncertainty.

                double stmin, stmax;

                if (brackt)
                {
                    stmin = Math.Min(stx, sty);
                    stmax = Math.Max(stx, sty);
                }
                else
                {
                    stmin = stx;
                    stmax = stp + 4.0 * (stp - stx);
                }

                // Force the step to be within the bounds stpmax and stpmin.

                stp = Math.Max(stp, stpmin);
                stp = Math.Min(stp, stpmax);

                // If an unusual termination is to occur then let
                // stp be the lowest point obtained so far.

                if ((brackt && (stp <= stmin || stp >= stmax)) ||
                    (brackt && stmax - stmin <= xtol * stmax) ||
                    (nfev >= maxfev - 1) || (infoc == 0))
                    stp = stx;

                // Evaluate the function and gradient at stp
                // and compute the directional derivative.
                // We return to main program to obtain F and G.

                for (int j = 0; j < x.Length; j++)
                {
                    x[j] = wa[j] + stp * s[j];

                    if (x[j] > upperBound[j])
                        x[j] = upperBound[j];
                    else if (x[j] < lowerBound[j])
                        x[j] = lowerBound[j];
                }


                // Reevaluate function and gradient
                f = getFunction(x);
                g = getGradient(x);

                nfev++;
                dg = 0;

                for (int j = 0; j < g.Length; j++)
                    dg = dg + g[j] * s[j];

                ftest1 = finit + stp * dgtest;

                // Test for convergence.

                if (nfev >= maxfev)
                    throw new LineSearchFailedException(3, "Maximum number of function evaluations has been reached.");

                if ((brackt && (stp <= stmin || stp >= stmax)) || infoc == 0)
                    throw new LineSearchFailedException(6, "Rounding errors prevent further progress." +
                        "There may not be a step which satisfies the sufficient decrease and curvature conditions. Tolerances may be too small.");

                if (stp == stpmax && f <= ftest1 && dg <= dgtest)
                    throw new LineSearchFailedException(5, "The step size has reached the upper bound.");

                if (stp == stpmin && (f > ftest1 || dg >= dgtest))
                    throw new LineSearchFailedException(4, "The step size has reached the lower bound.");

                if (brackt && stmax - stmin <= xtol * stmax)
                    throw new LineSearchFailedException(2, "Relative width of the interval of uncertainty is at machine precision.");

                if (f <= ftest1 && Math.Abs(dg) <= gtol * (-dginit))
                    return;

                // Not converged yet. Continuing with the search.

                // In the first stage we seek a step for which the modified
                // function has a nonpositive value and nonnegative derivative.

                if (stage1 && f <= ftest1 && dg >= Math.Min(ftol, gtol) * dginit)
                    stage1 = false;

                // A modified function is used to predict the step only if we
                // have not obtained a step for which the modified function has
                // a nonpositive function value and nonnegative derivative, and
                // if a lower function value has been obtained but the decrease
                // is not sufficient.

                if (stage1 && f <= fx && f > ftest1)
                {
                    // Define the modified function and derivative values.

                    double fm = f - stp * dgtest;
                    double fxm = fx - stx * dgtest;
                    double fym = fy - sty * dgtest;

                    double dgm = dg - dgtest;
                    double dgxm = dgx - dgtest;
                    double dgym = dgy - dgtest;

                    // Call cstep to update the interval of uncertainty
                    // and to compute the new step.

                    SearchStep(ref stx, ref fxm, ref dgxm,
                        ref sty, ref fym, ref dgym, ref stp,
                        fm, dgm, ref brackt, out infoc);

                    // Reset the function and gradient values for f.
                    fx = fxm + stx * dgtest;
                    fy = fym + sty * dgtest;
                    dgx = dgxm + dgtest;
                    dgy = dgym + dgtest;
                }
                else
                {
                    // Call mcstep to update the interval of uncertainty
                    // and to compute the new step.

                    SearchStep(ref stx, ref fx, ref dgx,
                        ref sty, ref fy, ref dgy, ref stp,
                        f, dg, ref brackt, out infoc);
                }

                // Force a sufficient decrease in the size of the
                // interval of uncertainty.

                if (brackt)
                {
                    if (Math.Abs(sty - stx) >= 0.66 * width1)
                        stp = stx + 0.5 * (sty - stx);

                    width1 = width;
                    width = Math.Abs(sty - stx);
                }

            }
        }

        // TODO: Move to separate classes
        internal static void SearchStep(ref double stx, ref double fx, ref double dx,
                                   ref double sty, ref double fy, ref double dy,
                                   ref double stp, double fp, double dp,
                                   ref bool brackt, out int info)
        {
            bool bound;
            double stpc, stpf, stpq;

            info = 0;

            if ((brackt && (stp <= Math.Min(stx, sty) || stp >= Math.Max(stx, sty))) ||
                (dx * (stp - stx) >= 0.0) || (stpmax < stpmin)) return;

            // Determine if the derivatives have opposite sign.
            double sgnd = dp * (dx / Math.Abs(dx));

            if (fp > fx)
            {
                // First case. A higher function value.
                // The minimum is bracketed. If the cubic step is closer
                // to stx than the quadratic step, the cubic step is taken,
                // else the average of the cubic and quadratic steps is taken.

                info = 1;
                bound = true;
                double theta = 3.0 * (fx - fp) / (stp - stx) + dx + dp;
                double s = Math.Max(Math.Abs(theta), Math.Max(Math.Abs(dx), Math.Abs(dp)));
                double gamma = s * Math.Sqrt((theta / s) * (theta / s) - (dx / s) * (dp / s));

                if (stp < stx) gamma = -gamma;

                double p = gamma - dx + theta;
                double q = gamma - dx + gamma + dp;
                double r = p / q;
                stpc = stx + r * (stp - stx);
                stpq = stx + ((dx / ((fx - fp) / (stp - stx) + dx)) / 2) * (stp - stx);

                if (Math.Abs(stpc - stx) < Math.Abs(stpq - stx))
                    stpf = stpc;
                else
                    stpf = stpc + (stpq - stpc) / 2.0;

                brackt = true;
            }
            else if (sgnd < 0.0)
            {
                // Second case. A lower function value and derivatives of
                // opposite sign. The minimum is bracketed. If the cubic
                // step is closer to stx than the quadratic (secant) step,
                // the cubic step is taken, else the quadratic step is taken.

                info = 2;
                bound = false;
                double theta = 3 * (fx - fp) / (stp - stx) + dx + dp;
                double s = Math.Max(Math.Abs(theta), Math.Max(Math.Abs(dx), Math.Abs(dp)));
                double gamma = s * Math.Sqrt((theta / s) * (theta / s) - (dx / s) * (dp / s));

                if (stp > stx) gamma = -gamma;

                double p = (gamma - dp) + theta;
                double q = ((gamma - dp) + gamma) + dx;
                double r = p / q;
                stpc = stp + r * (stx - stp);
                stpq = stp + (dp / (dp - dx)) * (stx - stp);

                if (Math.Abs(stpc - stp) > Math.Abs(stpq - stp))
                    stpf = stpc;
                else stpf = stpq;

                brackt = true;
            }
            else if (Math.Abs(dp) < Math.Abs(dx))
            {
                // Third case. A lower function value, derivatives of the
                // same sign, and the magnitude of the derivative decreases.
                // The cubic step is only used if the cubic tends to infinity
                // in the direction of the step or if the minimum of the cubic
                // is beyond stp. Otherwise the cubic step is defined to be
                // either stpmin or stpmax. The quadratic (secant) step is also
                // computed and if the minimum is bracketed then the step
                // closest to stx is taken, else the step farthest away is taken.

                info = 3;
                bound = true;
                double theta = 3 * (fx - fp) / (stp - stx) + dx + dp;
                double s = Math.Max(Math.Abs(theta), Math.Max(Math.Abs(dx), Math.Abs(dp)));
                double gamma = s * Math.Sqrt(Math.Max(0, (theta / s) * (theta / s) - (dx / s) * (dp / s)));

                if (stp > stx) gamma = -gamma;

                double p = (gamma - dp) + theta;
                double q = (gamma + (dx - dp)) + gamma;
                double r = p / q;

                if (r < 0.0 && gamma != 0.0)
                    stpc = stp + r * (stx - stp);
                else if (stp > stx)
                    stpc = stpmax;
                else stpc = stpmin;

                stpq = stp + (dp / (dp - dx)) * (stx - stp);

                if (brackt)
                {
                    if (Math.Abs(stp - stpc) < Math.Abs(stp - stpq))
                        stpf = stpc;
                    else stpf = stpq;
                }
                else
                {
                    if (Math.Abs(stp - stpc) > Math.Abs(stp - stpq))
                        stpf = stpc;
                    else stpf = stpq;
                }
            }
            else
            {
                // Fourth case. A lower function value, derivatives of the
                // same sign, and the magnitude of the derivative does
                // not decrease. If the minimum is not bracketed, the step
                // is either stpmin or stpmax, else the cubic step is taken.

                info = 4;
                bound = false;

                if (brackt)
                {
                    double theta = 3 * (fp - fy) / (sty - stp) + dy + dp;
                    double s = Math.Max(Math.Abs(theta), Math.Max(Math.Abs(dy), Math.Abs(dp)));
                    double gamma = s * Math.Sqrt((theta / s) * (theta / s) - (dy / s) * (dp / s));

                    if (stp > sty) gamma = -gamma;

                    double p = (gamma - dp) + theta;
                    double q = ((gamma - dp) + gamma) + dy;
                    double r = p / q;
                    stpc = stp + r * (sty - stp);
                    stpf = stpc;
                }
                else if (stp > stx)
                    stpf = stpmax;
                else stpf = stpmin;
            }

            // Update the interval of uncertainty. This update does not
            // depend on the new step or the case analysis above.

            if (fp > fx)
            {
                sty = stp;
                fy = fp;
                dy = dp;
            }
            else
            {
                if (sgnd < 0.0)
                {
                    sty = stx;
                    fy = fx;
                    dy = dx;
                }
                stx = stp;
                fx = fp;
                dx = dp;
            }

            // Compute the new step and safeguard it.
            stpf = Math.Min(stpmax, stpf);
            stpf = Math.Max(stpmin, stpf);
            stp = stpf;

            if (brackt && bound)
            {
                if (sty > stx)
                    stp = Math.Min(stx + 0.66 * (sty - stx), stp);
                else
                    stp = Math.Max(stx + 0.66 * (sty - stx), stp);
            }

            return;
        }


        #endregion


        private double[] getDiagonal()
        {
            double[] diag = Diagonal();
            if (diag.Length != numberOfVariables) throw new ArgumentException(
                "The length of the Hessian diagonal vector does not match the" +
                " number of free parameters in the optimization poblem.");
            for (int i = 0; i < diag.Length; i++)
                if (diag[i] <= 0) throw new ArgumentException(
                    "One of the diagonal elements of the inverse" +
                    " Hessian approximation is not strictly positive");
            return diag;
        }

        private double[] getGradient(double[] args)
        {
            double[] grad = Gradient(args);
            if (grad.Length != numberOfVariables) throw new ArgumentException(
                "The length of the gradient vector does not match the" +
                " number of free parameters in the optimization problem.");
            return grad;
        }

        private double getFunction(double[] args)
        {
            double func = Function(args);
            if (Double.IsNaN(func) || Double.IsInfinity(func))
                throw new NotFiniteNumberException(
                    "The function evaluation did not return a finite number.", func);
            return func;
        }

        private void createWorkVector()
        {
            this.work = new double[numberOfVariables * (2 * corrections + 1) + 2 * corrections];
        }


    }
}
