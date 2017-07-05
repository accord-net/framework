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
    using Accord.Math.Comparers;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;


    public static partial class Vector
    {

        /// <summary>
        ///   Creates a zero-valued vector.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the vector to be created.</typeparam>
        /// <param name="size">The number of elements in the vector.</param>
        /// 
        /// <returns>A vector of the specified size.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[] Zeros<T>(int size)
        {
            return new T[size];
        }

        /// <summary>
        ///   Creates a zero-valued vector.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the vector to be created.</typeparam>
        /// <param name="size">The number of elements in the vector.</param>
        /// 
        /// <returns>A vector of the specified size.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[] Ones<T>(int size) where T : struct
        {
            var one = (T)Convert.ChangeType(1, typeof(T));
            return Create(size, one);
        }

        /// <summary>
        ///   Creates a vector with ones at the positions where
        ///   <paramref name="mask"/> is true, and zero when they
        ///   are false.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the vector to be created.</typeparam>
        /// 
        /// <param name="mask">The boolean mask determining where ones will be placed.</param>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[] Ones<T>(bool[] mask) where T : struct
        {
            var one = (T)Convert.ChangeType(1, typeof(T));
            return Create(one, mask);
        }

        /// <summary>
        ///   Creates a zero-valued vector.
        /// </summary>
        /// 
        /// <param name="size">The number of elements in the vector.</param>
        /// 
        /// <returns>A vector of the specified size.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] Zeros(int size)
        {
            return Zeros<double>(size);
        }

        /// <summary>
        ///   Creates a one-valued vector.
        /// </summary>
        /// 
        /// <param name="size">The number of elements in the vector.</param>
        /// 
        /// <returns>A vector of the specified size.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] Ones(int size)
        {
            return Ones<double>(size);
        }

        /// <summary>
        ///   Creates a vector with ones at the positions where
        ///   <paramref name="mask"/> is true, and zero when they
        ///   are false.
        /// </summary>
        /// 
        /// <param name="mask">The boolean mask determining where ones will be placed.</param>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] Ones(bool[] mask)
        {
            return Ones<double>(mask);
        }

        /// <summary>
        ///   Creates a vector with the given dimension and starting value.
        /// </summary>
        /// 
        /// <param name="size">The number of elements in the vector.</param>
        /// <param name="value">The initial values for the vector.</param>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[] Create<T>(int size, T value)
        {
            var v = new T[size];
            for (int i = 0; i < v.Length; i++)
                v[i] = value;
            return v;
        }

        /// <summary>
        ///   Creates a vector with the given dimension and starting value.
        /// </summary>
        /// 
        /// <param name="size">The number of elements in the vector.</param>
        /// <param name="values">The initial values for the vector.</param>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[] Create<T>(int size, T[] values)
        {
            var v = new T[size];
            if (values != null)
            {
                for (int i = 0; i < Math.Min(values.Length, size); i++)
                    v[i] = values[i];
            }
            return v;
        }

        /// <summary>
        ///   Creates a vector with the given value at the positions where
        ///   <paramref name="mask"/> is true, and zero when they are false.
        /// </summary>
        /// 
        /// <param name="value">The initial values for the vector.</param>
        /// <param name="mask">The boolean mask determining where the values will be placed.</param>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[] Create<T>(T value, bool[] mask)
        {
            var v = new T[mask.Length];
            for (int i = 0; i < v.Length; i++)
                if (mask[i])
                    v[i] = value;
            return v;
        }

        /// <summary>
        ///   Creates a vector with the given starting values.
        /// </summary>
        /// 
        /// <param name="values">The initial values for the vector.</param>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[] Create<T>(params T[] values)
        {
            return (T[])values.Clone();
        }

        /// <summary>
        ///   Creates a vector with the shape of the given vector.
        /// </summary>
        /// 
        /// <param name="vector">The vector whose shape should be copied.</param>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[] CreateAs<T>(T[] vector)
        {
            return new T[vector.Length];
        }

        /// <summary>
        ///   Creates a one-hot vector, where all values are zero except for the indicated
        ///   <paramref name="index"/>, which is set to one.
        /// </summary>
        /// 
        /// <typeparam name="T">The data type for the vector.</typeparam>
        /// 
        /// <param name="index">The vector's dimension which will be marked as one.</param>
        /// <param name="columns">The size (length) of the vector.</param>
        /// 
        /// <returns>A one-hot vector where only a single position is one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[] OneHot<T>(int index, int columns)
        {
            return OneHot<T>(index, new T[columns]);
        }

        /// <summary>
        ///   Creates a vector with the given value at the positions where
        ///   <paramref name="mask"/> is true, and zero when they are false.
        /// </summary>
        /// 
        /// <param name="mask">The boolean mask determining where the values will be placed.</param>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[] OneHot<T>(bool mask)
        {
            return OneHot<T>(mask, new T[2]);
        }

        /// <summary>
        ///   Creates a vector with the given value at the positions where
        ///   <paramref name="mask"/> is true, and zero when they are false.
        /// </summary>
        /// 
        /// <param name="mask">The boolean mask determining where the values will be placed.</param>
        /// <param name="result">The vector where the one-hot should be marked.</param>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[] OneHot<T>(bool mask, T[] result)
        {
            return OneHot<T>(mask ? 0 : 1, result);
        }

        /// <summary>
        ///   Creates a one-hot vector, where all values are zero except for the indicated
        ///   <paramref name="index"/>, which is set to one.
        /// </summary>
        /// 
        /// <param name="index">The vector's dimension which will be marked as one.</param>
        /// <param name="columns">The size (length) of the vector.</param>
        /// 
        /// <returns>A one-hot vector where only a single position is one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] OneHot(int index, int columns)
        {
            return OneHot(index, new double[columns]);
        }

        /// <summary>
        ///   Creates a one-hot vector, where all values are zero except for the indicated
        ///   <paramref name="index"/>, which is set to one.
        /// </summary>
        /// 
        /// <typeparam name="T">The data type for the vector.</typeparam>
        /// 
        /// <param name="index">The vector's dimension which will be marked as one.</param>
        /// <param name="result">The vector where the one-hot should be marked.</param>
        /// 
        /// <returns>A one-hot vector where only a single position is one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[] OneHot<T>(int index, T[] result)
        {
            var one = (T)System.Convert.ChangeType(1, typeof(T));
            result[index] = one;
            return result;
        }

        /// <summary>
        ///   Creates a one-hot vector, where all values are zero except for the indicated
        ///   <paramref name="index"/>, which is set to one.
        /// </summary>
        /// 
        /// <param name="index">The vector's dimension which will be marked as one.</param>
        /// <param name="result">The vector where the one-hot should be marked.</param>
        /// 
        /// <returns>A one-hot vector where only a single position is one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] OneHot(int index, double[] result)
        {
            result[index] = 1;
            return result;
        }



        /// <summary>
        ///   Creates a k-hot vector, where all values are zero except for the elements
        ///   at the indicated <paramref name="indices"/>, which are set to one.
        /// </summary>
        /// 
        /// <param name="indices">The vector's dimensions which will be marked as ones.</param>
        /// <param name="columns">The size (length) of the vector.</param>
        /// 
        /// <returns>A k-hot vector where the indicated positions are one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[] KHot<T>(int[] indices, int columns)
        {
            return KHot<T>(indices, new T[columns]);
        }

        /// <summary>
        ///   Creates a k-hot vector, where all values are zero except for the elements
        ///   at the positions where <paramref name="mask"/> is true, which are set to one.
        /// </summary>
        /// 
        /// <param name="mask">The boolean mask determining where the values will be placed.</param>
        /// 
        /// <returns>A k-hot vector where the indicated positions are one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[] KHot<T>(bool[] mask)
        {
            return KHot<T>(mask);
        }


        /// <summary>
        ///   Creates a k-hot vector, where all values are zero except for the elements
        ///   at the indicated <paramref name="indices"/>, which are set to one.
        /// </summary>
        /// 
        /// <param name="indices">The vector's dimensions which will be marked as ones.</param>
        /// <param name="columns">The size (length) of the vector.</param>
        /// 
        /// <returns>A k-hot vector where the indicated positions are one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] KHot(int[] indices, int columns)
        {
            return KHot(indices, new double[columns]);
        }

        /// <summary>
        ///   Creates a k-hot vector, where all values are zero except for the elements
        ///   at the indicated <paramref name="indices"/>, which are set to one.
        /// </summary>
        /// 
        /// <param name="indices">The vector's dimensions which will be marked as ones.</param>
        /// <param name="result">The vector where the k-hot should be marked.</param>
        /// 
        /// <returns>A k-hot vector where the indicated positions are one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[] KHot<T>(int[] indices, T[] result)
        {
            var one = (T)System.Convert.ChangeType(1, typeof(T));
            for (int i = 0; i < indices.Length; i++)
                result[indices[i]] = one;
            return result;
        }

        /// <summary>
        ///   Creates a k-hot vector, where all values are zero except for the elements
        ///   at the positions where <paramref name="mask"/> is true, which are set to one.
        /// </summary>
        /// 
        /// <param name="mask">The boolean mask determining where the values will be placed.</param>
        /// <param name="result">The vector where the k-hot should be marked.</param>
        ///
        /// <returns>A k-hot vector where the indicated positions are one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[] KHot<T>(bool[] mask, T[] result)
        {
            var one = (T)System.Convert.ChangeType(1, typeof(T));
            for (int i = 0; i < mask.Length; i++)
            {
                if (mask[i])
                    result[i] = one;
            }
            return result;
        }

        /// <summary>
        ///   Creates a k-hot vector, where all values are zero except for the elements
        ///   at the indicated <paramref name="indices"/>, which are set to one.
        /// </summary>
        /// 
        /// <param name="indices">The vector's dimensions which will be marked as ones.</param>
        /// <param name="result">The vector where the k-hot should be marked.</param>
        /// 
        /// <returns>A k-hot vector where the indicated positions are one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] KHot(int[] indices, double[] result)
        {
            for (int i = 0; i < indices.Length; i++)
                result[indices[i]] = 1;
            return result;
        }


        /// <summary>
        ///   Counts how many times an integer label appears in a vector (i.e. creates 
        ///   an histogram of integer values assuming possible values start at zero and
        ///   go up to the maximum value of in the vector).
        /// </summary>
        /// 
        /// <param name="labels">An array containing the integer labels to be counted.</param>
        /// 
        /// <returns>
        ///   An integer array of size corresponding to the maximum label in the vector
        ///   <paramref name="labels"/>, containing how many times each possible label 
        ///   appears in <paramref name="labels"/>.
        /// </returns>
        /// 
        public static int[] Histogram(this int[] labels)
        {
            return Histogram(labels, new int[labels.Max() + 1]);
        }

        /// <summary>
        ///   Counts how many times an integer label appears in a vector (i.e. creates 
        ///   an histogram of integer values assuming possible values start at zero and
        ///   go up to the value of <paramref name="size"/>).
        /// </summary>
        /// 
        /// <param name="labels">An array containing the integer labels to be counted.</param>
        /// <param name="size">The number of labels (will be the size of the generated histogram).</param>
        /// 
        /// <returns>
        ///   An integer array of size <paramref name="size"/> containing how many 
        ///   times each possible label appears in <paramref name="labels"/>.
        /// </returns>
        /// 
        public static int[] Histogram(this int[] labels, int size)
        {
            return Histogram(labels, new int[size]);
        }

        /// <summary>
        ///   Counts how many times an integer label appears in a vector (i.e. creates 
        ///   an histogram of integer values assuming possible values start at zero and
        ///   go up to the maximum value of in the vector).
        /// </summary>
        /// 
        /// <param name="labels">An array containing the integer labels to be counted.</param>
        /// <param name="result">The histogram to were the counts will be added. This
        ///   vector should have been zeroed out before being passed to this method.</param>
        /// 
        /// <returns>
        ///   The same vector <paramref name="result"/> passed as an argument.
        /// </returns>
        /// 
        public static int[] Histogram(this int[] labels, int[] result)
        {
            for (int i = 0; i < labels.Length; i++)
                result[labels[i]]++;
            return result;
        }

        /// <summary>
        ///   Creates a shallow copy of the array.
        /// </summary>
        /// 
        public static T[] Copy<T>(this T[] vector)
        {
            return (T[])vector.Clone();
        }
    }
}
