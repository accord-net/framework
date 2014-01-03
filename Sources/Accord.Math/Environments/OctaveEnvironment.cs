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

namespace Accord.Math.Environments
{
    using Accord.Math;
    using Accord.Math.Decompositions;
    using System.CodeDom.Compiler;
    using System;
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    ///   Programming environment for Octave.
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    ///   This class implements a Domain Specific Language (DSL) for
    ///   C# which is remarkably similar to Octave. Please take a loook
    ///   on what is possible to do using this class in the examples
    ///   section.</para>
    ///   
    /// <para>
    ///   To use this class, inherit from <see cref="OctaveEnvironment"/>.
    ///   After this step, all code written inside your child class will
    ///   be able to use the syntax below:</para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   Using the <c>mat</c> and <c>ret</c> keywords, it is possible
    ///   to replicate most of the Octave environment inside plain C#
    ///   code. The example below demonstrates how to compute the 
    ///   Singular Value Decomposition of a matrix, which in turn was
    ///   generated using <see cref="Accord.Math.Matrix.Magic"/>.</para>
    ///   
    /// <code>
    ///   // Declare local matrices
    ///   mat u = _, s = _, v = _; 
    ///   
    ///   // Compute a new mat
    ///   mat M = magic(3) * 5;
    ///   
    ///   // Compute the SVD
    ///   ret [u, s, v] = svd(M);
    ///   
    ///   // Write the matrix
    ///   string str = u;
    ///   
    ///   /*
    ///        0.577350269189626 -0.707106781186548     0.408248290463863 
    ///   u =  0.577350269189626 -1.48007149071427E-16 -0.816496580927726 
    ///        0.577350269189626  0.707106781186548     0.408248290463863
    ///   */
    /// </code>
    /// 
    /// <para>
    ///   It is also possible to ignore certain parameters by
    ///   providing a wildcard in the return structure:</para>
    ///   
    /// <code>
    ///   // Declare local matrices
    ///   mat u = _, v = _; 
    ///   
    ///   // Compute a new mat
    ///   mat M = magic(3) * 5;
    ///   
    ///   // Compute the SVD
    ///   ret [u, _, v] = svd(M); // the second argument is omitted
    /// </code>
    /// 
    /// <para>
    ///   Standard matrix operations are also supported: </para>
    ///   
    /// <code>
    ///   
    ///   mat I = eye(3); // 3x3 identity matrix
    ///   
    ///   mat A = I * 2;  // matrix-scalar multiplication
    ///   
    ///   Console.WriteLine(A);
    ///   //
    ///   //        2 0 0
    ///   //    A = 0 2 0
    ///   //        0 0 2
    ///   
    ///   mat B = ones(3, 6); // 3x6 unit matrix
    ///   
    ///   Console.WriteLine(B);
    ///   //
    ///   //        1 1 1 1 1 1
    ///   //    B = 1 1 1 1 1 1
    ///   //        1 1 1 1 1 1
    ///   
    ///   mat C = new double[,]
    ///   {
    ///       { 2, 2, 2, 2, 2, 2 },
    ///       { 2, 0, 0, 0, 0, 2 },
    ///       { 2, 2, 2, 2, 2, 2 },
    ///   };
    ///   
    ///   mat D = A * B - C;
    ///   
    ///   Console.WriteLine(D);
    ///   //
    ///   //        0 0 0 0 0 0
    ///   //    C = 0 2 2 2 2 0
    ///   //        0 0 0 0 0 0
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="Accord.Math.Matrix"/>
    ///
    [GeneratedCode("", "")]
    public abstract class OctaveEnvironment
    {
        /// <summary>
        ///   Whether to use octave indexing or not.
        /// </summary>
        /// 
        protected static bool UseOctaveDimensionIndexing
        {
            get { return dimensionOffset == 1; }
            set { dimensionOffset = (value) ? 1 : 0; }
        }

        private static int dimensionOffset;


        /// <summary>Pi.</summary>
        protected static double pi = System.Math.PI;

        /// <summary>Machine epsilon.</summary>
        protected static double eps = Constants.DoubleEpsilon;

