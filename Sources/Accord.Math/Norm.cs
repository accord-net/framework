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
    using Accord.Math.Decompositions;

    /// <summary>
    ///   Static class Norm. Defines a set of extension methods defining norms measures.
    /// </summary>
    /// 
    public static partial class Norm
    {
        /// <summary>
        ///   Returns the maximum column sum of the given matrix.
        /// </summary>
        /// 
        public static double Norm1(this double[,] a)
        {
            double[] columnSums = Matrix.Sum(a, 1);
            return Matrix.Max(columnSums);
        }

        /// <summary>
        ///   Returns the maximum column sum of the given matrix.
        /// </summary>
        /// 
        public static double Norm1(this double[][] a)
        {
            double[] columnSums = Matrix.Sum(a, 1);
            return Matrix.Max(columnSums);
        }

        /// <summary>
        ///   Returns the maximum singular value of the given matrix.
        /// </summary>
        /// 
        public static double Norm2(this double[,] a)
        {
            return new SingularValueDecomposition(a, false, false).TwoNorm;
        }

        /// <summary>
        ///   Returns the maximum singular value of the given matrix.
        /// </summary>
        /// 
        public static double Norm2(this double[][] a)
        {
            return new JaggedSingularValueDecomposition(a, false, false).TwoNorm;
        }

    }
}
