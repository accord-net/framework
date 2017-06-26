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

namespace Accord.Statistics.Models.Markov.Learning
{
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using System;

    /// <summary>
    ///   Baum-Welch learning algorithms for learning Hidden Markov Models.
    /// </summary>
    /// 
    /// <typeparam name="TDistribution">The type of the emission distributions in the model.</typeparam>
    /// <typeparam name="TObservation">The type of the observations (i.e. int for a discrete model).</typeparam>
    /// <typeparam name="TOptions">The type of fitting options accepted by this distribution.</typeparam>
    /// 
    /// <remarks>
    ///   Please see the <see cref="BaumWelchLearning{TDistribution, TObservation}"/> documentation page for
    ///   the actual documentation of this class, including examples.
    /// </remarks>
    /// 
    public class BaumWelchLearning<TDistribution, TObservation, TOptions> :
        BaseBaumWelchLearningOptions<HiddenMarkovModel<TDistribution, TObservation>, TDistribution, TObservation, TOptions>,
        IConvergenceLearning
        where TDistribution : IFittableDistribution<TObservation, TOptions>
        where TOptions : class, IFittingOptions, new()
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BaumWelchLearning{TDistribution, TObservation, TOptions}"/> class.
        /// </summary>
        /// 
        public BaumWelchLearning()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaumWelchLearning{TDistribution, TObservation, TOptions}"/> class.
        /// </summary>
        /// 
        /// <param name="model">The model to be learned.</param>
        /// 
        public BaumWelchLearning(HiddenMarkovModel<TDistribution, TObservation> model)
            : base(model)
        {
        }

        /// <summary>
        ///   Creates an instance of the model to be learned. Inheritors of this abstract 
        ///   class must define this method so new models can be created from the training data.
        /// </summary>
        /// 
        protected override HiddenMarkovModel<TDistribution, TObservation> Create(TObservation[][] x)
        {
            if (Topology == null)
                throw new InvalidOperationException("Please set the Topology property before trying to learn a new hidden Markov model.");
            if (Emissions == null)
                throw new InvalidOperationException("Please set the Emissions property before trying to learn a new hidden Markov model.");
            return new HiddenMarkovModel<TDistribution, TObservation>(Topology, Emissions);
        }

    }
}
