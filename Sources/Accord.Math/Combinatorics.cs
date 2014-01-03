// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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

    /// <summary>
    ///   Static class for combinatorics functions.
    /// </summary>
    /// 
    public static class Combinatorics
    {
        /// <summary>
        ///   Generates all possible two symbol ordered
        ///   permutations with repetitions allowed (a truth table).
        /// </summary>
        /// 
        /// <param name="length">The length of the sequence to generate.</param>
        ///
        public static int[][] TruthTable(int length)
        {
            return TruthTable(2, length);
        }

        /// <summary>
        ///   Generates all possible ordered permutations
        ///   with repetitions allowed (a truth table).
        /// </summary>
        /// 
        /// <param name="symbols">The number of symbols.</param>
        /// <param name="length">The length of the sequence to generate.</param>
        ///
        public static int[][] TruthTable(int symbols, int length)
        {
            int size = (int)Math.Pow(symbols, length);

            int[][] sequences = new int[size][];
            for (int i = 0; i < sequences.Length; i++)
                sequences[i] = new int[length];

            // For each column
            for (int col = 0; col < length; col++)
            {
                int row = 0;

                // We will write the symbol 2^(l-i) times
                int p = (int)Math.Pow(symbols, length - col - 1);
                int q = (int)Math.Pow(symbols, col);

                // For each repeating block
                for (int r = 0; r < q; r++)
                {
                    // For each symbol to be written
                    for (int j = 0; j < symbols; j++)
                    {
                        for (int k = 0; k < p; k++)
                            sequences[row++][col] = j;
                    }
                }
            }

            return sequences;
        }

        /// <summary>
        ///   Enumerates all possible value combinations
        ///   of a given size for an array.
        /// </summary>
        /// 
        public static IEnumerable<T[]> Combinations<T>(T[] values, int k)
        {
            // Based on the Knuth algorithm implementation by
            // http://seekwell.wordpress.com/2007/11/17/knuth-generating-all-combinations/

            int n = values.Length;

            int t = k;

            int[] c = new int[t + 3];
            int[] result = new int[t];
            int j, x;

            for (j = 1; j <= t; j++)
                c[j] = j - 1;
            c[t + 1] = n;
            c[t + 2] = 0;

            j = t;

            do
            {
                for (int i = 0; i < t; i++)
                    result[i] = c[i + 1];
                yield return values.Submatrix(result);

                if (j > 0)
                {
                    x = j;
                }
                else
                {
                    if (c[1] + 1 < c[2])
                    {
                        c[1]++;
                        continue;
                    }
                    else
                    {
                        j = 2;
                    }
                }

                while (true)
                {
                    c[j - 1] = j - 2;
                    x = c[j] + 1;
                    if (x == c[j + 1]) j++;
                    else break;
                }

                c[j] = x;                 
                j--;
            } while (j < t);        

        }

        /// <summary>
        ///   Enumerates all possible value permutations for
        ///   a given array.
        /// </summary>
        /// 
        public static IEnumerable<T[]> Permutations<T>(T[] values)
        {
            int[] idx = Matrix.Indices(0, values.Length);

            int j, l;

            while (true)
            {
                for (j = values.Length - 2; j >= 0; j--)
                    if (idx[j + 1] > idx[j]) break;

                if (j == -1) yield break;

                for (l = values.Length - 1; l > j; l--)
                    if (idx[l] > idx[j]) break;

                int temp = idx[j];
                idx[j] = idx[l];
                idx[l] = temp;

                for (int i = j + 1; i < idx.Length; i++)
                {
                    if (i > idx.Length - i + j) break;
                    temp = idx[i];
                    idx[i] = idx[idx.Length - i + j];
                    idx[idx.Length - i + j] = temp;
                }

                yield return values.Submatrix(idx);
            }

        }


    }
}
