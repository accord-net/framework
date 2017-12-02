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

namespace Accord.Statistics.Distributions.Multivariate
{
    using System;
    using Accord.Math;
    using Accord.Statistics.Distributions.Fitting;
    using System.Globalization;
    using Accord.Statistics.Distributions.Sampling;
    using Accord.Math.Random;
    using Accord.Compat;

    /// <summary>
    ///   Abstract class for Matrix Probability Distributions.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   A probability distribution identifies either the probability of each value of an
    ///   unidentified random variable (when the variable is discrete), or the probability
    ///   of the value falling within a particular interval (when the variable is continuous).</para>
    /// <para>
    ///   The probability distribution describes the range of possible values that a random
    ///   variable can attain and the probability that the value of the random variable is
    ///   within any (measurable) subset of that range.</para>  
    /// <para>
    ///   The function describing the probability that a given value will occur is called
    ///   the probability function (or probability density function, abbreviated PDF), and
    ///   the function describing the cumulative probability that a given value or any value
    ///   smaller than it will occur is called the distribution function (or cumulative
    ///   distribution function, abbreviated CDF).</para>  
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Probability_distribution">
    ///       Wikipedia, The Free Encyclopedia. Probability distribution. Available on:
    ///       http://en.wikipedia.org/wiki/Probability_distribution </a></description></item>
    ///     <item><description><a href="http://mathworld.wolfram.com/StatisticalDistribution.html">
    ///       Weisstein, Eric W. "Statistical Distribution." From MathWorld--A Wolfram Web Resource.
    ///       http://mathworld.wolfram.com/StatisticalDistribution.html </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    [Serializable]
    public abstract class MatrixContinuousDistribution : DistributionBase,
        IMultivariateDistribution,
        IMultivariateDistribution<double[,]>,
        IFittableDistribution<double[,]>,
        ISampleableDistribution<double[,]>,
        IMultivariateDistribution<double[]>,
        IFittableDistribution<double[]>,
        ISampleableDistribution<double[]>,
        IFormattable
    {

        private int rows;
        private int cols;


        /// <summary>
        ///   Constructs a new MultivariateDistribution class.
        /// </summary>
        /// 
        /// <param name="rows">The number of rows for matrices modeled by the distribution.</param>
        /// <param name="cols">The number of rows for matrices modeled by the distribution.</param>
        /// 
        protected MatrixContinuousDistribution(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
        }

        /// <summary>
        ///   Gets the number of variables for this distribution.
        /// </summary>
        /// 
        public int Dimension { get { return rows * cols; } }

        /// <summary>
        ///   Gets the number of rows that matrices from this distribution should have.
        /// </summary>
        /// 
        public int NumberOfRows { get { return rows; } }

        /// <summary>
        ///   Gets the number of columns that matrices from this distribution should have.
        /// </summary>
        /// 
        public int NumberOfColumns { get { return cols; } }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>A vector containing the mean values for the distribution.</value>
        /// 
        public abstract double[,] Mean { get; }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>A vector containing the variance values for the distribution.</value>
        /// 
        public abstract double[] Variance { get; }

        /// <summary>
        ///   Gets the variance-covariance matrix for this distribution.
        /// </summary>
        /// 
        /// <value>A matrix containing the covariance values for the distribution.</value>
        /// 
        public abstract double[,] Covariance { get; }

        /// <summary>
        ///   Gets the mode for this distribution.
        /// </summary>
        /// 
        /// <value>A vector containing the mode values for the distribution.</value>
        /// 
        public virtual double[,] Mode
        {
            get { return Mean; }
        }

        /// <summary>
        ///   Gets the median for this distribution.
        /// </summary>
        /// 
        /// <value>A vector containing the median values for the distribution.</value>
        /// 
        public virtual double[,] Median
        {
            get { return Mean; }
        }

        double[] IMultivariateDistribution.Mean
        {
            get { return Mean.Reshape(); }
        }

        double[] IMultivariateDistribution.Median
        {
            get { return Median.Reshape(); }
        }

        double[] IMultivariateDistribution.Mode
        {
            get { return Mode.Reshape(); }
        }

        double[] IMultivariateDistribution<double[,]>.Mean
        {
            get { return Mean.Reshape(); }
        }

        double[] IMultivariateDistribution<double[,]>.Median
        {
            get { return Median.Reshape(); }
        }

        double[] IMultivariateDistribution<double[,]>.Mode
        {
            get { return Mode.Reshape(); }
        }

        double[] IMultivariateDistribution<double[]>.Mean
        {
            get { return Mean.Reshape(); }
        }

        double[] IMultivariateDistribution<double[]>.Median
        {
            get { return Median.Reshape(); }
        }

        double[] IMultivariateDistribution<double[]>.Mode
        {
            get { return Mode.Reshape(); }
        }



        #region IDistribution explicit members

        double IDistribution.ProbabilityFunction(double[] x)
        {
            return ProbabilityDensityFunction(reshape(x));
        }

        double IDistribution.LogProbabilityFunction(double[] x)
        {
            return LogProbabilityDensityFunction(reshape(x));
        }

        void IDistribution.Fit(Array observations)
        {
            (this as IDistribution).Fit(observations, (IFittingOptions)null);
        }

        void IDistribution.Fit(Array observations, double[] weights)
        {
            (this as IDistribution).Fit(observations, weights, (IFittingOptions)null);
        }

        void IDistribution.Fit(Array observations, int[] weights)
        {
            (this as IDistribution).Fit(observations, weights, (IFittingOptions)null);
        }

        void IDistribution.Fit(Array observations, IFittingOptions options)
        {
            (this as IDistribution).Fit(observations, (double[])null, options);
        }

        void IDistribution.Fit(Array observations, double[] weights, IFittingOptions options)
        {
            double[][,] multivariate = observations as double[][,];
            if (multivariate != null)
            {
                Fit(multivariate, weights, options);
                return;
            }

            throw new ArgumentException("Unsupported parameter type.", "observations");
        }

        void IDistribution.Fit(Array observations, int[] weights, IFittingOptions options)
        {
            double[][,] multivariate = observations as double[][,];
            if (multivariate != null)
            {
                Fit(multivariate, weights, options);
                return;
            }

            throw new ArgumentException("Unsupported parameter type.", "observations");
        }
        #endregion


        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range. For a 
        ///   univariate distribution, this should be a single
        ///   double value. For a multivariate distribution,
        ///   this should be a double array.</param>
        ///   
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.</returns>
        ///   
        public double DistributionFunction(double[,] x)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            return InnerDistributionFunction(x);
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range. For a 
        ///   univariate distribution, this should be a single
        ///   double value. For a multivariate distribution,
        ///   this should be a double array.</param>
        ///   
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.</returns>
        ///   
        protected internal abstract double InnerDistributionFunction(double[,] x);

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range. For a 
        ///   univariate distribution, this should be a single
        ///   double value. For a multivariate distribution,
        ///   this should be a double array.</param>
        ///   
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.</returns>
        ///   
        public double ProbabilityDensityFunction(double[,] x)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            return InnerProbabilityDensityFunction(x);
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range. For a 
        ///   univariate distribution, this should be a single
        ///   double value. For a multivariate distribution,
        ///   this should be a double array.</param>
        ///   
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.</returns>
        ///   
        protected internal abstract double InnerProbabilityDensityFunction(double[,] x);

        /// <summary>
        ///   Gets the log-probability density function (pdf)
        ///   for this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range. For a 
        ///   univariate distribution, this should be a single
        ///   double value. For a multivariate distribution,
        ///   this should be a double array.</param>
        ///   
        /// <returns>
        ///   The logarithm of the probability of <c>x</c>
        ///   occurring in the current distribution.</returns>
        ///   
        public double LogProbabilityDensityFunction(double[,] x)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            return InnerLogProbabilityDensityFunction(x);
        }

