using Accord.Math.Comparers;
using AForge;
using AForge.Math.Random;
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
    internal static partial class Vector
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
            for (int i = 0; i < size; i++)
                vector[i] = values[i];

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
        ///   Creates an interval vector.
        /// </summary>
        /// 
        public static int[] Interval(int a, int b)
        {
            return Interval(a, b, 1.0);
        }

        /// <summary>
        ///   Creates an interval vector.
        /// </summary>
        /// 
        public static T[] Interval<T>(T a, T b, double stepSize)
        {
            double from = cast<double>(a);
            double to = cast<double>(b);

            T[] r;

            if (from > to)
            {
                double range = from - to;
                int steps = (int)System.Math.Ceiling(range / stepSize) + 1;

                r = new T[steps];
                for (int i = 0; i < r.Length; i++)
                    r[i] = cast<T>(from - i * stepSize);
                r[steps - 1] = cast<T>(to);
            }
            else
            {
                double range = to - from;
                int steps = (int)System.Math.Ceiling(range == 0 ? 0 : range / stepSize) + 1;

                r = new T[steps];
                for (int i = 0; i < r.Length; i++)
                    r[i] = cast<T>(from + i * stepSize);
                r[steps - 1] = cast<T>(to);
            }

            return r;
        }

        /// <summary>
        ///   Creates an interval vector.
        /// </summary>
        /// 
        public static T[] Interval<T>(T a, T b, int steps)
        {
            double from = cast<double>(a);
            double to = cast<double>(b);

            if (from == to)
            {
                return new T[] { a };
            }

            if (steps == Int32.MaxValue)
            {
                throw new ArgumentOutOfRangeException("steps",
                    "input must be lesser than Int32.MaxValue");
            }

            var r = new T[steps + 1];

            if (from > to)
            {
                double range = from - to;
                double stepSize = range / steps;
                for (int i = 0; i < r.Length; i++)
                    r[i] = cast<T>(from - i * stepSize);
                r[steps] = cast<T>(to);
            }
            else
            {
                double range = to - from;
                double stepSize = range / steps;
                for (int i = 0; i < r.Length; i++)
                    r[i] = cast<T>(from + i * stepSize);
                r[steps] = cast<T>(to);
            }

            return r;
        }

        /// <summary>
        ///   Creates an interval vector.
        /// </summary>
        /// 
        public static double[] Interval(this DoubleRange range, int steps)
        {
            return Interval(range.Min, range.Max, steps);
        }

        /// <summary>
        ///   Creates an interval vector.
        /// </summary>
        /// 
        public static int[] Interval(this IntRange range, int steps)
        {
            return Interval(range.Min, range.Max, steps);
        }

        /// <summary>
        ///   Creates an interval vector.
        /// </summary>
        /// 
        public static float[] Interval(this Range range, int steps)
        {
            return Interval(range.Min, range.Max, steps);
        }

        /// <summary>
        ///   Creates an interval vector.
        /// </summary>
        /// 
        public static double[] Interval(this DoubleRange range, double stepSize)
        {
            return Interval(range.Min, range.Max, stepSize);
        }

        /// <summary>
        ///   Creates an interval vector.
        /// </summary>
        /// 
        public static int[] Interval(this IntRange range, double stepSize)
        {
            return Interval(range.Min, range.Max, stepSize);
        }

        /// <summary>
        ///   Creates an interval vector.
        /// </summary>
        /// 
        public static float[] Interval(this Range range, double stepSize)
        {
            return Interval(range.Min, range.Max, stepSize);
        }


    }
}
