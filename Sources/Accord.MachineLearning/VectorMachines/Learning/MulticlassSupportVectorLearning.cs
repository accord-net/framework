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
    using Accord.Math;
    using System.Threading.Tasks;
    using System.Threading;
    using System.Collections.Concurrent;
    using Accord.MachineLearning;
    using Accord.Math.Optimization.Losses;
    using Accord.Statistics.Kernels;
    using Accord.Statistics;

    using InnerParameters = InnerParameters<SupportVectorMachine<Accord.Statistics.Kernels.IKernel<double[]>>, double[]>;
    using InnerLearning = ISupervisedLearning<SupportVectorMachine<Accord.Statistics.Kernels.IKernel<double[]>>, double[], bool>;

    /// <summary>
    ///   Obsolete.
    /// </summary>
#pragma warning disable 0618
    [Obsolete("Please use the InnerLearning property to configure OneVsOneLearning and OneVsRestLearning classes.")]
    public delegate ISupportVectorMachineLearning
        SupportVectorMachineLearningConfigurationFunction(
            KernelSupportVectorMachine model,
            double[][] inputs, int[] outputs, int classA, int classB);
#pragma warning restore 0618


    /// <summary>
    ///   One-against-one Multi-class Support Vector Machine Learning Algorithm
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class can be used to train Kernel Support Vector Machines with
    ///   any algorithm using a <c>one-against-one</c> strategy. The underlying 
    ///   training algorithm can be configured by defining the <see cref="Algorithm"/>
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
    /// 
