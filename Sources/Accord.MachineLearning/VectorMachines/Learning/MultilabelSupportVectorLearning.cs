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
    using Accord.MachineLearning;
    using Accord.Statistics.Kernels;
    using Accord.Statistics;
    using Accord.Math.Optimization.Losses;

    using InnerParameters = InnerParameters<SupportVectorMachine<Accord.Statistics.Kernels.IKernel<double[]>>, double[]>;
    using InnerLearning = ISupervisedLearning<SupportVectorMachine<Accord.Statistics.Kernels.IKernel<double[]>>, double[], bool>;

    /// <summary>
    ///   Obsolete.
    /// </summary>
    [Obsolete("Please specify the desired kernel function as a template parameter.")]
    public class MultilabelSupportVectorLearning :
        BaseMultilabelSupportVectorLearning<double[],
            SupportVectorMachine<IKernel<double[]>>, IKernel<double[]>,
            MultilabelSupportVectorMachine>
    {

        private double[][] input;
        private int[][] output;

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please do not pass parameters in the constructor. Use the default constructor and the Learn method instead.")]
        public MultilabelSupportVectorLearning(MultilabelSupportVectorMachine model, double[][] input, int[][] output)
        {
            this.Model = model;
            this.input = input;
            this.output = output;
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please do not pass parameters in the constructor. Use the default constructor and the Learn method instead.")]
        public MultilabelSupportVectorLearning(MultilabelSupportVectorMachine model, double[][] input, int[] output)
        {
            this.Model = model;
            this.input = input;
            this.output = Jagged.OneHot<int>(output);
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please do not pass parameters in the constructor. Use the default constructor and the Learn method instead.")]
        public double Run()
        {
            Learn(input, output);
            return new HammingLoss(output)
            {
                Mean = true
            }.Loss(Model.Decide(input));
        }

#pragma warning disable 0618
#pragma warning disable 0612
        SupportVectorMachineLearningConfigurationFunction algorithm;

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please use the Configure method instead.")]
        public SupportVectorMachineLearningConfigurationFunction Algorithm
        {
            get { return algorithm; }
            set
            {
                algorithm = value;
                base.Learner = Convert(value);
            }
        }

        /// <summary>
        ///   Converts <see cref="SupportVectorMachineLearningConfigurationFunction "/>
        ///   into a lambda function that can be passed to the <see cref="OneVsRestLearning{TInput, TBinary, TModel}.Learner"/>
        ///   property of a <see cref="MultilabelSupportVectorLearning"/> learning algorithm.
        /// </summary>
        /// 
        public static Func<InnerParameters, InnerLearning> Convert(
            SupportVectorMachineLearningConfigurationFunction conf)
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
#pragma warning restore 0618
#pragma warning restore 0612

        /// <summary>
        /// Creates an instance of the model to be learned. Inheritors
        /// of this abstract class must define this method so new models
        /// can be created from the training data.
        /// </summary>
        protected override MultilabelSupportVectorMachine Create(int inputs, int classes)
        {
            return new MultilabelSupportVectorMachine(inputs, Kernel, classes);
        }

        
        /// <summary>
        ///   Gets or sets the kernel function to be used to learn the
        ///   <see cref="SupportVectorMachine{TKernel}">kernel support 
        ///   vector machines</see>.
        /// </summary>
        /// 
        public new IKernel Kernel
        {
            // TODO: Remove the shadowing and convert this class to a Linear
            // Multi label support vector learning
            get { return base.Kernel as IKernel; }
            set { base.Kernel = value; }
        }
    }

    /// <summary>
    ///   One-against-all Multi-label Support Vector Machine Learning Algorithm
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class can be used to train Kernel Support Vector Machines with
    ///   any algorithm using a <c>one-against-all</c> strategy. The underlying 
    ///   training algorithm can be configured by defining the <see cref="OneVsRestLearning{TInput, TBinary, TModel}.Learner"/>
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
    public class MultilabelSupportVectorLearning<TKernel> :
        BaseMultilabelSupportVectorLearning<double[],
            SupportVectorMachine<TKernel>, TKernel,
            MultilabelSupportVectorMachine<TKernel>>
        where TKernel : IKernel<double[]>
    {
        /// <summary>
        /// Creates an instance of the model to be learned. Inheritors
        /// of this abstract class must define this method so new models
        /// can be created from the training data.
        /// </summary>
        protected override MultilabelSupportVectorMachine<TKernel> Create(int inputs, int classes)
        {
            return new MultilabelSupportVectorMachine<TKernel>(inputs, Kernel, classes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultilabelSupportVectorLearning{TKernel}"/> class.
        /// </summary>
        public MultilabelSupportVectorLearning()
        {
            Learner = (_) => new SequentialMinimalOptimization<TKernel>();
        }
    }

    /// <summary>
    ///   One-against-all Multi-label Support Vector Machine Learning Algorithm
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class can be used to train Kernel Support Vector Machines with
    ///   any algorithm using a <c>one-against-all</c> strategy. The underlying 
    ///   training algorithm can be configured by defining the <see cref="OneVsRestLearning{TInput, TBinary, TModel}.Learner"/>
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
    public class MultilabelSupportVectorLearning<TKernel, TInput> :
        BaseMultilabelSupportVectorLearning<TInput,
            SupportVectorMachine<TKernel, TInput>, TKernel,
            MultilabelSupportVectorMachine<TKernel, TInput>>
        where TKernel : IKernel<TInput>
        where TInput : ICloneable
    {
        /// <summary>
        /// Creates an instance of the model to be learned. Inheritors
        /// of this abstract class must define this method so new models
        /// can be created from the training data.
        /// </summary>
        protected override MultilabelSupportVectorMachine<TKernel, TInput> Create(int inputs, int classes)
        {
            return new MultilabelSupportVectorMachine<TKernel, TInput>(inputs, Kernel, classes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultilabelSupportVectorLearning{TKernel, TInput}"/> class.
        /// </summary>
        public MultilabelSupportVectorLearning()
        {
            Learner = (_) => new SequentialMinimalOptimization<TKernel, TInput>();
        }
    }

    // TODO: Probably this class will prove not to be necessary in the long term:

    /// <summary>
    ///   Base class for multi-label support vector learning algorithms.
    /// </summary>
    public abstract class BaseMultilabelSupportVectorLearning<TInput, TBinary, TKernel, TModel> :
        OneVsRestLearning<TInput, TBinary, TModel>
        where TBinary : SupportVectorMachine<TKernel, TInput>
        where TModel : OneVsRest<TBinary, TInput>
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

}
