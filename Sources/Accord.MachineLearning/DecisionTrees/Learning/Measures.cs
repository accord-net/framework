// Accord Machine Learning Library
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

namespace Accord.MachineLearning.DecisionTrees.Learning
{
    using System;

    /// <summary>
    ///   Static class for common information measures.
    /// </summary>
    /// 
    public static class Measures
    {

        /// <summary>
        ///   Computes the split information measure.
        /// </summary>
        /// 
        /// <param name="samples">The total number of samples.</param>
        /// <param name="partitions">The partitioning.</param>
        /// <returns>The split information for the given partitions.</returns>
        /// 
        public static double SplitInformation(int samples, int[][] partitions)
        {
            double info = 0;

            for (int i = 0; i < partitions.Length; i++)
            {
                double p = (double)partitions[i].Length / samples;
                if (p != 0) info -= p * Math.Log(p, 2);
            }

            return info;
        }
    }
}
