// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
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
    using System.ComponentModel;
    using Accord.Compat;

    /// <summary>
    ///   Status codes for the <see cref="BroydenFletcherGoldfarbShanno"/>
    ///   function optimizer.
    /// </summary>
    /// 
    public enum BroydenFletcherGoldfarbShannoStatus
    {
        /// <summary>
        ///   Convergence was attained.
        /// </summary>
        /// 
        [Description("LBFGS_SUCCESS")]
        Success = 0,

        /// <summary>
        ///   The optimization stopped before convergence; maximum
        ///   number of iterations could have been reached.
        /// </summary>
        /// 
        [Description("LBFGS_STOP")]
        Stop = 1,

        /// <summary>
        ///   The function is already at a minimum.
        /// </summary>
        /// 
        [Description("LBFGS_ALREADY_MINIMIZED")]
        AlreadyMinimized,

        /// <summary>
        ///   Unknown error.
        /// </summary>
        /// 
        [Description("LBFGSERR_UNKNOWNERROR")]
        UnknownError = -1024,

        /// <summary>
        ///   The line-search step went out of the interval of uncertainty.
        /// </summary>
        /// 
        [Description("LBFGSERR_OUTOFINTERVAL")]
        OutOfInterval = -1003,

        /// <summary>
        ///   A logic error occurred; alternatively, the interval of uncertainty became too small.
        /// </summary>
        /// 
        [Description("LBFGSERR_INCORRECT_TMINMAX")]
        IncorrectMinMax,

        /// <summary>
        ///   A rounding error occurred; alternatively, no line-search step satisfies
        ///   the sufficient decrease and curvature conditions. The line search routine
        ///   will terminate with this code if the relative width of the interval of 
        ///   uncertainty is less than <see cref="BroydenFletcherGoldfarbShanno.FunctionTolerance"/>.
        /// </summary>
        /// 
        [Description("LBFGSERR_ROUNDING_ERROR")]
        RoundingError,

        /// <summary>
        ///   The line-search step became smaller than <see cref=" BroydenFletcherGoldfarbShanno.MinStep"/>.
        /// </summary>
        /// 
        [Description("LBFGSERR_MINIMUMSTEP")]
        MinimumStep,

        /// <summary>
        ///    The line-search step became larger than <see cref=" BroydenFletcherGoldfarbShanno.MaxStep"/>.
        /// </summary>
        /// 
        [Description("LBFGSERR_MAXIMUMSTEP")]
        MaximumStep,

        /// <summary>
        ///   The line-search routine reaches the maximum number of evaluations.
        /// </summary>
        /// 
        [Description("LBFGSERR_MAXIMUMLINESEARCH")]
        MaximumLineSearch,

        /// <summary>
        ///   Maximum number of iterations was reached.
        /// </summary>
        /// 
        [Description("LBFGSERR_MAXIMUMITERATION")]
        MaximumIterations,

        /// <summary>
        ///   Relative width of the interval of uncertainty is at most 
        ///   <see cref="BroydenFletcherGoldfarbShanno.FunctionTolerance"/>.
        /// </summary>
        ///
        [Description("LBFGSERR_WIDTHTOOSMALL")]
        IntervalWidthTooSmall,

        /// <summary>
        ///   A logic error (negative line-search step) occurred. This
        ///   could be an indication that something could be wrong with
        ///   the gradient function.
        /// </summary>
        /// 
        [Description("LBFGSERR_INVALIDPARAMETERS")]
        InvalidParameters,

        /// <summary>
        ///   The current search direction increases the objective function value.
        /// </summary>
        /// 
        [Description("LBFGSERR_INCREASEGRADIENT")]
        IncreaseGradient,
    }

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
        ///   -f(x + a * d) ≤ f(x) + FunctionTolerance * a * g(x)^T d,</code>
        /// <para>
        ///   where x is the current point, d is the current search direction, and
        ///   a is the step length.</para>
        /// </remarks>
        /// 
        BacktrackingArmijo = 1,

        /// <summary>
        ///   Backtracking method with regular Wolfe condition. 
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        ///   The backtracking method finds the step length such that it satisfies
        ///   both the Armijo condition (LineSearch.LBFGS_LINESEARCH_BACKTRACKING_ARMIJO)
        ///   and the curvature condition,</para>
        ///  <code>
        ///   - g(x + a * d)^T d ≥ lbfgs_parameter_t::wolfe * g(x)^T d,
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
        ///     - |g(x + a * d)^T d| ≤ lbfgs_parameter_t::wolfe * |g(x)^T d|,</code>
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
    /// bool success = lbfgs.Minimize();
    /// double minValue = lbfgs.Value;
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
    /// <seealso cref="ConjugateGradient"/>
    /// <seealso cref="ResilientBackpropagation"/>
    /// <seealso cref="BoundedBroydenFletcherGoldfarbShanno"/>
    /// <seealso cref="TrustRegionNewtonMethod"/>
    /// 
    public class BroydenFletcherGoldfarbShanno : BaseGradientOptimizationMethod,
        IGradientOptimizationMethod, IOptimizationMethod<BroydenFletcherGoldfarbShannoStatus>
    {

        // parameters
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



        #region Properties

        /// <summary>
        ///   The number of corrections to approximate the inverse Hessian matrix.
        ///   Default is 6. Values less than 3 are not recommended. Large values 
        ///   will result in excessive computing time.
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
                {
                    throw ArgumentException("value",
                        "The number of corrections must be greater than zero.",
                        "LBFGSERR_INVALID_M");
                }

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
                {
                    throw ArgumentException("value",
                        "Epsilon should be positive or zero.",
                        "LBFGSERR_INVALID_EPSILON");
                }

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
                {
                    throw ArgumentException("value",
                        "Past should be positive or zero.",
                        "LBFGSERR_INVALID_TESTPERIOD");
                }

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
                {
                    throw ArgumentException("value",
                        "Delta should be positive or zero.",
                        "LBFGSERR_INVALID_DELTA");
                }

                delta = value;
            }
        }

        /// <summary>
        ///    The maximum number of iterations.
        /// </summary>
        /// 
        /// <remarks>
        ///   The minimize function terminates an optimization process with 
        ///   <see cref="BroydenFletcherGoldfarbShannoStatus.MaximumIterations"/> status
        ///   code when the iteration count exceeds this parameter. Setting this parameter
        ///   to zero continues an optimization process until a convergence or error. The
        ///   default value is 0.</remarks>
        /// 
        public int MaxIterations
        {
            get { return max_iterations; }
            set
            {
                if (value < 0)
                {
                    throw ArgumentException("value",
                        "Maximum number of iterations must be positive or zero.",
                       "LBFGSERR_MAXIMUMITERATION");
                }

                max_iterations = value;
            }
        }

        /// <summary>
        ///   The line search algorithm. 
        /// </summary>
        /// 
        /// <remarks>
        ///   This parameter specifies a line search 
        ///   algorithm to be used by the L-BFGS routine.
        /// </remarks>
        /// 
        public LineSearch LineSearch
        {
            get { return linesearch; }
            set
            {
                if (!Enum.IsDefined(typeof(LineSearch), value))
                {
                    throw ArgumentException("value",
                        "Invalid line-search method.",
                        "LBFGSERR_INVALID_LINESEARCH");
                }

                linesearch = value;
            }
        }

        /// <summary>
        ///   The maximum number of trials for the line search.
        /// </summary>
        ///   
        /// <remarks>
        ///   This parameter controls the number of function and gradients evaluations 
        ///   per iteration for the line search routine. The default value is <c>20</c>.
        /// </remarks>
        /// 
        public int MaxLineSearch
        {
            get { return max_linesearch; }
            set
            {
                if (value <= 0)
                {
                    throw ArgumentException("value",
                        "Maximum line searches must be greater than zero.",
                        "LBFGSERR_INVALID_MAXLINESEARCH");
                }

                max_linesearch = value;
            }
        }

        /// <summary>
        ///   The minimum step of the line search routine.
        /// </summary>
        ///  
        /// <remarks>
        ///   The default value is <c>1e-20</c>. This value need not be modified unless 
        ///   the exponents are too large for the machine being used, or unless the problem
        ///   is extremely badly scaled (in which case the exponents should be increased).
        /// </remarks>
        /// 
        public double MinStep
        {
            get { return min_step; }
            set
            {
                if (value < 0)
                {
                    throw ArgumentException("value",
                        "Minimum step must be greater than zero and less than the maximum step",
                        "LBFGSERR_INVALID_MINSTEP");
                }

                min_step = value;
            }
        }

        /// <summary>
        ///   The maximum step of the line search.
        /// </summary>
        /// 
        /// <remarks>
        ///   The default value is <c>1e+20</c>. This value need not be modified unless the
        ///   exponents are too large for the machine being used, or unless the problem is 
        ///   extremely badly scaled (in which case the exponents should be increased).
        /// </remarks>
        /// 
        public double MaxStep
        {
            get { return max_step; }
            set
            {
                if (value < 0)
                {
                    throw ArgumentException("value",
                        "Maximum step must be greater than the minimum step",
                        "LBFGSERR_INVALID_MAXSTEP");
                }

                max_step = value;
            }
        }

        /// <summary>
        ///  A parameter to control the accuracy of the line search routine. The default 
        ///  value is <c>1e-4</c>. This parameter should be greater than zero and smaller 
        ///  than <c>0.5</c>.
        /// </summary>
        /// 
        public double ParameterTolerance
        {
            get { return ftol; }
            set
            {
                if (value < 0 || value > 0.5)
                {
                    throw ArgumentException("value",
                        "Parameter tolerance must be greater than zero and smaller than 0.5.",
                        "LBFGSERR_INVALID_FTOL");
                }

                ftol = value;
            }
        }

        /// <summary>
        ///   A coefficient for the Wolfe condition.
        /// </summary>
        /// 
        /// <remarks>
        ///   This parameter is valid only when the backtracking line-search algorithm is used 
        ///   with the Wolfe condition, <see cref="Accord.Math.Optimization.LineSearch.StrongWolfe"/> 
        ///   or <see cref="Accord.Math.Optimization.LineSearch.RegularWolfe"/>. The default value 
        ///   is <c>0.9</c>. This parameter should be greater the <see cref="ParameterTolerance"/> 
        ///   and smaller than <c>1.0</c>.
        /// </remarks>
        /// 
        public double Wolfe
        {
            get { return wolfe; }
            set
            {
                if (wolfe > 1.0)
                {
                    throw ArgumentException("value",
                        "Wolfe parameter must be smaller than 1.0.",
                        "LBFGSERR_INVALID_WOLFE");
                }

                wolfe = value;
            }
        }

        /// <summary>
        ///   A parameter to control the accuracy of the line search routine.
        /// </summary>
        /// 
        /// <remarks>
        ///   The default value is <c>0.9</c>. If the function and gradient evaluations are
        ///   inexpensive with respect to the cost of the iteration (which is sometimes the
        ///   case when solving very large problems) it may be advantageous to set this parameter
        ///   to a small value. A typical small value is <c>0.1</c>. This parameter should be
        ///   greater than the <see cref="ParameterTolerance"/> (<c>1e-4</c>) and smaller than
        ///   <c>1.0.</c>
        /// </remarks>
        /// 
        public double GradientTolerance
        {
            get { return gtol; }
            set
            {
                if (value < 0)
                {
                    throw ArgumentException("value",
                        "Gradient tolerance must be positive or zero.",
                        "LBFGSERR_INVALID_GTOL");
                }

                gtol = value;
            }
        }

        /// <summary>
        ///   The machine precision for floating-point values.
        /// </summary>
        /// 
        /// <remarks>
        ///   This parameter must be a positive value set by a client program to
        ///   estimate the machine precision. The line search routine will terminate
        ///   with the status code (::LBFGSERR_ROUNDING_ERROR) if the relative width
        ///   of the interval of uncertainty is less than this parameter.
        /// </remarks>
        /// 
        public double FunctionTolerance
        {
            get { return xtol; }
            set
            {
                if (value < 0)
                {
                    throw ArgumentException("value",
                        "Function tolerance must be positive or zero.",
                        "LBFGSERR_INVALID_XTOL");
                }

                xtol = value;
            }
        }

        /// <summary>
        ///   Coefficient for the L1 norm of variables.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        ///   This parameter should be set to zero for standard minimization problems. Setting this
        ///   parameter to a positive value activates Orthant-Wise Limited-memory Quasi-Newton (OWL-QN)
        ///   method, which minimizes the objective function F(x) combined with the L1 norm |x| of the
        ///   variables, <c>{F(x) + C |x|}</c>. This parameter is the coefficient for the |x|, i.e., C.</para>
        ///   
        /// <para>
        ///   As the L1 norm |x| is not differentiable at zero, the library modifies function and 
        ///   gradient evaluations from a client program suitably; a client program thus have only 
        ///   to return the function value F(x) and gradients G(x) as usual. The default value is zero.</para>
        /// </remarks>
        /// 
        public double OrthantwiseC
        {
            get { return orthantwise_c; }
            set
            {
                if (value < 0)
                {
                    throw ArgumentException("value",
                        "Orthant-wise C should be positive or zero.",
                        "LBFGSERR_INVALID_ORTHANTWISE");
                }

                orthantwise_c = value;
            }
        }


        /// <summary>
        ///    Start index for computing L1 norm of the variables.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        ///   This parameter is valid only for OWL-QN method (i.e., <see cref="OrthantwiseC"/> != 0).
        ///   This parameter b (0 &lt;= b &lt; N) specifies the index number from which the library 
        ///   computes the L1 norm of the variables x,</para>
        /// <code>
        ///     |x| := |x_{b}| + |x_{b+1}| + ... + |x_{N}|.</code>
        /// <para>
        ///   In other words, variables x_1, ..., x_{b-1} are not used for
        ///   computing the L1 norm. Setting b (0 &lt; b &lt; N), one can protect
        ///   variables, x_1, ..., x_{b-1} (e.g., a bias term of logistic
        ///   regression) from being regularized. The default value is zero.</para>
        /// </remarks>
        /// 
        public int OrthantwiseStart
        {
            get { return orthantwise_start; }
            set
            {
                if (value < 0 || value > NumberOfVariables)
                {
                    throw ArgumentException("value",
                        "Value must be between 0 and the number of variables in the problem.",
                        "LBFGSERR_INVALID_ORTHANTWISE_START");
                }

                orthantwise_start = value;
            }
        }

        /// <summary>
        ///   End index for computing L1 norm of the variables.
        /// </summary>
        /// 
        /// <remarks>
        ///   This parameter is valid only for OWL-QN method (i.e., <see cref="OrthantwiseC"/> != 0).
        ///   This parameter e (0 &lt; e &lt;= N) specifies the index number at which the library stops
        ///   computing the L1 norm of the variables x,
        /// <code>
        ///     |x| := |x_{b}| + |x_{b+1}| + ... + |x_{N}|.</code>
        /// </remarks>
        /// 
        public int OrthantwiseEnd
        {
            get { return orthantwise_end; }
            set
            {
                if (value > NumberOfVariables)
                {
                    throw ArgumentException("value",
                        "Value must be between 0 and the number of variables in the problem.",
                        "LBFGSERR_INVALID_ORTHANTWISE_END");
                }

                if (value < 0)
                    value = NumberOfVariables;

                orthantwise_end = value;
            }
        }

        /// <summary>
        ///   Occurs when progress is made during the optimization.
        /// </summary>
        /// 
        public event EventHandler<OptimizationProgressEventArgs> Progress;

        /// <summary>
        ///   Get the exit code returned in the last call to the
        ///   <see cref="IOptimizationMethod{TInput, TOutput}.Maximize()"/> or 
        ///   <see cref="IOptimizationMethod{TInput, TOutput}.Minimize()"/> methods.
        /// </summary>
        /// 
        public BroydenFletcherGoldfarbShannoStatus Status { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///   Creates a new instance of the L-BFGS optimization algorithm.
        /// </summary>
        /// 
        public BroydenFletcherGoldfarbShanno()
            : base()
        {
        }

        /// <summary>
        ///   Creates a new instance of the L-BFGS optimization algorithm.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of free parameters in the optimization problem.</param>
        /// 
        public BroydenFletcherGoldfarbShanno(int numberOfVariables)
            : base(numberOfVariables)
        {
        }

        /// <summary>
        ///   Creates a new instance of the L-BFGS optimization algorithm.
        /// </summary>
        /// 
        /// <param name="function">The function to be optimized.</param>
        /// 
        public BroydenFletcherGoldfarbShanno(NonlinearObjectiveFunction function)
            : this(function.NumberOfVariables, function.Function, function.Gradient)
        {
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
            : base(numberOfVariables, function, gradient)
        {
        }

        #endregion

        /// <summary>
        /// Called when the <see cref="IOptimizationMethod{TInput, TOutput}.NumberOfVariables" /> property has changed.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of variables.</param>
        /// 
        protected override void OnNumberOfVariablesChanged(int numberOfVariables)
        {
            base.OnNumberOfVariablesChanged(numberOfVariables);

            this.orthantwise_end = numberOfVariables;
        }



        /// <summary>
        ///   Implements the actual optimization algorithm. This
        ///   method should try to minimize the objective function.
        /// </summary>
        /// 
        protected override bool Optimize()
        {
            if (LineSearch == Optimization.LineSearch.RegularWolfe ||
                LineSearch == Optimization.LineSearch.StrongWolfe)
            {
                if (wolfe <= ftol || 1.0 <= wolfe)
                    throw OperationException("Wolfe tolerance must be between 'ParameterTolerance' and 1.", "LBFGSERR_INVALID_WOLFE");
            }

            if (OrthantwiseC != 0.0 && linesearch != Optimization.LineSearch.RegularWolfe)
            {
                throw OperationException("Orthant-wise updates are only available with Regular Wolfe line search.",
                    "LBFGSERR_INVALID_LINESEARCH");
            }

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

            LBFGS.Code ret = (LBFGS.Code)LBFGS.main(Solution, Function, Gradient, Progress, param);

            Status = (BroydenFletcherGoldfarbShannoStatus)ret;

            if (!Enum.IsDefined(typeof(BroydenFletcherGoldfarbShannoStatus), Status))
                throw new InvalidOperationException("Unhandled return code: " + ret);


            return Status == BroydenFletcherGoldfarbShannoStatus.Success ||
                   Status == BroydenFletcherGoldfarbShannoStatus.AlreadyMinimized;
        }


    }
}
