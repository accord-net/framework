// Accord Math Library
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

namespace Accord.Math
{
    using System;
    using Accord.Math.Decompositions;

    public static partial class Matrix
    {

        /// <summary>
        ///   Returns the solution matrix if the matrix is square or the least squares solution otherwise.
        /// </summary>
        /// 
        /// <param name="matrix">The matrix for the linear problem.</param>
        /// <param name="rightSide">The right side <c>b</c>.</param>
        /// <param name="leastSquares">True to produce a solution even if the 
        ///   <paramref name="matrix"/> is singular; false otherwise. Default is false.</param>
        /// 
        /// <remarks>
        ///   Please note that this does not check if the matrix is non-singular
        ///   before attempting to solve. If a least squares solution is desired
        ///   in case the matrix is singular, pass true to the <paramref name="leastSquares"/>
        ///   parameter when calling this function.
        /// </remarks>
        /// 
        /// <example>
        /// <code>
        /// // Create a matrix. Please note that this matrix
        /// // is singular (i.e. not invertible), so only a 
        /// // least squares solution would be feasible here.
        /// 
        /// double[,] matrix = 
        /// {
        ///     { 1, 2, 3 },
        ///     { 4, 5, 6 },
        ///     { 7, 8, 9 },
        /// };
        /// 
        /// // Define a right side matrix b:
        /// double[,] rightSide = { {1}, {2}, {3} };
        /// 
        /// // Solve the linear system Ax = b by finding x:
        /// double[,] x = Matrix.Solve(matrix, rightSide, leastSquares: true);
        /// 
        /// // The answer should be { {-1/18}, {2/18}, {5/18} }.
        /// </code>
        /// </example>
        /// 
        public static double[,] Solve(this double[,] matrix, double[,] rightSide, bool leastSquares = false)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (rows != rightSide.GetLength(0))
                throw new DimensionMismatchException("rightSide",
                    "The number of rows in the right hand side matrix must be "
                    + "equal to the number of rows in the problem matrix.");

            if (leastSquares)
            {
                return new SingularValueDecomposition(matrix,
                       computeLeftSingularVectors: true,
                       computeRightSingularVectors: true,
                       autoTranspose: true).Solve(rightSide);
            }


            if (rows == cols)
            {
                // Solve by LU Decomposition if matrix is square.
                return new LuDecomposition(matrix).Solve(rightSide);
            }
            else
            {
                if (cols < rows)
                {
                    // Solve by QR Decomposition if not.
                    return new QrDecomposition(matrix).Solve(rightSide);
                }
                else
                {
                    return new SingularValueDecomposition(matrix,
                        computeLeftSingularVectors: true,
                        computeRightSingularVectors: true,
                        autoTranspose: true).Solve(rightSide);
                }
            }
        }

        /// <summary>
        ///   Returns the solution matrix if the matrix is square or the least squares solution otherwise.
        /// </summary>
        /// 
        /// <param name="matrix">The matrix for the linear problem.</param>
        /// <param name="rightSide">The right side <c>b</c>.</param>
        /// <param name="leastSquares">True to produce a solution even if the 
        ///   <paramref name="matrix"/> is singular; false otherwise. Default is false.</param>
        /// 
        /// <remarks>
        ///   Please note that this does not check if the matrix is non-singular
        ///   before attempting to solve. If a least squares solution is desired
        ///   in case the matrix is singular, pass true to the <paramref name="leastSquares"/>
        ///   parameter when calling this function.
        /// </remarks>
        /// 
        /// <example>
        /// <code>
        /// // Create a matrix. Please note that this matrix
        /// // is singular (i.e. not invertible), so only a 
        /// // least squares solution would be feasible here.
        /// 
        /// double[,] matrix = 
        /// {
        ///     { 1, 2, 3 },
        ///     { 4, 5, 6 },
        ///     { 7, 8, 9 },
        /// };
        /// 
        /// // Define a right side vector b:
        /// double[] rightSide = { 1, 2, 3 };
        /// 
        /// // Solve the linear system Ax = b by finding x:
        /// double[] x = Matrix.Solve(matrix, rightSide, leastSquares: true);
        /// 
        /// // The answer should be { -1/18, 2/18, 5/18 }.
        /// </code>
        /// </example>
        /// 
        public static double[] Solve(this double[,] matrix, double[] rightSide, bool leastSquares = false)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            if (rightSide == null)
                throw new ArgumentNullException("rightSide");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (rows != rightSide.Length)
                throw new DimensionMismatchException("rightSide",
                    "The right hand side vector must have the same length"
                     + "as there are rows of the problem matrix.");

