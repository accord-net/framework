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
    public class MetropolisHasting<T> : IRandomNumberGenerator<T[]>
    {
        Func<T[], double> logPdf;
        Func<T[], T[], T[]> proposal;
        T[] current;
        T[] next;

        double currentLogProb;

        int dimensions;
        int discard = 0; // steps to discard
        long steps = 0; // steps so far
        long accepts = 0; // steps accepted

        bool initialized = false;

        /// <summary>
        ///   Gets the last successfully generated observation.
        /// </summary>
        /// 
        public T[] Current { get { return current; } }

        /// <summary>
        ///   Gets or sets a factory method to create random number generators used in this instance.
        /// </summary>
        /// 
        public Random RandomSource { get; set; }

        /// <summary>
        ///   Gets the log-probability of the <see cref="Current">last successfully
        ///   generated sample</see>.
        /// </summary>
        /// 
        public double CurrentValue { get { return currentLogProb; } }

        /// <summary>
        ///   Gets the log-probability density function of the target distribution.
        /// </summary>
        /// 
        public Func<T[], double> LogProbabilityDensityFunction
        {
            get { return logPdf; }
        }

        /// <summary>
        ///   Gets or sets the move proposal distribution.
        /// </summary>
        /// 
        public Func<T[], T[], T[]> Proposal
        {
            get { return proposal; }
        }

        /// <summary>
        ///   Gets the acceptance rate for the proposals generated
        ///   by the <see cref="Proposal">proposal distribution</see>.
        /// </summary>
        /// 
        public double AcceptanceRate
        {
            get { return this.accepts / (double)this.steps; }
        }

        /// <summary>
        ///   Gets the number of dimensions in each observation.
        /// </summary>
        /// 
        public int NumberOfInputs
        {
            get { return dimensions; }
        }

        /// <summary>
        ///   Gets or sets how many initial samples will get discarded as part
        ///   of the initial thermalization (warm-up, initialization) process.
        /// </summary>
        /// 
        public int Discard
        {
            get { return discard; }
            set { discard = value; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MetropolisHasting"/> algorithm.
        /// </summary>
        /// 
        /// <param name="dimensions">The number of dimensions in each observation.</param>
        /// <param name="logDensity">A function specifying the log probability density of the distribution to be sampled.</param>
        /// <param name="proposal">The proposal distribution that is used to generate new parameter samples to be explored.</param>
        /// 
        public MetropolisHasting(int dimensions, Func<T[], double> logDensity, Func<T[], T[], T[]> proposal)
        {
            Initialize(dimensions, logDensity, proposal);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MetropolisHasting{T}"/> class.
        /// </summary>
        /// 
        protected MetropolisHasting()
        {
        }

        /// <summary>
        ///   Initializes the algorithm.
        /// </summary>
        /// 
        protected void Initialize(int dimensions, Func<T[], double> logDensity, Func<T[], T[], T[]> proposal)
        {
            this.dimensions = dimensions;
            this.current = new T[dimensions];
            this.next = new T[dimensions];
            this.logPdf = logDensity;
            this.proposal = proposal;
            this.RandomSource = Accord.Math.Random.Generator.Random;
        }


        /// <summary>
        ///   Attempts to generate a new observation from the target
        ///   distribution, storing its value in the <see cref="Current"/>
        ///   property.
        /// </summary>
        /// 
        /// <param name="sample">A new observation, if the method has succeed; otherwise, null.</param>
        /// 
        /// <returns>True if the sample was successfully generated; otherwise, returns false.</returns>
        /// 
        public bool TryGenerate(out T[] sample)
        {
            if (TryGenerate())
            {
                sample = current;
                return true;
            }
            else
            {
                sample = null;
                return false;
            }
        }

        /// <summary>
        ///   Attempts to generate a new observation from the target
        ///   distribution, storing its value in the <see cref="Current"/>
        ///   property.
        /// </summary>
        /// 
        /// <returns>True if the sample was successfully generated; otherwise, false.</returns>
        /// 
        public bool TryGenerate()
        {
            Random source = RandomSource;

            next = proposal(current, next);

            double pNext = logPdf(next);
            double logRatio = pNext - currentLogProb;
            steps++;

            if (Math.Log(source.NextDouble()) < logRatio)
            {
                var aux = current;
                current = next;
                next = aux;
                currentLogProb = pNext;
                accepts++;
                return true;
            }

            return false;
        }

        /// <summary>
        ///   Thermalizes the sample generation process, generating up to
        ///   <see cref="Discard"/> samples and discarding them. This step
        ///   is done automatically upon the first call to any of the 
        ///   <see cref="Generate()"/> functions.
        /// </summary>
        /// 
        public void WarmUp()
        {
            for (int i = 0; i < discard; i++)
                TryGenerate();
            initialized = true;
        }


        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// 
        /// <returns>
        ///   A random vector of observations drawn from this distribution.
        /// </returns>
        /// 
        public T[][] Generate(int samples)
        {
            return Generate(samples, new T[samples][]);
        }

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// 
        /// <returns>
        ///   A random vector of observations drawn from this distribution.
        /// </returns>
        /// 
        public T[][] Generate(int samples, T[][] result)
        {
            if (!initialized)
                WarmUp();

            for (int i = 0; i < samples; i++)
            {
                while (!TryGenerate())
                {
                }

                for (int j = 0; j < current.Length; j++)
                    result[i][j] = current[j];
            }

            return result;
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <returns>
        ///   A random observation drawn from this distribution.
        /// </returns>
        /// 
        public T[] Generate()
        {
            if (!initialized)
                WarmUp();

            while (!TryGenerate()) { }

            return current;
        }
    }

}
