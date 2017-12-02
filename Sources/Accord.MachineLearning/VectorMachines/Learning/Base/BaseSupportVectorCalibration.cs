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
    using Accord.Math;
    using Accord.Math.Optimization.Losses;
    using Accord.Statistics;
    using Accord.Compat;
    using System.Threading;

    /// <summary>
    ///   Base class for <see cref="SupportVectorMachine"/> calibration algorithms.
    /// </summary>
    /// 
    public abstract class BaseSupportVectorCalibration<TModel, TKernel, TInput> :
        ISupervisedBinaryLearning<TModel, TInput>
        where TKernel : IKernel<TInput>
        where TModel : SupportVectorMachine<TKernel, TInput>
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

        // Support Vector Machine parameters
        private TModel machine;
        private TKernel kernel;

        /// <summary>
        ///   Gets or sets the input vectors for training.
        /// </summary>
        protected TInput[] Input { get; set; }

        /// <summary>
        ///   Gets or sets the output labels for each training vector.
        /// </summary>
        protected bool[] Output { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSupportVectorCalibration{TModel, TKernel, TInput}"/> class.
        /// </summary>
        /// <param name="machine">The machine to be calibrated.</param>
        protected BaseSupportVectorCalibration(TModel machine)
        {
            this.machine = machine;
            this.kernel = machine.Kernel;
        }

        /// <summary>
        /// Creates an instance of the model to be learned. Inheritors
        /// of this abstract class must define this method so new models
        /// can be created from the training data.
        /// </summary>
        protected virtual TModel Create(int inputs, TKernel kernel)
        {
            TModel result = null;
            var type = typeof(TModel);
            if (type == typeof(SupportVectorMachine))
                result = new SupportVectorMachine(inputs) as TModel;
#if !NETSTANDARD1_4
#pragma warning disable 0618
            else if (type == typeof(KernelSupportVectorMachine))
                result = new KernelSupportVectorMachine(kernel as IKernel, inputs) as TModel;
#pragma warning restore 0618
#endif
            else if (type == typeof(SupportVectorMachine<TKernel, TInput>))
                result = new SupportVectorMachine<TKernel, TInput>(inputs, kernel) as TModel;

            if (result == null)
                throw new NotSupportedException("If you are implementing your own support vector machine type, please override the Create method in your learning algorithm to instruct the framework how to instantiate a type of your new class.");

            return result;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSupportVectorCalibration{TModel, TKernel, TInput}"/> class.
        /// </summary>
        /// <param name="machine">The machine to be calibrated.</param>
        protected BaseSupportVectorCalibration(ISupportVectorMachine<TInput> machine)
        {
            this.machine = (TModel)machine;
            this.kernel = (TKernel)machine.Kernel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSupportVectorCalibration{TModel, TKernel, TInput}"/> class.
        /// </summary>
        public BaseSupportVectorCalibration()
        {

        }

        /// <summary>
        ///   Gets whether the machine being learned is linear.
        /// </summary>
        /// 
        public bool IsLinear
        {
            get { return Kernel is ILinear; }
        }


        /// <summary>
        ///   Gets the machine's <see cref="IKernel"/> function.
        /// </summary>
        /// 
        protected TKernel Kernel
        {
            get { return kernel; }
        }

        /// <summary>
        ///   Gets the machine to be taught.
        /// </summary>
        /// 
        public TModel Model
        {
            get { return machine; }
            set { machine = value; }
        }


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
            return Learn(x, Classes.Decide(y), weights);
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        public TModel Learn(TInput[] x, int[] y, double[] weights = null)
        {
            return Learn(x, Classes.Decide(y), weights);
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        public TModel Learn(TInput[] x, int[][] y, double[] weights = null)
        {
            return Learn(x, Classes.Decide(y.GetColumn(0)), weights);
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        public TModel Learn(TInput[] x, bool[][] y, double[] weights = null)
        {
            return Learn(x, y.GetColumn(0), weights);
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        public TModel Learn(TInput[] x, bool[] y, double[] weights = null)
        {
            Accord.MachineLearning.Tools.CheckArgs(x, y, weights, () =>
            {
                if (Model == null)
                {
                    int numberOfInputs = SupportVectorLearningHelper.GetNumberOfInputs(kernel, x);
                    Model = Create(numberOfInputs, Kernel);
                }

                return Model;
            }, 
            
            onlyBinary: true);

            InnerRun();

            return machine;
        }


        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        protected abstract void InnerRun();


        /// <summary>
        ///   Obsolete.
        /// </summary>
        protected BaseSupportVectorCalibration(ISupportVectorMachine<double[]> model, TInput[] input, int[] output)
        {
            this.Model = model as TModel;
            this.Input = input;
            this.Output = Classes.Decide(output);
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please use Learn() instead.")]
        public virtual double Run()
        {
            Learn(Input, Output, null);
            var classifier = (IClassifier<TInput, bool>)machine;
            return new ZeroOneLoss(Output).Loss(classifier.Decide(Input));
        }

    }
}
