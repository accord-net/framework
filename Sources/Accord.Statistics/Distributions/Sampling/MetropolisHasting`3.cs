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

namespace Accord.Statistics.Distributions.Sampling
{
    using Accord.Math.Random;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Univariate;
    using System;

    /// <summary>
    ///   Metropolis-Hasting sampling algorithm.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://en.wikipedia.org/wiki/Metropolis%E2%80%93Hastings_algorithm">
    ///       Wikipedia, The Free Encyclopedia. Metropolis-Hastings algorithm. 
    ///       Available on: https://en.wikipedia.org/wiki/Metropolis%E2%80%93Hastings_algorithm </a></description></item>
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Joint_probability_distribution">
    ///       Darren Wilkinson, Metropolis Hastings MCMC when the proposal and target have differing support.  
    ///       Available on: https://darrenjw.wordpress.com/2012/06/04/metropolis-hastings-mcmc-when-the-proposal-and-target-have-differing-support/ </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    public class MetropolisHasting<TObservation, TProposalDistribution, TTargetDistribution>
        : MetropolisHasting<TObservation, TProposalDistribution>
        where TProposalDistribution : ISampleableDistribution<TObservation[]>
        where TTargetDistribution : IMultivariateDistribution<TObservation[]>
    {
        TTargetDistribution target;

        /// <summary>
        ///   Gets the target distribution whose samples must be generated.
        /// </summary>
        /// 
        public TTargetDistribution Target { get { return target; } }


        /// <summary>
        ///   Initializes a new instance of the <see cref="MetropolisHasting{TObservation, TProposalDistribution, TTargetDistribution}"/> class.
        /// </summary>
        /// 
        /// <param name="target">The target distribution whose samples should be generated.</param>
        /// <param name="proposal">The proposal distribution that is used to generate new parameter samples to be explored.</param>
        ///
        public MetropolisHasting(TTargetDistribution target, TProposalDistribution proposal)
        {
            this.target = target;
            this.Initialize(target.Dimension, target.LogProbabilityFunction, proposal);
        }
    }
}
