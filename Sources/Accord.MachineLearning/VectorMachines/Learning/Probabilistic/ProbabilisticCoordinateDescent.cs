// Accord Statistics Library
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

namespace Accord.MachineLearning.VectorMachines.Learning
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Accord.Statistics.Links;
    using Accord.Statistics.Kernels;
    using Accord.Math;
    using Statistics.Models.Regression.Fitting;
    using Statistics.Models.Regression;
    using Accord.Compat;

    /// <summary>
    ///   L1-regularized logistic regression (probabilistic SVM) 
    ///   learning algorithm (-s 6).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class implements a <see cref="SupportVectorMachine"/> learning algorithm
    ///   specifically crafted for probabilistic linear machines only. It provides a L1-
    ///   regularized coordinate descent learning algorithm for optimizing the learning
    ///   problem. The code has been based on liblinear's method <c>solve_l1r_lr</c> 
    ///   method, whose original description is provided below.
    /// </para>
    /// 
    /// <para>
    ///   Liblinear's solver <c>-s 6</c>: <c>L1R_LR</c>. 
    ///   A coordinate descent algorithm for L1-regularized
    ///   logistic regression (probabilistic svm) problems.
    /// </para>
    /// 
    /// <code>
    ///   min_w \sum |wj| + C \sum log(1+exp(-yi w^T xi)),
    /// </code>
    /// 
    /// <para>
    /// Given: x, y, Cp, Cn, and eps as the stopping tolerance</para>
    ///
    /// <para>
    /// See Yuan et al. (2011) and appendix of LIBLINEAR paper, Fan et al. (2008)</para>
    /// </remarks>
    /// 
    /// <examples>
    /// <para>
    ///   Probabilistic SVMs are exactly the same as logistic regression models 
    ///   trained using a large-margin decision criteria. As such, any linear SVM 
    ///   learning algorithm can be used to obtain <see cref="LogisticRegression"/>
    ///   objects as well.</para>
    ///   
    /// <para>
    ///   The following example shows how to obtain a <see cref="LogisticRegression"/> 
    ///   from a probabilistic linear <see cref="SupportVectorMachine"/>. It contains
    ///   exactly the same data used in the <see cref="IterativeReweightedLeastSquares"/>
    ///   documentation page for <see cref="LogisticRegression"/>.</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\Probabilistic\ProbabilisticCoordinateDescentTest.cs" region="doc_logreg"/>
    /// </examples>
    /// 
    /// <see cref="SequentialMinimalOptimization{TKernel}"/>
    /// <see cref="ProbabilisticNewtonMethod"/>
    /// 
    public class ProbabilisticCoordinateDescent :
        BaseProbabilisticCoordinateDescent<SupportVectorMachine, Linear, double[]>,
        ILinearSupportVectorMachineLearning
    {
        /// <summary>
        ///   Constructs a new Newton method algorithm for L1-regularized
        ///   logistic regression (probabilistic linear vector machine).
        /// </summary>
        /// 
        public ProbabilisticCoordinateDescent()
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

        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        [Obsolete("Please do not pass parameters in the constructor. Use the default constructor and the Learn method instead.")]
        public ProbabilisticCoordinateDescent(ISupportVectorMachine<double[]> model, double[][] input, int[] output)
            : base(model, input, output)
        {
        }
    }

    /// <summary>
    ///   L1-regularized logistic regression (probabilistic SVM) 
    ///   learning algorithm (-s 6).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class implements a <see cref="SupportVectorMachine"/> learning algorithm
    ///   specifically crafted for probabilistic linear machines only. It provides a L1-
    ///   regularized coordinate descent learning algorithm for optimizing the learning
    ///   problem. The code has been based on liblinear's method <c>solve_l1r_lr</c> 
    ///   method, whose original description is provided below.
    /// </para>
    /// 
    /// <para>
    ///   Liblinear's solver <c>-s 6</c>: <c>L1R_LR</c>. 
    ///   A coordinate descent algorithm for L1-regularized
    ///   logistic regression (probabilistic svm) problems.
    /// </para>
    /// 
    /// <code>
    ///   min_w \sum |wj| + C \sum log(1+exp(-yi w^T xi)),
    /// </code>
    /// 
    /// <para>
    /// Given: x, y, Cp, Cn, and eps as the stopping tolerance</para>
    ///
    /// <para>
    /// See Yuan et al. (2011) and appendix of LIBLINEAR paper, Fan et al. (2008)</para>
    /// </remarks>
    /// 
    /// <examples>
    /// <para>
    ///   Probabilistic SVMs are exactly the same as logistic regression models 
    ///   trained using a large-margin decision criteria. As such, any linear SVM 
    ///   learning algorithm can be used to obtain <see cref="LogisticRegression"/>
    ///   objects as well.</para>
    ///   
    /// <para>
    ///   The following example shows how to obtain a <see cref="LogisticRegression"/> 
    ///   from a probabilistic linear <see cref="SupportVectorMachine"/>. It contains
    ///   exactly the same data used in the <see cref="IterativeReweightedLeastSquares"/>
    ///   documentation page for <see cref="LogisticRegression"/>.</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\Probabilistic\ProbabilisticCoordinateDescentTest.cs" region="doc_logreg"/>
    /// </examples>
    /// 
    /// <see cref="SequentialMinimalOptimization{TKernel}"/>
    /// <see cref="ProbabilisticNewtonMethod"/>
    /// 
    public class ProbabilisticCoordinateDescent<TKernel, TInput> :
        BaseProbabilisticCoordinateDescent<SupportVectorMachine<TKernel, TInput>, TKernel, TInput>
        where TKernel : struct, ILinear<TInput>
    {
        /// <summary>
        ///   Constructs a new Newton method algorithm for L1-regularized
        ///   logistic regression (probabilistic linear vector machine).
        /// </summary>
        /// 
        public ProbabilisticCoordinateDescent()
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
    ///   Base class for L1-regularized logistic regression (probabilistic SVM) learning algorithm (-s 6).
    /// </summary>
    /// 
    public abstract class BaseProbabilisticCoordinateDescent<TModel, TKernel, TInput> :
        BaseSupportVectorClassification<TModel, TKernel, TInput>
        where TModel : SupportVectorMachine<TKernel, TInput>
        where TKernel : struct, ILinear<TInput>
    {
        double[] weights;
        int biasIndex;

        double eps = 0.01;

        int max_iter = 1000;
        int max_newton_iter = 100;
        int max_num_linesearch = 20;


        /// <summary>
        /// Initializes a new instance of the <see cref="BaseProbabilisticCoordinateDescent{TModel, TKernel, TInput}"/> class.
        /// </summary>
        protected BaseProbabilisticCoordinateDescent()
        {

        }

        /// <summary>
        ///   Gets or sets the maximum number of iterations that should
        ///   be performed until the algorithm stops. Default is 1000.
        /// </summary>
        /// 
        public int MaximumIterations
        {
            get { return max_iter; }
            set { max_iter = value; }
        }

        /// <summary>
        ///   Gets or sets the maximum number of line searches
        ///   that can be performed per iteration. Default is 20.
        /// </summary>
        /// 
        public int MaximumLineSearches
        {
            get { return max_num_linesearch; }
            set { max_num_linesearch = value; }
        }

        /// <summary>
        ///   Gets or sets the maximum number of inner iterations that can
        ///   be performed by the inner solver algorithm. Default is 100.
        /// </summary>
        /// 
        public int MaximumNewtonIterations
        {
            get { return max_newton_iter; }
            set { max_newton_iter = value; }
        }

        /// <summary>
        ///   Convergence tolerance. Default value is 0.01.
        /// </summary>
        /// 
        /// <remarks>
        ///   The criterion for completing the model training process. The default is 0.01.
        /// </remarks>
        /// 
        public double Tolerance
        {
            get { return eps; }
            set { eps = value; }
        }

        private double[][] transposeAndAugment(TInput[] input)
        {
            double[][] inputs = Kernel.ToDouble(input);

            int oldRows = inputs.Length;
            int oldCols = inputs[0].Length;

            double[] ones = new double[oldRows];
            for (int i = 0; i < ones.Length; i++)
                ones[i] = 1.0;

            var x = new double[oldCols + 1][];
            for (int i = 0; i < oldCols; i++)
            {
                var col = x[i] = new double[oldRows];
                for (int j = 0; j < col.Length; j++)
                    col[j] = inputs[j][i];
            }

            x[oldCols] = ones;

            return x;
        }


        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        protected override void InnerRun()
        {
            int samples = Inputs.Length;
            int parameters = Kernel.GetLength(Inputs) + 1;
            this.weights = new double[parameters];
            this.biasIndex = parameters - 1;

            int[] y = Outputs;
            double[] w = weights;

            double[][] inputs = transposeAndAugment(Inputs);

            int newton_iter = 0;
            int iter = 0;

            int active_size;
            int QP_active_size;

            double nu = 1e-12;
            double inner_eps = 1;
            double sigma = 0.01;
            double w_norm, w_norm_new;
            double z, G, H;
            double Gnorm1_init = 0;
            double Gmax_old = Double.PositiveInfinity;
            double Gmax_new, Gnorm1_new;
            double QP_Gmax_old = Double.PositiveInfinity;
            double QP_Gmax_new, QP_Gnorm1_new;
            double delta = 0, negsum_xTd, cond;

            int[] index = new int[parameters];
            double[] Hdiag = new double[parameters];
            double[] Grad = new double[parameters];
            double[] wpd = new double[parameters];
            double[] xjneg_sum = new double[parameters];

            double[] xTd = new double[samples];
            double[] exp_wTx = new double[samples];
            double[] exp_wTx_new = new double[samples];
            double[] tau = new double[samples];
            double[] D = new double[samples];
            double[] c = C;

            // double[] C = { Cn, 0, Cp };

            for (int i = 0; i < exp_wTx.Length; i++)
                exp_wTx[i] = 0;

            w_norm = 0;
            for (int j = 0; j < w.Length; j++)
            {
                w_norm += Math.Abs(w[j]);
                wpd[j] = w[j];
                index[j] = j;
                xjneg_sum[j] = 0;

                double[] x = inputs[j];
                
                for (int k = 0; k < x.Length; k++)
                {
                    exp_wTx[k] += w[j] * x[k];

                    if (y[k] == -1)
                        xjneg_sum[j] += c[k] * x[k];
                }
            }


            for (int j = 0; j < exp_wTx.Length; j++)
            {
                exp_wTx[j] = Math.Exp(exp_wTx[j]);

                double tau_tmp = 1 / (1 + exp_wTx[j]);
                tau[j] = c[j] * tau_tmp;
                D[j] = c[j] * exp_wTx[j] * tau_tmp * tau_tmp;
            }

            while (newton_iter < max_newton_iter)
            {
                if (Token.IsCancellationRequested)
                    break;

                Gmax_new = 0;
                Gnorm1_new = 0;
                active_size = w.Length;

                for (int s = 0; s < active_size; s++)
                {
                    int j = index[s];
                    Hdiag[j] = nu;
                    Grad[j] = 0;

                    double tmp = 0;
                    double[] x = inputs[j];

                    for (int k = 0; k < x.Length; k++)
                    {
                        Hdiag[j] += x[k] * x[k] * D[k];
                        tmp += x[k] * tau[k];
                    }


                    Grad[j] = -tmp + xjneg_sum[j];

                    double Gp = Grad[j] + 1;
                    double Gn = Grad[j] - 1;
                    double violation = 0;

                    if (w[j] == 0)
                    {
                        if (Gp < 0)
                            violation = -Gp;

                        else if (Gn > 0)
                            violation = Gn;

                        //outer-level shrinking
                        else if (Gp > Gmax_old / x.Length && Gn < -Gmax_old / x.Length)
                        {
                            active_size--;
                            var t = index[s];
                            index[s] = index[active_size];
                            index[active_size] = t;
                            s--;
                            continue;
                        }
                    }
                    else if (w[j] > 0)
                        violation = Math.Abs(Gp);
                    else
                        violation = Math.Abs(Gn);

                    Gmax_new = Math.Max(Gmax_new, violation);
                    Gnorm1_new += violation;
                }

                if (newton_iter == 0)
                    Gnorm1_init = Gnorm1_new;

                if (Gnorm1_new <= eps * Gnorm1_init)
                    break;

                iter = 0;
                QP_Gmax_old = Double.PositiveInfinity;
                QP_active_size = active_size;

                for (int i = 0; i < xTd.Length; i++)
                    xTd[i] = 0;

                var rand = Accord.Math.Random.Generator.Random;

                // optimize QP over wpd
                while (iter < max_iter)
                {
                    if (Token.IsCancellationRequested)
                        break;

                    QP_Gmax_new = 0;
                    QP_Gnorm1_new = 0;

                    for (int j = 0; j < QP_active_size; j++)
                    {
                        int i = j + rand.Next() % (QP_active_size - j);
                        var t = index[i];
                        index[i] = index[j];
                        index[j] = t;
                    }

                    for (int s = 0; s < QP_active_size; s++)
                    {
                        int j = index[s];
                        H = Hdiag[j];

                        double[] x = inputs[j];
                        G = Grad[j] + (wpd[j] - w[j]) * nu;

                        for (int k = 0; k < x.Length; k++)
                            G += x[k] * D[k] * xTd[k];

                        double Gp = G + 1;
                        double Gn = G - 1;
                        double violation = 0;

                        if (wpd[j] == 0)
                        {
                            if (Gp < 0)
                                violation = -Gp;

                            else if (Gn > 0)
                                violation = Gn;

                            //inner-level shrinking
                            else if (Gp > QP_Gmax_old / inputs.Length && Gn < -QP_Gmax_old / inputs.Length)
                            {
                                QP_active_size--;
                                var t = index[s];
                                index[s] = index[QP_active_size];
                                index[QP_active_size] = t;
                                s--;
                                continue;
                            }
                        }
                        else if (wpd[j] > 0)
                            violation = Math.Abs(Gp);
                        else
                            violation = Math.Abs(Gn);

                        QP_Gmax_new = Math.Max(QP_Gmax_new, violation);
                        QP_Gnorm1_new += violation;

                        // obtain solution of one-variable problem
                        if (Gp < H * wpd[j])
                            z = -Gp / H;
                        else if (Gn > H * wpd[j])
                            z = -Gn / H;
                        else
                            z = -wpd[j];

                        if (Math.Abs(z) < 1.0e-12)
                            continue;

                        z = Math.Min(Math.Max(z, -10.0), 10.0);

                        wpd[j] += z;

                        x = inputs[j];

                        for (int k = 0; k < x.Length; k++)
                            xTd[k] += x[k] * z;
                    }

                    iter++;

                    if (QP_Gnorm1_new <= inner_eps * Gnorm1_init)
                    {
                        //inner stopping
                        if (QP_active_size == active_size)
                            break;

                        //active set reactivation
                        QP_active_size = active_size;
                        QP_Gmax_old = Double.PositiveInfinity;
                        continue;
                    }

                    QP_Gmax_old = QP_Gmax_new;
                }

                if (iter >= max_iter)
                    Trace.WriteLine("WARNING: reaching max number of inner iterations");

                delta = 0;
                w_norm_new = 0;
                for (int j = 0; j < w.Length; j++)
                {
                    delta += Grad[j] * (wpd[j] - w[j]);

                    if (wpd[j] != 0)
                        w_norm_new += Math.Abs(wpd[j]);
                }

                delta += (w_norm_new - w_norm);

                negsum_xTd = 0;
                for (int i = 0; i < y.Length; i++)
                {
                    if (y[i] == -1)
                        negsum_xTd += c[i] * xTd[i];
                }

                int num_linesearch;
                for (num_linesearch = 0; num_linesearch < max_num_linesearch; num_linesearch++)
                {
                    cond = w_norm_new - w_norm + negsum_xTd - sigma * delta;

                    for (int i = 0; i < xTd.Length; i++)
                    {
                        double exp_xTd = Math.Exp(xTd[i]);
                        exp_wTx_new[i] = exp_wTx[i] * exp_xTd;
                        cond += c[i] * Math.Log((1 + exp_wTx_new[i]) / (exp_xTd + exp_wTx_new[i]));
                    }

                    if (cond <= 0)
                    {
                        w_norm = w_norm_new;
                        for (int j = 0; j < w.Length; j++)
                            w[j] = wpd[j];

                        for (int i = 0; i < y.Length; i++)
                        {
                            exp_wTx[i] = exp_wTx_new[i];

                            double tau_tmp = 1 / (1 + exp_wTx[i]);
                            tau[i] = c[i] * tau_tmp;
                            D[i] = c[i] * exp_wTx[i] * tau_tmp * tau_tmp;
                        }

                        break;
                    }
                    else
                    {
                        w_norm_new = 0;
                        for (int j = 0; j < w.Length; j++)
                        {
                            wpd[j] = (w[j] + wpd[j]) * 0.5;

                            if (wpd[j] != 0)
                                w_norm_new += Math.Abs(wpd[j]);
                        }

                        delta *= 0.5;
                        negsum_xTd *= 0.5;
                        for (int i = 0; i < xTd.Length; i++)
                            xTd[i] *= 0.5;
                    }
                }

                // Recompute some info due to too many line search steps
                if (num_linesearch >= max_num_linesearch)
                {
                    for (int i = 0; i < exp_wTx.Length; i++)
                        exp_wTx[i] = 0;

                    for (int i = 0; i < w.Length; i++)
                    {
                        if (w[i] == 0)
                            continue;

                        double[] x = inputs[i];
                        for (int k = 0; k < x.Length; k++)
                            exp_wTx[k] += w[i] * x[k];
                    }

                    for (int i = 0; i < exp_wTx.Length; i++)
                        exp_wTx[i] = Math.Exp(exp_wTx[i]);
                }

                if (iter == 1)
                    inner_eps *= 0.25;

                newton_iter++;
                Gmax_old = Gmax_new;

                Trace.WriteLine("iter = " + newton_iter + "_#CD cycles " + iter);
            }

            Trace.WriteLine("=========================");
            Trace.WriteLine("optimization finished, #iter = " + newton_iter);

            if (newton_iter >= max_newton_iter)
                Trace.WriteLine("WARNING: reaching max number of iterations");

            // calculate objective value

            double v = 0;
            int nnz = 0;
            for (int j = 0; j < w.Length; j++)
            {
                if (w[j] != 0)
                {
                    v += Math.Abs(w[j]);
                    nnz++;
                }
            }

            for (int j = 0; j < y.Length; j++)
            {
                if (y[j] == 1)
                    v += c[j] * Math.Log(1 + 1 / exp_wTx[j]);
                else
                    v += c[j] * Math.Log(1 + exp_wTx[j]);
            }

            Trace.WriteLine("Objective value = " + v);
            Trace.WriteLine("#nonzeros/#features = " + nnz + "/" + w.Length);

            Model.Weights = new double[] { 1.0 };
            Model.SupportVectors = new[] { Kernel.CreateVector(weights.First(weights.Length - 1)) };
            Model.Threshold = weights[biasIndex];
            Model.IsProbabilistic = true;
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        public BaseProbabilisticCoordinateDescent(ISupportVectorMachine<TInput> model, TInput[] input, int[] output)
            : base(model, input, output)
        {
        }
    }
}
