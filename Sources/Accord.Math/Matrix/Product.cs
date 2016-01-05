// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
   ///   Vectorial and matrix products.
   /// </summary>
   /// 
    public static partial class Product
    {

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
        public static double Inner(this double[] a, double[] b)
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
        public static float Inner(this float[] a, float[] b)
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
        public static double[,] Outer(this double[] a, double[] b)
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
        public static double[] Cross(double[] a, double[] b)
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
        public static float[] Cross(float[] a, float[] b)
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

            List<T[]> list = new List<T[]>();
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

        /// <summary>
        ///   Computes the Kronecker product between two matrices.
        /// </summary>
        /// 
        /// <param name="a">The left matrix a.</param>
        /// <param name="b">The right matrix b.</param>
        /// 
        /// <returns>The Kronecker product of the two matrices.</returns>
        /// 
        public static double[,] Kronecker(this double[,] a, double[,] b)
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

            unsafe
            {
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
        public static double[] Kronecker(this double[] a, double[] b)
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

    }
}
