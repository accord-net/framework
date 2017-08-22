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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Accord.Statistics.Kernels;
    using System.Threading;
    using System.Diagnostics;
    using Accord.Math;
    using Accord.Math.Optimization.Losses;
    using Accord.Statistics;
    using System.Collections;
    using Accord.Compat;

    /// <summary>
    ///   Base class for <see cref="SupportVectorMachine"/> learning algorithms.
    /// </summary>
    /// 
    public abstract class BaseSupportVectorClassification<TModel, TKernel, TInput> :
        BinaryLearningBase<TModel, TInput>,
        ISupportVectorMachineLearning<TInput>
        where TKernel : IKernel<TInput>
        where TModel : ISupportVectorMachine<TInput>
    {

        private bool useKernelEstimation = false;
        private bool useComplexityHeuristic = true;
        private bool useClassLabelProportion;
        private bool hasKernelBeenSet;

        private double complexity = 1;
        private double positiveWeight = 1;
        private double negativeWeight = 1;

        private double Cpositive;
        private double Cnegative;

        // Support Vector Machine parameters
        private TKernel kernel;

        // TODO: Remove this field after a few releases. It exists
        // only to provide compatibility with previous releases of
        // the framework.
        private ISupportVectorMachine<TInput> machine;




        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSupportVectorClassification{TModel, TKernel, TInput}"/> class.
        /// </summary>
        /// 
        protected BaseSupportVectorClassification()
        {
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        protected BaseSupportVectorClassification(ISupportVectorMachine<TInput> model, TInput[] input, int[] output)
        {
            this.machine = model;
            this.Inputs = input;
            this.Outputs = output;
            this.Kernel = (TKernel)model.Kernel;
        }


        /// <summary>
        ///   Gets or sets the input vectors for training.
        /// </summary>
        /// 
        protected TInput[] Inputs { get; set; }

        /// <summary>
        ///   Gets or sets the output labels for each training vector.
        /// </summary>
        /// 
        protected int[] Outputs { get; set; }



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
        ///   Gets or sets the positive class weight. This should be a
        ///   value higher than 0 indicating how much of the <see cref="Complexity"/>
        ///   parameter C should be applied to instances carrying the positive label.
        /// </summary>
        /// 
        public double PositiveWeight
        {
            get { return this.positiveWeight; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                this.positiveWeight = value;
            }
        }

        /// <summary>
        ///   Gets or sets the negative class weight. This should be a
        ///   value higher than 0 indicating how much of the <see cref="Complexity"/>
        ///   parameter C should be applied to instances carrying the negative label.
        /// </summary>
        /// 
        public double NegativeWeight
        {
            get { return this.negativeWeight; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                this.negativeWeight = value;
            }
        }

        /// <summary>
        ///   Gets or sets the weight ratio between positive and negative class
        ///   weights. This ratio controls how much of the <see cref="Complexity"/>
        ///   parameter C should be applied to the positive class. 
        /// </summary>
        /// 
        /// <remarks>
        ///  <para>
        ///   A weight ratio lesser than one, such as 1/10 (0.1) means 10% of C will
        ///   be applied to the positive class, while 100% of C will be applied to the
        ///   negative class.</para>
        ///  <para>
        ///   A weight ratio greater than one, such as 10/1 (10) means that 100% of C will
        ///   be applied to the positive class, while 10% of C will be applied to the 
        ///   negative class.</para>
        /// </remarks>
        /// 
        public double WeightRatio
        {
            get { return positiveWeight / negativeWeight; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");

                if (value > 1)
                {
                    // There are more positive than negative instances, e.g. 5:2 (2.5)
                    // (read as five positive examples for each two negative examples).
                    positiveWeight = 1;
                    negativeWeight = 1 / value;
                }
                else // value < 1
                {
                    // There are more negative than positive instances, e.g. 2:5 (0.4)
                    // (read as two positive examples for each five negative examples).
                    negativeWeight = 1;
                    positiveWeight = value;
                }
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether the Complexity parameter C
        ///   should be computed automatically by employing an heuristic rule.
        ///   Default is true.
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
        ///   Gets or sets a value indicating whether the weight ratio to be used between
        ///   <see cref="Complexity"/> values for negative and positive instances should
        ///   be computed automatically from the data proportions. Default is false.
        /// </summary>
        /// 
        /// <value>
        /// 	<c>true</c> if the weighting coefficient should be computed 
        /// 	automatically from the data; otherwise, <c>false</c>.
        /// </value>
        /// 
        public bool UseClassProportions
        {
            get { return useClassLabelProportion; }
            set { useClassLabelProportion = value; }
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
        ///   Gets or sets the cost values associated with each input vector.
        /// </summary>
        /// 
        protected double[] C { get; set; }

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
        public override TModel Learn(TInput[] x, bool[] y, double[] weights = null)
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
            },

            onlyBinary: true);


            // Count class prevalence
            int positives, negatives;
            Classes.GetRatio(y, out positives, out negatives);

            // If all examples are positive or negative, terminate
            //   learning early by directly setting the threshold.

            try
            {
                if (positives == 0 || negatives == 0)
                {
                    Model.SupportVectors = new TInput[0];
                    Model.Weights = new double[0];
                    Model.Threshold = (positives == 0) ? -1 : +1;
                    return Model;
                }

                // Initialization heuristics
                if (useComplexityHeuristic)
                    complexity = kernel.EstimateComplexity(x);

                if (useClassLabelProportion)
                    WeightRatio = positives / (double)negatives;

                // Create per sample complexity
                Cpositive = complexity * positiveWeight;
                Cnegative = complexity * negativeWeight;

                Inputs = x;

                C = new double[y.Length];
                for (int i = 0; i < y.Length; i++)
                    C[i] = y[i] ? Cpositive : Cnegative;

                Outputs = new int[y.Length];
                for (int i = 0; i < y.Length; i++)
                    Outputs[i] = y[i] ? 1 : -1;

                if (weights != null)
                {
                    for (int i = 0; i < C.Length; i++)
                        C[i] *= weights[i];
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
        ///   Runs the main body of the learning algorithm.
        /// </summary>
        /// 
        protected abstract void InnerRun();







        /// <summary>
        ///   Computes the error rate for a given set of input and outputs.
        /// </summary>
        /// 
        [Obsolete("Please use Accord.Math.Optimization.ZeroOneLoss or any other losses of your choice from the Accord.Math.Optimization namespace.")]
        public double ComputeError(TInput[] inputs, int[] expectedOutputs)
        {
            var classifier = (IClassifier<TInput, bool>)Model;
            var loss = new ZeroOneLoss(Classes.Decide(expectedOutputs))
            {
                Mean = true
            };

            return loss.Loss(classifier.Decide(inputs));
        }



        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        [Obsolete("Please use Learn() instead.")]
        public double Run()
        {
            Learn(Inputs, Outputs, null);

            var classifier = (IClassifier<TInput, bool>)Model;
            return new ZeroOneLoss(Classes.Decide(Outputs))
            {
                Mean = true,
            }.Loss(classifier.Decide(Inputs));
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        [Obsolete("Please use Learn() instead.")]
        public double Run(bool computeError)
        {
            Learn(Inputs, Outputs, null);
            if (computeError)
            {
                var classifier = (IClassifier<TInput, bool>)Model;
                return new ZeroOneLoss(Classes.Decide(Outputs))
                {
                    Mean = true,
                }.Loss(classifier.Decide(Inputs));
            }

            return 0;
        }


        ISupportVectorMachine<TInput> ISupervisedLearning<ISupportVectorMachine<TInput>, TInput, int>.Learn(TInput[] x, int[] y, double[] weights)
        {
            return Learn(x, y, weights);
        }

        ISupportVectorMachine<TInput> ISupervisedLearning<ISupportVectorMachine<TInput>, TInput, int[]>.Learn(TInput[] x, int[][] y, double[] weights)
        {
            return Learn(x, y, weights);
        }

        ISupportVectorMachine<TInput> ISupervisedLearning<ISupportVectorMachine<TInput>, TInput, bool[]>.Learn(TInput[] x, bool[][] y, double[] weights)
        {
            return Learn(x, y, weights);
        }

        ISupportVectorMachine<TInput> ISupervisedLearning<ISupportVectorMachine<TInput>, TInput, bool>.Learn(TInput[] x, bool[] y, double[] weights)
        {
            return Learn(x, y, weights);
        }

        ISupportVectorMachine<TInput> ISupervisedLearning<ISupportVectorMachine<TInput>, TInput, double>.Learn(TInput[] x, double[] y, double[] weights)
        {
            return Learn(x, y, weights);
        }
    }
}
