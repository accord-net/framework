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
    ///   Base class for <see cref="SupportVectorMachine"/> learning algorithms.
    /// </summary>
    /// 
    public abstract class BaseSupportVectorLearning
    {
        // Training data
        private double[][] inputs;
        private int[] outputs;

        private double[] sampleWeights;

        private bool useComplexityHeuristic;
        private bool useClassLabelProportion;

        private double complexity = 1;
        private double positiveWeight = 1;
        private double negativeWeight = 1;

        private double Cpositive;
        private double Cnegative;

        // Support Vector Machine parameters
        private SupportVectorMachine machine;
        private IKernel kernel;

        private bool isLinear;


        /// <summary>
        ///   Initializes a new instance of the <see cref="BaseSupportVectorLearning"/> class.
        /// </summary>
        /// 
        /// <param name="machine">The machine to be learned.</param>
        /// <param name="inputs">The input data.</param>
        /// <param name="outputs">The corresponding output data.</param>
        /// 
        protected BaseSupportVectorLearning(SupportVectorMachine machine, double[][] inputs, int[] outputs)
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
        public int[] Outputs { get { return outputs; } }

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
            // Count class prevalence
            int positives = 0;
            for (int i = 0; i < outputs.Length; i++)
            {
                if (outputs[i] > 0)
                    positives++;
            }

            int negatives = outputs.Length - positives;


            // If all examples are positive or negative, terminate
            //   learning early by directly setting the threshold.

            if (positives == 0 || negatives == 0)
            {
                machine.SupportVectors = new double[0][];
                machine.Weights = new double[0];
                machine.Threshold = (positives == 0) ? -1 : +1;
                return computeError ? ComputeError(inputs, outputs) : 0.0;
            }

            // Initialization heuristics
            if (useComplexityHeuristic)
                complexity = EstimateComplexity(kernel, inputs);

            if (useClassLabelProportion)
                WeightRatio = positives / (double)negatives;

            // Create per sample complexity
            Cpositive = complexity * positiveWeight;
            Cnegative = complexity * negativeWeight;

            double[] c = new double[inputs.Length];
            for (int i = 0; i < outputs.Length; i++)
                c[i] = (outputs[i] > 0) ? Cpositive : Cnegative;

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
        ///   Computes the error rate for a given set of input and outputs.
        /// </summary>
        /// 
        public double ComputeError(double[][] inputs, int[] expectedOutputs)
        {
            // Compute errors
            int count = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                double output;
                double actual = machine.Compute(inputs[i], out output);
                double expected = expectedOutputs[i];

                if (Double.IsNaN(actual))
                    Trace.WriteLine("SVM has produced NaNs");

                bool a = actual >= 0;
                bool b = expected >= 0;

                if (a != b)
                    count++;
            }

            // Return misclassification error ratio
            return count / (double)inputs.Length;
        }


        /// <summary>
        ///   Estimates the <see cref="Complexity">complexity parameter C</see>
        ///   for a given kernel and a given data set.
        /// </summary>
        /// 
        /// <param name="kernel">The kernel function.</param>
        /// <param name="inputs">The input samples.</param>
        /// 
        /// <returns>A suitable value for C.</returns>
        /// 
        public static double EstimateComplexity(IKernel kernel, double[][] inputs)
        {
            // Compute initial value for C as the number of examples
            // divided by the trace of the input sample kernel matrix.

            double sum = 0.0;
            for (int i = 0; i < inputs.Length; i++)
            {
                sum += kernel.Function(inputs[i], inputs[i]);

                if (Double.IsNaN(sum))
                    throw new OverflowException();
            }

            return inputs.Length / sum;
        }

        /// <summary>
        ///   Estimates the <see cref="Complexity">complexity parameter C</see> a given data set.
        /// </summary>
        /// 
        /// <param name="inputs">The input samples.</param>
        /// 
        /// <returns>A suitable value for C.</returns>
        /// 
        public static double EstimateComplexity(double[][] inputs)
        {
            // Compute initial value for C as the number of examples
            // divided by the trace of the input sample kernel matrix.

            double sum = 0.0;
            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = 0; j < inputs[i].Length; j++)
                    sum += inputs[i][j] * inputs[i][j];

                if (Double.IsNaN(sum))
                    throw new OverflowException();
            }
            return inputs.Length / sum;
        }

        /// <summary>
        ///   Estimates the <see cref="Complexity">complexity parameter C</see>
        ///   for a given kernel and an unbalanced data set.
        /// </summary>
        /// 
        /// <param name="kernel">The kernel function.</param>
        /// <param name="inputs">The input samples.</param>
        /// <param name="outputs">The output samples.</param>
        /// 
        /// <returns>A suitable value for positive C and negative C, respectively.</returns>
        /// 
        public static Tuple<double, double> EstimateComplexity(IKernel kernel, double[][] inputs, int[] outputs)
        {
            // Compute initial value for C as the number of examples
            // divided by the trace of the input sample kernel matrix.

            double negativeSum = 0.0;
            double positiveSum = 0.0;

            int negativeCount = 0;
            int positiveCount = 0;

            for (int i = 0; i < inputs.Length; i++)
            {
                if (outputs[i] == -1)
                {
                    negativeSum += kernel.Function(inputs[i], inputs[i]);
                    negativeCount++;
                }
                else // outputs[i] == +1
                {
                    positiveSum += kernel.Function(inputs[i], inputs[i]);
                    positiveCount++;
                }

                if (Double.IsNaN(positiveSum) || Double.IsNaN(negativeSum))
                    throw new OverflowException();
            }

            return Tuple.Create
            (
                positiveCount / positiveSum,
                negativeCount / negativeSum
            );
        }

    }
}