            if (leastSquares)
            {
                return new SingularValueDecomposition(matrix,
                      computeLeftSingularVectors: true,
                      computeRightSingularVectors: true,
                      autoTranspose: true).Solve(rightSide);
            }


            if (rows == cols)
            {
                // Solve by LU Decomposition if matrix is square.
                return new LuDecomposition(matrix).Solve(rightSide);
            }
            else
            {
                if (cols < rows)
                {
                    // Solve by QR Decomposition if not.
                    return new QrDecomposition(matrix).Solve(rightSide);
                }
                else
                {
                    return new SingularValueDecomposition(matrix,
                        computeLeftSingularVectors: true,
                        computeRightSingularVectors: true,
                        autoTranspose: true).Solve(rightSide);
                }
            }
        }

        /// <summary>
        ///   Computes the inverse of a matrix.
        /// </summary>
        /// 
        public static double[,] Inverse(this double[,] matrix)
        {
            return Inverse(matrix, false);
        }

        /// <summary>
        ///   Computes the inverse of a matrix.
        /// </summary>
        /// 
        public static double[,] Inverse(this double[,] matrix, bool inPlace)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (rows != cols)
                throw new ArgumentException("Matrix must be square", "matrix");

            if (rows == 3)
            {
                // Special case for 3x3 matrices
                double a = matrix[0, 0], b = matrix[0, 1], c = matrix[0, 2];
                double d = matrix[1, 0], e = matrix[1, 1], f = matrix[1, 2];
                double g = matrix[2, 0], h = matrix[2, 1], i = matrix[2, 2];

                double den = a * (e * i - f * h) -
                             b * (d * i - f * g) +
                             c * (d * h - e * g);

                if (den == 0)
                    throw new SingularMatrixException();

                double m = 1.0 / den;

                double[,] inv = (inPlace) ? matrix : new double[3, 3];
                inv[0, 0] = m * (e * i - f * h);
                inv[0, 1] = m * (c * h - b * i);
                inv[0, 2] = m * (b * f - c * e);
                inv[1, 0] = m * (f * g - d * i);
                inv[1, 1] = m * (a * i - c * g);
                inv[1, 2] = m * (c * d - a * f);
                inv[2, 0] = m * (d * h - e * g);
                inv[2, 1] = m * (b * g - a * h);
                inv[2, 2] = m * (a * e - b * d);

                return inv;
            }

            if (rows == 2)
            {
                // Special case for 2x2 matrices
                double a = matrix[0, 0], b = matrix[0, 1];
                double c = matrix[1, 0], d = matrix[1, 1];

                double den = a * d - b * c;

                if (den == 0)
                    throw new SingularMatrixException();

                double m = 1.0 / den;

                double[,] inv = (inPlace) ? matrix : new double[2, 2];
                inv[0, 0] = +m * d;
                inv[0, 1] = -m * b;
                inv[1, 0] = -m * c;
                inv[1, 1] = +m * a;

                return inv;
            }

            return new LuDecomposition(matrix, false, inPlace).Inverse();
        }

        /// <summary>
        ///   Computes the pseudo-inverse of a matrix.
        /// </summary>
        /// 
        public static double[,] PseudoInverse(this double[,] matrix)
        {
            return new SingularValueDecomposition(matrix,
                computeLeftSingularVectors: true,
                computeRightSingularVectors: true,
                autoTranspose: true).Inverse();
        }

    }
}
