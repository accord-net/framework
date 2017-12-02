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
    using System;
    using Accord.Statistics.Kernels;
    using Accord.Math.Optimization.Losses;
    using Accord.Compat;
    using System.Threading;
    using System.Diagnostics;

    /// <summary>
    ///   Base class for <see cref="SupportVectorMachine"/> regression learning algorithms.
    /// </summary>
    /// 
    public abstract class BaseSupportVectorRegression<TModel, TKernel, TInput> :
        ISupervisedLearning<TModel, TInput, double>
        where TKernel : IKernel<TInput>
        where TModel : SupportVectorMachine<TKernel, TInput>, ISupportVectorMachine<TInput>
#if !NETSTANDARD1_4
        where TInput : ICloneable
#endif
    {
        [NonSerialized]
        CancellationToken token = new CancellationToken();

        /// <summary>
        /// Gets or sets a cancellation token that can be used to
        /// stop the learning algorithm while it is running.
        /// </summary>
        public CancellationToken Token
        {
            get { return token; }
            set { token = value; }
        }

        // Training data
        private TInput[] inputs;
        private double[] outputs;

        private double[] sampleWeights;

        private bool hasKernelBeenSet = false;
        private bool useKernelEstimation = false;
        private bool useComplexityHeuristic = true;

        private double complexity = 1;
        private double rho = 1e-3;

        // Support Vector Machine parameters
        private TKernel kernel;
        private bool isLinear;

        // TODO: Remove this field after a few releases. It exists
        // only to provide compatibility with previous releases of
        // the framework.
        private ISupportVectorMachine<TInput> machine;


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
            this.Kernel = (TKernel)model.Kernel;
        }

        /// <summary>
        ///   Complexity (cost) parameter C. Increasing the value of C forces the creation
        ///   of a more accurate model that may not generalize well. If this value is not
        ///   set and <see cref="UseComplexityHeuristic"/> is set to <c>true</c>, the framework
        ///   will automatically guess a value for <c>C</c>. If this value is manually set to 
        ///   something else, then <see cref="UseComplexityHeuristic"/> will be automatically 
        ///   disabled and the given value will be used instead.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        ///   The cost parameter C controls the trade off between allowing training
        ///   errors and forcing rigid margins. It creates a soft margin that permits
        ///   some misclassifications. Increasing the value of C increases the cost of
        ///   misclassifying points and forces the creation of a more accurate model
        ///   that may not generalize well.</para>
        ///   
        /// <para>
        ///   If this value is not set and <see cref="UseComplexityHeuristic"/> is set to 
        ///   <c>true</c>, the framework will automatically guess a suitable value for C by
        ///   calling <see cref="Accord.Statistics.Kernels.Kernel.EstimateComplexity{TKernel, TInput}(TKernel, TInput[])"/>.  If this value 
        ///   is manually set to something else, then the class will respect the new value 
        ///   and automatically disable <see cref="UseComplexityHeuristic"/>. </para>
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
                this.useComplexityHeuristic = false;
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
        ///   Gets or sets whether initial values for some kernel parameters
        ///   should be estimated from the data, if possible. Default is true.
        /// </summary>
        /// 
        public bool UseKernelEstimation
        {
            get { return useKernelEstimation; }
            set { useKernelEstimation = value; }
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
        ///   Gets or sets the kernel function use to create a 
        ///   kernel Support Vector Machine. If this property
        ///   is set, <see cref="UseKernelEstimation"/> will be
        ///   set to false.
        /// </summary>
        /// 
        public TKernel Kernel
        {
            get { return kernel; }
            set
            {
                this.kernel = value;
                this.useKernelEstimation = false;
                this.hasKernelBeenSet = true;
            }
        }

        /// <summary>
        ///   Gets or sets the input vectors for training.
        /// </summary>
        /// 
        protected TInput[] Inputs { get { return inputs; } }

        /// <summary>
        ///   Gets or sets the output values for each calibration vector.
        /// </summary>
        protected double[] Outputs { get { return outputs; } }

        /// <summary>
        ///   Gets the machine to be taught.
        /// </summary>
        /// 
        public TModel Model { get; set; }

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
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        public TModel Learn(TInput[] x, double[] y, double[] weights = null)
        {
            Accord.MachineLearning.Tools.CheckArgs(x, y, weights, () =>
            {
                bool initialized = false;

                if (kernel == null)
                {
                    kernel = SupportVectorLearningHelper.CreateKernel<TKernel, TInput>(x);
                    initialized = true;
                }

                if (!initialized)
                {
                    if (useKernelEstimation)
                    {
                        kernel = SupportVectorLearningHelper.EstimateKernel(kernel, x);
                    }
                    else
                    {
                        if (!hasKernelBeenSet)
                        {
                            Trace.TraceWarning("The Kernel property has not been set and the UseKernelEstimation property is set to false. Please" +
                                " make sure that the default parameters of the kernel are suitable for your application, otherwise the learning" +
                                " will result in a model with very poor performance.");
                        }
                    }
                }

                if (Model == null)
                    Model = Create(SupportVectorLearningHelper.GetNumberOfInputs(kernel, x), kernel);

                Model.Kernel = kernel;
                return Model;
            });

            // Learning data
            this.inputs = x;
            this.outputs = y;

            try
            {
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

                SupportVectorLearningHelper.CheckOutput(Model);

                return Model;
            }
            finally
            {
                if (machine != null)
                {
                    // TODO: This block is only necessary to offer compatibility
                    // to code written using previous versions of the framework,
                    // and should be removed after a few releases.
                    machine.SupportVectors = Model.SupportVectors;
                    machine.Weights = Model.Weights;
                    machine.Threshold = Model.Threshold;
                    machine.Kernel = Model.Kernel;
                    machine.IsProbabilistic = Model.IsProbabilistic;
                }
            }
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
            return new SquareLoss(expectedOutputs).Loss(machine.Score(inputs));
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        [Obsolete("Please use Learn() instead.")]
        public double Run()
        {
            Learn(Inputs, Outputs);
            return new SquareLoss(Outputs).Loss(machine.Score(Inputs));
        }
    }
}