        // octave language commands
        /// <summary>Creates an identity matrix.</summary>
        protected static mat eye(int size) { return Matrix.Identity(size); }
        /// <summary>Inverts a matrix.</summary>
        protected static mat inv(double[,] matrix) { return Matrix.Inverse(matrix); }
        /// <summary>Inverts a matrix.</summary>
        protected static mat pinv(double[,] matrix) { return Matrix.PseudoInverse(matrix); }
        /// <summary>Creates a unit matrix.</summary>
        protected static mat ones(int size) { return Matrix.Create(size, 1.0); }
        /// <summary>Creates a unit matrix.</summary>
        protected static mat ones(int n, int m) { return Matrix.Create(n, m, 1.0); }
        /// <summary>Creates a unit matrix.</summary>
        protected static mat zeros(int size) { return Matrix.Create(size, 0.0); }
        /// <summary>Creates a unit matrix.</summary>
        protected static mat zeros(int n, int m) { return Matrix.Create(n, m, 0.0); }
        /// <summary>Random vector.</summary>
        protected static mat rand(int n, int m) { return Matrix.Random(n, m, 0, 1); }
        /// <summary>Size of a matrix.</summary>
        protected static double[] size(double[,] m) { return new double[] { m.GetLength(0), m.GetLength(1) }; }
        /// <summary>Rank of a matrix.</summary>
        protected static int rank(double[,] m) { return new SingularValueDecomposition(m).Rank; }


        /// <summary>Matrix sum vector.</summary>
        protected static double[] sum(double[,] matrix) { return Matrix.Sum(matrix); }
        /// <summary>Sum of vector elements.</summary>
        protected static double sum(double[] vector) { return Matrix.Sum(vector); }
        /// <summary>Product of vector elements.</summary>
        protected static double prod(double[] vector) { return Matrix.Product(vector); }
        /// <summary>Matrix sum vector.</summary>
        protected static double[] sum(double[,] matrix, int dimension) { return Matrix.Sum(matrix, dimension - dimensionOffset); }


        /// <summary>Rounding.</summary>
        protected static double round(double f) { return System.Math.Round(f); }
        /// <summary>Ceiling.</summary>
        protected static double ceil(double f) { return System.Math.Ceiling(f); }
        /// <summary>Flooring.</summary>
        protected static double floor(double f) { return System.Math.Floor(f); }

        /// <summary>Rounding.</summary>
        protected static double[] round(double[] f) { return Matrix.Round(f, 0); }
        /// <summary>Ceiling.</summary>
        protected static double[] ceil(double[] f) { return Matrix.Ceiling(f); }
        /// <summary>Flooring.</summary>
        protected static double[] floor(double[] f) { return Matrix.Floor(f); }

        /// <summary>Rounding.</summary>
        protected static double[,] round(double[,] f) { return Matrix.Round(f, 0); }
        /// <summary>Ceiling.</summary>
        protected static double[,] ceil(double[,] f) { return Matrix.Ceiling(f); }
        /// <summary>Flooring.</summary>
        protected static double[,] floor(double[,] f) { return Matrix.Floor(f); }


        /// <summary>Sin.</summary>
        protected static double sin(double d) { return System.Math.Sin(d); }
        /// <summary>Cos.</summary>
        protected static double cos(double d) { return System.Math.Cos(d); }
        /// <summary>Exponential value.</summary>
        protected static double exp(double d) { return System.Math.Exp(d); }
        /// <summary>Absolute value.</summary>
        protected static double abs(double d) { return System.Math.Abs(d); }
        /// <summary>Logarithm.</summary>
        protected static double log(double d) { return System.Math.Log(d); }

        /// <summary>Sin.</summary>
        protected static double[] sin(double[] d) { return Matrix.Apply(d, x => System.Math.Sin(x)); }
        /// <summary>Cos.</summary>
        protected static double[] cos(double[] d) { return Matrix.Apply(d, x => System.Math.Cos(x)); }
        /// <summary>Exponential value.</summary>
        protected static double[] exp(double[] d) { return Matrix.Apply(d, x => System.Math.Exp(x)); }
        /// <summary>Absolute value.</summary>
        protected static double[] abs(double[] d) { return Matrix.Apply(d, x => System.Math.Abs(x)); }
        /// <summary>Logarithm.</summary>
        protected static double[] log(double[] d) { return Matrix.Apply(d, x => System.Math.Log(x)); }

