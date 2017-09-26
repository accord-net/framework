﻿// Accord Math Library
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
    ///   LU decomposition of a jagged rectangular matrix.
    /// </summary>
	///
    /// <remarks>
    ///   <para>
    ///     For an m-by-n matrix <c>A</c> with <c>m >= n</c>, the LU decomposition is an m-by-n
    ///     unit lower triangular matrix <c>L</c>, an n-by-n upper triangular matrix <c>U</c>,
    ///     and a permutation vector <c>piv</c> of length m so that <c>A(piv) = L*U</c>.
    ///     If m &lt; n, then <c>L</c> is m-by-m and <c>U</c> is m-by-n.</para>
    ///   <para>
    ///     The LU decomposition with pivoting always exists, even if the matrix is
    ///     singular, so the constructor will never fail.  The primary use of the
    ///     LU decomposition is in the solution of square systems of simultaneous
    ///     linear equations. This will fail if <see cref="Nonsingular"/> returns
    ///     <see langword="false"/>.</para>
	///   <para>
	///     If you need to compute a LU decomposition for matrices with data types other than
	///     double, see <see cref="JaggedLuDecompositionF"/>, <see cref="JaggedLuDecompositionD"/>. If you
	///     need to compute a LU decomposition for a multidimensional matrix, see <see cref="LuDecomposition"/>,
	///     <see cref="LuDecompositionF"/>, and <see cref="LuDecompositionD"/>.</para>
    /// </remarks>
	/// 
	/// <example>
    ///   <code source="Unit Tests\Accord.Tests.Math\Decompositions\JaggedLuDecompositionFTest.cs" region="doc_ctor" />
	/// </example>
	///
	/// <seealso cref="CholeskyDecomposition"/>
	/// <seealso cref="EigenvalueDecomposition"/>
    /// <seealso cref="SingularValueDecomposition"/>
    /// <seealso cref="JaggedEigenvalueDecomposition"/>
    /// <seealso cref="JaggedSingularValueDecomposition"/>
    /// 
    public sealed class JaggedLuDecomposition : ICloneable, ISolverArrayDecomposition<Double>
    {
        private int rows;
        private int cols;
        private Double[][] lu;

        private int pivotSign;
        private int[] pivotVector;


        // cache for lazy evaluation
        private Double? determinant;
        private double? lndeterminant;
        private bool? nonsingular;
        private Double[][] lowerTriangularFactor;
        private Double[][] upperTriangularFactor;



        /// <summary>
        ///   Constructs a new LU decomposition.
        /// </summary>    
        /// <param name="value">The matrix A to be decomposed.</param>
        /// <param name="inPlace">True if the decomposition should be performed over the
        /// <paramref name="value"/> matrix rather than on a copy of it. If true, the
        /// matrix will be destroyed during the decomposition. Default is false.</param>
        /// <param name="transpose">True if the decomposition should be performed on
        /// the transpose of A rather than A itself, false otherwise. Default is false.</param>
        /// 
        public JaggedLuDecomposition(Double[][] value, bool transpose = false, bool inPlace = false)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "Matrix cannot be null.");
            }

            if (transpose)
            {
                this.lu = value.Transpose(inPlace);
            }
            else
            {
                this.lu = inPlace ? value : value.MemberwiseClone();
            }

            this.rows = lu.Length;
            this.cols = lu[0].Length;
            this.pivotSign = 1;

            this.pivotVector = new int[rows];
            for (int i = 0; i < rows; i++)
                pivotVector[i] = i;

            Double[] LUcolj = new Double[rows];


            // Outer loop.
            for (int j = 0; j < cols; j++)
            {
                // Make a copy of the j-th column to localize references.
                for (int i = 0; i < rows; i++)
                    LUcolj[i] = lu[i][j];

                // Apply previous transformations.
                for (int i = 0; i < rows; i++)
                {
                    Double s = 0;

                    // Most of the time is spent in
                    // the following dot product:
                    int kmax = Math.Min(i, j);
                    for (int k = 0; k < kmax; k++)
                        s += lu[i][k] * LUcolj[k];

                    lu[i][j] = LUcolj[i] -= s;
                }

                // Find pivot and exchange if necessary.
                int p = j;
                for (int i = j + 1; i < rows; i++)
                {
                    if (Math.Abs(LUcolj[i]) > Math.Abs(LUcolj[p]))
                        p = i;
                }

                if (p != j)
                {
                    for (int k = 0; k < cols; k++)
                    {
                        Double t = lu[p][k];
                        lu[p][k] = lu[j][k];
                        lu[j][k] = t;
                    }

                    int v = pivotVector[p];
                    pivotVector[p] = pivotVector[j];
                    pivotVector[j] = v;

                    pivotSign = -pivotSign;
                }

                // Compute multipliers.
                if (j < rows && lu[j][j] != 0)
                {
                    for (int i = j + 1; i < rows; i++)
                        lu[i][j] /= lu[j][j];
                }
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
                    if (rows != cols)
                        throw new InvalidOperationException("Matrix must be square.");

                    bool nonSingular = true;
                    for (int i = 0; i < rows && nonSingular; i++)
                        if (lu[i][i] == 0) nonSingular = false;

                    nonsingular = nonSingular;
                }

                return nonsingular.Value;
            }
        }

        /// <summary>
        ///   Returns the determinant of the matrix.
        /// </summary>
        /// 
        public Double Determinant
        {
            get
            {
                if (!determinant.HasValue)
                {
                    if (rows != cols)
                        throw new InvalidOperationException("Matrix must be square.");

                    Double det = pivotSign;
                    for (int i = 0; i < rows; i++)
                        det *= lu[i][i];

                    determinant = det;
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
                    if (rows != cols)
                        throw new InvalidOperationException("Matrix must be square.");

                    double lndet = 0;
                    for (int i = 0; i < rows; i++)
                        lndet += Math.Log((double)Math.Abs(lu[i][i]));
                    lndeterminant = lndet;
                }

                return lndeterminant.Value;
            }
        }

        /// <summary>
        ///   Returns the lower triangular factor <c>L</c> with <c>A=LU</c>.
        /// </summary>
        /// 
        public Double[][] LowerTriangularFactor
        {
            get
            {
                if (lowerTriangularFactor == null)
                {
                    var L = new Double[rows][];

                    for (int i = 0; i < rows; i++)
                    {
                        L[i] = new Double[rows];

                        for (int j = 0; j < rows; j++)
                        {
                            if (i > j)
                                L[i][j] = lu[i][j];
                            else if (i == j)
                                L[i][j] = 1;
                            else
                                L[i][j] = 0;
                        }
                    }

                    lowerTriangularFactor = L;
                }

                return lowerTriangularFactor;
            }
        }

        /// <summary>
        ///   Returns the lower triangular factor <c>L</c> with <c>A=LU</c>.
        /// </summary>
        /// 
        public Double[][] UpperTriangularFactor
        {
            get
            {
                if (upperTriangularFactor == null)
                {
                    var U = new Double[rows][];
                    for (int i = 0; i < rows; i++)
                    {
                        U[i] = new Double[cols];

                        for (int j = 0; j < cols; j++)
                        {
                            if (i <= j)
                                U[i][j] = lu[i][j];
                            else
                                U[i][j] = 0;
                        }
                    }

                    upperTriangularFactor = U;
                }

                return upperTriangularFactor;
            }
        }

        /// <summary>
        ///   Returns the pivot permutation vector.
        /// </summary>
        /// 
        public int[] PivotPermutationVector
        {
            get { return this.pivotVector; }
        }

        /// <summary>
        ///   Solves a set of equation systems of type <c>A * X = I</c>.
        /// </summary>
        /// 
        public Double[][] Inverse()
        {
            if (!Nonsingular)
                throw new SingularMatrixException("Matrix is singular.");

            // Copy right hand side with pivoting
            var X = new Double[rows][];
            for (int i = 0; i < rows; i++)
            {
                X[i] = new Double[rows];
                int k = pivotVector[i];
                X[i][k] = 1;
            }

            // Solve L*Y = B(piv,:)
            for (int k = 0; k < rows; k++)
                for (int i = k + 1; i < rows; i++)
                    for (int j = 0; j < rows; j++)
                        X[i][j] -= X[k][j] * lu[i][k];

            // Solve U*X = I;
            for (int k = rows - 1; k >= 0; k--)
            {
                for (int j = 0; j < rows; j++)
                    X[k][j] /= lu[k][k];

                for (int i = 0; i < k; i++)
                    for (int j = 0; j < rows; j++)
                        X[i][j] -= X[k][j] * lu[i][k];
            }

            return X;
        }

        /// <summary>
        ///   Reverses the decomposition, reconstructing the original matrix <c>X</c>.
        /// </summary>
        /// 
        public Double[][] Reverse()
        {
            return LowerTriangularFactor.Dot(UpperTriangularFactor)
                .Get(PivotPermutationVector.ArgSort(), null);
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

        /// <summary>
        ///   Solves a set of equation systems of type <c>A * X = B</c>.
        /// </summary>
        /// <param name="value">Right hand side matrix with as many rows as <c>A</c> and any number of columns.</param>
        /// <returns>Matrix <c>X</c> so that <c>L * U * X = B</c>.</returns>
        /// 
        public Double[][] Solve(Double[][] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length != rows)
                throw new DimensionMismatchException("value", "The matrix should have the same number of rows as the decomposition.");

            if (!Nonsingular)
                throw new InvalidOperationException("Matrix is singular.");


            // Copy right hand side with pivoting
            int count = value[0].Length;
            var X = value.Get(pivotVector, null);


            // Solve L*Y = B(piv,:)
            for (int k = 0; k < cols; k++)
                for (int i = k + 1; i < cols; i++)
                    for (int j = 0; j < count; j++)
                        X[i][j] -= X[k][j] * lu[i][k];

            // Solve U*X = Y;
            for (int k = cols - 1; k >= 0; k--)
            {
                for (int j = 0; j < count; j++)
                    X[k][j] /= lu[k][k];

                for (int i = 0; i < k; i++)
                    for (int j = 0; j < count; j++)
                        X[i][j] -= X[k][j] * lu[i][k];
            }

            return X;
        }

		/// <summary>
        ///   Solves a set of equation systems of type <c>A * X = B</c> where B is a diagonal matrix.
        /// </summary>
        /// <param name="diagonal">Diagonal fo the right hand side matrix with as many rows as <c>A</c>.</param>
        /// <returns>Matrix <c>X</c> so that <c>L * U * X = B</c>.</returns>
        /// 
        public Double[][] SolveForDiagonal(Double[] diagonal)
        {
            if (diagonal == null)
                throw new ArgumentNullException("diagonal");

            return Solve(Jagged.Diagonal(diagonal));
        }

        /// <summary>
        ///   Solves a set of equation systems of type <c>X * A = B</c>.
        /// </summary>
        /// <param name="value">Right hand side matrix with as many columns as <c>A</c> and any number of rows.</param>
        /// <returns>Matrix <c>X</c> so that <c>X * L * U = A</c>.</returns>
        /// 
        public Double[][] SolveTranspose(Double[][] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length != rows)
                throw new DimensionMismatchException("value", "The matrix should have the same number of rows as the decomposition.");

            if (!Nonsingular)
                throw new SingularMatrixException("Matrix is singular.");


            // Copy right hand side with pivoting
            var X = value.Get(null, pivotVector);

            int count = X[0].Length;

            // Solve L*Y = B(piv,:)
            for (int k = 0; k < rows; k++)
                for (int i = k + 1; i < rows; i++)
                    for (int j = 0; j < count; j++)
                        X[j][i] -= X[j][k] * lu[i][k];

            // Solve U*X = Y;
            for (int k = rows - 1; k >= 0; k--)
            {
                for (int j = 0; j < count; j++)
                    X[j][k] /= lu[k][k];

                for (int i = 0; i < k; i++)
                    for (int j = 0; j < count; j++)
                        X[j][i] -= X[j][k] * lu[i][k];
            }

            return X;
        }

        /// <summary>
        ///   Solves a set of equation systems of type <c>A * X = B</c>.
        /// </summary>
        /// <param name="value">Right hand side column vector with as many rows as <c>A</c>.</param>
        /// <returns>Matrix <c>X</c> so that <c>L * U * X = B</c>.</returns>
        /// 
        public Double[] Solve(Double[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length != rows)
                throw new DimensionMismatchException("value", "The vector should have the same length as rows in the decomposition.");

            if (!Nonsingular)
                throw new InvalidOperationException("Matrix is singular.");


            // Copy right hand side with pivoting
            int count = value.Length;
            var b = new Double[count];
            for (int i = 0; i < b.Length; i++)
                b[i] = value[pivotVector[i]];


            // Solve L*Y = B
            var X = new Double[count];
            for (int i = 0; i < rows; i++)
            {
                X[i] = b[i];
                for (int j = 0; j < i; j++)
                    X[i] -= lu[i][j] * X[j];
            }

            // Solve U*X = Y;
            for (int i = rows - 1; i >= 0; i--)
            {
                for (int j = rows - 1; j > i; j--)
                    X[i] -= lu[i][j] * X[j];
                X[i] /= lu[i][i];
            }

            return X;
        }



        #region ICloneable Members

        private JaggedLuDecomposition()
        {
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// 
        public object Clone()
        {
            var lud = new JaggedLuDecomposition();
            lud.rows = this.rows;
            lud.cols = this.cols;
            lud.lu = this.lu.MemberwiseClone();
            lud.pivotSign = this.pivotSign;
            lud.pivotVector = (int[])this.pivotVector;
            return lud;
        }

        #endregion

    }
}

