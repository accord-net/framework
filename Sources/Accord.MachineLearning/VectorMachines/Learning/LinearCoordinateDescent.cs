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
    using Accord.Math;
    using System.Diagnostics;
    using System.Threading;
    using Statistics.Models.Regression.Linear;

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
    /// <examples>
    /// <para>
    ///   The following example shows how to obtain a <see cref="MultipleLinearRegression"/> 
    ///   from a linear <see cref="SupportVectorMachine"/>. It contains exactly the same data 
    ///   used in the <see cref="OrdinaryLeastSquares"/> documentation page for 
    ///   <see cref="MultipleLinearRegression"/>.</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\LinearCoordinateDescentTest.cs" region="doc_linreg"/>
    /// </examples>
    /// 
    /// <see cref="SequentialMinimalOptimization{TKernel}"/>
    /// <see cref="LinearNewtonMethod"/>
    /// <see cref="LinearDualCoordinateDescent"/>
    /// 
    public class LinearCoordinateDescent :
        BaseLinearCoordinateDescent<SupportVectorMachine, Linear>,
        ILinearSupportVectorMachineLearning
    {
        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please do not pass parameters in the constructor. Use the default constructor and the Learn method instead.")]
        public LinearCoordinateDescent(SupportVectorMachine model, double[][] input, int[] output)
            : base(model, input, output)
        {
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please do not pass parameters in the constructor. Use the default constructor and the Learn method instead.")]
        public LinearCoordinateDescent(KernelSupportVectorMachine model, double[][] input, int[] output)
            : base(model, input, output)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearCoordinateDescent"/> class.
        /// </summary>
        public LinearCoordinateDescent()
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
    /// <see cref="SequentialMinimalOptimization{TKernel}"/>
    /// <see cref="LinearNewtonMethod"/>
    /// <see cref="LinearDualCoordinateDescent"/>
    /// 
    public class LinearCoordinateDescent<TKernel> :
        BaseLinearCoordinateDescent<SupportVectorMachine<TKernel>, TKernel>
        where TKernel : ILinear
    {
        /// <summary>
        /// Creates an instance of the model to be learned. Inheritors
        /// of this abstract class must define this method so new models
        /// can be created from the training data.
        /// </summary>
        protected override SupportVectorMachine<TKernel> Create(int inputs, TKernel kernel)
        {
            return new SupportVectorMachine<TKernel>(inputs, kernel);
        }
    }

    // TODO: Add support for sparse linear kernels
    //public class LinearCoordinateDescent<TKernel, TInput> :
    //    BaseLinearCoordinateDescent<SupportVectorMachine<TKernel, TInput>, TKernel, TInput>
    //    where TKernel : struct, ILinear<TInput>
    //    where TInput : ICloneable, IList
    //{
    //    /// <summary>
    //    /// Creates an instance of the model to be learned. Inheritors
    //    /// of this abstract class must define this method so new models
    //    /// can be created from the training data.
    //    /// </summary>
    //    protected override SupportVectorMachine<TKernel, TInput> Create(int inputs, TKernel kernel)
    //    {
    //        return new SupportVectorMachine<TKernel, TInput>(inputs, kernel);
    //    }
    //}

    ///// <summary>
    /////   Base class for linear coordinate descent learning algorithm.
    ///// </summary>
    ///// 
    //public abstract class BaseLinearCoordinateDescent<TModel, TKernel, TInput> :
    //    BaseSupportVectorClassification<TModel, TKernel, TInput>
    //    where TModel : SupportVectorMachine<TKernel, TInput>
    //    where TKernel : struct, ILinear<TInput>
    //    where TInput : ICloneable, IList

        /// <summary>
        ///   Base class for linear coordinate descent learning algorithm.
        /// </summary>
        /// 
        public abstract class BaseLinearCoordinateDescent<TModel, TKernel> :
        BaseSupportVectorClassification<TModel, TKernel, double[]>
        where TModel : SupportVectorMachine<TKernel, double[]>
        where TKernel : ILinear
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
        public BaseLinearCoordinateDescent()
        {

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
            int dimension = Inputs[0].Length;

            // Lagrange multipliers
            this.alpha = new double[samples];
            this.weights = new double[dimension + 1];
            this.bias = new double[samples];
            this.biasIndex = dimension;

            double[] w = weights;


            var random = Accord.Math.Random.Generator.Random;

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

            double[] c = C;
            double[][] x = Inputs;
            int[] y = Outputs;
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
                        double val = x[i][j] * y[i];
                        b[i] -= w[j] * val;
                        xj_sq[j] += c[i] * val * val;
                    }
                }
            }

            while (iter < max_iter)
            {
                if (Token.IsCancellationRequested)
                    break;

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
                                double val = x[i][j] * y[i];
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
                                    b[i] += d_diff * x[i][j] * y[i];
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

                                    double b_new = b[i] + d_diff * x[i][j]* y[i];

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
                                    double b_new = b[i] + d_diff * x[i][j]* y[i];
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

            Model.SupportVectors = new[] { w.First(Model.NumberOfInputs) };
            Model.Weights = new[] { 1.0 };
            Model.Threshold = w[biasIndex];
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete()]
        protected BaseLinearCoordinateDescent(ISupportVectorMachine<double[]> model, double[][] input, int[] output)
            : base(model, input, output)
        {
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        protected BaseLinearCoordinateDescent(TModel model, double[][] input, int[] output)
            : base(model, input, output)
        {
        }
    }
}
