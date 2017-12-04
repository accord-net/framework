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
    using Accord.Math.Random;
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    ///   Set of mathematical tools.
    /// </summary>
    /// 
    public static partial class Tools
    {

        /// <summary>
        ///   Gets a reference to the random number generator used
        ///   internally by the Accord.NET classes and methods.
        /// </summary>
        /// 
        [Obsolete("Please use Generator.Random instead.")]
        public static System.Random Random { get { return Generator.Random; } }

        /// <summary>
        ///   Sets a random seed for the framework's main 
        ///   <see cref="Random">internal number generator</see>.
        /// </summary>
        ///
        [Obsolete("Please set the Seed property of Accord.Math.Random.Generator instead.")]
        public static void SetupGenerator(int? seed)
        {
            Generator.Seed = seed;
        }


        /// <summary>
        ///   Gets the angle formed by the vector [x,y].
        /// </summary>
        /// 
        public static float Angle(float x, float y)
        {
            if (y >= 0)
            {
                if (x >= 0)
                    return (float)Math.Atan(y / x);
                return (float)(Math.PI - Math.Atan(-y / x));
            }
            else
            {
                if (x >= 0)
                    return (float)(2 * Math.PI - Math.Atan(-y / x));
                return (float)(Math.PI + Math.Atan(y / x));
            }
        }

        /// <summary>
        ///   Gets the angle formed by the vector [x,y].
        /// </summary>
        /// 
        public static double Angle(double x, double y)
        {
            if (y >= 0)
            {
                if (x >= 0)
                    return Math.Atan2(y, x);
                return Math.PI - Math.Atan(-y / x);
            }
            else
            {
                if (x >= 0)
                    return 2.0 * Math.PI - Math.Atan2(-y, x);
                return Math.PI + Math.Atan(y / x);
            }
        }

        /// <summary>
        ///   Gets the displacement angle between two points.
        /// </summary>
        /// 
        public static double Angle(IntPoint previous, IntPoint next)
        {
            double dx = next.X - previous.X;
            double dy = next.Y - previous.Y;

            return Accord.Math.Tools.Angle(dx, dy);
        }

        /// <summary>
        ///   Gets the displacement angle between two points, coded
        ///   as an integer varying from 0 to 20.
        /// </summary>
        /// 
        public static int Direction(IntPoint previous, IntPoint next)
        {
            double dx = next.X - previous.X;
            double dy = next.Y - previous.Y;

            double radians = Accord.Math.Tools.Angle(dx, dy);

            // code = Floor(20 / (2*System.Math.PI))
            int code = (int)System.Math.Floor(radians * 3.183098861837907);

            return code;
        }

        /// <summary>
        ///   Gets the greatest common divisor between two integers.
        /// </summary>
        /// 
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// 
        /// <returns>The greatest common divisor.</returns>
        /// 
        public static int GreatestCommonDivisor(int a, int b)
        {
            int x = a - b * (int)Math.Floor(a / (double)b);
            while (x != 0)
            {
                a = b;
                b = x;
                x = a - b * (int)Math.Floor(a / (double)b);
            }
            return b;
        }

        /// <summary>
        ///   Returns the next power of 2 after the input value x.
        /// </summary>
        /// 
        /// <param name="x">Input value x.</param>
        /// 
        /// <returns>Returns the next power of 2 after the input value x.</returns>
        /// 
        public static int NextPowerOf2(int x)
        {
            --x;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return ++x;
        }

        /// <summary>
        ///   Returns the previous power of 2 after the input value x.
        /// </summary>
        /// 
        /// <param name="x">Input value x.</param>
        /// 
        /// <returns>Returns the previous power of 2 after the input value x.</returns>
        /// 
        public static int PreviousPowerOf2(int x)
        {
            return NextPowerOf2(x + 1) / 2;
        }


        /// <summary>
        ///   Hypotenuse calculus without overflow/underflow
        /// </summary>
        /// 
        /// <param name="a">First value</param>
        /// <param name="b">Second value</param>
        /// 
        /// <returns>The hypotenuse Sqrt(a^2 + b^2)</returns>
        /// 
        public static double Hypotenuse(double a, double b)
        {
            double r = 0.0;
            double absA = System.Math.Abs(a);
            double absB = System.Math.Abs(b);

            if (absA > absB)
            {
                r = b / a;
                r = absA * System.Math.Sqrt(1 + r * r);
            }
            else if (b != 0)
            {
                r = a / b;
                r = absB * System.Math.Sqrt(1 + r * r);
            }

            return r;
        }

        /// <summary>
        ///   Hypotenuse calculus without overflow/underflow
        /// </summary>
        /// 
        /// <param name="a">first value</param>
        /// <param name="b">second value</param>
        /// 
        /// <returns>The hypotenuse Sqrt(a^2 + b^2)</returns>
        /// 
        public static decimal Hypotenuse(decimal a, decimal b)
        {
            decimal r = 0;
            decimal absA = System.Math.Abs(a);
            decimal absB = System.Math.Abs(b);

            if (absA > absB)
            {
                r = b / a;
                r = absA * (decimal)System.Math.Sqrt((double)(1 + r * r));
            }
            else if (b != 0)
            {
                r = a / b;
                r = absB * (decimal)System.Math.Sqrt((double)(1 + r * r));
            }

            return r;
        }

        /// <summary>
        ///   Hypotenuse calculus without overflow/underflow
        /// </summary>
        /// 
        /// <param name="a">first value</param>
        /// <param name="b">second value</param>
        /// 
        /// <returns>The hypotenuse Sqrt(a^2 + b^2)</returns>
        /// 
        public static float Hypotenuse(float a, float b)
        {
            double r = 0;
            float absA = System.Math.Abs(a);
            float absB = System.Math.Abs(b);

            if (absA > absB)
            {
                r = b / a;
                r = absA * System.Math.Sqrt(1 + r * r);
            }
            else if (b != 0)
            {
                r = a / b;
                r = absB * System.Math.Sqrt(1 + r * r);
            }

            return (float)r;
        }

        /// <summary>
        ///   Gets the proper modulus operation for
        ///   an integer value x and modulo m.
        /// </summary>
        /// 
        public static int Mod(int x, int m)
        {
            if (m < 0)
                m = -m;

            int r = x % m;

            return r < 0 ? r + m : r;
        }

        /// <summary>
        ///   Gets the proper modulus operation for
        ///   a real value x and modulo m.
        /// </summary>
        /// 
        public static double Mod(double x, double m)
        {
            if (m < 0)
                m = -m;

            double r = x % m;

            return r < 0 ? r + m : r;
        }


        #region Scaling functions
        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Scale instead.")]
        public static int Scale(this IntRange from, IntRange to, int x)
        {
            return Accord.Math.Vector.Scale(x, (IRange<int>)from, (IRange<int>)to);
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Scale instead.")]
        public static double Scale(this DoubleRange from, DoubleRange to, double x)
        {
            return Accord.Math.Vector.Scale(x, from, to);
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Scale instead.")]
        public static double Scale(double fromMin, double fromMax, double toMin, double toMax, double x)
        {
            return Accord.Math.Vector.Scale(x, fromMin, fromMax, toMin, toMax);
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Scale instead.")]
        public static double[] Scale(double fromMin, double fromMax, double toMin, double toMax, double[] x)
        {
            return Accord.Math.Vector.Scale(x, fromMin, fromMax, toMin, toMax);
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Scale instead.")]
        public static int[] Scale(int fromMin, int fromMax, int toMin, int toMax, int[] x)
        {
            return Accord.Math.Vector.Scale(x, fromMin, fromMax, toMin, toMax);
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Scale instead.")]
        public static int[] Scale(IntRange from, IntRange to, int[] x)
        {
            return Accord.Math.Vector.Scale(x, (IRange<int>)from, (IRange<int>)to);
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Scale instead.")]
        public static double[] Scale(DoubleRange from, DoubleRange to, double[] x)
        {
            return Accord.Math.Vector.Scale(values: x, fromRange: from, toRange: to);
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Scale instead.")]
        public static float[] Scale(float fromMin, float fromMax, float toMin, float toMax, float[] x)
        {
            return Accord.Math.Vector.Scale(x, fromMin, fromMax, toMin, toMax);
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Scale instead.")]
        public static float[] Scale(Range from, Range to, float[] x)
        {
            return Accord.Math.Vector.Scale(x, (IRange<float>)from, (IRange<float>)to);
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Scale instead.")]
        public static double[] Scale(double toMin, double toMax, double[] x)
        {
            return Accord.Math.Vector.Scale(x, toMin, toMax);
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Scale instead.")]
        public static double[][] Scale(double[] fromMin, double[] fromMax, double[] toMin, double[] toMax, double[][] x)
        {
            int rows = x.Length;
            int cols = fromMin.Length;

            double[][] result = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                result[i] = new double[cols];
                for (int j = 0; j < cols; j++)
                {
                    result[i][j] = (toMax[j] - toMin[j]) * (x[i][j] - fromMin[j]) / (fromMax[j] - fromMin[j]) + toMin[j];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Scale instead.")]
        public static double[][] Scale(double fromMin, double fromMax, double toMin, double toMax, double[][] x)
        {
            int rows = x.Length;

            double[][] result = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                result[i] = new double[x[i].Length];
                for (int j = 0; j < result[i].Length; j++)
                {
                    result[i][j] = (toMax - toMin) * (x[i][j] - fromMin) / (fromMax - fromMin) + toMin;
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Scale instead.")]
        public static double[][] Scale(double[] fromMin, double[] fromMax, double toMin, double toMax, double[][] x)
        {
            int rows = x.Length;
            int cols = fromMin.Length;

            double[][] result = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                result[i] = new double[cols];
                for (int j = 0; j < cols; j++)
                {
                    result[i][j] = (toMax - toMin) * (x[i][j] - fromMin[j]) / (fromMax[j] - fromMin[j]) + toMin;
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Scale instead.")]
        public static double[][] Scale(double[] toMin, double[] toMax, double[][] x)
        {
            var min = Matrix.Min(x, 0);
            var max = Matrix.Max(x, 0);
            return Scale(min, max, toMin, toMax, x);
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Scale instead.")]
        public static double[][] Scale(double toMin, double toMax, double[][] x)
        {
            return Scale(Matrix.Min(x, 0), Matrix.Max(x, 0), toMin, toMax, x);
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Scale instead.")]
        public static float Scale(float fromMin, float fromMax, float toMin, float toMax, float x)
        {
            return Accord.Math.Vector.Scale(x, fromMin, fromMax, toMin, toMax);
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Scale instead.")]
        public static double Scale(IntRange from, DoubleRange to, int x)
        {
            return Accord.Math.Vector.Scale(x, from, to);
        }
        #endregion


        /// <summary>
        ///   Returns the hyperbolic arc cosine of the specified value.
        /// </summary>
        /// 
        public static double Acosh(double x)
        {
            if (x < 1.0)
                throw new ArgumentOutOfRangeException("x");
            return System.Math.Log(x + System.Math.Sqrt(x * x - 1));
        }

        /// <summary>
        /// Returns the hyperbolic arc sine of the specified value.
        /// </summary>
        /// 
        public static double Asinh(double d)
        {
            double x;
            int sign;

            if (d == 0.0)
                return d;

            if (d < 0.0)
            {
                sign = -1;
                x = -d;
            }
            else
            {
                sign = 1;
                x = d;
            }
            return sign * System.Math.Log(x + System.Math.Sqrt(x * x + 1));
        }

        /// <summary>
        /// Returns the hyperbolic arc tangent of the specified value.
        /// </summary>
        /// 
        public static double Atanh(double d)
        {
            if (d > 1.0 || d < -1.0)
                throw new ArgumentOutOfRangeException("d");
            return 0.5 * System.Math.Log((1.0 + d) / (1.0 - d));
        }



        /// <summary>
        ///   Returns the factorial falling power of the specified value.
        /// </summary>
        /// 
        public static int FactorialPower(int value, int degree)
        {
            int t = value;
            for (int i = 0; i < degree; i++)
                t *= degree--;
            return t;
        }

        /// <summary>
        ///   Truncated power function.
        /// </summary>
        /// 
        public static double TruncatedPower(double value, double degree)
        {
            double x = System.Math.Pow(value, degree);
            return (x > 0) ? x : 0.0;
        }

        /// <summary>
        ///   Fast inverse floating-point square root.
        /// </summary>
        ///
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float InvSqrt(float f)
        {
            unsafe
            {
                float xhalf = 0.5f * f;
                Int32 i = *(Int32*)&f;
                i = 0x5f375a86 - (i >> 1);
                f = *(float*)&i;
                f = f * (1.5f - xhalf * f * f);
                return f;
            }
        }


        /// <summary>
        ///   Sorts the elements of an entire one-dimensional array using the given comparison.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Sort instead.")]
        public static void StableSort<T>(this T[] values, Comparison<T> comparison)
        {
            Vector.Sort(values, comparison, true);
        }

        /// <summary>
        ///   Sorts the elements of an entire one-dimensional array using the given comparison.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Sort instead.")]
        public static void StableSort<T>(this T[] values)
            where T : IComparable<T>
        {
            Vector.Sort(values, true);
        }

        /// <summary>
        ///   Sorts the elements of an entire one-dimensional array using the given comparison.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Sort instead.")]
        public static void StableSort<T>(this T[] values, out int[] order)
            where T : IComparable<T>
        {
            Vector.Sort(values, out order, true);
        }

        /// <summary>
        ///   Interpolates data using a piece-wise linear function.
        /// </summary>
        /// 
        /// <param name="value">The value to be calculated.</param>
        /// <param name="x">The input data points <c>x</c>. Those values need to be sorted.</param>
        /// <param name="y">The output data points <c>y</c>.</param>
        /// <param name="lower">
        ///   The value to be returned for values before the first point in <paramref name="x"/>.</param>
        /// <param name="upper">
        ///   The value to be returned for values after the last point in <paramref name="x"/>.</param>
        ///   
        /// <returns>Computes the output for f(value) by using a piecewise linear
        ///   interpolation of the data points <paramref name="x"/> and <paramref name="y"/>.</returns>
        /// 
        public static double Interpolate1D(double value, double[] x, double[] y, double lower, double upper)
        {
            for (int i = 0; i < x.Length; i++)
            {
                if (value < x[i])
                {
                    if (i == 0)
                        return lower;

                    int start = i - 1;
                    int next = i;

                    double m = (value - x[start]) / (x[next] - x[start]);
                    return y[start] + (y[next] - y[start]) * m;
                }
            }

            return upper;
        }

        /// <summary>
        ///   Gets the maximum value among three values.
        /// </summary>
        /// 
        /// <param name="a">The first value <c>a</c>.</param>
        /// <param name="b">The second value <c>b</c>.</param>
        /// <param name="c">The third value <c>c</c>.</param>
        /// 
        /// <returns>The maximum value among <paramref name="a"/>, 
        ///   <paramref name="b"/> and <paramref name="c"/>.</returns>
        /// 
        public static double Max(double a, double b, double c)
        {
            if (a > b)
            {
                if (c > a)
                    return c;
                return a;
            }
            else
            {
                if (c > b)
                    return c;
                return b;
            }
        }

        /// <summary>
        ///   Gets the minimum value among three values.
        /// </summary>
        /// 
        /// <param name="a">The first value <c>a</c>.</param>
        /// <param name="b">The second value <c>b</c>.</param>
        /// <param name="c">The third value <c>c</c>.</param>
        /// 
        /// <returns>The minimum value among <paramref name="a"/>, 
        ///   <paramref name="b"/> and <paramref name="c"/>.</returns>
        /// 
        public static double Min(double a, double b, double c)
        {
            if (a < b)
            {
                if (c < a)
                    return c;
                return a;
            }
            else
            {
                if (c < b)
                    return c;
                return b;
            }
        }

        /// <summary>
        /// Calculates power of 2.
        /// </summary>
        /// 
        /// <param name="power">Power to raise in.</param>
        /// 
        /// <returns>Returns specified power of 2 in the case if power is in the range of
        /// [0, 30]. Otherwise returns 0.</returns>
        /// 
        public static int Pow2(int power)
        {
            return ((power >= 0) && (power <= 30)) ? (1 << power) : 0;
        }

        /// <summary>
        /// Checks if the specified integer is power of 2.
        /// </summary>
        /// 
        /// <param name="x">Integer number to check.</param>
        /// 
        /// <returns>Returns <b>true</b> if the specified number is power of 2.
        /// Otherwise returns <b>false</b>.</returns>
        /// 
        public static bool IsPowerOf2(int x)
        {
            return (x > 0) ? ((x & (x - 1)) == 0) : false;
        }

        /// <summary>
        /// Get base of binary logarithm.
        /// </summary>
        /// 
        /// <param name="x">Source integer number.</param>
        /// 
        /// <returns>Power of the number (base of binary logarithm).</returns>
        /// 
        public static int Log2(int x)
        {
            if (x <= 65536)
            {
                if (x <= 256)
                {
                    if (x <= 16)
                    {
                        if (x <= 4)
                        {
                            if (x <= 2)
                            {
                                if (x <= 1)
                                    return 0;
                                return 1;
                            }
                            return 2;
                        }
                        if (x <= 8)
                            return 3;
                        return 4;
                    }
                    if (x <= 64)
                    {
                        if (x <= 32)
                            return 5;
                        return 6;
                    }
                    if (x <= 128)
                        return 7;
                    return 8;
                }
                if (x <= 4096)
                {
                    if (x <= 1024)
                    {
                        if (x <= 512)
                            return 9;
                        return 10;
                    }
                    if (x <= 2048)
                        return 11;
                    return 12;
                }
                if (x <= 16384)
                {
                    if (x <= 8192)
                        return 13;
                    return 14;
                }
                if (x <= 32768)
                    return 15;
                return 16;
            }

            if (x <= 16777216)
            {
                if (x <= 1048576)
                {
                    if (x <= 262144)
                    {
                        if (x <= 131072)
                            return 17;
                        return 18;
                    }
                    if (x <= 524288)
                        return 19;
                    return 20;
                }
                if (x <= 4194304)
                {
                    if (x <= 2097152)
                        return 21;
                    return 22;
                }
                if (x <= 8388608)
                    return 23;
                return 24;
            }
            if (x <= 268435456)
            {
                if (x <= 67108864)
                {
                    if (x <= 33554432)
                        return 25;
                    return 26;
                }
                if (x <= 134217728)
                    return 27;
                return 28;
            }
            if (x <= 1073741824)
            {
                if (x <= 536870912)
                    return 29;
                return 30;
            }
            return 31;
        }

        /// <summary>
        ///   Returns the square root of the specified <see cref="decimal"/> number.
        /// </summary>
        /// 
        public static decimal Sqrt(decimal x, decimal epsilon = 0.0M)
        {
            if (x < 0)
                throw new OverflowException("Cannot calculate square root from a negative number.");

            decimal current = (decimal)Math.Sqrt((double)x), previous;

            do
            {
                previous = current;
                if (previous == 0.0M) return 0;
                current = (previous + x / previous) / 2;
            }
            while (Math.Abs(previous - current) > epsilon);

            return current;
        }
    }
}