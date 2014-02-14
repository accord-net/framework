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
    using System.Collections.Generic;
    using System.Linq;
    using Accord.Math.Decompositions;

    /// <summary>
    ///   Static class Matrix. Defines a set of extension methods
    ///   that operates mainly on multidimensional arrays and vectors.
    /// </summary>
    /// 
    /// 
    /// <remarks>
    ///   The matrix class is a static class containing several extension methods.
    ///   To use this class, import the <see cref="Accord.Math"/> and use the
    ///   standard .NET's matrices and jagged arrays. When you call the dot (.)
    ///   operator on those classes, the extension methods offered by this class
    ///   should become available through IntelliSense auto-complete.
    /// </remarks>
    /// 
    /// <example>
    ///   <h2>Introduction</h2>
    ///   
    ///   <para>
    ///     Declaring and using matrices in the Accord.NET Framework does
    ///     not requires much. In fact, it does not require anything else
    ///     that is not already present at the .NET Framework. If you have
    ///     already existing and working code using other libraries, you
    ///     don't have to convert your matrices to any special format used
    ///     by Accord.NET. This is because Accord.NET is built to interoperate
    ///     with other libraries and existing solutions, relying solely on
    ///     default .NET structures to work.</para>
    ///     
    ///   <para>
    ///     To begin, please add the following <c>using</c> directive on
    ///     top of your .cs (or equivalent) source code file: </para>
    ///     
    ///   <code>
    ///     using Accord.Math;
    ///   </code>
    ///   
    ///   <para>
    ///     This is all you need to start using the Accord.NET matrix library.</para>
    ///     
    ///   <h2>Creating matrices</h2>
    ///   
    ///   <para>
    ///     Let's start by declaring a matrix, or otherwise specifying matrices
    ///     from other sources. The most straightforward way to declare a matrix
    ///     in Accord.NET is simply using: </para>
    ///     
    /// <code>
    ///     double[,] matrix = 
    ///     {
    ///        { 1, 2 },
    ///        { 3, 4 },
    ///        { 5, 6 },
    ///    };
    /// </code>
    /// 
    ///  <para>
    ///    Yep, that is right. You don't need to create any fancy custom Matrix
    ///    classes or vectors to make Accord.NET work, which is a plus if you
    ///    have already existent code using other libraries. You are also free
    ///    to use both the multidimensional matrix syntax above or the jagged
    ///    matrix syntax below:</para>
    ///    
    /// <code>
    ///     double[][] matrix = 
    ///     {
    ///        new double[] { 1, 2 },
    ///        new double[] { 3, 4 },
    ///        new double[] { 5, 6 },
    ///    };
    /// </code>
    /// 
    ///  <para>
    ///    Special purpose matrices can also be created through specialized methods.
    ///    Those include</para>
    ///    
    /// <code>
    ///   // Creates a vector of indices
    ///   int[] idx = Matrix.Indices(0, 10);  // { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }
    ///   
    ///   // Creates a step vector within a given interval
    ///   double[] interval = Matrix.Interval(from: -2, to: 4); // { -2, -1, 0, 1, 2, 3, 4 };
    ///   
    ///   // Special matrices
    ///   double[,] I = Matrix.Identity(3);     // creates a 3x3 identity matrix
    ///   double[,] magic = Matrix.Magic(5);    // creates a magic square matrix of size 5
    ///   
    ///   double[] v = Matrix.Vector(5, 1.0);      // generates { 1, 1, 1, 1, 1 }
    ///   double[,] diagonal = Matrix.Diagonal(v); // matrix with v on its diagonal
    /// </code>
    ///    
    ///  <para>
    ///    Another way to declare matrices is by parsing the contents of a string:</para>
    ///    
    /// <code>
    ///   string str = @"1 2
    ///                  3 4";
    ///                  
    ///   double[,] matrix = Matrix.Parse(str);
    /// </code>
    /// 
    ///  <para>
    ///    You can even read directly from matrices formatted in C# syntax:</para>
    ///    
    ///  <code>
    ///   string str = @"double[,] matrix = 
    ///                  {
    ///                     { 1, 2 },
    ///                     { 3, 4 },
    ///                     { 5, 6 },
    ///                  }";
    ///                  
    ///   matrix[,] mdimensional = Matrix.Parse(str, CSharpMatrixFormatProvider.InvariantCulture);
    ///   matrix[,] jagged = Matrix.ParseJagged(str, CSharpMatrixFormatProvider.InvariantCulture);
    ///  </code>
    ///  
    ///  <para>
    ///    And even from <a href="http://www.gnu.org/software/octave/">Octave-compatible</a> syntax!</para>
    ///    
    ///  <code>
    ///   string str = "[1 2; 3 4]";
    ///                  
    ///   matrix[,] matrix = Matrix.Parse(str, OctaveMatrixFormatProvider.InvariantCulture);
    ///  </code>
    ///  
    ///  <para>
    ///    There are also other methods, such as specialization for arrays and other formats.
    ///    For more details, please take a look on <see cref="CSharpMatrixFormatProvider"/>,
    ///    <see cref="CSharpArrayFormatProvider"/>, <see cref="DefaultArrayFormatProvider"/>,
    ///    <see cref="DefaultMatrixFormatProvider"/> and <see cref="Matrix.Parse(string)"/>.
    ///  </para>
    ///    
    ///     
    ///  <h2>Matrix operations</h2>
    ///  
    ///  <para>
    ///    Albeit being simple <see cref="T:double[]"/> matrices, the framework leverages
    ///    .NET extension methods to support all basic matrix operations. For instance,
    ///    consider the elementwise operations (also known as dot operations in Octave):</para>
    /// 
    /// <code>
    ///   double[] vector = { 0, 2, 4 };
    ///   double[] a = vector.ElementwiseMultiply(2); // vector .* 2, generates { 0,  4,  8 }
    ///   double[] b = vector.ElementwiseDivide(2);   // vector ./ 2, generates { 0,  1,  2 }
    ///   double[] c = vector.ElementwisePower(2);    // vector .^ 2, generates { 0,  4, 16 }
    /// </code>
    /// 
    /// <para>
    ///   Operations between vectors, matrices, and both are also completely supported:</para>
    ///   
    /// <code>
    ///   // Declare two vectors
    ///   double[] u = { 1, 6, 3 };
    ///   double[] v = { 9, 4, 2 };
    /// 
    ///   // Products between vectors
    ///   double inner = u.InnerProduct(v);    // 39.0
    ///   double[,] outer = u.OuterProduct(v); // see below
    ///   double[] kronecker = u.KroneckerProduct(v); // { 9, 4, 2, 54, 24, 12, 27, 12, 6 }
    ///   double[][] cartesian = u.CartesianProduct(v); // all possible pair-wise combinations
    /// 
    /// /* outer =
    ///    { 
    ///       {  9,  4,  2 },
    ///       { 54, 24, 12 },
    ///       { 27, 12,  6 },
    ///    };                  */
    ///
    ///   // Addition
    ///   double[] addv = u.Add(v); // { 10, 10, 5 }
    ///   double[] add5 = u.Add(5); // {  6, 11, 8 }
    ///
    ///   // Elementwise operations
    ///   double[] abs = u.Abs();   // { 1, 6, 3 }
    ///   double[] log = u.Log();   // { 0, 1.79, 1.09 }
    ///   
    ///   // Apply *any* function to all elements in a vector
    ///   double[] cos = u.Apply(Math.Cos); // { 0.54, 0.96, -0.989 }
    ///   u.ApplyInPlace(Math.Cos); // can also do optionally in-place
    ///
    ///   
    ///   // Declare a matrix
    ///   double[,] M = 
    ///   {
    ///      { 0, 5, 2 },
    ///      { 2, 1, 5 }
    ///   };
    ///  
    ///   // Extract a subvector from v:
    ///   double[] vcut = v.Submatrix(0, 1); // { 9, 4 }
    ///   
    ///   // Some operations between vectors and matrices
    ///   double[] Mv = m.Multiply(v);    //  { 24, 32 }
    ///   double[] vM = vcut.Multiply(m); // { 8, 49, 38 }
    ///   
    ///   // Some operations between matrices
    ///   double[,] Md = m.MultiplyByDiagonal(v);   // { { 0, 20, 4 }, { 18, 4, 10 } }
    ///   double[,] MMt = m.MultiplyByTranspose(m); //   { { 29, 15 }, { 15, 30 } }
    /// </code>
    /// 
    /// <para>
    ///   Please note this is by no means an extensive list; please take a look on
    ///   all members available on this class or (preferably) use IntelliSense to
    ///   navigate through all possible options when trying to perform an operation.</para>
    /// </example>
    /// 
    /// <seealso cref="Accord.Math.DefaultMatrixFormatProvider"/>
    /// <seealso cref="Accord.Math.DefaultArrayFormatProvider"/>
    /// <seealso cref="Accord.Math.OctaveMatrixFormatProvider"/>
    /// <seealso cref="Accord.Math.OctaveArrayFormatProvider"/>
    /// <seealso cref="Accord.Math.CSharpMatrixFormatProvider"/>
    /// <seealso cref="Accord.Math.CSharpArrayFormatProvider"/>
    /// 
    public static partial class Matrix
    {



        #region Matrix-Matrix Multiplication

        /// <summary>
        ///   Computes the product <c>A*B</c> of two matrices <c>A</c> and <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The right matrix <c>B</c>.</param>
        /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        public static double[,] Multiply(this double[,] a, double[,] b)
        {
            double[,] r = new double[a.GetLength(0), b.GetLength(1)];
            Multiply(a, b, r);
            return r;
        }

        /// <summary>
        ///   Computes the product <c>A*B</c> of two matrices <c>A</c> and <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The right matrix <c>B</c>.</param>
        /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        public static double[][] Multiply(this double[][] a, double[][] b)
        {
            int rows = a.Length;
            int cols = b[0].Length;

            double[][] r = new double[rows][];
            for (int i = 0; i < rows; i++)
                r[i] = new double[cols];

            Multiply(a, b, r);
            return r;
        }

        /// <summary>
        ///   Computes the product <c>A*B</c> of two matrices <c>A</c> and <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The right matrix <c>B</c>.</param>
        /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        public static float[][] Multiply(this float[][] a, float[][] b)
        {
            int rows = a.Length;
            int cols = b[0].Length;

            float[][] r = new float[rows][];
            for (int i = 0; i < r.Length; i++)
                r[i] = new float[cols];

            Multiply(a, b, r);
            return r;
        }

        /// <summary>
        ///   Computes the product <c>A*B</c> of two matrices <c>A</c> and <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The right matrix <c>B</c>.</param>
        /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        public static double[][] Multiply(this float[][] a, double[][] b)
        {
            int rows = a.Length;
            int cols = b[0].Length;

            double[][] r = new double[rows][];
            for (int i = 0; i < r.Length; i++)
                r[i] = new double[cols];

            Multiply(a, b, r);
            return r;
        }


        /// <summary>
        ///   Computes the product <c>A*B</c> of two matrices <c>A</c> and <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The right matrix <c>B</c>.</param>
        /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        public static float[,] Multiply(this float[,] a, float[,] b)
        {
            float[,] r = new float[a.GetLength(0), b.GetLength(1)];
            Multiply(a, b, r);
            return r;
        }

        /// <summary>
        ///   Computes the product <c>R = A*B</c> of two matrices <c>A</c>
        ///   and <c>B</c>, storing the result in matrix <c>R</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The right matrix <c>B</c>.</param>
        /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B</c>
        ///   of the given matrices <c>A</c> and <c>B</c>.</param>
        /// 
        public static unsafe void Multiply(this double[,] a, double[,] b, double[,] result)
        {
            // TODO: enable argument checking
            // if (a.GetLength(1) != b.GetLength(0))
            //     throw new ArgumentException("Matrix dimensions must match");

            int n = a.GetLength(1);
            int m = result.GetLength(0); //a.GetLength(0);
            int p = result.GetLength(1); //b.GetLength(1);


            fixed (double* ptrA = a)
            {
                double[] Bcolj = new double[n];
                for (int j = 0; j < p; j++)
                {
                    for (int k = 0; k < Bcolj.Length; k++)
                        Bcolj[k] = b[k, j];

                    double* Arowi = ptrA;
                    for (int i = 0; i < m; i++)
                    {
                        double s = 0;
                        for (int k = 0; k < Bcolj.Length; k++)
                            s += *(Arowi++) * Bcolj[k];
                        result[i, j] = s;
                    }
                }
            }
        }

        /// <summary>
        ///   Computes the product <c>R = A*B</c> of two matrices <c>A</c>
        ///   and <c>B</c>, storing the result in matrix <c>R</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The right matrix <c>B</c>.</param>
        /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B</c>
        ///   of the given matrices <c>A</c> and <c>B</c>.</param>
        /// 
        public static void Multiply(this double[][] a, double[][] b, double[][] result)
        {
            // TODO: enable argument checking
            // if (a.GetLength(1) != b.GetLength(0))
            //     throw new ArgumentException("Matrix dimensions must match");


            int n = a[0].Length;
            int m = a.Length;
            int p = b[0].Length;

            double[] Bcolj = new double[n];
            for (int j = 0; j < p; j++)
            {
                for (int k = 0; k < n; k++)
                    Bcolj[k] = b[k][j];

                for (int i = 0; i < m; i++)
                {
                    double[] Arowi = a[i];

                    double s = 0;
                    for (int k = 0; k < n; k++)
                        s += Arowi[k] * Bcolj[k];

                    result[i][j] = s;
                }
            }
        }

        /// <summary>
        ///   Computes the product <c>R = A*B</c> of two matrices <c>A</c>
        ///   and <c>B</c>, storing the result in matrix <c>R</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The right matrix <c>B</c>.</param>
        /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B</c>
        ///   of the given matrices <c>A</c> and <c>B</c>.</param>
        /// 
        public static void Multiply(this float[][] a, float[][] b, float[][] result)
        {
            // TODO: enable argument checking
            // if (a.GetLength(1) != b.GetLength(0))
            //     throw new ArgumentException("Matrix dimensions must match");


            int n = a[0].Length;
            int m = a.Length;
            int p = b[0].Length;

            float[] Bcolj = new float[n];
            for (int j = 0; j < p; j++)
            {
                for (int k = 0; k < n; k++)
                    Bcolj[k] = b[k][j];

                for (int i = 0; i < m; i++)
                {
                    float[] Arowi = a[i];

                    float s = 0;
                    for (int k = 0; k < n; k++)
                        s += Arowi[k] * Bcolj[k];

                    result[i][j] = s;
                }
            }
        }

        /// <summary>
        ///   Computes the product <c>R = A*B</c> of two matrices <c>A</c>
        ///   and <c>B</c>, storing the result in matrix <c>R</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The right matrix <c>B</c>.</param>
        /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B</c>
        ///   of the given matrices <c>A</c> and <c>B</c>.</param>
        /// 
        public static void Multiply(this float[][] a, double[][] b, double[][] result)
        {
            // TODO: enable argument checking
            // if (a.GetLength(1) != b.GetLength(0))
            //     throw new ArgumentException("Matrix dimensions must match");


            int n = a[0].Length;
            int m = a.Length;
            int p = b[0].Length;

            double[] Bcolj = new double[n];
            for (int j = 0; j < p; j++)
            {
                for (int k = 0; k < n; k++)
                    Bcolj[k] = b[k][j];

                for (int i = 0; i < m; i++)
                {
                    float[] Arowi = a[i];

                    double s = 0;
                    for (int k = 0; k < n; k++)
                        s += Arowi[k] * Bcolj[k];

                    result[i][j] = s;
                }
            }
        }

        /// <summary>
        ///   Computes the product <c>R = A*B</c> of two matrices <c>A</c>
        ///   and <c>B</c>, storing the result in matrix <c>R</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The right matrix <c>B</c>.</param>
        /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B</c>
        ///   of the given matrices <c>A</c> and <c>B</c>.</param>
        /// 
        public static unsafe void Multiply(this float[,] a, float[,] b, float[,] result)
        {
            int acols = a.GetLength(1);
            int arows = a.GetLength(0);
            int bcols = b.GetLength(1);

            fixed (float* ptrA = a)
            {
                float[] Bcolj = new float[acols];
                for (int j = 0; j < bcols; j++)
                {
                    for (int k = 0; k < acols; k++)
                        Bcolj[k] = b[k, j];

                    float* Arowi = ptrA;
                    for (int i = 0; i < arows; i++)
                    {
                        float s = 0;
                        for (int k = 0; k < acols; k++)
                            s += *(Arowi++) * Bcolj[k];
                        result[i, j] = s;
                    }
                }
            }
        }


        /// <summary>
        ///   Computes the product <c>A*B'</c> of matrix <c>A</c> and transpose of <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The transposed right matrix <c>B</c>.</param>
        /// <returns>The product <c>A*B'</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        public static double[,] MultiplyByTranspose(this double[,] a, double[,] b)
        {
            double[,] r = new double[a.GetLength(0), b.GetLength(0)];
            MultiplyByTranspose(a, b, r);
            return r;
        }

        /// <summary>
        ///   Computes the product <c>A*B'</c> of matrix <c>A</c> and transpose of <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The transposed right matrix <c>B</c>.</param>
        /// <returns>The product <c>A*B'</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        public static float[,] MultiplyByTranspose(this float[,] a, float[,] b)
        {
            float[,] r = new float[a.GetLength(0), b.GetLength(0)];
            MultiplyByTranspose(a, b, r);
            return r;
        }

        /// <summary>
        ///   Computes the product <c>A*B'</c> of matrix <c>A</c> and
        ///   transpose of <c>B</c>, storing the result in matrix <c>R</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The transposed right matrix <c>B</c>.</param>
        /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B'</c>
        ///   of the given matrices <c>A</c> and <c>B</c>.</param>
        ///    
        public static unsafe void MultiplyByTranspose(this double[,] a, double[,] b, double[,] result)
        {
            int n = a.GetLength(1);
            int m = a.GetLength(0);
            int p = b.GetLength(0);

            fixed (double* ptrA = a)
            fixed (double* ptrB = b)
            fixed (double* ptrR = result)
            {
                double* rc = ptrR;

                for (int i = 0; i < m; i++)
                {
                    double* bColj = ptrB;
                    for (int j = 0; j < p; j++)
                    {
                        double* aColi = ptrA + n * i;

                        double s = 0;
                        for (int k = 0; k < n; k++)
                            s += *(aColi++) * *(bColj++);
                        *(rc++) = s;
                    }
                }
            }
        }

        /// <summary>
        ///   Computes the product <c>A*B'</c> of matrix <c>A</c> and
        ///   transpose of <c>B</c>, storing the result in matrix <c>R</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The transposed right matrix <c>B</c>.</param>
        /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B'</c>
        ///   of the given matrices <c>A</c> and <c>B</c>.</param>
        ///    
        public static unsafe void MultiplyByTranspose(this float[,] a, float[,] b, float[,] result)
        {
            int n = a.GetLength(1);
            int m = a.GetLength(0);
            int p = b.GetLength(0);

            fixed (float* ptrA = a)
            fixed (float* ptrB = b)
            fixed (float* ptrR = result)
            {
                float* rc = ptrR;

                for (int i = 0; i < m; i++)
                {
                    float* bColj = ptrB;
                    for (int j = 0; j < p; j++)
                    {
                        float* aColi = ptrA + n * i;

                        float s = 0;
                        for (int k = 0; k < n; k++)
                            s += *(aColi++) * *(bColj++);
                        *(rc++) = s;
                    }
                }
            }
        }


        /// <summary>
        ///   Computes the product <c>A'*B</c> of matrix <c>A</c> transposed and matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The transposed left matrix <c>A</c>.</param>
        /// <param name="b">The right matrix <c>B</c>.</param>
        /// <returns>The product <c>A'*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        public static double[,] TransposeAndMultiply(this double[,] a, double[,] b)
        {
            double[,] r = new double[a.GetLength(1), b.GetLength(1)];
            TransposeAndMultiply(a, b, r);
            return r;
        }

        /// <summary>
        ///   Computes the product <c>A'*B</c> of matrix <c>A</c> transposed and matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The transposed left matrix <c>A</c>.</param>
        /// <param name="b">The right matrix <c>B</c>.</param>
        /// <returns>The product <c>A'*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        public static double[][] TransposeAndMultiply(this double[][] a, double[][] b)
        {
            int aCols = a[0].Length;
            int bCols = b[0].Length;

            double[][] r = new double[aCols][];
            for (int i = 0; i < r.Length; i++)
                r[i] = new double[bCols];

            TransposeAndMultiply(a, b, r);

            return r;
        }

        /// <summary>
        ///   Computes the product <c>A'*B</c> of matrix <c>A</c> transposed and matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The transposed left matrix <c>A</c>.</param>
        /// <param name="b">The right matrix <c>B</c>.</param>
        /// <param name="result">The matrix <c>R</c> to store the product <c>R = A'*B</c>
        ///   of the given matrices <c>A</c> and <c>B</c>.</param>
        /// 
        public static unsafe void TransposeAndMultiply(this double[,] a, double[,] b, double[,] result)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");
            if (result == null) throw new ArgumentNullException("result");

            // TODO: Check dimensions
            // TODO: Change result to be an "out" value

            int n = a.GetLength(0);
            int m = a.GetLength(1);
            int p = b.GetLength(1);

            double[] Bcolj = new double[n];
            for (int i = 0; i < p; i++)
            {
                for (int k = 0; k < n; k++)
                    Bcolj[k] = b[k, i];

                for (int j = 0; j < m; j++)
                {
                    double s = 0;
                    for (int k = 0; k < n; k++)
                        s += a[k, j] * Bcolj[k];

                    result[j, i] = s;
                }
            }
        }

        /// <summary>
        ///   Computes the product <c>A'*B</c> of matrix <c>A</c> transposed and matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The transposed left matrix <c>A</c>.</param>
        /// <param name="b">The right matrix <c>B</c>.</param>
        /// <param name="result">The matrix <c>R</c> to store the product <c>R = A'*B</c>
        ///   of the given matrices <c>A</c> and <c>B</c>.</param>
        /// 
        public static unsafe void TransposeAndMultiply(this double[][] a, double[][] b, double[][] result)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");
            if (result == null) throw new ArgumentNullException("result");

            // TODO: Check dimensions
            // TODO: Change result to be an "out" value

            int n = a.Length;
            int m = a[0].Length;
            int p = b[0].Length;

            double[] Bcolj = new double[n];
            for (int i = 0; i < p; i++)
            {
                for (int k = 0; k < n; k++)
                    Bcolj[k] = b[k][i];

                for (int j = 0; j < m; j++)
                {
                    double s = 0;
                    for (int k = 0; k < n; k++)
                        s += a[k][j] * Bcolj[k];

                    result[j][i] = s;
                }
            }
        }


        /// <summary>
        ///   Computes the product <c>A'*B</c> of matrix <c>A</c> transposed and vector <c>b</c>.
        /// </summary>
        /// 
        /// <param name="a">The transposed left matrix <c>A</c>.</param>
        /// <param name="b">The right column vector <c>b</c>.</param>
        /// <returns>The product <c>A'*b</c> of the given matrices <c>A</c> and vector <c>b</c>.</returns>
        /// 
        public static double[] TransposeAndMultiply(this double[,] a, double[] b)
        {
            double[] r = new double[a.GetLength(1)];
            TransposeAndMultiply(a, b, r);
            return r;
        }

        /// <summary>
        ///   Computes the product <c>A'*b</c> of matrix <c>A</c> transposed and column vector <c>b</c>.
        /// </summary>
        /// 
        /// <param name="a">The transposed left matrix <c>A</c>.</param>
        /// <param name="b">The right column vector <c>b</c>.</param>
        /// <param name="result">The vector <c>r</c> to store the product <c>r = A'*b</c>
        ///   of the given matrix <c>A</c> and vector <c>b</c>.</param>
        /// 
        public static unsafe void TransposeAndMultiply(this double[,] a, double[] b, double[] result)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");
            if (result == null) throw new ArgumentNullException("result");

            for (int j = 0; j < result.Length; j++)
            {
                double s = 0;
                for (int k = 0; k < b.Length; k++)
                    s += a[k, j] * b[k];

                result[j] = s;
            }

        }

        /// <summary>
        ///   Computes the product A'*B of matrix <c>A</c> and diagonal matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The diagonal vector of right matrix <c>B</c>.</param>
        /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        public static double[,] TransposeAndMultiplyByDiagonal(this double[,] a, double[] b)
        {
            double[,] r = new double[a.GetLength(1), b.Length];
            TransposeAndMultiplyByDiagonal(a, b, r);
            return r;
        }

        /// <summary>
        ///   Computes the product A'*B of matrix <c>A</c> and diagonal matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The diagonal vector of right matrix <c>B</c>.</param>
        /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B</c>
        ///   of the given matrices <c>A</c> and <c>B</c>.</param>
        /// 
        public static unsafe void TransposeAndMultiplyByDiagonal(this double[,] a, double[] b, double[,] result)
        {
            if (a.GetLength(0) != b.Length)
                throw new ArgumentException("Matrix dimensions must match.");

            int m = a.GetLength(1);
            for (int i = 0; i < b.Length; i++)
                for (int j = 0; j < m; j++)
                    result[j, i] = a[i, j] * b[i];
        }

        /// <summary>
        ///   Computes the product A*B of matrix <c>A</c> and diagonal matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The diagonal vector of right matrix <c>B</c>.</param>
        /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        public static double[,] MultiplyByDiagonal(this double[,] a, double[] b)
        {
            double[,] r = new double[a.GetLength(0), b.Length];
            MultiplyByDiagonal(a, b, r);
            return r;
        }

        /// <summary>
        ///   Computes the product A*B of matrix <c>A</c> and diagonal matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The diagonal vector of right matrix <c>B</c>.</param>
        /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B</c>
        ///   of the given matrices <c>A</c> and <c>B</c>.</param>
        /// 
        public static unsafe void MultiplyByDiagonal(this double[,] a, double[] b, double[,] result)
        {
            if (a.GetLength(1) != b.Length)
                throw new ArgumentException("Matrix dimensions must match.");


            int rows = a.GetLength(0);

            fixed (double* ptrA = a, ptrR = result)
            {
                double* A = ptrA;
                double* R = ptrR;
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < b.Length; j++)
                        *R++ = *A++ * b[j];
            }
        }

        /// <summary>
        ///   Computes the product A*B of matrix <c>A</c> and diagonal matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The diagonal vector of right matrix <c>B</c>.</param>
        /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        public static float[,] MultiplyByDiagonal(this float[,] a, float[] b)
        {
            float[,] r = new float[a.GetLength(0), b.Length];
            MultiplyByDiagonal(a, b, r);
            return r;
        }

        /// <summary>
        ///   Computes the product A*B of matrix <c>A</c> and diagonal matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The diagonal vector of right matrix <c>B</c>.</param>
        /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B</c>
        ///   of the given matrices <c>A</c> and <c>B</c>.</param>
        /// 
        public static unsafe void MultiplyByDiagonal(this float[,] a, float[] b, float[,] result)
        {
            if (a.GetLength(1) != b.Length)
                throw new ArgumentException("Matrix dimensions must match.");


            int rows = a.GetLength(0);

            fixed (float* ptrA = a, ptrR = result)
            {
                float* A = ptrA;
                float* R = ptrR;
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < b.Length; j++)
                        *R++ = *A++ * b[j];
            }
        }

        /// <summary>
        ///   Computes the product A*inv(B) of matrix <c>A</c> and diagonal matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The diagonal vector of inverse right matrix <c>B</c>.</param>
        /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        public static double[,] DivideByDiagonal(this double[,] a, double[] b)
        {
            double[,] r = new double[a.GetLength(0), b.Length];
            DivideByDiagonal(a, b, r);
            return r;
        }

        /// <summary>
        ///   Computes the product A*inv(B) of matrix <c>A</c> and diagonal matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The diagonal vector of inverse right matrix <c>B</c>.</param>
        /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B</c>
        ///   of the given matrices <c>A</c> and <c>B</c>.</param>
        /// 
        public static unsafe void DivideByDiagonal(this double[,] a, double[] b, double[,] result)
        {
            if (a.GetLength(1) != b.Length)
                throw new ArgumentException("Matrix dimensions must match.");


            int rows = a.GetLength(0);

            fixed (double* ptrA = a, ptrR = result)
            {
                double* A = ptrA;
                double* R = ptrR;
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < b.Length; j++)
                        *R++ = *A++ / b[j];
            }
        }

        #endregion


        #region Matrix-Vector multiplication

        /// <summary>
        ///   Multiplies a row vector <c>v</c> and a matrix <c>A</c>,
        ///   giving the product <c>v'*A</c>.
        /// </summary>
        /// 
        /// <param name="rowVector">The row vector <c>v</c>.</param>
        /// <param name="matrix">The matrix <c>A</c>.</param>
        /// <returns>The product <c>v'*A</c>of the multiplication of the
        ///   given row vector <c>v</c> and matrix <c>A</c>.</returns>
        /// 
        public static double[] Multiply(this double[] rowVector, double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (rows != rowVector.Length)
                throw new DimensionMismatchException("matrix",
                    "Matrix must have the same number of rows as the length of the vector.");

            double[] r = new double[cols];

            for (int j = 0; j < cols; j++)
                for (int k = 0; k < rowVector.Length; k++)
                    r[j] += rowVector[k] * matrix[k, j];

            return r;
        }

        /// <summary>
        ///   Multiplies a row vector <c>v</c> and a matrix <c>A</c>,
        ///   giving the product <c>v'*A</c>.
        /// </summary>
        /// 
        /// <param name="rowVector">The row vector <c>v</c>.</param>
        /// <param name="matrix">The matrix <c>A</c>.</param>
        /// <returns>The product <c>v'*A</c>of the multiplication of the
        ///   given row vector <c>v</c> and matrix <c>A</c>.</returns>
        /// 
        public static float[] Multiply(this float[] rowVector, float[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (rows != rowVector.Length)
                throw new DimensionMismatchException("matrix",
                    "Matrix must have the same number of rows as the length of the vector.");

            float[] r = new float[cols];

            for (int j = 0; j < cols; j++)
                for (int k = 0; k < rowVector.Length; k++)
                    r[j] += rowVector[k] * matrix[k, j];

            return r;
        }

        /// <summary>
        ///   Multiplies a matrix <c>A</c> and a column vector <c>v</c>,
        ///   giving the product <c>A*v</c>
        /// </summary>
        /// 
        /// <param name="matrix">The matrix <c>A</c>.</param>
        /// <param name="columnVector">The column vector <c>v</c>.</param>
        /// <returns>The product <c>A*v</c> of the multiplication of the
        ///   given matrix <c>A</c> and column vector <c>v</c>.</returns>
        /// 
        public static double[] Multiply(this double[,] matrix, double[] columnVector)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (cols != columnVector.Length)
                throw new DimensionMismatchException("columnVector",
                    "Vector must have the same length as columns in the matrix.");

            double[] r = new double[rows];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columnVector.Length; j++)
                    r[i] += matrix[i, j] * columnVector[j];

            return r;
        }

        /// <summary>
        ///   Multiplies a matrix <c>A</c> and a column vector <c>v</c>,
        ///   giving the product <c>A*v</c>
        /// </summary>
        /// 
        /// <param name="matrix">The matrix <c>A</c>.</param>
        /// <param name="columnVector">The column vector <c>v</c>.</param>
        /// <returns>The product <c>A*v</c> of the multiplication of the
        ///   given matrix <c>A</c> and column vector <c>v</c>.</returns>
        /// 
        public static double[] Multiply(this double[][] matrix, double[] columnVector)
        {
            int rows = matrix.Length;
            int cols = matrix[0].Length;

            if (cols != columnVector.Length)
                throw new DimensionMismatchException("columnVector",
                    "Vector must have the same length as columns in the matrix.");

            double[] r = new double[rows];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columnVector.Length; j++)
                    r[i] += matrix[i][j] * columnVector[j];

            return r;
        }

        /// <summary>
        ///   Multiplies a matrix <c>A</c> and a column vector <c>v</c>,
        ///   giving the product <c>A*v</c>
        /// </summary>
        /// 
        /// <param name="matrix">The matrix <c>A</c>.</param>
        /// <param name="columnVector">The column vector <c>v</c>.</param>
        /// <returns>The product <c>A*v</c> of the multiplication of the
        ///   given matrix <c>A</c> and column vector <c>v</c>.</returns>
        /// 
        public static float[] Multiply(this float[,] matrix, float[] columnVector)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (cols != columnVector.Length)
                throw new DimensionMismatchException("columnVector",
                    "Vector must have the same length as columns in the matrix.");

            var r = new float[rows];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columnVector.Length; j++)
                    r[i] += matrix[i, j] * columnVector[j];

            return r;
        }

        /// <summary>
        ///   Multiplies a matrix <c>A</c> by a scalar <c>x</c>.
        /// </summary>
        /// 
        /// <param name="matrix">The matrix <c>A</c>.</param>
        /// <param name="x">The scalar <c>x</c>.</param>
        /// <param name="inPlace">True to perform the operation in-place,
        ///   overwriting the original matrix; false to return a new matrix.</param>
        /// 
        /// <returns>The product <c>A*x</c> of the multiplication of the
        ///   given matrix <c>A</c> and scalar <c>x</c>.</returns>
        /// 
        public static double[,] Multiply(this double[,] matrix, double x, bool inPlace = false)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[,] result = inPlace ? matrix : new double[rows, cols];
            Multiply(matrix, x, result);

            return result;
        }

        /// <summary>
        ///   Multiplies a matrix <c>A</c> by a scalar <c>x</c>.
        /// </summary>
        /// 
        /// <param name="matrix">The matrix <c>A</c>.</param>
        /// <param name="x">The scalar <c>x</c>.</param>
        /// <returns>The product <c>A*x</c> of the multiplication of the
        ///   given matrix <c>A</c> and scalar <c>x</c>.</returns>
        /// 
        public static double[,] Multiply(this double[,] matrix, double x)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[,] result = new double[rows, cols];
            Multiply(matrix, x, result);
            return result;
        }

        /// <summary>
        ///   Multiplies a matrix <c>A</c> by a scalar <c>x</c>.
        /// </summary>
        /// 
        /// <param name="matrix">The matrix <c>A</c>.</param>
        /// <param name="x">The scalar <c>x</c>.</param>
        /// <returns>The product <c>A*x</c> of the multiplication of the
        ///   given matrix <c>A</c> and scalar <c>x</c>.</returns>
        /// 
        public static float[,] Multiply(this float[,] matrix, float x)
        {
            float[,] result = new float[matrix.GetLength(0), matrix.GetLength(1)];
            Multiply(matrix, x, result);
            return result;
        }

        /// <summary>
        ///   Multiplies a matrix <c>A</c> by a scalar <c>x</c>.
        /// </summary>
        /// 
        /// <param name="matrix">The matrix <c>A</c>.</param>
        /// <param name="x">The scalar <c>x</c>.</param>
        /// <param name="result">The matrix <c>R</c> to store the product <c>R=A*x</c>
        ///   of the multiplication of the given matrix <c>A</c> and scalar <c>x</c>.</param>
        /// 
        public unsafe static void Multiply(this double[,] matrix, double x, double[,] result)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int length = matrix.Length;

            fixed (double* ptrA = matrix, ptrR = result)
            {
                double* pa = ptrA, pr = ptrR;
                for (int i = 0; i < length; i++, pa++, pr++)
                    *pr = *pa * x;
            }
        }

        /// <summary>
        ///   Multiplies a matrix <c>A</c> by a scalar <c>x</c>.
        /// </summary>
        /// 
        /// <param name="matrix">The matrix <c>A</c>.</param>
        /// <param name="x">The scalar <c>x</c>.</param>
        /// <param name="result">The matrix <c>R</c> to store the product <c>R=A*x</c>
        ///   of the multiplication of the given matrix <c>A</c> and scalar <c>x</c>.</param>
        /// 
        public unsafe static void Multiply(this float[,] matrix, float x, float[,] result)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int length = matrix.Length;

            fixed (float* ptrA = matrix, ptrR = result)
            {
                float* pa = ptrA, pr = ptrR;
                for (int i = 0; i < length; i++, pa++, pr++)
                    *pr = *pa * x;
            }
        }

        /// <summary>
        ///   Multiplies a vector <c>v</c> by a scalar <c>x</c>.
        /// </summary>
        /// <param name="vector">The vector <c>v</c>.</param>
        /// <param name="x">The scalar <c>x</c>.</param>
        /// <returns>The product <c>v*x</c> of the multiplication of the 
        ///   given vector <c>v</c> and scalar <c>x</c>.</returns>
        /// 
        public static double[] Multiply(this double[] vector, double x)
        {
            double[] r = new double[vector.Length];

            for (int i = 0; i < vector.Length; i++)
                r[i] = vector[i] * x;

            return r;
        }

        /// <summary>
        ///   Multiplies a vector <c>v</c> by a scalar <c>x</c>.
        /// </summary>
        /// <param name="vector">The vector <c>v</c>.</param>
        /// <param name="x">The scalar <c>x</c>.</param>
        /// <returns>The product <c>v*x</c> of the multiplication of the 
        ///   given vector <c>v</c> and scalar <c>x</c>.</returns>
        /// 
        public static float[] Multiply(this float[] vector, float x)
        {
            float[] r = new float[vector.Length];

            for (int i = 0; i < vector.Length; i++)
                r[i] = vector[i] * x;

            return r;
        }

        /// <summary>
        ///   Multiplies a scalar <c>x</c> by a matrix <c>A</c>.
        /// </summary>
        /// <param name="x">The scalar <c>x</c>.</param>
        /// <param name="matrix">The matrix <c>A</c>.</param>
        /// <returns>The product <c>x*A</c> of the multiplication of the
        ///   given scalar <c>x</c> and matrix <c>A</c>.</returns>
        /// 
        public static double[,] Multiply(this double x, double[,] matrix)
        {
            return matrix.Multiply(x);
        }

        /// <summary>
        ///   Multiplies a scalar <c>x</c> by a matrix <c>A</c>.
        /// </summary>
        /// <param name="x">The scalar <c>x</c>.</param>
        /// <param name="matrix">The matrix <c>A</c>.</param>
        /// <returns>The product <c>x*A</c> of the multiplication of the
        ///   given scalar <c>x</c> and matrix <c>A</c>.</returns>
        /// 
        public static float[,] Multiply(this float x, float[,] matrix)
        {
            return matrix.Multiply(x);
        }

        /// <summary>
        ///   Multiplies a scalar <c>x</c> by a vector <c>v</c>.
        /// </summary>
        /// <param name="x">The scalar <c>x</c>.</param>
        /// <param name="vector">The vector <c>v</c>.</param>
        /// <returns>The product <c>x*v</c> of the multiplication of the 
        ///   given scalar <c>x</c> and vector <c>v</c>.</returns>
        /// 
        public static double[] Multiply(this double x, double[] vector)
        {
            return vector.Multiply(x);
        }

        /// <summary>
        ///   Multiplies a scalar <c>x</c> by a vector <c>v</c>.
        /// </summary>
        /// <param name="x">The scalar <c>x</c>.</param>
        /// <param name="vector">The vector <c>v</c>.</param>
        /// <returns>The product <c>x*v</c> of the multiplication of the 
        ///   given scalar <c>x</c> and vector <c>v</c>.</returns>
        /// 
        public static float[] Multiply(this float x, float[] vector)
        {
            return vector.Multiply(x);
        }

        /// <summary>
        ///   Multiplies a scalar <c>x</c> by a vector <c>v</c>.
        /// </summary>
        /// <param name="x">The scalar <c>x</c>.</param>
        /// <param name="vector">The vector <c>v</c>.</param>
        /// <returns>The product <c>x*v</c> of the multiplication of the 
        ///   given scalar <c>x</c> and vector <c>v</c>.</returns>
        /// 
        public static double[] Multiply(this int x, double[] vector)
        {
            return vector.Multiply(x);
        }

        /// <summary>
        ///   Multiplies a scalar <c>x</c> by a vector <c>v</c>.
        /// </summary>
        /// <param name="x">The scalar <c>x</c>.</param>
        /// <param name="vector">The vector <c>v</c>.</param>
        /// <returns>The product <c>x*v</c> of the multiplication of the 
        ///   given scalar <c>x</c> and vector <c>v</c>.</returns>
        /// 
        public static float[] Multiply(this int x, float[] vector)
        {
            return vector.Multiply(x);
        }

        #endregion


        #region Division
        /// <summary>
        ///   Divides a scalar by a vector.
        /// </summary>
        /// 
        /// <param name="vector">A vector.</param>
        /// <param name="x">A scalar.</param>
        /// <param name="inPlace">True to perform the operation in-place,
        /// overwriting the original array; false to return a new array.</param>
        /// 
        /// <returns>The division quotient of the given vector <c>a</c> and scalar <c>b</c>.</returns>
        public static double[] Divide(this double x, double[] vector, bool inPlace = false)
        {
            double[] r = inPlace ? vector : new double[vector.Length];

            for (int i = 0; i < vector.Length; i++)
                r[i] = x / vector[i];

            return r;
        }

        /// <summary>
        ///   Divides a scalar by a vector.
        /// </summary>
        /// 
        /// <param name="vector">A vector.</param>
        /// <param name="x">A scalar.</param>
        /// <param name="inPlace">True to perform the operation in-place,
        /// overwriting the original array; false to return a new array.</param>
        /// 
        /// <returns>The division quotient of the given vector <c>a</c> and scalar <c>b</c>.</returns>
        /// 
        public static double[] Divide(this int x, double[] vector, bool inPlace = false)
        {
            double[] r = inPlace ? vector : new double[vector.Length];

            for (int i = 0; i < vector.Length; i++)
                r[i] = x / vector[i];

            return r;
        }

        /// <summary>
        ///   Divides a vector by a scalar.
        /// </summary>
        /// 
        /// <param name="vector">A vector.</param>
        /// <param name="x">A scalar.</param>
        /// <param name="inPlace">True to perform the operation in-place,
        /// overwriting the original array; false to return a new array.</param>
        /// 
        /// <returns>The division quotient of the given vector <c>a</c> and scalar <c>b</c>.</returns>
        /// 
        public static double[] Divide(this double[] vector, double x, bool inPlace = false)
        {
            double[] r = inPlace ? vector : new double[vector.Length];

            for (int i = 0; i < vector.Length; i++)
                r[i] = vector[i] / x;

            return r;
        }

        /// <summary>
        ///   Divides a vector by a scalar.
        /// </summary>
        /// 
        /// <param name="vector">A vector.</param>
        /// <param name="x">A scalar.</param>
        /// 
        /// <returns>The division quotient of the given vector <c>a</c> and scalar <c>b</c>.</returns>
        /// 
        public static double[] Divide(this int[] vector, double x)
        {
            double[] r = new double[vector.Length];

            for (int i = 0; i < vector.Length; i++)
                r[i] = vector[i] / x;

            return r;
        }

        /// <summary>
        ///   Divides a vector by a scalar.
        /// </summary>
        /// 
        /// <param name="vector">A vector.</param>
        /// <param name="x">A scalar.</param>
        /// 
        /// <returns>The division quotient of the given vector <c>a</c> and scalar <c>b</c>.</returns>
        /// 
        public static float[] Divide(this float[] vector, float x)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            float[] r = new float[vector.Length];

            for (int i = 0; i < vector.Length; i++)
                r[i] = vector[i] / x;

            return r;
        }

        /// <summary>
        ///   Elementwise divides a scalar by a vector.
        /// </summary>
        /// 
        /// <param name="vector">A vector.</param>
        /// <param name="x">A scalar.</param>
        /// 
        /// <returns>The division quotient of the given scalar <c>a</c> and vector <c>b</c>.</returns>
        /// 
        public static double[] Divide(this double x, double[] vector)
        {
            double[] r = new double[vector.Length];

            for (int i = 0; i < vector.Length; i++)
                r[i] = x / vector[i];

            return r;
        }


        /// <summary>
        ///   Divides two matrices by multiplying A by the inverse of B.
        /// </summary>
        /// 
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix (which will be inverted).</param>
        /// 
        /// <returns>The result from the division <c>AB^-1</c> of the given matrices.</returns>
        /// 
        public static double[,] Divide(this double[,] a, double[,] b)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");

            if (b.GetLength(0) == b.GetLength(1) &&
                a.GetLength(0) == a.GetLength(1))
            {
                // Solve by LU Decomposition if matrix is square.
                return new LuDecomposition(b, true).SolveTranspose(a);
            }
            else
            {
                // Solve by QR Decomposition if not.
                return new QrDecomposition(b, true).SolveTranspose(a);
            }
        }

        /// <summary>
        ///   Divides a matrix by a scalar.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix.</param>
        /// <param name="x">A scalar.</param>
        /// <param name="inPlace">True to perform the operation in-place,
        /// overwriting the original matrix; false to return a new matrix.</param>
        /// 
        /// <returns>The division quotient of the given matrix and scalar.</returns>
        /// 
        public static double[,] Divide(this double[,] matrix, double x, bool inPlace = false)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[,] r = inPlace ? matrix : new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = matrix[i, j] / x;

            return r;
        }

        /// <summary>
        ///   Divides a matrix by a scalar.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix.</param>
        /// <param name="x">A scalar.</param>
        /// 
        /// <returns>The division quotient of the given matrix and scalar.</returns>
        /// 
        public static float[,] Divide(this uint[,] matrix, float x)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            float[,] r = new float[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = matrix[i, j] / x;

            return r;
        }

        /// <summary>
        ///   Elementwise divides a scalar by a matrix.
        /// </summary>
        /// 
        /// <param name="x">A scalar.</param>
        /// <param name="matrix">A matrix.</param>
        /// 
        /// <returns>The elementwise division of the given scalar and matrix.</returns>
        /// 
        public static double[,] Divide(this double x, double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[,] r = new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = x / matrix[i, j];

            return r;
        }

        /// <summary>
        ///   Elementwise divides a scalar by a matrix.
        /// </summary>
        /// 
        /// <param name="x">A scalar.</param>
        /// <param name="matrix">A matrix.</param>
        /// 
        /// <returns>The elementwise division of the given scalar and matrix.</returns>
        /// 
        public static double[,] Divide(this int x, double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[,] r = new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = x / matrix[i, j];

            return r;
        }
        #endregion

        #region Products
        /// <summary>
        ///   Gets the inner product (scalar product) between two vectors (a'*b).
        /// </summary>
        /// 
        /// <param name="a">A vector.</param>
        /// <param name="b">A vector.</param>
        /// 
        /// <returns>The inner product of the multiplication of the vectors.</returns>
        /// 
        /// <remarks>
        ///  <para>
        ///    In mathematics, the dot product is an algebraic operation that takes two
        ///    equal-length sequences of numbers (usually coordinate vectors) and returns
        ///    a single number obtained by multiplying corresponding entries and adding up
        ///    those products. The name is derived from the dot that is often used to designate
        ///    this operation; the alternative name scalar product emphasizes the scalar
        ///    (rather than vector) nature of the result.</para>
        ///  <para>
        ///    The principal use of this product is the inner product in a Euclidean vector space:
        ///    when two vectors are expressed on an orthonormal basis, the dot product of their 
        ///    coordinate vectors gives their inner product.</para>  
        /// </remarks>
        /// 
        public static double InnerProduct(this double[] a, double[] b)
        {
            double r = 0.0;

            if (a.Length != b.Length)
                throw new ArgumentException("Vector dimensions must match", "b");

            for (int i = 0; i < a.Length; i++)
                r += a[i] * b[i];

            return r;
        }

        /// <summary>
        ///   Gets the inner product (scalar product) between two vectors (a'*b).
        /// </summary>
        /// 
        /// <param name="a">A vector.</param>
        /// <param name="b">A vector.</param>
        /// 
        /// <returns>The inner product of the multiplication of the vectors.</returns>
        /// 
        /// <remarks>
        ///  <para>
        ///    In mathematics, the dot product is an algebraic operation that takes two
        ///    equal-length sequences of numbers (usually coordinate vectors) and returns
        ///    a single number obtained by multiplying corresponding entries and adding up
        ///    those products. The name is derived from the dot that is often used to designate
        ///    this operation; the alternative name scalar product emphasizes the scalar
        ///    (rather than vector) nature of the result.</para>
        ///  <para>
        ///    The principal use of this product is the inner product in a Euclidean vector space:
        ///    when two vectors are expressed on an orthonormal basis, the dot product of their 
        ///    coordinate vectors gives their inner product.</para>  
        /// </remarks>
        /// 
        public static float InnerProduct(this float[] a, float[] b)
        {
            float r = 0.0f;

            if (a.Length != b.Length)
                throw new ArgumentException("Vector dimensions must match", "b");

            for (int i = 0; i < a.Length; i++)
                r += a[i] * b[i];

            return r;
        }

        /// <summary>
        ///   Gets the outer product (matrix product) between two vectors (a*bT).
        /// </summary>
        /// 
        /// <remarks>
        ///   In linear algebra, the outer product typically refers to the tensor
        ///   product of two vectors. The result of applying the outer product to
        ///   a pair of vectors is a matrix. The name contrasts with the inner product,
        ///   which takes as input a pair of vectors and produces a scalar.
        /// </remarks>
        /// 
        public static double[,] OuterProduct(this double[] a, double[] b)
        {
            double[,] r = new double[a.Length, b.Length];

            for (int i = 0; i < a.Length; i++)
                for (int j = 0; j < b.Length; j++)
                    r[i, j] += a[i] * b[j];

            return r;
        }

        /// <summary>
        ///   Vector product.
        /// </summary>
        /// 
        /// <remarks>
        ///   The cross product, vector product or Gibbs vector product is a binary operation
        ///   on two vectors in three-dimensional space. It has a vector result, a vector which
        ///   is always perpendicular to both of the vectors being multiplied and the plane
        ///   containing them. It has many applications in mathematics, engineering and physics.
        /// </remarks>
        /// 
        public static double[] VectorProduct(double[] a, double[] b)
        {
            return new double[] 
            {
                a[1]*b[2] - a[2]*b[1],
                a[2]*b[0] - a[0]*b[2],
                a[0]*b[1] - a[1]*b[0]
            };
        }

        /// <summary>
        ///   Vector product.
        /// </summary>
        /// 
        public static float[] VectorProduct(float[] a, float[] b)
        {
            return new float[]
            {
                a[1]*b[2] - a[2]*b[1],
                a[2]*b[0] - a[0]*b[2],
                a[0]*b[1] - a[1]*b[0]
            };
        }

        /// <summary>
        ///   Computes the Cartesian product of many sets.
        /// </summary>
        /// 
        /// <remarks>
        ///   References:
        ///   - http://blogs.msdn.com/b/ericlippert/archive/2010/06/28/computing-a-Cartesian-product-with-linq.aspx 
        /// </remarks>
        /// 
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> empty = new[] { Enumerable.Empty<T>() };

            return sequences.Aggregate(empty, (accumulator, sequence) =>
                from accumulatorSequence in accumulator
                from item in sequence
                select accumulatorSequence.Concat(new[] { item }));
        }

        /// <summary>
        ///   Computes the Cartesian product of many sets.
        /// </summary>
        /// 
        public static T[][] CartesianProduct<T>(params T[][] sequences)
        {
            var result = CartesianProduct(sequences as IEnumerable<IEnumerable<T>>);

            List<T[]> list = new List<T[]>();
            foreach (IEnumerable<T> point in result)
                list.Add(point.ToArray());

            return list.ToArray();
        }

        /// <summary>
        ///   Computes the Cartesian product of two sets.
        /// </summary>
        /// 
        public static T[][] CartesianProduct<T>(this T[] sequence1, T[] sequence2)
        {
            return CartesianProduct(new T[][] { sequence1, sequence2 });
        }

        /// <summary>
        ///   Computes the Kronecker product between two matrices.
        /// </summary>
        /// 
        /// <param name="a">The left matrix a.</param>
        /// <param name="b">The right matrix b.</param>
        /// 
        /// <returns>The Kronecker product of the two matrices.</returns>
        /// 
        public unsafe static double[,] KroneckerProduct(this double[,] a, double[,] b)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");

            int arows = a.GetLength(0);
            int acols = a.GetLength(1);

            int brows = b.GetLength(0);
            int bcols = b.GetLength(1);

            int crows = arows * brows;
            int ccols = acols * bcols;

            int block = brows * ccols;

            double[,] result = new double[crows, ccols];

            fixed (double* ptrR = result, ptrA = a, ptrB = b)
            {
                double* A = ptrA, Ri = ptrR;

                for (int i = 0; i < arows; Ri += block, i++)
                {
                    double* Rj = Ri;

                    for (int j = 0; j < acols; j++, Rj += bcols, A++)
                    {
                        double* R = Rj, B = ptrB;

                        for (int k = 0; k < brows; k++, R += ccols)
                        {
                            for (int l = 0; l < bcols; l++, B++)
                                *(R + l) = (*A) * (*B);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///   Computes the Kronecker product between two vectors.
        /// </summary>
        /// 
        /// <param name="a">The left vector a.</param>
        /// <param name="b">The right vector b.</param>
        /// 
        /// <returns>The Kronecker product of the two vectors.</returns>
        /// 
        public static double[] KroneckerProduct(this double[] a, double[] b)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");

            double[] result = new double[a.Length * b.Length];

            int k = 0;
            for (int i = 0; i < a.Length; i++)
                for (int j = 0; j < b.Length; j++)
                    result[k++] = a[i] * b[j];

            return result;
        }
        #endregion

        #region Addition and Subraction
        /// <summary>
        ///   Adds a scalar to each element of a matrix.
        /// </summary>
        /// 
        public static double[,] Add(this double[,] matrix, double x)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[,] r = new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = matrix[i, j] + x;

            return r;
        }

        /// <summary>
        ///   Subtracts a scalar to each element of a matrix.
        /// </summary>
        /// 
        public static double[,] Add(this double x, double[,] matrix)
        {
            return matrix.Add(x);
        }


        /// <summary>
        ///   Adds two matrices.
        /// </summary>
        /// 
        /// <param name="a">A matrix.</param>
        /// <param name="b">A matrix.</param>
        /// 
        /// <returns>The sum of the given matrices.</returns>
        /// 
        public unsafe static double[,] Add(this double[,] a, double[,] b)
        {
            if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1))
                throw new ArgumentException("Matrix dimensions must match", "b");

            int rows = a.GetLength(0);
            int cols = a.GetLength(1);
            int length = a.Length;

            double[,] r = new double[rows, cols];

            fixed (double* ptrA = a, ptrB = b, ptrR = r)
            {
                double* pa = ptrA, pb = ptrB, pr = ptrR;
                for (int i = 0; i < length; i++, pa++, pb++, pr++)
                    *pr = *pa + *pb;
            }

            return r;
        }

        /// <summary>
        ///   Adds two matrices.
        /// </summary>
        /// 
        /// <param name="a">A matrix.</param>
        /// <param name="b">A matrix.</param>
        /// 
        /// <returns>The sum of the given matrices.</returns>
        /// 
        public static double[][] Add(this double[][] a, double[][] b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Matrix dimensions must match", "b");

            double[][] r = new double[a.Length][];
            for (int i = 0; i < a.Length; i++)
            {
                r[i] = new double[a[i].Length];
                for (int j = 0; j < a[i].Length; j++)
                    r[i][j] = a[i][j] + b[i][j];
            }

            return r;
        }

        /// <summary>
        ///   Adds a matrix and a scalar.
        /// </summary>
        /// 
        /// <param name="a">A matrix.</param>
        /// <param name="x">A scalar.</param>
        /// 
        /// <returns>The sum of the given matrix and scalar.</returns>
        /// 
        public static double[][] Add(this double[][] a, double x)
        {
            double[][] r = new double[a.Length][];
            for (int i = 0; i < a.Length; i++)
            {
                r[i] = new double[a[i].Length];
                for (int j = 0; j < a[i].Length; j++)
                    r[i][j] = a[i][j] + x;
            }

            return r;
        }

        /// <summary>
        ///   Adds a vector to a column or row of a matrix.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix.</param>
        /// <param name="vector">A vector.</param>
        /// <param name="dimension">
        ///   Pass 0 if the vector should be added row-wise, 
        ///   or 1 if the vector should be added column-wise.
        /// </param>
        /// 
        public static double[,] Add(this double[,] matrix, double[] vector, int dimension)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[,] r = new double[rows, cols];

            if (dimension == 1)
            {
                if (rows != vector.Length) throw new DimensionMismatchException("vector",
                    "Length of vector should equal the number of rows in matrix.");

                for (int j = 0; j < cols; j++)
                    for (int i = 0; i < rows; i++)
                        r[i, j] = matrix[i, j] + vector[i];
            }
            else
            {
                if (cols != vector.Length) throw new DimensionMismatchException("vector",
                    "Length of vector should equal the number of columns in matrix.");

                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        r[i, j] = matrix[i, j] + vector[j];
            }

            return r;
        }

        /// <summary>
        ///   Adds a scalar to the diagonal of a matrix.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix.</param>
        /// <param name="scalar">A scalar.</param>
        /// <param name="inPlace">True to perform the operation in-place,
        /// overwriting the original matrix; false to return a new matrix.</param>
        /// 
        public static double[,] AddToDiagonal(this double[,] matrix, double scalar, bool inPlace = false)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            int min = Math.Min(rows, cols);

            double[,] r = inPlace ? matrix : (double[,])matrix.Clone();

            for (int i = 0; i < min; i++)
                r[i, i] = matrix[i, i] + scalar;

            return r;
        }

        /// <summary>
        ///   Adds a scalar to the diagonal of a matrix.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix.</param>
        /// <param name="scalar">A scalar.</param>
        /// <param name="inPlace">True to perform the operation in-place,
        /// overwriting the original matrix; false to return a new matrix.</param>
        /// 
        public static double[][] AddToDiagonal(this double[][] matrix, double scalar, bool inPlace = false)
        {
            int rows = matrix.Length;
            int cols = matrix[0].Length;

            int min = Math.Min(rows, cols);

            double[][] r = inPlace ? matrix : matrix.MemberwiseClone();

            for (int i = 0; i < min; i++)
                r[i][i] = matrix[i][i] + scalar;

            return r;
        }

        /// <summary>
        ///   Subtracts a scalar from the diagonal of a matrix.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix.</param>
        /// <param name="scalar">A scalar.</param>
        /// <param name="inPlace">True to perform the operation in-place,
        /// overwriting the original matrix; false to return a new matrix.</param>
        /// 
        public static double[][] SubtractFromDiagonal(this double[][] matrix, double scalar, bool inPlace = false)
        {
            return AddToDiagonal(matrix, -scalar, inPlace);
        }

        /// <summary>
        ///   Subtracts a scalar from the diagonal of a matrix.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix.</param>
        /// <param name="scalar">A scalar.</param>
        /// <param name="inPlace">True to perform the operation in-place,
        /// overwriting the original matrix; false to return a new matrix.</param>
        /// 
        public static double[,] SubtractFromDiagonal(this double[,] matrix, double scalar, bool inPlace = false)
        {
            return AddToDiagonal(matrix, -scalar, inPlace);
        }

        /// <summary>
        ///   Adds a vector to a column or row of a matrix.
        /// </summary>
        /// 
        /// <param name="a">A matrix.</param>
        /// <param name="b">A vector.</param>
        /// <param name="dimension">The dimension to add the vector to.</param>
        /// 
        public static double[,] Subtract(this double[,] a, double[] b, int dimension = 0)
        {
            int rows = a.GetLength(0);
            int cols = a.GetLength(1);

            double[,] r = new double[rows, cols];

            if (dimension == 1)
            {
                if (rows != b.Length) throw new ArgumentException(
                    "Length of B should equal the number of rows in A", "b");

                for (int j = 0; j < cols; j++)
                    for (int i = 0; i < rows; i++)
                        r[i, j] = a[i, j] - b[i];
            }
            else
            {
                if (cols != b.Length) throw new ArgumentException(
                    "Length of B should equal the number of cols in A", "b");

                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        r[i, j] = a[i, j] - b[j];
            }

            return r;
        }

        /// <summary>
        ///   Adds a vector to a column or row of a matrix.
        /// </summary>
        /// 
        /// <param name="a">A matrix.</param>
        /// <param name="b">A vector.</param>
        /// <param name="dimension">The dimension to add the vector to.</param>
        /// 
        public static double[][] Subtract(this double[][] a, double[] b, int dimension = 0)
        {
            int rows = a.Length;
            int cols = a[0].Length;

            double[][] r = new double[rows][];
            for (int i = 0; i < r.Length; i++)
                r[i] = new double[cols];


            if (dimension == 1)
            {
                if (rows != b.Length) throw new ArgumentException(
                    "Length of B should equal the number of rows in A", "b");

                for (int j = 0; j < cols; j++)
                    for (int i = 0; i < rows; i++)
                        r[i][j] = a[i][j] - b[i];
            }
            else
            {
                if (cols != b.Length) throw new ArgumentException(
                    "Length of B should equal the number of cols in A", "b");

                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        r[i][j] = a[i][j] - b[j];
            }

            return r;
        }

        /// <summary>
        ///   Adds two vectors.
        /// </summary>
        /// 
        /// <param name="a">A vector.</param>
        /// <param name="b">A vector.</param>
        /// 
        /// <returns>The addition of the given vectors.</returns>
        /// 
        public static double[] Add(this double[] a, double[] b)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");

            if (a.Length != b.Length)
                throw new ArgumentException("Vector lengths must match", "b");

            double[] r = new double[a.Length];

            for (int i = 0; i < a.Length; i++)
                r[i] = a[i] + b[i];

            return r;
        }

        /// <summary>
        ///   Adds two vectors.
        /// </summary>
        /// 
        /// <param name="a">A vector.</param>
        /// <param name="b">A vector.</param>
        /// 
        /// <returns>The addition of the given vectors.</returns>
        /// 
        public static double[] Add(this double[] a, double b)
        {
            if (a == null) throw new ArgumentNullException("a");

            double[] r = new double[a.Length];

            for (int i = 0; i < a.Length; i++)
                r[i] = a[i] + b;

            return r;
        }

        /// <summary>
        ///   Subtracts two matrices.
        /// </summary>
        /// 
        /// <param name="a">A matrix.</param>
        /// <param name="b">A matrix.</param>
        /// <param name="inPlace">True to perform the operation in-place,
        /// overwriting the original matrix; false to return a new matrix.</param>
        /// 
        /// <returns>The subtraction of the given matrices.</returns>
        /// 
        public unsafe static double[,] Subtract(this double[,] a, double[,] b, bool inPlace = false)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");

            if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1))
                throw new ArgumentException("Matrix dimensions must match", "b");

            int rows = a.GetLength(0);
            int cols = b.GetLength(1);
            int length = a.Length;

            double[,] r = inPlace ? a : new double[rows, cols];

            fixed (double* ptrA = a, ptrB = b, ptrR = r)
            {
                double* pa = ptrA, pb = ptrB, pr = ptrR;
                for (int i = 0; i < length; i++, pa++, pb++, pr++)
                    *pr = *pa - *pb;
            }
            return r;
        }

        /// <summary>
        ///   Subtracts two matrices.
        /// </summary>
        /// 
        /// <param name="a">A matrix.</param>
        /// <param name="b">A matrix.</param>
        /// 
        /// <returns>The subtraction of the given matrices.</returns>
        /// 
        public static double[][] Subtract(this double[][] a, double[][] b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Matrix dimensions must match", "b");

            double[][] r = new double[a.Length][];
            for (int i = 0; i < a.Length; i++)
            {
                r[i] = new double[a[i].Length];
                for (int j = 0; j < a[i].Length; j++)
                    r[i][j] = a[i][j] - b[i][j];
            }

            return r;
        }

        /// <summary>
        ///   Subtracts a scalar from each element of a matrix.
        /// </summary>
        /// 
        public static double[,] Subtract(this double[,] matrix, double x)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[,] r = new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = matrix[i, j] - x;

            return r;
        }

        /// <summary>
        ///   Elementwise subtracts an element of a matrix from a scalar.
        /// </summary>
        /// 
        /// <param name="x">A scalar.</param>
        /// <param name="matrix">A matrix.</param>
        /// 
        /// <returns>The elementwise subtraction of scalar a and matrix b.</returns>
        /// 
        public static double[,] Subtract(this double x, double[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[,] r = new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = x - matrix[i, j];

            return r;
        }

        /// <summary>
        ///   Elementwise subtracts an element of a matrix from a scalar.
        /// </summary>
        /// 
        /// <param name="x">A scalar.</param>
        /// <param name="matrix">A matrix.</param>
        /// 
        /// <returns>The elementwise subtraction of scalar a and matrix b.</returns>
        /// 
        public static double[,] Subtract(this int x, double[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[,] r = new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = x - matrix[i, j];

            return r;
        }

        /// <summary>
        ///   Subtracts two vectors.
        /// </summary>
        /// 
        /// <param name="a">A vector.</param>
        /// <param name="b">A vector.</param>
        /// <param name="inPlace">True to perform the operation in-place,
        /// overwriting the original array; false to return a new array.</param>
        /// 
        /// <returns>The subtraction of vector b from vector a.</returns>
        /// 
        public static double[] Subtract(this double[] a, double[] b, bool inPlace = false)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Vector length must match", "b");

            double[] r = inPlace ? a : new double[a.Length];

            for (int i = 0; i < a.Length; i++)
                r[i] = a[i] - b[i];

            return r;
        }

        /// <summary>
        ///   Subtracts two vectors.
        /// </summary>
        /// 
        /// <param name="a">A vector.</param>
        /// <param name="b">A vector.</param>
        /// <param name="inPlace">True to perform the operation in-place,
        /// overwriting the original array; false to return a new array.</param>
        /// 
        /// <returns>The subtraction of vector b from vector a.</returns>
        /// 
        public static int[] Subtract(this int[] a, int[] b, bool inPlace = false)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Vector length must match", "b");

            int[] r = inPlace ? a : new int[a.Length];

            for (int i = 0; i < a.Length; i++)
                r[i] = a[i] - b[i];

            return r;
        }


        /// <summary>
        ///   Subtracts a scalar from a vector.
        /// </summary>
        /// 
        /// <param name="vector">A vector.</param>
        /// <param name="x">A scalar.</param>
        /// <param name="inPlace">True to perform the operation in-place,
        /// overwriting the original array; false to return a new array.</param>
        /// 
        /// <returns>The subtraction of given scalar from all elements in the given vector.</returns>
        /// 
        public static double[] Subtract(this double[] vector, double x, bool inPlace = false)
        {
            double[] r = inPlace ? vector : new double[vector.Length];

            for (int i = 0; i < vector.Length; i++)
                r[i] = vector[i] - x;

            return r;
        }


        /// <summary>
        ///   Subtracts a scalar from a vector.
        /// </summary>
        /// 
        /// <param name="vector">A vector.</param>
        /// <param name="x">A scalar.</param>
        /// <param name="inPlace">True to perform the operation in-place,
        /// overwriting the original array; false to return a new array.</param>
        /// 
        /// <returns>The subtraction of given scalar from all elements in the given vector.</returns>
        /// 
        public static int[] Subtract(this int[] vector, int x, bool inPlace = false)
        {
            int[] r = inPlace ? vector : new int[vector.Length];

            for (int i = 0; i < vector.Length; i++)
                r[i] = vector[i] - x;

            return r;
        }

        /// <summary>
        ///   Subtracts a scalar from a vector.
        /// </summary>
        /// 
        /// <param name="vector">A vector.</param>
        /// <param name="x">A scalar.</param>
        /// 
        /// <returns>The subtraction of the given vector elements from the given scalar.</returns>
        /// 
        public static double[] Subtract(this double x, double[] vector)
        {
            double[] r = new double[vector.Length];

            for (int i = 0; i < vector.Length; i++)
                r[i] = vector[i] - x;

            return r;
        }
        #endregion


        /// <summary>
        ///   Multiplies a matrix by itself <c>n</c> times.
        /// </summary>
        /// 
        public static double[,] Power(double[,] matrix, int n)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            if (!matrix.IsSquare())
                throw new ArgumentException("Matrix must be square", "matrix");

            if (n == 0)
                return Matrix.Identity(matrix.GetLength(0));

            // TODO: Reduce the number of memory allocations
            // TODO: Use bitwise operations instead of strings

            double[,] result = matrix;
            string bin = System.Convert.ToString(n, 2);
            for (int i = 1; i < bin.Length; i++)
            {
                result = Matrix.Multiply(result, result);

                if (bin[i] == '1')
                    result = Matrix.Multiply(result, matrix);
            }

            return result;
        }

    }
}
