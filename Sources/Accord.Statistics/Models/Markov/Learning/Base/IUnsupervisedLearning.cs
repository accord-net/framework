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

namespace Accord.Statistics.Models.Markov.Learning
{
    using System;

    /// <summary>
    ///   Common interface for unsupervised learning algorithms for hidden
    ///   Markov models such as the <see cref="BaumWelchLearning">Baum-Welch
    ///   learning</see> and the <see cref="ViterbiLearning">Viterbi learning
    ///   </see> algorithms.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In the context of <see cref="HiddenMarkovModel">hidden Markov models</see>, 
    ///   unsupervised algorithms are algorithms which consider that the sequence
    ///   of states in a system is hidden, and just the system's outputs can be seen 
    ///   (or are known) during training. This is in contrast with <see cref="ISupervisedLearning">
    ///   supervised learning algorithms</see> such as the <see cref="MaximumLikelihoodLearning">
    ///   Maximum Likelihood (MLE)</see>, which consider that both the sequence of observations
    ///   and the sequence of states are observable during training.
    /// </para>
    /// </remarks>
    /// 
    /// <see cref="BaumWelchLearning"/>
    /// <see cref="BaumWelchLearning{TDistribution}"/>
    /// <see cref="ViterbiLearning"/>
    /// <see cref="ViterbiLearning{TDistribution}"/>
    /// 
    public interface IUnsupervisedLearning
    {

        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        /// <remarks>
        ///   Learning problem. Given some training observation sequences O = {o1, o2, ..., oK}
        ///   and general structure of HMM (numbers of hidden and visible states), determine
        ///   HMM parameters M = (A, B, pi) that best fit training data. 
        /// </remarks>
        /// 
        /// <param name="observations">The observations.</param>
        /// 
        double Run(Array[] observations);

    }

    /// <summary>
    ///   Common interface for unsupervised learning algorithms for hidden
    ///   Markov models such as the <see cref="BaumWelchLearning">Baum-Welch
    ///   learning</see> and the <see cref="ViterbiLearning">Viterbi learning
    ///   </see> algorithms.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In the context of <see cref="HiddenMarkovModel">hidden Markov models</see>, 
    ///   unsupervised algorithms are algorithms which consider that the sequence
    ///   of states in a system is hidden, and just the system's outputs can be seen 
    ///   (or are known) during training. This is in contrast with <see cref="ISupervisedLearning">
    ///   supervised learning algorithms</see> such as the <see cref="MaximumLikelihoodLearning">
    ///   Maximum Likelihood (MLE)</see>, which consider that both the sequence of observations
    ///   and the sequence of states are observable during training.
    /// </para>
    /// </remarks>
    /// 
    /// <see cref="BaumWelchLearning"/>
    /// <see cref="BaumWelchLearning{TDistribution}"/>
    /// <see cref="ViterbiLearning"/>
    /// <see cref="ViterbiLearning{TDistribution}"/>
    /// 
    public interface IUnsupervisedLearning<T> : IUnsupervisedLearning
    {

        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        /// <remarks>
        ///   Learning problem. Given some training observation sequences O = {o1, o2, ..., oK}
        ///   and general structure of HMM (numbers of hidden and visible states), determine
        ///   HMM parameters M = (A, B, pi) that best fit training data. 
        /// </remarks>
        /// 
        /// <param name="observations">The observations.</param>
        /// 
        double Run(params T[] observations);

    }

    /// <summary>
    ///   Common interface for unsupervised learning algorithms for hidden
    ///   Markov models which support for weighted training samples.
    /// </summary>
    /// 
    public interface IWeightedUnsupervisedLearning
    {

        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        /// <remarks>
        ///   Learning problem. Given some training observation sequences O = {o1, o2, ..., oK}
        ///   and general structure of HMM (numbers of hidden and visible states), determine
        ///   HMM parameters M = (A, B, pi) that best fit training data. 
        /// </remarks>
        /// 
        double Run(Array[] observations, double[] weights);

    }
}
