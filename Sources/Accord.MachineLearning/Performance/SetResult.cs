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
    using System;
    using Accord.Compat;

    /// <summary>
    ///   Training and validation errors of a model. 
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// 
    [Serializable]
    public class SetResult<TModel>
    {

        /// <summary>
        ///   Gets or sets the set name (e.g. "Training" or "Testing").
        /// </summary>
        /// 
        /// <value>The name of this set.</value>
        /// 
        public string Name { get; set; }


        /// <summary>
        ///   Gets the model.
        /// </summary>
        /// 
        public TModel Model { get; private set; }

        /// <summary>
        ///   Gets the indices of the samples in this subset in the original complete dataset.
        /// </summary>
        /// 
        public int[] Indices { get; private set; }

        /// <summary>
        ///   Gets the number of samples in this subset.
        /// </summary>
        /// 
        public int NumberOfSamples { get; private set; }

        /// <summary>
        ///   Gets how much this subset represents, in proportion, of the original dataset.
        /// </summary>
        /// 
        public double Proportion { get; private set; }

        /// <summary>
        ///   Gets the metric value for the model in the current <see cref="Name">set</see>.
        /// </summary>
        /// 
        public double Value { get; set; }

        /// <summary>
        ///   Gets the variance of the validation value for the model, if available.
        /// </summary>
        /// 
        public double Variance { get; set; }

        /// <summary>
        ///   Gets the standard deviation of the validation value for the model, if available.
        /// </summary>
        /// 
        public double StandardDeviation { get { return Math.Sqrt(Variance); } }

        /// <summary>
        ///   Gets or sets a tag for user-defined information.
        /// </summary>
        /// 
        public object Tag { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SetResult{TModel}"/> class.
        /// </summary>
        /// 
        /// <param name="model">The model computed in this subset.</param>
        /// <param name="indices">The indices of the samples in this subset.</param>
        /// <param name="name">The name of this set.</param>
        /// <param name="proportion">The proportion of samples in this subset, compared to the full dataset.</param>
        /// 
        public SetResult(TModel model, int[] indices, string name, double proportion)
        {
            this.Name = name;
            this.Model = model;
            this.Indices = indices;
            this.NumberOfSamples = indices.Length;
            this.Proportion = proportion;
        }

    }
}
