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
    using Accord.Statistics.Kernels;
    using System;
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    ///   Common interface for binary support vector machines.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The type of the input data handled by the machine.</typeparam>
    /// 
    public interface ISupportVectorMachine<TInput> : IBinaryLikelihoodClassifier<TInput>
    {
        /// <summary>
        ///   Gets or sets the collection of weights used by this machine.
        /// </summary>
        ///
        double[] Weights { get; set; }

        /// <summary>
        ///   Gets or sets the collection of support vectors used by this machine.
        /// </summary>
        ///
        TInput[] SupportVectors { get; set; }

        /// <summary>
        ///   Gets or sets the threshold (bias) term for this machine.
        /// </summary>
        ///
        double Threshold { get; set; }

        /// <summary>
        ///   Gets or sets the kernel used by this machine.
        /// </summary>
        /// 
        IKernel<TInput> Kernel { get; set; }

        /// <summary>
        ///   Gets whether this machine has been calibrated to
        ///   produce probabilistic outputs (through the Probability
        ///   method).
        /// </summary>
        /// 
        bool IsProbabilistic { get; set; }

        /// <summary>
        ///   If this machine has a linear kernel, compresses all
        ///   support vectors into a single parameter vector.
        /// </summary>
        /// 
        void Compress();

    }
}