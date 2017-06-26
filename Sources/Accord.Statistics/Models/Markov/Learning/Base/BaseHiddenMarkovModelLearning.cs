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
    using System;
    using Accord.Math;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.MachineLearning;
    using Accord.Statistics.Models.Markov.Topology;

    /// <summary>
    ///   Base class for implementations of the Baum-Welch learning algorithm.
    ///   This class cannot be instantiated.
    /// </summary>
    /// 
    public abstract class BaseHiddenMarkovModelLearning<TModel, TObservation> : ParallelLearningBase
    {
        private int numberOfStates;
        private ITopology topology;

        /// <summary>
        ///   Gets or sets the number of states to be used when this
        ///   learning algorithm needs to create new models.
        /// </summary>
        /// 
        /// <value>The number of states.</value>
        /// 
        public int NumberOfStates
        {
            get { return numberOfStates; }
            set
            {
                numberOfStates = value;
                this.topology = null;
            }
        }

        /// <summary>
        ///   Gets or sets the state transition topology to be used when this learning 
        ///   algorithm needs to create new models. Default is <see cref="Forward"/>.
        /// </summary>
        /// 
        /// <value>The topology to be used when this learning algorithm needs to create a new model.</value>
        /// 
        public ITopology Topology
        {
            get
            {
                if (topology != null)
                    return topology;

                if (numberOfStates == 0)
                    throw new InvalidOperationException("No topology has been set and no NumberOfStates has been set. Please set at least one of those properties before continuing.");

                this.topology = new Ergodic(numberOfStates);
                return this.topology;
            }
            set
            {
                this.topology = value;
                this.numberOfStates = value.States;
            }
        }

        /// <summary>
        ///   Gets or sets the model being trained.
        /// </summary>
        /// 
        public TModel Model
        {
            get; set;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="BaseHiddenMarkovModelLearning{TModel, TObservation}"/> class.
        /// </summary>
        /// 
        /// <param name="model">The model to be learned.</param>
        /// 
        public BaseHiddenMarkovModelLearning(TModel model)
        {
            this.Model = model;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="BaseHiddenMarkovModelLearning{TModel, TObservation}"/> class.
        /// </summary>
        /// 
        public BaseHiddenMarkovModelLearning()
        {
        }


        /// <summary>
        ///   Creates an instance of the model to be learned. Inheritors of this abstract 
        ///   class must define this method so new models can be created from the training data.
        /// </summary>
        /// 
        protected abstract TModel Create(TObservation[][] x);

    }
}
