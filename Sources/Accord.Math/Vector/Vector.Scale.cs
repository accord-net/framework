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


        public static int Scale(this double value, IntRange from, IntRange to)
        {
            return Scale(value, from.Min, from.Max, to.Min, to.Max);
        }

        public static double Scale(this double value, IntRange from, DoubleRange to)
        {
            return Scale(value, from.Min, from.Max, to.Min, to.Max);
        }

        public static float Scale(this double value, IntRange from, Range to)
        {
            return Scale(value, from.Min, from.Max, to.Min, to.Max);
        }

        public static int Scale(this double value, DoubleRange from, IntRange to)
        {
            return Scale(value, from.Min, from.Max, to.Min, to.Max);
        }

        public static double Scale(this double value, DoubleRange from, DoubleRange to)
        {
            return Scale(value, from.Min, from.Max, to.Min, to.Max);
        }

        public static float Scale(this double value, DoubleRange from, Range to)
        {
            return Scale(value, from.Min, from.Max, to.Min, to.Max);
        }

        public static int Scale(this double value, Range from, IntRange to)
        {
            return Scale(value, from.Min, from.Max, to.Min, to.Max);
        }

        public static double Scale(this double value, Range from, DoubleRange to)
        {
            return Scale(value, from.Min, from.Max, to.Min, to.Max);
        }

        public static float Scale(this double value, Range from, Range to)
        {
            return Scale(value, from.Min, from.Max, to.Min, to.Max);
        }


        public static int[] Scale(this int[] values, IntRange from, IntRange to)
        {
            return Scale(values, from.Min, from.Max, to.Min, to.Max);
        }

        public static int[] Scale(this int[] values, DoubleRange from, IntRange to)
        {
            return Scale(values, from.Min, from.Max, to.Min, to.Max);
        }

        public static int[] Scale(this int[] values, Range from, IntRange to)
        {
            return Scale(values, from.Min, from.Max, to.Min, to.Max);
        }

        public static double[] Scale(this double[] values, DoubleRange from, DoubleRange to)
        {
            return Scale(values, from.Min, from.Max, to.Min, to.Max);
        }

        public static double[] Scale(this double[] values, IntRange from, DoubleRange to)
        {
            return Scale(values, from.Min, from.Max, to.Min, to.Max);
        }

        public static double[] Scale(this double[] values, Range from, DoubleRange to)
        {
            return Scale(values, from.Min, from.Max, to.Min, to.Max);
        }

        public static float[] Scale(this float[] values, Range from, Range to)
        {
            return Scale(values, from.Min, from.Max, to.Min, to.Max);
        }

        public static float[] Scale(this float[] values, IntRange from, Range to)
        {
            return Scale(values, from.Min, from.Max, to.Min, to.Max);
        }

        public static float[] Scale(this float[] values, DoubleRange from, Range to)
        {
            return Scale(values, from.Min, from.Max, to.Min, to.Max);
        }

        public static T[] Scale<T>(this double[] values, T toMin, T toMax)
        {
            return Scale(values, values.Min(), values.Max(), toMin, toMax);
        }

        public static T[] Scale<T>(this int[] values, T toMin, T toMax)
        {
            return Scale(values, values.Min(), values.Max(), toMin, toMax);
        }

        public static T[] Scale<T>(this float[] values, T toMin, T toMax)
        {
            return Scale(values, values.Min(), values.Max(), toMin, toMax);
        }

        public static int[] Scale<T>(this double[] values, IntRange to)
        {
            return Scale(values, values.Min(), values.Max(), to.Min, to.Max);
        }

        public static int[] Scale<T>(this int[] values, IntRange to)
        {
            return Scale(values, values.Min(), values.Max(), to.Min, to.Max);
        }

        public static int[] Scale<T>(this float[] values, IntRange to)
        {
            return Scale(values, values.Min(), values.Max(), to.Min, to.Max);
        }

        public static double[] Scale<T>(this double[] values, DoubleRange to)
        {
            return Scale(values, values.Min(), values.Max(), to.Min, to.Max);
        }

        public static double[] Scale<T>(this float[] values, DoubleRange to)
        {
            return Scale(values, values.Min(), values.Max(), to.Min, to.Max);
        }

        public static double[] Scale<T>(this int[] values, DoubleRange to)
        {
            return Scale(values, values.Min(), values.Max(), to.Min, to.Max);
        }

        public static float[] Scale<T>(this double[] values, Range to)
        {
            return Scale(values, values.Min(), values.Max(), to.Min, to.Max);
        }

        public static float[] Scale<T>(this int[] values, Range to)
        {
            return Scale(values, values.Min(), values.Max(), to.Min, to.Max);
        }

        public static float[] Scale<T>(this float[] values, Range to)
        {
            return Scale(values, values.Min(), values.Max(), to.Min, to.Max);
        }






        public static T Scale<T>(this double value, double fromMin, double fromMax, T toMin, T toMax)
        {
            double max = cast<double>(toMax);
            double min = cast<double>(toMin);
            double result;
            if (fromMin != fromMax)
                result = (max - min) * (value - fromMin) / (fromMax - fromMin) + min;
            else
                result = value;

            return cast<T>(result);

        }

        public static T[] Scale<T>(this double[] values, double fromMin, double fromMax, T toMin, T toMax)
        {
            double max = cast<double>(toMax);
            double min = cast<double>(toMin);

            var results = new T[values.Length];
            if (fromMin != fromMax)
            {
                for (int i = 0; i < values.Length; i++)
                    results[i] = cast<T>((max - min) * (values[i] - fromMin) / (fromMax - fromMin) + min);
            }
            else
            {
                for (int i = 0; i < values.Length; i++)
                    results[i] = cast<T>(values[i]);
            }

            return results;
        }

        public static T[] Scale<T>(this int[] values, double fromMin, double fromMax, T toMin, T toMax)
        {
            double max = cast<double>(toMax);
            double min = cast<double>(toMin);

            var results = new T[values.Length];
            if (fromMin != fromMax)
            {
                for (int i = 0; i < values.Length; i++)
                    results[i] = cast<T>((max - min) * (values[i] - fromMin) / (fromMax - fromMin) + min);
            }
            else
            {
                for (int i = 0; i < values.Length; i++)
                    results[i] = cast<T>(values[i]);
            }

            return results;
        }

        public static T[] Scale<T>(this float[] values, double fromMin, double fromMax, T toMin, T toMax)
        {
            double max = cast<double>(toMax);
            double min = cast<double>(toMin);

            var results = new T[values.Length];
            if (fromMin != fromMax)
            {
                for (int i = 0; i < values.Length; i++)
                    results[i] = cast<T>((max - min) * (values[i] - fromMin) / (fromMax - fromMin) + min);
            }
            else
            {
                for (int i = 0; i < values.Length; i++)
                    results[i] = cast<T>(values[i]);
            }

            return results;
        }



        static T cast<T>(this object value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
