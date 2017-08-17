/*
 *      Limited memory BFGS (L-BFGS).
 *
 * Copyright (c) 1990, Jorge Nocedal
 * Copyright (c) 2007-2010 Naoaki Okazaki
 * All rights reserved.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */


/*
This library is a C port of the FORTRAN implementation of Limited-memory
Broyden-Fletcher-Goldfarb-Shanno (L-BFGS) method written by Jorge Nocedal.
The original FORTRAN source code is available at:
http://www.ece.northwestern.edu/~nocedal/lbfgs.html

The L-BFGS algorithm is described in:
    - Jorge Nocedal.
      Updating Quasi-Newton Matrices with Limited Storage.
      <i>Mathematics of Computation</i>, Vol. 35, No. 151, pp. 773--782, 1980.
    - Dong C. Liu and Jorge Nocedal.
      On the limited memory BFGS method for large scale optimization.
      <i>Mathematical Programming</i> B, Vol. 45, No. 3, pp. 503-528, 1989.

The line search algorithms used in this implementation are described in:
    - John E. Dennis and Robert B. Schnabel.
      <i>Numerical Methods for Unrained Optimization and Nonlinear
      Equations</i>, Englewood Cliffs, 1983.
    - Jorge J. More and David J. Thuente.
      Line search algorithm with guaranteed sufficient decrease.
      <i>ACM Transactions on Mathematical Software (TOMS)</i>, Vol. 20, No. 3,
      pp. 286-307, 1994.

This library also implements Orthant-Wise Limited-memory Quasi-Newton (OWL-QN)
method presented in:
    - Galen Andrew and Jianfeng Gao.
      Scalable training of L1-regularized log-linear models.
      In <i>Proceedings of the 24th International Conference on Machine
      Learning (ICML 2007)</i>, pp. 33-40, 2007.

I would like to thank the original author, Jorge Nocedal, who has been
distributing the efficient and explanatory implementation in an open source
license.
                                                        -- Naoaki Okazaki 
*/

namespace Accord.Math.Optimization
{
    using System;
    using Accord.Compat;

    internal class lbfgs_parameter_t
    {
        public int m;
        public double epsilon;
        public int past;
        public double delta;
        public int max_iterations;
        public Accord.Math.Optimization.LineSearch linesearch;
        public int max_linesearch;
        public double min_step;
        public double max_step;
        public double ftol;
        public double wolfe;
        public double gtol;
        public double xtol;
        public double orthantwise_c;
        public int orthantwise_start;
        public int orthantwise_end;
    };

    internal static class LBFGS
    {


        internal static int main(double[] start, Func<double[], double> fn, Func<double[], double[]> gn,
            EventHandler<OptimizationProgressEventArgs> progress, lbfgs_parameter_t param)
        {
            double fx = 0;

            lbfgs_evaluate_t target = NewMethod(fn, gn);
            lbfgs_progress_t prog = ProgressMethod(progress);


            /*
                Start the L-BFGS optimization; this will invoke the callback functions
                evaluate() and progress() when necessary.
             */
            var ret = lbfgs(start.Length, start, ref fx, target,
                prog, null, param);

            if ((int)ret < 0)
                fx = fn(start);

            if (progress != null)
                progress(null, new OptimizationProgressEventArgs(0, 0, null, 0, start, 0, fx, 0, false));

            //lbfgs_free(x);
            return (int)ret;
        }

        private static lbfgs_progress_t ProgressMethod(EventHandler<OptimizationProgressEventArgs> progress)
        {
            lbfgs_progress_t target = (object instance,
             double[] x,
             double[] g,
             double fx,
             double xnorm,
             double gnorm,
             double step,
            int n,
            int k,
            Code ls
            ) =>
            {
                if (progress != null)
                    progress(null,
                        new OptimizationProgressEventArgs(0, 0,
                        (double[])g.Clone(),
                        gnorm,
                        (double[])x.Clone(),
                        xnorm,
                        fx, step, false)
                        {
                            Tag = Tuple.Create(n, k, ls)
                        });
                return 0;
            };

            return target;
        }

        private static lbfgs_evaluate_t NewMethod(Func<double[], double> fn, Func<double[], double[]> gn)
        {

            lbfgs_evaluate_t target = (object instance,
                double[] x,
                double[] g,
                int n,
                double step
                ) =>
            {
                double r = fn(x);
                double[] newg = gn(x);

                for (int j = 0; j < newg.Length; j++)
                    g[j] = newg[j];

                return r;
            };
            return target;
        }


        static bool fsigndiff(double x, double y)
        {
            return ((x) * ((y) / Math.Abs((y))) < 0.0);
        }

        static double[] vecalloc(int size)
        {
            return new double[size];
        }
        /*
                static void vecset(double[] x, double c, int n)
                {
                    int i;

                    for (i = 0; i < n; ++i)
                    {
                        x[i] = c;
                    }
                }
        */
        static void veccpy(double[] y, double[] x, int n)
        {
            int i;

            for (i = 0; i < n; ++i)
            {
                y[i] = x[i];
            }
        }

        static void vecncpy(double[] y, double[] x, int n)
        {
            int i;

            for (i = 0; i < n; ++i)
            {
                y[i] = -x[i];
            }
        }

        static void vecadd(double[] y, double[] x, double c, int n)
        {
            int i;

            for (i = 0; i < n; ++i)
            {
                y[i] += c * x[i];
            }
        }

        static void vecdiff(double[] z, double[] x, double[] y, int n)
        {
            int i;

            for (i = 0; i < n; ++i)
            {
                z[i] = x[i] - y[i];
            }
        }

        static void vecscale(double[] y, double c, int n)
        {
            int i;

            for (i = 0; i < n; ++i)
            {
                y[i] *= c;
            }
        }
/*
        static void vecmul(double[] y, double[] x, int n)
        {
            int i;

            for (i = 0; i < n; ++i)
            {
                y[i] *= x[i];
            }
        }
*/
        static void vecdot(out double s, double[] x, double[] y, int n)
        {
            int i;
            s = 0.0;
            for (i = 0; i < n; ++i)
            {
                s += x[i] * y[i];
            }
        }

        static void vec2norm(out double s, double[] x, int n)
        {
            vecdot(out s, x, x, n);
            s = (double)System.Math.Sqrt(s);
        }

