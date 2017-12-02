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
    using Accord.Math.Decompositions;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Compat;

    /// <summary>
    ///   Wishart Distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Wishart distribution is a generalization to multiple dimensions of 
    ///   the <see cref="ChiSquareDistribution">Chi-Squared distribution, or, in
    ///   the case of non-integer <see cref="DegreesOfFreedom"/>degrees of 
    ///   freedom</see>, of the <see cref="GammaDistribution">Gamma distribution
    ///   </see>.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Wishart_distribution">
    ///       Wikipedia, The Free Encyclopedia. Wishart distribution. 
    ///       Available from: http://en.wikipedia.org/wiki/Wishart_distribution </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///   // Create a Wishart distribution with the parameters:
    ///   WishartDistribution wishart = new WishartDistribution(
    ///   
    ///       // Degrees of freedom
    ///       degreesOfFreedom: 7,
    ///   
    ///       // Scale parameter
    ///       scale: new double[,] 
    ///       {
    ///           { 4, 1, 1 },  
    ///           { 1, 2, 2 },  // (must be symmetric and positive definite)
    ///           { 1, 2, 6 },
    ///       }
    ///   );
    ///   
    ///   // Common measures
    ///   double[] var = wishart.Variance;  // { 224, 56, 504 }
    ///   double[,] cov = wishart.Covariance;   // see below
    ///   double[,] meanm = wishart.MeanMatrix; // see below
    ///               
    ///   //         224  63  175             28  7   7 
    ///   //   cov =  63  56  112     mean =   7  14  14
    ///   //         175 112  504              7  14  42
    ///   
    ///   // (the above matrix representations have been transcribed to text using)
    ///   string scov = cov.ToString(DefaultMatrixFormatProvider.InvariantCulture);
    ///   string smean = meanm.ToString(DefaultMatrixFormatProvider.InvariantCulture);
    ///   
    ///   // For compatibility reasons, .Mean stores a flattened mean matrix
    ///   double[] mean = wishart.Mean; // { 28, 7, 7, 7, 14, 14, 7, 14, 42 }
    ///   
    ///   
    ///   // Probability density functions
    ///   double pdf = wishart.ProbabilityDensityFunction(new double[,] 
    ///   {
    ///       { 8, 3, 1 },
    ///       { 3, 7, 1 },   //   0.000000011082455043473361
    ///       { 1, 1, 8 },
    ///   });
    ///   
    ///   double lpdf = wishart.LogProbabilityDensityFunction(new double[,] 
    ///   {
    ///       { 8, 3, 1 },
    ///       { 3, 7, 1 },   // -18.317902605850534
    ///       { 1, 1, 8 },
    ///   });
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="InverseWishartDistribution"/>
    /// 
    [Serializable]
    public class WishartDistribution : MatrixContinuousDistribution
    {

        int size;
        int n;
        double[,] scaleMatrix;

        double constant;
        double lnconstant;
        double power;
        CholeskyDecomposition chol;

        double[,] mean;
        double[] variance;
        double[,] covariance;

        /// <summary>
        ///   Creates a new Wishart distribution.
        /// </summary>
        /// 
        /// <param name="dimension">The number of rows in the covariance matrices.</param>
        /// <param name="degreesOfFreedom">The degrees of freedom n.</param>
        /// 
        public WishartDistribution(int dimension, int degreesOfFreedom)
            : this(degreesOfFreedom, Matrix.Identity(dimension))
        {
        }

        /// <summary>
        ///   Creates a new Wishart distribution.
        /// </summary>
        /// 
        /// <param name="degreesOfFreedom">The degrees of freedom <c>n</c>.</param>
        /// <param name="scale">The positive-definite matrix scale matrix <c>V</c>.</param>
        /// 
        public WishartDistribution(int degreesOfFreedom, double[,] scale)
        : base(scale.Rows(), scale.Columns())
        {
            if (scale.GetLength(0) != scale.GetLength(1))
                throw new DimensionMismatchException("scale", "Matrix must be square.");

            this.scaleMatrix = scale;
            this.n = degreesOfFreedom;
            this.size = scale.GetLength(0);

            if (degreesOfFreedom <= size - 1)
                throw new ArgumentOutOfRangeException("degreesOfFreedom", "Degrees of freedom must be greater "
                + "than or equal to the number of rows in the scale matrix.");

            this.chol = new CholeskyDecomposition(scale);

            if (!chol.IsPositiveDefinite)
                throw new NonPositiveDefiniteMatrixException("scale");
            //if (!chol.Symmetric)
            //    throw new NonSymmetricMatrixException("scale");

            double a = Math.Pow(chol.Determinant, n / 2.0);
            double b = Math.Pow(2, (n * size) / 2.0);
            double c = Gamma.Multivariate(n / 2.0, size);

            this.constant = 1.0 / (a * b * c);
            this.lnconstant = Math.Log(constant);

            this.power = (n - size - 1) / 2.0;
        }

        /// <summary>
        ///   Gets the degrees of freedom for this Wishart distribution.
        /// </summary>
        /// 
        public double DegreesOfFreedom
        {
            get { return n; }
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>A vector containing the mean values for the distribution.</value>
        /// 
        public override double[,] Mean
        {
            get
            {
                if (mean == null)
                    mean = n.Multiply(scaleMatrix);
                return mean;
            }
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
                    for (int i = 0; i < size; i++)
                    {
                        double vii = scaleMatrix[i, i];
                        variance[i] = 2 * n * (vii * vii);
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
                        double vii = scaleMatrix[i, i];

                        for (int j = 0; j < size; j++)
                        {
                            double vij = scaleMatrix[i, j];
                            double vjj = scaleMatrix[j, j];

                            covariance[i, j] = n * (vij * vij + vii * vjj);
                        }
                    }
                }

                return covariance;
            }
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
        protected internal override double InnerProbabilityDensityFunction(double[,] x)
        {
            double det = x.Determinant();
            double[,] Vx = chol.Solve(x);

            double z = -0.5 * Vx.Trace();
            double a = Math.Pow(det, power);
            double b = Math.Exp(z);

            return constant * a * b;
        }

        /// <summary>
        ///   Gets the log-probability density function (pdf)
        ///   for this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.
        ///   For a matrix distribution, such as the Wishart's, this
        ///   should be a positive-definite matrix or a matrix written
        ///   in flat vector form.
        /// </param>
        ///   
        /// <returns>
        ///   The logarithm of the probability of <c>x</c>
        ///   occurring in the current distribution.
        /// </returns>
        /// 
        protected internal override double InnerLogProbabilityDensityFunction(double[,] x)
        {
            double det = x.Determinant();
            double[,] Vx = chol.Solve(x);

            double z = -0.5 * Vx.Trace();
            double a = power * Math.Log(det);

            return lnconstant + a + z;
        }


        /// <summary>
        /// Generates a random vector of observations from the current distribution.
        /// </summary>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <param name="source">The random number generator to use as a source of randomness.
        /// Default is to use <see cref="Accord.Math.Random.Generator.Random" />.</param>
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        public override double[][,] Generate(int samples, double[][,] result, Random source)
        {
            return Random(samples, n, scaleMatrix, result, source);
        }

        /// <summary>
        ///   Unsupported.
        /// </summary>
        /// 
        protected internal override double InnerDistributionFunction(double[,] x)
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
        public override void Fit(double[][,] observations, double[] weights, Fitting.IFittingOptions options)
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
            return new WishartDistribution(n, scaleMatrix);
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
            return String.Format(formatProvider, "Wishart(X)");
        }


        /// <summary>
        /// Generates a random vector of observations from the current distribution.
        /// </summary>
        /// <param name="degreesOfFreedom">The degrees of freedom <c>n</c>.</param>
        /// <param name="scale">The positive-definite matrix scale matrix <c>V</c>.</param>
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        public static double[,] Random(int degreesOfFreedom, double[,] scale)
        {
            return Random(1, degreesOfFreedom, scale)[0];
        }

        /// <summary>
        /// Generates a random vector of observations from the current distribution.
        /// </summary>
        /// <param name="degreesOfFreedom">The degrees of freedom <c>n</c>.</param>
        /// <param name="scale">The positive-definite matrix scale matrix <c>V</c>.</param>
        /// <param name="source">The random number generator to use as a source of randomness.
        /// Default is to use <see cref="Accord.Math.Random.Generator.Random" />.</param>
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        public static double[,] Random(int degreesOfFreedom, double[,] scale, Random source)
        {
            return Random(1, degreesOfFreedom, scale, source)[0];
        }

        /// <summary>
        /// Generates a random vector of observations from the current distribution.
        /// </summary>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="degreesOfFreedom">The degrees of freedom <c>n</c>.</param>
        /// <param name="scale">The positive-definite matrix scale matrix <c>V</c>.</param>
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        public static double[][,] Random(int samples, int degreesOfFreedom, double[,] scale)
        {
            return Random(samples, degreesOfFreedom, scale, Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        /// Generates a random vector of observations from the current distribution.
        /// </summary>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="degreesOfFreedom">The degrees of freedom <c>n</c>.</param>
        /// <param name="scale">The positive-definite matrix scale matrix <c>V</c>.</param>
        /// <param name="source">The random number generator to use as a source of randomness.
        /// Default is to use <see cref="Accord.Math.Random.Generator.Random" />.</param>
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        public static double[][,] Random(int samples, int degreesOfFreedom, double[,] scale, Random source)
        {
            int np = scale.GetLength(0);
            double[][,] result = new double[samples].Apply(x => new double[np, np]);
            return Random(samples, degreesOfFreedom, scale, result, source);
        }

        /// <summary>
        /// Generates a random vector of observations from the current distribution.
        /// </summary>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <param name="degreesOfFreedom">The degrees of freedom <c>n</c>.</param>
        /// <param name="scale">The positive-definite matrix scale matrix <c>V</c>.</param>
        /// <param name="source">The random number generator to use as a source of randomness.
        /// Default is to use <see cref="Accord.Math.Random.Generator.Random" />.</param>
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        public static double[][,] Random(int samples, int degreesOfFreedom, double[,] scale, double[][,] result, Random source)
        {
            int np = scale.GetLength(0);
            var chol = new CholeskyDecomposition(scale);
            var d = new double[np * (np + 1) / 2];
            for (int j = 0, l = 0; j < np; j++)
                for (int k = 0; k <= j; k++, l++)
                    d[l] = chol.LeftTriangularFactor[j, k];

            double[] r = new double[(np * (np + 1)) / 2];

            for (int i = 0; i < samples; i++)
            {
                wshrt(d, degreesOfFreedom, np, source, r);

                double[,] ret = result[i];
                for (int j = 0, l = 0; j < np; j++)
                    for (int k = 0; k <= j; k++, l++)
                        ret[j, k] = ret[k, j] = r[l];
            }

            return result;
        }


        /******************************************************************************/
        /*
          Purpose:

            RNORM returns two independent standard random normal deviates.

          Discussion:

            This routine sets U1 and U2 to two independent standardized 
            random normal deviates.   This is a version of the 
            method given in Knuth.

          Licensing:

            This code is distributed under the GNU LGPL license.

          Modified:

            16 April 2014

          Author:

            Original FORTRAN77 version by William Smith, Ronald Hocking.
            This C version by John Burkardt.

          Reference:

            Donald Knuth,
            The Art of Computer Programming,
            Volume 2, Seminumerical Algorithms,
            Third Edition,
            Addison Wesley, 1997,
            ISBN: 0201896842,
            LC: QA76.6.K64.

          Parameters:

            Input/output, int *SEED, a seed for the random 
            number generator.

            Output, double *U1, *U2, two standard random normal deviates.
        */
        static void rnorm(Random random, out double u1, out double u2)
        {
            for (; ; )
            {
                double x = random.NextDouble();
                double y = random.NextDouble();
                x = 2.0 * x - 1.0;
                y = 2.0 * y - 1.0;
                double s = x * x + y * y;

                if (s <= 1.0)
                {
                    s = Math.Sqrt(-2.0 * Math.Log(s) / s);
                    u1 = x * s;
                    u2 = y * s;
                    break;
                }
            }
            return;
        }

        /******************************************************************************/
        /*
          Purpose:

            WSHRT returns a random Wishart variate.

          Discussion:

            This routine is a Wishart variate generator.  

            On output, SA is an upper-triangular matrix of size NP * NP,
            written in linear form, column ordered, whose elements have a 
            Wishart(N, SIGMA) distribution.

          Licensing:

            This code is distributed under the GNU LGPL license.

          Modified:

            16 April 2014

          Author:

            Original FORTRAN77 version by William Smith, Ronald Hocking.
            This C version by John Burkardt.

          Reference:

            William Smith, Ronald Hocking,
            Algorithm AS 53, Wishart Variate Generator,
            Applied Statistics,
            Volume 21, Number 3, pages 341-345, 1972.

          Parameters:

            Input, double D[NP*(NP+1)/2], the upper triangular array that
            represents the Cholesky factor of the correlation matrix SIGMA.
            D is stored in column-major form.

            Input, int N, the number of degrees of freedom.
            1 <= N <= NP.

            Input, int NP, the size of variables.

            Input/output, int *SEED, a seed for the random 
            number generator.

            Output, double WSHART[NP*(NP+1)/2], a sample from the 
            Wishart distribution.
        */
        static void wshrt(double[] d, int n, int np, Random seed, double[] sa)
        {
            int k = 0;
            int nnp = (np * (np + 1)) / 2;
            /*
              Load SB with independent normal (0, 1) variates.
            */
            var sb = new double[nnp];

            while (k < nnp)
            {
                double u1 = 0;
                double u2 = 0;
                rnorm(seed, out u1, out u2);

                sb[k] = u1;
                k = k + 1;

                if (k < nnp)
                {
                    sb[k] = u2;
                    k = k + 1;
                }
            }
            /*
              Load diagonal elements with square root of chi-square variates.
            */
            int ns = 0;

            for (int i = 1; i <= np; i++)
            {
                double df = (double)(np - i + 1);
                ns = ns + i;
                double u1 = 2.0 / (9.0 * df);
                double u2 = 1.0 - u1;
                u1 = Math.Sqrt(u1);
                /*
                  Wilson-Hilferty formula for approximating chi-square variates:
                  The original code did not take the absolute value!
                */
                sb[ns - 1] = Math.Sqrt(df * Math.Abs(Math.Pow(u2 + sb[ns - 1] * u1, 3)));
            }

            double rn = (double)(n);
            int nr = 1;

            for (int i = 1; i <= np; i++)
            {
                nr = nr + i - 1;
                for (int j = i; j <= np; j++)
                {
                    int ip = nr;
                    int nq = (j * (j - 1)) / 2 + i - 1;
                    double c = 0.0;
                    for (k = i; k <= j; k++)
                    {
                        ip = ip + k - 1;
                        nq = nq + 1;
                        c = c + sb[ip - 1] * d[nq - 1];
                    }
                    sa[ip - 1] = c;
                }
            }

            for (int i = 1; i <= np; i++)
            {
                int ii = np - i + 1;
                int nq = nnp - np;
                for (int j = 1; j <= i; j++)
                {
                    int ip = (ii * (ii - 1)) / 2;
                    double c = 0.0;
                    for (k = i; k <= np; k++)
                    {
                        ip = ip + 1;
                        nq = nq + 1;
                        c = c + sa[ip - 1] * sa[nq - 1];
                    }
                    sa[nq - 1] = c / rn;
                    nq = nq - 2 * np + i + j - 1;
                }
            }

        }
    }
}
