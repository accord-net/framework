// Accord Statistics Library
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
    using System.Threading;
    using Accord.Math.Optimization;
    using Accord.Statistics.Links;
    using System.Diagnostics;

    /// <summary>
    ///   L2-regularized L2-loss linear support vector regression
    ///   (SVR) learning algorithm in the primal formulation (-s 11).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class implements a L2-regularized L2-loss support vector regression (SVR)
    ///   learning algorithm that operates in the primal form of the optimization problem.
    ///   This method has been based on liblinear's <c>l2r_l2_svr_fun</c> problem specification,
    ///   optimized using a <see cref="TrustRegionNewtonMethod"> Trust-region Newton method</see>.</para>
    /// </remarks>
    /// 
    /// <para>
    ///   Liblinear's solver <c>-s 11</c>: <c>L2R_L2LOSS_SVR</c>. A trust region newton algorithm
    ///   for the primal of L2-regularized, L2-loss linear epsilon-vector regression (epsilon-SVR).
    /// </para>
    /// 
    /// <seealso cref="SequentialMinimalOptimization"/>
    /// <seealso cref="LinearDualCoordinateDescent"/>
    /// <seealso cref="LinearRegressionCoordinateDescent"/>
    /// 
    public class LinearRegressionNewtonMethod : BaseSupportVectorRegression,
        ISupportVectorMachineLearning, ISupportCancellation
    {

        TrustRegionNewtonMethod tron;

        double[] z;
        int[] I;
        int sizeI;

        double[] g;
        double[] h;
        double[] wa;

        int biasIndex;

        private double[] C;


        /// <summary>
        ///   Constructs a new Newton method algorithm for L2-regularized
        ///   support vector regression (SVR-SVMs) primal problems.
        /// </summary>
        /// 
        /// <param name="machine">A support vector machine.</param>
        /// <param name="inputs">The input data points as row vectors.</param>
        /// <param name="outputs">The output value for each input point.</param>
        /// 
        public LinearRegressionNewtonMethod(SupportVectorMachine machine, double[][] inputs, double[] outputs)
            : base(machine, inputs, outputs)
        {
            if (!IsLinear)
                throw new ArgumentException("Only linear machines are supported.", "machine");

            int samples = inputs.Length;
            int parameters = machine.Inputs + 1;

            this.z = new double[samples];
            this.I = new int[samples];
            this.wa = new double[samples];

            this.g = new double[parameters];
            this.h = new double[parameters];
            this.biasIndex = machine.Inputs;

            tron = new TrustRegionNewtonMethod(parameters);
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
            get { return tron.Tolerance; }
            set { tron.Tolerance = value; }
        }

        /// <summary>
        ///   Gets or sets the maximum number of iterations that should
        ///    be performed until the algorithm stops. Default is 1000.
        /// </summary>
        /// 
        public int MaximumIterations
        {
            get { return tron.MaxIterations; }
            set { tron.MaxIterations = value; }
        }



        private double objective(double[] w)
        {
            double[] y = Outputs;
            double p = Epsilon;

            LinearNewtonMethod.Xv(Inputs, biasIndex, w, z);

            double f = 0;
            for (int i = 0; i < w.Length; i++)
                f += w[i] * w[i];
            f /= 2;

            for (int i = 0; i < y.Length; i++)
            {
                double d = z[i] - y[i];

                if (d < -p)
                    f += C[i] * (d + p) * (d + p);

                else if (d > p)
                    f += C[i] * (d - p) * (d - p);
            }

            return f;
        }


        private double[] gradient(double[] w)
        {
            double[] y = Outputs;
            double p = Epsilon;

            sizeI = 0;
            for (int i = 0; i < y.Length; i++)
            {
                double d = z[i] - y[i];

                // generate index set I
                if (d < -p)
                {
                    z[sizeI] = C[i] * (d + p);
                    I[sizeI] = i;
                    sizeI++;
                }
                else if (d > p)
                {
                    z[sizeI] = C[i] * (d - p);
                    I[sizeI] = i;
                    sizeI++;
                }
            }

            LinearNewtonMethod.subXTv(Inputs, biasIndex, I, sizeI, z, g);

            for (int i = 0; i < w.Length; i++)
                g[i] = w[i] + 2 * g[i];

            return g;
        }


        private double[] hessian(double[] s)
        {
            LinearNewtonMethod.subXv(Inputs, biasIndex, I, sizeI, s, wa);

            for (int i = 0; i < sizeI; i++)
                wa[i] = C[I[i]] * wa[i];

            LinearNewtonMethod.subXTv(Inputs, biasIndex, I, sizeI, wa, h);
            for (int i = 0; i < s.Length; i++)
                h[i] = s[i] + 2 * h[i];

            return h;
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
            this.C = c;

            tron.Function = objective;
            tron.Gradient = gradient;
            tron.Hessian = hessian;

            for (int i = 0; i < tron.Solution.Length; i++)
                tron.Solution[i] = 0;

            tron.Minimize();

            double[] weights = tron.Solution;

            Machine.Weights = new double[Machine.Inputs];
            for (int i = 0; i < Machine.Weights.Length; i++)
                Machine.Weights[i] = weights[i];
            Machine.Threshold = weights[biasIndex];
        }
    }
}
