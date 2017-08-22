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

namespace Accord.MachineLearning.Performance
{
    using Accord.MachineLearning.Performance;
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Accord.Compat;

    /// <summary>
    ///   Bootstrap validation analysis results.
    /// </summary>
    /// 
    [Serializable]
    public class BootstrapResult<TModel, TInput, TOutput> : CrossValidationResult<TModel, TInput, TOutput>
        where TModel : class, ITransform<TInput, TOutput>
    {

        /// <summary>
        ///   Gets the 0.632 bootstrap estimate.
        /// </summary>
        /// 
        public double Estimate { get; private set; }

        /// <summary>
        ///   Gets the number of subsamples taken to compute the bootstrap estimate.
        /// </summary>
        /// 
        public int NumberOfSubsamples {  get { return Models.Length; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapResult{TModel, TInput, TOutput}"/> class.
        /// </summary>
        /// <param name="models">The models created during the cross-validation runs.</param>
        public BootstrapResult(SplitResult<TModel, TInput, TOutput>[] models)
            : base(models)
        {
            this.Estimate = 0.632 * Validation.Mean + 0.368 * Training.Mean;
        }

    }
}
