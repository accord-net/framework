using Accord.Math.Comparers;
using System;
using System.Collections.Generic;
using System.Linq;
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
    internal static class Vector
    {
        /// <summary>
        ///   Shuffles an array.
        /// </summary>
        /// 
        public static void Shuffle<T>(this T[] array)
        {
            var random = Accord.Math.Tools.Random;

            // i is the number of items remaining to be shuffled.
            for (int i = array.Length; i > 1; i--)
            {
                // Pick a random element to swap with the i-th element.
                int j = random.Next(i);

                // Swap array elements.
                var aux = array[j];
                array[j] = array[i - 1];
                array[i - 1] = aux;
            }
        }

        /// <summary>
        ///   Shuffles a collection.
        /// </summary>
        /// 
        public static void Shuffle<T>(this IList<T> array)
        {
            var random = Accord.Math.Tools.Random;

            // i is the number of items remaining to be shuffled.
            for (int i = array.Count; i > 1; i--)
            {
                // Pick a random element to swap with the i-th element.
                int j = random.Next(i);

                // Swap array elements.
                var aux = array[j];
                array[j] = array[i - 1];
                array[i - 1] = aux;
            }
        }

        /// <summary>
        ///   Sorts the elements of an entire one-dimensional array using the given comparison.
        /// </summary>
        /// 
        public static void Sort<T>(this T[] values, Comparison<T> comparison, bool stable = false)
        {
            if (!stable)
            {
                Array.Sort(values, comparison);
                return;
            }

            var keys = new KeyValuePair<int, T>[values.Length];
            for (var i = 0; i < values.Length; i++)
                keys[i] = new KeyValuePair<int, T>(i, values[i]);
            Array.Sort(keys, values, new StableComparer<T>(comparison));
        }

        /// <summary>
        ///   Sorts the elements of an entire one-dimensional array using the given comparison.
        /// </summary>
        /// 
        public static void Sort<T>(this T[] values, bool stable = false)
            where T : IComparable<T>
        {
            if (!stable)
            {
                Array.Sort(values);
                return;
            }

            var keys = new KeyValuePair<int, T>[values.Length];
            for (var i = 0; i < values.Length; i++)
                keys[i] = new KeyValuePair<int, T>(i, values[i]);
            Array.Sort(keys, values, new StableComparer<T>((a, b) => a.CompareTo(b)));
        }

        /// <summary>
        ///   Sorts the elements of an entire one-dimensional array using the given comparison.
        /// </summary>
        /// 
        public static void Sort<T>(this T[] values, out int[] order, bool stable = false)
            where T : IComparable<T>
        {
            if (!stable)
            {
                order = Matrix.Indices(values.Length);
                Array.Sort(values, order);
                return;
            }

            var keys = new KeyValuePair<int, T>[values.Length];
            for (var i = 0; i < values.Length; i++)
                keys[i] = new KeyValuePair<int, T>(i, values[i]);
            Array.Sort(keys, values, new StableComparer<T>((a, b) => a.CompareTo(b)));

            order = new int[values.Length];
            for (int i = 0; i < keys.Length; i++)
                order[i] = keys[i].Key;
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
            var v = new T[size];
            for (int i = 0; i < v.Length; i++)
                v[i] = (T)Convert.ChangeType(1, typeof(T));
            return v;
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
        ///   Creates a vector with the given dimension and starting values.
        /// </summary>
        /// 
        /// <param name="size">The number of elements in the vector.</param>
        /// <param name="values">The initial values for the vector.</param>
        /// 
        public static T[] Create<T>(int size, T[] values)
        {
            T[] vector = new T[size];

            if (values != null)
            {
                for (int i = 0; i < values.Length; i++)
                    vector[i] = values[i];
            }

            return vector;
        }

        /// <summary>
        ///   Creates a vector with the given dimension and starting values.
        /// </summary>
        /// 
        /// <param name="size">The number of elements in the vector.</param>
        /// <param name="value">The initial value for the elements in the vector.</param>
        /// 
        public static T[] Create<T>(int size, T value)
        {
            T[] vector = new T[size];

            for (int i = 0; i < size; i++)
                vector[i] = value;

            return vector;
        }

        /// <summary>
        ///   Creates a vector containing a given range of values.
        /// </summary>
        /// 
        /// <param name="a">The starting value in the range.</param>
        /// <param name="b">The ending value in the range.</param>
        /// <param name="increment">The size of the increment between two elements in the vector.</param>
        /// 
        public static double[] Create(double a, double b, double increment)
        {
            List<double> list = new List<double>();

            for (double i = a; i < b; i += increment)
                list.Add(i);

            if (list[list.Count - 1] != b)
                list.Add(b);

            return list.ToArray();
        }

        /// <summary>
        ///   Creates a vector containing a given range of values.
        /// </summary>
        /// 
        /// <param name="a">The starting value in the range.</param>
        /// <param name="b">The ending value in the range.</param>
        /// <param name="increment">The size of the increment between two elements in the vector.</param>
        /// 
        public static int[] Create(int a, int b, int increment)
        {
            List<int> list = new List<int>();

            for (int i = a; i < b; i += increment)
                list.Add(i);

            if (list[list.Count - 1] != b)
                list.Add(b);

            return list.ToArray();
        }


        /// <summary>
        ///   Creates a vector containing a given range of values.
        /// </summary>
        /// 
        /// <param name="size">The number of elements in the vector.</param>
        /// <param name="a">The starting value in the range.</param>
        /// <param name="b">The ending value in the range.</param>
        /// 
        public static double[] Create(int size, double a, double b)
        {
            double[] list = new double[size];
            double increment = (b - a) / size;

            for (int i = 0; i < list.Length; i++)
                list[i] = increment * i;

            return list;
        }
    }
}
