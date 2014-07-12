// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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
//
// This code has been based on the LIBLINEAR project;s implementation for the
// L2-regularized L2-loss support vector classification (dual) machines. The
// original LIBLINEAR license is given below:
//
//
//   Copyright (c) 2007-2011 The LIBLINEAR Project.
//   All rights reserved.
//
//   Redistribution and use in source and binary forms, with or without
//   modification, are permitted provided that the following conditions
//   are met:
//
//      1. Redistributions of source code must retain the above copyright
//      notice, this list of conditions and the following disclaimer.
//
//      2. Redistributions in binary form must reproduce the above copyright
//      notice, this list of conditions and the following disclaimer in the
//      documentation and/or other materials provided with the distribution.
//
//      3. Neither name of copyright holders nor the names of its contributors
//      may be used to endorse or promote products derived from this software
//      without specific prior written permission.
//
//
//   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//   ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//   A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE REGENTS OR
//   CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//   LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//   NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//

namespace Accord.MachineLearning.VectorMachines.Learning
{
    using System;
    using System.Diagnostics;
    using Accord.Math;

    public enum Loss 
    { 
        /// <summary>
        ///   Hinge-loss function. 
        /// </summary>
        /// 
        L1, 
        
        /// <summary>
        ///   Squared hinge-loss function.
        /// </summary>
        /// 
        L2 
    };

    // A coordinate descent algorithm for 
    // L1-loss and L2-loss SVM dual problems
    //
    //  min_\alpha  0.5(\alpha^T (Q + D)\alpha) - e^T \alpha,
    //    s.t.      0 <= \alpha_i <= upper_bound_i,
    // 
    //  where Qij = yi yj xi^T xj and
    //  D is a diagonal matrix 
    //
    // In L1-SVM case:
    // 		upper_bound_i = Cp if y_i = 1
    // 		upper_bound_i = Cn if y_i = -1
    // 		D_ii = 0
    // In L2-SVM case:
    // 		upper_bound_i = INF
    // 		D_ii = 1/(2*Cp)	if y_i = 1
    // 		D_ii = 1/(2*Cn)	if y_i = -1
    //
    // Given: 
    // x, y, Cp, Cn
    // eps is the stopping tolerance
    //
    // solution will be put in w
    // 
    // See Algorithm 3 of Hsieh et al., ICML 2008

    /// <summary>
    ///   L2-regularized, L1 or L2-loss Support Vector Machine learning.
    /// </summary>
    /// 
    /// solve_l2r_l1l2_svc
    /// 
    public class CoordinateDescentLinearLearning : ISupportVectorMachineLearning
    {

        SupportVectorMachine machine;

        double[][] inputs;
        int[] outputs;

        int max_iter = 1000;

        private double c = 1.0;
        private double eps = 0.1;
        private bool useComplexityHeuristic;
        private bool useClassLabelProportion;

        private double positiveWeight = 1;
        private double negativeWeight = 1;

        private double[] alpha;
        private double[] weights;
        private double bias;

        int positives;
        int negatives;

        Loss loss = Loss.L2;


        public CoordinateDescentLinearLearning(SupportVectorMachine machine, double[][] inputs, int[] outputs)
        {
            // Initial argument checking
            SupportVectorLearningHelper.CheckArgs(machine, inputs, outputs);

            // Machine
            this.machine = machine;

            // Learning data
            this.inputs = inputs;
            this.outputs = outputs;

            int samples = inputs.Length;
            int dimension = inputs[0].Length;

            // Lagrange multipliers
            this.alpha = new double[inputs.Length];
            this.weights = new double[dimension];

            this.countClasses(outputs);
        }



        public Loss Loss
        {
            get { return loss; }
            set { loss = value; }
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
        ///   Gets the value for the Lagrange multipliers
        ///   (alpha) for every observation vector.
        /// </summary>
        /// 
        public double[] Lagrange { get { return alpha; } }

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
            get { return this.eps; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                this.eps = value;
            }
        }

     

        public double Run()
        {
            return Run(true);
        }

