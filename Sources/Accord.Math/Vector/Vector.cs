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

    /// <summary>
    ///   Static class Vector. Defines a set of extension methods
    ///   that operates mainly on single-dimensional arrays.
    /// </summary>
    /// 
    public static partial class Vector
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
        public static void Sort<T>(this T[] values, out int[] order, bool stable = false, ComparerDirection direction = ComparerDirection.Ascending)
            where T : IComparable<T>
        {
            if (!stable)
            {
                order = Matrix.Indices(values.Length);
                if (direction == ComparerDirection.Ascending)
                    Array.Sort(values, order);
                else
                    Array.Sort(values, order, new GeneralComparer<T>(direction));

                return;
            }

            var keys = new KeyValuePair<int, T>[values.Length];
            for (var i = 0; i < values.Length; i++)
                keys[i] = new KeyValuePair<int, T>(i, values[i]);

            if (direction == ComparerDirection.Ascending)
                Array.Sort(keys, values, new StableComparer<T>((a, b) => a.CompareTo(b)));
            else
                Array.Sort(keys, values, new StableComparer<T>((a, b) => -a.CompareTo(b)));

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
            var one = (T)Convert.ChangeType(1, typeof(T));

            var v = new T[size];
            for (int i = 0; i < v.Length; i++)
                v[i] = one;
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
            int min = System.Math.Min(size, values.Length);
            for (int i = 0; i < min; i++)
                vector[i] = values[i];

            return vector;
        }

        /// <summary>
        ///   Creates a vector with the given dimension default value.
        /// </summary>
        /// 
        /// <param name="size">The number of elements in the vector.</param>
        /// 
        public static T[] Create<T>(int size)
        {
            return new T[size];
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
        ///   Creates a vector with uniformly distributed random data.
        /// </summary>
        /// 
        public static double[] Random(int size)
        {
            return Random(size, 0.0, 1.0);
        }

        /// <summary>
        ///   Creates a vector with uniformly distributed random data.
        /// </summary>
        /// 
        public static int[] Random(int size, int min, int max)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException("size", size, "Size must be a positive integer.");

            var random = Accord.Math.Random.Generator.Random;

            var vector = new int[size];
            for (int i = 0; i < size; i++)
                vector[i] = random.Next(max, min);
            return vector;
        }

        /// <summary>
        ///   Creates a vector with uniformly distributed random data.
        /// </summary>
        /// 
        public static double[] Random(int size, double min, double max)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException("size", size, "Size must be a positive integer.");

            var random = Accord.Math.Random.Generator.Random;

            var vector = new double[size];
            for (int i = 0; i < size; i++)
                vector[i] = random.NextDouble() * (max - min) + min;
            return vector;
        }

        /// <summary>
        ///   Creates a vector with uniformly distributed random data.
        /// </summary>
        /// 
        public static float[] Random(int size, float min, float max)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException("size", size, "Size must be a positive integer.");

            var random = Accord.Math.Random.Generator.Random;

            var vector = new float[size];
            for (int i = 0; i < size; i++)
                vector[i] = (float)(random.NextDouble() * (max - min) + min);
            return vector;
        }

        /// <summary>
        ///   Draws a random sample from a group of observations, without repetitions.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the observations.</typeparam>
        /// 
        /// <param name="values">The observation vector.</param>
        /// <param name="size">The size of the sample to be drawn (how many samples to get).</param>
        /// 
        /// <returns>A vector containing the samples drawn from <paramref name="values"/>.</returns>
        /// 
        public static T[] Sample<T>(T[] values, int size)
        {
            return values.Submatrix(Indices.Random(size));
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
        public static int[] Histogram(int[] labels)
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
        public static int[] Histogram(int[] labels, int size)
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
        public static int[] Histogram(int[] labels, int[] result)
        {
            for (int i = 0; i < labels.Length; i++)
                result[labels[i]]++;
            return result;
        }
    }
}
