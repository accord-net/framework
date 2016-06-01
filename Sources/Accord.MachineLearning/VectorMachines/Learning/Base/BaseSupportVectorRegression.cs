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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Accord.Statistics.Kernels;
    using System.Threading;
    using System.Diagnostics;
    using Accord.Math.Optimization.Losses;
    using System.Collections;

    /// <summary>
    ///   Base class for <see cref="SupportVectorMachine"/> regression learning algorithms.
    /// </summary>
    /// 
    public abstract class BaseSupportVectorRegression<TModel, TKernel, TInput> :
        ISupervisedLearning<TModel, TInput, double>
        where TKernel : IKernel<TInput>
        where TModel : SupportVectorMachine<TKernel, TInput>, ISupportVectorMachine<TInput>
        where TInput : ICloneable
    {

        /// <summary>
        /// Gets or sets a cancellation token that can be used to
        /// stop the learning algorithm while it is running.
        /// </summary>
        public CancellationToken Token { get; set; }

        // Training data
        private TInput[] inputs;
        private double[] outputs;

        private double[] sampleWeights;

        private bool useComplexityHeuristic;

        private double complexity = 1;
        private double rho = 1e-3;

        // Support Vector Machine parameters
        private ISupportVectorMachine<TInput> machine;
        private TKernel kernel;

        private bool isLinear;

        /// <summary>
        ///   Gets or sets the cost values associated with each input vector.
        /// </summary>
        /// 
        protected double[] C { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSupportVectorRegression{TModel, TKernel, TInput}"/> class.
        /// </summary>
        public BaseSupportVectorRegression()
        {

        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        protected BaseSupportVectorRegression(ISupportVectorMachine<TInput> model, TInput[] input, double[] output)
        {
            this.machine = model;
            this.inputs = input;
            this.outputs = output;
            this.kernel = (TKernel)model.Kernel;
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
        public TKernel Kernel
        {
            get { return kernel; }
            set { kernel = value; }
        }

        /// <summary>
        ///   Gets or sets the input vectors for training.
        /// </summary>
        /// 
        public TInput[] Inputs { get { return inputs; } }

        /// <summary>
        ///   Gets or sets the output values for each calibration vector.
        /// </summary>
        public double[] Outputs { get { return outputs; } }

        /// <summary>
        ///   Gets the machine to be taught.
        /// </summary>
        /// 
        public TModel Model
        {
            get { return (TModel)machine; }
            set { machine = value; }
        }

        /// <summary>
        ///   Creates an instance of the model to be learned. Inheritors
        ///   of this abstract class must define this method so new models
        ///   can be created from the training data.
        /// </summary>
        /// 
        protected abstract TModel Create(int inputs, TKernel kernel);

        //protected virtual TModel Create(int inputs, TKernel kernel)
        //{
        //    return SupportVectorLearningHelper.Create<TModel, TInput, TKernel>(inputs, kernel);
        //}

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair.</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        public TModel Learn(TInput[] x, double[] y, double[] weights)
        {
            // Initial argument checking
            SupportVectorLearningHelper.CheckArgs(machine, inputs, outputs);

            if (machine == null)
            {
                int numberOfInputs = SupportVectorLearningHelper.GetNumberOfInputs(x);
                this.machine = Create(numberOfInputs, Kernel);
            }

            // Learning data
            this.inputs = x;
            this.outputs = y;

            // Initialization heuristics
            if (useComplexityHeuristic)
                complexity = kernel.EstimateComplexity(inputs);

            C = new double[inputs.Length];
            for (int i = 0; i < outputs.Length; i++)
                C[i] = complexity;

            if (sampleWeights != null)
            {
                for (int i = 0; i < C.Length; i++)
                    C[i] *= sampleWeights[i];
            }

            InnerRun();

            // Compute error if required.
            return (TModel)machine;
        }



        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        protected abstract void InnerRun();


        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        [Obsolete("Please use Accord.Math.Optimization.SquareLoss or any other losses of your choice from the Accord.Math.Optimization namespace.")]
        public double ComputeError(TInput[] inputs, double[] expectedOutputs)
        {
            return new SquareLoss(expectedOutputs).Loss(Model.Distance(inputs));
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        [Obsolete("Please use Learn() instead.")]
        public double Run()
        {
            Learn(Inputs, Outputs, null);
            return new SquareLoss(Outputs).Loss(Model.Distance(Inputs));
        }
    }
}
