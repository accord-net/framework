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

        public static T[][] Row<T>(params T[] values)
        {
            return new T[][] { values };
        }

        public static T[][] Column<T>(params T[] values)
        {
            T[][] column = new T[values.Length][];
            for (int i = 0; i < column.Length; i++)
                column[i] = new[] { values[i] };

            return column;
        }
    }
}
