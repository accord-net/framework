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
    using Accord.Compat;

    /// <summary>
    ///   Tensor Product combination of Kernels.
    /// </summary>
    /// 
    /// <example>
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\Kernels\TensorTest.cs" region="doc_learn" />
    /// </example>
    /// 
    [Serializable]
    public struct Tensor<T> : IKernel
        where T : IKernel
    {
        private T[] kernels;

        /// <summary>
        ///   Gets or sets the inner kernels used in this tensor kernel.
        /// </summary>
        /// 
        public T[] Kernels
        {
            get { return kernels; }
            set { kernels = value; }
        }

        /// <summary>
        ///   Constructs a new additive kernel.
        /// </summary>
        /// 
        /// <param name="kernels">Kernels to combine.</param>
        /// 
        public Tensor(params T[] kernels)
        {
            this.kernels = kernels;
        }

        /// <summary>
        ///   Tensor Product Kernel Combination function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double[] x, double[] y)
        {
            double product = 1.0;
            for (int i = 0; i < kernels.Length; i++)
                product *= kernels[i].Function(x, y);
            return product;
        }

    }
}
