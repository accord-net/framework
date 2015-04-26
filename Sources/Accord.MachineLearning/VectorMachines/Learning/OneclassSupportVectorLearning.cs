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

namespace Accord.MachineLearning.VectorMachines.Learning
{
    using Accord.Math.Optimization;
    using Accord.Statistics.Kernels;
    using System;

    /// <summary>
    ///   One-class Support Vector Machine Learning Algorithm.
    /// </summary>
    /// 
    public class OneclassSupportVectorLearning : ISupportVectorMachineLearning
    {
        SupportVectorMachine machine;

        private double[][] inputs;
        private double[] alpha;

        private double nu = 0.5;

        IKernel kernel;
        readonly double[] zeros;
        readonly int[] ones;

        double eps = 0.001;
        bool shrinking = true;


        /// <summary>
        ///   Constructs a new one-class support vector learning algorithm.
        /// </summary>
        /// 
        /// <param name="machine">A support vector machine.</param>
        /// <param name="inputs">The input data points as row vectors.</param>
        /// 
        public OneclassSupportVectorLearning(SupportVectorMachine machine, double[][] inputs)
        {
            // Initial argument checking
            if (machine == null)
                throw new ArgumentNullException("machine");

            if (inputs == null)
                throw new ArgumentNullException("inputs");

            this.inputs = inputs;
            this.machine = machine;

            this.zeros = new double[inputs.Length];
            this.ones = new int[inputs.Length];
            this.alpha = new double[inputs.Length];

            for (int i = 0; i < alpha.Length; i++)
                alpha[i] = 1;

            for (int i = 0; i < ones.Length; i++)
                ones[i] = 1;


            // Kernel (if applicable)
            var ksvm = machine as KernelSupportVectorMachine;

            if (ksvm == null)
            {
                kernel = new Linear(0);
            }
            else
            {
                kernel = ksvm.Kernel;
            }
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
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        /// <param name="computeError">True to compute error after the training
        ///   process completes, false otherwise.</param>
        /// <returns>
        ///   The misclassification error rate of the resulting support
        ///   vector machine if <paramref name="computeError" /> is true,
        ///   returns zero otherwise.
        /// </returns>
        /// 
        public double Run(bool computeError)
        {
            int l = inputs.Length;
            int n = (int)(nu * l);	// # of alpha's at upper bound

            for (int i = 0; i < n; i++)
                alpha[i] = 1;

            if (n < inputs.Length)
                alpha[n] = nu * l - n;

            for (int i = n + 1; i < l; i++)
                alpha[i] = 0;


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

            machine.SupportVectors = new double[sv][];
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

            if (computeError)
                return ComputeError(inputs);
            return 0.0;
        }

        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        /// <returns>
        ///   The misclassification error rate of
        ///   the resulting support vector machine.
        /// </returns>
        /// 
        public double Run()
        {
            return Run(true);
        }

        /// <summary>
        ///   Computes the error rate for a given set of inputs.
        /// </summary>
        /// 
        public double ComputeError(double[][] inputs)
        {
            double error = 0;
            for (int i = 0; i < inputs.Length; i++)
                error += machine.Compute(inputs[i]);

            return error;
        }


        double Q(int i, int j)
        {
            return kernel.Function(inputs[i], inputs[j]);
        }

    }
}
