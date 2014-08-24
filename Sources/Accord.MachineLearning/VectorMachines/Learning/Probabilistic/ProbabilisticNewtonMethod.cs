// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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
    ///   L2-regularized L2-loss logistic regression (probabilistic 
    ///   support vector machine) learning algorithm in the primal.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class implements a L2-regularized L2-loss logistic regression (probabilistic
    ///   support vector machine) learning algorithm that operates in the primal form of the
    ///   optimization problem. This method has been based on liblinear's <c>l2r_lr_fun</c>
    ///   problem specification, optimized using a <see cref="TrustRegionNewtonMethod">
    ///   Trust-region Newton method</see>.</para>
    /// </remarks>
    /// 
    /// <para>
    ///   Liblinear's solver <c>-s 0</c>: <c>L2R_LR</c>. A trust region newton
    ///   algorithm for the primal of L2-regularized, L2-loss logistic regression.
    /// </para>
    /// 
    /// <seealso cref="SequentialMinimalOptimization"/>
    /// <seealso cref="LinearCoordinateDescent"/>
    /// 
    public class ProbabilisticNewtonMethod : BaseSupportVectorLearning,
        ISupportVectorMachineLearning, ISupportCancellation
    {

        TrustRegionNewtonMethod tron;

        private double[] z;
        private double[] C;
        private double[] D;

        private double[] g;
        private double[] Hs;

        private int biasIndex;


        /// <summary>
        ///   Constructs a new Newton method algorithm for L2-regularized
        ///   logistic regression (probabilistic linear SVMs) primal problems.
        /// </summary>
        /// 
        /// <param name="machine">A support vector machine.</param>
        /// <param name="inputs">The input data points as row vectors.</param>
        /// <param name="outputs">The output label for each input point. Values must be either -1 or +1.</param>
        /// 
        public ProbabilisticNewtonMethod(SupportVectorMachine machine, double[][] inputs, int[] outputs)
            : base(machine, inputs, outputs)
        {
            if (!IsLinear)
                throw new ArgumentException("Only linear machines are supported.", "machine");

            int samples = inputs.Length;
            int parameters = machine.Inputs + 1;

            this.C = new double[samples];
            this.z = new double[samples];
            this.D = new double[samples];
            this.g = new double[parameters];
            this.Hs = new double[parameters];
            this.biasIndex = machine.Inputs;

            tron = new TrustRegionNewtonMethod(parameters);
            this.Tolerance = 0.01;
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
            int[] y = Outputs;

            Xv(w, z);

            double f = 0;
            for (int i = 0; i < w.Length; i++)
                f += w[i] * w[i];
            f /= 2.0;

            for (int i = 0; i < y.Length; i++)
            {
                double yz = y[i] * z[i];

                if (yz >= 0)
                    f += C[i] * Math.Log(1 + Math.Exp(-yz));
                else
                    f += C[i] * (-yz + Math.Log(1 + Math.Exp(yz)));
            }

            return f;
        }


        private double[] gradient(double[] w)
        {
            int[] y = Outputs;

            for (int i = 0; i < y.Length; i++)
            {
                z[i] = 1 / (1 + Math.Exp(-y[i] * z[i]));
                D[i] = z[i] * (1 - z[i]);
                z[i] = C[i] * (z[i] - 1) * y[i];
            }

            XTv(z, g);

            for (int i = 0; i < w.Length; i++)
                g[i] = w[i] + g[i];

            return g;
        }


        private double[] hessian(double[] s)
        {
            double[] wa = new double[Inputs.Length];

            Xv(s, wa);
            for (int i = 0; i < wa.Length; i++)
                wa[i] = C[i] * D[i] * wa[i];

            XTv(wa, Hs);
            for (int i = 0; i < Hs.Length; i++)
                Hs[i] = s[i] + Hs[i];

            return Hs;
        }

        private void Xv(double[] v, double[] Xv)
        {
            double[][] x = Inputs;

            for (int i = 0; i < x.Length; i++)
            {
                double[] s = x[i];

                Debug.Assert(s.Length == v.Length - 1);

                double sum = v[biasIndex];
                for (int j = 0; j < s.Length; j++)
                    sum += v[j] * s[j];
                Xv[i] = sum;
            }
        }

        private void XTv(double[] v, double[] XTv)
        {
            double[][] x = Inputs;

            Debug.Assert(XTv.Length == g.Length);
            for (int i = 0; i < XTv.Length; i++)
                XTv[i] = 0;

            for (int i = 0; i < x.Length; i++)
            {
                double[] s = x[i];

                Debug.Assert(XTv.Length == s.Length + 1);

                for (int j = 0; j < s.Length; j++)
                    XTv[j] += v[i] * s[j];

                XTv[biasIndex] += v[i];
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
            Machine.Link = new LogLinkFunction();
        }
    }
}
