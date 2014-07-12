// Copyright (c) 2007-2011 The LIBLINEAR Project.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions
// are met:
//
//   1. Redistributions of source code must retain the above copyright
//   notice, this list of conditions and the following disclaimer.
//
//   2. Redistributions in binary form must reproduce the above copyright
//   notice, this list of conditions and the following disclaimer in the
//   documentation and/or other materials provided with the distribution.
//
//   3. Neither name of copyright holders nor the names of its contributors
//   may be used to endorse or promote products derived from this software
//   without specific prior written permission.
//
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE REGENTS OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//

namespace Accord.Math.Optimization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;

    public class TrustRegionNewtonMethod : BaseGradientOptimizationMethod
    {
        double eps = 0.1;
        int max_iter = 1000;

        public double Tolerance
        {
            get { return eps; }
            set { eps = value; }
        }

        public int MaxIterations
        {
            get { return max_iter; }
            set { max_iter = value; }
        }

        public Func<double[], double[]> Hessian { get; set; }


        public TrustRegionNewtonMethod(int numberOfVariables)
            : base(numberOfVariables)
        {
        }

        public TrustRegionNewtonMethod(int numberOfVariables, Func<double[], double> function,
            Func<double[], double[]> gradient, Func<double[], double[]> hessian)
            : base(numberOfVariables, function, gradient)
        {
            Hessian = hessian;
        }

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
            int i, cg_iter;
            double delta, snorm;
            double alpha, f, fnew, prered, actred, gs;
            int search = 1, iter = 1;
            double[] s = new double[n];
            double[] r = new double[n];
            double[] w_new = new double[n];
            double[] g = new double[n];

            for (i = 0; i < n; i++)
                w[i] = 0;

            f = Function(w);
            g = Gradient(w);

            // delta = dnrm2_(&n, g, &inc);
            delta = 0;
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
                cg_iter = trcg(delta, g, s, r);

                for (int j = 0; j < w_new.Length; j++)
                    w_new[j] = w[j] + s[j];

                gs = 0;
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
                snorm = 0;
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

                // info("iter %2d act %5.3e pre %5.3e delta %5.3e f %5.3e |g| %5.3e CG %3d\n", iter, actred, prered, delta, f, gnorm, cg_iter);

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
                if (f < -1.0e+32)
                {
                    Debug.WriteLine("WARNING: f < -1.0e+32\n");
                    break;
                }
                if (Math.Abs(actred) <= 0 && prered <= 0)
                {
                    Debug.WriteLine("WARNING: actred and prered <= 0\n");
                    break;
                }
                if (Math.Abs(actred) <= 1.0e-12 * Math.Abs(f) &&
                    Math.Abs(prered) <= 1.0e-12 * Math.Abs(f))
                {
                    Debug.WriteLine("WARNING: actred and prered too small\n");
                    break;
                }
            }
        }

        int trcg(double delta, double[] g, double[] s, double[] r)
        {
            int i;
            int n = NumberOfVariables;

            double[] d = new double[n];
            // double[] Hd = new double[n];
            double rTr, rnewTrnew, alpha, beta, cgtol;

            for (i = 0; i < n; i++)
            {
                s[i] = 0;
                r[i] = -g[i];
                d[i] = r[i];
            }


            cgtol = 0; // cgtol = 0.1 * dnrm2_(&n, g, &inc);
            for (int j = 0; j < g.Length; j++)
                cgtol += g[j] * g[j];
            cgtol = 0.1 * Math.Sqrt(cgtol);

            int cg_iter = 0;

            rTr = 0; // rTr = ddot_(&n, r, &inc, r, &inc);
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

                var Hd = Hessian(d);

                double dHd = 0; // ddot_(&n, d, &inc, Hd, &inc)
                for (int j = 0; j < d.Length; j++)
                    dHd += d[j] * Hd[j];

                alpha = rTr / dHd;

                // daxpy_(&n, &alpha, d, &inc, s, &inc);
                for (int j = 0; j < d.Length; j++)
                    s[j] += alpha * d[j];

                double sn = 0; // dnrm2_(&n, s, &inc)
                for (int j = 0; j < s.Length; j++)
                    sn += s[j] * s[j];
                sn = Math.Sqrt(sn);

                if (sn > delta)
                {
                    Debug.WriteLine("cg reaches trust region boundary\n");
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
                rnewTrnew = 0.0;
                for (int j = 0; j < r.Length; j++)
                    rnewTrnew += r[j] * r[j];

                beta = rnewTrnew / rTr;

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

        double norm_inf(double[] x)
        {
            double dmax = Math.Abs(x[0]);

            for (int i = 1; i < x.Length; i++)
            {
                if (Math.Abs(x[i]) >= dmax)
                    dmax = Math.Abs(x[i]);
            }

            return dmax;
        }
    }
}


