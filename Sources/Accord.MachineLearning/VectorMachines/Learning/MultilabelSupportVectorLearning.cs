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
    using Accord.Math;
    using System.Threading.Tasks;
    using System.Threading;


    /// <summary>
    ///   One-against-all Multi-label Support Vector Machine Learning Algorithm
    /// </summary>
    /// 
    /// <remarks>
    ///   This class can be used to train Kernel Support Vector Machines with
    ///   any algorithm using a one-against-all strategy. The underlying training
    ///   algorithm can be configured by defining the Configure delegate.
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
    ///       new[] { 0, 1, 0 }
    ///       new[] { 0, 0, 1 }
    ///       new[] { 1, 1, 0 }
    ///   }
    ///   
    ///   
    ///   // Create a new Linear kernel
    ///   IKernel kernel = new Linear();
    ///   
    ///   // Create a new Multi-class Support Vector Machine with one input,
    ///   //  using the linear kernel and for four disjoint classes.
    ///   var machine = new MultilabelSupportVectorMachine(1, kernel, 4);
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
    public class MultilabelSupportVectorLearning : ISupportVectorMachineLearning
    {
        // Training data
        private double[][] inputs;
        private int[][] outputs;

        // Machine
        private MultilabelSupportVectorMachine msvm;

        // Training configuration function
        private SupportVectorMachineLearningConfigurationFunction configure;


        /// <summary>
        ///   Occurs when the learning of a subproblem has started.
        /// </summary>
        /// 
        public event EventHandler<SubproblemEventArgs> SubproblemStarted;

        /// <summary>
        ///   Occurs when the learning of a subproblem has finished.
        /// </summary>
        /// 
        public event EventHandler<SubproblemEventArgs> SubproblemFinished;

        /// <summary>
        ///   Constructs a new Multi-label Support Vector Learning algorithm.
        /// </summary>
        /// 
        /// <param name="inputs">The input learning vectors for the machine learning algorithm.</param>
        /// <param name="machine">The <see cref="MulticlassSupportVectorMachine"/> to be trained.</param>
        /// <param name="outputs">The output labels associated with each of the input vectors. The
        /// class labels should be between 0 and the <see cref="MultilabelSupportVectorMachine.Classes">
        /// number of classes in the multiclass machine</see>. In a multi-label SVM, multiple classes
        /// can be associated with a single input vector.</param>
        /// 
        public MultilabelSupportVectorLearning(MultilabelSupportVectorMachine machine,
            double[][] inputs, int[] outputs)
        {
            // Initial argument checking
            if (machine == null) throw new ArgumentNullException("machine");
            if (inputs == null) throw new ArgumentNullException("inputs");
            if (outputs == null) throw new ArgumentNullException("outputs");
            if (inputs.Length != outputs.Length)
                throw new DimensionMismatchException("outputs", "The number of inputs and outputs does not match.");

            int classes = machine.Classes;
            int[][] expanded = new int[outputs.Length][];
            for (int i = 0; i < expanded.Length; i++)
            {
                int c = outputs[i];
                if (c < 0 || c >= classes)
                {
                    throw new ArgumentOutOfRangeException("outputs",
                        "Output labels should be either positive and less than the number of classes" +
                        " at the machine. The output at position " + i + " violates this criteria.");
                }

                int[] row = expanded[i] = new int[classes];
                for (int j = 0; j < row.Length; j++)
                    row[j] = -1;
                row[c] = 1;
            }

            initialize(machine, inputs, expanded);
        }

        /// <summary>
        ///   Constructs a new Multi-label Support Vector Learning algorithm.
        /// </summary>
        /// 
        /// <param name="inputs">The input learning vectors for the machine learning algorithm.</param>
        /// <param name="machine">The <see cref="MulticlassSupportVectorMachine"/> to be trained.</param>
        /// <param name="outputs">The output labels associated with each of the input vectors. The
        /// class labels should be between 0 and the <see cref="MultilabelSupportVectorMachine.Classes">
        /// number of classes in the multiclass machine</see>. In a multi-label SVM, multiple classes
        /// can be associated with a single input vector.</param>
        /// 
        public MultilabelSupportVectorLearning(MultilabelSupportVectorMachine machine,
            double[][] inputs, int[][] outputs)
        {
            // Initial argument checking
            if (machine == null) throw new ArgumentNullException("machine");
            if (inputs == null) throw new ArgumentNullException("inputs");
            if (outputs == null) throw new ArgumentNullException("outputs");
            if (inputs.Length != outputs.Length)
                throw new DimensionMismatchException("outputs", "The number of inputs and outputs does not match.");

            initialize(machine, inputs, outputs);
        }

        private void initialize(MultilabelSupportVectorMachine machine, double[][] inputs, int[][] outputs)
        {
            if (machine.Inputs > 0)
            {
                // This machine has a fixed input vector size
                for (int i = 0; i < inputs.Length; i++)
                {
                    if (inputs[i].Length != machine.Inputs)
                    {
                        throw new DimensionMismatchException("inputs",
                            "The size of the input vector at index " + i
                            + " does not match the expected number of inputs of the machine."
                            + " All input vectors for this machine must have length " + machine.Inputs);
                    }
                }
            }

            for (int i = 0; i < outputs.Length; i++)
            {
                if (outputs[i].Length != machine.Classes)
                {
                    throw new DimensionMismatchException("outputs",
                        "Output vectors should have the same length as there are classes in the multi-label" +
                        " machine. The output vector at position " + i + " has a different length.");
                }

                for (int j = 0; j < outputs[i].Length; j++)
                {
                    if (outputs[i][j] != -1 && outputs[i][j] != 1)
                    {
                        throw new ArgumentOutOfRangeException("outputs",
                            "Output values should be either -1 or +1. The output at index " + j +
                            " of the output vector at position " + i + " violates this criteria.");
                    }
                }
            }


            // Machine
            this.msvm = machine;

            // Learning data
            this.inputs = inputs;
            this.outputs = outputs;
        }

        /// <summary>
        ///   Gets or sets the configuration function for the learning algorithm.
        /// </summary>
        /// 
        /// <remarks>
        ///   The configuration function should return a properly configured ISupportVectorMachineLearning
        ///   algorithm using the given support vector machine and the input and output data.
        /// </remarks>
        /// 
        public SupportVectorMachineLearningConfigurationFunction Algorithm
        {
            get { return configure; }
            set { configure = value; }
        }

        /// <summary>
        ///   Runs the one-against-one learning algorithm.
        /// </summary>
        /// 
        public double Run()
        {
            return Run(true);
        }

        /// <summary>
        ///   Runs the one-against-one learning algorithm.
        /// </summary>
        /// 
        /// <param name="computeError">
        ///   True to compute error after the training
        ///   process completes, false otherwise. Default is true.
        /// </param>
        /// 
        /// <returns>
        ///   The sum of squares error rate for
        ///   the resulting support vector machine.
        /// </returns>
        /// 
        public double Run(bool computeError)
        {
            int total = msvm.Machines.Length;
            int progress = 0;

            // For each class i
            Parallel.For(0, msvm.Machines.Length, i =>
            {
                // We will start the binary sub-problem
                var args = new SubproblemEventArgs(i, -i);
                OnSubproblemStarted(args);

                // Retrieve the associated machine
                KernelSupportVectorMachine machine = msvm.Machines[i];

                // Extract outputs for the given label
                int[] subOutputs = outputs.GetColumn(i);

                // Train the machine on the two-class problem.
                configure(machine, inputs, subOutputs, i, -i).Run(false);


                // Update and report progress
                args.Progress = Interlocked.Increment(ref progress);
                args.Maximum = total;

                OnSubproblemFinished(args);
            });

            // Compute error if required.
            return (computeError) ? ComputeError(inputs, outputs) : 0.0;
        }

        /// <summary>
        ///   Compute the error ratio.
        /// </summary>
        /// 
        public double ComputeError(double[][] inputs, int[][] expectedOutputs)
        {
            // Compute errors
            int count = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                int[] actual = msvm.Compute(inputs[i]);
                int[] expected = expectedOutputs[i];

                for (int j = 0; j < actual.Length; j++)
                {
                    if (actual[j] != expected[j])
                        Interlocked.Increment(ref count);
                }
            }

            // Return misclassification error ratio
            return count / (double)(inputs.Length * msvm.Classes);
        }


        /// <summary>
        ///   Raises the <see cref="E:SubproblemFinished"/> event.
        /// </summary>
        /// 
        /// <param name="args">The <see cref="Accord.MachineLearning.VectorMachines.Learning.SubproblemEventArgs"/> instance containing the event data.</param>
        /// 
        protected void OnSubproblemFinished(SubproblemEventArgs args)
        {
            if (SubproblemFinished != null)
                SubproblemFinished(this, args);
        }

        /// <summary>
        ///   Raises the <see cref="E:SubproblemStarted"/> event.
        /// </summary>
        /// 
        /// <param name="args">The <see cref="Accord.MachineLearning.VectorMachines.Learning.SubproblemEventArgs"/> instance containing the event data.</param>
        /// 
        protected void OnSubproblemStarted(SubproblemEventArgs args)
        {
            if (SubproblemStarted != null)
                SubproblemStarted(this, args);
        }

    }
}