        static void vec2norminv(out double s, double[] x, int n)
        {
            vec2norm(out s, x, n);
            s = (double)(1.0 / s);
        }

        /**
         * Return values of lbfgs().
         * 
         *  Roughly speaking, a negative value indicates an error.
         */
        internal enum Code
        {
            /** L-BFGS reaches convergence. */
            LBFGS_SUCCESS = 0,
            LBFGS_CONVERGENCE = 0,
            LBFGS_STOP,
            /** The initial variables already minimize the objective function. */
            LBFGS_ALREADY_MINIMIZED,

            /** Unknown error. */
            LBFGSERR_UNKNOWNERROR = -1024,
            /** Logic error. */
            LBFGSERR_LOGICERROR,
            /** Insufficient memory. */
            LBFGSERR_OUTOFMEMORY,
            /** The minimization process has been canceled. */
            LBFGSERR_CANCELED,
            /** Invalid number of variables specified. */
            LBFGSERR_INVALID_N,
            /** Invalid number of variables (for SSE) specified. */
            LBFGSERR_INVALID_N_SSE,
            /** The array x must be aligned to 16 (for SSE). */
            LBFGSERR_INVALID_X_SSE,
            /** Invalid parameter lbfgs_parameter_t::epsilon specified. */
            LBFGSERR_INVALID_EPSILON,
            /** Invalid parameter lbfgs_parameter_t::past specified. */
            LBFGSERR_INVALID_TESTPERIOD,
            /** Invalid parameter lbfgs_parameter_t::delta specified. */
            LBFGSERR_INVALID_DELTA,
            /** Invalid parameter lbfgs_parameter_t::linesearch specified. */
            LBFGSERR_INVALID_LINESEARCH,
            /** Invalid parameter lbfgs_parameter_t::max_step specified. */
            LBFGSERR_INVALID_MINSTEP,
            /** Invalid parameter lbfgs_parameter_t::max_step specified. */
            LBFGSERR_INVALID_MAXSTEP,
            /** Invalid parameter lbfgs_parameter_t::ftol specified. */
            LBFGSERR_INVALID_FTOL,
            /** Invalid parameter lbfgs_parameter_t::wolfe specified. */
            LBFGSERR_INVALID_WOLFE,
            /** Invalid parameter lbfgs_parameter_t::gtol specified. */
            LBFGSERR_INVALID_GTOL,
            /** Invalid parameter lbfgs_parameter_t::xtol specified. */
            LBFGSERR_INVALID_XTOL,
            /** Invalid parameter lbfgs_parameter_t::max_linesearch specified. */
            LBFGSERR_INVALID_MAXLINESEARCH,
            /** Invalid parameter lbfgs_parameter_t::orthantwise_c specified. */
            LBFGSERR_INVALID_ORTHANTWISE,
            /** Invalid parameter lbfgs_parameter_t::orthantwise_start specified. */
            LBFGSERR_INVALID_ORTHANTWISE_START,
            /** Invalid parameter lbfgs_parameter_t::orthantwise_end specified. */
            LBFGSERR_INVALID_ORTHANTWISE_END,
            /** The line-search step went out of the interval of uncertainty. */
            LBFGSERR_OUTOFINTERVAL,
            /** A logic error occurred; alternatively, the interval of uncertainty
                became too small. */
            LBFGSERR_INCORRECT_TMINMAX,
            /** A rounding error occurred; alternatively, no line-search step
                satisfies the sufficient decrease and curvature conditions. */
            LBFGSERR_ROUNDING_ERROR,
            /** The line-search step became smaller than lbfgs_parameter_t::min_step. */
            LBFGSERR_MINIMUMSTEP,
            /** The line-search step became larger than lbfgs_parameter_t::max_step. */
            LBFGSERR_MAXIMUMSTEP,
            /** The line-search routine reaches the maximum number of evaluations. */
            LBFGSERR_MAXIMUMLINESEARCH,
            /** The algorithm routine reaches the maximum number of iterations. */
            LBFGSERR_MAXIMUMITERATION,
            /** Relative width of the interval of uncertainty is at most
                lbfgs_parameter_t::xtol. */
            LBFGSERR_WIDTHTOOSMALL,
            /** A logic error (negative line-search step) occurred. */
            LBFGSERR_INVALIDPARAMETERS,
            /** The current search direction increases the objective function value. */
            LBFGSERR_INCREASEGRADIENT,
        };


        delegate Code line_search_proc(
            int n,
            double[] x,
            ref double f,
            double[] g,
            double[] s,
            ref double stp,
             double[] xp,
             double[] gp,
            double[] wa,
            ref callback_data_t cd,
             ref lbfgs_parameter_t param
            );





        /**
         * Callback interface to provide objective function and gradient evaluations.
         *
         *  The lbfgs() function call this function to obtain the values of objective
         *  function and its gradients when needed. A client program must implement
         *  this function to evaluate the values of the objective function and its
         *  gradients, given current values of variables.
         *  
         *  @param  instance    The user data sent for lbfgs() function by the client.
         *  @param  x           The current values of variables.
         *  @param  g           The gradient vector. The callback function must compute
         *                      the gradient values for the current variables.
         *  @param  n           The number of variables.
         *  @param  step        The current step of the line search routine.
         *  @retval double The value of the objective function for the current
         *                          variables.
         */
        delegate double lbfgs_evaluate_t(
            object instance,
             double[] x,
            double[] g,
             int n,
             double step
            );

        /**
         * Callback interface to receive the progress of the optimization process.
         *
         *  The lbfgs() function call this function for each iteration. Implementing
         *  this function, a client program can store or display the current progress
         *  of the optimization process.
         *
         *  @param  instance    The user data sent for lbfgs() function by the client.
         *  @param  x           The current values of variables.
         *  @param  g           The current gradient values of variables.
         *  @param  fx          The current value of the objective function.
         *  @param  xnorm       The Euclidean norm of the variables.
         *  @param  gnorm       The Euclidean norm of the gradients.
         *  @param  step        The line-search step used for this iteration.
         *  @param  n           The number of variables.
         *  @param  k           The iteration count.
         *  @param  ls          The number of evaluations called for this iteration.
         *  @retval int         Zero to continue the optimization process. Returning a
         *                      non-zero value will cancel the optimization process.
         */
        delegate Code lbfgs_progress_t(
            object instance,
             double[] x,
             double[] g,
             double fx,
             double xnorm,
             double gnorm,
             double step,
            int n,
            int k,
            Code ls
            );

