// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// AForge Machine Learning Library
// AForge.NET framework
//
// Copyright � Andrew Kirillov, 2007-2008
// andrew.kirillov@gmail.com
//
// Copyright � C�sar Souza, 2009-2017
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

namespace Accord.MachineLearning
{
    using System;
    using Accord.Compat;

    /// <summary>
    /// Boltzmann distribution exploration policy.
    /// </summary>
    /// 
    /// <remarks><para>The class implements exploration policy base on Boltzmann distribution.
    /// Acording to the policy, action <b>a</b> at state <b>s</b> is selected with the next probability:</para>
    /// <code lang="none">
    ///                   exp( Q( s, a ) / t )
    /// p( s, a ) = -----------------------------
    ///              SUM( exp( Q( s, b ) / t ) )
    ///               b
    /// </code>
    /// <para>where <b>Q(s, a)</b> is action's <b>a</b> estimation (usefulness) at state <b>s</b> and
    /// <b>t</b> is <see cref="Temperature"/>.</para>
    /// </remarks>
    /// 
    /// <seealso cref="RouletteWheelExploration"/>
    /// <seealso cref="EpsilonGreedyExploration"/>
    /// <seealso cref="TabuSearchExploration"/>
    /// 
    [Serializable]
    public class BoltzmannExploration : IExplorationPolicy
    {
        // temperature parameter of Boltzmann distribution
        private double temperature;

        // random number generator
        [NonSerialized]
        private Random rand = Accord.Math.Random.Generator.Random;

        /// <summary>
        ///   Temperature parameter of Boltzmann distribution. Should be greater than 0.
        /// </summary>
        /// 
        /// <remarks><para>The property sets the balance between exploration and greedy actions.
        /// If temperature is low, then the policy tends to be more greedy.</para></remarks>
        /// 
        public double Temperature
        {
            get { return temperature; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Temperature should be greather than zero.");
                temperature = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoltzmannExploration"/> class.
        /// </summary>
        /// 
        /// <param name="temperature">Temperature parameter of Boltzmann distribution.</param>
        /// 
        public BoltzmannExploration(double temperature)
        {
            Temperature = temperature;
        }

        /// <summary>
        /// Choose an action.
        /// </summary>
        /// 
        /// <param name="actionEstimates">Action estimates.</param>
        /// 
        /// <returns>Returns selected action.</returns>
        /// 
        /// <remarks>The method chooses an action depending on the provided estimates. The
        /// estimates can be any sort of estimate, which values usefulness of the action
        /// (expected summary reward, discounted reward, etc).</remarks>
        /// 
        public int ChooseAction(double[] actionEstimates)
        {
            // actions count
            int actionsCount = actionEstimates.Length;

            // action probabilities
            double[] actionProbabilities = new double[actionsCount];

            // actions sum
            double sum = 0, probabilitiesSum = 0;

            for (int i = 0; i < actionsCount; i++)
            {
                double actionProbability = Math.Exp(actionEstimates[i] / temperature);

                actionProbabilities[i] = actionProbability;
                probabilitiesSum += actionProbability;
            }

            if ((double.IsInfinity(probabilitiesSum)) || (probabilitiesSum == 0))
            {
                // do greedy selection in the case of infinity or zero
                double maxReward = actionEstimates[0];
                int greedyAction = 0;

                for (int i = 1; i < actionsCount; i++)
                {
                    if (actionEstimates[i] > maxReward)
                    {
                        maxReward = actionEstimates[i];
                        greedyAction = i;
                    }
                }
                return greedyAction;
            }

            // get random number, which determines which action to choose
            double actionRandomNumber = rand.NextDouble();

            for (int i = 0; i < actionsCount; i++)
            {
                sum += actionProbabilities[i] / probabilitiesSum;
                if (actionRandomNumber <= sum)
                    return i;
            }

            return actionsCount - 1;
        }
    }
}