        /// <summary>Sin.</summary>
        protected static mat sin(double[,] d) { return Matrix.Apply(d, x => System.Math.Sin(x)); }
        /// <summary>Cos.</summary>
        protected static mat cos(double[,] d) { return Matrix.Apply(d, x => System.Math.Cos(x)); }
        /// <summary>Exponential value.</summary>
        protected static mat exp(double[,] d) { return Matrix.Apply(d, x => System.Math.Exp(x)); }
        /// <summary>Absolute value.</summary>
        protected static mat abs(double[,] d) { return Matrix.Apply(d, x => System.Math.Abs(x)); }
        /// <summary>Logarithm.</summary>
        protected static mat log(double[,] d) { return Matrix.Apply(d, x => System.Math.Log(x)); }

        /// <summary>Creates a magic square matrix.</summary>
        protected static mat magic(int n) { return Matrix.Magic(n); }

        // decompositions
        #region svd
        /// <summary>Singular value decomposition.</summary>
        protected List<mat> svd(double[,] m)
        {
            var svd = new SingularValueDecomposition(m, true, true, true);
            return new List<mat> 
            {
                svd.LeftSingularVectors, svd.DiagonalMatrix, svd.RightSingularVectors 
            };
        }

        #endregion

        #region qr
        /// <summary>QR decomposition.</summary>
        protected static void qr(double[,] m, out double[,] Q, out double[,] R)
        {
            var qr = new QrDecomposition(m);
            Q = qr.OrthogonalFactor;
            R = qr.UpperTriangularFactor;
        }

        /// <summary>QR decomposition.</summary>
        protected static void qr(double[,] m, out double[,] Q, out double[,] R, out double[] d)
        {
            var qr = new QrDecomposition(m);
            Q = qr.OrthogonalFactor;
            R = qr.UpperTriangularFactor;
            d = qr.Diagonal;
        }
        #endregion

        #region eig
        /// <summary>Eigenvalue decomposition.</summary>
        protected static double[] eig(double[,] a, out double[,] V)
        {
            var eig = new EigenvalueDecomposition(a);
            V = eig.Eigenvectors;
            return eig.RealEigenvalues;
        }

        /// <summary>Eigenvalue decomposition.</summary>
        protected static double[] eig(double[,] a, out double[,] V, out double[] im)
        {
            var eig = new EigenvalueDecomposition(a);
            V = eig.Eigenvectors;
            im = eig.ImaginaryEigenvalues;
            return eig.RealEigenvalues;
        }

        /// <summary>Eigenvalue decomposition.</summary>
        protected static double[] eig(double[,] a, double[,] b, out double[,] V)
        {
            var eig = new GeneralizedEigenvalueDecomposition(a, b);
            V = eig.Eigenvectors;
            return eig.RealEigenvalues;
        }

        /// <summary>Eigenvalue decomposition.</summary>
        protected static double[] eig(double[,] a, double[,] b, out double[,] V, out double[] im)
        {
            var eig = new GeneralizedEigenvalueDecomposition(a, b);
            V = eig.Eigenvectors;
            im = eig.ImaginaryEigenvalues;
            return eig.RealEigenvalues;
        }

        /// <summary>Eigenvalue decomposition.</summary>
        protected static double[] eig(double[,] a, double[,] b, out double[,] V, out double[] alphar, out double[] beta)
        {
            var eig = new GeneralizedEigenvalueDecomposition(a, b);
            V = eig.Eigenvectors;
            beta = eig.Betas;
            alphar = eig.RealAlphas;
            return eig.RealEigenvalues;
        }

        /// <summary>Eigenvalue decomposition.</summary>
        protected static double[] eig(double[,] a, double[,] b, out double[,] V, out double[] im, out double[] alphar, out double[] alphai, out double[] beta)
        {
            var eig = new GeneralizedEigenvalueDecomposition(a, b);
            V = eig.Eigenvectors;
            im = eig.ImaginaryEigenvalues;
            beta = eig.Betas;
            alphar = eig.RealAlphas;
            alphai = eig.ImaginaryAlphas;
            return eig.RealEigenvalues;
        }
        #endregion

        #region chol
        /// <summary>Cholesky decomposition.</summary>
        protected static double[,] chol(double[,] a)
        {
            var chol = new CholeskyDecomposition(a);
            return chol.LeftTriangularFactor;
        }
        #endregion

        /// <summary>
        ///   Matrix placeholder.
        /// </summary>
        /// 
        protected mat _
        {
            get { return new mat(null); }
        }

        /// <summary>
        ///   Return setter keyword.
        /// </summary>
        /// 
        protected readonly retm ret = new retm();

