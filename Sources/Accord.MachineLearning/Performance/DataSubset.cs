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
    ///   Subset of a larger dataset.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The type of the input data in this dataset.</typeparam>
    /// 
    public class DataSubset<TInput>
    {
        /// <summary>
        ///   Gets or sets the input data in the dataset.
        /// </summary>
        /// 
        public TInput[] Inputs { get; set; }

        /// <summary>
        ///   Gets or sets the weights associated with each input sample in the dataset.
        /// </summary>
        /// 
        public double[] Weights { get; set; }

        /// <summary>
        ///   Gets or sets the indices of the samples of this subset 
        ///   in relation to the original dataset they belong to.
        /// </summary>
        /// 
        public int[] Indices { get; set; }

        /// <summary>
        ///   Gets or sets a user-defined tag that can be associated with this instance.
        /// </summary>
        /// 
        public object Tag { get; set; }

        /// <summary>
        ///   Gets or sets the size of this subset as a proportion in 
        ///   relation to the original dataset this subset comes from.
        /// </summary>
        /// 
        public double Proportion { get; set; }

        /// <summary>
        ///   Gets or sets an index associated with this subset, if applicable.
        /// </summary>
        /// 
        public int Index { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSubset{TInput}"/> class.
        /// </summary>
        /// 
        public DataSubset()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSubset{TInput}"/> class.
        /// </summary>
        /// 
        /// <param name="subsetSize">The size of the data subset.</param>
        /// <param name="totalSize">The total size of the dataset that contains this subset.</param>
        /// 
        public DataSubset(int subsetSize, int totalSize)
        {
            this.Index = 0;
            this.Proportion = subsetSize / (double)totalSize;
            this.Indices = new int[subsetSize];
            this.Inputs = new TInput[subsetSize];
            this.Weights = Vector.Ones(subsetSize);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSubset{TInput}"/> class.
        /// </summary>
        /// 
        /// <param name="index">The index associated with this subset, if any.</param>
        /// <param name="inputs">The input instances in this subset.</param>
        /// <param name="weights">The weights associated with the input instances.</param>
        /// <param name="indices">The indices of the input instances in relation to the original dataset.</param>
        /// 
        public DataSubset(int index, TInput[] inputs, double[] weights, int[] indices)
        {
            this.Index = index;
            this.Proportion = indices.Length / (double)inputs.Length;
            this.Indices = indices;
            this.Inputs = inputs.Get(indices);
            this.Weights = (weights != null) ? weights.Get(indices) : null;
        }
    }

    /// <summary>
    ///   Subset of a larger dataset.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The type of the input data in this dataset.</typeparam>
    /// <typeparam name="TOutput"> The type of the output data in this dataset.</typeparam>
    /// 
    public class DataSubset<TInput, TOutput>
        : DataSubset<TInput>
    {
        /// <summary>
        ///   Gets or sets the input data in the dataset.
        /// </summary>
        /// 
        public TOutput[] Outputs { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="DataSubset{TInput, TOutput}"/> class.
        /// </summary>
        /// 
        public DataSubset()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSubset{TInput, TOutput}"/> class.
        /// </summary>
        /// 
        public DataSubset(int subsetSize, int totalSize)
            : base(subsetSize, totalSize)
        {
            this.Outputs = new TOutput[subsetSize];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSubset{TInput, TOutput}"/> class.
        /// </summary>
        /// 
        /// <param name="index">The index associated with this subset, if any.</param>
        /// <param name="inputs">The input instances in this subset.</param>
        /// <param name="outputs">The output instances in this subset.</param>
        /// <param name="weights">The weights associated with the input instances.</param>
        /// <param name="indices">The indices of the input instances in relation to the original dataset.</param>
        /// 
        public DataSubset(int index, TInput[] inputs, TOutput[] outputs, double[] weights, int[] indices)
            : base(index, inputs, weights, indices)
        {
            this.Outputs = outputs.Get(indices);
        }

    }
}
