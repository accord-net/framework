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
    using Accord.Math.Optimization.Losses;
    using System;
    using Accord.Compat;
    using System.Threading;
    using System.Diagnostics;

    /// <summary>
    ///   One-class Support Vector Machine learning algorithm.
    /// </summary>
    /// 
    /// <example>
    ///   <para>The following example shows how to use an one-class SVM.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\OneclassSupportVectorLearningTest.cs" region="doc_learn" />
    /// </example>
    /// 
    /// <seealso cref="SupportVectorMachine"/>
    /// <seealso cref="ProbabilisticOutputCalibration"/>
    /// <seealso cref="MulticlassSupportVectorLearning{TKernel}"/>
    /// 
#pragma warning disable 0618
    [Obsolete("Please use OneclassSupportVectorLearning<TKernel> instead.")]
    public class OneclassSupportVectorLearning
        : BaseOneclassSupportVectorLearning<ISupportVectorMachine<double[]>, IKernel<double[]>, double[]>
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

        /// <summary>
        /// Creates an instance of the model to be learned. Inheritors
        /// of this abstract class must define this method so new models
        /// can be created from the training data.
        /// </summary>
        protected override ISupportVectorMachine<double[]> Create(int inputs, IKernel<double[]> kernel)
        {
            if (kernel is Linear)
                return new SupportVectorMachine(inputs) { Kernel = (Linear)kernel };
            return new SupportVectorMachine<IKernel<double[]>>(inputs, kernel);
        }
    }
#pragma warning restore 0618

    /// <summary>
    ///   One-class Support Vector Machine learning algorithm.
    /// </summary>
    /// 
    /// <example>
    ///   <para>The following example shows how to use an one-class SVM.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\OneclassSupportVectorLearningTest.cs" region="doc_learn" />
    /// </example>
    /// 
    /// <seealso cref="SupportVectorMachine"/>
    /// <seealso cref="ProbabilisticOutputCalibration"/>
    /// <seealso cref="MulticlassSupportVectorLearning{TKernel}"/>
    /// 
    public class OneclassSupportVectorLearning<TKernel>
        : BaseOneclassSupportVectorLearning<SupportVectorMachine<TKernel>, TKernel, double[]>
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
    ///   One-class Support Vector Machine learning algorithm.
    /// </summary>
    /// 
    /// <example>
    ///   <para>The following example shows how to use an one-class SVM.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\OneclassSupportVectorLearningTest.cs" region="doc_learn" />
    /// </example>
    /// 
    /// <seealso cref="SupportVectorMachine"/>
    /// <seealso cref="ProbabilisticOutputCalibration"/>
    /// <seealso cref="MulticlassSupportVectorLearning{TKernel}"/>
    /// 
    public class OneclassSupportVectorLearning<TKernel, TInput>
        : BaseOneclassSupportVectorLearning<SupportVectorMachine<TKernel, TInput>, TKernel, TInput>
        where TKernel : IKernel<TInput>
#if !NETSTANDARD1_4
        where TInput : ICloneable
#endif
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
    ///   One-class Support Vector Machine Learning Algorithm.
    /// </summary>
    /// 
    public abstract class BaseOneclassSupportVectorLearning<TModel, TKernel, TInput>
        : IUnsupervisedLearning<TModel, TInput, bool>
        where TKernel : IKernel<TInput>
        where TModel : ISupportVectorMachine<TInput>
        // TODO: after a few releases, the TModel constraint should be changed to:
        // where TModel : SupportVectorMachine<TKernel, TInput>, ISupportVectorMachine<TInput>
    {
        [NonSerialized]
        CancellationToken token = new CancellationToken();

        private bool hasKernelBeenSet = false;
        private bool useKernelEstimation = false;

        private double[] alpha;
        private TInput[] inputs;
        private double nu = 0.5;

        TKernel kernel;

        double eps = 0.001;
        bool shrinking = true;

        // TODO: Remove this field after a few releases. It exists
        // only to provide compatibility with previous releases of
        // the framework.
        private ISupportVectorMachine<TInput> machine;



        /// <summary>
        ///   Gets or sets the classifier being learned.
        /// </summary>
        /// 
        public TModel Model { get; set; }

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
        ///   Constructs a new one-class support vector learning algorithm.
        /// </summary>
        /// 
        /// <param name="machine">A support vector machine.</param>
        /// 
        public BaseOneclassSupportVectorLearning(TModel machine)
        {
            this.Model = machine;
        }

        /// <summary>
        ///   Constructs a new one-class support vector learning algorithm.
        /// </summary>
        /// 
        public BaseOneclassSupportVectorLearning()
        {

        }

        /// <summary>
        ///   Gets the value for the Lagrange multipliers
        ///   (alpha) for every observation vector.
        /// </summary>
        /// 
        public double[] Lagrange { get { return alpha; } }

        /// <summary>
        ///   Gets or sets a cancellation token that can be used to
        ///   stop the learning algorithm while it is running.
        /// </summary>
        /// 
        public CancellationToken Token
        {
            get { return token; }
            set { token = value; }
        }

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
        ///   Creates an instance of the model to be learned. Inheritors
        ///   of this abstract class must define this method so new models
        ///   can be created from the training data.
        /// </summary>
        /// 
        protected abstract TModel Create(int inputs, TKernel kernel);


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
            Accord.MachineLearning.Tools.CheckArgs(x, weights, () =>
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

            try
            {
                this.inputs = x;
                double[] zeros = new double[inputs.Length];
                int[] ones = Vector.Ones<int>(inputs.Length);
                this.alpha = Vector.Ones<double>(inputs.Length);

                int l = inputs.Length;
                int n = (int)(nu * l);  // # of alpha's at upper bound

                for (int i = 0; i < n; i++)
                    alpha[i] = 1;

                if (n < inputs.Length)
                    alpha[n] = nu * l - n;

                for (int i = n + 1; i < l; i++)
                    alpha[i] = 0;

                Func<int, int[], int, double[], double[]> Q = (int i, int[] indices, int length, double[] row) =>
                {
                    for (int j = 0; j < length; j++)
                        row[j] = Kernel.Function(x[i], x[indices[j]]);
                    return row;
                };

                var s = new FanChenLinQuadraticOptimization(alpha.Length, Q, zeros, ones)
                {
                    Tolerance = eps,
                    Shrinking = this.shrinking,
                    Solution = alpha,
                    Token = Token
                };

                bool success = s.Minimize();

                int sv = 0;
                for (int i = 0; i < alpha.Length; i++)
                    if (alpha[i] > 0) sv++;

                Model.SupportVectors = new TInput[sv];
                Model.Weights = new double[sv];

                for (int i = 0, j = 0; i < alpha.Length; i++)
                {
                    if (alpha[i] > 0)
                    {
                        Model.SupportVectors[j] = inputs[i];
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

            return Model;
        }



        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        protected BaseOneclassSupportVectorLearning(ISupportVectorMachine<TInput> model, TInput[] input)
        {
            this.machine = model;
            this.inputs = input;
            this.Kernel = (TKernel)model.Kernel;
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete()]
        public double Run()
        {
            Learn(inputs);
            return new LogLikelihoodLoss().Loss(Model.Score(inputs));
        }
    }
}
