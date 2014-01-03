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
    using Accord.Math;

    /// <summary>
    ///      QR decomposition for a rectangular matrix.
    /// </summary>
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
    public sealed class QrDecomposition : ICloneable, ISolverMatrixDecomposition<double>
    {
        private double[,] qr;
        private double[] Rdiag;

        /// <summary>Constructs a QR decomposition.</summary>    
        /// <param name="value">The matrix A to be decomposed.</param>
        public QrDecomposition(double[,] value)
            : this(value, false)
        {
        }

        /// <summary>Constructs a QR decomposition.</summary>    
        /// <param name="value">The matrix A to be decomposed.</param>
        /// <param name="transpose">True if the decomposition should be performed on
        /// the transpose of A rather than A itself, false otherwise. Default is false.</param>
        public QrDecomposition(double[,] value, bool transpose)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "Matrix cannot be null.");
            }

            if ((!transpose && value.GetLength(0) < value.GetLength(1)) ||
                 (transpose && value.GetLength(1) < value.GetLength(0)))
            {
                throw new ArgumentException("Matrix has more columns than rows.", "value");
            }

            this.qr = transpose ? value.Transpose() : (double[,])value.Clone();

            int rows = qr.GetLength(0);
            int cols = qr.GetLength(1);
            this.Rdiag = new double[cols];

            for (int k = 0; k < cols; k++)
            {
                // Compute 2-norm of k-th column without under/overflow.
                double nrm = 0;
                for (int i = k; i < rows; i++)
                    nrm = Tools.Hypotenuse(nrm, qr[i, k]);

                if (nrm != 0)
                {
                    // Form k-th Householder vector.
                    if (qr[k, k] < 0)
                        nrm = -nrm;

                    for (int i = k; i < rows; i++)
                        qr[i, k] /= nrm;

                    qr[k, k] += 1;

                    // Apply transformation to remaining columns.
                    for (int j = k + 1; j < cols; j++)
                    {
                        double s = 0;

                        for (int i = k; i < rows; i++)
                            s += qr[i, k] * qr[i, j];

                        s = -s / qr[k, k];

                        for (int i = k; i < rows; i++)
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
        public double[,] Solve(double[,] value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Matrix cannot be null.");

            if (value.GetLength(0) != qr.GetLength(0))
                throw new ArgumentException("Matrix row dimensions must agree.");

            if (!this.FullRank)
                throw new InvalidOperationException("Matrix is rank deficient.");

            // Copy right hand side
            int count = value.GetLength(1);
            var X = (double[,])value.Clone();
            int m = qr.GetLength(0);
            int n = qr.GetLength(1);

            // Compute Y = transpose(Q)*B
            for (int k = 0; k < n; k++)
            {
                for (int j = 0; j < count; j++)
                {
                    double s = 0;

                    for (int i = k; i < m; i++)
                        s += qr[i, k] * X[i, j];

                    s = -s / qr[k, k];

                    for (int i = k; i < m; i++)
                        X[i, j] += s * qr[i, k];
                }
            }

            // Solve R*X = Y;
            for (int k = n - 1; k >= 0; k--)
            {
                for (int j = 0; j < count; j++)
                    X[k, j] /= Rdiag[k];

                for (int i = 0; i < k; i++)
                    for (int j = 0; j < count; j++)
                        X[i, j] -= X[k, j] * qr[i, k];
            }

            var r = new double[n, count];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < count; j++)
                    r[i, j] = X[i, j];

            return r;
        }

        /// <summary>Least squares solution of <c>X * A = B</c></summary>
        /// <param name="value">Right-hand-side matrix with as many columns as <c>A</c> and any number of rows.</param>
        /// <returns>A matrix that minimized the two norm of <c>X * Q * R - B</c>.</returns>
        /// <exception cref="T:System.ArgumentException">Matrix column dimensions must be the same.</exception>
        /// <exception cref="T:System.InvalidOperationException">Matrix is rank deficient.</exception>
        public double[,] SolveTranspose(double[,] value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Matrix cannot be null.");

            if (value.GetLength(1) != qr.GetLength(0))
                throw new ArgumentException("Matrix row dimensions must agree.");

            if (!this.FullRank)
                throw new InvalidOperationException("Matrix is rank deficient.");

            // Copy right hand side
            int count = value.GetLength(0);
            var X = value.Transpose();
            int m = qr.GetLength(0);
            int n = qr.GetLength(1);

            // Compute Y = transpose(Q)*B
            for (int k = 0; k < n; k++)
            {
                for (int j = 0; j < count; j++)
                {
                    double s = 0;

                    for (int i = k; i < m; i++)
                        s += qr[i, k] * X[i, j];

                    s = -s / qr[k, k];

                    for (int i = k; i < m; i++)
                        X[i, j] += s * qr[i, k];
                }
            }

            // Solve R*X = Y;
            for (int k = n - 1; k >= 0; k--)
            {
                for (int j = 0; j < count; j++)
                    X[k, j] /= Rdiag[k];

                for (int i = 0; i < k; i++)
                    for (int j = 0; j < count; j++)
                        X[i, j] -= X[k, j] * qr[i, k];
            }

            var r = new double[count, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < count; j++)
                    r[j, i] = X[i, j];

            return r;
        }

        /// <summary>Least squares solution of <c>A * X = B</c></summary>
        /// <param name="value">Right-hand-side matrix with as many rows as <c>A</c> and any number of columns.</param>
        /// <returns>A matrix that minimized the two norm of <c>Q * R * X - B</c>.</returns>
        /// <exception cref="T:System.ArgumentException">Matrix row dimensions must be the same.</exception>
        /// <exception cref="T:System.InvalidOperationException">Matrix is rank deficient.</exception>
        public double[] Solve(double[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.GetLength(0) != qr.GetLength(0))
                throw new ArgumentException("Matrix row dimensions must agree.");

            if (!this.FullRank)
                throw new InvalidOperationException("Matrix is rank deficient.");

            // Copy right hand side
            var X = (double[])value.Clone();
            int m = qr.GetLength(0);
            int n = qr.GetLength(1);

            // Compute Y = transpose(Q)*B
            for (int k = 0; k < n; k++)
            {
                double s = 0;

                for (int i = k; i < m; i++)
                    s += qr[i, k] * X[i];

                s = -s / qr[k, k];

                for (int i = k; i < m; i++)
                    X[i] += s * qr[i, k];
            }

            // Solve R*X = Y;
            for (int k = n - 1; k >= 0; k--)
            {
                X[k] /= Rdiag[k];

                for (int i = 0; i < k; i++)
                    X[i] -= X[k] * qr[i, k];
            }

            return X.Submatrix(n);
        }

        /// <summary>Shows if the matrix <c>A</c> is of full rank.</summary>
        /// <value>The value is <see langword="true"/> if <c>R</c>, and hence <c>A</c>, has full rank.</value>
        public bool FullRank
        {
            get
            {
                int columns = qr.GetLength(1);

                for (int i = 0; i < columns; i++)
                {
                    if (this.Rdiag[i] == 0)
                        return false;
                }

                return true;
            }
        }

        /// <summary>Returns the upper triangular factor <c>R</c>.</summary>
        public double[,] UpperTriangularFactor
        {
            get
            {
                int n = this.qr.GetLength(1);
                var x = new double[n, n];

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (i < j)
                        {
                            x[i, j] = qr[i, j];
                        }
                        else if (i == j)
                        {
                            x[i, j] = Rdiag[i];
                        }
                        else
                        {
                            x[i, j] = 0;
                        }
                    }
                }

                return x;
            }
        }

        /// <summary>Returns the orthogonal factor <c>Q</c>.</summary>
        public double[,] OrthogonalFactor
        {
            get
            {
                int rows = qr.GetLength(0);
                int cols = qr.GetLength(1);
                var x = new double[rows, cols];

                for (int k = cols - 1; k >= 0; k--)
                {
                    for (int i = 0; i < rows; i++)
                        x[i, k] = 0;

                    x[k, k] = 1;
                    for (int j = k; j < cols; j++)
                    {
                        if (qr[k, k] != 0)
                        {
                            double s = 0;

                            for (int i = k; i < rows; i++)
                                s += qr[i, k] * x[i, j];

                            s = -s / qr[k, k];

                            for (int i = k; i < rows; i++)
                                x[i, j] += s * qr[i, k];
                        }
                    }
                }

                return x;
            }
        }

        /// <summary>Returns the diagonal of <c>R</c>.</summary>
        public double[] Diagonal
        {
            get { return Rdiag; }
        }

        /// <summary>Least squares solution of <c>A * X = I</c></summary>
        public double[,] Inverse()
        {
            if (!this.FullRank)
                throw new InvalidOperationException("Matrix is rank deficient.");

            // Copy right hand side
            int m = qr.GetLength(0);
            int n = qr.GetLength(1);
            var X = new double[m, m];

            // Compute Y = transpose(Q)
            for (int k = n - 1; k >= 0; k--)
            {
                for (int i = 0; i < m; i++)
                    X[k, i] = 0;

                X[k, k] = 1;
                for (int j = k; j < n; j++)
                {
                    if (qr[k, k] != 0)
                    {
                        double s = 0;

                        for (int i = k; i < m; i++)
                            s += qr[i, k] * X[j, i];

                        s = -s / qr[k, k];

                        for (int i = k; i < m; i++)
                            X[j, i] += s * qr[i, k];
                    }
                }
            }

            // Solve R*X = Y;
            for (int k = n - 1; k >= 0; k--)
            {
                for (int j = 0; j < m; j++)
                    X[k, j] /= Rdiag[k];

                for (int i = 0; i < k; i++)
                    for (int j = 0; j < m; j++)
                        X[i, j] -= X[k, j] * qr[i, k];
            }

            return X;
        }



        #region ICloneable Members

        private QrDecomposition()
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
            var clone = new QrDecomposition();
            clone.qr = (double[,])qr.Clone();
            clone.Rdiag = (double[])Rdiag.Clone();
            return clone;
        }

        #endregion

    }
}
