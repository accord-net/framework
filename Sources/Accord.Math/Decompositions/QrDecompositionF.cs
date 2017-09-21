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
    using Accord.Math;
	using Accord.Compat;

    /// <summary>
    ///   QR decomposition for a rectangular matrix.
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    ///   For an m-by-n matrix <c>A</c> with <c>m >= n</c>, the QR decomposition
    ///   is an m-by-n orthogonal matrix <c>Q</c> and an n-by-n upper triangular
    ///   matrix <c>R</c> so that <c>A = Q * R</c>.</para>
    /// <para>
    ///   The QR decomposition always exists, even if the matrix does not have
    ///   full rank, so the constructor will never fail. The primary use of the
    ///   QR decomposition is in the least squares solution of nonsquare systems
    ///   of simultaneous linear equations.
    ///   This will fail if <see cref="FullRank"/> returns <see langword="false"/>.</para>  
    /// </remarks>
    /// 
    public sealed class QrDecompositionF : ICloneable, ISolverMatrixDecomposition<Single>
    {
        private int n; 
        private int m;
        private int p;
        private bool economy;

        private Single[,] qr;
        private Single[] Rdiag;

		// cache for lazy evaluation
        private bool? fullRank;
        private Single[,] orthogonalFactor;
        private Single[,] upperTriangularFactor;

        /// <summary>Constructs a QR decomposition.</summary>    
        /// <param name="value">The matrix A to be decomposed.</param>
        /// <param name="transpose">True if the decomposition should be performed on
        /// the transpose of A rather than A itself, false otherwise. Default is false.</param>
        /// <param name="inPlace">True if the decomposition should be done in place,
        /// overriding the given matrix <paramref name="value"/>. Default is false.</param>
        /// <param name="economy">True to perform the economy decomposition, where only
        ///.the information needed to solve linear systems is computed. If set to false,
        /// the full QR decomposition will be computed.</param>
        public QrDecompositionF(Single[,] value, bool transpose = false, 
            bool economy = true, bool inPlace = false)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "Matrix cannot be null.");
            }

            if ((!transpose && value.Rows() < value.Columns()) ||
                 (transpose && value.Columns() < value.Rows()))
            {
                throw new ArgumentException("Matrix has more columns than rows.", "value");
            }

            // https://www.inf.ethz.ch/personal/gander/papers/qrneu.pdf

            if (transpose)
            {
                this.p = value.Rows();

                if (economy)
                {
                    // Compute the faster, economy-sized QR decomposition 
                    this.qr = value.Transpose(inPlace: inPlace);
                }
                else
                {
                    // Create room to store the full decomposition
                    this.qr = Matrix.Create(value.Columns(), value.Columns(), value, transpose: true);
                }
            }
            else
            {
                this.p = value.Columns();

                if (economy)
                {
                    // Compute the faster, economy-sized QR decomposition 
                    this.qr = inPlace ? value : value.Copy();
                }
                else
                {
                    // Create room to store the full decomposition
                    this.qr = Matrix.Create(value.Rows(), value.Rows(), value, transpose: false);
                }
            }

            this.economy = economy;
            this.n = qr.Rows();
            this.m = qr.Columns();
            this.Rdiag = new Single[m];

            for (int k = 0; k < m; k++)
            {
                // Compute 2-norm of k-th column without under/overflow.
                Single nrm = 0;
                for (int i = k; i < n; i++)
                    nrm = Tools.Hypotenuse(nrm, qr[i, k]);

                if (nrm != 0)
                {
                    // Form k-th Householder vector.
                    if (qr[k, k] < 0)
                        nrm = -nrm;

                    for (int i = k; i < n; i++)
                        qr[i, k] /= nrm;

                    qr[k, k] += 1;

                    // Apply transformation to remaining columns.
                    for (int j = k + 1; j < m; j++)
                    {
                        Single s = 0;
                        for (int i = k; i < n; i++)
                            s += qr[i, k] * qr[i, j];

                        s = -s / qr[k, k];
                        for (int i = k; i < n; i++)
                            qr[i, j] += s * qr[i, k];
                    }
                }

                this.Rdiag[k] = -nrm;
            }
        }

        /// <summary>Least squares solution of <c>A * X = B</c></summary>
        /// <param name="value">Right-hand-side matrix with as many rows as <c>A</c> and any number of columns.</param>
        /// <returns>A matrix that minimized the two norm of <c>Q * R * X - B</c>.</returns>
        /// <exception cref="T:System.ArgumentException">Matrix row dimensions must be the same.</exception>
        /// <exception cref="T:System.InvalidOperationException">Matrix is rank deficient.</exception>
        public Single[,] Solve(Single[,] value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Matrix cannot be null.");

            if (value.Rows() != n)
                throw new ArgumentException("Matrix row dimensions must agree.");

            if (!this.FullRank)
                throw new InvalidOperationException("Matrix is rank deficient.");

            // Copy right hand side
            int count = value.Columns();
            var X = value.Copy();

            // Compute Y = transpose(Q)*B
            for (int k = 0; k < p; k++)
            {
                for (int j = 0; j < count; j++)
                {
                    Single s = 0;
                    for (int i = k; i < n; i++)
                        s += qr[i, k] * X[i, j];

                    s = -s / qr[k, k];
                    for (int i = k; i < n; i++)
                        X[i, j] += s * qr[i, k];
                }
            }

            // Solve R*X = Y;
            for (int k = p - 1; k >= 0; k--)
            {
                for (int j = 0; j < count; j++)
                    X[k, j] /= Rdiag[k];

                for (int i = 0; i < k; i++)
                    for (int j = 0; j < count; j++)
                        X[i, j] -= X[k, j] * qr[i, k];
            }

            return Matrix.Create(p, count, X, transpose: false);
        }

        /// <summary>Least squares solution of <c>X * A = B</c></summary>
        /// <param name="value">Right-hand-side matrix with as many columns as <c>A</c> and any number of rows.</param>
        /// <returns>A matrix that minimized the two norm of <c>X * Q * R - B</c>.</returns>
        /// <exception cref="T:System.ArgumentException">Matrix column dimensions must be the same.</exception>
        /// <exception cref="T:System.InvalidOperationException">Matrix is rank deficient.</exception>
        public Single[,] SolveTranspose(Single[,] value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Matrix cannot be null.");

            if (value.Columns() != qr.Rows())
                throw new ArgumentException("Matrix row dimensions must agree.");

            if (!this.FullRank)
                throw new InvalidOperationException("Matrix is rank deficient.");

            // Copy right hand side
            int count = value.Rows();
            var X = value.Transpose();

            // Compute Y = transpose(Q)*B
            for (int k = 0; k < p; k++)
            {
                for (int j = 0; j < count; j++)
                {
                    Single s = 0;
                    for (int i = k; i < n; i++)
                        s += qr[i, k] * X[i, j];

                    s = -s / qr[k, k];
                    for (int i = k; i < n; i++)
                        X[i, j] += s * qr[i, k];
                }
            }

            // Solve R*X = Y;
            for (int k = m - 1; k >= 0; k--)
            {
                for (int j = 0; j < count; j++)
                    X[k, j] /= Rdiag[k];

                for (int i = 0; i < k; i++)
                    for (int j = 0; j < count; j++)
                        X[i, j] -= X[k, j] * qr[i, k];
            }

            return Matrix.Create(count, p, X, transpose: true);
        }

        /// <summary>Least squares solution of <c>A * X = B</c></summary>
        /// <param name="value">Right-hand-side matrix with as many rows as <c>A</c> and any number of columns.</param>
        /// <returns>A matrix that minimized the two norm of <c>Q * R * X - B</c>.</returns>
        /// <exception cref="T:System.ArgumentException">Matrix row dimensions must be the same.</exception>
        /// <exception cref="T:System.InvalidOperationException">Matrix is rank deficient.</exception>
        public Single[] Solve(Single[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length != qr.Rows())
                throw new ArgumentException("Matrix row dimensions must agree.");

            if (!this.FullRank)
                throw new InvalidOperationException("Matrix is rank deficient.");

            // Copy right hand side
            var X = value.Copy();

            // Compute Y = transpose(Q)*B
            for (int k = 0; k < p; k++)
            {
                Single s = 0;
                for (int i = k; i < n; i++)
                    s += qr[i, k] * X[i];

                s = -s / qr[k, k];
                for (int i = k; i < n; i++)
                    X[i] += s * qr[i, k];
            }

            // Solve R*X = Y;
            for (int k = p - 1; k >= 0; k--)
            {
                X[k] /= Rdiag[k];

                for (int i = 0; i < k; i++)
                    X[i] -= X[k] * qr[i, k];
            }

            return X.First(p);
        }

        /// <summary>Shows if the matrix <c>A</c> is of full rank.</summary>
        /// <value>The value is <see langword="true"/> if <c>R</c>, and hence <c>A</c>, has full rank.</value>
        public bool FullRank
        {
            get
            {
				if (this.fullRank.HasValue)
					return this.fullRank.Value;

                for (int i = 0; i < p; i++)
                    if (this.Rdiag[i] == 0)
						return (bool)(this.fullRank = false);
                
				return (bool)(this.fullRank = true);
            }
        }

        /// <summary>Returns the upper triangular factor <c>R</c>.</summary>
        public Single[,] UpperTriangularFactor
        {
            get
            {
				if (this.upperTriangularFactor != null)
					return this.upperTriangularFactor;

                int rows = economy ? m : n;
                var x = Matrix.Zeros<Single>(rows, p);

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < p; j++)
                    {
                        if (i < j)
                            x[i, j] = qr[i, j];
                        else if (i == j)
                            x[i, j] = Rdiag[i];
                    }
                }

                return this.upperTriangularFactor = x;
            }
        }

        /// <summary>
        ///   Returns the (economy-size) orthogonal factor <c>Q</c>.
        /// </summary>
        public Single[,] OrthogonalFactor
        {
            get
            {
				if (this.orthogonalFactor != null)
					return this.orthogonalFactor;

                int cols = economy ? m : n;
                var x = Matrix.Zeros<Single>(n, cols);

                for (int k = cols - 1; k >= 0; k--)
                {
                    for (int i = 0; i < n; i++)
                        x[i, k] = 0;

                    x[k, k] = 1;
                    for (int j = k; j < cols; j++)
                    {
                        if (qr[k, k] != 0)
                        {
                            Single s = 0;
                            for (int i = k; i < n; i++)
                                s += qr[i, k] * x[i, j];

                            s = -s / qr[k, k];
                            for (int i = k; i < n; i++)
                                x[i, j] += s * qr[i, k];
                        }
                    }
                }

                return this.orthogonalFactor = x;
            }
        }

        /// <summary>Returns the diagonal of <c>R</c>.</summary>
        public Single[] Diagonal
        {
            get { return Rdiag; }
        }

        /// <summary>Least squares solution of <c>A * X = I</c></summary>
        public Single[,] Inverse()
        {
            if (!this.FullRank)
                throw new InvalidOperationException("Matrix is rank deficient.");

            return Solve(Matrix.Diagonal(n, n, (Single)1));
        }

        /// <summary>
        ///   Reverses the decomposition, reconstructing the original matrix <c>X</c>.
        /// </summary>
        /// 
        public Single[,] Reverse()
        {
            return OrthogonalFactor.Dot(UpperTriangularFactor);
        }

        /// <summary>
        ///   Computes <c>(Xt * X)^1</c> (the inverse of the covariance matrix). This
        ///   matrix can be used to determine standard errors for the coefficients when
        ///   solving a linear set of equations through any of the <see cref="Solve(Single[,])"/>
        ///   methods.
        /// </summary>
        /// 
        public Single[,] GetInformationMatrix()
        {
            var X = Reverse();
            return X.TransposeAndDot(X).Inverse();
        }

        #region ICloneable Members

        private QrDecompositionF()
        {
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            var clone = new QrDecompositionF();
            clone.qr = qr.Copy();
            clone.Rdiag = Rdiag.Copy();
            clone.n = n;
            clone.p = p;
            clone.m = m;
            clone.economy = economy;
            return clone;
        }

        #endregion

    }
}

