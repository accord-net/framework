﻿// Accord Statistics Library
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
    using Accord.Math.Decompositions;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Univariate;
    using System.Runtime.CompilerServices;
    using Accord.Math.Random;
    using Accord.Compat;

    /// <summary>
    ///   Multivariate Normal (Gaussian) distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Gaussian is the most widely used distribution for continuous
    ///   variables. In the case of many variables, it is governed by two
    ///   parameters, the mean vector and the variance-covariance matrix.</para>
    /// <para>
    ///   When a covariance matrix given to the class constructor is not positive
    ///   definite, the distribution is degenerate and this may be an indication
    ///   indication that it may be entirely contained in a r-dimensional subspace.
    ///   Applying a rotation to an orthogonal basis to recover a non-degenerate
    ///   r-dimensional distribution may help in this case.</para>
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://www.aiaccess.net/English/Glossaries/GlosMod/e_gm_positive_definite_matrix.htm">
    ///       Ai Access. Glossary of Data Modeling. Positive definite matrix. Available on:
    ///       http://www.aiaccess.net/English/Glossaries/GlosMod/e_gm_positive_definite_matrix.htm </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example shows how to create a Multivariate Gaussian
    ///   distribution with known parameters mean vector and covariance matrix
    /// </para>
    /// <code>
    ///   // Create a multivariate Gaussian distribution 
    ///   var dist = new MultivariateNormalDistribution(
    ///   
    ///       // mean vector mu
    ///       mean: new double[] 
    ///       {
    ///           4, 2 
    ///       },
    ///   
    ///       // covariance matrix sigma
    ///       covariance: new double[,] 
    ///       {
    ///           { 0.3, 0.1 },
    ///           { 0.1, 0.7 }
    ///       }
    ///   );
    ///   
    ///   // Common measures
    ///   double[] mean = dist.Mean;     // { 4, 2 }
    ///   double[] median = dist.Median; // { 4, 2 }
    ///   double[] var = dist.Variance;  // { 0.3, 0.7 } (diagonal from cov)
    ///   double[,] cov = dist.Covariance; // { { 0.3, 0.1 }, { 0.1, 0.7 } }
    ///   
    ///   // Probability mass functions
    ///   double pdf1 = dist.ProbabilityDensityFunction(new double[] { 2, 5 }); // 0.000000018917884164743237
    ///   double pdf2 = dist.ProbabilityDensityFunction(new double[] { 4, 2 }); // 0.35588127170858852
    ///   double pdf3 = dist.ProbabilityDensityFunction(new double[] { 3, 7 }); // 0.000000000036520107734505265
    ///   double lpdf = dist.LogProbabilityDensityFunction(new double[] { 3, 7 }); // -24.033158110192296
    ///   
    ///   // Cumulative distribution function (for up to two dimensions)
    ///   double cdf = dist.DistributionFunction(new double[] { 3, 5 }); // 0.033944035782101548
    ///   
    ///   // Generate samples from this distribution:
    ///   double[][] sample = dist.Generate(1000000);
    /// </code>
    /// 
    /// <para>
    ///   The following example demonstrates how to fit a multivariate Gaussian to
    ///   a set of observations. Since those observations would lead to numerical
    ///   difficulties, the example also demonstrates how to specify a regularization
    ///   constant to avoid getting a <see cref="NonPositiveDefiniteMatrixException"/>.
    /// </para>
    /// 
    /// <code>
    ///   double[][] observations = 
    ///   {
    ///       new double[] { 1, 2 },
    ///       new double[] { 1, 2 },
    ///       new double[] { 1, 2 },
    ///       new double[] { 1, 2 }
    ///   };
    ///   
    ///   // Create a multivariate Gaussian for 2 dimensions
    ///   var normal = new MultivariateNormalDistribution(2);
    ///   
    ///   // Specify a regularization constant in the fitting options
    ///   NormalOptions options = new NormalOptions() { Regularization = double.Epsilon };
    ///   
    ///   // Fit the distribution to the data
    ///   normal.Fit(observations, options);
    ///   
    ///   // Check distribution parameters
    ///   double[] mean = normal.Mean;     // { 1, 2 }
    ///   double[] var  = normal.Variance; // { 4.9E-324, 4.9E-324 } (almost machine zero)
    /// </code>
    /// 
    /// <para>
    ///   The next example shows how to estimate a Gaussian distribution from data
    ///   available inside a Microsoft Excel spreadsheet using the ExcelReader class.</para>
    ///   
    /// <code>
    ///   // Create a new Excel reader to read data from a spreadsheet
    ///   ExcelReader reader = new ExcelReader(@"test.xls", hasHeaders: false);
    ///   
    ///   // Extract the "Data" worksheet from the xls
    ///   DataTable table = reader.GetWorksheet("Data");
    ///   
    ///   // Convert the data table to a jagged matrix
    ///   double[][] observations = table.ToArray();
    ///   
    ///   
    ///   // Estimate a new Multivariate Normal Distribution from the observations
    ///   var dist = MultivariateNormalDistribution.Estimate(observations, new NormalOptions()
    ///   {
    ///       Regularization = 1e-10 // this value will be added to the diagonal until it becomes positive-definite
    ///   });
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="NormalDistribution"/>
    /// 
    [Serializable]
    public class MultivariateNormalDistribution : MultivariateContinuousDistribution,
        IFittableDistribution<double[], NormalOptions>,
        ISampleableDistribution<double[]>
    {

        // Distribution parameters
        private double[] mean;
        private double[,] covariance;

        private CholeskyDecomposition chol;
        private SingularValueDecomposition svd;
        private double lnconstant;

        // Derived measures
        private double[] variance;
        private double[,] leftTriangularFactor;


        /// <summary>
        ///   Constructs a multivariate Gaussian distribution
        ///   with zero mean vector and identity covariance matrix.
        /// </summary>
        /// 
        /// <param name="dimension">The number of dimensions in the distribution.</param>
        /// 
        public MultivariateNormalDistribution(int dimension)
            : this(dimension, true) { }


        private MultivariateNormalDistribution(int dimension, bool init)
            : base(dimension)
        {
            if (init)
            {
                // init is set to false during cloning
                double[] mean = new double[dimension];
                double[,] cov = Matrix.Identity(dimension);
                var chol = new CholeskyDecomposition(cov);
                initialize(mean, cov, chol, svd: null);
            }
        }

        /// <summary>
        ///   Constructs a multivariate Gaussian distribution
        ///   with given mean vector and covariance matrix.
        /// </summary>
        /// 
        /// <param name="mean">The mean vector μ (mu) for the distribution.</param>
        /// <param name="covariance">The covariance matrix Σ (sigma) for the distribution.</param>
        /// 
        public MultivariateNormalDistribution(double[] mean, double[][] covariance)
            : this(mean, covariance.ToMatrix())
        {
        }

        /// <summary>
        ///   Constructs a multivariate Gaussian distribution
        ///   with given mean vector and covariance matrix.
        /// </summary>
        /// 
        /// <param name="mean">The mean vector μ (mu) for the distribution.</param>
        /// 
        public MultivariateNormalDistribution(double[] mean)
            : this(mean, Matrix.Identity(mean.Length))
        {
        }

        /// <summary>
        ///   Constructs a multivariate Gaussian distribution
        ///   with given mean vector and covariance matrix.
        /// </summary>
        /// 
        /// <param name="mean">The mean vector μ (mu) for the distribution.</param>
        /// <param name="covariance">The covariance matrix Σ (sigma) for the distribution.</param>
        /// <param name="robust">Wether to allow non-positive definite covariance matrices by using the 
        ///   <see cref = "SingularValueDecomposition" > Singular Value Decomposition</see> instead of the
        ///   default <see cref="CholeskyDecomposition">Cholesky</see>. Default is false.</param>
        /// 
        public MultivariateNormalDistribution(double[] mean, double[,] covariance, bool robust = false)
            : base(mean.Length)
        {
            int rows = covariance.Rows();
            int cols = covariance.Columns();

            if (rows != cols)
                throw new DimensionMismatchException("covariance",
                    "Covariance matrix should be square.");

            if (mean.Length != rows)
                throw new DimensionMismatchException("covariance",
                    "Covariance matrix should have the same dimensions as mean vector's length.");

            if (!robust)
            {
                var chol = new CholeskyDecomposition(covariance);

                if (!chol.IsPositiveDefinite)
                {
                    throw new NonPositiveDefiniteMatrixException("Covariance matrix is not positive definite." +
                        " If are trying to estimate a distribution from data, please try using the Estimate method." +
                        " If you would like to force the creation of this distribution, please pass 'true' to the" +
                        " 'robust' argument of this constructor.");
                }

                initialize(mean, covariance, cd: chol, svd: null);
            }
            else
            {
                var svd = new SingularValueDecomposition(covariance, true, true, true);

                initialize(mean, covariance, cd: null, svd: svd);
            }
        }

        private void initialize(double[] m, double[,] cov, CholeskyDecomposition cd, SingularValueDecomposition svd)
        {
            int k = m.Length;

            this.mean = m;
            this.covariance = cov;
            this.chol = cd;
            this.svd = svd;
            this.leftTriangularFactor = null;

            if (chol != null || svd != null)
            {
                // Transforming to log operations, we have:
                double lndet = (cd != null) ? cd.LogDeterminant : svd.LogPseudoDeterminant;

                // So the log(constant) could be computed as:
                lnconstant = -(Constants.Log2PI * k + lndet) * 0.5;
            }
        }

        /// <summary>
        ///   Gets the Mean vector μ (mu) for 
        ///   the Gaussian distribution.
        /// </summary>
        /// 
        public override double[] Mean
        {
            get { return mean; }
        }

        /// <summary>
        ///   Gets the Variance vector diag(Σ), the diagonal of 
        ///   the sigma matrix, for the Gaussian distribution.
        /// </summary>
        /// 
        public override double[] Variance
        {
            get
            {
                if (variance == null)
                    variance = Matrix.Diagonal(covariance);
                return variance;
            }
        }

        /// <summary>
        ///   Gets the variance-covariance matrix
        ///   Σ (sigma) for the Gaussian distribution.
        /// </summary>
        /// 
        public override double[,] Covariance
        {
            get { return covariance; }
        }

        /// <summary>
        ///   Computes the cumulative distribution function for distributions
        ///   up to two dimensions. For more than two dimensions, this method
        ///   is not supported.
        /// </summary>
        /// 
        protected internal override double InnerDistributionFunction(params double[] x)
        {
            if (Dimension == 1)
            {
                double stdDev = Math.Sqrt(Covariance[0, 0]);

                if (stdDev == 0)
                    return (x[0] == mean[0]) ? 1 : 0;

                double z = (x[0] - mean[0]) / stdDev;

                return Normal.Function(z);
            }

            if (Dimension == 2)
            {
                double sigma1 = Math.Sqrt(Covariance[0, 0]);
                double sigma2 = Math.Sqrt(Covariance[1, 1]);
                double rho = Covariance[0, 1] / (sigma1 * sigma2);

                if (Double.IsNaN(rho))
                    return (x.IsEqual(mean)) ? 1 : 0;

                double z = (x[0] - mean[0]) / sigma1;
                double w = (x[1] - mean[1]) / sigma2;
                return Normal.Bivariate(z, w, rho);
            }

            throw new NotSupportedException("The cumulative distribution "
                + "function is only available for up to two dimensions.");
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
        protected internal override double InnerComplementaryDistributionFunction(params double[] x)
        {
            if (Dimension == 1)
            {
                double stdDev = Math.Sqrt(Covariance[0, 0]);
                double z = (x[0] - mean[0]) / stdDev;

                if (stdDev == 0)
                    return (x[0] == mean[0]) ? 0 : 1;

                return Normal.Complemented(z);
            }

            if (Dimension == 2)
            {
                double sigma1 = Math.Sqrt(Covariance[0, 0]);
                double sigma2 = Math.Sqrt(Covariance[1, 1]);
                double rho = Covariance[0, 1] / (sigma1 * sigma2);

                if (Double.IsNaN(rho))
                    return (x.IsEqual(mean)) ? 0 : 1;

                double z = (x[0] - mean[0]) / sigma1;
                double w = (x[1] - mean[1]) / sigma2;
                return Normal.BivariateComplemented(z, w, rho);
            }

            throw new NotSupportedException("The cumulative distribution "
                + "function is only available for up to two dimensions.");
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range. For a
        ///   univariate distribution, this should be a single
        ///   double value. For a multivariate distribution,
        ///   this should be a double array.</param>
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
        protected internal override double InnerProbabilityDensityFunction(params double[] x)
        {
            return Math.Exp(-0.5 * Mahalanobis(x) + lnconstant);
        }

        /// <summary>
        ///   Gets the log-probability density function (pdf)
        ///   for this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range. For a
        ///   univariate distribution, this should be a single
        ///   double value. For a multivariate distribution,
        ///   this should be a double array.</param>
        ///   
        /// <returns>
        ///   The logarithm of the probability of <c>x</c>
        ///   occurring in the current distribution.
        /// </returns>
        /// 
        protected internal override double InnerLogProbabilityDensityFunction(params double[] x)
        {
            return -0.5 * Mahalanobis(x) + lnconstant;
        }

        /// <summary>
        ///   Gets the Mahalanobis distance between a sample and this distribution.
        /// </summary>
        /// 
        /// <param name="x">A point in the distribution space.</param>
        /// 
        /// <returns>The Mahalanobis distance between the point and this distribution.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public double Mahalanobis(double[] x)
        {
            if (x.Length != Dimension)
                throw new DimensionMismatchException("x", "The vector should have the same dimension as the distribution.");

            var z = new double[mean.Length];
            for (int i = 0; i < x.Length; i++)
                z[i] = x[i] - mean[i];

            double[] a = (chol == null) ? svd.Solve(z) : chol.Solve(z);

            double b = 0;
            for (int i = 0; i < z.Length; i++)
                b += a[i] * z[i];
            return b;
        }


        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        ///   
        /// <example>
        ///   Please see <see cref="MultivariateNormalDistribution"/>.
        /// </example>
        /// 
        public override void Fit(double[][] observations, double[] weights, IFittingOptions options)
        {
            var normalOptions = options as NormalOptions;
            if (options != null && normalOptions == null)
                throw new ArgumentException("The specified options' type is invalid.", "options");

            Fit(observations, weights, normalOptions);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        /// 
        /// <example>
        ///   Please see <see cref="MultivariateNormalDistribution"/>.
        /// </example>
        /// 
        public void Fit(double[][] observations, double[] weights, NormalOptions options)
        {
            double[] mean;
            double[,] cov;


            if (weights != null)
            {
#if DEBUG
                for (int i = 0; i < weights.Length; i++)
                {
                    if (Double.IsNaN(weights[i]) || Double.IsInfinity(weights[i]))
                        throw new ArgumentException("Invalid numbers in the weight vector.", "weights");
                }
#endif
                // Compute weighted mean vector
                mean = Measures.WeightedMean(observations, weights);

                // Compute weighted covariance matrix
                if (options != null && options.Diagonal)
                    cov = Matrix.Diagonal(Measures.WeightedVariance(observations, weights, mean));
                else cov = Measures.WeightedCovariance(observations, weights, mean);
            }
            else
            {
                // Compute mean vector
                mean = Measures.Mean(observations, dimension: 0);

                // Compute covariance matrix
                if (options != null && options.Diagonal)
                    cov = Matrix.Diagonal(Measures.Variance(observations, mean));
                else cov = Measures.Covariance(observations, mean).ToMatrix();
            }


            if (options != null && options.Shared)
            {
                options.Postprocessing = (components, pi) =>
                {
                    var gaussians = components.To<MultivariateNormalDistribution[]>();

                    // Compute pooled variance
                    double[,] pooledCov = Measures.PooledCovariance(gaussians.Apply(x => x.covariance), pi);

                    // Decompose only once
                    decompose(options, pooledCov, out chol, out svd);

                    foreach (var gaussian in gaussians)
                        gaussian.initialize(gaussian.mean, pooledCov, chol, svd);
                };

                initialize(mean, cov, null, null);
            }
            else
            {
                decompose(options, cov, out chol, out svd);

                initialize(mean, cov, chol, svd);
            }
        }

        private static void decompose(NormalOptions options, double[,] cov, out CholeskyDecomposition chol, out SingularValueDecomposition svd)
        {
            svd = null;
            chol = null;

            if (options == null)
            {
                // We don't have options. Just attempt a standard fitting. If
                // the matrix is not positive semi-definite, throw an exception.

                chol = new CholeskyDecomposition(cov);

                if (!chol.IsPositiveDefinite)
                {
                    throw new NonPositiveDefiniteMatrixException("Covariance matrix is not positive "
                        + "definite. Try specifying a regularization constant in the fitting options "
                        + "(there is an example in the Multivariate Normal Distribution documentation).");
                }

                return;
            }
            else
            {
                // We have options. In this case, we will either be using the SVD
                // or we can add a regularization constant until the covariance
                // matrix becomes positive semi-definite.

                if (options.Robust)
                {
                    // No need to apply a regularization constant in this case
                    svd = new SingularValueDecomposition(cov, true, true, true);
                }
                else
                {
                    // Apply a regularization constant until covariance becomes positive
                    chol = new CholeskyDecomposition(cov);

                    // Check if need to add a regularization constant
                    double regularization = options.Regularization;

                    if (regularization > 0)
                    {
                        int dimension = cov.Rows();

                        while (!chol.IsPositiveDefinite)
                        {
                            for (int i = 0; i < dimension; i++)
                            {
                                for (int j = 0; j < dimension; j++)
                                    if (Double.IsNaN(cov[i, j]) || Double.IsInfinity(cov[i, j]))
                                        cov[i, j] = 0.0;

                                cov[i, i] += regularization;
                            }

                            chol = new CholeskyDecomposition(cov, false, true);
                        }
                    }

                    if (!chol.IsPositiveDefinite)
                    {
                        throw new NonPositiveDefiniteMatrixException("Covariance matrix is not positive "
                            + "definite. Try specifying a regularization constant in the fitting options "
                            + "(there is an example in the Multivariate Normal Distribution documentation).");
                    }
                }
            }
        }

        /// <summary>
        ///   Estimates a new Normal distribution from a given set of observations.
        /// </summary>
        /// 
        public static MultivariateNormalDistribution Estimate(double[][] observations)
        {
            return Estimate(observations, null, null);
        }

        /// <summary>
        ///   Estimates a new Normal distribution from a given set of observations.
        /// </summary>
        /// 
        /// <example>
        ///   Please see <see cref="MultivariateNormalDistribution"/>.
        /// </example>
        /// 
        public static MultivariateNormalDistribution Estimate(double[][] observations, NormalOptions options)
        {
            return Estimate(observations, null, options);
        }

        /// <summary>
        ///   Estimates a new Normal distribution from a given set of observations.
        /// </summary>
        /// 
        /// <example>
        ///   Please see <see cref="MultivariateNormalDistribution"/>.
        /// </example>
        /// 
        public static MultivariateNormalDistribution Estimate(double[][] observations, double[] weights)
        {
            var n = new MultivariateNormalDistribution(observations[0].Length);
            n.Fit(observations, weights, null);
            return n;
        }

        /// <summary>
        ///   Estimates a new Normal distribution from a given set of observations.
        /// </summary>
        /// 
        /// <example>
        ///   Please see <see cref="MultivariateNormalDistribution"/>.
        /// </example>
        /// 
        public static MultivariateNormalDistribution Estimate(double[][] observations, double[] weights, NormalOptions options)
        {
            var n = new MultivariateNormalDistribution(observations[0].Length);
            n.Fit(observations, weights, options);
            return n;
        }


        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public override object Clone()
        {
            var clone = new MultivariateNormalDistribution(this.Dimension, false);
            clone.lnconstant = lnconstant;
            clone.covariance = (double[,])covariance.Clone();
            clone.mean = (double[])mean.Clone();

            if (chol != null)
                clone.chol = (CholeskyDecomposition)chol.Clone();

            if (svd != null)
                clone.svd = (SingularValueDecomposition)svd.Clone();

            return clone;
        }

        /// <summary>
        ///   Converts this <see cref="MultivariateNormalDistribution">multivariate
        ///   normal distribution</see> into a <see cref="Independent{T}">joint distribution
        ///   of independent</see> <see cref="NormalDistribution">normal distributions</see>.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="Independent{T}">independent joint distribution</see> of 
        ///   <see cref="NormalDistribution">normal distributions</see>.
        /// </returns>
        /// 
        public Independent<NormalDistribution> ToIndependentNormalDistribution()
        {
            var components = new NormalDistribution[this.Dimension];
            for (int i = 0; i < components.Length; i++)
                components[i] = new NormalDistribution(this.Mean[i], Math.Sqrt(this.Variance[i]));
            return new Independent<NormalDistribution>(components);
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
        public override double[][] Generate(int samples, double[][] result, Random source)
        {
            double[] z = new double[Dimension];
            double[] u = Mean;

            if (leftTriangularFactor == null)
            {
                if (svd != null)
                {
                    leftTriangularFactor = new double[Dimension, Dimension];
                    double[,] V = svd.RightSingularVectors;
                    for (int i = 0; i < svd.Diagonal.Length; i++)
                    {
                        double d = Math.Sqrt(svd.Diagonal[i]);
                        for (int j = 0; j < Dimension; j++)
                            leftTriangularFactor[j, i] = d * V[j, i];
                    }
                }
                else
                {
                    leftTriangularFactor = chol.LeftTriangularFactor;
                }
            }

            for (int i = 0; i < samples; i++)
            {
                NormalDistribution.Random(Dimension, result: z, source: source);
                leftTriangularFactor.Dot(z, result: result[i]);
                result[i].Add(u, result: result[i]);
            }

            return result;
        }


        /// <summary>
        ///   Creates a new univariate Normal distribution.
        /// </summary>
        /// 
        /// <param name="mean">The mean value for the distribution.</param>
        /// <param name="stdDev">The standard deviation for the distribution.</param>
        /// 
        /// <returns>A <see cref="MultivariateNormalDistribution"/> object that
        /// actually represents a <see cref="Accord.Statistics.Distributions.Univariate.NormalDistribution"/>.</returns>
        /// 
        public static MultivariateNormalDistribution Univariate(double mean, double stdDev)
        {
            return new MultivariateNormalDistribution(new[] { mean }, new[,] { { stdDev * stdDev } });
        }

        /// <summary>
        ///   Creates a new bivariate Normal distribution.
        /// </summary>
        /// 
        /// <param name="mean1">The mean value for the first variate in the distribution.</param>
        /// <param name="mean2">The mean value for the second variate in the distribution.</param>
        /// <param name="stdDev1">The standard deviation for the first variate.</param>
        /// <param name="stdDev2">The standard deviation for the second variate.</param>
        /// <param name="rho">The correlation coefficient between the two distributions.</param>
        /// 
        /// <returns>A bi-dimensional <see cref="MultivariateNormalDistribution"/>.</returns>
        /// 
        public static MultivariateNormalDistribution Bivariate(double mean1, double mean2, double stdDev1, double stdDev2, double rho)
        {
            double[] mean = { mean1, mean2 };

            double[,] covariance =
            {
                { stdDev1 * stdDev1, stdDev1 * stdDev2 * rho },
                { stdDev1 * stdDev2 * rho, stdDev2 * stdDev2 },
            };

            return new MultivariateNormalDistribution(mean, covariance);
        }


        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// 
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// 
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// 
        public override string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format(formatProvider, "Normal(X; μ, Σ)");
        }

        /// <summary>
        ///   Generates a random vector of observations from a distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="mean">The mean vector μ (mu) for the distribution.</param>
        /// <param name="covariance">The covariance matrix Σ (sigma) for the distribution.</param>
        /// 
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public static double[][] Generate(int samples, double[] mean, double[,] covariance)
        {
            var normal = new MultivariateNormalDistribution(mean, covariance);
            return normal.Generate(samples);
        }

    }
}
