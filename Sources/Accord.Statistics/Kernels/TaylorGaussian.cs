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
    using Accord.Math.Distances;
    using Accord.Compat;

    /// <summary>
    ///   Taylor approximation for the explicit Gaussian kernel.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///      Lin, Keng-Pei, and Ming-Syan Chen. "Efficient kernel approximation for large-scale support 
    ///      vector machine classification." Proceedings of the Eleventh SIAM International Conference on
    ///      Data Mining. 2011. Available on: http://epubs.siam.org/doi/pdf/10.1137/1.9781611972818.19 
    ///      </description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    [Serializable]
    public struct TaylorGaussian : ITransform, ILinear, IReverseDistance, IDistance
    {
        Gaussian gaussian;
        Linear linear;
        double[] coefficients;

        /// <summary>
        ///   Gets or sets the approximation degree 
        ///   for this kernel. Default is 1024.
        /// </summary>
        /// 
        public int Degree
        {
            get { return coefficients.Length; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value", "Value must be higher than zero.");
                createCoefficients(value);
            }
        }

        /// <summary>
        ///   Gets or sets the Gaussian kernel being
        ///   approximated by this Taylor expansion.
        /// </summary>
        /// 
        public Gaussian Gaussian
        {
            get { return gaussian; }
            set { gaussian = value; }
        }

        /// <summary>
        ///   Constructs a new <see cref="TaylorGaussian"/> kernel with the given sigma.
        /// </summary>
        /// 
        /// <param name="sigma">The kernel's sigma parameter.</param>
        /// <param name="degree">The Gaussian approximation degree. Default is 1024.</param>
        /// 
        public TaylorGaussian(double sigma, int degree = 1024)
            : this(new Gaussian(sigma), degree)
        {
        }

        /// <summary>
        ///   Constructs a new <see cref="TaylorGaussian"/> kernel with the given sigma.
        /// </summary>
        /// 
        /// <param name="gaussian">The original Gaussian kernel to be approximated.</param>
        /// <param name="degree">The Gaussian approximation degree. Default is 1024.</param>
        /// 
        public TaylorGaussian(Gaussian gaussian, int degree = 1024)
        {
            this.gaussian = gaussian;
            this.coefficients = new double[degree];
            this.linear = new Linear();
            this.createCoefficients(degree);
        }

        /// <summary>
        ///   Gaussian Kernel function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// 
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double[] x, double[] y)
        {
            return linear.Function(x, Transform(y));
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
            // http://epubs.siam.org/doi/pdf/10.1137/1.9781611972818.19 

            double[] features = new double[coefficients.Length];

            features[0] = 1;

            for (int index = 1, k = 0; index < coefficients.Length; k++)
            {
                double alpha = coefficients[k];

                foreach (int[] s in Combinatorics.Sequences(input.Length, k + 1))
                {
                    double prod = 1.0;
                    for (int i = 0; i < s.Length; i++)
                        prod *= input[s[i]];

                    features[index++] = alpha * prod;
                    if (index >= coefficients.Length)
                        break;
                }
            }

            double norm = Norm.SquareEuclidean(input);
            double constant = Math.Exp(-gaussian.Gamma * norm);

            for (int i = 0; i < features.Length; i++)
                features[i] *= constant;

            return features;
        }

        /// <summary>
        ///   Projects a set of input points into feature space.
        /// </summary>
        /// 
        /// <param name="inputs">The input points to be projected into feature space.</param>
        /// 
        /// <returns>
        ///   The feature space representation of the given <paramref name="inputs"/> points.
        /// </returns>
        /// 
        public double[][] Transform(double[][] inputs)
        {
            double[][] r = new double[inputs.Length][];
            for (int i = 0; i < inputs.Length; i++)
                r[i] = Transform(inputs[i]);
            return r;
        }


        private void createCoefficients(int degree)
        {
            coefficients = new double[degree];
            for (int i = 0; i < coefficients.Length; i++)
                coefficients[i] = Math.Sqrt(Math.Pow(2 * gaussian.Gamma, i + 1) / Special.Factorial(i + 1));
        }


        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new TaylorGaussian(gaussian, Degree);
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
            linear.Product(a, Transform(b), result);
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
            return linear.Compress(weights, supportVectors.Apply(Transform), out c);
        }

        /// <summary>
        /// Computes the squared distance in input space
        /// between two points given in feature space.
        /// </summary>
        /// <param name="x">Vector <c>x</c> in feature (kernel) space.</param>
        /// <param name="y">Vector <c>y</c> in feature (kernel) space.</param>
        /// <returns>
        /// Squared distance between <c>x</c> and <c>y</c> in input space.
        /// </returns>
        public double ReverseDistance(double[] x, double[] y)
        {
            return gaussian.ReverseDistance(x, y);
        }

        /// <summary>
        /// Computes the distance <c>d(x,y)</c> between points
        /// <paramref name="x" /> and <paramref name="y" />.
        /// </summary>
        /// <param name="x">The first point <c>x</c>.</param>
        /// <param name="y">The second point <c>y</c>.</param>
        /// <returns>
        /// A double-precision value representing the distance <c>d(x,y)</c>
        /// between <paramref name="x" /> and <paramref name="y" /> according
        /// to the distance function implemented by this class.
        /// </returns>
        public double Distance(double[] x, double[] y)
        {
            return linear.Distance(Transform(x), Transform(y));
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
        public void Add(double[] a, double[] b, double[] result)
        {
            linear.Add(Transform(a), Transform(b), result);
        }

        /// <summary>
        ///   Gets the number of parameters in the input vectors.
        /// </summary>
        /// 
        public int GetLength(double[][] inputs)
        {
            return coefficients.Length;
        }

        /// <summary>
        ///   Creates an input vector from the given double values.
        /// </summary>
        /// 
        public double[] CreateVector(double[] values)
        {
            return Vector.Create(values);
        }

        ///// <summary>
        /////   Creates an input vector with the given dimensions.
        ///// </summary>
        ///// 
        //public double[] CreateVector(int dimensions)
        //{
        //    return new double[dimensions];
        //}

        /// <summary>
        ///   Elementwise multiplication of vector a and vector b, accumulating in result.
        /// </summary>
        /// 
        /// <param name="a">The vector to be multiplied.</param>
        /// <param name="b">The vector to be multiplied.</param>
        /// <param name="accumulate">An array to store the result.</param>
        /// 
        public void Product(double[] a, double[] b, double[] accumulate)
        {
            for (int i = 0; i < a.Length; i++)
                accumulate[i] += a[i] * b[i];
        }

        /// <summary>
        ///   Converts the input vectors to a double-precision representation.
        /// </summary>
        /// 
        public double[][] ToDouble(double[][] input)
        {
            return input;
        }
    }
}
