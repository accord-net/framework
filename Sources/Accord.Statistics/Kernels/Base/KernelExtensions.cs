﻿// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    using Accord.Math;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    
    /// <summary>
    ///   Extension methods for <see cref="IKernel">kernel functions</see>.
    /// </summary>
    /// 
    public static class KernelExtensions
    {
        /// <summary>
        ///   Creates the Gram matrix from the given vectors.
        /// </summary>
        /// 
        /// <param name="kernel">The kernel function.</param>
        /// <param name="x">The vectors.</param>
        /// 
        /// <param name="result">An optional matrix where the result should be stored in.</param>
        ///   
        /// <returns>A symmetric matrix containing the dot-products in
        ///   feature (kernel) space between all vectors in <paramref name="x"/>.</returns>
        ///   
        public static double[,] ToMatrix<TKernel, TInput>(this TKernel kernel, TInput[] x, double[,] result = null)
            where TKernel : IKernel<TInput>
        {
            if (result == null)
                result = new double[x.Length, x.Length];

            for (int i = 0; i < x.Length; i++)
                for (int j = i; j < x.Length; j++)
                    result[j, i] = result[i, j] = kernel.Function(x[i], x[j]);

            return result;
        }

        /// <summary>
        ///   Creates the Gram matrix containing all dot products in feature
        ///   (kernel) space between each vector in <paramref name="x">x</paramref>
        ///   and the ones in <paramref name="y">y</paramref>.
        /// </summary>
        /// 
        /// <param name="x">The first vectors.</param>
        /// <param name="y">The second vectors.</param>
        /// 
        /// <param name="kernel">The kernel function.</param>
        /// <param name="result">An optional matrix where the result should be stored in.</param>
        ///   
        /// <returns>A symmetric matrix containing the dot-products in
        ///   feature (kernel) space between each vector in <paramref name="x"/>
        ///   and the ones in <paramref name="y"/>.</returns>
        ///   
        public static double[,] ToMatrix<TKernel, TInput>(this TKernel kernel, TInput[] x, TInput[] y, double[,] result = null)
            where TKernel : IKernel<TInput>
        {
            if (result == null)
                result = new double[x.Length, y.Length];

            for (int i = 0; i < x.Length; i++)
                for (int j = i; j < x.Length; j++)
                    result[j, i] = result[i, j] = kernel.Function(x[i], y[j]);

            return result;
        }

        /// <summary>
        ///   Creates the Gram matrix from the given vectors.
        /// </summary>
        /// 
        /// <param name="kernel">The kernel function.</param>
        /// <param name="x">The vectors.</param>
        /// 
        /// <param name="result">An optional matrix where the result should be stored in.</param>
        ///   
        /// <returns>A symmetric matrix containing the dot-products in
        ///   feature (kernel) space between all vectors in <paramref name="x"/>.</returns>
        ///   
        public static double[][] ToJagged<TKernel, TInput>(this TKernel kernel, TInput[] x, double[][] result = null)
            where TKernel : IKernel<TInput>
        {
            if (result == null)
                result = Jagged.Create<double>(x.Length, x.Length);

            for (int i = 0; i < x.Length; i++)
                for (int j = i; j < x.Length; j++)
                    result[j][i] = result[i][j] = kernel.Function(x[i], x[j]);

            return result;
        }

        /// <summary>
        ///   Creates the Gram matrix containing all dot products in feature
        ///   (kernel) space between each vector in <paramref name="x">x</paramref>
        ///   and the ones in <paramref name="y">y</paramref>.
        /// </summary>
        /// 
        /// <param name="kernel">The kernel function.</param>
        /// <param name="x">The first vectors.</param>
        /// <param name="y">The second vectors.</param>
        /// 
        /// <param name="result">An optional matrix where the result should be stored in.</param>
        ///   
        /// <returns>A symmetric matrix containing the dot-products in
        ///   feature (kernel) space between each vector in <paramref name="x"/>
        ///   and the ones in <paramref name="y"/>.</returns>
        ///   
        public static double[][] ToJagged<TKernel, TInput>(this TKernel kernel, TInput[] x, TInput[] y, double[][] result = null)
            where TKernel : IKernel<TInput>
        {
            if (result == null)
                result = Jagged.Create<double>(x.Length, y.Length);

            for (int i = 0; i < x.Length; i++)
                for (int j = i; j < x.Length; j++)
                    result[j][i] = result[i][j] = kernel.Function(x[i], y[j]);

            return result;
        }

        /// <summary>
        ///   Estimates the complexity parameter C, present in many SVM algorithms,
        ///   for a given kernel and a given data set by summing every element
        ///   on the diagonal of the kernel matrix and using an heuristic based
        ///   on it.
        /// </summary>
        /// 
        /// <param name="kernel">The kernel function.</param>
        /// <param name="inputs">The input samples.</param>
        /// 
        /// <returns>A suitable value for C.</returns>
        /// 
        public static double EstimateComplexity<TKernel, TInput>(this TKernel kernel, TInput[] inputs)
            where TKernel : IKernel<TInput>
        {
            // Compute initial value for C as the number of examples
            // divided by the trace of the input sample kernel matrix.

            double sum = 0.0;
            for (int i = 0; i < inputs.Length; i++)
            {
                sum += kernel.Function(inputs[i], inputs[i]);

                if (Double.IsNaN(sum))
                    throw new OverflowException();
            }

            return inputs.Length / sum;
        }

        /// <summary>
        ///   Estimates the complexity parameter C, present in many SVM algorithms,
        ///   for a given kernel and an unbalanced data set by summing every element
        ///   on the diagonal of the kernel matrix and using an heuristic based on it.
        /// </summary>
        /// 
        /// <param name="kernel">The kernel function.</param>
        /// <param name="inputs">The input samples.</param>
        /// <param name="outputs">The output samples.</param>
        /// 
        /// <returns>A suitable value for positive C and negative C, respectively.</returns>
        /// 
        public static Tuple<double, double> EstimateComplexity<TKernel, TInput>(this TKernel kernel, TInput[] inputs, int[] outputs)
            where TKernel : IKernel<TInput>
        {
            // Compute initial value for C as the number of examples
            // divided by the trace of the input sample kernel matrix.

            double negativeSum = 0.0;
            double positiveSum = 0.0;

            int negativeCount = 0;
            int positiveCount = 0;

            for (int i = 0; i < inputs.Length; i++)
            {
                if (outputs[i] == -1)
                {
                    negativeSum += kernel.Function(inputs[i], inputs[i]);
                    negativeCount++;
                }
                else // outputs[i] == +1
                {
                    positiveSum += kernel.Function(inputs[i], inputs[i]);
                    positiveCount++;
                }

                if (Double.IsNaN(positiveSum) || Double.IsNaN(negativeSum))
                    throw new OverflowException();
            }

            return Tuple.Create
            (
                positiveCount / positiveSum,
                negativeCount / negativeSum
            );
        }

    }
}
