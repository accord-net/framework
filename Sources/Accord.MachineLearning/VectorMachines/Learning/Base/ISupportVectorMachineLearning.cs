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
    using Accord.Statistics.Kernels;
    using System;
    using System.Threading;
    using Accord.Compat;

    /// <summary>
    ///   Common interface for Support Machine Vector learning algorithms.
    /// </summary>
    /// 
    public interface ILinearSupportVectorMachineLearning :
        ISupervisedLearning<SupportVectorMachine, double[], double>,
        ISupervisedLearning<SupportVectorMachine, double[], int>,
        ISupervisedLearning<SupportVectorMachine, double[], bool>,
        ISupportVectorMachineLearning
    {
    }

    /*public interface ISupportVectorMachineLearning : 
        ISupportVectorMachineLearning<double[]>
    {
    }*/

    /*
    /// <summary>
    ///   Common interface for Support Machine Vector learning algorithms.
    /// </summary>
    /// 
    public interface ISupportVectorMachineLearning<TInput> :
        ISupervisedBinaryLearning<SupportVectorMachine<IKernel<TInput>, TInput>, TInput>,
        ISupervisedLearning<SupportVectorMachine<IKernel<TInput>, TInput>, TInput, double>
    {

    }*/

    // TODO: Remove those interfaces

    /// <summary>
    ///   Common interface for Support Machine Vector learning algorithms.
    /// </summary>
    /// 
    public interface ISupportVectorMachineLearning :
        ISupportVectorMachineLearning<double[]>
    {
    }

    /// <summary>
    ///   Common interface for Support Machine Vector learning algorithms.
    /// </summary>
    /// 
    public interface ISupportVectorMachineLearning<TInput> :
        ISupervisedBinaryLearning<ISupportVectorMachine<TInput>, TInput>,
        ISupervisedLearning<ISupportVectorMachine<TInput>, TInput, double>
    {
        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete]
        double Run();

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete]
        double Run(bool computeError);
    }

    /// <summary>
    ///   Common interface for Support Machine Vector learning algorithms.
    /// </summary>
    /// 
    public interface ISupportVectorMachineLearning<TKernel, TInput> :
        ISupervisedBinaryLearning<SupportVectorMachine<TKernel, TInput>, TInput>,
        ISupervisedLearning<SupportVectorMachine<TKernel, TInput>, TInput, double>
        where TKernel : IKernel<TInput>
        where TInput : ICloneable
    {
        /// <summary>
        ///   Gets or sets the support vector machine being learned.
        /// </summary>
        /// 
        SupportVectorMachine<TKernel, TInput> Model { get; set; }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete]
        double Run();

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete]
        double Run(bool computeError);
    }


}
