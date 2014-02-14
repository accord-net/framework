// Accord Statistics Library
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

namespace Accord.Statistics.Kernels
{
    using System;

    /// <summary>
    ///   Precomputed Gram Matrix Kernel.
    /// </summary>
    /// 
    [Serializable]
    public sealed class Precomputed : IKernel, ICloneable
    {
        private double[,] matrix;

        /// <summary>
        ///   Constructs a new Precomputed Matrix Kernel.
        /// </summary>
        /// 
        public Precomputed(double[,] matrix)
        {
            this.matrix = matrix;
        }

        /// <summary>
        ///   Gets or sets the precomputed Gram matrix for this kernel.
        /// </summary>
        /// 
        public double[,] Matrix
        {
            get { return matrix; }
            set { matrix = value; }
        }

        /// <summary>
        ///   Kernel function.
        /// </summary>
        /// 
        /// <param name="x">An array containing a first element with the index for input vector <c>x</c>.</param>
        /// <param name="y">An array containing a first element with the index for input vector <c>y</c>.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double[] x, double[] y)
        {
            int i = (int)x[0];
            int j = (int)y[0];

            return matrix[i, j];
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public object Clone()
        {
            return new Precomputed((double[,])matrix.Clone());
        }
    }
}
