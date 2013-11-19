// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2013
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

    public class ExpectationMaximization<TObservation>
    {

        public IFittingOptions InnerOptions { get; set; }

        public ISingleValueConvergence Convergence { get; set; }


        public double[] Coefficients { get; private set; }
        public IFittableDistribution<TObservation>[] Distributions { get; private set; }


        public double[][] Gamma { get; private set; }

        public ExpectationMaximization(double[] coefficients,
            IFittableDistribution<TObservation>[] distributions)
        {
            Coefficients = coefficients;
            Distributions = distributions;
            Convergence = new RelativeConvergence(0, 1e-3);
            Gamma = new double[coefficients.Length][];
        }


        public double Compute(TObservation[] observations, double[] weights)
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
            int K = components.Length;


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
                    for (int i = 0; i < observations.Length; i++)
                        Gamma[k][i] = pi[k] * pdf[k].ProbabilityFunction(observations[i]);
                });

                Parallel.For(0, norms.Length, i =>
                {
                    double sum = 0;
                    for (int k = 0; k < Gamma.Length; k++)
                        sum += Gamma[k][i];
                    norms[i] = sum;
                });

                if (weights == null)
                {
                    Parallel.For(0, Gamma.Length, k =>
                    {
                        for (int i = 0; i < norms.Length; i++)
                            if (norms[i] != 0)
                                Gamma[k][i] /= norms[i];
                    });
                }
                else
                {
                    Parallel.For(0, Gamma.Length, k =>
                    {
                        for (int i = 0; i < norms.Length; i++)
                            if (norms[i] != 0)
                                Gamma[k][i] *= weights[i] / norms[i];
                    });
                }

                // 3. Maximization: Re-estimate the distribution parameters
                //    using the previously computed responsibilities
                try
                {
                    Parallel.For(0, Gamma.Length, k =>
                    {
                        double sum = Gamma[k].Sum();

                        pi[k] = sum;

                        if (sum == 0)
                            return;

                        System.Diagnostics.Debug.Assert(sum != 0);
                        System.Diagnostics.Debug.Assert(!Gamma[k].HasNaN());

                        for (int i = 0; i < Gamma[k].Length; i++)
                            Gamma[k][i] /= sum;

                        pdf[k].Fit(observations, Gamma[k], InnerOptions);
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
        public static double LogLikelihood(double[] pi, IDistribution<TObservation>[] pdf,
            TObservation[] observations)
        {
            return LogLikelihood(pi, pdf, observations, null, 0);
        }

        /// <summary>
        ///   Computes the log-likelihood of the distribution
        ///   for a given set of observations.
        /// </summary>
        /// 
        public static double LogLikelihood(double[] pi, IDistribution<TObservation>[] pdf,
            TObservation[] observations, double[] weights, double weightSum)
        {
            double logLikelihood = 0.0;

#if NET35
            for (int i = 0; i < observations.Length; i++)
            {
                var x = observations[i];
                double w = weigths[i];

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

                    double w = 1.0 / observations.Length;

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

            if (weights != null)
                return logLikelihood / weightSum;

            return logLikelihood;
        }

    }
}
