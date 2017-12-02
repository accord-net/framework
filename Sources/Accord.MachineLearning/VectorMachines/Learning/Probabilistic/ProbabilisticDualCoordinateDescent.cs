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
    using Statistics.Models.Regression;
    using Statistics.Models.Regression.Fitting;
    using Accord.Compat;

    /// <summary>
    ///   L2-regularized logistic regression (probabilistic support 
    ///   vector machine) learning algorithm in the dual form (-s 7).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class implements a <see cref="SupportVectorMachine"/> learning algorithm
    ///   specifically crafted for probabilistic linear machines only. It provides a L2-
    ///   regularized coordinate descent learning algorithm for optimizing the dual form 
    ///   of the learning problem. The code has been based on liblinear's method 
    ///   <c>solve_l2r_lr_dual</c> method, whose original description is provided below.
    /// </para>
    /// 
    /// <para>
    ///   Liblinear's solver <c>-s 7</c>: <c>L2R_LR_DUAL</c>. A coordinate descent 
    ///   algorithm for the dual of L2-regularized logistic regression problems.
    /// </para>
    /// 
    /// <code>
    ///   min_\alpha  0.5(\alpha^T Q \alpha) + \sum \alpha_i log (\alpha_i) 
    ///     + (upper_bound_i - \alpha_i) log (upper_bound_i - \alpha_i),
    ///     
    ///    s.t.      0 &lt;= \alpha_i &lt;= upper_bound_i,
    /// </code>
    /// 
    /// <para>
    ///  where <c>Qij = yi yj xi^T xj</c> and</para>
    ///  
    /// <code>
    ///  upper_bound_i = Cp if y_i = 1
    ///  upper_bound_i = Cn if y_i = -1
    /// </code>
    /// 
    /// <para>
    /// Given: x, y, Cp, Cn, and eps as the stopping tolerance</para>
    ///
    /// <para>
    /// See Algorithm 5 of Yu et al., MLJ 2010.</para>
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
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\Probabilistic\ProbabilisticDualCoordinateDescentTest.cs" region="doc_logreg"/>
    /// </examples>
    /// 
    /// <see cref="SequentialMinimalOptimization{TKernel}"/>
    /// <see cref="ProbabilisticNewtonMethod"/>
    /// 
    public class ProbabilisticDualCoordinateDescent :
        BaseProbabilisticDualCoordinateDescent<SupportVectorMachine, Linear, double[]>,
        ILinearSupportVectorMachineLearning
    {
        /// <summary>
        ///   Constructs a new Newton method algorithm for L2-regularized
        ///   logistic regression (probabilistic linear SVMs) dual problems.
        /// </summary>
        /// 
        public ProbabilisticDualCoordinateDescent()
        {
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please do not pass parameters in the constructor. Use the default constructor and the Learn method instead.")]
        public ProbabilisticDualCoordinateDescent(SupportVectorMachine model, double[][] input, int[] output)
            : base(model, input, output)
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
    ///   L2-regularized logistic regression (probabilistic support 
    ///   vector machine) learning algorithm in the dual form (-s 7).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class implements a <see cref="SupportVectorMachine"/> learning algorithm
    ///   specifically crafted for probabilistic linear machines only. It provides a L2-
    ///   regularized coordinate descent learning algorithm for optimizing the dual form 
    ///   of the learning problem. The code has been based on liblinear's method 
    ///   <c>solve_l2r_lr_dual</c> method, whose original description is provided below.
    /// </para>
    /// 
    /// <para>
    ///   Liblinear's solver <c>-s 7</c>: <c>L2R_LR_DUAL</c>. A coordinate descent 
    ///   algorithm for the dual of L2-regularized logistic regression problems.
    /// </para>
    /// 
    /// <code>
    ///   min_\alpha  0.5(\alpha^T Q \alpha) + \sum \alpha_i log (\alpha_i) 
    ///     + (upper_bound_i - \alpha_i) log (upper_bound_i - \alpha_i),
    ///     
    ///    s.t.      0 &lt;= \alpha_i &lt;= upper_bound_i,
    /// </code>
    /// 
    /// <para>
    ///  where <c>Qij = yi yj xi^T xj</c> and</para>
    ///  
    /// <code>
    ///  upper_bound_i = Cp if y_i = 1
    ///  upper_bound_i = Cn if y_i = -1
    /// </code>
    /// 
    /// <para>
    /// Given: x, y, Cp, Cn, and eps as the stopping tolerance</para>
    ///
    /// <para>
    /// See Algorithm 5 of Yu et al., MLJ 2010.</para>
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
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\Probabilistic\ProbabilisticDualCoordinateDescentTest.cs" region="doc_logreg"/>
    /// </examples>
    /// 
    /// <see cref="SequentialMinimalOptimization{TKernel}"/>
    /// <see cref="ProbabilisticNewtonMethod"/>
    /// 
    public class ProbabilisticDualCoordinateDescent<TKernel, TInput> :
        BaseProbabilisticDualCoordinateDescent<SupportVectorMachine<TKernel, TInput>, TKernel, TInput>
        where TKernel : ILinear<TInput>
    {
        /// <summary>
        ///   Constructs a new Newton method algorithm for L2-regularized
        ///   logistic regression (probabilistic linear SVMs) dual problems.
        /// </summary>
        /// 
        public ProbabilisticDualCoordinateDescent()
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
    ///   Base class for L2-regularized logistic regression (probabilistic support 
    ///   vector machine) learning algorithm in the dual form (-s 7).
    /// </summary>
    /// 
    public abstract class BaseProbabilisticDualCoordinateDescent<TModel, TKernel, TInput> :
            BaseSupportVectorClassification<TModel, TKernel, TInput>
        where TModel : SupportVectorMachine<TKernel, TInput>
        where TKernel : ILinear<TInput>
    {
        double[] weights;
        int biasIndex;

        double eps = 0.1;

        int max_iter = 1000;
        int max_inner_iter = 100; // for inner Newton

        /// <summary>
        ///   Constructs a new Newton method algorithm for L2-regularized
        ///   logistic regression (probabilistic linear SVMs) dual problems.
        /// </summary>
        /// 
        protected BaseProbabilisticDualCoordinateDescent()
        {
        }


        /// <summary>
        ///   Gets or sets the maximum number of iterations that should
        ///    be performed until the algorithm stops. Default is 1000.
        /// </summary>
        /// 
        public int MaximumIterations
        {
            get { return max_iter; }
            set { max_iter = value; }
        }

        /// <summary>
        ///   Gets or sets the maximum number of inner iterations that can
        ///   be performed by the inner solver algorithm. Default is 100.
        /// </summary>
        /// 
        public int MaximumNewtonIterations
        {
            get { return max_inner_iter; }
            set { max_inner_iter = value; }
        }

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
            get { return eps; }
            set { eps = value; }
        }


        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        protected override void InnerRun()
        {
            int iter = 0;

            int samples = Inputs.Length;
            int parameters = Kernel.GetLength(Inputs);
            this.weights = new double[parameters + 1];
            this.biasIndex = parameters;

            TInput[] inputs = Inputs;
            int[] y = Outputs;
            double[] w = weights;


            double[] xTx = new double[inputs.Length];
            int[] index = new int[inputs.Length];
            double[] alpha = new double[2 * inputs.Length]; // store alpha and C - alpha

            double innereps = 1e-2;
            double innereps_min = System.Math.Min(1e-8, eps);

            double[] upper_bound = this.C; // = { Cn, 0, Cp };


            // Initial alpha can be set here. Note that
            // 0 < alpha[i] < upper_bound[GETI(i)]
            // alpha[2*i] + alpha[2*i+1] = upper_bound[GETI(i)]

            for (int i = 0; i < upper_bound.Length; i++)
            {
                alpha[2 * i] = Math.Min(0.001 * upper_bound[i], 1e-8);
                alpha[2 * i + 1] = upper_bound[i] - alpha[2 * i];
            }

            for (int i = 0; i < inputs.Length; i++)
            {
                index[i] = i;
                xTx[i] = 1;

                TInput xi = inputs[i];

                xTx[i] += Kernel.Function(xi, xi);
                Kernel.Product(y[i] * alpha[2 * i], xi, accumulate: w);
                w[biasIndex] += y[i] * alpha[2 * i];
            }

            var rand = Accord.Math.Random.Generator.Random;

            while (iter < max_iter)
            {
                if (Token.IsCancellationRequested)
                    break;

                for (int i = 0; i < inputs.Length; i++)
                {
                    int j = i + rand.Next() % (inputs.Length - i);

                    var t = index[i];
                    index[i] = index[j];
                    index[j] = t;
                }

                int newton_iter = 0;
                double Gmax = 0;

                for (int s = 0; s < index.Length; s++)
                {
                    int i = index[s];
                    int yi = y[i];
                    double C = upper_bound[i];

                    double xisq = xTx[i];
                    double ywTx = y[i] * (Kernel.Function(w, inputs[i]) + w[biasIndex]);

                    double a = xisq;
                    double b = ywTx;

                    // Decide to minimize g_1(z) or g_2(z)
                    int ind1 = 2 * i;
                    int ind2 = 2 * i + 1;
                    int sign = 1;

                    if (0.5 * a * (alpha[ind2] - alpha[ind1]) + b < 0)
                    {
                        ind1 = 2 * i + 1;
                        ind2 = 2 * i;
                        sign = -1;
                    }

                    //  g_t(z) = z*log(z) + (C-z)*log(C-z) + 0.5a(z-alpha_old)^2 + sign*b(z-alpha_old)
                    double alpha_old = alpha[ind1];
                    double z = alpha_old;

                    if (C - z < 0.5 * C)
                        z = 0.1 * z;

                    double gp = a * (z - alpha_old) + sign * b + Math.Log(z / (C - z));
                    Gmax = Math.Max(Gmax, Math.Abs(gp));

                    // Newton method on the sub-problem
                    const double eta = 0.1; // xi in the paper

                    int inner_iter = 0;
                    while (inner_iter <= max_inner_iter)
                    {
                        if (Math.Abs(gp) < innereps)
                            break;

                        double gpp = a + C / (C - z) / z;
                        double tmpz = z - gp / gpp;

                        if (tmpz <= 0)
                            z *= eta;
                        else // tmpz in (0, C)
                            z = tmpz;

                        gp = a * (z - alpha_old) + sign * b + Math.Log(z / (C - z));
                        newton_iter++;
                        inner_iter++;
                    }

                    if (inner_iter > 0) // update w
                    {
                        alpha[ind1] = z;
                        alpha[ind2] = C - z;

                        Kernel.Product(sign * (z - alpha_old) * yi, inputs[i], accumulate: w);
                        w[biasIndex] += sign * (z - alpha_old) * yi;
                    }
                }

                iter++;
                if (iter % 10 == 0)
                    Trace.Write(".");

                if (Gmax < eps)
                    break;

                if (newton_iter <= inputs.Length / 10)
                    innereps = Math.Max(innereps_min, 0.1 * innereps);

            }

            Trace.WriteLine("optimization finished, #iter = " + iter);
            if (iter >= max_iter)
            {
                Trace.WriteLine("WARNING: reaching max number of iterations");
                Trace.WriteLine("Using -s 0 may be faster (also see FAQ)");
            }

            // calculate objective value

            double v = 0;
            for (int i = 0; i < w.Length; i++)
                v += w[i] * w[i];
            v *= 0.5;

            for (int i = 0; i < upper_bound.Length; i++)
            {
                v += alpha[2 * i] * Math.Log(alpha[2 * i])
                    + alpha[2 * i + 1] * Math.Log(alpha[2 * i + 1])
                    - upper_bound[i] * Math.Log(upper_bound[i]);
            }

            Trace.WriteLine("Objective value = " + v);

            Model.Weights = new double[] { 1.0 };
            Model.SupportVectors = new[] { Kernel.CreateVector(w.First(w.Length - 1)) };
            Model.Threshold = weights[biasIndex];
            Model.IsProbabilistic = true;
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please do not pass parameters in the constructor. Use the default constructor and the Learn method instead.")]
        public BaseProbabilisticDualCoordinateDescent(SupportVectorMachine<TKernel, TInput> model, TInput[] input, int[] output)
            : base(model, input, output)
        {
        }
    }
}
