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

namespace Accord.Statistics.Models.Fields.Learning
{
    using Accord.Statistics.Models.Fields.Functions;
    using System;
    using Accord.Compat;
    using System.Threading;

    /// <summary>
    ///   Abstract base class for hidden Conditional Random Fields algorithms.
    /// </summary>
    /// 
    public abstract class BaseHiddenConditionalRandomFieldLearning<T>
    {
        [NonSerialized]
        CancellationToken token = new CancellationToken();

        /// <summary>
        /// Gets or sets a cancellation token that can be used to
        /// stop the learning algorithm while it is running.
        /// </summary>
        public CancellationToken Token
        {
            get { return token; }
            set { token = value; }
        }

        /// <summary>
        ///   Gets or sets the potential function to be used if this learning algorithm 
        ///   needs to create a new <see cref="HiddenConditionalRandomField{T}"/>.
        /// </summary>
        /// 
        public IPotentialFunction<T> Function { get; set; }

        /// <summary>
        ///   Gets or sets the model being trained.
        /// </summary>
        /// 
        public HiddenConditionalRandomField<T> Model { get; set; }


        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        public HiddenConditionalRandomField<T> Learn(T[][] x, int[] y, double[] weights = null)
        {
            if (weights != null)
                throw new ArgumentException(Accord.Properties.Resources.NotSupportedWeights, "weights");

            if (Model == null)
                Model = Create(x, y);

            InnerRun(x, y);

            return Model;
        }

        /// <summary>
        ///   Creates an instance of the model to be learned. Inheritors of this abstract 
        ///   class must define this method so new models can be created from the training data.
        /// </summary>
        /// 
        protected virtual HiddenConditionalRandomField<T> Create(T[][] x, int[] y)
        {
            if (Function == null)
                throw new InvalidOperationException("Please set the Function property with the PotentialFunction would like to use.");

            return new HiddenConditionalRandomField<T>(Function);
        }

        /// <summary>
        ///   Inheritors should implement the actual learning algorithm in this method.
        /// </summary>
        /// 
        protected abstract double InnerRun(T[][] x, int[] y);
    }
}
