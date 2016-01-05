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
    internal static class Jagged
    {

        public static T[][] Create<T>(int rows, int columns)
        {
            T[][] matrix = new T[rows][];
            for (int i = 0; i < matrix.Length; i++)
                matrix[i] = new T[columns];

            return matrix;
        }

        public static T[][] RowVector<T>(params T[] values)
        {
            return new T[][] { values };
        }

        public static T[][] ColumnVector<T>(params T[] values)
        {
            T[][] column = new T[values.Length][];
            for (int i = 0; i < column.Length; i++)
                column[i] = new[] { values[i] };

            return column;
        }

        /// <summary>
        ///   Returns the Identity matrix of the given size.
        /// </summary>
        /// 
        public static double[][] Identity(int size)
        {
            return Diagonal(size, 1.0);
        }

        /// <summary>
        ///   Creates a jagged magic square matrix.
        /// </summary>
        /// 
        public static double[][] Magic(int size)
        {
            return Matrix.Magic(size).ToArray();
        }

        /// <summary>
        ///   Returns a square diagonal matrix of the given size.
        /// </summary>
        /// 
        public static T[][] Diagonal<T>(int size, T value)
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException("size", size,
                "Square matrix's size must be a positive integer.");
            }

            var matrix = new T[size][];
            for (int i = 0; i < matrix.Length; i++)
            {
                matrix[i] = new T[size];
                matrix[i][i] = value;
            }

            return matrix;
        }

        /// <summary>
        ///   Return a jagged matrix with a vector of values on its diagonal.
        /// </summary>
        /// 
        // TODO: Mark as obsolete
        public static T[][] Diagonal<T>(T[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            T[][] matrix = new T[values.Length][];
            for (int i = 0; i < values.Length; i++)
            {
                matrix[i] = new T[values.Length];
                matrix[i][i] = values[i];
            }

            return matrix;
        }
    }
}
