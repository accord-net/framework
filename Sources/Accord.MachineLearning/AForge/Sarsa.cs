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
    /// Sarsa learning algorithm.
    /// </summary>
    /// 
    /// <remarks>The class provides implementation of Sarse algorithm, known as
    /// on-policy Temporal Difference control.</remarks>
    /// 
    /// <seealso cref="QLearning"/>
    /// 
    public class Sarsa
    {
        // amount of possible states
        private int states;
        // amount of possible actions
        private int actions;
        // q-values
        private double[][] qvalues;
        // exploration policy
        private IExplorationPolicy explorationPolicy;

        // discount factor
        private double discountFactor = 0.95;
        // learning rate
        private double learningRate = 0.25;

        /// <summary>
        /// Amount of possible states.
        /// </summary>
        /// 
        public int StatesCount
        {
            get { return states; }
        }

        /// <summary>
        /// Amount of possible actions.
        /// </summary>
        /// 
        public int ActionsCount
        {
            get { return actions; }
        }

        /// <summary>
        /// Exploration policy.
        /// </summary>
        /// 
        /// <remarks>Policy, which is used to select actions.</remarks>
        /// 
        public IExplorationPolicy ExplorationPolicy
        {
            get { return explorationPolicy; }
            set { explorationPolicy = value; }
        }

        /// <summary>
        /// Learning rate, [0, 1].
        /// </summary>
        /// 
        /// <remarks>The value determines the amount of updates Q-function receives
        /// during learning. The greater the value, the more updates the function receives.
        /// The lower the value, the less updates it receives.</remarks>
        /// 
        public double LearningRate
        {
            get { return learningRate; }
            set { learningRate = Math.Max( 0.0, Math.Min( 1.0, value ) ); }
        }

        /// <summary>
        /// Discount factor, [0, 1].
        /// </summary>
        /// 
        /// <remarks>Discount factor for the expected summary reward. The value serves as
        /// multiplier for the expected reward. So if the value is set to 1,
        /// then the expected summary reward is not discounted. If the value is getting
        /// smaller, then smaller amount of the expected reward is used for actions'
        /// estimates update.</remarks>
        /// 
        public double DiscountFactor
        {
            get { return discountFactor; }
            set { discountFactor = Math.Max( 0.0, Math.Min( 1.0, value ) ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sarsa"/> class.
        /// </summary>
        /// 
        /// <param name="states">Amount of possible states.</param>
        /// <param name="actions">Amount of possible actions.</param>
        /// <param name="explorationPolicy">Exploration policy.</param>
        /// 
        /// <remarks>Action estimates are randomized in the case of this constructor
        /// is used.</remarks>
        /// 
        public Sarsa( int states, int actions, IExplorationPolicy explorationPolicy ) :
            this( states, actions, explorationPolicy, true )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sarsa"/> class.
        /// </summary>
        /// 
        /// <param name="states">Amount of possible states.</param>
        /// <param name="actions">Amount of possible actions.</param>
        /// <param name="explorationPolicy">Exploration policy.</param>
        /// <param name="randomize">Randomize action estimates or not.</param>
        /// 
        /// <remarks>The <b>randomize</b> parameter specifies if initial action estimates should be randomized
        /// with small values or not. Randomization of action values may be useful, when greedy exploration
        /// policies are used. In this case randomization ensures that actions of the same type are not chosen always.</remarks>
        /// 
        public Sarsa( int states, int actions, IExplorationPolicy explorationPolicy, bool randomize )
        {
            this.states = states;
            this.actions = actions;
            this.explorationPolicy = explorationPolicy;

            // create Q-array
            qvalues = new double[states][];
            for ( int i = 0; i < states; i++ )
            {
                qvalues[i] = new double[actions];
            }

            // do randomization
            if ( randomize )
            {
                Random rand = new Random( );

                for ( int i = 0; i < states; i++ )
                {
                    for ( int j = 0; j < actions; j++ )
                    {
                        qvalues[i][j] = rand.NextDouble( ) / 10;
                    }
                }
            }
        }

        /// <summary>
        /// Get next action from the specified state.
        /// </summary>
        /// 
        /// <param name="state">Current state to get an action for.</param>
        /// 
        /// <returns>Returns the action for the state.</returns>
        /// 
        /// <remarks>The method returns an action according to current
        /// <see cref="ExplorationPolicy">exploration policy</see>.</remarks>
        /// 
        public int GetAction( int state )
        {
            return explorationPolicy.ChooseAction( qvalues[state] );
        }

        /// <summary>
        /// Update Q-function's value for the previous state-action pair.
        /// </summary>
        /// 
        /// <param name="previousState">Curren state.</param>
        /// <param name="previousAction">Action, which lead from previous to the next state.</param>
        /// <param name="reward">Reward value, received by taking specified action from previous state.</param>
        /// <param name="nextState">Next state.</param>
        /// <param name="nextAction">Next action.</param>
        /// 
        /// <remarks>Updates Q-function's value for the previous state-action pair in
        /// the case if the next state is non terminal.</remarks>
        /// 
        public void UpdateState( int previousState, int previousAction, double reward, int nextState, int nextAction )
        {
            // previous state's action estimations
            double[] previousActionEstimations = qvalues[previousState];
            // update expexted summary reward of the previous state
            previousActionEstimations[previousAction] *= ( 1.0 - learningRate );
            previousActionEstimations[previousAction] += ( learningRate * ( reward + discountFactor * 
                                                           qvalues[nextState][nextAction] ) );

        }

        /// <summary>
        /// Update Q-function's value for the previous state-action pair.
        /// </summary>
        /// 
        /// <param name="previousState">Curren state.</param>
        /// <param name="previousAction">Action, which lead from previous to the next state.</param>
        /// <param name="reward">Reward value, received by taking specified action from previous state.</param>
        /// 
        /// <remarks>Updates Q-function's value for the previous state-action pair in
        /// the case if the next state is terminal.</remarks>
        /// 
        public void UpdateState( int previousState, int previousAction, double reward )
        {
            // previous state's action estimations
            double[] previousActionEstimations = qvalues[previousState];
            // update expexted summary reward of the previous state
            previousActionEstimations[previousAction] *= ( 1.0 - learningRate );
            previousActionEstimations[previousAction] += ( learningRate * reward );
        }
    }
}
