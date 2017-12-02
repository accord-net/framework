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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    public static partial class Matrix
    {

        /// <summary>
        ///   Gets the number of rows in a jagged matrix.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the elements in the matrix.</typeparam>
        /// <param name="values">The matrix whose number of rows must be computed.</param>
        /// 
        /// <returns>The number of rows in the matrix.</returns>
        /// 
        public static int Rows<T>(this IList<IList<T>> values)
        {
            return values.Count;
        }

        /// <summary>
        ///   Gets the number of columns in a jagged matrix.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the elements in the matrix.</typeparam>
        /// <param name="values">The matrix whose number of rows must be computed.</param>
        /// 
        /// <returns>The number of columns in the matrix.</returns>
        /// 
        public static int Columns<T>(this IList<IList<T>> values)
        {
            return values[0].Count;
        }

        /// <summary>
        ///   Converts a matrix represented as a nested list of lists into a multi-dimensional matrix.
        /// </summary>
        /// 
        public static T[,] ToMatrix<T, U>(this IList<IList<T>> values)
        {
            int rows = values.Rows();
            int cols = values.Columns();

            T[,] result = Matrix.Zeros<T>(rows, cols);
            for (int i = 0; i < values.Count; i++)
                for (int j = 0; j < values[i].Count; j++)
                    result[i, j] = values[i][j];

            return result;
        }

        /// <summary>
        ///   Converts a matrix represented as a nested list of lists into a jagged matrix.
        /// </summary>
        /// 
        public static T[][] ToJagged<T>(this IList<IList<T>> values)
        {
            int rows = values.Rows();
            int cols = values.Columns();

            T[][] result = Jagged.Zeros<T>(rows, cols);
            for (int i = 0; i < values.Count; i++)
                for (int j = 0; j < values[i].Count; j++)
                    result[i][j] = values[i][j];

            return result;
        }


    }
}
