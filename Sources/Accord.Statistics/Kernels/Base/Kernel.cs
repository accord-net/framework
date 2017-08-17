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
    using Accord.Math;
    using Accord.Math.Distances;
    using System;
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Extension methods for <see cref="IKernel">kernel functions</see>.
    /// </summary>
    /// 
    public static class Kernel
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
        public static double[,] ToMatrix2<TKernel, TInput>(this TKernel kernel, TInput[] x, TInput[] y, double[,] result = null)
            where TKernel : IKernel<TInput>
        {
            if (result == null)
                result = new double[x.Length, y.Length];

            for (int i = 0; i < x.Length; i++)
                for (int j = 0; j < y.Length; j++)
                    result[i, j] = kernel.Function(x[i], y[j]);

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
        public static double[][] ToJagged2<TKernel, TInput>(this TKernel kernel, TInput[] x, TInput[] y, double[][] result = null)
            where TKernel : IKernel<TInput>
        {
            if (result == null)
                result = Jagged.Create<double>(x.Length, y.Length);

            for (int i = 0; i < x.Length; i++)
                for (int j = 0; j < y.Length; j++)
                    result[i][j] = kernel.Function(x[i], y[j]);

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


        /// <summary>
        ///   Estimates the complexity parameter C, present in many SVM algorithms,
        ///   for a given kernel and a given data set by summing every element
        ///   on the diagonal of the kernel matrix and using an heuristic based
        ///   on it.
        /// </summary>
        /// 
        /// <param name="inputs">The input samples.</param>
        /// 
        /// <returns>A suitable value for C.</returns>
        /// 
        public static double EstimateComplexity(double[][] inputs)
        {
            return EstimateComplexity(new Linear(), inputs);
        }

        /// <summary>
        ///   Estimates the complexity parameter C, present in many SVM algorithms,
        ///   for a given kernel and an unbalanced data set by summing every element
        ///   on the diagonal of the kernel matrix and using an heuristic based on it.
        /// </summary>
        /// 
        /// <param name="inputs">The input samples.</param>
        /// <param name="outputs">The output samples.</param>
        /// 
        /// <returns>A suitable value for positive C and negative C, respectively.</returns>
        /// 
        public static Tuple<double, double> EstimateComplexity(double[][] inputs, int[] outputs)
        {
            return EstimateComplexity(new Linear(), inputs, outputs);
        }

        /// <summary>
        ///   Computes the set of all distances between 
        ///   all points in a random subset of the data.
        /// </summary>
        /// 
        /// <param name="kernel">The inner kernel.</param>
        /// <param name="inputs">The inputs points.</param>
        /// <param name="samples">The number of samples.</param>
        /// 
        public static double[] Distances<T>(this T kernel, double[][] inputs, int samples)
            where T : IDistance, ICloneable
        {
            int[] idx = Vector.Sample(samples, inputs.Length);
            int[] idy = Vector.Sample(samples, inputs.Length);

            double[] distances = new double[samples * samples];

            for (int i = 0; i < idx.Length; i++)
            {
                double[] x = inputs[idx[i]];

                for (int j = 0; j < idy.Length; j++)
                {
                    double[] y = inputs[idy[j]];

                    distances[i * samples + j] = kernel.Distance(x, y);
                }
            }

            Array.Sort(distances);

            return distances;
        }

        /// <summary>
        ///   Computes the set of all distances between 
        ///   all points in a random subset of the data.
        /// </summary>
        /// 
        /// <param name="kernel">The inner kernel.</param>
        /// <param name="inputs">The inputs points.</param>
        /// <param name="samples">The number of samples.</param>
        /// 
        public static double[] Distances<TKernel, TData>(this TKernel kernel, TData[] inputs, int samples)
            where TKernel : IDistance<TData>, ICloneable
        {
            int[] idx = Vector.Sample(samples, inputs.Length);
            int[] idy = Vector.Sample(samples, inputs.Length);

            double[] distances = new double[samples * samples];

            for (int i = 0; i < idx.Length; i++)
            {
                TData x = inputs[idx[i]];

                for (int j = 0; j < idy.Length; j++)
                {
                    TData y = inputs[idy[j]];

                    distances[i * samples + j] = kernel.Distance(x, y);
                }
            }

            Array.Sort(distances);

            return distances;
        }

        /// <summary>
        ///   Centers the given kernel matrix K.
        /// </summary>
        /// 
        /// <param name="kernelMatrix">The kernel matrix to be centered.</param>
        /// <param name="result">The array where to store results.</param>
        /// 
        public static double[,] Center(double[,] kernelMatrix, double[,] result = null)
        {
            if (result == null)
                result = Matrix.CreateAs(kernelMatrix);

            double[] rowMean = kernelMatrix.Mean(dimension: 1);
#if DEBUG
            // row mean and column means should be equal on a symmetric matrix
            double[] colMean = kernelMatrix.Mean(dimension: 0);
            for (int i = 0; i < colMean.Length; i++)
                Accord.Diagnostics.Debug.Assert(colMean[i] == rowMean[i]);
#endif
            double mean = kernelMatrix.Mean();

            for (int i = 0; i < rowMean.Length; i++)
                for (int j = i; j < rowMean.Length; j++)
                    result[i, j] = result[j, i] = kernelMatrix[i, j] - rowMean[i] - rowMean[j] + mean;

            return result;
        }

        /// <summary>
        ///   Centers the given kernel matrix K.
        /// </summary>
        /// 
        /// <param name="kernelMatrix">The kernel matrix to be centered.</param>
        /// <param name="rowMean">The row-wise mean vector.</param>
        /// <param name="mean">The total mean (across all values in the matrix).</param>
        /// <param name="result">The array where to store results.</param>
        /// 
        public static double[][] Center(double[][] kernelMatrix, out double[] rowMean, out double mean, double[][] result = null)
        {
            if (result == null)
                result = Jagged.CreateAs(kernelMatrix);

            rowMean = kernelMatrix.Mean(dimension: 1);
#if DEBUG
            // row mean and column means should be equal on a symmetric matrix
            double[] colMean = kernelMatrix.Mean(dimension: 0);
            for (int i = 0; i < colMean.Length; i++)
                Accord.Diagnostics.Debug.Assert(colMean[i] == rowMean[i]);
#endif
            mean = kernelMatrix.Mean(); // TODO: This should become simply mean = K.Mean()

            for (int i = 0; i < rowMean.Length; i++)
                for (int j = i; j < rowMean.Length; j++)
                    result[i][j] = result[j][i] = kernelMatrix[i][j] - rowMean[i] - rowMean[j] + mean;

            return result;
        }

        /// <summary>
        ///   Centers the given kernel matrix K.
        /// </summary>
        /// 
        /// <param name="kernelMatrix">The kernel matrix to be centered.</param>
        /// <param name="rowMean">The row-wise mean vector.</param>
        /// <param name="mean">The total mean (across all values in the matrix).</param>
        /// <param name="result">The array where to store results.</param>
        /// 
        public static double[,] Center(double[,] kernelMatrix, double[] rowMean, double mean, double[,] result = null)
        {
            if (result == null)
                result = Matrix.CreateAs(kernelMatrix);

            int rows = kernelMatrix.Rows();
            int cols = kernelMatrix.Columns();

            double[] rowMean1 = kernelMatrix.Mean(1);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i, j] = kernelMatrix[i, j] - rowMean1[i] - rowMean[j] + mean;

            return result;
        }

        /// <summary>
        ///   Centers the given kernel matrix K.
        /// </summary>
        /// 
        /// <param name="kernelMatrix">The kernel matrix to be centered.</param>
        /// <param name="rowMean">The row-wise mean vector.</param>
        /// <param name="mean">The total mean (across all values in the matrix).</param>
        /// <param name="result">The array where to store results.</param>
        /// 
        public static double[][] Center(double[][] kernelMatrix, double[] rowMean, double mean, double[][] result = null)
        {
            if (result == null)
                result = Jagged.CreateAs(kernelMatrix);

#if DEBUG
            double[,] r = Center(kernelMatrix.ToMatrix(), rowMean, mean);
#endif

            int cols = kernelMatrix.Columns();
             
            double[] rowMean1 = kernelMatrix.Mean(1);
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < kernelMatrix[i].Length; j++)
                    result[i][j] = kernelMatrix[i][j] - rowMean1[i] - rowMean[j] + mean;

#if DEBUG
            if (!r.IsEqual(result, 1e-8))
                throw new Exception();
#endif

            return result;
        }
    }
}
