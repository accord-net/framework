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
    using Accord.Math;
    using Accord.Statistics;
    using Accord.Statistics.Kernels;
    using Math.Optimization;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Diagnostics;
    using Accord.Compat;

    /// <summary>
    ///   Gets the selection strategy to be used in SMO.
    /// </summary>
    /// 
    public enum SelectionStrategy
    {

        /// <summary>
        ///   Uses the sequential selection strategy as
        ///   suggested by Keerthi et al's algorithm 1.
        /// </summary>
        /// 
        Sequential,

        /// <summary>
        ///   Always select the worst violation pair
        ///   to be optimized first, as suggested in
        ///   Keerthi et al's algorithm 2.
        /// </summary>
        /// 
        WorstPair,

        /// <summary>
        ///   Use a second order selection algorithm, using
        ///   the same algorithm as LibSVM's implementation.
        /// </summary>
        /// 
        SecondOrder,
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
    ///   This class can also be used in combination with <see cref="MulticlassSupportVectorLearning{TKernel}"/>
    ///   or <see cref="MultilabelSupportVectorLearning{TKernel}"/> to learn <see cref="MulticlassSupportVectorMachine{TKernel}"/>s
    ///   using the <c>one-vs-one</c> or <c>one-vs-all</c> multi-class decision strategies, respectively.</para>
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
    ///   <para>The following example shows how to use a SVM to learn a simple XOR function.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\SequentialMinimalOptimizationTest.cs" region="doc_xor_normal" />
    ///   
    ///   <para>The next example shows how to solve a multi-class problem using a one-vs-one SVM 
    ///   where the binary machines are learned using SMO.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MulticlassSupportVectorLearningTest.cs" region="doc_learn" />
    ///   
    ///   <para>The same as before, but using a Gaussian kernel.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MulticlassSupportVectorLearningTest.cs" region="doc_learn_gaussian" />
    ///   
    ///   <para>
    ///   The following example shows how to learn a simple binary SVM using
    ///    a precomputed kernel matrix obtained from a Gaussian kernel.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\SequentialMinimalOptimizationTest.cs" region="doc_precomputed" />
    /// </example>
    /// 
    /// <seealso cref="SupportVectorMachine"/>
    /// <seealso cref="ProbabilisticOutputCalibration"/>
    /// <seealso cref="MulticlassSupportVectorLearning{TKernel}"/>
    /// 
    //[Obsolete("Please use SequentialMinimalOptimization<TKernel> instead.")]
    public class SequentialMinimalOptimization :
        BaseSequentialMinimalOptimization<ISupportVectorMachine<double[]>, IKernel<double[]>, double[]>,
        ISupportVectorMachineLearning
    {
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

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please do not pass parameters in the constructor. Use the default constructor and the Learn method instead.")]
        public SequentialMinimalOptimization(SupportVectorMachine model, double[][] input, int[] output)
            : base(model, input, output)
        {
            init();
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please do not pass parameters in the constructor. Use the default constructor and the Learn method instead.")]
        public SequentialMinimalOptimization(KernelSupportVectorMachine model, double[][] input, int[] output)
            : base(model, input, output)
        {
            init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequentialMinimalOptimization"/> class.
        /// </summary>
        public SequentialMinimalOptimization()
        {
            init();
        }

        private void init()
        {
            if (Kernel == null)
                this.Kernel = new Linear();
        }


        // TODO: Those methods are being shadowed to provide temporary support to the 
        // previous way of creating support vector machines in the framework. After a
        // a few releases, those methods will have to be removed and this class will need
        // to be updated to derive from SequentialMinimalOptimization<SupportVectorMachine>.

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        public new SupportVectorMachine Learn(double[][] x, double[] y, double[] weights = null)
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
        public new SupportVectorMachine Learn(double[][] x, int[] y, double[] weights = null)
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
        public new SupportVectorMachine Learn(double[][] x, int[][] y, double[] weights = null)
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
        public new SupportVectorMachine Learn(double[][] x, bool[][] y, double[] weights = null)
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
        public new SupportVectorMachine Learn(double[][] x, bool[] y, double[] weights = null)
        {
            ISupportVectorMachine<double[]> svm = base.Learn(x, y, weights);
            return (SupportVectorMachine)svm;
        }
    }

    /// <summary>
    ///   Sequential Minimal Optimization (SMO) Algorithm.
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
    ///   This class can also be used in combination with <see cref="MulticlassSupportVectorLearning{TKernel}"/>
    ///   or <see cref="MultilabelSupportVectorLearning{TKernel}"/> to learn <see cref="MulticlassSupportVectorMachine{TKernel}"/>s
    ///   using the <c>one-vs-one</c> or <c>one-vs-all</c> multi-class decision strategies, respectively.</para>
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
    ///   <para>The following example shows how to use a SVM to learn a simple XOR function.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\SequentialMinimalOptimizationTest.cs" region="doc_xor_normal" />
    ///   
    ///   <para>The next example shows how to solve a multi-class problem using a one-vs-one SVM 
    ///   where the binary machines are learned using SMO.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MulticlassSupportVectorLearningTest.cs" region="doc_learn" />
    ///   
    ///   <para>The same as before, but using a Gaussian kernel.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MulticlassSupportVectorLearningTest.cs" region="doc_learn_gaussian" />
    ///   
    ///   <para>
    ///   The following example shows how to learn a simple binary SVM using
    ///    a precomputed kernel matrix obtained from a Gaussian kernel.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\SequentialMinimalOptimizationTest.cs" region="doc_precomputed" />
    /// </example>
    /// 
    /// <seealso cref="SupportVectorMachine"/>
    /// 
    /// <seealso cref="ProbabilisticOutputCalibration"/>
    /// <seealso cref="MulticlassSupportVectorLearning{TKernel}"/>
    /// 
    public class SequentialMinimalOptimization<TKernel> :
    BaseSequentialMinimalOptimization<
        SupportVectorMachine<TKernel>, TKernel, double[]>
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
    ///   Sequential Minimal Optimization (SMO) Algorithm (for arbitrary data types).
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
    ///   This class can also be used in combination with <see cref="MulticlassSupportVectorLearning{TKernel}"/>
    ///   or <see cref="MultilabelSupportVectorLearning{TKernel}"/> to learn <see cref="MulticlassSupportVectorMachine{TKernel}"/>s
    ///   using the <c>one-vs-one</c> or <c>one-vs-all</c> multi-class decision strategies, respectively.</para>
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
    /// 
    /// <example>
    ///   <para>The following example shows how to use a SVM to learn a simple XOR function.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\SequentialMinimalOptimizationTest.cs" region="doc_xor_normal" />
    ///   
    ///   <para>The next example shows how to solve a multi-class problem using a one-vs-one SVM 
    ///   where the binary machines are learned using SMO.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MulticlassSupportVectorLearningTest.cs" region="doc_learn" />
    ///   
    ///   <para>The same as before, but using a Gaussian kernel.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MulticlassSupportVectorLearningTest.cs" region="doc_learn_gaussian" />
    ///   
    ///   <para>
    ///   The following example shows how to learn a simple binary SVM using
    ///    a precomputed kernel matrix obtained from a Gaussian kernel.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\SequentialMinimalOptimizationTest.cs" region="doc_precomputed" />
    /// </example>
    /// 
    /// <seealso cref="SupportVectorMachine"/>
    /// 
    /// <seealso cref="ProbabilisticOutputCalibration"/>
    /// <seealso cref="MulticlassSupportVectorLearning{TKernel}"/>
    /// 
    public class SequentialMinimalOptimization<TKernel, TInput> :
        BaseSequentialMinimalOptimization<
            SupportVectorMachine<TKernel, TInput>, TKernel, TInput>
        where TKernel : IKernel<TInput>
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
    ///   Base class for Sequential Minimal Optimization.
    /// </summary>
    /// 
    public abstract class BaseSequentialMinimalOptimization<TModel, TKernel, TInput> :
        BaseSupportVectorClassification<TModel, TKernel, TInput>,
        ISupportVectorMachineLearning<TInput>
        where TKernel : IKernel<TInput>
        where TModel : ISupportVectorMachine<TInput>
        // TODO: after a few releases, the TModel constraint should be changed to:
        // where TModel : SupportVectorMachine<TKernel, TInput>, ISupportVectorMachine<TInput>
    {
        // Learning algorithm parameters
        private double tolerance = 1e-2;
        private double epsilon = 1e-6;
        private bool shrinking;

        // Support Vector Machine parameters
        private double[] alpha;
        private bool isCompact;

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

        private int cacheSize = -1;
        private KernelFunctionCache<TKernel, TInput> kernelCache;

        private SelectionStrategy strategy = SelectionStrategy.WorstPair;

        private int maxChecks = 100;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSequentialMinimalOptimization{TModel, TKernel, TInput}"/> class.
        /// </summary>
        public BaseSequentialMinimalOptimization()
        {
            // Index sets
            activeExamples = new HashSet<int>();
            nonBoundExamples = new HashSet<int>();
            atBoundsExamples = new HashSet<int>();
        }


        /// <summary>
        ///   Epsilon for round-off errors. Default value is 1e-6.
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
        ///   Gets or sets the cache size to partially store the kernel 
        ///   matrix. Default is the same number of input vectors, meaning
        ///   the entire kernel matrix will be computed and cached in memory.
        ///   If set to zero, the cache will be disabled and all operations will
        ///   be computed as needed.
        /// </summary>
        /// 
        /// <remarks>
        ///   In order to know how many rows can fit under a amount of memory, you can use
        ///   <see cref="KernelFunctionCache.GetNumberOfRowsForMaximumSizeInBytes(int)"/>.
        ///   Be sure to also test the algorithm with the cache disabled, as sometimes the
        ///   cost of the extra memory allocations needed by the cache will be higher than
        ///   the cost of evaluating the kernel function, specially for fast kernels such
        ///   as <see cref="Linear"/>.
        /// </remarks>
        /// 
        public int CacheSize
        {
            get { return cacheSize; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");
                this.cacheSize = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether shrinking heuristics should be used. Default is false. Note: 
        ///   this property can only be used when <see cref="Strategy"/> is set to <see cref="SelectionStrategy.SecondOrder"/>.
        /// </summary>
        /// 
        /// <value>
        ///   <c>true</c> to use shrinking heuristics; otherwise, <c>false</c>.
        /// </value>
        /// 
        public bool Shrinking
        {
            get { return shrinking; }
            set { shrinking = value; }
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
        [Obsolete("To generate compact linear machines, call the Compact method after creation.")]
        public bool Compact
        {
            get { return isCompact; }
            set
            {
                if (!(Kernel is Linear) && value)
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




        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        protected override void InnerRun()
        {
            // Initialize variables
            int samples = Inputs.Length;
            double[] c = C;
            TInput[] x = Inputs;
            int[] y = Outputs;

            // Lagrange multipliers
            this.alpha = new double[samples];

            // Prepare indices sets
            activeExamples = new HashSet<int>();
            nonBoundExamples = new HashSet<int>();
            atBoundsExamples = new HashSet<int>();

            // Kernel cache
            if (this.cacheSize == -1)
                this.cacheSize = samples;

            using (this.kernelCache = new KernelFunctionCache<TKernel, TInput>(Kernel, Inputs, cacheSize))
            {
                bool diverged = false;


                if (Strategy == SelectionStrategy.SecondOrder)
                {
                    double[] minusOnes = Vector.Create(Inputs.Length, -1.0);
                    Func<int, int[], int, double[], double[]> Q;

                    if (kernelCache.Enabled)
                    {
                        Q = (int i, int[] indices, int length, double[] row) =>
                        {
                            for (int j = 0; j < length; j++)
                                row[j] = y[i] * y[indices[j]] * kernelCache.GetOrCompute(i, indices[j]);
                            return row;
                        };
                    }
                    else
                    {
                        Q = (int i, int[] indices, int length, double[] row) =>
                        {
                            for (int j = 0; j < length; j++)
                                row[j] = y[i] * y[indices[j]] * Kernel.Function(x[i], x[indices[j]]);
                            return row;
                        };
                    }

                    var s = new FanChenLinQuadraticOptimization(alpha.Length, Q, minusOnes, y)
                    {
                        Tolerance = tolerance,
                        Shrinking = this.shrinking,
                        Solution = alpha,
                        Token = Token,
                        UpperBounds = c
                    };

                    diverged = !s.Minimize();

                    // Store information about active examples
                    for (int i = 0; i < alpha.Length; i++)
                    {
                        if (alpha[i] > 0)
                            activeExamples.Add(i);
                    }

                    b_lower = b_upper = s.Rho;
                }
                else // Strategy is Strategy.WorstPair or Strategy.Sequential
                {
                    if (shrinking)
                        throw new InvalidOperationException("Shrinking heuristic can only be used if Strategy is set to SelectionStrategy.SecondOrder.");

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


                    // Error cache
                    this.errors = new double[samples];

                    // [Keerthi] Initialize b_up to -1 and 
                    //   i_up to any one index of class 1:
                    this.b_upper = -1;
                    this.i_upper = y.First(y_i => y_i > 0);

                    // [Keerthi] Initialize b_low to +1 and 
                    //   i_low to any one index of class 2:
                    this.b_lower = +1;
                    this.i_lower = y.First(y_i => y_i < 0);

                    // [Keerthi] Set error cache for i_low and i_up:
                    this.errors[i_lower] = +1;
                    this.errors[i_upper] = -1;


                    // Algorithm:
                    int numChanged = 0;
                    int wholeSetChecks = 0;
                    bool examineAll = true;
                    bool shouldStop = false;

                    while ((numChanged > 0 || examineAll) && !shouldStop)
                    {
                        if (Token.IsCancellationRequested)
                            break;

                        numChanged = 0;
                        if (examineAll)
                        {
                            // loop I over all training examples
                            for (int i = 0; i < samples; i++)
                                if (examineExample(i))
                                    numChanged++;

                            wholeSetChecks++;
                        }
                        else
                        {
                            if (strategy == SelectionStrategy.Sequential)
                            {
                                // loop I over examples not at bounds
                                for (int i = 0; i < alpha.Length; i++)
                                {
                                    if (alpha[i] != 0 && alpha[i] != c[i])
                                    {
                                        if (examineExample(i))
                                            numChanged++;

                                        if (b_upper > b_lower - 2.0 * tolerance)
                                        {
                                            numChanged = 0; break;
                                        }
                                    }
                                }
                            }
                            else if (strategy == SelectionStrategy.WorstPair)
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
                            else
                            {
                                throw new InvalidOperationException("Unknown strategy");
                            }
                        }

                        if (examineAll)
                            examineAll = false;

                        else if (numChanged == 0)
                            examineAll = true;

                        if (wholeSetChecks > maxChecks)
                            shouldStop = diverged = true;

                        if (Token.IsCancellationRequested)
                            shouldStop = true;
                    }
                }


                // Store information about bounded examples
                for (int i = 0; i < alpha.Length; i++)
                {
                    if (alpha[i] == c[i])
                        atBoundsExamples.Add(i);
                }


                // Store Support Vectors in the SV Machine. Only vectors which have Lagrange multipliers
                // greater than zero will be stored as only those are actually required during evaluation.

                int activeCount = activeExamples.Count;
                Model.SupportVectors = new TInput[activeCount];
                Model.Weights = new double[activeCount];
                int index = 0;
                foreach (var j in activeExamples)
                {
                    Model.SupportVectors[index] = x[j];
                    Model.Weights[index] = alpha[j] * y[j];
                    index++;
                }

                Model.Threshold = -(b_lower + b_upper) / 2;

                if (isCompact)
                    Model.Compress();

                if (diverged)
                {
                    throw new ConvergenceException("Convergence could not be attained. " +
                                "Please reduce the cost of misclassification errors by reducing " +
                                "the complexity parameter C or try a different kernel function.");
                }
            }
        }




        /// <summary>
        ///  Chooses which multipliers to optimize using heuristics.
        /// </summary>
        /// 
        private bool examineExample(int i2)
        {
            // double[] p2 = Inputs[i2]; // Input point at index i2
            double y2 = Outputs[i2];  // Classification label for p2
            // double alph2 = alpha[i2]; // Lagrange multiplier for p2

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
            double a = this.alpha[i];
            double y = base.Outputs[i];
            double c = this.C[i];

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
            else if (y == -1 && a == c)
                return 2; // I2
            else if (y == +1 && a == c)
                return 3; // I3
            else if (y == -1 && a == 0)
                return 4; // I4

            return 0; // I0 (not at bounds)
        }

        /// <summary>
        ///   Analytically solves the optimization problem for two Lagrange multipliers.
        /// </summary>
        /// 
        private bool takeStep(int i1, int i2)
        {
            if (i1 == i2)
                return false;

            //TInput p1 = Inputs[i1]; // Input point at index i1
            double alph1 = alpha[i1]; // Lagrange multiplier for p1
            double y1 = Outputs[i1];  // Classification label for p1
            double c1 = C[i1];

            // SVM output on p1 - y1 [without bias threshold]. Check if it has already been computed
            double e1 = (alph1 > 0 && alph1 < c1) ? errors[i1] : errors[i1] = computeNoBias(i1) - y1;

            //TInput p2 = Inputs[i2]; // Input point at index i2
            double alph2 = alpha[i2]; // Lagrange multiplier for p2
            double y2 = Outputs[i2];  // Classification label for p2
            double c2 = C[i2];


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
                H = Math.Min(c2, c1 + alph2 - alph1);
            }
            else
            {
                // If the target y1 does equal the target               (14)
                // y2, then the following bounds apply to a2:
                L = Math.Max(0, alph2 + alph1 - c1);
                H = Math.Min(c2, alph2 + alph1);
            }

            if (L >= H)
                return false;


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

                Trace.WriteLine("SMO: eta is zero.");
            }

            if (Math.Abs(a2 - alph2) < epsilon * (a2 + alph2 + epsilon))
                return false; // no step need to be taken

            // Approximate precision as
            // suggested in Platt's errata
            double roundoff = 1e-8;
            if (a2 > c2 - c2 * roundoff) a2 = c2;
            else if (a2 < c2 * roundoff) a2 = 0;

            // Compute update step
            a1 = alph1 + s * (alph2 - a2);

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

            if (value == 0 || value == C[index])
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
        private double computeNoBias(int j)
        {
            double sum = 0;
            foreach (int i in activeExamples)
                sum += alpha[i] * Outputs[i] * kernelCache.GetOrCompute(i, j);
            return sum;
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        protected BaseSequentialMinimalOptimization(ISupportVectorMachine<TInput> model, TInput[] input, int[] output)
            : base(model, input, output)
        {
        }

    }
}
