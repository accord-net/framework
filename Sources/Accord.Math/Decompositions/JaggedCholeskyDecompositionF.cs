// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
//
// Original work copyright © Lutz Roeder, 2000
//  Adapted from Mapack for .NET, September 2000
//  Adapted from Mapack for COM and Jama routines
//  http://www.aisto.com/roeder/dotnet
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
    using System.Threading.Tasks;

    /// <summary>
    ///        Cholesky Decomposition of a symmetric, positive definite matrix.
    ///    </summary>
    /// <remarks>
    ///   <para>
    ///        For a symmetric, positive definite matrix <c>A</c>, the Cholesky decomposition is a
    ///        lower triangular matrix <c>L</c> so that <c>A = L * L'</c>. The presented algorithm
    ///        only checks the upper triangular part of the matrix given as parameter and assumes
    ///        it is symmetric. If the matrix is not positive definite, the constructor returns a 
    ///        partial decomposition and sets two internal variables that can be queried using the
    ///        <see cref="PositiveDefinite"/> properties.</para>
    ///   <para>
    ///     Any square matrix A with non-zero pivots can be written as the product of a
    ///     lower triangular matrix L and an upper triangular matrix U; this is called
    ///     the LU decomposition. However, if A is symmetric and positive definite, we
    ///     can choose the factors such that U is the transpose of L, and this is called
    ///     the Cholesky decomposition. Both the LU and the Cholesky decomposition are
    ///     used to solve systems of linear equations.</para>
    ///   <para>
    ///     When it is applicable, the Cholesky decomposition is twice as efficient
    ///     as the LU decomposition.</para>
    ///    </remarks>
    ///    
    [Serializable]
    public sealed class JaggedCholeskyDecompositionF : ICloneable, ISolverArrayDecomposition<float>
    {

        private float[][] L;
        private float[] D;
        private int dimension;

        private bool positiveDefinite;
        private bool undefined;
        private bool robust;
        private bool destroyed;

        // cache for lazy evaluation
        private float[][] leftTriangularFactor;
        private float[][] diagonalMatrix;
        private float? determinant;
        private double? lndeterminant;
        private bool? nonsingular;


        /// <summary>
        ///   Constructs a new Cholesky Decomposition.
        /// </summary>
        /// 
        /// <param name="value">
        ///   The symmetric matrix, given in upper triangular form, to be decomposed.</param>
        /// <param name="robust">
        ///   True to perform a square-root free LDLt decomposition, false otherwise.</param>
        /// <param name="inPlace">
        ///   True to perform the decomposition in place, storing the factorization in the
        ///   lower triangular part of the given matrix.</param>
        /// 
        public JaggedCholeskyDecompositionF(float[][] value, bool robust = false, bool inPlace = false)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Matrix cannot be null.");

            if (value.Length != value[0].Length)
                throw new DimensionMismatchException("value", "Matrix is not square.");


            if (robust)
            {
                LDLt(value, inPlace); // Compute square-root free decomposition
            }
            else
            {
                LLt(value, inPlace); // Compute standard Cholesky decomposition
            }
        }

        /// <summary>
        ///   Returns <see langword="true"/> if the matrix is positive definite.
        /// </summary>
        public bool PositiveDefinite
        {
            get { return this.positiveDefinite; }
        }

        /// <summary>
        ///   Gets a value indicating whether the LDLt factorization
        ///   has been computed successfully or if it is undefined.
        /// </summary>
        /// 
        /// <value>
        ///     <c>true</c> if the factorization is not defined; otherwise, <c>false</c>.
        /// </value>
        /// 
        public bool IsNotDefined
        {
            get { return this.undefined; }
        }

        /// <summary>
        ///   Returns the left (lower) triangular factor
        ///   <c>L</c> so that <c>A = L * D * L'</c>.
        /// </summary>
        /// 
        public float[][] LeftTriangularFactor
        {
            get
            {
                if (leftTriangularFactor == null)
                {
                    if (destroyed)
                        throw new InvalidOperationException("The decomposition has been destroyed.");
                        
                    var left = new float[L.Length][];
                    for (int i = 0; i < left.Length; i++)
                    {
                        left[i] = new float[L.Length];
                        for (int j = 0; j <= i; j++)
                            left[i][j] = L[i][j];
                    }

                    leftTriangularFactor = left;
                }

                return leftTriangularFactor;
            }
        }

        /// <summary>
        ///   Returns the block diagonal matrix of diagonal
        ///   elements in a LDLt decomposition.
        /// </summary>        
        ///   
        public float[][] DiagonalMatrix
        {
            get
            {
                if (diagonalMatrix == null)
                {
                    if (destroyed)
                        throw new InvalidOperationException("The decomposition has been destroyed.");
                        
                    diagonalMatrix = new float[D.Length][];
                    for (int i = 0; i < diagonalMatrix.Length; i++)
                    {
                        diagonalMatrix[i] = new float[D.Length];
                        diagonalMatrix[i][i] = D[i];
                    }
                }

                return diagonalMatrix;
            }
        }

        /// <summary>
        ///   Returns the one-dimensional array of diagonal 
        ///   elements in a LDLt decomposition.
        /// </summary>        
        /// 
        public float[] Diagonal
        {
            get { return D; }
        }

        /// <summary>
        ///   Returns the determinant of
        ///   the decomposed matrix.
        /// </summary>
        /// 
        public float Determinant
        {
            get
            {
                if (!determinant.HasValue)
                {
                    if (destroyed)
                        throw new InvalidOperationException("The decomposition has been destroyed.");
                        
                    float detL = 1, detD = 1;
                    for (int i = 0; i < L.Length; i++)
                        detL *= L[i][i];

                    if (D != null)
                    {
                        for (int i = 0; i < L.Length; i++)
                            detD *= D[i];
                    }

                    determinant = detL * detL * detD;
                }

                return determinant.Value;
            }
        }

        /// <summary>
        ///   If the matrix is positive-definite, returns the
        ///   log-determinant of the decomposed matrix.
        /// </summary>
        /// 
        public double LogDeterminant
        {
            get
            {
                if (!lndeterminant.HasValue)
                {
                    if (destroyed)
                        throw new InvalidOperationException("The decomposition has been destroyed.");
                        
                    double detL = 0, detD = 0;
                    for (int i = 0; i < L.Length; i++)
                        detL += Math.Log((double)L[i][i]);

                    if (D != null)
                    {
                        for (int i = 0; i < D.Length; i++)
                            detD += Math.Log((double)D[i]);
                    }

                    lndeterminant = detL + detL + detD;
                }

                return lndeterminant.Value;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the decomposed
        ///   matrix is non-singular (i.e. invertible).
        /// </summary>
        /// 
        public bool Nonsingular
        {
            get
            {
                if (!nonsingular.HasValue)
                {
                    if (destroyed)
                        throw new InvalidOperationException("The decomposition has been destroyed.");
                        
                    bool nonSingular = true;
                    for (int i = 0; i < dimension && nonSingular; i++)
                        if (L[i][i] == 0 || D[i] == 0) nonSingular = false;

                    nonsingular = nonSingular;
                }

                return nonsingular.Value;
            }
        }


        private unsafe void LLt(float[][] value, bool inPlace = false)
        {
            dimension = value.Length;
            L = inPlace ? value : value.MemberwiseClone();
            robust = false;

            this.positiveDefinite = true;

            for (int j = 0; j < L.Length; j++)
            {
                float s = 0;
                for (int k = 0; k < j; k++)
                {
                    float t = L[j][k];
                    for (int i = 0; i < k; i++)
                        t -= L[j][i] * L[k][i];
                    t = t / L[k][k];

                    L[j][k] = t;
                    s += t * t;
                }

                s = L[j][j] - s;

                // Use a tolerance for positive-definiteness
                this.positiveDefinite &= (s > (float)1e-14 * Math.Abs(L[j][j]));

                L[j][j] = (float)Math.Sqrt((double)s);
            }
        }


        private unsafe void LDLt(float[][] value, bool inPlace)
        {
            dimension = value.Length;
            L = inPlace ? value : value.MemberwiseClone();
            D = new float[dimension];
            robust = true;

            float[] v = new float[dimension];
            this.positiveDefinite = true;

            for (int i = 0; i < L.Length; i++)
            {
                for (int j = 0; j < i; j++)
                    v[j] = L[i][j] * D[j];

                float sum1 = 0;
                for (int k = 0; k < i; k++)
                    sum1 += L[i][k] * v[k];

                D[i] = v[i] = L[i][i] - sum1;

                // If one of the diagonal elements is zero, the 
                // decomposition (without pivoting) is undefined.
                if (v[i] == 0) { undefined = true; return; }

                // Use a tolerance for positive-definiteness
                this.positiveDefinite &= (v[i] > (float)1e-14 * Math.Abs(L[i][i]));
				
                Parallel.For(i + 1, L.Length, k =>
                {
                     float sum2 = 0;
                     for (int j = 0; j < i; j++)
                         sum2 += L[k][j] * v[j];

                     L[k][i] = (L[i][k] - sum2) / v[i];
                });
            }

            for (int i = 0; i < L.Length; i++)
                L[i][i] = 1;
        }

        /// <summary>
        ///   Solves a set of equation systems of type <c>A * X = B</c>.
        /// </summary>
        /// 
        /// <param name="value">Right hand side matrix with as many rows as <c>A</c> and any number of columns.</param>
        /// <returns>Matrix <c>X</c> so that <c>L * L' * X = B</c>.</returns>
        /// <exception cref="T:System.ArgumentException">Matrix dimensions do not match.</exception>
        /// <exception cref="T:System.NonSymmetricMatrixException">Matrix is not symmetric.</exception>
        /// <exception cref="T:System.NonPositiveDefiniteMatrixException">Matrix is not positive-definite.</exception>
        /// 
        public float[][] Solve(float[][] value)
        {
            return Solve(value, false);
        }

        /// <summary>
        ///   Solves a set of equation systems of type <c>A * X = B</c>.
        /// </summary>
        /// 
        /// <param name="value">Right hand side matrix with as many rows as <c>A</c> and any number of columns.</param>
        /// <returns>Matrix <c>X</c> so that <c>L * L' * X = B</c>.</returns>
        /// <exception cref="T:System.ArgumentException">Matrix dimensions do not match.</exception>
        /// <exception cref="T:System.NonSymmetricMatrixException">Matrix is not symmetric.</exception>
        /// <exception cref="T:System.NonPositiveDefiniteMatrixException">Matrix is not positive-definite.</exception>
        /// <param name="inPlace">True to compute the solving in place, false otherwise.</param>
        /// 
        public float[][] Solve(float[][] value, bool inPlace)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length != dimension)
                throw new ArgumentException("Argument matrix should have the same number of rows as the decomposed matrix.", "value");

            if (!robust && !positiveDefinite)
                throw new NonPositiveDefiniteMatrixException("Decomposed matrix is not positive definite.");

            if (destroyed)
                throw new InvalidOperationException("The decomposition has been destroyed.");

            int count = value[0].Length;
            var B = inPlace ? value : value.MemberwiseClone();

            // Solve L*Y = B;
            for (int k = 0; k < L.Length; k++)
            {
                for (int j = 0; j < B[k].Length; j++)
                {
                    for (int i = 0; i < k; i++)
                        B[k][j] -= B[i][j] * L[k][i];
                    B[k][j] /= L[k][k];
                }
            }

            if (robust)
            {
                for (int k = 0; k < D.Length; k++)
                    for (int j = 0; j < B[k].Length; j++)
                        B[k][j] /= D[k];
            }

            // Solve L'*X = Y;
            for (int k = L.Length - 1; k >= 0; k--)
            {
                for (int j = 0; j < B[k].Length; j++)
                {
                    for (int i = k + 1; i < L.Length; i++)
                        B[k][j] -= B[i][j] * L[i][k];

                    B[k][j] /= L[k][k];
                }
            }

            return B;
        }

        /// <summary>
        ///   Solves a set of equation systems of type <c>A * x = b</c>.
        /// </summary>
        /// 
        /// <param name="value">Right hand side column vector with as many rows as <c>A</c>.</param>
        /// <returns>Vector <c>x</c> so that <c>L * L' * x = b</c>.</returns>
        /// <exception cref="T:System.ArgumentException">Matrix dimensions do not match.</exception>
        /// <exception cref="T:System.NonSymmetricMatrixException">Matrix is not symmetric.</exception>
        /// <exception cref="T:System.NonPositiveDefiniteMatrixException">Matrix is not positive-definite.</exception>
        /// 
        public float[] Solve(float[] value)
        {
            return Solve(value, false);
        }

        /// <summary>
        ///   Solves a set of equation systems of type <c>A * x = b</c>.
        /// </summary>
        /// 
        /// <param name="value">Right hand side column vector with as many rows as <c>A</c>.</param>
        /// <returns>Vector <c>x</c> so that <c>L * L' * x = b</c>.</returns>
        /// <exception cref="T:System.ArgumentException">Matrix dimensions do not match.</exception>
        /// <exception cref="T:System.NonSymmetricMatrixException">Matrix is not symmetric.</exception>
        /// <exception cref="T:System.NonPositiveDefiniteMatrixException">Matrix is not positive-definite.</exception>
        /// <param name="inPlace">True to compute the solving in place, false otherwise.</param>
        /// 
        public float[] Solve(float[] value, bool inPlace)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length != dimension)
                throw new ArgumentException("Argument vector should have the same length as rows in the decomposed matrix.", "value");

            if (!robust && !positiveDefinite)
                throw new NonPositiveDefiniteMatrixException("Decomposed matrix is not positive definite.");
                
            if (destroyed)
                throw new InvalidOperationException("The decomposition has been destroyed.");

            var B = inPlace ? value : (float[])value.Clone();

            // Solve L*Y = B;
            for (int k = 0; k < L.Length; k++)
            {
                for (int i = 0; i < k; i++)
                    B[k] -= B[i] * L[k][i];
                B[k] /= L[k][k];
            }

            if (robust)
            {
                for (int k = 0; k < D.Length; k++)
                    B[k] /= D[k];
            }

            // Solve L'*X = Y;
            for (int k = L.Length - 1; k >= 0; k--)
            {
                for (int i = k + 1; i < L.Length; i++)
                    B[k] -= B[i] * L[i][k];
                B[k] /= L[k][k];
            }
            
            return B;
        }

        /// <summary>
        ///   Solves a set of equation systems of type <c>A * X = I</c>.
        /// </summary>
        /// 
        public float[][] Inverse()
        {
            if (!robust && !positiveDefinite)
                throw new NonPositiveDefiniteMatrixException("Matrix is not positive definite.");

            if (destroyed)
                throw new InvalidOperationException("The decomposition has been destroyed.");
                
            // References:
            // http://books.google.com/books?id=myzIPBwyBbcC&pg=PA119

            var C = new float[dimension][];
            for (int i = 0; i < C.Length; i++)
                C[i] = new float[dimension];


            int n = C.Length - 1;

            // Compute last element C[n][n]
            C[n][n] = 1 / (L[n][n] * L[n][n]);

            // Compute last column (eq 2.8.12)
            for (int i = n - 1; i >= 0; i--)
            {
                float sum = 0;
                for (int j = i + 1; j < L.Length; j++)
                    sum += L[j][i] * C[n][j];
                C[n][i] = C[i][n] = -(1 / L[i][i]) * sum;
            }

            // Compute the diagonal (eq 2.8.13)
            for (int k = C.Length - 2; k >= 0; k--)
            {
                float sum = 0;
                for (int j = k + 1; j < L.Length; j++)
                    sum += L[j][k] * C[j][k];

                if (robust)
                    C[k][k] = (1 / D[k]) - ((1 / L[k][k]) * sum);
                else
                    C[k][k] = (1 / L[k][k]) * ((1 / L[k][k]) - sum);

                // Compute remaining (eq 2.8.14)
                for (int i = k - 1; i >= 0; i--)
                {
                    float sum1 = 0;
                    for (int j = i + 1; j <= k; j++)
                        sum1 += L[j][i] * C[k][j];

                    float sum2 = 0;
                    for (int j = k + 1; j <= n; j++)
                        sum2 += L[j][i] * C[j][k];

                    C[i][k] = C[k][i] = -(1 / L[i][i]) * (sum1 + sum2);
                }
            }


            return C;
        }

        /// <summary>
        ///   Computes the diagonal of the inverse of the decomposed matrix.
        /// </summary>
        /// 
        public float[] InverseDiagonal(bool destroy = false)
        {
            float[] diagonal = new float[L.Length];
            InverseDiagonal(diagonal, destroy);
            return diagonal;
        }
        
        /// <summary>
        ///   Computes the diagonal of the inverse of the decomposed matrix.
        /// </summary>
        ///
        /// <param name="destroy">True to conserve memory by reusing the
        ///    same space used to hold the decomposition, thus destroying
        ///    it in the process. Pass false otherwise.</param>
        /// <param name="result">The array to hold the result of the 
        ///    computation. Should be of same length as the diagonal
        ///    of the original matrix.</param>
        /// 
        public void InverseDiagonal(float[] result, bool destroy = false)
        {
            if (!robust && !positiveDefinite)
                throw new NonPositiveDefiniteMatrixException("Matrix is not positive definite.");

            if (destroyed)
                throw new InvalidOperationException("The decomposition has been destroyed.");

            float[][] S;

            if (destroy)
            {
                S = L; destroyed = true;
            }
            else
            {
                S = new float[L.Length][];
                for (int i = 0; i < S.Length; i++)
                    S[i] = new float[L.Length];
            }

            // References:
            // http://books.google.com/books?id=myzIPBwyBbcC&pg=PA119

            // Compute the inverse S of the lower triangular matrix L
            // and store in place of the upper triangular part of S.

            for (int j = L.Length - 1; j >= 0; j--)
            {
                S[j][j] = 1 / L[j][j];
                for (int i = j - 1; i >= 0; i--)
                {
                    float sum = 0;
                    for (int k = i + 1; k <= j; k++)
                        sum += L[k][i] * S[k][j];
                    S[i][j] = -sum / L[i][i];
                }
            }

            // Compute the 2-norm squared of the rows
            // of the upper (right) triangular matrix S.
            if (robust)
            {
                for (int i = 0; i < S.Length; i++)
                {
                    float sum = 0;
                    for (int j = i; j < S[i].Length; j++)
                        sum += S[i][j] * S[i][j] / D[j];
                    result[i] = sum;
                }
            }
            else
            {
                for (int i = 0; i < S.Length; i++)
                {
                    float sum = 0;
                    for (int j = i; j < S[i].Length; j++)
                        sum += S[i][j] * S[i][j];
                    result[i] = sum;
                }
            }
        }

        /// <summary>
        ///   Computes the trace of the inverse of the decomposed matrix.
        /// </summary>
        ///
        /// <param name="destroy">True to conserve memory by reusing the
        ///    same space used to hold the decomposition, thus destroying
        ///    it in the process. Pass false otherwise.</param>
        /// 
        public float InverseTrace(bool destroy = false)
        {
            if (!robust && !positiveDefinite)
                throw new NonPositiveDefiniteMatrixException("Matrix is not positive definite.");

            if (destroyed)
                throw new InvalidOperationException("The decomposition has been destroyed.");
                
            float[][] S;

            if (destroy)
            {
                S = L; destroyed = true;
            }
            else
            {
                S = new float[L.Length][];
                for (int i = 0; i < S.Length; i++)
                    S[i] = new float[L.Length];
            }

            // References:
            // http://books.google.com/books?id=myzIPBwyBbcC&pg=PA119

            // Compute the inverse S of the lower triangular matrix L
            // and store in place of the upper triangular part of S.

            for (int j = L.Length - 1; j >= 0; j--)
            {
                S[j][j] = 1 / L[j][j];
                for (int i = j - 1; i >= 0; i--)
                {
                    float sum = 0;
                    for (int k = i + 1; k <= j; k++)
                        sum += L[k][i] * S[k][j];
                    S[i][j] = -sum / L[i][i];
                }
            }

            // Compute the 2-norm squared of the rows
            // of the upper (right) triangular matrix S.
            float trace = 0;
            
            if (robust)
            {
                for (int i = 0; i < S.Length; i++)
                    for (int j = i; j < S[i].Length; j++)
                        trace += S[i][j] * S[i][j] / D[j];
            }
            else
            {
                for (int i = 0; i < S.Length; i++)
                    for (int j = i; j < S[i].Length; j++)
                        trace += S[i][j] * S[i][j];
            }
            
            return trace;
        }

        /// <summary>
        ///   Creates a new Cholesky decomposition directly from
        ///   an already computed left triangular matrix <c>L</c>.
        /// </summary>
        /// <param name="leftTriangular">The left triangular matrix from a Cholesky decomposition.</param>
        /// 
        public static JaggedCholeskyDecompositionF FromLeftTriangularMatrix(float[][] leftTriangular)
        {
            var chol = new JaggedCholeskyDecompositionF();
            chol.dimension = leftTriangular.Length;
            chol.L = leftTriangular;
            chol.positiveDefinite = true;
            chol.robust = false;
            chol.D = new float[chol.dimension];
            for (int i = 0; i < chol.D.Length; i++)
                chol.D[i] = 1;

            return chol;
        }


        #region ICloneable Members

        private JaggedCholeskyDecompositionF()
        {
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public object Clone()
        {
            var clone = new JaggedCholeskyDecompositionF();
            clone.L = L.MemberwiseClone();
            clone.D = (float[])D.Clone();
            clone.destroyed = destroyed;
            clone.dimension = dimension;
            clone.undefined = undefined;
            clone.robust = robust;
            clone.positiveDefinite = positiveDefinite;
            return clone;
        }

        #endregion

    }
}
