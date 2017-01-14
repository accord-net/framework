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

    /// <summary>
    ///   Kernel function interface.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In Machine Learning and statistics, a Kernel is a function that returns
    ///   the value of the dot product between the images of the two arguments.</para>
    ///   
    /// <para>  <c>k(x,y) = ‹S(x),S(y)›</c></para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://www.support-vector.net/icml-tutorial.pdf">
    ///     http://www.support-vector.net/icml-tutorial.pdf </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    public interface ILinear<T> : IKernel<T>
    {
        /// <summary>
        ///   Elementwise addition of a and b, storing in result.
        /// </summary>
        /// 
        /// <param name="a">The first vector to add.</param>
        /// <param name="b">The second vector to add.</param>
        /// <param name="result">An array to store the result.</param>
        /// <returns>The same vector passed as result.</returns>
        /// 
        void Add(T a, double[] b, double[] result);

        /// <summary>
        ///   Elementwise multiplication of scalar a and vector b, accumulating in result.
        /// </summary>
        /// 
        /// <param name="a">The scalar to be multiplied.</param>
        /// <param name="b">The vector to be multiplied.</param>
        /// <param name="accumulate">An array to store the result.</param>
        /// 
        void Product(double a, T b, double[] accumulate);

        /// <summary>
        ///   Elementwise multiplication of vector a and vector b, accumulating in result.
        /// </summary>
        /// 
        /// <param name="a">The vector to be multiplied.</param>
        /// <param name="b">The vector to be multiplied.</param>
        /// <param name="accumulate">An array to store the result.</param>
        /// 
        void Product(double[] a, T b, double[] accumulate);

        ///// <summary>
        /////   Elementwise multiplication of scalar a and vector b, accumulating in result.
        ///// </summary>
        ///// 
        ///// <param name="a">The scalar to be multiplied.</param>
        ///// <param name="b">The vector to be multiplied.</param>
        ///// <param name="accumulate">An array to store the result.</param>
        ///// 
        //void Product(double a, T b, T accumulate); 

        /// <summary>
        ///   Compress a set of support vectors and weights into a single
        ///   parameter vector.
        /// </summary>
        /// 
        /// <param name="weights">The weights associated with each support vector.</param>
        /// <param name="supportVectors">The support vectors.</param>
        /// <param name="c">The constant (bias) value.</param>
        /// 
        /// <returns>A single parameter vector.</returns>
        /// 
        T Compress(double[] weights, T[] supportVectors, out double c);


        /// <summary>
        ///   The kernel function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        double Function(double[] y, T x);

        /// <summary>
        ///   Gets the number of parameters in the input vectors.
        /// </summary>
        /// 
        int GetLength(T[] inputs);

        /// <summary>
        ///   Creates an input vector from the given double values.
        /// </summary>
        /// 
        T CreateVector(double[] values);

        ///// <summary>
        /////   Creates an input vector with the given dimensions.
        ///// </summary>
        ///// 
        //T CreateVector(int dimensions);

        /// <summary>
        ///   Converts the input vectors to a double-precision representation.
        /// </summary>
        /// 
        double[][] ToDouble(T[] input);
    }
}
