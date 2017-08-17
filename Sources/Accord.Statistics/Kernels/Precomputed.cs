// Accord Statistics Library
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

namespace Accord.Statistics.Kernels
{
    using System;
    using Accord.Math;
    using Accord.Compat;

    /// <summary>
    ///   Precomputed Gram Matrix Kernel.
    /// </summary>
    /// 
    /// <example>
    /// 
    /// <para>
    ///   The following example shows how to learn a multi-class SVM using
    ///   a precomputed kernel matrix, obtained from a Polynomial kernel.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MulticlassSupportVectorLearningTest.cs" region="doc_precomputed" />
    ///   
    /// <para>
    ///   The following example shows how to learn a simple binary SVM using
    ///    a precomputed kernel matrix obtained from a Gaussian kernel.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\SequentialMinimalOptimizationTest.cs" region="doc_precomputed" />
    /// </example>
    /// 
    [Serializable]
    public struct Precomputed : IKernel, IKernel<int>, ICloneable
    {
        private double[][] matrix;

        /// <summary>
        ///   Constructs a new Precomputed Matrix Kernel.
        /// </summary>
        /// 
        [Obsolete("Please use jagged matrices instead.")]
        public Precomputed(double[,] matrix)
        {
            this.matrix = matrix.ToJagged();
        }

        /// <summary>
        ///   Constructs a new Precomputed Matrix Kernel.
        /// </summary>
        /// 
        public Precomputed(double[][] matrix)
        {
            this.matrix = matrix;
        }

        /// <summary>
        ///   Gets or sets the precomputed Gram matrix for this kernel.
        /// </summary>
        /// 
        [Obsolete("Please use the Values property instead.")]
        public double[,] Matrix
        {
            get { return matrix.ToMatrix(); }
            set { matrix = value.ToJagged(); }
        }

        /// <summary>
        ///   Gets or sets the precomputed Gram matrix for this kernel.
        /// </summary>
        /// 
        public double[][] Values
        {
            get { return matrix; }
            set { matrix = value; }
        }

        /// <summary>
        /// Gets a vector of indices that can be fed as the inputs of a learning
        /// algorithm. The learning algorithm will then use the indices to refer 
        /// to each element in the precomputed kernel matrix.
        /// </summary>
        /// 
        public int[] Indices
        {
            get { return Vector.Range(0, NumberOfSamples); }
        }

        /// <summary>
        /// Gets the dimension of the basis spawned by the initial training vectors.
        /// </summary>
        /// 
        public int NumberOfBasisVectors
        {
            get { return matrix.Rows(); }
        }

        /// <summary>
        /// Gets the current number of training samples.
        /// </summary>
        /// 
        public int NumberOfSamples
        {
             get { return matrix.Columns(); }
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
            return matrix[i][j];
        }

        /// <summary>
        /// The kernel function.
        /// </summary>
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        public double Function(int x, int y)
        {
            return matrix[x][y];
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
            return new Precomputed(matrix.Copy());
        }

    }
}
