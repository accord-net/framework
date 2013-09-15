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

namespace Accord.Statistics.Distributions.Multivariate
{
    using System;
    using Accord.Math;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Univariate;

    /// <summary>
    ///   Mixture of multivariate probability distributions.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   A mixture density is a probability density function which is expressed
    ///   as a convex combination (i.e. a weighted sum, with non-negative weights
    ///   that sum to 1) of other probability density functions. The individual
    ///   density functions that are combined to make the mixture density are
    ///   called the mixture components, and the weights associated with each
    ///   component are called the mixture weights.</para>
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Mixture_density">
    ///       Wikipedia, The Free Encyclopedia. Mixture density. Available on:
    ///       http://en.wikipedia.org/wiki/Mixture_density </a></description></item>
    ///   </list></para>
    /// </remarks>
    ///   
    /// <typeparam name="T">
    ///   The type of the multivariate component distributions.</typeparam>
    ///   
    [Serializable]
    public class MultivariateMixture<T> : MultivariateContinuousDistribution, IMixture<T>,
        IFittableDistribution<double[], MixtureOptions>,
        ISampleableDistribution<double[]>
        where T : IMultivariateDistribution
    {

        // distribution parameters
        private double[] coefficients;
        private T[] components;

        // distributions measures
        double[] mean;
        double[,] covariance;
        double[] variance;


        /// <summary>
        ///   Initializes a new instance of the <see cref="MultivariateMixture&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="components">The mixture distribution components.</param>
        /// 
        public MultivariateMixture(params T[] components)
            : base(components[0].Dimension)
        {
            if (components == null)
                throw new ArgumentNullException("components");

            this.initialize(null, components);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MultivariateMixture&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="coefficients">The mixture weight coefficients.</param>
        /// <param name="components">The mixture distribution components.</param>
        /// 
        public MultivariateMixture(double[] coefficients, params T[] components)
            : base(components[0].Dimension)
        {
            if (components == null)
                throw new ArgumentNullException("components");

            if (coefficients == null)
                throw new ArgumentNullException("coefficients");

            if (coefficients.Length != components.Length)
                throw new ArgumentException(
                    "The coefficient and component arrays should have the same length.",
                    "components");

            this.initialize(coefficients, components);
        }


        private void initialize(double[] coef, T[] comp)
        {
            if (coef == null)
            {
                coef = new double[comp.Length];
                for (int i = 0; i < coef.Length; i++)
                    coef[i] = 1.0 / coef.Length;
            }

            this.coefficients = coef;
            this.components = comp;

            this.mean = null;
            this.covariance = null;
            this.variance = null;
        }

        /// <summary>
        ///   Gets the mixture components.
        /// </summary>
        /// 
        public T[] Components
        {
            get { return components; }
        }

        /// <summary>
        ///   Gets the weight coefficients.
        /// </summary>
        /// 
        public double[] Coefficients
        {
            get { return coefficients; }
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for one of
        ///   the components distributions evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="componentIndex">The index of the desired component distribution.</param>
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        public double ProbabilityDensityFunction(int componentIndex, params double[] x)
        {
            return coefficients[componentIndex] * components[componentIndex].ProbabilityFunction(x);
        }

        /// <summary>
        ///   Gets the log-probability density function (pdf)
        ///   for one of the components distributions evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="componentIndex">The index of the desired component distribution.</param>
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The logarithm of the probability of <c>x</c>
        ///    occurring in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        public double LogProbabilityDensityFunction(int componentIndex, params double[] x)
        {
            return Math.Log(coefficients[componentIndex]) + components[componentIndex].LogProbabilityFunction(x);
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        public override double ProbabilityDensityFunction(params double[] x)
        {
            double r = 0.0;
            for (int i = 0; i < components.Length; i++)
                r += coefficients[i] * components[i].ProbabilityFunction(x);
            return r;
        }

        /// <summary>
        ///   Gets the log-probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">
        ///   A single point in the distribution range. For a 
        ///   univariate distribution, this should be a single
        ///   double value. For a multivariate distribution,
        ///   this should be a double array.</param>
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// <returns>
        ///   The logarithm of the probability of <c>x</c> 
        ///   occurring in the current distribution.</returns>
        ///   
        public override double LogProbabilityDensityFunction(params double[] x)
        {
            double r = 0.0;
            for (int i = 0; i < components.Length; i++)
                r += coefficients[i] * components[i].ProbabilityFunction(x);
            return Math.Log(r);
        }

        /// <summary>
        ///   This method is not supported.
        /// </summary>
        /// 
        public override double DistributionFunction(params double[] x)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        /// elements can be either of type double (for univariate data) or
        /// type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, such
        /// as regularization constants and additional parameters.</param>
        /// 
        public override void Fit(double[][] observations, double[] weights, IFittingOptions options)
        {
            MixtureOptions mixOptions = options as MixtureOptions;
            if (options != null && mixOptions == null)
                throw new ArgumentException("The specified options' type is invalid.", "options");

            Fit(observations, weights, mixOptions);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        /// elements can be either of type double (for univariate data) or
        /// type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, such
        /// as regularization constants and additional parameters.</param>
        /// 
        public void Fit(double[][] observations, double[] weights, MixtureOptions options)
        {
            // Estimation parameters
            int maxIterations = 0;
            double threshold = 1e-3;
            IFittingOptions innerOptions = null;

#if DEBUG
            if (weights != null)
                for (int i = 0; i < weights.Length; i++)
                    if (Double.IsNaN(weights[i]) || Double.IsInfinity(weights[i]))
                        throw new ArgumentException("Invalid numbers in the weight vector.","weights");
#endif

            if (options != null)
            {
                // Process optional arguments
                threshold = options.Threshold;
                innerOptions = options.InnerOptions;
                maxIterations = options.Iterations;
            }


            // 1. Initialize means, covariances and mixing coefficients
            //    and evaluate the initial value of the log-likelihood

            int N = observations.Length;
            int K = components.Length;

            double weightSum;
            if (weights == null)
            {
                weights = new double[observations.Length];
                for (int i = 0; i < weights.Length; i++)
                    weights[i] = 1.0 / weights.Length;
                weightSum = 1.0;
            }
            else weightSum = weights.Sum();

            // Initialize responsibilities
            double[] norms = new double[N];
            double[][] gamma = new double[K][];
            for (int k = 0; k < gamma.Length; k++)
                gamma[k] = new double[N];

            // Clone the current distribution values
            double[] pi = (double[])coefficients.Clone();
            T[] pdf = new T[components.Length];
            for (int i = 0; i < components.Length; i++)
                pdf[i] = (T)components[i].Clone();

            // Prepare the iteration
            double likelihood = logLikelihood(pi, pdf, observations, weights);
            bool converged = false;
            int iteration = 0;

            // Start
            while (!converged)
            {
                iteration++;

                // 2. Expectation: Evaluate the component distributions 
                //    responsibilities using the current parameter values.
                Array.Clear(norms, 0, norms.Length);

                for (int k = 0; k < gamma.Length; k++)
                    for (int i = 0; i < observations.Length; i++)
                        norms[i] += gamma[k][i] = pi[k] * pdf[k].ProbabilityFunction(observations[i]);

                for (int k = 0; k < gamma.Length; k++)
                    for (int i = 0; i < weights.Length; i++)
                        if (norms[i] != 0) gamma[k][i] *= weights[i] / norms[i];

                // 3. Maximization: Re-estimate the distribution parameters
                //    using the previously computed responsibilities
                for (int k = 0; k < gamma.Length; k++)
                {
                    double sum = gamma[k].Sum();

                    if (sum == 0)
                    {
                        pi[k] = 0.0;
                        continue;
                    }

                    System.Diagnostics.Debug.Assert(sum != 0);
                    System.Diagnostics.Debug.Assert(weightSum != 0);
                    System.Diagnostics.Debug.Assert(!gamma[k].HasNaN());

                    for (int i = 0; i < gamma[k].Length; i++)
                        gamma[k][i] /= sum;

                    pi[k] = sum / weightSum;
                    pdf[k].Fit(observations, gamma[k], innerOptions);
                }

                // 4. Evaluate the log-likelihood and check for convergence
                double newLikelihood = logLikelihood(pi, pdf, observations, weights);

                if (Double.IsNaN(newLikelihood) || Double.IsInfinity(newLikelihood))
                    throw new ConvergenceException("Fitting did not converge.");

                if ((maxIterations > 0 && iteration >= maxIterations) ||
                    Math.Abs(likelihood - newLikelihood) < threshold * Math.Abs(likelihood))
                    converged = true;


                likelihood = newLikelihood;
            }

            // Become the newly fitted distribution.
            this.initialize(pi, pdf);
        }

        /// <summary>
        ///   Computes the log-likelihood of the distribution
        ///   for a given set of observations.
        /// </summary>
        /// 
        public double LogLikelihood(double[][] observations, double[] weights)
        {
            return logLikelihood(coefficients, components, observations, weights);
        }

        /// <summary>
        ///   Computes the log-likelihood of the distribution
        ///   for a given set of observations.
        /// </summary>
        /// 
        public double LogLikelihood(double[][] observations)
        {
            double[] weights = new double[observations.Length];
            for (int i = 0; i < weights.Length; i++)
                weights[i] = 1.0 / weights.Length;

            return logLikelihood(coefficients, components, observations, weights);
        }

        /// <summary>
        ///   Computes the log-likelihood of the distribution
        ///   for a given set of observations.
        /// </summary>
        /// 
        private static double logLikelihood(double[] pi, T[] pdf, double[][] observations, double[] weigths)
        {
            double logLikelihood = 0.0;

            for (int i = 0; i < observations.Length; i++)
            {
                double[] x = observations[i];
                double w = weigths[i];

                if (w == 0) continue;

                double sum = 0.0;
                for (int j = 0; j < pi.Length; j++)
                    sum += pi[j] * pdf[j].ProbabilityFunction(x);

                if (sum > 0) logLikelihood += Math.Log(sum) * w;
            }

            return logLikelihood;
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public override object Clone()
        {
            // Clone the mixture coefficients
            double[] pi = (double[])coefficients.Clone();

            // Clone the mixture components
            T[] pdf = new T[components.Length];
            for (int i = 0; i < components.Length; i++)
                pdf[i] = (T)components[i].Clone();

            return new MultivariateMixture<T>(pi, pdf);
        }


        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        public override double[] Mean
        {
            get
            {
                if (mean == null)
                {
                    mean = new double[Dimension];
                    for (int j = 0; j < mean.Length; j++)
                        for (int i = 0; i < coefficients.Length; i++)
                            mean[j] += coefficients[i] * components[i].Mean[j];
                }

                return mean;
            }
        }

        /// <summary>
        ///   Gets the variance-covariance matrix for this distribution.
        /// </summary>
        /// 
        public override double[,] Covariance
        {
            get
            {
                if (covariance == null)
                {
                    // E[Var[X|Y]]
                    double[,] EVar = new double[Dimension, Dimension];
                    for (int i = 0; i < Dimension; i++)
                    {
                        for (int j = 0; j < Dimension; j++)
                        {
                            for (int k = 0; k < Components.Length; k++)
                                EVar[i, j] += components[k].Covariance[i, j];
                            EVar[i, j] /= Components.Length;
                        }
                    }

                    // Var[E[X|Y]]
                    double[][] means = new double[components.Length][];
                    for (int k = 0; k < components.Length; k++)
                        means[k] = components[k].Mean;
                    double[,] VarE = Statistics.Tools.Scatter(means, (double)components.Length);

                    // Var[X] = E[Var [X|Y]] + Var[E[X|Y]]
                    covariance = EVar.Add(VarE);
                }

                return covariance;
            }
        }

        /// <summary>
        ///   Gets the variance vector for this distribution.
        /// </summary>
        /// 
        public override double[] Variance
        {
            get
            {
                if (variance == null)
                    variance = Matrix.Diagonal(Covariance);

                return variance;
            }
        }

        /// <summary>
        ///   Estimates a new mixture model from a given set of observations.
        /// </summary>
        /// 
        /// <param name="data">A set of observations.</param>
        /// <param name="components">The initial components of the mixture model.</param>
        /// <returns>Returns a new Mixture fitted to the given observations.</returns>
        /// 
        public static MultivariateMixture<T> Estimate(double[][] data, params T[] components)
        {
            var mixture = new MultivariateMixture<T>(components);
            mixture.Fit(data);
            return mixture;
        }

        /// <summary>
        ///   Estimates a new mixture model from a given set of observations.
        /// </summary>
        /// 
        /// <param name="data">A set of observations.</param>
        /// <param name="components">The initial components of the mixture model.</param>
        /// <param name="coefficients">The initial mixture coefficients.</param>
        /// <returns>Returns a new Mixture fitted to the given observations.</returns>
        /// 
        public static MultivariateMixture<T> Estimate(double[][] data, double[] coefficients, params T[] components)
        {
            var mixture = new MultivariateMixture<T>(coefficients, components);
            mixture.Fit(data);
            return mixture;
        }

        /// <summary>
        ///   Estimates a new mixture model from a given set of observations.
        /// </summary>
        /// 
        /// <param name="data">A set of observations.</param>
        /// <param name="components">The initial components of the mixture model.</param>
        /// <param name="coefficients">The initial mixture coefficients.</param>
        /// <param name="threshold">The convergence threshold for the Expectation-Maximization estimation.</param>
        /// <returns>Returns a new Mixture fitted to the given observations.</returns>
        /// 
        public static MultivariateMixture<T> Estimate(double[][] data, double threshold, double[] coefficients, params T[] components)
        {
            IFittingOptions options = new MixtureOptions()
            {
                Threshold = threshold
            };

            var mixture = new MultivariateMixture<T>(coefficients, components);
            mixture.Fit(data, options);
            return mixture;
        }


        #region ISampleableDistribution<double[]> Members

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public double[][] Generate(int samples)
        {
            double[][] r = new double[samples][];
            r.ApplyInPlace(x => Generate());
            return r;
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public double[] Generate()
        {
            // Choose one coefficient at random
            int c = GeneralDiscreteDistribution.Random(coefficients);

            // Sample from the chosen coefficient
            var d = components[c] as ISampleableDistribution<double[]>;

            if (d == null) throw new InvalidOperationException();

            return d.Generate();
        }

        #endregion
    }
}