        class callback_data_t
        {
            public int n;
            public object instance;
            public lbfgs_evaluate_t proc_evaluate;
            public lbfgs_progress_t proc_progress;
        };

        class iteration_data_t
        {
            public double alpha;
            public double[] s;     /* [n] */
            public double[] y;     /* [n] */
            public double ys;     /* vecdot(y, s) */
        };

        static lbfgs_parameter_t _defparam = new lbfgs_parameter_t()
        {
            m = 6,
            epsilon = 1e-5,
            past = 0,
            delta = 1e-5,
            max_iterations = 0,
            linesearch = (int)LineSearch.Default,
            max_linesearch = 40,
            min_step = 1e-20,
            max_step = 1e20,
            ftol = 1e-4,
            wolfe = 0.9,
            gtol = 0.9,
            xtol = 1.0e-16,
            orthantwise_c = 0.0,
            orthantwise_start = 0,
            orthantwise_end = -1,
        };

        /*
                static double[] LBFGS_malloc(int n)
                {
                    return new double[n];
                }

                static void LBFGS_free(double[] x)
                {
                }
        */
        static Code lbfgs(
            int n,
            double[] x,
            ref double ptr_fx,
            lbfgs_evaluate_t proc_evaluate,
            lbfgs_progress_t proc_progress,
            object instance,
            lbfgs_parameter_t _param
            )
        {
            Code ret;
            int i, j, k, end, bound;
            Code ls;
            double step;

            /* ant parameters and their default values. */
            lbfgs_parameter_t param = (_param != null) ? (_param) : _defparam;
            int m = param.m;

            double[] xp = null;
            double[] g = null, gp = null, pg = null;
            double[] d = null, w = null, pf = null;
            iteration_data_t[] lm = null;
            iteration_data_t it = null;
            double ys, yy;
            double xnorm, gnorm, beta;
            double fx = 0.0;
            double rate = 0.0;
            line_search_proc linesearch = line_search_morethuente;

            /* ruct a callback data. */
            callback_data_t cd = new callback_data_t();
            cd.n = n;
            cd.instance = instance;
            cd.proc_evaluate = proc_evaluate;
            cd.proc_progress = proc_progress;



            if (param.max_step < param.min_step)
            {
                return Code.LBFGSERR_INVALID_MAXSTEP;
            }
            if (param.ftol < 0.0)
            {
                return Code.LBFGSERR_INVALID_FTOL;
            }


            if (param.linesearch == LineSearch.RegularWolfe ||
                param.linesearch == LineSearch.StrongWolfe)
            {
                if (param.wolfe <= param.ftol || 1.0 <= param.wolfe)
                {
                    return Code.LBFGSERR_INVALID_WOLFE;
                }
            }

            if (param.orthantwise_start < 0 || n < param.orthantwise_start)
            {
                return Code.LBFGSERR_INVALID_ORTHANTWISE_START;
            }
            if (param.orthantwise_end < 0)
            {
                param.orthantwise_end = n;
            }
            if (n < param.orthantwise_end)
            {
                return Code.LBFGSERR_INVALID_ORTHANTWISE_END;
            }

            if (param.orthantwise_c != 0.0)
            {
                switch (param.linesearch)
                {
                    case LineSearch.RegularWolfe:
                        linesearch = line_search_backtracking_owlqn;
                        break;
                    default:
                        /* Only the backtracking method is available. */
                        return Code.LBFGSERR_INVALID_LINESEARCH;
                }
            }
            else
            {
                switch (param.linesearch)
                {
                    case LineSearch.Default:
                        linesearch = line_search_morethuente;
                        break;
                    case LineSearch.BacktrackingArmijo:
                    case LineSearch.RegularWolfe:
                    case LineSearch.StrongWolfe:
                        linesearch = line_search_backtracking;
                        break;
                    default:
                        return Code.LBFGSERR_INVALID_LINESEARCH;
                }
            }

            /* Allocate working space. */
            xp = (double[])vecalloc(n * sizeof(double));
            g = (double[])vecalloc(n * sizeof(double));
            gp = (double[])vecalloc(n * sizeof(double));
            d = (double[])vecalloc(n * sizeof(double));
            w = (double[])vecalloc(n * sizeof(double));
            if (xp == null || g == null || gp == null || d == null || w == null)
            {
                ret = Code.LBFGSERR_OUTOFMEMORY;
                goto lbfgs_exit;
            }

            if (param.orthantwise_c != 0.0)
            {
                /* Allocate working space for OW-LQN. */
                pg = (double[])vecalloc(n * sizeof(double));
                if (pg == null)
                {
                    ret = Code.LBFGSERR_OUTOFMEMORY;
                    goto lbfgs_exit;
                }
            }

            /* Allocate limited memory storage. */
            lm = new iteration_data_t[m];

            if (lm == null)
            {
                ret = Code.LBFGSERR_OUTOFMEMORY;
                goto lbfgs_exit;
            }

            /* Initialize the limited memory. */
            for (i = 0; i < m; ++i)
            {
                it = lm[i] = new iteration_data_t();
                it.alpha = 0;
                it.ys = 0;
                it.s = (double[])vecalloc(n * sizeof(double));
                it.y = (double[])vecalloc(n * sizeof(double));
                if (it.s == null || it.y == null)
                {
                    ret = Code.LBFGSERR_OUTOFMEMORY;
                    goto lbfgs_exit;
                }
            }

            /* Allocate an array for storing previous values of the objective function. */
            if (0 < param.past)
            {
                pf = (double[])vecalloc(param.past * sizeof(double));
            }

            /* Evaluate the function value and its gradient. */
            fx = cd.proc_evaluate(cd.instance, x, g, cd.n, 0);
            if (0.0 != param.orthantwise_c)
            {
                /* Compute the L1 norm of the variable and add it to the object value. */
                xnorm = owlqn_x1norm(x, param.orthantwise_start, param.orthantwise_end);
                fx += xnorm * param.orthantwise_c;
                owlqn_pseudo_gradient(
                    pg, x, g, n,
                    param.orthantwise_c, param.orthantwise_start, param.orthantwise_end
                    );
            }

            /* Store the initial value of the objective function. */
            if (pf != null)
            {
                pf[0] = fx;
            }

            /*
                Compute the direction;
                we assume the initial hessian matrix H_0 as the identity matrix.
             */
            if (param.orthantwise_c == 0.0)
            {
                vecncpy(d, g, n);
            }
            else
            {
                vecncpy(d, pg, n);
            }

            /*
               Make sure that the initial variables are not a minimizer.
             */
            vec2norm(out xnorm, x, n);
            if (param.orthantwise_c == 0.0)
            {
                vec2norm(out gnorm, g, n);
            }
            else
            {
                vec2norm(out gnorm, pg, n);
            }
            if (xnorm < 1.0) xnorm = 1.0;
            if (gnorm / xnorm <= param.epsilon)
            {
                ret = Code.LBFGS_ALREADY_MINIMIZED;
                goto lbfgs_exit;
            }

            /* Compute the initial step:
                step = 1.0 / System.Math.Sqrt(vecdot(d, d, n))
             */
            vec2norminv(out step, d, n);

            k = 1;
            end = 0;
            for (; ; )
            {
                /* Store the current position and gradient vectors. */
                veccpy(xp, x, n);
                veccpy(gp, g, n);

                /* Search for an optimal step. */
                if (param.orthantwise_c == 0.0)
                {
                    ls = linesearch(n, x, ref fx, g,
                        d, ref step, xp, gp, w, ref cd, ref param);
                }
                else
                {
                    ls = linesearch(n, x, ref fx, g, d, ref step,
                        xp, pg, w, ref cd, ref param);
                    owlqn_pseudo_gradient(
                        pg, x, g, n,
                        param.orthantwise_c, param.orthantwise_start, param.orthantwise_end
                        );
                }
                if (ls < 0)
                {
                    /* Revert to the previous point. */
                    veccpy(x, xp, n);
                    veccpy(g, gp, n);
                    ret = (Code)ls;
                    goto lbfgs_exit;
                }

                /* Compute x and g norms. */
                vec2norm(out xnorm, x, n);
                if (param.orthantwise_c == 0.0)
                {
                    vec2norm(out gnorm, g, n);
                }
                else
                {
                    vec2norm(out gnorm, pg, n);
                }

                /* Report the progress. */
                if (cd.proc_progress != null)
                {
                    if ((ret = cd.proc_progress(cd.instance, x, g, fx, xnorm,
                        gnorm, step, cd.n, k, ls)) != 0)
                    {
                        goto lbfgs_exit;
                    }
                }

                /*
                    Convergence test.
                    The criterion is given by the following formula:
                        |g(x)| / \max(1, |x|) < \epsilon
                 */
                if (xnorm < 1.0)
                    xnorm = 1.0;

                if (Double.IsNaN(xnorm) || double.IsNaN(gnorm))
                {
                    ret = Code.LBFGSERR_UNKNOWNERROR;
                    break;
                }

                if (gnorm / xnorm <= param.epsilon)
                {
                    /* Convergence. */
                    ret = Code.LBFGS_SUCCESS;
                    break;
                }

                /*
                    Test for stopping criterion.
                    The criterion is given by the following formula:
                        (f(past_x) - f(x)) / f(x) < \delta
                 */
                if (pf != null)
                {
                    /* We don't test the stopping criterion while k < past. */
                    if (param.past <= k)
                    {
                        /* Compute the relative improvement from the past. */
                        rate = (pf[k % param.past] - fx) / fx;

                        /* The stopping criterion. */
                        if (rate < param.delta)
                        {
                            ret = Code.LBFGS_STOP;
                            break;
                        }
                    }

                    /* Store the current value of the objective function. */
                    pf[k % param.past] = fx;
                }

                if (param.max_iterations != 0 && param.max_iterations < k + 1)
                {
                    /* Maximum number of iterations. */
                    ret = Code.LBFGSERR_MAXIMUMITERATION;
                    break;
                }

                /*
                    Update vectors s and y:
                        s_{k+1} = x_{k+1} - x_{k} = \step * d_{k}.
                        y_{k+1} = g_{k+1} - g_{k}.
                 */
                it = lm[end];
                vecdiff(it.s, x, xp, n);
                vecdiff(it.y, g, gp, n);

                /*
                    Compute scalars ys and yy:
                        ys = y^t \cdot s = 1 / \rho.
                        yy = y^t \cdot y.
                    Notice that yy is used for scaling the hessian matrix H_0 (Cholesky factor).
                 */
                vecdot(out ys, it.y, it.s, n);
                vecdot(out yy, it.y, it.y, n);
                it.ys = ys;

                /*
                    Recursive formula to compute dir = -(H \cdot g).
                        This is described in page 779 of:
                        Jorge Nocedal.
                        Updating Quasi-Newton Matrices with Limited Storage.
                        Mathematics of Computation, Vol. 35, No. 151,
                        pp. 773--782, 1980.
                 */
                bound = (m <= k) ? m : k;
                ++k;
                end = (end + 1) % m;

                /* Compute the steepest direction. */
                if (param.orthantwise_c == 0.0)
                {
                    /* Compute the negative of gradients. */
                    vecncpy(d, g, n);
                }
                else
                {
                    vecncpy(d, pg, n);
                }

                j = end;
                for (i = 0; i < bound; ++i)
                {
                    j = (j + m - 1) % m;    /* if (--j == -1) j = m-1; */
                    it = lm[j];
                    /* \alpha_{j} = \rho_{j} s^{t}_{j} \cdot q_{k+1}. */
                    vecdot(out it.alpha, it.s, d, n);
                    it.alpha /= it.ys;
                    /* q_{i} = q_{i+1} - \alpha_{i} y_{i}. */
                    vecadd(d, it.y, -it.alpha, n);
                }

                vecscale(d, ys / yy, n);

                for (i = 0; i < bound; ++i)
                {
                    it = lm[j];
                    /* \beta_{j} = \rho_{j} y^t_{j} \cdot \gamma_{i}. */
                    vecdot(out beta, it.y, d, n);
                    beta /= it.ys;
                    /* \gamma_{i+1} = \gamma_{i} + (\alpha_{j} - \beta_{j}) s_{j}. */
                    vecadd(d, it.s, it.alpha - beta, n);
                    j = (j + 1) % m;        /* if (++j == m) j = 0; */
                }

                /*
                    rain the search direction for orthant-wise updates.
                 */
                if (param.orthantwise_c != 0.0)
                {
                    for (i = param.orthantwise_start; i < param.orthantwise_end; ++i)
                    {
                        if (d[i] * pg[i] >= 0)
                        {
                            d[i] = 0;
                        }
                    }
                }

                /*
                    Now the search direction d is ready. We try step = 1 first.
                 */
                step = 1.0;
            }

        lbfgs_exit:
            /* Return the final value of the objective function. */
            ptr_fx = fx;

            return ret;
        }



