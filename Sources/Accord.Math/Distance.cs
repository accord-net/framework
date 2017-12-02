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
    using Accord.Math.Decompositions;
    using Accord.Math.Distances;
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Linq;

    /// <summary>
    ///   Static class Distance. Defines a set of extension methods defining distance measures.
    /// </summary>
    /// 
    public static partial class Distance
    {

        /// <summary>
        ///   Checks whether a function is a real metric distance, i.e. respects
        ///   the triangle inequality. Please note that a function can still pass
        ///   this test and not respect the triangle inequality.
        /// </summary>
        /// 
        public static bool IsMetric(Func<double[], double[], double> value)
        {
            // Direct test
            double z = value(new[] { 1.0 }, new[] { 1.0 });
            if (z > 2 || z < 0)
                return false;


            var a = new double[1];
            var b = new double[1];

            for (int i = -10; i < 10; i++)
            {
                a[0] = i;

                for (int j = -10; j < +10; j++)
                {
                    b[0] = j;
                    double c = value(a, b);

                    if (c > Math.Abs(i) + Math.Abs(j))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        ///   Checks whether a function is a real metric distance, i.e. respects
        ///   the triangle inequality. Please note that a function can still pass
        ///   this test and not respect the triangle inequality.
        /// </summary>
        /// 
        public static bool IsMetric(Func<int[], int[], double> value)
        {
            // Direct test
            double z = value(new[] { 1 }, new[] { 1 });
            if (z > 2 || z < 0)
                return false;

            int size = 3;
            int[] zero = new int[size];

            foreach (var a in Combinatorics.Sequences(3, size, inPlace: true))
            {
                foreach (var b in Combinatorics.Sequences(3, size, inPlace: true))
                {
                    double dza = value(zero, a);
                    double dzb = value(zero, b);
                    double dab = value(a, b);

                    if (dab > dza + dzb)
                        return false;

                    double daz = value(a, zero);
                    double dbz = value(b, zero);
                    double dba = value(b, a);

                    if (daz != dza || dbz != dzb || dab != dba)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        ///   Checks whether a function is a real metric distance, i.e. respects
        ///   the triangle inequality. Please note that a function can still pass
        ///   this test and not respect the triangle inequality.
        /// </summary>
        /// 
        public static bool IsMetric<T>(IDistance<T> value)
        {
            return value is IMetric<T>;
        }

        /// <summary>
        ///   Gets the a <see cref="IDistance{T}"/> object implementing a
        ///   particular method of the <see cref="Distance"/> static class.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        ///   This method is intended to be used in scenarios where you have been using any
        ///   of the static methods in the <see cref="Distance"/> class, but now you would like
        ///   to obtain a reference to an object that implements the same distance you have been
        ///   using before, but in a object-oriented, polymorphic manner. Please see the example
        ///   below for more details.</para>
        /// <para>
        ///   Note: This method relies on reflection and might not work 
        ///   on all scenarios, environments, and/or platforms.</para>  
        /// </remarks>
        /// 
        /// <typeparam name="T">The type of the elements being compared in the distance function.</typeparam>
        /// 
        /// <param name="func">The method of <see cref="Distance"/>.</param>
        /// 
        /// <returns>
        ///   An object of the class that implements the given distance.
        /// </returns>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\DistanceTest.cs" region="doc_getdistance" />
        /// </example>
        /// 
        public static IDistance<T> GetDistance<T>(Func<T, T, double> func)
        {
#if NETSTANDARD1_4
            var methods = typeof(Distance).GetTypeInfo().DeclaredMethods.Where(m=>m.IsPublic && m.IsStatic);
#else
            var methods = typeof(Distance).GetMethods(BindingFlags.Public | BindingFlags.Static);
#endif
            foreach (var method in methods)
            {
#if NETSTANDARD1_4
                var methodInfo = func.GetMethodInfo();
#else
                var methodInfo = func.Method;
#endif
                if (methodInfo == method)
                {
                    var t = Type.GetType("Accord.Math.Distances." + method.Name);

                    if (t == null)
                    {
                        // TODO: Remove the following special case, as it is needed only
                        // for preserving compatibility for a few next releases more.
                        if (methodInfo.Name == "BitwiseHamming")
                            return new Hamming() as IDistance<T>;
                    }

                    return (IDistance<T>)Activator.CreateInstance(t, new object[] { });
                }
            }

            return null;
        }

        /// <summary>
        ///   Gets the Bitwise Hamming distance between two points.
        ///   Please use the <see cref="Distance.Hamming(byte[], byte[])">Distance.Hamming</see>
        ///   method or the <see cref="Accord.Math.Distances.Hamming"/> class instead.
        /// </summary>
        /// 
        [Obsolete("Please use Distance.Hamming instead.")]
        public static double BitwiseHamming(byte[] x, byte[] y)
        {
            return Distance.Hamming(x, y);
        }

        /// <summary>
        ///   Gets the Levenshtein distance between two points.
        /// </summary>
        ///  
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// 
        /// <returns>The Levenshtein distance between x and y.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Levenshtein<T>(T[] x, T[] y)
        {
            return new Levenshtein<T>().Distance(x, y);
        }

    }
}
