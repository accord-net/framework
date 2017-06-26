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
    using Accord.Math;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    ///   Training-Validation-Testing data split.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The type of the input being partitioned into splits.</typeparam>
    /// <typeparam name="TOutput">The type of the output being partitioned into splits.</typeparam>
    /// 
    public class TrainValDataSplit<TInput, TOutput> : TrainValSplit<DataSubset<TInput, TOutput>>, IDataSplit<TInput, TOutput>
    {
        /// <summary>
        ///   Gets or sets the index of the split in relation to the original dataset, if applicable.
        /// </summary>
        /// 
        public int SplitIndex { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrainValDataSplit{TInput, TOutput}" /> class.
        /// </summary>
        /// 
        public TrainValDataSplit()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrainValDataSplit{TInput, TOutput}"/> class.
        /// </summary>
        /// 
        /// <param name="index">The index associated with this subset, if any.</param>
        /// <param name="inputs">The input instances in this subset.</param>
        /// <param name="outputs">The output instances in this subset.</param>
        /// <param name="weights">The weights associated with the input instances.</param>
        /// <param name="trainIndices">The indices of the training instances in relation to the original dataset.</param>
        /// <param name="validationIndices">The indices of the validation instances in relation to the original dataset.</param>
        /// 
        public TrainValDataSplit(int index, TInput[] inputs, TOutput[] outputs, double[] weights, int[] trainIndices, int[] validationIndices)
        {
            this.SplitIndex = index;
            this.Training = new DataSubset<TInput, TOutput>(index, inputs, outputs, weights, trainIndices);
            this.Validation = new DataSubset<TInput, TOutput>(index, inputs, outputs, weights, validationIndices);
        }

    }
}
