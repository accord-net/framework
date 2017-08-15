// Accord Core Library
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
    using System.Runtime.CompilerServices;
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Statistics.Kernels;

    /// <summary>
    ///  Contains classes related to <see cref="SupportVectorMachine">Support Vector Machines</see> (SVMs). 
    ///  Contains <see cref="SupportVectorMachine">linear machines</see>, <see cref="SupportVectorMachine{TKernel}">
    ///  kernel machines</see>, <see cref="MulticlassSupportVectorMachine{TKernel}">multi-class machines</see>, SVM-DAGs 
    ///  (Directed Acyclic Graphs), <see cref="MultilabelSupportVectorMachine{TKernel}">multi-label classification</see>
    ///  and also offers support for the <see cref="ProbabilisticOutputCalibration">probabilistic output calibration
    ///  </see> of SVM outputs.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This namespace contains both standard <see cref="SupportVectorMachine"/>s and the
    ///   kernel extension given by <see cref="SupportVectorMachine{TKernel}"/>s. For multiple
    ///   classes or categories, the framework offers <see cref="MulticlassSupportVectorMachine{TKernel}"/>s
    ///   and <see cref="MultilabelSupportVectorMachine{TKernel}"/>s. Multi-class machines can be used for
    ///   cases where a single class should be picked up from a list of several class labels, and
    ///   the multi-label machine for cases where multiple class labels might be detected for a 
    ///   single input vector. The multi-class machines also support two types of classification:
    ///   the faster decision based on Decision Directed Acyclic Graphs, and the more traditional
    ///   based on a Voting scheme.</para>
    ///   
    /// <para>
    ///   Learning can be achieved using the standard <see cref="SequentialMinimalOptimization{TKernel}"/>
    ///   (SMO) algorithm. However, the framework can also learn Least Squares SVMs (LS-SVMs) using <see 
    ///   cref="LeastSquaresLearning"/>, and even calibrate SVMs to produce probabilistic outputs
    ///   using <see cref="ProbabilisticOutputCalibration"/>. A <see cref="Accord.Statistics.Kernels">
    ///   huge variety of kernels functions is available in the statistics namespace</see>, and
    ///   new kernels can be created easily using the <see cref="IKernel"/> interface.</para>
    ///   
    /// <para>
    ///   The namespace class diagram is shown below. </para>
    ///   <img src="..\diagrams\classes\Accord.MachineLearning.VectorMachines.png" />
    ///   
    /// <para>
    ///   Please note that class diagrams for each of the inner namespaces are 
    ///   also available within their own documentation pages.</para>
    /// </remarks>
    /// 
    /// <seealso cref="Accord.MachineLearning.VectorMachines.Learning"/>
    /// <seealso cref="Accord.Statistics.Kernels"/>
    /// 
    [CompilerGenerated]
    class NamespaceDoc
    {
    }
}