        public double Run(bool computeError)
        {
            double[] w = weights;
            
            var random = new Random(Accord.Math.Tools.Random.Next());

            // Initialization heuristics
            if (useComplexityHeuristic)
                c = EstimateComplexity(inputs);

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

            // Lagrange multipliers
            Array.Clear(alpha, 0, alpha.Length);
            Array.Clear(w, 0, w.Length);


            int iter = 0;
            double[] QD = new double[inputs.Length];
            int[] index = new int[inputs.Length];

            int[] y = outputs;
            int active_size = inputs.Length;

            // PG: projected gradient, for shrinking and stopping
            double PG;
            double PGmax_old = Double.PositiveInfinity;
            double PGmin_old = Double.NegativeInfinity;
            double PGmax_new, PGmin_new;

            // default solver_type: L2R_L2LOSS_SVC_DUAL
            double[] diag = { 0.5 / Cn, 0, 0.5 / Cp };
            double[] upper_bound = { Double.PositiveInfinity, 0, Double.PositiveInfinity };

            if (Loss == Loss.L1)
            {
                diag[0] = 0;
                diag[2] = 0;
                upper_bound[0] = Cn;
                upper_bound[2] = Cp;
            }

            for (int i = 0; i < w.Length; i++)
                w[i] = 0;
            bias = 0;

            for (int i = 0; i < inputs.Length; i++)
            {
                int gi = y[i] + 1;
                QD[i] = diag[gi];

                double[] xi = inputs[i];

                for (int j = 0; j < xi.Length; j++)
                {
                    double val = xi[j];
                    QD[i] += val * val;
                    w[j] += y[i] * alpha[i] * val;
                }

                QD[i] += 1;
                bias = y[i] * alpha[i];

                index[i] = i;
            }

            while (iter < max_iter)
            {
                PGmax_new = double.NegativeInfinity;
                PGmin_new = double.PositiveInfinity;

                for (int i = 0; i < active_size; i++)
                {
                    int j = i + random.Next(active_size - i);

                    var old = index[i];
                    index[i] = index[j];
                    index[j] = old;
                }

                for (int s = 0; s < active_size; s++)
                {
                    int i = index[s];
                    int yi = y[i];
                    double[] xi = inputs[i];
                    int gi = y[i] + 1;

                    double G = bias;
                    for (int j = 0; j < xi.Length; j++)
                        G += w[j] * xi[j];

                    G = G * yi - 1;

                    double C = upper_bound[gi];
                    G += alpha[i] * diag[gi];

                    PG = 0;
                    if (alpha[i] == 0)
                    {
                        if (G > PGmax_old)
                        {
                            active_size--;

                            var old = index[s];
                            index[s] = index[active_size];
                            index[active_size] = old;

                            s--;
                            continue;
                        }
                        else if (G < 0)
                        {
                            PG = G;
                        }
                    }
                    else if (alpha[i] == C)
                    {
                        if (G < PGmin_old)
                        {
                            active_size--;

                            var old = index[s];
                            index[s] = index[active_size];
                            index[active_size] = old;
                            s--;
                            continue;
                        }
                        else if (G > 0)
                        {
                            PG = G;
                        }
                    }
                    else
                    {
                        PG = G;
                    }

                    PGmax_new = Math.Max(PGmax_new, PG);
                    PGmin_new = Math.Min(PGmin_new, PG);

                    if (Math.Abs(PG) > 1.0e-12)
                    {
                        double alpha_old = alpha[i];

                        alpha[i] = Math.Min(Math.Max(alpha[i] - G / QD[i], 0.0), C);

                        double d = (alpha[i] - alpha_old) * yi;

                        xi = inputs[i];

                        for (int j = 0; j < xi.Length; j++)
                            w[j] += d * xi[j];
                        bias += d;
                    }
                }

                iter++;

                if (iter % 10 == 0)
                    Debug.WriteLine(".");

                if (PGmax_new - PGmin_new <= eps)
                {
                    if (active_size == inputs.Length)
                        break;

                    active_size = inputs.Length;
                    Debug.WriteLine("*");
                    PGmax_old = Double.PositiveInfinity;
                    PGmin_old = Double.NegativeInfinity;
                    continue;
                }

                PGmax_old = PGmax_new;
                PGmin_old = PGmin_new;

                if (PGmax_old <= 0)
                    PGmax_old = Double.PositiveInfinity;

                if (PGmin_old >= 0)
                    PGmin_old = Double.NegativeInfinity;
            }

            Debug.WriteLine("\noptimization finished, #iter = %d\n", iter);

            if (iter >= max_iter)
            {
                Debug.WriteLine("WARNING: reaching max number of iterations");
                Debug.WriteLine("Using -s 2 may be faster (also see FAQ)");
            }


            machine.Weights = new double[machine.Inputs];
            for (int i = 0; i < machine.Weights.Length; i++)
                machine.Weights[i] = w[i];
            machine.Threshold = bias;


            return (computeError) ? ComputeError(inputs, outputs) : 0.0;
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
