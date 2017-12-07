// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// AForge Machine Learning Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007-2008
// andrew.kirillov@gmail.com
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

namespace Accord.MachineLearning
{
    using System;
    using Accord.Compat;

    /// <summary>
    /// Epsilon greedy exploration policy.
    /// </summary>
    /// 
    /// <remarks><para>The class implements epsilon greedy exploration policy. According to the policy,
    /// the best action is chosen with probability <b>1-epsilon</b>. Otherwise,
    /// with probability <b>epsilon</b>, any other action, except the best one, is
    /// chosen randomly.</para>
    /// 
    /// <para>According to the policy, the epsilon value is known also as exploration rate.</para>
    /// </remarks>
    /// 
    /// <seealso cref="RouletteWheelExploration"/>
    /// <seealso cref="BoltzmannExploration"/>
    /// <seealso cref="TabuSearchExploration"/>
    /// 
    [Serializable]
    public class EpsilonGreedyExploration : IExplorationPolicy
    {
        // exploration rate
        private double epsilon;

        // random number generator
        [NonSerialized]
        private Random rand = Accord.Math.Random.Generator.Random;

        /// <summary>
        /// Epsilon value (exploration rate), [0, 1].
        /// </summary>
        /// 
        /// <remarks><para>The value determines the amount of exploration driven by the policy.
        /// If the value is high, then the policy drives more to exploration - choosing random
        /// action, which excludes the best one. If the value is low, then the policy is more
        /// greedy - choosing the beat so far action.
        /// </para></remarks>
        /// 
        public double Epsilon
        {
            get { return epsilon; }
            set
            {
                if (value < 0 || value > 1.0)
                    throw new ArgumentOutOfRangeException("Epsilon should be between 0 and 1.");
                epsilon = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EpsilonGreedyExploration"/> class.
        /// </summary>
        /// 
        /// <param name="epsilon">Epsilon value (exploration rate).</param>
        /// 
        public EpsilonGreedyExploration(double epsilon)
        {
            Epsilon = epsilon;
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

            // find the best action (greedy)
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

            // try to do exploration
            if (rand.NextDouble() < epsilon)
            {
                int randomAction = rand.Next(actionsCount - 1);

                if (randomAction >= greedyAction)
                    randomAction++;

                return randomAction;
            }

            return greedyAction;
        }
    }
}
