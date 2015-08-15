// AForge Neural Net Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2012
// contacts@aforgenet.com
//

namespace AForge.Neuro.Learning
{
    using System;

    /// <summary>
    /// Unsupervised learning interface.
    /// </summary>
    /// 
    /// <remarks><para>The interface describes methods, which should be implemented
    /// by all unsupervised learning algorithms. Unsupervised learning is such
    /// type of learning algorithms, where system's desired output is not known on
    /// the learning stage. Given sample input values, it is expected, that
    /// system will organize itself in the way to find similarities betweed provided
    /// samples.</para></remarks>
    /// 
    public interface IUnsupervisedLearning
    {
        /// <summary>
        /// Runs learning iteration.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// 
        /// <returns>Returns learning error.</returns>
        /// 
        double Run( double[] input );

        /// <summary>
        /// Runs learning epoch.
        /// </summary>
        /// 
        /// <param name="input">Array of input vectors.</param>
        ///
        /// <returns>Returns sum of learning errors.</returns>
        /// 
        double RunEpoch( double[][] input );
    }
}
