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
    ///   Polynomial Kernel.
    /// </summary>
    /// 
    [Serializable]
    public struct Polynomial : IKernel, IDistance,
        IReverseDistance, ICloneable, ITransform,
        IKernel<Sparse<double>>, IDistance<Sparse<double>>
    {
        private int degree;
        private double constant;

        /// <summary>
        ///   Constructs a new Polynomial kernel of a given degree.
        /// </summary>
        /// 
        /// <param name="degree">The polynomial degree for this kernel.</param>
        /// <param name="constant">The polynomial constant for this kernel. Default is 1.</param>
        /// 
        public Polynomial(int degree, double constant)
        {
            this.degree = degree;
            this.constant = constant;
        }

        /// <summary>
        ///   Constructs a new Polynomial kernel of a given degree.
        /// </summary>
        /// 
        /// <param name="degree">The polynomial degree for this kernel.</param>
        /// 
        public Polynomial(int degree)
            : this(degree, 1.0) { }

        /// <summary>
        ///   Gets or sets the kernel's polynomial degree.
        /// </summary>
        /// 
        public int Degree
        {
            get { return degree; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value", "Degree must be positive.");

                degree = value;
            }
        }

        /// <summary>
        ///   Gets or sets the kernel's polynomial constant term.
        /// </summary>
        /// 
        public double Constant
        {
            get { return constant; }
            set { constant = value; }
        }


        /// <summary>
        ///   Polynomial kernel function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double[] x, double[] y)
        {
            double sum = constant;
            for (int i = 0; i < x.Length; i++)
                sum += x[i] * y[i];

            return Math.Pow(sum, degree);
        }

        /// <summary>
        ///   Polynomial kernel function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(Sparse<double> x, Sparse<double> y)
        {
            double sum = x.Dot(y) + constant;
            return Math.Pow(sum, degree);
        }

        /// <summary>
        ///   Polynomial kernel function.
        /// </summary>
        /// 
        /// <param name="z">Distance <c>z</c> in input space.</param>
        /// 
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double z)
        {
            return Math.Pow(z + constant, degree);
        }

        /// <summary>
        ///   Computes the squared distance in feature space
        ///   between two points given in input space.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// 
        /// <returns>Squared distance between <c>x</c> and <c>y</c> in feature (kernel) space.</returns>
        /// 
        public double Distance(double[] x, double[] y)
        {
            if (x == y)
                return 0.0;

            double sumx = constant;
            double sumy = constant;
            double sum = constant;

            for (int i = 0; i < x.Length; i++)
            {
                sumx += x[i] * x[i];
                sumy += y[i] * y[i];
                sum += x[i] * y[i];
            }

            int d = degree;

            return Math.Pow(sumx, d) + Math.Pow(sumy, d) - 2 * Math.Pow(sum, d);
        }

        /// <summary>
        ///   Computes the squared distance in feature space
        ///   between two points given in input space.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// 
        /// <returns>Squared distance between <c>x</c> and <c>y</c> in feature (kernel) space.</returns>
        /// 
        public double Distance(Sparse<double> x, Sparse<double> y)
        {
            if (x == y)
                return 0.0;

            double sumx = constant + x.Dot(x);
            double sumy = constant + y.Dot(y);
            double sum = constant + x.Dot(y);

            int d = degree;

            return Math.Pow(sumx, d) + Math.Pow(sumy, d) - 2 * Math.Pow(sum, d);
        }

        /// <summary>
        ///   Computes the distance in input space
        ///   between two points given in feature space.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in feature (kernel) space.</param>
        /// <param name="y">Vector <c>y</c> in feature (kernel) space.</param>
        /// <returns>Distance between <c>x</c> and <c>y</c> in input space.</returns>
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

            double q = 1.0 / degree;

            return Math.Pow(sumx, q) + Math.Pow(sumy, q) - 2.0 * Math.Pow(sum, q);
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
            switch (degree)
            {
                case 1:
                    return Linear.Transform(input, constant);

                case 2:
                    return Quadratic.Transform(input, constant);

                default:
                    return Transform(input, degree, constant);
            }
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

        /// <summary>
        ///   Projects an input point into feature space.
        /// </summary>
        /// 
        /// <param name="input">The input point to be projected into feature space.</param>
        /// <param name="constant">The <see cref="Constant"/> parameter of the kernel.</param>
        /// <param name="degree">The <see cref="Degree"/> parameter of the kernel.</param>
        /// 
        /// <returns>
        ///   The feature space representation of the given <paramref name="input"/> point.
        /// </returns>
        /// 
        public static double[] Transform(double[] input, int degree, double constant)
        {
            if (constant != 0)
            {
                throw new NotSupportedException(
                    "The explicit feature-space projection function for degrees "
                    + " higher than two is only available for homogeneous kernels"
                    + " (i.e. kernel functions with the constant term set to zero).");
            }

            int n = input.Length;
            int m = (int)Math.Pow(n, degree);

            double[] features = new double[m];

            int index = 0;
            foreach (int[] s in Combinatorics.Sequences(input.Length, degree))
            {
                double prod = 1;
                for (int i = 0; i < s.Length; i++)
                    prod *= input[s[i]];
                features[index++] = prod;
            }

            return features;
        }

    }
}
