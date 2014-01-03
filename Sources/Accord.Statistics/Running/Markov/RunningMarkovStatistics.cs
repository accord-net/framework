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

namespace Accord.Statistics.Running
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Accord.Statistics.Models.Markov;
    using Accord.Math;

    /// <summary>
    ///   Hidden Markov Model filter.
    /// </summary>
    /// 
    [Serializable]
    public class RunningMarkovStatistics : IRunning<int>, IRunningMarkovStatistics
    {

        private BaseRunningMarkovStatistics _base;

        private double[] previous;
        private double[] next;

        /// <summary>
        ///   Gets the <see cref="HiddenMarkovModel"/> used in this filter.
        /// </summary>
        /// 
        public HiddenMarkovModel Model { get; private set; }


        /// <summary>
        ///   Creates a new <see cref="RunningMarkovStatistics"/>.
        /// </summary>
        /// 
        /// <param name="model">The hidden Markov model to use in this filter.</param>
        /// 
        public RunningMarkovStatistics(HiddenMarkovModel model)
        {
            this.Model = model;
            this.previous = new double[model.States];
            this.next = new double[model.States];

            this._base = new BaseRunningMarkovStatistics(model);
        }

        /// <summary>
        ///   Registers the occurrence of a value.
        /// </summary>
        /// 
        /// <param name="value">The value to be registered.</param>
        /// 
        public void Push(int value)
        {
            if (!Started)
            {
                for (int i = 0; i < Current.Length; i++)
                    Current[i] = Model.Probabilities[i] + Model.Emissions[i, value];
                Started = true;
            }
            else
            {
                for (int i = 0; i < Current.Length; i++)
                    previous[i] = Current[i];

                for (int i = 0; i < Current.Length; i++)
                {
                    double sum = Double.NegativeInfinity;
                    for (int j = 0; j < previous.Length; j++)
                        sum = Special.LogSum(sum, previous[j] + Model.Transitions[j, i]);
                    Current[i] = sum + Model.Emissions[i, value];
                }
            }

            _base.Invalidate();
        }

        /// <summary>
        ///   Checks the classification after the insertion
        ///   of a new value without registering this value.
        /// </summary>
        /// 
        /// <param name="value">The value to be checked.</param>
        /// 
        public double Peek(int value)
        {
            if (!Started)
            {
                for (int i = 0; i < Current.Length; i++)
                    next[i] = Model.Probabilities[i] + Model.Emissions[i, value];
            }
            else
            {
                for (int i = 0; i < Current.Length; i++)
                {
                    double sum = Double.NegativeInfinity;
                    for (int j = 0; j < previous.Length; j++)
                        sum = Special.LogSum(sum, Current[j] + Model.Transitions[j, i]);
                    next[i] = sum + Model.Emissions[i, value];
                }
            }

            double logLikelihood = Double.NegativeInfinity;
            for (int i = 0; i < next.Length; i++)
                logLikelihood = Special.LogSum(logLikelihood, next[i]);

            return logLikelihood;
        }


        /// <summary>
        ///   Gets whether the model has been initialized or not.
        /// </summary>
        /// 
        public bool Started
        {
            get { return _base.Started; }
            set { _base.Started = value; }
        }

        /// <summary>
        ///   Gets the current vector of probabilities of being in each state.
        /// </summary>
        /// 
        public double[] Current
        {
            get { return _base.Current; }
        }

        /// <summary>
        ///   Gets the current most likely state (in the Viterbi path).
        /// </summary>
        /// 
        public int CurrentState
        {
            get { return _base.CurrentState; }
        }

        /// <summary>
        ///   Gets the current Viterbi probability
        ///   (along the most likely path).
        /// </summary>
        /// 
        public double LogViterbi
        {
            get { return _base.LogViterbi; }
        }

        /// <summary>
        ///   Gets the current Forward probability
        ///   (along all possible paths).
        /// </summary>
        /// 
        public double LogForward
        {
            get { return _base.LogForward; }
        }

        /// <summary>
        ///   Clears this instance.
        /// </summary>
        /// 
        public void Clear()
        {
            _base.Clear();
        }

    }
}