        /// <summary>
        ///   Gets the log-probability density function (pdf)
        ///   for this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range. For a 
        ///   univariate distribution, this should be a single
        ///   double value. For a multivariate distribution,
        ///   this should be a double array.</param>
        ///   
        /// <returns>
        ///   The logarithm of the probability of <c>x</c>
        ///   occurring in the current distribution.</returns>
        ///   
        protected internal virtual double InnerLogProbabilityDensityFunction(double[,] x)
        {
            return Math.Log(ProbabilityDensityFunction(x));
        }

        /// <summary>
        ///   Gets the complementary cumulative distribution function
        ///   (ccdf) for this distribution evaluated at point <c>x</c>.
        ///   This function is also known as the Survival function.
        /// </summary>
        /// 
        /// <remarks>
        ///   The Complementary Cumulative Distribution Function (CCDF) is
        ///   the complement of the Cumulative Distribution Function, or 1
        ///   minus the CDF.
        /// </remarks>
        /// 
        public double ComplementaryDistributionFunction(double[,] x)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            return InnerComplementaryDistributionFunction(x);
        }

        /// <summary>
        ///   Gets the complementary cumulative distribution function
        ///   (ccdf) for this distribution evaluated at point <c>x</c>.
        ///   This function is also known as the Survival function.
        /// </summary>
        /// 
        /// <remarks>
        ///   The Complementary Cumulative Distribution Function (CCDF) is
        ///   the complement of the Cumulative Distribution Function, or 1
        ///   minus the CDF.
        /// </remarks>
        /// 
        protected internal virtual double InnerComplementaryDistributionFunction(double[,] x)
        {
            return 1.0 - DistributionFunction(x);
        }

        /// 
        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">
        ///   The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        ///   
        public virtual void Fit(double[][,] observations)
        {
            Fit(observations, (IFittingOptions)null);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">
        ///   The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">
        ///   The weight vector containing the weight for each of the samples.</param>
        ///   
        public virtual void Fit(double[][,] observations, double[] weights)
        {
            Fit(observations, weights, (IFittingOptions)null);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">
        ///   The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">
        ///   The weight vector containing the weight for each of the samples.</param>
        ///   
        public virtual void Fit(double[][,] observations, int[] weights)
        {
            Fit(observations, weights, (IFittingOptions)null);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">
        ///   The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="options">
        ///   Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        ///   
        public virtual void Fit(double[][,] observations, IFittingOptions options)
        {
            Fit(observations, (double[])null, options);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">
        ///   The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data). </param>
        /// <param name="weights">
        ///   The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">
        ///   Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        ///   
        public virtual void Fit(double[][,] observations, double[] weights, IFittingOptions options)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">
        ///   The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data). </param>
        /// <param name="weights">
        ///   The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">
        ///   Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        ///   
        public virtual void Fit(double[][,] observations, int[] weights, IFittingOptions options)
        {
            if (weights != null)
                throw new NotSupportedException();

            Fit(observations, (double[])null, options);
        }




        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public double[][,] Generate(int samples)
        {
            return Generate(samples, Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        ///
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public double[][,] Generate(int samples, double[][,] result)
        {
            return Generate(samples, result, Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public double[,] Generate()
        {
            return Generate(Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public double[,] Generate(double[,] result)
        {
            return Generate(result, Accord.Math.Random.Generator.Random);
        }




        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///   
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public double[][,] Generate(int samples, Random source)
        {
            return Generate(samples, new double[samples].Apply(x => new double[rows, cols]), source);
        }

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///   
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public virtual double[][,] Generate(int samples, double[][,] result, Random source)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public double[,] Generate(Random source)
        {
            return Generate(1, source)[0];
        }


        /// <summary>
        /// Generates a random observation from the current distribution.
        /// </summary>
        /// <param name="result">The location where to store the sample.</param>
        /// <param name="source">The random number generator to use as a source of randomness.
        /// Default is to use <see cref="P:Accord.Math.Random.Generator.Random" />.</param>
        /// <returns>A random observation drawn from this distribution.</returns>
        public double[,] Generate(double[,] result, Random source)
        {
            return Generate(1, result: new[] { result }, source: source)[0];
        }



        /// <summary>
        /// Generates a random observation from the current distribution.
        /// </summary>
        /// <param name="result">The location where to store the sample.</param>
        /// <returns>A random observation drawn from this distribution.</returns>
        public virtual double[] Generate(double[] result)
        {
            return Generate(result, Accord.Math.Random.Generator.Random);
        }


        /// <summary>
        /// Generates a random observation from the current distribution.
        /// </summary>
        /// <param name="result">The location where to store the sample.</param>
        /// <param name="source">The random number generator to use as a source of randomness.
        /// Default is to use <see cref="P:Accord.Math.Random.Generator.Random" />.</param>
        /// <returns>A random observation drawn from this distribution.</returns>
        public double[] Generate(double[] result, Random source)
        {
            return Generate(1, result: new[] { result }, source: source)[0];
        }

        double[][] ISampleableDistribution<double[]>.Generate(int samples, Random source)
        {
            return Generate(samples, Jagged.Create<double>(samples, Dimension));
        }

        /// <summary>
        /// Generates a random vector of observations from the current distribution.
        /// </summary>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <param name="source">The random number generator to use as a source of randomness.
        /// Default is to use <see cref="P:Accord.Math.Random.Generator.Random" />.</param>
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        public double[][] Generate(int samples, double[][] result, Random source)
        {
            var buffer = new double[rows, cols];
            for (int i = 0; i < samples; i++)
            {
                Generate(buffer, source);
                Buffer.BlockCopy(buffer, 0, result, 0, Dimension);
            }

            return result;
        }

        double[] ISampleableDistribution<double[]>.Generate(Random source)
        {
            return Generate(source).Reshape();
        }

        double[][] IRandomNumberGenerator<double[]>.Generate(int samples)
        {
            return Generate(samples, Jagged.Create<double>(samples, Dimension));
        }

        /// <summary>
        /// Generates a random vector of observations from the current distribution.
        /// </summary>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        public double[][] Generate(int samples, double[][] result)
        {
            return Generate(samples, result, Accord.Math.Random.Generator.Random);
        }

        double[] IRandomNumberGenerator<double[]>.Generate()
        {
            return Generate().Reshape();
        }




        double IDistribution<double[,]>.ProbabilityFunction(double[,] x)
        {
            return ProbabilityDensityFunction(x);
        }

        double IDistribution<double[,]>.LogProbabilityFunction(double[,] x)
        {
            return LogProbabilityDensityFunction(x);
        }

        double IDistribution<double[]>.ProbabilityFunction(double[] x)
        {
            return ProbabilityDensityFunction(reshape(x));
        }

        double IDistribution<double[]>.LogProbabilityFunction(double[] x)
        {
            return LogProbabilityDensityFunction(reshape(x));
        }


        private double[,] reshape(double[] x)
        {
            return x.Reshape(rows, cols);
        }

        /// <summary>
        /// Gets the cumulative distribution function (cdf) for
        /// this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns>System.Double.</returns>
        /// <remarks>The Cumulative Distribution Function (CDF) describes the cumulative
        /// probability that a given value or any value smaller than it will occur.</remarks>
        public double DistributionFunction(params double[] x)
        {
            return DistributionFunction(reshape(x));
        }

        /// <summary>
        /// Gets the complementary cumulative distribution function
        /// (ccdf) for this distribution evaluated at point <c>x</c>.
        /// This function is also known as the Survival function.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns>System.Double.</returns>
        /// <remarks>The Complementary Cumulative Distribution Function (CCDF) is
        /// the complement of the Cumulative Distribution Function, or 1
        /// minus the CDF.</remarks>
        public double ComplementaryDistributionFunction(params double[] x)
        {
            return ComplementaryDistributionFunction(reshape(x));
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">
        ///   The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data). </param>
        ///   
        public void Fit(double[][] observations)
        {
            Fit(observations, null);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">
        ///   The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data). </param>
        /// <param name="weights">
        ///   The weight vector containing the weight for each of the samples.</param>
        ///   
        public virtual void Fit(double[][] observations, double[] weights)
        {
            Fit(observations.Apply(x => x.Reshape(rows, cols)), weights);
        }

    }
}