// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
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
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Cholesky Decomposition of a symmetric, positive definite matrix.
    /// </summary>
    ///
    /// <remarks>
    ///   <para>
    ///     For a symmetric, positive definite matrix <c>A</c>, the Cholesky decomposition is a
    ///     lower triangular matrix <c>L</c> so that <c>A = L * L'</c>.
    ///     If the matrix is not positive definite, the constructor returns a partial 
    ///     decomposition and sets two internal variables that can be queried using the
    ///     <see cref="IsUndefined"/> and <see cref="IsPositiveDefinite"/> properties.</para>
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
    public sealed class JaggedCholeskyDecompositionD : ICloneable, ISolverArrayDecomposition<Decimal>
    {

        private Decimal[][] L;
        private Decimal[] D;
        private int n;

        private bool positiveDefinite;
        private bool undefined;
        private bool robust;
        private bool destroyed;

        // cache for lazy evaluation
        private Decimal[][] leftTriangularFactor;
        private Decimal[][] diagonalMatrix;
        private Decimal? determinant;
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
        /// <param name="valueType">
        ///   How to interpret the matrix given to be decomposed. Using this parameter, a lower or
        ///   upper-triangular matrix can be interpreted as a symmetric matrix by assuming both lower
        ///   and upper parts contain the same elements. Use this parameter in conjunction with inPlace
        ///   to save memory by storing the original matrix and its decomposition at the same memory
        ///   location (lower part will contain the decomposition's L matrix, upper part will contains 
        ///   the original matrix).</param>
        /// 
        public JaggedCholeskyDecompositionD(Decimal[][] value, bool robust = false, 
            bool inPlace = false, MatrixType valueType = MatrixType.UpperTriangular)
        {
            if (value.Rows() != value.Columns())
                throw new DimensionMismatchException("value", "Matrix is not square.");

            if (!inPlace)
                value = value.Copy();

            this.n = value.Rows();
            this.L = value.ToUpperTriangular(valueType, result: value);
            this.robust = robust;

            if (robust)
            {
                LDLt(); // Compute square-root free decomposition
            }
            else
            {
                LLt(); // Compute standard Cholesky decomposition
            }
        }

        /// <summary>
        ///   Gets whether the decomposed matrix was positive definite.
        /// </summary>
        ///
        public bool IsPositiveDefinite
        {
            get { return this.positiveDefinite && !this.undefined; }
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
        public bool IsUndefined
        {
            get { return this.undefined; }
        }

        /// <summary>
        ///   Gets the left (lower) triangular factor
        ///   <c>L</c> so that <c>A = L * D * L'</c>.
        /// </summary>
        /// 
        public Decimal[][] LeftTriangularFactor
        {
            get
            {
                if (leftTriangularFactor == null)
                {
                    if (destroyed)
                        throw new InvalidOperationException("The decomposition has been destroyed.");
                        
                    if (undefined)
                        throw new InvalidOperationException("The decomposition is undefined (zero in diagonal).");

                    leftTriangularFactor = L.GetLowerTriangle();
                }

                return leftTriangularFactor;
            }
        }

        /// <summary>
        ///   Gets the block diagonal matrix of diagonal elements in a LDLt decomposition.
        /// </summary>        
        ///   
        public Decimal[][] DiagonalMatrix
        {
            get
            {
                if (diagonalMatrix == null)
                {
                    if (destroyed)
                        throw new InvalidOperationException("The decomposition has been destroyed.");
                        
                    diagonalMatrix = Jagged.Diagonal(D);
                }

                return diagonalMatrix;
            }
        }

        /// <summary>
        ///   Gets the one-dimensional array of diagonal elements in a LDLt decomposition.
        /// </summary>        
        /// 
        public Decimal[] Diagonal
        {
            get { return D; }
        }

        /// <summary>
        ///   Gets the determinant of the decomposed matrix.
        /// </summary>
        /// 
        public Decimal Determinant
        {
            get
            {
                if (!determinant.HasValue)
                {
                    if (destroyed)
                        throw new InvalidOperationException("The decomposition has been destroyed.");

                    if (undefined)
                        throw new InvalidOperationException("The decomposition is undefined (zero in diagonal).");

                    Decimal detL = 1, detD = 1;
                    for (int i = 0; i < n; i++)
                        detL *= L[i][i];

                    if (D != null)
                    {
                        for (int i = 0; i < n; i++)
                            detD *= D[i];
                    }

                    determinant = detL * detL * detD;
                }

                return determinant.Value;
            }
        }

        /// <summary>
        ///   If the matrix is positive-definite, gets the
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
                    
                    if (undefined)
                        throw new InvalidOperationException("The decomposition is undefined (zero in diagonal).");

                    double detL = 0, detD = 0;
                    for (int i = 0; i < n; i++)
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
                    for (int i = 0; i < n && nonSingular; i++)
                        if (L[i][i] == 0 || D[i] == 0) nonSingular = false;

                    nonsingular = nonSingular;
                }

                return nonsingular.Value;
            }
        }


        private unsafe void LLt()
        {
            D = Vector.Ones<Decimal>(n);

            this.positiveDefinite = true;

            for (int j = 0; j < n; j++)
            {
                Decimal s = 0;
                for (int k = 0; k < j; k++)
                {
                    Decimal t = L[k][j];
                    for (int i = 0; i < k; i++)
                        t -= L[j][i] * L[k][i];
                    t = t / L[k][k];

                    L[j][k] = t;
                    s += t * t;
                }

                s = L[j][j] - s;

                // Use a tolerance for positive-definiteness
                this.positiveDefinite &= (s > (Decimal)1e-14 * Math.Abs(L[j][j]));

                L[j][j] = (Decimal)Math.Sqrt((double)s);
            }
        }


        private unsafe void LDLt()
        {
            D = new Decimal[n];

            this.positiveDefinite = true;

            Decimal[] v = new Decimal[n];
            for (int i = 0; i < L.Length; i++)
            {
                for (int j = 0; j < i; j++)
                    v[j] = L[i][j] * D[j];

                Decimal d = 0;
                for (int k = 0; k < i; k++)
                    d += L[i][k] * v[k];

                d = D[i] = v[i] = L[i][i] - d;

                // Use a tolerance for positive-definiteness
                this.positiveDefinite &= (v[i] > (Decimal)1e-14 * Math.Abs(L[i][i]));

                // If one of the diagonal elements is zero, the 
                // decomposition (without pivoting) is undefined.
                if (v[i] == 0) { undefined = true; return; }
                
                Parallel.For(i + 1, L.Length, k =>
                {
                     Decimal s = 0;
                     for (int j = 0; j < i; j++)
                         s += L[k][j] * v[j];

                     L[k][i] = (L[i][k] - s) / d;
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
        public Decimal[][] Solve(Decimal[][] value)
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
        public Decimal[][] Solve(Decimal[][] value, bool inPlace)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length != n)
                throw new ArgumentException("Argument matrix should have the same number of rows as the decomposed matrix.", "value");

            if (!robust && !positiveDefinite)
                throw new NonPositiveDefiniteMatrixException("Decomposed matrix is not positive definite.");

            if (destroyed)
                throw new InvalidOperationException("The decomposition has been destroyed.");

            if (undefined)
                throw new InvalidOperationException("The decomposition is undefined (zero in diagonal).");

            int count = value[0].Length;
            var B = inPlace ? value : value.MemberwiseClone();

            // Solve L*Y = B;
            for (int k = 0; k < n; k++)
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
            for (int k = n - 1; k >= 0; k--)
            {
                for (int j = 0; j < B[k].Length; j++)
                {
                    for (int i = k + 1; i < n; i++)
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
        public Decimal[] Solve(Decimal[] value)
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
        public Decimal[] Solve(Decimal[] value, bool inPlace)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length != n)
                throw new ArgumentException("Argument vector should have the same length as rows in the decomposed matrix.", "value");

            if (!robust && !positiveDefinite)
                throw new NonPositiveDefiniteMatrixException("Decomposed matrix is not positive definite.");
                
            if (destroyed)
                throw new InvalidOperationException("The decomposition has been destroyed.");

            if (undefined)
                throw new InvalidOperationException("The decomposition is undefined (zero in diagonal).");

            var B = inPlace ? value : value.Copy();

            // Solve L*Y = B;
            for (int k = 0; k < n; k++)
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
            for (int k = n - 1; k >= 0; k--)
            {
                for (int i = k + 1; i < n; i++)
                    B[k] -= B[i] * L[i][k];
                B[k] /= L[k][k];
            }
            
            return B;
        }

		/// <summary>
        ///   Solves a set of equation systems of type <c>A * X = B</c> where B is a diagonal matrix.
        /// </summary>
        /// <param name="diagonal">Diagonal fo the right hand side matrix with as many rows as <c>A</c>.</param>
        /// <returns>Matrix <c>X</c> so that <c>L * U * X = B</c>.</returns>
        /// 
        public Decimal[][] SolveForDiagonal(Decimal[] diagonal)
        {
            if (diagonal == null)
                throw new ArgumentNullException("diagonal");

            return Solve(Jagged.Diagonal(diagonal));
        }

        /// <summary>
        ///   Solves a set of equation systems of type <c>A * X = I</c>.
        /// </summary>
        /// 
        public Decimal[][] Inverse()
        {
            return Solve(Jagged.Identity<Decimal>(n));
        }

        /// <summary>
        ///   Computes the diagonal of the inverse of the decomposed matrix.
        /// </summary>
        /// 
        public Decimal[] InverseDiagonal(bool destroy = false)
        {
            return InverseDiagonal(new Decimal[n], destroy);
        }
        
        /// <summary>
        ///   Computes the diagonal of the inverse of the decomposed matrix.
        /// </summary>
        ///
        /// <param name="destroy">True to conserve memory by reusing the
        ///    same space used to hold the decomposition, thus destroying
        ///    it in the process. Pass false otherwise.</param>
        /// <param name="result">The array to hold the result of the 
        ///    computation. Should be of same length as the the diagonal
        ///    of the original matrix.</param>
        /// 
        public Decimal[] InverseDiagonal(Decimal[] result, bool destroy = false)
        {
            if (!robust && !positiveDefinite)
                throw new NonPositiveDefiniteMatrixException("Matrix is not positive definite.");

            if (destroyed)
                throw new InvalidOperationException("The decomposition has been destroyed.");

            if (undefined)
                throw new InvalidOperationException("The decomposition is undefined (zero in diagonal).");

            Decimal[][] S;

            if (destroy)
            {
                S = L; destroyed = true;
            }
            else
            {
                S = Jagged.Zeros<Decimal>(n, n);
            }

            // References:
            // http://books.google.com/books?id=myzIPBwyBbcC&pg=PA119

            // Compute the inverse S of the lower triangular matrix L
            // and store in place of the upper triangular part of S.

            for (int j = n - 1; j >= 0; j--)
            {
                S[j][j] = 1 / L[j][j];
                for (int i = j - 1; i >= 0; i--)
                {
                    Decimal sum = 0;
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
                    Decimal sum = 0;
                    for (int j = i; j < S[i].Length; j++)
                        sum += S[i][j] * S[i][j] / D[j];
                    result[i] = sum;
                }
            }
            else
            {
                for (int i = 0; i < S.Length; i++)
                {
                    Decimal sum = 0;
                    for (int j = i; j < S[i].Length; j++)
                        sum += S[i][j] * S[i][j];
                    result[i] = sum;
                }
            }

            return result;
        }

        /// <summary>
        ///   Computes the trace of the inverse of the decomposed matrix.
        /// </summary>
        ///
        /// <param name="destroy">True to conserve memory by reusing the
        ///    same space used to hold the decomposition, thus destroying
        ///    it in the process. Pass false otherwise.</param>
        /// 
        public Decimal InverseTrace(bool destroy = false)
        {
            if (!robust && !positiveDefinite)
                throw new NonPositiveDefiniteMatrixException("Matrix is not positive definite.");

            if (destroyed)
                throw new InvalidOperationException("The decomposition has been destroyed.");
                
            if (undefined)
                throw new InvalidOperationException("The decomposition is undefined (zero in diagonal).");

            Decimal[][] S;

            if (destroy)
            {
                S = L; destroyed = true;
            }
            else
            {
                S = Jagged.Zeros<Decimal>(n, n);
            }

            // References:
            // http://books.google.com/books?id=myzIPBwyBbcC&pg=PA119

            // Compute the inverse S of the lower triangular matrix L
            // and store in place of the upper triangular part of S.

            for (int j = n - 1; j >= 0; j--)
            {
                S[j][j] = 1 / L[j][j];
                for (int i = j - 1; i >= 0; i--)
                {
                    Decimal sum = 0;
                    for (int k = i + 1; k <= j; k++)
                        sum += L[k][i] * S[k][j];
                    S[i][j] = -sum / L[i][i];
                }
            }

            // Compute the 2-norm squared of the rows
            // of the upper (right) triangular matrix S.
            Decimal trace = 0;
            
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
        ///   Reverses the decomposition, reconstructing the original matrix <c>X</c>.
        /// </summary>
        /// 
        public Decimal[][] Reverse()
        {
            if (destroyed)
                throw new InvalidOperationException("The decomposition has been destroyed.");
                
            if (undefined)
                throw new InvalidOperationException("The decomposition is undefined (zero in diagonal).");

            if (robust)
                return LeftTriangularFactor.Dot(DiagonalMatrix).DotWithTransposed(LeftTriangularFactor);
            return LeftTriangularFactor.DotWithTransposed(LeftTriangularFactor);
        }

        /// <summary>
        ///   Computes <c>(Xt * X)^1</c> (the inverse of the covariance matrix). This
        ///   matrix can be used to determine standard errors for the coefficients when
        ///   solving a linear set of equations through any of the <see cref="Solve(Decimal[][])"/>
        ///   methods.
        /// </summary>
        /// 
        public Decimal[][] GetInformationMatrix()
        {
            var X = Reverse();
            return X.TransposeAndDot(X).Inverse();
        }

        /// <summary>
        ///   Creates a new Cholesky decomposition directly from
        ///   an already computed left triangular matrix <c>L</c>.
        /// </summary>
        /// <param name="leftTriangular">The left triangular matrix from a Cholesky decomposition.</param>
        /// 
        public static JaggedCholeskyDecompositionD FromLeftTriangularMatrix(Decimal[][] leftTriangular)
        {
            var chol = new JaggedCholeskyDecompositionD();
            chol.n = leftTriangular.Length;
            chol.L = leftTriangular;
            chol.positiveDefinite = true;
            chol.robust = false;
            chol.D = Vector.Ones<Decimal>(chol.n);
            return chol;
        }


        #region ICloneable Members

        private JaggedCholeskyDecompositionD()
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
            var clone = new JaggedCholeskyDecompositionD();
            clone.L = L.MemberwiseClone();
            clone.D = (Decimal[])D.Clone();
            clone.destroyed = destroyed;
            clone.n = n;
            clone.undefined = undefined;
            clone.robust = robust;
            clone.positiveDefinite = positiveDefinite;
            return clone;
        }

        #endregion

    }
}

