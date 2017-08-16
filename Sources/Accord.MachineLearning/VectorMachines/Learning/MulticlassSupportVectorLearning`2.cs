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
    ///   <see cref="SequentialMinimalOptimization{TKernel}">Sequential Minimal Optimization
    ///   </see> (SMO) algorithm.</para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example shows how to learn a linear, multi-class support vector 
    ///   machine using the <see cref="LinearDualCoordinateDescent"/> algorithm. </para>
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MulticlassSupportVectorLearningTest.cs" region="doc_learn_ldcd" />
    /// 
    /// <para>
    ///   The following example shows how to learn a non-linear, multi-class support 
    ///   vector machine using the <see cref="Gaussian"/> kernel and the 
    ///   <see cref="SequentialMinimalOptimization{TKernel}"/> algorithm. </para>
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MulticlassSupportVectorLearningTest.cs" region="doc_learn_gaussian" />
    ///   
    /// <para>
    ///   Support vector machines can have their weights calibrated in order to produce 
    ///   probability estimates (instead of simple class separation distances). The
    ///   following example shows how to use <see cref="ProbabilisticOutputCalibration"/>
    ///   within <see cref="MulticlassSupportVectorLearning{TKernel}"/> to generate a probabilistic
    ///   SVM:</para>
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MulticlassSupportVectorLearningTest.cs" region="doc_learn_calibration" />
    /// </example>
    /// 
    /// <seealso cref="SequentialMinimalOptimization{TKernel}"/>
    /// <seealso cref="MulticlassSupportVectorLearning{TKernel}"/>
    /// <seealso cref="MultilabelSupportVectorLearning{TKernel}"/>
    /// 
    /// <seealso cref="SupportVectorMachine"/>
    /// 
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
            Learner = (p) => new SequentialMinimalOptimization<TKernel>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MulticlassSupportVectorLearning{TKernel}"/> class.
        /// </summary>
        /// 
        /// <param name="machine">The existing machine to be learned.</param>
        /// 
        public MulticlassSupportVectorLearning(MulticlassSupportVectorMachine<TKernel> machine)
        {
            Model = machine;
            Learner = (p) => new SequentialMinimalOptimization<TKernel>()
            {
                Model = Model[p.Pair.Class1, p.Pair.Class2]
            };
        }
    }
}