        static Code line_search_backtracking(
            int n,
            double[] x,
            ref double f,
            double[] g,
            double[] s,
            ref double stp,
             double[] xp,
             double[] gp,
            double[] wp,
            ref callback_data_t cd,
             ref lbfgs_parameter_t param
            )
        {
            int count = 0;
            double width, dg;
            double finit, dginit = 0.0, dgtest;
            double dec = 0.5, inc = 2.1;

            /* Check the input parameters for errors. */
            if (stp <= 0.0)
            {
                return Code.LBFGSERR_INVALIDPARAMETERS;
            }

            /* Compute the initial gradient in the search direction. */
            vecdot(out dginit, g, s, n);

            /* Make sure that s points to a descent direction. */
            if (0 < dginit)
            {
                return Code.LBFGSERR_INCREASEGRADIENT;
            }

            /* The initial value of the objective function. */
            finit = f;
            dgtest = param.ftol * dginit;

            for (; ; )
            {
                veccpy(x, xp, n);
                vecadd(x, s, stp, n);

                /* Evaluate the function and gradient values. */
                f = cd.proc_evaluate(cd.instance, x, g, cd.n, stp);

                ++count;

                if (f > finit + stp * dgtest)
                {
                    width = dec;
                }
                else
                {
                    /* The sufficient decrease condition (Armijo condition). */
                    if (param.linesearch == LineSearch.BacktrackingArmijo)
                    {
                        /* Exit with the Armijo condition. */
                        return (Code)count;
                    }

                    /* Check the Wolfe condition. */
                    vecdot(out dg, g, s, n);
                    if (dg < param.wolfe * dginit)
                    {
                        width = inc;
                    }
                    else
                    {
                        if (param.linesearch == LineSearch.RegularWolfe)
                        {
                            /* Exit with the regular Wolfe condition. */
                            return (Code)count;
                        }

                        /* Check the strong Wolfe condition. */
                        if (dg > -param.wolfe * dginit)
                        {
                            width = dec;
                        }
                        else
                        {
                            /* Exit with the strong Wolfe condition. */
                            return (Code)count;
                        }
                    }
                }

                if (stp < param.min_step)
                {
                    /* The step is the minimum value. */
                    return Code.LBFGSERR_MINIMUMSTEP;
                }
                if (stp > param.max_step)
                {
                    /* The step is the maximum value. */
                    return Code.LBFGSERR_MAXIMUMSTEP;
                }
                if (param.max_linesearch <= count)
                {
                    /* Maximum number of iteration. */
                    return Code.LBFGSERR_MAXIMUMLINESEARCH;
                }

                (stp) *= width;
            }
        }



