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
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//
//
// The source code presented in this file has been adapted from LIBLINEAR - 
// A Library for Large Linear Classification, leaded by Chih-Jen Lin. Its 
// original license is given below.
//
//    Copyright (c) 2007-2011 The LIBLINEAR Project.
//    All rights reserved.
//
//    Redistribution and use in source and binary forms, with or without
//    modification, are permitted provided that the following conditions
//    are met:
//
//      1. Redistributions of source code must retain the above copyright
//      notice, this list of conditions and the following disclaimer.
//
//      2. Redistributions in binary form must reproduce the above copyright
//      notice, this list of conditions and the following disclaimer in the
//      documentation and/or other materials provided with the distribution.
//
//      3. Neither name of copyright holders nor the names of its contributors
//      may be used to endorse or promote products derived from this software
//      without specific prior written permission.
//
//
//    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//    ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//    A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE REGENTS OR
//    CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//    EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//    PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//    PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//    LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//    NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//    SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//

namespace Accord.Math.Optimization
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Accord.Compat;

    /// <summary>
    ///   Simplified Trust Region Newton Method (TRON) for non-linear optimization.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Trust region is a term used in mathematical optimization to denote the subset 
    ///   of the region of the objective function to be optimized that is approximated 
    ///   using a model function (often a quadratic). If an adequate model of the objective
    ///   function is found within the trust region then the region is expanded; conversely,
    ///   if the approximation is poor then the region is contracted. Trust region methods 
    ///   are also known as restricted step methods.</para>
    /// <para>
    ///   The fit is evaluated by comparing the ratio of expected improvement from the model
    ///   approximation with the actual improvement observed in the objective function. Simple
    ///   thresholding of the ratio is used as the criteria for expansion and contraction—a
    ///   model function is "trusted" only in the region where it provides a reasonable 
    ///   approximation. </para>
    ///   
    /// <para>
    ///   Trust region methods are in some sense dual to line search methods: trust region 
    ///   methods first choose a step size (the size of the trust region) and then a step 
    ///   direction while line search methods first choose a step direction and then a step
    ///   size.</para>
    ///   
    /// <para>
    ///   This class implements a simplified version of Chih-Jen Lin and Jorge Moré's TRON,
    ///   a trust region Newton method for the solution of large bound-constrained optimization 
    ///   problems. This version was based upon liblinear's implementation.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       <a href="http://en.wikipedia.org/wiki/Trust_region">
    ///       Wikipedia, The Free Encyclopedia. Trust region. Available on:
    ///       http://en.wikipedia.org/wiki/Trust_region </a></description></item>
    ///     <item><description>
    ///       <a href="http://www.mcs.anl.gov/~more/tron/index.html">
    ///      Chih-Jen Lin and Jorge Moré, TRON. Available on: http://www.mcs.anl.gov/~more/tron/index.html
    ///       </a></description></item>
    ///     <item><description>
    ///       <a href="http://www.cs.iastate.edu/~honavar/keerthi-svm.pdf">
    ///       Chih-Jen Lin and Jorge J. Moré. 1999. Newton's Method for Large Bound-Constrained 
    ///       Optimization Problems. SIAM J. on Optimization 9, 4 (April 1999), 1100-1127. </a>
    ///       </description></item>
    ///     <item><description>
    ///       <a href="http://www.csie.ntu.edu.tw/~cjlin/liblinear/">
    ///       Machine Learning Group. LIBLINEAR -- A Library for Large Linear Classification.
    ///       National Taiwan University. Available at: http://www.csie.ntu.edu.tw/~cjlin/liblinear/
    ///       </a></description></item>
    ///     </list></para>  
    /// </remarks>
    /// 
    /// <seealso cref="ConjugateGradient"/>
    /// <seealso cref="BoundedBroydenFletcherGoldfarbShanno"/>
    /// <seealso cref="BroydenFletcherGoldfarbShanno"/>
    /// <seealso cref="ResilientBackpropagation"/>
    /// 
    public class TrustRegionNewtonMethod : BaseGradientOptimizationMethod
    {
        double eps = 0.1;
        int max_iter = 1000;

        /// <summary>
        ///   Gets or sets the tolerance under which the
        ///   solution should be found. Default is 0.1.
        /// </summary>
        /// 
        public double Tolerance
        {
            get { return eps; }
            set { eps = value; }
        }

        /// <summary>
        ///   Gets or sets the maximum number of iterations that should
        ///    be performed until the algorithm stops. Default is 1000.
        /// </summary>
        /// 
        public int MaxIterations
        {
            get { return max_iter; }
            set { max_iter = value; }
        }

        /// <summary>
        ///   Gets or sets the Hessian estimation function.
        /// </summary>
        /// 
        public Func<double[], double[]> Hessian { get; set; }

        /// <summary>
        ///   Creates a new <see cref="ResilientBackpropagation"/> function optimizer.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of parameters in the function to be optimized.</param>
        /// 
        public TrustRegionNewtonMethod(int numberOfVariables)
            : base(numberOfVariables)
        {
        }

        /// <summary>
        ///   Creates a new <see cref="ResilientBackpropagation"/> function optimizer.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of free parameters in the function to be optimized.</param>
        /// <param name="function">The function to be optimized.</param>
        /// <param name="gradient">The gradient of the function.</param>
        /// <param name="hessian">The hessian of the function.</param>
        /// 
        public TrustRegionNewtonMethod(int numberOfVariables, Func<double[], double> function,
            Func<double[], double[]> gradient, Func<double[], double[]> hessian)
            : base(numberOfVariables, function, gradient)
        {
            Hessian = hessian;
        }

        /// <summary>
        ///   Implements the actual optimization algorithm. This
        ///   method should try to minimize the objective function.
        /// </summary>
        /// 
        protected override bool Optimize()
        {
            tron(Solution);

            return true;
        }

        void tron(double[] w)
        {
            // Parameters for updating the iterates.
            double eta0 = 1e-4, eta1 = 0.25, eta2 = 0.75;

            // Parameters for updating the trust region size delta.
            double sigma1 = 0.25, sigma2 = 0.5, sigma3 = 4;

            int n = NumberOfVariables;
            int cg_iter;
            double alpha, fnew, prered, actred;
            int search = 1, iter = 1;
            double[] s = new double[n];
            double[] r = new double[n];
            double[] w_new = new double[n];


            double f = Function(w);
            double[] g = Gradient(w);

            // delta = dnrm2_(&n, g, &inc);
            double delta = 0;
            for (int j = 0; j < g.Length; j++)
                delta += g[j] * g[j];
            delta = Math.Sqrt(delta);

            double gnorm1 = delta;
            double gnorm = gnorm1;

            if (gnorm <= eps * gnorm1)
                search = 0;

            iter = 1;

            while (iter <= max_iter && search == 1)
            {
                if (Token.IsCancellationRequested)
                    break;

                cg_iter = trcg(delta, g, s, r);

                for (int j = 0; j < w_new.Length; j++)
                    w_new[j] = w[j] + s[j];

                double gs = 0;
                for (int j = 0; j < g.Length; j++)
                    gs += g[j] * s[j];

                double t = 0;
                for (int j = 0; j < s.Length; j++)
                    t += s[j] * r[j];
                prered = -0.5 * (gs - t);
                fnew = Function(w_new);

                // Compute the actual reduction.
                actred = f - fnew;

                // On the first iteration, adjust the initial step bound.
                double snorm = 0;
                for (int j = 0; j < s.Length; j++)
                    snorm += s[j] * s[j];
                snorm = Math.Sqrt(snorm);

                if (iter == 1)
                    delta = Math.Min(delta, snorm);

                // Compute prediction alpha*snorm of the step.
                if (fnew - f - gs <= 0)
                    alpha = sigma3;
                else
                    alpha = Math.Max(sigma1, -0.5 * (gs / (fnew - f - gs)));

                // Update the trust region bound according to the ratio of actual to predicted reduction.
                if (actred < eta0 * prered)
                    delta = Math.Min(Math.Max(alpha, sigma1) * snorm, sigma2 * delta);
                else if (actred < eta1 * prered)
                    delta = Math.Max(sigma1 * delta, Math.Min(alpha * snorm, sigma2 * delta));
                else if (actred < eta2 * prered)
                    delta = Math.Max(sigma1 * delta, Math.Min(alpha * snorm, sigma3 * delta));
                else
                    delta = Math.Max(delta, Math.Min(alpha * snorm, sigma3 * delta));

                Trace.WriteLine(String.Format(
                    "iter {0} act {1:E03} pre {2:E03} delta {3:E03} f {4:E03} |g| {5:E03} CG {6}",
                    iter, actred, prered, delta, f, gnorm, cg_iter));

                if (actred > eta0 * prered)
                {
                    iter++;

                    for (int j = 0; j < w.Length; j++)
                        w[j] = w_new[j];

                    f = fnew;
                    g = Gradient(w);

                    gnorm = 0;
                    for (int j = 0; j < g.Length; j++)
                        gnorm += g[j];
                    gnorm = Math.Sqrt(gnorm);

                    if (gnorm <= eps * gnorm1)
                        break;
                }

                // TODO: Use these status codes in a TrustRegionNewtonMethodStatus enumeration

                if (f < -1.0e+32)
                {
                    Trace.WriteLine("WARNING: f < -1.0e+32");
                    break;
                }
                if (Math.Abs(actred) <= 0 && prered <= 0)
                {
                    Trace.WriteLine("WARNING: actred and prered <= 0");
                    break;
                }
                if (Math.Abs(actred) <= 1.0e-12 * Math.Abs(f) &&
                    Math.Abs(prered) <= 1.0e-12 * Math.Abs(f))
                {
                    Trace.WriteLine("WARNING: actred and prered too small");
                    break;
                }
            }
        }

        int trcg(double delta, double[] g, double[] s, double[] r)
        {
            int n = NumberOfVariables;
            double[] d = new double[n];

            for (int i = 0; i < g.Length; i++)
            {
                s[i] = 0;
                r[i] = -g[i];
                d[i] = r[i];
            }


            double cgtol = 0; // cgtol = 0.1 * dnrm2_(&n, g, &inc);
            for (int j = 0; j < g.Length; j++)
                cgtol += g[j] * g[j];
            cgtol = 0.1 * Math.Sqrt(cgtol);

            int cg_iter = 0;

            double rTr = 0; // rTr = ddot_(&n, r, &inc, r, &inc);
            for (int j = 0; j < r.Length; j++)
                rTr += r[j] * r[j];

            while (true)
            {
                // rn = dnrm2_(&n, r, &inc)
                double rn = 0;
                for (int j = 0; j < r.Length; j++)
                    rn += r[j] * r[j];
                rn = Math.Sqrt(rn);

                if (rn <= cgtol)
                    break;

                cg_iter++;

                double[] Hd = Hessian(d);

                double dHd = 0; // ddot_(&n, d, &inc, Hd, &inc)
                for (int j = 0; j < d.Length; j++)
                    dHd += d[j] * Hd[j];

                double alpha = rTr / dHd;

                // daxpy_(&n, &alpha, d, &inc, s, &inc);
                for (int j = 0; j < d.Length; j++)
                    s[j] += alpha * d[j];

                double sn = 0; // dnrm2_(&n, s, &inc)
                for (int j = 0; j < s.Length; j++)
                    sn += s[j] * s[j];
                sn = Math.Sqrt(sn);

                if (sn > delta)
                {
                    Trace.WriteLine("cg reaches trust region boundary");
                    alpha = -alpha;

                    // daxpy_(&n, &alpha, d, &inc, s, &inc);
                    for (int j = 0; j < d.Length; j++)
                        s[j] += alpha * d[j];

                    double std = 0; // ddot_(&n, s, &inc, d, &inc);
                    for (int j = 0; j < s.Length; j++)
                        std += s[j] * d[j];

                    double sts = 0; // ddot_(&n, s, &inc, s, &inc);
                    for (int j = 0; j < s.Length; j++)
                        sts += s[j] * s[j];

                    double dtd = 0;// ddot_(&n, d, &inc, d, &inc);
                    for (int j = 0; j < d.Length; j++)
                        dtd += d[j] * d[j];

                    double dsq = delta * delta;
                    double rad = Math.Sqrt(std * std + dtd * (dsq - sts));


                    if (std >= 0)
                        alpha = (dsq - sts) / (std + rad);
                    else
                        alpha = (rad - std) / dtd;

                    // daxpy_(&n, &alpha, d, &inc, s, &inc);
                    for (int j = 0; j < d.Length; j++)
                        s[j] += alpha * d[j];

                    alpha = -alpha;

                    // daxpy_(&n, &alpha, Hd, &inc, r, &inc);
                    for (int j = 0; j < r.Length; j++)
                        r[j] += alpha * Hd[j];

                    break;
                }

                alpha = -alpha;

                // daxpy_(&n, &alpha, Hd, &inc, r, &inc);
                for (int j = 0; j < r.Length; j++)
                    r[j] += alpha * Hd[j];

                // ddot_(&n, r, &inc, r, &inc);
                double rnewTrnew = 0.0;
                for (int j = 0; j < r.Length; j++)
                    rnewTrnew += r[j] * r[j];

                double beta = rnewTrnew / rTr;

                // dscal_(&n, &beta, d, &inc);
                for (int j = 0; j < d.Length; j++)
                    d[j] *= beta;

                // daxpy_(&n, &one, r, &inc, d, &inc);
                for (int j = 0; j < d.Length; j++)
                    d[j] += r[j];

                rTr = rnewTrnew;
            }

            return cg_iter;
        }

    }
}


