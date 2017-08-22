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

#pragma warning disable 612, 618


namespace Accord.Statistics.Models.Fields.Learning
{
    using System;

    /// <summary>
    ///   Common interface for Hidden Conditional Random Fields learning algorithms.
    /// </summary>
    ///
    /// <example>
    /// <para>
    ///   For an example on how to learn Hidden Conditional Random Fields, please see the
    ///   <see cref="HiddenResilientGradientLearning{T}">Hidden Resilient Gradient Learning</see>
    ///   page. All learning algorithms can be utilized in a similar manner.</para>
    /// </example>
    /// 
    /// 
    [Obsolete("Please use ISupervisedLearning<HiddenConditionalRandomField<T>, T[], int> instead.")]
    public interface IHiddenConditionalRandomFieldLearning<T>
    {

        /// <summary>
        ///   Runs one iteration of the learning algorithm with the
        ///   specified input training observation and corresponding
        ///   output label.
        /// </summary>
        /// 
        /// <param name="observations">The training observations.</param>
        /// <param name="output">The observation labels.</param>
        /// 
        /// <returns>The error in the last iteration.</returns>
        /// 
        double Run(T[] observations, int output);

        /// <summary>
        ///   Runs one iteration of learning algorithm with the specified
        ///   input training observations and corresponding output labels.
        /// </summary>
        /// 
        /// <param name="observations">The training observations.</param>
        /// <param name="outputs">The observations' labels.</param>
        /// 
        /// <returns>The error in the last iteration.</returns>
        /// 
        double RunEpoch(T[][] observations, int[] outputs);

        /// <summary>
        ///   Runs the learning algorithm with the specified input
        ///   training observation and corresponding output label
        ///   until convergence.
        /// </summary>
        /// 
        /// <param name="observations">The training observations.</param>
        /// <param name="outputs">The observations' labels.</param>
        /// 
        /// <returns>The error in the last iteration.</returns>
        /// 
        double Run(T[][] observations, int[] outputs);

    }
}
