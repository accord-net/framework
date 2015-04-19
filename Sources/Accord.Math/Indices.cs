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

    /// <summary>
    ///   Static extension class for manipulating index vectors (i.e. vectors
    ///   containing integers that represent positions within a collection or
    ///   array.
    /// </summary>
    /// 
    // TODO: Make it public and obsolete the previous equivalent methods
    internal static class Indices
    {

        /// <summary>
        ///   Returns a vector of the specified <paramref name="size"/> containing 
        ///   indices (0, 1, 2, ... max) up to a given maximum number and in random 
        ///   order. The vector can grow up to to <paramref name="max"/>, but does 
        ///   not include <c>max</c> among its values.
        /// </summary>
        /// 
        /// <remarks>
        ///   In other words, this return a sample of size <c>k</c> from a population
        ///   of size <c>n</c>, where <c>k</c> is the parameter <paramref name="size"/>
        ///   and <c>n</c> is the parameter <paramref name="max"/>.
        /// </remarks>
        /// 
        /// <example>
        /// <code>
        ///   var a = Indices.Random(3, 10);  // a possible output is { 1, 7, 4 };
        ///   var b = Indices.Random(10, 10); // a possible output is { 5, 4, 2, 0, 1, 3, 7, 9, 8, 6 };
        ///   
        ///   foreach (var i in Indices.Random(5, 6))
        ///   {
        ///      // ...
        ///   }
        /// </code>
        /// </example>
        /// 
        public static int[] Random(int size, int max)
        {
            return Random<int>(size, max);
        }

        /// <summary>
        ///   Returns a vector containing indices (0, 1, 2, ..., n - 1) in random 
        ///   order. The vector grows up to to <paramref name="size"/>, but does not
        ///   include <c>size</c> among its values.
        /// </summary>
        /// 
        /// <example>
        /// <code>
        ///   var a = Indices.Random(3);  // a possible output is { 2, 1, 0 };
        ///   var b = Indices.Random(10); // a possible output is { 5, 4, 2, 0, 1, 3, 7, 9, 8, 6 };
        ///   
        ///   foreach (var i in Indices.Random(5))
        ///   {
        ///      // ...
        ///   }
        /// </code>
        /// </example>
        /// 
        public static int[] Random(int size)
        {
            return Random<int>(size);
        }

        /// <summary>
        ///   Returns a vector of the specified <paramref name="size"/> containing 
        ///   indices (0, 1, 2, ... max) up to a given maximum number and in random 
        ///   order. The vector can grow up to to <paramref name="max"/>, but does 
        ///   not include <c>max</c> among its values.
        /// </summary>
        /// 
        /// <remarks>
        ///   In other words, this return a sample of size <c>k</c> from a population
        ///   of size <c>n</c>, where <c>k</c> is the parameter <paramref name="size"/>
        ///   and <c>n</c> is the parameter <paramref name="max"/>.
        /// </remarks>
        /// 
        /// <example>
        /// <code>
        ///   var a = Indices.Random(3, 10);  // a possible output is { 1, 7, 4 };
        ///   var b = Indices.Random(10, 10); // a possible output is { 5, 4, 2, 0, 1, 3, 7, 9, 8, 6 };
        ///   
        ///   foreach (var i in Indices.Random(5, 6))
        ///   {
        ///      // ...
        ///   }
        /// </code>
        /// </example>
        /// 
        public static T[] Random<T>(int size, int max)
        {
            if (size > max)
            {
                throw new ArgumentOutOfRangeException("size",
                    "The sample size must be less than the size of the population.");
            }

            T[] idx = Random<T>(max);
            return idx.Submatrix(size);
        }

        /// <summary>
        ///   Returns a vector containing indices (0, 1, 2, ..., n - 1) in random 
        ///   order. The vector grows up to to <paramref name="size"/>, but does not
        ///   include <c>size</c> among its values.
        /// </summary>
        /// 
        /// <example>
        /// <code>
        ///   var a = Indices.Random(3);  // a possible output is { 2, 1, 0 };
        ///   var b = Indices.Random(10); // a possible output is { 5, 4, 2, 0, 1, 3, 7, 9, 8, 6 };
        ///   
        ///   foreach (var i in Indices.Random(5))
        ///   {
        ///      // ...
        ///   }
        /// </code>
        /// </example>
        /// 
        public static T[] Random<T>(int size)
        {
            var random = Accord.Math.Random.Generator.Random;

            T[] idx = new T[size];
            for (int i = 0; i < idx.Length; i++)
                idx[i] = cast<T>(i);

            double[] x = new double[size];
            for (int i = 0; i < x.Length; i++)
                x[i] = random.NextDouble();

            Array.Sort(x, idx);

            return idx;
        }



        /// <summary>
        ///   Creates a vector containing every index up to <paramref name="n"/>
        ///   such as { 0, 1, 2, 3, 4, ..., n-1 }.
        /// </summary>
        /// 
        /// <param name="n">The non-inclusive limit for the index generation.</param>
        /// 
        /// <returns>
        ///   A vector of size <paramref name="n"/> containing
        ///   all vector indices up to <paramref name="n"/>.
        /// </returns>
        /// 
        public static int[] Until(int n)
        {
            return Until<int>(n);
        }

        /// <summary>
        ///   Creates a vector containing every index up to <paramref name="n"/>
        ///   such as { 0.0, 1.0, 2.0, 3.0, 4.0, ..., n-1 } using any choice of 
        ///   numbers, such as byte or double.
        /// </summary>
        /// 
        /// <param name="n">The non-inclusive limit for the index generation.</param>
        /// 
        /// <returns>
        ///   A vector of size <paramref name="n"/> containing
        ///   all vector indices up to <paramref name="n"/>.
        /// </returns>
        /// 
        public static T[] Until<T>(int n)
        {
            return Range<T>(0, n);
        }


        /// <summary>
        ///   Creates a vector containing every index that can be used to
        ///   address a given <paramref name="array"/>, in order. 
        /// </summary>
        /// 
        /// <param name="array">The array whose indices will be returned.</param>
        /// 
        /// <returns>
        ///   A vector of the same size as the given <paramref name="array"/> 
        ///   containing all vector indices from 0 up to the length of 
        ///   <paramref name="array"/>.
        /// </returns>
        /// 
        /// <example>
        /// <code>
        ///   double[] a = { 5.3, 2.3, 4.2 };
        ///   int[] idx = Indices.From(a); // output will be { 0, 1, 2 }
        /// </code>
        /// </example>
        /// 
        public static int[] From<T>(T[] array)
        {
            return Range<int>(0, array.Length);
        }

        /// <summary>
        ///   Creates a vector containing every index that can be used to
        ///   address a given <paramref name="array"/>, in order, using any
        ///   choice of numbers, such as byte or double.
        /// </summary>
        /// 
        /// <param name="array">The array whose indices will be returned.</param>
        /// 
        /// <returns>
        ///   A vector of the same size as the given <paramref name="array"/> 
        ///   containing all vector indices from 0 up to the length of 
        ///   <paramref name="array"/>.
        /// </returns>
        /// 
        /// <example>
        /// <code>
        ///   double[] a = { 5.3, 2.3, 4.2 };
        ///   int[] idx = Indices.From(a); // output will be { 0, 1, 2 }
        /// </code>
        /// </example>
        /// 
        public static T[] From<T, U>(U[] array)
        {
            return Range<T>(0, array.Length);
        }

        /// <summary>
        ///   Creates a vector containing every index that can be used to
        ///   address a given <paramref name="array"/>, in order.
        /// </summary>
        /// 
        /// <param name="array">The array whose indices will be returned.</param>
        /// 
        /// <returns>
        ///   An enumerable object that can be used to iterate over all
        ///   positions of the given <paramref name="array">System.Array</paramref>.
        /// </returns>
        /// 
        /// <example>
        /// <code>
        ///   double[,] a = 
        ///   { 
        ///      { 5.3, 2.3 },
        ///      { 4.2, 9.2 }
        ///   };
        ///   
        ///   foreach (int[] idx in Indices.From(a))
        ///   {
        ///      // Get the current element
        ///      double e = (double)a.GetValue(idx);
        ///   }
        /// </code>
        /// </example>
        /// 
        public static IEnumerable<int[]> From(this Array array)
        {
            int[] dimensions = array.GetDimensions();
            return Combinatorics.Sequences(dimensions, inPlace: true);
        }



        /// <summary>
        ///   Creates a vector containing every index starting at <paramref name="from"/>
        ///   up to <paramref name="to"/> such as { from, from + 1, from + 2, ..., to-1 }.
        /// </summary>
        /// 
        /// <param name="from">The inclusive start for the index generation.</param>
        /// <param name="to">The non-inclusive limit for the index generation.</param>
        /// 
        /// <returns>
        ///   A vector of size <c>to - from</c> containing all vector 
        ///   indices starting at <paramref name="from"/> and going up
        ///   to (but not including) <paramref name="to"/>.
        /// </returns>
        /// 
        public static int[] Range(int from, int to)
        {
            return Range<int>(from, to);
        }

        /// <summary>
        ///   Creates a vector containing every index starting at <paramref name="from"/>
        ///   up to <paramref name="to"/> such as { from, from + 1, from + 2, ..., to-1 }
        ///   using any choice of numbers, such as byte or double.
        /// </summary>
        /// 
        /// <param name="from">The inclusive start for the index generation.</param>
        /// <param name="to">The non-inclusive limit for the index generation.</param>
        /// 
        /// <returns>
        ///   A vector of size <c>to - from</c> containing all vector 
        ///   indices starting at <paramref name="from"/> and going up
        ///   to (but not including) <paramref name="to"/>.
        /// </returns>
        /// 
        public static T[] Range<T>(int from, int to)
        {
            T[] vector;

            if (to > from)
            {
                vector = new T[to - from];
                for (int i = 0; i < vector.Length; i++)
                    vector[i] = cast<T>(from++);
            }
            else
            {
                vector = new T[from - to];
                for (int i = 0; i < vector.Length; i++)
                    vector[i] = cast<T>(from-- - 1);
            }

            return vector;
        }



        static T cast<T>(this object value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

    }
}
