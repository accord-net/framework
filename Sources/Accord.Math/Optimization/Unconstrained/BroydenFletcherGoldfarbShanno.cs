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
// Copyright © 2007-2010 Naoaki Okazaki
// http://www.chokkan.org/software/liblbfgs/
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
// This class has been based on the C library of Limited memory BFGS (L-BFGS).
//
//      C library of Limited memory BFGS (L-BFGS).
//
// Copyright (c) 1990, Jorge Nocedal
// Copyright (c) 2007-2010 Naoaki Okazaki
// All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

namespace Accord.Math.Optimization
{
    using System;

    /// <summary>
    ///   Line search algorithms.
    /// </summary>
    /// 
    public enum LineSearch
    {
        /// <summary>
        ///   More-Thuente method.
        /// </summary>
        /// 
        Default = 0,

        /// <summary>
        ///   Backtracking method with the Armijo condition.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        ///   The backtracking method finds the step length such that it satisfies
        ///   the sufficient decrease (Armijo) condition,</para>
        /// <code>
        ///   - f(x + a * d) &lte; f(x) + FunctionTolerance * a * g(x)^T d,</code>
        /// <para>
        ///   where x is the current point, d is the current search direction, and
        ///   a is the step length.</para>
        /// </remarks>
        /// 
        BacktrackingArmijo = 1,

        /// <summary>
        ///   Backtracking method with regular Wolfe condition. 
        /// </summary>
        /// <remarks>
        /// <para>
        ///   The backtracking method finds the step length such that it satisfies
        ///   both the Armijo condition (LineSearch.LBFGS_LINESEARCH_BACKTRACKING_ARMIJO)
        ///   and the curvature condition,</para>
        ///  <code>
        ///   - g(x + a * d)^T d >= lbfgs_parameter_t::wolfe * g(x)^T d,
        ///  </code>
        ///    where x is the current point, d is the current search direction, and
        ///    a is the step length.
        /// </remarks>
        /// 
        RegularWolfe = 2,

        /// <summary>
        ///   Backtracking method with strong Wolfe condition. 
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        ///   The backtracking method finds the step length such that it satisfies
        ///   both the Armijo condition (LineSearch.LBFGS_LINESEARCH_BACKTRACKING_ARMIJO)
        ///   and the following condition,</para>
        /// <code>
        ///     - |g(x + a * d)^T d| &lte; lbfgs_parameter_t::wolfe * |g(x)^T d|,</code>
        /// <para>
        ///   where x is the current point, d is the current search direction, and
        ///   a is the step length.</para>
        /// </remarks>
        /// 
        StrongWolfe = 3,
    };

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

        // parameters
        private int n;
        private int m = 6;
        private double epsilon = 1e-5;
        private int past = 0;
        private double delta = 1e-5;
        private int max_iterations = 0;
        private LineSearch linesearch = LineSearch.Default;
        private int max_linesearch = 40;
        private double min_step = 1e-20;
        private double max_step = 1e20;
        private double ftol = 1e-4;
        private double wolfe = 0.9;
        private double gtol = 0.9;
        private double xtol = 1.0e-16;
        private double orthantwise_c = 0;
        private int orthantwise_start = 0;
        private int orthantwise_end = -1;


        // outputs
        private double[] x;
        private double f;

        public enum Code
        {
            MaximumIterations
        }

        #region Properties

        /// <summary>
        ///   The number of corrections to approximate the inverse Hessian matrix.
        /// </summary>
        /// 
        /// <remarks>
        ///   The L-BFGS routine stores the computation results of the previous <c>m</c>
        ///   iterations to approximate the inverse Hessian matrix of the current
        ///   iteration. This parameter controls the size of the limited memories
        ///   (corrections). The default value is 6. Values less than 3 are not 
        ///   recommended. Large values will result in excessive computing time.
        /// </remarks>
        /// 
        public int Corrections
        {
            get { return m; }
            set
            {
                if (value <= 0)
                    throw exception("The number of corrections must be greater than zero.", "LBFGSERR_INVALID_M");
                m = value;
            }
        }

        /// <summary>
        ///   Epsilon for convergence test.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        ///   This parameter determines the accuracy with which the solution is to
        ///   be found. A minimization terminates when</para>
        /// <code>
        ///       ||g|| &lt; epsilon * max(1, ||x||),</code>
        /// <para>
        ///   where ||.|| denotes the Euclidean (L2) norm. The default value is 1e-5.</para>
        /// </remarks>
        /// 
        public double Epsilon
        {
            get { return epsilon; }
            set
            {
                if (value < 0)
                    throw exception("Epsilon should be positive or zero.", "LBFGSERR_INVALID_EPSILON");
                epsilon = value;
            }
        }

        /// <summary>
        ///   Distance for delta-based convergence test.
        /// </summary>
        /// 
        /// <remarks>
        ///   This parameter determines the distance, in iterations, to compute
        ///   the rate of decrease of the objective function. If the value of this
        ///   parameter is zero, the library does not perform the delta-based
        ///   convergence test. The default value is 0.
        /// </remarks>
        /// 
        public int Past
        {
            get { return past; }
            set
            {
                if (value < 0)
                    throw exception("Past should be positive or zero.", "LBFGSERR_INVALID_TESTPERIOD");
                past = value;
            }
        }

