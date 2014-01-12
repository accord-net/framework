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
        /// <example>
        /// <para>
        ///   Suppose we would like to generate a truth table for a binary
        ///   problem. In this case, we are only interested in two symbols:
        ///   0 and 1. Let's then generate the table for three binary values</para>
        /// 
        /// <code>
        /// int length = 3;  // The number of variables; or number 
        ///                  // of columns in the generated table.
        /// 
        /// // Generate the table using Combinatorics.TruthTable(3)
        /// int[][] table = Combinatorics.TruthTable(length);
        /// 
        /// // The generated table will be:
        /// {
        ///     new int[] { 0, 0, 0 },
        ///     new int[] { 0, 0, 1 },
        ///     new int[] { 0, 1, 0 },
        ///     new int[] { 0, 1, 1 },
        ///     new int[] { 1, 0, 0 },
        ///     new int[] { 1, 0, 1 },
        ///     new int[] { 1, 1, 0 },
        ///     new int[] { 1, 1, 1 },
        /// };
        /// </code>
        /// </example>
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
        /// <example>
        /// <para>
        ///   Suppose we would like to generate a truth table for a binary
        ///   problem. In this case, we are only interested in two symbols:
        ///   0 and 1. Let's then generate the table for three binary values</para>
        /// 
        /// <code>
        /// int symbols = 2; // Binary variables: either 0 or 1
        /// int length = 3;  // The number of variables; or number 
        ///                  // of columns in the generated table.
        /// 
        /// // Generate the table using Combinatorics.TruthTable(2,3)
        /// int[][] table = Combinatorics.TruthTable(symbols, length);
        /// 
        /// // The generated table will be:
        /// {
        ///     new int[] { 0, 0, 0 },
        ///     new int[] { 0, 0, 1 },
        ///     new int[] { 0, 1, 0 },
        ///     new int[] { 0, 1, 1 },
        ///     new int[] { 1, 0, 0 },
        ///     new int[] { 1, 0, 1 },
        ///     new int[] { 1, 1, 0 },
        ///     new int[] { 1, 1, 1 },
        /// };
        /// </code>
        /// </example>
        /// 
        public static int[][] TruthTable(int symbols, int length)
        {
            int[] sym = new int[length];
            for (int i = 0; i < sym.Length; i++)
                sym[i] = symbols;

            return TruthTable(sym);
        }

        /// <summary>
        ///   Generates all possible ordered permutations
        ///   with repetitions allowed (a truth table).
        /// </summary>
        /// 
        /// <param name="symbols">The number of symbols for each variable.</param>
        /// 
        /// <example>
        /// <para>
        ///   Suppose we would like to generate a truth table (i.e. all possible
        ///   combinations of a set of discrete symbols) for variables that contain
        ///   different numbers symbols. Let's say, for example, that the first 
        ///   variable may contain symbols 0 and 1, the second could contain either
        ///   0, 1, or 2, and the last one again could contain only 0 and 1. Thus
        ///   we can generate the truth table in the following way: </para>
        /// 
        /// <code>
        /// // Number of symbols for each variable
        /// int[] symbols = { 2, 3, 2 };
        /// 
        /// // Generate the truth table for the given symbols
        /// int[][] table = Combinatorics.TruthTable(symbols);
        /// 
        /// // The generated table will be:
        /// {
        ///     new int[] { 0, 0, 0 },
        ///     new int[] { 0, 0, 1 },
        ///     new int[] { 0, 1, 0 },
        ///     new int[] { 0, 1, 1 },
        ///     new int[] { 0, 2, 0 },
        ///     new int[] { 0, 2, 1 },
        ///     new int[] { 1, 0, 0 },
        ///     new int[] { 1, 0, 1 },
        ///     new int[] { 1, 1, 0 },
        ///     new int[] { 1, 1, 1 },
        ///     new int[] { 1, 2, 0 },
        ///     new int[] { 1, 2, 1 },
        /// };
        /// </code></example>
        ///
        public static int[][] TruthTable(int[] symbols)
        {
            int size = 1;
            for (int i = 0; i < symbols.Length; i++)
                size *= symbols[i];

            int[][] sequences = new int[size][];
            sequences[0] = new int[symbols.Length];

            for (int i = 1; i < sequences.Length; i++)
            {
                var row = sequences[i] = (int[])sequences[i - 1].Clone();

                for (int j = symbols.Length - 1; j >= 0; j--)
                {
                    if (row[j] < symbols[j] - 1)
                    {
                        row[j]++;
                        break;
                    }

                    row[j] = 0;
                }
            }

            return sequences;
        }

        /// <summary>
        ///   Provides a way to enumerate all possible ordered permutations
        ///   with repetitions allowed (i.e. a truth table), without using
        ///   many memory allocations.
        /// </summary>
        /// 
        /// <param name="symbols">The number of symbols.</param>
        /// <param name="length">The length of the sequence to generate.</param>
        /// 
        /// <example>
        /// <para>
        ///   Suppose we would like to generate the same sequences shown
        ///   in the <see cref="Combinatorics.TruthTable(int,int)"/>example,
        ///   however, without explicitly storing all possible combinations
        ///   in an array. In order to iterate over all possible combinations
        ///   efficiently, we can use:
        /// </para>
        /// 
        /// <code>
        /// int symbols = 2; // Binary variables: either 0 or 1
        /// int length = 3;  // The number of variables; or number 
        ///                  // of columns in the generated table.
        /// 
        /// foreach (int[] row in Combinatorics.Sequences(symbols, length))
        /// {
        ///     // The following sequences will be generated in order:
        ///     //
        ///     //   new int[] { 0, 0, 0 },
        ///     //   new int[] { 0, 0, 1 },
        ///     //   new int[] { 0, 1, 0 },
        ///     //   new int[] { 0, 1, 1 },
        ///     //   new int[] { 1, 0, 0 },
        ///     //   new int[] { 1, 0, 1 },
        ///     //   new int[] { 1, 1, 0 },
        ///     //   new int[] { 1, 1, 1 },
        /// }
        /// </code>
        /// </example>
        /// 
        public static IEnumerable<int[]> Sequences(int symbols, int length)
        {
            int[] sym = new int[length];
            for (int i = 0; i < sym.Length; i++)
                sym[i] = symbols;

            return Sequences(sym);
        }

        /// <summary>
        ///   Provides a way to enumerate all possible ordered permutations
        ///   with repetitions allowed (i.e. a truth table), without using
        ///   many memory allocations.
        /// </summary>
        /// 
        /// <param name="symbols">The number of symbols for each variable.</param>
        /// 
        /// <example>
        /// <para>
        ///   Suppose we would like to generate the same sequences shown
        ///   in the <see cref="Combinatorics.TruthTable(int,int)"/>example,
        ///   however, without explicitly storing all possible combinations
        ///   in an array. In order to iterate over all possible combinations
        ///   efficiently, we can use:
        /// </para>
        /// 
        /// <code>
        /// foreach (int[] row in Combinatorics.Sequences(new[] { 2, 2 }))
        /// {
        ///     // The following sequences will be generated in order:
        ///     //
        ///     //   new int[] { 0, 0, 0 },
        ///     //   new int[] { 0, 0, 1 },
        ///     //   new int[] { 0, 1, 0 },
        ///     //   new int[] { 0, 1, 1 },
        ///     //   new int[] { 1, 0, 0 },
        ///     //   new int[] { 1, 0, 1 },
        ///     //   new int[] { 1, 1, 0 },
        ///     //   new int[] { 1, 1, 1 },
        /// }
        /// </code>
        /// </example>
        /// 
        public static IEnumerable<int[]> Sequences(int[] symbols)
        {
            var current = new int[symbols.Length];

            while (true)
            {
                yield return current;

                bool match = true;
                for (int i = 0; i < current.Length && match; i++)
                    if (current[i] != symbols[i] - 1)
                        match = false;

                if (match)
                    break;

                for (int j = symbols.Length - 1; j >= 0; j--)
                {
                    if (current[j] < symbols[j] - 1)
                    {
                        current[j]++;
                        break;
                    }

                    current[j] = 0;
                }
            }
        }

        /// <summary>
        ///   Enumerates all possible value combinations for a given array.
        /// </summary>
        /// 
        /// <param name="values">The array whose combinations need to be generated.</param>
        /// <param name="k">The length of the combinations to be generated.</param>
        /// 
        /// <example>
        /// <code>
        ///   // Let's say we would like to generate all possible combinations
        ///   // of the elements (1, 2, 3). In order to enumerate all those
        ///   // combinations, we can use:
        /// 
        ///   int[] values = { 1, 2, 3 };
        ///   
        ///   foreach (int[] combination in Combinatorics.Combinations(values))
        ///   {
        ///       // The combinations will be generated in the following order:
        ///       //
        ///       //   { 1, 2 };
        ///       //   { 1, 3 };
        ///       //   { 2, 3 };
        ///       //
        ///   }
        /// </code>
        /// </example>
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
        ///   Enumerates all possible value permutations for a given array.
        /// </summary>
        /// 
        /// <param name="values">The array whose permutations need to be generated</param>.
        /// 
        /// <example>
        /// <code>
        ///   // Let's say we would like to generate all possible permutations
        ///   // of the elements (1, 2, 3). In order to enumerate all those
        ///   // permutations, we can use:
        /// 
        ///   int[] values = { 1, 2, 3 };
        ///   
        ///   foreach (int[] permutation in Combinatorics.Permutations(values))
        ///   {
        ///       // The permutations will be generated in the following order:
        ///       //
        ///       //   { 1, 3, 2 };
        ///       //   { 2, 1, 3 };
        ///       //   { 2, 3, 1 };
        ///       //   { 3, 1, 2 };
        ///       //   { 3, 2, 1 };
        ///       //
        ///   }
        /// </code>
        /// </example>
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
