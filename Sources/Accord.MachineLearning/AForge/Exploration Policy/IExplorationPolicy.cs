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
    /// Exploration policy interface.
    /// </summary>
    /// 
    /// <remarks>The interface describes exploration policies, which are used in Reinforcement
    /// Learning to explore state space.</remarks>
    /// 
    public interface IExplorationPolicy
    {
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
        int ChooseAction( double[] actionEstimates );
    }
}
