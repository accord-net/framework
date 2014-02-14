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

    /// <summary>
    ///   Reduced row Echelon form
    /// </summary>
    /// 
    public class ReducedRowEchelonForm
    {

        private double[,] rref;
        private int rows;
        private int cols;

        private int[] pivot;
        private int? freeCount;

        /// <summary>
        ///   Reduces a matrix to reduced row Echelon form.
        /// </summary>
        /// 
        /// <param name="value">The matrix to be reduced.</param>
        /// <param name="inPlace">
        ///   Pass <see langword="true"/> to perform the reduction in place. The matrix
        ///   <paramref name="value"/> will be destroyed in the process, resulting in less
        ///   memory consumption.</param>
        ///   
        public ReducedRowEchelonForm(double[,] value, bool inPlace = false)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            rref = inPlace ? value : (double[,])value.Clone();

            int lead = 0;
            rows = rref.GetLength(0);
            cols = rref.GetLength(1);

            pivot = new int[rows];
            for (int i = 0; i < pivot.Length; i++)
                pivot[i] = i;


            for (int r = 0; r < rows; r++)
            {
                if (cols <= lead)
                    break;

                int i = r;

                while (rref[i, lead] == 0)
                {
                    i++;

                    if (i >= rows)
                    {
                        i = r;

                        if (lead < cols - 1)
                            lead++;
                        else break;
                    }
                }

                if (i != r)
                {
                    // Swap rows i and r
                    for (int j = 0; j < cols; j++)
                    {
                        var temp = rref[r, j];
                        rref[r, j] = rref[i, j];
                        rref[i, j] = temp;
                    }

                    // Update indices
                    {
                        var temp = pivot[r];
                        pivot[r] = pivot[i];
                        pivot[i] = temp;
                    }
                }

                // Set to reduced row echelon form
                var div = rref[r, lead];
                if (div != 0)
                {
                    for (int j = 0; j < cols; j++)
                        rref[r, j] /= div;
                }

                for (int j = 0; j < rows; j++)
                {
                    if (j != r)
                    {
                        var sub = rref[j, lead];
                        for (int k = 0; k < cols; k++)
                            rref[j, k] -= (sub * rref[r, k]);
                    }
                }

                lead++;
            }
        }

        /// <summary>
        ///   Gets the pivot indicating the position
        ///   of the original rows before the swap.
        /// </summary>
        /// 
        public int[] Pivot { get { return pivot; } }

        /// <summary>
        ///   Gets the matrix in row reduced Echelon form.
        /// </summary>
        public double[,] Result { get { return rref; } }

        /// <summary>
        ///   Gets the number of free variables (linear
        ///   dependent rows) in the given matrix.
        /// </summary>
        public int FreeVariables
        {
            get
            {
                if (freeCount == null)
                    freeCount = count();

                return freeCount.Value;
            }
        }

        private int count()
        {
            for (int i = rows - 1; i >= 0; i--)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (rref[i, j] != 0)
                        return rows - i - 1;
                }
            }

            return 0;
        }

        

    }
}
