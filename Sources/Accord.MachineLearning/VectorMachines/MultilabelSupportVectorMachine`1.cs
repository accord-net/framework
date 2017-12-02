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
    using System;
    using System.IO;
    using Accord.MachineLearning.VectorMachines.Learning;
    using System.Runtime.Serialization.Formatters.Binary;
    using Accord.Math;
    using Accord.Statistics.Kernels;
    using System.Collections.Generic;
    using System.Threading;
    using System.Runtime.Serialization;
    using Accord.MachineLearning;
    using System.Reflection;
    using Accord.Compat;
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
    ///   <see cref="SequentialMinimalOptimization{TKernel}"/> algorithm. </para>
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MultilabelSupportVectorLearningTest.cs" region="doc_learn_gaussian" />
    ///   
    /// <para>
    ///   Support vector machines can have their weights calibrated in order to produce probability 
    ///   estimates (instead of simple class separation distances). The following example shows how 
    ///   to use <see cref="ProbabilisticOutputCalibration"/> within <see cref="MulticlassSupportVectorLearning{TKernel}"/> 
    ///   to generate a probabilistic SVM:</para>
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MultilabelSupportVectorLearningTest.cs" region="doc_learn_calibration" />
    /// </example>
    /// 
    /// <seealso cref="MultilabelSupportVectorLearning{TKernel}"/>
    /// <seealso cref="MulticlassSupportVectorMachine{TKernel}"/>
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
