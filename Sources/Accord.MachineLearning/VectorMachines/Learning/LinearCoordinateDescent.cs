// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
//
// This code has been based on the LIBLINEAR project;s implementation for the
// L2-regularized L2-loss support vector classification (dual) machines. The
// original LIBLINEAR license is given below:
//
//
//   Copyright (c) 2007-2011 The LIBLINEAR Project.
//   All rights reserved.
//
//   Redistribution and use in source and binary forms, with or without
//   modification, are permitted provided that the following conditions
//   are met:
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
//   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//   ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//   A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE REGENTS OR
//   CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//   LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//   NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//

namespace Accord.MachineLearning.VectorMachines.Learning
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    ///   L1-regularized L2-loss support vector 
    ///   Support Vector Machine learning (-s 5).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class implements a <see cref="SupportVectorMachine"/> learning algorithm
    ///   specifically crafted for linear machines only. It provides a L1-regularized, 
    ///   L2-loss coordinate descent learning algorithm for optimizing the primal form of
    ///   learning. The code has been based on liblinear's method <c>solve_l1r_l2_svc</c>
    ///   method, whose original description is provided below.
    /// </para>
    /// 
    /// <para>
    ///   Liblinear's solver <c>-s 5</c>: <c>L1R_L2LOSS_svc</c>. A coordinate descent 
    ///   algorithm for L2-loss SVM problems in the primal.
    /// </para>
    /// 
    /// <code>
    ///  min_w \sum |wj| + C \sum max(0, 1-yi w^T xi)^2,
    /// </code>
    /// 
    /// <para>
    ///   Given: x, y, Cp, Cn and eps as the stopping tolerance</para>
    ///
    /// <para>
    ///   See Yuan et al. (2010) and appendix of LIBLINEAR paper, Fan et al. (2008)</para>
    /// </remarks>
    /// 
    /// <see cref="SequentialMinimalOptimization"/>
    /// <see cref="LinearNewtonMethod"/>
    /// <see cref="LinearDualCoordinateDescent"/>
    /// 
    public class LinearCoordinateDescent : BaseSupportVectorLearning,
        ISupportVectorMachineLearning, ISupportCancellation
    {

        double eps = 0.1;

        private double[] alpha;
        private double[] weights;
        private double[] bias;
        private int biasIndex;


        /// <summary>
        ///   Constructs a new coordinate descent algorithm for L1-loss and L2-loss SVM dual problems.
        /// </summary>
        /// 
        /// <param name="machine">A support vector machine.</param>
        /// <param name="inputs">The input data points as row vectors.</param>
        /// <param name="outputs">The output label for each input point. Values must be either -1 or +1.</param>
        /// 
        public LinearCoordinateDescent(SupportVectorMachine machine, double[][] inputs, int[] outputs)
            : base(machine, inputs, outputs)
        {
            int samples = inputs.Length;
            int dimension = inputs[0].Length;

            if (!IsLinear)
                throw new ArgumentException("Only linear machines are supported.", "machine");

            // Lagrange multipliers
            this.alpha = new double[samples];
            this.weights = new double[dimension + 1];
            this.bias = new double[samples];
            this.biasIndex = dimension;
        }

        /// <summary>
        ///   Gets the value for the Lagrange multipliers
        ///   (alpha) for every observation vector.
        /// </summary>
        /// 
        public double[] Lagrange { get { return alpha; } }

        /// <summary>
        ///   Convergence tolerance. Default value is 0.1.
        /// </summary>
        /// 
        /// <remarks>
        ///   The criterion for completing the model training process. The default is 0.1.
        /// </remarks>
        /// 
        public double Tolerance
        {
            get { return this.eps; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                this.eps = value;
            }
        }


        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        /// <param name="token">A token to stop processing when requested.</param>
        /// <param name="c">The complexity for each sample.</param>
        /// 
        protected override void Run(CancellationToken token, double[] c)
        {
            double[] w = weights;
            double[][] x = Inputs;
            int[] y = Outputs;

            var random = Accord.Math.Tools.Random;

            // Lagrange multipliers
            Array.Clear(alpha, 0, alpha.Length);
            Array.Clear(w, 0, w.Length);

            int s, iter = 0;
            int max_iter = 1000;
            int active_size = w.Length;
            int max_num_linesearch = 20;

            double sigma = 0.01;
            double d, G_loss, G, H;
            double Gmax_old = Double.PositiveInfinity;
            double Gmax_new, Gnorm1_new;
            double Gnorm1_init = -1.0; // Gnorm1_init is initialized at the first iteration
            double d_old, d_diff;
            double loss_old = 0, loss_new;
            double appxcond, cond;

            int[] index = new int[w.Length];
            double[] b = new double[x.Length]; // b = 1-ywTx
            double[] xj_sq = new double[w.Length];

            for (int i = 0; i < bias.Length; i++)
                bias[i] = 1;

            for (int j = 0; j < b.Length; j++)
                b[j] = 1;

            for (int j = 0; j < w.Length; j++)
            {
                index[j] = j;
                xj_sq[j] = 0;

                if (j == biasIndex)
                {
                    for (int i = 0; i < x.Length; i++)
                    {
                        bias[i] *= y[i]; // x->value stores yi*xij
                        b[i] -= w[j];
                        xj_sq[j] += c[i];
                    }
                }
                else
                {
                    for (int i = 0; i < x.Length; i++)
                    {
                        x[i][j] *= y[i]; // x->value stores yi*xij
                        double val = x[i][j];
                        b[i] -= w[j] * val;
                        xj_sq[j] += c[i] * val * val;
                    }
                }
            }

            while (iter < max_iter)
            {
                Gmax_new = 0;
                Gnorm1_new = 0;
                int j;

                for (j = 0; j < active_size; j++)
                {
                    int i = j + random.Next() % (active_size - j);
                    var old = index[i];
                    index[i] = index[j];
                    index[j] = old;
                }

                for (s = 0; s < active_size; s++)
                {
                    j = index[s];
                    G_loss = 0;
                    H = 0;

                    for (int i = 0; i < x.Length; i++)
                    {
                        if (b[i] > 0)
                        {
                            if (j == biasIndex)
                            {
                                double val = bias[i];
                                double tmp = c[i] * val;
                                G_loss -= tmp * b[i];
                                H += tmp * val;
                            }
                            else
                            {
                                double val = x[i][j];
                                double tmp = c[i] * val;
                                G_loss -= tmp * b[i];
                                H += tmp * val;
                            }
                        }
                    }

                    G_loss *= 2;

                    G = G_loss;
                    H *= 2;
                    H = Math.Max(H, 1e-12);

                    double Gp = G + 1;
                    double Gn = G - 1;
                    double violation = 0;

                    if (w[j] == 0)
                    {
                        if (Gp < 0)
                        {
                            violation = -Gp;
                        }
                        else if (Gn > 0)
                        {
                            violation = Gn;
                        }
                        else if (Gp > Gmax_old / x.Length && Gn < -Gmax_old / x.Length)
                        {
                            active_size--;

                            var old = index[s];
                            index[s] = index[active_size];
                            index[active_size] = old;

                            s--;
                            continue;
                        }
                    }
                    else if (w[j] > 0)
                    {
                        violation = Math.Abs(Gp);
                    }
                    else
                    {
                        violation = Math.Abs(Gn);
                    }

                    Gmax_new = Math.Max(Gmax_new, violation);
                    Gnorm1_new += violation;

                    // obtain Newton direction d
                    if (Gp < H * w[j])
                        d = -Gp / H;
                    else if (Gn > H * w[j])
                        d = -Gn / H;
                    else
                        d = -w[j];

                    if (Math.Abs(d) < 1.0e-12)
                        continue;

                    double delta = Math.Abs(w[j] + d) - Math.Abs(w[j]) + G * d;
                    d_old = 0;

                    int num_linesearch;
                    for (num_linesearch = 0; num_linesearch < max_num_linesearch; num_linesearch++)
                    {
                        d_diff = d_old - d;
                        cond = Math.Abs(w[j] + d) - Math.Abs(w[j]) - sigma * delta;

                        appxcond = xj_sq[j] * d * d + G_loss * d + cond;

                        if (appxcond <= 0)
                        {
                            if (j == biasIndex)
                            {
                                for (int i = 0; i < b.Length; i++)
                                    b[i] += d_diff * bias[i];
                            }
                            else
                            {
                                for (int i = 0; i < b.Length; i++)
                                    b[i] += d_diff * x[i][j];
                            }

                            break;
                        }

                        if (num_linesearch == 0)
                        {
                            loss_old = 0;
                            loss_new = 0;

                            if (j == biasIndex)
                            {
                                for (int i = 0; i < x.Length; i++)
                                {
                                    if (b[i] > 0)
                                        loss_old += c[i] * b[i] * b[i];

                                    double b_new = b[i] + d_diff * bias[i];
                                    b[i] = b_new;

                                    if (b_new > 0)
                                        loss_new += c[i] * b_new * b_new;
                                }
                            }
                            else
                            {
                                for (int i = 0; i < x.Length; i++)
                                {
                                    if (b[i] > 0)
                                        loss_old += c[i] * b[i] * b[i];

                                    double b_new = b[i] + d_diff * x[i][j];

                                    b[i] = b_new;

                                    if (b_new > 0)
                                        loss_new += c[i] * b_new * b_new;
                                }
                            }
                        }
                        else
                        {
                            loss_new = 0;

                            if (j == biasIndex)
                            {
                                for (int i = 0; i < x.Length; i++)
                                {
                                    double b_new = b[i] + d_diff * bias[i];

                                    b[i] = b_new;

                                    if (b_new > 0)
                                        loss_new += c[i] * b_new * b_new;
                                }
                            }
                            else
                            {
                                for (int i = 0; i < x.Length; i++)
                                {
                                    double b_new = b[i] + d_diff * x[i][j];
                                    b[i] = b_new;

                                    if (b_new > 0)
                                        loss_new += c[i] * b_new * b_new;
                                }
                            }
                        }

                        cond = cond + loss_new - loss_old;

                        if (cond <= 0)
                        {
                            break;
                        }
                        else
                        {
                            d_old = d;
                            d *= 0.5;
                            delta *= 0.5;
                        }
                    }

                    w[j] += d;

                    // recompute b[] if line search takes too many steps
                    if (num_linesearch >= max_num_linesearch)
                    {
                        Debug.WriteLine("#");
                        for (int i = 0; i < b.Length; i++)
                            b[i] = 1;

                        for (int i = 0; i < w.Length; i++)
                        {
                            if (w[i] == 0)
                                continue;

                            if (i == biasIndex)
                            {
                                for (int k = 0; k < x.Length; k++)
                                    b[k] -= w[i] * bias[k];
                            }
                            else
                            {
                                for (int k = 0; k < x.Length; k++)
                                    b[k] -= w[i] * x[k][i];
                            }
                        }
                    }
                }

                if (iter == 0)
                    Gnorm1_init = Gnorm1_new;

                iter++;
                if (iter % 10 == 0)
                    Debug.WriteLine(".");

                if (Gnorm1_new <= eps * Gnorm1_init)
                {
                    if (active_size == w.Length)
                        break;

                    else
                    {
                        active_size = w.Length;
                        Debug.WriteLine("*");
                        Gmax_old = Double.PositiveInfinity;
                        continue;
                    }
                }

                Gmax_old = Gmax_new;
            }

            Debug.WriteLine("optimization finished, #iter = " + iter);
            if (iter >= max_iter)
                Debug.WriteLine("WARNING: reaching max number of iterations");

            // calculate objective value

            double v = 0;
            int nnz = 0;
            for (int j = 0; j < w.Length; j++)
            {

                if (j == biasIndex)
                {
                    for (int i = 0; i < x.Length; i++)
                        bias[i] *= y[i]; // restore x->value
                }
                else
                {
                    for (int i = 0; i < x.Length; i++)
                        x[i][j] *= y[i]; // restore x->value
                }

                if (w[j] != 0)
                {
                    v += Math.Abs(w[j]);
                    nnz++;
                }
            }

            for (int j = 0; j < c.Length; j++)
            {
                if (b[j] > 0)
                    v += c[j] * b[j] * b[j];
            }

            Debug.WriteLine("Objective value = " + v);
            Debug.WriteLine("#nonzeros/#features = " + nnz + "/" + w.Length);

            Machine.Weights = new double[Machine.Inputs];
            for (int i = 0; i < Machine.Weights.Length; i++)
                Machine.Weights[i] = w[i];
            Machine.Threshold = w[biasIndex];
        }

    }
}
