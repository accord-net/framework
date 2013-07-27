// Accord.NET Sample Applications
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// First step:
using Accord.Math;
using System.Diagnostics;
using Accord.Math.Decompositions;


namespace MatrixOperations
{
    class Program
    {
        static void Main(string[] args)
        {

            #region 1. Declaring matrices

            // 1.1 Using standard .NET declaration
            double[,] A = 
            {
                {1, 2, 3},
                {6, 2, 0},
                {0, 0, 1}
            };

            double[,] B = 
            {
                {2, 0, 0},
                {0, 2, 0},
                {0, 0, 2}
            };

            {
                // 1.2 Using Accord extension methods
                double[,] Bi = Matrix.Identity(3).Multiply(2);
                double[,] Bj = Matrix.Diagonal(3, 2.0); // both are equal to B

                // 1.2 Using Accord extension methods with implicit typing
                var I = Matrix.Identity(3);
            }
            #endregion



            #region 2. Matrix Operations
            {
                // 2.1 Addition
                var C = A.Add(B);

                // 2.2 Subtraction
                var D = A.Subtract(B);

                // 2.3 Multiplication
                {
                    // 2.3.1 By a scalar
                    var halfM = A.Multiply(0.5);

                    // 2.3.2 By a vector
                    double[] m = A.Multiply(new double[] { 1, 2, 3 });

                    // 2.3.3 By a matrix
                    var M = A.Multiply(B);

                    // 2.4 Transposing
                    var At = A.Transpose();
                }
            }


            // 2.5 Elementwise operations

            // 2.5.1 Elementwise multiplication
            A.ElementwiseMultiply(B); // A.*B

            // 2.5.1 Elementwise division
            A.ElementwiseDivide(B); // A./B

            #endregion



            #region 3. Matrix characteristics
            {
                // 3.1 Calculating the determinant
                double det = A.Determinant();

                // 3.2 Calculating the trace
                double tr = A.Trace();

                // 3.3 Computing the sum vector
                {
                    double[] sumVector = A.Sum();

                    // 3.3.1 Computing the total sum of elements
                    double sum = sumVector.Sum();

                    // 3.3.2 Computing the sum along the rows
                    sumVector = A.Sum(0); // Equivalent to Octave's sum(A, 1)

                    // 3.3.2 Computing the sum along the columns
                    sumVector = A.Sum(1); // Equivalent to Octave's sum(A, 2)
                }
            }
            #endregion



            #region 4. Linear Algebra
            {
                // 4.1 Computing the inverse
                var invA = A.Inverse();

                // 4.2 Computing the pseudo-inverse
                var pinvA = A.PseudoInverse();

                // 4.3 Solving a linear system (Ax = B)
                var x = A.Solve(B);
            }
            #endregion



            #region 5. Special operators
            {
                // 5.1 Finding the indices of elements
                double[] v = { 5, 2, 2, 7, 1, 0 };
                int[] idx = v.Find(e => e > 2); // finding the index of every element in v higher than 2.

                // 5.2 Selecting elements by index
                double[] u = v.Submatrix(idx); // u is { 5, 7 }

                // 5.3 Converting between different matrix representations
                double[][] jaggedA = A.ToArray(); // from multidimensional to jagged array

                // 5.4 Extracting a column or row from the matrix
                double[] a = A.GetColumn(0); // retrieves the first column
                double[] b = B.GetRow(1); // retrieves the second row

                // 5.5 Taking the absolute of a matrix
                var absA = A.Abs();

                // 5.6 Applying some function to every element
                var newv = v.Apply(e => e + 1);
            }
            #endregion



            #region 7. Vector operations
            {
                double[] u = { 1, 2, 3 };
                double[] v = { 4, 5, 6 };

                var w1 = u.InnerProduct(v);
                var w2 = u.OuterProduct(v);
                var w3 = u.CartesianProduct(v);


                double[] m = { 1, 2, 3, 4 };
                double[,] M = Matrix.Reshape(m, 2, 2);
            }
            #endregion


            #region Decompositions
            {
                // Singular value decomposition
                {
                    SingularValueDecomposition svd = new SingularValueDecomposition(A);
                    var U = svd.LeftSingularVectors;
                    var S = svd.Diagonal;
                    var V = svd.RightSingularVectors;
                }
                // or (please see documentation for details)
                {
                    SingularValueDecomposition svd = new SingularValueDecomposition(A.Transpose());
                    var U = svd.RightSingularVectors;
                    var S = svd.Diagonal;
                    var V = svd.LeftSingularVectors;
                }

                // Eigenvalue decomposition
                {
                    EigenvalueDecomposition eig = new EigenvalueDecomposition(A);
                    var V = eig.Eigenvectors;
                    var D = eig.DiagonalMatrix;
                }

                // QR decomposition
                {
                    QrDecomposition qr = new QrDecomposition(A);
                    var Q = qr.OrthogonalFactor;
                    var R = qr.UpperTriangularFactor;
                }

                // Cholesky decomposition
                {
                    CholeskyDecomposition chol = new CholeskyDecomposition(A);
                    var R = chol.LeftTriangularFactor;
                }

                // LU decomposition
                {
                    LuDecomposition lu = new LuDecomposition(A);
                    var L = lu.LowerTriangularFactor;
                    var U = lu.UpperTriangularFactor;
                }

            }
            #endregion

        }
    }
}
