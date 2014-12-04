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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Accord.Statistics.Kernels;
    using System.Threading;
    using System.Diagnostics;

    /// <summary>
    ///   Base class for <see cref="SupportVectorMachine"/> regression learning algorithms.
    /// </summary>
    /// 
    public abstract class BaseSupportVectorRegression
    {
        // Training data
        private double[][] inputs;
        private double[] outputs;

        private double[] sampleWeights;

        private bool useComplexityHeuristic;

        private double complexity = 1;
        private double rho = 1e-3;

        // Support Vector Machine parameters
        private SupportVectorMachine machine;
        private IKernel kernel;

        private bool isLinear;


        /// <summary>
        ///   Initializes a new instance of the <see cref="BaseSupportVectorRegression"/> class.
        /// </summary>
        /// 
        /// <param name="machine">The machine to be learned.</param>
        /// <param name="inputs">The input data.</param>
        /// <param name="outputs">The corresponding output data.</param>
        /// 
        protected BaseSupportVectorRegression(SupportVectorMachine machine, double[][] inputs, double[] outputs)
        {
            // Initial argument checking
            SupportVectorLearningHelper.CheckArgs(machine, inputs, outputs);

            // Machine
            this.machine = machine;

            // Kernel (if applicable)
            KernelSupportVectorMachine ksvm = machine as KernelSupportVectorMachine;

            if (ksvm == null)
            {
                isLinear = true;
                Linear linear = new Linear(0);
                kernel = linear;
            }
            else
            {
                Linear linear = ksvm.Kernel as Linear;
                isLinear = linear != null && linear.Constant == 0;
                kernel = ksvm.Kernel;
            }

            // Learning data
            this.inputs = inputs;
            this.outputs = outputs;
        }

        /// <summary>
        ///   Complexity (cost) parameter C. Increasing the value of C forces the creation
        ///   of a more accurate model that may not generalize well. Default value is 1.
        /// </summary>
        /// 
        /// <remarks>
        ///   The cost parameter C controls the trade off between allowing training
        ///   errors and forcing rigid margins. It creates a soft margin that permits
        ///   some misclassifications. Increasing the value of C increases the cost of
        ///   misclassifying points and forces the creation of a more accurate model
        ///   that may not generalize well.
        /// </remarks>
        /// 
        public double Complexity
        {
            get { return this.complexity; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                this.complexity = value;
            }
        }

        /// <summary>
        ///   Insensitivity zone ε. Increasing the value of ε can result in fewer 
        ///   support vectors in the created model. Default value is 1e-3.
        /// </summary>
        /// 
        /// <remarks>
        ///   Parameter ε controls the width of the ε-insensitive zone, used to fit the training
        ///   data. The value of ε can affect the number of support vectors used to construct the
        ///   regression function. The bigger ε, the fewer support vectors are selected. On the
        ///   other hand, bigger ε-values results in more flat estimates.
        /// </remarks>
        /// 
        public double Epsilon
        {
            get { return this.rho; }
            set { this.rho = value; }
        }

        /// <summary>
        ///   Gets or sets the individual weight of each sample in the training set. If set
        ///   to <c>null</c>, all samples will be assumed equal weight. Default is <c>null</c>.
        /// </summary>
        /// 
        public double[] Weights
        {
            get { return sampleWeights; }
            set { sampleWeights = value; }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether the Complexity parameter C
        ///   should be computed automatically by employing an heuristic rule.
        ///   Default is false.
        /// </summary>
        /// 
        /// <value>
        /// 	<c>true</c> if complexity should be computed automatically; otherwise, <c>false</c>.
        /// </value>
        /// 
        public bool UseComplexityHeuristic
        {
            get { return useComplexityHeuristic; }
            set { useComplexityHeuristic = value; }
        }

        /// <summary>
        ///   Gets whether the machine to be learned
        ///   has a <see cref="Linear"/> kernel.
        /// </summary>
        /// 
        protected bool IsLinear
        {
            get { return isLinear; }
            set { isLinear = value; }
        }

        /// <summary>
        ///   Gets the machine's <see cref="IKernel"/> function.
        /// </summary>
        /// 
        protected IKernel Kernel
        {
            get { return kernel; }
        }

        /// <summary>
        ///   Gets the training input data set.
        /// </summary>
        /// 
        public double[][] Inputs { get { return inputs; } }

        /// <summary>
        ///   Gets the training output labels set.
        /// </summary>
        /// 
        public double[] Outputs { get { return outputs; } }

        /// <summary>
        ///   Gets the machine to be taught.
        /// </summary>
        /// 
        public SupportVectorMachine Machine { get { return machine; } }

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
            return Run(true, CancellationToken.None);
        }

        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        /// <param name="computeError">
        ///   True to compute error after the training
        ///   process completes, false otherwise. Default is true.
        /// </param>
        /// 
        /// <returns>
        ///   The misclassification error rate of
        ///   the resulting support vector machine.
        /// </returns>
        /// 
        public double Run(bool computeError)
        {
            return Run(computeError, CancellationToken.None);
        }

        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        /// <param name="computeError">
        ///   True to compute error after the training
        ///   process completes, false otherwise. Default is true.
        /// </param>
        /// <param name="token">
        ///   A <see cref="CancellationToken"/> which can be used
        ///   to request the cancellation of the learning algorithm
        ///   when it is being run in another thread.
        /// </param>
        /// 
        /// <returns>
        ///   The misclassification error rate of
        ///   the resulting support vector machine.
        /// </returns>
        /// 
        public double Run(bool computeError, CancellationToken token)
        {
            // Initialization heuristics
            if (useComplexityHeuristic)
                complexity = BaseSupportVectorLearning.EstimateComplexity(kernel, inputs);

            double[] c = new double[inputs.Length];
            for (int i = 0; i < outputs.Length; i++)
                c[i] = complexity;

            if (sampleWeights != null)
            {
                for (int i = 0; i < c.Length; i++)
                    c[i] *= sampleWeights[i];
            }

            Run(token, c);

            // Compute error if required.
            return (computeError) ? ComputeError(inputs, outputs) : 0.0;
        }

        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        /// <param name="token">A token to stop processing when requested.</param>
        /// <param name="c">The complexity for each sample.</param>
        /// 
        protected abstract void Run(CancellationToken token, double[] c);


        /// <summary>
        ///   Computes the summed square error for a given set of input and outputs.
        /// </summary>
        /// 
        public double ComputeError(double[][] inputs, double[] expectedOutputs)
        {
            // Compute errors
            double error = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                double output;
                double actual = machine.Compute(inputs[i], out output);
                double expected = expectedOutputs[i];

                if (Double.IsNaN(actual))
                    Trace.WriteLine("SVM has produced NaNs");

                double e = (actual - expected);
                error += e * e;
            }

            // error sum of squares
            return error;
        }

    }
}
