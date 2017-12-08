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

namespace Accord.MachineLearning.VectorMachines.Learning
{
    using Accord.Math.Optimization;
    using Accord.Statistics.Kernels;
    using Accord.Math;
    using System;
    using Accord.Math.Optimization.Losses;
    using System.Threading;
    using Accord.Compat;

    /// <summary>
    ///   Support vector regression using <see cref="FanChenLinQuadraticOptimization"/> (LibSVM) algorithm.
    /// </summary>
    /// 
    public class FanChenLinSupportVectorRegression
        : BaseFanChenLinSupportVectorRegression<SupportVectorMachine, Linear, double[]>
    {
        /// <summary>
        /// Creates an instance of the model to be learned. Inheritors
        /// of this abstract class must define this method so new models
        /// can be created from the training data.
        /// </summary>
        protected override SupportVectorMachine Create(int inputs, Linear kernel)
        {
            return new SupportVectorMachine(inputs);
        }
    }

    /// <summary>
    ///   Support vector regression using <see cref="FanChenLinQuadraticOptimization"/> (LibSVM)  algorithm.
    /// </summary>
    /// 
    public class FanChenLinSupportVectorRegression<TKernel>
        : BaseFanChenLinSupportVectorRegression<SupportVectorMachine<TKernel>, TKernel, double[]>
        where TKernel : IKernel<double[]>
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

    /// <summary>
    ///   Support vector regression using <see cref="FanChenLinQuadraticOptimization"/>  (LibSVM) algorithm.
    /// </summary>
    /// 
    public class FanChenLinSupportVectorRegression<TKernel, TInput>
        : BaseFanChenLinSupportVectorRegression<SupportVectorMachine<TKernel, TInput>, TKernel, TInput>
        where TKernel : IKernel<TInput>
        where TInput : ICloneable
    {
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
    ///   Base class for Fan-Chen-Lin (LibSVM) regression algorithms.
    /// </summary>
    /// 
    public abstract class BaseFanChenLinSupportVectorRegression<TModel, TKernel, TInput>
        : BaseSupportVectorRegression<TModel, TKernel, TInput>
        where TKernel : IKernel<TInput>
        where TModel : SupportVectorMachine<TKernel, TInput>
#if !NETSTANDARD1_4
        where TInput : ICloneable
#endif
    {

        private double[] alpha;

        double eps = 1e-3;
        bool shrinking = true;
        double rho = 1e-3;

        /// <summary>
        ///   Constructs a new one-class support vector learning algorithm.
        /// </summary>
        /// 
        /// <param name="machine">A support vector machine.</param>
        /// 
        public BaseFanChenLinSupportVectorRegression(TModel machine)
        {
            this.Model = machine;
        }

        /// <summary>
        ///   Constructs a new one-class support vector learning algorithm.
        /// </summary>
        /// 
        public BaseFanChenLinSupportVectorRegression()
        {

        }

        /// <summary>
        ///   Gets the value for the Lagrange multipliers
        ///   (alpha) for every observation vector.
        /// </summary>
        /// 
        public double[] Lagrange { get { return alpha; } }

        /// <summary>
        ///   Convergence tolerance. Default value is 1e-3.
        /// </summary>
        /// 
        /// <remarks>
        ///   The criterion for completing the model training process. The default is 0.001.
        /// </remarks>
        /// 
        public double Tolerance
        {
            get { return eps; }
            set { eps = value; }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to use 
        ///   shrinking heuristics during learning. Default is true.
        /// </summary>
        /// 
        /// <value>
        ///   <c>true</c> to use shrinking; otherwise, <c>false</c>.
        /// </value>
        /// 
        public bool Shrinking
        {
            get { return shrinking; }
            set { shrinking = value; }
        }

        /// <summary>
        /// Runs the learning algorithm.
        /// </summary>
        /// 
        protected override void InnerRun()
        {
            TInput[] x = Inputs;
            double[] y = Outputs;

            int l = Inputs.Length;
            double[] alpha2 = new double[2 * l];
            double[] linear_term = new double[2 * l];
            int[] signs = new int[2 * l];
            double[] c = new double[2 * l];
            int[] sign = new int[2 * l];
            int[] index = new int[2 * l];

            for (int i = 0; i < l; i++)
            {
                linear_term[i] = rho - y[i];
                signs[i] = +1;
                c[i] = C[i];
                index[i] = i;

                linear_term[i + l] = rho + y[i];
                signs[i + l] = -1;
                c[i + l] = C[i];
                index[i + l] = i;
            }

            Func<int, int[], int, double[], double[]> Q = (int i, int[] indices, int length, double[] row) =>
            {
                for (int j = 0; j < length; j++)
                {
                    int k = indices[j];
                    row[j] = signs[i] * signs[k] * Kernel.Function(x[index[i]], x[index[k]]);
                }
                return row;
            };

            var s = new FanChenLinQuadraticOptimization(2 * l, Q, linear_term, signs)
            {
                Tolerance = eps,
                Shrinking = this.shrinking,
                Solution = alpha2,
                Token = Token,
                UpperBounds = c,
            };

            bool success = s.Minimize();

            this.alpha = new double[Inputs.Length];
            for (int i = 0; i < alpha.Length; i++)
                alpha[i] = alpha2[i] - alpha2[i + l];

            int sv = 0;
            for (int i = 0; i < alpha.Length; i++)
                if (alpha[i] != 0)
                    sv++;

            Model.SupportVectors = new TInput[sv];
            Model.Weights = new double[sv];

            for (int i = 0, j = 0; i < alpha.Length; i++)
            {
                if (alpha[i] != 0)
                {
                    Model.SupportVectors[j] = Inputs[i];
                    Model.Weights[j] = alpha[i];
                    j++;
                }
            }

            Model.Threshold = -s.Rho;

            if (success == false)
            {
                throw new ConvergenceException("Convergence could not be attained. " +
                            "Please reduce the cost of misclassification errors by reducing " +
                            "the complexity parameter C or try a different kernel function.");
            }
        }

    }
}
