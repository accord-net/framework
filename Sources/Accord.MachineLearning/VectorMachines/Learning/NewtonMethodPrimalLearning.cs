// Copyright (c) 2007-2011 The LIBLINEAR Project.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions
// are met:
//
//   1. Redistributions of source code must retain the above copyright
//   notice, this list of conditions and the following disclaimer.
//
//   2. Redistributions in binary form must reproduce the above copyright
//   notice, this list of conditions and the following disclaimer in the
//   documentation and/or other materials provided with the distribution.
//
//   3. Neither name of copyright holders nor the names of its contributors
//   may be used to endorse or promote products derived from this software
//   without specific prior written permission.
//
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE REGENTS OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//

namespace Accord.MachineLearning.VectorMachines.Learning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Accord.Math.Optimization;

    /// <summary>
    ///   L2-regularized L2-loss support vector classification (primal)
    /// </summary>
    /// 
    /// l2r_l2_svc_fun
    /// 
    public class NewtonMethodPrimalLearning : ISupportVectorMachineLearning
    {

        SupportVectorMachine machine;
        TrustRegionNewtonMethod tron;

        double[][] inputs;
        int[] outputs;

        double[] C;
        double[] z;
        double[] D;
        int[] I;
        int sizeI;

        double[] g;
        double[] h;

        // Learning algorithm parameters
        private double c = 1.0;
        private bool useComplexityHeuristic;
        private bool useClassLabelProportion;

        private double positiveWeight = 1;
        private double negativeWeight = 1;

        int positives;
        int negatives;

        int bias;


        public NewtonMethodPrimalLearning(SupportVectorMachine machine, double[][] inputs, int[] outputs)
        {
            // Initial argument checking
            SupportVectorLearningHelper.CheckArgs(machine, inputs, outputs);

            this.machine = machine;

            this.inputs = inputs;
            this.outputs = outputs;

            int l = inputs.Length;
            this.z = new double[l];
            this.D = new double[l];
            this.I = new int[l];
            this.C = new double[l];

            int parameters = machine.Inputs + 1;
            this.g = new double[parameters];
            this.h = new double[parameters];
            this.bias = machine.Inputs;

            tron = new TrustRegionNewtonMethod(parameters);

            this.countClasses(outputs);
        }

        /// <summary>
        ///   Complexity (cost) parameter C. Increasing the value of C forces the creation
        ///   of a more accurate model that may not generalize well. Default value is the
        ///   number of examples divided by the trace of the kernel matrix.
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
            get { return this.c; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                this.c = value;
            }
        }

        /// <summary>
        ///   Gets or sets the positive class weight. This should be a
        ///   value between 0 and 1 indicating how much of the <see cref="Complexity"/>
        ///   parameter C should be applied to instances carrying the positive label.
        /// </summary>
        /// 
        public double PositiveWeight
        {
            get { return this.positiveWeight; }
            set
            {
                if (value <= 0 || value > 1)
                    throw new ArgumentOutOfRangeException("value");
                this.positiveWeight = value;
            }
        }

        /// <summary>
        ///   Gets or sets the negative class weight. This should be a
        ///   value between 0 and 1 indicating how much of the <see cref="Complexity"/>
        ///   parameter C should be applied to instances carrying the negative label.
        /// </summary>
        /// 
        public double NegativeWeight
        {
            get { return this.negativeWeight; }
            set
            {
                if (value <= 0 || value > 1)
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
        ///   Convergence tolerance. Default value is 0.1.
        /// </summary>
        /// 
        /// <remarks>
        ///   The criterion for completing the model training process. The default is 0.1.
        /// </remarks>
        /// 
        public double Tolerance
        {
            get { return this.tron.Tolerance; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                this.tron.Tolerance = value;
            }
        }

        private double objective(double[] w)
        {
            int[] y = outputs;

            Xv(w, z);

            double f = 0;
            for (int i = 0; i < w.Length; i++)
                f += w[i] * w[i];
            f /= 2.0;

            for (int i = 0; i < y.Length; i++)
            {
                z[i] = y[i] * z[i];
                double d = 1 - z[i];
                if (d > 0)
                    f += C[i] * d * d;
            }

            return f;
        }

        private double[] gradient(double[] w)
        {
            int[] y = outputs;

            sizeI = 0;
            for (int i = 0; i < y.Length; i++)
            {
                if (z[i] < 1)
                {
                    z[sizeI] = C[i] * y[i] * (z[i] - 1);
                    I[sizeI] = i;
                    sizeI++;
                }
            }

            subXTv(z, g);

            for (int i = 0; i < w.Length; i++)
                g[i] = w[i] + 2 * g[i];

            return g;
        }

        private double[] hessian(double[] s)
        {
            double[] wa = new double[sizeI];

            subXv(s, wa);
            for (int i = 0; i < sizeI; i++)
                wa[i] = C[I[i]] * wa[i];

            subXTv(wa, h);
            for (int i = 0; i < s.Length; i++)
                h[i] = s[i] + 2 * h[i];

            return h;
        }

        private void Xv(double[] v, double[] Xv)
        {
            double[][] x = inputs;

            for (int i = 0; i < x.Length; i++)
            {
                double[] s = x[i];

                double sum = v[bias];
                for (int j = 0; j < s.Length; j++)
                    sum += v[j] * s[j];
                Xv[i] = sum;
            }
        }

        private void subXv(double[] v, double[] Xv)
        {
            double[][] x = inputs;

            for (int i = 0; i < sizeI; i++)
            {
                double[] s = x[I[i]];

                double sum = v[bias];
                for (int j = 0; j < s.Length; j++)
                    sum += v[j] * s[j];
                Xv[i] = sum;
            }
        }

        private void subXTv(double[] v, double[] XTv)
        {
            double[][] x = inputs;

            for (int i = 0; i < XTv.Length; i++)
                XTv[i] = 0;

            for (int i = 0; i < sizeI; i++)
            {
                double[] s = x[I[i]];
                
                for (int j = 0; j < s.Length; j++)
                    XTv[j] += v[i] * s[j];

                XTv[bias] += v[i];
            }
        }


        public double Run(bool computeError)
        {
            // Initialization heuristics
            if (useComplexityHeuristic)
                c = CoordinateDescentLinearLearning.EstimateComplexity(inputs);

            // If all examples are positive or negative, terminate
            //   learning early by directly setting the threshold.

            if (positives == 0)
            {
                machine.Weights = new double[machine.Inputs];
                machine.Threshold = -1;
                return 0;
            }

            if (negatives == 0)
            {
                machine.Weights = new double[machine.Inputs];
                machine.Threshold = +1;
                return 0;
            }

            if (useClassLabelProportion)
                WeightRatio = positives / (double)negatives;

            double Cp = c * positiveWeight;
            double Cn = c * negativeWeight;

            for (int i = 0; i < outputs.Length; i++)
            {
                if (outputs[i] > 0)
                    C[i] = Cp;
                else
                    C[i] = Cn;
            }

            tron.Function = objective;
            tron.Gradient = gradient;
            tron.Hessian = hessian;

            tron.Minimize();

            double[] weights = tron.Solution;

            machine.Weights = new double[machine.Inputs];
            for (int i = 0; i < machine.Weights.Length; i++)
                machine.Weights[i] = weights[i];
            machine.Threshold = weights[machine.Inputs];


            // Compute error if required.
            return (computeError) ? ComputeError(inputs, outputs) : 0.0;
        }

        /// <summary>
        ///   Runs the SMO algorithm.
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
        ///   Computes the error rate for a given set of input and outputs.
        /// </summary>
        /// 
        public double ComputeError(double[][] inputs, int[] expectedOutputs)
        {
            // Compute errors
            int count = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                bool actual = machine.Compute(inputs[i]) >= 0;
                bool expected = expectedOutputs[i] >= 0;

                if (actual != expected) count++;
            }

            // Return misclassification error ratio
            return count / (double)inputs.Length;
        }

        private void countClasses(int[] outputs)
        {
            positives = 0;
            for (int i = 0; i < outputs.Length; i++)
            {
                if (outputs[i] > 0)
                    positives++;
            }

            negatives = outputs.Length - positives;
        }
    }
}