        static Code line_search_backtracking_owlqn(
            int n,
            double[] x,
            ref double f,
            double[] g,
            double[] s,
            ref double stp,
             double[] xp,
             double[] gp,
            double[] wp,
            ref callback_data_t cd,
             ref lbfgs_parameter_t param
            )
        {
            int i, count = 0;
            double width = 0.5, norm = 0.0;
            double finit = f, dgtest;

            /* Check the input parameters for errors. */
            if (stp <= 0.0)
            {
                return Code.LBFGSERR_INVALIDPARAMETERS;
            }

            /* Choose the orthant for the new point. */
            for (i = 0; i < n; ++i)
            {
                wp[i] = (xp[i] == 0.0) ? -gp[i] : xp[i];
            }

            for (; ; )
            {
                /* Update the current point. */
                veccpy(x, xp, n);
                vecadd(x, s, stp, n);

                /* The current point is projected onto the orthant. */
                owlqn_project(x, wp, param.orthantwise_start, param.orthantwise_end);

                /* Evaluate the function and gradient values. */
                f = cd.proc_evaluate(cd.instance, x, g, cd.n, stp);

                /* Compute the L1 norm of the variables and add it to the object value. */
                norm = owlqn_x1norm(x, param.orthantwise_start, param.orthantwise_end);
                f += norm * param.orthantwise_c;

                ++count;

                dgtest = 0.0;
                for (i = 0; i < n; ++i)
                {
                    dgtest += (x[i] - xp[i]) * gp[i];
                }

                if (f <= finit + param.ftol * dgtest)
                {
                    /* The sufficient decrease condition. */
                    return (Code)count;
                }

                if (stp < param.min_step)
                {
                    /* The step is the minimum value. */
                    return Code.LBFGSERR_MINIMUMSTEP;
                }
                if (stp > param.max_step)
                {
                    /* The step is the maximum value. */
                    return Code.LBFGSERR_MAXIMUMSTEP;
                }
                if (param.max_linesearch <= count)
                {
                    /* Maximum number of iteration. */
                    return Code.LBFGSERR_MAXIMUMLINESEARCH;
                }

                stp *= width;
            }
        }



