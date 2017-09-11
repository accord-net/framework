// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Darko Jurić, 2013
// https://code.google.com/p/accord/issues/detail?id=27
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

namespace Accord.MachineLearning.Boosting.Learners
{
    using System;
    using Accord.Math.Comparers;
    using Accord.Math;
    using Accord.Statistics;

    /// <summary>
    ///   Adapter for models that do not implement a .Decide function.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type for the weak classifier model.</typeparam>
    /// 
    [Obsolete("This class will be removed.")]
    public class Weak<TModel> : BinaryClassifierBase<double[]>
    {
        /// <summary>
        ///   Gets or sets the weak decision model.
        /// </summary>
        /// 
        public TModel Model { get; set; }

        /// <summary>
        ///   Gets or sets the decision function used by the <see cref="Model"/>.
        /// </summary>
        /// 
        public Func<TModel, double[], int> Function { get; set; }

        /// <summary>
        ///   Creates a new Weak classifier given a 
        ///   classification model and its decision function.
        /// </summary>
        /// 
        /// <param name="model">The classifier.</param>
        /// <param name="function">The classifier decision function.</param>
        /// 
        public Weak(TModel model, Func<TModel, double[], int> function)
        {
            this.Model = model;
            this.Function = function;
        }

        /// <summary>
        ///   Computes the classifier decision for a given input.
        /// </summary>
        /// 
        /// <param name="inputs">The input vector.</param>
        /// 
        /// <returns>The model's decision label.</returns>
        /// 
        [Obsolete("Please use the Decide() method instead.")]
        public int Compute(double[] inputs)
        {
            return Function(Model, inputs);
        }

        /// <summary>
        /// Computes a class-label decision for a given <paramref name="input" />.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// one of the <see cref="P:Accord.MachineLearning.ITransform.NumberOfOutputs" /> possible classes.</param>
        /// <returns>A class-label that best described <paramref name="input" /> according
        /// to this classifier.</returns>
        public override bool Decide(double[] input)
        {
            return Classes.Decide(Function(Model, input));
        }
    }
}
