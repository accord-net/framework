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

    /// <summary>
    ///   Inverse Wishart Distribution.
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    ///   The inverse Wishart distribution, also called the inverted Wishart distribution,
    ///   is a probability distribution defined on real-valued positive-definite matrices.
    ///   In Bayesian statistics it is used as the conjugate prior for the covariance matrix
    ///   of a multivariate normal distribution.</para>
    ///
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Inverse-Wishart_distribution">
    ///       Wikipedia, The Free Encyclopedia. Inverse Wishart distribution. 
    ///       Available from: http://en.wikipedia.org/wiki/Inverse-Wishart_distribution </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///   // Create a Inverse Wishart with the parameters
    ///   var invWishart = new InverseWishartDistribution(
    ///   
    ///       // Degrees of freedom
    ///       degreesOfFreedom: 4,
    ///   
    ///       // Scale parameter
    ///       inverseScale: new double[,] 
    ///       {
    ///            {  1.7, -0.2 },
    ///            { -0.2,  5.3 },
    ///       }
    ///   );
    ///   
    ///   // Common measures
    ///   double[] var = invWishart.Variance;  // { -3.4, -10.6 }
    ///   double[,] cov = invWishart.Covariance;  // see below
    ///   double[,] mmean = invWishart.MeanMatrix; // see below
    ///   
    ///   //        cov                mean
    ///   //   -5.78   -4.56        1.7  -0.2 
    ///   //   -4.56  -56.18       -0.2   5.3 
    ///   
    ///   // (the above matrix representations have been transcribed to text using)
    ///   string scov = cov.ToString(DefaultMatrixFormatProvider.InvariantCulture);
    ///   string smean = mmean.ToString(DefaultMatrixFormatProvider.InvariantCulture);
    ///   
    ///   // For compatibility reasons, .Mean stores a flattened mean matrix
    ///   double[] mean = invWishart.Mean; // { 1.7, -0.2, -0.2, 5.3 }
    ///   
    ///   
    ///   // Probability density functions
    ///   double pdf = invWishart.ProbabilityDensityFunction(new double[,] 
    ///   {
    ///       {  5.2,  0.2 }, // 0.000029806281690351203
    ///       {  0.2,  4.2 },
    ///   });
    ///   
    ///   double lpdf = invWishart.LogProbabilityDensityFunction(new double[,] 
    ///   {
    ///       {  5.2,  0.2 }, // -10.420791391688828
    ///       {  0.2,  4.2 },
    ///   });
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="WishartDistribution"/>
    /// 
    [Serializable]
    public class InverseWishartDistribution : MultivariateContinuousDistribution
    {
        int size;
        double v; // degrees of freedom
        double[,] inverseScaleMatrix; // Ψ (psi)

        double constant;
        double power;

        double[,] mean;
        double[] variance;
        double[,] covariance;

        /// <summary>
        ///   Creates a new Inverse Wishart distribution.
        /// </summary>
        /// 
        /// <param name="degreesOfFreedom">The degrees of freedom v.</param>
        /// <param name="inverseScale">The inverse scale matrix Ψ (psi).</param>
        /// 
        public InverseWishartDistribution(double degreesOfFreedom, double[,] inverseScale)
            : base(inverseScale.Length)
        {

            if (inverseScale.GetLength(0) != inverseScale.GetLength(1))
                throw new DimensionMismatchException("inverseScale", "Matrix must be square.");

            this.inverseScaleMatrix = inverseScale;
            this.size = inverseScale.GetLength(0);
            this.v = degreesOfFreedom;

            if (degreesOfFreedom <= size - 1)
                throw new ArgumentOutOfRangeException("degreesOfFreedom", "Degrees of freedom must be greater "
                + "than or equal to the number of rows in the inverse scale matrix.");

            var chol = new CholeskyDecomposition(inverseScale);

            if (!chol.PositiveDefinite)
                throw new NonPositiveDefiniteMatrixException("scale");
            if (!chol.Symmetric)
                throw new NonSymmetricMatrixException("scale");

            double a = Math.Pow(chol.Determinant, v / 2.0);
            double b = Math.Pow(2, (v * size) / 2.0);
            double c = Gamma.Multivariate(v / 2.0, size);

            this.constant = a / (b * c);
            this.power = -(v + size + 1) / 2.0;
        }


        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>A vector containing the mean values for the distribution.</value>
        /// 
        public double[,] MeanMatrix
        {
            get
            {
                if (mean == null)
                    mean = inverseScaleMatrix.Divide(v - size - 1);
                return mean;
            }
        }

        /// <summary>
        ///   Gets the mean for this distribution as a flat matrix.
        /// </summary>
        /// 
        /// <value>A vector containing the mean values for the distribution.</value>
        /// 
        public override double[] Mean
        {
            get { return MeanMatrix.Reshape(0); }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>A vector containing the variance values for the distribution.</value>
        /// 
        public override double[] Variance
        {
            get
            {
                if (variance == null)
                {
                    variance = new double[size];
                    for (int i = 0; i < variance.Length; i++)
                    {
                        double num = 2 * inverseScaleMatrix[i, i];
                        double den = (v - size - 1) * (v - size - 1) * (v - size - 3);
                        variance[i] = num / den;
                    }
                }
                return variance;
            }
        }


        /// <summary>
        ///   Gets the variance-covariance matrix for this distribution.
        /// </summary>
        /// 
        /// <value>A matrix containing the covariance values for the distribution.</value>
        /// 
        public override double[,] Covariance
        {
            get
            {
                if (covariance == null)
                {
                    covariance = new double[size, size];
                    for (int i = 0; i < size; i++)
                    {
                        double pii = inverseScaleMatrix[i, i];

                        for (int j = 0; j < size; j++)
                        {
                            double pij = inverseScaleMatrix[i, j];
                            double pjj = inverseScaleMatrix[j, j];

                            double num = (v - size + 1) * (pij * pij) + (v - size - 1) * (pii * pjj);
                            double den = (v - size) * (v - size - 1) * (v - size - 1) * (v - size - 3);

                            covariance[i, j] = num / den;
                        }
                    }
                }

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
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.
        ///   For a matrix distribution, such as the Wishart's, this
        ///   should be a positive-definite matrix or a matrix written
        ///   in flat vector form.
        /// </param>
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
            double[,] X = x.Reshape(size, size);
            return ProbabilityDensityFunction(X);
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.
        ///   For a matrix distribution, such as the Wishart's, this
        ///   should be a positive-definite matrix or a matrix written
        ///   in flat vector form.
        /// </param>
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
        public double ProbabilityDensityFunction(double[,] x)
        {
            var chol = new CholeskyDecomposition(x);

            double det = chol.Determinant;
            double[,] Vx = chol.Solve(inverseScaleMatrix);

            double z = -0.5 * Vx.Trace();
            double a = Math.Pow(det, power);
            double b = Math.Exp(z);

            return constant * a * b;
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.
        ///   For a matrix distribution, such as the Wishart's, this
        ///   should be a positive-definite matrix or a matrix written
        ///   in flat vector form.
        /// </param>
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
        public override double LogProbabilityDensityFunction(params double[] x)
        {
            double[,] X = x.Reshape(size, size);
            return LogProbabilityDensityFunction(X);
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.
        ///   For a matrix distribution, such as the Wishart's, this
        ///   should be a positive-definite matrix or a matrix written
        ///   in flat vector form.
        /// </param>
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
        public double LogProbabilityDensityFunction(double[,] x)
        {
            var chol = new CholeskyDecomposition(x);

            double det = chol.Determinant;
            double[,] Vx = chol.Solve(inverseScaleMatrix);

            double z = -0.5 * Vx.Trace();
            double a = Math.Pow(det, power);
            double b = Math.Exp(z);

            return Math.Log(constant) + power * Math.Log(det) + z;
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public override void Fit(double[][] observations, double[] weights, Fitting.IFittingOptions options)
        {
            throw new NotSupportedException();
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
            return new InverseWishartDistribution(v, inverseScaleMatrix);
        }
    }
}