        static Code line_search_morethuente(
            int n,
            double[] x,
            ref double f,
            double[] g,
            double[] s,
            ref double stp,
             double[] xp,
             double[] gp,
            double[] wa,
            ref callback_data_t cd,
             ref lbfgs_parameter_t param
            )
        {
            int count = 0;
            int brackt, stage1;
            Code uinfo = 0;
            double dg;
            double stx, fx, dgx;
            double sty, fy, dgy;
            double fxm, dgxm, fym, dgym, fm, dgm;
            double finit, ftest1, dginit, dgtest;
            double width, prev_width;
            double stmin, stmax;

            /* Check the input parameters for errors. */
            if (stp <= 0.0)
            {
                return Code.LBFGSERR_INVALIDPARAMETERS;
            }

            /* Compute the initial gradient in the search direction. */
            vecdot(out dginit, g, s, n);

            /* Make sure that s points to a descent direction. */
            if (0 < dginit)
            {
                return Code.LBFGSERR_INCREASEGRADIENT;
            }

            /* Initialize local variables. */
            brackt = 0;
            stage1 = 1;
            finit = f;
            dgtest = param.ftol * dginit;
            width = param.max_step - param.min_step;
            prev_width = 2.0 * width;

            /*
                The variables stx, fx, dgx contain the values of the step,
                function, and directional derivative at the best step.
                The variables sty, fy, dgy contain the value of the step,
                function, and derivative at the other endpoint of
                the interval of uncertainty.
                The variables stp, f, dg contain the values of the step,
                function, and derivative at the current step.
            */
            stx = sty = 0.0;
            fx = fy = finit;
            dgx = dgy = dginit;

            for (; ; )
            {
                /*
                    Set the minimum and maximum steps to correspond to the
                    present interval of uncertainty.
                 */
                if (brackt != 0)
                {
                    stmin = System.Math.Min(stx, sty);
                    stmax = System.Math.Max(stx, sty);
                }
                else
                {
                    stmin = stx;
                    stmax = stp + 4.0 * (stp - stx);
                }

                /* Clip the step in the range of [stpmin, stpmax]. */
                if (stp < param.min_step) stp = param.min_step;
                if (param.max_step < stp) stp = param.max_step;

                /*
                    If an unusual termination is to occur then let
                    stp be the lowest point obtained so far.
                 */
                if ((brackt > 0 && ((stp <= stmin || stmax <= stp) ||
                    param.max_linesearch <= count + 1 || uinfo != 0)) || (brackt > 0 && (stmax - stmin <= param.xtol * stmax)))
                {
                    stp = stx;
                }

                /*
                    Compute the current value of x:
                        x <- x + (stp) * s.
                 */
                veccpy(x, xp, n);
                vecadd(x, s, stp, n);

                /* Evaluate the function and gradient values. */
                f = cd.proc_evaluate(cd.instance, x, g, cd.n, stp);
                vecdot(out dg, g, s, n);

                ftest1 = finit + stp * dgtest;
                ++count;

                /* Test for errors and convergence. */
                if (brackt > 0 && ((stp <= stmin || stmax <= stp) || uinfo != 0))
                {
                    /* Rounding errors prevent further progress. */
                    return Code.LBFGSERR_ROUNDING_ERROR;
                }
                if (stp == param.max_step && f <= ftest1 && dg <= dgtest)
                {
                    /* The step is the maximum value. */
                    return Code.LBFGSERR_MAXIMUMSTEP;
                }
                if (stp == param.min_step && (ftest1 < f || dgtest <= dg))
                {
                    /* The step is the minimum value. */
                    return Code.LBFGSERR_MINIMUMSTEP;
                }
                if (brackt > 0 && (stmax - stmin) <= param.xtol * stmax)
                {
                    /* Relative width of the interval of uncertainty is at most xtol. */
                    return Code.LBFGSERR_WIDTHTOOSMALL;
                }
                if (param.max_linesearch <= count)
                {
                    /* Maximum number of iteration. */
                    return Code.LBFGSERR_MAXIMUMLINESEARCH;
                }
                if (f <= ftest1 && Math.Abs(dg) <= param.gtol * (-dginit))
                {
                    /* The sufficient decrease condition and the directional derivative condition hold. */
                    return (Code)count;
                }

                /*
                    In the first stage we seek a step for which the modified
                    function has a nonpositive value and nonnegative derivative.
                 */
                if (stage1 > 0 && f <= ftest1 && Math.Min(param.ftol, param.gtol) * dginit <= dg)
                {
                    stage1 = 0;
                }

                /*
                    A modified function is used to predict the step only if
                    we have not obtained a step for which the modified
                    function has a nonpositive function value and nonnegative
                    derivative, and if a lower function value has been
                    obtained but the decrease is not sufficient.
                 */
                if (stage1 > 0 && ftest1 < f && f <= fx)
                {
                    /* Define the modified function and derivative values. */
                    fm = f - stp * dgtest;
                    fxm = fx - stx * dgtest;
                    fym = fy - sty * dgtest;
                    dgm = dg - dgtest;
                    dgxm = dgx - dgtest;
                    dgym = dgy - dgtest;

                    /*
                        Call update_trial_interval() to update the interval of
                        uncertainty and to compute the new step.
                     */
                    uinfo = update_trial_interval(
                        ref stx, ref fxm, ref dgxm,
                        ref sty, ref fym, ref dgym,
                        ref stp, ref fm, ref dgm,
                        stmin, stmax, ref brackt
                        );

                    /* Reset the function and gradient values for f. */
                    fx = fxm + stx * dgtest;
                    fy = fym + sty * dgtest;
                    dgx = dgxm + dgtest;
                    dgy = dgym + dgtest;
                }
                else
                {
                    /*
                        Call update_trial_interval() to update the interval of
                        uncertainty and to compute the new step.
                     */
                    uinfo = update_trial_interval(
                        ref stx, ref fx, ref dgx,
                        ref sty, ref fy, ref dgy,
                        ref stp, ref f, ref dg,
                        stmin, stmax, ref brackt
                        );
                }

                /*
                    Force a sufficient decrease in the interval of uncertainty.
                 */
                if (brackt > 0)
                {
                    if (0.66 * prev_width <= Math.Abs(sty - stx))
                    {
                        stp = stx + 0.5 * (sty - stx);
                    }
                    prev_width = width;
                    width = Math.Abs(sty - stx);
                }
            }

        }





        /**
         * Find a minimizer of an interpolated cubic function.
         *  @param  cm      The minimizer of the interpolated cubic.
         *  @param  u       The value of one point, u.
         *  @param  fu      The value of f(u).
         *  @param  du      The value of f'(u).
         *  @param  v       The value of another point, v.
         *  @param  fv      The value of f(v).
         *  @param  du      The value of f'(v).
         */
        private static void CUBIC_MINIMIZER(ref double cm, ref double u, ref double fu, ref double du, ref double v, ref double fv, ref double dv, ref double a, ref double d, ref double gamma, ref double theta, ref double p, ref double q,
            ref double r, ref double s)
        {
            d = (v) - (u);
            theta = ((fu) - (fv)) * 3 / d + (du) + (dv);
            p = System.Math.Abs(theta);
            q = System.Math.Abs(du);
            r = System.Math.Abs(dv);
            s = Accord.Math.Tools.Max(p, q, r);
            /* gamma = s*System.Math.Sqrt((theta/s)**2 - (du/s) * (dv/s)) */
            a = theta / s;
            gamma = s * System.Math.Sqrt(a * a - ((du) / s) * ((dv) / s));
            if ((v) < (u)) gamma = -gamma;
            p = gamma - (du) + theta;
            q = gamma - (du) + gamma + (dv);
            r = p / q;
            (cm) = (u) + r * d;
        }

