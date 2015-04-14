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
    using System;

    // TODO: Make it public and obsolete the previous equivalent methods
    internal class Indices
    {


        /// <summary>
        ///   Returns a random sample of size k from a population of size n.
        /// </summary>
        /// 
        public static int[] Random(int n, int k)
        {
            if (k > n)
            {
                throw new ArgumentOutOfRangeException("k",
                    "The sample size must be less than the size of the population.");
            }

            int[] idx = Random(n);
            return idx.Submatrix(k);
        }

      

        /// <summary>
        ///   Returns a random permutation of all indices from 0 to n.
        /// </summary>
        /// 
        public static int[] Random(int n)
        {
            var random = Accord.Math.Random.Generator.Random;

            int[] idx = new int[n];
            for (int i = 0; i < idx.Length; i++)
                idx[i] = i;

            double[] x = new double[n];
            for (int i = 0; i < x.Length; i++)
                x[i] = random.NextDouble();

            Array.Sort(x, idx);

            return idx;
        }
    }
}
