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
    /// Tabu search exploration policy.
    /// </summary>
    /// 
    /// <remarks>The class implements simple tabu search exploration policy,
    /// allowing to set certain actions as tabu for a specified amount of
    /// iterations. The actual exploration and choosing from non-tabu actions
    /// is done by <see cref="BasePolicy">base exploration policy</see>.</remarks>
    /// 
    /// <seealso cref="BoltzmannExploration"/>
    /// <seealso cref="EpsilonGreedyExploration"/>
    /// <seealso cref="RouletteWheelExploration"/>
    /// 
    public class TabuSearchExploration : IExplorationPolicy
    {
        // total actions count
        private int actions;
        // list of tabu actions
        private int[] tabuActions = null;
        // base exploration policy
        private IExplorationPolicy basePolicy;

        /// <summary>
        /// Base exploration policy.
        /// </summary>
        /// 
        /// <remarks>Base exploration policy is the policy, which is used
        /// to choose from non-tabu actions.</remarks>
        /// 
        public IExplorationPolicy BasePolicy
        {
            get { return basePolicy; }
            set { basePolicy = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TabuSearchExploration"/> class.
        /// </summary>
        /// 
        /// <param name="actions">Total actions count.</param>
        /// <param name="basePolicy">Base exploration policy.</param>
        /// 
        public TabuSearchExploration( int actions, IExplorationPolicy basePolicy )
        {
            this.actions = actions;
            this.basePolicy = basePolicy;

            // create tabu list
            tabuActions = new int[actions];
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
        /// (expected summary reward, discounted reward, etc). The action is choosed from
        /// non-tabu actions only.</remarks>
        /// 
        public int ChooseAction( double[] actionEstimates )
        {
            // get amount of non-tabu actions
            int nonTabuActions = actions;
            for ( int i = 0; i < actions; i++ )
            {
                if ( tabuActions[i] != 0 )
                {
                    nonTabuActions--;
                }
            }

            // allowed actions
            double[] allowedActionEstimates = new double[nonTabuActions];
            int[]    allowedActionMap = new int[nonTabuActions];

            for ( int i = 0, j = 0; i < actions; i++ )
            {
                if ( tabuActions[i] == 0 )
                {
                    // allowed action
                    allowedActionEstimates[j] = actionEstimates[i];
                    allowedActionMap[j] = i;
                    j++;
                }
                else
                {
                    // decrease tabu time of tabu action
                    tabuActions[i]--;
                }
            }

            return allowedActionMap[basePolicy.ChooseAction( allowedActionEstimates )]; ;
        }

        /// <summary>
        /// Reset tabu list.
        /// </summary>
        /// 
        /// <remarks>Clears tabu list making all actions allowed.</remarks>
        /// 
        public void ResetTabuList( )
        {
            Array.Clear( tabuActions, 0, actions );
        }

        /// <summary>
        /// Set tabu action.
        /// </summary>
        /// 
        /// <param name="action">Action to set tabu for.</param>
        /// <param name="tabuTime">Tabu time in iterations.</param>
        /// 
        public void SetTabuAction( int action, int tabuTime )
        {
            tabuActions[action] = tabuTime;
        }
    }
}
