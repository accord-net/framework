// Accord Core Library
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

#if NET35 || NETSTANDARD1_4
namespace Accord.Compat
{
    using Accord;
    using System.Collections.Generic;
    using System;
    using System.Threading;

    /// <summary>
    ///   Minimum Parallel Tasks implementation for .NET 3.5 and .NET Standard 1.4 to make
    ///   Accord.NET work. This is nowhere a functional implementation and exists only to 
    ///   provide compile-time compatibility with previous framework versions.
    /// </summary>
    /// 
    internal static class Parallel
    {
        /// <summary>
        ///   Loop body delegate.
        /// </summary>
        /// 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public delegate void ForLoopBody(int index);

        /// <summary>
        ///   Loop body delegate.
        /// </summary>
        /// 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public delegate void ForEachLoopBody<T>(T value);

        /// <summary>
        ///   Loop body delegate.
        /// </summary>
        /// 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public delegate T ForLoopBody<T>(int index, Object state, T result);

        /// <summary>
        ///   Parallel for mock-up. The provided
        ///   code will NOT be run in parallel.
        /// </summary>
        /// 
        public static void For(int start, int stop, ForLoopBody loopBody)
        {
            for (int i = start; i < stop; i++)
                loopBody(i);
        }

        /// <summary>
        ///   Parallel foreach mock-up. The provided
        ///   code will NOT be run in parallel.
        /// </summary>
        /// 
        public static void ForEach<T>(IEnumerable<T> values, ForEachLoopBody<T> loopBody)
        {
            foreach (var v in values)
                loopBody(v);
        }

        /// <summary>
        ///   Parallel for mock-up. The provided
        ///   code will NOT be run in parallel.
        /// </summary>
        /// 
        public static void For(int start, int stop, ParallelOptions options, ForLoopBody loopBody)
        {
            for (int i = start; i < stop; i++)
                loopBody(i);
        }

        /// <summary>
        ///   Parallel for mock-up. The provided
        ///   code will NOT be run in parallel.
        /// </summary>
        /// 
        public static void For<T>(int start, int stop, ParallelOptions options,
            Func<T> initial, ForLoopBody<T> loopBody, Action<T> end)
        {
            T obj = initial();
            for (int i = start; i < stop; i++)
                obj = loopBody(i, null, obj);
            end(obj);
        }

        /// <summary>
        ///   Parallel for mock-up. The provided
        ///   code will NOT be run in parallel.
        /// </summary>
        /// 
        public static void For<T>(int start, int stop,
            Func<T> initial, ForLoopBody<T> loopBody, Action<T> end)
        {
            For(start, stop, null, initial, loopBody, end);
        }
    }

    /// <summary>
    ///   Minimum Parallel Tasks implementation for .NET 3.5 to make
    ///   Accord.NET work. This is nowhere a functional implementation
    ///   and exists only to provide compile-time compatibility with
    ///   previous framework versions.
    /// </summary>
    /// 
    public class ParallelOptions
    {
        /// <summary>
        ///   Does not have any effect in .NET 3.5 or .NET Standard 1.4.
        /// </summary>
        /// 
        public int MaxDegreeOfParallelism { get; set; }

        /// <summary>
        ///   Does not have any effect in .NET 3.5 or .NET Standard 1.4.
        /// </summary>
        /// 
        public CancellationToken CancellationToken { get; set; }
    }
}
#endif