        /// <summary>
        ///   Return definition operator.
        /// </summary>
        /// 
        protected class retm
        {
            /// <summary>
            ///   Can be used to set output arguments
            ///   to the output of another function.
            /// </summary>
            /// 
            public List<mat> this[params mat[] a]
            {
                set
                {
                    int i = 0;
                    foreach (var m in value)
                    {
                        a[i++].matrix = m.matrix;
                    }
                }
            }
        }

        /// <summary>
        ///   Matrix definition operator.
        /// </summary>
        /// 
        protected class mat
        {
            /// <summary>
            ///   Inner matrix object.
            /// </summary>
            /// 
            public double[,] matrix;

            /// <summary>
            ///   Initializes a new instance of the <see cref="mat"/> class.
            /// </summary>
            /// 
            public mat(double[,] matrix)
            {
                this.matrix = matrix;
            }

            /// <summary>
            ///   Multiplication operator
            /// </summary>
            /// 
            public static mat operator *(mat a, mat b)
            {
                return a.matrix.Multiply(b.matrix);
            }

            /// <summary>
            ///   Multiplication operator
            /// </summary>
            /// 
            public static mat operator *(mat a, double x)
            {
                return a.matrix.Multiply(x);
            }

            /// <summary>
            ///   Multiplication operator
            /// </summary>
            /// 
            public static mat operator *(double x, mat a)
            {
                return a.matrix.Multiply(x);
            }

            /// <summary>
            ///   Addition operator
            /// </summary>
            /// 
            public static mat operator +(mat a, double x)
            {
                return a.matrix.Add(x);
            }

            /// <summary>
            ///   Addition operator
            /// </summary>
            /// 
            public static mat operator +(double x, mat a)
            {
                return a.matrix.Add(x);
            }

            /// <summary>
            ///   Addition operator
            /// </summary>
            /// 
            public static mat operator +(mat a, mat b)
            {
                return a.matrix.Add(b.matrix);
            }

            /// <summary>
            ///   Subtraction operator
            /// </summary>
            /// 
            public static mat operator -(mat a, mat b)
            {
                return a.matrix.Subtract(b.matrix);
            }

            /// <summary>
            ///   Subtraction operator
            /// </summary>
            /// 
            public static mat operator -(mat a, double x)
            {
                return a.matrix.Subtract(x);
            }

            /// <summary>
            ///   Subtraction operator
            /// </summary>
            /// 
            public static mat operator -(double x, mat a)
            {
                return a.matrix.Subtract(x);
            }

            /// <summary>
            ///   Equality operator.
            /// </summary>
            /// 
            public static bool operator ==(mat a, mat b)
            {
                return a.matrix.IsEqual(b.matrix);
            }

            /// <summary>
            ///   Inequality operator.
            /// </summary>
            /// 
            public static bool operator !=(mat a, mat b)
            {
                return !a.matrix.IsEqual(b.matrix);
            }

            /// <summary>
            ///   Implicit conversion from double[,].
            /// </summary>
            /// 
            public static implicit operator mat(double[,] m)
            {
                return new mat(m);
            }

            /// <summary>
            ///   Implicit conversion to double[,].
            /// </summary>
            /// 
            public static implicit operator double[,](mat m)
            {
                return m.matrix;
            }

            /// <summary>
            ///   Implicit conversion to string.
            /// </summary>
            /// 
            public static implicit operator string(mat m)
            {
                if ((Object)m == null) return String.Empty;
                return Matrix.ToString(m.matrix);
            }

            /// <summary>
            ///   Implicit conversion from list.
            /// </summary>
            /// 
            public static implicit operator mat(List<mat> m)
            {
                return m.First();
            }

            /// <summary>
            ///   Transpose operator.
            /// </summary>
            /// 
            public mat t
            {
                get { return matrix.Transpose(); }
            }

            /// <summary>
            ///   Determines whether the specified <see cref="System.Object" /> is equal to this instance.
            /// </summary>
            /// 
            public override bool Equals(object obj)
            {
                mat m = obj as mat;
                if (m != null)
                    return this.matrix == m.matrix;

                return matrix.Equals(obj);
            }

            /// <summary>
            ///   Returns a hash code for this instance.
            /// </summary>
            /// 
            public override int GetHashCode()
            {
                return matrix.GetHashCode();
            }
        }
    }
}
