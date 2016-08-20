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

namespace Accord.MachineLearning.VectorMachines.Learning
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Accord.Statistics.Kernels;

    /// <summary>
    ///   One-against-one Multi-class Support Vector Machine Learning Algorithm
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class can be used to train Kernel Support Vector Machines with
    ///   any algorithm using a <c>one-against-one</c> strategy. The underlying 
    ///   training algorithm can be configured by defining the <see cref="OneVsOneLearning{TInput, TBinary, TInput}.Learner"/>
    ///   property.</para>
    ///   
    /// <para>
    ///   One example of learning algorithm that can be used with this class is the
    ///   <see cref="SequentialMinimalOptimization">Sequential Minimal Optimization
    ///   </see> (SMO) algorithm.</para>
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
    ///   double error = teacher.Run(); // output should be 0
    ///   
    ///   // Compute the decision output for one of the input vectors
    ///   int decision = machine.Compute(new double[] { 3 }); // result should be 3
    ///   </code>
    ///   
    /// <para>
    ///   The next example is a simple 3 classes classification problem.
    ///   It shows how to use a different kernel function, such as the
    ///   polynomial kernel of degree 2.</para>
    /// 
    ///   <code>
    ///   // Sample input data
    ///   double[][] inputs =
    ///   {
    ///       new double[] { -1, 3, 2 },
    ///       new double[] { -1, 3, 2 },
    ///       new double[] { -1, 3, 2 },
    ///       new double[] { 10, 82, 4 },
    ///       new double[] { 10, 15, 4 },
    ///       new double[] { 0, 0, 1 },
    ///       new double[] { 0, 0, 2 },
    ///   };
    ///   
    ///   // Output for each of the inputs
    ///   int[] outputs = { 0, 3, 1, 2 };
    ///   
    ///   
    ///   // Create a new polynomial kernel
    ///   IKernel kernel = new Polynomial(2);
    ///   
    ///   // Create a new Multi-class Support Vector Machine with one input,
    ///   //  using the linear kernel and for four disjoint classes.
    ///   var machine = new MulticlassSupportVectorMachine(inputs: 3, kernel: kernel, classes: 4);
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
    ///   double error = teacher.Run(); // output should be 0
    ///   
    ///   // Compute the decision output for one of the input vectors
    ///   int decision = machine.Compute( new double[] { -1, 3, 2 });
    ///   </code>
    /// </example>
    /// 
    /// <seealso cref="SequentialMinimalOptimization"/>
    /// <seealso cref="MulticlassSupportVectorLearning"/>
    /// <seealso cref="MultilabelSupportVectorLearning"/>
    /// 
    /// <seealso cref="SupportVectorMachine"/>
    /// <seealso cref="KernelSupportVectorMachine"/>
    public class MulticlassSupportVectorLearning<TKernel> :
        BaseMulticlassSupportVectorLearning<double[],
        SupportVectorMachine<TKernel>, TKernel,
        MulticlassSupportVectorMachine<TKernel>>
    where TKernel : IKernel<double[]>
    {
        /// <summary>
        /// Creates an instance of the model to be learned. Inheritors
        /// of this abstract class must define this method so new models
        /// can be created from the training data.
        /// </summary>
        protected override MulticlassSupportVectorMachine<TKernel> Create(int inputs, int classes)
        {
            return new MulticlassSupportVectorMachine<TKernel>(inputs, Kernel, classes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MulticlassSupportVectorLearning{TKernel}"/> class.
        /// </summary>
        public MulticlassSupportVectorLearning()
        {
            Learner = (_) => new SequentialMinimalOptimization<TKernel>();
        }
    }
}