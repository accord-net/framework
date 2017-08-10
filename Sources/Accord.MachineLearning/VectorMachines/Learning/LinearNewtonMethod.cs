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
    using Accord.Math.Optimization;
    using System.Threading;
    using Accord.Statistics.Kernels;
    using Accord.Math;
    using Statistics.Models.Regression.Linear;
    using Diagnostics;

    /// <summary>
    ///   L2-regularized L2-loss linear support vector classification (primal).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class implements a L2-regularized L2-loss support vector machine
    ///   learning algorithm that operates in the primal form of the optimization
    ///   problem. This method has been based on liblinear's <c>l2r_l2_svc_fun</c>
    ///   problem specification, optimized using a <see cref="TrustRegionNewtonMethod">
    ///   Trust-region Newton method</see>. This method might be faster than the often
    ///   preferred <see cref="LinearDualCoordinateDescent"/>. </para>
    ///   
    /// <para>
    ///   Liblinear's solver <c>-s 2</c>: <c>L2R_L2LOSS_SVC</c>. A trust region newton
    ///   algorithm for the primal of L2-regularized, L2-loss linear support vector 
    ///   classification.
    /// </para>
    /// </remarks>
    /// 
    /// <examples>
    /// <para>
    ///   The following example shows how to obtain a <see cref="MultipleLinearRegression"/> 
    ///   from a linear <see cref="SupportVectorMachine"/>. It contains exactly the same data 
    ///   used in the <see cref="OrdinaryLeastSquares"/> documentation page for 
    ///   <see cref="MultipleLinearRegression"/>.</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\LinearNewtonMethodTest.cs" region="doc_linreg"/>
    /// </examples>
    /// 
    /// 
    /// <seealso cref="SequentialMinimalOptimization{TKernel}"/>
    /// <seealso cref="LinearDualCoordinateDescent"/>
    /// 
    public class LinearNewtonMethod :
        BaseLinearNewtonMethod<SupportVectorMachine, Linear>,
        ILinearSupportVectorMachineLearning
    {
        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please do not pass parameters in the constructor. Use the default constructor and the Learn method instead.")]
        public LinearNewtonMethod(KernelSupportVectorMachine model, double[][] input, int[] output)
            : base(model, input, output)
        {
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please do not pass parameters in the constructor. Use the default constructor and the Learn method instead.")]
        public LinearNewtonMethod(SupportVectorMachine model, double[][] input, int[] output)
            : base(model, input, output)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LinearNewtonMethod"/> class.
        /// </summary>
        /// 
        public LinearNewtonMethod()
        {

        }

        /// <summary>
        ///   Creates an instance of the model to be learned. Inheritors
        ///   of this abstract class must define this method so new models
        ///   can be created from the training data.
        /// </summary>
        /// 
        protected override SupportVectorMachine Create(int inputs, Linear kernel)
        {
            return new SupportVectorMachine(inputs) { Kernel = kernel };
        }

        internal static void Xv<TKernel, TInput>(TKernel kernel, TInput[] x, int biasIndex, double[] v, double[] Xv)
            where TKernel : struct, ILinear<TInput>
        {
            for (int i = 0; i < x.Length; i++)
                Xv[i] = kernel.Function(v, x[i]) + v[biasIndex];
        }

        internal static void subXv<TKernel, TInput>(TKernel kernel, TInput[] x, int biasIndex, int[] I, int sizeI, double[] v, double[] Xv)
            where TKernel : struct, ILinear<TInput>
        {
            for (int i = 0; i < sizeI; i++)
                Xv[i] = kernel.Function(v, x[I[i]]) + v[biasIndex];
        }

        internal static void subXTv<TKernel, TInput>(TKernel kernel, TInput[] x, int biasIndex, int[] I, int sizeI, double[] v, double[] XTv)
            where TKernel : struct, ILinear<TInput>
        {
            for (int i = 0; i < XTv.Length; i++)
                XTv[i] = 0;

            for (int i = 0; i < sizeI; i++)
            {
                kernel.Product(v[i], x[I[i]], accumulate: XTv);
                XTv[biasIndex] += v[i];
            }
        }
    }

    /// <summary>
    ///   L2-regularized L2-loss linear support vector classification (primal).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class implements a L2-regularized L2-loss support vector machine
    ///   learning algorithm that operates in the primal form of the optimization
    ///   problem. This method has been based on liblinear's <c>l2r_l2_svc_fun</c>
    ///   problem specification, optimized using a <see cref="TrustRegionNewtonMethod">
    ///   Trust-region Newton method</see>. This method might be faster than the often
    ///   preferred <see cref="LinearDualCoordinateDescent"/>. </para>
    ///   
    /// <para>
    ///   Liblinear's solver <c>-s 2</c>: <c>L2R_L2LOSS_SVC</c>. A trust region newton
    ///   algorithm for the primal of L2-regularized, L2-loss linear support vector 
    ///   classification.
    /// </para>
    /// </remarks>
    /// 
    /// <examples>
    /// <para>
    ///   The following example shows how to obtain a <see cref="MultipleLinearRegression"/> 
    ///   from a linear <see cref="SupportVectorMachine"/>. It contains exactly the same data 
    ///   used in the <see cref="OrdinaryLeastSquares"/> documentation page for 
    ///   <see cref="MultipleLinearRegression"/>.</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\LinearNewtonMethodTest.cs" region="doc_linreg_sparse"/>
    /// </examples>
    /// 
    /// 
    /// <seealso cref="SequentialMinimalOptimization{TKernel}"/>
    /// <seealso cref="LinearDualCoordinateDescent"/>
    /// 
    public class LinearNewtonMethod<TKernel, TInput> :
        BaseLinearNewtonMethod<SupportVectorMachine<TKernel, TInput>, TKernel, TInput>
        where TKernel : struct, IKernel<TInput>, ILinear<TInput>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="LinearNewtonMethod{TKernel, TInput}"/> class.
        /// </summary>
        /// 
        public LinearNewtonMethod()
        {

        }

        /// <summary>
        ///   Creates an instance of the model to be learned. Inheritors
        ///   of this abstract class must define this method so new models
        ///   can be created from the training data.
        /// </summary>
        /// 
        protected override SupportVectorMachine<TKernel, TInput> Create(int inputs, TKernel kernel)
        {
            return new SupportVectorMachine<TKernel, TInput>(inputs, kernel);
        }
    }

    /// <summary>
    ///   Base class for L2-regularized L2-loss linear support vector classification (primal).
    /// </summary>
    /// 
    public abstract class BaseLinearNewtonMethod<TModel, TKernel> :
        BaseLinearNewtonMethod<TModel, TKernel, double[]>
        where TKernel : struct, ILinear<double[]>
        where TModel : SupportVectorMachine<TKernel, double[]>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="BaseLinearNewtonMethod{TModel, TKernel}"/> class.
        /// </summary>
        /// 
        protected BaseLinearNewtonMethod()
        {
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        [Obsolete]
        protected BaseLinearNewtonMethod(ISupportVectorMachine<double[]> model, double[][] input, int[] output)
            : base(model, input, output)
        {
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        protected BaseLinearNewtonMethod(TModel model, double[][] input, int[] output)
            : base(model, input, output)
        {
        }
    }

    /// <summary>
    ///   L2-regularized L2-loss linear support vector classification (primal).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class implements a L2-regularized L2-loss support vector machine
    ///   learning algorithm that operates in the primal form of the optimization
    ///   problem. This method has been based on liblinear's <c>l2r_l2_svc_fun</c>
    ///   problem specification, optimized using a <see cref="TrustRegionNewtonMethod">
    ///   Trust-region Newton method</see>. This method might be faster than the often
    ///   preferred <see cref="LinearDualCoordinateDescent"/>. </para>
    ///   
    /// <para>
    ///   Liblinear's solver <c>-s 2</c>: <c>L2R_L2LOSS_SVC</c>. A trust region newton
    ///   algorithm for the primal of L2-regularized, L2-loss linear support vector 
    ///   classification.
    /// </para>
    /// </remarks>
    /// 
    /// <seealso cref="SequentialMinimalOptimization{TKernel}"/>
    /// <seealso cref="LinearDualCoordinateDescent"/>
    /// 
    public abstract class BaseLinearNewtonMethod<TModel, TKernel, TInput> :
        BaseSupportVectorClassification<TModel, TKernel, TInput>
        where TKernel : struct, ILinear<TInput>
        where TModel : SupportVectorMachine<TKernel, TInput>
    {
        TrustRegionNewtonMethod tron;

        int parameters;

        double[] z;
        int[] I;
        int sizeI;

        double[] g;
        double[] h;
        double[] wa;

        int biasIndex;

        double tolerance = 0.1;
        int maxIterations = 1000;


        /// <summary>
        ///   Constructs a new Newton method algorithm for L2-regularized
        ///   Support Vector Classification problems in the primal form (-s 2).
        /// </summary>
        /// 
        public BaseLinearNewtonMethod()
        {

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
            get { return this.tolerance; }
            set { this.tolerance = value; }
        }

        /// <summary>
        ///   Gets or sets the maximum number of iterations that should
        ///    be performed until the algorithm stops. Default is 1000.
        /// </summary>
        /// 
        public int MaximumIterations
        {
            get { return this.maxIterations; }
            set { this.maxIterations = value; }
        }



        private double objective(double[] w)
        {
            int[] y = Outputs;

            LinearNewtonMethod.Xv(Kernel, Inputs, biasIndex, w, z);

            double f = 0;
            for (int i = 0; i < w.Length; i++)
                f += w[i] * w[i];
            f /= 2.0;

            for (int i = 0; i < y.Length; i++)
            {
                z[i] = y[i] * z[i];
                double d = 1 - z[i];
                if (d > 0)
                    f += C[i] * d * d;
            }

            return f;
        }

        private double[] gradient(double[] w)
        {
            int[] y = Outputs;

            sizeI = 0;
            for (int i = 0; i < y.Length; i++)
            {
                if (z[i] < 1)
                {
                    z[sizeI] = C[i] * y[i] * (z[i] - 1);
                    I[sizeI] = i;
                    sizeI++;
                }
            }

            LinearNewtonMethod.subXTv(Kernel, Inputs, biasIndex, I, sizeI, z, g);

            for (int i = 0; i < w.Length; i++)
                g[i] = w[i] + 2 * g[i];

            return g;
        }

        private double[] hessian(double[] s)
        {
            LinearNewtonMethod.subXv(Kernel, Inputs, biasIndex, I, sizeI, s, wa);
            for (int i = 0; i < sizeI; i++)
                wa[i] = C[I[i]] * wa[i];

            LinearNewtonMethod.subXTv(Kernel, Inputs, biasIndex, I, sizeI, wa, h);
            for (int i = 0; i < s.Length; i++)
                h[i] = s[i] + 2 * h[i];

            return h;
        }


        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        protected override void InnerRun()
        {
            int samples = Inputs.Length;
            this.parameters = Kernel.GetLength(Inputs) + 1;

            this.z = new double[samples];
            this.I = new int[samples];
            this.wa = new double[samples];

            this.g = new double[parameters];
            this.h = new double[parameters];
            this.biasIndex = parameters - 1;

            tron = new TrustRegionNewtonMethod(parameters)
            {
                Function = objective,
                Gradient = gradient,
                Hessian = hessian,
                MaxIterations = maxIterations,
                Tolerance = tolerance,
                Token = Token
            };

            for (int i = 0; i < tron.Solution.Length; i++)
                tron.Solution[i] = 0;

            tron.Minimize();

            // Get the solution found by TRON
            double[] weightsWithBias = tron.Solution;

            // Separate the weights and the bias coefficient
            double[] weights = weightsWithBias.Get(0, -1);
            double bias = weightsWithBias[biasIndex];

            Debug.Assert(weights.Length == parameters - 1);

            // Create the machine
            Model.NumberOfInputs = weights.Length;
            Model.SupportVectors = new[] { Kernel.CreateVector(weights) };
            Model.Weights = new[] { 1.0 };
            Model.Threshold = bias;
        }


        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        [Obsolete]
        protected BaseLinearNewtonMethod(ISupportVectorMachine<TInput> model, TInput[] input, int[] output)
            : base(model, input, output)
        {
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        protected BaseLinearNewtonMethod(TModel model, TInput[] input, int[] output)
            : base(model, input, output)
        {
        }
    }
}
