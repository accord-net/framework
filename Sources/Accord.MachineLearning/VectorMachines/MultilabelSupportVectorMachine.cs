﻿// Accord Machine Learning Library
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
    using System;
    using System.IO;
    using Accord.MachineLearning.VectorMachines.Learning;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading.Tasks;
    using Accord.Math;
    using Accord.Statistics.Kernels;
    using System.Collections.Generic;
    using System.Threading;
    using System.Runtime.Serialization;
    using Accord.MachineLearning;
    using System.Reflection;

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
    /// <seealso cref="MultilabelSupportVectorLearning"/>
    /// <seealso cref="MulticlassSupportVectorMachine"/>
    /// 
    [Serializable]
    [Obsolete("Please use MultilabelSupportVectorMachine<TKernel> instead.")]
    [SerializationBinder(typeof(MultilabelSupportVectorMachine.MultilabelSupportVectorMachineBinder))]
    public class MultilabelSupportVectorMachine :
        MultilabelSupportVectorMachine<IKernel<double[]>>
    {
#pragma warning disable 0618
        /// <summary>
        /// Initializes a new instance of the <see cref="MultilabelSupportVectorMachine"/> class.
        /// </summary>
        /// <param name="inputs">The number of inputs by the machine.</param>
        /// <param name="classes">The number of classes to be handled by the machine.</param>
        ///
        public MultilabelSupportVectorMachine(int inputs, int classes)
            : base(classes, () => new KernelSupportVectorMachine(new Linear(), inputs))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultilabelSupportVectorMachine"/> class.
        /// </summary>
        /// 
        /// <param name="inputs">The number of inputs by the machine.</param>
        /// <param name="classes">The number of classes to be handled by the machine.</param>
        /// <param name="kernel">The kernel function to be used in the machine.</param>
        /// 
        public MultilabelSupportVectorMachine(int inputs, IKernel kernel, int classes)
            : base(classes, () => new KernelSupportVectorMachine(kernel, inputs))
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="MultilabelSupportVectorMachine"/> class.
        /// </summary>
        /// <param name="machines">The existing machines for detecting each of the classes against all other classes.</param>
        public MultilabelSupportVectorMachine(KernelSupportVectorMachine[] machines)
            : base(machines.Length, () => null)
        {
            NumberOfInputs = machines[0].NumberOfInputs;
            for (int i = 0; i < machines.Length; i++)
            {
                Models[i] = machines[i];
                if (machines[i].NumberOfInputs != NumberOfInputs)
                    throw new ArgumentException();
            }
        }
#pragma warning restore 0618



        #region Obsolete
        /// <summary>
        ///   Gets the classifier for class <paramref name="index"/>.
        /// </summary>
        /// 
        [Obsolete("Use the Models property instead.")]
        public new KernelSupportVectorMachine this[int index]
        {
            get { return Models[index] as KernelSupportVectorMachine; }
        }

        /// <summary>
        ///   Saves the machine to a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to which the machine is to be serialized.</param>
        /// 
        [Obsolete("Please use Accord.IO.Serializer.Save() instead (or use it as an extension method).")]
        public void Save(Stream stream)
        {
            Accord.IO.Serializer.Save(this, stream);
        }

        /// <summary>
        ///   Saves the machine to a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file to which the machine is to be serialized.</param>
        /// 
        [Obsolete("Please use Accord.IO.Serializer.Save() instead (or use it as an extension method).")]
        public void Save(string path)
        {
            Accord.IO.Serializer.Save(this, path);
        }

        /// <summary>
        ///   Loads a machine from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the machine is to be deserialized.</param>
        /// 
        /// <returns>The deserialized machine.</returns>
        /// 
        [Obsolete("Please use Accord.IO.Serializer.Save() instead (or use it as an extension method).")]
        public static MultilabelSupportVectorMachine Load(Stream stream)
        {
            return Accord.IO.Serializer.Load<MultilabelSupportVectorMachine>(stream);
        }

        /// <summary>
        ///   Loads a machine from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the machine is to be deserialized.</param>
        /// 
        /// <returns>The deserialized machine.</returns>
        /// 
        [Obsolete("Please use Accord.IO.Serializer.Save() instead (or use it as an extension method).")]
        public static MultilabelSupportVectorMachine Load(string path)
        {
            return Accord.IO.Serializer.Load<MultilabelSupportVectorMachine>(path);
        }


        /// <summary>
        ///   Gets the number of classes.
        /// </summary>
        /// 
        [Obsolete("Please use NumberOfOutputs instead.")]
        public int Classes
        {
            get { return NumberOfOutputs; }
        }

        /// <summary>
        ///   Gets the number of inputs of the machines.
        /// </summary>
        /// 
        [Obsolete("Please use NumberOfInputs instead.")]
        public int Inputs
        {
            get { return NumberOfInputs; }
        }

        /// <summary>
        ///   Gets the subproblems classifiers.
        /// </summary>
        /// 
#pragma warning disable 0618

        [Obsolete("Please use Models instead.")]
        public KernelSupportVectorMachine[] Machines
        {
            get { return Models.Convert(x => (KernelSupportVectorMachine)x); }
        }
#pragma warning restore 0618


        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="output">The output for the given input.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        /// 
        [Obsolete("Please use the Decide method instead.")]
        public int Compute(double[] inputs, out double output)
        {
            double[] responses;
            Compute(inputs, out responses);

            int imax;
            output = responses.Max(out imax);
            return imax;
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding outputs.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="responses">The model response for each class.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        /// 
        [Obsolete("Please use the Decide or Transform methods instead.")]
        public int[] Compute(double[] inputs, out double[] responses)
        {
            int[] decision = null;
            responses = this.Probabilities(inputs, ref decision);
            return decision;
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding outputs.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        /// 
        [Obsolete("Please use the Decide method instead.")]
        public int[] Compute(double[] inputs)
        {
            return Decide(inputs).ToInt32();
        }
        #endregion


        #region Serialization backwards compatibility

        internal class MultilabelSupportVectorMachineBinder : SerializationBinder
        {

            public override Type BindToType(string assemblyName, string typeName)
            {
                AssemblyName name = new AssemblyName(assemblyName);

                if (name.Version < new Version(3, 1, 0))
                {
#pragma warning disable 618
                    if (typeName == "Accord.MachineLearning.VectorMachines.MultilabelSupportVectorMachine")
                        return typeof(MultilabelSupportVectorMachine_2_13);
                    else if (typeName == "Accord.MachineLearning.VectorMachines.KernelSupportVectorMachine[]")
                        return typeof(KernelSupportVectorMachine.KernelSupportVectorMachine_2_13[]);
                    else if (typeName == "Accord.MachineLearning.VectorMachines.KernelSupportVectorMachine")
                        return typeof(KernelSupportVectorMachine.KernelSupportVectorMachine_2_13);
#pragma warning restore 618
                }

                return Type.GetType(typeName);
            }
        }

#pragma warning disable 0169
#pragma warning disable 0649

        [Serializable]
        internal class MultilabelSupportVectorMachine_2_13
        {
            public KernelSupportVectorMachine.KernelSupportVectorMachine_2_13[] machines;

            public MultilabelSupportVectorMachine_2_13()
            {

            }

            public static implicit operator MultilabelSupportVectorMachine(MultilabelSupportVectorMachine_2_13 obj)
            {
#pragma warning disable 618
                var svms = obj.machines.Apply((x, i) => (KernelSupportVectorMachine)x);
#pragma warning restore 618
                var svm = new MultilabelSupportVectorMachine(svms);
                return svm;
            }
        }

#pragma warning restore 0169
#pragma warning restore 0649

        #endregion
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
    /// <seealso cref="MultilabelSupportVectorLearning"/>
    /// <seealso cref="MulticlassSupportVectorMachine"/>
    /// 
    [Serializable]
    public class MultilabelSupportVectorMachine<TKernel> :
        MultilabelSupportVectorMachine<
            SupportVectorMachine<TKernel>, 
            TKernel, 
            double[]>
        where TKernel : IKernel<double[]>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultilabelSupportVectorMachine{TKernel}"/> class.
        /// </summary>
        /// <param name="classes">The number of classes in the multi-label classification problem.</param>
        /// <param name="initializer">A function to create the inner binary classifiers.</param>
        public MultilabelSupportVectorMachine(int classes, Func<SupportVectorMachine<TKernel>> initializer)
            : base(classes, initializer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultilabelSupportVectorMachine{TKernel}"/> class.
        /// </summary>
        /// <param name="inputs">The number of inputs by the machine.</param>
        /// <param name="classes">The number of classes to be handled by the machine.</param>
        /// <param name="kernel">The kernel function to be used in the machine.</param>
        /// 
        public MultilabelSupportVectorMachine(int inputs, TKernel kernel, int classes)
            : base(classes, () => new SupportVectorMachine<TKernel>(inputs, kernel))
        {
        }

    }

}