        /// <summary>
        ///   Delta for convergence test.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        ///   This parameter determines the minimum rate of decrease of the
        ///   objective function. The library stops iterations when the
        ///   following condition is met:</para>
        ///   <code>
        ///      (f' - f) / f &lt;  delta
        ///   </code>
        ///   <para>
        ///   where f' is the objective value of <see cref="Past">past iterations</see>
        ///   ago, and f is the objective value of the current iteration. Default value 
        ///   is 0.</para>
        /// </remarks>
        /// 
        public double Delta
        {
            get { return delta; }
            set
            {
                if (value < 0)
                    throw exception("Delta should be positive or zero.", "LBFGSERR_INVALID_DELTA");
                delta = value;
            }
        }

        /// <summary>
        ///    The maximum number of iterations.
        /// </summary>
        /// 
        /// <remarks>
        ///   The minimize function terminates an optimization process with <see
        ///   cref="Code.MaximumIterations"/> status code when the iteration count 
        ///   exceeds this parameter. Setting this parameter to zero continues an
        ///   optimization process until a convergence or error. The default value
        ///   is 0.</remarks>
        /// 
        public int MaxIterations
        {
            get { return max_iterations; }
            set
            {
                if (value <= 0)
                    throw exception("Maximum number of iterations must be positive or zero.", String.Empty);
                max_iterations = value;
            }
        }

        public LineSearch LineSearch
        {
            get { return linesearch; }
            set { linesearch = value; }
        }

        public int MaxLineSearch
        {
            get { return max_linesearch; }
            set
            {
                if (value <= 0)
                    throw exception("Maximum line searches must be greater than zero.", "LBFGSERR_INVALID_MAXLINESEARCH");
                max_linesearch = value;
            }
        }

        public double MinStep
        {
            get { return min_step; }
            set
            {
                if (value < 0)
                    throw exception("Minimum step must be greater than zero and less than the maximum step",
                        "LBFGSERR_INVALID_MINSTEP");
                min_step = value;
            }
        }
        public double MaxStep
        {
            get { return max_step; }
            set
            {
                if (value < 0)
                    throw exception("Maximum step must be greater than the minimum step", "LBFGSERR_INVALID_MAXSTEP");
                max_step = value;
            }
        }
        public double ParameterTolerance
        {
            get { return ftol; }
            set
            {
                if (value < 0)
                    throw exception("Parameter tolerance must be positive or zero.", "LBFGSERR_INVALID_FTOL");
                ftol = value;
            }
        }
        public double Wolfe
        {
            get { return wolfe; }
            set { wolfe = value; }
        }
        public double GradientTolerance
        {
            get { return gtol; }
            set
            {
                if (value < 0)
                    throw exception("Gradient tolerance must be positive or zero.", "LBFGSERR_INVALID_GTOL");
                gtol = value;
            }
        }
        public double FunctionTolerance
        {
            get { return xtol; }
            set
            {
                if (value < 0)
                    throw exception("Function tolerance must be positive or zero.", "LBFGSERR_INVALID_XTOL");
                xtol = value;
            }
        }
        public double OrthantwiseC
        {
            get { return orthantwise_c; }
            set
            {
                if (value < 0)
                    throw exception("Orthantwise C should be positive or zero.", "LBFGSERR_INVALID_ORTHANTWISE");
                orthantwise_c = value;
            }
        }



        public int OrthantwiseStart
        {
            get { return orthantwise_start; }
            set { orthantwise_start = value; }
        }

        public int OrthantwiseEnd
        {
            get { return orthantwise_end; }
            set { orthantwise_end = value; }
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
            get { return n; }
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

            this.n = numberOfVariables;
            x = new double[numberOfVariables];
        }

        /// <summary>
        ///   Creates a new instance of the L-BFGS optimization algorithm.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of free parameters in the function to be optimized.</param>
        /// <param name="function">The function to be optimized.</param>
        /// <param name="gradient">The gradient of the function.</param>
        /// 
        public BroydenFletcherGoldfarbShanno(int numberOfVariables,
            Func<double[], double> function, Func<double[], double[]> gradient)
            : this(numberOfVariables)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            if (gradient == null)
                throw new ArgumentNullException("gradient");

            this.Function = function;
            this.Gradient = gradient;
        }

        #endregion

        /// <summary>
        ///   Minimizes the defined function. 
        /// </summary>
        /// 
        /// <param name="values">The initial guess values for the parameters. Default is the zero vector.</param>
        /// 
        /// <returns>The minimum value found at the <see cref="Solution"/>.</returns>
        /// 
        public double Minimize()
        {
            return Minimize(Solution);
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

            var param = new lbfgs_parameter_t()
            {
                m = m,
                epsilon = epsilon,
                past = past,
                delta = delta,
                max_iterations = max_iterations,
                linesearch = linesearch,
                max_linesearch = max_linesearch,
                min_step = min_step,
                max_step = max_step,
                ftol = ftol,
                wolfe = wolfe,
                gtol = gtol,
                xtol = xtol,
                orthantwise_c = orthantwise_c,
                orthantwise_start = orthantwise_start,
                orthantwise_end = orthantwise_end,
            };

            int ret = LBFGS.main(values, Function, Gradient, Progress, param);

            for (int i = 0; i < values.Length; i++)
                x[i] = values[i];

            return Function(values);
        }



        private static ArgumentOutOfRangeException exception(string message, string code)
        {
            var e = new ArgumentOutOfRangeException("value", message);
            e.Data["Code"] = code;
            return e;
        }
    }
}