#pragma warning disable 612
#pragma warning disable 618
    [Obsolete("Please specify the desired kernel function as a template parameter.")]
    public class MulticlassSupportVectorLearning :
        BaseMulticlassSupportVectorLearning<double[],
            SupportVectorMachine<IKernel<double[]>>, IKernel<double[]>,
            MulticlassSupportVectorMachine>
    // TODO: Change to
    //public class MulticlassSupportVectorLearning :
    //OneVsOneLearning<double[],
    //    KernelSupportVectorMachine,
    //    MulticlassSupportVectorMachine>
    {

        /// <summary>
        ///   Computes the error ratio, the number of
        ///   misclassifications divided by the total
        ///   number of samples in a dataset.
        /// </summary>
        /// 
        [Obsolete()]
        public double ComputeError(double[][] inputs, int[] expected)
        {
            return new ZeroOneLoss(expected).Loss(Model.Decide(inputs));
        }


        private double[][] input;
        private int[] output;

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please do not pass parameters in the constructor. Use the default constructor and the Learn method instead.")]
        public MulticlassSupportVectorLearning(MulticlassSupportVectorMachine model, double[][] input, int[] output)
        {
            this.Model = model;
            this.Kernel = model.Kernel as IKernel;
            this.input = input;
            this.output = output;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MulticlassSupportVectorLearning"/> class.
        /// </summary>
        public MulticlassSupportVectorLearning()
        {
            Learner = (_) => new SequentialMinimalOptimization<IKernel<double[]>>()
            {
                Kernel = new Linear()
            };

            Kernel = new Linear();
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please do not pass parameters in the constructor. Use the default constructor and the Learn method instead.")]
        public double Run()
        {
            Learn(input, output);
            return new ZeroOneLoss(output)
            {
                Mean = true
            }.Loss(Model.Decide(input));
        }


        SupportVectorMachineLearningConfigurationFunction algorithm;

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please set the InnerLearning property instead.")]
        public SupportVectorMachineLearningConfigurationFunction Algorithm
        {
            get { return algorithm; }
            set
            {
                algorithm = value;
                Learner = Convert(value);
            }
        }

        /// <summary>
        ///   Converts <see cref="SupportVectorMachineLearningConfigurationFunction "/>
        ///   into a lambda function that can be passed to the <see cref=" OneVsOneLearning{TInput, TBinary, TModel}.Learner"/>
        ///   property of a <see cref="MulticlassSupportVectorLearning"/> learning algorithm.
        /// </summary>
        /// 
        public Func<InnerParameters, InnerLearning> Convert(SupportVectorMachineLearningConfigurationFunction conf)
        {
            return delegate(InnerParameters parameters)
            {
                int[] y = Classes.ToMinusOnePlusOne(parameters.Outputs);
                var machine = (KernelSupportVectorMachine)parameters.Model;
                ISupportVectorMachineLearning r = conf(machine, parameters.Inputs, y, parameters.Pair.Class1, parameters.Pair.Class2);

                var c = r as ISupervisedLearning<SupportVectorMachine<IKernel<double[]>>, double[], bool>;
                if (c != null)
                    return c;


                // TODO: The following checks exist only to provide support to previous way of using
                // the library and should be removed after a few releases.
                var svc = r as ISupportVectorMachineLearning;
                if (svc != null)
                {
                    svc.Run();
                    return null;
                }

                throw new Exception();
            };
        }

        /// <summary>
        /// Creates an instance of the model to be learned. Inheritors
        /// of this abstract class must define this method so new models
        /// can be created from the training data.
        /// </summary>
        protected override MulticlassSupportVectorMachine Create(int inputs, int classes)
        {
            return new MulticlassSupportVectorMachine(inputs, Kernel, classes);
        }

        /// <summary>
        ///   Gets or sets the kernel function to be used to learn the
        ///   <see cref="SupportVectorMachine{TKernel}">kernel support 
        ///   vector machines</see>.
        /// </summary>
        /// 
        public new IKernel Kernel
        {
            get { return base.Kernel as IKernel; }
            set { base.Kernel = value; }
        }
    }
#pragma warning restore 0612
#pragma warning restore 0618

    
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
    /// 
    public class MulticlassSupportVectorLearning<TKernel, TInput> :
        BaseMulticlassSupportVectorLearning<TInput,
            SupportVectorMachine<TKernel, TInput>, TKernel,
            MulticlassSupportVectorMachine<TKernel, TInput>>
        where TKernel : IKernel<TInput>
        where TInput : ICloneable
    {
        /// <summary>
        /// Creates an instance of the model to be learned. Inheritors
        /// of this abstract class must define this method so new models
        /// can be created from the training data.
        /// </summary>
        protected override MulticlassSupportVectorMachine<TKernel, TInput> Create(int inputs, int classes)
        {
            return new MulticlassSupportVectorMachine<TKernel, TInput>(inputs, Kernel, classes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MulticlassSupportVectorLearning{TKernel, TInput}"/> class.
        /// </summary>
        public MulticlassSupportVectorLearning()
        {
            Learner = (_) => new SequentialMinimalOptimization<TKernel, TInput>();
        }
    }

    // TODO: Probably those two will prove not to be necessary in the long term:

    /// <summary>
    ///   Base class for multi-class support vector learning algorithms.
    /// </summary>
    public abstract class BaseMulticlassSupportVectorLearning<TInput, TBinary, TKernel, TModel> :
        OneVsOneLearning<TInput, TBinary, TModel>
        where TBinary : SupportVectorMachine<TKernel, TInput>
        where TModel : OneVsOne<TBinary, TInput>
        where TKernel : IKernel<TInput>
        where TInput : ICloneable
    {
        /// <summary>
        ///   Gets or sets the kernel function to be used to learn the
        ///   <see cref="SupportVectorMachine{TKernel}">kernel support 
        ///   vector machines</see>.
        /// </summary>
        /// 
        public TKernel Kernel { get; set; }
    }

    /// <summary>
    ///   Base class for multi-class support vector learning algorithms.
    /// </summary>
    public abstract class BaseMulticlassSupportVectorLearning<TBinary, TKernel, TModel> :
        OneVsOneLearning<TBinary, TModel>
        where TBinary : SupportVectorMachine<TKernel>
        where TModel : OneVsOne<TBinary>
        where TKernel : IKernel<double[]>
    {
        /// <summary>
        ///   Gets or sets the kernel function to be used to learn the
        ///   <see cref="SupportVectorMachine{TKernel}">kernel support 
        ///   vector machines</see>.
        /// </summary>
        /// 
        public TKernel Kernel { get; set; }
    }
}