        /**
         * Find a minimizer of an interpolated cubic function.
         *  @param  cm      The minimizer of the interpolated cubic.
         *  @param  u       The value of one point, u.
         *  @param  fu      The value of f(u).
         *  @param  du      The value of f'(u).
         *  @param  v       The value of another point, v.
         *  @param  fv      The value of f(v).
         *  @param  du      The value of f'(v).
         *  @param  xmin    The maximum value.
         *  @param  xmin    The minimum value.
         */
        private static void CUBIC_MINIMIZER2(ref double cm, ref double u, ref double fu, ref double du, ref double v,
            ref double fv, ref double dv, ref double xmin, ref double xmax, ref double a, ref double d, ref double gamma, ref double theta, ref double p, ref double q,
            ref double r, ref double s)
        {
            d = (v) - (u);
            theta = ((fu) - (fv)) * 3 / d + (du) + (dv);
            p = System.Math.Abs(theta);
            q = System.Math.Abs(du);
            r = System.Math.Abs(dv);
            s = Accord.Math.Tools.Max(p, q, r);
            /* gamma = s*System.Math.Sqrt((theta/s)**2 - (du/s) * (dv/s)) */
            a = theta / s;
            gamma = s * System.Math.Sqrt(System.Math.Max(0, a * a - ((du) / s) * ((dv) / s)));
            if ((u) < (v)) gamma = -gamma;
            p = gamma - (dv) + theta;
            q = gamma - (dv) + gamma + (du);
            r = p / q;
            if (r < 0.0 && gamma != 0.0)
            {
                (cm) = (v) - r * d;
            }
            else if (a < 0)
            {
                (cm) = (xmax);
            }
            else
            {
                (cm) = (xmin);
            }
        }

        /**
         * Find a minimizer of an interpolated quadratic function.
         *  @param  qm      The minimizer of the interpolated quadratic.
         *  @param  u       The value of one point, u.
         *  @param  fu      The value of f(u).
         *  @param  du      The value of f'(u).
         *  @param  v       The value of another point, v.
         *  @param  fv      The value of f(v).
         */
        public static void QUARD_MINIMIZER(ref double qm, ref double u, ref double fu, ref double du, ref double v, ref double fv, ref double a)
        {
            a = (v) - (u);
            (qm) = (u) + (du) / (((fu) - (fv)) / a + (du)) / 2 * a;
        }
        /**
         * Find a minimizer of an interpolated quadratic function.
         *  @param  qm      The minimizer of the interpolated quadratic.
         *  @param  u       The value of one point, u.
         *  @param  du      The value of f'(u).
         *  @param  v       The value of another point, v.
         *  @param  dv      The value of f'(v).
         */
        public static void QUARD_MINIMIZER2(ref double qm, ref double u, ref double du, ref double v, ref double dv, ref double a)
        {
            a = (u) - (v);
            (qm) = (v) + (dv) / ((dv) - (du)) * a;
        }

