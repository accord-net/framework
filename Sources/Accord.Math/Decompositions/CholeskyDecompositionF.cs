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

    /// <summary>
    ///     Cholesky Decomposition of a symmetric, positive definite matrix.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     For a symmetric, positive definite matrix <c>A</c>, the Cholesky decomposition is a
    ///     lower triangular matrix <c>L</c> so that <c>A = L * L'</c>.
    ///     If the matrix is not symmetric or positive definite, the constructor returns a partial 
    ///     decomposition and sets two internal variables that can be queried using the
    ///     <see cref="Symmetric"/> and <see cref="PositiveDefinite"/> properties.</para>
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
    public sealed class CholeskyDecompositionF : ICloneable, ISolverMatrixDecomposition<float>
    {

        private float[,] L;
        private float[] D;
        private int n;

        private bool symmetric;
        private bool positiveDefinite;
        private bool robust;

        // cache for lazy evaluation
        private float[,] diagonalMatrix;
        private float? determinant;
        private double? lndeterminant;
        private bool? nonsingular;

        /// <summary>Constructs a new Cholesky Decomposition.</summary>
        /// <param name="value">The matrix to be decomposed.</param>
        public CholeskyDecompositionF(float[,] value)
            : this(value, false, false)
        {
        }

        /// <summary>Constructs a new Cholesky Decomposition.</summary>
        /// 
        /// <param name="value">The matrix to be decomposed.</param>
        /// <param name="robust">True to perform a square root free LDLt decomposition,
        /// false otherwise.</param>
        /// 
        public CholeskyDecompositionF(float[,] value, bool robust)
            : this(value, robust, false)
        {
        }

        /// <summary>Constructs a new Cholesky Decomposition.</summary>
        /// 
        /// <param name="value">The matrix to be decomposed.</param>
        /// <param name="robust">True to perform a square-root free LDLt decomposition,
        /// false otherwise.</param>
        /// <param name="lowerTriangular">True to assume the <paramref name="value">value
        /// matrix</paramref> is a lower triangular symmetric matrix, false otherwise.</param>
        /// 
        public CholeskyDecompositionF(float[,] value, bool robust, bool lowerTriangular)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "Matrix cannot be null.");
            }

            if (value.GetLength(0) != value.GetLength(1))
            {
                throw new DimensionMismatchException("value", "Matrix is not square.");
            }

            if (robust)
            {
                LDLt(value); // Compute square-root free decomposition
            }
            else
            {
                LLt(value); // Compute standard Cholesky decomposition
            }

            if (lowerTriangular)
                symmetric = true;
        }


        /// <summary>
        ///   Returns <see langword="true"/> if the matrix is symmetric.
        /// </summary>
        ///
        public bool Symmetric
        {
            get { return this.symmetric; }
        }

        /// <summary>
        ///   Returns <see langword="true"/> if the matrix is positive definite.
        /// </summary>
        ///
        public bool PositiveDefinite
        {
            get { return this.positiveDefinite; }
        }

        /// <summary>
        ///   Returns the left (lower) triangular factor <c>L</c> so that <c>A = L * D * L'</c>.
        /// </summary>
        ///
        public float[,] LeftTriangularFactor
        {
            get { return this.L; }
        }

        /// <summary>
        ///   Returns the block diagonal matrix of diagonal elements in a LDLt decomposition.
        /// </summary>        
        ///
        public float[,] DiagonalMatrix
        {
            get 
            {
                if (diagonalMatrix == null)
                {
                    diagonalMatrix = new float[n, n];
                    for (int i = 0; i < D.Length; i++)
                        diagonalMatrix[i,i] = D[i];
                }
                return diagonalMatrix; 
            }
        }

        /// <summary>
        ///   Returns the one-dimensional array of diagonal elements in a LDLt decomposition.
        /// </summary>        
        ///
        public float[] Diagonal
        {
            get { return D; }
        }

        /// <summary>
        ///   Returns the determinant of the matrix.
        /// </summary>
        ///
        public float Determinant
        {
            get
            {
                if (!determinant.HasValue)
                {
                    if (!this.symmetric)
                        throw new NonSymmetricMatrixException("Matrix is not symmetric.");

                    float detL = 1;
                    for (int i = 0; i < n; i++)
                        detL *= L[i, i];

                    float detD = 1;
                    for (int i = 0; i < D.Length; i++)
                        detD *= D[i];

                    determinant = detL * detL * detD;
                }

                return determinant.Value;
            }
        }

        /// <summary>
        ///   Returns the log-determinant of the matrix.
        /// </summary>
        ///
        public double LogDeterminant
        {
            get
            {
                if (!lndeterminant.HasValue)
                {
                    if (!this.symmetric)
                        throw new NonSymmetricMatrixException("Matrix is not symmetric.");

                    double detL = 0;
                    for (int i = 0; i < n; i++)
                        detL += Math.Log((double)L[i, i]);

                    double detD = 0;
                    for (int i = 0; i < D.Length; i++)
                        detD += Math.Log((double)D[i]);

                    lndeterminant = detL + detL + detD;
                }

                return lndeterminant.Value;
            }
        }

        /// <summary>
        ///   Returns if the matrix is non-singular (i.e. invertible).
        /// </summary>
        ///
        public bool Nonsingular
        {
            get
            {
                if (!nonsingular.HasValue)
                {
                    if (!symmetric)
                        throw new NonSymmetricMatrixException("Matrix is not symmetric.");

                    bool nonSingular = true;
                    for (int i = 0; i < D.Length && nonSingular; i++)
                        if (L[i, i] == 0 || D[i] == 0) nonSingular = false;

                    nonsingular = nonSingular;
                }

                return nonsingular.Value;
            }
        }


        private unsafe void LLt(float[,] value)
        {
            n = value.GetLength(0);
            L = new float[n, n];
            D = new float[n];

            for (int i = 0; i < D.Length; i++)
                D[i] = 1;

            robust = false;

            float[,] a = value;

            this.positiveDefinite = true;
            this.symmetric = true;

            fixed (float* ptrL = L)
            {
                for (int j = 0; j < n; j++)
                {
                    float* Lrowj = ptrL + j * n;
                    float d = 0;
                    for (int k = 0; k < j; k++)
                    {
                        float* Lrowk = ptrL + k * n;

                        float s = 0;
                        for (int i = 0; i < k; i++)
                            s += Lrowk[i] * Lrowj[i];

                        Lrowj[k] = s = (a[j, k] - s) / Lrowk[k];
                        d += s * s;

                        this.symmetric = this.symmetric & (a[k, j] == a[j, k]);
                    }

                    d = a[j, j] - d;

                    // Use a tolerance for positive-definiteness
                    this.positiveDefinite &= (d > (float)1e-14 * Math.Abs(a[j, j]));

                    Lrowj[j] = (float)System.Math.Sqrt((double)System.Math.Max(d, 0));

                    for (int k = j + 1; k < n; k++)
                        Lrowj[k] = 0;
                }
            }
        }

        private unsafe void LDLt(float[,] value)
        {
            n = value.GetLength(0);
            L = new float[n, n];
            D = new float[n];
            robust = true;

            float[,] a = value;

            float[] v = new float[n];
            this.positiveDefinite = true;
            this.symmetric = true;

            float d = D[0] = v[0] = a[0, 0];

            if (d == 0) 
                this.positiveDefinite = false;

            for (int j = 1; j < n; j++)
                L[j, 0] = a[j, 0] / d;

            for (int j = 1; j < n; j++)
            {
                d = 0;
                for (int k = 0; k < j; k++)
                {
                    v[k] = L[j, k] * D[k];
                    d += L[j, k] * v[k];
                }

                d = D[j] = v[j] = a[j, j] - d;

                // Use a tolerance for positive-definiteness
                this.positiveDefinite &= (d > (float)1e-14 * Math.Abs(a[j, j]));

                for (int k = j + 1; k < n; k++)
                {
                    float s = 0;
                    for (int i = 0; i < j; i++)
                        s += L[k, i] * v[i];

                    L[k, j] = (a[k, j] - s) / d;

                    this.symmetric = this.symmetric & (a[k, j] == a[j, k]);
                }
            }

            for (int i = 0; i < n; i++)
                L[i, i] += 1;
        }


        /// <summary>Solves a set of equation systems of type <c>A * X = B</c>.</summary>
        /// <param name="value">Right hand side matrix with as many rows as <c>A</c> and any number of columns.</param>
        /// <returns>Matrix <c>X</c> so that <c>L * L' * X = B</c>.</returns>
        /// <exception cref="T:System.ArgumentException">Matrix dimensions do not match.</exception>
        /// <exception cref="T:System.InvalidOperationException">Matrix is not symmetric and positive definite.</exception>
        /// 
        public float[,] Solve(float[,] value)
        {
            return Solve(value, false);
        }

        /// <summary>Solves a set of equation systems of type <c>A * X = B</c>.</summary>
        /// <param name="value">Right hand side matrix with as many rows as <c>A</c> and any number of columns.</param>
        /// <returns>Matrix <c>X</c> so that <c>L * L' * X = B</c>.</returns>
        /// <exception cref="T:System.ArgumentException">Matrix dimensions do not match.</exception>
        /// <exception cref="T:System.NonSymmetricMatrixException">Matrix is not symmetric.</exception>
        /// <exception cref="T:System.NonPositiveDefiniteMatrixException">Matrix is not positive-definite.</exception>
        /// <param name="inPlace">True to compute the solving in place, false otherwise.</param>
        /// 
        public float[,] Solve(float[,] value, bool inPlace)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.GetLength(0) != n)
                throw new ArgumentException("Argument matrix should have the same number of rows as the decomposed matrix.", "value");

            if (!symmetric)
                throw new NonSymmetricMatrixException("Decomposed matrix is not symmetric.");

            if (!robust && !positiveDefinite)
                throw new NonPositiveDefiniteMatrixException("Decomposed matrix is not positive definite.");


            int count = value.GetLength(1);
            float[,] B = inPlace ? value : (float[,])value.Clone();


            // Solve L*Y = B;
            for (int k = 0; k < n; k++)
            {
                for (int j = 0; j < count; j++)
                {
                    for (int i = 0; i < k; i++)
                        B[k, j] -= B[i, j] * L[k, i];

                    B[k, j] /= L[k, k];
                }
            }

            if (robust)
            {
                for (int k = 0; k < n; k++)
                    for (int j = 0; j < count; j++)
                        B[k, j] /= D[k];
            }

            // Solve L'*X = Y;
            for (int k = n - 1; k >= 0; k--)
            {
                for (int j = 0; j < count; j++)
                {
                    for (int i = k + 1; i < n; i++)
                        B[k, j] -= B[i, j] * L[i, k];

                    B[k, j] /= L[k, k];
                }
            }

            return B;
        }

        /// <summary>Solves a set of equation systems of type <c>A * X = B</c>.</summary>
        /// <param name="value">Right hand side matrix with as many rows as <c>A</c> and any number of columns.</param>
        /// <returns>Matrix <c>X</c> so that <c>L * L' * X = B</c>.</returns>
        /// <exception cref="T:System.ArgumentException">Matrix dimensions do not match.</exception>
        /// <exception cref="T:System.NonSymmetricMatrixException">Matrix is not symmetric.</exception>
        /// <exception cref="T:System.NonPositiveDefiniteMatrixException">Matrix is not positive-definite.</exception>
        /// 
        public float[] Solve(float[] value)
        {
            return Solve(value, false);
        }

        /// <summary>Solves a set of equation systems of type <c>A * x = b</c>.</summary>
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

            if (value.Length != n)
                throw new ArgumentException("Argument vector should have the same length as rows in the decomposed matrix.", "value");

            if (!symmetric)
                throw new NonSymmetricMatrixException("Decomposed matrix is not symmetric.");

            if (!robust && !positiveDefinite)
                throw new NonPositiveDefiniteMatrixException("Decomposed matrix is not positive definite.");


            float[] B = inPlace ? value : (float[])value.Clone();


            // Solve L*Y = B;
            for (int k = 0; k < n; k++)
            {
                for (int i = 0; i < k; i++)
                    B[k] -= B[i] * L[k, i];

                B[k] /= L[k, k];
            }

            if (robust)
            {
                for (int k = 0; k < n; k++)
                    B[k] /= D[k];
            }

            // Solve L'*X = Y;
            for (int k = n - 1; k >= 0; k--)
            {
                for (int i = k + 1; i < n; i++)
                    B[k] -= B[i] * L[i, k];
                B[k] /= L[k, k];
            }

            return B;
        }

        /// <summary>
        ///   Computes the inverse of the decomposed matrix.
        /// </summary>
        /// 
        public float[,] Inverse()
        {
            if (!symmetric)
                throw new NonSymmetricMatrixException("Matrix is not symmetric.");

            if (!robust && !positiveDefinite)
                throw new NonPositiveDefiniteMatrixException("Matrix is not positive definite.");


            float[,] B = new float[n, n];
            for (int i = 0; i < n; i++)
                B[i, i] = 1;

            // Solve L*Y = B;
            for (int k = 0; k < n; k++)
            {
                for (int j = 0; j < k; j++)
                {
                    for (int i = 0; i < k; i++)
                        B[k, j] -= B[i, j] * L[k, i];

                    B[k, j] /= L[k, k];
                }
            }

            if (robust)
            {
                for (int k = 0; k < n; k++)
                    for (int j = 0; j <= k; j++)
                        B[k, j] /= D[k];
            }

            // Solve L'*X = Y;
            for (int k = n - 1; k >= 0; k--)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int i = k + 1; i < n; i++)
                        B[k, j] -= B[i, j] * L[i, k];

                    B[k, j] /= L[k, k];
                }
            }

            return B;
        }


        /// <summary>
        ///   Creates a new Cholesky decomposition directly from
        ///   an already computed left triangular matrix <c>L</c>.
        /// </summary>
        /// <param name="leftTriangular">The left triangular matrix from a Cholesky decomposition.</param>
        /// 
        public static CholeskyDecompositionF FromLeftTriangularMatrix(float[,] leftTriangular)
        {
            var chol = new CholeskyDecompositionF();
            chol.n = leftTriangular.GetLength(0);
            chol.L = leftTriangular;
            chol.symmetric = true;
            chol.positiveDefinite = true;
            chol.robust = false;
            chol.D = new float[chol.n];
            for (int i = 0; i < chol.D.Length; i++)
                chol.D[i] = 1;

            return chol;
        }


        #region ICloneable Members

        private CholeskyDecompositionF()
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
            var clone = new CholeskyDecompositionF();
            clone.L = (float[,])L.Clone();
            clone.D = (float[])D.Clone();
            clone.n = n;
            clone.robust = robust;
            clone.positiveDefinite = positiveDefinite;
            clone.symmetric = symmetric;
            return clone;
        }

        #endregion

    }
}
