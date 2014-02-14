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

namespace Accord.Statistics.Distributions.Fitting
{
    using System;
    using Accord.Math;
    using System.Threading.Tasks;

    /// <summary>
    ///   Expectation Maximization algorithm for mixture model fitting.
    /// </summary>
    /// 
    /// <typeparam name="TObservation">The type of the observations being fitted.</typeparam>
    /// 
    /// <remarks>
    ///   <para>
    ///   This class implements a generic version of the Expectation-Maximization algorithm
    ///   which can be used with both univariate or multivariate distribution types.</para>
    /// </remarks>
    /// 
    public class ExpectationMaximization<TObservation>
    {

        /// <summary>
        ///   Gets or sets the fitting options to be used 
        ///   when any of the component distributions need
        ///   to be estimated from the data.
        /// </summary>
        /// 
        public IFittingOptions InnerOptions { get; set; }

        /// <summary>
        ///   Gets or sets convergence properties for
        ///   the expectation-maximization algorithm.
        /// </summary>
        /// 
        public ISingleValueConvergence Convergence { get; set; }

        /// <summary>
        ///   Gets the current coefficient values.
        /// </summary>
        /// 
        public double[] Coefficients { get; private set; }

        /// <summary>
        ///   Gets the current component distribution values.
        /// </summary>
        /// 
        public IFittableDistribution<TObservation>[] Distributions { get; private set; }

        /// <summary>
        ///   Gets the responsibility of each input vector when estimating 
        ///   each of the component distributions, in the last iteration.
        /// </summary>
        /// 
        public double[][] Gamma { get; private set; }


        /// <summary>
        ///   Creates a new <see cref="ExpectationMaximization{TObservation}"/> algorithm.
        /// </summary>
        /// 
        /// <param name="coefficients">The initial coefficient values.</param>
        /// <param name="distributions">The initial component distributions.</param>
        /// 
        public ExpectationMaximization(double[] coefficients,
            IFittableDistribution<TObservation>[] distributions)
        {
            Coefficients = coefficients;
            Distributions = distributions;
            Convergence = new RelativeConvergence(0, 1e-3);
            Gamma = new double[coefficients.Length][];
        }

        /// <summary>
        ///   Estimates a mixture distribution for the given observations
        ///   using the Expectation-Maximization algorithm.
        /// </summary>
        /// 
        /// <param name="observations">The observations from the mixture distribution.</param>
        /// 
        /// <returns>The log-likelihood of the estimated mixture model.</returns>
        /// 
        public double Compute(TObservation[] observations)
        {
            return compute(observations, null);
        }

        /// <summary>
        ///   Estimates a mixture distribution for the given observations
        ///   using the Expectation-Maximization algorithm, assuming different
        ///   weights for each observation.
        /// </summary>
        /// 
        /// <param name="observations">The observations from the mixture distribution.</param>
        /// <param name="weights">The weight of each observation.</param>
        /// 
        /// <returns>The log-likelihood of the estimated mixture model.</returns>
        /// 
        public double Compute(TObservation[] observations, double[] weights)
        {
            return compute(observations, weights);
        }

