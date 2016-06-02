// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    ///   double[,] multid = Matrix.Parse(str, CSharpMatrixFormatProvider.InvariantCulture);
    ///   double[,] jagged = Matrix.ParseJagged(str, CSharpMatrixFormatProvider.InvariantCulture);
    ///  </code>
    ///  
    ///  <para>
    ///    And even from <a href="http://www.gnu.org/software/octave/">Octave-compatible</a> syntax!</para>
    ///    
    ///  <code>
    ///   string str = "[1 2; 3 4]";
    ///                  
    ///   double[,] matrix = Matrix.Parse(str, OctaveMatrixFormatProvider.InvariantCulture);
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
        [Obsolete("Please use Dot instead.")]
        public static double[,] Multiply(this double[,] a, double[,] b)
        {
            return Dot(a, b);
        }

        /// <summary>
        ///   Computes the product <c>A*B</c> of two matrices <c>A</c> and <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The right matrix <c>B</c>.</param>
        /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        [Obsolete("Please use Dot instead.")]
        public static double[][] Multiply(this double[][] a, double[][] b)
        {
            return Dot(a, b);
        }

        /// <summary>
        ///   Computes the product <c>A*B</c> of two matrices <c>A</c> and <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The right matrix <c>B</c>.</param>
        /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        [Obsolete("Please use Dot instead.")]
        public static float[][] Multiply(this float[][] a, float[][] b)
        {
            return Dot(a, b);
        }

        /// <summary>
        ///   Computes the product <c>A*B</c> of two matrices <c>A</c> and <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The right matrix <c>B</c>.</param>
        /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        [Obsolete("Please use Dot instead.")]
        public static double[][] Multiply(this float[][] a, double[][] b)
        {
            return Dot(a, b);
        }


        /// <summary>
        ///   Computes the product <c>A*B</c> of two matrices <c>A</c> and <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The right matrix <c>B</c>.</param>
        /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        [Obsolete("Please use Dot instead.")]
        public static float[,] Multiply(this float[,] a, float[,] b)
        {
            return Dot(a, b);
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
        [Obsolete("Please use Dot instead.")]
        public static void Multiply(this double[,] a, double[,] b, double[,] result)
        {
            Dot(a, b, result);
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
        [Obsolete("Please use Dot instead.")]
        public static void Multiply(this double[][] a, double[][] b, double[][] result)
        {
            Dot(a, b, result);
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
        [Obsolete("Please use Dot instead.")]
        public static void Multiply(this double[,] a, double[,] b, double[][] result)
        {
            Dot(a, b).CopyTo(result);
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
        [Obsolete("Please use Dot instead.")]
        public static void Multiply(this float[][] a, float[][] b, float[][] result)
        {
            Dot(a, b, result);
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
        [Obsolete("Please use Dot instead.")]
        public static void Multiply(this float[][] a, double[][] b, double[][] result)
        {
            Dot(a, b, result);
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
        [Obsolete("Please use Dot instead.")]
        public static void Multiply(this float[,] a, float[,] b, float[,] result)
        {
            Dot(a, b, result);
        }


        /// <summary>
        ///   Computes the product <c>A*B'</c> of matrix <c>A</c> and transpose of <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The transposed right matrix <c>B</c>.</param>
        /// <returns>The product <c>A*B'</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        [Obsolete("Please use DotWithTransposed instead.")]
        public static double[,] MultiplyByTranspose(this double[,] a, double[,] b)
        {
            return DotWithTransposed(a, b);
        }

        /// <summary>
        ///   Computes the product <c>A*B'</c> of matrix <c>A</c> and transpose of <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The transposed right matrix <c>B</c>.</param>
        /// <returns>The product <c>A*B'</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        [Obsolete("Please use DotWithTransposed instead.")]
        public static float[,] MultiplyByTranspose(this float[,] a, float[,] b)
        {
            return DotWithTransposed(a, b);
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
        [Obsolete("Please use DotWithTransposed instead.")]
        public static void MultiplyByTranspose(this double[,] a, double[,] b, double[,] result)
        {
            DotWithTransposed(a, b, result);
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
        [Obsolete("Please use DotWithTransposed instead.")]
        public static void MultiplyByTranspose(this float[,] a, float[,] b, float[,] result)
        {
            DotWithTransposed(a, b, result);
        }


        /// <summary>
        ///   Computes the product <c>A'*B</c> of matrix <c>A</c> transposed and matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The transposed left matrix <c>A</c>.</param>
        /// <param name="b">The right matrix <c>B</c>.</param>
        /// <returns>The product <c>A'*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        [Obsolete("Please use TransposeAndDot instead.")]
        public static double[,] TransposeAndMultiply(this double[,] a, double[,] b)
        {
            return TransposeAndDot(a, b);
        }

        /// <summary>
        ///   Computes the product <c>A'*B</c> of matrix <c>A</c> transposed and matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The transposed left matrix <c>A</c>.</param>
        /// <param name="b">The right matrix <c>B</c>.</param>
        /// <returns>The product <c>A'*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        [Obsolete("Please use TransposeAndDot instead.")]
        public static double[][] TransposeAndMultiply(this double[][] a, double[][] b)
        {
            return TransposeAndDot(a, b);
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
        [Obsolete("Please use TransposeAndDot instead.")]
        public static void TransposeAndMultiply(this double[,] a, double[,] b, double[,] result)
        {
            TransposeAndDot(a, b, result);
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
        [Obsolete("Please use TransposeAndDot instead.")]
        public static void TransposeAndMultiply(this double[][] a, double[][] b, double[][] result)
        {
            TransposeAndDot(a, b, result);
        }


        /// <summary>
        ///   Computes the product <c>A'*B</c> of matrix <c>A</c> transposed and vector <c>b</c>.
        /// </summary>
        /// 
        /// <param name="a">The transposed left matrix <c>A</c>.</param>
        /// <param name="b">The right column vector <c>b</c>.</param>
        /// <returns>The product <c>A'*b</c> of the given matrices <c>A</c> and vector <c>b</c>.</returns>
        /// 
        [Obsolete("Please use TransposeAndDot instead.")]
        public static double[] TransposeAndMultiply(this double[,] a, double[] b)
        {
            return TransposeAndDot(a, b);
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
        [Obsolete("Please use TransposeAndDot instead.")]
        public static void TransposeAndMultiply(this double[,] a, double[] b, double[] result)
        {
            TransposeAndDot(a, b, result);
        }

        /// <summary>
        ///   Computes the product A'*B of matrix <c>A</c> and diagonal matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The diagonal vector of right matrix <c>B</c>.</param>
        /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        [Obsolete("Please use TransposeAndDotWithDiagonal instead.")]
        public static double[,] TransposeAndMultiplyByDiagonal(this double[,] a, double[] b)
        {
            return TransposeAndDotWithDiagonal(a, b);
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
        [Obsolete("Please use TransposeAndDotWithDiagonal instead.")]
        public static void TransposeAndMultiplyByDiagonal(this double[,] a, double[] b, double[,] result)
        {
            TransposeAndDotWithDiagonal(a, b, result);
        }

        /// <summary>
        ///   Computes the product A*B of matrix <c>A</c> and diagonal matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The diagonal vector of right matrix <c>B</c>.</param>
        /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        [Obsolete("Please use DotWithDiagonal instead.")]
        public static double[][] MultiplyByDiagonal(this double[][] a, double[] b)
        {
            return DotWithDiagonal(a, b);
        }

        /// <summary>
        ///   Computes the product A*B of matrix <c>A</c> and diagonal matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The diagonal vector of right matrix <c>B</c>.</param>
        /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        [Obsolete("Please use DotWithDiagonal instead.")]
        public static float[][] MultiplyByDiagonal(this float[][] a, float[] b)
        {
            return DotWithDiagonal(a, b);
        }

        /// <summary>
        ///   Computes the product A*B of matrix <c>A</c> and diagonal matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The diagonal vector of right matrix <c>B</c>.</param>
        /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        [Obsolete("Please use DotWithDiagonal instead.")]
        public static double[,] MultiplyByDiagonal(this double[,] a, double[] b)
        {
            return DotWithDiagonal(a, b);
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
        [Obsolete("Please use DotWithDiagonal instead.")]
        public static void MultiplyByDiagonal(this double[,] a, double[] b, double[,] result)
        {
            DotWithDiagonal(a, b, result);
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
        [Obsolete("Please use DotWithDiagonal instead.")]
        public static void MultiplyByDiagonal(this double[][] a, double[] b, double[][] result)
        {
            DotWithDiagonal(a, b, result);
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
        [Obsolete("Please use DotWithDiagonal instead.")]
        public static void MultiplyByDiagonal(this float[][] a, float[] b, float[][] result)
        {
            DotWithDiagonal(a, b, result);
        }

        /// <summary>
        ///   Computes the product A*B of matrix <c>A</c> and diagonal matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The left matrix <c>A</c>.</param>
        /// <param name="b">The diagonal vector of right matrix <c>B</c>.</param>
        /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
        /// 
        [Obsolete("Please use DotWithDiagonal instead.")]
        public static float[,] MultiplyByDiagonal(this float[,] a, float[] b)
        {
            return DotWithDiagonal(a, b);
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
        [Obsolete("Please use DotWithDiagonal instead.")]
        public static void MultiplyByDiagonal(this float[,] a, float[] b, float[,] result)
        {
            DotWithDiagonal(a, b, result);
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
        [Obsolete("Please use Dot instead.")]
        public static double[] Multiply(this double[] rowVector, double[,] matrix)
        {
            return Dot(rowVector, matrix);
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
        [Obsolete("Please use Dot instead.")]
        public static float[] Multiply(this float[] rowVector, float[,] matrix)
        {
            return Dot(rowVector, matrix);
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
        [Obsolete("Please use Dot instead.")]
        public static double[] Multiply(this double[,] matrix, double[] columnVector)
        {
            return Dot(matrix, columnVector);
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
        [Obsolete("Please use Dot instead.")]
        public static float[] Multiply(this float[][] matrix, float[] columnVector)
        {
            return Dot(matrix, columnVector);
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
        [Obsolete("Please use Dot instead.")]
        public static double[] Multiply(this double[][] matrix, double[] columnVector)
        {
            return Dot(matrix, columnVector);
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
        [Obsolete("Please use Dot instead.")]
        public static float[] Multiply(this float[,] matrix, float[] columnVector)
        {
            return Dot(matrix, columnVector);
        }



        #endregion


        #region Division

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
        [Obsolete("Please use Dot instead.")]
        public static double InnerProduct(this double[] a, double[] b)
        {
            return Dot(a, b);
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
        [Obsolete("Please use Dot instead.")]
        public static float InnerProduct(this float[] a, float[] b)
        {
            return Dot(a, b);
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
        [Obsolete("Please use Outer instead.")]
        public static double[,] OuterProduct(this double[] a, double[] b)
        {
            return Outer(a, b);
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
        [Obsolete("Please use Cross instead.")]
        public static double[] VectorProduct(double[] a, double[] b)
        {
            return Matrix.Cross(a, b);
        }

        /// <summary>
        ///   Vector product.
        /// </summary>
        /// 
        [Obsolete("Please use Cross instead.")]
        public static float[] VectorProduct(float[] a, float[] b)
        {
            return Matrix.Cross(a, b);
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
        [Obsolete("Please use Cartesian instead.")]
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            return Cartesian(sequences);
        }

        /// <summary>
        ///   Computes the Cartesian product of many sets.
        /// </summary>
        /// 
        [Obsolete("Please use Cartesian instead.")]
        public static T[][] CartesianProduct<T>(params T[][] sequences)
        {
            return Cartesian(sequences);
        }

        /// <summary>
        ///   Computes the Cartesian product of two sets.
        /// </summary>
        /// 
        [Obsolete("Please use Cartesian instead.")]
        public static T[][] CartesianProduct<T>(this T[] sequence1, T[] sequence2)
        {
            return Cartesian(new T[][] { sequence1, sequence2 });
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
        [Obsolete("Please use Kronecker instead.")]
        public static double[,] KroneckerProduct(this double[,] a, double[,] b)
        {
            return Kronecker(a, b);
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
        [Obsolete("Please use Kronecker instead.")]
        public static double[] KroneckerProduct(this double[] a, double[] b)
        {
            return Kronecker(a, b);
        }
        #endregion

        #region Addition and Subtraction

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
        [Obsolete("Please use the Elementwise class instead.")]
        public static double[,] Add(double[,] matrix, double[] vector, int dimension)
        {
            return Elementwise.Add(matrix, vector, dimension);
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
        [Obsolete("Please use the Elementwise class instead.")]
        public static double[,] AddToDiagonal(double[,] matrix, double scalar, bool inPlace = false)
        {
			if (inPlace)
				return Elementwise.AddToDiagonal(matrix, scalar, matrix);
			return Elementwise.AddToDiagonal(matrix, scalar);
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
        [Obsolete("Please use the Elementwise class instead.")]
        public static double[][] AddToDiagonal(double[][] matrix, double scalar, bool inPlace = false)
        {
            if (inPlace)
				return Elementwise.AddToDiagonal(matrix, scalar, matrix);
			return Elementwise.AddToDiagonal(matrix, scalar);
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
        [Obsolete("Please use the Elementwise class instead.")]
        public static double[][] SubtractFromDiagonal(double[][] matrix, double scalar, bool inPlace = false)
        {
            if (inPlace)
				return Elementwise.SubtractFromDiagonal(matrix, scalar, matrix);
			return Elementwise.SubtractFromDiagonal(matrix, scalar);
        }

        #endregion



        /// <summary>
        ///   Normalizes a vector to have unit length.
        /// </summary>
        /// 
        /// <param name="vector">A vector.</param>
        /// <param name="norm">A norm to use. Default is <see cref="Norm.Euclidean(double[])"/>.</param>
        /// <param name="inPlace">True to perform the operation in-place,
        ///   overwriting the original array; false to return a new array.</param>
        /// 
        /// <returns>A multiple of vector <c>a</c> where <c>||a|| = 1</c>.</returns>
        /// 
        public static double[] Normalize(this double[] vector, Func<double[], double> norm, bool inPlace = false)
        {
            double[] r = inPlace ? vector : new double[vector.Length];

            double w = norm(vector);

            if (w == 0)
            {
                for (int i = 0; i < vector.Length; i++)
                    r[i] = vector[i];
            }
            else
            {
                for (int i = 0; i < vector.Length; i++)
                    r[i] = vector[i] / w;
            }

            return r;
        }

        /// <summary>
        ///   Normalizes a vector to have unit length.
        /// </summary>
        /// 
        /// <param name="vector">A vector.</param>
        /// <param name="norm">A norm to use. Default is <see cref="Norm.Euclidean(float[])"/>.</param>
        /// <param name="inPlace">True to perform the operation in-place,
        /// overwriting the original array; false to return a new array.</param>
        /// 
        /// <returns>A multiple of vector <c>a</c> where <c>||a|| = 1</c>.</returns>
        /// 
        public static float[] Normalize(this float[] vector, Func<float[], float> norm, bool inPlace = false)
        {
            float[] r = inPlace ? vector : new float[vector.Length];

            double w = norm(vector);

            if (w == 0)
            {
                for (int i = 0; i < vector.Length; i++)
                    r[i] = vector[i];
            }
            else
            {
                for (int i = 0; i < vector.Length; i++)
                    r[i] = (float)(vector[i] / w);
            }

            return r;
        }

        /// <summary>
        ///   Normalizes a vector to have unit length.
        /// </summary>
        /// 
        /// <param name="vector">A vector.</param>
        /// <param name="inPlace">True to perform the operation in-place,
        /// overwriting the original array; false to return a new array.</param>
        /// 
        /// <returns>A multiple of vector <c>a</c> where <c>||a|| = 1</c>.</returns>
        /// 
        public static double[] Normalize(this double[] vector, bool inPlace = false)
        {
            return Normalize(vector, Norm.Euclidean, inPlace);
        }

        /// <summary>
        ///   Normalizes a vector to have unit length.
        /// </summary>
        /// 
        /// <param name="vector">A vector.</param>
        /// <param name="inPlace">True to perform the operation in-place,
        /// overwriting the original array; false to return a new array.</param>
        /// 
        /// <returns>A multiple of vector <c>a</c> where <c>||a|| = 1</c>.</returns>
        /// 
        public static float[] Normalize(this float[] vector, bool inPlace = false)
        {
            return Normalize(vector, Norm.Euclidean, inPlace);
        }

        /// <summary>
        ///   Multiplies a matrix by itself <c>n</c> times.
        /// </summary>
        /// 
        public static double[,] Power(this double[,] matrix, int n)
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
                result = Matrix.Dot(result, result);

                if (bin[i] == '1')
                    result = Matrix.Dot(result, matrix);
            }

            return result;
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
        public static IEnumerable<IEnumerable<T>> Cartesian<T>(this IEnumerable<IEnumerable<T>> sequences)
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
        public static T[][] Cartesian<T>(params T[][] sequences)
        {
            var result = Cartesian(sequences as IEnumerable<IEnumerable<T>>);

            var list = new List<T[]>();
            foreach (IEnumerable<T> point in result)
                list.Add(point.ToArray());

            return list.ToArray();
        }

        /// <summary>
        ///   Computes the Cartesian product of two sets.
        /// </summary>
        /// 
        public static T[][] Cartesian<T>(this T[] sequence1, T[] sequence2)
        {
            return Cartesian(new T[][] { sequence1, sequence2 });
        }

    }
}
