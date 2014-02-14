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

namespace Accord.Statistics.Distributions.Multivariate
{
    using System;
    using Accord.Math;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Univariate;
    using System.Threading.Tasks;

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
        where T : IMultivariateDistribution<double[]>
    {

        // distribution parameters
        private double[] coefficients;
        private T[] components;

        // distributions measures
        double[] mean;
        double[,] covariance;
        double[] variance;


        // cache
        IDistribution<double[]>[] cache;


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

            this.components = components;

            this.coefficients = new double[components.Length];
            for (int i = 0; i < coefficients.Length; i++)
                coefficients[i] = 1.0 / coefficients.Length;

            this.cache = new IDistribution<double[]>[coefficients.Length];
            for (int i = 0; i < cache.Length; i++)
                cache[i] = components[i];

            this.initialize();
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


            this.components = components;
            this.coefficients = coefficients;

            this.cache = new IDistribution<double[]>[coefficients.Length];
            for (int i = 0; i < cache.Length; i++)
                cache[i] = components[i];

            this.initialize();
        }


        private void initialize()
        {
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
            double[] logCoefficients = Matrix.Log(coefficients);

            double log = Double.NegativeInfinity;
            for (int i = 0; i < components.Length; i++)
                log = Special.LogSum(log, logCoefficients[i] + components[i].LogProbabilityFunction(x));
            return log;
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
            var pdf = new IFittableDistribution<double[]>[coefficients.Length];
            for (int i = 0; i < components.Length; i++)
                pdf[i] = (IFittableDistribution<double[]>)components[i];

            bool log = (options != null && options.Logarithm);

            if (log)
            {
                if (weights != null)
                {
                    throw new ArgumentException("The model fitting algorithm does not"
                    + " currently support different weights when the logarithm option"
                    + " is enabled. To avoid this exception, pass 'null' as the second"
                    + " parameter's value when calling this method.");
                }

                var em = new LogExpectationMaximization<double[]>(coefficients, pdf);

                if (options != null)
                {
                    em.InnerOptions = options.InnerOptions;
                    em.Convergence.Iterations = options.Iterations;
                    em.Convergence.Tolerance = options.Threshold;
                }

                em.Compute(observations);
            }
            else
            {
                var em = new ExpectationMaximization<double[]>(coefficients, pdf);

                if (options != null)
                {
                    em.InnerOptions = options.InnerOptions;
                    em.Convergence.Iterations = options.Iterations;
                    em.Convergence.Tolerance = options.Threshold;
                }

                em.Compute(observations, weights);
            }

            for (int i = 0; i < components.Length; i++)
                cache[i] = components[i] = (T)pdf[i];

            this.initialize();
        }

        /// <summary>
        ///   Computes the log-likelihood of the distribution
        ///   for a given set of observations.
        /// </summary>
        /// 
        public double LogLikelihood(double[][] observations, double[] weights)
        {
            return ExpectationMaximization<double[]>.LogLikelihood(coefficients, cache, 
                observations, weights, weights.Sum());
        }

        /// <summary>
        ///   Computes the log-likelihood of the distribution
        ///   for a given set of observations.
        /// </summary>
        /// 
        public double LogLikelihood(double[][] observations)
        {
            return ExpectationMaximization<double[]>.LogLikelihood(coefficients, cache, observations);
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
