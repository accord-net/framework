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
    ///   Expectation Maximization algorithm for mixture model fitting in the log domain.
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
    public class LogExpectationMaximization<TObservation>
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
        public double[][] LogGamma { get; private set; }


        /// <summary>
        ///   Creates a new <see cref="LogExpectationMaximization{TObservation}"/> algorithm.
        /// </summary>
        /// 
        /// <param name="coefficients">The initial coefficient values.</param>
        /// <param name="distributions">The initial component distributions.</param>
        /// 
        public LogExpectationMaximization(double[] coefficients,
            IFittableDistribution<TObservation>[] distributions)
        {
            Coefficients = coefficients;
            Distributions = distributions;
            Convergence = new RelativeConvergence(0, 1e-3);
            LogGamma = new double[coefficients.Length][];
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
            return compute(observations);
        }


        private double compute(TObservation[] observations)
        {
            // Estimation parameters

            double[] coefficients = Coefficients;
            var components = Distributions;


            // 1. Initialize means, covariances and mixing coefficients
            //    and evaluate the initial value of the log-likelihood

            int N = observations.Length;

            // Initialize responsibilities
            double[] lnnorms = new double[N];
            for (int k = 0; k < LogGamma.Length; k++)
                LogGamma[k] = new double[N];

            // Clone the current distribution values
            double[] logPi = Matrix.Log(coefficients);
            var pdf = new IFittableDistribution<TObservation>[components.Length];

            for (int i = 0; i < components.Length; i++)
                pdf[i] = (IFittableDistribution<TObservation>)components[i].Clone();

            // Prepare the iteration
            Convergence.NewValue = LogLikelihood(logPi, pdf, observations);

            // Start
            do
            {
                // 2. Expectation: Evaluate the component distributions 
                //    responsibilities using the current parameter values.

                Parallel.For(0, LogGamma.Length, k =>
                {
                    double[] logGammak = LogGamma[k];
                    for (int i = 0; i < observations.Length; i++)
                        logGammak[i] = logPi[k] + pdf[k].LogProbabilityFunction(observations[i]);
                });

                Parallel.For(0, lnnorms.Length, i =>
                {
                    double lnsum = Double.NegativeInfinity;
                    for (int k = 0; k < LogGamma.Length; k++)
                        lnsum = Special.LogSum(lnsum, LogGamma[k][i]);
                    lnnorms[i] = lnsum;
                });


                try
                {
                    Parallel.For(0, LogGamma.Length, k =>
                    {
                        double[] lngammak = LogGamma[k];

                        double lnsum = Double.NegativeInfinity;
                        for (int i = 0; i < lngammak.Length; i++)
                        {
                            double value = double.NegativeInfinity;

                            if (lnnorms[i] != Double.NegativeInfinity)
                                value = lngammak[i] - lnnorms[i];

                            lngammak[i] = value;

                            lnsum = Special.LogSum(lnsum, value);
                        }

                        if (Double.IsNaN(lnsum))
                            lnsum = Double.NegativeInfinity;


                        // 3. Maximization: Re-estimate the distribution parameters
                        //    using the previously computed responsibilities

                        logPi[k] = lnsum;

                        if (lnsum == Double.NegativeInfinity)
                            return;

                        for (int i = 0; i < lngammak.Length; i++)
                            lngammak[i] = Math.Exp(lngammak[i] - lnsum);

                        pdf[k].Fit(observations, lngammak, InnerOptions);
                    });
                }
                catch (AggregateException ex)
                {
                    if (ex.InnerException is NonPositiveDefiniteMatrixException)
                        throw ex.InnerException;
                }

                double lnsumPi = Double.NegativeInfinity;
                for (int i = 0; i < logPi.Length; i++)
                    lnsumPi = Special.LogSum(lnsumPi, logPi[i]);

                for (int i = 0; i < logPi.Length; i++)
                    logPi[i] -= lnsumPi;

                // 4. Evaluate the log-likelihood and check for convergence
                Convergence.NewValue = LogLikelihood(logPi, pdf, observations);


            } while (!Convergence.HasConverged);


            double newLikelihood = Convergence.NewValue;
            if (Double.IsNaN(newLikelihood) || Double.IsInfinity(newLikelihood))
                throw new ConvergenceException("Fitting did not converge.");


            // Become the newly fitted distribution.
            for (int i = 0; i < logPi.Length; i++)
                Coefficients[i] = Math.Exp(logPi[i]);

            for (int i = 0; i < pdf.Length; i++)
                Distributions[i] = pdf[i];

            return newLikelihood;
        }

        /// <summary>
        ///   Computes the log-likelihood of the distribution
        ///   for a given set of observations.
        /// </summary>
        /// 
        internal static double LogLikelihood(double[] lnpi, IDistribution<TObservation>[] pdf,
            TObservation[] observations)
        {
            double logLikelihood = 0.0;
           
#if NET35
            for (int i = 0; i < observations.Length; i++)
            {
                var x = observations[i];

                double sum = 0.0;

                for (int k = 0; k < lnpi.Length; k++)
                        sum = Special.LogSum(sum, lnpi[k] + pdf[k].LogProbabilityFunction(x));

                if (sum > 0) 
                    logLikelihood += Math.Log(sum);
            }
#else
            object syncObj = new object();

            Parallel.For(0, observations.Length,

                () => 0.0,

                (i, status, partial) =>
                {
                    var x = observations[i];

                    for (int k = 0; k < lnpi.Length; k++)
                        partial = Special.LogSum(partial, lnpi[k] + pdf[k].LogProbabilityFunction(x));

                    return partial;
                },

                (partial) =>
                {
                    lock (syncObj)
                    {
                        logLikelihood = Special.LogSum(logLikelihood, partial);
                    }
                }
            );
#endif

            System.Diagnostics.Debug.Assert(!Double.IsNaN(logLikelihood));
            
            return logLikelihood;
        }

    }
}
