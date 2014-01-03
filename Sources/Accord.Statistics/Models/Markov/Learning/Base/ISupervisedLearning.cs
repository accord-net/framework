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
    ///   Common interface for supervised learning algorithms for <see cref="HiddenMarkovModel">
    ///   hidden Markov models</see> such as the <see cref="MaximumLikelihoodLearning">
    ///   Maximum Likelihood (MLE)</see> learning algorithm.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In the context of <see cref="HiddenMarkovModel">hidden Markov models</see>, 
    ///   supervised algorithms are algorithms which consider that both the sequence
    ///   of observations and the sequence of states are visible (or known) during
    ///   training. This is in contrast with <see cref="IUnsupervisedLearning">
    ///   unsupervised learning algorithms</see> such as the <see cref="BaumWelchLearning">
    ///   Baum-Welch</see>, which consider that the sequence of states is hidden.
    /// </para>
    /// </remarks>
    ///
    /// <see cref="MaximumLikelihoodLearning"/>
    /// <see cref="MaximumLikelihoodLearning{TDistribution}"/>
    /// 
    public interface ISupervisedLearning
    {

        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        /// <remarks>
        ///   Supervised learning problem. Given some training observation sequences 
        ///   O = {o1, o2, ..., oK} and sequence of hidden states H = {h1, h2, ..., hK}
        ///   and general structure of HMM (numbers of hidden and visible states), 
        ///   determine HMM parameters M = (A, B, pi) that best fit training data. 
        /// </remarks>
        /// 
        double Run(Array[] observations, int[][] paths);

    }
}
