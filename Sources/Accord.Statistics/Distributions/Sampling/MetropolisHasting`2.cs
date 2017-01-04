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
    public class MetropolisHasting<TObservation, TProposalDistribution>
        : MetropolisHasting<TObservation>
        where TProposalDistribution : ISampleableDistribution<TObservation[]>
    {
        private TProposalDistribution proposal;

        /// <summary>
        ///   Gets or sets the move proposal distribution.
        /// </summary>
        /// 
        public new TProposalDistribution Proposal
        {
            get { return proposal; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MetropolisHasting"/> algorithm.
        /// </summary>
        /// 
        /// <param name="dimensions">The number of dimensions in each observation.</param>
        /// <param name="logDensity">A function specifying the log probability density of the distribution to be sampled.</param>
        /// <param name="proposal">The proposal distribution that is used to generate new parameter samples to be explored.</param>
        /// 
        public MetropolisHasting(int dimensions, Func<TObservation[], double> logDensity, TProposalDistribution proposal)
        {
            Initialize(dimensions, logDensity, proposal);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MetropolisHasting{TObservation, TProposalDistribution}"/> class.
        /// </summary>
        /// 
        protected MetropolisHasting()
        {

        }

        /// <summary>
        ///   Initializes the algorithm.
        /// </summary>
        /// 
        protected void Initialize(int dimensions, Func<TObservation[], double> logDensity, TProposalDistribution proposal)
        {
            this.proposal = proposal;
            Initialize(dimensions, logDensity, generate);
        }

        private TObservation[] generate(TObservation[] current, TObservation[] next)
        {
            return proposal.Generate(result: next);
        }
    }

}
