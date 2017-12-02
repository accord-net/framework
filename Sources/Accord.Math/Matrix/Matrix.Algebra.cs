// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
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
        ///   Obsolete. Please use the <see cref="Dot(Double[,], Double[,])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Dot(a, b) method instead.")]
        public static double[,] Multiply(double[,] a, double[,] b)
        {
            return Dot(a, b);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Dot(Double[,], Double[,])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Dot(a, b) method instead.")]
        public static double[][] Multiply(double[][] a, double[][] b)
        {
            return Dot(a, b);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Dot(Double[,], Double[,])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Dot(a, b) method instead.")]
        public static float[][] Multiply(float[][] a, float[][] b)
        {
            return Dot(a, b);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Dot(Double[,], Double[,])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Dot(a, b) method instead.")]
        public static double[][] Multiply(float[][] a, double[][] b)
        {
            return Dot(a, b);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Dot(Double[,], Double[,])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Dot(a, b) method instead.")]
        public static float[,] Multiply(float[,] a, float[,] b)
        {
            return Dot(a, b);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Dot(Double[,], Double[,])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Dot(a, b) method instead.")]
        public static void Multiply(double[,] a, double[,] b, double[,] result)
        {
            Dot(a, b, result);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Dot(Double[,], Double[,])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Dot(a, b) method instead.")]
        public static void Multiply(double[][] a, double[][] b, double[][] result)
        {
            Dot(a, b, result);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Dot(Double[,], Double[,])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Dot(a, b) method instead.")]
        public static void Multiply(double[,] a, double[,] b, double[][] result)
        {
            Dot(a, b).CopyTo(result);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Dot(Double[,], Double[,])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Dot(a, b) method instead.")]
        public static void Multiply(float[][] a, float[][] b, float[][] result)
        {
            Dot(a, b, result);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Dot(Double[,], Double[,])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Dot(a, b) method instead.")]
        public static void Multiply(float[][] a, double[][] b, double[][] result)
        {
            Dot(a, b, result);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Dot(Double[,], Double[,])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Dot(a, b) method instead.")]
        public static void Multiply(float[,] a, float[,] b, float[,] result)
        {
            Dot(a, b, result);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="DotWithTransposed(Double[,], Double[,])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the DotWithTranspose(a, b) method instead.")]
        public static double[,] MultiplyByTranspose(this double[,] a, double[,] b)
        {
            return DotWithTransposed(a, b);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="DotWithTransposed(Double[,], Double[,])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the DotWithTranspose(a, b) method instead.")]
        public static float[,] MultiplyByTranspose(this float[,] a, float[,] b)
        {
            return DotWithTransposed(a, b);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="DotWithTransposed(Double[,], Double[,])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the DotWithTranspose(a, b) method instead.")]
        public static void MultiplyByTranspose(this double[,] a, double[,] b, double[,] result)
        {
            DotWithTransposed(a, b, result);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="DotWithTransposed(Double[,], Double[,])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the DotWithTranspose(a, b) method instead.")]
        public static void MultiplyByTranspose(this float[,] a, float[,] b, float[,] result)
        {
            DotWithTransposed(a, b, result);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="TransposeAndDot(Double[,], Double[,])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the TransposeAndDot(a, b) method instead.")]
        public static double[,] TransposeAndMultiply(this double[,] a, double[,] b)
        {
            return TransposeAndDot(a, b);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="TransposeAndDot(Double[,], Double[,])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the TransposeAndDot(a, b) method instead.")]
        public static double[][] TransposeAndMultiply(this double[][] a, double[][] b)
        {
            return TransposeAndDot(a, b);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="TransposeAndDot(Double[,], Double[,])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the TransposeAndDot(a, b) method instead.")]
        public static void TransposeAndMultiply(this double[,] a, double[,] b, double[,] result)
        {
            TransposeAndDot(a, b, result);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="TransposeAndDot(Double[,], Double[,])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the TransposeAndDot(a, b) method instead.")]
        public static void TransposeAndMultiply(this double[][] a, double[][] b, double[][] result)
        {
            TransposeAndDot(a, b, result);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="TransposeAndDot(Double[,], Double[,])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the TransposeAndDot(a, b) method instead.")]
        public static double[] TransposeAndMultiply(this double[,] a, double[] b)
        {
            return TransposeAndDot(a, b);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="TransposeAndDot(Double[,], Double[,])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the TransposeAndDot(a, b) method instead.")]
        public static void TransposeAndMultiply(this double[,] a, double[] b, double[] result)
        {
            TransposeAndDot(a, b, result);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="TransposeAndDotWithDiagonal(Double[,], Double[])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the TransposeAndDotWithDiagonal(a, b) method instead.")]
        public static double[,] TransposeAndMultiplyByDiagonal(this double[,] a, double[] b)
        {
            return TransposeAndDotWithDiagonal(a, b);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="TransposeAndDotWithDiagonal(Double[,], Double[])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the TransposeAndDotWithDiagonal(a, b) method instead.")]
        public static void TransposeAndMultiplyByDiagonal(this double[,] a, double[] b, double[,] result)
        {
            TransposeAndDotWithDiagonal(a, b, result);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="DotWithDiagonal(Double[,], Double[])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the DotWithDiagonal(a, b) method instead.")]
        public static double[][] MultiplyByDiagonal(this double[][] a, double[] b)
        {
            return DotWithDiagonal(a, b);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="DotWithDiagonal(Double[,], Double[])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the DotWithDiagonal(a, b) method instead.")]
        public static float[][] MultiplyByDiagonal(this float[][] a, float[] b)
        {
            return DotWithDiagonal(a, b);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="DotWithDiagonal(Double[,], Double[])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the DotWithDiagonal(a, b) method instead.")]
        public static double[,] MultiplyByDiagonal(this double[,] a, double[] b)
        {
            return DotWithDiagonal(a, b);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="DotWithDiagonal(Double[,], Double[])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the DotWithDiagonal(a, b) method instead.")]
        public static void MultiplyByDiagonal(this double[,] a, double[] b, double[,] result)
        {
            DotWithDiagonal(a, b, result);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="DotWithDiagonal(Double[,], Double[])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the DotWithDiagonal(a, b) method instead.")]
        public static void MultiplyByDiagonal(this double[][] a, double[] b, double[][] result)
        {
            DotWithDiagonal(a, b, result);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="DotWithDiagonal(Double[,], Double[])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the DotWithDiagonal(a, b) method instead.")]
        public static void MultiplyByDiagonal(this float[][] a, float[] b, float[][] result)
        {
            DotWithDiagonal(a, b, result);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="DotWithDiagonal(Double[,], Double[])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the DotWithDiagonal(a, b) method instead.")]
        public static float[,] MultiplyByDiagonal(this float[,] a, float[] b)
        {
            return DotWithDiagonal(a, b);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="DotWithDiagonal(Double[,], Double[])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the DotWithDiagonal(a, b) method instead.")]
        public static void MultiplyByDiagonal(this float[,] a, float[] b, float[,] result)
        {
            DotWithDiagonal(a, b, result);
        }

        #endregion


        #region Matrix-Vector multiplication

        /// <summary>
        ///   Obsolete. Please use the <see cref="Dot(Double[], Double[,])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Dot(a, b) method instead.")]
        public static double[] Multiply(this double[] rowVector, double[,] matrix)
        {
            return Dot(rowVector, matrix);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Dot(Double[], Double[,])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Dot(a, b) method instead.")]
        public static float[] Multiply(this float[] rowVector, float[,] matrix)
        {
            return Dot(rowVector, matrix);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Dot(Double[,], Double[])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Dot(a, b) method instead.")]
        public static double[] Multiply(this double[,] matrix, double[] columnVector)
        {
            return Dot(matrix, columnVector);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Dot(Double[,], Double[])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Dot(a, b) method instead.")]
        public static float[] Multiply(this float[][] matrix, float[] columnVector)
        {
            return Dot(matrix, columnVector);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Dot(Double[,], Double[])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Dot(a, b) method instead.")]
        public static double[] Multiply(this double[][] matrix, double[] columnVector)
        {
            return Dot(matrix, columnVector);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Dot(Double[,], Double[])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Dot(a, b) method instead.")]
        public static float[] Multiply(this float[,] matrix, float[] columnVector)
        {
            return Dot(matrix, columnVector);
        }



        #endregion




        #region Products
        /// <summary>
        ///   Obsolete. Please use the <see cref="Dot(Double[], Double[])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Dot(a, b) method instead.")]
        public static double InnerProduct(this double[] a, double[] b)
        {
            return Dot(a, b);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Dot(Double[], Double[])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Dot(a, b) method instead.")]
        public static float InnerProduct(this float[] a, float[] b)
        {
            return Dot(a, b);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Outer(Double[], Double[])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Outer(a, b) method instead.")]
        public static double[,] OuterProduct(this double[] a, double[] b)
        {
            return Outer(a, b);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Cross(Double[], Double[])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Cross(a, b) method instead.")]
        public static double[] VectorProduct(double[] a, double[] b)
        {
            return Matrix.Cross(a, b);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Cross(Double[], Double[])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Cross(a, b) method instead.")]
        public static float[] VectorProduct(float[] a, float[] b)
        {
            return Matrix.Cross(a, b);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Cartesian{T}(IEnumerable{IEnumerable{T}})"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Cartesian(sequences) method instead.")]
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            return Cartesian(sequences);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Cartesian{T}(T[][])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Cartesian(sequences) method instead.")]
        public static T[][] CartesianProduct<T>(params T[][] sequences)
        {
            return Cartesian(sequences);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Cartesian{T}(T[], T[])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Cartesian(a, b) method instead.")]
        public static T[][] CartesianProduct<T>(this T[] sequence1, T[] sequence2)
        {
            return Cartesian(new T[][] { sequence1, sequence2 });
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Kronecker(Double[,], Double[,])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Konecker(a, b) method instead.")]
        public static double[,] KroneckerProduct(this double[,] a, double[,] b)
        {
            return Kronecker(a, b);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Kronecker(Double[], Double[])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Konecker(a, b) method instead.")]
        public static double[] KroneckerProduct(this double[] a, double[] b)
        {
            return Kronecker(a, b);
        }
        #endregion

        #region Addition and Subtraction

        /// <summary>
        ///   Obsolete. Please use the <see cref="Elementwise.Add(Double[,], Double[], int)"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Elementwise.Add(a, b) method instead.")]
        public static double[,] Add(double[,] matrix, double[] vector, int dimension)
        {
            return Elementwise.Add(matrix, vector, dimension);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Elementwise.AddToDiagonal(Double[,], Double)"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Elementwise.AddToDiagonal(a, b) method instead.")]
        public static double[,] AddToDiagonal(double[,] matrix, double scalar, bool inPlace = false)
        {
            if (inPlace)
                return Elementwise.AddToDiagonal(matrix, scalar, matrix);
            return Elementwise.AddToDiagonal(matrix, scalar);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Elementwise.AddToDiagonal(Double[,], Double)"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Elementwise.AddToDiagonal(a, b) method instead.")]
        public static double[][] AddToDiagonal(double[][] matrix, double scalar, bool inPlace = false)
        {
            if (inPlace)
                return Elementwise.AddToDiagonal(matrix, scalar, matrix);
            return Elementwise.AddToDiagonal(matrix, scalar);
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="Elementwise.SubtractFromDiagonal(Double[,], Double[])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Elementwise.SubtractFromDiagonal(a, b) method instead.")]
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
