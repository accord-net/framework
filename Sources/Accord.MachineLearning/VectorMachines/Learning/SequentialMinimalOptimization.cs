// Accord Machine Learning Library
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

namespace Accord.MachineLearning.VectorMachines.Learning
{
    using System;
    using System.Collections.Generic;
    using Accord.Math;
    using Accord.Statistics.Kernels;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///   Gets the selection strategy to be used in SMO.
    /// </summary>
    /// 
    public enum SelectionStrategy
    {

        /// <summary>
        ///   Uses the sequential selection strategy as
        ///    suggested by Keerthi et al's algorithm 1.
        /// </summary>
        /// 
        Sequential,

        /// <summary>
        ///   Always select the worst violation pair
        ///   to be optimized first, as suggested in
        ///   Keerthi et al's algorithm 2.
        /// </summary>
        /// 
        WorstPair
    };

    /// <summary>
    ///   Sequential Minimal Optimization (SMO) Algorithm
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The SMO algorithm is an algorithm for solving large quadratic programming (QP)
    ///   optimization problems, widely used for the training of support vector machines.
    ///   First developed by John C. Platt in 1998, SMO breaks up large QP problems into
    ///   a series of smallest possible QP problems, which are then solved analytically.</para>
    /// <para>
    ///   This class follows the original algorithm by Platt with additional modifications
    ///   by Keerthi et al.</para>
    ///  
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       <a href="http://en.wikipedia.org/wiki/Sequential_Minimal_Optimization">
    ///       Wikipedia, The Free Encyclopedia. Sequential Minimal Optimization. Available on:
    ///       http://en.wikipedia.org/wiki/Sequential_Minimal_Optimization </a></description></item>
    ///     <item><description>
    ///       <a href="http://research.microsoft.com/en-us/um/people/jplatt/smoTR.pdf">
    ///       John C. Platt, Sequential Minimal Optimization: A Fast Algorithm for Training Support
    ///       Vector Machines. 1998. Available on: http://research.microsoft.com/en-us/um/people/jplatt/smoTR.pdf </a></description></item>
    ///     <item><description>
    ///       <a href="http://www.cs.iastate.edu/~honavar/keerthi-svm.pdf">
    ///       S. S. Keerthi et al. Improvements to Platt's SMO Algorithm for SVM Classifier Design.
    ///       Technical Report CD-99-14. Available on: http://www.cs.iastate.edu/~honavar/keerthi-svm.pdf </a></description></item>
    ///     <item><description>
    ///       <a href="http://www.idiom.com/~zilla/Work/Notes/svmtutorial.pdf">
    ///       J. P. Lewis. A Short SVM (Support Vector Machine) Tutorial. Available on:
    ///       http://www.idiom.com/~zilla/Work/Notes/svmtutorial.pdf </a></description></item>
    ///     </list></para>  
    /// </remarks>
    /// 
    /// <example>
    ///   <code>
    ///   // Example XOR problem
    ///   double[][] inputs =
    ///   {
    ///       new double[] { 0, 0 }, // 0 xor 0: 1 (label +1)
    ///       new double[] { 0, 1 }, // 0 xor 1: 0 (label -1)
    ///       new double[] { 1, 0 }, // 1 xor 0: 0 (label -1)
    ///       new double[] { 1, 1 }  // 1 xor 1: 1 (label +1)
    ///   };
    ///    
    ///   // Dichotomy SVM outputs should be given as [-1;+1]
    ///   int[] labels =
    ///   {
    ///          1, -1, -1, 1
    ///   };
    ///  
    ///   // Create a Kernel Support Vector Machine for the given inputs
    ///   KernelSupportVectorMachine svm = new KernelSupportVectorMachine(new Gaussian(0.1), inputs[0].Length);
    /// 
    ///   // Instantiate a new learning algorithm for SVMs
    ///   SequentialMinimalOptimization smo = new SequentialMinimalOptimization(svm, inputs, labels);
    /// 
    ///   // Set up the learning algorithm
    ///   smo.Complexity = 1.0;
    /// 
    ///   // Run the learning algorithm
    ///   double error = smo.Run();
    ///   
    ///   // Compute the decision output for one of the input vectors
    ///   int decision = System.Math.Sign(svm.Compute(inputs[0])); // +1
    ///  </code>
    /// </example>
    /// 
    public class SequentialMinimalOptimization : ISupportVectorMachineLearning, ISupportCancellation
    {

