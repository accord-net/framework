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
    using System.Threading;

    /// <summary>
    ///   Common interface for Support Machine Vector learning algorithms.
    /// </summary>
    /// 
    public interface ISupportVectorMachineLearning
    {

        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        double Run();

        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        /// <param name="computeError">
        ///   True to compute error after the training
        ///   process completes, false otherwise.
        /// </param>
        /// 
        double Run(bool computeError);

    }

    /// <summary>
    ///   Common interface for Support Machine Vector learning
    ///   algorithms which support thread cancellation.
    /// </summary>
    /// 
    public interface ISupportCancellation : ISupportVectorMachineLearning
    {

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
        double Run(bool computeError, CancellationToken token);

    }
}
