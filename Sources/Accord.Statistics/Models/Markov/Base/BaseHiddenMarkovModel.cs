// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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

namespace Accord.Statistics.Models.Markov
{
    using System;
    using Accord.Statistics.Models.Markov.Topology;

    /// <summary>
    ///   Base class for Hidden Markov Models. This class cannot
    ///   be instantiated.
    /// </summary>
    /// 
    [Serializable]
    public abstract class BaseHiddenMarkovModel
    {

        private int states;  // number of states
        private object tag;


        // Model is defined as M = (A, B, pi)
        private double[,] logA; // Transition probabilities
        private double[] logPi; // Initial state probabilities



        /// <summary>
        ///   Constructs a new Hidden Markov Model.
        /// </summary>
        /// 
        protected BaseHiddenMarkovModel(ITopology topology)
        {
            this.states = topology.Create(true, out logA, out logPi);
        }



        /// <summary>
        ///   Gets the number of states of this model.
        /// </summary>
        /// 
        public int States
        {
            get { return this.states; }
        }

        /// <summary>
        ///   Gets the log-initial probabilities <c>log(pi)</c> for this model.
        /// </summary>
        /// 
        public double[] Probabilities
        {
            get { return this.logPi; }
        }

        /// <summary>
        ///   Gets the log-transition matrix <c>log(A)</c> for this model.
        /// </summary>
        /// 
        public double[,] Transitions
        {
            get { return this.logA; }
        }

        /// <summary>
        ///   Gets or sets a user-defined tag associated with this model.
        /// </summary>
        /// 
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }


    }
}
