// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    using Accord.Math;
    using Accord.Statistics.Kernels;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///   One-against-one Multi-class Kernel Support Vector Machine Classifier.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Support Vector Machine is by nature a binary classifier. One of the ways
    ///   to extend the original SVM algorithm to multiple classes is to build a one-
    ///   against-one scheme where multiple SVMs specialize to recognize each of the
    ///   available classes. By using a competition scheme, the original multi-class
    ///   classification problem is then reduced to <c>n*(n/2)</c> smaller binary problems.</para>
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
    [Serializable]
    public class MulticlassSupportVectorMachine<TKernel, TInput> :
        MulticlassSupportVectorMachine<
            SupportVectorMachine<TKernel, TInput>,
            TKernel, TInput>, ICloneable
        where TKernel : IKernel<TInput>
        where TInput : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MulticlassSupportVectorMachine{TKernel, TInput}"/> class.
        /// </summary>
        /// <param name="classes">The number of classes in the multi-class classification problem.</param>
        /// <param name="inputs">The number of inputs (length of the input vectors) accepted by the machine.</param>
        /// <param name="kernel">The kernel function to be used.</param>
        public MulticlassSupportVectorMachine(int inputs, TKernel kernel, int classes)
            : base(classes, () => new SupportVectorMachine<TKernel, TInput>(inputs, kernel))
        {
        }
    }

    /// <summary>
    ///   One-against-one Multi-class Kernel Support Vector Machine Classifier.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Support Vector Machine is by nature a binary classifier. One of the ways
    ///   to extend the original SVM algorithm to multiple classes is to build a one-
    ///   against-one scheme where multiple SVMs specialize to recognize each of the
    ///   available classes. By using a competition scheme, the original multi-class
    ///   classification problem is then reduced to <c>n*(n/2)</c> smaller binary problems.</para>
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
    [Serializable]
    public class MulticlassSupportVectorMachine<TModel, TKernel, TInput> :
        OneVsOne<TModel, TInput>,
        IDisposable, ICloneable
        where TKernel : IKernel<TInput>
        where TModel : SupportVectorMachine<TKernel, TInput>
        where TInput : ICloneable
    {

        // Multi-class statistics
        [NonSerialized]
        private int? totalVectorsCount;
        [NonSerialized]
        private int? uniqueVectorsCount;
        [NonSerialized]
        private int? sharedVectorsCount;


        // Performance optimizations
        [NonSerialized]
        private Lazy<int[][][]> sharedVectors;

        [NonSerialized]
        private ThreadLocal<Cache> vectorCache;

        /// <summary>
        ///   Gets or sets the kernel function used in all machines at once.
        /// </summary>
        /// 
        public TKernel Kernel
        {
            get { return Models[0][0].Kernel; }
            set
            {
                for (int i = 0; i < Models.Length; i++)
                    for (int j = 0; j < Models[i].Length; j++)
                        Models[i][j].Kernel = value;
            }
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
        /// Initializes a new instance of the <see cref="MulticlassSupportVectorMachine{TModel, TKernel, TInput}"/> class.
        /// </summary>
        /// <param name="classes">The number of classes in the multi-class classification problem.</param>
        /// <param name="initializer">A function to create the inner binary support vector machines.</param>
        public MulticlassSupportVectorMachine(int classes, Func<TModel> initializer)
            : base(classes, initializer)
        {
            initialize();
        }

        private void initialize()
        {
            this.vectorCache = new ThreadLocal<Cache>(() => new Cache());
            this.sharedVectors = new Lazy<int[][][]>(computeSharedVectors, true);
        }




        #region Properties


        /// <summary>
        ///   Gets the total number of support vectors
        ///   in the entire multi-class machine.
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
                        for (int j = 0; j < Models[i].Length; j++)
                            if (Models[i][j].SupportVectors != null)
                                count += Models[i][j].SupportVectors.Length;
                    totalVectorsCount = count;
                }

                return totalVectorsCount.Value;
            }
        }

        /// <summary>
        ///   Gets the number of unique support 
        ///   vectors in the multi-class machine.
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
                        for (int j = 0; j < Models[i].Length; j++)
                            if (Models[i][j].SupportVectors != null)
                                for (int k = 0; k < Models[i][j].SupportVectors.Length; k++)
                                    unique.Add(Models[i][j].SupportVectors[k]);

                    uniqueVectorsCount = unique.Count;
                }

                return uniqueVectorsCount.Value;
            }
        }

        /// <summary>
        ///   Gets the number of shared support
        ///   vectors in the multi-class machine.
        /// </summary>
        /// 
        public int SupportVectorSharedCount
        {
            get
            {
                if (sharedVectorsCount == null)
                    sharedVectors.Value.GetHashCode(); // force creation
                return sharedVectorsCount.Value;
            }
        }

        #endregion










        /// <summary>
        ///   Compute SVM output with support vector sharing.
        /// </summary>
        /// 
        private double distance(int a, int b, TInput input, Cache cache)
        {
            // Get the machine for this problem
            var machine = Models[a - 1][b];

            if (machine.SupportVectors.Length == 1)
            {
                // For linear machines, computation is simpler
                return machine.Threshold + machine.Weights[0] * machine.Kernel.Function(machine.SupportVectors[0], input);
            }

            // Get the vectors shared among all machines
            int[] sharedVectors = cache.Vectors[a - 1][b];
            double[] cachedValues = cache.Products;

            double sum = machine.Threshold;

            SpinLock[] locks = cache.SyncObjects;

            // For each support vector in the machine
            Parallel.For(0, sharedVectors.Length, ParallelOptions,

                // Init
                () => 0.0,

                // Map
                (i, state, partialSum) =>
                {
                    double value;

                    // Check if it is a shared vector
                    int j = sharedVectors[i];

                    if (j >= 0)
                    {
                        // This is a shared vector. Check
                        // if it has already been computed

                        bool taken = false;
                        locks[j].Enter(ref taken);

                        if (!Double.IsNaN(cachedValues[j]))
                        {
                            // Yes, it has. Retrieve the value from the cache
                            value = cachedValues[j];
                            Interlocked.Increment(ref cache.Hits);
                        }
                        else
                        {
                            // No, it has not. Compute and store the computed value in the cache
                            value = cachedValues[j] = machine.Kernel.Function(machine.SupportVectors[i], input);
                            Interlocked.Increment(ref cache.Evaluations);
                        }

                        locks[j].Exit();
                    }
                    else
                    {
                        // This vector is not shared by any other machine. No need to cache
                        value = machine.Kernel.Function(machine.SupportVectors[i], input);
                        Interlocked.Increment(ref cache.Evaluations);
                    }

                    return partialSum + machine.Weights[i] * value;
                },

                // Reduce
                (partialSum) => { lock (locks) sum += partialSum; }
            );

#if DEBUG
            double expected = machine.Distance(input);
            Accord.Diagnostics.Debug.Assert(sum.IsEqual(expected, rtol: 1e-3));
#endif

            return sum;
        }






        /// <summary>
        /// Computes a class-label decision for a given <paramref name="input" />.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// one of the <see cref="Accord.MachineLearning.ITransform.NumberOfOutputs" /> possible classes.</param>
        /// <returns>
        /// A class-label that best described <paramref name="input" /> according
        /// to this classifier.
        /// </returns>
        public override int Decide(TInput input)
        {
            // TODO: override the Decide(TInput[] input) overload instead,
            // and make the overriding of array input methods as the default
            // across the framework.
            Cache cache = createOrResetCache();

            int result;

            if (Method == MulticlassComputeMethod.Voting)
            {
                result = DecideByVoting(input, cache);
            }
            else
            {
                if (Track)
                    result = DecideByElimination(input, LastDecisionPath, cache);
                else
                    result = DecideByElimination(input, cache);
            }

#if DEBUG
            int expected = base.Decide(input);
            if (result != expected)
                throw new Exception();
#endif
            return result;
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and each class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the result will be stored,
        /// avoiding unnecessary memory allocations.</param>
        /// <returns></returns>
        public override double[] Distances(TInput input, double[] result)
        {
            Cache cache = createOrResetCache();

            if (Method == MulticlassComputeMethod.Voting)
            {
                DistanceByVoting(input, result, cache);
            }
            else
            {
                if (Track)
                    DistanceByElimination(input, result, LastDecisionPath, cache);
                else
                    DistanceByElimination(input, result, cache);
            }

#if DEBUG
            double[] expected = base.Distances(input, new double[NumberOfOutputs]);
            if (!result.IsEqual(expected, rtol: 1e-8))
                throw new Exception();
#endif
            return result;
        }




        private int DecideByVoting(TInput input, Cache cache)
        {
            return DistanceByVoting(input, new double[NumberOfOutputs], cache).ArgMax();
        }

        private int DecideByElimination(TInput input, Cache cache)
        {
            int i = NumberOfOutputs - 1;
            int j = 0;

            while (i != j)
            {
                if (Special.Decide(distance(i, j, input, cache)))
                    j++; // i won, so we advance j
                else
                    i--; // j won, so we advance i
            }

            return i;
        }

        private int DecideByElimination(TInput input, Decision[] path, Cache cache)
        {
            int i = NumberOfOutputs - 1;
            int j = 0;

            int k = 0;

            while (i != j)
            {
                if (Special.Decide(distance(i, j, input, cache)))
                {
                    path[k++] = new Decision(j, i, i);
                    j++; // i won, so we advance j
                }
                else
                {
                    path[k++] = new Decision(i, j, j);
                    i--; // j won, so we advance i
                }
            }

            return i;
        }


        private double[] DistanceByElimination(TInput input, double[] result, Cache cache)
        {
            int i = NumberOfOutputs - 1;
            int j = 0;
            double sum = 0;
            double max = Double.NegativeInfinity;

            while (i != j)
            {
                sum = distance(i, j, input, cache);
                bool decision = Special.Decide(sum);

                if (decision)
                {
                    result[j] = -sum;
                    j++;

                    if (sum > max)
                        max = sum;
                }
                else
                {
                    result[i] = sum;
                    i--;

                    if (-sum > max)
                        max = -sum;
                }
            }

            result[i] = max;
            return result;
        }

        private double[] DistanceByElimination(TInput input, double[] result, Decision[] path, Cache cache)
        {
            int i = NumberOfOutputs - 1;
            int j = 0;
            int k = 0;
            double sum = 0;
            double max = Double.NegativeInfinity;

            while (i != j)
            {
                sum = distance(i, j, input, cache);
                bool decision = Special.Decide(sum);

                if (decision)
                {
                    result[j] = -sum;
                    path[k++] = new Decision(j, i, i);
                    j++;

                    if (sum > max)
                        max = sum;
                }
                else
                {
                    result[i] = sum;
                    path[k++] = new Decision(i, j, j);
                    i--;

                    if (-sum > max)
                        max = -sum;
                }


            }

            result[i] = max;
            return result;
        }

        private double[] DistanceByVoting(TInput input, double[] result, Cache cache)
        {
            Parallel.For(0, Indices.Length, ParallelOptions, k =>
            {
                int i = Indices[k].Class1;
                int j = Indices[k].Class2;

                if (Special.Decide(distance(i, j, input, cache)))
                    InterlockedEx.Increment(ref result[i]);
                else InterlockedEx.Increment(ref result[j]);
            });

            return result;
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
        ///   Gets the total kernel evaluations performed in the last call
        ///   to <see cref="IClassifier{TInput, TClasses}.Decide(TInput)"/>
        ///   and similar functions in the current thread.
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
        ///   to <see cref="IClassifier{TInput, TClasses}.Decide(TInput)"/>
        ///   and similar functions in the current thread.
        /// </summary>
        ///
        /// <returns>The number of cache hits in the last decision.</returns>
        ///
        public int GetLastKernelHits()
        {
            return vectorCache.Value.Hits;
        }



        [OnDeserialized]
        private void onDeserialized(StreamingContext context)
        {
            initialize();
        }


        #region IDisposable members
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
        #endregion



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

                // Create synchronization objects
                cache.SyncObjects = new SpinLock[vectorCount];
                for (int i = 0; i < cache.SyncObjects.Length; i++)
                    cache.SyncObjects[i] = new SpinLock();
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


        private int[][][] computeSharedVectors()
        {
            // This method should only be called once after the machine has
            // been learned. If the inner machines or they Support Vectors
            // change, this method will need to be recomputed.

            // Detect all vectors which are being shared along the machines
            var shared = new Dictionary<TInput, List<Tuple<int, int, int>>>();

            // for all machines
            for (int i = 0; i < Models.Length; i++)
            {
                for (int j = 0; j < Models[i].Length; j++)
                {
                    // if the machine is not in compact form
                    if (Models[i][j].SupportVectors != null)
                    {
                        // register the support vector on the shared cache collection
                        for (int k = 0; k < Models[i][j].SupportVectors.Length; k++)
                        {
                            TInput sv = Models[i][j].SupportVectors[k];

                            List<Tuple<int, int, int>> count;
                            bool success = shared.TryGetValue(sv, out count);

                            if (success)
                            {
                                // Value is already in the dictionary
                                count.Add(Tuple.Create(i, j, k));
                            }
                            else
                            {
                                count = new List<Tuple<int, int, int>>();
                                count.Add(Tuple.Create(i, j, k));
                                shared[sv] = count;
                            }
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
            int[][][] sharedVectors = new int[Models.Length][][];
            for (int i = 0; i < sharedVectors.Length; i++)
            {
                sharedVectors[i] = new int[Models[i].Length][];
                for (int j = 0; j < sharedVectors[i].Length; j++)
                {
                    if (Models[i][j].SupportVectors != null)
                    {
                        sharedVectors[i][j] = new int[Models[i][j].SupportVectors.Length];

                        for (int k = 0; k < Models[i][j].SupportVectors.Length; k++)
                        {
                            TInput sv = Models[i][j].SupportVectors[k];
                            if (shared.ContainsKey(sv))
                                sharedVectors[i][j][k] = indices[sv];
                            else
                                sharedVectors[i][j][k] = -1;
                        }
                    }
                }
            }

            sharedVectorsCount = shared.Count;
            return sharedVectors;
        }

        private class Cache
        {
            public int Evaluations;
            public int Hits;
            public double[] Products;
            public int[][][] Vectors;
            public SpinLock[] SyncObjects;
        }

        #endregion

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public virtual object Clone()
        {
            var clone = new MulticlassSupportVectorMachine<TKernel, TInput>(NumberOfOutputs, Kernel, NumberOfInputs);
            clone.Method = Method;
            for (int i = 0; i < Models.Length; i++)
                for (int j = 0; j < Models[i].Length; j++)
                    clone.Models[i][j] = (TModel)Models[i][j].Clone();
            return clone;
        }
    }
}
