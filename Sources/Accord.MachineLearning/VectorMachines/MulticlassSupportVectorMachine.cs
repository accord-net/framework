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
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Threading.Tasks;
    using Accord.Math;
    using Accord.Statistics.Kernels;
    using System.IO.Compression;

    /// <summary>
    ///   Decision strategies for <see cref="MulticlassSupportVectorMachine">
    ///   Multi-class Support Vector Machines</see>.
    /// </summary>
    /// 
    public enum MulticlassComputeMethod
    {
        /// <summary>
        ///   Max-voting method (also known as 1vs1 decision).
        /// </summary>
        /// 
        Voting,

        /// <summary>
        ///   Elimination method (also known as DAG decision).
        /// </summary>
        /// 
        Elimination,
    }

    /// <summary>
    ///   One-against-one Multi-class Kernel Support Vector Machine Classifier.
    /// </summary>
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
    ///   // Output for each of the inputs
    ///   int[] outputs = { 0, 3, 1, 2 };
    ///   
    ///   
    ///   // Create a new Linear kernel
    ///   IKernel kernel = new Linear();
    ///   
    ///   // Create a new Multi-class Support Vector Machine with one input,
    ///   //  using the linear kernel and for four disjoint classes.
    ///   var machine = new MulticlassSupportVectorMachine(1, kernel, 4);
    ///   
    ///   // Create the Multi-class learning algorithm for the machine
    ///   var teacher = new MulticlassSupportVectorLearning(machine, inputs, outputs);
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
    /// <seealso cref="Learning.MulticlassSupportVectorLearning"/>
    ///
    [Serializable]
    public class MulticlassSupportVectorMachine : ISupportVectorMachine,
        IEnumerable<KeyValuePair<Tuple<int, int>, KernelSupportVectorMachine>>, IDisposable
    {

        // List of underlying binary classifiers
        private KernelSupportVectorMachine[][] machines;

        // Multi-class statistics
        private int? totalVectorsCount;
        private int? uniqueVectorsCount;
        private int? sharedVectorsCount;


        // Performance optimizations
        [NonSerialized]
        private Lazy<int[][][]> sharedVectors;

        [NonSerialized]
        private ThreadLocal<Cache> vectorCache;


        /// <summary>
        ///   Constructs a new Multi-class Kernel Support Vector Machine
        /// </summary>
        /// 
        /// <param name="inputs">The number of inputs for the machine. If sequences have
        ///   varying length, pass zero to this parameter and pass a suitable sequence 
        ///   kernel to this constructor, such as <see cref="DynamicTimeWarping"/>.</param>
        /// <param name="classes">The number of classes in the classification problem.</param>
        /// 
        /// <remarks>
        ///   If the number of inputs is zero, this means the machine
        ///   accepts a indefinite number of inputs. This is often the
        ///   case for kernel vector machines using a sequence kernel.
        /// </remarks>
        /// 
        public MulticlassSupportVectorMachine(int inputs, int classes)
            : this(inputs, new Linear(), classes)
        {
        }

        /// <summary>
        ///   Constructs a new Multi-class Kernel Support Vector Machine
        /// </summary>
        /// 
        /// <param name="kernel">The chosen kernel for the machine. Default is to
        ///   use the <see cref="Linear"/> kernel.</param>
        /// <param name="inputs">The number of inputs for the machine. If sequences have
        ///   varying length, pass zero to this parameter and pass a suitable sequence 
        ///   kernel to this constructor, such as <see cref="DynamicTimeWarping"/>.</param>
        /// <param name="classes">The number of classes in the classification problem.</param>
        /// 
        /// <remarks>
        ///   If the number of inputs is zero, this means the machine
        ///   accepts a indefinite number of inputs. This is often the
        ///   case for kernel vector machines using a sequence kernel.
        /// </remarks>
        /// 
        public MulticlassSupportVectorMachine(int inputs, IKernel kernel, int classes)
        {
            if (classes <= 1)
                throw new ArgumentException("The machine must have at least two classes.", "classes");

            // Create the kernel machines
            machines = new KernelSupportVectorMachine[classes - 1][];
            for (int i = 0; i < machines.Length; i++)
            {
                machines[i] = new KernelSupportVectorMachine[i + 1];

                for (int j = 0; j <= i; j++)
                    machines[i][j] = new KernelSupportVectorMachine(kernel, inputs);
            }

            this.initialize();
        }

        /// <summary>
        ///   Constructs a new Multi-class Kernel Support Vector Machine
        /// </summary>
        /// 
        /// <param name="machines">
        ///   The machines to be used in each of the pair-wise class subproblems.
        /// </param>
        /// 
        public MulticlassSupportVectorMachine(KernelSupportVectorMachine[][] machines)
        {
            if (machines == null)
                throw new ArgumentNullException("machines");

            this.machines = machines;
            this.initialize();
        }

        private void initialize()
        {
            this.vectorCache = new ThreadLocal<Cache>(() => new Cache());
            this.sharedVectors = new Lazy<int[][][]>(computeSharedVectors, true);
        }



    
        #region Properties
        /// <summary>
        ///   Gets the classifier for <paramref name="class1"/> against <paramref name="class2"/>.
        /// </summary>
        /// 
        /// <remarks>
        ///   If the index of <paramref name="class1"/> is greater than <paramref name="class2"/>,
        ///   the classifier for the <paramref name="class2"/> against <paramref name="class1"/>
        ///   will be returned instead. If both indices are equal, null will be
        ///   returned instead.
        /// </remarks>
        /// 
        public KernelSupportVectorMachine this[int class1, int class2]
        {
            get
            {
                if (class1 == class2)
                    return null;
                if (class1 > class2)
                    return machines[class1 - 1][class2];
                else
                    return machines[class2 - 1][class1];
            }
        }

        /// <summary>
        ///   Gets the total number of machines
        ///   in this multi-class classifier.
        /// </summary>
        /// 
        public int MachinesCount
        {
            get { return ((machines.Length + 1) * machines.Length) / 2; }
        }

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
                    for (int i = 0; i < machines.Length; i++)
                        for (int j = 0; j < machines[i].Length; j++)
                            if (machines[i][j].SupportVectors != null)
                                count += machines[i][j].SupportVectors.Length;
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
                    HashSet<double[]> unique = new HashSet<double[]>();
                    for (int i = 0; i < machines.Length; i++)
                    {
                        for (int j = 0; j < machines[i].Length; j++)
                        {
                            if (machines[i][j].SupportVectors != null)
                            {
                                for (int k = 0; k < machines[i][j].SupportVectors.Length; k++)
                                    unique.Add(machines[i][j].SupportVectors[k]);
                            }
                        }
                    }

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
            get { return machines.Length + 1; }
        }

        /// <summary>
        ///   Gets the number of inputs of the machines.
        /// </summary>
        /// 
        public int Inputs
        {
            get { return machines[0][0].Inputs; }
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
            get { return machines[0][0].IsProbabilistic; }
        }

        /// <summary>
        ///   Gets the subproblems classifiers.
        /// </summary>
        /// 
        public KernelSupportVectorMachine[][] Machines
        {
            get { return machines; }
        }
        #endregion


        #region Public Compute Overloads
        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        ///
        public int Compute(params double[] inputs)
        {
            double output; // Compute using elimination method as default.
            return Compute(inputs, MulticlassComputeMethod.Elimination, out output);
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="output">The output of the machine. If this is a 
        ///   <see cref="IsProbabilistic">probabilistic</see> machine, the
        ///   output is the probability of the positive class. If this is
        ///   a standard machine, the output is the distance to the decision
        ///   hyperplane in feature space.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        /// 
        /// 
        public int Compute(double[] inputs, out double output)
        {
            // Compute using elimination method as default.
            return Compute(inputs, MulticlassComputeMethod.Elimination, out output);
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="output">The output of the machine. If this is a 
        ///   <see cref="IsProbabilistic">probabilistic</see> machine, the
        ///   output is the probability of the positive class. If this is
        ///   a standard machine, the output is the distance to the decision
        ///   hyperplane in feature space.</param>
        /// <param name="decisionPath">The decision path followed by the Decision
        /// Directed Acyclic Graph used by the <see cref="MulticlassComputeMethod.Elimination">
        /// elimination</see> method.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        /// 
        /// 
        public int Compute(double[] inputs, out double output, out Tuple<int, int>[] decisionPath)
        {
            double[] responses;
            decisionPath = new Tuple<int, int>[Classes - 1];
            return computeElimination(inputs, out responses, out output, decisionPath);
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="responses">The model response for each class.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        /// 
        public int Compute(double[] inputs, out double[] responses)
        {
            double output; // Compute using elimination method as default.
            return Compute(inputs, MulticlassComputeMethod.Elimination, out responses, out output);
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="method">The <see cref="MulticlassComputeMethod">
        ///   multi-class classification method</see> to use.</param>
        /// <param name="responses">The model response for each class.</param>
        /// <param name="output">The output of the machine. If this is a 
        ///   <see cref="IsProbabilistic">probabilistic</see> machine, the
        ///   output is the probability of the positive class. If this is
        ///   a standard machine, the output is the distance to the decision
        ///   hyperplane in feature space.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        /// 
        public int Compute(double[] inputs, MulticlassComputeMethod method, out double[] responses, out double output)
        {
            if (method == MulticlassComputeMethod.Voting)
            {
                int[] votes;
                int result = computeVoting(inputs, out votes, out output);

                responses = new double[votes.Length];
                for (int i = 0; i < responses.Length; i++)
                    responses[i] = votes[i] * (2.0 / (Classes * (Classes - 1)));

                return result;
            }
            else
            {
                return computeElimination(inputs, out responses, out output, null);
            }
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="method">The <see cref="MulticlassComputeMethod">
        ///   multi-class classification method</see> to use.</param>
        /// <param name="responses">The model response for each class.</param>
        /// 
        /// <returns>The class decision for the given input.</returns>
        /// 
        public int Compute(double[] inputs, MulticlassComputeMethod method, out double[] responses)
        {
            double output;
            return Compute(inputs, method, out responses, out output);
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="method">The <see cref="MulticlassComputeMethod">
        ///   multi-class classification method</see> to use.</param>
        /// <param name="output">The output of the machine. If this is a 
        ///   <see cref="IsProbabilistic">probabilistic</see> machine, the
        ///   output is the probability of the positive class. If this is
        ///   a standard machine, the output is the distance to the decision
        ///   hyperplane in feature space.</param>
        /// 
        /// <returns>The class decision for the given input.</returns>
        ///
        public int Compute(double[] inputs, MulticlassComputeMethod method, out double output)
        {
            if (method == MulticlassComputeMethod.Voting)
            {
                int[] votes;
                return computeVoting(inputs, out votes, out output);
            }
            else
            {
                double[] responses;
                return computeElimination(inputs, out responses, out output, null);
            }
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="method">The <see cref="MulticlassComputeMethod">
        ///   multi-class classification method</see> to use.</param>
        /// 
        /// <returns>The class decision for the given input.</returns>
        ///
        public int Compute(double[] inputs, MulticlassComputeMethod method)
        {
            double output;
            return Compute(inputs, method, out output);
        }
        #endregion


        #region Private Multi-class Decision
        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="votes">A vector containing the number of votes for each class.</param>
        /// <param name="output">The output of the machine. If this is a 
        ///   <see cref="IsProbabilistic">probabilistic</see> machine, the
        ///   output is the probability of the positive class. If this is
        ///   a standard machine, the output is the distance to the decision
        ///   hyperplane in feature space.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        /// 
        private int computeVoting(double[] inputs, out int[] votes, out double output)
        {
            // Compute decision by Voting

            // Get a list of the shared vectors (lazy)
            int[][][] vectors = this.sharedVectors.Value;

            // Get the cache for this thread
            Cache cache = createOrResetCache();

            // out variables cannot be passed into delegates,
            // so will be creating a copy for the vote array.
            int[] voting = new int[Classes];

            // For each class
            Parallel.For(0, Classes, i =>
            {
                // For each other class
                for (int j = 0; j < i; j++)
                {
                    double machineOutput;

                    // Retrieve and compute the two-class problem for classes i x j
                    int answer = computeSequential(i, j, inputs, out machineOutput, cache);

                    // Determine the winner class
                    int y = (answer == -1) ? i : j;

                    // Increment votes for the winner
                    Interlocked.Increment(ref voting[y]);
                }
            });

            // Voting finished.
            votes = voting;

            // Select class which maximum number of votes
            int imax; output = Matrix.Max(votes, out imax);

            // Determine output probability using no. of votes
            output = output * (2.0 / (Classes * (Classes - 1)));

            return imax; // Return the winner as the output.
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <remarks>
        ///   This method computes the decision for a one-against-one multiclass
        ///   support vector machine using the Directed Acyclic Graph method by
        ///   Platt, Cristianini and Shawe-Taylor. Details are given on the 
        ///   original paper "Large Margin DAGs for Multiclass Classification", 2000.
        /// </remarks>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="responses">The model response for each class.</param>
        /// <param name="output">The output of the machine. If this is a 
        ///   <see cref="IsProbabilistic">probabilistic</see> machine, the
        ///   output is the probability of the positive class. If this is
        ///   a standard machine, the output is the distance to the decision
        ///   hyperplane in feature space.</param>
        /// <param name="decisionPath">The decision path followed by the Decision
        /// Directed Acyclic Graph used by the <see cref="MulticlassComputeMethod.Elimination">
        /// elimination</see> method.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        /// 
        private int computeElimination(double[] inputs, out double[] responses, 
            out double output, Tuple<int, int>[] decisionPath)
        {
            // Compute decision by Directed Acyclic Graph

            // Get a list of the shared vectors
            int[][][] vectors = this.sharedVectors.Value;

            // Get the cache for this thread
            Cache cache = createOrResetCache();

            output = 0;

            // Initialize metrics
            responses = new double[Classes];
            bool probabilistic = IsProbabilistic;

            if (probabilistic)
            {
                for (int i = 0; i < responses.Length; i++)
                    responses[i] = 1.0;
            }

            // Start with first and last classes
            int classA = Classes - 1, classB = 0;
            int progress = 0;

            // Navigate decision path
            while (classA != classB)
            {
                
                // Compute the two-class decision problem to decide for A x B
                int answer = computeParallel(classA, classB, inputs, out output, cache);

                if (decisionPath != null)
                    decisionPath[progress++] = Tuple.Create(classA, classB);

                // Check who won and update

                if (answer == -1)
                {
                    // The class A has won and class B has lost

                    if (probabilistic)
                    {
                        // Decrease loser likelihood
                        responses[classB] *= output;

                        // Increase for all other classes
                        for (int i = 0; i < responses.Length; i++)
                            if (i != classB) responses[i] *= 1.0 - output;
                    }
                    else
                    {
                        // Store the distance to the
                        // answer for the loser class
                        responses[classB] = -output;
                    }

                    // Advance classB towards
                    // the middle of the list
                    classB++;
                }

                else // answer == +1
                {
                    // The class A has lost and class B has won

                    if (probabilistic)
                    {
                        // Decrease loser likelihood
                        responses[classA] *= 1.0 - output;

                        // Increase for all other classes
                        for (int i = 0; i < responses.Length; i++)
                            if (i != classA) responses[i] *= output;
                    }
                    else
                    {
                        // Store the distance to the
                        // answer for the loser class
                        responses[classA] = output;
                    }

                    // Advance classA towards
                    // the middle of the list
                    classA--;
                }
            }

            // At this point, classA = classB is the winner
            if (!probabilistic) responses[classA] = output;

#if DEBUG
            else
            {
                int imax; responses.Max(out imax);
                if (imax != classA) throw new Exception();
            }
#endif

            // Return output for winner class
            output = responses[classA];

            return classA;
        }

        #endregion


        #region Private Single machine Decision
        /// <summary>
        ///   Compute SVM output with support vector sharing.
        /// </summary>
        /// 
        private int computeSequential(int classA, int classB, double[] input, out double output, Cache cache)
        {
            // Get the machine for this problem
            KernelSupportVectorMachine machine = machines[classA - 1][classB];

            // Get the vectors shared among all machines
            int[] vectors = cache.Vectors[classA - 1][classB];
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
        ///   Compute SVM output with support vector sharing.
        /// </summary>
        /// 
        private int computeParallel(int classA, int classB, double[] input, out double output, Cache cache)
        {
            // Get the machine for this problem
            KernelSupportVectorMachine machine = machines[classA - 1][classB];

            // Get the vectors shared among all machines
            int[] vectors = cache.Vectors[classA - 1][classB];

            double[] values = cache.Products;
#if !NET35
            SpinLock[] locks = cache.SyncObjects;
#endif
            double sum = machine.Threshold;


            if (machine.IsCompact)
            {
                // For linear machines, computation is simpler
                for (int i = 0; i < machine.Weights.Length; i++)
                    sum += machine.Weights[i] * input[i];
            }
            else
            {

#if NET35
                #region Backward compatibility
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
                #endregion
#else
                // For each support vector in the machine
                Parallel.For<double>(0, vectors.Length,

                    // Init
                    () => 0.0,

                    // Map
                    (i, state, partialSum) =>
                    {
                        double value;

                        // Check if it is a shared vector
                        int j = vectors[i];

                        if (j >= 0)
                        {
                            // This is a shared vector. Check
                            // if it has already been computed

                            bool taken = false;
                            locks[j].Enter(ref taken);

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
#endif
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
        #endregion





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
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                Save(fs);
            }
        }

        /// <summary>
        ///   Saves the machine to a zipped stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to which the machine is to be serialized.</param>
        /// 
        private void SaveCompressed(Stream stream)
        {
            using (var compressionStream = new GZipStream(stream, CompressionMode.Compress))
            {
                Save(compressionStream);
            }
        }

        /// <summary>
        ///   Saves the machine to a zipped file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file to which the machine is to be serialized.</param>
        /// 
        public void SaveCompressed(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                using (var compressionStream = new GZipStream(fs, CompressionMode.Compress))
                {
                    Save(compressionStream);
                }
            }
        }

        /// <summary>
        ///   Loads a machine from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the machine is to be deserialized.</param>
        /// 
        /// <returns>The deserialized machine.</returns>
        /// 
        public static MulticlassSupportVectorMachine Load(Stream stream)
        {
            object data = null;
            BinaryFormatter b = new BinaryFormatter();
            byte[] buffer = new byte[2];

            // Read first two bytes to test for compressed stream.
            stream.Read(buffer, 0, 2);

            // Reset stream position.
            stream.Seek(0, SeekOrigin.Begin);

            // Test for compression.
            if ((buffer[0] == 0x1f) && (buffer[1] == 0x8b))
            {
                // Compressed stream.
                data = LoadCompressed(stream);
            }
            else
            {
                data = b.Deserialize(stream);
            }

            return (MulticlassSupportVectorMachine)data;
        }

        /// <summary>
        ///   Loads a machine from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the machine is to be deserialized.</param>
        /// 
        /// <returns>The deserialized machine.</returns>
        /// 
        public static MulticlassSupportVectorMachine Load(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                return Load(fs);
            }
        }

        /// <summary>
        ///   Loads a machine from a zipped stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the machine is to be deserialized.</param>
        /// 
        /// <returns>The deserialized machine.</returns>
        /// 
        private static MulticlassSupportVectorMachine LoadCompressed(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();

            using (var compressionStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                return (MulticlassSupportVectorMachine)b.Deserialize(compressionStream);
            }
        }

        [OnDeserialized]
        private void onDeserialized(StreamingContext context)
        {
            initialize();
        }
        #endregion

        #region IEnumerable members
        /// <summary>
        ///   Returns an enumerator that iterates through all machines
        ///   contained inside this multi-class support vector machine.
        /// </summary>
        /// 
        public IEnumerator<KeyValuePair<Tuple<int, int>, KernelSupportVectorMachine>> GetEnumerator()
        {
            for (int i = 0; i < machines.Length; i++)
            {
                for (int j = 0; j < machines[i].Length; j++)
                {
                    yield return new KeyValuePair<Tuple<int, int>, KernelSupportVectorMachine>(
                        Tuple.Create(i + 1, j), machines[i][j]);
                }
            }
        }

        /// <summary>
        ///   Returns an enumerator that iterates through all machines
        ///   contained inside this multi-class support vector machine.
        /// </summary>
        /// 
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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


        private int[][][] computeSharedVectors()
        {
            // This method should only be called once after the machine has
            // been learned. If the inner machines or they Support Vectors
            // change, this method will need to be recomputed.

            // Detect all vectors which are being shared along the machines
            var shared = new Dictionary<double[], List<Tuple<int, int, int>>>();

            // for all machines
            for (int i = 0; i < machines.Length; i++)
            {
                for (int j = 0; j < machines[i].Length; j++)
                {
                    // if the machine is not in compact form
                    if (machines[i][j].SupportVectors != null)
                    {
                        // register the support vector on the shared cache collection
                        for (int k = 0; k < machines[i][j].SupportVectors.Length; k++)
                        {
                            double[] sv = machines[i][j].SupportVectors[k];

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

            var indices = new Dictionary<double[], int>();
            foreach (double[] sv in shared.Keys)
                indices[sv] = idx++;

            // Create a lookup table for the machines
            int[][][] sharedVectors = new int[machines.Length][][];
            for (int i = 0; i < sharedVectors.Length; i++)
            {
                sharedVectors[i] = new int[machines[i].Length][];
                for (int j = 0; j < sharedVectors[i].Length; j++)
                {
                    if (machines[i][j].SupportVectors != null)
                    {
                        sharedVectors[i][j] = new int[machines[i][j].SupportVectors.Length];

                        for (int k = 0; k < machines[i][j].SupportVectors.Length; k++)
                        {
                            double[] sv = machines[i][j].SupportVectors[k];
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
            public double[] Products;
            public int[][][] Vectors;
#if !NET35
            public SpinLock[] SyncObjects;
#endif
        }

        #endregion

    }
}
