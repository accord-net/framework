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
    ///   Different categories of loss functions that can be used to learn 
    ///   <see cref="SupportVectorMachine">support vector machines</see>.
    /// </summary>
    /// 
    public enum Loss
    {
        /// <summary>
        ///   Hinge-loss function. 
        /// </summary>
        /// 
        L1,

        /// <summary>
        ///   Squared hinge-loss function.
        /// </summary>
        /// 
        L2
    };



    /// <summary>
    ///   L2-regularized, L1 or L2-loss dual formulation 
    ///   Support Vector Machine learning (-s 1 and -s 3).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class implements a <see cref="SupportVectorMachine"/> learning algorithm
    ///   specifically crafted for linear machines only. It provides a L2-regularized, L1
    ///   or L2-loss coordinate descent learning algorithm for optimizing the dual form of
    ///   learning. The code has been based on liblinear's method <c>solve_l2r_l1l2_svc</c>
    ///   method, whose original description is provided below.
    /// </para>
    /// 
    /// <para>
    ///   Liblinear's solver <c>-s 1</c>: <c>L2R_L2LOSS_SVC_DUAL</c> and <c>-s 3</c>: 
    ///   <c>L2R_L1LOSS_SVC_DUAL</c>. A coordinate descent algorithm for L1-loss and 
    ///   L2-loss SVM problems in the dual.
    /// </para>
    /// 
    /// <code>
    ///  min_\alpha  0.5(\alpha^T (Q + D)\alpha) - e^T \alpha,
    ///    s.t.      0 &lt;= \alpha_i &lt;= upper_bound_i,
    /// </code>
    /// 
    /// <para>
    ///  where Qij = yi yj xi^T xj and
    ///  D is a diagonal matrix </para>
    ///
    /// <para>
    /// In L1-SVM case:</para>
    /// <code>
    ///         upper_bound_i = Cp if y_i = 1
    ///         upper_bound_i = Cn if y_i = -1
    ///         D_ii = 0
    /// </code>
    /// <para>
    /// In L2-SVM case:</para>
    /// <code>
    ///         upper_bound_i = INF
    ///         D_ii = 1/(2*Cp)	if y_i = 1
    ///         D_ii = 1/(2*Cn)	if y_i = -1
    /// </code>
    /// 
    /// <para>
    /// Given: x, y, Cp, Cn, and eps as the stopping tolerance</para>
    ///
    /// <para>
    /// See Algorithm 3 of Hsieh et al., ICML 2008.</para>
    /// </remarks>
    /// 
    /// <see cref="SequentialMinimalOptimization"/>
    /// <see cref="LinearNewtonMethod"/>
    /// <see cref="LinearCoordinateDescent"/>
    /// 
    public class LinearDualCoordinateDescent : BaseSupportVectorLearning,
        ISupportVectorMachineLearning, ISupportCancellation
    {

        int max_iter = 1000;

        private double eps = 0.1;

        private double[] alpha;
        private double[] weights;
        private double bias;

        private Loss loss = Loss.L2;

        /// <summary>
        ///   Constructs a new coordinate descent algorithm for L1-loss and L2-loss SVM dual problems.
        /// </summary>
        /// 
        /// <param name="machine">A support vector machine.</param>
        /// <param name="inputs">The input data points as row vectors.</param>
        /// <param name="outputs">The output label for each input point. Values must be either -1 or +1.</param>
        /// 
        public LinearDualCoordinateDescent(SupportVectorMachine machine, double[][] inputs, int[] outputs)
            : base(machine, inputs, outputs)
        {
            int samples = inputs.Length;
            int dimension = inputs[0].Length;

            if (!IsLinear)
                throw new ArgumentException("Only linear machines are supported.", "machine");

            // Lagrange multipliers
            this.alpha = new double[samples];
            this.weights = new double[dimension];
        }


        /// <summary>
        ///   Gets or sets the <see cref="Loss"/> cost function that
        ///   should be optimized. Default is 
        ///   <see cref="Accord.MachineLearning.VectorMachines.Learning.Loss.L2"/>.
        /// </summary>
        /// 
        public Loss Loss
        {
            get { return loss; }
            set { loss = value; }
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
            bias = 0;


            int iter = 0;
            double[] QD = new double[x.Length];
            int[] index = new int[x.Length];

            int active_size = x.Length;

            // PG: projected gradient, for shrinking and stopping
            double PG;
            double PGmax_old = Double.PositiveInfinity;
            double PGmin_old = Double.NegativeInfinity;
            double PGmax_new, PGmin_new;

            // default solver_type: L2R_L2LOSS_SVC_DUAL
            // double[] diag = { 0.5 / Cn, 0, 0.5 / Cp };
            // double[] upper_bound = { Double.PositiveInfinity, 0, Double.PositiveInfinity };

            double[] diag = new double[c.Length];
            double[] upper_bound = new double[c.Length];

            if (Loss == Loss.L2)
            {
                // Default
                for (int i = 0; i < diag.Length; i++)
                    diag[i] = 0.5 / c[i];

                for (int i = 0; i < upper_bound.Length; i++)
                    upper_bound[i] = Double.PositiveInfinity;
            }
            else
            {
                // diag remains zero, and
                upper_bound = c;
            }


            for (int i = 0; i < x.Length; i++)
            {
                QD[i] = diag[i];

                double[] xi = x[i];
                for (int j = 0; j < xi.Length; j++)
                {
                    double val = xi[j];
                    QD[i] += val * val;
                    w[j] += y[i] * alpha[i] * val;
                }

                QD[i] += 1;
                bias += y[i] * alpha[i];

                index[i] = i;
            }

            while (iter < max_iter)
            {
                if (token.IsCancellationRequested)
                    break;

                PGmax_new = double.NegativeInfinity;
                PGmin_new = double.PositiveInfinity;

                for (int i = 0; i < active_size; i++)
                {
                    int j = i + random.Next() % (active_size - i);

                    var old = index[i];
                    index[i] = index[j];
                    index[j] = old;
                }

                for (int s = 0; s < active_size; s++)
                {
                    int i = index[s];
                    int yi = y[i];
                    double[] xi = x[i];

                    double G = bias;
                    for (int j = 0; j < xi.Length; j++)
                        G += w[j] * xi[j];

                    G = G * yi - 1;

                    double C = upper_bound[i];
                    G += alpha[i] * diag[i];

                    PG = 0;
                    if (alpha[i] == 0)
                    {
                        if (G > PGmax_old)
                        {
                            active_size--;

                            var old = index[s];
                            index[s] = index[active_size];
                            index[active_size] = old;

                            s--;
                            continue;
                        }
                        else if (G < 0)
                        {
                            PG = G;
                        }
                    }
                    else if (alpha[i] == C)
                    {
                        if (G < PGmin_old)
                        {
                            active_size--;

                            var old = index[s];
                            index[s] = index[active_size];
                            index[active_size] = old;

                            s--;
                            continue;
                        }
                        else if (G > 0)
                        {
                            PG = G;
                        }
                    }
                    else
                    {
                        PG = G;
                    }

                    PGmax_new = Math.Max(PGmax_new, PG);
                    PGmin_new = Math.Min(PGmin_new, PG);

                    if (Math.Abs(PG) > 1.0e-12)
                    {
                        double alpha_old = alpha[i];

                        alpha[i] = Math.Min(Math.Max(alpha[i] - G / QD[i], 0.0), C);

                        double d = (alpha[i] - alpha_old) * yi;

                        xi = x[i];
                        for (int j = 0; j < xi.Length; j++)
                            w[j] += d * xi[j];
                        bias += d;
                    }
                }

                iter++;

                if (iter % 10 == 0)
                    Debug.WriteLine(".");

                if (PGmax_new - PGmin_new <= eps)
                {
                    if (active_size == x.Length)
                        break;

                    active_size = x.Length;
                    Debug.WriteLine("*");
                    PGmax_old = Double.PositiveInfinity;
                    PGmin_old = Double.NegativeInfinity;
                    continue;
                }

                PGmax_old = PGmax_new;
                PGmin_old = PGmin_new;

                if (PGmax_old <= 0)
                    PGmax_old = Double.PositiveInfinity;

                if (PGmin_old >= 0)
                    PGmin_old = Double.NegativeInfinity;
            }

            Debug.WriteLine("optimization finished, #iter = " + iter);

            if (iter >= max_iter)
            {
                Debug.WriteLine("WARNING: reaching max number of iterations");
                Debug.WriteLine("Using -s 2 may be faster (also see FAQ)");
            }


            Machine.Weights = new double[Machine.Inputs];
            for (int i = 0; i < Machine.Weights.Length; i++)
                Machine.Weights[i] = w[i];
            Machine.Threshold = bias;
        }

    }
}
