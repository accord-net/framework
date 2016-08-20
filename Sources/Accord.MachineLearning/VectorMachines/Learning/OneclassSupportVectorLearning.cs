// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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

    /// <summary>
    ///   One-class Support Vector Machine Learning Algorithm.
    /// </summary>
    /// 
#pragma warning disable 0618
    [Obsolete("Please use OneclassSupportVectorLearning<TKernel> instead.")]
    public class OneclassSupportVectorLearning
        : OneclassSupportVectorLearning<KernelSupportVectorMachine, IKernel<double[]>, double[]>
    {
        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please do not pass parameters in the constructor. Use the default constructor and the Learn method instead.")]
        public OneclassSupportVectorLearning(KernelSupportVectorMachine model, double[][] input)
            : base(model, input)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneclassSupportVectorLearning"/> class.
        /// </summary>
        public OneclassSupportVectorLearning()
        {

        }
    }
#pragma warning restore 0618



    /// <summary>
    ///   One-class Support Vector Machine Learning Algorithm.
    /// </summary>
    /// 
    public class OneclassSupportVectorLearning<TKernel, TInput>
        : OneclassSupportVectorLearning<SupportVectorMachine<TKernel, TInput>, TKernel, TInput>
        where TKernel : IKernel<TInput>
        where TInput : ICloneable
    {
    }

    /// <summary>
    ///   One-class Support Vector Machine Learning Algorithm.
    /// </summary>
    /// 
    public class OneclassSupportVectorLearning<TModel, TKernel, TInput>
        : IUnsupervisedLearning<TModel, TInput, bool>
        where TKernel : IKernel<TInput>
        where TModel : SupportVectorMachine<TKernel, TInput>
        where TInput : ICloneable
    {
        private double[] alpha;
        private TInput[] inputs;
        private double nu = 0.5;

        TModel machine;

        double eps = 0.001;
        bool shrinking = true;


        /// <summary>
        ///   Constructs a new one-class support vector learning algorithm.
        /// </summary>
        /// 
        /// <param name="machine">A support vector machine.</param>
        /// 
        public OneclassSupportVectorLearning(TModel machine)
        {
            this.machine = machine;
        }

        /// <summary>
        ///   Constructs a new one-class support vector learning algorithm.
        /// </summary>
        /// 
        public OneclassSupportVectorLearning()
        {

        }

        /// <summary>
        ///   Gets the value for the Lagrange multipliers
        ///   (alpha) for every observation vector.
        /// </summary>
        /// 
        public double[] Lagrange { get { return alpha; } }

        /// <summary>
        ///   Convergence tolerance. Default value is 1e-2.
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
        ///   Controls the number of outliers accepted by the algorithm. This
        ///   value provides an upper bound on the fraction of training errors
        ///   and a lower bound of the fraction of support vectors. Default is 0.5
        /// </summary>
        /// 
        /// <remarks>
        ///   The summary description is given in Chang and Lin,
        ///   "LIBSVM: A Library for Support Vector Machines", 2013.
        /// </remarks>
        /// 
        public double Nu
        {
            get { return nu; }
            set { nu = value; }
        }


        /// <summary>
        /// Learns a model that can map the given inputs to the desired outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="weights">The weight of importance for each input sample.</param>
        /// <returns>
        /// A model that has learned how to produce suitable outputs
        /// given the input data <paramref name="x" />.
        /// </returns>
        public TModel Learn(TInput[] x, double[] weights = null)
        {
            this.inputs = x;
            double[] zeros = new double[inputs.Length];
            int[] ones = Vector.Ones<int>(inputs.Length);
            this.alpha = Vector.Ones<double>(inputs.Length);

            var kernel = machine.Kernel;

            int l = inputs.Length;
            int n = (int)(nu * l);	// # of alpha's at upper bound

            for (int i = 0; i < n; i++)
                alpha[i] = 1;

            if (n < inputs.Length)
                alpha[n] = nu * l - n;

            for (int i = n + 1; i < l; i++)
                alpha[i] = 0;

            Func<int, int, double> Q = (int i, int j) => kernel.Function(x[i], x[j]);

            var s = new FanChenLinQuadraticOptimization(alpha.Length, Q, zeros, ones)
            {
                Tolerance = eps,
                Shrinking = true,
                Solution = alpha
            };

            bool success = s.Minimize();

            int sv = 0;
            for (int i = 0; i < alpha.Length; i++)
                if (alpha[i] > 0) sv++;

            machine.SupportVectors = new TInput[sv];
            machine.Weights = new double[sv];

            for (int i = 0, j = 0; i < alpha.Length; i++)
            {
                if (alpha[i] > 0)
                {
                    machine.SupportVectors[j] = inputs[i];
                    machine.Weights[j] = alpha[i];
                    j++;
                }
            }

            machine.Threshold = s.Rho;

            if (success == false)
            {
                throw new ConvergenceException("Convergence could not be attained. " +
                            "Please reduce the cost of misclassification errors by reducing " +
                            "the complexity parameter C or try a different kernel function.");
            }

            return machine;
        }



        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        protected OneclassSupportVectorLearning(TModel model, TInput[] input)
        {
            this.machine = model;
            this.inputs = input;
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete()]
        public double Run()
        {
            Learn(inputs);
            return new LogLikelihoodLoss().Loss(machine.Score(inputs));
        }
    }
}
