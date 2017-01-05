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

namespace Accord.MachineLearning.VectorMachines
{
    using Accord.MachineLearning;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Math;
    using Accord.Statistics;
    using Accord.Statistics.Kernels;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///   One-against-all Multi-label Kernel Support Vector Machine Classifier.
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    ///   The Support Vector Machine is by nature a binary classifier. Multiple label
    ///   problems are problems in which an input sample is allowed to belong to one
    ///   or more classes. A way to implement multi-label classes in support vector
    ///   machines is to build a one-against-all decision scheme where multiple SVMs
    ///   are trained to detect each of the available classes. </para>
    /// <para>
    ///   Currently this class supports only Kernel machines as the underlying classifiers.
    ///   If a Linear Support Vector Machine is needed, specify a Linear kernel in the
    ///   constructor at the moment of creation. </para>
    ///
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       <a href="http://courses.media.mit.edu/2006fall/mas622j/Projects/aisen-project/index.html">
    ///        http://courses.media.mit.edu/2006fall/mas622j/Projects/aisen-project/index.html </a></description></item>
    ///     <item><description>
    ///       <a href="http://nlp.stanford.edu/IR-book/html/htmledition/multiclass-svms-1.html">
    ///        http://nlp.stanford.edu/IR-book/html/htmledition/multiclass-svms-1.html </a></description></item>
    ///     </list></para>
    /// </remarks>
    ///
    /// <example>
    /// <para>
    ///   The following example shows how to learn a linear, multi-label (one-vs-rest) support 
    ///   vector machine using the <see cref="LinearDualCoordinateDescent"/> algorithm. </para>
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MultilabelSupportVectorLearningTest.cs" region="doc_learn_ldcd" />
    /// 
    /// <para>
    ///   The following example shows how to learn a non-linear, multi-label (one-vs-rest) 
    ///   support vector machine using the <see cref="Gaussian"/> kernel and the 
    ///   <see cref="SequentialMinimalOptimization"/> algorithm. </para>
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MultilabelSupportVectorLearningTest.cs" region="doc_learn_gaussian" />
    ///   
    /// <para>
    ///   Support vector machines can have their weights calibrated in order to produce probability 
    ///   estimates (instead of simple class separation distances). The following example shows how 
    ///   to use <see cref="ProbabilisticOutputCalibration"/> within <see cref="MulticlassSupportVectorLearning"/> 
    ///   to generate a probabilistic SVM:</para>
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MultilabelSupportVectorLearningTest.cs" region="doc_learn_calibration" />
    /// </example>
    /// 
    /// <seealso cref="MulticlassSupportVectorMachine"/>
    /// <seealso cref="SupportVectorMachine"/>
    /// <seealso cref="KernelSupportVectorMachine"/>
    /// 
    /// <seealso cref="MultilabelSupportVectorLearning"/>
    /// <seealso cref="SequentialMinimalOptimization"/>
    ///
    [Serializable]
    public class MultilabelSupportVectorMachine<TKernel, TInput> :
        MultilabelSupportVectorMachine<SupportVectorMachine<TKernel, TInput>, TKernel, TInput>
        where TKernel : IKernel<TInput>
        where TInput : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultilabelSupportVectorMachine{TModel, TKernel, TInput}"/> class.
        /// </summary>
        /// <param name="classes">The number of classes in the multi-class classification problem.</param>
        /// <param name="inputs">The number of inputs (length of the input vectors) accepted by the machine.</param>
        /// <param name="kernel">The kernel function to be used.</param>
        public MultilabelSupportVectorMachine(int inputs, TKernel kernel, int classes)
            : base(classes, () => new SupportVectorMachine<TKernel, TInput>(inputs, kernel))
        {
        }
    }

    /// <summary>
    ///   One-against-all Multi-label Kernel Support Vector Machine Classifier.
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    ///   The Support Vector Machine is by nature a binary classifier. Multiple label
    ///   problems are problems in which an input sample is allowed to belong to one
    ///   or more classes. A way to implement multi-label classes in support vector
    ///   machines is to build a one-against-all decision scheme where multiple SVMs
    ///   are trained to detect each of the available classes. </para>
    /// <para>
    ///   Currently this class supports only Kernel machines as the underlying classifiers.
    ///   If a Linear Support Vector Machine is needed, specify a Linear kernel in the
    ///   constructor at the moment of creation. </para>
    ///
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       <a href="http://courses.media.mit.edu/2006fall/mas622j/Projects/aisen-project/index.html">
    ///        http://courses.media.mit.edu/2006fall/mas622j/Projects/aisen-project/index.html </a></description></item>
    ///     <item><description>
    ///       <a href="http://nlp.stanford.edu/IR-book/html/htmledition/multiclass-svms-1.html">
    ///        http://nlp.stanford.edu/IR-book/html/htmledition/multiclass-svms-1.html </a></description></item>
    ///     </list></para>
    ///
    /// </remarks>
    ///
    /// <example>
    /// <para>
    ///   The following example shows how to learn a linear, multi-label (one-vs-rest) support 
    ///   vector machine using the <see cref="LinearDualCoordinateDescent"/> algorithm. </para>
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MultilabelSupportVectorLearningTest.cs" region="doc_learn_ldcd" />
    /// 
    /// <para>
    ///   The following example shows how to learn a non-linear, multi-label (one-vs-rest) 
    ///   support vector machine using the <see cref="Gaussian"/> kernel and the 
    ///   <see cref="SequentialMinimalOptimization"/> algorithm. </para>
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MultilabelSupportVectorLearningTest.cs" region="doc_learn_gaussian" />
    ///   
    /// <para>
    ///   Support vector machines can have their weights calibrated in order to produce probability 
    ///   estimates (instead of simple class separation distances). The following example shows how 
    ///   to use <see cref="ProbabilisticOutputCalibration"/> within <see cref="MulticlassSupportVectorLearning"/> 
    ///   to generate a probabilistic SVM:</para>
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MultilabelSupportVectorLearningTest.cs" region="doc_learn_calibration" />
    /// </example>
    /// 
    /// <seealso cref="MulticlassSupportVectorMachine"/>
    /// <seealso cref="SupportVectorMachine"/>
    /// <seealso cref="KernelSupportVectorMachine"/>
    /// 
    /// <seealso cref="MultilabelSupportVectorLearning"/>
    /// <seealso cref="SequentialMinimalOptimization"/>
    ///
    [Serializable]
    public class MultilabelSupportVectorMachine<TModel, TKernel, TInput> :
        OneVsRest<TModel, TInput>,
        IDisposable
        where TKernel : IKernel<TInput>
        where TModel : SupportVectorMachine<TKernel, TInput>
        where TInput : ICloneable
    {

        // Multi-label statistics
        private int? totalVectorsCount;
        private int? uniqueVectorsCount;
        private int? sharedVectorsCount;


        // Performance optimizations
        [NonSerialized]
        private Lazy<int[][]> sharedVectors;

        [NonSerialized]
        private ThreadLocal<Cache> vectorCache;

        [NonSerialized]
        private ParallelOptions parallelOptions = new ParallelOptions();




        private void initialize()
        {
            this.vectorCache = new ThreadLocal<Cache>(() => new Cache());
            this.sharedVectors = new Lazy<int[][]>(computeSharedVectors, true);
        }


        #region Properties


        /// <summary>
        ///   Gets the total number of support vectors
        ///   in the entire multi-label machine.
        /// </summary>
        ///
        public int SupportVectorCount
        {
            get
            {
                if (totalVectorsCount == null)
                {
                    int count = 0;
                    for (int i = 0; i < Models.Length; i++)
                        if (Models[i].SupportVectors != null)
                            count += Models[i].SupportVectors.Length;
                    totalVectorsCount = count;
                }

                return totalVectorsCount.Value;
            }
        }

        /// <summary>
        ///   Gets the number of unique support
        ///   vectors in the multi-label machine.
        /// </summary>
        ///
        public int SupportVectorUniqueCount
        {
            get
            {
                if (uniqueVectorsCount == null)
                {
                    var unique = new HashSet<TInput>();
                    for (int i = 0; i < Models.Length; i++)
                        if (Models[i].SupportVectors != null)
                            for (int k = 0; k < Models[i].SupportVectors.Length; k++)
                                unique.Add(Models[i].SupportVectors[k]);
                    uniqueVectorsCount = unique.Count;
                }

                return uniqueVectorsCount.Value;
            }
        }

        /// <summary>
        ///   Gets the number of shared support
        ///   vectors in the multi-label machine.
        /// </summary>
        ///
        public int SupportVectorSharedCount
        {
            get
            {
                if (sharedVectorsCount == null)
                {
                    var v = sharedVectors.Value;
                }
                return sharedVectorsCount.Value;
            }
        }


        #endregion

        /// <summary>
        ///   Gets or sets the parallelization options used
        ///   when deciding the class of a new sample.
        /// </summary>
        /// 
        public ParallelOptions ParallelOptions
        {
            get { return parallelOptions; }
            set { parallelOptions = value; }
        }

        /// <summary>
        ///   If the inner machines have a linear kernel, compresses
        ///   their support vectors into a single parameter vector for
        ///   each machine.
        /// </summary>
        /// 
        public void Compress()
        {
            foreach (var model in this)
                model.Value.Compress();
        }



        /// <summary>
        ///   Compute SVM output with support vector sharing.
        /// </summary>
        ///
        private double distance(int index, TInput input, Cache cache)
        {
            // Get the machine for this problem
            var machine = Models[index];

            // Get the vectors shared among all machines
            int[] vectors = cache.Vectors[index];
            double[] values = cache.Products;

            if (machine.SupportVectors.Length == 1)
            {
                // For linear machines, computation is simpler
                return machine.Threshold + machine.Weights[0] * machine.Kernel.Function(machine.SupportVectors[0], input);
            }

            double sum = machine.Threshold;

            // For each support vector in the machine
            for (int i = 0; i < vectors.Length; i++)
            {
                double value;

                // Check if it is a shared vector
                int j = vectors[i];

                if (j >= 0)
                {
                    // This is a shared vector. Check
                    // if it has already been computed

                    if (!Double.IsNaN(values[j]))
                    {
                        // Yes, it has. Retrieve the value from the cache
                        value = values[j];
                        Interlocked.Increment(ref cache.Hits);
                    }
                    else
                    {
                        // No, it has not. Compute and store the computed value in the cache
                        value = values[j] = machine.Kernel.Function(machine.SupportVectors[i], input);
                        Interlocked.Increment(ref cache.Evaluations);
                    }
                }
                else
                {
                    // This vector is not shared by any other machine. No need to cache
                    value = machine.Kernel.Function(machine.SupportVectors[i], input);
                    Interlocked.Increment(ref cache.Evaluations);
                }

                sum += machine.Weights[i] * value;
            }

#if DEBUG
            double expected = machine.Score(input);
            Accord.Diagnostics.Debug.Assert(sum.IsEqual(expected, rtol: 1e-3));
#endif

            return sum;
        }

        /// <summary>
        ///   Resets the cache and machine statistics
        ///   so they can be recomputed on next evaluation.
        /// </summary>
        ///
        public void Reset()
        {
            if (this.vectorCache != null)
                this.vectorCache.Dispose();

            this.sharedVectors = null;
            this.totalVectorsCount = null;
            this.uniqueVectorsCount = null;
            this.sharedVectorsCount = null;

            this.initialize();
        }




        /// <summary>
        /// Computes whether a class label applies to an <paramref name="input" /> vector.
        /// </summary>
        /// <param name="input">The input vectors that should be classified as
        /// any of the <see cref="Accord.MachineLearning.ITransform.NumberOfOutputs" /> possible classes.</param>
        /// <param name="classIndex">The class label index to be tested.</param>
        /// <returns>
        /// A boolean value indicating whether the given <paramref name="classIndex">
        /// class label</paramref> applies to the <paramref name="input" /> vector.
        /// </returns>
        public override bool Decide(TInput input, int classIndex)
        {
            return Models[classIndex].Decide(input);
        }

        /// <summary>
        /// Computes class-label decisions for the given <paramref name="input" />.
        /// </summary>
        /// <param name="input">The input vectors that should be classified as
        /// any of the <see cref="Accord.MachineLearning.ITransform.NumberOfOutputs" /> possible classes.</param>
        /// <param name="result">The location where to store the class-labels.</param>
        /// <returns>
        /// A set of class-labels that best describe the <paramref name="input" />
        /// vectors according to this classifier.
        /// </returns>
        public override bool[] Decide(TInput input, bool[] result)
        {
            Cache cache = createOrResetCache();
            Parallel.For(0, Models.Length, ParallelOptions, i =>
            {
                result[i] = Classes.Decide(distance(i, input, cache));
            });
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// <param name="result">An array where the scores will be stored,
        /// avoiding unnecessary memory allocations.</param>
        /// <returns></returns>
        public override double[] Scores(TInput input, ref bool[] decision, double[] result)
        {
            if (decision == null)
                decision = new bool[NumberOfOutputs];

            Cache cache = createOrResetCache();

            var d = decision;
            Parallel.For(0, Models.Length, ParallelOptions, i =>
            {
                result[i] = distance(i, input, cache);
                d[i] = Classes.Decide(result[i]);
            });

            decision = d;
            return result;
        }



        [OnDeserialized]
        private void onDeserialized(StreamingContext context)
        {
            initialize();
            parallelOptions = new ParallelOptions();
        }



        /// <summary>
        ///   Performs application-defined tasks associated with
        ///   freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        ///
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources
        /// </summary>
        ///
        /// <param name="disposing">
        ///   <c>true</c> to release both managed and unmanaged resources;
        ///   <c>false</c> to release only unmanaged resources.</param>
        ///
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (vectorCache != null)
                {
                    vectorCache.Dispose();
                    vectorCache = null;
                }
            }
        }




        #region Cache
        private Cache createOrResetCache()
        {
            Cache cache = vectorCache.Value;

            // First of all, check if the shared vectors in this machine
            // already have been identified. If they don't, identify them.

            cache.Vectors = sharedVectors.Value; // use lazy instantiation
            int vectorCount = SupportVectorSharedCount;

            // Now, check if a cache has already been created for this
            // thread and has adequate size. If it has not, create it.

            if (cache.Products == null || cache.Products.Length < vectorCount)
            {
                // The cache has not been created
                cache.Products = new double[vectorCount];

#if !NET35      // Create synchronization objects
                cache.SyncObjects = new SpinLock[vectorCount];
                for (int i = 0; i < cache.SyncObjects.Length; i++)
                    cache.SyncObjects[i] = new SpinLock();
#endif
            }

            // Initialize (or reset) the cache. A value of Not-a-Number
            // indicates that the value of corresponding vector has not
            // been computed yet.
            for (int i = 0; i < cache.Products.Length; i++)
                cache.Products[i] = Double.NaN;


            cache.Evaluations = 0;
            cache.Hits = 0;

            return cache;
        }


        private int[][] computeSharedVectors()
        {
            // This method should only be called once after the machine has
            // been learned. If the inner machines or they Support Vectors
            // change, this method will need to be recomputed.

            // Detect all vectors which are being shared along the machines
            var shared = new Dictionary<TInput, List<Tuple<int, int>>>();

            // for all machines
            for (int i = 0; i < Models.Length; i++)
            {
                // if the machine is not in compact form
                if (Models[i].SupportVectors != null)
                {
                    // register the support vector on the shared cache collection
                    for (int k = 0; k < Models[i].SupportVectors.Length; k++)
                    {
                        TInput sv = Models[i].SupportVectors[k];

                        List<Tuple<int, int>> count;
                        bool success = shared.TryGetValue(sv, out count);

                        if (success)
                        {
                            // Value is already in the dictionary
                            count.Add(Tuple.Create(i, k));
                        }
                        else
                        {
                            count = new List<Tuple<int, int>>();
                            count.Add(Tuple.Create(i, k));
                            shared[sv] = count;
                        }
                    }

                }
            }

            // Create a table of indices for shared vectors
            int idx = 0;

            var indices = new Dictionary<TInput, int>();
            foreach (TInput sv in shared.Keys)
                indices[sv] = idx++;

            // Create a lookup table for the machines
            var sharedVectors = new int[Models.Length][];
            for (int i = 0; i < sharedVectors.Length; i++)
            {
                if (Models[i].SupportVectors != null)
                {
                    sharedVectors[i] = new int[Models[i].SupportVectors.Length];

                    for (int k = 0; k < Models[i].SupportVectors.Length; k++)
                    {
                        TInput sv = Models[i].SupportVectors[k];
                        if (shared.ContainsKey(sv))
                            sharedVectors[i][k] = indices[sv];
                        else
                            sharedVectors[i][k] = -1;
                    }
                }
            }


            sharedVectorsCount = shared.Count;
            return sharedVectors;
        }



        /// <summary>
        ///   Gets the total kernel evaluations performed in the last call
        ///   to Decide(TInput) and similar functions in the current thread.
        /// </summary>
        ///
        /// <returns>The number of total kernel evaluations.</returns>
        ///
        public int GetLastKernelEvaluations()
        {
            return vectorCache.Value.Evaluations;
        }

        /// <summary>
        ///   Gets the number of cache hits during in the last call
        ///   to Decide(TInput) and similar functions in the current thread.
        /// </summary>
        ///
        /// <returns>The number of cache hits in the last decision.</returns>
        ///
        public int GetLastKernelHits()
        {
            return vectorCache.Value.Hits;
        }

        private class Cache
        {
            public int Hits;
            public int Evaluations;
            public double[] Products;
            public int[][] Vectors;
#if !NET35
            public SpinLock[] SyncObjects;
#endif
        }
        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="MultilabelSupportVectorMachine{TModel, TKernel, TInput}"/> class.
        /// </summary>
        /// <param name="classes">The number of classes in the multi-label classification problem.</param>
        /// <param name="initializer">A function to create the inner binary classifiers.</param>
        public MultilabelSupportVectorMachine(int classes, Func<TModel> initializer)
            : base(classes, initializer)
        {
            this.ParallelOptions = new ParallelOptions();
            initialize();
        }
        
    }
}