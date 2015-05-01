// AForge Machine Learning Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007-2008
// andrew.kirillov@gmail.com
//

namespace AForge.MachineLearning
{
    using System;

    /// <summary>
    /// Roulette wheel exploration policy.
    /// </summary>
    /// 
    /// <remarks><para>The class implements roulette whell exploration policy. Acording to the policy,
    /// action <b>a</b> at state <b>s</b> is selected with the next probability:</para>
    /// <code lang="none">
    ///                   Q( s, a )
    /// p( s, a ) = ------------------
    ///              SUM( Q( s, b ) )
    ///               b
    /// </code>
    /// <para>where <b>Q(s, a)</b> is action's <b>a</b> estimation (usefulness) at state <b>s</b>.</para>
    /// 
    /// <para><note>The exploration policy may be applied only in cases, when action estimates (usefulness)
    /// are represented with positive value greater then 0.</note></para>
    /// </remarks>
    /// 
    /// <seealso cref="BoltzmannExploration"/>
    /// <seealso cref="EpsilonGreedyExploration"/>
    /// <seealso cref="TabuSearchExploration"/>
    /// 
    public class RouletteWheelExploration : IExplorationPolicy
    {
        // random number generator
        private Random rand = new Random( );

        /// <summary>
        /// Initializes a new instance of the <see cref="RouletteWheelExploration"/> class.
        /// </summary>
        /// 
        public RouletteWheelExploration( ) { }

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
        public int ChooseAction( double[] actionEstimates )
        {
            // actions count
            int actionsCount = actionEstimates.Length;
            // actions sum
            double sum = 0, estimateSum = 0;

            for ( int i = 0; i < actionsCount; i++ )
            {
                estimateSum += actionEstimates[i];
            }

            // get random number, which determines which action to choose
            double actionRandomNumber = rand.NextDouble( );

            for ( int i = 0; i < actionsCount; i++ )
            {
                sum += actionEstimates[i] / estimateSum;
                if ( actionRandomNumber <= sum )
                    return i;
            }

            return actionsCount - 1;
        }
    }
}
