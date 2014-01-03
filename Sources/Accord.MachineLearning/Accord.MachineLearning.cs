// Accord Math Library
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

namespace Accord.MachineLearning
{
    using System.Runtime.CompilerServices;
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Statistics.Kernels;

    /// <summary>
    ///  Support Vector Machines, Decision Trees, Naive Bayesian models, K-means, 
    ///  Gaussian Mixture models and general algorithms such as RANSAC, Cross-validation
    ///  and Grid-Search for machine-learning applications.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Without question, the most popular models available in this namespace are the <see
    ///   cref="Accord.MachineLearning.VectorMachines">Support Vector Machines</see>. The inner
    ///   VectorMachines namespace contains both standard <see cref="SupportVectorMachine"/>s and
    ///   their kernel extension given by <see cref="KernelSupportVectorMachine"/>s. For multiple
    ///   classes or categories, the framework also offers <see cref="MulticlassSupportVectorMachine"/>s
    ///   and <see cref="MultilabelSupportVectorMachine"/>s. Multi-class machines can be used for
    ///   cases where a single class should be picked up from a list of several class labels, and
    ///   the multi-label machine for cases where multiple class labels might be detected for a 
    ///   single input vector. The multi-class machines also support two types of classification:
    ///   the faster decision based on Decision Directed Acyclic Graphs, and the more traditional
    ///   based on a Voting scheme.</para>
    ///   
    /// <para>
    ///   Still talking about SVMs, the framework offers several methods to learn and train those
    ///   machines. While the most popular method will always be the <see cref="SequentialMinimalOptimization"/>
    ///   (SMO) algorithm, the framework can also learn Least Squares SVMs (LS-SVMs) using <see 
    ///   cref="LeastSquaresLearning"/>, and even calibrate SVMs to produce probabilistic outputs
    ///   using <see cref="ProbabilisticOutputLearning"/>. A <see cref="Accord.Statistics.Kernels">
    ///   huge variety of kernels functions is available in the statistics namespace</see>, and
    ///   new kernels can be created easily using the <see cref="IKernel"/> interface.</para>
    ///   
    /// <para>
    ///   Besides SVMs, this namespace includes <see cref="Accord.MachineLearning.Bayes">Naïve Bayes</see>,
    ///   <see cref="Accord.MachineLearning.DecisionTrees">Decision Trees</see>, 
    ///   <see cref="Accord.MachineLearning.Boosting">Boosting methods</see>,
    ///   <see cref="IClusteringAlgorithm{T}">Clustering techniques</see>, and other
    ///   diverse classifiers, such as the <see cref="KNearestNeighbors"/>, and the much simpler
    ///   <see cref="MinimumMeanDistanceClassifier">minimum (mean) distance classifier</see>. </para>
    ///   
    /// <para>
    ///   To measure the performance of all those models, it is possible to choose from several
    ///   performance assessment techniques such as <see cref="Bootstrap"/>, <see cref="CrossValidation{T}">
    ///   k-Fold Cross-Validation (CV)</see> and <see cref="SplitSetValidation{T}">split-set validation</see>.
    ///   </para>
    ///   
    /// <para>
    ///   The namespace class diagram is shown below. </para>
    ///   <img src="..\diagrams\classes\Accord.MachineLearning.png" />
    ///   
    /// <para>
    ///   Please note that class diagrams for each of the inner namespaces are 
    ///   also available within their own documentation pages.</para>
    /// </remarks>
    /// 
    /// <seealso cref="Accord.MachineLearning.Bayes"/>
    /// <seealso cref="Accord.MachineLearning.Boosting"/>
    /// <seealso cref="Accord.MachineLearning.DecisionTrees"/>
    /// <seealso cref="Accord.MachineLearning.Geometry"/>
    /// <seealso cref="Accord.MachineLearning.Structures"/>
    /// <seealso cref="Accord.MachineLearning.VectorMachines"/>
    /// 
    [CompilerGenerated]
    class NamespaceDoc
    {
    }
}
