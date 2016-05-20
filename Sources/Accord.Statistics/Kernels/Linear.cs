// Accord Statistics Library
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

namespace Accord.Statistics.Kernels
{
    using Accord.Math;
    using Accord.Math.Distances;
    using System;

    /// <summary>
    ///   Linear Kernel.
    /// </summary>
    /// 
    [Serializable]
    public struct Linear : IKernel, IDistance, ILinear,
        ICloneable, IReverseDistance, ITransform,
        IKernel<Sparse<double>>, ILinear<Sparse<double>>
    {
        private double constant;

        /// <summary>
        ///   Constructs a new Linear kernel.
        /// </summary>
        /// 
        /// <param name="constant">A constant intercept term. Default is 0.</param>
        /// 
        public Linear(double constant)
        {
            this.constant = constant;
        }

        /// <summary>
        ///   Gets or sets the kernel's intercept term. Default is 0.
        /// </summary>
        /// 
        public double Constant
        {
            get { return constant; }
            set { constant = value; }
        }

        /// <summary>
        ///   Linear kernel function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// 
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double[] x, double[] y)
        {
            double sum = constant;
            for (int i = 0; i < y.Length; i++)
                sum += x[i] * y[i];

            return sum;
        }

        /// <summary>
        ///   Linear kernel function.
        /// </summary>
        /// 
        /// <param name="z">Distance <c>z</c> in input space.</param>
        /// 
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double z)
        {
            return z + constant;
        }

        /// <summary>
        ///   Computes the squared distance in input space
        ///   between two points given in feature space.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in feature (kernel) space.</param>
        /// <param name="y">Vector <c>y</c> in feature (kernel) space.</param>
        /// 
        /// <returns>Squared distance between <c>x</c> and <c>y</c> in input space.</returns>
        /// 
        public double Distance(double[] x, double[] y)
        {
            if (x == y)
                return 0.0;

            double sum = 0;
            for (int i = 0; i < x.Length; i++)
            {
                double u = x[i] - y[i];
                sum += u * u;
            }

            return sum;
        }

        /// <summary>
        ///   Computes the squared distance in input space
        ///   between two points given in feature space.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in feature (kernel) space.</param>
        /// <param name="y">Vector <c>y</c> in feature (kernel) space.</param>
        /// 
        /// <returns>
        ///   Squared distance between <c>x</c> and <c>y</c> in input space.
        /// </returns>
        /// 
        public double ReverseDistance(double[] x, double[] y)
        {
            double sumx = constant;
            double sumy = constant;
            double sum = constant;

            for (int i = 0; i < x.Length; i++)
            {
                sumx += x[i] * x[i];
                sumy += y[i] * y[i];
                sum += x[i] * y[i];
            }

            return sumx + sumy - 2.0 * sum;
        }

        /// <summary>
        ///   Elementwise addition of a and b, storing in result.
        /// </summary>
        /// 
        /// <param name="a">The first vector to add.</param>
        /// <param name="b">The second vector to add.</param>
        /// <param name="result">An array to store the result.</param>
        /// <returns>The same vector passed as result.</returns>
        /// 
        public double[] Add(double[] a, double[] b, double[] result)
        {
            for (int i = 0; i < a.Length; i++)
                result[i] = a[i] + b[i];
            return result;
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
            return MemberwiseClone();
        }


        /// <summary>
        ///   Elementwise multiplication of scalar a and vector b, storing in result.
        /// </summary>
        /// 
        /// <param name="a">The scalar to be multiplied.</param>
        /// <param name="b">The vector to be multiplied.</param>
        /// <param name="result">An array to store the result.</param>
        /// 
        public void Product(double a, double[] b, double[] result)
        {
            //double s2 = 0;
            for (int j = 0; j < b.Length; j++)
            {
                //s2 += val * val; // TODO: Remove the s2 calculation
                result[j] += a * b[j];
            }

            //return s2;
        }

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
        public double[] Compress(double[] weights, double[][] supportVectors, out double c)
        {
            double[] result = Matrix.Dot(weights, supportVectors);
            c = -constant;
            return result;
        }

        /// <summary>
        ///   Projects an input point into feature space.
        /// </summary>
        /// 
        /// <param name="input">The input point to be projected into feature space.</param>
        /// 
        /// <returns>
        ///   The feature space representation of the given <paramref name="input"/> point.
        /// </returns>
        /// 
        public double[] Transform(double[] input)
        {
            return Transform(input, constant);
        }

        /// <summary>
        ///   Projects an input point into feature space.
        /// </summary>
        /// 
        /// <param name="input">The input point to be projected into feature space.</param>
        /// <param name="constant">The <see cref="Constant"/> parameter of the kernel.</param>
        /// 
        /// <returns>
        ///   The feature space representation of the given <paramref name="input"/> point.
        /// </returns>
        /// 
        public static double[] Transform(double[] input, double constant)
        {
            if (constant == 0)
                return input;

            var feature = new double[input.Length + 3];
            for (int i = 0; i < input.Length; i++)
                feature[i] = input[i];

            feature[input.Length] = constant;

            return feature;
        }

        /// <summary>
        /// The kernel function.
        /// </summary>
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>
        /// Dot product in feature (kernel) space.
        /// </returns>
        public double Function(Sparse<double> x, Sparse<double> y)
        {
            return x.Dot(y) + constant;
        }

        /// <summary>
        /// The kernel function.
        /// </summary>
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>
        /// Dot product in feature (kernel) space.
        /// </returns>
        public double Function(double[] y, Sparse<double> x)
        {
            return x.Dot(y) + constant;
        }

        /// <summary>
        ///   Elementwise addition of a and b, storing in result.
        /// </summary>
        /// 
        /// <param name="a">The first vector to add.</param>
        /// <param name="b">The second vector to add.</param>
        /// <param name="result">An array to store the result.</param>
        /// <returns>The same vector passed as result.</returns>
        /// 
        public double[] Add(Sparse<double> a, double[] b, double[] result)
        {
            int i = 0;
            for (int j = 0; j < a.Indices.Length; j++)
            {
                if (a.Indices[j] == i)
                {
                    result[i] += b[i] + a.Values[j];
                }
                else
                {
                    result[i] += b[i];
                }

                i++;
            }

            return result;
        }

        /// <summary>
        /// Elementwise multiplication of scalar a and vector b, storing in result.
        /// </summary>
        /// <param name="a">The scalar to be multiplied.</param>
        /// <param name="b">The vector to be multiplied.</param>
        /// <param name="result">An array to store the result.</param>
        public void Product(double a, Sparse<double> b, double[] result)
        {
            for (int j = 0; j < b.Indices.Length; j++)
            {
                int i = b.Indices[j];
                result[i] += a * b.Values[j];
            }
        }

        /// <summary>
        /// Elementwise multiplication of scalar a and vector b, storing in result.
        /// </summary>
        /// <param name="a">The scalar to be multiplied.</param>
        /// <param name="b">The vector to be multiplied.</param>
        /// <param name="result">An array to store the result.</param>
        public void Product(double a, Sparse<double> b, Sparse<double> result)
        {
            int n = result.Indices.Length;
            bool seq = true;
            for (int i = 0; i < result.Indices.Length; i++)
            {
                if (result.Indices[i] != i)
                {
                    seq = false;
                    break;
                }
            }

            int m = b.Indices.Length;
            int max = Math.Max(result.Indices[n - 1], b.Indices[m - 1]);

            if (!seq || result.Indices[n - 1] < b.Indices[m - 1])
            {
                result.Values = result.ToDense(max + 1);
                result.Indices = Vector.Range(max + 1);
            }

            for (int j = 0; j < b.Indices.Length; j++)
            {
                int i = b.Indices[j];
                result.Values[i] += a * b.Values[j];
            }
        }

        /// <summary>
        /// Compress a set of support vectors and weights into a single
        /// parameter vector.
        /// </summary>
        /// <param name="weights">The weights associated with each support vector.</param>
        /// <param name="supportVectors">The support vectors.</param>
        /// <param name="c">The constant (bias) value.</param>
        /// <returns>
        /// A single parameter vector.
        /// </returns>
        public Sparse<double> Compress(double[] weights, Sparse<double>[] supportVectors, out double c)
        {
            return Accord.Math.Sparse.FromDense(Compress(weights, supportVectors.ToDense(), out c));
        }

        
    }
}
