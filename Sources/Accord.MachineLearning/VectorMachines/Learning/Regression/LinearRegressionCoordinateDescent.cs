// Accord Machine Learning Library
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
    using Accord.Statistics.Kernels;
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Accord.Compat;

    /// <summary>
    ///   Coordinate descent algorithm for the L1 or L2-loss linear Support 
    ///   Vector Regression (epsilon-SVR) learning problem in the dual form
    ///   (-s 12 and -s 13).
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
    ///   Liblinear's solver <c>-s 12</c>: <c>L2R_L2LOSS_SVR_DUAL</c> and <c>-s 13</c>: 
    ///   <c>L2R_L1LOSS_SVR_DUAL</c>. A coordinate descent algorithm for L1-loss and 
    ///   L2-loss linear epsilon-vector regression (epsilon-SVR).
    /// </para>
    /// 
    /// <code>
    ///   min_\beta  0.5\beta^T (Q + diag(lambda)) \beta - p \sum_{i=1}^l|\beta_i| + \sum_{i=1}^l yi\beta_i,
    ///     s.t.     -upper_bound_i &lt;= \beta_i &lt;= upper_bound_i,
    /// </code>
    /// 
    /// <para>
    ///  where Qij = yi yj xi^T xj and
    ///  D is a diagonal matrix </para>
    ///
    /// <para>
    /// In L1-SVM case:</para>
    /// <code>
    ///    upper_bound_i = C
    ///    lambda_i = 0
    /// </code>
    /// <para>
    /// In L2-SVM case:</para>
    /// <code>
    ///    upper_bound_i = INF
    ///    lambda_i = 1/(2*C)
    /// </code>
    /// 
    /// <para>
    /// Given: x, y, p, C and eps as the stopping tolerance</para>
    ///
    /// <para>
    /// See Algorithm 4 of Ho and Lin, 2012.</para>
    /// </remarks>
    /// 
    /// <see cref="SequentialMinimalOptimization{TKernel}"/>
    /// <see cref="LinearNewtonMethod"/>
    /// 
    public class LinearRegressionCoordinateDescent :
        BaseLinearRegressionCoordinateDescent<SupportVectorMachine, Linear, double[]>
    {
        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please do not pass parameters in the constructor. Use the default constructor and the Learn method instead.")]
        public LinearRegressionCoordinateDescent(SupportVectorMachine model, double[][] input, double[] output)
            : base(model, input, output)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearRegressionCoordinateDescent"/> class.
        /// </summary>
        public LinearRegressionCoordinateDescent()
        {
        }

        /// <summary>
        /// Creates an instance of the model to be learned. Inheritors
        /// of this abstract class must define this method so new models
        /// can be created from the training data.
        /// </summary>
        protected override SupportVectorMachine Create(int inputs, Linear kernel)
        {
            return new SupportVectorMachine(inputs) { Kernel = kernel };
        }
    }

    /// <summary>
    ///   Coordinate descent algorithm for the L1 or L2-loss linear Support 
    ///   Vector Regression (epsilon-SVR) learning problem in the dual form
    ///   (-s 12 and -s 13).
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
    ///   Liblinear's solver <c>-s 12</c>: <c>L2R_L2LOSS_SVR_DUAL</c> and <c>-s 13</c>: 
    ///   <c>L2R_L1LOSS_SVR_DUAL</c>. A coordinate descent algorithm for L1-loss and 
    ///   L2-loss linear epsilon-vector regression (epsilon-SVR).
    /// </para>
    /// 
    /// <code>
    ///   min_\beta  0.5\beta^T (Q + diag(lambda)) \beta - p \sum_{i=1}^l|\beta_i| + \sum_{i=1}^l yi\beta_i,
    ///     s.t.     -upper_bound_i &lt;= \beta_i &lt;= upper_bound_i,
    /// </code>
    /// 
    /// <para>
    ///  where Qij = yi yj xi^T xj and
    ///  D is a diagonal matrix </para>
    ///
    /// <para>
    /// In L1-SVM case:</para>
    /// <code>
    ///    upper_bound_i = C
    ///    lambda_i = 0
    /// </code>
    /// <para>
    /// In L2-SVM case:</para>
    /// <code>
    ///    upper_bound_i = INF
    ///    lambda_i = 1/(2*C)
    /// </code>
    /// 
    /// <para>
    /// Given: x, y, p, C and eps as the stopping tolerance</para>
    ///
    /// <para>
    /// See Algorithm 4 of Ho and Lin, 2012.</para>
    /// </remarks>
    /// 
    /// <see cref="SequentialMinimalOptimization{TKernel}"/>
    /// <see cref="LinearNewtonMethod"/>
    /// 
    public class LinearRegressionCoordinateDescent<TKernel, TInput> :
        BaseLinearRegressionCoordinateDescent<SupportVectorMachine<TKernel, TInput>, TKernel, TInput>
        where TKernel : struct, ILinear<TInput>
        where TInput : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinearRegressionCoordinateDescent"/> class.
        /// </summary>
        public LinearRegressionCoordinateDescent()
        {
        }

        /// <summary>
        /// Creates an instance of the model to be learned. Inheritors
        /// of this abstract class must define this method so new models
        /// can be created from the training data.
        /// </summary>
        protected override SupportVectorMachine<TKernel, TInput> Create(int inputs, TKernel kernel)
        {
            return new SupportVectorMachine<TKernel, TInput>(inputs, kernel);
        }
    }

    /// <summary>
    ///   Base class for Coordinate descent algorithm for the L1 or L2-loss linear Support 
    ///   Vector Regression (epsilon-SVR) learning problem in the dual form (-s 12 and -s 13).
    /// </summary>
    /// 
    public abstract class BaseLinearRegressionCoordinateDescent<TModel, TKernel, TInput> :
        BaseSupportVectorRegression<TModel, TKernel, TInput>
        where TModel : SupportVectorMachine<TKernel, TInput>
        where TKernel : struct, ILinear<TInput>