        /**
         * Update a safeguarded trial value and interval for line search.
         *
         *  The parameter x represents the step with the least function value.
         *  The parameter t represents the current step. This function assumes
         *  that the derivative at the point of x in the direction of the step.
         *  If the bracket is set to true, the minimizer has been bracketed in
         *  an interval of uncertainty with endpoints between x and y.
         *
         *  @param  x       The pointer to the value of one endpoint.
         *  @param  fx      The pointer to the value of f(x).
         *  @param  dx      The pointer to the value of f'(x).
         *  @param  y       The pointer to the value of another endpoint.
         *  @param  fy      The pointer to the value of f(y).
         *  @param  dy      The pointer to the value of f'(y).
         *  @param  t       The pointer to the value of the trial value, t.
         *  @param  ft      The pointer to the value of f(t).
         *  @param  dt      The pointer to the value of f'(t).
         *  @param  tmin    The minimum value for the trial value, t.
         *  @param  tmax    The maximum value for the trial value, t.
         *  @param  brackt  The pointer to the predicate if the trial value is
         *                  bracketed.
         *  @retval int     Status value. Zero indicates a normal termination.
         *  
         *  @see
         *      Jorge J. More and David J. Thuente. Line search algorithm with
         *      guaranteed sufficient decrease. ACM Transactions on Mathematical
         *      Software (TOMS), Vol 20, No 3, pp. 286-307, 1994.
         */
        static Code update_trial_interval(
            ref double x,
            ref double fx,
            ref double dx,
            ref double y,
            ref double fy,
            ref double dy,
            ref double t,
            ref double ft,
            ref double dt,
             double tmin,
             double tmax,
            ref int brackt
            )
        {
            int bound;
            bool dsign = fsigndiff(dt, dx);
            double mc = 0; /* minimizer of an interpolated cubic. */
            double mq = 0; /* minimizer of an interpolated quadratic. */
            double newt = 0;   /* new trial value. */
            double a = 0, d = 0, gamma = 0, theta = 0, p = 0, q = 0, r = 0, s = 0;     /* for CUBIC_MINIMIZER and QUARD_MINIMIZER. */

            /* Check the input parameters for errors. */
            if (brackt > 0)
            {
                if (t <= System.Math.Min(x, y) || System.Math.Max(x, y) <= t)
                {
                    /* The trival value t is out of the interval. */
                    return Code.LBFGSERR_OUTOFINTERVAL;
                }
                if (0.0 <= dx * (t - x))
                {
                    /* The function must decrease from x. */
                    return Code.LBFGSERR_INCREASEGRADIENT;
                }
                if (tmax < tmin)
                {
                    /* Incorrect tmin and tmax specified. */
                    return Code.LBFGSERR_INCORRECT_TMINMAX;
                }
            }

            /*
                Trial value selection.
             */
            if (fx < ft)
            {
                /*
                    Case 1: a higher function value.
                    The minimum is brackt. If the cubic minimizer is closer
                    to x than the quadratic one, the cubic one is taken, else
                    the average of the minimizers is taken.
                 */
                brackt = 1;
                bound = 1;
                CUBIC_MINIMIZER(ref mc, ref x, ref fx, ref dx, ref t, ref ft, ref dt, ref a, ref d, ref gamma, ref theta, ref p, ref q, ref r, ref s);
                QUARD_MINIMIZER(ref mq, ref x, ref fx, ref dx, ref t, ref ft, ref a);

                if (System.Math.Abs(mc - x) < System.Math.Abs(mq - x))
                {
                    newt = mc;
                }
                else
                {
                    newt = mc + 0.5 * (mq - mc);
                }
            }
            else if (dsign)
            {
                /*
                    Case 2: a lower function value and derivatives of
                    opposite sign. The minimum is brackt. If the cubic
                    minimizer is closer to x than the quadratic (secant) one,
                    the cubic one is taken, else the quadratic one is taken.
                 */
                brackt = 1;
                bound = 0;
                CUBIC_MINIMIZER(ref mc, ref x, ref fx, ref dx, ref t, ref ft, ref dt, ref a, ref d, ref gamma, ref theta, ref p, ref q, ref r, ref s);
                QUARD_MINIMIZER2(ref mq, ref x, ref dx, ref t, ref dt, ref a);
                if (System.Math.Abs(mc - t) > System.Math.Abs(mq - t))
                {
                    newt = mc;
                }
                else
                {
                    newt = mq;
                }
            }
            else if (System.Math.Abs(dt) < System.Math.Abs(dx))
            {
                /*
                    Case 3: a lower function value, derivatives of the
                    same sign, and the magnitude of the derivative decreases.
                    The cubic minimizer is only used if the cubic tends to
                    infinity in the direction of the minimizer or if the minimum
                    of the cubic is beyond t. Otherwise the cubic minimizer is
                    defined to be either tmin or tmax. The quadratic (secant)
                    minimizer is also computed and if the minimum is brackt
                    then the the minimizer closest to x is taken, else the one
                    farthest away is taken.
                 */
                bound = 1;
                CUBIC_MINIMIZER2(ref mc, ref x, ref fx, ref dx, ref t, ref ft, ref dt, ref tmin, ref tmax, ref a, ref d, ref gamma, ref theta, ref p, ref q, ref r, ref s);
                QUARD_MINIMIZER2(ref mq, ref x, ref dx, ref t, ref dt, ref a);
                if (brackt != 0)
                {
                    if (System.Math.Abs(t - mc) < System.Math.Abs(t - mq))
                    {
                        newt = mc;
                    }
                    else
                    {
                        newt = mq;
                    }
                }
                else
                {
                    if (System.Math.Abs(t - mc) > System.Math.Abs(t - mq))
                    {
                        newt = mc;
                    }
                    else
                    {
                        newt = mq;
                    }
                }
            }
            else
            {
                /*
                    Case 4: a lower function value, derivatives of the
                    same sign, and the magnitude of the derivative does
                    not decrease. If the minimum is not brackt, the step
                    is either tmin or tmax, else the cubic minimizer is taken.
                 */
                bound = 0;
                if (brackt != 0)
                {
                    CUBIC_MINIMIZER(ref newt, ref t, ref ft, ref dt, ref y, ref fy, ref dy, ref a, ref d, ref gamma, ref theta, ref p, ref q, ref r, ref s);
                }
                else if (x < t)
                {
                    newt = tmax;
                }
                else
                {
                    newt = tmin;
                }
            }

            /*
                Update the interval of uncertainty. This update does not
                depend on the new step or the case analysis above.

                - Case a: if f(x) < f(t),
                    x <- x, y <- t.
                - Case b: if f(t) <= f(x) && f'(t)*f'(x) > 0,
                    x <- t, y <- y.
                - Case c: if f(t) <= f(x) && f'(t)*f'(x) < 0, 
                    x <- t, y <- x.
             */
            if (fx < ft)
            {
                /* Case a */
                y = t;
                fy = ft;
                dy = dt;
            }
            else
            {
                /* Case c */
                if (dsign)
                {
                    y = x;
                    fy = fx;
                    dy = dx;
                }
                /* Cases b and c */
                x = t;
                fx = ft;
                dx = dt;
            }

            /* Clip the new trial value in [tmin, tmax]. */
            if (tmax < newt) newt = tmax;
            if (newt < tmin) newt = tmin;

            /*
                Redefine the new trial value if it is close to the upper bound
                of the interval.
             */
            if (brackt > 0 && bound > 0)
            {
                mq = x + 0.66 * (y - x);
                if (x < y)
                {
                    if (mq < newt) newt = mq;
                }
                else
                {
                    if (newt < mq) newt = mq;
                }
            }

            /* Return the new trial value. */
            t = newt;
            return 0;
        }





        static double owlqn_x1norm(
             double[] x,
             int start,
             int n
            )
        {
            int i;
            double norm = 0.0;

            for (i = start; i < n; ++i)
            {
                norm += System.Math.Abs(x[i]);
            }

            return norm;
        }

        static void owlqn_pseudo_gradient(
            double[] pg,
             double[] x,
             double[] g,
             int n,
             double c,
             int start,
             int end
            )
        {
            int i;

            /* Compute the negative of gradients. */
            for (i = 0; i < start; ++i)
            {
                pg[i] = g[i];
            }

            /* Compute the psuedo-gradients. */
            for (i = start; i < end; ++i)
            {
                if (x[i] < 0.0)
                {
                    /* Differentiable. */
                    pg[i] = g[i] - c;
                }
                else if (0.0 < x[i])
                {
                    /* Differentiable. */
                    pg[i] = g[i] + c;
                }
                else
                {
                    if (g[i] < -c)
                    {
                        /* Take the right partial derivative. */
                        pg[i] = g[i] + c;
                    }
                    else if (c < g[i])
                    {
                        /* Take the left partial derivative. */
                        pg[i] = g[i] - c;
                    }
                    else
                    {
                        pg[i] = 0.0;
                    }
                }
            }

            for (i = end; i < n; ++i)
            {
                pg[i] = g[i];
            }
        }

        static void owlqn_project(
            double[] d,
             double[] sign,
             int start,
             int end
            )
        {
            int i;

            for (i = start; i < end; ++i)
            {
                if (d[i] * sign[i] <= 0)
                {
                    d[i] = 0;
                }
            }
        }
    }
}
