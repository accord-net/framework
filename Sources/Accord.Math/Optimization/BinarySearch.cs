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

namespace Accord.Math.Optimization
{
    using System;

    /// <summary>
    ///   Binary search root finding algorithm.
    /// </summary>
    /// 
    public class BinarySearch
    {

        /// <summary>
        ///   Gets or sets the (inclusive) lower bound for the search interval <c>a</c>.
        /// </summary>
        /// 
        public int LowerBound { get; set; }

        /// <summary>
        ///   Gets or sets the (exclusive) upper bound for the search interval <c>a</c>.
        /// </summary>
        /// 
        public int UpperBound { get; set; }

        /// <summary>
        ///   Gets the solution found in the last call
        ///   to or <see cref="FindRoot()"/>.
        /// </summary>
        /// 
        public int Solution { get; private set; }

        /// <summary>
        ///   Gets the value at the solution found in the last call
        ///   to <see cref="FindRoot()"/>.
        /// </summary>
        /// 
        public double Value { get; private set; }


        /// <summary>
        ///   Gets the function to be searched.
        /// </summary>
        /// 
        public Func<int, double> Function { get; private set; }


        /// <summary>
        ///   Constructs a new Binary search algorithm.
        /// </summary>
        /// 
        /// <param name="function">The function to be searched.</param>
        /// <param name="a">Start of search region (inclusive).</param>
        /// <param name="b">End of search region (exclusive).</param>
        /// 
        public BinarySearch(Func<int, double> function, int a, int b)
        {
            this.Function = function;
            this.LowerBound = a;
            this.UpperBound = b;
        }


        /// <summary>
        ///  Finds a value of a function in the interval [a;b). The function can 
        ///  be monotonically increasing or decreasing over the interface [a;b). 
        /// </summary>
        /// 
        /// <returns>The location of the zero value in the given interval.</returns>
        /// 
        public int Find(double value)
        {
            Solution = Find(Function, LowerBound, UpperBound, value);
            Value = Function(Solution);
            return Solution;
        }

        /// <summary>
        ///  Finds a value of a function in the interval [a;b). The function can 
        ///  be monotonically increasing or decreasing over the interface [a;b). 
        /// </summary>
        /// 
        /// <returns>The location of the zero value in the given interval.</returns>
        /// 
        public int FindRoot()
        {
            Solution = Find(Function, LowerBound, UpperBound, 0);
            Value = Function(Solution);
            return Solution;
        }


        /// <summary>
        ///  Finds a value of a function in the interval [a;b). The function can 
        ///  be monotonically increasing or decreasing over the interface [a;b). 
        /// </summary>
        /// 
        /// <param name="function">The function to have its root computed.</param>
        /// <param name="lowerBound">Start of search region (inclusive).</param>
        /// <param name="upperBound">End of search region (exclusive).</param>
        /// <param name="value">The value to be looked for in the function.</param>
        /// 
        /// <returns>The location of the zero value in the given interval.</returns>
        /// 
        public static int Find(Func<int, double> function, int lowerBound, int upperBound, double value)
        {
            int start = lowerBound;
            int end = upperBound;

            // if sign is positive we assume an increasing function
            // otherwise we assume a decreasing function.
            int sign = function(start) <= function(end - 1) ? 1 : -1;

            value = sign * value;

            int m = (int)(((long)start + (long)end) / 2);

            double v = sign * function(m);

            while (end >= start)
            {
                if (v < value)
                {
                    start = m + 1;
                }
                else if (v > value)
                {
                    end = m - 1;
                }
                else
                {
                    return m;
                }

                m = (int)(((long)start + (long)end) / 2);
                v = sign * function(m);
            }

            if (v > value || m == upperBound)
                return m;
            return m + 1;
        }

    }
}