        // Training data
        private double[][] inputs;
        private int[] outputs;

        // Learning algorithm parameters
        private double c = 1.0;
        private double tolerance = 1e-2;
        private double epsilon = 1e-6;
        private bool useComplexityHeuristic;
        private bool useClassLabelProportion;

        // Support Vector Machine parameters
        private SupportVectorMachine machine;
        private IKernel kernel;
        private double[] alpha;

        private bool isLinear;
        private bool isCompact;
        private double[] weights;

        private HashSet<int> activeExamples;   // alpha[i] > 0
        private HashSet<int> nonBoundExamples; // alpha[i] > 0 && alpha[i] < c
        private HashSet<int> atBoundsExamples; // alpha[i] = c

        // Keerthi's improvements
        private int i_lower;
        private int i_upper;
        private double b_upper;
        private double b_lower;

        // Error cache to speed up computations
        private double[] errors;

        private int cacheSize;
        private KernelFunctionCache kernelCache;

        private SelectionStrategy strategy = SelectionStrategy.WorstPair;
        private double positiveWeight = 1;
        private double negativeWeight = 1;
        private double positiveCost;
        private double negativeCost;

        private int maxChecks = 100;

        /// <summary>
        ///   Initializes a new instance of a Sequential Minimal Optimization (SMO) algorithm.
        /// </summary>
        /// 
        /// <param name="machine">A Support Vector Machine.</param>
        /// <param name="inputs">The input data points as row vectors.</param>
        /// <param name="outputs">The output label for each input point. Values must be either -1 or +1.</param>
        /// 
        public SequentialMinimalOptimization(SupportVectorMachine machine,
            double[][] inputs, int[] outputs)
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
                Linear linear = new Linear();
                kernel = linear;
            }
            else
            {
                Linear linear = ksvm.Kernel as Linear;
                isLinear = linear != null;
                kernel = ksvm.Kernel;
            }


            // Learning data
            this.inputs = inputs;
            this.outputs = outputs;

            int samples = inputs.Length;
            int dimension = inputs[0].Length;

            // Lagrange multipliers
            this.alpha = new double[inputs.Length];

            if (isLinear) // Hyperplane weights
                this.weights = new double[dimension];

            // Error cache
            this.errors = new double[samples];

            // Kernel cache
            this.cacheSize = samples;

