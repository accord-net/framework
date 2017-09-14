// Accord Statistics Library
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

namespace Accord.MachineLearning // TODO: Move those classes to the root Accord namespace
{
    using Accord.Compat;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///   Common interface for parallel algorithms.
    /// </summary>
    /// 
    public interface IParallel : ISupportsCancellation
    {
        /// <summary>
        ///   Gets or sets the parallelization options for this algorithm. It can
        ///   be used to control the maximum number of cores that should be used
        ///   during the algorithm's execution.
        /// </summary>
        /// 
        /// <remarks>
        ///   The <see cref="IParallel"/> interface is implemented by most machine learning
        ///   algorithms in the framework, and it is most common use is to allow the user to
        ///   tune how many cores should be used by a multi-threaded learning algorithm.
        /// </remarks>
        /// 
        /// <example>
        /// <para>
        ///   In the following example, we will be using the <see cref="ParallelOptions"/> property to limit 
        ///   the maximum degree of parallelism of a support vector machine learning algorithm to be 1, meaning
        ///   the algorithm will be running in a single thread:</para>
        ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MulticlassSupportVectorLearningTest.cs" region="doc_learn_ldcd" />
        /// </example>
        /// 
        ParallelOptions ParallelOptions { get; set; }
    }
}
