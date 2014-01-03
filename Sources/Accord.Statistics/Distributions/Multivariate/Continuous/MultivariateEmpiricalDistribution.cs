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
    using Accord.Math.Decompositions;
    using Accord.Statistics.Distributions.DensityKernels;
    using Accord.Statistics.Distributions.Fitting;

    /// <summary>
    ///   Multivariate empirical distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Empirical distributions are based solely on the data. This class
    ///   uses the empirical distribution function and the Gaussian kernel
    ///   density estimation to provide an univariate continuous distribution
    ///   implementation which depends only on sampled data.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia, The Free Encyclopedia. Empirical Distribution Function. Available on:
    ///       <a href=" http://en.wikipedia.org/wiki/Empirical_distribution_function">
    ///        http://en.wikipedia.org/wiki/Empirical_distribution_function </a></description></item>
    ///     <item><description>
    ///       PlanetMath. Empirical Distribution Function. Available on:
    ///       <a href="http://planetmath.org/encyclopedia/EmpiricalDistributionFunction.html">
    ///       http://planetmath.org/encyclopedia/EmpiricalDistributionFunction.html </a></description></item>
    ///     <item><description>
    ///       Wikipedia, The Free Encyclopedia. Kernel Density Estimation. Available on:
    ///       <a href="http://en.wikipedia.org/wiki/Kernel_density_estimation">
    ///       http://en.wikipedia.org/wiki/Kernel_density_estimation </a></description></item>
    ///     <item><description>
    ///       Bishop, Christopher M.; Pattern Recognition and Machine Learning. 
    ///       Springer; 1st ed. 2006.</description></item>
    ///     <item><description>
    ///       Buch-Kromann, T.; Nonparametric Density Estimation (Multidimension), 2007. Available in 
    ///       http://www.buch-kromann.dk/tine/nonpar/Nonparametric_Density_Estimation_multidim.pdf </description></item>
    ///     <item><description>
    ///       W. Härdle, M. Müller, S. Sperlich, A. Werwatz; Nonparametric and Semiparametric Models, 2004. Available
    ///       in http://sfb649.wiwi.hu-berlin.de/fedc_homepage/xplore/ebooks/html/spm/spmhtmlnode18.html 
    ///       </description></item>
    ///  </list></para>  
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///   // Suppose we have the following data, and we would
    ///   // like to estimate a distribution from this data
    ///   
    ///   double[][] samples =
    ///   {
    ///       new double[] { 0, 1 },
    ///       new double[] { 1, 2 },
    ///       new double[] { 5, 1 },
    ///       new double[] { 7, 1 },
    ///       new double[] { 6, 1 },
    ///       new double[] { 5, 7 },
    ///       new double[] { 2, 1 },
    ///   };
    ///   
    ///   // Start by specifying a density kernel
    ///   IDensityKernel kernel = new EpanechnikovKernel(dimension: 2);
    ///   
    ///   // Create a multivariate Empirical distribution from the samples
    ///   var dist = new MultivariateEmpiricalDistribution(kernel, samples);
    ///   
    ///   
    ///   // Common measures
    ///   double[] mean = dist.Mean;     // { 3.71, 2.00 }
    ///   double[] median = dist.Median; // { 3.71, 2.00 }
    ///   double[] var = dist.Variance;  // { 7.23, 5.00 } (diagonal from cov)
    ///   double[,] cov = dist.Covariance; // { { 7.23, 0.83 }, { 0.83, 5.00 } }
    ///   
    ///   // Probability mass functions
    ///   double pdf1 = dist.ProbabilityDensityFunction(new double[] { 2, 1 }); // 0.039131176997318849
    ///   double pdf2 = dist.ProbabilityDensityFunction(new double[] { 4, 2 }); // 0.010212109770266639
    ///   double pdf3 = dist.ProbabilityDensityFunction(new double[] { 5, 7 }); // 0.02891906722705221
    ///   double lpdf = dist.LogProbabilityDensityFunction(new double[] { 5, 7 }); // -3.5432541357714742
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="IDensityKernel"/>
    /// <seealso cref="Accord.Statistics.Distributions.Univariate.EmpiricalDistribution"/>
    /// 
    [Serializable]
    public class MultivariateEmpiricalDistribution : MultivariateContinuousDistribution,
        IFittableDistribution<double[], MultivariateEmpiricalOptions>
    {

        // Distribution parameters
        double[][] samples;
        double[,] smoothing;
        double determinant;

        IDensityKernel kernel;

        private double[] mean;
        private double[] variance;
        private double[,] covariance;

        /// <summary>
        ///   Creates a new Empirical Distribution from the data samples.
        /// </summary>
        /// 
        /// <param name="samples">The data samples.</param>
        /// 
        public MultivariateEmpiricalDistribution(double[][] samples)
            : base(samples[0].Length)
        {
            this.initialize(null, samples, null);
        }

        /// <summary>
        ///   Creates a new Empirical Distribution from the data samples.
        /// </summary>
        /// 
        /// <param name="kernel">The kernel density function to use. 
        ///   Default is to use the <see cref="GaussianKernel"/>.</param>
        /// <param name="samples">The data samples forming the distribution.</param>
        /// 
        public MultivariateEmpiricalDistribution(IDensityKernel kernel, double[][] samples)
            : base(samples[0].Length)
        {
            this.initialize(kernel, samples, null);
        }

        /// <summary>
        ///   Creates a new Empirical Distribution from the data samples.
        /// </summary>
        /// 
        /// <param name="kernel">The kernel density function to use. 
        ///   Default is to use the <see cref="GaussianKernel"/>.</param>
        /// <param name="samples">The data samples.</param>
        /// <param name="smoothing">
        ///   The kernel smoothing or bandwidth to be used in density estimation.
        ///   By default, the normal distribution approximation will be used.</param>
        /// 
        public MultivariateEmpiricalDistribution(IDensityKernel kernel, double[][] samples, double[,] smoothing)
            : base(samples[0].Length)
        {
            this.initialize(kernel, samples, smoothing);
        }

        /// <summary>
        ///   Gets the kernel density function used in this distribution.
        /// </summary>
        /// 
        public IDensityKernel Kernel
        {
            get { return kernel; }
        }

        /// <summary>
        ///   Gets the samples giving this empirical distribution.
        /// </summary>
        /// 
        public double[][] Samples
        {
            get { return samples; }
        }

        /// <summary>
        ///   Gets the bandwidth smoothing parameter
        ///   used in the kernel density estimation.
        /// </summary>
        /// 
        public double[,] Smoothing
        {
            get { return smoothing; }
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
        public override double ProbabilityDensityFunction(double[] x)
        {
            // http://www.buch-kromann.dk/tine/nonpar/Nonparametric_Density_Estimation_multidim.pdf
            // http://sfb649.wiwi.hu-berlin.de/fedc_homepage/xplore/ebooks/html/spm/spmhtmlnode18.html

            double sum = 0;

            CholeskyDecomposition chol = new CholeskyDecomposition(smoothing);

            double[] delta = new double[Dimension];
            for (int i = 0; i < samples.Length; i++)
            {
                for (int j = 0; j < x.Length; j++)
                    delta[j] = (x[j] - samples[i][j]);

                sum += kernel.Function(chol.Solve(delta));
            }

            return sum / (samples.Length * determinant);
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
        public override double LogProbabilityDensityFunction(params double[] x)
        {
            return Math.Log(ProbabilityDensityFunction(x));
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   A vector containing the mean values for the distribution.
        /// </value>
        /// 
        public override double[] Mean
        {
            get
            {
                if (mean == null)
                    mean = Accord.Statistics.Tools.Mean(samples);
                return mean;
            }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   A vector containing the variance values for the distribution.
        /// </value>
        /// 
        public override double[] Variance
        {
            get
            {
                if (variance == null)
                    variance = Accord.Statistics.Tools.Variance(samples, Mean);
                return variance;
            }
        }

        /// <summary>
        ///   Gets the variance-covariance matrix for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   A matrix containing the covariance values for the distribution.
        /// </value>
        /// 
        public override double[,] Covariance
        {
            get
            {
                if (covariance == null)
                    covariance = Accord.Statistics.Tools.Covariance(samples, Mean);
                return covariance;
            }
        }

        /// <summary>
        ///   Not supported.
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
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        ///   
        public override void Fit(double[][] observations, double[] weights, Fitting.IFittingOptions options)
        {
            Fit(observations, weights, options as EmpiricalOptions);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against.</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        ///
        public void Fit(double[][] observations, double[] weights = null, MultivariateEmpiricalOptions options = null)
        {
            if (weights != null)
                throw new ArgumentException("This distribution does not support weighted samples.");

            if (options != null)
                throw new ArgumentException("This method does not accept fitting options.");

            double[,] smoothing = null;

            if (options != null)
                smoothing = options.SmoothingRule(observations);

            initialize(null, (double[][])observations.Clone(), smoothing);
        }

        private void initialize(IDensityKernel kernel, double[][] observations, double[,] smoothing)
        {
            if (smoothing == null)
                smoothing = SilvermanRule(observations);

            if (kernel == null)
                kernel = new GaussianKernel(Dimension);

            this.kernel = kernel;
            this.samples = observations;
            this.smoothing = smoothing;
            this.determinant = smoothing.Determinant();

            this.mean = null;
            this.variance = null;
            this.covariance = null;
        }

        private MultivariateEmpiricalDistribution(int dimension)
            : base(dimension)
        {
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
            var e = new MultivariateEmpiricalDistribution(Dimension);
            e.initialize(kernel, samples.MemberwiseClone(), (double[,])smoothing.Clone());
            return e;
        }


        /// <summary>
        ///   Gets the Silverman's rule. estimative of the smoothing parameter.
        ///   This is the default smoothing rule applied used when estimating 
        ///   <see cref="MultivariateEmpiricalDistribution"/>s.
        /// </summary>
        /// 
        /// <remarks>
        ///   This method is described on Wikipedia, at
        ///   http://en.wikipedia.org/wiki/Multivariate_kernel_density_estimation
        /// </remarks>
        /// 
        /// <param name="observations">The observations for the empirical distribution.</param>
        /// 
        /// <returns>An estimative of the smoothing parameter.</returns>
        /// 
        public static double[,] SilvermanRule(double[][] observations)
        {
            // Silverman's rule
            //  - http://en.wikipedia.org/wiki/Multivariate_kernel_density_estimation

            double[] sigma = Statistics.Tools.StandardDeviation(observations);

            double d = sigma.Length;
            double n = observations.Length;


            var smoothing = new double[sigma.Length, sigma.Length];
            for (int i = 0; i < sigma.Length; i++)
            {
                double a = Math.Pow(4 / (d + 2), 1 / (d + 4));
                double b = Math.Pow(n, -1 / (d + 4));
                smoothing[i, i] = a * b * sigma[i];
            }

            return smoothing;
        }

    }
}
