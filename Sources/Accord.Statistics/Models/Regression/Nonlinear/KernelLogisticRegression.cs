// Accord Statistics Library
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

namespace Accord.Statistics.Models.Regression
{
    using Accord.MachineLearning;
    using Accord.Statistics.Kernels;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    ///   Logistic regression using Kernels.
    /// </summary>
    /// 
    /// <typeparam name="TKernel">The kernel function.</typeparam>
    /// 
    public class KernelLogisticRegression<TKernel> : MulticlassGenerativeClassifierBase<double[]>
        where TKernel : IKernel<double[]>
    {
        /// <summary>
        ///   Gets or sets the kernel function.
        /// </summary>
        /// 
        public TKernel Kernel { get; set; }

        /// <summary>
        ///   Gets or sets the original input data that is needed to 
        ///   compute the kernel (Gram) matrices for the regression.
        /// </summary>
        /// 
        public double[][] Inputs { get; set; }

        /// <summary>
        ///   Gets or sets the linear weights of the regression model. The
        ///   intercept term is not stored in this vector, but is instead
        ///   available through the <see cref="Intercept"/> property.
        /// </summary>
        /// 
        public double[][] Weights { get; set; }

        /// <summary>
        ///   Gets or sets the intercept value for the regression.
        /// </summary>
        /// 
        public double[] Intercept { get; set; }

        /// <summary>Logs the likelihoods.</summary>
        /// <param name="input">The input.</param>
        /// <param name="results">The results.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override double[] LogLikelihoods(double[] input, double[] results)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///   Logistic regression using Kernels.
    /// </summary>
    /// 
    public class KernelLogisticRegression : KernelLogisticRegression<IKernel>
    {
    }

}
