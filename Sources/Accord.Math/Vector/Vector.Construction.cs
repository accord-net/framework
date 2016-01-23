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
    using Accord.Math.Comparers;
    using System;
    using System.Collections.Generic;


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
        public static T[] Ones<T>(int size) where T : struct
        {
            var one = (T)Convert.ChangeType(1, typeof(T));
            return Create(size, one);
        }

        /// <summary>
        ///   Creates a zero-valued vector.
        /// </summary>
        /// 
        /// <param name="size">The number of elements in the vector.</param>
        /// 
        /// <returns>A vector of the specified size.</returns>
        /// 
        public static double[] Zeros(int size)
        {
            return Zeros<double>(size);
        }

        /// <summary>
        ///   Creates a zero-valued vector.
        /// </summary>
        /// 
        /// <param name="size">The number of elements in the vector.</param>
        /// 
        /// <returns>A vector of the specified size.</returns>
        /// 
        public static double[] Ones(int size)
        {
            return Ones<double>(size);
        }

        /// <summary>
        ///   Creates a vector with the given dimension and starting value.
        /// </summary>
        /// 
        /// <param name="size">The number of elements in the vector.</param>
        /// <param name="value">The initial values for the vector.</param>
        /// 
        public static T[] Create<T>(int size, T value)
        {
            var v = new T[size];
            for (int i = 0; i < v.Length; i++)
                v[i] = value;
            return v;
        }

        /// <summary>
        ///   Creates a vector with the given starting values.
        /// </summary>
        /// 
        /// <param name="values">The initial values for the vector.</param>
        /// 
        public static T[] Create<T>(params T[] values)
        {
            return (T[])values.Clone();
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
            return Histogram(labels, new int[labels.Max()]);
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

    }
}