            // Index sets
            activeExamples = new HashSet<int>();
            nonBoundExamples = new HashSet<int>();
            atBoundsExamples = new HashSet<int>();
        }


        //---------------------------------------------


        #region Properties
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
        ///   Epsilon for round-off errors. Default value is 1e-12.
        /// </summary>
        /// 
        public double Epsilon
        {
            get { return epsilon; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                epsilon = value;
            }
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
            get { return this.tolerance; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                this.tolerance = value;
            }
        }

        /// <summary>
        ///   Gets or sets the <see cref="SelectionStrategy">pair selection 
        ///   strategy</see> to be used during optimization.
        /// </summary>
        /// 
        public SelectionStrategy Strategy
        {
            get { return strategy; }
            set { strategy = value; }
        }

        /// <summary>
        ///   Gets or sets the cache size to partially
        ///   stored the kernel matrix. Default is the
        ///   same number of input vectors.
        /// </summary>
        /// 
        public int CacheSize
        {
            get { return cacheSize; }
            set
            {
                if (cacheSize < 0)
                    throw new ArgumentOutOfRangeException("value");
                this.cacheSize = value;
            }
        }

        /// <summary>
        ///   Gets the value for the Lagrange multipliers
        ///   (alpha) for every observation vector.
        /// </summary>
        /// 
        public double[] Lagrange { get { return alpha; } }

        /// <summary>
        ///   Gets or sets whether to produce compact models. Compact
        ///   formulation is currently limited to linear models.
        /// </summary>
        /// 
        public bool Compact
        {
            get { return isCompact; }
            set
            {
                if (!isLinear && value)
                    throw new InvalidOperationException("Only linear machines can be created in compact form.");
                isCompact = value;
            }
        }

        /// <summary>
        ///   Gets the indices of the active examples (examples which have
        ///   the corresponding Lagrange multiplier different than zero).
        /// </summary>
        /// 
        public HashSet<int> ActiveExamples
        {
            get { return activeExamples; }
        }

        /// <summary>
        ///   Gets the indices of the non-bounded examples (examples which
        ///   have the corresponding Lagrange multipliers between 0 and C).
        /// </summary>
        /// 
        public HashSet<int> NonBoundExamples
        {
            get { return nonBoundExamples; }
        }

        /// <summary>
        ///   Gets the indices of the examples at the boundary (examples
        ///   which have the corresponding Lagrange multipliers equal to C).
        /// </summary>
        /// 
        public HashSet<int> BoundedExamples
        {
            get { return atBoundsExamples; }
        }

        #endregion


        //---------------------------------------------

        /// <summary>
        ///   Runs the SMO algorithm.
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
        ///   Runs the SMO algorithm.
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

            // The SMO algorithm chooses to solve the smallest possible optimization problem
            // at every step. At every step, SMO chooses two Lagrange multipliers to jointly
            // optimize, finds the optimal values for these multipliers, and updates the SVM
            // to reflect the new optimal values.
            //
            // Reference: http://research.microsoft.com/en-us/um/people/jplatt/smoTR.pdf

            // The algorithm has been updated to implement the improvements suggested
            // by Keerthi et al. The code has been based on the pseudo-code available
            // on the author's technical report.
            //
            // Reference: http://www.cs.iastate.edu/~honavar/keerthi-svm.pdf


            // Initialize variables
            int samples = inputs.Length;
            int dimension = inputs[0].Length;


            // Initialization heuristics
            if (useComplexityHeuristic)
                c = EstimateComplexity(kernel, inputs);

            int[] positives = outputs.Find(x => x == +1);
            int[] negatives = outputs.Find(x => x == -1);


            // If all examples are positive or negative, terminate
            //   learning early by directly setting the threshold.

            if (positives.Length == 0)
            {
                machine.SupportVectors = new double[0][];
                machine.Weights = new double[0];
                machine.Threshold = -1;
                return 0;
            }
            if (negatives.Length == 0)
            {
                machine.SupportVectors = new double[0][];
                machine.Weights = new double[0];
                machine.Threshold = +1;
                return 0;
            }


            if (useClassLabelProportion)
                WeightRatio = positives.Length / (double)negatives.Length;


            // Lagrange multipliers
            Array.Clear(alpha, 0, alpha.Length);

            if (isLinear) // Hyperplane weights
                Array.Clear(weights, 0, weights.Length);

            // Error cache
            Array.Clear(errors, 0, errors.Length);

            // Kernel evaluations cache
            this.kernelCache = new KernelFunctionCache(kernel, inputs, cacheSize);

            // [Keerthi] Initialize b_up to -1 and 
            //   i_up to any one index of class 1:
            this.b_upper = -1;
            this.i_upper = positives[0];

            // [Keerthi] Initialize b_low to +1 and 
            //   i_low to any one index of class 2:
            this.b_lower = +1;
            this.i_lower = negatives[0];

            // [Keerthi] Set error cache for i_low and i_up:
            this.errors[i_lower] = +1;
            this.errors[i_upper] = -1;


            // Prepare indices sets
            activeExamples.Clear();
            nonBoundExamples.Clear();
            atBoundsExamples.Clear();


            // Balance classes
            bool balanced = positiveWeight == 1 && negativeWeight == 1;
            positiveCost = c * positiveWeight;
            negativeCost = c * negativeWeight;



            // Algorithm:
            int numChanged = 0;
            int wholeSetChecks = 0;
            bool examineAll = true;
            bool diverged = false;
            bool shouldStop = false;

            while ((numChanged > 0 || examineAll) && !shouldStop)
            {
                numChanged = 0;
                if (examineAll)
                {
                    // loop I over all training examples
                    for (int i = 0; i < samples; i++)
                        if (examineExample(i)) numChanged++;

                    wholeSetChecks++;
                }
                else
                {
                    if (strategy == SelectionStrategy.Sequential)
                    {
                        if (balanced) // Assume balanced data
                        {
                            // loop I over examples not at bounds
                            for (int i = 0; i < alpha.Length; i++)
                            {
                                if (alpha[i] != 0 && alpha[i] != c)
                                {
                                    if (examineExample(i)) numChanged++;

                                    if (b_upper > b_lower - 2.0 * tolerance)
                                    {
                                        numChanged = 0; break;
                                    }
                                }
                            }
                        }
                        else // Use different weights for classes
                        {
                            // loop I over examples not at bounds
                            for (int i = 0; i < alpha.Length; i++)
                            {
                                if (alpha[i] != 0)
                                {
                                    if (outputs[i] == +1)
                                    {
                                        if (alpha[i] == positiveCost) continue;
                                    }
                                    else // outputs[i] == -1
                                    {
                                        if (alpha[i] == negativeCost) continue;
                                    }

                                    if (examineExample(i)) numChanged++;

                                    if (b_upper > b_lower - 2.0 * tolerance)
                                    {
                                        numChanged = 0; break;
                                    }
                                }
                            }
                        }
                    }
                    else // strategy == Strategy.WorstPair
                    {
                        int attempts = 0;
                        do
                        {
                            attempts++;

                            if (!takeStep(i_upper, i_lower))
                                break;

                            if (attempts > samples * maxChecks)
                                break;
                        }
                        while ((b_upper <= b_lower - 2.0 * tolerance));

                        numChanged = 0;
                    }
                }

                if (examineAll)
                    examineAll = false;

                else if (numChanged == 0)
                    examineAll = true;

                if (wholeSetChecks > maxChecks)
                    shouldStop = diverged = true;

                if (token.IsCancellationRequested)
                    shouldStop = true;
            }


            // Store information about bounded examples
            if (balanced)
            {
                // Assume equal weights for classes
                for (int i = 0; i < alpha.Length; i++)
                    if (alpha[i] == c) atBoundsExamples.Add(i);
            }
            else
            {
                // Use different weights for classes
                for (int i = 0; i < alpha.Length; i++)
                {
                    if (outputs[i] == +1)
                    {
                        if (alpha[i] == positiveCost)
                            atBoundsExamples.Add(i);
                    }
                    else // outputs[i] == -1
                    {
                        if (alpha[i] == negativeCost)
                            atBoundsExamples.Add(i);
                    }
                }
            }

            if (isCompact)
            {
                // Store the hyperplane directly
                machine.SupportVectors = null;
                machine.Weights = weights;
                machine.Threshold = -(b_lower + b_upper) / 2.0;
            }
            else
            {
                // Store Support Vectors in the SV Machine. Only vectors which have Lagrange multipliers
                // greater than zero will be stored as only those are actually required during evaluation.

                int activeCount = activeExamples.Count;

                int[] idx = new int[activeCount];
                activeExamples.CopyTo(idx);

                machine.SupportVectors = new double[activeCount][];
                machine.Weights = new double[activeCount];
                for (int i = 0; i < idx.Length; i++)
                {
                    int j = idx[i];
                    machine.SupportVectors[i] = inputs[j];
                    machine.Weights[i] = alpha[j] * outputs[j];
                }
                machine.Threshold = -(b_lower + b_upper) / 2;
            }

            // Clear function cache
            this.kernelCache.Clear();
            this.kernelCache = null;

            if (diverged)
            {
                throw new ConvergenceException("Convergence could not be attained. " +
                            "Please reduce the cost of misclassification errors by reducing " +
                            "the complexity parameter C or try a different kernel function.");
            }

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
                bool actual = compute(inputs[i]) >= 0;
                bool expected = expectedOutputs[i] >= 0;

                if (actual != expected) count++;
            }

            // Return misclassification error ratio
            return count / (double)inputs.Length;
        }

        //---------------------------------------------


        /// <summary>
        ///  Chooses which multipliers to optimize using heuristics.
        /// </summary>
        /// 
        private bool examineExample(int i2)
        {
            double[] p2 = inputs[i2]; // Input point at index i2
            double y2 = outputs[i2];  // Classification label for p2
            double alph2 = alpha[i2]; // Lagrange multiplier for p2

            double e2; // SVM output on p2 - y2. Check if it has already been computed

            int set = I(i2);

            if (set == 0)
            {
                // i2 is in I0
                e2 = errors[i2];
            }
            else
            {
                // Compute error and update
                e2 = errors[i2] = computeNoBias(i2) - y2;

                if ((set == 1 || set == 2) && (e2 < b_upper))
                {
                    // i2 is in I1 or I2
                    b_upper = e2; i_upper = i2;
                }
                else if ((set == 3 || set == 4) && (e2 > b_lower))
                {
                    // i2 is in I3 or I4
                    b_lower = e2; i_lower = i2;
                }
            }


            int i1 = -1;

            // [Keerthi] Check optimality using current b_low
            // and b_up and, if violated, find an index i1 to
            // do a joint optimization with i2.

            bool optimality = true;
            if (set == 0 || set == 1 || set == 2)
            {
                // i2 is in I0, I1 or I2
                if (b_lower - e2 > 2 * tolerance)
                {
                    optimality = false;
                    i1 = i_lower;
                }
            }

            if (set == 0 || set == 3 || set == 4)
            {
                // i2 is in I0, I3 or I4
                if (e2 - b_upper > 2 * tolerance)
                {
                    optimality = false;
                    i1 = i_upper;
                }
            }

            if (optimality)
                return false; // no need to take step

            if (set == 0)
            {
                // [Keerthi] for i2 in I0, select the best i1
                i1 = (b_lower - e2 > e2 - b_upper) ? i_lower : i_upper;
            }

            return takeStep(i1, i2);
        }

        private int I(int i)
        {
            double a = alpha[i];
            double y = outputs[i];

            // From Keerthi's technical report, define:
            //
            //   I0 = { 0 < a[i] < c }
            //   I1 = { y[i] = +1, a[i] = 0 }
            //   I2 = { y[i] = -1, a[i] = c }
            //   I3 = { y[i] = +1, a[i] = c }
            //   I4 = { y[i] = -1, a[i] = 0 }
            //

            if (y == +1 && a == 0)
                return 1; // I1
            else if (y == -1 && a == negativeCost)
                return 2; // I2
            else if (y == +1 && a == positiveCost)
                return 3; // I3
            else if (y == -1 && a == 0)
                return 4; // I4

            return 0; // I0
        }

        /// <summary>
        ///   Analytically solves the optimization problem for two Lagrange multipliers.
        /// </summary>
        /// 
        private bool takeStep(int i1, int i2)
        {
            if (i1 == i2) return false;

            double[] p1 = inputs[i1]; // Input point at index i1
            double alph1 = alpha[i1]; // Lagrange multiplier for p1
            double y1 = outputs[i1];  // Classification label for p1
            double c1 = y1 == +1 ? positiveCost : negativeCost;

            // SVM output on p1 - y1 [without bias threshold]. Check if it has already been computed
            double e1 = (alph1 > 0 && alph1 < c1) ? errors[i1] : errors[i1] = computeNoBias(i1) - y1;

            double[] p2 = inputs[i2]; // Input point at index i2
            double alph2 = alpha[i2]; // Lagrange multiplier for p2
            double y2 = outputs[i2];  // Classification label for p2
            double c2 = y2 == +1 ? positiveCost : negativeCost;


            // SVM output on p2 - y2. Should have been computed already.
            double e2 = errors[i2];


            double s = y1 * y2;


            // Compute L and H according to equations (13) and (14) (Platt, 1998)
            double L, H;
            if (y1 != y2)
            {
                // If the target y1 does not equal the target           (13)
                // y2, then the following bounds apply to a2:
                L = Math.Max(0, alph2 - alph1);
                H = Math.Min(c2, c2 + alph2 - alph1);
            }
            else
            {
                // If the target y1 does equal the target               (14)
                // y2, then the following bounds apply to a2:
                L = Math.Max(0, alph2 + alph1 - c2);
                H = Math.Min(c2, alph2 + alph1);
            }

            if (L == H) return false;


            double k11 = kernelCache.GetOrCompute(i1);
            double k22 = kernelCache.GetOrCompute(i2);
            double k12 = kernelCache.GetOrCompute(i1, i2);

            double eta = 2.0 * k12 - k11 - k22;


            double a1, a2;

            if (eta < 0)
            {
                // Under usual circumstances, eta will be negative.
                // Compute as indicated in Platt's SMO book (eq 12.6).

                a2 = alph2 - y2 * (e1 - e2) / eta;

                if (a2 < L) a2 = L;
                else if (a2 > H) a2 = H;
            }
            else
            {
                // Under unusual circumstances, eta could be zero. 
                // In this case, compute the objective function as 
                // suggested in "A Practical SMO Algorithm" by J.
                // Dong, A. Krzyzak and C. Y. Suen.

                //*
                double Lobj = y2 * (e1 - e2) * L;
                double Hobj = y2 * (e1 - e2) * H;

                /*/
                double L1 = alph1 + s * (alph2 - L);
                double H1 = alph1 + s * (alph2 - H);
                double f1 = y1 * e1 - alph1 * k11 - s * alph2 * k12;
                double f2 = y2 * e2 - alph2 * k22 - s * alph1 * k12;
                double Lobj = -0.5 * L1 * L1 * k11 - 0.5 * L * L * k22 - s * L * L1 * k12 - L1 * f1 - L * f2;
                double Hobj = -0.5 * H1 * H1 * k11 - 0.5 * H * H * k22 - s * H * H1 * k12 - H1 * f1 - H * f2;
                //*/

                if (Lobj > Hobj + epsilon) a2 = L;
                else if (Lobj < Hobj - epsilon) a2 = H;
                else a2 = alph2;

                System.Diagnostics.Trace.WriteLine("SMO: eta is zero.");
            }

            if (Math.Abs(a2 - alph2) < epsilon * (a2 + alph2 + epsilon))
                return false; // no step need to be taken

            // Compute update step
            a1 = alph1 + s * (alph2 - a2);

            if (a1 < 0)
            {
                a2 += s * a1;
                a1 = 0;
            }
            else if (a1 > c1)
            {
                a2 += s * (a1 - c2);
                a1 = c1;
            }

            // Approximate precision as
            // suggested in Platt's errata
            double roundoff = 1e-8;
            if (a2 > c2 - c2 * roundoff) a2 = c2;
            else if (a2 < c2 * roundoff) a2 = 0;
            if (a1 > c1 - c1 * roundoff) a1 = c1;
            else if (a1 < c1 * roundoff) a1 = 0;

            // Update error cache using new Lagrange multipliers
            double t1 = y1 * (a1 - alph1);
            double t2 = y2 * (a2 - alph2);
            foreach (int i in nonBoundExamples) // (alpha[i] > 0 && alpha[i] < c)
            {
                if (i != i1 && i != i2)
                {
                    errors[i] += t1 * kernelCache.GetOrCompute(i1, i) +
                                 t2 * kernelCache.GetOrCompute(i2, i);
                }
            }

            errors[i1] += t1 * k11 + t2 * k12;
            errors[i2] += t1 * k12 + t2 * k22;


            // Update Lagrange multipliers
            alpha[i1] = a1;
            alpha[i2] = a2;

            // If linear, update weights
            if (isLinear)
            {
                // (eq 22 of Platt, 1998)
                for (int i = 0; i < weights.Length; i++)
                    weights[i] += t1 * p1[i] + t2 * p2[i];
            }


            // Update indices 
            updateSets(i1, a1);
            updateSets(i2, a2);


            // Update threshold (bias) to reflect change in Lagrange multipliers
            // [Keerthi] This should be done by computing (i_low, b_low) and (i_up, b_up)
            //   by applying equations (11a) and (11b), using only i1, i2 and indices in
            //   I0 according to item 3 of section 5 from Keerthi's technical report.

            i_lower = i1; b_lower = errors[i1];
            i_upper = i1; b_upper = errors[i1];

            if (errors[i2] > b_lower) { i_lower = i2; b_lower = errors[i2]; }
            if (errors[i2] < b_upper) { i_upper = i2; b_upper = errors[i2]; }

            foreach (int i in nonBoundExamples)
            {
                if (errors[i] > b_lower) { i_lower = i; b_lower = errors[i]; }
                if (errors[i] < b_upper) { i_upper = i; b_upper = errors[i]; }
            }

            // step taken
            return true;
        }


        private void updateSets(int index, double value)
        {
            if (value > 0)
                activeExamples.Add(index);
            else
                activeExamples.Remove(index);

            double cindex = outputs[index] == +1 ? positiveCost : negativeCost;

            if (value == 0 || value == cindex)
            {
                // Value is at boundaries
                nonBoundExamples.Remove(index);
            }
            else
            {
                nonBoundExamples.Add(index);
            }
        }

        /// <summary>
        ///   Computes the SVM output for a given point.
        /// </summary>
        /// 
        private double compute(double[] point)
        {
            double sum = -(b_lower + b_upper) / 2;

            if (isLinear)
            {
                for (int i = 0; i < weights.Length; i++)
                    sum += weights[i] * point[i];
            }
            else
            {
                foreach (int i in activeExamples)
                    sum += alpha[i] * outputs[i] * kernel.Function(inputs[i], point);
            }

            return sum;
        }

        /// <summary>
        ///   Computes the SVM output for a given point.
        /// </summary>
        /// 
        private double computeNoBias(int j)
        {
            double sum = 0;

            double[] point = inputs[j];

            if (isLinear)
            {
                for (int i = 0; i < weights.Length; i++)
                    sum += weights[i] * point[i];
            }
            else
            {
                foreach (int i in activeExamples)
                    sum += alpha[i] * outputs[i] * kernelCache.GetOrCompute(i, j);
            }

            return sum;
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
