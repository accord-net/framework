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

namespace Accord.MachineLearning.VectorMachines
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading.Tasks;
    using Accord.Math;
    using Accord.Statistics.Kernels;
    using System.Collections.Generic;
    using System.Threading;
    using System.Runtime.Serialization;


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
    ///   <code>
    ///   // Sample data
    ///   //   The following is simple auto association function
    ///   //   where each input correspond to its own class. This
    ///   //   problem should be easily solved by a Linear kernel.
    ///
    ///   // Sample input data
    ///   double[][] inputs =
    ///   {
    ///       new double[] { 0 },
    ///       new double[] { 3 },
    ///       new double[] { 1 },
    ///       new double[] { 2 },
    ///   };
    ///   
    ///   // Outputs for each of the inputs
    ///   int[][] outputs =
    ///   {
    ///       new[] { -1,  1, -1 },
    ///       new[] { -1, -1,  1 },
    ///       new[] {  1,  1, -1 },
    ///       new[] { -1, -1, -1 },
    ///   };
    ///   
    ///   
    ///   // Create a new Linear kernel
    ///   IKernel kernel = new Linear();
    ///   
    ///   // Create a new Multi-class Support Vector Machine with one input,
    ///   //  using the linear kernel and for four disjoint classes.
    ///   var machine = new MultilabelSupportVectorMachine(1, kernel, 3);
    ///   
    ///   // Create the Multi-label learning algorithm for the machine
    ///   var teacher = new MultilabelSupportVectorLearning(machine, inputs, outputs);
    ///   
    ///   // Configure the learning algorithm to use SMO to train the
    ///   //  underlying SVMs in each of the binary class subproblems.
    ///   teacher.Algorithm = (svm, classInputs, classOutputs, i, j) =>
    ///       new SequentialMinimalOptimization(svm, classInputs, classOutputs);
    ///   
    ///   // Run the learning algorithm
    ///   double error = teacher.Run();
    ///   </code>
    /// </example>
    ///
    /// <seealso cref="Learning.MultilabelSupportVectorLearning"/>
    ///
    [Serializable]
    public class MultilabelSupportVectorMachine : ISupportVectorMachine,
        IEnumerable<KernelSupportVectorMachine>, IDisposable
    {

        // Underlying classifiers
        private KernelSupportVectorMachine[] machines;

        // Multi-label statistics
        private int? totalVectorsCount;
        private int? uniqueVectorsCount;
        private int? sharedVectorsCount;


        // Performance optimizations
        [NonSerialized]
        private Lazy<int[][]> sharedVectors;

        [NonSerialized]
        private ThreadLocal<Cache> vectorCache;


        /// <summary>
        ///   Constructs a new Multi-label Kernel Support Vector Machine
        /// </summary>
        /// 
        /// <param name="kernel">The chosen kernel for the machine.</param>
        /// <param name="inputs">The number of inputs for the machine.</param>
        /// <param name="classes">The number of classes in the classification problem.</param>
        /// 
        /// <remarks>
        ///   If the number of inputs is zero, this means the machine
        ///   accepts a indefinite number of inputs. This is often the
        ///   case for kernel vector machines using a sequence kernel.
        /// </remarks>
        /// 
        public MultilabelSupportVectorMachine(int inputs, IKernel kernel, int classes)
        {
            if (classes <= 1)
                throw new ArgumentException("The machine must have at least two classes.", "classes");

            // Create the kernel machines
            machines = new KernelSupportVectorMachine[classes];
            for (int i = 0; i < machines.Length; i++)
                machines[i] = new KernelSupportVectorMachine(kernel, inputs);

            initialize();
        }

        /// <summary>
        ///   Constructs a new Multi-label Kernel Support Vector Machine
        /// </summary>
        /// 
        /// <param name="machines">
        ///   The machines to be used for each class.
        /// </param>
        /// 
        public MultilabelSupportVectorMachine(KernelSupportVectorMachine[] machines)
        {
            if (machines == null) throw new ArgumentNullException("machines");

            this.machines = machines;
            initialize();
        }

        private void initialize()
        {
            this.vectorCache = new ThreadLocal<Cache>(() => new Cache());
            this.sharedVectors = new Lazy<int[][]>(computeSharedVectors, true);
        }



        #region Properties
        /// <summary>
        ///   Gets the classifier for class <paramref name="index"/>.
        /// </summary>
        /// 
        public KernelSupportVectorMachine this[int index]
        {
            get { return machines[index]; }
        }

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
                    for (int i = 0; i < machines.Length; i++)
                        if (machines[i].SupportVectors != null)
                            count += machines[i].SupportVectors.Length;
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
                    HashSet<double[]> unique = new HashSet<double[]>();
                    for (int i = 0; i < machines.Length; i++)
                    {
                        if (machines[i].SupportVectors != null)
                        {
                            for (int k = 0; k < machines[i].SupportVectors.Length; k++)
                                unique.Add(machines[i].SupportVectors[k]);
                        }
                    }

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

        /// <summary>
        ///   Gets the number of classes.
        /// </summary>
        /// 
        public int Classes
        {
            get { return machines.Length; }
        }

        /// <summary>
        ///   Gets the number of inputs of the machines.
        /// </summary>
        /// 
        public int Inputs
        {
            get { return machines[0].Inputs; }
        }

        /// <summary>
        ///   Gets a value indicating whether this machine produces probabilistic outputs.
        /// </summary>
        /// 
        /// <value>
        ///   <c>true</c> if this machine produces probabilistic outputs; otherwise, <c>false</c>.
        /// </value>
        /// 
        public bool IsProbabilistic
        {
            get { return machines[0].IsProbabilistic; }
        }

        /// <summary>
        ///   Gets the subproblems classifiers.
        /// </summary>
        /// 
        public KernelSupportVectorMachine[] Machines
        {
            get { return machines; }
        }
        #endregion




        /// <summary>
        ///   Computes the given input to produce the corresponding outputs.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="responses">The model response for each class.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        /// 
        public int[] Compute(double[] inputs, out double[] responses)
        {
            // Get a list of the shared vectors (lazy)
            int[][] vectors = this.sharedVectors.Value;

            // Get the cache for this thread
            Cache cache = createOrResetCache();

            int[] labels = new int[machines.Length];
            double[] outputs = new double[machines.Length];

            // For each machine
            Parallel.For(0, machines.Length, i =>
            {
                labels[i] = computeSequential(i, inputs, out outputs[i], cache);
            });

            responses = outputs;

            return labels;
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding outputs.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        /// 
        public int[] Compute(double[] inputs)
        {
            double[] responses;
            return Compute(inputs, out responses);
        }


        /// <summary>
        ///   Compute SVM output with support vector sharing.
        /// </summary>
        /// 
        private int computeSequential(int index, double[] input, out double output, Cache cache)
        {
            // Get the machine for this problem
            KernelSupportVectorMachine machine = machines[index];

            // Get the vectors shared among all machines
            int[] vectors = cache.Vectors[index];
            double[] values = cache.Products;

            double sum = machine.Threshold;


            if (machine.IsCompact)
            {
                // For linear machines, computation is simpler
                for (int i = 0; i < machine.Weights.Length; i++)
                    sum += machine.Weights[i] * input[i];
            }
            else
            {
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
            }

            // Produce probabilities if required
            if (machine.IsProbabilistic)
            {
                output = machine.Link.Inverse(sum);
                return output >= 0.5 ? +1 : -1;
            }
            else
            {
                output = sum;
                return output >= 0 ? +1 : -1;
            }
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





        #region Loading & Saving
        /// <summary>
        ///   Saves the machine to a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to which the machine is to be serialized.</param>
        /// 
        public void Save(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(stream, this);
        }

        /// <summary>
        ///   Saves the machine to a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file to which the machine is to be serialized.</param>
        /// 
        public void Save(string path)
        {
            Save(new FileStream(path, FileMode.Create));
        }

        /// <summary>
        ///   Loads a machine from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the machine is to be deserialized.</param>
        /// 
        /// <returns>The deserialized machine.</returns>
        /// 
        public static MultilabelSupportVectorMachine Load(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            return (MultilabelSupportVectorMachine)b.Deserialize(stream);
        }

        /// <summary>
        ///   Loads a machine from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the machine is to be deserialized.</param>
        /// 
        /// <returns>The deserialized machine.</returns>
        /// 
        public static MultilabelSupportVectorMachine Load(string path)
        {
            return Load(new FileStream(path, FileMode.Open));
        }

        [OnDeserialized]
        private void onDeserialized(StreamingContext context)
        {
            initialize();
        }
        #endregion

        #region ISupportVectorMachine Members

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="output">The output for the given input.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        /// 
        int ISupportVectorMachine.Compute(double[] inputs, out double output)
        {
            double[] responses;
            Compute(inputs, out responses);

            int imax;
            output = responses.Max(out imax);
            return imax;
        }

        #endregion

        #region IEnumerable members
        /// <summary>
        ///   Returns an enumerator that iterates through all machines in the classifier.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// 
        public IEnumerator<KernelSupportVectorMachine> GetEnumerator()
        {
            return (machines as IEnumerable<KernelSupportVectorMachine>).GetEnumerator();
        }

        /// <summary>
        ///   Returns an enumerator that iterates through all machines in the classifier.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// 
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return machines.GetEnumerator();
        }
        #endregion

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

            return cache;
        }


        private int[][] computeSharedVectors()
        {
            // This method should only be called once after the machine has
            // been learned. If the inner machines or they Support Vectors
            // change, this method will need to be recomputed.

            // Detect all vectors which are being shared along the machines
            var shared = new Dictionary<double[], List<Tuple<int, int>>>();

            // for all machines
            for (int i = 0; i < machines.Length; i++)
            {
                // if the machine is not in compact form
                if (machines[i].SupportVectors != null)
                {
                    // register the support vector on the shared cache collection
                    for (int k = 0; k < machines[i].SupportVectors.Length; k++)
                    {
                        double[] sv = machines[i].SupportVectors[k];

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

            var indices = new Dictionary<double[], int>();
            foreach (double[] sv in shared.Keys)
                indices[sv] = idx++;

            // Create a lookup table for the machines
            int[][] sharedVectors = new int[machines.Length][];
            for (int i = 0; i < sharedVectors.Length; i++)
            {
                if (machines[i].SupportVectors != null)
                {
                    sharedVectors[i] = new int[machines[i].SupportVectors.Length];

                    for (int k = 0; k < machines[i].SupportVectors.Length; k++)
                    {
                        double[] sv = machines[i].SupportVectors[k];
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
        ///   Gets the total kernel evaluations performed
        ///   in the last call to any of the <see cref="Compute(double[])"/>
        ///   functions in the current thread.
        /// </summary>
        /// 
        /// <returns>The number of total kernel evaluations.</returns>
        /// 
        public int GetLastKernelEvaluations()
        {
            return vectorCache.Value.Evaluations;
        }

        private class Cache
        {
            public int Evaluations;
            public double[] Products;
            public int[][] Vectors;
#if !NET35
            public SpinLock[] SyncObjects;
#endif
        }
        #endregion

    }
}
