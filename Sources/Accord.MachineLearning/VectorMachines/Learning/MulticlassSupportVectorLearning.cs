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
    ///   Configuration function to configure the learning algorithms
    ///   for each of the Kernel Support Vector Machines used in this
    ///   Multi-class Support Vector Machine.
    /// </summary>
    /// 
    /// <param name="inputs">The input data for the learning algorithm.</param>
    /// <param name="outputs">The output data for the learning algorithm.</param>
    /// <param name="machine">The machine for the learning algorithm.</param>
    /// <param name="class1">The class index corresponding to the negative values
    ///     in the output values contained in <paramref name="outputs"/>.</param>
    /// <param name="class2">The class index corresponding to the positive values
    ///     in the output values contained in <paramref name="outputs"/>.</param>
    ///     
    /// <returns>
    ///   The configured <see cref="ISupportVectorMachineLearning"/> algorithm
    ///   to be used to train the given <see cref="KernelSupportVectorMachine"/>.
    /// </returns>
    /// 
    public delegate ISupportVectorMachineLearning SupportVectorMachineLearningConfigurationFunction(
      KernelSupportVectorMachine machine, double[][] inputs, int[] outputs, int class1, int class2);

    /// <summary>
    ///   Subproblem progress event argument.
    /// </summary>
    /// 
    public class SubproblemEventArgs : EventArgs
    {
        /// <summary>
        ///   One of the classes belonging to the subproblem.
        /// </summary>
        /// 
        public int Class1 { get; set; }

        /// <summary>
        ///  One of the classes belonging to the subproblem.
        /// </summary>
        /// 
        public int Class2 { get; set; }

        /// <summary>
        ///   Gets the progress of the overall problem,
        ///   ranging from zero up to <see cref="Maximum"/>.
        /// </summary>
        /// 
        public int Progress { get; set; }

        /// <summary>
        ///   Gets the maximum value for the current <see cref="Progress"/>.
        /// </summary>
        public int Maximum { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SubproblemEventArgs"/> class.
        /// </summary>
        /// 
        /// <param name="class1">One of the classes in the subproblem.</param>
        /// <param name="class2">The other class in the subproblem.</param>
        /// 
        public SubproblemEventArgs(int class1, int class2)
        {
            this.Class1 = class1;
            this.Class2 = class2;
        }

    }

    /// <summary>
    ///   One-against-one Multi-class Support Vector Machine Learning Algorithm
    /// </summary>
    /// 
    /// <remarks>
    ///   This class can be used to train Kernel Support Vector Machines with
    ///   any algorithm using a one-against-one strategy. The underlying training
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
    public class MulticlassSupportVectorLearning : ISupportVectorMachineLearning, ISupportCancellation
    {
        // Training data
        private double[][] inputs;
        private int[] outputs;

        // Machine
        private MulticlassSupportVectorMachine msvm;

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
        ///   Constructs a new Multi-class Support Vector Learning algorithm.
        /// </summary>
        /// 
        /// <param name="inputs">The input learning vectors for the machine learning algorithm.</param>
        /// <param name="machine">The <see cref="MulticlassSupportVectorMachine"/> to be trained.</param>
        /// <param name="outputs">The output labels associated with each of the input vectors. The
        /// class labels should be between 0 and the <see cref="MulticlassSupportVectorMachine.Classes">
        /// number of classes in the multiclass machine</see>.</param>
        /// 
        public MulticlassSupportVectorLearning(MulticlassSupportVectorMachine machine,
            double[][] inputs, int[] outputs)
        {

            // Initial argument checking
            if (machine == null)
                throw new ArgumentNullException("machine");

            if (inputs == null)
                throw new ArgumentNullException("inputs");

            if (outputs == null)
                throw new ArgumentNullException("outputs");

            if (inputs.Length != outputs.Length)
                throw new DimensionMismatchException("outputs",
                    "The number of input vectors and output labels does not match.");

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
                if (outputs[i] < 0 || outputs[i] >= machine.Classes)
                {
                    throw new ArgumentOutOfRangeException("outputs",
                        "The output value at index " + i + " is outside the expected class range"
                        + " All output values must be higher than or equal to 0 and less than " + machine.Classes);
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
        /// <returns>
        ///   The sum of squares error rate for
        ///   the resulting support vector machine.
        /// </returns>
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
            return Run(true, CancellationToken.None);
        }

        /// <summary>
        ///   Runs the one-against-one learning algorithm.
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
        ///   The sum of squares error rate for
        ///   the resulting support vector machine.
        /// </returns>
        /// 
        public double Run(bool computeError, CancellationToken token)
        {
            if (configure == null)
            {
                var excp = new InvalidOperationException("Please specify the algorithm configuration function "
                    + "by setting the Algorithm property for this class. Examples are available in the "
                    + "documentation for Multiclass Support Vector Learning class (given in the help link).");
                excp.HelpLink = "http://accord-framework.net/svn/docs/html/T_Accord_MachineLearning_VectorMachines_MulticlassSupportVectorMachine.htm";
                throw excp;
            }

            
            int classes = msvm.Classes;
            int total = (classes * (classes - 1)) / 2;
            int progress = 0;

            msvm.Reset();


            // For each class i
            Parallel.For(0, msvm.Classes, i =>
            {
                // For each class j
                Parallel.For(0, i, j =>
                {
                    if (token.IsCancellationRequested) 
                        return;

                    // We will start the binary sub-problem
                    var args = new SubproblemEventArgs(i, j);
                    OnSubproblemStarted(args);

                    // Retrieve the associated machine
                    KernelSupportVectorMachine machine = msvm[i, j];

                    // Retrieve the associated classes
                    int[] idx = outputs.Find(x => x == i || x == j);

                    double[][] subInputs = inputs.Submatrix(idx);
                    int[] subOutputs = outputs.Submatrix(idx);


                    // Transform it into a two-class problem
                    subOutputs.ApplyInPlace(x => x = (x == i) ? -1 : +1);

                    // Train the machine on the two-class problem.
                    var subproblem = configure(machine, subInputs, subOutputs, i, j);

                    var canCancel = (subproblem as ISupportCancellation);

                    if (canCancel != null)
                        canCancel.Run(false, token); 
                    else subproblem.Run(false);


                    // Update and report progress
                    args.Progress = Interlocked.Increment(ref progress);
                    args.Maximum = total;

                    OnSubproblemFinished(args);
                });
            });

            // Compute error if required.
            return (computeError) ? ComputeError(inputs, outputs) : 0.0;
        }

        /// <summary>
        ///   Computes the error ratio, the number of
        ///   misclassifications divided by the total
        ///   number of samples in a dataset.
        /// </summary>
        /// 
        public double ComputeError(double[][] inputs, int[] expectedOutputs)
        {
            // Compute errors
            int count = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                int actual = msvm.Compute(inputs[i]);
                int expected = expectedOutputs[i];

                if (actual != expected)
                    Interlocked.Increment(ref count);
            }

            // Return misclassification error ratio
            return count / (double)inputs.Length;
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