        private double compute(TObservation[] observations, double[] weights)
        {
            // Estimation parameters

            double[] coefficients = Coefficients;
            var components = Distributions;

            double weightSum = 1;
            if (weights != null)
                weightSum = weights.Sum();

            // 1. Initialize means, covariances and mixing coefficients
            //    and evaluate the initial value of the log-likelihood

            int N = observations.Length;

            // Initialize responsibilities
            double[] norms = new double[N];
            for (int k = 0; k < Gamma.Length; k++)
                Gamma[k] = new double[N];

            // Clone the current distribution values
            double[] pi = (double[])coefficients.Clone();
            var pdf = new IFittableDistribution<TObservation>[components.Length];

            for (int i = 0; i < components.Length; i++)
                pdf[i] = (IFittableDistribution<TObservation>)components[i].Clone();

            // Prepare the iteration
            Convergence.NewValue = LogLikelihood(pi, pdf, observations, weights, weightSum);

            // Start
            do
            {
                // 2. Expectation: Evaluate the component distributions 
                //    responsibilities using the current parameter values.

                Parallel.For(0, Gamma.Length, k =>
                {
                    double[] gammak = Gamma[k];
                    for (int i = 0; i < observations.Length; i++)
                        gammak[i] = pi[k] * pdf[k].ProbabilityFunction(observations[i]);
                });

                Parallel.For(0, norms.Length, i =>
                {
                    double sum = 0;
                    for (int k = 0; k < Gamma.Length; k++)
                        sum += Gamma[k][i];
                    norms[i] = sum;
                });


                try
                {
                    Parallel.For(0, Gamma.Length, k =>
                    {
                        double[] gammak = Gamma[k];

                        for (int i = 0; i < gammak.Length; i++)
                            gammak[i] = (norms[i] != 0) ? gammak[i] / norms[i] : 0;

                        if (weights != null)
                        {
                            for (int i = 0; i < weights.Length; i++)
                                gammak[i] *= weights[i];
                        }

                        double sum = gammak.Sum();

                        if (Double.IsInfinity(sum) || Double.IsNaN(sum))
                            sum = 0;


                        // 3. Maximization: Re-estimate the distribution parameters
                        //    using the previously computed responsibilities

                        pi[k] = sum;

                        if (sum == 0)
                            return;

                        for (int i = 0; i < gammak.Length; i++)
                            gammak[i] /= sum;

                        pdf[k].Fit(observations, gammak, InnerOptions);
                    });
                }
                catch (AggregateException ex)
                {
                    if (ex.InnerException is NonPositiveDefiniteMatrixException)
                        throw ex.InnerException;
                }

                double sumPi = pi.Sum();
                for (int i = 0; i < pi.Length; i++)
                    pi[i] /= sumPi;

                // 4. Evaluate the log-likelihood and check for convergence
                Convergence.NewValue = LogLikelihood(pi, pdf, observations, weights, weightSum);


            } while (!Convergence.HasConverged);


            double newLikelihood = Convergence.NewValue;
            if (Double.IsNaN(newLikelihood) || Double.IsInfinity(newLikelihood))
                throw new ConvergenceException("Fitting did not converge.");


            // Become the newly fitted distribution.
            for (int i = 0; i < pi.Length; i++)
                Coefficients[i] = pi[i];

            for (int i = 0; i < pdf.Length; i++)
                Distributions[i] = pdf[i];

            return newLikelihood;
        }

        /// <summary>
        ///   Computes the log-likelihood of the distribution
        ///   for a given set of observations.
        /// </summary>
        /// 
        internal static double LogLikelihood(double[] pi, IDistribution<TObservation>[] pdf,
            TObservation[] observations)
        {
            return LogLikelihood(pi, pdf, observations, null, 0);
        }

        /// <summary>
        ///   Computes the log-likelihood of the distribution
        ///   for a given set of observations.
        /// </summary>
        /// 
        internal static double LogLikelihood(double[] pi, IDistribution<TObservation>[] pdf,
            TObservation[] observations, double[] weights, double weightSum)
        {
            double logLikelihood = 0.0;

#if NET35
            for (int i = 0; i < observations.Length; i++)
            {
                var x = observations[i];
                double w = weights[i];

                if (w == 0) 
                    continue;

                double sum = 0.0;
                for (int j = 0; j < pi.Length; j++)
                    sum += pi[j] * pdf[j].ProbabilityFunction(x);

                if (sum > 0) 
                    logLikelihood += Math.Log(sum) * w;
            }
#else
            object syncObj = new object();

            Parallel.For(0, observations.Length,

                () => 0.0,

                (i, status, partial) =>
                {
                    var x = observations[i];

                    double w = 1.0;

                    if (weights != null)
                    {
                        w = weights[i];

                        if (w == 0)
                            return partial;
                    }

                    double sum = 0.0;
                    for (int k = 0; k < pi.Length; k++)
                        sum += pi[k] * pdf[k].ProbabilityFunction(x);

                    if (sum > 0)
                        return partial + Math.Log(sum) * w;

                    return partial;
                },

                (partial) =>
                {
                    lock (syncObj)
                    {
                        logLikelihood += partial;
                    }
                }
            );
#endif

            System.Diagnostics.Debug.Assert(!Double.IsNaN(logLikelihood));
            
            if (weights != null)
            {
                System.Diagnostics.Debug.Assert(weightSum != 0);

                return logLikelihood / weightSum;
            }

            return logLikelihood;
        }

    }
}
