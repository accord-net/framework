// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    public sealed class JaggedQrDecomposition : ICloneable, ISolverArrayDecomposition<Double>
    {
        private int n; 
        private int m;

        private Double[][] qr;
        private Double[] Rdiag;

        /// <summary>Constructs a QR decomposition.</summary>    
        /// <param name="value">The matrix A to be decomposed.</param>
        public JaggedQrDecomposition(Double[][] value)
            : this(value, false)
        {
        }

        /// <summary>Constructs a QR decomposition.</summary>    
        /// <param name="value">The matrix A to be decomposed.</param>
        /// <param name="transpose">True if the decomposition should be performed on
        /// the transpose of A rather than A itself, false otherwise. Default is false.</param>
        public JaggedQrDecomposition(Double[][] value, bool transpose)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "Matrix cannot be null.");
            }

            if ((!transpose && value.Length < value[0].Length) ||
                 (transpose && value[0].Length < value.Length))
            {
                throw new ArgumentException("Matrix has more columns than rows.", "value");
            }

            this.qr = transpose ? value.Transpose() : (Double[][])value.MemberwiseClone();

            this.n = qr.Length;
            this.m = qr[0].Length;
            this.Rdiag = new Double[m];

            for (int k = 0; k < m; k++)
            {
                // Compute 2-norm of k-th column without under/overflow.
                Double nrm = 0;
                for (int i = k; i < qr.Length; i++)
                    nrm = Tools.Hypotenuse(nrm, qr[i][k]);

                if (nrm != 0)
                {
                    // Form k-th Householder vector.
                    if (qr[k][k] < 0)
                        nrm = -nrm;

                    for (int i = k; i < qr.Length; i++)
                        qr[i][k] /= nrm;

                    qr[k][k] += 1;

                    // Apply transformation to remaining columns.
                    for (int j = k + 1; j < m; j++)
                    {
                        Double s = 0;

                        for (int i = k; i < qr.Length; i++)
                            s += qr[i][k] * qr[i][j];

                        s = -s / qr[k][k];

                        for (int i = k; i < qr.Length; i++)
                            qr[i][j] += s * qr[i][k];
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
        public Double[][] Solve(Double[][] value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Matrix cannot be null.");

            if (value.Length != qr.Length)
                throw new ArgumentException("Matrix row dimensions must agree.");

            if (!this.FullRank)
                throw new InvalidOperationException("Matrix is rank deficient.");

            // Copy right hand side
            int count = value[0].Length;
            var X = (Double[][])value.MemberwiseClone();

            // Compute Y = transpose(Q)*B
            for (int k = 0; k < m; k++)
            {
                for (int j = 0; j < count; j++)
                {
                    Double s = 0;

                    for (int i = k; i < qr.Length; i++)
                        s += qr[i][k] * X[i][j];

                    s = -s / qr[k][k];

                    for (int i = k; i < qr.Length; i++)
                        X[i][j] += s * qr[i][k];
                }
            }

            // Solve R*X = Y;
            for (int k = m - 1; k >= 0; k--)
            {
                for (int j = 0; j < X[k].Length; j++)
                    X[k][j] /= Rdiag[k];

                for (int i = 0; i < k; i++)
                    for (int j = 0; j < X[i].Length; j++)
                        X[i][j] -= X[k][j] * qr[i][k];
            }

            var r = new Double[m][];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = new Double[count];
                for (int j = 0; j < r[i].Length; j++)
                    r[i][j] = X[i][j];
            }

            return r;
        }

        /// <summary>Least squares solution of <c>X * A = B</c></summary>
        /// <param name="value">Right-hand-side matrix with as many columns as <c>A</c> and any number of rows.</param>
        /// <returns>A matrix that minimized the two norm of <c>X * Q * R - B</c>.</returns>
        /// <exception cref="T:System.ArgumentException">Matrix column dimensions must be the same.</exception>
        /// <exception cref="T:System.InvalidOperationException">Matrix is rank deficient.</exception>
        public Double[][] SolveTranspose(Double[][] value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Matrix cannot be null.");

            if (value[0].Length != qr.Length)
                throw new ArgumentException("Matrix row dimensions must agree.");

            if (!this.FullRank)
                throw new InvalidOperationException("Matrix is rank deficient.");

            // Copy right hand side
            int count = value.Length;
            var X = value.Transpose();

            // Compute Y = transpose(Q)*B
            for (int k = 0; k < m; k++)
            {
                for (int j = 0; j < count; j++)
                {
                    Double s = 0;

                    for (int i = k; i < n; i++)
                        s += qr[i][k] * X[i][j];

                    s = -s / qr[k][k];

                    for (int i = k; i < n; i++)
                        X[i][j] += s * qr[i][k];
                }
            }

            // Solve R*X = Y;
            for (int k = m - 1; k >= 0; k--)
            {
                for (int j = 0; j < count; j++)
                    X[k][j] /= Rdiag[k];

                for (int i = 0; i < k; i++)
                    for (int j = 0; j < count; j++)
                        X[i][j] -= X[k][j] * qr[i][k];
            }

            var r = Jagged.Zeros<Double>(count, m);
            for (int i = 0; i < m; i++)
                for (int j = 0; j < count; j++)
                    r[j][i] = X[i][j];

            return r;
        }

        /// <summary>Least squares solution of <c>A * X = B</c></summary>
        /// <param name="value">Right-hand-side matrix with as many rows as <c>A</c> and any number of columns.</param>
        /// <returns>A matrix that minimized the two norm of <c>Q * R * X - B</c>.</returns>
        /// <exception cref="T:System.ArgumentException">Matrix row dimensions must be the same.</exception>
        /// <exception cref="T:System.InvalidOperationException">Matrix is rank deficient.</exception>
        public Double[] Solve(Double[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length != qr.Length)
                throw new ArgumentException("Matrix row dimensions must agree.");

            if (!this.FullRank)
                throw new InvalidOperationException("Matrix is rank deficient.");

            // Copy right hand side
            var X = (Double[])value.Clone();

            // Compute Y = transpose(Q)*B
            for (int k = 0; k < m; k++)
            {
                Double s = 0;

                for (int i = k; i < qr.Length; i++)
                    s += qr[i][k] * X[i];

                s = -s / qr[k][k];

                for (int i = k; i < qr.Length; i++)
                    X[i] += s * qr[i][k];
            }

            // Solve R*X = Y;
            for (int k = m - 1; k >= 0; k--)
            {
                X[k] /= Rdiag[k];

                for (int i = 0; i < k; i++)
                    X[i] -= X[k] * qr[i][k];
            }

            return X.First(m);
        }

        /// <summary>Shows if the matrix <c>A</c> is of full rank.</summary>
        /// <value>The value is <see langword="true"/> if <c>R</c>, and hence <c>A</c>, has full rank.</value>
        public bool FullRank
        {
            get
            {
                for (int i = 0; i < Rdiag.Length; i++)
                {
                    if (this.Rdiag[i] == 0)
                        return false;
                }

                return true;
            }
        }

        /// <summary>Returns the upper triangular factor <c>R</c>.</summary>
        public Double[][] UpperTriangularFactor
        {
            get
            {
                var x = Jagged.Zeros<Double>(n, m);

                for (int i = 0; i < qr.Length; i++)
                {
                    for (int j = 0; j < qr[i].Length; j++)
                    {
                        if (i < j)
                        {
                            x[i][j] = qr[i][j];
                        }
                        else if (i == j)
                        {
                            x[i][j] = Rdiag[i];
                        }
                        else
                        {
                            x[i][j] = 0;
                        }
                    }
                }

                return x;
            }
        }

        /// <summary>Returns the orthogonal factor <c>Q</c>.</summary>
        public Double[][] OrthogonalFactor
        {
            get
            {
                var x = Jagged.Zeros<Double>(n, n);

                for (int k = m - 1; k >= 0; k--)
                {
                    for (int i = 0; i < n; i++)
                        x[i][k] = 0;

                    x[k][k] = 1;
                    for (int j = k; j < m; j++)
                    {
                        if (qr[k][k] != 0)
                        {
                            Double s = 0;

                            for (int i = k; i < qr.Length; i++)
                                s += qr[i][k] * x[i][j];

                            s = -s / qr[k][k];

                            for (int i = k; i < qr.Length; i++)
                                x[i][j] += s * qr[i][k];
                        }
                    }
                }

                return x;
            }
        }

        /// <summary>Returns the diagonal of <c>R</c>.</summary>
        public Double[] Diagonal
        {
            get { return Rdiag; }
        }

        /// <summary>Least squares solution of <c>A * X = I</c></summary>
        public Double[][] Inverse()
        {
            if (!this.FullRank)
                throw new InvalidOperationException("Matrix is rank deficient.");

            return Solve(Jagged.Diagonal(n, n, (Double)1));
        }

        /// <summary>
        ///   Reverses the decomposition, reconstructing the original matrix <c>X</c>.
        /// </summary>
        /// 
        public Double[][] Reverse()
        {
            return OrthogonalFactor.Dot(UpperTriangularFactor);
        }

        /// <summary>
        ///   Computes <c>(Xt * X)^1</c> (the inverse of the covariance matrix). This
        ///   matrix can be used to determine standard errors for the coefficients when
        ///   solving a linear set of equations through any of the <see cref="Solve(Double[][])"/>
        ///   methods.
        /// </summary>
        /// 
        public Double[][] GetInformationMatrix()
        {
            var X = Reverse();
            return X.TransposeAndDot(X).Inverse();
        }


        #region ICloneable Members

        private JaggedQrDecomposition()
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
            var clone = new JaggedQrDecomposition();
            clone.qr = (Double[][])qr.MemberwiseClone();
            clone.Rdiag = (Double[])Rdiag.Clone();
            clone.m = m;
            clone.n = n;
            return clone;
        }

        #endregion

    }
}