#if !NETSTANDARD1_4
        where TInput : ICloneable
#endif
    {

        int max_iter = 1000;

        private double eps = 0.1;

        private double[] alpha;
        private double[] beta;
        private double[] weights;
        private double bias;

        private Loss loss = Loss.L2;

        /// <summary>
        ///   Constructs a new coordinate descent algorithm for L1-loss and L2-loss SVM dual problems.
        /// </summary>
        /// 
        public BaseLinearRegressionCoordinateDescent()
        {

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
        protected override void InnerRun()
        {
            int samples = Inputs.Length;
            int dimension = Kernel.GetLength(Inputs) + 1;

            // Lagrange multipliers
            this.alpha = new double[samples];
            this.beta = new double[samples];
            this.weights = new double[dimension];

            double[] w = weights;
            TInput[] x = Inputs;
            double[] y = Outputs;

            var random = Accord.Math.Random.Generator.Random;

            // Lagrange multipliers
            Array.Clear(alpha, 0, alpha.Length);
            Array.Clear(beta, 0, beta.Length);
            Array.Clear(w, 0, w.Length);
            bias = 0;


            double C = Complexity;
            double p = Epsilon;
            double eps = Tolerance;
            int iter = 0;
            int active_size = x.Length;
            int[] index = new int[x.Length];

            double d, G, H;
            double Gmax_old = Double.PositiveInfinity;
            double Gmax_new, Gnorm1_new;
            double Gnorm1_init = -1.0; // Gnorm1_init is initialized at the first iteration
            double[] QD = new double[x.Length];

            // L2R_L2LOSS_SVR_DUAL
            double lambda = 0.5 / C;
            double upper_bound = Double.PositiveInfinity;

            if (loss == Loss.L1)
            {
                lambda = 0;
                upper_bound = C;
            }


            for (int i = 0; i < x.Length; i++)
            {
                QD[i] = 0;

                TInput xi = x[i];

                QD[i] = Kernel.Function(xi, xi);
                Kernel.Product(beta[i], xi, accumulate: w);
                index[i] = i;
            }


            while (iter < max_iter)
            {
                if (Token.IsCancellationRequested)
                    break;

                int i;
                Gmax_new = 0;
                Gnorm1_new = 0;

                for (i = 0; i < active_size; i++)
                {
                    int j = i + random.Next() % (active_size - i);

                    var old = index[i];
                    index[i] = index[j];
                    index[j] = old;
                }

                for (int s = 0; s < active_size; s++)
                {
                    i = index[s];
                    G = -y[i] + lambda * beta[i];
                    H = QD[i] + lambda;

                    G += Kernel.Function(w, x[i]);

                    double Gp = G + p;
                    double Gn = G - p;
                    double violation = 0;

                    if (beta[i] == 0)
                    {
                        if (Gp < 0)
                        {
                            violation = -Gp;
                        }
                        else if (Gn > 0)
                        {
                            violation = Gn;
                        }
                        else if (Gp > Gmax_old && Gn < -Gmax_old)
                        {
                            active_size--;

                            var old = index[s];
                            index[s] = index[active_size];
                            index[active_size] = old;

                            s--;
                            continue;
                        }
                    }
                    else if (beta[i] >= upper_bound)
                    {
                        if (Gp > 0)
                        {
                            violation = Gp;
                        }
                        else if (Gp < -Gmax_old)
                        {
                            active_size--;

                            var old = index[s];
                            index[s] = index[active_size];
                            index[active_size] = old;

                            s--;
                            continue;
                        }
                    }
                    else if (beta[i] <= -upper_bound)
                    {
                        if (Gn < 0)
                        {
                            violation = -Gn;
                        }
                        else if (Gn > Gmax_old)
                        {
                            active_size--;

                            var old = index[s];
                            index[s] = index[active_size];
                            index[active_size] = old;

                            s--;
                            continue;
                        }
                    }
                    else if (beta[i] > 0)
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
                    if (Gp < H * beta[i])
                    {
                        d = -Gp / H;
                    }
                    else if (Gn > H * beta[i])
                    {
                        d = -Gn / H;
                    }
                    else
                    {
                        d = -beta[i];
                    }

                    if (Math.Abs(d) < 1.0e-12)
                        continue;

                    double beta_old = beta[i];
                    beta[i] = Math.Min(Math.Max(beta[i] + d, -upper_bound), upper_bound);
                    d = beta[i] - beta_old;

                    if (d != 0)
                    {
                        Kernel.Product(d, x[i], accumulate: w);
                    }
                }

                if (iter == 0)
                    Gnorm1_init = Gnorm1_new;

                iter++;

                if (iter % 10 == 0)
                    Debug.WriteLine(".");

                if (Gnorm1_new <= eps * Gnorm1_init)
                {
                    if (active_size == x.Length)
                    {
                        break;
                    }
                    else
                    {
                        active_size = x.Length;
                        Debug.WriteLine("*");
                        Gmax_old = Double.PositiveInfinity;
                        continue;
                    }
                }

                Gmax_old = Gmax_new;
            }

            Debug.WriteLine("optimization finished, #iter = " + iter);

            if (iter >= max_iter)
            {
                Debug.WriteLine("WARNING: reaching max number of iterations");
                Debug.WriteLine("Using -s 11 may be faster (also see FAQ)");
            }

            // calculate objective value
            double v = 0;
            int nSV = 0;

            for (int i = 0; i < w.Length; i++)
                v += w[i] * w[i];
            v = 0.5 * v;

            for (int i = 0; i < x.Length; i++)
            {
                v += p * Math.Abs(beta[i]) - y[i] * beta[i] + 0.5 * lambda * beta[i] * beta[i];

                if (beta[i] != 0)
                    nSV++;
            }

            Debug.WriteLine("Objective value = " + v);
            Debug.WriteLine("nSV = " + nSV);


            Model.SupportVectors = new[] { Kernel.CreateVector(w) };
            Model.Weights = new[] { 1.0 };
            Model.Threshold = bias;
        }


        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        protected BaseLinearRegressionCoordinateDescent(TModel model, TInput[] input, double[] output)
            : base(model, input, output)
        {
        }

    }
}
