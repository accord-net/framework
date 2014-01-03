// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
//
// Copyright © Antonino Porcino, 2010
// nino.porcino at yahoo.it
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

namespace Accord.Math.Decompositions
{
    using System;

    /// <summary>
    ///   Nonnegative Matrix Factorization.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Non-negative matrix factorization (NMF) is a group of algorithms in multivariate
    ///   analysis and linear algebra where a matrix <c>X</c> is factorized into (usually)
    ///   two matrices, <c>W</c> and <c>H</c>. The non-negative factorization enforces the
    ///   constraint that the factors <c>W</c> and <c>H</c> must be non-negative, i.e., all
    ///   elements must be equal to or greater than zero. The factorization is not unique.</para>
    ///   
    ///   <para>
    ///     References:
    ///     <list type="bullet">
    ///       <item><description>
    ///         <a href="http://en.wikipedia.org/wiki/Non-negative_matrix_factorization">
    ///         http://en.wikipedia.org/wiki/Non-negative_matrix_factorization </a>
    ///       </description></item>
    ///       <item><description>
    ///         Lee, D., Seung, H., 1999. Learning the Parts of Objects by Non-Negative
    ///         Matrix Factorization. Nature 401, 788–791. </description></item>
    ///       <item><description>
    ///         Michael W. Berry, et al. (June 2006). Algorithms and Applications for 
    ///         Approximate Nonnegative Matrix Factorization. </description></item>
    ///     </list>
    ///   </para>
    /// </remarks>
    /// 
    public class NonnegativeMatrixFactorization
    {
        private double[,] X;  // X is m x n (input data, must be positive)
        private double[,] W;  // W is m x r (weights)
        private double[,] H;  // H is r x n (transformed data) (transposed)

        private int n;   // number of input data vectors
        private int m;   // dimension of input vector
        private int r;   // dimension of output vector (reduced dimension)


        /// <summary>
        ///   Gets the nonnegative factor matrix W.
        /// </summary>
        /// 
        public double[,] LeftNonnegativeFactors
        {
            get { return W; }
        }

        /// <summary>
        ///   Gets the nonnegative factor matrix H.
        /// </summary>
        /// 
        public double[,] RightNonnegativeFactors
        {
            get { return H; }
        }

        /// <summary>
        ///   Initializes a new instance of the NMF algorithm
        /// </summary>
        /// 
        /// <param name="value">The input data matrix (must be positive).</param>
        /// <param name="r">The reduced dimension.</param>
        /// 
        public NonnegativeMatrixFactorization(double[,] value, int r)
            : this(value, r, 100) { }

        /// <summary>
        ///   Initializes a new instance of the NMF algorithm
        /// </summary>
        /// 
        /// <param name="value">The input data matrix (must be positive).</param>
        /// <param name="r">The reduced dimension.</param>
        /// <param name="maxiter">The number of iterations to perform.</param>
        /// 
        public NonnegativeMatrixFactorization(double[,] value, int r, int maxiter)
        {
            this.X = value;
            this.n = value.GetLength(0);
            this.m = value.GetLength(1);
            this.r = r;

            compute(maxiter);
        }

        /// <summary>
        ///    Performs NMF using the multiplicative method
        /// </summary>
        /// 
        /// <param name="maxiter">The maximum number of iterations</param>
        /// 
        /// <remarks>
        ///   At the end of the computation H contains the projected data
        ///   and W contains the weights. The multiplicative method is the
        ///   simplest factorization method.
        /// </remarks>
        /// 
        private void compute(int maxiter)
        {
            // chose W and H randomly, W with unit norm
            W = Accord.Math.Matrix.Random(m, r, 0, 1);
            H = Accord.Math.Matrix.Random(r, n, 0, 1);

            double[,] Z = new double[r, r];

            // a small epsilon is added to the
            //  denominator to avoid overflow.
            double eps = 10e-9;


            for (int t = 0; t < maxiter; t++)
            {
                double[,] newW = new double[m, r];
                double[,] newH = new double[r, n];


                // Update H using the multiplicative
                // H = H .* (W'*A) ./ (W'*W*H + eps) 
                for (int i = 0; i < r; i++)
                {
                    for (int j = i; j < r; j++)
                    {
                        double s = 0.0;
                        for (int l = 0; l < r; l++)
                            s += W[l, i] * W[l, j];
                        Z[i, j] = Z[j, i] = s;
                    }

                    for (int j = 0; j < n; j++)
                    {
                        double d = 0.0;
                        for (int l = 0; l < r; l++)
                            d += Z[i, l] * H[l, j];

                        double s = 0.0;
                        for (int l = 0; l < m; l++)
                            s += W[l, i] * X[j, l];

                        newH[i, j] = H[i, j] * s / (d + eps);
                    }
                }

                // Update W using the multiplicative
                //   W = W .* (A*H') ./ (W*H*H' + eps)
                for (int j = 0; j < r; j++)
                {
                    for (int i = j; i < r; i++)
                    {
                        double s = 0.0;
                        for (int l = 0; l < m; l++)
                            s += newH[i, l] * newH[j, l];
                        Z[i, j] = Z[j, i] = s;
                    }

                    for (int i = 0; i < r; i++)
                    {
                        double d = 0.0;
                        for (int l = 0; l < r; l++)
                            d += W[i, l] * Z[j, l];

                        double s = 0.0;
                        for (int l = 0; l < n; l++)
                            s += X[l, i] * newH[j, l];

                        newW[i, j] = W[i, j] * s / (d + eps);
                    }
                }

                W = newW;
                H = newH;
            }

           
        }
    }

}
