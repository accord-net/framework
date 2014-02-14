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
    using Accord.Math.Decompositions;

    /// <summary>
    ///   Static class Norm. Defines a set of extension methods defining norms measures.
    /// </summary>
    /// 
    public static class Norm
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
        ///   Returns the maximum singular value of the given matrix.
        /// </summary>
        /// 
        public static double Norm2(this double[,] a)
        {
            return new SingularValueDecomposition(a, false, false).TwoNorm;
        }

        /// <summary>
        ///   Gets the square root of the sum of squares for all elements in a matrix.
        /// </summary>
        /// 
        public static double Frobenius(this double[,] a)
        {
            if (a == null) 
                throw new ArgumentNullException("a");

            int rows = a.GetLength(0);
            int cols = a.GetLength(1);

            double norm = 0.0;
            for (int j = 0; j < cols; j++)
            {
                for (int i = 0; i < rows; i++)
                {
                    double v = a[i, j];
                    norm += v * v;
                }
            }

            return System.Math.Sqrt(norm);
        }

        /// <summary>
        ///   Gets the Squared Euclidean norm for a vector.
        /// </summary>
        /// 
        public static float SquareEuclidean(this float[] a)
        {
            float sum = 0;
            for (int i = 0; i < a.Length; i++)
                sum += a[i] * a[i];
            return sum;
        }

        /// <summary>
        ///   Gets the Squared Euclidean norm for a vector.
        /// </summary>
        /// 
        public static double SquareEuclidean(this double[] a)
        {
            double sum = 0.0;
            for (int i = 0; i < a.Length; i++)
                sum += a[i] * a[i];
            return sum;
        }

        /// <summary>
        ///   Gets the Euclidean norm for a vector.
        /// </summary>
        /// 
        public static float Euclidean(this float[] a)
        {
            return (float)Math.Sqrt(SquareEuclidean(a));
        }

        /// <summary>
        ///   Gets the Euclidean norm for a vector.
        /// </summary>
        /// 
        public static double Euclidean(this double[] a)
        {
            return System.Math.Sqrt(SquareEuclidean(a));
        }

        /// <summary>
        ///   Gets the Squared Euclidean norm vector for a matrix.
        /// </summary>
        /// 
        public static double[] SquareEuclidean(this double[,] a)
        {
            return SquareEuclidean(a, 0);
        }

        /// <summary>
        ///   Gets the Squared Euclidean norm vector for a matrix.
        /// </summary>
        /// 
        public static double[] SquareEuclidean(this double[,] a, int dimension)
        {
            int rows = a.GetLength(0);
            int cols = a.GetLength(1);
            
            double[] norm;

            if (dimension == 0)
            {
                norm = new double[cols];

                for (int j = 0; j < norm.Length; j++)
                {
                    double sum = 0.0;
                    for (int i = 0; i < rows; i++)
                    {
                        double v = a[i, j];
                        sum += v * v;
                    }
                    norm[j] = sum;
                }
            }
            else
            {
                norm = new double[rows];

                for (int i = 0; i < norm.Length; i++)
                {
                    double sum = 0.0;
                    for (int j = 0; j < cols; j++)
                    {
                        double v = a[i, j];
                        sum += v * v;
                    }
                    norm[i] = sum;
                }
            }

            return norm;
        }

        /// <summary>
        ///   Gets the Euclidean norm for a matrix.
        /// </summary>
        /// 
        public static double[] Euclidean(this double[,] a)
        {
            return Euclidean(a, 0);
        }

        /// <summary>
        ///   Gets the Euclidean norm for a matrix.
        /// </summary>
        /// 
        public static double[] Euclidean(this double[,] a, int dimension)
        {
            double[] norm = Norm.SquareEuclidean(a, dimension);

            for (int i = 0; i < norm.Length; i++)
                norm[i] = System.Math.Sqrt(norm[i]);

            return norm;
        }

        /// <summary>
        ///   Gets the Squared Euclidean norm vector for a matrix.
        /// </summary>
        /// 
        public static float[] SquareEuclidean(this float[,] a, int dimension)
        {
            int rows = a.GetLength(0);
            int cols = a.GetLength(1);

            float[] norm;

            if (dimension == 0)
            {
                norm = new float[cols];

                for (int j = 0; j < norm.Length; j++)
                {
                    float sum = 0;
                    for (int i = 0; i < rows; i++)
                    {
                        float v = a[i, j];
                        sum += v * v;
                    }
                    norm[j] = sum;
                }
            }
            else
            {
                norm = new float[rows];

                for (int i = 0; i < norm.Length; i++)
                {
                    float sum = 0;
                    for (int j = 0; j < cols; j++)
                    {
                        float v = a[i, j];
                        sum += v * v;
                    }
                    norm[i] = sum;
                }
            }

            return norm;
        }

        /// <summary>
        ///   Gets the Euclidean norm for a matrix.
        /// </summary>
        /// 
        public static float[] Euclidean(this float[,] a)
        {
            return Euclidean(a, 0);
        }

        /// <summary>
        ///   Gets the Euclidean norm for a matrix.
        /// </summary>
        /// 
        public static float[] Euclidean(this float[,] a, int dimension)
        {
            float[] norm = Norm.SquareEuclidean(a, dimension);

            for (int i = 0; i < norm.Length; i++)
                norm[i] = (float)System.Math.Sqrt(norm[i]);

            return norm;
        }

    }
}
